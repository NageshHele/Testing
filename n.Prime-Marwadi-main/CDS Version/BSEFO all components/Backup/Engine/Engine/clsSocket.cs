using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Engine
{
    public delegate void Response1(double prmTick);
    public delegate void ConnectionResponse(string prmMessage);
    public delegate void SpanResponse(string prmMessage);
    class clsSocket
    {
        Socket ReceiveSocket = null;
        public event Response1 _LastTradetimeResponse;
        public event ConnectionResponse _ConnectionResponse;
        public event SpanResponse _SpanResponse;

        private void ReceiveFOResponse(object obj)
        {
            Socket ClientSocket = (Socket)obj;
            try
            {
                Console.WriteLine("waiting for feed..");
                while (ClientSocket.Connected)
                {
                    var buffer = new byte[2048];      //commented on 14-11-18 by Amey
                    int received = ClientSocket.Receive(buffer, SocketFlags.None);
                    if (received == 0) return;

                    if (received > 0)
                    {
                        byte[] recBuf = new byte[received];
                        Array.Copy(buffer, recBuf, received);
                        string _data = Encoding.ASCII.GetString(recBuf);
                        string[] _ReceivedData = _data.Split('|');
                        if (_ReceivedData.Length > 0)
                        {
                            _LastTradetimeResponse(Convert.ToDouble(_ReceivedData[0]));
                        }
                        if (_ReceivedData.Length > 2)//added by Navin on 05-02-2020
                        {
                            _SpanResponse((_ReceivedData[2]));
                        }
                        //Console.WriteLine(_data);
                        //_LastTradetimeResponse(Convert.ToDouble(Encoding.ASCII.GetString(recBuf)));
                    }
                }// end while socket connected
            }
            catch (SocketException)
            {
                _ConnectionResponse("Engine Disconnected");
            }
            //catch (Exception e)
            //{
            //    Console.WriteLine("Exception while receiving " + e.ToString());
            //}
        }

        static int FOattempts = 0;
        private Thread thReceive;

        public void ConnectToGateway(string cId, string FOipAddress, int FOPORT)   // connecting to server
        {
            if (cId != "")
            {
                ReceiveSocket = new Socket
                (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                while (!ReceiveSocket.Connected && (FOattempts <= 5))
                {
                    try
                    {
                        FOattempts++;
                        Console.WriteLine("Connection attempt " + FOattempts);
                        ReceiveSocket.Connect(FOipAddress, FOPORT);
                        if (ReceiveSocket.Connected)
                        {
                            string str = "^" + cId + "^" + "<EOF>";
                            byte[] buffer = Encoding.ASCII.GetBytes(str);
                            ReceiveSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                            ParameterizedThreadStart paraThread = new ParameterizedThreadStart(ReceiveFOResponse);
                            // Paas the object ParameterizedThreadStart in Thread constructor  
                            thReceive = new Thread(paraThread);
                            //Pass the parameter into child thread
                            thReceive.Start(ReceiveSocket);
                            
                            //_ResponseStatus("Successful FO");
                        }
                    }
                    catch (SocketException err)
                    {
                        if (FOattempts <= 5)
                        {
                            //_ResponseStatus("Retrying FO");
                            Thread.Sleep(5000);
                            ConnectToGateway(cId, FOipAddress, FOPORT);
                        }
                        else
                        {
                            //_ResponseStatus("Failed FO");
                            ReceiveSocket.Close();
                            break;
                        }
                    }
                }
            }
        }
    }
}
