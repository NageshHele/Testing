using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Engine
{
    class clsPrimeConnector
    {

        private readonly Socket socket_PrimeData = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly Socket socket_SpanData = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        ConcurrentDictionary<Socket, string> dict_ConnectedPrime = new ConcurrentDictionary<Socket, string>();
        ConcurrentDictionary<Socket, string> dictConnectedPrimeSpan = new ConcurrentDictionary<Socket, string>();
        DataTable dt_trades = new DataTable();
        int _InstancesAllowed = 0;
        Errorlog objError;

        public void SetupServer(IPAddress ip_EngineServer, int PrimeDataPort, int SpanDataPort, int MaxAllowedInstances, string LogPath)
        {
            try
            {
                objError = new Errorlog(LogPath);
                _InstancesAllowed = MaxAllowedInstances;

                //changed to ConfigIP on 21DEC2020 by Amey
                socket_PrimeData.Bind(new IPEndPoint(ip_EngineServer, PrimeDataPort));

                //serverSocket.Bind(new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(
                //f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork), PrimeDataPort));//new IPEndPoint(IPAddress.Any, primePort));
                socket_PrimeData.Listen(0);
                socket_PrimeData.BeginAccept(AcceptCallbackPrime, null);

                //changed to ConfigIP on 21DEC2020 by Amey
                socket_SpanData.Bind(new IPEndPoint(ip_EngineServer, SpanDataPort));

                //socket_SpanData.Bind(new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(
                //f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork), SpanDataPort));
                socket_SpanData.Listen(0);
                socket_SpanData.BeginAccept(AcceptCallbackSpan, null);
            }
            catch (Exception ee)
            {
                objError.WriteLog("Initialising server " + ee.ToString());
            }
        }

        void SendConnectionResponse(string prmMessage, Socket sckt)
        {
            try
            {
                byte[] buffer = CompressData(JsonConvert.SerializeObject(prmMessage));
                TcpClient tcpClientResponseSocket = new TcpClient(AddressFamily.InterNetwork);
                try
                {
                    sckt.Send(buffer, buffer.Length, SocketFlags.None);
                }
                catch (Exception listner)
                {
                    objError.WriteLog("Sender " + listner.ToString());
                }
            }
            catch (Exception sendEx)
            {
                objError.WriteLog("SendConnectionResponse " + sendEx.ToString());
            }
        }

        private void AcceptCallbackPrime(IAsyncResult AR)
        {
            Socket socket;
            try
            {
                socket = socket_PrimeData.EndAccept(AR);
                if (dict_ConnectedPrime.Count < _InstancesAllowed)
                {
                    if (!dict_ConnectedPrime.ContainsKey(socket))
                    {
                        dict_ConnectedPrime.TryAdd(socket, socket.RemoteEndPoint.ToString());
                        objError.WriteLog("IP added to list " + socket.RemoteEndPoint.ToString());
                    }
                }
                else
                {
                    SendConnectionResponse("Max user limit exceeded, not able to register ", socket);
                    objError.WriteLog("Max user limit exceeded, not able to register " + socket.RemoteEndPoint.ToString());
                }
            }
            catch (ObjectDisposedException er) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }
            byte[] buffer = new byte[2048];
            socket.BeginReceive(buffer, 0, 2048, SocketFlags.None, ReceiveHeartbeat, socket);
            socket_PrimeData.BeginAccept(AcceptCallbackPrime, null);
        }

        private void AcceptCallbackSpan(IAsyncResult AR)
        {
            Socket socket;
            byte[] buffer = new byte[2048];
            try
            {
                socket = socket_SpanData.EndAccept(AR);
                if (dictConnectedPrimeSpan.Count < _InstancesAllowed)
                {
                    if (!dictConnectedPrimeSpan.ContainsKey(socket))
                    {
                        dictConnectedPrimeSpan.TryAdd(socket, socket.RemoteEndPoint.ToString());
                        objError.WriteLog("IP added to span list " + socket.RemoteEndPoint.ToString());
                    }
                }
                else
                {
                    SendConnectionResponse("Max user limit exceeded, not able to register ", socket);
                    objError.WriteLog("Max user limit exceeded, not able to register " + socket.RemoteEndPoint.ToString());
                }
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }
            socket.BeginReceive(buffer, 0, 2048, SocketFlags.None, ReceiveHeartbeat, socket);
            socket_SpanData.BeginAccept(AcceptCallbackSpan, null);
        }

        private void ReceiveHeartbeat(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            StringBuilder sb = new StringBuilder();
            try
            {
                byte[] buffer = new byte[2048];
                received = current.EndReceive(AR);
                try
                {
                    if (received > 0)
                    {
                        byte[] recBuf = new byte[received];
                        Array.Copy(buffer, recBuf, received);
                        string text = Encoding.ASCII.GetString(recBuf);
                        if (text == "0") { }
                    }
                }
                catch (Exception ee)
                {
                    objError.WriteLog("Hdeartbeat " + ee.ToString());
                }
                current.BeginReceive(buffer, 0, 2048, SocketFlags.None, ReceiveHeartbeat, current);
            }
            catch (SocketException error)
            {
                current.Close();
            }
        }

        //changed on 20NOV2020 by Amey
        public void SendToPrimeViaSocket(Dictionary<string, GroupsTabs.ConsolidateTradeinfoDataTable> dict_Positions)
        {
            try
            {
                byte[] buffer = CompressData(JsonConvert.SerializeObject(dict_Positions) + "<EOF>");

                for (int i = 0; i < dict_ConnectedPrime.Count; i++)
                {
                    try
                    {
                        dict_ConnectedPrime.ElementAt(i).Key.Send(buffer, buffer.Length, SocketFlags.None);
                    }
                    catch (Exception err)
                    {
                        string _ret = string.Empty;
                        if (dict_ConnectedPrime.TryRemove(dict_ConnectedPrime.ElementAt(i).Key, out _ret))
                        {
                            objError.WriteLog("Send data removed ip " + _ret + " _ " + err.ToString());
                        }
                        i--;
                    }
                }
            }
            catch (Exception load)
            {
                objError.WriteLog("send request " + load.ToString());
            }
        }

        //commented on 20NOV2020 by Amey. Unwanted!
        //public void sendJsonData()
        //{
        //    try
        //    {
        //        if (dict_ConnectedPrime.Count == 0) return;
        //        byte[] buffer = CompressData(JsonConvert.SerializeObject(dt_trades) + "<EOF>");

        //        for (int i = 0; i < dict_ConnectedPrime.Count; i++)
        //        {
        //            try
        //            {
        //                dict_ConnectedPrime.ElementAt(i).Key.Send(buffer, buffer.Length, SocketFlags.None);
        //            }
        //            catch (Exception err)
        //            {
        //                string _ret = string.Empty;
        //                if (dict_ConnectedPrime.TryRemove(dict_ConnectedPrime.ElementAt(i).Key, out _ret))
        //                {
        //                    objError.WriteLog("Send data removed ip " + _ret + " _ " + err.ToString());
        //                }
        //                i--;
        //            }
        //        }
        //    }
        //    catch (Exception load)
        //    {
        //        objError.WriteLog("Send data " + load.ToString());
        //    }
        //}

        //added isEODSpan param on 27NOV2020 by Amey
        public void SendSpanData(Dictionary<string, double[]> dict_SpanData, bool isEODSpan, bool isExpirySpan)//added by Navin on 04-12-2019   //Added isExpirySpan flag on 10-12-2020 by Akshay
        {
            try
            {
                if (dict_ConnectedPrime.Count == 0) return;

                //added flag on 27NOV2020 by Amey
                byte[] buffer = CompressData(isEODSpan + "^" + isExpirySpan + "^" + JsonConvert.SerializeObject(dict_SpanData) + "<EOF>");      //Added isExpirySpan flag on 10-12-2020 by Akshay

                for (int i = 0; i < dictConnectedPrimeSpan.Count; i++)
                {
                    try
                    {
                        dictConnectedPrimeSpan.ElementAt(i).Key.Send(buffer, buffer.Length, SocketFlags.None);
                    }
                    catch (Exception listner)
                    {
                        string _ret = string.Empty;
                        if (dictConnectedPrimeSpan.TryRemove(dictConnectedPrimeSpan.ElementAt(i).Key, out _ret))
                        {
                            objError.WriteLog("Span sender removed ip " + _ret + " _ " + listner.ToString());
                        }
                        i--;
                    }
                }
            }
            catch (Exception load)
            {
                objError.WriteLog("Send span data " + load.ToString());
            }
        }

        #region Compress/Decompress 15-05-2019 Navin
        byte[] CompressData(string inputString)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
            try
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
                        gZipStream.Write(inputBytes, 0, inputBytes.Length);
                    var outputBytes = outputStream.ToArray();
                    //var outputbase64 = Convert.ToBase64String(outputBytes);
                    return outputBytes;
                }
            }
            catch (Exception error)
            {
                objError.WriteLog("Compression " + error.ToString());
                return null;
            }
        }
        #endregion
    }
    public class Errorlog
    {
        #region Errorlog
        static StreamWriter fs;
        public Errorlog(string path)
        {
            try
            {
                path = path + "\\Dll";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string filename = path + "\\" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".txt";
                if (!File.Exists(filename))
                {
                    using (StreamWriter w = File.CreateText(filename))
                        w.Close();
                }
                if (fs == null)
                    fs = File.AppendText(filename);
            }
            catch (Exception)
            { }
        }
        public void WriteLog(string message)
        {
            try
            {
                fs.WriteAsync(message + ", " + DateTime.Now.ToString("hh.mm.ss.ffffff") + Environment.NewLine);
                fs.Flush();
            }
            catch (Exception)
            {
                //fs.Close();
            }
        }
        #endregion
    }
}
