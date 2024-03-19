using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Xml;
using MySql.Data.MySqlClient;
using System.IO;
using DevExpress.Skins;
using System.Net;
using System.Threading;
using FeedLibrary;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using NSEUtilitaire;
using n.Structs;
using Engine.UI;
using Engine.Data_Structures;
using XDMessaging;
using FeedLibrary.Data_Structures;
using System.Diagnostics;
using Engine.Connectors;
using en_Segment = n.Structs.en_Segment;


//Name of Sub-Client: EP
namespace Engine
{
    public partial class EngineProcess : XtraForm
    {
        //added on 21DEC2020 by Amey
        /// <summary>
        /// ENGINE-SERVER-IP from Config.
        /// </summary>
        IPAddress ip_EngineServer = IPAddress.Parse("127.0.0.1");

        //added on 25JAN2021 by Amey
        /// <summary>
        /// GATEWAY-SERVER-IP from Config.
        /// </summary>
        IPAddress ip_GatewayServer = IPAddress.Parse("127.0.0.1");

        /// <summary>
        /// GATEWAY-SPAN-SERVER-IP from config
        /// </summary>
        IPAddress ip_nImageB_Server = IPAddress.Parse("127.0.0.1");

        /// <summary>
        /// GATEWAY-SERVER-HB-PORT, ENGINE-SERVER-TRADE-PORT, ENGINE-SERVER-SPAN-PORT from Config.
        /// </summary>
        int _GatewayServerHBPORT, _nImageBServerHBPORT, _EngineServerTradePORT, _EngineServerSpanPORT, _EngineServerHBPORT, _GatewayServerTradePORT, _GatewayServerSpanPORT, _nImageBPORT;

        //added LTT on 18NOV2020 by Amey
        //added FOLTT & CMLTT on 07JAN2021 by Amey
        double _FOLastTradeTime = 0, _CMLastTradeTime = 0, _CDLastTradeTime = 0, _BSECMLastTradeTime = 0, _BSEFOLastTradeTime = 0, isGatewayConnected = 0, isSpanConnected = 0, isCDSSpanConnected = 0, FOLTT = 0, CMLTT = 0, BSEFOLTT = 0, CDLTT = 0, _SpanComputeTime = 0, _CDSSpanComputeTime = 0;

        /// <summary>
        /// Used to send Positions and Span to Prime. Uses _EngineServerTradePORT, _EngineServerSpanPORT.
        /// </summary>
        PrimeDataConnector _EngineDataServer = new PrimeDataConnector();

        /// <summary>
        /// Used to receive data from Gateway. Uses ip_GatewayServer, _GatewayServerHBPORT from config.
        /// </summary>
        GatewayHBConnector _GatewayHeartBeatClient = new GatewayHBConnector();


        /// <summary>
        /// Used to receive data from nImageB. Uses ip_EngineServer, _GatewayServerHBPORT from config.
        /// </summary>
        nImageBHBConnector _nImageBHeartBeatClient = new nImageBHBConnector();

        /// <summary> 
        /// Used to receive data from Gateway. Uses ip_GatewayServer, _GatewayServerTradePORT, _GatewayServerSpanPORT from config.
        /// </summary>
        GatewayDataConnector _GatewayDataClient = new GatewayDataConnector();

        /// <summary> 
        /// Used to receive data from Gateway. Uses ip_GatewayServer, _GatewayServerTradePORT, _GatewayServerSpanPORT from config.
        /// </summary>
        nImageBDataConnector _nImageBDataClient = new nImageBDataConnector();

        /// <summary>
        /// Used to send Heartbeat data to Prime. Uses _EngineServerHBPORT.
        /// </summary>
        PrimeHBConnector _EngineHeartBeatServer = new PrimeHBConnector();

        // Added by Snehadri on 30JUN2021 
        /// <summary>
        /// Added for Automatic BOD Process
        /// </summary>
        BODUtilityConnector bODUtilityConnector = new BODUtilityConnector();

        clsWriteLog _logger;//object of write log added by navin on 28-06-2018

        Feed _FeedLibrary = new Feed();

        /// <summary>
        /// Contents of Config.xml
        /// </summary>
        DataSet ds_Config = new DataSet();

        /// <summary>
        /// MySQL connection string.
        /// </summary>
        string _MySQLCon = string.Empty;

        /// <summary>
        /// version mismatch
        /// </summary>
        string _Version = "";

        string _LatestSpanFileName = string.Empty;
        string _LatestCDSSpanFileName = string.Empty;

        int _MaxAllowedInstances = 0;

        /// <summary>
        /// Value for SendTimeout for sockets.
        /// </summary>
        int _TimeoutMilliseconds = 120000;

        /// <summary>
        /// DEBUG-MODE/ENABLE from Config.
        /// </summary>
        bool isDebug = false;

        /// <summary>
        /// Set true when clicked on Start button. Set false when clicked on Stop button.
        /// </summary>
        bool isEngineStarted = false;

        /// <summary>
        /// Set false when Engine exe closing.
        /// </summary>
        bool isEngineRunning = true;

        /// <summary>
        /// set True when clicked on Start button, set False after clearing tbl_AllTrades.
        /// </summary>
        bool StartFromZero = false;

        /// <summary>
        /// Set True when trades received from Gateway.
        /// </summary>
        bool isNewTrades = false;

        // Added by Snehadri on 09MAR2022
        /// <summary>
        /// Set true whne computing Peak Margin
        /// </summary>
        bool IsPeakMarginComputing = false;

        bool IsCDSPeakMarginComputing = false;
 
        //added default values on 01DEC2020 by Amey
        double _InterestRate = 1, _IVMultiplier = 2, _IVDivisor = 2;

        /// <summary>
        /// Key : Segment|UnderlyingName | Value : [0] IVMiddle, [1] IVLower, [2] IVHigher
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_DefaultIVs = new ConcurrentDictionary<string, double[]>();//navin 23-11-2017

        //added on 12JAN2021 by Amey
        //ds_Engine.dt_AllTradesDataTable dt_AllTradesNew = new ds_Engine.dt_AllTradesDataTable();
        //ds_Engine.dt_AllTradesDataTable dt_EODTradesNew = new ds_Engine.dt_AllTradesDataTable();
        //ds_Engine.dt_AllTradesDataTable dt_CurrentTradesNew = new ds_Engine.dt_AllTradesDataTable();
        //ds_Engine.dt_AllTradesDataTable dt_Day1TradesNew = new ds_Engine.dt_AllTradesDataTable();

        //added on 10FEB2021 by Amey
        List<PositionInfo> list_AllTrades = new List<PositionInfo>();
        List<PositionInfo> list_EODPositions = new List<PositionInfo>();
        List<PositionInfo> list_CurrentPositions = new List<PositionInfo>();

        /// <summary>
        /// Lock for operations on list_AllTrades.
        /// </summary>
        private readonly object _TradeLock = new object();

        /// <summary>
        /// Lock for operations on dict_Span.
        /// </summary>
        //private readonly object _SpanLock = new object();

        /// <summary>
        /// Lock for operations on dict_ExpSpan.
        /// </summary>
        //private readonly object _ExpSpanLock = new object();

        /// <summary>
        /// Lock for operations on dict_EODSpan.
        /// </summary>
        //private readonly object _EODSpanLock = new object();

        //changed to different format on 20APR2021 by Amey
        List<EODPositionInfo> list_Day1Positions = new List<EODPositionInfo>();

        /// <summary>
        /// Key : ID | Value : Consolidated trades of respective ID
        /// </summary>
        //Dictionary<string, ds_Engine.ConsolidateTradeinfoDataTable> dict_ConsolidatedPos = new Dictionary<string, ds_Engine.ConsolidateTradeinfoDataTable>();
        Dictionary<string, List<ConsolidatedPositionInfo>> dict_ConsolidatedPos = new Dictionary<string, List<ConsolidatedPositionInfo>>();

        /// <summary>
        /// Key : CustomScripName | Value : Token
        /// </summary>
        //Dictionary<string, int> dict_ScripToken = new Dictionary<string, int>();   //changed on 9-1-18 by shri

        /// <summary>
        /// Key : Token | Value : Expiry in Unix
        /// </summary>
        //Dictionary<int, double> dict_ScripExpiry = new Dictionary<int, double>();  //changed on 9-1-18 by shri

        /// <summary>
        /// Key : ScripName | Value : Token
        /// </summary>
        //Dictionary<string, int> dict_OScripToken = new Dictionary<string, int>();  //changed on 9-1-18 by shri

        /// <summary>
        /// Key : Token | Value : OScripName
        /// </summary>
        //Dictionary<int, string> dict_TokenScrip = new Dictionary<int, string>();

        /// <summary>
        /// Key : ClientID_Underlying | Value : [0] Span [1] Exposure [2] EQSpan
        /// </summary>
        Dictionary<string, double[]> dict_SpanMargin = new Dictionary<string, double[]>();

        /// <summary>
        /// Key : ClientID_Underlying_EOD | Value : EOD Margin only.
        /// </summary>
        Dictionary<string, double[]> dict_EODMargin = new Dictionary<string, double[]>();

        /// <summary>
        /// Key : ClientID_Underlying_EXP | Value : Expiry Margin only.
        /// </summary>
        Dictionary<string, double[]> dict_ExpirySpanMargin = new Dictionary<string, double[]>();

        //CDS
        /// <summary>
        /// Key : ClientID_Underlying | Value : [0] Span [1] Exposure [2] EQSpan
        /// </summary>
        Dictionary<string, double[]> dict_CDSSpanMargin = new Dictionary<string, double[]>();

        /// <summary>
        /// Key : ClientID_Underlying_EOD | Value : EOD Margin only.
        /// </summary>
        Dictionary<string, double[]> dict_CDSEODMargin = new Dictionary<string, double[]>();

        /// <summary>
        /// Key : ClientID_Underlying_EXP | Value : Expiry Margin only.
        /// </summary>
        Dictionary<string, double[]> dict_CDSExpirySpanMargin = new Dictionary<string, double[]>();


        /// <summary>
        /// Key: ClientID | Value: [FNOPeakMargin, FNOPeakMarginTime, TotalPeakMargin, TotalPeakMarginTime]
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_PeakMargin = new ConcurrentDictionary<string, double[]>();


        /// <summary>
        /// Key: ClientID | Value: [FNOPeakMargin, FNOPeakMarginTime, TotalPeakMargin, TotalPeakMarginTime]
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_CDSPeakMargin = new ConcurrentDictionary<string, double[]>();

        /// <summary>
        /// Key: ClientID | Value: VARMargin
        /// </summary>
        ConcurrentDictionary<string, double> dict_ClientWiseVARMargin = new ConcurrentDictionary<string, double>();

        /// <summary>
        /// Key: ClientID_Underlying | Value : Consolidated Span Margin
        /// </summary>
        Dictionary<string, double[]> dict_ConsolidatedSpan = new Dictionary<string, double[]>();

        /// <summary>
        /// Key: ClientID   | Value: All span data
        /// </summary>
        Dictionary<string, double[]> dict_SpanData = new Dictionary<string, double[]>();
        /// <summary>
        /// Key: ClientID   | Value: All span data
        /// </summary>
        Dictionary<string, double[]> dict_SpanDataCDS = new Dictionary<string, double[]>();
        /// <summary>
        /// Key : Client|Underlying | Value : double value.
        /// </summary>
        Dictionary<string, double> dict_NPLValues = new Dictionary<string, double>();

        /// <summary>
        /// Key : ClientID | Value : double value.
        /// </summary>
        Dictionary<string, double> dict_MTDValues = new Dictionary<string, double>();

        /// <summary>
        /// Key : Segment|Token | Value : Expiry TimeSpan
        /// </summary>
        Dictionary<string, TimeSpan> dict_TokenExpiryTimeSpan = new Dictionary<string, TimeSpan>();

        /// <summary>
        /// Key : ClientID^Token | Value : BanInfo
        /// </summary>
        Dictionary<string, BanInfo> dict_BanInfo = new Dictionary<string, BanInfo>();

        const int LTPArray = 3;

        //changed to double array on 15OCT2020 by Amey
        /// <summary>
        /// Key : Segment|Token | Value : [ LTP, PreviosClose, 0/1(0 if LTP received is closing from yesterday, 1 if LTP is live) ]
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_LTP = new ConcurrentDictionary<string, double[]>();

        /// <summary>
        /// Key : Segment|Token | Value : Name
        /// </summary>
        ConcurrentDictionary<string, string> dict_IndexTokens = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Key : Name | Value : LTP
        /// </summary>
        ConcurrentDictionary<string, double> dict_IndexLTP = new ConcurrentDictionary<string, double>();

        /// <summary>
        /// Key : Segment|Token | Value : Greeks values received from FeedReceiver
        /// </summary>
        ConcurrentDictionary<string, Greeks> dict_Greeks = new ConcurrentDictionary<string, Greeks>();

        /// <summary>
        /// Key : Client_Segment|Token | Value : [0] = FillQty, [1] = FillPrice
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_CFInfo = new ConcurrentDictionary<string, double[]>();

        /// <summary>
        /// Key : Underlying | Value : BetaValue
        /// </summary>
        ConcurrentDictionary<string, double> dict_BetaValue = new ConcurrentDictionary<string, double>();

        /// <summary>
        /// <para>Key => Segment|Underlying Token (12556)</para>
        /// <para>Value => Key => Closest DateTime</para>
        /// <para>Value => Key => Closest Strikes (11200)</para>
        /// <para>Value => [0] : CEToken, [1] : PEToken </para>
        /// </summary>
        ConcurrentDictionary<string, Dictionary<DateTime, Dictionary<double, int[]>>> dict_ClosestATMTokens = new ConcurrentDictionary<string, Dictionary<DateTime, Dictionary<double, int[]>>>();

        /// <summary>
        /// Key => Segment|Underlying | Value => Closest Underlying Token
        /// </summary>
        ConcurrentDictionary<string, int> dict_UnderlyingToken = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// Key => ScripName | Value => VAR
        /// </summary>
        ConcurrentDictionary<string, double> dict_VARMargin = new ConcurrentDictionary<string, double>();      //Added by Akshay on 31-12-2020 For Storing VAR for EQ

        /// <summary>
        /// Key : Segment|Underlying | Value : [ Avg Computed IV, 0/1(0 if IV calculated using closing from yesterday, 1 if IV calculated using LTP) ]
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_ComputedATMIV = new ConcurrentDictionary<string, double[]>();

        //Added by Akshay on 30-06-2021 for Closing Price
        /// <summary>
        /// Key : Scrip | Value : ClosingPrice
        /// </summary>
        ConcurrentDictionary<string, double> dict_ScripClosing = new ConcurrentDictionary<string, double>();
        /// <summary>
        /// Key : Segment|Token | Value : Name
        /// </summary>
        ConcurrentDictionary<string, string> dict_IndexScrips = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Key => Segment|Current Month Underlying Token | Value : [ LTP, 0/1(0 if LTP received is closing from yesterday, 1 if LTP is live) ]
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_XXLTP = new ConcurrentDictionary<string, double[]>();

        /// <summary>
        /// Key => Segment|Underlying Token | Value => [0] : CEIV, [1] : PEIV
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_IVs = new ConcurrentDictionary<string, double[]>();

        /// <summary>
        /// Key : ClientID | Value : ClientInfo
        /// </summary>
        ConcurrentDictionary<string, ClientInfo> dict_ClientInfo = new ConcurrentDictionary<string, ClientInfo>();

        /// <summary>
        /// Key : Prime-Username | Value : [0] Password
        /// </summary>
        ConcurrentDictionary<string, string> dict_UserInfo = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Key : Prime-Username | Value : Mapped Clients array
        /// </summary>
        ConcurrentDictionary<string, string[]> dict_UserMappedInfo = new ConcurrentDictionary<string, string[]>();

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
        /// Key : Spot Underlying | Value : Segment|Token
        /// </summary>
        ConcurrentDictionary<string, string> dict_SpotTokenInfo = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// key: clientid | key : underlying | value : EarlyPayIn Qunatity
        /// </summary>
        ConcurrentDictionary<string, ConcurrentDictionary<string, long>> dict_EarlyPayIn = new ConcurrentDictionary<string, ConcurrentDictionary<string, long>>();

        /// <summary>
        /// key: clientid | key : underlying | value : T-1 quantity
        /// </summary>
        ConcurrentDictionary<string, ConcurrentDictionary<string, long>> dict_T1positions = new ConcurrentDictionary<string, ConcurrentDictionary<string, long>>();

        /// <summary>
        /// key: clientid | key : underlying | value : T-2 quantity
        /// </summary>
        ConcurrentDictionary<string, ConcurrentDictionary<string, long>> dict_T2positions = new ConcurrentDictionary<string, ConcurrentDictionary<string, long>>();

        //added by nikhil
        /// <summary>
        /// key : USERNAME | KEY : Deposit value | used for finacial shortage
        /// </summary>
        ConcurrentDictionary<string, double> dict_Deposit = new ConcurrentDictionary<string, double>();

        /// <summary>
        /// Key : Code | Value : List of Limits.
        /// </summary>
        Dictionary<string, LimitInfo> dict_LimitInfo = new Dictionary<string, LimitInfo>();

        //Added by Akshay For Collateral
        /// <summary>
        /// Key : Client_Underlying | Value : Holdings
        /// </summary>
        ConcurrentDictionary<string, long> dict_ClientHoldings = new ConcurrentDictionary<string, long>();

        /// <summary>
        /// Key : Underlying  | Value : Haircut
        /// </summary>
        ConcurrentDictionary<string, double> dict_UnderlyingHaircut = new ConcurrentDictionary<string, double>();


        /// <summary>
        /// Contains all usernames present in DB.
        /// </summary>
        HashSet<string> hs_Usernames = new HashSet<string>();

        /// <summary>
        /// Contains all CashTokens present in DB.ContractMaster.
        /// </summary>
        HashSet<int> hs_CashTokens = new HashSet<int>();

        /// <summary>
        /// Contains all FOTokens present in DB.ContractMaster.
        /// </summary>
        HashSet<int> hs_FOTokens = new HashSet<int>();

        //Added by Akshay on 15-11-2021 for CD tokens
        /// <summary>
        /// Contains all CDTokens present in DB.ContractMaster.
        /// </summary>
        HashSet<int> hs_CDTokens = new HashSet<int>();

        //added by Omkar
        /// <summary>
        /// Contains all FOTokens present in DB.ContractMaster.
        /// </summary>
        HashSet<int> hs_BSEFOTokens = new HashSet<int>();

        /// <summary>
        /// Contains all banned Underlyings presenf in fo_secban.csv.
        /// </summary>
        HashSet<string> hs_BannedUnderlyings = new HashSet<string>();

        /// <summary>
        /// [0] UnderlyingToken, [1] UnderlyingSegment|Underlying
        /// </summary>
        List<string[]> list_AvgUnderlying = new List<string[]>();

        /// <summary>
        /// DataSource for gc_PrimeConnections.
        /// </summary>
        List<PrimeConnections> list_PrimeConnections = new List<PrimeConnections>();

        List<string> list_VaRMarginUpdateInterval = new List<string>();       //Added by Akshay on 31-12-2020 for storing time

        //added by nikhil 
        /// <summary>
        /// List of t-1 positions
        /// </summary>
        List<PositionInfo> list_T1positions = new List<PositionInfo>();

        //added by nikhil
        /// <summary>
        /// list t-2 positions
        /// </summary>
        List<PositionInfo> list_T2positions = new List<PositionInfo>();

        /// <summary>
        /// list of CM bhavcopy
        /// </summary>
        List<NSEUtilitaire.CMBhavcopy> list_CMBhavcopy = new List<NSEUtilitaire.CMBhavcopy>();

        int _VaRMarginUpdateCount = 0;                                       //Added by Akshay on 31-12-2020 for storing time

        int TradeCounts = 0;

        public EngineProcess()
        {
            try
            {
                InitializeComponent();
                _logger = new clsWriteLog(Application.StartupPath + "\\Log");//28-06-2018 for writting errorlog by navin
                _logger.DeleteOldLogs(Application.StartupPath + "\\Log");  //added by ninad on 22AUG2022 for auto-delete old log files

                this.Hide();

            }
            catch (Exception ee)
            {
                _logger.WriteLog("Engine CTor " + ee);
            }
        }
        
        #region Form Event

        private async void EngineProcess_Load(object sender, EventArgs e)
        {
            try
            {
                ReadLicense();

                //changed location on 04MAR2021 by Amey
                ReadConfig();

                //added on 08APR2021 by Amey
                GetIndexTokens("C://Prime//IndexTokens.csv");

                //Added by Akshay on 30-06-2021 for Closing price
                ReadClosingFile();

                try
                {
                    //changed on 07JAN2021 by Amey
                    _FeedLibrary.eve_FO7208TickReceived += ObjFeed_eve_FO7208;

                    //added on 07JAN2021 by Amey
                    _FeedLibrary.eve_CM7208TickReceived += ObjFeed_eve_CM7208;

                    _FeedLibrary.eve_7202TickReceived += ObjFeed_eve_7202;
                    _FeedLibrary.eve_ConnectionResponse += eve_ConnectionResponse;
                    _FeedLibrary.eve_PartialCandleReceived += ObjFeed_eve_PartialCandle;

                    //added on 22APR2021 by Amey
                    _FeedLibrary.eve_BSECMTickReceived += _FeedLibrary_eve_BSECMTickReceived;
                    _FeedLibrary.eve_CD7208TickReceived += _FeedLibrary_eve_CD7208TickReceived;

                    //added by Omkar
                    _FeedLibrary.eve_BSEFOTickReceived += _FeedLibrary_eve_BSEFOTickReceived;

                    //_FeedLibrary.eve_7202 += ObjFeed_eve_7202;
                    //_FeedLibrary.eve_CM7208 += ObjFeed_eve_CM7208;
                    //_FeedLibrary.eve_ConenctionMessage += eve_ConnectionResponse;
                    //_FeedLibrary.eve_FO7208 += ObjFeed_eve_FO7208;
                    //_FeedLibrary.eve_PartialCandle += ObjFeed_eve_PartialCandle;

                    //added on 04MAR2021 by Amey
                    var ipEndPoint_FeedFO = ds_Config.Tables["CONNECTION"].Rows[0]["FO-FEED"].ToString().Split(',');
                    var ipEndPoint_FeedCM = ds_Config.Tables["CONNECTION"].Rows[0]["CM-FEED"].ToString().Split(',');
                    var ipEndPoint_FeedCD = ds_Config.Tables["CONNECTION"].Rows[0]["CD-FEED"].ToString().Split(',');

                    //changed logic on 30APR2021 by Amey. To allow multiple connections to Multiple FeedReceivers.
                    for (int i = 0; i < ipEndPoint_FeedFO.Length; i++)
                    {
                        var _RemoteEndPoint = ipEndPoint_FeedFO[i].Split(':');

                        await Task.Run(() => _FeedLibrary.FOConnect("n.ENGINE", _RemoteEndPoint[0], _RemoteEndPoint[1]));
                    }

                    for (int i = 0; i < ipEndPoint_FeedCM.Length; i++)
                    {
                        var _RemoteEndPoint = ipEndPoint_FeedCM[i].Split(':');

                        await Task.Run(() => _FeedLibrary.CMConnect("n.ENGINE", _RemoteEndPoint[0], _RemoteEndPoint[1]));
                    }

                    for (int i = 0; i < ipEndPoint_FeedCD.Length; i++)
                    {
                        var _RemoteEndPoint = ipEndPoint_FeedCD[i].Split(':');

                        await Task.Run(() => _FeedLibrary.CDConnect("n.ENGINE", _RemoteEndPoint[0], _RemoteEndPoint[1]));
                    }

                    string[] ipEndPoint_FeedBSECM = new string[0];

                    if (ds_Config.Tables["CONNECTION"].Columns.Contains("BSECM-FEED"))
                    {
                        ipEndPoint_FeedBSECM = ds_Config.Tables["CONNECTION"].Rows[0]["BSECM-FEED"].ToString().Split(',');
                    }

                    //changed logic on 30APR2021 by Amey. To allow multiple connections to Multiple FeedReceivers.
                    if (ipEndPoint_FeedBSECM.Any())
                    {
                        for (int i = 0; i < ipEndPoint_FeedBSECM.Length; i++)
                        {
                            var _RemoteEndPoint = ipEndPoint_FeedBSECM[i].Split(':');

                            await Task.Run(() => _FeedLibrary.BSECMConnect("n.ENGINE", _RemoteEndPoint[0], _RemoteEndPoint[1]));
                        }
                    }

                    //added by Omkar
                    string[] ipEndPoint_FeedBSEFO = new string[0];

                    if (ds_Config.Tables["CONNECTION"].Columns.Contains("BSEFO-FEED"))
                    {
                        ipEndPoint_FeedBSEFO = ds_Config.Tables["CONNECTION"].Rows[0]["BSEFO-FEED"].ToString().Split(',');
                    }

                    //changed logic on 30APR2021 by Amey. To allow multiple connections to Multiple FeedReceivers.
                    if (ipEndPoint_FeedBSEFO.Any())
                    {
                        for (int i = 0; i < ipEndPoint_FeedBSEFO.Length; i++)
                        {
                            var _RemoteEndPoint = ipEndPoint_FeedBSEFO[i].Split(':');

                            await Task.Run(() => _FeedLibrary.BSEFOConnect("n.ENGINE", _RemoteEndPoint[0], _RemoteEndPoint[1]));
                        }
                    }

                    // Altered by Snehadri on 30JUN2021
                    btn_Start.Enabled = false;
                    btn_StartProcess.Enabled = false;
                    btn_EditUploadClient.Enabled = false;
                    btn_AddUser.Enabled = false; 

                    //added on 09APR2021 by Amey
                    btn_EODPositions.Enabled = false;
                    btn_UploadEarlyPayIn.Enabled = false;
                }
                catch (Exception feedEx)
                {
                    _logger.WriteLog("Feed connect " + feedEx.ToString());
                }

                // Commented by Snehadri on 305JUN2021
                ////TODO: Remove this bad code.
                //StartPage p = new StartPage(this);
                //p.ShowDialog();

                //if (p.dres == DialogResult.OK)
                //{
                //    _logger.WriteLog("Entered " + DateTime.Now);
                //    this.Show();
                //}
                //else
                //{
                //    _logger.WriteLog("Exit " + DateTime.Now);
                //    this.Close();
                //    Application.Exit();
                //}

                SkinManager.EnableFormSkins();
                SkinManager.EnableMdiFormSkins();

                try
                {
                    //added on 25MAR2021 by Amey
                    Addons._logger = _logger;
                    dict_NPLValues = Addons.ReadNPLFile();

                    //added on 07APR2021 by Amey
                    hs_BannedUnderlyings = Addons.ReadBanScripFile();

                    //added on 08APR2021 by Amey
                    dict_MTDValues = Addons.ReadMTDFile();

                    //Added by Akshay on 18-01-2022 for Reading Limits file
                    dict_LimitInfo = Addons.ReadLimitFile();

                    //added on 17FEB2021 by Amey
                    ReadClientDetail();

                    //changed location on 03FEB2021 by Amey
                    ReadInterestRate();

                    _EngineDataServer.eve_PrimeSpanConnected += _EngineDataServer_eve_PrimeSpanConnected;
                    _EngineDataServer.eve_PrimeTradeConnected += _EngineDataServer_eve_PrimeTradeConnected;

                    _EngineDataServer.SetupServer(ip_EngineServer, _EngineServerTradePORT, _EngineServerSpanPORT, _MaxAllowedInstances, Application.StartupPath,
                        _logger, _EngineHeartBeatServer, _TimeoutMilliseconds);                //added by Navin on 04-02-2020

                    _EngineHeartBeatServer.SetupServer(ip_EngineServer, _EngineServerHBPORT, _logger, _EngineDataServer, _TimeoutMilliseconds, _Version);

                    _GatewayHeartBeatClient.eve_Error += _WriteLog;
                    _GatewayHeartBeatClient.eve_GatewayStatusReceived += _GatewayHeartBeatClient_eve_GatewayStatusReceived;
                    _GatewayHeartBeatClient.eve_SpanStatusReceived += _GatewayHeartBeatClient_eve_SpanStatusReceived;
                    _GatewayHeartBeatClient.eve_LTTReceived += _GatewayHeartBeatClient_eve_LTTReceived;
                    _GatewayHeartBeatClient.eve_SpanInfoReceived += _GatewayHeartBeatClient_eve_SpanInfoReceived;
                    Task.Run(() => _GatewayHeartBeatClient.ConnectToGateway("n.ENGINE", ip_GatewayServer, _GatewayServerHBPORT));

                    _GatewayDataClient.eve_Error += _WriteLog;
                    _GatewayDataClient.eve_TradesReceived += ReceiveTradesFromGateway;
                    _GatewayDataClient.eve_SpanReceived += ReceiveSpanFromGateway;
                    Task.Run(() => _GatewayDataClient.ConnectToGatewayTrade("n.ENGINE", ip_GatewayServer, _GatewayServerTradePORT));
                    Task.Run(() => _GatewayDataClient.ConnectToGatewaySpan("n.ENGINE", ip_GatewayServer, _GatewayServerSpanPORT));


                    ReadContractMaster();

                    //Console.WriteLine($"{DateTime.Now} | ContractMaster End");
                }
                catch (Exception loadEx)
                {
                    _logger.WriteLog("Load " + loadEx.ToString());
                }

                //Console.WriteLine($"{DateTime.Now} | ContractAndFill Start");

                //changed to await on 19JAN2021 by Amey
                ReadContractAndFill();
                ReadCDContractAndFill();
                ReadBSEContractAndFill();

                //Added by Akshay for Collateral Value
                ReadHaircutFile();
                ReadHoldingFile();


                //Console.WriteLine($"{DateTime.Now} | ContractAndFill End");

                //changed to Task on 06NOV2020 by Amey
                Task.Run(() => EngineProcessLinq());

                //added on MAR2021 by Amey
                Task.Run(() => SetAvgIV());

                Task.Run(() => SetAvgIVForCDS());


                Task.Run(() =>SetAvgIVBSEFO());

                //added on 15MAR2021 by Amey
                gc_PrimeConnections.DataSource = list_PrimeConnections;
                gv_PrimeConnections.BestFitColumns();

                //Console.WriteLine($"{DateTime.Now} | Initialise End");

                AddToList("Engine initialised successfully");

                // Added by Snehadri on 30JUN2021 for Automatic BOD Process
                StartBODConnector();
            }
            catch (Exception ee)
            {
                _logger.WriteLog("Engine Load " + ee.ToString());
            }
        }

        // Added by Snehadri on 30JUN2021 for Automatic BOD Process
        private void StartBODConnector()
        {
            try
            {
                bool connected = bODUtilityConnector.StartClient();

                if (connected)
                {
                    checkTerms.CheckState = CheckState.Checked;
                    btn_Start.PerformClick();
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("StartBODConnector Error: " + ee.ToString());
            }
        }

        private void _EngineDataServer_eve_PrimeTradeConnected(string Username, IPAddress IP_Prime, bool isConnected)
        {
            try
            {
                PrimeConnections _PrimeInstance = null;
                lock (list_PrimeConnections)
                {
                    _PrimeInstance = list_PrimeConnections.Where(v => v.Username == Username).FirstOrDefault();

                    if (_PrimeInstance is null)
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            list_PrimeConnections.Add(new PrimeConnections() { Username = Username, IP = IP_Prime, IsTradeConnected = isConnected });
                            gv_PrimeConnections.RefreshData();
                        }));
                    }
                    else
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            //added on 30MAR2021 by Amey to update IP if connection is made from another location.
                            _PrimeInstance.IP = IP_Prime;

                            _PrimeInstance.IsTradeConnected = isConnected;
                            gv_PrimeConnections.RefreshData();
                        }));
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog("_EngineDataServer_eve_PrimeTradeConnected : " + ee); }
        }

        private void _EngineDataServer_eve_PrimeSpanConnected(string Username, IPAddress IP_Prime, bool isConnected)
        {
            try
            {
                PrimeConnections _PrimeInstance = null;
                lock (list_PrimeConnections)
                {
                    _PrimeInstance = list_PrimeConnections.Where(v => v.Username == Username).FirstOrDefault();

                    if (_PrimeInstance is null)
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            list_PrimeConnections.Add(new PrimeConnections() { Username = Username, IP = IP_Prime, IsSpanConnected = isConnected });
                            gv_PrimeConnections.RefreshData();
                        }));
                    }
                    else
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            //added on 30MAR2021 by Amey to update IP if connection is made from another location.
                            _PrimeInstance.IP = IP_Prime;

                            _PrimeInstance.IsSpanConnected = isConnected;
                            gv_PrimeConnections.RefreshData();
                        }));
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog("_EngineDataServer_eve_PrimeSpanConnected : " + ee); }
        }

        private void EngineProcess_Resize(object sender, EventArgs e)
        {
            try
            {
                if (FormWindowState.Minimized == this.WindowState)
                {
                    notifyIconEngineProcess.Visible = true;
                    this.Hide();
                }
                else
                {
                    notifyIconEngineProcess.Visible = false;
                    this.Show();
                }
            }
            catch (Exception eq)
            {
                _logger.WriteLog("Resize " + eq.ToString());
            }
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            try
            {
                //changed on 27JAN2021 by Amey
                Task.Factory.StartNew(() => _GatewayHeartBeatClient.ConnectToGateway("n.ENGINE", ip_GatewayServer, _GatewayServerHBPORT));

                //added on 30OCT2020 by Amey
                chkEdit_MarkToClosing.Enabled = false;

                chkEdit_Day1CM.Enabled = false;
                chkEdit_BODPS03.Enabled = false;
                chkEdit_ClearEOD.Enabled = false;

                //added on 29JAN2021 by Amey
                chkEdit_MarkToClosing.Checked = false;
                chkEdit_Day1CM.Checked = false;
                chkEdit_BODPS03.Checked = false;
                chkEdit_ClearEOD.Checked = false;

                btn_StartProcess.Enabled = false;

                //added on 09APR2021 by Amey
                btn_EODPositions.Enabled = true;

                AddToList("Engine process started.");
                _logger.WriteLog("Engine process started.", true);

                btn_Start.Enabled = false;

                //btn_EditUploadClient.Enabled = false;    //added by Navin on 15-10-2019
                //btn_AddUser.Enabled = false;             //added by Navin on 15-10-2019
                Application.DoEvents();

                ReadEODTable();

                //added by nikhil for t-1 and t-2
                //added by nikhil
                SelectClientfromDatabase();
                ReadCMBhavcopy();
                Read_T1Positions();
                Read_T2positions();

                //added on 06NOV2020 by Amey
                isEngineStarted = true;
                StartFromZero = true;

                btn_Stop.Enabled = true;

                //changed location on 06NOV2020 by Amey
                _GatewayHeartBeatClient.SendToGateway("SIGNAL^START");
            }
            catch (Exception ee)
            {
                _logger.WriteLog("btn_Start_Click : " + ee);
            }
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                _GatewayHeartBeatClient.SendToGateway("SIGNAL^STOP");

                Application.DoEvents();

                //added on 30OCT2020 by Amey
                chkEdit_MarkToClosing.Enabled = true;

                chkEdit_Day1CM.Enabled = true;
                chkEdit_BODPS03.Enabled = true;
                chkEdit_ClearEOD.Enabled = true;

                btn_Stop.Enabled = false;
                btn_Start.Enabled = true;

                btn_StartProcess.Enabled = true;

                //added on 09APR2021 by Amey
                btn_EODPositions.Enabled = true;

                btn_EditUploadClient.Enabled = true;    //added by Navin on 15-10-2019
                btn_AddUser.Enabled = true;             //added by Navin on 15-10-2019

                //added on 06NOV2020 by Amey
                isEngineStarted = false;

                AddToList("Engine process stopped.");
                _logger.WriteLog("Engine process stopped.", true);
            }
            catch (Exception ee)
            {
                chkEdit_MarkToClosing.Enabled = true;

                chkEdit_Day1CM.Enabled = true;
                chkEdit_BODPS03.Enabled = true;
                chkEdit_ClearEOD.Enabled = true;

                btn_Stop.Enabled = false;
                btn_Start.Enabled = true;

                btn_StartProcess.Enabled = true;

                //added on 09APR2021 by Amey
                btn_EODPositions.Enabled = true;

                AddToList("Engine process stopped.");
                _logger.WriteLog("Engine process stopped.", true);

                _logger.WriteLog("btn_Stop_Click : " + ee);
            }
        }

        private void btn_StartProcess_Click(object sender, EventArgs e)
        {
            try
            {
                btn_Start.Enabled = false;
                if (chkEdit_ClearEOD.Checked)
                {
                    try
                    {
                        AddToList("Deleting EOD data");
                        _logger.WriteLog("Deleting EOD data", true);

                        Application.DoEvents();
                        using (MySqlConnection myConnClear = new MySqlConnection(_MySQLCon))
                        {
                            using (MySqlCommand myCmdClear = new MySqlCommand("sp_ClearEOD", myConnClear))
                            {
                                myConnClear.Open();
                                myCmdClear.CommandType = CommandType.StoredProcedure;
                                myCmdClear.ExecuteNonQuery();
                                myConnClear.Close();
                            }
                        }

                        AddToList("EOD data deleted successfully.");
                        _logger.WriteLog("EOD data deleted successfully.", true);
                    }
                    catch (Exception cEodEx)
                    {
                        _logger.WriteLog("Clear EOD : " + cEodEx);
                    }

                    chkEdit_ClearEOD.Checked = false;
                }
                if (chkEdit_BODPS03.Checked)
                {
                    AddToList("PS03 Process started.");

                    SelectClientfromDatabase();

                    _logger.WriteLog("PS03 Process started.", true);

                    var _OpenFile = new OpenFileDialog();
                    if (_OpenFile.ShowDialog() == DialogResult.OK)
                    {
                        //Seperated class for reading Day1 Positions for better track code updates of various Prime versions. 09MAR2021-Amey
                        list_Day1Positions = Day1.ReadPS03(_OpenFile.FileName, _logger, chkEdit_MarkToClosing.Checked,
                            radioGroup_MarkToPrice.SelectedIndex == 0 ? true : false,
                            hs_Usernames, dict_ScripInfo, dict_CustomScripInfo, dict_TokenScripInfo);

                        //addded on 09APR2021 by Amey
                        if (Day1.isAnyError)
                            AddToList("Invalid data found in PS03 file. Please check if positions are uploaded properly.");

                        InsertDay1();
                    }

                    chkEdit_Day1CM.Checked = false;
                }
                if (chkEdit_Day1CM.Checked)
                {
                    AddToList("Day1 Process started.");

                    SelectClientfromDatabase();

                    _logger.WriteLog("Day1 Process started.", true);

                    //Seperated class for reading Day1 Positions for better track code updates of various Prime versions. 09MAR2021-Amey
                    list_Day1Positions = Day1.Read(_logger, chkEdit_MarkToClosing.Checked, radioGroup_MarkToPrice.SelectedIndex == 0 ? true : false,
                        hs_Usernames, dict_ScripInfo, dict_CustomScripInfo, dict_TokenScripInfo);

                    //addded on 09APR2021 by Amey
                    if (Day1.isAnyError)
                        AddToList("Invalid data found in Day1 file. Please check if positions are uploaded properly.");

                    InsertDay1();

                    chkEdit_Day1CM.Checked = false;
                }

                btn_Start.Enabled = true;
            }
            catch (Exception processEx)
            {
                _logger.WriteLog("Start process " + processEx);
            }
        }

        private void chkEdit_ClearEOD_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEdit_ClearEOD.Checked)
            {
                chkEdit_BODPS03.Checked = false;
                chkEdit_Day1CM.Checked = false;
                chkEdit_MarkToClosing.Checked = false;
            }
        }

        private void chkEdit_BODPS03_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEdit_BODPS03.Checked)
            {
                chkEdit_ClearEOD.Checked = false;
                chkEdit_Day1CM.Checked = false;
                chkEdit_MarkToClosing.Checked = true;
            }
        }

        private void chkEdit_Day1CM_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEdit_Day1CM.Checked)
            {
                chkEdit_ClearEOD.Checked = false;
                chkEdit_BODPS03.Checked = false;
                chkEdit_MarkToClosing.Checked = false;
            }
        }

        private void chkEdit_MarkToClosing_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkEdit_MarkToClosing.Checked)
            {
                chkEdit_BODPS03.Checked = false;
            }   
        }

        private void btn_EditUploadClient_Click(object sender, EventArgs e)
        {
            try
            {
                AddToList("Client upload Started..");

                BODProcess _BODProcess = new BODProcess(_MySQLCon, _logger);

                //added on 17FEB2021 by Amey
                _BODProcess.eve_RefreshClientDetail += ReadClientDetail;
                //added by Nikhil | runtime client update
                _BODProcess.eve_SendBodUpdateToGetway += SendBodUpdateToGetway;

                _BODProcess.ShowDialog();

                AddToList("Client upload Completed..");
            }
            catch (Exception bodEX)
            {
                _logger.WriteLog("btn_EditUploadClient_Click " + bodEX);
            }
        }

        //Added by Nikhil on 29DEC2021
        private void SendBodUpdateToGetway() 
        {
            try
            {
                _GatewayHeartBeatClient.SendToGateway("SIGNAL^UPDATE");
            }
            catch (Exception ee)
            {
                _logger.WriteLog(ee.StackTrace);
            }

            //SEND UPDATE TO NIMAGE B
            try
            {
                bool success = _nImageBHeartBeatClient.SendTonImageB("SIGNAL^UPDATE");
            }
            catch (Exception ee)
            {
                _logger.WriteLog(ee.StackTrace);
            }
        }

        private void btn_AddUser_Click(object sender, EventArgs e)
        {
            AddUser _AddUser = new AddUser(_MySQLCon);
            _AddUser.eve_Errorlog += ObjAddUser_eve_Errorlog;

            //added on 17FEB2021 by Amey
            _AddUser.eve_RefreshClientDetail += ReadClientDetail;

            _AddUser.ShowDialog();
        }

        private void notifyIconEngineProcess_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;

                //added on 27JAN2021 by Amey
                //notifyIconEngineProcess.Visible = false;
            }
            catch (Exception ee)
            {
                _logger.WriteLog("notifyIconEngineProcess_MouseClick : " + ee);
            }
        }

        private void notifyIconEngineProcess_Click(object sender, EventArgs e)
        {
            try
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;

                //added on 27JAN2021 by Amey
                //notifyIconEngineProcess.Visible = false;
            }
            catch (Exception er)
            {
                _logger.WriteLog("Notify_" + er.Message.ToString());
            }
        }

        private void gv_PrimeConnections_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            //try
            //{
            //    //added on 18NOV2020 by Amey. To avoid NullRefExceptions.
            //    if (e is null) return;

            //    var _temp = new PrimeConnections();
            //    if (e.Column.FieldName == nameof(_temp.Disconnect))
            //        e.RepositoryItem = gc_PrimeConnections.RepositoryItems["repBtn_Disconnect"];
            //}
            //catch (Exception ee) { _logger.WriteLog("gv_PrimeConnections_CustomRowCellEdit : " + ee); }
        }

        private void gv_PrimeConnections_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                //var _temp = new PrimeConnections();
                //if (gv_PrimeConnections.FocusedColumn.FieldName == nameof(_temp.Disconnect))
                //    e.Cancel = false;
                //else
                e.Cancel = true;
            }
            catch (Exception error)
            {
                _logger.WriteLog("gv_PrimeConnections_ShowingEditor " + error);
            }
        }

        private void repBtn_Disconnect_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //try
            //{
            //    var Username = gv_PrimeConnections.GetFocusedRowCellValue("Username").ToString().ToLower();

            //    if (_EngineDataServer.dict_ConnectedPrime.ContainsKey(Username) && _EngineDataServer.dict_ConnectedPrimeSpan.ContainsKey(Username))
            //    {
            //        _EngineDataServer.dict_ConnectedPrime[Username].Close();
            //        _EngineDataServer.dict_ConnectedPrimeSpan[Username].Close();

            //        _EngineDataServer.dict_ConnectedPrime.TryRemove(Username, out _);
            //        _EngineDataServer.dict_ConnectedPrimeSpan.TryRemove(Username, out _);

            //        lock (list_PrimeConnections)
            //        {
            //            var _PrimeInstance = list_PrimeConnections.Where(v => v.Username == Username).FirstOrDefault();

            //            if (_PrimeInstance is null) return;

            //            Invoke((MethodInvoker)(() =>
            //            {
            //                _PrimeInstance.IsTradeConnected = false;
            //                _PrimeInstance.IsSpanConnected = false;
            //            }));
            //        }
            //    }
            //}
            //catch (Exception ee) { _logger.WriteLog("repBtn_Disconnect_ButtonClick : " + ee); }
        }

        private void EngineProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (XtraMessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    //added on 06NOV2020 by Amey
                    isEngineStarted = false;

                    //added on 24FEB2021 by Amey
                    isEngineRunning = false;

                    //added on 29JAN2021 by Amey
                    _logger.WriteLog("EngineProcess_FormClosing");

                    Environment.Exit(0);
                }
                catch (Exception ee)
                {
                    _logger.WriteLog("EngineProcess_FormClosing : " + ee);
                }
            }
            else
                e.Cancel = true;
        }

        //Added by Snehadri on 15JUN2021
        private void checkTerms_CheckedChanged(object sender, EventArgs e)
        {
            btn_AddUser.Enabled = true;
            btn_EditUploadClient.Enabled = true;
            btn_EODPositions.Enabled = true;
            btn_Start.Enabled = true;
            btn_StartProcess.Enabled = true;
            btn_UploadEarlyPayIn.Enabled = true;
            checkTerms.Visible = false;
            lbl_Terms.Visible = false;
            btn_UploadLedger.Enabled = true;
        }

        //Added by Snehadri on 15JUN2021
        private void lbl_Terms_Click(object sender, EventArgs e)
        {
            TermsAndConditions term = new TermsAndConditions();
            term.ShowDialog(this);
        }

        #endregion

        #region Important Methods

        /// <summary>
        /// Reads contract.txt and fills required dictionaries.
        /// </summary>
        private void ReadContractAndFill()
        {
            try
            {
                var list_Contract = Exchange.ReadContract("C:\\Prime\\contract.txt", false).OrderBy(v => v.Expiry).ToList();
                var ClosestExpiries = list_Contract.Select(v => new { v.Symbol, v.Expiry }).GroupBy(g => new { g.Symbol }).Select(r => new { Underlying = r.Select(x => x.Symbol).First(), Expiry = r.Select(x => x.Expiry).First() }).ToList();
                var ClosestUnderlyingTokens = list_Contract.Where(v => v.ScripType == NSEUtilitaire.en_ScripType.XX).Select(v => new { v.Token, v.Symbol }).GroupBy(g => new { g.Symbol }).Select(r => new { Token = r.Select(x => x.Token).First(), Underlying = r.Select(x => x.Symbol).First() }).ToList();

                foreach (var item in ClosestUnderlyingTokens)
                {
                    try
                    {
                        var Special = list_Contract.Where(v => v.Symbol.Equals(item.Underlying)).ToList();
                        DateTime Expiry = ClosestExpiries.Where(v => v.Underlying.Equals(item.Underlying)).FirstOrDefault().Expiry;

                        //changed on 20APR2021 by Amey
                        var ScripKey = $"{en_Segment.NSEFO}|{item.Token}";
                        dict_UnderlyingToken.TryAdd($"{en_Segment.NSEFO}|{item.Underlying}", item.Token);
                        dict_XXLTP.TryAdd(ScripKey, new double[2] { -1, 0 });
                        dict_IVs.TryAdd(ScripKey, new double[2] { 0, 0 });

                        int[] arr_CEPETokens = new int[2] { 0, 0 };
                        var dict_Strikes = Special.Where(v => v.Symbol.Equals(item.Underlying) && v.Expiry.Equals(Expiry) && v.ScripType != NSEUtilitaire.en_ScripType.XX).Select(v => v.StrikePrice).Distinct().ToDictionary(k => k, v => arr_CEPETokens);

                        foreach (var Strike in dict_Strikes.Keys.ToList())
                        {
                            arr_CEPETokens = Special.OrderBy(v => v.ScripType).Where(v => v.Symbol.Equals(item.Underlying) && v.Expiry.Equals(Expiry) && v.StrikePrice.Equals(Strike)).Select(v => v.Token).ToArray();
                            dict_Strikes[Strike] = arr_CEPETokens;
                        }

                        //changed on 20APR2021 by Amey
                        dict_ClosestATMTokens.TryAdd(ScripKey, new Dictionary<DateTime, Dictionary<double, int[]>> { [Expiry] = dict_Strikes });
                    }
                    catch (Exception ee)
                    {
                        _logger.WriteLog("ReadContractAndFill Loop : " + ee.ToString());
                    }
                }

                //added call here on 19NOV2020 by Amey
                ReadIVFile();

                //added on 19NOV2020 by Amey
                ReadOGFile();

                //added on 11NOV2020 by Amey
                foreach (var item in list_Contract.Where(v => v.ScripType != NSEUtilitaire.en_ScripType.XX))
                {
                    //added on 19NOV2020 by Amey
                    var _greeks = new Greeks();

                    var UnderlyingKey = $"{en_Segment.NSEFO}|{item.Symbol}";
                    if (dict_DefaultIVs.TryGetValue(UnderlyingKey, out double[] arr_IVs))
                    {
                        _greeks.IV = arr_IVs[0];
                        _greeks.IVLower = arr_IVs[1];
                        _greeks.IVHigher = arr_IVs[2];

                        //added on 04FEB2021 by Amey
                        _greeks.IsReceived = true;
                    }

                    //changed on 20APR2021 by Amey
                    var ScripKey = $"{en_Segment.NSEFO}|{item.Token}";
                    dict_Greeks.TryAdd(ScripKey, _greeks);
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("ReadContractAndFill : " + ee.ToString());
            }
        }

        /// <summary>
        /// Reads contract.txt and fills required dictionaries.
        /// </summary>
        private void ReadCDContractAndFill()
        {
            try
            {
                var list_Contract = Exchange.ReadCDContract("C:\\Prime\\cd_contract.txt", false).OrderBy(v => v.Expiry).ToList();
                var ClosestExpiries = list_Contract.Select(v => new { v.Symbol, v.Expiry }).GroupBy(g => new { g.Symbol }).Select(r => new { Underlying = r.Select(x => x.Symbol).First(), Expiry = r.Select(x => x.Expiry).First() }).ToList();
                var ClosestUnderlyingTokens = list_Contract.Where(v => v.ScripType == NSEUtilitaire.en_ScripType.XX).Select(v => new { v.Token, v.Symbol }).GroupBy(g => new { g.Symbol }).Select(r => new { Token = r.Select(x => x.Token).First(), Underlying = r.Select(x => x.Symbol).First() }).ToList();

                foreach (var item in ClosestUnderlyingTokens)
                {
                    try
                    {
                        var Special = list_Contract.Where(v => v.Symbol.Equals(item.Underlying)).ToList();
                        DateTime Expiry = ClosestExpiries.Where(v => v.Underlying.Equals(item.Underlying)).FirstOrDefault().Expiry;

                        //changed on 20APR2021 by Amey
                        var ScripKey = $"{en_Segment.NSECD}|{item.Token}";
                        dict_UnderlyingToken.TryAdd($"{en_Segment.NSECD}|{item.Underlying}", item.Token);
                        dict_XXLTP.TryAdd(ScripKey, new double[2] { -1, 0 });
                        dict_IVs.TryAdd(ScripKey, new double[2] { 0, 0 });

                        int[] arr_CEPETokens = new int[2] { 0, 0 };
                        var dict_Strikes = Special.Where(v => v.Symbol.Equals(item.Underlying) && v.Expiry.Equals(Expiry) && v.ScripType != NSEUtilitaire.en_ScripType.XX).Select(v => v.StrikePrice).Distinct().ToDictionary(k => k, v => arr_CEPETokens);

                        foreach (var Strike in dict_Strikes.Keys.ToList())
                        {
                            arr_CEPETokens = Special.OrderBy(v => v.ScripType).Where(v => v.Symbol.Equals(item.Underlying) && v.Expiry.Equals(Expiry) && v.StrikePrice.Equals(Strike)).Select(v => v.Token).ToArray();
                            dict_Strikes[Strike] = arr_CEPETokens;
                        }

                        //changed on 20APR2021 by Amey
                        dict_ClosestATMTokens.TryAdd(ScripKey, new Dictionary<DateTime, Dictionary<double, int[]>> { [Expiry] = dict_Strikes });
                    }
                    catch (Exception ee)
                    {
                        _logger.WriteLog("ReadCDContractAndFill Loop : " + ee.ToString());
                    }
                }

                //added call here on 19NOV2020 by Amey
                ReadIVFile();

                //added on 19NOV2020 by Amey
                ReadOGFile();

                //added on 11NOV2020 by Amey
                foreach (var item in list_Contract.Where(v => v.ScripType != NSEUtilitaire.en_ScripType.XX))
                {
                    //added on 19NOV2020 by Amey
                    var _greeks = new Greeks();

                    var UnderlyingKey = $"{en_Segment.NSECD}|{item.Symbol}";
                    if (dict_DefaultIVs.TryGetValue(UnderlyingKey, out double[] arr_IVs))
                    {
                        _greeks.IV = arr_IVs[0];
                        _greeks.IVLower = arr_IVs[1];
                        _greeks.IVHigher = arr_IVs[2];

                        //added on 04FEB2021 by Amey
                        _greeks.IsReceived = true;
                    }

                    ////changed on 20APR2021 by Amey
                    //var ScripKey = $"{en_Segment.NSEFO}|{item.Token}";
                    //dict_Greeks.TryAdd(ScripKey, _greeks);

                    //changed on 20APR2021 by Amey
                    var ScripKey = $"{en_Segment.NSECD}|{item.Token}";
                    dict_Greeks.TryAdd(ScripKey, _greeks);
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("ReadCDContractAndFill : " + ee.ToString());
            }
        }

        //added by Omkar
        private void ReadBSEContractAndFill()//Checked this function
        {
            try
            {
                //DirectoryInfo _PrimeDirectory = new DirectoryInfo("C:\\Prime\\");
                //var list_Contract = _PrimeDirectory.GetFiles("BSE_EQD_CONTRACT_*.csv").OrderByDescending(v => v.LastWriteTime).ToList();

                //var list_Contract = Exchange.ReadBSEFOContract("C:\\Prime\\BSE_EQD_CONTRACT_17072023.csv").OrderBy(v => v.Expiry).ToList();
                // expiry today //3:30 // 12:00 -> match
                // day before expuiy  // 29 jul 3:30 // 28july 12:00 -> match 
                // day after expiry   // 29 july 3:20 // 30 july 12 -> mismatch 

                DirectoryInfo _PrimeDirectory = new DirectoryInfo("C:\\Prime\\");
                var BSEFOSecurity = _PrimeDirectory.GetFiles("BSE_EQD_CONTRACT_*.csv").OrderByDescending(v => v.LastWriteTime).ToList();
                var list_Contract = Exchange.ReadBSEFOContract(BSEFOSecurity[0].FullName).Where(x=>x.Expiry >= DateTime.Today).OrderBy(v => v.Expiry).ToList();


                var ClosestExpiries = list_Contract.Select(v => new { v.Symbol, v.Expiry }).GroupBy(g => new { g.Symbol }).Select(r => new { Underlying = r.Select(x => x.Symbol).First(), Expiry = r.Select(x => x.Expiry).First() }).ToList();
                var ClosestUnderlyingTokens = list_Contract.Where(v => v.ScripType == NSEUtilitaire.en_ScripType.XX).Select(v => new { v.Token, v.Symbol }).GroupBy(g => new { g.Symbol }).Select(r => new { Token = r.Select(x => x.Token).First(), Underlying = r.Select(x => x.Symbol).First() }).ToList();

                foreach (var item in ClosestUnderlyingTokens)
                {
                    try
                    {
                        var Special = list_Contract.Where(v => v.Symbol.Equals(item.Underlying)).ToList();
                        DateTime Expiry = ClosestExpiries.Where(v => v.Underlying.Equals(item.Underlying)).FirstOrDefault().Expiry;

                        //changed on 20APR2021 by Amey
                        var ScripKey = $"{en_Segment.BSEFO}|{item.Token}";
                        dict_UnderlyingToken.TryAdd($"{en_Segment.BSEFO}|{item.Underlying}", item.Token);
                        dict_XXLTP.TryAdd(ScripKey, new double[2] { -1, 0 });
                        dict_IVs.TryAdd(ScripKey, new double[2] { 0, 0 });

                        int[] arr_CEPETokens = new int[2] { 0, 0 };
                        var dict_Strikes = Special.Where(v => v.Symbol.Equals(item.Underlying) && v.Expiry.Equals(Expiry) && v.ScripType != NSEUtilitaire.en_ScripType.XX).Select(v => v.StrikePrice).Distinct().ToDictionary(k => k, v => arr_CEPETokens);

                        foreach (var Strike in dict_Strikes.Keys.ToList())
                        {
                            arr_CEPETokens = Special.OrderBy(v => v.ScripType).Where(v => v.Symbol.Equals(item.Underlying) && v.Expiry.Equals(Expiry) && v.StrikePrice.Equals(Strike)).Select(v => v.Token).ToArray();
                            dict_Strikes[Strike] = arr_CEPETokens;
                        }

                        //changed on 20APR2021 by Amey
                        dict_ClosestATMTokens.TryAdd(ScripKey, new Dictionary<DateTime, Dictionary<double, int[]>> { [Expiry] = dict_Strikes });
                    }
                    catch (Exception ee)
                    {
                        _logger.WriteLog("ReadContractAndFill Loop : " + ee.ToString());
                    }
                }

                //added call here on 19NOV2020 by Amey
                ReadIVFile();

                //added on 19NOV2020 by Amey
                ReadOGFile();

                //added on 11NOV2020 by Amey
                foreach (var item in list_Contract.Where(v => v.ScripType != NSEUtilitaire.en_ScripType.XX))
                {
                    //added on 19NOV2020 by Amey
                    var _greeks = new Greeks();

                    var UnderlyingKey = $"{en_Segment.BSEFO}|{item.Symbol}";
                    if (dict_DefaultIVs.TryGetValue(UnderlyingKey, out double[] arr_IVs))
                    {
                        _greeks.IV = arr_IVs[0];
                        _greeks.IVLower = arr_IVs[1];
                        _greeks.IVHigher = arr_IVs[2];

                        //added on 04FEB2021 by Amey
                        _greeks.IsReceived = true;
                    }

                    //changed on 20APR2021 by Amey
                    var ScripKey = $"{en_Segment.BSEFO}|{item.Token}";
                    dict_Greeks.TryAdd(ScripKey, _greeks);
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("ReadContractAndFill : " + ee.ToString());
            }
        }



        private void SetAvgIV()
        {
            try
            {
                bool isIVUpdated = false;

                double XXLTP = -1;
                double CELTP = -1;
                double PELTP = -1;

                int[] Tokens = new int[2] { 0, 0 };
                double closestStrike = 0;
                bool isTokenPresent = true;

                while (isEngineRunning)
                {
                    double IV = 0;
                    isTokenPresent = true;

                    try
                    {
                        //added on 04MAR2021 by Amey
                        var list_AvgUnderlyingCpy = new List<string[]>(list_AvgUnderlying);

                        foreach (var arr_UnderlyingInfo in list_AvgUnderlyingCpy)
                        {
                            isIVUpdated = false;

                            XXLTP = -1;
                            CELTP = -1;
                            PELTP = -1;

                            Tokens = new int[2] { 0, 0 };
                            closestStrike = 0;
                            isTokenPresent = true;

                            int UnderlyingToken = Convert.ToInt32(arr_UnderlyingInfo[0]);
                            string UnderlyingKey = arr_UnderlyingInfo[1];

                            //added on 20APR2021 by Amey
                            var UnderlyingTokenKey = $"{en_Segment.NSEFO}|{UnderlyingToken}";
                            //Added on 7OCT2020 by Amey.
                            if (dict_ClosestATMTokens.TryGetValue(UnderlyingTokenKey, out Dictionary<DateTime, Dictionary<double, int[]>> dict)
                                && dict_XXLTP[UnderlyingTokenKey][0] != -1)
                            {
                                double LTP = dict_XXLTP[UnderlyingTokenKey][0];
                                DateTime Expiry = dict.ElementAt(0).Key;

                                if (LTP != XXLTP)
                                {
                                    XXLTP = LTP;

                                    closestStrike = dict[Expiry].Keys.Aggregate((accumulator, currentVal) => Math.Abs(accumulator - LTP) < Math.Abs(currentVal - LTP) ? accumulator : currentVal);
                                    Tokens = dict[Expiry][closestStrike];
                                }

                                //added on 15OCT2020 by Amey
                                if (Tokens[0] == 0 || Tokens[1] == 0) continue;

                                //added on 20APR2021 by Amey
                                var LTPTokenKey1 = $"{en_Segment.NSEFO}|{Tokens[0]}";
                                var LTPTokenKey2 = $"{en_Segment.NSEFO}|{Tokens[1]}";

                                double Time = (Expiry - DateTime.Now).TotalDays * 6.25;
                                TimeSpan ts = TimeSpan.Parse("15:30:00") - DateTime.Now.TimeOfDay;
                                if (ts.TotalSeconds > 0)
                                    Time = (Time + (ts.TotalSeconds / 3600)) / 6.25;
                                else
                                    Time = Time / 6.25;

                                double[] arr_CELTP;
                                double[] arr_PELTP;

                                if (dict_LTP.TryGetValue(LTPTokenKey1, out arr_CELTP) && CELTP != arr_CELTP[0])
                                {
                                    CELTP = arr_CELTP[0];

                                    IV = ImpliedCallVolatility(LTP, closestStrike, Time / 365, 0, arr_CELTP[0], 0, 0) * 100;
                                    dict_IVs[UnderlyingTokenKey][0] = IV > 0.1 ? (IV < 150 ? IV : 0) : 0;

                                    isIVUpdated = true;
                                }
                                else
                                {
                                    dict_LTP.TryAdd(LTPTokenKey1, new double[LTPArray] { -1, -1, 0 });

                                    _FeedLibrary.FOSubscribe(Tokens[0]);

                                    isTokenPresent = false;
                                }

                                if (dict_LTP.TryGetValue(LTPTokenKey2, out arr_PELTP) && PELTP != arr_PELTP[0])
                                {
                                    PELTP = arr_PELTP[0];

                                    IV = ImpliedPutVolatility(LTP, closestStrike, Time / 365, 0, arr_PELTP[0], 0, 0) * 100;
                                    dict_IVs[UnderlyingTokenKey][1] = IV > 0.1 ? (IV < 150 ? IV : 0) : 0;

                                    isIVUpdated = true;
                                }
                                else
                                {
                                    //changed on 03FEB2021 by Amey
                                    dict_LTP.TryAdd(LTPTokenKey2, new double[LTPArray] { -1, -1, 0 });

                                    _FeedLibrary.FOSubscribe(Tokens[1]);

                                    isTokenPresent = false;
                                }

                                if (!isTokenPresent) continue;

                                if (isIVUpdated && dict_IVs[UnderlyingTokenKey][0] != 0 && dict_IVs[UnderlyingTokenKey][1] != 0)
                                {
                                    double[] arr_IV = new double[2] { Math.Round((dict_IVs[UnderlyingTokenKey][0] + dict_IVs[UnderlyingTokenKey][1]) / 2, 2), 0 };
                                    //added check on 22OCT2020 by Amey
                                    if (dict_ComputedATMIV.ContainsKey(UnderlyingKey))
                                        dict_ComputedATMIV[UnderlyingKey][0] = arr_IV[0];
                                    else
                                        dict_ComputedATMIV.TryAdd(UnderlyingKey, arr_IV);

                                    //changed on 03FEB2021 by Amey
                                    //added on 15OCT2020 by Amey
                                    if (arr_CELTP[2] == 0 || arr_PELTP[2] == 0 || dict_XXLTP[UnderlyingTokenKey][1] == 0)
                                        dict_ComputedATMIV[UnderlyingKey][1] = 0;
                                    else
                                        dict_ComputedATMIV[UnderlyingKey][1] = 1;
                                }
                            }
                        }
                    }
                    catch (Exception ee) { _logger.WriteLog("GetAvgIV Loop : " + ee.ToString()); }

                    Thread.Sleep(200);
                }
            }
            catch (Exception ee) { _logger.WriteLog("GetAvgIV : " + ee.ToString()); }
        }


        private void SetAvgIVBSEFO()
        {
            try
            {
                bool isIVUpdated = false;

                double XXLTP = -1;
                double CELTP = -1;
                double PELTP = -1;

                int[] Tokens = new int[2] { 0, 0 };
                double closestStrike = 0;
                bool isTokenPresent = true;

                while (isEngineRunning)
                {
                    double IV = 0;
                    isTokenPresent = true;

                    try
                    {
                        //added on 04MAR2021 by Amey
                        var list_AvgUnderlyingCpy = new List<string[]>(list_AvgUnderlying);

                        foreach (var arr_UnderlyingInfo in list_AvgUnderlyingCpy)
                        {
                            isIVUpdated = false;

                            XXLTP = -1;
                            CELTP = -1;
                            PELTP = -1;

                            Tokens = new int[2] { 0, 0 };
                            closestStrike = 0;
                            isTokenPresent = true;

                            int UnderlyingToken = Convert.ToInt32(arr_UnderlyingInfo[0]);
                            string UnderlyingKey = arr_UnderlyingInfo[1];

                            //added on 20APR2021 by Amey
                            var UnderlyingTokenKey = $"{en_Segment.BSEFO}|{UnderlyingToken}";
                            //Added on 7OCT2020 by Amey.
                            
                            if (dict_ClosestATMTokens.TryGetValue(UnderlyingTokenKey, out Dictionary<DateTime, Dictionary<double, int[]>> dict)
                                && dict_XXLTP[UnderlyingTokenKey][0] != -1)
                            {
                                double LTP = dict_XXLTP[UnderlyingTokenKey][0];
                                DateTime Expiry = dict.ElementAt(0).Key;

                                if (LTP != XXLTP)
                                {
                                    XXLTP = LTP;

                                    closestStrike = dict[Expiry].Keys.Aggregate((accumulator, currentVal) => Math.Abs(accumulator - LTP) < Math.Abs(currentVal - LTP) ? accumulator : currentVal);
                                    Tokens = dict[Expiry][closestStrike];
                                }

                                //added on 15OCT2020 by Amey
                                if (Tokens[0] == 0 || Tokens[1] == 0) continue;

                                //added on 20APR2021 by Amey
                                var LTPTokenKey1 = $"{en_Segment.BSEFO}|{Tokens[0]}";
                                var LTPTokenKey2 = $"{en_Segment.BSEFO}|{Tokens[1]}";

                                double Time = (Expiry - DateTime.Now).TotalDays * 6.25;
                                TimeSpan ts = TimeSpan.Parse("15:30:00") - DateTime.Now.TimeOfDay;
                                if (ts.TotalSeconds > 0)
                                    Time = (Time + (ts.TotalSeconds / 3600)) / 6.25;
                                else
                                    Time = Time / 6.25;

                                double[] arr_CELTP;
                                double[] arr_PELTP;

                                if (dict_LTP.TryGetValue(LTPTokenKey1, out arr_CELTP) && CELTP != arr_CELTP[0])
                                {
                                    CELTP = arr_CELTP[0];

                                    IV = ImpliedCallVolatility(LTP, closestStrike, Time / 365, 0, arr_CELTP[0], 0, 0) * 100;
                                    dict_IVs[UnderlyingTokenKey][0] = IV > 0.1 ? (IV < 150 ? IV : 0) : 0;

                                    isIVUpdated = true;
                                }
                                else
                                {
                                    dict_LTP.TryAdd(LTPTokenKey1, new double[LTPArray] { -1, -1, 0 });

                                    _FeedLibrary.BSEFOSubscribe(Tokens[0]);

                                    isTokenPresent = false;
                                }

                                if (dict_LTP.TryGetValue(LTPTokenKey2, out arr_PELTP) && PELTP != arr_PELTP[0])
                                {
                                    PELTP = arr_PELTP[0];

                                    IV = ImpliedPutVolatility(LTP, closestStrike, Time / 365, 0, arr_PELTP[0], 0, 0) * 100;
                                    dict_IVs[UnderlyingTokenKey][1] = IV > 0.1 ? (IV < 150 ? IV : 0) : 0;

                                    isIVUpdated = true;
                                }
                                else
                                {
                                    //changed on 03FEB2021 by Amey
                                    dict_LTP.TryAdd(LTPTokenKey2, new double[LTPArray] { -1, -1, 0 });

                                    _FeedLibrary.BSEFOSubscribe(Tokens[1]);

                                    isTokenPresent = false;
                                }

                                if (!isTokenPresent) continue;

                                
                                if (isIVUpdated && dict_IVs[UnderlyingTokenKey][0] != 0 && dict_IVs[UnderlyingTokenKey][1] != 0)
                                {
                                    double[] arr_IV = new double[2] { Math.Round((dict_IVs[UnderlyingTokenKey][0] + dict_IVs[UnderlyingTokenKey][1]) / 2, 2), 0 };
                                    //added check on 22OCT2020 by Amey
                                    if (dict_ComputedATMIV.ContainsKey(UnderlyingKey))
                                        dict_ComputedATMIV[UnderlyingKey][0] = arr_IV[0];
                                    else
                                        dict_ComputedATMIV.TryAdd(UnderlyingKey, arr_IV);

                                    //changed on 03FEB2021 by Amey
                                    //added on 15OCT2020 by Amey
                                    if (arr_CELTP[2] == 0 || arr_PELTP[2] == 0 || dict_XXLTP[UnderlyingTokenKey][1] == 0)
                                        dict_ComputedATMIV[UnderlyingKey][1] = 0;
                                    else
                                        dict_ComputedATMIV[UnderlyingKey][1] = 1;
                                }
                            }
                        }
                    }
                    catch (Exception ee) { _logger.WriteLog("GetAvgIV Loop : " + ee.ToString()); }

                    Thread.Sleep(200);
                }
            }
            catch (Exception ee) { _logger.WriteLog("GetAvgIV : " + ee.ToString()); }
        }


        private void SetAvgIVForCDS()
        {
            try
            {
                bool isIVUpdated = false;

                double XXLTP = -1;
                double CELTP = -1;
                double PELTP = -1;

                int[] Tokens = new int[2] { 0, 0 };
                double closestStrike = 0;
                bool isTokenPresent = true;

                while (isEngineRunning)
                {
                    double IV = 0;
                    isTokenPresent = true;

                    try
                    {
                        //added on 04MAR2021 by Amey
                        var list_AvgUnderlyingCpy = new List<string[]>(list_AvgUnderlying);

                        foreach (var arr_UnderlyingInfo in list_AvgUnderlyingCpy)
                        {
                            isIVUpdated = false;

                            XXLTP = -1;
                            CELTP = -1;
                            PELTP = -1;

                            Tokens = new int[2] { 0, 0 };
                            closestStrike = 0;
                            isTokenPresent = true;

                            int UnderlyingToken = Convert.ToInt32(arr_UnderlyingInfo[0]);
                            string UnderlyingKey = arr_UnderlyingInfo[1];

                            //added on 20APR2021 by Amey
                            var UnderlyingTokenKey = $"{en_Segment.NSECD}|{UnderlyingToken}";
                            //Added on 7OCT2020 by Amey.
                            if (dict_ClosestATMTokens.TryGetValue(UnderlyingTokenKey, out Dictionary<DateTime, Dictionary<double, int[]>> dict)
                                && dict_XXLTP[UnderlyingTokenKey][0] != -1)
                            {
                                double LTP = dict_XXLTP[UnderlyingTokenKey][0];
                                DateTime Expiry = dict.ElementAt(0).Key;

                                if (LTP != XXLTP)
                                {
                                    XXLTP = LTP;

                                    closestStrike = dict[Expiry].Keys.Aggregate((accumulator, currentVal) => Math.Abs(accumulator - LTP) < Math.Abs(currentVal - LTP) ? accumulator : currentVal);
                                    Tokens = dict[Expiry][closestStrike];
                                }

                                //added on 15OCT2020 by Amey
                                if (Tokens[0] == 0 || Tokens[1] == 0) continue;

                                //added on 20APR2021 by Amey
                                var LTPTokenKey1 = $"{en_Segment.NSECD}|{Tokens[0]}";
                                var LTPTokenKey2 = $"{en_Segment.NSECD}|{Tokens[1]}";

                                double Time = (Expiry - DateTime.Now).TotalDays * 6.25;
                                TimeSpan ts = TimeSpan.Parse("12:00:00") - DateTime.Now.TimeOfDay;
                                if (ts.TotalSeconds > 0)
                                    Time = (Time + (ts.TotalSeconds / 3600)) / 6.25;
                                else
                                    Time = Time / 6.25;

                                double[] arr_CELTP;
                                double[] arr_PELTP;

                                if (dict_LTP.TryGetValue(LTPTokenKey1, out arr_CELTP) && CELTP != arr_CELTP[0])
                                {
                                    CELTP = arr_CELTP[0];

                                    IV = ImpliedCallVolatility(LTP, closestStrike, Time / 365, 0, arr_CELTP[0], 0, 0) * 100;
                                    dict_IVs[UnderlyingTokenKey][0] = IV > 0.1 ? (IV < 150 ? IV : 0) : 0;

                                    isIVUpdated = true;
                                }
                                else
                                {
                                    dict_LTP.TryAdd(LTPTokenKey1, new double[LTPArray] { -1, -1, 0 });

                                    _FeedLibrary.CDSubscribe(Tokens[0]);

                                    isTokenPresent = false;
                                }

                                if (dict_LTP.TryGetValue(LTPTokenKey2, out arr_PELTP) && PELTP != arr_PELTP[0])
                                {
                                    PELTP = arr_PELTP[0];

                                    IV = ImpliedPutVolatility(LTP, closestStrike, Time / 365, 0, arr_PELTP[0], 0, 0) * 100;
                                    dict_IVs[UnderlyingTokenKey][1] = IV > 0.1 ? (IV < 150 ? IV : 0) : 0;

                                    isIVUpdated = true;
                                }
                                else
                                {
                                    //changed on 03FEB2021 by Amey
                                    dict_LTP.TryAdd(LTPTokenKey2, new double[LTPArray] { -1, -1, 0 });

                                    _FeedLibrary.CDSubscribe(Tokens[1]);

                                    isTokenPresent = false;
                                }

                                if (!isTokenPresent) continue;

                                if (isIVUpdated && dict_IVs[UnderlyingTokenKey][0] != 0 && dict_IVs[UnderlyingTokenKey][1] != 0)
                                {
                                    double[] arr_IV = new double[2] { Math.Round((dict_IVs[UnderlyingTokenKey][0] + dict_IVs[UnderlyingTokenKey][1]) / 2, 2), 0 };
                                    //added check on 22OCT2020 by Amey
                                    if (dict_ComputedATMIV.ContainsKey(UnderlyingKey))
                                        dict_ComputedATMIV[UnderlyingKey][0] = arr_IV[0];
                                    else
                                        dict_ComputedATMIV.TryAdd(UnderlyingKey, arr_IV);

                                    //changed on 03FEB2021 by Amey
                                    //added on 15OCT2020 by Amey
                                    if (arr_CELTP[2] == 0 || arr_PELTP[2] == 0 || dict_XXLTP[UnderlyingTokenKey][1] == 0)
                                        dict_ComputedATMIV[UnderlyingKey][1] = 0;
                                    else
                                        dict_ComputedATMIV[UnderlyingKey][1] = 1;
                                }
                            }
                        }
                    }
                    catch (Exception ee) { _logger.WriteLog("GetAvgIV Loop : " + ee.ToString()); }

                    Thread.Sleep(200);
                }
            }
            catch (Exception ee) { _logger.WriteLog("GetAvgIV : " + ee.ToString()); }
        }
        private void ReceiveTradesFromGateway(List<PositionInfo> list_ReceivedTrades)
        {
            try
            {
                //changed on 05JAN2021 by Amey
                lock (_TradeLock)
                {
                    //dt_AllTradesNew.Merge((DataTable)JsonConvert.DeserializeObject(_data.Substring(_data.IndexOf('_') + 1), dt_AllTradesNew.GetType()));

                    //list_AllTrades = list_AllTrades.Union(JsonConvert.DeserializeObject<List<PositionInfo>>(_data.Substring(_data.IndexOf('_') + 1))).ToList();
                    list_AllTrades.AddRange(list_ReceivedTrades);

                    //added on 05JAN2021 by Amey
                    isNewTrades = true;
                }

                TradeCounts += list_ReceivedTrades.Count;
                //added on 04MAY2021 by Amey
                if (isDebug)
                {
                    _logger.WriteLog("Trades Received," + list_AllTrades.Count, isDebug);
                    _logger.WriteLog("Trades Count Received from Gateway," + TradeCounts, isDebug);
                }
                    
            }
            catch (Exception error)
            {
                _logger.WriteLog("Exception occurred while receiving trades from socket at " + DateTime.Now + Environment.NewLine + error.ToString());
            }
        }

        private void ReceiveSpanFromGateway(string flag, Dictionary<string, double[]> dict_Span)
        {
            try
            {
                #region Old-Code
                //if (flag == "ALL")
                //{
                //    dict_SpanMargin = dict_Span;

                //    //added on 18NOV2020 by Amey
                //    if (isDebug)
                //        _logger.WriteLog("Span Received Rows," + dict_SpanMargin.Count(), true);

                //}
                ////Added by Akshay on 10-12-2020 for Expiry Span Margin
                //else if (flag == "EXPIRY")
                //{
                //    dict_ExpirySpanMargin = dict_Span;

                //    //added on 18NOV2020 by Amey
                //    if (isDebug)
                //        _logger.WriteLog("Expiry Span Received Rows," + dict_ExpirySpanMargin.Count(), isDebug);
                //}                   
                //else if (flag == "EOD")
                //{
                //    dict_EODMargin = dict_Span;

                //    //added on 18NOV2020 by Amey
                //    if (isDebug)
                //        _logger.WriteLog("EOD Span Received Rows," + dict_EODMargin.Count(), isDebug);
                //}
                ////CDS
                //else if (flag == "CDSALL")
                //{
                //    dict_CDSSpanMargin = dict_Span;

                //    if (isDebug)
                //        _logger.WriteLog("CDS Span Received Rows," + dict_CDSSpanMargin.Count(), isDebug);
                //}
                //else if (flag == "CDSEXPIRY")
                //{
                //    dict_CDSExpirySpanMargin = dict_Span;
                //}

                //else if (flag == "CDSEOD")
                //    dict_CDSEODMargin = dict_Span;
                #endregion

                //Changed by Snehadri on 15May2023
                switch (flag)
                {
                    case "ALL":
                        dict_SpanMargin = dict_Span;
                        _logger.WriteLog("Span Received Rows," + dict_SpanMargin.Count(), true);
                        break;
                    case "EXPIRY":
                        dict_ExpirySpanMargin = dict_Span;
                        _logger.WriteLog("Expiry Span Received Rows," + dict_ExpirySpanMargin.Count(), true);
                        break;
                    case "EOD":
                        dict_EODMargin = dict_Span;
                        _logger.WriteLog("Expiry Span Received Rows," + dict_ExpirySpanMargin.Count(), true);
                        break;
                    case "CDSALL":
                        dict_CDSSpanMargin = dict_Span;
                        _logger.WriteLog("CDSALL Span Received Rows," + dict_CDSSpanMargin.Count(), true);
                        break;
                    case "CDSEXPIRY":
                        dict_CDSExpirySpanMargin = dict_Span;
                        _logger.WriteLog("CDSEXPIRY Span Received Rows," + dict_CDSExpirySpanMargin.Count(), true);
                        break;
                    case "CDSEOD":
                        dict_CDSEODMargin = dict_Span;
                        _logger.WriteLog("CDSEOD Span Received Rows," + dict_CDSEODMargin.Count(), true);
                        break;
                }

            }
            catch (Exception error)
            {
                _logger.WriteLog("Exception occurred while receiving margin from socket at " + DateTime.Now + Environment.NewLine + error.ToString());
            }
        }

        private void EngineProcessLinq()   //Main Engine Processes
        {
            try
            {
                while (isEngineRunning)
                {
                    var stopwatch = new Stopwatch();

                    //added on 06NOV2020 by Amey
                    while (isEngineStarted)
                    {
                        stopwatch.Start();

                        //Added by Akshay on 31-12-2020 For VAREQ read
                        if (_VaRMarginUpdateCount < list_VaRMarginUpdateInterval.Count)
                            ReadVAR_EQ(list_VaRMarginUpdateInterval[_VaRMarginUpdateCount]);

                        //_logger.WriteLog("Consolidate Start : " + DateTime.Now.ToString("ss:fff"), true);

                        ConsolidateAndSend();

                        stopwatch.Stop();
                        var elapsed_time = stopwatch.ElapsedMilliseconds;

                        stopwatch.Reset();

                        //changed to 800 from 400 on 11JUN2021 by Amey
                        int waittime = 800;
                        try
                        {
                            waittime -= Convert.ToInt32(elapsed_time);
                            waittime = waittime < 0 ? 0 : waittime;
                        }
                        catch (OverflowException) { }
                        catch (Exception) { }

                        Thread.Sleep(waittime);

                        //added on 16DEC2020 by Amey
                        //Thread.Sleep(50);

                        //_logger.WriteLog("Consolidate End : " + DateTime.Now.ToString("ss:fff"), true);
                    }

                    Thread.Sleep(250);
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("EngineProcessLinq : " + ee);
            }
        }

        private void ReadEODTable()
        {
            try
            {
                list_EODPositions.Clear();

                var dt_EOD = new ds_Engine.dt_EODPositionsDataTable();
                using (MySqlConnection myConnEod = new MySqlConnection(_MySQLCon))
                {
                    MySqlCommand myCmdEod = new MySqlCommand("sp_GetEOD", myConnEod);
                    myCmdEod.CommandType = CommandType.StoredProcedure;
                    myConnEod.Open();
                    MySqlDataAdapter dadapter = new MySqlDataAdapter(myCmdEod);

                    dadapter.Fill(dt_EOD);

                    dadapter.Dispose();
                    myConnEod.Close();
                }

                //changed on 20APR2021 by Amey
                //added on 10FEB2021 by Amey
                list_EODPositions = dt_EOD.AsEnumerable().Select(v => new PositionInfo
                {
                    Username = v.Username,
                    Segment = v.Segment == "NSECM" ? en_Segment.NSECM : (v.Segment == "NSECD" ? en_Segment.NSECD :
                        (v.Segment == "NSEFO" ? en_Segment.NSEFO : (v.Segment == "BSEFO" ? en_Segment.BSEFO : en_Segment.BSECM))),
                    Token = v.Token,
                    TradePrice = v.TradePrice,
                    TradeQuantity = v.TradeQuantity,

                    //added on 10JUN2021 by Amey
                    TradeValue = v.TradePrice * v.TradeQuantity,

                    UnderlyingSegment = v.UnderlyingSegment == "NSECM" ? en_Segment.NSECM : (v.Segment == "NSECD" ? en_Segment.NSECD :
                        (v.Segment == "NSEFO" ? en_Segment.NSEFO : (v.Segment == "BSEFO" ? en_Segment.BSEFO : en_Segment.BSECM))),
                    UnderlyingToken = v.UnderlyingToken
                }).ToList();

                //added on 10OCT2020 by Amey
                for (int i = 0; i < list_EODPositions.Count; i++)
                {
                    //changed on 10FEB2021 by Amey
                    var _PositionInfo = list_EODPositions[i];

                    //changed Key on 20APR2021 by Amey
                    dict_CFInfo.TryAdd(_PositionInfo.Username + "_" + $"{_PositionInfo.Segment}|{_PositionInfo.Token}",
                        new double[2] { _PositionInfo.TradeQuantity, _PositionInfo.TradePrice });
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("ReadEODTable : " + ee);
            }
        }

        private void ConsolidateAndSend()
        {
            try
            {
                if (StartFromZero || isNewTrades)
                {
                    //_logger.WriteLog("Merge Start : " + DateTime.Now.ToString("ss:fff"), true);

                    //changed on 06NOV2020 by Amey
                    if (StartFromZero)
                    {
                        if (isDebug)
                            _logger.WriteLog("EOD," + list_EODPositions.Count, isDebug);

                        //changed on 10FEB2021 by Amey
                        list_AllTrades.Clear();
                        list_AllTrades.AddRange(list_EODPositions);

                        // Added on 01JAN2022 by Nikhil for t - 1 positions
 
                         list_AllTrades.AddRange(list_T1positions);
                         list_AllTrades.AddRange(list_T2positions);

                         //added on 06NOV2020 by Amey
                         StartFromZero = false;

                        if (isDebug)
                            _logger.WriteLog("StartFromZero," + list_AllTrades.Count, isDebug);
                    }
                    else
                        isNewTrades = false;

                    if (isDebug)
                        _logger.WriteLog("Before Consolidate," + list_AllTrades.Count, isDebug);

                    #region Commented on 10JUN2021 by Amey. Logic changed.
                    //added on 05JAN2021 by Amey
                    //lock (_TradeLock)
                    //{
                    //    //changed on 10FEB2021 by Amey
                    //    dict_ConsolidatedPos.Clear();
                    //    list_CurrentPositions.Clear();
                    //    list_CurrentPositions = new List<PositionInfo>(list_AllTrades);
                    //}

                    ////added Segment on 20APR2021 by Amey. To avoid same Token conflict from different segments.
                    //var result = list_CurrentPositions.GroupBy(s => new { s.Username, s.Token, s.Segment })
                    //                    .Select(g => new
                    //                    {
                    //                        Username = g.Select(x => x.Username).First(),
                    //                        Segment = g.Select(x => x.Segment).First(),
                    //                        Token = g.Select(x => x.Token).First(),
                    //                        NetPosition = g.Sum(x => x.TradeQuantity),
                    //                        NetValue = g.Sum(x => x.TradeQuantity * x.TradePrice),
                    //                        IntradayNetPosition = g.Sum(x => x.IntradayQuantity),
                    //                        IntradayNetValue = g.Sum(x => x.IntradayValue),
                    //                        UnderlyingSegment = g.Select(x => x.UnderlyingSegment).First(),
                    //                        UnderlyingToken = g.Select(x => x.UnderlyingToken).First(),
                    //                        BuyQuantity = g.Sum(x => x.BuyQuantity),
                    //                        BuyValue = g.Sum(x => x.BuyValue),
                    //                        SellQuantity = g.Sum(x => x.SellQuantity),
                    //                        SellValue = g.Sum(x => x.SellValue),
                    //                    }); 
                    #endregion

                    lock (_TradeLock)
                    {
                        //changed on 10FEB2021 by Amey
                        dict_ConsolidatedPos.Clear();
                        dict_ClientWiseVARMargin.Clear();

                        list_AllTrades = list_AllTrades.GroupBy(s => new { s.Username, s.Token, s.Segment })
                                        .Select(g => new PositionInfo()
                                        {
                                            Username = g.Select(x => x.Username).First(),
                                            Segment = g.Select(x => x.Segment).First(),
                                            Token = g.Select(x => x.Token).First(),
                                            TradeQuantity = g.Sum(x => x.TradeQuantity),
                                            TradePrice = g.Sum(x => x.TradePrice),
                                            TradeValue = g.Sum(x => x.TradeValue),
                                            IntradayQuantity = g.Sum(x => x.IntradayQuantity),
                                            IntradayValue = g.Sum(x => x.IntradayValue),
                                            UnderlyingSegment = g.Select(x => x.UnderlyingSegment).First(),
                                            UnderlyingToken = g.Select(x => x.UnderlyingToken).First(),
                                            BuyQuantity = g.Sum(x => x.BuyQuantity),
                                            BuyValue = g.Sum(x => x.BuyValue),
                                            SellQuantity = g.Sum(x => x.SellQuantity),
                                            SellValue = g.Sum(x => x.SellValue),
                                        }).ToList();

                        list_CurrentPositions = new List<PositionInfo>(list_AllTrades);
                    }

                    //ds_Engine.ConsolidateTradeinfoRow dr_Trades;
                    ConsolidatedPositionInfo _PositionInfo;

                    //added on 20NOV2020 by Amey
                    //var ConsolidatedTradesCount = result.Count();

                    foreach (var Position in list_CurrentPositions)
                    {
                        var scripkeylogs = "";
                        try
                        {
                            _PositionInfo = new ConsolidatedPositionInfo();

                            //added on 20NOV2020 by Amey
                            if (!dict_ConsolidatedPos.ContainsKey(Position.Username))
                                dict_ConsolidatedPos.Add(Position.Username, new List<ConsolidatedPositionInfo>());

                            if (!dict_ClientWiseVARMargin.ContainsKey(Position.Username))
                                dict_ClientWiseVARMargin.TryAdd(Position.Username, 0.0);

                            //added on 20APR2021 by Amey

                            var ScripKey = $"{Position.Segment}|{Position.Token}";
                            scripkeylogs = ScripKey;
                                  
                            var ScripInfo = dict_TokenScripInfo[ScripKey];

                            _PositionInfo.Username = Position.Username;

                            //added on 20APR2021 by Amey
                            _PositionInfo.Segment = Position.Segment;
                            _PositionInfo.Series = ScripInfo.Series;

                            _PositionInfo.ScripName = ScripInfo.ScripName;
                            _PositionInfo.Token = Position.Token;
                            _PositionInfo.Expiry = ScripInfo.ExpiryUnix;
                            _PositionInfo.Underlying = ScripInfo.Symbol;

                            _PositionInfo.ScripType = ScripInfo.ScripType;
                            _PositionInfo.InstrumentName = ScripInfo.InstrumentName;
                            _PositionInfo.NetPosition = Position.TradeQuantity;

                            //changed on 20APR2021 by Amey
                            _PositionInfo.StrikePrice = ScripInfo.StrikePrice;

                            //if (isDebug)
                            //    _logger.WriteLog("Client," + Position.Username + ",Scripname," + ScripInfo.ScripName + ",NetPos," + Position.NetPosition, isDebug);

                            //added on 10OCT2020 by Amey
                            string EODKey = Position.Username + "_" + $"{Position.Segment}|{Position.Token}";
                            if (dict_CFInfo.TryGetValue(EODKey, out double[] arr_CFInfo))
                            {
                                _PositionInfo.NetPositionCF = Convert.ToInt64(arr_CFInfo[0]);
                                _PositionInfo.PriceCF = arr_CFInfo[1];
                            }

                            //changed logic to calculate BEP on 07APR2021 by Amey
                            _PositionInfo.BEP = Position.TradeValue / (Position.TradeQuantity == 0 ? -1 : Position.TradeQuantity);

                            //added on 23NOV2020 by Amey
                            _PositionInfo.IntradayBEP = Position.IntradayValue / (Position.IntradayQuantity == 0 ? -1 : Position.IntradayQuantity);

                            //added EQ check on 12APR2021 by Amey
                            //added on 07APR2021 by Amey for Banned Scrip feature.
                            if (hs_BannedUnderlyings.Contains(ScripInfo.Symbol) && _PositionInfo.InstrumentName != en_InstrumentName.EQ)
                            {
                                bool isBanned = false;
                                if (_PositionInfo.NetPositionCF > 0)
                                {
                                    if ((_PositionInfo.NetPosition > _PositionInfo.NetPositionCF) || (_PositionInfo.NetPosition < 0))
                                        isBanned = true;
                                }
                                else if (_PositionInfo.NetPositionCF < 0)
                                {
                                    if ((_PositionInfo.NetPosition < _PositionInfo.NetPositionCF) || (_PositionInfo.NetPosition > 0))
                                        isBanned = true;
                                }
                                else
                                {
                                    if (_PositionInfo.NetPosition != 0)
                                        isBanned = true;
                                }

                                var _BanKey = $"{_PositionInfo.Username}^{_PositionInfo.Token}";

                                //added ContainsKey check on 16JUN2021 by Amey. To update values once its added.
                                if (isBanned || dict_BanInfo.ContainsKey(_BanKey))
                                {
                                    //changed key on 7JUN2021 by Amey
                                    if (!dict_BanInfo.ContainsKey(_BanKey))
                                    {
                                        dict_BanInfo.Add(_BanKey, new BanInfo()
                                        {
                                            ClientID = Position.Username,
                                            Scrip = ScripInfo.ScripName,
                                            IntradayBuyQty = Position.BuyQuantity,
                                            IntradaySellQty = Position.SellQuantity,
                                            IntradayNetPos = Position.IntradayQuantity,
                                            NetPosCF = _PositionInfo.NetPositionCF
                                        });
                                    }
                                    else
                                    {
                                        var _BanInfo = dict_BanInfo[_BanKey];
                                        _BanInfo.IntradayBuyQty = Position.BuyQuantity;
                                        _BanInfo.IntradaySellQty = Position.SellQuantity;
                                        _BanInfo.IntradayNetPos = Position.IntradayQuantity;
                                    }
                                }
                            }

                            //added on 07APR2021 by Amey
                            _PositionInfo.IntradayBuyQuantity = Position.BuyQuantity;
                            _PositionInfo.IntradayBuyAvg = Position.BuyQuantity == 0 ? 0 : Position.BuyValue / Position.BuyQuantity;
                            _PositionInfo.IntradaySellQuantity = Position.SellQuantity;
                            _PositionInfo.IntradaySellAvg = Position.SellQuantity == 0 ? 0 : Position.SellValue / Position.SellQuantity;

                            _PositionInfo.IntradayNetPosition = Position.IntradayQuantity;//added by Navin on 27-05-2019

                            //added on 19NOV2020 by Amey
                            AssignLTPs(_PositionInfo);

                            //changedtoday
                            double BetaValue = 1;
                            if (!dict_BetaValue.TryGetValue(_PositionInfo.Underlying, out BetaValue))
                                BetaValue = BetaValue == 0 ? 1 : BetaValue;

                            AssignGreeks(_PositionInfo, BetaValue);

                            //start
                            // Added on 01JAN2022 by Nikhil for t - 1 t - 2 positions
                            long T1Quantity = 0;
                            long T2Quantity = 0;
                            long EpnQuantity = 0;
                            long TQuantity = _PositionInfo.IntradayNetPosition;

                            if (_PositionInfo.ScripType == n.Structs.en_ScripType.EQ)
                            {

                                if (dict_T1positions.TryGetValue(_PositionInfo.Username, out ConcurrentDictionary<string, long> dict_ClientWiseT1Position))
                                {
                                    if (dict_ClientWiseT1Position.TryGetValue(_PositionInfo.Underlying, out long _T1Quantity))
                                    {
                                        T1Quantity = _T1Quantity;
                                    }
                                }
                                if (dict_T2positions.TryGetValue(_PositionInfo.Username, out ConcurrentDictionary<string, long> dict_ClientWiseT2Position))
                                {
                                    if (dict_ClientWiseT2Position.TryGetValue(_PositionInfo.Underlying, out long _T2Quantity))
                                    {
                                        T2Quantity = _T2Quantity;
                                    }
                                }
                                if (dict_EarlyPayIn.TryGetValue(_PositionInfo.Username, out ConcurrentDictionary<string, long> dict_ClientWiseEPN))
                                {
                                    if (dict_ClientWiseEPN.TryGetValue(_PositionInfo.Underlying, out long _EpnQuantity))
                                    {
                                        EpnQuantity = _EpnQuantity;
                                    }
                                }
                            }

                            _PositionInfo.T1Quantity = T1Quantity;
                            _PositionInfo.T2Quantity = T2Quantity;
                            _PositionInfo.EarlyPayIn = EpnQuantity;

                            double VARMARGIN = 0;
                            //Added by Akshay on 31-12-2020 For VAREQ
                            if (_PositionInfo.ScripType == n.Structs.en_ScripType.EQ && dict_VARMargin.TryGetValue(_PositionInfo.ScripName, out VARMARGIN))
                            {
                                //  _PositionInfo.VARMargin = Math.Abs(_PositionInfo.IntradayNetPosition * _PositionInfo.LTP * (VARMARGIN / 100));
                                long netQuantity = T1Quantity + T2Quantity + TQuantity;
                                if (netQuantity < 0 && EpnQuantity > 0)
                                {
                                    netQuantity = netQuantity + EpnQuantity;
                                    if (netQuantity > 0)
                                    {
                                        netQuantity = 0;
                                    }
                                    _PositionInfo.VARMargin = Math.Abs(netQuantity * _PositionInfo.LTP * (VARMARGIN / 100));
                                }
                                else
                                {
                                    _PositionInfo.VARMargin = Math.Abs(netQuantity * _PositionInfo.LTP * (VARMARGIN / 100));
                                }

                                dict_ClientWiseVARMargin[Position.Username] += _PositionInfo.VARMargin;
                            }

                            //added on 25MAY2021 by Amey. To fetch EQ VaR margin for every position.
                            var _VaRMarginKey = $"{_PositionInfo.Underlying}-EQ";
                            //Added by Akshay on 22-03-2021 
                            if (dict_VARMargin.TryGetValue(_VaRMarginKey, out VARMARGIN))
                                _PositionInfo.UnderlyingVARMargin = Math.Abs(VARMARGIN / 100);

                            //Added by Akshay on 30-06-2021 for Closing Price
                            double ClosingPrice = 0;
                            if (dict_ScripClosing.TryGetValue(_PositionInfo.Underlying, out ClosingPrice))
                                _PositionInfo.ClosingPrice = ClosingPrice;

                            //added on 20NOV2020 by Amey
                            dict_ConsolidatedPos[_PositionInfo.Username].Add(_PositionInfo);
                        }
                        catch (Exception innerLinq)
                        {
                            _logger.WriteLog("Inner Linq Loop : " + innerLinq.ToString() + "| " + scripkeylogs );
                        }
                    }

                    //_logger.WriteLog("Merge End : " + DateTime.Now.ToString("ss:fff"), true);
                }
                else
                {
                    //added on 04MAY2021 by Amey
                    if (isDebug)
                        _logger.WriteLog("Before Loop," + dict_ConsolidatedPos.Keys.Count, isDebug);

                    dict_ClientWiseVARMargin.Clear();

                    foreach (var ClientID in dict_ConsolidatedPos.Keys)
                    {

                        if (!dict_ClientWiseVARMargin.ContainsKey(ClientID))
                            dict_ClientWiseVARMargin.TryAdd(ClientID, 0.0);

                        foreach (var _PositionInfo in dict_ConsolidatedPos[ClientID])
                        {
                            //added on 19NOV2020 by Amey
                            AssignLTPs(_PositionInfo); 

                            double BetaValue = 1;
                            if (!dict_BetaValue.TryGetValue(_PositionInfo.Underlying, out BetaValue))
                                BetaValue = BetaValue == 0 ? 1 : BetaValue;

                            AssignGreeks(_PositionInfo, BetaValue);
                            
                            //Added on 01JAN2022 by Nikhil for t-1 positions
                            long T1Quantity = 0;
                            long T2Quantity = 0;
                            long EpnQuantity = 0;
                            long TQuantity = _PositionInfo.IntradayNetPosition;

                            if (_PositionInfo.ScripType == n.Structs.en_ScripType.EQ)
                            {

                                if (dict_T1positions.TryGetValue(_PositionInfo.Username, out ConcurrentDictionary<string, long> dict_ClientWiseT1Position))
                                {
                                    if (dict_ClientWiseT1Position.TryGetValue(_PositionInfo.Underlying, out long _T1Quantity))
                                    {
                                        T1Quantity = _T1Quantity;
                                    }
                                }
                                if (dict_T2positions.TryGetValue(_PositionInfo.Username, out ConcurrentDictionary<string, long> dict_ClientWiseT2Position))
                                {
                                    if (dict_ClientWiseT2Position.TryGetValue(_PositionInfo.Underlying, out long _T2Quantity))
                                    {
                                        T2Quantity = _T2Quantity;
                                    }
                                }

                                if (dict_EarlyPayIn.TryGetValue(_PositionInfo.Username, out ConcurrentDictionary<string, long> dict_ClientWiseEPN))
                                {
                                    if (dict_ClientWiseEPN.TryGetValue(_PositionInfo.Underlying, out long _EpnQuantity))
                                    {
                                        EpnQuantity = _EpnQuantity;
                                    }
                                }
                            }

                            _PositionInfo.T1Quantity = T1Quantity;
                            _PositionInfo.T2Quantity = T2Quantity;
                            _PositionInfo.EarlyPayIn = EpnQuantity;
                            //end

                            double VARMARGIN = 0;
                            //Added by Akshay on 31-12-2020 For VAREQ
                            if (_PositionInfo.ScripType == n.Structs.en_ScripType.EQ && dict_VARMargin.TryGetValue(_PositionInfo.ScripName, out VARMARGIN))
                            {
                                //_PositionInfo.VARMargin = Math.Abs(_PositionInfo.IntradayNetPosition * _PositionInfo.LTP * (VARMARGIN / 100));
                                long netQuantity = T1Quantity + T2Quantity + TQuantity;
                                if (netQuantity < 0 && EpnQuantity > 0)
                                {
                                    netQuantity = netQuantity + EpnQuantity;
                                    if (netQuantity > 0)
                                    {
                                        netQuantity = 0;
                                    }
                                    _PositionInfo.VARMargin = Math.Abs(netQuantity * _PositionInfo.LTP * (VARMARGIN / 100));
                                }
                                else
                                {
                                    _PositionInfo.VARMargin = Math.Abs(netQuantity * _PositionInfo.LTP * (VARMARGIN / 100));
                                }

                                dict_ClientWiseVARMargin[ClientID] += _PositionInfo.VARMargin;
                            }

                            //added on 25MAY2021 by Amey. To fetch EQ VaR margin for every position.
                            var _VaRMarginKey = $"{_PositionInfo.Underlying}-EQ";
                            //Added by Akshay on 22-03-2021 
                            if (dict_VARMargin.TryGetValue(_VaRMarginKey, out VARMARGIN))
                                _PositionInfo.UnderlyingVARMargin = Math.Abs(VARMARGIN / 100);

                            if (_PositionInfo.Segment == en_Segment.NSECM)
                            {
                                //Added by Akshay For Collateral
                                var HoldingKey = $"{_PositionInfo.Username}-{_PositionInfo.Underlying}";

                                if (dict_ClientHoldings.TryGetValue(HoldingKey, out var _Holdings))
                                {
                                    _PositionInfo.CollateralQty = _Holdings;
                                    _PositionInfo.CollateralValue = Math.Round(_Holdings * _PositionInfo.LTP, 2);
                                }

                                if (dict_UnderlyingHaircut.TryGetValue(_PositionInfo.Underlying, out var _Haircut))
                                    _PositionInfo.CollateralHaircut = _PositionInfo.CollateralValue * (100 - _Haircut) / 100;
                            }
                        }
                    }

                    //_logger.WriteLog("Loop End : " + DateTime.Now.ToString("ss:fff"), true);
                }

                if (!IsPeakMarginComputing)
                    Task.Run(() => CalculatePeakMargin(new Dictionary<string, double[]>(dict_SpanMargin), new Dictionary<string, double>(dict_ClientWiseVARMargin)));

                if(!IsCDSPeakMarginComputing)
                    Task.Run(() => CalculateCDSPeakMargin(new Dictionary<string, double[]>(dict_CDSSpanMargin)));

                SendDataToPrime();

                //clearing here because TimeSpan chaneges each second. Can use same TimeSpan for same Token in same Package. 01APR2021-Amey
                dict_TokenExpiryTimeSpan.Clear();
            }
            catch (InvalidCastException ee)
            {
                _logger.WriteLog("Linq Invalid Data : " + JsonConvert.SerializeObject(list_CurrentPositions) + Environment.NewLine + ee);
            }
            catch (Exception linqEx)
            {
                _logger.WriteLog("Linq : " + linqEx);
            }
        }

        private void AssignLTPs(ConsolidatedPositionInfo _PositionInfo)
        {
            try
            {
                    //added on 20APR2021 by Amey
                    var ScripKey = $"{_PositionInfo.Segment}|{_PositionInfo.Token}";
                    var ScripInfo = dict_TokenScripInfo[ScripKey];
                    //CDS
                    _PositionInfo.LotSize = ScripInfo.LotSize;

                    string Underlying = _PositionInfo.Underlying;
                    var _ScripType = _PositionInfo.ScripType;

                    //changed on 20APR2021 by Amey
                    var UnderlyingNameKey = $"{_PositionInfo.Segment}|{Underlying}";
                    //added on 7OCT2020 by Amey
                    if (dict_UnderlyingToken.TryGetValue(UnderlyingNameKey, out int CurrentMonthUnderlyingToken))
                    {
                        //changed on 20APR2021 by Amey
                        var CurrentMonthUnderlyingKey = $"{_PositionInfo.Segment}|{CurrentMonthUnderlyingToken}";
                        if (!dict_LTP.ContainsKey(CurrentMonthUnderlyingKey))
                        {
                            //changed on 03FEB2021 by Amey
                            dict_LTP.TryAdd(CurrentMonthUnderlyingKey, new double[LTPArray] { -1, -1, 0 });
                        
                            if (_PositionInfo.Segment == en_Segment.NSEFO)
                            {
                                _FeedLibrary.FOSubscribe(CurrentMonthUnderlyingToken);
                            }else if(_PositionInfo.Segment == en_Segment.BSEFO)
                            {
                                _FeedLibrary.BSEFOSubscribe(CurrentMonthUnderlyingToken);
                            }
                            else if(_PositionInfo.Segment == en_Segment.NSECD)
                            {
                                _FeedLibrary.CDSubscribe(CurrentMonthUnderlyingToken);
                            }

                            list_AvgUnderlying.Add(new string[2] { CurrentMonthUnderlyingToken.ToString(), UnderlyingNameKey });
                        }
                    }

                    int ScripToken = _PositionInfo.Token;

                    double[] arr_LTP;
                    //added else condition on 18NOV2020 by Amey
                    if (dict_LTP.TryGetValue(ScripKey, out arr_LTP))
                    {
                        if (arr_LTP[0] != -1)
                        {
                            _PositionInfo.LTP = arr_LTP[0];

                            //Added by Akshay on 06-12-2021 for CDS LTP
                            if (_PositionInfo.Segment == en_Segment.NSECD && arr_LTP[2] == 1)
                                _PositionInfo.LTP = Math.Round(arr_LTP[0] / ScripInfo.LotSize, 4);
                            else
                                _PositionInfo.LTP = Math.Round(arr_LTP[0], 4);

                            _PositionInfo.IsLTPCalculated = false;
                        }

                        //if (_PositionInfo.ScripType == en_ScripType.CE || _PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.Segment != en_Segment.NSECD)
                        //{
                        //    _PositionInfo.UnderlyingLTP = arr_LTP[3];
                        //}

                    }
                    else
                    {
                        if (hs_CDTokens.Contains(ScripToken) && _PositionInfo.Segment == en_Segment.NSECD)
                            _FeedLibrary.CDSubscribe(ScripToken);

                        //added to Send subscribe request to particular FeedReceiver. 09MAR2021-Amey
                        if (hs_CashTokens.Contains(ScripToken))
                            _FeedLibrary.CMSubscribe(ScripToken);
                        else if (hs_FOTokens.Contains(ScripToken))
                            _FeedLibrary.FOSubscribe(ScripToken);
                        else if (hs_BSEFOTokens.Contains(ScripToken))
                            _FeedLibrary.BSEFOSubscribe(ScripToken);//added by Omkar
                        else
                            _FeedLibrary.BSECMSubscribe(ScripToken);


                        //changed on 03FEB2021 by Amey
                        dict_LTP.TryAdd(ScripKey, new double[LTPArray] { -1, -1, 0 });
                    }

                    //added on 20APR2021 by Amey
                    var UnderlyingKey = $"{ScripInfo.UnderlyingSegment}|{ScripInfo.UnderlyingToken}";
                    if (dict_LTP.TryGetValue(UnderlyingKey, out arr_LTP))
                    {
                        if (arr_LTP[0] != -1)
                        {
                            //changed location inside (else if (dict_LTP[UnderlyingKey][0] != -1)) block on 23APR2021 by Amey
                            //added ScripType condition on 01MAR2021 by Amey
                            //changed condition on 28JAN2021 by Amey
                            //added on 19NOV2020 by Amey
                            if (hs_CashTokens.Contains(ScripInfo.UnderlyingToken) && _ScripType != n.Structs.en_ScripType.EQ)
                                _PositionInfo.UnderlyingLTP = ((arr_LTP[0] * _InterestRate * _PositionInfo.ExpiryTimeSpan.TotalDays) / 36500) + arr_LTP[0];
                            else
                                _PositionInfo.UnderlyingLTP = arr_LTP[0];

                            //Added by Akshay on 06-12-2021 for CDS LTP
                            if (_PositionInfo.Segment == en_Segment.NSECD && arr_LTP[2] == 1)
                                _PositionInfo.UnderlyingLTP = Math.Round(arr_LTP[0] / ScripInfo.LotSize, 4); //_PositionInfo.UnderlyingLTP = Math.Round(arr_LTP[0] / ScripInfo.LotSize, 4);
                            else if (_PositionInfo.Segment == en_Segment.NSECD)
                                _PositionInfo.UnderlyingLTP = Math.Round(arr_LTP[0], 4);

                        }
                    }
                    else
                    {
                        if (hs_CDTokens.Contains(ScripInfo.UnderlyingToken) && _PositionInfo.Segment == en_Segment.NSECD)
                            _FeedLibrary.CDSubscribe(ScripInfo.UnderlyingToken);

                        //added to Send subscribe request to particular FeedReceiver. 09MAR2021-Amey
                        //added on 26FEB2021 by Amey
                        if (hs_CashTokens.Contains(ScripInfo.UnderlyingToken))
                            _FeedLibrary.CMSubscribe(ScripInfo.UnderlyingToken);
                        else if (hs_FOTokens.Contains(ScripInfo.UnderlyingToken))
                            _FeedLibrary.FOSubscribe(ScripInfo.UnderlyingToken);
                        else if (hs_BSEFOTokens.Contains(ScripToken))
                            _FeedLibrary.BSEFOSubscribe(ScripToken);//added by Omkar
                        else
                            _FeedLibrary.BSECMSubscribe(ScripInfo.UnderlyingToken);

                        //changed on 03FEB2021 by Amey
                        dict_LTP.TryAdd(UnderlyingKey, new double[LTPArray] { -1, -1, 0 });
                    }

                    if (dict_TokenExpiryTimeSpan.TryGetValue(ScripKey, out TimeSpan _TimeSpan))
                        _PositionInfo.ExpiryTimeSpan = _TimeSpan;
                    else
                    {
                        //changed location on 03DEC2020 by Amey
                        var TimeToExpiry = ConvertFromUnixTimestamp(_PositionInfo.Expiry).AddHours(15).AddMinutes(30) - DateTime.Now;

                        //Added by Akshay on 28-12-2021 for CDS
                        if (_PositionInfo.Segment == en_Segment.NSECD)
                            TimeToExpiry = ConvertFromUnixTimestamp(_PositionInfo.Expiry).AddHours(12) - DateTime.Now;

                        dict_TokenExpiryTimeSpan.Add(ScripKey, TimeToExpiry);
                        _PositionInfo.ExpiryTimeSpan = TimeToExpiry;
                    }

                    //changed on 20APR2021 by Amey
                    //added on 08APR2021 by Amey
                    if (dict_SpotTokenInfo.TryGetValue(Underlying, out string _SpotToken))
                    {
                        if (dict_LTP.TryGetValue(_SpotToken, out arr_LTP))
                        {
                            if (arr_LTP[0] != -1)
                                _PositionInfo.SpotPrice = dict_LTP[_SpotToken][0];
                        }
                        else
                        {
                            _FeedLibrary.CMSubscribe(Convert.ToInt32(_SpotToken.Split('|')[1]));

                            dict_LTP.TryAdd(_SpotToken, new double[LTPArray] { -1, -1, 0 });
                        }
                    }

                    //changed condition on 21MAY2021 by Amey
                    //added OPTIONS check on 01DEC2020 by Amey
                    if (_ScripType == n.Structs.en_ScripType.CE || _ScripType == n.Structs.en_ScripType.PE)
                    {
                        if (dict_ComputedATMIV.TryGetValue(UnderlyingNameKey, out double[] arr_IV))
                        {
                            double UnderlyingLTP = _PositionInfo.UnderlyingLTP;
                            double StrikePrice = _PositionInfo.StrikePrice;
                            double TheoreticalPrice = _PositionInfo.TheoreticalPrice;

                            _PositionInfo.ATM_IV = arr_IV[0];

                            //changed on 13APR2021 by Amey
                            if (_ScripType == n.Structs.en_ScripType.CE)
                                TheoreticalPrice = CallOption(UnderlyingLTP, StrikePrice, _PositionInfo.ExpiryTimeSpan.TotalDays / 365, 0, arr_IV[0] / 100, 0);
                            else if (_ScripType == n.Structs.en_ScripType.PE)
                                TheoreticalPrice = PutOption(UnderlyingLTP, StrikePrice, _PositionInfo.ExpiryTimeSpan.TotalDays / 365, 0, arr_IV[0] / 100, 0);

                            //added isNaN check on 06APR2021 by Amey
                            TheoreticalPrice = double.IsNaN(TheoreticalPrice) ? 0 : TheoreticalPrice;

                            //added on 15OCT2020 by Amey
                            //assigning min LTP if Calculated is close to 0;
                            TheoreticalPrice = TheoreticalPrice < 0.05 ? 0.05 : TheoreticalPrice;

                            //changed to TheoreticalLTP on 08APR2021 by Amey
                            _PositionInfo.TheoreticalPrice = TheoreticalPrice;

                            //added on 08APR2021 by Amey
                            if (_PositionInfo.Segment == en_Segment.NSECD)
                            {
                                if (_PositionInfo.NetPosition == 0)
                                    _PositionInfo.TheoreticalMTM = _PositionInfo.BEP; //* ScripInfo.LotSize;
                                else if (_PositionInfo.NetPosition > 0)
                                    _PositionInfo.TheoreticalMTM = (TheoreticalPrice - _PositionInfo.BEP) * Math.Abs(_PositionInfo.NetPosition) /* ScripInfo.LotSize*/;
                                else if (_PositionInfo.NetPosition < 0)
                                    _PositionInfo.TheoreticalMTM = (_PositionInfo.BEP - TheoreticalPrice) * Math.Abs(_PositionInfo.NetPosition) /* ScripInfo.LotSize*/;
                            }
                            else
                            {
                                //added on 08APR2021 by Amey
                                if (_PositionInfo.NetPosition == 0)
                                    _PositionInfo.TheoreticalMTM = _PositionInfo.BEP;
                                else if (_PositionInfo.NetPosition > 0)
                                    _PositionInfo.TheoreticalMTM = (TheoreticalPrice - _PositionInfo.BEP) * Math.Abs(_PositionInfo.NetPosition);
                                else if (_PositionInfo.NetPosition < 0)
                                    _PositionInfo.TheoreticalMTM = (_PositionInfo.BEP - TheoreticalPrice) * Math.Abs(_PositionInfo.NetPosition);
                            }
                        }

                        //changed location on 21MAY2021 by Amey
                        //added on 14MAY2021 by Amey
                        if (dict_LTP.ContainsKey(ScripKey))
                        {
                            if (dict_LTP[ScripKey][2] == 0)
                            {
                                _PositionInfo.LTP = _PositionInfo.TheoreticalPrice;
                                _PositionInfo.IsLTPCalculated = true;
                            }
                            else
                                _PositionInfo.IsLTPCalculated = false;
                        }
                    }

                    //removed BEP assignment to LTP on 10FEB2021 by Amey
                    //added on 01DEC2020 by Amey
                    if (_PositionInfo.LTP != -1)
                    {
                        if (_PositionInfo.Segment == en_Segment.NSECD)
                        {
                            //added on 20NOV2020 by Amey
                            if (_PositionInfo.NetPosition == 0)
                                _PositionInfo.CDSMTM = _PositionInfo.BEP;//* ScripInfo.LotSize;
                            else if (_PositionInfo.NetPosition > 0)
                                _PositionInfo.CDSMTM = (_PositionInfo.LTP - _PositionInfo.BEP) * Math.Abs(_PositionInfo.NetPosition) /* ScripInfo.LotSize*/;
                            else if (_PositionInfo.NetPosition < 0)
                                _PositionInfo.CDSMTM = (_PositionInfo.BEP - _PositionInfo.LTP) * Math.Abs(_PositionInfo.NetPosition) /* ScripInfo.LotSize*/;

                            //added on 23NOV2020 by Amey
                            if (_PositionInfo.IntradayNetPosition == 0)
                                _PositionInfo.CDSIntradayMTM = _PositionInfo.IntradayBEP; //* ScripInfo.LotSize;
                            else if (_PositionInfo.IntradayNetPosition > 0)
                                _PositionInfo.CDSIntradayMTM = (_PositionInfo.LTP - _PositionInfo.IntradayBEP) * Math.Abs(_PositionInfo.IntradayNetPosition) /* ScripInfo.LotSize*/;
                            else if (_PositionInfo.IntradayNetPosition < 0)
                                _PositionInfo.CDSIntradayMTM = (_PositionInfo.IntradayBEP - _PositionInfo.LTP) * Math.Abs(_PositionInfo.IntradayNetPosition) /* ScripInfo.LotSize*/;
                        }
                        else
                        {
                            //added on 20NOV2020 by Amey
                            if (_PositionInfo.NetPosition == 0)
                                _PositionInfo.MTM = _PositionInfo.BEP;
                            else if (_PositionInfo.NetPosition > 0)
                                _PositionInfo.MTM = (_PositionInfo.LTP - _PositionInfo.BEP) * Math.Abs(_PositionInfo.NetPosition);
                            else if (_PositionInfo.NetPosition < 0)
                                _PositionInfo.MTM = (_PositionInfo.BEP - _PositionInfo.LTP) * Math.Abs(_PositionInfo.NetPosition);

                            //added on 23NOV2020 by Amey
                            if (_PositionInfo.IntradayNetPosition == 0)
                                _PositionInfo.IntradayMTM = _PositionInfo.IntradayBEP;
                            else if (_PositionInfo.IntradayNetPosition > 0)
                                _PositionInfo.IntradayMTM = (_PositionInfo.LTP - _PositionInfo.IntradayBEP) * Math.Abs(_PositionInfo.IntradayNetPosition);
                            else if (_PositionInfo.IntradayNetPosition < 0)
                                _PositionInfo.IntradayMTM = (_PositionInfo.IntradayBEP - _PositionInfo.LTP) * Math.Abs(_PositionInfo.IntradayNetPosition);
                        }
                    }

                    //changed location from inside LTP != -1 block to here on 08APR2021 by Amey
                    //added on 11FEB2021 by Amey
                    if (_ScripType == n.Structs.en_ScripType.EQ)
                    {
                        _PositionInfo.EquityAmount = _PositionInfo.BEP * _PositionInfo.NetPosition;

                        //added on 08APR2021 by Amey because no TheoreticalPrice in case of EQ.
                        _PositionInfo.TheoreticalPrice = _PositionInfo.LTP;
                        _PositionInfo.TheoreticalMTM = _PositionInfo.MTM;
                    }
                    else if (_ScripType == n.Structs.en_ScripType.XX)
                    {
                        //added on 08APR2021 by Amey because no TheoreticalPrice in case of XX.
                        _PositionInfo.TheoreticalPrice = _PositionInfo.LTP;
                        _PositionInfo.TheoreticalMTM = _PositionInfo.MTM;
                    }

                    //Added by Akshay on 01-09-2021 for Non-LTP trade
                    if (_PositionInfo.LTP == -1)
                    {
                        _PositionInfo.LTP = _PositionInfo.BEP;
                       
                    }
                    if(_PositionInfo.UnderlyingLTP == -1)
                    {
                    _PositionInfo.UnderlyingLTP = _PositionInfo.BEP;

                    }

                    if (_PositionInfo.Segment != en_Segment.NSECD)
                        _PositionInfo.DayNetPremium = _PositionInfo.IntradayBEP * (_PositionInfo.IntradayNetPosition == 0 ? -1 : _PositionInfo.IntradayNetPosition);
                    else if (_PositionInfo.Segment == en_Segment.NSECD)
                        _PositionInfo.DayNetPremiumCDS = _PositionInfo.IntradayBEP * (_PositionInfo.IntradayNetPosition == 0 ? -1 : _PositionInfo.IntradayNetPosition);

                }
            
            catch (Exception ee)
            {
                _logger.WriteLog("AssignLTPs : " + ee);
            }
        }

        private void AssignGreeks(ConsolidatedPositionInfo _PositionInfo, double BetaValue)
        {
            try
            {
                //added on 19APR2021 by Amey
                string LTPKey = _PositionInfo.Segment + "|" + _PositionInfo.Token;

                if (dict_Greeks.TryGetValue(LTPKey, out Greeks _GreeksInfo))
                {
                    _PositionInfo.IVMiddle = _GreeksInfo.IV;
                    _PositionInfo.IVLower = _GreeksInfo.IVLower;
                    _PositionInfo.IVHigher = _GreeksInfo.IVHigher;

                    _PositionInfo.Delta = _GreeksInfo.Delta * _PositionInfo.NetPosition * BetaValue;
                    _PositionInfo.Gamma = _GreeksInfo.Gamma * _PositionInfo.NetPosition * BetaValue;
                    _PositionInfo.Theta = _GreeksInfo.Theta * _PositionInfo.NetPosition * BetaValue;
                    _PositionInfo.Vega = _GreeksInfo.Vega * _PositionInfo.NetPosition * BetaValue;

                    //added on 21-12-2020 by Akshay
                    _PositionInfo.SingleDelta = _GreeksInfo.Delta * (_PositionInfo.NetPosition == 0 ? 0 : 1);
                    _PositionInfo.SingleGamma = _GreeksInfo.Gamma * (_PositionInfo.NetPosition == 0 ? 0 : 1);
                }
                else
                {
                    _PositionInfo.Delta = _PositionInfo.NetPosition * BetaValue;
                    _PositionInfo.Gamma = 0;
                    _PositionInfo.Theta = 0;
                    _PositionInfo.Vega = 0;

                    //Both values should be 0 in case of NetPos = 0. 08MAR2021-Amey
                    if (_PositionInfo.ScripType == n.Structs.en_ScripType.EQ || _PositionInfo.ScripType == n.Structs.en_ScripType.XX)
                    {
                        _PositionInfo.SingleDelta = _PositionInfo.NetPosition == 0 ? 0 : 1;
                        _PositionInfo.SingleGamma = _PositionInfo.NetPosition == 0 ? 0 : 1;
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("AssignGreeks : " + ee);
            }
        }

        private void InsertDay1()
        {
            try
            {
                //added Segment on 20APR2021 by Amey. To avoid same Token conflict from different segments.
                var result = list_Day1Positions.GroupBy(s => new { s.Token, s.Username, s.Segment })
                                            .Select(g => new
                                            {
                                                Username = g.Select(x => x.Username).First(),
                                                Segment = g.Select(x => x.Segment).First(),
                                                Token = g.Select(x => x.Token).First(),
                                                BEP = Math.Round(g.Sum(x => x.TradeQuantity * x.TradePrice) / (g.Sum(x => x.TradeQuantity) == 0 ? -1 : g.Sum(x => x.TradeQuantity)), 2),
                                                TradeQuantity = g.Sum(x => x.TradeQuantity),
                                                UnderlyingToken = g.Select(x => x.UnderlyingToken).First(),
                                                UnderlyingSegment = g.Select(x => x.UnderlyingSegment).First()
                                            });

                MySqlCommand cmd = new MySqlCommand();

                //changed to IGNORE on 10JUN2021 by Amey
                //changed on 07JAN2021 by Amey
                StringBuilder insertCmd = new StringBuilder("INSERT IGNORE INTO tbl_eod (Username,Segment,Token,TradePrice,TradeQuantity,UnderlyingSegment,UnderlyingToken) VALUES");

                List<string> toInsert = new List<string>();

                //changed to var on 27APR2021 by Amey
                var date_Tick = ConvertToUnixTimestamp(DateTime.Now);

                foreach (var _Item in result)
                {
                    var ScripInfo = dict_TokenScripInfo[$"{_Item.Segment}|{_Item.Token}"];
                    if ((ScripInfo.ExpiryUnix) > date_Tick || ScripInfo.ScripType == n.Structs.en_ScripType.EQ)   //09-01-18
                        toInsert.Add($"('{_Item.Username}','{ScripInfo.Segment}',{ScripInfo.Token},{_Item.BEP},{_Item.TradeQuantity},'{ScripInfo.UnderlyingSegment}',{ScripInfo.UnderlyingToken})");
                }
                try
                {
                    if (toInsert.Count > 0)
                    {
                        insertCmd.Append(string.Join(",", toInsert));
                        insertCmd.Append(";");
                        using (MySqlConnection myconnDay1 = new MySqlConnection(_MySQLCon))
                        {
                            cmd = new MySqlCommand(insertCmd.ToString(), myconnDay1);
                            myconnDay1.Open();
                            cmd.ExecuteNonQuery();
                            myconnDay1.Close();
                        }
                    }

                    AddToList($"Day1 Process Completed. {toInsert.Count} Rows added.");
                    _logger.WriteLog($"Day1 Process Completed. {toInsert.Count} Rows added.", true);

                    list_Day1Positions.Clear();
                }
                catch (Exception ee)
                {
                    _logger.WriteLog("InsertDay1 -inner : " + ee);
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("InsertDay1 : " + ee);
            }
        }

        //Added by Akshay on 29-12-2020 For Reading VAREQ
        private void ReadVAR_EQ(string interval)
        {
            try
            {
                var DailyTime = TimeSpan.Parse(interval).ToString();
                var timeParts = interval.Split(new char[1] { ':' });

                var dateNow = DateTime.Now;
                var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day,
                           int.Parse(timeParts[0]), int.Parse(timeParts[1]), Convert.ToInt32(timeParts[2]));

                if (date < dateNow)
                {
                    try
                    {
                        //Changed reading latest file logic by Akshay on 11-01-2021
                        _VaRMarginUpdateCount += 1;

                        var VAREQ_directory = new DirectoryInfo("C:/Prime/VARMargin");

                        var VAREQfileName = VAREQ_directory.GetFiles("C_VAR1_" + DateTime.Now.ToString("ddMMyyyy") + "*.DAT").OrderByDescending
                                            (f => f.LastWriteTime).ToArray();

                        //added check on 07APR2021 by Amey
                        if (VAREQfileName.Length == 0) return;

                        if (File.Exists(VAREQfileName[0].FullName))
                        {
                            foreach (var line in File.ReadAllLines(VAREQfileName[0].FullName))
                            {
                                string[] fields = line.Split(',');
                                if (fields.Length < 10) continue;
                                //if (fields[2] != "EQ") continue;

                                //added Series on 24MAY2021 by Amey
                                string ScripName = $"{fields[1]}-{fields[2]}";
                                double VAR = Convert.ToDouble(fields[9]);

                                if (!dict_VARMargin.ContainsKey(ScripName))
                                    dict_VARMargin.TryAdd(ScripName, VAR);
                                else
                                    dict_VARMargin[ScripName] = VAR;
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        _logger.WriteLog("Read VAREQ -inner " + ee);
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("Read VAREQ " + ee);
            }
        }

       

        private void ChangeLedgerValue()
        {
            try
            {
                foreach (var CLinfo in dict_ClientInfo)
                {
                    var Username = CLinfo.Key;
                    if (dict_Deposit.TryGetValue(Username, out double _ledger))
                    {
                        dict_ClientInfo[Username].ELM = _ledger;
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog(ee.StackTrace);
            }
        } 

        private void ReadHoldingFile()
        {
            try
            {
                var HoldingFile_directory = new DirectoryInfo("C:/Prime/Other");

                var HoldingFile = HoldingFile_directory.GetFiles("Holding_*.csv").OrderByDescending
                                    (f => f.LastWriteTime).ToArray();

                if (HoldingFile.Length == 0) return;

                if (File.Exists(HoldingFile[0].FullName))
                {
                    foreach (var item in File.ReadAllLines(HoldingFile[0].FullName))
                    {
                        try
                        {
                            string[] arr_Fields = item.Split(',');
                            string HoldingKey = $"{arr_Fields[0]}-{arr_Fields[1]}";
 
                            if (!dict_ClientHoldings.ContainsKey(HoldingKey))
                                dict_ClientHoldings.TryAdd(HoldingKey, Convert.ToInt64(arr_Fields[3]));

                        }
                        catch (Exception ee) {// _logger.WriteLog("ReadHoldingFile loop : " + ee);
                                              }
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog("ReadHoldingFile : " + ee); }
        }

        //Added by Akshay For Collateral
        private void ReadHaircutFile()
        {
            try
            {
                var HaircutFile_directory = new DirectoryInfo("C:/Prime/Other");

                var HaircutFile = HaircutFile_directory.GetFiles("APPSEC_COLLVAL_*.csv").OrderByDescending
                                    (f => f.LastWriteTime).ToArray();

                if (HaircutFile.Length == 0) return;

                if (File.Exists(HaircutFile[0].FullName))
                {
                    foreach (var item in File.ReadAllLines(HaircutFile[0].FullName))
                    {
                        try
                        {
                            string[] arr_Fields = item.Split(',');
                            string Underlying = arr_Fields[1].Trim().ToUpper().ToString();

                            if (!dict_UnderlyingHaircut.ContainsKey(Underlying))
                                dict_UnderlyingHaircut.TryAdd(Underlying, Convert.ToDouble(arr_Fields[4]));

                        }
                        catch (Exception ee) {// _logger.WriteLog("HaircutFile loop : " + ee);
                                              }
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog("HaircutFile : " + ee); }
        }



        //Added on 01JAN2022 by Nikhil for t-1 and t-2 positions
        private void ReadCMBhavcopy()
        {
            try
            {
                list_CMBhavcopy.Clear();
                var BhavcopyDirectory = new DirectoryInfo("C:/Prime");

                var CMBhavcopy = BhavcopyDirectory.GetFiles("cm*.csv")
                           .OrderByDescending(f => f.LastWriteTime)
                           .First();

                list_CMBhavcopy = Exchange.ReadCMBhavcopy(CMBhavcopy.FullName, false);

            }
            catch (Exception ee)
            {
                _logger.WriteLog("Read CM Bhavcopy " + ee.ToString());
            }
        }


        //Added on 01JAN2022 by Nikhil for t-1 positions
        private void Read_T1Positions()
        {

            //  ConcurrentDictionary<string, ConcurrentDictionary<string, long>> dict_T1positions = new ConcurrentDictionary<string, ConcurrentDictionary<string, long>>();

            dict_T1positions.Clear();
            list_T1positions.Clear();
            try
            {
                var T1position_directory = new DirectoryInfo("C:/Prime/Other");

                var VAREQfileName = T1position_directory.GetFiles("T1_POSITIONS_" + DateTime.Now.ToString("ddMMyyyy") + "*.csv").OrderByDescending
                                    (f => f.LastWriteTime).ToArray();
                if (VAREQfileName.Length == 0) { return; }


                if (File.Exists(VAREQfileName[0].FullName))
                {
                    using (FileStream stream = File.Open(VAREQfileName[0].FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            string Line = string.Empty;

                            while ((Line = sr.ReadLine()) != null)
                            {

                                if (Line.Contains("UNDERLYING") || Line.Equals(""))
                                {
                                    continue;
                                }

                                var arr_Fields = Line.Split(',');
                                if (arr_Fields[0].Trim() != "")
                                {
                                    try
                                    {
                                        var UserName = arr_Fields[0].Trim().ToUpper();
                                        var Underlying = arr_Fields[1].Trim().ToUpper();
                                        var Quantity = long.Parse(arr_Fields[2]);

                                        if (!hs_Usernames.Contains(UserName)) continue;

                                        if (dict_T1positions.TryGetValue(UserName, out ConcurrentDictionary<string, long> dict_UserWisePositions))
                                        {
                                            if (dict_UserWisePositions.TryGetValue(Underlying, out long _Quantity))
                                            {
                                                dict_T1positions[UserName][Underlying] = Quantity;
                                            }
                                            else
                                            {
                                                dict_T1positions[UserName].TryAdd(Underlying, Quantity);
                                            }
                                        }
                                        else
                                        {
                                            dict_T1positions.TryAdd(UserName, new ConcurrentDictionary<string, long> { [Underlying] = Quantity });
                                        }


                                        string ScripName = $"NSECM|{Underlying}-EQ";
                                        if (!dict_ScripInfo.TryGetValue(ScripName, out ContractMaster ScripInfo))
                                        {
                                            continue;
                                        }

                                        double Price = 0;
                                        // var Scrip = list_CMBhavcopy.Where(x => x.ScripName.Equals(ScripName)).FirstOrDefault();
                                        //if (Scrip != null)
                                        //{
                                        //    Price = Scrip.Close;
                                        //}
                                        //else { continue; }

                                        //if (Price == 0) { continue; }

                                        var T1position = new PositionInfo()
                                        {
                                            Username = UserName,
                                            Segment = ScripInfo.Segment,
                                            Token = ScripInfo.Token,
                                            TradePrice = 0,      //Price
                                            TradeQuantity = 0,  //Quantity
                                            TradeValue = 0, //Price * Quantity
                                            UnderlyingSegment = ScripInfo.UnderlyingSegment,
                                            UnderlyingToken = ScripInfo.UnderlyingToken,
                                        };
                                        list_T1positions.Add(T1position);

                                    }
                                    catch (Exception ee)
                                    {
                                        _logger.WriteLog("Data incorrect T-1 File Upload " + Line + " " + ee.ToString());
                                        continue;
                                    }

                                }

                            }

                        }
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("T-1 File Upload " + ee.ToString());

                // return null;
            }
        }

        //Added on 01JAN2022 by Nikhil for t-2 positions
        private void Read_T2positions()
        {
            try
            {
                dict_T2positions.Clear();
                list_T2positions.Clear();


                var T1position_directory = new DirectoryInfo("C:/Prime/Other");

                var VAREQfileName = T1position_directory.GetFiles("T2_POSITIONS_" + DateTime.Now.ToString("ddMMyyyy") + "*.csv").OrderByDescending
                                    (f => f.LastWriteTime).ToArray();
                if (VAREQfileName.Length == 0) { return; }


                if (File.Exists(VAREQfileName[0].FullName))
                {
                    using (FileStream stream = File.Open(VAREQfileName[0].FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            string Line = string.Empty;

                            while ((Line = sr.ReadLine()) != null)
                            {
                                if (Line.Contains("UNDERLYING") || Line.Equals(""))
                                {
                                    continue;
                                }

                                var arr_Fields = Line.Split(',');
                                if (arr_Fields[0].Trim() != "")
                                {
                                    try
                                    {
                                        var UserName = arr_Fields[0].Trim().ToUpper();
                                        var Underlying = arr_Fields[1].Trim().ToUpper();
                                        var Quantity = long.Parse(arr_Fields[2]);

                                        if (!hs_Usernames.Contains(UserName)) continue;

                                        if (dict_T2positions.TryGetValue(UserName, out ConcurrentDictionary<string, long> dict_UserWisePositions))
                                        {
                                            if (dict_UserWisePositions.TryGetValue(Underlying, out long _Quantity))
                                            {
                                                dict_T2positions[UserName][Underlying] = Quantity;
                                            }
                                            else
                                            {
                                                dict_T2positions[UserName].TryAdd(Underlying, Quantity);
                                            }
                                        }
                                        else
                                        {
                                            dict_T2positions.TryAdd(UserName, new ConcurrentDictionary<string, long> { [Underlying] = Quantity });
                                        }

                                        string ScripName = $"NSECM|{Underlying}-EQ";
                                        if (!dict_ScripInfo.TryGetValue(ScripName, out ContractMaster ScripInfo))
                                        {
                                            continue;
                                        }

                                        double Price = 0;
                                        //var Scrip = list_CMBhavcopy.Where(x => x.ScripName.Equals(ScripName)).FirstOrDefault();
                                        //if (Scrip != null)
                                        //{
                                        //    Price = Scrip.Close;
                                        //}
                                        //else { continue; }

                                        //if (Price == 0) { continue; }

                                        var T2position = new PositionInfo()
                                        {
                                            Username = UserName,
                                            Segment = ScripInfo.Segment,
                                            Token = ScripInfo.Token,
                                            TradePrice = 0,  //Price
                                            TradeQuantity = 0,   //Quantity
                                            TradeValue = 0,   //Price * Quantity
                                            UnderlyingSegment = ScripInfo.UnderlyingSegment,
                                            UnderlyingToken = ScripInfo.UnderlyingToken,
                                        };
                                        list_T2positions.Add(T2position);

                                    }
                                    catch (Exception ee)
                                    {
                                        _logger.WriteLog("Data incorrect T-2 File Upload " + Line);
                                        continue;
                                    }

                                }

                            }

                        }
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog("T-2 File Upload " + ee.ToString());

                // return null;
            }
        }


        // Added by Snehadri on 09MAR2022 for calculating Peak Margin
        private void CalculatePeakMargin(Dictionary<string, double[]> dict_AllSpanMargin, Dictionary<string, double> dict_AllVarMargin)
        {
            try
            {
                IsPeakMarginComputing = true;

                if (dict_AllSpanMargin.Any() || dict_AllVarMargin.Any())
                {
                    Dictionary<string, double[]> dict_SpanExposure = new Dictionary<string, double[]>();
                    Dictionary<string, double> dict_ClientMargin = new Dictionary<string, double>();

                    //foreach (var Spankey in dict_AllSpanMargin.Keys)
                    //{
                    //    var Client = Spankey.Split('_')[0];
                    //    var arr_Margin = dict_AllSpanMargin[Spankey];
                    //    var Margin = ((arr_Margin[0] + arr_Margin[2]) < 0 ? 0 : (arr_Margin[0] + arr_Margin[2])) + arr_Margin[1];

                    //    if (dict_ClientMargin.ContainsKey(Client))
                    //        dict_ClientMargin[Client] += Margin;
                    //    else
                    //        dict_ClientMargin.Add(Client, Margin);
                    //}

                    // Changed the logic by Snehadri on 02SEP2022
                    foreach (var _Spankey in dict_AllSpanMargin.Keys)
                    {
                        var _Client = _Spankey.Split('_')[0];
                        var arr_Margin = dict_AllSpanMargin[_Spankey];

                        if (dict_SpanExposure.ContainsKey(_Client))
                        {
                            dict_SpanExposure[_Client][0] += (arr_Margin[0] + arr_Margin[2]);
                            dict_SpanExposure[_Client][1] += arr_Margin[1];
                        }
                        else
                        {
                            dict_SpanExposure.Add(_Client, new double[2] { (arr_Margin[0] + arr_Margin[2]), arr_Margin[1] });
                        }
                    }

                    foreach (var _Client in dict_SpanExposure.Keys)
                    {
                        var _Span = dict_SpanExposure[_Client][0];
                        var _Exposure = dict_SpanExposure[_Client][1];

                        var _Margin = (_Span < 0 ? 0 : _Span) + _Exposure;

                        if (dict_ClientMargin.ContainsKey(_Client))
                            dict_ClientMargin[_Client] = _Margin;
                        else
                            dict_ClientMargin.Add(_Client, _Margin);
                    }

                    foreach (var _Client in hs_Usernames)
                    {
                        if (!dict_ClientMargin.ContainsKey(_Client) && !dict_AllVarMargin.ContainsKey(_Client)) continue;

                        string Client_Key = _Client + "_PEAK";
                        double FNOmargin = dict_ClientMargin.ContainsKey(_Client) ? dict_ClientMargin[_Client] : 0.0;
                        double EQmargin = dict_AllVarMargin.ContainsKey(_Client) ? dict_AllVarMargin[_Client] : 0.0;
                        double TotalPeakMargin = FNOmargin + EQmargin;
                        double old_FNOPeakMargin = dict_PeakMargin.ContainsKey(Client_Key) ? dict_PeakMargin[Client_Key][0] : 0.0;
                        double old_TotalPeakMargin = dict_PeakMargin.ContainsKey(Client_Key) ? dict_PeakMargin[Client_Key][2] : 0.0;
                        double peaktime = ConvertToUnixTimestamp(DateTime.Now);

                        if (dict_PeakMargin.ContainsKey(Client_Key))
                        {
                            if (FNOmargin > old_FNOPeakMargin)
                            {
                                dict_PeakMargin[Client_Key][0] = FNOmargin;
                                dict_PeakMargin[Client_Key][1] = peaktime;
                            }

                            if (TotalPeakMargin > old_TotalPeakMargin)
                            {
                                dict_PeakMargin[Client_Key][2] = TotalPeakMargin;
                                dict_PeakMargin[Client_Key][3] = peaktime;
                            }
                        }
                        else
                        {
                            if (FNOmargin != 0.0 || TotalPeakMargin != 0.0)
                            {
                                dict_PeakMargin.TryAdd(Client_Key, new double[4] { FNOmargin, peaktime, TotalPeakMargin, peaktime });
                            }

                        }
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog(ee.ToString()); }

            IsPeakMarginComputing = false;
        }

        private void CalculateCDSPeakMargin(Dictionary<string, double[]> dict_AllSpanMargin)
        {
            try
            {
                IsCDSPeakMarginComputing = true;

                if (dict_AllSpanMargin.Any())
                {
                    Dictionary<string, double[]> dict_SpanExposure = new Dictionary<string, double[]>();
                    Dictionary<string, double> dict_ClientMargin = new Dictionary<string, double>();

                    //foreach (var Spankey in dict_AllSpanMargin.Keys)
                    //{
                    //    var Client = Spankey.Split('_')[0];
                    //    var arr_Margin = dict_AllSpanMargin[Spankey];
                    //    var Margin = ((arr_Margin[0] + arr_Margin[2]) < 0 ? 0 : (arr_Margin[0] + arr_Margin[2])) + arr_Margin[1];

                    //    if (dict_ClientMargin.ContainsKey(Client))
                    //        dict_ClientMargin[Client] += Margin;
                    //    else
                    //        dict_ClientMargin.Add(Client, Margin);
                    //}

                    // Changed the logic by Snehadri on 02SEP2022
                    foreach (var _Spankey in dict_AllSpanMargin.Keys)
                    {
                        var _Client = _Spankey.Split('_')[0];
                        var arr_Margin = dict_AllSpanMargin[_Spankey];

                        if (dict_SpanExposure.ContainsKey(_Client))
                        {
                            dict_SpanExposure[_Client][0] += (arr_Margin[0] + arr_Margin[2]);
                            dict_SpanExposure[_Client][1] += arr_Margin[1];
                        }
                        else
                        {
                            dict_SpanExposure.Add(_Client, new double[2] { (arr_Margin[0] + arr_Margin[2]), arr_Margin[1] });
                        }
                    }

                    foreach (var _Client in dict_SpanExposure.Keys)
                    {
                        var _Span = dict_SpanExposure[_Client][0];
                        var _Exposure = dict_SpanExposure[_Client][1];

                        var _Margin = (_Span < 0 ? 0 : _Span) + _Exposure;

                        if (dict_ClientMargin.ContainsKey(_Client))
                            dict_ClientMargin[_Client] = _Margin;
                        else
                            dict_ClientMargin.Add(_Client, _Margin);
                    }

                    foreach (var _Client in hs_Usernames)
                    {
                        if (!dict_ClientMargin.ContainsKey(_Client)) continue;

                        string Client_Key = _Client + "_PEAK";
                        double FNOmargin = dict_ClientMargin.ContainsKey(_Client) ? dict_ClientMargin[_Client] : 0.0;
                        double old_FNOPeakMargin = dict_CDSPeakMargin.ContainsKey(Client_Key) ? dict_CDSPeakMargin[Client_Key][0] : 0.0;
                        double peaktime = ConvertToUnixTimestamp(DateTime.Now);

                        if (dict_CDSPeakMargin.ContainsKey(Client_Key))
                        {
                            if (FNOmargin > old_FNOPeakMargin)
                            {
                                dict_CDSPeakMargin[Client_Key][0] = FNOmargin;
                                dict_CDSPeakMargin[Client_Key][1] = peaktime;
                            }
                        }
                        else
                        {
                            if (FNOmargin != 0.0)
                            {
                                dict_CDSPeakMargin.TryAdd(Client_Key, new double[2] { FNOmargin, peaktime });
                            }
                        }
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog(ee.ToString()); }

            IsCDSPeakMarginComputing = false;
        }

        #endregion

        #region Semi-Important Methods
        private void ReadConfig()
        {
            try
            {
                XmlTextReader tReader = new XmlTextReader("C:/Prime/Config.xml");
                tReader.Read();
                ds_Config.ReadXml(tReader);

                //added on 25JAN2021 by Amey
                var DBInfo = ds_Config.Tables["DB"].Rows[0];
                _MySQLCon = $"Data Source={DBInfo["SERVER"]};Initial Catalog={DBInfo["NAME"]};UserID={DBInfo["USER"]};Password={DBInfo["PASSWORD"]};SslMode=none;";

                if (ds_Config.Tables[1].Rows.Count > 0)
                {
                    //added on 21DEC2020 by Amey

                    ip_EngineServer = IPAddress.Parse(ds_Config.Tables["CONNECTION"].Rows[0]["ENGINE-SERVER-IP"].ToString());
                    ip_GatewayServer = IPAddress.Parse(ds_Config.Tables["CONNECTION"].Rows[0]["GATEWAY-SERVER-IP"].ToString());

                    _GatewayServerHBPORT = Convert.ToInt32(ds_Config.Tables["CONNECTION"].Rows[0]["GATEWAY-SERVER-HB-PORT"]);
                    _EngineServerTradePORT = Convert.ToInt32(ds_Config.Tables["CONNECTION"].Rows[0]["ENGINE-SERVER-TRADE-PORT"]);
                    _EngineServerSpanPORT = Convert.ToInt32(ds_Config.Tables["CONNECTION"].Rows[0]["ENGINE-SERVER-SPAN-PORT"]);
                    _EngineServerHBPORT = Convert.ToInt32(ds_Config.Tables["CONNECTION"].Rows[0]["ENGINE-SERVER-HB-PORT"]);

                    //added on 23JUN2021 by Amey
                    _GatewayServerTradePORT = Convert.ToInt32(ds_Config.Tables["CONNECTION"].Rows[0]["GATEWAY-SERVER-TRADE-PORT"]);
                    _GatewayServerSpanPORT = Convert.ToInt32(ds_Config.Tables["CONNECTION"].Rows[0]["GATEWAY-SERVER-SPAN-PORT"]);

                     ip_nImageB_Server = IPAddress.Parse(ds_Config.Tables["CONNECTION"].Rows[0]["GATEWAY-SPAN-SERVER-IP"].ToString());
                    _nImageBServerHBPORT = Convert.ToInt32(ds_Config.Tables["CONNECTION"].Rows[0]["GATEWAY-SPAN-SERVER-HB-PORT"]);
                    _nImageBPORT = Convert.ToInt32(ds_Config.Tables["CONNECTION"].Rows[0]["GATEWAY-SPAN-SERVER-PORT"]);
                }

                //changed on 25JAN2021 by Amey
                //Added by Akshay on 31-12-2020 for VAREQ 
                list_VaRMarginUpdateInterval.Add("08:30:00");   //Added by Akshay on 25-03-2021
                foreach (var _Time in ds_Config.Tables["SPAN"].Rows[0]["RECOMPUTE-TIME"].ToString().Split(','))
                    list_VaRMarginUpdateInterval.Add(_Time);

                isDebug = Convert.ToBoolean(ds_Config.Tables["DEBUG-MODE"].Rows[0]["ENABLE"].ToString());
                _Version = ds_Config.Tables["OTHER"].Rows[0]["VERSION"].ToString();

                //added on 12MAY2021 by Amey
                _TimeoutMilliseconds = Convert.ToInt32(ds_Config.Tables["OTHER"].Rows[0]["TIMEOUT-SECONDS"]) * 1000;
            }
            catch (Exception ee)
            {
                _logger.WriteLog("ReadConfig : " + ee);
                XtraMessageBox.Show("Invalid entry in Config file. Please check logs for more details.", "Error");
            }
        }

        //added on 11JAN2021 by Amey
        private void GetIndexTokens(string FilePath)
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string[] arr_Lines = File.ReadAllLines(FilePath);
                    foreach (var Line in arr_Lines)
                    {
                        string[] arr_Fields = Line.Split(',');

                        //changed on 19APR2021 by Amey
                        dict_IndexTokens.TryAdd($"{arr_Fields[3]}CM|{arr_Fields[1]}", arr_Fields[0]);

                        dict_IndexLTP.TryAdd(arr_Fields[0], 0.0);
                    }
                }
                else
                    XtraMessageBox.Show("Index Token file is not available.", "Error");
            }
            catch (Exception ee) { _logger.WriteLog("GetIndexTokens : " + ee); }
        }

        //Added by Akshay on 30-06-2021 for Closing Price
        private void ReadClosingFile()
        {
            try
            {
                var FilePath = "C://Prime//IndexScrip.csv";
                if (File.Exists(FilePath))
                {
                    string[] arr_Lines = File.ReadAllLines(FilePath);
                    foreach (var Line in arr_Lines)
                    {
                        string[] arr_Fields = Line.Split(',');

                        if (!dict_IndexScrips.ContainsKey(arr_Fields[0].Trim().ToUpper()))
                            dict_IndexScrips.TryAdd(arr_Fields[0].Trim().ToUpper(), arr_Fields[1].Trim().ToUpper());

                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog("ReadClosingFile : " + ee); }

            try
            {
                var FileDirectory = new DirectoryInfo("C:/Prime");
                var ClosingFile = FileDirectory.GetFiles("ind_close_all_*.csv").OrderByDescending(f => f.LastWriteTime).First();

                if (File.Exists(ClosingFile.FullName))
                {
                    var arr_Lines = File.ReadAllLines(ClosingFile.FullName);

                    foreach (var Line in arr_Lines)
                    {
                        try
                        {
                            string[] arr_Fields = Line.Split(',');

                            if (dict_IndexScrips.ContainsKey(arr_Fields[0].ToUpper()))
                                dict_ScripClosing.TryAdd(dict_IndexScrips[arr_Fields[0].ToUpper()], Convert.ToDouble(arr_Fields[5]));
                        }

                        catch (Exception ee) { }
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog("ReadClosingFile : " + ee); }

            try
            {
                var FileDirectory = new DirectoryInfo("C:/Prime");
                var ClosingFile = FileDirectory.GetFiles("cm*.csv").OrderByDescending(f => f.LastWriteTime).First();

                if (File.Exists(ClosingFile.FullName))
                {
                    var arr_Lines = File.ReadAllLines(ClosingFile.FullName);

                    foreach (var Line in arr_Lines)
                    {
                        try
                        {
                            string[] arr_Fields = Line.Split(',');

                            if (!dict_ScripClosing.ContainsKey(arr_Fields[0]) && arr_Fields[1].Trim().ToString() == "EQ")
                                dict_ScripClosing.TryAdd($"{arr_Fields[0]}", Convert.ToDouble(arr_Fields[5]));
                        }

                        catch (Exception ee) { }

                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog("ReadClosingFile : " + ee); }

        }

        private void ReadClientDetail()
        {
            try
            {
                dict_UserInfo.Clear();
                dict_ClientInfo.Clear();
                dict_UserMappedInfo.Clear();

                //added on 27APR2021 by Amey. Removed hardcoded MySQL query.
                DataTable dt_UserInfo = new DataTable();
                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                {
                    con_MySQL.Open();
                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetUserInfo", con_MySQL))//modified by Navin on 12-06-2019
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;

                        MySqlDataAdapter dadapt = new MySqlDataAdapter(myCmd);
                        dadapt.Fill(dt_UserInfo);
                    }
                }
                clsEncryptionDecryption.DecryptData(dt_UserInfo);

                //changed logic on 08APR2021 by Amey
                var dt_SelectClient = new DataTable();

                try
                {
                    //added on 28JAN2021 by Amey
                    using (MySqlConnection con_MySQL = new MySqlConnection(_MySQLCon))
                    {
                        con_MySQL.Open();

                        using (MySqlCommand myCmd = new MySqlCommand("sp_GetClientDetail", con_MySQL))
                        {
                            myCmd.CommandType = CommandType.StoredProcedure;

                            //added on 27APR2021 by Amey
                            myCmd.Parameters.Add("prm_Type", MySqlDbType.LongText);
                            myCmd.Parameters["prm_Type"].Value = "ALL";

                            using (MySqlDataAdapter dselect = new MySqlDataAdapter(myCmd))
                                dselect.Fill(dt_SelectClient);
                        }

                        con_MySQL.Close();
                    }
                }
                catch (Exception ee) { _logger.WriteLog("ReadClientDetail ALL : " + ee); }

                // TODO: Change this to retrive ClientInfo after Prime logins successfully later.
                foreach (DataRow dRow in dt_UserInfo.Rows)
                {
                    try
                    {
                        //Console.WriteLine($"{DateTime.Now} | Loop Start");
                        //var MappedClients = dRow[1].ToString().Split(',');

                        //changed to AddOrUpdate on 01MAR2021 by Amey
                        //added on 17FEB2021 by Amey
                        //dict_UserInfo.AddOrUpdate(dRow[0].ToString().ToLower(), new string[2] { dRow[2].ToString(), dRow[1].ToString() },
                        //    (oldKey, oldVal) => new string[2] { dRow[2].ToString(), dRow[1].ToString() });

                        var _Username = dRow[0].ToString().ToLower();
                        var _MappedClients = dRow[1].ToString();

                        //changed to AddOrUpdate on 7JUN2021 by Amey. Was not updating until Engine is restarted.
                        dict_UserInfo.AddOrUpdate(_Username, dRow[2].ToString(), (oldKey, oldVal) => dRow[2].ToString());

                        //changed to AddOrUpdate on 7JUN2021 by Amey. Was not updating until Engine is restarted.
                        //added on 3JUN2021 by Amey
                        dict_UserMappedInfo.AddOrUpdate(_Username, _MappedClients.Split(','), (oldKey, oldVal) => _MappedClients.Split(','));
                    }
                    catch (Exception ee) { _logger.WriteLog("ReadClientDetail Loop : " + ee); }
                }

                //changed location on 3JUN2021 by Amey
                for (int item = 0; item < dt_SelectClient.Rows.Count; item++)
                {
                    try
                    {
                        var dRow_ClientInfo = dt_SelectClient.Rows[item];
                        string ID = dRow_ClientInfo["Username"].ToString();

                        ////added on 08APR2021 by Amey. For faster load times.
                        //if (!MappedClients.Contains(ID)) continue;

                        //added on 27MAR2021 by Amey because cant serialise class containing Dictionary.
                        var tmp_Dict = dict_NPLValues.Where(k => k.Key.StartsWith(ID)).ToDictionary(k => k.Key, v => v.Value);
                        var nplval = "";
                        if (tmp_Dict.Any())
                        {
                            foreach (var key in tmp_Dict.Keys)
                                nplval += key + "_" + tmp_Dict[key] + ",";

                            nplval = nplval.Substring(0, nplval.Length - 1);
                        }

                        //added on 08APR2021 by Amey
                        var MTD = 0.0;
                        if (dict_MTDValues.TryGetValue(ID, out double _MTD))
                            MTD = _MTD;

                        ClientInfo _ClientInfo = new ClientInfo()
                        {
                            Name = dRow_ClientInfo["NAME"].ToString(),
                            Zone = dRow_ClientInfo["Zone"].ToString(),
                            Branch = dRow_ClientInfo["Branch"].ToString(),
                            Family = dRow_ClientInfo["Family"].ToString(),
                            Product = dRow_ClientInfo["Product"].ToString(),
                            ELM = Convert.ToDouble(dRow_ClientInfo["Margin"]),
                            AdHoc = Convert.ToDouble(dRow_ClientInfo["Adhoc"]),
                            NPLValues = nplval,
                            MTDValue = MTD
                        };

                        //changed to AddOrUpdate on 7JUN2021 by Amey. Was not updating until Engine is restarted.
                        dict_ClientInfo.AddOrUpdate(ID, _ClientInfo, (oldKey, oldVal) => _ClientInfo);
                    }
                    catch (Exception ee) { _logger.WriteLog("ReadClientDetail Loop 2 : " + ee); }
                }
            }
            catch (Exception ee) { _logger.WriteLog("ReadClientDetail : " + ee); }

            //ChangeLedgerValue();
            
            //added by nikhil | Runtime User update
            UpdateUserMappedInfo();

            _EngineHeartBeatServer.UpdateCollections(dict_UserInfo, dict_UserMappedInfo, dict_ClientInfo, hs_BannedUnderlyings, dict_LimitInfo);
          
            _EngineDataServer.UpdateCollections(dict_UserMappedInfo);

        }

        private void UpdateUserMappedInfo()
        {
            try
            {
                foreach (var Users in dict_UserMappedInfo)
                {
                    var arr_Clients = Users.Value;
                    var distinct = arr_Clients.Distinct().ToList();
                    var new_arr = arr_Clients.Where(x => dict_ClientInfo.ContainsKey(x)).ToArray();
                    if (new_arr != null)
                    {
                        dict_UserMappedInfo[Users.Key] = new_arr;
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.WriteLog(ee.StackTrace);
            }
        }


        private void ReadContractMaster()
        {
            try
            {
                var dt_ContractMaster = new ds_Engine.dt_ContractMasterDataTable();
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
                foreach (ds_Engine.dt_ContractMasterRow v in dt_ContractMaster.Rows)
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
                        ScripType = (v.ScripType == "EQ" ? n.Structs.en_ScripType.EQ : (v.ScripType == "XX" ? n.Structs.en_ScripType.XX : (v.ScripType == "CE" ? n.Structs.en_ScripType.CE :
                                    n.Structs.en_ScripType.PE))),
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

                    //Changed by Akshay on 07-07-2021
                    //added on 19APR2021 by Amey
                    if (ScripInfo.Segment == en_Segment.NSECM) //&& ScripInfo.Series == "EQ")
                    {
                        if (ScripInfo.Series == "EQ")
                            dict_SpotTokenInfo.TryAdd(ScripInfo.Symbol, $"{ScripInfo.Segment}|{ScripInfo.Token}");

                        hs_CashTokens.Add(ScripInfo.Token);
                    }
                    //added on 20APR2021 by Amey
                    else if (ScripInfo.Segment == en_Segment.NSEFO)
                        hs_FOTokens.Add(ScripInfo.Token);
                    else if (ScripInfo.Segment == en_Segment.NSECD)
                        hs_CDTokens.Add(ScripInfo.Token);
                    else if (ScripInfo.Segment == en_Segment.BSEFO)//added by Omkar not sure it should added or not
                        hs_BSEFOTokens.Add(ScripInfo.Token);    


                }
            }
            catch (Exception ee) { _logger.WriteLog("ReadContractMaster : " + ee); }
        }

        private void ReadInterestRate()
        {
            try
            {
                _InterestRate = Convert.ToDouble(ds_Config.Tables["OTHER"].Rows[0]["INTEREST-RATE"].ToString());
                _IVMultiplier = Convert.ToDouble(ds_Config.Tables["OTHER"].Rows[0]["IV-MULTIPLIER"].ToString());
                _IVDivisor = Convert.ToDouble(ds_Config.Tables["OTHER"].Rows[0]["IV-DIVISOR"].ToString());
            }
            catch (Exception ee)
            {
                _logger.WriteLog("ReadInterestRate : " + ee);
            }
        }

        private void ReadIVFile()
        {
            try
            {
                //changed on 03FEB2021 by Amey
                var arr_Lines = File.ReadAllLines("C:/Prime/IV.csv");
                for (int i = 1; i < arr_Lines.Length; i++)
                {
                    string[] fields = arr_Lines[i].Split(',');  //Navin 22-11-2017
                    string Underlying = fields[0].ToUpper();

                    //added on 11NOV2020 by Amey
                    double[] arr_DefaultIVs = new double[3] { Convert.ToDouble(fields[1]), Convert.ToDouble(fields[2]), Convert.ToDouble(fields[3]) };

                    //changed on 20APR2021 by Amey
                    var UnderlyingKey = $"{en_Segment.NSEFO}|{Underlying}";
                    dict_DefaultIVs.AddOrUpdate(UnderlyingKey, arr_DefaultIVs, (oldKey, oldVal) => arr_DefaultIVs);  //Navin 23-11-2017

                    //changed on 20APR2021 by Amey
                    //Added on 9OCT2020 by Amey
                    if (!dict_ComputedATMIV.ContainsKey(UnderlyingKey))
                        dict_ComputedATMIV.TryAdd(UnderlyingKey, new double[2] { Convert.ToDouble(fields[2]), 0 });
                }
            }
            catch (Exception readtxtex)
            {
                _logger.WriteLog(readtxtex.ToString() + " : Read iv");
            }
        }

        private void ReadOGFile()
        {
            try
            {
                //changed on 03FEB2021 by Amey
                var arr_Lines = File.ReadAllLines("C:/Prime/OGRange.csv");
                for (int i = 1; i < arr_Lines.Length; i++)
                {
                    string[] fields = arr_Lines[i].Split(',');

                    try
                    {
                        dict_BetaValue.TryAdd(fields[0].ToUpper(), Convert.ToDouble(fields[3].Trim()));
                    }
                    catch (Exception ee)
                    {
                        _logger.WriteLog("ReadOGFile Loop : " + arr_Lines[i] + Environment.NewLine + ee);
                    }
                }
            }
            catch (Exception ogex)
            {
                _logger.WriteLog("ReadOGFile :  " + ogex);
            }
        }

        private void SendDataToPrime()
        {
            try
            {
                //changed to Any() on 28APR2021 by Amey
                if (dict_ConsolidatedPos.Any())
                    Parallel.Invoke(() => SendPositionsToPrime(), () => SendHBToPrime(), () => SendSpanToPrime(),()=>SendClientInfoToPrime());
            }
            catch (Exception ee)
            {
                _logger.WriteLog("SendDataToPrime : " + ee);
            }
        }

        private void SendPositionsToPrime()
        {
            try
            {
                _EngineDataServer.SendTradeData(new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_ConsolidatedPos));

                if (isDebug)
                    _logger.WriteLog("Trades Sent To Prime", true);
            }
            catch (Exception ee)
            {
                _logger.WriteLog("SendPositionsToPrime : " + ee);
            }
        }

        //added by Nikhil | runtime Client update
        private void SendClientInfoToPrime()
        {
            try
            {
                _EngineHeartBeatServer.SendClientData();
            }
            catch (Exception ee)
            {
                _logger.WriteLog("SendClientInfoToPrime : " + ee);
            }
        }


        private void SendHBToPrime()
        {
            try
            {
                Task.Run(() =>
                {
                    //added seperated TradeTimes on 05APR2021 by Amey
                    //_EngineHeartBeatServer.SendHeartBeat($"{FOLTT}_{CMLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{isGatewayConnected}" +
                    //   $"_{isSpanConnected}_{dict_IndexLTP["NIFTY"]}_{dict_IndexLTP["BANKNIFTY"]}_{_SpanComputeTime}_{_LatestSpanFileName}");

                    _EngineHeartBeatServer.SendHeartBeat($"{FOLTT}_{CMLTT}_{CDLTT}_{BSEFOLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{_CDLastTradeTime}_{_BSECMLastTradeTime}_{_BSEFOLastTradeTime}_{isGatewayConnected}" +
                $"_{isSpanConnected}_{isCDSSpanConnected}_{dict_IndexLTP["NIFTY"]}_{dict_IndexLTP["BANKNIFTY"]}_{_SpanComputeTime}_{_CDSSpanComputeTime}_{_LatestSpanFileName}_{_LatestCDSSpanFileName}"); //added by Akshay on 16-11-2021 for CD LTT


                    //_logger.WriteLog("HB End : " + DateTime.Now.ToString("ss:fff"), true);

                    //added on 07APR2021 by Amey
                    _EngineHeartBeatServer.SendBanInfo(new Dictionary<string, BanInfo>(dict_BanInfo));
                });
            }
            catch (Exception ee)
            {
                _logger.WriteLog("SendPositionsToPrime : " + ee);
            }
        }

        private void SendSpanToPrime()
        {
            try
            {
                MergeSpanDictionaries();
                MergeSpanDictionariesCDS();

                //Parallel.Invoke(() => MergeSpanDictionaries(), () => MergeSpanDictionariesCDS());

                if (dict_SpanData.Any() || dict_SpanDataCDS.Any());
                {
                    _EngineDataServer.SendSpanData(dict_SpanData,dict_SpanDataCDS);
                }

                if (isDebug)
                    _logger.WriteLog("Span Sent To Prime Keys : " + dict_SpanMargin.Count + "", true);
            }
            catch (Exception ee)
            {
                _logger.WriteLog("SendPositionsToPrime : " + ee);
            }
        }

        private void MergeSpanDictionaries()
        {
            try
            {
                dict_SpanData.Clear();

                try
                {
                    if (dict_ExpirySpanMargin.Any())
                    {
                        foreach (var Spankey in dict_ExpirySpanMargin.Keys)
                        {
                            if (!dict_SpanData.ContainsKey(Spankey))
                                dict_SpanData.Add(Spankey, dict_ExpirySpanMargin[Spankey]);
                            else
                                dict_SpanData[Spankey] = dict_ExpirySpanMargin[Spankey];
                        }
                    }
                }
                catch (Exception ee) { _logger.WriteLog(ee.ToString()); }

                try
                {
                    if (dict_SpanMargin.Any())
                    {
                        foreach (var Spankey in dict_SpanMargin.Keys)
                        {
                            if (!dict_SpanData.ContainsKey(Spankey))
                                dict_SpanData.Add(Spankey, dict_SpanMargin[Spankey]);
                            else
                                dict_SpanData[Spankey] = dict_SpanMargin[Spankey];
                        }
                    }
                }
                catch (Exception ee) { _logger.WriteLog(ee.ToString()); }

                try
                {
                    if (dict_EODMargin.Any())
                    {
                        foreach (var Spankey in dict_EODMargin.Keys)
                        {
                            if (!dict_SpanData.ContainsKey(Spankey))
                                dict_SpanData.Add(Spankey, dict_EODMargin[Spankey]);
                            else
                                dict_SpanData[Spankey] = dict_EODMargin[Spankey];
                        }
                    }
                }
                catch (Exception ee) { _logger.WriteLog(ee.ToString()); }


                try
                {
                    if (dict_PeakMargin.Any() && !IsPeakMarginComputing)
                    {
                        foreach (var Spankey in dict_PeakMargin.Keys)
                        {
                            if (!dict_SpanData.ContainsKey(Spankey))
                                dict_SpanData.Add(Spankey, dict_PeakMargin[Spankey]);
                            else
                                dict_SpanData[Spankey] = dict_PeakMargin[Spankey];
                        }
                    }
                }
                catch (Exception ee) { _logger.WriteLog(ee.ToString()); }

                try
                {
                    if (dict_ConsolidatedSpan.Any())
                    {
                        foreach (var Spankey in dict_ConsolidatedSpan.Keys)
                        {
                            if (!dict_SpanData.ContainsKey(Spankey))
                                dict_SpanData.Add(Spankey, dict_ConsolidatedSpan[Spankey]);
                            else
                                dict_SpanData[Spankey] = dict_ConsolidatedSpan[Spankey];
                        }
                    }
                }
                catch (Exception ee) { _logger.WriteLog(ee.ToString()); }

            }
            catch (Exception ee)
            {
                _logger.WriteLog("MergeSpanDictionaries : " + ee);
            }
        }

        private void MergeSpanDictionariesCDS()
        {
            try
            {
                dict_SpanDataCDS.Clear();

                try
                {
                    if (dict_CDSExpirySpanMargin.Any())
                    {
                        foreach (var Spankey in dict_CDSExpirySpanMargin.Keys)
                        {
                            if (!dict_SpanDataCDS.ContainsKey(Spankey))
                                dict_SpanDataCDS.Add(Spankey, dict_CDSExpirySpanMargin[Spankey]);
                            else
                                dict_SpanDataCDS[Spankey] = dict_CDSExpirySpanMargin[Spankey];
                        }
                    }
                }
                catch (Exception ee) { _logger.WriteLog(ee.ToString()); }

                try
                {
                    if (dict_CDSSpanMargin.Any())
                    {
                        foreach (var Spankey in dict_CDSSpanMargin.Keys)
                        {
                            if (!dict_SpanDataCDS.ContainsKey(Spankey))
                                dict_SpanDataCDS.Add(Spankey, dict_CDSSpanMargin[Spankey]);
                            else
                                dict_SpanDataCDS[Spankey] = dict_CDSSpanMargin[Spankey];
                        }
                    }
                }
                catch (Exception ee) { _logger.WriteLog(ee.ToString()); }

                try
                {
                    if (dict_CDSEODMargin.Any())
                    {
                        foreach (var Spankey in dict_CDSEODMargin.Keys)
                        {
                            if (!dict_SpanDataCDS.ContainsKey(Spankey))
                                dict_SpanDataCDS.Add(Spankey, dict_CDSEODMargin[Spankey]);
                            else
                                dict_SpanDataCDS[Spankey] = dict_CDSEODMargin[Spankey];
                        }
                    }
                }
                catch (Exception ee) { _logger.WriteLog(ee.ToString()); }

                try
                {
                    if (dict_CDSPeakMargin.Any() && !IsCDSPeakMarginComputing)
                    {
                        foreach (var Spankey in dict_CDSPeakMargin.Keys)
                        {
                            if (!dict_SpanDataCDS.ContainsKey(Spankey))
                                dict_SpanDataCDS.Add(Spankey, dict_CDSPeakMargin[Spankey]);
                            else
                                dict_SpanDataCDS[Spankey] = dict_CDSPeakMargin[Spankey];
                        }
                    }
                }
                catch (Exception ee) { _logger.WriteLog(ee.ToString()); }

            }
            catch (Exception EE)
            {
                _logger.WriteLog("MergeSpanDictionariesCDS : " + EE);
            }
        }


        private void SelectClientfromDatabase()
        {
            try
            {
                using (MySqlConnection con_MySQL = new MySqlConnection(_MySQLCon))
                {
                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetClientDetail", con_MySQL))
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;

                        //added on 27APR2021 by Amey
                        myCmd.Parameters.Add("prm_Type", MySqlDbType.LongText);
                        myCmd.Parameters["prm_Type"].Value = "ID";

                        con_MySQL.Open();

                        using (MySqlDataReader _mySqlDataReader = myCmd.ExecuteReader())
                        {
                            while (_mySqlDataReader.Read())
                            {
                                string ClientID = _mySqlDataReader.GetString(0).ToUpper().Trim();

                                //added on 12JAN2021 by Amey
                                hs_Usernames.Add(ClientID);
                            }
                        }

                        con_MySQL.Close();
                    }
                }
            }
            catch (Exception clientEx)
            {
                _logger.WriteLog("SelectClientfromDatabase " + clientEx);
            }
        }

        #region FeedLibrary Events

        private void eve_ConnectionResponse(string status)
        {
            try
            {
                if (status.ToUpper().Contains("-CON"))
                {
                    _logger.WriteLog($"ObjFeed_eve_ConenctionMessage EX : {status}");
                    AddToList("Disconnected from Feed Receiver. Check logs for more details.");

                    //changed position on 23APR2021 by Amey. To avoid unneccessary clearing dict_LTP on error.
                    //added on 25MAR2021 by Amey
                    dict_LTP.Clear();
                }
                //added on 14JAN2021 by Amey
                else if (status.ToUpper().Contains("EXCEPTION"))
                {
                    _logger.WriteLog($"ObjFeed_eve_ConenctionMessage EX : {status}");
                    AddToList("Error occurred while receiving feed. Check logs for more details.");
                }
                else
                {
                    _logger.WriteLog($"ObjFeed_eve_ConenctionMessage : {status}");

                    AddToList(status);//added by Navin on 17-10-2019

                    //added BSECM param on 23APR2021 by Amey
                    //if (status == "Successful FO" || status == "Successful CM" || status == "Successful BSECM")//added by Navin on 17-10-2019
                    //    _FeedCount++;

                    //added on 25JAN2021 by Amey
                    dict_LTP.Clear();
                }
            }
            catch (Exception ee) { _logger.WriteLog($"ObjFeed_eve_ConenctionMessage Error : {ee}"); }
        }

        private void ObjFeed_eve_FO7208(MBPEventArgs Token7208)
        {
            try
            {
                //added on 18NOV2020 by Amey
                FOLTT = ConvertToUnixTimestamp(DateTime.Now);

                //added Task on 12MAY2021 by Amey
                //added seperated TradeTimes on 05APR2021 by Amey
                //Task.Run(() => _EngineHeartBeatServer.SendHeartBeat($"{FOLTT}_{CMLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{isGatewayConnected}" +
                //    $"_{isSpanConnected}_{dict_IndexLTP["NIFTY"]}_{dict_IndexLTP["BANKNIFTY"]}_{_SpanComputeTime}_{_LatestSpanFileName}"));

                Task.Run(() => _EngineHeartBeatServer.SendHeartBeat($"{FOLTT}_{CMLTT}_{CDLTT}_{BSEFOLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{_CDLastTradeTime}_{_BSECMLastTradeTime}_{_BSEFOLastTradeTime}_{isGatewayConnected}" +
              $"_{isSpanConnected}_{isCDSSpanConnected}_{dict_IndexLTP["NIFTY"]}_{dict_IndexLTP["BANKNIFTY"]}_{_SpanComputeTime}_{_CDSSpanComputeTime}_{_LatestSpanFileName}_{_LatestCDSSpanFileName}")); //added by Akshay on 16-11-2021 for CD LTT


                int Token = Convert.ToInt32(Token7208.Token);
                double LTP = Convert.ToDouble(Token7208.LastTradedPrice) / 100;

                //added on 19APR2021 by Amey
                string LTPKey = en_Segment.NSEFO + "|" + Token;

                //added on 03FEB2021 by Amey
                var PreClose = Token7208.ClosingPrice;

                double[] arr_LTP;
                if (dict_LTP.TryGetValue(LTPKey, out arr_LTP))
                {
                    arr_LTP[0] = LTP;
                    arr_LTP[1] = Convert.ToDouble(PreClose) / 100;

                    //changed condition on 14MAY2021 by Amey. Better written.
                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (arr_LTP[2] == 0 && Token7208.LastTradeQuantity != "0")
                        arr_LTP[2] = 1;
                }

                //added on 9OCT2020 by Amey
                if (dict_XXLTP.TryGetValue(LTPKey, out arr_LTP))
                {
                    arr_LTP[0] = LTP;

                    //changed condition on 21MAY2021 by Amey
                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (arr_LTP[1] == 0 && Token7208.LastTradeQuantity != "0")
                        arr_LTP[1] = 1;
                }

                if (dict_Greeks.TryGetValue(LTPKey, out Greeks _GreeksInfo))
                {
                    if (Token7208.IV != "-")
                    {
                        //added Rounding on 01DEC2020 by Amey
                        double ReceivedIV = Math.Round(Convert.ToDouble(Token7208.IV), 2);
                        double Lower = ReceivedIV / _IVDivisor;
                        double Higher = ReceivedIV * _IVMultiplier;

                        //added default values in case numbers are very close to 0;
                        _GreeksInfo.IVLower = Lower <= 0.01 ? 15 : Lower;
                        _GreeksInfo.IV = ReceivedIV <= 0.01 ? 30 : ReceivedIV;
                        _GreeksInfo.IVHigher = Higher <= 0.01 ? 45 : Higher;

                        _GreeksInfo.IsReceived = true;
                    }
                    if (Token7208.Delta != "-")
                        _GreeksInfo.Delta = Convert.ToDouble(Token7208.Delta);
                    if (Token7208.Gamma != "-")
                        _GreeksInfo.Gamma = Convert.ToDouble(Token7208.Gamma);
                    if (Token7208.Theta != "-")
                        _GreeksInfo.Theta = Convert.ToDouble(Token7208.Theta);
                    if (Token7208.Vega != "-")
                        _GreeksInfo.Vega = Convert.ToDouble(Token7208.Vega);
                }
            }
            catch (Exception ltpEx)
            {
                _logger.WriteLog("Receive FO LTP :" + ltpEx);
            }
        }

        //added on 07JAN2021 by Amey
        private void ObjFeed_eve_CM7208(MBPEventArgs Token7208)
        {
            try
            {
                //added on 18NOV2020 by Amey
                CMLTT = ConvertToUnixTimestamp(DateTime.Now);

                int Token = Convert.ToInt32(Token7208.Token);
                double LTP = Convert.ToDouble(Token7208.LastTradedPrice) / 100;

                //added on 19APR2021 by Amey
                string LTPKey = en_Segment.NSECM + "|" + Token;

                //added on 08APR2021 by Amey
                if (dict_IndexTokens.TryGetValue(LTPKey, out string _IndexToken))
                    dict_IndexLTP[_IndexToken] = LTP;

                //added Task on 12MAY2021 by Amey
                //added seperated TradeTimes on 05APR2021 by Amey
                //changed on 07JAN2021 by Amey
                //Task.Run(() => _EngineHeartBeatServer.SendHeartBeat($"{FOLTT}_{CMLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{isGatewayConnected}" +
                //    $"_{isSpanConnected}_{dict_IndexLTP["NIFTY"]}_{dict_IndexLTP["BANKNIFTY"]}_{_SpanComputeTime}_{_LatestSpanFileName}"));

                Task.Run(() => _EngineHeartBeatServer.SendHeartBeat($"{FOLTT}_{CMLTT}_{CDLTT}_{BSEFOLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{_CDLastTradeTime}_{_BSECMLastTradeTime}_{_BSEFOLastTradeTime}_{isGatewayConnected}" +
              $"_{isSpanConnected}_{isCDSSpanConnected}_{dict_IndexLTP["NIFTY"]}_{dict_IndexLTP["BANKNIFTY"]}_{_SpanComputeTime}_{_CDSSpanComputeTime}_{_LatestSpanFileName}_{_LatestCDSSpanFileName}")); //added by Akshay on 16-11-2021 for CD LTT

                //added on 03FEB2021 by Amey
                var PreClose = Token7208.ClosingPrice;

                double[] arr_LTP;
                if (dict_LTP.TryGetValue(LTPKey, out arr_LTP))
                {
                    arr_LTP[0] = LTP;
                    arr_LTP[1] = Convert.ToDouble(PreClose) / 100;

                    //changed condition on 14MAY2021 by Amey. Better written.
                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (arr_LTP[2] == 0 && Token7208.LastTradeQuantity != "0")
                        arr_LTP[2] = 1;
                }

                //added on 9OCT2020 by Amey
                if (dict_XXLTP.TryGetValue(LTPKey, out arr_LTP))
                {
                    arr_LTP[0] = LTP;

                    //changed condition on 21MAY2021 by Amey
                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (arr_LTP[1] == 0 && Token7208.LastTradeQuantity != "0")
                        arr_LTP[1] = 1;
                }
            }
            catch (Exception ltpEx)
            {
                _logger.WriteLog("Receive CM LTP :" + ltpEx);
            }
        }

        private void ObjFeed_eve_7202(TickerIndexEventArgs Token7202) { }

        private void ObjFeed_eve_PartialCandle(FirstPartialCandleEventArgs partialCandle) { }

        private void _FeedLibrary_eve_BSECMTickReceived(BSECMPacket _BSECMPacket)
        {
            try
            {
                //added on 18NOV2020 by Amey
                CMLTT = ConvertToUnixTimestamp(DateTime.Now);

                int Token =Convert.ToInt32(_BSECMPacket.Token);
                double LTP =Convert.ToInt32(_BSECMPacket.LTP);

                //added on 19APR2021 by Amey
                string LTPKey = en_Segment.BSECM + "|" + Token;

                //added on 08APR2021 by Amey
                if (dict_IndexTokens.TryGetValue(LTPKey, out string _IndexToken))
                    dict_IndexLTP[_IndexToken] = LTP;

                //added seperated TradeTimes on 05APR2021 by Amey
                //changed on 07JAN2021 by Amey
                //Task.Run(() => _EngineHeartBeatServer.SendHeartBeat($"{FOLTT}_{CMLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{isGatewayConnected}" +
                //    $"_{isSpanConnected}_{dict_IndexLTP["NIFTY"]}_{dict_IndexLTP["BANKNIFTY"]}_{_SpanComputeTime}_{_LatestSpanFileName}"));


                Task.Run(() => _EngineHeartBeatServer.SendHeartBeat($"{FOLTT}_{CMLTT}_{CDLTT}_{BSEFOLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{_CDLastTradeTime}_{_BSECMLastTradeTime}_{_BSEFOLastTradeTime}_{isGatewayConnected}" +
              $"_{isSpanConnected}_{isCDSSpanConnected}_{dict_IndexLTP["NIFTY"]}_{dict_IndexLTP["BANKNIFTY"]}_{_SpanComputeTime}_{_CDSSpanComputeTime}_{_LatestSpanFileName}_{_LatestCDSSpanFileName}")); //added by Akshay on 16-11-2021 for CD LTT


                //added on 03FEB2021 by Amey
                var PreClose = Convert.ToDouble(_BSECMPacket.PreviousClose);

                double[] arr_LTP;
                if (dict_LTP.TryGetValue(LTPKey, out arr_LTP))
                {
                    arr_LTP[0] = LTP;
                    arr_LTP[1] = PreClose;

                    //changed condition on 14MAY2021 by Amey. Better written.
                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (arr_LTP[2] == 0 && Convert.ToInt32(_BSECMPacket.LTQ) != 0)
                        arr_LTP[2] = 1;
                }

                //added on 9OCT2020 by Amey
                if (dict_XXLTP.TryGetValue(LTPKey, out arr_LTP))
                {
                    arr_LTP[0] = LTP;

                    //changed condition on 21MAY2021 by Amey
                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (arr_LTP[1] == 0 && Convert.ToInt32(_BSECMPacket.LTQ) != 0)
                        arr_LTP[1] = 1;
                }
            }
            catch (Exception ee) { _logger.WriteLog("_FeedLibrary_eve_BSECMTickReceived : " + ee); }
        }

        //added by Omkar
        private void _FeedLibrary_eve_BSEFOTickReceived(BSEFOPacket _BSEFOPacket)
        {
            try
            {
                //added on 18NOV2020 by Amey
                BSEFOLTT = ConvertToUnixTimestamp(DateTime.Now);

                int Token = Convert.ToInt32(_BSEFOPacket.Token);
                double LTP = Convert.ToDouble(_BSEFOPacket.LTP) /100;

                //added on 19APR2021 by Amey
                string LTPKey = en_Segment.BSEFO + "|" + Token;

                //added on 08APR2021 by Amey
                if (dict_IndexTokens.TryGetValue(LTPKey, out string _IndexToken))
                    dict_IndexLTP[_IndexToken] = LTP;

                //added seperated TradeTimes on 05APR2021 by Amey
                //changed on 07JAN2021 by Amey
                //Task.Run(() => _EngineHeartBeatServer.SendHeartBeat($"{FOLTT}_{CMLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{isGatewayConnected}" +
                //    $"_{isSpanConnected}_{dict_IndexLTP["NIFTY"]}_{dict_IndexLTP["BANKNIFTY"]}_{_SpanComputeTime}_{_LatestSpanFileName}"));


                Task.Run(() => _EngineHeartBeatServer.SendHeartBeat($"{FOLTT}_{CMLTT}_{CDLTT}_{BSEFOLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{_CDLastTradeTime}_{_BSECMLastTradeTime}_{_BSEFOLastTradeTime}_{isGatewayConnected}" +
              $"_{isSpanConnected}_{isCDSSpanConnected}_{dict_IndexLTP["NIFTY"]}_{dict_IndexLTP["BANKNIFTY"]}_{_SpanComputeTime}_{_CDSSpanComputeTime}_{_LatestSpanFileName}_{_LatestCDSSpanFileName}")); //added by Akshay on 16-11-2021 for CD LTT


                //added on 03FEB2021 by Amey
                var PreClose = Convert.ToDouble(_BSEFOPacket.PreviousClose);

                double[] arr_LTP;
                if (dict_LTP.TryGetValue(LTPKey, out arr_LTP))
                {
                    arr_LTP[0] = LTP;
                    arr_LTP[1] = PreClose;

                    //changed condition on 14MAY2021 by Amey. Better written.
                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (arr_LTP[2] == 0 && Convert.ToInt32(_BSEFOPacket.LTQ) != 0)
                        arr_LTP[2] = 1;
                }

                //added on 9OCT2020 by Amey
                if (dict_XXLTP.TryGetValue(LTPKey, out arr_LTP))
                {
                    arr_LTP[0] = LTP;

                    //changed condition on 21MAY2021 by Amey
                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (arr_LTP[1] == 0 && Convert.ToInt32(_BSEFOPacket.LTQ) != 0)
                        arr_LTP[1] = 1;
                }
            }
            catch (Exception ee) { _logger.WriteLog("_FeedLibrary_eve_BSEFOTickReceived : " + ee); }
        }

        private void _FeedLibrary_eve_CD7208TickReceived(MBPEventArgs Token7208)
        {
            try
            {
                //_logger.WriteLog($"LTP : {Token7208.ClosingPrice}, Delta : {Token7208.Delta}");

                //added on 18NOV2020 by Amey
                CDLTT = ConvertToUnixTimestamp(DateTime.Now);

                //added Task on 12MAY2021 by Amey
                //added seperated TradeTimes on 05APR2021 by Amey
                //Task.Run(() => _EngineHeartBeatServer.SendHeartBeat($"{FOLTT}_{CMLTT}_{CDLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{_CDLastTradeTime}_{isGatewayConnected}" +
                //    $"_{isSpanConnected}_{isCDSSpanConnected}_{dict_IndexLTP["NIFTY"]}_{dict_IndexLTP["BANKNIFTY"]}_{_SpanComputeTime}_{_CDSSpanComputeTime}_{_LatestSpanFileName}_{_LatestCDSSpanFileName}")); //added by Akshay on 16-11-2021 for CD LTT

                Task.Run(() => _EngineHeartBeatServer.SendHeartBeat($"{FOLTT}_{CMLTT}_{CDLTT}_{BSEFOLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{_CDLastTradeTime}_{_BSECMLastTradeTime}_{_BSEFOLastTradeTime}_{isGatewayConnected}" +
              $"_{isSpanConnected}_{isCDSSpanConnected}_{dict_IndexLTP["NIFTY"]}_{dict_IndexLTP["BANKNIFTY"]}_{_SpanComputeTime}_{_CDSSpanComputeTime}_{_LatestSpanFileName}_{_LatestCDSSpanFileName}")); //added by Akshay on 16-11-2021 for CD LTT


                int Token = Convert.ToInt32(Token7208.Token);
                double LTP = Math.Round(Convert.ToDouble(Token7208.LastTradedPrice) / 10000, 4);

                //added on 19APR2021 by Amey
                string LTPKey = en_Segment.NSECD + "|" + Token;

                //added on 03FEB2021 by Amey
                var PreClose = Token7208.ClosingPrice;

                double[] arr_LTP;
                if (dict_LTP.TryGetValue(LTPKey, out arr_LTP))
                {
                    arr_LTP[0] = LTP;
                    arr_LTP[1] = Math.Round(Convert.ToDouble(PreClose) / 10000, 4);

                    //changed condition on 14MAY2021 by Amey. Better written.
                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (arr_LTP[2] == 0 && Token7208.LastTradeQuantity != "0")
                        arr_LTP[2] = 1;
                }

                //added on 9OCT2020 by Amey
                if (dict_XXLTP.TryGetValue(LTPKey, out arr_LTP))
                {
                    arr_LTP[0] = LTP;

                    //changed condition on 21MAY2021 by Amey
                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (arr_LTP[1] == 0 && Token7208.LastTradeQuantity != "0")
                        arr_LTP[1] = 1;
                }

                if (dict_Greeks.TryGetValue(LTPKey, out Greeks _GreeksInfo))
                {
                    if (Token7208.IV != "-")
                    {
                        //added Rounding on 01DEC2020 by Amey
                        double ReceivedIV = Math.Round(Convert.ToDouble(Token7208.IV), 2);
                        double Lower = ReceivedIV / _IVDivisor;
                        double Higher = ReceivedIV * _IVMultiplier;

                        //added default values in case numbers are very close to 0;
                        _GreeksInfo.IVLower = Lower <= 0.01 ? 15 : Lower;
                        _GreeksInfo.IV = ReceivedIV <= 0.01 ? 30 : ReceivedIV;
                        _GreeksInfo.IVHigher = Higher <= 0.01 ? 45 : Higher;

                        _GreeksInfo.IsReceived = true;
                    }
                    if (Token7208.Delta != "-")
                        _GreeksInfo.Delta = Convert.ToDouble(Token7208.Delta);
                    if (Token7208.Gamma != "-")
                        _GreeksInfo.Gamma = Convert.ToDouble(Token7208.Gamma);
                    if (Token7208.Theta != "-")
                        _GreeksInfo.Theta = Convert.ToDouble(Token7208.Theta);
                    if (Token7208.Vega != "-")
                        _GreeksInfo.Vega = Convert.ToDouble(Token7208.Vega);
                }
            }
            catch (Exception ltpEx)
            {
                _logger.WriteLog("Receive CD LTP :" + ltpEx);
            }
        }

        #endregion

        #region Gateway-HeartBeat-Server Events

        private void _GatewayHeartBeatClient_eve_LTTReceived(double _FOLastTradeTime, double _CMLastTradeTime, double _CDLastTradeTime,double _BsecmLastTradeTime, double _BSEFOLastTradeTime)
        {
            this._FOLastTradeTime = _FOLastTradeTime;
            this._CMLastTradeTime = _CMLastTradeTime;
            this._CDLastTradeTime = _CDLastTradeTime;
            this._BSECMLastTradeTime = _BsecmLastTradeTime;
            this._BSEFOLastTradeTime = _BSEFOLastTradeTime;
        }

        private void _GatewayHeartBeatClient_eve_SpanStatusReceived(bool isSpanConnected, bool isCDSSpanConnected)
        {
            this.isSpanConnected = isSpanConnected ? 1 : 0;
            this.isCDSSpanConnected = isCDSSpanConnected ? 1 : 0;
        }

        private void _GatewayHeartBeatClient_eve_GatewayStatusReceived(bool isGatewayConnected)
        {
            if (isGatewayConnected)
                this.isGatewayConnected = 1;
            else
            {
                this.isGatewayConnected = 0;
                isSpanConnected = 0;
            }
        }

        private void _GatewayHeartBeatClient_eve_SpanInfoReceived(double _SpanComputeTime, double _CDSSpanComputeTime, string _LatestSpanFileName, string _LatestCDSSpanFileName)
        {
            this._SpanComputeTime = _SpanComputeTime;
            this._CDSSpanComputeTime = _CDSSpanComputeTime;
            this._LatestSpanFileName = _LatestSpanFileName;
            this._LatestCDSSpanFileName = _LatestCDSSpanFileName;
        }

        private void _WriteLog(string _Message)
        {
            _logger.WriteLog(_Message);
        }

        #endregion

        #endregion

        #region Supplimentary Methods

        public void ReadLicense()//(string scrip)
        {
            try
            {
                string[] arr_LicenseFields = clsWriteLog.DecryptLicense(File.ReadAllText(Application.StartupPath + @"\Engine.ns")).Split('|');
                if (arr_LicenseFields.Length == 5 && arr_LicenseFields[0] == "ENGINE")
                {
                    DateTime dt_LicenseExpiry = ConvertFromUnixTimestamp(Convert.ToDouble(arr_LicenseFields[1]));

                    if (dt_LicenseExpiry.Date < DateTime.Now.Date)
                    {
                        XtraMessageBox.Show("License expired. Please contact System Administrator.", "Error");
                        Environment.Exit(0);
                    }

                    _MaxAllowedInstances = Convert.ToInt32(arr_LicenseFields[4]);
                }
                else
                {
                    XtraMessageBox.Show("Invalid License. Please contact System Administrator.", "Error");
                    Environment.Exit(0);
                }
            }
            catch (Exception)
            {
                XtraMessageBox.Show("License file not found.", "Error");
                Environment.Exit(0);
            }
        }

        private void AddToList(string text)//event handler of delegate
        {
            if (this.lb_ErrorLog.InvokeRequired)
                this.Invoke((MethodInvoker)(() => { lb_ErrorLog.Items.Insert(0, DateTime.Now + " : " + text); }));
            else
                lb_ErrorLog.Items.Insert(0, DateTime.Now + " : " + text);
        }

        private void btn_UploadEarlyPayIn_Click(object sender, EventArgs e)
        {
            SelectClientfromDatabase();
            bool IsError = false;
            try
            {
                btn_UploadEarlyPayIn.Enabled = false;

                AddToList("EPN file upload started");

                var _OpenFile = new OpenFileDialog();
                if (_OpenFile.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream stream = File.Open(_OpenFile.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            string Line = string.Empty;

                            while ((Line = sr.ReadLine()) != null)
                            {
                                if (Line.Contains("UNDERLYING")) { continue; }
                                var arr_Fields = Line.Split(',');
                                if (arr_Fields[0].Trim() != "")
                                {
                                    try
                                    {
                                        var UserName = arr_Fields[0].ToUpper().Trim();
                                        var Underlying = arr_Fields[1].ToUpper().Trim();
                                        var EpnQuantity = long.Parse(arr_Fields[2]);

                                        if (!hs_Usernames.Contains(UserName)) continue;


                                        if (dict_EarlyPayIn.TryGetValue(UserName, out ConcurrentDictionary<string, long> dict_UserWisePositions))
                                        {
                                            if (dict_UserWisePositions.TryGetValue(Underlying, out long _Quantity))
                                            {
                                                dict_EarlyPayIn[UserName][Underlying] = EpnQuantity;
                                            }
                                            else
                                            {
                                                dict_EarlyPayIn[UserName].TryAdd(Underlying, EpnQuantity);
                                            }
                                        }
                                        else
                                        {
                                            dict_EarlyPayIn.TryAdd(UserName, new ConcurrentDictionary<string, long> { [Underlying] = EpnQuantity });
                                        }

                                    }
                                    catch (Exception ee)
                                    {
                                        _logger.WriteLog("Data Incorrect EPN File Upload " + Line);
                                        IsError = true;
                                        continue;
                                    }

                                }

                            }

                        }
                    }

                    if (IsError)
                    {
                        AddToList("Invalid data found in EPN file");
                    }
                    else { AddToList("EPN file upload process completed."); }
                }

            }
            catch (Exception ee)
            {
                IsError = true;
                _logger.WriteLog("Read EPN file " + ee.StackTrace);
            }

            btn_UploadEarlyPayIn.Enabled = true;
        }

        private void btn_UploadLedger_Click(object sender, EventArgs e)
        {
            dict_Deposit.Clear();

            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var All_Lines = File.ReadAllLines(openFileDialog1.FileName);
                    foreach (var line in All_Lines)
                    {
                        if (line.Equals("")) { continue; }

                        var arr_field = line.Split(',');
                        try
                        {
                            if (arr_field.Length < 2) continue;

                            var userID = arr_field[0].ToUpper().Trim();
                            var Margin = arr_field[1].ToUpper().Trim();
                            //var Adhoc = arr_field[2].ToUpper().Trim();

                            if (dict_ClientInfo.ContainsKey(userID))
                            {
                                dict_ClientInfo[userID].ELM = Convert.ToDouble(Margin);
                              //  dict_ClientInfo[userID].AdHoc = Convert.ToDouble(Adhoc);
                            }

                        }
                        catch (Exception ee)
                        {
                            _logger.WriteLog("Deposit file Entry skipped for" + line);
                            continue;
                        }
                    }
                    Task.Run(() => UpdateMArginDB());
                    AddToList("Ledger File Uploaded.");

                    _EngineHeartBeatServer.ClientLedgerUpdated(new ConcurrentDictionary<string, ClientInfo>(dict_ClientInfo));
                }
            }
            catch (Exception ee)
            {
                XtraMessageBox.Show("Something went wrong", "Error");
                _logger.WriteLog(ee.ToString());
            }       
        }

        private void UpdateMArginDB()
        {
            try
            {
                var sbupdate = new StringBuilder();
                foreach (var _ClientID in dict_ClientInfo.Keys)
                {
                   
                    using (var con_MySQL = new MySqlConnection(_MySQLCon))
                    {
                        con_MySQL.Open();

                        //using (var cmd = new MySqlCommand("UPDATE tbl_clientdetail SET Margin=@Margin, Adhoc=@Adhoc WHERE Username=@Username", con_MySQL))
                        using (var cmd = new MySqlCommand("UPDATE tbl_clientdetail SET Margin=@Margin WHERE Username=@Username", con_MySQL))
                        {
                            cmd.Parameters.AddWithValue("@Username", _ClientID.ToString().ToUpper());
                            cmd.Parameters.AddWithValue("@Margin", dict_ClientInfo[_ClientID].ELM.ToString().ToUpper());
                           // cmd.Parameters.AddWithValue("@Adhoc", dict_ClientInfo[_ClientID].AdHoc.ToString().ToUpper());
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog(ee.StackTrace); XtraMessageBox.Show("Something went wrong while uploading in DB", "Error"); }
        }

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

        private void ObjAddUser_eve_Errorlog(string strMessage)
        {
            _logger.WriteLog(strMessage);
        }

        private void btn_EODPositions_Click(object sender, EventArgs e)
        {
            try
            {
                _nImageBHeartBeatClient.eve_nImageBError += _GatewayHeartBeatClient_eve_Error;
                _nImageBDataClient.eve_Error += _WriteLog;
                _nImageBDataClient.eve_ConsSpanReceived += _Consolidated_SpanReceived;
                Task.Run(() => _nImageBHeartBeatClient.ConnectTonImageB("n.ENGINE", ip_nImageB_Server, _nImageBServerHBPORT));
                Thread.Sleep(500);
                Task.Run(() => _nImageBDataClient.ConnectTonImageBSpan("n.ENGINE", ip_nImageB_Server, _nImageBPORT));
                Thread.Sleep(500);
                bool success = _nImageBHeartBeatClient.SendTonImageB("SIGNAL^START");

                if (success)
                    AddToList("nImageB Connected");
                else
                    AddToList("Failed to connect nImageB");

            }
            catch (Exception ee) { _logger.WriteLog(ee.ToString()); }
        }

        private void _GatewayHeartBeatClient_eve_Error(string _Message)
        {
            _logger.WriteLog(_Message);
        }

        private void _Consolidated_SpanReceived(Dictionary<string, double[]> dict_Span)
        {
            try
            {
                dict_ConsolidatedSpan = dict_Span;
            }
            catch (Exception ee) { _logger.WriteLog("Exception occurred while receiving margin from socket at " + DateTime.Now + Environment.NewLine + ee.ToString()); }
        }

        private DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        private double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - origin;
            return diff.TotalSeconds;
        }

        #region Other Calculation Functions

        public double ImpliedCallVolatility(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Target, double Volatility, int Dividend)
        {
            try
            {
                double high = 0;
                double low = 0;
                high = 5;
                low = 0;
                while ((high - low) > 0.0001)
                {
                    if (CallOption(UnderlyingPrice, ExercisePrice, Time, Interest, (high + low) / 2, Dividend) > Target)
                    {
                        high = (high + low) / 2;
                    }
                    else
                    {
                        low = (high + low) / 2;
                    }
                }
                return (high + low) / 2;
            }
            catch (Exception ee)
            {
                _logger.WriteLog("ImpliedCallVolatility : " + ee);

                return 0;
            }
        }

        public double ImpliedPutVolatility(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Target, double Volatility, int Dividend)
        {
            try
            {
                double high = 0;
                double low = 0;

                high = 5;
                low = 0;
                while ((high - low) > 0.0001)
                {
                    if (PutOption(UnderlyingPrice, ExercisePrice, Time, Interest, (high + low) / 2, Dividend) > Target)
                    {
                        high = (high + low) / 2;
                    }
                    else
                    {
                        low = (high + low) / 2;
                    }
                }
                return (high + low) / 2;
            }
            catch (Exception ee)
            {
                _logger.WriteLog("ImpliedPutVolatility : " + ee);

                return 0;
            }
        }

        public double CallOption(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {
            try
            {
                //double test= Math.Exp(-Dividend * Time) * UnderlyingPrice * NORMSDIST(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend)) - ExercisePrice * Math.Exp(-Interest * Time) * NORMSDIST(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend) - Volatility * Math.Sqrt(Time));
                //return test;
                return Math.Exp(-Dividend * Time) * UnderlyingPrice * NORMSDIST(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend)) - ExercisePrice * Math.Exp(-Interest * Time) * NORMSDIST(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend) - Volatility * Math.Sqrt(Time));
            }
            catch (Exception ee)
            {
                _logger.WriteLog("CallOption : " + ee);

                return 0;
            }
        }

        public double PutOption(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {
            try
            {
                double dTwoo = -dTwo(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend);
                double dOnee = -dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend);

                return ExercisePrice * Math.Exp(-Interest * Time) * NORMSDIST(dTwoo) - Math.Exp(-Dividend * Time) * UnderlyingPrice * NORMSDIST(dOnee);
            }
            catch (Exception ee)
            {
                _logger.WriteLog("PutOption : " + ee);

                return 0;
            }
        }

        public static double NORMSDIST(double z)
        {

            double sign = 1;
            if (z < 0) sign = -1;
            //double te= 0.5 * (1.0 + sign * erf(Math.Abs(z) / Math.Sqrt(2)));
            //return te;
            return 0.5 * (1.0 + sign * erf(Math.Abs(z) / Math.Sqrt(2)));
        }

        public double dOne(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {
            try
            {
                //double test= (Math.Log(UnderlyingPrice / ExercisePrice) + (Interest - Dividend + 0.5 * Math.Pow(Volatility, 2)) * Time) / (Volatility * (Math.Sqrt(Time)));
                return (Math.Log(UnderlyingPrice / ExercisePrice) + (Interest - Dividend + 0.5 * Math.Pow(Volatility, 2)) * Time) / (Volatility * (Math.Sqrt(Time)));
                //return test;
            }
            catch (Exception ee)
            {
                _logger.WriteLog("dOne : " + ee);

                return 0;
            }
        }

        public double dTwo(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {
            try
            {
                return dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend) - Volatility * Math.Sqrt(Time);
            }
            catch (Exception ee)
            {
                _logger.WriteLog("dTwo : " + ee);

                return 0;
            }
        }

        private static double erf(double x)
        {
            //A&S formula 7.1.26
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;
            x = Math.Abs(x);
            double t = 1 / (1 + p * x);
            return 1 - ((((((a5 * t + a4) * t) + a3) * t + a2) * t) + a1) * t * Math.Exp(-1 * x * x);
        }

        #endregion

        #endregion
    }
}