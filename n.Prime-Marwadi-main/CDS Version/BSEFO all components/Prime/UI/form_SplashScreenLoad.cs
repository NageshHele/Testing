using System;
using DevExpress.XtraSplashScreen;

namespace Prime
{
    public partial class SplashScreenLoad : SplashScreen
    {
        public SplashScreenLoad()
        {
            InitializeComponent();
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        public enum SplashScreenCommand
        {
        }
    }
}