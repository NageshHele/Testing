
namespace Prime.UI
{
    partial class uc_DeliveryReport
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
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject11 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject12 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc_DeliveryReport));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject13 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject14 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject15 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject16 = new DevExpress.Utils.SerializableAppearanceObject();
            this.gc_DeliveryReport = new DevExpress.XtraGrid.GridControl();
            this.gv_DeliveryReport = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repBtn_VaRDistribution = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repBtn_ClientWindow = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btn_Refresh = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.gc_DeliveryReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_DeliveryReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_VaRDistribution)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_ClientWindow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gc_DeliveryReport
            // 
            this.gc_DeliveryReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_DeliveryReport.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.gc_DeliveryReport.Location = new System.Drawing.Point(0, 46);
            this.gc_DeliveryReport.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gc_DeliveryReport.MainView = this.gv_DeliveryReport;
            this.gc_DeliveryReport.Margin = new System.Windows.Forms.Padding(2);
            this.gc_DeliveryReport.Name = "gc_DeliveryReport";
            this.gc_DeliveryReport.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repBtn_VaRDistribution,
            this.repBtn_ClientWindow});
            this.gc_DeliveryReport.Size = new System.Drawing.Size(881, 402);
            this.gc_DeliveryReport.TabIndex = 2;
            this.gc_DeliveryReport.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_DeliveryReport});
            this.gc_DeliveryReport.ViewRegistered += new DevExpress.XtraGrid.ViewOperationEventHandler(this.gc_DeliveryReport_ViewRegistered);
            // 
            // gv_DeliveryReport
            // 
            this.gv_DeliveryReport.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent;
            this.gv_DeliveryReport.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv_DeliveryReport.Appearance.FooterPanel.Options.UseTextOptions = true;
            this.gv_DeliveryReport.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_DeliveryReport.Appearance.FooterPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_DeliveryReport.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gv_DeliveryReport.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_DeliveryReport.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_DeliveryReport.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_DeliveryReport.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_DeliveryReport.Appearance.HorzLine.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.gv_DeliveryReport.Appearance.HorzLine.BackColor2 = System.Drawing.SystemColors.ActiveBorder;
            this.gv_DeliveryReport.Appearance.HorzLine.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.gv_DeliveryReport.Appearance.HorzLine.Options.UseBackColor = true;
            this.gv_DeliveryReport.Appearance.HorzLine.Options.UseBorderColor = true;
            this.gv_DeliveryReport.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.gv_DeliveryReport.Appearance.Row.Options.UseFont = true;
            this.gv_DeliveryReport.Appearance.Row.Options.UseTextOptions = true;
            this.gv_DeliveryReport.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_DeliveryReport.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_DeliveryReport.Appearance.VertLine.BackColor = System.Drawing.Color.Transparent;
            this.gv_DeliveryReport.Appearance.VertLine.BackColor2 = System.Drawing.Color.Transparent;
            this.gv_DeliveryReport.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_DeliveryReport.DetailHeight = 800;
            this.gv_DeliveryReport.GridControl = this.gc_DeliveryReport;
            this.gv_DeliveryReport.Name = "gv_DeliveryReport";
            this.gv_DeliveryReport.OptionsBehavior.Editable = false;
            this.gv_DeliveryReport.OptionsDetail.ShowDetailTabs = false;
            this.gv_DeliveryReport.OptionsPrint.AllowMultilineHeaders = true;
            this.gv_DeliveryReport.OptionsPrint.AutoWidth = false;
            this.gv_DeliveryReport.OptionsPrint.ExpandAllDetails = true;
            this.gv_DeliveryReport.OptionsPrint.PrintDetails = true;
            this.gv_DeliveryReport.OptionsView.ColumnAutoWidth = false;
            this.gv_DeliveryReport.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_DeliveryReport.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
            this.gv_DeliveryReport.OptionsView.ShowFooter = true;
            this.gv_DeliveryReport.OptionsView.ShowGroupPanel = false;
            this.gv_DeliveryReport.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
            this.gv_DeliveryReport.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
            this.gv_DeliveryReport.RowHeight = 30;
            this.gv_DeliveryReport.CustomDrawFooterCell += new DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventHandler(this.gv_DeliveryReport_CustomDrawFooterCell);
            this.gv_DeliveryReport.MasterRowExpanded += new DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventHandler(this.gv_DeliveryReport_MasterRowExpanded);
            this.gv_DeliveryReport.MasterRowCollapsed += new DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventHandler(this.gv_DeliveryReport_MasterRowCollapsed);
            this.gv_DeliveryReport.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_DeliveryReport_PopupMenuShowing);
            this.gv_DeliveryReport.ColumnUnboundExpressionChanged += new DevExpress.XtraGrid.Views.Base.ColumnEventHandler(this.gv_DeliveryReport_ColumnUnboundExpressionChanged);
            this.gv_DeliveryReport.UnboundExpressionEditorCreated += new DevExpress.XtraGrid.Views.Base.UnboundExpressionEditorEventHandler(this.gv_DeliveryReport_UnboundExpressionEditorCreated);
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
            // btn_Refresh
            // 
            this.btn_Refresh.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Bold);
            this.btn_Refresh.Appearance.Options.UseFont = true;
            this.btn_Refresh.Location = new System.Drawing.Point(5, 5);
            this.btn_Refresh.LookAndFeel.SkinName = "DevExpress Dark Style";
            this.btn_Refresh.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btn_Refresh.Name = "btn_Refresh";
            this.btn_Refresh.Size = new System.Drawing.Size(105, 37);
            this.btn_Refresh.TabIndex = 7;
            this.btn_Refresh.Text = "Refresh";
            this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btn_Refresh);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(881, 46);
            this.panelControl1.TabIndex = 8;
            // 
            // uc_DeliveryReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gc_DeliveryReport);
            this.Controls.Add(this.panelControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "uc_DeliveryReport";
            this.Size = new System.Drawing.Size(881, 448);
            ((System.ComponentModel.ISupportInitialize)(this.gc_DeliveryReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_DeliveryReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_VaRDistribution)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_ClientWindow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gc_DeliveryReport;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_DeliveryReport;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repBtn_VaRDistribution;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repBtn_ClientWindow;
        private DevExpress.XtraEditors.SimpleButton btn_Refresh;
        private DevExpress.XtraEditors.PanelControl panelControl1;
    }
}
