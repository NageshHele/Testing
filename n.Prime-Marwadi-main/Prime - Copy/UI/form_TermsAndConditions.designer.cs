namespace Prime
{
    partial class TermsAndConditions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TermsAndConditions));
            this.txt_TermsAndConditions = new System.Windows.Forms.RichTextBox();
            this.btn_TermsOK = new DevExpress.XtraEditors.SimpleButton();
            this.panelControlTerms = new DevExpress.XtraEditors.PanelControl();
            this.panelControlTermsOK = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlTerms)).BeginInit();
            this.panelControlTerms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlTermsOK)).BeginInit();
            this.panelControlTermsOK.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_TermsAndConditions
            // 
            this.txt_TermsAndConditions.Dock = System.Windows.Forms.DockStyle.Top;
            this.txt_TermsAndConditions.Location = new System.Drawing.Point(2, 2);
            this.txt_TermsAndConditions.Name = "txt_TermsAndConditions";
            this.txt_TermsAndConditions.ReadOnly = true;
            this.txt_TermsAndConditions.Size = new System.Drawing.Size(996, 589);
            this.txt_TermsAndConditions.TabIndex = 0;
            this.txt_TermsAndConditions.Text = "";
            // 
            // btn_TermsOK
            // 
            this.btn_TermsOK.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_TermsOK.Appearance.Options.UseFont = true;
            this.btn_TermsOK.Location = new System.Drawing.Point(857, 8);
            this.btn_TermsOK.LookAndFeel.SkinName = "McSkin";
            this.btn_TermsOK.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_TermsOK.Name = "btn_TermsOK";
            this.btn_TermsOK.Size = new System.Drawing.Size(126, 50);
            this.btn_TermsOK.TabIndex = 1;
            this.btn_TermsOK.Text = "OK";
            this.btn_TermsOK.Click += new System.EventHandler(this.btn_TermsOK_Click);
            // 
            // panelControlTerms
            // 
            this.panelControlTerms.Controls.Add(this.txt_TermsAndConditions);
            this.panelControlTerms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControlTerms.Location = new System.Drawing.Point(0, 0);
            this.panelControlTerms.Name = "panelControlTerms";
            this.panelControlTerms.Size = new System.Drawing.Size(1000, 673);
            this.panelControlTerms.TabIndex = 2;
            // 
            // panelControlTermsOK
            // 
            this.panelControlTermsOK.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControlTermsOK.Controls.Add(this.btn_TermsOK);
            this.panelControlTermsOK.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControlTermsOK.Location = new System.Drawing.Point(0, 603);
            this.panelControlTermsOK.Name = "panelControlTermsOK";
            this.panelControlTermsOK.Size = new System.Drawing.Size(1000, 70);
            this.panelControlTermsOK.TabIndex = 3;
            // 
            // TermsAndConditions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 673);
            this.Controls.Add(this.panelControlTermsOK);
            this.Controls.Add(this.panelControlTerms);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("TermsAndConditions.IconOptions.Icon")));
            this.IconOptions.ShowIcon = false;
            this.MaximizeBox = false;
            this.Name = "TermsAndConditions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Terms and Conditions";
            this.Load += new System.EventHandler(this.TermsAndConditions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControlTerms)).EndInit();
            this.panelControlTerms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControlTermsOK)).EndInit();
            this.panelControlTermsOK.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox txt_TermsAndConditions;
        private DevExpress.XtraEditors.SimpleButton btn_TermsOK;
        private DevExpress.XtraEditors.PanelControl panelControlTerms;
        private DevExpress.XtraEditors.PanelControl panelControlTermsOK;
    }
}