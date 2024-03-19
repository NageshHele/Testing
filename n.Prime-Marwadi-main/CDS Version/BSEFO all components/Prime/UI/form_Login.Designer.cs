
namespace Prime.UI
{
    partial class form_Login
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
            DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager_Login = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Prime.SplashScreenLoad), true, true);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_Login));
            this.pic_Logo = new DevExpress.XtraEditors.PictureEdit();
            this.btn_Settings = new DevExpress.XtraEditors.PictureEdit();
            this.btn_Login = new DevExpress.XtraEditors.SimpleButton();
            this.linkLabel_TAndC = new System.Windows.Forms.LinkLabel();
            this.chk_TAndC = new DevExpress.XtraEditors.CheckEdit();
            this.txt_Password = new System.Windows.Forms.TextBox();
            this.txt_Username = new System.Windows.Forms.TextBox();
            this.lbl_Password = new DevExpress.XtraEditors.LabelControl();
            this.lbl_Username = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Logo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Settings.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chk_TAndC.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // splashScreenManager_Login
            // 
            splashScreenManager_Login.ClosingDelay = 500;
            // 
            // pic_Logo
            // 
            this.pic_Logo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pic_Logo.EditValue = ((object)(resources.GetObject("pic_Logo.EditValue")));
            this.pic_Logo.Location = new System.Drawing.Point(241, 0);
            this.pic_Logo.Name = "pic_Logo";
            this.pic_Logo.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.pic_Logo.Properties.Appearance.Options.UseBackColor = true;
            this.pic_Logo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pic_Logo.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pic_Logo.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.pic_Logo.Size = new System.Drawing.Size(120, 29);
            this.pic_Logo.TabIndex = 28;
            // 
            // btn_Settings
            // 
            this.btn_Settings.EditValue = ((object)(resources.GetObject("btn_Settings.EditValue")));
            this.btn_Settings.Location = new System.Drawing.Point(5, 4);
            this.btn_Settings.Name = "btn_Settings";
            this.btn_Settings.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.btn_Settings.Properties.Appearance.Options.UseBackColor = true;
            this.btn_Settings.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btn_Settings.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.btn_Settings.Size = new System.Drawing.Size(23, 23);
            this.btn_Settings.TabIndex = 37;
            this.btn_Settings.Click += new System.EventHandler(this.btn_Settings_Click);
            // 
            // btn_Login
            // 
            this.btn_Login.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Login.Appearance.Options.UseFont = true;
            this.btn_Login.AppearanceDisabled.BackColor = System.Drawing.Color.Gray;
            this.btn_Login.AppearanceDisabled.ForeColor = System.Drawing.Color.Gray;
            this.btn_Login.AppearanceDisabled.Options.UseBackColor = true;
            this.btn_Login.AppearanceDisabled.Options.UseForeColor = true;
            this.btn_Login.AppearancePressed.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btn_Login.AppearancePressed.BackColor2 = System.Drawing.Color.CornflowerBlue;
            this.btn_Login.AppearancePressed.Options.UseBackColor = true;
            this.btn_Login.Cursor = System.Windows.Forms.Cursors.Default;
            this.btn_Login.Enabled = false;
            this.btn_Login.Location = new System.Drawing.Point(36, 165);
            this.btn_Login.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_Login.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.Size = new System.Drawing.Size(108, 36);
            this.btn_Login.TabIndex = 31;
            this.btn_Login.Text = "Login";
            this.btn_Login.Click += new System.EventHandler(this.btn_Login_Click);
            // 
            // linkLabel_TAndC
            // 
            this.linkLabel_TAndC.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel_TAndC.LinkColor = System.Drawing.Color.Black;
            this.linkLabel_TAndC.Location = new System.Drawing.Point(178, 169);
            this.linkLabel_TAndC.Name = "linkLabel_TAndC";
            this.linkLabel_TAndC.Size = new System.Drawing.Size(147, 44);
            this.linkLabel_TAndC.TabIndex = 36;
            this.linkLabel_TAndC.TabStop = true;
            this.linkLabel_TAndC.Text = "I accept the terms and conditions";
            this.linkLabel_TAndC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_TAndC_LinkClicked);
            // 
            // chk_TAndC
            // 
            this.chk_TAndC.Location = new System.Drawing.Point(160, 170);
            this.chk_TAndC.Margin = new System.Windows.Forms.Padding(2);
            this.chk_TAndC.Name = "chk_TAndC";
            this.chk_TAndC.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chk_TAndC.Properties.Appearance.Options.UseFont = true;
            this.chk_TAndC.Properties.Caption = "";
            this.chk_TAndC.Size = new System.Drawing.Size(38, 20);
            this.chk_TAndC.TabIndex = 32;
            this.chk_TAndC.CheckedChanged += new System.EventHandler(this.chk_TAndC_CheckedChanged);
            // 
            // txt_Password
            // 
            this.txt_Password.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.txt_Password.Location = new System.Drawing.Point(145, 99);
            this.txt_Password.Name = "txt_Password";
            this.txt_Password.PasswordChar = '*';
            this.txt_Password.Size = new System.Drawing.Size(144, 27);
            this.txt_Password.TabIndex = 30;
            // 
            // txt_Username
            // 
            this.txt_Username.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.txt_Username.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.txt_Username.Location = new System.Drawing.Point(145, 43);
            this.txt_Username.Name = "txt_Username";
            this.txt_Username.Size = new System.Drawing.Size(144, 27);
            this.txt_Username.TabIndex = 29;
            // 
            // lbl_Password
            // 
            this.lbl_Password.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Password.Appearance.Options.UseFont = true;
            this.lbl_Password.Location = new System.Drawing.Point(61, 101);
            this.lbl_Password.Name = "lbl_Password";
            this.lbl_Password.Size = new System.Drawing.Size(69, 21);
            this.lbl_Password.TabIndex = 33;
            this.lbl_Password.Text = "Password";
            // 
            // lbl_Username
            // 
            this.lbl_Username.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Username.Appearance.Options.UseFont = true;
            this.lbl_Username.Location = new System.Drawing.Point(61, 45);
            this.lbl_Username.Name = "lbl_Username";
            this.lbl_Username.Size = new System.Drawing.Size(73, 21);
            this.lbl_Username.TabIndex = 34;
            this.lbl_Username.Text = "Username";
            // 
            // form_Login
            // 
            this.AcceptButton = this.btn_Login;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 241);
            this.Controls.Add(this.btn_Settings);
            this.Controls.Add(this.btn_Login);
            this.Controls.Add(this.linkLabel_TAndC);
            this.Controls.Add(this.chk_TAndC);
            this.Controls.Add(this.txt_Password);
            this.Controls.Add(this.txt_Username);
            this.Controls.Add(this.lbl_Password);
            this.Controls.Add(this.lbl_Username);
            this.Controls.Add(this.pic_Logo);
            this.IconOptions.ShowIcon = false;
            this.MaximizeBox = false;
            this.Name = "form_Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Login_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Logo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_Settings.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chk_TAndC.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PictureEdit pic_Logo;
        private DevExpress.XtraEditors.PictureEdit btn_Settings;
        private DevExpress.XtraEditors.SimpleButton btn_Login;
        private System.Windows.Forms.LinkLabel linkLabel_TAndC;
        private DevExpress.XtraEditors.CheckEdit chk_TAndC;
        private System.Windows.Forms.TextBox txt_Password;
        private System.Windows.Forms.TextBox txt_Username;
        private DevExpress.XtraEditors.LabelControl lbl_Password;
        private DevExpress.XtraEditors.LabelControl lbl_Username;
    }
}