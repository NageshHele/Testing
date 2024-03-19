namespace Engine
{
    partial class Download
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
            this.btn_DownloadFiles = new DevExpress.XtraEditors.SimpleButton();
            this.comboBoxEdit_SpanType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.comboBoxEdit_FileType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.panelControlDownload = new DevExpress.XtraEditors.PanelControl();
            this.webBrowser_SpanCD = new System.Windows.Forms.WebBrowser();
            this.webBrowser_SpanFO = new System.Windows.Forms.WebBrowser();
            this.webBrowserFO = new System.Windows.Forms.WebBrowser();
            this.webBrowserCM = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_SpanType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_FileType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlDownload)).BeginInit();
            this.panelControlDownload.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_DownloadFiles
            // 
            this.btn_DownloadFiles.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_DownloadFiles.Appearance.Options.UseFont = true;
            this.btn_DownloadFiles.AppearanceHovered.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_DownloadFiles.AppearanceHovered.ForeColor = System.Drawing.Color.Black;
            this.btn_DownloadFiles.Location = new System.Drawing.Point(427, 46);
            this.btn_DownloadFiles.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_DownloadFiles.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_DownloadFiles.Name = "btn_DownloadFiles";
            this.btn_DownloadFiles.Size = new System.Drawing.Size(128, 47);
            this.btn_DownloadFiles.TabIndex = 22;
            this.btn_DownloadFiles.Text = "Download";
            this.btn_DownloadFiles.Click += new System.EventHandler(this.btn_DownloadFiles_Click);
            // 
            // comboBoxEdit_SpanType
            // 
            this.comboBoxEdit_SpanType.EditValue = "Span Type";
            this.comboBoxEdit_SpanType.Location = new System.Drawing.Point(215, 80);
            this.comboBoxEdit_SpanType.Name = "comboBoxEdit_SpanType";
            this.comboBoxEdit_SpanType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxEdit_SpanType.Properties.Appearance.Options.UseFont = true;
            this.comboBoxEdit_SpanType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit_SpanType.Properties.Items.AddRange(new object[] {
            "Span Type",
            "T0",
            "T1"});
            this.comboBoxEdit_SpanType.Size = new System.Drawing.Size(193, 26);
            this.comboBoxEdit_SpanType.TabIndex = 21;
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CalendarFont = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker.Location = new System.Drawing.Point(5, 40);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(193, 25);
            this.dateTimePicker.TabIndex = 19;
            this.dateTimePicker.Value = new System.DateTime(2017, 10, 6, 15, 37, 3, 0);
            // 
            // comboBoxEdit_FileType
            // 
            this.comboBoxEdit_FileType.EditValue = "File type";
            this.comboBoxEdit_FileType.Location = new System.Drawing.Point(215, 41);
            this.comboBoxEdit_FileType.Name = "comboBoxEdit_FileType";
            this.comboBoxEdit_FileType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxEdit_FileType.Properties.Appearance.Options.UseFont = true;
            this.comboBoxEdit_FileType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit_FileType.Properties.Items.AddRange(new object[] {
            "file type",
            "Begin Day SPAN",
            "1st Intra-day SPAN",
            "2nd Intra-day SPAN",
            "3rd Intra-day SPAN",
            "4th Intra-day SPAN",
            "End of day SPAN"});
            this.comboBoxEdit_FileType.Properties.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.comboBoxEdit_FileType.Size = new System.Drawing.Size(193, 26);
            this.comboBoxEdit_FileType.TabIndex = 20;
            // 
            // panelControlDownload
            // 
            this.panelControlDownload.Appearance.BackColor = System.Drawing.Color.White;
            this.panelControlDownload.Appearance.Options.UseBackColor = true;
            this.panelControlDownload.Controls.Add(this.webBrowser_SpanCD);
            this.panelControlDownload.Controls.Add(this.webBrowser_SpanFO);
            this.panelControlDownload.Controls.Add(this.webBrowserFO);
            this.panelControlDownload.Controls.Add(this.webBrowserCM);
            this.panelControlDownload.Controls.Add(this.dateTimePicker);
            this.panelControlDownload.Controls.Add(this.btn_DownloadFiles);
            this.panelControlDownload.Controls.Add(this.comboBoxEdit_FileType);
            this.panelControlDownload.Controls.Add(this.comboBoxEdit_SpanType);
            this.panelControlDownload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControlDownload.Location = new System.Drawing.Point(0, 0);
            this.panelControlDownload.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.panelControlDownload.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.panelControlDownload.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panelControlDownload.Name = "panelControlDownload";
            this.panelControlDownload.Size = new System.Drawing.Size(578, 211);
            this.panelControlDownload.TabIndex = 23;
            // 
            // webBrowser_SpanCD
            // 
            this.webBrowser_SpanCD.Location = new System.Drawing.Point(253, 180);
            this.webBrowser_SpanCD.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser_SpanCD.Name = "webBrowser_SpanCD";
            this.webBrowser_SpanCD.Size = new System.Drawing.Size(186, 26);
            this.webBrowser_SpanCD.TabIndex = 26;
            this.webBrowser_SpanCD.Visible = false;
            this.webBrowser_SpanCD.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser_SpanCD_Navigating);
            // 
            // webBrowser_SpanFO
            // 
            this.webBrowser_SpanFO.Location = new System.Drawing.Point(253, 143);
            this.webBrowser_SpanFO.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser_SpanFO.Name = "webBrowser_SpanFO";
            this.webBrowser_SpanFO.Size = new System.Drawing.Size(186, 20);
            this.webBrowser_SpanFO.TabIndex = 25;
            this.webBrowser_SpanFO.Visible = false;
            this.webBrowser_SpanFO.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser_Span_Navigating);
            // 
            // webBrowserFO
            // 
            this.webBrowserFO.Location = new System.Drawing.Point(5, 143);
            this.webBrowserFO.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserFO.Name = "webBrowserFO";
            this.webBrowserFO.Size = new System.Drawing.Size(186, 20);
            this.webBrowserFO.TabIndex = 23;
            this.webBrowserFO.Visible = false;
            this.webBrowserFO.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebBrowserFO_Navigating);
            // 
            // webBrowserCM
            // 
            this.webBrowserCM.Location = new System.Drawing.Point(5, 186);
            this.webBrowserCM.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserCM.Name = "webBrowserCM";
            this.webBrowserCM.Size = new System.Drawing.Size(186, 20);
            this.webBrowserCM.TabIndex = 24;
            this.webBrowserCM.Visible = false;
            this.webBrowserCM.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebBrowserCM_Navigating);
            // 
            // Download
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 211);
            this.Controls.Add(this.panelControlDownload);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Download";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Download";
            this.Load += new System.EventHandler(this.Download_Load);
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_SpanType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_FileType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlDownload)).EndInit();
            this.panelControlDownload.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btn_DownloadFiles;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit_SpanType;
        private System.Windows.Forms.DateTimePicker dateTimePicker;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit_FileType;
        private DevExpress.XtraEditors.PanelControl panelControlDownload;
        private System.Windows.Forms.WebBrowser webBrowser_SpanCD;
        private System.Windows.Forms.WebBrowser webBrowser_SpanFO;
        private System.Windows.Forms.WebBrowser webBrowserFO;
        private System.Windows.Forms.WebBrowser webBrowserCM;
    }
}