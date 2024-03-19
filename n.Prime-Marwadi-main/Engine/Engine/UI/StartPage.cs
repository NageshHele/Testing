using System;
using System.Threading;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Diagnostics;

namespace Engine
{
    public partial class StartPage : DevExpress.XtraEditors.XtraForm
    {
        //Name of Sub-Client: SP
        DataSet ds = new DataSet();
        DataTable dt_ConnectionStrings = new DataTable();
        internal DialogResult dres;
        public StartPage(EngineProcess ep)
        {
            InitializeComponent();
            clsWriteLog obj = new clsWriteLog(Application.StartupPath + "\\Log");
            obj.WriteLog("StartPage");
        }

        private void lbl_Terms_Click(object sender, EventArgs e)
        {
            TermsAndConditions term = new TermsAndConditions();
            term.ShowDialog(this); 
        }
        private void checkTerms_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkTerms.Checked == true)
            {
                this.Hide();
                Close();
                dres = DialogResult.OK;
            }
        }
    }
}
