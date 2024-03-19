using n.Structs;
using NerveLog;
using Newtonsoft.Json;
using Prime.Helper;
using Prime.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prime
{
    public class EngineHBConnector
    {
        Socket soc_EngineHB = null;

        public event del_HeartBeatTickReceived eve_HeartBeatTickReceived;
        public event del_IsStateChanged eve_LoginResponse;
        public event del_IsStateChanged eve_ClientInfoReceived;
        public event del_ClientUpdateReceived eve_ClientUpateRecevied;

        NerveLogger _logger;

        //IPAddress _IpAddress = IPAddress.Parse("127.0.0.1");
        //int port;

        int _HBConnectionAttempts = 0;

        int _MaxConnectionAttempts = 3;

        internal string Username = "";

        bool wasHBConnected = false;

        IPAddress HBIP = IPAddress.Parse("127.0.0.1");
        int HBPORT = 0;

        public EngineHBConnector()
        {
            _logger = CollectionHelper._logger;
        }

        private void ReceiveResponse(Socket soc_Current)
        {
            string EOF = "<EOF>";
            int EOFIndex = 0;
            int EOFLength = EOF.Length;

            int _ReceivedBytesLength = 0;
            byte[] arr_BytesReceived;
            string[] arr_Fields;
            var buffer = new byte[2048];

            string PreviousDataHB = string.Empty;
            string ProperData = string.Empty;

            var isConnected = true;

            while (isConnected)
            {
                try
                {
                    _ReceivedBytesLength = soc_Current.Receive(buffer, SocketFlags.None);

                    if (_ReceivedBytesLength > 0)
                    {
                        arr_BytesReceived = new byte[_ReceivedBytesLength];
                        Array.Copy(buffer, arr_BytesReceived, _ReceivedBytesLength);

                        PreviousDataHB += Encoding.ASCII.GetString(arr_BytesReceived);

                        while ((EOFIndex = PreviousDataHB.IndexOf(EOF)) >= 0)
                        {
                            //added on 03MAY2021 by Amey
                            //To avoid "System.ArgumentOutOfRangeException: Length cannot be less than zero." exception.
                            //EOFIndex = PreviousDataSpan.IndexOf(EOF);
                            while (EOFIndex == 0)
                            {
                                PreviousDataHB = PreviousDataHB.Substring(EOFIndex + EOFLength);
                                EOFIndex = PreviousDataHB.IndexOf(EOF);
                            }

                            if (EOFIndex < 0)
                                continue;

                            ProperData = PreviousDataHB.Substring(0, EOFIndex);
                            PreviousDataHB = PreviousDataHB.Substring(EOFIndex + EOF.Length);

                            arr_Fields = ProperData.Split('|');

                            //changed on 17FEB2021 by Amey
                            if (arr_Fields[0] == "HB")
                                eve_HeartBeatTickReceived(arr_Fields[1]);
                            else if (arr_Fields[0] == "LOGIN")
                                eve_LoginResponse(Convert.ToBoolean(arr_Fields[2]), arr_Fields[1]);
                            //changed on 07APR2021 by Amey
                            else if (arr_Fields[0] == "CLIENT")
                            {
                                CollectionHelper.dict_ClientInfo = new ConcurrentDictionary<string, ClientInfo>(JsonConvert.DeserializeObject<Dictionary<string, ClientInfo>>(arr_Fields[1]));

                                try
                                {
                                    foreach (var Key in CollectionHelper.dict_ClientInfo.Keys)
                                    {
                                        var arr_NPL = CollectionHelper.dict_ClientInfo[Key].NPLValues.Split(',');
                                        foreach (var NPLKey in arr_NPL)
                                        {
                                            try
                                            {
                                                var arr_NPLVal = NPLKey.Split('_');
                                                if (arr_NPLVal.Length > 1 && !CollectionHelper.dict_NPLValues.ContainsKey(arr_NPLVal[0]))
                                                    CollectionHelper.dict_NPLValues.Add(arr_NPLVal[0], Convert.ToDouble(arr_NPLVal[1]));
                                            }
                                            catch (Exception) { }
                                        }
                                    }
                                }
                                catch (Exception ee) { _logger.Error(ee, "arr_NPLVal"); }

                                //added on 09APR2021 by Amey
                                _logger.Error(null, "ClientInfo Received for : " + CollectionHelper.dict_ClientInfo.Count);

                                eve_ClientInfoReceived(true);
                            }
                            else if (arr_Fields[0] == "CLIENT_UPDATE")
                            {
                                var dict_TempClientInfo = new ConcurrentDictionary<string, ClientInfo>(JsonConvert.DeserializeObject<Dictionary<string, ClientInfo>>(arr_Fields[1]));
                                CollectionHelper.dict_ClientInfo = dict_TempClientInfo;
                                try
                                {
                                    foreach (var Key in CollectionHelper.dict_ClientInfo.Keys)
                                    {
                                        var arr_NPL = CollectionHelper.dict_ClientInfo[Key].NPLValues.Split(',');
                                        foreach (var NPLKey in arr_NPL)
                                        {
                                            try
                                            {
                                                var arr_NPLVal = NPLKey.Split('_');
                                                if (arr_NPLVal.Length > 1 && !CollectionHelper.dict_NPLValues.ContainsKey(arr_NPLVal[0]))
                                                    CollectionHelper.dict_NPLValues.Add(arr_NPLVal[0], Convert.ToDouble(arr_NPLVal[1]));
                                            }
                                            catch (Exception) { }
                                        }
                                    }
                                    _logger.Error(null, "ClientInfo Update Received for : " + CollectionHelper.dict_ClientInfo.Count);
                                    eve_ClientUpateRecevied();
                                }
                                catch (Exception ee) { _logger.Error(ee, "arr_NPLVal"); }
                            }
                            //added on 07APR2021 by Amey
                            else if (arr_Fields[0] == "BAN")
                                CollectionHelper.hs_BannedUnderlyings = JsonConvert.DeserializeObject<HashSet<string>>(arr_Fields[1]);
                            //added on 07APR2021 by Amey
                            else if (arr_Fields[0] == "BANINFO")
                            {
                                CollectionHelper.dict_BanInfo = new ConcurrentDictionary<string, BanInfo>(JsonConvert.DeserializeObject<Dictionary<string, BanInfo>>(arr_Fields[1]));
                                uc_Violations.Instance?.RefreshGrid();
                            }
                            
                        }
                    }
                    else
                        throw new SocketException();
                }
                catch (SocketException ee)
                {
                    isConnected = false;

                    PreviousDataHB = "";

                    _logger.Error(ee, "ReceiveResponseHB Received- " + ProperData.Length);

                    if (wasHBConnected)
                    {
                        wasHBConnected = false;

                        Thread.Sleep(1000);

                        _MaxConnectionAttempts = 500;
                        ConnectToEngine(HBIP, HBPORT, Username);
                    }
                }
                catch (Exception ee)
                {
                    isConnected = false;

                    _logger.Error(ee, "ReceiveResponseHB Received- " + ProperData.Length);

                    //added on 12APR2021 by Amey
                    PreviousDataHB = "";

                    if (wasHBConnected && !soc_Current.Connected)
                    {
                        wasHBConnected = false;

                        Thread.Sleep(1000);

                        _MaxConnectionAttempts = 500;
                        ConnectToEngine(HBIP, HBPORT, Username);
                    }
                }
            }// end while socket connected
        }

        public void ConnectToEngine(IPAddress _IP, int _PORT, string Username = "")   // connecting to server
        {
            HBIP = _IP;
            HBPORT = _PORT;

            string _MACAddress = GetMACAddress();
            if (_MACAddress != "")
            {
                soc_EngineHB = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                while (!soc_EngineHB.Connected && (_HBConnectionAttempts < _MaxConnectionAttempts))
                {
                    try
                    {
                        _HBConnectionAttempts++;
                        soc_EngineHB.Connect(_IP, _PORT);

                        if (soc_EngineHB.Connected)
                        {
                            wasHBConnected = true;

                            _HBConnectionAttempts = 0;

                            //changed message on 17FEB2021 by Amey
                            string str = "ID^" + _MACAddress + "^" + "<EOF>";
                            byte[] buffer = Encoding.ASCII.GetBytes(str);
                            soc_EngineHB.Send(buffer, 0, buffer.Length, SocketFlags.None);

                            //ParameterizedThreadStart paraThread = new ParameterizedThreadStart(ReceiveResponse);
                            //var thReceive = new Thread(paraThread);
                            //thReceive.Start(soc_EngineHB);

                            //changed to Task on 08APR2021 by Amey
                            Task.Run(() => ReceiveResponse(soc_EngineHB));

                            if (Username != "")
                                SendToEngine("RELOGIN^" + Username);
                        }
                    }
                    catch (SocketException ee)
                    {
                        if (_HBConnectionAttempts < _MaxConnectionAttempts)
                        {
                            _logger.Error(ee, "Retrying " + _IP + ", " + _PORT);
                            Thread.Sleep(10000);
                        }
                        else
                        {
                            //added on 08APR2021 by Amey
                            eve_LoginResponse(false, "Unable to connect to the Server. Please contact System Administrator.");

                            _HBConnectionAttempts = 0;

                            _logger.Error(ee, "Failed to connect " + _IP + ", " + _PORT);

                            break;
                        }
                    }
                }
            }
            //added on 08APR2021 by Amey
            else
                eve_LoginResponse(false, "Unable to fetch required info from the Server. Please contact System Administrator.");
        }

        public bool SendToEngine(string _Message)
        {
            if (soc_EngineHB.Connected)
            {
                string str = _Message + "^" + "<EOF>";
                byte[] buffer = Encoding.ASCII.GetBytes(str);
                soc_EngineHB.Send(buffer);

                return true;
            }
            else
                return false;
        }

        private string GetMACAddress()
        {
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                String sMacAddress = string.Empty;
                foreach (NetworkInterface adapter in nics)
                {
                    if (sMacAddress == String.Empty)// only return MAC Address from first card  
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        sMacAddress = adapter.GetPhysicalAddress().ToString();
                    }
                }
                return sMacAddress;
            }
            catch (Exception macEx)
            {
                _logger.Error(macEx);
                return string.Empty;
            }
        }

        private string DecompressData(string inputString)
        {
            try
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);

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
    }
}
