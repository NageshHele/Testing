namespace BOD_Utility
{
    partial class Home
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
            this.listBox_Messages = new DevExpress.XtraEditors.ListBoxControl();
            this.dateEdit_DownloadDate = new DevExpress.XtraEditors.DateEdit();
            this.btn_StartMnually = new DevExpress.XtraEditors.SimpleButton();
            this.btn_StartAuto = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Settings = new DevExpress.XtraEditors.PictureEdit();
            this.btn_RestartAuto = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.listBox_Messages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit_DownloadDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit_DownloadDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Settings.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox_Messages
            // 
            this.listBox_Messages.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listBox_Messages.HorizontalScrollbar = true;
            this.listBox_Messages.Location = new System.Drawing.Point(0, 91);
            this.listBox_Messages.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBox_Messages.Name = "listBox_Messages";
            this.listBox_Messages.Size = new System.Drawing.Size(976, 392);
            this.listBox_Messages.TabIndex = 0;
            this.listBox_Messages.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.listBox_Messages_DrawItem);
            // 
            // dateEdit_DownloadDate
            // 
            this.dateEdit_DownloadDate.EditValue = new System.DateTime(2020, 12, 21, 13, 34, 59, 609);
            this.dateEdit_DownloadDate.Location = new System.Drawing.Point(45, 17);
            this.dateEdit_DownloadDate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dateEdit_DownloadDate.Name = "dateEdit_DownloadDate";
            this.dateEdit_DownloadDate.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateEdit_DownloadDate.Properties.Appearance.Options.UseFont = true;
            this.dateEdit_DownloadDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit_DownloadDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEdit_DownloadDate.Properties.NullDate = new System.DateTime(2020, 12, 21, 13, 35, 36, 62);
            this.dateEdit_DownloadDate.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.dateEdit_DownloadDate.Size = new System.Drawing.Size(202, 44);
            this.dateEdit_DownloadDate.TabIndex = 1;
            // 
            // btn_StartMnually
            // 
            this.btn_StartMnually.Appearance.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_StartMnually.Appearance.Options.UseFont = true;
            this.btn_StartMnually.Location = new System.Drawing.Point(293, 14);
            this.btn_StartMnually.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_StartMnually.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_StartMnually.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_StartMnually.Name = "btn_StartMnually";
            this.btn_StartMnually.Size = new System.Drawing.Size(169, 51);
            this.btn_StartMnually.TabIndex = 2;
            this.btn_StartMnually.Text = "Start Manually";
            this.btn_StartMnually.Click += new System.EventHandler(this.btn_StartMnually_Click);
            // 
            // btn_StartAuto
            // 
            this.btn_StartAuto.Appearance.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_StartAuto.Appearance.Options.UseFont = true;
            this.btn_StartAuto.Location = new System.Drawing.Point(494, 14);
            this.btn_StartAuto.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_StartAuto.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_StartAuto.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_StartAuto.Name = "btn_StartAuto";
            this.btn_StartAuto.Size = new System.Drawing.Size(185, 51);
            this.btn_StartAuto.TabIndex = 3;
            this.btn_StartAuto.Text = "Start Automatically";
            this.btn_StartAuto.Click += new System.EventHandler(this.btn_StartAuto_Click);
            // 
            // btn_Settings
            // 
            this.btn_Settings.EditValue = ((object)(resources.GetObject("btn_Settings.EditValue")));
            this.btn_Settings.Location = new System.Drawing.Point(901, 20);
            this.btn_Settings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_Settings.Name = "btn_Settings";
            this.btn_Settings.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.btn_Settings.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Settings.Properties.Appearance.Options.UseBackColor = true;
            this.btn_Settings.Properties.Appearance.Options.UseFont = true;
            this.btn_Settings.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btn_Settings.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.btn_Settings.Size = new System.Drawing.Size(35, 41);
            this.btn_Settings.TabIndex = 38;
            this.btn_Settings.Click += new System.EventHandler(this.btn_Settings_Click);
            // 
            // btn_RestartAuto
            // 
            this.btn_RestartAuto.Appearance.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_RestartAuto.Appearance.Options.UseFont = true;
            this.btn_RestartAuto.Location = new System.Drawing.Point(708, 14);
            this.btn_RestartAuto.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_RestartAuto.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_RestartAuto.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_RestartAuto.Name = "btn_RestartAuto";
            this.btn_RestartAuto.Size = new System.Drawing.Size(185, 51);
            this.btn_RestartAuto.TabIndex = 3;
            this.btn_RestartAuto.Text = "Restart Process";
            this.btn_RestartAuto.Click += new System.EventHandler(this.btn_RestartAuto_Click);
            // 
            // Home
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 483);
            this.Controls.Add(this.btn_Settings);
            this.Controls.Add(this.btn_RestartAuto);
            this.Controls.Add(this.btn_StartAuto);
            this.Controls.Add(this.btn_StartMnually);
            this.Controls.Add(this.dateEdit_DownloadDate);
            this.Controls.Add(this.listBox_Messages);
            this.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("Home.IconOptions.Image")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Home";
            this.Text = "BOD Utility";
            ((System.ComponentModel.ISupportInitialize)(this.listBox_Messages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit_DownloadDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit_DownloadDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Settings.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.ListBoxControl listBox_Messages;
        private DevExpress.XtraEditors.DateEdit dateEdit_DownloadDate;
        private DevExpress.XtraEditors.SimpleButton btn_StartMnually;
        private DevExpress.XtraEditors.SimpleButton btn_StartAuto;
        private DevExpress.XtraEditors.PictureEdit btn_Settings;
        private DevExpress.XtraEditors.SimpleButton btn_RestartAuto;
    }
}

