using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Net;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using NSEUtilitaire;
using static NSEUtilitaire.Exchange;
using Gateway.Core_Logic;

namespace Gateway
{
    public partial class frm_Main : XtraForm
    {
        /// <summary>
        /// GATEWAY-SERVER-IP from Config.
        /// </summary>
        IPAddress ip_GatewayHBServer = IPAddress.Parse("127.0.0.1");

        /// <summary>
        /// GATEWAY-SERVER-HB-PORT from Config.
        /// </summary>
        int _GatewayHBServerPORT;//added by Navin on 11-10-2019

        /// <summary>
        /// MySQL connection string.
        /// </summary>
        string _MySQLCon = string.Empty;
        
        /// <summary>
        /// Span file fownload path from Config.xml
        /// </summary>
        string _SpanFolderPath = string.Empty;

        /// <summary>
        /// Contents of Config.xml
        /// </summary>
        DataSet ds_Config = new DataSet();

        clsWriteLog _logger;

        // Added by Snehadri on 30JUN2021 for Automatic BOD Process
        BODUtilityConnector bodutilityConnector = new BODUtilityConnector();
        bool IsAuto = false;

        public frm_Main()
        {
            try
            {
                InitializeComponent();

                _logger = new clsWriteLog(Application.StartupPath.ToString() + "\\Log");
                _logger.DeleteOldLogs(Application.StartupPath.ToString() + "\\Log");

                ReadConfig();

                CheckMaxAllowedSqlPacket();

                StartBODconnector();

                SetText("nImageB initialised successfully.");
            }
            catch (Exception ee) { XtraMessageBox.Show("Error while Initialising nImageB. " + ee, "Error"); Environment.Exit(0); }
        }

        #region Supplimentary Methods


        private void StartBODconnector()                // Added by Snehadri on 30JUN2021 for Automatic BOD Process
        {

            try
            {
                Thread.Sleep(2000);
                bool connection = bodutilityConnector.StartClient();
                if (connection)
                {
                    chk_TAndC.CheckState = CheckState.Checked;
                    IsAuto = true;
                    btn_StartPick.PerformClick();
                    btn_StartPick.Click += btn_StartPick_Click;
                }
            }
            catch (Exception ee) { _logger.Error("BODConnector Error: " + ee.ToString()); }
        }
        private void ReadLicense()
        {
            try
            {
                string[] arr_LicenseFields = DecryptLicense(File.ReadAllText(Application.StartupPath + @"\Gateway.ns")).Split('|');
                if (arr_LicenseFields.Length == 4 && arr_LicenseFields[0] == "GATEWAY")
                {
                    DateTime dt_LicenseExpiry = ConvertFromUnixTimestamp(Convert.ToDouble(arr_LicenseFields[1]));

                    if (dt_LicenseExpiry.Date < DateTime.Now.Date)
                    {
                        XtraMessageBox.Show("License expired. Please contact System Administrator.", "Error");
                        Environment.Exit(0);
                    }
                }
                else
                {
                    XtraMessageBox.Show("Invalid License. Please contact System Administrator.", "Error");
                    Environment.Exit(0);
                }
            }
            catch (Exception)
            {
                XtraMessageBox.Show("License file not found.", "Error");
                Environment.Exit(0);
            }
        }

        private void ReadConfig()
        {
            try
            {
                XmlTextReader tReader = new XmlTextReader("C:/Prime/Config.xml");
                tReader.Read();
                ds_Config.ReadXml(tReader);

                var DBInfo = ds_Config.Tables["DB"].Rows[0];

                //added convert zero datetime=True on 16APR2021 by Amey. Was not assigning SQL datetime to C# DateTime in sp_ContractMaster.
                _MySQLCon = $"Data Source={DBInfo["SERVER"]};Initial Catalog={DBInfo["NAME"]};UserID={DBInfo["USER"]};Password={DBInfo["PASSWORD"]};SslMode=none;convert zero datetime=True;";

                var CONInfo = ds_Config.Tables["CONNECTION"].Rows[0];
                //changed on 13JAN2021 by Amey
                ip_GatewayHBServer = IPAddress.Parse(CONInfo["GATEWAY-SPAN-SERVER-IP"].ToString());
                _GatewayHBServerPORT = Convert.ToInt32(CONInfo["GATEWAY-SPAN-SERVER-HB-PORT"].ToString());

                try
                {
                    var SpanInfo = ds_Config.Tables["NIMAGE2"].Rows[0];
                    var arr_SpanInfo = SpanInfo["FILE-PATH"].ToString().Split('$');

                    _SpanFolderPath = arr_SpanInfo[0] + DateTime.Now.ToString(arr_SpanInfo[1].Substring(0, arr_SpanInfo[1].LastIndexOf('\\'))).ToUpper();
                }
                catch (Exception)
                {
                    _logger.Error("Span exe file path not specified");
                }

                SetMaxAllowedSqlPacket();

                //added on 28FEB2021 by Amey
                FlushMySQLConnectionErrors();
            }
            catch (Exception error)
            {
                _logger.Error("ReadConfig : " + error);
                XtraMessageBox.Show("Invalid entry in Config file. Please check logs for more details.", "Error");
            }
        }

        private void CheckMaxAllowedSqlPacket()
        {
            string _pcket = string.Empty;
            try
            {
                using (MySqlConnection myCon = new MySqlConnection(_MySQLCon))
                {
                    //changed to SP on 27APR2021 by Amey
                    using (MySqlCommand cmd = new MySqlCommand("sp_GetMaxAllowedPacket", myCon))
                    {
                        myCon.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                _pcket = reader.GetString(1);

                            reader.Close();
                        }
                        myCon.Close();
                    }
                }
                if (_pcket != "1073741824")
                {
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
            catch (Exception errror)
            {
                _logger.Error("CheckMaxAllowedSqlPacket : " + errror);
            }
        }

        private void SetMaxAllowedSqlPacket()
        {
            try
            {
                using (MySqlConnection myConn = new MySqlConnection(_MySQLCon))
                {
                    //changed to SP on 27APR2021 by Amey
                    using (MySqlCommand myCmd = new MySqlCommand("sp_SetMaxAllowedPacket", myConn))
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;

                        myConn.Open();
                        myCmd.ExecuteNonQuery();
                        myConn.Close();
                    }
                }
            }
            catch (Exception errror)
            {
                _logger.Error("SetMaxAllowedSqlPacket : " + errror);
            }
        }

        //added on 28FEB2021 by Amey
        private void FlushMySQLConnectionErrors()
        {
            try
            {
                using (MySqlConnection myConn = new MySqlConnection(_MySQLCon))
                {
                    using (MySqlCommand myCmd = new MySqlCommand("flush hosts;", myConn))
                    {
                        myConn.Open();
                        myCmd.ExecuteNonQuery();
                        myConn.Close();
                    }
                }
            }
            catch (Exception errror)
            {
                _logger.Error("FlushMySQLConnectionErrors : " + errror);
            }
        }

        public string DecryptLicense(string cipherText)
        {
            string EncryptionKey = "n!e@r#v$e$s#o@l!";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        delegate void SetTextCallback(string text);
        private void SetText(string text)
        {
            if (this.listBox_Messages.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
                listBox_Messages.Items.Insert(0, DateTime.Now + " : " + text);
        }

        private DateTime ConvertFromUnixTimestamp(double timestamp1)
        {
            DateTime origin = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp1);
        }

        private double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - origin;
            return diff.TotalSeconds;
        }

        #endregion

        #region Form Events

        
        nProcess _DumpData;

        private void btn_StartPick_Click(object sender, EventArgs e)
        {
            try
            {
                // Altered by Snehadri on 30JUN2021 for Automatic BOD Process
                if (!IsAuto)
                {
                    XtraMessageBox.Show("Margin will not be computed in case span and exposure files are unavailable. Kindly ensure span and exposure files are present in " + _SpanFolderPath + " before clicking on OK");
                }

               
                btn_StartPick.Enabled = false;

                //changed on 13JAN2021 by Amey
                //added params on 15JAN2021 by Amey
                _DumpData = new nProcess(_MySQLCon, ip_GatewayHBServer, _GatewayHBServerPORT, _logger, ds_Config, _SpanFolderPath);

                //added on 25JAN2021 by Amey
                
                ind_TradeFileFOAvailable.ToolTip = CTradeProcess.Instance._TradeFileFO;
                ind_TradeFileFO.ToolTip = CTradeProcess.Instance._TradeFileFO;

                

                //added on 20APR2021 by Amey
                

                //added on 22JAN2021 by Amey
                _DumpData.eve_AddToList += SetText;
                _DumpData.eve_TradeFileStatus += _DumpData_eve_TradeFileStatus;
                _DumpData.eve_ConnectionStatus += _DumpData_eve_ConnectionStatus;
                _DumpData.eve_SpanTime += _DumpData_eve_SpanTime;
                _DumpData.eve_ButtonState += _DumpData_eve_ButtonState;
                _DumpData.StartSpanProcess();
                

                Thread t_ReadIntradayFiles = new Thread(_DumpData.ReadIntradayFiles);
                t_ReadIntradayFiles.IsBackground = true;
                t_ReadIntradayFiles.Start();
            }
            catch (Exception ee)
            {
                _logger.Error("btn_Start_Click : " + ee);

                SetText("n.Gateway Start failed. Please check logs for more details.");
            }
        }

        private void _DumpData_eve_ButtonState(bool buttonstate)
        {
            try
            {
                this.Invoke((MethodInvoker)(() =>
                {

                    if (buttonstate)
                    {
                        btn_ComputeSpan.Enabled = true;
                        btn_ReloadAndRecompute.Enabled = true;
                    }
                    else
                    {
                        btn_ComputeSpan.Enabled = false;
                        btn_ReloadAndRecompute.Enabled = false;
                    }

                }));
            }
            catch (Exception) { }
        }

        private void _DumpData_eve_SpanTime(double SpanTime)
        {
            this.Invoke((MethodInvoker)(() =>
            {

                var temp = ConvertFromUnixTimestamp(SpanTime);

                lbl_SpanTime.Text = ConvertFromUnixTimestamp(SpanTime).ToString();

            }));
        }

        private void _DumpData_eve_ConnectionStatus(bool isSpanConnected, bool isEngineConnected)
        {
            this.Invoke((MethodInvoker)(() =>
            {

                ind_EngineConnected.Visible = isEngineConnected;
                ind_EngineNotConnected.Visible = !isEngineConnected;

                ind_SpanConnected.Visible = isSpanConnected;
                ind_SpanNotConnected.Visible = !isSpanConnected;

            }));
        }

        private void btn_ReloadAndRecompute_Click(object sender, EventArgs e)
        {
            if (_DumpData != null)
                _DumpData.ReloadAndRecalculateMargin(true);
        }

        private void btn_ComputeSpan_Click(object sender, EventArgs e)
        {
            try
            {
                SetText("Computing Span...");   
                if (_DumpData != null)
                    _DumpData.ComputeSpan();
            }
            catch (Exception) { }
        }

        private void _DumpData_eve_TradeFileStatus(bool isFOAvailable)
        {
            try
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    
                    ind_TradeFileFOAvailable.Visible = isFOAvailable;
                    ind_TradeFileFO.Visible = !isFOAvailable;

                }));
            }
            catch (Exception ee) { _logger.Error("_DumpData_eve_TradeFileStatus : " + ee); }
        }

        private void chk_TAndC_CheckedChanged(object sender, EventArgs e)
        {
            
            btn_StartPick.Enabled = true;
            btn_ReloadAndRecompute.Enabled = true;
            btn_ComputeSpan.Enabled = true;
            linkLbl_TAndC.Visible = false;
            panel_TAndC.Visible = false;
            chk_TAndC.Visible = false;
        }

        private void linkLbl_TAndC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frm_TAndC TC = new frm_TAndC();
            TC.ShowDialog();
        }

        private void TradePicker_Resize(object sender, EventArgs e)
        {
            try
            {
                if (FormWindowState.Minimized == this.WindowState)
                {
                    notifyIconGateway.Visible = true;
                    this.Hide();
                }
                else
                {
                    this.Show();
                    notifyIconGateway.Visible = false;
                }
            }
            catch (Exception errror)
            {
                _logger.Error("TradePicker_Resize : " + errror);
            }
        }

        private void TradePicker_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (XtraMessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    Environment.Exit(0);
                }
                catch (Exception ee)
                {
                    _logger.Error("TradePicker_FormClosing : " + ee);
                }
            }
            else
                e.Cancel = true;
        }

        private void notifyIconGateway_Click(object sender, EventArgs e)
        {
            try
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;

                //added on 15JAN2021 by Amey
                notifyIconGateway.Visible = false;
            }
            catch (Exception errror)
            {
                _logger.Error("notifyIconGateway_Click : " + errror);
            }
        }

        private void TradePicker_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity = (System.IntPtr)2;
            lbl_SpanTime.Text = "01-01-1980 00:00:00";
        }



        #endregion

       
    }
}