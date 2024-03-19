using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Gateway
{
    public delegate void del_ErrorReceived(string message, bool isDebug = false);//05-02-2020
    public delegate void del_SignalReceived(string _Signal);//05-02-2020

    class EngineHBConnector
    {
        private readonly Socket socket_EngineHeartBeat = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public event del_ErrorReceived eve_ErrorReceived;
        public event del_SignalReceived eve_SignalReceived;

        Socket soc_Engine = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        int _TimeoutMilliseconds = 120000;

        //changed on 13JAN2021 by Amey
        public void Setup(IPAddress GatewayHBServerIP, int GatewayServerHBPORT, int _TimeoutMilliseconds)
        {
            try
            {
                //added on 23JUN2021 by Amey
                this._TimeoutMilliseconds = _TimeoutMilliseconds;

                //changed on 13JAN2021 by Amey
                socket_EngineHeartBeat.Bind(new IPEndPoint(GatewayHBServerIP, GatewayServerHBPORT));

                socket_EngineHeartBeat.Listen(0);
                socket_EngineHeartBeat.BeginAccept(AcceptCallback, null);
            }
            catch (Exception error)
            {
                eve_ErrorReceived("Start gateway socket " + error.ToString());//05-02-2020
            }
        }

        private void AcceptCallback(IAsyncResult AR)
        {
            try
            {
                Socket soc_Current = socket_EngineHeartBeat.EndAccept(AR);

                //added on 10MAY2021 by Amey
                Task.Run(() => ReceiveCallback(soc_Current));

                //soc_Current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, soc_Current);
            }
            catch (ObjectDisposedException ee) { eve_ErrorReceived("AcceptCallBack Disposed : " + ee); return; }
            catch (Exception ee)
            {
                eve_ErrorReceived("AcceptCallBack : " + ee);
            }

            socket_EngineHeartBeat.BeginAccept(AcceptCallback, null);
        }

        //changed on 13JAN2021 by Amey
        /// <summary>
        /// Will send HeartBeat to Engine.
        /// </summary>
        /// <param name="prmValue">FOLastTradeTime(Unix)|CMLastTradeTime(Unix)|IsSpanConnected(bool)</param>
        public bool SendToEngine(string prmValue)
        {
            bool _result = false;

            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(prmValue + "|");
                if (soc_Engine.Connected)
                {
                    lock (soc_Engine)
                    {
                        soc_Engine.Send(buffer, buffer.Length, SocketFlags.None);
                    }

                    _result = true;
                }
            }
            catch (Exception error)
            {
                eve_ErrorReceived("SendToEngine " + error.ToString());//05-02-2020
            }

            return _result;
        }

        private void ReceiveCallback(Socket soc_Current)
        {
            var isConnected = true;
            int _ReceivedBytesLength;

            byte[] arr_Buffer = new byte[2048];
            byte[] arr_BytesReceived;
            string[] arr_Fields;
            string EOF = "<EOF>";
            int EOFIndex = 0;
            int EOFLength = EOF.Length;
            string ProperData = string.Empty;
            string PreviousDataHB = string.Empty;

            while (isConnected)
            {
                try
                {
                    _ReceivedBytesLength = soc_Current.Receive(arr_Buffer, SocketFlags.None);

                    if (_ReceivedBytesLength > 0)
                    {
                        arr_BytesReceived = new byte[_ReceivedBytesLength];
                        Array.Copy(arr_Buffer, arr_BytesReceived, _ReceivedBytesLength);

                        PreviousDataHB += Encoding.UTF8.GetString(arr_BytesReceived);

                        while ((EOFIndex = PreviousDataHB.IndexOf(EOF)) >= 0)
                        {
                            //added on 03MAY2021 by Amey
                            //To avoid "System.ArgumentOutOfRangeException: Length cannot be less than zero." exception.
                            //EOFIndex = PreviousDataSpan.IndexOf(EOF);
                            while (EOFIndex == 0)
                            {
                                PreviousDataHB = PreviousDataHB.Substring(EOFIndex + EOFLength);
                                EOFIndex = PreviousDataHB.IndexOf(EOF);
                            }

                            if (EOFIndex < 0)
                                continue;

                            ProperData = PreviousDataHB.Substring(0, EOFIndex - 1);
                            PreviousDataHB = PreviousDataHB.Substring(EOFIndex + EOF.Length);

                            eve_ErrorReceived("Received HB Server : " + ProperData);

                            arr_Fields = ProperData.Split('^');
                            if (arr_Fields[0] == "ID" && arr_Fields[1] == "n.ENGINE")
                            {
                                if (!soc_Engine.Connected)
                                {
                                    soc_Engine = soc_Current;

                                    soc_Current.SendTimeout = _TimeoutMilliseconds;
                                }
                            }
                            else if (arr_Fields[0] == "SIGNAL")
                            {
                                //data => SIGNAL^START/STOP
                                eve_SignalReceived(arr_Fields[1]);
                            }
                        }
                    }
                }
                catch (SocketException ee)
                {
                    isConnected = false;

                    eve_ErrorReceived("ReceiveCallBack HB Socket : " + ee);

                    try
                    {
                        soc_Engine.Close();
                    }
                    catch (Exception) { }

                    soc_Current.Close();
                }
                catch (Exception ee) { eve_ErrorReceived("ReceiveCallBack HB : " + ee); }
            }
        }
    }
}
