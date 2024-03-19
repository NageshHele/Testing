using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using NerveLog;
using Prime.Helper;
using n.Structs;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Concurrent;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors.Popup;
using DevExpress.Utils.Win;

namespace Prime.UI
{
    // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder

    public delegate void del_RuleManagerFormClosed();
   
    public partial class form_RuleBuilder : XtraForm
    {
        public event del_RuleManagerFormClosed eve_RuleManagerFormClosed;

        bool IsEditWindow = false;

        NerveLogger _logger;
        
        List<string> list_RuleName = new List<string>();
        List<string> list_ColumnList = new List<string>();
        List<string> list_ClientList = new List<string>();
        ConcurrentDictionary<string, ClientInfo> dict_ClientInfo = new ConcurrentDictionary<string, ClientInfo>();
        static ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, SortedSet<string>>>>> dict_ClientData = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, SortedSet<string>>>>>();
        

        string rulefileloc = Application.StartupPath + "\\Report\\Rule.json";
        int detail_level;

        public form_RuleBuilder()
        {
            InitializeComponent();
            this._logger = CollectionHelper._logger;
            
        }

        private void RuleForm_Load(object sender, EventArgs e)
        {
            try
            {
                list_ColumnList.Sort();
                cb_ColumnNames.Properties.Items.AddRange(list_ColumnList);

                cb_RuleNames.Visible = false;
                lbl_RuleNameMainControl.Visible = false;
                pc_RuleEditor.Visible = false;
                btn_AddRule.Visible = false;
                btn_EditRule.Visible = false;
                //btn_DeleteRule.Visible = false;

                lbl_ScripName.Visible = false;
                txt_ScripName.Visible = false;
                chkCB_DeleteRuleList.Visible = false;
                pnl_ClientList.Visible = false;

                foreach(var Client in dict_ClientInfo.Keys)
                {
                    string Zone = dict_ClientInfo[Client].Zone;
                    string Branch = dict_ClientInfo[Client].Branch;
                    string Family = dict_ClientInfo[Client].Family;
                    string Product = dict_ClientInfo[Client].Product;
                    string Username = Client;

                    //changed to SortedSet on 30APR2021 by Amey

                    if (!dict_ClientData.ContainsKey(Zone))
                        dict_ClientData.TryAdd(Zone, new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, SortedSet<string>>>>());

                    if (!dict_ClientData[Zone].ContainsKey(Branch))
                        dict_ClientData[Zone].TryAdd(Branch, new ConcurrentDictionary<string, ConcurrentDictionary<string, SortedSet<string>>>());

                    if (!dict_ClientData[Zone][Branch].ContainsKey(Family))
                        dict_ClientData[Zone][Branch].TryAdd(Family, new ConcurrentDictionary<string, SortedSet<string>>());

                    if (!dict_ClientData[Zone][Branch][Family].ContainsKey(Product))
                        dict_ClientData[Zone][Branch][Family].TryAdd(Product, new SortedSet<string>());

                    dict_ClientData[Zone][Branch][Family][Product].Add(Username);
                }

                
            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        private void btn_AddGroup_Click(object sender, EventArgs e)
        {
            try
            {
                if (groupBox1.Visible == true && groupBox2.Visible == false)
                {
                    groupBox2.Visible = true;
                    grpBox_Group2Rule1.Visible = true;
                    cmb_Group2.Visible = false;
                    grpBox_Group2Rule2.Visible = false;
                    cmb_Group2Rule1.Text = "<";
                }

                else if (groupBox2.Visible == true && groupBox3.Visible == false)
                {
                    groupBox3.Visible = true;
                    grpBox_Group3Rule1.Visible = true;
                    cmb_Group3.Visible = false;
                    grpBox_Group3Rule2.Visible = false;
                    cmb_Group3Rule1.Text = "<";
                }
            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        private void btn_AddRuleinGroup1_Click(object sender, EventArgs e)
        {
            try
            {
                cmb_Group1.Visible = true;
                cmb_Group1.Text = "AND";
                cmb_Group1Rule2.Text = "<";
                grpBox_Group1Rule2.Visible = true;
            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        private void btn_AddRuleinGroup2_Click(object sender, EventArgs e)
        {
            try
            {
                cmb_Group2.Visible = true;
                cmb_Group2.Text = "AND";
                cmb_Group2Rule2.Text = "<";
                grpBox_Group2Rule2.Visible = true;
            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        private void btn_AddRuleinGroup3_Click(object sender, EventArgs e)
        {
            try
            {
                cmb_Group3.Visible = true;
                cmb_Group3.Text = "AND";
                cmb_Group3Rule2.Text = "<";
                grpBox_Group3Rule2.Visible = true;
            }
            catch (Exception ee) { _logger.Error(ee); }


        }

        private void btn_RemoveRuleGrp1_Click(object sender, EventArgs e)
        {
            try
            {
                if (grpBox_Group1Rule2.Visible == true)
                {
                    txt_Group1Rule2.Text = "";
                    cmb_Group1.Text = "OR";
                    cmb_Group1.Visible = false;
                    grpBox_Group1Rule2.Visible = false;

                }
                else
                {
                    txt_Group1Rule1.Text = "";
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void btn_RemoveRuleGrp2_Click(object sender, EventArgs e)
        {
            try
            {
                if (grpBox_Group2Rule2.Visible == true)
                {
                    txt_Group2Rule2.Text = "";
                    cmb_Group2.Text = "OR";
                    cmb_Group2.Visible = false;
                    grpBox_Group2Rule2.Visible = false;

                }
                else if (groupBox3.Visible == false)
                {
                    txt_Group2Rule1.Text = "";
                    grpBox_Group2Rule1.Visible = false;
                    groupBox2.Visible = false;
                    txt_Group2AlertText.Text = "";
                }
            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        private void btn_RemoveRuleGrp3_Click(object sender, EventArgs e)
        {
            try
            {
                if (grpBox_Group3Rule2.Visible == true)
                {
                    txt_Group3Rule2.Text = "";
                    cmb_Group3.Text = "OR";
                    cmb_Group3.Visible = false;
                    grpBox_Group3Rule2.Visible = false;

                }
                else
                {
                    txt_Group3Rule1.Text = "";
                    grpBox_Group3Rule1.Visible = false;
                    groupBox3.Visible = false;
                    txt_Group3AlertText.Text = "";
                }
            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        private void txt_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                TextEdit textBox = (TextEdit)sender;
                if (textBox.Visible == true && textBox.Text.Trim() == "") { e.Cancel = true; textBox.Focus(); errorProvider.SetError(textBox, "Cannot be empty"); } else { errorProvider.SetError(textBox, null); }

                try
                {
                    if (textBox.Visible)
                    {
                        double.Parse(textBox.Text.Trim());
                        errorProvider.SetError(textBox, null);
                    }

                }
                catch (Exception ee) { e.Cancel = true; textBox.Focus(); errorProvider.SetError(textBox, "Invalid Input"); }

            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void chkcb_ClientList_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                CheckedComboBoxEdit checkedComboBoxEdit = (CheckedComboBoxEdit)sender;
                if (checkedComboBoxEdit.Visible == true && checkedComboBoxEdit.Text.Trim() == "") { e.Cancel = true; checkedComboBoxEdit.Focus(); errorProvider.SetError(checkedComboBoxEdit, "Cannot be empty"); } else { errorProvider.SetError(checkedComboBoxEdit, null); }


            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void txt_RuleName_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                TextEdit textBox = (TextEdit)sender;
                if (textBox.Visible == true && textBox.Text.Trim() == "") { e.Cancel = true; textBox.Focus(); errorProvider.SetError(textBox, "Cannot be empty"); } else { errorProvider.SetError(textBox, null); }

                if (CollectionHelper.dict_RuleInfo.ContainsKey(textBox.Text.Trim()) && !IsEditWindow)
                {
                    e.Cancel = true;
                    textBox.Focus();
                    errorProvider.SetError(textBox, "Rule Name already exists");

                }
                else { errorProvider.SetError(textBox, null); }

            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        private void txt_Keypress(object sender, KeyPressEventArgs e)
        {
            try
            {
                TextEdit txtbox = (TextEdit)sender;
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.' && e.KeyChar != '-') { e.Handled = true; }

                if (e.KeyChar == ('.') && txtbox.Text.Contains('.')) { e.Handled = true; }

                if (e.KeyChar == ('-') && txtbox.Text.Contains('-')) { e.Handled = true; }
            }
            catch (Exception ee) { _logger.Error(ee); }

        }


        private void btn_AddRule_Click(object sender, EventArgs e)
        {
            try
            {
                if (CollectionHelper.dict_RuleInfo.Count < 500)
                {

                    IsEditWindow = false;

                    AddClientInfo(new List<string>());

                    btn_AddRule.Enabled = false;
                    btn_EditRule.Enabled = false;
                    btn_DeleteRule.Enabled = false;
                    cb_ColumnNames.Enabled = false;
                    cb_RuleNames.Enabled = false;

                    txt_RuleName.Text = string.Empty;
                    //chkcb_ClientList.Properties.Items.Clear();
                    //chkcb_ClientList.Properties.Items.AddRange(list_ClientList.ToArray());
                    //chkcb_ClientList.Text = string.Empty;
                    //chkcb_ClientList.DeselectAll();
                    txt_Group1Rule1.Text = string.Empty;
                    txt_Group1Rule2.Text = string.Empty;
                    txt_Group1AlertText.Text = string.Empty;
                    txt_Group2Rule1.Text = string.Empty;
                    txt_Group2Rule2.Text = string.Empty;
                    txt_Group2AlertText.Text = string.Empty;
                    txt_Group3Rule1.Text = string.Empty;
                    txt_Group3Rule2.Text = string.Empty;
                    txt_Group3AlertText.Text = string.Empty;
                    pc_RuleEditor.Visible = true;



                    Label_1.Text = cb_ColumnNames.Text.ToString();
                    Label_2.Text = Label_1.Text;
                    Label_3.Text = Label_1.Text;
                    Label_4.Text = Label_1.Text;
                    Label_5.Text = Label_1.Text;
                    Label_6.Text = Label_1.Text;

                    if (detail_level != 0)
                    {
                        lbl_ScripName.Visible = true;
                        txt_ScripName.Visible = true;
                    }

                    cmb_Group1Rule1.Text = "<";
                    groupBox2.Visible = false;
                    groupBox3.Visible = false;
                    cmb_Group1.Visible = false;
                    grpBox_Group1Rule2.Visible = false;
                }
                else
                {
                    XtraMessageBox.Show("Only 500 Rules Allowed", "Error");
                }


            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void btn_EditRule_Click(object sender, EventArgs e)
        {
            try
            {
                IsEditWindow = true;

                btn_AddRule.Enabled = false;
                btn_EditRule.Enabled = false;
                btn_DeleteRule.Enabled = false;
                cb_ColumnNames.Enabled = false;
                cb_RuleNames.Enabled = false;

                string colName = cb_ColumnNames.Text.ToString();
                string ruleName = cb_RuleNames.Text.ToString();

                Label_1.Text = cb_ColumnNames.Text.ToString();
                Label_2.Text = Label_1.Text;
                Label_3.Text = Label_1.Text;
                Label_4.Text = Label_1.Text;
                Label_5.Text = Label_1.Text;
                Label_6.Text = Label_1.Text;

                RuleAlert ruleAlert = CollectionHelper.dict_RuleInfo[ruleName];

                var Group1 = ruleAlert.list_Groups[0];
                var Group2 = ruleAlert.list_Groups[1];
                var Group3 = ruleAlert.list_Groups[2];


                if (ruleAlert.detail_level > 0)
                {
                    lbl_ScripName.Visible = true;
                    txt_ScripName.Visible = true;
                    txt_ScripName.Text = string.Join(",", ruleAlert.arr_Scrips).ToUpper();
                }
                else
                {
                    lbl_ScripName.Visible = false;
                    txt_ScripName.Visible = false;

                }

                AddClientInfo(new List<string>(ruleAlert.list_Clients));

                //chkcb_ClientList.Properties.Items.Clear();
                //chkcb_ClientList.Properties.Items.AddRange(list_ClientList.ToArray());
                //chkcb_ClientList.Properties.EditValueType = DevExpress.XtraEditors.Repository.EditValueTypeCollection.List;
                //chkcb_ClientList.SetEditValue(ruleAlert.list_Clients);

                txt_RuleName.Text = ruleName;
                cmb_Group1Rule1.Text = Group1.Op1;
                txt_Group1Rule1.Text = Group1.Value1.ToString();
                txt_Group1AlertText.Text = Group1.AlertMessage;

                if (!double.IsNaN(Group1.Value2))
                {
                    grpBox_Group1Rule2.Visible = true;
                    cmb_Group1.Visible = true;
                    cmb_Group1.Text = Group1.LogicalOp;
                    cmb_Group1Rule2.Text = Group1.Op2;
                    txt_Group1Rule2.Text = Group1.Value2.ToString();

                }
                else
                {
                    grpBox_Group1Rule2.Visible = false;
                    cmb_Group1.Visible = false;
                }

                if (!double.IsNaN(Group2.Value1))
                {

                    grpBox_Group2Rule1.Visible = true;
                    cmb_Group2Rule1.Text = Group2.Op1;
                    txt_Group2Rule1.Text = Group2.Value1.ToString();
                    txt_Group2AlertText.Text = Group2.AlertMessage;
                    if (double.IsNaN(Group2.Value2)) { cmb_Group2.Visible = false; grpBox_Group2Rule2.Visible = false; }
                }
                else
                {
                    grpBox_Group2Rule1.Visible = false;
                    groupBox2.Visible = false;
                }

                if (!double.IsNaN(Group2.Value2))
                {
                    grpBox_Group2Rule2.Visible = true;
                    cmb_Group2.Visible = true;
                    cmb_Group2.Text = Group2.LogicalOp;
                    cmb_Group2Rule2.Text = Group2.Op2;
                    txt_Group2Rule2.Text = Group2.Value2.ToString();
                }
                else { grpBox_Group2Rule2.Visible = false; }

                if (!double.IsNaN(Group3.Value1))
                {
                    groupBox3.Visible = true;
                    grpBox_Group3Rule1.Visible = true;
                    cmb_Group3Rule1.Text = Group3.Op1;
                    txt_Group3Rule1.Text = Group3.Value1.ToString();
                    txt_Group3AlertText.Text = Group3.AlertMessage;
                    if (double.IsNaN(Group3.Value2)) { cmb_Group3.Visible = false; grpBox_Group3Rule2.Visible = false; }
                }
                else { groupBox3.Visible = false; }

                if (!double.IsNaN(Group3.Value2))
                {
                    grpBox_Group3Rule2.Visible = true;
                    cmb_Group3.Visible = true;
                    cmb_Group3.Text = Group3.LogicalOp;
                    cmb_Group3Rule2.Text = Group3.Op2;
                    txt_Group3Rule2.Text = Group3.Value2.ToString();
                }
                else { grpBox_Group3Rule2.Visible = false; }

                pc_RuleEditor.Visible = true;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void btn_DeleteRule_Click(object sender, EventArgs e)
        {
            try
            {

                if (cb_RuleNames.Visible == false)
                {

                    btn_DeleteRule.Enabled = false;
                    cb_ColumnNames.Enabled = false;
                    btn_AddRule.Enabled = false;
                    chkCB_DeleteRuleList.Properties.Items.Clear();
                    chkCB_DeleteRuleList.Properties.Items.AddRange(CollectionHelper.dict_RuleInfo.Keys.ToArray());
                    chkCB_DeleteRuleList.Visible = true;
                }
                else
                {
                    btn_AddRule.Enabled = false;
                    btn_EditRule.Enabled = false;
                    btn_DeleteRule.Enabled = false;
                    cb_RuleNames.Enabled = false;
                    cb_ColumnNames.Enabled = false;

                    string colName = cb_ColumnNames.Text.ToString();
                    string ruleName = cb_RuleNames.Text.ToString();

                    if (XtraMessageBox.Show("Do you really want to delete this rule?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        CollectionHelper.dict_RuleInfo.Remove(ruleName);

                        if (list_RuleName.Contains(ruleName))
                            list_RuleName.Remove(ruleName);

                        if (CollectionHelper.dict_RuleInfo.Count > 0)
                        {
                            string str_Rule = JsonConvert.SerializeObject(CollectionHelper.dict_RuleInfo);
                            File.WriteAllText(rulefileloc, str_Rule);
                        }
                        else
                        {
                            File.Delete(rulefileloc);
                        }

                        XtraMessageBox.Show($"Rule Deleted for {colName} column", "Information");


                        var dict_temp = CollectionHelper.dict_RuleInfo.Where(v => v.Value.ColumnName == colName && v.Value.detail_level == detail_level).ToDictionary(v => v.Key, v => v.Value); // To get all the rules for the selected column

                        if (dict_temp.Any())
                        {
                            btn_AddRule.Enabled = true;
                            btn_EditRule.Enabled = true;
                            btn_DeleteRule.Enabled = true;
                            cb_RuleNames.Enabled = true;
                            cb_ColumnNames.Enabled = true;

                            cb_RuleNames.Properties.Items.Clear();
                            cb_RuleNames.Properties.Items.AddRange(dict_temp.Keys.ToArray());
                            cb_RuleNames.Text = CollectionHelper.dict_RuleInfo.Keys.ToArray()[0];
                        }
                        else
                        {
                            btn_AddRule.Enabled = true;
                            btn_EditRule.Enabled = true;
                            btn_DeleteRule.Enabled = true;
                            cb_RuleNames.Enabled = true;
                            cb_ColumnNames.Enabled = true;

                            cb_RuleNames.Visible = false;
                            lbl_RuleNameMainControl.Visible = false;
                            //btn_DeleteRule.Visible = false;
                            btn_EditRule.Visible = false;
                        }

                    }
                    else
                    {
                        btn_AddRule.Enabled = true;
                        btn_EditRule.Enabled = true;
                        btn_DeleteRule.Enabled = true;
                        cb_RuleNames.Enabled = true;
                        cb_ColumnNames.Enabled = true;
                        return;
                    }
                }                    
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {

                if (ValidateChildren(ValidationConstraints.Enabled))
                {
                    RuleAlert ruleAlert = new RuleAlert();

                    string ruleName = txt_RuleName.Text.ToString().Trim();
                    string colName = cb_ColumnNames.Text.ToString();

                    ruleAlert.detail_level = detail_level;
                    ruleAlert.ColumnName = colName;

                    List<string> selectedclients = new List<string>();
                    //selectedclients = chkcb_ClientList.Properties.Items.GetCheckedValues().Select(v => (string)v).ToList();
                    List<TreeListNode> lst_SelectedClients = tl_ClientList.GetAllCheckedNodes();
                    foreach (var client in lst_SelectedClients)
                    {
                        if (client.HasChildren == false)
                        {
                            selectedclients.Add(client.GetValue(tlc_SelectClients).ToString());
                        }
                    }

                    // Group1 
                    var Group1 = new Group
                    {
                        Op1 = cmb_Group1Rule1.Text,
                        Value1 = txt_Group1Rule1.Text.ToString() != "" ? Convert.ToDouble(txt_Group1Rule1.Text.ToString()) : Double.NaN,
                        LogicalOp = cmb_Group1.Text.ToString(),
                        Op2 = cmb_Group1Rule2.Text.ToString(),
                        Value2 = txt_Group1Rule2.Text.ToString() != "" ? Convert.ToDouble(txt_Group1Rule2.Text.ToString()) : Double.NaN,
                        AlertMessage = txt_Group1AlertText.Text.ToString()
                    };

                    //Group2
                    var Group2 = new Group
                    {
                        Op1 = cmb_Group2Rule1.Text,
                        Value1 = txt_Group2Rule1.Text != "" ? Convert.ToDouble(txt_Group2Rule1.Text) : Double.NaN,
                        LogicalOp = cmb_Group2.Text,
                        Op2 = cmb_Group2Rule2.Text,
                        Value2 = txt_Group2Rule2.Text != "" ? Convert.ToDouble(txt_Group2Rule2.Text) : Double.NaN,
                        AlertMessage = txt_Group2AlertText.Text
                    };

                    //Group3
                    var Group3 = new Group
                    {
                        Op1 = cmb_Group3Rule1.Text,
                        Value1 = txt_Group3Rule1.Text != "" ? Convert.ToDouble(txt_Group3Rule1.Text) : Double.NaN,
                        LogicalOp = cmb_Group3.Text,
                        Op2 = cmb_Group3Rule2.Text,
                        Value2 = txt_Group3Rule2.Text != "" ? Convert.ToDouble(txt_Group3Rule2.Text) : Double.NaN,
                        AlertMessage = txt_Group3AlertText.Text
                    };

                    List<Group> list_groups = new List<Group>();
                    list_groups.AddRange(new Group[] { Group1, Group2, Group3 });
                    ruleAlert.list_Groups = list_groups;

                    //Client List
                    ruleAlert.list_Clients = selectedclients;

                    ruleAlert.arr_Scrips = txt_ScripName.Text.ToUpper().Split(',');

                    if (CollectionHelper.dict_RuleInfo.ContainsKey(ruleName))
                    {
                        CollectionHelper.dict_RuleInfo[ruleName] = ruleAlert;
                    }
                    else
                    {

                        CollectionHelper.dict_RuleInfo.Add(ruleName, ruleAlert);
                    }

                    if (!IsEditWindow)
                        list_RuleName.Add(txt_RuleName.Text.Trim());

                    string str_Rule = JsonConvert.SerializeObject(CollectionHelper.dict_RuleInfo);
                    File.WriteAllText(rulefileloc, str_Rule);


                    pc_RuleEditor.Visible = false;


                    lbl_RuleNameMainControl.Visible = true;
                    cb_RuleNames.Visible = true;
                    btn_EditRule.Visible = true;
                    // btn_DeleteRule.Visible = true;

                    var dict_temp = CollectionHelper.dict_RuleInfo.Where(v => v.Value.ColumnName == colName && v.Value.detail_level == detail_level).ToDictionary(v => v.Key, v => v.Value); // To get all the rules for the selected column
                    cb_RuleNames.Properties.Items.Clear();
                    cb_RuleNames.Properties.Items.AddRange(dict_temp.Keys.ToArray());
                    cb_RuleNames.Text = dict_temp.Keys.ToArray()[0];

                    cb_ColumnNames.Enabled = true;
                    cb_RuleNames.Enabled = true;
                    btn_AddRule.Enabled = true;
                    btn_DeleteRule.Enabled = true;
                    btn_EditRule.Enabled = true;

                }
                
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                pc_RuleEditor.Visible = false;

                cb_ColumnNames.Enabled = true;
                cb_RuleNames.Enabled = true;
                btn_AddRule.Enabled = true;
                btn_DeleteRule.Enabled = true;
                btn_EditRule.Enabled = true;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void AddClientInfo(List<string> list_selectedClients)
        {
            try
            {

                tl_ClientList.ClearNodes();
                //Added on 05-11-2020 by Akshay for newMappedClients treelist in Add user
                if (dict_ClientData.Count != 0)     //Added by Akshay For Avoiding error when Clientdata is Empty
                {
                    TreeListNode Selectednode = tl_ClientList.AppendNode(null, null);
                    Selectednode.SetValue("name", "Select All");
                    foreach (var zoneitem in dict_ClientData)
                    {
                        TreeListNode zonenode = tl_ClientList.AppendNode(null, Selectednode);
                        zonenode.SetValue("name", zoneitem.Key);
                        foreach (var branchitem in zoneitem.Value)
                        {
                            TreeListNode branchnode = tl_ClientList.AppendNode(null, zonenode);
                            branchnode.SetValue("name", branchitem.Key);
                            foreach (var familyitem in branchitem.Value)
                            {
                                TreeListNode familynode = tl_ClientList.AppendNode(null, branchnode);
                                familynode.SetValue("name", familyitem.Key);
                                foreach (var productitem in familyitem.Value)
                                {
                                    TreeListNode productnode = tl_ClientList.AppendNode(null, familynode);
                                    productnode.SetValue("name", productitem.Key);
                                    foreach (var dealer in productitem.Value)
                                    {
                                        TreeListNode dealernode = tl_ClientList.AppendNode(null, productnode);
                                        dealernode.SetValue("name", dealer);
                                    }
                                }
                            }
                        }
                    }
                }

                if (IsEditWindow)
                {
                    foreach (TreeListNode node in tl_ClientList.GetNodeList())
                    {

                        if (list_selectedClients.Contains(node.GetValue(tlc_SelectClients).ToString()))
                        {
                            tl_ClientList.SetNodeCheckState(node, CheckState.Checked, true);
                        }
                    }
                }

                tl_ClientList.EndUnboundLoad();
                tl_ClientList.EndUpdate();
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        internal void GetRuleInfo(List<string> list_Client, List<string> list_column, int detail_level)
        {
            try
            {

                if (list_Client.Count > 0)
                    list_ClientList = new List<string>(list_Client);

                if (list_column.Count > 0)
                {
                    list_ColumnList = new List<string>(list_column);
                }

                dict_ClientInfo = new ConcurrentDictionary<string, ClientInfo>(CollectionHelper.dict_ClientInfo);

                this.detail_level = detail_level;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }


        private void cb_ColumnNames_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string colName = cb_ColumnNames.Text.ToString();

                lbl_RuleNameMainControl.Visible = false;
                cb_RuleNames.Visible = false;
                btn_AddRule.Visible = false;
                btn_EditRule.Visible = false;
                //btn_DeleteRule.Visible = false;

                if (list_ColumnList.Contains(colName))
                {
                    var dict_temp = CollectionHelper.dict_RuleInfo.Where(v => v.Value.ColumnName == colName && v.Value.detail_level == detail_level).ToDictionary(v => v.Key, v => v.Value); // To get all the rules for the selected column

                    if (dict_temp.Any())
                    {
                        lbl_RuleNameMainControl.Visible = true;
                        cb_RuleNames.Properties.Items.Clear();
                        cb_RuleNames.Properties.Items.AddRange(dict_temp.Keys.ToArray());
                        cb_RuleNames.Text = dict_temp.Keys.ToArray()[0];
                        cb_RuleNames.Visible = true;
                        btn_AddRule.Visible = true;
                        btn_EditRule.Visible = true;
                        //.Visible = true;

                    }
                    else
                    {
                        btn_AddRule.Visible = true;

                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void form_RuleBuilder_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                eve_RuleManagerFormClosed();
            }
            catch(Exception ee) { _logger.Error(ee); }
        }

        private void btn_SelectClients_Click(object sender, EventArgs e)
        {
            try
            {
                btn_SelectClients.Visible = false;
                pnl_ClientList.Visible = true;
            }
            catch(Exception ee) { _logger.Error(ee); }
        }

        private void btn_ClientList_Ok_Click(object sender, EventArgs e)
        {
            try
            {
                pnl_ClientList.Visible = false;
                btn_SelectClients.Visible = true;
            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        private void tl_ClientList_Validating(object sender, CancelEventArgs e)
        {
            try
            {

                if (tl_ClientList.GetAllCheckedNodes().Count == 0) { e.Cancel = true; btn_SelectClients.Focus(); errorProvider.SetError(btn_SelectClients, "Cannot be empty"); } else { errorProvider.SetError(btn_SelectClients, null); }
                
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void chkCB_DeleteRuleList_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if(chkCB_DeleteRuleList.Text != "")
                {

                    if (XtraMessageBox.Show("Do you really want to delete this rule?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        string[] Rules = chkCB_DeleteRuleList.Text.Split(',');

                        foreach(var rule in Rules)
                        {
                            CollectionHelper.dict_RuleInfo.Remove(rule);

                            if (list_RuleName.Contains(rule))
                                list_RuleName.Remove(rule);
                        }

                        if (CollectionHelper.dict_RuleInfo.Count > 0)
                        {
                            string str_Rule = JsonConvert.SerializeObject(CollectionHelper.dict_RuleInfo);
                            File.WriteAllText(rulefileloc, str_Rule);
                        }
                        else
                        {
                            File.Delete(rulefileloc);
                        }

                        XtraMessageBox.Show($"Rules Deleted", "Information");

                        btn_DeleteRule.Enabled = true;
                        cb_ColumnNames.Enabled = true;
                        btn_AddRule.Enabled = true;
                        chkCB_DeleteRuleList.Visible = false;
                    }
                    else
                    {
                        btn_DeleteRule.Enabled = true;
                        cb_ColumnNames.Enabled = true;
                        btn_AddRule.Enabled = true;

                        chkCB_DeleteRuleList.Properties.Items.Clear();
                        chkCB_DeleteRuleList.Visible = false;
                    }
                        
                }
                         
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void chkCB_DeleteRuleList_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if(chkCB_DeleteRuleList.Text == "")
                {
                    btn_DeleteRule.Enabled = true;
                    cb_ColumnNames.Enabled = true;
                    btn_AddRule.Enabled = true;
                    chkCB_DeleteRuleList.Visible = false;
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
    }
}