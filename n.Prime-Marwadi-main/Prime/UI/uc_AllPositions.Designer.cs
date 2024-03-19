
namespace Prime.UI
{
    partial class uc_AllPositions
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
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.gc_AllPositions = new DevExpress.XtraGrid.GridControl();
            this.gv_AllPositions = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repBtn_VaRDistribution = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.progressPanel_GetPositions = new DevExpress.XtraWaitForm.ProgressPanel();
            this.timer_AutoSave = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gc_AllPositions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_AllPositions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_VaRDistribution)).BeginInit();
            this.SuspendLayout();
            // 
            // gc_AllPositions
            // 
            this.gc_AllPositions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_AllPositions.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.gc_AllPositions.Location = new System.Drawing.Point(10, 10);
            this.gc_AllPositions.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gc_AllPositions.MainView = this.gv_AllPositions;
            this.gc_AllPositions.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gc_AllPositions.Name = "gc_AllPositions";
            this.gc_AllPositions.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repBtn_VaRDistribution});
            this.gc_AllPositions.Size = new System.Drawing.Size(900, 440);
            this.gc_AllPositions.TabIndex = 2;
            this.gc_AllPositions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_AllPositions});
            this.gc_AllPositions.DataSourceChanged += new System.EventHandler(this.gc_AllPositions_DataSourceChanged);
            // 
            // gv_AllPositions
            // 
            this.gv_AllPositions.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent;
            this.gv_AllPositions.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv_AllPositions.Appearance.FooterPanel.Options.UseTextOptions = true;
            this.gv_AllPositions.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_AllPositions.Appearance.FooterPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_AllPositions.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gv_AllPositions.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_AllPositions.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_AllPositions.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_AllPositions.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_AllPositions.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.gv_AllPositions.Appearance.Row.Options.UseFont = true;
            this.gv_AllPositions.Appearance.Row.Options.UseTextOptions = true;
            this.gv_AllPositions.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_AllPositions.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_AllPositions.Appearance.VertLine.BackColor = System.Drawing.Color.Transparent;
            this.gv_AllPositions.Appearance.VertLine.BackColor2 = System.Drawing.Color.Transparent;
            this.gv_AllPositions.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_AllPositions.DetailHeight = 800;
            this.gv_AllPositions.GridControl = this.gc_AllPositions;
            this.gv_AllPositions.Name = "gv_AllPositions";
            this.gv_AllPositions.OptionsBehavior.Editable = false;
            this.gv_AllPositions.OptionsDetail.ShowDetailTabs = false;
            this.gv_AllPositions.OptionsPrint.AllowMultilineHeaders = true;
            this.gv_AllPositions.OptionsPrint.AutoWidth = false;
            this.gv_AllPositions.OptionsPrint.ExpandAllDetails = true;
            this.gv_AllPositions.OptionsPrint.PrintDetails = true;
            this.gv_AllPositions.OptionsView.ColumnAutoWidth = false;
            this.gv_AllPositions.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_AllPositions.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
            this.gv_AllPositions.OptionsView.ShowFooter = true;
            this.gv_AllPositions.OptionsView.ShowGroupPanel = false;
            this.gv_AllPositions.RowHeight = 30;
            this.gv_AllPositions.CustomDrawFooterCell += new DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventHandler(this.gridView_CustomDrawFooterCell);
            this.gv_AllPositions.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_AllPositions_RowCellStyle);
            this.gv_AllPositions.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_AllPositions_PopupMenuShowing);
            this.gv_AllPositions.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_AllPositions_CustomColumnDisplayText);
            // 
            // repBtn_VaRDistribution
            // 
            this.repBtn_VaRDistribution.AutoHeight = false;
            serializableAppearanceObject1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            serializableAppearanceObject1.Options.UseFont = true;
            this.repBtn_VaRDistribution.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "Calculate", -1, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.repBtn_VaRDistribution.Name = "repBtn_VaRDistribution";
            this.repBtn_VaRDistribution.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // progressPanel_GetPositions
            // 
            this.progressPanel_GetPositions.Appearance.BackColor = System.Drawing.Color.White;
            this.progressPanel_GetPositions.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Bold);
            this.progressPanel_GetPositions.Appearance.Options.UseBackColor = true;
            this.progressPanel_GetPositions.Appearance.Options.UseFont = true;
            this.progressPanel_GetPositions.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI", 12.25F);
            this.progressPanel_GetPositions.AppearanceCaption.Options.UseFont = true;
            this.progressPanel_GetPositions.AppearanceCaption.Options.UseTextOptions = true;
            this.progressPanel_GetPositions.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.progressPanel_GetPositions.AppearanceCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.progressPanel_GetPositions.Caption = "Please wait...";
            this.progressPanel_GetPositions.Description = "";
            this.progressPanel_GetPositions.Location = new System.Drawing.Point(337, 197);
            this.progressPanel_GetPositions.Name = "progressPanel_GetPositions";
            this.progressPanel_GetPositions.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.progressPanel_GetPositions.Size = new System.Drawing.Size(246, 66);
            this.progressPanel_GetPositions.TabIndex = 8;
            this.progressPanel_GetPositions.Visible = false;
            // 
            // timer_AutoSave
            // 
            this.timer_AutoSave.Enabled = true;
            this.timer_AutoSave.Tick += new System.EventHandler(this.timer_AutoSave_Tick);
            // 
            // uc_AllPositions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.progressPanel_GetPositions);
            this.Controls.Add(this.gc_AllPositions);
            this.Name = "uc_AllPositions";
            this.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.Size = new System.Drawing.Size(920, 460);
            ((System.ComponentModel.ISupportInitialize)(this.gc_AllPositions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_AllPositions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtn_VaRDistribution)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gc_AllPositions;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_AllPositions;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repBtn_VaRDistribution;
        private DevExpress.XtraWaitForm.ProgressPanel progressPanel_GetPositions;
        private System.Windows.Forms.Timer timer_AutoSave;
    }
}
