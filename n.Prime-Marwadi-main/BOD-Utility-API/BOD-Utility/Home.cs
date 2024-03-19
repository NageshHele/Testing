using DevExpress.XtraEditors;
using NerveLog;
using System;
using System.Data;
using System.Net;
using System.Windows.Forms;
using NerveUtility;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Ionic.Zip;
using MySql.Data.MySqlClient;
using NSEUtilitaire;
using static NSEUtilitaire.Exchange;
using n.Structs;
using System.Xml;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Net.Mail;
using System.Xml.Linq;
using Newtonsoft.Json;
using en_ScripType = NSEUtilitaire.en_ScripType;
using en_Segment = n.Structs.en_Segment;

namespace BOD_Utility
{
    public partial class Home : XtraForm
    {
        string ApplicationPath = Application.StartupPath + "\\";
        NerveLogger _logger = new NerveLogger(true, true, ApplicationName: "BOD-Utility");

        DataSet ds_Config = new DataSet();

        int SpanIndex = 0;
        int VaRIndex = 0;
        int BSESpanIndex = 0;
        int CDSpanIndex = 0;    //Addded by Akshay

        string[] arr_SpanFileExtensions;
        string[] arr_VaRFileExtensions;
        string[] arr_BSESpanFileExtensions;
        string[] arr_CDSpanFileExtensions;  //Added by Akshay

        Dictionary<string, FTPCRED> dict_FTPCred = new Dictionary<string, FTPCRED>();

        HashSet<string> hs_Usernames = new HashSet<string>();

        List<EODPositionInfo> list_Day1Positions = new List<EODPositionInfo>();

        /// <summary>
        /// Key : Segment|ScripName | Value : ScripInfo
        /// </summary>
        ConcurrentDictionary<string, ContractMaster> dict_ScripInfo = new ConcurrentDictionary<string, ContractMaster>();

        /// <summary>
        /// Key : Segment|CustomScripName | Value : ScripInfo
        /// </summary>
        ConcurrentDictionary<string, ContractMaster> dict_CustomScripInfo = new ConcurrentDictionary<string, ContractMaster>();

        /// <summary>
        /// Key : Segment|Token | Value : ScripInfo
        /// </summary>
        ConcurrentDictionary<string, ContractMaster> dict_TokenScripInfo = new ConcurrentDictionary<string, ContractMaster>();

        /// <summary>
        /// MySQL connection string.
        /// </summary>
        string _MySQLCon = string.Empty;

        /// <summary>
        /// Contents of Config.xml
        /// </summary>
        DataSet ds_SQLConfig = new DataSet();
        
        GatewayEngineConnector GatewayEngineConnector = new GatewayEngineConnector();

        bool IsWorking = true;

        List<string> list_ComponentStarted = new List<string>();

        Dictionary<string, string> dict_MappedClient = new Dictionary<string, string>();

        bool IsSpanFileDownloading = false;
        string SpanPath = string.Empty;
        bool SpanFileDownloaded = false;

        res_General res_APIResponse;

        string MemberCode = string.Empty;
        string APILoginID = string.Empty;
        string APIPassword = string.Empty;
        string SecretKey = string.Empty;

        int SpanWaitSeconds = 1000;
        int SpanWaitSecondsWebSite = 1000;
        int SpanWaitSecondsApi = 1000;
        public Home()
        {
            try
            {
                InitializeComponent();

                btn_Settings.Enabled = true;
                btn_StartAuto.Enabled = true;
                btn_RestartAuto.Enabled = true;

                _logger.Initialize(ApplicationPath);

                ds_Config = NerveUtils.XMLC(ApplicationPath + "config.xml");

                //added on 16MAR2021 by Amey
                var dRow = ds_Config.Tables["LOGIN"].Rows[0];
                var GuestCred = dRow["GUEST"].STR().SPL(',');
                dict_FTPCred.Add("GUEST", new FTPCRED() { Username = GuestCred[0], Password = GuestCred[1] });
                GuestCred = dRow["FO"].STR().SPL(',');
                dict_FTPCred.Add("FO", new FTPCRED() { Username = GuestCred[0], Password = GuestCred[1] });
                //Added by Akshay
                GuestCred = dRow["CD"].STR().SPL(',');
                dict_FTPCred.Add("CD", new FTPCRED() { Username = GuestCred[0], Password = GuestCred[1] });

                arr_SpanFileExtensions = ds_Config.GET("SAVEPATH", "SPAN-EXTENSTIONS").SPL(',');

                //Added by Akshay 
                arr_CDSpanFileExtensions = ds_Config.GET("SAVEPATH", "SPAN-EXTENSTIONS").SPL(',');

                //added on 05JAN2021 by Amey
                arr_VaRFileExtensions = ds_Config.GET("SAVEPATH", "VAR-EXTENSTIONS").SPL(',');

                //added on 05JAN2021 by Amey
                arr_BSESpanFileExtensions = ds_Config.GET("SAVEPATH", "BSE-SPAN-EXTENSTIONS").SPL(',');

                SpanWaitSecondsWebSite = Convert.ToInt32(ds_Config.GET("INTERVAL", "SPAN-WAIT-SECONDS-WEBSITE")) * 1000;
                SpanWaitSecondsApi = Convert.ToInt32(ds_Config.GET("INTERVAL", "SPAN-WAIT-SECONDS-API")) * 1000;
                SpanWaitSeconds = Convert.ToInt32(ds_Config.GET("INTERVAL", "SPAN-WAIT-SECONDS")) * 1000;

                _logger.Debug("Span wait seconds init: SpanWaitSecondsWebSite | " + SpanWaitSecondsWebSite + " , SpanWaitSecondsApi | " + SpanWaitSecondsApi);

                dateEdit_DownloadDate.DateTime = DateTime.Now;

                #region Added on 5-11-19 by Amey
                /// <summary>
                /// To Avoid => Exception : "The request was aborted: Could not create SSL/TLS secure channel"
                /// </summary>
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                #endregion

                ReadConfig();
                CheckMaxAllowedSqlPacket();
            }
            catch (Exception ee) { XtraMessageBox.Show("Error while initialising.", "Error"); _logger.Error(ee); }
        }

        HashSet<string> hs_ErrorIndex = new HashSet<string>();

        private void AddToList(string Message, bool IsError = false)
        {
            try
            {
                Message = $"{DateTime.Now} : {Message}";
                if (IsError)
                    hs_ErrorIndex.Add(Message);

                if (this.InvokeRequired)
                    this.Invoke((MethodInvoker)(() => { listBox_Messages.Items.Insert(0, Message); }));
                else
                    listBox_Messages.Items.Insert(0, Message);
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        async private void btn_StartMnually_Click(object sender, EventArgs e)
        {
            try
            {
                btn_StartMnually.Enabled = false;
                btn_Settings.Enabled = false;
                btn_StartAuto.Enabled = false;
                btn_RestartAuto.Enabled = false;
                btn_DownloadSpan.Enabled = false;
               
                await Task.Run(() =>
                {

                    //CHANGES ON 23DEC2022 BY NIKHIL | API DOWNLOAD
                    AddToList("Connecting to NSE API");
                    Config _conf;
                    nNSEUtils.Instance.Initialize(ds_Config.GET("LOGIN", "MEMBER-CODE"), ds_Config.GET("LOGIN", "API-LOGINID"), ds_Config.GET("LOGIN", "API-PASSWORD"), ds_Config.GET("LOGIN", "SECRET-KEY"), Application.StartupPath + "\\config.json",out _conf);
                    res_APIResponse = nNSEUtils.Instance.LoginAPI(out res_LoginAPI _Response);
                    _logger.Debug("LOGIN REPONSE | STATUS : " + res_APIResponse.ResponseStatus + " | MESSAGE : " + res_APIResponse.Message);
                    AddToList("API Connection Status | " + res_APIResponse.ResponseStatus);

                    //Old Span files not needed and takes space in HDD. 22MAR2021 by Amey
                    DeleteOldSpanDirectories();

                    DownloadContractFile();
                    Thread.Sleep(1000);

                    DownloadSecurityFile();
                    Thread.Sleep(1000);

                    //Added by Akshay on 12 - 10 - 2021 for downloading CD contract

                    DownloadCDContractFile();
                    Thread.Sleep(1000);

                    DownloadFOBhavcopy(ds_Config.GET("URLs", "FO_BHAVCOPY").SPL(','), ds_Config.GET("SAVEPATH", "FO_BHAVCOPY").SPL(','));
                    Thread.Sleep(1000);
                    DownloadCMBhavcopy(ds_Config.GET("URLs", "CM_BHAVCOPY").SPL(','), ds_Config.GET("SAVEPATH", "CM_BHAVCOPY").SPL(','));
                    Thread.Sleep(1000);

                    ////Added by Akshay on 12-10-2021 for downloading CD Bhavcopy
                    DownloadCDBhavcopy(ds_Config.GET("URLs", "CD_BHAVCOPY").SPL(','), ds_Config.GET("SAVEPATH", "CD_BHAVCOPY").SPL(','));
                    Thread.Sleep(1000);

                    DownloadNNFSecurityFile();
                    Thread.Sleep(1000);
                    DownloadMFundHaircutFile();
                    Thread.Sleep(1000);
                    DownloadHaricutFile();
                    Thread.Sleep(1000);


                    DownloadFOSecBanFile(ds_Config.GET("URLs", "FO_SECBAN").SPL(','), ds_Config.GET("SAVEPATH", "FO_SECBAN"));
                    Thread.Sleep(1000);
                    DownloadSnapShot(ds_Config.GET("URLs", "DAILY_SNAPSHOT").SPL(','), ds_Config.GET("SAVEPATH", "DAILY_SNAPSHOT").SPL(','));
                    Thread.Sleep(1000);
                    //added on 30APR2021 by Amey
                    DownloadBSEScripFile();
                    Thread.Sleep(1000);
                    ////added on 30APR2021 by Amey
                    DownloadBSECMBhavcopy(ds_Config.GET("URLs", "BSECM_BHAVCOPY").Split(','), ds_Config.GET("SAVEPATH", "BSECM_BHAVCOPY").SPL(','));
                    Thread.Sleep(1000);

                    var arr_SpanInfo = ds_Config.GET("SAVEPATH", "SPAN").SPL(',');
                    var arr_ExposureInfo = ds_Config.GET("SAVEPATH", "EXPOSURE").SPL(',');

                    for (int i = 0; i < arr_SpanInfo.Length; i++)
                    {
                        arr_SpanInfo[i] = arr_SpanInfo[i] + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        arr_ExposureInfo[i] = arr_ExposureInfo[i] + DateTime.Now.ToString("yyyyMMdd") + "\\";

                        if (!Directory.Exists(arr_SpanInfo[i]))
                            Directory.CreateDirectory(arr_SpanInfo[i]);

                        if (!Directory.Exists(arr_ExposureInfo[i]))
                            Directory.CreateDirectory(arr_ExposureInfo[i]);
                    }

                    //added on 05JAN2021 by Amey
                    var VaRExposurePath = ds_Config.GET("SAVEPATH", "VAREXPOSURE");
                    if (!Directory.Exists(VaRExposurePath))
                        Directory.CreateDirectory(VaRExposurePath);

                    InvokeDownloader(arr_SpanInfo, arr_ExposureInfo, VaRExposurePath);

                });

                //btn_Download.Enabled = true;
            }
            catch (Exception ee) { _logger.Error(ee); }

        }

        async private void btn_StartAuto_Click(object sender, EventArgs e)
        {
            try
            {
                btn_StartMnually.Enabled = false;
                btn_Settings.Enabled = false;
                btn_StartAuto.Enabled = false;
                btn_RestartAuto.Enabled = false;

                await Task.Run(() =>
                {
                    AddToList("Connecting to NSE API");
                    Config _Conf;
                    nNSEUtils.Instance.Initialize(ds_Config.GET("LOGIN", "MEMBER-CODE"), ds_Config.GET("LOGIN", "API-LOGINID"), ds_Config.GET("LOGIN", "API-PASSWORD"), ds_Config.GET("LOGIN", "SECRET-KEY"), Application.StartupPath + "\\config.json",out _Conf);
                    res_APIResponse = nNSEUtils.Instance.LoginAPI(out res_LoginAPI _Response);
                    _logger.Debug("LOGIN REPONSE | STATUS : " + res_APIResponse.ResponseStatus + " | MESSAGE : " + res_APIResponse.Message);
                    AddToList("API Connection Status | " + res_APIResponse.ResponseStatus);

                    //Old Span files not needed and takes space in HDD. 22MAR2021 by Amey
                    DeleteOldSpanDirectories();

                    DownloadContractFile();
                    Thread.Sleep(1000);
                    DownloadSecurityFile();
                    Thread.Sleep(1000);

                    //Added by Akshay on 12-10-2021 for downloading CD contract
                    DownloadCDContractFile();
                    Thread.Sleep(1000);

                    DownloadFOBhavcopy(ds_Config.GET("URLs", "FO_BHAVCOPY").SPL(','), ds_Config.GET("SAVEPATH", "FO_BHAVCOPY").SPL(','));
                    Thread.Sleep(1000);
                    DownloadCMBhavcopy(ds_Config.GET("URLs", "CM_BHAVCOPY").SPL(','), ds_Config.GET("SAVEPATH", "CM_BHAVCOPY").SPL(','));
                    Thread.Sleep(1000);

                    DownloadNNFSecurityFile();
                    Thread.Sleep(1000);
                    DownloadMFundHaircutFile();
                    Thread.Sleep(1000);
                    DownloadHaricutFile();
                    Thread.Sleep(1000);

                    //Added by Akshay on 12-10-2021 for downloading CD Bhavcopy
                    DownloadCDBhavcopy(ds_Config.GET("URLs", "CD_BHAVCOPY").SPL(','), ds_Config.GET("SAVEPATH", "CD_BHAVCOPY").SPL(','));
                    Thread.Sleep(1000);

                    DownloadFOSecBanFile(ds_Config.GET("URLs", "FO_SECBAN").SPL(','), ds_Config.GET("SAVEPATH", "FO_SECBAN"));
                    Thread.Sleep(1000);
                    DownloadSnapShot(ds_Config.GET("URLs", "DAILY_SNAPSHOT").SPL(','), ds_Config.GET("SAVEPATH", "DAILY_SNAPSHOT").SPL(','));
                    Thread.Sleep(1000);
                    //added on 30APR2021 by Amey
                    DownloadBSEScripFile();
                    Thread.Sleep(1000);
                    //added on 30APR2021 by Amey
                    DownloadBSECMBhavcopy(ds_Config.GET("URLs", "BSECM_BHAVCOPY").Split(','), ds_Config.GET("SAVEPATH", "BSECM_BHAVCOPY").SPL(','));
                    Thread.Sleep(1000);


                    var arr_SpanInfo = ds_Config.GET("SAVEPATH", "SPAN").SPL(',');
                    var arr_ExposureInfo = ds_Config.GET("SAVEPATH", "EXPOSURE").SPL(',');

                    for (int i = 0; i < arr_SpanInfo.Length; i++)
                    {
                        arr_SpanInfo[i] = arr_SpanInfo[i] + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        arr_ExposureInfo[i] = arr_ExposureInfo[i] + DateTime.Now.ToString("yyyyMMdd") + "\\";

                        if (!Directory.Exists(arr_SpanInfo[i]))
                            Directory.CreateDirectory(arr_SpanInfo[i]);

                        if (!Directory.Exists(arr_ExposureInfo[i]))
                            Directory.CreateDirectory(arr_ExposureInfo[i]);
                    }

                    //added on 05JAN2021 by Amey
                    var VaRExposurePath = ds_Config.GET("SAVEPATH", "VAREXPOSURE");
                    if (!Directory.Exists(VaRExposurePath))
                        Directory.CreateDirectory(VaRExposurePath);

                    InvokeDownloader(arr_SpanInfo, arr_ExposureInfo, VaRExposurePath);


                });

                StartComponents();                      // Added by Snehadri on 15JUN2021 for Automatic BOD Process

            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void DeleteOldSpanDirectories()
        {
            try
            {
                var arr_SpanInfo = ds_Config.GET("SAVEPATH", "SPAN").SPL(',');

                foreach (var _SpnaDir in arr_SpanInfo)
                {
                    string[] subdirs = Directory.GetDirectories(_SpnaDir);

                    foreach (var item in subdirs)
                    {
                        try
                        {
                            var FolderDate = DateTime.ParseExact(item.SUB(item.LastIndexOf('\\') + 1), "yyyyMMdd", CultureInfo.InvariantCulture);
                            if (FolderDate.Date < DateTime.Now.AddDays(-5))
                                Directory.Delete(item, true);
                        }
                        catch (Exception ee) { _logger.Error(ee, "DeleteOldSpanDirectories Loop : " + item); }
                    }
                }


                var arr_SpanLogPath = ds_Config.GET("SAVEPATH", "SPAN-LOGS").SPL(',');

                foreach (var _SpanLogPath in arr_SpanLogPath)
                {
                    foreach (var item in Directory.GetFiles(_SpanLogPath))
                    {
                        try
                        {
                            var _ExtractedDate = item.SUB(item.LastIndexOf('\\') + 1).SPL('_')[1].SPL('.')[0];
                            var FolderDate = DateTime.ParseExact(_ExtractedDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                            if (FolderDate.Date < DateTime.Now.AddDays(-5))
                                File.Delete(item);
                        }
                        catch (Exception ee) { _logger.Error(ee, "DeleteOldSpanDirectories -CONTRACT Loop : " + item); }
                    }
                }

                var arr_VaRExposurepath = ds_Config.GET("SAVEPATH", "VAREXPOSURE");

                foreach (var item in Directory.GetFiles(arr_VaRExposurepath))
                {
                    try
                    {
                        var _ExtractedDate = item.SUB(item.LastIndexOf('\\') + 1).SPL('_')[2];
                       var FolderDate = DateTime.ParseExact(_ExtractedDate, "ddMMyyyy", CultureInfo.InvariantCulture);
                        if (FolderDate.Date < DateTime.Now.AddDays(-5))
                            File.Delete(item);
                    }
                    catch (Exception ee) { _logger.Error(ee, "DeleteOldSpanDirectories -VaRExosure Loop : " + item); }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void DownloadContractFile()
        {
            try
            {
                string[] arr_ContractFolderPath = ds_Config.GET("SAVEPATH", "CONTRACT").SPL(',');
                string[] arr_ContractURL = ds_Config.GET("URLs", "CONTRACT").SPL(',');
                bool filedownloaded = false;

                if (arr_ContractFolderPath.Length != 0)
                {
                    AddToList("Contract file downloading.");

                    for (int i = 0; i < arr_ContractFolderPath.Length; i++)
                    {
                        if (!Directory.Exists(arr_ContractFolderPath[i]))
                            Directory.CreateDirectory(arr_ContractFolderPath[i]);
                        else
                        {
                            string[] files = Directory.GetFiles(arr_ContractFolderPath[i], @"*.gz");
                            for (int j = 0; j < files.Count(); j++)
                                File.Delete(files[j]);
                        }
                    }

                    try
                    {
                        //downloading with NSE-API
                        var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.FO_CONTRACT, "contract.gz", arr_ContractFolderPath[0]);
                        _logger.Debug("DownloadContractFile API Response: " + JsonConvert.SerializeObject(response));
                        if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                        {
                            File.Delete(arr_ContractFolderPath[0] + @"contract.txt");
                            DecompressGZAndDelete(new FileInfo(arr_ContractFolderPath[0] + @"contract.gz"), ".txt");
                            filedownloaded = true;
                        }
                    }
                    catch (Exception ee)
                    {
                        _logger.Error(ee, "DownloadExchangeFiles API- Contract ");
                    }


                    if (!filedownloaded)
                    {
                        try
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                //Added to login and download from NSE FTP link. 16MAR2021-Amey
                                webClient.Credentials = new NetworkCredential(dict_FTPCred["FO"].Username, dict_FTPCred["FO"].Password);

                                webClient.DownloadFile(arr_ContractURL[0], arr_ContractFolderPath[0] + @"contract.gz");
                            }

                            File.Delete(arr_ContractFolderPath[0] + @"contract.txt");

                            DecompressGZAndDelete(new FileInfo(arr_ContractFolderPath[0] + @"contract.gz"), ".txt");

                            filedownloaded = true;
                        }
                        catch (Exception ee) { _logger.Error(ee, "DownloadExchangeFiles - Contract"); }
                    }

                    try
                    {
                        if (!filedownloaded)
                        {

                            File.Copy(arr_ContractURL[1] + @"contract.txt", arr_ContractFolderPath[0] + @"contract.txt", true);
                            filedownloaded = true;
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee); }

                    if (!filedownloaded)
                    {
                        AddToList("Contract file download failed.", true);
                    }

                    if (arr_ContractFolderPath.Length > 1 && filedownloaded)
                    {
                        for (int i = 1; i < arr_ContractFolderPath.Length; i++)
                            File.Copy(arr_ContractFolderPath[0] + @"contract.txt", arr_ContractFolderPath[i] + @"contract.txt", true);
                        AddToList("Contract file downloaded successfully.");
                    }
                }
                else
                    AddToList("Invalid path specified for Contract file.", true);
            }
            catch (Exception ee) { _logger.Error(ee); AddToList("Unable to download Contract file.", true); }
        }

        private void DownloadSecurityFile()
        {
            try
            {
                string[] arr_SecurityFolderPath = ds_Config.GET("SAVEPATH", "SECURITY").SPL(',');
                string[] arr_SecurityUrl = ds_Config.GET("URLs", "SECURITY").SPL(',');
                bool filedownloaded = false;

                if (arr_SecurityFolderPath.Length != 0)
                {
                    AddToList("Security file downloading.");

                    for (int i = 0; i < arr_SecurityFolderPath.Length; i++)
                    {
                        if (!Directory.Exists(arr_SecurityFolderPath[i]))
                            Directory.CreateDirectory(arr_SecurityFolderPath[i]);
                        else
                        {
                            string[] files = Directory.GetFiles(arr_SecurityFolderPath[i], @"*.gz");
                            for (int j = 0; j < files.Count(); j++)
                                File.Delete(files[j]);
                        }
                    }

                    try
                    {
                        var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.CM_SECURITY, "security.gz", arr_SecurityFolderPath[0]);
                        _logger.Debug("DownloadSecurityFile API Response: " + JsonConvert.SerializeObject(response));
                        if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                        {
                            DecompressGZAndDelete(new FileInfo(arr_SecurityFolderPath[0] + @"security.gz"), ".txt");
                            filedownloaded = true;
                        }
                    }
                    catch (Exception ee)
                    {
                        _logger.Error(ee);
                    }

                    if (!filedownloaded)
                    {

                        try
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                //Added to login and download from NSE FTP link. 16MAR2021-Amey
                                webClient.Credentials = new NetworkCredential(dict_FTPCred["FO"].Username, dict_FTPCred["FO"].Password);

                                webClient.DownloadFile(arr_SecurityUrl[0], arr_SecurityFolderPath[0] + @"security.gz");
                            }

                            File.Delete(arr_SecurityFolderPath[0] + @"security.txt");

                            DecompressGZAndDelete(new FileInfo(arr_SecurityFolderPath[0] + @"security.gz"), ".txt");
                            filedownloaded = true;
                        }
                        catch (Exception ee) { _logger.Error(ee, "DownloadExchangeFiles - Security"); }
                    }

                    try
                    {
                        if (!filedownloaded)
                        {
                            File.Copy(arr_SecurityUrl[1] + @"security.txt", arr_SecurityFolderPath[0] + @"security.txt", true);
                            filedownloaded = true;
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee); }

                    if (!filedownloaded)
                    {
                        AddToList("Security file download failed.", true);
                        return;
                    }

                    if (arr_SecurityFolderPath.Length > 1 && filedownloaded)
                    {
                        AddToList("Security file downloaded successfully.");

                        for (int i = 1; i < arr_SecurityFolderPath.Length; i++)
                            File.Copy(arr_SecurityFolderPath[0] + @"security.txt", arr_SecurityFolderPath[i] + @"security.txt", true);
                    }
                }
                else
                    AddToList("Invalid path specified for Security file.", true);
            }
            catch (Exception ee) { _logger.Error(ee); AddToList("Unable to download Security file.", true); }
        }


        private void DownloadNNFSecurityFile()
        {
            try
            {
                string[] arr_NNFSecurityFolderPath = ds_Config.GET("SAVEPATH", "NNF-SECURITY").SPL(',');
                string[] arr_NNFSecurityURL = ds_Config.GET("URLs", "NNF-SECURITY").SPL(',');
                bool filedownloaded = false;

                if (arr_NNFSecurityFolderPath.Length != 0)
                {
                    AddToList("NNF Security file downloading.");

                    for (int i = 0; i < arr_NNFSecurityFolderPath.Length; i++)
                    {
                        if (!Directory.Exists(arr_NNFSecurityFolderPath[i]))
                            Directory.CreateDirectory(arr_NNFSecurityFolderPath[i]);
                        else
                        {
                            string[] files = Directory.GetFiles(arr_NNFSecurityFolderPath[i], @".gz");
                            for (int j = 0; j < files.Count(); j++)
                                File.Delete(files[j]);
                        }
                    }

                    try
                    {
                        var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.CM_NNF_SECUIRTY, "nnf_security.gz", arr_NNFSecurityFolderPath[0]);
                        _logger.Debug("DownloadSecurityFile API Response: " + JsonConvert.SerializeObject(response));
                        if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                        {
                            DecompressGZAndDelete(new FileInfo(arr_NNFSecurityFolderPath[0] + @"nnf_security.gz"), ".txt");
                            filedownloaded = true;
                        }
                    }
                    catch (Exception ee)
                    {
                        _logger.Error(ee);
                    }

                    if (!filedownloaded)
                    {
                        try
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                //Added to login and download from NSE FTP link. 16MAR2021-Amey
                                webClient.Credentials = new NetworkCredential(dict_FTPCred["FO"].Username, dict_FTPCred["FO"].Password);
                                webClient.DownloadFile(arr_NNFSecurityURL[0], arr_NNFSecurityFolderPath[0] + @"nnf_security.gz");
                            }

                            File.Delete(arr_NNFSecurityFolderPath[0] + @"nnf_security.dat");

                            DecompressGZAndDelete(new FileInfo(arr_NNFSecurityFolderPath[0] + @"nnf_security.gz"), ".txt");

                            filedownloaded = true;
                        }
                        catch (Exception ee)
                        {
                            _logger.Error(ee, "DownloadExchangeFiles - NNF-SECURITY");
                        }
                    }


                    if (!filedownloaded)
                    {
                        AddToList("NNF Security file download failed.", true);
                        return;
                    }

                    if (arr_NNFSecurityFolderPath.Length > 1 && filedownloaded)
                    {
                        // AddToList("NNF Security file downloaded successfully.");
                        for (int i = 1; i < arr_NNFSecurityFolderPath.Length; i++)
                            File.Copy(arr_NNFSecurityFolderPath[0] + @"nnf_security.txt", arr_NNFSecurityFolderPath[i] + @"nnf_security.txt", true);
                    }
                    if (filedownloaded) { AddToList("NNF Security file downloaded successfully."); }
                }
                else
                    AddToList("Invalid path specified for NNF Security file.", true);
            }
            catch (Exception ee)
            {
                _logger.Error(ee); AddToList("Unable to download NNF Security file.", true);
            }
        }

        private void DownloadMFundHaircutFile()
        {
            try
            {

                string[] arr_MFundHaircutFolderPath = ds_Config.GET("SAVEPATH", "MF-HAIRCUT").SPL(',');
                string[] arr_MFundHaircutURL = ds_Config.GET("URLs", "MF-HAIRCUT").SPL(',');
                string FileName = $"MF_VAR_{dateEdit_DownloadDate.DateTime.STR("ddMMyyyy").UPP()}.csv";

                string URL = arr_MFundHaircutURL[0] + FileName;
                bool filedownloaded = false;

                if (arr_MFundHaircutFolderPath.Length != 0)
                {
                    AddToList("Mutual fund haircut file downloading.");

                    for (int i = 0; i < arr_MFundHaircutFolderPath.Length; i++)
                    {
                        if (!Directory.Exists(arr_MFundHaircutFolderPath[i]))
                            Directory.CreateDirectory(arr_MFundHaircutFolderPath[i]);
                        else
                        {
                            string[] files = Directory.GetFiles(arr_MFundHaircutFolderPath[i], @"*.gz");
                            for (int j = 0; j < files.Count(); j++)
                                File.Delete(files[j]);
                        }
                    }

                    //try
                    //{
                    //    var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.CM_APPSEC_COLLVAL,FileName, arr_MFundHaircutFolderPath[0]);
                    //    _logger.Debug("DownloadSecurityFile API Response: " + JsonConvert.SerializeObject(response));
                    //    if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                    //    {
                    //        //DecompressGZAndDelete(new FileInfo(arr_MFundHaircutFolderPath[0] + @"security.gz"), ".txt");
                    //        filedownloaded = true;
                    //    }
                    //}
                    //catch (Exception ee)
                    //{
                    //    _logger.Error(ee);
                    //}

                    //trying with website
                    try
                    {
                        if (!filedownloaded)
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadFile(URL, arr_MFundHaircutFolderPath[0] + FileName);
                                filedownloaded = true;
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        _logger.Error(ee);
                    }

                    if (!filedownloaded)
                    {
                        AddToList("Mutual Fund Haircut file download failed.", true);
                        return;
                    }

                    if (arr_MFundHaircutFolderPath.Length > 1 && filedownloaded)
                    {
                        // AddToList("NNF Security file downloaded successfully.");
                        for (int i = 1; i < arr_MFundHaircutFolderPath.Length; i++)
                            File.Copy(arr_MFundHaircutFolderPath[0] + FileName, arr_MFundHaircutFolderPath[i] + FileName, true);
                    }
                    if (filedownloaded) { AddToList("Mutual Fund Haircut file downloaded successfully."); }
                }
                else
                    AddToList("Invalid path specified for Mutual Fund Haircut file.", true);
            }
            catch (Exception ee)
            {
                _logger.Error(ee); AddToList("Unable to download Mutual Fund Haircut file.", true);
            }
        }
        
        //added by nikhil | gross F/O
        private void DownloadHaricutFile()
        {
            try
            {
                string[] arr_HaircutFolderPath = ds_Config.GET("SAVEPATH", "COLLATERAL-HAIRCUT").SPL(',');
                string[] arr_HaircutURL = ds_Config.GET("URLs", "COLLATERAL-HAIRCUT").SPL(',');

                string FileName = $"APPSEC_COLLVAL_{dateEdit_DownloadDate.DateTime.STR("ddMMyyyy").UPP()}.csv";

                string URL = arr_HaircutURL[1] + FileName;

                bool fileDownloaded = false;

                if (arr_HaircutFolderPath.Length != 0)
                {
                    AddToList("Collateral Haircut file downloading.");

                    for (int i = 0; i < arr_HaircutFolderPath.Length; i++)
                    {
                        if (!Directory.Exists(arr_HaircutFolderPath[i]))
                            Directory.CreateDirectory(arr_HaircutFolderPath[i]);
                        else
                        {
                            string[] files = Directory.GetFiles(arr_HaircutFolderPath[i], @"APPSEC_COLLVAL_*.csv");
                            for (int j = 0; j < files.Count(); j++)
                                File.Delete(files[j]);
                        }
                    }

                    try
                    {
                        var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.CM_APPSEC_COLLVAL, FileName, arr_HaircutFolderPath[0]);
                        _logger.Debug("DownloadSecurityFile API Response: " + JsonConvert.SerializeObject(response));
                        if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                        {
                            //DecompressGZAndDelete(new FileInfo(arr_MFundHaircutFolderPath[0] + @"security.gz"), ".txt");
                            fileDownloaded = true;
                        }
                    }
                    catch (Exception ee)
                    {
                        _logger.Error(ee);
                    }

                  
                   
                    //trying with website
                    try
                    {
                        if (!fileDownloaded)
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadFile(URL, arr_HaircutFolderPath[0] + FileName);
                                fileDownloaded = true;
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        _logger.Error(ee);
                    }

                    if (!fileDownloaded)
                    {
                        //trying with ftp
                        try
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                //Added to login and download from NSE FTP link. 16MAR2021-Amey
                                webClient.Credentials = new NetworkCredential(dict_FTPCred["FO"].Username, dict_FTPCred["FO"].Password);

                                webClient.DownloadFile(arr_HaircutURL[0] + FileName, arr_HaircutFolderPath[0] + FileName);

                                fileDownloaded = true;
                            }
                        }
                        catch (Exception ee)
                        {
                            _logger.Error(ee);
                        }
                    }

                    if (arr_HaircutFolderPath.Length > 1 && fileDownloaded)
                    {
                        for (int i = 1; i < arr_HaircutFolderPath.Length; i++)
                            File.Copy(arr_HaircutFolderPath[0] + FileName, arr_HaircutFolderPath[i] + FileName, true);
                    }

                    if (fileDownloaded)
                    {
                        AddToList("Collateral Haircut file downloaded successfully.");
                    }
                    else
                    {
                        AddToList("Collateral Haircut file download failed.", true);
                    }
                }
                else
                {
                    AddToList("Invalid path specified for Collateral Haircut file.", true);
                }
            }
            catch (Exception ee)
            {
                _logger.Error(ee);
                AddToList("Unable to download Collateral Haircut file.", true);
            }
        }



        //Added by Akshay on 12-10-2021 for Downloading CD Bhavcopy
        private void DownloadCDContractFile()
        {
            try
            {
                string[] arr_ContractFolderPath = ds_Config.GET("SAVEPATH", "CD_CONTRACT").SPL(',');
                string[] arr_ContractURL = ds_Config.GET("URLs", "CD_CONTRACT").SPL(',');
                bool filedownloaded = false;

                if (arr_ContractFolderPath.Length != 0)
                {
                    AddToList("cd_Contract file downloading.");

                    for (int i = 0; i < arr_ContractFolderPath.Length; i++)
                    {
                        if (!Directory.Exists(arr_ContractFolderPath[i]))
                            Directory.CreateDirectory(arr_ContractFolderPath[i]);
                        else
                        {
                            string[] files = Directory.GetFiles(arr_ContractFolderPath[i], @"*.gz");
                            for (int j = 0; j < files.Count(); j++)
                                File.Delete(files[j]);
                        }
                    }

                    try
                    {
                        var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.CD_CONTRACT, "cd_contract.gz", arr_ContractFolderPath[0]);
                        _logger.Debug("DownloadCDContractFile API Response: " + JsonConvert.SerializeObject(response));
                        if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                        {
                            File.Delete(arr_ContractFolderPath[0] + @"cd_contract.txt");
                            DecompressGZAndDelete(new FileInfo(arr_ContractFolderPath[0] + @"cd_contract.gz"), ".txt");
                            filedownloaded = true;
                        }
                    }
                    catch (Exception ee)
                    {
                        _logger.Error(ee);
                    }

                    if (!filedownloaded)
                    {
                        try
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                //Added to login and download from NSE FTP link. 16MAR2021-Amey
                                webClient.Credentials = new NetworkCredential(dict_FTPCred["CD"].Username, dict_FTPCred["CD"].Password);

                                webClient.DownloadFile(arr_ContractURL[0], arr_ContractFolderPath[0] + @"cd_contract.gz");
                            }

                            File.Delete(arr_ContractFolderPath[0] + @"cd_contract.txt");

                            DecompressGZAndDelete(new FileInfo(arr_ContractFolderPath[0] + @"cd_contract.gz"), ".txt");

                            filedownloaded = true;
                        }
                        catch (Exception ee) { _logger.Error(ee, "DownloadExchangeFiles - cd_Contract"); }

                    }

                    try
                    {
                        if (!filedownloaded)
                        {

                            File.Copy(arr_ContractURL[1] + @"cd_contract.txt", arr_ContractFolderPath[0] + @"cd_contract.txt", true);
                            filedownloaded = true;
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee); }

                    if (!filedownloaded)
                    {
                        AddToList("cd_Contract file download failed.", true);
                        return;
                    }

                    if (arr_ContractFolderPath.Length > 1 && filedownloaded)
                    {
                        AddToList("cd_Contract file downloaded successfully.");
                        for (int i = 1; i < arr_ContractFolderPath.Length; i++)
                            File.Copy(arr_ContractFolderPath[0] + @"cd_contract.txt", arr_ContractFolderPath[i] + @"cd_contract.txt", true);
                    }
                }
                else
                    AddToList("Invalid path specified for cd_Contract file.", true);
            }
            catch (Exception ee) { _logger.Error(ee); AddToList("Unable to download cd_Contract file.", true); }
        }


        //added on 30APR2021 by Amey
        private void DownloadBSEScripFile()
        {
            try
            {
                bool filedownloaded = false;
                string[] arr_SecurityFolderPath = ds_Config.GET("SAVEPATH", "BSECM_SCRIP").SPL(',');
                string[] arr_ScripUrl = ds_Config.GET("URLs", "BSECM_SCRIP").Split(',');

                string[] arr_OldFiles;
                if (arr_SecurityFolderPath.Length != 0)
                {
                    AddToList("BSE ScripFile file downloading.");

                    var TempDirectory = arr_SecurityFolderPath[0] + "SCRIP\\";
                    if (Directory.Exists(TempDirectory))
                        Directory.Delete(TempDirectory, true);
                    Directory.CreateDirectory(TempDirectory);

                    for (int i = 0; i < arr_SecurityFolderPath.Length; i++)
                    {
                        if (!Directory.Exists(arr_SecurityFolderPath[i]))
                            Directory.CreateDirectory(arr_SecurityFolderPath[i]);
                        else
                        {
                            arr_OldFiles = Directory.GetFiles(arr_SecurityFolderPath[i], @"scrip.zip");
                            for (int j = 0; j < arr_OldFiles.Count(); j++)
                                File.Delete(arr_OldFiles[j]);
                        }
                    }

                    arr_OldFiles = Directory.GetFiles(arr_SecurityFolderPath[0], @"SCRIP_*.txt");
                    for (int j = 0; j < arr_OldFiles.Count(); j++)
                        File.Delete(arr_OldFiles[j]);

                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(arr_ScripUrl[0], arr_SecurityFolderPath[0] + @"scrip.zip");
                        }
                        using (ZipFile zip = ZipFile.Read(arr_SecurityFolderPath[0] + @"scrip.zip"))
                            zip.ExtractAll(arr_SecurityFolderPath[0], ExtractExistingFileAction.DoNotOverwrite);

                        File.Delete(arr_SecurityFolderPath[0] + @"scrip.zip");

                        var allFiles = Directory.GetFiles(TempDirectory);
                        foreach (var _File in allFiles)
                        {
                            if (!_File.Contains("SCRIP_"))
                                File.Delete(_File);
                        }

                        var _ScripTXT = Directory.GetFiles(TempDirectory, "SCRIP_*.txt");
                        var SCRIPFILENAME = $"SCRIP_{dateEdit_DownloadDate.DateTime.STR("ddMMyy")}.txt";

                        File.Copy(_ScripTXT[0], arr_SecurityFolderPath[0] + SCRIPFILENAME, true);

                        filedownloaded = true;
                    }
                    catch (Exception ee)
                    {
                        _logger.Error(ee, "DownloadExchangeFiles - BSEScripFile");
                    }


                    try
                    {
                        if (!filedownloaded)
                        {
                            File.Copy(arr_ScripUrl[1] + @"SCRIP.txt", arr_SecurityFolderPath[0] + $"SCRIP_{dateEdit_DownloadDate.DateTime.STR("ddMMyy")}.txt", true);
                            filedownloaded = true;
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee); }

                    if (filedownloaded)
                    {


                        if (arr_SecurityFolderPath.Length > 0)
                        {
                            var SCRIPFILENAME = $"SCRIP_{dateEdit_DownloadDate.DateTime.STR("ddMMyy")}.txt";

                            for (int i = 1; i < arr_SecurityFolderPath.Length; i++)
                            {
                                arr_OldFiles = Directory.GetFiles(arr_SecurityFolderPath[i], "SCRIP_*.txt");
                                for (int j = 0; j < arr_OldFiles.Count(); j++)
                                    File.Delete(arr_OldFiles[j]);

                                File.Copy(arr_SecurityFolderPath[0] + $"SCRIP_{dateEdit_DownloadDate.DateTime.STR("ddMMyy")}.txt", arr_SecurityFolderPath[i] + SCRIPFILENAME, true);
                            }
                        }

                        if (Directory.Exists(TempDirectory))
                            Directory.Delete(TempDirectory, true);

                        AddToList("BSE ScripFile file downloaded successfully.");
                    }
                    else
                    {
                        AddToList("BSE ScripFile file download failed.", true);

                        if (Directory.Exists(TempDirectory))
                            Directory.Delete(TempDirectory, true);
                    }

                }
                else
                    AddToList("Invalid path specified for BSE ScripFile file.", true);
            }
            catch (Exception ee) { _logger.Error(ee); AddToList("Unable to download BSEScripFile file.", true); }
        }

        private void DownloadFOBhavcopy(string[] BhavcopyURL, string[] arr_BhavcopyFolderPath)
        {
            try
            {

                AddToList($"FO Bhavcopy downloading.");

                bool filedownloaded = false;
                string[] arr_OldBhavcopyFiles;

                for (int i = 1; i < arr_BhavcopyFolderPath.Length; i++)
                {
                    if (!Directory.Exists(arr_BhavcopyFolderPath[i]))
                        Directory.CreateDirectory(arr_BhavcopyFolderPath[i]);
                    else
                    {
                        arr_OldBhavcopyFiles = Directory.GetFiles(arr_BhavcopyFolderPath[i], @"fo*.csv");
                        for (int j = 0; j < arr_OldBhavcopyFiles.Count(); j++)
                            File.Delete(arr_OldBhavcopyFiles[j]);
                    }
                }

                var BhavcopyFileName = $"fo{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv.gz";

                arr_OldBhavcopyFiles = Directory.GetFiles(arr_BhavcopyFolderPath[0], @"fo*.csv");
                for (int j = 0; j < arr_OldBhavcopyFiles.Count(); j++)
                    File.Delete(arr_OldBhavcopyFiles[j]);

                var FOBhavcopyFilename = string.Empty;

                try
                {
                    var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.FO_BHAVCOPY, BhavcopyFileName, arr_BhavcopyFolderPath[0]);
                    _logger.Debug("DownloadFOBhavcopy " + BhavcopyFileName + " | API Response : " + JsonConvert.SerializeObject(response));
                    if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                    {
                        FOBhavcopyFilename = DecompressGZAndDelete(new FileInfo(arr_BhavcopyFolderPath[0] + BhavcopyFileName), ".csv");
                        filedownloaded = true;
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error(ee);
                }

                //Downloading from website
                if (!filedownloaded)
                {
                    try
                    {
                        string url = BhavcopyURL[1] + $"{dateEdit_DownloadDate.DateTime.STR("yyyy")}/{dateEdit_DownloadDate.DateTime.STR("MMM").UPP()}/fo{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv.zip";

                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(url, arr_BhavcopyFolderPath[0] + BhavcopyFileName);
                        }
                        using (ZipFile zip = ZipFile.Read(arr_BhavcopyFolderPath[0] + BhavcopyFileName))
                            zip.ExtractAll(arr_BhavcopyFolderPath[0], ExtractExistingFileAction.DoNotOverwrite);

                        File.Delete(arr_BhavcopyFolderPath[0] + BhavcopyFileName);
                        filedownloaded = true;
                    }
                    catch (Exception ee) { _logger.Error(ee); }
                }


                if (!filedownloaded)
                {

                    //Downloading from FTP
                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            //Added to login and download from NSE FTP link. 16MAR2021-Amey
                            webClient.Credentials = new NetworkCredential(dict_FTPCred["FO"].Username, dict_FTPCred["FO"].Password);

                            webClient.DownloadFile(BhavcopyURL[0] + BhavcopyFileName, arr_BhavcopyFolderPath[0] + BhavcopyFileName);

                        }

                        FOBhavcopyFilename = DecompressGZAndDelete(new FileInfo(arr_BhavcopyFolderPath[0] + BhavcopyFileName), ".csv");
                        filedownloaded = true;
                    }
                    catch (Exception ee) { _logger.Error(ee); }



                }


                if (!filedownloaded)
                {
                    try
                    {
                        var FileName = $"fo{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv";
                        File.Copy(BhavcopyURL[2] + FileName, arr_BhavcopyFolderPath[0] + FileName, true);
                        filedownloaded = true;
                    }
                    catch (Exception ee) { _logger.Error(ee); }
                }

                if (!filedownloaded)
                {
                    AddToList("FO Bhavcopy file download failed.", true);

                    return;
                }
                else
                {
                    if (arr_BhavcopyFolderPath.Length > 1)
                    {
                        var arr_BhavcopyCSV = Directory.GetFiles(arr_BhavcopyFolderPath[0], "fo*.csv");
                        BhavcopyFileName = $"fo{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv";

                        for (int i = 1; i < arr_BhavcopyFolderPath.Length; i++)
                        {
                            var oldcsvFiles = Directory.GetFiles(arr_BhavcopyFolderPath[i], "fo*.csv");
                            for (int j = 0; j < oldcsvFiles.Count(); j++)
                                File.Delete(oldcsvFiles[j]);

                            File.Copy(arr_BhavcopyCSV[0], arr_BhavcopyFolderPath[i] + BhavcopyFileName, true);
                        }
                    }

                    AddToList($"FO Bhavcopy downloaded successfully.");
                }


            }
            catch (Exception ee) { _logger.Error(ee, $"Download FO Bhavcopy With {BhavcopyURL}"); AddToList($"Unable to download FO Bhavcopy file.", true); }
        }

        private void DownloadCMBhavcopy(string[] BhavcopyURL, string[] arr_BhavcopyFolderPath)
        {
            try
            {
                AddToList($"CM Bhavcopy downloading.");

                bool filedownloaded = false;

                string[] arr_OldBhavcopyFiles;

                for (int i = 1; i < arr_BhavcopyFolderPath.Length; i++)
                {
                    if (!Directory.Exists(arr_BhavcopyFolderPath[i]))
                        Directory.CreateDirectory(arr_BhavcopyFolderPath[i]);
                    else
                    {
                        arr_OldBhavcopyFiles = Directory.GetFiles(arr_BhavcopyFolderPath[i], @"cm*.csv");
                        for (int j = 0; j < arr_OldBhavcopyFiles.Count(); j++)
                            File.Delete(arr_OldBhavcopyFiles[j]);
                    }
                }

                var BhavcopyFileName = $"{dateEdit_DownloadDate.DateTime.STR("ddMM")}0000.md";

                arr_OldBhavcopyFiles = Directory.GetFiles(arr_BhavcopyFolderPath[0], @"cm*.csv");
                for (int j = 0; j < arr_OldBhavcopyFiles.Count(); j++)
                    File.Delete(arr_OldBhavcopyFiles[j]);

                try
                {
                    var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.CM_BHAVCOPY, BhavcopyFileName, arr_BhavcopyFolderPath[0]);
                    _logger.Debug("DownloadCMBhavcopy " + BhavcopyFileName + " | API Response: " + JsonConvert.SerializeObject(response));
                    if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                    {
                        var arr_Lines = File.ReadAllLines(arr_BhavcopyFolderPath[0] + BhavcopyFileName);
                        File.Delete(arr_BhavcopyFolderPath[0] + BhavcopyFileName);
                        var arr_Lines_Copy = new string[arr_Lines.Length + 1];
                        arr_Lines_Copy[0] = "SYMBOL,SERIES,OPEN,HIGH,LOW,CLOSE,LAST,PREVCLOSE,TOTTRDQTY,TOTTRDVAL,TIMESTAMP,TOTALTRADES,ISIN,";

                        for (int i = 0; i < arr_Lines.Length; i++)
                        {
                            var arr_Fields = arr_Lines[i].SPL(',').Select(v => v.TRM()).ToArray();
                            var newLine = arr_Fields[1] + ",";
                            newLine += arr_Fields[2] + ",";
                            newLine += arr_Fields[4] + ",";
                            newLine += arr_Fields[5] + ",";
                            newLine += arr_Fields[6] + ",";
                            newLine += arr_Fields[7] + ",";
                            newLine += "0" + ",";
                            newLine += arr_Fields[3] + ",";
                            newLine += arr_Fields[9] + ",";
                            newLine += arr_Fields[10] + ",";
                            newLine += dateEdit_DownloadDate.DateTime.Date + ",";
                            newLine += 0 + ",";
                            newLine += "" + ",";

                            arr_Lines_Copy[i + 1] = newLine;
                        }

                        BhavcopyFileName = $"cm{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv";
                        File.WriteAllLines(arr_BhavcopyFolderPath[0] + BhavcopyFileName, arr_Lines_Copy);

                        filedownloaded = true;
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error(ee);
                }

                try
                {
                    if (!filedownloaded)
                    {
                        BhavcopyFileName = string.Empty;

                        string url = BhavcopyURL[1] + $"{dateEdit_DownloadDate.DateTime.STR("yyyy")}/{dateEdit_DownloadDate.DateTime.STR("MMM").UPP()}/cm{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv.zip";

                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(url, arr_BhavcopyFolderPath[0] + $"cm{ dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.zip");
                        }

                        using (ZipFile zip = ZipFile.Read(arr_BhavcopyFolderPath[0] + $"cm{ dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.zip"))
                            zip.ExtractAll(arr_BhavcopyFolderPath[0], ExtractExistingFileAction.DoNotOverwrite);

                        File.Delete(arr_BhavcopyFolderPath[0] + $"cm{ dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.zip");

                        filedownloaded = true;
                    }
                }
                catch (Exception ee) { _logger.Error(ee); }

                if (!filedownloaded)
                {
                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            //Added to login and download from NSE FTP link. 16MAR2021-Amey
                            webClient.Credentials = new NetworkCredential(dict_FTPCred["GUEST"].Username, dict_FTPCred["GUEST"].Password);

                            webClient.DownloadFile(BhavcopyURL[0] + BhavcopyFileName, arr_BhavcopyFolderPath[0] + BhavcopyFileName);

                        }

                        var arr_Lines = File.ReadAllLines(arr_BhavcopyFolderPath[0] + BhavcopyFileName);

                        File.Delete(arr_BhavcopyFolderPath[0] + BhavcopyFileName);

                        var arr_Lines_Copy = new string[arr_Lines.Length + 1];
                        arr_Lines_Copy[0] = "SYMBOL,SERIES,OPEN,HIGH,LOW,CLOSE,LAST,PREVCLOSE,TOTTRDQTY,TOTTRDVAL,TIMESTAMP,TOTALTRADES,ISIN,";

                        for (int i = 0; i < arr_Lines.Length; i++)
                        {
                            var arr_Fields = arr_Lines[i].SPL(',').Select(v => v.TRM()).ToArray();
                            var newLine = arr_Fields[1] + ",";
                            newLine += arr_Fields[2] + ",";
                            newLine += arr_Fields[4] + ",";
                            newLine += arr_Fields[5] + ",";
                            newLine += arr_Fields[6] + ",";
                            newLine += arr_Fields[7] + ",";
                            newLine += "0" + ",";
                            newLine += arr_Fields[3] + ",";
                            newLine += arr_Fields[9] + ",";
                            newLine += arr_Fields[10] + ",";
                            newLine += dateEdit_DownloadDate.DateTime.Date + ",";
                            newLine += 0 + ",";
                            newLine += "" + ",";

                            arr_Lines_Copy[i + 1] = newLine;
                        }

                        BhavcopyFileName = $"cm{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv";
                        File.WriteAllLines(arr_BhavcopyFolderPath[0] + BhavcopyFileName, arr_Lines_Copy);
                        filedownloaded = true;
                    }
                    catch (Exception ee) { _logger.Error(ee); }

                }

                try
                {
                    if (!filedownloaded)
                    {
                        string filename = BhavcopyURL[2] + $"cm{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv";
                        File.Copy(filename, arr_BhavcopyFolderPath[0] + $"cm{ dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv", true);
                        filedownloaded = true;
                    }
                }
                catch (Exception ee) { _logger.Error(ee); }

                if (!filedownloaded)
                {
                    AddToList("CM Bhavcopy file download failed.", true);

                    return;
                }
                else
                {
                    if (arr_BhavcopyFolderPath.Length > 1)
                    {
                        var arr_BhavcopyCSV = Directory.GetFiles(arr_BhavcopyFolderPath[0], "cm*.csv");
                        BhavcopyFileName = $"cm{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv";
                        for (int i = 1; i < arr_BhavcopyFolderPath.Length; i++)
                        {
                            var oldcsvFiles = Directory.GetFiles(arr_BhavcopyFolderPath[i], "cm*.csv");
                            for (int j = 0; j < oldcsvFiles.Count(); j++)
                                File.Delete(oldcsvFiles[j]);

                            File.Copy(arr_BhavcopyCSV[0], arr_BhavcopyFolderPath[i] + BhavcopyFileName, true);
                        }
                    }
                }

                AddToList($"CM Bhavcopy downloaded successfully.");
            }
            catch (Exception ee) { _logger.Error(ee, $"Download CM Bhavcopy With {BhavcopyURL}"); AddToList($"Unable to download CM Bhavcopy file.", true); }
        }

        //Added by Akshay on 12-10-2021 for Downloading CD Bhavcopy
        private void DownloadCDBhavcopy(string[] BhavcopyURL, string[] arr_BhavcopyFolderPath)
        {
            try
            {
                AddToList($"CD Bhavcopy downloading.");

                bool filedownloaded = false;

                string[] arr_OldBhavcopyFiles;

                for (int i = 1; i < arr_BhavcopyFolderPath.Length; i++)
                {
                    if (!Directory.Exists(arr_BhavcopyFolderPath[i]))
                        Directory.CreateDirectory(arr_BhavcopyFolderPath[i]);
                    else
                    {
                        arr_OldBhavcopyFiles = Directory.GetFiles(arr_BhavcopyFolderPath[i], @"cd*.csv");
                        for (int j = 0; j < arr_OldBhavcopyFiles.Count(); j++)
                            File.Delete(arr_OldBhavcopyFiles[j]);
                    }
                }

                var BhavcopyFileName = $"FINAL_{dateEdit_DownloadDate.DateTime.STR("ddMM")}0000.md";

                arr_OldBhavcopyFiles = Directory.GetFiles(arr_BhavcopyFolderPath[0], @"cd*.csv");
                for (int j = 0; j < arr_OldBhavcopyFiles.Count(); j++)
                    File.Delete(arr_OldBhavcopyFiles[j]);

                try
                {
                    var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.CD_BHAVCOPY, BhavcopyFileName, arr_BhavcopyFolderPath[0]);
                    _logger.Debug("DownloadCDBhavcopy  " + BhavcopyFileName + " | API Response: " + JsonConvert.SerializeObject(response));

                    if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                    {
                        var arr_Lines = File.ReadAllLines(arr_BhavcopyFolderPath[0] + BhavcopyFileName);
                        File.Delete(arr_BhavcopyFolderPath[0] + BhavcopyFileName);

                        var arr_Lines_Copy = new string[arr_Lines.Length + 1];
                        //arr_Lines_Copy[0] = "SYMBOL,SERIES,OPEN,HIGH,LOW,CLOSE,LAST,PREVCLOSE,TOTTRDQTY,TOTTRDVAL,TIMESTAMP,TOTALTRADES,ISIN,";
                        arr_Lines_Copy[0] = "INSTR,SYMBOL,EXPIRYDATE,STRIKEPRICE,OPTIONTYPE,PREVCLOSE,OPENPRICE,HIGHPRICE,LOWPRICE,CLOSEPRICE,QTYTRADED,VALUEINLACS,OPENINTEREST,CHANGEINOPENINTEREST,";

                        for (int i = 0; i < arr_Lines.Length; i++)
                        {
                            var arr_Fields = arr_Lines[i].SPL(',').Select(v => v.TRM()).ToArray();
                            var newLine = arr_Fields[1] + ",";
                            newLine += arr_Fields[2] + ",";
                            newLine += Convert.ToDateTime(arr_Fields[3]).ToString("dd-MMM-yyyy").ToUpper() + ",";

                            newLine += Convert.ToDouble(arr_Fields[4]).ToString() + ",";
                            newLine += (arr_Fields[5] == "" ? "XX" : arr_Fields[5]) + ",";
                            newLine += Convert.ToDouble(arr_Fields[6]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[7]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[8]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[9]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[10]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[11]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[12]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[13]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[14]).ToString() + ",";

                            arr_Lines_Copy[i + 1] = newLine;
                        }

                        BhavcopyFileName = $"cd{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv";
                        File.WriteAllLines(arr_BhavcopyFolderPath[0] + BhavcopyFileName, arr_Lines_Copy);
                        filedownloaded = true;

                    }
                }
                catch (Exception ee)
                {
                    _logger.Error(ee);
                }


                try
                {
                    if (!filedownloaded)
                    {
                        BhavcopyFileName = string.Empty;

                        string url = BhavcopyURL[1] + $"{dateEdit_DownloadDate.DateTime.STR("yyyy")}/{dateEdit_DownloadDate.DateTime.STR("MMM").UPP()}/cm{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv.zip";

                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(url, arr_BhavcopyFolderPath[0] + $"cd{ dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.zip");
                        }

                        using (ZipFile zip = ZipFile.Read(arr_BhavcopyFolderPath[0] + $"cd{ dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.zip"))
                            zip.ExtractAll(arr_BhavcopyFolderPath[0], ExtractExistingFileAction.DoNotOverwrite);

                        File.Delete(arr_BhavcopyFolderPath[0] + $"cd{ dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.zip");

                        filedownloaded = true;
                    }
                }
                catch (Exception ee) { _logger.Error(ee); }

                if (!filedownloaded)
                {
                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            //Added to login and download from NSE FTP link. 16MAR2021-Amey
                            webClient.Credentials = new NetworkCredential(dict_FTPCred["CD"].Username, dict_FTPCred["CD"].Password);

                            webClient.DownloadFile(BhavcopyURL[0] + BhavcopyFileName, arr_BhavcopyFolderPath[0] + BhavcopyFileName);

                        }

                        var arr_Lines = File.ReadAllLines(arr_BhavcopyFolderPath[0] + BhavcopyFileName);

                        File.Delete(arr_BhavcopyFolderPath[0] + BhavcopyFileName);

                        var arr_Lines_Copy = new string[arr_Lines.Length + 1];
                        //arr_Lines_Copy[0] = "SYMBOL,SERIES,OPEN,HIGH,LOW,CLOSE,LAST,PREVCLOSE,TOTTRDQTY,TOTTRDVAL,TIMESTAMP,TOTALTRADES,ISIN,";
                        arr_Lines_Copy[0] = "INSTR,SYMBOL,EXPIRYDATE,STRIKEPRICE,OPTIONTYPE,PREVCLOSE,OPENPRICE,HIGHPRICE,LOWPRICE,CLOSEPRICE,QTYTRADED,VALUEINLACS,OPENINTEREST,CHANGEINOPENINTEREST,";

                        for (int i = 0; i < arr_Lines.Length; i++)
                        {
                            var arr_Fields = arr_Lines[i].SPL(',').Select(v => v.TRM()).ToArray();
                            var newLine = arr_Fields[1] + ",";
                            newLine += arr_Fields[2] + ",";
                            newLine += Convert.ToDateTime(arr_Fields[3]).ToString("dd-MMM-yyyy").ToUpper() + ",";

                            newLine += Convert.ToDouble(arr_Fields[4]).ToString() + ",";
                            newLine += (arr_Fields[5] == "" ? "XX" : arr_Fields[5]) + ",";
                            newLine += Convert.ToDouble(arr_Fields[6]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[7]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[8]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[9]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[10]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[11]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[12]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[13]).ToString() + ",";
                            newLine += Convert.ToDouble(arr_Fields[14]).ToString() + ",";

                            arr_Lines_Copy[i + 1] = newLine;
                        }

                        BhavcopyFileName = $"cd{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv";
                        File.WriteAllLines(arr_BhavcopyFolderPath[0] + BhavcopyFileName, arr_Lines_Copy);
                        filedownloaded = true;
                    }
                    catch (Exception ee) { _logger.Error(ee); }

                }

                try
                {
                    if (!filedownloaded)
                    {
                        string filename = BhavcopyURL[2] + $"cd{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv";
                        File.Copy(filename, arr_BhavcopyFolderPath[0] + $"cd{ dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv", true);
                        filedownloaded = true;
                    }
                }
                catch (Exception ee) { _logger.Error(ee); }

                if (!filedownloaded)
                {
                    AddToList("CD Bhavcopy file download failed.", true);

                    return;
                }
                else
                {
                    if (arr_BhavcopyFolderPath.Length > 1)
                    {
                        var arr_BhavcopyCSV = Directory.GetFiles(arr_BhavcopyFolderPath[0], "cd*.csv");
                        BhavcopyFileName = $"cd{dateEdit_DownloadDate.DateTime.STR("ddMMMyyyy").UPP()}bhav.csv";
                        for (int i = 1; i < arr_BhavcopyFolderPath.Length; i++)
                        {
                            var oldcsvFiles = Directory.GetFiles(arr_BhavcopyFolderPath[i], "cd*.csv");
                            for (int j = 0; j < oldcsvFiles.Count(); j++)
                                File.Delete(oldcsvFiles[j]);

                            File.Copy(arr_BhavcopyCSV[0], arr_BhavcopyFolderPath[i] + BhavcopyFileName, true);
                        }
                    }
                }

                AddToList($"CD Bhavcopy downloaded successfully.");
            }
            catch (Exception ee) { _logger.Error(ee, $"Download CD Bhavcopy With {BhavcopyURL}"); AddToList($"Unable to download CD Bhavcopy file.", true); }
        }

        // Added by SNehadri on 08NOV2021
        private void DownloadSnapShot(string[] arr_DailySnapShotUrl, string[] arr_DailySnapShotFilePath)
        {
            try
            {
                AddToList("Daily Snapshot Downloading");

                string SnapShotFileName = $"ind_close_all_{dateEdit_DownloadDate.DateTime.STR("ddMMyyyy")}.csv";
                bool filedownloaded = false;
                if (!File.Exists(arr_DailySnapShotFilePath[0] + SnapShotFileName))
                {
                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(arr_DailySnapShotUrl[0] + SnapShotFileName, arr_DailySnapShotFilePath[0] + SnapShotFileName);
                        }
                        filedownloaded = true;
                    }
                    catch (Exception ee) { _logger.Error(ee); }

                    try
                    {
                        if (!filedownloaded)
                        {
                            File.Copy(arr_DailySnapShotUrl[1] + SnapShotFileName, arr_DailySnapShotFilePath[0] + SnapShotFileName, true);
                            filedownloaded = true;
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee); }


                    if (filedownloaded)
                    {
                        AddToList("Daily SnapShot file downloaded successfully");

                        var arr_Lines = File.ReadAllLines(arr_DailySnapShotFilePath[0] + SnapShotFileName);

                        for (int i = 0; i < arr_DailySnapShotFilePath.Length; i++)
                        {
                            var oldcsvFiles = Directory.GetFiles(arr_DailySnapShotFilePath[i], "ind_close_all_*.csv");
                            for (int j = 1; j < oldcsvFiles.Count(); j++)
                                File.Delete(oldcsvFiles[j]);

                            File.WriteAllLines(arr_DailySnapShotFilePath[i] + SnapShotFileName, arr_Lines);
                        }
                        arr_Lines = null;
                    }
                    else
                        AddToList("Daily SnapShot failed to download", true);
                }
            }
            catch (Exception ee) { _logger.Error(ee, "Download DailySnap Shot"); AddToList("Daily SnapShot failed to download", true); }
        }

        //added on 30APR2021 by Amey
        private void DownloadBSECMBhavcopy(string[] BhavcopyURL, string[] arr_BhavcopyFolderPath)
        {
            try
            {
                AddToList($"BSECM Bhavcopy downloading.");

                bool filedownloaded = false;

                string[] arr_OldBhavcopyFiles;

                for (int i = 1; i < arr_BhavcopyFolderPath.Length; i++)
                {
                    if (!Directory.Exists(arr_BhavcopyFolderPath[i]))
                        Directory.CreateDirectory(arr_BhavcopyFolderPath[i]);
                    else
                    {
                        arr_OldBhavcopyFiles = Directory.GetFiles(arr_BhavcopyFolderPath[i], @"EQ_ISINCODE_*.csv");
                        for (int j = 0; j < arr_OldBhavcopyFiles.Count(); j++)
                            File.Delete(arr_OldBhavcopyFiles[j]);
                    }
                }

                var BhavcopyFileName = $"EQ_ISINCODE_{dateEdit_DownloadDate.DateTime.STR("ddMMyy")}.zip";

                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.DownloadFile(BhavcopyURL[0] + BhavcopyFileName, arr_BhavcopyFolderPath[0] + BhavcopyFileName);
                    }

                    arr_OldBhavcopyFiles = Directory.GetFiles(arr_BhavcopyFolderPath[0], @"EQ_ISINCODE_*.csv");
                    for (int j = 0; j < arr_OldBhavcopyFiles.Count(); j++)
                        File.Delete(arr_OldBhavcopyFiles[j]);

                    using (ZipFile zip = ZipFile.Read(arr_BhavcopyFolderPath[0] + BhavcopyFileName))
                        zip.ExtractAll(arr_BhavcopyFolderPath[0], ExtractExistingFileAction.DoNotOverwrite);

                    File.Delete(arr_BhavcopyFolderPath[0] + BhavcopyFileName);
                    filedownloaded = true;
                }
                catch (Exception ee) { _logger.Error(ee); }

                try
                {
                    if (!filedownloaded)
                    {
                        BhavcopyFileName = $"EQ_ISINCODE_{dateEdit_DownloadDate.DateTime.STR("ddMMyy")}.csv";

                        File.Copy(BhavcopyURL[1] + BhavcopyFileName, arr_BhavcopyFolderPath[0] + BhavcopyFileName, true);
                        filedownloaded = true;

                    }
                }
                catch (Exception ee) { _logger.Error(ee); }

                if (filedownloaded)
                {
                    if (arr_BhavcopyFolderPath.Length > 1)
                    {
                        var arr_BhavcopyCSV = Directory.GetFiles(arr_BhavcopyFolderPath[0], "EQ_ISINCODE_*.csv");
                        BhavcopyFileName = $"EQ_ISINCODE_{dateEdit_DownloadDate.DateTime.STR("ddMMyy")}.csv";

                        for (int i = 1; i < arr_BhavcopyFolderPath.Length; i++)
                        {
                            var oldcsvFiles = Directory.GetFiles(arr_BhavcopyFolderPath[i], "EQ_ISINCODE_*.csv");
                            for (int j = 0; j < oldcsvFiles.Count(); j++)
                                File.Delete(oldcsvFiles[j]);

                            File.Copy(arr_BhavcopyCSV[0], arr_BhavcopyFolderPath[i] + BhavcopyFileName, true);
                        }
                    }

                    AddToList($"BSECM Bhavcopy downloaded successfully.");
                }
                else
                {
                    AddToList($"Unable to download BSECM Bhavcopy file.", true);
                }
            }
            catch (Exception ee) { _logger.Error(ee, $"Download BSECM Bhavcopy"); AddToList($"Unable to download BSECM Bhavcopy file.", true); }
        }

        private void InvokeDownloader(string[] SpanPath, string[] ExposurePath, string VaRExposurePath)
        {
            try
            {
                object tempObj = null;
                ElapsedEventArgs tempE = null;

                //added on 28DEC2020 by Amey
                DownloadOTMScripFile(SpanPath);
                Thread.Sleep(1000);

                //Added by Snehadri on 11NOV2022
                DownloadNiftyExposureFile(ApplicationPath, ExposurePath);
                Thread.Sleep(1000);

                //Added by Snehadri on 11NOV20222
                CombineOTMFiles(SpanPath, ExposurePath);
                Thread.Sleep(1000);

                DownloadExposureFile(ApplicationPath, ExposurePath);
                Thread.Sleep(1000);

                DownloadBSEExposureFile(ApplicationPath, ExposurePath);
                Thread.Sleep(1000);

                //Added by Akshay on 13-10-2021 for Downloading CD Span
                DownloadCDExposureFile(ApplicationPath, ExposurePath);
                Thread.Sleep(1000);

                AddToList($"VaR Exposure file downloading.");
                AddToList($"Span file downloading.");
                AddToList($"CD Span file downloading.");    //Added by Akshay on 13-10-2021 for Downloading CD Span

                Parallel.Invoke(() => DownloadSpan(tempObj, tempE, SpanPath), () => DownloadCDSpan(tempObj, tempE, SpanPath), () => DownloadVARExposure(tempObj, tempE, VaRExposurePath), () => DownloadBSESpan(tempObj, tempE, SpanPath));

                if (this.InvokeRequired)
                    this.Invoke((MethodInvoker)(() => btn_DownloadSpan.Enabled = true));

                var timer = new System.Timers.Timer();
                timer.Interval = Convert.ToInt32(ds_Config.GET("INTERVAL", "SPAN-RECHECK-SECONDS")) * 1000;

                timer.Elapsed += (sender, e) => { Parallel.Invoke(() => DownloadSpan(tempObj, tempE, SpanPath), () => DownloadCDSpan(tempObj, tempE, SpanPath), () => DownloadVARExposure(tempObj, tempE, VaRExposurePath), () => DownloadBSESpan(tempObj, tempE, SpanPath)); };
                timer.AutoReset = true;
                timer.Enabled = true;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void DownloadExposureFile(string IndexFolderPath, string[] ExposureFolderPath)
        {
            try
            {
                bool filedownloaded = false;
                string[] ExposureFileUrl = ds_Config.GET("URLs", "EXPOSURE").SPL(',');
                string ExposureFileName = "ael_" + DateTime.Now.STR("ddMMyyyy");
                string ExposureFilePath = ExposureFolderPath[0] + ExposureFileName + ".csv";

                AddToList($"Exposure file [{ExposureFileName}] downloading.");

                //using API 
                try
                {
                    var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.FO_OTM, ExposureFileName + ".csv", ExposureFolderPath[0]);
                    _logger.Debug("DownloadExposureFile " + ExposureFileName + " | API Response: " + JsonConvert.SerializeObject(response));
                    if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                    {
                        //DecompressGZAndDelete(new FileInfo(SpanFilePath + @"\" + OTMFileName), "");
                        //_logger.Debug("Extracted OTM : " + true);
                        filedownloaded = true;
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error(ee);
                }

                // using URL
                try
                {
                    if (!filedownloaded)
                    {
                        string url = ExposureFileUrl[1] + "/ael_" + DateTime.Now.STR("ddMMyyyy") + ".csv";

                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(url, ExposureFilePath);
                        }
                        filedownloaded = true;
                    }
                }
                catch (Exception ee) { _logger.Error(ee); }

                // using FTP
                if (!filedownloaded)
                {
                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            //Added to login and download from NSE FTP link. 16MAR2021-Amey
                            webClient.Credentials = new NetworkCredential(dict_FTPCred["FO"].Username, dict_FTPCred["FO"].Password);

                            webClient.DownloadFile(ExposureFileUrl[0] + ExposureFileName + ".csv", ExposureFilePath);
                            filedownloaded = true;
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee); }

                }

                // using File
                try
                {
                    if (!filedownloaded)
                    {
                        string filename = ExposureFileUrl[2] + ExposureFileName + ".csv";
                        File.Copy(filename, ExposureFilePath, true);
                        filedownloaded = true;
                    }
                }
                catch (Exception ee) { _logger.Error(ee); }

                if (filedownloaded)
                {
                    _logger.Debug("FOExposure FileName : " + ExposureFileName);
                    AddIndexExposure(ExposureFilePath, IndexFolderPath + @"\IndexExposure.csv");

                    for (int i = 1; i < ExposureFolderPath.Length; i++)
                    {
                        File.Copy(ExposureFolderPath[0] + ExposureFileName + ".csv", ExposureFolderPath[i] + ExposureFileName + ".csv", true);
                    }

                    AddToList($"Exposure file [{ExposureFileName}] downloaded successfully.");
                }
                else
                {
                    AddToList($"Exposure file failed to download.", true);
                }
            }
            catch (Exception expEX)
            {
                _logger.Error(expEX, "DownloadExposureFile");

                AddToList($"Exposure file downloaded failed.", true);
            }
        }

        private void AddIndexExposure(string ExposureFilePath, string IndexExposureFilePath)
        {
            try
            {
                using (FileStream stream = File.Open(ExposureFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string _line = reader.ReadToEnd();
                        StreamWriter _streamWriter = File.AppendText(ExposureFilePath);

                        using (FileStream _IndexStream = File.Open(IndexExposureFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (StreamReader _IndexReader = new StreamReader(_IndexStream))
                            {
                                string _innerVal;
                                while ((_innerVal = _IndexReader.ReadLine()) != null)
                                {
                                    string[] _strSep = _innerVal.Split(',');
                                    if (_line.Contains("," + _strSep[0] + ",")) continue;
                                    if (_strSep.Count() >= 4)
                                    {
                                        string LineToWrite = $"1,{_strSep[0]},{_strSep[1]},{_strSep[2]},{_strSep[3]},{_strSep[4]} {Environment.NewLine}";
                                        _streamWriter.WriteAsync(LineToWrite);
                                        _streamWriter.Flush();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                _logger.Error(err, "AddIndexExposure");
            }
        }

        private void DownloadSpan(Object source, ElapsedEventArgs e, string[] SpanFilePath)
        {
            try
            {
                if (this.InvokeRequired)
                    this.Invoke((MethodInvoker)(() => btn_DownloadSpan.Enabled = false));

                IsSpanFileDownloading = true;
                _logger.Debug("Outside IF Index : " + SpanIndex + " SpanFileExtensions Count : " + arr_SpanFileExtensions.Count());
                string[] SpanFileConfigURL = ds_Config.GET("URLs", "SPAN").SPL(',');
                if (SpanIndex < arr_SpanFileExtensions.Count())
                {
                    SpanFileDownloaded = false;

                    var SpanFileExactName = "nsccl." + DateTime.Now.Year.STR("0000") + DateTime.Now.Month.STR("00") + DateTime.Now.Date.STR("dd") + ".";
                    SpanFileExactName += arr_SpanFileExtensions[SpanIndex] + ".spn.gz";

                    var SpanFileName = SpanFileConfigURL[0] + SpanFileExactName;

                    _logger.Debug("Attempted SpanFileName : " + SpanFileName);

                    if (!Directory.Exists(SpanFilePath[0] +"TEMP"))
                        Directory.CreateDirectory(SpanFilePath[0] + "TEMP");

                    var TempSpanFilepPath = SpanFilePath[0] + "TEMP\\";

                    if (!File.Exists(SpanFilePath[0] + $"nsccl.{DateTime.Today.ToString("yyyyMMdd")}." + arr_SpanFileExtensions[SpanIndex] + ".spn"))   //changes by nikhil 
                    {
                        var DecmpressSpanFileName = $"nsccl.{DateTime.Today.ToString("yyyyMMdd")}." + arr_SpanFileExtensions[SpanIndex] + ".spn";

                        try
                        {
                            res_General response = new res_General();
                            //_logger.Debug("Downloding span : " + SpanFileExactName);

                            Thread th_API = new Thread(() => response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.FO_SPAN, SpanFileExactName, TempSpanFilepPath));
                            th_API.Start();
                            Thread.Sleep(SpanWaitSecondsApi);
                            th_API.Abort();

                            //var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.FO_SPAN, SpanFileExactName, TempSpanFilepPath);
                            _logger.Debug("Download span  " + SpanFileExactName + " | API RESPONSE :  " + JsonConvert.SerializeObject(response));
                            if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                            {
                                var decompressfilePath = DecompressGZAndDelete(new FileInfo(TempSpanFilepPath + SpanFileExactName));
                                if (decompressfilePath != "")
                                {
                                    SpanFileDownloaded = true;
                                }

                            }
                        }
                        catch (Exception EE)
                        {
                            _logger.Error(EE);
                        }

                        try
                        {
                            if (!SpanFileDownloaded)
                            {
                                string extension = string.Empty;
                                if (arr_SpanFileExtensions[SpanIndex].Length > 1)
                                {
                                    extension = arr_SpanFileExtensions[SpanIndex].SUB(0, 1) + arr_SpanFileExtensions[SpanIndex].SUB(2, 1);
                                }
                                else
                                {
                                    extension = arr_SpanFileExtensions[SpanIndex];
                                }
                                
                                string url = SpanFileConfigURL[1] + $"nsccl.{DateTime.Today.ToString("yyyyMMdd")}." + extension + ".zip";
                               
                                WebClient client = new WebClient();
                                Thread th = new Thread(() => client.DownloadFileTaskAsync(new Uri(url), TempSpanFilepPath + SpanFileExactName));
                                th.Start();
                                _logger.Debug("Span Thread init");
                                Thread.Sleep(SpanWaitSecondsWebSite);
                                client.CancelAsync();
                                th.Abort();

                                _logger.Debug("Span thread status | "+ th.IsAlive.ToString());

                                if(File.Exists(TempSpanFilepPath + SpanFileExactName))
                                {
                                    //if (FileDownloadStatus)
                                    {
                                        using (ZipFile zip = ZipFile.Read(TempSpanFilepPath + SpanFileExactName))
                                            zip.ExtractAll(TempSpanFilepPath, ExtractExistingFileAction.DoNotOverwrite);

                                        File.Delete(TempSpanFilepPath + SpanFileExactName);

                                        SpanFileDownloaded = true;
                                    }
                                }

                                //using (WebClient webClient = new WebClient())
                                //{
                                //    Task downloadTask = Task.Run(() => {
                                     
                                //        //using (WebClient webClient = new WebClient())
                                //        {
                                //            webClient.DownloadFile(new Uri(url), TempSpanFilepPath + SpanFileExactName);
                                //            FileDownloadStatus = true;

                                //            //if (token.IsCancellationRequested)  
                                //        }

                                //    },token);

                                //    Thread.Sleep(SpanWaitSecondsWebSite);
                                //    var status = downloadTask.Status.ToString();
                                    

                                //    webClient.CancelAsync();

                                //    Csource.Cancel();

                                //    status = downloadTask.Status.ToString();
                                    
                                //    //Task.WaitAll(new Task[] { downloadTask }, token);                                    
                                //}
                                //th_WebSite.Start();
                                //_logger.Debug("SPAN WebSite thread sleepd for | " + SpanWaitSecondsWebSite);
                                //Thread.Sleep(SpanWaitSecondsWebSite);
                                //_logger.Debug("Span WebSite thread aborting");
                                //th_WebSite.Abort();

                                //using (WebClient webClient = new WebClient())
                                //{
                                //    webClient.DownloadFile(url, TempSpanFilepPath + SpanFileExactName);
                                //}                                
                            }
                        }
                        catch (Exception ee) { 
                            _logger.Error(ee); 
                        }


                        if (!SpanFileDownloaded)
                        {
                            Thread th_FTP = new Thread(() => DownloadviaFTP(SpanFileName, TempSpanFilepPath, SpanFilePath, SpanFileExactName));
                            th_FTP.Start();
                            Thread.Sleep(SpanWaitSeconds);
                            th_FTP.Abort();
                        }

                        try
                        {
                            if (!SpanFileDownloaded)
                            {
                                string filename = SpanFileConfigURL[2] + $"nsccl.{DateTime.Today.ToString("yyyyMMdd")}." + arr_SpanFileExtensions[SpanIndex] + ".spn";
                                File.Copy(filename, TempSpanFilepPath + $"nsccl.{DateTime.Today.ToString("yyyyMMdd")}." + arr_SpanFileExtensions[SpanIndex] + ".spn", true);
                                SpanFileDownloaded = true;

                            }
                        }
                        catch (Exception ee) { _logger.Error(ee); }

                    }

                    if (SpanFileDownloaded)
                    {
                        var isCurrptFile = false;
                        try
                        {
                            var decompressSpanFileName = $"nsccl.{DateTime.Today.ToString("yyyyMMdd")}." + arr_SpanFileExtensions[SpanIndex] + ".spn";
                            var decompressfilePath = TempSpanFilepPath + decompressSpanFileName;
                            var tryOpen = File.ReadAllText(decompressfilePath);
                            tryOpen = "";

                            XDocument doc = new XDocument();
                            doc = XDocument.Load(decompressfilePath);

                            if(!File.Exists(SpanFilePath[0] + decompressSpanFileName))
                                 File.Copy(decompressfilePath, SpanFilePath[0] + decompressSpanFileName);

                            File.Delete(decompressfilePath);
                            
                        }
                        catch (Exception ee)
                        {
                            isCurrptFile = true;
                            _logger.Error(ee);
                        }

                        if (!isCurrptFile)
                        {
                            AddToList($"Span file [{SpanFileExactName}] downloaded successfully.");
                            _logger.Debug("Downloaded SpanFileName : " + SpanFileName);

                            string filename = $"nsccl.{DateTime.Today.ToString("yyyyMMdd")}." + arr_SpanFileExtensions[SpanIndex] + ".spn";
                            for (int i = 1; i < SpanFilePath.Length; i++)
                                File.Copy(SpanFilePath[0] + filename, SpanFilePath[i] + filename, true);

                            try
                            {
                                arr_SpanFileExtensions = arr_SpanFileExtensions.Take(arr_SpanFileExtensions.Count() - (arr_SpanFileExtensions.Count() - SpanIndex)).ToArray();
                                SpanIndex = 0;
                                _logger.Debug("After Slice Index : " + SpanIndex + " FOSpanFileExtensions Count : " + arr_SpanFileExtensions.Count());
                            }
                            catch (Exception ee) { _logger.Error(ee, "Slicing Array"); }

                            _logger.Debug("--------------------------------------------------------------------------------");
                        }
                        else
                        {
                            _logger.Debug("Download Failed SpanFileName : " + SpanFileName);
                            _logger.Debug("Currpt File downloaded");
                        }
                    }
                    else
                    {
                        _logger.Debug("Download Failed SpanFileName : " + SpanFileName);

                        SpanIndex++;
                        if (SpanIndex < arr_SpanFileExtensions.Count())
                            DownloadSpan(source, e, SpanFilePath);
                        else
                            SpanIndex = 0;
                    }
                }

                IsSpanFileDownloading = false;
                if (this.InvokeRequired)
                    this.Invoke((MethodInvoker)(() => btn_DownloadSpan.Enabled = true));
            }
            catch (Exception ee)
            {
                IsSpanFileDownloading = false;
                SpanIndex = 0;
                _logger.Error(ee, "DownloadFOSpan");
                if (this.InvokeRequired)
                    this.Invoke((MethodInvoker)(() => btn_DownloadSpan.Enabled = true));
            }
        }
       
       
        private void DownloadviaFTP(string SpanFileName, string TempSpanFilePath, string[] SpanFilePath, string SpanFileExactName)
        {

            try
            {
                try
                {
                    using (WebClient webClient = new WebClient())
                    {

                        //Added to login and download from NSE FTP link. 16MAR2021-Amey
                        webClient.Credentials = new NetworkCredential(dict_FTPCred["FO"].Username, dict_FTPCred["FO"].Password);

                        webClient.DownloadFile(SpanFileName, TempSpanFilePath + SpanFileExactName);
                    }

                    var decompressfilePath = DecompressGZAndDelete(new FileInfo(TempSpanFilePath + SpanFileExactName));
                    SpanFileDownloaded = true;

                }
                catch (Exception EE)
                {
                    _logger.Error(EE);
                }

            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private bool DownloadviaWebSite(string url, string TempSpanFilepPath, string SpanFileExactName /*string TempSpanFilePath, string[] SpanFilePath, string SpanFileExactName*/)
        {
            var result = false;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(url, TempSpanFilepPath + SpanFileExactName);
                }
                result = true;

                //using (ZipFile zip = ZipFile.Read(TempSpanFilepPath+SpanFileExactName))
                //    zip.ExtractAll(TempSpanFilepPath, ExtractExistingFileAction.DoNotOverwrite);

                //File.Delete(TempSpanFilepPath + SpanFileExactName);

                //SpanFileDownloaded = true;
            }
            catch (Exception ee) { result = false; _logger.Error(ee); }
            return result;
        }


        //bool spanfileDownloadedUsingSite = false;
        //private WebClient DownloadviaWebSite(string url,string TempSpanFilepPath, string SpanFileExactName /*string TempSpanFilePath, string[] SpanFilePath, string SpanFileExactName*/)
        //{
        //    var result = false;
        //    try
        //    {

        //        Webclient.DownloadFile(url, TempSpanFilepPath + SpanFileExactName);
        //        result = true;

        //        //using (ZipFile zip = ZipFile.Read(TempSpanFilepPath+SpanFileExactName))
        //        //    zip.ExtractAll(TempSpanFilepPath, ExtractExistingFileAction.DoNotOverwrite);

        //        //File.Delete(TempSpanFilepPath + SpanFileExactName);

        //        //SpanFileDownloaded = true;
        //    }
        //    catch (Exception ee) { result = false; _logger.Error(ee); }
        //    return result;
        //}



        //Added by Akshay on 13-10-2021 for Downloading CD Span
        private void DownloadCDSpan(Object source, ElapsedEventArgs e, string[] SpanFilePath)
        {
            try
            {
                if (SpanFilePath.Length > 0)
                {
                    if (!Directory.Exists(SpanFilePath[0] + "TEMP"))
                        Directory.CreateDirectory(SpanFilePath[0] + "TEMP");

                    var TempSpanFilePath = SpanFilePath[0] + "TEMP\\";

                    _logger.Debug("Outside IF Index : " + CDSpanIndex + " SpanFileExtensions Count : " + arr_CDSpanFileExtensions.Count());
                    var FileDownloaded = false;

                    if (CDSpanIndex < arr_CDSpanFileExtensions.Count())
                    {
                        var SpanFileConfigURL = ds_Config.GET("URLs", "CD-SPAN");
                        var SpanFileExactName = "nsccl_ix." + DateTime.Now.Year.STR("0000") + DateTime.Now.Month.STR("00") + DateTime.Now.Date.STR("dd") + ".";
                        SpanFileExactName += arr_CDSpanFileExtensions[CDSpanIndex] + ".spn.gz";

                        var CDSpanFileName = SpanFileConfigURL + SpanFileExactName;

                        _logger.Debug("CDSpanFileName Before : " + CDSpanFileName);
                        
                        var SpanFileNme = "";
                        var DownlodedFromFTP = false;

                        try
                        {

                            res_General response = new res_General();
                            Thread th_API = new Thread(() => response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.CD_SPAN, SpanFileExactName, TempSpanFilePath));
                            th_API.Start();
                            Thread.Sleep(SpanWaitSecondsApi);
                            th_API.Abort();
                            //var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.CD_SPAN, SpanFileExactName, TempSpanFilePath);
                            _logger.Debug("Download CDSspan  " + SpanFileExactName + " | API RESPONSE :  " + JsonConvert.SerializeObject(response));
                            if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                            {
                                SpanFileNme = DecompressGZAndDelete(new FileInfo(TempSpanFilePath + SpanFileExactName));
                            }
                        }
                        catch (Exception ee)
                        {
                            _logger.Error(ee, $"DownloadCDSpan {SpanFileExactName}");
                        }

                        if (SpanFileNme == "")
                        {
                            //try
                            //{
                            //    using (WebClient webClient = new WebClient())
                            //    {
                            //        //Added to login and download from NSE FTP link. 16MAR2021-Amey
                            //        webClient.Credentials = new NetworkCredential(dict_FTPCred["CD"].Username, dict_FTPCred["CD"].Password);

                            //        webClient.DownloadFile(CDSpanFileName, TempSpanFilePath + SpanFileExactName);
                            //    }
                            //    SpanFileNme = DecompressGZAndDelete(new FileInfo(TempSpanFilePath + SpanFileExactName));
                              
                            //}
                            //catch (Exception ee)
                            //{
                            //    _logger.Error(ee, $"DownloadCDSpan {SpanFileExactName}");
                               
                            //}
                        }

                        _logger.Debug("CDSpanFileName After : " + CDSpanFileName);
                        // var SpanFileNme = DecompressGZAndDelete(new FileInfo(TempSpanFilePath + SpanFileExactName));
                        if (SpanFileNme != "")
                        {
                            //trying to open
                            try
                            {
                                var tryOpen = File.ReadAllText(SpanFileNme);
                                tryOpen = "";

                                XDocument doc = new XDocument();
                                doc = XDocument.Load(SpanFileNme);

                                var decompressFileName= "nsccl_ix." + DateTime.Now.Year.STR("0000") + DateTime.Now.Month.STR("00") + DateTime.Now.Date.STR("dd") + "."+ arr_CDSpanFileExtensions[CDSpanIndex] + ".spn"; ;
                                if (!File.Exists(SpanFilePath[0] + decompressFileName))
                                    File.Copy(SpanFileNme, SpanFilePath[0] + decompressFileName);

                                File.Delete(SpanFileNme);
                                FileDownloaded = true;
                                
                            }
                            catch (Exception ee)
                            {
                                _logger.Debug("Currpt file downloaded or File opened somewhere");
                                _logger.Error(ee);
                                FileDownloaded = false;
                            }

                        }
                        
                        if (FileDownloaded)
                        {

                            string filename = $"nsccl_ix.{DateTime.Today.ToString("yyyyMMdd")}." + arr_CDSpanFileExtensions[CDSpanIndex] + ".spn";

                            for (int i = 1; i < SpanFilePath.Length; i++)
                                File.Copy(SpanFilePath[0] + filename, SpanFilePath[i] + filename, true);

                            AddToList($"CD Span file [{SpanFileExactName}] downloaded successfully.");

                            _logger.Debug("Extracted : " + true);

                            try
                            {
                                arr_CDSpanFileExtensions = arr_CDSpanFileExtensions.Take(arr_CDSpanFileExtensions.Count() - (arr_CDSpanFileExtensions.Count() - CDSpanIndex)).ToArray();
                                CDSpanIndex = 0;
                                _logger.Debug("After Slice Index : " + CDSpanIndex + " CDSpanFileExtensions Count : " + arr_CDSpanFileExtensions.Count());
                            }
                            catch (Exception ee) { _logger.Error(ee, "Slicing Array"); }

                        }
                        else
                        {
                            CDSpanIndex++;
                            if (CDSpanIndex < arr_CDSpanFileExtensions.Count())
                                DownloadCDSpan(source, e, SpanFilePath);
                            else
                                CDSpanIndex = 0;
                        }

                    }
                }
            }            
            catch (Exception ee)
            {
                CDSpanIndex = 0;
                _logger.Error(ee, "DownloadCDSpan");
            }
        }

        private void DownloadOTMScripFile(string[] SpanFilePath)
        {
            try
            {
                string[] OTMFileURL = ds_Config.Tables["URLs"].Rows[0]["OTM"].ToString().SPL(',');
                string OTMFileName = $"F_AEL_OTM_CONTRACTS_{DateTime.Now.ToString("ddMMyyyy")}.csv.gz";
                bool filedownloaded = false;

                AddToList($"OTM Exposure file [{OTMFileName}] downloading.");

                try
                {
                    var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.FO_OTM, OTMFileName, SpanFilePath[0]);
                    _logger.Debug("DownloadOTMScripFile " + OTMFileName + " | API Response: " + JsonConvert.SerializeObject(response));
                    if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                    {
                        DecompressGZAndDelete(new FileInfo(SpanFilePath[0] + @"\" + OTMFileName), "");
                        _logger.Debug("Extracted OTM : " + true);
                        filedownloaded = true;
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error(ee);
                }


                if (!filedownloaded)
                {
                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            //Added to login and download from NSE FTP link. 16MAR2021-Amey
                            webClient.Credentials = new NetworkCredential(dict_FTPCred["FO"].Username, dict_FTPCred["FO"].Password);
                            webClient.DownloadFile(OTMFileURL[0] + OTMFileName, SpanFilePath[0] + @"\" + OTMFileName);
                        }

                        DecompressGZAndDelete(new FileInfo(SpanFilePath[0] + @"\" + OTMFileName), "");

                        _logger.Debug("Extracted OTM : " + true);
                        filedownloaded = true;

                    }
                    catch (Exception ee) { _logger.Error(ee); }
                }

                if (filedownloaded)
                {
                    var FileName = $"F_AEL_OTM_CONTRACTS_{DateTime.Now.ToString("ddMMyyyy")}.csv";

                    for (int i = 1; i < SpanFilePath.Length; i++)
                    {
                        File.Copy(SpanFilePath[0] + FileName, SpanFilePath[i] + FileName, true);
                    }

                    AddToList($"OTM file [{OTMFileName}] downloaded successfully.");

                }
                else
                    AddToList($"Unable to download OTM file.", true);
            }
            catch (Exception ee) { _logger.Error(ee); AddToList($"Unable to download OTM file.", true); }
        }

        // Added by Snehadri on 11NOV2022
        private void DownloadNiftyExposureFile(string IndexFolderPath, string[] ExposureFolderPath)
        {
            try
            {
                bool filedownloaded = false;
                string[] ExposureFileUrl = ds_Config.GET("URLs", "NIFTY_EXPOSURE").SPL(',');
                string ExposureFileName = "ael_NIFTY_Options";
                string ExposureFilePath = ExposureFolderPath[0] + ExposureFileName + ".csv";

                AddToList($"Nifty Exposure file [{ExposureFileName}] downloading.");

                //using API 
                try
                {
                    var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.FO_OTM, ExposureFileName + ".csv", ExposureFolderPath[0]);
                    _logger.Debug("DownloadNiftyExposureFile " + ExposureFileName + " | API Response: " + JsonConvert.SerializeObject(response));
                    if (response.ResponseStatus == en_ResponseStatus.SUCCESS)
                    {
                        //DecompressGZAndDelete(new FileInfo(SpanFilePath + @"\" + OTMFileName), "");
                        //_logger.Debug("Extracted OTM : " + true);
                        filedownloaded = true;
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error(ee);
                }

                // using FT
                if (!filedownloaded)
                {
                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            //Added to login and download from NSE FTP link. 16MAR2021-Amey
                            webClient.Credentials = new NetworkCredential(dict_FTPCred["FO"].Username, dict_FTPCred["FO"].Password);

                            webClient.DownloadFile(ExposureFileUrl[0] + ExposureFileName + ".csv", ExposureFilePath);
                            filedownloaded = true;
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee); }
                }
              
                // using File
                try
                {
                    if (!filedownloaded)
                    {
                        string filename = ExposureFileUrl[2] + ExposureFileName + ".csv";
                        File.Copy(filename, ExposureFilePath, true);
                        filedownloaded = true;
                    }
                }
                catch (Exception ee) { _logger.Error(ee); }

                if (filedownloaded)
                {

                    for (int i = 1; i < ExposureFolderPath.Length; i++)
                    {
                        File.Copy(ExposureFolderPath[0] + ExposureFileName + ".csv", ExposureFolderPath[i] + ExposureFileName + ".csv", true);
                    }

                    _logger.Debug("Nifty Exposure FileName : " + ExposureFileName);
                    AddToList($"Nifty Exposure file [{ExposureFileName}] downloaded successfully.");
                }
                else
                {
                    AddToList($"Nifty Exposure file failed to download.", true);
                }
            }
            catch (Exception expEX)
            {
                _logger.Error(expEX, "DownloadNiftyExposureFile ");
            }
        }

        // Added by Snehadri on 11NOV2022
        private void CombineOTMFiles(string[] SpanPath, string[] ExposurePath)
        {
            try
            {
                var Directory = new DirectoryInfo(ExposurePath[0]);

                var OTMFile = Directory.GetFiles("F_AEL_OTM_CONTRACTS*.csv")
                               .OrderByDescending(f => f.LastWriteTime)
                               .FirstOrDefault();

                var NiftyOTMFile = Directory.GetFiles("ael_NIFTY_Options*.csv")
                           .OrderByDescending(f => f.LastWriteTime)
                           .FirstOrDefault();


                var BhavcopyDirectory = new DirectoryInfo(@"C:/Prime");

                var FOBhavcopy = BhavcopyDirectory.GetFiles("fo*.csv")
                               .OrderByDescending(f => f.LastWriteTime)
                               .First();

                if (OTMFile != null && NiftyOTMFile != null)
                {
                    AddToList("Adding Scrips in OTM File");

                    var _Result = AddOTMExposure(FOBhavcopy.FullName, ExposurePath);

                    if (!_Result)
                    {
                        AddToList("Adding Scrips in OTM File Failed", true);
                        DownloadOTMScripFile(SpanPath);
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.Error(ee, "CombineOTMFiles ");
            }
        }

        private bool AddOTMExposure(string FOBhavcopyPath, string[] ExposureFolder)
        {
            bool result = false;

            try
            {
                ConcurrentDictionary<string, OTMFileData> dict_OTMFileData = new ConcurrentDictionary<string, OTMFileData>();
                ConcurrentDictionary<string, NiftyOTMFile> dict_NiftyOTMFile = new ConcurrentDictionary<string, NiftyOTMFile>();

                var list_FOBhavcopy = Exchange.ReadFOBhavcopy(FOBhavcopyPath,false);

                var list_NiftyContracts = list_FOBhavcopy.Where(v => v.Symbol == "NIFTY").ToList();

                var OTMFileName = $"F_AEL_OTM_CONTRACTS_{DateTime.Now.ToString("ddMMyyyy")}.csv";
                var NiftyOTMFileName = "ael_NIFTY_Options.csv";

                using (FileStream fs = File.Open(ExposureFolder[0] + OTMFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BufferedStream bs = new BufferedStream(fs))
                    {
                        using (StreamReader sr = new StreamReader(bs))
                        {
                            string strData = string.Empty;

                            while ((strData = sr.ReadLine()) != null)
                            {
                                var data = strData.ToUpper().Split(',').Select(v => v.Trim()).ToArray();

                                OTMFileData oTMFileData = new OTMFileData()
                                {
                                    InstName = data[0],
                                    Symbol = data[1],
                                    ExpiryDate = DateTime.Parse(data[2]),
                                    StrikePrice = data[3],
                                    ScripType = data[4],
                                    Percentage = data[6]
                                };

                                string _Key = $"{oTMFileData.Symbol}|{oTMFileData.ScripType}|{oTMFileData.StrikePrice}|{oTMFileData.ExpiryDate.ToString("dd-MMM-yy")}";

                                dict_OTMFileData.TryAdd(_Key, oTMFileData);
                            }
                        }
                    }
                }

                using (FileStream fs = File.Open(ExposureFolder[0] + NiftyOTMFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BufferedStream bs = new BufferedStream(fs))
                    {
                        using (StreamReader sr = new StreamReader(bs))
                        {
                            string strData = string.Empty;

                            while ((strData = sr.ReadLine()) != null)
                            {
                                var data = strData.ToUpper().Split(',').Select(v => v.Trim()).ToArray();


                                var _Symbol = data[1];
                                var _Expiry = data[3];

                                string _Key = $"{_Symbol}|{_Expiry}";

                                if (dict_NiftyOTMFile.TryGetValue(_Key, out NiftyOTMFile oNiftyOTMFile))
                                {
                                    if (data[2] == "OTM")
                                        oNiftyOTMFile.OTMPercentage = data[4];
                                    else
                                        oNiftyOTMFile.OTHPercentage = data[4];
                                }
                                else
                                {
                                    NiftyOTMFile OTMFileData = new NiftyOTMFile()
                                    {
                                        Symbol = _Symbol,
                                        ExpiryDate = _Expiry
                                    };

                                    if (data[2] == "OTM")
                                    {
                                        OTMFileData.OTMPercentage = data[4];
                                        OTMFileData.OTHPercentage = "0";
                                    }
                                    else
                                    {
                                        OTMFileData.OTHPercentage = data[4];
                                        OTMFileData.OTMPercentage = "0";
                                    }

                                    dict_NiftyOTMFile.TryAdd(_Key, OTMFileData);

                                }
                            }
                        }
                    }
                }

                StringBuilder sb_NewData = new StringBuilder();

                var _Today = DateTime.Today.AddHours(15).AddMinutes(15);

                foreach (var _Contract in list_NiftyContracts)
                {
                    if (GetMonthDifference(_Contract.Expiry, _Today) < 9) continue;

                    string _Key = $"{_Contract.Symbol}|{_Contract.ScripType}|{_Contract.StrikePrice}|{_Contract.Expiry.ToString("dd-MMM-yy")}";

                    var _NKey = $"{_Contract.Symbol}|{_Contract.Expiry.ToString("dd-MMM-yy").ToUpper()}";

                    if (!dict_NiftyOTMFile.ContainsKey(_NKey)) continue;

                    var _NValue = dict_NiftyOTMFile[_NKey];

                    if (!dict_OTMFileData.ContainsKey(_Key))
                    {
                        var str = $"{_Contract.Instrument},{_Contract.Symbol},{_Contract.Expiry.ToString("dd-MMM-yyyy").ToUpper()},{_Contract.StrikePrice},{_Contract.ScripType},0,{_NValue.OTHPercentage}";
                        sb_NewData.AppendLine(str);
                    }
                }

                for (int i = 0; i < ExposureFolder.Length; i++)
                    File.AppendAllText(ExposureFolder[i] + OTMFileName, sb_NewData.ToString());

                AddToList("Added Scrips in OTM File");

                result = true;
            }
            catch (Exception ee)
            {
                _logger.Error(ee, "AddOTMExposure ");
            }

            return result;
        }

        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = (12 * (startDate.Year - endDate.Year)) + (startDate.Month - endDate.Month);
            return Math.Abs(monthsApart);
        }

        private void DownloadVARExposure(Object source, ElapsedEventArgs e, string VaRExposureFilePath)
        {
            try
            {
                _logger.Debug("[VaRExposure] Outside IF Index : " + VaRIndex + " VaRFileExtensions Count : " + arr_VaRFileExtensions.Count());
                if (VaRIndex < arr_VaRFileExtensions.Count())
                {
                   bool filedownloaded = false;

                    string[] VaRUrl = ds_Config.GET("URLs", "VAREXPOSURE").SPL(',');

                    string VaRFileName = VaRUrl[0];
                    VaRFileName += "C_VAR1_" + DateTime.Now.STR("ddMMyyyy") + "_";
                    VaRFileName += arr_VaRFileExtensions[VaRIndex] + ".DAT";

                    var ExactSpanName = VaRFileName.SUB((VaRFileName.CON("\\") ? VaRFileName.LastIndexOf("\\") : VaRFileName.LastIndexOf("/")) + 1);

                    _logger.Debug("[VaRExposure] VaRFileName Before : " + VaRFileName);

                    File.Delete(VaRExposureFilePath + ExactSpanName);

                    try
                    {
                        _logger.Debug("Downloding Var File : " + ExactSpanName);

                        res_General response = new res_General();
                        
                        Thread th_API = new Thread(() => response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.CM_VAREXPOSURE, ExactSpanName, VaRExposureFilePath));
                        th_API.Start();
                        Thread.Sleep(SpanWaitSecondsApi);
                        th_API.Abort();

                        //var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.CM_VAREXPOSURE, ExactSpanName, VaRExposureFilePath);
                        _logger.Debug("DownloadVarFile API Response: " + JsonConvert.SerializeObject(response));
                        filedownloaded = response.ResponseStatus == en_ResponseStatus.SUCCESS ? true : false;
                    }
                    catch (Exception ee) { _logger.Error(ee); }

                    // using Url
                    try
                    {
                        if (!filedownloaded)
                        {
                            string url = VaRUrl[1] + "C_VAR1_" + DateTime.Now.STR("ddMMyyyy") + "_" + arr_VaRFileExtensions[VaRIndex] + ".DAT";

                            //var FileDownloadStatus = false;
                            //Thread th_WebSite = new Thread(() => FileDownloadStatus = DownloadviaWebSite(url, VaRExposureFilePath, ExactSpanName));
                            //th_WebSite.Start();
                            //_logger.Debug("VAR WebSite thread sleeped for | " + SpanWaitSecondsWebSite);
                            //Thread.Sleep(SpanWaitSecondsWebSite);
                            //_logger.Debug("VAR Website thread aborting ");
                            //th_WebSite.Abort();

                            WebClient client = new WebClient();
                            Thread th = new Thread(() => client.DownloadFileTaskAsync(new Uri(url), VaRExposureFilePath + ExactSpanName));
                            th.Start();
                            _logger.Debug("VAR Thread init");
                            Thread.Sleep(SpanWaitSecondsWebSite);
                            client.CancelAsync();

                            th.Abort();

                            _logger.Debug("Var thread status | " + th.IsAlive.ToString());
                            if (File.Exists(VaRExposureFilePath + ExactSpanName))
                            {
                                var arr_bytes = File.ReadAllBytes(VaRExposureFilePath + ExactSpanName);
                                if (arr_bytes.Length > 0)
                                    filedownloaded = true;
                                else
                                    File.Delete(VaRExposureFilePath + ExactSpanName);
                            }

                        }
                    }
                    catch (Exception ee) { _logger.Error(ee); }

                    // using FTP
                    //if (!filedownloaded)
                    //{
                    //    try
                    //    {
                    //        using (WebClient webClient = new WebClient())
                    //        {
                    //            //Added to login and download from NSE FTP link. 16MAR2021-Amey
                    //            webClient.Credentials = new NetworkCredential(dict_FTPCred["GUEST"].Username, dict_FTPCred["GUEST"].Password);

                    //            webClient.DownloadFile(VaRFileName, VaRExposureFilePath + ExactSpanName);

                    //            filedownloaded = true;
                    //        }
                    //    }
                    //    catch (Exception ee) { _logger.Error(ee); }
                    //}

                    // using file
                    try
                    {
                        if (!filedownloaded)
                        {
                            string filename = VaRUrl[2] + "C_VAR1_" + DateTime.Now.STR("ddMMyyyy") + "_" + arr_VaRFileExtensions[VaRIndex] + ".DAT";
                            File.Copy(filename, VaRExposureFilePath + "C_VAR1_" + DateTime.Now.STR("ddMMyyyy") + "_" + arr_VaRFileExtensions[VaRIndex] + ".DAT", true);
                            filedownloaded = true;
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee); }

                    _logger.Debug("[VaRExposure] VaRFileName After : " + VaRFileName);

                    if (filedownloaded)
                    {
                        AddToList($"VaR file [{ExactSpanName}] downloaded successfully.");

                        try
                        {
                            arr_VaRFileExtensions = arr_VaRFileExtensions.Take(arr_VaRFileExtensions.Count() - (arr_VaRFileExtensions.Count() - VaRIndex)).ToArray();
                            VaRIndex = 0;
                            _logger.Debug("[VaRExposure] After Slice Index : " + VaRIndex + " arr_VaRFileExtensions Count : " + arr_VaRFileExtensions.Count());
                        }
                        catch (Exception ee) { _logger.Error(ee, "[VaRExposure] Slicing Array"); }
                    }
                    else
                    {
                        VaRIndex++;

                        if (VaRIndex < arr_VaRFileExtensions.Count())
                            DownloadVARExposure(source, e, VaRExposureFilePath);
                        else
                        {
                            VaRIndex = 0;
                            _logger.Debug("[VaRExposure] Failed to dowanload, Index : " + VaRIndex + " arr_VaRFileExtensions Count : " + arr_VaRFileExtensions.Count());
                        }
                    }

                }
            }
            catch (Exception ee)
            {
                VaRIndex = 0;
                _logger.Error(ee);
            }
        }

        private void DownloadBSEExposureFile(string IndexFolderPath, string[] ExposureFolderPath)
        {
            try
            {
                string ExposureFileName = "EF" + DateTime.Now.STR("ddMMyy");
                string ExposureFilePath = ExposureFolderPath[0] + @"\" + ExposureFileName;

                AddToList($"BSE-Exposure file [{ExposureFileName}] downloading.");

                using (WebClient webClient = new WebClient())
                    webClient.DownloadFile(ds_Config.GET("URLs", "BSE-EXPOSURE") + ExposureFileName, ExposureFilePath);

                //string NSEExposureFileName = "ael_" + DateTime.Now.STR("ddMMyyyy") + ".csv";
                //string NSEExposureFilePath = ExposureFolderPath + @"\" + NSEExposureFileName;

                var list_ExposureLines = new List<string>();
                using (var stream = new FileStream(ExposureFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            // Do something with line, e.g. add to a list or whatever.
                            list_ExposureLines.Add(line);
                        }
                    }
                }

                var dict_BSEUnderlyingNames = ReadBSEConvertorFiles();
                var dict_BSEScripNames = ReadBSEContractFile();

                StringBuilder sb_BSEExposure = new StringBuilder();
                sb_BSEExposure.AppendLine($"#{DateTime.Now.STR("dd-MM-yyyy")}|EF{DateTime.Now.STR("ddMMyy")}|");

                for (int i = 1; i < list_ExposureLines.Count; i++)
                {
                    try
                    {
                        var arr_Fields = list_ExposureLines[i].SPL(',');
                        var Token = Convert.ToInt32(arr_Fields[0]);

                        if (dict_BSEScripNames.ContainsKey(Token) && dict_BSEUnderlyingNames.ContainsKey(dict_BSEScripNames[Token]))
                            sb_BSEExposure.AppendLine($"{Token},{dict_BSEUnderlyingNames[dict_BSEScripNames[Token]]},{arr_Fields[2]}");
                    }
                    catch (Exception ee) { _logger.Error(ee, "DownloadBSEExposure : " + list_ExposureLines[i]); }
                }

                var PrimeDirectory = new DirectoryInfo("C:/Prime");

                string _IndexExposureBSE = "BSEIndexExposure";

                var BSEIndexExposure = PrimeDirectory.GetFiles($"{_IndexExposureBSE}*.csv")
                           .OrderByDescending(f => f.LastWriteTime)
                           .First();

                if (BSEIndexExposure.Length > 0)
                {
                    var arr_IndexExLines = File.ReadAllLines(BSEIndexExposure.FullName);
                    foreach (var line in arr_IndexExLines)
                    {
                        var arr_Fields = line.SPL(',');
                        sb_BSEExposure.AppendLine($"{arr_Fields[0]},{arr_Fields[1]},{arr_Fields[2]}");
                    }
                }

                File.WriteAllText(ExposureFilePath, sb_BSEExposure.STR());

                _logger.Debug("BSE-FOExposure FileName : " + ExposureFileName);

                AddToList($"BSE-Exposure file [{ExposureFileName}] downloaded successfully.");
            }
            catch (Exception expEX)
            {
                _logger.Error(expEX);

                AddToList($"BSE-Exposure file downloaded failed.", true);
            }
        }

        //Added by Akshay on 13-10-2021 for Downloading Exposure File for CD
        private void DownloadCDExposureFile(string IndexExposureFilePath, string[] ExposureFolderPath)
        {
            try
            {
                string ExactFileName = "CDSael_" + DateTime.Now.ToString("ddMMyyyy") + ".csv";
                string ExposureFileName = ExposureFolderPath[0] + ExactFileName;

                AddToList($"CD Exposure file [{ExposureFileName}] downloading.");

                File.WriteAllText(ExposureFileName, File.ReadAllText(IndexExposureFilePath + @"\CDExposure.csv"));

                for (int i = 1; i < ExposureFolderPath.Length; i++)
                    File.Copy(ExposureFolderPath[0] + ExactFileName, ExposureFolderPath[i] + ExactFileName, true);

                AddToList($"CD Exposure file [{ExactFileName}] downloaded successfully.");

                _logger.Debug("CDExposure FileName : " + ExposureFileName);
            }
            catch (Exception ee) { _logger.Error(ee, "DownloadCDSpan"); }
        }

        private Dictionary<string, string> ReadBSEConvertorFiles()
        {
            var dict_BSEUnderlyingNames = new Dictionary<string, string>();

            try
            {
                var PrimeDirectory = new DirectoryInfo("C:/Prime");

                string _ScripConvertorFileFO = "EQD_CC_CO";
                string _ScripConvertorFileCM = "EQ_MAP_CC_";

                var ScripConvertorFileFO = PrimeDirectory.GetFiles($"{_ScripConvertorFileFO}*.csv")
                           .OrderByDescending(f => f.LastWriteTime)
                           .First();

                if (ScripConvertorFileFO.Length > 0)
                {
                    var arr_Lines = File.ReadAllLines(ScripConvertorFileFO.FullName);
                    foreach (var _line in arr_Lines)
                    {
                        try
                        {
                            var arr_Fields = _line.UPP().SPL(',');

                            if (arr_Fields.Length <= 4) continue;

                            if (!dict_BSEUnderlyingNames.ContainsKey(arr_Fields[4].TRM()))
                                dict_BSEUnderlyingNames.Add(arr_Fields[4].TRM(), arr_Fields[3].TRM());
                        }
                        catch (Exception ee) { _logger.Error(ee, "ReadBSEConvertor Loop FO : " + _line); }
                    }
                }

                var ScripConvertorFileCM = PrimeDirectory.GetFiles($"{_ScripConvertorFileCM}*.csv")
                           .OrderByDescending(f => f.LastWriteTime)
                           .First();

                if (ScripConvertorFileCM.Length > 0)
                {
                    var arr_Lines = File.ReadAllLines(ScripConvertorFileCM.FullName);
                    foreach (var _line in arr_Lines)
                    {
                        try
                        {
                            var arr_Fields = _line.UPP().SPL(',');

                            if (arr_Fields.Length <= 5) continue;

                            if (arr_Fields[2].TRM() == "0") continue;

                            if (arr_Fields[6].TRM() != "EQ") continue;

                            if (!dict_BSEUnderlyingNames.ContainsKey(arr_Fields[5].TRM()))
                                dict_BSEUnderlyingNames.Add(arr_Fields[5].TRM(), arr_Fields[2].TRM());
                        }
                        catch (Exception ee) { _logger.Error(ee, "ReadBSEConvertor Loop CM : " + _line); }
                    }
                }

                
            }
            catch (Exception ee) { _logger.Error(ee); }

            return dict_BSEUnderlyingNames;
        }

        private Dictionary<int, string> ReadBSEContractFile()
        {
            var dict_BSEScripName = new Dictionary<int, string>();

            try
            {
                var PrimeDirectory = new DirectoryInfo("C:/Prime");

                string _ScripFileBSE = "SCRIP_CC_";

                var ScripFileBSE = PrimeDirectory.GetFiles($"{_ScripFileBSE}*.txt")
                           .OrderByDescending(f => f.LastWriteTime)
                           .First();

                if (ScripFileBSE.Length > 0)
                {
                    var arr_Lines = File.ReadAllLines(ScripFileBSE.FullName);
                    foreach (var line in arr_Lines)
                    {
                        var arr_Fields = line.SPL('|');
                        if (arr_Fields[1] == "BSE")
                        {
                            var Token = Convert.ToInt32(arr_Fields[0]);

                            if (!dict_BSEScripName.ContainsKey(Token))
                                dict_BSEScripName.Add(Token, arr_Fields[2]);
                        }
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }

            return dict_BSEScripName;
        }

        private void DownloadBSESpan(Object source, ElapsedEventArgs e, string[] SpanFilePath)
        {
            try
            {
                _logger.Debug("Outside IF Index : " + BSESpanIndex + " BSESpanFileExtensions Count : " + arr_BSESpanFileExtensions.Count());
                if (BSESpanIndex < arr_BSESpanFileExtensions.Count())
                {
                    //BSERISK20210317-00
                    var SpanFileConfigURL = ds_Config.GET("URLs", "BSE-SPAN");
                    var SpanFileExactName = "BSERISK" + DateTime.Now.Year.STR("0000") + DateTime.Now.Month.STR("00") + DateTime.Now.Date.STR("dd") + "-";
                    SpanFileExactName += arr_BSESpanFileExtensions[BSESpanIndex] + ".spn";

                    var SpanFileName = SpanFileConfigURL + SpanFileExactName;

                    _logger.Debug("SpanFileName Before : " + SpanFileName);

                    using (WebClient webClient = new WebClient())
                        webClient.DownloadFile(SpanFileName, SpanFilePath[0] + SpanFileExactName);

                    _logger.Debug("SpanFileName After : " + SpanFileName);

                    AddToList($"Span file [{SpanFileExactName}] downloaded successfully.");

                    try
                    {
                        arr_BSESpanFileExtensions = arr_BSESpanFileExtensions.Take(arr_BSESpanFileExtensions.Count() - (arr_BSESpanFileExtensions.Count() - BSESpanIndex)).ToArray();
                        BSESpanIndex = 0;
                        _logger.Debug("After Slice Index : " + BSESpanIndex + " BSEFOSpanFileExtensions Count : " + arr_BSESpanFileExtensions.Count());
                    }
                    catch (Exception ee) { _logger.Error(ee, "Slicing Array"); }

                    _logger.Debug("--------------------------------------------------------------------------------");
                }
            }
            catch (WebException)
            {
                BSESpanIndex++;
                _logger.Debug("Inside WebException Before Index : " + BSESpanIndex + " BSEFOSpanFileExtensions Count : " + arr_BSESpanFileExtensions.Count());

                if (BSESpanIndex < arr_BSESpanFileExtensions.Count())
                    DownloadBSESpan(source, e, SpanFilePath);
                else
                {
                    BSESpanIndex = 0;
                    _logger.Debug("Inside WebException After Index : " + BSESpanIndex + " BSEFOSpanFileExtensions Count : " + arr_BSESpanFileExtensions.Count());
                }
            }
            catch (Exception ee)
            {
                BSESpanIndex = 0;
                _logger.Error(ee, "DownloadBSEFOSpan");
            }
        }

        private void DownloadFOSecBanFile(string[] URL, string SavePath)
        {

            try
            {
                bool filedownloaded = false;
                //added on 15APR2021 by Amey. To delete old files.
                var DownloadDirectory = new DirectoryInfo(SavePath);
                try
                {
                    var OldFiles = DownloadDirectory.GetFiles("fo_secban_*.csv");
                    foreach (var OldFOSecfile in OldFiles)
                        File.Delete(OldFOSecfile.FullName);
                }
                catch(Exception ee)
                {
                    _logger.Error(ee);
                }
             

                string FOSecFileName = $"fo_secban_{DateTime.Now.ToString("ddMMyyyy")}.csv";
                AddToList($"FO-SEC file [{FOSecFileName}] downloading.");

                try
                {
                    var response = nNSEUtils.Instance.DownloadCommonFile(en_FolderTypes.FO_SEC_BAN, FOSecFileName, SavePath);
                    _logger.Debug("DownloadFOSecBanFile" + FOSecFileName + " | API Response : " + JsonConvert.SerializeObject(response));
                    filedownloaded = response.ResponseStatus == en_ResponseStatus.SUCCESS ? true : false;
                }
                catch (Exception ee)
                {
                    _logger.Error(ee);
                }

                //using url
                try
                {
                    if (!filedownloaded)
                    {
                        string url = URL[1] + "fo_secban.csv";

                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(url, SavePath + @"\" + FOSecFileName);
                        }
                        filedownloaded = true;
                    }
                }
                catch (Exception ee) { _logger.Error(ee); }

                if (!filedownloaded)
                {

                    // using FTP
                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            //Added to login and download from NSE FTP link. 16MAR2021-Amey
                            webClient.Credentials = new NetworkCredential(dict_FTPCred["FO"].Username, dict_FTPCred["FO"].Password);
                            webClient.DownloadFile(URL[0] + FOSecFileName, SavePath + @"\" + FOSecFileName);
                            filedownloaded = true;
                        }
                    }
                    catch (Exception ee) { _logger.Error(ee); }

                }



                // using file
                try
                {
                    if (!filedownloaded)
                    {
                        string filename = URL[2] + FOSecFileName;
                        if (File.Exists(filename))
                        {
                            File.Copy(filename, SavePath + FOSecFileName, true);

                        }
                        filedownloaded = true;
                    }
                }
                catch (Exception ee) { _logger.Error(ee); }

                if (!filedownloaded)
                    AddToList($"Unable to download FO-SEC file.", true);
                else
                    AddToList($"FO-SEC file [{FOSecFileName}] downloaded successfully.");
            }
            catch (Exception ee) { _logger.Error(ee); AddToList($"Unable to download FO-SEC file.", true); }
        }

        private string DecompressGZAndDelete(FileInfo fileToDecompress, string FileExtension = "")
        {
            string newFileName = "";

            try
            {
                string currentFileName = fileToDecompress.FullName;

                using (FileStream originalFileStream = fileToDecompress.OpenRead())
                {
                    newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length) + (FileExtension != "" ? FileExtension : "");

                    File.Delete(newFileName);

                    using (FileStream decompressedFileStream = File.Create(newFileName))
                    {
                        using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress,true))
                        {
                            decompressionStream.CopyTo(decompressedFileStream);
                        }
                    }
                }

                File.Delete(currentFileName);
            }
            catch (Exception ee) { _logger.Error(ee); newFileName = ""; }

            return newFileName;
        }

        private void listBox_Messages_DrawItem(object sender, ListBoxDrawItemEventArgs e)
        {
            try
            {
                if (hs_ErrorIndex.CON(listBox_Messages.Items[e.Index].STR()))
                    e.Appearance.ForeColor = Color.OrangeRed;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void ReadConfig()
        {
            try
            {
                XmlTextReader tReader = new XmlTextReader("C:/Prime/Config.xml");
                tReader.Read();
                ds_SQLConfig.ReadXml(tReader);

                var DBInfo = ds_SQLConfig.Tables["DB"].Rows[0];

                //added convert zero datetime=True on 16APR2021 by Amey. Was not assigning SQL datetime to C# DateTime in sp_ContractMaster.
                _MySQLCon = $"Data Source={DBInfo["SERVER"]};Initial Catalog={DBInfo["NAME"]};UserID={DBInfo["USER"]};Password={DBInfo["PASSWORD"]};SslMode=none;convert zero datetime=True;";

                var CONInfo = ds_SQLConfig.Tables["CONNECTION"].Rows[0];

                SetMaxAllowedSqlPacket();

                //added on 28FEB2021 by Amey
                FlushMySQLConnectionErrors();

                //try
                //{
                //    SecretKey = 


                //}catch(Exception ee)
                //{

                //}
                



            }
            catch (Exception error)
            {
                _logger.Error(error, ": ReadConfig");
                XtraMessageBox.Show("Invalid entry in Config file. Please check logs for more details.", "Error");
            }
        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
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
                _logger.Error(errror, "SetMaxAllowedSqlPacket");
            }
        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
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
                _logger.Error(errror, " : FlushMySQLConnectionErrors");
            }
        }

        // Added by Snehadri on 14JUL2021
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
                _logger.Error(errror, "CheckMaxAllowedSqlPacket ");
            }
        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void StartCMFeedReceivers(string[] CMFeedPath)
        {
            // CM Feed Receiver     
            try
            {
                CloseComponentexe("CM FeedReceiver");

                foreach (var cmpath in CMFeedPath)
                {
                    OpenComponentexe("CM FeedReceiver", cmpath);
                    bool connected = GatewayEngineConnector.ConnectComponents("CMFeedReceiver");

                    if (connected)
                    {
                        AddToList($"CM Feed Receiver Started ({cmpath}).");
                        if (!list_ComponentStarted.Contains("CMFeedReceiver")) { list_ComponentStarted.Add("CMFeedReceiver"); }
                        Thread.Sleep(3000);
                    }
                    else
                    {
                        CloseComponentexe("CM FeedReceiver");

                        AddToList($"Can't open CM feed receiver ({cmpath}). Please check log.", true);
                        IsWorking = false;
                        btn_RestartAuto.Enabled = true;
                        btn_Settings.Enabled = true;
                        SentMail("CM Feed Receiver has failed to launch.");

                        if (list_ComponentStarted.Contains("CMFeedReceiver")) { list_ComponentStarted.Remove("CMFeedReceiver"); }                        
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.Error(ee, "CM Feed Receiver");
                AddToList("Can't open CM feed receiver. Please check log", true);
                IsWorking = false;
                btn_RestartAuto.Enabled = true;
                btn_Settings.Enabled = true;
                if (list_ComponentStarted.Contains("CMFeedReceiver")) { list_ComponentStarted.Remove("CMFeedReceiver"); }

                SentMail("CM Feed Receiver has failed to launch.");

            }
        }

        private void StartFOFeedReceiver(string[] FOFeedpath)
        {
                  
            try
            {
                CloseComponentexe("FO FeedReceiver");

                foreach (var fopath in FOFeedpath)
                {
                    OpenComponentexe("FO FeedReceiver", fopath);
                    bool connected = GatewayEngineConnector.ConnectComponents("FOFeedReceiver");
                    if (connected)
                    {
                        AddToList($"FO Feed Receiver Started ({fopath}).");
                        if (!list_ComponentStarted.Contains("FOFeedReceiver")) 
                            list_ComponentStarted.Add("FOFeedReceiver"); 
                        Thread.Sleep(3000);
                    }
                    else
                    {
                        CloseComponentexe("FO FeedReceiver");
                        AddToList("Can't open FO feed receiver. Please check log", true);
                        IsWorking = false;
                        btn_RestartAuto.Enabled = true;
                        btn_Settings.Enabled = true;
                        SentMail("FO Feed Receiver has failed to launch");
                        if (list_ComponentStarted.Contains("FOFeedReceiver")) 
                             list_ComponentStarted.Remove("FOFeedReceiver");                        
                        break;
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.Error(ee, "FO Feed Receiver");
                AddToList("Can't open FO feed receiver. Please check log", true);
                IsWorking = false;
                btn_RestartAuto.Enabled = true;
                btn_Settings.Enabled = true;
                if (list_ComponentStarted.Contains("FOFeedReceiver")) 
                    list_ComponentStarted.Remove("FOFeedReceiver");
                SentMail("FO Feed Receiver has failed to launch");
            }
        }

        private void StartCDFeedReceiver(string[] CDFeedPath)
        {
            // CD Feed Receiver   
            try
            {
                CloseComponentexe("CD FeedReceiver");

                foreach (var cdpath in CDFeedPath)
                {
                    OpenComponentexe("CD FeedReceiver", cdpath);

                    bool connected = GatewayEngineConnector.ConnectComponents("CDFeedReceiver");

                    if (connected)
                    {
                        AddToList($"CD Feed Receiver Started ({cdpath}).");
                        if (!list_ComponentStarted.Contains("CDFeedReceiver")) { list_ComponentStarted.Add("CDFeedReceiver"); }
                        Thread.Sleep(3000);
                    }
                    else
                    {
                        CloseComponentexe("CD FeedReceiver");
                        AddToList("Can't open CD feed receiver. Please check log.", true);
                        IsWorking = false;
                        btn_RestartAuto.Enabled = true;
                        SentMail("CD Feed Receiver has failed to launch.");
                        if (list_ComponentStarted.Contains("CDFeedReceiver"))
                         list_ComponentStarted.Remove("CDFeedReceiver"); 
                        
                    }
                }
            }
            catch (Exception ee)
            {
                _logger.Error(ee, "CD Feed Receiver");
                AddToList("Can't open CD feed receiver. Please check log", true);
                IsWorking = false;
                btn_RestartAuto.Enabled = true;
                if (list_ComponentStarted.Contains("CDFeedReceiver")) { list_ComponentStarted.Remove("CDFeedReceiver"); }
                SentMail("CD Feed Receiver has failed to launch.");
            }

        }

        private void StartBSEFOFeedReceiver(string[] BseFOFeedHandlerPath)
        {
            // CD Feed Receiver   
            try
            {
                CloseComponentexe("BSEFO Feed");

                foreach (var cdpath in BseFOFeedHandlerPath)
                {
                    OpenComponentexe("BSEFO FeedReceiver", cdpath);

                    //bool connected = GatewayEngineConnector.ConnectComponents("BSEFO");

                    if (true)
                    {
                        AddToList($"BSEFO Feed Receiver Started ({cdpath}).");
                        if (!list_ComponentStarted.Contains("BSEFOFeedReceiver")) { list_ComponentStarted.Add("BSEFOFeedReceiver"); }
                        Thread.Sleep(3000);
                    }
                    //else
                    //{
                    //    CloseComponentexe("CD FeedReceiver");
                    //    AddToList("Can't open CD feed receiver. Please check log.", true);
                    //    IsWorking = false;
                    //    btn_RestartAuto.Enabled = true;
                    //    SentMail("CD Feed Receiver has failed to launch.");
                    //    if (list_ComponentStarted.Contains("CDFeedReceiver"))
                    //        list_ComponentStarted.Remove("CDFeedReceiver");

                    //}
                }
            }
            catch (Exception ee)
            {
                _logger.Error(ee, "BSEFO Feed Receiver");
                AddToList("Can't open BSEFO feed receiver. Please check log", true);
                IsWorking = false;
                btn_RestartAuto.Enabled = true;
                if (list_ComponentStarted.Contains("BSEFOFeedReceiver")) { list_ComponentStarted.Remove("BSEFOFeedReceiver"); }
                SentMail("BSEFO Feed Receiver has failed to launch.");
            }

        }


        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void StartCMNotisApi(string[] NOTISEQPath)
        {            
            // NOTIS EQ Receiver
            try
            {
                CloseComponentexe("NOTIS API EQ Manager");

                foreach (var path in NOTISEQPath)
                {
                    OpenComponentexe("NOTIS EQ Receiver", path);
                    bool notiscdstarted = GatewayEngineConnector.ConnectComponents("NOTISEQ");
                    if (notiscdstarted)
                    {
                        AddToList("NOTIS EQ Started");
                        Thread.Sleep(5000);
                        if (!list_ComponentStarted.Contains("NOTISEQReceiver"))
                            list_ComponentStarted.Add("NOTISEQReceiver");
                    }
                    else
                    {
                        CloseComponentexe("NOTIS API EQ Manager");
                        AddToList("NOTIS EQ has failed to launch. Please check the log", true);
                        IsWorking = false;
                        btn_RestartAuto.Enabled = true;
                        btn_Settings.Enabled = true;
                        if (list_ComponentStarted.Contains("NOTISEQReceiver"))
                            list_ComponentStarted.Remove("NOTISEQReceiver");
                        SentMail("NOTIS EQ has failed to launch.");

                    }
                }
            }
            catch (Exception error)
            {
                AddToList("NOTIS EQ has failed to launch. Please check the log", true);
                _logger.Error(error);
                IsWorking = false;
                btn_RestartAuto.Enabled = true;
                btn_Settings.Enabled = true;
                if (list_ComponentStarted.Contains("NOTISEQReceiver"))
                    list_ComponentStarted.Remove("NOTISEQReceiver");
                SentMail("NOTIS EQ has failed to launch.");
            }

        }

        private void StartFONotisApi(string[] NOTISFOPath)
        {
            // NOTIS FO Receiver
            try
            {
                CloseComponentexe("NOTIS API FO Manager");

                foreach (var path in NOTISFOPath)
                {
                    OpenComponentexe("NOTIS FO Receiver", path);
                    bool notiscdstarted = GatewayEngineConnector.ConnectComponents("NOTISFO");
                    if (notiscdstarted)
                    {
                        AddToList("NOTIS FO Started");
                        Thread.Sleep(5000);
                        if (!list_ComponentStarted.Contains("NOTISFOReceiver"))
                            list_ComponentStarted.Add("NOTISFOReceiver");
                    }
                    else
                    {
                        CloseComponentexe("NOTIS API FO Manager");
                        AddToList("NOTIS FO has failed to launch. Please check the log", true);
                        IsWorking = false;
                        btn_RestartAuto.Enabled = true;
                        btn_Settings.Enabled = true;                        
                        if (list_ComponentStarted.Contains("NOTISFOReceiver"))
                            list_ComponentStarted.Remove("NOTISFOReceiver");
                        SentMail("NOTIS FO has failed to launch.");
                    }
                }
            }
            catch (Exception error)
            {
                _logger.Error(error);
                AddToList("NOTIS FO has failed to launch. Please check the log", true);
                IsWorking = false;
                btn_RestartAuto.Enabled = true;
                btn_Settings.Enabled = true;
                if (list_ComponentStarted.Contains("NOTISFOReceiver"))
                    list_ComponentStarted.Remove("NOTISFOReceiver");
                SentMail("NOTIS FO has failed to launch.");
            }
        }

        private void StartCDNotisApi(string[] NOTISCDPath)
        {

            try
            {
                CloseComponentexe("NOTIS API CD Manager");

                foreach (var path in NOTISCDPath)
                {
                    OpenComponentexe("NOTIS CD Receiver", path);
                    bool notiscdstarted = GatewayEngineConnector.ConnectComponents("NOTISCD");
                    if (notiscdstarted)
                    {
                        AddToList("NOTIS CD Started");
                        Thread.Sleep(5000);
                        if (!list_ComponentStarted.Contains("NOTISCDReceiver"))
                            list_ComponentStarted.Add("NOTISCDReceiver");
                    }
                    else
                    {
                        CloseComponentexe("NOTIS API CD Manager");
                        AddToList("NOTIS CD has failed to launch. Please check the log", true);
                        IsWorking = false;
                        btn_RestartAuto.Enabled = true;
                        btn_Settings.Enabled = true;                        
                        if (list_ComponentStarted.Contains("NOTISCDReceiver"))
                            list_ComponentStarted.Remove("NOTISCDReceiver");
                        SentMail("NOTIS CD has failed to launch.");
                    }
                }

            }
            catch (Exception error)
            {
                _logger.Error(error);
                AddToList("NOTIS CD has failed to launch. Please check the log", true);
                IsWorking = false;
                btn_RestartAuto.Enabled = true;
                btn_Settings.Enabled = true;
                if (list_ComponentStarted.Contains("NOTISCDReceiver"))
                    list_ComponentStarted.Remove("NOTISCDReceiver");
                SentMail("NOTIS CD has failed to launch.");
            }
        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void InsertTokensIntoDB()
        {
            AddToList("Upload Token Started");
            bool CMTokenUploaded = false; bool FOTokenUploaded = false;

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
                        list_ComponentStarted.Add("CMUploadToken");
                        CMTokenUploaded = true;
                    }
                    else
                    {
                        AddToList("Security file not found", true);
                        CMTokenUploaded = false;
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error(ee , " : InsertTokensIntoDB CM");

                    AddToList("CM Tokens Upload failed. Please check logs for more details.", true);

                    IsWorking = false;
                    btn_RestartAuto.Enabled = true;
                    btn_Settings.Enabled = true;
                    CMTokenUploaded = false;

                    if (list_ComponentStarted.Contains("CMUploadToken"))
                        list_ComponentStarted.Remove("CMUploadToken");
                    if (list_ComponentStarted.Contains("FOUploadToken"))
                        list_ComponentStarted.Remove("FOUploadToken");

                    SentMail("CM Tokens");

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
                        var FutContracts = list_Contract.Where(v => v.ScripType == en_ScripType.XX).ToList();

                        foreach (var _Contract in list_Contract)
                        {
                            var USegment = "NSEFO";
                            var UnderlyingToken = -1;

                            if (_Contract.ScripType == en_ScripType.XX)
                                UnderlyingToken = _Contract.Token;
                            else
                            {
                                var temp = FutContracts.Where(v => v.Symbol == _Contract.Symbol && v.Expiry.Month == _Contract.Expiry.Month && v.Expiry.Year == _Contract.Expiry.Year).FirstOrDefault();
                                if (temp != null)
                                    UnderlyingToken = temp.Token;
                                else
                                {
                                    //var twmp = list_EQSecurity.Where(v => v.Symbol == _Contract.Symbol).FirstOrDefault();
                                    //if (twmp != null)
                                    //{
                                    //    UnderlyingToken = twmp.Token;
                                    //    USegment = "NSECM";
                                    //}

                                    var temp1 = FutContracts.Where(v => v.Symbol == _Contract.Symbol).OrderByDescending(v => v.Expiry).FirstOrDefault();

                                    if (temp1 != null)
                                        UnderlyingToken = temp1.Token;
                                     
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
                    
                        list_ComponentStarted.Add("FOUploadToken");
                        FOTokenUploaded = true;
                    }
                    else
                    {
                        AddToList("Contract file not found", true);
                        FOTokenUploaded = false;
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error(ee , " : InsertTokensIntoDB FO");

                    AddToList("FO Tokens Upload failed. Please check logs for more details.", true);

                    IsWorking = false;
                    btn_RestartAuto.Enabled = true;
                    btn_Settings.Enabled = true;
                    FOTokenUploaded = false;

                    if (list_ComponentStarted.Contains("CMUploadToken"))
                        list_ComponentStarted.Remove("CMUploadToken");
                    if (list_ComponentStarted.Contains("FOUploadToken"))
                        list_ComponentStarted.Remove("FOUploadToken");


                    SentMail("FO Tokens");
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
                        var FutContracts = list_CDContract.Where(v => v.ScripType == en_ScripType.XX).ToList();

                        foreach (var _CDContract in list_CDContract)
                        {
                            var UnderlyingToken = -1;
                            if (_CDContract.ScripType == en_ScripType.XX)
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
                    _logger.Error(ee, " : InsertTokensIntoDB CD");

                    AddToList("CD Tokens Upload failed. Please check logs for more details.", true);
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
                    _logger.Error(ee , " : InsertTokensIntoDB BSECM");

                    AddToList("BSECM Tokens Upload failed. Please check logs for more details.", true);
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
                        var list_Security = Exchange.ReadBSEFOContract(BSEFOSecurity[0].FullName).Where(x => x.Expiry >= DateTime.Today).ToList();
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
                    _logger.Error(ee, " : InsertTokensIntoDB BSEFO");

                    AddToList("BSEFO Tokens Upload failed. Please check logs for more details.", true);
                }

                #endregion


                if (CMTokenUploaded == true && FOTokenUploaded == true)
                {
                    AddToList("Upload Token completed successfully");
                }
                else
                {
                    IsWorking = false;
                    btn_RestartAuto.Enabled = true;
                    AddToList("Upload Token unsuccessful", true);
                    SentMail("Upload Tokens");
                    if (list_ComponentStarted.Contains("FOUploadToken"))
                        list_ComponentStarted.Remove("FOUploadToken");
                    if (list_ComponentStarted.Contains("CMUploadToken"))
                        list_ComponentStarted.Remove("CMUploadToken");
                }
            }
            catch (Exception ee) { _logger.Error(ee); IsWorking = false; }
        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void ReadContractMaster()
        {
            try
            {
                var dt_ContractMaster = new ds_Engine.dt_ContractMasterDataTable();
                using (MySqlConnection _mySqlConnection = new MySqlConnection(_MySQLCon))
                {
                    _mySqlConnection.Open();

                    using (MySqlCommand myCmdEod = new MySqlCommand("sp_GetContractMaster", _mySqlConnection))
                    {
                        myCmdEod.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataAdapter dadapter = new MySqlDataAdapter(myCmdEod))
                        {
                            //changed on 13JAN2021 by Amey
                            dadapter.Fill(dt_ContractMaster);

                            dadapter.Dispose();
                        }
                    }

                    _mySqlConnection.Close();
                }

                ContractMaster ScripInfo = null;
                foreach (ds_Engine.dt_ContractMasterRow v in dt_ContractMaster.Rows)
                {
                    ScripInfo = new ContractMaster()
                    {
                        Token = v.Token,
                        Series = v.Series,
                        Symbol = v.Symbol,
                        InstrumentName = v.InstrumentName == "EQ" ? en_InstrumentName.EQ : (v.InstrumentName == "FUTIDX" ? en_InstrumentName.FUTIDX :
                        (v.InstrumentName == "FUTSTK" ? en_InstrumentName.FUTSTK : (v.InstrumentName == "OPTIDX" ? en_InstrumentName.OPTIDX : en_InstrumentName.OPTSTK))),
                        Segment = v.Segment == "NSECM" ? en_Segment.NSECM : (v.Segment == "NSECD" ? en_Segment.NSECD :
                        (v.Segment == "NSEFO" ? en_Segment.NSEFO : (v.Segment == "BSEFO" ? en_Segment.BSEFO : en_Segment.BSECM))),
                        ScripName = v.ScripName,
                        CustomScripName = v.CustomScripName,
                        ScripType = (v.ScripType == "EQ" ? n.Structs.en_ScripType.EQ : (v.ScripType == "XX" ? n.Structs.en_ScripType.XX : (v.ScripType == "CE" ? n.Structs.en_ScripType.CE :
                                    n.Structs.en_ScripType.PE))),
                        ExpiryUnix = v.ExpiryUnix,
                        StrikePrice = v.StrikePrice,
                        LotSize = v.LotSize,
                        UnderlyingToken = v.UnderlyingToken,
                        UnderlyingSegment = v.UnderlyingSegment == "NSECM" ? en_Segment.NSECM : (v.UnderlyingSegment == "NSECD" ? en_Segment.NSECD :
                        (v.UnderlyingSegment == "NSEFO" ? en_Segment.NSEFO : (v.Segment == "BSEFO" ? en_Segment.BSEFO : en_Segment.BSECM)))
                    };

                    dict_ScripInfo.TryAdd($"{ScripInfo.Segment}|{ScripInfo.ScripName}", ScripInfo);
                    dict_CustomScripInfo.TryAdd($"{ScripInfo.Segment}|{ScripInfo.CustomScripName}", ScripInfo);
                    dict_TokenScripInfo.TryAdd($"{ScripInfo.Segment}|{ScripInfo.Token}", ScripInfo);

                   
                }

                list_ComponentStarted.Add("ReadContractMaster");
            }
            catch (Exception ee) { _logger.Error(ee, "ReadContractMaster : "); }
        }

        private void UploadClientMaster(string uploadtype, string UploadPath)
        {
            try
            {
                StringBuilder sCommand = new StringBuilder("INSERT INTO tbl_clientdetail(ClientID,DealerID,UserID,Username,Name,Margin,Adhoc,Zone,Branch,Family,Product) values(");
                int _rowsAffected = 0; DateTime dt_StartTime = DateTime.Now;//02-01-2020
                using (FileStream stream = File.Open(UploadPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line1;
                        StringBuilder detail = new StringBuilder();
                        string[] uploadClients;
                        List<StringBuilder> Rows = new List<StringBuilder>();
                        while ((line1 = reader.ReadLine()) != null)
                        {
                            uploadClients = line1.Split(',');
                            if (uploadClients.Length >= 11)   //17-11-17
                            {
                                //Changed by Akshay on 08 - 01 - 2021 For new Columns
                                if ((uploadClients[0].Trim() == "" && uploadClients[1].Trim() == "" && uploadClients[2].Trim() == "") || uploadClients[3].Trim() == "" || uploadClients[4].Trim() == "" || uploadClients[5].Trim() == "" || uploadClients[6].Trim() == "" || uploadClients[7].Trim() == "" || uploadClients[8].Trim() == "" || uploadClients[9].Trim() == "" || uploadClients[10].Trim() == "")
                                {
                                    //added logs on 03MAY2021 by Amey
                                    _logger.Error(null,"Client Upload File Empty : " + line1);
                                    AddToList("Client Upload File Empty : " + line1, true);
                                    IsWorking = false;
                                    btn_RestartAuto.Enabled = true;
                                    btn_Settings.Enabled = true;
                                    SentMail("Client File upload failed");
                                    return;
                                }
                                _rowsAffected++;//02-01-2020

                                detail.Append("'" + uploadClients[0].ToUpper().Trim() + "',");   //ClientID
                                detail.Append("'" + uploadClients[1].ToUpper().Trim() + "',");   //DealerID
                                detail.Append("'" + uploadClients[2].ToUpper().Trim() + "',");   //UserID
                                detail.Append("'" + uploadClients[3].ToUpper().Trim() + "',");   //UserName
                                detail.Append("'" + uploadClients[4].ToUpper().Trim() + "',");   //Name

                                //added on 31DEC2020 by Amey
                                try
                                {
                                    decimal Margin = Convert.ToDecimal(uploadClients[5]);
                                    decimal AdHoc = Convert.ToDecimal(uploadClients[6]);

                                    if (AdHoc < 0)
                                    {
                                        _logger.Error(null,"Client Upload Loop Negetive : " + line1);
                                        AddToList("Please Check Client information in the Client File",true);
                                        IsWorking = false;
                                        btn_RestartAuto.Enabled = true;
                                        btn_Settings.Enabled = true;
                                        SentMail("Client File upload failed");
                                        return;
                                    }

                                    detail.Append("" + Margin + ",");
                                    detail.Append("" + AdHoc + ",");
                                }
                                catch (Exception ee)
                                {
                                    _logger.Error(ee, Environment.NewLine + "Client Upload Loop : " + line1);
                                    AddToList("Please Check Client information in the Client Data File",true);
                                    IsWorking = false;
                                    btn_Settings.Enabled = true;
                                    btn_RestartAuto.Enabled = true;
                                    SentMail("Client File upload failed");
                                    return;
                                }

                                detail.Append("'" + uploadClients[7].ToUpper().Trim() + "',");          //Zone          
                                detail.Append("'" + uploadClients[8].ToUpper().Trim() + "',");          //Branch        
                                detail.Append("'" + uploadClients[9].ToUpper().Trim() + "',");          //Family        
                                detail.Append("'" + uploadClients[10].ToUpper().Trim() + "'");           //Product 

                                detail.Append("),(");
                            }
                            else
                            {
                                //added logs on 03MAY2021 by Amey
                                _logger.Error(null,"Client Upload Loop Incomplete : " + line1);
                                AddToList("Incomplete client data", true);
                                IsWorking = false;
                                btn_Settings.Enabled = true;
                                btn_RestartAuto.Enabled = true;
                                SentMail("Client File upload failed");
                                return;
                            }
                        }
                        try
                        {
                            if (uploadtype == "Complete")
                            {
                                AddToList("Full Client upload started");

                                //added on 29JAN2021 by Amey
                                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                                {
                                    con_MySQL.Open();
                                    using (var cmd = new MySqlCommand("TRUNCATE tbl_clientdetail ", con_MySQL))
                                    {
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();
                                    }
                                }
                            }
                            else
                                AddToList("Partial Client upload started");
                        }
                        catch (Exception trunEx)
                        {
                            _logger.Error(null,"Upload client " + trunEx.ToString());
                            AddToList("Full Client upload failed. Please check the log", true);
                            IsWorking = false;
                            btn_RestartAuto.Enabled = true;
                            btn_Settings.Enabled = true;
                            SentMail("Client File upload failed");
                        }
                        //}
                        //added on 22-12-17 to add only proper rows with proper data format
                        Rows.Add(detail);
                        if (Rows.Count != 0)
                        {
                            sCommand.Append(string.Join(",", Rows));
                            int a = sCommand.Length;
                            sCommand.Remove(a - 2, 2);
                            sCommand.Append("ON DUPLICATE KEY UPDATE `Username`= VALUES(`Username`),`Name`= VALUES(`Name`),`Margin`= VALUES(`Margin`),`Adhoc`= VALUES(`Adhoc`),`Zone`= VALUES(`Zone`),`Branch`= VALUES(`Branch`),`Family`= VALUES(`Family`),`Product`= VALUES(`Product`);");
                            //sCommand.Append(";");

                            using (var con_MySQL = new MySqlConnection(_MySQLCon))
                            {
                                con_MySQL.Open();

                                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), con_MySQL))
                                {
                                    myCmd.CommandType = CommandType.Text;
                                    myCmd.ExecuteNonQuery();
                                    myCmd.Dispose();
                                }
                            }
                        }
                    }
                }

                AddToList("Client File Uploaded Successfully. Row count " + _rowsAffected);
                _logger.Error(null, "Row count " + _rowsAffected + ", Total time taken for client upload " + (DateTime.Now - dt_StartTime));
                list_ComponentStarted.Add("ClientFileUpload");

            }
            catch(Exception ee) 
            { 
                _logger.Error(ee, "UploadClientMaster: "); 
                IsWorking = false;
                btn_RestartAuto.Enabled = true;
                btn_Settings.Enabled = true;
                SentMail("Client File upload failed");
            }
        }

        //Added by Snehadri on 
        private void AddUserandClientMapping(string UserFile)
        {
            try
            {
                DataTable dt_UserInfo = new DataTable();
                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                {
                    con_MySQL.Open();
                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetUserInfo", con_MySQL))//modified by Navin on 12-06-2019
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;

                        MySqlDataAdapter dadapt = new MySqlDataAdapter(myCmd);
                        dadapt.Fill(dt_UserInfo);
                    }
                }
                clsEncryptionDecryption.DecryptData(dt_UserInfo);


                var arr_Info = File.ReadAllLines(UserFile);

                foreach (var Info in arr_Info)
                {
                    if (Info != "")
                    {
                        var list_data = Info.Split(',').ToList();
                        var username = list_data[0].Trim();
                        var command = list_data[1].Trim().ToLower();

                        if (username == "")
                        {
                            AddToList("Invalid data in client mapping file", true);
                            IsWorking = false;
                            btn_RestartAuto.Enabled = true;
                            return;
                        }

                        List<string> list_MappedClients = new List<string>();

                        DataColumn[] columns = dt_UserInfo.Columns.Cast<DataColumn>().ToArray();
                        bool userpresent = dt_UserInfo.AsEnumerable().Any(row => columns.Any(col => row[col].ToString() == username));

                        if (userpresent)
                        {
                            DataRow user_row = dt_UserInfo.AsEnumerable().Where(row => row.Field<string>("UserName") == username).First();

                            if (command == "partial")
                            {
                                list_MappedClients = user_row[1].ToString().Split(',').ToList();
                                for (int i = 2; i < list_data.Count; i++)
                                {
                                    if (!list_MappedClients.Contains(list_data[i]))
                                        list_MappedClients.Add(list_data[i]);
                                }

                            }
                            else if (command == "delete")
                            {

                                list_MappedClients = user_row[1].ToString().Split(',').ToList();
                                for (int i = 2; i < list_data.Count; i++)
                                {
                                    if (list_MappedClients.Contains(list_data[i]))
                                        list_MappedClients.Remove(list_data[i]);
                                }

                            }
                            else if (command == "full")
                            {
                                for (int i = 2; i < list_data.Count; i++)
                                {
                                    if (!list_MappedClients.Contains(list_data[i]))
                                        list_MappedClients.Add(list_data[i]);
                                }
                            }
                            else if (command == "all")
                            {
                                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                                {
                                    con_MySQL.Open();
                                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetClientDetail", con_MySQL))
                                    {
                                        myCmd.CommandType = CommandType.StoredProcedure;

                                        myCmd.Parameters.Add("prm_Type", MySqlDbType.LongText);
                                        myCmd.Parameters["prm_Type"].Value = "ALL";

                                        using (MySqlDataReader mySqlDataReader = myCmd.ExecuteReader())
                                        {
                                            while (mySqlDataReader.Read())
                                            {
                                                list_MappedClients.Add(mySqlDataReader.GetString(3));

                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                AddToList("Invalid data in client mapping file", true);
                                IsWorking = false;
                                btn_RestartAuto.Enabled = true;
                                return;
                            }


                            StringBuilder sbMappedClient = new StringBuilder();
                            if (list_MappedClients.Count > 0 || command == "delete")
                            {
                                for (int i = 0; i < list_MappedClients.Count; i++)
                                {
                                    sbMappedClient.Append(list_MappedClients[i] + ",");
                                }
                                if (sbMappedClient.Length > 1)
                                    sbMappedClient.Remove(sbMappedClient.ToString().LastIndexOf(','), 1);

                                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                                {
                                    con_MySQL.Open();
                                    using (var cmd = new MySqlCommand("UPDATE tbl_login SET MappedClient = '" + clsEncryptionDecryption.EncryptString(sbMappedClient.ToString(), "Nerve123") + "' where UserName= '" + clsEncryptionDecryption.EncryptString(username.ToLower(), "Nerve123") + "'", con_MySQL))
                                    {
                                        int result = cmd.ExecuteNonQuery();
                                        if (result != 1)
                                        {
                                            AddToList("Client Mapping failed for User: " + username, true);
                                            IsWorking = false;
                                            btn_RestartAuto.Enabled = true;
                                            if (list_ComponentStarted.Contains("Clientmapping"))
                                                list_ComponentStarted.Remove("Clientmapping");
                                            return;
                                        }

                                    }
                                }
                            }



                        }
                        else
                        {

                            if (command == "all")
                            {
                                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                                {
                                    con_MySQL.Open();
                                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetClientDetail", con_MySQL))
                                    {
                                        myCmd.CommandType = CommandType.StoredProcedure;

                                        myCmd.Parameters.Add("prm_Type", MySqlDbType.LongText);
                                        myCmd.Parameters["prm_Type"].Value = "ALL";

                                        using (MySqlDataReader mySqlDataReader = myCmd.ExecuteReader())
                                        {
                                            while (mySqlDataReader.Read())
                                            {
                                                list_MappedClients.Add(mySqlDataReader.GetString(3));

                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {
                                for (int i = 2; i < list_data.Count; i++)
                                {
                                    list_MappedClients.Add(list_data[i]);
                                }

                            }

                            StringBuilder sbMappedClient = new StringBuilder();
                            if (list_MappedClients.Count > 0)
                            {

                                for (int i = 0; i < list_MappedClients.Count; i++)
                                {
                                    sbMappedClient.Append(list_MappedClients[i] + ",");
                                }
                                if (sbMappedClient.Length > 1)
                                    sbMappedClient.Remove(sbMappedClient.ToString().LastIndexOf(','), 1);

                                int result = 0;
                                using (var con_MySQL = new MySqlConnection(_MySQLCon))
                                {
                                    con_MySQL.Open();
                                    using (var cmd = new MySqlCommand("INSERT INTO `tbl_login` (`UserName`, `Password`, `IsAdmin`, MappedClient) VALUES ( '" + clsEncryptionDecryption.EncryptString(username.ToLower(), "Nerve123") + "','" + clsEncryptionDecryption.EncryptString("prime123", "Nerve123") + "','" + clsEncryptionDecryption.EncryptString("false", "Nerve123") + "','" + clsEncryptionDecryption.EncryptString(sbMappedClient.ToString(), "Nerve123") + "')", con_MySQL))
                                    {
                                        result = cmd.ExecuteNonQuery();
                                        if (result != 1)
                                        {
                                            AddToList("Client Mapping failed for User: " + username, true);
                                            IsWorking = false;
                                            btn_RestartAuto.Enabled = true;
                                            if (list_ComponentStarted.Contains("Clientmapping"))
                                                list_ComponentStarted.Remove("Clientmapping");
                                            return;
                                        }
                                    }
                                }
                            }


                        }
                    }

                }

                AddToList("Mapping Clients completed successfully");
                list_ComponentStarted.Add("Clientmapping");
            }
            catch (Exception ee)
            {
                _logger.Error(ee, "AddUserandClientMapping: ");
                IsWorking = false;
                btn_RestartAuto.Enabled = true;
                SentMail("Client Mapping failed to execute");
                AddToList("Mapping Clients unsuccessful", true);
                if (list_ComponentStarted.Contains("Clientmapping"))
                    list_ComponentStarted.Remove("Clientmapping");
            }
        }



        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void ClearEOD()
        {
            try
            {
                AddToList("Deleting EOD data");

                Application.DoEvents();
                using (MySqlConnection myConnClear = new MySqlConnection(_MySQLCon))
                {
                    using (MySqlCommand myCmdClear = new MySqlCommand("sp_ClearEOD", myConnClear))
                    {
                        myConnClear.Open();
                        myCmdClear.CommandType = CommandType.StoredProcedure;
                        myCmdClear.ExecuteNonQuery();
                        myConnClear.Close();
                    }
                }

                AddToList("EOD data deleted successfully.");
                list_ComponentStarted.Add("ClearEOD");

            }
            catch (Exception ee) 
            { 
                _logger.Error(ee); 
                AddToList("EOD data deletion unsuccessfull. Please check logs for more details.", true); 
                IsWorking = false;
                btn_RestartAuto.Enabled = true;
                btn_Settings.Enabled = true;
                SentMail("Clear EOD");
            }
        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void SelectClientfromDatabase()
        {
            try
            {
                using (MySqlConnection con_MySQL = new MySqlConnection(_MySQLCon))
                {
                    using (MySqlCommand myCmd = new MySqlCommand("sp_GetClientDetail", con_MySQL))
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;

                        //added on 27APR2021 by Amey
                        myCmd.Parameters.Add("prm_Type", MySqlDbType.LongText);
                        myCmd.Parameters["prm_Type"].Value = "ID";

                        con_MySQL.Open();

                        using (MySqlDataReader _mySqlDataReader = myCmd.ExecuteReader())
                        {
                            while (_mySqlDataReader.Read())
                            {
                                string ClientID = _mySqlDataReader.GetString(0).ToUpper().Trim();

                                //added on 12JAN2021 by Amey
                                hs_Usernames.Add(ClientID);
                            }
                        }

                        con_MySQL.Close();
                    }
                }
            }
            catch (Exception clientEx)
            {
                _logger.Error(null, "SelectClientfromDatabase " + clientEx);
            }
        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void InsertDay1(string filename)
        {
            try
            {
                //added Segment on 20APR2021 by Amey. To avoid same Token conflict from different segments.
                var result = list_Day1Positions.GroupBy(s => new { s.Token, s.Username, s.Segment })
                                            .Select(g => new
                                            {
                                                Username = g.Select(x => x.Username).First(),
                                                Segment = g.Select(x => x.Segment).First(),
                                                Token = g.Select(x => x.Token).First(),
                                                BEP = Math.Round(g.Sum(x => x.TradeQuantity * x.TradePrice) / (g.Sum(x => x.TradeQuantity) == 0 ? -1 : g.Sum(x => x.TradeQuantity)), 4),
                                                TradeQuantity = g.Sum(x => x.TradeQuantity),
                                                UnderlyingToken = g.Select(x => x.UnderlyingToken).First(),
                                                UnderlyingSegment = g.Select(x => x.UnderlyingSegment).First()
                                            });

                MySqlCommand cmd = new MySqlCommand();

                //changed on 07JAN2021 by Amey
                StringBuilder insertCmd = new StringBuilder("INSERT IGNORE INTO tbl_eod (Username,Segment,Token,TradePrice,TradeQuantity,UnderlyingSegment,UnderlyingToken) VALUES");

                List<string> toInsert = new List<string>();

                //changed to var on 27APR2021 by Amey
                var date_Tick = ConvertToUnixTimestamp(DateTime.Now);

                foreach (var _Item in result)
                {
                    var ScripInfo = dict_TokenScripInfo[$"{_Item.Segment}|{_Item.Token}"];
                    if ((ScripInfo.ExpiryUnix) > date_Tick || ScripInfo.ScripType == n.Structs.en_ScripType.EQ)   //09-01-18
                        toInsert.Add($"('{_Item.Username}','{ScripInfo.Segment}',{ScripInfo.Token},{_Item.BEP},{_Item.TradeQuantity},'{ScripInfo.UnderlyingSegment}',{ScripInfo.UnderlyingToken})");
                }
                try
                {
                    if (toInsert.Count > 0)
                    {
                        insertCmd.Append(string.Join(",", toInsert));
                        insertCmd.Append(";");
                        using (MySqlConnection myconnDay1 = new MySqlConnection(_MySQLCon))
                        {
                            cmd = new MySqlCommand(insertCmd.ToString(), myconnDay1);
                            myconnDay1.Open();
                            cmd.ExecuteNonQuery();
                            myconnDay1.Close();
                        }
                    }

                    AddToList($"{filename} Process Completed. {toInsert.Count} Rows added.");
                    _logger.Debug($"{filename} Process Completed. {toInsert.Count} Rows added.");

                    list_Day1Positions.Clear();
                }
                catch (Exception ee)
                {
                    _logger.Error(ee, "InsertDay1 -inner");
                    AddToList($"{filename} Process failed. Please check the log file.",true);
                }
            }
            catch (Exception ee)
            {
                _logger.Error(ee,"InsertDay1");
                AddToList($"{filename} Process failed. Please check the log file.",true);
            }
        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void Day1andPS03FileUpload(string DAY1Folder, string PS03Folder, string BhavcopyPath)
        {
            try
            {
                bool Day1_error = false; bool PS03_error = false;

                try
                {
                    SelectClientfromDatabase();

                    var Day1Directory = new DirectoryInfo(DAY1Folder);

                    var Day1File = Day1Directory.GetFiles()
                                                  .OrderByDescending(f => f.LastWriteTime)
                                                  .First();
                    AddToList("Day1 Process started.");

                    //Seperated class for reading Day1 Positions for better track code updates of various Prime versions. 09MAR2021-Amey
                    list_Day1Positions = Day1.Read(DAY1Folder, BhavcopyPath, _logger, true, true, hs_Usernames, dict_ScripInfo, dict_CustomScripInfo, dict_TokenScripInfo);

                    //addded on 09APR2021 by Amey
                    if (Day1.isAnyError)
                    {
                        AddToList("Invalid data found in Day1 file. Please check if positions are uploaded properly.", true);
                        Day1_error = true;

                    }
                    else
                    {
                        InsertDay1("Day1");
                        Day1_error = false;
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error(ee, "Day1 File Upload Error.");
                    Day1_error = true;
                }


                try
                {
                    var PS03directory = new DirectoryInfo(PS03Folder);
                    var arr_PS03 = PS03directory.GetFiles().OrderByDescending(f => f.LastWriteTime).ToArray();
                    AddToList("PS03 Process started.");

                    foreach (var _File in arr_PS03)
                    {
                        //Seperated class for reading Day1 Positions for better track code updates of various Prime versions. 09MAR2021-Amey
                        var list_Temp = Day1.ReadPS03(_File.FullName, BhavcopyPath, _logger, true, true, hs_Usernames, dict_ScripInfo, dict_CustomScripInfo, dict_TokenScripInfo);
                        list_Day1Positions.AddRange(list_Temp);
                    }

                    //addded on 09APR2021 by Amey
                    if (Day1.isAnyError)
                    {
                        AddToList("Invalid data found in PS03 file. Please check if positions are uploaded properly.", true);
                        PS03_error = true;

                    }
                    else
                    {
                        InsertDay1("PS03");
                        PS03_error = false;
                    }
                }
                catch (Exception ee)
                {
                    _logger.Error(ee, "PS03 File Upload Error.");
                    PS03_error = true;
                }

                if (!Day1_error || !PS03_error)
                {
                    IsWorking = true;
                    list_ComponentStarted.Add("Day1PS03File");
                }
                else
                {
                    IsWorking = false;
                    btn_RestartAuto.Enabled = true;
                    btn_Settings.Enabled = true;
                    SentMail("Day1/PS03 file upload error.");
                }

            }
            catch (Exception ee)
            {
                _logger.Error(ee);
                AddToList("Error in Uploading Day1/PS03 file. Please check logs for more details.", true);
                IsWorking = false;
                btn_RestartAuto.Enabled = true;
                btn_Settings.Enabled = true;
                SentMail("Day1 or PS03 file failed to upload.");
            }

        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void StartGateway(string GatewayPath)
        {
            try
            {
                CloseComponentexe("Gateway");

                OpenComponentexe("Gateway", GatewayPath);

                bool gatewaystarted = GatewayEngineConnector.ConnectComponents("Gateway");
                if (gatewaystarted) { AddToList("Gateway Started"); list_ComponentStarted.Add("Gateway"); }
                else
                {
                    AddToList("Gateway has failed to launch, Please check the log", true);
                    IsWorking = false;
                    btn_RestartAuto.Enabled = true;
                    btn_Settings.Enabled = true;
                    SentMail("Gateway has failed to launch");
                }
            }
            catch (Exception ee)
            {
                _logger.Error(ee);
                AddToList("Gateway has failed to launch, Please check the log", true);
                IsWorking = false;
                btn_RestartAuto.Enabled = true;
                btn_Settings.Enabled = true;
                SentMail("Gateway has failed to launch");
            }

        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void StartEngine(string EnginePath)
        {
            try
            {
                CloseComponentexe("Engine");

                OpenComponentexe("Engine", EnginePath);

                Thread.Sleep(5000);         // This sleep is to allow the n.Engine to initialise 
                bool gatewaystarted = GatewayEngineConnector.ConnectComponents("Engine");
                if (gatewaystarted) { AddToList("Engine Started"); list_ComponentStarted.Add("Engine"); }
                else
                {
                    AddToList("Engine Not Started, Please check the log", true);
                    IsWorking = false;
                    btn_RestartAuto.Enabled = true;
                    btn_Settings.Enabled = true;
                    SentMail("Engine has failed to launch");
                }
            }
            catch (Exception ee)
            {
                _logger.Error(ee);
                AddToList("Engine has failed to launch, Please check the log", true);
                IsWorking = false;
                btn_RestartAuto.Enabled = true;
                btn_Settings.Enabled = true;
                SentMail("Engine has failed to launch");
            }
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void btn_Settings_Click(object sender, EventArgs e)
        {
            try
            {
                new Settings().ShowDialog();
            }
            catch(Exception ee) { _logger.Error(ee); }
        }

        private void OpenComponentexe(string componentname, string filepath)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = filepath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(filepath);
                process.Start();
            }
            catch (Exception ee) { _logger.Error(ee, $"{componentname} has failed to start."); }
        }

        private void CloseComponentexe(string componentname)
        {
            try
            {
                string[] array = componentname.Split(',');
                foreach (var item in array)
                {
                    Process[] process = Process.GetProcessesByName(item.Trim());
                    if (process.Length > 0)
                    {
                        foreach (var prog in process)
                        {
                            prog.Kill();
                        }
                        Thread.Sleep(5000);
                    }

                }


            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void StartComponents(bool IsRestart = false)            // Added by Snehadri on
        {
            try
            {
                // To avoid interference of the Engine's different socket connections 
                

                CloseComponentexe("Engine");

                if (!IsRestart)
                {
                    CloseComponentexe("CM FeedReceiver,FO FeedReceiver,CD FeedReceiver,NOTIS API EQ Manager,NOTIS API FO Manager,NOTIS API CD Manager,Gateway");

                    GatewayEngineConnector.StartServer();
                    Thread.Sleep(2000);
                }

                AddToList("Starting n.Prime Components");

                DataSet ds_SettingConfig = NerveUtils.XMLC(ApplicationPath + "config.xml");
                var dRow = ds_SettingConfig.Tables["AUTOMATICSETTINGS"].Rows[0];
                string[] CDFEEDPath = dRow["CDFEEDPATH"].STR().SPL(',');
                string[] CMFEEDPath = dRow["CMFEEDPATH"].STR().SPL(',');
                string[] FOFEEDPath = dRow["FOFEEDPATH"].STR().SPL(',');
                //string[] BSEFOFEEDHandlerPath = dRow["BSEFOFEEDPATH"].STR().SPL(',');
                //string   BSEFOFEEDPath = dRow["BSEFOFEEDPATH"].STR();
                string[] NOTISEQPath = dRow["NOTISEQPATH"].STR().SPL(',');
                string[] NOTISFOPath = dRow["NOTISFOPATH"].STR().SPL(',');
                string[] NOTISCDPath = dRow["NOTISCDPATH"].STR().SPL(',');
                string DAY1Folder = dRow["DAY1FOLDER"].STR();
                string PS03Folder = dRow["PSO3FOLDER"].STR();
                string BhavcopyPath = dRow["BHAVCOPYPATH"].STR();
                string StartApi = dRow["STARTNOTIS"].STR().ToLower();
                string StartCDComponents = dRow["START_CD_COMPONENTS"].STR().ToLower();
                string GatewayPath = dRow["GATEWAYPATH"].STR();
                string EnginePath = dRow["ENGINEPATH"].STR();
                string ClientFullUploadPath = dRow["CLIENTFULLUPLOADPATH"].STR();
                string ClientPartialUploadPath = dRow["CLIENTPARTIALUPLOADPATH"].STR();
                string UserMappingFilePath = dRow["USERMAPPINGFILEPATH"].STR();

                //IsWorking = true;
                //GatewayEngineConnector.StartServer();
                //if (IsWorking && !list_ComponentStarted.Contains("Gateway")) { StartGateway(GatewayPath); Thread.Sleep(5000); }     // To start Gateway


                if (IsWorking && !list_ComponentStarted.Contains("CMFeedReceiver"))
                {
                    StartCMFeedReceivers(CMFEEDPath);
                }

                if (IsWorking && !list_ComponentStarted.Contains("FOFeedReceiver"))
                {
                    StartFOFeedReceiver(FOFEEDPath);
                }

                if ((IsWorking && StartCDComponents == "yes") && !list_ComponentStarted.Contains("CDFeedReceiver"))
                {
                    StartCDFeedReceiver(CDFEEDPath);
                }


                if (StartApi == "yes")
                {
                    if(IsWorking && !list_ComponentStarted.Contains("NOTISEQReceiver"))
                    {
                        StartCMNotisApi(NOTISEQPath);
                    }

                    if(IsWorking && !list_ComponentStarted.Contains("NOTISFOReceiver"))
                    {
                        StartFONotisApi(NOTISFOPath);
                    }

                    if ((StartCDComponents == "yes" && IsWorking) && !list_ComponentStarted.Contains("NOTISCDReceiver"))
                    {
                        StartCDNotisApi(NOTISCDPath);
                    }
                }

                if (IsWorking && !list_ComponentStarted.Contains("CMUploadToken") && !list_ComponentStarted.Contains("FOUploadToken"))   // To start the Upload Token Procedure 
                {
                    //uncomment for prime 1
                    InsertTokensIntoDB();
                    Thread.Sleep(5000);
                }

                if (IsWorking && !list_ComponentStarted.Contains("Gateway")) { StartGateway(GatewayPath); Thread.Sleep(5000); }     // To start Gateway

                if (IsWorking && !list_ComponentStarted.Contains("ClientFileUpload"))
                {
                    //uncomment for prime1
                    try
                    {
                        var FullUpload = new DirectoryInfo(ClientFullUploadPath);
                        var FullUploadFile = FullUpload.GetFiles()
                                   .OrderByDescending(f => f.LastWriteTime)
                                   .First();

                        UploadClientMaster("Complete", FullUploadFile.FullName);
                        Thread.Sleep(2000);

                    }
                    catch (Exception ee) { }
                    try
                    {
                        var PartialUpload = new DirectoryInfo(ClientPartialUploadPath);

                        var PartialUploadFile = PartialUpload.GetFiles()
                                   .OrderByDescending(f => f.LastWriteTime)
                                   .First();

                        UploadClientMaster("Partial", PartialUploadFile.FullName);
                        Thread.Sleep(2000);
                    }
                    catch (Exception ee) { }

                }

                if (IsWorking && !list_ComponentStarted.Contains("ClientMapped"))      // To Add User and Mapp the Clients 
                {
                    try
                    {
                        //uncomment for prime1
                        var UserMapping = new DirectoryInfo(UserMappingFilePath);
                        var UserMappingFile = UserMapping.GetFiles()
                                   .OrderByDescending(f => f.LastWriteTime)
                                   .First();
                        AddUserandClientMapping(UserMappingFile.FullName);
                    }
                    catch (Exception ee) { }
                }

                //uncomment for prime1
                if (IsWorking && !list_ComponentStarted.Contains("ReadContractMaster")) { ReadContractMaster(); Thread.Sleep(5000); }     // Read the contract master from the database
                //uncomment for prime1
                if (IsWorking && !list_ComponentStarted.Contains("ClearEOD")){ ClearEOD(); Thread.Sleep(5000); }                                  // To clear EOD
                
                if (IsWorking && !list_ComponentStarted.Contains("Day1PS03File")) // To Upload Day1 and PS03 File
                {
                    //uncomment for prime1
                    Day1andPS03FileUpload(DAY1Folder, PS03Folder, BhavcopyPath);
                    Thread.Sleep(5000);
                }

                if (IsWorking && !list_ComponentStarted.Contains("Engine")) { StartEngine(EnginePath); Thread.Sleep(5000); }                                                                               // To start Engine
                

                if (IsWorking)
                {
                    AddToList("BOD process successfull");
                    SentMail(null, false);
                    btn_Settings.Enabled = true;
                    GatewayEngineConnector.CloseServer();
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void SentMail(string error_message, bool Isfault = true)
        {
            try
            {
                DataSet ds_SettingConfig = NerveUtils.XMLC(ApplicationPath + "config.xml");
                var dRow = ds_SettingConfig.Tables["AUTOMATICSETTINGS"].Rows[0];
                string sent_from = dRow["FROMEMAIL"].STR();
                string sent_to = dRow["TOEMAIL"].STR();
                string password = dRow["PASSWORD"].STR();
                string smtp = dRow["SMTP"].STR();
                string subject = "Automatic BOD process notifiation";
                string message = null;

                if (Isfault)
                {
                    message += $"Hi, \n There was a problem in starting the BOD process.\n" + error_message;
                }
                else
                {
                    message += "Hi, \n BOD process completed succefully";
                }

                //Sending Email
                SmtpClient client = new SmtpClient()
                {
                    Host = smtp,
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = sent_from,
                        Password = password,
                    }
                };
                MailAddress FromEmail = new MailAddress(sent_from);
                MailAddress ToEmail = new MailAddress(sent_to);
                MailMessage Message = new MailMessage()
                {
                    From = FromEmail,
                    Subject = subject,                    
                    Body = message,

                };
                Message.To.Add(ToEmail);

                try
                {
                    client.Send(Message);
                    AddToList("Email has been sent successfully");
                }
                catch (Exception ee) { _logger.Error(ee); AddToList("Please check the email configuration in the setting", true); }


            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - origin;
            return diff.TotalSeconds;
        }

        // Added by Snehadri on 15JUN2021 for Automatic BOD Process
        private void btn_RestartAuto_Click(object sender, EventArgs e)
        {
            try
            {
                btn_RestartAuto.Enabled = false;
                IsWorking = true;
                AddToList("Automatic BOD Process Restarted");
                StartComponents(true);

            }
            catch (Exception ee) { _logger.Error(ee); }


        }

        public void DownloadFile()
        {
            try
            {
                
                WebClient client = new WebClient();
                
                var url = "https://archives.nseindia.com/archives/nsccl/span/nsccl.20230505.s.zip";
                var path = "C:\\BOD-API\\Span1\\20230504\\TEMP\\nsccl.20230504.s.spn.gz";

                client.DownloadFile(url, path);
             
            }
            catch(Exception ee) { _logger.Error(ee); }
        }


        private void btn_DownloadSpan_Click(object sender, EventArgs e)
        {
            try
            {
                _logger.Error(null, "Download Span Button Clicked");

                object tempObj = null;
                ElapsedEventArgs tempE = null;

                var arr_SpanInfo = ds_Config.GET("SAVEPATH", "SPAN").SPL(',');

                Task.Run(() => DownloadSpan(tempObj, tempE, arr_SpanInfo));

            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void Home_Load(object sender, EventArgs e)
        {
            try
            {
                btn_DownloadSpan.Enabled = false;
                btn_RestartAuto.Enabled = false;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
    }
}
