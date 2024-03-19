using System;

namespace Gateway
{
    public class Span
    {
        public long pMemberId { get; set; }
        public string pClientId { get; set; }
        public string pExchange { get; set; }
        public string pSegment { get; set; }
        public string pScripName { get; set; }
        public string pExpiry { get; set; }
        public string pStrikePrice { get; set; }
        public string pCallPut { get; set; }
        public string pFactor { get; set; }
        public long pQty { get; set; }
        public double pTradeTime { get; set; } 
        public int pToken { get; set; }
    }

    public class PositionInfo
    {
        static string defStr = "-";
        static double defDbl = 0;
        static int defInt = 0;

        public string Username { get; set; } = defStr;
        public en_Segment Segment { get; set; } = en_Segment.NSECM;
        public int Token { get; set; } = defInt;
        public long TradeQuantity { get; set; } = defInt;
        public double TradePrice { get; set; } = defDbl;
        public double TradeValue { get; set; } = defDbl;
        public long IntradayQuantity { get; set; } = defInt;
        public double IntradayValue { get; set; } = defDbl;
        public en_Segment UnderlyingSegment { get; set; }
        public int UnderlyingToken { get; set; } = defInt;
        public long BuyQuantity { get; set; } = defInt;
        public double BuyValue { get; set; } = defDbl;
        public long SellQuantity { get; set; } = defInt;
        public double SellValue { get; set; } = defDbl;
        public double TradeTime { get; set; } = defDbl;
        
    }

    public class ContractMaster
    {
        public int Token { get; set; }
        public string Symbol { get; set; }
        public string Series { get; set; } = "-";
        public en_InstrumentName InstrumentName { get; set; }
        public en_Segment Segment { get; set; }
        public string ScripName { get; set; }
        public string CustomScripName { get; set; }
        public en_ScripType ScripType { get; set; }
        public double ExpiryUnix { get; set; } = 0;
        public double StrikePrice { get; set; }
        public int LotSize { get; set; }
        public int UnderlyingToken { get; set; }
        public en_Segment UnderlyingSegment { get; set; }
    }

    public enum en_Segment
    {
        NSECM,
        NSECD,
        NSEFO,
        BSECM
    }

    public enum en_ScripType
    {
        EQ,
        CE,
        PE,
        XX
    }

    public enum en_InstrumentName
    {
        EQ,
        FUTIDX,
        FUTSTK,
        OPTIDX,
        OPTSTK
    }
}
