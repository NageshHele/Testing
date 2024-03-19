using DevExpress.XtraEditors;
using Prime.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prime.UI
{
    public partial class form_EditableProperty : XtraForm
    {
        public event del_EditedValueReceived eve_EditedValueReceived;

        public form_EditableProperty(string LabelName, double CurrentValue)
        {
            InitializeComponent();

            lbl_Default.Text = LabelName;
            txt_Val.Text = CurrentValue.ToString();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            eve_EditedValueReceived(Convert.ToDouble(txt_Val.Text), lbl_Default.Text);
            this.Close();
        }
    }
}