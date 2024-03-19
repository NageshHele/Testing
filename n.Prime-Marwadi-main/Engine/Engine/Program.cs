using System;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using System.Threading;
using DevExpress.XtraEditors;

namespace Engine
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
                mutex = new Mutex(true, "n.Engine", out bool isFreshInstance);
                if (isFreshInstance)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    BonusSkins.Register();
                    SkinManager.EnableFormSkins();

                    Application.Run(new EngineProcess());
                }
                else
                {
                    XtraMessageBox.Show("n.Engine already running in background. Please close all running instances.", "Warning");
                    Environment.Exit(0);
                }
            }
            catch (Exception ee) { XtraMessageBox.Show("Error while Initialising n.Engine. " + ee, "Error"); Environment.Exit(0); }
        }   
    }
}
