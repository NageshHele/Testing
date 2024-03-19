using MySql.Data.MySqlClient;
using n.Structs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Core_Logic
{
    delegate void del_SpanRead(Span q_SpanRequests);
    delegate void del_CDSSpanRead(Span q_CDSSpanRequests);  //CDS
    delegate void del_TradeTime(double _FOLastTradeTime, double _CMLastTradeTime, double _CDLastTradeTime,double _BSECMLastTradeTime, double _BSEFOLastTradeTime);

    class CTradeProcess
    {
        clsWriteLog _logger;

        DataSet ds_Config;

        #region Events

        public event del_SpanRead eve_SpanRead;
        public event del_CDSSpanRead eve_CDSSpanRead;   //CDS
        public event del_TradeTime eve_TradeTime;
        public event del_TradeFileStatus eve_TradeFileStatus;

        #endregion

        #region Variables

        bool isDebug;
        bool isTradeFileFOAvailable = false;
        bool isTradeFileCMAvailable = false;
        bool isTradeFileCDAvailable = false;
        bool isTradeFileBSECMAvailable = false;
        bool isTradeFileBSEFOAvailable = false;
        bool isTradeFileCPAvailable = false;
        double _FOLastTradeTime = 0;
        double _CMLastTradeTime = 0;
        double _CDLastTradeTime = 0;
        double _BSECMLastTradeTime = 0;
        double _BSEFOLastTradeTime = 0;
        const int TotalCombinations = 4;
        const int TotalCombinationsCP = 1;
        //added on 25JAN2021 by Amey
        internal string _TradeFileFO = "TradeFO_" + DateTime.Now.ToString("ddMMyyyy");
        internal string _TradeFileCM = "TradeCM_" + DateTime.Now.ToString("ddMMyyyy");
        internal string _TradeFileCD = "TradeCD_" + DateTime.Now.ToString("ddMMyyyy");

        //added on 20APR2021 by Amey
        internal string _TradeFileBSECM = "EQ_ITR_*_" + DateTime.Now.ToString("yyyyMMdd");
        //EQD_ITRCM_910_20230602
        internal string _TradeFileBSEFO = "EQD_ITRTM_*_" + DateTime.Now.ToString("yyyyMMdd");
        //Added by Akshay on 02-08-2022 for Reading New trade file
        internal string _TradeFileCP = "EQD_ITRCM_*_" + DateTime.Now.ToString("yyyyMMdd");
        
        List<NotisDbConfiguration> lst_NotisConfigurationsFO = new List<NotisDbConfiguration>();
        List<NotisDbConfiguration> lst_NotisConfigurationsCM = new List<NotisDbConfiguration>();
        List<NotisDbConfiguration> lst_NotisConfigurationsCD = new List<NotisDbConfiguration>();
        string _MySQLTradeCon;
        int id_FOCount = 0;
        int id_CMCount = 0;
        int id_BSECMCount = 0;
        int id_CDCount = 0;

        #endregion

        #region List/Arrays

        List<PositionInfo> list_AllTrades = new List<PositionInfo>();

        /// <summary>
        /// [0] ClientID|CTCLID|UserID, [1] ClientID, [2] CTCLID, [3] UserID
        /// </summary>
        private readonly string[] arr_IDCombinations = new string[TotalCombinations] { "ClientID|CTCLID|UserID", "ClientID", "CTCLID", "UserID" };

        HashSet<string> hs_RandomEntriesFO = new HashSet<string>();
        HashSet<string> hs_RandomEntriesCM = new HashSet<string>();

        #endregion

        #region Dictionaries

        //Changed to HashSet on 22AUG2020 by Amey
        //changed to ConcurrentDictionary on 22OCT2020 by Amey.
        ConcurrentDictionary<string, HashSet<string>> dict_RandomEntriesCM = new ConcurrentDictionary<string, HashSet<string>>();
        ConcurrentDictionary<string, HashSet<string>> dict_RandomEntriesFO = new ConcurrentDictionary<string, HashSet<string>>();
        ConcurrentDictionary<string, HashSet<string>> dict_RandomEntriesBSECM = new ConcurrentDictionary<string, HashSet<string>>();
        ConcurrentDictionary<string, HashSet<string>> dict_RandomEntriesBSEFO = new ConcurrentDictionary<string, HashSet<string>>();
        ConcurrentDictionary<string, HashSet<string>> dict_RandomEntriesCD = new ConcurrentDictionary<string, HashSet<string>>();
        ConcurrentDictionary<string, HashSet<string>> dict_RandomEntriesCP = new ConcurrentDictionary<string, HashSet<string>>();

        // <summary>
        /// Key => CTCL_ID,ClientCode,UserID,[ClientCode|CTCL_ID|UserID] | Value => Username
        /// </summary>
        ConcurrentDictionary<string, HashSet<string>> dict_ClientInfo = new ConcurrentDictionary<string, HashSet<string>>();

        /// <summary>
        /// Key : Segment|CustomScripName | Value : ScripInfo
        /// </summary>
        ConcurrentDictionary<string, ContractMaster> dict_CustomScripInfo = new ConcurrentDictionary<string, ContractMaster>();

        /// <summary>
        /// Key : Segment|ScripName | Value : ScripInfo
        /// </summary>
        ConcurrentDictionary<string, ContractMaster> dict_ScripInfo = new ConcurrentDictionary<string, ContractMaster>();

        /// <summary>
        /// Key : Segment|Token | Value : ScripInfo
        /// </summary>
        ConcurrentDictionary<string, ContractMaster> dict_TokenScripInfo = new ConcurrentDictionary<string, ContractMaster>();

        //Added by Akshay on 01-09-2021 for storing line Counter.
        /// <summary>
        /// Key : FullFilePath | Value : LineCount
        /// </summary>
        ConcurrentDictionary<string, long> dict_CharToSkip = new ConcurrentDictionary<string, long>();

        HashSet<string> hs_BrokerID = new HashSet<string>();
        //Added by Snehadri on 23FEB2022
        /// <summary>
        /// Key: NSE Symbols | Value: Corresponding BSE Symbols
        /// </summary>
        Dictionary<string, string> dict_BSEMapping = new Dictionary<string, string>();
        #endregion

        int FOTradeCount = 0;
        int CMTradeCount = 0;
        int CDTradeCount = 0;
        int BSETradeCount = 0;
        int CPTradeCount = 0;
        int Trade_Batch_Count = 10000;

        private CTradeProcess(clsWriteLog _logger, DataSet ds_Config, bool isDebug,
           ConcurrentDictionary<string, ContractMaster> dict_CustomScripInfo, 
           ConcurrentDictionary<string, ContractMaster> dict_ScripInfo,
           ConcurrentDictionary<string, ContractMaster> dict_TokenScripInfo, Dictionary<string, string> dict_RecvBSEMapping,
           HashSet<string> hs_BrokerID, int trade_batch_count)
        {
            this.ds_Config = ds_Config;
            this.isDebug = isDebug;
            this._logger = _logger;
            this.dict_CustomScripInfo = dict_CustomScripInfo;
            this.dict_ScripInfo = dict_ScripInfo;
			this.dict_TokenScripInfo = dict_TokenScripInfo;
            this.hs_BrokerID = hs_BrokerID;
            this.Trade_Batch_Count = trade_batch_count;
            this.dict_BSEMapping = dict_RecvBSEMapping;

            GETDBInfo();
        }

        #region Instance Initializing

        public static CTradeProcess Instance { get; private set; }

        public static void Initialise(clsWriteLog _logger, DataSet ds_Config, bool isDebug,
            ConcurrentDictionary<string, ContractMaster> dict_CustomScripInfo,
            ConcurrentDictionary<string, ContractMaster> dict_ScripInfo,
            ConcurrentDictionary<string, ContractMaster> dict_TokenScripInfo,
            Dictionary<string, string> dict_RecvBSEMapping,
            HashSet<string> hs_BrokerID, int trade_batch_count)
        {
            if (Instance is null)
                Instance = new CTradeProcess(_logger, ds_Config, isDebug, dict_CustomScripInfo, dict_ScripInfo, dict_TokenScripInfo, dict_RecvBSEMapping, hs_BrokerID, trade_batch_count);
        }

        #endregion
        
        #region Imp Methods

        internal void EngineRestarted()
        {
            _FOLastTradeTime = 0;
            _CMLastTradeTime = 0;
            _CDLastTradeTime = 0;

            dict_RandomEntriesCM.Clear();
            dict_RandomEntriesCD.Clear();
            dict_RandomEntriesFO.Clear();

            //added on 26APR2021 by Amey
            dict_RandomEntriesBSECM.Clear();

            dict_RandomEntriesBSEFO.Clear();
            //Added by Akshay on 01-09-2021 for LineCounter
            dict_CharToSkip.Clear();

            //changed on 12JAN2021 by Amey
            list_AllTrades.Clear();
        }

        private void GETDBInfo()
        {
            try
            {
                //XmlTextReader tReader = new XmlTextReader("C:/Prime/Config.xml");
                //tReader.Read();
                //ds_Config.ReadXml(tReader);
                var Intraday = ds_Config.Tables["INTRADAY"].Rows[0];

                //var SqlServerOmne= ds_Config.Tables["SQL-SERVER-OMNE"].Rows[0];

                var NOofDbs = Convert.ToInt32(ds_Config.Tables["OTHER"].Rows[0]["NOTIS-DB-COUNT"]);
               

                for (int i = 1; i <= NOofDbs; i++)
                {
                    var _SqlServerOmne = ds_Config.Tables["DB-TRADE-" + i].Rows[0];
                    var _ConnString = new MySqlConnectionStringBuilder()
                    {
                        Server = _SqlServerOmne["SERVER"].ToString(),
                        Database = _SqlServerOmne["NAME"].ToString(),
                        Port = Convert.ToUInt32(_SqlServerOmne["PORT"].ToString()),
                        UserID = _SqlServerOmne["USER"].ToString(),
                        Password = _SqlServerOmne["PASSWORD"].ToString(),
                        
                    };
                    //var _tableName = _SqlServerOmne["TABLE-NAME"].ToString();
                    lst_NotisConfigurationsFO.Add(new NotisDbConfiguration { ConnectionString = _ConnString.ConnectionString, IdCount= 0 });
                    lst_NotisConfigurationsCM.Add(new NotisDbConfiguration { ConnectionString = _ConnString.ConnectionString, IdCount = 0 });
                    lst_NotisConfigurationsCD.Add(new NotisDbConfiguration { ConnectionString = _ConnString.ConnectionString, IdCount = 0 });

                }
            }
            catch (Exception EE)
            {
                _logger.Error("ReadDBInfo " + EE.ToString());

            }
        }


        internal void UpdateCollections(ConcurrentDictionary<string, HashSet<string>> dict_ClientInfo)
        {
            this.dict_ClientInfo = dict_ClientInfo;
        }

        /// <summary>
        /// Will throw events as soon as Reading is complete.
        /// </summary>
        internal List<PositionInfo> GetTrades(bool ClearTrades)
        {
            //added on 23JUN2021 by Amey. To avoid clearing trades if sending is unsuccessful.
            if (ClearTrades)
                list_AllTrades.Clear();

            //added for testing omkar
            //ReadCD();
            
            Parallel.Invoke(() => ReadFO(), () => ReadCM(), () => ReadBSECM(), () => ReadBSEFO(), () => ReadCD(),()=>ReadCP());//, () => ReadBSECM(), () => ReadGiveUpTrades());

            eve_TradeFileStatus(isTradeFileFOAvailable, isTradeFileCMAvailable, isTradeFileBSECMAvailable, isTradeFileBSEFOAvailable, isTradeFileCDAvailable);
            eve_TradeTime(_FOLastTradeTime, _CMLastTradeTime, _CDLastTradeTime, _BSECMLastTradeTime, _BSEFOLastTradeTime);

            
            return list_AllTrades;
        }

        #region NOTISE-DB

        private void ReadFO()
        {
            try
            {
                PositionInfo _PositionInfo;

                var arr_Combinations = new string[TotalCombinations] { "", "", "", "" };

                //added on 19APR2021 by Amey
                var CustomScripNameKey = string.Empty;
                var Symbol = string.Empty;
                var ScripType = en_ScripType.EQ;
                var StrikePrice = 0.0;
                DateTime _ExpiryDate = new DateTime(1980, 1, 1, 0, 0, 0);

                foreach (var DbConfiguration in lst_NotisConfigurationsFO)
                {

                    //added on 03MAR2021 by Amey
                    var dt_FORows = new DataTable();
                    using (MySqlConnection _MySqlConnection = new MySqlConnection(DbConfiguration.ConnectionString))
                    {
                        _MySqlConnection.Open();

                        using (MySqlCommand _MySqlCommand = new MySqlCommand("sp_GetFOTradesN", _MySqlConnection))
                        {
                            _MySqlCommand.CommandType = CommandType.StoredProcedure;

                            _MySqlCommand.Parameters.Add("prm_ID", MySqlDbType.Int32);
                            _MySqlCommand.Parameters["prm_ID"].Value = DbConfiguration.IdCount;

                            using (MySqlDataAdapter _MySqlAdapter = new MySqlDataAdapter(_MySqlCommand))
                            {
                                //changed on 13JAN2021 by Amey
                                _MySqlAdapter.Fill(dt_FORows);
                                _MySqlAdapter.Dispose();
                            }
                        }

                        _MySqlConnection.Close();
                    }

                    if (!dict_RandomEntriesFO.ContainsKey(DbConfiguration.ConnectionString))
                        dict_RandomEntriesFO.TryAdd(DbConfiguration.ConnectionString, new HashSet<string>());

                    foreach (DataRow dRow in dt_FORows.Rows)
                    {
                        try
                        {
                            var _ID = Convert.ToInt32(dRow["id"]);
                            DbConfiguration.IdCount = _ID > DbConfiguration.IdCount ? _ID : DbConfiguration.IdCount;

                            string ClientCode = dRow["Client_account_number"].ToString().Trim().ToUpper();
                            string FullDealerID = dRow["CTCL_code"].ToString().Trim().ToUpper();
                            string CTCL_ID = FullDealerID.Substring(0, (FullDealerID.Length > 11 ? 12 : FullDealerID.Length));
                            string UserId = dRow["User_id"].ToString().Trim().ToUpper();
                            string ClientInfokey = $"{ClientCode}|{CTCL_ID}|{UserId}";

                            arr_Combinations = new string[TotalCombinations] { ClientInfokey, ClientCode, CTCL_ID, UserId };

                            //changed on 10FEB2021 by Amey
                            _PositionInfo = new PositionInfo();

                            bool isValidTrade = false;
                            bool isErrorCode = true;
                            for (int i = 0; i < arr_Combinations.Length; i++)
                            {
                                try
                                {
                                    //changed location at top on 28JUL2021 by Amey. To avoid skipping if not valid trade.
                                    //added on 01DEC2020 by Amey

                                    //changed location at top on 28JUL2021 by Amey. To avoid skipping if not valid trade.

                                    var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSEFO;
                                    if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

                                    var check = dRow["Trade_number"].ToString() + "_" + ClientCode;
                                    if (dict_RandomEntriesFO[DbConfiguration.ConnectionString].Contains(check)) continue;

                                    //added on 19APR2021 by Amey
                                    _PositionInfo.Segment = en_Segment.NSEFO;

                                    //added on 20APR2021 by Amey
                                    Symbol = dRow["Symbol"].ToString().Trim().ToUpper();
                                    ScripType = dRow["Option_Type"].ToString().Trim().ToUpper() == "CE" ? en_ScripType.CE :
                                        (dRow["Option_Type"].ToString().Trim().ToUpper() == "PE" ? en_ScripType.PE : en_ScripType.XX);
                                    StrikePrice = Convert.ToDouble(ScripType == en_ScripType.XX ? "0" : dRow["Strike_price"].ToString());
                                    _ExpiryDate = Convert.ToDateTime(dRow["Expiry_date"]);

                                    //added on 19APR2021 by Amey
                                    ContractMaster _ScripInfo = null;
                                    CustomScripNameKey = _PositionInfo.Segment.ToString() + "|" + $"{Symbol}|{_ExpiryDate.ToString("ddMMMyyyy").ToUpper()}|{(StrikePrice == 0 ? "0" : StrikePrice.ToString("#.00"))}|{ScripType}";

                                    if (!dict_CustomScripInfo.TryGetValue(CustomScripNameKey, out _ScripInfo))
                                    {
                                        if (!dict_ScripInfo.TryGetValue(_PositionInfo.Segment +"|"+ dRow["Scrip_name"], out _ScripInfo))
                                        {
                                            if (isDebug)
                                                _logger.Error("ReadFO Skipped : " + CustomScripNameKey, true);

                                            continue;
                                        }
                                    }

                                    _PositionInfo.TradePrice = Convert.ToDouble(dRow["Trade_price"]);

                                    //added on 07APR2021 by Amey
                                    var Qty = long.Parse(dRow["Trade_quantity"].ToString());
                                    if (dRow["Buy_Sell_flag"].ToString().Trim().ToUpper() == "2")
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

                                    //changed to - on 13APR2021 by Amey
                                    //changed computation logic on 07APR2021 by Amey
                                    _PositionInfo.IntradayValue = _PositionInfo.TradeValue; // _PositionInfo.BuyValue - _PositionInfo.SellValue;

                                    //changed on 19APR2021 by Amey
                                    _PositionInfo.Token = _ScripInfo.Token;
                                    _PositionInfo.UnderlyingToken = _ScripInfo.UnderlyingToken;
                                    _PositionInfo.UnderlyingSegment = _ScripInfo.UnderlyingSegment;

                                    DateTime _TradeTime = Convert.ToDateTime(dRow["Trade_time"]);
                                    double dbl_TradeTime = ConvertToUnixTimestamp(_TradeTime);
                                    _FOLastTradeTime = dbl_TradeTime > _FOLastTradeTime ? dbl_TradeTime : _FOLastTradeTime;

                                    _PositionInfo.TradeTime = _TradeTime.ToString("ddMMMyyyy HH:mm:ss").ToUpper();

                                    var TradeID = dRow["Trade_number"].ToString();

                                    _PositionInfo.TradeID = TradeID;

                                    dict_RandomEntriesFO[DbConfiguration.ConnectionString].Add(check);

                                    isValidTrade = true;

                                    break;
                                }
                                catch (Exception ee) { _logger.Error("ReadFO Loop -inner : " + string.Join(",", dRow.ItemArray) + Environment.NewLine + ee); }
                            }

                            if (isValidTrade)
                            {
                                for (int i = 0; i < arr_Combinations.Length; i++)
                                {
                                    var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSEFO;

                                    HashSet<string> hs_Username = new HashSet<string>();

                                    if (!dict_ClientInfo.TryGetValue(UniqueID, out hs_Username)) continue;

                                    foreach (var Username in hs_Username)
                                    {
                                        #region Span using dll
                                        try
                                        {
                                            Span _spn = new Span()
                                            {
                                                pMemberId = 1,
                                                pClientId = Username + "_" + Symbol,
                                                pExchange = "NSE",
                                                pSegment = "EQFO",
                                                pScripName = Symbol,
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

                                            eve_SpanRead(_spn);
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
                        catch (Exception ee) { _logger.Error("ReadFO Loop : " + string.Join(",", dRow.ItemArray) + Environment.NewLine + ee); }
                    }

                }
            }
            catch (Exception foEX)
            {
                _logger.Error("Read FO " + foEX.ToString());
            }
        }

        private void ReadCM()
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
                var Series = "EQ";

                foreach (var DbConfiguration in lst_NotisConfigurationsCM)
                {
                   

                    //added on 03MAR2021 by Amey
                    var dt_CMRows = new DataTable();
                    using (MySqlConnection _MySqlConnection = new MySqlConnection(DbConfiguration.ConnectionString))
                    {
                        _MySqlConnection.Open();

                        using (MySqlCommand _MySqlCommand = new MySqlCommand("sp_GetCMTradesN", _MySqlConnection))
                        {
                            _MySqlCommand.CommandType = CommandType.StoredProcedure;

                            _MySqlCommand.Parameters.Add("prm_ID", MySqlDbType.Int32);
                            _MySqlCommand.Parameters["prm_ID"].Value = DbConfiguration.IdCount;

                            using (MySqlDataAdapter _MySqlAdapter = new MySqlDataAdapter(_MySqlCommand))
                            {
                                //changed on 13JAN2021 by Amey
                                _MySqlAdapter.Fill(dt_CMRows);

                                _MySqlAdapter.Dispose();
                            }
                        }

                        _MySqlConnection.Close();
                    }

                    if (!dict_RandomEntriesCM.ContainsKey(DbConfiguration.ConnectionString))//added on 07-02-2020
                        dict_RandomEntriesCM.TryAdd(DbConfiguration.ConnectionString, new HashSet<string>());

                    foreach (DataRow dRow in dt_CMRows.Rows)
                    {
                        try
                        {

                            var _ID = Convert.ToInt32(dRow["id"]);
                            DbConfiguration.IdCount = _ID > DbConfiguration.IdCount ? _ID : DbConfiguration.IdCount;

                            string ClientCode = dRow["Client_account_number"].ToString().Trim().ToUpper();
                            string FullDealerID = dRow["CTCL_code"].ToString().Trim().ToUpper();
                            string CTCL_ID = FullDealerID.Substring(0, (FullDealerID.Length > 11 ? 12 : FullDealerID.Length));
                            string UserId = dRow["User_id"].ToString().Trim().ToUpper();
                            string ClientInfokey = $"{ClientCode}|{CTCL_ID}|{UserId}";

                            arr_Combinations = new string[TotalCombinations] { ClientInfokey, ClientCode, CTCL_ID, UserId };

                            //                        arr_Combinations = new string[TotalCombinations] { ClientCode };

                            //changed on 10FEB2021 by Amey
                            _PositionInfo = new PositionInfo();

                            bool isValidTrade = false;
                            bool isErrorCode = true;
                            for (int i = 0; i < arr_Combinations.Length; i++)
                            {
                                try
                                {
                                    //changed location at top on 28JUL2021 by Amey. To avoid skipping if not valid trade.
                                    //added on 01DEC2020 by Amey
                                    //double dbl_TradeTime = Convert.ToDouble(dRow["Time"]);
                                    //_CMLastTradeTime = dbl_TradeTime > _CMLastTradeTime ? dbl_TradeTime : _CMLastTradeTime;

                                    //changed location at top on 28JUL2021 by Amey. To avoid skipping if not valid trade.

                                    var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSECM;
                                    if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

                                    var check = dRow["Trade_number"].ToString()+"_"+ ClientCode;
                                    if (dict_RandomEntriesCM[DbConfiguration.ConnectionString].Contains(check))
                                        continue;

                                    isErrorCode = false;

                                    //added on 19APR2021 by Amey
                                    _PositionInfo.Segment = en_Segment.NSECM;
                                    //Series = dRow["Series"].ToString().Trim().ToUpper();
                                    Symbol = dRow["Symbol"].ToString().Trim().ToUpper();
                                    var Token = dRow["Token"].ToString().Trim().ToUpper();

                                    ScripNameKey = _PositionInfo.Segment + "|" + $"{Symbol}-{Series}";
                                    var TokenScripKey = _PositionInfo.Segment + "|" + Token;

                                    //added on 19APR2021 by Amey
                                    ContractMaster _ScripInfo = null;
                                    if (!dict_TokenScripInfo.TryGetValue(TokenScripKey, out _ScripInfo))
                                    {
                                        CustomScripNameKey = _PositionInfo.Segment + "|" + $"{Symbol}|01JAN1980|0|{Series}";

                                        if (!dict_CustomScripInfo.TryGetValue(CustomScripNameKey, out _ScripInfo))
                                        {
                                            if (isDebug)
                                                _logger.Error("ReadCM Skipped : " + ScripNameKey + "^" + CustomScripNameKey, true);

                                            continue;
                                        }
                                    }

                                    _PositionInfo.TradePrice = Convert.ToDouble(dRow["Trade_price"]);

                                    //added on 07APR2021 by Amey
                                    var Qty = long.Parse(dRow["Trade_quantity"].ToString());
                                    if (dRow["Buy_Sell_flag"].ToString().Trim().ToUpper() == "2")
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

                                    //changed to - on 13APR2021 by Amey
                                    //changed computation logic on 07APR2021 by Amey
                                    _PositionInfo.IntradayValue = _PositionInfo.TradeValue; // _PositionInfo.BuyValue - _PositionInfo.SellValue;

                                    //changed on 19APR2021 by Amey
                                    _PositionInfo.Token = _ScripInfo.Token;
                                    _PositionInfo.UnderlyingToken = _ScripInfo.UnderlyingToken;
                                    _PositionInfo.UnderlyingSegment = _ScripInfo.UnderlyingSegment;

                                    DateTime _TradeTime = Convert.ToDateTime(dRow["Trade_time"]);
                                    double dbl_TradeTime = ConvertToUnixTimestamp(_TradeTime);

                                    _CMLastTradeTime = dbl_TradeTime > _CMLastTradeTime ? dbl_TradeTime : _CMLastTradeTime;   //

                                    _PositionInfo.TradeTime = _TradeTime.ToString("ddMMMyyyy HH:mm:ss").ToUpper();

                                    isValidTrade = true;
                                    dict_RandomEntriesCM[DbConfiguration.ConnectionString].Add(check);

                                    var TradeID = dRow["Trade_number"].ToString();

                                    _PositionInfo.TradeID = TradeID;

                                    break;
                                }
                                catch (Exception ee) { _logger.Error("ReadCM Loop : " + string.Join(",", dRow.ItemArray) + Environment.NewLine + ee); }
                            }

                            if (isValidTrade)
                            {
                                HashSet<string> hs_Usernames = new HashSet<string>();
                                for (int i = 0; i < arr_Combinations.Length; i++)
                                {
                                    var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSECM;
                                    if (!dict_ClientInfo.TryGetValue(UniqueID, out hs_Usernames)) continue;

                                    foreach (var Username in hs_Usernames)
                                    {
                                        var newPosInfo = CopyPropertiesFrom(new PositionInfo(), _PositionInfo);
                                        newPosInfo.Username = Username;

                                        lock (list_AllTrades)
                                            list_AllTrades.Add(newPosInfo);

                                    }
                                }
                            }
                        }
                        catch (Exception ee) { _logger.Error("ReadCM Loop : " + string.Join(",", dRow.ItemArray) + Environment.NewLine + ee); }
                    }

                   
                }

            }
            catch (Exception foEX)
            {
                _logger.Error("Read CM " + foEX.ToString());
            }
        }
       
        private void ReadCD()
        {
            try
            {

                PositionInfo _PositionInfo;

                var arr_Combinations = new string[TotalCombinations] { "", "", "", "" };

                //added on 19APR2021 by Amey
                var CustomScripNameKey = string.Empty;
                var Symbol = string.Empty;
                var ScripType = en_ScripType.EQ;
                var StrikePrice = 0.0;
                DateTime _ExpiryDate = new DateTime(1980, 1, 1, 0, 0, 0);
                
                foreach (var DbConfiguration in lst_NotisConfigurationsCD)
                {
                  
                    var dt_CDRows = new DataTable();
                    using (MySqlConnection _MySqlConnection = new MySqlConnection(DbConfiguration.ConnectionString))
                    {
                        _MySqlConnection.Open();

                        using (MySqlCommand _MySqlCommand = new MySqlCommand("sp_GetCDTradesN", _MySqlConnection))
                        {
                            _MySqlCommand.CommandType = CommandType.StoredProcedure;

                            _MySqlCommand.Parameters.Add("prm_ID", MySqlDbType.Int32);
                            _MySqlCommand.Parameters["prm_ID"].Value = DbConfiguration.IdCount;

                            using (MySqlDataAdapter _MySqlAdapter = new MySqlDataAdapter(_MySqlCommand))
                            {
                                //changed on 13JAN2021 by Amey
                                _MySqlAdapter.Fill(dt_CDRows);

                                _MySqlAdapter.Dispose();
                            }
                        }

                        _MySqlConnection.Close();
                    }

                    if (!dict_RandomEntriesCD.ContainsKey(DbConfiguration.ConnectionString))//added on 07-02-2020
                        dict_RandomEntriesCD.TryAdd(DbConfiguration.ConnectionString, new HashSet<string>());

                    foreach (DataRow dRow in dt_CDRows.Rows)
                    {
                        try
                        {

                            string ClientCode = dRow["Client_account_number"].ToString().Trim().ToUpper();
                            string FullDealerID = dRow["CTCL_code"].ToString().Trim().ToUpper();
                            string CTCL_ID = FullDealerID.Substring(0, (FullDealerID.Length > 11 ? 12 : FullDealerID.Length));
                            string UserId = dRow["User_id"].ToString().Trim().ToUpper();
                            string ClientInfokey = $"{ClientCode}|{CTCL_ID}|{UserId}";
                            int Lotsize = 1; 
                            arr_Combinations = new string[TotalCombinations] { ClientInfokey, ClientCode, CTCL_ID, UserId };

                            //arr_Combinations = new string[TotalCombinations] { ClientCode };

                            //changed on 10FEB2021 by Amey
                            _PositionInfo = new PositionInfo();

                            bool isValidTrade = false;

                            var _ID = Convert.ToInt32(dRow["id"]);
                            DbConfiguration.IdCount = _ID > DbConfiguration.IdCount ? _ID : DbConfiguration.IdCount;

                            for (int i = 0; i < arr_Combinations.Length; i++)
                            {
                                try
                                {
                                    //changed location at top on 28JUL2021 by Amey. To avoid skipping if not valid trade.
                                    //added on 01DEC2020 by Amey
                                    //double dbl_TradeTime = Convert.ToDouble(dRow["Time"]);
                                    //_CDLastTradeTime = dbl_TradeTime > _CDLastTradeTime ? dbl_TradeTime : _CDLastTradeTime;

                                    var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSECD;
                                    if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

                                    var check = dRow["Trade_number"].ToString() + "_" + ClientCode;
                                    if (dict_RandomEntriesCD[DbConfiguration.ConnectionString].Contains(check)) continue;

                                    //added on 19APR2021 by Amey
                                    _PositionInfo.Segment = en_Segment.NSECD;

                                    //added on 20APR2021 by Amey
                                    Symbol = dRow["Symbol"].ToString().Trim().ToUpper();
                                    ScripType = dRow["Option_Type"].ToString().Trim().ToUpper() == "CE" ? en_ScripType.CE :
                                        (dRow["Option_Type"].ToString().Trim().ToUpper() == "PE" ? en_ScripType.PE : en_ScripType.XX);
                                    StrikePrice = Convert.ToDouble(ScripType == en_ScripType.XX ? "0" : dRow["Strike_price"].ToString()) / 100;
                                    _ExpiryDate = Convert.ToDateTime(dRow["Expiry_date"]);

                                    var token = dRow["Scrip_name"].ToString().Trim().ToUpper();
                                    //added on 19APR2021 by Amey
                                    ContractMaster _ScripInfo = null;
                                    CustomScripNameKey = _PositionInfo.Segment.ToString() + "|" + $"{Symbol}|{_ExpiryDate.ToString("ddMMMyyyy").ToUpper()}|{(StrikePrice == 0 ? "0" : StrikePrice.ToString("#.0000"))}|{ScripType}";

                                    if (!dict_CustomScripInfo.TryGetValue(CustomScripNameKey, out _ScripInfo))
                                    {
                                        if (!dict_ScripInfo.TryGetValue(_PositionInfo.Segment + "|" + token, out _ScripInfo))
                                        {
                                            if (isDebug)
                                                _logger.Error("ReadCD Skipped : " + CustomScripNameKey, true);

                                            continue;
                                        }
                                    }

                                    _PositionInfo.TradePrice = Convert.ToDouble(dRow["Trade_price"]) / 100000;

                                    Lotsize = _ScripInfo.LotSize;
                                    //added on 07APR2021 by Amey
                                    var Qty = long.Parse(dRow["Trade_quantity"].ToString());
                                    if (dRow["Buy_Sell_flag"].ToString().Trim().ToUpper() == "2")
                                    {
                                        _PositionInfo.TradeQuantity = Qty * _ScripInfo.LotSize * -1;
                                        _PositionInfo.SellQuantity = Qty * _ScripInfo.LotSize;
                                        _PositionInfo.SellValue = Qty * _ScripInfo.LotSize * _PositionInfo.TradePrice;
                                    }
                                    else
                                    {
                                        _PositionInfo.TradeQuantity = Qty * _ScripInfo.LotSize;
                                        _PositionInfo.BuyQuantity = Qty * _ScripInfo.LotSize;
                                        _PositionInfo.BuyValue = Qty * _ScripInfo.LotSize * _PositionInfo.TradePrice;
                                    }

                                    //Added by Snehadri on 26SEp2022 for DealerWisePostion Dump

                                    //added on 10JUN2021 by Amey
                                    _PositionInfo.TradeValue = _PositionInfo.TradeQuantity * _PositionInfo.TradePrice;

                                    _PositionInfo.IntradayQuantity = _PositionInfo.TradeQuantity;

                                    //changed to - on 13APR2021 by Amey
                                    //changed computation logic on 07APR2021 by Amey
                                    _PositionInfo.IntradayValue = _PositionInfo.TradeValue; // _PositionInfo.BuyValue - _PositionInfo.SellValue;

                                    //changed on 19APR2021 by Amey
                                    _PositionInfo.Token = _ScripInfo.Token;
                                    _PositionInfo.UnderlyingToken = _ScripInfo.UnderlyingToken;
                                    _PositionInfo.UnderlyingSegment = _ScripInfo.UnderlyingSegment;

                                    isValidTrade = true;

                                    DateTime _TradeTime = Convert.ToDateTime(dRow["Trade_time"]);
                                    _PositionInfo.TradeTime = _TradeTime.ToString("ddMMMyyyy HH:mm:ss").ToUpper();

                                    double dbl_TradeTime = ConvertToUnixTimestamp(_TradeTime);
                                    _CDLastTradeTime = dbl_TradeTime > _CDLastTradeTime ? dbl_TradeTime : _CDLastTradeTime;

                                    var TradeID = dRow["Trade_number"].ToString();
                                    _PositionInfo.TradeID = TradeID;

                                    dict_RandomEntriesCD[DbConfiguration.ConnectionString].Add(check);

                                    break;
                                }
                                catch (Exception ee) { _logger.Error("ReadCD Loop -inner : " + string.Join(",", dRow.ItemArray) + Environment.NewLine + ee); }
                            }

                            if (isValidTrade)
                            {
                                for (int i = 0; i < arr_Combinations.Length; i++)
                                {
                                    HashSet<string> hs_Usernames = new HashSet<string>();
                                    var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSECD;
                                    if (!dict_ClientInfo.TryGetValue(UniqueID, out hs_Usernames)) continue;

                                    foreach (var Username in hs_Usernames)
                                    {
                                        #region Span using dll
                                        try
                                        {
                                            Span _spn = new Span()
                                            {
                                                pMemberId = 1,
                                                pClientId = Username + "_" + Symbol,
                                                pExchange = "NSE",
                                                pSegment = "CURR",
                                                pScripName = Symbol,
                                                pExpiry = _ExpiryDate.ToString("yyyyMMdd"),
                                                pFactor = "E",
                                                pQty = (_PositionInfo.TradeQuantity / Lotsize).ToString(),

                                                //added on 11JUN2021 by Amey
                                                pTradeTime = _CDLastTradeTime
                                            };

                                            if (ScripType == en_ScripType.XX)
                                            {
                                                _spn.pStrikePrice = "";
                                                _spn.pCallPut = "";
                                            }
                                            else
                                            {
                                                _spn.pStrikePrice = StrikePrice.ToString("#.0000");
                                                _spn.pCallPut = ScripType.ToString().Substring(0, 1);
                                            }

                                            eve_CDSSpanRead(_spn);
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
                        catch (Exception ee) { _logger.Error("ReadCD Loop : " + string.Join(",", dRow.ItemArray) + Environment.NewLine + ee); }
                    }

                }

            }
            catch (Exception foEX)
            {
                _logger.Error("Read CD " + foEX.ToString());
            }
        }

        #endregion



        private void ReadBSECM()
        {
            try
            {
                DateTime dte_StartReadingTrades = DateTime.Now; //added for testing and Dubugging

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
                    string[] arr_FileEntry = Directory.GetFiles(_BSECMFilePath, _TradeFileBSECM + ".csv");
                    List<PositionInfo> lst_LocalTrade = new List<PositionInfo>();
                    if (arr_FileEntry.Length > 0)
                    {

                        try
                        {
                            string FullFilePath = arr_FileEntry[0];

                            if (!dict_RandomEntriesBSECM.ContainsKey(FullFilePath))//added on 07-02-2020
                                dict_RandomEntriesBSECM.TryAdd(FullFilePath, new HashSet<string>());

                            //Added by Akshay on 01-09-2021 for NewTrade reading Logic
                            if (!dict_CharToSkip.ContainsKey(FullFilePath))//added on 07-02-2020
                                dict_CharToSkip.TryAdd(FullFilePath, 0);

                            var CharToSkip = dict_CharToSkip[FullFilePath];
                            var CurrentCharToSkip = 0;
                            //var MaxLineNumber = dict_CharToSkip[FullFilePath];
                            var LineCounter = 0;

                            using (FileStream stream = File.Open(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                stream.Seek(CharToSkip, SeekOrigin.Begin);
                                using (StreamReader sr = new StreamReader(stream))
                                {
                                    string strData = String.Empty;
                                    while ((strData = sr.ReadLine()) != null)
                                    {
                                        //Added by Akshay on 01-09-2021 for New trade reading Logic
                                        //CurrentCharToSkip += strData.Length + 1;
                                        //LineCounter += 1;

                                        //added on 25JAN2021 by Amey
                                        isTradeFileBSECMAvailable = true;

                                        //added on 05JAN2021 by Amey
                                        string[] arr_Fields = strData.ToUpper().Split('|').Select(v => v.Trim()).ToArray();

                                        if (arr_Fields.Length > 31 && arr_Fields[11] != "")
                                        {
                                            CurrentCharToSkip += strData.Length + 2;
                                            LineCounter += 1;

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
                                                    var UniqueID = arr_Combinations[i] + "|" + en_Segment.BSECM;
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
                                                    // Commented by Faraz To Read BSECM without converting to nsecm 01NOV2023 
                                                    //if (dict_ScripInfo.ContainsKey(en_Segment.NSECM + "|" + _ScripInfo.ScripName))
                                                    //    _ScripInfo = dict_ScripInfo[en_Segment.NSECM + "|" + _ScripInfo.ScripName];

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
                                                    _BSECMLastTradeTime = dbl_TradeTime > _BSECMLastTradeTime ? dbl_TradeTime : _BSECMLastTradeTime;

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
                                                //BSETradeCount += 1;
                                                //if (isDebug)
                                                //    _logger.Error($"BSE Trade Count: {BSETradeCount}", true);

                                                HashSet<string> hs_Usernames = new HashSet<string>();

                                                for (int i = 0; i < arr_Combinations.Length; i++)
                                                {
                                                    var UniqueID = arr_Combinations[i] + "|" + en_Segment.BSECM;
                                                    if (!dict_ClientInfo.TryGetValue(UniqueID, out hs_Usernames)) continue;

                                                    foreach (var Username in hs_Usernames)
                                                    {
                                                        var newPosInfo = CopyPropertiesFrom(new PositionInfo(), _PositionInfo);
                                                        newPosInfo.Username = Username;

                                                        lst_LocalTrade.Add(newPosInfo);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var arr_Check = strData.Split(',');
                                            if (arr_Check.Length == 1)
                                            {
                                                var chkStrData = strData.Trim();
                                                CurrentCharToSkip += chkStrData.Length == 0 ? 1 : chkStrData.Length + 2;
                                            }

                                            _logger.Error("Inconsistent line found |" + strData + " Path:" + FullFilePath);
                                            break;
                                        }

                                        //Added by Akshay on 01-09-2021 for New Trade reading Logic
                                        if (LineCounter == Trade_Batch_Count)
                                        {
                                            dict_CharToSkip[FullFilePath] += CurrentCharToSkip;

                                            lock (list_AllTrades)
                                                list_AllTrades.AddRange(lst_LocalTrade);

                                            if (isDebug && list_AllTrades.Any())
                                                _logger.Error("ReadBCECM Loop " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs" + ", Trades:" + lst_LocalTrade.Count.ToString(), isDebug);

                                            return;
                                        }
                                    }
                                    dict_CharToSkip[FullFilePath] += CurrentCharToSkip;   //Added by Akshay on 01-09-2021 for New Trade reading Logic
                                    lock (list_AllTrades)
                                        list_AllTrades.AddRange(lst_LocalTrade);
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            _logger.Error(ex.ToString());
                        }
                       
                    }
                    else
                    {
                        //added on 25JAN2021 by Amey
                        isTradeFileBSECMAvailable = false;
                    }
                }

               

                if (isDebug && list_AllTrades.Any())
                    _logger.Error("ReadBCECM Loop " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs" + ", Trades:" + list_AllTrades.Count.ToString(), isDebug);

            }
            catch (Exception foEX)
            {
                _logger.Error("Read BSECM " + foEX.ToString());
            }
        }

        //added by Omkar
        private void ReadBSEFO()
        {
            try
            {
                DateTime dte_StartReadingTrades = DateTime.Now; //added for testing and Dubugging

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
                foreach (var _BSEFOFilePath in ds_Config.Tables["INTRADAY"].Rows[0]["BSE-FO"].ToString().Split(','))
                {
                    string[] arr_FileEntry = Directory.GetFiles(_BSEFOFilePath, _TradeFileBSEFO + ".csv");
                    List<PositionInfo> lst_LocalTrade = new List<PositionInfo>();
                    if (arr_FileEntry.Length > 0)
                    {
                        try
                        {
                            string FullFilePath = arr_FileEntry[0];

                            if (!dict_RandomEntriesBSEFO.ContainsKey(FullFilePath))//added on 07-02-2020
                                dict_RandomEntriesBSEFO.TryAdd(FullFilePath, new HashSet<string>());

                            //Added by Akshay on 01-09-2021 for NewTrade reading Logic
                            //if (!dict_CharToSkip.ContainsKey(FullFilePath))//added on 07-02-2020
                            //    dict_CharToSkip.TryAdd(FullFilePath, 0);


                            //var CharToSkip = dict_CharToSkip[FullFilePath];
                            //var CurrentCharToSkip = 0;
                            //var LineCounter = 0;

                            using (FileStream fs = File.Open(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                //fs.Seek(CharToSkip, SeekOrigin.Begin);
                                using (BufferedStream bs = new BufferedStream(fs))
                                {
                                    using (StreamReader sr = new StreamReader(bs))
                                    {
                                        string strData = string.Empty;

                                        while ((strData = sr.ReadLine()) != null)
                                        {
                                            //Added by Akshay on 01-09-2021 for New trade reading Logic
                                            //CurrentCharToSkip += strData.Length + 1;
                                            //LineCounter += 1;

                                            //added on 25JAN2021 by Amey
                                            isTradeFileBSEFOAvailable = true;

                                            //added on 05JAN2021 by Amey
                                            string[] arr_Fields = strData.ToUpper().Split(',').Select(v => v.Trim()).ToArray();
                                            // > 49 if file "ITR-CM"
                                            if (arr_Fields.Length > 46 && arr_Fields[0] != "") //added on 2-1-18
                                            {
                                                //CurrentCharToSkip += strData.Length + 2;
                                                //LineCounter += 1;

                                                string TradeID = arr_Fields[0];

                                                string FullDealerID = (arr_Fields[17] == "") ? arr_Fields[20] : arr_Fields[17];

                                                string CTCL_ID = FullDealerID.Substring(0, (FullDealerID.Length > 12 ? 13 : FullDealerID.Length));

                                                string ClientCode = (arr_Fields[35] == "") ? arr_Fields[36] : arr_Fields[35];
                                                
                                                string UserId = "0";
                                                try { UserId = (arr_Fields[31] == "") ? arr_Fields[32] : arr_Fields[31]; } catch (Exception) { }
                                                string ClientInfokey = $"{ClientCode}|{CTCL_ID}|{UserId}";
                                                string OrderNo = (arr_Fields[33] == "") ? arr_Fields[34] : arr_Fields[33];

                                                arr_Combinations = new string[TotalCombinations] { ClientInfokey, ClientCode, CTCL_ID, UserId};

                                                //changed on 10FEB2021 by Amey
                                                _PositionInfo = new PositionInfo();

                                                bool isValidTrade = false;

                                                for (int i = 0; i < arr_Combinations.Length; i++)
                                                {
                                                    try
                                                    {
                                                        var UniqueID = arr_Combinations[i] + "|" + en_Segment.BSEFO;
                                                        if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

                                                        string _check = TradeID + "_" + CTCL_ID + "_" + OrderNo;
                                                        if (dict_RandomEntriesBSEFO[FullFilePath].Contains(_check)) continue;//this dict need to change.

                                                        //added on 19APR2021 by Amey
                                                        _PositionInfo.Segment = en_Segment.BSEFO;

                                                        //added on 20APR2021 by Amey
                                                        Underlying = arr_Fields[7];

                                                        if (dict_BSEMapping.ContainsKey(Underlying))
                                                        {
                                                            Underlying = dict_BSEMapping[Underlying];
                                                        }

                                                        ScripType = arr_Fields[10] == "" ? en_ScripType.XX : (arr_Fields[10] == "CE" ? en_ScripType.CE : en_ScripType.PE);
                                                        StrikePrice = Convert.ToDouble(ScripType == en_ScripType.XX ? "0" : arr_Fields[9]);//not sure
                                                        _ExpiryDate = Convert.ToDateTime(arr_Fields[8]);//not sure

                                                        ScripNameKey = _PositionInfo.Segment + "|" + arr_Fields[11].ToUpper();

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

                                                        _PositionInfo.TradePrice = Convert.ToDouble(arr_Fields[14]);

                                                        //added on 07APR2021 by Amey
                                                        var Qty = long.Parse(arr_Fields[15]);//not sure about the fields.

                                                        #region original logic commented by Omkar
                                                        //if (arr_Fields[13] == "20")
                                                        //{
                                                        //    _PositionInfo.TradeQuantity = Qty * -1;
                                                        //    _PositionInfo.SellQuantity = Qty;
                                                        //    _PositionInfo.SellValue = Qty * _PositionInfo.TradePrice;
                                                        //}
                                                        //else
                                                        //{
                                                        //    _PositionInfo.TradeQuantity = Qty;
                                                        //    _PositionInfo.BuyQuantity = Qty;
                                                        //    _PositionInfo.BuyValue = Qty * _PositionInfo.TradePrice;
                                                        //}
                                                        #endregion

                                                        #region added by Omkar
                                                        if (string.IsNullOrEmpty(arr_Fields[12]))
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
                                                        #endregion

                                                        //added on 10JUN2021 by Amey
                                                        _PositionInfo.TradeValue = _PositionInfo.TradeQuantity * _PositionInfo.TradePrice;

                                                        _PositionInfo.IntradayQuantity = _PositionInfo.TradeQuantity;

                                                        //changed to TradeValue on 10JUN2021 by Amey
                                                        //changed to - on 13APR2021 by Amey
                                                        //changed computation logic on 07APR2021 by Amey
                                                        _PositionInfo.IntradayValue = _PositionInfo.TradeValue; //_PositionInfo.BuyValue - _PositionInfo.SellValue;

                                                        //added on 01DEC2020 by Amey
                                                        double dbl_TradeTime = ConvertToUnixTimestamp(DateTime.Parse(arr_Fields[1]));
                                                        _BSEFOLastTradeTime = dbl_TradeTime > _BSEFOLastTradeTime ? dbl_TradeTime : _BSEFOLastTradeTime;

                                                        //changed on 19APR2021 by Amey
                                                        _PositionInfo.Token = _ScripInfo.Token;
                                                        _PositionInfo.UnderlyingToken = _ScripInfo.UnderlyingToken;
                                                        _PositionInfo.UnderlyingSegment = _ScripInfo.UnderlyingSegment;

                                                        isValidTrade = true;

                                                        //changed position on 28FEB2021 by Amey
                                                        dict_RandomEntriesBSEFO[FullFilePath].Add(_check);

                                                        break;
                                                    }
                                                    catch (Exception ee) { _logger.Error("ReadFO Loop : " + strData + Environment.NewLine + ee); }
                                                }

                                                if (isValidTrade)
                                                {
                                                    FOTradeCount += 1;
                                                    //if (isDebug)
                                                    //    _logger.Error($"FO Trade Count: {FOTradeCount}", true);

                                                    HashSet<string> hs_usernames = new HashSet<string>();

                                                    for (int i = 0; i < arr_Combinations.Length; i++)
                                                    {
                                                        var UniqueID = arr_Combinations[i] + "|" + en_Segment.BSEFO;
                                                        if (!dict_ClientInfo.TryGetValue(UniqueID, out hs_usernames)) continue;

                                                        foreach (var Username in hs_usernames)
                                                        {
                                                            //need to ask wheather its needed or not.
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
                                                                    
                                                                    pTradeTime = _BSEFOLastTradeTime
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

                                                                eve_SpanRead(_spn);//you have to track this.
                                                            }
                                                            catch (Exception spanEx)//added try catch for individual span calculation on 06-01-2020
                                                            {
                                                                _logger.Error("Span calculation individual scrip " + spanEx);
                                                            }

                                                            #endregion

                                                            var newPosInfo = CopyPropertiesFrom(new PositionInfo(), _PositionInfo);
                                                            newPosInfo.Username = Username;

                                                            //changed on 10FEB2021 by Amey
                                                            lst_LocalTrade.Add(newPosInfo);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                ////strData.Trim();
                                                ///// 
                                                ///// \n\r
                                                ///// ,12|345
                                                ///// 12|345,12345
                                                ///// <space>
                                                ///// enter
                                                ///// 

                                                //var arr_Check = strData.Split(',');
                                                //if (arr_Check.Length == 1)
                                                //{
                                                //    var chkStrData = strData.Trim();
                                                //    CurrentCharToSkip += chkStrData.Length == 0 ? 1 : chkStrData.Length + 2;
                                                //}

                                                //_logger.Error("Inconsistent line found |" + strData + " Path:" + FullFilePath);
                                                //break;
                                            }

                                            //Added by Akshay on 01-09-2021 for New Trade reading Logic
                                            //if (LineCounter == Trade_Batch_Count)
                                            //{
                                            //    dict_CharToSkip[FullFilePath] += CurrentCharToSkip;

                                            //    lock (list_AllTrades)
                                            //        list_AllTrades.AddRange(lst_LocalTrade);

                                            //    //added for testing and Debugging
                                            //    if (isDebug && list_AllTrades.Any())
                                            //        _logger.Error("ReadBSEFO Loop " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs" + ", Trades:" + lst_LocalTrade.Count.ToString(), isDebug);

                                            //    return;
                                            //}
                                        }
                                        //dict_CharToSkip[FullFilePath] += CurrentCharToSkip;    //Added by Akshay on 01-09-2021 for New Trade reading Logic

                                        lock (list_AllTrades)
                                            list_AllTrades.AddRange(lst_LocalTrade);

                                    }
                                }
                            }

                        }
                        catch (Exception FileEx)
                        {
                            _logger.Error("ReadBSEFO inner fileentriesFO " + arr_FileEntry.Length + FileEx.ToString());
                        }
                    }
                    else
                    {
                        //added on 25JAN2021 by Amey
                        isTradeFileBSEFOAvailable = false;
                    }
                }

                //added for testing and Debugging
                if (isDebug && list_AllTrades.Any())
                    _logger.Error("ReadBSEFO Loop " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs" + ", Trades:" + list_AllTrades.Count.ToString(), isDebug);

                //objWriteLog.WriteLog("Time Taken To Read : " + (DateTime.Now - dt_Start).TotalSeconds + " Seconds " + DateTime.Now);
            }
            catch (Exception foEX)
            {
                _logger.Error("ReadBSEFO " + foEX.ToString());
            }
        }

        private void ReadCP()
        {
            try
            {
                DateTime dte_StartReadingTrades = DateTime.Now; //added for testing and Dubugging

                PositionInfo _PositionInfo;

                var arr_Combinations = new string[TotalCombinationsCP] { "" };

                //added on 19APR2021 by Amey
                var ScripNameKey = string.Empty;
                var CustomScripNameKey = string.Empty;
                var Underlying = string.Empty;
                var ScripType = en_ScripType.EQ;
                var StrikePrice = 0.0;
                DateTime _ExpiryDate = new DateTime(1980, 1, 1, 0, 0, 0);
               
                //changed on 15JAN2021 by Amey
                foreach (var _NoticeCPFilePath in ds_Config.Tables["INTRADAY"].Rows[0]["TRADE-CP"].ToString().Split(','))
                {
                    string[] arr_FileEntry = Directory.GetFiles(_NoticeCPFilePath, _TradeFileCP + ".csv");
                    List<PositionInfo> lst_LocalTrade = new List<PositionInfo>();
                    if (arr_FileEntry.Length > 0)
                    {
                        try
                        {
                            string FullFilePath = arr_FileEntry[0];

                            if (!dict_RandomEntriesCP.ContainsKey(FullFilePath))//added on 07-02-2020
                                dict_RandomEntriesCP.TryAdd(FullFilePath, new HashSet<string>());

                            //Added by Akshay on 01-09-2021 for NewTrade reading Logic
                            if (!dict_CharToSkip.ContainsKey(FullFilePath))//added on 07-02-2020
                                dict_CharToSkip.TryAdd(FullFilePath, 0);

                            var CharToSkip = dict_CharToSkip[FullFilePath];
                            var CurrentCharToSkip = 0;
                            var LineCounter = 0;

                            using (FileStream fs = File.Open(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {

                                fs.Seek(CharToSkip, SeekOrigin.Begin);

                                using (BufferedStream bs = new BufferedStream(fs))
                                {
                                    using (StreamReader sr = new StreamReader(bs))
                                    {
                                        string strData = string.Empty;
                                        //var PrevLineCounter = 0;    //Added by Akshay on 01-09-2021 for new trade reading logic

                                        while ((strData = sr.ReadLine()) != null)
                                        {
                                            //Added by Akshay on 01-09-2021 for New trade reading Logic
                                            //CurrentCharToSkip += strData.Length + 1;
                                            //LineCounter += 1;

                                            //added on 25JAN2021 by Amey
                                            isTradeFileCPAvailable = true;

                                            //added on 05JAN2021 by Amey
                                            string[] arr_Fields = strData.ToUpper().Split(',').Select(v => v.Trim()).ToArray();

                                            if (arr_Fields.Length > 49 && arr_Fields[0] != "") //added on 2-1-18
                                            {
                                                CurrentCharToSkip += strData.Length + 2;
                                                LineCounter += 1;

                                                string TradeID = arr_Fields[0];

                                                //string FullDealerID = arr_Fields[26];
                                                //string CTCL_ID = FullDealerID.Substring(0, (FullDealerID.Length > 11 ? 12 : FullDealerID.Length));
                                                //string ClientCode = arr_Fields[17];
                                                //string UserId = arr_Fields[11];
                                                //string ClientInfokey = $"{ClientCode}|{CTCL_ID}|{UserId}";

                                                string BrokerID = arr_Fields[12] == "" ? arr_Fields[13] : arr_Fields[12];

                                                if (hs_BrokerID.Contains(BrokerID))
                                                    continue;

                                                string ClientCode = arr_Fields[21] == "" ? arr_Fields[23] : arr_Fields[21];

                                                arr_Combinations = new string[TotalCombinationsCP] { ClientCode };

                                                //changed on 10FEB2021 by Amey
                                                _PositionInfo = new PositionInfo();

                                                bool isValidTrade = false;

                                                for (int i = 0; i < arr_Combinations.Length; i++)
                                                {
                                                    try
                                                    {
                                                        var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSEFO;
                                                        if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

                                                        string _check = TradeID + "_" + ClientCode;
                                                        if (dict_RandomEntriesCP[FullFilePath].Contains(_check)) continue;

                                                        //added on 19APR2021 by Amey
                                                        _PositionInfo.Segment = en_Segment.NSEFO;

                                                        //added on 20APR2021 by Amey
                                                        Underlying = arr_Fields[7];

                                                        if (dict_BSEMapping.ContainsKey(Underlying))
                                                        {
                                                            Underlying = dict_BSEMapping[Underlying];
                                                        }

                                                        ScripType = arr_Fields[10] == "" ? en_ScripType.XX : (arr_Fields[10] == "CE" ? en_ScripType.CE : en_ScripType.PE);
                                                        StrikePrice = Convert.ToDouble(ScripType == en_ScripType.XX ? "0" : arr_Fields[9]);
                                                        _ExpiryDate = Convert.ToDateTime(arr_Fields[8]);

                                                        ScripNameKey = _PositionInfo.Segment + "|" + arr_Fields[6].ToUpper();

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

                                                        _PositionInfo.TradePrice = Convert.ToDouble(arr_Fields[14]);

                                                        //added on 07APR2021 by Amey
                                                        var Qty = long.Parse(arr_Fields[15]);

                                                        var BuySell = arr_Fields[12] == "" ? "2" : "1";

                                                        if (BuySell == "2")
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
                                                        //double dbl_TradeTime = ConvertToUnixTimestamp(DateTime.Parse(arr_Fields[1]));
                                                        //_FOLastTradeTime = dbl_TradeTime > _FOLastTradeTime ? dbl_TradeTime : _FOLastTradeTime;

                                                        //changed on 19APR2021 by Amey
                                                        _PositionInfo.Token = _ScripInfo.Token;
                                                        _PositionInfo.UnderlyingToken = _ScripInfo.UnderlyingToken;
                                                        _PositionInfo.UnderlyingSegment = _ScripInfo.UnderlyingSegment;

                                                        isValidTrade = true;

                                                        //changed position on 28FEB2021 by Amey
                                                        dict_RandomEntriesCP[FullFilePath].Add(_check);

                                                        break;
                                                    }
                                                    catch (Exception ee) { _logger.Error("ReadFO Loop : " + strData + Environment.NewLine + ee); }
                                                }

                                                if (isValidTrade)
                                                {
                                                    CPTradeCount += 1;
                                                    //if (isDebug)
                                                    //    _logger.Error($"CP Trade Count: {CPTradeCount}", true);

                                                    HashSet<string> hs_usernames = new HashSet<string>();

                                                    for (int i = 0; i < arr_Combinations.Length; i++)
                                                    {
                                                        var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSEFO;
                                                        if (!dict_ClientInfo.TryGetValue(UniqueID, out hs_usernames)) continue;

                                                        foreach (var Username in hs_usernames)
                                                        {

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

                                                                eve_SpanRead(_spn);
                                                            }
                                                            catch (Exception spanEx)//added try catch for individual span calculation on 06-01-2020
                                                            {
                                                                _logger.Error("Span calculation individual scrip " + spanEx);
                                                            }

                                                            #endregion

                                                            var newPosInfo = CopyPropertiesFrom(new PositionInfo(), _PositionInfo);
                                                            newPosInfo.Username = Username;

                                                            lst_LocalTrade.Add(newPosInfo);
                                                            //changed on 10FEB2021 by Amey
                                                            //lock (list_AllTrades)
                                                            //    list_AllTrades.Add(newPosInfo);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var arr_Check = strData.Split(',');
                                                if (arr_Check.Length == 1)
                                                {
                                                    var chkStrData = strData.Trim();
                                                    CurrentCharToSkip += chkStrData.Length == 0 ? 1 : chkStrData.Length + 2;
                                                }

                                                _logger.Error("Inconsistent line found |" + strData + " Path:" + FullFilePath);
                                                break;
                                            }

                                            //Added by Akshay on 01-09-2021 for New Trade reading Logic
                                            if (LineCounter == Trade_Batch_Count)
                                            {
                                                dict_CharToSkip[FullFilePath] += CurrentCharToSkip;
                                                lock (list_AllTrades)
                                                    list_AllTrades.AddRange(lst_LocalTrade);

                                                //added for testing and Debugging
                                                if (isDebug && list_AllTrades.Any())
                                                    _logger.Error("ReadCP Loop " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs" + ", Trades:" + lst_LocalTrade.Count.ToString(), isDebug);

                                                return;
                                            }
                                        }
                                        dict_CharToSkip[FullFilePath] += CurrentCharToSkip;   //Added by Akshay on 01-09-2021 for New Trade reading Logic
                                        lock (list_AllTrades)
                                            list_AllTrades.AddRange(lst_LocalTrade);

                                    }
                                }
                            }
                        }
                        catch (Exception FileEx)
                        {
                            _logger.Error("Read CP inner fileentriesFO " + arr_FileEntry.Length + FileEx.ToString());
                        }
                    }
                    else
                    {
                        //added on 25JAN2021 by Amey
                        isTradeFileCPAvailable = false;
                    }
                }

              
                //added for testing and Debugging
                if (isDebug && list_AllTrades.Any())
                    _logger.Error("ReadCP Loop " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs" + ", Trades:" + list_AllTrades.Count.ToString(), isDebug);

                //objWriteLog.WriteLog("Time Taken To Read : " + (DateTime.Now - dt_Start).TotalSeconds + " Seconds " + DateTime.Now);
            }
            catch (Exception foEX)
            {
                _logger.Error("Read CP " + foEX.ToString());
            }
        }

        #region no-use

        //private void ReadFO()
        //{
        //    try
        //    {
        //        DateTime dte_StartReadingTrades = DateTime.Now; //added for testing and Dubugging

        //        PositionInfo _PositionInfo;

        //        var arr_Combinations = new string[TotalCombinations] { "", "", "", "" };

        //        //added on 19APR2021 by Amey
        //        var ScripNameKey = string.Empty;
        //        var CustomScripNameKey = string.Empty;
        //        var Underlying = string.Empty;
        //        var ScripType = en_ScripType.EQ;
        //        var StrikePrice = 0.0;
        //        DateTime _ExpiryDate = new DateTime(1980, 1, 1, 0, 0, 0);


        //        //changed on 15JAN2021 by Amey
        //        foreach (var _NoticeFOFilePath in ds_Config.Tables["INTRADAY"].Rows[0]["NOTICE-FO"].ToString().Split(','))
        //        {
        //            string[] arr_FileEntry = Directory.GetFiles(_NoticeFOFilePath, _TradeFileFO + ".txt");
        //            List<PositionInfo> lst_LocalTrade = new List<PositionInfo>();
        //            if (arr_FileEntry.Length > 0)
        //            {
        //                try
        //                {
        //                    string FullFilePath = arr_FileEntry[0];

        //                    if (!dict_RandomEntriesFO.ContainsKey(FullFilePath))//added on 07-02-2020
        //                        dict_RandomEntriesFO.TryAdd(FullFilePath, new HashSet<string>());

        //                    //Added by Akshay on 01-09-2021 for NewTrade reading Logic
        //                    if (!dict_CharToSkip.ContainsKey(FullFilePath))//added on 07-02-2020
        //                        dict_CharToSkip.TryAdd(FullFilePath, 0);


        //                    var CharToSkip = dict_CharToSkip[FullFilePath];
        //                    var CurrentCharToSkip = 0;
        //                    var LineCounter = 0;

        //                    using (FileStream fs = File.Open(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //                    {
        //                        fs.Seek(CharToSkip, SeekOrigin.Begin);
        //                        using (BufferedStream bs = new BufferedStream(fs))
        //                        {
        //                            using (StreamReader sr = new StreamReader(bs))
        //                            {
        //                                string strData = string.Empty;

        //                                while ((strData = sr.ReadLine()) != null)
        //                                {
        //                                    //Added by Akshay on 01-09-2021 for New trade reading Logic
        //                                    //CurrentCharToSkip += strData.Length + 1;
        //                                    //LineCounter += 1;

        //                                    //added on 25JAN2021 by Amey
        //                                    isTradeFileFOAvailable = true;

        //                                    //added on 05JAN2021 by Amey
        //                                    string[] arr_Fields = strData.ToUpper().Split(',').Select(v => v.Trim()).ToArray();

        //                                    if (arr_Fields.Length > 27 && arr_Fields[0] != "") //added on 2-1-18
        //                                    {
        //                                        CurrentCharToSkip += strData.Length + 2;
        //                                        LineCounter += 1;

        //                                        string TradeID = arr_Fields[0];

        //                                        string FullDealerID = arr_Fields[26];
        //                                        string CTCL_ID = FullDealerID.Substring(0, (FullDealerID.Length > 11 ? 12 : FullDealerID.Length));
        //                                        string ClientCode = arr_Fields[17];
        //                                        string UserId = arr_Fields[11];
        //                                        string ClientInfokey = $"{ClientCode}|{CTCL_ID}|{UserId}";

        //                                        arr_Combinations = new string[TotalCombinations] { ClientInfokey, ClientCode, CTCL_ID, UserId };

        //                                        //changed on 10FEB2021 by Amey
        //                                        _PositionInfo = new PositionInfo();

        //                                        bool isValidTrade = false;

        //                                        for (int i = 0; i < arr_Combinations.Length; i++)
        //                                        {
        //                                            try
        //                                            {
        //                                                var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSEFO;
        //                                                if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

        //                                                string _check = TradeID + "_" + ClientCode;
        //                                                if (dict_RandomEntriesFO[FullFilePath].Contains(_check)) continue;

        //                                                //added on 19APR2021 by Amey
        //                                                _PositionInfo.Segment = en_Segment.NSEFO;

        //                                                //added on 20APR2021 by Amey
        //                                                Underlying = arr_Fields[3];
        //                                                ScripType = arr_Fields[6] == "" ? en_ScripType.XX : (arr_Fields[6] == "CE" ? en_ScripType.CE : en_ScripType.PE);
        //                                                StrikePrice = Convert.ToDouble(ScripType == en_ScripType.XX ? "0" : arr_Fields[5]);
        //                                                _ExpiryDate = Convert.ToDateTime(arr_Fields[4]);

        //                                                ScripNameKey = _PositionInfo.Segment + "|" + arr_Fields[7].ToUpper();

        //                                                //added on 19APR2021 by Amey
        //                                                ContractMaster _ScripInfo = null;
        //                                                if (!dict_ScripInfo.TryGetValue(ScripNameKey, out _ScripInfo))
        //                                                {
        //                                                    CustomScripNameKey = _PositionInfo.Segment + "|" + $"{Underlying}|{_ExpiryDate.ToString("ddMMMyyyy").ToUpper()}|{(StrikePrice == 0 ? "0" : StrikePrice.ToString("#.00"))}|{ScripType}";

        //                                                    if (!dict_CustomScripInfo.TryGetValue(CustomScripNameKey, out _ScripInfo))
        //                                                    {
        //                                                        if (isDebug)
        //                                                            _logger.Error("ReadFO Skipped : " + ScripNameKey + "^" + CustomScripNameKey, true);
        //                                                        continue;
        //                                                    }
        //                                                }

        //                                                _PositionInfo.TradePrice = Convert.ToDouble(arr_Fields[15]);

        //                                                //added on 07APR2021 by Amey
        //                                                var Qty = long.Parse(arr_Fields[14]);
        //                                                if (arr_Fields[13] == "2")
        //                                                {
        //                                                    _PositionInfo.TradeQuantity = Qty * -1;
        //                                                    _PositionInfo.SellQuantity = Qty;
        //                                                    _PositionInfo.SellValue = Qty * _PositionInfo.TradePrice;
        //                                                }
        //                                                else
        //                                                {
        //                                                    _PositionInfo.TradeQuantity = Qty;
        //                                                    _PositionInfo.BuyQuantity = Qty;
        //                                                    _PositionInfo.BuyValue = Qty * _PositionInfo.TradePrice;
        //                                                }

        //                                                //added on 10JUN2021 by Amey
        //                                                _PositionInfo.TradeValue = _PositionInfo.TradeQuantity * _PositionInfo.TradePrice;

        //                                                _PositionInfo.IntradayQuantity = _PositionInfo.TradeQuantity;

        //                                                //changed to TradeValue on 10JUN2021 by Amey
        //                                                //changed to - on 13APR2021 by Amey
        //                                                //changed computation logic on 07APR2021 by Amey
        //                                                _PositionInfo.IntradayValue = _PositionInfo.TradeValue; //_PositionInfo.BuyValue - _PositionInfo.SellValue;

        //                                                //added on 01DEC2020 by Amey
        //                                                double dbl_TradeTime = ConvertToUnixTimestamp(DateTime.Parse(arr_Fields[25]));
        //                                                _FOLastTradeTime = dbl_TradeTime > _FOLastTradeTime ? dbl_TradeTime : _FOLastTradeTime;

        //                                                //changed on 19APR2021 by Amey
        //                                                _PositionInfo.Token = _ScripInfo.Token;
        //                                                _PositionInfo.UnderlyingToken = _ScripInfo.UnderlyingToken;
        //                                                _PositionInfo.UnderlyingSegment = _ScripInfo.UnderlyingSegment;

        //                                                isValidTrade = true;

        //                                                //changed position on 28FEB2021 by Amey
        //                                                dict_RandomEntriesFO[FullFilePath].Add(_check);

        //                                                break;
        //                                            }
        //                                            catch (Exception ee) { _logger.Error("ReadFO Loop : " + strData + Environment.NewLine + ee); }
        //                                        }

        //                                        if (isValidTrade)
        //                                        {
        //                                            FOTradeCount += 1;
        //                                            //if (isDebug)
        //                                            //    _logger.Error($"FO Trade Count: {FOTradeCount}", true);

        //                                            HashSet<string> hs_usernames = new HashSet<string>();

        //                                            for (int i = 0; i < arr_Combinations.Length; i++)
        //                                            {
        //                                                var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSEFO;
        //                                                if (!dict_ClientInfo.TryGetValue(UniqueID, out hs_usernames)) continue;

        //                                                foreach (var Username in hs_usernames)
        //                                                {

        //                                                    #region Span using dll
        //                                                    try
        //                                                    {
        //                                                        Span _spn = new Span()
        //                                                        {
        //                                                            pMemberId = 1,
        //                                                            pClientId = Username + "_" + Underlying,
        //                                                            pExchange = "NSE",
        //                                                            pSegment = "EQFO",
        //                                                            pScripName = Underlying,
        //                                                            pExpiry = _ExpiryDate.ToString("yyyyMMdd"),
        //                                                            pFactor = "E",
        //                                                            pQty = _PositionInfo.TradeQuantity.ToString(),

        //                                                            //added on 11JUN2021 by Amey
        //                                                            pTradeTime = _FOLastTradeTime
        //                                                        };

        //                                                        if (ScripType == en_ScripType.XX)
        //                                                        {
        //                                                            _spn.pStrikePrice = "";
        //                                                            _spn.pCallPut = "";
        //                                                        }
        //                                                        else
        //                                                        {
        //                                                            _spn.pStrikePrice = StrikePrice.ToString("#.00");
        //                                                            _spn.pCallPut = ScripType.ToString().Substring(0, 1);
        //                                                        }

        //                                                        eve_SpanRead(_spn);
        //                                                    }
        //                                                    catch (Exception spanEx)//added try catch for individual span calculation on 06-01-2020
        //                                                    {
        //                                                        _logger.Error("Span calculation individual scrip " + spanEx);
        //                                                    }

        //                                                    #endregion

        //                                                    var newPosInfo = CopyPropertiesFrom(new PositionInfo(), _PositionInfo);
        //                                                    newPosInfo.Username = Username;

        //                                                    //changed on 10FEB2021 by Amey
        //                                                    lst_LocalTrade.Add(newPosInfo);
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        //strData.Trim();
        //                                        /// 
        //                                        /// \n\r
        //                                        /// ,12|345
        //                                        /// 12|345,12345
        //                                        /// <space>
        //                                        /// enter
        //                                        /// 

        //                                        var arr_Check = strData.Split(',');
        //                                        if (arr_Check.Length == 1)
        //                                        {
        //                                            var chkStrData = strData.Trim();
        //                                            CurrentCharToSkip += chkStrData.Length == 0 ? 1 : chkStrData.Length + 2;
        //                                        }

        //                                        _logger.Error("Inconsistent line found |" + strData + " Path:" + FullFilePath);
        //                                        break;
        //                                    }

        //                                    //Added by Akshay on 01-09-2021 for New Trade reading Logic
        //                                    if (LineCounter == Trade_Batch_Count)
        //                                    {
        //                                        dict_CharToSkip[FullFilePath] += CurrentCharToSkip;

        //                                        lock (list_AllTrades)
        //                                            list_AllTrades.AddRange(lst_LocalTrade);

        //                                        //added for testing and Debugging
        //                                        if (isDebug && list_AllTrades.Any())
        //                                            _logger.Error("ReadFO Loop " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs" + ", Trades:" + lst_LocalTrade.Count.ToString(), isDebug);

        //                                        return;
        //                                    }
        //                                }
        //                                dict_CharToSkip[FullFilePath] += CurrentCharToSkip;    //Added by Akshay on 01-09-2021 for New Trade reading Logic
        //                                lock (list_AllTrades)
        //                                    list_AllTrades.AddRange(lst_LocalTrade);
        //                            }
        //                        }
        //                    }
        //                }
        //                catch (Exception FileEx)
        //                {
        //                    _logger.Error("Read FO inner fileentriesFO " + arr_FileEntry.Length + FileEx.ToString());
        //                }
        //            }
        //            else
        //            {
        //                //added on 25JAN2021 by Amey
        //                isTradeFileFOAvailable = false;
        //            }
        //        }



        //        //added for testing and Debugging
        //        if (isDebug && list_AllTrades.Any())
        //            _logger.Error("ReadFO Loop " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs" + ", Trades:" + list_AllTrades.Count.ToString(), isDebug);

        //        //objWriteLog.WriteLog("Time Taken To Read : " + (DateTime.Now - dt_Start).TotalSeconds + " Seconds " + DateTime.Now);
        //    }
        //    catch (Exception foEX)
        //    {
        //        _logger.Error("Read FO " + foEX.ToString());
        //    }
        //}

        //private void ReadCM()
        //{
        //    try
        //    {
        //        DateTime dte_StartReadingTrades = DateTime.Now; //added for testing and Dubugging

        //        //ds_Gateway.dt_AllTradesRow dRow_Trade;
        //        PositionInfo _PositionInfo;

        //        var arr_Combinations = new string[TotalCombinations] { "", "", "", "" };

        //        //added on 19APR2021 by Amey
        //        var ScripNameKey = string.Empty;
        //        var CustomScripNameKey = string.Empty;
        //        var Symbol = string.Empty;
        //        var Series = string.Empty;

        //        //changed on 15JAN2021 by Amey
        //        foreach (var _NoticeCMFilePath in ds_Config.Tables["INTRADAY"].Rows[0]["NOTICE-CM"].ToString().Split(','))
        //        {
        //            string[] arr_FileEntry = Directory.GetFiles(_NoticeCMFilePath, _TradeFileCM + ".txt");
        //            List<PositionInfo> lst_LocalTrade = new List<PositionInfo>();
        //            if (arr_FileEntry.Length > 0)
        //            {
        //                try
        //                {
        //                    string FullFilePath = arr_FileEntry[0];

        //                    if (!dict_RandomEntriesCM.ContainsKey(FullFilePath))//added on 07-02-2020
        //                        dict_RandomEntriesCM.TryAdd(FullFilePath, new HashSet<string>());

        //                    //Added by Akshay on 01-09-2021 for NewTrade reading Logic
        //                    if (!dict_CharToSkip.ContainsKey(FullFilePath))//added on 07-02-2020
        //                        dict_CharToSkip.TryAdd(FullFilePath, 0);

        //                    var CharToSkip = dict_CharToSkip[FullFilePath];
        //                    var CurrentCharToSkip = 0;
        //                    var LineCounter = 0;

        //                    using (FileStream stream = File.Open(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //                    {
        //                        stream.Seek(CharToSkip, SeekOrigin.Begin);

        //                        using (StreamReader sr = new StreamReader(stream))
        //                        {
        //                            string strData = String.Empty;

        //                            while ((strData = sr.ReadLine()) != null)
        //                            {
        //                                //Added by Akshay on 01-09-2021 for New trade reading Logic
        //                                //CurrentCharToSkip += strData.Length + 1;
        //                                //LineCounter += 1;

        //                                //added on 25JAN2021 by Amey
        //                                isTradeFileCMAvailable = true;

        //                                //added on 05JAN2021 by Amey
        //                                string[] arr_Fields = strData.ToUpper().Split(',').Select(v => v.Trim()).ToArray();

        //                                if (arr_Fields.Length > 25 && arr_Fields[0] != "")
        //                                {
        //                                    CurrentCharToSkip += strData.Length + 2;
        //                                    LineCounter += 1;

        //                                    string TradeID = arr_Fields[0];

        //                                    string FullDealerID = arr_Fields[24];
        //                                    string CTCL_ID = FullDealerID.Substring(0, (FullDealerID.Length > 11 ? 12 : FullDealerID.Length));
        //                                    string ClientCode = arr_Fields[14];
        //                                    string UserId = arr_Fields[8];
        //                                    string ClientInfokey = $"{ClientCode}|{CTCL_ID}|{UserId}";

        //                                    arr_Combinations = new string[TotalCombinations] { ClientInfokey, ClientCode, CTCL_ID, UserId };

        //                                    //changed on 10FEB2021 by Amey
        //                                    _PositionInfo = new PositionInfo();

        //                                    bool isValidTrade = false;

        //                                    for (int i = 0; i < arr_Combinations.Length; i++)
        //                                    {
        //                                        try
        //                                        {
        //                                            var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSECM;
        //                                            if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

        //                                            string _check = TradeID + "_" + ClientCode;
        //                                            if (dict_RandomEntriesCM[FullFilePath].Contains(_check)) continue;

        //                                            //added on 19APR2021 by Amey
        //                                            _PositionInfo.Segment = en_Segment.NSECM;
        //                                            Series = arr_Fields[3];
        //                                            Symbol = arr_Fields[2];

        //                                            ScripNameKey = _PositionInfo.Segment + "|" + $"{Symbol}-{Series}";

        //                                            //added on 19APR2021 by Amey
        //                                            ContractMaster _ScripInfo = null;
        //                                            if (!dict_ScripInfo.TryGetValue(ScripNameKey, out _ScripInfo))
        //                                            {
        //                                                CustomScripNameKey = _PositionInfo.Segment + "|" + $"{Symbol}|01JAN1980|0|{Series}";

        //                                                if (!dict_CustomScripInfo.TryGetValue(CustomScripNameKey, out _ScripInfo))
        //                                                {
        //                                                    if (isDebug)
        //                                                        _logger.Error("ReadCM Skipped : " + ScripNameKey + "^" + CustomScripNameKey, true);

        //                                                    continue;
        //                                                }
        //                                            }

        //                                            _PositionInfo.TradePrice = Convert.ToDouble(arr_Fields[12]);

        //                                            //added on 07APR2021 by Amey
        //                                            var Qty = long.Parse(arr_Fields[11]);
        //                                            if (arr_Fields[10] == "2")
        //                                            {
        //                                                _PositionInfo.TradeQuantity = Qty * -1;
        //                                                _PositionInfo.SellQuantity = Qty;
        //                                                _PositionInfo.SellValue = Qty * _PositionInfo.TradePrice;
        //                                            }
        //                                            else
        //                                            {
        //                                                _PositionInfo.TradeQuantity = Qty;
        //                                                _PositionInfo.BuyQuantity = Qty;
        //                                                _PositionInfo.BuyValue = Qty * _PositionInfo.TradePrice;
        //                                            }

        //                                            //added on 10JUN2021 by Amey
        //                                            _PositionInfo.TradeValue = _PositionInfo.TradeQuantity * _PositionInfo.TradePrice;

        //                                            _PositionInfo.IntradayQuantity = _PositionInfo.TradeQuantity;

        //                                            //changed to TradeValue on 10JUN2021 by Amey
        //                                            //changed to - on 13APR2021 by Amey
        //                                            //changed computation logic on 07APR2021 by Amey
        //                                            _PositionInfo.IntradayValue = _PositionInfo.TradeValue; //_PositionInfo.BuyValue - _PositionInfo.SellValue;

        //                                            //added on 01DEC2020 by Amey
        //                                            double dbl_TradeTime = ConvertToUnixTimestamp(DateTime.Parse(arr_Fields[23]));
        //                                            _CMLastTradeTime = dbl_TradeTime > _CMLastTradeTime ? dbl_TradeTime : _CMLastTradeTime;

        //                                            //changed on 19APR2021 by Amey
        //                                            _PositionInfo.Token = _ScripInfo.Token;
        //                                            _PositionInfo.UnderlyingToken = _ScripInfo.UnderlyingToken;
        //                                            _PositionInfo.UnderlyingSegment = _ScripInfo.UnderlyingSegment;

        //                                            isValidTrade = true;

        //                                            //changed position on 28FEB2021 by Amey
        //                                            dict_RandomEntriesCM[FullFilePath].Add(_check);

        //                                            break;
        //                                        }
        //                                        catch (Exception ee) { _logger.Error("ReadCM Loop : " + strData + Environment.NewLine + ee); }
        //                                    }

        //                                    if (isValidTrade)
        //                                    {
        //                                        CMTradeCount += 1;
        //                                        //if (isDebug)
        //                                        //    _logger.Error($"CM Trade Count: {CMTradeCount}", true);

        //                                        HashSet<string> hs_Usernames = new HashSet<string>();
        //                                        for (int i = 0; i < arr_Combinations.Length; i++)
        //                                        {
        //                                            var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSECM;
        //                                            if (!dict_ClientInfo.TryGetValue(UniqueID, out hs_Usernames)) continue;
        //                                            foreach (var Username in hs_Usernames)
        //                                            {
        //                                                var newPosInfo = CopyPropertiesFrom(new PositionInfo(), _PositionInfo);
        //                                                newPosInfo.Username = Username;

        //                                                lst_LocalTrade.Add(newPosInfo);
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    //strData.Trim();
        //                                    /// 
        //                                    /// \n\r
        //                                    /// ,12|345
        //                                    /// 12|345,12345
        //                                    /// <space>
        //                                    /// enter
        //                                    /// 

        //                                    var arr_Check = strData.Split(',');
        //                                    if (arr_Check.Length == 1)
        //                                    {
        //                                        var chkStrData = strData.Trim();
        //                                        CurrentCharToSkip += chkStrData.Length == 0 ? 1 : chkStrData.Length + 2;
        //                                    }

        //                                    _logger.Error("Inconsistent line found |" + strData + " Path:" + FullFilePath);
        //                                    break;
        //                                }

        //                                //Added by Akshay on 01-09-2021 for New Trade reading Logic
        //                                if (LineCounter == Trade_Batch_Count)
        //                                {
        //                                    dict_CharToSkip[FullFilePath] += CurrentCharToSkip;

        //                                    lock (list_AllTrades)
        //                                        list_AllTrades.AddRange(lst_LocalTrade);

        //                                    //added for testing and Debugging
        //                                    if (isDebug && list_AllTrades.Any())
        //                                        _logger.Error("ReadCM Loop " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs" + ", Trades:" + lst_LocalTrade.Count.ToString(), isDebug);

        //                                    return;
        //                                }
        //                            }
        //                            dict_CharToSkip[FullFilePath] += CurrentCharToSkip;  //Added by Akshay on 01-09-2021 for New Trade reading Logic
        //                            lock (list_AllTrades)
        //                                list_AllTrades.AddRange(lst_LocalTrade);

        //                        }
        //                    }
        //                }
        //                catch (Exception ee) { _logger.Error("Read CM " + ee.ToString()); }

        //            }
        //            else
        //            {
        //                //added on 25JAN2021 by Amey
        //                isTradeFileCMAvailable = false;
        //            }
        //        }

        //        //added for testing and Debugging
        //        if (isDebug && list_AllTrades.Any())
        //            _logger.Error("ReadCM Loop " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs" + ", Trades:" + list_AllTrades.Count.ToString(), isDebug);
        //    }
        //    catch (Exception foEX)
        //    {
        //        _logger.Error("Read CM " + foEX.ToString());
        //    }
        //}

        //private void ReadCD()
        //{
        //    try
        //    {
        //        DateTime dte_StartReadingTrades = DateTime.Now; //added for testing and Dubugging

        //        PositionInfo _PositionInfo;

        //        var arr_Combinations = new string[TotalCombinations] { "", "", "", "" };

        //        //added on 19APR2021 by Amey
        //        var ScripNameKey = string.Empty;
        //        var CustomScripNameKey = string.Empty;
        //        var Underlying = string.Empty;
        //        var ScripType = en_ScripType.EQ;
        //        var StrikePrice = 0.0;
        //        DateTime _ExpiryDate = new DateTime(1980, 1, 1, 0, 0, 0);

        //        //changed on 15JAN2021 by Amey
        //        foreach (var _NoticeCDFilePath in ds_Config.Tables["INTRADAY"].Rows[0]["NOTICE-CD"].ToString().Split(','))
        //        {
        //            string[] arr_FileEntry = Directory.GetFiles(_NoticeCDFilePath, _TradeFileCD + ".txt");
        //            List<PositionInfo> lst_LocalTrade = new List<PositionInfo>();
        //            if (arr_FileEntry.Length > 0)
        //            {
        //                try
        //                {
        //                    string FullFilePath = arr_FileEntry[0];

        //                    if (!dict_RandomEntriesCD.ContainsKey(FullFilePath))//added on 07-02-2020
        //                        dict_RandomEntriesCD.TryAdd(FullFilePath, new HashSet<string>());

        //                    //Added by Akshay on 01-09-2021 for NewTrade reading Logic
        //                    if (!dict_CharToSkip.ContainsKey(FullFilePath))//added on 07-02-2020
        //                        dict_CharToSkip.TryAdd(FullFilePath, 0);

        //                    var CharToSkip = dict_CharToSkip[FullFilePath];
        //                    var CurrentCharToSkip = 0;
        //                    var LineCounter = 0;

        //                    using (FileStream fs = File.Open(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //                    {
        //                        fs.Seek(CharToSkip, SeekOrigin.Begin);
        //                        using (BufferedStream bs = new BufferedStream(fs))
        //                        {
        //                            using (StreamReader sr = new StreamReader(bs))
        //                            {
        //                                //Added by  Akshay on 30-08-2021 for reading Latest trade
        //                                //for (var i = 0; i < LineCounter; i++)
        //                                //    sr.ReadLine();

        //                                string strData = string.Empty;

        //                                while ((strData = sr.ReadLine()) != null)
        //                                {
        //                                    //Added by Akshay on 01-09-2021 for New trade reading Logic
        //                                    //CurrentCharToSkip += strData.Length + 1;
        //                                    //LineCounter += 1;

        //                                    //added on 25JAN2021 by Amey
        //                                    isTradeFileCDAvailable = true;

        //                                    //added on 05JAN2021 by Amey
        //                                    string[] arr_Fields = strData.ToUpper().Split(',').Select(v => v.Trim()).ToArray();

        //                                    if (arr_Fields.Length > 27 && arr_Fields[0] != "") //added on 2-1-18
        //                                    {
        //                                        CurrentCharToSkip += strData.Length + 2;
        //                                        LineCounter += 1;

        //                                        string TradeID = arr_Fields[0];

        //                                        string FullDealerID = arr_Fields[26]; // count = 27
        //                                        string CTCL_ID = FullDealerID.Substring(0, (FullDealerID.Length > 11 ? 12 : FullDealerID.Length));
        //                                        string ClientCode = arr_Fields[17];
        //                                        string UserId = arr_Fields[11];
        //                                        string ClientInfokey = $"{ClientCode}|{CTCL_ID}|{UserId}";

        //                                        arr_Combinations = new string[TotalCombinations] { ClientInfokey, ClientCode, CTCL_ID, UserId };

        //                                        //changed on 10FEB2021 by Amey
        //                                        _PositionInfo = new PositionInfo();

        //                                        bool isValidTrade = false;

        //                                        for (int i = 0; i < arr_Combinations.Length; i++)
        //                                        {
        //                                            try
        //                                            {
        //                                                var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSECD;
        //                                                if (!dict_ClientInfo.ContainsKey(UniqueID)) continue;

        //                                                string _check = TradeID + "_" + ClientCode;
        //                                                if (dict_RandomEntriesCD[FullFilePath].Contains(_check)) continue;

        //                                                //added on 19APR2021 by Amey
        //                                                _PositionInfo.Segment = en_Segment.NSECD;

        //                                                //added on 20APR2021 by Amey
        //                                                Underlying = arr_Fields[3];
        //                                                ScripType = arr_Fields[6] == "" ? en_ScripType.XX : (arr_Fields[6] == "CE" ? en_ScripType.CE : en_ScripType.PE);
        //                                                StrikePrice = Convert.ToDouble(ScripType == en_ScripType.XX ? "0" : arr_Fields[5]);
        //                                                _ExpiryDate = Convert.ToDateTime(arr_Fields[4]);

        //                                                ScripNameKey = _PositionInfo.Segment + "|" + arr_Fields[7].ToUpper();

        //                                                //added on 19APR2021 by Amey
        //                                                ContractMaster _ScripInfo = null;
        //                                                if (!dict_ScripInfo.TryGetValue(ScripNameKey, out _ScripInfo))
        //                                                {
        //                                                    CustomScripNameKey = _PositionInfo.Segment + "|" + $"{Underlying}|{_ExpiryDate.ToString("ddMMMyyyy").ToUpper()}|{(StrikePrice == 0 ? "0" : StrikePrice.ToString("#.00"))}|{ScripType}";

        //                                                    if (!dict_CustomScripInfo.TryGetValue(CustomScripNameKey, out _ScripInfo))
        //                                                    {
        //                                                        if (isDebug)
        //                                                            _logger.Error("ReadCD Skipped : " + ScripNameKey + "^" + CustomScripNameKey, true);

        //                                                        continue;
        //                                                    }
        //                                                }

        //                                                _PositionInfo.TradePrice = Convert.ToDouble(arr_Fields[15]);

        //                                                //added on 07APR2021 by Amey
        //                                                var Qty = long.Parse(arr_Fields[14]);
        //                                                if (arr_Fields[13] == "2")
        //                                                {
        //                                                    _PositionInfo.TradeQuantity = Qty * _ScripInfo.LotSize * -1;
        //                                                    _PositionInfo.SellQuantity = Qty * _ScripInfo.LotSize;
        //                                                    _PositionInfo.SellValue = Qty * _ScripInfo.LotSize * _PositionInfo.TradePrice;
        //                                                }
        //                                                else
        //                                                {
        //                                                    _PositionInfo.TradeQuantity = Qty * _ScripInfo.LotSize;
        //                                                    _PositionInfo.BuyQuantity = Qty * _ScripInfo.LotSize;
        //                                                    _PositionInfo.BuyValue = Qty * _ScripInfo.LotSize * _PositionInfo.TradePrice;
        //                                                }

        //                                                //added on 10JUN2021 by Amey
        //                                                _PositionInfo.TradeValue = _PositionInfo.TradeQuantity * _PositionInfo.TradePrice;

        //                                                _PositionInfo.IntradayQuantity = _PositionInfo.TradeQuantity;

        //                                                //changed to TradeValue on 10JUN2021 by Amey
        //                                                //changed to - on 13APR2021 by Amey
        //                                                //changed computation logic on 07APR2021 by Amey
        //                                                _PositionInfo.IntradayValue = _PositionInfo.TradeValue; //_PositionInfo.BuyValue - _PositionInfo.SellValue;

        //                                                //added on 01DEC2020 by Amey
        //                                                double dbl_TradeTime = ConvertToUnixTimestamp(DateTime.Parse(arr_Fields[25]));
        //                                                _CDLastTradeTime = dbl_TradeTime > _CDLastTradeTime ? dbl_TradeTime : _CDLastTradeTime;

        //                                                //changed on 19APR2021 by Amey
        //                                                _PositionInfo.Token = _ScripInfo.Token;
        //                                                _PositionInfo.UnderlyingToken = _ScripInfo.UnderlyingToken;
        //                                                _PositionInfo.UnderlyingSegment = _ScripInfo.UnderlyingSegment;

        //                                                isValidTrade = true;

        //                                                //changed position on 28FEB2021 by Amey
        //                                                dict_RandomEntriesCD[FullFilePath].Add(_check);

        //                                                break;
        //                                            }
        //                                            catch (Exception ee) { _logger.Error("ReadCD Loop : " + strData + Environment.NewLine + ee); }
        //                                        }

        //                                        if (isValidTrade)
        //                                        {
        //                                            CDTradeCount += 1;
        //                                            //if (isDebug)
        //                                            //    _logger.Error($"CD Trade Count: {CDTradeCount}", true);

        //                                            HashSet<string> hs_usernames = new HashSet<string>();

        //                                            for (int i = 0; i < arr_Combinations.Length; i++)
        //                                            {
        //                                                var UniqueID = arr_Combinations[i] + "|" + en_Segment.NSECD;
        //                                                if (!dict_ClientInfo.TryGetValue(UniqueID, out hs_usernames)) continue;

        //                                                //string Username = dict_ClientInfo[UniqueID];
        //                                                foreach (var Username in hs_usernames)
        //                                                {
        //                                                    #region Span using dll

        //                                                    try
        //                                                    {
        //                                                        Span _spn = new Span()
        //                                                        {
        //                                                            pMemberId = 1,
        //                                                            pClientId = Username + "_" + Underlying,
        //                                                            pExchange = "NSE",
        //                                                            pSegment = "CURR",
        //                                                            pScripName = Underlying,
        //                                                            pExpiry = _ExpiryDate.ToString("yyyyMMdd"),
        //                                                            pFactor = "E",
        //                                                            pQty = long.Parse(arr_Fields[14]).ToString(),   //.ToString(),
        //                                                            //added on 11JUN2021 by Amey
        //                                                            pTradeTime = _CDLastTradeTime
        //                                                        };

        //                                                        if (ScripType == en_ScripType.XX)
        //                                                        {
        //                                                            _spn.pStrikePrice = "";
        //                                                            _spn.pCallPut = "";
        //                                                        }
        //                                                        else
        //                                                        {
        //                                                            _spn.pStrikePrice = StrikePrice.ToString("#.0000");
        //                                                            _spn.pCallPut = ScripType.ToString().Substring(0, 1);
        //                                                        }

        //                                                        eve_CDSSpanRead(_spn);
        //                                                    }
        //                                                    catch (Exception spanEx)//added try catch for individual span calculation on 06-01-2020
        //                                                    {
        //                                                        _logger.Error("Span calculation individual scrip " + spanEx);
        //                                                    }

        //                                                    #endregion

        //                                                    var newPosInfo = CopyPropertiesFrom(new PositionInfo(), _PositionInfo);
        //                                                    newPosInfo.Username = Username;

        //                                                    //changed on 10FEB2021 by Amey
        //                                                    lst_LocalTrade.Add(newPosInfo);
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        var arr_Check = strData.Split(',');
        //                                        if (arr_Check.Length == 1)
        //                                        {
        //                                            var chkStrData = strData.Trim();
        //                                            CurrentCharToSkip += chkStrData.Length == 0 ? 1 : chkStrData.Length + 2;
        //                                        }

        //                                        _logger.Error("Inconsistent line found |" + strData + " Path:" + FullFilePath);
        //                                        break;
        //                                    }

        //                                    //Added by Akshay on 01-09-2021 for New Trade reading Logic
        //                                    if (LineCounter == Trade_Batch_Count)
        //                                    {
        //                                        dict_CharToSkip[FullFilePath] += CurrentCharToSkip;

        //                                        lock (list_AllTrades)
        //                                            list_AllTrades.AddRange(lst_LocalTrade);

        //                                        //added for testing and Debugging
        //                                        if (isDebug && list_AllTrades.Any())
        //                                            _logger.Error("ReadFO Loop " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs" + ", Trades:" + lst_LocalTrade.Count.ToString(), isDebug);

        //                                        return;
        //                                    }
        //                                }
        //                                dict_CharToSkip[FullFilePath] += CurrentCharToSkip;
        //                                lock (list_AllTrades)
        //                                    list_AllTrades.AddRange(lst_LocalTrade);
        //                            }
        //                        }
        //                    }
        //                }
        //                catch (Exception FileEx)
        //                {
        //                    _logger.Error("Read CD inner fileentriesCD " + arr_FileEntry.Length + FileEx.ToString());
        //                }
        //            }
        //            else
        //            {
        //                //added on 25JAN2021 by Amey
        //                isTradeFileFOAvailable = false;
        //            }
        //        }



        //        //added for testing and Debugging
        //        if (isDebug && list_AllTrades.Any())
        //            _logger.Error("ReadFO Loop " + Math.Round((DateTime.Now - dte_StartReadingTrades).TotalSeconds, 2) + "secs" + ", Trades:" + list_AllTrades.Count.ToString(), isDebug);

        //    }
        //    catch (Exception foEX)
        //    {
        //        _logger.Error("Read CD " + foEX.ToString());
        //    }
        //}
        #endregion



        #endregion

        #region Supplimentary Methods

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

        #endregion
    }
}
