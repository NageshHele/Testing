using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Engine
{
    //Name of Sub-client: EOP
    public class EODProcess
    {
        clsWriteLog objWriteLog;//object of write log added by navin on 28-06-2018
        public bool closingDne = false;    //added on 9-12-17 by shri
        char[] b;
        DataTable dayOne = new DataTable();
        DataTable endTrades = new DataTable();
        DataTable Closing = new DataTable();
        DataTable clientdetail = new DataTable();
        DataSet ConnectionStrings = new DataSet();
        DataSet EOD = new DataSet();
        MySqlConnection arrcsDB = new MySqlConnection();
        StringBuilder errorString = new StringBuilder();
        string Exdate,scrip,scripCM,strike;
        int Thursday;
        public static string[,] tradeInfoEOD = new string[99000, 22];
        public int record = 0;
        bool eodStatus = false;

        public EODProcess()
        {
            endTrades = EOD.Tables.Add("endTrades");
            
            //Previous Day Data
            dayOne.Columns.Add("Client");
            dayOne.Columns.Add("ScripToken");
            dayOne.Columns.Add("BEP");
            dayOne.Columns.Add("ScripName");
            dayOne.Columns.Add("Underlying");
            dayOne.Columns.Add("OptionType");
            dayOne.Columns.Add("ScripExpiry");
            dayOne.Columns.Add("StrikePrice");
            dayOne.Columns.Add("InstrumentName");
            dayOne.Columns.Add("NetPosition");
            dayOne.Columns.Add("UnderlyingFuture");
            // dayOne.Columns.Add("FlatScripName");

            objWriteLog  = new clsWriteLog(Application.StartupPath + "\\Log");
        }
        public void GetMainSPACEConnection()
        {
            try
            {
                XmlTextReader tReader = new XmlTextReader("C:/Prime/PrimeDBConnection.xml");
                tReader.Read();
                ConnectionStrings.ReadXml(tReader);
                //ConnectionStrings.Tables[0].Rows[0]["user"] = "Engine"; //added by navin on 02-07-2018
                arrcsDB = new MySqlConnection("Data Source=" + ConnectionStrings.Tables[0].Rows[0]["Server"] + "; Port=" + ConnectionStrings.Tables[0].Rows[0]["Port"] + "; Initial Catalog=" + ConnectionStrings.Tables[0].Rows[0]["Database"] + "; user ID=" + ConnectionStrings.Tables[0].Rows[0]["user"] + "; Password=" + ConnectionStrings.Tables[0].Rows[0]["password"] + " ;SslMode=none ;default command timeout=100;");
                arrcsDB.Open();
            }
            catch (Exception bind)
            {
                InsertError(bind.Message+":"+bind.StackTrace.ToString().Substring(bind.StackTrace.ToString().Length - 10));
            }

        }
        public void DoEOD()
        {
            try
            {
                GetMainSPACEConnection();
                ReadClosing();
                if (closingDne == true)
                {
                    GetClient();
                    ReadPreviousDayFile();      //added on 8-12-17 by shri
                    InsertTokensToData();   //added on 09-01-18 by shri
                    //GetEodData();
                    ReadConsolidate();
                    ReplaceClosing();
                    ClearTrades();  //added on 9-11-17 by shri
                }
                
            }
            catch(Exception doeodEx)
            {
                InsertError(doeodEx.Message+":"+doeodEx.StackTrace.ToString().Substring(doeodEx.StackTrace.ToString().Length - 10));
            }
            

        }
        void InsertTokensToData()
        {
            try
            {
                for (int n = 0; n < dayOne.Rows.Count; n++)
                {
                    int token;
                    if (EngineProcess.dict_OScrip_Token.TryGetValue(dayOne.Rows[n]["ScripName"].ToString(), out token))
                    {
                        dayOne.Rows[n]["ScripToken"] = token;
                    }
                    else
                    {
                        dayOne.Rows[n]["ScripToken"] = "-999999";
                    }

                }
            }
            catch (Exception insertEx)
            {
                 InsertError(insertEx.Message + ":" + insertEx.StackTrace.ToString().Substring(insertEx.StackTrace.ToString().Length - 10));
                
            }
        }

        public void ClearTrades()
        {
            try
            {
                if(arrcsDB.State!=ConnectionState.Open)
                {
                    arrcsDB.Open();
                }
                MySqlCommand cmd = new MySqlCommand("truncate trades",arrcsDB);
                cmd.ExecuteNonQuery();

            }
            catch(Exception clrEx)
            {
                InsertError(clrEx.Message+":"+clrEx.StackTrace.ToString().Substring(clrEx.StackTrace.ToString().Length - 10));
            }
            

        }
        #region ReadClosing
        public void ReadClosing()
        {
            Closing=EOD.Tables.Add("Closing");
            Closing.Columns.Add("Scrip");
            Closing.Columns.Add("Closing");
            Closing.Columns.Add("InstrumentName");
            Closing.Columns.Add("StrikePrice");//added by navin on 11-02-2019 for all scrips irrespective of weekly or monthly expiry
            Closing.Columns.Add("Expiry");     //added by navin on 11-02-2019 for all scrips irrespective of weekly or monthly expiry
            Closing.Columns.Add("OptionType"); //added by navin on 11-02-2019 for all scrips irrespective of weekly or monthly expiry
            Closing.Columns.Add("Underlying"); //added by navin on 11-02-2019 for all scrips irrespective of weekly or monthly expiry
            DataRow rwClosing;

            #region FO
            try
            {
                string[] foPath = Directory.GetFiles("C:\\Prime", "fo*"+".csv");
                
                if(foPath.Length==1)
                {
                    using (FileStream stream = File.Open(foPath[0], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string line;
                            while((line=reader.ReadLine())!=null)
                            {
                                //string scripName = "";
                                string[] fields = line.Split(',');
                                if(!fields[0].Contains("INSTRUMENT"))
                                {
                                    //lastThursday(Convert.ToInt32(Exdate.Substring(5, 4)), Exdate.Substring(2, 3));
                                    var date = DateTime.Parse(fields[2],CultureInfo.InvariantCulture).ToString("dd",CultureInfo.InvariantCulture);
                                    var month= DateTime.Parse(fields[2], CultureInfo.InvariantCulture).ToString("MMM", CultureInfo.InvariantCulture);
                                    var year = DateTime.Parse(fields[2], CultureInfo.InvariantCulture).ToString("yyyy", CultureInfo.InvariantCulture);
                                    Exdate = year.Substring(2) + month.ToUpper();
                                    scrip = fields[1];
                                    strike = fields[3];
                                    rwClosing = Closing.NewRow();
                                    /* commented by navin on 11-02-2019
                                    string inst = "";
                                    if(fields[4]=="XX")
                                    {
                                        inst = "FUT";
                                    }
                                    else
                                    {
                                        inst = fields[4];
                                    }
                                    if (fields[1] == "BANKNIFTY")
                                    {
                                        if((inst=="FUT")&&(strike == "0"))
                                        {
                                            rwClosing["Scrip"] = scrip + Exdate + inst;
                                        }
                                        else if(inst!="FUT")
                                        {
                                            rwClosing["Scrip"] = scrip + date+month.ToUpper()+year.Substring(2) + strike + inst;
                                        }
                                    }
                                    else
                                    {
                                        if(inst=="FUT")
                                        {
                                            rwClosing["Scrip"] = scrip + Exdate + inst;
                                        }
                                        else
                                        {
                                            if (strike != "0")
                                            {
                                                rwClosing["Scrip"] = scrip + Exdate + strike + inst;
                                            }
                                            else
                                            {
                                                rwClosing["Scrip"] = scrip + Exdate + inst;
                                            }
                                        }
                                    }
                                     */
                                    
                                    #region added by navin on 11-02-2019 to remove weekly expiry issues
                                    DateTime dte = DateTime.ParseExact(fields[2].Trim().ToString().Replace("-", ""), "ddMMMyyyy", CultureInfo.InvariantCulture);
                                    if (fields[4] == "XX")
                                        rwClosing["Scrip"] = scrip + Exdate+"FUT";
                                    rwClosing["Underlying"] = fields[1];
                                    rwClosing["OptionType"] = fields[4];
                                   // fields[3]= String.Format("{0:0.00}",Convert.ToDouble(fields[3]));
                                    rwClosing["StrikePrice"] =Convert.ToDouble(fields[3]);
                                    
                                    rwClosing["Expiry"] = ConvertToUnixTimestamp(dte);
                                    #endregion
                                    

                                    if (fields[8]=="")
                                    {
                                        rwClosing["Closing"] = fields[9];
                                    }
                                    else
                                    {
                                        rwClosing["Closing"] = fields[8];
                                    }
                                    rwClosing["InstrumentName"] = fields[0];       //26-10-17
                                    Closing.Rows.Add(rwClosing);
                                }
                            }
                        }
                    }
                    closingDne = true;      //added on 8-12-17 by shri
                }
                else if (foPath.Length == 0)    //added on 8-12-17 by shri
                {
                    XtraMessageBox.Show("Please download the Latest Bhavcopy file.");
                    closingDne = false; //added on 8-12-17 by shri
                }

            }
            catch(Exception foEx)
            {
                InsertError(foEx.Message);
            }
            #endregion

            #region CM

            try
            {
                string[] cmPath = Directory.GetFiles("C:\\Prime", "cm*" + ".csv");
                if (cmPath.Length == 1)
                {
                    using (FileStream stream = File.Open(cmPath[0], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                string[] fields = line.Split(',');
                                if (!fields[0].Contains("SYMBOL"))
                                {
                                    scripCM = fields[0];
                                    rwClosing = Closing.NewRow();
                                    rwClosing["Scrip"] = scripCM;
                                    if (fields[5] == "")
                                    {
                                        rwClosing["Closing"] = fields[6];
                                    }
                                    else
                                    {
                                        rwClosing["Closing"] = fields[5];
                                    }
                                    
                                    Closing.Rows.Add(rwClosing);
                                }
                            }
                        }
                    }
                    closingDne = true;  //added on 8-12-17 by shri
                }
                else if(cmPath.Length==0)   //8-12-17
                {
                    XtraMessageBox.Show("Please download the Latest Bhavcopy file.");
                    closingDne = false; //added on 8 - 12 - 17 by shri
                }

            }
            catch (Exception foEx)
            {
                InsertError(foEx.Message);
            }

            #endregion

        }
        #endregion

        #region getLastThursdayOfMonth
        public void lastThursday(int year, string month)
        {
            int mon = DateTime.ParseExact(month, "MMM", CultureInfo.InvariantCulture).Month;
            var daysInMonth = DateTime.DaysInMonth(year, mon);
            for (int day = daysInMonth; day > 0; day--)
            {

                DateTime currentDateTime = new DateTime(year, mon, day);
                if (currentDateTime.DayOfWeek == DayOfWeek.Thursday)
                {
                    Thursday = currentDateTime.Day;
                    break;
                }
                
            }
        }
        #endregion

        #region Select Client
        public void GetClient()
        {
            string clientlist = "";
            clientdetail.Clear();
            try
            {
                string query = "select ClientID as DealerID from clientdetail";
                MySqlDataAdapter da = new MySqlDataAdapter(query, arrcsDB);
                da.Fill(clientdetail);
                DecryptDatatable(clientdetail);
            }
            catch (Exception dltEr)
            {
                InsertError(dltEr.Message+":"+dltEr.StackTrace.ToString().Substring(dltEr.StackTrace.ToString().Length - 10));
            }
            try
            {
                if (clientdetail.Rows.Count > 0)
                {
                    for (int clrows = 0; clrows < clientdetail.Rows.Count; clrows++)
                    {
                        if (clientlist == "")
                        {
                            clientlist = "'" + clientdetail.Rows[clrows][0].ToString() + "'";
                        }
                        else
                        {
                            clientlist = clientlist + ",'" + clientdetail.Rows[clrows][0].ToString() + "'";
                        }
                    }
                }
                else
                {
                    clientlist = "''";//added on 09_01_2017
                }
            }
            catch (Exception clex)
            {
                InsertError(clex.Message+":"+clex.StackTrace.ToString().Substring(clex.StackTrace.ToString().Length - 10));
            }
        }
        #endregion

        #region Fetch Data From Day1
        #region OLD Previous day method removed by shri on 8-12-17
        //public void GetDay1Data()
        //{
        //    DataRow dr;
        //    try
        //    {
        //        using (FileStream stream2 = File.Open("C:/Prime/Day1/Day1FO.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //        {

        //            using (StreamReader sr = new StreamReader(stream2))
        //            {
        //                string line1;

        //                while ((line1 = sr.ReadLine()) != null)
        //                {
        //                    string[] fields = line1.Split(',');

        //                    if (fields[0].Trim() != "")
        //                    {
        //                        dr = dayOne.NewRow();
        //                        dr["InstrumentName"] = fields[5].Trim().ToString();
        //                        dr["ScripName"] = fields[2].Trim().ToString();
        //                        DateTime dte = DateTime.ParseExact(fields[8].Trim().ToString(), "ddMMMyyyy", CultureInfo.InvariantCulture);
        //                        dr["ScripExpiry"] = ConvertToUnixTimestamp(dte);
        //                        dr["NetPosition"] = Convert.ToInt64(fields[10].Trim().ToString());
        //                        dr["BEP"] = Convert.ToDecimal(fields[9].Trim().ToString());
        //                        dr["Underlying"] = fields[6].Trim().ToString();
        //                        if (fields[11].ToString().Trim() == "2")
        //                        {
        //                            dr["NetPosition"] = (Convert.ToDouble(fields[10].Trim()) * (-1));    //30-11-17
        //                        }
        //                        dr["Client"] = fields[0].Trim().ToString();
        //                        dr["OptionType"] = fields[4].Trim().ToString();
        //                        dr["StrikePrice"] = fields[7] == "" ? -1 : Convert.ToDecimal(fields[5].Trim());
        //                        dr["ScripToken"] = fields[1] == "" ? 1 : Convert.ToInt32(fields[1].Trim());
        //                        dr["UnderlyingFuture"] = fields[3].Trim().ToString();

        //                        dayOne.Rows.Add(dr);
        //                    }
        //                }
        //            }

        //        }
        //        using (FileStream stream3 = File.Open(@"C:/Prime/Day1/Day1CD.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //        {
        //            using (StreamReader sr = new StreamReader(stream3))
        //            {
        //                string line1;
        //                while ((line1 = sr.ReadLine()) != null)
        //                {
        //                    string[] fields = line1.Split(',');
        //                    if (fields[0].Trim() != "")
        //                    {
        //                        dr = dayOne.NewRow();
        //                        dr["InstrumentName"] = fields[5].Trim().ToString();
        //                        dr["ScripName"] = fields[2].Trim().ToString();
        //                        DateTime dte = DateTime.ParseExact(fields[8].Trim().ToString(), "ddMMMyyyy", CultureInfo.InvariantCulture);
        //                        dr["ScripExpiry"] = ConvertToUnixTimestamp(dte);
        //                        dr["BEP"] = Convert.ToDecimal(fields[9].Trim().ToString());
        //                        dr["Underlying"] = fields[6].Trim().ToString();
        //                        if (fields[11].ToString().Trim() == "2")
        //                        {
        //                            dr["NetPosition"] = (Convert.ToDouble(fields[10].Trim()) * (-1000));
        //                        }
        //                        else
        //                        {
        //                            dr["NetPosition"] = (Convert.ToDouble(fields[10].Trim()) * (1000));
        //                        }
        //                        dr["Client"] = fields[0].Trim().ToString();
        //                        dr["OptionType"] = fields[4].Trim().ToString();
        //                        dr["StrikePrice"] = fields[7].Trim() == "" ? -1 : Convert.ToDecimal(fields[5].Trim());
        //                        dr["ScripToken"] = fields[1].Trim() == "" ? 1 : Convert.ToInt32(fields[1].Trim());
        //                        dr["UnderlyingFuture"] = fields[3].Trim().ToString();
        //                        dayOne.Rows.Add(dr);
        //                    }
        //                }
        //            }
        //        }
        //        using (FileStream stream4 = File.Open("C:/Prime/Day1/Day1CM.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //        {
        //            using (StreamReader sr = new StreamReader(stream4))
        //            {
        //                string line1;

        //                while ((line1 = sr.ReadLine()) != null)
        //                {
        //                    string[] fields = line1.Split(',');
        //                    if (fields[0].Trim() != "")
        //                    {
        //                        dr = dayOne.NewRow();
        //                        dr["InstrumentName"] = fields[5].Trim().ToString();
        //                        dr["ScripName"] = fields[2].Trim().ToString();
        //                        dr["ScripExpiry"] = "01JAN1900";
        //                        dr["NetPosition"] = Convert.ToDouble(fields[10].Trim().ToString());
        //                        dr["BEP"] = Convert.ToDecimal(fields[9].Trim().ToString());
        //                        dr["Underlying"] = fields[6].Trim().ToString();
        //                        if (fields[11].ToString().Trim() == "2")
        //                        {
        //                            dr["NetPosition"] = (Convert.ToDouble(fields[10].Trim()) * (-1));
        //                        }
        //                        dr["Client"] = fields[0].Trim().ToString();
        //                        dr["OptionType"] = fields[4].Trim().ToString();
        //                        dr["StrikePrice"] = "-1";
        //                        dr["ScripToken"] = fields[1].Trim() == "" ? 1 : Convert.ToInt32(fields[1].Trim().ToString());
        //                        dr["UnderlyingFuture"] = fields[3].Trim().ToString();
        //                        dayOne.Rows.Add(dr);
        //                    }
        //                }
        //            }
        //        }
        //        //int tradeCount = dayOne.Rows.Count;
        //    }
        //    catch (Exception ex)
        //    {
        //        InsertError(ex.StackTrace.ToString().Substring(ex.StackTrace.ToString().Length - 10));
        //    }
        //}
        #endregion
        public void ReadPreviousDayFile()
        {
            try
            {
                DataRow dr;
                string EQScripName;
                DateTime DofExpiry;
                double ExpiryInTicks;
                try
                {
                    using (FileStream stream2 = File.Open("C:/Prime/Day1/Day1FO.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {

                        using (StreamReader sr = new StreamReader(stream2))
                        {
                            string line1;

                            while ((line1 = sr.ReadLine()) != null)
                            {
                                string[] fields = line1.Split(',');

                                if (fields[0].Trim() != "")
                                {
                                    dr = dayOne.NewRow();
                                    dr["InstrumentName"] = fields[5].Trim().ToString();
                                    dr["ScripName"] = fields[2].Trim().ToString();
                                    DateTime dte = DateTime.ParseExact(fields[8].Trim().ToString(), "ddMMMyyyy", CultureInfo.InvariantCulture);
                                    dr["ScripExpiry"] = ConvertToUnixTimestamp(dte);
                                    dr["NetPosition"] = Convert.ToInt64(fields[10].Trim().ToString());
                                    dr["BEP"] = Convert.ToDouble(fields[9].Trim().ToString());
                                    dr["Underlying"] = fields[6].Trim().ToString();
                                    if (fields[11].ToString().Trim() == "2")
                                    {

                                        dr["NetPosition"] = (Convert.ToDouble(fields[10].Trim()) * (-1));
                                    }
                                    dr["Client"] = fields[0].Trim().ToString();
                                    dr["OptionType"] = fields[4].Trim().ToString();
                                    dr["StrikePrice"] = fields[7] == "" ? -1 : Convert.ToDecimal(fields[7].Trim());
                                    dr["ScripToken"] = fields[1] == "" ? 1 : Convert.ToInt32(fields[1].Trim());
                                    dr["UnderlyingFuture"] = fields[3].Trim().ToString();
                                    //dr["dCounterTraderOrderNumber"] = previousDay.Rows.Count + 1;
                                    dayOne.Rows.Add(dr);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    InsertError(ex.ToString());
                }
                try
                {
                    using (FileStream stream2 = File.Open("C:/Prime/Day1/Day1CD.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader sr = new StreamReader(stream2))
                        {
                            string line1;
                            while ((line1 = sr.ReadLine()) != null)
                            {
                                string[] fields = line1.Split(',');
                                if (fields[0].Trim() != "")
                                {
                                    dr = dayOne.NewRow();
                                    dr["InstrumentName"] = fields[5].Trim().ToString();
                                    dr["ScripName"] = fields[2].Trim().ToString();
                                    DateTime dte = DateTime.ParseExact(fields[8].Trim().ToString(), "ddMMMyyyy", CultureInfo.InvariantCulture); //shri on 30-11-17
                                    dr["ScripExpiry"] = ConvertToUnixTimestamp(dte);
                                    dr["BEP"] = Convert.ToDouble(fields[9].Trim().ToString());
                                    dr["Underlying"] = fields[6].Trim().ToString();
                                    if (fields[11].ToString().Trim() == "2")
                                    {
                                        dr["NetPosition"] = ((Convert.ToDouble(fields[10].Trim())) * (-1000));
                                    }
                                    else
                                    {
                                        dr["NetPosition"] = ((Convert.ToDouble(fields[10].Trim())) * (1000));
                                    }
                                    dr["Client"] = fields[0].Trim().ToString();
                                    dr["OptionType"] = fields[4].Trim().ToString();
                                    dr["StrikePrice"] = fields[7].Trim() == "" ? -1 : Convert.ToDecimal(fields[7].Trim());
                                    dr["ScripToken"] = fields[1].Trim() == "" ? 1 : Convert.ToInt32(fields[1].Trim());
                                    dr["UnderlyingFuture"] = fields[3].Trim().ToString();
                                    dayOne.Rows.Add(dr);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    InsertError(ex.ToString());
                }
                try
                {
                    using (FileStream stream2 = File.Open("C:/Prime/Day1/Day1CM.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader sr = new StreamReader(stream2))
                        {
                            string line1;

                            while ((line1 = sr.ReadLine()) != null)
                            {
                                string[] fields = line1.Split(',');

                                DofExpiry = Convert.ToDateTime("01JAN1980");
                                ExpiryInTicks = ConvertToUnixTimestamp(DofExpiry);

                                EQScripName = fields[2].Trim().ToString() + "_|" + ExpiryInTicks + "_|" + fields[3].Trim().ToString() + "_|" + 0;

                                if (fields[0].Trim() != "")
                                {
                                    dr = dayOne.NewRow();
                                    dr["InstrumentName"] = fields[5].Trim().ToString();
                                    dr["ScripName"] = fields[2].Trim().ToString();
                                    dr["ScripExpiry"] = "01JAN1900";
                                    dr["Netposition"] = Convert.ToInt32(fields[10].Trim().ToString());
                                    dr["BEP"] = Convert.ToDouble(fields[9].Trim().ToString());
                                    dr["Underlying"] = fields[6].Trim().ToString();
                                    if (fields[11].ToString().Trim() == "2")
                                    {
                                        dr["Netposition"] = ((Convert.ToDouble(fields[10].Trim())) * (-1));
                                    }
                                    dr["Client"] = fields[0].Trim().ToString();
                                    dr["OptionType"] = fields[4].Trim().ToString();
                                    dr["StrikePrice"] = "-1";
                                    dr["ScripToken"] = fields[1].Trim() == "" ? 1 : Convert.ToInt32(fields[1].Trim().ToString());
                                    dr["UnderlyingFuture"] = fields[3].Trim().ToString();
                                    dayOne.Rows.Add(dr);
                                }
                            }
                        }
                    }
                    int tradeCount = dayOne.Rows.Count;
                }
                catch (Exception ex)
                {
                    InsertError(ex.ToString());
                }
            }
            catch (Exception ee)
            {
                 InsertError(ee.Message+":"+ee.ToString());
            }
        }
        #endregion


        #region Read ConsolidatedTradeInfo Data
        public void ReadConsolidate()
        {
            try
            {
                if (arrcsDB.State != ConnectionState.Open)
                {
                    arrcsDB.Open();
                }
            }
            catch (Exception conscon)
            {
                InsertError(conscon.Message+":"+conscon.StackTrace.ToString().Substring(conscon.StackTrace.ToString().Length - 10)); 
            }
            try
            {
                string query = "select ClientID as Client,convert(ScripName using utf8) as ScripName,convert(ScripToken using utf8) as ScripToken,convert(ScripLtp using utf8) as ScripLtp,convert(ScripExpiry using utf8) as ScripExpiry,convert(Underlying using utf8) as Underlying,convert(UnderlyingFuture using utf8) as UnderlyingFuture,convert(UnderlyingToken using utf8) as UnderlyingToken,convert(UnderlyingLtp using utf8) as UnderlyingLtp,convert(UnderlyingExpiry using utf8) as UnderlyingExpiry,convert(StrikePrice using utf8) as StrikePrice,convert(OptionType using utf8) as OptionType,convert(InstrumentName using utf8) as InstrumentName,convert(BEP using utf8) as BEP,convert(NetPosition using utf8) as NetPosition,convert(IVLower using utf8) as IVLower,convert(IVMiddle using utf8) as IVMiddle,convert(IVHigher using utf8) as IVHigher from consolidatedtradeinfo";
                MySqlCommand IVcmd = new MySqlCommand(query, arrcsDB);
                MySqlDataAdapter da = new MySqlDataAdapter(IVcmd);
                DataTable dtComsolidatedData = new DataTable();
                da.Fill(dtComsolidatedData);
                DecryptDatatable(dtComsolidatedData);
                endTrades = dtComsolidatedData.Copy();
                //ds_Main.Tables["tradeinfo"].Select().Distinct();
            }
            catch (Exception readConsol)
            {
                InsertError(readConsol.Message+":"+readConsol.StackTrace.ToString().Substring(readConsol.StackTrace.ToString().Length - 10)); 
            }
            arrcsDB.Close();
           
        }
        #endregion

        #region Get EOD Data from DataBase
        public void GetEodData()
        {
            try
            {
                string query = "select convert(DealerID using utf8) as DealerID,convert(ScripName using utf8) as ScripName,convert(TokenNo using utf8) as TokenNo,convert(StrikePrice using utf8) as StrikePrice,convert(ClosingPrice using utf8) as FillPrice,convert(OptionType using utf8) as OptionType,convert(InstrumentName using utf8) as InstrumentName,convert(Expiry using utf8) as Expiry,convert(Underlying using utf8) as Underlying,convert(UnderlyingScripName using utf8) as UnderlyingScripName,convert(FillQuantity using utf8) as FillQuantity FROM EOD";
                MySqlDataAdapter da = new MySqlDataAdapter(query, arrcsDB);
                da.Fill(EOD, "eodB");
                DecryptDatatable(EOD.Tables["eodB"]);   //8-12-17
                //Eodcount = Convert.ToInt32(Tradeinfo.Tables["tradeinfo1"].Rows[Tradeinfo.Tables["tradeinfo1"].Rows.Count - 1]["dCounterTraderOrderNumber"]);
            }
            catch (Exception eodData)
            {
                //eodFlag = -1; record = -1;
                InsertError(eodData.Message+":"+eodData.StackTrace.ToString().Substring(eodData.StackTrace.ToString().Length - 10));
            }
        }
        #endregion

        #region Read From Position File
        
        public void ReadPositionfile()
        {
            DataRow dr;
            string ScripName;
            DateTime DofExpiry;
            double ExpiryInTicks;
            try
            {
                using (StreamReader sr = new StreamReader(@"C:/Prime/VTECH_NSE_FNO_POS_M2M_0K500_AS_ON_22Aug2017.CSV"))
                {
                    string[] csvRows = System.IO.File.ReadAllLines(@"C:/Prime/VTECH_NSE_FNO_POS_M2M_0K500_AS_ON_22Aug2017.CSV");
                    int lengthCSV = csvRows.Length;
                    for (int i = 0; i < lengthCSV; i++)
                    {
                        string test3 = csvRows[i];
                        string[] fields = test3.Split(',');


                        DofExpiry = Convert.ToDateTime(fields[2].Trim());
                        ExpiryInTicks = ConvertToUnixTimestamp(DofExpiry);

                        string Day = DofExpiry.ToString().Substring(0, 2);
                        string monthName = DofExpiry.ToString("MMM", CultureInfo.InvariantCulture).ToUpper();
                        string year = DofExpiry.ToString().Substring(8, 2);

                        ScripName = fields[1].Trim().ToString() + "_|" + ExpiryInTicks + "_|" + (fields[3].Trim() == "FX" ? "XX" : fields[3].Trim()).ToString() + "_|" + (Math.Round(Convert.ToDecimal(fields[4].Trim() == "" ? "0" : fields[4].Trim()))).ToString();

                        dr = endTrades.NewRow();
                        ////dr["cInstrumentName"] = fields[2].Trim().ToString();
                        //dr["ScripName"] = fields[7].Trim().ToString(); // commented on 15-08-2017
                        dr["ScripName"] = ScripName;// added on 15-08-2017
                        //dr["Expiry"] = fields[4].Trim().ToString(); // commented on 15-08-2017
                        dr["Expiry"] = ExpiryInTicks; // added on 15-08-2017

                        dr["FillQuantity"] = fields[5].Trim().ToString();

                        dr["FillPrice"] = fields[6].Trim().ToString();

                        dr["Underlying"] = fields[1].Trim().ToString();
                        //if (fields[13].ToString().Trim() == "2")
                        //{
                        //    dr["FillQuantity"] = (long.Parse(fields[14].Trim()) * (-1));
                        //}
                        dr["DealerID"] = fields[0].Trim().ToString();

                        dr["UnderlyingScripName"] = (fields[3].Trim().ToString() == "EQ" ? fields[1].Trim().ToString() + "0_|EQ_|0" : fields[1].Trim().ToString() + "_|" + ExpiryInTicks + "_|FUT_|0");
                        dr["OptionType"] = (fields[3].Trim() == "FX" ? "XX" : fields[3].Trim()).ToString();
                        dr["StrikePrice"] = Convert.ToDecimal(fields[4].Trim() == "" ? "0" : fields[4].Trim());

                        if (fields[3].Trim() == "FX")
                        {
                            dr["FlatScripName"] = fields[1].Trim().ToString() + year + monthName + "FUT";
                        }
                        else if (fields[3].Trim() == "EQ")
                        {
                            dr["FlatScripName"] = fields[1].Trim().ToString();
                        }
                        else
                        {
                            if (fields[1].Trim() == "BANKNIFTY")
                            {
                                dr["FlatScripName"] = fields[1].Trim().ToString() + Day + monthName + year + fields[4] + fields[3];
                            }
                            else
                            {
                                dr["FlatScripName"] = fields[1].Trim().ToString() + year + monthName + fields[4] + fields[3];
                            }

                        }

                       endTrades.Rows.Add(dr);

                    }

                }

                foreach (DataRow Rw in endTrades.Rows)
                {
                    DataRow[] dr1 = Closing.Select("Scrip='" + Rw["FlatScripName"].ToString() + "'");

                    if (dr1.Any())
                    {
                        Rw["InstrumentName"] = dr1[0]["Instrumentname"];
                    }


                }

                for (int i = endTrades.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr1 = endTrades.Rows[i];
                    if (dr1["FillPrice"].ToString() == "0" && dr1["FillQuantity"].ToString() == "0")
                        dr1.Delete();
                }

                CombineToken();

                //DateTime asdfg = ConvertFromUnixTimestamp(1188604800);
            }
            catch (Exception ReadPositionError)
            {
                InsertError(ReadPositionError.StackTrace.ToString().Substring(ReadPositionError.StackTrace.ToString().Length - 10));
            }
        }
        #endregion

        #region To ADD Token From EOD.Tables["token"] to endTrades
        public void CombineToken()
        {
            try
            {
                // query to get Token From tokenmaster table
                string eodQuery = "select TokenNo,Expiry,OExpiry,OScripName,ScripName from rd_tokenmaster";//08-08-2017   changes the datetime
                MySqlDataAdapter da = new MySqlDataAdapter(eodQuery, arrcsDB);
                da.Fill(EOD, "token");
                // to add token into eodDay1.Tables["tradeinfo1"] from  ds.Tables["Token"]
                foreach (DataRow Rw in endTrades.Rows)
                {
                    DataRow[] dr = EOD.Tables["token"].Select("ScripName='" + Rw["ScripName"].ToString() + "'");

                    if (dr.Any())
                    {
                        Rw["TokenNo"] = dr[0]["TokenNo"];
                    }

                }

                for (int i = endTrades.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr1 = endTrades.Rows[i];
                    if (dr1["TokenNo"].ToString() == "")
                        dr1.Delete();
                }

            }
            catch (Exception AddTokenErr)
            {
                InsertError(AddTokenErr.StackTrace.ToString().Substring(AddTokenErr.StackTrace.ToString().Length - 10));
            } 
        }

        #endregion

        #region ReplaceClosing
        public void ReplaceClosing()
        {
            try
            {
                if (arrcsDB.State != ConnectionState.Open)
                {
                    arrcsDB.Open();
                }
                endTrades.Merge(dayOne);        //added on 19-11-17
                var result = (endTrades.AsEnumerable()
                                     .Select(
                                            x => new
                                            {
                                                TokenNumber = x["ScripToken"],
                                                Client = x["Client"],
                                                FillPrice = x["BEP"],
                                                ScripName = x["ScripName"],
                                                Underlying = x["Underlying"],
                                                OptionType = x["OptionType"],
                                                StrikePrice = x["StrikePrice"],
                                                InstrumentName = x["InstrumentName"],
                                                FillQuantity = x["NetPosition"],
                                                expiry = x["ScripExpiry"],
                                                UnderlyingScripName = x["UnderlyingFuture"]
                                            }
                                            )
                                            .GroupBy(s => new { s.TokenNumber, s.Client })
                                            .Select(
                                                    g => new
                                                    {
                                                        Underlying = g.Select(x => x.Underlying).First().ToString(),
                                                        BEP = Math.Round(Convert.ToDecimal(g.Sum(x => Convert.ToDecimal(x.FillQuantity) * Convert.ToDecimal(x.FillPrice)) / ((g.Sum(x => Convert.ToDecimal(x.FillQuantity))) == 0 ? -1 : (g.Sum(x => Convert.ToInt64(x.FillQuantity))))), 2),//added on 25_08_16
                                                        NetPos = g.Sum(x => Convert.ToDecimal(x.FillQuantity)),
                                                        ScripName = g.Select(x => x.ScripName).First().ToString(),
                                                        OptionType = g.Select(x => x.OptionType).First().ToString(),
                                                        InstrumentName = g.Select(x => x.InstrumentName).First().ToString(),
                                                        Expiry = g.Select(x => x.expiry).First().ToString(),
                                                        StrikePrice = g.Select(x => x.StrikePrice).First().ToString(),
                                                        Client = g.Select(x => x.Client).First().ToString(),
                                                        UnderlyingFuture = g.Select(x => x.UnderlyingScripName).First().ToString(),
                                                        ScripToken = g.Select(x => x.TokenNumber).First().ToString(),
                                                    }
                                                )
                                            .OrderByDescending(x => x.Client))
                                            ;
                int tr = 0;
                foreach (var item in result)
                {
                    tradeInfoEOD[tr, 0] = item.Client;
                    tradeInfoEOD[tr, 1] = item.ScripToken;
                    tradeInfoEOD[tr, 2] = item.ScripName;
                    tradeInfoEOD[tr, 3] = item.NetPos.ToString();
                    tradeInfoEOD[tr, 4] = item.Underlying;
                    tradeInfoEOD[tr, 5] = item.UnderlyingFuture;
                    tradeInfoEOD[tr, 6] = item.InstrumentName;
                    tradeInfoEOD[tr, 7] = item.Expiry == "" ? "1980-01-01 " : item.Expiry.ToString();
                    tradeInfoEOD[tr, 8] = item.StrikePrice;
                    tradeInfoEOD[tr, 9] = item.BEP.ToString();
                    tradeInfoEOD[tr, 10] = item.OptionType;
                    tr++;
                }
                int val = result.Count();
                //string query="",query1 = "";
                MySqlCommand cmd = new MySqlCommand();
                StringBuilder insertCmd = new StringBuilder("insert into eod (DealerID,ScripName,TokenNo,StrikePrice,FillPrice,OptionType,InstrumentName,Expiry,Underlying,UnderlyingScripName,FillQuantity,ClosingPrice) values");
                List<string> toInsert = new List<string>();
                cmd = new MySqlCommand("truncate eod", arrcsDB);
                cmd.ExecuteNonQuery();
                long date_Tick =Convert.ToInt64(ConvertToUnixTimestamp(DateTime.Now.Date).ToString());
                for (int a = 0; a < tr; a++)
                {
                    try
                    {
                        #region Marking only future scrips to closing logic changed by navin on 15-02-2019
                        if (tradeInfoEOD[a, 10].ToString().Trim() == "XX")
                        {
                            var closingPrice = Closing.AsEnumerable().Where(r => r.Field<string>("Underlying") == tradeInfoEOD[a, 4]).Select(r => r.Field<string>("Closing"));
                            if (closingPrice.Count() > 0)
                            {
                                tradeInfoEOD[a, 11] = closingPrice.First().ToString();  //9-11-17
                            }
                        }
                        else
                        {
                            tradeInfoEOD[a, 11] = tradeInfoEOD[a, 9];
                        }
                        #endregion
                        #region added by navin on 11-02-2019 to remove weekly expiry issues
                        //if (tradeInfoEOD[a, 10].ToString().Trim() == "EQ" || tradeInfoEOD[a, 10].ToString().Trim() == "XX")
                        //{
                        //    var closingPrice = Closing.AsEnumerable().Where(r => r.Field<string>("Underlying") == tradeInfoEOD[a, 4]).Select(r => r.Field<string>("Closing"));
                        //    if (closingPrice.Count() > 0)
                        //    {
                        //        tradeInfoEOD[a, 11] = closingPrice.First().ToString();  //9-11-17
                        //    }
                        //}
                        //else
                        //{
                        //    DataRow[] closingPrice = Closing.Select("Underlying='" + tradeInfoEOD[a, 4] + "' and StrikePrice='" + tradeInfoEOD[a, 8] + "' and OptionType='" + tradeInfoEOD[a, 10] + "' and Expiry='" + tradeInfoEOD[a, 7] + "'");
                        //    if (closingPrice.Any())
                        //    {
                        //        tradeInfoEOD[a, 11] = closingPrice[0]["Closing"].ToString();  //9-11-17
                        //    }
                        //}

                        #endregion

                        //var closingPrice = Closing.AsEnumerable().Where(r => r.Field<string>("Underlying") == tradeInfoEOD[a, 4]).Select(r => r.Field<string>("Closing"));
                        //if (closingPrice.Count() > 0)
                        //{
                        //    tradeInfoEOD[a, 11] = closingPrice.First().ToString();  //9-11-17
                        //}
                        if (Convert.ToInt64(tradeInfoEOD[a,7])>date_Tick)   //11-12-17
                        {
                            toInsert.Add("('" +tradeInfoEOD[a, 0].ToString() + "','" +tradeInfoEOD[a, 2].ToString() + "','" +tradeInfoEOD[a, 1].ToString() + "','" +tradeInfoEOD[a, 8].ToString() + "','" +tradeInfoEOD[a, 9].ToString() + "','" +tradeInfoEOD[a, 10].ToString() + "','" +tradeInfoEOD[a, 6].ToString() + "','" +tradeInfoEOD[a, 7].ToString() + "','" +tradeInfoEOD[a, 4].ToString() + "','" +tradeInfoEOD[a, 5].ToString() + "','" +tradeInfoEOD[a, 3].ToString() + "','" +(tradeInfoEOD[a, 11] == null ? "0" : tradeInfoEOD[a, 11]) + "')");   //9-11-17//(tradeInfoEOD[a, 11]==null?"0": tradeInfoEOD[a, 11]) on 09-08-2018
                        }
                        else if (tradeInfoEOD[a, 10].ToString().Trim() == "EQ") //added on 30-04-18 by shri
                        {
                            toInsert.Add("('" +tradeInfoEOD[a, 0].ToString() + "','" +tradeInfoEOD[a, 2].ToString() + "','" +tradeInfoEOD[a, 1].ToString() + "','" +tradeInfoEOD[a, 8].ToString() + "','" +tradeInfoEOD[a, 9].ToString() + "','" +tradeInfoEOD[a, 10].ToString() + "','" +tradeInfoEOD[a, 6].ToString() + "','" +tradeInfoEOD[a, 7].ToString() + "','" +tradeInfoEOD[a, 4].ToString() + "','" +tradeInfoEOD[a, 5].ToString() + "','" +tradeInfoEOD[a, 3].ToString() + "','" +(tradeInfoEOD[a, 11]==null?"0": tradeInfoEOD[a, 11]) + "')");   //9-11-17//(tradeInfoEOD[a, 11]==null?"0": tradeInfoEOD[a, 11]) on 09-08-2018
                        }
                    }
                    catch (Exception insertEodex)
                    {
                        record = -1;
                        InsertError(insertEodex.Message+":"+insertEodex.StackTrace.ToString().Substring(insertEodex.StackTrace.ToString().Length - 10));
                    }
                }
                if (toInsert.Count > 0)
                {
                    insertCmd.Append(string.Join(",", toInsert));
                    insertCmd.Append(";");
                    cmd = new MySqlCommand(insertCmd.ToString(), arrcsDB);
                    cmd.ExecuteNonQuery();
                }
                #region
                //try
                //{
                //string q = "select convert(DealerID using utf8) as DealerID,convert(ScripName using utf8) as ScripName,convert(TokenNo using utf8) as TokenNo,convert(StrikePrice using utf8) as StrikePrice,convert(ClosingPrice using utf8) as FillPrice,convert(OptionType using utf8) as OptionType,convert(InstrumentName using utf8) as InstrumentName,convert(Expiry using utf8) as Expiry,convert(Underlying using utf8) as Underlying,convert(UnderlyingScripName using utf8) as UnderlyingScripName,convert(FillQuantity using utf8) as FillQuantity FROM EOD";
                //MySqlDataAdapter e = new MySqlDataAdapter(q, arrcsDB);
                //DataTable eod1 = new DataTable();
                //e.Fill(eod1);
                //test(eod1);
                //   string you = DecryptString("¶¶¾ºº¸½¿··";
                //    //DateTime gg = ConvertFromUnixTimestamp(1198886400);

                //    //string fdf = ConvertToUnixTimestamp(DateTime.Now).ToString();

                //    //string now = EncryptString(ConvertToUnixTimestamp(DateTime.Now.Date).ToString();
                //    //query1 = "delete from eod where Expiry < '" +ConvertToUnixTimestamp(DateTime.Now.Date).ToString(),134) + "' and OptionType<>'"+EncryptString("EQ",134)+"'";   //changed EOD query
                //    //cmd = new MySqlCommand(query1, arrcsDB);    //30-11-17
                //    //cmd.ExecuteNonQuery();


                //}
                //catch (Exception eodex)
                //{
                //    InsertError(eodex.StackTrace.ToString().Substring(eodex.StackTrace.ToString().Length - 10));
                //    record = -1;
                //}
                #endregion
                if (record >= 0)
                {
                    //MessageBox.Show("Eod Process completed successfully");
                    //lblmessage.Visible = true;
                    ClearRecord();
                    eodStatus = true;

                }
            }
            catch (Exception positionconsolex)
            {
                record = -1;
                InsertError(positionconsolex.Message+":"+positionconsolex.StackTrace.ToString().Substring(positionconsolex.StackTrace.ToString().Length - 10));
            }

        }
        #endregion

        #region encrypt_sam
        public string EncryptString(string message, int key)
        {
            try
            {
                char[] a = message.ToCharArray();
                b = new char[a.Length];
                int j = 0;
                for (int i = a.Length - 1; i >= 0; i--, j++)
                {
                    b[j] = Convert.ToChar(a[i] + key);
                }
                string d = new string(b);

                return d;
            }
            catch (Exception ee)
            {
                InsertError(ee.ToString());
                return "";
            }
        }
        #endregion

        #region decrypt_sam
        public string DecryptString(object message, int key)
        {
            try
            {
                char[] a = message.ToString().ToCharArray();
                b = new char[a.Length];
                int j = 0;
                for (int i = a.Length - 1; i >= 0; i--, j++)
                {
                    b[j] = Convert.ToChar(a[i] - key);
                }
                string d = new string(b);
                return d;

            }
            catch (Exception ee)
            {
                InsertError(ee.ToString());
                return "";
            }
        }
        #endregion

        DataTable DecryptDatatable(DataTable dt)
        {
            try
            {
                //DataTable dt = new DataTable();
                //MySqlCommand cmd = new MySqlCommand("Select convert(`Client` using utf8),convert(ScripName using utf8),convert(ScripToken using utf8),convert(ScripLtp using utf8),convert(ScripExpiry using utf8),convert(Underlying using utf8),convert(UnderlyingFuture using utf8),convert(UnderlyingToken using utf8),convert(UnderlyingLtp using utf8),convert(UnderlyingExpiry using utf8),convert(StrikePrice using utf8),convert(OptionType using utf8),convert(InstrumentName using utf8),convert(BEP using utf8),convert(NetPosition using utf8),convert(IVLower using utf8),convert(IVMiddle using utf8),convert(IVHigher using utf8) from consolidatedtradeinfo", mySqlArrcsDBConn);
                //MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                //da.Fill(dt);
                int a = 0;
                foreach (DataRow dr in dt.Rows)
                {

                    for (int i = a; i < dr.ItemArray.Count(); i++)
                    {

                        dr[i] =objWriteLog.DecryptString(dr[i].ToString(), "Nerve123");
                    }
                }
                return dt;
            }
            catch (Exception ee)
            {
                InsertError(ee.ToString());
                return dt;
                
            }
        }

        #region ClearFile
        public void ClearRecord()
        {
            try
            {
                //StreamWriter wtr = new StreamWriter("C:/Prime/Day1FO.txt");
                //wtr.Write("");
                //wtr.Close();
                //wtr.Dispose();

                //wtr = new StreamWriter("C:/Prime/Day1CD.txt");
                //wtr.Write("");
                //wtr.Close();
                //wtr.Dispose();

                //wtr = new StreamWriter("C:/Prime/Day1CM.txt");
                //wtr.Write("");
                //wtr.Close();
                //wtr.Dispose();
                File.WriteAllText(@"C:\Prime\Day1\Day1FO.txt", string.Empty);
                File.WriteAllText(@"C:\Prime\Day1\Day1CD.txt", string.Empty);
                File.WriteAllText(@"C:\Prime\Day1\Day1CM.txt", string.Empty);
            }
            catch (Exception ClrRecord)
            {
                InsertError(ClrRecord.StackTrace.ToString().Substring(ClrRecord.StackTrace.ToString().Length - 10));
                
            }
            
            
        }
        #endregion

        #region methods to convert datetime to tick and tick to datetime
        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }
        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - origin;
            return diff.TotalSeconds;
        }
        #endregion

        #region Insert Error log For EOD Process
        public void InsertError(string error)
        {
            try
            {
                if (arrcsDB.State != ConnectionState.Open)
                {
                    arrcsDB.Open();
                }
                objWriteLog.WriteLog("Engine _ EOP _ " + error + " _ " + DateTime.Now.ToString());
                MySqlCommand errCmd = new MySqlCommand("insert into errorlog values('Engine','EOP',Now(),'" + error + "')", arrcsDB);
                errCmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Error Occured in Engine. The Application will now Exit");
                Application.Exit();
                //lb_ErrorLog.Items.Insert(0, "Exception while inserting error log into table : " + errorlogEx.Message);
            }
        }
        #endregion
    }
}
