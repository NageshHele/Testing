using n.Structs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Connectors
{
    public delegate void del_TradesReceived(List<PositionInfo> list_AllTrades);
    public delegate void del_SpanReceived(string flag, Dictionary<string, double[]> dict_Span);

    public class GatewayDataConnector
    {
        public event del_Error eve_Error;
        public event del_TradesReceived eve_TradesReceived;
        public event del_SpanReceived eve_SpanReceived;

        string _EngineID = "";

        int _GatewayTradePort = 0;
        int _GatewaySpanPort = 0;

        IPAddress _GatewayServerIP = IPAddress.Any;

        int _TradeConnectionAttempts = 0;
        int _SpanConnectionAttempts = 0;

        int _MaxConnectionAttempts = 5;

        public void ConnectToGatewayTrade(string ID, IPAddress _GatewayIP, int _GatewayPORT)   // connecting to server
        {
            if (ID != "")
            {
                _EngineID = ID;
                _GatewayServerIP = _GatewayIP;
                _GatewayTradePort = _GatewayPORT;

                var soc_GatewayHB = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                while (!soc_GatewayHB.Connected && (_TradeConnectionAttempts < _MaxConnectionAttempts))
                {
                    try
                    {
                        _TradeConnectionAttempts++;

                        soc_GatewayHB.Connect(_GatewayIP, _GatewayPORT);

                        if (soc_GatewayHB.Connected)
                        {
                            _TradeConnectionAttempts = 0;

                            string str = "ID^" + ID + "^<EOF>";
                            byte[] buffer = Encoding.UTF8.GetBytes(str);
                            soc_GatewayHB.Send(buffer, 0, buffer.Length, SocketFlags.None);

                            Task.Run(() => ReceiveTradeResponse(soc_GatewayHB));
                        }
                    }
                    catch (SocketException ee)
                    {
                        if (_TradeConnectionAttempts < _MaxConnectionAttempts)
                        {
                            eve_Error("Retrying " + _GatewayIP + ":" + _GatewayPORT + Environment.NewLine + ee);
                            Thread.Sleep(10000);
                        }
                        else
                        {
                            //added on 08APR2021 by Amey
                            eve_Error("Unable to connect to the Gateway Server. " + _GatewayIP + ":" + _GatewayPORT + Environment.NewLine + ee);

                            _TradeConnectionAttempts = 0;

                            break;
                        }
                    }
                }
            }
        }

        public void ConnectToGatewaySpan(string ID, IPAddress _GatewayIP, int _GatewayPORT)   // connecting to server
        {
            if (ID != "")
            {
                _EngineID = ID;
                _GatewayServerIP = _GatewayIP;
                _GatewaySpanPort = _GatewayPORT;

                var soc_GatewayHB = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                while (!soc_GatewayHB.Connected && (_SpanConnectionAttempts < _MaxConnectionAttempts))
                {
                    try
                    {
                        _SpanConnectionAttempts++;

                        soc_GatewayHB.Connect(_GatewayIP, _GatewayPORT);

                        if (soc_GatewayHB.Connected)
                        {
                            _SpanConnectionAttempts = 0;

                            string str = "ID^" + ID + "^<EOF>";
                            byte[] buffer = Encoding.UTF8.GetBytes(str);
                            soc_GatewayHB.Send(buffer, 0, buffer.Length, SocketFlags.None);

                            Task.Run(() => ReceiveSpanResponse(soc_GatewayHB));
                        }
                    }
                    catch (SocketException ee)
                    {
                        if (_SpanConnectionAttempts < _MaxConnectionAttempts)
                        {
                            eve_Error("Retrying Gateway Span " + _GatewayIP + ":" + _GatewayPORT + Environment.NewLine + ee);
                            Thread.Sleep(10000);
                        }
                        else
                        {
                            //added on 08APR2021 by Amey
                            eve_Error("Unable to connect to the Gateway Span Server. " + _GatewayIP + ":" + _GatewayPORT + Environment.NewLine + ee);

                            _SpanConnectionAttempts = 0;

                            break;
                        }
                    }
                }
            }
        }

        private void ReceiveTradeResponse(Socket soc_Current)
        {
            string EOF = "<EOF>";
            int EOFIndex = 0;
            int EOFLength = EOF.Length;

            var list_AllTrades = new List<PositionInfo>();

            var arr_Buffer = new byte[67108864];
            int _ReceivedBytesLength = 0;
            byte[] arr_BytesReceived;

            string PreviousDataTrade = string.Empty;
            string ProperDataTrade = string.Empty;

            var isConnected = true;

            while (isConnected)
            {
                try
                {
                    _ReceivedBytesLength = soc_Current.Receive(arr_Buffer, SocketFlags.None);

                    if (_ReceivedBytesLength > 0)
                    {
                        arr_BytesReceived = new byte[_ReceivedBytesLength];
                        Array.Copy(arr_Buffer, arr_BytesReceived, _ReceivedBytesLength);

                        //Console.WriteLine("Received : " + DateTime.Now.ToString("HH:mm:ss:fff"));
                        PreviousDataTrade += DecompressData(arr_BytesReceived);
                        //Console.WriteLine("DecompressAndDecryptData : " + DateTime.Now.ToString("HH:mm:ss:fff"));

                        while ((EOFIndex = PreviousDataTrade.IndexOf(EOF)) >= 0)
                        {
                            //added on 03MAY2021 by Amey
                            //To avoid "System.ArgumentOutOfRangeException: Length cannot be less than zero." exception.
                            //EOFIndex = PreviousDataTrade.IndexOf(EOF);
                            while (EOFIndex == 0)
                            {
                                PreviousDataTrade = PreviousDataTrade.Substring(EOFIndex + EOFLength);
                                EOFIndex = PreviousDataTrade.IndexOf(EOF);
                            }

                            if (EOFIndex < 0)
                                continue;

                            ProperDataTrade = PreviousDataTrade.Substring(0, EOFIndex - 1);
                            PreviousDataTrade = PreviousDataTrade.Substring(EOFIndex + EOFLength);

                            list_AllTrades = (List<PositionInfo>)JsonConvert.DeserializeObject(ProperDataTrade, list_AllTrades.GetType());

                            eve_TradesReceived(list_AllTrades);
                        }
                    }
                    else
                        throw new SocketException();
                }
                catch (JsonReaderException) { }
                catch (SocketException ee)
                {
                    isConnected = false;

                    PreviousDataTrade = "";

                    eve_Error("ReceiveTradeResponse Received- " + ProperDataTrade.Length + Environment.NewLine + ee);

                    ConnectToGatewayTrade(_EngineID, _GatewayServerIP, _GatewayTradePort);
                }
                catch (Exception ee)
                {
                    eve_Error("ReceiveTradeResponse Received- " + ProperDataTrade.Length + Environment.NewLine + ee);

                    //added on 12APR2021 by Amey
                    PreviousDataTrade = "";
                }
            }// end while socket connected
        }

        private void ReceiveSpanResponse(Socket soc_Current)
        {
            string EOF = "<EOF>";
            int EOFIndex = 0;
            int EOFLength = EOF.Length;

            var dict_Span = new Dictionary<string, double[]>();

            var arr_Buffer = new byte[67108864];
            int _ReceivedBytesLength = 0;
            byte[] arr_BytesReceived;

            string[] arr_Fields;

            string PreviousDataSpan = string.Empty;
            string ProperDataSpan = string.Empty;

            var isConnected = true;

            while (isConnected)
            {
                try
                {
                    _ReceivedBytesLength = soc_Current.Receive(arr_Buffer, SocketFlags.None);

                    if (_ReceivedBytesLength > 0)
                    {
                        arr_BytesReceived = new byte[_ReceivedBytesLength];
                        Array.Copy(arr_Buffer, arr_BytesReceived, _ReceivedBytesLength);

                        //Console.WriteLine("Received : " + DateTime.Now.ToString("HH:mm:ss:fff"));
                        PreviousDataSpan += DecompressData(arr_BytesReceived);
                        //Console.WriteLine("DecompressAndDecryptData : " + DateTime.Now.ToString("HH:mm:ss:fff"));

                        while ((EOFIndex = PreviousDataSpan.IndexOf(EOF)) >= 0)
                        {
                            //added on 03MAY2021 by Amey
                            //To avoid "System.ArgumentOutOfRangeException: Length cannot be less than zero." exception.
                            //EOFIndex = PreviousDataTrade.IndexOf(EOF);
                            while (EOFIndex == 0)
                            {
                                PreviousDataSpan = PreviousDataSpan.Substring(EOFIndex + EOFLength);
                                EOFIndex = PreviousDataSpan.IndexOf(EOF);
                            }

                            if (EOFIndex < 0)
                                continue;

                            ProperDataSpan = PreviousDataSpan.Substring(0, EOFIndex - 1);
                            PreviousDataSpan = PreviousDataSpan.Substring(EOFIndex + EOFLength);

                            arr_Fields = ProperDataSpan.Split('^');

                            dict_Span = (Dictionary<string, double[]>)JsonConvert.DeserializeObject(arr_Fields[1], dict_Span.GetType());

                            eve_SpanReceived(arr_Fields[0], dict_Span);
                        }
                    }
                    else
                        throw new SocketException();
                }
                catch (JsonReaderException) { }
                catch (SocketException ee)
                {
                    isConnected = false;

                    PreviousDataSpan = "";

                    eve_Error("ReceiveSpanResponse Received- " + ProperDataSpan.Length + Environment.NewLine + ee);

                    ConnectToGatewaySpan(_EngineID, _GatewayServerIP, _GatewaySpanPort);
                }
                catch (Exception ee)
                {
                    eve_Error("ReceiveSpanResponse Received- " + ProperDataSpan.Length + Environment.NewLine + ee);

                    //added on 12APR2021 by Amey
                    PreviousDataSpan = "";
                }
            }// end while socket connected
        }

        string DecompressData(byte[] inputBytes)
        {
            try
            {
                //using (var inputStream = new MemoryStream(inputBytes))
                //using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                //using (var streamReader = new StreamReader(gZipStream))
                //{
                //    return streamReader.ReadToEnd();
                //}

                return Encoding.UTF8.GetString(inputBytes);
            }
            catch (InvalidDataException) { return string.Empty; }
            catch (Exception ee)
            {
                eve_Error("DecompressData : " + ee);
                return string.Empty;
            }
        }
    }
}
