using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace Gateway
{
    class BODUtilityConnector                                           // Added by Snehadri on 30JUN2021 for Automatic BOD Process
    {

        public bool IsStarted = false;
        public static clsWriteLog _logger = new clsWriteLog(Application.StartupPath.ToString() + "\\Log");
        public bool StartClient()
        {
            try
            {
                // For Socket Connection 
                DataSet ds_MainConfig = new DataSet();
                XmlTextReader tReader = new XmlTextReader("C:/Prime/Config.xml");
                tReader.Read();
                ds_MainConfig.ReadXml(tReader);
                var conn_info = ds_MainConfig.Tables["CONNECTION"].Rows[0];

                string IP = conn_info["BOD-IP"].ToString();
                int PORT = int.Parse(conn_info["BOD-PORT"].ToString());
                byte[] bytes = new byte[1024];

                IPAddress ipAddress = IPAddress.Parse(IP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, PORT);
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                string text = null;

                try
                {
                    while (!IsStarted)
                    {
                        sender.Connect(remoteEP);
                        byte[] msg = Encoding.ASCII.GetBytes("GatewayStarted");

                        int bytesSent = sender.Send(msg);

                        int bytesRec = sender.Receive(bytes);
                        text = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (text == "START")
                            IsStarted = true;

                        sender.Shutdown(SocketShutdown.Both);
                        sender.Close();
                    }

                    if (text == "START")
                        return true;
                    else
                    {
                        sender.Close();
                        return false;
                    }
                }
                catch (Exception error)
                {
                    sender.Close();
                    _logger.Error("Sending Message Error in BODUtilityConnector: " + error.ToString());
                    return false;
                }

            }
            catch (Exception error)
            {
                _logger.Error("Socket Error in BODUtilityConnector: " + error.ToString()); ;
                return false;
            }
        }
    }
}