using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using n.Structs;
using NerveLog;
using Newtonsoft.Json;
using Prime.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prime.UI
{
    public partial class uc_ScenarioAnalysis : XtraUserControl
    {
        public event del_IsStateChanged eve_ComputeScenarioClicked;

        NerveLogger _logger;

        /// <summary>
        /// Key : Stock Name | Value : [0] Stock Movement, [1] IV Movement. (in %)
        /// </summary>
        Dictionary<string, double[]> dict_ScenarioMovement = new Dictionary<string, double[]>();

        /// <summary>
        /// Key : ClientID | Value : List of Positions.
        /// </summary>
        Dictionary<string, List<ConsolidatedPositionInfo>> dict_AllPositions = new Dictionary<string, List<ConsolidatedPositionInfo>>();

        /// <summary>
        /// DataSource for gc_Scenarios
        /// </summary>
        List<Scenario> list_Scenarios = new List<Scenario>();

        /// <summary>
        /// Set true when Positions sent from Main.
        /// </summary>
        bool ArePositionsAvaialble = false;

        DXMenuItem menu_ExportClientAnalysisToCSV = new DXMenuItem();
        DXMenuItem menu_ExportUnderlyingAnalysisToCSV = new DXMenuItem();
        DXMenuItem menu_ExportPositionAnalysisToCSV = new DXMenuItem();
        DXMenuItem menu_SaveLayout = new DXMenuItem();

        /// <summary>
        /// Path to save ClientPorofolio Positions Grid Layout.
        /// </summary>
        readonly string layout_ScenarioAnalysis = Application.StartupPath + "\\Layout\\" + "ScenarioAnalysis.xml";

         /// <summary>
        /// Path to save ClientPorofolio Positions Grid Layout.
        /// </summary>
        readonly string layout_ScenarioAnalysisUnderlying = Application.StartupPath + "\\Layout\\" + "ScenarioAnalysisChildUnderlying.xml";

        /// <summary>
        /// Path to save ClientPorofolio Positions Grid Layout.
        /// </summary>
        readonly string layout_ScenarioAnalysisPosition = Application.StartupPath + "\\Layout\\" + "ScenarioAnalysisChildPosition.xml";

        /// <summary>
        /// Contails Column field names having String datatype.
        /// </summary>
        HashSet<string> hs_StringColumns = new HashSet<string>();

        /// <summary>
        /// Contains all the Datatables
        /// </summary>
        DataTable dt_ScenarioAnalysis; DataTable dt_ChildUnderlying; DataTable dt_ChildPosition;

        GridView view_popup;


        private uc_ScenarioAnalysis()
        {
            InitializeComponent();

            this._logger = CollectionHelper._logger;

            InitialiseUC();
        }

        #region Instance Initializing

        public static uc_ScenarioAnalysis Instance { get; private set; }

        public static uc_ScenarioAnalysis Initialise()
        {
            if (Instance is null)
                Instance = new uc_ScenarioAnalysis();

            return Instance;
        }

        #endregion

        #region UI Events

        private void cmb_Symbol_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                string SelectedSymbol = cmb_Symbol.SelectedItem.ToString();
                if (SelectedSymbol != "--SELECT--")
                {
                    if (!dict_ScenarioMovement.ContainsKey(SelectedSymbol))
                        dict_ScenarioMovement.Add(SelectedSymbol, new double[2] { 0, 0 });

                    txt_StockMovement.Text = (dict_ScenarioMovement[SelectedSymbol][0]).ToString();
                    txt_IVMovement.Text = (dict_ScenarioMovement[SelectedSymbol][1]).ToString();
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void txt_StockMovement_Leave(object sender, EventArgs e)
        {
            try
            {
                string SelectedSymbol = cmb_Symbol.SelectedItem.ToString();

                if (dict_ScenarioMovement.ContainsKey(SelectedSymbol))
                    dict_ScenarioMovement[SelectedSymbol][0] = Convert.ToDouble(txt_StockMovement.Text.Replace("%", ""));
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void txt_IVMovement_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string SelectedSymbol = cmb_Symbol.SelectedItem.ToString();

                if (dict_ScenarioMovement.ContainsKey(SelectedSymbol))
                    dict_ScenarioMovement[SelectedSymbol][1] = Convert.ToDouble(txt_IVMovement.Text.Replace("%", ""));
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void btn_CreateScenario_Click(object sender, EventArgs e)
        {
            try
            {
                if (list_Scenarios.Where(v => v.Name == txt_ScenarioName.Text).Any())
                {
                    XtraMessageBox.Show("Scenario with the exact same name is already added. Please assign unique name for each Scenario.", "Warning");
                    return;
                }

                if (list_Scenarios.Count == 10)
                {
                    XtraMessageBox.Show("Maximum number of Scenarios are already added. Please remove unused Scenarios from below and try again.", "Warning");
                    return;
                }

                List<ScenarioParams> list_ScenarioParams = new List<ScenarioParams>();
                foreach (var item in cmb_Symbol.Properties.Items)
                {
                    string Symbol = item.ToString();
                    if (Symbol == "--SELECT--") continue;

                    var StockMovement = 0.0;
                    var IVMovement = 0.0;

                    //changed to TryGetValue on 27MAY2021 by Amey
                    if (dict_ScenarioMovement.TryGetValue(Symbol, out double[] arr_Movement))
                    {
                        StockMovement = arr_Movement[0];
                        IVMovement = arr_Movement[1];
                    }

                    list_ScenarioParams.Add(new ScenarioParams()
                    {
                        Stock = item.ToString().Trim().ToUpper(),
                        StockJump = StockMovement,
                        IVJump = IVMovement
                    });
                }

                list_Scenarios.Add(new Scenario()
                {
                    Name = txt_ScenarioName.Text,
                    list_ScenarioParams = list_ScenarioParams
                });

                gv_Scenarios.RefreshData();
                gv_Scenarios.BestFitColumns();

                WriteToScenarioFile();

                dict_ScenarioMovement.Clear();
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void btn_ScenarioReset_Click(object sender, EventArgs e)
        {
            dict_ScenarioMovement.Clear();

            cmb_Symbol.SelectedIndex = 0;
            txt_StockMovement.Text = "0";
            txt_IVMovement.Text = "0";
        }

        private void gc_Scenarios_DataSourceChanged(object sender, EventArgs e)
        {
            Scenario _TempScenario = new Scenario();
            gv_Scenarios.Columns.ColumnByFieldName(nameof(_TempScenario.Name)).OptionsColumn.AllowEdit = false;
        }

        private void gc_Scenarios_ViewRegistered(object sender, DevExpress.XtraGrid.ViewOperationEventArgs e)
        {
            GridView view = (e.View as GridView);
            if (view.IsDetailView == false)
                return;

            view.BestFitColumns();

            ScenarioParams _TempScenarioParams = new ScenarioParams();
            view.Columns.ColumnByFieldName(nameof(_TempScenarioParams.Stock)).OptionsColumn.AllowEdit = false;

            view.CustomRowCellEdit += View_CustomRowCellEdit;
        }

        private void gv_Scenarios_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                string FocusedFieldName = e.Column.FieldName.ToUpper();
                if (FocusedFieldName == "UPDATE")
                    e.RepositoryItem = gc_Scenarios.RepositoryItems["repBtn_UpdateScenario"];
                else if (FocusedFieldName == "DELETE")
                    e.RepositoryItem = gc_Scenarios.RepositoryItems["repBtn_DeleteScenario"];
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void repBtn_UpdateScenario_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            WriteToScenarioFile();

            XtraMessageBox.Show("Updated successfully.", "Success");
        }

        private void repBtn_DeleteScenario_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (XtraMessageBox.Show("Do you really want to delete the Scenario?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Scenario _TempScenario = new Scenario();

                    var ScenarioName = gv_Scenarios.GetFocusedRowCellValue(nameof(_TempScenario.Name)).ToString();
                    var _Scenario = list_Scenarios.Where(v => v.Name == ScenarioName).First();
                    list_Scenarios.Remove(_Scenario);

                    gv_Scenarios.RefreshData();

                    WriteToScenarioFile();
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void trackBar_ScenarioAnalysis_EditValueChanged(object sender, EventArgs e)
        {
            gv_ScenarioAnalysis.RefreshData();
        }

        private async void btn_ComputeScenarios_Click(object sender, EventArgs e)
        {
            progressPanel_ComputeScenario.Visible = true;
            btn_ComputeScenarios.Enabled = false;

            try
            {
                //Added by Snehadri on 24JUN2021
                var ds_ScenarioAnalysis = new DataSet();
                dt_ScenarioAnalysis = new DataTable();

                dt_ScenarioAnalysis.Columns.Add("ClientID");

                //added on 25MAR2021 by Amey
                dt_ScenarioAnalysis.Columns.Add("Name"); // Added by Snehadri on 24JUN2021
                dt_ScenarioAnalysis.Columns.Add("Zone");
                dt_ScenarioAnalysis.Columns.Add("Branch");
                dt_ScenarioAnalysis.Columns.Add("Family");
                dt_ScenarioAnalysis.Columns.Add("Product");

                dt_ScenarioAnalysis.Columns.Add("Deposit", typeof(double));

                dt_ScenarioAnalysis.Columns.Add("FuturesMTM", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("OptionsMTM", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("EquityMTM", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("MTM", typeof(double));

                dt_ScenarioAnalysis.Columns.Add("IntradayFuturesMTM", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("IntradayOptionsMTM", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("IntradayEquityMTM", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("IntradayMTM", typeof(double));

                dt_ScenarioAnalysis.Columns.Add("CDFuturesMTM", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("CDOptionsMTM", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("CDMTM", typeof(double));

                dt_ScenarioAnalysis.Columns.Add("CDIntradayFuturesMTM", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("CDIntradayOptionsMTM", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("CDIntradayMTM", typeof(double));

                dt_ScenarioAnalysis.Columns.Add("MTMDifference", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("MTM%", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("MarginUtil", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("Margin%", typeof(double));

                foreach (var item in list_Scenarios)
                {
                    dt_ScenarioAnalysis.Columns.Add(item.Name, typeof(double));
                    dt_ScenarioAnalysis.Columns.Add("Deposit-" + item.Name, typeof(double));
                    dt_ScenarioAnalysis.Columns.Add(item.Name + " Bal%", typeof(double));
                    hs_StringColumns.Add(item.Name + " Bal%");
                }

                dt_ScenarioAnalysis.Columns.Add("Delta", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("Theta", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("Gamma", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("Vega", typeof(double));
                dt_ScenarioAnalysis.Columns.Add("DeltaAmount", typeof(double));

                //added on 06APR2021 by Amey
                hs_StringColumns.Add("ClientID");
                hs_StringColumns.Add("Name");       // Added by Snehadri on 24JUN2021
                hs_StringColumns.Add("Zone");
                hs_StringColumns.Add("Branch");
                hs_StringColumns.Add("Family");
                hs_StringColumns.Add("Product");
                hs_StringColumns.Add("MTM%");
                hs_StringColumns.Add("Margin%");

                // Added by Snehadri on 24JUN2021 for Client Position Details
                dt_ChildPosition = new DataTable();

                dt_ChildPosition.Columns.Add("ClientID");
                dt_ChildPosition.Columns.Add("Name");
                dt_ChildPosition.Columns.Add("Zone");
                dt_ChildPosition.Columns.Add("Branch");
                dt_ChildPosition.Columns.Add("Family");
                dt_ChildPosition.Columns.Add("Product");

                dt_ChildPosition.Columns.Add("Underlying");
                dt_ChildPosition.Columns.Add("ScripName");
                dt_ChildPosition.Columns.Add("ScripType");
                dt_ChildPosition.Columns.Add("InstrumentName");
                dt_ChildPosition.Columns.Add("ExpiryDate", typeof(DateTime));
                dt_ChildPosition.Columns.Add("Segment");

                dt_ChildPosition.Columns.Add("IntradayMTM", typeof(double));
                dt_ChildPosition.Columns.Add("MTM", typeof(double));

                dt_ChildPosition.Columns.Add("CDIntradayMTM", typeof(double));
                dt_ChildPosition.Columns.Add("CDMTM", typeof(double));

                foreach (var item in list_Scenarios)
                {
                    dt_ChildPosition.Columns.Add(item.Name, typeof(double));
                }

                dt_ChildPosition.Columns.Add("Delta", typeof(double));
                dt_ChildPosition.Columns.Add("Theta", typeof(double));
                dt_ChildPosition.Columns.Add("Gamma", typeof(double));
                dt_ChildPosition.Columns.Add("Vega", typeof(double));
                dt_ChildPosition.Columns.Add("DeltaAmount", typeof(double));
                dt_ChildPosition.Columns.Add("NetPosition", typeof(double));
                dt_ChildPosition.Columns.Add("StrikePrice", typeof(double));
                dt_ChildPosition.Columns.Add("IV", typeof(double));
                dt_ChildPosition.Columns.Add("StockPrice", typeof(double));


                // Added by Snehadri on 24JUN2021
                hs_StringColumns.Add("Underlying");
                hs_StringColumns.Add("ScripName");
                hs_StringColumns.Add("ScripType");
                hs_StringColumns.Add("InstrumentName");
                hs_StringColumns.Add("ExpiryDate");
                hs_StringColumns.Add("Segment");

                // Added by Snehadri on 22JUL2021 for Client Underlying Details
                dt_ChildUnderlying = new DataTable();

                dt_ChildUnderlying.Columns.Add("ClientID");
                dt_ChildUnderlying.Columns.Add("Name");
                dt_ChildUnderlying.Columns.Add("Zone");
                dt_ChildUnderlying.Columns.Add("Branch");
                dt_ChildUnderlying.Columns.Add("Family");
                dt_ChildUnderlying.Columns.Add("Product");

                dt_ChildUnderlying.Columns.Add("Underlying");

                dt_ChildUnderlying.Columns.Add("FuturesMTM", typeof(double));
                dt_ChildUnderlying.Columns.Add("OptionsMTM", typeof(double));
                dt_ChildUnderlying.Columns.Add("EquityMTM", typeof(double));
                dt_ChildUnderlying.Columns.Add("MTM", typeof(double));

                foreach (var item in list_Scenarios)
                {
                    dt_ChildUnderlying.Columns.Add(item.Name, typeof(double));
                }

                dt_ChildUnderlying.Columns.Add("IntradayFuturesMTM", typeof(double));
                dt_ChildUnderlying.Columns.Add("IntradayOptionsMTM", typeof(double));
                dt_ChildUnderlying.Columns.Add("IntradayEquityMTM", typeof(double));
                dt_ChildUnderlying.Columns.Add("IntradayMTM", typeof(double));

                dt_ChildUnderlying.Columns.Add("CDFuturesMTM", typeof(double));
                dt_ChildUnderlying.Columns.Add("CDOptionsMTM", typeof(double));
                dt_ChildUnderlying.Columns.Add("CDMTM", typeof(double));
                dt_ChildUnderlying.Columns.Add("CDIntradayFuturesMTM", typeof(double));
                dt_ChildUnderlying.Columns.Add("CDIntradayOptionsMTM", typeof(double));
                dt_ChildUnderlying.Columns.Add("CDIntradayMTM", typeof(double));


                dt_ChildUnderlying.Columns.Add("MTMDifference", typeof(double));
                dt_ChildUnderlying.Columns.Add("MTM%", typeof(double));
                dt_ChildUnderlying.Columns.Add("MarginUtil", typeof(double));
                dt_ChildUnderlying.Columns.Add("Margin%", typeof(double));

                dt_ChildUnderlying.Columns.Add("Delta", typeof(double));
                dt_ChildUnderlying.Columns.Add("Theta", typeof(double));
                dt_ChildUnderlying.Columns.Add("Gamma", typeof(double));
                dt_ChildUnderlying.Columns.Add("Vega", typeof(double));
                dt_ChildUnderlying.Columns.Add("DeltaAmount", typeof(double));

                await Task.Run(() => ComputeScenarioVaR(ref dt_ScenarioAnalysis, ref dt_ChildUnderlying, ref dt_ChildPosition));

                // Added by Snehadri on 22JUL2021 for Client Underlying Details
                ds_ScenarioAnalysis.Tables.AddRange(new DataTable[] { dt_ScenarioAnalysis ,dt_ChildUnderlying, dt_ChildPosition });
                DataColumn[] colarr_Underlying = new DataColumn[] { ds_ScenarioAnalysis.Tables[1].Columns["ClientID"], ds_ScenarioAnalysis.Tables[1].Columns["Underlying"] };
                DataColumn[] colarr_Positions = new DataColumn[] { ds_ScenarioAnalysis.Tables[2].Columns["ClientID"], ds_ScenarioAnalysis.Tables[2].Columns["Underlying"] };
                DataRelation UnderlyingPositionRelation = new DataRelation("UnderlyingPositionRelation", colarr_Underlying, colarr_Positions);
                ds_ScenarioAnalysis.Relations.Add(ds_ScenarioAnalysis.Tables[0].Columns["ClientID"], ds_ScenarioAnalysis.Tables[1].Columns["ClientID"]); 
                ds_ScenarioAnalysis.Relations.Add(UnderlyingPositionRelation);

                gv_ScenarioAnalysis.Columns.Clear();
                gc_ScenarioAnalysis.DataSource = null;
                gc_ScenarioAnalysis.DataSource = ds_ScenarioAnalysis.Tables[0];
                gc_ScenarioAnalysis.RefreshDataSource();


                //added on 24MAY2021 by Amey
                if (File.Exists(layout_ScenarioAnalysis))
                    gv_ScenarioAnalysis.RestoreLayoutFromXml(layout_ScenarioAnalysis);
                else
                    gv_ScenarioAnalysis.BestFitColumns();
            }
            catch (Exception ee) { _logger.Error(ee); }

            btn_ComputeScenarios.Enabled = true;
            progressPanel_ComputeScenario.Visible = false;
        }

        private void gc_ScenarioAnalysis_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                var dict_CustomColumnNames = CollectionHelper.dict_CustomColumnNames;
                var dict_CustomDigits = CollectionHelper.dict_CustomDigits;

                //added on 25MAR2021 by Amey
                for (int i = 0; i < gv_ScenarioAnalysis.Columns.Count; i++)
                {
                    string ColumnFieldName = gv_ScenarioAnalysis.Columns[i].FieldName;

                    //added on 08APR2021 by Amey
                    if (!hs_StringColumns.Contains(ColumnFieldName))
                    {
                        gv_ScenarioAnalysis.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;

                        //added on 15APR2021 by Amey. To display commas with Indian format.
                        gv_ScenarioAnalysis.Columns[i].DisplayFormat.Format = CultureInfo.CreateSpecificCulture("hi-IN").NumberFormat;
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_ScenarioAnalysis_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e is null) return;

                var FieldName = e.Column.FieldName;

                //added on 24MAY2021 by Amey
                string _DisplayFormat = "#0,0.00";
                
                int RoundDigit = 2;
                if (CollectionHelper.dict_CustomDigits.TryGetValue(FieldName, out RoundDigit))
                {
                    switch (RoundDigit)
                    {
                        case 3:
                            _DisplayFormat = "#0,0.000";
                            break;
                        case 4:
                            _DisplayFormat = "#0,0.0000";
                            break;
                    }
                }
                else
                    RoundDigit = 2;

                //added on 4JAN2021 by Amey
                if (!hs_StringColumns.Contains(FieldName))
                {
                    e.DisplayText = Math.Round(Convert.ToDecimal(e.Value) / CollectionHelper.dict_DisplayValues[trackBar_ScenarioAnalysis.Value], RoundDigit).ToString(_DisplayFormat, CultureInfo.CreateSpecificCulture("hi-IN"));
                }
                    
                else if (FieldName == "MTM%" || FieldName == "Margin%")
                    e.DisplayText = Math.Round(Convert.ToDecimal(e.Value), 2).ToString("#0.00") + "%";
                else if (FieldName.Contains("Bal%"))
                    e.DisplayText = Math.Round(Convert.ToDecimal(e.Value), 2).ToString("#0.00") + "%";
            }
            catch (Exception ee) { _logger.Error(ee, $"gv_ScenarioAnalysis_CustomColumnDisplayText : {e.Column.FieldName}|{e.Value}"); }
        }

        private void gv_ScenarioAnalysis_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            view_popup = new GridView(); view_popup = e.HitInfo.View;
            e.Menu?.Items.Add(menu_ExportClientAnalysisToCSV);
            e.Menu?.Items.Add(menu_SaveLayout);
        }

        // Added by Snehadri on 09JUL2021
        private void gv_Positions_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            view_popup = new GridView(); view_popup = e.HitInfo.View;
            e.Menu?.Items.Add(menu_ExportPositionAnalysisToCSV);
            e.Menu?.Items.Add(menu_SaveLayout);
        }

        // Added by Snehadri on 22JUL2021 for Client Underlying Details
        private void gv_Underlying_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            view_popup = new GridView(); view_popup = e.HitInfo.View;
            e.Menu?.Items.Add(menu_ExportUnderlyingAnalysisToCSV);
            e.Menu?.Items.Add(menu_SaveLayout);
        }

        private void gridView_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            try
            {
                if (e is null) return;

                CommonFunctions.gridView_CustomDrawFooterCell(sender, e);

                var FieldName = e.Column.FieldName;

                //added on 24MAY2021 by Amey
                string _DisplayFormat = "#0,0.00";
                int RoundDigit = 2;
                if (CollectionHelper.dict_CustomDigits.TryGetValue(FieldName, out RoundDigit))
                {
                    switch (RoundDigit)
                    {
                        case 3:
                            _DisplayFormat = "#0,0.000";
                            break;
                        case 4:
                            _DisplayFormat = "#0,0.0000";
                            break;
                    }
                }
                else
                    RoundDigit = 2;

                //added on 4JAN2021 by Amey
                if (!hs_StringColumns.Contains(FieldName))
                {
                    e.Info.DisplayText = Math.Round(Convert.ToDecimal(e.Info.Value) / CollectionHelper.dict_DisplayValues[trackBar_ScenarioAnalysis.Value], RoundDigit).ToString(_DisplayFormat, CultureInfo.CreateSpecificCulture("hi-IN"));                   
                }
            }
            catch(Exception ee) { _logger.Error(ee); }
        }

        // Added by Snehadri on 09JUL2021
        private void gc_ScenarioAnalysis_ViewRegistered(object sender, ViewOperationEventArgs e)
        {
            try
            {
                GridView view = (e.View as GridView);
                if (view.IsDetailView == false)
                    return;
                // Added by Snehadri on 22JUL2021 for Client Underlying Details
                if (view.DetailLevel == 1)
                {
                    try
                    {
                        view.CustomColumnDisplayText -= gv_Underlying_CustomColumnDisplayText;
                        view.PopupMenuShowing -= gv_Underlying_PopupMenuShowing;
                        view.CustomDrawFooterCell -= gridView_CustomDrawFooterCell;
                    }
                    catch (Exception) { }

                    view.CustomColumnDisplayText += gv_Underlying_CustomColumnDisplayText;
                    view.PopupMenuShowing += gv_Underlying_PopupMenuShowing;
                    view.CustomDrawFooterCell += gridView_CustomDrawFooterCell;

                    if (File.Exists(layout_ScenarioAnalysisUnderlying))
                        view.RestoreLayoutFromXml(layout_ScenarioAnalysisUnderlying);
                }
                else if (view.DetailLevel == 2)
                {
                    try
                    {
                        view.CustomColumnDisplayText -= gv_Positions_CustomColumnDisplayText;
                        view.PopupMenuShowing -= gv_Positions_PopupMenuShowing;
                        view.CustomDrawFooterCell -= gridView_CustomDrawFooterCell;
                    }
                    catch (Exception) { }

                    view.CustomColumnDisplayText += gv_Positions_CustomColumnDisplayText;
                    view.PopupMenuShowing += gv_Positions_PopupMenuShowing;
                    view.CustomDrawFooterCell += gridView_CustomDrawFooterCell;

                    if (File.Exists(layout_ScenarioAnalysisPosition))
                        view.RestoreLayoutFromXml(layout_ScenarioAnalysisPosition);
                }
                
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        // Added by Snehadri on 09JUL2021
        private void gv_Positions_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e is null) return;

                var FieldName = e.Column.FieldName;

                //added on 24MAY2021 by Amey
                string _DisplayFormat = "#0,0.00";
                int RoundDigit = 2;
                if (CollectionHelper.dict_CustomDigits.TryGetValue(FieldName, out RoundDigit))
                {
                    switch (RoundDigit)
                    {
                        case 3:
                            _DisplayFormat = "#0,0.000";
                            break;
                        case 4:
                            _DisplayFormat = "#0,0.0000";
                            break;
                    }
                }
                else
                    RoundDigit = 2;

                if (!hs_StringColumns.Contains(FieldName))
                {
                    e.DisplayText = Math.Round(Convert.ToDecimal(e.Value) / CollectionHelper.dict_DisplayValues[trackBar_ScenarioAnalysis.Value], RoundDigit).ToString(_DisplayFormat, CultureInfo.CreateSpecificCulture("hi-IN"));
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        // Added by Snehadri on 22JUL2021 for Client Underlying Details
        private void gv_Underlying_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e is null) return;

                var FieldName = e.Column.FieldName;

                //added on 24MAY2021 by Amey
                string _DisplayFormat = "#0,0.00";
                int RoundDigit = 2;
                if (CollectionHelper.dict_CustomDigits.TryGetValue(FieldName, out RoundDigit))
                {
                    switch (RoundDigit)
                    {
                        case 3:
                            _DisplayFormat = "#0,0.000";
                            break;
                        case 4:
                            _DisplayFormat = "#0,0.0000";
                            break;
                    }
                }
                else
                    RoundDigit = 2;

                if (!hs_StringColumns.Contains(FieldName))
                {
                    e.DisplayText = Math.Round(Convert.ToDecimal(e.Value) / CollectionHelper.dict_DisplayValues[trackBar_ScenarioAnalysis.Value], RoundDigit).ToString(_DisplayFormat, CultureInfo.CreateSpecificCulture("hi-IN"));
                     
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        // Added by Snehadri on 09SEP2021
        private void gc_ScenarioAnalysis_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    var view = gc_ScenarioAnalysis.FocusedView as GridView;

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
                    var view = gc_ScenarioAnalysis.FocusedView as GridView;
                    view.ExpandMasterRow(view.FocusedRowHandle);
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
        #endregion

        #region Supplimentary Methods

        private void InitialiseUC()
        {
            try
            {
                //added on 30DEC2020 by Amey
                menu_ExportClientAnalysisToCSV.Caption = "Export Client Analysis as CSV";
                menu_ExportClientAnalysisToCSV.Click += ExportToCSV_Click;

                menu_ExportUnderlyingAnalysisToCSV.Caption = "Export Underlying Analysis as CSV";
                menu_ExportUnderlyingAnalysisToCSV.Click += ExportToCSV_Click;

                menu_ExportPositionAnalysisToCSV.Caption = "Export Position Analysis as CSV";
                menu_ExportPositionAnalysisToCSV.Click += ExportToCSV_Click;

                menu_SaveLayout.Caption = "Save layout";
                menu_SaveLayout.Click += Menu_SaveLayout_Click;

                list_Scenarios = CollectionHelper.list_Scenarios;

                gc_Scenarios.DataSource = list_Scenarios;
                gv_Scenarios.BestFitColumns();

                if (CollectionHelper.IsVarticalLines)
                {
                    gv_ScenarioAnalysis.Appearance.VertLine.BackColor = SystemColors.ActiveBorder;
                    gv_ScenarioAnalysis.Appearance.VertLine.BackColor2 = SystemColors.ActiveBorder;
                }

                //added on 14MAY2021 by Amey
                if (File.Exists(layout_ScenarioAnalysis))
                    gv_ScenarioAnalysis.RestoreLayoutFromXml(layout_ScenarioAnalysis);

                // FontSize
                try
                {
                    gv_ScenarioAnalysis.Appearance.Row.Font = new Font("Segoe UI", CollectionHelper.DataFontSize);
                    gv_ScenarioAnalysis.Appearance.HeaderPanel.Font = new Font("Segoe UI", CollectionHelper.DataFontSize + 2, FontStyle.Bold);
                    gv_ScenarioAnalysis.Appearance.FooterPanel.Font = new Font("Segoe UI", CollectionHelper.FooterFontSize);

                }
                catch (Exception) { }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void WriteToScenarioFile()
        {
            try
            {
                File.WriteAllText(Application.StartupPath + "\\" + "Report\\ScenarioAnalysis.txt", JsonConvert.SerializeObject(list_Scenarios));
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void Menu_SaveLayout_Click(object sender, EventArgs e)
        {
            try
            {
                //Added by Snehadri 09JUL2021
                foreach (GridView view in gc_ScenarioAnalysis.Views)
                {
                    if (view.DetailLevel == 0)
                        view.SaveLayoutToXml(layout_ScenarioAnalysis);
                    else if (view.DetailLevel == 1)
                        view.SaveLayoutToXml(layout_ScenarioAnalysisUnderlying);
                    else if (view.DetailLevel == 2)
                        view.SaveLayoutToXml(layout_ScenarioAnalysisPosition);
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

        private void View_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                string FocusedFieldName = e.Column.FieldName.ToUpper();
                if (FocusedFieldName == "STOCKJUMP" || FocusedFieldName == "IVJUMP")
                    e.RepositoryItem = gc_Scenarios.RepositoryItems["repTxt_GridCell"];
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void ComputeScenarioVaR(ref DataTable dt_ScenarioAnalysis, ref DataTable dt_ChildUnderlying, ref DataTable dt_ChildPosition)
        {
            try
            {

                //added on 14JAN2021 by Amey
                eve_ComputeScenarioClicked(true);
                while (!ArePositionsAvaialble) { Thread.Sleep(1000); }
                ArePositionsAvaialble = false;

                var list_AllScenarios = new List<Scenario>(list_Scenarios);
                var dict_AllClientInfo = new ConcurrentDictionary<string, ClientInfo>(CollectionHelper.dict_ClientInfo);
                var dict_ClientWiseSpanInfo = new ConcurrentDictionary<string, ClientSpanInfo>(CollectionHelper.dict_ClientWiseSpanInfo);

                var AllPositions = new Dictionary<string, List<ConsolidatedPositionInfo>>();

                if (!CollectionHelper.IncExpContract)
                {
                    AllPositions = dict_AllPositions.Values.SelectMany(x => x).Where(y => CommonFunctions.ConvertFromUnixTimestamp(y.Expiry).Date != DateTime.Now.Date).ToList().GroupBy(v=>v.Username).ToDictionary(kvp=>kvp.Key,v=>v.ToList());  //.ToDictionary(k => k, y => y.ToList());
                }
                else
                {
                    AllPositions = dict_AllPositions;
                }

                // Changed by Snehadri on 08JUL2021
                if (list_AllScenarios.Count > 0)
                {
                    
                    foreach (var _Client in AllPositions.Keys)
                    {
                        //var CLevelVaR = 0.0;
                        double CLevelFuturesMTM = 0, CLevelOptionsMTM = 0, CLevelEquityMTM = 0, CLevelCDFuturesMTM = 0, CLevelCDOptionsMTM = 0;
                        double CLevelIntradayFuturesMTM = 0, CLevelIntradayOptionsMTM = 0, CLevelIntradayEquityMTM = 0, CLevelIntradayCDFuturesMTM = 0, CLevelIntradayCDOptionsMTM = 0;

                        var CLevelDelta = 0.0;
                        var CLevelGamma = 0.0;
                        var CLevelTheta = 0.0;
                        var CLevelVega = 0.0;
                        var CLevelMargin = 0.0;
                        var CLevelDeltaAmt = 0.0;                       
                        var Deposit = 0.0; // Added by Snehadri on 08JUL2021 for Client Position Details

                        //changed to TryGetValue on 27MAY2021 by Amey
                        if (dict_AllClientInfo.TryGetValue(_Client, out ClientInfo _ClientInfo))
                        {
                            Deposit += _ClientInfo.ELM + _ClientInfo.AdHoc;
                        }

                        //Added by Snehadri on 08JUL2021 for Client Position Details
                        Dictionary<string, double> dict_ClientScenario = new Dictionary<string, double>();
                        

                        // Added by Snehadri on 22JUL2021 for Client Underlying Details
                        Dictionary<string, List<ConsolidatedPositionInfo>> dict_UnderlyingInfo = AllPositions[_Client].GroupBy(row => row.Underlying).ToDictionary(kvp => kvp.Key, kvp => kvp.ToList());
                                                

                        // Added by Snehadri on 22JUL2021 for Client Underlying Details
                        foreach (var _Underlying in dict_UnderlyingInfo.Keys)
                        {
                            double UnderlyingFuturesMTM = 0, UnderlyingOptionsMTM = 0, UnderlyingEquityMTM = 0, UnderlyingCDFuturesMTM = 0, UnderlyingCDOptionsMTM = 0;
                            double UnderlyingIntradayFuturesMTM = 0, UnderlyingIntradayOptionsMTM = 0, UnderlyingIntradayEquityMTM = 0, UnderlyingIntradayCDFuturesMTM = 0, UnderlyingIntradayCDOptionsMTM = 0;

                            var UnderlyingDelta = 0.0;
                            var UnderlyingGamma = 0.0;
                            var UnderlyingTheta = 0.0;
                            var UnderlyingVega = 0.0;
                            var UnderlyingMargin = 0.0;
                            var UnderlyingDeltaAmt = 0.0;

                            // Added by Snehadri on 22JUL2021 for Client Underlying Details
                            Dictionary<string, double> dict_UnderlyingScenario = new Dictionary<string, double>();
                            //foreach(var Scenario in list_AllScenarios) { dict_UnderlyingScenario.Add(Scenario.Name, 0.0); }
                            

                            foreach (var _PositionInfo in dict_UnderlyingInfo[_Underlying])
                            {

                                // Added by Snehadri on 24JUN2021 for Client Position Details
                                var VAR = 0.0;
                                var IV = 0.0;
                                var StockPrice = 0.0;

                                Dictionary<string, double> dict_PositionScenario = new Dictionary<string, double>();

                                //changed location on 07APR2021 by Amey
                                if (_PositionInfo.LTP <= 0 || _PositionInfo.UnderlyingLTP <= 0)
                                    continue;

                                //changed location on 07APR2021 by Amey
                                if (_PositionInfo.ScripType != en_ScripType.EQ && _PositionInfo.ExpiryTimeSpan.TotalDays <= 0)
                                    continue;

                                //if (!CollectionHelper.IncExpContract)
                                //{
                                   
                                //}


                                //added on 24MAY2021 by Amey
                                switch (_PositionInfo.ScripType)
                                {
                                    case en_ScripType.EQ:
                                        CLevelEquityMTM += _PositionInfo.MTM;
                                        CLevelIntradayEquityMTM += _PositionInfo.IntradayMTM;
                                        UnderlyingEquityMTM += _PositionInfo.MTM;
                                        UnderlyingIntradayEquityMTM += _PositionInfo.IntradayMTM;
                                        break;
                                    case en_ScripType.XX:
                                        CLevelFuturesMTM += _PositionInfo.MTM;
                                        CLevelIntradayFuturesMTM += _PositionInfo.IntradayMTM;
                                        UnderlyingFuturesMTM += _PositionInfo.MTM;
                                        UnderlyingIntradayFuturesMTM += _PositionInfo.IntradayMTM;

                                        break;
                                    case en_ScripType.CE:
                                    case en_ScripType.PE:
                                        CLevelOptionsMTM += _PositionInfo.MTM;
                                        CLevelIntradayOptionsMTM += _PositionInfo.IntradayMTM;
                                        UnderlyingOptionsMTM += _PositionInfo.MTM;
                                        UnderlyingIntradayOptionsMTM += _PositionInfo.IntradayMTM;
                                        break;
                                }

                                //CDS
                                if (_PositionInfo.Segment == en_Segment.NSECD)
                                {
                                    switch (_PositionInfo.ScripType)
                                    {
                                        case en_ScripType.XX:
                                            CLevelCDFuturesMTM += _PositionInfo.CDSMTM;
                                            UnderlyingCDFuturesMTM += _PositionInfo.CDSMTM;
                                            CLevelIntradayCDFuturesMTM += _PositionInfo.CDSIntradayMTM;
                                            UnderlyingIntradayCDFuturesMTM += _PositionInfo.CDSIntradayMTM;
                                            break;
                                        case en_ScripType.CE:
                                        case en_ScripType.PE:
                                            CLevelCDOptionsMTM += _PositionInfo.CDSMTM;
                                            UnderlyingCDOptionsMTM += _PositionInfo.CDSMTM;
                                            CLevelIntradayCDOptionsMTM += _PositionInfo.CDSIntradayMTM;
                                            UnderlyingIntradayCDOptionsMTM += _PositionInfo.CDSIntradayMTM;
                                            break;
                                    }
                                }

                                CLevelDelta += _PositionInfo.Delta * (CollectionHelper._ValueSigns.Delta);
                                CLevelGamma += _PositionInfo.Gamma * (CollectionHelper._ValueSigns.Gamma);
                                CLevelTheta += _PositionInfo.Theta * (CollectionHelper._ValueSigns.Theta);
                                CLevelVega += _PositionInfo.Vega * (CollectionHelper._ValueSigns.Vega);
                                CLevelDeltaAmt += _PositionInfo.Delta * _PositionInfo.UnderlyingLTP * (CollectionHelper._ValueSigns.DeltaAmt);

                                UnderlyingDelta += _PositionInfo.Delta * (CollectionHelper._ValueSigns.Delta);
                                UnderlyingGamma += _PositionInfo.Gamma * (CollectionHelper._ValueSigns.Gamma);
                                UnderlyingTheta += _PositionInfo.Theta * (CollectionHelper._ValueSigns.Theta);
                                UnderlyingVega += _PositionInfo.Vega * (CollectionHelper._ValueSigns.Vega);
                                UnderlyingDeltaAmt += _PositionInfo.Delta * _PositionInfo.UnderlyingLTP * (CollectionHelper._ValueSigns.DeltaAmt);

                                // Added by Snehadri on 08JUL2021 for Client Position Details
                                foreach (var _Scenario in list_AllScenarios)
                                {
                                    if (!dict_ClientScenario.ContainsKey(_Scenario.Name)) { dict_ClientScenario.Add(_Scenario.Name,0.0); }
                                    if (!dict_UnderlyingScenario.ContainsKey(_Scenario.Name)) { dict_UnderlyingScenario.Add(_Scenario.Name, 0.0); }
                                    if (!dict_PositionScenario.ContainsKey(_Scenario.Name)) { dict_PositionScenario.Add(_Scenario.Name, 0.0); }
                                       
                                    

                                    //changed logic on 18JAN2021 by Amey. Was always picking values assigned to ALL.
                                    var vall = _Scenario.list_ScenarioParams.Where(c => c.Stock == _PositionInfo.Underlying.Trim().ToUpper()).FirstOrDefault();
                                    if (vall is null)
                                        vall = _Scenario.list_ScenarioParams.Where(c => c.Stock == "ALL").FirstOrDefault();

                                    var CurrStockMovement = vall.StockJump;
                                    var CurrIVMovement = vall.IVJump;


                                    if (_PositionInfo.NetPosition == 0)
                                        dict_ClientScenario[_Scenario.Name] += 0;
                                    else
                                    {
                                        var TimetoExpiry = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry) - DateTime.Now;
                                        var CalculatedLTP = 0.0;
                                        var CurrIV = 0.0;
                                        var AdjustedPrice = 0.0;

                                        if (_PositionInfo.ScripType == en_ScripType.EQ || _PositionInfo.ScripType == en_ScripType.XX)
                                        {
                                            AdjustedPrice = (_PositionInfo.LTP * (100 + CurrStockMovement)) / 100;
                                            CalculatedLTP = AdjustedPrice; // Added by Snehadri on 24JUN2021
                                            StockPrice = AdjustedPrice;
                                        }
                                        else if (TimetoExpiry.TotalDays > 0 && (_PositionInfo.ScripType == en_ScripType.CE || _PositionInfo.ScripType == en_ScripType.PE))
                                        {
                                            CurrIV = (_PositionInfo.IVMiddle * (100 + CurrIVMovement)) / 100;
                                            AdjustedPrice = (_PositionInfo.UnderlyingLTP * (100 + CurrStockMovement)) / 100;

                                            
                                            // Added by Snehadri on 24JUN2021
                                            IV = CurrIV;
                                            StockPrice = AdjustedPrice;

                                            if (_PositionInfo.ScripType == en_ScripType.CE && CurrIV > 0)
                                                CalculatedLTP = CommonFunctions.CallOption(AdjustedPrice, _PositionInfo.StrikePrice, TimetoExpiry.TotalDays / 365, 0, (CurrIV / 100), 0);
                                            else if (_PositionInfo.ScripType == en_ScripType.PE && CurrIV > 0)
                                                CalculatedLTP = CommonFunctions.PutOption(AdjustedPrice, _PositionInfo.StrikePrice, TimetoExpiry.TotalDays / 365, 0, (CurrIV / 100), 0);
                                        }
                                        else
                                            continue;

                                        //added isNaN check on 06APR2021 by Amey
                                        //added on 04JAN2021 by Amey
                                        var Price = 0.0;

                                        if (radioGrp_ScenarioPrice.Properties.Items[radioGrp_ScenarioPrice.SelectedIndex].Value.ToString() == "LTP" && !double.IsNaN(_PositionInfo.LTP))
                                            Price = _PositionInfo.LTP;
                                        else
                                            Price = _PositionInfo.BEP;

                                        if (_PositionInfo.NetPosition > 0)
                                        {
                                            VAR = (CalculatedLTP - Price) * Math.Abs(_PositionInfo.NetPosition);    // Added by Snehadri on 24JUN2021
                                            dict_PositionScenario[_Scenario.Name] = VAR;
                                            dict_UnderlyingScenario[_Scenario.Name] += VAR;
                                            dict_ClientScenario[_Scenario.Name] += VAR;
                                        }
                                        else if (_PositionInfo.NetPosition < 0)
                                        {
                                            VAR = (Price - CalculatedLTP) * Math.Abs(_PositionInfo.NetPosition);    // Added by Snehadri on 24JUN2021
                                            dict_PositionScenario[_Scenario.Name] = VAR;
                                            dict_UnderlyingScenario[_Scenario.Name] += VAR;
                                            dict_ClientScenario[_Scenario.Name] += VAR;
                                        }
                                    }
                                }
                                
                                // Added by Snehadri on 22JUN2021 for Client Position Details
                                DataRow drow_position = dt_ChildPosition.NewRow();

                                drow_position["ClientID"] = _Client;
                                drow_position["Underlying"] = _PositionInfo.Underlying;
                                drow_position["ScripName"] = _PositionInfo.ScripName;
                                drow_position["ScripType"] = _PositionInfo.ScripType;
                                drow_position["InstrumentName"] = _PositionInfo.InstrumentName;
                                drow_position["Segment"] = _PositionInfo.Segment;
                                drow_position["ExpiryDate"] = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry);

                                drow_position["IntradayMTM"] = RoundDecimal(_PositionInfo.IntradayMTM, nameof(CPParent.IntradayMTM));
                                drow_position["MTM"] = RoundDecimal(_PositionInfo.MTM, nameof(CPParent.MTM));

                                drow_position["Delta"] = RoundDecimal(_PositionInfo.Delta * (CollectionHelper._ValueSigns.Delta), nameof(CPParent.Delta));
                                drow_position["Theta"] = RoundDecimal(_PositionInfo.Theta * (CollectionHelper._ValueSigns.Theta), nameof(CPParent.Theta));
                                drow_position["Gamma"] = RoundDecimal(_PositionInfo.Gamma * (CollectionHelper._ValueSigns.Gamma), nameof(CPParent.Gamma));
                                drow_position["DeltaAmount"] = RoundDecimal(_PositionInfo.Delta * _PositionInfo.LTP * (CollectionHelper._ValueSigns.DeltaAmt), nameof(CPParent.DeltaAmount));
                                drow_position["Vega"] = RoundDecimal(_PositionInfo.Vega * (CollectionHelper._ValueSigns.Vega), nameof(CPParent.Vega));

                                //added by nikhil
                                if (_PositionInfo.Segment == en_Segment.NSECD)
                                {
                                    drow_position["CDMTM"] = RoundDecimal(_PositionInfo.CDSMTM, nameof(CPParent.CDSMTM));
                                    drow_position["CDIntradayMTM"] = RoundDecimal(_PositionInfo.CDSIntradayMTM, nameof(CPParent.CDSIntradayMTM));
                                }
                                else
                                {
                                    drow_position["CDMTM"] = 0.0;
                                    drow_position["CDIntradayMTM"] = 0.0;
                                }

                                foreach (var _Scenario in list_AllScenarios)
                                {
                                    if (dict_PositionScenario.ContainsKey(_Scenario.Name))
                                        drow_position[_Scenario.Name] = RoundDecimal(dict_PositionScenario[_Scenario.Name] * (CollectionHelper._ValueSigns.VaR), _Scenario.Name);
                                    else
                                        drow_position[_Scenario.Name] = 0.0;
                                }

                                drow_position["NetPosition"] = _PositionInfo.NetPosition;
                                drow_position["StrikePrice"] = _PositionInfo.StrikePrice;
                                drow_position["IV"] = IV;
                                drow_position["StockPrice"] = StockPrice;

                                if (_ClientInfo != null)
                                {
                                    drow_position["Name"] = _ClientInfo.Name;
                                    drow_position["Zone"] = _ClientInfo.Zone;
                                    drow_position["Branch"] = _ClientInfo.Branch;
                                    drow_position["Family"] = _ClientInfo.Family;
                                    drow_position["Product"] = _ClientInfo.Product;
                                }
                                else
                                {
                                    drow_position["Name"] = "-";
                                    drow_position["Zone"] = "-";
                                    drow_position["Branch"] = "-";
                                    drow_position["Family"] = "-";
                                    drow_position["Product"] = "-";
                                }

                                dt_ChildPosition.Rows.Add(drow_position);
                            }

                            // Added by Snehadri on 22JUL2021 for Client Underlying Details
                            var dict_underlyingspaninfo = CollectionHelper.dict_ClientUnderlyingWiseSpanInfo;
                            if (dict_underlyingspaninfo.ContainsKey(_Client + "_" + _Underlying))
                            {
                                UnderlyingMargin += dict_underlyingspaninfo[_Client + "_" + _Underlying].MarginUtil;
                            }

                            DataRow drow_Underlying = dt_ChildUnderlying.NewRow();

                            drow_Underlying["ClientID"] = _Client;
                            drow_Underlying["Underlying"] = _Underlying;

                            drow_Underlying["FuturesMTM"] = RoundDecimal(UnderlyingFuturesMTM, nameof(CPParent.MTM));
                            drow_Underlying["OptionsMTM"] = RoundDecimal(UnderlyingOptionsMTM, nameof(CPParent.MTM));
                            drow_Underlying["EquityMTM"] = RoundDecimal(UnderlyingEquityMTM, nameof(CPParent.MTM));
                            var TotalUnderlyingMTM = UnderlyingFuturesMTM + UnderlyingOptionsMTM + UnderlyingEquityMTM;
                            drow_Underlying["MTM"] = RoundDecimal(TotalUnderlyingMTM, nameof(CPParent.MTM));

                            //CD
                            drow_Underlying["CDFuturesMTM"] = RoundDecimal(UnderlyingCDFuturesMTM, nameof(CPParent.MTM));
                            drow_Underlying["CDOptionsMTM"] = RoundDecimal(UnderlyingCDOptionsMTM, nameof(CPParent.MTM));
                            var CDTotalUnderlyingMTM = UnderlyingCDFuturesMTM + UnderlyingCDOptionsMTM;
                            drow_Underlying["CDMTM"] = RoundDecimal(CDTotalUnderlyingMTM, nameof(CPParent.MTM));

                            drow_Underlying["IntradayFuturesMTM"] = RoundDecimal(UnderlyingIntradayFuturesMTM, nameof(CPParent.IntradayMTM));
                            drow_Underlying["IntradayOptionsMTM"] = RoundDecimal(UnderlyingIntradayOptionsMTM, nameof(CPParent.IntradayMTM));
                            drow_Underlying["IntradayEquityMTM"] = RoundDecimal(UnderlyingIntradayEquityMTM, nameof(CPParent.IntradayMTM));
                            var TotalUnderlyingIntradayMTM = UnderlyingIntradayFuturesMTM + UnderlyingIntradayOptionsMTM + UnderlyingIntradayEquityMTM;
                            drow_Underlying["IntradayMTM"] = RoundDecimal(TotalUnderlyingIntradayMTM, nameof(CPParent.IntradayMTM));


                            //CD
                            drow_Underlying["CDIntradayFuturesMTM"] = RoundDecimal(UnderlyingIntradayCDFuturesMTM, nameof(CPParent.IntradayMTM));
                            drow_Underlying["CDIntradayOptionsMTM"] = RoundDecimal(UnderlyingIntradayCDOptionsMTM, nameof(CPParent.IntradayMTM));
                            var CDTotalUnderlyingIntradayMTM = UnderlyingIntradayCDFuturesMTM + UnderlyingIntradayCDOptionsMTM;
                            drow_Underlying["CDIntradayMTM"] = RoundDecimal(CDTotalUnderlyingIntradayMTM, nameof(CPParent.IntradayMTM));

                            drow_Underlying["Delta"] = RoundDecimal(UnderlyingDelta, nameof(CPParent.Delta));
                            drow_Underlying["Theta"] = RoundDecimal(UnderlyingTheta, nameof(CPParent.Theta));
                            drow_Underlying["Gamma"] = RoundDecimal(UnderlyingGamma, nameof(CPParent.Gamma));
                            drow_Underlying["Vega"] = RoundDecimal(UnderlyingVega, nameof(CPParent.Vega));
                            drow_Underlying["DeltaAmount"] = RoundDecimal(UnderlyingDeltaAmt, nameof(CPParent.DeltaAmount));

                            drow_Underlying["MTMDifference"] = RoundDecimal(TotalUnderlyingMTM - TotalUnderlyingIntradayMTM, nameof(CPParent.MTM));

                            foreach (var _Scenario in list_AllScenarios)
                            {
                                if (dict_UnderlyingScenario.ContainsKey(_Scenario.Name))
                                    drow_Underlying[_Scenario.Name] = RoundDecimal(dict_UnderlyingScenario[_Scenario.Name] * (CollectionHelper._ValueSigns.VaR), _Scenario.Name);
                                else
                                    drow_Underlying[_Scenario.Name] = 0.0;
                            }

                            drow_Underlying["MarginUtil"] = RoundDecimal(UnderlyingMargin, nameof(CPParent.MarginUtil));

                            
                            if (Deposit!=0.0)
                            {
                                drow_Underlying["MTM%"] = RoundDecimal((TotalUnderlyingMTM / Deposit)*100, "MTM%");
                                drow_Underlying["Margin%"] = RoundDecimal((UnderlyingMargin / Deposit)*100, "MARGIN%");
                            }
                            else
                            {
                                drow_Underlying["MTM%"] = RoundDecimal(TotalUnderlyingMTM, "MTM%");
                                drow_Underlying["Margin%"] = RoundDecimal(UnderlyingMargin, "MARGIN%");
                            }

                            if (_ClientInfo != null)
                            {
                                drow_Underlying["Name"] = _ClientInfo.Name;
                                drow_Underlying["Zone"] = _ClientInfo.Zone;
                                drow_Underlying["Branch"] = _ClientInfo.Branch;
                                drow_Underlying["Family"] = _ClientInfo.Family;
                                drow_Underlying["Product"] = _ClientInfo.Product;
                            }
                            else
                            {
                                drow_Underlying["Name"] = "-";
                                drow_Underlying["Zone"] = "-";
                                drow_Underlying["Branch"] = "-";
                                drow_Underlying["Family"] = "-";
                                drow_Underlying["Product"] = "-";
                            }

                            dt_ChildUnderlying.Rows.Add(drow_Underlying);
                        }

                        //added check on 16APR2021 by Amey
                        //added on 31DEC2020 by Amey
                        //CLevelVaR *= (CollectionHelper.IsVaRValueReversed ? -1 : 1);

                        //changed to TryGetValue on 27MAY2021 by Amey
                        //changed on 16FEB2021 by Amey
                        if (dict_ClientWiseSpanInfo.TryGetValue(_Client, out ClientSpanInfo _ClientSpanInfo))
                            CLevelMargin = _ClientSpanInfo.MarginUtil;



                        DataRow dRow_ScenarioAnalysis = dt_ScenarioAnalysis.NewRow();


                        dRow_ScenarioAnalysis["FuturesMTM"] = RoundDecimal(CLevelFuturesMTM, nameof(CPParent.MTM));
                        dRow_ScenarioAnalysis["OptionsMTM"] = RoundDecimal(CLevelOptionsMTM, nameof(CPParent.MTM));
                        dRow_ScenarioAnalysis["EquityMTM"] = RoundDecimal(CLevelEquityMTM, nameof(CPParent.MTM));

                        var TotalMTM = CLevelFuturesMTM + CLevelOptionsMTM + CLevelEquityMTM;
                        dRow_ScenarioAnalysis["MTM"] = RoundDecimal(TotalMTM, nameof(CPParent.MTM));

                        dRow_ScenarioAnalysis["IntradayFuturesMTM"] = RoundDecimal(CLevelIntradayFuturesMTM, nameof(CPParent.IntradayMTM));
                        dRow_ScenarioAnalysis["IntradayOptionsMTM"] = RoundDecimal(CLevelIntradayOptionsMTM, nameof(CPParent.IntradayMTM));
                        dRow_ScenarioAnalysis["IntradayEquityMTM"] = RoundDecimal(CLevelIntradayEquityMTM, nameof(CPParent.IntradayMTM));

                        var TotalIntradayMTM = CLevelIntradayFuturesMTM + CLevelIntradayOptionsMTM + CLevelIntradayEquityMTM;
                        dRow_ScenarioAnalysis["IntradayMTM"] = RoundDecimal(TotalIntradayMTM, nameof(CPParent.IntradayMTM));

                        //CD
                        dRow_ScenarioAnalysis["CDIntradayFuturesMTM"] = RoundDecimal(CLevelIntradayCDFuturesMTM, nameof(CPParent.IntradayMTM));
                        dRow_ScenarioAnalysis["CDIntradayOptionsMTM"] = RoundDecimal(CLevelIntradayCDOptionsMTM, nameof(CPParent.IntradayMTM));
                        var CDTotalIntradayMTM = CLevelIntradayCDFuturesMTM + CLevelIntradayCDOptionsMTM;
                        dRow_ScenarioAnalysis["CDIntradayMTM"] = RoundDecimal(CDTotalIntradayMTM, nameof(CPParent.IntradayMTM));

                        //CD
                        dRow_ScenarioAnalysis["CDFuturesMTM"] = RoundDecimal(CLevelCDFuturesMTM, nameof(CPParent.MTM));
                        dRow_ScenarioAnalysis["CDOptionsMTM"] = RoundDecimal(CLevelCDOptionsMTM, nameof(CPParent.MTM));
                        var CDTotalMTM = CLevelCDFuturesMTM + CLevelCDOptionsMTM;
                        dRow_ScenarioAnalysis["CDMTM"] = RoundDecimal(CDTotalMTM, nameof(CPParent.MTM));

                        dRow_ScenarioAnalysis["MTMDifference"] = RoundDecimal(TotalMTM - TotalIntradayMTM, nameof(CPParent.MTM));

                        dRow_ScenarioAnalysis["MarginUtil"] = RoundDecimal(CLevelMargin, nameof(CPParent.MarginUtil));
                        dRow_ScenarioAnalysis["Delta"] = RoundDecimal(CLevelDelta, nameof(CPParent.Delta));
                        dRow_ScenarioAnalysis["Gamma"] = RoundDecimal(CLevelGamma, nameof(CPParent.Gamma));
                        dRow_ScenarioAnalysis["Theta"] = RoundDecimal(CLevelTheta, nameof(CPParent.Theta));
                        dRow_ScenarioAnalysis["Vega"] = RoundDecimal(CLevelVega, nameof(CPParent.Vega));
                        dRow_ScenarioAnalysis["DeltaAmount"] = RoundDecimal(CLevelDeltaAmt, nameof(CPParent.DeltaAmount));
                        dRow_ScenarioAnalysis["Deposit"] = RoundDecimal(Deposit, "DEPOSIT");

                        
                        //changed to TryGetValue on 27MAY2021 by Amey
                        if (Deposit!=0.0)
                        {
                            dRow_ScenarioAnalysis["MTM%"] = RoundDecimal((TotalMTM / Deposit)*100, "MTM%");
                            dRow_ScenarioAnalysis["Margin%"] = RoundDecimal((CLevelMargin / Deposit)*100, "MARGIN%");
                        }
                        else
                        {
                            dRow_ScenarioAnalysis["MTM%"] = RoundDecimal(TotalMTM, "MTM%");
                            dRow_ScenarioAnalysis["Margin%"] = RoundDecimal(CLevelMargin, "MARGIN%");

                        }

                        // Added by Snehadri on 08JUL2021 for Client Position Details
                        
                        foreach (var _Scenario in list_AllScenarios)
                        {
                            if (dict_ClientScenario.ContainsKey(_Scenario.Name))
                            {
                                dRow_ScenarioAnalysis[_Scenario.Name] = RoundDecimal(dict_ClientScenario[_Scenario.Name] * (CollectionHelper._ValueSigns.VaR), _Scenario.Name);

                                var DepositScenario = (Deposit - (dict_ClientScenario[_Scenario.Name] * -1));

                                if (Deposit != 0.0)
                                {
                                    //added fixed -1 multiplication here beacuse positive VaR will get add up. Ex. 1000[Deposit] - (500 * -1)[VaR] = 1500. Becasuse VaR is positive cant subract it from Deposit.
                                    dRow_ScenarioAnalysis["Deposit-" + _Scenario.Name] = RoundDecimal(DepositScenario, "DEPOSIT%");
                                    dRow_ScenarioAnalysis[_Scenario.Name + " Bal%"] = RoundDecimal(((DepositScenario - Deposit) / Deposit) * 100, "DEPOSIT%");
                                }
                                else
                                {
                                    dRow_ScenarioAnalysis["Deposit-" + _Scenario.Name] = RoundDecimal(dict_ClientScenario[_Scenario.Name], "DEPOSIT%");
                                    dRow_ScenarioAnalysis[_Scenario.Name + " Bal%"] = RoundDecimal(dict_ClientScenario[_Scenario.Name], "DEPOSIT%");
                                }
                            }
                            else
                            {
                                dRow_ScenarioAnalysis[_Scenario.Name] = 0.0;
                                if (Deposit != 0.0)
                                {
                                    //added fixed -1 multiplication here beacuse positive VaR will get add up. Ex. 1000[Deposit] - (500 * -1)[VaR] = 1500. Becasuse VaR is positive cant subract it from Deposit.
                                    dRow_ScenarioAnalysis["Deposit-" + _Scenario.Name] = RoundDecimal(Deposit, "DEPOSIT%");
                                    dRow_ScenarioAnalysis[_Scenario.Name + " Bal%"] = RoundDecimal(100, "Deposit%");
                                }
                                else
                                {
                                    dRow_ScenarioAnalysis["Deposit-" + _Scenario.Name] = 0.0;
                                    dRow_ScenarioAnalysis[_Scenario.Name + " Bal%"] = 0.0;
                                }
                            }
                        }

                        dRow_ScenarioAnalysis["ClientID"] = _Client;

                        // Changed by Snehadri on 08JUL2021 for Client Position Details
                        //added on 25MAR2021 by Amey
                        if (_ClientInfo != null)
                        {
                            dRow_ScenarioAnalysis["Name"] = _ClientInfo.Name; // Added by Snehadri on 24JUN2021
                            dRow_ScenarioAnalysis["Zone"] = _ClientInfo.Zone;
                            dRow_ScenarioAnalysis["Branch"] = _ClientInfo.Branch;
                            dRow_ScenarioAnalysis["Family"] = _ClientInfo.Family;
                            dRow_ScenarioAnalysis["Product"] = _ClientInfo.Product;
                        }
                        else
                        {
                            dRow_ScenarioAnalysis["Name"] = "-"; // Added by Snehadri on 24JUN2021
                            dRow_ScenarioAnalysis["Zone"] = "-";
                            dRow_ScenarioAnalysis["Branch"] = "-";
                            dRow_ScenarioAnalysis["Family"] = "-";
                            dRow_ScenarioAnalysis["Product"] = "-";
                        }


                        dt_ScenarioAnalysis.Rows.Add(dRow_ScenarioAnalysis);
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void ExportToCSV_Click(object sender, EventArgs e)
        {
            try
            {
                var ColumnNames = view_popup.VisibleColumns.ToList();

                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "CSV file (*.csv)|*.csv";
                if (_Save.ShowDialog() == DialogResult.OK)
                {
                    if (view_popup.DetailLevel == 0)
                    {
                        gv_ScenarioAnalysis.ExportToCsv(_Save.FileName);
                        XtraMessageBox.Show("Exported successfully", "Success");
                    }
                        

                    else if (view_popup.DetailLevel == 1)
                    {
                        if(dt_ChildUnderlying!= null && dt_ChildUnderlying.Rows.Count > 0)
                        {
                            int colindex = 0;
                            foreach (var column in ColumnNames)
                            {
                                dt_ChildUnderlying.Columns[column.FieldName].SetOrdinal(colindex);
                                colindex += 1;
                            }
                            StringBuilder sb_Underlying = new StringBuilder();
                            string[] columnNames = dt_ChildUnderlying.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();
                            sb_Underlying.AppendLine(string.Join(",", columnNames));

                            List<string> datarows = dt_ChildUnderlying.AsEnumerable().Select(row => string.Join(",", row.ItemArray)).ToList();
                            sb_Underlying.AppendLine(string.Join(Environment.NewLine, datarows));

                            File.WriteAllText(_Save.FileName, sb_Underlying.ToString());
                            XtraMessageBox.Show("Exported successfully", "Success");
                        }
                    }

                    else if(view_popup.DetailLevel == 2)
                    {
                        if (dt_ChildPosition != null && dt_ChildPosition.Rows.Count > 0)
                        {
                            int colindex = 0;
                            foreach (var column in ColumnNames)
                            {
                                dt_ChildPosition.Columns[column.FieldName].SetOrdinal(colindex);
                                colindex += 1;
                            }
                            StringBuilder sb_Position = new StringBuilder();
                            string[] columnNames = dt_ChildPosition.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();
                            sb_Position.AppendLine(string.Join(",", columnNames));

                            List<string> datarows = dt_ChildPosition.AsEnumerable().Select(row => string.Join(",", row.ItemArray)).ToList();
                            sb_Position.AppendLine(string.Join(Environment.NewLine, datarows));

                            File.WriteAllText(_Save.FileName, sb_Position.ToString());
                            XtraMessageBox.Show("Exported successfully", "Success");
                        }

                    }
                    
                }
            }
            catch (Exception ee) { _logger.Error(ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }

        #region Exposed Methods

        internal void ClientPositionsReceived(Dictionary<string, List<ConsolidatedPositionInfo>> dict_AllPositions)
        {
            this.dict_AllPositions = new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_AllPositions);
            ArePositionsAvaialble = true;
        }

        #endregion

        private double RoundDecimal(double Val, string ColName)
        {
            if (CollectionHelper.dict_CustomDigits.TryGetValue(ColName, out int RoundDigit))
                return Math.Round(Val, RoundDigit);
            else
                return Math.Round(Val, 2);
        }


        #endregion

        private void tgSwitch_VAR_Toggled(object sender, EventArgs e)
        {
            try
            {
                if (tgSwitch_VAR.IsOn)
                    CollectionHelper.IncExpContract = true;
                else
                    CollectionHelper.IncExpContract = false;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
    }
}
