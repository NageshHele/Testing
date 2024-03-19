using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using MySql.Data.MySqlClient;

namespace Engine
{
    public delegate void Errorlogging(string strMessage);
    public partial class AddUser : DevExpress.XtraEditors.XtraForm
    {
        MySqlConnection mySqlConnection;
        internal event Errorlogging eveErrorlog;
        static Dictionary<string, string> dict_MappedClient = new Dictionary<string, string>();//23-12-2019
        static ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, HashSet<string>>>>> dict_ClientData = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, HashSet<string>>>>>();
        
        public AddUser(MySqlConnection myConn)
        {
            InitializeComponent();
            mySqlConnection = myConn;
            SelectClient();
            SelectUser();
        }
        void SelectClient()
        {
            try
            {
                using (MySqlCommand cmdClients =new MySqlCommand("sp_Clients",mySqlConnection))
                {
                    if (mySqlConnection.State == ConnectionState.Closed) mySqlConnection.Open();
                    MySqlDataReader mySqlDataReader = cmdClients.ExecuteReader();
                    while (mySqlDataReader.Read())
                    {
                        //commented on 05-11-2020 by Akshay for NewMappedClientsTreelist
                        //ccbe_MappedClients.Properties.Items.Add(mySqlDataReader.GetString(0));
                        //ccbe_Clients.Properties.Items.Add(mySqlDataReader.GetString(0));

                        //Added on 05-11-2020 by Akshay for newMappedClients treelist
                        string zone = mySqlDataReader.GetString(5);
                        string branch = mySqlDataReader.GetString(6);
                        string family = mySqlDataReader.GetString(7);
                        string product = mySqlDataReader.GetString(8);
                        string dealer = mySqlDataReader.GetString(0);

                        if (!dict_ClientData.ContainsKey(zone))
                        {
                            dict_ClientData.TryAdd(zone, new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, HashSet<string>>>>());
                        }

                        if (!dict_ClientData[zone].ContainsKey(branch))
                        {
                            dict_ClientData[zone].TryAdd(branch, new ConcurrentDictionary<string, ConcurrentDictionary<string, HashSet<string>>>());
                        }

                        if (!dict_ClientData[zone][branch].ContainsKey(family))
                        {
                            dict_ClientData[zone][branch].TryAdd(family, new ConcurrentDictionary<string, HashSet<string>>());
                        }

                        if (!dict_ClientData[zone][branch][family].ContainsKey(product))
                        {
                            dict_ClientData[zone][branch][family].TryAdd(product, new HashSet<string>());
                        }

                        dict_ClientData[zone][branch][family][product].Add(dealer);
                    }
                    mySqlDataReader.Close();
                    mySqlDataReader.Dispose();
                    mySqlConnection.Close();
                }

                //Added on 05-11-2020 by Akshay for newMappedClients treelist in Add user
                if (dict_ClientData.Count != 0)     //Added by Akshay For Avoiding error when Clientdata is Empty
                {
                    TreeListNode Selectednode = tl_SelectClients.AppendNode(null, null);
                    Selectednode.SetValue("name", "Select All");
                    foreach (var zoneitem in dict_ClientData)
                    {
                        TreeListNode zonenode = tl_SelectClients.AppendNode(null, Selectednode);
                        zonenode.SetValue("name", zoneitem.Key);
                        foreach (var branchitem in zoneitem.Value)
                        {
                            TreeListNode branchnode = tl_SelectClients.AppendNode(null, zonenode);
                            branchnode.SetValue("name", branchitem.Key);
                            foreach (var familyitem in branchitem.Value)
                            {
                                TreeListNode familynode = tl_SelectClients.AppendNode(null, branchnode);
                                familynode.SetValue("name", familyitem.Key);
                                foreach (var productitem in familyitem.Value)
                                {
                                    TreeListNode productnode = tl_SelectClients.AppendNode(null, familynode);
                                    productnode.SetValue("name", productitem.Key);
                                    foreach (var dealer in productitem.Value)
                                    {
                                        TreeListNode dealernode = tl_SelectClients.AppendNode(null, productnode);
                                        dealernode.SetValue("name", dealer);
                                    }
                                }
                            }
                        }
                    }
                }
                //Added on 05-11-2020 by Akshay for newMappedClients treelist in EditUser
                if (dict_ClientData.Count != 0)     //Added by Akshay For Avoiding error when Clientdata is Empty
                {
                    TreeListNode SelectMappednode = tl_MappedClients.AppendNode(null, null);
                    SelectMappednode.SetValue("name", "Select All");
                    foreach (var zoneitem in dict_ClientData)
                    {
                        TreeListNode zonenode = tl_MappedClients.AppendNode(null, SelectMappednode);
                        zonenode.SetValue("name", zoneitem.Key);
                        foreach (var branchitem in zoneitem.Value)
                        {
                            TreeListNode branchnode = tl_MappedClients.AppendNode(null, zonenode);
                            branchnode.SetValue("name", branchitem.Key);
                            foreach (var familyitem in branchitem.Value)
                            {
                                TreeListNode familynode = tl_MappedClients.AppendNode(null, branchnode);
                                familynode.SetValue("name", familyitem.Key);
                                foreach (var productitem in familyitem.Value)
                                {
                                    TreeListNode productnode = tl_MappedClients.AppendNode(null, familynode);
                                    productnode.SetValue("name", productitem.Key);
                                    foreach (var dealer in productitem.Value)
                                    {
                                        TreeListNode dealernode = tl_MappedClients.AppendNode(null, productnode);
                                        dealernode.SetValue("name", dealer);
                                    }
                                }
                            }
                        }
                    }
                }
                
            }
            catch (Exception error)
            {
                eveErrorlog("Bind client " + error.ToString());
            }
        }
        void SelectUser()
        {
            try
            {
                cmb_RUsername.Properties.Items.Clear();    //Added on 12-11-2020 by Akshay For Clearing Existing Items
                ccbe_Username.Properties.Items.Clear();    //Added on 12-11-2020 by Akshay For Clearing Existing Items
                using (MySqlDataAdapter mydaUsers = new MySqlDataAdapter("select convert(UserName using utf8) as UserName,convert(Password using utf8) as Password,convert(IsAdmin using utf8) as IsAdmin,convert(MappedClient using utf8) as MappedClient from login", mySqlConnection))
                {
                    if (mySqlConnection.State == ConnectionState.Closed) mySqlConnection.Open();
                    DataTable dtUser = new DataTable();
                    mydaUsers.Fill(dtUser);
                    clsEncryptionDecryption.DecryptData(dtUser);
                    mydaUsers.Dispose();
                    mySqlConnection.Close();
                    for (int iUser = 0; iUser < dtUser.Rows.Count; iUser++)
                    {
                        cmb_RUsername.Properties.Items.Add(dtUser.Rows[iUser][0].ToString());
                        ccbe_Username.Properties.Items.Add(dtUser.Rows[iUser][0].ToString());
                        dict_MappedClient[dtUser.Rows[iUser][0].ToString()] = dtUser.Rows[iUser][3].ToString();
                    }
                }
            }
            catch (Exception error)
            {
                eveErrorlog("Bind client " + error.ToString());
            }
        }
        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (txteUsername.Text.Trim() != "" && txtePassword.Text.Trim() != "")
                {
                    StringBuilder sbMappedClient = new StringBuilder();
                    try
                    {
                        //Added on 05-11-2020 by akshay for new treelist mappedclients
                        if (ckeIsAdmin.Checked.ToString() == "True")
                        {
                            foreach (TreeListNode node in tl_SelectClients.GetNodeList())
                            {
                                tl_SelectClients.SetNodeCheckState(node, CheckState.Checked, true);
                            }
                        }

                        List<string> lst_MappedClients = new List<string>();
                        List<TreeListNode> lst_SelectedClients = tl_SelectClients.GetAllCheckedNodes();
                        foreach (var client in lst_SelectedClients)
                        {
                            if (client.HasChildren == false)
                            {
                                lst_MappedClients.Add(client.GetValue(tlc_Clients).ToString());
                            }
                        }

                        if (lst_MappedClients.Count > 0)
                        {
                            //sbMappedClient.Append("'");
                            for (int i = 0; i < lst_MappedClients.Count; i++)
                            {
                                sbMappedClient.Append(lst_MappedClients[i] + ",");
                            }
                            if (sbMappedClient.Length > 1)
                                sbMappedClient.Remove(sbMappedClient.ToString().LastIndexOf(','), 1);
                            //sbMappedClient.Append("'");
                        }

                        
                        //Commented on 05-11-2020 by akshay for new treelist clientMapped
                        //if (ccbe_MappedClients.Properties.Items.Count > 0)
                        //{
                        //    //sbMappedClient.Append("'");
                        //    for (int i = 0; i < ccbe_MappedClients.Properties.Items.Count; i++)
                        //    {
                        //        if (ccbe_MappedClients.Properties.Items[i].CheckState.ToString().ToUpper() == "CHECKED")
                        //            sbMappedClient.Append(ccbe_MappedClients.Properties.Items[i].Value + ",");
                        //    }
                        //    if (sbMappedClient.Length > 1)
                        //        sbMappedClient.Remove(sbMappedClient.ToString().LastIndexOf(','), 1);
                        //    //sbMappedClient.Append("'");
                        //}
                    }
                    catch (Exception mappedEx)
                    {
                        eveErrorlog("Mapped client "+mappedEx.ToString());
                    }
                    
                    if (mySqlConnection.State == ConnectionState.Closed) mySqlConnection.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO `login` (`UserName`, `Password`, `IsAdmin`,MappedClient) VALUES ( '" + clsEncryptionDecryption.EncryptString(txteUsername.Text.Trim().ToLower(), "Nerve123") + "','" + clsEncryptionDecryption.EncryptString(txtePassword.Text.Trim(), "Nerve123") + "','" + clsEncryptionDecryption.EncryptString(ckeIsAdmin.Checked.ToString(), "Nerve123") + "','" + clsEncryptionDecryption.EncryptString(sbMappedClient.ToString(), "Nerve123") + "')", mySqlConnection);//original
                    //MySqlCommand cmd = new MySqlCommand("INSERT INTO `login` (`UserName`, `Password`, `IsAdmin`,MappedClient,UserType,UserID) VALUES ( '" + clsEncryptionDecryption.EncryptString(txteUsername.Text.Trim().ToLower(), "Nerve123") + "','" + clsEncryptionDecryption.EncryptString(txtePassword.Text.Trim(), "Nerve123") + "','" + clsEncryptionDecryption.EncryptString(ckeIsAdmin.Checked.ToString(), "Nerve123") + "','" + clsEncryptionDecryption.EncryptString(sbMappedClient.ToString(), "Nerve123") + "','" + clsEncryptionDecryption.EncryptString("Admin", "Nerve123") + "','" + clsEncryptionDecryption.EncryptString(sbMappedClient.ToString(), "Nerve123") + "')", mySqlConnection);//with 2 additional column
                    int result= cmd.ExecuteNonQuery();
                    if (result == 1)
                    {
                        cmb_RUsername.Properties.Items.Add(txteUsername.Text.Trim().ToLower());//02-01-2020
                        ccbe_Username.Properties.Items.Add(txteUsername.Text.Trim().ToLower());//02-01-2020
                        XtraMessageBox.Show("User added successfully");
                        eveErrorlog("User " + txteUsername.Text.Trim().ToLower() + " Added successfully. Mapped client-" + sbMappedClient.ToString());//12-12-2019
                    }
                    else
                        XtraMessageBox.Show("Unable to create user, Please try after some time");
                    //mySqlConnection.Close(); //added by akshay on 10-11-2020 for NewMappedClients
                }
                else
                    XtraMessageBox.Show("Please enter all details");
            }
            catch (Exception addEx)
            {
                eveErrorlog("Add user "+addEx.ToString());
            }
        }

        private void BtnResetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmb_RUsername.SelectedItem.ToString().Trim() != "")
                {
                    if (btnResetPassword.Text == "Reset Password")
                    {
                        if (mySqlConnection.State == ConnectionState.Closed) mySqlConnection.Open();
                        MySqlCommand cmd = new MySqlCommand("Update login SET Password = '" + clsEncryptionDecryption.EncryptString("Prime123", "Nerve123") + "' where UserName= '" + clsEncryptionDecryption.EncryptString(cmb_RUsername.SelectedItem.ToString().ToLower(), "Nerve123") + "'", mySqlConnection);
                        int result = cmd.ExecuteNonQuery();
                        if (result == 1)
                        {
                            eveErrorlog("Password resetted for user " + cmb_RUsername.SelectedItem.ToString()+" default password is Prime123");
                            XtraMessageBox.Show("Password reset successful");
                        }
                        else
                            XtraMessageBox.Show("User does not exist");
                    }
                    else
                    {
                        if (mySqlConnection.State == ConnectionState.Closed) mySqlConnection.Open();
                        MySqlCommand cmd = new MySqlCommand("delete from login where UserName= '" + clsEncryptionDecryption.EncryptString(cmb_RUsername.SelectedItem.ToString().Trim().ToLower(), "Nerve123") + "'", mySqlConnection);
                        int result = cmd.ExecuteNonQuery();
                        if (result == 1)
                        {
                            cmb_RUsername.Properties.Items.Remove(cmb_RUsername.SelectedItem.ToString());

                            eveErrorlog("User- " + cmb_RUsername.SelectedItem.ToString()+" deleted");
                            XtraMessageBox.Show("User deleted successfully");
                        }
                        else
                            XtraMessageBox.Show("User does not exist");
                    }
                    SelectUser();
                }
                else
                    XtraMessageBox.Show("Please enter username");
            }
            catch (Exception ResetEx)
            {
                eveErrorlog("Reset Password " + ResetEx.ToString());
            }
        }

        private void BtnRAdd_Click(object sender, EventArgs e)
        {
            pnlEditMappedUsers.Visible = false;
            pnlResetPassword.Visible = false;
            pnlSelectClients.Visible = false;
            pnlAddUser.Visible = true;
        }

        private void BtnAReset_Click(object sender, EventArgs e)
        {
            pnlResetPassword.Visible = true;
            pnlAddUser.Visible = false;
            pnlSelectClients.Visible = false;
            btnResetPassword.Text = "Reset Password";
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            pnlResetPassword.Visible = true;
            pnlAddUser.Visible = false;
            pnlSelectClients.Visible = false;
            btnResetPassword.Text = "Delete";
        }

        private void btnEditUserMapping_Click(object sender, EventArgs e)
        {
            pnlEditMappedUsers.Visible = true;
            pnlSelectClients.Visible = false;
            pnlResetPassword.Visible = false;
            pnlAddUser.Visible = false;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            pnlEditMappedUsers.Visible = false;
            pnlSelectClients.Visible = false;
            pnlResetPassword.Visible = false;
            pnlAddUser.Visible = true;
        }

        private void btnUpdateClient_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sbMappedClient = new StringBuilder();
                try
                {
                    //Added on 05-11-2020 by akshay for new treelist mappedclients
                    List<string> lst_MappedClients = new List<string>();
                    List<TreeListNode> lst_SelectedClients = tl_MappedClients.GetAllCheckedNodes();
                    foreach (var client in lst_SelectedClients)
                    {
                        if (client.HasChildren == false)
                        {
                            lst_MappedClients.Add(client.GetValue(tlc_MappedClients).ToString());
                        }
                    }

                    if (lst_MappedClients.Count > 0)
                    {
                        //sbMappedClient.Append("'");
                        for (int i = 0; i < lst_MappedClients.Count; i++)
                        {
                            sbMappedClient.Append(lst_MappedClients[i] + ",");
                        }
                        if (sbMappedClient.Length > 1)
                            sbMappedClient.Remove(sbMappedClient.ToString().LastIndexOf(','), 1);
                        //sbMappedClient.Append("'");
                    }

                    //Commented on 05-11-2020 by Akshay for NewMappedClients
                    //if (ccbe_Clients.Properties.Items.Count > 0)
                    //{
                    //    for (int i = 0; i < ccbe_Clients.Properties.Items.Count; i++)
                    //    {
                    //        if (ccbe_Clients.Properties.Items[i].CheckState.ToString().ToUpper() == "CHECKED")
                    //            sbMappedClient.Append(ccbe_Clients.Properties.Items[i].Value + ",");
                    //    }
                    //    sbMappedClient.Remove(sbMappedClient.ToString().LastIndexOf(','), 1);
                    //}
                }
                catch (Exception mappedEx)
                {
                    eveErrorlog("Mapped client " + mappedEx.ToString());
                }

                if (mySqlConnection.State == ConnectionState.Closed) mySqlConnection.Open();
                MySqlCommand cmd = new MySqlCommand("Update login SET MappedClient = '" + clsEncryptionDecryption.EncryptString(sbMappedClient.ToString(), "Nerve123") + "' where UserName= '" + clsEncryptionDecryption.EncryptString(ccbe_Username.SelectedItem.ToString().ToLower(), "Nerve123") + "'", mySqlConnection);
                int result = cmd.ExecuteNonQuery();
                if (result == 1)
                {
                    XtraMessageBox.Show("Mapped client updated successfully");
                    eveErrorlog("Mapping updated for user " + ccbe_Username.SelectedItem.ToString().ToLower() + ", Mapped client-" + sbMappedClient.ToString());//12-12-2019
                }
                else
                    XtraMessageBox.Show("User does not exist");
                //mySqlConnection.Close(); //added by akshay on 10-11-2020 for NewMappedClients
            }
            catch (Exception updateEx)
            {
                eveErrorlog("Updating user mapping " + updateEx.ToString());
            }
        }

        #region 12-12-2019 
        private void btnBulkUpdate_Click(object sender, EventArgs e)
        {
            UpdateClientMapping("Complete");
        }

        void UpdateClientMapping(string strType)
        {
            try
            {
                DateTime _dtStartTime = DateTime.Now;
                Dictionary<string, StringBuilder> dict_Usermapping = new Dictionary<string, StringBuilder>();
                OpenFileDialog _openFileDialog = new OpenFileDialog();
                int _rowsAffected = 0, _totalRecordsPresent=0;//added by Navin on 20-01-2020
                if (_openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string uploadPath = _openFileDialog.FileName;
                    eveErrorlog("File selected for mapping " + uploadPath);
                    _dtStartTime = DateTime.Now;
                    using (FileStream stream = File.Open(uploadPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string _line1;
                            if (strType == "Complete")
                            {
                                eveErrorlog("Started complete user client mapping");
                                try
                                {
                                    for (int iuser = 0; iuser < dict_MappedClient.Count; iuser++)
                                    {
                                        dict_MappedClient[dict_MappedClient.ElementAt(iuser).Key] = string.Empty;
                                    }
                                }
                                catch (Exception error)
                                {
                                    eveErrorlog("Client mapping clear " + error.ToString());
                                }
                            }
                            else
                                eveErrorlog("Started partial user client mapping");
                            
                            while ((_line1 = reader.ReadLine()) != null)
                            {
                                try
                                {
                                    string[] fields = _line1.Split(',');
                                    _totalRecordsPresent++;
                                    if (fields[0].Trim().ToUpper() != "USER" && fields.Length >= 2)
                                    {
                                        
                                        if (dict_MappedClient.ContainsKey(fields[0].Trim().ToLower()))
                                        {
                                            _rowsAffected++;//02-01-2020
                                            if (dict_Usermapping.ContainsKey(fields[0].Trim().ToLower()))
                                            {
                                                if (!dict_Usermapping[fields[0].Trim().ToLower()].ToString().Contains(fields[1].Trim().ToUpper()) && !dict_MappedClient[fields[0].Trim().ToLower()].Contains(fields[1].Trim().ToUpper()))
                                                    dict_Usermapping[fields[0].Trim().ToLower()].Append("," + fields[1].Trim().ToUpper());
                                            }
                                            else
                                            {
                                                if (!dict_MappedClient[fields[0].Trim().ToLower()].Contains(fields[1].Trim().ToUpper()))
                                                    dict_Usermapping.Add(fields[0].Trim().ToLower(), new StringBuilder(fields[1].Trim().ToUpper()));
                                                else
                                                    dict_Usermapping.Add(fields[0].Trim().ToLower(), new StringBuilder());
                                            }
                                        }
                                        else
                                        {
                                            eveErrorlog("User `" + fields[0].Trim().ToLower()+ "` does not exists, Skipped record- "+_line1);
                                        }
                                    }
                                    else
                                        eveErrorlog("Data not in proper format- " + _line1);
                                }
                                catch (Exception)
                                {
                                    eveErrorlog("Data not in proper format- " + _line1);
                                }
                            }
                        }
                    }
                }
                foreach (KeyValuePair<string, StringBuilder> item in dict_Usermapping)
                {
                    try
                    {
                        if (mySqlConnection.State == ConnectionState.Closed) mySqlConnection.Open();
                        //if (dict_MappedClient[item.Key] != string.Empty)
                        //    item.Value.ToString() + "," + dict_MappedClient[item.Key];
                        MySqlCommand cmd = new MySqlCommand("Update login SET MappedClient = '" + clsEncryptionDecryption.EncryptString((dict_MappedClient[item.Key] == string.Empty? item.Value.ToString():(item.Value.ToString()==string.Empty? dict_MappedClient[item.Key]:item.Value + ","+dict_MappedClient[item.Key])), "Nerve123") + "' where UserName= '" + clsEncryptionDecryption.EncryptString(item.Key, "Nerve123") + "'", mySqlConnection);
                        int result = cmd.ExecuteNonQuery();
                        if (result == 1)
                        {
                            //XtraMessageBox.Show("Mapped client updated successfully");
                            eveErrorlog("Mapping updated for user `" + item.Key + "`, Mapped client-" + (dict_MappedClient[item.Key] == string.Empty ? item.Value.ToString() : (item.Value.ToString() == string.Empty ? dict_MappedClient[item.Key] : item.Value + "," + dict_MappedClient[item.Key])));//12-12-2019
                        }
                        else
                            eveErrorlog("User `"+item.Key+ "` does not exist");
                    }
                    catch (Exception error)
                    {
                        eveErrorlog("Exception occurred while updating user mapping for `"+ item.Key+ "`, "+error.ToString());
                    }
                }
                SelectMappedClient();
                XtraMessageBox.Show("Client mapping completed");
                eveErrorlog("Client mapping completed, "+ _totalRecordsPresent+" records present in file, " + _rowsAffected + " records affected in database.");//modified error message on 02-01-2020
                eveErrorlog("Time taken to upload client mapping " + (DateTime.Now - _dtStartTime));
            }
            catch (Exception pathEx)
            {
                eveErrorlog("UpdateClientMapping " + pathEx.ToString());
            }
        }
        #endregion

        private void btn_PartialUpdate_Click(object sender, EventArgs e)
        {
            UpdateClientMapping("Partial");
        }

        void SelectMappedClient()
        {
            try
            {
                //sp_SelectMappedClient
                DataTable dtClient = new DataTable();
                MySqlCommand myCmd = new MySqlCommand("sp_SelectMappedClient", mySqlConnection);//modified by Navin on 12-06-2019
                myCmd.CommandType = CommandType.StoredProcedure;
                MySqlDataAdapter dadapt = new MySqlDataAdapter(myCmd);
                if (mySqlConnection.State == ConnectionState.Closed)
                    mySqlConnection.Open();
                dadapt.Fill(dtClient);
                clsEncryptionDecryption.DecryptData(dtClient);
                mySqlConnection.Close();//added by akshay on 10-11-2020 for NewMappedClients
                dict_MappedClient.Clear();
                for (int iCl = 0; iCl < dtClient.Rows.Count; iCl++)
                {
                    dict_MappedClient[dtClient.Rows[iCl][0].ToString().Trim()] = dtClient.Rows[iCl][1].ToString().Trim();
                }
            }
            catch (Exception str)
            {
                eveErrorlog("Select mapped client " + str.ToString());
            }
        }

        private void ccbe_Username_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //Added on 10-11-2020 by Akshay for newMappedClients
                SelectUser();
                foreach (TreeListNode node in tl_MappedClients.GetNodeList())
                {
                    tl_MappedClients.SetNodeCheckState(node, CheckState.Unchecked, true);
                }
                string _mappedClient = string.Empty;
                if (dict_MappedClient.TryGetValue(ccbe_Username.SelectedItem.ToString(), out _mappedClient))
                {
                    string[] _IndividuaClient = _mappedClient.Split(',');
                    foreach (var item in _IndividuaClient)
                    {
                        //Commented by akshay on 10-11-2020 for new MappedClients
                        //if (ccbe_Clients.Properties.Items.Contains(item))
                        //    ccbe_Clients.Properties.Items.FirstOrDefault(x => x.Value.ToString() == item).CheckState = CheckState.Checked;

                        //Added by akshay on 10-11-2020 for new MappedClients
                        foreach (TreeListNode node in tl_MappedClients.GetNodeList())
                        {
                            if (node.GetValue(tlc_MappedClients).ToString() == item)
                            {
                                tl_MappedClients.SetNodeCheckState(node, CheckState.Checked,true);
                            }
                        }
                    }
                }
            }
            catch (Exception autoEx)
            {
                eveErrorlog("Auto search " + autoEx.ToString());
            }
        }

        private void ccbe_Clients_Click(object sender, EventArgs e)
        {
            //ccbe_Clients.ShowPopup();
        }

        private void ccbe_MappedClients_Click(object sender, EventArgs e)
        {
            //ccbe_MappedClients.ShowPopup();
        }

        private void ccbe_Username_Click(object sender, EventArgs e)
        {
            ccbe_Username.ShowPopup();
        }

        private void btn_ClearMapping_Click(object sender, EventArgs e)
        {
            try
            {
                if (mySqlConnection.State == ConnectionState.Closed) mySqlConnection.Open();
                MySqlCommand cmd = new MySqlCommand("Update login SET MappedClient = ''", mySqlConnection);
                int result = cmd.ExecuteNonQuery();
                SelectMappedClient();
                //added by akshay on 11-11-2020 for NewMappedClients
                foreach (TreeListNode node in tl_MappedClients.GetNodeList())
                {
                    tl_MappedClients.SetNodeCheckState(node, CheckState.Unchecked, true);
                }
                XtraMessageBox.Show("Mapping cleared successfully.");
                eveErrorlog("Mapping cleared successfully.");
            }
            catch (Exception clear)
            {
                eveErrorlog("Clear mapping " + clear.ToString());
            }
        }

        private void pnlEditMappedUsers_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnlResetPassword_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btn_ClientsUpdate_Click(object sender, EventArgs e)
        {
            pnlEditMappedUsers.Visible = false;
            pnlResetPassword.Visible = false;
            pnlAddUser.Visible = false;
            pnlSelectClients.Visible = true;
            tl_SelectClients.Visible = true;
            btn_okSelectClients.Visible = true;
            tl_MappedClients.Visible = false;
            btn_okMappedClients.Visible = false;
        }

        private void btnMaapedClients_Click(object sender, EventArgs e)
        {
            pnlEditMappedUsers.Visible = false;
            pnlResetPassword.Visible = false;
            pnlAddUser.Visible = false;
            pnlSelectClients.Visible = true;
            tl_MappedClients.Visible = true;
            btn_okMappedClients.Visible = true;
            tl_SelectClients.Visible = false;
            btn_okSelectClients.Visible = false;
        }

        private void btn_BackMapped_Click(object sender, EventArgs e)
        {
            pnlEditMappedUsers.Visible = true;
            pnlResetPassword.Visible = false;
            pnlAddUser.Visible = false;
            pnlSelectClients.Visible = false;
        }

        private void btn_okSelectClients_Click(object sender, EventArgs e)
        {
            pnlEditMappedUsers.Visible = false;
            pnlResetPassword.Visible = false;
            pnlAddUser.Visible = true;
            pnlSelectClients.Visible = false;
        }

        private void btn_okMappedClients_Click(object sender, EventArgs e)
        {
            pnlEditMappedUsers.Visible = true;
            pnlResetPassword.Visible = false;
            pnlAddUser.Visible = false;
            pnlSelectClients.Visible = false;
        }

        private void pnlAddUser_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tl_SelectClients_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {

        }

        private void tl_MappedClients_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {

        }
    }
}