﻿using NSEUtilitaire;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using n.Structs;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Engine
{
    internal static class Day1
    {
        //added on 09APR2021 by Amey. To notify user if there is inconsistency in Day1 file.
        internal static bool isAnyError = false;

        public static List<EODPositionInfo> Read(clsWriteLog _logger, bool MarkToClosing, bool UseClosing, HashSet<string> hs_Usernames,
            ConcurrentDictionary<string, ContractMaster> dict_ScripInfo, ConcurrentDictionary<string, ContractMaster> dict_CustomScripInfo,
            ConcurrentDictionary<string, ContractMaster> dict_TokenScripInfo)
        {
            isAnyError = false;

            List<EODPositionInfo> list_Day1Positions = new List<EODPositionInfo>();
            HashSet<string> hs_Index = new HashSet<string>();   //Added by Akshay on 26-03-2021
        //C:\Github\n.Prime - Marwadi\CDS Version\Engine\Engine\UI\
            try
            {
                EODPositionInfo _EODPositionInfo;

                DateTime dte_ScripExpiry;
                double ExpiryInTicks;

                var directory = new DirectoryInfo("C:/Prime/Day1");

                //added on 30OCT2020 by Amey
                var BhavcopyDirectory = new DirectoryInfo("C:/Prime");
                var Day1Directory = new DirectoryInfo("C:/Prime/Day1");

                var FOBhavcopy = BhavcopyDirectory.GetFiles("fo*.csv")
                               .OrderByDescending(f => f.LastWriteTime)
                               .First();

                var CMBhavcopy = BhavcopyDirectory.GetFiles("cm*.csv")
                           .OrderByDescending(f => f.LastWriteTime)
                           .First();

                var CDBhavcopy = BhavcopyDirectory.GetFiles("cd*.csv").OrderByDescending(f => f.LastWriteTime).First();

                var list_FOBhavcopy = Exchange.ReadFOBhavcopy(FOBhavcopy.FullName, false);
                var list_CMBhavcopy = Exchange.ReadCMBhavcopy(CMBhavcopy.FullName, false);
                var list_CDBhavcopy = Exchange.ReadCDBhavcopy(CDBhavcopy.FullName, false);

                ContractMaster ScripInfo = new ContractMaster();
                string CustomScripNameKey = string.Empty;

                //NFO
                try
                {
                    var Day1File = Day1Directory.GetFiles("Prime*")
                                             .OrderByDescending(f => f.LastWriteTime)
                                             .First();

                    var arr_Day1 = File.ReadAllLines(Day1File.FullName);

                    string Symbol = string.Empty;
                    string ScripType = string.Empty;
                    double StrikePrice = -0;
                    string CustomScripName = string.Empty;

                    //long NetQty = 0;
                    //double NetValue = 0;
                    //double AvgPrice = 0;
                    //var Segment = n.Structs.en_Segment.NSEFO;

                    foreach (var _Line in arr_Day1)
                    {

                        long NetQty = 0;
                        double NetValue = 0;
                        double AvgPrice = 0;
                        var Segment = n.Structs.en_Segment.NSEFO;

                        var arr_Fields = _Line.Split(',').Select(v => v.Trim()).ToArray();

                        //changed to arr_Fields on 18MAR2021 by Amey
                        if (arr_Fields.Length < 63) continue;

                        try
                        {
                            //added for testing
                            //hs_Usernames.Add(arr_Fields[62].ToUpper());

                            //changed from AccountID to LoginID on 23MAR2021 by Amey
                            if (!hs_Usernames.Contains(arr_Fields[62].ToUpper())) continue;

                            arr_Fields[10] = arr_Fields[10] == "" ? (arr_Fields[8] != "" ? "FUT" : arr_Fields[7]) : arr_Fields[10];

                            ScripType = arr_Fields[10].ToUpper();

                            if (ScripType == "") continue;

                            //added on 05APR2021 by Amey. To Treat AF Series as EQ beacuse no feed and closing was coming.
                            ScripType = ScripType == "AF" ? "EQ" : ScripType;
                            //added by nikhil on 4APR2022
                            if (ScripType != "EQ" && ScripType != "CE" && ScripType != "PE" && ScripType != "FUT")
                            {
                                ScripType = "EQ";
                            }

                            if (arr_Fields[8] == "")
                                dte_ScripExpiry = Convert.ToDateTime("01JAN1980");
                            else
                                dte_ScripExpiry = Convert.ToDateTime(arr_Fields[8]);

                            ExpiryInTicks = ConvertToUnixTimestamp(dte_ScripExpiry);

                            //changed on 27MAR2021 by Amey
                            Symbol = arr_Fields[45].ToUpper();
                            if (Symbol == "") continue;

                            if (ExpiryInTicks == 0 || ScripType == "FUT")
                                StrikePrice = 0;
                            else
                                StrikePrice = Convert.ToDouble(arr_Fields[9]);

                            if (ExpiryInTicks == 0)
                                Segment = n.Structs.en_Segment.NSECM;
                            else
                                Segment = n.Structs.en_Segment.NSEFO;

                            NetQty = Convert.ToInt64(Convert.ToDouble(arr_Fields[17]));
                            //NetValue = Math.Abs(Convert.ToDouble(arr_Fields[13]) - Convert.ToDouble(arr_Fields[16]));

                            if (NetQty == 0 || (dte_ScripExpiry.Date < DateTime.Now.Date && ExpiryInTicks != 0))
                            {
                                _logger.WriteLog("Day1 Loop Qty/Expiry = 0 : " + _Line, true);
                                continue;
                            }

                            AvgPrice = Convert.ToDouble(arr_Fields[18]);

                            CustomScripName = $"{Symbol}|{dte_ScripExpiry.ToString("ddMMMyyyy").ToUpper()}|{(StrikePrice == 0 ? "0" : StrikePrice.ToString("#.00"))}|{(ScripType == "FUT" ? "XX" : ScripType)}";

                            //added on 29OCT2020 by Amey
                            if (MarkToClosing)
                            {
                                try
                                {
                                    if (ExpiryInTicks == 0)
                                    {
                                        var ClosePrice = list_CMBhavcopy.Where(v => v.CustomScripname.Equals(CustomScripName)).FirstOrDefault();
                                        if (ClosePrice is null)
                                            _logger.WriteLog($"Closing Not Found For : {CustomScripName}", true);
                                        else
                                            AvgPrice = ClosePrice.Close;
                                    }
                                    else
                                    {
                                        var ClosePrice = list_FOBhavcopy.Where(v => v.CustomScripname.Equals(CustomScripName)).FirstOrDefault();

                                        if (ClosePrice is null)
                                            _logger.WriteLog($"Closing Not Found For : {CustomScripName}", true);
                                        else if (UseClosing)
                                            AvgPrice = ClosePrice.Close;
                                        else
                                            AvgPrice = ClosePrice.SettlePrice;
                                    }
                                }
                                catch (Exception ee) { _logger.WriteLog("Day1 Loop Closing : " + _Line + Environment.NewLine + ee); }
                            }

                            //added on 20JAN2021 by Amey  //
                            if (AvgPrice == 0)
                            {
                                _logger.WriteLog("Day1 Loop Price = 0 : " + _Line, true);
                                continue;
                            }

                            //added on 20JAN2021 by Amey
                            if (dte_ScripExpiry.Date < DateTime.Now.Date && ExpiryInTicks != 0)
                                continue;

                            //added on 20APR2021 by Amey
                            CustomScripNameKey = Segment + "|" + CustomScripName;
                            if (!dict_CustomScripInfo.TryGetValue(CustomScripNameKey, out ScripInfo))
                            {
                                Segment = n.Structs.en_Segment.BSEFO;
                                CustomScripNameKey = n.Structs.en_Segment.BSEFO+"|" + CustomScripName;
                                if(!dict_CustomScripInfo.TryGetValue(CustomScripNameKey,out ScripInfo))
                                {
                                    _logger.WriteLog("ReadDAY1 Skipped :  " + CustomScripName);
                                    continue;
                                }
                            }
                               
                            _EODPositionInfo = new EODPositionInfo()
                            {
                                Username = arr_Fields[62].ToUpper(),
                                Segment = ScripInfo.Segment,
                                Token = ScripInfo.Token,
                                TradePrice = AvgPrice,
                                TradeQuantity = NetQty,
                                UnderlyingSegment = ScripInfo.UnderlyingSegment,
                                UnderlyingToken = ScripInfo.UnderlyingToken
                            };

                            list_Day1Positions.Add(_EODPositionInfo);
                        }
                        catch (Exception ee) { _logger.WriteLog("Day1 Loop : " + _Line + Environment.NewLine + ee); isAnyError = true; }
                    }
                }
                catch(Exception ee) { _logger.WriteLog(ee.ToString()); }


                //CDS
                try
                {
                    var Day1File = Day1Directory.GetFiles("CD*.csv")
                                             .OrderByDescending(f => f.LastWriteTime)
                                             .First();

                    var arr_Day1 = File.ReadAllLines(Day1File.FullName);

                    string Symbol = string.Empty;
                    string ScripType = string.Empty;
                    double StrikePrice = -0;
                    string CustomScripName = string.Empty;

                    
                    foreach (var _Line in arr_Day1)
                    {
                        long NetQty = 0;
                        double NetValue = 0;
                        double AvgPrice = 0;
                        var Segment = n.Structs.en_Segment.NSECD;

                        var arr_Fields = _Line.Split(',').Select(v => v.Trim()).ToArray();

                        //changed to arr_Fields on 18MAR2021 by Amey
                        if (arr_Fields.Length < 63) continue;

                        try
                        {
                            //added for testing
                            //hs_Usernames.Add(arr_Fields[62].ToUpper());

                            //changed from AccountID to LoginID on 23MAR2021 by Amey
                            if (!hs_Usernames.Contains(arr_Fields[62].ToUpper())) continue;
                            if (arr_Fields[8].ToUpper() == "NULL")
                                continue;

                            ScripType = arr_Fields[10].ToUpper() == "CE" ? "CE" : (arr_Fields[10].ToUpper() == "PE" ? "PE" : "FUT");
                           
                            if (arr_Fields[8] == "")
                                dte_ScripExpiry = Convert.ToDateTime("01JAN1980");
                            else
                                dte_ScripExpiry = Convert.ToDateTime(arr_Fields[8]);

                            ExpiryInTicks = ConvertToUnixTimestamp(dte_ScripExpiry);

                            //changed on 27MAR2021 by Amey
                            Symbol = arr_Fields[45].ToUpper();
                            if (Symbol == "") continue;

                            if (ExpiryInTicks == 0 || ScripType == "FUT")
                                StrikePrice = 0;
                            else
                                StrikePrice = Convert.ToDouble(arr_Fields[9]);

                            //if (ExpiryInTicks == 0)
                            //    Segment = en_Segment.NSECM;
                            //else
                            //    Segment = en_Segment.NSEFO;

                            NetQty = Convert.ToInt64(Convert.ToDouble(arr_Fields[17]));
                            //NetValue = Math.Abs(Convert.ToDouble(arr_Fields[13]) - Convert.ToDouble(arr_Fields[16]));

                            if (NetQty == 0 || (dte_ScripExpiry.Date < DateTime.Now.Date && ExpiryInTicks != 0))
                            {
                                _logger.WriteLog("Day1 Loop Qty/Expiry = 0 : " + _Line, true);
                                continue;
                            }

                            AvgPrice = Convert.ToDouble(arr_Fields[18]);

                            CustomScripName = $"{Symbol}|{dte_ScripExpiry.ToString("ddMMMyyyy").ToUpper()}|{(StrikePrice == 0 ? "0" : StrikePrice.ToString("#.0000"))}|{(ScripType == "FUT" ? "XX" : ScripType)}";

                            //added on 29OCT2020 by Amey
                            if (MarkToClosing)
                            {
                                try
                                {
                                    //if (ExpiryInTicks == 0)
                                    //{
                                    //    var ClosePrice = list_CMBhavcopy.Where(v => v.CustomScripname.Equals(CustomScripName)).FirstOrDefault();
                                    //    if (ClosePrice is null)
                                    //        _logger.WriteLog($"Closing Not Found For : {CustomScripName}", true);
                                    //    else
                                    //        AvgPrice = ClosePrice.Close;
                                    //}
                                    //else
                                    {
                                        var ClosePrice = list_CDBhavcopy.Where(v => v.CustomScripname.Equals(CustomScripName)).FirstOrDefault();

                                        if (ClosePrice is null)
                                            _logger.WriteLog($"Closing Not Found For : {CustomScripName}", true);
                                        else if (UseClosing)
                                            AvgPrice = ClosePrice.Close;
                                       
                                    }
                                }
                                catch (Exception ee) { _logger.WriteLog("Day1 Loop Closing : " + _Line + Environment.NewLine + ee); }
                            }
                        
                            //added on 20JAN2021 by Amey  //
                            if (AvgPrice == 0)
                            {
                                _logger.WriteLog("Day1 Loop Price = 0 : " + _Line, true);
                                continue;
                            }

                            //added on 20JAN2021 by Amey
                            if (dte_ScripExpiry.Date < DateTime.Now.Date && ExpiryInTicks != 0)
                                continue;

                            //added on 20APR2021 by Amey
                            CustomScripNameKey = Segment + "|" + CustomScripName;
                            if (!dict_CustomScripInfo.TryGetValue(CustomScripNameKey, out ScripInfo))
                                continue;

                            _EODPositionInfo = new EODPositionInfo()
                            {
                                Username = arr_Fields[62].ToUpper(),
                                Segment = Segment,
                                Token = ScripInfo.Token,
                                TradePrice = AvgPrice,
                                TradeQuantity = NetQty,
                                UnderlyingSegment = ScripInfo.UnderlyingSegment,
                                UnderlyingToken = ScripInfo.UnderlyingToken
                            };

                            list_Day1Positions.Add(_EODPositionInfo);
                        }
                        catch (Exception ee) { _logger.WriteLog("Day1 Loop : " + _Line + Environment.NewLine + ee); isAnyError = true; }
                    }
                }
                catch (Exception ee) { _logger.WriteLog(ee.ToString()); }

            }
            catch (Exception ee)
            {
                _logger.WriteLog("Read DAYFOCM " + ee);
                isAnyError = true;
            }

            return list_Day1Positions;
        }

        public static List<EODPositionInfo> ReadPS03(string FilePath, clsWriteLog _logger, bool MarkToClosing, bool UseClosing, HashSet<string> hs_Usernames,
            ConcurrentDictionary<string, ContractMaster> dict_ScripInfo, ConcurrentDictionary<string, ContractMaster> dict_CustomScripInfo,
            ConcurrentDictionary<string, ContractMaster> dict_TokenScripInfo)
        {
            isAnyError = false;

            List<EODPositionInfo> list_Day1Positions = new List<EODPositionInfo>();

            try
            {
                EODPositionInfo _EODPositionInfo;

                var directory = new DirectoryInfo("C:/Prime/Day1");

                //added on 30OCT2020 by Amey
                var BhavcopyDirectory = new DirectoryInfo("C:/Prime");

                var FOBhavcopy = BhavcopyDirectory.GetFiles("fo*.csv")
                           .OrderByDescending(f => f.LastWriteTime)
                           .First();

                var CMBhavcopy = BhavcopyDirectory.GetFiles("cm*.csv")
                           .OrderByDescending(f => f.LastWriteTime)
                           .First();

                var list_FOBhavcopy = Exchange.ReadFOBhavcopy(FOBhavcopy.FullName, false);
                var list_CMBhavcopy = Exchange.ReadCMBhavcopy(CMBhavcopy.FullName, false);

                using (FileStream stream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        string line1;

                        string Underlying = string.Empty;
                        double StrikePrice = 0;
                        string ScripType = string.Empty;
                        string CustomScripName = string.Empty;
                        string UnderlyingScripName = string.Empty;

                        //added on 20APR2021 by Amey
                        var CustomScripNameKey = string.Empty;
                        var Segment = n.Structs.en_Segment.NSEFO;
                        long Qty = 0;
                        double TradePrice = 0;
                        ContractMaster ScripInfo = new ContractMaster();

                        while ((line1 = sr.ReadLine()) != null)
                        {
                            string[] fields = line1.Split(',');

                            if (fields[0].Trim() != "")
                            {
                                try
                                {

                                    string Username = fields[7].Trim().ToUpper();

                                    //Changed by Snehadri on 06JAN2022
                                    if (!hs_Usernames.Contains(Username))
                                    {
                                        if(Username == "INST")
                                        {
                                            Username = fields[5].Trim().ToUpper();

                                            if (!hs_Usernames.Contains(Username))
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }

                                    //added on 9SEP2020 to avoid 0 Qty Uploads
                                    Qty = Convert.ToInt64(Convert.ToDouble(fields[30].Trim())) - Convert.ToInt64(Convert.ToDouble(fields[32].Trim()));
                                    if (Qty == 0)
                                    {
                                        _logger.WriteLog("Data Incorrect PS03 Skipped Invalid Qty : " + Qty + Environment.NewLine + line1, true);
                                        continue;
                                    }

                                    Underlying = Regex.Match(fields[14], @"^[^0-9]*").Value.Trim().ToUpper();
                                    ScripType = fields[13].Trim().ToUpper();

                                    if (ScripType == "")
                                        ScripType = "FUT";

                                    DateTime dte_ScripExpiry = DateTime.Parse(fields[11].Trim().ToUpper());

                                    if (dte_ScripExpiry.Date < DateTime.Now.Date && ScripType != "EQ")
                                    {
                                        _logger.WriteLog("Data Incorrect PS03 Skipped Expired : " + dte_ScripExpiry + "|" + ScripType + Environment.NewLine + line1, true);
                                        continue;
                                    }

                                    try { StrikePrice = Convert.ToDouble(fields[12]); } catch (Exception) { }

                                    CustomScripName = $"{Underlying}|{dte_ScripExpiry.ToString("ddMMMyyyy").ToUpper()}|{(StrikePrice == 0 ? "0" : StrikePrice.ToString("#.00"))}|{(ScripType == "FUT" ? "XX" : ScripType)}";

                                    //added on 20APR2021 by Amey
                                    CustomScripNameKey = Segment + "|" + CustomScripName;
                                    if (!dict_CustomScripInfo.TryGetValue(CustomScripNameKey, out ScripInfo))
                                        continue;

                                    try
                                    {

                                        if (ScripType == "EQ")
                                        {
                                            var ClosePrice = list_CMBhavcopy.Where(v => v.CustomScripname.Equals(CustomScripName)).FirstOrDefault();
                                            if (ClosePrice is null)
                                                _logger.WriteLog($"Closing Not Found For : {CustomScripName}", true);
                                            else
                                                TradePrice = ClosePrice.Close;
                                        }
                                        else
                                        {
                                            var ClosePrice = list_FOBhavcopy.Where(v => v.CustomScripname.Equals(CustomScripName)).FirstOrDefault();

                                            if (ClosePrice is null)
                                                _logger.WriteLog($"Closing Not Found For : {CustomScripName}", true);
                                            else if (UseClosing)
                                                TradePrice = ClosePrice.Close;
                                            else
                                                TradePrice = ClosePrice.SettlePrice;
                                        }
                                    }
                                    catch (Exception ee) { _logger.WriteLog("PS03 Loop Closing : " + line1 + Environment.NewLine + ee); }

                                    //added 20JAN2021 by Amey
                                    if (TradePrice <= 0)
                                    {
                                        _logger.WriteLog("Data Incorrect PS03 Skipped Invalid Qty/Price : " + Qty + "/" + TradePrice + Environment.NewLine + line1, true);
                                        continue;
                                    };

                                    //changed on 20APR2021 by Amey
                                    //changed on 12JAN2021 by Amey
                                    _EODPositionInfo = new EODPositionInfo()
                                    {
                                        Username = Username,
                                        Segment = Segment,
                                        Token = ScripInfo.Token,
                                        TradePrice = TradePrice,
                                        TradeQuantity = Qty,
                                        UnderlyingSegment = ScripInfo.UnderlyingSegment,
                                        UnderlyingToken = ScripInfo.UnderlyingToken
                                    };

                                    list_Day1Positions.Add(_EODPositionInfo);
                                }
                                catch (Exception ee)
                                {
                                    _logger.WriteLog("Data Incorrect PS03 : " + line1 + Environment.NewLine + ee);

                                    isAnyError = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Psex)
            {
                _logger.WriteLog("PS03 Upload " + Psex.ToString());

                isAnyError = true;
            }

            return list_Day1Positions;
        }

        //TODO: Make seperate class for such methods.
        private static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - origin;
            return diff.TotalSeconds;
        }
    }
}
