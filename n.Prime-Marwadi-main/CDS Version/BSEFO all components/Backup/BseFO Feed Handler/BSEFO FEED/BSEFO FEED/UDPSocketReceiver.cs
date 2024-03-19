using NerveLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

internal class UDPSocketReceiver
{
    /// <summary>
    /// Will save incomplete data.
    /// </summary>
    string PreviousData = string.Empty;
    private static NerveLogger _logger;

    

    internal void StartUDPApp()
    {
        try
        {

            NerveLogger _logger = new NerveLogger(true, true, false, "");
            _logger.Initialize();

            EndProcessTree("BSEFO_FEED");
            try { Process.Start(GlobalCollections._AppPath + "\\BSEFO_FEED.exe"); } catch (Exception ex) { _logger.Error(ex, "StartUDPApp Start"); }
        }
        catch (Exception ee) { _logger.Error(ee); }
    }

    private void EndProcessTree(string imageName)
    {
        try
        {
            var ProcessToBeClose = Process.GetProcesses().Where(p => p.ProcessName.Equals(imageName)).FirstOrDefault();
            ProcessToBeClose?.Kill();
        }
        catch (Exception ee) { _logger.Error(ee, "EndProcessTree"); }
    }

    internal void StartUDPReceive()
    {
        try
        {

            _logger = new NerveLogger(true, true, false, "UDP");

            _logger.Initialize();

            //_logger.Initialize();

            //int receiverPort = Convert.ToInt32(CommonMethods.GetFromConfig("UDP-SERVER", "PORT"));
            //IPAddress receiverIP = IPAddress.Parse(CommonMethods.GetFromConfig("UDP-SERVER", "IP").ToString());

            //IPEndPoint receiverEP = new IPEndPoint(receiverIP, receiverPort);

            //UdpClient receiver = new UdpClient(receiverEP);

            //// Start async receiving
            //receiver.BeginReceive(DataReceived, receiver);

            //var _Text = $"Receiver Setup Success on {receiverPort} Port.";
            //CommonMethods.ConsoleWrite(_Text);
            //_logger.Debug(_Text);

            int receiverPort = Convert.ToInt32(CommonMethods.GetFromConfig("UDP-SERVER", "PORT"));
            IPAddress receiverIP = IPAddress.Parse(CommonMethods.GetFromConfig("UDP-SERVER", "IP").ToString());

            IPEndPoint receiverEP = new IPEndPoint(receiverIP, receiverPort);

            UdpClient receiver = new UdpClient(receiverEP);

            // Start async receiving
            receiver.BeginReceive(DataReceived, receiver);

            var _Text = $"Receiver Setup Success on {receiverPort} Port.";
            CommonMethods.ConsoleWrite(_Text);
            _logger.Debug(_Text);

        }
        catch (Exception ee) { CommonMethods.ConsoleWrite(ee.ToString()); _logger.Error(ee); }
    }

    private void DataReceived(IAsyncResult ar)
    {
        //NerveLogger _logger = new NerveLogger(true, true, false, "");
        //_logger.Initialize();

        int receiverPort = Convert.ToInt32(CommonMethods.GetFromConfig("UDP-SERVER", "PORT"));
        IPAddress receiverIP = IPAddress.Parse(CommonMethods.GetFromConfig("UDP-SERVER", "IP").ToString());

        UdpClient c = (UdpClient)ar.AsyncState;
        //IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        IPEndPoint receivedIpEndPoint = new IPEndPoint(receiverIP, receiverPort);

        Byte[] receivedBytes = c.EndReceive(ar, ref receivedIpEndPoint);
              
        string receivedText = Encoding.UTF8.GetString(receivedBytes);

        _logger.Debug("Bytes rec | "+ receivedBytes);

        //var brdocastpacket = FromByteArray(receivedBytes);
        //if (brdocastpacket != null)
        //    _logger.Debug("Exception occured | converting byte array to object");
        //else
        //    _logger.Debug("InstrumentID |" + brdocastpacket.InstrumentID);

        //_logger.Debug(receivedText);
        //CommonMethods.ConsoleWrite(receivedText);

        //the whole logic added by Omkar
        //^ format data getting by BSEFO feed and we have to separate it by ^ this separator.
        //once that is separated we need to send that data from nCalculate.cs to feed library.
        //comment
        PreviousData += receivedText;
        while (PreviousData.Contains("<EOF>"))
        {
            try
            {
                string ProperData = PreviousData.Substring(0, PreviousData.IndexOf("<EOF>") - 1);
                PreviousData = PreviousData.Substring(PreviousData.IndexOf("<EOF>") + 1);

                string[] fields = ProperData.Split(new char[] { '^', '|' });

                if (fields.Length < 49)
                    continue;

                var _Token = Convert.ToInt32(fields[1]);
                if (!GlobalCollections.dict_LastPacket.ContainsKey(_Token))
                    continue;

                GlobalCollections. dte_LTT = DateTime.Now;

                var TickInfo = new List<string[]>();
                int[] startingIndex = new int[4] { 19, 20, 34, 35 };

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 5; i++)
                {
                    //19 +  
                    sb.Append(fields[startingIndex[0] + (3 * i)] + "|" + fields[startingIndex[1] + (3 * i)] + "|" + fields[startingIndex[2] + (3 * i)] + "|" + fields[startingIndex[3] + (3 * i)]);
                    sb.Append(GlobalCollections._Seperator);
                }

                Packet _Packet = new Packet()
                {
                    Token = _Token,
                    Open = fields[10],
                    High = fields[12],
                    Low = fields[13],
                    Close = fields[11],
                    LTP = fields[9],
                    LTQ = fields[8],
                    LTT = fields[7],             //doubt                         
                    TotalBuyQty = fields[14],
                    TotalSellQty = fields[15],
                    AvgPrice = fields[18],
                    TotalTrades = fields[3],
                    OpenInterest = fields[50],   //doubt               
                    list_BidAskDepth = sb.ToString()            
                };


                _logger.Debug("Response Rec | " + _Packet.Token +":" + _Packet.LTP);
                GlobalCollections.dict_LastPacket[_Token] = _Packet;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

       // _logger.Debug("Data rec | " + PreviousData);

        // Restart listening for udp data packages
        c.BeginReceive(DataReceived, ar.AsyncState);
    }

    public string PrintByteArray(byte[] bytes)
    {
        var sb = new StringBuilder("new byte[] { ");
        foreach (var b in bytes)
        {
            sb.Append(b + ",");
        }
        sb.Append("}");
        return sb.ToString();
    }

    public BrodCastPacket FromByteArray(byte[] data)
    {
        try
        {
            if (data == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (BrodCastPacket)obj;
            }
        }
        catch (Exception ee) { _logger.Error(ee); return null; }

    }

}