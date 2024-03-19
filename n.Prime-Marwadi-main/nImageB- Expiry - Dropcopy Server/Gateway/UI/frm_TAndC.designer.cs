namespace Gateway
{
    partial class frm_TAndC
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
            this.btn_OKTerms = new DevExpress.XtraEditors.SimpleButton();
            this.lbl_TermsAndConditions = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btn_OKTerms
            // 
            this.btn_OKTerms.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_OKTerms.Appearance.Options.UseFont = true;
            this.btn_OKTerms.Location = new System.Drawing.Point(456, 332);
            this.btn_OKTerms.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_OKTerms.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_OKTerms.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_OKTerms.Name = "btn_OKTerms";
            this.btn_OKTerms.Size = new System.Drawing.Size(81, 26);
            this.btn_OKTerms.TabIndex = 5;
            this.btn_OKTerms.Text = "OK";
            this.btn_OKTerms.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // lbl_TermsAndConditions
            // 
            this.lbl_TermsAndConditions.Location = new System.Drawing.Point(12, 12);
            this.lbl_TermsAndConditions.Name = "lbl_TermsAndConditions";
            this.lbl_TermsAndConditions.Size = new System.Drawing.Size(525, 312);
            this.lbl_TermsAndConditions.TabIndex = 6;
            this.lbl_TermsAndConditions.Text = "";
            // 
            // TermsAndConditions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(550, 364);
            this.Controls.Add(this.lbl_TermsAndConditions);
            this.Controls.Add(this.btn_OKTerms);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TermsAndConditions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Terms & Conditions";
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton btn_OKTerms;
        private System.Windows.Forms.RichTextBox lbl_TermsAndConditions;
    }
}