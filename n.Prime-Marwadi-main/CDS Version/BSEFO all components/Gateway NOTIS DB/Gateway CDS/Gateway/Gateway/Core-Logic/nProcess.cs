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
using System.Xml.Linq;

namespace Gateway
{
    public delegate void del_AddToList(string Message);

    public delegate void del_TradeFileStatus(bool isFOAvailable, bool isCMAvailable, bool isBSECMAvailable, bool isBSEFOAvailable, bool isCDAvailable);

    public class nProcess
    {
        public event del_AddToList eve_AddToList;
        public event del_TradeFileStatus eve_TradeFileStatus;

        string _nImageExeName = string.Empty;
        string _nImageCDSExeName = string.Empty;
        string _MySQLCon = string.Empty;

        string _SpanFolderPath = string.Empty;
        string _CDSSpanFolderPath = string.Empty;
        string LatestSpanFileName = string.Empty;
        string LatestCDSSpanFileName = string.Empty;

        private const int TotalCombinations = 4;

        double _FOLastTradeTime = 0;
        double _CMLastTradeTime = 0;
        double _BSECMLastTradeTime = 0;
        double _BSEFOLastTradeTime = 0;
        double _CDLastTradeTime = 0;
        double _SpanComputeTime = 0;
        double _CDSSpanComputeTime = 0;

        bool isDebug = false;
        bool isAdmin = false;
        bool isTradeFileFOAvailable = false;
        bool isTradeFileCMAvailable = false;
        bool isTradeFileCDAvailable = false;
        bool isTradeFileBSECMAvailable = false;
        bool isTradeFileBSEFOAvailable = false;


        /// <summary>
        /// Set true when connected to Span. False when disconnedted or crashed.
        /// </summary>
        bool isSpanConnected = false;


        /// <summary>
        /// Set true when connected to Span. False when disconnedted or crashed.
        /// </summary>
        bool isCDSSpanConnected = false;

        /// <summary>
        /// Set true when Engine sends START. False when Engine sends STOP
        /// </summary>
        bool IsEngineStarted = false;

        /// <summary>
        /// Set true when Engine sends START. False when Engine sends STOP
        /// </summary>
        bool SpanComputeStart = false;


        /// <summary>
        /// Set true when Engine sends START. False when Engine sends STOP
        /// </summary>
        bool CDSSpanComputeStart = false;


        /// <summary>
        /// Set true when CDS expiry time reach
        /// </summary>
        bool isCDSExpiryReach = false;

        ///
        bool isCdsEODSpanComputed = false;

        bool isEODSpanComputed = false;

        clsWriteLog _logger;

        SpanConnectorFO _SpanConnector;

        SpanConnectorCD _SpanConnectorCD;

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
        HashSet<string> hs_BrokerID = new HashSet<string>();
        /// <summary>
        /// Key => CTCL_ID,ClientCode,UserID,[ClientCode|CTCL_ID|UserID] | Value => Username
        /// </summary>
        ConcurrentDictionary<string, HashSet<string>> dict_ClientInfo = new ConcurrentDictionary<string, HashSet<string>>();

        ConcurrentDictionary<string, double[]> dict_SpanMargin = new ConcurrentDictionary<string, double[]>();
        ConcurrentDictionary<string, double[]> dict_EODMargin = new ConcurrentDictionary<string, double[]>();
        ConcurrentDictionary<string, double[]> dict_ExpirySpanMargin = new ConcurrentDictionary<string, double[]>();


        ConcurrentDictionary<string, double[]> dict_SpanMarginToday = new ConcurrentDictionary<string, double[]>();
        ConcurrentDictionary<string, double[]> dict_ExpirySpanMarginToday = new ConcurrentDictionary<string, double[]>();
        ConcurrentDictionary<string, double[]> dict_EODMarginToday = new ConcurrentDictionary<string, double[]>();

        //ConcurrentDictionary<string, double> dict_EQSpan = new ConcurrentDictionary<string, double>();//11-12-2019



        //CDS
        ConcurrentDictionary<string, double[]> dict_CDSSpanMargin = new ConcurrentDictionary<string, double[]>();
        ConcurrentDictionary<string, double[]> dict_CDSEODMargin = new ConcurrentDictionary<string, double[]>();
        ConcurrentDictionary<string, double[]> dict_CDSExpirySpanMargin = new ConcurrentDictionary<string, double[]>();
        ConcurrentDictionary<string, double[]> dict_CDSSpanMarginToday = new ConcurrentDictionary<string, double[]>();
        ConcurrentDictionary<string, double[]> dict_CDSExpirySpanMarginToday = new ConcurrentDictionary<string, double[]>();
        ConcurrentDictionary<string, double[]> dict_CDSEODMarginToday = new ConcurrentDictionary<string, double[]>();

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


        //CDS
        /// <summary>
        /// Stores current Span requests. Gets cleared after NImage or Engine Started.
        /// </summary>
        ConcurrentQueue<Span> q_CDSSpanRequests = new ConcurrentQueue<Span>();

        /// <summary>
        /// Stores all Span requests. Gets cleared after Engine started. Gets copied to q_SpanRequests after NImage Crashes.
        /// </summary>
        ConcurrentQueue<Span> q_TotalCDSSpanRequests = new ConcurrentQueue<Span>();

        /// <summary>
        /// Contains SpanKeys passed to getMargin()
        /// </summary>
        HashSet<string> hs_CalculatedCDSSpanKeys = new HashSet<string>();


        /// <summary>
        /// Contains span file names that are used
        /// </summary>
        HashSet<string> hs_CDSSpanFileNames = new HashSet<string>();

        /// <summary>
        /// Expiry time for FO segment
        /// </summary>
        DateTime dt_ExpiryTime = DateTime.Parse("15:30:00");


        //Added by Akshay on 03-01-2022 for CDS
        /// <summary>
        /// Expiry time for CD segment
        /// </summary>
        DateTime dt_CDExpiryTime = DateTime.Parse("12:00:00");

        int trade_batch_count = 10000;

        //Added by Snehadri on 23FEB2022
        /// <summary>
        /// Key: BSE Symbols | Value: Corresponding NSE Symbols
        /// </summary>
        Dictionary<string, string> dict_BSEMapping = new Dictionary<string, string>();

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

                SetupGatewayServers();
                ReadBSEMappingFile();
                ReadBrokerID();

                //changed location here on 16JUN2021 by Amey 
                ReadContractMaster();

                //added on 16JUN2021 by Amey
                CTradeProcess.Initialise(_logger, ds_Config, isDebug, dict_CustomScripInfo, dict_ScripInfo, dict_TokenScripInfo, dict_BSEMapping, hs_BrokerID, trade_batch_count);
                CTradeProcess.Instance.eve_SpanRead += Instance_eve_SpanRead;
                CTradeProcess.Instance.eve_TradeTime += Instance_eve_TradeTime;
                CTradeProcess.Instance.eve_TradeFileStatus += Instance_eve_TradeFileStatus;

                CTradeProcess.Instance.eve_CDSSpanRead += Instance_eve_CDSSpanRead;



            }
            catch (Exception Error)
            {
                _logger.Error("DumpData CTor : " + Error);
            }
        }

        private void Instance_eve_TradeFileStatus(bool isFOAvailable, bool isCMAvailable, bool isBSECMAvailable, bool isBSEFOAvailable, bool isCDAvailable)
        {
            eve_TradeFileStatus(isFOAvailable, isCMAvailable, isBSECMAvailable, isBSEFOAvailable, isCDAvailable);
        }

        //CDS
        private void Instance_eve_CDSSpanRead(Span _Span)
        {
            q_CDSSpanRequests.Enqueue(_Span);
            q_TotalCDSSpanRequests.Enqueue(_Span);
        }

        #region TradeProcess Events

        private void Instance_eve_TradeTime(double _FOLastTradeTime, double _CMLastTradeTime, double _CDLastTradeTime, double _BSECMLastTradeTime, double _BSEFOLastTradeTime)
        {
            this._FOLastTradeTime = _FOLastTradeTime;
            this._CMLastTradeTime = _CMLastTradeTime;
            this._CDLastTradeTime = _CDLastTradeTime;
            this._BSECMLastTradeTime = _BSECMLastTradeTime;
            this._BSEFOLastTradeTime = _BSEFOLastTradeTime;
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
                _nImageCDSExeName = ds_Config.Tables["NIMAGE"].Rows[0]["CDSEXE"].ToString();

                isDebug = Convert.ToBoolean(ds_Config.Tables["DEBUG-MODE"].Rows[0]["ENABLE"].ToString());
                isAdmin = Convert.ToBoolean(ds_Config.Tables["NIMAGE"].Rows[0]["ADMIN"].ToString());
                trade_batch_count = Convert.ToInt32(ds_Config.Tables["OTHER"].Rows[0]["TRADE-BATCH-COUNT"].ToString());
                foreach (var _Time in ds_Config.Tables["SPAN"].Rows[0]["RECOMPUTE-TIME"].ToString().Split(','))
                {
                    double waitSeconds = (DateTime.Parse(_Time) - DateTime.Now).TotalSeconds;
                    if (waitSeconds > 0)
                    {
                        Task.Delay(Convert.ToInt32(waitSeconds * 1000)).ContinueWith(t => ReloadAndRecalculateMargin());
                        Task.Delay(Convert.ToInt32(waitSeconds * 1000)).ContinueWith(t => ReloadAndRecalculateCDSMargin());
                    }
                       
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

        //TODO: Make seperate class for such methods.
        private void StartProcess(string FileName, bool isAdmin)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = FileName;
                proc.StartInfo.UseShellExecute = true;

                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(FileName);  //Added by Akshay on 20-10-2021 for CD n.Image

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
                    _CDLastTradeTime = 0;   //Added by Akshay on 01-12-2021 for CDS
                    _BSECMLastTradeTime =0;
                    _BSEFOLastTradeTime = 0;
                    _SpanComputeTime = 0;
                    isSpanConnected = false;

                    q_SpanRequests = new ConcurrentQueue<Span>();           //18-03-2020

                    //added on 11OCT2020 by Amey
                    q_TotalSpanRequests = new ConcurrentQueue<Span>();

                    //added on 17DEC2020 by Amey
                    hs_CalculatedSpanKeys.Clear();


                    //CDS
                    _CDLastTradeTime = 0;   //Added by Akshay on 01-12-2021 for CDS
                    _CDSSpanComputeTime = 0;
                    isCDSSpanConnected = false;
                    q_CDSSpanRequests = new ConcurrentQueue<Span>();
                    q_TotalCDSSpanRequests = new ConcurrentQueue<Span>();
                    hs_CalculatedCDSSpanKeys.Clear();

                    SelectClientfromDatabase();
                    ReadEODTable();

                    //added on 16JUN2021 by Amey
                    CTradeProcess.Instance.UpdateCollections(dict_ClientInfo);
                    CTradeProcess.Instance.EngineRestarted();

                    IsEngineStarted = true;

                    //added on 17MAY2021 by Amey. To Display Span file info at Prime frontend.
                    var SpanDirectory = new DirectoryInfo(_SpanFolderPath);
                    //LatestSpanFileName = SpanDirectory.GetFiles("nsccl.*.spn").Any() ?
                    //            SpanDirectory.GetFiles("nsccl.*.spn")
                    //           .OrderByDescending(f => f.LastWriteTime)
                    //           .First().Name : "";

                    LatestSpanFileName = "";
                    var arr_Files = SpanDirectory.GetFiles("nsccl.*.spn")
                               .OrderByDescending(f => f.LastWriteTime).ToArray();
                    try
                    {
                        foreach (var SpanFile in arr_Files)
                        {
                            try
                            {
                                XDocument doc = new XDocument();
                                doc = XDocument.Load(SpanFile.FullName);
                                LatestSpanFileName = SpanFile.Name;
                                break;
                            }
                            catch (Exception ee)
                            {
                                _logger.Error(ee.ToString());
                                File.Delete(SpanFile.FullName);
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        _logger.Error(ee.ToString());
                    }



                    if (LatestSpanFileName == "")
                        eve_AddToList($"Span file is not available. Please check BOD Utility for Span file status.");
                    else if (!hs_SpanFileNames.Contains(LatestSpanFileName))
                        hs_SpanFileNames.Add(LatestSpanFileName);


                    //changed location on 01FEB2021 by Amey
                    RestartAndComputeSpan();
                    SpanComputeStart = true;

                    //CDS
                    var CDSSpanDirectory = new DirectoryInfo(_SpanFolderPath);
                    //LatestCDSSpanFileName = CDSSpanDirectory.GetFiles("nsccl_ix*.spn").Any() ?
                    //            SpanDirectory.GetFiles("nsccl_ix*.spn")
                    //           .OrderByDescending(f => f.LastWriteTime)
                    //           .First().Name : "";

                    LatestCDSSpanFileName = "";
                    var arr_CDSpanFiles = CDSSpanDirectory.GetFiles("nsccl_ix*.spn")
                               .OrderByDescending(f => f.LastWriteTime)
                               .ToArray();

                    try
                    {
                        foreach (var SpanFile in arr_CDSpanFiles)
                        {
                            try
                            {
                                XDocument doc = new XDocument();
                                doc = XDocument.Load(SpanFile.FullName);
                                LatestCDSSpanFileName = SpanFile.Name;
                                break;
                            }
                            catch (Exception ee)
                            {
                                _logger.Error(ee.ToString());
                                File.Delete(SpanFile.FullName);
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        _logger.Error(ee.ToString());
                    }

                    if (LatestCDSSpanFileName == "")
                        eve_AddToList($"CDS Span file is not available. Please check BOD Utility for CDS Span file status.");
                    else if (!hs_CDSSpanFileNames.Contains(LatestCDSSpanFileName))
                        hs_CDSSpanFileNames.Add(LatestCDSSpanFileName);


                    RestartAndComputeCDSSpan();
                    CDSSpanComputeStart = true;


                    //added extra params on 17MAY2021 by Amey
                    //changed on 15JAN2021 by Amey
                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                    Task.Run(() => SendSpan());

                    //CDS Expiry
                    //Task.Run(() => SetUpTimer(new TimeSpan(15, 30, 00)));
                    //Task.Run(() => SetUpCDSTimer(new TimeSpan(12, 00, 00)));

                }
                else if (_Signal.Equals("STOP"))
                {
                    //added on 22JAN2021 by Amey
                    eve_AddToList("Engine Stopped.");

                    IsEngineStarted = false;
                    SpanComputeStart = false;
                    CDSSpanComputeStart = false;

                    //added on 11JUN2021 by Amey
                    isSpanConnected = false;
                    isCDSSpanConnected = false;
                    _SpanConnector = null;
                    //_SpanConnectorCD = null;    //Added by Akshay on 20-10-2021 for CD n.Image
                    EndProcess(_nImageExeName);
                    EndProcess(_nImageCDSExeName);
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
                        (v.Segment == "NSEFO" ? en_Segment.NSEFO : (v.Segment == "BSEFO" ? en_Segment.BSEFO : en_Segment.BSECM))),
                    Token = v.Token,                   
                    TradePrice = v.TradePrice,
                    TradeQuantity = v.TradeQuantity,
                    UnderlyingSegment = v.UnderlyingSegment == "NSECM" ? en_Segment.NSECM : (v.Segment == "NSECD" ? en_Segment.NSECD :
                        (v.Segment == "NSEFO" ? en_Segment.NSEFO : (v.Segment == "BSEFO" ? en_Segment.BSEFO : en_Segment.BSECM))),
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

                //Task.Run(() => SendSpan());
            }
            catch (Exception error)
            {
                _logger.Error("InitialiseSpan : " + error);
            }
        }


        private void RestartAndComputeCDSSpan()
        {
            try
            {
                //added on 02FEB2021 by Amey
                if (!IsEngineStarted) return;

                if (isDebug)
                    _logger.Error("Initialising CDS Image.", isDebug);

                isCDSSpanConnected = false;
                //_SpanConnectorCD = null;    //Added by Akshay on 20-10-2021 for CD n.Image

                dict_CDSSpanMargin.Clear();

                EndProcess(_nImageCDSExeName);

                //Added by Akshay on 20-10-2021 for CD n.Image
                StartProcess(Application.StartupPath + "\\CD\\" + _nImageCDSExeName, isAdmin);

                //added on 22OCT2020 by Amey.
                q_CDSSpanRequests = new ConcurrentQueue<Span>(q_TotalCDSSpanRequests);

                //added on 01FEB2021 by Amey
                Thread.Sleep(300);

                //added on 15JAN2021 by Amey
                Task.Run(() => ComputeCDSSpan());

                //Task.Run(() => CDSSendSpan());

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
                    isEODSpanComputed = false;

                    //added extra params on 17MAY2021 by Amey
                    //changed on 15JAN2021 by Amey
                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                    // Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

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
                                //if (DateTime.Now.Date == dtExpiry.Date && DateTime.Now > dt_ExpiryTime)
                                //    continue;
                                // Changed logic by Snehadri on 22SEP2022
                                //if (dtExpiry < DateTime.Now) continue; 
                                if (dtExpiry.Subtract(DateTime.Today).TotalDays < 0)
                                {
                                    continue;
                                }

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
                        //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                        //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                        Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

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
                        //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                        //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                        Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                    }

                    #region added by Navin on 28-01-2020
                    isEODSpanComputed = true;

                    //Task.Run(() => SendSpan());

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
                                                //no need to calculate margin for expired contracts. Added on 26APR2021 by Amey.
                                                //Changed by Snehadri on 22SEP2022
                                                //if (dtExpiry < DateTime.Now) continue;

                                                //Added on 22JAN2021 by Akshay
                                                DateTime dtExpiry = DateTime.ParseExact(_Span.pExpiry, "yyyyMMdd", CultureInfo.InvariantCulture);

                                                //no need to calculate margin for expired contracts. Added on 26APR2021 by Amey.
                                                //if (DateTime.Now.Date == dtExpiry.Date && DateTime.Now > dt_ExpiryTime)
                                                //    continue;

                                                if (dtExpiry.Subtract(DateTime.Today).TotalDays < 0)
                                                {
                                                    continue;
                                                }

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
                                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                                    // Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                                }
                            }
                        }

                        if (!isSpanConnected)//05-02-2020
                        {
                            //added extra params on 17MAY2021 by Amey
                            //changed on 15JAN2021 by Amey
                            //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                            // Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                            Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

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
                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                    // Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                }
            }
            catch (Exception error)
            {
                isSpanConnected = false;

                //added extra params on 17MAY2021 by Amey
                //changed on 15JAN2021 by Amey
                //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                _logger.Error("Span status false, ComputeSpan " + error);

                //added on 15JAN2021 by Amey
                RestartAndComputeSpan();
            }
        }

        private void ComputeCDSSpan()
        {
            try
            {
                //added on 15JAN2021 by Amey
                string[] arr_Margin = new string[0];
                string[] arr_EODMargin = new string[0];     //added on 22JAN2021 by Akshay
                string[] arr_ExpiryMargin = new string[0];  //added on 22JAN2021 by Akshay

                //Added on 29SEP2020 by Amey
                IPAddress cdImageIP = IPAddress.Parse(ds_Config.Tables["NIMAGE"].Rows[0]["CDADDRESS"].ToString().Split(':')[0]);
                int cdImagePORT = Convert.ToInt32(ds_Config.Tables["NIMAGE"].Rows[0]["CDADDRESS"].ToString().Split(':')[1]); //added on 15-11-2021 by Akshay

                _SpanConnectorCD = SpanConnectorCD.Instance(cdImageIP, cdImagePORT);//added on 15-11-2021 by Akshay

                SelectNearestCDSExpiryDate();  //added on 22JAN2021 by Akshay 

                //SelectClosestCDSExpiryDate();  //added on 24JAN2022 by Akshay 

                var dt_DateTimeNow = DateTime.Now.Date;

                if (dt_DateTimeNow == dt_NearestCDSExpiry.Date)
                    Task.Run(() => SetUpCDSTimer(new TimeSpan(12, 00, 00)));

                if (_SpanConnectorCD != null)
                {
                    if (isDebug)
                        _logger.Error("CD Image Initialised successfully", isDebug);

                    isCDSSpanConnected = true;
                    isCdsEODSpanComputed = false;

                    //added extra params on 17MAY2021 by Amey
                    //changed on 15JAN2021 by Amey
                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                    if (isDebug)
                        _logger.Error("Computing CDSspan for eod data TotalRows : " + list_EODPositions.Count, isDebug);

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

                                if (dt_DateTimeNow == dtExpiry.Date && DateTime.Now > dt_CDExpiryTime && _PositionInfo.Segment == en_Segment.NSECD)
                                    continue;

                                string SpanKey = $"{_PositionInfo.Username}_{ScripInfo.Symbol}_CDS";          //Added on 22JAN2021 by Akshay
                                string EODKey = $"{_PositionInfo.Username}_{ScripInfo.Symbol}_CDSEOD";       //Added on 22JAN2021 by Akshay
                                string ExpiryKey = $"{_PositionInfo.Username}_{ScripInfo.Symbol}_CDSEXP";    //Added on 22JAN2021 by Akshay

                                if (ScripInfo.ScripType != en_ScripType.EQ && _PositionInfo.Segment == en_Segment.NSECD)
                                {

                                     _PositionInfo.TradeQuantity = _PositionInfo.TradeQuantity / ScripInfo.LotSize;

                                    //changed on 15JAN2021 by Amey
                                    if (isCDSSpanConnected)
                                    {
                                        try
                                        {
                                            if (ScripInfo.ScripType == en_ScripType.XX)
                                            {
                                                arr_Margin = _SpanConnectorCD.GetMargin(1, SpanKey, "NSE", "CURR", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), "", "", "E", _PositionInfo.TradeQuantity.ToString());
                                                arr_EODMargin = _SpanConnectorCD.GetMargin(1, EODKey, "NSE", "CURR", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), "", "", "E", _PositionInfo.TradeQuantity.ToString());

                                                //Changed on 22JAN2021 by Akshay
                                                if (dt_NearestCDSExpiry < dtExpiry)  //Changed on 22JAN2021 by Akshay
                                                {
                                                    arr_ExpiryMargin = _SpanConnectorCD.GetMargin(1, ExpiryKey, "NSE", "CURR", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), "", "", "E", _PositionInfo.TradeQuantity.ToString());

                                                    if (!dict_CDSExpirySpanMargin.ContainsKey(ExpiryKey))
                                                        dict_CDSExpirySpanMargin.TryAdd(ExpiryKey, new double[3] { Convert.ToDouble(arr_ExpiryMargin[0]), Convert.ToDouble(arr_ExpiryMargin[1]), 0 });
                                                    else
                                                    {
                                                        dict_CDSExpirySpanMargin[ExpiryKey][0] = Convert.ToDouble(arr_ExpiryMargin[0]);
                                                        dict_CDSExpirySpanMargin[ExpiryKey][1] = Convert.ToDouble(arr_ExpiryMargin[1]);
                                                    }
                                                    hs_CalculatedCDSSpanKeys.Add(ExpiryKey);  //Added by Akshay on 21-12-2020 
                                                }
                                            }
                                            else
                                            {

                                                arr_Margin = _SpanConnectorCD.GetMargin(1, SpanKey, "NSE", "CURR", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), ScripInfo.StrikePrice.ToString("#.0000").Replace(",", ""), ScripInfo.ScripType.ToString().Substring(0, 1), "E", _PositionInfo.TradeQuantity.ToString());
                                                arr_EODMargin = _SpanConnectorCD.GetMargin(1, EODKey, "NSE", "CURR", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), ScripInfo.StrikePrice.ToString("#.0000").Replace(",", ""), ScripInfo.ScripType.ToString().Substring(0, 1), "E", _PositionInfo.TradeQuantity.ToString());

                                                //Changed on 22JAN2021 For Nearest expirydate
                                                if (dtNearestExpiry < dtExpiry)  //Changed on 22JAN2021 For Nearest expirydate
                                                {
                                                    if (ScripInfo.Segment == en_Segment.NSECD)
                                                        arr_ExpiryMargin = _SpanConnectorCD.GetMargin(1, ExpiryKey, "NSE", "CURR", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), "", "", "E", _PositionInfo.TradeQuantity.ToString());
                                                    else
                                                        arr_ExpiryMargin = _SpanConnectorCD.GetMargin(1, ExpiryKey, "NSE", "CURR", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), "", "", "E", _PositionInfo.TradeQuantity.ToString());

                                                    if (!dict_CDSExpirySpanMargin.ContainsKey(ExpiryKey))
                                                        dict_CDSExpirySpanMargin.TryAdd(ExpiryKey, new double[3] { Convert.ToDouble(arr_ExpiryMargin[0]), Convert.ToDouble(arr_ExpiryMargin[1]), 0 });
                                                    else
                                                    {
                                                        dict_CDSExpirySpanMargin[ExpiryKey][0] = Convert.ToDouble(arr_ExpiryMargin[0]);
                                                        dict_CDSExpirySpanMargin[ExpiryKey][1] = Convert.ToDouble(arr_ExpiryMargin[1]);
                                                    }
                                                    hs_CalculatedCDSSpanKeys.Add(ExpiryKey);  //Added by Akshay on 21-12-2020 
                                                }
                                            }

                                            if (!dict_CDSSpanMargin.ContainsKey(SpanKey))
                                                dict_CDSSpanMargin.TryAdd(SpanKey, new double[3] { Convert.ToDouble(arr_Margin[0]), Convert.ToDouble(arr_Margin[1]), 0 });
                                            else
                                            {
                                                dict_CDSSpanMargin[SpanKey][0] = Convert.ToDouble(arr_Margin[0]);
                                                dict_CDSSpanMargin[SpanKey][1] = Convert.ToDouble(arr_Margin[1]);
                                            }

                                            //Changed on 22JAN2021 by Akshay
                                            if (!dict_CDSEODMargin.ContainsKey(EODKey))
                                                dict_CDSEODMargin.TryAdd(EODKey, new double[3] { Convert.ToDouble(arr_EODMargin[0]), Convert.ToDouble(arr_EODMargin[1]), 0 });
                                            else
                                            {
                                                dict_CDSEODMargin[EODKey][0] = Convert.ToDouble(arr_EODMargin[0]);
                                                dict_CDSEODMargin[EODKey][1] = Convert.ToDouble(arr_EODMargin[1]);
                                            }

                                            //added on 17DEC2020 by Amey
                                            hs_CalculatedCDSSpanKeys.Add(SpanKey);
                                            hs_CalculatedCDSSpanKeys.Add(EODKey);

                                            //CDS Expiry
                                            if (dtExpiry.Date != dt_DateTimeNow && dt_DateTimeNow == dt_NearestCDSExpiry.Date)
                                            {
                                                SpanKey = $"{_PositionInfo.Username}_{ScripInfo.Symbol}_CDS_Today";          //Added on 22JAN2021 by Akshay
                                                EODKey = $"{_PositionInfo.Username}_{ScripInfo.Symbol}_CDSEOD_Today";       //Added on 22JAN2021 by Akshay
                                                ExpiryKey = $"{_PositionInfo.Username}_{ScripInfo.Symbol}_CDSEXP_Today";    //Added on 22JAN2021 by Akshay

                                                if (ScripInfo.ScripType == en_ScripType.XX)
                                                {
                                                    arr_Margin = _SpanConnectorCD.GetMargin(1, SpanKey, "NSE", "CURR", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), "", "", "E", _PositionInfo.TradeQuantity.ToString());
                                                    arr_EODMargin = _SpanConnectorCD.GetMargin(1, EODKey, "NSE", "CURR", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), "", "", "E", _PositionInfo.TradeQuantity.ToString());

                                                    //Changed on 22JAN2021 by Akshay
                                                    if (dtNearestExpiry < dtExpiry)  //Changed on 22JAN2021 by Akshay
                                                    {
                                                        arr_ExpiryMargin = _SpanConnectorCD.GetMargin(1, ExpiryKey, "NSE", "CURR", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), "", "", "E", _PositionInfo.TradeQuantity.ToString());

                                                        if (!dict_CDSExpirySpanMarginToday.ContainsKey(ExpiryKey))
                                                            dict_CDSExpirySpanMarginToday.TryAdd(ExpiryKey, new double[3] { Convert.ToDouble(arr_ExpiryMargin[0]), Convert.ToDouble(arr_ExpiryMargin[1]), 0 });
                                                        else
                                                        {
                                                            dict_CDSExpirySpanMarginToday[ExpiryKey][0] = Convert.ToDouble(arr_ExpiryMargin[0]);
                                                            dict_CDSExpirySpanMarginToday[ExpiryKey][1] = Convert.ToDouble(arr_ExpiryMargin[1]);
                                                        }
                                                        hs_CalculatedCDSSpanKeys.Add(ExpiryKey);  //Added by Akshay on 21-12-2020 
                                                    }
                                                }
                                                else
                                                {
                                                    arr_Margin = _SpanConnectorCD.GetMargin(1, SpanKey, "NSE", "CURR", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), ScripInfo.StrikePrice.ToString("#.0000").Replace(",", ""), ScripInfo.ScripType.ToString().Substring(0, 1), "E", _PositionInfo.TradeQuantity.ToString());
                                                    arr_EODMargin = _SpanConnectorCD.GetMargin(1, EODKey, "NSE", "CURR", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), ScripInfo.StrikePrice.ToString("#.0000").Replace(",", ""), ScripInfo.ScripType.ToString().Substring(0, 1), "E", _PositionInfo.TradeQuantity.ToString());

                                                    //Changed on 22JAN2021 For Nearest expirydate
                                                    if (dtNearestExpiry < dtExpiry)  //Changed on 22JAN2021 For Nearest expirydate
                                                    {
                                                        arr_ExpiryMargin = _SpanConnectorCD.GetMargin(1, ExpiryKey, "NSE", "CURR", ScripInfo.Symbol, dtExpiry.ToString("yyyy-MM-dd").Replace("-", ""), "", "", "E", _PositionInfo.TradeQuantity.ToString());

                                                        if (!dict_CDSExpirySpanMarginToday.ContainsKey(ExpiryKey))
                                                            dict_CDSExpirySpanMarginToday.TryAdd(ExpiryKey, new double[3] { Convert.ToDouble(arr_ExpiryMargin[0]), Convert.ToDouble(arr_ExpiryMargin[1]), 0 });
                                                        else
                                                        {
                                                            dict_CDSExpirySpanMarginToday[ExpiryKey][0] = Convert.ToDouble(arr_ExpiryMargin[0]);
                                                            dict_CDSExpirySpanMarginToday[ExpiryKey][1] = Convert.ToDouble(arr_ExpiryMargin[1]);
                                                        }
                                                        hs_CalculatedCDSSpanKeys.Add(ExpiryKey);  //Added by Akshay on 21-12-2020 
                                                    }
                                                }

                                                if (!dict_CDSSpanMarginToday.ContainsKey(SpanKey))
                                                    dict_CDSSpanMarginToday.TryAdd(SpanKey, new double[3] { Convert.ToDouble(arr_Margin[0]), Convert.ToDouble(arr_Margin[1]), 0 });
                                                else
                                                {
                                                    dict_CDSSpanMarginToday[SpanKey][0] = Convert.ToDouble(arr_Margin[0]);
                                                    dict_CDSSpanMarginToday[SpanKey][1] = Convert.ToDouble(arr_Margin[1]);
                                                }

                                                //Changed on 22JAN2021 by Akshay
                                                if (!dict_CDSEODMarginToday.ContainsKey(EODKey))
                                                    dict_CDSEODMarginToday.TryAdd(EODKey, new double[3] { Convert.ToDouble(arr_EODMargin[0]), Convert.ToDouble(arr_EODMargin[1]), 0 });
                                                else
                                                {
                                                    dict_CDSEODMarginToday[EODKey][0] = Convert.ToDouble(arr_EODMargin[0]);
                                                    dict_CDSEODMarginToday[EODKey][1] = Convert.ToDouble(arr_EODMargin[1]);
                                                }

                                                //added on 17DEC2020 by Amey
                                                hs_CalculatedCDSSpanKeys.Add(SpanKey);
                                                hs_CalculatedCDSSpanKeys.Add(EODKey);
                                            }

                                        }
                                        catch (Exception error)
                                        {
                                            isCDSSpanConnected = false;

                                            _logger.Error("CDS Span status false, margin[0] " + arr_Margin[0] + ", margin[1] " + arr_Margin[1] + ", margin[2] " + arr_Margin[2] + " for request " + EODKey + " , " + "NSE" + " , " + "EQFO" + " , " + dtExpiry.ToString("yyyy-MM-dd").Replace("-", "") + " , " + ScripInfo.StrikePrice.ToString("N2").Replace(",", "") + " , " + ScripInfo.ScripType.ToString().Substring(0, 1) + " , " + "E" + " , " + _PositionInfo.TradeQuantity + " , " + error);

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception inerEx)
                        {
                            isCDSSpanConnected = false;

                            _logger.Error("CDS Span status false, Span if eod data at " + EODIdx + "_" + inerEx);

                            RestartAndComputeCDSSpan(); return;
                        }
                    }

                    if (!isCDSSpanConnected)
                    {
                        //added extra params on 17MAY2021 by Amey
                        //changed on 15JAN2021 by Amey
                        //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                        //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                        Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                        RestartAndComputeCDSSpan(); return;
                    }
                    else
                    {

                        if (!isCDSExpiryReach)
                        {
                            if (isDebug)
                                _logger.Error("CDS Span for EOD data computed successfully , SpanComputeStart " + CDSSpanComputeStart + ", Intraday scrip count " + q_CDSSpanRequests.Count, isDebug);

                            _GatewayDataServer.SendToEngineSpan("CDSALL^" + JsonConvert.SerializeObject(dict_CDSSpanMargin));

                            if (isDebug)
                                _logger.Error("CDS EOD Span Rows : " + dict_CDSEODMargin.Count, isDebug);

                            Thread.Sleep(1000); //Added on 22JAN2021 by Akshay

                            _GatewayDataServer.SendToEngineSpan("CDSEOD^" + JsonConvert.SerializeObject(dict_CDSEODMargin));

                            Thread.Sleep(1000); //Added on 22JAN2021 by Akshay

                            _GatewayDataServer.SendToEngineSpan("CDSEXPIRY^" + JsonConvert.SerializeObject(dict_CDSExpirySpanMargin));

                        }
                        else
                        {
                            if (isDebug)
                                _logger.Error("CDS Span for EOD data computed successfully , SpanComputeStart " + CDSSpanComputeStart + ", Intraday scrip count " + q_CDSSpanRequests.Count, isDebug);

                            _GatewayDataServer.SendToEngineSpan("CDSALL^" + JsonConvert.SerializeObject(dict_CDSSpanMarginToday));

                            if (isDebug)
                                _logger.Error("CDS EOD Span Rows Today : " + dict_CDSEODMarginToday.Count, isDebug);

                            Thread.Sleep(1000); //Added on 22JAN2021 by Akshay

                            _GatewayDataServer.SendToEngineSpan("CDSEOD^" + JsonConvert.SerializeObject(dict_CDSEODMarginToday));

                            Thread.Sleep(1000); //Added on 22JAN2021 by Akshay

                            _GatewayDataServer.SendToEngineSpan("CDSEXPIRY^" + JsonConvert.SerializeObject(dict_CDSExpirySpanMarginToday));

                        }


                        //added on 17MAY2021 by Amey
                        _CDSSpanComputeTime = ConvertToUnixTimestamp(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 15, 00));

                        //added extra params on 17MAY2021 by Amey
                        //changed on 15JAN2021 by Amey
                        //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                        // Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                        Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                    }

                    isCdsEODSpanComputed = true;

                    #region added by Navin on 28-01-2020

                    while (isCDSSpanConnected)
                    {
                        //changed on 15JAN2021 by Amey
                        if (CDSSpanComputeStart)
                        {
                            if (_SpanConnectorCD != null)
                            {
                                Span _Span = new Span();
                                bool _IsCDSSpanComputed = false;

                                if (q_CDSSpanRequests.Any())
                                {
                                    while (q_CDSSpanRequests.TryDequeue(out _Span))
                                    {
                                        try
                                        {
                                            _IsCDSSpanComputed = true;
                                            try
                                            {
                                                //Added on 22JAN2021 by Akshay
                                                DateTime dtExpiry = DateTime.ParseExact(_Span.pExpiry, "yyyyMMdd", CultureInfo.InvariantCulture);

                                                if (dt_DateTimeNow == dtExpiry.Date && DateTime.Now > dt_CDExpiryTime && _Span.pSegment == "CURR")
                                                    continue;

                                                if (dt_NearestCDSExpiry < dtExpiry)
                                                {
                                                    string ExpiryKey = _Span.pClientId + "_CDSEXP";

                                                    arr_Margin = _SpanConnectorCD.GetMargin(_Span.pMemberId, ExpiryKey, _Span.pExchange, _Span.pSegment, _Span.pScripName, _Span.pExpiry, _Span.pStrikePrice, _Span.pCallPut, _Span.pFactor, _Span.pQty);

                                                    if (!dict_CDSExpirySpanMargin.ContainsKey(ExpiryKey))
                                                        dict_CDSExpirySpanMargin.TryAdd(ExpiryKey, new double[3] { Convert.ToDouble(arr_Margin[0]), Convert.ToDouble(arr_Margin[1]), 0 });
                                                    else
                                                    {
                                                        dict_CDSExpirySpanMargin[ExpiryKey][0] = Convert.ToDouble(arr_Margin[0]);
                                                        dict_CDSExpirySpanMargin[ExpiryKey][1] = Convert.ToDouble(arr_Margin[1]);
                                                    }
                                                    hs_CalculatedCDSSpanKeys.Add(ExpiryKey);
                                                }

                                                arr_Margin = _SpanConnectorCD.GetMargin(_Span.pMemberId, _Span.pClientId + "_CDS", _Span.pExchange, _Span.pSegment, _Span.pScripName, _Span.pExpiry, _Span.pStrikePrice, _Span.pCallPut, _Span.pFactor, _Span.pQty);

                                                if (!dict_CDSSpanMargin.ContainsKey(_Span.pClientId + "_CDS"))
                                                    dict_CDSSpanMargin.TryAdd(_Span.pClientId + "_CDS", new double[3] { Convert.ToDouble(arr_Margin[0]), Convert.ToDouble(arr_Margin[1]), 0 });
                                                else
                                                {
                                                    dict_CDSSpanMargin[_Span.pClientId + "_CDS"][0] = Convert.ToDouble(arr_Margin[0]);
                                                    dict_CDSSpanMargin[_Span.pClientId + "_CDS"][1] = Convert.ToDouble(arr_Margin[1]);
                                                }

                                                //added on 17DEC2020 by Amey
                                                hs_CalculatedCDSSpanKeys.Add(_Span.pClientId + "_CDS");

                                                //added on 11JUN2021 by Amey
                                                _CDSSpanComputeTime = _Span.pTradeTime;

                                                //CDS Expiry
                                                if (dtExpiry.Date != dt_DateTimeNow && dt_DateTimeNow == dt_NearestCDSExpiry.Date)
                                                {
                                                    if (dt_NearestCDSExpiry < dtExpiry)
                                                    {
                                                        string ExpiryKey = _Span.pClientId + "_CDSEXP_Today";

                                                        arr_Margin = _SpanConnectorCD.GetMargin(_Span.pMemberId, ExpiryKey, _Span.pExchange, _Span.pSegment, _Span.pScripName, _Span.pExpiry, _Span.pStrikePrice, _Span.pCallPut, _Span.pFactor, _Span.pQty);

                                                        if (!dict_CDSExpirySpanMarginToday.ContainsKey(ExpiryKey))
                                                            dict_CDSExpirySpanMarginToday.TryAdd(ExpiryKey, new double[3] { Convert.ToDouble(arr_Margin[0]), Convert.ToDouble(arr_Margin[1]), 0 });
                                                        else
                                                        {
                                                            dict_CDSExpirySpanMarginToday[ExpiryKey][0] = Convert.ToDouble(arr_Margin[0]);
                                                            dict_CDSExpirySpanMarginToday[ExpiryKey][1] = Convert.ToDouble(arr_Margin[1]);
                                                        }
                                                        hs_CalculatedCDSSpanKeys.Add(ExpiryKey);
                                                    }

                                                    //Added by akshay on 15-11-2021 for CD Files
                                                    var SpanKey = _Span.pClientId + "_CDS_Today";

                                                    arr_Margin = _SpanConnectorCD.GetMargin(_Span.pMemberId, SpanKey, _Span.pExchange, _Span.pSegment, _Span.pScripName, _Span.pExpiry, _Span.pStrikePrice, _Span.pCallPut, _Span.pFactor, _Span.pQty);

                                                    if (!dict_CDSSpanMarginToday.ContainsKey(SpanKey))
                                                        dict_CDSSpanMarginToday.TryAdd(SpanKey, new double[3] { Convert.ToDouble(arr_Margin[0]), Convert.ToDouble(arr_Margin[1]), 0 });
                                                    else
                                                    {
                                                        dict_CDSSpanMarginToday[SpanKey][0] = Convert.ToDouble(arr_Margin[0]);
                                                        dict_CDSSpanMarginToday[SpanKey][1] = Convert.ToDouble(arr_Margin[1]);
                                                    }

                                                    hs_CalculatedCDSSpanKeys.Add(SpanKey);


                                                }
                                            }
                                            catch (Exception error)
                                            {
                                                isCDSSpanConnected = false;

                                                _logger.Error("CDS Span status false, " + _Span.pMemberId + " , " + _Span.pClientId + " , " + _Span.pExchange + " , " + _Span.pSegment + " , " + _Span.pScripName + " , " + _Span.pExpiry + " , " + _Span.pStrikePrice + " , " + _Span.pCallPut + " , " + _Span.pFactor + " , " + _Span.pQty + ", Span disconnected  _ " + error);
                                            }
                                        }
                                        catch (Exception error)
                                        {
                                            isCDSSpanConnected = false;

                                            _logger.Error("CDS Span status false, " + _Span.pMemberId + " , " + _Span.pClientId + " , " + _Span.pExchange + " , " + _Span.pSegment + " , " + _Span.pScripName + " , " + _Span.pExpiry + " , " + _Span.pStrikePrice + " , " + _Span.pCallPut + " , " + _Span.pFactor + " , " + _Span.pQty + "   _ " + error);
                                        }
                                    }
                                }

                                if (_IsCDSSpanComputed)
                                {
                                    if (!isCDSExpiryReach)
                                    {

                                        _GatewayDataServer.SendToEngineSpan("CDSALL^" + JsonConvert.SerializeObject(dict_CDSSpanMargin));

                                        if (isDebug)
                                            _logger.Error("CDS Span Rows : " + dict_CDSSpanMargin.Count, isDebug);

                                        Thread.Sleep(1000); //Added on 22JAN2021 by Akshay

                                        _GatewayDataServer.SendToEngineSpan("CDSEXPIRY^" + JsonConvert.SerializeObject(dict_CDSExpirySpanMargin));

                                    }
                                    else
                                    {

                                        _GatewayDataServer.SendToEngineSpan("CDSALL^" + JsonConvert.SerializeObject(dict_CDSSpanMarginToday));

                                        if (isDebug)
                                            _logger.Error("CDS Span Rows : " + dict_CDSSpanMarginToday.Count, isDebug);

                                        Thread.Sleep(1000); //Added on 22JAN2021 by Akshay

                                        _GatewayDataServer.SendToEngineSpan("CDSEXPIRY^" + JsonConvert.SerializeObject(dict_CDSExpirySpanMarginToday));

                                    }

                                    //added extra params on 17MAY2021 by Amey
                                    //changed on 15JAN2021 by Amey
                                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                                }
                            }
                        }

                        if (!isCDSSpanConnected)//05-02-2020
                        {
                            //added extra params on 17MAY2021 by Amey
                            //changed on 15JAN2021 by Amey
                            //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                            //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                            Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                            //added on 15JAN2021 by Amey
                            RestartAndComputeCDSSpan();
                        }

                        Thread.Sleep(250);
                    }

                    #endregion
                }
                else
                {
                    if (isDebug)
                        _logger.Error("CDS Image does not initialised properly ", isDebug);

                    //added extra params on 17MAY2021 by Amey
                    //changed on 15JAN2021 by Amey
                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                }
            }
            catch (Exception error)
            {
                isCDSSpanConnected = false;

                //added extra params on 17MAY2021 by Amey
                //changed on 15JAN2021 by Amey
                //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                // Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                _logger.Error("CDS Span status false, ComputeSpan " + error);

                //added on 15JAN2021 by Amey
                RestartAndComputeCDSSpan();
            }
        }


        DateTime dtNearestExpiry; //Added on 22JAN2021 by Akshay
        DateTime dtClosestExpiry; //Added on 24JAN2022 by Akshay
        DateTime dt_NearestCDSExpiry; //Added on 22JAN2021 by Akshay
        DateTime dt_ClosestCDSExpiry; //Added on 22JAN2021 by Akshay

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

        private void SelectNearestCDSExpiryDate()
        {
            try
            {
                using (MySqlConnection mySqlArrcsDBConn = new MySqlConnection(_MySQLCon))
                {
                    //changed to SP on 27APR2021 by Amey
                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetNearestCDSExpiry", mySqlArrcsDBConn))
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
                                    dt_NearestCDSExpiry = ConvertFromUnixTimestamp(Convert.ToDouble(_mySqlDataReader.GetString(0)));
                                }
                                catch (Exception ee) { _logger.Error("SelectNearestCDSExpiryDate " + ee.ToString()); }
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
                _logger.Error("SelectNearestCDSExpiryDate " + clientEx.ToString());
            }
        }

        #endregion


        #region SelectClosestExpiryDate

        //Added on 24JAN2022 by Akshay
        private void SelectClosestExpiryDate()
        {
            try
            {
                using (MySqlConnection mySqlArrcsDBConn = new MySqlConnection(_MySQLCon))
                {
                    //changed to SP on 27APR2021 by Amey
                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetClosestExpiry", mySqlArrcsDBConn))
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
                                    dtClosestExpiry = ConvertFromUnixTimestamp(Convert.ToDouble(_mySqlDataReader.GetString(0)));
                                }
                                catch (Exception ee) { _logger.Error("SelectClosestExpiryDate " + ee.ToString()); }
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
                _logger.Error("SelectClosestExpiryDate " + clientEx.ToString());
            }
        }

        private void SelectClosestCDSExpiryDate()
        {
            try
            {
                using (MySqlConnection mySqlArrcsDBConn = new MySqlConnection(_MySQLCon))
                {
                    //changed to SP on 27APR2021 by Amey
                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetClosestCDSExpiry", mySqlArrcsDBConn))
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
                                    dt_ClosestCDSExpiry = ConvertFromUnixTimestamp(Convert.ToDouble(_mySqlDataReader.GetString(0)));
                                }
                                catch (Exception ee) { _logger.Error("SelectClosestExpiryDate " + ee.ToString()); }
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
                _logger.Error("SelectClosestExpiryDate " + clientEx.ToString());
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
                        (v.Segment == "NSEFO" ? en_Segment.NSEFO : (v.Segment == "BSEFO" ? en_Segment.BSEFO : en_Segment.BSECM))),
                        ScripName = v.ScripName,
                        CustomScripName = v.CustomScripName,
                        ScripType = (v.ScripType == "EQ" ? en_ScripType.EQ : (v.ScripType == "XX" ? en_ScripType.XX : (v.ScripType == "CE" ? en_ScripType.CE :
                                    en_ScripType.PE))),
                        ExpiryUnix = v.ExpiryUnix,
                        StrikePrice = v.StrikePrice,
                        LotSize = v.LotSize,
                        UnderlyingToken = v.UnderlyingToken,
                        UnderlyingSegment = v.UnderlyingSegment == "NSECM" ? en_Segment.NSECM : (v.UnderlyingSegment == "NSECD" ? en_Segment.NSECD :
                        (v.UnderlyingSegment == "NSEFO" ? en_Segment.NSEFO : (v.Segment == "BSEFO" ? en_Segment.BSEFO : en_Segment.BSECM)))
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

                                ClearTrades = _GatewayDataServer.SendToEngineTrades(JsonConvert.SerializeObject(list_AllTrades));

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
                                //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                                //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                                Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

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
                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                }
                catch (Exception ee) { _logger.Error("ReloadAndRecalculateMargin : " + ee); }

                //added on 4JUN2021 by Amey
                SpanComputeStart = SpanComputeStartState;
            }
        }


        internal void ReloadAndRecalculateCDSMargin(bool isButtonClick = false)
        {
            try
            {
                string[] arr_Margin = new string[0];

                if (isCDSSpanConnected)
                {
                    bool CDSSpanComputeStartState = CDSSpanComputeStart;

                    //added on 22JAN2021 by Amey. To stop calculating Intraday span when recomputing.
                    CDSSpanComputeStart = false;
                    Thread.Sleep(1000);

                    //added on 17MAY2021 by Amey. To Display Span file info at Prime frontend.
                    var CDSSpanDirectory = new DirectoryInfo(_SpanFolderPath);
                    LatestCDSSpanFileName = CDSSpanDirectory.GetFiles("nsccl_ix*.spn").Any() ?
                                CDSSpanDirectory.GetFiles("nsccl_ix*.spn")
                               .OrderByDescending(f => f.LastWriteTime)
                               .First().Name : "";

                    if (LatestCDSSpanFileName == "")
                        eve_AddToList($"CDS Span file is not available. Please check BOD Utility for CDS Span file status.");
                    else if (hs_CDSSpanFileNames.Contains(LatestCDSSpanFileName))
                    {
                        if (isButtonClick)
                            eve_AddToList($"New CDS span file not available. Span file in use is [{LatestCDSSpanFileName}]");

                        //added on 28MAY2021 by Amey
                        CDSSpanComputeStart = CDSSpanComputeStartState;

                        return;
                    }
                    if (File.Exists(CDSSpanDirectory + "\\" + LatestCDSSpanFileName))
                    {
                        try
                        {
                            var tryOpen = File.ReadAllText(CDSSpanDirectory + "\\" + LatestCDSSpanFileName);
                            tryOpen = "";

                            XDocument doc = new XDocument();
                            doc = XDocument.Load(CDSSpanDirectory + "\\" + LatestCDSSpanFileName);
                        }
                        catch (Exception ee)
                        {
                            _logger.Error(ee.StackTrace);
                            CDSSpanComputeStart = CDSSpanComputeStartState;
                            return;
                        }
                    }
                    eve_AddToList($"CDS Span file {LatestCDSSpanFileName} loading.");
                    hs_CDSSpanFileNames.Add(LatestCDSSpanFileName);

                    _SpanConnectorCD.Reload("NSE", "CURR");

                    if (!isCDSExpiryReach)
                    {
                        //added on 16JUN2021 by Amey. To recompute span only for 1st span file.
                        if (hs_CDSSpanFileNames.Count == 1)
                        {
                            #region EOD Margin
                            //commented on 05APR2021 by Amey.No need to calculate EOD margin using latest Span files.
                            //changed to ForEach on 10FEB2021 by Amey
                            //added on 27NOV2020 by Amey
                            foreach (var SpanKey in dict_CDSEODMargin.Keys)
                            {
                                try
                                {
                                    if (!hs_CalculatedCDSSpanKeys.Contains(SpanKey)) continue;

                                    arr_Margin = _SpanConnectorCD.Recompute(1, SpanKey);
                                    if (arr_Margin[2] == "")
                                    {
                                        //changed code on 17DEC2020 by Amey
                                        dict_CDSEODMargin[SpanKey][0] = Convert.ToDouble(arr_Margin[0]);
                                        dict_CDSEODMargin[SpanKey][1] = Convert.ToDouble(arr_Margin[1]);
                                    }
                                }
                                catch (Exception ee)
                                {
                                    _logger.Error("ReloadAndRecalculateCDSMargin Loop EOD : " + ee);

                                    //added on 28MAY2021 by Amey
                                    CDSSpanComputeStart = CDSSpanComputeStartState;

                                    //added on 15JAN2021 by Amey
                                    isCDSSpanConnected = false;
                                    RestartAndComputeCDSSpan();

                                    return;
                                }
                            }

                            _GatewayDataServer.SendToEngineSpan("CDSEOD^" + JsonConvert.SerializeObject(dict_CDSEODMargin));
                            #endregion
                        }

                        //changed to ForEach on 10FEB2021 by Amey
                        foreach (var SpanKey in dict_CDSSpanMargin.Keys)
                        {
                            try
                            {
                                if (!hs_CalculatedCDSSpanKeys.Contains(SpanKey)) continue;

                                arr_Margin = _SpanConnectorCD.Recompute(1, SpanKey);
                                if (arr_Margin[2] == "")
                                {
                                    //changed code on 17DEC2020 by Amey
                                    dict_CDSSpanMargin[SpanKey][0] = Convert.ToDouble(arr_Margin[0]);
                                    dict_CDSSpanMargin[SpanKey][1] = Convert.ToDouble(arr_Margin[1]);
                                }
                            }
                            catch (Exception ee)
                            {
                                _logger.Error("ReloadAndRecalculateMargin Loop : " + ee);

                                //added on 15JAN2021 by Amey
                                isCDSSpanConnected = false;
                                RestartAndComputeCDSSpan();

                                //added on 28MAY2021 by Amey
                                CDSSpanComputeStart = CDSSpanComputeStartState;

                                return;
                            }
                        }

                        _GatewayDataServer.SendToEngineSpan("CDSALL^" + JsonConvert.SerializeObject(dict_CDSSpanMargin));

                        //changed to ForEach on 10FEB2021 by Amey
                        //Added by Akshay on 11-12-2020 for Expiry Span
                        foreach (var SpanKey in dict_CDSExpirySpanMargin.Keys)
                        {
                            try
                            {
                                if (!hs_CalculatedCDSSpanKeys.Contains(SpanKey)) continue;

                                arr_Margin = _SpanConnectorCD.Recompute(1, SpanKey);
                                if (arr_Margin[2] == "")
                                {
                                    //changed code on 17DEC2020 by Amey
                                    dict_CDSExpirySpanMargin[SpanKey][0] = Convert.ToDouble(arr_Margin[0]);
                                    dict_CDSExpirySpanMargin[SpanKey][1] = Convert.ToDouble(arr_Margin[1]);
                                }
                            }
                            catch (Exception ee)
                            {
                                _logger.Error("ReloadAndRecalculateCDSMargin Loop EXPIRY : " + ee);

                                //added on 15JAN2021 by Amey
                                isCDSSpanConnected = false;
                                RestartAndComputeCDSSpan();

                                //added on 28MAY2021 by Amey
                                CDSSpanComputeStart = CDSSpanComputeStartState;

                                return;
                            }
                        }

                        _GatewayDataServer.SendToEngineSpan("CDSEXPIRY^" + JsonConvert.SerializeObject(dict_CDSExpirySpanMargin));

                        //added on 22JAN2021 by Amey
                        CDSSpanComputeStart = CDSSpanComputeStartState;

                    }
                    else
                    {
                        //added on 16JUN2021 by Amey. To recompute span only for 1st span file.
                        if (hs_CDSSpanFileNames.Count == 1)
                        {
                            #region EOD Margin
                            //commented on 05APR2021 by Amey.No need to calculate EOD margin using latest Span files.
                            //changed to ForEach on 10FEB2021 by Amey
                            //added on 27NOV2020 by Amey
                            foreach (var SpanKey in dict_CDSEODMarginToday.Keys)
                            {
                                try
                                {
                                    if (!hs_CalculatedCDSSpanKeys.Contains(SpanKey)) continue;

                                    arr_Margin = _SpanConnectorCD.Recompute(1, SpanKey);
                                    if (arr_Margin[2] == "")
                                    {
                                        //changed code on 17DEC2020 by Amey
                                        dict_CDSEODMarginToday[SpanKey][0] = Convert.ToDouble(arr_Margin[0]);
                                        dict_CDSEODMarginToday[SpanKey][1] = Convert.ToDouble(arr_Margin[1]);
                                    }
                                }
                                catch (Exception ee)
                                {
                                    _logger.Error("ReloadAndRecalculateCDSMargin Loop EOD : " + ee);

                                    //added on 28MAY2021 by Amey
                                    CDSSpanComputeStart = CDSSpanComputeStartState;

                                    //added on 15JAN2021 by Amey
                                    isCDSSpanConnected = false;
                                    RestartAndComputeCDSSpan();

                                    return;
                                }
                            }

                            _GatewayDataServer.SendToEngineSpan("CDSEOD^" + JsonConvert.SerializeObject(dict_CDSEODMarginToday));
                            #endregion
                        }

                        //changed to ForEach on 10FEB2021 by Amey
                        foreach (var SpanKey in dict_CDSSpanMarginToday.Keys)
                        {
                            try
                            {
                                if (!hs_CalculatedCDSSpanKeys.Contains(SpanKey)) continue;

                                arr_Margin = _SpanConnectorCD.Recompute(1, SpanKey);
                                if (arr_Margin[2] == "")
                                {
                                    //changed code on 17DEC2020 by Amey
                                    dict_CDSSpanMarginToday[SpanKey][0] = Convert.ToDouble(arr_Margin[0]);
                                    dict_CDSSpanMarginToday[SpanKey][1] = Convert.ToDouble(arr_Margin[1]);
                                }
                            }
                            catch (Exception ee)
                            {
                                _logger.Error("ReloadAndRecalculateMargin Loop : " + ee);

                                //added on 15JAN2021 by Amey
                                isCDSSpanConnected = false;
                                RestartAndComputeCDSSpan();

                                //added on 28MAY2021 by Amey
                                CDSSpanComputeStart = CDSSpanComputeStartState;

                                return;
                            }
                        }

                        _GatewayDataServer.SendToEngineSpan("CDSALL^" + JsonConvert.SerializeObject(dict_CDSSpanMarginToday));

                        //changed to ForEach on 10FEB2021 by Amey
                        //Added by Akshay on 11-12-2020 for Expiry Span
                        foreach (var SpanKey in dict_CDSExpirySpanMarginToday.Keys)
                        {
                            try
                            {
                                if (!hs_CalculatedCDSSpanKeys.Contains(SpanKey)) continue;

                                arr_Margin = _SpanConnectorCD.Recompute(1, SpanKey);
                                if (arr_Margin[2] == "")
                                {
                                    //changed code on 17DEC2020 by Amey
                                    dict_CDSExpirySpanMarginToday[SpanKey][0] = Convert.ToDouble(arr_Margin[0]);
                                    dict_CDSExpirySpanMarginToday[SpanKey][1] = Convert.ToDouble(arr_Margin[1]);
                                }
                            }
                            catch (Exception ee)
                            {
                                _logger.Error("ReloadAndRecalculateCDSMargin Loop EXPIRY : " + ee);

                                //added on 15JAN2021 by Amey
                                isCDSSpanConnected = false;
                                RestartAndComputeCDSSpan();

                                //added on 28MAY2021 by Amey
                                CDSSpanComputeStart = CDSSpanComputeStartState;

                                return;
                            }
                        }

                        _GatewayDataServer.SendToEngineSpan("CDSEXPIRY^" + JsonConvert.SerializeObject(dict_CDSExpirySpanMarginToday));

                        //added on 22JAN2021 by Amey
                        CDSSpanComputeStart = CDSSpanComputeStartState;

                    }


                    //commented on 10JUN2021 by Amey. Added TradeTime as SpanComputeTime.
                    //added on 17MAY2021 by Amey
                    //_SpanComputeTime = ConvertToUnixTimestamp(DateTime.Now);

                    //added extra params on 17MAY2021 by Amey
                    //changed on 15JAN2021 by Amey
                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{isSpanConnected}|{_SpanComputeTime}|{LatestSpanFileName}"));
                    //Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));
                    Task.Run(() => _GatewayHBServer.SendToEngine($"{_FOLastTradeTime}|{_CMLastTradeTime}|{_CDLastTradeTime}|{_BSECMLastTradeTime}|{_BSEFOLastTradeTime}|{isSpanConnected}|{isCDSSpanConnected}|{_SpanComputeTime}|{_CDSSpanComputeTime}|{LatestSpanFileName}|{LatestCDSSpanFileName}"));

                }
            }
            catch (Exception ee) { _logger.Error("ReloadAndRecalculateMargin : " + ee); }
        }



        //CDS EXPIRY
        private System.Threading.Timer timer;

        private void SetUpTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;//time already passed
            }
            this.timer = new System.Threading.Timer(x =>
            {
                this.ExpiryTimeReach();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private void SetUpCDSTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;//time already passed
            }
            this.timer = new System.Threading.Timer(x =>
            {
                this.CDSExpiryTimeReach();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private void ExpiryTimeReach()
        {
            if (isSpanConnected && dict_SpanMargin.Any())
            {
                bool SpanComputeStartState = SpanComputeStart;
                try
                {
                    SpanComputeStart = false;
                    Thread.Sleep(1000);

                    dict_SpanMargin.Clear();
                    dict_EODMargin.Clear();
                    dict_ExpirySpanMargin.Clear();

                    dict_SpanMargin = dict_SpanMarginToday;
                    dict_EODMargin = dict_EODMarginToday;
                    dict_ExpirySpanMargin = dict_ExpirySpanMarginToday;

                }
                catch (Exception ee) { _logger.Error("SomeMethodRunsAt1600 : " + ee); }

                SpanComputeStart = SpanComputeStartState;
            }
        }

        private void CDSExpiryTimeReach()
        {
            if (isCDSSpanConnected && dict_CDSSpanMargin.Any())
            {
                bool CDSSpanComputeStartState = CDSSpanComputeStart;
                try
                {
                    isCDSExpiryReach = true;

                    CDSSpanComputeStart = false;
                    Thread.Sleep(1000);

                    dict_CDSSpanMargin.Clear();
                    dict_CDSEODMargin.Clear();
                    dict_CDSExpirySpanMargin.Clear();

                    //dict_CDSSpanMargin = dict_CDSSpanMarginToday;
                    //dict_CDSEODMargin = dict_CDSEODMarginToday;
                    //dict_CDSExpirySpanMargin = dict_CDSExpirySpanMarginToday;

                }
                catch (Exception ee) { _logger.Error("CDSExpiryTimeReach : " + ee); }

                CDSSpanComputeStart = CDSSpanComputeStartState;
            }
        }



        private void SendSpan()
        {
            try
            {
                while (true)
                {
                    if (isEODSpanComputed)
                    {
                        if (dict_SpanMargin.Any())
                        {
                            _GatewayDataServer.SendToEngineSpan("ALL^" + JsonConvert.SerializeObject(dict_SpanMargin));
                            Thread.Sleep(100);
                        }

                        if (dict_ExpirySpanMargin.Any())
                        {
                            _GatewayDataServer.SendToEngineSpan("EXPIRY^" + JsonConvert.SerializeObject(dict_ExpirySpanMargin));
                            Thread.Sleep(1000);
                        }

                    }

                    #region cds
                    if (isCdsEODSpanComputed)
                    {
                        
                        if (!isCDSExpiryReach)
                        {
                            if (dict_CDSSpanMargin.Any())
                            {
                                _GatewayDataServer.SendToEngineSpan("CDSALL^" + JsonConvert.SerializeObject(dict_CDSSpanMargin));
                                Thread.Sleep(100);
                            }

                            if (dict_CDSExpirySpanMargin.Any())
                            {
                                _GatewayDataServer.SendToEngineSpan("CDSEXPIRY^" + JsonConvert.SerializeObject(dict_CDSExpirySpanMargin));
                                Thread.Sleep(100);
                            }
                        }
                        else
                        {
                            if (dict_CDSSpanMarginToday.Any())
                            {
                                _GatewayDataServer.SendToEngineSpan("CDSALL^" + JsonConvert.SerializeObject(dict_CDSSpanMarginToday));
                                Thread.Sleep(100);
                            }
                            if (dict_CDSExpirySpanMarginToday.Any())
                            {
                                _GatewayDataServer.SendToEngineSpan("CDSEXPIRY^" + JsonConvert.SerializeObject(dict_CDSExpirySpanMarginToday));
                                Thread.Sleep(100);
                            }
                        }
                    }
                    #endregion

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ee) { _logger.Error(ee.ToString()); }
        }

        #endregion       
    }
}
