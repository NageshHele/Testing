using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using n.Structs;
using System.Threading;

namespace Engine
{
    class PrimeHBConnector
    {
        private readonly Socket socket_PrimeHeartBeat = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private const int BUFFER_SIZE = 2048;

        internal ConcurrentDictionary<string, Socket> dict_ClientSockets = new ConcurrentDictionary<string, Socket>();

        /// <summary>
        /// Key : ClientID | Value : ClientInfo
        /// </summary>
        ConcurrentDictionary<string, ClientInfo> dict_ClientInfo = new ConcurrentDictionary<string, ClientInfo>();

        /// <summary>
        /// Key : Prime-Username | Value : Mapped Clients
        /// </summary>
        ConcurrentDictionary<string, string[]> dict_UserMappedInfo = new ConcurrentDictionary<string, string[]>();

        /// <summary>
        /// Key : Prime-Username | Value : Password
        /// </summary>
        ConcurrentDictionary<string, string> dict_UserInfo = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Key : Username | Value : true if sending is complete, false if still compressing and sending.
        /// </summary>
        ConcurrentDictionary<string, bool> dict_IsHBSent = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Key : Username | Value : true if sending is complete, false if still compressing and sending.
        /// </summary>
        ConcurrentDictionary<string, bool> dict_IsBanInfoSent = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Key : Code | Value : List of Limits
        /// </summary>
        Dictionary<string, LimitInfo> dict_LimitInfo = new Dictionary<string, LimitInfo>();

        /// <summary>
        /// Contains all banned Underlyings presenf in fo_secban.csv.
        /// </summary>
        HashSet<string> hs_BannedUnderlyings = new HashSet<string>();

        clsWriteLog _logger;

        PrimeDataConnector _DataServer;

        int _TimeoutMilliseconds = 120000;

        //added on 29DEC2021 by Nikhil
        bool IsClientInfoUpdated = false;

        string _Version = "";

        public void SetupServer(IPAddress ip_EngineServer, int PrimeHeartBeatPort, clsWriteLog _logger, PrimeDataConnector _DataServer,
            int _TimeoutMilliseconds, string _Version)
        {
            try
            {
                this._logger = _logger;
                this._DataServer = _DataServer;
                this._TimeoutMilliseconds = _TimeoutMilliseconds;
                this._Version = _Version;

                //changed on 21DEC2020 by Amey
                socket_PrimeHeartBeat.Bind(new IPEndPoint(ip_EngineServer, PrimeHeartBeatPort));

                //socket_PrimeHeartBeat.Bind(new IPEndPoint(IPAddress.Any, PrimeHeartBeatPort));
                socket_PrimeHeartBeat.Listen(0);
                socket_PrimeHeartBeat.BeginAccept(AcceptCallback, null);
            }
            catch (Exception ee)
            {
                _logger.WriteLog("SetupServer : " + ee);
            }
        }

        //To update Dict after changing User/Client info in DB. 09MAR2021-Amey
        public void UpdateCollections(ConcurrentDictionary<string, string> dict_UserInfo, ConcurrentDictionary<string, string[]> dict_UserMappedInfo,
            ConcurrentDictionary<string, ClientInfo> dict_ClientInfo,
            HashSet<string> hs_BannedUnderlyings, Dictionary<string, LimitInfo> dict_LimitInfo)
        {
            //added on 17FEB2021 by Amey
            this.dict_UserInfo = dict_UserInfo;
            this.dict_UserMappedInfo = dict_UserMappedInfo;
            this.dict_ClientInfo = dict_ClientInfo;

            //added on 29DEC2021 by Nikhil  | runtime client update
            IsClientInfoUpdated = true;

            //added on 07APR2021 by Amey
            this.hs_BannedUnderlyings = hs_BannedUnderlyings;
            this.dict_LimitInfo = dict_LimitInfo;
        }

        //added by nikhil | runtime client update
        public void SendClientData()
        {
            try
            {
                if (IsClientInfoUpdated)
                {
                    var list_ConnectedTradeUsers = dict_ClientSockets.Keys.ToList();
                    foreach (var Username in list_ConnectedTradeUsers)
                    {
                        Task.Run(() => SendClientInfoToPrime(Username));
                    }
                    IsClientInfoUpdated = false;
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("SendClientData : " + ee);
            }
        }

        public void ClientLedgerUpdated(ConcurrentDictionary<string, ClientInfo> dict_ClientInfo)
        {
            try
            {
                this.dict_ClientInfo = dict_ClientInfo;
                SendLedgerValues();//Task.Run(() => SendLedgerValues());
            }
            catch (Exception ee)
            {
                _logger.WriteLog(ee.StackTrace);
            }
        }

        public void SendLedgerValues()
        {
            try
            {
                var list_ConnectedTradeUsers = dict_ClientSockets.Keys.ToList();
                foreach (var Username in list_ConnectedTradeUsers)
                {
                    Task.Run(() => SendLedgerToPrime(Username));
                }

            }
            catch (Exception ee)
            {
                _logger.WriteLog("SendClientData : " + ee);
            }

        }

        private void SendLedgerToPrime(string UserName)
        {
            try
            {
                if (dict_UserMappedInfo.TryGetValue(UserName, out string[] arr_MappedClients))
                {
                    var dict_Temp = dict_ClientInfo.Where(x => arr_MappedClients.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
                    try
                    {
                        byte[] buffer = Encoding.ASCII.GetBytes("LEDGER_UPDATE|" + JsonConvert.SerializeObject(dict_Temp) + "<EOF>");

                        if (dict_ClientSockets.TryGetValue(UserName, out Socket _Socket))
                            _Socket.Send(buffer, buffer.Length, SocketFlags.None);
                    }
                    catch (Exception ee)
                    {
                        _logger.WriteLog("SendCLientInfoToPrime : " + ee);
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("SendCLientInfoToPrime : " + ee);
            }
        }



        private void SendClientInfoToPrime(string UserName)
        {
            try
            {
                if (dict_UserMappedInfo.TryGetValue(UserName, out string[] arr_MappedClients))
                {
                    var dict_Temp = dict_ClientInfo.Where(x => arr_MappedClients.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
                    try
                    {
                        byte[] buffer = Encoding.ASCII.GetBytes("CLIENT_UPDATE|" + JsonConvert.SerializeObject(dict_Temp) + "<EOF>");

                        if (dict_ClientSockets.TryGetValue(UserName, out Socket _Socket))
                            _Socket.Send(buffer, buffer.Length, SocketFlags.None);
                    }
                    catch (Exception ee)
                    {
                        _logger.WriteLog("SendCLientInfoToPrime : " + ee);
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("SendCLientInfoToPrime : " + ee);
            }
        }

        private void AcceptCallback(IAsyncResult AR)
        {
            try
            {
                Socket soc_Current = socket_PrimeHeartBeat.EndAccept(AR);

                //added on 10MAY2021 by Amey
                Task.Run(() => ReceiveCallback(soc_Current));

                //soc_Current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, soc_Current);
            }
            catch (ObjectDisposedException ee) { _logger.WriteLog("AcceptCallBack Disposed : " + ee); return; }
            catch (Exception ee)
            {
                _logger.WriteLog("AcceptCallBack : " + ee);
            }

            socket_PrimeHeartBeat.BeginAccept(AcceptCallback, null);
        }

        public void SendHeartBeat(string prmValue)
        {
            try
            {
                var list_ConnectedHBUsers = dict_ClientSockets.Keys.ToList();

                foreach (var Username in list_ConnectedHBUsers)
                {
                    if (dict_IsHBSent[Username])
                    {
                        dict_IsHBSent[Username] = false;

                        Task.Run(() => SendHBToPrime(Username.ToString(), prmValue.ToString()));
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog("SendHeartBeat : " + ee); }
        }

        //added on 07APR2021 by Amey
        public void SendBanInfo(Dictionary<string, BanInfo> dict_BanInfo)
        {
            try
            {
                var list_ConnectedHBUsers = dict_ClientSockets.Keys.ToList();

                foreach (var Username in list_ConnectedHBUsers)
                {
                    if (dict_IsBanInfoSent[Username])
                    {
                        dict_IsBanInfoSent[Username] = false;

                        Task.Run(() => SendBanInfoToPrime(Username.ToString(), new Dictionary<string, BanInfo>(dict_BanInfo)));
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog("SendBanInfo : " + ee); }
        }

        private void SendHBToPrime(string Username, string _HB)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes("HB|" + _HB + "<EOF>");
                if (dict_ClientSockets.TryGetValue(Username, out Socket _Socket))
                    _Socket.Send(buffer, buffer.Length, SocketFlags.None);
            }
            catch (Exception ee) { _logger.WriteLog("SendHBToPrime : " + Username + Environment.NewLine + ee); CloseAndRemoveConnections(Username); }

            Thread.Sleep(200);

            dict_IsHBSent[Username] = true;
        }
        
        private void SendBanInfoToPrime(string Username, Dictionary<string, BanInfo> dict_BanInfo)
        {
            try
            {
                if (dict_UserMappedInfo.TryGetValue(Username, out string[] arr_MappedClients) && dict_ClientSockets.TryGetValue(Username, out Socket _Socket))
                {
                    var dict_Filtered = dict_BanInfo.Where(x => arr_MappedClients.Contains(x.Key.Split('^')[0])).ToDictionary(x => x.Key, x => x.Value);

                    //changed on 17FEB2021 by Amey
                    byte[] buffer = Encoding.ASCII.GetBytes("BANINFO|" + JsonConvert.SerializeObject(dict_Filtered) + "<EOF>");

                    _Socket.Send(buffer, buffer.Length, SocketFlags.None);
                }
            }
            catch (Exception ee) { _logger.WriteLog("SendBanInfoToPrime : " + Username + Environment.NewLine + ee); CloseAndRemoveConnections(Username); }

            dict_IsBanInfoSent[Username] = true;
        }

        internal void CloseAndRemoveConnections(string Username)
        {
            try
            {
                if (dict_ClientSockets.TryGetValue(Username, out Socket _Socket))
                {
                    dict_ClientSockets.TryRemove(Username, out _);
                    _Socket.Close();
                }
            }
            catch (Exception ee) { _logger.WriteLog("CloseAndRemoveConnections HB : " + Username + Environment.NewLine + ee); }

            //added on 07MAY2021 by Amey. To close every connection for this Username.
            _DataServer.CloseAndRemoveConnections(Username);
        }

        private void ReceiveCallback(Socket soc_Current)
        {
            int _ReceivedBytesLength;

            byte[] arr_Buffer = new byte[2048];
            byte[] arr_BytesReceived;
            string[] arr_Fields;
            string EOF = "<EOF>";
            int EOFIndex = 0;
            int EOFLength = EOF.Length;
            string ProperData = string.Empty;
            string PreviousDataHB = string.Empty;

            var isConnected = true;

            while (isConnected)
            {
                try
                {
                    _ReceivedBytesLength = soc_Current.Receive(arr_Buffer, SocketFlags.None);

                    if (_ReceivedBytesLength > 0)
                    {
                        arr_BytesReceived = new byte[_ReceivedBytesLength];
                        Array.Copy(arr_Buffer, arr_BytesReceived, _ReceivedBytesLength);

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

                            ProperData = PreviousDataHB.Substring(0, EOFIndex - 1);
                            PreviousDataHB = PreviousDataHB.Substring(EOFIndex + EOF.Length);

                            _logger.WriteLog("Received HB Server : " + ProperData);

                            //changed on 17FEB2021 by Amey
                            arr_Fields = ProperData.Split('^');
                            if (arr_Fields[0] == "ID")
                            {
                                //dict_ClientSockets.TryAdd(text, current);
                            }
                            else if (arr_Fields[0] == "USER")
                            {
                                //Format => USER^Username|Password^<EOF>
                                var arr_Info = arr_Fields[1].Split('|');
                                var Username = arr_Info[0].ToLower();

                                string res_Message = "LOGIN|";

                                string Version = "";

                                try
                                {
                                    Version = arr_Info[2].ToLower();
                                }
                                catch (Exception ee)
                                {

                                }

                                if (dict_ClientSockets.TryGetValue(Username, out Socket _Socket))
                                {
                                    try
                                    {
                                        //added on 12MAY2021 by Amey
                                        if (_Socket.Connected)
                                            res_Message += "User already logged in. Please check and relogin.|false";
                                        else
                                            CloseAndRemoveConnections(Username);
                                    }
                                    catch (Exception ee) { _logger.WriteLog("HB Socket Available  : " + ee); CloseAndRemoveConnections(Username); }
                                }

                                if (Version == _Version)
                                {
                                    if (!dict_ClientSockets.ContainsKey(Username))
                                    {
                                        if (dict_UserInfo.TryGetValue(Username, out string _Password))
                                        {
                                            if (_Password == arr_Info[1])
                                            {
                                                dict_ClientSockets.TryAdd(Username, soc_Current);

                                                //added on 10JUN2021 by Amey
                                                if (dict_IsHBSent.ContainsKey(Username))
                                                    dict_IsHBSent[Username] = true;
                                                else
                                                    dict_IsHBSent.TryAdd(Username, true);

                                                if (dict_IsBanInfoSent.ContainsKey(Username))
                                                    dict_IsBanInfoSent[Username] = true;
                                                else
                                                    dict_IsBanInfoSent.TryAdd(Username, true);

                                                res_Message += "Login Successful.|true";
                                            }
                                            else
                                                res_Message += "Incorrect login detail. Please check and relogin.|false";
                                        }
                                        else
                                            res_Message += "Username not found. Please check and relogin.|false";
                                    }
                                }
                                else
                                    res_Message += "You are running an old version of n.Prime. \n\n\t\t\t\t\tPlease Update.|false";

                                byte[] buffer = Encoding.ASCII.GetBytes(res_Message + "<EOF>");
                                soc_Current.Send(buffer, buffer.Length, SocketFlags.None);
                            }
                            else if (arr_Fields[0] == "CLIENT")
                            {
                                var arr_Info = arr_Fields[1].Split('|');
                                var Username = arr_Info[0].ToLower();
                                string res_Message = $"CLIENT|{JsonConvert.SerializeObject(new Dictionary<string, ClientInfo>())}";

                                if (dict_UserMappedInfo.TryGetValue(Username, out string[] arr_MappedClients))
                                {
                                    //var MappedClients = dict_UserInfo[Username][1].Split(',');
                                    var dict_Temp = dict_ClientInfo.Where(x => arr_MappedClients.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);

                                    //TODO: Fix naming convention.
                                    byte[] buffer2 = Encoding.ASCII.GetBytes("CLIENT|" + JsonConvert.SerializeObject(dict_Temp) + "<EOF>");
                                    soc_Current.Send(buffer2, buffer2.Length, SocketFlags.None);
                                }
                                else
                                {
                                    byte[] buffer = Encoding.ASCII.GetBytes(res_Message + "<EOF>");
                                    soc_Current.Send(buffer, buffer.Length, SocketFlags.None);
                                }
                            }
                            //added on 07APR2021 by Amey
                            else if (arr_Fields[0] == "BAN")
                            {
                                //BAN^USERNAME

                                //TODO: Fix naming convention.
                                byte[] buffer2 = Encoding.ASCII.GetBytes("BAN|" + JsonConvert.SerializeObject(hs_BannedUnderlyings) + "<EOF>");
                                soc_Current.Send(buffer2, buffer2.Length, SocketFlags.None);

                                //changed location on 10MAY2021 by Amey
                                //added on 07MAY2021 by Amey
                                soc_Current.SendTimeout = _TimeoutMilliseconds;

                                isConnected = false;
                            }
                            //Added by Akshay on 18-01-2022 for Limit File
                            else if (arr_Fields[0] == "LIMIT")
                            {
                                //BAN^USERNAME

                                //TODO: Fix naming convention.
                                byte[] buffer3 = Encoding.ASCII.GetBytes("LIMIT|" + JsonConvert.SerializeObject(dict_LimitInfo) + "<EOF>");
                                soc_Current.Send(buffer3, buffer3.Length, SocketFlags.None);

                                //changed location on 10MAY2021 by Amey
                                //added on 07MAY2021 by Amey
                                soc_Current.SendTimeout = _TimeoutMilliseconds;

                                isConnected = false;
                            }
                            //added on 07MAY2021 by Amey
                            else if (arr_Fields[0] == "RELOGIN")
                            {
                                //Format => RELOGIN^Username^<EOF>
                                var Username = arr_Fields[1].ToLower();

                                if (!dict_ClientSockets.ContainsKey(Username))
                                {
                                    if (dict_UserMappedInfo.ContainsKey(Username))
                                    {
                                        dict_ClientSockets.TryAdd(Username, soc_Current);

                                        //added on 10JUN2021 by Amey
                                        if (dict_IsHBSent.ContainsKey(Username))
                                            dict_IsHBSent[Username] = true;
                                        else
                                            dict_IsHBSent.TryAdd(Username, true);

                                        if (dict_IsBanInfoSent.ContainsKey(Username))
                                            dict_IsBanInfoSent[Username] = true;
                                        else
                                            dict_IsBanInfoSent.TryAdd(Username, true);

                                        //changed location on 10MAY2021 by Amey
                                        //added on 07MAY2021 by Amey
                                        soc_Current.SendTimeout = _TimeoutMilliseconds;

                                        isConnected = false;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (SocketException ee)
                {
                    isConnected = false;

                    _logger.WriteLog("ReceiveCallBack HB Socket : " + ee);

                    try
                    {
                        //added on 28JAN2021 by Amey
                        var _Username = dict_ClientSockets.Where(v => v.Value == soc_Current).First().Key;
                        CloseAndRemoveConnections(_Username);
                    }
                    catch (Exception) { }

                    soc_Current.Close();
                }
                catch (Exception ee) {
                    _logger.WriteLog("ReceiveCallBack HB : " + ee); isConnected = false; }
            }
        }
    }
}
