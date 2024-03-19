using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using NerveLog;
using Prime.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Prime.UI
{
    public partial class uc_Violations : XtraUserControl
    {
        NerveLogger _logger;

        DXMenuItem menu_ExportToCSV = new DXMenuItem();
        DXMenuItem menu_ExportBreachedData = new DXMenuItem();

        int new_alerts;

        private uc_Violations()
        {
            InitializeComponent();

            this._logger = CollectionHelper._logger;

            InitialiseUC();
        }

        #region Instance Initializing

        public static uc_Violations Instance { get; private set; }

        public static uc_Violations Initialise()
        {
            if (Instance is null)
                Instance = new uc_Violations();

            return Instance;
        }

        #endregion

        #region Supplimentary Methods

        private void InitialiseUC()
        {
            try
            {
                //added on 30DEC2020 by Amey
                menu_ExportToCSV.Caption = "Export As CSV";
                menu_ExportToCSV.Click += ExportToCSV_Click;

                menu_ExportBreachedData.Caption = "Export alerts logs";
                menu_ExportBreachedData.Click += Menu_ExportBreachedData_Click;

                gc_Violations.DataSource = CollectionHelper.dict_BanInfo.Values.ToList();
                gv_Violations.BestFitColumns();

                gc_BannedUnderlyings.DataSource = CollectionHelper.hs_BannedUnderlyings.ToList();
                gv_BannedUnderlyings.BestFitColumns();

                // FontSize
                try
                {
                    gv_Violations.Appearance.Row.Font = new Font("Segoe UI", CollectionHelper.DataFontSize);
                    gv_Violations.Appearance.HeaderPanel.Font = new Font("Segoe UI", CollectionHelper.DataFontSize + 2, FontStyle.Bold);
                    gv_Violations.Appearance.FooterPanel.Font = new Font("Segoe UI", CollectionHelper.FooterFontSize);

                    gv_BannedUnderlyings.Appearance.Row.Font = new Font("Segoe UI", CollectionHelper.DataFontSize);
                    gv_BannedUnderlyings.Appearance.HeaderPanel.Font = new Font("Segoe UI", CollectionHelper.DataFontSize + 2, FontStyle.Bold);
                    gv_BannedUnderlyings.Appearance.FooterPanel.Font = new Font("Segoe UI", CollectionHelper.FooterFontSize);

                    lbc_RuleAlert.Appearance.Font = new Font("Segoe UI", CollectionHelper.DataFontSize);
                }
                catch (Exception) { }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

       
        private void ExportToCSV_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "CSV file (*.csv)|*.csv";
                DialogResult result = _Save.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string folderName = _Save.FileName;
                    gv_Violations.ExportToCsv(folderName);

                    XtraMessageBox.Show("Exported successfully", "Success");
                }
            }
            catch (Exception ee) { _logger.Error(ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }

        #region Exposed Methods

        internal void RefreshGrid()
        {
            if (IsHandleCreated)
            {
                Invoke((MethodInvoker)(() =>
                {
                    gc_Violations.DataSource = CollectionHelper.dict_BanInfo.Values.ToList();
                    gv_Violations.BestFitColumns();

                    gc_BannedUnderlyings.DataSource = CollectionHelper.hs_BannedUnderlyings.ToList();
                    gv_BannedUnderlyings.BestFitColumns();
                }));
            }
        }

        // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        internal void RuleAlert(string text)
        {
            try
            {
                List<string> list_alerts = text.Split('\n').ToList();
                list_alerts.Reverse();
                list_alerts.RemoveAt(0);

                new_alerts = list_alerts.Count;

                list_alerts.AddRange(lbc_RuleAlert.Items.OfType<string>().ToList());

                int count = list_alerts.Count - 32000;
                if (count > 0)
                    list_alerts.RemoveRange(32000, count);

                lbc_RuleAlert.Items.Clear();
                lbc_RuleAlert.Items.AddRange(list_alerts.ToArray());
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        private void lbc_RuleAlert_DrawItem_1(object sender, ListBoxDrawItemEventArgs e)
        {
            try
            {
                if (e.Index < new_alerts)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
            }
            catch(Exception ee) { _logger.Error(ee); }

        }

        #endregion

        #endregion

        #region UI Events

        private void gc_Violations_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                var dict_CustomColumnNames = CollectionHelper.dict_CustomColumnNames;
                var list_DecimalColumns = CollectionHelper.list_DecimalColumns;
                var dict_CustomDigits = CollectionHelper.dict_CustomDigits;

                //added on 25MAR2021 by Amey
                for (int i = 0; i < gv_Violations.Columns.Count; i++)
                {
                    string ColumnFieldName = gv_Violations.Columns[i].FieldName;

                    //changed to TryGetValue on 27MAY2021 by Amey
                    //added on 22MAR2021 by Amey
                    if (dict_CustomColumnNames.TryGetValue(ColumnFieldName, out string CName) && CName != "")
                        gv_Violations.Columns.ColumnByFieldName(ColumnFieldName).Caption = CName;

                    //added on 08APR2021 by Amey
                    if (list_DecimalColumns.Contains(ColumnFieldName))
                    {
                        gv_Violations.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;

                        //added on 24MAY2021 by Amey
                        if (dict_CustomDigits.TryGetValue(ColumnFieldName, out int RoundDigit))
                            gv_Violations.Columns[i].DisplayFormat.FormatString = "N" + RoundDigit;
                        else
                            gv_Violations.Columns[i].DisplayFormat.FormatString = "N2";

                        //added on 15APR2021 by Amey. To display commas with Indian format.
                        gv_Violations.Columns[i].DisplayFormat.Format = CultureInfo.CreateSpecificCulture("hi-IN").NumberFormat;
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gc_BannedUnderlyings_DataSourceChanged(object sender, EventArgs e)
        {
            gv_BannedUnderlyings.Columns[0].Caption = "Banned Scrips";
        }

        private void gv_Violations_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            e.Menu?.Items.Add(menu_ExportToCSV);
            e.Menu?.Items.Add(menu_ExportBreachedData);
        }

        private void gridView_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            CommonFunctions.gridView_CustomDrawFooterCell(sender, e);
        }

        // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
        private void Menu_ExportBreachedData_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "Text file (*.txt)|*.txt";
                if (_Save.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllLines(_Save.FileName, lbc_RuleAlert.Items.OfType<string>().ToList());
                    XtraMessageBox.Show("Alerts logs saved successfully");

                }

            }
            catch (Exception ee) { _logger.Error(ee); }
        }
        #endregion

    }
}
