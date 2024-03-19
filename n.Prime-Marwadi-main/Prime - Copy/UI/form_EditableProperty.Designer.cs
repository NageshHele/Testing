
namespace Prime.UI
{
    partial class form_EditableProperty
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
            this.btn_Save = new DevExpress.XtraEditors.SimpleButton();
            this.lbl_Default = new DevExpress.XtraEditors.LabelControl();
            this.txt_Val = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Val.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Save
            // 
            this.btn_Save.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btn_Save.Appearance.Options.UseFont = true;
            this.btn_Save.Location = new System.Drawing.Point(188, 59);
            this.btn_Save.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_Save.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(104, 24);
            this.btn_Save.TabIndex = 71;
            this.btn_Save.Text = "Save";
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // lbl_Default
            // 
            this.lbl_Default.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Default.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lbl_Default.Appearance.Options.UseFont = true;
            this.lbl_Default.Appearance.Options.UseForeColor = true;
            this.lbl_Default.Appearance.Options.UseTextOptions = true;
            this.lbl_Default.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lbl_Default.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lbl_Default.Location = new System.Drawing.Point(11, 21);
            this.lbl_Default.Name = "lbl_Default";
            this.lbl_Default.Size = new System.Drawing.Size(131, 17);
            this.lbl_Default.TabIndex = 70;
            this.lbl_Default.Text = "Default Label Text : ";
            // 
            // txt_Val
            // 
            this.txt_Val.Location = new System.Drawing.Point(148, 20);
            this.txt_Val.Name = "txt_Val";
            this.txt_Val.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.25F);
            this.txt_Val.Properties.Appearance.Options.UseFont = true;
            this.txt_Val.Properties.BeepOnError = false;
            this.txt_Val.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.txt_Val.Properties.MaskSettings.Set("mask", "n");
            this.txt_Val.Properties.MaskSettings.Set("valueAfterDelete", DevExpress.Data.Mask.NumericMaskManager.ValueAfterDelete.ZeroThenNull);
            this.txt_Val.Properties.MaskSettings.Set("culture", "hi-IN");
            this.txt_Val.Properties.UseMaskAsDisplayFormat = true;
            this.txt_Val.Size = new System.Drawing.Size(144, 22);
            this.txt_Val.TabIndex = 72;
            // 
            // EditableProperty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 95);
            this.Controls.Add(this.txt_Val);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.lbl_Default);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.ShowIcon = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditableProperty";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit";
            ((System.ComponentModel.ISupportInitialize)(this.txt_Val.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btn_Save;
        private DevExpress.XtraEditors.LabelControl lbl_Default;
        private DevExpress.XtraEditors.TextEdit txt_Val;
    }
}