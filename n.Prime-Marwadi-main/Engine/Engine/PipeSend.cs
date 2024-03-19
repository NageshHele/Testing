using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    class Sender
    {
        string result = "";
        internal string sendtoSpecificIP(IPAddress ip, DateTime date)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                tcpclnt.Connect(ip, 6600); // use the ipaddress as in the server program

                Console.WriteLine("Connected");
                Console.Write("Enter the string to be transmitted : ");

                String str = "One to one Transmission";
                Stream stm = tcpclnt.GetStream();

                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(date.ToString());
                Console.WriteLine("Transmitting.....");

                stm.Write(ba, 0, ba.Length);

                //byte[] bb = new byte[100];
                //int k = stm.Read(bb, 0, 100);

                //for (int i = 0; i < k; i++)
                //    Console.Write(Convert.ToChar(bb[i]));

                tcpclnt.Close();
            }

            catch (Exception e)
            {
                if (!EngineProcess.exceptionsList.Contains(e.Message))
                {
                    Console.WriteLine("Error while sending : " + e.Message);
                }

            }
            return result;
        }
    }
}
