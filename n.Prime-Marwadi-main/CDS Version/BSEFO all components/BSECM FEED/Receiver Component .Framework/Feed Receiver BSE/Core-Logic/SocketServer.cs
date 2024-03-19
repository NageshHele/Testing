using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Feed_Receiver_BSE.Data_Structures;
using Feed_Receiver_BSE.Helper;
using NerveLog;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Feed_Receiver_BSE.Core_Logic
{
    internal class SocketServer
    {
        GridControl gc_ConnectionInfo;
        GridView gv_ConnectionInfo;

        NerveLogger _logger;

        private readonly Socket _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int BUFFER_SIZE = 2048;
        private readonly byte[] arr_Buffer = new byte[BUFFER_SIZE];

        /// <summary>
        /// Will save incomplete data.
        /// </summary>
        static string PreviousData = string.Empty;

        internal SocketServer(GridControl gc, GridView gv)
        {
            gc_ConnectionInfo = gc;
            gv_ConnectionInfo = gv;

            _logger = CollectionHelper._logger;
        }

        internal void Setup()
        {
            try
            {
                var ipAddress = IPAddress.Parse(CollectionHelper.GetFromConfig("FEEDRECEIVER-SERVER", "IP").ToString());

                _Socket.Bind(new IPEndPoint(ipAddress, Convert.ToInt32(CollectionHelper.GetFromConfig("FEEDRECEIVER-SERVER", "PORT"))));
                _Socket.Listen(0);
                _Socket.BeginAccept(AcceptCallback, null);

                _logger.Debug("Server Setup Success");
            }
            catch (Exception ee) { _logger.Error(ee); }
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

            Task.Run(() => ReceiveReqeust(socket));

            //socket.BeginReceive(arr_Buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveReqeust, socket);
            //_Socket.BeginAccept(AcceptCallback, null);
        }

        //private void _ReceiveReqeust(IAsyncResult AR)
        //{
        //    Socket _Socket = (Socket)AR.AsyncState;
        //    int received;

        //    IPEndPoint remoteIpEndPoint = _Socket.RemoteEndPoint as IPEndPoint;

        //    string EOF = "<EOF>";
        //    int _Token = 0;

        //    try
        //    {
        //        received = _Socket.EndReceive(AR);
        //        if (received > 0)
        //        {
        //            byte[] receivedBytes = new byte[received];
        //            Array.Copy(arr_Buffer, receivedBytes, received);
        //            string receivedText = Encoding.UTF8.GetString(receivedBytes).ToUpper();
        //            _logger.Debug("Received Text: " + receivedText);

        //            PreviousData += receivedText;
        //            while (PreviousData.Contains(EOF))
        //            {
        //                string ProperData = PreviousData.Substring(0, PreviousData.IndexOf(EOF) - 1);
        //                PreviousData = PreviousData.Substring(PreviousData.IndexOf(EOF) + EOF.Length);

        //                string[] arr_Fields = ProperData.ToUpper().Split('^').Select(v => v.Trim()).ToArray();

        //                ConnectionInfo _ConnectionInfo = null;
        //                lock (CollectionHelper._GridLock)
        //                {
        //                    _ConnectionInfo = CollectionHelper.bList_ConnectionInfo.Where(v => v.Username == arr_Fields[1]).FirstOrDefault();
        //                }

        //                switch (arr_Fields[0])
        //                {
        //                    case "CONNECT":
        //                        //Format : CONNECT^ID^ip_UDPReceiver^UDPReceiverPORT^<EOF>

        //                        if (_ConnectionInfo is null)
        //                        {
        //                            //added on 30APR2021 by Amey
        //                            var _RemoteEndPoint = new IPEndPoint(IPAddress.Parse(arr_Fields[2]), Convert.ToInt32(arr_Fields[3]));
        //                            var _UDPClient = new UdpClient();

        //                            if (CollectionHelper.dict_UDPClientSocket.ContainsKey(arr_Fields[1]))
        //                            {
        //                                CollectionHelper.dict_UDPClientSocket[arr_Fields[1]].RemoteEndPoint = _RemoteEndPoint;
        //                                CollectionHelper.dict_UDPClientSocket[arr_Fields[1]].Client = _UDPClient;
        //                            }
        //                            else
        //                                CollectionHelper.dict_UDPClientSocket.TryAdd(arr_Fields[1], new SocketInfo() { RemoteEndPoint = _RemoteEndPoint, Client = _UDPClient });

        //                            gc_ConnectionInfo.Invoke((MethodInvoker)(() =>
        //                            {
        //                                CollectionHelper.bList_ConnectionInfo.Add(new ConnectionInfo()
        //                                {
        //                                    Username = arr_Fields[1],
        //                                    IP = remoteIpEndPoint.Address,
        //                                    PORT = remoteIpEndPoint.Port
        //                                });

        //                                gv_ConnectionInfo.BestFitColumns();
        //                            }));
        //                        }
        //                        else
        //                        {
        //                            gc_ConnectionInfo.Invoke((MethodInvoker)(() =>
        //                            {
        //                                _ConnectionInfo.IsConnected = true;
        //                                _ConnectionInfo.Subscribed = 0;
        //                            }));
        //                        }

        //                        break;
        //                    case "UNSUBSCRIBE":
        //                        //UNSUBSCRIBE^TOKEN^ID^<EOF>

        //                        _Token = Convert.ToInt32(arr_Fields[1]);
        //                        if (_ConnectionInfo != null)
        //                        {
        //                            gc_ConnectionInfo.Invoke((MethodInvoker)(() =>
        //                            {
        //                                _ConnectionInfo.Subscribed -= 1;
        //                            }));

        //                            CollectionHelper.dict_SubscribedClients[_Token].Remove(arr_Fields[2]);
        //                        }

        //                        break;
        //                    case "SUBSCRIBE":
        //                        //SUBSCRIBE^TOKEN^ID^<EOF>

        //                        _Token = Convert.ToInt32(arr_Fields[1]);
        //                        if (CollectionHelper.dict_SubscribedClients.ContainsKey(_Token))
        //                        {
        //                            if (_ConnectionInfo != null)
        //                            {
        //                                gc_ConnectionInfo.Invoke((MethodInvoker)(() =>
        //                                {
        //                                    _ConnectionInfo.Subscribed += 1;
        //                                }));
        //                            }

        //                            CollectionHelper.dict_SubscribedClients[_Token].Add(arr_Fields[2]);

        //                            SocketSender.SendToClient(CollectionHelper.dict_UDPClientSocket[arr_Fields[2]], _Token);
        //                        }
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ee) 
        //    { 
        //        _logger.Error(ee);

        //        try
        //        {
        //            ConnectionInfo _ConnectionInfo = null;
        //            lock (CollectionHelper._GridLock)
        //            {
        //                _ConnectionInfo = CollectionHelper.bList_ConnectionInfo.Where(v => v.IP == remoteIpEndPoint.Address && v.PORT == remoteIpEndPoint.Port).FirstOrDefault();
        //            }

        //            if (_ConnectionInfo != null)
        //            {
        //                gc_ConnectionInfo.Invoke((MethodInvoker)(() =>
        //                {
        //                    _ConnectionInfo.IsConnected = false;
        //                }));
        //            }
        //        }
        //        catch (Exception) { }
        //    }
        //}
        
        private void ReceiveReqeust(Socket socket)
        {
            Socket _Socket = socket;
            int received;
            byte[] arr_Buffer = new byte[2048];
            IPEndPoint remoteIpEndPoint = _Socket.RemoteEndPoint as IPEndPoint;
            var _ReceivedBytesLength = 0;

            string EOF = "<EOF>";
            int _Token = 0;
            var connected = true;

            while (connected)
            {
                try
                {
                    _ReceivedBytesLength = _Socket.Receive(arr_Buffer, SocketFlags.None);
                    if (_ReceivedBytesLength > 0)
                    {
                        byte[] receivedBytes = new byte[_ReceivedBytesLength];
                        Array.Copy(arr_Buffer, receivedBytes, _ReceivedBytesLength);
                        string receivedText = Encoding.UTF8.GetString(receivedBytes).ToUpper();
                        _logger.Debug("Received Text: " + receivedText);

                        PreviousData += receivedText;
                        while (PreviousData.Contains(EOF))
                        {
                            string ProperData = PreviousData.Substring(0, PreviousData.IndexOf(EOF) - 1);
                            PreviousData = PreviousData.Substring(PreviousData.IndexOf(EOF) + EOF.Length);

                            string[] arr_Fields = ProperData.ToUpper().Split('^').Select(v => v.Trim()).ToArray();

                            //ConnectionInfo _ConnectionInfo = null;
                            //lock (CollectionHelper._GridLock)
                            //{
                            //    _ConnectionInfo = CollectionHelper.bList_ConnectionInfo.Where(v => v.Username == arr_Fields[1]).FirstOrDefault();
                            //}
                            ConnectionInfo _ConnectionInfo = null;
                            switch (arr_Fields[0])
                            {
                                case "CONNECT":
                                    //Format : CONNECT^ID^ip_UDPReceiver^UDPReceiverPORT^<EOF>


                                    lock (CollectionHelper._GridLock)
                                    {
                                        _ConnectionInfo = CollectionHelper.bList_ConnectionInfo.Where(v => v.Username == arr_Fields[1]).FirstOrDefault();
                                    }


                                    if (_ConnectionInfo is null)
                                    {
                                        //added on 30APR2021 by Amey
                                        var _RemoteEndPoint = new IPEndPoint(IPAddress.Parse(arr_Fields[2]), Convert.ToInt32(arr_Fields[3]));
                                        var _UDPClient = new UdpClient();

                                        if (CollectionHelper.dict_UDPClientSocket.ContainsKey(arr_Fields[1]))
                                        {
                                            CollectionHelper.dict_UDPClientSocket[arr_Fields[1]].RemoteEndPoint = _RemoteEndPoint;
                                            CollectionHelper.dict_UDPClientSocket[arr_Fields[1]].Client = _Socket;
                                        }
                                        else
                                            CollectionHelper.dict_UDPClientSocket.TryAdd(arr_Fields[1], new SocketInfo() { RemoteEndPoint = _RemoteEndPoint, Client = _Socket });

                                        gc_ConnectionInfo.Invoke((MethodInvoker)(() =>
                                        {
                                            CollectionHelper.bList_ConnectionInfo.Add(new ConnectionInfo()
                                            {
                                                Username = arr_Fields[1],
                                                IP = remoteIpEndPoint.Address,
                                                PORT = remoteIpEndPoint.Port
                                            });

                                            gv_ConnectionInfo.BestFitColumns();
                                        }));
                                    }
                                    else
                                    {
                                        gc_ConnectionInfo.Invoke((MethodInvoker)(() =>
                                        {
                                            _ConnectionInfo.IsConnected = true;
                                            _ConnectionInfo.Subscribed = 0;
                                        }));
                                    }

                                    break;
                                case "UNSUBSCRIBE":
                                    //UNSUBSCRIBE^TOKEN^ID^<EOF>

                                  
                                    lock (CollectionHelper._GridLock)
                                    {
                                        _ConnectionInfo = CollectionHelper.bList_ConnectionInfo.Where(v => v.Username == arr_Fields[2]).FirstOrDefault();
                                    }


                                    _Token = Convert.ToInt32(arr_Fields[1]);
                                    if (_ConnectionInfo != null)
                                    {
                                        gc_ConnectionInfo.Invoke((MethodInvoker)(() =>
                                        {
                                            _ConnectionInfo.Subscribed -= 1;
                                        }));

                                        CollectionHelper.dict_SubscribedClients[_Token].Remove(arr_Fields[2]);
                                    }

                                    break;
                                case "SUBSCRIBE":
                                    //SUBSCRIBE^TOKEN^ID^<EOF>

                                    lock (CollectionHelper._GridLock)
                                    {
                                        _ConnectionInfo = CollectionHelper.bList_ConnectionInfo.Where(v => v.Username == arr_Fields[2]).FirstOrDefault();
                                    }

                                    _Token = Convert.ToInt32(arr_Fields[1]);
                                    if (CollectionHelper.dict_SubscribedClients.ContainsKey(_Token))
                                    {
                                        if (_ConnectionInfo != null)
                                        {
                                            gc_ConnectionInfo.Invoke((MethodInvoker)(() =>
                                            {
                                                _ConnectionInfo.Subscribed += 1;
                                            }));
                                        }

                                        CollectionHelper.dict_SubscribedClients[_Token].Add(arr_Fields[2]);
                                        //byte[] buffer = Encoding.ASCII.GetBytes("TEST MSG" + "<EOF>");
                                        //_Socket.Send(buffer, buffer.Length, SocketFlags.None);
                                        //_Socket.Send()
                                         SocketSender.SendToClient(CollectionHelper.dict_UDPClientSocket[arr_Fields[2]], _Token);
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error(ee);

                    try
                    {
                        ConnectionInfo _ConnectionInfo = null;
                        lock (CollectionHelper._GridLock)
                        {
                            _ConnectionInfo = CollectionHelper.bList_ConnectionInfo.Where(v => v.IP == remoteIpEndPoint.Address && v.PORT == remoteIpEndPoint.Port).FirstOrDefault();
                        }

                        if (_ConnectionInfo != null)
                        {
                            gc_ConnectionInfo.Invoke((MethodInvoker)(() =>
                            {
                                _ConnectionInfo.IsConnected = false;
                            }));
                        }
                    }
                    catch (Exception) { }
                }
            }


           
        }
    }
}
