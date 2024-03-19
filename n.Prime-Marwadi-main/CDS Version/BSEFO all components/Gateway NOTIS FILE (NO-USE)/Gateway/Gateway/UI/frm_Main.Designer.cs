namespace Gateway
{
    partial class frm_Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_Main));
            this.notifyIconGateway = new System.Windows.Forms.NotifyIcon(this.components);
            this.panel_Main = new DevExpress.XtraEditors.PanelControl();
            this.listBox_Messages = new DevExpress.XtraEditors.ListBoxControl();
            this.panel_Top = new DevExpress.XtraEditors.PanelControl();
            this.ind_TradeFileCDAvailable = new DevExpress.XtraEditors.LabelControl();
            this.ind_TradeFileCD = new DevExpress.XtraEditors.LabelControl();
            this.btn_ReloadAndRecompute = new DevExpress.XtraEditors.SimpleButton();
            this.ind_TradeFileBSECMAvailable = new DevExpress.XtraEditors.LabelControl();
            this.ind_TradeFileBSECM = new DevExpress.XtraEditors.LabelControl();
            this.ind_TradeFileCMAvailable = new DevExpress.XtraEditors.LabelControl();
            this.ind_TradeFileCM = new DevExpress.XtraEditors.LabelControl();
            this.ind_TradeFileFOAvailable = new DevExpress.XtraEditors.LabelControl();
            this.ind_TradeFileFO = new DevExpress.XtraEditors.LabelControl();
            this.progressPanel_Start = new DevExpress.XtraWaitForm.ProgressPanel();
            this.btn_StartPick = new DevExpress.XtraEditors.SimpleButton();
            this.btn_UploadTokens = new DevExpress.XtraEditors.SimpleButton();
            this.panel_TAndC = new DevExpress.XtraEditors.PanelControl();
            this.linkLbl_TAndC = new System.Windows.Forms.LinkLabel();
            this.chk_TAndC = new DevExpress.XtraEditors.CheckEdit();
            this.ind_TradeFileBSEFOAvailable = new DevExpress.XtraEditors.LabelControl();
            this.ind_TradeFileBSEFO = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panel_Main)).BeginInit();
            this.panel_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listBox_Messages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panel_Top)).BeginInit();
            this.panel_Top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panel_TAndC)).BeginInit();
            this.panel_TAndC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chk_TAndC.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIconGateway
            // 
            this.notifyIconGateway.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconGateway.Icon")));
            this.notifyIconGateway.Text = "Gateway";
            this.notifyIconGateway.Visible = true;
            this.notifyIconGateway.Click += new System.EventHandler(this.notifyIconGateway_Click);
            // 
            // panel_Main
            // 
            this.panel_Main.Appearance.BackColor = System.Drawing.Color.White;
            this.panel_Main.Appearance.Options.UseBackColor = true;
            this.panel_Main.Controls.Add(this.listBox_Messages);
            this.panel_Main.Controls.Add(this.panel_Top);
            this.panel_Main.Controls.Add(this.panel_TAndC);
            this.panel_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Main.Location = new System.Drawing.Point(0, 0);
            this.panel_Main.LookAndFeel.SkinMaskColor = System.Drawing.Color.White;
            this.panel_Main.LookAndFeel.SkinMaskColor2 = System.Drawing.Color.White;
            this.panel_Main.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.panel_Main.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panel_Main.Margin = new System.Windows.Forms.Padding(4);
            this.panel_Main.Name = "panel_Main";
            this.panel_Main.Size = new System.Drawing.Size(348, 500);
            this.panel_Main.TabIndex = 8;
            // 
            // listBox_Messages
            // 
            this.listBox_Messages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Messages.HorizontalScrollbar = true;
            this.listBox_Messages.Location = new System.Drawing.Point(3, 146);
            this.listBox_Messages.Margin = new System.Windows.Forms.Padding(4);
            this.listBox_Messages.Name = "listBox_Messages";
            this.listBox_Messages.Size = new System.Drawing.Size(342, 317);
            this.listBox_Messages.TabIndex = 20;
            // 
            // panel_Top
            // 
            this.panel_Top.Controls.Add(this.ind_TradeFileBSEFO);
            this.panel_Top.Controls.Add(this.ind_TradeFileBSEFOAvailable);
            this.panel_Top.Controls.Add(this.ind_TradeFileCDAvailable);
            this.panel_Top.Controls.Add(this.ind_TradeFileCD);
            this.panel_Top.Controls.Add(this.btn_ReloadAndRecompute);
            this.panel_Top.Controls.Add(this.ind_TradeFileBSECMAvailable);
            this.panel_Top.Controls.Add(this.ind_TradeFileBSECM);
            this.panel_Top.Controls.Add(this.ind_TradeFileCMAvailable);
            this.panel_Top.Controls.Add(this.ind_TradeFileCM);
            this.panel_Top.Controls.Add(this.ind_TradeFileFOAvailable);
            this.panel_Top.Controls.Add(this.ind_TradeFileFO);
            this.panel_Top.Controls.Add(this.progressPanel_Start);
            this.panel_Top.Controls.Add(this.btn_StartPick);
            this.panel_Top.Controls.Add(this.btn_UploadTokens);
            this.panel_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Top.Location = new System.Drawing.Point(3, 3);
            this.panel_Top.Margin = new System.Windows.Forms.Padding(4);
            this.panel_Top.Name = "panel_Top";
            this.panel_Top.Size = new System.Drawing.Size(342, 143);
            this.panel_Top.TabIndex = 19;
            // 
            // ind_TradeFileCDAvailable
            // 
            this.ind_TradeFileCDAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileCDAvailable.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileCDAvailable.Appearance.Image")));
            this.ind_TradeFileCDAvailable.Appearance.Options.UseImage = true;
            this.ind_TradeFileCDAvailable.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileCDAvailable.Location = new System.Drawing.Point(300, 78);
            this.ind_TradeFileCDAvailable.Margin = new System.Windows.Forms.Padding(4);
            this.ind_TradeFileCDAvailable.Name = "ind_TradeFileCDAvailable";
            this.ind_TradeFileCDAvailable.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileCDAvailable.TabIndex = 34;
            this.ind_TradeFileCDAvailable.Visible = false;
            // 
            // ind_TradeFileCD
            // 
            this.ind_TradeFileCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileCD.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileCD.Appearance.Image")));
            this.ind_TradeFileCD.Appearance.Options.UseImage = true;
            this.ind_TradeFileCD.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileCD.Location = new System.Drawing.Point(300, 78);
            this.ind_TradeFileCD.Margin = new System.Windows.Forms.Padding(4);
            this.ind_TradeFileCD.Name = "ind_TradeFileCD";
            this.ind_TradeFileCD.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileCD.TabIndex = 33;
            // 
            // btn_ReloadAndRecompute
            // 
            this.btn_ReloadAndRecompute.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btn_ReloadAndRecompute.Appearance.Options.UseFont = true;
            this.btn_ReloadAndRecompute.Enabled = false;
            this.btn_ReloadAndRecompute.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_ReloadAndRecompute.ImageOptions.Image")));
            this.btn_ReloadAndRecompute.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btn_ReloadAndRecompute.Location = new System.Drawing.Point(8, 8);
            this.btn_ReloadAndRecompute.LookAndFeel.SkinName = "DevExpress Style";
            this.btn_ReloadAndRecompute.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_ReloadAndRecompute.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.btn_ReloadAndRecompute.Name = "btn_ReloadAndRecompute";
            this.btn_ReloadAndRecompute.Size = new System.Drawing.Size(56, 47);
            this.btn_ReloadAndRecompute.TabIndex = 32;
            this.btn_ReloadAndRecompute.ToolTip = "Reload and Recompute";
            this.btn_ReloadAndRecompute.Click += new System.EventHandler(this.btn_ReloadAndRecompute_Click);
            // 
            // ind_TradeFileBSECMAvailable
            // 
            this.ind_TradeFileBSECMAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileBSECMAvailable.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileBSECMAvailable.Appearance.Image")));
            this.ind_TradeFileBSECMAvailable.Appearance.Options.UseImage = true;
            this.ind_TradeFileBSECMAvailable.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileBSECMAvailable.Location = new System.Drawing.Point(276, 78);
            this.ind_TradeFileBSECMAvailable.Margin = new System.Windows.Forms.Padding(4);
            this.ind_TradeFileBSECMAvailable.Name = "ind_TradeFileBSECMAvailable";
            this.ind_TradeFileBSECMAvailable.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileBSECMAvailable.TabIndex = 31;
            this.ind_TradeFileBSECMAvailable.Visible = false;
            // 
            // ind_TradeFileBSECM
            // 
            this.ind_TradeFileBSECM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileBSECM.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileBSECM.Appearance.Image")));
            this.ind_TradeFileBSECM.Appearance.Options.UseImage = true;
            this.ind_TradeFileBSECM.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileBSECM.Location = new System.Drawing.Point(276, 78);
            this.ind_TradeFileBSECM.Margin = new System.Windows.Forms.Padding(4);
            this.ind_TradeFileBSECM.Name = "ind_TradeFileBSECM";
            this.ind_TradeFileBSECM.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileBSECM.TabIndex = 30;
            // 
            // ind_TradeFileCMAvailable
            // 
            this.ind_TradeFileCMAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileCMAvailable.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileCMAvailable.Appearance.Image")));
            this.ind_TradeFileCMAvailable.Appearance.Options.UseImage = true;
            this.ind_TradeFileCMAvailable.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileCMAvailable.Location = new System.Drawing.Point(300, 103);
            this.ind_TradeFileCMAvailable.Margin = new System.Windows.Forms.Padding(4);
            this.ind_TradeFileCMAvailable.Name = "ind_TradeFileCMAvailable";
            this.ind_TradeFileCMAvailable.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileCMAvailable.TabIndex = 29;
            this.ind_TradeFileCMAvailable.Visible = false;
            // 
            // ind_TradeFileCM
            // 
            this.ind_TradeFileCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileCM.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileCM.Appearance.Image")));
            this.ind_TradeFileCM.Appearance.Options.UseImage = true;
            this.ind_TradeFileCM.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileCM.Location = new System.Drawing.Point(300, 103);
            this.ind_TradeFileCM.Margin = new System.Windows.Forms.Padding(4);
            this.ind_TradeFileCM.Name = "ind_TradeFileCM";
            this.ind_TradeFileCM.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileCM.TabIndex = 28;
            // 
            // ind_TradeFileFOAvailable
            // 
            this.ind_TradeFileFOAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileFOAvailable.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileFOAvailable.Appearance.Image")));
            this.ind_TradeFileFOAvailable.Appearance.Options.UseImage = true;
            this.ind_TradeFileFOAvailable.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileFOAvailable.Location = new System.Drawing.Point(276, 103);
            this.ind_TradeFileFOAvailable.Margin = new System.Windows.Forms.Padding(4);
            this.ind_TradeFileFOAvailable.Name = "ind_TradeFileFOAvailable";
            this.ind_TradeFileFOAvailable.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileFOAvailable.TabIndex = 27;
            this.ind_TradeFileFOAvailable.Visible = false;
            // 
            // ind_TradeFileFO
            // 
            this.ind_TradeFileFO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileFO.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileFO.Appearance.Image")));
            this.ind_TradeFileFO.Appearance.Options.UseImage = true;
            this.ind_TradeFileFO.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileFO.Location = new System.Drawing.Point(276, 103);
            this.ind_TradeFileFO.Margin = new System.Windows.Forms.Padding(4);
            this.ind_TradeFileFO.Name = "ind_TradeFileFO";
            this.ind_TradeFileFO.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileFO.TabIndex = 26;
            // 
            // progressPanel_Start
            // 
            this.progressPanel_Start.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.progressPanel_Start.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressPanel_Start.Appearance.ForeColor = System.Drawing.Color.Black;
            this.progressPanel_Start.Appearance.Options.UseBackColor = true;
            this.progressPanel_Start.Appearance.Options.UseFont = true;
            this.progressPanel_Start.Appearance.Options.UseForeColor = true;
            this.progressPanel_Start.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressPanel_Start.AppearanceCaption.ForeColor = System.Drawing.Color.Black;
            this.progressPanel_Start.AppearanceCaption.Options.UseFont = true;
            this.progressPanel_Start.AppearanceCaption.Options.UseForeColor = true;
            this.progressPanel_Start.Caption = "Process Started....";
            this.progressPanel_Start.Description = "";
            this.progressPanel_Start.Location = new System.Drawing.Point(15, 61);
            this.progressPanel_Start.LookAndFeel.SkinName = "Visual Studio 2013 Blue";
            this.progressPanel_Start.LookAndFeel.UseDefaultLookAndFeel = false;
            this.progressPanel_Start.Margin = new System.Windows.Forms.Padding(4);
            this.progressPanel_Start.Name = "progressPanel_Start";
            this.progressPanel_Start.Size = new System.Drawing.Size(203, 81);
            this.progressPanel_Start.TabIndex = 9;
            this.progressPanel_Start.Visible = false;
            // 
            // btn_StartPick
            // 
            this.btn_StartPick.Appearance.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btn_StartPick.Appearance.Options.UseFont = true;
            this.btn_StartPick.Enabled = false;
            this.btn_StartPick.Location = new System.Drawing.Point(231, 8);
            this.btn_StartPick.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_StartPick.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_StartPick.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.btn_StartPick.Name = "btn_StartPick";
            this.btn_StartPick.Size = new System.Drawing.Size(102, 47);
            this.btn_StartPick.TabIndex = 3;
            this.btn_StartPick.Text = "Start";
            this.btn_StartPick.Click += new System.EventHandler(this.btn_StartPick_Click);
            // 
            // btn_UploadTokens
            // 
            this.btn_UploadTokens.Appearance.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btn_UploadTokens.Appearance.Options.UseFont = true;
            this.btn_UploadTokens.Enabled = false;
            this.btn_UploadTokens.Location = new System.Drawing.Point(68, 8);
            this.btn_UploadTokens.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_UploadTokens.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_UploadTokens.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.btn_UploadTokens.Name = "btn_UploadTokens";
            this.btn_UploadTokens.Size = new System.Drawing.Size(156, 47);
            this.btn_UploadTokens.TabIndex = 10;
            this.btn_UploadTokens.Text = "Upload Tokens";
            this.btn_UploadTokens.Click += new System.EventHandler(this.btn_UploadTokens_Click);
            // 
            // panel_TAndC
            // 
            this.panel_TAndC.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panel_TAndC.Controls.Add(this.linkLbl_TAndC);
            this.panel_TAndC.Controls.Add(this.chk_TAndC);
            this.panel_TAndC.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_TAndC.Location = new System.Drawing.Point(3, 463);
            this.panel_TAndC.Margin = new System.Windows.Forms.Padding(4);
            this.panel_TAndC.Name = "panel_TAndC";
            this.panel_TAndC.Size = new System.Drawing.Size(342, 34);
            this.panel_TAndC.TabIndex = 17;
            // 
            // linkLbl_TAndC
            // 
            this.linkLbl_TAndC.AutoSize = true;
            this.linkLbl_TAndC.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLbl_TAndC.LinkColor = System.Drawing.Color.Black;
            this.linkLbl_TAndC.Location = new System.Drawing.Point(22, 12);
            this.linkLbl_TAndC.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLbl_TAndC.Name = "linkLbl_TAndC";
            this.linkLbl_TAndC.Size = new System.Drawing.Size(225, 19);
            this.linkLbl_TAndC.TabIndex = 4;
            this.linkLbl_TAndC.TabStop = true;
            this.linkLbl_TAndC.Text = "I agree to the terms and conditions";
            this.linkLbl_TAndC.VisitedLinkColor = System.Drawing.Color.RoyalBlue;
            this.linkLbl_TAndC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLbl_TAndC_LinkClicked);
            // 
            // chk_TAndC
            // 
            this.chk_TAndC.Location = new System.Drawing.Point(5, 10);
            this.chk_TAndC.Margin = new System.Windows.Forms.Padding(4);
            this.chk_TAndC.Name = "chk_TAndC";
            this.chk_TAndC.Properties.Caption = "";
            this.chk_TAndC.Size = new System.Drawing.Size(20, 20);
            this.chk_TAndC.TabIndex = 5;
            this.chk_TAndC.CheckedChanged += new System.EventHandler(this.chk_TAndC_CheckedChanged);
            // 
            // ind_TradeFileBSEFOAvailable
            // 
            this.ind_TradeFileBSEFOAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileBSEFOAvailable.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("labelControl1.Appearance.Image1")));
            this.ind_TradeFileBSEFOAvailable.Appearance.Options.UseImage = true;
            this.ind_TradeFileBSEFOAvailable.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileBSEFOAvailable.Location = new System.Drawing.Point(252, 90);
            this.ind_TradeFileBSEFOAvailable.Margin = new System.Windows.Forms.Padding(4);
            this.ind_TradeFileBSEFOAvailable.Name = "ind_TradeFileBSEFOAvailable";
            this.ind_TradeFileBSEFOAvailable.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileBSEFOAvailable.TabIndex = 31;
            this.ind_TradeFileBSEFOAvailable.Visible = false;
            // 
            // ind_TradeFileBSEFO
            // 
            this.ind_TradeFileBSEFO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileBSEFO.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("labelControl1.Appearance.Image")));
            this.ind_TradeFileBSEFO.Appearance.Options.UseImage = true;
            this.ind_TradeFileBSEFO.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileBSEFO.Location = new System.Drawing.Point(252, 90);
            this.ind_TradeFileBSEFO.Margin = new System.Windows.Forms.Padding(4);
            this.ind_TradeFileBSEFO.Name = "ind_TradeFileBSEFO";
            this.ind_TradeFileBSEFO.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileBSEFO.TabIndex = 30;
            // 
            // frm_Main
            // 
            this.Appearance.BackColor = System.Drawing.Color.Black;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 500);
            this.Controls.Add(this.panel_Main);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frm_Main.IconOptions.Icon")));
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.SkinName = "The Bezier";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "frm_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "n.Gateway";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TradePicker_FormClosing);
            this.Load += new System.EventHandler(this.TradePicker_Load);
            this.Resize += new System.EventHandler(this.TradePicker_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.panel_Main)).EndInit();
            this.panel_Main.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listBox_Messages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panel_Top)).EndInit();
            this.panel_Top.ResumeLayout(false);
            this.panel_Top.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panel_TAndC)).EndInit();
            this.panel_TAndC.ResumeLayout(false);
            this.panel_TAndC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chk_TAndC.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.NotifyIcon notifyIconGateway;
        private DevExpress.XtraEditors.PanelControl panel_Main;
        private DevExpress.XtraEditors.PanelControl panel_TAndC;
        private System.Windows.Forms.LinkLabel linkLbl_TAndC;
        private DevExpress.XtraEditors.CheckEdit chk_TAndC;
        private DevExpress.XtraEditors.PanelControl panel_Top;
        private DevExpress.XtraWaitForm.ProgressPanel progressPanel_Start;
        private DevExpress.XtraEditors.SimpleButton btn_StartPick;
        private DevExpress.XtraEditors.SimpleButton btn_UploadTokens;
        private DevExpress.XtraEditors.ListBoxControl listBox_Messages;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileFOAvailable;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileFO;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileCMAvailable;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileCM;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileBSECMAvailable;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileBSECM;
        private DevExpress.XtraEditors.SimpleButton btn_ReloadAndRecompute;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileCDAvailable;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileCD;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileBSEFOAvailable;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileBSEFO;
    }
}