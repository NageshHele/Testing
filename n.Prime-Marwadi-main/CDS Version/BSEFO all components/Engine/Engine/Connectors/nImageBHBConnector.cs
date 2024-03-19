using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Engine
{
    
    public delegate void del_nImageBStatusReceived(bool isnImageBConnected);
    
    public delegate void del_nImageB_Error(string _Message);
    

    class nImageBHBConnector
    {
        Socket soc_nImageBHB = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        Socket ReceiveSocket = null;

        
        public event del_nImageBStatusReceived eve_nImageBStatusReceived;
       
        public event del_nImageB_Error eve_nImageBError;
        

        string _EngineID = "";
        int _nImageBHBPort = 0;
        IPAddress _nImageBHBIP = IPAddress.Any;
        int _HBConnectionAttempts = 0;
        int _MaxConnectionAttempts = 5;

        int _Reattempts = 0;

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
                        
                        if (_ReceivedText == "Connect Engine")
                        {
                            //added on 25JAN2021 by Amey
                            //eve_nImageBStatusReceived(true);                            
                        }
                                                                        
                    }
                    else
                        throw new SocketException();
                }
                catch (SocketException se)
                {
                    //eve_nImageBStatusReceived(false);
                    eve_nImageBError("nImageBHB : " + se);
                    isConnected = false;

                    //added on 23JUN2021 by Amey
                    ConnectTonImageB(_EngineID, _nImageBHBIP, _nImageBHBPort);
                }
                catch (Exception ee) { eve_nImageBError("nImageBHB : " + _ReceivedText + Environment.NewLine + ee); }
            }
        }



        public void ConnectTonImageB(string ID, IPAddress _nImageBIP, int _nImageBPORT)   // connecting to server
        {
            if (ID != "")
            {
                _EngineID = ID;
                _nImageBHBIP = _nImageBIP;
                _nImageBHBPort = _nImageBPORT;

                soc_nImageBHB = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                while (!soc_nImageBHB.Connected && (_HBConnectionAttempts < _MaxConnectionAttempts))
                {
                    try
                    {
                        _HBConnectionAttempts++;

                        soc_nImageBHB.Connect(_nImageBIP, _nImageBPORT);

                        if (soc_nImageBHB.Connected)
                        {
                            _HBConnectionAttempts = 0;

                            //added on 25JAN2021 by Amey
                           // eve_nImageBStatusReceived(true);

                            SendTonImageB("ID^" + ID);

                            Task.Run(() => ReceiveResponse(soc_nImageBHB));
                        }
                    }
                    catch (SocketException ee)
                    {
                        if (_HBConnectionAttempts < _MaxConnectionAttempts)
                        {
                            eve_nImageBError("Retrying " + _nImageBIP + ":" + _nImageBPORT + Environment.NewLine + ee);
                            Thread.Sleep(10000);
                        }
                        else
                        {
                            //added on 08APR2021 by Amey
                            eve_nImageBError("Unable to connect to the nImageB Server. " + _nImageBIP + ":" + _nImageBPORT + Environment.NewLine + ee);

                            _HBConnectionAttempts = 0;

                            break;
                        }
                    }
                }
            }
        }

        internal bool SendTonImageB(string _Message)
        {
            try
            {
                if (soc_nImageBHB.Connected)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(_Message + "^<EOF>");
                    soc_nImageBHB.Send(buffer, 0, buffer.Length, SocketFlags.None);
                    return true;
                }
                else { return false; }
            }
            catch(Exception ee) { eve_nImageBError(ee.ToString()); return false; }
        }
    }
}
