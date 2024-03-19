using System.Net;

public class Packet
{
    static string _Empty = "-";
    public int Token { get; set; } = 0;
    public double Multiplier { get; set; } = 1;
    public string Open { get; set; } = _Empty;
    public string High { get; set; } = _Empty;
    public string Low { get; set; } = _Empty;
    public string Close { get; set; } = _Empty;
    public string LTP { get; set; } = "0";
    public string LTQ { get; set; } = "0";
    public string LTT { get; set; } = _Empty;
    public string TotalTrades { get; set; } = _Empty;
    public string TotalBuyQty { get; set; } = _Empty;
    public string TotalSellQty { get; set; } = _Empty;
    public string AvgPrice { get; set; } = _Empty;
    public string OpenInterest { get; set; } = _Empty;
    public string list_BidAskDepth { get; set; } = _Empty;
}


public class BrodCastPacket
{
    public long InstrumentID { get; set; } = 0;
    public int NumberOfTrades { get; set; } = 1;
    public int TradeVolume { get; set; } = 0;
    public int TradeValue { get; set; } = 0;
    public char TraceValue { get; set; }

    public long TimeStamp { get; set; } = 0;
    public int TodayCloseRate { get; set; } = 0;
    public int LTQ { get; set; } = 0;
    public int LTP { get; set; } = 0;
    public int OpenPrice { get; set; } = 0;
    public int LastTradingSessionClosePrice { get; set; } = 0;
    public int High { get; set; } = 0;
    public int Low { get; set; } = 0;
    public int TotalBidQuantity { get; set; } = 0;
    public int TotalAskQuantity { get; set; } = 0;

    public int LowerCircuit { get; set; } = 0;
    public int UpperCircuit { get; set; } = 0;
    public int WeightedAveragePrice { get; set; } = 0;
    public int[] BID { get; set; }

    public int[] ASK { get; set; }
    public int OpenInterestQuantity { get; set; } = 0;

    public int OpenInterestValue { get; set; } = 0;

    public int OpenInterestChange { get; set; } = 0;

    public int VarIMPercentage { get; set; } = 0;

    public int ELMVARPercentage { get; set; } = 0;

}

public class ConnectionInfo
{
    public string Username { get; set; } = "-";
    public IPAddress IP { get; set; } = IPAddress.Parse("127.0.0.1");
    public int PORT { get; set; } = 0;
    public int Subscribed { get; set; } = 0;
    public bool IsConnected { get; set; } = true;
}
