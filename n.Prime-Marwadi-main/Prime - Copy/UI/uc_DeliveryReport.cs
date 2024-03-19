using DevExpress.Data;
using DevExpress.DataAccess.UI.ExpressionEditor;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using n.Structs;
using NerveLog;
using Newtonsoft.Json;
using Prime.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prime.UI
{
    public partial class uc_DeliveryReport : DevExpress.XtraEditors.XtraUserControl
    {
        public event del_IsStateChanged eve_IsDRClientExpanded;
        public event del_IsStateChanged eve_IsDRUnderlyingExpanded;

        CPParent _TempCPParent = new CPParent();
        CPUnderlying _TempCPUnderlying = new CPUnderlying();

        NerveLogger _logger;

        /// <summary>
        /// Path to save ClientPorofolio Parent Grid Layout.
        /// </summary>
        readonly string layout_DRParent = Application.StartupPath + "\\Layout\\" + "DRParent.xml";

        /// <summary>
        /// Path to save ClientPorofolio Underlying Grid Layout.
        /// </summary>
        readonly string layout_DRUnderlying = Application.StartupPath + "\\Layout\\" + "DRUnderlying.xml";

        /// <summary>
        /// Path to save ClientPorofolio Positions Grid Layout.
        /// </summary>
        readonly string layout_DRPositions = Application.StartupPath + "\\Layout\\" + "DRPositions.xml";

        string fixcolumnloc = Application.StartupPath + "\\Report\\DelReport_FixColumns.json";

        string expressionfileloc = Application.StartupPath + "\\Report\\ExpressionColumns.json";

        /// <summary>
        /// DataSource for gc_ClientPortfolio.
        /// </summary>
        RealTimeSource rts_DeliveryReport = new RealTimeSource();

        DXMenuItem menu_ExportPositionLevelToCSV = new DXMenuItem();// added by navin on 18-03-2019 for export to excel feature
        DXMenuItem menu_SaveLayout = new DXMenuItem();// added by navin on 18-03-2019 for export to excel feature
        DXMenuItem menu_FixColumn = new DXMenuItem(); // Added by Snehadri on 06JUL2021 for freeze column feature 
        DXMenuItem menu_ExportClientPosition = new DXMenuItem();// Added by Snehadri on 24JUN2021 to save selected client position
        DXMenuItem menu_ExportReport = new DXMenuItem();
        DXMenuItem menu_AddExpressCol = new DXMenuItem(); // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        DXMenuItem menu_DeleteExpCol = new DXMenuItem(); // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder


        // Added by Snehadri on 05JUL2021 for fix column feature
        /// <summary>
        /// Stores the name of the column on which right click was performed
        /// </summary>
        string RightClicked_ColumnName = string.Empty;

        /// <summary>
        /// Stores the information of the gridview on which right click was performed
        /// </summary>
        GridView RightClicked_ViewInfo = new GridView();


        //Added by Snehadri on 24JUN2021
        /// <summary>
        /// Key : ClientID | Value : List of Positions. For saving positions of client
        /// </summary>
        Dictionary<string, List<ConsolidatedPositionInfo>> dict_ClientPositions = new Dictionary<string, List<ConsolidatedPositionInfo>>();


        /// <summary>
        /// Key : Expiry | Value : Days
        /// </summary>
        Dictionary<double, int> dict_ExpiryDays = new Dictionary<double, int>(); //Added by Akshay on 24-03-2021


        public uc_DeliveryReport()
        {
            InitializeComponent();
            this._logger = CollectionHelper._logger;
            InitialiseUC();

        }

        public static uc_DeliveryReport Instance { get; private set; }


        public static void Initialise()
        {
            if (Instance is null)
                Instance = new uc_DeliveryReport();
        }

        private void InitialiseUC()
        {
            try
            {
                menu_SaveLayout.Caption = "Save layout";
                menu_SaveLayout.Click += Menu_SaveLayout_Click;

                //added on 30DEC2020 by Amey
                menu_ExportPositionLevelToCSV.Caption = "Export Positions As CSV";
                menu_ExportPositionLevelToCSV.Click += Menu_ExportPositionToCSV_Click;

                menu_FixColumn.Caption = "Fix/Unfix Column";
                menu_FixColumn.Click += Menu_FixColumn_Click;

                // Added by Snehadri on 24JUN2021
                menu_ExportClientPosition.Caption = "Export Client Position";
                menu_ExportClientPosition.Click += Menu_ExportClientPosition_Click;

                menu_ExportReport.Caption = "Export Report";
                menu_ExportReport.Click += Menu_ExportReport_Click;


                // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
                menu_AddExpressCol.Caption = "Add Formula Column";
                menu_AddExpressCol.Click += AddExpressCol;

                // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
                menu_DeleteExpCol.Caption = "Delete Formula Column";
                menu_DeleteExpCol.Click += DeleteExpressionCol;


                rts_DeliveryReport.DataSource = CollectionHelper.bList_DeliveryReport;
                gc_DeliveryReport.DataSource = rts_DeliveryReport;

                CollectionHelper.gc_DR = gc_DeliveryReport;

                CollectionHelper.IsVarticalLines = Convert.ToBoolean(ConfigurationManager.AppSettings["VERTICAL-LINES"]);    //Added on Akshay on 24-08-2021
                CollectionHelper.IsFullVAR = Convert.ToBoolean(ConfigurationManager.AppSettings["FULL-VAR"]);    //Added by Akshay on 24-08-2021

                if (CollectionHelper.IsVarticalLines)
                {
                    gv_DeliveryReport.Appearance.VertLine.BackColor = SystemColors.ActiveBorder;
                    gv_DeliveryReport.Appearance.VertLine.BackColor2 = SystemColors.ActiveBorder;
                }

                gv_DeliveryReport.BestFitColumns();
                
                // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
                if (CollectionHelper.dict_ExpressionColumn.ContainsKey(5))
                {
                    var dict_temp = new Dictionary<string, string>(CollectionHelper.dict_ExpressionColumn[5]);
                    if (dict_temp != null)
                    {
                        foreach (var Colname in dict_temp.Keys)
                        {
                            string colName = Colname;
                            string expression = dict_temp[Colname];

                            GridColumn unbColumn = gv_DeliveryReport.Columns.AddField(colName);
                            unbColumn.Caption = colName;
                            unbColumn.UnboundExpression = expression;
                            unbColumn.VisibleIndex = gv_DeliveryReport.Columns.Count;
                            unbColumn.UnboundType = UnboundColumnType.Decimal;
                            unbColumn.ShowUnboundExpressionMenu = true;
                            unbColumn.OptionsColumn.AllowEdit = false;
                            unbColumn.DisplayFormat.FormatType = FormatType.Numeric;
                            unbColumn.DisplayFormat.FormatString = "N2";
                            unbColumn.AppearanceCell.BackColor = Color.LightBlue;
                            unbColumn.UnboundDataType = typeof(double);
                        }
                    }
                }

                //added on 01APR2021 by Amey
                if (File.Exists(layout_DRParent))
                    gv_DeliveryReport.RestoreLayoutFromXml(layout_DRParent);

            }
            catch (Exception ee) { _logger.Error(ee); }
        }


        private void gv_DeliveryReport_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            try
            {
                string ClientID = gv_DeliveryReport.GetFocusedRowCellValue(nameof(_TempCPUnderlying.ClientID)).ToString();
                eve_IsDRClientExpanded(true, ClientID);
            }
            catch(Exception ee) { _logger.Error(ee); }
        }

        private void gv_DeliveryReport_MasterRowCollapsed(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            try
            {
                string ClientID = gv_DeliveryReport.GetFocusedRowCellValue(nameof(_TempCPUnderlying.ClientID)).ToString();
                eve_IsDRClientExpanded(false, ClientID);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        // Added by Snehadri on 25JUN2021 
        internal void UpdatePositionDictionary(Dictionary<string, List<ConsolidatedPositionInfo>> dict_AllPositions)
        {
            try
            {
                dict_ClientPositions = new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_AllPositions);

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

                eve_IsDRUnderlyingExpanded(false, $"{ID}_{Underlying}");
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

                eve_IsDRUnderlyingExpanded(true, $"{ID}_{Underlying}");
            }
            catch (NullReferenceException ee) { _logger.Error(ee); }
        }

        private void gc_DeliveryReport_ViewRegistered(object sender, DevExpress.XtraGrid.ViewOperationEventArgs e)
        {
            GridView view = (e.View as GridView);
            if (view.IsDetailView == false)
                return;

            if (view.LevelName == nameof(_TempCPUnderlying.bList_Positions))
            {
                //added on 01APR2021 by Amey
                try
                {
                    view.MasterRowExpanded -= gv_CPUnderlying_MasterRowExpanded;
                    view.MasterRowCollapsed -= gv_CPUnderlying_MasterRowCollapsed;
                    view.CustomDrawFooterCell -= gv_CPUnderlying_CustomDrawFooterCell;
                }
                catch (Exception) { }

                view.MasterRowExpanded += gv_CPUnderlying_MasterRowExpanded;
                view.MasterRowCollapsed += gv_CPUnderlying_MasterRowCollapsed;
                view.CustomDrawFooterCell += gv_CPUnderlying_CustomDrawFooterCell;
            }

            //changed on 03FEB2021 by Amey
            if (File.Exists(layout_DRUnderlying))
                view.RestoreLayoutFromXml(layout_DRUnderlying);


            //changed on 03FEB2021 by Amey
            if (File.Exists(layout_DRPositions))
                view.RestoreLayoutFromXml(layout_DRPositions);
            else
                view.BestFitColumns();

        }

        private void Menu_SaveLayout_Click(object sender, EventArgs e)
        {
            try
            {
                //OptionsLayoutGrid opts = new OptionsLayoutGrid();
                //opts.Columns.RemoveOldColumns = false;
                //opts.Columns.StoreAllOptions = true;

                gv_DeliveryReport.SaveLayoutToXml(layout_DRParent);

                foreach (GridView view in gc_DeliveryReport.Views)
                {
                    if (view.IsDetailView)
                    {
                        if (view.LevelName == nameof(_TempCPParent.bList_Underlying))
                            view.SaveLayoutToXml(layout_DRUnderlying);
                        else if (view.LevelName == nameof(_TempCPUnderlying.bList_Positions))
                            view.SaveLayoutToXml(layout_DRPositions);
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

        private void Menu_ExportPositionToCSV_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "CSV file (*.csv)|*.csv";
                if (_Save.ShowDialog() == DialogResult.OK)
                {
                    DataTable dt_AllPositions = new DataTable();

                    foreach (GridColumn column in gv_DeliveryReport.VisibleColumns)
                    {
                        dt_AllPositions.Columns.Add(column.FieldName);
                    }

                    for (int i = 0; i < gv_DeliveryReport.DataRowCount; i++)
                    {
                        DataRow row = dt_AllPositions.NewRow();
                        foreach (GridColumn column in gv_DeliveryReport.VisibleColumns)
                        {
                            if (gv_DeliveryReport.GetRowCellValue(i, column) != null)
                                row[column.FieldName] = gv_DeliveryReport.GetRowCellValue(i, column).ToString();
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

        private void gv_DeliveryReport_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.Menu == null) return;//added by Navin on 06-06-2019


                var ediorItem = e.Menu.Items.Where(x => x.Caption == "Expression Editor...").FirstOrDefault();
                if (ediorItem != null)
                    ediorItem.Caption = "Formula Editor";

                if (e.HitInfo.Column == null) return;

                e.Menu.Items.Add(menu_SaveLayout);
                e.Menu.Items.Add(menu_ExportPositionLevelToCSV);
                e.Menu.Items.Add(menu_FixColumn);
                //e.Menu.Items.Add(menu_ExportClientPosition);
                e.Menu.Items.Add(menu_ExportReport);
                e.Menu.Items.Add(menu_AddExpressCol);
                e.Menu.Items.Add(menu_DeleteExpCol);

                //Added by Snehadri on 05JUL2021 for fix column feature
                if (e.MenuType == GridMenuType.Column)
                {
                    RightClicked_ViewInfo = new GridView(); RightClicked_ViewInfo = e.HitInfo.View;
                    RightClicked_ColumnName = string.Empty; RightClicked_ColumnName = e.HitInfo.Column.FieldName;
                }
            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        //Added by Snehadri on 24JUN2021 for saving Client's Position
        private void Menu_ExportClientPosition_Click(object sender, EventArgs e)
        {
            try
            {
                //Added by Akshay on 25-03-2021
                var ExpiryThreshHold = CollectionHelper.dict_DaysPercentage.Keys.Max();

                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "CSV file (*.csv)|*.csv";
                if (_Save.ShowDialog() == DialogResult.OK)
                {
                    string clientid = gv_DeliveryReport.GetFocusedRowCellValue("ClientID").ToString();

                    StringBuilder sb_CPChild = new StringBuilder();
                    //var list_Properties = new List<ConsolidatedPositionInfo>();
                    PropertyInfo[] Properties = dict_ClientPositions[clientid].First().GetType().GetProperties();
                    sb_CPChild.Append(string.Join(",", Properties.Select(p => p.Name)));
                    sb_CPChild.Append("\n");

                    foreach(var list_AllPositions in dict_ClientPositions.Values)
                    {
                        foreach (var Row in list_AllPositions)
                        {
                            try
                            {
                                if (Row.InstrumentName == en_InstrumentName.OPTSTK || Row.InstrumentName == en_InstrumentName.FUTSTK)
                                {
                                    var Expiry = Row.Expiry;

                                    if (!dict_ExpiryDays.ContainsKey(Expiry))
                                        dict_ExpiryDays.Add(Expiry, CountDaysToExpiry(CommonFunctions.ConvertFromUnixTimestamp(Expiry), DateTime.Now));

                                    var DaysToExpiry = dict_ExpiryDays[Expiry];
                                    if (DaysToExpiry <= ExpiryThreshHold)
                                    {

                                        if (Row.SpotPrice > 0 && Row.InstrumentName == en_InstrumentName.OPTSTK)
                                        {

                                            if ((Row.ScripType == en_ScripType.CE && Row.StrikePrice < Row.SpotPrice && Row.NetPosition > 0) ||
                                            (Row.ScripType == en_ScripType.CE && Row.StrikePrice < Row.SpotPrice && Row.NetPosition < 0) ||
                                            (Row.ScripType == en_ScripType.PE && Row.StrikePrice > Row.SpotPrice && Row.NetPosition > 0) ||
                                            (Row.ScripType == en_ScripType.PE && Row.StrikePrice > Row.SpotPrice && Row.NetPosition < 0))
                                            {

                                            }
                                            else
                                                continue;
                                        }
                                    }
                                    else
                                        continue;
                                }
                                else
                                    continue;
                            }
                            catch (Exception ee) { _logger.Error(ee, "|DeliveryMargin Loop : "); }

                            foreach (var details in Properties)
                            {
                                sb_CPChild.Append(details.GetValue(Row) + ",");
                            }
                            sb_CPChild.Remove(sb_CPChild.ToString().LastIndexOf(','), 1);
                            sb_CPChild.Append("\n");
                        }
                    }
                    
                    File.WriteAllText(_Save.FileName, sb_CPChild.ToString());

                    XtraMessageBox.Show("Exported successfully.", "Success");

                }

            }
            catch (Exception ee) { _logger.Error(ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }

        private void Menu_ExportReport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "CSV file (*.csv)|*.csv";
                if (_Save.ShowDialog() == DialogResult.OK)
                {
                    DataTable dt_AllPositions = new DataTable();

                    foreach (GridColumn column in gv_DeliveryReport.VisibleColumns)
                    {
                        dt_AllPositions.Columns.Add(column.FieldName);
                    }

                    for (int i = 0; i < gv_DeliveryReport.DataRowCount; i++)
                    {
                        DataRow row = dt_AllPositions.NewRow();
                        foreach (GridColumn column in gv_DeliveryReport.VisibleColumns)
                        {
                            if (gv_DeliveryReport.GetRowCellValue(i, column) != null)
                                row[column.FieldName] = gv_DeliveryReport.GetRowCellValue(i, column).ToString();
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


        // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        private void AddExpressCol(object sender, EventArgs e)
        {
            try
            {

                var colName = XtraInputBox.Show("Enter Column Name", "Column Name", "Formula Column").Trim();
                var dict_ColumnNames = CollectionHelper.dict_CustomColumnNames;
                List<string> list_expressioncolName = new List<string>();

                foreach (var key in CollectionHelper.dict_ExpressionColumn.Keys)
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
                            unbColumn.VisibleIndex = gv_DeliveryReport.Columns.Count;
                            unbColumn.ShowUnboundExpressionMenu = true;
                            unbColumn.OptionsColumn.AllowEdit = false;
                            unbColumn.FieldName = colName;
                            unbColumn.Name = colName;
                            unbColumn.Caption = colName;
                            unbColumn.DisplayFormat.FormatType = FormatType.Numeric;
                            unbColumn.DisplayFormat.FormatString = "N2";
                            unbColumn.AppearanceCell.BackColor = Color.LightBlue;
                            unbColumn.UnboundDataType = typeof(double);

                            if (CollectionHelper.dict_ExpressionColumn.ContainsKey(5))
                                CollectionHelper.dict_ExpressionColumn[5].Add(colName, unbColumn.UnboundExpression);
                            else
                            {
                                CollectionHelper.dict_ExpressionColumn.Add(5, new Dictionary<string, string>());
                                CollectionHelper.dict_ExpressionColumn[5].Add(colName, unbColumn.UnboundExpression);
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
                    CollectionHelper.dict_ExpressionColumn[5].Remove(colName);
                    view.Columns[colName].Dispose();

                    File.WriteAllText(expressionfileloc, JsonConvert.SerializeObject(CollectionHelper.dict_ExpressionColumn));
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
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


        //Added by Akshay on 24-03-2021 For Calculating Days to Expiry excluding Weekends.
        private int CountDaysToExpiry(DateTime endDate, DateTime fromDate)
        {
            try
            {
                var DaysCounter = 0;
                for (DateTime i = fromDate.Date; i < endDate.Date; i = i.AddDays(1))
                {
                    if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday)
                        DaysCounter += 1;
                }
                return DaysCounter;
            }

            catch (Exception ee) { _logger.Error(ee); }

            return 10;
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            try
            {
                CollectionHelper.IsDeliveryRefresh = true;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_DeliveryReport_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            CommonFunctions.gridView_CustomDrawFooterCell(sender, e);
        }

        private void gv_CPUnderlying_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            CommonFunctions.gridView_CustomDrawFooterCell(sender, e);
        }

        private void gv_DeliveryReport_ColumnUnboundExpressionChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            try
            {
                GridView view = RightClicked_ViewInfo;
                string colname = e.Column.FieldName;
                string expression = e.Column.UnboundExpression;
                if (CollectionHelper.dict_ExpressionColumn.ContainsKey(5))
                {
                    if (CollectionHelper.dict_ExpressionColumn[5].ContainsKey(colname))
                    {
                        CollectionHelper.dict_ExpressionColumn[5][colname] = e.Column.UnboundExpression;
                        File.WriteAllText(expressionfileloc, JsonConvert.SerializeObject(CollectionHelper.dict_ExpressionColumn));
                    }

                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_DeliveryReport_UnboundExpressionEditorCreated(object sender, DevExpress.XtraGrid.Views.Base.UnboundExpressionEditorEventArgs e)
        {
            try
            {
                ExpressionEditorView editorView = (ExpressionEditorView)e.ExpressionEditorView;
                editorView.Text = "Formula Editor";
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
    }
}
