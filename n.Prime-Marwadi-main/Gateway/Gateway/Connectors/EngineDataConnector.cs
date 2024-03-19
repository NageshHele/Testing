using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Connectors
{
    public class EngineDataConnector
    {
        public event del_ErrorReceived eve_ErrorReceived;//05-02-2020

        private readonly Socket socket_EngineTradeData = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly Socket socket_EngineSpanData = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        Socket soc_EngineTrade = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Socket soc_EngineSpan = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        int _TimeoutMilliseconds = 120000;

        //changed on 13JAN2021 by Amey
        internal void SetupTrade(IPAddress GatewayDataServerIP, int GatewayServerDataPORT, int _TimeoutMilliseconds)
        {
            try
            {
                //added on 23JUN2021 by Amey
                this._TimeoutMilliseconds = _TimeoutMilliseconds;

                //changed on 13JAN2021 by Amey
                socket_EngineTradeData.Bind(new IPEndPoint(GatewayDataServerIP, GatewayServerDataPORT));

                socket_EngineTradeData.Listen(0);
                socket_EngineTradeData.BeginAccept(AcceptCallbackTrade, null);
            }
            catch (Exception error)
            {
                eve_ErrorReceived("Start gateway data socket " + error.ToString());//05-02-2020
            }
        }

        internal void SetupSpan(IPAddress GatewayDataServerIP, int GatewayServerDataPORT, int _TimeoutMilliseconds)
        {
            try
            {
                //added on 23JUN2021 by Amey
                this._TimeoutMilliseconds = _TimeoutMilliseconds;

                //changed on 13JAN2021 by Amey
                socket_EngineSpanData.Bind(new IPEndPoint(GatewayDataServerIP, GatewayServerDataPORT));

                socket_EngineSpanData.Listen(0);
                socket_EngineSpanData.BeginAccept(AcceptCallbackSpan, null);
            }
            catch (Exception error)
            {
                eve_ErrorReceived("Start gateway span socket " + error.ToString());//05-02-2020
            }
        }

        private void AcceptCallbackTrade(IAsyncResult AR)
        {
            try
            {
                Socket soc_Current = socket_EngineTradeData.EndAccept(AR);

                //added on 10MAY2021 by Amey
                Task.Run(() => ReceiveCallbackTrade(soc_Current));

                //soc_Current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, soc_Current);
            }
            catch (ObjectDisposedException ee) { eve_ErrorReceived("AcceptCallBack Trade Disposed : " + ee); return; }
            catch (Exception ee)
            {
                eve_ErrorReceived("AcceptCallBack Data : " + ee);
            }

            socket_EngineSpanData.BeginAccept(AcceptCallbackSpan, null);
        }

        private void AcceptCallbackSpan(IAsyncResult AR)
        {
            try
            {
                Socket soc_Current = socket_EngineSpanData.EndAccept(AR);

                //added on 10MAY2021 by Amey
                Task.Run(() => ReceiveCallbackSpan(soc_Current));

                //soc_Current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, soc_Current);
            }
            catch (ObjectDisposedException ee) { eve_ErrorReceived("AcceptCallBack Span Disposed : " + ee); return; }
            catch (Exception ee)
            {
                eve_ErrorReceived("AcceptCallBack Data : " + ee);
            }

            socket_EngineSpanData.BeginAccept(AcceptCallbackSpan, null);
        }

        private void ReceiveCallbackTrade(Socket soc_Current)
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

                            eve_ErrorReceived("Received Trade Server : " + ProperData);

                            arr_Fields = ProperData.Split('^');
                            if (arr_Fields[0] == "ID" && arr_Fields[1] == "n.ENGINE")
                            {
                                if (!soc_EngineTrade.Connected)
                                {
                                    soc_EngineTrade = soc_Current;

                                    soc_Current.SendTimeout = _TimeoutMilliseconds;
                                }

                                isConnected = false;
                            }
                        }
                    }
                }
                catch (SocketException ee)
                {
                    isConnected = false;

                    eve_ErrorReceived("ReceiveCallBack Trade Socket : " + ee);

                    try
                    {
                        soc_EngineTrade.Close();
                    }
                    catch (Exception) { }

                    soc_Current.Close();
                }
                catch (Exception ee) { eve_ErrorReceived("ReceiveCallBack Trade : " + ee); }
            }
        }

        private void ReceiveCallbackSpan(Socket soc_Current)
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

                            eve_ErrorReceived("Received Span Server : " + ProperData);

                            arr_Fields = ProperData.Split('^');
                            if (arr_Fields[0] == "ID" && arr_Fields[1] == "n.ENGINE")
                            {
                                if (!soc_EngineSpan.Connected)
                                {
                                    soc_EngineSpan = soc_Current;

                                    soc_Current.SendTimeout = _TimeoutMilliseconds;
                                }

                                isConnected = false;
                            }
                        }
                    }
                }
                catch (SocketException ee)
                {
                    isConnected = false;

                    eve_ErrorReceived("ReceiveCallBack Span Socket : " + ee);

                    try
                    {
                        soc_EngineSpan.Close();
                    }
                    catch (Exception) { }

                    soc_Current.Close();
                }
                catch (Exception ee) { eve_ErrorReceived("ReceiveCallBack Span : " + ee); }
            }
        }

        internal bool SendToEngineTrades(string prmValue)
        {
            bool _result = false;

            try
            {
                byte[] buffer = CompressData(prmValue + "^<EOF>");
                if (soc_EngineTrade.Connected)
                {
                    lock (soc_EngineTrade)
                    {
                        soc_EngineTrade.Send(buffer, buffer.Length, SocketFlags.None);
                    }

                    _result = true;
                }
            }
            catch (Exception error)
            {
                eve_ErrorReceived("SendToEngine Trade " + error.ToString());//05-02-2020
            }

            return _result;
        }

        internal bool SendToEngineSpan(string prmValue)
        {
            bool _result = false;

            try
            {
                byte[] buffer = CompressData(prmValue + "^<EOF>");
                if (soc_EngineSpan.Connected)
                {
                    lock (soc_EngineSpan)
                    {
                        soc_EngineSpan.Send(buffer, buffer.Length, SocketFlags.None);
                    }

                    _result = true;
                }
            }
            catch (Exception error)
            {
                eve_ErrorReceived("SendToEngine Span " + error.ToString());//05-02-2020
            }

            return _result;
        }

        private byte[] CompressData(string inputString)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
            try
            {
                //using (var outputStream = new MemoryStream())
                //{
                //    using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
                //        gZipStream.Write(inputBytes, 0, inputBytes.Length);

                //    return outputStream.ToArray();
                //}

                return inputBytes;
            }
            catch (Exception error)
            {
                eve_ErrorReceived("Compression " + error.ToString());
                return null;
            }
        }

    }
}
