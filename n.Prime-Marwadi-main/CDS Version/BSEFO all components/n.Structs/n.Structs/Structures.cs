using ProtoBuf;
using System;
using System.Collections.Generic;

namespace n.Structs
{
    public class Greeks
    {
        public double IV { get; set; } = 30;
        public double IVHigher { get; set; } = 45;
        public double IVLower { get; set; } = 15;
        public double Delta { get; set; } = 0;
        public double Theta { get; set; } = 0;
        public double Gamma { get; set; } = 0;
        public double Vega { get; set; } = 0;
        public bool IsReceived { get; set; } = false;
    }
    
    public class ClientInfo
    {
        public string Name { get; set; }
        public string Zone { get; set; }
        public string Branch { get; set; }
        public string Family { get; set; }
        public string Product { get; set; }
        public double ELM { get; set; }
        public double AdHoc { get; set; }
        /// <summary>
        /// Format : CI172^ALL_839955.0,CI172^NIFTY_839955.0
        /// </summary>
        public string NPLValues { get; set; } = "";
        public double MTDValue { get; set; } = 0;
    }

    [ProtoContract]
    public class ConsolidatedPositionInfo
    {
        static string defStr = "-";
        static int defInt = 0;
        static double defDbl = 0;
        [ProtoMember(1)]
        public string Username { get; set; } = defStr;
        [ProtoMember(2)]
        public en_Segment Segment { get; set; } = en_Segment.NSECM;
        [ProtoMember(3)]
        public string Series { get; set; } = "-";
        [ProtoMember(4)]
        public string ScripName { get; set; } = defStr;
        [ProtoMember(5)]
        public int Token { get; set; } = defInt;
        [ProtoMember(6)]
        public double LTP { get; set; } = -1;
        [ProtoMember(7)]
        public double Expiry { get; set; } = defDbl;
        [ProtoMember(8)]
        public string Underlying { get; set; } = defStr;
        [ProtoMember(9)]
        public double UnderlyingLTP { get; set; } = -1;
        [ProtoMember(10)]
        public double StrikePrice { get; set; } = defDbl;
        [ProtoMember(11)]
        public en_ScripType ScripType { get; set; }
        [ProtoMember(12)]
        public en_InstrumentName InstrumentName { get; set; }
        [ProtoMember(13)]
        public double BEP { get; set; } = defDbl;
        [ProtoMember(14)]
        public double IVLower { get; set; } = defDbl;
        [ProtoMember(15)]
        public double IVMiddle { get; set; } = defDbl;
        [ProtoMember(16)]
        public double IVHigher { get; set; } = defDbl;
        [ProtoMember(17)]
        public long IntradayNetPosition { get; set; } = defInt;
        [ProtoMember(18)]
        public double ATM_IV { get; set; } = defDbl;
        [ProtoMember(19)]
        public long NetPositionCF { get; set; } = defInt;
        [ProtoMember(20)]
        public double PriceCF { get; set; } = defDbl;
        [ProtoMember(21)]
        public bool IsLTPCalculated { get; set; } = false;
        [ProtoMember(22)]
        public double Delta { get; set; } = defDbl;
        [ProtoMember(23)]
        public double Gamma { get; set; } = defDbl;
        [ProtoMember(24)]
        public double Theta { get; set; } = defDbl;
        [ProtoMember(25)]
        public double Vega { get; set; } = defDbl;
        [ProtoMember(26)]
        public bool IsLDO { get; set; } = false;
        [ProtoMember(27)]
        public double MTM { get; set; } = defDbl;
        [ProtoMember(28)]
        public long NetPosition { get; set; } = defInt;
        [ProtoMember(29)]
        public double IntradayMTM { get; set; } = defDbl;
        [ProtoMember(30)]
        public double IntradayBEP { get; set; } = defDbl;
        [ProtoMember(31)]
        public double VARMargin { get; set; } = defDbl;
        [ProtoMember(32)]
        public double SingleDelta { get; set; } = defDbl;
        [ProtoMember(33)]
        public double SingleGamma { get; set; } = defDbl;
        [ProtoMember(34)]
        public double EquityAmount { get; set; } = defDbl;
        [ProtoMember(35)]
        public double DayNetPremium { get; set; } = defDbl;
        [ProtoMember(36)]
        public double UnderlyingVARMargin { get; set; } = defDbl;
        [ProtoMember(37)]
        public double SpotPrice { get; set; } = -1;
        [ProtoMember(38)]
        public TimeSpan ExpiryTimeSpan { get; set; }
        [ProtoMember(39)]
        public long IntradayBuyQuantity { get; set; } = defInt;
        [ProtoMember(40)]
        public double IntradayBuyAvg { get; set; } = defDbl;
        [ProtoMember(41)]
        public long IntradaySellQuantity { get; set; } = defInt;
        [ProtoMember(42)]
        public double IntradaySellAvg { get; set; } = defDbl;
        [ProtoMember(43)]
        public double TheoreticalPrice { get; set; } = -1;
        [ProtoMember(44)]
        public double TheoreticalMTM { get; set; } = defDbl;
        [ProtoMember(45)]
        public double ClosingPrice { get; set; } = defDbl;  //Added by Akshay on 30-06-2021 for closing
        [ProtoMember(46)]
        public double IntradayCost { get; set; } = defDbl;  //Added by Akshay on 22-10-2021 for Cost Computing
        [ProtoMember(47)]
        public int LotSize { get; set; } = defInt;

        [ProtoMember(48)]
        public double CDSMTM { get; set; } = defDbl;
        [ProtoMember(49)]
        public double CDSIntradayMTM { get; set; } = defDbl;
        [ProtoMember(50)]
        public long T1Quantity { get; set; } = defInt;   //added on 11JAN2022 by Nikhil
        [ProtoMember(51)]
        public long T2Quantity { get; set; } = defInt;    //added on 11JAN2022 by Nikhil 
        [ProtoMember(52)]
        public long EarlyPayIn { get; set; } = defInt;   //added on 11JAN2022 by nikhil 
        [ProtoMember(53)]
        public long CollateralQty { get; set; } = defInt;
        [ProtoMember(54)]
        public double CollateralValue { get; set; } = defDbl;
        [ProtoMember(55)]
        public double CollateralHaircut { get; set; } = defDbl;

        [ProtoMember(56)]
        public double DayNetPremiumCDS { get; set; } = defDbl;
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
        public double IntradayCost { get; set; } = defDbl;  //Added by Akshay on 22-10-2021 for Cost Computing
        public string TradeTime { get; set; } = defStr;
        public string TradeID { get; set; } = defStr;
    }


    public class IntrinsicInfo
    {
        public string TradeID { get; set; }
        public string Username { get; set; }
        public string Underlying { get; set; }
        public en_ScripType ScripType { get; set; }
        public double StrikePrice { get; set; }
        public string ExpiryDate { get; set; }
        public long NetPosition { get; set; }
        public double BEP { get; set; }
        public double UnderlyingLTP { get; set; }
        //public string TradeTime { get; set; }
    }

    public class BanInfo
    {
        public string ClientID { get; set; } = "-";
        public string Scrip { get; set; } = "-";
        public long IntradayBuyQty { get; set; } = 0;
        public long IntradaySellQty { get; set; } = 0;
        public long NetPosCF { get; set; } = 0;
        public long IntradayNetPos { get; set; } = 0;
    }

    //public class HeartBeatTick
    //{
    //    static double defDbl = 0;

    //    public double FOLastTickTime { get; set; } = defDbl;
    //    public double CMLastTickTime { get; set; } = defDbl;

    //    public double FOLastTradeTime { get; set; } = defDbl;
    //    public double CMLastTradeTime { get; set; } = defDbl;

    //    public bool isGatewayConnected { get; set; } = false;
    //    public bool isSpanConnected { get; set; } = false;
    //}

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

    public class EODPositionInfo
    {
        public string Username { get; set; }
        public en_Segment Segment { get; set; }
        public int Token { get; set; }
        public double TradePrice { get; set; }
        public long TradeQuantity { get; set; }
        public en_Segment UnderlyingSegment { get; set; }
        public int UnderlyingToken { get; set; }
    }

    [ProtoContract]
    public class ErrorCodes
    {
        [ProtoMember(1)]
        public string TradeId { get; set; }
        [ProtoMember(2)]
        public string ClientCode { get; set; }
        [ProtoMember(3)]
        public string CtclId { get; set; }
        [ProtoMember(4)]
        public string NeatId { get; set; }
        [ProtoMember(5)]
        public string Symbol { get; set; }
        [ProtoMember(6)]
        public string Scriptype { get; set; }
        [ProtoMember(7)]
        public string Strike { get; set; }
        [ProtoMember(8)]
        public string ExpiryDate { get; set; }
        [ProtoMember(9)]
        public string Direction { get; set; }
        [ProtoMember(10)]
        public string Quantity { get; set; }
    }


    public class Surveillance
    {
        public int ID { get; set; }
        public string Buyer { get; set; }
        public string Seller { get; set; }
        public string TradeId { get; set; }
        public string Underlying { get; set; }
        public string Series { get; set; }
        public int Token { get; set; }
        public long Quantity { get; set; }
        public TimeSpan TradeTime { get; set; }
        public List<Surveillance> list_Surv { get; set; }
    }

    public enum en_Segment
    {
        NSECM,
        NSECD,
        NSEFO,
        BSECM,
        BSEFO
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
        OPTSTK,
        FUTCUR,
        OPTCUR
    }
}
