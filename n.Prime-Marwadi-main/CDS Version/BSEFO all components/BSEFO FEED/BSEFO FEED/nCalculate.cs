using NerveLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

internal class nCalculate
{
    public static nCalculate Instance { get; private set; }

    public static ConcurrentDictionary<string, double> dict_DefaultIV = new ConcurrentDictionary<string, double>();          //added on 30-3-20 by Amey
    public static ConcurrentDictionary<string, string> dict_ScripsXX = new ConcurrentDictionary<string, string>();
    public static ConcurrentDictionary<string, string> dict_FUTExpiry = new ConcurrentDictionary<string, string>();
    public static ConcurrentDictionary<string, string> dict_LDOCE = new ConcurrentDictionary<string, string>();
    public static ConcurrentDictionary<string, string> dict_LDOPE = new ConcurrentDictionary<string, string>();
    public static ConcurrentDictionary<string, string> dict_WeeklyCE = new ConcurrentDictionary<string, string>();       //added on 19-3-20 by Amey
    public static ConcurrentDictionary<string, string> dict_WeeklyPE = new ConcurrentDictionary<string, string>();       //added on 19-3-20 by Amey
    public static ConcurrentDictionary<string, string> dict_MonthlyCE = new ConcurrentDictionary<string, string>();          //added on 19-3-20 by Amey
    public static ConcurrentDictionary<string, string> dict_MonthlyPE = new ConcurrentDictionary<string, string>();          //added on 19-3-20 by Amey
    /// <summary>
    /// <para>Key => Underlying_Expiry_Strike(If FUT Then -1)_OptionType</para>
    /// <para>Value => [0] OI, [1] SettlePriceinPaise/Close, [2] Token</para>
    /// </summary>
    public static ConcurrentDictionary<string, string[]> dict_ScripName = new ConcurrentDictionary<string, string[]>();     //added on 28-11-18 by Amey


    /// <summary>
    /// <para>Key => [Underlying]|[Month-Year] (NIFTY|10-2020) </para>
    /// <para>Value => LTPinRS </para>
    /// </summary>
    public static ConcurrentDictionary<string, double> dict_xxLTP = new ConcurrentDictionary<string, double>();


    public static DataSet ds_Config = new DataSet();

    public static double IVNOFUTINTEREST = 2;
    public static int IVINTEREST = 0;
    public static int IVDIVIDEND = 0;
    public static double IVMAXVALUE = 150;
    public static double IVDEFAULTVALUE = 15;

    public static int GREEKSINTEREST = 0;
    public static int GREEKSDIVIDEND = 0;

    bool flagCloseEvents = true;
    public static bool flag_ComputeIV = false;
    public static bool flag_MARKETVOL = false;
    public static bool flag_GREEKS = false;

    public nCalculate(NerveLogger logger)
    {
        _logger = logger;
    }


    public static void Initialise(NerveLogger logger)
    {              
        if (Instance is null)
            Instance = new nCalculate(logger);
    }

    public static NerveLogger _logger;

    internal void StartSending()
    {
        DisableConsoleQuickEdit.Go();

        var stopwatch = new Stopwatch();

        while (true)
        {
            stopwatch.Start();

            try
            {
                var list_Tokens = GlobalCollections.dict_SubscribedClients.Keys.ToList();

                foreach (var _Token in list_Tokens)
                {                   
                    if (CreatePacketToClient(_Token, out byte[] arr_Buffer))
                    {
                        if (GlobalCollections.dict_SubscribedClients.TryGetValue(_Token, out List<Socket> list_Clients))
                        {
                            foreach (var soc_Client in list_Clients)
                            {
                                try
                                {
                                    soc_Client.Send(arr_Buffer, arr_Buffer.Length, SocketFlags.None);
                                }
                                catch (Exception) { GlobalCollections.dict_SubscribedClients[_Token].Remove(soc_Client); }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            stopwatch.Stop();
            var elapsed_time = stopwatch.ElapsedMilliseconds;

            stopwatch.Reset();

            int waittime = 800;
            try
            {
                waittime = waittime - Convert.ToInt32(elapsed_time);
                waittime = waittime < 0 ? 0 : waittime;
            }
            catch (OverflowException) { }
            catch (Exception) { }

            Thread.Sleep(waittime);
        }
    }

    //1103091
    public static void CalculateCallGreeks(string token, string Underlying, double LTPinRS, double UnderlyingPriceinRS, double StrikePrice, DateTime Expiry)
    {
        try
        {
            Expiry = Expiry.AddHours(Convert.ToInt32(CommonMethods.GetFromConfig("SEGMENT", "END").ToString().Split(':')[0])).AddMinutes(Convert.ToInt32(CommonMethods.GetFromConfig("SEGMENT", "END").ToString().Split(':')[1]));
            double Time = (Expiry - DateTime.Now).TotalDays;

            double IV = Math.Round(GreeksCompute.ImpliedCallVolatility(UnderlyingPriceinRS, StrikePrice, (Time / 365), IVINTEREST, LTPinRS, 0, IVDIVIDEND), 8);
            IV = CheckForNAN(IV, $"IV|{token}|{Underlying}|{LTPinRS}|{UnderlyingPriceinRS}|{StrikePrice}|{Time / 365}|{IVINTEREST}|{IVDIVIDEND}");

            if (IV < 0.1 || IV > IVMAXVALUE)
                IV = dict_DefaultIV[Underlying];

            if (GlobalCollections.dict_Greeks.ContainsKey(token))
            {
                GlobalCollections.dict_Greeks[token][0] = IV;
                GlobalCollections.dict_Greeks[token][5] = UnderlyingPriceinRS;
            }
            else
                GlobalCollections.dict_Greeks.TryAdd(token, new double[6] { IV, 0, 0, 0, 0, UnderlyingPriceinRS });

            if (flag_GREEKS)
            {
                double Delta = Math.Round(GreeksCompute.CallDelta(UnderlyingPriceinRS, StrikePrice, (Time / 365), GREEKSINTEREST, (IV / 100), GREEKSDIVIDEND), 8);
                double Gamma = Math.Round(GreeksCompute.Gamma(UnderlyingPriceinRS, StrikePrice, (Time / 365), GREEKSINTEREST, (IV / 100), GREEKSDIVIDEND), 8);
                double Theta = Math.Round(GreeksCompute.CallTheta(UnderlyingPriceinRS, StrikePrice, (Time / 365), GREEKSINTEREST, (IV / 100), GREEKSDIVIDEND), 8);
                double Vega = Math.Round(GreeksCompute.Vega(UnderlyingPriceinRS, StrikePrice, (Time / 365), GREEKSINTEREST, (IV / 100), GREEKSDIVIDEND), 8);

                GlobalCollections.dict_Greeks[token][1] = CheckForNAN(Delta, $"Delta|{token}|{Underlying}|{LTPinRS}|{UnderlyingPriceinRS}|{StrikePrice}|{Time / 365}|{GREEKSINTEREST}|{IV / 100}|{GREEKSDIVIDEND}");
                GlobalCollections.dict_Greeks[token][2] = CheckForNAN(Gamma, $"Gamma|{token}|{Underlying}|{LTPinRS}|{UnderlyingPriceinRS}|{StrikePrice}|{Time / 365}|{GREEKSINTEREST}|{IV / 100}|{GREEKSDIVIDEND}"); ;
                GlobalCollections.dict_Greeks[token][3] = CheckForNAN(Theta, $"Theta|{token}|{Underlying}|{LTPinRS}|{UnderlyingPriceinRS}|{StrikePrice}|{Time / 365}|{GREEKSINTEREST}|{IV / 100}|{GREEKSDIVIDEND}"); ;
                GlobalCollections.dict_Greeks[token][4] = CheckForNAN(Vega, $"Vega|{token}|{Underlying}|{LTPinRS}|{UnderlyingPriceinRS}|{StrikePrice}|{Time / 365}|{GREEKSINTEREST}|{IV / 100}|{GREEKSDIVIDEND}"); ;

            }
        }
        catch (Exception ee) {  CommonMethods._logger.Debug("COMPUTE CallGreek " + ee.ToString()); }
    }

    //1114294
    public static void CalculatePutGreeks(string token, string Underlying, double LTPinRS, double UnderlyingPriceinRS, double StrikePrice, DateTime Expiry)
    {
        try
        {

            Expiry = Expiry.AddHours(Convert.ToInt32(CommonMethods.GetFromConfig("SEGMENT", "END").ToString().Split(':')[0])).AddMinutes(Convert.ToInt32(CommonMethods.GetFromConfig("SEGMENT", "END").ToString().Split(':')[1]));
            double Time = (Expiry - DateTime.Now).TotalDays;

            double IV = Math.Round(GreeksCompute.ImpliedPutVolatility(UnderlyingPriceinRS, StrikePrice, (Time / 365), IVINTEREST, LTPinRS, 0, IVDIVIDEND), 8);
            IV = CheckForNAN(IV, $"IV|{token}|{Underlying}|{LTPinRS}|{UnderlyingPriceinRS}|{StrikePrice}|{Time / 365}|{IVINTEREST}|{IVDIVIDEND}");

            if (IV < 0.1 || IV > IVMAXVALUE)
                IV = dict_DefaultIV[Underlying];

            //if (token == "79063" || token == "79062" || token == "41640" || token == "41644" || token == "47515")
            //    FO.logger.WriteLog($"TOKEN:{token}|{IV},{UnderlyingPrice},{StrikePrice},{Time}({Time / 365}),{LTPinRS}");

            if (GlobalCollections.dict_Greeks.ContainsKey(token))
            {
                GlobalCollections.dict_Greeks[token][0] = IV;
                GlobalCollections.dict_Greeks[token][5] = UnderlyingPriceinRS;
            }
            else
                GlobalCollections.dict_Greeks.TryAdd(token, new double[6] { IV, 0, 0, 0, 0, UnderlyingPriceinRS });

            if (flag_GREEKS)
            {
                double Delta = Math.Round(GreeksCompute.PutDelta(UnderlyingPriceinRS, StrikePrice, (Time / 365), GREEKSINTEREST, (IV / 100), GREEKSDIVIDEND), 8);
                double Gamma = Math.Round(GreeksCompute.Gamma(UnderlyingPriceinRS, StrikePrice, (Time / 365), GREEKSINTEREST, (IV / 100), GREEKSDIVIDEND), 8);
                double Theta = Math.Round(GreeksCompute.PutTheta(UnderlyingPriceinRS, StrikePrice, (Time / 365), GREEKSINTEREST, (IV / 100), GREEKSDIVIDEND), 8);
                double Vega = Math.Round(GreeksCompute.Vega(UnderlyingPriceinRS, StrikePrice, (Time / 365), GREEKSINTEREST, (IV / 100), GREEKSDIVIDEND), 8);

                GlobalCollections.dict_Greeks[token][1] = CheckForNAN(Delta, $"Delta|{token}|{Underlying}|{LTPinRS}|{UnderlyingPriceinRS}|{StrikePrice}|{Time / 365}|{GREEKSINTEREST}|{IV / 100}|{GREEKSDIVIDEND}");
                GlobalCollections.dict_Greeks[token][2] = CheckForNAN(Gamma, $"Gamma|{token}|{Underlying}|{LTPinRS}|{UnderlyingPriceinRS}|{StrikePrice}|{Time / 365}|{GREEKSINTEREST}|{IV / 100}|{GREEKSDIVIDEND}"); ;
                GlobalCollections.dict_Greeks[token][3] = CheckForNAN(Theta, $"Theta|{token}|{Underlying}|{LTPinRS}|{UnderlyingPriceinRS}|{StrikePrice}|{Time / 365}|{GREEKSINTEREST}|{IV / 100}|{GREEKSDIVIDEND}"); ;
                GlobalCollections.dict_Greeks[token][4] = CheckForNAN(Vega, $"Vega|{token}|{Underlying}|{LTPinRS}|{UnderlyingPriceinRS}|{StrikePrice}|{Time / 365}|{GREEKSINTEREST}|{IV / 100}|{GREEKSDIVIDEND}"); ;

            }
        }
        catch (Exception ee) { CommonMethods._logger.Error(ee); }
    }


    private static double CheckForNAN(double ValueToBeChecked, string Message)
    {
        double returnVal = 0;

        try
        {
            if (Double.IsNaN(ValueToBeChecked))
            {
                CommonMethods._logger.Debug($"CheckForNAN {Message}");
            }
            else
                returnVal = ValueToBeChecked;
        }
        catch (Exception ee) { CommonMethods._logger.Error(ee); }

        return returnVal;
    }


    static class DisableConsoleQuickEdit
    {

        const uint ENABLE_QUICK_EDIT = 0x0040;

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        internal static bool Go()
        {

            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // get current console mode
            if (!GetConsoleMode(consoleHandle, out uint consoleMode))
            {
                // ERROR: Unable to get console mode.
                return false;
            }

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~ENABLE_QUICK_EDIT;

            // set the new mode
            if (!SetConsoleMode(consoleHandle, consoleMode))
            {
                // ERROR: Unable to set console mode
                return false;
            }

            return true;
        }
    }

    #region Supplimentary Methods

    public static void UpdateGreekValue(string _Token,double LTPinRS)
    {
        //IV LOGIC

        try
        {
            if (flag_ComputeIV)
            {
                try
                {
                    if (dict_ScripsXX.ContainsKey(_Token.ToString()))
                    {
                        //Server.dict_ScripsXX[token] = NIFTY|11-2019
                        //Server.dict_ScripsXX[token] = [Underlying]|[Month-Year]

                        string[] arr_Underlying = dict_ScripsXX[_Token.ToString()].Split('|');
                        if (dict_xxLTP.ContainsKey(dict_ScripsXX[_Token.ToString()]))
                            dict_xxLTP[dict_ScripsXX[_Token.ToString()]] = LTPinRS;
                        else
                            dict_xxLTP.TryAdd(dict_ScripsXX[_Token.ToString()], LTPinRS);
                    }
                    else if (dict_MonthlyCE.ContainsKey(_Token))
                    {
                        //Server.dict_MonthlyCE[token] = NIFTY|05-11-2019_30000
                        //Server.dict_MonthlyCE[token] = [Underlying]|[Day-Month-Year]_[StrikePrice]

                        string[] arr_Underlying = dict_MonthlyCE[_Token].Split('|');
                        try
                        {
                            string MonthYear = arr_Underlying[0] + "|" + (arr_Underlying[1].Split('_')[0].Split('-')[1] + "-" + arr_Underlying[1].Split('_')[0].Split('-')[2]);
                            double StrikePrice = Convert.ToDouble(arr_Underlying[1].Split('_')[1]);
                            DateTime Expiry = DateTime.ParseExact(arr_Underlying[1].Split('_')[0], "dd-MM-yyyy", CultureInfo.InvariantCulture);

                            if (dict_xxLTP.ContainsKey(MonthYear))
                                CalculateCallGreeks(_Token, arr_Underlying[0], LTPinRS, dict_xxLTP[MonthYear], StrikePrice, Expiry);
                        }
                        catch (Exception ee) { CommonMethods._logger.Error(ee); }
                    }
                    else if (dict_MonthlyPE.ContainsKey(_Token))
                    {
                        //Server.dict_MonthlyPE[token] = NIFTY|05-11-2019_30000
                        //Server.dict_MonthlyPE[token] = [Underlying]|[Day-Month-Year]_[StrikePrice]

                        string[] arr_Underlying = dict_MonthlyPE[_Token].Split('|');
                        try
                        {
                            string MonthYear = arr_Underlying[0] + "|" + (arr_Underlying[1].Split('_')[0].Split('-')[1] + "-" + arr_Underlying[1].Split('_')[0].Split('-')[2]);
                            double StrikePrice = Convert.ToDouble(arr_Underlying[1].Split('_')[1]);
                            DateTime Expiry = DateTime.ParseExact(arr_Underlying[1].Split('_')[0], "dd-MM-yyyy", CultureInfo.InvariantCulture);

                            if (dict_xxLTP.ContainsKey(MonthYear))
                                CalculatePutGreeks(_Token, arr_Underlying[0], LTPinRS, dict_xxLTP[MonthYear], StrikePrice, Expiry);
                        }
                        catch (Exception ee) { CommonMethods._logger.Error(ee); }
                    }
                    else if (dict_WeeklyCE.ContainsKey(_Token))
                    {
                        //Server.dict_WeeklyCE[token] = NIFTY|05-11-2019_30000_26-11-2019
                        //Server.dict_WeeklyCE[token] = [Underlying]|[TokenExpiry:Day-Month-Year]_[StrikePrice]_[MonthExpiry:Day-Month-Year]

                        string[] arr_Underlying = dict_WeeklyCE[_Token].Split('|');
                        try
                        {
                            string MonthYear = arr_Underlying[0] + "|" + (arr_Underlying[1].Split('_')[0].Split('-')[1] + "-" + arr_Underlying[1].Split('_')[0].Split('-')[2]);
                            double StrikePrice = Convert.ToDouble(arr_Underlying[1].Split('_')[1]);
                            DateTime TokenExpiry = DateTime.ParseExact(arr_Underlying[1].Split('_')[0], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                            DateTime MonthExpiry = DateTime.ParseExact(arr_Underlying[1].Split('_')[2], "dd-MM-yyyy", CultureInfo.InvariantCulture);

                            if (dict_xxLTP.ContainsKey(MonthYear))
                            {
                                double UnderlyingPrice = dict_xxLTP[MonthYear];
                                double adjustedUnderlyingLTP = (UnderlyingPrice - ((UnderlyingPrice * (IVNOFUTINTEREST / 100) * (MonthExpiry - TokenExpiry).TotalDays) / 365));

                                CalculateCallGreeks(_Token, arr_Underlying[0], LTPinRS, adjustedUnderlyingLTP, StrikePrice, TokenExpiry);
                            }
                        }
                        catch (Exception ee) { CommonMethods._logger.Error(ee); }
                    }
                    else if (dict_WeeklyPE.ContainsKey(_Token))
                    {
                        //Server.dict_WeeklyPE[token] = NIFTY|05-11-2019_30000_26-11-2019
                        //Server.dict_WeeklyPE[token] = [Underlying]|[TokenExpiry:Day-Month-Year]_[StrikePrice]_[MonthExpiry:Day-Month-Year]

                        string[] arr_Underlying = dict_WeeklyPE[_Token].Split('|');
                        try
                        {
                            string MonthYear = arr_Underlying[0] + "|" + (arr_Underlying[1].Split('_')[0].Split('-')[1] + "-" + arr_Underlying[1].Split('_')[0].Split('-')[2]);
                            double StrikePrice = Convert.ToDouble(arr_Underlying[1].Split('_')[1]);
                            DateTime TokenExpiry = DateTime.ParseExact(arr_Underlying[1].Split('_')[0], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                            DateTime MonthExpiry = DateTime.ParseExact(arr_Underlying[1].Split('_')[2], "dd-MM-yyyy", CultureInfo.InvariantCulture);

                            if (dict_xxLTP.ContainsKey(MonthYear))
                            {
                                double UnderlyingPrice = dict_xxLTP[MonthYear];
                                double adjustedUnderlyingLTP = (UnderlyingPrice - ((UnderlyingPrice * (IVNOFUTINTEREST / 100) * (MonthExpiry - TokenExpiry).TotalDays) / 365));

                                CalculatePutGreeks(_Token, arr_Underlying[0], LTPinRS, adjustedUnderlyingLTP, StrikePrice, TokenExpiry);
                            }
                        }
                        catch (Exception ee) { CommonMethods._logger.Error(ee); }
                    }
                    else if (dict_LDOCE.ContainsKey(_Token))
                    {
                        //Server.dict_WeeklyCE[token] = NIFTY|05-11-2019_30000_26-11-2019
                        //Server.dict_WeeklyCE[token] = [Underlying]|[TokenExpiry:Day-Month-Year]_[StrikePrice]_[CurrentMonthExpiry:Day-Month-Year]

                        string[] arr_Underlying = dict_LDOCE[_Token].Split('|');
                        try
                        {
                            string MonthYear = arr_Underlying[0] + "|" + (arr_Underlying[1].Split('_')[0].Split('-')[1] + "-" + arr_Underlying[1].Split('_')[0].Split('-')[2]);
                            double StrikePrice = Convert.ToDouble(arr_Underlying[1].Split('_')[1]);
                            DateTime TokenExpiry = DateTime.ParseExact(arr_Underlying[1].Split('_')[0], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                            DateTime MonthExpiry = DateTime.ParseExact(arr_Underlying[1].Split('_')[2], "dd-MM-yyyy", CultureInfo.InvariantCulture);

                            if (dict_xxLTP.ContainsKey(MonthYear))
                            {
                                double UnderlyingPrice = dict_xxLTP[MonthYear];
                                double adjustedUnderlyingLTP = (UnderlyingPrice + ((UnderlyingPrice * (IVNOFUTINTEREST / 100) * (TokenExpiry - MonthExpiry).TotalDays) / 365));

                                CalculateCallGreeks(_Token, arr_Underlying[0], LTPinRS, adjustedUnderlyingLTP, StrikePrice, TokenExpiry);
                            }
                        }
                        catch (Exception ee) { CommonMethods._logger.Error(ee); }
                    }
                    else if (dict_LDOPE.ContainsKey(_Token))
                    {
                        //Server.dict_WeeklyCE[token] = NIFTY|05-11-2019_30000_26-11-2019
                        //Server.dict_WeeklyCE[token] = [Underlying]|[TokenExpiry:Day-Month-Year]_[StrikePrice]_[CurrentMonthExpiry:Day-Month-Year]

                        string[] arr_Underlying = dict_LDOPE[_Token].Split('|');
                        try
                        {
                            string MonthYear = arr_Underlying[0] + "|" + (arr_Underlying[1].Split('_')[0].Split('-')[1] + "-" + arr_Underlying[1].Split('_')[0].Split('-')[2]);
                            double StrikePrice = Convert.ToDouble(arr_Underlying[1].Split('_')[1]);
                            DateTime TokenExpiry = DateTime.ParseExact(arr_Underlying[1].Split('_')[0], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                            DateTime MonthExpiry = DateTime.ParseExact(arr_Underlying[1].Split('_')[2], "dd-MM-yyyy", CultureInfo.InvariantCulture);

                            if (dict_xxLTP.ContainsKey(MonthYear))
                            {
                                double UnderlyingPrice = dict_xxLTP[MonthYear];
                                double adjustedUnderlyingLTP = (UnderlyingPrice + ((UnderlyingPrice * (IVNOFUTINTEREST / 100) * (TokenExpiry - MonthExpiry).TotalDays) / 365));

                                CalculatePutGreeks(_Token, arr_Underlying[0], LTPinRS, adjustedUnderlyingLTP, StrikePrice, TokenExpiry);
                            }
                        }
                        catch (Exception ee) { CommonMethods._logger.Error(ee); }
                    }
                }
                catch (Exception ee) { CommonMethods._logger.Error(ee); }
            }
        }
        catch(Exception ee)
        {  }
    }



    private bool CreatePacketToClient(int _Token, out byte[] arr_Buffer)
    {
        var _Result = false;
        arr_Buffer = new byte[0];

        try
        {
            //sending format -> feed- libraray 
            if (_Result = GlobalCollections.dict_LastPacket.TryGetValue(_Token, out var _Packet))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(CreateTransferString(7208)); //0
                sb.Append(CreateTransferString(_Token)); //1
                sb.Append(CreateTransferString(_Packet.LTP)); //2
                sb.Append(CreateTransferString(_Packet.LTQ));//3
                sb.Append(CreateTransferString(_Packet.LTT));//4
                sb.Append(CreateTransferString(_Packet.Open));//5
                sb.Append(CreateTransferString(_Packet.High));//6
                sb.Append(CreateTransferString(_Packet.Low));//7
                sb.Append(CreateTransferString(_Packet.Close));//8
                sb.Append(CreateTransferString(_Packet.TotalBuyQty));//9
                sb.Append(CreateTransferString(_Packet.TotalSellQty));//10
                sb.Append(CreateTransferString(_Packet.AvgPrice));//11
                sb.Append(CreateTransferString(_Packet.TotalTrades));//12
                sb.Append(CreateTransferString(_Packet.OpenInterest));//13
                sb.Append(CreateTransferString(_Packet.Multiplier));//14
                sb.Append(CreateTransferString(_Packet.list_BidAskDepth));//15

                if (flag_ComputeIV && GlobalCollections.dict_Greeks.ContainsKey(_Token.ToString()))
                {
                    sb.Append(CreateTransferString(GlobalCollections.dict_Greeks[_Token.ToString()][0]));
                    sb.Append(CreateTransferString(GlobalCollections.dict_Greeks[_Token.ToString()][5]));
                }
                else
                {
                    sb.Append(CreateTransferString("-"));
                    sb.Append(CreateTransferString("-"));
                }

                if(flag_GREEKS && GlobalCollections.dict_Greeks.ContainsKey(_Token.ToString()))
                {
                    sb.Append(CreateTransferString(GlobalCollections.dict_Greeks[_Token.ToString()][1]));
                    sb.Append(CreateTransferString(GlobalCollections.dict_Greeks[_Token.ToString()][2]));
                    sb.Append(CreateTransferString(GlobalCollections.dict_Greeks[_Token.ToString()][3]));
                    sb.Append(CreateTransferString(GlobalCollections.dict_Greeks[_Token.ToString()][4]));
                }
                else
                {
                    sb.Append(CreateTransferString("-"));
                    sb.Append(CreateTransferString("-"));
                    sb.Append(CreateTransferString("-"));
                    sb.Append(CreateTransferString("-"));
                }

                sb.Append(GlobalCollections._EOF);

                arr_Buffer = Encoding.ASCII.GetBytes(sb.ToString());
            }
        }
        catch (Exception ee) { CommonMethods._logger.Error(ee); }

        return _Result;
    }

    private string CreateTransferString(object Val) => Val + GlobalCollections._Seperator;
   
    #endregion
}