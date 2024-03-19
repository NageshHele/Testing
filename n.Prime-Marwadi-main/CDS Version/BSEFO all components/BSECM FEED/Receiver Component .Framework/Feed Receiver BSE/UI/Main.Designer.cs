
namespace Feed_Receiver_BSE.UI
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.panel_Main = new DevExpress.XtraEditors.PanelControl();
            this.gc_ConnectionInfo = new DevExpress.XtraGrid.GridControl();
            this.gv_ConnectionInfo = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grp_Exchange = new System.Windows.Forms.GroupBox();
            this.lbl_PORT = new System.Windows.Forms.Label();
            this.lbl_IP = new System.Windows.Forms.Label();
            this.lbl_LastTradedTime = new System.Windows.Forms.Label();
            this.grp_FileStatus = new System.Windows.Forms.GroupBox();
            this.lbl_Security = new System.Windows.Forms.Label();
            this.lbl_Bhavcopy = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.panel_Main)).BeginInit();
            this.panel_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gc_ConnectionInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_ConnectionInfo)).BeginInit();
            this.grp_Exchange.SuspendLayout();
            this.grp_FileStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_Main
            // 
            this.panel_Main.Controls.Add(this.gc_ConnectionInfo);
            this.panel_Main.Controls.Add(this.grp_Exchange);
            this.panel_Main.Controls.Add(this.grp_FileStatus);
            this.panel_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Main.Location = new System.Drawing.Point(0, 0);
            this.panel_Main.Name = "panel_Main";
            this.panel_Main.Size = new System.Drawing.Size(409, 254);
            this.panel_Main.TabIndex = 0;
            // 
            // gc_ActiveConnections
            // 
            this.gc_ConnectionInfo.Location = new System.Drawing.Point(12, 96);
            this.gc_ConnectionInfo.MainView = this.gv_ConnectionInfo;
            this.gc_ConnectionInfo.Name = "gc_ActiveConnections";
            this.gc_ConnectionInfo.Size = new System.Drawing.Size(385, 146);
            this.gc_ConnectionInfo.TabIndex = 20;
            this.gc_ConnectionInfo.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_ConnectionInfo});
            // 
            // gv_ActiveConnections
            // 
            this.gv_ConnectionInfo.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.gv_ConnectionInfo.Appearance.HeaderPanel.Options.UseFont = true;
            this.gv_ConnectionInfo.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gv_ConnectionInfo.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_ConnectionInfo.Appearance.Row.Options.UseTextOptions = true;
            this.gv_ConnectionInfo.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gv_ConnectionInfo.GridControl = this.gc_ConnectionInfo;
            this.gv_ConnectionInfo.Name = "gv_ActiveConnections";
            this.gv_ConnectionInfo.OptionsBehavior.Editable = false;
            this.gv_ConnectionInfo.OptionsView.EnableAppearanceEvenRow = true;
            this.gv_ConnectionInfo.OptionsView.ShowGroupPanel = false;
            this.gv_ConnectionInfo.OptionsView.ShowIndicator = false;
            // 
            // grp_Exchange
            // 
            this.grp_Exchange.Controls.Add(this.lbl_PORT);
            this.grp_Exchange.Controls.Add(this.lbl_IP);
            this.grp_Exchange.Controls.Add(this.lbl_LastTradedTime);
            this.grp_Exchange.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.grp_Exchange.Location = new System.Drawing.Point(12, 12);
            this.grp_Exchange.Name = "grp_Exchange";
            this.grp_Exchange.Size = new System.Drawing.Size(198, 78);
            this.grp_Exchange.TabIndex = 19;
            this.grp_Exchange.TabStop = false;
            this.grp_Exchange.Text = "BSE CM";
            // 
            // lbl_PORT
            // 
            this.lbl_PORT.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_PORT.Font = new System.Drawing.Font("Consolas", 7.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_PORT.Location = new System.Drawing.Point(13, 44);
            this.lbl_PORT.Name = "lbl_PORT";
            this.lbl_PORT.Size = new System.Drawing.Size(156, 16);
            this.lbl_PORT.TabIndex = 18;
            this.lbl_PORT.Text = "PORT : 33125";
            this.lbl_PORT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_IP
            // 
            this.lbl_IP.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_IP.Font = new System.Drawing.Font("Consolas", 7.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_IP.Location = new System.Drawing.Point(13, 25);
            this.lbl_IP.Name = "lbl_IP";
            this.lbl_IP.Size = new System.Drawing.Size(156, 16);
            this.lbl_IP.TabIndex = 17;
            this.lbl_IP.Text = "IP : 233.1.2.5";
            this.lbl_IP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_LastTradedTime
            // 
            this.lbl_LastTradedTime.Location = new System.Drawing.Point(65, 0);
            this.lbl_LastTradedTime.Name = "lbl_LastTradedTime";
            this.lbl_LastTradedTime.Size = new System.Drawing.Size(127, 13);
            this.lbl_LastTradedTime.TabIndex = 6;
            this.lbl_LastTradedTime.Text = "LTT 7208 : 12:00:00";
            this.lbl_LastTradedTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // grp_FileStatus
            // 
            this.grp_FileStatus.Controls.Add(this.lbl_Security);
            this.grp_FileStatus.Controls.Add(this.lbl_Bhavcopy);
            this.grp_FileStatus.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.grp_FileStatus.Location = new System.Drawing.Point(216, 12);
            this.grp_FileStatus.Name = "grp_FileStatus";
            this.grp_FileStatus.Size = new System.Drawing.Size(181, 78);
            this.grp_FileStatus.TabIndex = 18;
            this.grp_FileStatus.TabStop = false;
            this.grp_FileStatus.Text = "Status";
            // 
            // lbl_Security
            // 
            this.lbl_Security.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_Security.Font = new System.Drawing.Font("Consolas", 7.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Security.Location = new System.Drawing.Point(14, 44);
            this.lbl_Security.Name = "lbl_Security";
            this.lbl_Security.Size = new System.Drawing.Size(156, 16);
            this.lbl_Security.TabIndex = 16;
            this.lbl_Security.Text = "Security : 01-01-1970";
            this.lbl_Security.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_Bhavcopy
            // 
            this.lbl_Bhavcopy.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_Bhavcopy.Font = new System.Drawing.Font("Consolas", 7.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Bhavcopy.Location = new System.Drawing.Point(14, 25);
            this.lbl_Bhavcopy.Name = "lbl_Bhavcopy";
            this.lbl_Bhavcopy.Size = new System.Drawing.Size(156, 16);
            this.lbl_Bhavcopy.TabIndex = 15;
            this.lbl_Bhavcopy.Text = "Bhavcopy : 01-01-1970";
            this.lbl_Bhavcopy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 254);
            this.Controls.Add(this.panel_Main);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("Main.IconOptions.Image")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "Packet Dump";
            this.Shown += new System.EventHandler(this.Main_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.panel_Main)).EndInit();
            this.panel_Main.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gc_ConnectionInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_ConnectionInfo)).EndInit();
            this.grp_Exchange.ResumeLayout(false);
            this.grp_FileStatus.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panel_Main;
        private System.Windows.Forms.GroupBox grp_FileStatus;
        private System.Windows.Forms.Label lbl_Security;
        private System.Windows.Forms.Label lbl_Bhavcopy;
        private System.Windows.Forms.GroupBox grp_Exchange;
        private System.Windows.Forms.Label lbl_PORT;
        private System.Windows.Forms.Label lbl_IP;
        public System.Windows.Forms.Label lbl_LastTradedTime;
        private DevExpress.XtraGrid.GridControl gc_ConnectionInfo;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_ConnectionInfo;
    }
}