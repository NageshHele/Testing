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
            this.lbl_SpanTime = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btn_ComputeSpan = new DevExpress.XtraEditors.SimpleButton();
            this.btn_ReloadAndRecompute = new DevExpress.XtraEditors.SimpleButton();
            this.ind_SpanConnected = new DevExpress.XtraEditors.LabelControl();
            this.ind_EngineConnected = new DevExpress.XtraEditors.LabelControl();
            this.ind_TradeFileFOAvailable = new DevExpress.XtraEditors.LabelControl();
            this.ind_SpanNotConnected = new DevExpress.XtraEditors.LabelControl();
            this.ind_EngineNotConnected = new DevExpress.XtraEditors.LabelControl();
            this.ind_TradeFileFO = new DevExpress.XtraEditors.LabelControl();
            this.btn_StartPick = new DevExpress.XtraEditors.SimpleButton();
            this.panel_TAndC = new DevExpress.XtraEditors.PanelControl();
            this.linkLbl_TAndC = new System.Windows.Forms.LinkLabel();
            this.chk_TAndC = new DevExpress.XtraEditors.CheckEdit();
            this.ind_TradeFileBSEFO = new DevExpress.XtraEditors.LabelControl();
            this.ind_TradeFileBSEFOAvailable = new DevExpress.XtraEditors.LabelControl();
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
            this.notifyIconGateway.Text = "nImageB";
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
            this.panel_Main.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel_Main.Name = "panel_Main";
            this.panel_Main.Size = new System.Drawing.Size(348, 500);
            this.panel_Main.TabIndex = 8;
            // 
            // listBox_Messages
            // 
            this.listBox_Messages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Messages.HorizontalScrollbar = true;
            this.listBox_Messages.Location = new System.Drawing.Point(3, 121);
            this.listBox_Messages.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listBox_Messages.Name = "listBox_Messages";
            this.listBox_Messages.Size = new System.Drawing.Size(342, 342);
            this.listBox_Messages.TabIndex = 20;
            // 
            // panel_Top
            // 
            this.panel_Top.Controls.Add(this.ind_TradeFileBSEFO);
            this.panel_Top.Controls.Add(this.ind_TradeFileBSEFOAvailable);
            this.panel_Top.Controls.Add(this.lbl_SpanTime);
            this.panel_Top.Controls.Add(this.labelControl1);
            this.panel_Top.Controls.Add(this.btn_ComputeSpan);
            this.panel_Top.Controls.Add(this.btn_ReloadAndRecompute);
            this.panel_Top.Controls.Add(this.ind_SpanConnected);
            this.panel_Top.Controls.Add(this.ind_EngineConnected);
            this.panel_Top.Controls.Add(this.ind_TradeFileFOAvailable);
            this.panel_Top.Controls.Add(this.ind_SpanNotConnected);
            this.panel_Top.Controls.Add(this.ind_EngineNotConnected);
            this.panel_Top.Controls.Add(this.ind_TradeFileFO);
            this.panel_Top.Controls.Add(this.btn_StartPick);
            this.panel_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Top.Location = new System.Drawing.Point(3, 3);
            this.panel_Top.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel_Top.Name = "panel_Top";
            this.panel_Top.Size = new System.Drawing.Size(342, 118);
            this.panel_Top.TabIndex = 19;
            // 
            // lbl_SpanTime
            // 
            this.lbl_SpanTime.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_SpanTime.Appearance.Options.UseFont = true;
            this.lbl_SpanTime.Location = new System.Drawing.Point(85, 68);
            this.lbl_SpanTime.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lbl_SpanTime.Name = "lbl_SpanTime";
            this.lbl_SpanTime.Size = new System.Drawing.Size(116, 20);
            this.lbl_SpanTime.TabIndex = 33;
            this.lbl_SpanTime.Text = "Latest Span Time:";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(10, 68);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(73, 20);
            this.labelControl1.TabIndex = 33;
            this.labelControl1.Text = "Span Time:";
            // 
            // btn_ComputeSpan
            // 
            this.btn_ComputeSpan.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btn_ComputeSpan.Appearance.Options.UseFont = true;
            this.btn_ComputeSpan.Enabled = false;
            this.btn_ComputeSpan.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_ComputeSpan.ImageOptions.Image")));
            this.btn_ComputeSpan.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btn_ComputeSpan.Location = new System.Drawing.Point(85, 8);
            this.btn_ComputeSpan.LookAndFeel.SkinName = "DevExpress Style";
            this.btn_ComputeSpan.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_ComputeSpan.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.btn_ComputeSpan.Name = "btn_ComputeSpan";
            this.btn_ComputeSpan.Size = new System.Drawing.Size(56, 47);
            this.btn_ComputeSpan.TabIndex = 32;
            this.btn_ComputeSpan.ToolTip = "Compute Span";
            this.btn_ComputeSpan.Click += new System.EventHandler(this.btn_ComputeSpan_Click);
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
            // ind_SpanConnected
            // 
            this.ind_SpanConnected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_SpanConnected.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_SpanConnected.Appearance.Image")));
            this.ind_SpanConnected.Appearance.Options.UseImage = true;
            this.ind_SpanConnected.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_SpanConnected.Location = new System.Drawing.Point(241, 65);
            this.ind_SpanConnected.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ind_SpanConnected.Name = "ind_SpanConnected";
            this.ind_SpanConnected.Size = new System.Drawing.Size(16, 17);
            this.ind_SpanConnected.TabIndex = 27;
            this.ind_SpanConnected.ToolTip = "Span Connected";
            this.ind_SpanConnected.Visible = false;
            // 
            // ind_EngineConnected
            // 
            this.ind_EngineConnected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_EngineConnected.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_EngineConnected.Appearance.Image")));
            this.ind_EngineConnected.Appearance.Options.UseImage = true;
            this.ind_EngineConnected.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_EngineConnected.Location = new System.Drawing.Point(267, 67);
            this.ind_EngineConnected.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ind_EngineConnected.Name = "ind_EngineConnected";
            this.ind_EngineConnected.Size = new System.Drawing.Size(16, 17);
            this.ind_EngineConnected.TabIndex = 27;
            this.ind_EngineConnected.ToolTip = "Engine Connected";
            this.ind_EngineConnected.Visible = false;
            // 
            // ind_TradeFileFOAvailable
            // 
            this.ind_TradeFileFOAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileFOAvailable.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileFOAvailable.Appearance.Image")));
            this.ind_TradeFileFOAvailable.Appearance.Options.UseImage = true;
            this.ind_TradeFileFOAvailable.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileFOAvailable.Location = new System.Drawing.Point(292, 67);
            this.ind_TradeFileFOAvailable.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ind_TradeFileFOAvailable.Name = "ind_TradeFileFOAvailable";
            this.ind_TradeFileFOAvailable.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileFOAvailable.TabIndex = 27;
            this.ind_TradeFileFOAvailable.Visible = false;
            // 
            // ind_SpanNotConnected
            // 
            this.ind_SpanNotConnected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_SpanNotConnected.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_SpanNotConnected.Appearance.Image")));
            this.ind_SpanNotConnected.Appearance.Options.UseImage = true;
            this.ind_SpanNotConnected.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_SpanNotConnected.Location = new System.Drawing.Point(241, 65);
            this.ind_SpanNotConnected.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ind_SpanNotConnected.Name = "ind_SpanNotConnected";
            this.ind_SpanNotConnected.Size = new System.Drawing.Size(16, 17);
            this.ind_SpanNotConnected.TabIndex = 26;
            this.ind_SpanNotConnected.ToolTip = "Span Not Connected";
            // 
            // ind_EngineNotConnected
            // 
            this.ind_EngineNotConnected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_EngineNotConnected.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_EngineNotConnected.Appearance.Image")));
            this.ind_EngineNotConnected.Appearance.Options.UseImage = true;
            this.ind_EngineNotConnected.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_EngineNotConnected.Location = new System.Drawing.Point(267, 65);
            this.ind_EngineNotConnected.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ind_EngineNotConnected.Name = "ind_EngineNotConnected";
            this.ind_EngineNotConnected.Size = new System.Drawing.Size(16, 17);
            this.ind_EngineNotConnected.TabIndex = 26;
            this.ind_EngineNotConnected.ToolTip = "Engine Not Connected";
            // 
            // ind_TradeFileFO
            // 
            this.ind_TradeFileFO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileFO.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileFO.Appearance.Image")));
            this.ind_TradeFileFO.Appearance.Options.UseImage = true;
            this.ind_TradeFileFO.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileFO.Location = new System.Drawing.Point(292, 65);
            this.ind_TradeFileFO.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ind_TradeFileFO.Name = "ind_TradeFileFO";
            this.ind_TradeFileFO.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileFO.TabIndex = 26;
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
            // panel_TAndC
            // 
            this.panel_TAndC.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panel_TAndC.Controls.Add(this.linkLbl_TAndC);
            this.panel_TAndC.Controls.Add(this.chk_TAndC);
            this.panel_TAndC.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_TAndC.Location = new System.Drawing.Point(3, 463);
            this.panel_TAndC.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.chk_TAndC.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chk_TAndC.Name = "chk_TAndC";
            this.chk_TAndC.Properties.Caption = "";
            this.chk_TAndC.Size = new System.Drawing.Size(20, 20);
            this.chk_TAndC.TabIndex = 5;
            this.chk_TAndC.CheckedChanged += new System.EventHandler(this.chk_TAndC_CheckedChanged);
            // 
            // ind_TradeFileBSEFO
            // 
            this.ind_TradeFileBSEFO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileBSEFO.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileBSEFO.Appearance.Image")));
            this.ind_TradeFileBSEFO.Appearance.Options.UseImage = true;
            this.ind_TradeFileBSEFO.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileBSEFO.Location = new System.Drawing.Point(316, 66);
            this.ind_TradeFileBSEFO.Margin = new System.Windows.Forms.Padding(4);
            this.ind_TradeFileBSEFO.Name = "ind_TradeFileBSEFO";
            this.ind_TradeFileBSEFO.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileBSEFO.TabIndex = 34;
            // 
            // ind_TradeFileBSEFOAvailable
            // 
            this.ind_TradeFileBSEFOAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ind_TradeFileBSEFOAvailable.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("ind_TradeFileBSEFOAvailable.Appearance.Image")));
            this.ind_TradeFileBSEFOAvailable.Appearance.Options.UseImage = true;
            this.ind_TradeFileBSEFOAvailable.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ind_TradeFileBSEFOAvailable.Location = new System.Drawing.Point(316, 66);
            this.ind_TradeFileBSEFOAvailable.Margin = new System.Windows.Forms.Padding(4);
            this.ind_TradeFileBSEFOAvailable.Name = "ind_TradeFileBSEFOAvailable";
            this.ind_TradeFileBSEFOAvailable.Size = new System.Drawing.Size(16, 17);
            this.ind_TradeFileBSEFOAvailable.TabIndex = 35;
            this.ind_TradeFileBSEFOAvailable.Visible = false;
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
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frm_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "nImageB";
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
        private DevExpress.XtraEditors.SimpleButton btn_StartPick;
        private DevExpress.XtraEditors.ListBoxControl listBox_Messages;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileFOAvailable;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileFO;
        private DevExpress.XtraEditors.LabelControl ind_SpanConnected;
        private DevExpress.XtraEditors.LabelControl ind_EngineConnected;
        private DevExpress.XtraEditors.LabelControl ind_SpanNotConnected;
        private DevExpress.XtraEditors.LabelControl ind_EngineNotConnected;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lbl_SpanTime;
        internal DevExpress.XtraEditors.SimpleButton btn_ComputeSpan;
        internal DevExpress.XtraEditors.SimpleButton btn_ReloadAndRecompute;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileBSEFO;
        private DevExpress.XtraEditors.LabelControl ind_TradeFileBSEFOAvailable;
    }
}