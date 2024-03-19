using DevExpress.XtraEditors;
using n.Structs;
using NerveLog;
using Prime.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Prime.UI
{
    public partial class form_Login : XtraForm
    {
        NerveLogger _logger;

        /// <summary>
        /// Main form object.
        /// </summary>
        form_Main _Main;

        /// <summary>
        /// Used to receive Heartbeat data from Engine. Uses _EngineServerHBPORT.
        /// </summary>
        EngineHBConnector _EngineHeartBeatClient = new EngineHBConnector();

        /// <summary>
        /// ENGINE-SERVER-IP from Config.
        /// </summary>
        IPAddress ip_EngineServer = IPAddress.Parse("127.0.0.1");

        /// <summary>
        /// ENGINE-SERVER-HB-PORT from Config.
        /// </summary>
        int _EngineServerHBPORT;

        /// <summary>
        /// Set True when Login process is successful.
        /// </summary>
        bool isClosedProgramatically = false;

        public form_Login(form_Main _Main)
        {
            InitializeComponent();

            this._logger = CollectionHelper._logger;
            this._Main = _Main;

            txt_Username.Focus();

            _EngineHeartBeatClient.eve_HeartBeatTickReceived += _EngineHeartBeatClient_eve_HeartBeatTickReceived;
            _EngineHeartBeatClient.eve_LoginResponse += _EngineHeartBeatClient_eve_LoginResponse;
            _EngineHeartBeatClient.eve_ClientInfoReceived += _EngineHeartBeatClient_eve_ClientInfoReceived;
        }

        #region Engine HeartBeat Socket Events

        private void _EngineHeartBeatClient_eve_ClientInfoReceived(bool isReceived, string ID = "")
        {
            if (isReceived)
            {
                
                //added on 07APR2021 by Amey
                //_EngineHeartBeatClient.SendToEngine("BAN^" + txt_Username.Text.ToLower());

                XtraMessageBox.Show("Login Successful.", "Success");


                _Main.Initialise(_EngineHeartBeatClient, txt_Username.Text.ToUpper());

                isClosedProgramatically = true;

                this.Invoke((MethodInvoker)(() =>
                {
                    this.Close();
                }));
            }
        }

        private void _EngineHeartBeatClient_eve_LoginResponse(bool isSuccess, string _Message)
        {
            if (isSuccess)
            {
                _EngineHeartBeatClient.Username = txt_Username.Text.ToLower();
                _EngineHeartBeatClient.SendToEngine("CLIENT^" + txt_Username.Text.ToLower());
                //_EngineHeartBeatClient.SendToEngine("LIMIT^" + txt_Username.Text.ToLower());
                _EngineHeartBeatClient.SendToEngine("BAN^" + txt_Username.Text.ToLower());
            }
            else
            {
                XtraMessageBox.Show(_Message, "Error");

                this.Invoke((MethodInvoker)(() =>
                {
                    btn_Login.Enabled = true;
                }));
            }
        }

        private void _EngineHeartBeatClient_eve_HeartBeatTickReceived(string prmTick) { }

        #endregion

        #region Form Events

        private void chk_TAndC_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_TAndC.CheckState == CheckState.Checked)
            {
                btn_Login.Enabled = true;
                linkLabel_TAndC.Enabled = false;
            }
            else
            {
                btn_Login.Enabled = false;
                linkLabel_TAndC.Enabled = true;
            }
        }

        private void linkLabel_TAndC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TermsAndConditions term = new TermsAndConditions();
            term.ShowDialog(this);
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_Username.Text != "" && txt_Password.Text != "")
                {
                    btn_Login.Enabled = false;

                    //added on 24FEB2021 by Amey
                    ReadConfig();

                    _EngineHeartBeatClient.ConnectToEngine(ip_EngineServer, _EngineServerHBPORT);

                    Thread.Sleep(1000);

                    var VersionInfo = FileVersionInfo.GetVersionInfo("Prime.exe");
                    string Version = VersionInfo.FileVersion;

                    if (!_EngineHeartBeatClient.SendToEngine("USER^" + txt_Username.Text.ToLower() + "|" + txt_Password.Text + "|" + Version))
                    {
                        XtraMessageBox.Show("Unable to connect to server. Please contact System Administrator.");
                        btn_Login.Enabled = true;
                    }
                    else
                        btn_Login.Enabled = false;
                }
                else
                    XtraMessageBox.Show("Enter login Details");
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isClosedProgramatically)
                Environment.Exit(0);
        }

        #endregion

        #region Supplimentary Methods

        private void ReadConfig()
        {
            try
            {
                var CONInfo = ConfigurationManager.AppSettings;

                ip_EngineServer = IPAddress.Parse(CONInfo["ENGINE-SERVER-IP"].ToString());
                _EngineServerHBPORT = Convert.ToInt32(CONInfo["ENGINE-SERVER-HB-PORT"]);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        #endregion

        private void btn_Settings_Click(object sender, EventArgs e)
        {
            try
            {
                new form_Settings().ShowDialog();
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
    }
}