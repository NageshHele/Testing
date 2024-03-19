using Gateway.Connectors;
using Gateway.Core_Logic;
using Gateway.Data_Structures;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SpanCalculate;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gateway
{
    public delegate void del_AddToList(string Message);

    public delegate void del_TradeFileStatus(bool isFOAvailable);

    public delegate void del_EngineSpanConnection(bool isSpanConnected, bool isEngineConnected);

    public delegate void del_SpanTime(double SpanTime);

    public delegate void del_ButtonState(bool buttonstate);

    public class nProcess
    {
        public event del_AddToList eve_AddToList;
        public event del_TradeFileStatus eve_TradeFileStatus;
        public event del_EngineSpanConnection eve_ConnectionStatus;
        public event del_SpanTime eve_SpanTime;
        public event del_ButtonState eve_ButtonState;

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

        /// <summary>
        /// Set true when connected to Engine. False when disconnedted or crashed.
        /// </summary>
        bool isEngineConnected = false;

        /// <summary>
        /// Set true when connected to Span. False when disconnedted or crashed.
        /// </summary>
        bool isSpanConnected = false;

        /// <summary>
        /// Set true 
        /// </summary>
        bool StartIntradayFileReading = false;

        /// <summary>
        /// Set true when Gateway Starts
        /// </summary>
        bool SpanComputeStart = false;

        /// <summary>
        /// Sets true when while computing span
        /// </summary>
        bool isSpanComputing = false;

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
        DateTime dt_ExpiryTime = DateTime.Parse("18:00:00");

        /// <summary>
        /// Key: Client_Underlying_Con | Value: List of Span of that particular spankey
        /// </summary>
        ConcurrentDictionary<string, List<Span>> dict_Span = new ConcurrentDictionary<string, List<Span>>();

        //Added by Snehadri on 23FEB2022
        /// <summary>
        /// Key: BSE Symbols | Value: Corresponding NSE Symbols
        /// </summary>
        Dictionary<string, string> dict_BSEMapping = new Dictionary<string, string>();

        /// <summary>
        /// Interval for the span calculation
        /// </summary>
        int interval;

        HashSet<string> hs_BrokerID = new HashSet<string>();

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
                CTradeProcess.Initialise(_logger, ds_Config, isDebug, dict_CustomScripInfo, dict_ScripInfo, dict_TokenScripInfo, dict_BSEMapping, hs_BrokerID);
                CTradeProcess.Instance.eve_TradeTime += Instance_eve_TradeTime;
                CTradeProcess.Instance.eve_TradeFileStatus += Instance_eve_TradeFileStatus;

            }
            catch (Exception Error)
            {
                _logger.Error("DumpData CTor : " + Error);
            }
        }

        private void Instance_eve_TradeFileStatus(bool isFOAvailable)
        {
            eve_TradeFileStatus(isFOAvailable);
        }

        #region TradeProcess Events

        private void Instance_eve_TradeTime(double _FOLastTradeTime)
        {
            this._FOLastTradeTime = _FOLastTradeTime;
        }

        #endregion

        #region Supplimentary Methods

        //added on 15JAN2021 by Amey
        private void ReadConfig()
        {
            try
            {
                _nImageExeName = ds_Config.Tables["NIMAGE2"].Rows[0]["EXE"].ToString();

                isDebug = Convert.ToBoolean(ds_Config.Tables["DEBUG-MODE"].Rows[0]["ENABLE"].ToString());
                isAdmin = Convert.ToBoolean(ds_Config.Tables["NIMAGE2"].Rows[0]["ADMIN"].ToString());

                foreach (var _Time in ds_Config.Tables["NIMAGE2"].Rows[0]["RECOMPUTE-TIME"].ToString().Split(','))
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



        private void SetupGatewayServers()
        {
            try
            {
                var CONInfo = ds_Config.Tables["CONNECTION"].Rows[0];
                //changed on 13JAN2021 by Amey
                var ip_GatewayServer = IPAddress.Parse(CONInfo["GATEWAY-SPAN-SERVER-IP"].ToString());
                var _GatewayHBServerPORT = Convert.ToInt32(CONInfo["GATEWAY-SPAN-SERVER-HB-PORT"].ToString());
                var _GatewaySpanServerPORT = Convert.ToInt32(CONInfo["GATEWAY-SPAN-SERVER-PORT"].ToString());

                var _TimeoutMS = Convert.ToInt32(ds_Config.Tables["OTHER"].Rows[0]["TIMEOUT-SECONDS"]) * 1000;

                _GatewayHBServer.eve_ErrorReceived += _logger.Error;
                _GatewayHBServer.eve_SignalReceived += ReceiveResponseFromEngine;
                _GatewayHBServer.Setup(ip_GatewayServer, _GatewayHBServerPORT, _TimeoutMS);

                _GatewayDataServer.eve_ErrorReceived += _logger.Error;
                _GatewayDataServer.SetupSpan(ip_GatewayServer, _GatewaySpanServerPORT, _TimeoutMS);

                interval = Convert.ToInt32(ds_Config.Tables["NIMAGE2"].Rows[0]["INTERVAL"]);
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
                    isEngineConnected = true;
                    eve_ConnectionStatus(isSpanConnected, isEngineConnected);
                }
                else if (_Signal.Equals("STOP"))
                {
                    //added on 22JAN2021 by Amey
                    eve_AddToList("Engine Stopped.");
                    isEngineConnected = false;
                    eve_ConnectionStatus(isSpanConnected, isEngineConnected);
                }
                else if (_Signal.Equals("UPDATE"))
                {
                    SelectClientfromDatabase();
                    CTradeProcess.Instance.UpdateCollections(dict_ClientInfo);
                    eve_ConnectionStatus(isSpanConnected, isEngineConnected);
                }
            }
            catch (Exception error)
            {
                _logger.Error("ReceiveResponseFromEngine : " + error);
            }
        }

        internal void StartSpanProcess()
        {
            try
            {
                hs_CalculatedSpanKeys.Clear();

                SelectClientfromDatabase();
                ReadEODTable();

                //added on 16JUN2021 by Amey
                CTradeProcess.Instance.UpdateCollections(dict_ClientInfo);
                CTradeProcess.Instance.GatewayRestarted();

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
            }
            catch (Exception error)
            {
                _logger.Error("StartProcess : " + error);
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

                if (isDebug)
                    _logger.Error("Initialising Image.", isDebug);

                isSpanConnected = false;
                _SpanConnector = null;

                dict_SpanMargin.Clear();

                EndProcess(_nImageExeName);

                //added on 15JAN2021 by Amey
                StartProcess(Application.StartupPath + "\\" + _nImageExeName, isAdmin);

                //added on 01FEB2021 by Amey
                Thread.Sleep(300);

                IPAddress ImageIP = IPAddress.Parse(ds_Config.Tables["NIMAGE2"].Rows[0]["ADDRESS"].ToString().Split(':')[0]);
                int ImagePORT = Convert.ToInt32(ds_Config.Tables["NIMAGE2"].Rows[0]["ADDRESS"].ToString().Split(':')[1]);
                _SpanConnector = SpanConnectorFO.Instance(ImageIP, ImagePORT);

                SelectNearestExpiryDate();

                if (_SpanConnector != null)
                {
                    eve_AddToList("nImage Initialised successfully");
                    eve_AddToList("Span Calculation Started");

                    var starttime = ConvertToUnixTimestamp(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 15, 00));

                    eve_SpanTime(starttime);

                    isSpanConnected = true;
                    eve_ConnectionStatus(isSpanConnected, isEngineConnected);
                    
                    InitialiseSpan();

                }
                else
                {
                    isSpanConnected = false;
                    eve_ConnectionStatus(isSpanConnected, isEngineConnected);
                    RestartAndComputeSpan();

                }
            }
            catch (Exception error)
            {
                _logger.Error("RestartAndComputeSpan : " + error);
            }
        }

        private void InitialiseSpan()
        {
            try
            {
                var list_NSEFOEOD = list_EODPositions.Where(v => v.Segment == en_Segment.NSEFO).ToList();

                if (list_NSEFOEOD.Any())
                    ConsolidatePositions(new List<PositionInfo>(list_NSEFOEOD), true);
                
                StartIntradayFileReading = true;

                var timer = new System.Timers.Timer();
                timer.Interval = interval * 1000;
                
                timer.Elapsed += (sender, e) =>
                {
                    if (!isSpanComputing && dict_Span.Any())
                        Task.Run(() => ComputeSpan());
                };
                timer.AutoReset = true;
                timer.Enabled = true;
            }
            catch (Exception error)
            {
                _logger.Error("InitialiseSpan : " + error);
            }
        }

        internal void ComputeSpan()
        {
            try
            {
                if (_SpanConnector != null)
                {
                    if (isSpanConnected)
                    {
                        if (SpanComputeStart && dict_Span.Any())
                        {
                            isSpanComputing = true;
                            eve_ButtonState(false);

                            dict_SpanMargin.Clear();

                            List<Span> list_TempSpan = new List<Span>(dict_Span.Values.SelectMany(v => v).ToList());
                                                       
                            string timestamp = DateTime.Now.ToString("HH:mm:ss:ffff");

                            Stopwatch sw = new Stopwatch();
                            sw.Start();

                            SelectNearestExpiryDate();      //added on 22AUG2022 by Ninad

                            foreach (var _Span in list_TempSpan)
                            {
                                try
                                {

                                    string[] arr_Margin = new string[3] { "0", "0", "0" };
                                    string[] arr_ExpMargin = new string[3] { "0", "0", "0" };

                                    if (!SpanComputeStart || _SpanConnector == null)
                                    {                                       
                                        isSpanComputing = false;
                                        return;
                                    }

                                    string _SpanKey = _Span.pClientId + "_" + timestamp;
                                    string ExpiryKey = _Span.pClientId + "_EXP_" + timestamp;   //added on 22AUG2022

                                    DateTime dtExpiry = DateTime.ParseExact(_Span.pExpiry, "yyyyMMdd", CultureInfo.InvariantCulture);

                                    
                                    //no need to calculate margin for expired contracts. Added on 26APR2021 by Amey.
                                    if (DateTime.Now.Date == dtExpiry.Date && DateTime.Now > dt_ExpiryTime)
                                        continue;
                                                                        

                                    //added on 22AUG2022 by Ninad
                                    if (dtNearestExpiry < dtExpiry)
                                    {
                                        arr_ExpMargin = _SpanConnector.GetMargin(_Span.pMemberId, ExpiryKey, _Span.pExchange, _Span.pSegment, _Span.pScripName, _Span.pExpiry, _Span.pStrikePrice, _Span.pCallPut, _Span.pFactor, _Span.pQty.ToString());
                                    }
                                    arr_Margin = _SpanConnector.GetMargin(_Span.pMemberId, _SpanKey, _Span.pExchange, _Span.pSegment, _Span.pScripName, _Span.pExpiry, _Span.pStrikePrice, _Span.pCallPut, _Span.pFactor, _Span.pQty.ToString());

                                    var ExpSpan = Convert.ToDouble(arr_ExpMargin[0]);
                                    var ExpExposure = Convert.ToDouble(arr_ExpMargin[1]);
                                    var Span = Convert.ToDouble(arr_Margin[0]);
                                    var Exposure = Convert.ToDouble(arr_Margin[1]);


                                    if (dict_SpanMargin.TryGetValue(_SpanKey, out double[] arr_oldMargin))
                                    {
                                        arr_oldMargin[0] = Span;
                                        arr_oldMargin[1] = Exposure;
                                        arr_oldMargin[2] = ExpSpan == 0 ? arr_oldMargin[2] : ExpSpan;
                                        arr_oldMargin[3] = ExpExposure == 0 ? arr_oldMargin[3] : ExpExposure;
                                    }
                                    else
                                    {
                                        dict_SpanMargin.TryAdd(_SpanKey, new double[4] { Span, Exposure, ExpSpan, ExpExposure });
                                    }

                                }
                                catch (Exception error)
                                {
                                    isSpanConnected = false;

                                    eve_ConnectionStatus(isSpanConnected, isEngineConnected);

                                    _logger.Error("Span status false, " + _Span.pMemberId + " , " + _Span.pClientId + " , " + _Span.pExchange + " , " + _Span.pSegment + " , " + _Span.pScripName + " , " + _Span.pExpiry + " , " + _Span.pStrikePrice + " , " + _Span.pCallPut + " , " + _Span.pFactor + " , " + _Span.pQty + ", Span disconnected  _ " + error);
                                }
                            }

                            sw.Stop();
                            //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:fff") + $"- SpanTime: {sw.ElapsedMilliseconds}ms, SpanList Count: {list_TempSpan.Count}");
                            _logger.Error($"SpanTime: {sw.ElapsedMilliseconds}ms, SpanList Count: {list_TempSpan.Count}");

                            var spantime = list_TempSpan.OrderByDescending(v => v.pTradeTime).Select(x => x).FirstOrDefault();
                            if (spantime != null && spantime.pTradeTime != 0)
                                eve_SpanTime(spantime.pTradeTime);

                            if (dict_SpanMargin.Any() && (_GatewayDataServer.isEngineSpanConnected && isEngineConnected))
                            {

                                Task.Run(() =>
                                {
                                    var success = _GatewayDataServer.SendToEngineSpan("CON^" + JsonConvert.SerializeObject(dict_SpanMargin));
                                    if (!success)
                                    {
                                        isEngineConnected = false;
                                        eve_ConnectionStatus(isSpanConnected, isEngineConnected);
                                        eve_AddToList("Engine Stopped.");
                                        SetupGatewayServers();
                                    }

                                });
                            }

                            eve_ButtonState(true);
                            isSpanComputing = false;
                        }
                    }
                }

                if (!isSpanConnected)
                {
                    eve_ConnectionStatus(isSpanConnected, isEngineConnected);
                    RestartAndComputeSpan();
                }

            }
            catch (Exception error)
            {
                isSpanConnected = false;

                eve_ConnectionStatus(isSpanConnected, isEngineConnected);

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
                    if (StartIntradayFileReading)
                    {
                        var list_AllTrades = CTradeProcess.Instance.GetTrades(ClearTrades);

                        //added on 25JAN2021 by Amey
                        //eve_TradeFileStatus(isTradeFileFOAvailable, isTradeFileCMAvailable, isTradeFileBSECMAvailable);

                        eve_ConnectionStatus(isSpanConnected, isEngineConnected);

                        try
                        {
                            //changed to Any() on 28APR2021 by Amey
                            if (list_AllTrades.Any())
                            {
                                if (isDebug)
                                    _logger.Error("Trades Read in " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs", isDebug);

                                ConsolidatePositions(new List<PositionInfo>(list_AllTrades));

                                if (isEngineConnected)
                                {
                                    Task.Run(() =>
                                    {
                                        var success = _GatewayHBServer.SendToEngine("Connected");
                                        if (!success)
                                        {
                                            isEngineConnected = false;
                                            eve_ConnectionStatus(isSpanConnected, isEngineConnected);
                                            eve_AddToList("Engine Stopped.");
                                            SetupGatewayServers();
                                        }
                                    });
                                }


                                Thread.Sleep(10000);
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
                        SpanComputeStart = true;

                        return;
                    }

                    eve_AddToList($"Span file {LatestSpanFileName} loading.");
                    hs_SpanFileNames.Add(LatestSpanFileName);

                    _SpanConnector.Reload("NSE", "EQFO");

                    SpanComputeStart = true;
                }
                catch (Exception ee) { _logger.Error("ReloadAndRecalculateMargin : " + ee); }

                //added on 4JUN2021 by Amey

            }
        }

        private void ConsolidatePositions(List<PositionInfo> list_RecvPosition, bool isEODPosition = false)
        {
            try
            {
                int tradecount = list_RecvPosition.Count;

                Stopwatch sw = new Stopwatch();
                sw.Start();

                List<Span> list_TempSpan = new List<Span>();

                Span _span;

                foreach (var _Position in list_RecvPosition)
                {
                    var _ScripInfo = dict_TokenScripInfo[$"NSEFO|{_Position.Token}"];
                    var User_Symbol = _Position.Username + "_" + _ScripInfo.Symbol + "_CON";
                    var _ExpiryDate = ConvertFromUnixTimestamp(_ScripInfo.ExpiryUnix);

                    //_span = list_Span.AsQueryable().Where(v => v.pClientId == User_Symbol && v.pToken == _Position.Token).FirstOrDefault();

                    if (dict_Span.ContainsKey(User_Symbol))
                    {
                        _span = dict_Span[User_Symbol].Where(v => v.pClientId == User_Symbol && v.pToken == _Position.Token).FirstOrDefault();

                        if (_span != null)
                        {
                            _span.pQty += _Position.IntradayQuantity;
                            _span.pTradeTime = _Position.TradeTime;
                        }
                        else
                        {
                            _span = new Span()
                            {
                                pMemberId = 1,
                                pClientId = User_Symbol,
                                pExchange = "NSE",
                                pSegment = "EQFO",
                                pScripName = _ScripInfo.Symbol,
                                pExpiry = _ExpiryDate.ToString("yyyyMMdd"),
                                pFactor = "E",
                                pQty = _Position.TradeQuantity,
                                pToken = _Position.Token,
                                pTradeTime = _Position.TradeTime
                            };

                            if (_ScripInfo.ScripType == en_ScripType.XX)
                            {
                                _span.pStrikePrice = "";
                                _span.pCallPut = "";
                            }
                            else
                            {
                                _span.pStrikePrice = _ScripInfo.StrikePrice.ToString("#.00");
                                _span.pCallPut = _ScripInfo.ScripType.ToString().Substring(0, 1);
                            }

                            dict_Span[User_Symbol].Add(_span);

                        }
                    }
                    else
                    {
                        _span = new Span()
                        {
                            pMemberId = 1,
                            pClientId = User_Symbol,
                            pExchange = "NSE",
                            pSegment = "EQFO",
                            pScripName = _ScripInfo.Symbol,
                            pExpiry = _ExpiryDate.ToString("yyyyMMdd"),
                            pFactor = "E",
                            pQty = _Position.TradeQuantity,
                            pToken = _Position.Token,
                            pTradeTime = _Position.TradeTime
                        };

                        if (_ScripInfo.ScripType == en_ScripType.XX)
                        {
                            _span.pStrikePrice = "";
                            _span.pCallPut = "";
                        }
                        else
                        {
                            _span.pStrikePrice = _ScripInfo.StrikePrice.ToString("#.00");
                            _span.pCallPut = _ScripInfo.ScripType.ToString().Substring(0, 1);
                        }

                        dict_Span.TryAdd(User_Symbol, new List<Span>() { _span });
                    }

                };

                sw.Stop();
                //Console.WriteLine($"ConsolidationTime: {sw.ElapsedMilliseconds}ms, TradeCount: {tradecount}");
                _logger.Error($"ConsolidationTime: {sw.ElapsedMilliseconds}ms, TradeCount: {tradecount}");

                if (isEODPosition)
                    Task.Run(() => ComputeSpan());
            }
            catch (Exception ee) { _logger.Error("ConsolidatePositions: " + ee); }
        }

        
    }
}
