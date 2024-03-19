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
            this.lb_ErrorLog = new DevExpress.XtraEditors.ListBoxControl();
            this.timerMarginCalc = new System.Windows.Forms.Timer(this.components);
            this.bgWorker_Margin = new System.ComponentModel.BackgroundWorker();
            this.btn_StopEngine = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Start = new DevExpress.XtraEditors.SimpleButton();
            this.btn_StopEnginee = new DevExpress.XtraEditors.SimpleButton();
            this.panelEngineProcess = new DevExpress.XtraEditors.PanelControl();
            this.lbl_BODPS03File = new DevExpress.XtraEditors.LabelControl();
            this.lbl_IntradayPS03File = new DevExpress.XtraEditors.LabelControl();
            this.lbl_ELM_AdhocFile = new DevExpress.XtraEditors.LabelControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.ckEdit_DownloadExposure = new DevExpress.XtraEditors.CheckEdit();
            this.btnAddUser = new DevExpress.XtraEditors.SimpleButton();
            this.ckEdit_ClearEOD = new DevExpress.XtraEditors.CheckEdit();
            this.btnEditUploadClient = new DevExpress.XtraEditors.SimpleButton();
            this.dtpBhavcopydate = new System.Windows.Forms.DateTimePicker();
            this.btnStartProcess = new DevExpress.XtraEditors.SimpleButton();
            this.ckEdit_Day1CM = new DevExpress.XtraEditors.CheckEdit();
            this.ckEdit_IntradayPS03 = new DevExpress.XtraEditors.CheckEdit();
            this.ckEdit_UploadELM = new DevExpress.XtraEditors.CheckEdit();
            this.ckEdit_BODPS03 = new DevExpress.XtraEditors.CheckEdit();
            this.ckEdit_DownloadBhavcopy = new DevExpress.XtraEditors.CheckEdit();
            this.lbl_LatestTrade = new DevExpress.XtraEditors.LabelControl();
            this.notifyIconEngineProcess = new System.Windows.Forms.NotifyIcon(this.components);
            this.chkBox_MarkToClosing = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.lb_ErrorLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelEngineProcess)).BeginInit();
            this.panelEngineProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_DownloadExposure.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_ClearEOD.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_Day1CM.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_IntradayPS03.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_UploadELM.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_BODPS03.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_DownloadBhavcopy.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBox_MarkToClosing.Properties)).BeginInit();
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
            this.lb_ErrorLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lb_ErrorLog.Location = new System.Drawing.Point(3, 267);
            this.lb_ErrorLog.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lb_ErrorLog.MaximumSize = new System.Drawing.Size(540, 200);
            this.lb_ErrorLog.MinimumSize = new System.Drawing.Size(540, 176);
            this.lb_ErrorLog.Name = "lb_ErrorLog";
            this.lb_ErrorLog.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lb_ErrorLog.ShowFocusRect = false;
            this.lb_ErrorLog.Size = new System.Drawing.Size(540, 176);
            this.lb_ErrorLog.TabIndex = 1;
            // 
            // timerMarginCalc
            // 
            this.timerMarginCalc.Interval = 30000;
            this.timerMarginCalc.Tick += new System.EventHandler(this.timerMarginCalc_Tick);
            // 
            // bgWorker_Margin
            // 
            this.bgWorker_Margin.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_Margin_DoWork);
            // 
            // btn_StopEngine
            // 
            this.btn_StopEngine.Appearance.BackColor = System.Drawing.Color.Gainsboro;
            this.btn_StopEngine.Appearance.BackColor2 = System.Drawing.Color.Gainsboro;
            this.btn_StopEngine.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.btn_StopEngine.Appearance.Options.UseBackColor = true;
            this.btn_StopEngine.Appearance.Options.UseFont = true;
            this.btn_StopEngine.Appearance.Options.UseForeColor = true;
            this.btn_StopEngine.AppearanceDisabled.BackColor = System.Drawing.Color.DarkGray;
            this.btn_StopEngine.AppearanceDisabled.BackColor2 = System.Drawing.Color.DarkGray;
            this.btn_StopEngine.AppearanceDisabled.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.btn_StopEngine.AppearanceDisabled.Options.UseBackColor = true;
            this.btn_StopEngine.AppearanceDisabled.Options.UseFont = true;
            this.btn_StopEngine.AppearanceHovered.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btn_StopEngine.AppearanceHovered.BackColor2 = System.Drawing.Color.LightSteelBlue;
            this.btn_StopEngine.AppearanceHovered.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold);
            this.btn_StopEngine.AppearanceHovered.ForeColor = System.Drawing.Color.Black;
            this.btn_StopEngine.AppearanceHovered.Options.UseBackColor = true;
            this.btn_StopEngine.AppearanceHovered.Options.UseFont = true;
            this.btn_StopEngine.AppearanceHovered.Options.UseForeColor = true;
            this.btn_StopEngine.Location = new System.Drawing.Point(1341, 12);
            this.btn_StopEngine.LookAndFeel.SkinName = "McSkin";
            this.btn_StopEngine.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_StopEngine.Name = "btn_StopEngine";
            this.btn_StopEngine.Size = new System.Drawing.Size(121, 52);
            this.btn_StopEngine.TabIndex = 2;
            this.btn_StopEngine.Text = "Stop";
            this.btn_StopEngine.Click += new System.EventHandler(this.btn_StopEngine_Click);
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
            this.btn_Start.Location = new System.Drawing.Point(6, 8);
            this.btn_Start.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_Start.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(78, 33);
            this.btn_Start.TabIndex = 1;
            this.btn_Start.Text = "Start";
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // btn_StopEnginee
            // 
            this.btn_StopEnginee.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_StopEnginee.Appearance.Options.UseFont = true;
            this.btn_StopEnginee.AppearanceDisabled.BackColor = System.Drawing.Color.Gray;
            this.btn_StopEnginee.AppearanceDisabled.ForeColor = System.Drawing.Color.Gray;
            this.btn_StopEnginee.AppearanceDisabled.Options.UseBackColor = true;
            this.btn_StopEnginee.AppearanceDisabled.Options.UseForeColor = true;
            this.btn_StopEnginee.Cursor = System.Windows.Forms.Cursors.Default;
            this.btn_StopEnginee.Enabled = false;
            this.btn_StopEnginee.Location = new System.Drawing.Point(101, 8);
            this.btn_StopEnginee.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_StopEnginee.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_StopEnginee.Name = "btn_StopEnginee";
            this.btn_StopEnginee.Size = new System.Drawing.Size(87, 33);
            this.btn_StopEnginee.TabIndex = 33;
            this.btn_StopEnginee.Text = "Stop";
            this.btn_StopEnginee.Click += new System.EventHandler(this.btn_StopEngine_Click);
            // 
            // panelEngineProcess
            // 
            this.panelEngineProcess.Appearance.BackColor = System.Drawing.Color.White;
            this.panelEngineProcess.Appearance.BackColor2 = System.Drawing.Color.WhiteSmoke;
            this.panelEngineProcess.Appearance.Options.UseBackColor = true;
            this.panelEngineProcess.Controls.Add(this.chkBox_MarkToClosing);
            this.panelEngineProcess.Controls.Add(this.lbl_BODPS03File);
            this.panelEngineProcess.Controls.Add(this.lbl_IntradayPS03File);
            this.panelEngineProcess.Controls.Add(this.lbl_ELM_AdhocFile);
            this.panelEngineProcess.Controls.Add(this.gridControl1);
            this.panelEngineProcess.Controls.Add(this.ckEdit_DownloadExposure);
            this.panelEngineProcess.Controls.Add(this.btnAddUser);
            this.panelEngineProcess.Controls.Add(this.ckEdit_ClearEOD);
            this.panelEngineProcess.Controls.Add(this.btnEditUploadClient);
            this.panelEngineProcess.Controls.Add(this.dtpBhavcopydate);
            this.panelEngineProcess.Controls.Add(this.btnStartProcess);
            this.panelEngineProcess.Controls.Add(this.ckEdit_Day1CM);
            this.panelEngineProcess.Controls.Add(this.ckEdit_IntradayPS03);
            this.panelEngineProcess.Controls.Add(this.ckEdit_UploadELM);
            this.panelEngineProcess.Controls.Add(this.ckEdit_BODPS03);
            this.panelEngineProcess.Controls.Add(this.ckEdit_DownloadBhavcopy);
            this.panelEngineProcess.Controls.Add(this.lbl_LatestTrade);
            this.panelEngineProcess.Controls.Add(this.lb_ErrorLog);
            this.panelEngineProcess.Controls.Add(this.btn_StopEnginee);
            this.panelEngineProcess.Controls.Add(this.btn_Start);
            this.panelEngineProcess.Controls.Add(this.btn_StopEngine);
            this.panelEngineProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEngineProcess.Location = new System.Drawing.Point(0, 0);
            this.panelEngineProcess.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.panelEngineProcess.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panelEngineProcess.Name = "panelEngineProcess";
            this.panelEngineProcess.Size = new System.Drawing.Size(546, 446);
            this.panelEngineProcess.TabIndex = 5;
            // 
            // lbl_BODPS03File
            // 
            this.lbl_BODPS03File.Location = new System.Drawing.Point(191, 86);
            this.lbl_BODPS03File.Name = "lbl_BODPS03File";
            this.lbl_BODPS03File.Size = new System.Drawing.Size(0, 13);
            this.lbl_BODPS03File.TabIndex = 52;
            this.lbl_BODPS03File.ToolTip = "BOD PS03 file";
            // 
            // lbl_IntradayPS03File
            // 
            this.lbl_IntradayPS03File.Location = new System.Drawing.Point(191, 132);
            this.lbl_IntradayPS03File.Name = "lbl_IntradayPS03File";
            this.lbl_IntradayPS03File.Size = new System.Drawing.Size(0, 13);
            this.lbl_IntradayPS03File.TabIndex = 51;
            this.lbl_IntradayPS03File.ToolTip = "Intraday PS03 File ";
            this.lbl_IntradayPS03File.ToolTipTitle = "File Path";
            // 
            // lbl_ELM_AdhocFile
            // 
            this.lbl_ELM_AdhocFile.Location = new System.Drawing.Point(191, 110);
            this.lbl_ELM_AdhocFile.Name = "lbl_ELM_AdhocFile";
            this.lbl_ELM_AdhocFile.Size = new System.Drawing.Size(0, 13);
            this.lbl_ELM_AdhocFile.TabIndex = 50;
            this.lbl_ELM_AdhocFile.ToolTip = "ELM Adhoc file";
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(437, 106);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(95, 67);
            this.gridControl1.TabIndex = 49;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.gridControl1.Visible = false;
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // ckEdit_DownloadExposure
            // 
            this.ckEdit_DownloadExposure.Location = new System.Drawing.Point(15, 197);
            this.ckEdit_DownloadExposure.Name = "ckEdit_DownloadExposure";
            this.ckEdit_DownloadExposure.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckEdit_DownloadExposure.Properties.Appearance.Options.UseFont = true;
            this.ckEdit_DownloadExposure.Properties.Caption = "Download Exposure";
            this.ckEdit_DownloadExposure.Size = new System.Drawing.Size(142, 21);
            this.ckEdit_DownloadExposure.TabIndex = 48;
            // 
            // btnAddUser
            // 
            this.btnAddUser.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddUser.Appearance.Options.UseFont = true;
            this.btnAddUser.AppearanceDisabled.BackColor = System.Drawing.Color.Gray;
            this.btnAddUser.AppearanceDisabled.ForeColor = System.Drawing.Color.Gray;
            this.btnAddUser.AppearanceDisabled.Options.UseBackColor = true;
            this.btnAddUser.AppearanceDisabled.Options.UseForeColor = true;
            this.btnAddUser.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnAddUser.Enabled = false;
            this.btnAddUser.Location = new System.Drawing.Point(380, 8);
            this.btnAddUser.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btnAddUser.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.Size = new System.Drawing.Size(152, 33);
            this.btnAddUser.TabIndex = 47;
            this.btnAddUser.Text = "Add/Reset User";
            this.btnAddUser.Click += new System.EventHandler(this.BtnAddUser_Click);
            // 
            // ckEdit_ClearEOD
            // 
            this.ckEdit_ClearEOD.Location = new System.Drawing.Point(15, 174);
            this.ckEdit_ClearEOD.Name = "ckEdit_ClearEOD";
            this.ckEdit_ClearEOD.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckEdit_ClearEOD.Properties.Appearance.Options.UseFont = true;
            this.ckEdit_ClearEOD.Properties.Caption = "Clear EOD";
            this.ckEdit_ClearEOD.Size = new System.Drawing.Size(120, 21);
            this.ckEdit_ClearEOD.TabIndex = 46;
            this.ckEdit_ClearEOD.CheckedChanged += new System.EventHandler(this.ckEdit_ClearEOD_CheckedChanged);
            // 
            // btnEditUploadClient
            // 
            this.btnEditUploadClient.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditUploadClient.Appearance.Options.UseFont = true;
            this.btnEditUploadClient.AppearanceDisabled.BackColor = System.Drawing.Color.Gray;
            this.btnEditUploadClient.AppearanceDisabled.ForeColor = System.Drawing.Color.Gray;
            this.btnEditUploadClient.AppearanceDisabled.Options.UseBackColor = true;
            this.btnEditUploadClient.AppearanceDisabled.Options.UseForeColor = true;
            this.btnEditUploadClient.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnEditUploadClient.Enabled = false;
            this.btnEditUploadClient.Location = new System.Drawing.Point(206, 8);
            this.btnEditUploadClient.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btnEditUploadClient.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnEditUploadClient.Name = "btnEditUploadClient";
            this.btnEditUploadClient.Size = new System.Drawing.Size(152, 33);
            this.btnEditUploadClient.TabIndex = 44;
            this.btnEditUploadClient.Text = "Edit/Upload Clients";
            this.btnEditUploadClient.Click += new System.EventHandler(this.btnEditUploadClient_Click);
            // 
            // dtpBhavcopydate
            // 
            this.dtpBhavcopydate.Enabled = false;
            this.dtpBhavcopydate.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpBhavcopydate.Location = new System.Drawing.Point(191, 54);
            this.dtpBhavcopydate.Name = "dtpBhavcopydate";
            this.dtpBhavcopydate.Size = new System.Drawing.Size(200, 25);
            this.dtpBhavcopydate.TabIndex = 43;
            // 
            // btnStartProcess
            // 
            this.btnStartProcess.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnStartProcess.Appearance.Options.UseFont = true;
            this.btnStartProcess.Enabled = false;
            this.btnStartProcess.Location = new System.Drawing.Point(12, 223);
            this.btnStartProcess.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btnStartProcess.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnStartProcess.Name = "btnStartProcess";
            this.btnStartProcess.Size = new System.Drawing.Size(123, 31);
            this.btnStartProcess.TabIndex = 42;
            this.btnStartProcess.Text = "Start Process";
            this.btnStartProcess.Click += new System.EventHandler(this.btnStartProcess_Click);
            // 
            // ckEdit_Day1CM
            // 
            this.ckEdit_Day1CM.Location = new System.Drawing.Point(15, 150);
            this.ckEdit_Day1CM.Name = "ckEdit_Day1CM";
            this.ckEdit_Day1CM.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckEdit_Day1CM.Properties.Appearance.Options.UseFont = true;
            this.ckEdit_Day1CM.Properties.Caption = "Day1 CM/FO";
            this.ckEdit_Day1CM.Size = new System.Drawing.Size(120, 21);
            this.ckEdit_Day1CM.TabIndex = 41;
            this.ckEdit_Day1CM.ToolTip = "Selects `Day1FO.txt` and `Day1CM.txt` file from `C:\\Prime\\Day1` folder";
            // 
            // ckEdit_IntradayPS03
            // 
            this.ckEdit_IntradayPS03.Location = new System.Drawing.Point(15, 127);
            this.ckEdit_IntradayPS03.Name = "ckEdit_IntradayPS03";
            this.ckEdit_IntradayPS03.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckEdit_IntradayPS03.Properties.Appearance.Options.UseFont = true;
            this.ckEdit_IntradayPS03.Properties.Caption = "Intraday PS03";
            this.ckEdit_IntradayPS03.Size = new System.Drawing.Size(118, 21);
            this.ckEdit_IntradayPS03.TabIndex = 40;
            this.ckEdit_IntradayPS03.CheckedChanged += new System.EventHandler(this.ckEdit_IntradayPS03_CheckedChanged);
            // 
            // ckEdit_UploadELM
            // 
            this.ckEdit_UploadELM.Location = new System.Drawing.Point(15, 104);
            this.ckEdit_UploadELM.Name = "ckEdit_UploadELM";
            this.ckEdit_UploadELM.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckEdit_UploadELM.Properties.Appearance.Options.UseFont = true;
            this.ckEdit_UploadELM.Properties.Caption = "Upload ELM & Adhoc";
            this.ckEdit_UploadELM.Size = new System.Drawing.Size(142, 21);
            this.ckEdit_UploadELM.TabIndex = 39;
            this.ckEdit_UploadELM.CheckedChanged += new System.EventHandler(this.ckEdit_UploadELM_CheckedChanged);
            // 
            // ckEdit_BODPS03
            // 
            this.ckEdit_BODPS03.Location = new System.Drawing.Point(15, 81);
            this.ckEdit_BODPS03.Name = "ckEdit_BODPS03";
            this.ckEdit_BODPS03.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckEdit_BODPS03.Properties.Appearance.Options.UseFont = true;
            this.ckEdit_BODPS03.Properties.Caption = "BOD PS03";
            this.ckEdit_BODPS03.Size = new System.Drawing.Size(75, 21);
            this.ckEdit_BODPS03.TabIndex = 38;
            this.ckEdit_BODPS03.CheckedChanged += new System.EventHandler(this.ckEdit_BODPS03_CheckedChanged);
            // 
            // ckEdit_DownloadBhavcopy
            // 
            this.ckEdit_DownloadBhavcopy.Location = new System.Drawing.Point(15, 58);
            this.ckEdit_DownloadBhavcopy.Name = "ckEdit_DownloadBhavcopy";
            this.ckEdit_DownloadBhavcopy.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckEdit_DownloadBhavcopy.Properties.Appearance.Options.UseFont = true;
            this.ckEdit_DownloadBhavcopy.Properties.Caption = "Download bhavcopy";
            this.ckEdit_DownloadBhavcopy.Size = new System.Drawing.Size(154, 21);
            this.ckEdit_DownloadBhavcopy.TabIndex = 37;
            this.ckEdit_DownloadBhavcopy.CheckedChanged += new System.EventHandler(this.ckEdit_DownloadBhavcopy_CheckedChanged);
            // 
            // lbl_LatestTrade
            // 
            this.lbl_LatestTrade.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_LatestTrade.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lbl_LatestTrade.Appearance.Options.UseFont = true;
            this.lbl_LatestTrade.Appearance.Options.UseForeColor = true;
            this.lbl_LatestTrade.Location = new System.Drawing.Point(358, 6);
            this.lbl_LatestTrade.Name = "lbl_LatestTrade";
            this.lbl_LatestTrade.Size = new System.Drawing.Size(0, 25);
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
            // chkBox_MarkToClosing
            // 
            this.chkBox_MarkToClosing.Location = new System.Drawing.Point(141, 152);
            this.chkBox_MarkToClosing.Name = "chkBox_MarkToClosing";
            this.chkBox_MarkToClosing.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkBox_MarkToClosing.Properties.Appearance.Options.UseFont = true;
            this.chkBox_MarkToClosing.Properties.Caption = "Mark to Closing";
            this.chkBox_MarkToClosing.Size = new System.Drawing.Size(120, 21);
            this.chkBox_MarkToClosing.TabIndex = 53;
            this.chkBox_MarkToClosing.ToolTip = "Selects `Day1FO.txt` and `Day1CM.txt` file from `C:\\Prime\\Day1` folder";
            // 
            // EngineProcess
            // 
            this.Appearance.BackColor = System.Drawing.Color.Black;
            this.Appearance.BackColor2 = System.Drawing.Color.Black;
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 446);
            this.Controls.Add(this.panelEngineProcess);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("EngineProcess.IconOptions.Icon")));
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.SkinName = "The Bezier";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(546, 30);
            this.Name = "EngineProcess";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "n.engine";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EngineProcess_FormClosing);
            this.Load += new System.EventHandler(this.EngineProcess_Load);
            this.Resize += new System.EventHandler(this.EngineProcess_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.lb_ErrorLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelEngineProcess)).EndInit();
            this.panelEngineProcess.ResumeLayout(false);
            this.panelEngineProcess.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_DownloadExposure.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_ClearEOD.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_Day1CM.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_IntradayPS03.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_UploadELM.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_BODPS03.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckEdit_DownloadBhavcopy.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBox_MarkToClosing.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        

        #endregion
        private System.Windows.Forms.Timer timerMarginCalc;
        private System.ComponentModel.BackgroundWorker bgWorker_Margin;
        public DevExpress.XtraEditors.ListBoxControl lb_ErrorLog;
        private DevExpress.XtraEditors.SimpleButton btn_StopEngine;
        private DevExpress.XtraEditors.SimpleButton btn_Start;
        private DevExpress.XtraEditors.SimpleButton btn_StopEnginee;
        private DevExpress.XtraEditors.PanelControl panelEngineProcess;
        private DevExpress.XtraEditors.LabelControl lbl_LatestTrade;
        private System.Windows.Forms.NotifyIcon notifyIconEngineProcess;
        private DevExpress.XtraEditors.CheckEdit ckEdit_UploadELM;
        private DevExpress.XtraEditors.CheckEdit ckEdit_BODPS03;
        private DevExpress.XtraEditors.CheckEdit ckEdit_DownloadBhavcopy;
        private DevExpress.XtraEditors.CheckEdit ckEdit_Day1CM;
        private DevExpress.XtraEditors.CheckEdit ckEdit_IntradayPS03;
        private DevExpress.XtraEditors.SimpleButton btnStartProcess;
        private System.Windows.Forms.DateTimePicker dtpBhavcopydate;
        private DevExpress.XtraEditors.SimpleButton btnEditUploadClient;
        private DevExpress.XtraEditors.CheckEdit ckEdit_ClearEOD;
        private DevExpress.XtraEditors.SimpleButton btnAddUser;
        private DevExpress.XtraEditors.CheckEdit ckEdit_DownloadExposure;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.LabelControl lbl_ELM_AdhocFile;
        private DevExpress.XtraEditors.LabelControl lbl_IntradayPS03File;
        private DevExpress.XtraEditors.LabelControl lbl_BODPS03File;
        private DevExpress.XtraEditors.CheckEdit chkBox_MarkToClosing;
    }
}