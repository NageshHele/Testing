using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using n.Structs;
using NerveLog;
using Newtonsoft.Json;
using Prime.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prime.UI
{
    public partial class uc_AllPositions : XtraUserControl
    {
        public event del_IsStateChanged eve_GetAllTradesClicked;

        NerveLogger _logger;

        NetPositionInfo _TempPosInfo = new NetPositionInfo();

        /// <summary>
        /// Key : ClientID | Value : List of Positions.
        /// </summary>
        Dictionary<string, List<ConsolidatedPositionInfo>> dict_AllPositions = new Dictionary<string, List<ConsolidatedPositionInfo>>();

        ///// <summary>
        ///// Set true when Positions sent from Main.
        ///// </summary>
        //bool ArePositionsAvaialble = false;

        // Added by Snehadri on 05JUL2021 for fix column feature
        /// <summary>
        /// Stores the name of the column on which right click was performed
        /// </summary>
        string RightClicked_ColumnName = string.Empty;

        /// <summary>
        /// Stores the information of the gridview on which right click was performed
        /// </summary>
        GridView RightClicked_ViewInfo = new GridView();
               
        #region Right Click Menu Items

        DXMenuItem menu_ExportToCSV = new DXMenuItem();// added by navin on 18-03-2019 for export to excel feature
        DXMenuItem menu_SaveLayout = new DXMenuItem();// added by navin on 18-03-2019 for export to excel feature
        DXMenuItem menu_RefreshData = new DXMenuItem();
        DXMenuItem menu_FixColumns = new DXMenuItem();

        #endregion

        #region File Names

        /// <summary>
        /// Path to save ClientPorofolio Positions Grid Layout.
        /// </summary>
        readonly string layout_CPositions = Application.StartupPath + "\\Layout\\" + "CPositions.xml";
        
        /// <summary>
        /// Path to save the Fix/Unfix Columns names
        /// </summary>
        readonly string fixcolumnloc = Application.StartupPath + "\\Report\\AllPosition_FixColumns.json";

        #endregion

        bool isRefreshing = false;


        private uc_AllPositions()
        {
            InitializeComponent();

            this._logger = CollectionHelper._logger;

            InitialiseUC();
        }

        #region Instance Initializing

        public static uc_AllPositions Instance { get; private set; }

        public static void Initialise()
        {
            if (Instance is null)
                Instance = new uc_AllPositions();
        }

        #endregion

        #region Imp Methods

        private void InitialiseUC()
        {
            try
            {
                menu_SaveLayout.Caption = "Save layout";
                menu_SaveLayout.Click += Menu_SaveLayout_Click;

                //added on 30DEC2020 by Amey
                menu_ExportToCSV.Caption = "Export As CSV";
                menu_ExportToCSV.Click += Menu_ExportToCSV_Click;

                menu_RefreshData.Caption = "Refresh Data";
                menu_RefreshData.Click += Menu_RefreshData_Click;

                menu_FixColumns.Caption = "Fix/Unfix Columns";
                menu_FixColumns.Click += Menu_FixColumns_Click;

                gc_AllPositions.DataSource = new List<NetPositionInfo>();

                
                if (CollectionHelper.IsVarticalLines)
                {
                    gv_AllPositions.Appearance.VertLine.BackColor = SystemColors.ActiveBorder;
                    gv_AllPositions.Appearance.VertLine.BackColor2 = SystemColors.ActiveBorder;
                }

                //added on 01APR2021 by Amey
                if (File.Exists(layout_CPositions))
                    gv_AllPositions.RestoreLayoutFromXml(layout_CPositions);

                // FontSize
                try
                {
                    gv_AllPositions.Appearance.Row.Font = new Font("Segoe UI", CollectionHelper.DataFontSize);
                    gv_AllPositions.Appearance.HeaderPanel.Font = new Font("Segoe UI", CollectionHelper.DataFontSize + 2, FontStyle.Bold);
                    gv_AllPositions.Appearance.FooterPanel.Font = new Font("Segoe UI", CollectionHelper.FooterFontSize);                    
                }
                catch (Exception) { }

                // Added by Snehadri on 19JUL2021 for Fix/Unfix Column
                if (CollectionHelper.dict_APFixColumn.ContainsKey(0))
                {
                    foreach (var item in CollectionHelper.dict_APFixColumn[0])
                    {
                        if (gv_AllPositions.Columns[item] != null) { gv_AllPositions.Columns[item].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left; }
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        
        internal void AllPositionsReceived(Dictionary<string, List<ConsolidatedPositionInfo>> dict_ReceivedAllPositions)
        {
            dict_AllPositions = new Dictionary<string, List<ConsolidatedPositionInfo>>(dict_ReceivedAllPositions);

            Task.Run(() =>
            {
                isRefreshing = true;

                try
                {
                    var list_AllPositions = new List<NetPositionInfo>();

                    foreach (var item in dict_AllPositions.Values.SelectMany(v => v))
                    {
                        if (CollectionHelper.dict_ClientInfo.TryGetValue(item.Username, out ClientInfo _ClientInfo))
                        {
                            var _NetPosInfo = CopyPropertiesFrom(item, new NetPositionInfo());

                            _NetPosInfo.Name = _ClientInfo.Name;
                            _NetPosInfo.Zone = _ClientInfo.Zone;
                            _NetPosInfo.Branch = _ClientInfo.Branch;
                            _NetPosInfo.Family = _ClientInfo.Family;
                            _NetPosInfo.Product = _ClientInfo.Product;
                            _NetPosInfo.ExpiryDate = CommonFunctions.ConvertFromUnixTimestamp(item.Expiry);

                            if (_NetPosInfo.ScripType == en_ScripType.CE || _NetPosInfo.ScripType == en_ScripType.PE)
                            {
                                _NetPosInfo.ROV = _NetPosInfo.NetPosition * _NetPosInfo.LTP;
                                _NetPosInfo.DayPremium = _NetPosInfo.DayNetPremium * (-1);
                                _NetPosInfo.DayPremiumCDS = _NetPosInfo.DayNetPremiumCDS * (-1);
                            }
                            else
                            {
                                _NetPosInfo.ROV = 0;
                                _NetPosInfo.DayPremium = 0;
                                _NetPosInfo.DayNetPremium = 0;
                                _NetPosInfo.DayNetPremiumCDS = 0;
                                _NetPosInfo.DayPremiumCDS = 0;
                            }

                            // Added vy Snehadri on 09Jun2022
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
                        }
                    }

                    //added on 3JUN2021 by Amey
                    if (IsHandleCreated)
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            gc_AllPositions.DataSource = list_AllPositions;

                        //commented because layout should be restored from Layout File. On 3JUN2021 by Amey
                        //gv_AllPositions.BestFitColumns();

                        progressPanel_GetPositions.Visible = false;
                        }));
                    }
                }
                catch (Exception ee) { _logger.Error(ee); }

                isRefreshing = false;
            });
        }

        #endregion

        #region Supplimentary Methods

        private void Menu_SaveLayout_Click(object sender, EventArgs e)
        {
            try
            {
                gv_AllPositions.SaveLayoutToXml(layout_CPositions);

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

        private void Menu_ExportToCSV_Click(object sender, EventArgs e)
        {
            try
            {
                if (gv_AllPositions.RowCount > 0)
                {
                    SaveFileDialog _Save = new SaveFileDialog();
                    _Save.Filter = "CSV file (*.csv)|*.csv";
                    if (_Save.ShowDialog() == DialogResult.OK)
                    {
                        //var list_AllTrades = dict_AllPositions.Values.SelectMany(v => v).ToList();
                        //StringBuilder sb_CPChild = new StringBuilder();
                        //var Properties = list_AllTrades[0].GetType().GetProperties();
                        //sb_CPChild.Append(string.Join(",", Properties.Select(p => p.Name)));
                        //sb_CPChild.Append("\n");
                        //foreach (var Row in list_AllTrades)
                        //{
                        //    foreach (var details in Properties)
                        //    {
                        //        sb_CPChild.Append(details.GetValue(Row) + ",");
                        //    }
                        //    sb_CPChild.Remove(sb_CPChild.ToString().LastIndexOf(','), 1);
                        //    sb_CPChild.Append("\n");
                        //}
                        //File.WriteAllText(_Save.FileName, sb_CPChild.ToString());

                        // Changed the logic on 25AUG2021 by Snehadri 
                        DataTable dt_AllPositions = new DataTable();

                        foreach (GridColumn column in gv_AllPositions.VisibleColumns)
                        {
                            dt_AllPositions.Columns.Add(column.FieldName);
                        }

                        for (int i = 0; i < gv_AllPositions.DataRowCount; i++)
                        {
                            DataRow row = dt_AllPositions.NewRow();
                            foreach (GridColumn column in gv_AllPositions.VisibleColumns)
                            {
                                row[column.FieldName] = gv_AllPositions.GetRowCellValue(i, column).ToString();
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
            }
            catch (Exception ee) { _logger.Error(ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }

        private void Menu_RefreshData_Click(object sender, EventArgs e)
        {
            progressPanel_GetPositions.Visible = true;
            eve_GetAllTradesClicked(true);
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

        private void Menu_FixColumns_Click(object sender, EventArgs e)
        {
            try
            {
                if (RightClicked_ColumnName != "" && RightClicked_ViewInfo != null)
                {
                    GridView view = RightClicked_ViewInfo;
                    string columnName = RightClicked_ColumnName;

                    if (view.Columns[columnName].Fixed == DevExpress.XtraGrid.Columns.FixedStyle.Left)
                    {
                        CollectionHelper.dict_APFixColumn[view.DetailLevel].Remove(columnName);

                        view.Columns[columnName].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.None;
                        WriteToFixedColumnFile();
                    }
                    else
                    {
                        if (CollectionHelper.dict_APFixColumn.ContainsKey(view.DetailLevel))
                            CollectionHelper.dict_APFixColumn[view.DetailLevel].Add(columnName);
                        else
                            CollectionHelper.dict_APFixColumn.Add(view.DetailLevel, new HashSet<string>() { columnName });

                        view.Columns[columnName].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
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
                File.WriteAllText(fixcolumnloc, JsonConvert.SerializeObject(CollectionHelper.dict_APFixColumn));
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        #endregion

        #region UI Events

        private void gc_AllPositions_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                var dict_CustomColumnNames = CollectionHelper.dict_CustomColumnNames;
                var list_DecimalColumns = CollectionHelper.list_DecimalColumns;
                var dict_CustomDigits = CollectionHelper.dict_CustomDigits;

                for (int i = 0; i < gv_AllPositions.Columns.Count; i++)
                {
                    string ColumnFieldName = gv_AllPositions.Columns[i].FieldName;

                    //changed to TryGetValue on 27MAY2021 by Amey
                    //added on 22MAR2021 by Amey
                    if (dict_CustomColumnNames.TryGetValue(ColumnFieldName, out string CName) && CName != "")
                        gv_AllPositions.Columns.ColumnByFieldName(ColumnFieldName).Caption = CName;

                    //added on 08APR2021 by Amey
                    if (list_DecimalColumns.Contains(ColumnFieldName))
                    {
                        gv_AllPositions.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;

                        //added on 24MAY2021 by Amey
                        if (dict_CustomDigits.TryGetValue(ColumnFieldName, out int RoundDigit))
                            gv_AllPositions.Columns[i].DisplayFormat.FormatString = "N" + RoundDigit;
                        else
                            gv_AllPositions.Columns[i].DisplayFormat.FormatString = "N2";

                        //added on 15APR2021 by Amey. To display commas with Indian format.
                        gv_AllPositions.Columns[i].DisplayFormat.Format = CultureInfo.CreateSpecificCulture("hi-IN").NumberFormat;
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_AllPositions_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                //added on 18NOV2020 by Amey. To avoid NullRefExceptions.
                if (e is null) return;

                var FieldName = e.Column.FieldName;
                //changed on 03FEB2021 by Amey
                if (FieldName == nameof(_TempPosInfo.MTM) || FieldName == nameof(_TempPosInfo.IntradayMTM) || FieldName == nameof(_TempPosInfo.TheoreticalMTM))
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

        private void gv_AllPositions_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;//added by Navin on 06-06-2019

            e.Menu.Items.Add(menu_ExportToCSV);
            e.Menu.Items.Add(menu_SaveLayout);

            //added on 17MAY2021 by Amey
            if (!isRefreshing)
                e.Menu.Items.Add(menu_RefreshData);
            //Added by Snehadri on 05JUL2021 for fix column feature
            if (e.MenuType == GridMenuType.Column)
            {
                RightClicked_ViewInfo = new GridView(); RightClicked_ViewInfo = e.HitInfo.View;
                RightClicked_ColumnName = string.Empty; RightClicked_ColumnName = e.HitInfo.Column.FieldName;

                e.Menu.Items.Add(menu_FixColumns);
            }
        }

        private void gridView_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            CommonFunctions.gridView_CustomDrawFooterCell(sender, e);
        }

        #endregion

        private void timer_AutoSave_Tick(object sender, EventArgs e)
        {
            
        }

        private void gv_AllPositions_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "ITM_OTM_Percentage")
                    e.DisplayText = e.Value + "%";
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

    }
}
