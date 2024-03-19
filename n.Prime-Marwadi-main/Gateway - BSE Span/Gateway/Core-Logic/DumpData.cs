using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Windows.Forms;
using XDMessaging;
using SpanCalculate;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Net;
using n.Structs;
using Gateway.Data_Structures;

namespace Gateway
{
    public delegate void del_AddToList(string Message);

    public delegate void del_TradeFileStatus(bool isFOAvailable, bool isCMAvailable, bool isBSECMAvailable);

    public class DumpData
    {
        public event del_AddToList eve_AddToList;
        public event del_TradeFileStatus eve_TradeFileStatus;

        /// <summary>
        /// [0] ClientID|CTCLID|UserID, [1] ClientID, [2] CTCLID, [3] UserID
        /// </summary>
        private readonly string[] arr_IDCombinations = new string[TotalCombinations] { "ClientID|CTCLID|UserID", "ClientID", "CTCLID", "UserID" };
        string _nImageExeName = string.Empty;
        string _MySQLCon = string.Empty;

        //added on 25JAN2021 by Amey
        public string _TradeFileFO = "TradeFO_" + DateTime.Now.ToString("ddMMyyyy");
        public string _TradeFileCM = "TradeCM_" + DateTime.Now.ToString("ddMMyyyy");

        //added on 20APR2021 by Amey
        public string _TradeFileBSECM = "EQ_ITR_*_" + DateTime.Now.ToString("yyyyMMdd");

        string _SpanFolderPath = string.Empty;
        string LatestSpanFileName = string.Empty;

        private const int TotalCombinations = 4;

        double _FOLastTradeTime = 0;
        double _CMLastTradeTime = 0;
        double _SpanComputeTime = 0;

        bool isDebug = false;
        bool isAdmin = false;
        bool isTradeFileFOAvailable = false;
        bool isTradeFileCMAvailable = false;
        bool isTradeFileBSECMAvailable = false;

        /// <summary>
        /// Set true when connected to Span. False when disconnedted or crashed.
        /// </summary>
        bool isSpanConnected = false;

        /// <summary>
        /// Set true when Engine sends START. False when Engine sends STOP
        /// </summary>
        bool IsEngineStarted = false;

        /// <summary>
        /// Set true when Engine sends START. False when Engine sends STOP
        /// </summary>
        bool SpanComputeStart = false;

        clsWriteLog _logger;

        SpanConnectorFO _SpanConnector;

        EngineHBConnector _HeartBeatServer = new EngineHBConnector();

        /// <summary>
        /// Will send data on "n.TRADE" channel.
        /// </summary>
        IXDBroadcaster bCast_Trades;
        XDMessagingClient xd_TradeClient = new XDMessagingClient();

        /// <summary>
        /// Listining on "n.ENGINE" channel.
        /// </summary>
        IXDListener xd_EngineListner;
        XDMessagingClient xd_EngineClient = new XDMessagingClient();

        /// <summary>
        /// Will send data on "n.SPAN" channel.
        /// </summary>
        IXDBroadcaster bCast_Span;//04-12-2019
        XDMessagingClient xd_SpanClient = new XDMessagingClient();

        /// <summary>
        /// Contents of Config.xml
        /// </summary>
        DataSet ds_Config;

        //added on 11JAN2021 by Amey
        //ds_Gateway.dt_AllTradesDataTable dt_AllPositions = new ds_Gateway.dt_AllTradesDataTable();

        //added on 13JAN2021 by Amey
        //ds_Gateway.dt_AllTradesDataTable dt_EODPositions = new ds_Gateway.dt_AllTradesDataTable();

        //added on 10FEB2021 by Amey
        List<PositionInfo> list_AllTrades = new List<PositionInfo>();
        List<PositionInfo> list_EODPositions = new List<PositionInfo>();

        //Changed to HashSet on 22AUG2020 by Amey
        //changed to ConcurrentDictionary on 22OCT2020 by Amey.
        ConcurrentDictionary<string, HashSet<string>> dict_RandomEntriesCM = new ConcurrentDictionary<string, HashSet<string>>();
        ConcurrentDictionary<string, HashSet<string>> dict_RandomEntriesFO = new ConcurrentDictionary<string, HashSet<string>>();
        ConcurrentDictionary<string, HashSet<string>> dict_RandomEntriesBSECM = new ConcurrentDictionary<string, HashSet<string>>();

        /// <summary>
        /// Key => CTCL_ID,ClientCode,UserID,[ClientCode|CTCL_ID|UserID] | Value => Username
        /// </summary>
        ConcurrentDictionary<string, string> dict_ClientInfo = new ConcurrentDictionary<string, string>();

        ConcurrentDictionary<string, double[]> dict_SpanMargin = new ConcurrentDictionary<string, double[]>();
        ConcurrentDictionary<string, double[]> dict_EODMargin = new ConcurrentDictionary<string, double[]>();
        ConcurrentDictionary<string, double[]> dict_ExpirySpanMargin = new ConcurrentDictionary<string, double[]>();

        //ConcurrentDictionary<string, double> dict_EQSpan = new ConcurrentDictionary<string, double>();//11-12-2019

        /// <summary>
        /// Key : Segment|ScripName | Value : ScripInfo
        /// </summary>
        ConcurrentDictionary<string, ContractMaster> dict_ScripInfo = new ConcurrentDictionary<string, ContractMaster>();

        /// <summary>
        /// Key : Segment|CustomScripName | Value : ScripInfo
        /// </summary>
        ConcurrentDictionary<string, ContractMaster> dict_CustomScripInfo = new ConcurrentDictionary<string, ContractMaster>();

        /// <summary>
        /// Key : Segment|Token | Value : ScripInfo
        /// </summary>
        ConcurrentDictionary<string, ContractMaster> dict_TokenScripInfo = new ConcurrentDictionary<string, ContractMaster>();

        /// <summary>
        /// Stores current Span requests. Gets cleared after NImage or Engine Started.
        /// </summary>
        ConcurrentQueue<Span> q_SpanRequests = new ConcurrentQueue<Span>();

        /// <summary>
        /// Stores all Span requests. Gets cleared after Engine started. Gets copied to q_SpanRequests after NImage Crashes.
        /// </summary>
        ConcurrentQueue<Span> q_TotalSpanRequests = new ConcurrentQueue<Span>();

        /// <summary>
        /// Contains SpanKeys passed to getMargin()
        /// </summary>
        HashSet<string> hs_CalculatedSpanKeys = new HashSet<string>();

        /// <summary>
        /// Contains span file names that are used
        /// </summary>
        HashSet<string> hs_SpanFileNames = new HashSet<string>();

        /// <summary>
        /// Expiry time for FO segment
        /// </summary>
        DateTime dt_ExpiryTime = DateTime.Parse("15:30:00");

        //changed params on 13JAN2021 by Amey
        //changed params on 15JAN2021 by Amey
        public DumpData(string _ReceivedMySQLCon, IPAddress GatewayHBServerIP, int GatewayServerHBPORT, clsWriteLog _Receivedlogger, DataSet ds_ReceivedConfig,
            string _SpanFolderPath)
        {
            try
            {
                _MySQLCon = _ReceivedMySQLCon;

                _logger = _Receivedlogger;
                ds_Config = ds_ReceivedConfig;
                this._SpanFolderPath = _SpanFolderPath;

                ReadConfig();

                bCast_Trades = xd_TradeClient.Broadcasters.GetBroadcasterForMode(XDTransportMode.HighPerformanceUI);
                bCast_Span = xd_SpanClient.Broadcasters.GetBroadcasterForMode(XDTransportMode.HighPerformanceUI);

                if (xd_EngineListner != null)
                    xd_EngineListner.Dispose();
                xd_EngineListner = xd_EngineClient.Listeners.GetListenerForMode(XDTransportMode.HighPerformanceUI);
                xd_EngineListner.MessageReceived += ReceiveResponseFromEngine;
                xd_EngineListner.RegisterChannel("n.ENGINE");

                _HeartBeatServer.eveError += _logger.Error;
                _HeartBeatServer.Setup(GatewayHBServerIP, GatewayServerHBPORT);
            }
            catch (Exception Error)
            {
                _logger.Error("DumpData CTor : " + Error);
            }
        }

        #region Supplimentary Methods

        //added on 15JAN2021 by Amey
        private void ReadConfig()
        {
            try
            {
                _nImageExeName = ds_Config.Tables["NIMAGE"].Rows[0]["EXE"].ToString();

                isDebug = Convert.ToBoolean(ds_Config.Tables["DEBUG-MODE"].Rows[0]["ENABLE"].ToString());
                isAdmin = Convert.ToBoolean(ds_Config.Tables["NIMAGE"].Rows[0]["ADMIN"].ToString());

                foreach (var _Time in ds_Config.Tables["SPAN"].Rows[0]["RECOMPUTE-TIME"].ToString().Split(','))
                {
                    double waitSeconds = (DateTime.Parse(_Time) - DateTime.Now).TotalSeconds;
                    if (waitSeconds > 0)
                        Task.Delay(Convert.ToInt32(waitSeconds * 1000)).ContinueWith(t => ReloadAndRecalculateMargin());
                }
            }
            catch (Exception Error)
            {
                _logger.Error("ReadConfig : " + Error);
            }
        }

        public PositionInfo CopyPropertiesFrom(PositionInfo self, PositionInfo parent)
        {
            var fromProperties = parent.GetType().GetProperties();
            var toProperties = self.GetType().GetProperties();

            foreach (var fromProperty in fromProperties)
            {
                foreach (var toProperty in toProperties)
                {
                    if (fromProperty.Name == toProperty.Name && fromProperty.PropertyType == toProperty.PropertyType)
                    {
                        toProperty.SetValue(self, fromProperty.GetValue(parent));
                        break;
                    }
                }
            }

            return self;
        }

        //TODO: Make seperate class for such methods.
        private DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        //TODO: Make seperate class for such methods.
        private double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - origin;
            return diff.TotalSeconds;
        }

        //TODO: Make seperate class for such methods.
        private string CompressString(string text)
        {
            byte[] send = { };
            try
            {
                //DateTime dtime = DateTime.Now;
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                var memoryStream = new MemoryStream();
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                }

                memoryStream.Position = 0;

                var compressedData = new byte[memoryStream.Length];
                memoryStream.Read(compressedData, 0, compressedData.Length);

                var gZipBuffer = new byte[compressedData.Length + 4];
                Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
                send = gZipBuffer;
                //Console.WriteLine("Gtime- " + (DateTime.Now - dtime));
                return Convert.ToBase64String(send);
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        //TODO: Make seperate class for such methods.
        private string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        //TODO: Make seperate class for such methods.
        private void StartProcess(string FileName, bool isAdmin)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = FileName;
                proc.StartInfo.UseShellExecute = true;

                if (isAdmin)
                    proc.StartInfo.Verb = "runas";

                proc.Start();
            }
            catch (Exception ee) { _logger.Error("StartProcess : " + ee.ToString()); }
        }

        //TODO: Make seperate class for such methods.
        private void EndProcess(string ProcessName)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = $"/im {ProcessName} /f /t",
                CreateNoWindow = true,
                UseShellExecute = false
            }).WaitForExit();
        }

        #endregion

        #region Important Methods

        //changed on 15JAN2021 by Amey
        private void ReceiveResponseFromEngine(object sender, XDMessageEventArgs e)
        {
            try
            {
                //changed on 12JAN2021 by Amey
                lock (list_AllTrades)
                {
                    //changed on 15JAN2021 by Amey
                    if (e.DataGram.Message.Contains("START"))
                    {
                        //added on 22JAN2021 by Amey
                        eve_AddToList("Engine Started.");

                        //added on 15JAN2021 by Amey
                        _FOLastTradeTime = 0;
                        _CMLastTradeTime = 0;
                        _SpanComputeTime = 0;
                        isSpanConnected = false;

                        q_SpanRequests = new ConcurrentQueue<Span>();           //18-03-2020

                        //added on 11OCT2020 by Amey
                        q_TotalSpanRequests = new ConcurrentQueue<Span>();

                        //added on 17DEC2020 by Amey
                        hs_CalculatedSpanKeys.Clear();

                        dict_RandomEntriesCM.Clear();
                        dict_RandomEntriesFO.Clear();

                        //added on 26APR2021 by Amey
                        dict_RandomEntriesBSECM.Clear();

                        //changed on 12JAN2021 by Amey
                        list_AllTrades.Clear();

                        SelectClientfromDatabase();
                        ReadEODTable();

                        IsEngineStarted = true;

                        //added on 17MAY2021 by Amey. To Display Span file info at Prime frontend.
                        var SpanDirectory = new DirectoryInfo(_SpanFolderPath);
                        LatestSpanFileName = SpanDirectory.GetFiles("nsccl*.spn").Any() ? 
                                    SpanDirectory.GetFiles("nsccl*.spn")
                                   .OrderByDescending(f => f.LastWriteTime)
                                   .First().Name : "";

                        if (LatestSpanFileName == "")
                            eve_AddToList($"Span file is not available. Please check BOD Utility for Span file status.");
                        else if (!hs_SpanFileNames.Contains(LatestSpanFileName))
                            hs_SpanFileNames.Add(LatestSpanFileName);

                        //changed location on 01FEB2021 by Amey
                        RestartAndComputeSpan();

                        SpanComputeStart = true;

                        //added extra params on 17MAY2021 by Amey
                        //changed on 15JAN2021 by Amey
                        Task.Run(() => _HeartBeatServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                    }
                    else if (e.DataGram.Message.Contains("STOP"))
                    {
                        //added on 22JAN2021 by Amey
                        eve_AddToList("Engine Stopped.");

                        IsEngineStarted = false;
                        SpanComputeStart = false;

                        //added on 11JUN2021 by Amey
                        isSpanConnected = false;
                        _SpanConnector = null;
                        EndProcess(_nImageExeName);
                    }
                }
            }
            catch (Exception error)
            {
                _logger.Error("ReceiveResponseFromEngine : " + error);
            }
        }

        private void ReadEODTable()
        {
            //added on 13JAN2021 by Amey
            list_EODPositions.Clear();

            try
            {
                var dt_EOD = new ds_Gateway.dt_EODPositionsDataTable();
                using (MySqlConnection _mySqlConnection = new MySqlConnection(_MySQLCon))
                {
                    _mySqlConnection.Open();

                    using (MySqlCommand myCmdEod = new MySqlCommand("sp_GetEOD", _mySqlConnection))
                    {
                        myCmdEod.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataAdapter dadapter = new MySqlDataAdapter(myCmdEod))
                        {
                            //changed on 13JAN2021 by Amey
                            dadapter.Fill(dt_EOD);

                            dadapter.Dispose();
                        }
                    }

                    _mySqlConnection.Close();
                }

                //added on 10FEB2021 by Amey
                list_EODPositions = dt_EOD.AsEnumerable().Select(v => new PositionInfo
                {
                    Username = v.Username,
                    Segment = v.Segment == "NSECM" ? en_Segment.NSECM : (v.Segment == "NSECD" ? en_Segment.NSECD :
                        (v.Segment == "NSEFO" ? en_Segment.NSEFO : en_Segment.BSECM)),
                    Token = v.Token,                   
                    TradePrice = v.TradePrice,
                    TradeQuantity = v.TradeQuantity,
                    UnderlyingSegment = v.UnderlyingSegment == "NSECM" ? en_Segment.NSECM : (v.Segment == "NSECD" ? en_Segment.NSECD :
                        (v.Segment == "NSEFO" ? en_Segment.NSEFO : en_Segment.BSECM)),
                    UnderlyingToken = v.UnderlyingToken
                }).ToList();
            }
            catch (Exception ee)
            {
                _logger.Error("ReadEODTable : " + ee);
            }
        }

        private void RestartAndComputeSpan()
        {
            try
            {
                //added on 02FEB2021 by Amey
                if (!IsEngineStarted) return;

                if (isDebug)
                    _logger.Error("Initialising Image.", isDebug);

                isSpanConnected = false;
                _SpanConnector = null;

                dict_SpanMargin.Clear();

                EndProcess(_nImageExeName);

                //added on 15JAN2021 by Amey
                StartProcess(Application.StartupPath + "\\" + _nImageExeName, isAdmin);

                //added on 22OCT2020 by Amey.
                q_SpanRequests = new ConcurrentQueue<Span>(q_TotalSpanRequests);

                //added on 01FEB2021 by Amey
                Thread.Sleep(300);

                //added on 15JAN2021 by Amey
                Task.Run(() => ComputeSpan());
            }
            catch (Exception error)
            {
                _logger.Error("InitialiseSpan : " + error);
            }
        }

        private void ComputeSpan()
        {
            try
            {
                //added on 15JAN2021 by Amey
                string[] arr_Margin = new string[0];
                string[] arr_EODMargin = new string[0];     //added on 22JAN2021 by Akshay
                string[] arr_ExpiryMargin = new string[0];  //added on 22JAN2021 by Akshay

                //Added on 29SEP2020 by Amey
                IPAddress ImageIP = IPAddress.Parse(ds_Config.Tables["NIMAGE"].Rows[0]["ADDRESS"].ToString().Split(':')[0]);
                int ImagePORT = Convert.ToInt32(ds_Config.Tables["NIMAGE"].Rows[0]["ADDRESS"].ToString().Split(':')[1]);
                _SpanConnector = SpanConnectorFO.Instance(ImageIP, ImagePORT);

                SelectNearestExpiryDate();  //added on 22JAN2021 by Akshay 

                if (_SpanConnector != null)
                {
                    if (isDebug)
                        _logger.Error("Image Initialised successfully", isDebug);

                    isSpanConnected = true;

                    //added extra params on 17MAY2021 by Amey
                    //changed on 15JAN2021 by Amey
                    Task.Run(() => _HeartBeatServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));

                    if (isDebug)
                        _logger.Error("Computing span for eod data TotalRows : " + list_EODPositions.Count, isDebug);

                    //changed on 13JAN2021 by Amey
                    for (int EODIdx = 0; EODIdx < list_EODPositions.Count; EODIdx++)
                    {
                        try
                        {
                            //added on 10FEB2021 by Amey
                            var _PositionInfo = list_EODPositions[EODIdx];

                            //added on 20APR2021 by Amey
                            var ScripKey = $"{_PositionInfo.Segment}|{_PositionInfo.Token}";

                            //added on 26MAY2021 by Amey
                            if (dict_TokenScripInfo.TryGetValue(ScripKey, out ContractMaster ScripInfo))
                            {
                                DateTime dtExpiry = ConvertFromUnixTimestamp(ScripInfo.ExpiryUnix);

                                //no need to calculate margin for expired contracts. Added on 26APR2021 by Amey.
                                if (DateTime.Now.Date == dtExpiry.Date && DateTime.Now > dt_ExpiryTime)
                                    continue;

                                string SpanKey = $"{_PositionInfo.Username}_{ScripInfo.Symbol}";          //Added on 22JAN2021 by Akshay
                                string EODKey = $"{_PositionInfo.Username}_{ScripInfo.Symbol}_EOD";       //Added on 22JAN2021 by Akshay
                                string ExpiryKey = $"{_PositionInfo.Username}_{ScripInfo.Symbol}_EXP";    //Added on 22JAN2021 by Akshay

                                if (ScripInfo.ScripType != en_ScripType.EQ)
                                {
                                    //changed on 15JAN2021 by Amey
                                    if (isSpanConnected)
                                    {
                                        try
                                        {
                                            if (ScripInfo.ScripType == en_ScripType.XX)
                                            {
                                                arr_Margin = _SpanConnector.GetMargin(1, SpanKey, "NSE", "EQFO", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), "", "", "E", _PositionInfo.TradeQuantity.ToString());
                                                arr_EODMargin = _SpanConnector.GetMargin(1, EODKey, "NSE", "EQFO", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), "", "", "E", _PositionInfo.TradeQuantity.ToString());

                                                //Changed on 22JAN2021 by Akshay
                                                if (dtNearestExpiry < dtExpiry)  //Changed on 22JAN2021 by Akshay
                                                {
                                                    arr_ExpiryMargin = _SpanConnector.GetMargin(1, ExpiryKey, "NSE", "EQFO", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), "", "", "E", _PositionInfo.TradeQuantity.ToString());

                                                    if (!dict_ExpirySpanMargin.ContainsKey(ExpiryKey))
                                                        dict_ExpirySpanMargin.TryAdd(ExpiryKey, new double[3] { Convert.ToDouble(arr_ExpiryMargin[0]), Convert.ToDouble(arr_ExpiryMargin[1]), 0 });
                                                    else
                                                    {
                                                        dict_ExpirySpanMargin[ExpiryKey][0] = Convert.ToDouble(arr_ExpiryMargin[0]);
                                                        dict_ExpirySpanMargin[ExpiryKey][1] = Convert.ToDouble(arr_ExpiryMargin[1]);
                                                    }
                                                    hs_CalculatedSpanKeys.Add(ExpiryKey);  //Added by Akshay on 21-12-2020 
                                                }
                                            }
                                            else
                                            {
                                                arr_Margin = _SpanConnector.GetMargin(1, SpanKey, "NSE", "EQFO", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), ScripInfo.StrikePrice.ToString("N2").Replace(",", ""), ScripInfo.ScripType.ToString().Substring(0, 1), "E", _PositionInfo.TradeQuantity.ToString());
                                                arr_EODMargin = _SpanConnector.GetMargin(1, EODKey, "NSE", "EQFO", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), ScripInfo.StrikePrice.ToString("N2").Replace(",", ""), ScripInfo.ScripType.ToString().Substring(0, 1), "E", _PositionInfo.TradeQuantity.ToString());

                                                //Changed on 22JAN2021 For Nearest expirydate
                                                if (dtNearestExpiry < dtExpiry)  //Changed on 22JAN2021 For Nearest expirydate
                                                {
                                                    arr_ExpiryMargin = _SpanConnector.GetMargin(1, ExpiryKey, "NSE", "EQFO", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), ScripInfo.StrikePrice.ToString("N2").Replace(",", ""), ScripInfo.ScripType.ToString().Substring(0, 1), "E", _PositionInfo.TradeQuantity.ToString());

                                                    if (!dict_ExpirySpanMargin.ContainsKey(ExpiryKey))
                                                        dict_ExpirySpanMargin.TryAdd(ExpiryKey, new double[3] { Convert.ToDouble(arr_ExpiryMargin[0]), Convert.ToDouble(arr_ExpiryMargin[1]), 0 });
                                                    else
                                                    {
                                                        dict_ExpirySpanMargin[ExpiryKey][0] = Convert.ToDouble(arr_ExpiryMargin[0]);
                                                        dict_ExpirySpanMargin[ExpiryKey][1] = Convert.ToDouble(arr_ExpiryMargin[1]);
                                                    }
                                                    hs_CalculatedSpanKeys.Add(ExpiryKey);  //Added by Akshay on 21-12-2020 
                                                }
                                            }

                                            if (!dict_SpanMargin.ContainsKey(SpanKey))
                                                dict_SpanMargin.TryAdd(SpanKey, new double[3] { Convert.ToDouble(arr_Margin[0]), Convert.ToDouble(arr_Margin[1]), 0 });
                                            else
                                            {
                                                dict_SpanMargin[SpanKey][0] = Convert.ToDouble(arr_Margin[0]);
                                                dict_SpanMargin[SpanKey][1] = Convert.ToDouble(arr_Margin[1]);
                                            }

                                            //Changed on 22JAN2021 by Akshay
                                            if (!dict_EODMargin.ContainsKey(EODKey))
                                                dict_EODMargin.TryAdd(EODKey, new double[3] { Convert.ToDouble(arr_EODMargin[0]), Convert.ToDouble(arr_EODMargin[1]), 0 });
                                            else
                                            {
                                                dict_EODMargin[EODKey][0] = Convert.ToDouble(arr_EODMargin[0]);
                                                dict_EODMargin[EODKey][1] = Convert.ToDouble(arr_EODMargin[1]);
                                            }

                                            //added on 17DEC2020 by Amey
                                            hs_CalculatedSpanKeys.Add(SpanKey);
                                            hs_CalculatedSpanKeys.Add(EODKey);
                                        }
                                        catch (Exception error)
                                        {
                                            isSpanConnected = false;

                                            _logger.Error("Span status false, margin[0] " + arr_Margin[0] + ", margin[1] " + arr_Margin[1] + ", margin[2] " + arr_Margin[2] + " for request " + EODKey + " , " + "NSE" + " , " + "EQFO" + " , " + dtExpiry.ToString("yyyy-MM-dd").Replace("-", "") + " , " + ScripInfo.StrikePrice.ToString("N2").Replace(",", "") + " , " + ScripInfo.ScripType.ToString().Substring(0, 1) + " , " + "E" + " , " + _PositionInfo.TradeQuantity + " , " + error);

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception inerEx)
                        {
                            isSpanConnected = false;

                            _logger.Error("Span status false, Span if eod data at " + EODIdx + "_" + inerEx);

                            RestartAndComputeSpan(); return;
                        }
                    }

                    if (!isSpanConnected)
                    {
                        //added extra params on 17MAY2021 by Amey
                        //changed on 15JAN2021 by Amey
                        Task.Run(() => _HeartBeatServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));

                        RestartAndComputeSpan(); return;
                    }
                    else
                    {
                        if (isDebug)
                            _logger.Error("Span for EOD data computed successfully , SpanComputeStart " + SpanComputeStart + ", Intraday scrip count " + q_SpanRequests.Count, isDebug);
                        
                        //Added on 22JAN2021 by Akshay
                        bCast_Span.SendToChannel("n.SPAN", CompressString("ALL^" + JsonConvert.SerializeObject(dict_SpanMargin)));

                        if (isDebug)
                            _logger.Error("EOD Span Rows : " + dict_EODMargin.Count, isDebug);

                        Thread.Sleep(1000); //Added on 22JAN2021 by Akshay

                        bCast_Span.SendToChannel("n.SPAN", CompressString("EOD^" + JsonConvert.SerializeObject(dict_EODMargin)));
                        //_logger.Error("EOD Span data " + JsonConvert.SerializeObject(dict_EODMargin), isDebug);
                        Thread.Sleep(1000); //Added on 22JAN2021 by Akshay

                        bCast_Span.SendToChannel("n.SPAN", CompressString("EXPIRY^" + JsonConvert.SerializeObject(dict_ExpirySpanMargin)));
                        //_logger.Error("EOD Span data " + JsonConvert.SerializeObject(dict_ExpirySpanMargin), isDebug);

                        //added on 17MAY2021 by Amey
                        _SpanComputeTime = ConvertToUnixTimestamp(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 15, 00));

                        //added extra params on 17MAY2021 by Amey
                        //changed on 15JAN2021 by Amey
                        Task.Run(() => _HeartBeatServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                    }

                    #region added by Navin on 28-01-2020

                    while (isSpanConnected)
                    {
                        //changed on 15JAN2021 by Amey
                        if (SpanComputeStart)
                        {
                            if (_SpanConnector != null)
                            {
                                Span _Span = new Span();
                                bool _IsSpanComputed = false;

                                if (q_SpanRequests.Any())
                                {
                                    while (q_SpanRequests.TryDequeue(out _Span))
                                    {
                                        try
                                        {
                                            _IsSpanComputed = true;
                                            try
                                            {
                                                //Added on 22JAN2021 by Akshay
                                                DateTime dtExpiry = DateTime.ParseExact(_Span.pExpiry, "yyyyMMdd", CultureInfo.InvariantCulture);

                                                //no need to calculate margin for expired contracts. Added on 26APR2021 by Amey.
                                                if (DateTime.Now.Date == dtExpiry.Date && DateTime.Now > dt_ExpiryTime)
                                                    continue;

                                                if (dtNearestExpiry < dtExpiry)
                                                {
                                                    string ExpiryKey = _Span.pClientId + "_EXP";
                                                    arr_Margin = _SpanConnector.GetMargin(_Span.pMemberId, ExpiryKey, _Span.pExchange, _Span.pSegment, _Span.pScripName, _Span.pExpiry, _Span.pStrikePrice, _Span.pCallPut, _Span.pFactor, _Span.pQty);

                                                    if (!dict_ExpirySpanMargin.ContainsKey(ExpiryKey))
                                                        dict_ExpirySpanMargin.TryAdd(ExpiryKey, new double[3] { Convert.ToDouble(arr_Margin[0]), Convert.ToDouble(arr_Margin[1]), 0 });
                                                    else
                                                    {
                                                        dict_ExpirySpanMargin[ExpiryKey][0] = Convert.ToDouble(arr_Margin[0]);
                                                        dict_ExpirySpanMargin[ExpiryKey][1] = Convert.ToDouble(arr_Margin[1]);
                                                    }
                                                    hs_CalculatedSpanKeys.Add(ExpiryKey);
                                                }

                                                arr_Margin = _SpanConnector.GetMargin(_Span.pMemberId, _Span.pClientId, _Span.pExchange, _Span.pSegment, _Span.pScripName, _Span.pExpiry, _Span.pStrikePrice, _Span.pCallPut, _Span.pFactor, _Span.pQty);
                                                if (!dict_SpanMargin.ContainsKey(_Span.pClientId))
                                                    dict_SpanMargin.TryAdd(_Span.pClientId, new double[3] { Convert.ToDouble(arr_Margin[0]), Convert.ToDouble(arr_Margin[1]), 0 });
                                                else
                                                {
                                                    dict_SpanMargin[_Span.pClientId][0] = Convert.ToDouble(arr_Margin[0]);
                                                    dict_SpanMargin[_Span.pClientId][1] = Convert.ToDouble(arr_Margin[1]);
                                                }

                                                //added on 17DEC2020 by Amey
                                                hs_CalculatedSpanKeys.Add(_Span.pClientId);

                                                //added on 11JUN2021 by Amey
                                                _SpanComputeTime = _Span.pTradeTime;
                                            }
                                            catch (Exception error)
                                            {
                                                isSpanConnected = false;

                                                _logger.Error("Span status false, " + _Span.pMemberId + " , " + _Span.pClientId + " , " + _Span.pExchange + " , " + _Span.pSegment + " , " + _Span.pScripName + " , " + _Span.pExpiry + " , " + _Span.pStrikePrice + " , " + _Span.pCallPut + " , " + _Span.pFactor + " , " + _Span.pQty + ", Span disconnected  _ " + error);
                                            }
                                        }
                                        catch (Exception error)
                                        {
                                            isSpanConnected = false;

                                            _logger.Error("Span status false, " + _Span.pMemberId + " , " + _Span.pClientId + " , " + _Span.pExchange + " , " + _Span.pSegment + " , " + _Span.pScripName + " , " + _Span.pExpiry + " , " + _Span.pStrikePrice + " , " + _Span.pCallPut + " , " + _Span.pFactor + " , " + _Span.pQty + "   _ " + error);
                                        }
                                    }
                                }

                                if (_IsSpanComputed)
                                {
                                    //Changed on 22JAN2021 by Akshay
                                    bCast_Span.SendToChannel("n.SPAN", CompressString("ALL^" + JsonConvert.SerializeObject(dict_SpanMargin)));   //04-12-2019

                                    if (isDebug)
                                        _logger.Error("Span Rows : " + dict_SpanMargin.Count, isDebug);         
                                   
                                    Thread.Sleep(1000); //Added on 22JAN2021 by Akshay

                                    bCast_Span.SendToChannel("n.SPAN", CompressString("EXPIRY^" + JsonConvert.SerializeObject(dict_ExpirySpanMargin)));   //04-12-2019

                                    //commented on 10JUN2021 by Amey. Added TradeTime as SpanComputeTime.
                                    //added on 17MAY2021 by Amey
                                    //_SpanComputeTime = ConvertToUnixTimestamp(DateTime.Now);

                                    //added extra params on 17MAY2021 by Amey
                                    //changed on 15JAN2021 by Amey
                                    Task.Run(() => _HeartBeatServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                                }
                            }
                        }

                        if (!isSpanConnected)//05-02-2020
                        {
                            //added extra params on 17MAY2021 by Amey
                            //changed on 15JAN2021 by Amey
                            Task.Run(() => _HeartBeatServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));

                            //added on 15JAN2021 by Amey
                            RestartAndComputeSpan();
                        }

                        Thread.Sleep(250);
                    }

                    #endregion
                }
                else
                {
                    if (isDebug)
                        _logger.Error("Image does not initialised properly ", isDebug);

                    //added extra params on 17MAY2021 by Amey
                    //changed on 15JAN2021 by Amey
                    Task.Run(() => _HeartBeatServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                }
            }
            catch (Exception error)
            {
                isSpanConnected = false;

                //added extra params on 17MAY2021 by Amey
                //changed on 15JAN2021 by Amey
                Task.Run(() => _HeartBeatServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));

                _logger.Error("Span status false, ComputeSpan " + error);

                //added on 15JAN2021 by Amey
                RestartAndComputeSpan();
            }
        }

        DateTime dtNearestExpiry; //Added on 22JAN2021 by Akshay

        #region SelectNearestExpiryDate
        //Added on 22JAN2021 by Akshay
        private void SelectNearestExpiryDate()
        {
            try
            {
                using (MySqlConnection mySqlArrcsDBConn = new MySqlConnection(_MySQLCon))
                {
                    //changed to SP on 27APR2021 by Amey
                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetNearestExpiry", mySqlArrcsDBConn))
                    {
                        if (mySqlArrcsDBConn.State == ConnectionState.Closed) 
                            mySqlArrcsDBConn.Open();

                        myCmd.CommandType = CommandType.StoredProcedure;

                        using (MySqlDataReader _mySqlDataReader = myCmd.ExecuteReader())
                        {
                            while (_mySqlDataReader.Read())
                            {
                                try
                                {
                                    dtNearestExpiry = ConvertFromUnixTimestamp(Convert.ToDouble(_mySqlDataReader.GetString(0)));
                                }
                                catch (Exception ee) { _logger.Error("SelectNearestExpiryDate " + ee.ToString()); }
                            }
                            _mySqlDataReader.Close();
                            _mySqlDataReader.Dispose();
                        }
                        mySqlArrcsDBConn.Close();
                    }
                }
                
            }
            catch (Exception clientEx)
            {
                _logger.Error("SelectNearestExpiryDate " + clientEx.ToString());
            }
        }
        #endregion

        private void SelectClientfromDatabase()
        {
            try
            {
                dict_ClientInfo.Clear();

                //added on 15JAN2021 by Amey
                using (MySqlConnection _Con = new MySqlConnection(_MySQLCon))
                {
                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetClientDetail", _Con))
                    {
                        _Con.Open();

                        myCmd.CommandType = CommandType.StoredProcedure;

                        //added on 27APR2021 by Amey
                        myCmd.Parameters.Add("prm_Type", MySqlDbType.LongText);
                        myCmd.Parameters["prm_Type"].Value = "ALL";

                        using (MySqlDataReader _mySqlDataReader = myCmd.ExecuteReader())
                        {
                            while (_mySqlDataReader.Read())
                            {
                                try
                                {
                                    //added on 11JAN2021 by Amey
                                    string ClienCode = _mySqlDataReader.GetString(0);
                                    string CTCL_ID = _mySqlDataReader.GetString(1);
                                    string UserID = _mySqlDataReader.GetString(2);

                                    string Username = _mySqlDataReader.GetString(3);

                                    if (ClienCode != "" && CTCL_ID != "" && UserID != "")
                                        dict_ClientInfo.TryAdd($"{ClienCode}|{CTCL_ID}|{UserID}", Username);
                                    else if (ClienCode != "")
                                        dict_ClientInfo.TryAdd(ClienCode, Username);
                                    else if (CTCL_ID != "")
                                        dict_ClientInfo.TryAdd(CTCL_ID, Username);
                                    else if (UserID != "")
                                        dict_ClientInfo.TryAdd(UserID, Username);
                                }
                                catch (Exception) { }
                            }
                            _mySqlDataReader.Close();
                            _mySqlDataReader.Dispose();
                        }

                        _Con.Close();
                    }
                }
            }
            catch (Exception clientEx)
            {
                _logger.Error("SelectClientfromDatabase  : " + clientEx);
            }
        }

        //added on 16APR2021 by Amey
        private void ReadContractMaster()
        {
            try
            {
                var dt_ContractMaster = new ds_Gateway.dt_ContractMasterDataTable();
                using (MySqlConnection _mySqlConnection = new MySqlConnection(_MySQLCon))
                {
                    _mySqlConnection.Open();

                    using (MySqlCommand myCmdEod = new MySqlCommand("sp_GetContractMaster", _mySqlConnection))
                    {
                        myCmdEod.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataAdapter dadapter = new MySqlDataAdapter(myCmdEod))
                        {
                            //changed on 13JAN2021 by Amey
                            dadapter.Fill(dt_ContractMaster);

                            dadapter.Dispose();
                        }
                    }

                    _mySqlConnection.Close();
                }

                ContractMaster ScripInfo = null;
                foreach (ds_Gateway.dt_ContractMasterRow v in dt_ContractMaster.Rows)
                {
                    ScripInfo = new ContractMaster()
                    {
                        Token = v.Token,
                        Series = v.Series,
                        Symbol = v.Symbol,
                        InstrumentName = v.InstrumentName == "EQ" ? en_InstrumentName.EQ : (v.InstrumentName == "FUTIDX" ? en_InstrumentName.FUTIDX :
                        (v.InstrumentName == "FUTSTK" ? en_InstrumentName.FUTSTK : (v.InstrumentName == "OPTIDX" ? en_InstrumentName.OPTIDX : en_InstrumentName.OPTSTK))),
                        Segment = v.Segment == "NSECM" ? en_Segment.NSECM : (v.Segment == "NSECD" ? en_Segment.NSECD :
                        (v.Segment == "NSEFO" ? en_Segment.NSEFO : en_Segment.BSECM)),
                        ScripName = v.ScripName,
                        CustomScripName = v.CustomScripName,
                        ScripType = (v.ScripType == "EQ" ? en_ScripType.EQ : (v.ScripType == "XX" ? en_ScripType.XX : (v.ScripType == "CE" ? en_ScripType.CE :
                                    en_ScripType.PE))),
                        ExpiryUnix = v.ExpiryUnix,
                        StrikePrice = v.StrikePrice,
                        LotSize = v.LotSize,
                        UnderlyingToken = v.UnderlyingToken,
                        UnderlyingSegment = v.UnderlyingSegment == "NSECM" ? en_Segment.NSECM : (v.UnderlyingSegment == "NSECD" ? en_Segment.NSECD :
                        (v.UnderlyingSegment == "NSEFO" ? en_Segment.NSEFO : en_Segment.BSECM))
                    };

                    dict_ScripInfo.TryAdd($"{ScripInfo.Segment}|{ScripInfo.ScripName}", ScripInfo);
                    dict_CustomScripInfo.TryAdd($"{ScripInfo.Segment}|{ScripInfo.CustomScripName}", ScripInfo);
                    dict_TokenScripInfo.TryAdd($"{ScripInfo.Segment}|{ScripInfo.Token}", ScripInfo);
                }
            }
            catch (Exception ee) { _logger.Error("ReadContractMaster : " + ee); }
        }

        public void ReadIntradayFiles()
        {
            ReadContractMaster();

            while (true)
            {
                try
                {
                    //changed on 12JAN2021 by Amey
                    list_AllTrades.Clear();

                    DateTime dte_StartReadingTrades = DateTime.Now;

                    //added for testing
                    //SelectClientfromDatabase();
                    //IsEngineStarted = true;

                    //removed flatFile param on 13JAN2021 by Amey
                    if (IsEngineStarted)
                        Parallel.Invoke(() => ReadFO(_TradeFileFO), () => ReadCM(_TradeFileCM), () => ReadBSECM(_TradeFileBSECM));

                    //added on 25JAN2021 by Amey
                    eve_TradeFileStatus(isTradeFileFOAvailable, isTradeFileCMAvailable, isTradeFileBSECMAvailable);

                    try
                    {
                        //changed to Any() on 28APR2021 by Amey
                        if (IsEngineStarted && list_AllTrades.Any())
                        {
                            if (isDebug)
                                _logger.Error("Trades Read in " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs", isDebug);

                            bCast_Trades.SendToChannel("n.TRADE", CompressString("ALL_" + JsonConvert.SerializeObject(list_AllTrades)));

                            //added on 28MAY2021 by Amey
                            if (dict_SpanMargin.Any())
                            {
                                //commented on 10JUN2021 by Amey. Added TradeTime as SpanComputeTime.
                                //added on 17MAY2021 by Amey
                                //_SpanComputeTime = ConvertToUnixTimestamp(DateTime.Now);

                                //added on 28JAN2021 by Amey
                                bCast_Span.SendToChannel("n.SPAN", CompressString("ALL^" + JsonConvert.SerializeObject(dict_SpanMargin)));
                            }
							
                            if (isDebug)
                                _logger.Error("Total Trade Rows Sent," + list_AllTrades.Count, isDebug);

                            //added extra params on 17MAY2021 by Amey
                            //changed on 15JAN2021 by Amey
                            Task.Run(() => _HeartBeatServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                        }
                    }
                    catch (Exception ee)
                    {
                        _logger.Error("Exception occurred while sending flatfiles over socket" + DateTime.Now + Environment.NewLine + ee.ToString());
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error("ReadIntraDay -OUT : " + ee);
                }

                Thread.Sleep(50);
            }
        }

        private void ReadFO(string FileName)
        {
            try
            {
                PositionInfo _PositionInfo;

                var arr_Combinations = new string[TotalCombinations] { "", "", "", "" };

                //added on 19APR2021 by Amey
                var ScripNameKey = string.Empty;
                var CustomScripNameKey = string.Empty;
                var Underlying = string.Empty;
                var ScripType = en_ScripType.EQ;
                var StrikePrice = 0.0;
                DateTime _ExpiryDate = new DateTime(1980, 1, 1, 0, 0, 0);

                //changed on 15JAN2021 by Amey
                foreach (var _NoticeFOFilePath in ds_Config.Tables["INTRADAY"].Rows[0]["NOTICE-FO"].ToString().Split(','))
                {
                    string[] arr_FileEntry = Directory.GetFiles(_NoticeFOFilePath, FileName + ".txt");
                    if (arr_FileEntry.Length > 0)
                    {
                        try
                        {
                            string FullFilePath = arr_FileEntry[0];

                            if (!dict_RandomEntriesFO.ContainsKey(FullFilePath))//added on 07-02-2020
                                dict_RandomEntriesFO.TryAdd(FullFilePath, new HashSet<string>());

                            using (FileStream fs = File.Open(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                using (BufferedStream bs = new BufferedStream(fs))
                                {
                                    using (StreamReader sr = new StreamReader(bs))
                                    {
                                        string strData = string.Empty;
                                        while ((strData = sr.ReadLine()) != null)
                                        {
                                            //added on 25JAN2021 by Amey
                                            isTradeFileFOAvailable = true;

                                            //added on 05JAN2021 by Amey
                                            string[] arr_Fields = strData.ToUpper().Split(',').Select(v => v.Trim()).ToArray();

                                            if (arr_Fields.Length > 26 && arr_Fields[0] != "") //added on 2-1-18
                                            {
                                                string TradeID = arr_Fields[0];

                                                string FullDealerID = arr_Fields[26];
                                                string CTCL_ID = FullDealerID.Substring(0, (FullDealerID.Length > 11 ? 12 : FullDealerID.Length));
                                                string ClientCode = arr_Fields[17];
                                                string UserId = arr_Fields[11];
                                                string ClientInfokey = $"{ClientCode}|{CTCL_ID}|{UserId}";

                                                arr_Combinations = new string[TotalCombinations] { ClientInfokey, ClientCode, CTCL_ID, UserId };

                                                //changed on 10FEB2021 by Amey
                                                _PositionInfo = new PositionInfo();

                                                bool isValidTrade = false;

                                                for (int i = 0; i < arr_Combinations.Length; i++)
                                                {
                                                    try
                                                    {
                                                        var UniqueID = arr_Combinations[i];
                                                        if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

                                                        string _check = TradeID + "_" + ClientCode;
                                                        if (dict_RandomEntriesFO[FullFilePath].Contains(_check)) continue;

                                                        //added on 19APR2021 by Amey
                                                        _PositionInfo.Segment = en_Segment.NSEFO;

                                                        //added on 20APR2021 by Amey
                                                        Underlying = arr_Fields[3];
                                                        ScripType = arr_Fields[6] == "" ? en_ScripType.XX : (arr_Fields[6] == "CE" ? en_ScripType.CE : en_ScripType.PE);
                                                        StrikePrice = Convert.ToDouble(ScripType == en_ScripType.XX ? "0" : arr_Fields[5]);
                                                        _ExpiryDate = Convert.ToDateTime(arr_Fields[4]);

                                                        ScripNameKey = _PositionInfo.Segment + "|" + arr_Fields[7].ToUpper();

                                                        //added on 19APR2021 by Amey
                                                        ContractMaster _ScripInfo = null;
                                                        if (!dict_ScripInfo.TryGetValue(ScripNameKey, out _ScripInfo))
                                                        {
                                                            CustomScripNameKey = _PositionInfo.Segment + "|" + $"{Underlying}|{_ExpiryDate.ToString("ddMMMyyyy").ToUpper()}|{(StrikePrice == 0 ? "0" : StrikePrice.ToString("#.00"))}|{ScripType}";

                                                            if (!dict_CustomScripInfo.TryGetValue(CustomScripNameKey, out _ScripInfo))
                                                            {
                                                                if (isDebug)
                                                                    _logger.Error("ReadFO Skipped : " + ScripNameKey + "^" + CustomScripNameKey, true);

                                                                continue;
                                                            }
                                                        }

                                                        _PositionInfo.TradePrice = Convert.ToDouble(arr_Fields[15]);

                                                        //added on 07APR2021 by Amey
                                                        var Qty = long.Parse(arr_Fields[14]);
                                                        if (arr_Fields[13] == "2")
                                                        {
                                                            _PositionInfo.TradeQuantity = Qty * -1;
                                                            _PositionInfo.SellQuantity = Qty;
                                                            _PositionInfo.SellValue = Qty * _PositionInfo.TradePrice;
                                                        }
                                                        else
                                                        {
                                                            _PositionInfo.TradeQuantity = Qty;
                                                            _PositionInfo.BuyQuantity = Qty;
                                                            _PositionInfo.BuyValue = Qty * _PositionInfo.TradePrice;
                                                        }

                                                        //added on 10JUN2021 by Amey
                                                        _PositionInfo.TradeValue = _PositionInfo.TradeQuantity * _PositionInfo.TradePrice;

                                                        _PositionInfo.IntradayQuantity = _PositionInfo.TradeQuantity;

                                                        //changed to TradeValue on 10JUN2021 by Amey
                                                        //changed to - on 13APR2021 by Amey
                                                        //changed computation logic on 07APR2021 by Amey
                                                        _PositionInfo.IntradayValue = _PositionInfo.TradeValue; //_PositionInfo.BuyValue - _PositionInfo.SellValue;

                                                        //added on 01DEC2020 by Amey
                                                        double dbl_TradeTime = ConvertToUnixTimestamp(DateTime.Parse(arr_Fields[25]));
                                                        _FOLastTradeTime = dbl_TradeTime > _FOLastTradeTime ? dbl_TradeTime : _FOLastTradeTime;

                                                        //changed on 19APR2021 by Amey
                                                        _PositionInfo.Token = _ScripInfo.Token;
                                                        _PositionInfo.UnderlyingToken = _ScripInfo.UnderlyingToken;
                                                        _PositionInfo.UnderlyingSegment = _ScripInfo.UnderlyingSegment;

                                                        isValidTrade = true;

                                                        //changed position on 28FEB2021 by Amey
                                                        dict_RandomEntriesFO[FullFilePath].Add(_check);

                                                        break;
                                                    }
                                                    catch (Exception ee) { _logger.Error("ReadFO Loop : " + strData + Environment.NewLine + ee); }
                                                }

                                                if (isValidTrade)
                                                {
                                                    for (int i = 0; i < arr_Combinations.Length; i++)
                                                    {
                                                        var UniqueID = arr_Combinations[i];
                                                        if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

                                                        string Username = dict_ClientInfo[UniqueID];

                                                        #region Span using dll
                                                        try
                                                        {
                                                            Span _spn = new Span()
                                                            {
                                                                pMemberId = 1,
                                                                pClientId = Username + "_" + Underlying,
                                                                pExchange = "NSE",
                                                                pSegment = "EQFO",
                                                                pScripName = Underlying,
                                                                pExpiry = _ExpiryDate.ToString("yyyyMMdd"),
                                                                pFactor = "E",
                                                                pQty = _PositionInfo.TradeQuantity.ToString(),

                                                                //added on 11JUN2021 by Amey
                                                                pTradeTime = _FOLastTradeTime
                                                            };

                                                            if (ScripType == en_ScripType.XX)
                                                            {
                                                                _spn.pStrikePrice = "";
                                                                _spn.pCallPut = "";
                                                            }
                                                            else
                                                            {
                                                                _spn.pStrikePrice = StrikePrice.ToString("#.00");
                                                                _spn.pCallPut = ScripType.ToString().Substring(0, 1);
                                                            }

                                                            q_SpanRequests.Enqueue(_spn);
                                                            q_TotalSpanRequests.Enqueue(_spn);
                                                        }
                                                        catch (Exception spanEx)//added try catch for individual span calculation on 06-01-2020
                                                        {
                                                            _logger.Error("Span calculation individual scrip " + spanEx);
                                                        }

                                                        #endregion

                                                        var newPosInfo = CopyPropertiesFrom(new PositionInfo(), _PositionInfo);
                                                        newPosInfo.Username = Username;

                                                        //changed on 10FEB2021 by Amey
                                                        lock (list_AllTrades)
                                                            list_AllTrades.Add(newPosInfo);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception FileEx)
                        {
                            _logger.Error("Read FO inner fileentriesFO " + arr_FileEntry.Length + FileEx.ToString());
                        }
                    }
                    else
                    {
                        //added on 25JAN2021 by Amey
                        isTradeFileFOAvailable = false;
                    }
                }

                //objWriteLog.WriteLog("Time Taken To Read : " + (DateTime.Now - dt_Start).TotalSeconds + " Seconds " + DateTime.Now);
            }
            catch (Exception foEX)
            {
                _logger.Error("Read FO " + foEX.ToString());
            }
        }

        private void ReadCM(string FileName)
        {
            try
            {
                //ds_Gateway.dt_AllTradesRow dRow_Trade;
                PositionInfo _PositionInfo;

                var arr_Combinations = new string[TotalCombinations] { "", "", "", "" };

                //added on 19APR2021 by Amey
                var ScripNameKey = string.Empty;
                var CustomScripNameKey = string.Empty;
                var Symbol = string.Empty;
                var Series = string.Empty;

                //changed on 15JAN2021 by Amey
                foreach (var _NoticeCMFilePath in ds_Config.Tables["INTRADAY"].Rows[0]["NOTICE-CM"].ToString().Split(','))
                {
                    string[] arr_FileEntry = Directory.GetFiles(_NoticeCMFilePath, FileName + ".txt");
                    if (arr_FileEntry.Length > 0)
                    {
                        string FullFilePath = arr_FileEntry[0];

                        if (!dict_RandomEntriesCM.ContainsKey(FullFilePath))//added on 07-02-2020
                            dict_RandomEntriesCM.TryAdd(FullFilePath, new HashSet<string>());

                        using (FileStream stream = File.Open(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (StreamReader sr = new StreamReader(stream))
                            {
                                string strData = String.Empty;
                                while ((strData = sr.ReadLine()) != null)
                                {
                                    //added on 25JAN2021 by Amey
                                    isTradeFileCMAvailable = true;

                                    //added on 05JAN2021 by Amey
                                    string[] arr_Fields = strData.ToUpper().Split(',').Select(v => v.Trim()).ToArray();

                                    if (arr_Fields.Length > 24 && arr_Fields[0] != "")
                                    {
                                        string TradeID = arr_Fields[0];

                                        string FullDealerID = arr_Fields[24];
                                        string CTCL_ID = FullDealerID.Substring(0, (FullDealerID.Length > 11 ? 12 : FullDealerID.Length));
                                        string ClientCode = arr_Fields[14];
                                        string UserId = arr_Fields[8];
                                        string ClientInfokey = $"{ClientCode}|{CTCL_ID}|{UserId}";

                                        arr_Combinations = new string[TotalCombinations] { ClientInfokey, ClientCode, CTCL_ID, UserId };

                                        //changed on 10FEB2021 by Amey
                                        _PositionInfo = new PositionInfo();

                                        bool isValidTrade = false;

                                        for (int i = 0; i < arr_Combinations.Length; i++)
                                        {
                                            try
                                            {
                                                var UniqueID = arr_Combinations[i];
                                                if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

                                                string _check = TradeID + "_" + ClientCode;
                                                if (dict_RandomEntriesCM[FullFilePath].Contains(_check)) continue;

                                                //added on 19APR2021 by Amey
                                                _PositionInfo.Segment = en_Segment.NSECM;
                                                Series = arr_Fields[3];
                                                Symbol = arr_Fields[2];

                                                ScripNameKey = _PositionInfo.Segment + "|" + $"{Symbol}-{Series}";

                                                //added on 19APR2021 by Amey
                                                ContractMaster _ScripInfo = null;
                                                if (!dict_ScripInfo.TryGetValue(ScripNameKey, out _ScripInfo))
                                                {
                                                    CustomScripNameKey = _PositionInfo.Segment + "|" + $"{Symbol}|01JAN1980|0|{Series}";

                                                    if (!dict_CustomScripInfo.TryGetValue(CustomScripNameKey, out _ScripInfo))
                                                    {
                                                        if (isDebug)
                                                            _logger.Error("ReadCM Skipped : " + ScripNameKey + "^" + CustomScripNameKey, true);

                                                        continue;
                                                    }
                                                }

                                                _PositionInfo.TradePrice = Convert.ToDouble(arr_Fields[12]);

                                                //added on 07APR2021 by Amey
                                                var Qty = long.Parse(arr_Fields[11]);
                                                if (arr_Fields[10] == "2")
                                                {
                                                    _PositionInfo.TradeQuantity = Qty * -1;
                                                    _PositionInfo.SellQuantity = Qty;
                                                    _PositionInfo.SellValue = Qty * _PositionInfo.TradePrice;
                                                }
                                                else
                                                {
                                                    _PositionInfo.TradeQuantity = Qty;
                                                    _PositionInfo.BuyQuantity = Qty;
                                                    _PositionInfo.BuyValue = Qty * _PositionInfo.TradePrice;
                                                }

                                                //added on 10JUN2021 by Amey
                                                _PositionInfo.TradeValue = _PositionInfo.TradeQuantity * _PositionInfo.TradePrice;

                                                _PositionInfo.IntradayQuantity = _PositionInfo.TradeQuantity;

                                                //changed to TradeValue on 10JUN2021 by Amey
                                                //changed to - on 13APR2021 by Amey
                                                //changed computation logic on 07APR2021 by Amey
                                                _PositionInfo.IntradayValue = _PositionInfo.TradeValue; //_PositionInfo.BuyValue - _PositionInfo.SellValue;

                                                //added on 01DEC2020 by Amey
                                                double dbl_TradeTime = ConvertToUnixTimestamp(DateTime.Parse(arr_Fields[23]));
                                                _CMLastTradeTime = dbl_TradeTime > _CMLastTradeTime ? dbl_TradeTime : _CMLastTradeTime;

                                                //changed on 19APR2021 by Amey
                                                _PositionInfo.Token = _ScripInfo.Token;
                                                _PositionInfo.UnderlyingToken = _ScripInfo.UnderlyingToken;
                                                _PositionInfo.UnderlyingSegment = _ScripInfo.UnderlyingSegment;

                                                isValidTrade = true;

                                                //changed position on 28FEB2021 by Amey
                                                dict_RandomEntriesCM[FullFilePath].Add(_check);

                                                break;
                                            }
                                            catch (Exception ee) { _logger.Error("ReadCM Loop : " + strData + Environment.NewLine + ee); }
                                        }

                                        if (isValidTrade)
                                        {
                                            for (int i = 0; i < arr_Combinations.Length; i++)
                                            {
                                                var UniqueID = arr_Combinations[i];
                                                if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

                                                string Username = dict_ClientInfo[UniqueID];

                                                var newPosInfo = CopyPropertiesFrom(new PositionInfo(), _PositionInfo);
                                                newPosInfo.Username = Username;

                                                lock (list_AllTrades)
                                                    list_AllTrades.Add(newPosInfo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //added on 25JAN2021 by Amey
                        isTradeFileCMAvailable = false;
                    }
                }

            }
            catch (Exception foEX)
            {
                _logger.Error("Read CM " + foEX.ToString());
            }
        }

        private void ReadBSECM(string FileName)
        {
            try
            {
                //ds_Gateway.dt_AllTradesRow dRow_Trade;
                PositionInfo _PositionInfo;

                var arr_Combinations = new string[TotalCombinations] { "", "", "", "" };

                //added on 19APR2021 by Amey
                var TokenKey = string.Empty;
                var Symbol = string.Empty;
                var Series = string.Empty;

                //changed on 15JAN2021 by Amey
                foreach (var _BSECMFilePath in ds_Config.Tables["INTRADAY"].Rows[0]["BSE-CM"].ToString().Split(','))
                {
                    string[] arr_FileEntry = Directory.GetFiles(_BSECMFilePath, FileName + ".csv");
                    if (arr_FileEntry.Length > 0)
                    {
                        string FullFilePath = arr_FileEntry[0];

                        if (!dict_RandomEntriesBSECM.ContainsKey(FullFilePath))//added on 07-02-2020
                            dict_RandomEntriesBSECM.TryAdd(FullFilePath, new HashSet<string>());

                        using (FileStream stream = File.Open(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (StreamReader sr = new StreamReader(stream))
                            {
                                string strData = String.Empty;
                                while ((strData = sr.ReadLine()) != null)
                                {
                                    //added on 25JAN2021 by Amey
                                    isTradeFileBSECMAvailable = true;

                                    //added on 05JAN2021 by Amey
                                    string[] arr_Fields = strData.ToUpper().Split('|').Select(v => v.Trim()).ToArray();

                                    if (arr_Fields.Length > 22 && arr_Fields[11] != "")
                                    {
                                        string TradeID = arr_Fields[14];

                                        string FullDealerID = arr_Fields[21].ToUpper();

                                        //changed to 13 digits to on 22OCT2020 by Amey. BSE file has first 13 digits CTCL ID.
                                        string CTCL_ID = FullDealerID.Substring(0, (FullDealerID.Length > 12 ? 13 : FullDealerID.Length));

                                        string ClientCode = arr_Fields[10].ToUpper();
                                        string UserId = arr_Fields[1].ToUpper();
                                        string ClientInfokey = $"{ClientCode}|{CTCL_ID}|{UserId}";

                                        arr_Combinations = new string[TotalCombinations] { ClientInfokey, ClientCode, CTCL_ID, UserId };

                                        //changed on 10FEB2021 by Amey
                                        _PositionInfo = new PositionInfo();

                                        bool isValidTrade = false;

                                        for (int i = 0; i < arr_Combinations.Length; i++)
                                        {
                                            try
                                            {
                                                var UniqueID = arr_Combinations[i];
                                                if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

                                                string _check = TradeID + "_" + ClientCode;
                                                if (dict_RandomEntriesBSECM[FullFilePath].Contains(_check)) continue;

                                                //added on 20APR2021 by Amey
                                                TokenKey = en_Segment.BSECM + "|" + Convert.ToInt32(arr_Fields[2]);

                                                //added on 19APR2021 by Amey
                                                ContractMaster _ScripInfo = null;
                                                if (!dict_TokenScripInfo.TryGetValue(TokenKey, out _ScripInfo))
                                                {
                                                    if (isDebug)
                                                        _logger.Error("ReadBSECM Skipped : " + TokenKey, true);

                                                    continue;
                                                }

                                                //added on 20APR2021 by Amey
                                                if (dict_ScripInfo.ContainsKey(en_Segment.NSECM + "|" + _ScripInfo.ScripName))
                                                    _ScripInfo = dict_ScripInfo[en_Segment.NSECM + "|" + _ScripInfo.ScripName];
                                                
                                                _PositionInfo.Segment = _ScripInfo.Segment;

                                                _PositionInfo.TradePrice = Convert.ToDouble(arr_Fields[4]) / 100;

                                                //added on 07APR2021 by Amey
                                                var Qty = long.Parse(arr_Fields[5]);
                                                if (arr_Fields[13] == "S")
                                                {
                                                    _PositionInfo.TradeQuantity = Qty * -1;
                                                    _PositionInfo.SellQuantity = Qty;
                                                    _PositionInfo.SellValue = Qty * _PositionInfo.TradePrice;
                                                }
                                                else
                                                {
                                                    _PositionInfo.TradeQuantity = Qty;
                                                    _PositionInfo.BuyQuantity = Qty;
                                                    _PositionInfo.BuyValue = Qty * _PositionInfo.TradePrice;
                                                }

                                                //added on 10JUN2021 by Amey
                                                _PositionInfo.TradeValue = _PositionInfo.TradeQuantity * _PositionInfo.TradePrice;

                                                _PositionInfo.IntradayQuantity = _PositionInfo.TradeQuantity;

                                                //changed to TradeValue on 10JUN2021 by Amey
                                                //changed to - on 13APR2021 by Amey
                                                //changed computation logic on 07APR2021 by Amey
                                                _PositionInfo.IntradayValue = _PositionInfo.TradeValue; //_PositionInfo.BuyValue - _PositionInfo.SellValue;

                                                //added on 01DEC2020 by Amey
                                                double dbl_TradeTime = ConvertToUnixTimestamp(DateTime.Parse(arr_Fields[22]));
                                                _CMLastTradeTime = dbl_TradeTime > _CMLastTradeTime ? dbl_TradeTime : _CMLastTradeTime;

                                                //changed on 19APR2021 by Amey
                                                _PositionInfo.Token = _ScripInfo.Token;
                                                _PositionInfo.UnderlyingToken = _ScripInfo.UnderlyingToken;
                                                _PositionInfo.UnderlyingSegment = _ScripInfo.UnderlyingSegment;

                                                isValidTrade = true;

                                                //changed position on 28FEB2021 by Amey
                                                dict_RandomEntriesBSECM[FullFilePath].Add(_check);

                                                break;
                                            }
                                            catch (Exception ee) { _logger.Error("ReadBSECM Loop : " + strData + Environment.NewLine + ee); }
                                        }

                                        if (isValidTrade)
                                        {
                                            for (int i = 0; i < arr_Combinations.Length; i++)
                                            {
                                                var UniqueID = arr_Combinations[i];
                                                if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

                                                string Username = dict_ClientInfo[UniqueID];

                                                var newPosInfo = CopyPropertiesFrom(new PositionInfo(), _PositionInfo);
                                                newPosInfo.Username = Username;

                                                lock (list_AllTrades)
                                                    list_AllTrades.Add(newPosInfo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //added on 25JAN2021 by Amey
                        isTradeFileBSECMAvailable = false;
                    }
                }

            }
            catch (Exception foEX)
            {
                _logger.Error("Read BSECM " + foEX.ToString());
            }
        }

        //added on 15JAN2021 by Amey
        internal void ReloadAndRecalculateMargin(bool isButtonClick = false)
        {
            string[] arr_Margin = new string[0];

            if (isSpanConnected)
            {
                bool SpanComputeStartState = SpanComputeStart;

                try
                {
                    //added on 22JAN2021 by Amey. To stop calculating Intraday span when recomputing.
                    SpanComputeStart = false;
                    Thread.Sleep(1000);

                    //added on 17MAY2021 by Amey. To Display Span file info at Prime frontend.
                    var SpanDirectory = new DirectoryInfo(_SpanFolderPath);
                    LatestSpanFileName = SpanDirectory.GetFiles("nsccl*.spn").Any() ?
                                SpanDirectory.GetFiles("nsccl*.spn")
                               .OrderByDescending(f => f.LastWriteTime)
                               .First().Name : "";

                    if (LatestSpanFileName == "")
                        eve_AddToList($"Span file is not available. Please check BOD Utility for Span file status.");
                    else if (hs_SpanFileNames.Contains(LatestSpanFileName))
                    {
                        if (isButtonClick)
                            eve_AddToList($"New span file not available. Span file in use is [{LatestSpanFileName}]");

                        //added on 28MAY2021 by Amey
                        SpanComputeStart = SpanComputeStartState;

                        return;
                    }

                    eve_AddToList($"Span file {LatestSpanFileName} loading.");
                    hs_SpanFileNames.Add(LatestSpanFileName);

                    _SpanConnector.Reload("NSE", "EQFO");

                    //added on 16JUN2021 by Amey. To recompute span only for 1st span file.
                    if (hs_SpanFileNames.Count == 1)
                    {
                        #region EOD Margin
                        //commented on 05APR2021 by Amey.No need to calculate EOD margin using latest Span files.
                        //changed to ForEach on 10FEB2021 by Amey
                        //added on 27NOV2020 by Amey
                        foreach (var SpanKey in dict_EODMargin.Keys)
                        {
                            try
                            {
                                if (!hs_CalculatedSpanKeys.Contains(SpanKey)) continue;

                                arr_Margin = _SpanConnector.Recompute(1, SpanKey);
                                if (arr_Margin[2] == "")
                                {
                                    //changed code on 17DEC2020 by Amey
                                    dict_EODMargin[SpanKey][0] = Convert.ToDouble(arr_Margin[0]);
                                    dict_EODMargin[SpanKey][1] = Convert.ToDouble(arr_Margin[1]);
                                }
                            }
                            catch (Exception ee)
                            {
                                _logger.Error("ReloadAndRecalculateMargin Loop EOD : " + ee);

                                //added on 28MAY2021 by Amey
                                SpanComputeStart = SpanComputeStartState;

                                //added on 15JAN2021 by Amey
                                isSpanConnected = false;
                                RestartAndComputeSpan();

                                return;
                            }
                        }

                        //added on 27NOV2020 by Amey
                        bCast_Span.SendToChannel("n.SPAN", CompressString("EOD^" + JsonConvert.SerializeObject(dict_EODMargin)));
                        #endregion
                    }

                    //changed to ForEach on 10FEB2021 by Amey
                    foreach (var SpanKey in dict_SpanMargin.Keys)
                    {
                        try
                        {
                            if (!hs_CalculatedSpanKeys.Contains(SpanKey)) continue;

                            arr_Margin = _SpanConnector.Recompute(1, SpanKey);
                            if (arr_Margin[2] == "")
                            {
                                //changed code on 17DEC2020 by Amey
                                dict_SpanMargin[SpanKey][0] = Convert.ToDouble(arr_Margin[0]);
                                dict_SpanMargin[SpanKey][1] = Convert.ToDouble(arr_Margin[1]);
                            }
                        }
                        catch (Exception ee)
                        {
                            _logger.Error("ReloadAndRecalculateMargin Loop : " + ee);

                            //added on 28MAY2021 by Amey
                            SpanComputeStart = SpanComputeStartState;

                            //added on 15JAN2021 by Amey
                            isSpanConnected = false;
                            RestartAndComputeSpan();

                            return;
                        }
                    }

                    bCast_Span.SendToChannel("n.SPAN", CompressString("ALL^" + JsonConvert.SerializeObject(dict_SpanMargin)));

                    //changed to ForEach on 10FEB2021 by Amey
                    //Added by Akshay on 11-12-2020 for Expiry Span
                    foreach (var SpanKey in dict_ExpirySpanMargin.Keys)
                    {
                        try
                        {
                            if (!hs_CalculatedSpanKeys.Contains(SpanKey)) continue;

                            arr_Margin = _SpanConnector.Recompute(1, SpanKey);
                            if (arr_Margin[2] == "")
                            {
                                //changed code on 17DEC2020 by Amey
                                dict_ExpirySpanMargin[SpanKey][0] = Convert.ToDouble(arr_Margin[0]);
                                dict_ExpirySpanMargin[SpanKey][1] = Convert.ToDouble(arr_Margin[1]);
                            }
                        }
                        catch (Exception ee)
                        {
                            _logger.Error("ReloadAndRecalculateMargin Loop EXPIRY : " + ee);

                            //added on 28MAY2021 by Amey
                            SpanComputeStart = SpanComputeStartState;

                            //added on 15JAN2021 by Amey
                            isSpanConnected = false;
                            RestartAndComputeSpan();

                            return;
                        }
                    }

                    //added on 27NOV2020 by Amey
                    bCast_Span.SendToChannel("n.SPAN", CompressString("EXPIRY^" + JsonConvert.SerializeObject(dict_ExpirySpanMargin)));

                    //added on 22JAN2021 by Amey
                    SpanComputeStart = SpanComputeStartState;

                    //commented on 10JUN2021 by Amey. Added TradeTime as SpanComputeTime.
                    //added on 17MAY2021 by Amey
                    //_SpanComputeTime = ConvertToUnixTimestamp(DateTime.Now);

                    //added extra params on 17MAY2021 by Amey
                    //changed on 15JAN2021 by Amey
                    Task.Run(() => _HeartBeatServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                }
                catch (Exception ee) { _logger.Error("ReloadAndRecalculateMargin : " + ee); }

                //added on 4JUN2021 by Amey
                SpanComputeStart = SpanComputeStartState;
            }
        }

        #endregion       
    }
}
