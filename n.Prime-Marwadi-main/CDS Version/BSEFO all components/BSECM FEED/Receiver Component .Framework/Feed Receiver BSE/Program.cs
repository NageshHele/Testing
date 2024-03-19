using DevExpress.XtraEditors;
using Feed_Receiver_BSE.Helper;
using Feed_Receiver_BSE.UI;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Feed_Receiver_BSE
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
            bool IsFreshStart;
            mutex = new Mutex(true, "FR-BSE", out IsFreshStart);

            if (IsFreshStart)
            {
                CollectionHelper.Initialise();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
            }
            else
            {
                XtraMessageBox.Show("BSE Feed Receiver is already running in background. Please close and try agagin.", "Error");
                Environment.Exit(0);
            }
        }
    }
}
