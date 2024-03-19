using n.LicenseValidator;
using n.LicenseValidator.Data_Structures;
using NerveLog;
using NSEUtilitaire;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BSEFO_Feed
{
    class Program
    {
        private static Mutex mutex = null;
        private static NerveLogger _logger;
        /// <summary>
        /// Underlying|dd-MM-yyyy (NIFTY|27-08-2020)
        /// </summary>
        private HashSet<string> hSet_MonthlyExpiry = new HashSet<string>();
        public static ConcurrentDictionary<string, string> dict_FUTExpiry = new ConcurrentDictionary<string, string>();
        public const string dateFormat = "dd-MM-yyyy";

        static void Main(string[] args)
        {

            Console.Title = "BSEFO Feed";

            Program _Main = new Program();

            _logger = new NerveLogger(true, true, false, ""); //log

            _logger.Initialize();

            #region mutex added by omkar
            //try
            //{
            //    mutex = new Mutex(true, "n.Engine", out bool isFreshInstance);
            //    if (isFreshInstance)
            //    {
            //        Application.EnableVisualStyles();
            //        Application.SetCompatibleTextRenderingDefault(false);

            //        BonusSkins.Register();
            //        SkinManager.EnableFormSkins();

            //        Application.Run(new EngineProcess());
            //    }
            //    else
            //    {
            //        XtraMessageBox.Show("n.Engine already running in background. Please close all running instances.", "Warning");
            //        Environment.Exit(0);
            //    }
            //}
            //catch (Exception ee) { XtraMessageBox.Show("Error while Initialising n.Engine. " + ee, "Error"); Environment.Exit(0); }
            #endregion

            CommonMethods.ConsoleWrite($"Initialising FeedReceiver-BSEFO.");

            CommonMethods.Initialise(_logger);

            if (_Main.LicenseValidate())
            {
                _Main.StartClosingCheck();

               _Main.ReadContract();
                //_Main.ReadBhavcopy();

                var _Server = new SocketServer();
                _Server.Setup(_logger);

                var _UDPSocketReceiver = new UDPSocketReceiver();

                //Console.ReadLine();

                _UDPSocketReceiver.StartUDPReceive();

                ///_UDPSocketReceiver.StartUDPApp();

                nCalculate.Initialise(_logger);
                Task.Run(() => nCalculate.Instance.StartSending());

                CommonMethods.ConsoleWrite($"FeedReceiver-BSEFO initialised.");
                _Main.KeepAwake();
            }

            Console.ReadLine();
        }


        #region Imp Methods

        internal void ReadBhavcopy()
        {
            try
            {
                var BHAVCOPYPATH = CommonMethods.GetFromConfig("FILE-PATH", "BHAVCOPY").ToString();



                var arr_BSEFOBhavcopy = Directory.GetFiles(BHAVCOPYPATH, "bhavcopy*.csv");
                if (arr_BSEFOBhavcopy.Length == 0) return;

                //var list_BidAskDepth = new List<double[]>();
                //for (int i = 0; i < 5; i++)
                //    list_BidAskDepth.Add(new double[4] { 0, 0, 0, 0 });

                //846336//846336
                var lst_bhavcopy = Exchange.ReadBSEFOBhavcopy(arr_BSEFOBhavcopy[0]);

                foreach (var item in lst_bhavcopy)
                {

                   
                    if (GlobalCollections.dict_tokenScripname.TryGetValue(item.ScripName, out int Token))
                    {



                        if (GlobalCollections.dict_LastPacket.TryGetValue(Token, out Packet packet))
                        {
                            packet.LTP = (item.ClosePrice * 100).ToString();
                            packet.Close = item.ClosePrice.ToString();

                        }
                        else
                        {
                            GlobalCollections.dict_LastPacket.TryAdd(Token, new Packet { LTP = item.ClosePrice.ToString(), Close = item.ClosePrice.ToString(), Multiplier = 1 });
                        }

                    }
                   
                    //GlobalCollections.dict_LastPacket = new ConcurrentDictionary<int, Packet>(Exchange.ReadBSEFOBhavcopy(arr_BSEFOBhavcopy[0]).ToDictionary(k => k.Token, v => new Packet() { LTP = v.ClosePrice.ToString(), Close = v.ClosePrice.ToString(), LTQ = v.TotalTrades, list_BidAskDepth = list_BidAskDepth });

                }

                //string key;
                //int value;

                ////SENSEX23JUN64600CE
                //foreach (var temp in GlobalCollections.dict_tokenScripname)
                //{
                //    key = temp.Key;
                //    value = temp.Value;

                //    if (key == "SENSEX23664600CE")
                //    {
                //        var Token = value;
                //    }

                //}



                //GlobalCollections.dict_LastPacket = new ConcurrentDictionary<int, Packet>((Exchange.ReadBSEFOBhavcopy(arr_BSEFOBhavcopy[0]).ToDictionary(k => k. , v => new Packet() { LTP = v.ClosePrice.ToString(), Close = v.ClosePrice.ToString(), LTQ = v.TotalTrades, list_BidAskDepth = list_BidAskDepth }));


                ///CollectionHelper.dict_SubscribedClients = new ConcurrentDictionary<int, HashSet<string>>(CollectionHelper.dict_LastPacket.ToDictionary(k => k.Key, v => new HashSet<string>()));
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void ReadContract()
        {
            try
            {
                //bool flag_ComputeIV = Convert.ToBoolean(CommonMethods.GetFromConfig("IV", "IV-ENABLE"));
                string IvPath = Convert.ToString(CommonMethods.GetFromConfig("IV", "IV-PATH"));
                var dict_AllFutExpiries = new ConcurrentDictionary<string, HashSet<DateTime>>();

                //var CONTRACTPATH = CommonMethods.GetFromConfig("FILE-PATH", "CONTRACT").ToString();

                //var arr_MCXCMSecurity = Directory.GetFiles(CONTRACTPATH, "MCX*.bcp");
                //if (arr_MCXCMSecurity.Length == 0) return;

                //CommonMethods.ConsoleWrite($"Reading Contract {arr_MCXCMSecurity[0]}.");

                //var list_Contract = Exchange.ReadMCXContract(arr_MCXCMSecurity[0]);
                //foreach (var _Contract in list_Contract)
                //    GlobalCollections.dict_LastPacket.TryAdd(_Contract.Token, new Packet() { Multiplier = _Contract.Multiplier });

                //CommonMethods.ConsoleWrite($"Contract loaded with {list_Contract.Count} items.");

                //added by Omkar
                var BSE_CONTRACTPATH = CommonMethods.GetFromConfig("FILE-PATH", "BSE-CONTRACT").ToString();

                var arr_BSEFOSecurity = new DirectoryInfo(BSE_CONTRACTPATH).GetFiles("BSE_*_CONTRACT_*.csv").OrderByDescending(x => x.LastWriteTime).ToArray();
                if (arr_BSEFOSecurity.Count() == 0) return;

                CommonMethods.ConsoleWrite($"Reading BSE_Contract {arr_BSEFOSecurity[0].FullName}.");

                var list_BSE_Contract = Exchange.ReadBSEFOContract(arr_BSEFOSecurity[0].FullName);
                foreach (var _BSE_Contract in list_BSE_Contract)
                {
                    string key = _BSE_Contract.Symbol + "_" + _BSE_Contract.Expiry.ToString("dd-MMM-yyyy") + "_" + _BSE_Contract.StrikePrice*100 + "_" + _BSE_Contract.ScripType;

                    GlobalCollections.dict_LastPacket.TryAdd(_BSE_Contract.Token, new Packet() { LTP = (_BSE_Contract.ClosePrice * 100).ToString(), Close = (_BSE_Contract.ClosePrice * 100).ToString(), Multiplier = 1, LTQ = "1" });
                    nCalculate.dict_ScripName.TryAdd(key, new string[3] { "0",( _BSE_Contract.ClosePrice * 100 ).ToString(), "0" });


                    if (_BSE_Contract.ScripType == en_ScripType.XX)
                    {
                        if (dict_AllFutExpiries.ContainsKey(_BSE_Contract.Symbol))
                            dict_AllFutExpiries[_BSE_Contract.Symbol].Add(_BSE_Contract.Expiry);
                        else
                            dict_AllFutExpiries.TryAdd(_BSE_Contract.Symbol, new HashSet<DateTime>() { _BSE_Contract.Expiry });

                       
                        hSet_MonthlyExpiry.Add(_BSE_Contract.Symbol + "|" + _BSE_Contract.Expiry.ToString("dd-MM-yyyy"));

                        if (!nCalculate.dict_ScripsXX.ContainsKey(_BSE_Contract.Token.ToString()))
                        {
                            string item = $"{_BSE_Contract.Symbol}|{_BSE_Contract.Expiry.Month:00}-{_BSE_Contract.Expiry.Year:0000}";
                            nCalculate.dict_ScripsXX.TryAdd(_BSE_Contract.Token.ToString(), item);

                            nCalculate.dict_xxLTP.TryAdd(item, Math.Round( Convert.ToInt32(nCalculate.dict_ScripName[key][1]) / 100.00, 2));
                        }

                        if (nCalculate.flag_ComputeIV)
                        {
                            dict_FUTExpiry.TryAdd($"{_BSE_Contract.Symbol}|{_BSE_Contract.Expiry.Month:00}-{_BSE_Contract.Expiry.Year:0000}`", $"{_BSE_Contract.Expiry.Day:00}-{_BSE_Contract.Expiry.Month:00}-{_BSE_Contract.Expiry.Year:0000}");
                        }
                    }
                }


                if (nCalculate.flag_ComputeIV)
                {
                    if (File.Exists(IvPath))
                    {
                        string[] fileLines = File.ReadAllLines(IvPath);
                        foreach (string line in fileLines)
                        {
                            try
                            {
                                string[] data = line.Split(',');
                                nCalculate.dict_DefaultIV.AddOrUpdate(data[0], Convert.ToDouble(data[1]), (oldKey, oldValue) => oldValue);
                            }
                            catch(Exception ee) { _logger.Error(ee); }
                          
                        }
                    }

                    foreach(var Bse_Contract in list_BSE_Contract)
                    {
                        string MonthYear = Bse_Contract.Expiry.Month.ToString("00") + "-" + Bse_Contract.Expiry.Year.ToString("0000");
                        string Underlying = Bse_Contract.Symbol.ToUpper();
                        string FUTKey = Underlying + "|" + MonthYear;
                        string CustomDateTime = Bse_Contract.Expiry.Day.ToString("00") + "-" + Bse_Contract.Expiry.Month.ToString("00") + "-" + Bse_Contract.Expiry.Year.ToString("0000");
                        
                        if (Bse_Contract.ScripType == en_ScripType.CE || Bse_Contract.ScripType == en_ScripType.PE)
                        {
                            DateTime Expiry = DateTime.ParseExact(Bse_Contract.Expiry.ToString(dateFormat), dateFormat, CultureInfo.InvariantCulture);
                            string key = Bse_Contract.Symbol + "_" + Bse_Contract.Expiry.ToString("dd-MMM-yyyy") + "_" + Bse_Contract.StrikePrice * 100 + "_" + Bse_Contract.ScripType.ToString();

                            var _ClosestMonthExpiry = dict_AllFutExpiries[Bse_Contract.Symbol].OrderBy(v => v).First();
                            var dtCurrentMonthExpiry = dict_AllFutExpiries[Bse_Contract.Symbol].Where(v => v.Month == _ClosestMonthExpiry.Month && v.Year == _ClosestMonthExpiry.Year).OrderByDescending(v => v).First();

                            if (hSet_MonthlyExpiry.Contains(Bse_Contract.Symbol + "|" + Bse_Contract.Expiry.ToString("dd-MM-yyyy")))
                            {
                                //MonthlyExpiry
                                if (dict_FUTExpiry.ContainsKey(FUTKey))
                                {
                                    if (Bse_Contract.ScripType == en_ScripType.CE)
                                    {
                                        nCalculate.dict_MonthlyCE.TryAdd(Bse_Contract.Token.ToString(), (Underlying + "|" + CustomDateTime + "_" + Bse_Contract.StrikePrice));

                                        if (nCalculate.dict_xxLTP.ContainsKey(FUTKey) && nCalculate.dict_ScripName.ContainsKey(key))
                                            nCalculate.CalculateCallGreeks(Bse_Contract.Token.ToString(), Underlying, Convert.ToDouble(nCalculate.dict_ScripName[key][1]) / 100.0f, nCalculate.dict_xxLTP[FUTKey], Bse_Contract.StrikePrice, Expiry);
                                    
                                    }
                                    else if (Bse_Contract.ScripType==en_ScripType.PE)
                                    {
                                        nCalculate.dict_MonthlyPE.TryAdd(Bse_Contract.Token.ToString(), (Underlying + "|" + CustomDateTime + "_" + Bse_Contract.StrikePrice));

                                        if (nCalculate.dict_xxLTP.ContainsKey(FUTKey) && nCalculate.dict_ScripName.ContainsKey(key))
                                            nCalculate.CalculatePutGreeks(Bse_Contract.Token.ToString(), Underlying, Convert.ToDouble(nCalculate.dict_ScripName[key][1]) / 100.0f, nCalculate.dict_xxLTP[FUTKey], Bse_Contract.StrikePrice, Expiry);
                                    }
                                }
                                else
                                {
                                    //LongDatedContracts
                                    if (Bse_Contract.ScripType== en_ScripType.CE)
                                    {
                                        nCalculate.dict_LDOCE.TryAdd(Bse_Contract.Token.ToString(), (Underlying + "|" + CustomDateTime + "_" + Bse_Contract.StrikePrice + "_" + dtCurrentMonthExpiry.Day.ToString("00") + "-" + dtCurrentMonthExpiry.Month.ToString("00") + "-" + dtCurrentMonthExpiry.Year.ToString("0000")));

                                        DateTime TokenExpiry = Expiry;
                                        DateTime MonthExpiry = DateTime.ParseExact(dtCurrentMonthExpiry.ToString(dateFormat),dateFormat, CultureInfo.InvariantCulture);

                                        MonthYear = MonthExpiry.Month.ToString("00") + "-" + MonthExpiry.Year.ToString("0000");
                                        FUTKey = Underlying + "|" + MonthYear;

                                        if (nCalculate.dict_xxLTP.ContainsKey(FUTKey) && nCalculate.dict_ScripName.ContainsKey(key))
                                        {
                                            double UnderlyingPrice = nCalculate.dict_xxLTP[FUTKey];
                                            double adjustedUnderlyingLTP = (UnderlyingPrice + ((UnderlyingPrice * (nCalculate.IVNOFUTINTEREST / 100) * (TokenExpiry - MonthExpiry).TotalDays) / 365));

                                            nCalculate.CalculateCallGreeks(Bse_Contract.Token.ToString(), Underlying, Convert.ToDouble(nCalculate.dict_ScripName[key][1]) / 100.0f, adjustedUnderlyingLTP, Bse_Contract.StrikePrice, TokenExpiry);
                                        }
                                    }
                                    else if (Bse_Contract.ScripType == en_ScripType.PE)
                                    {
                                        nCalculate.dict_LDOPE.TryAdd(Bse_Contract.Token.ToString(), (Underlying + "|" + CustomDateTime + "_" + Bse_Contract.StrikePrice + "_" + dtCurrentMonthExpiry.Day.ToString("00") + "-" + dtCurrentMonthExpiry.Month.ToString("00") + "-" + dtCurrentMonthExpiry.Year.ToString("0000")));

                                        DateTime TokenExpiry = Expiry;
                                        DateTime MonthExpiry = DateTime.ParseExact(dtCurrentMonthExpiry.ToString(dateFormat), dateFormat, CultureInfo.InvariantCulture);

                                        MonthYear = MonthExpiry.Month.ToString("00") + "-" + MonthExpiry.Year.ToString("0000");
                                        FUTKey = Underlying + "|" + MonthYear;

                                        if (nCalculate.dict_xxLTP.ContainsKey(FUTKey) && nCalculate.dict_ScripName.ContainsKey(key))
                                        {
                                            double UnderlyingPrice = nCalculate.dict_xxLTP[FUTKey];
                                            double adjustedUnderlyingLTP = (UnderlyingPrice + ((UnderlyingPrice * (nCalculate.IVNOFUTINTEREST / 100) * (TokenExpiry - MonthExpiry).TotalDays) / 365));

                                            nCalculate.CalculatePutGreeks(Bse_Contract.Token.ToString(), Underlying, Convert.ToDouble(nCalculate.dict_ScripName[key][1]) / 100.0f, adjustedUnderlyingLTP, Bse_Contract.StrikePrice, TokenExpiry);
                                        }
                                    }
                                }
                            }
                            else if (!hSet_MonthlyExpiry.Contains(Underlying + "|" + Bse_Contract.Expiry.ToString("dd-MM-yyyy")))
                            {
                                //WeeklyExpiry
                                if (dict_FUTExpiry.ContainsKey(FUTKey))
                                {
                                    if (Bse_Contract.ScripType == en_ScripType.CE)
                                    {
                                        nCalculate.dict_WeeklyCE.TryAdd(Bse_Contract.Token.ToString(), (Underlying + "|" + CustomDateTime + "_" + Bse_Contract.StrikePrice + "_" + dict_FUTExpiry[FUTKey]));

                                        DateTime TokenExpiry = Expiry;
                                        DateTime MonthExpiry = DateTime.ParseExact(dict_FUTExpiry[FUTKey], "dd-MM-yyyy", CultureInfo.InvariantCulture);

                                        if (nCalculate.dict_xxLTP.ContainsKey(FUTKey) && nCalculate.dict_ScripName.ContainsKey(key))
                                        {
                                            double UnderlyingPrice = nCalculate.dict_xxLTP[FUTKey];
                                            double adjustedUnderlyingLTP = (UnderlyingPrice - ((UnderlyingPrice * (nCalculate.IVNOFUTINTEREST / 100) * (MonthExpiry - TokenExpiry).TotalDays) / 365));

                                            nCalculate.CalculateCallGreeks(Bse_Contract.Token.ToString(), Underlying, Convert.ToDouble(nCalculate.dict_ScripName[key][1]) / 100.0f, adjustedUnderlyingLTP, Bse_Contract.StrikePrice, TokenExpiry);
                                        }
                                    }
                                    else if (Bse_Contract.ScripType == en_ScripType.PE)
                                    {
                                        nCalculate.dict_WeeklyPE.TryAdd(Bse_Contract.Token.ToString(), (Underlying + "|" + CustomDateTime + "_" + Bse_Contract.StrikePrice + "_" + dict_FUTExpiry[FUTKey]));

                                        DateTime TokenExpiry = Expiry;
                                        DateTime MonthExpiry = DateTime.ParseExact(dict_FUTExpiry[FUTKey], "dd-MM-yyyy", CultureInfo.InvariantCulture);

                                        if (nCalculate.dict_xxLTP.ContainsKey(FUTKey) && nCalculate.dict_ScripName.ContainsKey(key))
                                        {
                                            double UnderlyingPrice = nCalculate.dict_xxLTP[FUTKey];
                                            double adjustedUnderlyingLTP = (UnderlyingPrice - ((UnderlyingPrice * (nCalculate.IVNOFUTINTEREST / 100) * (MonthExpiry - TokenExpiry).TotalDays) / 365));

                                            nCalculate.CalculatePutGreeks(Bse_Contract.Token.ToString(), Underlying, Convert.ToDouble(nCalculate.dict_ScripName[key][1]) / 100.0f, adjustedUnderlyingLTP, Bse_Contract.StrikePrice, TokenExpiry);
                                        }
                                    }
                                }
                                else
                                {
                                    //LongDatedContracts
                                    if (Bse_Contract.ScripType == en_ScripType.CE)
                                    {
                                        nCalculate.dict_LDOCE.TryAdd(Bse_Contract.Token.ToString(), (Underlying + "|" + CustomDateTime + "_" + Bse_Contract.StrikePrice + "_" + dtCurrentMonthExpiry.Day.ToString("00") + "-" + dtCurrentMonthExpiry.Month.ToString("00") + "-" + dtCurrentMonthExpiry.Year.ToString("0000")));

                                        DateTime TokenExpiry = Expiry;
                                        DateTime MonthExpiry = DateTime.ParseExact(dtCurrentMonthExpiry.ToString(dateFormat), dateFormat, CultureInfo.InvariantCulture);

                                        MonthYear = MonthExpiry.Month.ToString("00") + "-" + MonthExpiry.Year.ToString("0000");
                                        FUTKey = Underlying + "|" + MonthYear;

                                        if (nCalculate.dict_xxLTP.ContainsKey(FUTKey) && nCalculate.dict_ScripName.ContainsKey(key))
                                        {
                                            double UnderlyingPrice = nCalculate.dict_xxLTP[FUTKey];
                                            double adjustedUnderlyingLTP = (UnderlyingPrice + ((UnderlyingPrice * (nCalculate.IVNOFUTINTEREST / 100) * (TokenExpiry - MonthExpiry).TotalDays) / 365));

                                            nCalculate.CalculateCallGreeks(Bse_Contract.Token.ToString(), Underlying, Convert.ToDouble(nCalculate.dict_ScripName[key][1]) / 100.0f, adjustedUnderlyingLTP, Bse_Contract.StrikePrice, TokenExpiry);
                                        }
                                    }
                                    else if (Bse_Contract.ScripType == en_ScripType.PE)
                                    {
                                        nCalculate.dict_LDOPE.TryAdd(Bse_Contract.Token.ToString(), (Underlying + "|" + CustomDateTime + "_" + Bse_Contract.StrikePrice + "_" + dtCurrentMonthExpiry.Day.ToString("00") + "-" + dtCurrentMonthExpiry.Month.ToString("00") + "-" + dtCurrentMonthExpiry.Year.ToString("0000")));

                                        DateTime TokenExpiry = Expiry;
                                        DateTime MonthExpiry = DateTime.ParseExact(dtCurrentMonthExpiry.ToString(dateFormat), dateFormat, CultureInfo.InvariantCulture);

                                        MonthYear = MonthExpiry.Month.ToString("00") + "-" + MonthExpiry.Year.ToString("0000");
                                        FUTKey = Underlying + "|" + MonthYear;

                                        if (nCalculate.dict_xxLTP.ContainsKey(FUTKey) && nCalculate.dict_ScripName.ContainsKey(key))
                                        {
                                            double UnderlyingPrice = nCalculate.dict_xxLTP[FUTKey];
                                            double adjustedUnderlyingLTP = (UnderlyingPrice + ((UnderlyingPrice * (nCalculate.IVNOFUTINTEREST / 100) * (TokenExpiry - MonthExpiry).TotalDays) / 365));

                                            nCalculate.CalculatePutGreeks(Bse_Contract.Token.ToString(), Underlying, Convert.ToDouble(nCalculate.dict_ScripName[key][1]) / 100.0f, adjustedUnderlyingLTP, Bse_Contract.StrikePrice, TokenExpiry);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }

                //GlobalCollections.dict_tokenScripname = new ConcurrentDictionary<string, int>(list_BSE_Contract.ToDictionary(x => x.ScripName, y => y.Token));

                //ReadBhavcopy(GlobalCollections.dict_tokenScripname);

                CommonMethods.ConsoleWrite($"BSE_Contract loaded with {list_BSE_Contract.Count} items.");
            }
            catch (Exception ee) { }//_logger.Error(ee); }
        }


       
        #endregion

        #region Supplimentary Methods

        private void StartClosingCheck()
        {
            DateTime dt_ClosingTime = DateTime.Parse(CommonMethods.GetFromConfig("OTHER", "CLOSING-TIME").ToString());

            CommonMethods.ConsoleWrite("App will shut down at : " + dt_ClosingTime.ToLongTimeString());

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(5);

            var timer = new Timer((v) =>
            {
                if (DateTime.Now > dt_ClosingTime)
                    Environment.Exit(0);

            }, null, startTimeSpan, periodTimeSpan);
        }

        private void KeepAwake()
        {
            while (true)
                if (Console.ReadKey().Key == ConsoleKey.X)
                    CommonMethods.ConsoleWrite($"Last Tick Time {GlobalCollections.dte_LTT.ToLongTimeString()}");
        }

        private bool LicenseValidate()
        {
            bool _Result = true;

            try
            {
                var AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                var _licenseResponse = NerveLicenseValidator.Validate($@"{AppPath}\FeedReceiver.ns", "FEEDRECEIVER", out LicenseInfo _LicenseInfo);
                //_logger.Debug($" license response : {JsonConvert.SerializeObject(_LicenseInfo)}");
                ////_licenseinfo.enabledsegments.fo
                if (_licenseResponse)
                {
                    DateTime expirydate = _LicenseInfo.ExpiryDate;
                    CommonMethods.ConsoleWrite("License expires on :"+ expirydate.ToString("dd-MM-yyyy"));
                    _Result = true;
                }
                else
                {
                    CommonMethods.ConsoleWrite($"license error. {_LicenseInfo.Message} [{_LicenseInfo.Error}]");
                    Thread.Sleep(5000);
                    _Result = false;
                }

            }
            catch (Exception ee)
            {
                Console.WriteLine("License file not found. Please contact system administrator. " + ee.Message);
            }

            return _Result; 
        }

        #endregion
    }
}
