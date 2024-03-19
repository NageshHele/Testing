using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Feed_Receiver_BSE.Data_Structures
{
    public class ConnectionInfo
    {
        public string Username { get; set; } = "-";
        public IPAddress IP { get; set; } = IPAddress.Parse("127.0.0.1");
        public int PORT { get; set; } = 0;
        public int Subscribed { get; set; } = 0;
        public bool IsConnected { get; set; } = true;
    }

    public class Packet
    {
        public int Token { get; set; } = 0;
        public double Open { get; set; } = 0;
        public double High { get; set; } = 0;
        public double Low { get; set; } = 0;
        public double Close { get; set; } = 0;
        public double PreviousClose { get; set; } = 0;
        public double LTP { get; set; } = 0;
        public long LTQ { get; set; } = 0;
        public long Volume { get; set; } = 0;
        public double AvgPrice { get; set; } = 0;
        public List<double[]> list_BidAskDepth { get; set; }
    }

    public class SocketInfo
    {
        public IPEndPoint RemoteEndPoint { get; set; }
        public Socket Client { get; set; }
    }
}
