using n.Structs;
using NerveLog;
using Newtonsoft.Json;
using Prime.Helper;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prime
{
    delegate void del_SpanData(Dictionary<string, double[]> keyValuePairs);  //Added bool isExpiry Flag Parameter by Akshay on 11-12-2020

    delegate void del_PositionsReceived(Dictionary<string, List<ConsolidatedPositionInfo>> dict_ReceivedClientWiseTrades);

    class EngineDataConnector
    {
        NerveLogger _logger;

        public event del_IsStateChanged eve_EngineStatus;
        public event del_SpanData eve_SpanData;

        public event del_PositionsReceived eve_PositionsReceived;

        static Socket soc_Trade = null;
        static Socket soc_Span = null;

        int _SpanConnectionAttempts = 0, _TradeConnectionAttempts = 0;
        int _MaxSpanConnectionAttempts = 3, _MaxTradeConnectionAttempts = 3;

        string TradeUsername, SpanUsername = "";
        IPAddress TradeIP, SpanIP = IPAddress.Parse("127.0.0.1");
        int TradePORT, SpanPORT = 0;

        bool wasTradeConnected, wasSpanConnected = false;

        public EngineDataConnector()
        {
            this._logger = CollectionHelper._logger;
        }

        public void ConnectToEngineSpan(IPAddress _IP, int _PORT, string Username)   // connecting to server engineip, primeport
        {
            SpanUsername = Username;
            SpanIP = _IP;
            SpanPORT = _PORT;

            soc_Span = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while (!soc_Span.Connected && _SpanConnectionAttempts < _MaxSpanConnectionAttempts)
            {
                _logger.Error(null, "Span Connection Attempt: " + _SpanConnectionAttempts);

                try
                {
                    _SpanConnectionAttempts++;
                    soc_Span.Connect(_IP, _PORT);

                    if (soc_Span.Connected)
                    {
                        _logger.Error(null, "Connected to Span Engine....");

                        wasSpanConnected = true;

                        //added on 09APR2021 by Amey
                        eve_EngineStatus(true, "Engine Connected");

                        _SpanConnectionAttempts = 0;

                        //added on 17FEB2021 by Amey
                        string str = "ID^SPAN^" + Username + "^" + "<EOF>";
                        byte[] buffer = Encoding.ASCII.GetBytes(str);
                        soc_Span.Send(buffer, 0, buffer.Length, SocketFlags.None);

                        //ParameterizedThreadStart paraThread = new ParameterizedThreadStart(ReceiveSpanResponse);
                        //var th = new Thread(paraThread);
                        //th.Start(soc_Span);

                        //changed to Task on 08APR2021 by Amey
                        Task.Run(() => ReceiveSpanResponse(soc_Span));
                    }
                    else
                    {
                        _logger.Error(null, "Soc_Span is Not Connected");
                    }
                }
                catch (SocketException ee)
                {
                    if (_SpanConnectionAttempts < _MaxSpanConnectionAttempts)
                    {
                        _logger.Error(ee, "Span Retrying " + _IP + ", " + _PORT);
                        Thread.Sleep(10000);
                    }
                    else
                    {
                        //added on 09APR2021 by Amey
                        eve_EngineStatus(true, "Engine Disconnected");

                        _SpanConnectionAttempts = 0;

                        _logger.Error(ee, "Span Failed to connect " + _IP + ", " + _PORT);

                        break;
                    }
                }
            }
        }

        public void ConnectToEngineData(IPAddress _IP, int _PORT, string Username)   // connecting to server engineip, primeport
        {
            TradeUsername = Username;
            TradeIP = _IP;
            TradePORT = _PORT;

            soc_Trade = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while (!soc_Trade.Connected && _TradeConnectionAttempts < _MaxTradeConnectionAttempts)
            {
                _logger.Error(null, "Trade Connection Attempt: " + _TradeConnectionAttempts);

                try
                {
                    _TradeConnectionAttempts++;
                    soc_Trade.Connect(_IP, _PORT);

                    if (soc_Trade.Connected)
                    {

                        _logger.Error(null, "Connected to Trade Engine....");

                        wasTradeConnected = true;

                        //added on 09APR2021 by Amey
                        eve_EngineStatus(true, "Engine Connected");

                        _TradeConnectionAttempts = 0;

                        //added on 17FEB2021 by Amey
                        string str = "ID^TRADE^" + Username + "^" + "<EOF>";
                        byte[] buffer = Encoding.ASCII.GetBytes(str);
                        soc_Trade.Send(buffer, 0, buffer.Length, SocketFlags.None);

                        //ParameterizedThreadStart paraThread = new ParameterizedThreadStart(ReceiveSpanResponse);
                        //var th = new Thread(paraThread);
                        //th.Start(soc_Span);

                        //changed to Task on 08APR2021 by Amey
                        Task.Run(() => ReceivePrimeResponse(soc_Trade));
                    }
                    else
                    {
                        _logger.Error(null, "Soc_Trade is Not Connected");
                    }
                }
                catch (SocketException ee)
                {
                    if (_TradeConnectionAttempts < _MaxTradeConnectionAttempts)
                    {
                        _logger.Error(ee, "Trade Retrying " + _IP + ", " + _PORT);
                        Thread.Sleep(10000);
                    }
                    else
                    {
                        //added on 09APR2021 by Amey
                        eve_EngineStatus(true, "Engine Disconnected");

                        _TradeConnectionAttempts = 0;

                        _logger.Error(ee, "Trade Failed to connect " + _IP + ", " + _PORT);

                        break;
                    }
                }
            }
        }

        private void ReceivePrimeResponse(Socket soc_Current)
        {
            _logger.Error(null, "Waiting for Position data..");

            string EOF = "<EOF>";
            int EOFIndex = 0;
            int EOFLength = EOF.Length;

            Dictionary<string, List<ConsolidatedPositionInfo>> dict_ReceivedClientWiseTrades = new Dictionary<string, List<ConsolidatedPositionInfo>>();

            var arr_Buffer = new byte[138412032];
            int _ReceivedBytesLength = 0;
            byte[] arr_BytesReceived;

            string PreviousDataTrade = string.Empty;
            string ProperDataTrade = string.Empty;

            var isConnected = true;

            List<byte> list_byte = new List<byte>();

            while (isConnected)
            {
                try
                {

                    _ReceivedBytesLength = soc_Current.Receive(arr_Buffer, SocketFlags.None);

                    if (_ReceivedBytesLength > 0)
                    {
                        arr_BytesReceived = new byte[_ReceivedBytesLength];
                        Array.Copy(arr_Buffer, arr_BytesReceived, _ReceivedBytesLength);

                        list_byte.AddRange(arr_BytesReceived);

                        arr_BytesReceived = new byte[0];

                        int index = GetIndex(list_byte.ToArray());
                        while (index >= 0)
                        {
                            byte[] decomp = new byte[index];
                            Array.Copy(list_byte.ToArray(), decomp, index);

                            list_byte.RemoveRange(0, index + 5);

                            dict_ReceivedClientWiseTrades = DecompressDeserializeTrades(decomp);

                            if (dict_ReceivedClientWiseTrades != null)
                                eve_PositionsReceived(dict_ReceivedClientWiseTrades);

                            index = -1;     // To break the loop once we receive the dictionary
                        }
                    }
                    else
                        throw new SocketException();
                }
                catch (ArgumentException) { list_byte.Clear(); }
                catch (OutOfMemoryException) { list_byte.Clear(); }
                catch (SocketException ee)
                {
                    isConnected = false;

                    PreviousDataTrade = "";

                    _logger.Error(ee, "SocketException ReceivePrimeResponse Trade Received- " + ProperDataTrade.Length);

                    if (wasTradeConnected)
                    {
                        wasTradeConnected = false;

                        Thread.Sleep(1000);

                        _MaxTradeConnectionAttempts = 500;
                        _logger.Error(null, "Connecting to Trade Engine...");
                        ConnectToEngineData(TradeIP, TradePORT, TradeUsername);
                    }


                    if (!wasTradeConnected)
                    {
                        _logger.Error(null, "wasTradeConnected is False");
                    }
                }
                catch (Exception ee)
                {
                    isConnected = false;

                    _logger.Error(ee, "Exception ReceivePrimeResponse Trade Received- " + ProperDataTrade.Length);

                    //added on 12APR2021 by Amey
                    PreviousDataTrade = "";

                    if (wasTradeConnected && !soc_Current.Connected)
                    {
                        wasTradeConnected = false;

                        Thread.Sleep(1000);

                        _MaxTradeConnectionAttempts = 500;
                        _logger.Error(null, "Connecting to Trade Engine...");
                        ConnectToEngineData(TradeIP, TradePORT, TradeUsername);
                    }

                    if (!wasTradeConnected)
                    {
                        _logger.Error(null, "wasTradeConnected is False");
                    }

                    if (soc_Current.Connected)
                    {
                        _logger.Error(null, "Trade soc_Current is still connected");
                    }
                }
            }
        }

        private void ReceiveSpanResponse(Socket soc_Current)
        {
            _logger.Error(null, "Waiting for Span data..");

            string EOF = "<EOF>";
            int EOFIndex = 0;
            int EOFLength = EOF.Length;

            Dictionary<string, double[]> dict_ReceivedData = new Dictionary<string, double[]>();

            var arr_Buffer = new byte[67108864];
            int _ReceivedBytesLength = 0;
            byte[] arr_BytesReceived;
            string[] arr_Fields;

            string PreviousDataSpan = "";
            string ProperDataSpan = "";

            var arr_MappedClients = CollectionHelper.dict_ClientInfo.Keys.ToArray();
            List<byte> list_byte = new List<byte>();
            var isConnected = true;

            while (isConnected)
            {
                try
                {
                    //_logger.Debug("Waiting for Span");
                    _ReceivedBytesLength = soc_Current.Receive(arr_Buffer, SocketFlags.None);

                    if (_ReceivedBytesLength > 0)
                    {
                        arr_BytesReceived = new byte[_ReceivedBytesLength];
                        Array.Copy(arr_Buffer, arr_BytesReceived, _ReceivedBytesLength);

                        list_byte.AddRange(arr_BytesReceived);

                        arr_BytesReceived = new byte[0];

                        int index = GetIndex(list_byte.ToArray());

                        while (index >= 0)
                        {
                            byte[] decomp = new byte[index];
                            Array.Copy(list_byte.ToArray(), decomp, index);

                            list_byte.RemoveRange(0, index + 5);

                            var dict_Received = DecompressDeserializeSpan(decomp);

                            if (dict_Received != null)
                            {
                                eve_SpanData(dict_Received);
                            }
                                

                            index = -1;     // To break the loop once we receive the dictionary
                        }
                    }
                    else
                        throw new SocketException();
                }
                catch (ArgumentException) { list_byte.Clear(); }
                catch (OutOfMemoryException) { list_byte.Clear(); }
                catch (SocketException ee)
                {
                    isConnected = false;

                    PreviousDataSpan = "";

                    _logger.Error(ee, "SocketException ReceivePrimeResponse Span Received- " + ProperDataSpan.Length);

                    if (wasSpanConnected)
                    {
                        wasSpanConnected = false;

                        Thread.Sleep(1000);

                        _MaxSpanConnectionAttempts = 500;
                        _logger.Error(null, "Connecting to Span Engine...");
                        ConnectToEngineSpan(SpanIP, SpanPORT, SpanUsername);
                    }

                    if (!wasSpanConnected)
                    {
                        _logger.Error(null, "wasTradeConnected is False");
                    }
                }
                catch (Exception ee)
                {
                    isConnected = false;

                    _logger.Error(ee, "Exception ReceivePrimeResponse Span Received- " + ProperDataSpan.Length);

                    //added on 12APR2021 by Amey
                    PreviousDataSpan = "";

                    if (wasSpanConnected && !soc_Current.Connected)
                    {
                        wasSpanConnected = false;

                        Thread.Sleep(1000);

                        _MaxSpanConnectionAttempts = 500;
                        _logger.Error(null, "Connecting to Span Engine...");
                        ConnectToEngineSpan(SpanIP, SpanPORT, SpanUsername);
                    }

                    if (!wasSpanConnected)
                    {
                        _logger.Error(null, "wasTradeConnected is False");
                    }

                    if (soc_Current.Connected)
                    {
                        _logger.Error(null, "Span soc_Current is still connected");
                    }
                }
            }// end while socket connected
        }

        string DecompressData(byte[] inputBytes)
        {
            try
            {
                using (var inputStream = new MemoryStream(inputBytes))
                using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                using (var streamReader = new StreamReader(gZipStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (InvalidDataException) { return string.Empty; }
            catch (Exception ee)
            {
                _logger.Error(ee);
                return string.Empty;
            }
        }

        private Dictionary<string, List<ConsolidatedPositionInfo>> DecompressDeserializeTrades(byte[] buffer)
        {
            try
            {
                
                byte[] decompressed = QuickLZ.decompress(buffer);
                Dictionary<string, List<ConsolidatedPositionInfo>> dict = new Dictionary<string, List<ConsolidatedPositionInfo>>();
                using (MemoryStream ms = new MemoryStream(decompressed, true))
                {
                    ms.Position = 0;
                    dict = Serializer.Deserialize<Dictionary<string, List<ConsolidatedPositionInfo>>>(ms);
                }

                return dict;
            }
            catch (Exception ee) { return null;  }
        }

        private Dictionary<string, double[]> DecompressDeserializeSpan(byte[] buffer)
        {
            try
            {
                
                List<byte> decompressed = QuickLZ.decompress(buffer).ToList();
                Dictionary<string, double[]> dict = new Dictionary<string, double[]>();
                using (MemoryStream ms = new MemoryStream(decompressed.ToArray(), true))
                {
                    ms.Position = 0;
                    dict = Serializer.Deserialize<Dictionary<string, double[]>>(ms);
                }

                return dict;
            }
            catch (Exception ee) { return null; }
        }

        private int GetIndex(byte[] bytearray)
        {
            try
            {
                byte[] b_EOF = Encoding.UTF8.GetBytes("<EOF>");

                int maxFirstCharSlot = bytearray.Length - b_EOF.Length + 1;
                int index = 0;
                bool found = false;

                for (int i = 0; i < maxFirstCharSlot; i++)
                {
                    if (bytearray[i] != b_EOF[0])
                        continue;

                    for (int j = b_EOF.Length - 1; j >= 1; j--)
                    {
                        if (bytearray[i + j] != b_EOF[j]) break;
                        if (j == 1)
                        {
                            index += i;
                            found = true;
                        }

                        if (found)
                            break;
                    }

                    if (found)
                        break;
                }

                if (!found)
                    index = -1;

                bytearray = new byte[0];

                return index;
            }
            catch (Exception ee)
            {
                bytearray = new byte[0]; return -1;
            }
        }
        private byte[] Decrypt(byte[] input)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes("hjiweykaksd", new byte[] { 0x43, 0x87, 0x23, 0x72 });

            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms,
              aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }
    }
}
