
namespace Prime.UI
{
    partial class uc_Violations
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
            this.gc_Violations = new DevExpress.XtraGrid.GridControl();
            this.gv_Violations = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gc_BannedUnderlyings = new DevExpress.XtraGrid.GridControl();
            this.gv_BannedUnderlyings = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lbc_RuleAlert = new DevExpress.XtraEditors.ListBoxControl();
            ((System.ComponentModel.ISupportInitialize)(this.gc_Violations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_Violations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gc_BannedUnderlyings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_BannedUnderlyings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbc_RuleAlert)).BeginInit();
            this.SuspendLayout();
            // 
            // gc_Violations
            // 
            this.gc_Violations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_Violations.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gc_Violations.Location = new System.Drawing.Point(12, 13);
            this.gc_Violations.MainView = this.gv_Violations;
            this.gc_Violations.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gc_Violations.Name = "gc_Violations";
            this.gc_Violations.Size = new System.Drawing.Size(813, 576);
            this.gc_Violations.TabIndex = 2;
            this.gc_Violations.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_Violations});
            this.gc_Violations.DataSourceChanged += new System.EventHandler(this.gc_Violations_DataSourceChanged);
            // 
            // gv_Violations
            // 
            this.gv_Violations.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.gv_Violations.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_Violations.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_Violations.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_Violations.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_Violations.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.gv_Violations.Appearance.Row.Options.UseFont = true;
            this.gv_Violations.Appearance.Row.Options.UseTextOptions = true;
            this.gv_Violations.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_Violations.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_Violations.Appearance.VertLine.BackColor = System.Drawing.Color.Transparent;
            this.gv_Violations.Appearance.VertLine.BackColor2 = System.Drawing.Color.Transparent;
            this.gv_Violations.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_Violations.DetailHeight = 458;
            this.gv_Violations.GridControl = this.gc_Violations;
            this.gv_Violations.Name = "gv_Violations";
            this.gv_Violations.OptionsBehavior.Editable = false;
            this.gv_Violations.OptionsDetail.ShowDetailTabs = false;
            this.gv_Violations.OptionsView.ColumnAutoWidth = false;
            this.gv_Violations.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_Violations.OptionsView.ShowGroupPanel = false;
            this.gv_Violations.OptionsView.ShowIndicator = false;
            this.gv_Violations.RowHeight = 39;
            this.gv_Violations.CustomDrawFooterCell += new DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventHandler(this.gridView_CustomDrawFooterCell);
            this.gv_Violations.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Violations_PopupMenuShowing);
            // 
            // gc_BannedUnderlyings
            // 
            this.gc_BannedUnderlyings.Dock = System.Windows.Forms.DockStyle.Right;
            this.gc_BannedUnderlyings.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gc_BannedUnderlyings.Location = new System.Drawing.Point(825, 13);
            this.gc_BannedUnderlyings.MainView = this.gv_BannedUnderlyings;
            this.gc_BannedUnderlyings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gc_BannedUnderlyings.MaximumSize = new System.Drawing.Size(236, 0);
            this.gc_BannedUnderlyings.Name = "gc_BannedUnderlyings";
            this.gc_BannedUnderlyings.Size = new System.Drawing.Size(236, 576);
            this.gc_BannedUnderlyings.TabIndex = 3;
            this.gc_BannedUnderlyings.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_BannedUnderlyings});
            this.gc_BannedUnderlyings.DataSourceChanged += new System.EventHandler(this.gc_BannedUnderlyings_DataSourceChanged);
            // 
            // gv_BannedUnderlyings
            // 
            this.gv_BannedUnderlyings.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.gv_BannedUnderlyings.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_BannedUnderlyings.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_BannedUnderlyings.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_BannedUnderlyings.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_BannedUnderlyings.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.gv_BannedUnderlyings.Appearance.Row.Options.UseFont = true;
            this.gv_BannedUnderlyings.Appearance.Row.Options.UseTextOptions = true;
            this.gv_BannedUnderlyings.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_BannedUnderlyings.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_BannedUnderlyings.Appearance.VertLine.BackColor = System.Drawing.Color.Transparent;
            this.gv_BannedUnderlyings.Appearance.VertLine.BackColor2 = System.Drawing.Color.Transparent;
            this.gv_BannedUnderlyings.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_BannedUnderlyings.DetailHeight = 458;
            this.gv_BannedUnderlyings.GridControl = this.gc_BannedUnderlyings;
            this.gv_BannedUnderlyings.Name = "gv_BannedUnderlyings";
            this.gv_BannedUnderlyings.OptionsBehavior.Editable = false;
            this.gv_BannedUnderlyings.OptionsDetail.ShowDetailTabs = false;
            this.gv_BannedUnderlyings.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_BannedUnderlyings.OptionsView.ShowGroupPanel = false;
            this.gv_BannedUnderlyings.OptionsView.ShowIndicator = false;
            this.gv_BannedUnderlyings.RowHeight = 39;
            // 
            // lbc_RuleAlert
            // 
            this.lbc_RuleAlert.Appearance.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbc_RuleAlert.Appearance.Options.UseFont = true;
            this.lbc_RuleAlert.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbc_RuleAlert.Location = new System.Drawing.Point(12, 311);
            this.lbc_RuleAlert.Name = "lbc_RuleAlert";
            this.lbc_RuleAlert.Size = new System.Drawing.Size(813, 278);
            this.lbc_RuleAlert.TabIndex = 5;
            this.lbc_RuleAlert.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.lbc_RuleAlert_DrawItem_1);
            // 
            // uc_Violations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbc_RuleAlert);
            this.Controls.Add(this.gc_Violations);
            this.Controls.Add(this.gc_BannedUnderlyings);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "uc_Violations";
            this.Padding = new System.Windows.Forms.Padding(12, 13, 12, 13);
            this.Size = new System.Drawing.Size(1073, 602);
            ((System.ComponentModel.ISupportInitialize)(this.gc_Violations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_Violations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gc_BannedUnderlyings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_BannedUnderlyings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbc_RuleAlert)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gc_Violations;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_Violations;
        private DevExpress.XtraGrid.GridControl gc_BannedUnderlyings;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_BannedUnderlyings;
        private DevExpress.XtraEditors.ListBoxControl lbc_RuleAlert;
    }
}
