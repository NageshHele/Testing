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
    
    public delegate void del_nImageBReceived(Dictionary<string, double[]> dict_Span);

    class nImageBDataConnector
    {
        public event del_Error eve_Error;
        public event del_nImageBReceived eve_ConsSpanReceived;

        string _EngineID = "";

        
        int _nImageBSpanPort = 0;

        IPAddress _nImageBServerIP = IPAddress.Any;
                
        int _SpanConnectionAttempts = 0;

        int _MaxConnectionAttempts = 5;

       
        public void ConnectTonImageBSpan(string ID, IPAddress _nImageBIP, int _nImageBPORT)   // connecting to server
        {
            if (ID != "")
            {
                _EngineID = ID;
                _nImageBServerIP = _nImageBIP;
                _nImageBSpanPort = _nImageBPORT;

                var soc_nImageBHB = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                while (!soc_nImageBHB.Connected && (_SpanConnectionAttempts < _MaxConnectionAttempts))
                {
                    try
                    {
                        _SpanConnectionAttempts++;

                        soc_nImageBHB.Connect(_nImageBIP, _nImageBPORT);

                        if (soc_nImageBHB.Connected)
                        {
                            _SpanConnectionAttempts = 0;

                            string str = "ID^" + ID + "^<EOF>";
                            byte[] buffer = Encoding.UTF8.GetBytes(str);
                            soc_nImageBHB.Send(buffer, 0, buffer.Length, SocketFlags.None);

                            Task.Run(() => ReceiveSpanResponse(soc_nImageBHB));
                        }
                    }
                    catch (SocketException ee)
                    {
                        if (_SpanConnectionAttempts < _MaxConnectionAttempts)
                        {
                            eve_Error("Retrying nImageB Span " + _nImageBIP + ":" + _nImageBPORT + Environment.NewLine + ee);
                            Thread.Sleep(10000);
                        }
                        else
                        {
                            //added on 08APR2021 by Amey
                            eve_Error("Unable to connect to the nImageB Span Server. " + _nImageBIP + ":" + _nImageBPORT + Environment.NewLine + ee);

                            _SpanConnectionAttempts = 0;

                            break;
                        }
                    }
                }
            }
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

                            eve_ConsSpanReceived(dict_Span);
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

                    ConnectTonImageBSpan(_EngineID, _nImageBServerIP, _nImageBSpanPort);
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
