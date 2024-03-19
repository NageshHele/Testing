using n.Structs;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Engine
{
    public delegate void del_PrimeConnected(string Username, IPAddress IP_Prime, bool isConnected);

    class PrimeDataConnector
    {
        public event del_PrimeConnected eve_PrimeTradeConnected;
        public event del_PrimeConnected eve_PrimeSpanConnected;

        private readonly Socket socket_TradeData = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly Socket socket_SpanData = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int BUFFER_SIZE = 2048;
        //private readonly byte[] Trade_buffer = new byte[BUFFER_SIZE];
        //private readonly byte[] Span_buffer = new byte[BUFFER_SIZE];

        /// <summary>
        /// Key : Username-Prime | Value : Socket
        /// </summary>
        ConcurrentDictionary<string, Socket> dict_ConnectedPrime = new ConcurrentDictionary<string, Socket>();

        /// <summary>
        /// Key : Username-Prime | Value : Socket
        /// </summary>
        ConcurrentDictionary<string, Socket> dict_ConnectedPrimeSpan = new ConcurrentDictionary<string, Socket>();

        /// <summary>
        /// Key : Username | Value : true if sending is complete, false if still compressing and sending.
        /// </summary>
        ConcurrentDictionary<string, bool> dict_IsPosSent = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Key : Username | Value : true if sending is complete, false if still compressing and sending.
        /// </summary>
        ConcurrentDictionary<string, bool> dict_IsSpanSent = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Key : Prime-Username | Value : Mapped Clients
        /// </summary>
        ConcurrentDictionary<string, string[]> dict_UserMappedInfo = new ConcurrentDictionary<string, string[]>();

        int _InstancesAllowed = 0;

        clsWriteLog _logger;
        PrimeHBConnector _HeartBeatServer;

        int _TimeoutMilliseconds = 120000;

        static Dictionary<string, List<ConsolidatedPositionInfo>> dict_Positions = new Dictionary<string, List<ConsolidatedPositionInfo>>();
        static ConcurrentDictionary<string, double[]> dict_MarginData = new ConcurrentDictionary<string, double[]>();

        byte[] arr_EOFBytes = Encoding.UTF8.GetBytes("<EOF>");

        public void SetupServer(IPAddress ip_EngineServer, int TradeDataPort, int SpanDataPort, int MaxAllowedInstances, string LogPath, clsWriteLog _logger,
            PrimeHBConnector _HeartBeatServer, int _TimeoutMilliseconds)
        {
            try
            {
                this._logger = _logger;
                this._HeartBeatServer = _HeartBeatServer;
                this._TimeoutMilliseconds = _TimeoutMilliseconds;

                _InstancesAllowed = MaxAllowedInstances;

                //changed to ConfigIP on 21DEC2020 by Amey
                socket_TradeData.Bind(new IPEndPoint(ip_EngineServer, TradeDataPort));

                //serverSocket.Bind(new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(
                //f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork), PrimeDataPort));//new IPEndPoint(IPAddress.Any, primePort));
                socket_TradeData.Listen(0);
                socket_TradeData.BeginAccept(AcceptCallbackPrime, null);

                //changed to ConfigIP on 21DEC2020 by Amey
                socket_SpanData.Bind(new IPEndPoint(ip_EngineServer, SpanDataPort));

                //socket_SpanData.Bind(new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(
                //f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork), SpanDataPort));
                socket_SpanData.Listen(0);
                socket_SpanData.BeginAccept(AcceptCallbackSpan, null);
            }
            catch (Exception ee)
            {
                _logger.WriteLog("Initialising server " + ee.ToString());
            }
        }

        //To update Dict after changing User/Client info in DB. 09MAR2021-Amey
        public void UpdateCollections(ConcurrentDictionary<string, string[]> dict_UserMappedInfo)
        {
            //added on 17FEB2021 by Amey
            this.dict_UserMappedInfo = dict_UserMappedInfo;
        }

        void SendConnectionResponse(string prmMessage, Socket sckt)
        {
            try
            {
                byte[] buffer = CompressAndEncryptData(JsonConvert.SerializeObject(prmMessage), "SendconnectionRequest");


                TcpClient tcpClientResponseSocket = new TcpClient(AddressFamily.InterNetwork);
                try
                {
                    sckt.Send(buffer, buffer.Length, SocketFlags.None);
                }
                catch (Exception listner)
                {
                    _logger.WriteLog("Sender " + listner.ToString());
                }
            }
            catch (Exception sendEx)
            {
                _logger.WriteLog("SendConnectionResponse " + sendEx.ToString());
            }
        }

        private void AcceptCallbackPrime(IAsyncResult AR)
        {
            try
            {
                Socket soc_Current = socket_TradeData.EndAccept(AR);

                //added on 10APR2021 by Amey
                Task.Run(() => ReceiveCallbackTrade(soc_Current));

                //soc_Current.BeginReceive(Trade_buffer, 0, 2048, SocketFlags.None, ReceiveCallbackTrade, soc_Current);
            }
            catch (ObjectDisposedException ee) { _logger.WriteLog("AcceptCallbackPrime Disposed : " + ee); return; }
            catch (Exception ee) { _logger.WriteLog("AcceptCallbackPrime : " + ee); }

            socket_TradeData.BeginAccept(AcceptCallbackPrime, null);
        }

        private void AcceptCallbackSpan(IAsyncResult AR)
        {
            try
            {
                Socket soc_Current = socket_SpanData.EndAccept(AR);

                //added on 10APR2021 by Amey
                Task.Run(() => ReceiveCallbackSpan(soc_Current));

                //socket.BeginReceive(Span_buffer, 0, 2048, SocketFlags.None, ReceiveCallbackSpan, socket);
            }
            catch (ObjectDisposedException ee) { _logger.WriteLog("AcceptCallbackSpan Disposed : " + ee); return; }
            catch (Exception ee) { _logger.WriteLog("AcceptCallbackSpan : " + ee); }

            socket_SpanData.BeginAccept(AcceptCallbackSpan, null);
        }

        private void ReceiveCallbackTrade(Socket soc_Current)
        {
            byte[] arr_Buffer = new byte[2048];
            byte[] arr_BytesReceived;

            var isConnected = true;

            string EOF = "<EOF>";
            string ProperData = string.Empty;
            string PreviousData = string.Empty;

            int _ReceivedBytesLength;
            int EOFIndex = 0;
            int EOFLength = EOF.Length;

            while (isConnected)
            {
                try
                {
                    _ReceivedBytesLength = soc_Current.Receive(arr_Buffer, SocketFlags.None);

                    if (_ReceivedBytesLength > 0)
                    {
                        arr_BytesReceived = new byte[_ReceivedBytesLength];
                        Array.Copy(arr_Buffer, arr_BytesReceived, _ReceivedBytesLength);

                        PreviousData += Encoding.ASCII.GetString(arr_BytesReceived);

                        while ((EOFIndex = PreviousData.IndexOf(EOF)) >= 0)
                        {
                            //added on 03MAY2021 by Amey
                            //To avoid "System.ArgumentOutOfRangeException: Length cannot be less than zero." exception.
                            //EOFIndex = PreviousDataSpan.IndexOf(EOF);
                            while (EOFIndex == 0)
                            {
                                PreviousData = PreviousData.Substring(EOFIndex + EOFLength);
                                EOFIndex = PreviousData.IndexOf(EOF);
                            }

                            if (EOFIndex < 0)
                                continue;

                            ProperData = PreviousData.Substring(0, EOFIndex - 1);
                            PreviousData = PreviousData.Substring(EOFIndex + EOF.Length);

                            //changed on 17FEB2021 by Amey
                            var arr_Fields = ProperData.Split('^');
                            if (arr_Fields[0] == "ID")
                            {
                                if (arr_Fields[1] == "TRADE")
                                {
                                    //changed on 17FEB2021 by Amey
                                    if (dict_ConnectedPrime.Count < _InstancesAllowed)
                                    {
                                        string Username = arr_Fields[2].ToLower();
                                        //added on 12MAY2021 by Amey
                                        if (dict_ConnectedPrime.TryGetValue(Username, out Socket _Socket))
                                        {
                                            try
                                            {
                                                if (!_Socket.Connected)
                                                    CloseAndRemoveConnections(Username);
                                            }
                                            catch (Exception ee) { _logger.WriteLog("Trade Socket Available  : " + ee); CloseAndRemoveConnections(Username); }
                                        }

                                        if (!dict_ConnectedPrime.ContainsKey(Username))
                                        {
                                            dict_ConnectedPrime.TryAdd(Username, soc_Current);
                                            _logger.WriteLog("IP added to list " + Username.ToUpper() + "|" + soc_Current.RemoteEndPoint.ToString());

                                            //added on 10JUN2021 by Amey
                                            if (dict_IsPosSent.ContainsKey(Username))
                                                dict_IsPosSent[Username] = true;
                                            else
                                                dict_IsPosSent.TryAdd(Username, true);

                                            //added on 15MAR2021 by Amey
                                            eve_PrimeTradeConnected(Username.ToUpper(), ((IPEndPoint)soc_Current.RemoteEndPoint).Address, true);

                                            //changed location on 10MAY2021 by Amey
                                            //added on 07MAY2021 by Amey
                                            soc_Current.SendTimeout = _TimeoutMilliseconds;

                                            isConnected = false;
                                        }
                                    }
                                    else
                                    {
                                        isConnected = false;
                                        SendConnectionResponse("Max user limit exceeded, not able to register ", soc_Current);
                                        _logger.WriteLog("Max user limit exceeded, not able to register " + arr_Fields[2].ToUpper() + "|" + soc_Current.RemoteEndPoint.ToString());

                                        Thread.Sleep(100);
                                        soc_Current.Close();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (SocketException)
                {
                    isConnected = false;

                    try
                    {
                        //added on 16MAR2021 by Amey
                        var _Username = dict_ConnectedPrime.Where(v => v.Value == soc_Current).First().Key;

                        //added on 16MAR2021 by Amey
                        eve_PrimeTradeConnected(_Username.ToUpper(), ((IPEndPoint)soc_Current.RemoteEndPoint).Address, false);

                        CloseAndRemoveConnections(_Username);
                    }
                    catch (Exception) { }

                    soc_Current.Close();
                }
                catch (Exception ee) { _logger.WriteLog("ReceiveCallBack Trade : " + ee); isConnected = false; }
            }
        }

        private void ReceiveCallbackSpan(Socket soc_Current)
        {
            byte[] arr_Buffer = new byte[2048];
            byte[] arr_BytesReceived;

            var isConnected = true;

            string EOF = "<EOF>";
            string ProperData = string.Empty;
            string PreviousData = string.Empty;

            int _ReceivedBytesLength;
            int EOFIndex = 0;
            int EOFLength = EOF.Length;

            while (isConnected)
            {
                try
                {
                    _ReceivedBytesLength = soc_Current.Receive(arr_Buffer, SocketFlags.None);

                    if (_ReceivedBytesLength > 0)
                    {
                        arr_BytesReceived = new byte[_ReceivedBytesLength];
                        Array.Copy(arr_Buffer, arr_BytesReceived, _ReceivedBytesLength);

                        PreviousData += Encoding.ASCII.GetString(arr_BytesReceived);

                        while ((EOFIndex = PreviousData.IndexOf(EOF)) >= 0)
                        {
                            //added on 03MAY2021 by Amey
                            //To avoid "System.ArgumentOutOfRangeException: Length cannot be less than zero." exception.
                            //EOFIndex = PreviousDataSpan.IndexOf(EOF);
                            while (EOFIndex == 0)
                            {
                                PreviousData = PreviousData.Substring(EOFIndex + EOFLength);
                                EOFIndex = PreviousData.IndexOf(EOF);
                            }

                            if (EOFIndex < 0)
                                continue;

                            ProperData = PreviousData.Substring(0, EOFIndex - 1);
                            PreviousData = PreviousData.Substring(EOFIndex + EOF.Length);

                            //changed on 17FEB2021 by Amey
                            var arr_Fields = ProperData.Split('^');
                            if (arr_Fields[0] == "ID")
                            {
                                if (arr_Fields[1] == "SPAN")
                                {
                                    string Username = arr_Fields[2].ToLower();
                                    //changed on 17FEB2021 by Amey
                                    if (dict_ConnectedPrimeSpan.Count < _InstancesAllowed)
                                    {
                                        //added on 12MAY2021 by Amey
                                        if (dict_ConnectedPrimeSpan.TryGetValue(Username, out Socket _Socket))
                                        {
                                            try
                                            {
                                                if (!_Socket.Connected)
                                                    CloseAndRemoveConnections(Username);
                                            }
                                            catch (Exception ee) { _logger.WriteLog("Span Socket Available  : " + ee); CloseAndRemoveConnections(Username); }
                                        }

                                        if (!dict_ConnectedPrimeSpan.ContainsKey(Username))
                                        {
                                            dict_ConnectedPrimeSpan.TryAdd(Username, soc_Current);
                                            _logger.WriteLog("IP added to Span list " + Username.ToUpper() + "|" + soc_Current.RemoteEndPoint.ToString());

                                            //added on 10JUN2021 by Amey
                                            if (dict_IsSpanSent.ContainsKey(Username))
                                                dict_IsSpanSent[Username] = true;
                                            else
                                                dict_IsSpanSent.TryAdd(Username, true);

                                            //added on 15MAR2021 by Amey
                                            eve_PrimeSpanConnected(Username.ToUpper(), ((IPEndPoint)soc_Current.RemoteEndPoint).Address, true);

                                            //changed location on 10MAY2021 by Amey
                                            //added on 07MAY2021 by Amey
                                            soc_Current.SendTimeout = _TimeoutMilliseconds;

                                            isConnected = false;
                                        }
                                    }
                                    else
                                    {
                                        isConnected = false;
                                        SendConnectionResponse("Max user limit exceeded, not able to register ", soc_Current);
                                        _logger.WriteLog("Max user limit exceeded, not able to register SPAN " + arr_Fields[2].ToUpper() + "|" + soc_Current.RemoteEndPoint.ToString());

                                        Thread.Sleep(100);
                                        soc_Current.Close();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (SocketException ee)
                {
                    isConnected = false;

                    try
                    {
                        //added on 16MAR2021 by Amey
                        var _Username = dict_ConnectedPrimeSpan.Where(v => v.Value == soc_Current).First().Key;

                        //added on 16MAR2021 by Amey
                        eve_PrimeSpanConnected(_Username.ToUpper(), ((IPEndPoint)soc_Current.RemoteEndPoint).Address, false);

                        CloseAndRemoveConnections(_Username);
                    }
                    catch (Exception) { }

                    soc_Current.Close();
                }
                catch (Exception ee) { _logger.WriteLog("ReceiveCallBack Span : " + ee); isConnected = false; }
            }
        }

        public void SendTradeData(Dictionary<string, List<ConsolidatedPositionInfo>> dict_AllPosition)
        {
            try
            {
                var list_ConnectedTradeUsers = dict_ConnectedPrime.Keys.ToList();
                dict_Positions = new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_AllPosition);

                foreach (var Username in list_ConnectedTradeUsers)
                {
                    if (dict_IsPosSent[Username] && dict_UserMappedInfo.TryGetValue(Username, out string[] arr_MappedClients))
                    {
                        dict_IsPosSent[Username] = false;
                        var dict_Filtered = dict_Positions.Where(x => arr_MappedClients.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
                        //byte[] buffer = SerializeCompressTrades(dict_Filtered);
                        Task.Run(() => SendTradeToPrime(Username.ToString(), dict_Filtered));
                    }
                }
            }
            catch (Exception load)
            {
                _logger.WriteLog("send request " + load.ToString());
            }
        }

        //added isEODSpan param on 27NOV2020 by Amey
        public void SendSpanData(Dictionary<string, double[]> dict_AllMarginData)
        {
            try
            {
                dict_MarginData = new ConcurrentDictionary<string, double[]>(dict_AllMarginData);

                if (dict_MarginData.Any())
                {
                    var list_ConnectedSpanUsers = dict_ConnectedPrimeSpan.Keys.ToList();
                    foreach (var Username in list_ConnectedSpanUsers)
                    {

                        if (dict_IsSpanSent[Username] && dict_UserMappedInfo.TryGetValue(Username, out string[] arr_MappedClients))
                        {
                            dict_IsSpanSent[Username] = false;

                            Task.Run(() => SendSpanToPrime(Username.ToString()));
                        }

                    }
                }
            }
            catch (Exception load)
            {
                _logger.WriteLog("Send span data " + load.ToString());
            }
        }

        private void SendTradeToPrime(string Username, Dictionary<string, List<ConsolidatedPositionInfo>> dict_AllPositions)
        {
            try
            {
                if (dict_ConnectedPrime.TryGetValue(Username, out Socket _Socket))
                {

                    byte[] buffer = SerializeCompressTrades(dict_AllPositions);

                    _Socket.Send(buffer, buffer.Length, SocketFlags.None);

                    _logger.WriteLog($"Username:- {Username}, ByteLength:- {buffer.Length}");
                }

            }
            catch (Exception ee)
            {
                CloseAndRemoveConnections(Username);

                _logger.WriteLog("SendTradeData Removed " + Username + " _ " + ee);
            }

            dict_IsPosSent[Username] = true;
        }



        private void SendSpanToPrime(string Username)
        {
            try
            {
                Socket _Socket;
                byte[] buffer;

                if (dict_ConnectedPrimeSpan.TryGetValue(Username, out _Socket) && dict_UserMappedInfo.TryGetValue(Username, out string[] arr_MappedClients))
                {

                    var dict_Filtered = dict_MarginData.Where(x => arr_MappedClients.Contains(x.Key.Split('_')[0])).ToDictionary(x => x.Key, x => x.Value);

                    buffer = SerializeCompressSpan(dict_Filtered);

                    if (buffer != null)
                    {
                        _Socket.Send(buffer, buffer.Length, SocketFlags.None);                        
                    }
                }
            }
            catch (Exception ee)
            {
                CloseAndRemoveConnections(Username);

                _logger.WriteLog("SendSpanToPrime Removed " + Username + " _ " + ee);
            }

            dict_IsSpanSent[Username] = true;
        }


        internal void CloseAndRemoveConnections(string Username)
        {
            try
            {
                if (dict_ConnectedPrime.TryGetValue(Username, out Socket _Socket))
                {
                    dict_ConnectedPrime.TryRemove(Username, out _);
                    eve_PrimeTradeConnected(Username.ToUpper(), ((IPEndPoint)_Socket.RemoteEndPoint).Address, false);
                    _Socket.Close();
                }
            }
            catch (Exception ee) { _logger.WriteLog("CloseAndRemoveConnections 1 : " + Username + Environment.NewLine + ee); }

            try
            {
                if (dict_ConnectedPrimeSpan.TryGetValue(Username, out Socket _Socket))
                {
                    dict_ConnectedPrimeSpan.TryRemove(Username, out _);
                    eve_PrimeSpanConnected(Username.ToUpper(), ((IPEndPoint)_Socket.RemoteEndPoint).Address, false);
                    _Socket.Close();
                }
            }
            catch (Exception ee) { _logger.WriteLog("CloseAndRemoveConnections 2 : " + Username + Environment.NewLine + ee); }

            try
            {
                if (_HeartBeatServer.dict_ClientSockets.TryGetValue(Username, out Socket _Socket))
                {
                    _HeartBeatServer.dict_ClientSockets.TryRemove(Username, out _);
                    _Socket.Close();
                }
            }
            catch (Exception ee) { _logger.WriteLog("CloseAndRemoveConnections HB : " + Username + Environment.NewLine + ee); }
        }

        #region Compress/Encrypt 

        private byte[] CompressAndEncryptData(string inputString, string username)
        {

            try
            {

                byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);

                using (var outputStream = new MemoryStream())
                {

                    using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
                        gZipStream.Write(inputBytes, 0, inputBytes.Length);

                    inputBytes = new byte[0];

                    return outputStream.ToArray();
                }
            }
            catch (Exception error)
            {
                _logger.WriteLog("Compression " + error.ToString());
                return null;
            }
        }

        private byte[] SerializeCompressTrades(Dictionary<string, List<ConsolidatedPositionInfo>> dict_Trades)
        {
            try
            {

                MemoryStream ms = new MemoryStream();
                using (ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, dict_Trades);
                }
                byte[] arr_compressed = QuickLZ.compress(ms.ToArray(), 1);
                //byte[] b_end = Encoding.UTF8.GetBytes("<EOF>");
                //byte[] buffer = new byte[b_end.Length + arr_compressed.Length];
                //Buffer.BlockCopy(arr_compressed, 0, buffer, 0, arr_compressed.Length);
                //Buffer.BlockCopy(b_end, 0, buffer, arr_compressed.Length, b_end.Length);

                List<byte> list_byte = new List<byte>();
                list_byte.AddRange(arr_compressed);
                list_byte.AddRange(Encoding.UTF8.GetBytes("<EOF>"));

                return list_byte.ToArray();
            }
            catch (Exception ee) {
                _logger.WriteLog(ee.ToString()); return null; }
        }

        private byte[] SerializeCompressSpan(Dictionary<string, double[]> dict_Span)
        {
            try
            {

                MemoryStream ms = new MemoryStream();
                using (ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, dict_Span);
                }
                //byte[] b_marker = Encoding.UTF8.GetBytes(marker);
                byte[] arr_CompressedBytes = QuickLZ.compress(ms.ToArray(), 1);

                ms = new MemoryStream();

                var _CompressedLength = arr_CompressedBytes.Length;
                int _FullCompressedLength = _CompressedLength + arr_EOFBytes.Length;
                byte[] arr_FullCompressedBytes = new byte[_FullCompressedLength];


                Buffer.BlockCopy(arr_CompressedBytes, 0, arr_FullCompressedBytes, 0, arr_CompressedBytes.Length);
                Buffer.BlockCopy(arr_EOFBytes, 0, arr_FullCompressedBytes, arr_CompressedBytes.Length, arr_EOFBytes.Length);


                arr_CompressedBytes = new byte[0];

                return arr_FullCompressedBytes;
            }
            catch (Exception ee) { _logger.WriteLog(ee.ToString()); return null; }
        }

        private byte[] Encrypt(byte[] input)
        {
            try
            {
                PasswordDeriveBytes pdb = new PasswordDeriveBytes("hjiweykaksd", new byte[] { 0x43, 0x87, 0x23, 0x72 });
                MemoryStream ms = new MemoryStream();
                Aes aes = new AesManaged();
                aes.Key = pdb.GetBytes(aes.KeySize / 8);
                aes.IV = pdb.GetBytes(aes.BlockSize / 8);
                CryptoStream cs = new CryptoStream(ms,
                  aes.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(input, 0, input.Length);
                cs.Close();
                return ms.ToArray();
            }
            catch (Exception ee) { _logger.WriteLog("Encrypt : " + ee); }

            return input;
        }


        #endregion
    }
}
