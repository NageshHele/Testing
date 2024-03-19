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
using Gateway.Core_Logic;
using Gateway.Connectors;

namespace Gateway
{
    public delegate void del_AddToList(string Message);

    public delegate void del_TradeFileStatus(bool isFOAvailable, bool isCMAvailable, bool isBSECMAvailable);

    public class nProcess
    {
        public event del_AddToList eve_AddToList;
        public event del_TradeFileStatus eve_TradeFileStatus;

        string _nImageExeName = string.Empty;
        string _MySQLCon = string.Empty;

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

        EngineHBConnector _GatewayHBServer = new EngineHBConnector();

        EngineDataConnector _GatewayDataServer = new EngineDataConnector();

        /// <summary>
        /// Contents of Config.xml
        /// </summary>
        DataSet ds_Config;

        //added on 11JAN2021 by Amey
        //ds_Gateway.dt_AllTradesDataTable dt_AllPositions = new ds_Gateway.dt_AllTradesDataTable();

        //added on 13JAN2021 by Amey
        //ds_Gateway.dt_AllTradesDataTable dt_EODPositions = new ds_Gateway.dt_AllTradesDataTable();

        //added on 10FEB2021 by Amey
        List<PositionInfo> list_EODPositions = new List<PositionInfo>();

        /// <summary>
        /// Key => CTCL_ID,ClientCode,UserID,[ClientCode|CTCL_ID|UserID] | Value => Username
        /// </summary>
        ConcurrentDictionary<string, HashSet<string>> dict_ClientInfo = new ConcurrentDictionary<string, HashSet<string>>();

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


        HashSet<string> hs_BrokerID = new HashSet<string>();

        /// <summary>
        /// Expiry time for FO segment
        /// </summary>
        DateTime dt_ExpiryTime = DateTime.Parse("15:30:00");


        //Added by Snehadri on 23FEB2022
        /// <summary>
        /// Key: BSE Symbols | Value: Corresponding NSE Symbols
        /// </summary>
        Dictionary<string, string> dict_BSEMapping = new Dictionary<string, string>();

        int trade_batch_count = 10000;

        //changed params on 13JAN2021 by Amey
        //changed params on 15JAN2021 by Amey
        public nProcess(string _ReceivedMySQLCon, IPAddress GatewayHBServerIP, int GatewayServerHBPORT, clsWriteLog _Receivedlogger, DataSet ds_ReceivedConfig,
            string _SpanFolderPath)
        {
            try
            {
                _MySQLCon = _ReceivedMySQLCon;

                _logger = _Receivedlogger;
                ds_Config = ds_ReceivedConfig;
                this._SpanFolderPath = _SpanFolderPath;

                ReadConfig();

                ReadBSEMappingFile();

                ReadBrokerID();

                SetupGatewayServers();

                //changed location here on 16JUN2021 by Amey 
                ReadContractMaster();

                //added on 16JUN2021 by Amey
                CTradeProcess.Initialise(_logger, ds_Config, isDebug, dict_CustomScripInfo, dict_ScripInfo, dict_TokenScripInfo, dict_BSEMapping, hs_BrokerID, trade_batch_count);
                CTradeProcess.Instance.eve_SpanRead += Instance_eve_SpanRead;
                CTradeProcess.Instance.eve_TradeTime += Instance_eve_TradeTime;
                CTradeProcess.Instance.eve_TradeFileStatus += Instance_eve_TradeFileStatus;

            }
            catch (Exception Error)
            {
                _logger.Error("DumpData CTor : " + Error);
            }
        }

        private void Instance_eve_TradeFileStatus(bool isFOAvailable, bool isCMAvailable, bool isBSECMAvailable)
        {
            eve_TradeFileStatus(isFOAvailable, isCMAvailable, isBSECMAvailable);
        }

        #region TradeProcess Events

        private void Instance_eve_TradeTime(double _FOLastTradeTime, double _CMLastTradeTime)
        {
            this._FOLastTradeTime = _FOLastTradeTime;
            this._CMLastTradeTime = _CMLastTradeTime;
        }

        private void Instance_eve_SpanRead(Span _Span)
        {
            q_SpanRequests.Enqueue(_Span);
            q_TotalSpanRequests.Enqueue(_Span);
        }

        #endregion

        #region Supplimentary Methods

        //added on 15JAN2021 by Amey
        private void ReadConfig()
        {
            try
            {
                _nImageExeName = ds_Config.Tables["NIMAGE"].Rows[0]["EXE"].ToString();

                isDebug = Convert.ToBoolean(ds_Config.Tables["DEBUG-MODE"].Rows[0]["ENABLE"].ToString());
                isAdmin = Convert.ToBoolean(ds_Config.Tables["NIMAGE"].Rows[0]["ADMIN"].ToString());

                trade_batch_count = Convert.ToInt32(ds_Config.Tables["OTHER"].Rows[0]["TRADE-BATCH-COUNT"].ToString());

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


        private Dictionary<string, string> ReadBSEMappingFile()
        {
            dict_BSEMapping = new Dictionary<string, string>();

            try
            {
                var _BSEMappingFolder = new DirectoryInfo(ds_Config.Tables["SPAN"].Rows[0]["BSE-NSE-MAPPING"].ToString());
                var _MappingFileName = _BSEMappingFolder.GetFiles("EQD_CC*.csv").OrderByDescending(v => v.LastWriteTime).First();

                if (isDebug)
                    _logger.Error($"Reading file {_MappingFileName.FullName}");

                var arr_Lines = File.ReadAllLines(_MappingFileName.FullName);

                foreach (var Line in arr_Lines)
                {
                    try
                    {

                        var arr_Fields = Line.Split(',');
                        if (arr_Fields.Length < 5) { continue; }
                        var _BSESymbol = arr_Fields[3].Trim();
                        var _NSESymbol = arr_Fields[4].Trim();

                        if (_BSESymbol == "" || _NSESymbol == "") continue;

                        if (!dict_BSEMapping.ContainsKey(_BSESymbol))
                            dict_BSEMapping.Add(_BSESymbol, _NSESymbol);
                    }
                    catch (Exception ee)
                    {

                    }
                }

                if (isDebug)
                    _logger.Error($"File {_MappingFileName.FullName} read successfully.");
            }
            catch (Exception ee) { _logger.Error("ReadBSEMappingFile : " + ee); }

            return dict_BSEMapping;
        }

        private void ReadBrokerID()
        {
            try
            {
                string ClientNamesFile = "C:\\Prime\\Other\\BrokerID.csv";

                if (File.Exists(ClientNamesFile))
                {
                    var arr_ID = File.ReadAllLines(ClientNamesFile);

                    foreach (var ID in arr_ID)
                    {
                        try
                        {
                            hs_BrokerID.Add(ID.ToString().ToUpper());
                        }
                        catch (Exception ee) { }
                    }
                }
            }
            catch (Exception ee) { _logger.Error("ReadBrokerID : " + ee.ToString()); }
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

        private void SetupGatewayServers()
        {
            try
            {
                var CONInfo = ds_Config.Tables["CONNECTION"].Rows[0];
                //changed on 13JAN2021 by Amey
                var ip_GatewayServer = IPAddress.Parse(CONInfo["GATEWAY-SERVER-IP"].ToString());
                var _GatewayHBServerPORT = Convert.ToInt32(CONInfo["GATEWAY-SERVER-HB-PORT"].ToString());
                var _GatewayTradeServerPORT = Convert.ToInt32(CONInfo["GATEWAY-SERVER-TRADE-PORT"].ToString());
                var _GatewaySpanServerPORT = Convert.ToInt32(CONInfo["GATEWAY-SERVER-SPAN-PORT"].ToString());

                var _TimeoutMS = Convert.ToInt32(ds_Config.Tables["OTHER"].Rows[0]["TIMEOUT-SECONDS"]) * 1000;

                _GatewayHBServer.eve_ErrorReceived += _logger.Error;
                _GatewayHBServer.eve_SignalReceived += ReceiveResponseFromEngine;
                _GatewayHBServer.Setup(ip_GatewayServer, _GatewayHBServerPORT, _TimeoutMS);

                _GatewayDataServer.eve_ErrorReceived += _logger.Error;
                _GatewayDataServer.SetupTrade(ip_GatewayServer, _GatewayTradeServerPORT, _TimeoutMS);
                _GatewayDataServer.SetupSpan(ip_GatewayServer, _GatewaySpanServerPORT, _TimeoutMS);
            }
            catch (Exception ee) { _logger.Error("SetupGatewayServers : " + ee); }
        }



        //changed on 15JAN2021 by Amey
        private void ReceiveResponseFromEngine(string _Signal)
        {
            try
            {
                //changed on 15JAN2021 by Amey
                if (_Signal.Equals("START"))
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

                    SelectClientfromDatabase();
                    ReadEODTable();

                    //added on 16JUN2021 by Amey
                    CTradeProcess.Instance.UpdateCollections(dict_ClientInfo);
                    CTradeProcess.Instance.EngineRestarted();

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
                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                }
                else if (_Signal.Equals("STOP"))
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
                else if (_Signal.Equals("UPDATE"))
                {
                    SelectClientfromDatabase();
                    CTradeProcess.Instance.UpdateCollections(dict_ClientInfo);
                    _logger.Error("ReceiveResponseFromEngine : | UPDATE ", true);
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
                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));

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
                        Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));

                        RestartAndComputeSpan(); return;
                    }
                    else
                    {
                        if (isDebug)
                            _logger.Error("Span for EOD data computed successfully , SpanComputeStart " + SpanComputeStart + ", Intraday scrip count " + q_SpanRequests.Count, isDebug);

                        //Added on 22JAN2021 by Akshay
                        _GatewayDataServer.SendToEngineSpan("ALL^" + JsonConvert.SerializeObject(dict_SpanMargin));

                        if (isDebug)
                            _logger.Error("EOD Span Rows : " + dict_EODMargin.Count, isDebug);

                        Thread.Sleep(1000); //Added on 22JAN2021 by Akshay

                        _GatewayDataServer.SendToEngineSpan("EOD^" + JsonConvert.SerializeObject(dict_EODMargin));
                        //_logger.Error("EOD Span data " + JsonConvert.SerializeObject(dict_EODMargin), isDebug);

                        Thread.Sleep(1000); //Added on 22JAN2021 by Akshay

                        _GatewayDataServer.SendToEngineSpan("EXPIRY^" + JsonConvert.SerializeObject(dict_ExpirySpanMargin));
                        //_logger.Error("EOD Span data " + JsonConvert.SerializeObject(dict_ExpirySpanMargin), isDebug);

                        //added on 17MAY2021 by Amey
                        _SpanComputeTime = ConvertToUnixTimestamp(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 15, 00));

                        //added extra params on 17MAY2021 by Amey
                        //changed on 15JAN2021 by Amey
                        Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                    }


                    Task.Run(() => SendSpan());

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
                                    _GatewayDataServer.SendToEngineSpan("ALL^" + JsonConvert.SerializeObject(dict_SpanMargin));

                                    if (isDebug)
                                        _logger.Error("Span Rows : " + dict_SpanMargin.Count, isDebug);         
                                   
                                    Thread.Sleep(1000); //Added on 22JAN2021 by Akshay

                                    _GatewayDataServer.SendToEngineSpan("EXPIRY^" + JsonConvert.SerializeObject(dict_ExpirySpanMargin));

                                    //commented on 10JUN2021 by Amey. Added TradeTime as SpanComputeTime.
                                    //added on 17MAY2021 by Amey
                                    //_SpanComputeTime = ConvertToUnixTimestamp(DateTime.Now);

                                    //added extra params on 17MAY2021 by Amey
                                    //changed on 15JAN2021 by Amey
                                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                                }
                            }
                        }

                        if (!isSpanConnected)//05-02-2020
                        {
                            //added extra params on 17MAY2021 by Amey
                            //changed on 15JAN2021 by Amey
                            Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));

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
                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                }
            }
            catch (Exception error)
            {
                isSpanConnected = false;

                //added extra params on 17MAY2021 by Amey
                //changed on 15JAN2021 by Amey
                Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));

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

                                    // Added by Snehadri on 16JUL2022 for Client-Segment biforcation
                                    string Segment = _mySqlDataReader.GetString(11);

                                    if (ClienCode != "" && CTCL_ID != "" && UserID != "")
                                    {
                                        if (dict_ClientInfo.ContainsKey($"{ClienCode}|{CTCL_ID}|{UserID}|{Segment}"))
                                            dict_ClientInfo[$"{ClienCode}|{CTCL_ID}|{UserID}|{Segment}"].Add(Username);
                                        else
                                            dict_ClientInfo.TryAdd($"{ClienCode}|{CTCL_ID}|{UserID}|{Segment}", new HashSet<string> { Username });
                                    }
                                    else if (ClienCode != "")
                                    {
                                        if (dict_ClientInfo.ContainsKey(ClienCode + $"|{Segment}"))
                                            dict_ClientInfo[ClienCode + $"|{Segment}"].Add(Username);
                                        else
                                            dict_ClientInfo.TryAdd(ClienCode + $"|{Segment}", new HashSet<string> { Username });
                                    }
                                    else if (CTCL_ID != "")
                                    {
                                        if (dict_ClientInfo.ContainsKey(CTCL_ID + $"|{Segment}"))
                                            dict_ClientInfo[CTCL_ID + $"|{Segment}"].Add(Username);
                                        else
                                            dict_ClientInfo.TryAdd(CTCL_ID + $"|{Segment}", new HashSet<string> { Username });
                                    }
                                    else if (UserID != "")
                                    {
                                        if (dict_ClientInfo.ContainsKey(UserID + $"|{Segment}"))
                                            dict_ClientInfo[UserID + $"|{Segment}"].Add(Username);
                                        else
                                            dict_ClientInfo.TryAdd(UserID + $"|{Segment}", new HashSet<string> { Username });
                                    }
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
            //added for testing
            //SelectClientfromDatabase();
            //IsEngineStarted = true;

            bool ClearTrades = true;

            var stopwatch = new Stopwatch();

            while (true)
            {
                try
                {
                    stopwatch.Start();

                    DateTime dte_StartReadingTrades = DateTime.Now;

                    //removed flatFile param on 13JAN2021 by Amey
                    if (IsEngineStarted)
                    {
                        var list_AllTrades = CTradeProcess.Instance.GetTrades(ClearTrades);

                        //added on 25JAN2021 by Amey
                        //eve_TradeFileStatus(isTradeFileFOAvailable, isTradeFileCMAvailable, isTradeFileBSECMAvailable);

                        try
                        {
                            //changed to Any() on 28APR2021 by Amey
                            if (list_AllTrades.Any())
                            {
                                if (isDebug)
                                    _logger.Error("Trades Read in " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs", isDebug);

                                ClearTrades = _GatewayDataServer.SendToEngineTrades("ALL^" + JsonConvert.SerializeObject(list_AllTrades));

                                //added on 28MAY2021 by Amey
                                if (dict_SpanMargin.Any())
                                {
                                    //added on 28JAN2021 by Amey
                                    _GatewayDataServer.SendToEngineSpan("ALL^" + JsonConvert.SerializeObject(dict_SpanMargin));
                                }

                                if (isDebug)
                                    _logger.Error("Total Trade Rows Sent," + list_AllTrades.Count + ", Status," + ClearTrades, isDebug);

                                //added extra params on 17MAY2021 by Amey
                                //changed on 15JAN2021 by Amey
                                Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                            }
                        }
                        catch (Exception ee)
                        {
                            _logger.Error("Exception occurred while sending flatfiles over socket" + DateTime.Now + Environment.NewLine + ee.ToString());
                        }
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error("ReadIntraDay -OUT : " + ee);
                }

                #region Wait Time Calculation
                stopwatch.Stop();
                var elapsed_time = stopwatch.ElapsedMilliseconds;

                stopwatch.Reset();

                int waittime = 800;
                try
                {
                    waittime -= Convert.ToInt32(elapsed_time);
                    waittime = waittime < 0 ? 0 : waittime;
                }
                catch (OverflowException) { }
                catch (Exception) { }
                #endregion

                Thread.Sleep(waittime);
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

                        _GatewayDataServer.SendToEngineSpan("EOD^" + JsonConvert.SerializeObject(dict_EODMargin));
                        
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

                    _GatewayDataServer.SendToEngineSpan("ALL^" + JsonConvert.SerializeObject(dict_SpanMargin));

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
                    _GatewayDataServer.SendToEngineSpan("EXPIRY^" + JsonConvert.SerializeObject(dict_ExpirySpanMargin));

                    //added on 22JAN2021 by Amey
                    SpanComputeStart = SpanComputeStartState;

                    //commented on 10JUN2021 by Amey. Added TradeTime as SpanComputeTime.
                    //added on 17MAY2021 by Amey
                    //_SpanComputeTime = ConvertToUnixTimestamp(DateTime.Now);

                    //added extra params on 17MAY2021 by Amey
                    //changed on 15JAN2021 by Amey
                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                }
                catch (Exception ee) { _logger.Error("ReloadAndRecalculateMargin : " + ee); }

                //added on 4JUN2021 by Amey
                SpanComputeStart = SpanComputeStartState;
            }
        }

        private void SendSpan()
        {
            try
            {
                while (true)
                {
                    if (dict_SpanMargin.Any())
                    {
                        _GatewayDataServer.SendToEngineSpan("ALL^" + JsonConvert.SerializeObject(dict_SpanMargin));
                        Thread.Sleep(100);
                    }

                    if (dict_ExpirySpanMargin.Any())
                    {
                        _GatewayDataServer.SendToEngineSpan("EXPIRY^" + JsonConvert.SerializeObject(dict_ExpirySpanMargin));
                    }

                    Thread.Sleep(1000);
                }

            }
            catch(Exception ee) { _logger.Error(ee.ToString()); }
        }

        #endregion       
    }
}
