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
        public string pQty { get; set; }
        public double pTradeTime { get; set; } = 0;
    }
}
