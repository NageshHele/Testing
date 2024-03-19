using Prime.Helper;
using System;
using System.Windows.Forms;

namespace Prime.UI
{
    public partial class uc_Indicators : DevExpress.XtraEditors.XtraUserControl
    {
        string PreviousHeartBeatTick = "";

        private uc_Indicators()
        {
            InitializeComponent();
        }

        public static uc_Indicators Instance { get; private set; }

        public static void Initialise()
        {
            if (Instance is null)
                Instance = new uc_Indicators();
        }

        /// <summary>
        /// Will be called to update indicatios uptop.
        /// </summary>
        /// <param name="HeartBeatTick">Format : {FOLTT}_{CMLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{isGatewayConnected}_{isSpanConnected}_{NIFTY}_{BANKNIFTY}</param>
        internal void UpdateIndicators(string HeartBeatTick = "")
        {
            try
            {
                string[] arr_fields = (HeartBeatTick == "" ? PreviousHeartBeatTick : HeartBeatTick).Split('_');
                if (arr_fields.Length > 5)
                {
                    //added on 05APR2021 by Amey
                    PreviousHeartBeatTick = HeartBeatTick;

                    //changed on 07JAN2021 by Amey
                    DateTime dte_FOLastTickTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[0] == "" ? "0" : arr_fields[0]));

                    //added on 07JAN2021 by Amey
                    DateTime dte_CMLastTickTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[1] == "" ? "0" : arr_fields[1]));

                    //changed Index number on 07JAN2021 by Amey
                    DateTime dte_FOLastTradeTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[2] == "" ? "0" : arr_fields[2]));

                    //added on 05APR2021 by Amey
                    DateTime dte_CMLastTradeTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[3] == "" ? "0" : arr_fields[3]));

                    //added on 17MAY2021 by Amey
                    DateTime dte_SpanComputeTime = DateTime.Now;
                    bool isSpanInfoAvailable = false;
                    bool SpanredTickInd = true;
                    if (arr_fields.Length > 9)
                    {
                        isSpanInfoAvailable = true;
                        dte_SpanComputeTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[8] == "" ? "0" : arr_fields[8]));

                        if ((DateTime.Now - dte_SpanComputeTime).TotalSeconds < 60)
                            SpanredTickInd = false;
                    }

                    bool FOredTickInd = true;
                    if ((DateTime.Now - dte_FOLastTickTime).TotalSeconds < 60)
                        FOredTickInd = false;

                    //added on 07JAN2021 by Amey
                    bool CMredTickInd = true;
                    if ((DateTime.Now - dte_CMLastTickTime).TotalSeconds < 60)
                        CMredTickInd = false;

                    bool redFOTradeInd = true;
                    if ((DateTime.Now - dte_FOLastTradeTime).TotalSeconds < 60)
                        redFOTradeInd = false;

                    bool redCMTradeInd = true;
                    if ((DateTime.Now - dte_CMLastTradeTime).TotalSeconds < 60)
                        redCMTradeInd = false;

                    if (IsHandleCreated)  //Added check by Akshay on 24-12-2020
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            //changed text on 26NOV2020 by Amey
                            ind_FOLastTickTime.ToolTip = "FO Tick : " + dte_FOLastTickTime;
                            ind_ActiveFOLastTickTime.ToolTip = "FO Tick : " + dte_FOLastTickTime;
                            lbl_FOLastTickTime.Text = "FO Tick : " + dte_FOLastTickTime;

                            ind_FOLastTickTime.Visible = FOredTickInd;
                            ind_ActiveFOLastTickTime.Visible = !FOredTickInd;

                            //added on 07JAN2021 by Amey
                            ind_CMLastTickTime.ToolTip = "CM Tick : " + dte_CMLastTickTime;
                            ind_ActiveCMLastTickTime.ToolTip = "CM Tick : " + dte_CMLastTickTime;
                            lbl_CMLastTickTime.Text = "CM Tick : " + dte_CMLastTickTime;

                            ind_CMLastTickTime.Visible = CMredTickInd;
                            ind_ActiveCMLastTickTime.Visible = !CMredTickInd;

                            //changed text on 26NOV2020 by Amey
                            ind_FOLastTradeTime.ToolTip = "FO Trade : " + dte_FOLastTradeTime;
                            ind_ActiveFOLastTradeTime.ToolTip = "FO Trade : " + dte_FOLastTradeTime;
                            lbl_FOLastTradeTime.Text = "FO Trade : " + dte_FOLastTradeTime;

                            ind_FOLastTradeTime.Visible = redFOTradeInd;
                            ind_ActiveFOLastTradeTime.Visible = !redFOTradeInd;

                            //added on 05APR2021 by Amey
                            ind_CMLastTradeTime.ToolTip = "CM Trade : " + dte_CMLastTradeTime;
                            ind_ActiveCMLastTradeTime.ToolTip = "CM Trade : " + dte_CMLastTradeTime;
                            lbl_CMLastTradeTime.Text = "CM Trade : " + dte_CMLastTradeTime;

                            ind_CMLastTradeTime.Visible = redCMTradeInd;
                            ind_ActiveCMLastTradeTime.Visible = !redCMTradeInd;

                            //changed Index number on 07JAN2021 by Amey
                            if (arr_fields[4] == "0")
                            {
                                ind_GatewayDisconnected.Visible = true;
                                ind_GatewayConnected.Visible = false;
                            }
                            else
                            {
                                ind_GatewayDisconnected.Visible = false;
                                ind_GatewayConnected.Visible = true;
                            }

                            //changed Index number on 07JAN2021 by Amey
                            if (arr_fields[5] == "0")
                            {
                                ind_SpanDisconnected.Visible = true;
                                ind_SpanConnected.Visible = false;
                            }
                            else
                            {
                                ind_SpanDisconnected.Visible = false;
                                ind_SpanConnected.Visible = true;
                            }

                            if (isSpanInfoAvailable)
                            {
                                //added on 17MAY2021 by Amey
                                ind_SpanComputeTime.ToolTip = "Span : " + dte_SpanComputeTime;
                                ind_ActiveSpanComputeTime.ToolTip = "Span : " + dte_SpanComputeTime;
                                lbl_SpanComputeTime.Text = "Span : " + dte_SpanComputeTime;

                                ind_SpanComputeTime.Visible = SpanredTickInd;
                                ind_ActiveSpanComputeTime.Visible = !SpanredTickInd;

                                lbl_LatestSpanFileName.Text = "Span : " + arr_fields[9];
                            }
                        }));
                    }
                }
            }
            catch (Exception ee) { CollectionHelper._logger.Error(ee); }
        }

        internal void UpdateSocketIndicator(bool isEngineConnected, string Message)
        {
            try
            {
                if (IsHandleCreated)  //Added check by Akshay on 24-12-2020
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        ind_EngineConnected.ToolTip = Message;
                        ind_EngineDisconnected.ToolTip = Message;

                        ind_EngineDisconnected.Visible = !isEngineConnected;
                        ind_EngineConnected.Visible = isEngineConnected;

                        if (!isEngineConnected)
                        {
                            ind_GatewayDisconnected.Visible = !isEngineConnected;
                            ind_GatewayConnected.Visible = isEngineConnected;

                            ind_SpanDisconnected.Visible = !isEngineConnected;
                            ind_SpanConnected.Visible = isEngineConnected;
                        }
                    }));
                }
            }
            catch (Exception ee) { CollectionHelper._logger.Error(ee); }
        }
    }
}
