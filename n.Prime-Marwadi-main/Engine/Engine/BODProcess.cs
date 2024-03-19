using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using System.Xml;
using System.IO;
using System.Threading;

//Name Of Sub-Client: BP

namespace Engine
{
    public partial class BODProcess : DevExpress.XtraEditors.XtraForm
    {
        clsWriteLog objWriteLog;
        string _connString = string.Empty;
        DataSet ds_Client = new DataSet();
        DataTable clientDetail = new DataTable();
        List<string> groupMaster = new List<string>();
        DataTable dt_allGroups = new DataTable();
        MySqlConnection arrcsDB;
        MySqlCommand cmd;
        
        //char[] b;

        public BODProcess()
        {
            InitializeComponent();
            objWriteLog = new clsWriteLog(Application.StartupPath + "\\Log");
            GetArrcsConnection();
            GetClientDetails();
           // GetGroupDetails(); //temp comment by Navin on 10-06-2019
        }
        void GetGroupDetails()
        {
            try
            {
                DataTable dt_grp = new DataTable();
                MySqlCommand cmd = new MySqlCommand("Select Distinct `Group` from groupmaster_client", arrcsDB);
                if (arrcsDB.State == ConnectionState.Closed)
                    arrcsDB.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt_grp);
                MySqlCommand cmdnew = new MySqlCommand("select sc.`Client`,gm.`group` from Client as sc left join groupmaster_Client as gm on sc.`Groupno`=gm.Groupno;", arrcsDB);
                if (arrcsDB.State == ConnectionState.Closed)
                    arrcsDB.Open();
                MySqlDataAdapter da2 = new MySqlDataAdapter(cmdnew);
                da2.Fill(dt_allGroups);
            }
            catch (Exception groupEx)
            {
                InsertError(groupEx.StackTrace.ToString().Substring(groupEx.StackTrace.ToString().Length - 10));
                //lstError_BOD.Items.Insert(0, "Exception in group details");
            }
        }
      
        void GetArrcsConnection()
        {
            try
            {
                XmlTextReader tReader = new XmlTextReader("C:/Prime/PrimeDBConnection.xml");
                tReader.Read();
                ds_Client.ReadXml(tReader);
                arrcsDB = new MySqlConnection("Data Source = " + ds_Client.Tables[0].Rows[0]["Server"] + "; Port = " + ds_Client.Tables[0].Rows[0]["Port"] + "; Initial Catalog = " + ds_Client.Tables[0].Rows[0]["Database"] + "; user ID = " + ds_Client.Tables[0].Rows[0]["user"] + "; Password = " + ds_Client.Tables[0].Rows[0]["password"] + " ;SslMode=none ");
                arrcsDB.Open();
                _connString = "Data Source = " + ds_Client.Tables[0].Rows[0]["Server"] + "; Port = " + ds_Client.Tables[0].Rows[0]["Port"] + "; Initial Catalog = " + ds_Client.Tables[0].Rows[0]["Database"] + "; user ID = " + ds_Client.Tables[0].Rows[0]["user"] + "; Password = " + ds_Client.Tables[0].Rows[0]["password"] + " ;SslMode=none "; //added by Navin on 10-06-2019
            }
            catch (Exception arrcsDbEx)
            {
                InsertError("Database connection " + arrcsDbEx.ToString());
            }
        }
        void GetClientDetails()
        {
            try
            {
                clientDetail.Clear();
                using (MySqlConnection myConnClient = new MySqlConnection(_connString))
                {
                    MySqlCommand myCmd = new MySqlCommand("sp_BODClients", myConnClient);//modified by Navin on 12-06-2019
                    myCmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataAdapter dadapt = new MySqlDataAdapter(myCmd);
                    myConnClient.Open();
                    dadapt.Fill(clientDetail);
                    myConnClient.Close();
                    //test(clientDetail); //9-11-17
                    gc_ClientDetails.DataSource = clientDetail;
                }
            }
            catch (Exception clientDetailEx)
            {
                InsertError("BOD getClientDetails " + clientDetailEx.ToString());
            }
        }

        private void btn_Upload_Click(object sender, EventArgs e)       //To Upload Clients From Files
        {
            UploadClient("Complete");
        }

        void UploadClient(string strType)
        {
            if (arrcsDB.State != ConnectionState.Open)
            {
                arrcsDB.Open();
            }
            if (txt_UploadPath.Text == "")
            {
                XtraMessageBox.Show("Please Select A Path for Client Detail Files.");
            }
            else
            {
                try
                {
                    StringBuilder sCommand = new StringBuilder("insert into clientdetail(ClientID,ClientName,Margin,Adhoc,Deltamargin,gammamargin,thetamargin,vegamargin,lhsQty,rhsQty,SpanOnly,Leverage,ClientCode,CTCL_ID,UserID,SwastikID,Zone,Branch,Family,Product) values(");   //Changed on 05-01-2021 by Akshay for extra columns
                    using (arrcsDB)
                    {
                        int _rowsAffected = 0; DateTime dt_StartTime = DateTime.Now;//02-01-2020
                        InsertError("Client file selected " + txt_UploadPath.Text);
                        using (FileStream stream = File.Open(txt_UploadPath.Text.Trim(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string line1;
                                StringBuilder detail = new StringBuilder();
                                string[] uploadClients;
                                List<StringBuilder> Rows = new List<StringBuilder>();
                                while ((line1 = reader.ReadLine()) != null)
                                {
                                    uploadClients = line1.Split(',');
                                    if (uploadClients.Length >= 18)   //17-11-17        //Changed 14 to 18 on 2-11-2020 by Akshay for extra columns
                                    {
                                        if (uploadClients.Contains("") || (uploadClients.Contains(" ")))
                                        {
                                            XtraMessageBox.Show("Please Ensure all fields are present");
                                            return;
                                        }
                                        _rowsAffected++;//02-01-2020
                                        #region Without Encryption added  by Navin on 12-06-2019
                                        //strClient.Append("'" + uploadClients[0].ToUpper() + "',");//added by Navin on 12-06-2019 
                                        detail.Append("'" + uploadClients[0].ToUpper().Trim() + "',");
                                        detail.Append("'" + uploadClients[1].Trim() + "',");
                                        for (int j = 2; j < 10; j++)                //Changed 9 to 10 to avoid skiping one value on 02-11-2020 by Akshay for extra columns
                                        {
                                            if (IsDigitsOnly(uploadClients[j]))
                                            {
                                                detail.Append("" + uploadClients[j] + ",");
                                            }
                                            else
                                            {
                                                XtraMessageBox.Show("Please Check Client information in the Client Data File");
                                                return;
                                            }
                                        }
                                        string spanonly = uploadClients[10] == "1" ? "YES" : "NO";
                                        detail.Append("'" + spanonly + "',");
                                        detail.Append("" + uploadClients[11] + ",");
                                        //detail.Append("'" + uploadClients[12] == "1" ? "true" : "false" + "'");

                                        //commented on 23AUG2020 by Amey to remove FixedLoss Entry
                                        //detail.Append("'" + (uploadClients[12] == "1" ? "YES" : "NO") + "'"); 

                                        detail.Append("'" + uploadClients[12] + "',");        //ClientCode
                                        detail.Append("'" + uploadClients[13] + "',");        //CTCL_ID
                                        detail.Append("'" + uploadClients[14] + "',");        //UserID
                                        detail.Append("'" + uploadClients[15] + "',");        //SwastikID       //Added by Akshay on 05-01-2021 
                                        detail.Append("'" + uploadClients[16] + "',");        //Zone          //Added on 02-11-2020 by Akshay
                                        detail.Append("'" + uploadClients[17] + "',");        //Branch        //Added on 02-11-2020 by Akshay
                                        detail.Append("'" + uploadClients[18] + "',");        //Family        //Added on 02-11-2020 by Akshay
                                        detail.Append("'" + uploadClients[19] + "'");        //Product       //Remove comma for last column added on 02-11-2020 by Akshay

                                        detail.Append("),(");
                                        #endregion
                                    }
                                    else
                                    {
                                        if (!((uploadClients.Length == 1) && (uploadClients[0] == "")))
                                        {
                                            XtraMessageBox.Show("Incomplete client data");
                                            return;
                                        }
                                    }
                                }
                                try
                                {
                                    if (strType == "Complete")
                                    {
                                        InsertError("Full Client upload started");
                                        cmd = new MySqlCommand("truncate clientdetail ", arrcsDB);// modified by Navin on 12-06-2019
                                        if (arrcsDB.State == ConnectionState.Closed)
                                            arrcsDB.Open();
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();
                                    }
                                    else
                                        InsertError("Partial Client upload started");
                                }
                                catch (Exception trunEx)
                                {
                                    InsertError("Upload client " + trunEx.ToString());
                                }
                                //}
                                //added on 22-12-17 to add only proper rows with proper data format
                                Rows.Add(detail);
                                if (Rows.Count != 0)
                                {
                                    sCommand.Append(string.Join(",", Rows));
                                    int a = sCommand.Length;
                                    sCommand.Remove(a - 2, 2);
                                    sCommand.Append("on duplicate key update `ClientName`= Values(`ClientName`),`Margin`= Values(`Margin`),`Adhoc`= Values(`Adhoc`),`FixedLoss`= Values(`FixedLoss`);");
                                    //sCommand.Append(";");
                                    using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), arrcsDB))
                                    {
                                        myCmd.CommandType = CommandType.Text;
                                        if (arrcsDB.State == ConnectionState.Closed)
                                            arrcsDB.Open();
                                        myCmd.ExecuteNonQuery();
                                        myCmd.Dispose();
                                    }
                                }
                            }
                        }

                        InsertError("Client Uploaded Successfully.");
                        InsertError("Row count "+_rowsAffected+", Total time taken for client upload " + (DateTime.Now - dt_StartTime));
                        txt_UploadPath.Text = string.Empty;
                        XtraMessageBox.Show("Client Uploaded Successfully.");
                        GetClientDetails();
                    }
                }
                catch (Exception uploadEx)
                {
                    if (uploadEx.Message.ToString().Contains("Duplicate"))
                    {
                        XtraMessageBox.Show("Multiple Records for the same user. Please check the client file.");
                    }
                    else
                    {
                        InsertError(uploadEx.ToString());
                        XtraMessageBox.Show("Please check the client file.");
                    }
                }
            }
        }
        bool IsDigitsOnly(string str)
        {
            try
            {
                foreach (char c in str)
                {
                    if (c == '.') continue;
                    if (c < '0' || c > '9')
                        return false;
                }
            }
            catch(Exception digEx)
            {
                InsertError(digEx.StackTrace.ToString().Substring(digEx.StackTrace.ToString().Length - 10));
            }
            return true;
        }

        public bool ValidateInput()
        {
            //if(txt_ClientID.Text=="") { return false; }
            //if (txt_Name.Text == "") { return false; }
            //if (txt_Margin.Text == "") { return false; }
            //if (txt_Adhoc.Text == "") { return false; }
            //if (txt_Delta.Text == "") { return false; }
            //if (txt_Theta.Text == "") { return false; }
            //if (txt_Vega.Text == "") { return false; }
            //if (txt_Gamma.Text == "") { return false; }
            //if (txt_LHS.Text == "") { return false; }
            //if (txt_RHS.Text == "") { return false; }
            //if (comboBoxSpanOnly.Text == "") { return false; }
            //if (txt_Leverage.Text == "") { return false; }
            return true;
        }

        private void btn_DoneBOD_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Browse_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txt_UploadPath.Text = openFileDialog1.FileName;
                }
            }
            catch (Exception pathEx)
            {
                InsertError(pathEx.ToString());
                //lstError_BOD.Items.Insert(0,"Error occurred while selecting upload path");
            }
        }

        void Cmd_addnewgrp()
        {
            try
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    //checkCmbGroups.Properties.Items.Clear();
                    DataTable dt_grp = new DataTable();
                    MySqlCommand cmd = new MySqlCommand("Select Distinct `Group` from groupmaster_client", arrcsDB);
                    if (arrcsDB.State == ConnectionState.Closed)
                        arrcsDB.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    da.Fill(dt_grp);
                    //foreach (DataRow R in dt_grp.Rows)
                    //{
                    //    checkCmbGroups.Properties.Items.Add(R["Group"].ToString());
                    //}

                    dt_allGroups.Clear();
                    MySqlCommand cmdnew = new MySqlCommand("select sc.`Client`,gm.`group` from Client as sc left join groupmaster_Client as gm on sc.`Groupno`=gm.Groupno;", arrcsDB);
                    if (arrcsDB.State == ConnectionState.Closed)
                        arrcsDB.Open();
                    MySqlDataAdapter da2 = new MySqlDataAdapter(cmdnew);
                    da2.Fill(dt_allGroups);
                }));

            }
            catch (Exception groupEx)

            {
                InsertError(groupEx.StackTrace.ToString().Substring(groupEx.StackTrace.ToString().Length - 10));
                //lstError_BOD.Items.Insert(0, "Error in group details");
            }
        }

        private void GridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                gc_ClientDetails.EmbeddedNavigator.Buttons.EndEdit.Enabled = true;
                gc_ClientDetails.EmbeddedNavigator.Buttons.CancelEdit.Enabled = true;
                gc_ClientDetails.EmbeddedNavigator.Buttons.Append.Enabled = false;
                gc_ClientDetails.EmbeddedNavigator.Buttons.Remove.Enabled = false;
            }
            catch (Exception ee)
            {
                InsertError(ee.ToString());
            }
        }

        private void GridView1_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                if (gv_ClientDetails.FocusedColumn.Caption == "Groups")
                {
                    if (!(gv_ClientDetails.ActiveEditor is ComboBoxEdit combo)) return;
                    DataRowView temp_view = (DataRowView)gv_ClientDetails.GetFocusedRow();
                    string cb_dealer = temp_view[0].ToString();
                    foreach (DataRow r in dt_allGroups.Rows)
                    {
                        if (r["Client"].ToString()== cb_dealer)
                        {
                            combo.Properties.Items.Add(r["Group"].ToString());
                        }
                    }
                    if (combo.Properties.Items.Count == 0)
                    {
                        combo.Properties.Items.Add("No groups");
                    }           
                }
            }
            catch (Exception ee)
            {
                InsertError(ee.ToString());
            }
        }

        private void BODProcess_Load(object sender, EventArgs e)
        {
            gv_ClientDetails.OptionsBehavior.Editable = true;
        }

        #region Insert Error into Database
        public void InsertError(string error)
        {
            try
            {
                objWriteLog.WriteLog(error + "  _" + DateTime.Now.ToString());
            }
            catch (Exception)
            {
                //XtraMessageBox.Show("Error Occured in Engine. The Application will now Exit");
                //Application.Exit();
            }
        }
        #endregion

        private void GridControlBOD_EmbeddedNavigator_ButtonClick(object sender, NavigatorButtonClickEventArgs e)
        {
            try
            {
                if (e.Button.ButtonType == NavigatorButtonType.Append)
                {
                    gc_ClientDetails.EmbeddedNavigator.Buttons.EndEdit.Enabled = true;
                    gc_ClientDetails.EmbeddedNavigator.Buttons.CancelEdit.Enabled = true;
                    gc_ClientDetails.EmbeddedNavigator.Buttons.Append.Enabled = false;
                    gc_ClientDetails.EmbeddedNavigator.Buttons.Remove.Enabled = false;
                    gv_ClientDetails.Columns[5].OptionsColumn.AllowEdit = false;
                   
                }
                else if (e.Button.ButtonType == NavigatorButtonType.EndEdit)
                {
                    gc_ClientDetails.EmbeddedNavigator.Buttons.Append.Enabled = true;
                    gc_ClientDetails.EmbeddedNavigator.Buttons.Remove.Enabled = true;
                    gc_ClientDetails.EmbeddedNavigator.Buttons.EndEdit.Enabled = false;
                    gc_ClientDetails.EmbeddedNavigator.Buttons.CancelEdit.Enabled = false;
                    
                }
                else if (e.Button.ButtonType == NavigatorButtonType.CancelEdit)
                {
                    gc_ClientDetails.EmbeddedNavigator.Buttons.Append.Enabled = true;
                    gc_ClientDetails.EmbeddedNavigator.Buttons.Remove.Enabled = true;
                    gc_ClientDetails.EmbeddedNavigator.Buttons.EndEdit.Enabled = false;
                    gc_ClientDetails.EmbeddedNavigator.Buttons.CancelEdit.Enabled = false;
                }
                else if (e.Button.ButtonType == NavigatorButtonType.Remove)
                {
                    try
                    {
                        DataRowView temp_view = (DataRowView)gv_ClientDetails.GetFocusedRow();
                        if (!ValidateInput())
                        {
                            XtraMessageBox.Show("Please select a record to delete");
                        }
                        else
                        {
                            cmd = new MySqlCommand("delete from clientdetail where ClientID =@DealerID", arrcsDB);
                            if (arrcsDB.State == ConnectionState.Closed)
                                arrcsDB.Open();
                            cmd.Parameters.AddWithValue("@DealerID",temp_view["DealerID"].ToString().ToUpper());
                            cmd.ExecuteNonQuery();
                            GetClientDetails();
                            XtraMessageBox.Show("Record deleted successfully");
                        }
                    }
                    catch (Exception clientDeleteEx)
                    {
                        InsertError(clientDeleteEx.StackTrace.ToString().Substring(clientDeleteEx.StackTrace.ToString().Length - 10));
                    }
                    finally
                    {
                        gc_ClientDetails.EmbeddedNavigator.Buttons.EndEdit.Enabled = true;
                        gc_ClientDetails.EmbeddedNavigator.Buttons.CancelEdit.Enabled = true;
                        gc_ClientDetails.EmbeddedNavigator.Buttons.Append.Enabled = false;
                        gc_ClientDetails.EmbeddedNavigator.Buttons.Remove.Enabled = false;
                    }
                }
            }
            catch (Exception ee)
            {
                InsertError("Delete record "+ee.ToString());
            }
        }
  
        private void btnUpdate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DataRowView temp_view = (DataRowView)gv_ClientDetails.GetFocusedRow();
                if (temp_view["Leverage"].ToString() == "0")
                {
                    XtraMessageBox.Show("Leverage cannot be 0");
                    return;
                }
                else if ((temp_view["SpanOnly"].ToString().ToUpper() != "YES") && (temp_view["SpanOnly"].ToString().ToUpper() != "NO"))
                {
                    XtraMessageBox.Show("Please select appropriate Span-Only Option from the given options.");
                    return;
                }
                else
                {
                    using (MySqlConnection myConn=new MySqlConnection(_connString))
                    {
                        //cmd = new MySqlCommand("update clientdetail set DealerName=@DealerName ,Margin=@Margin,Adhoc=@Adhoc,Deltamargin=@Deltamargin,gammamargin=@gammamargin,thetamargin=@thetamargin,vegamargin=@vegamargin,lhsQty=@lhsQty,rhsQty=@rhsQty,SpanOnly=@SpanOnly,Leverage=@Leverage, FixedLoss=@FixedLoss where ClientID=@DealerID", myConn);
                        cmd = new MySqlCommand("update clientdetail set ClientName=@DealerName ,Margin=@Margin,Adhoc=@Adhoc,Leverage=@Leverage,ClientCode=@ClientCode,CTCL_ID=@CTCL_ID,UserID=@UserID,SwastikID=@SwastikID,Zone=@Zone,Branch=@Branch,Family=@Family,Product=@Product  where ClientID=@ClientID", myConn);        //changed by Akshay on 05-01-2021 for extra columns
                        cmd.Parameters.AddWithValue("@ClientID", temp_view["DealerID"].ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@DealerName", temp_view["DealerName"].ToString());
                        cmd.Parameters.AddWithValue("@Margin", temp_view["Margin"].ToString());
                        cmd.Parameters.AddWithValue("@Adhoc", temp_view["Adhoc"].ToString());
                        cmd.Parameters.AddWithValue("@Leverage", temp_view["Leverage"].ToString());
                        cmd.Parameters.AddWithValue("@ClientCode", temp_view["ClientCode"].ToString());  //Added by Akshay on 19-11-2020 for ClientCode
                        cmd.Parameters.AddWithValue("@CTCL_ID", temp_view["CTCL_ID"].ToString());   //Added by Akshay on 19-11-2020 for CTCL_ID
                        cmd.Parameters.AddWithValue("@UserID", temp_view["UserID"].ToString());   //Added by Akshay on 19-11-2020 for UserID
                        cmd.Parameters.AddWithValue("@SwastikID", temp_view["SwastikID"].ToString());   //Added by Akshay on 05-01-2021 For SwastikID
                        cmd.Parameters.AddWithValue("@Zone", temp_view["Zone"].ToString());   //Added by Akshay on 20-11-2020 for Zone
                        cmd.Parameters.AddWithValue("@Branch", temp_view["Branch"].ToString());   //Added by Akshay on 20-11-2020 for Branch
                        cmd.Parameters.AddWithValue("@Family", temp_view["Family"].ToString());   //Added by Akshay on 20-11-2020 for Family
                        cmd.Parameters.AddWithValue("@Product", temp_view["Product"].ToString());   //Added by Akshay on 20-11-2020 for Product


                        //cmd.Parameters.AddWithValue("@FixedLoss", temp_view["FixedLoss"].ToString());
                        myConn.Open();
                        cmd.ExecuteNonQuery();
                        myConn.Close();
                        XtraMessageBox.Show("Records updated successfully");
                    }
                    GetClientDetails();
                }
            }
            catch (Exception updateEx)
            {
                InsertError("Update Click : " + updateEx);

                XtraMessageBox.Show("Record not updated. Please check logs for more details.");
            }
            finally
            {
                gc_ClientDetails.EmbeddedNavigator.Buttons.Append.Enabled = true;
                gc_ClientDetails.EmbeddedNavigator.Buttons.Remove.Enabled = true;
                gc_ClientDetails.EmbeddedNavigator.Buttons.EndEdit.Enabled = false;
                gc_ClientDetails.EmbeddedNavigator.Buttons.CancelEdit.Enabled = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                AddBOD add = new AddBOD();
                add.ShowDialog();
                GetClientDetails();
            }
            catch(Exception addEx)
            {
                InsertError(addEx.StackTrace.ToString().Substring(addEx.StackTrace.ToString().Length - 10));
            }
        }

        private void btnPartialClientUpdate_Click(object sender, EventArgs e)
        {
            UploadClient("Partial");
        }
    }
}