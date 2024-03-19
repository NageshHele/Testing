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
using n.Structs;
using NSEUtilitaire;
using static NSEUtilitaire.Exchange;
using Gateway.Core_Logic;
using Security = NSEUtilitaire.Exchange.Security;//added by Omkar
using static DevExpress.XtraEditors.TextEdit;
using System.Diagnostics.Contracts;
using crypto;

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
                _logger = new clsWriteLog(Application.StartupPath.ToString() + "\\Log");
                ReadConfig();

                InitializeComponent();

                CheckMaxAllowedSqlPacket();

                SetText("n.Gateway initialised successfully.");


                //StartBODconnector();
            }
            catch (Exception ee) { XtraMessageBox.Show("Error while Initialising n.Gateway. " + ee, "Error"); Environment.Exit(0); }
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
                ip_GatewayHBServer = IPAddress.Parse(CONInfo["GATEWAY-SERVER-IP"].ToString());
                _GatewayHBServerPORT = Convert.ToInt32(CONInfo["GATEWAY-SERVER-HB-PORT"].ToString());

                try
                {
                    var SpanInfo = ds_Config.Tables["SPAN"].Rows[0];
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

        private async void btn_UploadTokens_Click(object sender, EventArgs e)
        {
            try
            {
                SetText("Upload Tokens started.");

                btn_UploadTokens.Enabled = false;

                //added on 13JAN2021 by Amey
                btn_StartPick.Enabled = false;
                btn_ReloadAndRecompute.Enabled = false;

                await Task.Run(() => InsertTokensIntoDBNew());

                SetText("Upload Tokens completed successfully.");

                //added on 13JAN2021 by Amey
                btn_UploadTokens.Enabled = true;

                //added on 13JAN2021 by Amey
                btn_StartPick.Enabled = true;
                btn_ReloadAndRecompute.Enabled = true;
            }
            catch (Exception ee)
            {
                _logger.Error("btn_UploadTokens_Click : " + ee);

                SetText("Upload Tokens failed. Please check logs for more details.");
            }
        }

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

                btn_UploadTokens.Enabled = false;
                btn_StartPick.Enabled = false;

                progressPanel_Start.Visible = true;

                //changed on 13JAN2021 by Amey
                //added params on 15JAN2021 by Amey
                _DumpData = new nProcess(_MySQLCon, ip_GatewayHBServer, _GatewayHBServerPORT, _logger, ds_Config, _SpanFolderPath);

                //added on 25JAN2021 by Amey
                ind_TradeFileCMAvailable.ToolTip = CTradeProcess.Instance._TradeFileCM;
                ind_TradeFileCM.ToolTip = CTradeProcess.Instance._TradeFileCM;
                ind_TradeFileFOAvailable.ToolTip = CTradeProcess.Instance._TradeFileFO;
                ind_TradeFileFO.ToolTip = CTradeProcess.Instance._TradeFileFO;

                //added on 20APR2021 by Amey
                ind_TradeFileBSECMAvailable.ToolTip = CTradeProcess.Instance._TradeFileBSECM;
                ind_TradeFileBSECM.ToolTip = CTradeProcess.Instance._TradeFileBSECM;

                //added by Omkar
                ind_TradeFileBSEFOAvailable.ToolTip = CTradeProcess.Instance._TradeFileBSEFO;
                ind_TradeFileBSEFO.ToolTip = CTradeProcess.Instance._TradeFileBSEFO;

                //added on 22JAN2021 by Amey
                _DumpData.eve_AddToList += SetText;
                _DumpData.eve_TradeFileStatus += _DumpData_eve_TradeFileStatus;

                Thread t_ReadIntradayFiles = new Thread(_DumpData.ReadIntradayFiles);
                t_ReadIntradayFiles.IsBackground = true;
                t_ReadIntradayFiles.Start();
            }
            catch (Exception ee)
            {
                _logger.Error("btn_UploadTokens_Click : " + ee);

                SetText("n.Gateway Start failed. Please check logs for more details.");
            }
        }

        private void btn_ReloadAndRecompute_Click(object sender, EventArgs e)
        {
            if (_DumpData != null)
                _DumpData.ReloadAndRecalculateMargin(true);


            if (_DumpData != null)
                _DumpData.ReloadAndRecalculateCDSMargin(true);
        }

        private void _DumpData_eve_TradeFileStatus(bool isFOAvailable, bool isCMAvailable, bool isBSECMAvailable, bool isBSEFOAvailable, bool isCDAvailable)
        {
            try
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    ind_TradeFileCMAvailable.Visible = isCMAvailable;
                    ind_TradeFileCM.Visible = !isCMAvailable;

                    ind_TradeFileFOAvailable.Visible = isFOAvailable;
                    ind_TradeFileFO.Visible = !isFOAvailable;

                    //added on 20APR2021 by Amey
                    ind_TradeFileBSECMAvailable.Visible = isBSECMAvailable;
                    ind_TradeFileBSECM.Visible = !isBSECMAvailable;

                    //added by Omkar
                    ind_TradeFileBSEFOAvailable.Visible = isBSEFOAvailable;
                    ind_TradeFileBSEFO.Visible = !isBSEFOAvailable;

                    //Added by Akshay on 15-11-2021 for cds files
                    ind_TradeFileCDAvailable.Visible = isCDAvailable;
                    ind_TradeFileCD.Visible = !isCDAvailable;
                }));
            }
            catch (Exception ee) { _logger.Error("_DumpData_eve_TradeFileStatus : " + ee); }
        }

        private void chk_TAndC_CheckedChanged(object sender, EventArgs e)
        {
            btn_UploadTokens.Enabled = true;
            btn_StartPick.Enabled = true;
            btn_ReloadAndRecompute.Enabled = true;

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
            StartBODconnector();
        }

        #endregion

        #region Important Methods

        public void InsertTokensIntoDB()
        {
            try
            {
                List<string> list_ContractMasterRows = new List<string>();

                StringBuilder sb_InsertCommand = new StringBuilder("TRUNCATE contractmaster; " +
                    "INSERT INTO contractmaster(TokenNo,Expiry,OExpiry,OScripName,ScripName,LotSize) VALUES");

                #region CM Security

                try
                {
                    using (System.IO.FileStream stream = File.Open("C:\\Prime\\security.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string line;
                            string[] fields;

                            while ((line = reader.ReadLine()) != null)
                            {
                                fields = line.Split(new[] { "|" }, StringSplitOptions.None);
                                if (fields.Length > 4)
                                {
                                    string Token = fields[0];
                                    DateTime SExpiry = ConvertFromUnixTimestamp(Convert.ToDouble(0));

                                    string Day = SExpiry.ToString("dd-mm-yyyy", CultureInfo.InvariantCulture).Substring(0, 2);
                                    string monthName = SExpiry.ToString("MMM", CultureInfo.InvariantCulture).ToUpper();
                                    string year = SExpiry.ToString("dd-mm-yyyy", CultureInfo.InvariantCulture).Substring(6, 4);

                                    string OExpiry = Day + monthName.ToUpper() + year;

                                    //changed ScripName format for CM scrips to Underlying-Series excluding EQ scrips. 23MAR2021 by Amey
                                    string OScripname = fields[1] + (fields[2] == "EQ" ? "" : $"-{fields[2]}");

                                    string LotSize = "1";

                                    string Scripname = fields[1].Trim() + "_|0_|" + fields[2] + "_|0";
                                    list_ContractMasterRows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}')", MySqlHelper.EscapeString(Token), MySqlHelper.EscapeString((Convert.ToDouble(0)).ToString()), MySqlHelper.EscapeString(OExpiry), MySqlHelper.EscapeString(OScripname), MySqlHelper.EscapeString(Scripname), MySqlHelper.EscapeString(LotSize)));
                                }
                            }
                        }
                    }

                    //added on 11JAN2021 by Amey
                    if (File.Exists("C:\\Prime\\IndexTokens.csv"))
                    {
                        foreach (var item in File.ReadAllLines("C:\\Prime\\IndexTokens.csv"))
                        {
                            string[] arr_Fields = item.Split(',');
                            string IndexName = arr_Fields[0];
                            string CustomScripName = $"{IndexName}_|0_|EQ_|0";
                            list_ContractMasterRows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}')", MySqlHelper.EscapeString(arr_Fields[1]), MySqlHelper.EscapeString("0"), MySqlHelper.EscapeString("01JAN1980"), MySqlHelper.EscapeString(IndexName), MySqlHelper.EscapeString(CustomScripName), MySqlHelper.EscapeString("1")));
                        }
                    }
                    else
                        XtraMessageBox.Show("Index Token file is not available.", "Error");

                    sb_InsertCommand.Append(string.Join(",", list_ContractMasterRows));
                    sb_InsertCommand.Append("ON DUPLICATE KEY UPDATE `OScripName`= Values(`OScripName`),`ScripName`= Values(`ScripName`),`OExpiry`= Values(`OExpiry`),`Expiry`= Values(`Expiry`),`LotSize`= Values(`LotSize`) ;");

                    using (MySqlConnection myConnToken = new MySqlConnection(_MySQLCon))
                    {
                        using (MySqlCommand myCmd = new MySqlCommand(sb_InsertCommand.ToString(), myConnToken))
                        {
                            myConnToken.Open();
                            myCmd.CommandType = CommandType.Text;
                            myCmd.ExecuteNonQuery();
                            myConnToken.Close();
                        }
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error("InsertTokensIntoDB CM : " + ee);

                    SetText("CM Tokens Upload failed. Please check logs for more details.");
                }

                #endregion

                #region FO Contract

                list_ContractMasterRows.Clear();
                sb_InsertCommand.Clear();
                sb_InsertCommand = new StringBuilder("INSERT INTO contractmaster(TokenNo,Expiry,OExpiry,OScripName,ScripName,LotSize) VALUES");

                try
                {
                    using (FileStream stream = File.Open("C:\\Prime\\contract.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string line;
                            string[] fields;
                            string OScripname = "";

                            while ((line = reader.ReadLine()) != null)
                            {
                                fields = line.Split('|');
                                if (fields.Length > 3)
                                {
                                    string Token = fields[0];
                                    DateTime SExpiry = ConvertFromUnixTimestamp(Convert.ToDouble(fields[6]) - 52200);

                                    string Day = SExpiry.ToString("dd-mm-yyyy", CultureInfo.InvariantCulture).Substring(0, 2);
                                    string monthName = SExpiry.ToString("MMM", CultureInfo.InvariantCulture).ToUpper();
                                    string year = SExpiry.ToString("dd-mm-yyyy", CultureInfo.InvariantCulture).Substring(8, 2); //changed on 20-12-17

                                    string OExpiry = Day + monthName.ToUpper() + year;
                                    OScripname = fields[53];

                                    string LotSize = fields[30];

                                    //changed Strike to "0" on 05JAN2021 by Amey
                                    string Scripname = fields[3].Trim() + "_|" + (Convert.ToInt32(fields[6]) - 52200) + "_|" + (fields[8].Trim() == "XX" ? "FUT" : fields[8].Trim()) + "_|" + (fields[8].Trim() == "XX" ? "0" : (Convert.ToDouble(fields[7]) / 100).ToString());

                                    list_ContractMasterRows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}')", MySqlHelper.EscapeString(Token), MySqlHelper.EscapeString((Convert.ToDouble(fields[6]) - 52200).ToString()), MySqlHelper.EscapeString(OExpiry), MySqlHelper.EscapeString(OScripname), MySqlHelper.EscapeString(Scripname), MySqlHelper.EscapeString(LotSize)));
                                }
                            }
                        }
                    }

                    sb_InsertCommand.Append(string.Join(",", list_ContractMasterRows));
                    sb_InsertCommand.Append("ON DUPLICATE KEY UPDATE `OScripName`= Values(`OScripName`),`ScripName`= Values(`ScripName`),`OExpiry`= Values(`OExpiry`),`Expiry`= Values(`Expiry`),`LotSize`= Values(`LotSize`) ;");

                    using (MySqlConnection myConnToken = new MySqlConnection(_MySQLCon))
                    {
                        using (MySqlCommand myCmd = new MySqlCommand(sb_InsertCommand.ToString(), myConnToken))
                        {
                            myConnToken.Open();
                            myCmd.CommandType = CommandType.Text;
                            myCmd.ExecuteNonQuery();
                            myConnToken.Close();
                        }
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error("InsertTokensIntoDB FO : " + ee);

                    SetText("FO Tokens Upload failed. Please check logs for more details.");
                }

                #endregion
            }
            catch (Exception) { throw; }
        }

        //added by Omkar for Testing

        //public static List<BSEFOContract> ReadBSEFOContract()
        //{
        //    List<BSEFOContract> list = new List<BSEFOContract>();
        //    try
        //    {
        //        DirectoryInfo _PrimeDirectory = new DirectoryInfo("C:\\Prime\\");
        //        var BSEFOSecurity = _PrimeDirectory.GetFiles("BSE_EQD_CONTRACT_*.csv").OrderByDescending(v => v.LastWriteTime).ToList();


        //        string[] array = File.ReadAllLines(BSEFOSecurity[0].FullName);
        //        for (int i = 1; i < array.Length; i++)
        //        {
        //            try
        //            {
        //                string[] bsefo_array = array[i].ToUpper().Split(',');
        //                BSEFOContract _BSEFO = new BSEFOContract();
        //                var Token = Convert.ToInt32(bsefo_array[0].Trim());
        //                var UnderlyingToken = Convert.ToInt32(bsefo_array[1].Trim());
        //                var Symbol = bsefo_array[3].Trim();
        //                string text = bsefo_array[2].Trim();
        //                //var Instrument = (text == "SO" ? "OPTSTK" : text == "SF" ? "FUTSTK" : text == "IF" ? "FUTIDX" : text == "IO" ? "OPTIDX" : "UNKNOWN");
        //                var Instrument = ((!text.Equals("SF")) ? (text.Equals("SO") ? "OPTSTK" : (text.Equals("IF") ? "FUTIDX" : "OPTIDX")) : "FUTSTK");
        //                var ScripName = bsefo_array[18].Trim();
        //                string text2 = bsefo_array[6].Trim();
        //                var ScripType = (text2.Equals("CE") ? "CE" : (text2.Equals("PE") ? "PE" : "XX"));
        //                var Expiry = DateTime.Parse(bsefo_array[4]).Date.AddHours(15.0).AddMinutes(30.0);
                        
        //                var StrikePrice = Convert.ToDouble(bsefo_array[5]);
        //                var LotSize = Convert.ToInt16(bsefo_array[8]);
        //                var CustomScripname = string.Format("{0}|{1}|{2}|{3}", _BSEFO.Symbol, _BSEFO.Expiry.ToString("ddMMMyyyy").ToUpper(), (_BSEFO.StrikePrice == 0.0) ? "0" : Math.Round(_BSEFO.StrikePrice, 2).ToString("#.00"), _BSEFO.ScripType);
        //                list.Add(_BSEFO);
        //            }
        //            catch (Exception)
        //            {
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    return list;
        //}
        //--

        public void InsertTokensIntoDBNew()
        {
            try
            {
                List<string> list_ContractMasterRows = new List<string>();
                List<Security> list_EQSecurity = new List<Security>();

                StringBuilder sb_InsertCommand = new StringBuilder("TRUNCATE tbl_contractmaster; ");

                #region CM Security

                try
                {
                    //added Exists check on 27APR2021 by Amey
                    if (File.Exists("C:\\Prime\\security.txt"))
                    {
                        sb_InsertCommand.Append("INSERT IGNORE INTO tbl_contractmaster(Token,Symbol,InstrumentName,Series,Segment,ScripName,CustomScripName,ScripType,ExpiryUnix,StrikePrice,LotSize,UnderlyingToken,UnderlyingSegment) VALUES");

                        var list_Security = Exchange.ReadSecurity("C:\\Prime\\security.txt",false);
                        list_EQSecurity = list_Security.Where(v => v.Series == "EQ").ToList();

                        foreach (var _Security in list_Security)
                        {
                            list_ContractMasterRows.Add($"({_Security.Token},'{_Security.Symbol}','EQ','{_Security.Series}','NSECM','{_Security.ScripName}'," +
                                $"'{_Security.CustomScripname}','EQ','{_Security.ExpiryUnix}',{0},{_Security.LotSize},{_Security.Token},'NSECM')");
                        }

                        if (File.Exists("C:\\Prime\\IndexTokens.csv"))
                        {
                            foreach (var item in File.ReadAllLines("C:\\Prime\\IndexTokens.csv"))
                            {
                                string[] arr_Fields = item.Split(',');
                                string IndexName = arr_Fields[0];
                                string CustomScripName = $"{IndexName}|0|EQ|0";
                                list_ContractMasterRows.Add($"({arr_Fields[1]},'{IndexName}','EQ','{en_InstrumentName.EQ}','{arr_Fields[3]}CM','{IndexName}-EQ'," +
                                      $"'{CustomScripName}','EQ',{0},{0},{1},{arr_Fields[1]},'{arr_Fields[3]}CM')");

                                list_EQSecurity.Add(new Security() { Symbol = IndexName, Token = Convert.ToInt32(arr_Fields[1]) });
                            }
                        }
                        else
                            XtraMessageBox.Show("Index Token file is not available.", "Error");

                        sb_InsertCommand.Append(string.Join(",", list_ContractMasterRows));

                        using (MySqlConnection myConnToken = new MySqlConnection(_MySQLCon))
                        {
                            using (MySqlCommand myCmd = new MySqlCommand(sb_InsertCommand.ToString(), myConnToken))
                            {
                                myConnToken.Open();
                                myCmd.CommandType = CommandType.Text;
                                myCmd.ExecuteNonQuery();
                                myConnToken.Close();
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error("InsertTokensIntoDB CM : " + ee);

                    SetText("CM Tokens Upload failed. Please check logs for more details.");
                }

                #endregion

                #region FO Contract

                list_ContractMasterRows.Clear();
                sb_InsertCommand.Clear();

                try
                {
                    //added Exists check on 27APR2021 by Amey
                    if (File.Exists("C:\\Prime\\contract.txt"))
                    {
                        sb_InsertCommand = new StringBuilder("INSERT IGNORE INTO tbl_contractmaster(Token,Symbol,InstrumentName,Series,Segment,ScripName,CustomScripName,ScripType,ExpiryUnix,StrikePrice,LotSize,UnderlyingToken,UnderlyingSegment) VALUES");

                        var list_Contract = Exchange.ReadContract("C:\\Prime\\contract.txt",false);
                        var FutContracts = list_Contract.Where(v => v.ScripType == NSEUtilitaire.en_ScripType.XX).ToList();

                        foreach (var _Contract in list_Contract)
                        {
                            var USegment = "NSEFO";
                            var UnderlyingToken = -1;

                            if (_Contract.ScripType == NSEUtilitaire.en_ScripType.XX)
                                UnderlyingToken = _Contract.Token;
                            else
                            {
                                var temp = FutContracts.Where(v => v.Symbol == _Contract.Symbol && v.Expiry.Month == _Contract.Expiry.Month && v.Expiry.Year == _Contract.Expiry.Year).FirstOrDefault();
                                if (temp != null)
                                    UnderlyingToken = temp.Token;
                                else
                                {
                                    var twmp = list_EQSecurity.Where(v => v.Symbol == _Contract.Symbol).FirstOrDefault();
                                    if (twmp != null)
                                    {
                                        UnderlyingToken = twmp.Token;
                                        USegment = "NSECM";
                                    }
                                }
                            }

                            if (UnderlyingToken != -1)
                                list_ContractMasterRows.Add($"({_Contract.Token},'{_Contract.Symbol}','{_Contract.Instrument}','-','NSEFO','{_Contract.ScripName}'," +
                                    $"'{_Contract.CustomScripname}','{_Contract.ScripType}','{_Contract.ExpiryUnix}',{_Contract.StrikePrice},{_Contract.LotSize}," +
                                    $"{UnderlyingToken},'{USegment}')");
                        }

                        sb_InsertCommand.Append(string.Join(",", list_ContractMasterRows));

                        using (MySqlConnection myConnToken = new MySqlConnection(_MySQLCon))
                        {
                            using (MySqlCommand myCmd = new MySqlCommand(sb_InsertCommand.ToString(), myConnToken))
                            {
                                myConnToken.Open();
                                myCmd.CommandType = CommandType.Text;
                                myCmd.ExecuteNonQuery();
                                myConnToken.Close();
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error("InsertTokensIntoDB FO : " + ee);

                    SetText("FO Tokens Upload failed. Please check logs for more details.");
                }

                #endregion

                #region CD Contract

                list_ContractMasterRows.Clear();
                sb_InsertCommand.Clear();

                try
                {
                    //added Exists check on 27APR2021 by Amey
                    if (File.Exists("C:\\Prime\\cd_contract.txt"))
                    {
                        sb_InsertCommand = new StringBuilder("INSERT IGNORE INTO tbl_contractmaster(Token,Symbol,InstrumentName,Series,Segment,ScripName,CustomScripName,ScripType,ExpiryUnix,StrikePrice,LotSize,UnderlyingToken,UnderlyingSegment) VALUES");

                        var list_CDContract = Exchange.ReadCDContract("C:\\Prime\\cd_contract.txt",false);
                        var FutContracts = list_CDContract.Where(v => v.ScripType == NSEUtilitaire.en_ScripType.XX).ToList();

                        foreach (var _CDContract in list_CDContract)
                        {
                            var UnderlyingToken = -1;
                            if (_CDContract.ScripType == NSEUtilitaire.en_ScripType.XX)
                                UnderlyingToken = _CDContract.Token;
                            else
                            {
                                var temp = FutContracts.Where(v => v.Symbol == _CDContract.Symbol && v.Expiry.Month == _CDContract.Expiry.Month && v.Expiry.Year == _CDContract.Expiry.Year).FirstOrDefault();
                                if (temp != null)
                                    UnderlyingToken = temp.Token;
                            }

                            if (UnderlyingToken != -1)
                                list_ContractMasterRows.Add($"({_CDContract.Token},'{_CDContract.Symbol}','{_CDContract.Instrument}','-','NSECD','{_CDContract.ScripName}'," +
                                    $"'{_CDContract.CustomScripname}','{_CDContract.ScripType}','{_CDContract.ExpiryUnix}',{_CDContract.StrikePrice},{_CDContract.Multiplier}," +
                                    $"{UnderlyingToken},'NSECD')");
                        }

                        sb_InsertCommand.Append(string.Join(",", list_ContractMasterRows));

                        using (MySqlConnection myConnToken = new MySqlConnection(_MySQLCon))
                        {
                            using (MySqlCommand myCmd = new MySqlCommand(sb_InsertCommand.ToString(), myConnToken))
                            {
                                myConnToken.Open();
                                myCmd.CommandType = CommandType.Text;
                                myCmd.ExecuteNonQuery();
                                myConnToken.Close();
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error("InsertTokensIntoDB CD : " + ee);

                    SetText("CD Tokens Upload failed. Please check logs for more details.");
                }

                #endregion

                #region BSECM Security

                try
                {
                    list_ContractMasterRows.Clear();
                    sb_InsertCommand.Clear();
                    sb_InsertCommand.Append("INSERT IGNORE INTO tbl_contractmaster(Token,Symbol,InstrumentName,Series,Segment,ScripName,CustomScripName,ScripType,ExpiryUnix,StrikePrice,LotSize,UnderlyingToken,UnderlyingSegment) VALUES");

                    DirectoryInfo _PrimeDirectory = new DirectoryInfo("C:\\Prime\\");
                    var BSESecurity = _PrimeDirectory.GetFiles("SCRIP_*.txt").OrderByDescending(v => v.LastWriteTime).ToList();

                    if (BSESecurity.Any())
                    {
                        var list_Security = Exchange.ReadBSESecurity(BSESecurity[0].FullName,false);

                        foreach (var _Security in list_Security)
                        {
                            list_ContractMasterRows.Add($"({_Security.Token},'{_Security.Symbol}','EQ','{_Security.Series}','BSECM','{_Security.ScripName}'," +
                                $"'{_Security.CustomScripname}','EQ','{_Security.ExpiryUnix}',{0},{_Security.LotSize},{_Security.Token},'BSECM')");
                        }

                        sb_InsertCommand.Append(string.Join(",", list_ContractMasterRows));

                        using (MySqlConnection myConnToken = new MySqlConnection(_MySQLCon))
                        {
                            using (MySqlCommand myCmd = new MySqlCommand(sb_InsertCommand.ToString(), myConnToken))
                            {
                                myConnToken.Open();
                                myCmd.CommandType = CommandType.Text;
                                myCmd.ExecuteNonQuery();
                                myConnToken.Close();
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error("InsertTokensIntoDB BSECM : " + ee);

                    SetText("BSECM Tokens Upload failed. Please check logs for more details.");
                }

                #endregion

                //added by omkar 
                #region BSEFO Security
                try
                {
                    list_ContractMasterRows.Clear();
                    sb_InsertCommand.Clear();
                    
                    sb_InsertCommand.Append("INSERT IGNORE INTO tbl_contractmaster(Token,Symbol,InstrumentName,Series,Segment,ScripName,CustomScripName,ScripType,ExpiryUnix,StrikePrice,LotSize,UnderlyingToken,UnderlyingSegment) VALUES");

                    DirectoryInfo _PrimeDirectory = new DirectoryInfo("C:\\Prime\\");
                    var BSEFOSecurity = _PrimeDirectory.GetFiles("BSE_EQD_CONTRACT_*.csv").OrderByDescending(v => v.LastWriteTime).ToList();

                   
                    if (BSEFOSecurity.Any())
                    {
                        //var list2 = ReadBSEFOContract();//added for testing

                        var time = DateTime.Today;
                        var list_Security = Exchange.ReadBSEFOContract(BSEFOSecurity[0].FullName).Where(x=>x.Expiry >= DateTime.Today).ToList();
                        var FutContracts = list_Security.Where(v => v.ScripType == NSEUtilitaire.en_ScripType.XX).ToList();

                        foreach (var _Security in list_Security)
                        {
                            var USegment = "BSEFO";
                            var UnderlyingToken = -1;

                            if (_Security.ScripType == NSEUtilitaire.en_ScripType.XX)
                                UnderlyingToken = _Security.Token;
                            else
                            {
                                var temp = FutContracts.Where(v => v.Symbol == _Security.Symbol && v.Expiry.Month == _Security.Expiry.Month && v.Expiry.Year == _Security.Expiry.Year).FirstOrDefault();
                                if (temp != null)
                                    UnderlyingToken = temp.Token;
                                else
                                {
                                    var twmp = list_EQSecurity.Where(v => v.Symbol == _Security.Symbol).FirstOrDefault();
                                    if (twmp != null)
                                    {
                                        UnderlyingToken = twmp.Token;
                                        USegment = "BSECM";
                                    }
                                }
                            }

                            if (UnderlyingToken != -1)
                                list_ContractMasterRows.Add($"({_Security.Token},'{_Security.Symbol}','{_Security.Instrument}','-','BSEFO','{_Security.ScripName}'," +
                                $"'{_Security.CustomScripname}','{_Security.ScripType}','{_Security.ExpiryUnix}',{_Security.StrikePrice},{_Security.LotSize}," +
                                $"{UnderlyingToken},'BSEFO')");

                        }
                        sb_InsertCommand.Append(string.Join(",", list_ContractMasterRows));

                        using (MySqlConnection myConnToken = new MySqlConnection(_MySQLCon))
                        {
                            using (MySqlCommand myCmd = new MySqlCommand(sb_InsertCommand.ToString(), myConnToken))
                            {
                                myConnToken.Open();
                                myCmd.CommandType = CommandType.Text;
                                myCmd.ExecuteNonQuery();
                                myConnToken.Close();
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error("InsertTokensIntoDB BSEFO : " + ee);

                    SetText("BSEFO Tokens Upload failed. Please check logs for more details.");
                }
                #endregion
            }
            catch (Exception) { throw; }
        }

        #endregion
    }
}