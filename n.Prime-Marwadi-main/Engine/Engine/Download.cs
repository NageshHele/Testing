using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using System.IO.Compression;
using System.Globalization;
using MySql.Data.MySqlClient;
using System.Net;

namespace Engine
{
    public partial class Download : DevExpress.XtraEditors.XtraForm
    {
        clsWriteLog objWriteLog;
        string SpanNameFO, SpanNameCD;
        public static bool downloadFlag = true;
        public static int downloadStatus = 0;
        public Download()
        {
            InitializeComponent();
            objWriteLog = new clsWriteLog();
        }

        private void btn_DownloadFiles_Click(object sender, EventArgs e)
        {
            try
            {
                DeletePreviousFile();
                CreateSpanFile();
                DownloadFile();
            }
            catch (Exception ee)
            {
                if (!EngineProcess.exceptionsList.Contains(ee.Message))
                {
                    InsertError(ee.StackTrace.ToString().Substring(ee.StackTrace.ToString().Length - 10));
                }
            }

        }

        void DeletePreviousFile()
        {
            try
            {
                string[] filePaths = Directory.GetFiles(@"c:\Prime\", "*.spn");
                string[] filePathsFO = Directory.GetFiles(@"c:\Prime\", "fo*.csv");
                string[] filePathsCM = Directory.GetFiles(@"c:\Prime\", "cm*.csv");

                foreach (var item in filePaths)
                {
                    File.Delete(item);
                }
                foreach (var item in filePathsFO)
                {
                    File.Delete(item);
                }
                foreach (var item in filePathsCM)
                {
                    File.Delete(item);
                }
            }
            catch (Exception deleteEx)
            {
                if (!EngineProcess.exceptionsList.Contains(deleteEx.Message))
                {
                    InsertError(deleteEx.Message + ":" + deleteEx.StackTrace.ToString().Substring(deleteEx.StackTrace.ToString().Length - 10));
                }
            }

        }

        void CreateSpanFile()
        {
            try
            {
                SpanNameFO = "https://www.nseindia.com/archives/nsccl/span/nsccl." + dateTimePicker.Value.Year.ToString("0000") + dateTimePicker.Value.Month.ToString("00") + dateTimePicker.Value.Day.ToString("00");
                SpanNameCD = "https://www.nseindia.com/archives/cd/span/nsccl_x." + dateTimePicker.Value.Year.ToString("0000") + dateTimePicker.Value.Month.ToString("00") + dateTimePicker.Value.Day.ToString("00");
                switch (comboBoxEdit_FileType.SelectedIndex)
                {

                    case 1:

                        if (comboBoxEdit_SpanType.SelectedIndex == 1)
                        {
                            SpanNameFO = SpanNameFO + ".i1.zip";
                            SpanNameCD = SpanNameCD + ".i1.zip";
                        }
                        else if (comboBoxEdit_SpanType.SelectedIndex == 2)
                        {
                            SpanNameFO = SpanNameFO + ".i1_1.zip";
                            SpanNameCD = SpanNameCD + ".i1.zip";

                        }
                        else
                        {
                            XtraMessageBox.Show("Please select appropriate span type");
                            downloadFlag = false;
                        }
                        break;
                    case 2:
                        if (comboBoxEdit_SpanType.SelectedIndex == 1)
                        {
                            SpanNameFO = SpanNameFO + ".i2.zip";
                            SpanNameCD = SpanNameCD + ".i2.zip";
                        }
                        else if (comboBoxEdit_SpanType.SelectedIndex == 2)
                        {
                            SpanNameFO = SpanNameFO + ".i2_1.zip";
                            SpanNameCD = SpanNameCD + ".i2.zip";
                        }
                        else
                        {
                            XtraMessageBox.Show("Please select appropriate span type");
                            downloadFlag = false;
                        }
                        break;
                    case 3:
                        if (comboBoxEdit_SpanType.SelectedIndex == 1)
                        {
                            SpanNameFO = SpanNameFO + ".i3.zip";
                            SpanNameCD = SpanNameCD + ".i3.zip";
                        }
                        else if (comboBoxEdit_SpanType.SelectedIndex == 2)
                        {
                            SpanNameFO = SpanNameFO + ".i3_1.zip";
                            SpanNameCD = SpanNameCD + ".i3.zip";
                        }
                        else
                        {
                            XtraMessageBox.Show("Please select appropriate span type");
                            downloadFlag = false;
                        }
                        break;
                    case 4:
                        if (comboBoxEdit_SpanType.SelectedIndex == 1)
                        {
                            SpanNameFO = SpanNameFO + ".i4.zip";
                            SpanNameCD = SpanNameCD + ".i4.zip";
                        }
                        else if (comboBoxEdit_SpanType.SelectedIndex == 2)
                        {
                            SpanNameFO = SpanNameFO + ".i4_1.zip";
                            SpanNameCD = SpanNameCD + ".i4.zip";
                        }
                        else
                        {
                            XtraMessageBox.Show("Please select appropriate span type");
                            downloadFlag = false;
                        }
                        break;
                    case 5:
                        if (comboBoxEdit_SpanType.SelectedIndex == 1)
                        {
                            SpanNameFO = SpanNameFO + ".i5.zip";
                            SpanNameCD = SpanNameCD + ".i4.zip";
                        }
                        else if (comboBoxEdit_SpanType.SelectedIndex == 2)
                        {
                            SpanNameFO = SpanNameFO + ".i5_1.zip";
                            SpanNameCD = SpanNameCD + ".i4.zip";
                        }
                        else
                        {
                            XtraMessageBox.Show("Please select appropriate span type");
                            downloadFlag = false;
                        }
                        break;
                    case 6:
                        if (comboBoxEdit_SpanType.SelectedIndex == 1)
                        {
                            SpanNameFO = SpanNameFO + ".s.zip";
                            SpanNameCD = SpanNameCD + ".s.zip";
                        }
                        else if (comboBoxEdit_SpanType.SelectedIndex == 2)
                        {
                            SpanNameFO = SpanNameFO + ".s_1.zip";
                            SpanNameCD = SpanNameCD + ".s.zip";
                        }
                        else
                        {
                            XtraMessageBox.Show("Please select appropriate span type");
                            downloadFlag = false;
                        }
                        break;
                    default:
                        XtraMessageBox.Show("Please select proper file type");
                        downloadFlag = false;
                        break;
                }
            }
            catch (Exception span)
            {
                if (!EngineProcess.exceptionsList.Contains(span.Message))
                {
                    InsertError(span.StackTrace.ToString().Substring(span.StackTrace.ToString().Length - 10));
                }
            }
        }

        void DownloadFile()
        {
            try
            {
                string monthStr = dateTimePicker.Value.ToString("MMM", CultureInfo.InvariantCulture).ToUpper();
                string year = dateTimePicker.Value.Year.ToString("0000");
                string fileNameFO, fileNameCM;
                if (DateTime.Now.Hour >= 17 || (dateTimePicker.Value.Date < DateTime.Now.Date))
                {
                    fileNameFO = year + "//" + monthStr + "//fo" + dateTimePicker.Value.Day.ToString("00") + monthStr + year + "bhav.csv.zip";
                    fileNameCM = year + "//" + monthStr + "//cm" + dateTimePicker.Value.Day.ToString("00") + monthStr + year + "bhav.csv.zip";
                    //fileName = year + "//" + monthStr + "//fo" + DateTime.Now.Day.ToString("00") + monthStr + year + "bhav.csv.zip";
                }
                else
                {
                    fileNameFO = year + "//" + monthStr + "//fo" + (dateTimePicker.Value.Day - 1).ToString("00") + monthStr + year + "bhav.csv.zip";
                    fileNameCM = year + "//" + monthStr + "//cm" + (dateTimePicker.Value.Day - 1).ToString("00") + monthStr + year + "bhav.csv.zip";
                }
                fileNameFO = "https://www.nseindia.com/content/historical/DERIVATIVES/" + fileNameFO;
                fileNameCM = "https://www.nseindia.com/content/historical/EQUITIES/" + fileNameCM;
                if (!fileNameFO.StartsWith("http://") && !fileNameFO.StartsWith("https://"))
                {
                    fileNameFO = "http://" + fileNameFO;
                }
                if (!fileNameCM.StartsWith("http://") && !fileNameCM.StartsWith("https://"))
                {
                    fileNameCM = "http://" + fileNameCM;
                }
                try
                {
                    webBrowserFO.Navigate(new Uri(fileNameFO));
                    webBrowserCM.Navigate(new Uri(fileNameCM));
                    webBrowser_SpanFO.Navigate(new Uri(SpanNameFO));
                    webBrowser_SpanCD.Navigate(new Uri(SpanNameCD));

                }
                catch (System.UriFormatException Error)
                {
                    if (!EngineProcess.exceptionsList.Contains(Error.Message))
                    {
                        InsertError(Error.StackTrace.ToString().Substring(Error.StackTrace.ToString().Length - 10));
                    }
                }
            }
            catch (Exception error)
            {
                if (!EngineProcess.exceptionsList.Contains(error.Message))
                {
                    InsertError(error.StackTrace.ToString().Substring(error.StackTrace.ToString().Length - 10));
                }
            }

        }

        void ExtractZip()
        {
            try
            {
                string[] filePaths = Directory.GetFiles(@"c:\Prime", "*.zip");
                for (int i = 0; i < filePaths.Length; i++)
                {
                    ZipFile.ExtractToDirectory(filePaths[i], @"c:\Prime\");
                }
                DeleteZip();
            }
            catch (Exception errorZip)
            {
                if (errorZip.Message.ToString().Contains("already exists"))
                {
                    DeleteZip();
                    return;
                }
                if (!EngineProcess.exceptionsList.Contains(errorZip.Message))
                {
                    InsertError(errorZip.Message + ":" + errorZip.StackTrace.ToString().Substring(errorZip.StackTrace.ToString().Length - 10));
                }
            }

        }

        void DeleteZip()
        {
            try
            {
                string[] filePaths = Directory.GetFiles(@"c:\Prime\", "*.zip");
                foreach (var item in filePaths)
                {
                    File.Delete(item);
                }

            }
            catch (Exception deleteEx)
            {
                if (!EngineProcess.exceptionsList.Contains(deleteEx.Message))
                {
                    InsertError(deleteEx.Message + ":" + deleteEx.StackTrace.ToString().Substring(deleteEx.StackTrace.ToString().Length - 10));
                }
            }
        }

        public void InsertError(string error)
        {
            try
            {
                if (EngineProcess._downloadConn.State != ConnectionState.Open)
                {
                    EngineProcess._downloadConn.Open();
                }
                objWriteLog.WriteLog("Engine_EP_" + error + "_" + DateTime.Now);
                MySqlCommand errCmd = new MySqlCommand("insert into errorlog values('Engine','EP',Now(),'" + error + "')", EngineProcess._downloadConn);
                errCmd.ExecuteNonQuery();
                errCmd.Dispose();
            }
            catch (Exception errorlogEx)
            {
                if (!EngineProcess.exceptionsList.Contains(errorlogEx.Message))
                {
                    objWriteLog.WriteLog("Error Occured in Engine. The Application will now Exit :_" + error + " _ "+ errorlogEx.Message +" _ "+ DateTime.Now);
                    XtraMessageBox.Show("Error Occured in Engine. The Application will now Exit : " + error);
                }
            }
        }

        private void WebBrowserCM_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            try
            {
                e.Cancel = true;
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", "Only a test!");
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(cm_DownloadDataCompleted);
                client.UseDefaultCredentials = true;
                client.DownloadDataAsync(e.Url);
                client.Dispose();
            }
            catch (Exception ee)
            {
                if (!EngineProcess.exceptionsList.Contains(ee.Message))
                {
                    InsertError(ee.StackTrace.ToString().Substring(ee.StackTrace.ToString().Length - 10));
                }
            }
        }

        private void WebBrowserFO_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            try
            {
                e.Cancel = true;
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", "Only a test!");
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(fo_DownloadDataCompleted);
                client.UseDefaultCredentials = true;
                client.DownloadDataAsync(e.Url);
                client.Dispose();
            }
            catch (Exception ee)
            {
                if (!EngineProcess.exceptionsList.Contains(ee.Message))
                {
                    InsertError(ee.StackTrace.ToString().Substring(ee.StackTrace.ToString().Length - 10));
                }
            }
        }

        private void webBrowser_Span_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            try
            {
                e.Cancel = true;
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", "Only a test!");
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(span_DownloadDataCompleted);
                client.UseDefaultCredentials = true;

                client.DownloadDataAsync(e.Url);
                client.Dispose();
            }
            catch (Exception ee)
            {
                if (!EngineProcess.exceptionsList.Contains(ee.Message))
                {
                    InsertError(ee.StackTrace.ToString().Substring(ee.StackTrace.ToString().Length - 10));
                }
            }
        }

        private void webBrowser_SpanCD_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            try
            {
                e.Cancel = true;
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", "Only a test!");
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(spanCD_DownloadDataCompleted);
                client.UseDefaultCredentials = true;
                client.DownloadDataAsync(e.Url);
                client.Dispose();
            }
            catch (Exception ee)
            {
                if (!EngineProcess.exceptionsList.Contains(ee.Message))
                {
                    InsertError(ee.StackTrace.ToString().Substring(ee.StackTrace.ToString().Length - 10));
                }
            }
        }
        public void fo_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs d)
        {
            try
            {
                string filepath = @"c:\Prime\fo.zip";
                File.WriteAllBytes(filepath, d.Result);
                ExtractZip();
                downloadStatus = 1;
                //b_ErrorLog.Items.Insert(0, "Latest files have been downloaded");
            }
            catch (Exception foBhavEx)
            {
                if (foBhavEx.Message.ToString().Contains("An exception occurred during the operation, making the result invalid"))
                {
                    XtraMessageBox.Show("Bhavcopy for FO not available, please select another date");
                    return;
                }
                if (!EngineProcess.exceptionsList.Contains(foBhavEx.Message))
                {
                    InsertError(foBhavEx.StackTrace.ToString().Substring(foBhavEx.StackTrace.ToString().Length - 10));
                }
                downloadStatus = -1;
            }

        }

        public void cm_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs d)
        {
            try
            {
                string filepath = @"c:\Prime\cm.zip";
                File.WriteAllBytes(filepath, d.Result);
                ExtractZip();
                downloadStatus = 1;
            }
            catch (Exception cmEx)
            {
                if (cmEx.Message.ToString().Contains("An exception occurred during the operation, making the result invalid"))
                {
                    XtraMessageBox.Show("Bhavcopy for Equity not available, please select another date");
                }
                else if (!EngineProcess.exceptionsList.Contains(cmEx.Message))
                {
                    InsertError(cmEx.StackTrace.ToString().Substring(cmEx.StackTrace.ToString().Length - 10));
                }
                downloadStatus = -1;
            }

        }

        void span_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                string filepath = @"c:\Prime\spanFO.zip";
                File.WriteAllBytes(filepath, e.Result);
                ExtractZip();
                downloadStatus = 1;
            }
            catch (Exception er)
            {
                if (er.Message.ToString().Contains("An exception occurred during the operation, making the result invalid"))
                {
                    XtraMessageBox.Show("FO Span file not available, please select another type or date");
                    btn_DownloadFiles.Enabled = true;
                    dateTimePicker.Enabled = true;
                }
                else if (!EngineProcess.exceptionsList.Contains(er.Message))
                {
                    InsertError(er.StackTrace.ToString().Substring(er.StackTrace.ToString().Length - 10));
                }
                downloadStatus = -1;
            }
        }

        private void Download_Load(object sender, EventArgs e)
        {
            try
            {
                dateTimePicker.Value = DateTime.Now.Date;
            }
            catch (Exception er)
            {
                if (!EngineProcess.exceptionsList.Contains(er.Message))
                {
                    InsertError(er.StackTrace.ToString().Substring(er.StackTrace.ToString().Length - 10));
                }
            }
        }

        void spanCD_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                string filepath = @"c:\Prime\spanCD.zip";
                File.WriteAllBytes(filepath, e.Result);
                ExtractZip();
                downloadStatus = 1;
            }
            catch (Exception er)
            {
                if (er.Message.ToString().Contains("An exception occurred during the operation, making the result invalid"))
                {
                    XtraMessageBox.Show("Currency Span file not available, please select another type or date");
                    btn_DownloadFiles.Enabled = true;
                    dateTimePicker.Enabled = true;
                }
                else if (!EngineProcess.exceptionsList.Contains(er.Message))
                {
                    InsertError(er.StackTrace.ToString().Substring(er.StackTrace.ToString().Length - 10));
                }
                downloadStatus = -1;
            }
        }
    }
}