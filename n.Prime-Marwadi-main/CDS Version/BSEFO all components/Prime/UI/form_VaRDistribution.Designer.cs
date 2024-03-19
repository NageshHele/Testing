using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Prime
{
    partial class form_VaRDistribution
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
            DevExpress.XtraCharts.XYDiagram xyDiagram1 = new DevExpress.XtraCharts.XYDiagram();
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series();
            DevExpress.XtraCharts.SplineSeriesView splineSeriesView1 = new DevExpress.XtraCharts.SplineSeriesView();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_VaRDistribution));
            this.gc_VaRPortfolio = new DevExpress.XtraGrid.GridControl();
            this.gv_VaRPortfolio = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn16 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn17 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn18 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn19 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn20 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn21 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn22 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn23 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn24 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn25 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn26 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn27 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn28 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn29 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn30 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn31 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn32 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn33 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn34 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn35 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn36 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn37 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn38 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn39 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn40 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn41 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn42 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn43 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn44 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn45 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn46 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn47 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn48 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn49 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn50 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn51 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn52 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn53 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn54 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn55 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn56 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn57 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn58 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn59 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn60 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn61 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn62 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.chart_VaRDistribution = new DevExpress.XtraCharts.ChartControl();
            this.scVarDistribution = new System.Windows.Forms.SplitContainer();
            this.groupC_VarDistribution = new DevExpress.XtraEditors.GroupControl();
            this.btn_Refresh = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gc_VaRPortfolio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_VaRPortfolio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_VaRDistribution)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(splineSeriesView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scVarDistribution)).BeginInit();
            this.scVarDistribution.Panel1.SuspendLayout();
            this.scVarDistribution.Panel2.SuspendLayout();
            this.scVarDistribution.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupC_VarDistribution)).BeginInit();
            this.groupC_VarDistribution.SuspendLayout();
            this.SuspendLayout();
            // 
            // gc_VaRPortfolio
            // 
            this.gc_VaRPortfolio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_VaRPortfolio.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gc_VaRPortfolio.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gc_VaRPortfolio.Location = new System.Drawing.Point(2, 29);
            this.gc_VaRPortfolio.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gc_VaRPortfolio.MainView = this.gv_VaRPortfolio;
            this.gc_VaRPortfolio.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gc_VaRPortfolio.Name = "gc_VaRPortfolio";
            this.gc_VaRPortfolio.Size = new System.Drawing.Size(1341, 349);
            this.gc_VaRPortfolio.TabIndex = 0;
            this.gc_VaRPortfolio.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_VaRPortfolio});
            this.gc_VaRPortfolio.ViewRegistered += new DevExpress.XtraGrid.ViewOperationEventHandler(this.gc_VaRPortfolio_ViewRegistered);
            // 
            // gv_VaRPortfolio
            // 
            this.gv_VaRPortfolio.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gv_VaRPortfolio.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_VaRPortfolio.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_VaRPortfolio.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_VaRPortfolio.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_VaRPortfolio.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gv_VaRPortfolio.Appearance.Row.Options.UseFont = true;
            this.gv_VaRPortfolio.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_VaRPortfolio.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_VaRPortfolio.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10,
            this.gridColumn11,
            this.gridColumn12,
            this.gridColumn13,
            this.gridColumn14,
            this.gridColumn15,
            this.gridColumn16,
            this.gridColumn17,
            this.gridColumn18,
            this.gridColumn19,
            this.gridColumn20,
            this.gridColumn21,
            this.gridColumn22,
            this.gridColumn23,
            this.gridColumn24,
            this.gridColumn25,
            this.gridColumn26,
            this.gridColumn27,
            this.gridColumn28,
            this.gridColumn29,
            this.gridColumn30,
            this.gridColumn31,
            this.gridColumn32,
            this.gridColumn33,
            this.gridColumn34,
            this.gridColumn35,
            this.gridColumn36,
            this.gridColumn37,
            this.gridColumn38,
            this.gridColumn39,
            this.gridColumn40,
            this.gridColumn41,
            this.gridColumn42,
            this.gridColumn43,
            this.gridColumn44,
            this.gridColumn45,
            this.gridColumn46,
            this.gridColumn47,
            this.gridColumn48,
            this.gridColumn49,
            this.gridColumn50,
            this.gridColumn51,
            this.gridColumn52,
            this.gridColumn53,
            this.gridColumn54,
            this.gridColumn55,
            this.gridColumn56,
            this.gridColumn57,
            this.gridColumn58,
            this.gridColumn59,
            this.gridColumn60,
            this.gridColumn61,
            this.gridColumn62});
            this.gv_VaRPortfolio.DetailHeight = 458;
            this.gv_VaRPortfolio.GridControl = this.gc_VaRPortfolio;
            this.gv_VaRPortfolio.Name = "gv_VaRPortfolio";
            this.gv_VaRPortfolio.OptionsBehavior.Editable = false;
            this.gv_VaRPortfolio.OptionsBehavior.ReadOnly = true;
            this.gv_VaRPortfolio.OptionsView.ColumnAutoWidth = false;
            this.gv_VaRPortfolio.OptionsView.ShowFooter = true;
            this.gv_VaRPortfolio.OptionsView.ShowGroupPanel = false;
            this.gv_VaRPortfolio.OptionsView.ShowIndicator = false;
            this.gv_VaRPortfolio.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.Grdvw_VarPortfolio_RowCellStyle);
            this.gv_VaRPortfolio.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_VaRPortfolio_PopupMenuShowing);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Underlying";
            this.gridColumn1.FieldName = "Underlying";
            this.gridColumn1.MinWidth = 23;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 87;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "-30%";
            this.gridColumn2.DisplayFormat.FormatString = "N2";
            this.gridColumn2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn2.FieldName = "-30";
            this.gridColumn2.MinWidth = 23;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-30", "{0:N2}")});
            this.gridColumn2.Width = 87;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "-29%";
            this.gridColumn3.DisplayFormat.FormatString = "N2";
            this.gridColumn3.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn3.FieldName = "-29";
            this.gridColumn3.MinWidth = 23;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-29", "{0:N2}")});
            this.gridColumn3.Width = 87;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "-28%";
            this.gridColumn4.DisplayFormat.FormatString = "N2";
            this.gridColumn4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn4.FieldName = "-28";
            this.gridColumn4.MinWidth = 23;
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-28", "{0:N2}")});
            this.gridColumn4.Width = 87;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "-27%";
            this.gridColumn5.DisplayFormat.FormatString = "N2";
            this.gridColumn5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn5.FieldName = "-27";
            this.gridColumn5.MinWidth = 23;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-27", "{0:N2}")});
            this.gridColumn5.Width = 87;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "-26%";
            this.gridColumn6.DisplayFormat.FormatString = "N2";
            this.gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn6.FieldName = "-26";
            this.gridColumn6.MinWidth = 23;
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-26", "{0:N2}")});
            this.gridColumn6.Width = 87;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "-25%";
            this.gridColumn7.DisplayFormat.FormatString = "N2";
            this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn7.FieldName = "-25";
            this.gridColumn7.MinWidth = 23;
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-25", "{0:N2}")});
            this.gridColumn7.Width = 87;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "-24%";
            this.gridColumn8.DisplayFormat.FormatString = "N2";
            this.gridColumn8.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn8.FieldName = "-24";
            this.gridColumn8.MinWidth = 23;
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-24", "{0:N2}")});
            this.gridColumn8.Width = 87;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "-23%";
            this.gridColumn9.DisplayFormat.FormatString = "N2";
            this.gridColumn9.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn9.FieldName = "-23";
            this.gridColumn9.MinWidth = 23;
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-23", "{0:N2}")});
            this.gridColumn9.Width = 87;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "-22%";
            this.gridColumn10.DisplayFormat.FormatString = "N2";
            this.gridColumn10.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn10.FieldName = "-22";
            this.gridColumn10.MinWidth = 23;
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-22", "{0:N2}")});
            this.gridColumn10.Width = 87;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "-21%";
            this.gridColumn11.DisplayFormat.FormatString = "N2";
            this.gridColumn11.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn11.FieldName = "-21";
            this.gridColumn11.MinWidth = 23;
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-21", "{0:N2}")});
            this.gridColumn11.Width = 87;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "-20%";
            this.gridColumn12.DisplayFormat.FormatString = "N2";
            this.gridColumn12.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn12.FieldName = "-20";
            this.gridColumn12.MinWidth = 23;
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-20", "{0:N2}")});
            this.gridColumn12.Width = 87;
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "-19%";
            this.gridColumn13.DisplayFormat.FormatString = "N2";
            this.gridColumn13.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn13.FieldName = "-19";
            this.gridColumn13.MinWidth = 23;
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-19", "{0:N2}")});
            this.gridColumn13.Width = 87;
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "-18%";
            this.gridColumn14.DisplayFormat.FormatString = "N2";
            this.gridColumn14.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn14.FieldName = "-18";
            this.gridColumn14.MinWidth = 23;
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-18", "{0:N2}")});
            this.gridColumn14.Width = 87;
            // 
            // gridColumn15
            // 
            this.gridColumn15.Caption = "-17%";
            this.gridColumn15.DisplayFormat.FormatString = "N2";
            this.gridColumn15.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn15.FieldName = "-17";
            this.gridColumn15.MinWidth = 23;
            this.gridColumn15.Name = "gridColumn15";
            this.gridColumn15.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-17", "{0:N2}")});
            this.gridColumn15.Width = 87;
            // 
            // gridColumn16
            // 
            this.gridColumn16.Caption = "-16%";
            this.gridColumn16.DisplayFormat.FormatString = "N2";
            this.gridColumn16.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn16.FieldName = "-16";
            this.gridColumn16.MinWidth = 23;
            this.gridColumn16.Name = "gridColumn16";
            this.gridColumn16.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-16", "{0:N2}")});
            this.gridColumn16.Width = 87;
            // 
            // gridColumn17
            // 
            this.gridColumn17.Caption = "-15%";
            this.gridColumn17.DisplayFormat.FormatString = "N2";
            this.gridColumn17.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn17.FieldName = "-15";
            this.gridColumn17.MinWidth = 23;
            this.gridColumn17.Name = "gridColumn17";
            this.gridColumn17.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-15", "{0:N2}")});
            this.gridColumn17.Visible = true;
            this.gridColumn17.VisibleIndex = 1;
            this.gridColumn17.Width = 87;
            // 
            // gridColumn18
            // 
            this.gridColumn18.Caption = "-14%";
            this.gridColumn18.DisplayFormat.FormatString = "N2";
            this.gridColumn18.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn18.FieldName = "-14";
            this.gridColumn18.MinWidth = 23;
            this.gridColumn18.Name = "gridColumn18";
            this.gridColumn18.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-14", "{0:N2}")});
            this.gridColumn18.Visible = true;
            this.gridColumn18.VisibleIndex = 2;
            this.gridColumn18.Width = 87;
            // 
            // gridColumn19
            // 
            this.gridColumn19.Caption = "-13%";
            this.gridColumn19.DisplayFormat.FormatString = "N2";
            this.gridColumn19.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn19.FieldName = "-13";
            this.gridColumn19.MinWidth = 23;
            this.gridColumn19.Name = "gridColumn19";
            this.gridColumn19.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-13", "{0:N2}")});
            this.gridColumn19.Visible = true;
            this.gridColumn19.VisibleIndex = 3;
            this.gridColumn19.Width = 87;
            // 
            // gridColumn20
            // 
            this.gridColumn20.Caption = "-12%";
            this.gridColumn20.DisplayFormat.FormatString = "N2";
            this.gridColumn20.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn20.FieldName = "-12";
            this.gridColumn20.MinWidth = 23;
            this.gridColumn20.Name = "gridColumn20";
            this.gridColumn20.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-12", "{0:N2}")});
            this.gridColumn20.Visible = true;
            this.gridColumn20.VisibleIndex = 4;
            this.gridColumn20.Width = 87;
            // 
            // gridColumn21
            // 
            this.gridColumn21.Caption = "-11%";
            this.gridColumn21.DisplayFormat.FormatString = "N2";
            this.gridColumn21.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn21.FieldName = "-11";
            this.gridColumn21.MinWidth = 23;
            this.gridColumn21.Name = "gridColumn21";
            this.gridColumn21.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-11", "{0:N2}")});
            this.gridColumn21.Visible = true;
            this.gridColumn21.VisibleIndex = 5;
            this.gridColumn21.Width = 87;
            // 
            // gridColumn22
            // 
            this.gridColumn22.Caption = "-10%";
            this.gridColumn22.DisplayFormat.FormatString = "N2";
            this.gridColumn22.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn22.FieldName = "-10";
            this.gridColumn22.MinWidth = 23;
            this.gridColumn22.Name = "gridColumn22";
            this.gridColumn22.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-10", "{0:N2}")});
            this.gridColumn22.Visible = true;
            this.gridColumn22.VisibleIndex = 6;
            this.gridColumn22.Width = 87;
            // 
            // gridColumn23
            // 
            this.gridColumn23.Caption = "-9%";
            this.gridColumn23.DisplayFormat.FormatString = "N2";
            this.gridColumn23.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn23.FieldName = "-9";
            this.gridColumn23.MinWidth = 23;
            this.gridColumn23.Name = "gridColumn23";
            this.gridColumn23.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-9", "{0:N2}")});
            this.gridColumn23.Visible = true;
            this.gridColumn23.VisibleIndex = 7;
            this.gridColumn23.Width = 87;
            // 
            // gridColumn24
            // 
            this.gridColumn24.Caption = "-8%";
            this.gridColumn24.DisplayFormat.FormatString = "N2";
            this.gridColumn24.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn24.FieldName = "-8";
            this.gridColumn24.MinWidth = 23;
            this.gridColumn24.Name = "gridColumn24";
            this.gridColumn24.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-8", "{0:N2}")});
            this.gridColumn24.Visible = true;
            this.gridColumn24.VisibleIndex = 8;
            this.gridColumn24.Width = 87;
            // 
            // gridColumn25
            // 
            this.gridColumn25.Caption = "-7%";
            this.gridColumn25.DisplayFormat.FormatString = "N2";
            this.gridColumn25.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn25.FieldName = "-7";
            this.gridColumn25.MinWidth = 23;
            this.gridColumn25.Name = "gridColumn25";
            this.gridColumn25.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-7", "{0:N2}")});
            this.gridColumn25.Visible = true;
            this.gridColumn25.VisibleIndex = 9;
            this.gridColumn25.Width = 87;
            // 
            // gridColumn26
            // 
            this.gridColumn26.Caption = "-6%";
            this.gridColumn26.DisplayFormat.FormatString = "N2";
            this.gridColumn26.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn26.FieldName = "-6";
            this.gridColumn26.MinWidth = 23;
            this.gridColumn26.Name = "gridColumn26";
            this.gridColumn26.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-6", "{0:N2}")});
            this.gridColumn26.Visible = true;
            this.gridColumn26.VisibleIndex = 10;
            this.gridColumn26.Width = 87;
            // 
            // gridColumn27
            // 
            this.gridColumn27.Caption = "-5%";
            this.gridColumn27.DisplayFormat.FormatString = "N2";
            this.gridColumn27.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn27.FieldName = "-5";
            this.gridColumn27.MinWidth = 23;
            this.gridColumn27.Name = "gridColumn27";
            this.gridColumn27.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-5", "{0:N2}")});
            this.gridColumn27.Visible = true;
            this.gridColumn27.VisibleIndex = 11;
            this.gridColumn27.Width = 87;
            // 
            // gridColumn28
            // 
            this.gridColumn28.Caption = "-4%";
            this.gridColumn28.DisplayFormat.FormatString = "N2";
            this.gridColumn28.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn28.FieldName = "-4";
            this.gridColumn28.MinWidth = 23;
            this.gridColumn28.Name = "gridColumn28";
            this.gridColumn28.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-4", "{0:N2}")});
            this.gridColumn28.Visible = true;
            this.gridColumn28.VisibleIndex = 12;
            this.gridColumn28.Width = 87;
            // 
            // gridColumn29
            // 
            this.gridColumn29.Caption = "-3%";
            this.gridColumn29.DisplayFormat.FormatString = "N2";
            this.gridColumn29.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn29.FieldName = "-3";
            this.gridColumn29.MinWidth = 23;
            this.gridColumn29.Name = "gridColumn29";
            this.gridColumn29.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-3", "{0:N2}")});
            this.gridColumn29.Visible = true;
            this.gridColumn29.VisibleIndex = 13;
            this.gridColumn29.Width = 87;
            // 
            // gridColumn30
            // 
            this.gridColumn30.Caption = "-2%";
            this.gridColumn30.DisplayFormat.FormatString = "N2";
            this.gridColumn30.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn30.FieldName = "-2";
            this.gridColumn30.MinWidth = 23;
            this.gridColumn30.Name = "gridColumn30";
            this.gridColumn30.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-2", "{0:N2}")});
            this.gridColumn30.Visible = true;
            this.gridColumn30.VisibleIndex = 14;
            this.gridColumn30.Width = 87;
            // 
            // gridColumn31
            // 
            this.gridColumn31.Caption = "-1%";
            this.gridColumn31.DisplayFormat.FormatString = "N2";
            this.gridColumn31.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn31.FieldName = "-1";
            this.gridColumn31.MinWidth = 23;
            this.gridColumn31.Name = "gridColumn31";
            this.gridColumn31.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "-1", "{0:N2}")});
            this.gridColumn31.Visible = true;
            this.gridColumn31.VisibleIndex = 15;
            this.gridColumn31.Width = 87;
            // 
            // gridColumn32
            // 
            this.gridColumn32.Caption = "0%";
            this.gridColumn32.DisplayFormat.FormatString = "N2";
            this.gridColumn32.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn32.FieldName = "0";
            this.gridColumn32.MinWidth = 23;
            this.gridColumn32.Name = "gridColumn32";
            this.gridColumn32.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "0", "{0:N2}")});
            this.gridColumn32.Visible = true;
            this.gridColumn32.VisibleIndex = 16;
            this.gridColumn32.Width = 87;
            // 
            // gridColumn33
            // 
            this.gridColumn33.Caption = "1%";
            this.gridColumn33.DisplayFormat.FormatString = "N2";
            this.gridColumn33.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn33.FieldName = "1";
            this.gridColumn33.MinWidth = 23;
            this.gridColumn33.Name = "gridColumn33";
            this.gridColumn33.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "1", "{0:N2}")});
            this.gridColumn33.Visible = true;
            this.gridColumn33.VisibleIndex = 17;
            this.gridColumn33.Width = 87;
            // 
            // gridColumn34
            // 
            this.gridColumn34.Caption = "2%";
            this.gridColumn34.DisplayFormat.FormatString = "N2";
            this.gridColumn34.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn34.FieldName = "2";
            this.gridColumn34.MinWidth = 23;
            this.gridColumn34.Name = "gridColumn34";
            this.gridColumn34.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "2", "{0:N2}")});
            this.gridColumn34.Visible = true;
            this.gridColumn34.VisibleIndex = 18;
            this.gridColumn34.Width = 87;
            // 
            // gridColumn35
            // 
            this.gridColumn35.Caption = "3%";
            this.gridColumn35.DisplayFormat.FormatString = "N2";
            this.gridColumn35.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn35.FieldName = "3";
            this.gridColumn35.MinWidth = 23;
            this.gridColumn35.Name = "gridColumn35";
            this.gridColumn35.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "3", "{0:N2}")});
            this.gridColumn35.Visible = true;
            this.gridColumn35.VisibleIndex = 19;
            this.gridColumn35.Width = 87;
            // 
            // gridColumn36
            // 
            this.gridColumn36.Caption = "4%";
            this.gridColumn36.DisplayFormat.FormatString = "N2";
            this.gridColumn36.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn36.FieldName = "4";
            this.gridColumn36.MinWidth = 23;
            this.gridColumn36.Name = "gridColumn36";
            this.gridColumn36.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "4", "{0:N2}")});
            this.gridColumn36.Visible = true;
            this.gridColumn36.VisibleIndex = 20;
            this.gridColumn36.Width = 87;
            // 
            // gridColumn37
            // 
            this.gridColumn37.Caption = "5%";
            this.gridColumn37.DisplayFormat.FormatString = "N2";
            this.gridColumn37.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn37.FieldName = "5";
            this.gridColumn37.MinWidth = 23;
            this.gridColumn37.Name = "gridColumn37";
            this.gridColumn37.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "5", "{0:N2}")});
            this.gridColumn37.Visible = true;
            this.gridColumn37.VisibleIndex = 21;
            this.gridColumn37.Width = 87;
            // 
            // gridColumn38
            // 
            this.gridColumn38.Caption = "6%";
            this.gridColumn38.DisplayFormat.FormatString = "N2";
            this.gridColumn38.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn38.FieldName = "6";
            this.gridColumn38.MinWidth = 23;
            this.gridColumn38.Name = "gridColumn38";
            this.gridColumn38.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "6", "{0:N2}")});
            this.gridColumn38.Visible = true;
            this.gridColumn38.VisibleIndex = 22;
            this.gridColumn38.Width = 87;
            // 
            // gridColumn39
            // 
            this.gridColumn39.Caption = "7%";
            this.gridColumn39.DisplayFormat.FormatString = "N2";
            this.gridColumn39.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn39.FieldName = "7";
            this.gridColumn39.MinWidth = 23;
            this.gridColumn39.Name = "gridColumn39";
            this.gridColumn39.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "7", "{0:N2}")});
            this.gridColumn39.Visible = true;
            this.gridColumn39.VisibleIndex = 23;
            this.gridColumn39.Width = 87;
            // 
            // gridColumn40
            // 
            this.gridColumn40.Caption = "8%";
            this.gridColumn40.DisplayFormat.FormatString = "N2";
            this.gridColumn40.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn40.FieldName = "8";
            this.gridColumn40.MinWidth = 23;
            this.gridColumn40.Name = "gridColumn40";
            this.gridColumn40.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "8", "{0:N2}")});
            this.gridColumn40.Visible = true;
            this.gridColumn40.VisibleIndex = 24;
            this.gridColumn40.Width = 87;
            // 
            // gridColumn41
            // 
            this.gridColumn41.Caption = "9%";
            this.gridColumn41.DisplayFormat.FormatString = "N2";
            this.gridColumn41.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn41.FieldName = "9";
            this.gridColumn41.MinWidth = 23;
            this.gridColumn41.Name = "gridColumn41";
            this.gridColumn41.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "9", "{0:N2}")});
            this.gridColumn41.Visible = true;
            this.gridColumn41.VisibleIndex = 25;
            this.gridColumn41.Width = 87;
            // 
            // gridColumn42
            // 
            this.gridColumn42.Caption = "10%";
            this.gridColumn42.DisplayFormat.FormatString = "N2";
            this.gridColumn42.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn42.FieldName = "10";
            this.gridColumn42.MinWidth = 23;
            this.gridColumn42.Name = "gridColumn42";
            this.gridColumn42.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "10", "{0:N2}")});
            this.gridColumn42.Visible = true;
            this.gridColumn42.VisibleIndex = 26;
            this.gridColumn42.Width = 87;
            // 
            // gridColumn43
            // 
            this.gridColumn43.Caption = "11%";
            this.gridColumn43.DisplayFormat.FormatString = "N2";
            this.gridColumn43.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn43.FieldName = "11";
            this.gridColumn43.MinWidth = 23;
            this.gridColumn43.Name = "gridColumn43";
            this.gridColumn43.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "11", "{0:N2}")});
            this.gridColumn43.Visible = true;
            this.gridColumn43.VisibleIndex = 27;
            this.gridColumn43.Width = 87;
            // 
            // gridColumn44
            // 
            this.gridColumn44.Caption = "12%";
            this.gridColumn44.DisplayFormat.FormatString = "N2";
            this.gridColumn44.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn44.FieldName = "12";
            this.gridColumn44.MinWidth = 23;
            this.gridColumn44.Name = "gridColumn44";
            this.gridColumn44.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "12", "{0:N2}")});
            this.gridColumn44.Visible = true;
            this.gridColumn44.VisibleIndex = 28;
            this.gridColumn44.Width = 87;
            // 
            // gridColumn45
            // 
            this.gridColumn45.Caption = "13%";
            this.gridColumn45.DisplayFormat.FormatString = "N2";
            this.gridColumn45.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn45.FieldName = "13";
            this.gridColumn45.MinWidth = 23;
            this.gridColumn45.Name = "gridColumn45";
            this.gridColumn45.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "13", "{0:N2}")});
            this.gridColumn45.Visible = true;
            this.gridColumn45.VisibleIndex = 29;
            this.gridColumn45.Width = 87;
            // 
            // gridColumn46
            // 
            this.gridColumn46.Caption = "14%";
            this.gridColumn46.DisplayFormat.FormatString = "N2";
            this.gridColumn46.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn46.FieldName = "14";
            this.gridColumn46.MinWidth = 23;
            this.gridColumn46.Name = "gridColumn46";
            this.gridColumn46.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "14", "{0:N2}")});
            this.gridColumn46.Visible = true;
            this.gridColumn46.VisibleIndex = 30;
            this.gridColumn46.Width = 87;
            // 
            // gridColumn47
            // 
            this.gridColumn47.Caption = "15%";
            this.gridColumn47.DisplayFormat.FormatString = "N2";
            this.gridColumn47.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn47.FieldName = "15";
            this.gridColumn47.MinWidth = 23;
            this.gridColumn47.Name = "gridColumn47";
            this.gridColumn47.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "15", "{0:N2}")});
            this.gridColumn47.Visible = true;
            this.gridColumn47.VisibleIndex = 31;
            this.gridColumn47.Width = 87;
            // 
            // gridColumn48
            // 
            this.gridColumn48.Caption = "16%";
            this.gridColumn48.DisplayFormat.FormatString = "N2";
            this.gridColumn48.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn48.FieldName = "16";
            this.gridColumn48.MinWidth = 23;
            this.gridColumn48.Name = "gridColumn48";
            this.gridColumn48.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "16", "{0:N2}")});
            this.gridColumn48.Width = 87;
            // 
            // gridColumn49
            // 
            this.gridColumn49.Caption = "17%";
            this.gridColumn49.DisplayFormat.FormatString = "N2";
            this.gridColumn49.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn49.FieldName = "17";
            this.gridColumn49.MinWidth = 23;
            this.gridColumn49.Name = "gridColumn49";
            this.gridColumn49.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "17", "{0:N2}")});
            this.gridColumn49.Width = 87;
            // 
            // gridColumn50
            // 
            this.gridColumn50.Caption = "18%";
            this.gridColumn50.DisplayFormat.FormatString = "N2";
            this.gridColumn50.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn50.FieldName = "18";
            this.gridColumn50.MinWidth = 23;
            this.gridColumn50.Name = "gridColumn50";
            this.gridColumn50.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "18", "{0:N2}")});
            this.gridColumn50.Width = 87;
            // 
            // gridColumn51
            // 
            this.gridColumn51.Caption = "19%";
            this.gridColumn51.DisplayFormat.FormatString = "N2";
            this.gridColumn51.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn51.FieldName = "19";
            this.gridColumn51.MinWidth = 23;
            this.gridColumn51.Name = "gridColumn51";
            this.gridColumn51.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "19", "{0:N2}")});
            this.gridColumn51.Width = 87;
            // 
            // gridColumn52
            // 
            this.gridColumn52.Caption = "20%";
            this.gridColumn52.DisplayFormat.FormatString = "N2";
            this.gridColumn52.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn52.FieldName = "20";
            this.gridColumn52.MinWidth = 23;
            this.gridColumn52.Name = "gridColumn52";
            this.gridColumn52.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "20", "{0:N2}")});
            this.gridColumn52.Width = 87;
            // 
            // gridColumn53
            // 
            this.gridColumn53.Caption = "21%";
            this.gridColumn53.DisplayFormat.FormatString = "N2";
            this.gridColumn53.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn53.FieldName = "21";
            this.gridColumn53.MinWidth = 23;
            this.gridColumn53.Name = "gridColumn53";
            this.gridColumn53.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "21", "{0:N2}")});
            this.gridColumn53.Width = 87;
            // 
            // gridColumn54
            // 
            this.gridColumn54.Caption = "22%";
            this.gridColumn54.DisplayFormat.FormatString = "N2";
            this.gridColumn54.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn54.FieldName = "22";
            this.gridColumn54.MinWidth = 23;
            this.gridColumn54.Name = "gridColumn54";
            this.gridColumn54.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "22", "{0:N2}")});
            this.gridColumn54.Width = 87;
            // 
            // gridColumn55
            // 
            this.gridColumn55.Caption = "23%";
            this.gridColumn55.DisplayFormat.FormatString = "N2";
            this.gridColumn55.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn55.FieldName = "23";
            this.gridColumn55.MinWidth = 23;
            this.gridColumn55.Name = "gridColumn55";
            this.gridColumn55.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "23", "{0:N2}")});
            this.gridColumn55.Width = 87;
            // 
            // gridColumn56
            // 
            this.gridColumn56.Caption = "24%";
            this.gridColumn56.DisplayFormat.FormatString = "N2";
            this.gridColumn56.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn56.FieldName = "24";
            this.gridColumn56.MinWidth = 23;
            this.gridColumn56.Name = "gridColumn56";
            this.gridColumn56.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "24", "{0:N2}")});
            this.gridColumn56.Width = 87;
            // 
            // gridColumn57
            // 
            this.gridColumn57.Caption = "25%";
            this.gridColumn57.DisplayFormat.FormatString = "N2";
            this.gridColumn57.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn57.FieldName = "25";
            this.gridColumn57.MinWidth = 23;
            this.gridColumn57.Name = "gridColumn57";
            this.gridColumn57.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "25", "{0:N2}")});
            this.gridColumn57.Width = 87;
            // 
            // gridColumn58
            // 
            this.gridColumn58.Caption = "26%";
            this.gridColumn58.DisplayFormat.FormatString = "N2";
            this.gridColumn58.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn58.FieldName = "26";
            this.gridColumn58.MinWidth = 23;
            this.gridColumn58.Name = "gridColumn58";
            this.gridColumn58.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "26", "{0:N2}")});
            this.gridColumn58.Width = 87;
            // 
            // gridColumn59
            // 
            this.gridColumn59.Caption = "27%";
            this.gridColumn59.DisplayFormat.FormatString = "N2";
            this.gridColumn59.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn59.FieldName = "27";
            this.gridColumn59.MinWidth = 23;
            this.gridColumn59.Name = "gridColumn59";
            this.gridColumn59.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "27", "{0:N2}")});
            this.gridColumn59.Width = 87;
            // 
            // gridColumn60
            // 
            this.gridColumn60.Caption = "28%";
            this.gridColumn60.DisplayFormat.FormatString = "N2";
            this.gridColumn60.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn60.FieldName = "28";
            this.gridColumn60.MinWidth = 23;
            this.gridColumn60.Name = "gridColumn60";
            this.gridColumn60.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "28", "{0:N2}")});
            this.gridColumn60.Width = 87;
            // 
            // gridColumn61
            // 
            this.gridColumn61.Caption = "29%";
            this.gridColumn61.DisplayFormat.FormatString = "N2";
            this.gridColumn61.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn61.FieldName = "30";
            this.gridColumn61.MinWidth = 23;
            this.gridColumn61.Name = "gridColumn61";
            this.gridColumn61.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "29", "{0:N2}")});
            this.gridColumn61.Width = 87;
            // 
            // gridColumn62
            // 
            this.gridColumn62.Caption = "30%";
            this.gridColumn62.DisplayFormat.FormatString = "N2";
            this.gridColumn62.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn62.FieldName = "30";
            this.gridColumn62.MinWidth = 23;
            this.gridColumn62.Name = "gridColumn62";
            this.gridColumn62.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "30", "{0:N2}")});
            this.gridColumn62.Width = 87;
            // 
            // chart_VaRDistribution
            // 
            xyDiagram1.AxisX.Title.Text = "VaR Distribution range";
            xyDiagram1.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            xyDiagram1.AxisX.Visibility = DevExpress.Utils.DefaultBoolean.True;
            xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
            xyDiagram1.AxisX.VisualRange.Auto = false;
            xyDiagram1.AxisX.VisualRange.AutoSideMargins = false;
            xyDiagram1.AxisX.VisualRange.EndSideMargin = 0D;
            xyDiagram1.AxisX.VisualRange.MaxValueSerializable = "30";
            xyDiagram1.AxisX.VisualRange.MinValueSerializable = "-30";
            xyDiagram1.AxisX.VisualRange.StartSideMargin = 0D;
            xyDiagram1.AxisX.WholeRange.Auto = false;
            xyDiagram1.AxisX.WholeRange.AutoSideMargins = false;
            xyDiagram1.AxisX.WholeRange.EndSideMargin = 0D;
            xyDiagram1.AxisX.WholeRange.MaxValueSerializable = "30";
            xyDiagram1.AxisX.WholeRange.MinValueSerializable = "-30";
            xyDiagram1.AxisX.WholeRange.StartSideMargin = 0D;
            xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
            this.chart_VaRDistribution.Diagram = xyDiagram1;
            this.chart_VaRDistribution.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart_VaRDistribution.Legend.Name = "Default Legend";
            this.chart_VaRDistribution.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
            this.chart_VaRDistribution.Location = new System.Drawing.Point(0, 0);
            this.chart_VaRDistribution.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chart_VaRDistribution.Name = "chart_VaRDistribution";
            series1.Name = "VaR";
            series1.View = splineSeriesView1;
            this.chart_VaRDistribution.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1};
            this.chart_VaRDistribution.Size = new System.Drawing.Size(1345, 274);
            this.chart_VaRDistribution.TabIndex = 1;
            // 
            // scVarDistribution
            // 
            this.scVarDistribution.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scVarDistribution.Location = new System.Drawing.Point(0, 0);
            this.scVarDistribution.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.scVarDistribution.Name = "scVarDistribution";
            this.scVarDistribution.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scVarDistribution.Panel1
            // 
            this.scVarDistribution.Panel1.Controls.Add(this.groupC_VarDistribution);
            // 
            // scVarDistribution.Panel2
            // 
            this.scVarDistribution.Panel2.Controls.Add(this.chart_VaRDistribution);
            this.scVarDistribution.Size = new System.Drawing.Size(1345, 659);
            this.scVarDistribution.SplitterDistance = 380;
            this.scVarDistribution.SplitterWidth = 5;
            this.scVarDistribution.TabIndex = 2;
            // 
            // groupC_VarDistribution
            // 
            this.groupC_VarDistribution.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupC_VarDistribution.AppearanceCaption.Options.UseFont = true;
            this.groupC_VarDistribution.Controls.Add(this.btn_Refresh);
            this.groupC_VarDistribution.Controls.Add(this.gc_VaRPortfolio);
            this.groupC_VarDistribution.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupC_VarDistribution.Location = new System.Drawing.Point(0, 0);
            this.groupC_VarDistribution.LookAndFeel.SkinName = "Office 2019 Colorful";
            this.groupC_VarDistribution.LookAndFeel.UseDefaultLookAndFeel = false;
            this.groupC_VarDistribution.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupC_VarDistribution.Name = "groupC_VarDistribution";
            this.groupC_VarDistribution.Size = new System.Drawing.Size(1345, 380);
            this.groupC_VarDistribution.TabIndex = 1;
            // 
            // btn_Refresh
            // 
            this.btn_Refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Refresh.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Refresh.Appearance.Options.UseFont = true;
            this.btn_Refresh.Location = new System.Drawing.Point(1272, 0);
            this.btn_Refresh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Refresh.Name = "btn_Refresh";
            this.btn_Refresh.Size = new System.Drawing.Size(70, 30);
            this.btn_Refresh.TabIndex = 1;
            this.btn_Refresh.Text = "Refresh";
            this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
            // 
            // form_VaRDistribution
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1345, 659);
            this.Controls.Add(this.scVarDistribution);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("form_VaRDistribution.IconOptions.Icon")));
            this.LookAndFeel.SkinName = "The Bezier";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "form_VaRDistribution";
            this.Text = "VaR Distribution";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPortfolio_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.gc_VaRPortfolio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_VaRPortfolio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(xyDiagram1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(splineSeriesView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(series1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_VaRDistribution)).EndInit();
            this.scVarDistribution.Panel1.ResumeLayout(false);
            this.scVarDistribution.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scVarDistribution)).EndInit();
            this.scVarDistribution.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupC_VarDistribution)).EndInit();
            this.groupC_VarDistribution.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        internal GridControl gc_VaRPortfolio;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn16;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn17;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn18;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn19;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn20;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn21;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn22;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn23;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn24;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn25;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn26;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn27;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn28;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn29;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn30;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn31;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn32;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn33;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn34;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn35;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn36;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn37;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn38;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn39;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn40;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn41;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn42;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn43;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn44;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn45;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn46;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn47;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn48;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn49;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn50;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn51;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn52;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn53;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn54;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn55;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn56;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn57;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn58;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn59;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn60;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn61;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn62;
        internal DevExpress.XtraCharts.ChartControl chart_VaRDistribution;
        private System.Windows.Forms.SplitContainer scVarDistribution;
        internal GridView gv_VaRPortfolio;
        private DevExpress.XtraEditors.SimpleButton btn_Refresh;
        internal DevExpress.XtraEditors.GroupControl groupC_VarDistribution;
    }
}