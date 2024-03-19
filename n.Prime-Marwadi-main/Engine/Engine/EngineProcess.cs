using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Xml;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.IO;
using DevExpress.Skins;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using FeedLibrary;
using Newtonsoft.Json;
using XDMessaging;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;

//Name of Sub-Client: EP
namespace Engine
{
    public partial class EngineProcess : DevExpress.XtraEditors.XtraForm
    {
        //added on 21DEC2020 by Amey
        IPAddress ip_EngineServer = IPAddress.Parse("127.0.0.1");

        //PrimeConnector objenginedll;// added by navin on 01-03-2019 for delegate//commented on 04-02-2020
        int PrimeDataPort;//added by Navin on 04-02-2020
        #region added by Navin on 03-10-2019 for margin calculation
        DataTable dt_SpanTrades = new DataTable();//added by Navin on 03-10-2019 for span calculations
        int _SpanInterval = 3;
        int SpanDataPort;//14-10-2019
        StringBuilder commonpath = new StringBuilder();
        bool _spanFlag = false;
        #endregion
        int _GatewayPort, PrimeHeartBeatPort;

        //added LTT on 18NOV2020 by Amey
        //added FOLTT & CMLTT on 07JAN2021 by Amey
        double _LastTradeTime = 0, _GatewayStatus = 0, FOLTT = 0, CMLTT = 0;

        /// <summary>
        /// Used to send Positions and Span to Prime. Uses PrimePort, SpanPort from config.
        /// </summary>
        clsPrimeConnector _PrimeDataServer;

        /// <summary>
        /// Used to receive data from Gateway. Uses EngineIP, GatewayPort from config.
        /// </summary>
        clsSocket _GatewayHeartBeatClient = new clsSocket();

        /// <summary>
        /// Used to send Heartbeat data to Prime. Uses HeartBeatPort from config.
        /// </summary>
        clsSocketSender _PrimeHeartBeatServer = new clsSocketSender();

        int MaxAllowedInstances = 0;
        string _ps03Filelocation = string.Empty;
        string _ConnectionString = string.Empty;//added by Navin on 10-06-2019
        #region Added by navin on 7-03-2019 for removing multiarray and feed receiver  
        static Feed objFeed = new Feed();
        bool _tradeBusy = false;

        string _EngineIP = "";// added by navin on 01-03-2019 for delegate
        int EnginePort;
        Dictionary<string, string[]> dict_Trades = new Dictionary<string, string[]>();
        //StringBuilder _tradesFlag = new StringBuilder();
        
        #endregion

        #region Amey Send receive data on same machine
        IXDBroadcaster bCast_EngineGateway;
        IXDListener listener;
        XDMessagingClient client;

        IXDListener Sender;
        XDMessagingClient clientSender;

        IXDListener spanListener;    //04-12-2019
        XDMessagingClient spanClient;//04-12-2019
        #endregion

        clsWriteLog objWriteLog;//object of write log added by navin on 28-06-2018
        bool getLtpFlag = true; //added on 11-01-18 by shri
        double cmTick = 0;//added by navin on 03-01-2018
        static string timeStamp;
        TcpListener myList;
        //static bool doengine = true;        //changed on 21-11-17

        MySqlConnection mySqlArrcsDBConn;
        DataSet ds_Engine = new DataSet();
        //DataTable tradesFromDB_Copy = new DataTable();  //added on 8-12-17 by shri
        DataTable dt_Day1Trades = new DataTable();

        //added default values on 01DEC2020 by Amey
        double interest, IVMultiplier = 2, IVDivisor = 2;
        List<string> IVValues = new List<string>();

        /// <summary>
        /// Key : UnderlyingName | Value : [0] IVMiddle, [1] IVLower, [2] IVHigher
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_DefaultIVs = new ConcurrentDictionary<string, double[]>();//navin 23-11-2017

        double IV = 0;
        public static int record = 0;
        //Thread t2;

        //Changed names on 29SEP2020 by Amey
        DataTable dt_TradesFromGateway = new DataTable();
        DataTable dt_EODTrades = new DataTable();

        /// <summary>
        /// Key : CustomScripName | Value : Token
        /// </summary>
        public static Dictionary<string, int> dict_Scrip_Token = new Dictionary<string, int>();   //changed on 9-1-18 by shri

        /// <summary>
        /// Key : CustomScripName | Value : Expiry in Unix
        /// </summary>
        public static Dictionary<string, double> dict_Scrip_Expiry = new Dictionary<string, double>();  //changed on 9-1-18 by shri

        /// <summary>
        /// Key : ScripName | Value : Token
        /// </summary>
        public static Dictionary<string, int> dict_OScrip_Token = new Dictionary<string, int>();  //changed on 9-1-18 by shri

        /// <summary>
        /// Key : ScripName | Value : Expiry in Unix
        /// </summary>
        public static Dictionary<string, double> dict_OScrip_Expiry = new Dictionary<string, double>(); //changed on 9-1-18 by shri

        //changed to double array on 15OCT2020 by Amey
        /// <summary>
        /// Key : Token | Value : [ LTP, 0/1(0 if LTP received is closing from yesterday, 1 if LTP is live) ]
        /// </summary>
        private ConcurrentDictionary<int, double[]> dict_LTP = new ConcurrentDictionary<int, double[]>();

        List<string> list_UpdateInterval = new List<string>();       //Added by Akshay on 31-12-2020 for storing time
        int _UpdateCount = 0;     //Added by Akshay on 31-12-2020 for storing time

        public static List<string> exceptionsList = new List<string>();
        public List<string> ExceptionsList { get { return exceptionsList; } set { exceptionsList = value; ; } }
        private static Mutex mutex = null;
        int _FeedCount = 0;//added by Navin on 17-10-2019
        public EngineProcess()
        {
            try
            {
                bool createdNew;
                mutex = new Mutex(true, "n.Engine", out createdNew);
                if (createdNew)
                {
                    InitializeComponent();
                    objWriteLog = new clsWriteLog(Application.StartupPath + "\\Log");//28-06-2018 for writting errorlog by navin

                    this.Hide();
                    dt_Day1Trades.Columns.Add("DealerID");
                    dt_Day1Trades.Columns.Add("TokenNo");
                    dt_Day1Trades.Columns.Add("FillPrice");
                    dt_Day1Trades.Columns.Add("ScripName");
                    dt_Day1Trades.Columns.Add("Underlying");
                    dt_Day1Trades.Columns.Add("OptionType");
                    dt_Day1Trades.Columns.Add("Expiry");
                    dt_Day1Trades.Columns.Add("StrikePrice");
                    dt_Day1Trades.Columns.Add("InstrumentName");
                    dt_Day1Trades.Columns.Add("FillQuantity");
                    dt_Day1Trades.Columns.Add("UnderlyingScripName");
                    dt_Day1Trades.Columns.Add("FlatScripName");
                    dt_Day1Trades.Columns.Add("CashToken");//added by Navin on 15-10-2019
                    timeStamp = DateTime.Now.AddHours(1).ToString();    //30-11-17 for heartbeat by navin
                    objWriteLog.WriteLog("Initialised " + DateTime.Now);

                    #region Amey
                    if (listener != null)
                        listener.Dispose();

                    client = new XDMessagingClient();
                    listener = client.Listeners.GetListenerForMode(XDTransportMode.HighPerformanceUI);
                    listener.MessageReceived += getTradesThroughSocket;
                    listener.RegisterChannel("All");

                    spanClient = new XDMessagingClient();//04-12-2019
                    spanListener = client.Listeners.GetListenerForMode(XDTransportMode.HighPerformanceUI);
                    spanListener.MessageReceived += getMarginThroughSocket;
                    spanListener.RegisterChannel("Span");

                    if (Sender != null)
                        Sender.Dispose();
                    clientSender = new XDMessagingClient();
                    Sender = client.Listeners.GetListenerForMode(XDTransportMode.HighPerformanceUI);
                    Sender.RegisterChannel("Engine-Gateway");
                    bCast_EngineGateway = clientSender.Broadcasters.GetBroadcasterForMode(XDTransportMode.HighPerformanceUI);
                    #endregion
                }
                else
                {
                    XtraMessageBox.Show("Engine already running");
                    Environment.Exit(0);
                }
            }
            catch (Exception ee)
            {
                if (!exceptionsList.Contains(ee.Message))
                {
                    InsertError("Engine start " + ee.ToString());
                }
            }
        }
        private void getTradesThroughSocket(object sender, XDMessageEventArgs e)
        {
            try
            {
                _tradeBusy = true;

                //changed on 06NOV2020 by Amey
                string _data = DecompressString(e.DataGram.Message);
                lock (dt_TradesFromGateway)
                {
                    dt_TradesFromGateway.Merge((DataTable)JsonConvert.DeserializeObject(_data.Substring(_data.IndexOf('_') + 1), dt_TradesFromGateway.GetType()));
                }

                //string _data = DecompressString(e.DataGram.Message);
                ////InsertError("Data received from gateway " + e.DataGram.Message.Length);//25-12-2019
                //if (_data.Substring(0, _data.IndexOf('_')) == "All")
                //{
                //    lock (dt_TradesFromGateway)
                //    {
                //        dt_TradesFromGateway.Clear();
                //        dt_TradesFromGateway.Merge((DataTable)JsonConvert.DeserializeObject(_data.Remove(0, 4), dt_TradesFromGateway.GetType()));
                //    }
                //    _tradesFlag.Append("All");

                //    //objWriteLog.WriteLog("ALL Trades Received," + tradesFromDB.Rows.Count);
                //}
                //else if (_data.Substring(0, _data.IndexOf('_')) == "Individual")
                //{
                //    lock (dt_TradesFromGateway)
                //    {
                //        dt_TradesFromGateway.Merge((DataTable)JsonConvert.DeserializeObject(_data.Remove(0, 11), dt_TradesFromGateway.GetType()));
                //    }

                //    //objWriteLog.WriteLog("Individual Trades Received," + tradesFromDB.Rows.Count);
                //}

                _tradeBusy = false;
            }
            catch (Exception error)
            {
                objWriteLog.WriteLog("Exception occurred while receiving trades from socket at " + DateTime.Now + Environment.NewLine + error.ToString());
            }
        }
        #region Span margin 04-12-2019
        Dictionary<string, double[]> dict_SpanMargin = new Dictionary<string, double[]>();

        /// <summary>
        /// Key : ClientID | Value : EOD Margin only.
        /// </summary>
        Dictionary<string, double[]> dict_EODMargin = new Dictionary<string, double[]>();

        //Added by Akshay on 10-12-2020 for New Span Margin
        Dictionary<string, double[]> dict_ExpirySpanMargin = new Dictionary<string, double[]>();

        private void getMarginThroughSocket(object sender, XDMessageEventArgs e)
        {
            try
            {
                if (!_spanFlag)
                {
                    _spanFlag = true;//05-12-2019
                    Dictionary<string, double[]> dict_SpanMargin_Copy = new Dictionary<string, double[]>();
                    string _data = DecompressString(e.DataGram.Message);

                    //added on 27NOV2020 by Amey
                    string[] arr_Fields = _data.Split('^');

                    if (arr_Fields[0] == "ALL")
                    {
                        dict_SpanMargin_Copy = ((Dictionary<string, double[]>)JsonConvert.DeserializeObject(arr_Fields[1], dict_SpanMargin.GetType()));
                        dict_SpanMargin = new Dictionary<string, double[]>(dict_SpanMargin_Copy);

                        //added on 18NOV2020 by Amey
                        objWriteLog.WriteLog("Span Received Rows," + dict_SpanMargin.Count(), isDebug);

                        //commented on 18NOV2020 by Amey. Was writing too much log.
                        //InsertError("Span received " + _data);//added by Navin on 10-02-2020
                    }

                    //Added by Akshay on 10-12-2020 for Expiry Span Margin
                    else if (arr_Fields[0] == "Expiry")
                    {
                        dict_ExpirySpanMargin = ((Dictionary<string, double[]>)JsonConvert.DeserializeObject(arr_Fields[1], dict_ExpirySpanMargin.GetType()));
                    }

                    else
                        dict_EODMargin = ((Dictionary<string, double[]>)JsonConvert.DeserializeObject(arr_Fields[1], dict_EODMargin.GetType()));

                    _spanFlag = false;//05-12-2019
                }
            }
            catch (Exception error)
            {
                objWriteLog.WriteLog("Exception occurred while receiving margin from socket at " + DateTime.Now + Environment.NewLine + error.ToString());
            }
        }
        #endregion
        private void Feed__Response4ScripFile(List<string[]> ScripFile)
        {
            throw new NotImplementedException();
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
                if (!exceptionsList.Contains(eq.Message))
                {
                    InsertError("Resize " + eq.ToString());
                }
            }
        }

        void SetColour()
        {
            try
            {
                btn_Start.Appearance.BackColor = Color.FromArgb(0x00, 0x73, 0xC4);
                btn_StopEngine.Appearance.BackColor = Color.FromArgb(0x00, 0x73, 0xC4);
            }
            catch (Exception colourEx)
            {
                InsertError(colourEx.ToString());
            }
        }

        #region MyRegion
        public void readLicenseFile()//(string scrip)
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

                    MaxAllowedInstances = Convert.ToInt32(arr_LicenseFields[4]);
                }
                else
                {
                    XtraMessageBox.Show("Invalid License. Please contact System Administrator.", "Error");
                    Environment.Exit(0);
                }
            }
            catch (Exception)
            {
                XtraMessageBox.Show("License file not found.");
                Environment.Exit(0);
            }
        }

        #endregion
        private async void EngineProcess_Load(object sender, EventArgs e)
        {
            try
            {
                readLicenseFile();
                #region Added by Navin on 17-10-2019 
                try
                {
                    //changed on 07JAN2021 by Amey
                    objFeed.eve_FO7208 += ObjFeed_eve_FO7208;

                    //added on 07JAN2021 by Amey
                    objFeed.eve_CM7208 += ObjFeed_eve_CM7208;

                    objFeed.eve_7202 += ObjFeed_eve_7202;
                    objFeed.eve_ConenctionMessage += ObjFeed_eve_ConenctionMessage;
                    objFeed.eve_PartialCandle += ObjFeed_eve_PartialCandle;

                    SetText("Connecting FO feed receiver");
                    await Task.Factory.StartNew(() => objFeed.FOConnect("ENGINE"));
                    SetText("Connecting CM feed receiver");
                    await Task.Factory.StartNew(() => objFeed.CMConnect("ENGINE"));
                    objWriteLog.WriteLog(" _Count " + _FeedCount);
                    if (_FeedCount == 2)
                    {
                        objWriteLog.WriteLog("Feed receiver connected ");
                        btn_Start.Enabled = true;
                        btnStartProcess.Enabled = true;
                        btnEditUploadClient.Enabled = true;
                        btnAddUser.Enabled = true;
                    }
                    else
                    {
                        XtraMessageBox.Show("Feed receiver not connected. Please start feed receiver and re-run this application");
                        Application.Exit();
                    }
                }
                catch (Exception feedEx)
                {
                    InsertError("Feed connect " + feedEx.ToString());
                }
                #endregion
                #region Initialising exception list
                //exceptionsList = new List<string>() { "Object reference not set to an instance of an object.", "Thread was being aborted.", "The source contains no DataRows.", "Cannot access disposed object." };
                #endregion
                objWriteLog.WriteLog("Entering startpage " + DateTime.Now);
                //System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity = (System.IntPtr)2;
                StartPage p = new StartPage(this);
                p.ShowDialog();
                if (p.dres == DialogResult.OK)
                {
                    objWriteLog.WriteLog("Entered " + DateTime.Now);
                    this.Show();
                }
                else
                {
                    objWriteLog.WriteLog("Exit " + DateTime.Now);
                    this.Close();
                    Application.Exit();
                }
                #region Commented by Navin on 16-10-2019 to mask span feature for edel release
                //th_SpanDownload = new Task(() => CreateSpanfile(filecount));//added by Navin on 03-10-2019 //commented by Navin on 10-10-2019 to mask span feature
                //th_SpanDownload.Start();
                //SetTimer(DateTime.Now.TimeOfDay.Add(TimeSpan.Parse("00:1:00")).ToString()); //commented by Navin on 10-10-2019 to mask span features 
                #endregion
                SkinManager.EnableFormSkins();
                SkinManager.EnableMdiFormSkins();

                try
                {
                    GetPrimeDBConnection();
                    _PrimeDataServer = new clsPrimeConnector();                                                                 //added by Navin on 04-02-2020
                    _PrimeDataServer.SetupServer(ip_EngineServer, PrimeDataPort, SpanDataPort, MaxAllowedInstances, Application.StartupPath);                //added by Navin on 04-02-2020

                    //objenginedll = new PrimeConnector(dt_Trades, _EngineIP, EnginePort,Application.StartupPath, _allowedInstance,_SpanPort);//commented by navin on 04-02-2020//added spanport on 14-10-2019

                    _PrimeHeartBeatServer.SetupServer(ip_EngineServer, PrimeHeartBeatPort);
                    //Task.Factory.StartNew(()=> objGatewaySocket.Connect("Engine", _EngineIP, _GatewayPort));
                    //objGatewaySocket._ConnectionResponse += GetConnectionStatus;
                    //objGatewaySocket._LastTradetimeResponse += GetLastTradeTime;
                    GetToken();
                }
                catch (Exception loadEx)
                {
                    if (!exceptionsList.Contains(loadEx.Message))
                    {
                        InsertError("Load " + loadEx.ToString());
                    }
                }

                ReadContractAndFill();

                //changed to Task on 06NOV2020 by Amey
                Task.Run(() => EngineProcessLinq());
            }
            catch (Exception ee)
            {
                if (!exceptionsList.Contains(ee.Message))
                {
                    InsertError("Engine Load " + ee.ToString());
                }
            }
        }

        private void ObjFeed_eve_PartialCandle(FirstPartialCandleEventArgs partialCandle)
        {

        }

        //private void ObjFeed_eve_ConenctionMessage(string status)
        //{
        //    SetText(status);//added by Navin on 17-10-2019
        //    if (status == "Successful FO" || status == "Successful CM")//added by Navin on 17-10-2019
        //        _FeedCount++;

        //}
        private void ObjFeed_eve_ConenctionMessage(string status)
        {
            try
            {
                //added on 14JAN2021 by Amey
                if (status.ToUpper().Contains("EXCEPTION"))
                {
                    objWriteLog.WriteLog($"ObjFeed_eve_ConenctionMessage : {status}");
                    SetText("Error occurred while receiving feed. Check logs for more details.");
                }
                else
                {
                    SetText(status);//added by Navin on 17-10-2019
                    if (status == "Successful FO" || status == "Successful CM")//added by Navin on 17-10-2019
                        _FeedCount++;

                    //added on 18JAN2021 by Amey
                    dict_LTP.Clear();
                }
            }
            catch (Exception ee) { objWriteLog.WriteLog($"ObjFeed_eve_ConenctionMessage Error : {ee}"); }

            //added for testing
            //if (_FeedCount == 1)
            //    Task.Delay(25000).ContinueWith((task) => { objFeed.FOSubscribe("123"); });
        }

        private void ObjFeed_eve_7202(TickerIndexEventArgs Token7202)
        {

        }

        /// <summary>
        /// <para>Key : CEToken</para>
        /// <para>Value : UnderlyingToken|Strike|Expiry|LTP</para>
        /// </summary>
        //ConcurrentDictionary<int, string> dict_CETokenInfo = new ConcurrentDictionary<int, string>();

        /// <summary>
        /// <para>Key : PEToken</para>
        /// <para>Value : UnderlyingToken|Strike|Expiry|LTP</para>
        /// </summary>
        //ConcurrentDictionary<int, string> dict_PETokenInfo = new ConcurrentDictionary<int, string>();

        /// <summary>
        /// Key : Token | Value : Greeks values received from FeedReceiver
        /// </summary>
        ConcurrentDictionary<int, Greeks> dict_Greeks = new ConcurrentDictionary<int, Greeks>();

        //private void ObjFeed_eve_7208(MBPEventArgs Token7208)
        //{
        //    try
        //    {
        //        //added on 18NOV2020 by Amey
        //        LTT = ConvertToUnixTimestamp(DateTime.Now);

        //        _PrimeHeartBeatServer.SendToPrimeViaSocket(LTT + "_" + _LastTradeTime + "_" + _GatewayStatus + "_" + _SpanStatus);
                
        //        int Token = Convert.ToInt32(Token7208.Token);
        //        double LTP = Convert.ToDouble(Token7208.LastTradedPrice);
        //        if (dict_LTP.ContainsKey(Token))
        //        {
        //            dict_LTP[Token][0] = LTP;

        //            //added on 15OCT2020 by Amey
        //            //To identify whether received packet is Live or From Bhavcopy.
        //            if (Token7208.LastTradeQuantity == "0")
        //                dict_LTP[Token][1] = dict_LTP[Token][1] == 1 ? 1 : 0;
        //            else
        //                dict_LTP[Token][1] = 1;
        //        }

        //        //added on 9OCT2020 by Amey
        //        if (dict_XXLTP.ContainsKey(Token))
        //        {
        //            dict_XXLTP[Token][0] = LTP;

        //            //added on 15OCT2020 by Amey
        //            //To identify whether received packet is Live or From Bhavcopy.
        //            if (Token7208.LastTradeQuantity == "0")
        //                dict_XXLTP[Token][1] = dict_XXLTP[Token][1] == 1 ? 1 : 0;
        //            else
        //                dict_XXLTP[Token][1] = 1;
        //        }

        //        if (dict_Greeks.ContainsKey(Token))
        //        {
        //            if (Token7208.IV != "-")
        //            {
        //                //added Rounding on 01DEC2020 by Amey
        //                double ReceivedIV = Math.Round(Convert.ToDouble(Token7208.IV), 2);
        //                double Lower = ReceivedIV / IVDivisor;
        //                double Higher = ReceivedIV * IVMultiplier;

        //                //added default values in case numbers are very close to 0;
        //                dict_Greeks[Token].IVLower = Lower <= 0.01 ? 15 : Lower;
        //                dict_Greeks[Token].IV = ReceivedIV <= 0.01 ? 30 : ReceivedIV;
        //                dict_Greeks[Token].IVHigher = Higher <= 0.01 ? 45 : Higher;

        //                dict_Greeks[Token].IsReceived = true;
        //            }
        //            if (Token7208.Delta != "-")
        //                dict_Greeks[Token].Delta = Convert.ToDouble(Token7208.Delta);
        //            if (Token7208.Gamma != "-")
        //                dict_Greeks[Token].Gamma = Convert.ToDouble(Token7208.Gamma);
        //            if (Token7208.Theta != "-")
        //                dict_Greeks[Token].Theta = Convert.ToDouble(Token7208.Theta);
        //            if (Token7208.Vega != "-")
        //                dict_Greeks[Token].Vega = Convert.ToDouble(Token7208.Vega);
        //        }
        //    }
        //    catch (Exception ltpEx)
        //    {
        //        objWriteLog.WriteLog("Receive LTP :" + ltpEx.Message.ToString());
        //    }
        //}

        #region Gateway Connectivity & Last trade time
        void GetLastTradeTime(double prmTime)
        {
            try
            {
                _LastTradeTime = prmTime;
                _GatewayStatus = 1;
            }
            catch (Exception error)
            {
                InsertError("GetLastTradeTime " + error.ToString());
            }
        }
        void GetConnectionStatus(string prmStatus)
        {
            try
            {
                InsertError("Gateway connection status " + prmStatus);
                if (prmStatus.ToLower() == "connected")
                    _GatewayStatus = 1;
                else
                {
                    _GatewayStatus = 0;
                    _SpanStatus = 0;//06-02-2020
                }
            }
            catch (Exception error)
            {
                InsertError("GetConnectionStatus " + error.ToString());
            }
        }
        #region added by Navin on 05-02-2020
        int _SpanStatus = 0;
        void GetSpanStatus(string prmStatus)
        {
            try
            {
                //InsertError("Span status " + prmStatus);
                if (prmStatus.ToLower() == "true")
                    _SpanStatus = 1;
                else
                    _SpanStatus = 0;
            }
            catch (Exception error)
            {
                InsertError("GetSpanStatus " + error.ToString());
            }
        }
        #endregion
        #endregion

        //added on 07JAN2021 by Amey
        private void ObjFeed_eve_FO7208(MBPEventArgs Token7208)
        {
            try
            {
                //added on 18NOV2020 by Amey
                FOLTT = ConvertToUnixTimestamp(DateTime.Now);

                _PrimeHeartBeatServer.SendToPrimeViaSocket(FOLTT + "_" + CMLTT + "_" + _LastTradeTime + "_" + _GatewayStatus + "_" + _SpanStatus);

                int Token = Convert.ToInt32(Token7208.Token);
                double LTP = Convert.ToDouble(Token7208.LastTradedPrice);
                if (dict_LTP.ContainsKey(Token))
                {
                    dict_LTP[Token][0] = LTP;

                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (Token7208.LastTradeQuantity == "0")
                        dict_LTP[Token][1] = dict_LTP[Token][1] == 1 ? 1 : 0;
                    else
                        dict_LTP[Token][1] = 1;
                }

                //added on 9OCT2020 by Amey
                if (dict_XXLTP.ContainsKey(Token))
                {
                    dict_XXLTP[Token][0] = LTP;

                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (Token7208.LastTradeQuantity == "0")
                        dict_XXLTP[Token][1] = dict_XXLTP[Token][1] == 1 ? 1 : 0;
                    else
                        dict_XXLTP[Token][1] = 1;
                }

                if (dict_Greeks.ContainsKey(Token))
                {
                    if (Token7208.IV != "-")
                    {
                        //added Rounding on 01DEC2020 by Amey
                        double ReceivedIV = Math.Round(Convert.ToDouble(Token7208.IV), 2);
                        double Lower = ReceivedIV / IVDivisor;
                        double Higher = ReceivedIV * IVMultiplier;

                        //added default values in case numbers are very close to 0;
                        dict_Greeks[Token].IVLower = Lower <= 0.01 ? 15 : Lower;
                        dict_Greeks[Token].IV = ReceivedIV <= 0.01 ? 30 : ReceivedIV;
                        dict_Greeks[Token].IVHigher = Higher <= 0.01 ? 45 : Higher;

                        dict_Greeks[Token].IsReceived = true;
                    }
                    if (Token7208.Delta != "-")
                        dict_Greeks[Token].Delta = Convert.ToDouble(Token7208.Delta);
                    if (Token7208.Gamma != "-")
                        dict_Greeks[Token].Gamma = Convert.ToDouble(Token7208.Gamma);
                    if (Token7208.Theta != "-")
                        dict_Greeks[Token].Theta = Convert.ToDouble(Token7208.Theta);
                    if (Token7208.Vega != "-")
                        dict_Greeks[Token].Vega = Convert.ToDouble(Token7208.Vega);
                }
            }
            catch (Exception ltpEx)
            {
                objWriteLog.WriteLog("Receive FO LTP :" + ltpEx.Message.ToString());
            }
        }

        //added on 07JAN2021 by Amey
        private void ObjFeed_eve_CM7208(MBPEventArgs Token7208)
        {
            try
            {
                //added on 18NOV2020 by Amey
                CMLTT = ConvertToUnixTimestamp(DateTime.Now);

                //changed on 07JAN2021 by Amey
                _PrimeHeartBeatServer.SendToPrimeViaSocket(FOLTT + "_" + CMLTT + "_" + _LastTradeTime + "_" + _GatewayStatus + "_" + _SpanStatus);

                int Token = Convert.ToInt32(Token7208.Token);
                double LTP = Convert.ToDouble(Token7208.LastTradedPrice);
                if (dict_LTP.ContainsKey(Token))
                {
                    dict_LTP[Token][0] = LTP;

                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (Token7208.LastTradeQuantity == "0")
                        dict_LTP[Token][1] = dict_LTP[Token][1] == 1 ? 1 : 0;
                    else
                        dict_LTP[Token][1] = 1;
                }

                //added on 9OCT2020 by Amey
                if (dict_XXLTP.ContainsKey(Token))
                {
                    dict_XXLTP[Token][0] = LTP;

                    //added on 15OCT2020 by Amey
                    //To identify whether received packet is Live or From Bhavcopy.
                    if (Token7208.LastTradeQuantity == "0")
                        dict_XXLTP[Token][1] = dict_XXLTP[Token][1] == 1 ? 1 : 0;
                    else
                        dict_XXLTP[Token][1] = 1;
                }
            }
            catch (Exception ltpEx)
            {
                objWriteLog.WriteLog("Receive CM LTP :" + ltpEx.Message.ToString());
            }
        }

        public void GetPrimeDBConnection()
        {
            try
            {
                XmlTextReader tReader = new XmlTextReader("C:/Prime/PrimeDBConnection.xml");
                tReader.Read();
                ds_Engine.ReadXml(tReader);
                if (ds_Engine.Tables[0].Rows.Count > 0)
                {
                    _ConnectionString = "Data Source = " + ds_Engine.Tables[0].Rows[0]["Server"] + "; Port = " + ds_Engine.Tables[0].Rows[0]["Port"] + "; Initial Catalog = " + ds_Engine.Tables[0].Rows[0]["Database"] + "; user ID = " + ds_Engine.Tables[0].Rows[0]["user"] + "; Password = " + ds_Engine.Tables[0].Rows[0]["password"] + " ;SslMode=none ;default command timeout=100;";//added by Navin on 10-06-2019
                    mySqlArrcsDBConn = new MySqlConnection("Data Source = " + ds_Engine.Tables[0].Rows[0]["Server"] + "; Port = " + ds_Engine.Tables[0].Rows[0]["Port"] + "; Initial Catalog = " + ds_Engine.Tables[0].Rows[0]["Database"] + "; user ID = " + ds_Engine.Tables[0].Rows[0]["user"] + "; Password = " + ds_Engine.Tables[0].Rows[0]["password"] + " ;SslMode=none ;default command timeout=100;");
                }
                if (ds_Engine.Tables[1].Rows.Count > 0)
                {
                    //added on 21DEC2020 by Amey
                    ip_EngineServer = IPAddress.Parse(ds_Engine.Tables["ConnectionIPs"].Rows[0]["ENGINE-SERVER-IP"].ToString());

                    PrimeDataPort = Convert.ToInt32(ds_Engine.Tables[1].Rows[0]["PrimePort"].ToString());//added by navin on 04-02-2020
                    _EngineIP = ds_Engine.Tables[1].Rows[0]["EngineIP"].ToString();//added by navin on 06-03-2019 for delegate
                    EnginePort = Convert.ToInt32(ds_Engine.Tables[1].Rows[0]["EnginePort"].ToString());//added by navin on 06-03-2019 for delegate
                    _GatewayPort = Convert.ToInt32(ds_Engine.Tables[1].Rows[0]["GatewayPort"].ToString());//added by navin on 06-03-2019 for delegate
                    PrimeHeartBeatPort = Convert.ToInt32(ds_Engine.Tables[1].Rows[0]["HeartbeatPort"].ToString());//added by navin on 06-03-2019 for delegate
                    SpanDataPort = Convert.ToInt32(ds_Engine.Tables[1].Rows[0]["SpanPort"].ToString());//added by navin on 14-10-2019 for delegate
                    _SpanInterval = Convert.ToInt32(ds_Engine.Tables[1].Rows[0]["SpanInterval"].ToString());//added by navin on 14-10-2019 for delegate
                }
                try
                {
                    _ps03Filelocation = ds_Engine.Tables[2].Rows[0]["Path"].ToString();//added by navin on 06-03-2019 for delegate
                }
                catch (Exception)
                {
                    SetText("PS03 file not specified");
                }

                //Added by Akshay on 31-12-2020 for VAREQ 
                for (int i = 7; i <= 12; i++)
                {
                    list_UpdateInterval.Add(ds_Engine.Tables[2].Rows[0][i].ToString());
                }

                try//added by Navin on 17-01-2020
                {
                    //_sentimentPort = Convert.ToInt32(ds_engine.Tables[1].Rows[0]["SentimentPort"].ToString());
                    //_sentimentIP = ds_engine.Tables[1].Rows[0]["SentimentIP"].ToString();
                }
                catch (Exception)
                {
                    SetText("Sentiment config not specified");
                }
            }
            catch (Exception arrcsDbEx)
            {
                if (!exceptionsList.Contains(arrcsDbEx.Message))
                {
                    InsertError("Read connection " + arrcsDbEx.ToString());
                }
            }
        }

        StringBuilder sb_Client = new StringBuilder();

        public void EngineProcessLinq()   //Main Engine Processes
        {
            try
            {
                ds_Engine.Tables[0].Clear();

                #region added by navin on 20-03-2019
                if (!ds_Engine.Tables.Contains("tbl_AllTrades"))
                {
                    ds_Engine.Tables.Add(dt_TradesFromGateway.Clone());
                    ds_Engine.Tables[3].TableName = "tbl_AllTrades";
                    ds_Engine.Tables[3].Merge(dt_EODTrades);
                }
                if (!ds_Engine.Tables.Contains("tbl_EODTrades"))
                {
                    ds_Engine.Tables.Add(dt_EODTrades);    //EOD Data
                    ds_Engine.Tables[4].TableName = "tbl_EODTrades";
                }
                #endregion

                while (true)
                {
                    //added on 06NOV2020 by Amey
                    while (isEngineStarted)
                    {
                        //Added by Akshay on 31-12-2020 For VAREQ read
                        if (_UpdateCount < list_UpdateInterval.Count)
                            ReadVAR_EQ(list_UpdateInterval[_UpdateCount]);

                        ReadInterestRate();
                        ConsolidateAndSend();

                        //added on 16DEC2020 by Amey
                        Thread.Sleep(50);
                    }

                    Thread.Sleep(500);
                }
            }
            catch (Exception procEx)
            {
                if (!exceptionsList.Contains(procEx.Message))
                {
                    InsertError("Process " + procEx.ToString());
                }
            }
        }

        #region Read Interest Rate and IV Multiplier,Divisor
        public void ReadInterestRate()
        {
            try
            {
                using (FileStream stream2 = File.Open("C:/Prime/intrestrate.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr2 = new StreamReader(stream2))
                    {
                        string line1;
                        while ((line1 = sr2.ReadLine()) != null)
                        {
                            try
                            {
                                string[] fields = line1.Split(',');
                                interest = Convert.ToDouble(fields[0]) + Convert.ToDouble(fields[1]);
                                IVMultiplier = Convert.ToDouble(fields[2]);
                                IVDivisor = Convert.ToDouble(fields[3]);
                            }
                            catch (Exception ee) { InsertError("ReadInterestRate Loop : " + line1 + Environment.NewLine + ee.ToString()); }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //if (!exceptionsList.Contains(ex.Message))
                {
                    InsertError("Intraest " + ex.ToString());
                }
            }
        }
        #endregion

        #region getEODData

        /// <summary>
        /// Key : Client_Scripname | Value : [0] = FillQty, [1] = FillPrice
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_CFInfo = new ConcurrentDictionary<string, double[]>();

        public void GetEODData()
        {
            try
            {
                dt_EODTrades.Clear();
                using (MySqlConnection myConnEod = new MySqlConnection(_ConnectionString))
                {
                    MySqlCommand myCmdEod = new MySqlCommand("sp_SelectEodData", myConnEod);
                    myCmdEod.CommandType = CommandType.StoredProcedure;
                    myConnEod.Open();
                    MySqlDataAdapter dadapter = new MySqlDataAdapter(myCmdEod);
                    //DataTable dtTest = new DataTable();
                    dadapter.Fill(dt_EODTrades);
                    isEOD = true;
                    flag_NewTrades = true;
                    dadapter.Dispose();
                    myConnEod.Close();
                }

                //added on 10OCT2020 by Amey
                for (int i = 0; i < dt_EODTrades.Rows.Count; i++)
                    dict_CFInfo.TryAdd(dt_EODTrades.Rows[i]["DealerID"] + "_" + dt_EODTrades.Rows[i]["ScripName"], new double[2] { Convert.ToDouble(dt_EODTrades.Rows[i]["FillQuantity"]), Convert.ToDouble(dt_EODTrades.Rows[i]["FillPrice"]) });
            }
            catch (Exception getDataEx)
            {
                if (!exceptionsList.Contains(getDataEx.Message))
                {
                    InsertError("Eod Data : " + getDataEx.ToString());
                }
            }
        }
        #endregion

        #region LINQ QUERY AND MULTIARRAY SECTION
        bool flag_NewTrades = true;
        bool isEOD = true;
        bool isDebug = true;

        /// <summary>
        /// Set true when clicked on Start button. Set false when clicked on Stop button.
        /// </summary>
        bool isEngineStarted = false;

        /// <summary>
        /// set True when clicked on Start button, set False after clearing tbl_AllTrades.
        /// </summary>
        bool StartFromZero = false;

        /// <summary>
        /// Key : ID | Value : Consolidated trades of respective ID
        /// </summary>
        Dictionary<string, GroupsTabs.ConsolidateTradeinfoDataTable> dict_ConsolidatedPos = new Dictionary<string, GroupsTabs.ConsolidateTradeinfoDataTable>();

        public void ConsolidateAndSend()
        {
            try
            {
                //GroupsTabs.ConsolidateTradeinfoDataTable dt_Trades = new GroupsTabs.ConsolidateTradeinfoDataTable();

                lock (dt_TradesFromGateway)
                {
                    //added StartFromZero condition on 10NOV2020 by Amey. To read EOD trades and send.
                    if ((dt_TradesFromGateway.Rows.Count > 0 && !_tradeBusy) || ds_Engine.Tables["tbl_AllTrades"].Rows.Count > 0 || StartFromZero)// added _tradeBusy by Navin on 16-04-2019
                    {
                        if (dt_TradesFromGateway.Rows.Count == 0)
                        {
                            if (isEOD)
                                isEOD = false;
                            else
                                flag_NewTrades = false;
                        }
                        else
                            flag_NewTrades = true;

                        //changed on 06NOV2020 by Amey
                        if (StartFromZero)
                        {
                            objWriteLog.WriteLog("StartFromZero," + dt_TradesFromGateway.Rows.Count, isDebug);
                            objWriteLog.WriteLog("EOD," + dt_EODTrades.Rows.Count, isDebug);

                            ds_Engine.Tables["tbl_AllTrades"].Clear();
                            ds_Engine.Tables["tbl_AllTrades"].Merge(dt_EODTrades);

                            //added on 06NOV2020 by Amey
                            StartFromZero = false;

                            flag_NewTrades = true;
                        }

                        if (flag_NewTrades)
                            objWriteLog.WriteLog("Trades From Gateway," + dt_TradesFromGateway.Rows.Count, isDebug);

                        //DataTable tradesFromDB_Copy = tradesFromDB.Clone();
                        //tradesFromDB_Copy.Merge(tradesFromDB);

                        //if (flagNewTrades)
                        //    objWriteLog.WriteLog("Trades Datatable Copy," + tradesFromDB_Copy.Rows.Count, isDebug);

                        //tradesFromDB.Clear();

                        ds_Engine.Tables["tbl_AllTrades"].Merge(dt_TradesFromGateway.Copy());
                        dt_TradesFromGateway.Clear();

                        if (flag_NewTrades)
                            objWriteLog.WriteLog("Before Consolidate," + ds_Engine.Tables["tbl_AllTrades"].Rows.Count, isDebug);

                        //var ssd = ds_engine.Tables["tradesFromDB"].Select("DealerID = '" + "CT038" + "' AND Underlying = '" + "PNB" + "'");

                        int ConsolidatedTradesCount = 0;
                        if (flag_NewTrades)
                        {
                            //dt_Trades.Clear();

                            //added on 20NOV2020 by Amey
                            dict_ConsolidatedPos.Clear();

                            var result = ds_Engine.Tables["tbl_AllTrades"].AsEnumerable()
                                         .Select(
                                                x => new
                                                {
                                                    Client = x["DealerID"],
                                                    TokenNumber = x["TokenNo"],
                                                    FillPrice = x["FillPrice"],
                                                    ScripName = x["ScripName"],
                                                    Underlying = x["Underlying"],
                                                    OptionType = x["OptionType"],
                                                    Expiry = x["Expiry"],
                                                    StrikePrice = x["StrikePrice"],
                                                    InstrumentName = x["InstrumentName"],
                                                    FillQuantity = x["FillQuantity"],
                                                    UnderlyingScripName = x["UnderlyingScripName"],
                                                    FlatUnderlyingScripName = x["FlatUnderlyingScripName"],
                                                    DayNet = x["DayQty"],
                                                    DayAmt = x["DayAmt"],
                                                    Cash = x["CashToken"],
                                                    DateTime = x["DateTime"]
                                                }
                                                )
                                                .GroupBy(s => new { s.Client, s.TokenNumber })
                                                .Select(
                                                        g => new
                                                        {
                                                            Client = g.Select(x => x.Client).First().ToString(),
                                                            ScriptToken = g.Select(x => x.TokenNumber).First().ToString(),
                                                            ScriptName = g.Select(x => x.ScripName).First().ToString(),
                                                            Underlying = g.Select(x => x.Underlying).First().ToString(),
                                                            OptionType = g.Select(x => x.OptionType).First().ToString(),
                                                            Expiry = g.Select(x => x.Expiry).First().ToString(),
                                                            StrikePrice = g.Select(x => x.StrikePrice).First().ToString(),
                                                            InstrumentName = g.Select(x => x.InstrumentName).First().ToString(),
                                                            UnderlyingScriptName = g.Select(x => x.UnderlyingScripName).First().ToString(),
                                                            FlatUnderlyingScripName = g.Select(x => x.FlatUnderlyingScripName).First().ToString(),
                                                            BEP = Math.Round(Convert.ToDouble(g.Sum(x => Convert.ToDecimal(x.FillQuantity) * Convert.ToDecimal(x.FillPrice)) / ((g.Sum(x => Convert.ToDecimal(x.FillQuantity))) == 0 ? -1 : (g.Sum(x => Convert.ToInt64(x.FillQuantity))))), 2),//added on 25_08_16
                                                            NetPos = g.Sum(x => Convert.ToInt64(x.FillQuantity)),
                                                            IntradayNetPosition = g.Sum(x => Convert.ToInt64(x.DayNet)),
                                                            IntradayNetValue = g.Sum(x => Convert.ToDouble(x.DayAmt)),
                                                            CashToken = g.Select(x => Convert.ToInt32(x.Cash)).First(),
                                                            DateTime = g.Select(x => x.DateTime).First().ToString()
                                                        }
                                                    )
                                                //.OrderByDescending(x => x.Client).ThenByDescending(x => x.Underlying))
                                                .ToList();

                            GroupsTabs.ConsolidateTradeinfoRow dr_Trades;

                            //added on 20NOV2020 by Amey
                            ConsolidatedTradesCount = result.Count();

                            foreach (var Position in result)
                            {
                                try
                                {
                                    //added on 20NOV2020 by Amey
                                    if (dict_ConsolidatedPos.ContainsKey(Position.Client))
                                        dr_Trades = dict_ConsolidatedPos[Position.Client].NewConsolidateTradeinfoRow();
                                    else
                                    {
                                        GroupsTabs.ConsolidateTradeinfoDataTable dt_Trades = new GroupsTabs.ConsolidateTradeinfoDataTable();
                                        dict_ConsolidatedPos.Add(Position.Client, dt_Trades);
                                        dr_Trades = dt_Trades.NewConsolidateTradeinfoRow();
                                    }

                                    dr_Trades.Client = Position.Client;
                                    dr_Trades.OptionType = Position.OptionType;
                                    dr_Trades.ScripName = Position.ScriptName;
                                    dr_Trades.InstrumentName = Position.InstrumentName;
                                    dr_Trades.Underlying = Position.Underlying;
                                    dr_Trades.NetPosition = Convert.ToInt64(Position.NetPos);

                                    //if (flag_NewTrades)
                                    //    objWriteLog.WriteLog("Client," + Position.Client + ",Scripname," + Position.ScriptName + ",NetPos," + Convert.ToDouble(Position.NetPos), isDebug);

                                    //added on 10OCT2020 by Amey
                                    string EODKey = Position.Client + "_" + Position.ScriptName;
                                    if (dict_CFInfo.ContainsKey(EODKey))
                                    {
                                        dr_Trades.NetPositionCF = Convert.ToInt64(dict_CFInfo[EODKey][0]);
                                        dr_Trades.PriceCF = dict_CFInfo[EODKey][1];
                                    }

                                    dr_Trades.BEP = Convert.ToDouble(Position.BEP);
                                    dr_Trades.ScripToken = Convert.ToInt32(Position.ScriptToken);
                                    dr_Trades.FlatUnderlyingScripName = Position.FlatUnderlyingScripName;

                                    //added on 23NOV2020 by Amey
                                    dr_Trades.IntradayBEP = Position.IntradayNetValue / (Position.IntradayNetPosition == 0 ? -1 : Position.IntradayNetPosition);

                                    //changed default values on 19NOV2020 by Amey
                                    dr_Trades.UnderlyingLtp = -1;
                                    dr_Trades.ScripLtp = -1;

                                    if (Position.OptionType == "EQ")
                                    {
                                        dr_Trades.UnderlyingToken = Convert.ToInt32(Position.ScriptToken);
                                        dr_Trades.UnderlyingFuture = Position.ScriptName;
                                        dr_Trades.StrikePrice = 0;
                                        dr_Trades.UnderlyingExpiry = ConvertToUnixTimestamp(Convert.ToDateTime("01-01-1980"));
                                        dr_Trades.ScripExpiry = ConvertToUnixTimestamp(Convert.ToDateTime("01-01-1980"));
                                    }
                                    else
                                    {
                                        dr_Trades.UnderlyingFuture = Position.UnderlyingScriptName;

                                        //added on 18NOV2020 by Amey
                                        int DefaultVal = Position.CashToken;
                                        dr_Trades.UnderlyingToken = dict_Scrip_Token.TryGetValue(Position.FlatUnderlyingScripName, out DefaultVal) || dict_OScrip_Token.TryGetValue(Position.UnderlyingScriptName, out DefaultVal)
                                            ? DefaultVal
                                            : DefaultVal;

                                        //added on 18NOV2020 by Amey
                                        double dbl_DefaultVal = 0;
                                        dr_Trades.UnderlyingExpiry = dict_Scrip_Expiry.TryGetValue(Position.FlatUnderlyingScripName, out dbl_DefaultVal) || dict_OScrip_Expiry.TryGetValue(Position.UnderlyingScriptName, out dbl_DefaultVal)
                                           ? dbl_DefaultVal
                                           : dbl_DefaultVal;

                                        //added on 19NOV2020 by Amey
                                        if (DefaultVal == Position.CashToken)
                                            dr_Trades.IsLDO = true;

                                        dr_Trades.StrikePrice = Convert.ToDouble(Position.StrikePrice);
                                        //dr_Trades.UnderlyingExpiry = item.Expiry;
                                        dr_Trades.ScripExpiry = Convert.ToDouble(Position.Expiry);
                                    }

                                    dr_Trades.IntradayNetPosition = Position.IntradayNetPosition;//added by Navin on 27-05-2019

                                    //changed location on 30OCT2020 by Amey
                                    dr_Trades.IntradayNetValue = Position.IntradayNetValue;

                                    //commented on 30OCT2020 by Amey
                                    //if (item.DayNetQty == 0)
                                    //    dr_Trades.DayAmt = 0;//added by Navin on 20-06-2019
                                    //else
                                    //    dr_Trades.DayAmt = item.DayNetAmt;//added by Navin on 20-06-2019

                                    dr_Trades.CashToken = Position.CashToken;//added by Navin on 15-10-2019

                                    //added on 19NOV2020 by Amey
                                    int ScripToken = Convert.ToInt32(Position.ScriptToken);
                                    AssignLTPs(ref dr_Trades, ScripToken);

                                    double BetaValue = 1;
                                    dict_BetaValue.TryGetValue(dr_Trades.Underlying, out BetaValue);
                                    AssignGreeks(ref dr_Trades, ScripToken, BetaValue);

                                    //Added by Akshay on 31-12-2020 For VAREQ
                                    if (dr_Trades.OptionType == "EQ" && dict_VARMargin.ContainsKey(dr_Trades.ScripName))
                                        dr_Trades.VARMargin = Math.Abs(Math.Round(dr_Trades.IntradayNetPosition * dr_Trades.ScripLtp * (dict_VARMargin[dr_Trades.ScripName] / 100), 2));

                                    //added on 20NOV2020 by Amey
                                    dict_ConsolidatedPos[dr_Trades.Client].Rows.Add(dr_Trades);

                                    //dt_Trades.Rows.Add(dr_Trades);
                                }
                                catch (Exception innerLinq)
                                {
                                    InsertError("Inner Linq Loop : " + innerLinq.ToString());
                                }
                            }
                        }
                        else
                        {
                            GroupsTabs.ConsolidateTradeinfoRow dr_Trades;

                            //changed logic on 20NOV2020 by Amey
                            for (int CIdx = 0; CIdx < dict_ConsolidatedPos.Keys.Count; CIdx++)
                            {
                                string ClientID = dict_ConsolidatedPos.ElementAt(CIdx).Key;
                                try
                                {
                                    for (int TIdx = 0; TIdx < dict_ConsolidatedPos[ClientID].Rows.Count; TIdx++)
                                    {
                                        dr_Trades = (GroupsTabs.ConsolidateTradeinfoRow)dict_ConsolidatedPos[ClientID].Rows[TIdx];
                                        try
                                        {
                                            //added on 19NOV2020 by Amey
                                            int ScripToken = Convert.ToInt32(dr_Trades.ScripToken);
                                            AssignLTPs(ref dr_Trades, ScripToken);

                                            double BetaValue = 1;
                                            dict_BetaValue.TryGetValue(dr_Trades.Underlying, out BetaValue);
                                            AssignGreeks(ref dr_Trades, ScripToken, BetaValue);
                                        }
                                        catch (Exception ee) { InsertError("Linq Loop Trades : " + ee.ToString()); }
                                    }
                                }
                                catch(Exception ee) { InsertError("Linq Loop -Outer Trades : " + ee.ToString()); }
                            }
                        }

                        if (flag_NewTrades)
                            objWriteLog.WriteLog("After Consolidate," + ConsolidatedTradesCount, isDebug);
                    }
                }

                //var re = dt_Trades.Select("Client = '" + "CT038" + "'");

                //GetTokenLtp();
                //AdjustLtp();
                //CalculateIV();

                SendDataToPrime(dict_ConsolidatedPos);
            }
            catch (InvalidCastException ee)
            {
                InsertError("Linq Invalid Data : " + JsonConvert.SerializeObject(ds_Engine.Tables["tbl_AllTrades"]) + Environment.NewLine + ee.ToString());
            }
            catch (Exception linqEx)
            {
                //if (!exceptionsList.Contains(linqEx.Message))
                {
                    InsertError("Linq : " + linqEx.ToString());
                }
            }
        }

        private void AssignLTPs(ref GroupsTabs.ConsolidateTradeinfoRow dr_Trade, int ScripToken)
        {
            try
            {
                string Underlying = dr_Trade.Underlying;
                //added on 7OCT2020 by Amey
                if (dict_UnderlyingToken.ContainsKey(Underlying))
                {
                    int CurrentMonthUnderlyingToken = dict_UnderlyingToken[Underlying];
                    if (!dict_LTP.ContainsKey(CurrentMonthUnderlyingToken))
                    {
                        dict_LTP.TryAdd(CurrentMonthUnderlyingToken, new double[2] { -1, 0 });
                        objFeed.FOSubscribe(CurrentMonthUnderlyingToken.ToString());

                        Task.Run(() => SetAvgIV(CurrentMonthUnderlyingToken, Underlying));
                    }
                }

                //added else condition on 18NOV2020 by Amey
                if (!dict_LTP.ContainsKey(ScripToken))
                {
                    objFeed.Subscribe(ScripToken.ToString());

                    dict_LTP.TryAdd(ScripToken, new double[2] { -1, 0 });
                }
                else if (dict_LTP[ScripToken][0] != -1)
                {
                    dr_Trade.ScripLtp = dict_LTP[ScripToken][0];

                    dr_Trade.IsLTPCalculated = false;
                }

                //added on 18NOV2020 by Amey
                int UnderlyingToken = dr_Trade.UnderlyingToken;
                string UnderlyingFut = dr_Trade.UnderlyingFuture;
                if (!dict_LTP.ContainsKey(UnderlyingToken) || (dict_LTP.ContainsKey(UnderlyingToken) && dict_LTP[UnderlyingToken][0] == -1))
                {
                    if (!dict_LTP.ContainsKey(UnderlyingToken))
                        objFeed.Subscribe(UnderlyingToken.ToString());

                    var closingPrice = clsWriteLog.Closing.AsEnumerable().Where(r => r.Field<string>("Scrip") == UnderlyingFut).Select(r => r.Field<string>("Closing")).FirstOrDefault();
                    if (closingPrice != null)
                    {
                        var UnderlyingPrice = Convert.ToDouble(closingPrice);
                        dict_LTP.TryAdd(UnderlyingToken, new double[2] { UnderlyingPrice, 0 }); //, (oldKey, oldVal) => new double[2] { UnderlyingPrice, 0 });

                        dr_Trade.UnderlyingLtp = dict_LTP[UnderlyingToken][0];
                    }
                    else
                        dict_LTP.TryAdd(UnderlyingToken, new double[2] { -1, 0 });
                }
                else
                    dr_Trade.UnderlyingLtp = dict_LTP[UnderlyingToken][0];

                if (!dict_LTP.ContainsKey(dr_Trade.CashToken))//added by Navin on 15-10-2019
                {
                    objFeed.Subscribe(dr_Trade.CashToken.ToString());

                    dict_LTP.TryAdd(dr_Trade.CashToken, new double[2] { -1, 0 });
                }

                //changed location on 03DEC2020 by Amey
                var TimeToExpiry = ConvertFromUnixTimestamp(dr_Trade.ScripExpiry).AddHours(15).AddMinutes(30) - DateTime.Now;

                //added on 19NOV2020 by Amey
                if (dr_Trade.IsLDO && (dict_LTP[UnderlyingToken][0] != -1))
                    dr_Trade.UnderlyingLtp = ((dict_LTP[UnderlyingToken][0] * interest * TimeToExpiry.TotalDays) / 36500) + dict_LTP[UnderlyingToken][0];

                //added OPTIONS check on 01DEC2020 by Amey
                string OptionType = dr_Trade.OptionType;
                if (dict_ComputedATMIV.ContainsKey(Underlying) && (OptionType.Equals("CE") || OptionType.Equals("PE")))
                {
                    double UnderlyingLTP = dr_Trade.UnderlyingLtp;
                    double StrikePrice = dr_Trade.StrikePrice;
                    double ScripLTP = dr_Trade.ScripLtp;

                    dr_Trade.ATM_IV = dict_ComputedATMIV[Underlying][0];

                    //added on 12OCT2020 by Amey
                    //changed condition on 15OCT2020 by Amey
                    //changed condition on 27NOV2020 by Amey
                    if (dict_LTP[ScripToken][0] == -1 && dict_ComputedATMIV[Underlying][1] == 1)
                    {
                        if (OptionType.Equals("CE"))
                            ScripLTP = CallOption(UnderlyingLTP, StrikePrice, TimeToExpiry.TotalDays / 365, 0, dict_ComputedATMIV[Underlying][0] / 100, 0);
                        else if (OptionType.Equals("PE"))
                            ScripLTP = PutOption(UnderlyingLTP, StrikePrice, TimeToExpiry.TotalDays / 365, 0, dict_ComputedATMIV[Underlying][0] / 100, 0);

                        //added on 15OCT2020 by Amey
                        //assigning min LTP if Calculated is close to 0;
                        ScripLTP = ScripLTP < 0.05 ? 0.05 : ScripLTP;

                        dr_Trade.ScripLtp = ScripLTP;
                        dr_Trade.IsLTPCalculated = true;
                    }
                    else
                        dr_Trade.IsLTPCalculated = false;
                }

                //added on 01DEC2020 by Amey
                if (dr_Trade.ScripLtp == -1)
                    dr_Trade.ScripLtp = dr_Trade.BEP;

                    //added on 27NOV2020 by Amey
                if (dr_Trade.ScripLtp != -1)
                {
                    //added on 20NOV2020 by Amey
                    if (dr_Trade.NetPosition == 0)
                        dr_Trade.MTM = dr_Trade.BEP;
                    else if (dr_Trade.NetPosition > 0 && dr_Trade.ScripLtp > 0)
                        dr_Trade.MTM = (dr_Trade.ScripLtp - dr_Trade.BEP) * Math.Abs(dr_Trade.NetPosition);
                    else if (dr_Trade.NetPosition < 0 && dr_Trade.ScripLtp > 0)
                        dr_Trade.MTM = (dr_Trade.BEP - dr_Trade.ScripLtp) * Math.Abs(dr_Trade.NetPosition);

                    //added on 23NOV2020 by Amey
                    if (dr_Trade.IntradayNetPosition == 0)
                        dr_Trade.IntradayMTM = dr_Trade.IntradayBEP;
                    else if (dr_Trade.IntradayNetPosition > 0 && dr_Trade.ScripLtp > 0)
                        dr_Trade.IntradayMTM = (dr_Trade.ScripLtp - dr_Trade.IntradayBEP) * Math.Abs(dr_Trade.IntradayNetPosition);
                    else if (dr_Trade.IntradayNetPosition < 0 && dr_Trade.ScripLtp > 0)
                        dr_Trade.IntradayMTM = (dr_Trade.IntradayBEP - dr_Trade.ScripLtp) * Math.Abs(dr_Trade.IntradayNetPosition);
                }
            }
            catch (Exception ee)
            {
                InsertError("AssignLTPs : " + ee);
            }
        }

        private void AssignGreeks(ref GroupsTabs.ConsolidateTradeinfoRow dr_Trades, int ScripToken, double BetaValue)
        {
            try
            {
                if (dict_Greeks.ContainsKey(ScripToken))
                {
                    dr_Trades.IVMiddle = dict_Greeks[ScripToken].IV;
                    dr_Trades.IVLower = dict_Greeks[ScripToken].IVLower;
                    dr_Trades.IVHigher = dict_Greeks[ScripToken].IVHigher;

                    //Added by Akshay on 21-12-2020 For SingleDelta
                    dr_Trades.SingleDelta = dict_Greeks[ScripToken].Delta;
                    dr_Trades.SingleGamma = dict_Greeks[ScripToken].Gamma;

                    dr_Trades.Delta = dict_Greeks[ScripToken].Delta * dr_Trades.NetPosition * BetaValue;
                    dr_Trades.Gamma = dict_Greeks[ScripToken].Gamma * dr_Trades.NetPosition * BetaValue;
                    dr_Trades.Theta = dict_Greeks[ScripToken].Theta * dr_Trades.NetPosition * BetaValue;
                    dr_Trades.Vega = dict_Greeks[ScripToken].Vega * dr_Trades.NetPosition * BetaValue;
                }
                else
                {
                    dr_Trades.Delta = dr_Trades.NetPosition * BetaValue;
                    dr_Trades.Gamma = 0;
                    dr_Trades.Theta = 0;
                    dr_Trades.Vega = 0;
                }
            }
            catch (Exception ee)
            {
                InsertError("AssignGreeks : " + ee);
            }
        }

        #endregion

        #region getToken
        void GetToken()
        {
            try
            {
                //DateTime _dtTest = DateTime.Now;
                #region added by Navin on 23-04-2019
                using (MySqlConnection myConToken = new MySqlConnection(_ConnectionString))
                {
                    using (MySqlCommand IVcmd = new MySqlCommand("sp_selectContractmaster", myConToken))
                    {
                        IVcmd.CommandType = CommandType.StoredProcedure;
                        IVcmd.Parameters.AddWithValue("prmCase", 1);
                        IVcmd.Parameters.AddWithValue("prmScripname", "");
                        IVcmd.Parameters.AddWithValue("prmScrip2", "");
                        myConToken.Open();
                        #region Added by Navin on 15-10-2019
                        MySqlDataReader readToken = IVcmd.ExecuteReader();
                        //dict_OScrip_Token.Add("NIFTY", -999);
                        //dict_OScrip_Token.Add("BANKNIFTY", -888);
                        while (readToken.Read())
                        {
                            try
                            {
                                dict_OScrip_Token[readToken.GetString(2)] = readToken.GetInt32(0);
                                dict_Scrip_Token[readToken.GetString(3)] = readToken.GetInt32(0);

                                //changed to double on 23NOV2020 by Amey
                                dict_Scrip_Expiry[readToken.GetString(3)] = Convert.ToDouble(readToken.GetString(1)); //Expiry
                                dict_OScrip_Expiry[readToken.GetString(2)] = Convert.ToDouble(readToken.GetString(1));//Expiry
                            }
                            catch(Exception ee) { InsertError("Get token Loop : " + ee); }
                        }
                        readToken.Dispose();
                        #endregion

                        #region Commented by Navin on 15-10-2019
                        //MySqlDataAdapter da = new MySqlDataAdapter(IVcmd);
                        //da.Fill(ds_engine, "token");
                        //da.Dispose();
                        //myConToken.Close();
                        #endregion

                        myConToken.Close();
                    }
                }
                #endregion
            }
            catch (Exception tokenMaster)
            {
                if (!exceptionsList.Contains(tokenMaster.Message))
                {
                    InsertError("Get token : " + tokenMaster.ToString());
                }
            }
        }
        #endregion

        #region
        //public void GetTokenLtp()
        //{
        //    try
        //    {
        //        if (getLtpFlag == false)
        //            return;

        //        getLtpFlag = false;
        //        #region Added by Navin on 08-04-2019
        //        DataRow[] dr_UnderlyingToken = dt_Trades.Select("UnderlyingToken=0");
        //        for (int i = 0; i < dr_UnderlyingToken.Count(); i++)
        //        {
        //            try
        //            {
        //                int token;
        //                string expiry;
        //                if (dict_Scrip_Token.TryGetValue(dr_UnderlyingToken[i]["FlatUnderlyingScripName"].ToString(), out token))
        //                {
        //                    dr_UnderlyingToken[i]["UnderlyingToken"] = token;
        //                    #region ltp from feedreceiver 03-12-2018
        //                    if (!dict_LTP.ContainsKey(token))
        //                    {
        //                        objFeed.Subscribe(token.ToString());

        //                        #region added by Navin on 23-09-2019 to add closing of underlying from bhavcopy
        //                        var closingPrice = clsWriteLog.Closing.AsEnumerable().Where(r => r.Field<string>("Scrip") == dr_UnderlyingToken[i]["UnderlyingFuture"].ToString()).Select(r => r.Field<string>("Closing"));
        //                        if (closingPrice.Count() > 0)
        //                            dict_LTP.TryAdd(token, new double[2] { Convert.ToDouble(closingPrice.First()), 0 });
        //                        #endregion
        //                    }
        //                    else if (dict_LTP[token][0] == -1)
        //                    {
        //                        var closingPrice = clsWriteLog.Closing.AsEnumerable().Where(r => r.Field<string>("Scrip") == dr_UnderlyingToken[i]["UnderlyingFuture"].ToString()).Select(r => r.Field<string>("Closing"));
        //                        if (closingPrice.Count() > 0)
        //                            dict_LTP[token][0] = Convert.ToDouble(closingPrice.First().ToString());
        //                    }
        //                    #endregion
        //                }
        //                else if (dict_OScrip_Token.TryGetValue(dr_UnderlyingToken[i]["UnderlyingFuture"].ToString(), out token))
        //                {
        //                    dr_UnderlyingToken[i]["UnderlyingToken"] = token;
        //                    #region ltp from feedreceiver 03-12-2018
        //                    if (!dict_LTP.ContainsKey(token))
        //                    {
        //                        objFeed.Subscribe(token.ToString());

        //                        #region added by Navin on 23-09-2019 to add closing of underlying from bhavcopy
        //                        var closingPrice = clsWriteLog.Closing.AsEnumerable().Where(r => r.Field<string>("Scrip") == dr_UnderlyingToken[i]["UnderlyingFuture"].ToString()).Select(r => r.Field<string>("Closing"));
        //                        if (closingPrice.Count() > 0)
        //                            dict_LTP.TryAdd(token, new double[2] { Convert.ToDouble(closingPrice.First()), 0 });
        //                        #endregion
        //                    }
        //                    else if (dict_LTP[token][0] == -1)
        //                    {
        //                        var closingPrice = clsWriteLog.Closing.AsEnumerable().Where(r => r.Field<string>("Scrip") == dr_UnderlyingToken[i]["UnderlyingFuture"].ToString()).Select(r => r.Field<string>("Closing"));
        //                        if (closingPrice.Count() > 0)
        //                            dict_LTP[token][0] = Convert.ToDouble(closingPrice.First().ToString());
        //                    }
        //                    #endregion
        //                }
        //                else
        //                {
        //                    dr_UnderlyingToken[i]["UnderlyingToken"] = "-999999";
        //                }

        //                if (dict_Scrip_Expiry.TryGetValue(dr_UnderlyingToken[i]["FlatUnderlyingScripName"].ToString(), out expiry))
        //                {
        //                    dr_UnderlyingToken[i]["UnderlyingExpiry"] = expiry;
        //                }
        //                else if (dict_OScrip_Expiry.TryGetValue(dr_UnderlyingToken[i]["UnderlyingFuture"].ToString(), out expiry))
        //                {
        //                    dr_UnderlyingToken[i]["UnderlyingExpiry"] = expiry; //rows.Field<string>("Expiry").ToString();
        //                }
        //                else
        //                {
        //                    dr_UnderlyingToken[i]["UnderlyingExpiry"] = "-999999";
        //                }
        //            }
        //            catch (Exception tokenEx)
        //            {
        //                objWriteLog.WriteLog(tokenEx.ToString());
        //            }
        //        }
        //        #endregion
        //        getLtpFlag = true;
        //        GetLTPFromTable();
        //    }
        //    catch (Exception tokenselectionex)
        //    {
        //        if (!exceptionsList.Contains(tokenselectionex.Message))
        //        {
        //            InsertError(tokenselectionex + " : GetTokenLtp" + tokenselectionex.StackTrace.ToString().Substring(tokenselectionex.StackTrace.ToString().Length - 10));
        //        }
        //    }
        //}

        #endregion

        #region To fetch LTP into the Multi Array
        //public void GetLTPFromTable()  // need to modify this method for feed receiver
        //{
        //    try
        //    {
        //        long tokenvariable = 0;
        //        //string ltpQuery = "";
        //        for (int TIdx = 0; TIdx < dt_Trades.Rows.Count; TIdx++)
        //        {
        //            #region ScripLtp
        //            try
        //            {
        //                tokenvariable = (long.Parse(dt_Trades.Rows[TIdx]["ScripToken"].ToString()));
        //                double[] ltp_found;
        //                if (dict_LTP.TryGetValue(Convert.ToInt32(tokenvariable), out ltp_found))
        //                {
        //                    //added on 15OCT2020 by Amey
        //                    if (ltp_found[0] != -1)
        //                    {
        //                        if (dt_Trades.Rows[TIdx]["InstrumentName"].ToString() == "FUTCUR" || dt_Trades.Rows[TIdx]["InstrumentName"].ToString() == "OPTCUR")
        //                            dt_Trades.Rows[TIdx]["ScripLtp"] = (ltp_found[0] / 100000).ToString();
        //                        else
        //                            dt_Trades.Rows[TIdx]["ScripLtp"] = ltp_found[0].ToString();

        //                        dt_Trades.Rows[TIdx]["IsLTPCalculated"] = false;
        //                    }
        //                    else
        //                        dt_Trades.Rows[TIdx]["ScripLtp"] = "-1";
        //                }
        //                else
        //                    dt_Trades.Rows[TIdx]["ScripLtp"] = "-1";
        //            }
        //            catch (Exception ltpEx)
        //            {
        //                objWriteLog.WriteLog(ltpEx.ToString());
        //            }
        //            #endregion

        //            #region UnderlyingLtp
        //            try
        //            {
        //                tokenvariable = long.Parse(dt_Trades.Rows[TIdx]["UnderlyingToken"].ToString()) == -999999 ? 0 : long.Parse(dt_Trades.Rows[TIdx]["UnderlyingToken"].ToString());
        //                double[] ltp_found_new;
        //                if (dict_LTP.TryGetValue(Convert.ToInt32(tokenvariable), out ltp_found_new))
        //                {
        //                    //added on 15OCT2020 by Amey
        //                    if (ltp_found_new[0] != -1)
        //                    {
        //                        if (dt_Trades.Rows[TIdx]["InstrumentName"].ToString() == "FUTCUR" || dt_Trades.Rows[TIdx]["InstrumentName"].ToString() == "OPTCUR")
        //                            dt_Trades.Rows[TIdx]["UnderlyingLtp"] = (ltp_found_new[0] / 100000).ToString();
        //                        else
        //                            dt_Trades.Rows[TIdx]["UnderlyingLtp"] = ltp_found_new[0].ToString();
        //                    }
        //                    else
        //                        dt_Trades.Rows[TIdx]["UnderlyingLtp"] = "-1";
        //                }
        //                else
        //                    dt_Trades.Rows[TIdx]["UnderlyingLtp"] = "-1";
        //            }
        //            catch (Exception ltpEx)
        //            {
        //                objWriteLog.WriteLog(ltpEx.ToString());
        //            }
        //            #endregion
        //        }
        //    }
        //    catch (Exception getLtpEx)
        //    {
        //        if (!exceptionsList.Contains(getLtpEx.Message))
        //        {
        //            InsertError(" : GetLtpfromTable" + getLtpEx.ToString());
        //        }

        //    }
        //}
        #endregion

        #region LDOWeeklyScrip
        //int z = 0;
        //public void AdjustLtp()
        //{
        //    try
        //    {
        //        //adjustLtpFlag = false;  //added on 06-01-18 by shri
        //        string ScripName = "";
        //        #region Added by Navin on 09-04-2019
        //        for (int iLtp = 0; iLtp < dt_Trades.Rows.Count; iLtp++)
        //        {
        //            try
        //            {
        //                if (dt_Trades.Rows[iLtp]["InstrumentName"].ToString() == "FUTCUR" || dt_Trades.Rows[iLtp]["InstrumentName"].ToString() == "OPTCUR")
        //                {
        //                    TimeToExpiry = ConvertFromUnixTimestamp(Convert.ToDouble(dt_Trades.Rows[iLtp]["ScripExpiry"].ToString())).AddHours(12) - ConvertFromUnixTimestamp(ConvertToUnixTimestamp(DateTime.Now));
        //                }
        //                else
        //                {
        //                    TimeToExpiry = ConvertFromUnixTimestamp(Convert.ToDouble(dt_Trades.Rows[iLtp]["ScripExpiry"].ToString())).AddHours(15).AddMinutes(30) - ConvertFromUnixTimestamp(ConvertToUnixTimestamp(DateTime.Now));
        //                }
        //                if (dt_Trades.Rows[iLtp]["UnderlyingLtp"].ToString() == "-1" && TimeToExpiry.TotalDays > 0)//added on 18_8_16 to check Long dated options(LDO)
        //                {
        //                    #region calculate LTP of LDO Scrip
        //                    if (dt_Trades.Rows[iLtp]["UnderlyingLtp"].ToString() == "-1")
        //                    {
        //                        if (dt_Trades.Rows[iLtp]["OptionType"].ToString() == "EQ")
        //                        {
        //                            dt_Trades.Rows[iLtp]["UnderlyingExpiry"] = dt_Trades.Rows[iLtp]["ScripExpiry"].ToString();
        //                            DateTime UExpiry = ConvertFromUnixTimestamp(Convert.ToDouble(dt_Trades.Rows[iLtp]["ScripExpiry"].ToString() == "" ? "0" : dt_Trades.Rows[iLtp]["ScripExpiry"].ToString()));
        //                            string Day = UExpiry.ToString().Substring(0, 2);
        //                            string monthName = UExpiry.ToString("MMM", CultureInfo.InvariantCulture);
        //                            string year = UExpiry.ToString().Substring(8, 2);
        //                            ScripName = dt_Trades.Rows[iLtp]["ScripName"].ToString();
        //                        }
        //                        else
        //                        {
        //                            ScripName = dt_Trades.Rows[iLtp]["Underlying"].ToString().Trim() + "_|" + cmTick + "_|EQ_|0";  //added by navin on 03-01-2018
        //                        }
        //                        #region added by navin on 03-1-2018
        //                        int _token;
        //                        if (dict_Scrip_Token.TryGetValue(ScripName, out _token))
        //                        {
        //                            dt_Trades.Rows[iLtp]["UnderlyingToken"] = _token;
        //                        }
        //                        //DataRow[] dr = ds_engine.Tables["token"].Select("ScripName='" + ScripName + "'");
        //                        //if (dr.Any())
        //                        //{
        //                        //    dt_Trades.Rows[iLtp]["UnderlyingToken"] = dr[0][0].ToString();
        //                        //}
        //                        #endregion
        //                        try
        //                        {
        //                            double[] _ltp;
        //                            if (dict_LTP.TryGetValue(Convert.ToInt32(dt_Trades.Rows[iLtp]["UnderlyingToken"].ToString()), out _ltp))
        //                                dt_Trades.Rows[iLtp]["UnderlyingLtp"] = _ltp[0].ToString();
        //                        }
        //                        catch (Exception)
        //                        {
        //                            dt_Trades.Rows[iLtp]["UnderlyingLtp"] = "-1";
        //                        }
        //                        dt_Trades.Rows[iLtp]["UnderlyingLtp"] = (((Convert.ToDouble(dt_Trades.Rows[iLtp]["UnderlyingLtp"].ToString()) * interest * TimeToExpiry.TotalDays) / 36500) + Convert.ToDouble(dt_Trades.Rows[iLtp]["UnderlyingLtp"].ToString())).ToString();
        //                    }

        //                    #endregion
        //                }
        //                else
        //                if (ConvertFromUnixTimestamp(Convert.ToDouble(dt_Trades.Rows[iLtp]["ScripExpiry"].ToString())) < (dt_Trades.Rows[iLtp]["UnderlyingExpiry"].ToString() == "" ? ConvertFromUnixTimestamp(Convert.ToDouble(dt_Trades.Rows[iLtp]["ScripExpiry"].ToString())) : ConvertFromUnixTimestamp(Convert.ToDouble(dt_Trades.Rows[iLtp]["UnderlyingExpiry"].ToString()))))
        //                {
        //                    dt_Trades.Rows[iLtp]["UnderlyingLtp"] = (Convert.ToDouble(dt_Trades.Rows[iLtp]["UnderlyingLtp"].ToString()) - ((Convert.ToDouble(dt_Trades.Rows[iLtp]["UnderlyingLtp"].ToString()) * interest * (ConvertFromUnixTimestamp(Convert.ToDouble(dt_Trades.Rows[iLtp]["UnderlyingExpiry"].ToString())) - ConvertFromUnixTimestamp(Convert.ToDouble(dt_Trades.Rows[iLtp]["ScripExpiry"].ToString()))).TotalDays) / 36500)).ToString();//25-01-2017
        //                }
        //            }
        //            catch (Exception LTPex)
        //            {
        //                objWriteLog.WriteLog(LTPex.ToString());
        //            }
        //        }
        //        #endregion
        //    }
        //    catch (Exception adjustEx)
        //    {
        //        if (!exceptionsList.Contains(adjustEx.Message))
        //        {
        //            InsertError(adjustEx.ToString() + " adjustltp : " + adjustEx.StackTrace.ToString().Substring(adjustEx.StackTrace.ToString().Length - 10));
        //        }
        //    }
        //}
        #endregion

        #region CalculateIV
        //public void CalculateIV()
        //{
        //    try
        //    {
        //        ReadIVFile();

        //        for (int TIdx = 0; TIdx < dt_Trades.Rows.Count; TIdx++)
        //        {
        //            //added on 12OCT2020 by Amey
        //            DataRow dr_Trade = dt_Trades.Rows[TIdx];

        //            //if (dictSentimentData.ContainsKey(dt_Trades.Rows[CalcIV]["Underlying"].ToString()))
        //            //    dt_Trades.Rows[CalcIV]["Sentiment"] = (dictSentimentData[dt_Trades.Rows[CalcIV]["Underlying"].ToString()].ToUpper() == "NEUTRAL" ? "-" : dictSentimentData[dt_Trades.Rows[CalcIV]["Underlying"].ToString()]);//added by Navin on 17-01-2020
        //            if (dr_Trade["OptionType"].ToString() != "XX" && dr_Trade["OptionType"].ToString() != "EQ")
        //            {
        //                //Added on 9OCT2020 by Amey
        //                string Underlying = dr_Trade["Underlying"].ToString();

        //                if (dr_Trade["InstrumentName"].ToString() == "FUTCUR" || dr_Trade["InstrumentName"].ToString() == "OPTCUR")
        //                    TimeToExpiry = ConvertFromUnixTimestamp(Convert.ToDouble(dr_Trade["ScripExpiry"])).AddHours(12) - ConvertFromUnixTimestamp(ConvertToUnixTimestamp(DateTime.Now));
        //                else
        //                    TimeToExpiry = ConvertFromUnixTimestamp(Convert.ToDouble(dr_Trade["ScripExpiry"])).AddHours(15).AddMinutes(30) - ConvertFromUnixTimestamp(ConvertToUnixTimestamp(DateTime.Now));

        //                double UnderlyingLTP = Convert.ToDouble(dr_Trade["UnderlyingLtp"]);
        //                double StrikePrice = Convert.ToDouble(dr_Trade["StrikePrice"].ToString());
        //                double ScripLTP = Convert.ToDouble(dr_Trade["ScripLtp"].ToString());
        //                string OptionType = dr_Trade["OptionType"].ToString();
        //                int ScripToken = Convert.ToInt32(dr_Trade["ScripToken"]);

        //                if (dict_ComputedATMIV.ContainsKey(Underlying))
        //                {
        //                    dr_Trade["ATM_IV"] = dict_ComputedATMIV[Underlying][0];

        //                    //added on 12OCT2020 by Amey
        //                    //changed condition on 15OCT2020 by Amey
        //                    if (dict_LTP[Convert.ToInt32(dr_Trade["ScripToken"])][1] == 0 && dict_ComputedATMIV[Underlying][1] == 1)
        //                    {
        //                        if (OptionType.Equals("CE"))
        //                            ScripLTP = CallOption(UnderlyingLTP, StrikePrice, TimeToExpiry.TotalDays / 365, 0, dict_ComputedATMIV[Underlying][0] / 100, 0);
        //                        else if (OptionType.Equals("PE"))
        //                            ScripLTP = PutOption(UnderlyingLTP, StrikePrice, TimeToExpiry.TotalDays / 365, 0, dict_ComputedATMIV[Underlying][0] / 100, 0);

        //                        //added on 15OCT2020 by Amey
        //                        //assigning min LTP if Calculated is close to 0;
        //                        ScripLTP = ScripLTP < 0.05 ? 0.05 : ScripLTP;

        //                        dr_Trade["ScripLtp"] = ScripLTP;
        //                        dr_Trade["IsLTPCalculated"] = true;
        //                    }
        //                    else
        //                        dr_Trade["IsLTPCalculated"] = false;
        //                }

        //                if (ScripLTP == -1 && dict_DefaultIVs.ContainsKey(Underlying))
        //                {
        //                    dr_Trade["IVLower"] = dict_DefaultIVs[Underlying][1];
        //                    dr_Trade["IVMiddle"] = dict_DefaultIVs[Underlying][0];
        //                    dr_Trade["IVHigher"] = dict_DefaultIVs[Underlying][2];
        //                }
        //                else if(dict_Greeks.ContainsKey(ScripToken))
        //                {
        //                    dr_Trade["IVMiddle"] = dict_Greeks[ScripToken].IV;
        //                    dr_Trade["IVLower"] = dict_Greeks[ScripToken].IVLower;
        //                    dr_Trade["IVHigher"] = dict_Greeks[ScripToken].IVHigher;
        //                }
        //                else
        //                {
        //                    dr_Trade["IVMiddle"] = "30";
        //                    dr_Trade["IVLower"] = "15";
        //                    dr_Trade["IVHigher"] = "60";
        //                }

        //                //commented on 11NOV2020 by Amey
        //                //else if (TimeToExpiry.TotalDays > 0)
        //                //{
        //                //    if (OptionType == "CE")
        //                //    {
        //                //        IV = ImpliedCallVolatility(UnderlyingLTP, StrikePrice, TimeToExpiry.TotalDays / 365, 0, ScripLTP, 0, 0) * 100;
        //                //        if ((IV < 0.1) || (IV > 150)) //changed on 18-01-18 by shri to handle 999 logic
        //                //        {
        //                //            if (dict_IV.ContainsKey(Underlying))
        //                //            {
        //                //                dr_Trade["IVLower"] = dict_IV[Underlying][1];
        //                //                dr_Trade["IVMiddle"] = dict_IV[Underlying][0];
        //                //                dr_Trade["IVHigher"] = dict_IV[Underlying][2];
        //                //            }
        //                //            else
        //                //            {
        //                //                dr_Trade["IVMiddle"] = "30";
        //                //                dr_Trade["IVLower"] = "15";
        //                //                dr_Trade["IVHigher"] = "60";
        //                //            }
        //                //        }
        //                //        else
        //                //        {
        //                //            dr_Trade["IVMiddle"] = IV.ToString();
        //                //            dr_Trade["IVLower"] = (IV / IVDivisor).ToString();
        //                //            dr_Trade["IVHigher"] = (IV * IVMultiplier).ToString();
        //                //        }

        //                //    }
        //                //    else if (OptionType == "PE")
        //                //    {
        //                //        IV = ImpliedPutVolatility(UnderlyingLTP, StrikePrice, TimeToExpiry.TotalDays / 365, 0, ScripLTP, 0, 0) * 100;
        //                //        if ((IV < 0.1) || (IV > 150))  //changed on 18-01-18 by shri to handle 999 logic
        //                //        {
        //                //            if (dict_IV.ContainsKey(Underlying))
        //                //            {
        //                //                dr_Trade["IVLower"] = dict_IV[Underlying][1];
        //                //                dr_Trade["IVMiddle"] = dict_IV[Underlying][0];
        //                //                dr_Trade["IVHigher"] = dict_IV[Underlying][2];
        //                //            }
        //                //            else
        //                //            {
        //                //                dr_Trade["IVMiddle"] = "30";
        //                //                dr_Trade["IVLower"] = "15";
        //                //                dr_Trade["IVHigher"] = "60";
        //                //            }
        //                //        }
        //                //        else
        //                //        {
        //                //            dr_Trade["IVMiddle"] = IV.ToString();
        //                //            dr_Trade["IVLower"] = (IV / IVDivisor).ToString();
        //                //            dr_Trade["IVHigher"] = (IV * IVMultiplier).ToString();
        //                //        }
        //                //    }
        //                //}
        //            }
        //        }
        //    }
        //    catch (Exception ivEx)
        //    {
        //        if (!exceptionsList.Contains(ivEx.Message))
        //        {
        //            InsertError("calculate iv " + ivEx.ToString());
        //        }
        //    }
        //}
        #endregion

        #region Read IV TextFile
        public void ReadIVFile()
        {
            try
            {
                using (FileStream stream2 = File.Open("C:/Prime/IV.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(stream2))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] fields = line.Split(',');  //Navin 22-11-2017
                            string Underlying = fields[0].ToUpper();

                            //added on 11NOV2020 by Amey
                            double[] arr_DefaultIVs = new double[3] { Convert.ToDouble(fields[1]), Convert.ToDouble(fields[2]), Convert.ToDouble(fields[3]) };
                            dict_DefaultIVs.AddOrUpdate(Underlying, arr_DefaultIVs, (oldKey, oldVal) => arr_DefaultIVs);  //Navin 23-11-2017

                            //Added on 9OCT2020 by Amey
                            if (!dict_ComputedATMIV.ContainsKey(Underlying))
                                dict_ComputedATMIV.TryAdd(Underlying, new double[2] { Convert.ToDouble(fields[2]), 0 });
                        }
                    }
                }
            }
            catch (Exception readtxtex)
            {
                if (!exceptionsList.Contains(readtxtex.Message))
                {
                    InsertError(readtxtex.ToString() + " : Read iv");
                    //SetText("Unable to access IV file");
                }
            }
        }
        #endregion

        /// <summary>
        /// Key : Underlying | Value : BetaValue
        /// </summary>
        ConcurrentDictionary<string, double> dict_BetaValue = new ConcurrentDictionary<string, double>();
        public void ReadOGFile()
        {
            try
            {
                using (FileStream stream2 = File.Open("C:/Prime/OGrange.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr1 = new StreamReader(stream2))
                    {
                        try
                        {
                            string line1;
                            while ((line1 = sr1.ReadLine()) != null)
                            {
                                string[] fields = line1.Split(',');

                                if (fields[0].ToUpper() == "UNDERLYING") continue;

                                try
                                {
                                    dict_BetaValue.TryAdd(fields[0].ToUpper(), Convert.ToDouble(fields[5].Trim()));
                                }
                                catch (Exception ee)
                                {
                                    InsertError("Stockwise og file " + line1 + Environment.NewLine + ee);
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            InsertError("OG file " + err.ToString());
                        }
                    }
                }
            }
            catch (Exception ogex)
            {
                InsertError("Og from textfile: " + ogex);
            }
        }

        #region add data to consolidated database
        public void SendDataToPrime(Dictionary<string, GroupsTabs.ConsolidateTradeinfoDataTable> dict_Positions)
        {
            try
            {
                int TotalCount = dict_Positions.Count;
                if (TotalCount > 0)
                {
                    //objWriteLog.WriteLog("Rows Sent," + dt_Trades.Rows.Count);

                    //objenginedll.insert_array(dt_Trades);
                    _PrimeDataServer.SendToPrimeViaSocket(dict_Positions);//added by Navin on 04-02-2020

                    //_PrimeHeartBeatServer.SendToPrimeViaSocket(LTT + "_" + _LastTradeTime + "_" + _GatewayStatus + "_" + _SpanStatus);//added +"_"+ _SpanStatus on 05-02-2020
                    //changed on 07JAN2021 by Amey
                    _PrimeHeartBeatServer.SendToPrimeViaSocket(FOLTT + "_" + CMLTT + "_" + _LastTradeTime + "_" + _GatewayStatus + "_" + _SpanStatus);//added +"_"+ _SpanStatus on 05-02-2020

                    if (dict_SpanMargin.Count > 0)
                        _PrimeDataServer.SendSpanData(dict_SpanMargin, false, false);//added by Navin on 04-02-2020 //Added last bool parameter by Akshay on 11-12-2020 

                    Thread.Sleep(100);

                    //Added on 10-12-2020 by Akshay for Expiry Span Margin
                    if (dict_ExpirySpanMargin.Count > 0)
                        _PrimeDataServer.SendSpanData(dict_ExpirySpanMargin, false, true);//added by Navin on 04-02-2020  //Added last bool parameter by Akshay on 11-12-2020
                    Thread.Sleep(100);

                    if (dict_EODMargin.Any())
                        _PrimeDataServer.SendSpanData(dict_EODMargin, true, true);    //Added last bool parameter by Akshay on 11-12-2020

                    objWriteLog.WriteLog("Trades Sent To Prime For : " + TotalCount + " clients", isDebug);

                    //objenginedll.SendSpanData(dict_SpanMargin);
                }
            }
            catch (Exception IVInsert)
            {
                if (!exceptionsList.Contains(IVInsert.Message))
                {
                    //if (!IVInsert.Message.ToString().Contains("Invoke or BeginInvoke cannot be called on a control until the window handle has been created") && !IVInsert.Message.ToString().Contains("Thread was being aborted"))
                    {
                        InsertError(IVInsert.ToString() + " : send data to prime");
                    }
                }
            }
        }
        #endregion

        #region methods to convert datetime to tick and tick to datetime
        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }
        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - origin;
            return diff.TotalSeconds;
        }
        #endregion

        delegate void SetTextCallback(string text);//this is used to remove cross thread exception

        private void SetText(string text)//event handler of delegate
        {
            if (this.lb_ErrorLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                lb_ErrorLog.Items.Insert(0, text);
            }
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            try
            {
                //Task.Run(() => InitialiseSentiment());//added by Navin on 17-01-2020
                #region added by Navin on 01-11-2019
                Task.Factory.StartNew(() => _GatewayHeartBeatClient.ConnectToGateway("Engine", _EngineIP, _GatewayPort));
                _GatewayHeartBeatClient._ConnectionResponse += GetConnectionStatus;
                _GatewayHeartBeatClient._LastTradetimeResponse += GetLastTradeTime;
                _GatewayHeartBeatClient._SpanResponse += GetSpanStatus;//05-02-2020
                #endregion

                //added on 30OCT2020 by Amey
                chkBox_MarkToClosing.Enabled = false;

                ckEdit_BODPS03.Enabled = false;
                ckEdit_IntradayPS03.Enabled = false;
                ckEdit_UploadELM.Enabled = false;
                ckEdit_Day1CM.Enabled = false;
                ckEdit_DownloadBhavcopy.Enabled = false;
                btnStartProcess.Enabled = false;
                getLtpFlag = true;
                lb_ErrorLog.Items.Insert(0, "Engine process started..");
                InsertError("Engine process started..");//03-01-2020
                btn_Start.Enabled = false;
                ckEdit_DownloadExposure.Enabled = false;//added by Navin on 15-10-2019
                ckEdit_ClearEOD.Enabled = false;        //added by Navin on 15-10-2019
                btnEditUploadClient.Enabled = false;    //added by Navin on 15-10-2019
                btnAddUser.Enabled = false;             //added by Navin on 15-10-2019
                Application.DoEvents();

                if (clsWriteLog.Closing.Rows.Count == 0)//added by Navin on 23-09-2019
                    objWriteLog.ReadClosing();

                GetEODData();

                //added on 06NOV2020 by Amey
                isEngineStarted = true;
                StartFromZero = true;

                btn_StopEnginee.Enabled = true;

                //changed location on 06NOV2020 by Amey
                bCast_EngineGateway.SendToChannel("Engine", JsonConvert.SerializeObject("Start")); //added by Navin to send start Flag
            }
            catch (Exception ee)
            {
                if (!exceptionsList.Contains(ee.Message))
                { InsertError(ee.ToString()); }
            }
        }

        private void btn_StopEngine_Click(object sender, EventArgs e)
        {
            try
            {
                bCast_EngineGateway.SendToChannel("Engine", JsonConvert.SerializeObject("Stop")); //added by Navin to send start Flag
                //_startFlag = false;//added by navin on 22-11-2017
                timerMarginCalc.Stop();
                Application.DoEvents();

                //added on 30OCT2020 by Amey
                chkBox_MarkToClosing.Enabled = true;

                //_startFlag = true;//added by navin on 22-11-2017
                btn_StopEnginee.Enabled = false;
                btn_Start.Enabled = true;
                btn_StopEngine.Enabled = false;

                ckEdit_BODPS03.Enabled = true;
                ckEdit_IntradayPS03.Enabled = true;
                ckEdit_UploadELM.Enabled = true;
                ckEdit_Day1CM.Enabled = true;
                ckEdit_DownloadBhavcopy.Enabled = true;
                btnStartProcess.Enabled = true;
                ckEdit_DownloadExposure.Enabled = true;//added by Navin on 15-10-2019
                ckEdit_ClearEOD.Enabled = true;        //added by Navin on 15-10-2019
                btnEditUploadClient.Enabled = true;    //added by Navin on 15-10-2019
                btnAddUser.Enabled = true;             //added by Navin on 15-10-2019

                //added on 06NOV2020 by Amey
                isEngineStarted = false;

                lb_ErrorLog.Items.Insert(0, "Engine process stopped.");
                InsertError("Engine process stopped.");//03-01-2020
            }
            catch (Exception ee)
            {
                if (!exceptionsList.Contains(ee.Message))
                {
                    InsertError(ee.Message + " : " + ee.ToString());
                }
                else
                {
                    //_startFlag = true;//added by navin on 22-11-2017
                    //btn_EODProcess.Enabled = true;
                    btn_StopEnginee.Enabled = false;
                    btn_Start.Enabled = true;
                    btn_StopEngine.Enabled = false;

                    ckEdit_BODPS03.Enabled = true;
                    ckEdit_IntradayPS03.Enabled = true;
                    ckEdit_UploadELM.Enabled = true;
                    ckEdit_Day1CM.Enabled = true;
                    ckEdit_DownloadBhavcopy.Enabled = true;
                    btnStartProcess.Enabled = true;
                    lb_ErrorLog.Items.Insert(0, "Engine process stopped.");
                }
            }
        }

        void InsertTokensToData()
        {
            try
            {
                for (int n = 0; n < dt_Day1Trades.Rows.Count; n++)
                {
                    int token;

                    if (dict_OScrip_Token.TryGetValue(dt_Day1Trades.Rows[n]["ScripName"].ToString(), out token))
                        dt_Day1Trades.Rows[n]["TokenNo"] = token;
                    else if (dict_Scrip_Token.TryGetValue(dt_Day1Trades.Rows[n]["Underlying"].ToString() + "_|" + dt_Day1Trades.Rows[n]["Expiry"].ToString() + "_|" + (dt_Day1Trades.Rows[n]["OptionType"].ToString() == "XX" ? "FUT" : dt_Day1Trades.Rows[n]["OptionType"].ToString()) + "_|" + dt_Day1Trades.Rows[n]["StrikePrice"].ToString().Replace(".00", ""), out token))
                        dt_Day1Trades.Rows[n]["TokenNo"] = token;
                    else
                        dt_Day1Trades.Rows[n]["TokenNo"] = "-999999";

                    if (dict_OScrip_Token.TryGetValue(dt_Day1Trades.Rows[n]["Underlying"].ToString(), out token))
                        dt_Day1Trades.Rows[n]["CashToken"] = token;
                }
            }
            catch (Exception insertEx)
            {
                if (!exceptionsList.Contains(insertEx.Message))
                {
                    InsertError(insertEx + " InsertTokensToData :" + insertEx.StackTrace.ToString().Substring(insertEx.StackTrace.ToString().Length - 10));
                }
            }
        }

        #region ReadPreviousData
        //public void ReadDay1FOCM()
        //{
        //    try
        //    {
        //        DataRow dr;
        //        DateTime dte_ScripExpiry;
        //        double ExpiryInTicks;
        //        int counter = 0;

        //        try
        //        {
        //            //DataTable dt_FO = ReadFromXLSX("C:/Prime/Day1/Day1FO.xlsx");

        //            bool MarkToClosing = chkBox_MarkToClosing.Checked;

        //            var directory = new DirectoryInfo("C:/Prime/Day1");

        //            //added on 30OCT2020 by Amey
        //            var BhavcopyDirectory = new DirectoryInfo("C:/Prime");

        //            var FOBhavcopy = BhavcopyDirectory.GetFiles("fo*.csv")
        //                       .OrderByDescending(f => f.LastWriteTime)
        //                       .First();

        //            var CMBhavcopy = BhavcopyDirectory.GetFiles("cm*.csv")
        //                       .OrderByDescending(f => f.LastWriteTime)
        //                       .First();

        //            var list_FOBhavcopy = Exchange.ReadFOBhavcopy(FOBhavcopy.FullName);
        //            var list_CMBhavcopy = Exchange.ReadCMBhavcopy(CMBhavcopy.FullName);

        //            var myFile = directory.GetFiles("NetPosition*.csv")
        //                        .OrderByDescending(f => f.LastWriteTime)
        //                        .First();

        //            string[] arr_Day1 = File.ReadAllLines(myFile.FullName);

        //            string ClientCode = string.Empty;
        //            string Underlying = string.Empty;
        //            string UnderlyingFullName = string.Empty;
        //            string ScripType = string.Empty;
        //            double StrikePrice = -1;
        //            string CustomScripName = string.Empty;
        //            string CustomBhavcopyScripName = string.Empty;

        //            long Qty = 0;
        //            double Price = 0;

        //            foreach (string line in arr_Day1)
        //            {
        //                string[] fields = line.Split(',');

        //                try
        //                {
        //                    if (fields.Length < 5 || fields[0].Contains("Client")) { Console.WriteLine("CLIENT : " + line); counter++; continue; }

        //                    string[] arr_ScripDetails = fields[2].Trim().Split(' ');

        //                    if (!arr_ScripDetails.Last().Equals("NSE")) { Console.WriteLine("NSE : " + line); counter++; continue; }

        //                    ScripType = arr_ScripDetails[3].ToString().Trim().ToUpper();
        //                    ScripType = ScripType == "C" ? "CE" : (ScripType == "P" ? "PE" : ScripType == "F" ? "FUT" : "EQ");

        //                    if (ScripType == "EQ")
        //                        dte_ScripExpiry = Convert.ToDateTime("01JAN1980");
        //                    else
        //                        dte_ScripExpiry = Convert.ToDateTime(arr_ScripDetails[0]);
        //                    ExpiryInTicks = ConvertToUnixTimestamp(dte_ScripExpiry);

        //                    Underlying = fields[1].ToString().Trim().ToUpper();

        //                    if (ScripType == "EQ" || ScripType == "FUT")
        //                        StrikePrice = -1;
        //                    else
        //                        StrikePrice = Convert.ToDouble(arr_ScripDetails[1]);

        //                    UnderlyingFullName = Underlying + dte_ScripExpiry.ToString("yyMMM").ToUpper() + "FUT";
        //                    if (ScripType == "EQ")
        //                        UnderlyingFullName = Underlying;

        //                    Qty = Convert.ToInt64(fields[3]);
        //                    Price = Convert.ToDouble(fields[4]);

        //                    //added on 29OCT2020 by Amey
        //                    if (MarkToClosing)
        //                    {
        //                        try
        //                        {
        //                            CustomBhavcopyScripName = $"{Underlying}|{dte_ScripExpiry.ToString("ddMMMyyyy").ToUpper()}|{(StrikePrice == -1 ? "0" : StrikePrice.ToString("#.00"))}|{(ScripType == "FUT" ? "XX" : ScripType)}";

        //                            if (ScripType == "EQ")
        //                            {
        //                                var ClosePrice = list_CMBhavcopy.Where(v => v.CustomScripname.Equals(CustomBhavcopyScripName)).FirstOrDefault();
        //                                if (ClosePrice is null)
        //                                    objWriteLog.WriteLog($"Closing Not Found For : {CustomBhavcopyScripName}", isDebug);
        //                                else
        //                                    Price = ClosePrice.Close;
        //                            }
        //                            else
        //                            {
        //                                var ClosePrice = list_FOBhavcopy.Where(v => v.CustomScripname.Equals(CustomBhavcopyScripName)).FirstOrDefault();

        //                                if (ClosePrice is null)
        //                                    objWriteLog.WriteLog($"Closing Not Found For : {CustomBhavcopyScripName}", isDebug);
        //                                else
        //                                    Price = ClosePrice.SettlePrice;
        //                            }
        //                        }
        //                        catch (Exception ee) { InsertError("Day1 Loop Closing : " + line + Environment.NewLine + ee); }
        //                    }

        //                    CustomScripName = Underlying + "_|" + ExpiryInTicks + "_|" + ScripType + "_|" + StrikePrice;
        //                    if (ScripType == "EQ")
        //                        CustomScripName = Underlying + "_|" + ExpiryInTicks + "_|" + ScripType + "_|" + 0;

        //                    int Token = -1;
        //                    if (ScripType == "EQ" && !dict_OScrip_Token.TryGetValue(Underlying, out Token)) { Console.WriteLine("EQ : " + line); counter++; continue; }
        //                    else if (ScripType != "EQ" && !dict_Scrip_Token.TryGetValue(CustomScripName, out Token)) { Console.WriteLine("FO : " + line); counter++; continue; }

        //                    ScripType = ScripType == "FUT" ? "XX" : ScripType;

        //                    if (fields[0].ToString().Trim() != "")
        //                    {
        //                        dr = dt_Day1Trades.NewRow();
        //                        dr["InstrumentName"] = ScripType == "EQ" ? "EQ" : arr_ScripDetails[2].Trim();

        //                        if (ScripType == "XX" || ScripType == "EQ")
        //                            dr["ScripName"] = UnderlyingFullName;
        //                        else
        //                            dr["ScripName"] = Underlying + dte_ScripExpiry.ToString("ddMMMyy").ToUpper() + StrikePrice + ScripType;

        //                        dr["Expiry"] = ExpiryInTicks;
        //                        dr["FillQuantity"] = Qty;
        //                        dr["FillPrice"] = Price;
        //                        dr["Underlying"] = Underlying;
        //                        dr["DealerID"] = fields[0].Trim().ToUpper();
        //                        dr["OptionType"] = ScripType;
        //                        dr["StrikePrice"] = StrikePrice;
        //                        dr["tokenNo"] = Token;
        //                        dr["UnderlyingScripname"] = UnderlyingFullName;
        //                        dt_Day1Trades.Rows.Add(dr);
        //                    }
        //                }
        //                catch (Exception ee) { InsertError("Day1 Loop : " + line + Environment.NewLine + ee); }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //if (!exceptionsList.Contains(ex.Message))
        //            {
        //                InsertError("Day1 " + ex.ToString());
        //            }
        //        }

        //        Console.WriteLine($"Total Rows : {dt_Day1Trades.Rows.Count} | Skipped : {counter}");
        //    }
        //    catch (Exception ee)
        //    {
        //        if (!exceptionsList.Contains(ee.Message))
        //        {
        //            InsertError("Read day1cm " + ee.ToString());
        //        }
        //    }
        //}


        //Changed by Akshay on 04-01-2021 for reading day1 files
        public void ReadDay1FOCM()
        {
            try
            {
                DataRow dr;
                DateTime dte_ScripExpiry;
                double ExpiryInTicks;

                bool MarkToClosing = chkBox_MarkToClosing.Checked;

                var directory = new DirectoryInfo("C:/Prime/Day1");

                //added on 21JAN2021 by Amey
                var BhavcopyDirectory = new DirectoryInfo("C:/Prime");

                var FOBhavcopy = BhavcopyDirectory.GetFiles("fo*.csv")
                           .OrderByDescending(f => f.LastWriteTime)
                           .First();

                var CMBhavcopy = BhavcopyDirectory.GetFiles("cm*.csv")
                           .OrderByDescending(f => f.LastWriteTime)
                           .First();

                var list_FOBhavcopy = Exchange.ReadFOBhavcopy(FOBhavcopy.FullName);
                var list_CMBhavcopy = Exchange.ReadCMBhavcopy(CMBhavcopy.FullName);

                try
                {
                    string[] arr_FOLines = File.ReadAllLines("C:/Prime/Day1/PACE_fo position file.txt");

                    string Underlying = string.Empty;
                    string UnderlyingFullName = string.Empty;
                    string ScripType = string.Empty;
                    double StrikePrice = -1;
                    string CustomScripName = string.Empty;
                    long Qty = 0;
                    double Price = 0;
                    string CustomBhavcopyScripName = string.Empty;

                    foreach (string line in arr_FOLines)
                    {
                        string[] fields = line.Split(',');
                        if (fields.Length < 10) continue;

                        Qty = Convert.ToInt64(fields[8].ToString().Trim());
                        Price = Convert.ToDouble(fields[7].ToString().Trim());

                        if (Qty == 0 || Price == 0) continue;

                        dte_ScripExpiry = Convert.ToDateTime(fields[4]);
                        ExpiryInTicks = ConvertToUnixTimestamp(dte_ScripExpiry);

                        Underlying = fields[2].ToString().Trim().ToUpper();
                        ScripType = fields[5].ToString().Trim().ToUpper();
                        try { StrikePrice = Convert.ToDouble(fields[6]); } catch (Exception) { }
                        UnderlyingFullName = Underlying + dte_ScripExpiry.ToString("yyMMM").ToUpper() + "FUT";

                        if (ScripType == "FX")
                        {
                            ScripType = "FUT";
                            StrikePrice = -1;
                        }

                        //added on 20JAN2021 by Amey
                        if (MarkToClosing)
                        {
                            try
                            {
                                CustomBhavcopyScripName = $"{Underlying}|{dte_ScripExpiry.ToString("ddMMMyyyy").ToUpper()}|{(StrikePrice == -1 ? "0" : StrikePrice.ToString("#.00"))}|{(ScripType == "FUT" ? "XX" : ScripType)}";

                                var ClosePrice = list_FOBhavcopy.Where(v => v.CustomScripname.Equals(CustomBhavcopyScripName)).FirstOrDefault();

                                if (ClosePrice is null)
                                    objWriteLog.WriteLog($"Closing Not Found For : {CustomBhavcopyScripName}", isDebug);
                                else
                                    Price = ClosePrice.SettlePrice;
                            }
                            catch (Exception ee) { InsertError("Day1 Loop Closing : " + line + Environment.NewLine + ee); }
                        }

                        //added on 20JAN2021 by Amey
                        if (Price == 0) continue;

                        //added on 20JAN2021 by Amey
                        if (dte_ScripExpiry.Date < DateTime.Now.Date)
                            continue;


                        CustomScripName = Underlying + "_|" + ExpiryInTicks + "_|" + ScripType + "_|" + StrikePrice;
                        if (!dict_Scrip_Token.ContainsKey(CustomScripName)) continue;

                        ScripType = ScripType == "FUT" ? "XX" : ScripType;
                        //StrikePrice = StrikePrice == -0.01 ? -1 : StrikePrice;

                        dr = dt_Day1Trades.NewRow();
                        dr["InstrumentName"] = fields[1].ToString().Trim().ToString();

                        if (ScripType == "XX")
                            dr["ScripName"] = UnderlyingFullName;
                        else
                            dr["ScripName"] = Underlying + dte_ScripExpiry.ToString("ddMMMyy").ToUpper() + StrikePrice + ScripType;

                        dr["Expiry"] = ExpiryInTicks;
                        dr["FillQuantity"] = Qty;
                        dr["FillPrice"] = Price;
                        dr["Underlying"] = Underlying;
                        dr["DealerID"] = fields[0].ToString().Trim().ToString().ToUpper();
                        dr["OptionType"] = ScripType;
                        dr["StrikePrice"] = StrikePrice;
                        dr["tokenNo"] = dict_Scrip_Token[CustomScripName];
                        dr["UnderlyingScripname"] = UnderlyingFullName;
                        dt_Day1Trades.Rows.Add(dr);
                    }
                }
                catch (Exception ex)
                {
                    //if (!exceptionsList.Contains(ex.Message))
                    {
                        InsertError("Day1 Fo " + ex.ToString());
                    }
                }

                try
                {
                    string[] arr_CMLines = File.ReadAllLines("C:/Prime/Day1/FTHold2222_cm Position file.txt"); //Chnaged File Name and new format on 28-12-2020 by Akshay

                    string EQScripName = string.Empty;
                    long Qty = 0;
                    double Price = 0;
                    string CustomBhavcopyScripName = string.Empty;

                    foreach (string line in arr_CMLines)
                    {
                        string[] fields = line.Split(',');
                        if (fields.Length < 7) continue;

                        dte_ScripExpiry = Convert.ToDateTime("01JAN1980");
                        ExpiryInTicks = ConvertToUnixTimestamp(dte_ScripExpiry);

                        //Added by Akshay on 19-01-2021 for EQ check
                        if (fields[2].ToString().Trim().ToUpper() != "EQ") continue;

                        EQScripName = fields[0].ToString().Trim().ToUpper();

                        if (!dict_OScrip_Token.ContainsKey(EQScripName)) continue;

                        Qty = Convert.ToInt64(fields[5].ToString().Trim());
                        Price = Convert.ToDouble(fields[6].ToString().Trim());

                        //Added by Akshay on 20-01-2020 for checking ignoring zero.
                        if (Qty == 0 || Price == 0) continue;

                        //added on 29OCT2020 by Amey
                        if (MarkToClosing)
                        {
                            try
                            {
                                var ClosePrice = list_CMBhavcopy.Where(v => v.CustomScripname.Equals(CustomBhavcopyScripName)).FirstOrDefault();
                                if (ClosePrice is null)
                                    objWriteLog.WriteLog($"Closing Not Found For : {CustomBhavcopyScripName}", isDebug);
                                else
                                    Price = ClosePrice.Close;
                            }
                            catch (Exception ee) { InsertError("Day1 CM Loop Closing : " + line + Environment.NewLine + ee); }
                        }

                        //added on 20JAN2021 by Amey
                        if (Price == 0) continue;

                        dr = dt_Day1Trades.NewRow();
                        dr["InstrumentName"] = "EQ";  
                        dr["ScripName"] = EQScripName;
                        dr["Expiry"] = "0";
                        dr["FillQuantity"] = Qty;
                        dr["FillPrice"] = Price;
                        dr["Underlying"] = EQScripName;
                        dr["DealerID"] = fields[4].ToString().Trim().ToUpper();
                        dr["OptionType"] = "EQ";  
                        dr["StrikePrice"] = "-1";
                        dr["TokenNo"] = dict_OScrip_Token[EQScripName];
                        dr["UnderlyingScripName"] = EQScripName;
                        dt_Day1Trades.Rows.Add(dr);
                    }

                    int tradeCount = dt_Day1Trades.Rows.Count;
                }
                catch (Exception ex)
                {
                    //if (!exceptionsList.Contains(ex.Message))
                    {
                        InsertError("Day1 CM " + ex.ToString());
                        //SetText("Unable to read Day1CM Files : " + ex.Message);
                    }
                }
            }
            catch (Exception ee)
            {
                if (!exceptionsList.Contains(ee.Message))
                {
                    InsertError("Read day1cm " + ee.ToString());
                }
            }
        }


        #endregion

        #region Insert Day1 data in EOD table
        public void InsertDay1()
        {
            try
            {
                var result = (dt_Day1Trades.AsEnumerable()
                                     .Select(
                                            x => new
                                            {
                                                TokenNumber = x["TokenNo"],
                                                Client = x["DealerID"],
                                                FillPrice = x["FillPrice"],
                                                ScripName = x["ScripName"],
                                                Underlying = x["Underlying"],
                                                OptionType = x["OptionType"],
                                                StrikePrice = x["StrikePrice"],
                                                InstrumentName = x["InstrumentName"],
                                                FillQuantity = x["FillQuantity"],
                                                Expiry = x["Expiry"],
                                                Cash = x["CashToken"],
                                                UnderlyingScripName = x["UnderlyingScripName"]
                                            }
                                            )
                                            .GroupBy(s => new { s.TokenNumber, s.Client })

                                            .Select(
                                                    g => new
                                                    {
                                                        Underlying = g.Select(x => x.Underlying).First().ToString(),
                                                        BEP = Math.Round(Convert.ToDouble(g.Sum(x => Convert.ToDouble(x.FillQuantity) * Convert.ToDouble(x.FillPrice)) / ((g.Sum(x => Convert.ToDouble(x.FillQuantity))) == 0 ? -1 : (g.Sum(x => Convert.ToInt64(x.FillQuantity))))), 2),//added on 25_08_16
                                                        NetPos = g.Sum(x => Convert.ToInt64(x.FillQuantity)),
                                                        ScripName = g.Select(x => x.ScripName).First().ToString(),
                                                        OptionType = g.Select(x => x.OptionType).First().ToString(),
                                                        InstrumentName = g.Select(x => x.InstrumentName).First().ToString(),
                                                        Expiry = g.Select(x => x.Expiry).First().ToString(),
                                                        StrikePrice = g.Select(x => x.StrikePrice).First().ToString(),
                                                        Client = g.Select(x => x.Client).First().ToString(),
                                                        UnderlyingFuture = g.Select(x => x.UnderlyingScripName).First().ToString(),
                                                        Cash = g.Select(x => x.Cash).First().ToString(),
                                                        Scriptoken = g.Select(x => x.TokenNumber).First().ToString(),
                                                    }
                                                )
                                            .OrderByDescending(x => x.Client))
                                            ;
                // int tr = 0;
                MySqlCommand cmd = new MySqlCommand();
                StringBuilder insertCmd = new StringBuilder("insert into eod (DealerID,ScripName,TokenNo,StrikePrice,FillPrice,OptionType,InstrumentName,Expiry,Underlying,UnderlyingScripName,FillQuantity,ClosingPrice,CashToken) values");
                List<string> toInsert = new List<string>();
                long date_Tick = Convert.ToInt64(ConvertToUnixTimestamp(DateTime.Now.Date).ToString());
                foreach (var item in result)
                {
                    if ((Convert.ToDouble(item.Expiry) + 43200) > date_Tick || item.OptionType.ToUpper() == "EQ")   //09-01-18
                    {
                        toInsert.Add("('" + item.Client.ToString() + "','"
                            + item.ScripName.ToString() + "','" +
                            item.Scriptoken.ToString() + "','" +
                            item.StrikePrice.ToString() + "','" +
                            item.BEP.ToString() + "','" +
                            item.OptionType.ToString() + "','" +
                            item.InstrumentName.ToString() + "','" +
                            Convert.ToDouble(item.Expiry).ToString() + "','" +
                            //(Convert.ToDouble(item.Expiry) + 43200).ToString() + "','" +//commented by Navin on 25-09-2019 because it was returninng 12.00 PM as time
                            item.Underlying.ToString() + "','" +
                            item.UnderlyingFuture.ToString() + "','" +
                            item.NetPos.ToString() + "','" +
                            item.BEP.ToString() + "','" + item.Cash + "')");   //10-1-18 to include closing price in day1
                    }
                }
                try
                {
                    if (toInsert.Count > 0)
                    {
                        insertCmd.Append(string.Join(",", toInsert));
                        insertCmd.Append(";");
                        using (MySqlConnection myconnDay1 = new MySqlConnection(_ConnectionString))
                        {
                            cmd = new MySqlCommand(insertCmd.ToString(), myconnDay1);
                            myconnDay1.Open();
                            cmd.ExecuteNonQuery();
                            myconnDay1.Close();
                        }
                    }

                    SetText($"Day1 Process Completed. {toInsert.Count} Rows added.");

                    dt_Day1Trades.Clear();
                }
                catch (Exception eodex)
                {
                    if (!exceptionsList.Contains(eodex.Message))
                    {
                        record = -1;
                        InsertError(eodex + ":" + eodex);
                    }
                }
                if (record >= 0)
                {
                    ClearRecord();
                }
            }
            catch (Exception positionconsolex)
            {
                record = -1;
                if (!exceptionsList.Contains(positionconsolex.Message))
                {
                    InsertError(positionconsolex + ":" + positionconsolex.StackTrace.ToString().Substring(positionconsolex.StackTrace.ToString().Length - 10));
                }
            }
        }
        #endregion

        #region ClearFile
        public void ClearRecord()
        {
            try
            {
                //File.WriteAllText(@"C:\Prime\Day1\Day1FO.txt", string.Empty);
                //File.WriteAllText(@"C:\Prime\Day1\Day1CD.txt", string.Empty);
                File.WriteAllText(@"C:\Prime\Day1\Day1CM.txt", string.Empty);
            }
            catch (Exception ee)
            {
                InsertError(ee.Message);
            }
        }
        #endregion

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
                if (!exceptionsList.Contains(ee.Message))
                {
                    InsertError(ee.ToString());
                }
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
                if (!exceptionsList.Contains(ee.Message))
                {
                    InsertError(ee.ToString());
                }
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
                InsertError(ee.ToString());
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
                InsertError(ee.ToString());
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
                InsertError(ee.ToString());
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
                InsertError(ee.ToString());
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

        private void notifyIconEngineProcess_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            catch (Exception er)
            {
                if (!exceptionsList.Contains(er.Message))
                {
                    InsertError(er.StackTrace.ToString().Substring(er.StackTrace.ToString().Length - 10));
                }
            }
        }

        private void EngineProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (XtraMessageBox.Show("Are you sure to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    //added on 06NOV2020 by Amey
                    isEngineStarted = false;

                    Application.ExitThread();
                    Environment.Exit(0);
                }
                catch (Exception closeEx)
                {
                    if (!exceptionsList.Contains(closeEx.Message))
                    {
                        InsertError(closeEx.Message + ":" + closeEx.StackTrace.ToString().Substring(closeEx.StackTrace.ToString().Length - 10));
                    }
                }
            }
            else
                e.Cancel = true;
        }

        private void notifyIconEngineProcess_Click(object sender, EventArgs e)
        {
            try
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            catch (Exception er)
            {
                objWriteLog.WriteLog("Notify_" + er.Message.ToString());
            }
        }

        #region one to one receive
        void ReceiveOneIP(bool userflag)
        {
            //while (true)
            //{
            IPAddress ipAd;
            bool flag = userflag;
            try
            {

                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                if (localIPs.Length > 2)
                {
                    ipAd = localIPs[2]; //use local m/c IP address, and use the same in the client
                }
                else
                {
                    ipAd = localIPs[1]; //use local m/c IP address, and use the same in the client
                }
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(ipAd);

                if (pingReply.Status == IPStatus.Success)
                {
                    //Server is alive
                }
                myList = new TcpListener(ipAd, 8005);
                myList.Start();
                Socket s;

                s = myList.AcceptSocket();
                s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                while (true)
                {
                    Application.DoEvents();
                    byte[] b = new byte[100];
                    int k = s.Receive(b);
                    this.Invoke((MethodInvoker)(() =>
                    {
                        timeStamp = System.Text.Encoding.ASCII.GetString(b, 0, b.Length);
                        timeStamp = timeStamp.Substring(0, timeStamp.IndexOf('\0'));
                    }));
                    s = myList.AcceptSocket();
                }
                //s.Close();
                //myList.Stop();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("A blocking operation was interrupted by a call to WSACancelBlockingCall") || e.Message.Contains("An existing connection was forcibly closed by the remote host"))
                {
                    return;
                }
                if (!exceptionsList.Contains(e.Message) && !e.Message.Contains("A request to send or receive data was disallowed because the socket is not connected "))
                {
                    InsertError(e.Message + ":" + e.StackTrace.ToString().Substring(e.StackTrace.ToString().Length - 10));
                }

            }
        }
        #endregion

        #region Insert Error log
        public void InsertError(string error)
        {
            try
            {
                objWriteLog.WriteLog(error + "  _" + DateTime.Now);//added by navin on 28-06-2018
            }
            catch (Exception errorlogEx)
            {
                if (!exceptionsList.Contains(errorlogEx.Message))
                {
                    XtraMessageBox.Show("Error Occured in Engine. The Application will now Exit : " + error);
                    Application.Exit();
                }
            }
        }
        #endregion

        #region Not in use
        public void SetDictionary()
        {
            try
            {
                //for (int i = 0; i < ds_engine.Tables["token"].Rows.Count; i++)
                //{
                //    dict_OScrip_Token[ds_engine.Tables["token"].Rows[i]["OScripName"].ToString()] = ds_engine.Tables["token"].Rows[i]["TokenNo"].ToString();
                //    dict_Scrip_Token[ds_engine.Tables["token"].Rows[i]["ScripName"].ToString()] = ds_engine.Tables["token"].Rows[i]["TokenNo"].ToString();
                //    dict_Scrip_Expiry[ds_engine.Tables["token"].Rows[i]["ScripName"].ToString()] = ds_engine.Tables["token"].Rows[i]["Expiry"].ToString();
                //    dict_OScrip_Expiry[ds_engine.Tables["token"].Rows[i]["OScripName"].ToString()] = ds_engine.Tables["token"].Rows[i]["Expiry"].ToString();
                //}
            }
            catch (Exception setEx)
            {
                if (!exceptionsList.Contains(setEx.Message))
                {
                    // SetText("Error occurred while reading from token master ");
                    InsertError("Set dictionary : " + setEx.ToString());
                }
            }
        }
        private void bgWorker_Margin_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //mar.GetMargin();// temp comment
            }
            catch (Exception ee)
            {
                if (!exceptionsList.Contains(ee.Message))
                {
                    InsertError(ee.ToString());
                }
            }
        }

        private void timerMarginCalc_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!bgWorker_Margin.IsBusy)
                {
                    bgWorker_Margin.RunWorkerAsync();
                }
            }
            catch (Exception ee)
            {
                if (!exceptionsList.Contains(ee.Message))
                {
                    InsertError(ee.ToString());
                }
            }
        }

        #endregion

        #region Decompression
        public static string DecompressString(string compressedText)
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
        #endregion

        #region Upload PS03 File added by Navin on 10-06-2019
        StringBuilder _bodClient = new StringBuilder();
        void downloadBhavcopy()
        {
            WebClient webClient = new WebClient();
            SetText("Downloading bhavcopy ");
            Application.DoEvents();
            try
            {
                try
                {
                    string[] filePaths = Directory.GetFiles(@"c:\Prime\", "*bhav.zip");
                    foreach (var item in filePaths)
                    {
                        File.Delete(item);
                    }
                }
                catch (Exception deleteEx)
                {
                    InsertError("Exception occurred while deleting zip files " + deleteEx.ToString());
                }
                string[] filePaths1 = Directory.GetFiles(@"c:\Prime\", "cm*.csv");
                foreach (var item in filePaths1)
                {
                    File.Delete(item);
                }
                filePaths1 = Directory.GetFiles(@"c:\Prime\", "fo*.csv");
                foreach (var item in filePaths1)
                {
                    File.Delete(item);
                }
                string monthStr = dtpBhavcopydate.Value.ToString("MMM", CultureInfo.InvariantCulture).ToUpper();
                string year = dtpBhavcopydate.Value.Year.ToString("0000");
                string fileName = year + "//" + monthStr + "//fo" + dtpBhavcopydate.Value.Day.ToString("00") + monthStr + year + "bhav.csv.zip";
                string fileNameCM = year + "//" + monthStr + "//cm" + dtpBhavcopydate.Value.Day.ToString("00") + monthStr + year + "bhav.csv.zip";
                webClient.DownloadFile("https://www.nseindia.com/content/historical/DERIVATIVES/" + fileName, @"c:\Prime\foBhav.zip");
                webClient.DownloadFile("https://www.nseindia.com/content/historical/EQUITIES/" + fileNameCM, @"c:\Prime\cmBhav.zip");
                extractZip();
                SetText("Bhavcopy downloaded successfully");
            }
            catch (Exception error)
            {
                if (error.Message.Contains("The remote server returned an error: (404) Not Found"))
                    SetText("Bhavcopy not available, please try again");
                else
                    InsertError("Bhavcopy download " + error.ToString());
            }
        }
        void extractZip()
        {
            try
            {
                string[] filePaths = Directory.GetFiles(@"c:\Prime\", "*Bhav.zip");
                for (int i = 0; i < filePaths.Length; i++)
                {
                    ZipFile.ExtractToDirectory(filePaths[i], @"c:\Prime\");
                }
            }
            catch (Exception error)
            {
                if (error.Message.ToString().Contains("already exists"))
                {
                    return;
                }
                InsertError("Exception occurred while extracting zip file " + error.ToString());
                SetText("Exception occurred while extracting zip file");
            }
        }
        void uploadElmAdhoc()
        {
            try
            {
                int _ElmCount = 0;
                SetText("Uploading ELM");
                InsertError("Uploading ELM");
                InsertError("Selected file to update ELMAdhoc -" + lbl_ELM_AdhocFile.Text.Trim());
                Application.DoEvents();
                //string[] cmPath = Directory.GetFiles("C:\\Prime", "ElmAdhoc.csv");
                //if (cmPath.Length == 1)
                //{
                DateTime dateTimeStart = DateTime.Now;
                using (FileStream stream = File.Open(lbl_ELM_AdhocFile.Text.Trim(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        string line1;
                        using (MySqlConnection mysqlConnElm = new MySqlConnection(_ConnectionString))
                        {
                            while ((line1 = sr.ReadLine()) != null)
                            {
                                string[] fields = line1.Split(',');
                                if (fields[1].Trim().ToUpper() != "ELM")
                                {
                                    try
                                    {
                                        _ElmCount++;
                                        using (MySqlCommand cmdELM = new MySqlCommand("update clientdetail set Margin=" + Convert.ToDouble(fields[1].Trim()) + ", Adhoc=" + Convert.ToDouble(fields[2].Trim()) + " where ClientID='" + fields[0].Trim() + "'", mysqlConnElm))
                                        {
                                            if (mysqlConnElm.State == ConnectionState.Closed) mysqlConnElm.Open();
                                            cmdELM.ExecuteNonQuery();
                                        }
                                    }
                                    catch (Exception error)
                                    {
                                        SetText("Data not in proper format -" + line1);
                                        InsertError("Data not in proper format -" + line1 + ", " + error.ToString());
                                    }
                                }
                            }
                            mysqlConnElm.Close();
                        }
                    }
                }
                //}
                InsertError("ELM uploaded successfully. ELM file row count " + _ElmCount + ", time taken to update elm " + (DateTime.Now - dateTimeStart));
                SetText("ELM uploaded successfully. ELM file row count " + _ElmCount);
            }
            catch (Exception elmEx)
            {
                InsertError("Upload ELM & Adhoc " + elmEx.ToString());
            }
        }
        private void btnStartProcess_Click(object sender, EventArgs e)
        {
            try
            {
                btn_Start.Enabled = false;
                if (ckEdit_ClearEOD.Checked)
                {
                    try
                    {
                        SetText("Deleting eod data");
                        InsertError("Deleting eod data");
                        Application.DoEvents();
                        using (MySqlConnection myConnClear = new MySqlConnection(_ConnectionString))
                        {
                            using (MySqlCommand myCmdClear = new MySqlCommand("spClearEOD", myConnClear))
                            {
                                myConnClear.Open();
                                myCmdClear.CommandType = CommandType.StoredProcedure;
                                myCmdClear.ExecuteNonQuery();
                                myConnClear.Close();
                            }
                        }
                        SetText("Eod data deleted successfully");
                        InsertError("Eod data deleted successfully");
                    }
                    catch (Exception cEodEx)
                    {
                        InsertError("Clear EOD " + cEodEx.ToString());
                    }
                    ckEdit_ClearEOD.Checked = false;
                }
                if (ckEdit_DownloadBhavcopy.Checked)
                {
                    DateTime _dtStartTime = DateTime.Now;//02-01-2020
                    downloadBhavcopy();
                    ckEdit_DownloadBhavcopy.Checked = false;
                    InsertError("Time taken to download bhavcopy " + (DateTime.Now - _dtStartTime));//02-01-2020
                }
                if (ckEdit_BODPS03.Checked)
                {
                    lb_ErrorLog.Items.Insert(0, "PS03 upload started..");
                    Application.DoEvents();
                    SelectClientfromDatabase();//02-12-2019
                    InsertError("Selected BOD PS03 file- " + lbl_BODPS03File.Text);
                    DateTime _dtStartTime = DateTime.Now;//02-01-2020
                    UploadPS03(lbl_BODPS03File.Text.Trim());
                    if (dt_Day1Trades.Rows.Count > 0)
                    {
                        InsertTokensToData();
                        if (clsWriteLog.Closing.Rows.Count == 0)
                            objWriteLog.ReadClosing();
                        if (clsWriteLog.Closing.Rows.Count > 0)
                        {
                            ReplaceClosing();
                            ClearPS03Clientdata("BOD");
                            InsertDay1();
                            //lb_ErrorLog.Items.Insert(0, "PS03 upload completed..");
                            InsertError("PS03 upload completed");
                        }
                        else
                            SetText("Bhavcopy not available");
                    }
                    ckEdit_BODPS03.Checked = false;
                    lb_ErrorLog.Items.Insert(0, "PS03 upload completed..");
                    InsertError("Time taken to upload PS03 file " + (DateTime.Now - _dtStartTime));//02-01-2020
                }
                if (ckEdit_UploadELM.Checked)
                {
                    uploadElmAdhoc();
                    ckEdit_UploadELM.Checked = false;
                }
                if (ckEdit_IntradayPS03.Checked)
                {
                    lb_ErrorLog.Items.Insert(0, "Intraday PS03 upload started..");
                    InsertError("Selected Intraday PS03 file- " + lbl_IntradayPS03File.Text);
                    SelectClientfromDatabase();//02-12-2019
                    DateTime _dtStartTime = DateTime.Now;//02-01-2020

                    UploadPS03(lbl_IntradayPS03File.Text.Trim());

                    if (dt_Day1Trades.Rows.Count > 0)
                    {
                        InsertTokensToData();
                        if (clsWriteLog.Closing.Rows.Count == 0)
                            objWriteLog.ReadClosing();
                        if (clsWriteLog.Closing.Rows.Count > 0)
                        {
                            ReplaceClosing();
                            ClearPS03Clientdata("Intra");
                            InsertDay1();

                            InsertError("Intraday PS03 upload completed");
                        }
                        else
                            SetText("Bhavcopy not available");
                    }
                    ckEdit_IntradayPS03.Checked = false;
                    InsertError("Time taken to upload intraday PS03 file " + (DateTime.Now - _dtStartTime));//02-01-2020
                    lb_ErrorLog.Items.Insert(0, "Intraday PS03 upload completed..");
                }
                if (ckEdit_Day1CM.Checked)
                {
                    SetText("Day1 Process started...");

                    SelectClientfromDatabase();

                    InsertError("Day1 Process started...");
                    ReadDay1FOCM();
                    InsertTokensToData();
                    InsertDay1();

                    //commented on 10NOV2020 by Amey
                    //SetText("Day1 Process Completed..");

                    InsertError("Day1 Process Completed..");
                    ckEdit_Day1CM.Checked = false;
                }
                if (ckEdit_DownloadExposure.Checked)//added by Navin on 03-10-2019 for Span Calculation 
                {
                    SetText("Downloading exposure file from exchange...");
                    DownloadExposure();
                    SetText("Exposure file downloaded successfully..");
                    ckEdit_DownloadExposure.Checked = false;
                }
                btn_Start.Enabled = true;
            }
            catch (Exception processEx)
            {
                InsertError("Start process " + processEx.ToString());
            }
        }
        private void ckEdit_ClearEOD_CheckedChanged(object sender, EventArgs e)
        {
            if (ckEdit_ClearEOD.Checked)
            {
                ckEdit_BODPS03.Checked = false;
                ckEdit_Day1CM.Checked = false;
                ckEdit_IntradayPS03.Checked = false;
                ckEdit_UploadELM.Checked = false;
                ckEdit_DownloadBhavcopy.Checked = false;
            }
        }
        private void ckEdit_DownloadBhavcopy_CheckedChanged(object sender, EventArgs e)
        {
            if (ckEdit_DownloadBhavcopy.Checked)
            {
                dtpBhavcopydate.Enabled = true;
                ckEdit_ClearEOD.Checked = false;
                ckEdit_ClearEOD.Enabled = false;
            }
            else
            {
                dtpBhavcopydate.Enabled = false;
                ckEdit_ClearEOD.Enabled = true;
            }
        }

        private void ckEdit_BODPS03_CheckedChanged(object sender, EventArgs e)
        {
            if (ckEdit_BODPS03.Checked)
            {
                ckEdit_IntradayPS03.Enabled = false;
                ckEdit_ClearEOD.Checked = false;
                ckEdit_ClearEOD.Enabled = false;
                OpenFileDialog _openFileDialog = new OpenFileDialog();//23-12-2019
                if (_openFileDialog.ShowDialog() == DialogResult.OK)
                    lbl_BODPS03File.Text = _openFileDialog.FileName;
                else
                    ckEdit_BODPS03.CheckState = CheckState.Unchecked;
            }
            else
            {
                lbl_BODPS03File.Text = string.Empty;
                ckEdit_IntradayPS03.Enabled = true;
                ckEdit_ClearEOD.Enabled = true;
            }
        }

        private void ckEdit_IntradayPS03_CheckedChanged(object sender, EventArgs e)
        {
            if (ckEdit_IntradayPS03.Checked)
            {
                ckEdit_BODPS03.Enabled = false;
                ckEdit_ClearEOD.Enabled = false;
                ckEdit_ClearEOD.Checked = false;
                OpenFileDialog _openFileDialog = new OpenFileDialog();//23-12-2019
                if (_openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    lbl_IntradayPS03File.Text = _openFileDialog.FileName;
                }
                else
                    ckEdit_IntradayPS03.CheckState = CheckState.Unchecked;
            }
            else
            {
                lbl_IntradayPS03File.Text = string.Empty;
                ckEdit_BODPS03.Enabled = true;
                ckEdit_ClearEOD.Enabled = true;
            }

        }

        private void btnEditUploadClient_Click(object sender, EventArgs e)
        {
            try
            {
                lb_ErrorLog.Items.Insert(0, "Client upload Started..");
                BODProcess bod = new BODProcess();
                bod.ShowDialog();
                lb_ErrorLog.Items.Insert(0, "Client upload Completed..");
            }
            catch (Exception bodEX)
            {
                InsertError("Edit upload client " + bodEX.ToString());
            }
        }

        #region Get client from database added by Navin on 23-10-2019
        ConcurrentDictionary<string, int> dictClient = new ConcurrentDictionary<string, int>();
        private void SelectClientfromDatabase()
        {
            try
            {
                dictClient.Clear();
                using (MySqlCommand cmdClients = new MySqlCommand("sp_Clients", mySqlArrcsDBConn))
                {
                    if (mySqlArrcsDBConn.State == ConnectionState.Closed) mySqlArrcsDBConn.Open();
                    MySqlDataReader _mySqlDataReader = cmdClients.ExecuteReader();
                    while (_mySqlDataReader.Read())
                    {
                        string ClientID = _mySqlDataReader.GetString(0).ToUpper().Trim();
                        dictClient.TryAdd(ClientID, 0);
                    }
                    _mySqlDataReader.Close();
                    _mySqlDataReader.Dispose();
                    mySqlArrcsDBConn.Close();
                }
            }
            catch (Exception clientEx)
            {
                InsertError("SelectClientfromDatabase " + clientEx.ToString());
            }
        }
        #endregion
        void UploadPS03(string filePath)
        {
            try
            {
                int _totalCount = 0, _Inserted = 0;//12-12-2019

                StringBuilder sCommand = new StringBuilder("INSERT INTO eod (DealerID, ScripName, TokenNo, StrikePrice, FillPrice, OptionType, InstrumentName, Expiry, Underlying, UnderlyingScripName, FillQuantity, ClosingPrice)  values");
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        string line1;

                        while ((line1 = sr.ReadLine()) != null)
                        {
                            string[] fields = line1.Split(',');

                            if (fields[0].Trim() != "")
                            {
                                _totalCount++;//12-12-2019
                                if (!dictClient.ContainsKey(fields[7].Trim().ToString().ToUpper())) continue;//added by Navin on 02-12-2019 to pick records of uploaded clients
                                DataRow dr = dt_Day1Trades.NewRow();
                                try
                                {
                                    dr["InstrumentName"] = fields[8].Trim().ToString();
                                    fields[11] = Convert.ToDouble(fields[11]).ToString();
                                    //fields[11] = fields[11].Replace(".00","").Trim('0');//29-11-2019
                                    if (fields[12].Trim().ToUpper() == "FF")
                                    {
                                        dr["ScripName"] = fields[9].Trim().ToString() + fields[10].Substring(9, 2) + fields[10].Substring(3, 3).ToUpper() + "FUT";
                                        //string ScripName = fields[3].Trim().ToString() + "_|" + ExpiryInTicks + "_|" + (fields[6].Trim() == "" ? "XX" : fields[6].Trim()).ToString() + "_|" + (Math.Round(Convert.ToDecimal(fields[5].Trim() == "" ? "0" : fields[5].Trim()))).ToString();
                                        fields[12] = "XX";
                                    }
                                    else
                                    {
                                        dr["ScripName"] = fields[9].Trim().ToString() + fields[10].Remove(7, 2).Replace("-", "").ToUpper() + fields[11].Trim() + fields[12].Trim();//added .Replace(".00","") on 29-11-2019 by Navin
                                    }
                                    DateTime dte = DateTime.ParseExact(fields[10].Trim().ToString().ToUpper(), "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                                    dr["Expiry"] = ConvertToUnixTimestamp(dte);
                                    dr["FillQuantity"] = Convert.ToInt64(fields[28].Trim().ToString()) - Convert.ToInt64(fields[30].Trim().ToString());
                                    dr["FillPrice"] = 0;//need to change
                                    dr["Underlying"] = fields[9].Trim().ToString();
                                    dr["DealerID"] = fields[7].Trim().ToString();
                                    dr["OptionType"] = fields[12].Trim().ToString();
                                    dr["StrikePrice"] = (fields[11].Trim() == "" || fields[11].Trim() == "0") ? -1 : Convert.ToDecimal(fields[11].Trim());
                                    dr["tokenNo"] = 1;
                                    dr["UnderlyingScripname"] = fields[9].Trim().ToString() + fields[10].Substring(9, 2) + fields[10].Substring(3, 3).ToUpper() + "FUT";
                                    dt_Day1Trades.Rows.Add(dr);
                                    _Inserted++;//12-12-2019
                                }
                                catch (Exception)
                                {
                                    InsertError("Data mismatch : InstrumentName " + fields[8] + ", Expiry " + fields[10] + ",  FillQuantity " + fields[28] + " - " + fields[30] + " , Underlying " + fields[9] + " , DealerID " + fields[7] + " , OptionType " + fields[12] + ", StrikePrice " + fields[11]);
                                }
                            }
                        }
                        InsertError("Total records present is PS03 file- " + _totalCount + ", Records inserted in database " + _Inserted);
                        lb_ErrorLog.Items.Insert(0, "Total records present is PS03 file- " + _totalCount + ", Records inserted in database " + _Inserted);
                    }
                }
            }
            catch (Exception Psex)
            {
                InsertError("PS03 Upload " + Psex.ToString());
            }
        }

        private DataTable ReadFromXLSX(string FullFilePath)
        {
            DataTable dt_Excel = new DataTable();

            try
            {
                Workbook workbook = new Workbook();
                workbook.LoadDocument(FullFilePath, DocumentFormat.Xlsx);
                Worksheet worksheet = workbook.Worksheets[0];
                var range = worksheet.GetDataRange();

                dt_Excel = worksheet.CreateDataTable(range, true);
                DataTableExporter exporter = worksheet.CreateDataTableExporter(range, dt_Excel, true);
                exporter.Options.ConvertEmptyCells = false;
                exporter.Options.DefaultCellValueToColumnTypeConverter.EmptyCellValue =
                exporter.Options.DefaultCellValueToColumnTypeConverter.SkipErrorValues = true;
                exporter.Export();
            }
            catch (Exception ee) { InsertError("ReadFromXSLX : " + ee); }

            return dt_Excel;
        }

        void ReplaceClosing()
        {
            _bodClient.Clear();
            if (dt_Day1Trades.Rows.Count < 1) return;
            long date_Tick = Convert.ToInt64(ConvertToUnixTimestamp(DateTime.Now.Date).ToString());
            for (int a = 0; a < dt_Day1Trades.Rows.Count; a++)
            {
                try
                {
                    if (a == 0)
                        _bodClient.Append("('" + dt_Day1Trades.Rows[a]["DealerID"].ToString().Trim() + "',");
                    else if (dt_Day1Trades.Rows[a - 1]["DealerID"].ToString().Trim() != dt_Day1Trades.Rows[a]["DealerID"].ToString().Trim())
                        _bodClient.Append("'" + dt_Day1Trades.Rows[a]["DealerID"].ToString().Trim() + "',");
                    #region Marking only future scrips to closing logic changed by navin on 15-02-2019
                    if (dt_Day1Trades.Rows[a]["Underlying"].ToString().Trim() == "XX")
                    {
                        var closingPrice = clsWriteLog.Closing.AsEnumerable().Where(r => r.Field<string>("Scrip") == dt_Day1Trades.Rows[a]["ScripName"].ToString()).Select(r => r.Field<string>("Closing"));
                        if (closingPrice.Count() > 0)
                        {
                            dt_Day1Trades.Rows[a]["FillPrice"] = closingPrice.First().ToString();  //9-11-17
                        }
                    }
                    else
                    {
                        DataRow[] closingPrice = clsWriteLog.Closing.Select("Underlying='" + dt_Day1Trades.Rows[a]["Underlying"].ToString() + "' and StrikePrice='" + dt_Day1Trades.Rows[a]["StrikePrice"].ToString() + "' and OptionType='" + dt_Day1Trades.Rows[a]["OptionType"].ToString() + "' and Expiry='" + dt_Day1Trades.Rows[a]["Expiry"].ToString() + "'");
                        if (closingPrice.Any())
                        {
                            dt_Day1Trades.Rows[a]["FillPrice"] = closingPrice[0]["Closing"].ToString();  //9-11-17
                        }
                    }
                    #endregion
                }
                catch (Exception insertEodex)
                {
                    record = -1;
                    InsertError("Replace closing " + insertEodex.ToString());
                }
            }
            _bodClient.Remove(_bodClient.Length - 1, 1);
            _bodClient.Append(")");
        }

        private void ckEdit_UploadELM_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog _openFileDialog = new OpenFileDialog();//23-12-2019
                if (ckEdit_UploadELM.CheckState == CheckState.Checked)
                {
                    if (_openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //_ElmAdhocPath = _openFileDialog.FileName;
                        lbl_ELM_AdhocFile.Text = _openFileDialog.FileName;
                    }
                    else
                        ckEdit_UploadELM.CheckState = CheckState.Unchecked;
                }
                else
                    lbl_ELM_AdhocFile.Text = string.Empty;
            }
            catch (Exception uploadEx)
            {
                InsertError("Upload ELM and Adhoc path " + uploadEx.ToString());
            }
        }

        #region Reading VAR File For EQ
        //Added by Akshay on 29-12-2020 For Reading VAREQ
        public void ReadVAR_EQ(string interval)
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
                        _UpdateCount += 1;

                        var VAREQ_directory = new DirectoryInfo("C:/Prime/VARMargin");

                        var VAREQfileName = VAREQ_directory.GetFiles("C_VAR1_" + DateTime.Now.ToString("ddMMyyyy") + "*.DAT").OrderByDescending
                                            (f => f.LastWriteTime).First();

                        if (File.Exists(VAREQfileName.FullName))
                        {
                            foreach (var line in File.ReadAllLines(VAREQfileName.FullName))
                            {
                                string[] fields = line.Split(',');
                                if (fields.Length < 10) continue;
                                if (fields[2] != "EQ") continue;

                                string ScripName = fields[1];
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
                        if (!exceptionsList.Contains(ee.Message))
                        {
                            InsertError("Read VAREQ " + ee.ToString());
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                if (!exceptionsList.Contains(ee.Message))
                {
                    InsertError("Read VAREQ " + ee.ToString());
                }
            }
        }
        #endregion

        void ClearPS03Clientdata(string prmType)
        {
            try
            {
                string strQuery = string.Empty;
                if (prmType == "BOD")
                    strQuery = "truncate eod ";
                else
                    strQuery = "delete from eod where DealerID in " + _bodClient.ToString() + "";

                using (MySqlConnection myConnClear = new MySqlConnection(_ConnectionString))
                {
                    using (MySqlCommand myCmdClear = new MySqlCommand(strQuery, myConnClear))
                    {
                        myConnClear.Open();
                        myCmdClear.ExecuteNonQuery();
                        myConnClear.Close();
                    }
                }
            }
            catch (Exception error)
            {
                InsertError("ClearPS03Clientdata " + error.ToString());
            }
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            AddUser objAddUser = new AddUser(mySqlArrcsDBConn);
            objAddUser.eveErrorlog += InsertError;
            objAddUser.ShowDialog();
        }
        #endregion

        #region not in use methods
        private void btn_EODClear_Click(object sender, EventArgs e)
        {
            try
            {
                //to clear EOD Table
                if (mySqlArrcsDBConn != null) mySqlArrcsDBConn.Close();// navin 22-11-2017 
                MySqlCommand clearCmd = new MySqlCommand("truncate eod", mySqlArrcsDBConn);
                clearCmd.ExecuteNonQuery();
                clearCmd.Dispose();//added by navin on 22-11-2017
                XtraMessageBox.Show("EOD data cleared successfully");
            }
            catch (Exception clearEx)
            {
                if (!exceptionsList.Contains(clearEx.Message))
                {
                    InsertError(clearEx.StackTrace.ToString().Substring(clearEx.StackTrace.ToString().Length - 10));
                }
            }
        }
        #endregion

        void DownloadExposure()
        {
            try
            {
                string expoName = (DateTime.Now.Day).ToString("00") + (DateTime.Now.Month).ToString("00") + (DateTime.Now.Year).ToString();
                WebClient webClient = new WebClient();
                webClient.DownloadFile("https://www.nseindia.com/archives/exp_lim/ael_" + expoName + ".csv", @"c:\Prime\" + @"exp.csv");
            }
            catch (Exception exposureEx)
            {
                InsertError("Exception occurred while downloading exposure file " + exposureEx.ToString());
            }
        }

        #region Sentiment Analysis added by Navin on 16-01-2020
        //Sentiment sentiment;//added by Navin on 17-01-2020
        //ConcurrentDictionary<string, string> dictSentimentAnalysisData = new ConcurrentDictionary<string, string>();//added by Navin on 16-01-2020
        //ConcurrentDictionary<string, string> dictSentimentData = new ConcurrentDictionary<string, string>();
        //string _sentimentIP; int _sentimentPort;
        //public void ReceiveSentimentData(string strData)
        //{
        //    try
        //    {
        //        dictSentimentData = (ConcurrentDictionary<string, string>)JsonConvert.DeserializeObject(strData, dictSentimentData.GetType());
        //    }
        //    catch (Exception error)
        //    {
        //        InsertError("ReceiveSentimentData " + error.ToString());
        //    }
        //}
        //public void InitialiseSentiment()
        //{
        //    try
        //    {
        //        sentiment = new Sentiment();
        //        sentiment._eveSentimentResponse += ReceiveSentimentData;
        //        Task.Run(() => sentiment.FOConnect(_sentimentIP, _sentimentPort));
        //    }
        //    catch (Exception sentEx)
        //    {
        //        InsertError("InitialiseSentiment " + sentEx.ToString());
        //    }
        //}
        #endregion

        /// <summary>
        /// <para>Key => Underlying Token (12556)</para>
        /// <para>Value => Key => Closest DateTime</para>
        /// <para>Value => Key => Closest Strikes (11200)</para>
        /// <para>Value => [0] : CEToken, [1] : PEToken </para>
        /// </summary>
        ConcurrentDictionary<int, Dictionary<DateTime, Dictionary<double, int[]>>> dict_ClosestATMTokens = new ConcurrentDictionary<int, Dictionary<DateTime, Dictionary<double, int[]>>>();

        /// <summary>
        /// Key => Underlying | Value => Closest Underlying Token
        /// </summary>
        ConcurrentDictionary<string, int> dict_UnderlyingToken = new ConcurrentDictionary<string, int>();


        /// <summary>
        /// Key => ScripName | Value => VAR
        /// </summary>
        ConcurrentDictionary<string, double> dict_VARMargin = new ConcurrentDictionary<string, double>();      //Added by Akshay on 31-12-2020 For Storing VAR for EQ

        /// <summary>
        /// Key : Underlying | Value : [ Avg Computed IV, 0/1(0 if IV calculated using closing from yesterday, 1 if IV calculated using LTP) ]
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_ComputedATMIV = new ConcurrentDictionary<string, double[]>();

        /// <summary>
        /// Key => Current Month Underlying Token | Value : [ LTP, 0/1(0 if LTP received is closing from yesterday, 1 if LTP is live) ]
        /// </summary>
        ConcurrentDictionary<int, double[]> dict_XXLTP = new ConcurrentDictionary<int, double[]>();

        /// <summary>
        /// Key => Underlying Token | Value => [0] : CEIV, [1] : PEIV
        /// </summary>
        ConcurrentDictionary<int, double[]> dict_IVs = new ConcurrentDictionary<int, double[]>();

        /// <summary>
        /// Reads contract.txt and fills required dictionaries.
        /// </summary>
        private void ReadContractAndFill()
        {
            try
            {
                var list_Contract = Exchange.ReadContract("C:\\Prime\\contract.txt").OrderBy(v => v.Expiry).ToList();
                var ClosestExpiries = list_Contract.Select(v => new { v.Symbol, v.Expiry }).GroupBy(g => new { g.Symbol }).Select(r => new { Underlying = r.Select(x => x.Symbol).First(), Expiry = r.Select(x => x.Expiry).First() }).ToList();
                var ClosestUnderlyingTokens = list_Contract.Where(v => v.ScripType == Exchange.ScripType.XX).Select(v => new { v.Token, v.Symbol }).GroupBy(g => new { g.Symbol }).Select(r => new { Token = r.Select(x => x.Token).First(), Underlying = r.Select(x => x.Symbol).First() }).ToList();

                foreach (var item in ClosestUnderlyingTokens)
                {
                    try
                    {
                        //if (!item.Underlying.Equals("NIFTY")) continue;

                        var Special = list_Contract.Where(v => v.Symbol.Equals(item.Underlying)).ToList();
                        DateTime Expiry = ClosestExpiries.Where(v => v.Underlying.Equals(item.Underlying)).FirstOrDefault().Expiry;

                        dict_UnderlyingToken.TryAdd(item.Underlying, item.Token);
                        dict_XXLTP.TryAdd(item.Token, new double[2] { -1, 0 });
                        dict_IVs.TryAdd(item.Token, new double[2] { 0, 0 });

                        int[] arr_CEPETokens = new int[2] { 0, 0 };
                        var dict_Strikes = Special.Where(v => v.Symbol.Equals(item.Underlying) && v.Expiry.Equals(Expiry) && v.ScripType != Exchange.ScripType.XX).Select(v => v.StrikePrice).Distinct().ToDictionary(k => k, v => arr_CEPETokens);

                        foreach (var Strike in dict_Strikes.Keys.ToList())
                        {
                            arr_CEPETokens = Special.OrderBy(v => v.ScripType).Where(v => v.Symbol.Equals(item.Underlying) && v.Expiry.Equals(Expiry) && v.StrikePrice.Equals(Strike)).Select(v => v.Token).ToArray();
                            dict_Strikes[Strike] = arr_CEPETokens;
                        }

                        dict_ClosestATMTokens.TryAdd(item.Token, new Dictionary<DateTime, Dictionary<double, int[]>> { [Expiry] = dict_Strikes });
                    }
                    catch (Exception ee)
                    {
                        InsertError("ReadContractAndFill Loop : " + ee.ToString());
                    }
                }

                //added call here on 19NOV2020 by Amey
                ReadIVFile();

                //added on 19NOV2020 by Amey
                ReadOGFile();

                //added on 11NOV2020 by Amey
                foreach (var item in list_Contract.Where(v => v.ScripType != Exchange.ScripType.XX))
                {
                    //added on 19NOV2020 by Amey
                    var _greeks = new Greeks();
                    if (dict_DefaultIVs.ContainsKey(item.Symbol))
                    {
                        _greeks.IV = dict_DefaultIVs[item.Symbol][0];
                        _greeks.IVLower = dict_DefaultIVs[item.Symbol][1];
                        _greeks.IVHigher = dict_DefaultIVs[item.Symbol][2];
                    }
                    dict_Greeks.TryAdd(item.Token, _greeks);
                }
            }
            catch (Exception ee)
            {
                InsertError("ReadContractAndFill : " + ee.ToString());
            }
        }

        private void SetAvgIV(int UnderlyingToken, string Underlying)
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

                while (true)
                {
                    double IV = 0;
                    isTokenPresent = true;

                    try
                    {
                        //if (UnderlyingToken != 51819) continue;

                        //Added on 7OCT2020 by Amey.
                        if (dict_ClosestATMTokens.ContainsKey(UnderlyingToken) && dict_XXLTP[UnderlyingToken][0] != -1)
                        {
                            double LTP = dict_XXLTP[UnderlyingToken][0];
                            DateTime Expiry = dict_ClosestATMTokens[UnderlyingToken].ElementAt(0).Key;

                            if (LTP != XXLTP)
                            {
                                XXLTP = LTP;

                                closestStrike = dict_ClosestATMTokens[UnderlyingToken][Expiry].Keys.Aggregate((accumulator, currentVal) => Math.Abs(accumulator - LTP) < Math.Abs(currentVal - LTP) ? accumulator : currentVal);
                                Tokens = dict_ClosestATMTokens[UnderlyingToken][Expiry][closestStrike];
                            }

                            //added on 15OCT2020 by Amey
                            if (Tokens[0] == 0 || Tokens[1] == 0) continue;

                            if (!dict_LTP.ContainsKey(Tokens[0]))
                            {
                                dict_LTP.TryAdd(Tokens[0], new double[2] { -1, 0 });
                                objFeed.FOSubscribe(Tokens[0].ToString());

                                isTokenPresent = false;
                            }
                            if (!dict_LTP.ContainsKey(Tokens[1]))
                            {
                                dict_LTP.TryAdd(Tokens[1], new double[2] { -1, 0 });
                                objFeed.FOSubscribe(Tokens[1].ToString());

                                isTokenPresent = false;
                            }

                            if (!isTokenPresent) continue;

                            double Time = (Expiry - DateTime.Now).TotalDays * 6.25;
                            TimeSpan ts = TimeSpan.Parse("15:30:00") - DateTime.Now.TimeOfDay;
                            if (ts.TotalSeconds > 0)
                                Time = (Time + (ts.TotalSeconds / 3600)) / 6.25;
                            else
                                Time = Time / 6.25;

                            if (dict_LTP.ContainsKey(Tokens[0]) && CELTP != dict_LTP[Tokens[0]][0])
                            {
                                CELTP = dict_LTP[Tokens[0]][0];

                                IV = ImpliedCallVolatility(LTP, closestStrike, Time / 365, 0, dict_LTP[Tokens[0]][0], 0, 0) * 100;
                                dict_IVs[UnderlyingToken][0] = IV > 0.1 ? (IV < 150 ? IV : 0) : 0;

                                isIVUpdated = true;
                            }
                            else if (dict_LTP.ContainsKey(Tokens[1]) && PELTP != dict_LTP[Tokens[1]][0])
                            {
                                PELTP = dict_LTP[Tokens[1]][0];

                                IV = ImpliedPutVolatility(LTP, closestStrike, Time / 365, 0, dict_LTP[Tokens[1]][0], 0, 0) * 100;
                                dict_IVs[UnderlyingToken][1] = IV > 0.1 ? (IV < 150 ? IV : 0) : 0;

                                isIVUpdated = true;
                            }

                            if (isIVUpdated && dict_IVs[UnderlyingToken][0] != 0 && dict_IVs[UnderlyingToken][1] != 0)
                            {
                                //added check on 22OCT2020 by Amey
                                if (dict_ComputedATMIV.ContainsKey(Underlying))
                                    dict_ComputedATMIV[Underlying][0] = Math.Round((dict_IVs[UnderlyingToken][0] + dict_IVs[UnderlyingToken][1]) / 2, 2);
                                else
                                    dict_ComputedATMIV.TryAdd(Underlying, new double[2] { Math.Round((dict_IVs[UnderlyingToken][0] + dict_IVs[UnderlyingToken][1]) / 2, 2), 0 });

                                //added on 15OCT2020 by Amey
                                if (dict_LTP[Tokens[0]][1] == 0 || dict_LTP[Tokens[0]][1] == 0 || dict_XXLTP[UnderlyingToken][1] == 0)
                                    dict_ComputedATMIV[Underlying][1] = 0;
                                else
                                    dict_ComputedATMIV[Underlying][1] = 1;
                            }
                        }
                    }
                    catch (Exception ee) { InsertError("GetAvgIV Loop : " + ee.ToString()); }

                    Thread.Sleep(200);
                }
            }
            catch (Exception ee) { InsertError("GetAvgIV : " + ee.ToString()); }
        }
    }
}