using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine
{
    public static class Addons
    {
        public static clsWriteLog _logger;

        public static Dictionary<string, double> ReadNPLFile()
        {
            var dict_NPLValues = new Dictionary<string, double>();

            try
            {
                var NPLFile = "C://Prime//Other//OptionP.csv";
                if (File.Exists(NPLFile))
                {
                    var arr_Lines = File.ReadAllLines(NPLFile);
                    foreach (var line in arr_Lines)
                    {
                        try
                        {
                            var arr_Fields = line.Split(',').Select(v => v.Trim().ToUpper()).ToArray();

                            var NPLKey = $"{arr_Fields[0]}^{arr_Fields[1]}";
                            if (dict_NPLValues.ContainsKey(NPLKey))
                                dict_NPLValues[NPLKey] = Convert.ToDouble(arr_Fields[2]);
                            else
                                dict_NPLValues.Add(NPLKey, Convert.ToDouble(arr_Fields[2]));
                        }
                        catch (Exception ee) { _logger.WriteLog("ReadNPLFile : " + line + Environment.NewLine + ee); }
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog("ReadNPLFile : " + ee); }

            return dict_NPLValues;
        }

        public static HashSet<string> ReadBanScripFile()
        {
            var hs_BannedUnderlyings = new HashSet<string>();

            try
            {
                var BanFile = $"C://Prime//Other//fo_secban_{DateTime.Now.ToString("ddMMyyyy")}.csv";
                if (File.Exists(BanFile))
                {
                    var arr_Lines = File.ReadAllLines(BanFile);

                    if (arr_Lines.Length > 1)
                    {
                        for (int i = 1; i < arr_Lines.Length; i++)
                        {
                            try
                            {
                                var arr_Fields = arr_Lines[i].Split(',').Select(v => v.Trim().ToUpper()).ToArray();
                                hs_BannedUnderlyings.Add(arr_Fields[1]);
                            }
                            catch (Exception ee) { _logger.WriteLog("ReadBanScripFile : " + arr_Lines[i] + Environment.NewLine + ee); }
                        }
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog("ReadBanScripFile : " + ee); }

            return hs_BannedUnderlyings;
        }

        public static Dictionary<string, double> ReadMTDFile()
        {
            var dict_MTD = new Dictionary<string, double>();

            try
            {
                var MTDFile = $"C://Prime//Other//MTD.csv";
                if (File.Exists(MTDFile))
                {
                    var arr_Lines = File.ReadAllLines(MTDFile);
                    foreach (var line in arr_Lines)
                    {
                        try
                        {
                            var arr_Fields = line.Split(',').Select(v => v.Trim().ToUpper()).ToArray();

                            if (dict_MTD.ContainsKey(arr_Fields[1]))
                                dict_MTD[arr_Fields[1]] = Convert.ToDouble(arr_Fields[7]);
                            else
                                dict_MTD.Add(arr_Fields[1], Convert.ToDouble(arr_Fields[7]));
                        }
                        catch (Exception ee) { _logger.WriteLog("ReadMTDFile : " + line + Environment.NewLine + ee); }
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog("ReadMTDFile : " + ee); }

            return dict_MTD;
        }

        public static Dictionary<string, LimitInfo> ReadLimitFile()
        {
            var dict_Limit = new Dictionary<string, LimitInfo>();

            try
            {
                var LimitFile = $"C://Prime//Other//LimitFile.csv";
                if (File.Exists(LimitFile))
                {
                    var arr_Lines = File.ReadAllLines(LimitFile);
                    foreach (var line in arr_Lines)
                    {
                        try
                        {
                            var arr_Fields = line.Split(',').Select(v => v.Trim().ToUpper()).ToArray();

                            if (dict_Limit.ContainsKey(arr_Fields[1]))
                                dict_Limit[arr_Fields[0]] = new LimitInfo() { MTMLimit = Convert.ToDouble(arr_Fields[1]), VARLimit = Convert.ToDouble(arr_Fields[2]), MarginLimit = Convert.ToDouble(arr_Fields[3]), BankniftyExpoLimit = Convert.ToDouble(arr_Fields[4]), NiftyExpoLimit = Convert.ToDouble(arr_Fields[5]) };
                            else
                                dict_Limit.Add(arr_Fields[0], new LimitInfo() { MTMLimit = Convert.ToDouble(arr_Fields[1]), VARLimit = Convert.ToDouble(arr_Fields[2]), MarginLimit = Convert.ToDouble(arr_Fields[3]), BankniftyExpoLimit = Convert.ToDouble(arr_Fields[4]), NiftyExpoLimit = Convert.ToDouble(arr_Fields[5]) });
                        }
                        catch (Exception ee) { _logger.WriteLog("ReadLimitFile : " + line + Environment.NewLine + ee); }
                    }
                }
            }
            catch (Exception ee) { _logger.WriteLog("ReadLimitFile : " + ee); }

            return dict_Limit;
        }


    }
}
