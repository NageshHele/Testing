using Feed_Receiver_BSE.Data_Structures;
using Ionic.Zip;
using NerveLog;
using NSEUtilitaire;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Feed_Receiver_BSE.Helper
{
    internal class ReadFiles
    {
        NerveLogger _logger;

        internal ReadFiles()
        {
            _logger = CollectionHelper._logger;
        }

        internal void ReadBhavcopy()
        {
            try
            {
                var BHAVCOPYPATH = CollectionHelper.GetFromConfig("FILE-PATH", "BHAVCOPY").ToString();

                try
                {
                    string[] files = Directory.GetFiles(BHAVCOPYPATH, "*.zip");
                    foreach (var _file in files)
                    {
                        using (ZipFile zip = ZipFile.Read(_file))
                            zip.ExtractAll(BHAVCOPYPATH, ExtractExistingFileAction.DoNotOverwrite);

                        File.Delete(_file);
                    }
                }
                catch (Exception) { }

                var arr_BSECMBhavcopy = Directory.GetFiles(BHAVCOPYPATH, "EQ_ISINCODE_*.csv");
                if (arr_BSECMBhavcopy.Length == 0) return;

                var list_BidAskDepth = new List<double[]>();
                for (int i = 0; i < 5; i++)
                    list_BidAskDepth.Add(new double[4] { 0, 0, 0, 0 });

                CollectionHelper.dict_LastPacket = new ConcurrentDictionary<int, Packet>(Exchange.ReadBSECMBhavcopy(arr_BSECMBhavcopy[0]).ToDictionary(k => k.Token, v => new Packet() { LTP = v.Close, Close = v.Close, LTQ = v.TotalTrades, list_BidAskDepth = list_BidAskDepth }));
                CollectionHelper.dict_SubscribedClients = new ConcurrentDictionary<int, HashSet<string>>(CollectionHelper.dict_LastPacket.ToDictionary(k => k.Key, v => new HashSet<string>()));
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        internal void ReadIndexClosing()
        {
            try
            {
                var dict_IndexClose = new Dictionary<string, double>();

                var IndexClosnigFilePath = CollectionHelper.GetFromConfig("FILE-PATH", "INDEX-CLOSE").ToString();

                var _Closingfile = new DirectoryInfo(IndexClosnigFilePath).GetFiles("index*_*").OrderByDescending(x => x.LastWriteTime).FirstOrDefault();
                if (_Closingfile == null)
                    return;

                var arr_lines = File.ReadAllLines(_Closingfile.FullName);

                foreach(var line in arr_lines)
                {
                    try
                    {
                        var Arr_Fields = line.Split('|');
                        if (!dict_IndexClose.ContainsKey(Arr_Fields[0]))
                        {
                            dict_IndexClose.Add(Arr_Fields[0], Convert.ToDouble(Arr_Fields[1]) / 100);
                        }
                    }
                    catch(Exception ee) { _logger.Error(ee); }
                    
                }

                var IndexTokenFile = CollectionHelper.GetFromConfig("FILE-PATH", "INDEX-TOKEN").ToString();
                var arr_Lines = File.ReadAllLines(IndexTokenFile +"\\IndexTokens.csv");

                var list_BidAskDepth = new List<double[]>();
                for (int i = 0; i < 5; i++)
                    list_BidAskDepth.Add(new double[4] { 0, 0, 0, 0 });

                _logger.Debug("Reading index token file");
                foreach (var Line in arr_Lines)
                {
                    try
                    {

                        var arr_Fields = Line.Trim().Split(',');
                        if (arr_Fields[3].Trim() == "BSE" && arr_Fields[4] != "")
                        {
                            if (dict_IndexClose.TryGetValue(arr_Fields[4], out double Closing))
                            {
                                CollectionHelper.dict_LastPacket.TryAdd(Convert.ToInt32(arr_Fields[1]), new Packet() { LTP = Closing, Close = Closing, LTQ = 1, list_BidAskDepth = list_BidAskDepth });
                                CollectionHelper.dict_SubscribedClients.TryAdd(Convert.ToInt32(arr_Fields[1]), new HashSet<string>());
                                CollectionHelper.dict_IndexToken.TryAdd(arr_Fields[0], Convert.ToInt32(arr_Fields[1]));
                                _logger.Debug("IndexToken entry added | " + arr_Fields[0] + "|" + Convert.ToInt32(arr_Fields[1]));
                            }
                        }
                    }
                    catch(Exception ee) { _logger.Error(ee); }
                   
                }
            }catch(Exception ee) { _logger.Error(ee); }
        }



        //internal void ReadSecurity()
        //{
        //    try
        //    {
        //        var SECURITYPATH = CollectionHelper.GetFromConfig("FILE-PATH", "SECURITY").ToString();
        //        var arr_BSECMSecurity = Directory.GetFiles(SECURITYPATH, "*.txt");
        //        if (arr_BSECMSecurity.Length == 0) return;

        //        var list_BSECMSecurity = Exchange.ReadBSESecurity(arr_BSECMSecurity[0]);
        //    }
        //    catch (Exception ee) { _logger.Error(ee); }
        //}
    }
}
