using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using System.IO;
using DevExpress.Utils.Menu;
using System.Windows.Forms;

//Name Of Sub-Client: BP

namespace Engine
{
    //added by nikhil | runtime client update
    public delegate void del_SendBodUpdateToGetway();

    public partial class BODProcess : XtraForm
    {
        internal event del_RefreshClientDetail eve_RefreshClientDetail;
        
        //added by Nikhil | runtime client update
        internal event del_SendBodUpdateToGetway eve_SendBodUpdateToGetway;

        clsWriteLog _logger;
        DataSet ds_Client = new DataSet();
        DataTable clientDetail = new DataTable();

        string _MySQLCon = string.Empty;

        DXMenuItem menu_ExportToCSV = new DXMenuItem();

        public BODProcess(string _ReceivedMySQLCon, clsWriteLog _logger)
        {
            InitializeComponent();

            this._logger = _logger;
            _MySQLCon = _ReceivedMySQLCon;

            //added on 09APR2021 by Amey
            menu_ExportToCSV.Caption = "Export As CSV";
            menu_ExportToCSV.Click += Menu_ExportToCSV_Click;

            GetClientDetails();
        }

        private void Menu_ExportToCSV_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "CSV file (*.csv)|*.csv";
                DialogResult result = _Save.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string folderName = _Save.FileName;
                    gv_ClientDetails.ExportToCsv(folderName);

                    XtraMessageBox.Show("Exported successfully", "Success");
                }
            }
            catch (Exception ee) { _logger.WriteLog("Menu_ExportToCSV_Click : " + ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }

        void GetClientDetails()
        {
            try
            {
                clientDetail.Clear();
                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                {
                    con_MySQL.Open();
                    try
                    {
                        using (MySqlCommand myCmd = new MySqlCommand("sp_GetClientDetail", con_MySQL))
                        {
                            myCmd.CommandType = CommandType.StoredProcedure;

                            //added on 27APR2021 by Amey
                            myCmd.Parameters.Add("prm_Type", MySqlDbType.LongText);
                            myCmd.Parameters["prm_Type"].Value = "ALL";

                            using (MySqlDataAdapter dadapt = new MySqlDataAdapter(myCmd))
                            {
                                dadapt.Fill(clientDetail);
                            }
                        }

                        gc_ClientDetails.DataSource = clientDetail;

                        //added on 12JAN2021 by Amey
                        gv_ClientDetails.BestFitColumns();
                    }
                    catch(Exception ee) { InsertError("GetClientDetails -inner : " + ee); }
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
            if (txt_UploadPath.Text == "")
            {
                XtraMessageBox.Show("Please Select A Path for Client Detail Files.");
            }
            else
            {
                try
                {
                    StringBuilder sCommand = new StringBuilder("INSERT INTO tbl_clientdetail(ClientID,DealerID,UserID,Username,Name,Margin,Adhoc,Zone,Branch,Family,Product,Segment) values(");
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
                                if (uploadClients.Length >= 11)   //17-11-17
                                {
                                    //Changed by Akshay on 08 - 01 - 2021 For new Columns
                                    if ((uploadClients[0].Trim() == "" && uploadClients[1].Trim() == "" && uploadClients[2].Trim() == "") || uploadClients[3].Trim() == "" || uploadClients[4].Trim() == "" || uploadClients[5].Trim() == "" || uploadClients[6].Trim() == "" || uploadClients[7].Trim() == "" || uploadClients[8].Trim() == "" || uploadClients[9].Trim() == "" || uploadClients[10].Trim() == "")
                                    {
                                        //added logs on 03MAY2021 by Amey
                                        InsertError("Client Upload Empty : " + line1);
                                        XtraMessageBox.Show("Please Ensure all fields are present");
                                        return;
                                    }
                                    _rowsAffected++;//02-01-2020

                                    detail.Append("'" + uploadClients[0].ToUpper().Trim() + "',");   //ClientID
                                    detail.Append("'" + uploadClients[1].ToUpper().Trim() + "',");   //DealerID
                                    detail.Append("'" + uploadClients[2].ToUpper().Trim() + "',");   //UserID
                                    detail.Append("'" + uploadClients[3].ToUpper().Trim() + "',");   //UserName
                                    detail.Append("'" + uploadClients[4].ToUpper().Trim() + "',");   //Name

                                    //added on 31DEC2020 by Amey
                                    try
                                    {
                                        decimal Margin = Convert.ToDecimal(uploadClients[5]);
                                        decimal AdHoc = Convert.ToDecimal(uploadClients[6]);

                                        if (AdHoc < 0)
                                        {
                                            InsertError("Client Upload Loop Negetive : " + line1);
                                            XtraMessageBox.Show("Please Check Client information in the Client Data File");
                                            return;
                                        }

                                        detail.Append("" + Margin + ",");
                                        detail.Append("" + AdHoc + ",");
                                    }
                                    catch (Exception ee)
                                    {
                                        InsertError("Client Upload Loop : " + line1 + Environment.NewLine + ee);
                                        XtraMessageBox.Show("Please Check Client information in the Client Data File");
                                        return;
                                    }

                                    detail.Append("'" + uploadClients[7].ToUpper().Trim() + "',");          //Zone          
                                    detail.Append("'" + uploadClients[8].ToUpper().Trim() + "',");          //Branch        
                                    detail.Append("'" + uploadClients[9].ToUpper().Trim() + "',");          //Family        
                                    detail.Append("'" + uploadClients[10].ToUpper().Trim() + "',");           //Product 
                                    detail.Append("'" + uploadClients[11].ToUpper().Trim() + "'");           //Segment //Added by Snehadri on 16JUL2022

                                    detail.Append("),(");
                                }
                                else
                                {
                                    //added logs on 03MAY2021 by Amey
                                    InsertError("Client Upload Loop Incomplete : " + line1);
                                    XtraMessageBox.Show("Incomplete client data");
                                    return;
                                }
                            }
                            try
                            {
                                if (strType == "Complete")
                                {
                                    InsertError("Full Client upload started");

                                    //added on 29JAN2021 by Amey
                                    using (var con_MySQL = new MySqlConnection(_MySQLCon))
                                    {
                                        con_MySQL.Open();
                                        using (var cmd = new MySqlCommand("TRUNCATE tbl_clientdetail ", con_MySQL))
                                        {
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();
                                        }
                                    }
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
                                sCommand.Append("ON DUPLICATE KEY UPDATE `Username`= VALUES(`Username`),`Name`= VALUES(`Name`),`Margin`= VALUES(`Margin`),`Adhoc`= VALUES(`Adhoc`),`Zone`= VALUES(`Zone`),`Branch`= VALUES(`Branch`),`Family`= VALUES(`Family`),`Product`= VALUES(`Product`),`Segment`= VALUES(`Segment`);");
                                //sCommand.Append(";");

                                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                                {
                                    con_MySQL.Open();

                                    using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), con_MySQL))
                                    {
                                        myCmd.CommandType = CommandType.Text;
                                        myCmd.ExecuteNonQuery();
                                        myCmd.Dispose();
                                    }
                                }
                            }
                        }
                    }

                    InsertError("Client Uploaded Successfully.");
                    InsertError("Row count " + _rowsAffected + ", Total time taken for client upload " + (DateTime.Now - dt_StartTime));
                    txt_UploadPath.Text = string.Empty;
                    XtraMessageBox.Show("Client Uploaded Successfully.");
                    GetClientDetails();
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

        private void BODProcess_Load(object sender, EventArgs e)
        {
            gv_ClientDetails.OptionsBehavior.Editable = true;
        }

        #region Insert Error into Database
        public void InsertError(string error)
        {
            try
            {
                _logger.WriteLog(error + "  _" + DateTime.Now.ToString());
            }
            catch (Exception)
            {
                //XtraMessageBox.Show("Error Occured in Engine. The Application will now Exit");
                //Application.Exit();
            }
        }
        #endregion
  
        private void btnUpdate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DataRowView temp_view = (DataRowView)gv_ClientDetails.GetFocusedRow();

                //added on 29JAN2021 by Amey
                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                {
                    con_MySQL.Open();

                    using (var cmd = new MySqlCommand("UPDATE tbl_clientdetail SET Username=@Username, Name=@Name, Margin=@Margin, Adhoc=@Adhoc, Zone=@Zone, Branch=@Branch, Family=@Family, Product=@Product, Segment=@Segment WHERE ClientID=@ClientID AND DealerID=@DealerID AND UserID=@UserID", con_MySQL))
                    {
                        cmd.Parameters.AddWithValue("@ClientID", temp_view["ClientID"].ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@DealerID", temp_view["DealerID"].ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@UserID", temp_view["UserID"].ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@Username", temp_view["Username"].ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@Name", temp_view["Name"].ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@Margin", temp_view["Margin"].ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@Adhoc", temp_view["Adhoc"].ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@Zone", temp_view["Zone"].ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@Branch", temp_view["Branch"].ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@Family", temp_view["Family"].ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@Product", temp_view["Product"].ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@Segment", temp_view["Segment"].ToString().ToUpper()); // Added by Snehadri on 16JUL2022 for Client-Segment biforcation
                        cmd.ExecuteNonQuery();
                    }
                    XtraMessageBox.Show("Records updated successfully");
                }
                GetClientDetails();
            }
            catch (Exception updateEx)
            {
                InsertError(updateEx.StackTrace.ToString().Substring(updateEx.StackTrace.ToString().Length - 10));
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
                AddBOD add = new AddBOD(_MySQLCon, _logger);
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

        //added on 31DEC2020 by Amey
        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //added on 29JAN2021 by Amey
                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                {
                    con_MySQL.Open();

                    //changed on 31DEC2020 by Amey
                    using (var cmd = new MySqlCommand("DELETE FROM tbl_clientdetail WHERE ClientID=@ClientID AND DealerID=@DealerID AND UserID=@UserID AND Username=@Username", con_MySQL))
                    {
                        //changed on 31DEC2020 by Amey
                        cmd.Parameters.AddWithValue("@ClientID", gv_ClientDetails.GetFocusedRowCellValue("ClientID").ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@DealerID", gv_ClientDetails.GetFocusedRowCellValue("DealerID").ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@UserID", gv_ClientDetails.GetFocusedRowCellValue("UserID").ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@Username", gv_ClientDetails.GetFocusedRowCellValue("Username").ToString().ToUpper());

                        cmd.ExecuteNonQuery();
                        GetClientDetails();
                    }
                }
                XtraMessageBox.Show("Record deleted successfully");
            }
            catch (Exception ee) { InsertError("btnDelete_ButtonClick : " + ee); }
        }

        //added on 31DEC2020 by Amey
        //private void gc_ClientDetails_DataSourceChanged(object sender, EventArgs e)
        //{
            //gv_ClientDetails.Columns.ColumnByFieldName("ID").OptionsColumn.AllowEdit = false;
            //gv_ClientDetails.Columns.ColumnByFieldName("Username").OptionsColumn.AllowEdit = false;
        //}

        private void gv_ClientDetails_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName.ToUpper() == "UPDATE")
                e.RepositoryItem = gc_ClientDetails.RepositoryItems["btnUpdate"];
            else if (e.Column.FieldName.ToUpper() == "DELETE")
                e.RepositoryItem = gc_ClientDetails.RepositoryItems["btnDelete"];
        }

        private void BODProcess_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            //added on 17FEB2021 by Amey
            eve_RefreshClientDetail();

            //added by Nikhil | runtime client update
            eve_SendBodUpdateToGetway();
        }

        private void gv_ClientDetails_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            e.Menu.Items.Add(menu_ExportToCSV);
        }
    }
}