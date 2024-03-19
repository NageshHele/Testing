using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using System.Threading;
using DevExpress.XtraEditors;

namespace Gateway
{
    static class Program
    {
        private static Mutex mutex = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                mutex = new Mutex(true, "n.Gateway", out bool isFreshInstance);
                if (isFreshInstance)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    BonusSkins.Register();
                    SkinManager.EnableFormSkins();

                    Application.Run(new frm_Main());
                }
                else
                {
                    XtraMessageBox.Show("Gateway already running in background. Please close all running instances.", "Warning");
                    Environment.Exit(0);
                }
            }
            catch (Exception ee) { XtraMessageBox.Show("Error while Initialising n.Gateway. " + ee, "Error"); Environment.Exit(0); }
        }
    }
}
