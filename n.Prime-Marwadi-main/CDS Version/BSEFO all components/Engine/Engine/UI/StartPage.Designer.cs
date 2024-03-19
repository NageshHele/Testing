namespace Engine
{
    partial class StartPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartPage));
            this.lbl_Terms = new DevExpress.XtraEditors.HyperlinkLabelControl();
            this.checkTerms = new DevExpress.XtraEditors.CheckEdit();
            this.panelControlStart = new DevExpress.XtraEditors.PanelControl();
            this.pic_Logo = new DevExpress.XtraEditors.PictureEdit();
            ((System.ComponentModel.ISupportInitialize)(this.checkTerms.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlStart)).BeginInit();
            this.panelControlStart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Logo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_Terms
            // 
            this.lbl_Terms.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25F);
            this.lbl_Terms.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lbl_Terms.Appearance.Options.UseFont = true;
            this.lbl_Terms.Appearance.Options.UseForeColor = true;
            this.lbl_Terms.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_Terms.Location = new System.Drawing.Point(248, 290);
            this.lbl_Terms.Name = "lbl_Terms";
            this.lbl_Terms.Size = new System.Drawing.Size(248, 18);
            this.lbl_Terms.TabIndex = 2;
            this.lbl_Terms.Text = "I have read the terms and conditions.";
            this.lbl_Terms.Click += new System.EventHandler(this.lbl_Terms_Click);
            // 
            // checkTerms
            // 
            this.checkTerms.Location = new System.Drawing.Point(220, 292);
            this.checkTerms.Name = "checkTerms";
            this.checkTerms.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25F);
            this.checkTerms.Properties.Appearance.Options.UseFont = true;
            this.checkTerms.Properties.Caption = "";
            this.checkTerms.Size = new System.Drawing.Size(19, 17);
            this.checkTerms.TabIndex = 3;
            this.checkTerms.CheckedChanged += new System.EventHandler(this.checkTerms_CheckedChanged_1);
            // 
            // panelControlStart
            // 
            this.panelControlStart.Appearance.BackColor = System.Drawing.Color.White;
            this.panelControlStart.Appearance.Options.UseBackColor = true;
            this.panelControlStart.Controls.Add(this.pic_Logo);
            this.panelControlStart.Controls.Add(this.lbl_Terms);
            this.panelControlStart.Controls.Add(this.checkTerms);
            this.panelControlStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControlStart.Location = new System.Drawing.Point(0, 0);
            this.panelControlStart.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.panelControlStart.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panelControlStart.Name = "panelControlStart";
            this.panelControlStart.Size = new System.Drawing.Size(531, 347);
            this.panelControlStart.TabIndex = 4;
            // 
            // pic_Logo
            // 
            this.pic_Logo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pic_Logo.EditValue = ((object)(resources.GetObject("pic_Logo.EditValue")));
            this.pic_Logo.Location = new System.Drawing.Point(0, 0);
            this.pic_Logo.Name = "pic_Logo";
            this.pic_Logo.Properties.AllowFocused = false;
            this.pic_Logo.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.pic_Logo.Properties.Appearance.Options.UseBackColor = true;
            this.pic_Logo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pic_Logo.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pic_Logo.Size = new System.Drawing.Size(531, 284);
            this.pic_Logo.TabIndex = 27;
            // 
            // StartPage
            // 
            this.Appearance.BackColor = System.Drawing.Color.Black;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 347);
            this.Controls.Add(this.panelControlStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("StartPage.IconOptions.Icon")));
            this.IconOptions.ShowIcon = false;
            this.MaximizeBox = false;
            this.Name = "StartPage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "n.Engine";
            ((System.ComponentModel.ISupportInitialize)(this.checkTerms.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlStart)).EndInit();
            this.panelControlStart.ResumeLayout(false);
            this.panelControlStart.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Logo.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.HyperlinkLabelControl lbl_Terms;
        private DevExpress.XtraEditors.CheckEdit checkTerms;
        private DevExpress.XtraEditors.PanelControl panelControlStart;
        private DevExpress.XtraEditors.PictureEdit pic_Logo;
    }
}

