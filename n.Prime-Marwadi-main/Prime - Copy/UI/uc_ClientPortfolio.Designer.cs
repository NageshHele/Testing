
namespace Prime.UI
{
    partial class uc_ClientPortfolio
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
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject11 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject12 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc_ClientPortfolio));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject13 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject14 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject15 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject16 = new DevExpress.Utils.SerializableAppearanceObject();
            this.gc_ClientPortfolio = new DevExpress.XtraGrid.GridControl();
            this.gv_ClientPortfolio = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repBtn_VaRDistribution = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repBtn_ClientWindow = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.timer_Rule = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gc_ClientPortfolio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_ClientPortfolio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_VaRDistribution)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_ClientWindow)).BeginInit();
            this.SuspendLayout();
            // 
            // gc_ClientPortfolio
            // 
            this.gc_ClientPortfolio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_ClientPortfolio.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.gc_ClientPortfolio.Location = new System.Drawing.Point(10, 10);
            this.gc_ClientPortfolio.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gc_ClientPortfolio.MainView = this.gv_ClientPortfolio;
            this.gc_ClientPortfolio.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gc_ClientPortfolio.Name = "gc_ClientPortfolio";
            this.gc_ClientPortfolio.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repBtn_VaRDistribution,
            this.repBtn_ClientWindow});
            this.gc_ClientPortfolio.Size = new System.Drawing.Size(900, 440);
            this.gc_ClientPortfolio.TabIndex = 1;
            this.gc_ClientPortfolio.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_ClientPortfolio});
            this.gc_ClientPortfolio.DataSourceChanged += new System.EventHandler(this.gc_ClientPortfolio_DataSourceChanged);
            this.gc_ClientPortfolio.ViewRegistered += new DevExpress.XtraGrid.ViewOperationEventHandler(this.gc_ClientPortfolio_ViewRegistered);
            this.gc_ClientPortfolio.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gc_ClientPortfolio_KeyDown);
            // 
            // gv_ClientPortfolio
            // 
            this.gv_ClientPortfolio.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent;
            this.gv_ClientPortfolio.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv_ClientPortfolio.Appearance.FooterPanel.Options.UseTextOptions = true;
            this.gv_ClientPortfolio.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_ClientPortfolio.Appearance.FooterPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_ClientPortfolio.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gv_ClientPortfolio.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_ClientPortfolio.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_ClientPortfolio.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_ClientPortfolio.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_ClientPortfolio.Appearance.HorzLine.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.gv_ClientPortfolio.Appearance.HorzLine.BackColor2 = System.Drawing.SystemColors.ActiveBorder;
            this.gv_ClientPortfolio.Appearance.HorzLine.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.gv_ClientPortfolio.Appearance.HorzLine.Options.UseBackColor = true;
            this.gv_ClientPortfolio.Appearance.HorzLine.Options.UseBorderColor = true;
            this.gv_ClientPortfolio.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.gv_ClientPortfolio.Appearance.Row.Options.UseFont = true;
            this.gv_ClientPortfolio.Appearance.Row.Options.UseTextOptions = true;
            this.gv_ClientPortfolio.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_ClientPortfolio.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_ClientPortfolio.Appearance.VertLine.BackColor = System.Drawing.Color.Transparent;
            this.gv_ClientPortfolio.Appearance.VertLine.BackColor2 = System.Drawing.Color.Transparent;
            this.gv_ClientPortfolio.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_ClientPortfolio.DetailHeight = 800;
            this.gv_ClientPortfolio.GridControl = this.gc_ClientPortfolio;
            this.gv_ClientPortfolio.Name = "gv_ClientPortfolio";
            this.gv_ClientPortfolio.OptionsDetail.ShowDetailTabs = false;
            this.gv_ClientPortfolio.OptionsPrint.AllowMultilineHeaders = true;
            this.gv_ClientPortfolio.OptionsPrint.AutoWidth = false;
            this.gv_ClientPortfolio.OptionsPrint.ExpandAllDetails = true;
            this.gv_ClientPortfolio.OptionsPrint.PrintDetails = true;
            this.gv_ClientPortfolio.OptionsView.ColumnAutoWidth = false;
            this.gv_ClientPortfolio.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_ClientPortfolio.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
            this.gv_ClientPortfolio.OptionsView.ShowFooter = true;
            this.gv_ClientPortfolio.OptionsView.ShowGroupPanel = false;
            this.gv_ClientPortfolio.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
            this.gv_ClientPortfolio.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
            this.gv_ClientPortfolio.RowHeight = 30;
            this.gv_ClientPortfolio.CustomDrawRowFooterCell += new DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventHandler(this.gv_ClientPortfolio_CustomDrawRowFooterCell);
            this.gv_ClientPortfolio.CustomDrawFooterCell += new DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventHandler(this.gridView_CustomDrawFooterCell);
            this.gv_ClientPortfolio.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_ClientPortfolio_RowCellStyle);
            this.gv_ClientPortfolio.MasterRowExpanded += new DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventHandler(this.gv_ClientPortfolio_MasterRowExpanded);
            this.gv_ClientPortfolio.MasterRowCollapsed += new DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventHandler(this.gv_ClientPortfolio_MasterRowCollapsed);
            this.gv_ClientPortfolio.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_ClientPortfolio_CustomRowCellEdit);
            this.gv_ClientPortfolio.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_PopupMenuShowing);
            this.gv_ClientPortfolio.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_ClientPortfolio_ShowingEditor);
            this.gv_ClientPortfolio.EndSorting += new System.EventHandler(this.gv_ClientPortfolio_EndSorting);
            this.gv_ClientPortfolio.ColumnUnboundExpressionChanged += new DevExpress.XtraGrid.Views.Base.ColumnEventHandler(this.gv_ClientPortfolio_ColumnUnboundExpressionChanged);
            this.gv_ClientPortfolio.UnboundExpressionEditorCreated += new DevExpress.XtraGrid.Views.Base.UnboundExpressionEditorEventHandler(this.gv_ClientPortfolio_UnboundExpressionEditorCreated);
            this.gv_ClientPortfolio.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_ClientPortfolio_CustomColumnDisplayText);
            // 
            // repBtn_VaRDistribution
            // 
            this.repBtn_VaRDistribution.AutoHeight = false;
            serializableAppearanceObject9.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            serializableAppearanceObject9.Options.UseFont = true;
            this.repBtn_VaRDistribution.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "Calculate", -1, true, true, false, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, serializableAppearanceObject10, serializableAppearanceObject11, serializableAppearanceObject12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.repBtn_VaRDistribution.Name = "repBtn_VaRDistribution";
            this.repBtn_VaRDistribution.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repBtn_VaRDistribution.Click += new System.EventHandler(this.repBtn_VaRDistribution_Click);
            // 
            // repBtn_ClientWindow
            // 
            this.repBtn_ClientWindow.AutoHeight = false;
            editorButtonImageOptions4.Image = ((System.Drawing.Image)(resources.GetObject("editorButtonImageOptions4.Image")));
            this.repBtn_ClientWindow.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject13, serializableAppearanceObject14, serializableAppearanceObject15, serializableAppearanceObject16, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.repBtn_ClientWindow.Name = "repBtn_ClientWindow";
            this.repBtn_ClientWindow.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // timer_Rule
            // 
            this.timer_Rule.Enabled = true;
            this.timer_Rule.Interval = 30000;
            this.timer_Rule.Tick += new System.EventHandler(this.timer_Rule_Tick);
            // 
            // uc_ClientPortfolio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gc_ClientPortfolio);
            this.Name = "uc_ClientPortfolio";
            this.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.Size = new System.Drawing.Size(920, 460);
            ((System.ComponentModel.ISupportInitialize)(this.gc_ClientPortfolio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_ClientPortfolio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_VaRDistribution)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_ClientWindow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gc_ClientPortfolio;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_ClientPortfolio;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repBtn_VaRDistribution;
        private System.Windows.Forms.Timer timer_Rule;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repBtn_ClientWindow;
    }
}
