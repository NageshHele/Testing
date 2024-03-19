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
            this.lblLogo = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.checkTerms.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlStart)).BeginInit();
            this.panelControlStart.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_Terms
            // 
            this.lbl_Terms.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.checkTerms.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.panelControlStart.Controls.Add(this.lblLogo);
            this.panelControlStart.Controls.Add(this.lbl_Terms);
            this.panelControlStart.Controls.Add(this.checkTerms);
            this.panelControlStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControlStart.Location = new System.Drawing.Point(0, 0);
            this.panelControlStart.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.panelControlStart.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panelControlStart.Name = "panelControlStart";
            this.panelControlStart.Size = new System.Drawing.Size(521, 345);
            this.panelControlStart.TabIndex = 4;
            // 
            // lblLogo
            // 
            this.lblLogo.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogo.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblLogo.Appearance.Options.UseFont = true;
            this.lblLogo.Appearance.Options.UseForeColor = true;
            this.lblLogo.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblLogo.Location = new System.Drawing.Point(83, 57);
            this.lblLogo.Name = "lblLogo";
            this.lblLogo.Size = new System.Drawing.Size(319, 108);
            this.lblLogo.TabIndex = 4;
            this.lblLogo.Text = "n.prime";
            // 
            // StartPage
            // 
            this.Appearance.BackColor = System.Drawing.Color.Black;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 345);
            this.Controls.Add(this.panelControlStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.Name = "StartPage";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "n.engine";
            ((System.ComponentModel.ISupportInitialize)(this.checkTerms.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlStart)).EndInit();
            this.panelControlStart.ResumeLayout(false);
            this.panelControlStart.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.HyperlinkLabelControl lbl_Terms;
        private DevExpress.XtraEditors.CheckEdit checkTerms;
        private DevExpress.XtraEditors.PanelControl panelControlStart;
        private DevExpress.XtraEditors.LabelControl lblLogo;
    }
}

