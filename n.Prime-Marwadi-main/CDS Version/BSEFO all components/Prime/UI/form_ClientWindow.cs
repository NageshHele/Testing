
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using n.Structs;
using NerveLog;
using Prime.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prime.UI
{
    public delegate void del_FormClosed();
    public delegate void del_IndexChanged(string _ClientID, string _underlying, string _Expiry);

    public partial class form_ClientWindow : DevExpress.XtraEditors.XtraForm
    {
        public event del_FormClosed eve_FormClosed;
        public event del_IndexChanged eve_ClientIDIndexChanged;
        public event del_IndexChanged eve_UnderlyingIndexChanged;
        public event del_IndexChanged eve_ExpiryIndexChanged;
        DXMenuItem menu_SaveLayout = new DXMenuItem();// added by navin on 18-03-2019 for export to excel feature

        ConcurrentDictionary<string, string> dict_CustomColumnNames = new ConcurrentDictionary<string, string>();
        List<string> list_DecimalColumns = new List<string>();
        ConcurrentDictionary<string, int> dict_CustomDigits = new ConcurrentDictionary<string, int>();

        NerveLogger _logger;

        CWPositions _CWPositions = new CWPositions();

        /// <summary>
        /// Path to save ClientPorofolio Positions Grid Layout.
        /// </summary>
        readonly string layout_AnalysisPositions = Application.StartupPath + "\\Layout\\" + "AnalysisPositions.xml";

        public form_ClientWindow()
        {
            InitializeComponent();
            this._logger = CollectionHelper._logger;
            InitialiseUC();
        }

        public void InitialiseUC()
        {
            try
            {
                menu_SaveLayout.Caption = "Save layout";
                menu_SaveLayout.Click += Menu_SaveLayout_Click;

                //added on 01APR2021 by Amey
                if (File.Exists(layout_AnalysisPositions))
                {
                    OptionsLayoutGrid opt = new OptionsLayoutGrid();
                    opt.Columns.RemoveOldColumns = false;
                    opt.Columns.StoreAllOptions = true;
                    gc_windows.MainView.RestoreLayoutFromXml(layout_AnalysisPositions, opt);
                }
                    

                if (CollectionHelper.IsVarticalLines)
                {
                    gv_window.Appearance.VertLine.BackColor = SystemColors.ActiveBorder;
                    gv_window.Appearance.VertLine.BackColor2 = SystemColors.ActiveBorder;
                }

                this.dict_CustomColumnNames = CollectionHelper.dict_CustomColumnNames;
                this.list_DecimalColumns = CollectionHelper.list_DecimalColumns;
                this.dict_CustomDigits = CollectionHelper.dict_CustomDigits;

            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void form_window_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                eve_FormClosed();
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void cbe_ClientID_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cbe_Underlying.Text = "";
                cbe_Expiry.Text = "ALL";
                cbe_Underlying.Properties.Items.Clear();
                cbe_Expiry.Properties.Items.Clear();
                //CollectionHelper.bList_ClientWindow.Clear();

                var _ClientID = cbe_ClientID.Text.ToString();
                var _Underlying = cbe_Underlying.Text.ToString();
                var _Expiry = cbe_Expiry.Text.ToString();

                eve_ClientIDIndexChanged(_ClientID, _Underlying, _Expiry);
            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        private void cbe_Underlying_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cbe_Expiry.Text = "ALL";

                var _ClientID = cbe_ClientID.Text.ToString();
                var _Underlying = cbe_Underlying.Text.ToString();
                var _Expiry = cbe_Expiry.Text.ToString();

                cbe_Expiry.Properties.Items.Clear();
                //CollectionHelper.bList_ClientWindow.Clear();

                if (_Underlying != "")
                    eve_UnderlyingIndexChanged(_ClientID, _Underlying, _Expiry);
            }

            catch (Exception ee) { _logger.Error(ee); }

        }

        private void cbe_Expiry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var _ClientID = cbe_ClientID.Text.ToString();
                var _Underlying = cbe_Underlying.Text.ToString();
                var _Expiry = cbe_Expiry.Text.ToString();

                //CollectionHelper.bList_ClientWindow.Clear();

                eve_ExpiryIndexChanged(_ClientID, _Underlying, _Expiry);
            }

            catch (Exception ee) { _logger.Error(ee); }


        }

        private void cbe_ClientID_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                cbe_ClientID.Properties.Items.Clear();
                cbe_ClientID.Properties.Items.AddRange(CollectionHelper.dict_ComboUniverse.Keys.ToList());
                cbe_ClientID.ShowPopup();
            }

            catch (Exception ee) { _logger.Error(ee); }

        }

        private void cbe_Underlying_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var _ClientID = cbe_ClientID.Text.ToString();
                cbe_Underlying.Properties.Items.Clear();
                cbe_Underlying.Properties.Items.AddRange(CollectionHelper.dict_ComboUniverse[_ClientID].Keys.ToList());
                cbe_Underlying.ShowPopup();
            }

            catch (Exception ee) { _logger.Error(ee); }

        }

        private void cbe_Expiry_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var _ClientID = cbe_ClientID.Text.ToString();
                var _Underlying = cbe_Underlying.Text.ToString();

                if (_Underlying != "")
                {
                    cbe_Expiry.Properties.Items.Clear();
                    cbe_Expiry.Properties.Items.Add("ALL");
                    cbe_Expiry.Properties.Items.AddRange(CollectionHelper.dict_ComboUniverse[_ClientID][_Underlying].Keys.ToList());
                    cbe_Expiry.ShowPopup();
                }
            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        private void Menu_SaveLayout_Click(object sender, EventArgs e)
        {
            try
            {
                OptionsLayoutGrid opt = new OptionsLayoutGrid();
                opt.Columns.RemoveOldColumns = false;
                opt.Columns.StoreAllOptions = true;

                gc_windows.MainView.SaveLayoutToXml(layout_AnalysisPositions, opt);

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

        private void gv_window_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null) return;//added by Navin on 06-06-2019
            e.Menu.Items.Add(menu_SaveLayout);
        }

        // Added by Snehadri on 09SEP2021 to close the form on pressing Escape key
        protected override bool ProcessCmdKey(ref Message msg ,Keys key)
        {
            try
            {
                if(key == Keys.Escape)
                {
                    this.Close();
                    return true;
                }

                return base.ProcessCmdKey(ref msg,key);    
            }
            catch(Exception ee) { _logger.Error(ee); return base.ProcessCmdKey(ref msg, key); }
        }

        // Added by Snehadri on 21OCT2021
        private void gv_window_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var FieldName = e.Column.FieldName;
                
                if(FieldName == nameof(_CWPositions.NetPosition))
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
        // Added by Snehadri on 21OCT2021
        private void gc_windows_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < gv_window.Columns.Count; i++)
                {
                    string ColumnFieldName = gv_window.Columns[i].FieldName;

                    //changed to TryGetValue on 27MAY2021 by Amey
                    //added on 22MAR2021 by Amey
                    if (dict_CustomColumnNames.TryGetValue(ColumnFieldName, out string CName) && CName != "")
                        gv_window.Columns.ColumnByFieldName(ColumnFieldName).Caption = CName;

                    //added on 08APR2021 by Amey
                    if (list_DecimalColumns.Contains(ColumnFieldName))
                    {
                        gv_window.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;

                        //added on 24MAY2021 by Amey
                        if (dict_CustomDigits.TryGetValue(ColumnFieldName, out int RoundDigit))
                            gv_window.Columns[i].DisplayFormat.FormatString = "N" + RoundDigit;
                        else
                            gv_window.Columns[i].DisplayFormat.FormatString = "N2";

                        //added on 15APR2021 by Amey. To display commas with Indian format.
                        gv_window.Columns[i].DisplayFormat.Format = CultureInfo.CreateSpecificCulture("hi-IN").NumberFormat;
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
        // Added by Snehadri on 21OCT2021
        private void gc_Greeks_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                
                for (int i = 0; i < gv_Greeks.Columns.Count; i++)
                {
                    string ColumnFieldName = gv_Greeks.Columns[i].FieldName;

                    //changed to TryGetValue on 27MAY2021 by Amey
                    //added on 22MAR2021 by Amey
                    if (dict_CustomColumnNames.TryGetValue(ColumnFieldName, out string CName) && CName != "")
                        gv_Greeks.Columns.ColumnByFieldName(ColumnFieldName).Caption = CName;

                    //added on 08APR2021 by Amey
                    if (list_DecimalColumns.Contains(ColumnFieldName))
                    {
                        gv_Greeks.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;

                        //added on 24MAY2021 by Amey
                        if (dict_CustomDigits.TryGetValue(ColumnFieldName, out int RoundDigit))
                            gv_Greeks.Columns[i].DisplayFormat.FormatString = "N" + RoundDigit;
                        else
                            gv_Greeks.Columns[i].DisplayFormat.FormatString = "N2";

                        //added on 15APR2021 by Amey. To display commas with Indian format.
                        gv_Greeks.Columns[i].DisplayFormat.Format = CultureInfo.CreateSpecificCulture("hi-IN").NumberFormat;
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
        // Added by Snehadri on 21OCT2021
        private void gc_Options_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < gv_Options.Columns.Count; i++)
                {
                    string ColumnFieldName = gv_Options.Columns[i].FieldName;

                    //changed to TryGetValue on 27MAY2021 by Amey
                    //added on 22MAR2021 by Amey
                    if (dict_CustomColumnNames.TryGetValue(ColumnFieldName, out string CName) && CName != "")
                        gv_Options.Columns.ColumnByFieldName(ColumnFieldName).Caption = CName;

                    //added on 08APR2021 by Amey
                    if (list_DecimalColumns.Contains(ColumnFieldName))
                    {
                        gv_Options.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;

                        //added on 24MAY2021 by Amey
                        if (dict_CustomDigits.TryGetValue(ColumnFieldName, out int RoundDigit))
                            gv_Options.Columns[i].DisplayFormat.FormatString = "N" + RoundDigit;
                        else
                            gv_Options.Columns[i].DisplayFormat.FormatString = "N2";

                        //added on 15APR2021 by Amey. To display commas with Indian format.
                        gv_Options.Columns[i].DisplayFormat.Format = CultureInfo.CreateSpecificCulture("hi-IN").NumberFormat;
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
        // Added by Snehadri on 21OCT2021
        private void gc_Futures_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < gv_Futures.Columns.Count; i++)
                {
                    string ColumnFieldName = gv_Futures.Columns[i].FieldName;

                    //changed to TryGetValue on 27MAY2021 by Amey
                    //added on 22MAR2021 by Amey
                    if (dict_CustomColumnNames.TryGetValue(ColumnFieldName, out string CName) && CName != "")
                        gv_Futures.Columns.ColumnByFieldName(ColumnFieldName).Caption = CName;

                    //added on 08APR2021 by Amey
                    if (list_DecimalColumns.Contains(ColumnFieldName))
                    {
                        gv_Futures.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;

                        //added on 24MAY2021 by Amey
                        if (dict_CustomDigits.TryGetValue(ColumnFieldName, out int RoundDigit))
                            gv_Futures.Columns[i].DisplayFormat.FormatString = "N" + RoundDigit;
                        else
                            gv_Futures.Columns[i].DisplayFormat.FormatString = "N2";

                        //added on 15APR2021 by Amey. To display commas with Indian format.
                        gv_Futures.Columns[i].DisplayFormat.Format = CultureInfo.CreateSpecificCulture("hi-IN").NumberFormat;
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
    }
}