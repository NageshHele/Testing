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
        /// 
        //$"{FOLTT}_{CMLTT}_{CDLTT}_{_FOLastTradeTime}_{_CMLastTradeTime}_{_CDLastTradeTime}_{isGatewayConnected}" +
                //$"_{isSpanConnected}_{isCDSSpanConnected}_{dict_IndexLTP["NIFTY"]}_{dict_IndexLTP["BANKNIFTY"]}_{_SpanComputeTime}_{_CDSSpanComputeTime}_{_LatestSpanFileName}_{_LatestCDSSpanFileName}")); //added by Akshay on 16-11-2021 for CD LTT


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

                    DateTime dte_CDLastTickTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[2] == "" ? "0" : arr_fields[2]));

                    DateTime dte_BSEFOLastTickTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[3] == "" ? "0" : arr_fields[3]));


                    //changed Index number on 07JAN2021 by Amey
                    DateTime dte_FOLastTradeTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[4] == "" ? "0" : arr_fields[4]));

                    //added on 05APR2021 by Amey
                    DateTime dte_CMLastTradeTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[5] == "" ? "0" : arr_fields[5]));

                    DateTime dte_CDLastTradeTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[6] == "" ? "0" : arr_fields[6]));

                    DateTime dte_BSECMLastTradeTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[7] == "" ? "0" : arr_fields[7]));

                    DateTime dte_BSEFOLastTradeTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[8] == "" ? "0" : arr_fields[8]));//added by Omkar

                    //added on 17MAY2021 by Amey
                    DateTime dte_SpanComputeTime = DateTime.Now;
                    bool isSpanInfoAvailable = false;
                    bool SpanredTickInd = true;
                    if (arr_fields.Length > 14)
                    {
                        isSpanInfoAvailable = true;
                        dte_SpanComputeTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[14] == "" ? "0" : arr_fields[14]));

                        if ((DateTime.Now - dte_SpanComputeTime).TotalSeconds < 60)
                            SpanredTickInd = false;
                    }

                    //added on 17MAY2021 by Amey
                    DateTime dte_CDSSpanComputeTime = DateTime.Now;
                    bool isCDSSpanInfoAvailable = false;
                    bool CDSSpanredTickInd = true;
                    if (arr_fields.Length > 15)
                    {
                        isCDSSpanInfoAvailable = true;
                        dte_CDSSpanComputeTime = CommonFunctions.ConvertFromUnixTimestamp(Convert.ToDouble(arr_fields[15] == "" ? "0" : arr_fields[15]));

                        if ((DateTime.Now - dte_CDSSpanComputeTime).TotalSeconds < 60)
                            CDSSpanredTickInd = false;
                    }

                    bool FOredTickInd = true;
                    if ((DateTime.Now - dte_FOLastTickTime).TotalSeconds < 60)
                        FOredTickInd = false;

                    //added on 07JAN2021 by Amey
                    bool CMredTickInd = true;
                    if ((DateTime.Now - dte_CMLastTickTime).TotalSeconds < 60)
                        CMredTickInd = false;

                    //added on 07JAN2021 by Amey
                    bool CDredTickInd = true;
                    if ((DateTime.Now - dte_CDLastTickTime).TotalSeconds < 60)
                        CDredTickInd = false;

                    //added by Omkar
                    bool BSEFOredTickInd = true;
                    if ((DateTime.Now - dte_BSEFOLastTickTime).TotalSeconds < 60)
                        BSEFOredTickInd = false;

                    bool redFOTradeInd = true;
                    if ((DateTime.Now - dte_FOLastTradeTime).TotalSeconds < 60)
                        redFOTradeInd = false;

                    bool redCMTradeInd = true;
                    if ((DateTime.Now - dte_CMLastTradeTime).TotalSeconds < 60)
                        redCMTradeInd = false;

                    bool redCDTradeInd = true;
                    if ((DateTime.Now - dte_CDLastTradeTime).TotalSeconds < 60)
                        redCDTradeInd = false;

                    bool redBSECMTradeInd = true;
                    if ((DateTime.Now - dte_BSECMLastTradeTime).TotalSeconds < 60)
                        redBSECMTradeInd = false;
                    
                    //added by Omkar
                    bool redBSEFOTradeInd = true;
                    if ((DateTime.Now - dte_BSEFOLastTradeTime).TotalSeconds < 60)
                        redBSEFOTradeInd = false;

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

                            //added on 07JAN2021 by Amey
                            ind_CDLastTickTime.ToolTip = "CD Tick : " + dte_CDLastTickTime;
                            ind_ActiveCDLastTickTime.ToolTip = "CD Tick : " + dte_CDLastTickTime;
                            lbl_CDLastTickTime.Text = "CD Tick : " + dte_CDLastTickTime;

                            ind_CDLastTickTime.Visible = CDredTickInd;
                            ind_ActiveCDLastTickTime.Visible = !CDredTickInd;

                            //added by Omkar
                            ind_BSEFOLastTickTime.ToolTip = "BSEFO Tick : " + dte_BSEFOLastTickTime;
                            ind_ActiveBSEFOLastTickTime.ToolTip = "BSEFO Tick : " + dte_BSEFOLastTickTime;
                            lbl_BSEFOLastTickTime.Text = "BSEFO Tick : " + dte_BSEFOLastTickTime;

                            ind_BSEFOLastTickTime.Visible = BSEFOredTickInd;
                            ind_ActiveBSEFOLastTickTime.Visible = !BSEFOredTickInd;

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

                            //added on 05APR2021 by Amey
                            ind_CDLastTradeTime.ToolTip = "CD Trade : " + dte_CDLastTradeTime;
                            ind_ActiveCDLastTradeTime.ToolTip = "CD Trade : " + dte_CDLastTradeTime;
                            lbl_CDLastTradeTime.Text = "CD Trade : " + dte_CDLastTradeTime;

                            ind_CDLastTradeTime.Visible = redCDTradeInd;
                            ind_ActiveCDLastTradeTime.Visible = !redCDTradeInd;

                            //added on 05APR2021 by Amey
                            ind_BSECMLastTradeTime.ToolTip = "BSECM Trade : " + dte_BSECMLastTradeTime;
                            ind_ActiveBSECMLastTradeTime.ToolTip = "BSECM Trade : " + dte_BSECMLastTradeTime;
                            lbl_BSECMLastTradeTime.Text = "BSECM Trade : " + dte_BSECMLastTradeTime;

                            ind_BSECMLastTradeTime.Visible = redBSECMTradeInd;
                            ind_ActiveBSECMLastTradeTime.Visible = !redBSECMTradeInd;

                            //added by Omkar
                            ind_BSEFOLastTradeTime.ToolTip = "BSEFO Trade : " + dte_BSEFOLastTradeTime;
                            ind_ActiveBSEFOLastTradeTime.ToolTip = "BSEFO Trade : " + dte_BSEFOLastTradeTime;
                            lbl_BSEFOLastTradeTime.Text = "BSEFO Trade : " + dte_BSEFOLastTradeTime;

                            ind_BSEFOLastTradeTime.Visible = redBSEFOTradeInd;
                            ind_ActiveBSEFOLastTradeTime.Visible = !redBSEFOTradeInd;

                            //changed Index number on 07JAN2021 by Amey
                            if (arr_fields[9] == "0")
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
                            if (arr_fields[10] == "0")
                            {
                                ind_SpanDisconnected.Visible = true;
                                ind_SpanConnected.Visible = false;
                            }
                            else
                            {
                                ind_SpanDisconnected.Visible = false;
                                ind_SpanConnected.Visible = true;
                            }

                            if (arr_fields[11] == "0")
                            {
                                ind_CDSSpanDisconnected.Visible = true;
                                ind_CDSSpanConnected.Visible = false;
                            }
                            else
                            {
                                ind_CDSSpanDisconnected.Visible = false;
                                ind_CDSSpanConnected.Visible = true;
                            }

                            if (isSpanInfoAvailable)
                            {
                                //added on 17MAY2021 by Amey
                                ind_SpanComputeTime.ToolTip = "Span : " + dte_SpanComputeTime;
                                ind_ActiveSpanComputeTime.ToolTip = "Span : " + dte_SpanComputeTime;
                                lbl_SpanComputeTime.Text = "Span : " + dte_SpanComputeTime;

                                ind_SpanComputeTime.Visible = SpanredTickInd;
                                ind_ActiveSpanComputeTime.Visible = !SpanredTickInd;

                                lbl_LatestSpanFileName.Text = "Span : " + arr_fields[16];
                            }

                            if (isCDSSpanInfoAvailable)
                            {
                                //added on 17MAY2021 by Amey
                                ind_CDSSpanComputeTime.ToolTip = "CDS Span : " + dte_CDSSpanComputeTime;
                                ind_ActiveCDSSpanComputeTime.ToolTip = "CDS Span : " + dte_CDSSpanComputeTime;
                                lbl_CDSSpanComputeTime.Text = "CDS Span : " + dte_CDSSpanComputeTime;

                                ind_CDSSpanComputeTime.Visible = CDSSpanredTickInd;
                                ind_ActiveCDSSpanComputeTime.Visible = !CDSSpanredTickInd;

                                lbl_LatestCDSSpanFileName.Text = "CDS Span : " + arr_fields[17];
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

                            ind_CDSSpanDisconnected.Visible = !isEngineConnected;
                            ind_CDSSpanConnected.Visible = isEngineConnected;
                        }
                    }));
                }
            }
            catch (Exception ee) { CollectionHelper._logger.Error(ee); }
        }

        private void labelControl1_Click(object sender, EventArgs e)
        {

        }

        private void ind_ActiveCDLastTradeTime_Click(object sender, EventArgs e)
        {

        }

        private void lbl_LatestCDSSpanFileName_Click(object sender, EventArgs e)
        {

        }
    }
}
