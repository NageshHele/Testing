using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using NerveLog;
using Prime.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Prime
{
    public delegate void del_btn_Refresh_Click(string prmMessage);
    public delegate void del_FormClosed(string ClientID = "");

    public partial class form_VaRDistribution : DevExpress.XtraEditors.XtraForm
    {
        public event del_btn_Refresh_Click eve_btn_Refresh_Click;

        //added on 24FEB2021 by Amey
        public event del_FormClosed eve_FormClosed;

        public StringBuilder sbClient = new StringBuilder();
        public Dictionary<string, double> dict_MaxValue = new Dictionary<string, double>();//added by Navin on 24-09-2019

        DXMenuItem menu_SaveLayout = new DXMenuItem();
        readonly string layout_VARDistribution = Application.StartupPath + "\\Layout\\" + "VarDistribution.xml";

        NerveLogger _logger;

        public form_VaRDistribution()
        {
            InitializeComponent();
            InitializeUC();
            this._logger = CollectionHelper._logger;
        }

        private void InitializeUC()
        {
            try
            {
                menu_SaveLayout.Caption = "Save layout";
                menu_SaveLayout.Click += Menu_SaveLayout_Click;

                if (File.Exists(layout_VARDistribution))
                    gv_VaRPortfolio.RestoreLayoutFromXml(layout_VARDistribution);

            }
            catch(Exception ee) { _logger.Error(ee); }
        }

        private void Menu_SaveLayout_Click(object sender, EventArgs e)
        {
            try
            {
                gv_VaRPortfolio.SaveLayoutToXml(layout_VARDistribution);
                XtraMessageBox.Show("Layout saved successfully.", "Success");
            }
            catch (Exception ee) { XtraMessageBox.Show("Something went wrong","Error"); _logger.Error(ee); }
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            dict_MaxValue.Clear();
            eve_btn_Refresh_Click(sbClient.ToString());
        }

        private void Grdvw_VarPortfolio_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "Underlying") return;

                e.Appearance.BackColor = Color.White;
                
                if (Convert.ToDouble(e.CellValue) == dict_MaxValue[gv_VaRPortfolio.GetRowCellValue(e.RowHandle, "Underlying").ToString()] && Convert.ToDouble(e.CellValue) != 0)
                {
                    e.Appearance.BackColor = Color.Red;
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        //added on 24FEB2021 by Amey
        private void frmPortfolio_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            eve_FormClosed(sbClient.ToString());
        }

        private void gv_VaRPortfolio_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.Menu == null) return;//Added on 26AUG2021 by Snehadri 

                e.Menu.Items.Add(menu_SaveLayout);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void gc_VaRPortfolio_ViewRegistered(object sender, DevExpress.XtraGrid.ViewOperationEventArgs e)
        {
            try
            {
                if (File.Exists(layout_VARDistribution))
                    gv_VaRPortfolio.RestoreLayoutFromXml(layout_VARDistribution);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
    }
}