using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Engine
{
    class clsWriteLog
    {
        static StreamWriter fs;
        static StreamWriter sw_Debug;

        public clsWriteLog(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string filename = path+"\\Logfile" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".txt";
                if (!File.Exists(filename))
                {
                    using (StreamWriter w = File.CreateText(filename))
                        w.Close();
                }
                if (fs == null)
                    fs = File.AppendText(filename);

                filename = path + "\\Debugfile" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".txt";
                if (!File.Exists(filename))
                {
                    using (StreamWriter w = File.CreateText(filename))
                        w.Close();
                }
                if (sw_Debug == null)
                    sw_Debug = File.AppendText(filename);
            }
            catch (Exception)
            { }
        }

        public void WriteLog(string message, bool isDebug = false)
        {
            try
            {
                if (isDebug)
                {
                    sw_Debug.WriteLine(DateTime.Now + "," + message);
                    sw_Debug.Flush();
                }
                else
                {
                    fs.WriteAsync(message + "\r\n");
                    fs.Flush();
                }
            }
            catch (Exception)
            { }
        }

        public string EncryptString(string strData, string strKey)
        {
            //string strKey = "Nerve123";
            byte[] key = { }; //Encryption Key   
            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] inputByteArray;

            try
            {
                key = Encoding.UTF8.GetBytes(strKey);
                // DESCryptoServiceProvider is a cryptography class defind in c#.  
                DESCryptoServiceProvider ObjDES = new DESCryptoServiceProvider();
                inputByteArray = Encoding.UTF8.GetBytes(strData);
                MemoryStream Objmst = new MemoryStream();
                CryptoStream Objcs = new CryptoStream(Objmst, ObjDES.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                Objcs.Write(inputByteArray, 0, inputByteArray.Length);
                Objcs.FlushFinalBlock();

                return Convert.ToBase64String(Objmst.ToArray());//encrypted string  
            }
            catch (Exception ee)
            {
                WriteLog(ee.Message.ToString());
                return "";
            }

        }

        public string DecryptString(string strData, string strKey)
        {

            try
            {
                byte[] key = { };// Key   
                byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
                byte[] inputByteArray = new byte[strData.Length];


                key = Encoding.UTF8.GetBytes(strKey);
                DESCryptoServiceProvider ObjDES = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(strData);

                MemoryStream Objmst = new MemoryStream();
                CryptoStream Objcs = new CryptoStream(Objmst, ObjDES.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                Objcs.Write(inputByteArray, 0, inputByteArray.Length);
                Objcs.FlushFinalBlock();

                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(Objmst.ToArray());
            }
            catch (Exception ex)
            {

                WriteLog(ex.Message.ToString());
                return "";
            }
            //return rtMesssage;
        }

        #region ReadClosing
        public static DataTable Closing = new DataTable();
        public void ReadClosing()
        {
            if (Closing.Columns.Count == 0)
            {
                Closing.Columns.Add("Scrip");
                Closing.Columns.Add("Closing");
                Closing.Columns.Add("InstrumentName");
                Closing.Columns.Add("StrikePrice");//added by navin on 11-02-2019 for all scrips irrespective of weekly or monthly expiry
                Closing.Columns.Add("Expiry");     //added by navin on 11-02-2019 for all scrips irrespective of weekly or monthly expiry
                Closing.Columns.Add("OptionType"); //added by navin on 11-02-2019 for all scrips irrespective of weekly or monthly expiry
                Closing.Columns.Add("Underlying"); //added by navin on 11-02-2019 for all scrips irrespective of weekly or monthly expiry
            }
            DataRow rwClosing;
            #region FO
            try
            {
                string[] foPath = Directory.GetFiles("C:\\Prime", "fo*" + ".csv");
                if (foPath.Length == 1)
                {
                    using (FileStream stream = File.Open(foPath[0], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                string[] fields = line.Split(',');
                                if (!fields[0].Contains("INSTRUMENT"))
                                {
                                    var date = DateTime.Parse(fields[2], CultureInfo.InvariantCulture).ToString("dd", CultureInfo.InvariantCulture);
                                    var month = DateTime.Parse(fields[2], CultureInfo.InvariantCulture).ToString("MMM", CultureInfo.InvariantCulture);
                                    var year = DateTime.Parse(fields[2], CultureInfo.InvariantCulture).ToString("yyyy", CultureInfo.InvariantCulture);
                                    string Exdate = year.Substring(2) + month.ToUpper();
                                    string scrip = fields[1];
                                    string strike = fields[3];
                                    rwClosing = Closing.NewRow();

                                    #region added by navin on 11-02-2019 to remove weekly expiry issues
                                    DateTime dte = DateTime.ParseExact(fields[2].Trim().ToString().Replace("-", ""), "ddMMMyyyy", CultureInfo.InvariantCulture);
                                    if (fields[4] == "XX")
                                        rwClosing["Scrip"] = scrip + Exdate + "FUT";
                                    rwClosing["Underlying"] = fields[1];
                                    rwClosing["OptionType"] = fields[4];
                                    // fields[3]= String.Format("{0:0.00}",Convert.ToDouble(fields[3]));
                                    rwClosing["StrikePrice"] = Convert.ToDouble(fields[3]);
                                    rwClosing["Expiry"] = ConvertToUnixTimestamp(dte);
                                    #endregion
                                    if (fields[8] == "")
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
                    // closingDne = true;      //added on 8-12-17 by shri
                }
                //else if (foPath.Length == 0)    //added on 8-12-17 by shri
                //{
                //    XtraMessageBox.Show("Please download the Latest Bhavcopy file.");
                //    closingDne = false; //added on 8-12-17 by shri
                //}
            }
            catch (Exception foEx)
            {
                WriteLog(foEx.Message );
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
                                    string scripCM = fields[0];
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
                    //closingDne = true;  //added on 8-12-17 by shri
                }
                //else if (cmPath.Length == 0)   //8-12-17
                //{
                //    XtraMessageBox.Show("Please download the Latest Bhavcopy file.");
                //    closingDne = false; //added on 8 - 12 - 17 by shri
                //}

            }
            catch (Exception foEx)
            {
                WriteLog(foEx.Message );
            }
            #endregion
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

        public static string DecryptLicense(string cipherText)                                                     //added on 25-2-19 by Amey
        {
            string EncryptionKey = "n!e@r#v$e$s#o@l!";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


        
    }
}
