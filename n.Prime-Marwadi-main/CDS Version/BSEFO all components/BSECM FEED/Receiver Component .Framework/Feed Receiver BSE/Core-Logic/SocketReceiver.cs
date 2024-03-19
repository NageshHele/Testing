using DevExpress.XtraEditors;
using Feed_Receiver_BSE.Data_Structures;
using Feed_Receiver_BSE.Helper;
using NerveLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Feed_Receiver_BSE.Core_Logic
{
    class SocketReceiver
    {
        NerveLogger _logger;

        /// <summary>
        /// Will save incomplete data.
        /// </summary>
        static string PreviousData = string.Empty;

        Label lbl_LastTradeTime;

        internal SocketReceiver(Label lbl)
        {
            _logger = CollectionHelper._logger;

            lbl_LastTradeTime = lbl;
        }

        #region Semi-Imp Methods

        internal void StartUDPApp()
        {
            try
            {
                EndProcessTree("BSE_FEED");
                try { Process.Start(Application.StartupPath + "\\BSE_FEED.exe"); } catch (Exception ex) { _logger.Error(ex, "StartUDPApp Start"); }
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
                int receiverPort = Convert.ToInt32(CollectionHelper.GetFromConfig("EXCHANGE-CONNECTION", "PORT"));
                IPAddress _IP = IPAddress.Parse(CollectionHelper.GetFromConfig("EXCHANGE-CONNECTION", "IP").ToString());
                // IPAddress receiverIP = IPAddress.Parse(CollectionHelper.GetFromConfig("UDP-SERVER", "IP").ToString());
                //IPEndPoint receiverEP = new IPEndPoint(receiverIP, receiverPort);
                //UdpClient receiver = new UdpClient(receiverEP);
               
                UdpClient receiver = new UdpClient(receiverPort);
                // Start async receiving
                receiver.BeginReceive(DataReceived, receiver);

                _logger.Debug("Receiver Setup Success " + _IP +":"+receiverPort);
            }
            catch (Exception ee) { _logger.Error(ee, "StartUDPReceive"); }
        }

        private void DataReceived(IAsyncResult ar)
        {
            //int receiverPort = Convert.ToInt32(CollectionHelper.GetFromConfig("UDP-SERVER", "PORT"));
            //IPAddress receiverIP = IPAddress.Parse(CollectionHelper.GetFromConfig("UDP-SERVER", "IP").ToString());

            UdpClient c = (UdpClient)ar.AsyncState;
            IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Byte[] receivedBytes = c.EndReceive(ar, ref receivedIpEndPoint);

            _logger.Debug("NEW PACKET REC |" + receivedBytes.Length);

            string receivedText = Encoding.UTF8.GetString(receivedBytes);

            //_logger.Debug("Packet data | " + receivedText);

            PreviousData += receivedText;
            while (PreviousData.Contains("<EOF>"))
            {
                try
                {
                    string ProperData = PreviousData.Substring(0, PreviousData.IndexOf("<EOF>") - 1);
                    PreviousData = PreviousData.Substring(PreviousData.IndexOf("<EOF>") + 1);

                    string[] fields = ProperData.Split(new char[] { '^', '|' });

                    if (fields.Length < 31)
                    {
                        _logger.Debug("Packet length short | "+ receivedText);
                        continue;
                    }

                    var _Token = Convert.ToInt32(fields[2]);
                    var IndexNameTosearch = fields[1].Trim().Replace("\x00","").ToUpper();
                    if (CollectionHelper.dict_IndexToken.TryGetValue(IndexNameTosearch, out _Token))
                    {
                        _logger.Debug("Index ScripFound |"+IndexNameTosearch+"|"+ _Token);
                    }
                    
                    if (!CollectionHelper.dict_SubscribedClients.ContainsKey(_Token))
                    {
                        continue;
                    }
                    
                    //Added this check beacuse improper data is receiving with 3276.8 value.
                    //if (DBLRS(fields[22]).Equals(3276.8) && DBLRS(fields[37]).Equals(0)) continue;

                    lbl_LastTradeTime.Invoke((MethodInvoker)(() =>
                    {
                        lbl_LastTradeTime.Text = "LTT : " + DateTime.Now.ToString("HH:mm:ss");
                    }));

                    var list_BidAskDepth = new List<double[]>();
                    for (int i = 0; i < 5; i++)
                        list_BidAskDepth.Add(new double[4] { 0, 0, 0, 0 });

                    Packet _Packet = new Packet()
                    {
                        Token = _Token,
                        Open = DBLRS(fields[11]),
                        High = DBLRS(fields[13]),
                        Low =  DBLRS(fields[14]),
                        Close = DBLRS(fields[12]),
                        LTP =DBLRS(fields[10]),
                        LTQ =Convert.ToInt64(fields[9]),
                        PreviousClose = 0.0,

                        //Open = DBLRS(fields[1]),
                        //High = DBLRS(fields[2]),
                        //Low = DBLRS(fields[3]),
                        //Close = DBLRS(fields[4]),                       
                        //LTP = DBLRS(fields[6]),
                        //LTQ = Convert.ToInt64(Convert.ToDouble(fields[7])),
                        AvgPrice = DBLRS(fields[19]),
                        Volume = Convert.ToInt64(Convert.ToDouble(fields[4])),
                        list_BidAskDepth = list_BidAskDepth
                    };

                    CollectionHelper.dict_LastPacket[_Token] = _Packet;

                    foreach (var ID in CollectionHelper.dict_SubscribedClients[_Token])
                        SocketSender.SendToClient(CollectionHelper.dict_UDPClientSocket[ID], _Token);
                }
                catch (Exception ee) { _logger.Error(ee); }
            }

            // Restart listening for udp data packages
            c.BeginReceive(DataReceived, ar.AsyncState);
        }

        /// <summary>
        /// Converts value to Rs.
        /// </summary>
        /// <param name="Value">Pass string value.</param>
        /// <returns>Value / 100</returns>
        private static double DBLRS(string Value) => Convert.ToDouble(Value) / 100;

        /// <summary>
        /// Converts value to double.
        /// </summary>
        /// <param name="Value">Pass string value.</param>
        /// <returns>Convert.ToDouble(Value)</returns>
        private static double DBLQ(string Value) => Convert.ToDouble(Value);

        #endregion
    }
}
