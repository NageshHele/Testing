using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using n.Structs;
using NerveLog;
using Newtonsoft.Json;
using Prime.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraGrid.Columns;
using System.Configuration;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Tulpep.NotificationWindow;
using System.Text.RegularExpressions;
using DevExpress.DataAccess.UI.ExpressionEditor;
using System.Media;

namespace Prime.UI
{
    public partial class uc_ClientPortfolio : XtraUserControl
    {
        public event del_IsStateChanged eve_IsClientExpanded;
        public event del_IsStateChanged eve_IsUnderlyingExpanded;
        public event del_IsFlagChanged eve_IsClientWindowShown;
        public event del_ClientWindowData eve_ClientWindowDataSend;
        public event del_IsStateChanged eve_IsVaRDistributionShown;

        NerveLogger _logger;

        /// <summary>
        /// Will be set when inside ShowingEditor event with FocusedRows ClientID value.
        /// </summary>
        string FocusedRowClientID = string.Empty;

        #region Layout File Names

        /// <summary>
        /// Path to save ClientPorofolio Parent Grid Layout.
        /// </summary>
        readonly string layout_CPParent = Application.StartupPath + "\\Layout\\" + "CPParent.xml";

        /// <summary>
        /// Path to save ClientPorofolio Underlying Grid Layout.
        /// </summary>
        readonly string layout_CPUnderlying = Application.StartupPath + "\\Layout\\" + "CPUnderlying.xml";

        /// <summary>
        /// Path to save ClientPorofolio Positions Grid Layout.
        /// </summary>
        readonly string layout_CPPositions = Application.StartupPath + "\\Layout\\" + "CPPositions.xml";

        #endregion

        CPParent _TempCPParent = new CPParent();
        CPUnderlying _TempCPUnderlying = new CPUnderlying();
        CPPositions _TempCPPositions = new CPPositions();

        form_VaRDistribution _VaRDistribution = new form_VaRDistribution();
        form_ClientWindow _ClientWindow = new form_ClientWindow();     //Added by Akshay on 28-07-2021 for Client Window

        /// <summary>
        /// Set True when VaRDistribution form is open.
        /// </summary>
        bool isVaRDistributionOpen = false;

        /// <summary>
        /// Set True when ClientWindow form is open.
        /// </summary>
        bool isClientWindowOpen = false;

        /// <summary>
        /// Set true when Rule Manager is open
        /// </summary>
        bool isRuleManagerOpen = false;

        /// <summary>
        /// Set true when Rules are being compared
        /// </summary>
        bool isComparing = false;

        #region Right Click Menu Items

        DXMenuItem menu_ExportToCSV = new DXMenuItem();// added by navin on 18-03-2019 for export to excel feature
        DXMenuItem menu_SaveLayout = new DXMenuItem();// added by navin on 18-03-2019 for export to excel feature
        DXMenuItem menu_RefreshPriority = new DXMenuItem();// Added by Akshay on 28-12-2020 for refresh priority feature
        DXMenuItem menu_ExportClientPosition = new DXMenuItem();// Added by Snehadri on 24JUN2021 to save selected client position
        DXMenuItem menu_FixColumn = new DXMenuItem(); // Added by Snehadri on 06JUL2021 for freeze column feature 

        DXMenuItem menu_RuleManager = new DXMenuItem();
        DXMenuItem menu_AddExpressCol = new DXMenuItem(); // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        DXMenuItem menu_DeleteExpCol = new DXMenuItem(); // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        

        DXMenuItem menu_ExportUnderlyingLevelToCSV = new DXMenuItem();
        DXMenuItem menu_ExportPositionLevelToCSV = new DXMenuItem();
        #endregion

        /// <summary>
        /// DataSource for gc_ClientPortfolio.
        /// </summary>
        RealTimeSource rts_ClientPortfolio = new RealTimeSource();

        /// <summary>
        /// DataSource for VaR Distribution grid.
        /// </summary>
        DataTable dt_IndividualRangeVaR = new DataTable();

        //Added by Akshay on 28-07-2021 for Client Window
        /// <summary>
        /// DataSource for gc_ClientWindow.
        /// </summary>
        RealTimeSource rts_ClientWindow = new RealTimeSource();

        //Added by Akshay on 28-07-2021 for Client Window
        /// <summary>
        /// DataSource for gc_ClientWindow.
        /// </summary>
        RealTimeSource rts_ClientWindowOptions = new RealTimeSource();

        //Added by Akshay on 28-07-2021 for Client Window
        /// <summary>
        /// DataSource for gc_ClientWindow.
        /// </summary>
        RealTimeSource rts_ClientWindowFutures = new RealTimeSource();

        //Added by Akshay on 28-07-2021 for Client Window
        /// <summary>
        /// DataSource for gc_ClientWindow.
        /// </summary>
        RealTimeSource rts_ClientWindowGreeks = new RealTimeSource();

        //Added by Snehadri on 24JUN2021
        /// <summary>
        /// Key : ClientID | Value : List of Positions. For saving positions of client
        /// </summary>
        Dictionary<string, List<ConsolidatedPositionInfo>> dict_ClientPositions = new Dictionary<string, List<ConsolidatedPositionInfo>>();

        
        // Added by Snehadri on 05JUL2021 for fix column feature
        /// <summary>
        /// Stores the name of the column on which right click was performed
        /// </summary>
        string RightClicked_ColumnName = string.Empty;

        /// <summary>
        /// Stores the information of the gridview on which right click was performed
        /// </summary>
        GridView RightClicked_ViewInfo = new GridView();

        // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        
        string expressionfileloc = Application.StartupPath + "\\Report\\ExpressionColumns.json";
        string rulefileloc = Application.StartupPath + "\\Report\\Rule.json";
        string fixcolumnloc = Application.StartupPath + "\\Report\\Portfolio_FixColumns.json";

        // Added by Snehadri on 25AUG2021 for the popup snooze 
        /// <summary>
        /// Key: ClientId,Rule Name   | Value: DateTime
        /// </summary>
        Dictionary<string, DateTime> dict_ClinetAlertTime = new Dictionary<string, DateTime>();

        // Added by Snehadri on 25AUG2021 for the popup snooze
        int rule_timeinterval;
        int popup_snooze;

        private uc_ClientPortfolio()
        {
            InitializeComponent();

            this._logger = CollectionHelper._logger;

            InitialiseUC();
        }

        #region Instance Initializing

        public static uc_ClientPortfolio Instance { get; private set; }

        public static void Initialise()
        {
            if (Instance is null)
                Instance = new uc_ClientPortfolio();
        }

        #endregion

        #region Imp Methods

        private void InitialiseUC()
        {
            try
            {
                menu_SaveLayout.Caption = "Save layout";
                menu_SaveLayout.Click += Menu_SaveLayout_Click;

                //Added by Akshay on 28-12-2020 for new refresh priority feature
                menu_RefreshPriority.Caption = "Refresh Priority";
                menu_RefreshPriority.Click += Menu_RefreshPriority_Click;

                //added on 30DEC2020 by Amey
                menu_ExportToCSV.Caption = "Export As CSV";
                menu_ExportToCSV.Click += Menu_ExportToCSV_Click;

                //added on 30DEC2020 by Amey
                menu_ExportUnderlyingLevelToCSV.Caption = "Export Underlyings As CSV";
                menu_ExportUnderlyingLevelToCSV.Click += Menu_ExportUnderlyingLevelToCSV_Click;

                //added on 30DEC2020 by Amey
                menu_ExportPositionLevelToCSV.Caption = "Export Positions As CSV";
                menu_ExportPositionLevelToCSV.Click += Menu_ExportPositionToCSV_Click;

                // Added by Snehadri on 24JUN2021
                menu_ExportClientPosition.Caption = "Export Client Position";
                menu_ExportClientPosition.Click += Menu_ExportClientPosition_Click;

                // Added by Snehadri on
                menu_FixColumn.Caption = "Fix/Unfix Column";
                menu_FixColumn.Click += Menu_FixColumn_Click;

                menu_RuleManager.Caption = "Rule Manager";
                menu_RuleManager.Click += Menu_RuleManger;

                // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
                menu_AddExpressCol.Caption = "Add Formula Column";
                menu_AddExpressCol.Click += AddExpressCol;
                                
                // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
                menu_DeleteExpCol.Caption = "Delete Formula Column";
                menu_DeleteExpCol.Click += DeleteExpressionCol;


                rts_ClientPortfolio.DataSource = CollectionHelper.bList_ClientPortfolio;
                gc_ClientPortfolio.DataSource = rts_ClientPortfolio;

                CollectionHelper.gc_CP = gc_ClientPortfolio;

                CollectionHelper.IsVarticalLines = Convert.ToBoolean(ConfigurationManager.AppSettings["VERTICAL-LINES"]);    //Added on Akshay on 24-08-2021
                CollectionHelper.IsFullVAR = Convert.ToBoolean(ConfigurationManager.AppSettings["FULL-VAR"]);    //Added by Akshay on 24-08-2021

                if (CollectionHelper.IsVarticalLines)
                {
                    gv_ClientPortfolio.Appearance.VertLine.BackColor = SystemColors.ActiveBorder;
                    gv_ClientPortfolio.Appearance.VertLine.BackColor2 = SystemColors.ActiveBorder;
                }

                if (!CollectionHelper.IsFullVAR)
                    gv_ClientPortfolio.Columns["VarDistribution"].Visible = false;

                // Added by Snehadri on 19JUL2021 for Rule Builder
                var CONInfo = ConfigurationManager.AppSettings;
                rule_timeinterval = int.Parse(CONInfo["RULE-ALERT-INTERVAL"].ToString());
                timer_Rule.Interval = rule_timeinterval * 1000;
                popup_snooze = int.Parse(CONInfo["POPUP-SNOOZE"].ToString());


                if (dt_IndividualRangeVaR.Columns.Count == 0)
                {
                    dt_IndividualRangeVaR.Columns.Add("Underlying");

                    for (int i = -30; i <= 30; i++)
                        dt_IndividualRangeVaR.Columns.Add(i.ToString(), typeof(decimal));
                }


                if (dt_IndividualRangeVaR.Columns.Count == 0)
                {
                    dt_IndividualRangeVaR.Columns.Add("Underlying");

                    for (int i = -30; i <= 30; i++)
                        dt_IndividualRangeVaR.Columns.Add(i.ToString(), typeof(decimal));
                }

                // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
                if (CollectionHelper.dict_ExpressionColumn.ContainsKey(0))
                {
                    var dict_temp = new Dictionary<string, string>(CollectionHelper.dict_ExpressionColumn[0]);
                    if (dict_temp != null)
                    {
                        foreach (var Colname in dict_temp.Keys)
                        {
                            string colName = Colname;
                            string expression = dict_temp[Colname];

                            GridColumn unbColumn = gv_ClientPortfolio.Columns.AddField(colName);
                            unbColumn.Caption = colName;
                            unbColumn.UnboundExpression = expression; 
                            unbColumn.VisibleIndex = gv_ClientPortfolio.Columns.Count;
                            unbColumn.UnboundType = UnboundColumnType.Decimal;
                            unbColumn.ShowUnboundExpressionMenu = true;
                            
                            unbColumn.OptionsColumn.AllowEdit = false;

                            unbColumn.DisplayFormat.FormatType = FormatType.Numeric;
                            unbColumn.DisplayFormat.FormatString = "N2";

                            unbColumn.AppearanceCell.BackColor = Color.LightBlue;

                            var pattern = @"(?<=\[)[^]]+(?=\])";
                            var isNumericCol = true;
                            foreach (Match match in Regex.Matches(unbColumn.UnboundExpression, pattern))
                            {
                                if (!CollectionHelper.list_DecimalColumns.Contains(match.Value))
                                    isNumericCol = false;
                            }

                            if (isNumericCol)
                                unbColumn.UnboundDataType = typeof(double);
                        }
                    }
                }

                // Added by Snehadri on 19JUL2021 for Fix/Unfix Column
                if (CollectionHelper.dict_FixColumn.ContainsKey(0))
                {
                    foreach (var item in CollectionHelper.dict_FixColumn[0])
                    {
                        if(gv_ClientPortfolio.Columns[item] != null) { gv_ClientPortfolio.Columns[item].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left; }                        
                    }
                }

                //added on 01APR2021 by Amey
                if (File.Exists(layout_CPParent))
                    gv_ClientPortfolio.RestoreLayoutFromXml(layout_CPParent);

                //gv_ClientPortfolio.Columns["VAR1"].Caption = CollectionHelper.hs_VarRange.ElementAt(0).ToString() + "%";
                //gv_ClientPortfolio.Columns["VAR2"].Caption = CollectionHelper.hs_VarRange.ElementAt(1).ToString() + "%";
                //gv_ClientPortfolio.Columns["VAR3"].Caption = CollectionHelper.hs_VarRange.ElementAt(2).ToString() + "%";
                //gv_ClientPortfolio.Columns["VAR4"].Caption = CollectionHelper.hs_VarRange.ElementAt(3).ToString() + "%";
                // FontSize
                try
                {
                    gv_ClientPortfolio.Appearance.Row.Font = new Font("Segoe UI", CollectionHelper.DataFontSize);
                    gv_ClientPortfolio.Appearance.HeaderPanel.Font = new Font("Segoe UI", CollectionHelper.DataFontSize + 2, FontStyle.Bold);
                    gv_ClientPortfolio.Appearance.FooterPanel.Font = new Font("Segoe UI", CollectionHelper.FooterFontSize);
                }
                catch (Exception) { }
                               
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        #endregion

        #region UI Events

        private void gc_ClientPortfolio_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                var dict_CustomColumnNames = CollectionHelper.dict_CustomColumnNames;
                var list_DecimalColumns = CollectionHelper.list_DecimalColumns;
                var dict_CustomDigits = CollectionHelper.dict_CustomDigits;

                for (int i = 0; i < gv_ClientPortfolio.Columns.Count; i++)
                {
                    string ColumnFieldName = gv_ClientPortfolio.Columns[i].FieldName;

                    //changed to TryGetValue on 27MAY2021 by Amey
                    //added on 22MAR2021 by Amey
                    if (dict_CustomColumnNames.TryGetValue(ColumnFieldName, out string CName) && CName != "")
                        gv_ClientPortfolio.Columns.ColumnByFieldName(ColumnFieldName).Caption = CName;

                    //added on 08APR2021 by Amey
                    if (list_DecimalColumns.Contains(ColumnFieldName))
                    {
                        gv_ClientPortfolio.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;

                        //added on 24MAY2021 by Amey
                        if (dict_CustomDigits.TryGetValue(ColumnFieldName, out int RoundDigit))
                            gv_ClientPortfolio.Columns[i].DisplayFormat.FormatString = "N" + RoundDigit;
                        else
                            gv_ClientPortfolio.Columns[i].DisplayFormat.FormatString = "N2";

                        //added on 15APR2021 by Amey. To display commas with Indian format.
                        gv_ClientPortfolio.Columns[i].DisplayFormat.Format = CultureInfo.CreateSpecificCulture("hi-IN").NumberFormat;
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gc_ClientPortfolio_ViewRegistered(object sender, ViewOperationEventArgs e)
        {
            try
            {
                GridView view = (e.View as GridView);
                if (view.IsDetailView == false)
                    return;

                //added on 28FEB2021 by Amey
                view.OptionsBehavior.Editable = false;
                var dict_CustomColumnNames = CollectionHelper.dict_CustomColumnNames;
                var list_DecimalColumns = CollectionHelper.list_DecimalColumns;
                var dict_CustomDigits = CollectionHelper.dict_CustomDigits;

                if (view.LevelName == nameof(_TempCPParent.bList_Underlying))
                {

                    //added on 01APR2021 by Amey
                    try
                    {
                        view.PopupMenuShowing -= gv_Underlying_PopupMenuShowing;
                        view.MasterRowExpanded -= gv_CPUnderlying_MasterRowExpanded;
                        view.MasterRowCollapsed -= gv_CPUnderlying_MasterRowCollapsed;
                        view.RowCellStyle -= gv_CPUnderlying_RowCellStyle;
                        view.CustomDrawFooterCell -= gridView_CustomDrawFooterCell;
                        view.RowCellClick -= gv_CPUnderlying_RowCellClick;  //Added by Akshay on 28-07 for window
                        view.CustomRowCellEdit -= gv_CPUnderlying_CustomRowCellEdit;
                        view.ColumnUnboundExpressionChanged -= gv_Underlying_ColumnUnboundExpressionChanged; //Added by Snehadri on 11AUG2021 for Expression column in underlying level

                        
                    }
                    catch (Exception) { }

                    view.PopupMenuShowing += gv_Underlying_PopupMenuShowing;
                    view.MasterRowExpanded += gv_CPUnderlying_MasterRowExpanded;
                    view.MasterRowCollapsed += gv_CPUnderlying_MasterRowCollapsed;
                    view.RowCellStyle += gv_CPUnderlying_RowCellStyle;
                    view.CustomDrawFooterCell += gridView_CustomDrawFooterCell;
                    view.RowCellClick += gv_CPUnderlying_RowCellClick;  //Added by Akshay on 28-07 for window
                    view.CustomRowCellEdit += gv_CPUnderlying_CustomRowCellEdit;
                    view.ColumnUnboundExpressionChanged += gv_Underlying_ColumnUnboundExpressionChanged; //Added by Snehadri on 11AUG2021 for Expression column in underlying level

                    

                    //Added by Snehadri 05JUL2021 for fix column feature
                    if (CollectionHelper.dict_FixColumn.ContainsKey(view.DetailLevel))
                    {
                        foreach (var item in CollectionHelper.dict_FixColumn[view.DetailLevel])
                        {
                            if (view.Columns[item] != null) { view.Columns[item].Fixed = FixedStyle.Left; }
                        }
                    }

                    for (int i = 0; i < view.Columns.Count; i++)
                    {
                        string ColumnFieldName = view.Columns[i].FieldName;

                        //changed to TryGetValue on 27MAY2021 by Amey
                        //added on 22MAR2021 by Amey
                        if (dict_CustomColumnNames.TryGetValue(ColumnFieldName, out string CName) && CName != "")
                            view.Columns.ColumnByFieldName(ColumnFieldName).Caption = CName;

                        //added on 08APR2021 by Amey
                        if (list_DecimalColumns.Contains(ColumnFieldName))
                        {
                            view.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;

                            //added on 24MAY2021 by Amey
                            if (dict_CustomDigits.TryGetValue(ColumnFieldName, out int RoundDigit))
                                view.Columns[i].DisplayFormat.FormatString = "N" + RoundDigit;
                            else
                                view.Columns[i].DisplayFormat.FormatString = "N2";

                            //added on 15APR2021 by Amey. To display commas with Indian format.
                            view.Columns[i].DisplayFormat.Format = CultureInfo.CreateSpecificCulture("hi-IN").NumberFormat;
                        }
                    }


                    #region added on 19-03-2020 to restore child layout

                    //changed on 03FEB2021 by Amey
                    if (File.Exists(layout_CPUnderlying))
                        view.RestoreLayoutFromXml(layout_CPUnderlying);

                    #endregion

                    //view.Columns["VAR1"].Caption = CollectionHelper.hs_VarRange.ElementAt(0).ToString() + "%";
                    //view.Columns["VAR2"].Caption = CollectionHelper.hs_VarRange.ElementAt(1).ToString() + "%";
                    //view.Columns["VAR3"].Caption = CollectionHelper.hs_VarRange.ElementAt(2).ToString() + "%";
                    //view.Columns["VAR4"].Caption = CollectionHelper.hs_VarRange.ElementAt(3).ToString() + "%";
                }
                else if (view.LevelName == nameof(_TempCPUnderlying.bList_Positions))
                {
                    //added on 13OCT2020 by Amey
                    try
                    {
                        view.RowCellStyle -= gv_CPPositions_RowCellStyle;
                        view.PopupMenuShowing -= gv_Positions_PopupMenuShowing;
                        view.CustomDrawFooterCell -= gridView_CustomDrawFooterCell;
                        
                    }
                    catch (Exception) { }

                    view.RowCellStyle += gv_CPPositions_RowCellStyle;
                    view.PopupMenuShowing += gv_Positions_PopupMenuShowing;
                    view.CustomDrawFooterCell += gridView_CustomDrawFooterCell;


                    //Added by Snehadri on 05JUL2021 for fix column feature 
                    if (CollectionHelper.dict_FixColumn.ContainsKey(view.DetailLevel))
                    {
                        foreach (var item in CollectionHelper.dict_FixColumn[view.DetailLevel])
                        {
                            if (view.Columns[item] != null) { view.Columns[item].Fixed = FixedStyle.Left; }
                        }
                    }

                    //added on 27NOV2020 by Amey
                    for (int i = 0; i < view.Columns.Count; i++)
                    {
                        string ColumnFieldName = view.Columns[i].FieldName;

                        //changed to TryGetValue on 27MAY2021 by Amey
                        //added on 22MAR2021 by Amey
                        if (dict_CustomColumnNames.TryGetValue(ColumnFieldName, out string CName) && CName != "")
                            view.Columns.ColumnByFieldName(ColumnFieldName).Caption = CName;

                        //added on 08APR2021 by Amey
                        if (list_DecimalColumns.Contains(ColumnFieldName))
                        {
                            view.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;

                            //added on 24MAY2021 by Amey
                            if (dict_CustomDigits.TryGetValue(ColumnFieldName, out int RoundDigit))
                                view.Columns[i].DisplayFormat.FormatString = "N" + RoundDigit;
                            else
                                view.Columns[i].DisplayFormat.FormatString = "N2";

                            //added on 15APR2021 by Amey. To display commas with Indian format.
                            view.Columns[i].DisplayFormat.Format = CultureInfo.CreateSpecificCulture("hi-IN").NumberFormat;
                        }
                    }

                    #region added on 19-03-2020 to restore child layout

                    //changed on 03FEB2021 by Amey
                    if (File.Exists(layout_CPPositions))
                        view.RestoreLayoutFromXml(layout_CPPositions);
                    else
                        view.BestFitColumns();

                    #endregion


                    //view.Columns["VAR1"].Caption = CollectionHelper.hs_VarRange.ElementAt(0).ToString() + "%";
                    //view.Columns["VAR2"].Caption = CollectionHelper.hs_VarRange.ElementAt(1).ToString() + "%";
                    //view.Columns["VAR3"].Caption = CollectionHelper.hs_VarRange.ElementAt(2).ToString() + "%";
                    //view.Columns["VAR4"].Caption = CollectionHelper.hs_VarRange.ElementAt(3).ToString() + "%";
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_ClientPortfolio_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                //added on 18NOV2020 by Amey. To avoid NullRefExceptions.
                if (e is null) return;

                //changed on 03FEB2021 by Amey
                if (e.Column.FieldName == nameof(_TempCPParent.VaRUti) || e.Column.FieldName==nameof(_TempCPParent.MarginLimitUtil) || e.Column.FieldName==nameof(_TempCPParent.MTMLimitUtil) || e.Column.FieldName == nameof(_TempCPParent.VARLimitUtil))
                    e.DisplayText = e.Value + "%";
                if (e.DisplayText == "∞" || e.DisplayText == "-∞" || e.DisplayText == "")
                    e.DisplayText = "0.00";

            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_ClientPortfolio_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                //added on 18NOV2020 by Amey. To avoid NullRefExceptions.
                if (e is null) return;

                if (e.Column.FieldName == nameof(_TempCPParent.VarDistribution))
                    e.RepositoryItem = gc_ClientPortfolio.RepositoryItems["repBtn_VaRDistribution"];
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_ClientPortfolio_MasterRowCollapsed(object sender, CustomMasterRowEventArgs e)
        {
            string ClientID = gv_ClientPortfolio.GetFocusedRowCellValue(nameof(_TempCPParent.ClientID)).ToString();
            eve_IsClientExpanded(false, ClientID);
        }

        private void gv_ClientPortfolio_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            string ClientID = gv_ClientPortfolio.GetFocusedRowCellValue(nameof(_TempCPParent.ClientID)).ToString();
            eve_IsClientExpanded(true, ClientID);
        }

        private void gv_ClientPortfolio_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {

                //added on 18NOV2020 by Amey. To avoid NullRefExceptions.
                if (e is null) return;

                var FieldName = e.Column.FieldName;

                //changed on 03FEB2021 by Amey
                if (FieldName == nameof(_TempCPParent.VaRUti))
                {
                    var val = Convert.ToInt32(e.CellValue);
                    if (val < 50)
                        e.Appearance.BackColor = Color.LightGreen;
                    else if ((Convert.ToInt32(e.CellValue) >= 50) && (Convert.ToInt32(e.CellValue) < 75))
                        e.Appearance.BackColor = Color.FromArgb(255, 179, 0);
                    else if (Convert.ToInt32(e.CellValue) >= 75)
                    {
                        //added ForeColor change on 15APR2021 by Amey
                        e.Appearance.ForeColor = Color.White;
                        e.Appearance.BackColor = Color.Red;
                    }
                }
                //added on 15APR2021 by Amey
                else if (FieldName == nameof(_TempCPParent.MTM) || FieldName == nameof(_TempCPParent.IntradayMTM) || FieldName == nameof(_TempCPParent.TheoreticalMTM) ||
                    FieldName == nameof(_TempCPParent.FuturesMTM) || FieldName == nameof(_TempCPParent.OptionsMTM) || FieldName == nameof(_TempCPParent.EquityMTM) ||
                    FieldName == nameof(_TempCPParent.IntradayFuturesMTM) || FieldName == nameof(_TempCPParent.IntradayOptionsMTM) || FieldName == nameof(_TempCPParent.IntradayEquityMTM) ||
                    FieldName == nameof(_TempCPParent.ROV) || FieldName == nameof(_TempCPParent.DayPremium) || FieldName == nameof(_TempCPParent.DayNetPremium) || 
                    FieldName == nameof(_TempCPParent.BankniftyExpoOPT) || FieldName == nameof(_TempCPParent.NiftyExpoOPT))
                {
                    var val = Convert.ToDouble(e.CellValue);
                    if (val >= 0)
                        e.Appearance.ForeColor = Color.Green;
                    else
                        e.Appearance.ForeColor = Color.Red;
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_ClientPortfolio_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                var FocusedFieldName = gv_ClientPortfolio.FocusedColumn.FieldName;
                if (FocusedFieldName == nameof(_TempCPParent.VarDistribution))
                    e.Cancel = false;
                else if (FocusedFieldName == nameof(_TempCPParent.Adhoc))
                {
                    FocusedRowClientID = gv_ClientPortfolio.GetRowCellValue(gv_ClientPortfolio.FocusedRowHandle, nameof(_TempCPParent.ClientID)).ToString();

                    e.Cancel = false;
                    form_EditableProperty _Edit = new form_EditableProperty(FocusedFieldName, Convert.ToDouble(gv_ClientPortfolio.FocusedValue));
                    _Edit.eve_EditedValueReceived += _Edit_eve_EditedValueReceived;
                    _Edit.ShowDialog();
                }
                else
                    e.Cancel = true;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }


        private void gv_CPUnderlying_MasterRowCollapsed(object sender, CustomMasterRowEventArgs e)
        {
            try
            {
                GridView gv_CPUnderlying = sender as GridView;

                string ID = gv_CPUnderlying.GetFocusedRowCellValue(nameof(_TempCPUnderlying.ClientID)).ToString();
                string Underlying = gv_CPUnderlying.GetFocusedRowCellValue(nameof(_TempCPUnderlying.Underlying)).ToString();

                eve_IsUnderlyingExpanded(false, $"{ID}_{Underlying}");
            }
            catch (NullReferenceException ee) { _logger.Error(ee); }
        }

        private void gv_CPUnderlying_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            try
            {
                GridView gv_CPUnderlying = sender as GridView;

                string ID = gv_CPUnderlying.GetFocusedRowCellValue(nameof(_TempCPUnderlying.ClientID)).ToString();
                string Underlying = gv_CPUnderlying.GetFocusedRowCellValue(nameof(_TempCPUnderlying.Underlying)).ToString();

                eve_IsUnderlyingExpanded(true, $"{ID}_{Underlying}");
            }
            catch (NullReferenceException ee) { _logger.Error(ee); }
        }

        private void gv_CPPositions_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                //added on 18NOV2020 by Amey. To avoid NullRefExceptions.
                if (e is null) return;

                var FieldName = e.Column.FieldName;
                

                //added on 27NOV2020 by Amey
                if (FieldName == nameof(_TempCPPositions.LTP))
                {
                    GridView view = sender as GridView;
                    bool isLTPCalculated = Convert.ToBoolean(view.GetRowCellValue(e.RowHandle, nameof(_TempCPPositions.IsLTPCalculated)));

                    if (isLTPCalculated)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Black;
                }
                //added on 15APR2021 by Amey
                else if (FieldName == nameof(_TempCPPositions.MTM) || FieldName == nameof(_TempCPPositions.IntradayMTM) || FieldName == nameof(_TempCPPositions.TheoreticalMTM) ||
                         FieldName == nameof(_TempCPPositions.FuturesMTM) || FieldName == nameof(_TempCPPositions.OptionsMTM) || FieldName == nameof(_TempCPPositions.EquityMTM) ||
                         FieldName == nameof(_TempCPPositions.IntradayFuturesMTM) || FieldName == nameof(_TempCPPositions.IntradayOptionsMTM) || 
                         FieldName == nameof(_TempCPPositions.IntradayEquityMTM) || FieldName == nameof(_TempCPPositions.ROV))
                {
                    var val = Convert.ToDouble(e.CellValue);
                    if (val >= 0)
                        e.Appearance.ForeColor = Color.Green;
                    else
                        e.Appearance.ForeColor = Color.Red;
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        //added on 15APR2021 by Amey
        private void gv_CPUnderlying_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                //added on 18NOV2020 by Amey. To avoid NullRefExceptions.
                if (e is null) return;

                var FieldName = e.Column.FieldName;

                //added on 15APR2021 by Amey
                if (FieldName == nameof(_TempCPUnderlying.MTM) || FieldName == nameof(_TempCPUnderlying.IntradayMTM) || FieldName == nameof(_TempCPUnderlying.TheoreticalMTM) ||
                    FieldName == nameof(_TempCPUnderlying.FuturesMTM) || FieldName == nameof(_TempCPUnderlying.OptionsMTM) || FieldName == nameof(_TempCPUnderlying.EquityMTM) ||
                    FieldName == nameof(_TempCPUnderlying.IntradayFuturesMTM) || FieldName == nameof(_TempCPUnderlying.IntradayOptionsMTM) ||
                    FieldName == nameof(_TempCPUnderlying.IntradayEquityMTM) || FieldName == nameof(_TempCPUnderlying.PosExpoOPT) || 
                    FieldName == nameof(_TempCPUnderlying.PosExpoFUT) || FieldName == nameof(_TempCPUnderlying.ROV))
                {
                    var val = Convert.ToDouble(e.CellValue);
                    if (val >= 0)
                        e.Appearance.ForeColor = Color.Green;
                    else
                        e.Appearance.ForeColor = Color.Red;
                }
                
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;//added by Navin on 06-06-2019

            var ediorItem = e.Menu.Items.Where(x => x.Caption == "Expression Editor...").FirstOrDefault();
            if (ediorItem != null)
                ediorItem.Caption = "Formula Editor";

            e.Menu.Items.Add(menu_ExportToCSV);
            e.Menu.Items.Add(menu_SaveLayout);

            e.Menu.Items.Add(menu_RefreshPriority);//Added by Akshay on 28-12-2020 for Priority Feature       //Commented by Akshay on 29-12-2020 For removing priority feature
            e.Menu.Items.Add(menu_ExportClientPosition); // Added by Snehadri on 22JUN2021 to save Client Position data

            //Added by Snehadri on 05JUL2021 for fix column feature
            if (e.MenuType == GridMenuType.Column)
            {
                RightClicked_ViewInfo = new GridView(); RightClicked_ViewInfo = e.HitInfo.View;
                RightClicked_ColumnName = string.Empty; RightClicked_ColumnName = e.HitInfo.Column.FieldName;

                e.Menu.Items.Add(menu_FixColumn);
                e.Menu.Items.Add(menu_RuleManager);
                e.Menu.Items.Add(menu_AddExpressCol);                // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
                e.Menu.Items.Add(menu_DeleteExpCol);                // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder

            }
        }

        private void gv_Underlying_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;

            e.Menu.Items.Add(menu_ExportUnderlyingLevelToCSV);
            e.Menu.Items.Add(menu_SaveLayout);
            e.Menu.Items.Add(menu_ExportClientPosition); // Added by Snehadri on 22JUN2021 to save Client Position data

            //Added by Snehadri on 05JUL2021 for fix column feature
            if (e.MenuType == GridMenuType.Column)
            {
                RightClicked_ViewInfo = new GridView(); RightClicked_ViewInfo = e.HitInfo.View;
                RightClicked_ColumnName = string.Empty; RightClicked_ColumnName = e.HitInfo.Column.FieldName;

                e.Menu.Items.Add(menu_FixColumn);
                e.Menu.Items.Add(menu_RuleManager);
                
            }
        }

        private void gv_Positions_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;

            e.Menu.Items.Add(menu_ExportPositionLevelToCSV);
            e.Menu.Items.Add(menu_SaveLayout);
            e.Menu.Items.Add(menu_ExportClientPosition); // Added by Snehadri on 22JUN2021 to save Client Position data

            //Added by Snehadri on 05JUL2021 for fix column feature
            if (e.MenuType == GridMenuType.Column)
            {
                RightClicked_ViewInfo = new GridView(); RightClicked_ViewInfo = e.HitInfo.View;
                RightClicked_ColumnName = string.Empty; RightClicked_ColumnName = e.HitInfo.Column.FieldName;
                
                e.Menu.Items.Add(menu_FixColumn);
                
            }
        }

        private void repBtn_VaRDistribution_Click(object sender, EventArgs e)
        {
            VarDistribution(gv_ClientPortfolio.GetFocusedRowCellValue(nameof(_TempCPParent.ClientID)).ToString());
        }

        private void gridView_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            try 
            {
                CommonFunctions.gridView_CustomDrawFooterCell(sender, e);

            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        //added on 25MAY2021 by Amey
        private void gc_ClientPortfolio_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    var view = gc_ClientPortfolio.FocusedView as GridView;

                    if (view.IsDetailView)
                    {
                        var ss = view.GetVisibleDetailView(view.FocusedRowHandle);
                        if (ss != null && !ss.IsFocusedView)
                            view.CollapseMasterRow(view.FocusedRowHandle);
                        else
                        {
                            var parentv = view.ParentView as GridView;
                            parentv.CollapseMasterRow(parentv.FocusedRowHandle);
                        }
                    }
                    else
                        view.CollapseMasterRow(view.FocusedRowHandle);
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    var view = gc_ClientPortfolio.FocusedView as GridView;
                    view.ExpandMasterRow(view.FocusedRowHandle);
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        #endregion

        #region Supplimentary Methods


        internal void SetFocusedRow(string ClientID, string Underlying = "")
        {
            try
            {
                var _Handle = gv_ClientPortfolio.LocateByValue(nameof(_TempCPParent.ClientID), ClientID);
                if (_Handle != GridControl.InvalidRowHandle)
                {
                    gv_ClientPortfolio.FocusedRowHandle = _Handle;
                    gv_ClientPortfolio.ExpandMasterRow(_Handle);

                    if (Underlying != "")
                    {
                        gc_ClientPortfolio.FocusedView = gv_ClientPortfolio.GetDetailView(_Handle, 0);
                        var view = gc_ClientPortfolio.FocusedView as GridView;

                        _Handle = view.LocateByValue(nameof(_TempCPUnderlying.Underlying), Underlying);
                        if (_Handle != GridControl.InvalidRowHandle)
                            view.FocusedRowHandle = _Handle;
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void _Edit_eve_EditedValueReceived(double EditedValue, string FieldName)
        {
            try
            {
                CPParent _CPParent = null;
                lock (CollectionHelper._CPLock)
                {
                    _CPParent = CollectionHelper.bList_ClientPortfolio.Where(v => v.ClientID == FocusedRowClientID).FirstOrDefault();
                }

                if (_CPParent != null)
                {
                    if (FieldName == nameof(_CPParent.Adhoc))
                    {
                        _CPParent.Adhoc = Math.Round(EditedValue, 2);

                        if (CollectionHelper.dict_ClientInfo.TryGetValue(_CPParent.ClientID, out ClientInfo info))
                        {
                            info.AdHoc = Math.Round(EditedValue, 2);
                        }
                    }
                    
                    gv_ClientPortfolio.LayoutChanged();
                }
            }
            catch (Exception ee) { CollectionHelper._logger.Error(ee); };
        }
        // Added by Snehadri on 25JUN2021 
        internal void UpdatePositionDictionary(Dictionary<string, List<ConsolidatedPositionInfo>> dict_AllPositions)
        {
            try
            {
                if(dict_AllPositions != null)
                    dict_ClientPositions = new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_AllPositions);

            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void Menu_ExportToCSV_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "CSV file (*.csv)|*.csv";
                if (_Save.ShowDialog() == DialogResult.OK)
                {
                    DataTable dt_AllPositions = new DataTable();

                    foreach (GridColumn column in gv_ClientPortfolio.VisibleColumns)
                    {
                        dt_AllPositions.Columns.Add(column.FieldName);
                    }

                    for (int i = 0; i < gv_ClientPortfolio.DataRowCount; i++)
                    {
                        DataRow row = dt_AllPositions.NewRow();
                        foreach (GridColumn column in gv_ClientPortfolio.VisibleColumns)
                        {
                            if (gv_ClientPortfolio.GetRowCellValue(i, column) != null)
                                row[column.FieldName] = gv_ClientPortfolio.GetRowCellValue(i, column).ToString();
                        }
                        dt_AllPositions.Rows.Add(row);
                    }

                    StringBuilder sb_AllPosition = new StringBuilder();
                    IEnumerable<string> columnnames = dt_AllPositions.Columns.Cast<DataColumn>().Select(col => col.ColumnName);
                    sb_AllPosition.AppendLine(string.Join(",", columnnames));

                    foreach (DataRow row in dt_AllPositions.Rows)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sb_AllPosition.AppendLine(string.Join(",", fields));
                    }

                    File.WriteAllText(_Save.FileName, sb_AllPosition.ToString());

                    XtraMessageBox.Show("Exported successfully.", "Success");
                }
            }
            catch (Exception ee) { _logger.Error(ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }

        private void Menu_ExportPositionToCSV_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "CSV file (*.csv)|*.csv";
                if (_Save.ShowDialog() == DialogResult.OK)
                {
                    var blist_CPParent = new BindingList<CPParent>(CollectionHelper.bList_ClientPortfolio);
                    var blist_Underlying = blist_CPParent.SelectMany(v => v.bList_Underlying).ToList();
                    var blist_Positions = blist_Underlying.SelectMany(v => v.bList_Positions).ToList();

                    var sb_CPChild = new StringBuilder();
                    var Properties = blist_Positions[0].GetType().GetProperties();
                    var list_columns = RightClicked_ViewInfo.VisibleColumns.ToList();
                    sb_CPChild.Append(string.Join(",", list_columns.Select(p => p.FieldName)));
                    sb_CPChild.Append("\n");

                    foreach (var Row in blist_Positions)
                    {
                        foreach (var columns in list_columns)
                        {
                            var details = Properties.Where(v => v.Name == columns.FieldName).FirstOrDefault();
                            if (details != null)
                                sb_CPChild.Append(details.GetValue(Row) + ",");
                        }
                        sb_CPChild.Remove(sb_CPChild.ToString().LastIndexOf(','), 1);
                        sb_CPChild.Append("\n");
                    }

                    File.WriteAllText(_Save.FileName, sb_CPChild.ToString());

                    XtraMessageBox.Show("Exported successfully.", "Success");
                }
            }
            catch (Exception ee) { _logger.Error(ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }

        private void Menu_ExportUnderlyingLevelToCSV_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "CSV file (*.csv)|*.csv";
                if (_Save.ShowDialog() == DialogResult.OK)
                {
                    var blist_CPParent = new BindingList<CPParent>(CollectionHelper.bList_ClientPortfolio);
                    var blist_Underlying = blist_CPParent.SelectMany(v => v.bList_Underlying).ToList();

                    var sb_CPChild = new StringBuilder();
                    var Properties = blist_Underlying[0].GetType().GetProperties();
                    var list_columns = RightClicked_ViewInfo.VisibleColumns.ToList();
                    sb_CPChild.Append(string.Join(",", list_columns.Select(p => p.FieldName)));
                    sb_CPChild.Append("\n");

                    foreach (var Row in blist_Underlying)
                    {
                        foreach (var column in list_columns)
                        {
                            var details = Properties.Where(v => v.Name == column.FieldName).FirstOrDefault();
                            if (details != null)
                                sb_CPChild.Append(details.GetValue(Row) + ",");
                        }
                        sb_CPChild.Remove(sb_CPChild.ToString().LastIndexOf(','), 1);
                        sb_CPChild.Append("\n");
                    }
                    File.WriteAllText(_Save.FileName, sb_CPChild.ToString());

                    XtraMessageBox.Show("Exported successfully.", "Success");
                }
            }
            catch (Exception ee) { _logger.Error(ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }

        //Added by Snehadri on 24JUN2021 for saving Client's Position
        private void Menu_ExportClientPosition_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "CSV file (*.csv)|*.csv";
                if (_Save.ShowDialog() == DialogResult.OK)
                {
                    string clientid = gv_ClientPortfolio.GetFocusedRowCellValue("ClientID").ToString();

                    var list_AllPositions = new List<NetPositionInfo>();

                    foreach (var position in dict_ClientPositions[clientid])
                    {
                        if (CollectionHelper.dict_ClientInfo.TryGetValue(position.Username, out ClientInfo _ClientInfo))
                        {
                            var _NetPosInfo = CopyPropertiesFrom(position, new NetPositionInfo());

                            _NetPosInfo.Name = _ClientInfo.Name;
                            _NetPosInfo.Zone = _ClientInfo.Zone;
                            _NetPosInfo.Branch = _ClientInfo.Branch;
                            _NetPosInfo.Family = _ClientInfo.Family;
                            _NetPosInfo.Product = _ClientInfo.Product;
                            _NetPosInfo.ExpiryDate = CommonFunctions.ConvertFromUnixTimestamp(position.Expiry);

                            if (_NetPosInfo.ScripType == en_ScripType.CE || _NetPosInfo.ScripType == en_ScripType.PE)
                                _NetPosInfo.ROV = _NetPosInfo.NetPosition * _NetPosInfo.LTP;

                            _NetPosInfo.DayPremium = _NetPosInfo.DayNetPremium * (-1);
                            _NetPosInfo.DayPremiumCDS = _NetPosInfo.DayNetPremiumCDS * (-1);
                            //added on 25MAY2021 by Amey
                            _NetPosInfo.Delta = _NetPosInfo.Delta * CollectionHelper._ValueSigns.Delta;
                            _NetPosInfo.Gamma = _NetPosInfo.Gamma * CollectionHelper._ValueSigns.Gamma;
                            _NetPosInfo.Theta = _NetPosInfo.Theta * CollectionHelper._ValueSigns.Theta;
                            _NetPosInfo.Vega = _NetPosInfo.Vega * CollectionHelper._ValueSigns.Vega;
                            _NetPosInfo.DeltaAmount = _NetPosInfo.Delta * _NetPosInfo.UnderlyingLTP * CollectionHelper._ValueSigns.DeltaAmt;

                            list_AllPositions.Add(_NetPosInfo);
                        }
                    }

                    StringBuilder sb_CPChild = new StringBuilder();
                    PropertyInfo[] Properties = list_AllPositions[0].GetType().GetProperties();
                    sb_CPChild.Append(string.Join(",", Properties.Select(p => p.Name)));
                    sb_CPChild.Append("\n");

                    foreach (var Row in list_AllPositions)
                    {
                        foreach (var details in Properties)
                        {
                            sb_CPChild.Append(details.GetValue(Row) + ",");
                        }
                        sb_CPChild.Remove(sb_CPChild.ToString().LastIndexOf(','), 1);
                        sb_CPChild.Append("\n");
                    }
                    File.WriteAllText(_Save.FileName, sb_CPChild.ToString());

                    XtraMessageBox.Show("Exported successfully.", "Success");

                }

            }
            catch (Exception ee) { _logger.Error(ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }

        //Added by Snehadri on 24JUN2021 for saving Client's Position 
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

        // Added by Snehadri on 05JUL2021 for fix column feature
        private void Menu_FixColumn_Click(object sender, EventArgs e)
        {
            try
            {
                if (RightClicked_ColumnName != "" && RightClicked_ViewInfo != null)
                {
                    GridView view = RightClicked_ViewInfo;
                    string columnName = RightClicked_ColumnName;

                    if (view.Columns[columnName].Fixed == FixedStyle.Left)
                    {
                        CollectionHelper.dict_FixColumn[view.DetailLevel].Remove(columnName);

                        view.Columns[columnName].Fixed = FixedStyle.None;
                        WriteToFixedColumnFile();
                    }
                    else
                    {
                        if (CollectionHelper.dict_FixColumn.ContainsKey(view.DetailLevel))
                            CollectionHelper.dict_FixColumn[view.DetailLevel].Add(columnName);
                        else
                            CollectionHelper.dict_FixColumn.Add(view.DetailLevel, new HashSet<string>() { columnName });

                        view.Columns[columnName].Fixed = FixedStyle.Left;
                        WriteToFixedColumnFile();
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }

        //Added by Snehadri on 05JUL2021 for fix column feature
        private void WriteToFixedColumnFile()
        {
            try
            {
                File.WriteAllText(fixcolumnloc, JsonConvert.SerializeObject(CollectionHelper.dict_FixColumn));
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void Menu_RefreshPriority_Click(object sender, EventArgs e)
        {
            CollectionHelper.RefreshPriority();
        }

        private void Menu_SaveLayout_Click(object sender, EventArgs e)
        {
            try
            {
                gv_ClientPortfolio.SaveLayoutToXml(layout_CPParent);

                foreach (GridView view in gc_ClientPortfolio.Views)
                {
                    if (view.IsDetailView)
                    {
                        if (view.LevelName == nameof(_TempCPParent.bList_Underlying))
                            view.SaveLayoutToXml(layout_CPUnderlying);
                        else if (view.LevelName == nameof(_TempCPUnderlying.bList_Positions))
                            view.SaveLayoutToXml(layout_CPPositions);
                    }
                }
                //added on 27NOV2020 by Amey
                XtraMessageBox.Show("Layout saved successfully.", "Success");
            }
            catch (Exception ee)
            {
                _logger.Error(ee);

                //added on 01APR2021 by Amey
                XtraMessageBox.Show("Something went wrong. Please try again.", "Error");
            }
        }

        private void VarDistribution(string ClientID)
        {
            try
            {
                //added on 28FEB2021 by Amey
                if (!isVaRDistributionOpen)
                {
                    _VaRDistribution = new form_VaRDistribution();

                    //changed on 03FEB2021 by Amey
                    _VaRDistribution.eve_btn_Refresh_Click += VarDistribution;
                    _VaRDistribution.eve_FormClosed += _VaRDistribution_eve_FormClosed;
                }

                dt_IndividualRangeVaR.Clear();

                _VaRDistribution.chart_VaRDistribution.Series["VaR"].Points.Clear();

                DataRow drVaR;
                Dictionary<int, double> dict_GraphValue = new Dictionary<int, double>();//added by Navin on 29-07-2019

                if (CollectionHelper.dict_VaRDistribution.TryGetValue(ClientID, out var dict_values))
                {
                    foreach (var item in dict_values)
                    {
                        string Underlying = item.Key;
                        _VaRDistribution.dict_MaxValue[Underlying] = 0;

                        double _MaxNumb = 0;            //modified by navin on 27-09-2019
                        drVaR = dt_IndividualRangeVaR.NewRow();
                        drVaR["Underlying"] = Underlying;

                        for (int OGRange = -30; OGRange <= 30; OGRange++)
                        {
                            double _VaRNum = 0;

                            //changed to TryGetValue on 27MAY2021 by Amey
                            if (item.Value.TryGetValue(OGRange, out double _Range))
                                _VaRNum = DivideByBaseAndRound(_Range, nameof(CPParent.VAR));

                            //reversed sign on 18JAN2021 by Amey. To highlight Minimun Var number on VarDistribution grid. 
                            //Because Var numbers are not multiplied by -1 in this grid.
                            _MaxNumb = _MaxNumb == 0 ? _VaRNum : (_MaxNumb > _VaRNum ? _VaRNum : _MaxNumb);

                            drVaR[OGRange.ToString()] = _VaRNum;

                            if (dict_GraphValue.ContainsKey(OGRange))
                                dict_GraphValue[OGRange] += _VaRNum;
                            else
                                dict_GraphValue.Add(OGRange, _VaRNum);
                        }

                        dt_IndividualRangeVaR.Rows.Add(drVaR);
                        _VaRDistribution.dict_MaxValue[Underlying] = _MaxNumb;//added by Navin on 24-09-2019 for max number
                    }
                    foreach (KeyValuePair<int, double> item in dict_GraphValue)
                    {
                        _VaRDistribution.chart_VaRDistribution.Series["VaR"].Points.Add(new SeriesPoint(item.Key, item.Value));
                    }
                }

                _VaRDistribution.sbClient.Clear();
                _VaRDistribution.sbClient.Append(ClientID);

                _VaRDistribution.groupC_VarDistribution.Text = ClientID.ToString();//added by Navin on 30-08-2019

                _VaRDistribution.gc_VaRPortfolio.DataSource = dt_IndividualRangeVaR;
                _VaRDistribution.gv_VaRPortfolio.BestFitColumns();

                _VaRDistribution.chart_VaRDistribution.Update();

                //added on 28FEB2021 by Amey
                if (!isVaRDistributionOpen)
                {
                    isVaRDistributionOpen = true;

                    //added on 31MAR2021 by Amey
                    eve_IsVaRDistributionShown(true, ClientID);

                    _VaRDistribution.Show();
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        //Added by Akshay on 28-07-2021 for Client Window
        private void ClientWindow(string _ClientID, string _Underlying, string _Expiry)
        {
            try
            {
                if (!isClientWindowOpen)
                {
                    _ClientWindow = new form_ClientWindow();
                    _ClientWindow.eve_FormClosed += _ClientWindow_eve_FormClosed;
                    _ClientWindow.eve_ClientIDIndexChanged += _ClientWindow_eve_ClientIDIndexChanged;
                    _ClientWindow.eve_UnderlyingIndexChanged += _ClientWindow_eve_UnderlyingIndexChanged;
                    _ClientWindow.eve_ExpiryIndexChanged += _ClientWindow_eve_ExpiryIndexChanged;
                    eve_IsClientWindowShown(true);
                    eve_ClientWindowDataSend(_ClientID, _Underlying, _Expiry);


                    _ClientWindow.cbe_ClientID.Text = _ClientID;
                    _ClientWindow.cbe_Underlying.Text = _Underlying;
                    _ClientWindow.cbe_Expiry.Text = "ALL";

                    _ClientWindow.cbe_ClientID.Properties.Items.AddRange(CollectionHelper.dict_ComboUniverse.Keys.ToList());
                    _ClientWindow.cbe_Underlying.Properties.Items.AddRange(CollectionHelper.dict_ComboUniverse[_ClientID].Keys.ToList());
                    _ClientWindow.cbe_Expiry.Properties.Items.Add("ALL");
                    _ClientWindow.cbe_Expiry.Properties.Items.AddRange(CollectionHelper.dict_ComboUniverse[_ClientID][_Underlying].Keys.ToList());

                    rts_ClientWindow.DataSource = CollectionHelper.bList_ClientWindow;
                    _ClientWindow.gc_windows.DataSource = rts_ClientWindow;

                    rts_ClientWindowOptions.DataSource = CollectionHelper.bList_ClientWindowOptions;
                    _ClientWindow.gc_Options.DataSource = rts_ClientWindowOptions;

                    rts_ClientWindowFutures.DataSource = CollectionHelper.bList_ClientWindowFutures;
                    _ClientWindow.gc_Futures.DataSource = rts_ClientWindowFutures;

                    rts_ClientWindowGreeks.DataSource = CollectionHelper.bList_ClientWindowGreeks;
                    _ClientWindow.gc_Greeks.DataSource = rts_ClientWindowGreeks;

                    _ClientWindow.gv_Greeks.Columns[0].Visible = false;

                    
                }


                if (!isClientWindowOpen)
                {
                    isClientWindowOpen = true;
                    _ClientWindow.ShowDialog();
                }
            }
            catch (Exception ee)
            {
                _logger.Error(ee);
            }
        }

        private void _ClientWindow_eve_FormClosed()
        {
            isClientWindowOpen = false;
            eve_IsClientWindowShown(false);
            //CollectionHelper.dict_ClientWindowPositions.Clear();
            //CollectionHelper.bList_ClientWindow.Clear();
            //CollectionHelper.IsClear = true;
            CollectionHelper.bList_ClientWindow.Clear();
            CollectionHelper.bList_ClientWindowGreeks.Clear();
            CollectionHelper.bList_ClientWindowOptions.Clear();
            CollectionHelper.bList_ClientWindowFutures.Clear();

        }

        private void _ClientWindow_eve_ClientIDIndexChanged(string _ClientID, string _Underlying, string _Expiry)
        {
            _ClientWindow.cbe_Underlying.Properties.Items.AddRange(CollectionHelper.dict_ComboUniverse[_ClientID].Keys.ToList());
            eve_ClientWindowDataSend(_ClientID, _Underlying, _Expiry);
        }

        private void _ClientWindow_eve_UnderlyingIndexChanged(string _ClientID, string _Underlying, string _Expiry)
        {
            _ClientWindow.cbe_Expiry.Properties.Items.Add("ALL");
            _ClientWindow.cbe_Expiry.Properties.Items.AddRange(CollectionHelper.dict_ComboUniverse[_ClientID][_Underlying].Keys.ToList());
            eve_ClientWindowDataSend(_ClientID, _Underlying, _Expiry);
        }

        private void _ClientWindow_eve_ExpiryIndexChanged(string _ClientID, string _Underlying, string _Expiry)
        {
            eve_ClientWindowDataSend(_ClientID, _Underlying, _Expiry);
        }


        private void _VaRDistribution_eve_FormClosed(string ClientID)
        {
            isVaRDistributionOpen = false;

            //added on 31MAR2021 by Amey
            eve_IsVaRDistributionShown(false, ClientID);
        }

        private double DivideByBaseAndRound(double Value, string ColName)
        {
            return Math.Round(Value / CollectionHelper.dict_BaseValue[ColName], CollectionHelper.dict_CustomDigits[ColName]);
        }

        // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        private void AddExpressCol(object sender, EventArgs e)
        {
            try
            {

                var colName = XtraInputBox.Show("Enter Column Name", "Column Name", "Formula Column").Trim();
                var dict_ColumnNames = CollectionHelper.dict_CustomColumnNames;
                List<string> list_expressioncolName = new List<string>();

                foreach(var key in CollectionHelper.dict_ExpressionColumn.Keys)
                {
                    list_expressioncolName.AddRange(CollectionHelper.dict_ExpressionColumn[key].Keys.ToList());
                }

                if (!dict_ColumnNames.ContainsKey(colName) && !list_expressioncolName.Contains(colName))
                {
                    if (colName.Length != 0)
                    {
                        GridView view = RightClicked_ViewInfo;

                        GridColumn unbColumn = new GridColumn();
                        unbColumn = view.AddUnboundColumn();

                        if (unbColumn != null)
                        {
                            unbColumn.VisibleIndex = gv_ClientPortfolio.Columns.Count;
                            unbColumn.ShowUnboundExpressionMenu = true;
                            unbColumn.OptionsColumn.AllowEdit = false;
                            unbColumn.FieldName = colName;
                            unbColumn.Name = colName;
                            unbColumn.Caption = colName;
                            unbColumn.DisplayFormat.FormatType = FormatType.Numeric;
                            unbColumn.DisplayFormat.FormatString = "N2";
                            unbColumn.AppearanceCell.BackColor = Color.LightBlue;

                            var pattern = @"(?<=\[)[^]]+(?=\])";
                            var isNumericCol = true;
                            foreach (Match match in Regex.Matches(unbColumn.UnboundExpression, pattern))
                            {
                                if (!CollectionHelper.list_DecimalColumns.Contains(match.Value))
                                    isNumericCol = false;
                            }

                            if (isNumericCol)
                                unbColumn.UnboundDataType = typeof(double);

                            if (CollectionHelper.dict_ExpressionColumn.ContainsKey(view.DetailLevel))
                                CollectionHelper.dict_ExpressionColumn[view.DetailLevel].Add(colName, unbColumn.UnboundExpression);
                            else
                            {
                                CollectionHelper.dict_ExpressionColumn.Add(view.DetailLevel, new Dictionary<string, string>());
                                CollectionHelper.dict_ExpressionColumn[view.DetailLevel].Add(colName, unbColumn.UnboundExpression);
                            }

                            File.WriteAllText(expressionfileloc, JsonConvert.SerializeObject(CollectionHelper.dict_ExpressionColumn));
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("Column Name already present", "Error");
                }
            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        private void DeleteExpressionCol(object sender, EventArgs e)            //Added by Snehadri on 
        {
            try
            {
                List<string> list_expressioncolName = new List<string>();

                foreach (var key in CollectionHelper.dict_ExpressionColumn.Keys)
                {
                    list_expressioncolName.AddRange(CollectionHelper.dict_ExpressionColumn[key].Keys.ToList());
                }

                string colName = RightClicked_ColumnName;
                GridView view = RightClicked_ViewInfo;


                if (!list_expressioncolName.Contains(colName))
                {
                    XtraMessageBox.Show("Cannot Delete This Column", "Error");
                }
                else
                {
                    CollectionHelper.dict_ExpressionColumn[view.DetailLevel].Remove(colName);
                    view.Columns[colName].Dispose();
                    
                    File.WriteAllText(expressionfileloc, JsonConvert.SerializeObject(CollectionHelper.dict_ExpressionColumn));

                    if (CollectionHelper.dict_RuleInfo.Count > 0)
                    {
                        var remove_key = CollectionHelper.dict_RuleInfo.Where(v => v.Value.ColumnName == colName).Select(v => v.Key).ToList();

                        if (remove_key.Count > 0)
                        {
                            foreach (var key in remove_key)
                            {
                                CollectionHelper.dict_RuleInfo.Remove(key);
                            }
                            
                            File.WriteAllText(rulefileloc, JsonConvert.SerializeObject(CollectionHelper.dict_RuleInfo));

                        }

                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        private void gv_ClientPortfolio_ColumnUnboundExpressionChanged(object sender, ColumnEventArgs e)
        {
            try
            {
                GridView view = RightClicked_ViewInfo;
                string colname = e.Column.FieldName;
                string expression = e.Column.UnboundExpression;
                if (CollectionHelper.dict_ExpressionColumn.ContainsKey(view.DetailLevel))
                {
                    if (CollectionHelper.dict_ExpressionColumn[view.DetailLevel].ContainsKey(colname))
                    {
                        CollectionHelper.dict_ExpressionColumn[view.DetailLevel][colname] = e.Column.UnboundExpression;
                        File.WriteAllText(expressionfileloc, JsonConvert.SerializeObject(CollectionHelper.dict_ExpressionColumn));
                    }

                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_Underlying_ColumnUnboundExpressionChanged(object sender, ColumnEventArgs e)
        {
            try
            {
                GridView view = RightClicked_ViewInfo;
                string colname = e.Column.FieldName;
                string expression = e.Column.UnboundExpression;
                if (CollectionHelper.dict_ExpressionColumn.ContainsKey(view.DetailLevel))
                {
                    if (CollectionHelper.dict_ExpressionColumn[view.DetailLevel].ContainsKey(colname))
                    {
                        CollectionHelper.dict_ExpressionColumn[view.DetailLevel][colname] = e.Column.UnboundExpression;
                        File.WriteAllText(expressionfileloc, JsonConvert.SerializeObject(CollectionHelper.dict_ExpressionColumn));
                    }

                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
        private void Menu_RuleManger(object sender, EventArgs e)
        {
            try
            {
                form_RuleBuilder _RuleBuilder = new form_RuleBuilder();

                if (!isRuleManagerOpen)
                {
                    isRuleManagerOpen = true;
                    
                    _RuleBuilder.eve_RuleManagerFormClosed += _RuleManagerFormClosed;

                    List<string> clientlist = dict_ClientPositions.Keys.ToList();
                    List<string> list_column = new List<string>();
                    foreach (GridColumn column in RightClicked_ViewInfo.VisibleColumns)
                    {
                        list_column.Add(column.FieldName);
                    }

                    _RuleBuilder.GetRuleInfo(clientlist, list_column, RightClicked_ViewInfo.DetailLevel);

                    _RuleBuilder.Show();

                }
                else
                {
                    Application.OpenForms[_RuleBuilder.Name].Focus();
                    
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void _RuleManagerFormClosed()
        {
            try
            {
                isRuleManagerOpen = false;
            }
            catch(Exception ee) { _logger.Error(ee); }
        }


        //updated on 25MAR2022
        private void CompareandAlert(DataTable dt_Gridviewinfo, DataTable dt_Underlyings)
        {
            try
            {
                isComparing = true;

                Stopwatch sw_timetaken = new Stopwatch();
                if (CollectionHelper.IsDebug)
                    sw_timetaken.Start();

                StringBuilder sb_RuleAlert = new StringBuilder();

                if (popup_snooze > 0)
                {
                    List<string> clientlist = dict_ClientPositions.Keys.ToList();
                    foreach (var ClientID in clientlist)
                    {

                    }
                }

                Dictionary<string, RuleAlert> dict_RuleInfo = new Dictionary<string, RuleAlert>(CollectionHelper.dict_RuleInfo);

                foreach (var RuleName in dict_RuleInfo.Keys)
                {
                    DataTable dt_temp = new DataTable();

                    RuleAlert ruleAlert = new RuleAlert();

                    ruleAlert = dict_RuleInfo[RuleName];
                    string colname = ruleAlert.ColumnName;

                    if (ruleAlert.detail_level == 1)
                    {
                        dt_temp = dt_Underlyings;
                    }
                    else if (ruleAlert.detail_level == 0)
                    {
                        dt_temp = dt_Gridviewinfo;
                    }

                    for (int i = 0; i < dt_temp.Rows.Count; i++)
                    {
                        string ClientID = dt_temp.Rows[i]["ClientID"].ToString();
                        double cellvalue = Convert.ToDouble(dt_temp.Rows[i][colname]);
                        string underlying = string.Empty;
                        bool rulesatisfied = false;
                        //Added by Avinash on 22MAR2022
                        string Name = dt_temp.Rows[i]["Name"].ToString();

                        if (!ruleAlert.list_Clients.Contains(ClientID))
                            continue;

                        if (dt_temp.Columns.Contains("Underlying"))
                        {
                            underlying = dt_temp.Rows[i]["Underlying"].ToString();
                            if (!ruleAlert.arr_Scrips.Contains(underlying))
                                continue;
                        }

                        int count = 0;

                        foreach (var Group in ruleAlert.list_Groups)
                        {
                            bool rule1 = false; bool rule2 = false; count += 1;

                            if (double.IsNaN(Group.Value1))
                                continue;

                            switch (Group.Op1)
                            {
                                case "<":
                                    if (!double.IsNaN(Group.Value1)) { if (cellvalue < Group.Value1) { rule1 = true; } };
                                    break;
                                case ">":
                                    if (!double.IsNaN(Group.Value1)) { if (cellvalue > Group.Value1) { rule1 = true; } };
                                    break;
                                case "==":
                                    if (!double.IsNaN(Group.Value1)) { if (cellvalue == Group.Value1) { rule1 = true; } };
                                    break;
                                case ">=":
                                    if (!double.IsNaN(Group.Value1)) { if (cellvalue >= Group.Value1) { rule1 = true; } };
                                    break;
                                case "<=":
                                    if (!double.IsNaN(Group.Value1)) { if (cellvalue <= Group.Value1) { rule1 = true; } };
                                    break;
                                default:
                                    break;
                            }

                            switch (Group.Op2)
                            {
                                case "<":
                                    if (!double.IsNaN(Group.Value2)) { if (cellvalue < Group.Value2) { rule2 = true; } };
                                    break;
                                case ">":
                                    if (!double.IsNaN(Group.Value2)) { if (cellvalue > Group.Value2) { rule2 = true; } };
                                    break;
                                case "==":
                                    if (!double.IsNaN(Group.Value2)) { if (cellvalue == Group.Value2) { rule2 = true; } };
                                    break;
                                case ">=":
                                    if (!double.IsNaN(Group.Value2)) { if (cellvalue >= Group.Value2) { rule2 = true; } };
                                    break;
                                case "<=":
                                    if (!double.IsNaN(Group.Value2)) { if (cellvalue <= Group.Value2) { rule2 = true; } };
                                    break;
                                default:
                                    break;
                            }

                            switch (Group.LogicalOp)
                            {
                                case "AND":
                                    if ((rule1 == true) && (rule2 == true))
                                    {


                                        if (Group.AlertMessage != "")
                                        {
                                            if (underlying != string.Empty)
                                                sb_RuleAlert.Append(DateTime.Now + ": " + Group.AlertMessage + $" in Rule {RuleName} for Client {ClientID} \"{Name}\" and {colname} value {cellvalue} for underlying {underlying}.\n");
                                            else
                                                sb_RuleAlert.Append(DateTime.Now + ": " + Group.AlertMessage + $" in Rule {RuleName} for Client {ClientID} \"{Name}\" and {colname} value {cellvalue}.\n");
                                        }
                                        else
                                        {
                                            if (underlying != string.Empty)
                                                sb_RuleAlert.Append(DateTime.Now + ": " + $"Breach Occurred in Rule {RuleName} in Group {count} for Client {ClientID} \"{Name}\" and {colname} value {cellvalue} for underlying {underlying}.\n");
                                            else
                                                sb_RuleAlert.Append(DateTime.Now + ": " + $"Breach Occurred in Rule {RuleName} in Group {count} for Client {ClientID} \"{Name}\" and {colname} value {cellvalue}.\n");
                                        }
                                        rulesatisfied = true;
                                    }
                                    break;

                                case "OR":
                                    if ((rule1 == true) || (rule2 == true))
                                    {

                                        if (Group.AlertMessage != "")
                                        {
                                            if (underlying != string.Empty)
                                                sb_RuleAlert.Append(DateTime.Now + ": " + Group.AlertMessage + $" in Rule {RuleName} for Client {ClientID} \"{Name}\" and {colname} value {cellvalue} for underlying {underlying}.\n");
                                            else
                                                sb_RuleAlert.Append(DateTime.Now + ": " + Group.AlertMessage + $" in Rule {RuleName} for Client {ClientID} \"{Name}\" and {colname} value {cellvalue}.\n");
                                        }
                                        else
                                        {
                                            if (underlying != string.Empty)
                                                sb_RuleAlert.Append(DateTime.Now + ": " + $"Breach Occurred in Rule {RuleName} in Group {count} for Client {ClientID} \"{Name}\" and {colname} value {cellvalue} for underlying {underlying}.\n");
                                            else
                                                sb_RuleAlert.Append(DateTime.Now + ": " + $"Breach Occurred in Rule {RuleName} in Group {count} for Client {ClientID} \"{Name}\" and {colname} value {cellvalue}.\n");
                                        }
                                        rulesatisfied = true;
                                    }
                                    break;

                                default:
                                    if (rule1 == true)
                                    {//

                                        if (Group.AlertMessage != "")
                                        {
                                            if (underlying != string.Empty)
                                                sb_RuleAlert.Append(DateTime.Now + ": " + Group.AlertMessage + $" in Rule {RuleName} for Client {ClientID} \"{Name}\" and {colname} value {cellvalue} for underlying {underlying}.\n");
                                            else
                                                sb_RuleAlert.Append(DateTime.Now + ": " + Group.AlertMessage + $" in Rule {RuleName} for Client {ClientID} \"{Name}\" and {colname} value {cellvalue}.\n");
                                        }
                                        else
                                        {
                                            if (underlying != string.Empty)
                                                sb_RuleAlert.Append(DateTime.Now + ": " + $"Breach Occurred in Rule {RuleName} in Group {count} for Client {ClientID} \"{Name}\" and {colname} value {cellvalue} for underlying {underlying}.\n");
                                            else
                                                sb_RuleAlert.Append(DateTime.Now + ": " + $"Breach Occurred in Rule {RuleName} in Group {count} for Client {ClientID} \"{Name}\" and {colname} value {cellvalue}.\n");
                                        }
                                        rulesatisfied = true;
                                    }
                                    break;
                            }
                        }

                        if (popup_snooze > 0 && rulesatisfied)
                        {
                            string key = ClientID + "^" + RuleName;

                            if (dict_ClinetAlertTime.ContainsKey(key))
                            {
                                var time_diff = Math.Round((DateTime.Now).Subtract(dict_ClinetAlertTime[key]).TotalMinutes);
                                if (time_diff >= popup_snooze)
                                    dict_ClinetAlertTime[key] = DateTime.Now;
                            }
                            else
                                dict_ClinetAlertTime.Add(key, DateTime.Now);
                        }
                    }

                }
                if (sb_RuleAlert.Length > 0)
                {
                    uc_Violations.Instance.Invoke((MethodInvoker)(() => uc_Violations.Instance.RuleAlert(sb_RuleAlert.ToString())));
                    PopupAlert(sb_RuleAlert.ToString());
                }

                dt_Gridviewinfo.Clear();
                dt_Underlyings.Clear();
                sb_RuleAlert.Clear();

                if (CollectionHelper.IsDebug)
                {
                    sw_timetaken.Stop();
                    _logger.Debug($"Time taken for comparing rule: {sw_timetaken.ElapsedMilliseconds} ms");
                }

                isComparing = false;

            }
            catch (Exception ee) { _logger.Error(ee); }
        }




        // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        //private void CompareandAlert(DataTable dt_Gridviewinfo, DataTable dt_Underlyings)
        //{
        //    try
        //    {
        //        isComparing = true;

        //        Stopwatch sw_timetaken = new Stopwatch();
        //        if(CollectionHelper.IsDebug)
        //            sw_timetaken.Start();

        //        StringBuilder sb_RuleAlert = new StringBuilder();

        //        if (popup_snooze > 0)
        //        {
        //            List<string> clientlist = dict_ClientPositions.Keys.ToList();
        //            foreach (var ClientID in clientlist)
        //            {

        //            }
        //        }

        //        Dictionary<string, RuleAlert> dict_RuleInfo = new Dictionary<string, RuleAlert>(CollectionHelper.dict_RuleInfo);

        //        foreach (var RuleName in dict_RuleInfo.Keys)
        //        {
        //            DataTable dt_temp = new DataTable();

        //            RuleAlert ruleAlert = new RuleAlert();

        //            ruleAlert = dict_RuleInfo[RuleName];
        //            string colname = ruleAlert.ColumnName;

        //            if (ruleAlert.detail_level == 1)
        //            {
        //                dt_temp = dt_Underlyings;
        //            }
        //            else if (ruleAlert.detail_level == 0)
        //            {
        //                dt_temp = dt_Gridviewinfo;
        //            }

        //            for (int i = 0; i < dt_temp.Rows.Count; i++)
        //            {
        //                string ClientID = dt_temp.Rows[i]["ClientID"].ToString();
        //                double cellvalue = Convert.ToDouble(dt_temp.Rows[i][colname]);
        //                string underlying = string.Empty;
        //                bool rulesatisfied = false;

        //                if (!ruleAlert.list_Clients.Contains(ClientID))
        //                    continue;

        //                if (dt_temp.Columns.Contains("Underlying"))
        //                {
        //                    underlying = dt_temp.Rows[i]["Underlying"].ToString();
        //                    if (!ruleAlert.arr_Scrips.Contains(underlying))
        //                        continue;
        //                }

        //                int count = 0;

        //                foreach (var Group in ruleAlert.list_Groups)
        //                {
        //                    bool rule1 = false; bool rule2 = false; count += 1;

        //                    if (double.IsNaN(Group.Value1))
        //                        continue;

        //                    switch (Group.Op1)
        //                    {
        //                        case "<":
        //                            if (!double.IsNaN(Group.Value1)) { if (cellvalue < Group.Value1) { rule1 = true; } };
        //                            break;
        //                        case ">":
        //                            if (!double.IsNaN(Group.Value1)) { if (cellvalue > Group.Value1) { rule1 = true; } };
        //                            break;
        //                        case "==":
        //                            if (!double.IsNaN(Group.Value1)) { if (cellvalue == Group.Value1) { rule1 = true; } };
        //                            break;
        //                        case ">=":
        //                            if (!double.IsNaN(Group.Value1)) { if (cellvalue >= Group.Value1) { rule1 = true; } };
        //                            break;
        //                        case "<=":
        //                            if (!double.IsNaN(Group.Value1)) { if (cellvalue <= Group.Value1) { rule1 = true; } };
        //                            break;
        //                        default:
        //                            break;
        //                    }

        //                    switch (Group.Op2)
        //                    {
        //                        case "<":
        //                            if (!double.IsNaN(Group.Value2)) { if (cellvalue < Group.Value2) { rule2 = true; } };
        //                            break;
        //                        case ">":
        //                            if (!double.IsNaN(Group.Value2)) { if (cellvalue > Group.Value2) { rule2 = true; } };
        //                            break;
        //                        case "==":
        //                            if (!double.IsNaN(Group.Value2)) { if (cellvalue == Group.Value2) { rule2 = true; } };
        //                            break;
        //                        case ">=":
        //                            if (!double.IsNaN(Group.Value2)) { if (cellvalue >= Group.Value2) { rule2 = true; } };
        //                            break;
        //                        case "<=":
        //                            if (!double.IsNaN(Group.Value2)) { if (cellvalue <= Group.Value2) { rule2 = true; } };
        //                            break;
        //                        default:
        //                            break;
        //                    }

        //                    switch (Group.LogicalOp)
        //                    {
        //                        case "AND":
        //                            if ((rule1 == true) && (rule2 == true))
        //                            {


        //                                if (Group.AlertMessage != "")
        //                                {
        //                                    if(underlying != string.Empty)
        //                                        sb_RuleAlert.Append(DateTime.Now + ": " + Group.AlertMessage + $" in Rule {RuleName} for Client {ClientID} and {colname} value {cellvalue} for underlying {underlying}.\n");
        //                                    else
        //                                        sb_RuleAlert.Append(DateTime.Now + ": " + Group.AlertMessage + $" in Rule {RuleName} for Client {ClientID} and {colname} value {cellvalue}.\n");
        //                                }
        //                                else
        //                                {
        //                                    if (underlying != string.Empty)
        //                                        sb_RuleAlert.Append(DateTime.Now + ": " + $"Breach Occurred in Rule {RuleName} in Group {count} for Client {ClientID} and {colname} value {cellvalue} for underlying {underlying}.\n");
        //                                    else
        //                                        sb_RuleAlert.Append(DateTime.Now + ": " + $"Breach Occurred in Rule {RuleName} in Group {count} for Client {ClientID} and {colname} value {cellvalue}.\n");
        //                                }
        //                                rulesatisfied = true;
        //                            }
        //                            break;

        //                        case "OR":
        //                            if ((rule1 == true) || (rule2 == true))
        //                            {

        //                                if (Group.AlertMessage != "")
        //                                {
        //                                    if (underlying != string.Empty)
        //                                        sb_RuleAlert.Append(DateTime.Now + ": " + Group.AlertMessage + $" in Rule {RuleName} for Client {ClientID} and {colname} value {cellvalue} for underlying {underlying}.\n");
        //                                    else
        //                                        sb_RuleAlert.Append(DateTime.Now + ": " + Group.AlertMessage + $" in Rule {RuleName} for Client {ClientID} and {colname} value {cellvalue}.\n");
        //                                }
        //                                else
        //                                {
        //                                    if (underlying != string.Empty)
        //                                        sb_RuleAlert.Append(DateTime.Now + ": " + $"Breach Occurred in Rule {RuleName} in Group {count} for Client {ClientID} and {colname} value {cellvalue} for underlying {underlying}.\n");
        //                                    else
        //                                        sb_RuleAlert.Append(DateTime.Now + ": " + $"Breach Occurred in Rule {RuleName} in Group {count} for Client {ClientID} and {colname} value {cellvalue}.\n");
        //                                }
        //                                rulesatisfied = true;
        //                            }
        //                            break;

        //                        default:
        //                            if (rule1 == true)
        //                            {

        //                                if (Group.AlertMessage != "")
        //                                {
        //                                    if (underlying != string.Empty)
        //                                        sb_RuleAlert.Append(DateTime.Now + ": " + Group.AlertMessage + $" in Rule {RuleName} for Client {ClientID} and {colname} value {cellvalue} for underlying {underlying}.\n");
        //                                    else
        //                                        sb_RuleAlert.Append(DateTime.Now + ": " + Group.AlertMessage + $" in Rule {RuleName} for Client {ClientID} and {colname} value {cellvalue}.\n");
        //                                }
        //                                else
        //                                {
        //                                    if (underlying != string.Empty)
        //                                        sb_RuleAlert.Append(DateTime.Now + ": " + $"Breach Occurred in Rule {RuleName} in Group {count} for Client {ClientID} and {colname} value {cellvalue} for underlying {underlying}.\n");
        //                                    else
        //                                        sb_RuleAlert.Append(DateTime.Now + ": " + $"Breach Occurred in Rule {RuleName} in Group {count} for Client {ClientID} and {colname} value {cellvalue}.\n");
        //                                }
        //                                rulesatisfied = true;
        //                            }
        //                            break;
        //                    }
        //                }

        //                if (popup_snooze > 0 && rulesatisfied)
        //                {
        //                    string key = ClientID + "^" + RuleName;

        //                    if (dict_ClinetAlertTime.ContainsKey(key))
        //                    {
        //                        var time_diff = Math.Round((DateTime.Now).Subtract(dict_ClinetAlertTime[key]).TotalMinutes);
        //                        if (time_diff >= popup_snooze)
        //                            dict_ClinetAlertTime[key] = DateTime.Now;
        //                    }
        //                    else
        //                        dict_ClinetAlertTime.Add(key, DateTime.Now);
        //                }
        //            }

        //        }
        //        if (sb_RuleAlert.Length > 0)
        //        {
        //            uc_Violations.Instance.Invoke((MethodInvoker)(() => uc_Violations.Instance.RuleAlert(sb_RuleAlert.ToString())));
        //            PopupAlert(sb_RuleAlert.ToString());
        //        }

        //        dt_Gridviewinfo.Clear();
        //        dt_Underlyings.Clear();
        //        sb_RuleAlert.Clear();

        //        if (CollectionHelper.IsDebug)
        //        {
        //            sw_timetaken.Stop();
        //            _logger.Debug($"Time taken for comparing rule: {sw_timetaken.ElapsedMilliseconds} ms");
        //        }

        //        isComparing = false;

        //    }
        //    catch (Exception ee) { _logger.Error(ee); }
        //}

        // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        private void timer_Rule_Tick(object sender, EventArgs e)
        {
            try
            {

                if (!isComparing && ( gv_ClientPortfolio.RowCount > 0 && CollectionHelper.dict_RuleInfo.Count > 0))
                {
                    ComputeforRule.StartCompute = true;


                    DataTable dt_Gridviewinfo = new DataTable();

                    foreach (GridColumn column in gv_ClientPortfolio.VisibleColumns)
                    {
                        if(column != null)
                        {
                            dt_Gridviewinfo.Columns.Add(column.FieldName);
                        }
                        
                    }

                    for (int i = 0; i < gv_ClientPortfolio.DataRowCount; i++)
                    {
                        DataRow row = dt_Gridviewinfo.NewRow();
                        foreach (GridColumn column in gv_ClientPortfolio.VisibleColumns)
                        {
                            if(column != null && gv_ClientPortfolio.GetRowCellValue(i, column) != null)
                            {
                                row[column.FieldName] = gv_ClientPortfolio.GetRowCellValue(i, column);
                            }
                        }
                        dt_Gridviewinfo.Rows.Add(row);
                    }

                    BindingList<CPParent> blist_Client = new BindingList<CPParent>(CollectionHelper.bList_ClientPortfolio);
                    List<CPUnderlying> list_underlying = new List<CPUnderlying>();

                    foreach(var _CPParent in blist_Client)
                    {
                        list_underlying.AddRange(_CPParent.bList_Underlying);
                    }

                    DataTable dt_Underlyings = new DataTable();

                    PropertyInfo[] Props = typeof(CPUnderlying).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach(var prop in Props)
                    {
                        dt_Underlyings.Columns.Add(prop.Name, prop.PropertyType);
                    }
                    
                    foreach(var item in list_underlying)
                    {
                        var values = new object[Props.Length];
                        for(int i=0; i<Props.Length; i++)
                        {
                            values[i] = Props[i].GetValue(item, null);
                        }
                        dt_Underlyings.Rows.Add(values);
                    }

                    Task.Run(() => CompareandAlert(dt_Gridviewinfo,dt_Underlyings));
                   
                }
            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        private void PopupAlert(string text)
        {
            try
            {
                List<string> list_ClientList = dict_ClinetAlertTime.Keys.ToList();

                List<string> list_alerts = text.Split('\n').Reverse().ToList();
                list_alerts.RemoveAt(0);

                if (popup_snooze > 0)
                {
                    foreach (var key in list_ClientList)
                    {
                        string clientid = key.Split('^')[0];
                        string rulename = key.Split('^')[1];

                        for (int i = 0; i < list_alerts.Count; i++)
                        {
                            if (list_alerts[i].Contains(clientid) && list_alerts[i].Contains(rulename))
                            {

                                var alert_time = DateTime.Parse(Regex.Split(list_alerts[i], ": ")[0]);
                                var time_diff = Math.Round((alert_time).Subtract(dict_ClinetAlertTime[key]).TotalSeconds);

                                if (time_diff > (rule_timeinterval / 2))
                                    list_alerts.Remove(list_alerts[i]);
                            }
                        }
                    }
                }

                PopupNotifier popup = new PopupNotifier();
                popup.TitleText = "BREACH ALERT";
                popup.TitlePadding = new Padding(10);
                popup.TitleFont = new Font("Tahoma", 12F);
                popup.TitleColor = Color.DarkGray;


                popup.ContentColor = Color.Black;
                popup.ContentPadding = new Padding(10, 5, 5, 5);
                popup.ContentFont = new Font("Tahoma", 10.5F);
                popup.ContentHoverColor = Color.Black;

                popup.HeaderColor = Color.Black;
                popup.HeaderHeight = 20;

                popup.BodyColor = Color.Transparent;

                                
                popup.Scroll = true;
                popup.ShowGrip = false;
                popup.ShowCloseButton = true;

                popup.Image = Properties.Resources.icon;
                popup.ImagePadding = new Padding(7);
                popup.ImageSize = new Size(25, 25);

                popup.Delay = 3000;

                if (list_alerts.Count > 0)
                {
                    if (list_alerts.Count <= 3)
                    {
                        popup.Size = new Size(400, 300);
                        popup.ContentText = string.Join(Environment.NewLine + Environment.NewLine, list_alerts);
                        Invoke((MethodInvoker)(() => {
                            popup.Popup();
                            SystemSounds.Beep.Play();
                        }));
                    }
                    else
                    {
                        popup.Size = new Size(400, 150);
                        popup.ContentText = "Multiple breaches occured. Please check the violation tab.";
                        Invoke((MethodInvoker)(() => {
                            popup.Popup();
                            SystemSounds.Beep.Play();
                        }));

                    }
                }
                                
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
               
        //Added by Akshay on 28-07 for window
        private void gv_CPUnderlying_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (e.Column.FieldName == nameof(_TempCPUnderlying.Analysis))
            {
                GridView gv_CPUnderlying = sender as GridView;

                string _ClientID = gv_CPUnderlying.GetFocusedRowCellValue(nameof(_TempCPUnderlying.ClientID)).ToString();
                string _Underlying = gv_CPUnderlying.GetFocusedRowCellValue(nameof(_TempCPUnderlying.Underlying)).ToString();
                string _Expiry = "ALL";

                ClientWindow(_ClientID, _Underlying, _Expiry);

            }
            else { return; }
        }

        private void gv_CPUnderlying_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                //added on 18NOV2020 by Amey. To avoid NullRefExceptions.
                if (e is null) return;

                if (e.Column.FieldName == nameof(_TempCPUnderlying.Analysis))
                    e.RepositoryItem = gc_ClientPortfolio.RepositoryItems["repBtn_ClientWindow"];
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        #endregion

        // Added by NIhil on 27DEC2021
        private void gv_ClientPortfolio_UnboundExpressionEditorCreated(object sender, UnboundExpressionEditorEventArgs e)
        {
            try
            {
                ExpressionEditorView editorView = (ExpressionEditorView)e.ExpressionEditorView;
                editorView.Text = "Formula Editor";
            }
            catch(Exception ee) { _logger.Error(ee); }
        }

        private void gv_ClientPortfolio_EndSorting(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception) { }
        }

        private void gv_ClientPortfolio_CustomDrawRowFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            try
            {
                e.Info.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
                e.Info.SummaryItem.DisplayFormat = "{0:N2}";
                
                //e.Info.DisplayText = e.Info.SummaryItem.GetFormatDisplayText("{0:N2}", e.Info.SummaryItem.SummaryValue);

            }
            catch (Exception ee) { _logger.Error(ee); }
        }
    }
}
