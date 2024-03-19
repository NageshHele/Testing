namespace Gateway
{
    partial class TradePicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TradePicker));
            this.notifyIconGateway = new System.Windows.Forms.NotifyIcon(this.components);
            this.panel_Main = new DevExpress.XtraEditors.PanelControl();
            this.listBox_Messages = new DevExpress.XtraEditors.ListBoxControl();
            this.panel_Top = new DevExpress.XtraEditors.PanelControl();
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
            this.btn_ReloadAndRecompute = new DevExpress.XtraEditors.SimpleButton();
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
            this.panel_Main.Name = "panel_Main";
            this.panel_Main.Size = new System.Drawing.Size(298, 382);
            this.panel_Main.TabIndex = 8;
            // 
            // listBox_Messages
            // 
            this.listBox_Messages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Messages.HorizontalScrollbar = true;
            this.listBox_Messages.Location = new System.Drawing.Point(3, 112);
            this.listBox_Messages.Name = "listBox_Messages";
            this.listBox_Messages.Size = new System.Drawing.Size(292, 241);
            this.listBox_Messages.TabIndex = 20;
            // 
            // panel_Top
            // 
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
            this.panel_Top.Name = "panel_Top";
            this.panel_Top.Size = new System.Drawing.Size(292, 109);
            this.panel_Top.TabIndex = 19;
            // 
            // ind_TradeFileBSECMAvailable
            // 
            this.ind_TradeFileBSECMAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileBSECMAvailable.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileBSECMAvailable.Appearance.Image")));
            this.ind_TradeFileBSECMAvailable.Appearance.Options.UseImage = true;
            this.ind_TradeFileBSECMAvailable.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileBSECMAvailable.Location = new System.Drawing.Point(245, 66);
            this.ind_TradeFileBSECMAvailable.Name = "ind_TradeFileBSECMAvailable";
            this.ind_TradeFileBSECMAvailable.Size = new System.Drawing.Size(16, 16);
            this.ind_TradeFileBSECMAvailable.TabIndex = 31;
            this.ind_TradeFileBSECMAvailable.Visible = false;
            // 
            // ind_TradeFileBSECM
            // 
            this.ind_TradeFileBSECM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileBSECM.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileBSECM.Appearance.Image")));
            this.ind_TradeFileBSECM.Appearance.Options.UseImage = true;
            this.ind_TradeFileBSECM.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileBSECM.Location = new System.Drawing.Point(245, 66);
            this.ind_TradeFileBSECM.Name = "ind_TradeFileBSECM";
            this.ind_TradeFileBSECM.Size = new System.Drawing.Size(16, 16);
            this.ind_TradeFileBSECM.TabIndex = 30;
            // 
            // ind_TradeFileCMAvailable
            // 
            this.ind_TradeFileCMAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileCMAvailable.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileCMAvailable.Appearance.Image")));
            this.ind_TradeFileCMAvailable.Appearance.Options.UseImage = true;
            this.ind_TradeFileCMAvailable.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileCMAvailable.Location = new System.Drawing.Point(267, 77);
            this.ind_TradeFileCMAvailable.Name = "ind_TradeFileCMAvailable";
            this.ind_TradeFileCMAvailable.Size = new System.Drawing.Size(16, 16);
            this.ind_TradeFileCMAvailable.TabIndex = 29;
            this.ind_TradeFileCMAvailable.Visible = false;
            // 
            // ind_TradeFileCM
            // 
            this.ind_TradeFileCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileCM.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileCM.Appearance.Image")));
            this.ind_TradeFileCM.Appearance.Options.UseImage = true;
            this.ind_TradeFileCM.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileCM.Location = new System.Drawing.Point(267, 77);
            this.ind_TradeFileCM.Name = "ind_TradeFileCM";
            this.ind_TradeFileCM.Size = new System.Drawing.Size(16, 16);
            this.ind_TradeFileCM.TabIndex = 28;
            // 
            // ind_TradeFileFOAvailable
            // 
            this.ind_TradeFileFOAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileFOAvailable.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileFOAvailable.Appearance.Image")));
            this.ind_TradeFileFOAvailable.Appearance.Options.UseImage = true;
            this.ind_TradeFileFOAvailable.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileFOAvailable.Location = new System.Drawing.Point(267, 55);
            this.ind_TradeFileFOAvailable.Name = "ind_TradeFileFOAvailable";
            this.ind_TradeFileFOAvailable.Size = new System.Drawing.Size(16, 16);
            this.ind_TradeFileFOAvailable.TabIndex = 27;
            this.ind_TradeFileFOAvailable.Visible = false;
            // 
            // ind_TradeFileFO
            // 
            this.ind_TradeFileFO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileFO.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileFO.Appearance.Image")));
            this.ind_TradeFileFO.Appearance.Options.UseImage = true;
            this.ind_TradeFileFO.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileFO.Location = new System.Drawing.Point(267, 55);
            this.ind_TradeFileFO.Name = "ind_TradeFileFO";
            this.ind_TradeFileFO.Size = new System.Drawing.Size(16, 16);
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
            this.progressPanel_Start.Location = new System.Drawing.Point(13, 47);
            this.progressPanel_Start.LookAndFeel.SkinName = "Visual Studio 2013 Blue";
            this.progressPanel_Start.LookAndFeel.UseDefaultLookAndFeel = false;
            this.progressPanel_Start.Name = "progressPanel_Start";
            this.progressPanel_Start.Size = new System.Drawing.Size(174, 62);
            this.progressPanel_Start.TabIndex = 9;
            this.progressPanel_Start.Visible = false;
            // 
            // btn_StartPick
            // 
            this.btn_StartPick.Appearance.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btn_StartPick.Appearance.Options.UseFont = true;
            this.btn_StartPick.Enabled = false;
            this.btn_StartPick.Location = new System.Drawing.Point(198, 6);
            this.btn_StartPick.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_StartPick.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_StartPick.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_StartPick.Name = "btn_StartPick";
            this.btn_StartPick.Size = new System.Drawing.Size(87, 36);
            this.btn_StartPick.TabIndex = 3;
            this.btn_StartPick.Text = "Start";
            this.btn_StartPick.Click += new System.EventHandler(this.btn_StartPick_Click);
            // 
            // btn_UploadTokens
            // 
            this.btn_UploadTokens.Appearance.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btn_UploadTokens.Appearance.Options.UseFont = true;
            this.btn_UploadTokens.Enabled = false;
            this.btn_UploadTokens.Location = new System.Drawing.Point(58, 6);
            this.btn_UploadTokens.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_UploadTokens.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_UploadTokens.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_UploadTokens.Name = "btn_UploadTokens";
            this.btn_UploadTokens.Size = new System.Drawing.Size(134, 36);
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
            this.panel_TAndC.Location = new System.Drawing.Point(3, 353);
            this.panel_TAndC.Name = "panel_TAndC";
            this.panel_TAndC.Size = new System.Drawing.Size(292, 26);
            this.panel_TAndC.TabIndex = 17;
            // 
            // linkLbl_TAndC
            // 
            this.linkLbl_TAndC.AutoSize = true;
            this.linkLbl_TAndC.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLbl_TAndC.LinkColor = System.Drawing.Color.Black;
            this.linkLbl_TAndC.Location = new System.Drawing.Point(19, 9);
            this.linkLbl_TAndC.Name = "linkLbl_TAndC";
            this.linkLbl_TAndC.Size = new System.Drawing.Size(188, 13);
            this.linkLbl_TAndC.TabIndex = 4;
            this.linkLbl_TAndC.TabStop = true;
            this.linkLbl_TAndC.Text = "I agree to the terms and conditions";
            this.linkLbl_TAndC.VisitedLinkColor = System.Drawing.Color.RoyalBlue;
            this.linkLbl_TAndC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLbl_TAndC_LinkClicked);
            // 
            // chk_TAndC
            // 
            this.chk_TAndC.Location = new System.Drawing.Point(4, 8);
            this.chk_TAndC.Name = "chk_TAndC";
            this.chk_TAndC.Properties.Caption = "";
            this.chk_TAndC.Size = new System.Drawing.Size(17, 17);
            this.chk_TAndC.TabIndex = 5;
            this.chk_TAndC.CheckedChanged += new System.EventHandler(this.chk_TAndC_CheckedChanged);
            // 
            // btn_ReloadAndRecompute
            // 
            this.btn_ReloadAndRecompute.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btn_ReloadAndRecompute.Appearance.Options.UseFont = true;
            this.btn_ReloadAndRecompute.Enabled = false;
            this.btn_ReloadAndRecompute.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.ImageOptions.Image")));
            this.btn_ReloadAndRecompute.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btn_ReloadAndRecompute.Location = new System.Drawing.Point(7, 6);
            this.btn_ReloadAndRecompute.LookAndFeel.SkinName = "DevExpress Style";
            this.btn_ReloadAndRecompute.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_ReloadAndRecompute.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_ReloadAndRecompute.Name = "btn_ReloadAndRecompute";
            this.btn_ReloadAndRecompute.Size = new System.Drawing.Size(48, 36);
            this.btn_ReloadAndRecompute.TabIndex = 32;
            this.btn_ReloadAndRecompute.ToolTip = "Reload and Recompute";
            this.btn_ReloadAndRecompute.Click += new System.EventHandler(this.btn_ReloadAndRecompute_Click);
            // 
            // TradePicker
            // 
            this.Appearance.BackColor = System.Drawing.Color.Black;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 382);
            this.Controls.Add(this.panel_Main);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("TradePicker.IconOptions.Icon")));
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.SkinName = "The Bezier";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.Name = "TradePicker";
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
    }
}