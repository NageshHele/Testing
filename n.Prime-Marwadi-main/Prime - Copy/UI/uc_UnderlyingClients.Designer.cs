
namespace Prime.UI
{
    partial class uc_UnderlyingClients
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
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.gc_UnderlyingClients = new DevExpress.XtraGrid.GridControl();
            this.gv_UnderlyingClients = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repBtn_VaRDistribution = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            ((System.ComponentModel.ISupportInitialize)(this.gc_UnderlyingClients)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_UnderlyingClients)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_VaRDistribution)).BeginInit();
            this.SuspendLayout();
            // 
            // gc_UnderlyingClients
            // 
            this.gc_UnderlyingClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_UnderlyingClients.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.gc_UnderlyingClients.Location = new System.Drawing.Point(12, 13);
            this.gc_UnderlyingClients.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gc_UnderlyingClients.MainView = this.gv_UnderlyingClients;
            this.gc_UnderlyingClients.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.gc_UnderlyingClients.Name = "gc_UnderlyingClients";
            this.gc_UnderlyingClients.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repBtn_VaRDistribution});
            this.gc_UnderlyingClients.Size = new System.Drawing.Size(1049, 576);
            this.gc_UnderlyingClients.TabIndex = 2;
            this.gc_UnderlyingClients.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_UnderlyingClients});
            this.gc_UnderlyingClients.DataSourceChanged += new System.EventHandler(this.gc_UnderlyingClients_DataSourceChanged);
            this.gc_UnderlyingClients.ViewRegistered += new DevExpress.XtraGrid.ViewOperationEventHandler(this.gc_UnderlyingClients_ViewRegistered);
            this.gc_UnderlyingClients.DoubleClick += new System.EventHandler(this.gc_UnderlyingClients_DoubleClick);
            this.gc_UnderlyingClients.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gc_UnderlyingClients_KeyDown);
            // 
            // gv_UnderlyingClients
            // 
            this.gv_UnderlyingClients.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent;
            this.gv_UnderlyingClients.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv_UnderlyingClients.Appearance.FooterPanel.Options.UseTextOptions = true;
            this.gv_UnderlyingClients.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_UnderlyingClients.Appearance.FooterPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_UnderlyingClients.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gv_UnderlyingClients.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_UnderlyingClients.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_UnderlyingClients.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_UnderlyingClients.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_UnderlyingClients.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.gv_UnderlyingClients.Appearance.Row.Options.UseFont = true;
            this.gv_UnderlyingClients.Appearance.Row.Options.UseTextOptions = true;
            this.gv_UnderlyingClients.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_UnderlyingClients.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_UnderlyingClients.Appearance.VertLine.BackColor = System.Drawing.Color.Transparent;
            this.gv_UnderlyingClients.Appearance.VertLine.BackColor2 = System.Drawing.Color.Transparent;
            this.gv_UnderlyingClients.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_UnderlyingClients.DetailHeight = 1046;
            this.gv_UnderlyingClients.GridControl = this.gc_UnderlyingClients;
            this.gv_UnderlyingClients.Name = "gv_UnderlyingClients";
            this.gv_UnderlyingClients.OptionsBehavior.Editable = false;
            this.gv_UnderlyingClients.OptionsDetail.ShowDetailTabs = false;
            this.gv_UnderlyingClients.OptionsPrint.AllowMultilineHeaders = true;
            this.gv_UnderlyingClients.OptionsPrint.AutoWidth = false;
            this.gv_UnderlyingClients.OptionsPrint.ExpandAllDetails = true;
            this.gv_UnderlyingClients.OptionsPrint.PrintDetails = true;
            this.gv_UnderlyingClients.OptionsView.ColumnAutoWidth = false;
            this.gv_UnderlyingClients.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_UnderlyingClients.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
            this.gv_UnderlyingClients.OptionsView.ShowFooter = true;
            this.gv_UnderlyingClients.OptionsView.ShowGroupPanel = false;
            this.gv_UnderlyingClients.RowHeight = 39;
            this.gv_UnderlyingClients.CustomDrawFooterCell += new DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventHandler(this.gv_UnderlyingClients_CustomDrawFooterCell);
            this.gv_UnderlyingClients.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_UnderlyingClients_RowCellStyle);
            this.gv_UnderlyingClients.MasterRowExpanded += new DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventHandler(this.gv_UnderlyingClients_MasterRowExpanded);
            this.gv_UnderlyingClients.MasterRowCollapsed += new DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventHandler(this.gv_UnderlyingClients_MasterRowCollapsed);
            this.gv_UnderlyingClients.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_PopupMenuShowing);
            // 
            // repBtn_VaRDistribution
            // 
            this.repBtn_VaRDistribution.AutoHeight = false;
            serializableAppearanceObject5.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            serializableAppearanceObject5.Options.UseFont = true;
            this.repBtn_VaRDistribution.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "Calculate", -1, true, true, false, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.repBtn_VaRDistribution.Name = "repBtn_VaRDistribution";
            this.repBtn_VaRDistribution.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // uc_UnderlyingClients
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gc_UnderlyingClients);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "uc_UnderlyingClients";
            this.Padding = new System.Windows.Forms.Padding(12, 13, 12, 13);
            this.Size = new System.Drawing.Size(1073, 602);
            ((System.ComponentModel.ISupportInitialize)(this.gc_UnderlyingClients)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_UnderlyingClients)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_VaRDistribution)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gc_UnderlyingClients;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_UnderlyingClients;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repBtn_VaRDistribution;
    }
}
