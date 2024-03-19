using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using NerveLog;
using Prime.Helper;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Prime.UI
{
    public partial class uc_ConcentrationRisk : XtraUserControl
    {
        public event del_IsStateChanged eve_IsClientDoubleClicked;

        NerveLogger _logger;

        SpanMargin _TempSpanMargin = new SpanMargin();

        DXMenuItem menu_ExportToCSV = new DXMenuItem();
        DXMenuItem menu_SaveLayout = new DXMenuItem();

        /// <summary>
        /// Path to save ClientPorofolio Parent Grid Layout.
        /// </summary>
        readonly string layout_CRChartDetail = Application.StartupPath + "\\Layout\\" + "CRChartDetail.xml";

        /// <summary>
        /// DataSource for gc_ConcentrationRisk.
        /// </summary>
        RealTimeSource rts_ConcentrationRisk = new RealTimeSource();

        private uc_ConcentrationRisk()
        {
            InitializeComponent();

            this._logger = CollectionHelper._logger;

            InitialiseUC();
        }

        #region Instance Initializing

        public static uc_ConcentrationRisk Instance { get; private set; }

        public static uc_ConcentrationRisk Initialise()
        {
            if (Instance is null)
                Instance = new uc_ConcentrationRisk();

            return Instance;
        }

        #endregion

        #region Imp Methods

        private void InitialiseUC()
        {
            try
            {
                menu_ExportToCSV.Caption = "Export As CSV";
                menu_ExportToCSV.Click += Menu_ExportToCSV_Click;

                menu_SaveLayout.Caption = "Save layout";
                menu_SaveLayout.Click += Menu_SaveLayout_Click;

                rts_ConcentrationRisk.DataSource = CollectionHelper.bList_ConcentrationRisk;
                gc_ConcentrationRisk.DataSource = rts_ConcentrationRisk;

                // FontSize
                try
                {

                    gv_ConcentrationRisk.Appearance.Row.Font = new Font("Segoe UI", CollectionHelper.DataFontSize);
                    gv_ConcentrationRisk.Appearance.HeaderPanel.Font = new Font("Segoe UI", CollectionHelper.DataFontSize + 2, FontStyle.Bold);
                    gv_ConcentrationRisk.Appearance.FooterPanel.Font = new Font("Segoe UI", CollectionHelper.FooterFontSize);

                    gv_ChartDetail.Appearance.Row.Font = new Font("Segoe UI", CollectionHelper.DataFontSize);
                    gv_ChartDetail.Appearance.HeaderPanel.Font = new Font("Segoe UI", CollectionHelper.DataFontSize + 2, FontStyle.Bold);
                    gv_ChartDetail.Appearance.FooterPanel.Font = new Font("Segoe UI", CollectionHelper.FooterFontSize);

                    if (CollectionHelper.IsVarticalLines)
                    {
                        gv_ConcentrationRisk.Appearance.VertLine.BackColor = SystemColors.ActiveBorder;
                        gv_ConcentrationRisk.Appearance.VertLine.BackColor2 = SystemColors.ActiveBorder;

                        gv_ChartDetail.Appearance.VertLine.BackColor = SystemColors.ActiveBorder;
                        gv_ChartDetail.Appearance.VertLine.BackColor2 = SystemColors.ActiveBorder;
                    }

                }
                catch (Exception) { }

                if (File.Exists(layout_CRChartDetail))
                    gv_ChartDetail.RestoreLayoutFromXml(layout_CRChartDetail);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void DisplayChartData(string ClientID)
        {
            try
            {
                chartControl_ConcentrationRisk.DataSource = null;
                chartControl_ConcentrationRisk.Series[0].ArgumentDataMember = null;
                chartControl_ConcentrationRisk.Series[0].ValueDataMembers.Clear();
                chartControl_ConcentrationRisk.Series[0].Name = ClientID;
               // chartControl_ConcentrationRisk.Series[0].Points.Clear();

                //added > 0 check on 04MAR2021 by Amey
                //changed on 18FEB2021 by Amey
                chartControl_ConcentrationRisk.DataSource = CollectionHelper.dict_ClientUnderlyingWiseSpanInfo.Where(v => v.Key.StartsWith(ClientID) && v.Value.MarginUtil > 0).Select(v => v.Value).ToList();
                chartControl_ConcentrationRisk.Series[0].ArgumentDataMember = "Underlying";
                chartControl_ConcentrationRisk.Series[0].ValueDataMembers.AddRange(new string[] { "MarginUtil" });
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void ChartDetailTable(string ClientID)
        {
            try
            {
                CollectionHelper.list_UnderlyingMargin.Clear();
                gc_ChartDetail.DataSource = null;

                var Client_MarginDetail = CollectionHelper.bList_ConcentrationRisk.Where(v => v.Client == ClientID).First();
                var list_UnderlyingMargin = CollectionHelper.dict_ClientUnderlyingWiseSpanInfo.Where(v => v.Key.StartsWith(ClientID) && v.Value.MarginUtil > 0).Select(v => v.Value).ToList();


                //var TotalMargin = Client_MarginDetail.MarginUtil;
                var TotalMargin = list_UnderlyingMargin.Sum(v => v.MarginUtil);

                foreach (var detail in list_UnderlyingMargin)
                {
                    ConcentrationRiskMargin _RiskMargin = new ConcentrationRiskMargin
                    {
                        Underlying = detail.Underlying,
                        MarginUtil = detail.MarginUtil,
                        Percentage = (detail.MarginUtil / TotalMargin) * 100
                    };

                    CollectionHelper.list_UnderlyingMargin.Add(_RiskMargin);
                }

                gc_ChartDetail.DataSource = CollectionHelper.list_UnderlyingMargin;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        #region Exposed Methods

        internal void RefreshChart()
        {
            if (IsHandleCreated)  //Added check by Akshay on 24-12-2020
                Invoke((MethodInvoker)(() => { chartControl_ConcentrationRisk.RefreshData(); }));
        }

        #endregion

        #endregion

        #region UI Events

        private void gv_ConcentrationRisk_RowClick(object sender, RowClickEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                DisplayChartData(gv_ConcentrationRisk.GetRowCellValue(e.RowHandle, nameof(_TempSpanMargin.Client)).ToString());
                ChartDetailTable(gv_ConcentrationRisk.GetRowCellValue(e.RowHandle, nameof(_TempSpanMargin.Client)).ToString());
            }
        }

        private void gc_ConcentrationRisk_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                var dict_CustomColumnNames = CollectionHelper.dict_CustomColumnNames;
                var list_DecimalColumns = CollectionHelper.list_DecimalColumns;
                var dict_CustomDigits = CollectionHelper.dict_CustomDigits;

                for (int i = 0; i < gv_ConcentrationRisk.Columns.Count; i++)
                {
                    string ColumnFieldName = gv_ConcentrationRisk.Columns[i].FieldName;

                    //changed to TryGetValue on 27MAY2021 by Amey
                    //added on 22MAR2021 by Amey
                    if (dict_CustomColumnNames.TryGetValue(ColumnFieldName, out string CName) && CName != "")
                        gv_ConcentrationRisk.Columns.ColumnByFieldName(ColumnFieldName).Caption = CName;

                    //added on 08APR2021 by Amey
                    if (list_DecimalColumns.Contains(ColumnFieldName))
                    {
                        gv_ConcentrationRisk.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;

                        //added on 24MAY2021 by Amey
                        if (dict_CustomDigits.TryGetValue(ColumnFieldName, out int RoundDigit))
                            gv_ConcentrationRisk.Columns[i].DisplayFormat.FormatString = "N" + RoundDigit;
                        else
                            gv_ConcentrationRisk.Columns[i].DisplayFormat.FormatString = "N2";

                        //added on 15APR2021 by Amey. To display commas with Indian format.
                        gv_ConcentrationRisk.Columns[i].DisplayFormat.Format = CultureInfo.CreateSpecificCulture("hi-IN").NumberFormat;
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gridView_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            CommonFunctions.gridView_CustomDrawFooterCell(sender, e);
        }

        #endregion

        private void gc_ConcentrationRisk_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var view = gc_ConcentrationRisk.FocusedView as GridView;
                var _ID = view.GetFocusedRowCellValue(nameof(_TempSpanMargin.Client)).ToString();

                eve_IsClientDoubleClicked(true, _ID);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gc_ChartDetail_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {

                var dict_CustomColumnNames = CollectionHelper.dict_CustomColumnNames;
                var list_DecimalColumns = CollectionHelper.list_DecimalColumns;
                var dict_CustomDigits = CollectionHelper.dict_CustomDigits;

                for (int i = 0; i < gv_ChartDetail.Columns.Count; i++)
                {
                    string ColumnFieldName = gv_ChartDetail.Columns[i].FieldName;

                    if (ColumnFieldName == "Percentage")
                        gv_ChartDetail.Columns[i].Caption = "Percentage%";

                    //changed to TryGetValue on 27MAY2021 by Amey
                    //added on 22MAR2021 by Amey
                    if (dict_CustomColumnNames.TryGetValue(ColumnFieldName, out string CName) && CName != "")
                        gv_ChartDetail.Columns.ColumnByFieldName(ColumnFieldName).Caption = CName;

                    //added on 08APR2021 by Amey
                    if (list_DecimalColumns.Contains(ColumnFieldName))
                    {
                        gv_ChartDetail.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;

                        //added on 24MAY2021 by Amey
                        if (dict_CustomDigits.TryGetValue(ColumnFieldName, out int RoundDigit))
                            gv_ChartDetail.Columns[i].DisplayFormat.FormatString = "N" + RoundDigit;
                        else
                            gv_ChartDetail.Columns[i].DisplayFormat.FormatString = "N2";

                        //added on 15APR2021 by Amey. To display commas with Indian format.
                        gv_ChartDetail.Columns[i].DisplayFormat.Format = CultureInfo.CreateSpecificCulture("hi-IN").NumberFormat;
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_ChartDetail_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e is null)
                    return;
                if (e.Column.FieldName == "Percentage")
                    e.DisplayText = Math.Round(Convert.ToDecimal(e.Value), 2).ToString("#0.00") + "%";

            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_ChartDetail_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            try
            {
                CommonFunctions.gridView_CustomDrawFooterCell(sender, e);

            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_ChartDetail_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.Menu == null) return;//added by Navin on 06-06-2019

                e.Menu.Items.Add(menu_ExportToCSV);
                e.Menu.Items.Add(menu_SaveLayout);
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
                    gv_ChartDetail.ExportToCsv(_Save.FileName);
                    XtraMessageBox.Show("Exported successfully", "Success");

                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }


        private void Menu_SaveLayout_Click(object sender, EventArgs e)
        {
            try
            {
                gv_ChartDetail.SaveLayoutToXml(layout_CRChartDetail);
                XtraMessageBox.Show("Layout saved successfully.", "Success");
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
    }
}
