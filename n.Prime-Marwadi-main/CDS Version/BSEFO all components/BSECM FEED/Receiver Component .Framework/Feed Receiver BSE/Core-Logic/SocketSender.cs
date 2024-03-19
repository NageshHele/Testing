using Feed_Receiver_BSE.Data_Structures;
using Feed_Receiver_BSE.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Feed_Receiver_BSE.Core_Logic
{
    internal class SocketSender
    {
        const string _Seperator = "^";
        const string _EOF = "<EOF>";

        internal static void SendToClient(SocketInfo _SocketInfo, int _Token)
        {
            try
            {
                if (CollectionHelper.dict_LastPacket.ContainsKey(_Token))
                {
                    var _Packet = CollectionHelper.dict_LastPacket[_Token];

                    StringBuilder sb = new StringBuilder();
                    sb.Append("7208" + _Seperator);
                    sb.Append(_Token + _Seperator);
                    sb.Append(_Packet.Open + _Seperator);
                    sb.Append(_Packet.High + _Seperator);
                    sb.Append(_Packet.Low + _Seperator);
                    sb.Append(_Packet.Close + _Seperator);
                    sb.Append(_Packet.PreviousClose + _Seperator);
                    sb.Append(_Packet.LTP + _Seperator);
                    sb.Append(_Packet.LTQ + _Seperator);
                    sb.Append(_Packet.Volume + _Seperator);
                    sb.Append(_Packet.AvgPrice + _Seperator);
                    sb.Append(_Packet.list_BidAskDepth + _Seperator);

                    foreach (var item in _Packet.list_BidAskDepth)
                    {
                        for (int i = 0; i < item.Length; i++)
                        {
                            sb.Append(item[i] + "|");
                        }
                        sb.Append(_Seperator);
                    }

                    sb.Append(_EOF);

                    byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
                    lock (_SocketInfo.Client)
                    {
                        _SocketInfo.Client.Send(buffer, buffer.Length, SocketFlags.None);
                    }
                }
            }
            catch (Exception ee) { CollectionHelper._logger.Error(ee); }
        }
    }
}
