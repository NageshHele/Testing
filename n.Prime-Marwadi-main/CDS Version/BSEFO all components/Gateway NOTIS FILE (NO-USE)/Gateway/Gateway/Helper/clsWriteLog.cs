using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Gateway
{
    public class clsWriteLog
    {
        static StreamWriter fs;
        static StreamWriter sw_Debug;

        public clsWriteLog(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string filename = path + "\\" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".txt";
                if (!File.Exists(filename))
                {
                    using (StreamWriter w = File.CreateText(filename))
                        w.Close();
                }
                if (fs == null)
                    fs = File.AppendText(filename);

                //added on 18NOV2020 by Amey
                filename = path + "\\Debugfile" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".txt";
                if (!File.Exists(filename))
                {
                    using (StreamWriter w = File.CreateText(filename))
                        w.Close();
                }
                if (sw_Debug is null)
                    sw_Debug = File.AppendText(filename);
            }
            catch (Exception)
            { }

        }
        public void Error(string message, bool isDebug = false)
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
                    fs.WriteLine(DateTime.Now + " : " + message + Environment.NewLine);
                    fs.Flush();
                }
            }
            catch (Exception)
            {
                //fs.Close();
            }
            
        }
    }
}
