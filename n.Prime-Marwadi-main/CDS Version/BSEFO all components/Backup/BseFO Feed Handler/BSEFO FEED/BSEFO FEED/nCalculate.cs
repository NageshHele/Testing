using NerveLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

internal class nCalculate
{
    public static nCalculate Instance { get; private set; }

    public static void Initialise()
    {
        if (Instance is null)
            Instance = new nCalculate();
    }

    public static NerveLogger _logger;

    internal void StartSending()
    {
        DisableConsoleQuickEdit.Go();

        var stopwatch = new Stopwatch();

        while (true)
        {
            stopwatch.Start();

            try
            {
                var list_Tokens = GlobalCollections.dict_SubscribedClients.Keys.ToList();

                foreach (var _Token in list_Tokens)
                {                   
                    if (CreatePacketToClient(_Token, out byte[] arr_Buffer))
                    {
                        if (GlobalCollections.dict_SubscribedClients.TryGetValue(_Token, out List<Socket> list_Clients))
                        {
                            foreach (var soc_Client in list_Clients)
                            {
                                try
                                {
                                    soc_Client.Send(arr_Buffer, arr_Buffer.Length, SocketFlags.None);
                                }
                                catch (Exception) { GlobalCollections.dict_SubscribedClients[_Token].Remove(soc_Client); }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            stopwatch.Stop();
            var elapsed_time = stopwatch.ElapsedMilliseconds;

            stopwatch.Reset();

            int waittime = 800;
            try
            {
                waittime = waittime - Convert.ToInt32(elapsed_time);
                waittime = waittime < 0 ? 0 : waittime;
            }
            catch (OverflowException) { }
            catch (Exception) { }

            Thread.Sleep(waittime);
        }
    }

    static class DisableConsoleQuickEdit
    {

        const uint ENABLE_QUICK_EDIT = 0x0040;

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        internal static bool Go()
        {

            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // get current console mode
            if (!GetConsoleMode(consoleHandle, out uint consoleMode))
            {
                // ERROR: Unable to get console mode.
                return false;
            }

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~ENABLE_QUICK_EDIT;

            // set the new mode
            if (!SetConsoleMode(consoleHandle, consoleMode))
            {
                // ERROR: Unable to set console mode
                return false;
            }

            return true;
        }
    }

    #region Supplimentary Methods

    private bool CreatePacketToClient(int _Token, out byte[] arr_Buffer)
    {
        var _Result = false;
        arr_Buffer = new byte[0];

        try
        {
            //sending format -> feed- libraray 
            if (_Result = GlobalCollections.dict_LastPacket.TryGetValue(_Token, out var _Packet))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(CreateTransferString(7208)); //0
                sb.Append(CreateTransferString(_Token)); //1
                sb.Append(CreateTransferString(_Packet.LTP)); //2
                sb.Append(CreateTransferString(_Packet.LTQ));//3
                sb.Append(CreateTransferString(_Packet.LTT));//4
                sb.Append(CreateTransferString(_Packet.Open));//5
                sb.Append(CreateTransferString(_Packet.High));//6
                sb.Append(CreateTransferString(_Packet.Low));//7
                sb.Append(CreateTransferString(_Packet.Close));//8
                sb.Append(CreateTransferString(_Packet.TotalBuyQty));//9
                sb.Append(CreateTransferString(_Packet.TotalSellQty));//10
                sb.Append(CreateTransferString(_Packet.AvgPrice));//11
                sb.Append(CreateTransferString(_Packet.TotalTrades));//12
                sb.Append(CreateTransferString(_Packet.OpenInterest));//13
                sb.Append(CreateTransferString(_Packet.Multiplier));//14
                sb.Append(CreateTransferString(_Packet.list_BidAskDepth));//15
               
                sb.Append(GlobalCollections._EOF);

                arr_Buffer = Encoding.ASCII.GetBytes(sb.ToString());
            }
        }
        catch (Exception ee) { _logger.Error(ee); }

        return _Result;
    }

    private string CreateTransferString(object Val) => Val + GlobalCollections._Seperator;
   
    #endregion
}