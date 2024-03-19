using System.Collections.Generic;
using System.Net;

namespace Engine
{
    public class LevelInfo
    {
        public double MTM { get; set; } = 0;
        public double IntradayMTM { get; set; } = 0;
        public double Delta { get; set; } = 0;
        public double Gamma { get; set; } = 0;
        public double Theta { get; set; } = 0;
        public double Vega { get; set; } = 0;
    }

    public class PrimeConnections
    {
        public string Username { get; set; }
        public IPAddress IP { get; set; }
        public bool IsTradeConnected { get; set; } = false;
        public bool IsSpanConnected { get; set; } = false;
        //public string Disconnect { get; set; } = "";
    }

    public class LimitInfo
    {
        public double MTMLimit { get; set; } = 0;
        public double VARLimit { get; set; } = 0;
        public double MarginLimit { get; set; } = 0;
        public double BankniftyExpoLimit { get; set; } = 0;
        public double NiftyExpoLimit { get; set; } = 0;

    }
}
