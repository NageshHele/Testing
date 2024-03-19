using DevExpress.Data.Extensions;
using DevExpress.XtraBars.FluentDesignSystem;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using Microsoft.Win32;
using n.Structs;
using NerveLog;
using Prime.Core_Logic;
using Prime.Helper;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prime.UI
{
    public partial class form_Main : FluentDesignForm
    {
        form_ClientWindow _ClientWindow = new form_ClientWindow();     //Added by Akshay on 28-07-2021 for Client Window

        NerveLogger _logger;

        /// <summary>
        /// Previously selected element from SideBar.
        /// </summary>
        AccordionControlElement acc_Previous = new AccordionControlElement();

        /// <summary>
        /// Logged in Username.
        /// </summary>
        string Username = string.Empty;

        /// <summary>
        /// Selected Username in ClientWindow.
        /// </summary>
        string _SelectedClientID = string.Empty;

        /// <summary>
        /// Selected Username in ClientWindow.
        /// </summary>
        string _SelectedUnderlying = string.Empty;

        /// <summary>
        /// Selected Username in ClientWindow.
        /// </summary>
        string _SelectedExpiry = string.Empty;

        #region Flags

        /// <summary>
        /// Set True when process previously received Span Data from Engine Socket. False when processing is finished.
        /// </summary>
        bool isSpanUpdating = false;

        /// <summary>
        /// Set True when process previously received Position Data from Engine Socket. False when processing is finished.
        /// </summary>
        bool isPositionProcessing = false;

        /// <summary>
        /// Set true when Compute clicked from Scenario form.
        /// </summary>
        bool isScenarioComputeClicked = false;

        /// <summary>
        /// Set true when Compute clicked from All Trades form.
        /// </summary>
        bool RequestTradesInAllPositionsTab = true;

        /// <summary>
        /// Set true when ClientPortfolio tab is visible.
        /// </summary>
        bool isClientPortfolioVisible = true;

        /// <summary>
        /// Set true when UnderlyingView tab is visible.
        /// </summary>
        bool isUnderlyingViewVisible = true;

        /// <summary>
        /// Set true when ConcentrationRisk tab is visible.
        /// </summary>
        bool isConcentrationRiskVisible = false;

        /// <summary>
        /// Set true for NPL calculation
        /// </summary>
        bool NPL_Flag = true;   //Added by Akshay on 19-07-2021 for NPL Values

        /// <summary>
        /// Set true when ClientWindow form is visible.
        /// </summary>
        bool isClientWindowViewVisible = false; //Added by Akshay on 28-07-2021 for client Window

        //Added by Akshay on 25-10-2021 for Delivery Report
        /// <summary>
        /// Set true when DeliveryReport tab is visible.
        /// </summary>
        bool isDeliveryReportVisible = true;

        /// <summary>
        /// Sets true when AutoSaving
        /// </summary>
        bool isAutoSaving = false;

        /// <summary>
        /// Set True when corresponding span dictionary is updating
        /// </summary>
        bool isSpanMarginUpdating = false;
        bool isEODMarginUpdating = false;
        bool isEXPMarginUpdating = false;
        bool isPeakMarginUpdating = false;
        bool isCDSPeakMarginUpdating = false;
        bool isConMarginUdating = false;

        bool isCDSpanMarginUpdating = false;
        bool isCDEODMarginUpdating = false;
        bool isCDEXPMarginUpdating = false;

        #endregion

        #region Locks
        private object _SpanEODLock = new object();
        private object _SpanExpLock = new object();
        private object _SpanLock = new object();
        private object _SpanPeakLock = new object();
        private object _SpanConLock = new object();

        private object _CDSSpanEODLock = new object();
        private object _CDSSpanExpLock = new object();
        private object _CDSSpanLock = new object();
        private object _CDSSpanPeakLock = new object();

        #endregion

        #region Dictionaries

        // <summary>
        /// Key : ClientID | Value : Compute Class Object
        /// </summary>
        ConcurrentDictionary<string, nCompute> dict_ClientCompute = new ConcurrentDictionary<string, nCompute>();

        // <summary>
        /// Key : Underlying | Value : Compute Class Object
        /// </summary>
        ConcurrentDictionary<string, nCompute> dict_UnderlyingCompute = new ConcurrentDictionary<string, nCompute>();

        /// <summary>
        /// Key : ClientID | Value : ConsolidatedPositionInfo
        /// </summary>
        Dictionary<string, List<ConsolidatedPositionInfo>> dict_ClientWisePositions = new Dictionary<string, List<ConsolidatedPositionInfo>>();

        /// <summary>
        /// Key : ClientId | [0] Span, [1] Exposure
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_ClientWiseMargin = new ConcurrentDictionary<string, double[]>();

        /// <summary>
        /// Key : ClientId | [0] Span, [1] Exposure
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_ClientWiseExpMargin = new ConcurrentDictionary<string, double[]>();    //Added on 10-12-2020 by Akshay For Expiry Span

        /// <summary>
        /// Key : ClientId | [0] Span, [1] Exposure
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_ClientWiseEODMargin = new ConcurrentDictionary<string, double[]>();    //Added on 10-12-2020 by Akshay For Expiry Span
        
        /// <summary>
        /// Key : ClientId | [0] Span, [1] Exposure
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_ClientWiseConsolidatedMargin = new ConcurrentDictionary<string, double[]>();

        /// /// <summary>
        /// Key : ClientId | [0] Span, [1] Exposure
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_ClientWiseConsolidatedEXPMargin = new ConcurrentDictionary<string, double[]>();    //Added on 22AUG2022 by Ninad 

        /// <summary>
        /// Key : ClientID | Value : State
        /// </summary>
        ConcurrentDictionary<string, bool> dict_IsCPExpanded = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Key : ClientID : Value : Format : List of expanded Underlying
        /// </summary>
        ConcurrentDictionary<string, List<string>> dict_ExpandedUnderlyings = new ConcurrentDictionary<string, List<string>>();

        /// <summary>
        /// Key : ClientID : Value : Format : State
        /// </summary>
        ConcurrentDictionary<string, bool> dict_IsUCExpanded = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Key : ClientID | Value : State
        /// </summary>
        ConcurrentDictionary<string, bool> dict_IsVaRDistributionShown = new ConcurrentDictionary<string, bool>();

        //Added by Akshay on 23-07-2021 for NPL
        /// <summary>
        /// Key : Underlying | Value : NPL_Values
        /// </summary>
        ConcurrentDictionary<string, double> dict_UnderlyingLevelNPL = new ConcurrentDictionary<string, double>();

        // <summary>
        /// Key : ClientID | Value : Compute Class Object
        /// </summary>
        ConcurrentDictionary<string, nCompute> dict_ClientComputeCW = new ConcurrentDictionary<string, nCompute>();

        //added by Akshay on 25-10-2021 for Delivery Report
        /// <summary>
        /// Key : ClientID | Value : State
        /// </summary>
        ConcurrentDictionary<string, bool> dict_IsDRCPExpanded = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Key : ClientID : Value : Format : List of expanded Underlying
        /// </summary>
        ConcurrentDictionary<string, List<string>> dict_DRExpandedUnderlyings = new ConcurrentDictionary<string, List<string>>();

        /// <summary>
        /// Key : ClientId | [0] Span, [1] Exposure
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_CDSClientWiseExpMargin = new ConcurrentDictionary<string, double[]>();    //Added on 10-12-2020 by Akshay For Expiry Span


        /// <summary>
        /// Key : ClientId | [0] Span, [1] Exposure
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_CDSClientWiseEODMargin = new ConcurrentDictionary<string, double[]>();    //Added on 10-12-2020 by Akshay For Expiry Span


        /// <summary>
        /// Key : ClientId | [0] Span, [1] Exposure
        /// </summary>
        ConcurrentDictionary<string, double[]> dict_CDSClientWiseMargin = new ConcurrentDictionary<string, double[]>();



        #endregion

        public form_Main(Thread STAThread)
        {
            this._logger = CollectionHelper._logger;

            try
            {
                var _Login = new form_Login(this);
                _Login.ShowDialog();

                if (_Login.DialogResult == DialogResult.Cancel && Username == "")
                    Environment.Exit(0);

                CollectionHelper.InitialiseCollections();

                if (InvokeRequired || (STAThread != Thread.CurrentThread))
                    _logger.Debug($"NotUIThread:{Thread.CurrentThread.Name}|{Thread.CurrentThread.ManagedThreadId}|{Thread.CurrentThread.IsThreadPoolThread}");

                InitializeComponent();

                UnsubscribeUserPreferenceChangedEvent();
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        #region Form Events

        private void Main_Shown(object sender, EventArgs e)
        {
            //added on 09MAY2021 by Amey. Dont remember when it ws removed by me accidently.
            Invoke((MethodInvoker)(() =>
            {
                Text = "n.Prime | " + Username;
            }));

            uc_ClientPortfolio.Initialise();
            uc_ConcentrationRisk.Initialise();
            uc_ScenarioAnalysis.Initialise();
            uc_Violations.Initialise();
            uc_AllPositions.Initialise();
            uc_DeliveryReport.Initialise();

            //added on 1JUN2021 by Amey
            uc_UnderlyingClients.Initialise();

            uc_Indicators.Initialise();

            //added on 3JUN2021 by Amey
            //uc_Changelog.Initialise();

            if (!panel_TopIndicators.Controls.Contains(uc_Indicators.Instance))
            {
                panel_TopIndicators.Controls.Add(uc_Indicators.Instance);
                uc_Indicators.Instance.Dock = DockStyle.Fill;
            }
            uc_Indicators.Instance.BringToFront();

            //added on 3JUN2021 by Amey
            //if (!popupControlContainer_Changelog.Controls.Contains(uc_Changelog.Instance))
            //{
            //    popupControlContainer_Changelog.Controls.Add(uc_Changelog.Instance);
            //    uc_Changelog.Instance.Dock = DockStyle.Fill;
            //}
            //uc_Changelog.Instance.BringToFront();

            //added on 13APR2021 by Amey for faster tab switching.

            //added on 1JUN2021 by Amey
            HighlightAndShowSelected(accEle_UnderlyingClients, uc_UnderlyingClients.Instance, false);

            HighlightAndShowSelected(accEle_ConcentrationRisk, uc_ConcentrationRisk.Instance, false);
            HighlightAndShowSelected(accEle_ScenarioAnalysis, uc_ScenarioAnalysis.Instance, false);
            HighlightAndShowSelected(accEle_Violations, uc_Violations.Instance, false);
            HighlightAndShowSelected(accEle_AllPositons, uc_AllPositions.Instance, false);
            HighlightAndShowSelected(accEle_DeliveryReport, uc_DeliveryReport.Instance, false);

            HighlightAndShowSelected(accEle_ClientPortfolio, uc_ClientPortfolio.Instance);

            InitialiseRequiredEvents();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (XtraMessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    Environment.Exit(0);
                }
                catch (Exception ee) { _logger.Error(ee); Environment.Exit(0); }
            }
            else
                e.Cancel = true;
        }

        private void accEle_ClientPortfolio_Click(object sender, EventArgs e)
        {
            try
            {
                HighlightAndShowSelected(accEle_ClientPortfolio, uc_ClientPortfolio.Instance);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void accEle_ConcentrationRisk_Click(object sender, EventArgs e)
        {
            try
            {
                HighlightAndShowSelected(accEle_ConcentrationRisk, uc_ConcentrationRisk.Instance);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void accEle_ScenarioAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                HighlightAndShowSelected(accEle_ScenarioAnalysis, uc_ScenarioAnalysis.Instance);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void accEle_Violations_Click(object sender, EventArgs e)
        {
            try
            {
                HighlightAndShowSelected(accEle_Violations, uc_Violations.Instance);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void accEle_AllPositons_Click(object sender, EventArgs e)
        {
            try
            {
                HighlightAndShowSelected(accEle_AllPositons, uc_AllPositions.Instance);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void accEle_UnderlyingClients_Click(object sender, EventArgs e)
        {
            try
            {
                HighlightAndShowSelected(accEle_UnderlyingClients, uc_UnderlyingClients.Instance);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }


        private void accEle_DeliveryReport_Click(object sender, EventArgs e)
        {
            try
            {
                HighlightAndShowSelected(accEle_DeliveryReport, uc_DeliveryReport.Instance);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        // Added by Snehadri on 21OCT2021 for Autosave Positon
        private void timer_AutosavePositions_Tick(object sender, EventArgs e)
        {
            try
            {
                if(!isAutoSaving)
                    Task.Run(() => AutoSavePositions());
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        #endregion

        #region Imp Methods

        internal void Initialise(EngineHBConnector _EngineHeartBeatClient, string Username)
        {
            try
            {
                this.Username = Username;
                
                _EngineHeartBeatClient.eve_HeartBeatTickReceived += _EngineHeartBeatClient_eve_HeartBeatTickReceived;
                _EngineHeartBeatClient.eve_LoginResponse += _EngineHeartBeatClient_eve_LoginResponse;
                _EngineHeartBeatClient.eve_ClientInfoReceived += _EngineHeartBeatClient_eve_ClientInfoReceived;
                //added by nikhil
                _EngineHeartBeatClient.eve_ClientUpateRecevied += _EngineHeartBeatClient_eve_ClientUpdateRecived;

            }
            catch (Exception ee) { _logger.Error(ee); }
        }


        #region Engine Socket Events
        
        //added by Nikhil 
        private void _EngineHeartBeatClient_eve_ClientUpdateRecived()
        {
            try
            {
                RemoveClients();
            }
            catch(Exception ee) { _logger.Error(ee);}
        }

        //added by nikhil
        private void RemoveClients()
        {
            try
            {

                var bList_ClientPortfolio = CollectionHelper.bList_ClientPortfolio;
                var bList_UnderlyingClients = CollectionHelper.bList_UnderlyingClients;
                var blist_ClientWindow = CollectionHelper.bList_ClientWindow;

                var dict_ComboUniverse = CollectionHelper.dict_ComboUniverse;
                var dict_ClientInfo = CollectionHelper.dict_ClientInfo;

                var dict_UniqueClients = CollectionHelper.dict_UniqueClients;
                var dict_UniqueTokens = CollectionHelper.dict_UniqueTokens;

                var dict_ClientWiseSpanInfo = CollectionHelper.dict_ClientWiseSpanInfo;
                var dict_ClientUnderlyingWiseSpanInfo = CollectionHelper.dict_ClientUnderlyingWiseSpanInfo;
                var bList_ConcentrationRisk = CollectionHelper.bList_ConcentrationRisk;

                var list_ScripKey = new List<string>();

                var dict_BanInfo = CollectionHelper.dict_BanInfo;


                if (!isClientPortfolioVisible)
                {
                    isClientPortfolioVisible = true;  // used for - if new client is added and we aren't visited client postfolio then new users are not there posfolio dictonary or list
                }

                // CollectionHelper.gc_UC.Invoke((MethodInvoker)(() => {
                try
                {
                    foreach (var CPParent in bList_ClientPortfolio.ToList())
                    {
                        if (!dict_ClientInfo.ContainsKey(CPParent.ClientID))
                        {
                            //remove from dictionary
                            dict_ClientCompute.TryRemove(CPParent.ClientID, out _);
                            //checking client window
                            dict_ComboUniverse.TryRemove(CPParent.ClientID, out _);
                            dict_ClientComputeCW.TryRemove(CPParent.ClientID, out _);

                            lock (CollectionHelper._CPLock)
                            {
                                var All_ScripKeyClientWise = CPParent.bList_Underlying.SelectMany(x => x.bList_Positions.Select(y => y.Segment + "|" + y.ScripToken)).ToList();
                                if (All_ScripKeyClientWise != null || All_ScripKeyClientWise.Count > 0)
                                {
                                    list_ScripKey.AddRange(All_ScripKeyClientWise);
                                }
                            }

                            CollectionHelper.gc_CP.Invoke((MethodInvoker)(() =>
                            {
                                bList_ClientPortfolio.Remove(CPParent);
                            }));

                        }
                    }

                }
                catch (Exception ee)
                {
                    _logger.Error(ee, "CLIENT UPDATE-CLIENT PORTFOLIO");
                }

                //remove from concentration risk
                //
                try
                {
                    foreach (var _SpanMargin in bList_ConcentrationRisk.ToList())
                    {
                        if (!dict_ClientInfo.ContainsKey(_SpanMargin.Client))
                        {
                            Invoke((MethodInvoker)(() =>
                            {
                                bList_ConcentrationRisk.Remove(_SpanMargin);

                                dict_ClientWiseSpanInfo.TryRemove(_SpanMargin.Client, out _);
                                //dict_ClientWiseSpanInfo.TryRemove(CPParent.ClientID, out _);
                            }));

                            lock (_SpanEODLock)
                            {
                                dict_ClientWiseEODMargin.TryRemove(_SpanMargin.Client, out _);
                            }
                            lock (_SpanExpLock)
                            {
                                dict_ClientWiseExpMargin.TryRemove(_SpanMargin.Client, out _);
                            }
                            lock (_SpanLock)
                            {
                                dict_ClientWiseMargin.TryRemove(_SpanMargin.Client, out _);
                            }

                        }
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error(ee, "CLIENT UPDATE- CONCENTRATION RISK");
                }


                try
                {
                    foreach (var UCParent in bList_UnderlyingClients.ToList())
                    {
                        foreach (var UCClient in UCParent.bList_Clients.ToList())
                        {
                            if (!dict_ClientInfo.ContainsKey(UCClient.ClientID))
                            {
                                CollectionHelper.gc_UC.Invoke((MethodInvoker)(() =>
                                {
                                    UCParent.bList_Clients.Remove(UCClient);
                                }));
                            }
                        }
                        if (UCParent.bList_Clients.ToList().Count == 0 || UCParent.bList_Clients.ToList() == null)
                        {

                            bList_UnderlyingClients.Remove(UCParent);
                            //remove from dictionary
                            dict_UnderlyingCompute.TryRemove(UCParent.Underlying, out _);
                            dict_ClientUnderlyingWiseSpanInfo.TryRemove(UCParent.Underlying, out _);
                        }
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error(ee, "CLIENT UPDATE- UNDERLYING VIEW");
                }

                //remove from banned info 
                foreach (var Client in dict_BanInfo)
                {
                    if (!dict_ClientInfo.TryGetValue(Client.Key, out _))
                    {
                        dict_BanInfo.TryRemove(Client.Key, out BanInfo info);
                    }
                }

                //update unique clients
                foreach (var client in CollectionHelper.dict_UniqueClients)
                {
                    if (!dict_ClientInfo.ContainsKey(client.Key))
                    {
                        CollectionHelper.dict_UniqueClients.TryRemove(client.Key, out _);
                    }
                }

                //upadte unique scrip
                if (list_ScripKey != null || list_ScripKey.Count > 0)
                {
                    foreach (var ScripKey in list_ScripKey)
                    {
                        var isScripPresent = false;
                        foreach (var CPParent in bList_ClientPortfolio.ToList())
                        {
                            var All_ScripKey = CPParent.bList_Underlying.SelectMany(x => x.bList_Positions.Select(y => y.Segment + "|" + y.ScripToken)).ToList();
                            if (All_ScripKey.Contains(ScripKey))
                            {
                                isScripPresent = true;
                                break;
                            }
                        }
                        if (!isScripPresent)
                        {
                            dict_UniqueTokens.TryRemove(ScripKey, out _);
                        }
                    }
                }


                Invoke((MethodInvoker)(() =>
                {
                    lbl_NoOfClientsVal.Text = CollectionHelper.dict_UniqueClients.Count.ToString();
                    lbl_NoOfScripsVal.Text = CollectionHelper.dict_UniqueTokens.Count.ToString();
                }));

            }
            catch (Exception ee)
            {
                // CollectionHelper.isClientUpdating = false;

                _logger.Error(ee);
            }

        }

        private void _EngineHeartBeatClient_eve_LoginResponse(bool isSuccess, string _Message) { }

        /// <summary>
        /// Will be called to when tick received from Engine HeartBeat Server.
        /// </summary>
        /// <param name="_ReceivedTick">Format : {FOLTT}_{CMLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{isGatewayConnected}_{isSpanConnected}_{NIFTY}_{BANKNIFTY}</param>
        private void _EngineHeartBeatClient_eve_HeartBeatTickReceived(string _ReceivedTick)
        {
            uc_Indicators.Instance?.UpdateIndicators(_ReceivedTick);

            //added on 08APR2021 by Amey
            try
            {
                if (IsHandleCreated)
                {
                    string[] arr_fields = _ReceivedTick.Split('_');

                    if (arr_fields.Length > 10)
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            lbl_NiftyVal.Text = arr_fields[12];
                            lbl_BankNiftyVal.Text = arr_fields[13];
                        }));
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void _EngineHeartBeatClient_eve_ClientInfoReceived(bool flag_State, string ID = "") { }

        private void _EngineDataClient_eve_SpanData(Dictionary<string, double[]> dict_ReceivedSpan,String Marker)
        {
            ProcessSpanData(dict_ReceivedSpan,Marker);
        }

        private void _EngineDataClient_eve_PositionsReceived(Dictionary<string, List<ConsolidatedPositionInfo>> dict_ReceivedClientWiseTrades)
        {
            //_logger.Debug("Received : " + DateTime.Now.ToString("ss:fff"));
            ProcessPositionData(dict_ReceivedClientWiseTrades);
            
        }

        private void _EngineDataClient_eve_EngineStatus(bool isEngineConnected, string _ReceivedStatus)
        {
            try
            {
                uc_Indicators.Instance.UpdateSocketIndicator(isEngineConnected, _ReceivedStatus);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

       
        #endregion

        private void UpdateLedgerValue()
        {
            try
            {
                var dict_clientInfo = CollectionHelper.dict_ClientInfo;
                lock (CollectionHelper._CPLock)
                {
                    foreach (var _CPParent in CollectionHelper.bList_ClientPortfolio.ToList())
                    {
                        if (dict_clientInfo.TryGetValue(_CPParent.ClientID, out ClientInfo Cinfo))
                        {
                            _CPParent.Ledger = Cinfo.ELM;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.Error(ee);
            }
        }


        private void ProcessSpanData(Dictionary<string, double[]> dict_ReceivedSpanData,string Marker)
        {
            try
            {
                // Changed logic by Snehadri on 09Mar2022

                if (Marker == "FO^")
                {

                    try
                    {
                        var dict_Eod = dict_ReceivedSpanData.Where(x => x.Key.Split('_').Length == 3 && x.Key.Split('_')[2] == "EOD").ToDictionary(x => x.Key, x => x.Value);
                        if (dict_Eod.Any())  //Added isExpirySpan Flag on 10-12-2020 by Akshay
                        {
                            Task.Run(() => UpdateEODSpanMargin(dict_Eod));
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee, "Merge Dictionaries (EOD):"); }

                    try
                    {
                        var dict_Span = dict_ReceivedSpanData.Where(x => x.Key.Split('_').Length == 2 && x.Key.Split('_')[1] != "PEAK").ToDictionary(x => x.Key, x => x.Value);
                        if (dict_Span.Any())    //Added isExpirySpan Flag on 10-12-2020 by Akshay
                        {
                            Task.Run(() => UpdateSpanMargin(dict_Span));
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee, "Merge Dictionaries (SpanMargin):"); }

                    try
                    {
                        var dict_Exp = dict_ReceivedSpanData.Where(x => x.Key.Split('_').Length == 3 && x.Key.Split('_')[2] == "EXP").ToDictionary(x => x.Key, x => x.Value);
                        if (dict_Exp.Any())
                        {
                            Task.Run(() => UpdateEXPSpanMargin(dict_Exp));
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee, "Merge Dictionaries (EXP):"); }

                    try
                    {
                        var dict_Peak = dict_ReceivedSpanData.Where(x => x.Key.Split('_').Length == 2 && x.Key.Split('_')[1] == "PEAK").ToDictionary(x => x.Key, x => x.Value);
                        if (dict_Peak.Any())   // Added by Snehadri on 09MAR2022 for Peak Margin data 
                        {
                            Task.Run(() => UpdatePeakMargin(dict_Peak));
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee, "Merge Dictionaries (PEAK):"); }

                    try
                    {
                        var dict_Con = dict_ReceivedSpanData.Where(x => x.Key.Split('_').Length == 4 && x.Key.Split('_')[2] == "CON").ToDictionary(x => x.Key, x => x.Value);
                        if (dict_Con.Any())
                        {
                            Task.Run(() => UpdateConMargin(dict_Con));
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee, "Merge Dictionaries (CON):"); }
                }

                if (Marker == "CD^")
                {
                    try
                    {
                        var dict_Eod = dict_ReceivedSpanData.Where(x => x.Key.Split('_').Length == 3 && x.Key.Split('_')[2] == "CDSEOD").ToDictionary(x => x.Key, x => x.Value);
                        if (dict_Eod.Any())  //Added isExpirySpan Flag on 10-12-2020 by Akshay
                        {
                            Task.Run(() => UpdateCDEODSpanMargin(dict_Eod));
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee, "Merge Dictionaries (EOD):"); }

                    try
                    {
                        var dict_Span = dict_ReceivedSpanData.Where(x => x.Key.Split('_').Length == 3 && x.Key.Split('_')[2] == "CDS").ToDictionary(x => x.Key, x => x.Value);
                        if (dict_Span.Any())    //Added isExpirySpan Flag on 10-12-2020 by Akshay
                        {
                            Task.Run(() => UpdateCDSpanMargin(dict_Span));
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee, "Merge Dictionaries (SpanMargin):"); }

                    try
                    {
                        var dict_Exp = dict_ReceivedSpanData.Where(x => x.Key.Split('_').Length == 3 && x.Key.Split('_')[2] == "CDSEXP").ToDictionary(x => x.Key, x => x.Value);
                        if (dict_Exp.Any())
                        {
                            Task.Run(() => UpdateCDEXPSpanMargin(dict_Exp));
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee, "Merge Dictionaries (EXP):"); }

                    try
                    {
                        var dict_Peak = dict_ReceivedSpanData.Where(x => x.Key.Split('_').Length == 2 && x.Key.Split('_')[1] == "PEAK").ToDictionary(x => x.Key, x => x.Value);
                        if (dict_Peak.Any())   // Added by Snehadri on 09MAR2022 for Peak Margin data 
                        {
                            Task.Run(() => UpdatePeakMarginCDS(dict_Peak));
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee, "Merge Dictionaries (PEAK):"); }
                }

            }
            catch (Exception ee) { _logger.Error(ee); isSpanUpdating = false; }
        }

        private void UpdateSpanMargin(Dictionary<string, double[]> dict_ReceivedSpanData)
        {
            try
            {
                if (!isSpanMarginUpdating)
                {
                    isSpanMarginUpdating = true;

                    lock (_SpanLock)
                    {
                        string Client = "";
                        string Underlying = "";

                        //added on 26OCT2020 by Amey
                        double Span = 0;
                        double Exposure = 0;

                        double[] arr_Margin;

                        //added on 31MAR2021 by Amey
                        var dict_ClientUnderlyingWiseSpanInfo = CollectionHelper.dict_ClientUnderlyingWiseSpanInfo;

                        foreach (var SpanKey in dict_ReceivedSpanData.Keys)
                        {
                            try
                            {
                                Client = SpanKey.Split('_')[0];
                                Underlying = SpanKey.Split('_')[1];

                                //added on 28APR2021 by Amey
                                arr_Margin = dict_ReceivedSpanData[SpanKey];

                                //changed names on 26OCT2020 by Amey
                                //Span = arr_Margin[0] + arr_Margin[2];
                                Span = arr_Margin[0];   //changed on 22AUG2022 by Ninad
                                Exposure = arr_Margin[1];

                                //changed on 16FEB2021 by Amey
                                if (dict_ClientWiseMargin.ContainsKey(Client))
                                {
                                    dict_ClientWiseMargin[Client][0] += Span;
                                    dict_ClientWiseMargin[Client][1] += Exposure;
                                }
                                else
                                    dict_ClientWiseMargin.TryAdd(Client, new double[2] { Span, Exposure });

                                Span = DivideByBaseAndRound(Span, nameof(CPParent.Span));
                                Exposure = DivideByBaseAndRound(Exposure, nameof(CPParent.Exposure));

                                //added on 18FEB2021 by Amey
                                var _USpanInfo = new UnderlyingSpanInfo()
                                {
                                    Underlying = Underlying,
                                    Span = Span,
                                    Exposure = Exposure,

                                    //fixed on 27MAY2021 by Amey
                                    MarginUtil = (Span < 0 ? 0 : Span) + Exposure
                                };

                                //changed from AddOrUpdate on 01MAR2021 by Amey
                                if (dict_ClientUnderlyingWiseSpanInfo.ContainsKey(Client + "_" + Underlying))
                                    dict_ClientUnderlyingWiseSpanInfo[Client + "_" + Underlying] = _USpanInfo;
                                else
                                    dict_ClientUnderlyingWiseSpanInfo.TryAdd(Client + "_" + Underlying, _USpanInfo);
                            }
                            catch (Exception ee) { _logger.Error(ee, "ReceiveSpanData Loop"); }
                        }

                        //added on 31MAR2021 by Amey
                        var bList_ConcentrationRisk = CollectionHelper.bList_ConcentrationRisk;
                        var dict_ClientWiseSpanInfo = CollectionHelper.dict_ClientWiseSpanInfo;

                        foreach (var ClientID in dict_ClientWiseMargin.Keys)
                        {
                            try
                            {
                                arr_Margin = dict_ClientWiseMargin[ClientID];

                                //added on 16FEB2021 by Amey
                                var _SpanInfo = new ClientSpanInfo()
                                {
                                    ClientID = ClientID,
                                    Span = arr_Margin[0],
                                    Exposure = arr_Margin[1],
                                    MarginUtil = (arr_Margin[0] < 0 ? 0 : arr_Margin[0]) + arr_Margin[1]
                                };

                                //changed from AddOrUpdate on 01MAR2021 by Amey
                                if (dict_ClientWiseSpanInfo.ContainsKey(ClientID))
                                    dict_ClientWiseSpanInfo[ClientID] = _SpanInfo;
                                else
                                    dict_ClientWiseSpanInfo.TryAdd(ClientID, _SpanInfo);

                                //added on 25MAY2021 by Amey
                                if (isConcentrationRiskVisible)
                                {
                                    //removed Invoke because using RTS now. 31MAR2021 by Amey
                                    int SpanGridIndex = bList_ConcentrationRisk.FindIndex(v => v.Client.Equals(ClientID));
                                    if (SpanGridIndex > -1)
                                    {
                                        bList_ConcentrationRisk[SpanGridIndex].Span = DivideByBaseAndRound(_SpanInfo.Span, nameof(CPParent.Span));
                                        bList_ConcentrationRisk[SpanGridIndex].Exposure = DivideByBaseAndRound(_SpanInfo.Exposure, nameof(CPParent.Exposure));
                                        bList_ConcentrationRisk[SpanGridIndex].MarginUtil = DivideByBaseAndRound(_SpanInfo.MarginUtil, nameof(CPParent.MarginUtil));
                                    }
                                    else
                                        bList_ConcentrationRisk.Add(new SpanMargin()
                                        {
                                            Client = ClientID,
                                            Span = DivideByBaseAndRound(_SpanInfo.Span, nameof(CPParent.Span)),
                                            Exposure = DivideByBaseAndRound(_SpanInfo.Exposure, nameof(CPParent.Exposure)),
                                            MarginUtil = DivideByBaseAndRound(_SpanInfo.MarginUtil, nameof(CPParent.MarginUtil))
                                        });
                                }

                                dict_ClientWiseMargin[ClientID] = new double[5] { 0, 0, 0, 0, 0 };
                            }
                            catch (Exception ee) { _logger.Error(ee, "ReceiveSpanData Loop2"); }
                        }

                        //added on 25MAY2021 by Amey
                        if (isConcentrationRiskVisible)
                        {
                            //added on 01APR2021 by Amey
                            uc_ConcentrationRisk.Instance.RefreshChart();
                        }
                    }

                    isSpanMarginUpdating = false;
                }

            }
            catch (Exception ee) { _logger.Error(ee); isSpanMarginUpdating = false; }
        }

        private void UpdateEODSpanMargin(Dictionary<string, double[]> dict_ReceivedSpanData)
        {
            try
            {
                if (!isEODMarginUpdating)
                {
                    isEODMarginUpdating = true;

                    lock (_SpanEODLock)
                    {
                        string Client = "";

                        //added on 26OCT2020 by Amey
                        double Span = 0;
                        double Exposure = 0;

                        double[] arr_Margin;

                        foreach (var EODSpanKey in dict_ReceivedSpanData.Keys)
                        {
                            Client = EODSpanKey.Split('_')[0];

                            //added on 28APR2021 by Amey
                            arr_Margin = dict_ReceivedSpanData[EODSpanKey];

                            //Span = arr_Margin[0] + arr_Margin[2];
                            Span = arr_Margin[0];   //changed on 22AUG2022 by Ninad
                            Exposure = arr_Margin[1];

                            //changed on 16FEB2021 by Amey
                            if (dict_ClientWiseEODMargin.ContainsKey(Client))
                            {
                                dict_ClientWiseEODMargin[Client][0] += Span;
                                dict_ClientWiseEODMargin[Client][1] += Exposure;
                            }
                            else
                                dict_ClientWiseEODMargin.TryAdd(Client, new double[2] { Span, Exposure });
                        }

                        //added on 31MAR2021 by Amey
                        var dict_EODMargin = CollectionHelper.dict_EODMargin;

                        foreach (var ClientID in dict_ClientWiseEODMargin.Keys)
                        {
                            arr_Margin = dict_ClientWiseEODMargin[ClientID];

                            if (dict_EODMargin.ContainsKey(ClientID))
                                dict_EODMargin[ClientID] = (arr_Margin[0] < 0 ? 0 : arr_Margin[0]) + arr_Margin[1];
                            else
                                dict_EODMargin.TryAdd(ClientID, (arr_Margin[0] < 0 ? 0 : arr_Margin[0]) + arr_Margin[1]);

                            dict_ClientWiseEODMargin[ClientID] = new double[2] { 0, 0 };
                        }
                    }


                    isEODMarginUpdating = false;
                }
            }
            catch (Exception ee) { _logger.Error(ee); isEODMarginUpdating = false; }
        }

        private void UpdateEXPSpanMargin(Dictionary<string, double[]> dict_ReceivedSpanData)
        {
            try
            {
                if (!isEXPMarginUpdating)
                {
                    isEXPMarginUpdating = true;

                    lock (_SpanExpLock)
                    {
                        string Client = "";
                        double Span = 0;
                        double Exposure = 0;

                        double[] arr_Margin;

                        foreach (var ExpSpanKey in dict_ReceivedSpanData.Keys)
                        {
                            try
                            {
                                Client = ExpSpanKey.Split('_')[0];

                                //added on 28APR2021 by Amey
                                arr_Margin = dict_ReceivedSpanData[ExpSpanKey];

                                //changed names on 26OCT2020 by Amey
                                //Span = arr_Margin[0] + arr_Margin[2];
                                Span = arr_Margin[0];   //changed on 22AUG2022 by ninad
                                Exposure = arr_Margin[1];

                                //changed on 16FEB2021 by Amey
                                if (dict_ClientWiseExpMargin.ContainsKey(Client))
                                {
                                    dict_ClientWiseExpMargin[Client][0] += Span;
                                    dict_ClientWiseExpMargin[Client][1] += Exposure;
                                }
                                else
                                    dict_ClientWiseExpMargin.TryAdd(Client, new double[2] { Span, Exposure });
                            }
                            catch (Exception ee) { _logger.Error(ee, "ReceiveSpanData Exp Loop"); }
                        }

                        //added on 18FEB2021 by Amey
                        foreach (var ClientID in dict_ClientWiseExpMargin.Keys)
                        {
                            try
                            {
                                arr_Margin = dict_ClientWiseExpMargin[ClientID];

                                var dict_ExpiryMargin = CollectionHelper.dict_ExpiryMargin;
                                //changed from AddOrUpdate on 01MAR2021 by Amey
                                if (dict_ExpiryMargin.ContainsKey(ClientID))
                                    dict_ExpiryMargin[ClientID] = (arr_Margin[0] < 0 ? 0 : arr_Margin[0]) + arr_Margin[1];
                                else
                                    dict_ExpiryMargin.TryAdd(ClientID, (arr_Margin[0] < 0 ? 0 : arr_Margin[0]) + arr_Margin[1]);

                                dict_ClientWiseExpMargin[ClientID] = new double[3] { 0, 0, 0 };
                            }
                            catch (Exception ee) { _logger.Error(ee, "ReceiveSpanData Exp Loop2"); }
                        }
                    }

                    isEXPMarginUpdating = false;
                }
            }
            catch (Exception ee) { _logger.Error(ee); isEXPMarginUpdating = false; }
        }

        private void UpdatePeakMargin(Dictionary<string, double[]> dict_ReceivedSpanData)
        {
            try
            {
                if (!isPeakMarginUpdating)
                {
                    isPeakMarginUpdating = true;

                    lock (_SpanPeakLock)
                    {
                        var dict_PeakMargin = CollectionHelper.dict_PeakMargin;

                        foreach (var spankey in dict_ReceivedSpanData.Keys)
                        {
                            var _Client = spankey.Split('_')[0];
                            var arr_data = dict_ReceivedSpanData[spankey];

                            if (dict_PeakMargin.ContainsKey(_Client))
                                dict_PeakMargin[_Client] = arr_data;
                            else
                                dict_PeakMargin.TryAdd(_Client, arr_data);
                        }
                    }

                    isPeakMarginUpdating = false;
                }
            }
            catch (Exception ee) { _logger.Error(ee); isPeakMarginUpdating = false; }
        }

        private void UpdatePeakMarginCDS(Dictionary<string, double[]> dict_ReceivedSpanData)
        {
            try
            {
                if (!isCDSPeakMarginUpdating)
                {
                    isCDSPeakMarginUpdating = true;

                    lock (_CDSSpanEODLock)
                    {
                        var dict_PeakMargin = CollectionHelper.dict_CDSPeakMargin;

                        foreach (var spankey in dict_ReceivedSpanData.Keys)
                        {
                            var _Client = spankey.Split('_')[0];
                            var arr_data = dict_ReceivedSpanData[spankey];

                            if (dict_PeakMargin.ContainsKey(_Client))
                                dict_PeakMargin[_Client] = arr_data;
                            else
                                dict_PeakMargin.TryAdd(_Client, arr_data);
                        }
                    }

                    isCDSPeakMarginUpdating = false;
                }
            }
            catch (Exception ee) { _logger.Error(ee); isCDSPeakMarginUpdating = false; }
        }

        private void UpdateConMargin(Dictionary<string, double[]> dict_ReceivedSpanData)
        {
            try
            {
                if (!isConMarginUdating)
                {
                    isConMarginUdating = true;

                    lock (_SpanConLock)
                    {
                        string Client = "";
                        string Underlying = "";

                        //added on 26OCT2020 by Amey
                        double Span = 0;
                        double Exposure = 0;
                        //added on 22AUG2022 by Ninad
                        double ExpSpan = 0; 
                        double ExpExposure = 0;

                        double[] arr_Margin;
                        double[] arr_ExpMargin;     //added on 22AUG2022 by Ninad

                        //added on 31MAR2021 by Amey
                        var dict_ClientUnderlyingWiseConsolidatedSpanInfo = CollectionHelper.dict_ClientUnderlyingWiseConsolidatedSpanInfo;

                        foreach (var SpanKey in dict_ReceivedSpanData.Keys)
                        {
                            try
                            {
                                Client = SpanKey.Split('_')[0];
                                Underlying = SpanKey.Split('_')[1];

                                //added on 28APR2021 by Amey
                                arr_Margin = dict_ReceivedSpanData[SpanKey];

                                //changed names on 26OCT2020 by Amey
                                //Span = arr_Margin[0] + arr_Margin[2];
                                Span = arr_Margin[0];   //changed on 22AUG2022 by Ninad
                                Exposure = arr_Margin[1];
                                //added on 22AUG2022 by Ninad
                                ExpSpan = arr_Margin[2];
                                ExpExposure = arr_Margin[3];

                                //changed on 16FEB2021 by Amey
                                if (dict_ClientWiseConsolidatedMargin.ContainsKey(Client))
                                {
                                    dict_ClientWiseConsolidatedMargin[Client][0] += Span;
                                    dict_ClientWiseConsolidatedMargin[Client][1] += Exposure;
                                }
                                else
                                    dict_ClientWiseConsolidatedMargin.TryAdd(Client, new double[2] { Span, Exposure });
                                //added on 22AUG2022 by Ninad
                                if (dict_ClientWiseConsolidatedEXPMargin.ContainsKey(Client))
                                {
                                    dict_ClientWiseConsolidatedEXPMargin[Client][0] += ExpSpan;
                                    dict_ClientWiseConsolidatedEXPMargin[Client][1] += ExpExposure;
                                }
                                else
                                    dict_ClientWiseConsolidatedEXPMargin.TryAdd(Client, new double[2] { ExpSpan, ExpExposure });

                                //changed on 22AUG2022 by Ninad
                                //Span = DivideByBaseAndRound(Span, nameof(CPParent.Span));
                                //Exposure = DivideByBaseAndRound(Exposure, nameof(CPParent.Exposure));

                                //added on 18FEB2021 by Amey
                                var _USpanInfo = new UnderlyingSpanInfo()
                                {
                                    Underlying = Underlying,
                                    Span = Span,
                                    Exposure = Exposure,

                                    //fixed on 27MAY2021 by Amey
                                    MarginUtil = (Span < 0 ? 0 : Span) + Exposure
                                };

                                //changed from AddOrUpdate on 01MAR2021 by Amey
                                if (dict_ClientUnderlyingWiseConsolidatedSpanInfo.ContainsKey(Client + "_" + Underlying))
                                    dict_ClientUnderlyingWiseConsolidatedSpanInfo[Client + "_" + Underlying] = _USpanInfo;
                                else
                                    dict_ClientUnderlyingWiseConsolidatedSpanInfo.TryAdd(Client + "_" + Underlying, _USpanInfo);
                            }
                            catch (Exception ee) { _logger.Error(ee, "ReceiveSpanData Loop"); }
                        }

                        var dict_ClientWiseConsolidatedSpanInfo = CollectionHelper.dict_ClientWiseConsolidatedSpanInfo;
                        var dict_ClientWiseConsolidatedExpMarginInfo = CollectionHelper.dict_ClientWiseConsolidatedExpMarginInfo;   //added on 22AG2022 by Ninad

                        foreach (var ClientID in dict_ClientWiseConsolidatedMargin.Keys)
                        {
                            try
                            {
                                arr_Margin = dict_ClientWiseConsolidatedMargin[ClientID];
                                arr_ExpMargin = dict_ClientWiseConsolidatedEXPMargin[ClientID];     //added on 22AUG2022 by Ninad
                                //added on 16FEB2021 by Amey
                                var _SpanInfo = new ClientSpanInfo()
                                {
                                    ClientID = ClientID,
                                    Span = arr_Margin[0],
                                    Exposure = arr_Margin[1],
                                    MarginUtil = (arr_Margin[0] < 0 ? 0 : arr_Margin[0]) + arr_Margin[1]
                                };

                                var ExpMargin = (arr_ExpMargin[0] < 0 ? 0 : arr_ExpMargin[0]) + arr_ExpMargin[1];   //added on 22AUG2022 by Ninad

                                //changed from AddOrUpdate on 01MAR2021 by Amey
                                if (dict_ClientWiseConsolidatedSpanInfo.ContainsKey(ClientID))
                                    dict_ClientWiseConsolidatedSpanInfo[ClientID] = _SpanInfo;
                                else
                                    dict_ClientWiseConsolidatedSpanInfo.TryAdd(ClientID, _SpanInfo);

                                //added on 22AUG2022 by Ninad
                                if (dict_ClientWiseConsolidatedExpMarginInfo.ContainsKey(ClientID))
                                    dict_ClientWiseConsolidatedExpMarginInfo[ClientID] = ExpMargin;
                                else
                                    dict_ClientWiseConsolidatedExpMarginInfo.TryAdd(ClientID, ExpMargin);
                                
                                dict_ClientWiseConsolidatedMargin[ClientID] = new double[5] { 0, 0, 0, 0, 0 };
                                dict_ClientWiseConsolidatedEXPMargin[ClientID] = new double[2] { 0, 0 };    //added on 22AUG2022 by Ninad
                            }
                            catch (Exception ee) { _logger.Error(ee, "ReceiveSpanData Loop4"); }
                        }

                    }

                    isConMarginUdating = false;
                }
            }
            catch (Exception ee) { _logger.Error(ee); isConMarginUdating = false; }
        }


        private void UpdateCDEODSpanMargin(Dictionary<string, double[]> dict_ReceivedSpanData)
        {
            try
            {
                if (!isCDEODMarginUpdating)
                {
                    isCDEODMarginUpdating = true;
                    lock (_SpanEODLock)
                    {
                        string Client = "";

                        //added on 26OCT2020 by Amey
                        double Span = 0;
                        double Exposure = 0;

                        double[] arr_Margin;

                        foreach (var EODSpanKey in dict_ReceivedSpanData.Keys)
                        {
                            Client = EODSpanKey.Split('_')[0];

                            //added on 28APR2021 by Amey
                            arr_Margin = dict_ReceivedSpanData[EODSpanKey];

                            Span = arr_Margin[0] + arr_Margin[2];
                            Exposure = arr_Margin[1];

                            //changed on 16FEB2021 by Amey
                            if (dict_CDSClientWiseEODMargin.ContainsKey(Client))
                            {
                                dict_CDSClientWiseEODMargin[Client][0] += Span;
                                dict_CDSClientWiseEODMargin[Client][1] += Exposure;
                            }
                            else
                                dict_CDSClientWiseEODMargin.TryAdd(Client, new double[2] { Span, Exposure });
                        }

                        //added on 31MAR2021 by Amey
                        var dict_CDSEODMargin = CollectionHelper.dict_CDSEODMargin;

                        foreach (var ClientID in dict_CDSClientWiseEODMargin.Keys)
                        {
                            arr_Margin = dict_CDSClientWiseEODMargin[ClientID];

                            if (dict_CDSEODMargin.ContainsKey(ClientID))
                                dict_CDSEODMargin[ClientID] = (arr_Margin[0] < 0 ? 0 : arr_Margin[0]) + arr_Margin[1];
                            else
                                dict_CDSEODMargin.TryAdd(ClientID, (arr_Margin[0] < 0 ? 0 : arr_Margin[0]) + arr_Margin[1]);

                            dict_CDSClientWiseEODMargin[ClientID] = new double[2] { 0, 0 };
                        }
                    }

                    isCDEODMarginUpdating = false;
                }
            }
            catch (Exception ee)
            {
                _logger.Error(ee);
                isCDEODMarginUpdating = false;
            }
        }

        private void UpdateCDEXPSpanMargin(Dictionary<string, double[]> dict_ReceivedSpanData)
        {
            try
            {
                if (!isCDEXPMarginUpdating)
                {
                    isCDEXPMarginUpdating = true;
                    lock (_SpanExpLock)
                    {
                        string Client = "";
                        double Span = 0;
                        double Exposure = 0;

                        double[] arr_Margin;

                        foreach (var ExpSpanKey in dict_ReceivedSpanData.Keys)
                        {
                            try
                            {
                                Client = ExpSpanKey.Split('_')[0];

                                //added on 28APR2021 by Amey
                                arr_Margin = dict_ReceivedSpanData[ExpSpanKey];

                                //changed names on 26OCT2020 by Amey
                                Span = arr_Margin[0] + arr_Margin[2];
                                Exposure = arr_Margin[1];

                                //changed on 16FEB2021 by Amey
                                if (dict_CDSClientWiseExpMargin.ContainsKey(Client))
                                {
                                    dict_CDSClientWiseExpMargin[Client][0] += Span;
                                    dict_CDSClientWiseExpMargin[Client][1] += Exposure;
                                }
                                else
                                    dict_CDSClientWiseExpMargin.TryAdd(Client, new double[2] { Span, Exposure });
                            }
                            catch (Exception ee) { _logger.Error(ee, "ReceiveSpanData Exp Loop"); }
                        }

                        //added on 18FEB2021 by Amey
                        foreach (var ClientID in dict_CDSClientWiseExpMargin.Keys)
                        {
                            try
                            {
                                arr_Margin = dict_CDSClientWiseExpMargin[ClientID];

                                var dict_CDSExpiryMargin = CollectionHelper.dict_CDSExpiryMargin;
                                //changed from AddOrUpdate on 01MAR2021 by Amey
                                if (dict_CDSExpiryMargin.ContainsKey(ClientID))
                                    dict_CDSExpiryMargin[ClientID] = (arr_Margin[0] < 0 ? 0 : arr_Margin[0]) + arr_Margin[1];
                                else
                                    dict_CDSExpiryMargin.TryAdd(ClientID, (arr_Margin[0] < 0 ? 0 : arr_Margin[0]) + arr_Margin[1]);

                                dict_CDSClientWiseExpMargin[ClientID] = new double[3] { 0, 0, 0 };
                            }
                            catch (Exception ee) { _logger.Error(ee, "ReceiveSpanData Exp Loop2"); }
                        }
                    }
                    isCDEXPMarginUpdating = false;
                }
            }
            catch (Exception ee) { _logger.Error(ee); isCDEXPMarginUpdating = false; }
        }

        private void UpdateCDSpanMargin(Dictionary<string, double[]> dict_ReceivedSpanData)
        {
            try
            {
                if (!isCDSpanMarginUpdating)
                {
                    isCDSpanMarginUpdating = true;

                    lock (_SpanLock)
                    {
                        string Client = "";
                        string Underlying = "";

                        //added on 26OCT2020 by Amey
                        double Span = 0;
                        double Exposure = 0;

                        double[] arr_Margin;

                        //added on 31MAR2021 by Amey
                        var dict_CDSClientUnderlyingWiseSpanInfo = CollectionHelper.dict_CDSClientUnderlyingWiseSpanInfo;

                        foreach (var SpanKey in dict_ReceivedSpanData.Keys)
                        {
                            try
                            {
                                Client = SpanKey.Split('_')[0];
                                Underlying = SpanKey.Split('_')[1];

                                //added on 28APR2021 by Amey
                                arr_Margin = dict_ReceivedSpanData[SpanKey];

                                //changed names on 26OCT2020 by Amey
                                Span = arr_Margin[0] + arr_Margin[2];
                                Exposure = arr_Margin[1];

                                //changed on 16FEB2021 by Amey
                                if (dict_CDSClientWiseMargin.ContainsKey(Client))
                                {
                                    dict_CDSClientWiseMargin[Client][0] += Span;
                                    dict_CDSClientWiseMargin[Client][1] += Exposure;
                                }
                                else
                                    dict_CDSClientWiseMargin.TryAdd(Client, new double[2] { Span, Exposure });

                                Span = DivideByBaseAndRound(Span, nameof(CPParent.Span));
                                Exposure = DivideByBaseAndRound(Exposure, nameof(CPParent.Exposure));

                                //added on 18FEB2021 by Amey
                                var _USpanInfo = new UnderlyingSpanInfo()
                                {
                                    Underlying = Underlying,
                                    Span = Span,
                                    Exposure = Exposure,

                                    //fixed on 27MAY2021 by Amey
                                    MarginUtil = (Span < 0 ? 0 : Span) + Exposure
                                };

                                //changed from AddOrUpdate on 01MAR2021 by Amey
                                if (dict_CDSClientUnderlyingWiseSpanInfo.ContainsKey(Client + "_" + Underlying))
                                    dict_CDSClientUnderlyingWiseSpanInfo[Client + "_" + Underlying] = _USpanInfo;
                                else
                                    dict_CDSClientUnderlyingWiseSpanInfo.TryAdd(Client + "_" + Underlying, _USpanInfo);
                            }
                            catch (Exception ee) { _logger.Error(ee, "ReceiveSpanData Loop"); }
                        }

                        //added on 31MAR2021 by Amey
                        //var bList_ConcentrationRisk = CollectionHelper.bList_ConcentrationRisk;
                        var dict_CDSClientWiseSpanInfo = CollectionHelper.dict_CDSClientWiseSpanInfo;

                        foreach (var ClientID in dict_CDSClientWiseMargin.Keys)
                        {
                            try
                            {
                                arr_Margin = dict_CDSClientWiseMargin[ClientID];

                                //added on 16FEB2021 by Amey
                                var _SpanInfo = new ClientSpanInfo()
                                {
                                    ClientID = ClientID,
                                    Span = arr_Margin[0],
                                    Exposure = arr_Margin[1],
                                    MarginUtil = (arr_Margin[0] < 0 ? 0 : arr_Margin[0]) + arr_Margin[1]
                                };

                                //changed from AddOrUpdate on 01MAR2021 by Amey
                                if (dict_CDSClientWiseSpanInfo.ContainsKey(ClientID))
                                    dict_CDSClientWiseSpanInfo[ClientID] = _SpanInfo;
                                else
                                    dict_CDSClientWiseSpanInfo.TryAdd(ClientID, _SpanInfo);

                                ////added on 25MAY2021 by Amey
                                //if (isConcentrationRiskVisible)
                                //{
                                //    //removed Invoke because using RTS now. 31MAR2021 by Amey
                                //    int SpanGridIndex = bList_ConcentrationRisk.FindIndex(v => v.Client.Equals(ClientID));
                                //    if (SpanGridIndex > -1)
                                //    {
                                //        bList_ConcentrationRisk[SpanGridIndex].Span = DivideByBaseAndRound(_SpanInfo.Span, nameof(CPParent.Span));
                                //        bList_ConcentrationRisk[SpanGridIndex].Exposure = DivideByBaseAndRound(_SpanInfo.Exposure, nameof(CPParent.Exposure));
                                //        bList_ConcentrationRisk[SpanGridIndex].MarginUtil = DivideByBaseAndRound(_SpanInfo.MarginUtil, nameof(CPParent.MarginUtil));
                                //    }
                                //    else
                                //        bList_ConcentrationRisk.Add(new SpanMargin()
                                //        {
                                //            Client = ClientID,
                                //            Span = DivideByBaseAndRound(_SpanInfo.Span, nameof(CPParent.Span)),
                                //            Exposure = DivideByBaseAndRound(_SpanInfo.Exposure, nameof(CPParent.Exposure)),
                                //            MarginUtil = DivideByBaseAndRound(_SpanInfo.MarginUtil, nameof(CPParent.MarginUtil))
                                //        });
                                //}

                                dict_CDSClientWiseMargin[ClientID] = new double[5] { 0, 0, 0, 0, 0 };
                            }
                            catch (Exception ee) { _logger.Error(ee, "ReceiveSpanData Loop2"); }
                        }

                        ////added on 25MAY2021 by Amey
                        //if (isConcentrationRiskVisible)
                        //{
                        //    //added on 01APR2021 by Amey
                        //    uc_ConcentrationRisk.Instance.RefreshChart();
                        //}
                    }
                    isCDSpanMarginUpdating = false;
                }
            }
            catch (Exception ee) { _logger.Error(ee); isCDSpanMarginUpdating = false; }
        }


        private void ProcessPositionData(Dictionary<string, List<ConsolidatedPositionInfo>> dict_ReceivedClientWiseTrades)
        {
            try
            {
                

                if (!isPositionProcessing)
                {
                    isPositionProcessing = true;

                    ReadOGFile();

                    if (IsHandleCreated)
                    {
                        //added on 14JAN2021 by Amey
                        if (isScenarioComputeClicked)
                        {
                            isScenarioComputeClicked = false;

                            //added on 30DEC2020 by Amey
                            uc_ScenarioAnalysis.Instance.ClientPositionsReceived(dict_ReceivedClientWiseTrades);
                        }

                        if (RequestTradesInAllPositionsTab)
                        {
                            RequestTradesInAllPositionsTab = false;
                            
                            uc_AllPositions.Instance.AllPositionsReceived(dict_ReceivedClientWiseTrades);
                        }

                        //added on 25MAY2021 by Amey
                        // Second flag added by Snehadri on 11AUG2021
                        if (isClientPortfolioVisible || ComputeforRule.StartCompute)
                        {
                            

                            //Added by Akshay on 19-07-2021 for NPL Values
                            if (NPL_Flag)
                            {
                                NPL_Flag = false;
                                foreach (var _NPLClientID in CollectionHelper.dict_NPLValues.Keys)
                                {
                                    var _ClientID = _NPLClientID.Split('^')[0];
                                    var _Underlying = _NPLClientID.Split('^')[1];
                                    var _NPL = CollectionHelper.dict_NPLValues[_NPLClientID];

                                    if (!dict_UnderlyingLevelNPL.ContainsKey(_Underlying))
                                        dict_UnderlyingLevelNPL.TryAdd(_Underlying, _NPL);
                                    else
                                        dict_UnderlyingLevelNPL[_Underlying] += _NPL;

                                    CollectionHelper.dict_UniqueClients.TryAdd(_ClientID, true);

                                    if (!dict_ClientCompute.ContainsKey(_ClientID))
                                    {
                                        if (CollectionHelper.dict_ClientInfo.TryGetValue(_ClientID, out ClientInfo _ClientInfo))
                                        {
                                            var _CPParent = new CPParent
                                            {
                                                ClientID = _ClientID,
                                                Name = _ClientInfo.Name,
                                                Ledger = _ClientInfo.ELM,
                                                Adhoc = _ClientInfo.AdHoc,
                                                Zone = _ClientInfo.Zone,
                                                Branch = _ClientInfo.Branch,
                                                Family = _ClientInfo.Family,
                                                Product = _ClientInfo.Product,
                                                bList_Underlying = new BindingList<CPUnderlying>()
                                            };

                                            if (CollectionHelper.dict_LimitInfo.TryGetValue(_ClientID, out LimitInfo limitInfo))
                                            {
                                                _CPParent.MTMLimit = DivideByBaseAndRound(limitInfo.MTMLimit, nameof(CPParent.MTMLimit));
                                                _CPParent.VARLimit = DivideByBaseAndRound(limitInfo.VARLimit, nameof(CPParent.VARLimit));
                                                _CPParent.MarginLimit = DivideByBaseAndRound(limitInfo.MarginLimit, nameof(CPParent.MarginLimit));
                                                _CPParent.BankniftyExpoLimit = DivideByBaseAndRound(limitInfo.BankniftyExpoLimit, nameof(CPParent.BankniftyExpoLimit));
                                                _CPParent.NiftyExpoLimit = DivideByBaseAndRound(limitInfo.NiftyExpoLimit, nameof(CPParent.NiftyExpoLimit));
                                            }

                                            if (_Underlying != "ALL")
                                            {
                                                var _CPUnderlying = new CPUnderlying();
                                                _CPUnderlying.bList_Positions = new BindingList<CPPositions>();
                                                _CPUnderlying.ClientID = _ClientID;
                                                _CPUnderlying.Underlying = _Underlying;
                                                _CPUnderlying.NPL = DivideByBaseAndRound(_NPL, nameof(_CPParent.MTM));
                                                _CPParent.bList_Underlying.Add(_CPUnderlying);
                                            }
                                            else
                                                _CPParent.NPL = DivideByBaseAndRound(_NPL, nameof(_CPParent.MTM));

                                            CollectionHelper.bList_ClientPortfolio.Add(_CPParent);

                                            var _Compute = new nCompute(_ClientID, _CPParent);

                                            dict_IsCPExpanded.TryAdd(_ClientID, false);
                                            dict_ExpandedUnderlyings.TryAdd(_ClientID, new List<string>());
                                            dict_IsVaRDistributionShown.TryAdd(_ClientID, false);

                                            dict_ClientCompute.TryAdd(_ClientID, _Compute);
                                        }
                                    }
                                    else
                                    {
                                        var _CPParent = CollectionHelper.bList_ClientPortfolio.Where(v => v.ClientID == _ClientID).FirstOrDefault();

                                        if (_Underlying != "ALL")
                                        {
                                            var _CPUnderlying = new CPUnderlying();
                                            _CPUnderlying.bList_Positions = new BindingList<CPPositions>();
                                            _CPUnderlying.ClientID = _ClientID;
                                            _CPUnderlying.Underlying = _Underlying;
                                            _CPUnderlying.NPL = DivideByBaseAndRound(_NPL, nameof(_CPParent.MTM));
                                            _CPParent.bList_Underlying.Add(_CPUnderlying);
                                        }
                                        else
                                            _CPParent.NPL = DivideByBaseAndRound(_NPL, nameof(_CPParent.MTM));
                                    }

                                    if (!dict_UnderlyingCompute.ContainsKey(_Underlying))
                                    {
                                        if (_Underlying == "ALL") { continue; }

                                        var _UCParent = new UCParent()
                                        {
                                            Underlying = _Underlying,
                                            bList_Clients = new BindingList<UCClient>()
                                        };

                                        if (_Underlying != "ALL")
                                        {
                                            if (CollectionHelper.dict_ClientInfo.TryGetValue(_ClientID, out ClientInfo _ClientInfo))
                                            {
                                                var _UCClient = new UCClient();
                                                _UCClient.ClientID = _ClientID;
                                                _UCClient.Name = _ClientInfo.Name;
                                                _UCClient.Ledger = _ClientInfo.ELM;
                                                _UCClient.Adhoc = _ClientInfo.AdHoc;
                                                _UCClient.Zone = _ClientInfo.Zone;
                                                _UCClient.Branch = _ClientInfo.Branch;
                                                _UCClient.Family = _ClientInfo.Family;
                                                _UCClient.Product = _ClientInfo.Product;
                                                _UCClient.NPL = DivideByBaseAndRound(_NPL, nameof(_UCParent.MTM));
                                                _UCParent.NPL += _UCClient.NPL;
                                                _UCParent.bList_Clients.Add(_UCClient);

                                            }
                                        }

                                        CollectionHelper.bList_UnderlyingClients.Add(_UCParent);

                                        var _Compute = new nCompute(_Underlying, _UCParent);

                                        dict_IsUCExpanded.TryAdd(_Underlying, false);
                                        dict_UnderlyingCompute.TryAdd(_Underlying, _Compute);
                                    }
                                    else
                                    {
                                        if (_Underlying == "ALL") { continue; }

                                        var _UCParent = CollectionHelper.bList_UnderlyingClients.Where(v => v.Underlying == _Underlying).FirstOrDefault();

                                        if (_Underlying != "ALL")
                                        {
                                            if (CollectionHelper.dict_ClientInfo.TryGetValue(_ClientID, out ClientInfo _ClientInfo))
                                            {
                                                var _UCClient = new UCClient();
                                                _UCClient.ClientID = _ClientID;
                                                _UCClient.Name = _ClientInfo.Name;
                                                _UCClient.Ledger = _ClientInfo.ELM;
                                                _UCClient.Adhoc = _ClientInfo.AdHoc;
                                                _UCClient.Zone = _ClientInfo.Zone;
                                                _UCClient.Branch = _ClientInfo.Branch;
                                                _UCClient.Family = _ClientInfo.Family;
                                                _UCClient.Product = _ClientInfo.Product;
                                                _UCClient.NPL = DivideByBaseAndRound(_NPL, nameof(_UCParent.MTM));
                                                _UCParent.NPL += _UCClient.NPL;
                                                _UCParent.bList_Clients.Add(_UCClient);
                                            }
                                        }
                                    }

                                }
                            }
                            // Added by Snehadri on 24JUN2021   
                            uc_ClientPortfolio.Instance.UpdatePositionDictionary(dict_ReceivedClientWiseTrades);


                            //changed to ForEach on 12APR2021 by Amey
                            //Parallel.ForEach(dict_Received, KVP =>
                            if (dict_ReceivedClientWiseTrades != null)
                            {
                                foreach (var KVP in dict_ReceivedClientWiseTrades)
                                {
                                    if (CollectionHelper.dict_ClientInfo.TryGetValue(KVP.Key, out ClientInfo _ClientInfo))
                                    {
                                        string ClientID = KVP.Key;

                                        if (!dict_ClientCompute.ContainsKey(ClientID))
                                        {
                                            var _CPParent = new CPParent
                                            {
                                                ClientID = ClientID,
                                                Name = _ClientInfo.Name,
                                                Ledger = _ClientInfo.ELM,
                                                Adhoc = _ClientInfo.AdHoc,
                                                Zone = _ClientInfo.Zone,
                                                Branch = _ClientInfo.Branch,
                                                Family = _ClientInfo.Family,
                                                Product = _ClientInfo.Product
                                            };

                                            if (CollectionHelper.dict_LimitInfo.TryGetValue( ClientID, out LimitInfo limitInfo))
                                            {
                                                _CPParent.MTMLimit = DivideByBaseAndRound(limitInfo.MTMLimit, nameof(CPParent.MTMLimit));
                                                _CPParent.VARLimit = DivideByBaseAndRound(limitInfo.VARLimit, nameof(CPParent.VARLimit));
                                                _CPParent.MarginLimit = DivideByBaseAndRound(limitInfo.MarginLimit, nameof(CPParent.MarginLimit));
                                                _CPParent.BankniftyExpoLimit = DivideByBaseAndRound(limitInfo.BankniftyExpoLimit, nameof(CPParent.BankniftyExpoLimit));
                                                _CPParent.NiftyExpoLimit = DivideByBaseAndRound(limitInfo.NiftyExpoLimit, nameof(CPParent.NiftyExpoLimit));
                                            }

                                            lock (CollectionHelper._CPLock)
                                            {
                                                //changed on 19FEB2021 by Amey
                                                _CPParent.bList_Underlying = new BindingList<CPUnderlying>();

                                                CollectionHelper.bList_ClientPortfolio.Add(_CPParent);
                                            }

                                            var _Compute = new nCompute(ClientID, _CPParent);

                                            dict_IsCPExpanded.TryAdd(ClientID, false);
                                            dict_ExpandedUnderlyings.TryAdd(ClientID, new List<string>());
                                            dict_IsVaRDistributionShown.TryAdd(ClientID, false);

                                            if (!_Compute.isComputing)
                                                _Compute.AddToClientQueue(new List<ConsolidatedPositionInfo>(KVP.Value), true, new List<string>(), true);

                                            dict_ClientCompute.TryAdd(ClientID, _Compute);
                                        }
                                        else
                                        {
                                            if (!dict_ClientCompute[ClientID].isComputing)
                                            {
                                                dict_ClientCompute[ClientID].AddToClientQueue(new List<ConsolidatedPositionInfo>(KVP.Value), dict_IsCPExpanded[ClientID],
                                                    dict_ExpandedUnderlyings[ClientID], dict_IsVaRDistributionShown[ClientID]);
                                            }
                                        }
                                    }
                                }//);
                            }

                            //  Added by Snehadri on 11AUG2021
                            ComputeforRule.StartCompute = false;
                        }
                        if (isUnderlyingViewVisible)
                        {
                            var dict_UPositions = dict_ReceivedClientWiseTrades.Values.SelectMany(v => v).GroupBy(v => v.Underlying).ToDictionary(k => k.Key, v => v.ToList());
                            
                            //changed to ForEach on 12APR2021 by Amey
                            //Parallel.ForEach(dict_Received, KVP =>
                            foreach (var KVP in dict_UPositions)
                            {
                                string _Underlying = KVP.Key;

                                //var list_Clients = KVP.Value.Select(v => v.Username).Distinct();

                                if (!dict_UnderlyingCompute.ContainsKey(_Underlying))
                                {
                                    var _UCParent = new UCParent()
                                    {
                                        Underlying = _Underlying
                                    };

                                    lock (CollectionHelper._CULock)
                                    {
                                        //changed on 19FEB2021 by Amey
                                        _UCParent.bList_Clients = new BindingList<UCClient>();

                                        CollectionHelper.bList_UnderlyingClients.Add(_UCParent);
                                    }

                                    var _Compute = new nCompute(_Underlying, _UCParent);

                                    dict_IsUCExpanded.TryAdd(_Underlying, false);

                                    if (!_Compute.isComputing)
                                        _Compute.AddToUnderlyingQueue(new List<ConsolidatedPositionInfo>(KVP.Value), true, dict_UnderlyingLevelNPL);

                                    dict_UnderlyingCompute.TryAdd(_Underlying, _Compute);
                                }
                                else
                                {
                                    if (!dict_UnderlyingCompute[_Underlying].isComputing)
                                        dict_UnderlyingCompute[_Underlying].AddToUnderlyingQueue(new List<ConsolidatedPositionInfo>(KVP.Value), dict_IsUCExpanded[_Underlying], dict_UnderlyingLevelNPL);
                                }
                            }//);                            
                        }

                        //Added by Akshay on 28-07-2021 for ClientWindow
                        if (isClientWindowViewVisible)
                        {
                            if (dict_ReceivedClientWiseTrades.TryGetValue(_SelectedClientID, out List<ConsolidatedPositionInfo> list_ClientWisePositions))
                            {

                                List<ConsolidatedPositionInfo> list_Positions = new List<ConsolidatedPositionInfo>();
                                if (_SelectedExpiry != "ALL")
                                {
                                    var _SelectedExpiryDate = CommonFunctions.ConvertToUnixTimestamp(Convert.ToDateTime(_SelectedExpiry));
                                    list_Positions = list_ClientWisePositions.Where(row => row.Underlying == _SelectedUnderlying && row.Expiry == _SelectedExpiryDate).ToList();
                                }
                                else
                                    list_Positions = list_ClientWisePositions.Where(row => row.Underlying == _SelectedUnderlying).ToList();


                                if (list_Positions.Any())
                                {
                                    if (!dict_ClientComputeCW.ContainsKey(_SelectedClientID))
                                    {
                                        var _Compute = new nCompute();

                                        if (!_Compute.isComputing)
                                            _Compute.AddToClientWindowQueue(list_Positions);
                                        dict_ClientComputeCW.TryAdd(_SelectedClientID, _Compute);
                                    }
                                    else
                                    {
                                        if (!dict_ClientComputeCW[_SelectedClientID].isComputing)
                                            dict_ClientComputeCW[_SelectedClientID].AddToClientWindowQueue(list_Positions);
                                    }
                                }
                                else
                                {
                                    CollectionHelper.bList_ClientWindow.Clear();
                                    CollectionHelper.bList_ClientWindowOptions.Clear();
                                    CollectionHelper.bList_ClientWindowFutures.Clear();
                                    CollectionHelper.bList_ClientWindowGreeks.Clear();
                                }

                            }
                            //else
                            //CollectionHelper.bList_ClientWindow.Clear();
                        }

                        if (!isClientPortfolioVisible)
                        {
                            foreach (var KVP in dict_ReceivedClientWiseTrades)
                            {
                                string _ClientID = KVP.Key;
                                var list_Positions = KVP.Value;

                                foreach (var _PositionInfo in list_Positions)
                                {
                                    string Underlying = _PositionInfo.Underlying;
                                    DateTime Expiry = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry);

                                    if (!CollectionHelper.dict_ComboUniverse.ContainsKey(_ClientID))
                                    {
                                        CollectionHelper.dict_ComboUniverse.TryAdd(_ClientID, new ConcurrentDictionary<string, ConcurrentDictionary<DateTime, string>>());
                                        CollectionHelper.dict_ComboUniverse[_ClientID].TryAdd(Underlying, new ConcurrentDictionary<DateTime, string>());
                                        CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].TryAdd(Expiry, "Done");
                                    }
                                    else if (!CollectionHelper.dict_ComboUniverse[_ClientID].ContainsKey(Underlying))
                                    {
                                        CollectionHelper.dict_ComboUniverse[_ClientID].TryAdd(Underlying, new ConcurrentDictionary<DateTime, string>());
                                        CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].TryAdd(Expiry, "Done");
                                    }
                                    else if (!CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].ContainsKey(Expiry))
                                    {
                                        CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].TryAdd(Expiry, "Done");
                                    }

                                    //if (!CollectionHelper.dict_ComboUniverse.ContainsKey(_ClientID))
                                    //    CollectionHelper.dict_ComboUniverse.TryAdd(_ClientID, new ConcurrentDictionary<string, ConcurrentDictionary<DateTime, string>>());

                                    //if (!CollectionHelper.dict_ComboUniverse[_ClientID].ContainsKey(Underlying))
                                    //    CollectionHelper.dict_ComboUniverse[_ClientID].TryAdd(Underlying, new ConcurrentDictionary<DateTime, string>());

                                    //if (!CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].ContainsKey(Expiry))
                                    //    CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].TryAdd(Expiry, "Done");

                                }



                            }
                        }

                        //Added by Akshay on 13-06-2022 for Delivery Tab

                        if (isDeliveryReportVisible)
                        {
                            if (CollectionHelper.IsDeliveryRefresh)
                            {
                                foreach (var KVP in dict_ReceivedClientWiseTrades)
                                {
                                    string ClientID = KVP.Key;
                                    var _Compute = new nCompute(ClientID, true);
                                    _Compute.AddToDRClientQueue(new List<ConsolidatedPositionInfo>(KVP.Value));
                                }
                                CollectionHelper.IsDeliveryRefresh = false;
                            }
                            ComputeforRule.StartCompute = false;

                        }

                        Invoke((MethodInvoker)(() =>
                        {
                            lbl_NoOfClientsVal.Text = CollectionHelper.dict_UniqueClients.Count.ToString();
                            lbl_NoOfScripsVal.Text = CollectionHelper.dict_UniqueTokens.Count.ToString();
                        }));
                    }

                    //_logger.Debug("Processed : " + DateTime.Now.ToString("ss:fff"));

                    //added on 1JUN2021 by Amey
                    dict_ClientWisePositions = dict_ReceivedClientWiseTrades;

                    isPositionProcessing = false;
                }
            }
            catch (Exception ee) { _logger.Error(ee); isPositionProcessing = false; }
        }

        private void ReadOGFile()
        {
            try
            {
                var arr_Lines = File.ReadAllLines(Application.StartupPath + "/OGRange.csv");

                var dict_OG = CollectionHelper.dict_OG;
                for (int i = 1; i < arr_Lines.Length; i++)
                {
                    string[] fields = arr_Lines[i].Split(',');

                    string Underlying = fields[0].ToUpper();

                    int OGFrom = Convert.ToInt32(fields[1].Trim());
                    int OGTo = Convert.ToInt32(fields[2].Trim());
                   
                    //added on 2JUN2021 by Amey
                    OGFrom = OGFrom < -30 ? -30 : OGFrom;
                    OGTo = OGTo > 30 ? 30 : OGTo;

                    //fixed on 1JUN2021 by Amey
                    OGTo = OGTo < OGFrom ? (OGFrom + 10) : OGTo;

                    if (dict_OG.ContainsKey(Underlying))
                    {
                        dict_OG[Underlying].OGFrom = OGFrom;
                        dict_OG[Underlying].OGTo = OGTo;
                    }
                    else
                        dict_OG.Add(Underlying, new OGInfo() { OGFrom = OGFrom, OGTo = OGTo });
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        /// <summary>
        /// Call after Form is shown.
        /// </summary>
        private void InitialiseRequiredEvents()
        {
            try
            {
                uc_ClientPortfolio.Instance.eve_IsClientExpanded += CP_eve_IsClientExpanded;
                uc_ClientPortfolio.Instance.eve_IsUnderlyingExpanded += CP_eve_IsUnderlyingExpanded;
                uc_ClientPortfolio.Instance.eve_IsVaRDistributionShown += CP_eve_IsVaRDistributionShown;


                uc_DeliveryReport.Instance.eve_IsDRClientExpanded += DR_eve_IsClientExpanded;
                uc_DeliveryReport.Instance.eve_IsDRUnderlyingExpanded += DR_eve_IsUnderlyingExpanded;

                uc_ClientPortfolio.Instance.eve_IsClientWindowShown += CP_eve_IsClientWindowShown;    //Added by Akshay on 28-07-2021 for Client Window
                uc_ClientPortfolio.Instance.eve_ClientWindowDataSend += CP_eve_ClientWindowData;    //Added by Akshay on 28-07-2021 for Client Window

                uc_ScenarioAnalysis.Instance.eve_ComputeScenarioClicked += Instance_eve_ComputeScenarioClicked;

                uc_AllPositions.Instance.eve_GetAllTradesClicked += Instance_eve_GetAllTradesClicked;

                //added on 1JUN2021 by Amey
                uc_UnderlyingClients.Instance.eve_IsUnderlyingExpanded += UC_eve_IsUnderlyingExpanded;
                uc_UnderlyingClients.Instance.eve_IsClientDoubleClicked += UC_eve_IsClientDoubleClicked;

                //added on 3JUN2021 by Amey
                uc_ConcentrationRisk.Instance.eve_IsClientDoubleClicked += UC_eve_ConecntrationRisk_IsClientDoubleClicked;

                var CONInfo = ConfigurationManager.AppSettings;
                var ip_EngineServer = IPAddress.Parse(CONInfo["ENGINE-SERVER-IP"].ToString());
                var _EngineServerTradePORT = Convert.ToInt32(CONInfo["ENGINE-SERVER-TRADE-PORT"]);
                var _EngineServerSpanPORT = Convert.ToInt32(CONInfo["ENGINE-SERVER-SPAN-PORT"]);

                EngineDataConnector _EngineDataClient = new EngineDataConnector();
                _EngineDataClient.eve_EngineStatus += _EngineDataClient_eve_EngineStatus;
                _EngineDataClient.eve_PositionsReceived += _EngineDataClient_eve_PositionsReceived;
                _EngineDataClient.eve_SpanData += _EngineDataClient_eve_SpanData;
                _EngineDataClient.ConnectToEngineData(ip_EngineServer, _EngineServerTradePORT, Username);
                _EngineDataClient.ConnectToEngineSpan(ip_EngineServer, _EngineServerSpanPORT, Username);

                timer_AutosavePositions.Interval = Convert.ToInt32(CONInfo["AUTOSAVE-INTERVAL"]) * 60 * 1000;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void CP_eve_IsClientWindowShown(bool flag_ClientWindow)
        {
            isClientWindowViewVisible = flag_ClientWindow;
        }

        private void CP_eve_ClientWindowData(string _ClientID, string _Underlying, string _Expiry)
        {
            _SelectedClientID = _ClientID;
            _SelectedUnderlying = _Underlying;
            _SelectedExpiry = _Expiry;
            CollectionHelper.bList_ClientWindow.Clear();
            CollectionHelper.bList_ClientWindowGreeks.Clear();
            CollectionHelper.bList_ClientWindowOptions.Clear();
            CollectionHelper.bList_ClientWindowFutures.Clear();
            CollectionHelper.IsClear = true;
        }

        #region UserControl Events

        private void CP_eve_IsVaRDistributionShown(bool IsShown, string ID = "")
        {
            dict_IsVaRDistributionShown[ID] = IsShown;
        }

        /// <summary>
        /// Will be called when Client row is expanded.
        /// </summary>
        /// <param name="ID">ClientID</param>
        /// <param name="IsExpanded">State</param>
        private void CP_eve_IsClientExpanded(bool IsExpanded, string ID = "")
        {
            dict_IsCPExpanded[ID] = IsExpanded;

            //added on 1JUN2021 by Amey. To avoid not filling expanded grid after Engine stops.
            if (IsExpanded)
                ProcessPositionData(new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_ClientWisePositions));
        }

        /// <summary>
        /// Will be called when Underlyng row is expanded.
        /// </summary>
        /// <param name="ID">ClientID_Underlying</param>
        /// <param name="IsExpanded">State</param>
        private void CP_eve_IsUnderlyingExpanded(bool IsExpanded, string ID = "")
        {
            var arr_Fields = ID.Split('_');

            if (IsExpanded)
            {
                dict_ExpandedUnderlyings[arr_Fields[0]].Add(arr_Fields[1]);

                //added on 1JUN2021 by Amey. To avoid not filling expanded grid after Engine stops.
                ProcessPositionData(new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_ClientWisePositions));
            }
            else
                dict_ExpandedUnderlyings[arr_Fields[0]].Remove(arr_Fields[1]);
        }


        /// <summary>
        /// Will be called when Client row is expanded.
        /// </summary>
        /// <param name="ID">ClientID</param>
        /// <param name="IsExpanded">State</param>
        private void DR_eve_IsClientExpanded(bool IsExpanded, string ID = "")
        {
            dict_IsDRCPExpanded[ID] = IsExpanded;

            //added on 1JUN2021 by Amey. To avoid not filling expanded grid after Engine stops.
            if (IsExpanded)
                ProcessPositionData(new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_ClientWisePositions));
        }


        /// <summary>
        /// Will be called when Underlyng row is expanded.
        /// </summary>
        /// <param name="ID">ClientID_Underlying</param>
        /// <param name="IsExpanded">State</param>
        private void DR_eve_IsUnderlyingExpanded(bool IsExpanded, string ID = "")
        {
            var arr_Fields = ID.Split('_');

            if (IsExpanded)
            {
                dict_DRExpandedUnderlyings[arr_Fields[0]].Add(arr_Fields[1]);

                //added on 1JUN2021 by Amey. To avoid not filling expanded grid after Engine stops.
                ProcessPositionData(new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_ClientWisePositions));
            }
            else
                dict_DRExpandedUnderlyings[arr_Fields[0]].Remove(arr_Fields[1]);
        }

        /// <summary>
        /// Will be called when Underlyng row is expanded.
        /// </summary>
        /// <param name="ID">Underlying</param>
        /// <param name="IsExpanded">State</param>
        private void UC_eve_IsUnderlyingExpanded(bool IsExpanded, string ID = "")
        {
            dict_IsUCExpanded[ID] = IsExpanded;
        }

        /// <summary>
        /// Will be called when double clicked on Client row from underlying View tab.
        /// </summary>
        /// <param name="ID">ClientID^Underlying</param>
        /// <param name="IsClicked">State</param>
        private void UC_eve_IsClientDoubleClicked(bool IsClicked, string ID = "")
        {
            HighlightAndShowSelected(accEle_ClientPortfolio, uc_ClientPortfolio.Instance);
            var arr_Values = ID.Split('^');
            uc_ClientPortfolio.Instance.SetFocusedRow(arr_Values[0], arr_Values[1]);

            //added on 1JUN2021 by Amey. To avoid not filling expanded grid after Engine stops.
            ProcessPositionData(new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_ClientWisePositions));
        }

        /// <summary>
        /// Will be called when double clicked on Client row from Concentration Risk tab.
        /// </summary>
        /// <param name="ID">ClientID</param>
        /// <param name="IsClicked">State</param>
        private void UC_eve_ConecntrationRisk_IsClientDoubleClicked(bool IsClicked, string ID = "")
        {
            HighlightAndShowSelected(accEle_ClientPortfolio, uc_ClientPortfolio.Instance);
            uc_ClientPortfolio.Instance.SetFocusedRow(ID);

            //added on 1JUN2021 by Amey. To avoid not filling expanded grid after Engine stops.
            ProcessPositionData(new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_ClientWisePositions));
        }

        private void Instance_eve_ComputeScenarioClicked(bool isCliked, string ID = "")
        {
            isScenarioComputeClicked = isCliked;
            
            //added on 1JUN2021 by Amey. To avoid not filling expanded grid after Engine stops.
            ProcessPositionData(new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_ClientWisePositions));
        }

        private void Instance_eve_GetAllTradesClicked(bool flag_State, string ID = "")
        {
            RequestTradesInAllPositionsTab = true;

            //added on 1JUN2021 by Amey. To avoid not filling expanded grid after Engine stops.
            ProcessPositionData(new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_ClientWisePositions));
        }

        #endregion

        private void UnsubscribeUserPreferenceChangedEvent()
        {
            try
            {
                var handlers = typeof(SystemEvents).GetField("_handlers", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                var handlersValues = handlers.GetType().GetProperty("Values").GetValue(handlers);
                foreach (var invokeInfos in (handlersValues as IEnumerable).OfType<object>().ToArray())
                    foreach (var invokeInfo in (invokeInfos as IEnumerable).OfType<object>().ToArray())
                    {
                        var syncContext = invokeInfo.GetType().GetField("_syncContext", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(invokeInfo);
                        if (syncContext == null)
                            throw new Exception("syncContext missing");
                        if (!(syncContext is WindowsFormsSynchronizationContext))
                            continue;
                        var threadRef = (WeakReference)syncContext.GetType().GetField("destinationThreadRef", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(syncContext);
                        if (!threadRef.IsAlive)
                            continue;
                        var thread = (System.Threading.Thread)threadRef.Target;
                        if (thread.ManagedThreadId == 1)
                            continue;  // Change here if you have more valid UI threads to ignore
                        var dlg = (Delegate)invokeInfo.GetType().GetField("_delegate", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(invokeInfo);
                        var handler = (UserPreferenceChangedEventHandler)Delegate.CreateDelegate(typeof(UserPreferenceChangedEventHandler), dlg.Target, dlg.Method.Name);
                        SystemEvents.UserPreferenceChanged -= handler;
                    }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        /// <summary>
        /// Will be called after after each element click.
        /// </summary>
        /// <param name="accEle_Clicked">Accordian Element</param>
        private void HighlightAndShowSelected(AccordionControlElement accEle_Clicked, Control uc_Clicked, bool ShowHighlightColor = true)
        {
            if (!MainPanel_Fill.Controls.Contains(uc_Clicked))
            {
                MainPanel_Fill.Controls.Add(uc_Clicked);
                uc_Clicked.Dock = DockStyle.Fill;
            }
            uc_Clicked.BringToFront();

            if (ShowHighlightColor)
            {
                acc_Previous.Appearance.Normal.BackColor = Color.Transparent;
                acc_Previous.Appearance.Hovered.BackColor = Color.Transparent;

                accEle_Clicked.Appearance.Normal.BackColor = ColorTranslator.FromHtml("#78909C");  //LightGray             // ("#3498db");
                accEle_Clicked.Appearance.Hovered.BackColor = ColorTranslator.FromHtml("#78909C"); //LightGray 
            }

            acc_Previous = accEle_Clicked;
            isClientPortfolioVisible = false;
            isConcentrationRiskVisible = false;
            isUnderlyingViewVisible = false;
            isDeliveryReportVisible = false;

            //changed on 1JUN2021 by Amey
            //added on 25MAY2021 by Amey
            if (uc_Clicked.Name == nameof(uc_ClientPortfolio))
                isClientPortfolioVisible = true;
            else if(uc_Clicked.Name == nameof(uc_UnderlyingClients))
                isUnderlyingViewVisible = true;
            else if (uc_Clicked.Name == nameof(uc_ConcentrationRisk))
                isConcentrationRiskVisible = true;
            else if (uc_Clicked.Name == nameof(uc_DeliveryReport))
                isDeliveryReportVisible = true;

            if (isClientPortfolioVisible || isUnderlyingViewVisible || isDeliveryReportVisible)
                ProcessPositionData(new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_ClientWisePositions));
        }

        // Added by Snehadri on 21OCT2021 for Autosave Positon
        private void AutoSavePositions()
        {
            isAutoSaving = true;

            // ClientPortfolio
            try
            {
                string autosave_ClientPortfolio = Application.StartupPath + "\\" + "Report\\" + "ClientPortfolio.csv";
                var blist_CPParent = new BindingList<CPParent>(CollectionHelper.bList_ClientPortfolio);
                if (blist_CPParent.Any())
                {
                    var sb_CP = new StringBuilder();
                    var Properties = blist_CPParent[0].GetType().GetProperties();
                    sb_CP.Append(string.Join(",", Properties.Select(p => p.Name)));
                    sb_CP.Append("\n");

                    foreach (var Row in blist_CPParent)
                    {
                        foreach (var details in Properties)
                        {
                            if (!details.Name.StartsWith("bList_"))
                                sb_CP.Append(details.GetValue(Row) + ",");
                        }
                        sb_CP.Remove(sb_CP.ToString().LastIndexOf(','), 1);
                        sb_CP.Append("\n");
                    }
                    File.WriteAllText(autosave_ClientPortfolio, sb_CP.ToString());
                }
            }
            catch (Exception ee) { _logger.Error(ee, "AutoSave- ClientPortfolio :-"); }

            // All Position
            try
            {
                Dictionary<string, List<ConsolidatedPositionInfo>> dict_AllPositions = new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_ClientWisePositions);

                if (dict_AllPositions.Count > 0)
                {
                    var list_AllPositions = new List<NetPositionInfo>();
                    //var sb_AllPosition = new StringBuilder();
                    //var Properties = new NetPositionInfo().GetType().GetProperties();
                    //sb_AllPosition.Append(string.Join(",", Properties.Select(p => p.Name)));
                    //sb_AllPosition.Append("\n");

                    foreach (var _position in dict_AllPositions.Values.SelectMany(v => v))
                    {
                        if (CollectionHelper.dict_ClientInfo.TryGetValue(_position.Username, out ClientInfo _ClientInfo))
                        {
                            var _NetPosInfo = CopyPropertiesFrom(_position, new NetPositionInfo());

                            _NetPosInfo.Name = _ClientInfo.Name;
                            _NetPosInfo.Zone = _ClientInfo.Zone;
                            _NetPosInfo.Branch = _ClientInfo.Branch;
                            _NetPosInfo.Family = _ClientInfo.Family;
                            _NetPosInfo.Product = _ClientInfo.Product;
                            _NetPosInfo.ExpiryDate = CommonFunctions.ConvertFromUnixTimestamp(_position.Expiry);

                            var temp_exp = _NetPosInfo.ExpiryDate.ToString("ddMMyyyy");
                            if (_NetPosInfo.ScripType == en_ScripType.CE || _NetPosInfo.ScripType == en_ScripType.PE)
                            {
                                if (_NetPosInfo.ScripType == en_ScripType.CE)
                                {
                                    if (_NetPosInfo.StrikePrice > _NetPosInfo.SpotPrice)
                                        _NetPosInfo.Moneyness = "OTM";
                                    else
                                        _NetPosInfo.Moneyness = "ITM";

                                    _NetPosInfo.ITM_OTM_Percentage = Math.Round((Math.Abs(_NetPosInfo.StrikePrice - _NetPosInfo.SpotPrice) * 100) / _NetPosInfo.StrikePrice, 2);

                                    if (temp_exp == DateTime.Now.ToString("ddMMyyyy"))
                                    {
                                        _NetPosInfo.IntrinsicMTM = (_NetPosInfo.SpotPrice - _NetPosInfo.StrikePrice) * _NetPosInfo.NetPosition;
                                    }
                                }
                                else if (_NetPosInfo.ScripType == en_ScripType.PE)
                                {
                                    if (_NetPosInfo.StrikePrice > _NetPosInfo.SpotPrice)
                                        _NetPosInfo.Moneyness = "ITM";
                                    else
                                        _NetPosInfo.Moneyness = "OTM";

                                    _NetPosInfo.ITM_OTM_Percentage = Math.Round((Math.Abs(_NetPosInfo.StrikePrice - _NetPosInfo.SpotPrice) * 100) / _NetPosInfo.StrikePrice, 2);

                                    if (temp_exp == DateTime.Now.ToString("ddMMyyyy"))
                                    {
                                        _NetPosInfo.IntrinsicMTM = (_NetPosInfo.StrikePrice - _NetPosInfo.SpotPrice) * _NetPosInfo.NetPosition;
                                    }
                                }
                            }


                            //added on 25MAY2021 by Amey
                            _NetPosInfo.Delta = _NetPosInfo.Delta * CollectionHelper._ValueSigns.Delta;
                            _NetPosInfo.Gamma = _NetPosInfo.Gamma * CollectionHelper._ValueSigns.Gamma;
                            _NetPosInfo.Theta = _NetPosInfo.Theta * CollectionHelper._ValueSigns.Theta;
                            _NetPosInfo.Vega = _NetPosInfo.Vega * CollectionHelper._ValueSigns.Vega;
                            _NetPosInfo.DeltaAmount = _NetPosInfo.Delta * _NetPosInfo.UnderlyingLTP * CollectionHelper._ValueSigns.DeltaAmt;
                            _NetPosInfo.Turnover = (_NetPosInfo.IntradayBuyQuantity * _NetPosInfo.IntradayBuyAvg) + (_NetPosInfo.IntradaySellQuantity * _NetPosInfo.IntradaySellAvg);

                            list_AllPositions.Add(_NetPosInfo);

                            //foreach (var details in Properties)
                            //{
                            //   sb_AllPosition.Append(details.GetValue(_NetPosInfo) + ",");
                            //}
                            //sb_AllPosition.Remove(sb_AllPosition.ToString().LastIndexOf(','), 1);
                            //sb_AllPosition.Append("\n");

                        }
                    }

                    DataTable dataTable = new DataTable();
                    //Get all the properties by using reflection   
                    PropertyInfo[] Props = typeof(NetPositionInfo).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo prop in Props)
                    {
                        //Setting column names as Property names  
                        dataTable.Columns.Add(prop.Name);
                    }
                    foreach (var item in list_AllPositions)
                    {
                        var values = new object[Props.Length];
                        for (int i = 0; i < Props.Length; i++)
                        {

                            values[i] = Props[i].GetValue(item, null);
                        }
                        dataTable.Rows.Add(values);
                    }

                    StringBuilder sb_AllPosition = new StringBuilder();
                    IEnumerable<string> columnnames = dataTable.Columns.Cast<DataColumn>().Select(col => col.ColumnName);
                    sb_AllPosition.AppendLine(string.Join(",", columnnames));

                    foreach (DataRow row in dataTable.Rows)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sb_AllPosition.AppendLine(string.Join(",", fields));
                    }

                    string autosave_fileloc = Application.StartupPath + "\\" + "Report\\" + "AllPositions.csv";
                    File.WriteAllText(autosave_fileloc, sb_AllPosition.ToString());
                                        
                    sb_AllPosition.Clear();
                }
            }
            catch (Exception ee) { _logger.Error(ee); }

            isAutoSaving = false;
        }

        private T CopyPropertiesFrom<T>(ConsolidatedPositionInfo FromObj, T ToObj)
        {
            foreach (PropertyInfo propTo in ToObj.GetType().GetProperties())
            {
                PropertyInfo propFrom = FromObj.GetType().GetProperty(propTo.Name);
                if (propFrom != null && propFrom.CanWrite)
                    propTo.SetValue(ToObj, propFrom.GetValue(FromObj, null), null);
            }

            return ToObj;
        }

        private double DivideByBaseAndRound(double Value, string ColName)
        {
            return Math.Round(Value / CollectionHelper.dict_BaseValue[ColName], CollectionHelper.dict_CustomDigits[ColName]);
        }

        #endregion

        private void lbl_NiftyVal_Click(object sender, EventArgs e)
        {

        }

        private void pic_Logo_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}
