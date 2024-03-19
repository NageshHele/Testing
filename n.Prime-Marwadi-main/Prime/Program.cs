using System;
using System.Windows.Forms;
using DevExpress.Skins;
using System.Threading;
using System.Reflection;
using Microsoft.Win32;
using System.Collections;
using System.Linq;
using NerveLog;
using System.Configuration;
using DevExpress.XtraEditors;
using Prime.UI;
using Prime.Helper;

namespace Prime
{
    internal static class Program
    {
        private static Mutex mutex = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>  
        [STAThread]
        private static void Main()
        {
            bool IsFreshStart;
            mutex = new Mutex(true, "n.Prime", out IsFreshStart);

            if (IsFreshStart)
            {
                CollectionHelper.Initialise();

                //DevExpress.Data.Helpers.SyncHelper.ZombieContextsDetector.DetectionLevel = 3;

                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
                SystemEvents.DisplaySettingsChanging += SystemEvents_DisplaySettingsChanging;
                SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

                //AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                SkinManager.EnableFormSkins();
                //Application.Run(new Dashboard(_logger, Thread.CurrentThread));
                Application.Run(new form_Main(Thread.CurrentThread));
            }
            else
            {
                XtraMessageBox.Show("n.Prime already running in background. Please close and try agagin.", "Error");
                Environment.Exit(0);
            }
        }

        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            //try
            //{
            //    System.Text.StringBuilder msg = new System.Text.StringBuilder();
            //    msg.AppendLine(e.Exception.GetType().FullName);
            //    msg.AppendLine(e.Exception.Message);
            //    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            //    msg.AppendLine(st.ToString());
            //    msg.AppendLine();

            //    _logger.WriteLog("CurrentDomain_FirstChanceException : " + msg);
            //}
            //catch(Exception ee) { _logger.WriteLog("CurrentDomain_FirstChanceException Error : " + ee); }
        }

        static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            //Do nothing
            CollectionHelper._logger.Error(null, $"SystemEvents_UserPreferenceChanged Sender : {sender}|e : {e.Category}");

            CheckSystemEventsHandlersForFreeze();
        }

        static void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            //Do nothing
            CollectionHelper._logger.Error(null, "SystemEvents_DisplaySettingsChanged");
        }

        static void SystemEvents_DisplaySettingsChanging(object sender, EventArgs e)
        {
            //Do nothing
            CollectionHelper._logger.Error(null, "SystemEvents_DisplaySettingsChanging");
        }

        private static void CheckSystemEventsHandlersForFreeze()
        {
            var handlers = typeof(SystemEvents).GetField("_handlers", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            var handlersValues = handlers.GetType().GetProperty("Values").GetValue(handlers);
            foreach (var invokeInfos in (handlersValues as IEnumerable).OfType<object>().ToArray())
                foreach (var invokeInfo in (invokeInfos as IEnumerable).OfType<object>().ToArray())
                {
                    var syncContext = invokeInfo.GetType().GetField("_syncContext", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(invokeInfo);
                    if (syncContext == null) throw new Exception("syncContext missing");
                    if (!(syncContext is WindowsFormsSynchronizationContext)) continue;
                    var threadRef = (WeakReference)syncContext.GetType().GetField("destinationThreadRef", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(syncContext);
                    if (!threadRef.IsAlive) continue;
                    var thread = (Thread)threadRef.Target;
                    if (thread.ManagedThreadId == 1) continue;  // Change here if you have more valid UI threads to ignore
                    var dlg = (Delegate)invokeInfo.GetType().GetField("_delegate", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(invokeInfo);
                    CollectionHelper._logger.Error(null, $"SystemEvents handler '{dlg.Method.DeclaringType}.{dlg.Method.Name}' could freeze app due to wrong thread: "
                                    + $"ThreadID:{thread.ManagedThreadId},IsThreadPoolThread:{thread.IsThreadPoolThread},IsAlive:{thread.IsAlive},Name:{thread.Name}");
                }
        }
    }

}

