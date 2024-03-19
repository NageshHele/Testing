using DevExpress.XtraEditors;
using Feed_Receiver_BSE.Core_Logic;
using Feed_Receiver_BSE.Helper;
using NerveLog;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Feed_Receiver_BSE.UI
{
    public partial class Main : XtraForm
    {
        NerveLogger _logger;

        public Main()
        {
            InitializeComponent();

            _logger = CollectionHelper._logger;

            SetFormText();

            gc_ConnectionInfo.DataSource = CollectionHelper.bList_ConnectionInfo;
            gv_ConnectionInfo.BestFitColumns();
        }

        #region UI Events

        private void Main_Shown(object sender, EventArgs e)
        {
            try
            {
                DateTime dt_ClosingTime = DateTime.Parse(CollectionHelper.GetFromConfig("CLOSING-TIME", "TIME").ToString());

                var startTimeSpan = TimeSpan.Zero;
                var periodTimeSpan = TimeSpan.FromMinutes(5);

                var timer = new Timer((v) =>
                {
                    CheckClosingTime(dt_ClosingTime);
                }, null, startTimeSpan, periodTimeSpan);

                ReadFiles _Read = new ReadFiles();
                _Read.ReadBhavcopy();
                _Read.ReadIndexClosing();

                SocketServer _Server = new SocketServer(gc_ConnectionInfo, gv_ConnectionInfo);
                _Server.Setup();

                SocketReceiver _Receiver = new SocketReceiver(lbl_LastTradedTime);
                //_Receiver.StartUDPApp();
                _Receiver.StartUDPReceive();
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        #endregion

        #region Imp Methods

        private void SetFormText()
        {
            try
            {
                lbl_IP.Text = "IP : "+CollectionHelper.GetFromConfig("EXCHANGE-CONNECTION", "IP").ToString();
                lbl_PORT.Text ="Port : "+ CollectionHelper.GetFromConfig("EXCHANGE-CONNECTION", "PORT").ToString();

                lbl_LastTradedTime.Text = "LTT : " + DateTime.Now.ToString("HH:mm:ss");

                var BHAVCOPYPATH = CollectionHelper.GetFromConfig("FILE-PATH", "BHAVCOPY").ToString();
                string[] files = Directory.GetFiles(BHAVCOPYPATH, "EQ_ISINCODE_*.csv");
                //if (!files.Any())
                //    files = Directory.GetFiles(BHAVCOPYPATH, "EQ_ISINCODE_*.csv");
                if (files.Any())
                    lbl_Bhavcopy.Text = "Bhavcopy : " + File.GetLastWriteTime(files[0]).Date;

                var SECURITYPATH = CollectionHelper.GetFromConfig("FILE-PATH", "SECURITY").ToString();
                files = Directory.GetFiles(SECURITYPATH, "Scrip*.txt");
                if (files.Any())
                    lbl_Security.Text = "Security : " + File.GetLastWriteTime(files[0]).Date;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void CheckClosingTime(DateTime dt_ClosingTime)
        {
            if (DateTime.Now > dt_ClosingTime)
            {
                Environment.Exit(0);
            }
        }

        #endregion
    }
}