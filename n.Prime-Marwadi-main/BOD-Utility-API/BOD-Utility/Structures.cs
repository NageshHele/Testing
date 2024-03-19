using System;

namespace BOD_Utility
{
    public class FTPCRED
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ContractMasterData
    {
        public string Symbol { get; set; }
        public string InstName { get; set; }
        public string ScripType { get; set; }
        public string StrikePrice { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

    public class OTMFileData
    {
        public string Symbol { get; set; }
        public string InstName { get; set; }
        public string ScripType { get; set; }
        public string StrikePrice { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Percentage { get; set; }
    }

    public class NiftyOTMFile
    {
        public string Symbol { get; set; }
        public string ExpiryDate { get; set; }
        public string OTMPercentage { get; set; } = "0";
        public string OTHPercentage { get; set; } = "0";
    }
}
