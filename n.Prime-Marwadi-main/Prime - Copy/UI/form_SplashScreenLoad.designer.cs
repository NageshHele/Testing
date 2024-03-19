namespace Prime
{
    partial class SplashScreenLoad
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreenLoad));
            this.progressBarControl = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            this.lbl_Status = new DevExpress.XtraEditors.LabelControl();
            this.img_Logo = new DevExpress.XtraEditors.PictureEdit();
            this.lbl_Copyright = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.progressBarControl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.img_Logo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBarControl
            // 
            this.progressBarControl.EditValue = 0;
            this.progressBarControl.Location = new System.Drawing.Point(12, 170);
            this.progressBarControl.Name = "progressBarControl";
            this.progressBarControl.Properties.EndColor = System.Drawing.SystemColors.Desktop;
            this.progressBarControl.Properties.LookAndFeel.SkinName = "Office 2013 Dark Gray";
            this.progressBarControl.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.progressBarControl.Properties.StartColor = System.Drawing.SystemColors.Desktop;
            this.progressBarControl.Size = new System.Drawing.Size(276, 12);
            this.progressBarControl.TabIndex = 5;
            // 
            // lbl_Status
            // 
            this.lbl_Status.Location = new System.Drawing.Point(12, 150);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(50, 13);
            this.lbl_Status.TabIndex = 7;
            this.lbl_Status.Text = "Starting...";
            // 
            // img_Logo
            // 
            this.img_Logo.EditValue = ((object)(resources.GetObject("img_Logo.EditValue")));
            this.img_Logo.Location = new System.Drawing.Point(12, 26);
            this.img_Logo.Name = "img_Logo";
            this.img_Logo.Properties.AllowFocused = false;
            this.img_Logo.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.img_Logo.Properties.Appearance.Options.UseBackColor = true;
            this.img_Logo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.img_Logo.Properties.ShowMenu = false;
            this.img_Logo.Properties.ZoomPercent = 25D;
            this.img_Logo.Size = new System.Drawing.Size(276, 91);
            this.img_Logo.TabIndex = 9;
            // 
            // lbl_Copyright
            // 
            this.lbl_Copyright.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbl_Copyright.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lbl_Copyright.Location = new System.Drawing.Point(12, 191);
            this.lbl_Copyright.Name = "lbl_Copyright";
            this.lbl_Copyright.Size = new System.Drawing.Size(138, 13);
            this.lbl_Copyright.TabIndex = 6;
            this.lbl_Copyright.Text = "Copyright © Nerve Solutions";
            // 
            // SplashScreenLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 216);
            this.Controls.Add(this.img_Logo);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.lbl_Copyright);
            this.Controls.Add(this.progressBarControl);
            this.Name = "SplashScreenLoad";
            this.Text = "frm_SpanshScreen";
            ((System.ComponentModel.ISupportInitialize)(this.progressBarControl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.img_Logo.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.MarqueeProgressBarControl progressBarControl;
        private DevExpress.XtraEditors.LabelControl lbl_Status;
        private DevExpress.XtraEditors.PictureEdit img_Logo;
        private DevExpress.XtraEditors.LabelControl lbl_Copyright;
    }
}
