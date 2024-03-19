using System;
using System.Collections.Generic;
using System.IO;

namespace Engine
{
    public class Exchange
    {
        public static List<Contract> ReadContract(string ContractFilePath)
        {
            List<Contract> list_Contract = new List<Contract>();

            try
            {
                string[] arr_ContractLines = File.ReadAllLines(ContractFilePath);

                for (int CIdx = 1; CIdx < arr_ContractLines.Length; CIdx++)
                {
                    try
                    {
                        string[] fields = arr_ContractLines[CIdx].ToUpper().Split('|');

                        Contract _contract = new Contract();
                        _contract.Token = Convert.ToInt32(fields[0]);

                        string ScripTyp = fields[2].Trim();
                        _contract.Instrument = ScripTyp.Equals("FUTSTK") ? Instrument.FUTSTK : (ScripTyp.Equals("OPTSTK") ? Instrument.OPTSTK : (ScripTyp.Equals("FUTIDX") ? Instrument.FUTIDX : Instrument.OPTIDX));

                        _contract.Symbol = fields[3];
                        _contract.ScripName = fields[53];

                        DateTime dte_Expiry = new DateTime(1980, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
                        dte_Expiry = dte_Expiry.AddSeconds(Convert.ToDouble(fields[6])).ToLocalTime();
                        _contract.Expiry = dte_Expiry;

                        string TYPE = fields[8];
                        double StrikePrice = (Convert.ToDouble(fields[7]) / 100);
                        _contract.StrikePrice = TYPE.Equals("CE") ? StrikePrice : (TYPE.Equals("PE") ? StrikePrice : 0);

                        _contract.ScripType = TYPE.Equals("CE") ? ScripType.CE : (TYPE.Equals("PE") ? ScripType.PE : ScripType.XX);
                        _contract.LotSize = Convert.ToInt32(fields[30]);
                        _contract.LowerLimit = Convert.ToDouble(fields[42]) / 100;
                        _contract.UpperLimit = Convert.ToDouble(fields[43]) / 100;

                        _contract.CustomScripname = $"{_contract.Symbol}|{_contract.Expiry.ToString("ddMMMyyyy").ToUpper()}|{(_contract.StrikePrice == 0 ? "0" : Math.Round(_contract.StrikePrice, 2).ToString("#.00"))}|{_contract.ScripType}";

                        list_Contract.Add(_contract);
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }

            return list_Contract;
        }

        public static List<Security> ReadSecurity(string SecurityFilePath)
        {
            List<Security> list_Security = new List<Security>();

            try
            {
                string[] arr_SecurityLines = File.ReadAllLines(SecurityFilePath);

                for (int SIdx = 1; SIdx < arr_SecurityLines.Length; SIdx++)
                {
                    try
                    {
                        string[] fields = arr_SecurityLines[SIdx].ToUpper().Split('|');

                        Security _security = new Security();
                        _security.Token = Convert.ToInt32(fields[0]);
                        _security.Symbol = fields[1];
                        _security.Series = fields[2];
                        _security.ScripName = $"{_security.Symbol}-{_security.Series}";
                        _security.LotSize = 1;
                        _security.LowerLimit = Convert.ToDouble(fields[6].Split('-')[0]);
                        _security.UpperLimit = Convert.ToDouble(fields[6].Split('-')[1]);

                        _security.CustomScripname = $"{_security.Symbol}|{_security.Expiry.ToString("ddMMMyyyy").ToUpper()}|{0}|{_security.Series}";

                        list_Security.Add(_security);
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }

            return list_Security;
        }

        public static List<FOBhavcopy> ReadFOBhavcopy(string FOBhavcopyPath)
        {
            List<FOBhavcopy> list_FOBhavcopy = new List<FOBhavcopy>();

            try
            {
                string[] arr_BhavcopyLines = File.ReadAllLines(FOBhavcopyPath);

                for (int SIdx = 1; SIdx < arr_BhavcopyLines.Length; SIdx++)
                {
                    try
                    {
                        string[] fields = arr_BhavcopyLines[SIdx].ToUpper().Split(',');

                        FOBhavcopy _FO = new FOBhavcopy();
                        _FO.Symbol = fields[1];
                        string ScripTyp = fields[0].Trim();
                        _FO.Instrument = ScripTyp.Equals("FUTSTK") ? Instrument.FUTSTK : (ScripTyp.Equals("OPTSTK") ? Instrument.OPTSTK : (ScripTyp.Equals("FUTIDX") ? Instrument.FUTIDX : Instrument.OPTIDX));
                        _FO.Expiry = DateTime.Parse(fields[2]);
                        _FO.StrikePrice = Convert.ToDouble(fields[3]);

                        string TYPE = fields[4];
                        _FO.ScripType = TYPE.Equals("CE") ? ScripType.CE : (TYPE.Equals("PE") ? ScripType.PE : ScripType.XX);

                        _FO.Open = Convert.ToDouble(fields[5]);
                        _FO.High = Convert.ToDouble(fields[6]);
                        _FO.Low = Convert.ToDouble(fields[7]);
                        _FO.Close = Convert.ToDouble(fields[8]);
                        _FO.SettlePrice = Convert.ToDouble(fields[9]);

                        _FO.Contracts = long.Parse(fields[10]);

                        _FO.ValueInLacs = Convert.ToDouble(fields[11]);

                        _FO.OpenInterest = long.Parse(fields[12]);
                        _FO.ChangeInOpenInterest = long.Parse(fields[13]);

                        _FO.Timestamp = Convert.ToDateTime(fields[14]);

                        _FO.CustomScripname = $"{_FO.Symbol}|{_FO.Expiry.ToString("ddMMMyyyy").ToUpper()}|{(_FO.StrikePrice == 0 ? "0" : Math.Round(_FO.StrikePrice, 2).ToString("#.00"))}|{_FO.ScripType}";

                        list_FOBhavcopy.Add(_FO);
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }

            return list_FOBhavcopy;
        }

        public static List<CMBhavcopy> ReadCMBhavcopy(string CMBhavcopyPath)
        {
            List<CMBhavcopy> list_CMBhavcopy = new List<CMBhavcopy>();

            try
            {
                string[] arr_BhavcopyLines = File.ReadAllLines(CMBhavcopyPath);

                for (int SIdx = 1; SIdx < arr_BhavcopyLines.Length; SIdx++)
                {
                    try
                    {
                        string[] fields = arr_BhavcopyLines[SIdx].ToUpper().Split(',');

                        CMBhavcopy _CM = new CMBhavcopy();
                        _CM.Symbol = fields[0];
                        _CM.Series = fields[1];

                        _CM.ScripName = $"{_CM.Symbol}-{_CM.Series}";

                        _CM.Open = Convert.ToDouble(fields[2]);
                        _CM.High = Convert.ToDouble(fields[3]);
                        _CM.Low = Convert.ToDouble(fields[4]);
                        _CM.Close = Convert.ToDouble(fields[5]);
                        _CM.Last = Convert.ToDouble(fields[6]);
                        _CM.PreviousClose = Convert.ToDouble(fields[7]);

                        _CM.TotalTradeQty = long.Parse(fields[8]);

                        _CM.TotalTradeValue = Convert.ToDouble(fields[9]);
                        _CM.TotalTrades = long.Parse(fields[11]);

                        _CM.ISIN = fields[12];

                        _CM.Timestamp = Convert.ToDateTime(fields[10]);

                        _CM.CustomScripname = $"{_CM.Symbol}|{_CM.Expiry.ToString("ddMMMyyyy").ToUpper()}|{0}|{_CM.Series}";

                        list_CMBhavcopy.Add(_CM);
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }

            return list_CMBhavcopy;
        }

        public class Contract
        {
            public int Token { get; set; }
            /// <summary>
            /// Symbol|Expiry|Strike|ScripType (NIFTY|29OCT2020|11500.00|CE OR NIFTY|29OCT2020|0|XX) 
            /// </summary>
            public string CustomScripname { get; set; }

            public Instrument Instrument { get; set; }
            /// <summary>
            /// Underlying
            /// </summary>
            public string Symbol { get; set; }
            public string ScripName { get; set; }
            public DateTime Expiry { get; set; }
            /// <summary>
            /// 0 for XX.
            /// </summary>
            public double StrikePrice { get; set; }
            public ScripType ScripType { get; set; }
            public int LotSize { get; set; }
            public double LowerLimit { get; set; }
            public double UpperLimit { get; set; }
        }

        public class FOBhavcopy
        {
            public string Symbol { get; set; }

            /// <summary>
            /// Symbol|Expiry|Strike|ScripType (NIFTY|29OCT2020|11500.00|CE OR NIFTY|29OCT2020|0|XX) 
            /// </summary>
            public string CustomScripname { get; set; }
            public Instrument Instrument { get; set; }
            
            public DateTime Expiry { get; set; }
            /// <summary>
            /// 0 for XX.
            /// </summary>
            public double StrikePrice { get; set; }
            public ScripType ScripType { get; set; }

            public double Open { get; set; }
            public double High { get; set; }
            public double Low { get; set; }
            public double Close { get; set; }
            public double SettlePrice { get; set; }

            public long Contracts { get; set; }

            public double ValueInLacs { get; set; }
            
            public long OpenInterest { get; set; }
            public long ChangeInOpenInterest { get; set; }

            /// <summary>
            /// Date of the data present in file.
            /// </summary>
            public DateTime Timestamp { get; set; }
        }

        public class Security
        {
            public int Token { get; set; }

            /// <summary>
            /// Symbol|Expiry|Strike|Series (ACC|01JAN1980|0|EQ OR ACC|01JAN1980|0|SM) 
            /// </summary>
            public string CustomScripname { get; set; }

            /// <summary>
            /// Sample : ACC
            /// </summary>
            public string Symbol { get; set; }
            /// <summary>
            /// Sample : ACC-EQ
            /// </summary>
            public string ScripName { get; set; }
            /// <summary>
            /// Default set to 1/1/1980
            /// </summary>
            public DateTime Expiry { get; set; } = new DateTime(1980, 1, 1, 0, 0, 0);
            /// <summary>
            /// Sample : EQ Or BE Or SM Or ...
            /// </summary>
            public string Series { get; set; }
            public int LotSize { get; set; }
            public double LowerLimit { get; set; }
            public double UpperLimit { get; set; }
        }

        public class CMBhavcopy
        {
            /// <summary>
            /// Symbol|Expiry|Strike|Series (ACC|01JAN1980|0|EQ OR ACC|01JAN1980|0|SM) 
            /// </summary>
            public string CustomScripname { get; set; }
            /// <summary>
            /// Sample : ACC
            /// </summary>
            public string Symbol { get; set; }

            /// <summary>
            /// Sample : EQ Or BE Or SM Or ...
            /// </summary>
            public string Series { get; set; }

            /// <summary>
            /// Sample : ACC-EQ
            /// </summary>
            public string ScripName { get; set; }

            /// <summary>
            /// Default set to 1/1/1980
            /// </summary>
            public DateTime Expiry { get; set; } = new DateTime(1980, 1, 1, 0, 0, 0);

            public double Open { get; set; }
            public double High { get; set; }
            public double Low { get; set; }
            public double Close { get; set; }
            public double Last { get; set; }
            public double PreviousClose { get; set; }

            public long TotalTradeQty { get; set; }

            public double TotalTradeValue { get; set; }
            public long TotalTrades { get; set; }

            /// <summary>
            /// Date of the data present in file.
            /// </summary>
            public DateTime Timestamp { get; set; }

            public string ISIN { get; set; }
        }

        public enum Instrument
        {
            FUTSTK,
            OPTSTK,
            FUTIDX,
            OPTIDX
        }

        public enum ScripType
        {
            XX,
            CE,
            PE
        }
    }
}
