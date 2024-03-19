using DevExpress.XtraEditors;
using NerveLog;
using Newtonsoft.Json;
using Prime.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace Prime
{
    public partial class form_Settings : XtraForm
    {
        ConcurrentDictionary<string, int> dict_BaseValue = new ConcurrentDictionary<string, int>();
        ConcurrentDictionary<string, string> dict_CustomColumnNames = new ConcurrentDictionary<string, string>();
        ConcurrentDictionary<string, int> dict_CustomDigits = new ConcurrentDictionary<string, int>();

        ValueSigns _ValueSigns = new ValueSigns();

        List<string> list_ColList = new List<string>();

        NerveLogger _logger;

        // Added by Snehadri on 27JUN2022
        bool IsSelIndChanegd = false;

        public form_Settings()
        {
            InitializeComponent();

            _logger = CollectionHelper._logger;
        }

        private void frm_Settings_Shown(object sender, EventArgs e)
        {
            try
            {

                list_ColList = new List<string>(CollectionHelper.hs_ColumnNames);

                var CONInfo = ConfigurationManager.AppSettings;

                txt_EngineIP.Text = CONInfo["ENGINE-SERVER-IP"].ToString();
                txt_TradePORT.Text = CONInfo["ENGINE-SERVER-TRADE-PORT"].ToString();
                txt_SpanPORT.Text = CONInfo["ENGINE-SERVER-SPAN-PORT"].ToString();
                txt_HBPORT.Text = CONInfo["ENGINE-SERVER-HB-PORT"].ToString();
                spEdit_RuleAlertTime.Text = CONInfo["RULE-ALERT-INTERVAL"].ToString(); // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
                spEdit_PopupSnooze.Text = CONInfo["POPUP-SNOOZE"].ToString();
                spEdit_DataFontSize.Text = CONInfo["DATA-FONT-SIZE"].ToString(); // Added by Snehadri on 30AUG2021 for changing font size
                spEdit_FooterFontSize.Text = CONInfo["FOOTER-FONT-SIZE"].ToString();
                spEdit_Autosave.Text = CONInfo["AUTOSAVE-INTERVAL"].ToString(); // Added by Snehadri on20OCT2021

                try
                {
                    //added on 16APR2021 by Amey
                    chk_VaRValue.CheckState = Convert.ToBoolean(CONInfo["VAR-REVERSE-MODE"]) ? CheckState.Checked : CheckState.Unchecked;
                    chk_VerticalLines.CheckState = Convert.ToBoolean(CONInfo["VERTICAL-LINES"]) ? CheckState.Checked : CheckState.Unchecked; //Added by Akshay on 24-08-2021
                    chk_FullVaR.CheckState = Convert.ToBoolean(CONInfo["FULL-VAR"]) ? CheckState.Checked : CheckState.Unchecked; //Added by Akshay on 24-08-2021

                }
                catch(Exception) { chk_VaRValue.CheckState = CheckState.Checked; }

                if (Convert.ToBoolean(CONInfo["DEBUG-MODE"]))
                    chk_Debug.CheckState = CheckState.Checked;
                else
                    chk_Debug.CheckState = CheckState.Unchecked; 

                #region Custom Base Values

                //var arr_Properties = new string[] { nameof(CPParent.MTM), nameof(CPParent.IntradayMTM), nameof(CPParent.AbsDelta), nameof(CPParent.AbsGamma),
                //    nameof(CPParent.Delta), nameof(CPParent.Gamma), nameof(CPParent.Theta), nameof(CPParent.Vega), nameof(CPParent.EquityAmount),
                //    nameof(CPParent.PayInPayOut), nameof(CPParent.VARMargin), nameof(CPParent.DayNetPremium), nameof(CPParent.ROV), nameof(CPParent.DeliveryMargin),
                //    nameof(CPParent.VAR), nameof(CPParent.Span), nameof(CPParent.Exposure), nameof(CPParent.MarginUtil), nameof(CPParent.ExpiryMargin),
                //    nameof(CPParent.EODMargin), nameof(CPParent.DeltaAmount) , nameof(CPUnderlying.PosExpoOPT), nameof(CPUnderlying.PosExpoFUT), nameof(CPParent.Turnover)};

                //dict_BaseValue.TryAdd("All", 0);

                //added on 24FEB2021 by Amey
                if (File.Exists(Application.StartupPath + "\\" + "Report\\BaseValues.txt"))
                {
                    string txt = File.ReadAllText(Application.StartupPath + "\\" + "Report\\BaseValues.txt");
                    var tx = JsonConvert.DeserializeObject<ConcurrentDictionary<string, int>>(txt);
                    if (tx != null)
                        dict_BaseValue = tx;
                }

                //changed to arr_Properties on 24MAY2021 by Amey. To avoid unneccesary column in ComboBox.
                //changed location on 16APR2021 by Amey. Logic was written like an idiot by Amey.
                foreach (var item in list_ColList)
                {
                    cmb_Property.Properties.Items.Add(item);
                    dict_BaseValue.TryAdd(item, 0);
                }

                cmb_Property.Properties.Items.Insert(0, "All"); 

                #endregion

                #region Custom Column Captions

                //added on 22MAR2021 by Amey
                if (File.Exists(Application.StartupPath + "\\" + "Report\\CustomColumns.json"))
                {
                    string txt = File.ReadAllText(Application.StartupPath + "\\" + "Report\\CustomColumns.json");
                    var tx = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(txt);
                    if (tx != null)
                        dict_CustomColumnNames = tx;
                }

                //added on 07APR2021 by Amey
                foreach (var item in list_ColList)
                    dict_CustomColumnNames.TryAdd(item, "");

                foreach (var ColumnName in dict_CustomColumnNames.Keys)
                    cmb_ColumnName.Properties.Items.Add(ColumnName); 

                #endregion

                #region Custom Digits

                //arr_Properties = new string[] { nameof(CPParent.MTM), nameof(CPParent.IntradayMTM), nameof(CPParent.TheoreticalMTM), nameof(CPParent.Span), nameof(CPParent.Exposure),
                //    nameof(CPParent.MarginUtil), nameof(CPParent.ExpiryMargin), nameof(CPParent.EODMargin), nameof(CPPositions.BEP), nameof(CPPositions.IntradayBEP),
                //    nameof(CPPositions.IntradayBuyAvg), nameof(CPPositions.IntradaySellAvg), nameof(CPParent.VAR), nameof(CPParent.VARMargin),
                //    nameof(CPParent.DeliveryMargin), nameof(CPParent.PayInPayOut), nameof(CPParent.DayNetPremium), nameof(CPParent.ROV), nameof(CPParent.Turnover)};

                

                //added on 24MAY2021 by Amey
                if (File.Exists(Application.StartupPath + "\\Report\\CustomDigits.json"))
                {
                    string txt = File.ReadAllText(Application.StartupPath + "\\Report\\CustomDigits.json");
                    var tx = JsonConvert.DeserializeObject<ConcurrentDictionary<string, int>>(txt);
                    if (tx != null)
                        dict_CustomDigits = tx;
                }

                //changed to arr_Properties on 24MAY2021 by Amey. To avoid unneccesary column in ComboBox.
                foreach (var item in list_ColList)
                {
                    cmb_DigitProperty.Properties.Items.Add(item);
                    dict_CustomDigits.TryAdd(item, 2);
                }

                cmb_DigitProperty.Properties.Items.Insert(0, "All");

                #endregion

                #region Custom Value Reversal

                //added on 25MAY2021 by Amey
                if (File.Exists(Application.StartupPath + "\\Report\\ValueSigns.json"))
                {
                    string txt = File.ReadAllText(Application.StartupPath + "\\Report\\ValueSigns.json");
                    var tx = JsonConvert.DeserializeObject<ValueSigns>(txt);
                    if (tx != null)
                        _ValueSigns = tx;
                }

                chk_Delta.CheckState = _ValueSigns.Delta == 1 ? CheckState.Unchecked : CheckState.Checked;
                chk_DeltaAmount.CheckState = _ValueSigns.DeltaAmt == 1 ? CheckState.Unchecked : CheckState.Checked;
                chk_Gamma.CheckState = _ValueSigns.Gamma == 1 ? CheckState.Unchecked : CheckState.Checked;
                chk_Theta.CheckState = _ValueSigns.Theta == 1 ? CheckState.Unchecked : CheckState.Checked;
                chk_VaRValue.CheckState = _ValueSigns.VaR == 1 ? CheckState.Unchecked : CheckState.Checked;
                chk_Vega.CheckState = _ValueSigns.Vega == 1 ? CheckState.Unchecked : CheckState.Checked;

                #endregion
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void cmb_Property_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string SelectedSymbol = cmb_Property.SelectedItem.ToString();
                if (SelectedSymbol != "--SELECT--")
                    trackBar_BaseValue.Value = dict_BaseValue[SelectedSymbol];
                else
                    trackBar_BaseValue.Value = 0;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void frm_Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                File.WriteAllText(Application.StartupPath + "\\" + "Report\\BaseValues.txt", JsonConvert.SerializeObject(dict_BaseValue));

                //added on 22MAR2021 by Amey
                File.WriteAllText(Application.StartupPath + "\\" + "Report\\CustomColumns.json", JsonConvert.SerializeObject(dict_CustomColumnNames));

                //added on 24MAY2021 by Amey
                File.WriteAllText(Application.StartupPath + "\\" + "Report\\CustomDigits.json", JsonConvert.SerializeObject(dict_CustomDigits));

                Configuration _Config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                _Config.AppSettings.Settings["ENGINE-SERVER-IP"].Value = txt_EngineIP.Text;
                _Config.AppSettings.Settings["ENGINE-SERVER-TRADE-PORT"].Value = txt_TradePORT.Text;
                _Config.AppSettings.Settings["ENGINE-SERVER-SPAN-PORT"].Value = txt_SpanPORT.Text;
                _Config.AppSettings.Settings["ENGINE-SERVER-HB-PORT"].Value = txt_HBPORT.Text;
                _Config.AppSettings.Settings["DEBUG-MODE"].Value = (chk_Debug.CheckState == CheckState.Checked ? true : false).ToString(); // Changed by Snehadri on 09SEP2021
                _Config.AppSettings.Settings["VERTICAL-LINES"].Value = (chk_VerticalLines.CheckState == CheckState.Checked ? true : false).ToString(); //Added by Akshay on 24-08-2021
                _Config.AppSettings.Settings["FULL-VAR"].Value = (chk_FullVaR.CheckState == CheckState.Checked ? true : false).ToString(); //Added by Akshay on 24-08-2021
                // Added by Snehadri on 30AUG2021 for changing font size
                //_Config.AppSettings.Settings["DATA-FONT-SIZE"].Value = (int.Parse(spEdit_DataFontSize.Text) < 8 ) ? "8" : (int.Parse(spEdit_DataFontSize.Text) > 20) ? "20" : spEdit_DataFontSize.Text;
                //_Config.AppSettings.Settings["FOOTER-FONT-SIZE"].Value = (int.Parse(spEdit_FooterFontSize.Text) < 6) ? "8" : (int.Parse(spEdit_FooterFontSize.Text) > 20) ? "20" : spEdit_FooterFontSize.Text;

                _Config.AppSettings.Settings["DATA-FONT-SIZE"].Value = spEdit_DataFontSize.Text;
                _Config.AppSettings.Settings["FOOTER-FONT-SIZE"].Value = spEdit_FooterFontSize.Text;


                // Added by Snehadri on 19JUL2021 for expression Column and Rule Builder
                _Config.AppSettings.Settings["RULE-ALERT-INTERVAL"].Value = (int.Parse(spEdit_RuleAlertTime.Text) < 30) ? "30" : spEdit_RuleAlertTime.Text;
                _Config.AppSettings.Settings["POPUP-SNOOZE"].Value = spEdit_PopupSnooze.Text;

                _Config.AppSettings.Settings["AUTOSAVE-INTERVAL"].Value = spEdit_Autosave.Text;

                _Config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                //added on 15APR2021 by Amey
                //CollectionHelper.IsVaRValueReversed = chk_VaRValue.Checked;

                _ValueSigns.Delta = chk_Delta.Checked ? -1 : 1;
                _ValueSigns.Gamma = chk_Gamma.Checked ? -1 : 1;
                _ValueSigns.Theta = chk_Theta.Checked ? -1 : 1;
                _ValueSigns.Vega = chk_Vega.Checked ? -1 : 1;
                _ValueSigns.DeltaAmt = chk_DeltaAmount.Checked ? -1 : 1;
                _ValueSigns.VaR = chk_VaRValue.Checked ? -1 : 1;

                //added on 25MAY2021 by Amey
                File.WriteAllText(Application.StartupPath + "\\" + "Report\\ValueSigns.json", JsonConvert.SerializeObject(_ValueSigns));
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void trackBar_BaseValue_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string SelectedSymbol = cmb_Property.SelectedItem.ToString();
                if (SelectedSymbol != "--SELECT--")
                {
                    if (SelectedSymbol == "All")
                    {
                        foreach (var _Symbol in dict_BaseValue.Keys)
                            dict_BaseValue[_Symbol] = trackBar_BaseValue.Value;
                    }
                    else
                        dict_BaseValue[SelectedSymbol] = trackBar_BaseValue.Value;
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void cmb_ColumnName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Added by Snehadri on 27JUN2022
                IsSelIndChanegd = true;

                //changed to TryGetValue on 27MAY2021 by Amey
                if (dict_CustomColumnNames.TryGetValue(cmb_ColumnName.SelectedItem.ToString(), out string _CName))
                    txt_CustomCaption.Text = _CName;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void txt_CustomCaption_Leave(object sender, EventArgs e)
        {
            
        }

        private void cmb_DigitProperty_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //changed to TryGetValue on 27MAY2021 by Amey
                if (dict_CustomDigits.TryGetValue(cmb_DigitProperty.SelectedItem.ToString(), out int _Digit))
                    radGrp_Digits.EditValue = _Digit;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void radGrp_Digits_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string _SelectedProperty = cmb_DigitProperty.SelectedItem.ToString();
                if (_SelectedProperty != "--SELECT--")
                {
                    if (_SelectedProperty == "All")
                    {
                        foreach (var _Property in dict_CustomDigits.Keys)
                            dict_CustomDigits[_Property] = Convert.ToInt32(radGrp_Digits.EditValue);
                        dict_CustomDigits.TryAdd("All", Convert.ToInt32(radGrp_Digits.EditValue));
                    }
                    else
                        dict_CustomDigits[_SelectedProperty] = Convert.ToInt32(radGrp_Digits.EditValue);
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
        }

        private void txt_CustomCaption_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                if (!IsSelIndChanegd && dict_CustomColumnNames.ContainsKey(cmb_ColumnName.SelectedItem.ToString()))
                    dict_CustomColumnNames[cmb_ColumnName.SelectedItem.ToString()] = txt_CustomCaption.Text;

                // Added by Snehadri on 27JUN2022
                IsSelIndChanegd = false;
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
    }
}