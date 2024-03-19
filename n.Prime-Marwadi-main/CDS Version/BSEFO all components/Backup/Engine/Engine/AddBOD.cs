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
        MySqlConnection _connect;
        clsWriteLog objWriteLog;
        public AddBOD()
        {
            InitializeComponent();
            objWriteLog = new clsWriteLog(Application.StartupPath + "\\Log");

        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                {
                    XtraMessageBox.Show("All details are mandatory");
                }
                else
                {
                    if (txt_Leverage.Text.ToString().Trim() == "0")
                    {
                        XtraMessageBox.Show("Leverage cannot be 0");
                        return;
                    }
                    else if ((comboBoxSpanOnly.SelectedItem.ToString().ToUpper() != "YES") && (comboBoxSpanOnly.SelectedItem.ToString().ToUpper() != "NO"))
                    {
                        XtraMessageBox.Show("Please select appropriate Span-Only Option from the given options.");
                    }
                    else
                    {
                        if (_connect.State != ConnectionState.Open)
                        {
                            _connect.Open();
                        }
                        MySqlCommand cmd = new MySqlCommand("insert into clientdetail(ClientID,ClientName,Margin,Adhoc,Deltamargin,gammamargin,thetamargin,vegamargin,lhsQty,rhsQty,SpanOnly,Leverage,ClientCode,CTCL_ID,UserID,SwastikID,ZONE,BRANCH,FAMILY,PRODUCT) values(@DealerID,@DealerName,@Margin,@Adhoc,@Deltamargin,@gammamargin,@thetamargin,@vegamargin,@lhsQty,@rhsQty,@SpanOnly,@Leverage,@ClientCode,@CTCL_ID,@UserID,@SwastikID,@ZONE,@BRANCH,@FAMILY,@PRODUCT)", _connect);        //Added SwastikID by Akshay on 05-01-2021
                        cmd.Parameters.AddWithValue("@DealerID",txt_ClientID.Text.ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@DealerName", txt_Name.Text.ToString());
                        cmd.Parameters.AddWithValue("@Margin", txt_Margin.Text.ToString());
                        cmd.Parameters.AddWithValue("@Adhoc", txt_Adhoc.Text.ToString());
                        cmd.Parameters.AddWithValue("@Deltamargin", txt_Delta.Text.ToString());
                        cmd.Parameters.AddWithValue("@gammamargin", txt_Gamma.Text.ToString());
                        cmd.Parameters.AddWithValue("@thetamargin", txt_Theta.Text.ToString());
                        cmd.Parameters.AddWithValue("@vegamargin", txt_Vega.Text.ToString());
                        cmd.Parameters.AddWithValue("@lhsQty", txt_LHS.Text.ToString());
                        cmd.Parameters.AddWithValue("@rhsQty", txt_RHS.Text.ToString());
                        cmd.Parameters.AddWithValue("@SpanOnly", comboBoxSpanOnly.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@Leverage", txt_Leverage.Text.ToString());

                        cmd.Parameters.AddWithValue("@ClientCode", txt_ClientCode.Text.ToString());
                        cmd.Parameters.AddWithValue("@CTCL_ID", txt_CTCL_ID.Text.ToString());
                        cmd.Parameters.AddWithValue("@UserID", txt_UserID.Text.ToString());

                        cmd.Parameters.AddWithValue("@SwastikID", txt_SwastikID.Text.ToString());        //Added SwastikID by Akshay on 05-01-2021  

                        cmd.Parameters.AddWithValue("@ZONE", txt_ZONE.Text.ToString());
                        cmd.Parameters.AddWithValue("@BRANCH", txt_BRANCH.Text.ToString());
                        cmd.Parameters.AddWithValue("@FAMILY", txt_FAMILY.Text.ToString());
                        cmd.Parameters.AddWithValue("@PRODUCT", txt_PRODUCTS.Text.ToString());

                        //cmd.Parameters.AddWithValue("@FixedLoss", comboBoxFixedLoss.SelectedItem.ToString());
                        cmd.ExecuteNonQuery();
                        InsertError("Client affected in database -" + txt_ClientID.Text.ToString().ToUpper() + "," + txt_Name.Text.ToString() + "," + txt_Margin.Text.ToString() + "," + txt_Adhoc.Text.ToString() + "," + txt_Delta.Text.ToString() + "," + txt_Theta.Text.ToString() + "," + txt_Gamma.Text.ToString() + "," + txt_Vega.Text.ToString() + "," + txt_LHS.Text.ToString() + "," + txt_RHS.Text.ToString() + ",Fixedloss-" + comboBoxFixedLoss.SelectedItem.ToString());
                        XtraMessageBox.Show("Record inserted successfully");
                    }
                }
            }
            catch (Exception InsertBodEx)
            {
                if (InsertBodEx.ToString().Contains("Duplicate"))
                {
                    XtraMessageBox.Show("User already exists");
                }
                else
                {
                    XtraMessageBox.Show("Please enter details in correct format");
                    InsertError(InsertBodEx.Message + ":" + InsertBodEx.StackTrace.ToString().Substring(InsertBodEx.StackTrace.ToString().Length - 10));
                }
            }
        }

        public bool ValidateInput()
        {
            if (txt_ClientID.Text == "") { return false; }
            if (txt_Name.Text == "") { return false; }
            if (txt_Margin.Text == "") { return false; }
            if (txt_Adhoc.Text == "") { return false; }
            if (txt_Delta.Text == "") { return false; }
            if (txt_Theta.Text == "") { return false; }
            if (txt_Vega.Text == "") { return false; }
            if (txt_Gamma.Text == "") { return false; }
            if (txt_LHS.Text == "") { return false; }
            if (txt_RHS.Text == "") { return false; }
            if (comboBoxSpanOnly.Text == "") { return false; }
            //if (comboBoxFixedLoss.Text == "") { return false; }
            return true;
        }

       
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddBOD_Load(object sender, EventArgs e)
        {
            GetConnection();
        }
        void GetConnection()
        {
            try
            {
                XmlTextReader tReader = new XmlTextReader("C:/Prime/PrimeDBConnection.xml");
                tReader.Read();
                DataSet ds = new DataSet();
                ds.ReadXml(tReader);
                //ds.Tables[0].Rows[0]["user"] = "Engine"; //added by navin on 02-07-2018
                _connect = new MySqlConnection("Data Source = " + ds.Tables[0].Rows[0]["Server"] + "; Port = " + ds.Tables[0].Rows[0]["Port"] + "; Initial Catalog = " + ds.Tables[0].Rows[0]["Database"] + "; user ID = " + ds.Tables[0].Rows[0]["user"] + "; Password = " + ds.Tables[0].Rows[0]["password"] + " ; SslMode=none ");
                _connect.Open();
            }
            catch (Exception arrcsDbEx)
            {
                InsertError(arrcsDbEx.StackTrace.ToString().Substring(arrcsDbEx.StackTrace.ToString().Length - 10));
            }
        }

        public void InsertError(string error)
        {
            try
            {
                objWriteLog.WriteLog("Add client : " + error +" _"+ DateTime.Now);
            }
            catch (Exception errorlogEx)
            {
                objWriteLog.WriteLog("Add client :  " + error+" _ " + errorlogEx.Message.ToString() + DateTime.Now);
            }
        }
    }
}