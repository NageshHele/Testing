using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using NerveLog;
using Newtonsoft.Json;
using Prime.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Prime.UI
{
    public partial class uc_UnderlyingClients : XtraUserControl
    {
        public event del_IsStateChanged eve_IsUnderlyingExpanded;
        public event del_IsStateChanged eve_IsClientDoubleClicked;

        NerveLogger _logger;

        #region File Names

        /// <summary>
        /// Path to save UndderlyingClients Parent Grid Layout.
        /// </summary>
        readonly string layout_UCParent = Application.StartupPath + "\\Layout\\" + "UCParent.xml";

        /// <summary>
        /// Path to save UndderlyingClients Clients Grid Layout.
        /// </summary>
        readonly string layout_UCClients = Application.StartupPath + "\\Layout\\" + "UCClients.xml";

        /// <summary>
        /// Path to save the Fixed Columns Name
        /// </summary>
        readonly string fixcolumnloc = Application.StartupPath + "\\Report\\Underlying_FixColumns.json";

        #endregion

        UCParent _TempUCParent = new UCParent();
        UCClient _TempUCClients = new UCClient();

        #region Right Click Menu Items

        DXMenuItem menu_ExportToCSV = new DXMenuItem();// added by navin on 18-03-2019 for export to excel feature
        DXMenuItem menu_SaveLayout = new DXMenuItem();// added by navin on 18-03-2019 for export to excel feature

        DXMenuItem menu_ExportClientLevelToCSV = new DXMenuItem();
        DXMenuItem menu_FixColumn = new DXMenuItem(); // Added by Snehadri on 21OCT2021 to fix/unfix column

        #endregion

        /// <summary>
        /// DataSource for gc_ClientPortfolio.
        /// </summary>
        RealTimeSource rts_UnderlyingClients = new RealTimeSource();

        // Added by Snehadri on 05JUL2021 for fix column feature
        /// <summary>
        /// Stores the name of the column on which right click was performed
        /// </summary>
        string RightClicked_ColumnName = string.Empty;

        /// <summary>
        /// Stores the information of the gridview on which right click was performed
        /// </summary>
        GridView RightClicked_ViewInfo = new GridView();

        private uc_UnderlyingClients()
        {
            InitializeComponent();

            this._logger = CollectionHelper._logger;

            InitialiseUC();
        }

        #region Instance Initializing

        public static uc_UnderlyingClients Instance { get; private set; }

        public static void Initialise()
        {
            if (Instance is null)
                Instance = new uc_UnderlyingClients();
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

                menu_ExportClientLevelToCSV.Caption = "Export Clients As CSV";
                menu_ExportClientLevelToCSV.Click += Menu_ExportClientsToCSV_Click;

                menu_FixColumn.Caption = "Fix/Unfix Column";
                menu_FixColumn.Click += Menu_FixColumn_Click;

                rts_UnderlyingClients.DataSource = CollectionHelper.bList_UnderlyingClients;
                gc_UnderlyingClients.DataSource = rts_UnderlyingClients;

                if (CollectionHelper.IsVarticalLines)
                {
                    gv_UnderlyingClients.Appearance.VertLine.BackColor = SystemColors.ActiveBorder;
                    gv_UnderlyingClients.Appearance.VertLine.BackColor2 = SystemColors.ActiveBorder;
                }

                //added on 01APR2021 by Amey
                if (File.Exists(layout_UCParent))
                    gv_UnderlyingClients.RestoreLayoutFromXml(layout_UCParent);

                CollectionHelper.gc_UC = gc_UnderlyingClients;

                // FontSize
                try
                {
                    gv_UnderlyingClients.Appearance.Row.Font = new Font("Segoe UI", CollectionHelper.DataFontSize);
                    gv_UnderlyingClients.Appearance.HeaderPanel.Font = new Font("Segoe UI", CollectionHelper.DataFontSize + 2, FontStyle.Bold);
                    gv_UnderlyingClients.Appearance.FooterPanel.Font = new Font("Segoe UI", CollectionHelper.FooterFontSize);

                }
                catch (Exception) { }

                //Added by Snehadri on 21OCT2021 for Fix/Unfix Columns
                if (CollectionHelper.dict_ULFixColumn.ContainsKey(0))
                {
                    foreach (var item in CollectionHelper.dict_ULFixColumn[0])
                    {
                        if (gv_UnderlyingClients.Columns[item] != null) { gv_UnderlyingClients.Columns[item].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left; }
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        #endregion

        #region UI Events

        private void gc_UnderlyingClients_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                var dict_CustomColumnNames = CollectionHelper.dict_CustomColumnNames;
                var list_DecimalColumns = CollectionHelper.list_DecimalColumns;
                var dict_CustomDigits = CollectionHelper.dict_CustomDigits;

                for (int i = 0; i < gv_UnderlyingClients.Columns.Count; i++)
                {
                    string ColumnFieldName = gv_UnderlyingClients.Columns[i].FieldName;

                    //changed to TryGetValue on 27MAY2021 by Amey
                    //added on 22MAR2021 by Amey
                    if (dict_CustomColumnNames.TryGetValue(ColumnFieldName, out string CName) && CName != "")
                        gv_UnderlyingClients.Columns.ColumnByFieldName(ColumnFieldName).Caption = CName;

                    //added on 08APR2021 by Amey
                    if (list_DecimalColumns.Contains(ColumnFieldName))
                    {
                        gv_UnderlyingClients.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;

                        if (ColumnFieldName == nameof(_TempUCParent.PercentChange))
                            gv_UnderlyingClients.Columns[i].DisplayFormat.FormatString = "P2";
                        else
                        {
                            //added on 24MAY2021 by Amey
                            if (dict_CustomDigits.TryGetValue(ColumnFieldName, out int RoundDigit))
                                gv_UnderlyingClients.Columns[i].DisplayFormat.FormatString = "N" + RoundDigit;
                            else
                                gv_UnderlyingClients.Columns[i].DisplayFormat.FormatString = "N2";
                        }

                        //added on 15APR2021 by Amey. To display commas with Indian format.
                        gv_UnderlyingClients.Columns[i].DisplayFormat.Format = CultureInfo.CreateSpecificCulture("hi-IN").NumberFormat;
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gc_UnderlyingClients_ViewRegistered(object sender, DevExpress.XtraGrid.ViewOperationEventArgs e)
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

                try
                {
                    // Corrected by Snehadri on 12JUL2021
                    view.RowCellStyle -= gv_UCClients_RowCellStyle;
                    view.PopupMenuShowing -= gv_Client_PopupMenuShowing;
                    view.CustomDrawFooterCell -= gv_UnderlyingClients_CustomDrawFooterCell;
                }
                catch (Exception) { }

                // Corrected by Snehadri on 12JUL2021
                view.RowCellStyle += gv_UCClients_RowCellStyle;
                view.PopupMenuShowing += gv_Client_PopupMenuShowing;
                view.CustomDrawFooterCell += gv_UnderlyingClients_CustomDrawFooterCell;

                //Added by Snehadri on 21OCT2021 for Fix/Unfix Columns 
                if (CollectionHelper.dict_ULFixColumn.ContainsKey(view.DetailLevel))
                {
                    foreach (var item in CollectionHelper.dict_ULFixColumn[view.DetailLevel])
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
                if (File.Exists(layout_UCClients))
                    view.RestoreLayoutFromXml(layout_UCClients);
                else
                    view.BestFitColumns();

                #endregion
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_UCClients_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                //added on 18NOV2020 by Amey. To avoid NullRefExceptions.
                if (e is null) return;

                var FieldName = e.Column.FieldName;
                //added on 27NOV2020 by Amey
                if (FieldName == nameof(_TempUCClients.MTM) || FieldName == nameof(_TempUCClients.IntradayMTM) || FieldName == nameof(_TempUCClients.TheoreticalMTM) ||
                    FieldName == nameof(_TempUCClients.FuturesMTM) || FieldName == nameof(_TempUCClients.OptionsMTM) || FieldName == nameof(_TempUCClients.EquityMTM) ||
                    FieldName == nameof(_TempUCClients.IntradayFuturesMTM) || FieldName == nameof(_TempUCClients.IntradayOptionsMTM) || 
                    FieldName == nameof(_TempUCClients.IntradayEquityMTM) || FieldName == nameof(_TempUCClients.PosExpoOPT) || FieldName == nameof(_TempUCClients.PosExpoFUT))
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

            e.Menu.Items.Add(menu_ExportToCSV);
            e.Menu.Items.Add(menu_SaveLayout);

            // Added by Snehadri on 21OCT2021
            if (e.MenuType == GridMenuType.Column)
            {
                RightClicked_ViewInfo = new GridView(); RightClicked_ViewInfo = e.HitInfo.View;
                RightClicked_ColumnName = string.Empty; RightClicked_ColumnName = e.HitInfo.Column.FieldName;

                e.Menu.Items.Add(menu_FixColumn);
            }
        }

        private void gv_Client_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;//added by Navin on 06-06-2019

            e.Menu.Items.Add(menu_ExportClientLevelToCSV);
            e.Menu.Items.Add(menu_SaveLayout);
            // Added by Snehadri on 21OCT2021
            if (e.MenuType == GridMenuType.Column)
            {
                RightClicked_ViewInfo = new GridView(); RightClicked_ViewInfo = e.HitInfo.View;
                RightClicked_ColumnName = string.Empty; RightClicked_ColumnName = e.HitInfo.Column.FieldName;

                e.Menu.Items.Add(menu_FixColumn);
            }
        }

        private void gv_UnderlyingClients_MasterRowCollapsed(object sender, CustomMasterRowEventArgs e)
        {
            try
            {
                GridView gv_CPUnderlying = sender as GridView;

                string Underlying = gv_CPUnderlying.GetFocusedRowCellValue(nameof(_TempUCParent.Underlying)).ToString();

                eve_IsUnderlyingExpanded(false, $"{Underlying}");
            }
            catch (NullReferenceException ee) { _logger.Error(ee); }
        }

        private void gv_UnderlyingClients_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            try
            {
                GridView gv_CPUnderlying = sender as GridView;

                string Underlying = gv_CPUnderlying.GetFocusedRowCellValue(nameof(_TempUCParent.Underlying)).ToString();

                eve_IsUnderlyingExpanded(true, $"{Underlying}");
            }
            catch (NullReferenceException ee) { _logger.Error(ee); }
        }

        private void gv_UnderlyingClients_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                //added on 18NOV2020 by Amey. To avoid NullRefExceptions.
                if (e is null) return;

                var FieldName = e.Column.FieldName;
                if (FieldName == nameof(_TempUCParent.MTM) || FieldName == nameof(_TempUCParent.IntradayMTM) || FieldName == nameof(_TempUCParent.TheoreticalMTM) ||
                    FieldName == nameof(_TempUCParent.FuturesMTM) || FieldName == nameof(_TempUCParent.OptionsMTM) || FieldName == nameof(_TempUCParent.EquityMTM) ||
                    FieldName == nameof(_TempUCParent.IntradayFuturesMTM) || FieldName == nameof(_TempUCParent.IntradayOptionsMTM) || 
                    FieldName == nameof(_TempUCParent.IntradayEquityMTM) || FieldName == nameof(_TempUCParent.PosExpoOPT) || FieldName == nameof(_TempUCParent.PosExpoFUT))
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

        private void gc_UnderlyingClients_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var view = gc_UnderlyingClients.FocusedView as GridView;

                if (view.IsDetailView)
                {
                    var ClientID = view.GetFocusedRowCellValue(nameof(_TempUCClients.ClientID)).ToString();

                    var parentv = view.ParentView as GridView;
                    var Underlying = parentv.GetFocusedRowCellValue(nameof(_TempUCParent.Underlying)).ToString();

                    eve_IsClientDoubleClicked(true, ClientID + "^" + Underlying);
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gc_UnderlyingClients_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    var view = gc_UnderlyingClients.FocusedView as GridView;

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
                    var view = gc_UnderlyingClients.FocusedView as GridView;
                    view.ExpandMasterRow(view.FocusedRowHandle);
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gv_UnderlyingClients_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            CommonFunctions.gridView_CustomDrawFooterCell(sender, e);
        }

        #endregion

        #region Supplimentary Methods

        private void Menu_SaveLayout_Click(object sender, EventArgs e)
        {
            try
            {
                gv_UnderlyingClients.SaveLayoutToXml(layout_UCParent);

                foreach (GridView view in gc_UnderlyingClients.Views)
                {
                    if (view.IsDetailView)
                    {
                        if (view.LevelName == nameof(_TempUCParent.bList_Clients))
                            view.SaveLayoutToXml(layout_UCClients);
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

        private void Menu_ExportToCSV_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "CSV file (*.csv)|*.csv";
                if (_Save.ShowDialog() == DialogResult.OK)
                {
                    //gc_ClientPortfolio.ExportToCsv(folderName + "\\" + "Client-" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv");

                    var blist_UCParent = new List<UCParent>(CollectionHelper.bList_UnderlyingClients);
                    StringBuilder sb_UCParent = new StringBuilder();
                    var Properties = blist_UCParent[0].GetType().GetProperties();
                    sb_UCParent.Append(string.Join(",", Properties.Select(p => p.Name)));
                    sb_UCParent.Append("\n");

                    foreach (var Row in blist_UCParent)
                    {
                        foreach (var details in Properties)
                        {
                            sb_UCParent.Append(details.GetValue(Row) + ",");
                        }
                        sb_UCParent.Remove(sb_UCParent.ToString().LastIndexOf(','), 1);
                        sb_UCParent.Append("\n");
                    }
                    File.WriteAllText(_Save.FileName, sb_UCParent.ToString());

                    XtraMessageBox.Show("Exported successfully.", "Success");
                }
            }
            catch (Exception ee) { _logger.Error(ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }

        private void Menu_ExportClientsToCSV_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "CSV file (*.csv)|*.csv";
                if (_Save.ShowDialog() == DialogResult.OK)
                {
                    var blist_UCParent = new List<UCParent>(CollectionHelper.bList_UnderlyingClients);
                    var blist_Clients = blist_UCParent.SelectMany(v => v.bList_Clients).ToList();

                    StringBuilder sb_UCParent = new StringBuilder();
                    var Properties = blist_Clients[0].GetType().GetProperties();
                    sb_UCParent.Append(string.Join(",", Properties.Select(p => p.Name)));
                    sb_UCParent.Append("\n");

                    foreach (var Row in blist_Clients)
                    {
                        foreach (var details in Properties)
                        {
                            sb_UCParent.Append(details.GetValue(Row) + ",");
                        }
                        sb_UCParent.Remove(sb_UCParent.ToString().LastIndexOf(','), 1);
                        sb_UCParent.Append("\n");
                    }
                    File.WriteAllText(_Save.FileName, sb_UCParent.ToString());

                    XtraMessageBox.Show("Exported successfully.", "Success");
                }
            }
            catch (Exception ee) { _logger.Error(ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }

        //Added by Snehadri on 21OCT2021 for Fix/Unfix Columns
        private void Menu_FixColumn_Click(object sender, EventArgs e)
        {
            try
            {
                if (RightClicked_ColumnName != "" && RightClicked_ViewInfo != null)
                {
                    GridView view = RightClicked_ViewInfo;
                    string columnName = RightClicked_ColumnName;

                    if (view.Columns[columnName].Fixed == DevExpress.XtraGrid.Columns.FixedStyle.Left)
                    {
                        CollectionHelper.dict_ULFixColumn[view.DetailLevel].Remove(columnName);

                        view.Columns[columnName].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.None;
                        WriteToFixedColumnFile();
                    }
                    else
                    {
                        if (CollectionHelper.dict_ULFixColumn.ContainsKey(view.DetailLevel))
                            CollectionHelper.dict_ULFixColumn[view.DetailLevel].Add(columnName);
                        else
                            CollectionHelper.dict_ULFixColumn.Add(view.DetailLevel, new HashSet<string>() { columnName });

                        view.Columns[columnName].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                        WriteToFixedColumnFile();
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }
        //Added by Snehadri on 21OCT2021 for Fix/Unfix Columns
        private void WriteToFixedColumnFile()
        {
            try
            {
                File.WriteAllText(fixcolumnloc, JsonConvert.SerializeObject(CollectionHelper.dict_ULFixColumn));
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
        #endregion


    }
}
