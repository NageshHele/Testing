using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using MySql.Data.MySqlClient;

namespace Engine
{
    public delegate void Errorlogging(string strMessage);

    public delegate void del_RefreshClientDetail();

    public partial class AddUser : XtraForm
    {
        internal event Errorlogging eve_Errorlog;
        internal event del_RefreshClientDetail eve_RefreshClientDetail;

        static Dictionary<string, string> dict_MappedClient = new Dictionary<string, string>();//23-12-2019
        //static ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, SortedSet<string>>>>> dict_ClientData = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, SortedSet<string>>>>>();

        static SortedDictionary<string, SortedDictionary<string, SortedDictionary<string, SortedDictionary<string, SortedSet<string>>>>> dict_ClientData = new SortedDictionary<string, SortedDictionary<string, SortedDictionary<string, SortedDictionary<string, SortedSet<string>>>>>();


        string _MySQLCon = string.Empty;

        public AddUser(string _MySQLCon)
        {
            InitializeComponent();

            this._MySQLCon = _MySQLCon;

            SelectClient();
            SelectUser();
        }

        void SelectClient()
        {
            try
            {
                dict_ClientData.Clear();
                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                {
                    con_MySQL.Open();
                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetClientDetail", con_MySQL))
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;

                        //added on 27APR2021 by Amey
                        myCmd.Parameters.Add("prm_Type", MySqlDbType.LongText);
                        myCmd.Parameters["prm_Type"].Value = "ALL";

                        using (MySqlDataReader mySqlDataReader = myCmd.ExecuteReader())
                        {
                            while (mySqlDataReader.Read())
                            {
                                //commented on 05-11-2020 by Akshay for NewMappedClientsTreelist
                                //ccbe_MappedClients.Properties.Items.Add(mySqlDataReader.GetString(0));
                                //ccbe_Clients.Properties.Items.Add(mySqlDataReader.GetString(0));

                                //chanegd Indexes on 27APR2021 by Amey
                                //Added on 05-11-2020 by Akshay for newMappedClients treelist
                                string Zone = mySqlDataReader.GetString(7);
                                string Branch = mySqlDataReader.GetString(8);
                                string Family = mySqlDataReader.GetString(9);
                                string Product = mySqlDataReader.GetString(10);
                                string Username = mySqlDataReader.GetString(3);

                                //changed to SortedSet on 30APR2021 by Amey

                                if (!dict_ClientData.ContainsKey(Zone))
                                    dict_ClientData.Add(Zone, new SortedDictionary<string, SortedDictionary<string, SortedDictionary<string, SortedSet<string>>>>());

                                if (!dict_ClientData[Zone].ContainsKey(Branch))
                                    dict_ClientData[Zone].Add(Branch, new SortedDictionary<string, SortedDictionary<string, SortedSet<string>>>());

                                if (!dict_ClientData[Zone][Branch].ContainsKey(Family))
                                    dict_ClientData[Zone][Branch].Add(Family, new SortedDictionary<string, SortedSet<string>>());

                                if (!dict_ClientData[Zone][Branch][Family].ContainsKey(Product))
                                    dict_ClientData[Zone][Branch][Family].Add(Product, new SortedSet<string>());

                                dict_ClientData[Zone][Branch][Family][Product].Add(Username);
                            }
                        }
                    }
                }

                //AddUserMappedClients();
                //EditUserMappedClients();

            }
            catch (Exception error)
            {
                eve_Errorlog("Bind client " + error.ToString());
            }
        }

        private void AddUserMappedClients()
        {
            try
            {
                tl_SelectClients.ClearNodes();
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
                                    foreach (var dealer in productitem.Value.OrderBy(v => v).ToList())
                                    {
                                        TreeListNode dealernode = tl_SelectClients.AppendNode(null, productnode);
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
                eve_Errorlog("AddUserMappedClients " + error.ToString());
            }

        }

        private void EditUserMappedClients()
        {
            try
            {
                tl_MappedClients.ClearNodes();
                //Added on 05-11-2020 by Akshay for newMappedClients treelist in EditUser
                tl_MappedClients.BeginUpdate();
                tl_MappedClients.BeginUnboundLoad();

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
                                    foreach (var dealer in productitem.Value.OrderBy(v => v).ToList())
                                    {
                                        TreeListNode dealernode = tl_MappedClients.AppendNode(null, productnode);
                                        dealernode.SetValue("name", dealer);
                                    }
                                }
                            }
                        }
                    }
                }

                //tl_MappedClients.EndUnboundLoad();
                //tl_MappedClients.EndUpdate();

                //tl_MappedClients.BeginUpdate();
                //tl_MappedClients.BeginUnboundLoad();
                SelectUser();
                //foreach (TreeListNode node in tl_MappedClients.GetNodeList())
                //{
                //    tl_MappedClients.SetNodeCheckState(node, CheckState.Unchecked, true);
                //}
                string _mappedClient = string.Empty;
                if (dict_MappedClient.TryGetValue(ccbe_Username.SelectedItem.ToString(), out _mappedClient))
                {
                    string[] _IndividuaClient = _mappedClient.Split(',');

                    foreach (TreeListNode node in tl_MappedClients.GetNodeList())
                    {

                        if (_IndividuaClient.Contains(node.GetValue(tlc_MappedClients).ToString()))
                        {
                            tl_MappedClients.SetNodeCheckState(node, CheckState.Checked, true);
                        }
                    }

                    //foreach (var item in _IndividuaClient)
                    //{
                    //    //Commented by akshay on 10-11-2020 for new MappedClients
                    //    //if (ccbe_Clients.Properties.Items.Contains(item))
                    //    //    ccbe_Clients.Properties.Items.FirstOrDefault(x => x.Value.ToString() == item).CheckState = CheckState.Checked;

                    //    //Added by akshay on 10-11-2020 for new MappedClients
                    //    foreach (TreeListNode node in tl_MappedClients.GetNodeList())
                    //    {
                    //        if (node.GetValue(tlc_MappedClients).ToString() == item)
                    //        {
                    //            tl_MappedClients.SetNodeCheckState(node, CheckState.Checked, true);
                    //        }
                    //    }
                    //}
                }

                tl_MappedClients.EndUnboundLoad();
                tl_MappedClients.EndUpdate();
            }
            catch (Exception error)
            {
                eve_Errorlog("EditUserMappedClients " + error.ToString());
            }
        }

        //private void EditUserMappedClients()
        //{
        //    try
        //    {
        //        tl_MappedClients.ClearNodes();
        //        //Added on 05-11-2020 by Akshay for newMappedClients treelist in EditUser
        //        if (dict_ClientData.Count != 0)     //Added by Akshay For Avoiding error when Clientdata is Empty
        //        {
        //            TreeListNode SelectMappednode = tl_MappedClients.AppendNode(null, null);
        //            SelectMappednode.SetValue("name", "Select All");
        //            foreach (var zoneitem in dict_ClientData)
        //            {
        //                TreeListNode zonenode = tl_MappedClients.AppendNode(null, SelectMappednode);
        //                zonenode.SetValue("name", zoneitem.Key);
        //                foreach (var branchitem in zoneitem.Value)
        //                {
        //                    TreeListNode branchnode = tl_MappedClients.AppendNode(null, zonenode);
        //                    branchnode.SetValue("name", branchitem.Key);
        //                    foreach (var familyitem in branchitem.Value)
        //                    {
        //                        TreeListNode familynode = tl_MappedClients.AppendNode(null, branchnode);
        //                        familynode.SetValue("name", familyitem.Key);
        //                        foreach (var productitem in familyitem.Value)
        //                        {
        //                            TreeListNode productnode = tl_MappedClients.AppendNode(null, familynode);
        //                            productnode.SetValue("name", productitem.Key);
        //                            foreach (var dealer in productitem.Value)
        //                            {
        //                                TreeListNode dealernode = tl_MappedClients.AppendNode(null, productnode);
        //                                dealernode.SetValue("name", dealer);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        SelectUser();
        //        foreach (TreeListNode node in tl_MappedClients.GetNodeList())
        //        {
        //            tl_MappedClients.SetNodeCheckState(node, CheckState.Unchecked, true);
        //        }
        //        string _mappedClient = string.Empty;
        //        if (dict_MappedClient.TryGetValue(ccbe_Username.SelectedItem.ToString(), out _mappedClient))
        //        {
        //            string[] _IndividuaClient = _mappedClient.Split(',');
        //            foreach (var item in _IndividuaClient)
        //            {
        //                //Commented by akshay on 10-11-2020 for new MappedClients
        //                //if (ccbe_Clients.Properties.Items.Contains(item))
        //                //    ccbe_Clients.Properties.Items.FirstOrDefault(x => x.Value.ToString() == item).CheckState = CheckState.Checked;

        //                //Added by akshay on 10-11-2020 for new MappedClients
        //                foreach (TreeListNode node in tl_MappedClients.GetNodeList())
        //                {
        //                    if (node.GetValue(tlc_MappedClients).ToString() == item)
        //                    {
        //                        tl_MappedClients.SetNodeCheckState(node, CheckState.Checked, true);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception error)
        //    {
        //        eve_Errorlog("EditUserMappedClients " + error.ToString());
        //    }
        //}

        private void SelectUser()
        {
            try
            {
                cmb_RUsername.Properties.Items.Clear();    //Added on 12-11-2020 by Akshay For Clearing Existing Items
                ccbe_Username.Properties.Items.Clear();    //Added on 12-11-2020 by Akshay For Clearing Existing Items

                //added on 27APR2021 by Amey. Removed hardcoded MySQL query.
                DataTable dt_UserInfo = new DataTable();
                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                {
                    con_MySQL.Open();
                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetUserInfo", con_MySQL))//modified by Navin on 12-06-2019
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;

                        MySqlDataAdapter dadapt = new MySqlDataAdapter(myCmd);
                        dadapt.Fill(dt_UserInfo);
                    }
                }
                clsEncryptionDecryption.DecryptData(dt_UserInfo);

                foreach (DataRow dRow in dt_UserInfo.Rows)
                {
                    cmb_RUsername.Properties.Items.Add(dRow[0].ToString());
                    ccbe_Username.Properties.Items.Add(dRow[0].ToString());
                    dict_MappedClient[dRow[0].ToString()] = dRow[1].ToString();
                }
            }
            catch (Exception error)
            {
                eve_Errorlog("Bind client " + error.ToString());
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
                        if (ckeIsAdmin.Checked)
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
                        eve_Errorlog("Mapped client " + mappedEx.ToString());
                    }

                    int result = 0;
                    using (var con_MySQL = new MySqlConnection(_MySQLCon))
                    {
                        con_MySQL.Open();
                        using (var cmd = new MySqlCommand("INSERT INTO `tbl_login` (`UserName`, `Password`, `IsAdmin`, MappedClient) VALUES ( '" + clsEncryptionDecryption.EncryptString(txteUsername.Text.Trim().ToLower(), "Nerve123") + "','" + clsEncryptionDecryption.EncryptString(txtePassword.Text.Trim(), "Nerve123") + "','" + clsEncryptionDecryption.EncryptString(ckeIsAdmin.Checked.ToString(), "Nerve123") + "','" + clsEncryptionDecryption.EncryptString(sbMappedClient.ToString(), "Nerve123") + "')", con_MySQL))
                        {
                            result = cmd.ExecuteNonQuery();
                        }
                    }
                    if (result == 1)
                    {
                        cmb_RUsername.Properties.Items.Add(txteUsername.Text.Trim().ToLower());//02-01-2020
                        ccbe_Username.Properties.Items.Add(txteUsername.Text.Trim().ToLower());//02-01-2020
                        XtraMessageBox.Show("User added successfully");
                        eve_Errorlog("User " + txteUsername.Text.Trim().ToLower() + " Added successfully. Mapped client-" + sbMappedClient.ToString());//12-12-2019
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
                eve_Errorlog("Add user " + addEx.ToString());
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
                        using (var con_MySQL = new MySqlConnection(_MySQLCon))
                        {
                            con_MySQL.Open();
                            using (var cmd = new MySqlCommand("UPDATE tbl_login SET Password = '" + clsEncryptionDecryption.EncryptString("Prime123", "Nerve123") + "' where UserName= '" + clsEncryptionDecryption.EncryptString(cmb_RUsername.SelectedItem.ToString().ToLower(), "Nerve123") + "'", con_MySQL))
                            {
                                int result = cmd.ExecuteNonQuery();
                                if (result == 1)
                                {
                                    eve_Errorlog("Password resetted for user " + cmb_RUsername.SelectedItem.ToString() + " default password is Prime123");
                                    XtraMessageBox.Show("Password reset successful");
                                }
                                else
                                    XtraMessageBox.Show("User does not exist");
                            }
                        }
                    }
                    else
                    {
                        using (var con_MySQL = new MySqlConnection(_MySQLCon))
                        {
                            con_MySQL.Open();
                            using (var cmd = new MySqlCommand("DELETE FROM tbl_login WHERE UserName= '" + clsEncryptionDecryption.EncryptString(cmb_RUsername.SelectedItem.ToString().Trim().ToLower(), "Nerve123") + "'", con_MySQL))
                            {
                                int result = cmd.ExecuteNonQuery();
                                if (result == 1)
                                {
                                    cmb_RUsername.Properties.Items.Remove(cmb_RUsername.SelectedItem.ToString());

                                    eve_Errorlog("User- " + cmb_RUsername.SelectedItem.ToString() + " deleted");
                                    XtraMessageBox.Show("User deleted successfully");
                                }
                                else
                                    XtraMessageBox.Show("User does not exist");
                            }
                        }
                    }
                    SelectUser();
                }
                else
                    XtraMessageBox.Show("Please enter username");
            }
            catch (Exception ResetEx)
            {
                eve_Errorlog("Reset Password " + ResetEx.ToString());
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
                    eve_Errorlog("Mapped client " + mappedEx.ToString());
                }

                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                {
                    con_MySQL.Open();
                    using (var cmd = new MySqlCommand("UPDATE tbl_login SET MappedClient = '" + clsEncryptionDecryption.EncryptString(sbMappedClient.ToString(), "Nerve123") + "' where UserName= '" + clsEncryptionDecryption.EncryptString(ccbe_Username.SelectedItem.ToString().ToLower(), "Nerve123") + "'", con_MySQL))
                    {
                        int result = cmd.ExecuteNonQuery();
                        if (result == 1)
                        {
                            XtraMessageBox.Show("Mapped client updated successfully");
                            eve_Errorlog("Mapping updated for user " + ccbe_Username.SelectedItem.ToString().ToLower() + ", Mapped client-" + sbMappedClient.ToString());//12-12-2019
                        }
                        else
                            XtraMessageBox.Show("User does not exist");
                    }
                }
                //mySqlConnection.Close(); //added by akshay on 10-11-2020 for NewMappedClients
            }
            catch (Exception updateEx)
            {
                eve_Errorlog("Updating user mapping " + updateEx.ToString());
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
                int _rowsAffected = 0, _totalRecordsPresent = 0;//added by Navin on 20-01-2020
                if (_openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string uploadPath = _openFileDialog.FileName;
                    eve_Errorlog("File selected for mapping " + uploadPath);
                    _dtStartTime = DateTime.Now;
                    using (FileStream stream = File.Open(uploadPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string _line1;
                            if (strType == "Complete")
                            {
                                eve_Errorlog("Started complete user client mapping");
                                try
                                {
                                    for (int iuser = 0; iuser < dict_MappedClient.Count; iuser++)
                                    {
                                        dict_MappedClient[dict_MappedClient.ElementAt(iuser).Key] = string.Empty;
                                    }
                                }
                                catch (Exception error)
                                {
                                    eve_Errorlog("Client mapping clear " + error.ToString());
                                }
                            }
                            else
                                eve_Errorlog("Started partial user client mapping");

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
                                            eve_Errorlog("User `" + fields[0].Trim().ToLower() + "` does not exists, Skipped record- " + _line1);
                                        }
                                    }
                                    else
                                        eve_Errorlog("Data not in proper format- " + _line1);
                                }
                                catch (Exception)
                                {
                                    eve_Errorlog("Data not in proper format- " + _line1);
                                }
                            }
                        }
                    }
                }
                foreach (KeyValuePair<string, StringBuilder> item in dict_Usermapping)
                {
                    try
                    {
                        using (var con_MySQL = new MySqlConnection(_MySQLCon))
                        {
                            con_MySQL.Open();
                            using (var cmd = new MySqlCommand("UPDATE tbl_login SET MappedClient = '" + clsEncryptionDecryption.EncryptString((dict_MappedClient[item.Key] == string.Empty ? item.Value.ToString() : (item.Value.ToString() == string.Empty ? dict_MappedClient[item.Key] : item.Value + "," + dict_MappedClient[item.Key])), "Nerve123") + "' where UserName= '" + clsEncryptionDecryption.EncryptString(item.Key, "Nerve123") + "'", con_MySQL))
                            {
                                int result = cmd.ExecuteNonQuery();
                                if (result == 1)
                                {
                                    //XtraMessageBox.Show("Mapped client updated successfully");
                                    eve_Errorlog("Mapping updated for user `" + item.Key + "`, Mapped client-" + (dict_MappedClient[item.Key] == string.Empty ? item.Value.ToString() : (item.Value.ToString() == string.Empty ? dict_MappedClient[item.Key] : item.Value + "," + dict_MappedClient[item.Key])));//12-12-2019
                                }
                                else
                                    eve_Errorlog("User `" + item.Key + "` does not exist");
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        eve_Errorlog("Exception occurred while updating user mapping for `" + item.Key + "`, " + error.ToString());
                    }
                }
                SelectMappedClient();
                XtraMessageBox.Show("Client mapping completed");
                eve_Errorlog("Client mapping completed, " + _totalRecordsPresent + " records present in file, " + _rowsAffected + " records affected in database.");//modified error message on 02-01-2020
                eve_Errorlog("Time taken to upload client mapping " + (DateTime.Now - _dtStartTime));
            }
            catch (Exception pathEx)
            {
                eve_Errorlog("UpdateClientMapping " + pathEx.ToString());
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
                DataTable dtClient = new DataTable();

                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                {
                    con_MySQL.Open();
                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetUserInfo", con_MySQL))//modified by Navin on 12-06-2019
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;

                        MySqlDataAdapter dadapt = new MySqlDataAdapter(myCmd);
                        dadapt.Fill(dtClient);
                    }
                }

                clsEncryptionDecryption.DecryptData(dtClient);

                dict_MappedClient.Clear();
                for (int iCl = 0; iCl < dtClient.Rows.Count; iCl++)
                    dict_MappedClient[dtClient.Rows[iCl][0].ToString().Trim()] = dtClient.Rows[iCl][1].ToString().Trim();
            }
            catch (Exception str)
            {
                eve_Errorlog("Select mapped client " + str.ToString());
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
                    
                    foreach (TreeListNode node in tl_MappedClients.GetNodeList())
                    {
                        var jvhjv = node.GetValue(tlc_MappedClients).ToString();

                        if (_IndividuaClient.Contains(node.GetValue(tlc_MappedClients).ToString()))
                        {
                            tl_MappedClients.SetNodeCheckState(node, CheckState.Checked, true);
                        }
                    }
                }
            }
            catch (Exception autoEx)
            {
                eve_Errorlog("Auto search " + autoEx.ToString());
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
                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                {
                    con_MySQL.Open();
                    using (var cmd = new MySqlCommand("UPDATE tbl_login SET MappedClient = ''", con_MySQL))
                    {
                        int result = cmd.ExecuteNonQuery();
                    }
                }

                SelectMappedClient();
                //added by akshay on 11-11-2020 for NewMappedClients
                foreach (TreeListNode node in tl_MappedClients.GetNodeList())
                {
                    tl_MappedClients.SetNodeCheckState(node, CheckState.Unchecked, true);
                }
                XtraMessageBox.Show("Mapping cleared successfully.");
                eve_Errorlog("Mapping cleared successfully.");
            }
            catch (Exception clear)
            {
                eve_Errorlog("Clear mapping " + clear.ToString());
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
            AddUserMappedClients();
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
            EditUserMappedClients();
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

        private void AddUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            //added on 17FEB2021 by Amey
            eve_RefreshClientDetail();
        }

        private void btn_FileMapping_Click(object sender, EventArgs e)
        {
            try
            {
                var UserFile = new OpenFileDialog();
                if (UserFile.ShowDialog() == DialogResult.OK) { AddUserandClientMapping(UserFile.FileName); }

            }
            catch (Exception ee) { eve_Errorlog("File Mapping " + ee.ToString()); }
        }

        private void AddUserandClientMapping(string UserFile)
        {
            try
            {

                DataTable dt_UserInfo = new DataTable();
                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                {
                    con_MySQL.Open();
                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetUserInfo", con_MySQL))//modified by Navin on 12-06-2019
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;

                        MySqlDataAdapter dadapt = new MySqlDataAdapter(myCmd);
                        dadapt.Fill(dt_UserInfo);
                    }
                }
                clsEncryptionDecryption.DecryptData(dt_UserInfo);


                var arr_Info = File.ReadAllLines(UserFile);


                foreach (var Info in arr_Info)
                {

                    if (Info != "")
                    {
                        var list_data = Info.Split(',').ToList();
                        var username = list_data[0].Trim().ToLower();
                        var command = list_data[1].Trim().ToLower();

                        if (username == "")
                        {
                            XtraMessageBox.Show("Invalid data in client mapping file", "Error");

                            return;
                        }

                        List<string> list_MappedClients = new List<string>();

                        DataColumn[] columns = dt_UserInfo.Columns.Cast<DataColumn>().ToArray();
                        bool userpresent = dt_UserInfo.AsEnumerable().Any(row => columns.Any(col => row[col].ToString() == username));

                        if (userpresent)
                        {
                            DataRow user_row = dt_UserInfo.AsEnumerable().Where(row => row.Field<string>("UserName") == username).First();

                            if (command == "partial")
                            {
                                list_MappedClients = user_row[1].ToString().Split(',').ToList();
                                for (int i = 2; i < list_data.Count; i++)
                                {
                                    if (!list_MappedClients.Contains(list_data[i]))
                                        list_MappedClients.Add(list_data[i]);
                                }

                            }
                            else if (command == "delete")
                            {

                                list_MappedClients = user_row[1].ToString().Split(',').ToList();
                                for (int i = 2; i < list_data.Count; i++)
                                {
                                    if (list_MappedClients.Contains(list_data[i]))
                                        list_MappedClients.Remove(list_data[i]);
                                }

                            }
                            else if (command == "full")
                            {
                                for (int i = 2; i < list_data.Count; i++)
                                {
                                    list_MappedClients.Add(list_data[i]);
                                }
                            }

                            else if (command == "all")
                            {
                                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                                {
                                    con_MySQL.Open();
                                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetClientDetail", con_MySQL))
                                    {
                                        myCmd.CommandType = CommandType.StoredProcedure;

                                        myCmd.Parameters.Add("prm_Type", MySqlDbType.LongText);
                                        myCmd.Parameters["prm_Type"].Value = "ALL";

                                        using (MySqlDataReader mySqlDataReader = myCmd.ExecuteReader())
                                        {
                                            while (mySqlDataReader.Read())
                                            {
                                                list_MappedClients.Add(mySqlDataReader.GetString(3));

                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                XtraMessageBox.Show("Invalid data in client mapping file", "Error");

                                return;
                            }


                            StringBuilder sbMappedClient = new StringBuilder();
                            if (list_MappedClients.Count > 0 || command == "delete")
                            {
                                for (int i = 0; i < list_MappedClients.Count; i++)
                                {
                                    sbMappedClient.Append(list_MappedClients[i] + ",");
                                }
                                if (sbMappedClient.Length > 1)
                                    sbMappedClient.Remove(sbMappedClient.ToString().LastIndexOf(','), 1);

                                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                                {
                                    con_MySQL.Open();
                                    using (var cmd = new MySqlCommand("UPDATE tbl_login SET MappedClient = '" + clsEncryptionDecryption.EncryptString(sbMappedClient.ToString(), "Nerve123") + "' where UserName= '" + clsEncryptionDecryption.EncryptString(username.ToLower(), "Nerve123") + "'", con_MySQL))
                                    {
                                        int result = cmd.ExecuteNonQuery();
                                        if (result != 1)
                                        {
                                            XtraMessageBox.Show("Client Mapping failed for User: " + username, "Error");

                                            return;
                                        }

                                    }
                                }
                            }



                        }
                        else
                        {
                            if (command == "all")
                            {
                                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                                {
                                    con_MySQL.Open();
                                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetClientDetail", con_MySQL))
                                    {
                                        myCmd.CommandType = CommandType.StoredProcedure;

                                        myCmd.Parameters.Add("prm_Type", MySqlDbType.LongText);
                                        myCmd.Parameters["prm_Type"].Value = "ALL";

                                        using (MySqlDataReader mySqlDataReader = myCmd.ExecuteReader())
                                        {
                                            while (mySqlDataReader.Read())
                                            {
                                                list_MappedClients.Add(mySqlDataReader.GetString(3));

                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {
                                for (int i = 2; i < list_data.Count; i++)
                                {
                                    list_MappedClients.Add(list_data[i]);
                                }

                            }


                            StringBuilder sbMappedClient = new StringBuilder();
                            if (list_MappedClients.Count > 0)
                            {

                                for (int i = 0; i < list_MappedClients.Count; i++)
                                {
                                    sbMappedClient.Append(list_MappedClients[i] + ",");
                                }
                                if (sbMappedClient.Length > 1)
                                    sbMappedClient.Remove(sbMappedClient.ToString().LastIndexOf(','), 1);

                                int result = 0;
                                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                                {
                                    con_MySQL.Open();
                                    using (var cmd = new MySqlCommand("INSERT INTO `tbl_login` (`UserName`, `Password`, `IsAdmin`, MappedClient) VALUES ( '" + clsEncryptionDecryption.EncryptString(username.ToLower(), "Nerve123") + "','" + clsEncryptionDecryption.EncryptString("prime123", "Nerve123") + "','" + clsEncryptionDecryption.EncryptString("false", "Nerve123") + "','" + clsEncryptionDecryption.EncryptString(sbMappedClient.ToString(), "Nerve123") + "')", con_MySQL))
                                    {
                                        result = cmd.ExecuteNonQuery();
                                        if (result != 1)
                                        {
                                            XtraMessageBox.Show("Client Mapping failed for User: " + username, "Error");

                                            return;
                                        }
                                    }
                                }
                            }


                        }

                    }


                }
                XtraMessageBox.Show("Mapped client updated successfully");

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

                        foreach (TreeListNode node in tl_MappedClients.GetNodeList())
                        {
                            if (node.GetValue(tlc_MappedClients).ToString() == item)
                            {
                                tl_MappedClients.SetNodeCheckState(node, CheckState.Checked, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                eve_Errorlog("AddUserandClientMapping: " + ee.ToString());

            }
        }
    }
}