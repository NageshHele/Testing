using NerveLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class SocketServer
{
    private readonly Socket _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private const int BUFFER_SIZE = 2048;
    private readonly byte[] arr_Buffer = new byte[BUFFER_SIZE];

    private static NerveLogger _logger;


    internal void Setup(NerveLogger logger)
    {
        try
        {
            _logger = logger; //new NerveLogger(true, true, false, "BSEFO_Feed");
            //_logger.Initialize();

            var _Port = Convert.ToInt32(CommonMethods.GetFromConfig("SELF-SERVER", "PORT"));
            var _IP = IPAddress.Parse(CommonMethods.GetFromConfig("SELF-SERVER", "IP").ToString());
            _Socket.Bind(new IPEndPoint(_IP, _Port));
            _Socket.Listen(0);
            _Socket.BeginAccept(AcceptCallback, null);

            var _Text = $"Server Setup Success on {_Port} Port.";
            CommonMethods.ConsoleWrite(_Text);
            _logger.Debug(_Text);
        }
        catch (Exception ex) { _logger.Error(ex); }
    }

    private void AcceptCallback(IAsyncResult AR)
    {
        Socket socket;

        try
        {
            socket = _Socket.EndAccept(AR);

            //changed Timeout to 1Second on 25MAR2021 by Amey
            //added on 20JAN2021 by Amey
            socket.SendTimeout = 1000;
        }
        catch (ObjectDisposedException) { return; }

        socket.BeginReceive(arr_Buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveReqeust, socket);
        _Socket.BeginAccept(AcceptCallback, null);
    }

    private void ReceiveReqeust(IAsyncResult AR)
    {

        NerveLogger _logger = new NerveLogger(true, true, false, "");
        _logger.Initialize();

        Socket _Socket = (Socket)AR.AsyncState;
        int received;

        IPEndPoint remoteIpEndPoint = _Socket.RemoteEndPoint as IPEndPoint;

        string EOF = "<EOF>";
        int _Token = 0;

        string PreviousData = string.Empty;
        string Username = "";

        try
        {
            received = _Socket.EndReceive(AR);
            if (received > 0)
            {
                byte[] receivedBytes = new byte[received];
                Array.Copy(arr_Buffer, receivedBytes, received);
                string receivedText = Encoding.UTF8.GetString(receivedBytes).ToUpper();
                _logger.Debug("Received Text: " + receivedText);

                PreviousData += receivedText;
                while (PreviousData.Contains(EOF))
                {
                    string ProperData = PreviousData.Substring(0, PreviousData.IndexOf(EOF) - 1);
                    PreviousData = PreviousData.Substring(PreviousData.IndexOf(EOF) + EOF.Length);

                    string[] arr_Fields = ProperData.ToUpper().Split('^').Select(v => v.Trim()).ToArray();

                    switch (arr_Fields[0])
                    {
                        case "CONNECT":
                            //Format : CONNECT^ID^<EOF>

                            if (GlobalCollections.dict_ConnectionInfo.ContainsKey(arr_Fields[1]))
                            {
                                var _ConnectionInfo = GlobalCollections.dict_ConnectionInfo[arr_Fields[1]];

                                _ConnectionInfo.IsConnected = true;
                                _ConnectionInfo.Subscribed = 0;
                            }
                            else
                            {
                                GlobalCollections.dict_ConnectionInfo.TryAdd(arr_Fields[1], new ConnectionInfo()
                                {
                                    Username =arr_Fields[1],
                                    IP = remoteIpEndPoint.Address,
                                    PORT = remoteIpEndPoint.Port
                                });
                            }                            

                            break;
                        case "UNSUBSCRIBE":
                            //UNSUBSCRIBE^TOKEN^ID^<EOF>

                            _Token = Convert.ToInt32(arr_Fields[1]);

                            if (GlobalCollections.dict_ConnectionInfo.ContainsKey(arr_Fields[2]))
                            {
                                GlobalCollections.dict_ConnectionInfo[arr_Fields[2]].Subscribed -= 1;

                                if (GlobalCollections.dict_SubscribedClients.ContainsKey(_Token))
                                    GlobalCollections.dict_SubscribedClients[_Token].Remove(_Socket);
                            }

                            break;
                        case "SUBSCRIBE":
                            //SUBSCRIBE^TOKEN^ID^<EOF>

                            _Token = Convert.ToInt32(arr_Fields[1]);

                            if (GlobalCollections.dict_ConnectionInfo.ContainsKey(arr_Fields[2]))
                            {
                                GlobalCollections.dict_ConnectionInfo[arr_Fields[2]].Subscribed += 1;

                                if (GlobalCollections.dict_SubscribedClients.ContainsKey(_Token))
                                    GlobalCollections.dict_SubscribedClients[_Token].Add(_Socket);
                                else
                                    GlobalCollections.dict_SubscribedClients.TryAdd(_Token, new List<Socket>() { _Socket });
                            }
                            break;
                    }
                }
            }


            _Socket.BeginReceive(arr_Buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveReqeust, _Socket);

        }
        catch (Exception ee)
        {
            _logger.Error(ee);

            try
            {
                ConnectionInfo _ConnectionInfo = GlobalCollections.dict_ConnectionInfo.Values.Where(v => v.IP == remoteIpEndPoint.Address && v.PORT == remoteIpEndPoint.Port).FirstOrDefault();
               
                if (_ConnectionInfo != null)
                    _ConnectionInfo.IsConnected = false;
            }
            catch (Exception) { }
        }
    }
}