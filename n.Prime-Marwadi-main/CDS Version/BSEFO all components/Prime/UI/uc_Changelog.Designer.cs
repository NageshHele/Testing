
namespace Prime.UI
{
    partial class uc_Changelog
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gc_Changelog = new DevExpress.XtraGrid.GridControl();
            this.gv_Changelog = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.gc_Changelog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_Changelog)).BeginInit();
            this.SuspendLayout();
            // 
            // gc_Changelog
            // 
            this.gc_Changelog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_Changelog.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.gc_Changelog.Location = new System.Drawing.Point(0, 0);
            this.gc_Changelog.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gc_Changelog.MainView = this.gv_Changelog;
            this.gc_Changelog.Margin = new System.Windows.Forms.Padding(2);
            this.gc_Changelog.Name = "gc_Changelog";
            this.gc_Changelog.Size = new System.Drawing.Size(655, 221);
            this.gc_Changelog.TabIndex = 4;
            this.gc_Changelog.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_Changelog});
            this.gc_Changelog.DataSourceChanged += new System.EventHandler(this.gc_Changelog_DataSourceChanged);
            // 
            // gv_Changelog
            // 
            this.gv_Changelog.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent;
            this.gv_Changelog.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv_Changelog.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gv_Changelog.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_Changelog.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_Changelog.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_Changelog.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_Changelog.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.gv_Changelog.Appearance.Row.Options.UseFont = true;
            this.gv_Changelog.Appearance.Row.Options.UseTextOptions = true;
            this.gv_Changelog.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gv_Changelog.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_Changelog.Appearance.VertLine.BackColor = System.Drawing.Color.Transparent;
            this.gv_Changelog.Appearance.VertLine.BackColor2 = System.Drawing.Color.Transparent;
            this.gv_Changelog.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_Changelog.GridControl = this.gc_Changelog;
            this.gv_Changelog.GroupFormat = "[#image]{1} {2}";
            this.gv_Changelog.Name = "gv_Changelog";
            this.gv_Changelog.OptionsBehavior.Editable = false;
            this.gv_Changelog.OptionsDetail.ShowDetailTabs = false;
            this.gv_Changelog.OptionsPrint.AllowMultilineHeaders = true;
            this.gv_Changelog.OptionsPrint.AutoWidth = false;
            this.gv_Changelog.OptionsPrint.ExpandAllDetails = true;
            this.gv_Changelog.OptionsPrint.PrintDetails = true;
            this.gv_Changelog.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_Changelog.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
            this.gv_Changelog.OptionsView.ShowColumnHeaders = false;
            this.gv_Changelog.OptionsView.ShowGroupPanel = false;
            this.gv_Changelog.OptionsView.ShowIndicator = false;
            this.gv_Changelog.RowHeight = 15;
            // 
            // uc_Changelog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gc_Changelog);
            this.Name = "uc_Changelog";
            this.Size = new System.Drawing.Size(655, 221);
            ((System.ComponentModel.ISupportInitialize)(this.gc_Changelog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_Changelog)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gc_Changelog;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_Changelog;
    }
}
