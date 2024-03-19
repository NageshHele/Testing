namespace Engine
{
    partial class EngineProcess
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
            this.components = new System.ComponentModel.Container();
            DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EngineProcess));
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.lb_ErrorLog = new DevExpress.XtraEditors.ListBoxControl();
            this.btn_Start = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Stop = new DevExpress.XtraEditors.SimpleButton();
            this.panel_EngineProcess = new DevExpress.XtraEditors.PanelControl();
            this.btn_UploadLedger = new DevExpress.XtraEditors.SimpleButton();
            this.btn_UploadEarlyPayIn = new DevExpress.XtraEditors.SimpleButton();
            this.checkTerms = new System.Windows.Forms.CheckBox();
            this.lbl_Terms = new System.Windows.Forms.LinkLabel();
            this.radioGroup_MarkToPrice = new DevExpress.XtraEditors.RadioGroup();
            this.btn_EODPositions = new DevExpress.XtraEditors.SimpleButton();
            this.chkEdit_MarkToClosing = new DevExpress.XtraEditors.CheckEdit();
            this.btn_AddUser = new DevExpress.XtraEditors.SimpleButton();
            this.chkEdit_ClearEOD = new DevExpress.XtraEditors.CheckEdit();
            this.btn_EditUploadClient = new DevExpress.XtraEditors.SimpleButton();
            this.btn_StartProcess = new DevExpress.XtraEditors.SimpleButton();
            this.chkEdit_Day1CM = new DevExpress.XtraEditors.CheckEdit();
            this.chkEdit_BODPS03 = new DevExpress.XtraEditors.CheckEdit();
            this.lbl_LatestTrade = new DevExpress.XtraEditors.LabelControl();
            this.notifyIconEngineProcess = new System.Windows.Forms.NotifyIcon(this.components);
            this.dockManager_Main = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.hideContainerRight = new DevExpress.XtraBars.Docking.AutoHideContainer();
            this.dockPanel_Right = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel_Right_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.gc_PrimeConnections = new DevExpress.XtraGrid.GridControl();
            this.gv_PrimeConnections = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repBtn_Disconnect = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            ((System.ComponentModel.ISupportInitialize)(this.lb_ErrorLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panel_EngineProcess)).BeginInit();
            this.panel_EngineProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup_MarkToPrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEdit_MarkToClosing.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEdit_ClearEOD.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEdit_Day1CM.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEdit_BODPS03.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager_Main)).BeginInit();
            this.hideContainerRight.SuspendLayout();
            this.dockPanel_Right.SuspendLayout();
            this.dockPanel_Right_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gc_PrimeConnections)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_PrimeConnections)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_Disconnect)).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_ErrorLog
            // 
            this.lb_ErrorLog.Appearance.BackColor = System.Drawing.Color.White;
            this.lb_ErrorLog.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25F);
            this.lb_ErrorLog.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lb_ErrorLog.Appearance.Options.UseBackColor = true;
            this.lb_ErrorLog.Appearance.Options.UseFont = true;
            this.lb_ErrorLog.Appearance.Options.UseForeColor = true;
            this.lb_ErrorLog.Cursor = System.Windows.Forms.Cursors.Default;
            this.lb_ErrorLog.HorizontalScrollbar = true;
            this.lb_ErrorLog.Location = new System.Drawing.Point(0, 200);
            this.lb_ErrorLog.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lb_ErrorLog.MaximumSize = new System.Drawing.Size(540, 176);
            this.lb_ErrorLog.MinimumSize = new System.Drawing.Size(0, 176);
            this.lb_ErrorLog.Name = "lb_ErrorLog";
            this.lb_ErrorLog.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_ErrorLog.ShowFocusRect = false;
            this.lb_ErrorLog.Size = new System.Drawing.Size(520, 176);
            this.lb_ErrorLog.TabIndex = 1;
            // 
            // btn_Start
            // 
            this.btn_Start.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Start.Appearance.Options.UseFont = true;
            this.btn_Start.AppearanceDisabled.BackColor = System.Drawing.Color.Gray;
            this.btn_Start.AppearanceDisabled.ForeColor = System.Drawing.Color.Gray;
            this.btn_Start.AppearanceDisabled.Options.UseBackColor = true;
            this.btn_Start.AppearanceDisabled.Options.UseForeColor = true;
            this.btn_Start.AppearancePressed.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btn_Start.AppearancePressed.BackColor2 = System.Drawing.Color.CornflowerBlue;
            this.btn_Start.AppearancePressed.Options.UseBackColor = true;
            this.btn_Start.Cursor = System.Windows.Forms.Cursors.Default;
            this.btn_Start.Enabled = false;
            this.btn_Start.Location = new System.Drawing.Point(12, 8);
            this.btn_Start.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_Start.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(78, 33);
            this.btn_Start.TabIndex = 1;
            this.btn_Start.Text = "Start";
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // btn_Stop
            // 
            this.btn_Stop.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Stop.Appearance.Options.UseFont = true;
            this.btn_Stop.AppearanceDisabled.BackColor = System.Drawing.Color.Gray;
            this.btn_Stop.AppearanceDisabled.ForeColor = System.Drawing.Color.Gray;
            this.btn_Stop.AppearanceDisabled.Options.UseBackColor = true;
            this.btn_Stop.AppearanceDisabled.Options.UseForeColor = true;
            this.btn_Stop.Cursor = System.Windows.Forms.Cursors.Default;
            this.btn_Stop.Enabled = false;
            this.btn_Stop.Location = new System.Drawing.Point(96, 8);
            this.btn_Stop.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_Stop.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(87, 33);
            this.btn_Stop.TabIndex = 33;
            this.btn_Stop.Text = "Stop";
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // panel_EngineProcess
            // 
            this.panel_EngineProcess.Appearance.BackColor = System.Drawing.Color.White;
            this.panel_EngineProcess.Appearance.BackColor2 = System.Drawing.Color.WhiteSmoke;
            this.panel_EngineProcess.Appearance.Options.UseBackColor = true;
            this.panel_EngineProcess.Controls.Add(this.btn_UploadLedger);
            this.panel_EngineProcess.Controls.Add(this.btn_UploadEarlyPayIn);
            this.panel_EngineProcess.Controls.Add(this.checkTerms);
            this.panel_EngineProcess.Controls.Add(this.lbl_Terms);
            this.panel_EngineProcess.Controls.Add(this.radioGroup_MarkToPrice);
            this.panel_EngineProcess.Controls.Add(this.btn_EODPositions);
            this.panel_EngineProcess.Controls.Add(this.chkEdit_MarkToClosing);
            this.panel_EngineProcess.Controls.Add(this.btn_AddUser);
            this.panel_EngineProcess.Controls.Add(this.chkEdit_ClearEOD);
            this.panel_EngineProcess.Controls.Add(this.btn_EditUploadClient);
            this.panel_EngineProcess.Controls.Add(this.btn_StartProcess);
            this.panel_EngineProcess.Controls.Add(this.chkEdit_Day1CM);
            this.panel_EngineProcess.Controls.Add(this.chkEdit_BODPS03);
            this.panel_EngineProcess.Controls.Add(this.lbl_LatestTrade);
            this.panel_EngineProcess.Controls.Add(this.lb_ErrorLog);
            this.panel_EngineProcess.Controls.Add(this.btn_Stop);
            this.panel_EngineProcess.Controls.Add(this.btn_Start);
            this.panel_EngineProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_EngineProcess.Location = new System.Drawing.Point(0, 0);
            this.panel_EngineProcess.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.panel_EngineProcess.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panel_EngineProcess.Name = "panel_EngineProcess";
            this.panel_EngineProcess.Size = new System.Drawing.Size(547, 432);
            this.panel_EngineProcess.TabIndex = 5;
            // 
            // btn_UploadLedger
            // 
            this.btn_UploadLedger.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btn_UploadLedger.Appearance.Options.UseFont = true;
            this.btn_UploadLedger.Enabled = false;
            this.btn_UploadLedger.Location = new System.Drawing.Point(358, 122);
            this.btn_UploadLedger.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_UploadLedger.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_UploadLedger.Name = "btn_UploadLedger";
            this.btn_UploadLedger.Size = new System.Drawing.Size(152, 31);
            this.btn_UploadLedger.TabIndex = 66;
            this.btn_UploadLedger.Text = "Upload Ledger";
            this.btn_UploadLedger.Click += new System.EventHandler(this.btn_UploadLedger_Click);
            // 
            // btn_UploadEarlyPayIn
            // 
            this.btn_UploadEarlyPayIn.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btn_UploadEarlyPayIn.Appearance.Options.UseFont = true;
            this.btn_UploadEarlyPayIn.Enabled = false;
            this.btn_UploadEarlyPayIn.Location = new System.Drawing.Point(358, 86);
            this.btn_UploadEarlyPayIn.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_UploadEarlyPayIn.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_UploadEarlyPayIn.Name = "btn_UploadEarlyPayIn";
            this.btn_UploadEarlyPayIn.Size = new System.Drawing.Size(152, 31);
            this.btn_UploadEarlyPayIn.TabIndex = 64;
            this.btn_UploadEarlyPayIn.Text = "Upload EPN File";
            this.btn_UploadEarlyPayIn.Click += new System.EventHandler(this.btn_UploadEarlyPayIn_Click);
            // 
            // checkTerms
            // 
            this.checkTerms.AutoSize = true;
            this.checkTerms.Location = new System.Drawing.Point(13, 394);
            this.checkTerms.Name = "checkTerms";
            this.checkTerms.Size = new System.Drawing.Size(18, 17);
            this.checkTerms.TabIndex = 63;
            this.checkTerms.UseVisualStyleBackColor = true;
            this.checkTerms.CheckStateChanged += new System.EventHandler(this.checkTerms_CheckedChanged);
            // 
            // lbl_Terms
            // 
            this.lbl_Terms.AutoSize = true;
            this.lbl_Terms.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Terms.LinkColor = System.Drawing.Color.Black;
            this.lbl_Terms.Location = new System.Drawing.Point(33, 389);
            this.lbl_Terms.Name = "lbl_Terms";
            this.lbl_Terms.Size = new System.Drawing.Size(319, 25);
            this.lbl_Terms.TabIndex = 62;
            this.lbl_Terms.TabStop = true;
            this.lbl_Terms.Text = "I have read the terms and conditions";
            this.lbl_Terms.VisitedLinkColor = System.Drawing.Color.RoyalBlue;
            this.lbl_Terms.Click += new System.EventHandler(this.lbl_Terms_Click);
            // 
            // radioGroup_MarkToPrice
            // 
            this.radioGroup_MarkToPrice.EditValue = "Close";
            this.radioGroup_MarkToPrice.Location = new System.Drawing.Point(110, 99);
            this.radioGroup_MarkToPrice.Name = "radioGroup_MarkToPrice";
            this.radioGroup_MarkToPrice.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.radioGroup_MarkToPrice.Properties.Appearance.Options.UseFont = true;
            this.radioGroup_MarkToPrice.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.radioGroup_MarkToPrice.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem("Close", "Close", true, "Close"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem("SettlePrice", "Settle Price", true, "Settle Price")});
            this.radioGroup_MarkToPrice.Properties.ItemsLayout = DevExpress.XtraEditors.RadioGroupItemsLayout.Flow;
            this.radioGroup_MarkToPrice.Size = new System.Drawing.Size(159, 23);
            this.radioGroup_MarkToPrice.TabIndex = 55;
            // 
            // btn_EODPositions
            // 
            this.btn_EODPositions.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btn_EODPositions.Appearance.Options.UseFont = true;
            this.btn_EODPositions.Enabled = false;
            this.btn_EODPositions.Location = new System.Drawing.Point(358, 49);
            this.btn_EODPositions.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_EODPositions.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_EODPositions.Name = "btn_EODPositions";
            this.btn_EODPositions.Size = new System.Drawing.Size(152, 31);
            this.btn_EODPositions.TabIndex = 54;
            this.btn_EODPositions.Text = "Connect nImageB";
            this.btn_EODPositions.Click += new System.EventHandler(this.btn_EODPositions_Click);
            // 
            // chkEdit_MarkToClosing
            // 
            this.chkEdit_MarkToClosing.Location = new System.Drawing.Point(12, 99);
            this.chkEdit_MarkToClosing.Name = "chkEdit_MarkToClosing";
            this.chkEdit_MarkToClosing.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEdit_MarkToClosing.Properties.Appearance.Options.UseFont = true;
            this.chkEdit_MarkToClosing.Properties.Caption = "Mark to Price";
            this.chkEdit_MarkToClosing.Size = new System.Drawing.Size(120, 25);
            this.chkEdit_MarkToClosing.TabIndex = 53;
            this.chkEdit_MarkToClosing.CheckedChanged += new System.EventHandler(this.chkEdit_MarkToClosing_CheckedChanged);
            // 
            // btn_AddUser
            // 
            this.btn_AddUser.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_AddUser.Appearance.Options.UseFont = true;
            this.btn_AddUser.AppearanceDisabled.BackColor = System.Drawing.Color.Gray;
            this.btn_AddUser.AppearanceDisabled.ForeColor = System.Drawing.Color.Gray;
            this.btn_AddUser.AppearanceDisabled.Options.UseBackColor = true;
            this.btn_AddUser.AppearanceDisabled.Options.UseForeColor = true;
            this.btn_AddUser.Cursor = System.Windows.Forms.Cursors.Default;
            this.btn_AddUser.Enabled = false;
            this.btn_AddUser.Location = new System.Drawing.Point(358, 8);
            this.btn_AddUser.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_AddUser.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_AddUser.Name = "btn_AddUser";
            this.btn_AddUser.Size = new System.Drawing.Size(152, 33);
            this.btn_AddUser.TabIndex = 47;
            this.btn_AddUser.Text = "Add/Reset User";
            this.btn_AddUser.Click += new System.EventHandler(this.btn_AddUser_Click);
            // 
            // chkEdit_ClearEOD
            // 
            this.chkEdit_ClearEOD.Location = new System.Drawing.Point(12, 128);
            this.chkEdit_ClearEOD.Name = "chkEdit_ClearEOD";
            this.chkEdit_ClearEOD.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEdit_ClearEOD.Properties.Appearance.Options.UseFont = true;
            this.chkEdit_ClearEOD.Properties.Caption = "Clear EOD";
            this.chkEdit_ClearEOD.Size = new System.Drawing.Size(120, 25);
            this.chkEdit_ClearEOD.TabIndex = 46;
            this.chkEdit_ClearEOD.CheckedChanged += new System.EventHandler(this.chkEdit_ClearEOD_CheckedChanged);
            // 
            // btn_EditUploadClient
            // 
            this.btn_EditUploadClient.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_EditUploadClient.Appearance.Options.UseFont = true;
            this.btn_EditUploadClient.AppearanceDisabled.BackColor = System.Drawing.Color.Gray;
            this.btn_EditUploadClient.AppearanceDisabled.ForeColor = System.Drawing.Color.Gray;
            this.btn_EditUploadClient.AppearanceDisabled.Options.UseBackColor = true;
            this.btn_EditUploadClient.AppearanceDisabled.Options.UseForeColor = true;
            this.btn_EditUploadClient.Cursor = System.Windows.Forms.Cursors.Default;
            this.btn_EditUploadClient.Enabled = false;
            this.btn_EditUploadClient.Location = new System.Drawing.Point(200, 8);
            this.btn_EditUploadClient.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_EditUploadClient.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_EditUploadClient.Name = "btn_EditUploadClient";
            this.btn_EditUploadClient.Size = new System.Drawing.Size(152, 33);
            this.btn_EditUploadClient.TabIndex = 44;
            this.btn_EditUploadClient.Text = "Edit/Upload Clients";
            this.btn_EditUploadClient.Click += new System.EventHandler(this.btn_EditUploadClient_Click);
            // 
            // btn_StartProcess
            // 
            this.btn_StartProcess.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btn_StartProcess.Appearance.Options.UseFont = true;
            this.btn_StartProcess.Enabled = false;
            this.btn_StartProcess.Location = new System.Drawing.Point(358, 159);
            this.btn_StartProcess.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_StartProcess.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_StartProcess.Name = "btn_StartProcess";
            this.btn_StartProcess.Size = new System.Drawing.Size(152, 31);
            this.btn_StartProcess.TabIndex = 42;
            this.btn_StartProcess.Text = "Start Process";
            this.btn_StartProcess.Click += new System.EventHandler(this.btn_StartProcess_Click);
            // 
            // chkEdit_Day1CM
            // 
            this.chkEdit_Day1CM.Location = new System.Drawing.Point(12, 72);
            this.chkEdit_Day1CM.Name = "chkEdit_Day1CM";
            this.chkEdit_Day1CM.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEdit_Day1CM.Properties.Appearance.Options.UseFont = true;
            this.chkEdit_Day1CM.Properties.Caption = "Day1 CM/FO";
            this.chkEdit_Day1CM.Size = new System.Drawing.Size(120, 25);
            this.chkEdit_Day1CM.TabIndex = 41;
            this.chkEdit_Day1CM.ToolTip = "Selects `Day1FO.txt` and `Day1CM.txt` file from `C:\\Prime\\Day1` folder";
            this.chkEdit_Day1CM.CheckedChanged += new System.EventHandler(this.chkEdit_Day1CM_CheckedChanged);
            // 
            // chkEdit_BODPS03
            // 
            this.chkEdit_BODPS03.Location = new System.Drawing.Point(12, 47);
            this.chkEdit_BODPS03.Name = "chkEdit_BODPS03";
            this.chkEdit_BODPS03.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEdit_BODPS03.Properties.Appearance.Options.UseFont = true;
            this.chkEdit_BODPS03.Properties.Caption = "BOD PS03";
            this.chkEdit_BODPS03.Size = new System.Drawing.Size(106, 25);
            this.chkEdit_BODPS03.TabIndex = 38;
            this.chkEdit_BODPS03.CheckedChanged += new System.EventHandler(this.chkEdit_BODPS03_CheckedChanged);
            // 
            // lbl_LatestTrade
            // 
            this.lbl_LatestTrade.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_LatestTrade.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lbl_LatestTrade.Appearance.Options.UseFont = true;
            this.lbl_LatestTrade.Appearance.Options.UseForeColor = true;
            this.lbl_LatestTrade.Location = new System.Drawing.Point(358, 6);
            this.lbl_LatestTrade.Name = "lbl_LatestTrade";
            this.lbl_LatestTrade.Size = new System.Drawing.Size(0, 32);
            toolTipItem2.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            toolTipItem2.Appearance.Options.UseImage = true;
            toolTipItem2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image1")));
            toolTipItem2.Text = "Latest Trade";
            superToolTip2.Items.Add(toolTipItem2);
            this.lbl_LatestTrade.SuperTip = superToolTip2;
            this.lbl_LatestTrade.TabIndex = 35;
            // 
            // notifyIconEngineProcess
            // 
            this.notifyIconEngineProcess.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconEngineProcess.Icon")));
            this.notifyIconEngineProcess.Text = "Engine";
            this.notifyIconEngineProcess.Visible = true;
            this.notifyIconEngineProcess.Click += new System.EventHandler(this.notifyIconEngineProcess_Click);
            // 
            // dockManager_Main
            // 
            this.dockManager_Main.AutoHideContainers.AddRange(new DevExpress.XtraBars.Docking.AutoHideContainer[] {
            this.hideContainerRight});
            this.dockManager_Main.Form = this;
            this.dockManager_Main.HiddenPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockPanel1});
            this.dockManager_Main.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl",
            "DevExpress.XtraBars.Navigation.OfficeNavigationBar",
            "DevExpress.XtraBars.Navigation.TileNavPane",
            "DevExpress.XtraBars.TabFormControl",
            "DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl",
            "DevExpress.XtraBars.ToolbarForm.ToolbarFormControl"});
            // 
            // hideContainerRight
            // 
            this.hideContainerRight.BackColor = System.Drawing.Color.Black;
            this.hideContainerRight.Controls.Add(this.dockPanel_Right);
            this.hideContainerRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.hideContainerRight.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Bold);
            this.hideContainerRight.ForeColor = System.Drawing.Color.Black;
            this.hideContainerRight.Location = new System.Drawing.Point(516, 0);
            this.hideContainerRight.Name = "hideContainerRight";
            this.hideContainerRight.Size = new System.Drawing.Size(31, 432);
            // 
            // dockPanel_Right
            // 
            this.dockPanel_Right.Controls.Add(this.dockPanel_Right_Container);
            this.dockPanel_Right.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.dockPanel_Right.ID = new System.Guid("52b09fd7-c279-40cc-903d-c4f05d3177a9");
            this.dockPanel_Right.Location = new System.Drawing.Point(-4, 0);
            this.dockPanel_Right.Name = "dockPanel_Right";
            this.dockPanel_Right.Options.ShowAutoHideButton = false;
            this.dockPanel_Right.Options.ShowCloseButton = false;
            this.dockPanel_Right.Options.ShowMaximizeButton = false;
            this.dockPanel_Right.Options.ShowMinimizeButton = false;
            this.dockPanel_Right.OriginalSize = new System.Drawing.Size(519, 200);
            this.dockPanel_Right.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Right;
            this.dockPanel_Right.SavedIndex = 0;
            this.dockPanel_Right.Size = new System.Drawing.Size(519, 408);
            this.dockPanel_Right.Text = "Prime Connections";
            this.dockPanel_Right.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
            // 
            // dockPanel_Right_Container
            // 
            this.dockPanel_Right_Container.Controls.Add(this.gc_PrimeConnections);
            this.dockPanel_Right_Container.Location = new System.Drawing.Point(6, 37);
            this.dockPanel_Right_Container.Name = "dockPanel_Right_Container";
            this.dockPanel_Right_Container.Size = new System.Drawing.Size(509, 367);
            this.dockPanel_Right_Container.TabIndex = 0;
            // 
            // gc_PrimeConnections
            // 
            this.gc_PrimeConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_PrimeConnections.Location = new System.Drawing.Point(0, 0);
            this.gc_PrimeConnections.MainView = this.gv_PrimeConnections;
            this.gc_PrimeConnections.Name = "gc_PrimeConnections";
            this.gc_PrimeConnections.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repBtn_Disconnect});
            this.gc_PrimeConnections.Size = new System.Drawing.Size(509, 367);
            this.gc_PrimeConnections.TabIndex = 0;
            this.gc_PrimeConnections.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_PrimeConnections});
            // 
            // gv_PrimeConnections
            // 
            this.gv_PrimeConnections.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.gv_PrimeConnections.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_PrimeConnections.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_PrimeConnections.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_PrimeConnections.Appearance.Row.Options.UseTextOptions = true;
            this.gv_PrimeConnections.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_PrimeConnections.GridControl = this.gc_PrimeConnections;
            this.gv_PrimeConnections.Name = "gv_PrimeConnections";
            this.gv_PrimeConnections.OptionsView.ColumnAutoWidth = false;
            this.gv_PrimeConnections.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_PrimeConnections_CustomRowCellEdit);
            this.gv_PrimeConnections.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_PrimeConnections_ShowingEditor);
            // 
            // repBtn_Disconnect
            // 
            this.repBtn_Disconnect.AutoHeight = false;
            this.repBtn_Disconnect.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "Disconnect", -1, true, true, false, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.repBtn_Disconnect.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.repBtn_Disconnect.Name = "repBtn_Disconnect";
            this.repBtn_Disconnect.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repBtn_Disconnect.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repBtn_Disconnect_ButtonClick);
            // 
            // dockPanel1
            // 
            this.dockPanel1.Controls.Add(this.dockPanel1_Container);
            this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Float;
            this.dockPanel1.DockedAsTabbedDocument = true;
            this.dockPanel1.ID = new System.Guid("e2f70fb1-c1a3-4e95-b4e3-03cd538817bd");
            this.dockPanel1.Location = new System.Drawing.Point(-32768, -32768);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 200);
            this.dockPanel1.SavedDockingContainerId = new System.Guid("9d003c18-8b35-4801-bd23-135eaf56e7cf");
            this.dockPanel1.SavedIndex = 0;
            this.dockPanel1.SavedMdiDocument = true;
            this.dockPanel1.SavedMdiDocumentIndex = 0;
            this.dockPanel1.Size = new System.Drawing.Size(515, 303);
            this.dockPanel1.Text = "dockPanel1";
            this.dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Location = new System.Drawing.Point(0, 0);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(515, 303);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // EngineProcess
            // 
            this.Appearance.BackColor = System.Drawing.Color.Black;
            this.Appearance.BackColor2 = System.Drawing.Color.Black;
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 432);
            this.Controls.Add(this.hideContainerRight);
            this.Controls.Add(this.panel_EngineProcess);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("EngineProcess.IconOptions.Icon")));
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.SkinName = "The Bezier";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(546, 37);
            this.Name = "EngineProcess";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "n.Engine";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EngineProcess_FormClosing);
            this.Load += new System.EventHandler(this.EngineProcess_Load);
            this.Resize += new System.EventHandler(this.EngineProcess_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.lb_ErrorLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panel_EngineProcess)).EndInit();
            this.panel_EngineProcess.ResumeLayout(false);
            this.panel_EngineProcess.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup_MarkToPrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEdit_MarkToClosing.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEdit_ClearEOD.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEdit_Day1CM.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEdit_BODPS03.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager_Main)).EndInit();
            this.hideContainerRight.ResumeLayout(false);
            this.dockPanel_Right.ResumeLayout(false);
            this.dockPanel_Right_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gc_PrimeConnections)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_PrimeConnections)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_Disconnect)).EndInit();
            this.dockPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        

        #endregion
        public DevExpress.XtraEditors.ListBoxControl lb_ErrorLog;
        private DevExpress.XtraEditors.SimpleButton btn_Start;
        private DevExpress.XtraEditors.SimpleButton btn_Stop;
        private DevExpress.XtraEditors.PanelControl panel_EngineProcess;
        private DevExpress.XtraEditors.LabelControl lbl_LatestTrade;
        private System.Windows.Forms.NotifyIcon notifyIconEngineProcess;
        private DevExpress.XtraEditors.CheckEdit chkEdit_BODPS03;
        private DevExpress.XtraEditors.CheckEdit chkEdit_Day1CM;
        private DevExpress.XtraEditors.SimpleButton btn_StartProcess;
        private DevExpress.XtraEditors.SimpleButton btn_EditUploadClient;
        private DevExpress.XtraEditors.CheckEdit chkEdit_ClearEOD;
        private DevExpress.XtraEditors.SimpleButton btn_AddUser;
        private DevExpress.XtraEditors.CheckEdit chkEdit_MarkToClosing;
        private DevExpress.XtraBars.Docking.DockManager dockManager_Main;
        private DevExpress.XtraBars.Docking.AutoHideContainer hideContainerRight;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel_Right;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel_Right_Container;
        private DevExpress.XtraGrid.GridControl gc_PrimeConnections;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_PrimeConnections;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repBtn_Disconnect;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraEditors.SimpleButton btn_EODPositions;
        private DevExpress.XtraEditors.RadioGroup radioGroup_MarkToPrice;
        private System.Windows.Forms.CheckBox checkTerms;
        private System.Windows.Forms.LinkLabel lbl_Terms;
        private DevExpress.XtraEditors.SimpleButton btn_UploadEarlyPayIn;
        private DevExpress.XtraEditors.SimpleButton btn_UploadLedger;
    }
}