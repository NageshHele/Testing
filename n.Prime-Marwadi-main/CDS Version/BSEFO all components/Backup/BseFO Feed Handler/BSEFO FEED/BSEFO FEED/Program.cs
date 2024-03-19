using n.LicenseValidator;
using n.LicenseValidator.Data_Structures;
using NerveLog;
using NSEUtilitaire;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        static void Main(string[] args)
        {

            Console.Title = "BSEFO Feed";

            Program _Main = new Program();

            NerveLogger _logger = new NerveLogger(true, true, false, ""); //log

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

                //_UDPSocketReceiver.StartUDPApp();

                nCalculate.Initialise();
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

                    if(item.ScripName == "SENSEX23JULFUT")
                    {

                    }
                        

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

                var arr_BSEFOSecurity = Directory.GetFiles(BSE_CONTRACTPATH, "BSE_*_CONTRACT_*.csv");
                if (arr_BSEFOSecurity.Length == 0) return;

                CommonMethods.ConsoleWrite($"Reading BSE_Contract {arr_BSEFOSecurity[0]}.");

                var list_BSE_Contract = Exchange.ReadBSEFOContract(arr_BSEFOSecurity[0]);
                foreach (var _BSE_Contract in list_BSE_Contract)
                    GlobalCollections.dict_LastPacket.TryAdd(_BSE_Contract.Token, new Packet() { LTP=(_BSE_Contract.ClosePrice*100).ToString(),Close= (_BSE_Contract.ClosePrice * 100).ToString(), Multiplier = 1 ,LTQ="1" });

              
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
