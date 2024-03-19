using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Engine
{
    public delegate void del_LTTReceived(double _FOLastTradeTime, double _CMLasetTradeTime, double _CDLasetTradeTime,double _BSECMLastTradeTime, double _BSEFOLastTradeTime);
    public delegate void del_GatewayStatusReceived(bool isGatewayConnected);
    public delegate void del_SpanStatusReceived(bool isSpanConnected, bool isCDSSpanConnected);
    public delegate void del_Error(string _Message);
    public delegate void del_SpanInfoReceived(double _SpanComputeTime, double _CDSSpanComputeTime, string _LatestSpanFileName, string _LatestCDSSpanFileName);

    class GatewayHBConnector
    {
        Socket soc_GatewayHB = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //changed on 25JAN2021 by Amey
        public event del_LTTReceived eve_LTTReceived;
        public event del_GatewayStatusReceived eve_GatewayStatusReceived;
        public event del_SpanStatusReceived eve_SpanStatusReceived;
        public event del_Error eve_Error;
        public event del_SpanInfoReceived eve_SpanInfoReceived;

        string _EngineID = "";
        int _GatewayHBPort = 0;
        IPAddress _GatewayHBIP = IPAddress.Any;

        int _HBConnectionAttempts = 0;

        int _MaxConnectionAttempts = 5;

        public void ConnectToGateway(string ID, IPAddress _GatewayIP, int _GatewayPORT)   // connecting to server
        {
            if (ID != "")
            {
                _EngineID = ID;
                _GatewayHBIP = _GatewayIP;
                _GatewayHBPort = _GatewayPORT;

                soc_GatewayHB = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                while (!soc_GatewayHB.Connected && (_HBConnectionAttempts < _MaxConnectionAttempts))
                {
                    try
                    {
                        _HBConnectionAttempts++;

                        soc_GatewayHB.Connect(_GatewayIP, _GatewayPORT);

                        if (soc_GatewayHB.Connected)
                        {
                            _HBConnectionAttempts = 0;

                            //added on 25JAN2021 by Amey
                            eve_GatewayStatusReceived(true);

                            SendToGateway("ID^" + ID);

                            Task.Run(() => ReceiveResponse(soc_GatewayHB));
                        }
                    }
                    catch (SocketException ee)
                    {
                        if (_HBConnectionAttempts < _MaxConnectionAttempts)
                        {
                            eve_Error("Retrying " + _GatewayIP + ":" + _GatewayPORT + Environment.NewLine + ee);
                            Thread.Sleep(10000);
                        }
                        else
                        {
                            //added on 08APR2021 by Amey
                            eve_Error("Unable to connect to the Gateway Server. " + _GatewayIP + ":" + _GatewayPORT + Environment.NewLine + ee);

                            _HBConnectionAttempts = 0;

                            break;
                        }
                    }
                }
            }
        }

        private void ReceiveResponse(Socket soc_Current)
        {
            string _ReceivedText = "";
            string[] arr_Fields;
            int _ReceivedFieldsLength = 0;

            var isConnected = true;

            while (isConnected)
            {
                try
                {
                    var buffer = new byte[2048];      //commented on 14-11-18 by Amey
                    int _ReceivedBytesLength = soc_Current.Receive(buffer, SocketFlags.None);

                    if (_ReceivedBytesLength > 0)
                    {
                        byte[] recBuf = new byte[_ReceivedBytesLength];
                        Array.Copy(buffer, recBuf, _ReceivedBytesLength);

                        //changed on 25JAN2021 by Amey
                        _ReceivedText = Encoding.UTF8.GetString(recBuf);
                        arr_Fields = _ReceivedText.Split('|');
                        _ReceivedFieldsLength = arr_Fields.Length;

                        if (_ReceivedFieldsLength > 1)
                        {
                            //added on 25JAN2021 by Amey
                            eve_GatewayStatusReceived(true);
                            eve_LTTReceived(Convert.ToDouble(arr_Fields[0]), Convert.ToDouble(arr_Fields[1]), Convert.ToDouble(arr_Fields[2]), Convert.ToDouble(arr_Fields[3]), Convert.ToDouble(arr_Fields[4]));//added by Omkar
                        }

                        if (_ReceivedFieldsLength > 4)
                            eve_SpanStatusReceived(Convert.ToBoolean(arr_Fields[5]), Convert.ToBoolean(arr_Fields[6]));

                        if (_ReceivedFieldsLength > 6)
                            eve_SpanInfoReceived(Convert.ToDouble(arr_Fields[7]), Convert.ToDouble(arr_Fields[8]), arr_Fields[9], arr_Fields[10]);

                    }
                    else
                        throw new SocketException();
                }
                catch (SocketException se)
                {
                    eve_GatewayStatusReceived(false);
                    eve_Error("GatewayHB : " + se);
                    isConnected = false;

                    //added on 23JUN2021 by Amey
                    ConnectToGateway(_EngineID, _GatewayHBIP, _GatewayHBPort);
                }
                catch (Exception ee) { eve_Error("GatewayHB : " + _ReceivedText + Environment.NewLine + ee); }
            }
        }

        internal void SendToGateway(string _Message)
        {
           
            byte[] buffer = Encoding.UTF8.GetBytes(_Message + "^<EOF>");
            soc_GatewayHB.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }
    }
}
