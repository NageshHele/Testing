using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Engine
{
    class clsSocketSender
    {
        private readonly Socket socket_PrimeHeartBeat = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private const int BUFFER_SIZE = 2048;
        private readonly byte[] buffer = new byte[BUFFER_SIZE];
        List<string> clientIdList = new List<string>();
        ConcurrentDictionary<string, Socket> clientSockets = new ConcurrentDictionary<string, Socket>();

        public void SetupServer(IPAddress ip_EngineServer, int PrimeHeartBeatPort)
        {
            try
            {
                //changed on 21DEC2020 by Amey
                socket_PrimeHeartBeat.Bind(new IPEndPoint(ip_EngineServer, PrimeHeartBeatPort));

                //socket_PrimeHeartBeat.Bind(new IPEndPoint(IPAddress.Any, PrimeHeartBeatPort));
                socket_PrimeHeartBeat.Listen(0);
                socket_PrimeHeartBeat.BeginAccept(AcceptCallback, null);
            }
            catch (Exception)
            { }
        }
        private void AcceptCallback(IAsyncResult AR)
        {
            try
            {
                Socket socket;
                try
                {
                    socket = socket_PrimeHeartBeat.EndAccept(AR);
                }
                catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
                {
                    return;
                }
                socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
                socket_PrimeHeartBeat.BeginAccept(AcceptCallback, null);
            }
            catch (Exception)
            { }
        }
        //Random r = new Random();
        //string separator = "^";
        //string endOfString = "<EOF>";
        public void SendToPrimeViaSocket(string prmValue)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(prmValue + "|");
                foreach (string clientId in clientIdList)
                {
                    Socket clientSocket;
                    Boolean clientSocketPresent = clientSockets.TryGetValue(clientId, out clientSocket);
                    if (clientSocketPresent)
                    {
                        if (clientSocket.Connected)
                        {
                            lock (clientSocket)
                            {
                                clientSocket.Send(buffer, buffer.Length, SocketFlags.None);
                            }
                        }
                        else
                        {
                            Socket s;
                            clientSockets.TryRemove(clientId, out s);
                        }
                    }
                }
            }
            catch (Exception)
            { }
        }
        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            StringBuilder sb = new StringBuilder();
            try
            {
                received = current.EndReceive(AR);
                if (received > 0)
                {
                    byte[] recBuf = new byte[received];
                    Array.Copy(buffer, recBuf, received);
                    string text = Encoding.ASCII.GetString(recBuf);
                    if (text.IndexOf("<EOF>") > -1)
                    {
                      text= text.Substring(1, text.LastIndexOf('^') - 2);
                        clientIdList.Add(text);
                        clientSockets.TryAdd(text, current);
                    }
                }
                current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
            }
            catch (SocketException)
            {
                current.Close();
            }
        }
    }
}
