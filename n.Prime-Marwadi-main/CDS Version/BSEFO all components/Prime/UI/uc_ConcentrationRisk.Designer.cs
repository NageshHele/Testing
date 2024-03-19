
namespace Prime.UI
{
    partial class uc_ConcentrationRisk
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
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.PieSeriesLabel pieSeriesLabel1 = new DevExpress.XtraCharts.PieSeriesLabel();
            DevExpress.XtraCharts.PieSeriesView pieSeriesView1 = new DevExpress.XtraCharts.PieSeriesView();
            this.splitContainerControl_Main = new DevExpress.XtraEditors.SplitContainerControl();
            this.gc_ConcentrationRisk = new DevExpress.XtraGrid.GridControl();
            this.gv_ConcentrationRisk = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gc_ChartDetail = new DevExpress.XtraGrid.GridControl();
            this.gv_ChartDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chartControl_ConcentrationRisk = new DevExpress.XtraCharts.ChartControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl_Main)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl_Main.Panel1)).BeginInit();
            this.splitContainerControl_Main.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl_Main.Panel2)).BeginInit();
            this.splitContainerControl_Main.Panel2.SuspendLayout();
            this.splitContainerControl_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gc_ConcentrationRisk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_ConcentrationRisk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gc_ChartDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_ChartDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl_ConcentrationRisk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(pieSeriesLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(pieSeriesView1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl_Main
            // 
            this.splitContainerControl_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl_Main.Location = new System.Drawing.Point(10, 10);
            this.splitContainerControl_Main.Name = "splitContainerControl_Main";
            // 
            // splitContainerControl_Main.Panel1
            // 
            this.splitContainerControl_Main.Panel1.Controls.Add(this.gc_ConcentrationRisk);
            this.splitContainerControl_Main.Panel1.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.splitContainerControl_Main.Panel1.Text = "Panel1";
            // 
            // splitContainerControl_Main.Panel2
            // 
            this.splitContainerControl_Main.Panel2.Controls.Add(this.gc_ChartDetail);
            this.splitContainerControl_Main.Panel2.Controls.Add(this.chartControl_ConcentrationRisk);
            this.splitContainerControl_Main.Panel2.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.splitContainerControl_Main.Panel2.Text = "Panel2";
            this.splitContainerControl_Main.Size = new System.Drawing.Size(900, 440);
            this.splitContainerControl_Main.SplitterPosition = 377;
            this.splitContainerControl_Main.TabIndex = 0;
            // 
            // gc_ConcentrationRisk
            // 
            this.gc_ConcentrationRisk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_ConcentrationRisk.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.gc_ConcentrationRisk.Location = new System.Drawing.Point(10, 10);
            this.gc_ConcentrationRisk.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gc_ConcentrationRisk.MainView = this.gv_ConcentrationRisk;
            this.gc_ConcentrationRisk.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gc_ConcentrationRisk.Name = "gc_ConcentrationRisk";
            this.gc_ConcentrationRisk.Size = new System.Drawing.Size(357, 420);
            this.gc_ConcentrationRisk.TabIndex = 3;
            this.gc_ConcentrationRisk.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_ConcentrationRisk});
            this.gc_ConcentrationRisk.DataSourceChanged += new System.EventHandler(this.gc_ConcentrationRisk_DataSourceChanged);
            this.gc_ConcentrationRisk.DoubleClick += new System.EventHandler(this.gc_ConcentrationRisk_DoubleClick);
            // 
            // gv_ConcentrationRisk
            // 
            this.gv_ConcentrationRisk.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent;
            this.gv_ConcentrationRisk.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv_ConcentrationRisk.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gv_ConcentrationRisk.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_ConcentrationRisk.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_ConcentrationRisk.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_ConcentrationRisk.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_ConcentrationRisk.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.gv_ConcentrationRisk.Appearance.Row.Options.UseFont = true;
            this.gv_ConcentrationRisk.Appearance.Row.Options.UseTextOptions = true;
            this.gv_ConcentrationRisk.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_ConcentrationRisk.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_ConcentrationRisk.Appearance.VertLine.BackColor = System.Drawing.Color.Transparent;
            this.gv_ConcentrationRisk.Appearance.VertLine.BackColor2 = System.Drawing.Color.Transparent;
            this.gv_ConcentrationRisk.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_ConcentrationRisk.GridControl = this.gc_ConcentrationRisk;
            this.gv_ConcentrationRisk.Name = "gv_ConcentrationRisk";
            this.gv_ConcentrationRisk.OptionsBehavior.Editable = false;
            this.gv_ConcentrationRisk.OptionsDetail.ShowDetailTabs = false;
            this.gv_ConcentrationRisk.OptionsPrint.AllowMultilineHeaders = true;
            this.gv_ConcentrationRisk.OptionsPrint.AutoWidth = false;
            this.gv_ConcentrationRisk.OptionsPrint.ExpandAllDetails = true;
            this.gv_ConcentrationRisk.OptionsPrint.PrintDetails = true;
            this.gv_ConcentrationRisk.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_ConcentrationRisk.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
            this.gv_ConcentrationRisk.OptionsView.ShowFooter = true;
            this.gv_ConcentrationRisk.OptionsView.ShowGroupPanel = false;
            this.gv_ConcentrationRisk.OptionsView.ShowIndicator = false;
            this.gv_ConcentrationRisk.RowHeight = 30;
            this.gv_ConcentrationRisk.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gv_ConcentrationRisk_RowClick);
            this.gv_ConcentrationRisk.CustomDrawFooterCell += new DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventHandler(this.gridView_CustomDrawFooterCell);
            // 
            // gc_ChartDetail
            // 
            this.gc_ChartDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_ChartDetail.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.gc_ChartDetail.Location = new System.Drawing.Point(10, 359);
            this.gc_ChartDetail.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gc_ChartDetail.MainView = this.gv_ChartDetail;
            this.gc_ChartDetail.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gc_ChartDetail.Name = "gc_ChartDetail";
            this.gc_ChartDetail.Size = new System.Drawing.Size(493, 71);
            this.gc_ChartDetail.TabIndex = 5;
            this.gc_ChartDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_ChartDetail});
            this.gc_ChartDetail.DataSourceChanged += new System.EventHandler(this.gc_ChartDetail_DataSourceChanged);
            // 
            // gv_ChartDetail
            // 
            this.gv_ChartDetail.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent;
            this.gv_ChartDetail.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv_ChartDetail.Appearance.FooterPanel.Options.UseTextOptions = true;
            this.gv_ChartDetail.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_ChartDetail.Appearance.FooterPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_ChartDetail.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gv_ChartDetail.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_ChartDetail.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_ChartDetail.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_ChartDetail.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_ChartDetail.Appearance.HorzLine.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.gv_ChartDetail.Appearance.HorzLine.BackColor2 = System.Drawing.SystemColors.ActiveBorder;
            this.gv_ChartDetail.Appearance.HorzLine.Options.UseBackColor = true;
            this.gv_ChartDetail.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.gv_ChartDetail.Appearance.Row.Options.UseFont = true;
            this.gv_ChartDetail.Appearance.Row.Options.UseTextOptions = true;
            this.gv_ChartDetail.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_ChartDetail.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_ChartDetail.Appearance.VertLine.BackColor = System.Drawing.Color.Transparent;
            this.gv_ChartDetail.Appearance.VertLine.BackColor2 = System.Drawing.Color.Transparent;
            this.gv_ChartDetail.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_ChartDetail.GridControl = this.gc_ChartDetail;
            this.gv_ChartDetail.Name = "gv_ChartDetail";
            this.gv_ChartDetail.OptionsBehavior.Editable = false;
            this.gv_ChartDetail.OptionsDetail.ShowDetailTabs = false;
            this.gv_ChartDetail.OptionsPrint.AllowMultilineHeaders = true;
            this.gv_ChartDetail.OptionsPrint.AutoWidth = false;
            this.gv_ChartDetail.OptionsPrint.ExpandAllDetails = true;
            this.gv_ChartDetail.OptionsPrint.PrintDetails = true;
            this.gv_ChartDetail.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_ChartDetail.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
            this.gv_ChartDetail.OptionsView.ShowFooter = true;
            this.gv_ChartDetail.OptionsView.ShowGroupPanel = false;
            this.gv_ChartDetail.OptionsView.ShowIndicator = false;
            this.gv_ChartDetail.RowHeight = 30;
            this.gv_ChartDetail.CustomDrawFooterCell += new DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventHandler(this.gv_ChartDetail_CustomDrawFooterCell);
            this.gv_ChartDetail.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_ChartDetail_PopupMenuShowing);
            this.gv_ChartDetail.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_ChartDetail_CustomColumnDisplayText);
            // 
            // chartControl_ConcentrationRisk
            // 
            this.chartControl_ConcentrationRisk.Dock = System.Windows.Forms.DockStyle.Top;
            this.chartControl_ConcentrationRisk.Legend.Name = "Default Legend";
            this.chartControl_ConcentrationRisk.Location = new System.Drawing.Point(10, 10);
            this.chartControl_ConcentrationRisk.Name = "chartControl_ConcentrationRisk";
            pieSeriesLabel1.TextPattern = "{A}:{V:#,#.00}:{VP:p0}";
            series1.Label = pieSeriesLabel1;
            series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            series1.Name = "chartSeries_ConcentrationRisk";
            series1.View = pieSeriesView1;
            this.chartControl_ConcentrationRisk.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1};
            this.chartControl_ConcentrationRisk.Size = new System.Drawing.Size(493, 349);
            this.chartControl_ConcentrationRisk.TabIndex = 1;
            // 
            // uc_ConcentrationRisk
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl_Main);
            this.Name = "uc_ConcentrationRisk";
            this.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.Size = new System.Drawing.Size(920, 460);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl_Main.Panel1)).EndInit();
            this.splitContainerControl_Main.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl_Main.Panel2)).EndInit();
            this.splitContainerControl_Main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl_Main)).EndInit();
            this.splitContainerControl_Main.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gc_ConcentrationRisk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_ConcentrationRisk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gc_ChartDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_ChartDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(pieSeriesLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(pieSeriesView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartControl_ConcentrationRisk)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl_Main;
        private DevExpress.XtraGrid.GridControl gc_ConcentrationRisk;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_ConcentrationRisk;
        private DevExpress.XtraCharts.ChartControl chartControl_ConcentrationRisk;
        private DevExpress.XtraGrid.GridControl gc_ChartDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_ChartDetail;
    }
}
