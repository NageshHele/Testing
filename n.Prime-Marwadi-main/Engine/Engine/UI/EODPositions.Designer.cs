
namespace Engine.UI
{
    partial class EODPositions
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
            this.gc_EODPositions = new DevExpress.XtraGrid.GridControl();
            this.gv_EODPositions = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.gc_EODPositions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_EODPositions)).BeginInit();
            this.SuspendLayout();
            // 
            // gc_EODPositions
            // 
            this.gc_EODPositions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gc_EODPositions.EmbeddedNavigator.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gc_EODPositions.EmbeddedNavigator.Appearance.Options.UseFont = true;
            this.gc_EODPositions.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gc_EODPositions.EmbeddedNavigator.Buttons.Edit.Enabled = false;
            this.gc_EODPositions.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gc_EODPositions.EmbeddedNavigator.Buttons.EndEdit.Enabled = false;
            this.gc_EODPositions.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gc_EODPositions.Location = new System.Drawing.Point(0, 0);
            this.gc_EODPositions.MainView = this.gv_EODPositions;
            this.gc_EODPositions.Name = "gc_EODPositions";
            this.gc_EODPositions.Size = new System.Drawing.Size(825, 420);
            this.gc_EODPositions.TabIndex = 23;
            this.gc_EODPositions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_EODPositions});
            // 
            // gv_EODPositions
            // 
            this.gv_EODPositions.Appearance.EvenRow.BackColor = System.Drawing.Color.LightGray;
            this.gv_EODPositions.Appearance.EvenRow.Options.UseBackColor = true;
            this.gv_EODPositions.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent;
            this.gv_EODPositions.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gv_EODPositions.Appearance.FocusedRow.BackColor = System.Drawing.Color.LightBlue;
            this.gv_EODPositions.Appearance.FocusedRow.ForeColor = System.Drawing.Color.Black;
            this.gv_EODPositions.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gv_EODPositions.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gv_EODPositions.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.gv_EODPositions.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_EODPositions.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_EODPositions.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_EODPositions.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_EODPositions.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.gv_EODPositions.Appearance.Row.Options.UseFont = true;
            this.gv_EODPositions.Appearance.Row.Options.UseTextOptions = true;
            this.gv_EODPositions.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_EODPositions.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gv_EODPositions.Appearance.VertLine.BackColor = System.Drawing.Color.Transparent;
            this.gv_EODPositions.Appearance.VertLine.BackColor2 = System.Drawing.Color.Transparent;
            this.gv_EODPositions.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_EODPositions.GridControl = this.gc_EODPositions;
            this.gv_EODPositions.Name = "gv_EODPositions";
            this.gv_EODPositions.OptionsBehavior.Editable = false;
            this.gv_EODPositions.OptionsView.ColumnAutoWidth = false;
            this.gv_EODPositions.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_EODPositions.OptionsView.ShowFooter = true;
            this.gv_EODPositions.OptionsView.ShowGroupPanel = false;
            this.gv_EODPositions.OptionsView.ShowIndicator = false;
            this.gv_EODPositions.RowHeight = 30;
            this.gv_EODPositions.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_EODPositions_PopupMenuShowing);
            // 
            // EODPositions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 420);
            this.Controls.Add(this.gc_EODPositions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.ShowIcon = false;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(825, 450);
            this.Name = "EODPositions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EOD Positions";
            ((System.ComponentModel.ISupportInitialize)(this.gc_EODPositions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_EODPositions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gc_EODPositions;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_EODPositions;
    }
}