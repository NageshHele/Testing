using DevExpress.XtraGrid;
using n.Structs;
using NerveLog;
using Newtonsoft.Json;
using Prime.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Prime.Helper
{
    #region Delagates

    public delegate void del_IsStateChanged(bool flag_State, string ID = "");

    public delegate void del_HeartBeatTickReceived(string prmTick);

    public delegate void del_EditedValueReceived(double EditedValue, string FieldName);

    public delegate void del_IsFlagChanged(bool flag_ClientWindow);
    public delegate void del_ClientWindowData(string _ClientID, string _Underlying, string _Expiry);

    public delegate void del_ClientUpdateReceived();
    #endregion

    public class CollectionHelper
    {
        public static NerveLogger _logger;

        #region Flags
        /// <summary>
        /// Set value from App.Config [DEBUG-MODE] property.
        /// </summary>
        public static bool IsDebug = false;

        /// <summary>
        /// Set true when Clear Event is Called.
        /// </summary>
        public static bool IsClear = false;

        //Added by Akshay on 28-08-2021
        /// <summary>
        /// Set value from App.Config [FULL-VAR] property.
        /// </summary>
        public static bool IsFullVAR = false;

        /// <summary>
        /// Set value from App.Config [VERTICAL-LINES] property.
        /// </summary>
        public static bool IsVarticalLines = false;

        /// <summary>
        /// Set value from App.Config [] property.
        /// </summary>
        //public static bool IsVaRValueReversed = true;

        /// <summary>
        /// Default muliplier for properties present in class.
        /// </summary>
        public static ValueSigns _ValueSigns = new ValueSigns();

        //Added by Akshay on 13-06-2022 for Delivery Tab
        /// <summary>
        /// Set value when refresh butto click 
        /// </summary>
        public static bool IsDeliveryRefresh = false;

        /// <summary>
        /// Set value from VAR toggle.
        /// </summary>
        public static bool IncExpContract = true;

        #endregion

        #region Lists

        /// <summary>
        /// DataSource for gc_Scenarios
        /// </summary>
        public static List<Scenario> list_Scenarios = new List<Scenario>();

        /// <summary>
        /// Contains fieldnames of each double/decimal column from classes assigned to DataSource.
        /// </summary>
        public static List<string> list_DecimalColumns = new List<string>();

        /// <summary>
        /// Contains fieldnames of each column from classes assigned to DataSource.
        /// </summary>
        public static List<string> list_Properties = new List<string>();

        /// <summary>
        /// Contains all banned Underlyings presenf in fo_secban.csv.
        /// </summary>
        public static HashSet<string> hs_BannedUnderlyings = new HashSet<string>();

        ////Added by Akshay on 24-08-2021 For Reading VarRange File.
        ///// <summary>
        ///// Contains all Ranges For VaR Calculations.
        ///// </summary>
        //public static SortedSet<int> hs_VarRange = new SortedSet<int>();

        //Added by Akshay on 24-08-2021 For Reading VarRange File.
        /// <summary>
        /// Contains all Ranges For VaR Calculations.
        /// </summary>
        public static List<UpDownVarRange> list_UpDownVarRange = new List<UpDownVarRange>();

        #endregion

        #region Dictionaries

        /// <summary>
        /// Key : Index | Value : Divisor
        /// </summary>
        public static Dictionary<int, int> dict_DisplayValues = new Dictionary<int, int>();

        /// <summary>
        /// Key : Underlying | Value : OGInfo
        /// </summary>
        public static Dictionary<string, OGInfo> dict_OG = new Dictionary<string, OGInfo>();

        /// <summary>
        /// Key : Client|Underlying | Value : double value.
        /// </summary>
        public static Dictionary<string, double> dict_NPLValues = new Dictionary<string, double>();

        /// <summary>
        /// Key : ClientID | Value : BanInfo
        /// </summary>
        public static ConcurrentDictionary<string, BanInfo> dict_BanInfo = new ConcurrentDictionary<string, BanInfo>();

        /// <summary>
        /// Key : Username (From DB) | Value : Other info from ClientDetail table from DB.
        /// </summary>
        public static ConcurrentDictionary<string, ClientInfo> dict_ClientInfo = new ConcurrentDictionary<string, ClientInfo>();

        /// <summary>
        /// Key : ColumnName | Value : 1/1000/100000/10000000
        /// </summary>
        public static ConcurrentDictionary<string, int> dict_BaseValue = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// Key : Days | Value : Percentage
        /// </summary>
        public static ConcurrentDictionary<int, double> dict_DaysPercentage = new ConcurrentDictionary<int, double>(); //Added by Akshay on 24-03-2021

        /// <summary>
        /// Key : PropertyName | Value : Custom Column Caption
        /// </summary>
        public static ConcurrentDictionary<string, string> dict_CustomColumnNames = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Key : PropertyName | Value : Custom Digits Caption
        /// </summary>
        public static ConcurrentDictionary<string, int> dict_CustomDigits = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// Key : ClientID | Value : Key : Underlying | Value : [0] OGIndex, [1] VAR divided by lacs.
        /// </summary>
        public static ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<int, double>>> dict_VaRDistribution = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<int, double>>>();

        /// <summary>
        /// Key : ClientID | Value : Margin
        /// </summary>
        public static ConcurrentDictionary<string, double> dict_EODMargin = new ConcurrentDictionary<string, double>();

        /// <summary>
        /// Key : Client | Value : SpanInfo
        /// </summary>
        public static ConcurrentDictionary<string, ClientSpanInfo> dict_ClientWiseConsolidatedSpanInfo = new ConcurrentDictionary<string, ClientSpanInfo>();

        /// <summary>
        /// Key : Client_Underlying | Value : SpanInfo
        /// </summary>
        public static ConcurrentDictionary<string, UnderlyingSpanInfo> dict_ClientUnderlyingWiseConsolidatedSpanInfo = new ConcurrentDictionary<string, UnderlyingSpanInfo>();

        /// <summary>
        /// Key : Client | Value : Expiry Margin
        /// </summary>
        public static ConcurrentDictionary<string, double> dict_ClientWiseConsolidatedExpMarginInfo = new ConcurrentDictionary<string, double>();  //Added on 22AUG2022 by Ninad   

        /// <summary>
        /// Key : Client | Value : SpanInfo
        /// </summary>
        public static ConcurrentDictionary<string, ClientSpanInfo> dict_ClientWiseSpanInfo = new ConcurrentDictionary<string, ClientSpanInfo>();

        /// <summary>
        /// Key : Client_Underlying | Value : SpanInfo
        /// </summary>
        public static ConcurrentDictionary<string, UnderlyingSpanInfo> dict_ClientUnderlyingWiseSpanInfo = new ConcurrentDictionary<string, UnderlyingSpanInfo>();

        /// <summary>
        /// Key : Client | Value : SpanInfo
        /// </summary>
        public static ConcurrentDictionary<string, double> dict_ExpiryMargin = new ConcurrentDictionary<string, double>();

        /// <summary>
        /// Key : Client | Value : Priority Number
        /// </summary>
        public static ConcurrentDictionary<string, double> dict_Priority = new ConcurrentDictionary<string, double>();   //Added by Akshay on 18-12-2020 for Priority Feature

        /// <summary>
        /// Key : Segment|ScripToken | Value : True (always)
        /// </summary>
        public static ConcurrentDictionary<string, bool> dict_UniqueTokens = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Key : ClientID | Value : True (always)
        /// </summary>
        public static ConcurrentDictionary<string, bool> dict_UniqueClients = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Key : Segment|Token | Value : CPPositions
        /// </summary>
        public static ConcurrentDictionary<string, CPPositions> dict_ClientWindowPositions = new ConcurrentDictionary<string, CPPositions>();

        /// <summary>
        /// Key :  | Value : 
        /// </summary>
        public static ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<DateTime, string>>> dict_ComboUniverse = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<DateTime, string>>>();

        /// <summary>
        /// Key : Client | Value : [Peak Margin, Peak Margin Time]
        /// </summary>
        public static ConcurrentDictionary<string, double[]> dict_PeakMargin = new ConcurrentDictionary<string, double[]>();

        /// <summary>
        /// Key : Client | Value : [Peak Margin, Peak Margin Time]
        /// </summary>
        public static ConcurrentDictionary<string, double[]> dict_CDSPeakMargin = new ConcurrentDictionary<string, double[]>();

        /// <summary>
        /// Key : ClientID | Value : LimitInfo
        /// </summary>
        public static Dictionary<string, LimitInfo> dict_LimitInfo = new Dictionary<string, LimitInfo>();

        /// <summary>
        /// Key: Gridview Detail Level | Value: Hashset of Column Name of Portfolio Tab 
        /// </summary>
        public static Dictionary<int, HashSet<string>> dict_FixColumn = new Dictionary<int, HashSet<string>>();

        /// <summary>
        /// Key: Gridview Detail Level | Value: Hashset of Column Name of Underlying Tab 
        /// </summary>
        public static Dictionary<int, HashSet<string>> dict_ULFixColumn = new Dictionary<int, HashSet<string>>();

        /// <summary>
        /// Key: Gridview Detail Level | Value: Hashset of Column Name of AllPosition Tab 
        /// </summary>
        public static Dictionary<int, HashSet<string>> dict_APFixColumn = new Dictionary<int, HashSet<string>>();

        // Added by Snehadri on 19JUL2021 for Expression Column
        /// <summary>
        /// Key: Gridview Detail Level | Value: Dictionary with Expression column name as key and expression as value
        /// </summary>
        public static Dictionary<int, Dictionary<string, string>> dict_ExpressionColumn = new Dictionary<int, Dictionary<string, string>>();

        // Added by Snehadri on 11AUG2021 for Rule Builder
        /// <summary>
        /// Key: Rule Name | Value : Rule Info
        /// </summary>
        public static Dictionary<string, RuleAlert> dict_RuleInfo = new Dictionary<string, RuleAlert>();



        //Added by Akshay on 31-12-2021 for CDS
        /// <summary>
        /// Key : Client | Value : SpanInfo
        /// </summary>
        public static ConcurrentDictionary<string, ClientSpanInfo> dict_CDSClientWiseSpanInfo = new ConcurrentDictionary<string, ClientSpanInfo>();


        /// <summary>
        /// Key : Client_Underlying | Value : SpanInfo
        /// </summary>
        public static ConcurrentDictionary<string, UnderlyingSpanInfo> dict_CDSClientUnderlyingWiseSpanInfo = new ConcurrentDictionary<string, UnderlyingSpanInfo>();


        /// <summary>
        /// Key : Client | Value : SpanInfo
        /// </summary>
        public static ConcurrentDictionary<string, double> dict_CDSExpiryMargin = new ConcurrentDictionary<string, double>();

        /// <summary>
        /// Key : ClientID | Value : Margin
        /// </summary>
        public static ConcurrentDictionary<string, double> dict_CDSEODMargin = new ConcurrentDictionary<string, double>();

        #endregion

        #region BindingLists

        /// <summary>
        /// DataSource for ConcentrationRisk grid.
        /// </summary>
        public static BindingList<SpanMargin> bList_ConcentrationRisk = new BindingList<SpanMargin>();

        /// <summary>
        /// DataSource for ClientPortfolio grid.
        /// </summary>
        public static BindingList<CPParent> bList_ClientPortfolio = new BindingList<CPParent>();

        /// <summary>
        /// DataSource for UnderlyingClients grid.
        /// </summary>
        public static BindingList<UCParent> bList_UnderlyingClients = new BindingList<UCParent>();

        //Added by Akshay on 28-07-2021 for Client window
        /// <summary>
        /// DataSource for _ClientWindow grid.
        /// </summary>
        public static BindingList<CWPositions> bList_ClientWindow = new BindingList<CWPositions>();

        //Added by Akshay on 28-07-2021 for Client window
        /// <summary>
        /// DataSource for ClientWindowOptions grid.
        /// </summary>
        public static BindingList<CWOptions> bList_ClientWindowOptions = new BindingList<CWOptions>();

        //Added by Akshay on 28-07-2021 for Client window
        /// <summary>
        /// DataSource for ClientWindowFutures grid.
        /// </summary>
        public static BindingList<CWFutures> bList_ClientWindowFutures = new BindingList<CWFutures>();

        //Added by Akshay on 28-07-2021 for Client window
        /// <summary>
        /// DataSource for ClientWindowGreeks grid.
        /// </summary>
        public static BindingList<CWGreeks> bList_ClientWindowGreeks = new BindingList<CWGreeks>();

        //Added by Akshay on 25-10-2021 for Delivery Report
        /// <summary>
        /// DataSource for Delivery Report grid.
        /// </summary>
        public static BindingList<DRUnderlying> bList_DeliveryReport = new BindingList<DRUnderlying>();

        #endregion

        #region UI Elements

        /// <summary>
        /// ClientPortfolio gridcontol.
        /// </summary>
        public static GridControl gc_CP;

        // <summary>
        /// UnderlyingClients gridcontol.
        /// </summary>
        public static GridControl gc_UC;


        //Added by Akshay on 25-10-2021 for Delivery Report
        // <summary>
        /// UnderlyingClients gridcontol.
        /// </summary>
        public static GridControl gc_DR;

        /// <summary>
        /// Lock to SELECT anything in ClientWindow grid datasource.
        /// </summary>
        public static object _CWLock = new object();

        #endregion

        #region Locks

        /// <summary>
        /// Lock to SELECT anything in ClientPortfolio grid datasource.
        /// </summary>
        public static object _CPLock = new object();

        /// <summary>
        /// Lock to SELECT anything in UnderlyingClients grid datasource.
        /// </summary>
        public static object _CULock = new object();

        /// <summary>
        /// Lock to SELECT anything in DeliveryReport grid datasource.
        /// </summary>
        public static object _DRLock = new object();

        #endregion


        public static List<string> list_SqTrades = new List<string>();

        /// <summary>
        /// Stores the data font size
        /// </summary>
        public static int DataFontSize;

        /// <summary>
        /// Stores the footer font size
        /// </summary>
        public static int FooterFontSize;

        /// <summary>
        /// Time seconds for socket Trade/Socket/Hb connectio
        /// </summary>
        public static int TimeoutSeconds = 60;

        public static List<ConcentrationRiskMargin> list_UnderlyingMargin = new List<ConcentrationRiskMargin>();

        public static HashSet<string> hs_ColumnNames = new HashSet<string>();

        public static void Initialise()
        {
            IsDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["DEBUG-MODE"]);
            _logger = new NerveLogger(true, IsDebug, ApplicationName: "n.Prime");
            _logger.Initialize(Application.StartupPath);
            DeleteOldLogFiles(Application.StartupPath + "\\n.Prime-LOG");   //added on 22AUG2022 by Ninad

            GetPropertiesList();
        }
        //added by ninad for auto-delete old log files/folders
        public static void DeleteOldLogFiles(string logfilepath)
        {
            DirectoryInfo dir = new DirectoryInfo(logfilepath);
            DirectoryInfo[] subDirs = dir.GetDirectories().OrderByDescending(p => p.CreationTime).ToArray();

            if (subDirs.Length > 7)
            {
                for (var index = 7; index < subDirs.Length; index++)
                {
                    Directory.Delete(subDirs[index].FullName, true);
                }
            }
        }

        public static void InitialiseCollections()
        {
            try
            {
                string Path = Application.StartupPath + "\\Report";
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);

                Path = Application.StartupPath + "\\Layout";
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);

                //added on 2JUN2021 by Amey
                RefreshPriority();

                //Added by Akshay on 24-08-2021 for Reading VarRange File.
                ReadVarRangeFile();

                //ADDED BY NIKHIL ON 08SEP2022
                dict_LimitInfo= ReadLimitFile();

                if (File.Exists(Application.StartupPath + "\\Report\\ScenarioAnalysis.txt"))
                {
                    string txt = File.ReadAllText(Application.StartupPath + "\\Report\\ScenarioAnalysis.txt");
                    var tx = JsonConvert.DeserializeObject<List<Scenario>>(txt);
                    if (tx != null)
                        list_Scenarios = tx;
                }

                dict_DisplayValues.Add(0, 1);
                dict_DisplayValues.Add(1, 1000);
                dict_DisplayValues.Add(2, 100000);
                dict_DisplayValues.Add(3, 10000000);

                //added on 24FEB2021 by Amey
                if (File.Exists(Application.StartupPath + "\\Report\\BaseValues.txt"))
                {
                    string txt = File.ReadAllText(Application.StartupPath + "\\Report\\BaseValues.txt");
                    var tx = JsonConvert.DeserializeObject<ConcurrentDictionary<string, int>>(txt);
                    if (tx != null)
                        dict_BaseValue = tx;
                }

                //added on 29APR2021 by Amey. To set default value for newly added columns.
                foreach (var item in list_DecimalColumns)
                    dict_BaseValue.TryAdd(item, 0);

                foreach (var _Val in dict_BaseValue.Keys)
                    dict_BaseValue[_Val] = dict_DisplayValues[dict_BaseValue[_Val]];

                //Added by Akshay on 24-03-2021
                if (File.Exists(Application.StartupPath + "\\Report\\DelMarginDaysPercentage.txt"))
                {
                    string txt = File.ReadAllText(Application.StartupPath + "\\Report\\DelMarginDaysPercentage.txt");
                    var tx = JsonConvert.DeserializeObject<ConcurrentDictionary<int, double>>(txt);
                    if (tx != null)
                        dict_DaysPercentage = tx;
                }

                //added on 22MAR2021 by Amey
                if (File.Exists(Application.StartupPath + "\\Report\\CustomColumns.json"))
                {
                    string txt = File.ReadAllText(Application.StartupPath + "\\Report\\CustomColumns.json");
                    var tx = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(txt);
                    if (tx != null)
                        dict_CustomColumnNames = tx;
                }

                //added on 24MAY2021 by Amey
                if (File.Exists(Application.StartupPath + "\\Report\\CustomDigits.json"))
                {
                    string txt = File.ReadAllText(Application.StartupPath + "\\Report\\CustomDigits.json");
                    var tx = JsonConvert.DeserializeObject<ConcurrentDictionary<string, int>>(txt);
                    if (tx != null)
                        dict_CustomDigits = tx;
                }

                foreach (var item in list_DecimalColumns)
                    dict_CustomDigits.TryAdd(item, 2);

                //added on 25MAY2021 by Amey
                if (File.Exists(Application.StartupPath + "\\Report\\ValueSigns.json"))
                {
                    string txt = File.ReadAllText(Application.StartupPath + "\\Report\\ValueSigns.json");
                    var tx = JsonConvert.DeserializeObject<ValueSigns>(txt);
                    if (tx != null)
                        _ValueSigns = tx;
                }

                //Added by Snehadri on  05JUL2021 for fix column feature
                if (File.Exists(Application.StartupPath + "\\Report\\Portfolio_FixColumns.json"))
                {
                    string text = File.ReadAllText(Application.StartupPath + "\\Report\\Portfolio_FixColumns.json");
                    var tx = JsonConvert.DeserializeObject<Dictionary<int, HashSet<string>>>(text);
                    if (tx != null)
                        dict_FixColumn = tx;
                }

                //Added by Snehadri on  21OCT2021 for fix column feature
                if (File.Exists(Application.StartupPath + "\\Report\\Underlying_FixColumns.json"))
                {
                    string text = File.ReadAllText(Application.StartupPath + "\\Report\\Underlying_FixColumns.json");
                    var tx = JsonConvert.DeserializeObject<Dictionary<int, HashSet<string>>>(text);
                    if (tx != null)
                        dict_ULFixColumn = tx;
                }
                
                //Added by Snehadri on  21OCT2021 for fix column feature
                if (File.Exists(Application.StartupPath + "\\Report\\AllPosition_FixColumns.json"))
                {
                    string text = File.ReadAllText(Application.StartupPath + "\\Report\\AllPosition_FixColumns.json");
                    var tx = JsonConvert.DeserializeObject<Dictionary<int, HashSet<string>>>(text);
                    if (tx != null)
                        dict_APFixColumn = tx;
                }

                if (File.Exists(Application.StartupPath + "\\Report\\ExpressionColumns.json"))
                {
                    string text = File.ReadAllText(Application.StartupPath + "\\Report\\ExpressionColumns.json");
                    var tx = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, string>>>(text);
                    if (tx != null)
                        dict_ExpressionColumn = tx;
                }

                // Added by Snehadri on 11AUG2021 for Rule Builder
                if (File.Exists(Application.StartupPath + "\\Report\\Rule.json"))
                {
                    string txt = File.ReadAllText(Application.StartupPath + "\\Report\\Rule.json");
                    var tx = JsonConvert.DeserializeObject<Dictionary<string, RuleAlert>>(txt);
                    if (tx != null)
                        dict_RuleInfo = tx;
                   
                }
               
                // Added by Snehadri on 30AUG2021 for changing font size
                var CONInfo = ConfigurationManager.AppSettings;
                DataFontSize = int.Parse(CONInfo["DATA-FONT-SIZE"].ToString()); 
                FooterFontSize = int.Parse(CONInfo["FOOTER-FONT-SIZE"].ToString());
                TimeoutSeconds = int.Parse(CONInfo["TIME-OUT-SECONDS"].ToString());
                
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private static void GetPropertiesList()
        {
            AddToLists(typeof(CPParent).GetProperties());
            AddToLists(typeof(CPUnderlying).GetProperties());
            AddToLists(typeof(CPPositions).GetProperties());
            AddToLists(typeof(BanInfo).GetProperties());
            AddToLists(typeof(SpanMargin).GetProperties());
            AddToLists(typeof(ConsolidatedPositionInfo).GetProperties());
            AddToLists(typeof(UCParent).GetProperties());
            AddToLists(typeof(UCClient).GetProperties());
        }

        private static void AddToLists(PropertyInfo[] arr_Properties)
        {
            foreach (var property in arr_Properties)
            {
                if (property.PropertyType == typeof(double) || property.PropertyType == typeof(decimal) || property.PropertyType == typeof(int)
                    || property.PropertyType == typeof(long) || property.PropertyType == typeof(string))
                {
                    if (property.PropertyType == typeof(double) || property.PropertyType == typeof(decimal))
                    {
                        if (!list_DecimalColumns.Contains(property.Name))
                            list_DecimalColumns.Add(property.Name);
                    }
                    
                }               

                if (!property.Name.ToLower().StartsWith("blist"))
                {
                    hs_ColumnNames.Add(property.Name);
                }
            }
        }

        public static void RefreshPriority()
        {
            try
            {
                string PriorityFile = Application.StartupPath + "\\Layout\\" + "PriorityFile.csv";

                if (File.Exists(PriorityFile))
                {
                    var arr_Lines = File.ReadAllLines(PriorityFile);

                    foreach (var line in arr_Lines)
                    {
                        try
                        {
                            var arr_Fields = line.Split(',');
                            var _ID = arr_Fields[0].ToUpper();

                            if (dict_Priority.ContainsKey(_ID))
                                dict_Priority[_ID] = Convert.ToDouble(arr_Fields[1]);
                            else
                                dict_Priority.TryAdd(_ID, Convert.ToDouble(arr_Fields[1]));
                        }
                        catch (Exception ee) { _logger.Error(ee, "Menu_RefreshPriority_Click -loop : " + line); }
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        public static Dictionary<string, LimitInfo> ReadLimitFile()
        {
            var dict_Limit = new Dictionary<string, LimitInfo>();

            try
            {
                var LimitFile = Application.StartupPath+"\\Report\\LimitFile.csv";
                if (File.Exists(LimitFile))
                {
                    var arr_Lines = File.ReadAllLines(LimitFile);
                    foreach (var line in arr_Lines)
                    {
                        try
                        {
                            var arr_Fields = line.Split(',').Select(v => v.Trim().ToUpper()).ToArray();

                            if (dict_Limit.ContainsKey(arr_Fields[1]))
                                dict_Limit[arr_Fields[0]] = new LimitInfo() { MTMLimit = Convert.ToDouble(arr_Fields[1]), VARLimit = Convert.ToDouble(arr_Fields[2]), MarginLimit = Convert.ToDouble(arr_Fields[3]), BankniftyExpoLimit = Convert.ToDouble(arr_Fields[4]), NiftyExpoLimit = Convert.ToDouble(arr_Fields[5]) };
                            else
                                dict_Limit.Add(arr_Fields[0], new LimitInfo() { MTMLimit = Convert.ToDouble(arr_Fields[1]), VARLimit = Convert.ToDouble(arr_Fields[2]), MarginLimit = Convert.ToDouble(arr_Fields[3]), BankniftyExpoLimit = Convert.ToDouble(arr_Fields[4]), NiftyExpoLimit = Convert.ToDouble(arr_Fields[5]) });
                        }
                        catch (Exception ee) { _logger.Error(ee,"ReadLimitFile : " + line ); }
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee,"ReadLimitFile : "); }

            return dict_Limit;
        }


        //Added by Akshay on 28-08-2021 for reading varRange File
        public static void ReadVarRangeFile()
        {
            try
            {
                string UpDownVarRangeFile = Application.StartupPath + "\\Report\\" + "VarRange.csv";

                var arr_Lines = File.ReadAllLines(UpDownVarRangeFile);

                foreach (var line in arr_Lines)
                {
                    var arr_Fields = line.Split(',');
                    var UpDownVarRange = new UpDownVarRange()
                    {
                        Underlying = arr_Fields[0].Trim().ToString().ToUpper(),
                        SS_UpDownVarRange = new SortedSet<int>() { Convert.ToInt32(arr_Fields[1]), Convert.ToInt32(arr_Fields[2]), Convert.ToInt32(arr_Fields[3]), Convert.ToInt32(arr_Fields[4]) },
                    };
                    list_UpDownVarRange.Add(UpDownVarRange);
                }
            }
            catch (Exception ee)
            {
                try
                {
                    list_UpDownVarRange.Clear();
                    list_UpDownVarRange.Add(new UpDownVarRange() { Underlying = "ALL", SS_UpDownVarRange = new SortedSet<int>() { -10, -5, 5, 10 } });
                    list_UpDownVarRange.Add(new UpDownVarRange() { Underlying = "NIFTY", SS_UpDownVarRange = new SortedSet<int>() { -20, -10, 10, 20 } });
                    list_UpDownVarRange.Add(new UpDownVarRange() { Underlying = "BANKNIFTY", SS_UpDownVarRange = new SortedSet<int>() { -20, -10, 10, 20 } });
                }
                catch (Exception eee)
                {
                    _logger.Error(eee);
                }
                _logger.Error(ee);
            }
        }
    }
}
