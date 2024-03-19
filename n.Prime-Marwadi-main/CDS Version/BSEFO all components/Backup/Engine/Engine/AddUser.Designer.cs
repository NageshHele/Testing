using DevExpress.XtraEditors;

namespace Engine
{
    partial class AddUser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddUser));
            this.pnlAddUser = new DevExpress.XtraEditors.PanelControl();
            this.btn_ClientsUpdate = new DevExpress.XtraEditors.SimpleButton();
            this.btnEditUserMapping = new DevExpress.XtraEditors.SimpleButton();
            this.lblClients = new DevExpress.XtraEditors.LabelControl();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.btnAReset = new DevExpress.XtraEditors.SimpleButton();
            this.btnAddUser = new DevExpress.XtraEditors.SimpleButton();
            this.ckeIsAdmin = new DevExpress.XtraEditors.CheckEdit();
            this.lblIsAdmin = new DevExpress.XtraEditors.LabelControl();
            this.txtePassword = new DevExpress.XtraEditors.TextEdit();
            this.txteUsername = new DevExpress.XtraEditors.TextEdit();
            this.lblUserChange = new DevExpress.XtraEditors.LabelControl();
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.pnlResetPassword = new DevExpress.XtraEditors.PanelControl();
            this.cmb_RUsername = new DevExpress.XtraEditors.ComboBoxEdit();
            this.btnRAdd = new DevExpress.XtraEditors.SimpleButton();
            this.btnResetPassword = new DevExpress.XtraEditors.SimpleButton();
            this.lblRUsername = new DevExpress.XtraEditors.LabelControl();
            this.pnlEditMappedUsers = new DevExpress.XtraEditors.PanelControl();
            this.btnMaapedClients = new DevExpress.XtraEditors.SimpleButton();
            this.btn_ClearMapping = new DevExpress.XtraEditors.SimpleButton();
            this.ccbe_Username = new DevExpress.XtraEditors.ComboBoxEdit();
            this.btnUpdateClient = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.lblUser = new DevExpress.XtraEditors.LabelControl();
            this.btnBacktoUser = new DevExpress.XtraEditors.SimpleButton();
            this.pnlSelectClients = new DevExpress.XtraEditors.PanelControl();
            this.pnlOK = new System.Windows.Forms.Panel();
            this.btn_okSelectClients = new DevExpress.XtraEditors.SimpleButton();
            this.btn_okMappedClients = new DevExpress.XtraEditors.SimpleButton();
            this.tl_MappedClients = new DevExpress.XtraTreeList.TreeList();
            this.tlc_MappedClients = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tl_SelectClients = new DevExpress.XtraTreeList.TreeList();
            this.tlc_Clients = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAddUser)).BeginInit();
            this.pnlAddUser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ckeIsAdmin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtePassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txteUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlResetPassword)).BeginInit();
            this.pnlResetPassword.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmb_RUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlEditMappedUsers)).BeginInit();
            this.pnlEditMappedUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ccbe_Username.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSelectClients)).BeginInit();
            this.pnlSelectClients.SuspendLayout();
            this.pnlOK.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tl_MappedClients)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tl_SelectClients)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlAddUser
            // 
            this.pnlAddUser.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlAddUser.Controls.Add(this.btn_ClientsUpdate);
            this.pnlAddUser.Controls.Add(this.btnEditUserMapping);
            this.pnlAddUser.Controls.Add(this.lblClients);
            this.pnlAddUser.Controls.Add(this.btnDelete);
            this.pnlAddUser.Controls.Add(this.btnAReset);
            this.pnlAddUser.Controls.Add(this.btnAddUser);
            this.pnlAddUser.Controls.Add(this.ckeIsAdmin);
            this.pnlAddUser.Controls.Add(this.lblIsAdmin);
            this.pnlAddUser.Controls.Add(this.txtePassword);
            this.pnlAddUser.Controls.Add(this.txteUsername);
            this.pnlAddUser.Controls.Add(this.lblUserChange);
            this.pnlAddUser.Controls.Add(this.lblPassword);
            this.pnlAddUser.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlAddUser.Location = new System.Drawing.Point(0, 0);
            this.pnlAddUser.Margin = new System.Windows.Forms.Padding(0);
            this.pnlAddUser.Name = "pnlAddUser";
            this.pnlAddUser.Size = new System.Drawing.Size(249, 297);
            this.pnlAddUser.TabIndex = 0;
            this.pnlAddUser.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlAddUser_Paint);
            // 
            // btn_ClientsUpdate
            // 
            this.btn_ClientsUpdate.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ClientsUpdate.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btn_ClientsUpdate.Appearance.Options.UseFont = true;
            this.btn_ClientsUpdate.Appearance.Options.UseForeColor = true;
            this.btn_ClientsUpdate.Location = new System.Drawing.Point(100, 84);
            this.btn_ClientsUpdate.LookAndFeel.SkinName = "DevExpress Style";
            this.btn_ClientsUpdate.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_ClientsUpdate.Name = "btn_ClientsUpdate";
            this.btn_ClientsUpdate.Size = new System.Drawing.Size(127, 28);
            this.btn_ClientsUpdate.TabIndex = 15;
            this.btn_ClientsUpdate.Text = "Select Clients";
            this.btn_ClientsUpdate.Click += new System.EventHandler(this.btn_ClientsUpdate_Click);
            // 
            // btnEditUserMapping
            // 
            this.btnEditUserMapping.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditUserMapping.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnEditUserMapping.Appearance.Options.UseFont = true;
            this.btnEditUserMapping.Appearance.Options.UseForeColor = true;
            this.btnEditUserMapping.Location = new System.Drawing.Point(19, 207);
            this.btnEditUserMapping.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btnEditUserMapping.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnEditUserMapping.Name = "btnEditUserMapping";
            this.btnEditUserMapping.Size = new System.Drawing.Size(90, 34);
            this.btnEditUserMapping.TabIndex = 17;
            this.btnEditUserMapping.Text = "Edit User";
            this.btnEditUserMapping.Click += new System.EventHandler(this.btnEditUserMapping_Click);
            // 
            // lblClients
            // 
            this.lblClients.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblClients.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lblClients.Appearance.Options.UseFont = true;
            this.lblClients.Appearance.Options.UseForeColor = true;
            this.lblClients.Location = new System.Drawing.Point(36, 86);
            this.lblClients.Name = "lblClients";
            this.lblClients.Size = new System.Drawing.Size(49, 21);
            this.lblClients.TabIndex = 15;
            this.lblClients.Text = "Clients";
            // 
            // btnDelete
            // 
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Appearance.Options.UseForeColor = true;
            this.btnDelete.Location = new System.Drawing.Point(132, 207);
            this.btnDelete.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btnDelete.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(90, 34);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnAReset
            // 
            this.btnAReset.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAReset.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnAReset.Appearance.Options.UseFont = true;
            this.btnAReset.Appearance.Options.UseForeColor = true;
            this.btnAReset.Location = new System.Drawing.Point(19, 153);
            this.btnAReset.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btnAReset.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnAReset.Name = "btnAReset";
            this.btnAReset.Size = new System.Drawing.Size(90, 34);
            this.btnAReset.TabIndex = 12;
            this.btnAReset.Text = "Reset";
            this.btnAReset.Click += new System.EventHandler(this.BtnAReset_Click);
            // 
            // btnAddUser
            // 
            this.btnAddUser.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddUser.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnAddUser.Appearance.Options.UseFont = true;
            this.btnAddUser.Appearance.Options.UseForeColor = true;
            this.btnAddUser.Location = new System.Drawing.Point(132, 153);
            this.btnAddUser.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btnAddUser.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.Size = new System.Drawing.Size(90, 34);
            this.btnAddUser.TabIndex = 11;
            this.btnAddUser.Text = "Add";
            this.btnAddUser.Click += new System.EventHandler(this.BtnAddUser_Click);
            // 
            // ckeIsAdmin
            // 
            this.ckeIsAdmin.Location = new System.Drawing.Point(100, 121);
            this.ckeIsAdmin.Name = "ckeIsAdmin";
            this.ckeIsAdmin.Properties.Caption = "";
            this.ckeIsAdmin.Size = new System.Drawing.Size(17, 20);
            this.ckeIsAdmin.TabIndex = 10;
            // 
            // lblIsAdmin
            // 
            this.lblIsAdmin.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIsAdmin.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lblIsAdmin.Appearance.Options.UseFont = true;
            this.lblIsAdmin.Appearance.Options.UseForeColor = true;
            this.lblIsAdmin.Location = new System.Drawing.Point(25, 118);
            this.lblIsAdmin.Name = "lblIsAdmin";
            this.lblIsAdmin.Size = new System.Drawing.Size(60, 21);
            this.lblIsAdmin.TabIndex = 9;
            this.lblIsAdmin.Text = "IsAdmin";
            // 
            // txtePassword
            // 
            this.txtePassword.Location = new System.Drawing.Point(100, 53);
            this.txtePassword.Name = "txtePassword";
            this.txtePassword.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtePassword.Properties.Appearance.Options.UseFont = true;
            this.txtePassword.Properties.PasswordChar = '*';
            this.txtePassword.Size = new System.Drawing.Size(127, 24);
            this.txtePassword.TabIndex = 8;
            // 
            // txteUsername
            // 
            this.txteUsername.Location = new System.Drawing.Point(100, 16);
            this.txteUsername.Name = "txteUsername";
            this.txteUsername.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txteUsername.Properties.Appearance.Options.UseFont = true;
            this.txteUsername.Size = new System.Drawing.Size(127, 24);
            this.txteUsername.TabIndex = 7;
            // 
            // lblUserChange
            // 
            this.lblUserChange.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserChange.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lblUserChange.Appearance.Options.UseFont = true;
            this.lblUserChange.Appearance.Options.UseForeColor = true;
            this.lblUserChange.Location = new System.Drawing.Point(12, 14);
            this.lblUserChange.Name = "lblUserChange";
            this.lblUserChange.Size = new System.Drawing.Size(73, 21);
            this.lblUserChange.TabIndex = 6;
            this.lblUserChange.Text = "Username";
            // 
            // lblPassword
            // 
            this.lblPassword.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lblPassword.Appearance.Options.UseFont = true;
            this.lblPassword.Appearance.Options.UseForeColor = true;
            this.lblPassword.Location = new System.Drawing.Point(16, 51);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(69, 21);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "Password";
            // 
            // pnlResetPassword
            // 
            this.pnlResetPassword.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlResetPassword.Controls.Add(this.cmb_RUsername);
            this.pnlResetPassword.Controls.Add(this.btnRAdd);
            this.pnlResetPassword.Controls.Add(this.btnResetPassword);
            this.pnlResetPassword.Controls.Add(this.lblRUsername);
            this.pnlResetPassword.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlResetPassword.Location = new System.Drawing.Point(-249, 0);
            this.pnlResetPassword.Margin = new System.Windows.Forms.Padding(0);
            this.pnlResetPassword.Name = "pnlResetPassword";
            this.pnlResetPassword.Size = new System.Drawing.Size(250, 297);
            this.pnlResetPassword.TabIndex = 1;
            this.pnlResetPassword.Visible = false;
            this.pnlResetPassword.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlResetPassword_Paint);
            // 
            // cmb_RUsername
            // 
            this.cmb_RUsername.Location = new System.Drawing.Point(98, 81);
            this.cmb_RUsername.Name = "cmb_RUsername";
            this.cmb_RUsername.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.cmb_RUsername.Properties.Appearance.Options.UseFont = true;
            this.cmb_RUsername.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmb_RUsername.Size = new System.Drawing.Size(127, 24);
            this.cmb_RUsername.TabIndex = 14;
            // 
            // btnRAdd
            // 
            this.btnRAdd.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.btnRAdd.Appearance.BackColor2 = System.Drawing.Color.Transparent;
            this.btnRAdd.Appearance.BorderColor = System.Drawing.Color.Transparent;
            this.btnRAdd.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRAdd.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnRAdd.Appearance.Options.UseBackColor = true;
            this.btnRAdd.Appearance.Options.UseBorderColor = true;
            this.btnRAdd.Appearance.Options.UseFont = true;
            this.btnRAdd.Appearance.Options.UseForeColor = true;
            this.btnRAdd.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnRAdd.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnRAdd.ImageOptions.SvgImage")));
            this.btnRAdd.Location = new System.Drawing.Point(2, 19);
            this.btnRAdd.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btnRAdd.LookAndFeel.UseWindowsXPTheme = true;
            this.btnRAdd.Name = "btnRAdd";
            this.btnRAdd.Size = new System.Drawing.Size(40, 34);
            this.btnRAdd.TabIndex = 13;
            this.btnRAdd.ToolTip = "Back to Add User";
            this.btnRAdd.ToolTipTitle = "Back";
            this.btnRAdd.Click += new System.EventHandler(this.BtnRAdd_Click);
            // 
            // btnResetPassword
            // 
            this.btnResetPassword.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResetPassword.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnResetPassword.Appearance.Options.UseFont = true;
            this.btnResetPassword.Appearance.Options.UseForeColor = true;
            this.btnResetPassword.Location = new System.Drawing.Point(98, 120);
            this.btnResetPassword.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btnResetPassword.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnResetPassword.Name = "btnResetPassword";
            this.btnResetPassword.Size = new System.Drawing.Size(127, 34);
            this.btnResetPassword.TabIndex = 12;
            this.btnResetPassword.Text = "Reset Password";
            this.btnResetPassword.Click += new System.EventHandler(this.BtnResetPassword_Click);
            // 
            // lblRUsername
            // 
            this.lblRUsername.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRUsername.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lblRUsername.Appearance.Options.UseFont = true;
            this.lblRUsername.Appearance.Options.UseForeColor = true;
            this.lblRUsername.Location = new System.Drawing.Point(14, 79);
            this.lblRUsername.Name = "lblRUsername";
            this.lblRUsername.Size = new System.Drawing.Size(73, 21);
            this.lblRUsername.TabIndex = 8;
            this.lblRUsername.Text = "Username";
            // 
            // pnlEditMappedUsers
            // 
            this.pnlEditMappedUsers.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlEditMappedUsers.Controls.Add(this.btnMaapedClients);
            this.pnlEditMappedUsers.Controls.Add(this.btn_ClearMapping);
            this.pnlEditMappedUsers.Controls.Add(this.ccbe_Username);
            this.pnlEditMappedUsers.Controls.Add(this.btnUpdateClient);
            this.pnlEditMappedUsers.Controls.Add(this.labelControl2);
            this.pnlEditMappedUsers.Controls.Add(this.lblUser);
            this.pnlEditMappedUsers.Controls.Add(this.btnBacktoUser);
            this.pnlEditMappedUsers.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlEditMappedUsers.Location = new System.Drawing.Point(1, 0);
            this.pnlEditMappedUsers.Margin = new System.Windows.Forms.Padding(0);
            this.pnlEditMappedUsers.Name = "pnlEditMappedUsers";
            this.pnlEditMappedUsers.Size = new System.Drawing.Size(242, 297);
            this.pnlEditMappedUsers.TabIndex = 2;
            this.pnlEditMappedUsers.Visible = false;
            this.pnlEditMappedUsers.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlEditMappedUsers_Paint);
            // 
            // btnMaapedClients
            // 
            this.btnMaapedClients.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMaapedClients.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnMaapedClients.Appearance.Options.UseFont = true;
            this.btnMaapedClients.Appearance.Options.UseForeColor = true;
            this.btnMaapedClients.Location = new System.Drawing.Point(110, 96);
            this.btnMaapedClients.LookAndFeel.SkinName = "DevExpress Style";
            this.btnMaapedClients.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnMaapedClients.Name = "btnMaapedClients";
            this.btnMaapedClients.Size = new System.Drawing.Size(119, 31);
            this.btnMaapedClients.TabIndex = 28;
            this.btnMaapedClients.Text = "Mapped Clients";
            this.btnMaapedClients.Click += new System.EventHandler(this.btnMaapedClients_Click);
            // 
            // btn_ClearMapping
            // 
            this.btn_ClearMapping.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ClearMapping.Appearance.ForeColor = System.Drawing.Color.White;
            this.btn_ClearMapping.Appearance.Options.UseFont = true;
            this.btn_ClearMapping.Appearance.Options.UseForeColor = true;
            this.btn_ClearMapping.Location = new System.Drawing.Point(10, 143);
            this.btn_ClearMapping.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_ClearMapping.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_ClearMapping.Name = "btn_ClearMapping";
            this.btn_ClearMapping.Size = new System.Drawing.Size(99, 34);
            this.btn_ClearMapping.TabIndex = 26;
            this.btn_ClearMapping.Text = "Clear Mapping";
            this.btn_ClearMapping.Click += new System.EventHandler(this.btn_ClearMapping_Click);
            // 
            // ccbe_Username
            // 
            this.ccbe_Username.Location = new System.Drawing.Point(104, 56);
            this.ccbe_Username.Name = "ccbe_Username";
            this.ccbe_Username.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.ccbe_Username.Properties.Appearance.Options.UseFont = true;
            this.ccbe_Username.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ccbe_Username.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.ccbe_Username.Size = new System.Drawing.Size(125, 24);
            this.ccbe_Username.TabIndex = 23;
            this.ccbe_Username.SelectedIndexChanged += new System.EventHandler(this.ccbe_Username_SelectedIndexChanged);
            this.ccbe_Username.Click += new System.EventHandler(this.ccbe_Username_Click);
            // 
            // btnUpdateClient
            // 
            this.btnUpdateClient.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateClient.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnUpdateClient.Appearance.Options.UseFont = true;
            this.btnUpdateClient.Appearance.Options.UseForeColor = true;
            this.btnUpdateClient.Location = new System.Drawing.Point(115, 143);
            this.btnUpdateClient.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btnUpdateClient.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnUpdateClient.Name = "btnUpdateClient";
            this.btnUpdateClient.Size = new System.Drawing.Size(114, 34);
            this.btnUpdateClient.TabIndex = 21;
            this.btnUpdateClient.Text = "Update Clients";
            this.btnUpdateClient.Click += new System.EventHandler(this.btnUpdateClient_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelControl2.Appearance.ForeColor = System.Drawing.Color.Black;
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Appearance.Options.UseForeColor = true;
            this.labelControl2.Location = new System.Drawing.Point(37, 99);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(49, 21);
            this.labelControl2.TabIndex = 19;
            this.labelControl2.Text = "Clients";
            // 
            // lblUser
            // 
            this.lblUser.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblUser.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lblUser.Appearance.Options.UseFont = true;
            this.lblUser.Appearance.Options.UseForeColor = true;
            this.lblUser.Location = new System.Drawing.Point(10, 54);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(73, 21);
            this.lblUser.TabIndex = 17;
            this.lblUser.Text = "Username";
            // 
            // btnBacktoUser
            // 
            this.btnBacktoUser.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.btnBacktoUser.Appearance.BackColor2 = System.Drawing.Color.Transparent;
            this.btnBacktoUser.Appearance.BorderColor = System.Drawing.Color.Transparent;
            this.btnBacktoUser.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBacktoUser.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnBacktoUser.Appearance.Options.UseBackColor = true;
            this.btnBacktoUser.Appearance.Options.UseBorderColor = true;
            this.btnBacktoUser.Appearance.Options.UseFont = true;
            this.btnBacktoUser.Appearance.Options.UseForeColor = true;
            this.btnBacktoUser.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnBacktoUser.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnBacktoUser.ImageOptions.SvgImage")));
            this.btnBacktoUser.Location = new System.Drawing.Point(0, 10);
            this.btnBacktoUser.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btnBacktoUser.LookAndFeel.UseWindowsXPTheme = true;
            this.btnBacktoUser.Name = "btnBacktoUser";
            this.btnBacktoUser.Size = new System.Drawing.Size(40, 34);
            this.btnBacktoUser.TabIndex = 22;
            this.btnBacktoUser.ToolTip = "Back to Add User";
            this.btnBacktoUser.ToolTipTitle = "Back";
            this.btnBacktoUser.Click += new System.EventHandler(this.BtnRAdd_Click);
            // 
            // pnlSelectClients
            // 
            this.pnlSelectClients.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSelectClients.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlSelectClients.Controls.Add(this.pnlOK);
            this.pnlSelectClients.Controls.Add(this.tl_MappedClients);
            this.pnlSelectClients.Controls.Add(this.tl_SelectClients);
            this.pnlSelectClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSelectClients.Location = new System.Drawing.Point(249, 0);
            this.pnlSelectClients.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSelectClients.Name = "pnlSelectClients";
            this.pnlSelectClients.Size = new System.Drawing.Size(0, 297);
            this.pnlSelectClients.TabIndex = 28;
            this.pnlSelectClients.Visible = false;
            // 
            // pnlOK
            // 
            this.pnlOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlOK.Controls.Add(this.btn_okSelectClients);
            this.pnlOK.Controls.Add(this.btn_okMappedClients);
            this.pnlOK.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlOK.Location = new System.Drawing.Point(0, 260);
            this.pnlOK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlOK.Name = "pnlOK";
            this.pnlOK.Size = new System.Drawing.Size(0, 37);
            this.pnlOK.TabIndex = 31;
            // 
            // btn_okSelectClients
            // 
            this.btn_okSelectClients.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_okSelectClients.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_okSelectClients.Appearance.ForeColor = System.Drawing.Color.White;
            this.btn_okSelectClients.Appearance.Options.UseFont = true;
            this.btn_okSelectClients.Appearance.Options.UseForeColor = true;
            this.btn_okSelectClients.Location = new System.Drawing.Point(-75, 6);
            this.btn_okSelectClients.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_okSelectClients.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_okSelectClients.Name = "btn_okSelectClients";
            this.btn_okSelectClients.Size = new System.Drawing.Size(61, 25);
            this.btn_okSelectClients.TabIndex = 30;
            this.btn_okSelectClients.Text = "OK";
            this.btn_okSelectClients.Click += new System.EventHandler(this.btn_okSelectClients_Click);
            // 
            // btn_okMappedClients
            // 
            this.btn_okMappedClients.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_okMappedClients.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_okMappedClients.Appearance.ForeColor = System.Drawing.Color.White;
            this.btn_okMappedClients.Appearance.Options.UseFont = true;
            this.btn_okMappedClients.Appearance.Options.UseForeColor = true;
            this.btn_okMappedClients.Location = new System.Drawing.Point(-75, 6);
            this.btn_okMappedClients.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_okMappedClients.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_okMappedClients.Name = "btn_okMappedClients";
            this.btn_okMappedClients.Size = new System.Drawing.Size(61, 25);
            this.btn_okMappedClients.TabIndex = 29;
            this.btn_okMappedClients.Text = "OK";
            this.btn_okMappedClients.Click += new System.EventHandler(this.btn_okMappedClients_Click);
            // 
            // tl_MappedClients
            // 
            this.tl_MappedClients.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.tlc_MappedClients});
            this.tl_MappedClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tl_MappedClients.Location = new System.Drawing.Point(0, 0);
            this.tl_MappedClients.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tl_MappedClients.MinWidth = 17;
            this.tl_MappedClients.Name = "tl_MappedClients";
            this.tl_MappedClients.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.tl_MappedClients.OptionsBehavior.Editable = false;
            this.tl_MappedClients.OptionsSelection.MultiSelect = true;
            this.tl_MappedClients.OptionsView.CheckBoxStyle = DevExpress.XtraTreeList.DefaultNodeCheckBoxStyle.Check;
            this.tl_MappedClients.OptionsView.ShowColumns = false;
            this.tl_MappedClients.OptionsView.ShowIndicator = false;
            this.tl_MappedClients.Size = new System.Drawing.Size(0, 297);
            this.tl_MappedClients.TabIndex = 28;
            this.tl_MappedClients.TreeLevelWidth = 15;
            this.tl_MappedClients.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tl_MappedClients_FocusedNodeChanged);
            // 
            // tlc_MappedClients
            // 
            this.tlc_MappedClients.Caption = "tlc_MappedClients";
            this.tlc_MappedClients.FieldName = "name";
            this.tlc_MappedClients.MinWidth = 17;
            this.tlc_MappedClients.Name = "tlc_MappedClients";
            this.tlc_MappedClients.Visible = true;
            this.tlc_MappedClients.VisibleIndex = 0;
            // 
            // tl_SelectClients
            // 
            this.tl_SelectClients.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.tlc_Clients});
            this.tl_SelectClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tl_SelectClients.Location = new System.Drawing.Point(0, 0);
            this.tl_SelectClients.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tl_SelectClients.MinWidth = 17;
            this.tl_SelectClients.Name = "tl_SelectClients";
            this.tl_SelectClients.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.tl_SelectClients.OptionsBehavior.Editable = false;
            this.tl_SelectClients.OptionsSelection.MultiSelect = true;
            this.tl_SelectClients.OptionsView.CheckBoxStyle = DevExpress.XtraTreeList.DefaultNodeCheckBoxStyle.Check;
            this.tl_SelectClients.OptionsView.ShowColumns = false;
            this.tl_SelectClients.OptionsView.ShowIndicator = false;
            this.tl_SelectClients.Size = new System.Drawing.Size(0, 297);
            this.tl_SelectClients.TabIndex = 28;
            this.tl_SelectClients.TreeLevelWidth = 15;
            this.tl_SelectClients.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tl_SelectClients_FocusedNodeChanged);
            // 
            // tlc_Clients
            // 
            this.tlc_Clients.Caption = "tlc_Clients";
            this.tlc_Clients.FieldName = "name";
            this.tlc_Clients.MinWidth = 17;
            this.tlc_Clients.Name = "tlc_Clients";
            this.tlc_Clients.Visible = true;
            this.tlc_Clients.VisibleIndex = 0;
            this.tlc_Clients.Width = 64;
            // 
            // AddUser
            // 
            this.Appearance.ForeColor = System.Drawing.Color.Black;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 297);
            this.Controls.Add(this.pnlSelectClients);
            this.Controls.Add(this.pnlResetPassword);
            this.Controls.Add(this.pnlAddUser);
            this.Controls.Add(this.pnlEditMappedUsers);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("AddUser.IconOptions.Icon")));
            this.LookAndFeel.SkinName = "The Bezier";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(238, 283);
            this.Name = "AddUser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add/Reset User";
            ((System.ComponentModel.ISupportInitialize)(this.pnlAddUser)).EndInit();
            this.pnlAddUser.ResumeLayout(false);
            this.pnlAddUser.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ckeIsAdmin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtePassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txteUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlResetPassword)).EndInit();
            this.pnlResetPassword.ResumeLayout(false);
            this.pnlResetPassword.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmb_RUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlEditMappedUsers)).EndInit();
            this.pnlEditMappedUsers.ResumeLayout(false);
            this.pnlEditMappedUsers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ccbe_Username.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSelectClients)).EndInit();
            this.pnlSelectClients.ResumeLayout(false);
            this.pnlOK.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tl_MappedClients)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tl_SelectClients)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlAddUser;
        private DevExpress.XtraEditors.PanelControl pnlResetPassword;
        private DevExpress.XtraEditors.LabelControl lblUserChange;
        private DevExpress.XtraEditors.LabelControl lblPassword;
        private DevExpress.XtraEditors.TextEdit txteUsername;
        private DevExpress.XtraEditors.TextEdit txtePassword;
        private DevExpress.XtraEditors.CheckEdit ckeIsAdmin;
        private DevExpress.XtraEditors.LabelControl lblIsAdmin;
        private DevExpress.XtraEditors.LabelControl lblRUsername;
        private DevExpress.XtraEditors.SimpleButton btnResetPassword;
        private DevExpress.XtraEditors.SimpleButton btnAReset;
        private DevExpress.XtraEditors.SimpleButton btnAddUser;
        private DevExpress.XtraEditors.SimpleButton btnRAdd;
        private DevExpress.XtraEditors.SimpleButton btnDelete;
        private LabelControl lblClients;
        private PanelControl pnlEditMappedUsers;
        private LabelControl labelControl2;
        private LabelControl lblUser;
        private SimpleButton btnEditUserMapping;
        private SimpleButton btnUpdateClient;
        private SimpleButton btnBacktoUser;
        private ComboBoxEdit cmb_RUsername;
        private ComboBoxEdit ccbe_Username;
        private SimpleButton btn_ClearMapping;
        private SimpleButton btn_ClientsUpdate;
        private PanelControl pnlSelectClients;
        private DevExpress.XtraTreeList.TreeList tl_SelectClients;
        private SimpleButton btnMaapedClients;
        private DevExpress.XtraTreeList.TreeList tl_MappedClients;
        private SimpleButton btn_okSelectClients;
        private SimpleButton btn_okMappedClients;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlc_Clients;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlc_MappedClients;
        private System.Windows.Forms.Panel pnlOK;
    }
}