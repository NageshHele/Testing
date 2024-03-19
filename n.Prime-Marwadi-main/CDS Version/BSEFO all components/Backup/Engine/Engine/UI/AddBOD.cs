using System;
using System.Data;
using DevExpress.XtraEditors;
using System.Xml;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace Engine
{
    public partial class AddBOD : DevExpress.XtraEditors.XtraForm
    {
        clsWriteLog _logger;
        string _MySQLCon = string.Empty;

        public AddBOD(string _ReceivedMySQLCon, clsWriteLog _logger)
        {
            InitializeComponent();

            this._logger = _logger;
            _MySQLCon = _ReceivedMySQLCon;
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    XtraMessageBox.Show("All details are mandatory");
                else
                {
                    using (var con_MySQL = new MySqlConnection(_MySQLCon))
                    {
                        con_MySQL.Open();
                        //changed on 31DEC2020 by Amey
                        using (MySqlCommand cmd = new MySqlCommand("INSERT INTO tbl_clientdetail(ClientID,DealerID,UserID,Username,Name,Margin,Adhoc,Zone,Branch,Family,Product) VALUES(@ClientID,@DealerID,@USerID,@Username,@Name,@Margin,@Adhoc,@Zone,@Branch,@Family,@Product)", con_MySQL))
                        {
                            //changed on 31DEC2020 by Amey
                            cmd.Parameters.AddWithValue("@ClientID", txt_ClientID.Text);
                            cmd.Parameters.AddWithValue("@DealerID", txt_DealerID.Text);
                            cmd.Parameters.AddWithValue("@UserID", txt_UserID.Text);
                            cmd.Parameters.AddWithValue("@Username", txt_Username.Text);
                            cmd.Parameters.AddWithValue("@Name", txt_Name.Text);
                            cmd.Parameters.AddWithValue("@Margin", txt_Margin.Text);
                            cmd.Parameters.AddWithValue("@Adhoc", txt_Adhoc.Text);

                            cmd.Parameters.AddWithValue("@Zone", txt_Zone.Text);
                            cmd.Parameters.AddWithValue("@Branch", txt_Branch.Text);
                            cmd.Parameters.AddWithValue("@Family", txt_Family.Text);
                            cmd.Parameters.AddWithValue("@Product", txt_Product.Text);
                            cmd.Parameters.AddWithValue("@Segment", txt_Segment.Text); // Added by Snehadri on 16JUL2022 for Client-Segment biforcation

                            cmd.ExecuteNonQuery();
                            _logger.WriteLog("Client affected in database -" + txt_ClientID.Text + "," + txt_Username.Text + "," + txt_Name.Text + "," + txt_Margin.Text + "," + txt_Adhoc.Text);
                            XtraMessageBox.Show("Record inserted successfully");
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                if (ee.ToString().Contains("Duplicate"))
                    XtraMessageBox.Show("User already exists");
                else
                {
                    XtraMessageBox.Show("Please enter details in correct format");
                    _logger.WriteLog("btnInsert_Click : " + ee);
                }
            }
        }

        public bool ValidateInput()
        {
            if (txt_Username.Text == "") { return false; }
            if (txt_Name.Text == "") { return false; }
            if (txt_Margin.Text == "") { return false; }
            if (txt_Adhoc.Text == "") { return false; }
            if (txt_Zone.Text == "") { return false; }
            if (txt_Branch.Text == "") { return false; }
            if (txt_Family.Text == "") { return false; }
            if (txt_Product.Text == "") { return false; }
            if (txt_ClientID.Text == "" && txt_DealerID.Text == "" && txt_UserID.Text == "") { return false; }
            return true;
        }

       
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddBOD_Load(object sender, EventArgs e) { }
    }
}