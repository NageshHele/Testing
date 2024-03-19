
namespace Prime.UI
{
    partial class form_ClientWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_ClientWindow));
            this.gc_windows = new DevExpress.XtraGrid.GridControl();
            this.gv_window = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gb_grid = new System.Windows.Forms.GroupBox();
            this.lbc_ClientID = new DevExpress.XtraEditors.LabelControl();
            this.lbc_Underlying = new DevExpress.XtraEditors.LabelControl();
            this.lbc_Expiry = new DevExpress.XtraEditors.LabelControl();
            this.cbe_ClientID = new DevExpress.XtraEditors.ComboBoxEdit();
            this.cbe_Underlying = new DevExpress.XtraEditors.ComboBoxEdit();
            this.cbe_Expiry = new DevExpress.XtraEditors.ComboBoxEdit();
            this.gc_Greeks = new DevExpress.XtraGrid.GridControl();
            this.gv_Greeks = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gc_Options = new DevExpress.XtraGrid.GridControl();
            this.gv_Options = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gc_Futures = new DevExpress.XtraGrid.GridControl();
            this.gv_Futures = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.gc_windows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_window)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.gb_grid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbe_ClientID.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbe_Underlying.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbe_Expiry.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gc_Greeks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_Greeks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gc_Options)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_Options)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gc_Futures)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_Futures)).BeginInit();
            this.SuspendLayout();
            // 
            // gc_windows
            // 
            this.gc_windows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_windows.Location = new System.Drawing.Point(3, 21);
            this.gc_windows.MainView = this.gv_window;
            this.gc_windows.Name = "gc_windows";
            this.gc_windows.Size = new System.Drawing.Size(1306, 382);
            this.gc_windows.TabIndex = 0;
            this.gc_windows.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_window,
            this.gridView1});
            this.gc_windows.DataSourceChanged += new System.EventHandler(this.gc_windows_DataSourceChanged);
            // 
            // gv_window
            // 
            this.gv_window.Appearance.FixedLine.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gv_window.Appearance.FixedLine.Options.UseFont = true;
            this.gv_window.Appearance.FocusedRow.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.gv_window.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.gv_window.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Gray;
            this.gv_window.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_window.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gv_window.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_window.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_window.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_window.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gv_window.Appearance.Row.Options.UseFont = true;
            this.gv_window.Appearance.Row.Options.UseTextOptions = true;
            this.gv_window.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_window.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_window.Appearance.VertLine.BackColor = System.Drawing.Color.Transparent;
            this.gv_window.Appearance.VertLine.BackColor2 = System.Drawing.Color.Transparent;
            this.gv_window.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_window.GridControl = this.gc_windows;
            this.gv_window.Name = "gv_window";
            this.gv_window.OptionsBehavior.Editable = false;
            this.gv_window.OptionsBehavior.ReadOnly = true;
            this.gv_window.OptionsPrint.AutoWidth = false;
            this.gv_window.OptionsPrint.EnableAppearanceOddRow = true;
            this.gv_window.OptionsPrint.ExpandAllDetails = true;
            this.gv_window.OptionsPrint.PrintDetails = true;
            this.gv_window.OptionsPrint.PrintFooter = false;
            this.gv_window.OptionsPrint.PrintGroupFooter = false;
            this.gv_window.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gv_window.OptionsView.ColumnAutoWidth = false;
            this.gv_window.OptionsView.EnableAppearanceOddRow = true;
            this.gv_window.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
            this.gv_window.OptionsView.ShowGroupPanel = false;
            this.gv_window.OptionsView.ShowIndicator = false;
            this.gv_window.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
            this.gv_window.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_window_RowCellStyle);
            this.gv_window.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_window_PopupMenuShowing);
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gc_windows;
            this.gridView1.Name = "gridView1";
            // 
            // gb_grid
            // 
            this.gb_grid.Controls.Add(this.gc_windows);
            this.gb_grid.Location = new System.Drawing.Point(12, 86);
            this.gb_grid.Name = "gb_grid";
            this.gb_grid.Size = new System.Drawing.Size(1312, 406);
            this.gb_grid.TabIndex = 2;
            this.gb_grid.TabStop = false;
            // 
            // lbc_ClientID
            // 
            this.lbc_ClientID.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lbc_ClientID.Appearance.Options.UseFont = true;
            this.lbc_ClientID.Location = new System.Drawing.Point(170, 21);
            this.lbc_ClientID.Name = "lbc_ClientID";
            this.lbc_ClientID.Size = new System.Drawing.Size(18, 23);
            this.lbc_ClientID.TabIndex = 6;
            this.lbc_ClientID.Text = "ID";
            // 
            // lbc_Underlying
            // 
            this.lbc_Underlying.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lbc_Underlying.Appearance.Options.UseFont = true;
            this.lbc_Underlying.Location = new System.Drawing.Point(616, 21);
            this.lbc_Underlying.Name = "lbc_Underlying";
            this.lbc_Underlying.Size = new System.Drawing.Size(89, 23);
            this.lbc_Underlying.TabIndex = 7;
            this.lbc_Underlying.Text = "Underlying";
            // 
            // lbc_Expiry
            // 
            this.lbc_Expiry.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lbc_Expiry.Appearance.Options.UseFont = true;
            this.lbc_Expiry.Location = new System.Drawing.Point(1100, 21);
            this.lbc_Expiry.Name = "lbc_Expiry";
            this.lbc_Expiry.Size = new System.Drawing.Size(50, 23);
            this.lbc_Expiry.TabIndex = 8;
            this.lbc_Expiry.Text = "Expiry";
            // 
            // cbe_ClientID
            // 
            this.cbe_ClientID.Location = new System.Drawing.Point(55, 50);
            this.cbe_ClientID.Name = "cbe_ClientID";
            this.cbe_ClientID.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbe_ClientID.Properties.Appearance.Options.UseFont = true;
            this.cbe_ClientID.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbe_ClientID.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbe_ClientID.Size = new System.Drawing.Size(250, 30);
            this.cbe_ClientID.TabIndex = 9;
            this.cbe_ClientID.SelectedIndexChanged += new System.EventHandler(this.cbe_ClientID_SelectedIndexChanged);
            this.cbe_ClientID.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbe_ClientID_ButtonPressed);
            // 
            // cbe_Underlying
            // 
            this.cbe_Underlying.Location = new System.Drawing.Point(534, 50);
            this.cbe_Underlying.Name = "cbe_Underlying";
            this.cbe_Underlying.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbe_Underlying.Properties.Appearance.Options.UseFont = true;
            this.cbe_Underlying.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbe_Underlying.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbe_Underlying.Size = new System.Drawing.Size(250, 30);
            this.cbe_Underlying.TabIndex = 10;
            this.cbe_Underlying.SelectedIndexChanged += new System.EventHandler(this.cbe_Underlying_SelectedIndexChanged);
            this.cbe_Underlying.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbe_Underlying_ButtonPressed);
            // 
            // cbe_Expiry
            // 
            this.cbe_Expiry.Location = new System.Drawing.Point(1019, 50);
            this.cbe_Expiry.Name = "cbe_Expiry";
            this.cbe_Expiry.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbe_Expiry.Properties.Appearance.Options.UseFont = true;
            this.cbe_Expiry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbe_Expiry.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbe_Expiry.Size = new System.Drawing.Size(250, 30);
            this.cbe_Expiry.TabIndex = 11;
            this.cbe_Expiry.SelectedIndexChanged += new System.EventHandler(this.cbe_Expiry_SelectedIndexChanged);
            this.cbe_Expiry.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbe_Expiry_ButtonPressed);
            // 
            // gc_Greeks
            // 
            this.gc_Greeks.Location = new System.Drawing.Point(18, 512);
            this.gc_Greeks.LookAndFeel.SkinName = "The Bezier";
            this.gc_Greeks.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gc_Greeks.MainView = this.gv_Greeks;
            this.gc_Greeks.Name = "gc_Greeks";
            this.gc_Greeks.Size = new System.Drawing.Size(1306, 91);
            this.gc_Greeks.TabIndex = 16;
            this.gc_Greeks.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_Greeks});
            this.gc_Greeks.DataSourceChanged += new System.EventHandler(this.gc_Greeks_DataSourceChanged);
            // 
            // gv_Greeks
            // 
            this.gv_Greeks.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.gv_Greeks.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Gray;
            this.gv_Greeks.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_Greeks.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gv_Greeks.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_Greeks.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_Greeks.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_Greeks.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gv_Greeks.Appearance.Row.Options.UseFont = true;
            this.gv_Greeks.Appearance.Row.Options.UseTextOptions = true;
            this.gv_Greeks.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_Greeks.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_Greeks.AppearancePrint.Row.Font = new System.Drawing.Font("Tahoma", 12F);
            this.gv_Greeks.AppearancePrint.Row.Options.UseFont = true;
            this.gv_Greeks.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gv_Greeks.GridControl = this.gc_Greeks;
            this.gv_Greeks.Name = "gv_Greeks";
            this.gv_Greeks.OptionsBehavior.Editable = false;
            this.gv_Greeks.OptionsPrint.MaxMergedCellHeight = 1000;
            this.gv_Greeks.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gv_Greeks.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gv_Greeks.OptionsView.ShowGroupPanel = false;
            this.gv_Greeks.OptionsView.ShowIndicator = false;
            // 
            // gc_Options
            // 
            this.gc_Options.EmbeddedNavigator.Appearance.BackColor = System.Drawing.Color.WhiteSmoke;
            this.gc_Options.EmbeddedNavigator.Appearance.Options.UseBackColor = true;
            this.gc_Options.Location = new System.Drawing.Point(18, 627);
            this.gc_Options.LookAndFeel.SkinName = "The Bezier";
            this.gc_Options.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gc_Options.MainView = this.gv_Options;
            this.gc_Options.Name = "gc_Options";
            this.gc_Options.Size = new System.Drawing.Size(840, 182);
            this.gc_Options.TabIndex = 15;
            this.gc_Options.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_Options});
            this.gc_Options.DataSourceChanged += new System.EventHandler(this.gc_Options_DataSourceChanged);
            // 
            // gv_Options
            // 
            this.gv_Options.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.gv_Options.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Gray;
            this.gv_Options.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_Options.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gv_Options.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_Options.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_Options.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_Options.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gv_Options.Appearance.Row.Options.UseFont = true;
            this.gv_Options.Appearance.Row.Options.UseTextOptions = true;
            this.gv_Options.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_Options.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_Options.AppearancePrint.Row.Font = new System.Drawing.Font("Tahoma", 10F);
            this.gv_Options.AppearancePrint.Row.Options.UseFont = true;
            this.gv_Options.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gv_Options.GridControl = this.gc_Options;
            this.gv_Options.Name = "gv_Options";
            this.gv_Options.OptionsBehavior.Editable = false;
            this.gv_Options.OptionsDetail.AllowExpandEmptyDetails = true;
            this.gv_Options.OptionsPrint.MaxMergedCellHeight = 1000;
            this.gv_Options.OptionsPrint.PrintFooter = false;
            this.gv_Options.OptionsPrint.PrintGroupFooter = false;
            this.gv_Options.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gv_Options.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gv_Options.OptionsView.ShowGroupPanel = false;
            this.gv_Options.OptionsView.ShowIndicator = false;
            // 
            // gc_Futures
            // 
            this.gc_Futures.Location = new System.Drawing.Point(904, 627);
            this.gc_Futures.LookAndFeel.SkinName = "The Bezier";
            this.gc_Futures.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gc_Futures.MainView = this.gv_Futures;
            this.gc_Futures.Name = "gc_Futures";
            this.gc_Futures.Size = new System.Drawing.Size(423, 182);
            this.gc_Futures.TabIndex = 16;
            this.gc_Futures.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_Futures});
            this.gc_Futures.DataSourceChanged += new System.EventHandler(this.gc_Futures_DataSourceChanged);
            // 
            // gv_Futures
            // 
            this.gv_Futures.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.gv_Futures.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Gray;
            this.gv_Futures.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_Futures.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gv_Futures.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_Futures.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_Futures.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_Futures.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gv_Futures.Appearance.Row.Options.UseFont = true;
            this.gv_Futures.Appearance.Row.Options.UseTextOptions = true;
            this.gv_Futures.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_Futures.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_Futures.AppearancePrint.Row.Font = new System.Drawing.Font("Tahoma", 12F);
            this.gv_Futures.AppearancePrint.Row.Options.UseFont = true;
            this.gv_Futures.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gv_Futures.GridControl = this.gc_Futures;
            this.gv_Futures.Name = "gv_Futures";
            this.gv_Futures.OptionsBehavior.Editable = false;
            this.gv_Futures.OptionsPrint.MaxMergedCellHeight = 1000;
            this.gv_Futures.OptionsPrint.PrintFooter = false;
            this.gv_Futures.OptionsPrint.PrintGroupFooter = false;
            this.gv_Futures.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gv_Futures.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gv_Futures.OptionsView.ShowGroupPanel = false;
            this.gv_Futures.OptionsView.ShowIndicator = false;
            // 
            // form_ClientWindow
            // 
            this.Appearance.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1350, 823);
            this.Controls.Add(this.gc_Futures);
            this.Controls.Add(this.gc_Options);
            this.Controls.Add(this.gc_Greeks);
            this.Controls.Add(this.cbe_Expiry);
            this.Controls.Add(this.cbe_Underlying);
            this.Controls.Add(this.cbe_ClientID);
            this.Controls.Add(this.lbc_Expiry);
            this.Controls.Add(this.lbc_Underlying);
            this.Controls.Add(this.lbc_ClientID);
            this.Controls.Add(this.gb_grid);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("form_ClientWindow.IconOptions.Icon")));
            this.LookAndFeel.SkinName = "The Bezier";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_ClientWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Client Window";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.form_window_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.gc_windows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_window)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.gb_grid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbe_ClientID.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbe_Underlying.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbe_Expiry.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gc_Greeks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_Greeks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gc_Options)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_Options)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gc_Futures)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_Futures)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public DevExpress.XtraGrid.GridControl gc_windows;
        public DevExpress.XtraGrid.Views.Grid.GridView gv_window;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.GroupBox gb_grid;
        private DevExpress.XtraEditors.LabelControl lbc_ClientID;
        private DevExpress.XtraEditors.LabelControl lbc_Underlying;
        private DevExpress.XtraEditors.LabelControl lbc_Expiry;
        public DevExpress.XtraEditors.ComboBoxEdit cbe_ClientID;
        public DevExpress.XtraEditors.ComboBoxEdit cbe_Underlying;
        public DevExpress.XtraEditors.ComboBoxEdit cbe_Expiry;
        public DevExpress.XtraGrid.GridControl gc_Options;
        public DevExpress.XtraGrid.Views.Grid.GridView gv_Options;
        public DevExpress.XtraGrid.GridControl gc_Futures;
        public DevExpress.XtraGrid.Views.Grid.GridView gv_Futures;
        public DevExpress.XtraGrid.GridControl gc_Greeks;
        public DevExpress.XtraGrid.Views.Grid.GridView gv_Greeks;
    }
}