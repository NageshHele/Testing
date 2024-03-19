using n.Structs;
using NerveLog;
using Prime.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prime.Core_Logic
{
    internal class nCompute
    {
        NerveLogger _logger;

        /// <summary>
        /// Client Portfolio Parent row object.
        /// </summary>
        CPParent _CPParent = null;

        /// <summary>
        ///Underlying Clients Parent row object.
        /// </summary>
        UCParent _UCParent = null;

        /// <summary>
        /// Fetched from ClientID^ALL key from dict_NPLValues if avaviable.
        /// </summary>
        double ClientLevelNPL = 0;

        /// <summary>
        /// Fetched from ClientID key from dict_ClientInfo.MTDValue if avaviable.
        /// </summary>
        double ClientLevelMTD = 0;

        /// <summary>
        /// Max keys count of dict_DaysPercentage;
        /// </summary>
        int ExpiryThreshHold = 0;

        string _ClientID = string.Empty;
        string _Underlying = string.Empty;

        #region Flags

        /// <summary>
        /// Set true when Task is created. False after task is complete.
        /// </summary>
        internal bool isComputing = false;

        /// <summary>
        /// Set true when at least one Underlying row is added to CPParent.bList_Underlying.
        /// </summary>
        bool isInitialLoadCPUnderlyingSuccess = false;

        /// <summary>
        /// Set true when at least one Client row is added to CUCParent.bList_Clients.
        /// </summary>
        bool isInitialLoadUCClientsSuccess = false;

        /// <summary>
        /// Set true when CPParent is expanded.
        /// </summary>
        bool isCPParentExpanded = false;

        /// <summary>
        /// Set true when CPParent.bList_Underlying is expanded.
        /// </summary>
        bool isCPUnderlyingExpanded = false;

        /// <summary>
        /// Set true when VaRDistribution form is shown.
        /// </summary>
        bool isVaRDistributionShown = false;

        /// <summary>
        /// Set true when UCClients is expanded.
        /// </summary>
        bool isUCParentExpanded = false;

        #endregion

        /// <summary>
        /// List of expanded underluing rows. ClientID_Underlying.
        /// </summary>
        List<string> list_ExpandedUnderlying = new List<string>();

        #region Dictionaries

        /// <summary>
        /// Key : Underlying | Value : State
        /// </summary>
        Dictionary<string, bool> dict_IsInitialCPPositionLoadSuccess = new Dictionary<string, bool>();

        /// <summary>
        /// Key : Underlying | Value : Consolidated Underlying Values   
        /// </summary>
        Dictionary<string, UnderlyingLevelInfo> dict_UnderlyingLevel = new Dictionary<string, UnderlyingLevelInfo>();

        /// <summary>
        /// Key : ClientID | Value : Consolidated ClientLevel Values   
        /// </summary>
        Dictionary<string, ClientLevelInfo> dict_ClientLevel = new Dictionary<string, ClientLevelInfo>();

        /// <summary>
        /// Key : Segment|Token | Value : Underlying if not cleared yet, "" if cleared.
        /// </summary>
        Dictionary<string, string> dict_TokensRemove = new Dictionary<string, string>();

        /// <summary>
        /// Key : Expiry | Value : Days
        /// </summary>
        Dictionary<double, int> dict_ExpiryDays = new Dictionary<double, int>(); //Added by Akshay on 24-03-2021

        /// <summary>
        /// Key : IVIdx | Value : List of VaRInfo on respective IVIdx
        /// </summary>
        Dictionary<int, List<VaRInfo>> dList_MaxLossUnderlyingWise = new Dictionary<int, List<VaRInfo>>();

        /// <summary>
        /// Key : Underlying | Value : List of MaxVaRinfo
        /// </summary>
        Dictionary<string, List<MaxVaRStruct>> dict_MaxLossUnderlyingWise = new Dictionary<string, List<MaxVaRStruct>>();

        //Added by Akshay on 23-07-2021 for NPL
        /// <summary>
        /// Key : Underlying | Value : NPL_Values
        /// </summary>
        ConcurrentDictionary<string, double> dict_UnderlyingLevelNPL = new ConcurrentDictionary<string, double>();

        //Added by Akshay on 23-08-2021 for UpsideDown VaR
        /// <summary>
        /// Key : Underlying | Value : List of MaxVaRinfo
        /// </summary>
        ConcurrentDictionary<string, ConcurrentDictionary<int, ConcurrentDictionary<int, VaRInfo>>> dict_UnderlyingWiseVaR = new ConcurrentDictionary<string, ConcurrentDictionary<int, ConcurrentDictionary<int, VaRInfo>>>();
        //ConcurrentDictionary<string, ConcurrentDictionary<int, VaRInfo>> dict_UnderlyingWiseVaR = new ConcurrentDictionary<string, ConcurrentDictionary<int, VaRInfo>>();

        /// <summary>
        /// Key : Underlying | Value : List of MaxVaRinfo
        /// </summary>
        ConcurrentDictionary<string, ConcurrentDictionary<int, double>> dict_UCUnderlyingWiseVaR = new ConcurrentDictionary<string, ConcurrentDictionary<int, double>>();


        #endregion

        double[] arr_VaRAtIV = new double[2] { 0, 0 };

        #region Client Portfolio

        internal nCompute(string ClientID, CPParent _CPParent)
        {
            _logger = CollectionHelper._logger;

            this._ClientID = ClientID;
            this._CPParent = _CPParent;

            var NPLKey = $"{ClientID}^ALL";

            //changed to TryGetValue on 27MAY2021 by Amey
            if (CollectionHelper.dict_NPLValues.TryGetValue(NPLKey, out double _NPL))
                ClientLevelNPL = DivideByBaseAndRound(_NPL, nameof(_CPParent.MTM));

            //added on 08APR2021 by Amey
            ClientLevelMTD = DivideByBaseAndRound(CollectionHelper.dict_ClientInfo[ClientID].MTDValue, nameof(_CPParent.MTM));

            //Added by Akshay on 25-03-2021
            ExpiryThreshHold = CollectionHelper.dict_DaysPercentage.Keys.Max();
        }

        internal void AddToClientQueue(List<ConsolidatedPositionInfo> list_ReceivedPositions, bool isCPParentExpanded, List<string> list_ExpandedUnderlying,
            bool isVaRDistributionShown)
        {
            //changed to TryGetValue on 27MAY2021 by Amey
            //added on 15MAR2021 by Amey
            if (CollectionHelper.dict_Priority.TryGetValue(_ClientID, out double _Priority))
                _CPParent.Priority = _Priority;

            isComputing = true;

            this.isCPParentExpanded = isCPParentExpanded || !isInitialLoadCPUnderlyingSuccess;
            //this.isCPUnderlyingExpanded = list_ExpandedUnderlying.Any() || !isInitialLoadCPUnderlyingSuccess;

            this.list_ExpandedUnderlying = list_ExpandedUnderlying;
            this.isVaRDistributionShown = isVaRDistributionShown;

            Task.Run(() => ComputeAndUpdatePortfolio(list_ReceivedPositions));
        }

        private void ComputeAndUpdatePortfolio(List<ConsolidatedPositionInfo> list_ReceivedPositions)
        {
            try
            {
                dict_MaxLossUnderlyingWise.Clear();
                dict_UnderlyingWiseVaR.Clear(); //Added by Akshay on 23-08-2021 for UpsideDown VaR

                //added on 11FEB2021 by Amey
                var dict_RecentPositions = list_ReceivedPositions.GroupBy(row => row.Underlying).ToDictionary(kvp => kvp.Key, kvp => kvp.ToList());

                var dict_OG = CollectionHelper.dict_OG;

                //Key : Segment|Token | Value : CPPositions
                var dict_Positions = new Dictionary<string, CPPositions>();

                //added on 11FEB2021 by Amey
                var _CLevelInfo = new ClientLevelInfo();
                dict_UnderlyingLevel.Clear();

                double CLevelVaR = 0, CLevelVaRAbs = 0, CurrIV = 0, _PayInPayOut = 0, _DayNetPremium = 0, _DayNetPremiumCDS = 0, CLevelDelMargin = 0;
                double CLevelScenario1 = 0, CLevelScenario2 = 0, CLevelScenario3 = 0, CLevelScenario4 = 0;  //Added by Akshay on 23-08-2021 for UpsideDown VaR
                // Added by Snehadri for banknifty and nifty Exposure 
                double _CLevelBankNiftyCEQty = 0, _CLevelBankNiftyPEQty = 0, _CLevelBankNiftyClosePrice = 0, _CLevelNiftyCEQty = 0, _CLevelNiftyPEQty = 0, _CLevelNiftyClosePrice = 0;
                var UpdateCPParent = false;

                foreach (var Underlying in dict_RecentPositions.Keys)
                {
                    //Iterating on all Underlying Level Positions

                    //added for testing
                    //if (Underlying != "CONCOR") continue;

                    bool _UpdateValues = false;

                    //added on 11FEB2021 by Amey
                    if (!dict_UnderlyingLevel.ContainsKey(Underlying))
                        dict_UnderlyingLevel.Add(Underlying, new UnderlyingLevelInfo());

                    //added on 06APR2021 by Amey
                    if (!dict_IsInitialCPPositionLoadSuccess.ContainsKey(Underlying))
                        dict_IsInitialCPPositionLoadSuccess.Add(Underlying, false);

                    //Added by Akshay on 23-08-2021 for UpsideDown VaR
                    if (CollectionHelper.IsFullVAR)
                    {
                        //changed to TryGetValue on 27MAY2021 by Amey
                        int OGFrom = -10, OGTo = 10;
                        if (dict_OG.TryGetValue(Underlying, out OGInfo _OGInfo))
                        {
                            OGFrom = _OGInfo.OGFrom;
                            OGTo = _OGInfo.OGTo;
                        }

                        for (int CurrOGIdx = OGFrom; CurrOGIdx <= OGTo; CurrOGIdx += 1)
                        {
                            //Iterating on OGRange

                            //Key : IVIdx | Value : List of List(VaRInfo)
                            //changed List(double[]) to List(VaRInfo) on 07DEC2020 by Amey
                            dList_MaxLossUnderlyingWise.Clear();
                            arr_VaRAtIV[0] = 0; arr_VaRAtIV[1] = 0;

                            for (int IVIdx = 0; IVIdx <= 1; IVIdx++)
                            {
                                //Iterating on Two types of IV

                                //This is to use values only once. Needed because the loops are running multiple times.
                                if (IVIdx == 0 && CurrOGIdx == OGFrom)
                                    _UpdateValues = true;
                                else
                                    _UpdateValues = false;

                                foreach (var _PositionInfo in dict_RecentPositions[Underlying])
                                {
                                    //Iterating on every position under Underlying specified above

                                    //added on 20APR2021 by Amey
                                    var ScripKey = $"{_PositionInfo.Segment}|{_PositionInfo.Token}";

                                    ///changed position on 07APR2021 by Amey
                                    if (_PositionInfo.LTP <= 0 || _PositionInfo.UnderlyingLTP <= 0)
                                    {
                                        //If LTP is <= 0, means the LTP is not available yet. Therefore skip the Position.

                                        if (CollectionHelper.IsDebug)
                                            _logger.Debug($"NOLTP|{_ClientID}|{_PositionInfo.ScripName}|{_PositionInfo.Token}|{_PositionInfo.LTP}|{_PositionInfo.UnderlyingLTP}");

                                        continue;
                                    }

                                    if (_PositionInfo.ScripType != en_ScripType.EQ && _PositionInfo.ExpiryTimeSpan.TotalDays <= 0)
                                    {
                                        if (_UpdateValues)
                                        {
                                            //If Expired, dont compute other values excluding what mentioned below.

                                            //changed on 20APR2021 by Amey
                                            if (!dict_TokensRemove.ContainsKey(ScripKey))
                                                dict_TokensRemove.Add(ScripKey, Underlying);

                                            double tFMTM = 0;
                                            double tOMTM = 0;
                                            double tEMTM = 0;

                                            double tIntradayFMTM = 0;
                                            double tIntradayOMTM = 0;
                                            double tIntradayEMTM = 0;

                                            //added on 24MAY2021 by Amey
                                            switch (_PositionInfo.ScripType)
                                            {
                                                case en_ScripType.EQ:
                                                    tEMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                    tIntradayEMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                    break;
                                                case en_ScripType.XX:
                                                    tFMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                    tIntradayFMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                    break;
                                                case en_ScripType.CE:
                                                case en_ScripType.PE:
                                                    tOMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                    tIntradayOMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                    break;
                                            }

                                            //changed on 11FEB2021 by Amey
                                            _CLevelInfo.FuturesMTM += tFMTM;
                                            _CLevelInfo.OptionsMTM += tOMTM;
                                            _CLevelInfo.EquityMTM += tEMTM;

                                            _CLevelInfo.IntradayFuturesMTM += tIntradayFMTM;
                                            _CLevelInfo.IntradayOptionsMTM += tIntradayOMTM;
                                            _CLevelInfo.IntradayEquityMTM += tIntradayEMTM;

                                            dict_UnderlyingLevel[Underlying].FuturesMTM += tFMTM;
                                            dict_UnderlyingLevel[Underlying].OptionsMTM += tOMTM;
                                            dict_UnderlyingLevel[Underlying].EquityMTM += tEMTM;

                                            dict_UnderlyingLevel[Underlying].IntradayFuturesMTM += tIntradayFMTM;
                                            dict_UnderlyingLevel[Underlying].IntradayOptionsMTM += tIntradayOMTM;
                                            dict_UnderlyingLevel[Underlying].IntradayEquityMTM += tIntradayEMTM;


                                            //Added by Akshay on 30-12-2021 for CDS
                                            double tCDSFMTM = 0;
                                            double tCDSOMTM = 0;

                                            double tCDSIntradayFMTM = 0;
                                            double tCDSIntradayOMTM = 0;

                                            //Added by Akshay on 28-12-2021 for CDS
                                            if (_PositionInfo.Segment == en_Segment.NSECD)
                                            {
                                                switch (_PositionInfo.ScripType)
                                                {
                                                    case en_ScripType.XX:
                                                        tCDSFMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                        tCDSIntradayFMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                        break;
                                                    case en_ScripType.CE:
                                                    case en_ScripType.PE:
                                                        tCDSOMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                        tCDSIntradayOMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                        break;
                                                }
                                            }

                                            //Added by Akshay on 28-12-2021 for CDS
                                            _CLevelInfo.CDSFuturesMTM += tCDSFMTM;
                                            _CLevelInfo.CDSOptionsMTM += tCDSOMTM;

                                            _CLevelInfo.CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                            _CLevelInfo.CDSIntradayOptionsMTM += tCDSIntradayOMTM;

                                            dict_UnderlyingLevel[Underlying].CDSFuturesMTM += tCDSFMTM;
                                            dict_UnderlyingLevel[Underlying].CDSOptionsMTM += tCDSOMTM;

                                            dict_UnderlyingLevel[Underlying].CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                            dict_UnderlyingLevel[Underlying].CDSIntradayOptionsMTM += tCDSIntradayOMTM;


                                            //added on 15APR2021 by Amey
                                            double tTMTM = DivideByBaseAndRound(_PositionInfo.TheoreticalMTM, nameof(_CPParent.MTM));
                                            _CLevelInfo.TheoreticalMTM += tTMTM;
                                            dict_UnderlyingLevel[Underlying].TheoreticalMTM += tTMTM;
                                        }

                                        if (CollectionHelper.IsDebug)
                                            _logger.Debug($"Expired|{_ClientID}|{_PositionInfo.ScripName}|{_PositionInfo.ExpiryTimeSpan.TotalDays}");

                                        continue;
                                    }

                                    if (CurrOGIdx < 0)
                                    {
                                        //changed on 01FEB2021 by Amey
                                        var OGABS = Math.Abs(OGFrom);
                                        double IVChange = (_PositionInfo.IVHigher - _PositionInfo.IVMiddle) / (OGABS == 0 ? 1 : OGABS);

                                        if (IVIdx == 1)
                                            CurrIV = _PositionInfo.IVHigher - (IVChange * (OGABS - Math.Abs(CurrOGIdx)));
                                        else
                                            CurrIV = _PositionInfo.IVMiddle;
                                    }
                                    else if (CurrOGIdx >= 0)
                                    {
                                        //changed on 01FEB2021 by Amey
                                        var OGABS = Math.Abs(OGTo);
                                        double IVChange = (_PositionInfo.IVMiddle - _PositionInfo.IVLower) / (OGABS == 0 ? 1 : OGABS);

                                        if (IVIdx == 1)
                                            CurrIV = _PositionInfo.IVMiddle - (IVChange * Math.Abs(CurrOGIdx));
                                        else
                                            CurrIV = _PositionInfo.IVMiddle;
                                    }

                                    if (_UpdateValues)
                                    {
                                        //changed positon on 20APR2021 by Amey
                                        //Using this dictionary to display Total Unique Scrips at the top.
                                        CollectionHelper.dict_UniqueTokens.TryAdd(ScripKey, true);
                                        CollectionHelper.dict_UniqueClients.TryAdd(_ClientID, true);

                                        //Added by Akshay on 29-07-2021 for ClientWindow
                                        DateTime Expiry = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry);

                                        if (!CollectionHelper.dict_ComboUniverse.ContainsKey(_ClientID))
                                        {
                                            CollectionHelper.dict_ComboUniverse.TryAdd(_ClientID, new ConcurrentDictionary<string, ConcurrentDictionary<DateTime, string>>());
                                            CollectionHelper.dict_ComboUniverse[_ClientID].TryAdd(Underlying, new ConcurrentDictionary<DateTime, string>());
                                            CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].TryAdd(Expiry, "Done");
                                        }
                                        else if (!CollectionHelper.dict_ComboUniverse[_ClientID].ContainsKey(Underlying))
                                        {
                                            CollectionHelper.dict_ComboUniverse[_ClientID].TryAdd(Underlying, new ConcurrentDictionary<DateTime, string>());
                                            CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].TryAdd(Expiry, "Done");
                                        }
                                        else if (!CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].ContainsKey(Expiry))
                                        {
                                            CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].TryAdd(Expiry, "Done");
                                        }


                                        //added on 05APR2021 by Amey
                                        isCPUnderlyingExpanded = isCPParentExpanded && (list_ExpandedUnderlying.Contains(Underlying) || !dict_IsInitialCPPositionLoadSuccess[Underlying]);

                                        //Added by Akshay on 25-03-2021 For Delivery Margin
                                        try
                                        {
                                            if (_PositionInfo.InstrumentName == en_InstrumentName.OPTSTK || _PositionInfo.InstrumentName == en_InstrumentName.FUTSTK)
                                            {
                                                if (!dict_ExpiryDays.ContainsKey(_PositionInfo.Expiry))
                                                    dict_ExpiryDays.Add(_PositionInfo.Expiry, CountDaysToExpiry(CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry), DateTime.Now));

                                                var DaysToExpiry = dict_ExpiryDays[_PositionInfo.Expiry];
                                                if (DaysToExpiry <= ExpiryThreshHold)
                                                {
                                                    if (_PositionInfo.SpotPrice > 0 && _PositionInfo.InstrumentName == en_InstrumentName.OPTSTK)
                                                    {

                                                        if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.StrikePrice < _PositionInfo.SpotPrice && _PositionInfo.NetPosition > 0)
                                                            dict_UnderlyingLevel[Underlying].CallBuyQty += _PositionInfo.NetPosition;
                                                        else if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.StrikePrice < _PositionInfo.SpotPrice && _PositionInfo.NetPosition < 0)
                                                            dict_UnderlyingLevel[Underlying].CallSellQty += _PositionInfo.NetPosition;
                                                        else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.StrikePrice > _PositionInfo.SpotPrice && _PositionInfo.NetPosition > 0)
                                                            dict_UnderlyingLevel[Underlying].PutBuyQty += _PositionInfo.NetPosition * -1;
                                                        else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.StrikePrice > _PositionInfo.SpotPrice && _PositionInfo.NetPosition < 0)
                                                            dict_UnderlyingLevel[Underlying].PutSellQty += _PositionInfo.NetPosition * -1;
                                                    }
                                                    else
                                                        dict_UnderlyingLevel[Underlying].FutQty += _PositionInfo.NetPosition;

                                                    dict_UnderlyingLevel[Underlying].Obligation += _PositionInfo.NetPosition;

                                                }

                                                //changed to TryGetValue on 27MAY2021 by Amey
                                                //changed to SpotPrice from UnderlyingLTP on 08APR2021 by Amey
                                                if (_PositionInfo.SpotPrice > 0 && CollectionHelper.dict_DaysPercentage.TryGetValue(DaysToExpiry, out double _DaysPercentage))
                                                    dict_UnderlyingLevel[Underlying].ValMargin = _PositionInfo.SpotPrice * _PositionInfo.UnderlyingVARMargin * _DaysPercentage / 100;

                                                
                                            }

                                            // Added by Snehadri for banknifty and nifty Exposure 
                                            if (_PositionInfo.InstrumentName == en_InstrumentName.OPTIDX || _PositionInfo.InstrumentName == en_InstrumentName.FUTIDX)
                                            {
                                                if (!CheckTodayExpiry(_PositionInfo.Expiry))
                                                {
                                                    if (Underlying == "BANKNIFTY")
                                                    {
                                                        if (_PositionInfo.ScripType == en_ScripType.CE)
                                                            _CLevelBankNiftyCEQty += _PositionInfo.NetPosition;
                                                        else if (_PositionInfo.ScripType == en_ScripType.PE)
                                                            _CLevelBankNiftyPEQty += _PositionInfo.NetPosition;

                                                        _CLevelBankNiftyClosePrice = _PositionInfo.ClosingPrice;

                                                    }
                                                    else if (Underlying == "NIFTY")
                                                    {
                                                        if (_PositionInfo.ScripType == en_ScripType.CE)
                                                            _CLevelNiftyCEQty += _PositionInfo.NetPosition;
                                                        else if (_PositionInfo.ScripType == en_ScripType.PE)
                                                            _CLevelNiftyPEQty += _PositionInfo.NetPosition;

                                                        _CLevelNiftyClosePrice = _PositionInfo.ClosingPrice;
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ee) { _logger.Error(ee, _ClientID + "|DeliveryMargin & Expo Loop : " + Underlying); }

                                        //Added by Akshay on 29-06-2021 For POS EXPO
                                        try
                                        {
                                            //Added by Akshay on 29-06-2021 for POS EXPO
                                            if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.ExpiryTimeSpan.TotalDays > 1)
                                                dict_UnderlyingLevel[Underlying].NetCallQnty += _PositionInfo.NetPosition;

                                            //Added by Akshay on 29-06-2021 for POS EXPO
                                            else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.ExpiryTimeSpan.TotalDays > 1)
                                                dict_UnderlyingLevel[Underlying].NetPutQnty += _PositionInfo.NetPosition;

                                            //Added by Akshay on 29-06-2021 for POS EXPO
                                            else if (_PositionInfo.ScripType == en_ScripType.XX && _PositionInfo.ExpiryTimeSpan.TotalDays > 1)
                                                dict_UnderlyingLevel[Underlying].NetFutQnty += _PositionInfo.NetPosition;

                                            //Added by Akshay on 29-06-2021 for Net Qnty
                                            dict_UnderlyingLevel[Underlying].NetQnty += _PositionInfo.NetPosition;

                                            //Added by Akshay on 30-06-2021 for Closing Price
                                            dict_UnderlyingLevel[Underlying].ClosingPrice = _PositionInfo.ClosingPrice;

                                        }
                                        catch (Exception ee) { _logger.Error(ee, _ClientID + "|POS EXPO Loop : " + Underlying); }

                                        //To Add or Update ClientPortfolio Child Row. added on 24NOV2020 by Amey
                                        try
                                        {
                                            DateTime tExpiry = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry);
                                            double tBEP = _PositionInfo.BEP;
                                            double tPriceCF = _PositionInfo.PriceCF;

                                            double tFMTM = 0;
                                            double tOMTM = 0;
                                            double tEMTM = 0;

                                            double tIntradayFMTM = 0;
                                            double tIntradayOMTM = 0;
                                            double tIntradayEMTM = 0;

                                            //added on 24MAY2021 by Amey
                                            switch (_PositionInfo.ScripType)
                                            {
                                                case en_ScripType.EQ:
                                                    tEMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                    tIntradayEMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                    break;
                                                case en_ScripType.XX:
                                                    tFMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                    tIntradayFMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                    break;
                                                case en_ScripType.CE:
                                                case en_ScripType.PE:
                                                    tOMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                    tIntradayOMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                    break;
                                            }

                                            double tIntradayBEP = _PositionInfo.IntradayBEP;

                                            // Added by Snehadr on 29SEP2021
                                            double tTurnover = DivideByBaseAndRound((_PositionInfo.IntradayBuyQuantity * _PositionInfo.IntradayBuyAvg) + (_PositionInfo.IntradaySellQuantity * _PositionInfo.IntradaySellAvg), nameof(_CPParent.Turnover));
                                            _CLevelInfo.Turnover += tTurnover;
                                            dict_UnderlyingLevel[Underlying].Turnover += tTurnover;

                                            //Added by Akshay on 21-12-2020 
                                            double tSingleDelta = DivideByBaseAndRound(_PositionInfo.SingleDelta, nameof(_CPParent.AbsDelta));
                                            _CLevelInfo.SingleDelta += tSingleDelta;
                                            dict_UnderlyingLevel[Underlying].SingleDelta += tSingleDelta;

                                            //Added by Akshay on 21-12-2020 
                                            double tSingleGamma = DivideByBaseAndRound(_PositionInfo.SingleGamma, nameof(_CPParent.AbsGamma));
                                            _CLevelInfo.SingleGamma += tSingleGamma;
                                            dict_UnderlyingLevel[Underlying].SingleGamma += tSingleGamma;

                                            double tDelta = DivideByBaseAndRound(_PositionInfo.Delta * (CollectionHelper._ValueSigns.Delta), nameof(_CPParent.Delta));
                                            _CLevelInfo.Delta += tDelta;
                                            dict_UnderlyingLevel[Underlying].Delta += tDelta;

                                            double tGamma = DivideByBaseAndRound(_PositionInfo.Gamma * (CollectionHelper._ValueSigns.Gamma), nameof(_CPParent.Gamma));
                                            _CLevelInfo.Gamma += tGamma;
                                            dict_UnderlyingLevel[Underlying].Gamma += tGamma;

                                            double tTheta = DivideByBaseAndRound(_PositionInfo.Theta * (CollectionHelper._ValueSigns.Theta), nameof(_CPParent.Theta));
                                            _CLevelInfo.Theta += tTheta;
                                            dict_UnderlyingLevel[Underlying].Theta += tTheta;

                                            //added on 24MAY2021 by Amey
                                            double tDeltaAmt = DivideByBaseAndRound((_PositionInfo.Delta * _PositionInfo.UnderlyingLTP) * (CollectionHelper._ValueSigns.DeltaAmt), nameof(_CPParent.DeltaAmount));
                                            _CLevelInfo.DeltaAmount += tDeltaAmt;
                                            dict_UnderlyingLevel[Underlying].DeltaAmount += tDeltaAmt;

                                            double TimeValue = 0;
                                            double _ExpTheta = tTheta;
                                            double _intrinsicmtm = 0;
                                            //added on 29APR2021 by Amey
                                            if (_PositionInfo.ScripType == en_ScripType.CE || _PositionInfo.ScripType == en_ScripType.PE)
                                            {
                                                TimeValue = DivideByBaseAndRound(GetTimeValue(_PositionInfo.ScripType, _PositionInfo.UnderlyingLTP, _PositionInfo.StrikePrice,
                                                    _PositionInfo.LTP, _PositionInfo.NetPosition), nameof(_CPParent.Theta));

                                                _CLevelInfo.TimeValue += TimeValue;
                                                dict_UnderlyingLevel[Underlying].TimeValue += TimeValue;

                                                //added on 3JUN2021 by Amey
                                                if (_PositionInfo.ExpiryTimeSpan.TotalDays < 1)
                                                    _ExpTheta = Math.Min(Math.Abs(TimeValue), Math.Abs(tTheta)) * (tTheta < 0 ? -1 : 1);

                                                _CLevelInfo.ExpTheta += _ExpTheta;
                                                dict_UnderlyingLevel[Underlying].ExpTheta += _ExpTheta;

                                                // Added by Snehadri for Intrinsic MTM value                                                
                                                if (CheckTodayExpiry(_PositionInfo.Expiry))
                                                {
                                                    if(_PositionInfo.ScripType == en_ScripType.CE)
                                                        _intrinsicmtm = (_PositionInfo.SpotPrice - _PositionInfo.StrikePrice) * _PositionInfo.NetPosition; 
                                                    else if (_PositionInfo.ScripType == en_ScripType.PE)
                                                        _intrinsicmtm = (_PositionInfo.StrikePrice - _PositionInfo.SpotPrice) * _PositionInfo.NetPosition;
                                                }

                                                dict_UnderlyingLevel[Underlying].IntrinsicMTM += _intrinsicmtm;
                                                _CLevelInfo.IntrinsicMTM += _intrinsicmtm;
                                            }

                                            double tVega = DivideByBaseAndRound(_PositionInfo.Vega * (CollectionHelper._ValueSigns.Vega), nameof(_CPParent.Vega));
                                            _CLevelInfo.Vega += tVega;
                                            dict_UnderlyingLevel[Underlying].Vega += tVega;

                                            //changed on 11FEB2021 by Amey
                                            _CLevelInfo.FuturesMTM += tFMTM;
                                            _CLevelInfo.OptionsMTM += tOMTM;
                                            _CLevelInfo.EquityMTM += tEMTM;

                                            _CLevelInfo.IntradayFuturesMTM += tIntradayFMTM;
                                            _CLevelInfo.IntradayOptionsMTM += tIntradayOMTM;
                                            _CLevelInfo.IntradayEquityMTM += tIntradayEMTM;

                                            dict_UnderlyingLevel[Underlying].FuturesMTM += tFMTM;
                                            dict_UnderlyingLevel[Underlying].OptionsMTM += tOMTM;
                                            dict_UnderlyingLevel[Underlying].EquityMTM += tEMTM;

                                            dict_UnderlyingLevel[Underlying].IntradayFuturesMTM += tIntradayFMTM;
                                            dict_UnderlyingLevel[Underlying].IntradayOptionsMTM += tIntradayOMTM;
                                            dict_UnderlyingLevel[Underlying].IntradayEquityMTM += tIntradayEMTM;


                                            //Added by Akshay on 30-12-2021 for CDS
                                            double tCDSFMTM = 0;
                                            double tCDSOMTM = 0;

                                            double tCDSIntradayFMTM = 0;
                                            double tCDSIntradayOMTM = 0;

                                            //Added by Akshay on 28-12-2021 for CDS
                                            if (_PositionInfo.Segment == en_Segment.NSECD)
                                            {
                                                switch (_PositionInfo.ScripType)
                                                {
                                                    case en_ScripType.XX:
                                                        tCDSFMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                        tCDSIntradayFMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                        break;
                                                    case en_ScripType.CE:
                                                    case en_ScripType.PE:
                                                        tCDSOMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                        tCDSIntradayOMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                        break;
                                                }
                                            }

                                            //Added by Akshay on 28-12-2021 for CDS
                                            _CLevelInfo.CDSFuturesMTM += tCDSFMTM;
                                            _CLevelInfo.CDSOptionsMTM += tCDSOMTM;

                                            _CLevelInfo.CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                            _CLevelInfo.CDSIntradayOptionsMTM += tCDSIntradayOMTM;

                                            dict_UnderlyingLevel[Underlying].CDSFuturesMTM += tCDSFMTM;
                                            dict_UnderlyingLevel[Underlying].CDSOptionsMTM += tCDSOMTM;

                                            dict_UnderlyingLevel[Underlying].CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                            dict_UnderlyingLevel[Underlying].CDSIntradayOptionsMTM += tCDSIntradayOMTM;

                                            //Added by Akshay For Collateral
                                            long tCollateralQty = _PositionInfo.CollateralQty;
                                            double tCollateralValue = _PositionInfo.CollateralValue;
                                            double tCollateralHaircut = _PositionInfo.CollateralHaircut;

                                            _CLevelInfo.CollateralQty += tCollateralQty;
                                            _CLevelInfo.CollateralValue += tCollateralValue;
                                            _CLevelInfo.CollateralHaircut += tCollateralHaircut;

                                            dict_UnderlyingLevel[Underlying].CollateralQty += tCollateralQty;
                                            dict_UnderlyingLevel[Underlying].CollateralValue += tCollateralValue;
                                            dict_UnderlyingLevel[Underlying].CollateralHaircut += tCollateralHaircut;

                                            //added on 15APR2021 by Amey
                                            double tTMTM = DivideByBaseAndRound(_PositionInfo.TheoreticalMTM, nameof(_CPParent.MTM));
                                            _CLevelInfo.TheoreticalMTM += tTMTM;
                                            dict_UnderlyingLevel[Underlying].TheoreticalMTM += tTMTM;

                                            double _ROV = 0;

                                            //changed position on 20MAY2021 by Amey
                                            //added on 11FEB2021 by Amey
                                            //Added by Akshay on 09-12-2020 for seperate EquityAmt
                                            if (_PositionInfo.ScripType == en_ScripType.EQ)
                                            {
                                                _CLevelInfo.EquityAmount += DivideByBaseAndRound(_PositionInfo.EquityAmount, nameof(_CPParent.EquityAmount));

                                                //changed logic on 15APR2021 by Amey
                                                _PayInPayOut += DivideByBaseAndRound(_PositionInfo.DayNetPremium, nameof(_CPParent.PayInPayOut));

                                                _CLevelInfo.VARMargin += DivideByBaseAndRound(_PositionInfo.VARMargin, nameof(_CPParent.VARMargin));

                                                _CLevelInfo.T1Quantity += _PositionInfo.T1Quantity;
                                                _CLevelInfo.T2Quantity += _PositionInfo.T2Quantity;
                                                _CLevelInfo.EarlyPayIn += _PositionInfo.EarlyPayIn;


                                            }
                                            else if (_PositionInfo.ScripType == en_ScripType.CE || _PositionInfo.ScripType == en_ScripType.PE)
                                            {
                                                //changed logic on 15APR2021 by Amey
                                                //Added by Akshay on 12-01-2021 for DayNet premium

                                                _DayNetPremium += DivideByBaseAndRound(_PositionInfo.DayNetPremium, nameof(_CPParent.DayNetPremium));
                                                _DayNetPremiumCDS += DivideByBaseAndRound(_PositionInfo.DayNetPremiumCDS, nameof(_CPParent.DayNetPremium));

                                             
                                                  
                                                _ROV = DivideByBaseAndRound(_PositionInfo.NetPosition * _PositionInfo.LTP, nameof(_CPParent.ROV));
                                            }

                                            //added on 20MAY2021 by Amey
                                            _CLevelInfo.ROV += _ROV;
                                            dict_UnderlyingLevel[Underlying].ROV += _ROV;

                                            if (isCPUnderlyingExpanded)
                                            {
                                                double tDaysToExpiry = _PositionInfo.ScripType == en_ScripType.EQ ? 0 : Math.Floor((_PositionInfo.ExpiryTimeSpan.TotalDays * 0.1) * 100) / 10;

                                                var _CPPositions = new CPPositions
                                                {
                                                    //Added by Akshay o 31-12-2020 for VAREQ
                                                    VARMargin = DivideByBaseAndRound(_PositionInfo.VARMargin, nameof(_CPParent.VARMargin)),

                                                    T1Quantity = _PositionInfo.T1Quantity,
                                                    T2Quantity = _PositionInfo.T2Quantity,
                                                    EarlyPayIn = _PositionInfo.EarlyPayIn,

                                                    ClientID = _ClientID,
                                                    Underlying = Underlying,
                                                    ScripName = _PositionInfo.ScripName,

                                                    //added on 20APR2021 by Amey
                                                    Segment = _PositionInfo.Segment,
                                                    Series = _PositionInfo.Series,
                                                    InstrumentName = _PositionInfo.InstrumentName,

                                                    ScripToken = _PositionInfo.Token,
                                                    ExpiryDate = tExpiry,
                                                    ScripType = _PositionInfo.ScripType,
                                                    StrikePrice = _PositionInfo.StrikePrice,

                                                    NetPosition = _PositionInfo.NetPosition,
                                                    BEP = tBEP,
                                                    NetPositionCF = _PositionInfo.NetPositionCF,
                                                    PriceCF = tPriceCF,

                                                    //changed on 01FEB2021 by Amey
                                                    LTP = _PositionInfo.LTP,
                                                    UnderlyingLTP = _PositionInfo.UnderlyingLTP,

                                                    //added on 08APR2021 by Amey
                                                    TheoreticalPrice = _PositionInfo.TheoreticalPrice,
                                                    TheoreticalMTM = tTMTM,

                                                    IntrinsicMTM = _intrinsicmtm,

                                                    //added on 20MAY2021 by Amey
                                                    ROV = _ROV,

                                                    SpotPrice = _PositionInfo.SpotPrice,

                                                    AtmIV = _PositionInfo.ATM_IV,

                                                    //changed on 20MAY2021 by Amey
                                                    FuturesMTM = tFMTM + tCDSFMTM ,
                                                    OptionsMTM = tOMTM + tCDSOMTM ,
                                                    EquityMTM = tEMTM,
                                                    MTM = tFMTM + tOMTM + tEMTM + tCDSFMTM + tCDSOMTM ,

                                                    //changed on 20MAY2021 by Amey
                                                    /*IntradayFuturesMTM = tIntradayFMTM,
                                                    IntradayOptionsMTM = tIntradayOMTM,*/

                                                    //Changed on 31AUG2023 by Sujit for MCX
                                                    IntradayFuturesMTM = tIntradayFMTM + tCDSIntradayFMTM,
                                                    IntradayOptionsMTM = tIntradayOMTM + tCDSIntradayOMTM ,
                                                    IntradayEquityMTM = tIntradayEMTM,
                                                    //IntradayMTM = tIntradayFMTM + tIntradayOMTM + tIntradayEMTM,
                                                    IntradayMTM = tIntradayFMTM + tIntradayOMTM + tIntradayEMTM + tCDSIntradayFMTM + tCDSIntradayOMTM ,



                                                    IntradayBEP = tIntradayBEP,
                                                    IntradayNetPosition = _PositionInfo.IntradayNetPosition,

                                                    AbsDelta = tSingleDelta,    //Added by Akshay on 21-12-2020
                                                    AbsGamma = tSingleGamma,    //Added by Akshay on 21-12-2020

                                                    Delta = tDelta,
                                                    Theta = tTheta,
                                                    Gamma = tGamma,
                                                    Vega = tVega,

                                                    //added on 29APR2021 by Amey
                                                    TV = TimeValue,

                                                    //added on 3JUN2021 by Amey
                                                    ExpTheta = _ExpTheta,

                                                    //added on 24MAY2021 by Amey
                                                    DeltaAmount = tDeltaAmt,

                                                    DaysToExpiry = tDaysToExpiry,
                                                    IsLTPCalculated = _PositionInfo.IsLTPCalculated,

                                                    //added on 07APR2021 by Amey
                                                    IntradayBuyQuantity = _PositionInfo.IntradayBuyQuantity,
                                                    IntradayBuyAvg = _PositionInfo.IntradayBuyAvg,
                                                    IntradaySellQuantity = _PositionInfo.IntradaySellQuantity,
                                                    IntradaySellAvg = _PositionInfo.IntradaySellAvg,
                                                    Turnover = tTurnover,
                                                    
                                                };

                                                //changed on 20APR2021 by Amey
                                                dict_Positions.Add(ScripKey, _CPPositions);
                                            }
                                        }
                                        catch (Exception ee) { _logger.Error(ee, _ClientID + "|ComputeAndUpdate Loop : " + Underlying); }
                                    }

                                    double CurrPosVaR = 0;
                                    double CalculatedLTP = 0;

                                    if (_PositionInfo.NetPosition == 0)
                                        CurrPosVaR = 0;
                                    else
                                    {
                                        if (_PositionInfo.ScripType == en_ScripType.EQ || _PositionInfo.ScripType == en_ScripType.XX)
                                            CalculatedLTP = (_PositionInfo.LTP * (100 + CurrOGIdx)) / 100;
                                        else if (_PositionInfo.ExpiryTimeSpan.TotalDays > 0 && (_PositionInfo.ScripType == en_ScripType.CE || _PositionInfo.ScripType == en_ScripType.PE))
                                        {
                                            if (_PositionInfo.ScripType == en_ScripType.CE && CurrIV > 0)
                                                CalculatedLTP = CommonFunctions.CallOption((_PositionInfo.UnderlyingLTP * (100 + CurrOGIdx)) / 100, _PositionInfo.StrikePrice, _PositionInfo.ExpiryTimeSpan.TotalDays / 365,
                                                    0, (CurrIV / 100), 0);
                                            else if (_PositionInfo.ScripType == en_ScripType.PE && CurrIV > 0)
                                                CalculatedLTP = CommonFunctions.PutOption((_PositionInfo.UnderlyingLTP * (100 + CurrOGIdx)) / 100, _PositionInfo.StrikePrice, _PositionInfo.ExpiryTimeSpan.TotalDays / 365,
                                                    0, (CurrIV / 100), 0);
                                        }

                                        if (_PositionInfo.NetPosition > 0)
                                            CurrPosVaR = (CalculatedLTP - _PositionInfo.BEP) * Math.Abs(_PositionInfo.NetPosition);
                                        else if (_PositionInfo.NetPosition < 0)
                                            CurrPosVaR = (_PositionInfo.BEP - CalculatedLTP) * Math.Abs(_PositionInfo.NetPosition);
                                    }

                                    arr_VaRAtIV[IVIdx] += CurrPosVaR;

                                    //changed on 07DEC2020 by Amey
                                    if (dList_MaxLossUnderlyingWise.ContainsKey(IVIdx))
                                        dList_MaxLossUnderlyingWise[IVIdx].Add(new VaRInfo() { Segment = _PositionInfo.Segment, Token = _PositionInfo.Token, VaR = CurrPosVaR, IV = CurrIV, OGIdx = CurrOGIdx, PriceAtOG = CalculatedLTP });
                                    else
                                        dList_MaxLossUnderlyingWise.Add(IVIdx, new List<VaRInfo>() { new VaRInfo() { Segment = _PositionInfo.Segment, Token = _PositionInfo.Token, VaR = CurrPosVaR, IV = CurrIV, OGIdx = CurrOGIdx, PriceAtOG = CalculatedLTP } });

                                }
                            }

                            int MaxVARIdx = Array.IndexOf(arr_VaRAtIV, arr_VaRAtIV.Min());

                            //changed to TryGetValue on 27MAY2021 by Amey
                            if (dList_MaxLossUnderlyingWise.TryGetValue(MaxVARIdx, out List<VaRInfo> list_VaRInf))
                            {
                                if (dict_MaxLossUnderlyingWise.ContainsKey(Underlying))
                                {
                                    //changed on 07DEC2020 by Amey
                                    dict_MaxLossUnderlyingWise[Underlying].Add(new MaxVaRStruct() { MaxVaR = arr_VaRAtIV[MaxVARIdx], list_VaRInfo = list_VaRInf });
                                }
                                else
                                {
                                    //changed on 07DEC2020 by Amey
                                    dict_MaxLossUnderlyingWise.Add(Underlying, new List<MaxVaRStruct>() { new MaxVaRStruct() { MaxVaR = arr_VaRAtIV[MaxVARIdx], list_VaRInfo = list_VaRInf } });
                                }
                            }
                            else if (CollectionHelper.IsDebug)
                                _logger.Debug($"NoMaxLossFound|{_ClientID}|{Underlying}|{MaxVARIdx}|{dict_MaxLossUnderlyingWise.Count}");

                            if (isVaRDistributionShown)
                            {
                                try
                                {
                                    var dict_VaRDistribution = CollectionHelper.dict_VaRDistribution;
                                    if (dict_VaRDistribution.ContainsKey(_ClientID))
                                    {
                                        if (!dict_VaRDistribution[_ClientID].ContainsKey(Underlying))
                                            dict_VaRDistribution[_ClientID].TryAdd(Underlying, new ConcurrentDictionary<int, double>() { [CurrOGIdx] = arr_VaRAtIV[MaxVARIdx] });
                                        else if (dict_VaRDistribution[_ClientID][Underlying].ContainsKey(CurrOGIdx))
                                            dict_VaRDistribution[_ClientID][Underlying][CurrOGIdx] = arr_VaRAtIV[MaxVARIdx];
                                        else
                                            dict_VaRDistribution[_ClientID][Underlying].TryAdd(CurrOGIdx, arr_VaRAtIV[MaxVARIdx]);
                                    }
                                    else
                                        dict_VaRDistribution.TryAdd(_ClientID, new ConcurrentDictionary<string, ConcurrentDictionary<int, double>>() { [Underlying] = new ConcurrentDictionary<int, double>() { [CurrOGIdx] = arr_VaRAtIV[MaxVARIdx] } });
                                }
                                catch (Exception ee) { _logger.Error(ee, $"VarDistribution : {_ClientID}"); }
                            }
                        }
                    }

                    //Added by Akshay on 23-08-2021 for UpsideDown VaR
                    else if (!CollectionHelper.IsFullVAR)
                    {
                        var list_UpDownVarRange = CollectionHelper.list_UpDownVarRange.Where(c => c.Underlying == Underlying).FirstOrDefault();
                        if (list_UpDownVarRange is null)
                            list_UpDownVarRange = CollectionHelper.list_UpDownVarRange.Where(c => c.Underlying == "ALL").FirstOrDefault();

                        var ss_VarRange = list_UpDownVarRange.SS_UpDownVarRange;
                        var OGFrom = ss_VarRange.Min();
                        var OGTo = ss_VarRange.Max();

                        foreach (var CurrOGIdx in ss_VarRange)
                        {
                            if (CurrOGIdx == OGFrom)
                                _UpdateValues = true;
                            else
                                _UpdateValues = false;

                            foreach (var _PositionInfo in dict_RecentPositions[Underlying])
                            {
                                //Iterating on every position under Underlying specified above

                                //added on 20APR2021 by Amey
                                var ScripKey = $"{_PositionInfo.Segment}|{_PositionInfo.Token}";
                               
                                ///changed position on 07APR2021 by Amey
                                if (_PositionInfo.LTP <= 0 || _PositionInfo.UnderlyingLTP <= 0)
                                {
                                    //If LTP is <= 0, means the LTP is not available yet. Therefore skip the Position.

                                    if (CollectionHelper.IsDebug)
                                        _logger.Debug($"NOLTP|{_ClientID}|{_PositionInfo.ScripName}|{_PositionInfo.Token}|{_PositionInfo.LTP}|{_PositionInfo.UnderlyingLTP}");

                                    continue;
                                }

                                if (_PositionInfo.ScripType != en_ScripType.EQ && _PositionInfo.ExpiryTimeSpan.TotalDays <= 0)
                                {
                                    if (_UpdateValues)
                                    {
                                        //If Expired, dont compute other values excluding what mentioned below.

                                        //changed on 20APR2021 by Amey
                                        if (!dict_TokensRemove.ContainsKey(ScripKey))
                                            dict_TokensRemove.Add(ScripKey, Underlying);

                                        double tFMTM = 0;
                                        double tOMTM = 0;
                                        double tEMTM = 0;

                                        double tIntradayFMTM = 0;
                                        double tIntradayOMTM = 0;
                                        double tIntradayEMTM = 0;

                                        //added on 24MAY2021 by Amey
                                        switch (_PositionInfo.ScripType)
                                        {
                                            case en_ScripType.EQ:
                                                tEMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                tIntradayEMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                break;
                                            case en_ScripType.XX:
                                                tFMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                tIntradayFMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                break;
                                            case en_ScripType.CE:
                                            case en_ScripType.PE:
                                                tOMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                tIntradayOMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                break;
                                        }

                                        //changed on 11FEB2021 by Amey
                                        _CLevelInfo.FuturesMTM += tFMTM;
                                        _CLevelInfo.OptionsMTM += tOMTM;
                                        _CLevelInfo.EquityMTM += tEMTM;

                                        _CLevelInfo.IntradayFuturesMTM += tIntradayFMTM;
                                        _CLevelInfo.IntradayOptionsMTM += tIntradayOMTM;
                                        _CLevelInfo.IntradayEquityMTM += tIntradayEMTM;

                                        dict_UnderlyingLevel[Underlying].FuturesMTM += tFMTM;
                                        dict_UnderlyingLevel[Underlying].OptionsMTM += tOMTM;
                                        dict_UnderlyingLevel[Underlying].EquityMTM += tEMTM;

                                        dict_UnderlyingLevel[Underlying].IntradayFuturesMTM += tIntradayFMTM;
                                        dict_UnderlyingLevel[Underlying].IntradayOptionsMTM += tIntradayOMTM;
                                        dict_UnderlyingLevel[Underlying].IntradayEquityMTM += tIntradayEMTM;

                                        //Added by Akshay on 30-12-2021 for CDS
                                        double tCDSFMTM = 0;
                                        double tCDSOMTM = 0;

                                        double tCDSIntradayFMTM = 0;
                                        double tCDSIntradayOMTM = 0;

                                        //Added by Akshay on 28-12-2021 for CDS
                                        if (_PositionInfo.Segment == en_Segment.NSECD)
                                        {
                                            switch (_PositionInfo.ScripType)
                                            {
                                                case en_ScripType.XX:
                                                    tCDSFMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                    tCDSIntradayFMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                    break;
                                                case en_ScripType.CE:
                                                case en_ScripType.PE:
                                                    tCDSOMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                    tCDSIntradayOMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                    break;
                                            }
                                        }

                                        //Added by Akshay on 28-12-2021 for CDS
                                        _CLevelInfo.CDSFuturesMTM += tCDSFMTM;
                                        _CLevelInfo.CDSOptionsMTM += tCDSOMTM;

                                        _CLevelInfo.CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                        _CLevelInfo.CDSIntradayOptionsMTM += tCDSIntradayOMTM;

                                        dict_UnderlyingLevel[Underlying].CDSFuturesMTM += tCDSFMTM;
                                        dict_UnderlyingLevel[Underlying].CDSOptionsMTM += tCDSOMTM;

                                        dict_UnderlyingLevel[Underlying].CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                        dict_UnderlyingLevel[Underlying].CDSIntradayOptionsMTM += tCDSIntradayOMTM;

                                        //added on 15APR2021 by Amey
                                        double tTMTM = DivideByBaseAndRound(_PositionInfo.TheoreticalMTM, nameof(_CPParent.MTM));
                                        _CLevelInfo.TheoreticalMTM += tTMTM;
                                        dict_UnderlyingLevel[Underlying].TheoreticalMTM += tTMTM;
                                    }

                                    if (CollectionHelper.IsDebug)
                                        _logger.Debug($"Expired|{_ClientID}|{_PositionInfo.ScripName}|{_PositionInfo.ExpiryTimeSpan.TotalDays}");

                                    continue;
                                }

                                CurrIV = _PositionInfo.IVMiddle;

                                if (_UpdateValues)
                                {
                                    //changed positon on 20APR2021 by Amey
                                    //Using this dictionary to display Total Unique Scrips at the top.
                                    CollectionHelper.dict_UniqueTokens.TryAdd(ScripKey, true);
                                    CollectionHelper.dict_UniqueClients.TryAdd(_ClientID, true);

                                    //Added by Akshay on 29-07-2021 for ClientWindow
                                    DateTime Expiry = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry);

                                    if (!CollectionHelper.dict_ComboUniverse.ContainsKey(_ClientID))
                                    {
                                        CollectionHelper.dict_ComboUniverse.TryAdd(_ClientID, new ConcurrentDictionary<string, ConcurrentDictionary<DateTime, string>>());
                                        CollectionHelper.dict_ComboUniverse[_ClientID].TryAdd(Underlying, new ConcurrentDictionary<DateTime, string>());
                                        CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].TryAdd(Expiry, "Done");
                                    }
                                    else if (!CollectionHelper.dict_ComboUniverse[_ClientID].ContainsKey(Underlying))
                                    {
                                        CollectionHelper.dict_ComboUniverse[_ClientID].TryAdd(Underlying, new ConcurrentDictionary<DateTime, string>());
                                        CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].TryAdd(Expiry, "Done");
                                    }
                                    else if (!CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].ContainsKey(Expiry))
                                    {
                                        CollectionHelper.dict_ComboUniverse[_ClientID][Underlying].TryAdd(Expiry, "Done");
                                    }

                                    //added on 05APR2021 by Amey
                                    isCPUnderlyingExpanded = isCPParentExpanded && (list_ExpandedUnderlying.Contains(Underlying) || !dict_IsInitialCPPositionLoadSuccess[Underlying]);

                                    //Added by Akshay on 25-03-2021 For Delivery Margin
                                    try
                                    {
                                        if (_PositionInfo.InstrumentName == en_InstrumentName.OPTSTK || _PositionInfo.InstrumentName == en_InstrumentName.FUTSTK)
                                        {
                                            if (!dict_ExpiryDays.ContainsKey(_PositionInfo.Expiry))
                                                dict_ExpiryDays.Add(_PositionInfo.Expiry, CountDaysToExpiry(CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry), DateTime.Now));

                                            var DaysToExpiry = dict_ExpiryDays[_PositionInfo.Expiry];
                                            if (DaysToExpiry <= ExpiryThreshHold)
                                            {

                                                if (_PositionInfo.SpotPrice > 0 && _PositionInfo.InstrumentName == en_InstrumentName.OPTSTK)
                                                {

                                                    if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.StrikePrice < _PositionInfo.SpotPrice && _PositionInfo.NetPosition > 0)
                                                        dict_UnderlyingLevel[Underlying].CallBuyQty += _PositionInfo.NetPosition;
                                                    else if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.StrikePrice < _PositionInfo.SpotPrice && _PositionInfo.NetPosition < 0)
                                                        dict_UnderlyingLevel[Underlying].CallSellQty += _PositionInfo.NetPosition;
                                                    else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.StrikePrice > _PositionInfo.SpotPrice && _PositionInfo.NetPosition > 0)
                                                        dict_UnderlyingLevel[Underlying].PutBuyQty += _PositionInfo.NetPosition * -1;
                                                    else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.StrikePrice > _PositionInfo.SpotPrice && _PositionInfo.NetPosition < 0)
                                                        dict_UnderlyingLevel[Underlying].PutSellQty += _PositionInfo.NetPosition * -1;
                                                }
                                                else
                                                    dict_UnderlyingLevel[Underlying].FutQty = _PositionInfo.NetPosition;

                                                dict_UnderlyingLevel[Underlying].Obligation += _PositionInfo.NetPosition;
                                            }

                                            //changed to TryGetValue on 27MAY2021 by Amey
                                            //changed to SpotPrice from UnderlyingLTP on 08APR2021 by Amey
                                            if (_PositionInfo.SpotPrice > 0 && CollectionHelper.dict_DaysPercentage.TryGetValue(DaysToExpiry, out double _DaysPercentage))
                                                dict_UnderlyingLevel[Underlying].ValMargin = _PositionInfo.SpotPrice * _PositionInfo.UnderlyingVARMargin * _DaysPercentage / 100;
                                        }

                                        // Added by Snehadri for banknifty and nifty Exposure 
                                        if (_PositionInfo.InstrumentName == en_InstrumentName.OPTIDX || _PositionInfo.InstrumentName == en_InstrumentName.FUTIDX)
                                        {
                                            if (!CheckTodayExpiry(_PositionInfo.Expiry))
                                            {
                                                if (Underlying == "BANKNIFTY")
                                                {
                                                    if (_PositionInfo.ScripType == en_ScripType.CE)
                                                        _CLevelBankNiftyCEQty += _PositionInfo.NetPosition;
                                                    else if (_PositionInfo.ScripType == en_ScripType.PE)
                                                        _CLevelBankNiftyPEQty += _PositionInfo.NetPosition;

                                                    _CLevelBankNiftyClosePrice = _PositionInfo.ClosingPrice;

                                                }
                                                else if (Underlying == "NIFTY")
                                                {
                                                    if (_PositionInfo.ScripType == en_ScripType.CE)
                                                        _CLevelNiftyCEQty += _PositionInfo.NetPosition;
                                                    else if (_PositionInfo.ScripType == en_ScripType.PE)
                                                        _CLevelNiftyPEQty += _PositionInfo.NetPosition;

                                                    _CLevelNiftyClosePrice = _PositionInfo.ClosingPrice;
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ee) { _logger.Error(ee, _ClientID + "|DeliveryMargin & Expo Loop : " + Underlying); }

                                    //Added by Akshay on 29-06-2021 For POS EXPO
                                    try
                                    {
                                        //Added by Akshay on 29-06-2021 for POS EXPO
                                        if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.ExpiryTimeSpan.TotalDays > 1)
                                            dict_UnderlyingLevel[Underlying].NetCallQnty += _PositionInfo.NetPosition;

                                        //Added by Akshay on 29-06-2021 for POS EXPO
                                        else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.ExpiryTimeSpan.TotalDays > 1)
                                            dict_UnderlyingLevel[Underlying].NetPutQnty += _PositionInfo.NetPosition;

                                        //Added by Akshay on 29-06-2021 for POS EXPO
                                        else if (_PositionInfo.ScripType == en_ScripType.XX && _PositionInfo.ExpiryTimeSpan.TotalDays > 1)
                                            dict_UnderlyingLevel[Underlying].NetFutQnty += _PositionInfo.NetPosition;

                                        //Added by Akshay on 29-06-2021 for Net Qnty
                                        dict_UnderlyingLevel[Underlying].NetQnty += _PositionInfo.NetPosition;

                                        //Added by Akshay on 30-06-2021 for Closing Price
                                        dict_UnderlyingLevel[Underlying].ClosingPrice = _PositionInfo.ClosingPrice;

                                    }
                                    catch (Exception ee) { _logger.Error(ee, _ClientID + "|POS EXPO Loop : " + Underlying); }

                                    //To Add or Update ClientPortfolio Child Row. added on 24NOV2020 by Amey
                                    try
                                    {
                                        DateTime tExpiry = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry);
                                        double tBEP = _PositionInfo.BEP;
                                        double tPriceCF = _PositionInfo.PriceCF;

                                        double tFMTM = 0;
                                        double tOMTM = 0;
                                        double tEMTM = 0;

                                        double tIntradayFMTM = 0;
                                        double tIntradayOMTM = 0;
                                        double tIntradayEMTM = 0;



                                        //added on 24MAY2021 by Amey
                                        switch (_PositionInfo.ScripType)
                                        {
                                            case en_ScripType.EQ:
                                                tEMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                tIntradayEMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                break;
                                            case en_ScripType.XX:
                                                tFMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                tIntradayFMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                break;
                                            case en_ScripType.CE:
                                            case en_ScripType.PE:
                                                tOMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                tIntradayOMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                break;
                                        }

                                        double tIntradayBEP = _PositionInfo.IntradayBEP;

                                        // Added by Snehadr on 29SEP2021
                                        double tTurnover = DivideByBaseAndRound((_PositionInfo.IntradayBuyQuantity * _PositionInfo.IntradayBuyAvg) + (_PositionInfo.IntradaySellQuantity * _PositionInfo.IntradaySellAvg), nameof(_CPParent.Turnover));
                                        _CLevelInfo.Turnover += tTurnover;
                                        dict_UnderlyingLevel[Underlying].Turnover += tTurnover;

                                        //Added by Akshay on 21-12-2020 
                                        double tSingleDelta = DivideByBaseAndRound(_PositionInfo.SingleDelta, nameof(_CPParent.AbsDelta));
                                        _CLevelInfo.SingleDelta += tSingleDelta;
                                        dict_UnderlyingLevel[Underlying].SingleDelta += tSingleDelta;

                                        //Added by Akshay on 21-12-2020 
                                        double tSingleGamma = DivideByBaseAndRound(_PositionInfo.SingleGamma, nameof(_CPParent.AbsGamma));
                                        _CLevelInfo.SingleGamma += tSingleGamma;
                                        dict_UnderlyingLevel[Underlying].SingleGamma += tSingleGamma;

                                        double tDelta = DivideByBaseAndRound(_PositionInfo.Delta * (CollectionHelper._ValueSigns.Delta), nameof(_CPParent.Delta));
                                        _CLevelInfo.Delta += tDelta;
                                        dict_UnderlyingLevel[Underlying].Delta += tDelta;

                                        double tGamma = DivideByBaseAndRound(_PositionInfo.Gamma * (CollectionHelper._ValueSigns.Gamma), nameof(_CPParent.Gamma));
                                        _CLevelInfo.Gamma += tGamma;
                                        dict_UnderlyingLevel[Underlying].Gamma += tGamma;

                                        double tTheta = DivideByBaseAndRound(_PositionInfo.Theta * (CollectionHelper._ValueSigns.Theta), nameof(_CPParent.Theta));
                                        _CLevelInfo.Theta += tTheta;
                                        dict_UnderlyingLevel[Underlying].Theta += tTheta;

                                        //added on 24MAY2021 by Amey
                                        double tDeltaAmt = DivideByBaseAndRound((_PositionInfo.Delta * _PositionInfo.UnderlyingLTP) * (CollectionHelper._ValueSigns.DeltaAmt), nameof(_CPParent.DeltaAmount));
                                        _CLevelInfo.DeltaAmount += tDeltaAmt;
                                        dict_UnderlyingLevel[Underlying].DeltaAmount += tDeltaAmt;

                                        double TimeValue = 0;
                                        double _ExpTheta = tTheta;
                                        double _intrinsicmtm = 0;
                                        //added on 29APR2021 by Amey
                                        if (_PositionInfo.ScripType == en_ScripType.CE || _PositionInfo.ScripType == en_ScripType.PE)
                                        {
                                            TimeValue = DivideByBaseAndRound(GetTimeValue(_PositionInfo.ScripType, _PositionInfo.UnderlyingLTP, _PositionInfo.StrikePrice,
                                                _PositionInfo.LTP, _PositionInfo.NetPosition), nameof(_CPParent.Theta));

                                            _CLevelInfo.TimeValue += TimeValue;
                                            dict_UnderlyingLevel[Underlying].TimeValue += TimeValue;

                                            //added on 3JUN2021 by Amey
                                            if (_PositionInfo.ExpiryTimeSpan.TotalDays < 1)
                                                _ExpTheta = Math.Min(Math.Abs(TimeValue), Math.Abs(tTheta)) * (tTheta < 0 ? -1 : 1);

                                            _CLevelInfo.ExpTheta += _ExpTheta;
                                            dict_UnderlyingLevel[Underlying].ExpTheta += _ExpTheta;

                                            // Added by Snehadri for Intrinsic MTM value                                            
                                            if (CheckTodayExpiry(_PositionInfo.Expiry))
                                            {
                                                if (_PositionInfo.ScripType == en_ScripType.CE)
                                                    _intrinsicmtm = (_PositionInfo.SpotPrice - _PositionInfo.StrikePrice) * _PositionInfo.NetPosition;
                                                else if (_PositionInfo.ScripType == en_ScripType.PE)
                                                    _intrinsicmtm = (_PositionInfo.StrikePrice - _PositionInfo.SpotPrice) * _PositionInfo.NetPosition;
                                            }

                                            dict_UnderlyingLevel[Underlying].IntrinsicMTM += _intrinsicmtm;
                                            _CLevelInfo.IntrinsicMTM += _intrinsicmtm;
                                        }

                                        double tVega = DivideByBaseAndRound(_PositionInfo.Vega * (CollectionHelper._ValueSigns.Vega), nameof(_CPParent.Vega));
                                        _CLevelInfo.Vega += tVega;
                                        dict_UnderlyingLevel[Underlying].Vega += tVega;

                                        //changed on 11FEB2021 by Amey
                                        _CLevelInfo.FuturesMTM += tFMTM;
                                        _CLevelInfo.OptionsMTM += tOMTM;
                                        _CLevelInfo.EquityMTM += tEMTM;

                                        _CLevelInfo.IntradayFuturesMTM += tIntradayFMTM;
                                        _CLevelInfo.IntradayOptionsMTM += tIntradayOMTM;
                                        _CLevelInfo.IntradayEquityMTM += tIntradayEMTM;

                                        dict_UnderlyingLevel[Underlying].FuturesMTM += tFMTM;
                                        dict_UnderlyingLevel[Underlying].OptionsMTM += tOMTM;
                                        dict_UnderlyingLevel[Underlying].EquityMTM += tEMTM;

                                        dict_UnderlyingLevel[Underlying].IntradayFuturesMTM += tIntradayFMTM;
                                        dict_UnderlyingLevel[Underlying].IntradayOptionsMTM += tIntradayOMTM;
                                        dict_UnderlyingLevel[Underlying].IntradayEquityMTM += tIntradayEMTM;


                                        //Added by Akshay on 30-12-2021 for CDS
                                        double tCDSFMTM = 0;
                                        double tCDSOMTM = 0;

                                        double tCDSIntradayFMTM = 0;
                                        double tCDSIntradayOMTM = 0;

                                        //Added by Akshay on 28-12-2021 for CDS
                                        if (_PositionInfo.Segment == en_Segment.NSECD)
                                        {
                                            switch (_PositionInfo.ScripType)
                                            {
                                                case en_ScripType.XX:
                                                    tCDSFMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                    tCDSIntradayFMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                    break;
                                                case en_ScripType.CE:
                                                case en_ScripType.PE:
                                                    tCDSOMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                    tCDSIntradayOMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                    break;
                                            }
                                        }

                                        //Added by Akshay on 28-12-2021 for CDS
                                        _CLevelInfo.CDSFuturesMTM += tCDSFMTM;
                                        _CLevelInfo.CDSOptionsMTM += tCDSOMTM;

                                        _CLevelInfo.CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                        _CLevelInfo.CDSIntradayOptionsMTM += tCDSIntradayOMTM;

                                        dict_UnderlyingLevel[Underlying].CDSFuturesMTM += tCDSFMTM;
                                        dict_UnderlyingLevel[Underlying].CDSOptionsMTM += tCDSOMTM;

                                        dict_UnderlyingLevel[Underlying].CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                        dict_UnderlyingLevel[Underlying].CDSIntradayOptionsMTM += tCDSIntradayOMTM;

                                        long tCollateralQty = _PositionInfo.CollateralQty;
                                        double tCollateralValue = _PositionInfo.CollateralValue;
                                        double tCollateralHaircut = _PositionInfo.CollateralHaircut;

                                        _CLevelInfo.CollateralQty += tCollateralQty;
                                        _CLevelInfo.CollateralValue += tCollateralValue;
                                        _CLevelInfo.CollateralHaircut += tCollateralHaircut;

                                        dict_UnderlyingLevel[Underlying].CollateralQty += tCollateralQty;
                                        dict_UnderlyingLevel[Underlying].CollateralValue += tCollateralValue;
                                        dict_UnderlyingLevel[Underlying].CollateralHaircut += tCollateralHaircut;


                                        //added on 15APR2021 by Amey
                                        double tTMTM = DivideByBaseAndRound(_PositionInfo.TheoreticalMTM, nameof(_CPParent.MTM));
                                        _CLevelInfo.TheoreticalMTM += tTMTM;
                                        dict_UnderlyingLevel[Underlying].TheoreticalMTM += tTMTM;

                                        double _ROV = 0;

                                        //changed position on 20MAY2021 by Amey
                                        //added on 11FEB2021 by Amey
                                        //Added by Akshay on 09-12-2020 for seperate EquityAmt
                                        if (_PositionInfo.ScripType == en_ScripType.EQ)
                                        {
                                            _CLevelInfo.EquityAmount += DivideByBaseAndRound(_PositionInfo.EquityAmount, nameof(_CPParent.EquityAmount));

                                            //changed logic on 15APR2021 by Amey
                                            _PayInPayOut += DivideByBaseAndRound(_PositionInfo.DayNetPremium, nameof(_CPParent.PayInPayOut));

                                            _CLevelInfo.VARMargin += DivideByBaseAndRound(_PositionInfo.VARMargin, nameof(_CPParent.VARMargin));


                                            _CLevelInfo.T1Quantity += _PositionInfo.T1Quantity;
                                            _CLevelInfo.T2Quantity += _PositionInfo.T2Quantity;
                                            _CLevelInfo.EarlyPayIn += _PositionInfo.EarlyPayIn;

                                        }
                                        else if (_PositionInfo.ScripType == en_ScripType.CE || _PositionInfo.ScripType == en_ScripType.PE)
                                        {
                                            //changed logic on 15APR2021 by Amey
                                            //Added by Akshay on 12-01-2021 for DayNet premium

                                            _DayNetPremium += DivideByBaseAndRound(_PositionInfo.DayNetPremium, nameof(_CPParent.DayNetPremium));
                                            _DayNetPremiumCDS += DivideByBaseAndRound(_PositionInfo.DayNetPremiumCDS, nameof(_CPParent.DayNetPremium));


                                            _ROV = DivideByBaseAndRound(_PositionInfo.NetPosition * _PositionInfo.LTP, nameof(_CPParent.ROV));
                                        }

                                        //added on 20MAY2021 by Amey
                                        _CLevelInfo.ROV += _ROV;
                                        dict_UnderlyingLevel[Underlying].ROV += _ROV;

                                        if (isCPUnderlyingExpanded)
                                        {
                                            double tDaysToExpiry = _PositionInfo.ScripType == en_ScripType.EQ ? 0 : Math.Floor((_PositionInfo.ExpiryTimeSpan.TotalDays * 0.1) * 100) / 10;

                                            var _CPPositions = new CPPositions
                                            {
                                                //Added by Akshay o 31-12-2020 for VAREQ
                                                VARMargin = DivideByBaseAndRound(_PositionInfo.VARMargin, nameof(_CPParent.VARMargin)),


                                                //adde by nikhil 
                                                T1Quantity = _PositionInfo.T1Quantity,
                                                T2Quantity = _PositionInfo.T2Quantity,

                                                EarlyPayIn = _PositionInfo.EarlyPayIn,

                                                ClientID = _ClientID,
                                                Underlying = Underlying,
                                                ScripName = _PositionInfo.ScripName,

                                                //added on 20APR2021 by Amey
                                                Segment = _PositionInfo.Segment,
                                                Series = _PositionInfo.Series,
                                                InstrumentName = _PositionInfo.InstrumentName,

                                                ScripToken = _PositionInfo.Token,
                                                ExpiryDate = tExpiry,
                                                ScripType = _PositionInfo.ScripType,
                                                StrikePrice = _PositionInfo.StrikePrice,

                                                NetPosition = _PositionInfo.NetPosition,
                                                BEP = tBEP,
                                                NetPositionCF = _PositionInfo.NetPositionCF,
                                                PriceCF = tPriceCF,

                                                //changed on 01FEB2021 by Amey
                                                LTP = _PositionInfo.LTP,
                                                UnderlyingLTP = _PositionInfo.UnderlyingLTP,

                                                //added on 08APR2021 by Amey
                                                TheoreticalPrice = _PositionInfo.TheoreticalPrice,
                                                TheoreticalMTM = tTMTM,

                                                //added on 20MAY2021 by Amey
                                                ROV = _ROV,

                                                SpotPrice = _PositionInfo.SpotPrice,

                                                AtmIV = _PositionInfo.ATM_IV,

                                                //changed on 20MAY2021 by Amey
                                                FuturesMTM = tFMTM + tCDSFMTM,
                                                OptionsMTM = tOMTM + tCDSOMTM,
                                                EquityMTM = tEMTM,
                                                MTM = tFMTM + tOMTM + tEMTM + tCDSFMTM + tCDSOMTM,

                                                //changed on 20MAY2021 by Amey
                                                /*IntradayFuturesMTM = tIntradayFMTM,
                                                IntradayOptionsMTM = tIntradayOMTM,*/

                                                //Changed on 31AUG2023 by Sujit for MCX
                                                IntradayFuturesMTM = tIntradayFMTM + tCDSIntradayFMTM,
                                                IntradayOptionsMTM = tIntradayOMTM + tCDSIntradayOMTM,
                                                IntradayEquityMTM = tIntradayEMTM,
                                                //IntradayMTM = tIntradayFMTM + tIntradayOMTM + tIntradayEMTM,
                                                IntradayMTM = tIntradayFMTM + tIntradayOMTM + tIntradayEMTM + tCDSIntradayFMTM + tCDSIntradayOMTM,
                                                IntrinsicMTM = _intrinsicmtm,

                                                IntradayBEP = tIntradayBEP,
                                                IntradayNetPosition = _PositionInfo.IntradayNetPosition,

                                                AbsDelta = tSingleDelta,    //Added by Akshay on 21-12-2020
                                                AbsGamma = tSingleGamma,    //Added by Akshay on 21-12-2020

                                                Delta = tDelta,
                                                Theta = tTheta,
                                                Gamma = tGamma,
                                                Vega = tVega,

                                                //added on 29APR2021 by Amey
                                                TV = TimeValue,

                                                //added on 3JUN2021 by Amey
                                                ExpTheta = _ExpTheta,

                                                //added on 24MAY2021 by Amey
                                                DeltaAmount = tDeltaAmt,

                                                DaysToExpiry = tDaysToExpiry,
                                                IsLTPCalculated = _PositionInfo.IsLTPCalculated,

                                                //added on 07APR2021 by Amey
                                                IntradayBuyQuantity = _PositionInfo.IntradayBuyQuantity,
                                                IntradayBuyAvg = _PositionInfo.IntradayBuyAvg,
                                                IntradaySellQuantity = _PositionInfo.IntradaySellQuantity,
                                                IntradaySellAvg = _PositionInfo.IntradaySellAvg,
                                                Turnover = tTurnover
                                            };

                                            //changed on 20APR2021 by Amey
                                            dict_Positions.Add(ScripKey, _CPPositions);
                                        }
                                    }
                                    catch (Exception ee) { _logger.Error(ee, _ClientID + "|ComputeAndUpdate Loop : " + Underlying); }
                                }

                                double CurrPosVaR = 0;
                                double CalculatedLTP = 0;

                                if (_PositionInfo.NetPosition == 0)
                                    CurrPosVaR = 0;
                                else
                                {
                                    if (_PositionInfo.ScripType == en_ScripType.EQ || _PositionInfo.ScripType == en_ScripType.XX)
                                        CalculatedLTP = (_PositionInfo.LTP * (100 + CurrOGIdx)) / 100;
                                    else if (_PositionInfo.ExpiryTimeSpan.TotalDays > 0 && (_PositionInfo.ScripType == en_ScripType.CE || _PositionInfo.ScripType == en_ScripType.PE))
                                    {
                                        if (_PositionInfo.ScripType == en_ScripType.CE && CurrIV > 0)
                                            CalculatedLTP = CommonFunctions.CallOption((_PositionInfo.UnderlyingLTP * (100 + CurrOGIdx)) / 100, _PositionInfo.StrikePrice, _PositionInfo.ExpiryTimeSpan.TotalDays / 365,
                                                0, (CurrIV / 100), 0);
                                        else if (_PositionInfo.ScripType == en_ScripType.PE && CurrIV > 0)
                                            CalculatedLTP = CommonFunctions.PutOption((_PositionInfo.UnderlyingLTP * (100 + CurrOGIdx)) / 100, _PositionInfo.StrikePrice, _PositionInfo.ExpiryTimeSpan.TotalDays / 365,
                                                0, (CurrIV / 100), 0);
                                    }

                                    if (_PositionInfo.NetPosition > 0)
                                        CurrPosVaR = (CalculatedLTP - _PositionInfo.BEP) * Math.Abs(_PositionInfo.NetPosition);
                                    else if (_PositionInfo.NetPosition < 0)
                                        CurrPosVaR = (_PositionInfo.BEP - CalculatedLTP) * Math.Abs(_PositionInfo.NetPosition);
                                }

                                var Token = _PositionInfo.Token;

                                if (!dict_UnderlyingWiseVaR.ContainsKey(Underlying))
                                {
                                    dict_UnderlyingWiseVaR.TryAdd(Underlying, new ConcurrentDictionary<int, ConcurrentDictionary<int, VaRInfo>>());
                                    dict_UnderlyingWiseVaR[Underlying].TryAdd(Token, new ConcurrentDictionary<int, VaRInfo>());
                                    dict_UnderlyingWiseVaR[Underlying][Token].TryAdd(CurrOGIdx, new VaRInfo());
                                    dict_UnderlyingWiseVaR[Underlying][Token][CurrOGIdx] = new VaRInfo() { Segment = _PositionInfo.Segment, Token = _PositionInfo.Token, VaR = CurrPosVaR, IV = CurrIV, OGIdx = CurrOGIdx, PriceAtOG = CalculatedLTP };
                                }
                                else if (!dict_UnderlyingWiseVaR[Underlying].ContainsKey(Token))
                                {
                                    dict_UnderlyingWiseVaR[Underlying].TryAdd(Token, new ConcurrentDictionary<int, VaRInfo>());
                                    dict_UnderlyingWiseVaR[Underlying][Token].TryAdd(CurrOGIdx, new VaRInfo());
                                    dict_UnderlyingWiseVaR[Underlying][Token][CurrOGIdx] = new VaRInfo() { Segment = _PositionInfo.Segment, Token = _PositionInfo.Token, VaR = CurrPosVaR, IV = CurrIV, OGIdx = CurrOGIdx, PriceAtOG = CalculatedLTP };
                                }
                                else if (!dict_UnderlyingWiseVaR[Underlying][Token].ContainsKey(CurrOGIdx))
                                {
                                    dict_UnderlyingWiseVaR[Underlying][Token].TryAdd(CurrOGIdx, new VaRInfo());
                                    dict_UnderlyingWiseVaR[Underlying][Token][CurrOGIdx] = new VaRInfo() { Segment = _PositionInfo.Segment, Token = _PositionInfo.Token, VaR = CurrPosVaR, IV = CurrIV, OGIdx = CurrOGIdx, PriceAtOG = CalculatedLTP };
                                }
                                else
                                    dict_UnderlyingWiseVaR[Underlying][Token][CurrOGIdx] = new VaRInfo() { Segment = _PositionInfo.Segment, Token = _PositionInfo.Token, VaR = CurrPosVaR, IV = CurrIV, OGIdx = CurrOGIdx, PriceAtOG = CalculatedLTP };

                                //if (!dict_UnderlyingWiseVaR.ContainsKey(Underlying))
                                //{
                                //    dict_UnderlyingWiseVaR.TryAdd(Underlying, new ConcurrentDictionary<int, VaRInfo>());
                                //    dict_UnderlyingWiseVaR[Underlying].TryAdd(Token, new VaRInfo());
                                //    dict_UnderlyingWiseVaR[Underlying][Token] = new VaRInfo() { Segment = _PositionInfo.Segment, Token = _PositionInfo.Token, VaR = CurrPosVaR, IV = CurrIV, OGIdx = CurrOGIdx, PriceAtOG = CalculatedLTP };
                                //}
                                //else if (!dict_UnderlyingWiseVaR[Underlying].ContainsKey(Token))
                                //{
                                //    dict_UnderlyingWiseVaR[Underlying].TryAdd(Token, new VaRInfo());
                                //    dict_UnderlyingWiseVaR[Underlying][Token] = new VaRInfo() { Segment = _PositionInfo.Segment, Token = _PositionInfo.Token, VaR = CurrPosVaR, IV = CurrIV, OGIdx = CurrOGIdx, PriceAtOG = CalculatedLTP };
                                //}
                                //else
                                //    dict_UnderlyingWiseVaR[Underlying][Token] = new VaRInfo() { Segment = _PositionInfo.Segment, Token = _PositionInfo.Token, VaR = CurrPosVaR, IV = CurrIV, OGIdx = CurrOGIdx, PriceAtOG = CalculatedLTP };


                                if (isVaRDistributionShown)
                                {
                                    try
                                    {
                                        var dict_VaRDistribution = CollectionHelper.dict_VaRDistribution;
                                        if (dict_VaRDistribution.ContainsKey(_ClientID))
                                        {
                                            if (!dict_VaRDistribution[_ClientID].ContainsKey(Underlying))
                                                dict_VaRDistribution[_ClientID].TryAdd(Underlying, new ConcurrentDictionary<int, double>() { [CurrOGIdx] = CurrPosVaR });
                                            else if (dict_VaRDistribution[_ClientID][Underlying].ContainsKey(CurrOGIdx))
                                                dict_VaRDistribution[_ClientID][Underlying][CurrOGIdx] = CurrPosVaR;
                                            else
                                                dict_VaRDistribution[_ClientID][Underlying].TryAdd(CurrOGIdx, CurrPosVaR);
                                        }
                                        else
                                            dict_VaRDistribution.TryAdd(_ClientID, new ConcurrentDictionary<string, ConcurrentDictionary<int, double>>() { [Underlying] = new ConcurrentDictionary<int, double>() { [CurrOGIdx] = CurrPosVaR } });
                                    }
                                    catch (Exception ee) { _logger.Error(ee, $"VarDistribution : {_ClientID}"); }
                                }
                            }
                        }
                    }
                }

                //added ToList on 12APR2021 by Amey. For some reason without it, throws CollectionWasModified exception.
                foreach (var _ScripKey in dict_TokensRemove.Keys.ToList())
                {
                    if (dict_TokensRemove[_ScripKey] != "")
                    {
                        dict_TokensRemove[_ScripKey] = "";

                        CPUnderlying _UnderlyingRow = null;
                        CPPositions _ExpiredPosition = null;

                        //added on 18FEB2021 by Amey
                        lock (CollectionHelper._CPLock)
                        {
                            _UnderlyingRow = _CPParent.bList_Underlying.Where(v => v.Underlying == dict_TokensRemove[_ScripKey]).FirstOrDefault();
                            if (_UnderlyingRow is null)
                                continue;

                            //added on 20APR2021 by Amey
                            var TokenToRemove = Convert.ToInt32(_ScripKey.Split('|')[1]);
                            var TokenSegment = _ScripKey.Split('|')[0];

                            //changed on 20APR2021 by Amey
                            _ExpiredPosition = _UnderlyingRow.bList_Positions.Where(v => v.ScripToken == TokenToRemove && v.Segment.ToString() == TokenSegment).FirstOrDefault();
                            if (_ExpiredPosition is null)
                                continue;
                        }

                        CollectionHelper.gc_CP.Invoke((MethodInvoker)(() =>
                        {
                            _UnderlyingRow.bList_Positions.Remove(_ExpiredPosition);
                        }));
                    }
                }

                //Added by Akshay on 23-08-2021 for UpsideDown VaR
                if (CollectionHelper.IsFullVAR)
                {
                    foreach (var Underlying in dict_MaxLossUnderlyingWise.Keys)
                    {

                        UpdateCPParent = true;

                        double ULevelVaR = 0;

                        //changed on 07DEC2020 by Amey
                        var MaxVaRVal = dict_MaxLossUnderlyingWise[Underlying].Min(v => v.MaxVaR);
                        var MaxVaRinfo = dict_MaxLossUnderlyingWise[Underlying].Where(v => v.MaxVaR == MaxVaRVal).FirstOrDefault();

                        var _UnderlyingInfo = dict_UnderlyingLevel[Underlying];


                        long ITMQty = _UnderlyingInfo.CallBuyQty + Math.Abs(_UnderlyingInfo.PutBuyQty);
                        long DeliveryQty = _UnderlyingInfo.CallBuyQty + _UnderlyingInfo.CallSellQty + _UnderlyingInfo.PutBuyQty + _UnderlyingInfo.PutSellQty + _UnderlyingInfo.FutQty;
                        long DeliveryMargin = Math.Min(ITMQty, Math.Abs(DeliveryQty));

                        CLevelDelMargin += DivideByBaseAndRound(Math.Abs(DeliveryMargin) * _UnderlyingInfo.ValMargin, nameof(_CPParent.DeliveryMargin));

                        CPUnderlying _CPUnderlying = null;

                        if (isCPParentExpanded)
                        {
                            //added on 18FEB2021 by Amey
                            lock (CollectionHelper._CPLock)
                            {
                                _CPUnderlying = _CPParent.bList_Underlying.Where(v => v.Underlying == Underlying).FirstOrDefault();
                            }

                            string Key = _ClientID + "_" + Underlying;

                            //changed to TryGetValue on 27MAY2021 by Amey
                            //changed on 16FEB2021 by Amey
                            if (CollectionHelper.dict_ClientUnderlyingWiseSpanInfo.TryGetValue(Key, out UnderlyingSpanInfo _UnderlyingSpanInfo))
                            {
                                _UnderlyingInfo.Span = _UnderlyingSpanInfo.Span;
                                _UnderlyingInfo.Exposure = _UnderlyingSpanInfo.Exposure;
                                _UnderlyingInfo.MarginUtil = _UnderlyingSpanInfo.MarginUtil;
                            }
                            else if (CollectionHelper.dict_CDSClientUnderlyingWiseSpanInfo.TryGetValue(Key, out UnderlyingSpanInfo _CDSUnderlyingSpanInfo))
                            {
                                _UnderlyingInfo.Span = _CDSUnderlyingSpanInfo.Span;
                                _UnderlyingInfo.Exposure = _CDSUnderlyingSpanInfo.Exposure;
                                _UnderlyingInfo.MarginUtil = _CDSUnderlyingSpanInfo.MarginUtil;
                            }
                            if (CollectionHelper.dict_ClientUnderlyingWiseConsolidatedSpanInfo.TryGetValue(Key, out UnderlyingSpanInfo _UnderlyingConSpanInfo))
                            {
                                _UnderlyingInfo.SnapSpan = _UnderlyingConSpanInfo.Span;
                                _UnderlyingInfo.SnapExposure = _UnderlyingConSpanInfo.Exposure;
                                _UnderlyingInfo.SnapMarginUtil = _UnderlyingConSpanInfo.MarginUtil;
                            }

                            //added here to avoid computation in Invoke. 25MAR2021-Amey
                            var UNPLKey = $"{_ClientID}^{Underlying}";

                            var TotalMTM = _UnderlyingInfo.FuturesMTM + _UnderlyingInfo.OptionsMTM + _UnderlyingInfo.EquityMTM + _UnderlyingInfo.CDSFuturesMTM + _UnderlyingInfo.CDSOptionsMTM;

                            //changed to TryGetValue on 27MAY2021 by Amey
                            //changed default value to MTM on 06MAY2021 by Amey
                            var UNPL = TotalMTM;
                            if (CollectionHelper.dict_NPLValues.TryGetValue(UNPLKey, out double _NPL))
                                UNPL = TotalMTM + DivideByBaseAndRound(_NPL, nameof(_CPParent.MTM));

                            CollectionHelper.gc_CP.Invoke((MethodInvoker)(() =>
                            {
                                if (_CPUnderlying is null)
                                {
                                    _CPUnderlying = new CPUnderlying();
                                    _CPUnderlying.ClientID = _ClientID;
                                    _CPUnderlying.Underlying = Underlying;

                                    _CPUnderlying.bList_Positions = new BindingList<CPPositions>();

                                    _CPParent.bList_Underlying.Add(_CPUnderlying);

                                    //added on 08MAR2021 by Amey
                                    isInitialLoadCPUnderlyingSuccess = true;
                                }

                                //added on 26MAY2021 by Amey
                                _CPUnderlying.FuturesMTM = _UnderlyingInfo.FuturesMTM + _UnderlyingInfo.CDSFuturesMTM ;
                                _CPUnderlying.OptionsMTM = _UnderlyingInfo.OptionsMTM + _UnderlyingInfo.CDSOptionsMTM;
                                _CPUnderlying.EquityMTM = _UnderlyingInfo.EquityMTM;

                                _CPUnderlying.MTM = TotalMTM;

                                //added on 28MAY2021 by Amey
                                _CPUnderlying.IntradayFuturesMTM = _UnderlyingInfo.IntradayFuturesMTM + _UnderlyingInfo.CDSIntradayFuturesMTM ;
                                _CPUnderlying.IntradayOptionsMTM = _UnderlyingInfo.IntradayOptionsMTM + _UnderlyingInfo.CDSIntradayOptionsMTM ;
                                _CPUnderlying.IntradayEquityMTM = _UnderlyingInfo.IntradayEquityMTM;

                                //added on 15APR2021 by Amey
                                _CPUnderlying.IntradayMTM = _UnderlyingInfo.IntradayFuturesMTM + _UnderlyingInfo.IntradayOptionsMTM + _UnderlyingInfo.IntradayEquityMTM + _UnderlyingInfo.CDSIntradayFuturesMTM + _UnderlyingInfo.CDSIntradayOptionsMTM ;
                                _CPUnderlying.TheoreticalMTM = _UnderlyingInfo.TheoreticalMTM;

                                //added on 20MAY2021 by Amey
                                _CPUnderlying.ROV = _UnderlyingInfo.ROV;

                                //added on 25MAR2021 by Amey
                                _CPUnderlying.NPL = UNPL;

                                _CPUnderlying.Delta = _UnderlyingInfo.Delta;
                                _CPUnderlying.AbsDelta = _UnderlyingInfo.SingleDelta;   //Added by Akshay on 21-12-2020
                                _CPUnderlying.AbsGamma = _UnderlyingInfo.SingleGamma;   //Added by Akshay on 21-12-2020
                                _CPUnderlying.Theta = _UnderlyingInfo.Theta;
                                _CPUnderlying.Gamma = _UnderlyingInfo.Gamma;
                                _CPUnderlying.Vega = _UnderlyingInfo.Vega;
                                _CPUnderlying.Span = _UnderlyingInfo.Span;
                                _CPUnderlying.Exposure = _UnderlyingInfo.Exposure;
                                _CPUnderlying.MarginUtil = _UnderlyingInfo.MarginUtil;
                                _CPUnderlying.SnapSpan = _UnderlyingInfo.SnapSpan;
                                _CPUnderlying.SnapExposure = _UnderlyingInfo.SnapExposure;
                                _CPUnderlying.SnapMarginUtil = _UnderlyingInfo.SnapMarginUtil;

                                _CPUnderlying.IntrinsicMTM = DivideByBaseAndRound(_UnderlyingInfo.IntrinsicMTM, nameof(_CPUnderlying.MTM));

                                //added on 29APR2021 by Amey
                                _CPUnderlying.TV = _UnderlyingInfo.TimeValue;

                                //added on 3JUN2021 by Amey
                                _CPUnderlying.ExpTheta = _UnderlyingInfo.ExpTheta;

                                //added on 24MAY2021 by Amey
                                _CPUnderlying.DeltaAmount = _UnderlyingInfo.DeltaAmount;

                                //Added by Akshay on 23-03-2021
                                _CPUnderlying.NetposDelMargin = DeliveryMargin;
                                _CPUnderlying.DeliveryMargin = DivideByBaseAndRound(Math.Abs(DeliveryMargin) * _UnderlyingInfo.ValMargin, nameof(_CPParent.DeliveryMargin));
                                _CPUnderlying.Obligation = DeliveryMargin;

                                //Added by Akshay on 29-06-2021
                                _CPUnderlying.PosExpoOPT = DivideByBaseAndRound((_UnderlyingInfo.NetCallQnty - _UnderlyingInfo.NetPutQnty) * _UnderlyingInfo.ClosingPrice, nameof(_CPUnderlying.PosExpoOPT));
                                _CPUnderlying.PosExpoFUT = DivideByBaseAndRound(_UnderlyingInfo.NetFutQnty * _UnderlyingInfo.ClosingPrice, nameof(_CPUnderlying.PosExpoFUT));
                                _CPUnderlying.NetQnty = _UnderlyingInfo.NetQnty;
                                _CPUnderlying.Turnover = _UnderlyingInfo.Turnover;

                                //Added by Akshay for Collateral
                                _CPUnderlying.CollateralQty = _UnderlyingInfo.CollateralQty;
                                _CPUnderlying.CollateralValue = _UnderlyingInfo.CollateralValue;
                                _CPUnderlying.CollateralHaircut = _UnderlyingInfo.CollateralHaircut;
                            }));
                        }

                        //changed on 07DEC2020 by Amey
                        foreach (var VaRInfo in MaxVaRinfo.list_VaRInfo)
                        {
                            //changed on 07DEC2020 by Amey
                            int Token = VaRInfo.Token;
                            var TokenSegment = VaRInfo.Segment;

                            //added IsVaRValueReversed check on 15APR2021 by Amey
                            //changed on 07DEC2020 by Amey
                            var MaxVarCalculated = VaRInfo.VaR * (CollectionHelper._ValueSigns.VaR);

                            //added on 1JUN2021 by Amey
                            CLevelVaRAbs += MaxVarCalculated;

                            //added DivideByBaseAndRound here to sum Absoulye values in CLevelVaR and ULevelVaR on 03MAY2021 by Amey
                            MaxVarCalculated = DivideByBaseAndRound(MaxVarCalculated, nameof(_CPParent.VAR));

                            //changed position on 1JUN2021 by Amey
                            CLevelVaR += MaxVarCalculated;
                            ULevelVaR += MaxVarCalculated;

                            //added on 05APR2021 by Amey
                            isCPUnderlyingExpanded = isCPParentExpanded && (list_ExpandedUnderlying.Contains(Underlying) || !dict_IsInitialCPPositionLoadSuccess[Underlying]);

                            //changed on 19FEB2021 by Amey
                            if (isCPUnderlyingExpanded)
                            {
                                CPPositions _CPPositions = null;

                                //added on 18FEB2021 by Amey
                                lock (CollectionHelper._CPLock)
                                {
                                    _CPPositions = _CPUnderlying.bList_Positions.Where(v => v.ScripToken == Token).FirstOrDefault();
                                }

                                //changed to TryGetValue on 27MAY2021 by Amey
                                //added check on 10MAY2021 by Amey
                                if (dict_Positions.TryGetValue(TokenSegment + "|" + Token, out CPPositions _Position))
                                {
                                    CollectionHelper.gc_CP.Invoke((MethodInvoker)(() =>
                                    {
                                        if (_CPPositions is null)
                                        {
                                            _CPPositions = new CPPositions
                                            {
                                                ClientID = _ClientID,
                                                Underlying = _Position.Underlying,
                                                ScripName = _Position.ScripName,

                                                //added on 20APR2021 by Amey
                                                Segment = _Position.Segment,
                                                Series = _Position.Series,

                                                InstrumentName = _Position.InstrumentName,

                                                ScripToken = _Position.ScripToken,
                                                ExpiryDate = _Position.ExpiryDate,
                                                ScripType = _Position.ScripType,
                                                StrikePrice = _Position.StrikePrice
                                            };

                                            _CPUnderlying.bList_Positions.Add(_CPPositions);

                                            //added on 08MAR2021 by Amey
                                            dict_IsInitialCPPositionLoadSuccess[Underlying] = true;
                                        }

                                        _CPPositions.NetPosition = _Position.NetPosition;
                                        _CPPositions.BEP = _Position.BEP;

                                        _CPPositions.NetPositionCF = _Position.NetPositionCF;
                                        _CPPositions.PriceCF = _Position.PriceCF;

                                        _CPPositions.LTP = _Position.LTP;
                                        _CPPositions.UnderlyingLTP = _Position.UnderlyingLTP;

                                        //added on 08APR2021 by Amey
                                        _CPPositions.TheoreticalPrice = _Position.TheoreticalPrice;
                                        _CPPositions.TheoreticalMTM = _Position.TheoreticalMTM;

                                        //added on 20MAY2021 by Amey
                                        _CPPositions.ROV = _Position.ROV;

                                        _CPPositions.SpotPrice = _Position.SpotPrice;

                                        _CPPositions.AtmIV = _Position.AtmIV;

                                        _CPPositions.FuturesMTM = _Position.FuturesMTM;
                                        _CPPositions.OptionsMTM = _Position.OptionsMTM;
                                        _CPPositions.EquityMTM = _Position.EquityMTM;
                                        _CPPositions.MTM = _Position.MTM;

                                        _CPPositions.IntradayFuturesMTM = _Position.IntradayFuturesMTM;
                                        _CPPositions.IntradayOptionsMTM = _Position.IntradayOptionsMTM;
                                        _CPPositions.IntradayEquityMTM = _Position.IntradayEquityMTM;
                                        _CPPositions.IntradayMTM = _Position.IntradayMTM;

                                        _CPPositions.IntradayBEP = _Position.IntradayBEP;
                                        _CPPositions.IntradayNetPosition = _Position.IntradayNetPosition;

                                        _CPPositions.AbsDelta = _Position.AbsDelta;   //Added by Akshay on 21-12-2020
                                        _CPPositions.AbsGamma = _Position.AbsGamma;   //Added by Akshay on 21-12-2020

                                        _CPPositions.Delta = _Position.Delta;
                                        _CPPositions.Theta = _Position.Theta;
                                        _CPPositions.Gamma = _Position.Gamma;
                                        _CPPositions.Vega = _Position.Vega;

                                        //added on 29APR2021 by Amey
                                        _CPPositions.TV = _Position.TV;

                                        //added on 3JUN2021 by Amey
                                        _CPPositions.ExpTheta = _Position.ExpTheta;

                                        //added on 24MAY2021 by Amey
                                        _CPPositions.DeltaAmount = _Position.DeltaAmount;

                                        _CPPositions.DaysToExpiry = _Position.DaysToExpiry;
                                        _CPPositions.IsLTPCalculated = _Position.IsLTPCalculated;

                                        //Added by Akshay on 31-12-2020 For VAREQ
                                        _CPPositions.VARMargin = _Position.VARMargin;

                                        //Added by nikhil
                                        _CPPositions.T1Quantity = _Position.T1Quantity;
                                        _CPPositions.T2Quantity = _Position.T2Quantity;
                                        _CPPositions.EarlyPayIn = _Position.EarlyPayIn;


                                        _CPPositions.VAR = MaxVarCalculated;
                                        _CPPositions.OGRange = VaRInfo.OGIdx;
                                        _CPPositions.IV = VaRInfo.IV;
                                        _CPPositions.PriceAtOG = VaRInfo.PriceAtOG;

                                        //added on 13APR2021 by Amey
                                        _CPPositions.IntradayBuyQuantity = _Position.IntradayBuyQuantity;
                                        _CPPositions.IntradayBuyAvg = _Position.IntradayBuyAvg;
                                        _CPPositions.IntradaySellQuantity = _Position.IntradaySellQuantity;
                                        _CPPositions.IntradaySellAvg = _Position.IntradaySellAvg;
                                        _CPPositions.Turnover = DivideByBaseAndRound((_Position.IntradayBuyQuantity * _Position.IntradayBuyAvg) + (_Position.IntradaySellQuantity * _Position.IntradaySellAvg), nameof(_CPParent.Turnover));
                                        _CPPositions.IntrinsicMTM = DivideByBaseAndRound(_Position.IntrinsicMTM, nameof(_CPParent.IntrinsicMTM));
                                    
                                    }));
                                }
                            }
                        }

                        if (isCPParentExpanded)
                        {
                            CollectionHelper.gc_CP.Invoke((MethodInvoker)(() =>
                            {
                                _CPUnderlying.VAR = ULevelVaR;
                            }));
                        }
                    }
                }

                //Added by Akshay on 23-08-2021 for UpsideDown VaR
                else if (!CollectionHelper.IsFullVAR)
                {
                    foreach (var Underlying in dict_UnderlyingWiseVaR.Keys)
                    {
                        UpdateCPParent = true;

                        double ULevelVaR = 0;
                        double ULevelScenario1 = 0, ULevelScenario2 = 0, ULevelScenario3 = 0, ULevelScenario4 = 0;

                        var _UnderlyingInfo = dict_UnderlyingLevel[Underlying];

                        long ITMQty = _UnderlyingInfo.CallBuyQty + Math.Abs(_UnderlyingInfo.PutBuyQty);
                        long DeliveryQty = _UnderlyingInfo.CallBuyQty + _UnderlyingInfo.CallSellQty + _UnderlyingInfo.PutBuyQty + _UnderlyingInfo.PutSellQty + _UnderlyingInfo.FutQty;
                        long DeliveryMargin = Math.Min(ITMQty, Math.Abs(DeliveryQty));

                        CLevelDelMargin += DivideByBaseAndRound(Math.Abs(DeliveryMargin) * _UnderlyingInfo.ValMargin, nameof(_CPParent.DeliveryMargin));

                        CPUnderlying _CPUnderlying = null;

                        if (isCPParentExpanded)
                        {
                            //added on 18FEB2021 by Amey
                            lock (CollectionHelper._CPLock)
                            {
                                _CPUnderlying = _CPParent.bList_Underlying.Where(v => v.Underlying == Underlying).FirstOrDefault();
                            }

                            string Key = _ClientID + "_" + Underlying;

                            //changed to TryGetValue on 27MAY2021 by Amey
                            //changed on 16FEB2021 by Amey
                            if (CollectionHelper.dict_ClientUnderlyingWiseSpanInfo.TryGetValue(Key, out UnderlyingSpanInfo _UnderlyingSpanInfo))
                            {
                                _UnderlyingInfo.Span = _UnderlyingSpanInfo.Span;
                                _UnderlyingInfo.Exposure = _UnderlyingSpanInfo.Exposure;
                                _UnderlyingInfo.MarginUtil = _UnderlyingSpanInfo.MarginUtil;
                            }
                            else if (CollectionHelper.dict_CDSClientUnderlyingWiseSpanInfo.TryGetValue(Key, out UnderlyingSpanInfo _CDSUnderlyingSpanInfo))
                            {
                                _UnderlyingInfo.Span = _CDSUnderlyingSpanInfo.Span;
                                _UnderlyingInfo.Exposure = _CDSUnderlyingSpanInfo.Exposure;
                                _UnderlyingInfo.MarginUtil = _CDSUnderlyingSpanInfo.MarginUtil;
                            }
                            if (CollectionHelper.dict_ClientUnderlyingWiseConsolidatedSpanInfo.TryGetValue(Key, out UnderlyingSpanInfo _UnderlyingConSpanInfo))
                            {
                                _UnderlyingInfo.SnapSpan = _UnderlyingConSpanInfo.Span;
                                _UnderlyingInfo.SnapExposure = _UnderlyingConSpanInfo.Exposure;
                                _UnderlyingInfo.SnapMarginUtil = _UnderlyingConSpanInfo.MarginUtil;
                            }

                            //added here to avoid computation in Invoke. 25MAR2021-Amey
                            var UNPLKey = $"{_ClientID}^{Underlying}";

                            var TotalMTM = _UnderlyingInfo.FuturesMTM + _UnderlyingInfo.OptionsMTM + _UnderlyingInfo.EquityMTM + _UnderlyingInfo.CDSFuturesMTM + _UnderlyingInfo.CDSOptionsMTM;


                            //changed to TryGetValue on 27MAY2021 by Amey
                            //changed default value to MTM on 06MAY2021 by Amey
                            var UNPL = TotalMTM;
                            if (CollectionHelper.dict_NPLValues.TryGetValue(UNPLKey, out double _NPL))
                                UNPL = TotalMTM + DivideByBaseAndRound(_NPL, nameof(_CPParent.MTM));

                            CollectionHelper.gc_CP.Invoke((MethodInvoker)(() =>
                            {
                                if (_CPUnderlying is null)
                                {
                                    _CPUnderlying = new CPUnderlying();
                                    _CPUnderlying.ClientID = _ClientID;
                                    _CPUnderlying.Underlying = Underlying;

                                    _CPUnderlying.bList_Positions = new BindingList<CPPositions>();

                                    _CPParent.bList_Underlying.Add(_CPUnderlying);

                                    //added on 08MAR2021 by Amey
                                    isInitialLoadCPUnderlyingSuccess = true;
                                }

                                _CPUnderlying.FuturesMTM = _UnderlyingInfo.FuturesMTM + _UnderlyingInfo.CDSFuturesMTM;
                                //added on 26MAY2021 by Amey
                                _CPUnderlying.OptionsMTM = _UnderlyingInfo.OptionsMTM + _UnderlyingInfo.CDSOptionsMTM;
                                _CPUnderlying.EquityMTM = _UnderlyingInfo.EquityMTM;

                                _CPUnderlying.MTM = TotalMTM;

                                //added on 28MAY2021 by Amey
                                _CPUnderlying.IntradayFuturesMTM = _UnderlyingInfo.IntradayFuturesMTM + _UnderlyingInfo.CDSIntradayFuturesMTM;
                                _CPUnderlying.IntradayOptionsMTM = _UnderlyingInfo.IntradayOptionsMTM + _UnderlyingInfo.CDSIntradayOptionsMTM;
                                _CPUnderlying.IntradayEquityMTM = _UnderlyingInfo.IntradayEquityMTM;

                                //added on 15APR2021 by Amey
                                _CPUnderlying.IntradayMTM = _UnderlyingInfo.IntradayFuturesMTM + _UnderlyingInfo.IntradayOptionsMTM + _UnderlyingInfo.IntradayEquityMTM + _UnderlyingInfo.CDSIntradayFuturesMTM + _UnderlyingInfo.CDSIntradayOptionsMTM;
                                _CPUnderlying.TheoreticalMTM = _UnderlyingInfo.TheoreticalMTM;

                                //added on 20MAY2021 by Amey
                                _CPUnderlying.ROV = _UnderlyingInfo.ROV;

                                //added on 25MAR2021 by Amey
                                _CPUnderlying.NPL = UNPL;

                                _CPUnderlying.Delta = _UnderlyingInfo.Delta;
                                _CPUnderlying.AbsDelta = _UnderlyingInfo.SingleDelta;   //Added by Akshay on 21-12-2020
                                _CPUnderlying.AbsGamma = _UnderlyingInfo.SingleGamma;   //Added by Akshay on 21-12-2020
                                _CPUnderlying.Theta = _UnderlyingInfo.Theta;
                                _CPUnderlying.Gamma = _UnderlyingInfo.Gamma;
                                _CPUnderlying.Vega = _UnderlyingInfo.Vega;
                                _CPUnderlying.Span = _UnderlyingInfo.Span;
                                _CPUnderlying.Exposure = _UnderlyingInfo.Exposure;
                                _CPUnderlying.MarginUtil = _UnderlyingInfo.MarginUtil;
                                _CPUnderlying.SnapSpan = _UnderlyingInfo.SnapSpan;
                                _CPUnderlying.SnapExposure = _UnderlyingInfo.SnapExposure;
                                _CPUnderlying.SnapMarginUtil = _UnderlyingInfo.SnapMarginUtil;

                                _CPUnderlying.IntrinsicMTM = DivideByBaseAndRound(_UnderlyingInfo.IntrinsicMTM, nameof(_CPUnderlying.MTM)); 

                                //added on 29APR2021 by Amey
                                _CPUnderlying.TV = _UnderlyingInfo.TimeValue;

                                //added on 3JUN2021 by Amey
                                _CPUnderlying.ExpTheta = _UnderlyingInfo.ExpTheta;

                                //added on 24MAY2021 by Amey
                                _CPUnderlying.DeltaAmount = _UnderlyingInfo.DeltaAmount;

                                //Added by Akshay on 23-03-2021
                                _CPUnderlying.NetposDelMargin = DeliveryMargin;
                                _CPUnderlying.DeliveryMargin = DivideByBaseAndRound(Math.Abs(DeliveryMargin) * _UnderlyingInfo.ValMargin, nameof(_CPParent.DeliveryMargin));
                                _CPUnderlying.Obligation = DeliveryMargin;

                                //Added by Akshay on 29-06-2021
                                _CPUnderlying.PosExpoOPT = DivideByBaseAndRound((_UnderlyingInfo.NetCallQnty - _UnderlyingInfo.NetPutQnty) * _UnderlyingInfo.ClosingPrice, nameof(_CPUnderlying.PosExpoOPT));
                                _CPUnderlying.PosExpoFUT = DivideByBaseAndRound(_UnderlyingInfo.NetFutQnty * _UnderlyingInfo.ClosingPrice, nameof(_CPUnderlying.PosExpoFUT));
                                _CPUnderlying.NetQnty = _UnderlyingInfo.NetQnty;
                                _CPUnderlying.Turnover = _UnderlyingInfo.Turnover;
                                _CPUnderlying.CollateralQty = _UnderlyingInfo.CollateralQty;
                                _CPUnderlying.CollateralValue = _UnderlyingInfo.CollateralValue;
                                _CPUnderlying.CollateralHaircut = _UnderlyingInfo.CollateralHaircut;
                            }));
                        }

                        //changed on 07DEC2020 by Amey
                        foreach (var TokenKey in dict_UnderlyingWiseVaR[Underlying].Values)
                        {
                            var iRange = 0;
                            foreach (var VaRInfo in TokenKey.Values.OrderBy(v => v.OGIdx))
                            //foreach(var VaRInfo in dict_UnderlyingWiseVaR[Underlying].Values)
                            {
                                iRange += 1;
                                //changed on 07DEC2020 by Amey
                                var Token = VaRInfo.Token;
                                var TokenSegment = VaRInfo.Segment;

                                var VARIDX = VaRInfo.OGIdx;
                                var VaR = VaRInfo.VaR * (CollectionHelper._ValueSigns.VaR);

                                VaR = DivideByBaseAndRound(VaR, nameof(_CPParent.VAR));

                                switch (iRange)
                                {
                                    case 1:
                                        CLevelScenario1 += VaR;
                                        ULevelScenario1 += VaR;
                                        break;
                                    case 2:
                                        CLevelScenario2 += VaR;
                                        ULevelScenario2 += VaR;
                                        break;
                                    case 3:
                                        CLevelScenario3 += VaR;
                                        ULevelScenario3 += VaR;
                                        break;
                                    case 4:
                                        CLevelScenario4 += VaR;
                                        ULevelScenario4 += VaR;
                                        break;
                                    default:
                                        break;
                                }

                                //added on 05APR2021 by Amey
                                isCPUnderlyingExpanded = isCPParentExpanded && (list_ExpandedUnderlying.Contains(Underlying) || !dict_IsInitialCPPositionLoadSuccess[Underlying]);

                                //changed on 19FEB2021 by Amey
                                if (isCPUnderlyingExpanded)
                                {
                                    CPPositions _CPPositions = null;

                                    //added on 18FEB2021 by Amey
                                    lock (CollectionHelper._CPLock)
                                    {
                                        _CPPositions = _CPUnderlying.bList_Positions.Where(v => v.ScripToken == Token).FirstOrDefault();
                                    }

                                    //changed to TryGetValue on 27MAY2021 by Amey
                                    //added check on 10MAY2021 by Amey
                                    if (dict_Positions.TryGetValue(TokenSegment + "|" + Token, out CPPositions _Position))
                                    {
                                        CollectionHelper.gc_CP.Invoke((MethodInvoker)(() =>
                                        {
                                            if (_CPPositions is null)
                                            {
                                                _CPPositions = new CPPositions
                                                {
                                                    ClientID = _ClientID,
                                                    Underlying = _Position.Underlying,
                                                    ScripName = _Position.ScripName,

                                                    //added on 20APR2021 by Amey
                                                    Segment = _Position.Segment,
                                                    Series = _Position.Series,

                                                    InstrumentName = _Position.InstrumentName,

                                                    ScripToken = _Position.ScripToken,
                                                    ExpiryDate = _Position.ExpiryDate,
                                                    ScripType = _Position.ScripType,
                                                    StrikePrice = _Position.StrikePrice
                                                };

                                                _CPUnderlying.bList_Positions.Add(_CPPositions);

                                                //added on 08MAR2021 by Amey
                                                dict_IsInitialCPPositionLoadSuccess[Underlying] = true;
                                            }

                                            if (iRange == 1)
                                            {
                                                _CPPositions.NetPosition = _Position.NetPosition;
                                                _CPPositions.BEP = _Position.BEP;

                                                _CPPositions.NetPositionCF = _Position.NetPositionCF;
                                                _CPPositions.PriceCF = _Position.PriceCF;

                                                _CPPositions.LTP = _Position.LTP;
                                                _CPPositions.UnderlyingLTP = _Position.UnderlyingLTP;

                                                //added on 08APR2021 by Amey
                                                _CPPositions.TheoreticalPrice = _Position.TheoreticalPrice;
                                                _CPPositions.TheoreticalMTM = _Position.TheoreticalMTM;

                                                //added on 20MAY2021 by Amey
                                                _CPPositions.ROV = _Position.ROV;

                                                _CPPositions.SpotPrice = _Position.SpotPrice;

                                                _CPPositions.AtmIV = _Position.AtmIV;

                                                _CPPositions.FuturesMTM = _Position.FuturesMTM;
                                                _CPPositions.OptionsMTM = _Position.OptionsMTM;
                                                _CPPositions.EquityMTM = _Position.EquityMTM;
                                                _CPPositions.MTM = _Position.MTM;

                                                _CPPositions.IntradayFuturesMTM = _Position.IntradayFuturesMTM;
                                                _CPPositions.IntradayOptionsMTM = _Position.IntradayOptionsMTM;
                                                _CPPositions.IntradayEquityMTM = _Position.IntradayEquityMTM;
                                                _CPPositions.IntradayMTM = _Position.IntradayMTM;

                                                _CPPositions.IntradayBEP = _Position.IntradayBEP;
                                                _CPPositions.IntradayNetPosition = _Position.IntradayNetPosition;

                                                _CPPositions.AbsDelta = _Position.AbsDelta;   //Added by Akshay on 21-12-2020
                                                _CPPositions.AbsGamma = _Position.AbsGamma;   //Added by Akshay on 21-12-2020

                                                _CPPositions.Delta = _Position.Delta;
                                                _CPPositions.Theta = _Position.Theta;
                                                _CPPositions.Gamma = _Position.Gamma;
                                                _CPPositions.Vega = _Position.Vega;

                                                //added on 29APR2021 by Amey
                                                _CPPositions.TV = _Position.TV;

                                                //added on 3JUN2021 by Amey
                                                _CPPositions.ExpTheta = _Position.ExpTheta;

                                                //added on 24MAY2021 by Amey
                                                _CPPositions.DeltaAmount = _Position.DeltaAmount;

                                                _CPPositions.DaysToExpiry = _Position.DaysToExpiry;
                                                _CPPositions.IsLTPCalculated = _Position.IsLTPCalculated;

                                                //Added by Akshay on 31-12-2020 For VAREQ
                                                _CPPositions.VARMargin = _Position.VARMargin;

                                                _CPPositions.T1Quantity = _Position.T1Quantity;
                                                _CPPositions.T2Quantity = _Position.T2Quantity;
                                                _CPPositions.EarlyPayIn = _Position.EarlyPayIn;

                                                //_CPPositions.VAR = MaxVarCalculated;
                                                _CPPositions.OGRange = VaRInfo.OGIdx;
                                                _CPPositions.IV = VaRInfo.IV;
                                                _CPPositions.PriceAtOG = VaRInfo.PriceAtOG;

                                                //added on 13APR2021 by Amey
                                                _CPPositions.IntradayBuyQuantity = _Position.IntradayBuyQuantity;
                                                _CPPositions.IntradayBuyAvg = _Position.IntradayBuyAvg;
                                                _CPPositions.IntradaySellQuantity = _Position.IntradaySellQuantity;
                                                _CPPositions.IntradaySellAvg = _Position.IntradaySellAvg;
                                                _CPPositions.Turnover = DivideByBaseAndRound((_Position.IntradayBuyQuantity * _Position.IntradayBuyAvg) + (_Position.IntradaySellQuantity * _Position.IntradaySellAvg), nameof(_CPParent.Turnover));
                                                _CPPositions.IntrinsicMTM = DivideByBaseAndRound(_Position.IntrinsicMTM, nameof(_CPParent.IntrinsicMTM));
                                            }

                                            switch (iRange)
                                            {
                                                case 1:
                                                    _CPPositions.Scenario1 = VaR;
                                                    break;
                                                case 2:
                                                    _CPPositions.Scenario2 = VaR;
                                                    break;
                                                case 3:
                                                    _CPPositions.Scenario3 = VaR;
                                                    break;
                                                case 4:
                                                    _CPPositions.Scenario4 = VaR;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }));
                                    }
                                }

                            }
                        }

                        if (isCPParentExpanded)
                        {
                            CollectionHelper.gc_CP.Invoke((MethodInvoker)(() =>
                            {
                                _CPUnderlying.VAR = ULevelVaR;
                                _CPUnderlying.Scenario1 = ULevelScenario1;
                                _CPUnderlying.Scenario2 = ULevelScenario2;
                                _CPUnderlying.Scenario3 = ULevelScenario3;
                                _CPUnderlying.Scenario4 = ULevelScenario4;
                            }));
                        }
                    }
                }


                dict_Positions.Clear();

                //added on 06MAY2021 by Amey. To avoid updating parent row with 0 values if Feed disconnected.
                if (UpdateCPParent)
                {
                    if (CollectionHelper.dict_ClientInfo.TryGetValue(_ClientID, out ClientInfo _CInfo))
                    {
                        _CPParent.Name = _CInfo.Name;
                        _CPParent.Ledger = _CInfo.ELM;
                        _CPParent.Adhoc = _CInfo.AdHoc;
                        _CPParent.Zone = _CInfo.Zone;
                        _CPParent.Branch = _CInfo.Branch;
                        _CPParent.Family = _CInfo.Family;
                        _CPParent.Product = _CInfo.Product;
                    }

                    _CPParent.VAR = CLevelVaR;

                    _CPParent.Scenario1 = CLevelScenario1;
                    _CPParent.Scenario2 = CLevelScenario2;
                    _CPParent.Scenario3 = CLevelScenario3;
                    _CPParent.Scenario4 = CLevelScenario4;
                    _CPParent.DeliveryMargin = CLevelDelMargin;  //Added by Akshay on 24-03-2021

                    //changed to 100 on 03MAY2021 by Amey
                    //added <= 0 condition on 21OCT2020 by Amey. To avoid DivideByZero error.
                    if (_CPParent.Ledger + _CPParent.Adhoc <= 0)
                        _CPParent.VaRUti = 100;
                    else
                        _CPParent.VaRUti = Math.Round((CLevelVaRAbs / (_CPParent.Ledger + _CPParent.Adhoc)) * 100, 2);                        //removed + (2 * _rov) on 16-11-2019

                    //Chnaged by Akshay on 11-01-2021 for displaying data
                    _CPParent.EquityAmount = _CLevelInfo.EquityAmount;
                    _CPParent.PayInPayOut = _PayInPayOut;
                    _CPParent.VARMargin = _CLevelInfo.VARMargin;

                    //added by nikhil
                    _CPParent.T1Quantity = _CLevelInfo.T1Quantity;
                    _CPParent.T2Quantity = _CLevelInfo.T2Quantity;
                    _CPParent.EarlyPayIn = _CLevelInfo.EarlyPayIn;


                    //Added by Akshay on 12-01-2021 for Displaying DayNet Premium 
                    _CPParent.DayNetPremium = _DayNetPremium;
                    _CPParent.DayNetPremiumCDS = _DayNetPremiumCDS;
                    //added on 17MAY2021 by Amey
                    _CPParent.DayPremium = _DayNetPremium * (-1);
                    _CPParent.DayPremiumCDS = _DayNetPremiumCDS * (-1);

                    //changed on 20MAY2021 by Amey
                    _CPParent.FuturesMTM = _CLevelInfo.FuturesMTM;
                    _CPParent.OptionsMTM = _CLevelInfo.OptionsMTM;
                    _CPParent.EquityMTM = _CLevelInfo.EquityMTM;
                    _CPParent.MTM = _CLevelInfo.FuturesMTM + _CLevelInfo.OptionsMTM + _CLevelInfo.EquityMTM;

                    _CPParent.IntradayFuturesMTM = _CLevelInfo.IntradayFuturesMTM;
                    _CPParent.IntradayOptionsMTM = _CLevelInfo.IntradayOptionsMTM;
                    _CPParent.IntradayEquityMTM = _CLevelInfo.IntradayEquityMTM;
                    _CPParent.IntradayMTM = _CLevelInfo.IntradayFuturesMTM + _CLevelInfo.IntradayOptionsMTM + _CLevelInfo.IntradayEquityMTM;

                    _CPParent.CDSFuturesMTM = _CLevelInfo.CDSFuturesMTM;
                    _CPParent.CDSOptionsMTM = _CLevelInfo.CDSOptionsMTM;
                    _CPParent.CDSMTM = _CLevelInfo.CDSFuturesMTM + _CLevelInfo.CDSOptionsMTM;

                    _CPParent.CDSIntradayFuturesMTM = _CLevelInfo.CDSIntradayFuturesMTM;
                    _CPParent.CDSIntradayOptionsMTM = _CLevelInfo.CDSIntradayOptionsMTM;
                    _CPParent.CDSIntradayMTM = _CLevelInfo.CDSIntradayFuturesMTM + _CLevelInfo.CDSIntradayOptionsMTM;

                    // Added by Snehadri for banknifty and nifty Exposure 

                    var BNExpoOpt = ((_CLevelBankNiftyCEQty - _CLevelBankNiftyPEQty) * _CLevelBankNiftyClosePrice);                    
                    var NExpoOpt = ((_CLevelNiftyCEQty - _CLevelNiftyPEQty) * _CLevelNiftyClosePrice);
                   
                    _CPParent.BankniftyExpoOPT = DivideByBaseAndRound(BNExpoOpt, nameof(_CPParent.BankniftyExpoOPT));
                    _CPParent.NiftyExpoOPT = DivideByBaseAndRound(NExpoOpt, nameof(_CPParent.NiftyExpoOPT));
                    _CPParent.IntrinsicMTM = DivideByBaseAndRound(_CLevelInfo.IntrinsicMTM, nameof(_CPParent.MTM));
                    //added on 15APR2021 by Amey
                    _CPParent.TheoreticalMTM = _CLevelInfo.TheoreticalMTM;

                    //added on 20MAY2021 by Amey
                    _CPParent.ROV = _CLevelInfo.ROV;

                    //added on 08APR2021 by Amey
                    _CPParent.MonthlyMTM = _CPParent.MTM + ClientLevelMTD;

                    //added on 25MAR2021 by Amey
                    _CPParent.NPL = _CPParent.MTM + ClientLevelNPL;

                    //changed to TryGetValue on 27MAY2021 by Amey
                    var MarginUtil = 0.0;
                    if (CollectionHelper.dict_ClientWiseSpanInfo.TryGetValue(_ClientID, out ClientSpanInfo _ClientWiseSpanInfo))
                    {
                        //added DivideByBaseAndRound here to avoid incorrect Sum of values on 03MAY2021 by Amey

                        MarginUtil = _ClientWiseSpanInfo.MarginUtil;

                        //Changed by Akshay on 29-12-2020 For Avoiding Negative SpanMargin                        
                        _CPParent.Span = DivideByBaseAndRound(_ClientWiseSpanInfo.Span, nameof(_CPParent.Span));
                        _CPParent.Exposure = DivideByBaseAndRound(_ClientWiseSpanInfo.Exposure, nameof(_CPParent.Exposure));

                        double MarginUtiliaztion = DivideByBaseAndRound(MarginUtil, nameof(_CPParent.MarginUtil)); //Changed by Akshay on 29-12-2020 For Avoiding Negative SpanMargin

                        _CPParent.MarginUtil = MarginUtiliaztion;


                        //changed to TryGetValue on 27MAY2021 by Amey
                        if (CollectionHelper.dict_ExpiryMargin.TryGetValue(_ClientID, out double _EXMargin))
                        {
                            //Changed by Akshay on 15-01-2021 for Expiry Span
                            _CPParent.ExpiryMargin = DivideByBaseAndRound(_EXMargin, nameof(_CPParent.ExpiryMargin));
                        }

                        //changed to TryGetValue on 27MAY2021 by Amey
                        var EODMargin = 0.0;
                        if (CollectionHelper.dict_EODMargin.TryGetValue(_ClientID, out EODMargin))
                        {
                            //added on 10MAR2021 by Amey
                            _CPParent.EODMargin = DivideByBaseAndRound(EODMargin, nameof(_CPParent.EODMargin));
                        }

                        //changed on 07JAN2021 by Amey
                        _CPParent.MarginDifference = DivideByBaseAndRound(MarginUtil - EODMargin, nameof(_CPParent.MarginUtil));
                    }

                    if (CollectionHelper.dict_PeakMargin.TryGetValue(_ClientID, out double[] arr_PeakMargin))
                    {
                        if (arr_PeakMargin[0] > 0)
                        {
                            _CPParent.PeakMargin = DivideByBaseAndRound(arr_PeakMargin[0], nameof(_CPParent.PeakMargin));
                            _CPParent.PeakMarginTime = CommonFunctions.ConvertFromUnixTimestamp(arr_PeakMargin[1]).ToString("T");
                        }

                        if(arr_PeakMargin.Length > 2)
                        {
                            if (arr_PeakMargin[2] > 0)
                            {
                                _CPParent.TotalPeakMargin = DivideByBaseAndRound(arr_PeakMargin[2], nameof(_CPParent.TotalPeakMargin));
                                _CPParent.TotalPeakMarginTime = CommonFunctions.ConvertFromUnixTimestamp(arr_PeakMargin[3]).ToString("T");
                            }
                        }

                    }


                    if (CollectionHelper.dict_ClientWiseConsolidatedSpanInfo.TryGetValue(_ClientID, out ClientSpanInfo _ClientWiseConSpanInfo))
                    {
                        //added DivideByBaseAndRound here to avoid incorrect Sum of values on 03MAY2021 by Amey

                        var ConMarginUtil = _ClientWiseConSpanInfo.MarginUtil;

                        //Changed by Akshay on 29-12-2020 For Avoiding Negative SpanMargin
                        //modified on 17OCT2022 by Ninad for dividing by Base
                        _CPParent.SnapSpan = DivideByBaseAndRound(_ClientWiseConSpanInfo.Span, nameof(_CPParent.SnapSpan));
                        _CPParent.SnapExposure = DivideByBaseAndRound(_ClientWiseConSpanInfo.Exposure, nameof(_CPParent.SnapExposure));
                        _CPParent.SnapMarginUtil = DivideByBaseAndRound(ConMarginUtil, nameof(_CPParent.SnapMarginUtil)); //Changed by Akshay on 29-12-2020 For Avoiding Negative SpanMargin

                    }
                    //added on 22AUG2022 by Ninad
                    if (CollectionHelper.dict_ClientWiseConsolidatedExpMarginInfo.TryGetValue(_ClientID, out double ExpMargin))
                    {
                        _CPParent.SnapExpiryMargin = DivideByBaseAndRound(ExpMargin, nameof(_CPParent.SnapExpiryMargin));
                    }

                    if (CollectionHelper.dict_CDSClientWiseSpanInfo.TryGetValue(_ClientID, out ClientSpanInfo _CDSClientWiseSpanInfo))
                    {
                        MarginUtil = _CDSClientWiseSpanInfo.MarginUtil;

                        //Changed by Akshay on 29-12-2020 For Avoiding Negative SpanMargin                        
                        _CPParent.CDSSpan = DivideByBaseAndRound(_CDSClientWiseSpanInfo.Span, nameof(_CPParent.Span));
                        _CPParent.CDSExposure = DivideByBaseAndRound(_CDSClientWiseSpanInfo.Exposure, nameof(_CPParent.Exposure));

                        double MarginUtiliaztion = DivideByBaseAndRound(MarginUtil, nameof(_CPParent.MarginUtil)); //Changed by Akshay on 29-12-2020 For Avoiding Negative SpanMargin

                        //_CPParent.CDSPeakMargin = MarginUtiliaztion > _CPParent.CDSPeakMargin ? MarginUtiliaztion : _CPParent.CDSPeakMargin;
                        _CPParent.CDSMarginUtil = MarginUtiliaztion;

                        //changed to TryGetValue on 27MAY2021 by Amey
                        if (CollectionHelper.dict_CDSExpiryMargin.TryGetValue(_ClientID, out double _EXMargin))
                        {
                            //Changed by Akshay on 15-01-2021 for Expiry Span
                            _CPParent.CDSExpiryMargin = DivideByBaseAndRound(_EXMargin, nameof(_CPParent.ExpiryMargin));
                        }

                        //changed to TryGetValue on 27MAY2021 by Amey
                        var EODMargin = 0.0;
                        if (CollectionHelper.dict_CDSEODMargin.TryGetValue(_ClientID, out EODMargin))
                        {
                            //added on 10MAR2021 by Amey
                            _CPParent.CDSEODMargin = DivideByBaseAndRound(EODMargin, nameof(_CPParent.EODMargin));
                        }

                        //changed on 07JAN2021 by Amey
                        _CPParent.CDSMarginDifference = DivideByBaseAndRound(MarginUtil - EODMargin, nameof(_CPParent.MarginUtil));
                    }

                    if(CollectionHelper.dict_CDSPeakMargin.TryGetValue(_ClientID,out double[] arr_CDSPeakMargin))
                    {
                        _CPParent.CDSPeakMargin = arr_CDSPeakMargin[0];
                    }

                    //_CPParent.VARPeakMargin = _CPParent.VARMargin > _CPParent.VARPeakMargin ? _CPParent.VARMargin : _CPParent.VARPeakMargin;    //Added on 24JUN2021 by Akshay for VAR PeakMargin
                    // Changed by Snehadri on 10NOV2021 to capture the Time 
                    if (_CPParent.VARMargin > _CPParent.VARPeakMargin)
                    {
                        _CPParent.VARPeakMargin = _CPParent.VARMargin;
                        _CPParent.VarPeakMarginTime = DateTime.Now.ToString("T");
                    }
                    else
                    {
                        _CPParent.VARPeakMargin = _CPParent.VARPeakMargin;
                        _CPParent.VarPeakMarginTime = _CPParent.VarPeakMarginTime;
                    }

                    //added on 03MAY2021 by Amey
                    _CPParent.MarginAvailable = DivideByBaseAndRound((_CPParent.Ledger + _CPParent.Adhoc) - MarginUtil, nameof(_CPParent.MarginUtil));

                    _CPParent.AbsDelta = _CLevelInfo.SingleDelta;   //Added by Akshay on 21-12-2020
                    _CPParent.AbsGamma = _CLevelInfo.SingleGamma;   //Added by Akshay on 21-12-2020

                    _CPParent.Delta = _CLevelInfo.Delta;
                    _CPParent.Gamma = _CLevelInfo.Gamma;
                    _CPParent.Theta = _CLevelInfo.Theta;
                    _CPParent.Vega = _CLevelInfo.Vega;

                    //added on 29APR2021 by Amey
                    _CPParent.TV = _CLevelInfo.TimeValue;

                    //added on 3JUN2021 by Amey
                    _CPParent.ExpTheta = _CLevelInfo.ExpTheta;

                    //added on 24MAY2021 by Amey
                    _CPParent.DeltaAmount = _CLevelInfo.DeltaAmount;
                    _CPParent.Turnover = _CLevelInfo.Turnover;

                    _CPParent.CollateralQty = _CLevelInfo.CollateralQty;
                    _CPParent.CollateralValue = _CLevelInfo.CollateralValue;
                    _CPParent.CollateralHaircut = _CLevelInfo.CollateralHaircut;

                    if (CollectionHelper.dict_LimitInfo.TryGetValue(_ClientID, out LimitInfo limitInfo))
                    {
                        var MTMLimit = DivideByBaseAndRound(limitInfo.MTMLimit, nameof(CPParent.MTM));
                        var VARLimit = DivideByBaseAndRound(limitInfo.VARLimit, nameof(CPParent.VAR));
                        var MarginLimit = DivideByBaseAndRound(limitInfo.MarginLimit, nameof(CPParent.MarginUtil));
                        var bankniftyExpoLimit = limitInfo.BankniftyExpoLimit;
                        var niftyExpoLimit = limitInfo.NiftyExpoLimit;

                        if (MTMLimit != 0)
                            _CPParent.MTMLimitUtil = _CPParent.MTM / MTMLimit * 100;

                        if (VARLimit != 0)
                            _CPParent.VARLimitUtil = _CPParent.VAR / VARLimit * 100;

                        if (MarginLimit != 0)
                            _CPParent.MarginLimitUtil = _CPParent.MarginUtil / MarginLimit * 100;
                    }
                }
            }
            catch (Exception ee) { _logger.Error(ee); }
            isComputing = false;
        }

        #endregion

        #region Underlying View 

        internal nCompute(string _Underlying, UCParent _UCParent)
        {
            _logger = CollectionHelper._logger;

            this._Underlying = _Underlying;
            this._UCParent = _UCParent;

            //Added by Akshay on 25-03-2021
            ExpiryThreshHold = CollectionHelper.dict_DaysPercentage.Keys.Max();
        }

        internal void AddToUnderlyingQueue(List<ConsolidatedPositionInfo> list_ReceivedPositions, bool isUCParentExpanded, ConcurrentDictionary<string, double> dict_UnderlyingLevelNPL)
        {
            isComputing = true;

            this.isUCParentExpanded = isUCParentExpanded || !isInitialLoadUCClientsSuccess;
            this.dict_UnderlyingLevelNPL = dict_UnderlyingLevelNPL;

            Task.Run(() => ComputeAndUpdateUnderlyingView(list_ReceivedPositions));
        }

        private void ComputeAndUpdateUnderlyingView(List<ConsolidatedPositionInfo> list_ReceivedPositions)
        {
            try
            {
                //added on 11FEB2021 by Amey
                var dict_RecentPositions = list_ReceivedPositions.GroupBy(row => row.Username).ToDictionary(kvp => kvp.Key, kvp => kvp.ToList());

                var dict_OG = CollectionHelper.dict_OG;

                double CurrIV = 0, _SpotPrice = 0, _CurrMonthLTP = 0;

                //added on 11FEB2021 by Amey
                var _ULevelInfo = new UnderlyingLevelInfo();
                dict_ClientLevel.Clear();

                //Key : ClientID | Value : VaR
                var dict_OGVaR = new Dictionary<string, double>();

                bool UpdateUCParent = false;

                //Added by Akshay on 30-06-2021 for POS EXPO
                double ULevelPosExpoOPT = 0;
                double ULevelPosExpoFUT = 0;

                string Underlying = this._Underlying;

                foreach (var _ID in dict_RecentPositions.Keys)
                {
                    //Iterating on all ClientID Level Positions

                    bool _UpdateValues = false;

                    if (!dict_ClientLevel.ContainsKey(_ID))
                        dict_ClientLevel.Add(_ID, new ClientLevelInfo());


                    if (CollectionHelper.IsFullVAR)
                    {

                        int OGFrom = -10, OGTo = 10;
                        if (dict_OG.TryGetValue(_Underlying, out OGInfo _OGInfo))
                        {
                            OGFrom = _OGInfo.OGFrom;
                            OGTo = _OGInfo.OGTo;
                        }

                        for (int CurrOGIdx = OGFrom; CurrOGIdx <= OGTo; CurrOGIdx += 1)
                        {
                            //Iterating on OGRange

                            arr_VaRAtIV[0] = 0; arr_VaRAtIV[1] = 0;

                            for (int IVIdx = 0; IVIdx <= 1; IVIdx++)
                            {
                                //Iterating on Two types of IV

                                //This is to use values only once. Needed because the loops are running multiple times.
                                if (IVIdx == 0 && CurrOGIdx == OGFrom)
                                    _UpdateValues = true;
                                else
                                    _UpdateValues = false;

                                foreach (var _PositionInfo in dict_RecentPositions[_ID])
                                {
                                    //Iterating on every position under Underlying specified above

                                    //added on 20APR2021 by Amey
                                    var ScripKey = $"{_PositionInfo.Segment}|{_PositionInfo.Token}";

                                    ///changed position on 07APR2021 by Amey
                                    if (_PositionInfo.LTP <= 0 || _PositionInfo.UnderlyingLTP <= 0)
                                    {
                                        //If LTP is <= 0, means the LTP is not available yet. Therefore skip the Position.

                                        if (CollectionHelper.IsDebug)
                                            _logger.Debug($"NOLTP|{_ID}|{_PositionInfo.ScripName}|{_PositionInfo.Token}|{_PositionInfo.LTP}|{_PositionInfo.UnderlyingLTP}");

                                        continue;
                                    }

                                    if (_PositionInfo.ScripType != en_ScripType.EQ && _PositionInfo.ExpiryTimeSpan.TotalDays <= 0)
                                    {
                                        if (_UpdateValues)
                                        {
                                            //If Expired, dont compute other values excluding what mentioned below.

                                            double tFMTM = 0;
                                            double tOMTM = 0;
                                            double tEMTM = 0;


                                            double tIntradayFMTM = 0;
                                            double tIntradayOMTM = 0;
                                            double tIntradayEMTM = 0;

                                            //added on 24MAY2021 by Amey
                                            switch (_PositionInfo.ScripType)
                                            {
                                                case en_ScripType.EQ:
                                                    tEMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                    tIntradayEMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                    break;
                                                case en_ScripType.XX:
                                                    tFMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                    tIntradayFMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                    break;
                                                case en_ScripType.CE:
                                                case en_ScripType.PE:
                                                    tOMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                    tIntradayOMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                    break;
                                            }

                                            //changed on 11FEB2021 by Amey
                                            _ULevelInfo.FuturesMTM += tFMTM;
                                            _ULevelInfo.OptionsMTM += tOMTM;
                                            _ULevelInfo.EquityMTM += tEMTM;

                                            _ULevelInfo.IntradayFuturesMTM += tIntradayFMTM;
                                            _ULevelInfo.IntradayOptionsMTM += tIntradayOMTM;
                                            _ULevelInfo.IntradayEquityMTM += tIntradayEMTM;

                                            dict_ClientLevel[_ID].FuturesMTM += tFMTM;
                                            dict_ClientLevel[_ID].OptionsMTM += tOMTM;
                                            dict_ClientLevel[_ID].EquityMTM += tEMTM;

                                            dict_ClientLevel[_ID].IntradayFuturesMTM += tIntradayFMTM;
                                            dict_ClientLevel[_ID].IntradayOptionsMTM += tIntradayOMTM;
                                            dict_ClientLevel[_ID].IntradayEquityMTM += tIntradayEMTM;

                                            //Added by Akshay on 28-12-2021 for CDS
                                            double tCDSFMTM = 0;
                                            double tCDSOMTM = 0;

                                            double tCDSIntradayFMTM = 0;
                                            double tCDSIntradayOMTM = 0;

                                            //Added by Akshay on 28-12-2021 for CDS
                                            if (_PositionInfo.Segment == en_Segment.NSECD)
                                            {
                                                switch (_PositionInfo.ScripType)
                                                {
                                                    case en_ScripType.XX:
                                                        tCDSFMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                        tCDSIntradayFMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                        break;
                                                    case en_ScripType.CE:
                                                    case en_ScripType.PE:
                                                        tCDSOMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                        tCDSIntradayOMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                        break;
                                                }
                                            }

                                            //Added by Akshay on 28-12-2021
                                            _ULevelInfo.CDSFuturesMTM += tCDSFMTM;
                                            _ULevelInfo.CDSOptionsMTM += tCDSOMTM;

                                            _ULevelInfo.CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                            _ULevelInfo.CDSIntradayOptionsMTM += tCDSIntradayOMTM;

                                            dict_ClientLevel[_ID].CDSFuturesMTM += tCDSFMTM;
                                            dict_ClientLevel[_ID].CDSOptionsMTM += tCDSOMTM;

                                            dict_ClientLevel[_ID].CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                            dict_ClientLevel[_ID].CDSIntradayOptionsMTM += tCDSIntradayOMTM;


                                            //added on 15APR2021 by Amey
                                            double tTMTM = DivideByBaseAndRound(_PositionInfo.TheoreticalMTM, nameof(_CPParent.MTM));
                                            _ULevelInfo.TheoreticalMTM += tTMTM;
                                            dict_ClientLevel[_ID].TheoreticalMTM += tTMTM;
                                        }

                                        if (CollectionHelper.IsDebug)
                                            _logger.Debug($"Expired|{_ID}|{_PositionInfo.ScripName}|{_PositionInfo.ExpiryTimeSpan.TotalDays}");

                                        continue;
                                    }

                                    if (CurrOGIdx < 0)
                                    {
                                        //changed on 01FEB2021 by Amey
                                        var OGABS = Math.Abs(OGFrom);
                                        double IVChange = (_PositionInfo.IVHigher - _PositionInfo.IVMiddle) / (OGABS == 0 ? 1 : OGABS);

                                        if (IVIdx == 1)
                                            CurrIV = _PositionInfo.IVHigher - (IVChange * (OGABS - Math.Abs(CurrOGIdx)));
                                        else
                                            CurrIV = _PositionInfo.IVMiddle;
                                    }
                                    else if (CurrOGIdx >= 0)
                                    {
                                        //changed on 01FEB2021 by Amey
                                        var OGABS = Math.Abs(OGTo);
                                        double IVChange = (_PositionInfo.IVMiddle - _PositionInfo.IVLower) / (OGABS == 0 ? 1 : OGABS);

                                        if (IVIdx == 1)
                                            CurrIV = _PositionInfo.IVMiddle - (IVChange * Math.Abs(CurrOGIdx));
                                        else
                                            CurrIV = _PositionInfo.IVMiddle;
                                    }

                                    if (_UpdateValues)
                                    {
                                        //changed positon on 20APR2021 by Amey
                                        //Using this dictionary to display Total Unique Scrips at the top.
                                        CollectionHelper.dict_UniqueTokens.TryAdd(ScripKey, true);
                                        CollectionHelper.dict_UniqueClients.TryAdd(_ID, true);

                                        try
                                        {
                                        
                                            if (_PositionInfo.InstrumentName == en_InstrumentName.OPTSTK || _PositionInfo.InstrumentName == en_InstrumentName.FUTSTK)
                                            {
                                                if (!dict_ExpiryDays.ContainsKey(_PositionInfo.Expiry))
                                                    dict_ExpiryDays.Add(_PositionInfo.Expiry, CountDaysToExpiry(CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry), DateTime.Now));

                                                var DaysToExpiry = dict_ExpiryDays[_PositionInfo.Expiry];
                                                if (DaysToExpiry <= ExpiryThreshHold)
                                                {
                                                    if (_PositionInfo.SpotPrice > 0 && _PositionInfo.InstrumentName == en_InstrumentName.OPTSTK)
                                                    {

                                                        if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.StrikePrice < _PositionInfo.SpotPrice && _PositionInfo.NetPosition > 0)
                                                            dict_ClientLevel[_ID].CallBuyQty += _PositionInfo.NetPosition;
                                                        else if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.StrikePrice < _PositionInfo.SpotPrice && _PositionInfo.NetPosition < 0)
                                                            dict_ClientLevel[_ID].CallSellQty += _PositionInfo.NetPosition;
                                                        else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.StrikePrice > _PositionInfo.SpotPrice && _PositionInfo.NetPosition > 0)
                                                            dict_ClientLevel[_ID].PutBuyQty += _PositionInfo.NetPosition * -1;
                                                        else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.StrikePrice > _PositionInfo.SpotPrice && _PositionInfo.NetPosition < 0)
                                                            dict_ClientLevel[_ID].PutSellQty += _PositionInfo.NetPosition * -1;
                                                    }
                                                    else
                                                        dict_ClientLevel[_ID].FutQty += _PositionInfo.NetPosition;

                                                    dict_ClientLevel[_ID].Obligation += _PositionInfo.NetPosition;
                                                }

                                                //changed to TryGetValue on 27MAY2021 by Amey
                                                //changed to SpotPrice from UnderlyingLTP on 08APR2021 by Amey
                                                if (_PositionInfo.SpotPrice > 0 && CollectionHelper.dict_DaysPercentage.TryGetValue(DaysToExpiry, out double _DaysPercentage))
                                                    dict_ClientLevel[_ID].ValMargin = _PositionInfo.SpotPrice * _PositionInfo.UnderlyingVARMargin * _DaysPercentage / 100;
                                            }
                                        }
                                        catch (Exception ee) { _logger.Error(ee, _ID + "|DeliveryMargin Loop : " + _Underlying); }

                                        //Added by Akshay on 30-06-2021 for POS Expo
                                        try
                                        {
                                            //Added by Akshay on 29-06-2021 for POS EXPO
                                            if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.ExpiryTimeSpan.TotalDays > 1)
                                                dict_ClientLevel[_ID].NetCallQnty += _PositionInfo.NetPosition;

                                            //Added by Akshay on 29-06-2021 for POS EXPO
                                            else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.ExpiryTimeSpan.TotalDays > 1)
                                                dict_ClientLevel[_ID].NetPutQnty += _PositionInfo.NetPosition;

                                            //Added by Akshay on 29-06-2021 for POS EXPO
                                            else if (_PositionInfo.ScripType == en_ScripType.XX && _PositionInfo.ExpiryTimeSpan.TotalDays > 1)
                                                dict_ClientLevel[_ID].NetFutQnty += _PositionInfo.NetPosition;

                                            //Added by Akshay on 29-06-2021 for Net Qnty
                                            dict_ClientLevel[_ID].NetQnty += _PositionInfo.NetPosition;

                                            //Added by Akshay on 30-06-2021 for Closing Price
                                            dict_ClientLevel[_ID].ClosingPrice = _PositionInfo.ClosingPrice;

                                        }
                                        catch (Exception ee) { _logger.Error(ee, "|POS EXPO Loop : " + _ID); }



                                        //To Add or Update ClientPortfolio Child Row. added on 24NOV2020 by Amey
                                        try
                                        {
                                            DateTime tExpiry = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry);
                                            double tBEP = _PositionInfo.BEP;
                                            double tPriceCF = _PositionInfo.PriceCF;

                                            double tFMTM = 0;
                                            double tOMTM = 0;
                                            double tEMTM = 0;

                                            double tIntradayFMTM = 0;
                                            double tIntradayOMTM = 0;
                                            double tIntradayEMTM = 0;

                                            _SpotPrice = _PositionInfo.SpotPrice;
                                            if (_SpotPrice <= 0)
                                            {
                                                if (tExpiry.Month == DateTime.Now.Month)
                                                    _CurrMonthLTP = _PositionInfo.UnderlyingLTP;
                                            }

                                            //added on 24MAY2021 by Amey
                                            switch (_PositionInfo.ScripType)
                                            {
                                                case en_ScripType.EQ:
                                                    tEMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                    tIntradayEMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                    break;
                                                case en_ScripType.XX:
                                                    tFMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                    tIntradayFMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                    break;
                                                case en_ScripType.CE:
                                                case en_ScripType.PE:
                                                    tOMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                    tIntradayOMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                    break;
                                            }

                                            double tIntradayBEP = _PositionInfo.IntradayBEP;

                                            //Added on by Snehadri 29SEP2021
                                            double tTurnover = DivideByBaseAndRound((_PositionInfo.IntradayBuyQuantity * _PositionInfo.IntradayBuyAvg) + (_PositionInfo.IntradaySellQuantity * _PositionInfo.IntradaySellAvg), nameof(_CPParent.Turnover));
                                            _ULevelInfo.Turnover += tTurnover;
                                            dict_ClientLevel[_ID].Turnover += tTurnover;

                                            double tDelta = DivideByBaseAndRound(_PositionInfo.Delta * (CollectionHelper._ValueSigns.Delta), nameof(_CPParent.Delta));
                                            _ULevelInfo.Delta += tDelta;
                                            dict_ClientLevel[_ID].Delta += tDelta;

                                            double tGamma = DivideByBaseAndRound(_PositionInfo.Gamma * (CollectionHelper._ValueSigns.Gamma), nameof(_CPParent.Gamma));
                                            _ULevelInfo.Gamma += tGamma;
                                            dict_ClientLevel[_ID].Gamma += tGamma;

                                            double tTheta = DivideByBaseAndRound(_PositionInfo.Theta * (CollectionHelper._ValueSigns.Theta), nameof(_CPParent.Theta));
                                            _ULevelInfo.Theta += tTheta;
                                            dict_ClientLevel[_ID].Theta += tTheta;

                                            //added on 24MAY2021 by Amey
                                            double tDeltaAmt = DivideByBaseAndRound((_PositionInfo.Delta * _PositionInfo.UnderlyingLTP) * (CollectionHelper._ValueSigns.DeltaAmt), nameof(_CPParent.DeltaAmount));
                                            _ULevelInfo.DeltaAmount += tDeltaAmt;
                                            dict_ClientLevel[_ID].DeltaAmount += tDeltaAmt;

                                            double tVega = DivideByBaseAndRound(_PositionInfo.Vega * (CollectionHelper._ValueSigns.Vega), nameof(_CPParent.Vega));
                                            _ULevelInfo.Vega += tVega;
                                            dict_ClientLevel[_ID].Vega += tVega;

                                            //changed on 11FEB2021 by Amey
                                            _ULevelInfo.FuturesMTM += tFMTM;
                                            _ULevelInfo.OptionsMTM += tOMTM;
                                            _ULevelInfo.EquityMTM += tEMTM;

                                            _ULevelInfo.IntradayFuturesMTM += tIntradayFMTM;
                                            _ULevelInfo.IntradayOptionsMTM += tIntradayOMTM;
                                            _ULevelInfo.IntradayEquityMTM += tIntradayEMTM;

                                            dict_ClientLevel[_ID].FuturesMTM += tFMTM;
                                            dict_ClientLevel[_ID].OptionsMTM += tOMTM;
                                            dict_ClientLevel[_ID].EquityMTM += tEMTM;

                                            dict_ClientLevel[_ID].IntradayFuturesMTM += tIntradayFMTM;
                                            dict_ClientLevel[_ID].IntradayOptionsMTM += tIntradayOMTM;
                                            dict_ClientLevel[_ID].IntradayEquityMTM += tIntradayEMTM;

                                            //Added by Akshay on 28-12-2021 for CDS
                                            double tCDSFMTM = 0;
                                            double tCDSOMTM = 0;

                                            double tCDSIntradayFMTM = 0;
                                            double tCDSIntradayOMTM = 0;

                                            //Added by Akshay on 28-12-2021 for CDS
                                            if (_PositionInfo.Segment == en_Segment.NSECD)
                                            {
                                                switch (_PositionInfo.ScripType)
                                                {
                                                    case en_ScripType.XX:
                                                        tCDSFMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                        tCDSIntradayFMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                        break;
                                                    case en_ScripType.CE:
                                                    case en_ScripType.PE:
                                                        tCDSOMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                        tCDSIntradayOMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                        break;
                                                }
                                            }

                                            //Added by Akshay on 28-12-2021
                                            _ULevelInfo.CDSFuturesMTM += tCDSFMTM;
                                            _ULevelInfo.CDSOptionsMTM += tCDSOMTM;

                                            _ULevelInfo.CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                            _ULevelInfo.CDSIntradayOptionsMTM += tCDSIntradayOMTM;

                                            dict_ClientLevel[_ID].CDSFuturesMTM += tCDSFMTM;
                                            dict_ClientLevel[_ID].CDSOptionsMTM += tCDSOMTM;

                                            dict_ClientLevel[_ID].CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                            dict_ClientLevel[_ID].CDSIntradayOptionsMTM += tCDSIntradayOMTM;


                                            //added on 15APR2021 by Amey
                                            double tTMTM = DivideByBaseAndRound(_PositionInfo.TheoreticalMTM, nameof(_CPParent.MTM));
                                            _ULevelInfo.TheoreticalMTM += tTMTM;
                                            dict_ClientLevel[_ID].TheoreticalMTM += tTMTM;

                                            //changed position on 20MAY2021 by Amey
                                            //added on 11FEB2021 by Amey
                                            //Added by Akshay on 09-12-2020 for seperate EquityAmt
                                            if (_PositionInfo.ScripType == en_ScripType.EQ)
                                            {
                                                dict_ClientLevel[_ID].EquityAmount += DivideByBaseAndRound(_PositionInfo.EquityAmount, nameof(_CPParent.EquityAmount));

                                                //changed logic on 15APR2021 by Amey
                                                dict_ClientLevel[_ID].PayInPayOut += DivideByBaseAndRound(_PositionInfo.DayNetPremium, nameof(_CPParent.PayInPayOut));

                                                dict_ClientLevel[_ID].VARMargin += DivideByBaseAndRound(_PositionInfo.VARMargin, nameof(_CPParent.VARMargin));
                                                dict_ClientLevel[_ID].T1Quantity += _PositionInfo.T1Quantity;
                                                dict_ClientLevel[_ID].T2Quantity += _PositionInfo.T2Quantity;
                                                dict_ClientLevel[_ID].EarlyPayIn += _PositionInfo.EarlyPayIn;
                                            }
                                            else if (_PositionInfo.ScripType == en_ScripType.CE || _PositionInfo.ScripType == en_ScripType.PE)
                                            {
                                                //changed logic on 15APR2021 by Amey
                                                //Added by Akshay on 12-01-2021 for DayNet premium
                                              
                                                dict_ClientLevel[_ID].DayNetPremium += DivideByBaseAndRound(_PositionInfo.DayNetPremium, nameof(_CPParent.DayNetPremium));
                                                dict_ClientLevel[_ID].DayNetPremiumCDS += DivideByBaseAndRound(_PositionInfo.DayNetPremiumCDS, nameof(_CPParent.DayNetPremium));

                                            }
                                        }
                                        catch (Exception ee) { _logger.Error(ee, _ID + "|ComputeAndUpdate Loop : " + _Underlying); }
                                    }

                                    double CurrPosVaR = 0;
                                    double CalculatedLTP = 0;

                                    if (_PositionInfo.NetPosition == 0)
                                        CurrPosVaR = 0;
                                    else
                                    {
                                        if (_PositionInfo.ScripType == en_ScripType.EQ || _PositionInfo.ScripType == en_ScripType.XX)
                                            CalculatedLTP = (_PositionInfo.LTP * (100 + CurrOGIdx)) / 100;
                                        else if (_PositionInfo.ExpiryTimeSpan.TotalDays > 0 && (_PositionInfo.ScripType == en_ScripType.CE || _PositionInfo.ScripType == en_ScripType.PE))
                                        {
                                            if (_PositionInfo.ScripType == en_ScripType.CE && CurrIV > 0)
                                                CalculatedLTP = CommonFunctions.CallOption((_PositionInfo.UnderlyingLTP * (100 + CurrOGIdx)) / 100, _PositionInfo.StrikePrice, _PositionInfo.ExpiryTimeSpan.TotalDays / 365,
                                                    0, (CurrIV / 100), 0);
                                            else if (_PositionInfo.ScripType == en_ScripType.PE && CurrIV > 0)
                                                CalculatedLTP = CommonFunctions.PutOption((_PositionInfo.UnderlyingLTP * (100 + CurrOGIdx)) / 100, _PositionInfo.StrikePrice, _PositionInfo.ExpiryTimeSpan.TotalDays / 365,
                                                    0, (CurrIV / 100), 0);
                                        }

                                        if (_PositionInfo.NetPosition > 0)
                                            CurrPosVaR = (CalculatedLTP - _PositionInfo.BEP) * Math.Abs(_PositionInfo.NetPosition);
                                        else if (_PositionInfo.NetPosition < 0)
                                            CurrPosVaR = (_PositionInfo.BEP - CalculatedLTP) * Math.Abs(_PositionInfo.NetPosition);
                                    }

                                    arr_VaRAtIV[IVIdx] += CurrPosVaR;
                                }
                            }

                            var MaxVAR = arr_VaRAtIV[Array.IndexOf(arr_VaRAtIV, arr_VaRAtIV.Min())];
                            if (dict_OGVaR.ContainsKey(_ID))
                            {
                                if (dict_OGVaR[_ID] > MaxVAR)
                                    dict_OGVaR[_ID] = MaxVAR;
                            }
                            else
                                dict_OGVaR.Add(_ID, MaxVAR);
                        }

                    }

                    else if (!CollectionHelper.IsFullVAR)
                    {
                        var list_UpDownVarRange = CollectionHelper.list_UpDownVarRange.Where(c => c.Underlying == Underlying).FirstOrDefault();
                        if (list_UpDownVarRange is null)
                            list_UpDownVarRange = CollectionHelper.list_UpDownVarRange.Where(c => c.Underlying == "ALL").FirstOrDefault();

                        var ss_VarRange = list_UpDownVarRange.SS_UpDownVarRange;
                        var OGFrom = ss_VarRange.Min();
                        var OGTo = ss_VarRange.Max();

                        var iRange = 0;

                        foreach (var CurrOGIdx in ss_VarRange)
                        {
                            iRange += 1;

                            if (CurrOGIdx == OGFrom)
                                _UpdateValues = true;
                            else
                                _UpdateValues = false;

                            foreach (var _PositionInfo in dict_RecentPositions[_ID])
                            {
                                //Iterating on every position under Underlying specified above

                                //added on 20APR2021 by Amey
                                var ScripKey = $"{_PositionInfo.Segment}|{_PositionInfo.Token}";

                                ///changed position on 07APR2021 by Amey
                                if (_PositionInfo.LTP <= 0 || _PositionInfo.UnderlyingLTP <= 0)
                                {
                                    //If LTP is <= 0, means the LTP is not available yet. Therefore skip the Position.

                                    if (CollectionHelper.IsDebug)
                                        _logger.Debug($"NOLTP|{_ID}|{_PositionInfo.ScripName}|{_PositionInfo.Token}|{_PositionInfo.LTP}|{_PositionInfo.UnderlyingLTP}");

                                    continue;
                                }

                                if (_PositionInfo.ScripType != en_ScripType.EQ && _PositionInfo.ExpiryTimeSpan.TotalDays <= 0)
                                {
                                    if (_UpdateValues)
                                    {
                                        //If Expired, dont compute other values excluding what mentioned below.

                                        double tFMTM = 0;
                                        double tOMTM = 0;
                                        double tEMTM = 0;

                                        double tIntradayFMTM = 0;
                                        double tIntradayOMTM = 0;
                                        double tIntradayEMTM = 0;

                                        //added on 24MAY2021 by Amey
                                        switch (_PositionInfo.ScripType)
                                        {
                                            case en_ScripType.EQ:
                                                tEMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                tIntradayEMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                break;
                                            case en_ScripType.XX:
                                                tFMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                tIntradayFMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                break;
                                            case en_ScripType.CE:
                                            case en_ScripType.PE:
                                                tOMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                tIntradayOMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                break;
                                        }

                                        //changed on 11FEB2021 by Amey
                                        _ULevelInfo.FuturesMTM += tFMTM;
                                        _ULevelInfo.OptionsMTM += tOMTM;
                                        _ULevelInfo.EquityMTM += tEMTM;

                                        _ULevelInfo.IntradayFuturesMTM += tIntradayFMTM;
                                        _ULevelInfo.IntradayOptionsMTM += tIntradayOMTM;
                                        _ULevelInfo.IntradayEquityMTM += tIntradayEMTM;

                                        dict_ClientLevel[_ID].FuturesMTM += tFMTM;
                                        dict_ClientLevel[_ID].OptionsMTM += tOMTM;
                                        dict_ClientLevel[_ID].EquityMTM += tEMTM;

                                        dict_ClientLevel[_ID].IntradayFuturesMTM += tIntradayFMTM;
                                        dict_ClientLevel[_ID].IntradayOptionsMTM += tIntradayOMTM;
                                        dict_ClientLevel[_ID].IntradayEquityMTM += tIntradayEMTM;


                                        //Added by Akshay on 28-12-2021 for CDS
                                        double tCDSFMTM = 0;
                                        double tCDSOMTM = 0;

                                        double tCDSIntradayFMTM = 0;
                                        double tCDSIntradayOMTM = 0;

                                        //Added by Akshay on 28-12-2021 for CDS
                                        if (_PositionInfo.Segment == en_Segment.NSECD)
                                        {
                                            switch (_PositionInfo.ScripType)
                                            {
                                                case en_ScripType.XX:
                                                    tCDSFMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                    tCDSIntradayFMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                    break;
                                                case en_ScripType.CE:
                                                case en_ScripType.PE:
                                                    tCDSOMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                    tCDSIntradayOMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                    break;
                                            }
                                        }

                                        //Added by Akshay on 28-12-2021
                                        _ULevelInfo.CDSFuturesMTM += tCDSFMTM;
                                        _ULevelInfo.CDSOptionsMTM += tCDSOMTM;

                                        _ULevelInfo.CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                        _ULevelInfo.CDSIntradayOptionsMTM += tCDSIntradayOMTM;

                                        dict_ClientLevel[_ID].CDSFuturesMTM += tCDSFMTM;
                                        dict_ClientLevel[_ID].CDSOptionsMTM += tCDSOMTM;

                                        dict_ClientLevel[_ID].CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                        dict_ClientLevel[_ID].CDSIntradayOptionsMTM += tCDSIntradayOMTM;

                                        //added on 15APR2021 by Amey
                                        double tTMTM = DivideByBaseAndRound(_PositionInfo.TheoreticalMTM, nameof(_CPParent.MTM));
                                        _ULevelInfo.TheoreticalMTM += tTMTM;
                                        dict_ClientLevel[_ID].TheoreticalMTM += tTMTM;
                                    }

                                    if (CollectionHelper.IsDebug)
                                        _logger.Debug($"Expired|{_ID}|{_PositionInfo.ScripName}|{_PositionInfo.ExpiryTimeSpan.TotalDays}");

                                    continue;
                                }
                                CurrIV = _PositionInfo.IVMiddle;

                                if (_UpdateValues)
                                {
                                    //changed positon on 20APR2021 by Amey
                                    //Using this dictionary to display Total Unique Scrips at the top.
                                    CollectionHelper.dict_UniqueTokens.TryAdd(ScripKey, true);
                                    CollectionHelper.dict_UniqueClients.TryAdd(_ID, true);

                                    try
                                    {
                                        if (_PositionInfo.InstrumentName == en_InstrumentName.OPTSTK || _PositionInfo.InstrumentName == en_InstrumentName.FUTSTK)
                                        {
                                            if (!dict_ExpiryDays.ContainsKey(_PositionInfo.Expiry))
                                                dict_ExpiryDays.Add(_PositionInfo.Expiry, CountDaysToExpiry(CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry), DateTime.Now));

                                            var DaysToExpiry = dict_ExpiryDays[_PositionInfo.Expiry];
                                            if (DaysToExpiry <= ExpiryThreshHold)
                                            {
                                                if (_PositionInfo.SpotPrice > 0 && _PositionInfo.InstrumentName == en_InstrumentName.OPTSTK)
                                                {

                                                    if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.StrikePrice < _PositionInfo.SpotPrice && _PositionInfo.NetPosition > 0)
                                                        dict_ClientLevel[_ID].CallBuyQty += _PositionInfo.NetPosition;
                                                    else if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.StrikePrice < _PositionInfo.SpotPrice && _PositionInfo.NetPosition < 0)
                                                        dict_ClientLevel[_ID].CallSellQty += _PositionInfo.NetPosition;
                                                    else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.StrikePrice > _PositionInfo.SpotPrice && _PositionInfo.NetPosition > 0)
                                                        dict_ClientLevel[_ID].PutBuyQty += _PositionInfo.NetPosition * -1;
                                                    else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.StrikePrice > _PositionInfo.SpotPrice && _PositionInfo.NetPosition < 0)
                                                        dict_ClientLevel[_ID].PutSellQty += _PositionInfo.NetPosition * -1;
                                                }
                                                else
                                                    dict_ClientLevel[_ID].FutQty += _PositionInfo.NetPosition;

                                                dict_ClientLevel[_ID].Obligation += _PositionInfo.NetPosition;
                                            }

                                            //changed to TryGetValue on 27MAY2021 by Amey
                                            //changed to SpotPrice from UnderlyingLTP on 08APR2021 by Amey
                                            if (_PositionInfo.SpotPrice > 0 && CollectionHelper.dict_DaysPercentage.TryGetValue(DaysToExpiry, out double _DaysPercentage))
                                                dict_ClientLevel[_ID].ValMargin = _PositionInfo.SpotPrice * _PositionInfo.UnderlyingVARMargin * _DaysPercentage / 100;
                                        }
                                    }
                                    catch (Exception ee) { _logger.Error(ee, _ID + "|DeliveryMargin Loop : " + _Underlying); }

                                    //Added by Akshay on 30-06-2021 for POS Expo
                                    try
                                    {
                                        //Added by Akshay on 29-06-2021 for POS EXPO
                                        if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.ExpiryTimeSpan.TotalDays > 1)
                                            dict_ClientLevel[_ID].NetCallQnty += _PositionInfo.NetPosition;

                                        //Added by Akshay on 29-06-2021 for POS EXPO
                                        else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.ExpiryTimeSpan.TotalDays > 1)
                                            dict_ClientLevel[_ID].NetPutQnty += _PositionInfo.NetPosition;

                                        //Added by Akshay on 29-06-2021 for POS EXPO
                                        else if (_PositionInfo.ScripType == en_ScripType.XX && _PositionInfo.ExpiryTimeSpan.TotalDays > 1)
                                            dict_ClientLevel[_ID].NetFutQnty += _PositionInfo.NetPosition;

                                        //Added by Akshay on 29-06-2021 for Net Qnty
                                        dict_ClientLevel[_ID].NetQnty += _PositionInfo.NetPosition;

                                        //Added by Akshay on 30-06-2021 for Closing Price
                                        dict_ClientLevel[_ID].ClosingPrice = _PositionInfo.ClosingPrice;

                                    }
                                    catch (Exception ee) { _logger.Error(ee, "|POS EXPO Loop : " + _ID); }



                                    //To Add or Update ClientPortfolio Child Row. added on 24NOV2020 by Amey
                                    try
                                    {
                                        DateTime tExpiry = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry);
                                        double tBEP = _PositionInfo.BEP;
                                        double tPriceCF = _PositionInfo.PriceCF;

                                        double tFMTM = 0;
                                        double tOMTM = 0;
                                        double tEMTM = 0;

                                        double tIntradayFMTM = 0;
                                        double tIntradayOMTM = 0;
                                        double tIntradayEMTM = 0;

                                        _SpotPrice = _PositionInfo.SpotPrice;
                                        if (_SpotPrice <= 0)
                                        {
                                            if (tExpiry.Month == DateTime.Now.Month)
                                                _CurrMonthLTP = _PositionInfo.UnderlyingLTP;
                                        }

                                        //added on 24MAY2021 by Amey
                                        switch (_PositionInfo.ScripType)
                                        {
                                            case en_ScripType.EQ:
                                                tEMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                tIntradayEMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                break;
                                            case en_ScripType.XX:
                                                tFMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                tIntradayFMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                break;
                                            case en_ScripType.CE:
                                            case en_ScripType.PE:
                                                tOMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                                                tIntradayOMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                                                break;
                                        }

                                        double tIntradayBEP = _PositionInfo.IntradayBEP;

                                        //Added on by Snehadri 29SEP2021
                                        double tTurnover = DivideByBaseAndRound((_PositionInfo.IntradayBuyQuantity * _PositionInfo.IntradayBuyAvg) + (_PositionInfo.IntradaySellQuantity * _PositionInfo.IntradaySellAvg), nameof(_CPParent.Turnover));
                                        _ULevelInfo.Turnover += tTurnover;
                                        dict_ClientLevel[_ID].Turnover += tTurnover;

                                        double tDelta = DivideByBaseAndRound(_PositionInfo.Delta * (CollectionHelper._ValueSigns.Delta), nameof(_CPParent.Delta));
                                        _ULevelInfo.Delta += tDelta;
                                        dict_ClientLevel[_ID].Delta += tDelta;

                                        double tGamma = DivideByBaseAndRound(_PositionInfo.Gamma * (CollectionHelper._ValueSigns.Gamma), nameof(_CPParent.Gamma));
                                        _ULevelInfo.Gamma += tGamma;
                                        dict_ClientLevel[_ID].Gamma += tGamma;

                                        double tTheta = DivideByBaseAndRound(_PositionInfo.Theta * (CollectionHelper._ValueSigns.Theta), nameof(_CPParent.Theta));
                                        _ULevelInfo.Theta += tTheta;
                                        dict_ClientLevel[_ID].Theta += tTheta;

                                        //added on 24MAY2021 by Amey
                                        double tDeltaAmt = DivideByBaseAndRound((_PositionInfo.Delta * _PositionInfo.UnderlyingLTP) * (CollectionHelper._ValueSigns.DeltaAmt), nameof(_CPParent.DeltaAmount));
                                        _ULevelInfo.DeltaAmount += tDeltaAmt;
                                        dict_ClientLevel[_ID].DeltaAmount += tDeltaAmt;

                                        double tVega = DivideByBaseAndRound(_PositionInfo.Vega * (CollectionHelper._ValueSigns.Vega), nameof(_CPParent.Vega));
                                        _ULevelInfo.Vega += tVega;
                                        dict_ClientLevel[_ID].Vega += tVega;

                                        //changed on 11FEB2021 by Amey
                                        _ULevelInfo.FuturesMTM += tFMTM;
                                        _ULevelInfo.OptionsMTM += tOMTM;
                                        _ULevelInfo.EquityMTM += tEMTM;

                                        _ULevelInfo.IntradayFuturesMTM += tIntradayFMTM;
                                        _ULevelInfo.IntradayOptionsMTM += tIntradayOMTM;
                                        _ULevelInfo.IntradayEquityMTM += tIntradayEMTM;

                                        dict_ClientLevel[_ID].FuturesMTM += tFMTM;
                                        dict_ClientLevel[_ID].OptionsMTM += tOMTM;
                                        dict_ClientLevel[_ID].EquityMTM += tEMTM;

                                        dict_ClientLevel[_ID].IntradayFuturesMTM += tIntradayFMTM;
                                        dict_ClientLevel[_ID].IntradayOptionsMTM += tIntradayOMTM;
                                        dict_ClientLevel[_ID].IntradayEquityMTM += tIntradayEMTM;

                                        //Added by Akshay on 28-12-2021 for CDS
                                        double tCDSFMTM = 0;
                                        double tCDSOMTM = 0;

                                        double tCDSIntradayFMTM = 0;
                                        double tCDSIntradayOMTM = 0;

                                        //Added by Akshay on 28-12-2021 for CDS
                                        if (_PositionInfo.Segment == en_Segment.NSECD)
                                        {
                                            switch (_PositionInfo.ScripType)
                                            {
                                                case en_ScripType.XX:
                                                    tCDSFMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                    tCDSIntradayFMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                    break;
                                                case en_ScripType.CE:
                                                case en_ScripType.PE:
                                                    tCDSOMTM = DivideByBaseAndRound(_PositionInfo.CDSMTM, nameof(_CPParent.CDSMTM));
                                                    tCDSIntradayOMTM = DivideByBaseAndRound(_PositionInfo.CDSIntradayMTM, nameof(_CPParent.CDSIntradayMTM));
                                                    break;
                                            }
                                        }

                                        //Added by Akshay on 28-12-2021
                                        _ULevelInfo.CDSFuturesMTM += tCDSFMTM;
                                        _ULevelInfo.CDSOptionsMTM += tCDSOMTM;

                                        _ULevelInfo.CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                        _ULevelInfo.CDSIntradayOptionsMTM += tCDSIntradayOMTM;

                                        dict_ClientLevel[_ID].CDSFuturesMTM += tCDSFMTM;
                                        dict_ClientLevel[_ID].CDSOptionsMTM += tCDSOMTM;

                                        dict_ClientLevel[_ID].CDSIntradayFuturesMTM += tCDSIntradayFMTM;
                                        dict_ClientLevel[_ID].CDSIntradayOptionsMTM += tCDSIntradayOMTM;


                                        //added on 15APR2021 by Amey
                                        double tTMTM = DivideByBaseAndRound(_PositionInfo.TheoreticalMTM, nameof(_CPParent.MTM));
                                        _ULevelInfo.TheoreticalMTM += tTMTM;
                                        dict_ClientLevel[_ID].TheoreticalMTM += tTMTM;

                                        //changed position on 20MAY2021 by Amey
                                        //added on 11FEB2021 by Amey
                                        //Added by Akshay on 09-12-2020 for seperate EquityAmt
                                        if (_PositionInfo.ScripType == en_ScripType.EQ)
                                        {
                                            dict_ClientLevel[_ID].EquityAmount += DivideByBaseAndRound(_PositionInfo.EquityAmount, nameof(_CPParent.EquityAmount));

                                            //changed logic on 15APR2021 by Amey
                                            dict_ClientLevel[_ID].PayInPayOut += DivideByBaseAndRound(_PositionInfo.DayNetPremium, nameof(_CPParent.PayInPayOut));

                                            dict_ClientLevel[_ID].VARMargin += DivideByBaseAndRound(_PositionInfo.VARMargin, nameof(_CPParent.VARMargin));

                                            dict_ClientLevel[_ID].T1Quantity += _PositionInfo.T1Quantity;
                                            dict_ClientLevel[_ID].T2Quantity += _PositionInfo.T2Quantity;
                                            dict_ClientLevel[_ID].EarlyPayIn += _PositionInfo.EarlyPayIn;
                                        }
                                        else if (_PositionInfo.ScripType == en_ScripType.CE || _PositionInfo.ScripType == en_ScripType.PE)
                                        {
                                            //changed logic on 15APR2021 by Amey
                                            //Added by Akshay on 12-01-2021 for DayNet premium

                                            dict_ClientLevel[_ID].DayNetPremium += DivideByBaseAndRound(_PositionInfo.DayNetPremium, nameof(_CPParent.DayNetPremium));
                                            dict_ClientLevel[_ID].DayNetPremiumCDS += DivideByBaseAndRound(_PositionInfo.DayNetPremiumCDS, nameof(_CPParent.DayNetPremium));

                                        }
                                    }
                                    catch (Exception ee) { _logger.Error(ee, _ID + "|ComputeAndUpdate Loop : " + _Underlying); }
                                }

                                double CurrPosVaR = 0;
                                double CalculatedLTP = 0;

                                if (_PositionInfo.NetPosition == 0)
                                    CurrPosVaR = 0;
                                else
                                {
                                    if (_PositionInfo.ScripType == en_ScripType.EQ || _PositionInfo.ScripType == en_ScripType.XX)
                                        CalculatedLTP = (_PositionInfo.LTP * (100 + CurrOGIdx)) / 100;
                                    else if (_PositionInfo.ExpiryTimeSpan.TotalDays > 0 && (_PositionInfo.ScripType == en_ScripType.CE || _PositionInfo.ScripType == en_ScripType.PE))
                                    {
                                        if (_PositionInfo.ScripType == en_ScripType.CE && CurrIV > 0)
                                            CalculatedLTP = CommonFunctions.CallOption((_PositionInfo.UnderlyingLTP * (100 + CurrOGIdx)) / 100, _PositionInfo.StrikePrice, _PositionInfo.ExpiryTimeSpan.TotalDays / 365,
                                                0, (CurrIV / 100), 0);
                                        else if (_PositionInfo.ScripType == en_ScripType.PE && CurrIV > 0)
                                            CalculatedLTP = CommonFunctions.PutOption((_PositionInfo.UnderlyingLTP * (100 + CurrOGIdx)) / 100, _PositionInfo.StrikePrice, _PositionInfo.ExpiryTimeSpan.TotalDays / 365,
                                                0, (CurrIV / 100), 0);
                                    }

                                    if (_PositionInfo.NetPosition > 0)
                                        CurrPosVaR = (CalculatedLTP - _PositionInfo.BEP) * Math.Abs(_PositionInfo.NetPosition);
                                    else if (_PositionInfo.NetPosition < 0)
                                        CurrPosVaR = (_PositionInfo.BEP - CalculatedLTP) * Math.Abs(_PositionInfo.NetPosition);
                                }

                                // Added by Snehadri on 21OCT2021 for UpsideDownVaR in Underlying Tab
                                switch (iRange)
                                {
                                    case 1:
                                        _ULevelInfo.Scenario1 += DivideByBaseAndRound(CurrPosVaR * (CollectionHelper._ValueSigns.VaR), nameof(_CPParent.VAR));
                                        dict_ClientLevel[_ID].Scenario1 += DivideByBaseAndRound(CurrPosVaR * (CollectionHelper._ValueSigns.VaR), nameof(_CPParent.VAR));
                                        break;
                                    case 2:
                                        _ULevelInfo.Scenario2 += DivideByBaseAndRound(CurrPosVaR * (CollectionHelper._ValueSigns.VaR), nameof(_CPParent.VAR));
                                        dict_ClientLevel[_ID].Scenario2 += DivideByBaseAndRound(CurrPosVaR * (CollectionHelper._ValueSigns.VaR), nameof(_CPParent.VAR));
                                        break;
                                    case 3:
                                        _ULevelInfo.Scenario3 += DivideByBaseAndRound(CurrPosVaR * (CollectionHelper._ValueSigns.VaR), nameof(_CPParent.VAR));
                                        dict_ClientLevel[_ID].Scenario3 += DivideByBaseAndRound(CurrPosVaR * (CollectionHelper._ValueSigns.VaR), nameof(_CPParent.VAR));
                                        break;
                                    case 4:
                                        _ULevelInfo.Scenario4 += DivideByBaseAndRound(CurrPosVaR * (CollectionHelper._ValueSigns.VaR), nameof(_CPParent.VAR));
                                        dict_ClientLevel[_ID].Scenario4 += DivideByBaseAndRound(CurrPosVaR * (CollectionHelper._ValueSigns.VaR), nameof(_CPParent.VAR));
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }

                if (CollectionHelper.IsFullVAR)
                {
                    foreach (var _ID in dict_OGVaR.Keys)
                    {
                        UpdateUCParent = true;

                        var MaxVarCalculated = DivideByBaseAndRound(dict_OGVaR[_ID] * (CollectionHelper._ValueSigns.VaR), nameof(_CPParent.VAR));
                        _ULevelInfo.VaR += MaxVarCalculated;

                        var _ClientLevelInfo = dict_ClientLevel[_ID];


                        long ITMQty = _ClientLevelInfo.CallBuyQty + Math.Abs(_ClientLevelInfo.PutBuyQty);
                        long DeliveryQty = _ClientLevelInfo.CallBuyQty + _ClientLevelInfo.CallSellQty + _ClientLevelInfo.PutBuyQty + _ClientLevelInfo.PutSellQty + _ClientLevelInfo.FutQty;
                        long DeliveryMargin = Math.Min(ITMQty, Math.Abs(DeliveryQty));


                        var _DeliveryMargin = DivideByBaseAndRound(Math.Abs(DeliveryMargin) * _ClientLevelInfo.ValMargin, nameof(_CPParent.DeliveryMargin));


                        _ULevelInfo.NetposDelMargin += Math.Abs(DeliveryMargin);
                        _ULevelInfo.Obligation += Math.Abs(DeliveryQty);

                        //Added by Akshay on 29-06-2021
                        var _CPUnderlying = new CPUnderlying();

                        var PosExpoOPT = DivideByBaseAndRound((_ClientLevelInfo.NetCallQnty - _ClientLevelInfo.NetPutQnty) * _ClientLevelInfo.ClosingPrice, nameof(_CPUnderlying.PosExpoOPT));
                        var PosExpoFUT = DivideByBaseAndRound(_ClientLevelInfo.NetFutQnty * _ClientLevelInfo.ClosingPrice, nameof(_CPUnderlying.PosExpoFUT));
                        var NetQnty = _ClientLevelInfo.NetQnty;

                        ULevelPosExpoOPT += Math.Abs(PosExpoOPT);
                        ULevelPosExpoFUT += Math.Abs(PosExpoFUT);

                        string Key = _ID + "_" + _Underlying;

                        //changed to TryGetValue on 27MAY2021 by Amey
                        //changed on 16FEB2021 by Amey
                        if (CollectionHelper.dict_ClientUnderlyingWiseSpanInfo.TryGetValue(Key, out UnderlyingSpanInfo _UnderlyingSpanInfo))
                        {
                            _ClientLevelInfo.Span = _UnderlyingSpanInfo.Span;
                            _ClientLevelInfo.Exposure = _UnderlyingSpanInfo.Exposure;
                            _ClientLevelInfo.MarginUtil = _UnderlyingSpanInfo.MarginUtil;

                            _ULevelInfo.Span += _UnderlyingSpanInfo.Span;
                            _ULevelInfo.Exposure += _UnderlyingSpanInfo.Exposure;
                        }
                        else if (CollectionHelper.dict_CDSClientUnderlyingWiseSpanInfo.TryGetValue(Key, out UnderlyingSpanInfo _CDSUnderlyingSpanInfo))
                        {
                            _ClientLevelInfo.Span = _CDSUnderlyingSpanInfo.Span;
                            _ClientLevelInfo.Exposure = _CDSUnderlyingSpanInfo.Exposure;
                            _ClientLevelInfo.MarginUtil = _CDSUnderlyingSpanInfo.MarginUtil;

                            _ULevelInfo.Span += _CDSUnderlyingSpanInfo.Span;
                            _ULevelInfo.Exposure += _CDSUnderlyingSpanInfo.Exposure;
                        }

                        if (CollectionHelper.dict_ClientUnderlyingWiseConsolidatedSpanInfo.TryGetValue(Key, out UnderlyingSpanInfo _UnderlyingConSpanInfo))
                        {
                            _ClientLevelInfo.SnapSpan = _UnderlyingConSpanInfo.Span;
                            _ClientLevelInfo.SnapExposure = _UnderlyingConSpanInfo.Exposure;
                            _ClientLevelInfo.SnapMarginUtil = _UnderlyingConSpanInfo.MarginUtil;

                            _ULevelInfo.SnapSpan += _UnderlyingConSpanInfo.Span;
                            _ULevelInfo.SnapExposure += _UnderlyingConSpanInfo.Exposure;
                        }

                        if (isUCParentExpanded)
                        {
                            UCClient _UCClient = null;

                            //added on 18FEB2021 by Amey
                            lock (CollectionHelper._CPLock)
                            {
                                _UCClient = _UCParent.bList_Clients.Where(v => v.ClientID == _ID).FirstOrDefault();
                            }

                            CollectionHelper.gc_UC.Invoke((MethodInvoker)(() =>
                            {
                                if (_UCClient is null)
                                {
                                    _UCClient = new UCClient();
                                    _UCClient.ClientID = _ID;

                                    if (CollectionHelper.dict_ClientInfo.TryGetValue(_ID, out ClientInfo _ClientInfo))
                                    {
                                        _UCClient.Name = _ClientInfo.Name;
                                        _UCClient.Ledger = _ClientInfo.ELM;
                                        _UCClient.Adhoc = _ClientInfo.AdHoc;
                                        _UCClient.Zone = _ClientInfo.Zone;
                                        _UCClient.Branch = _ClientInfo.Branch;
                                        _UCClient.Family = _ClientInfo.Family;
                                        _UCClient.Product = _ClientInfo.Product;
                                    }

                                    _UCParent.bList_Clients.Add(_UCClient);

                                    //added on 08MAR2021 by Amey
                                    isInitialLoadUCClientsSuccess = true;
                                }

                                if (CollectionHelper.dict_ClientInfo.TryGetValue(_ID, out ClientInfo _CInfo))
                                {
                                    _UCClient.Name = _CInfo.Name;
                                    _UCClient.Ledger = _CInfo.ELM;
                                    _UCClient.Adhoc = _CInfo.AdHoc;
                                    _UCClient.Zone = _CInfo.Zone;
                                    _UCClient.Branch = _CInfo.Branch;
                                    _UCClient.Family = _CInfo.Family;
                                    _UCClient.Product = _CInfo.Product;
                                }

                                //added on 26MAY2021 by Amey
                                //_UCClient.FuturesMTM = _ClientLevelInfo.FuturesMTM;
                                //_UCClient.OptionsMTM = _ClientLevelInfo.OptionsMTM;
                                //_UCClient.EquityMTM = _ClientLevelInfo.EquityMTM;

                                //_UCClient.MTM = _ClientLevelInfo.FuturesMTM + _ClientLevelInfo.OptionsMTM + _ClientLevelInfo.EquityMTM;

                                _UCClient.FuturesMTM = _ClientLevelInfo.FuturesMTM + _ClientLevelInfo.CDSFuturesMTM ;
                                _UCClient.OptionsMTM = _ClientLevelInfo.OptionsMTM + _ClientLevelInfo.CDSOptionsMTM ;
                                _UCClient.EquityMTM = _ClientLevelInfo.EquityMTM;

                                _UCClient.MTM = _ClientLevelInfo.FuturesMTM + _ClientLevelInfo.OptionsMTM + _ClientLevelInfo.EquityMTM + _ClientLevelInfo.CDSFuturesMTM + _ClientLevelInfo.CDSOptionsMTM ;
                                _UCClient.IntradayFuturesMTM = _ClientLevelInfo.IntradayFuturesMTM + _ClientLevelInfo.CDSIntradayFuturesMTM ;
                                _UCClient.IntradayOptionsMTM = _ClientLevelInfo.IntradayOptionsMTM + _ClientLevelInfo.CDSIntradayOptionsMTM ;
                                _UCClient.IntradayEquityMTM = _ClientLevelInfo.IntradayEquityMTM;

                                //added on 15APR2021 by Amey
                                _UCClient.IntradayMTM = _ClientLevelInfo.IntradayFuturesMTM + _ClientLevelInfo.IntradayOptionsMTM + _ClientLevelInfo.IntradayEquityMTM + _ClientLevelInfo.CDSIntradayFuturesMTM + _ClientLevelInfo.CDSIntradayOptionsMTM ;


                                //Added by Akshay on 23-07-2021 for NPL
                                var UNPLKey = $"{_ID}^{_Underlying}";
                                var UNPL = _UCClient.MTM;
                                if (CollectionHelper.dict_NPLValues.TryGetValue(UNPLKey, out double _NPL))
                                    UNPL = _UCClient.MTM + DivideByBaseAndRound(_NPL, nameof(_CPParent.MTM));

                                _UCClient.NPL = UNPL;

                                ////added on 28MAY2021 by Amey
                                //_UCClient.IntradayFuturesMTM = _ClientLevelInfo.IntradayFuturesMTM;
                                //_UCClient.IntradayOptionsMTM = _ClientLevelInfo.IntradayOptionsMTM;
                                //_UCClient.IntradayEquityMTM = _ClientLevelInfo.IntradayEquityMTM;

                                //added on 15APR2021 by Amey
                                //_UCClient.IntradayMTM = _ClientLevelInfo.IntradayFuturesMTM + _ClientLevelInfo.IntradayOptionsMTM + _ClientLevelInfo.IntradayEquityMTM;
                                
                                
                                
                                _UCClient.TheoreticalMTM = _ClientLevelInfo.TheoreticalMTM;

                                _UCClient.VAR = MaxVarCalculated;
                                // Added by Snehadri on 21OCT2021 for UpsideDownVaR in Underlying Tab
                                _UCClient.Scenario1 = 0;
                                _UCClient.Scenario2 = 0;
                                _UCClient.Scenario3 = 0;
                                _UCClient.Scenario4 = 0;

                                _UCClient.Span = _ClientLevelInfo.Span;
                                _UCClient.Exposure = _ClientLevelInfo.Exposure;
                                _UCClient.MarginUtil = _ClientLevelInfo.MarginUtil;

                                _UCClient.SnapSpan = _ClientLevelInfo.SnapSpan;
                                _UCClient.SnapExposure = _ClientLevelInfo.SnapExposure;
                                _UCClient.SnapMarginUtil = _ClientLevelInfo.SnapMarginUtil;

                                _UCClient.Delta = _ClientLevelInfo.Delta;
                                _UCClient.Theta = _ClientLevelInfo.Theta;
                                _UCClient.Gamma = _ClientLevelInfo.Gamma;
                                _UCClient.Vega = _ClientLevelInfo.Vega;

                                //added on 24MAY2021 by Amey
                                _UCClient.DeltaAmount = _ClientLevelInfo.DeltaAmount;

                                _UCClient.EquityAmount = _ClientLevelInfo.EquityAmount;
                                _UCClient.PayInPayOut = _ClientLevelInfo.PayInPayOut;
                                _UCClient.DayNetPremium = _ClientLevelInfo.DayNetPremium;
                                _UCClient.VARMargin = _ClientLevelInfo.VARMargin;

                                _UCClient.DayNetPremiumCDS = _ClientLevelInfo.DayNetPremiumCDS;

                                //added by nikhil
                                _UCClient.T1Quantity = _ClientLevelInfo.T1Quantity;
                                _UCClient.T2Quantity = _ClientLevelInfo.T2Quantity;
                                _UCClient.EarlyPayIn = _ClientLevelInfo.EarlyPayIn;

                                _UCClient.DeliveryMargin = _DeliveryMargin;

                                //Added by Akshay on 29-06-2021
                                _UCClient.PosExpoOPT = PosExpoOPT;
                                _UCClient.PosExpoFUT = PosExpoFUT;
                                _UCClient.NetQnty = NetQnty;

                            }));
                        }
                    }
                    // Added by Snehadri on 21OCT2021 for UpsideDownVaR in Underlying Tab
                    _ULevelInfo.Scenario1 = 0.0;
                    _ULevelInfo.Scenario2 = 0.0;
                    _ULevelInfo.Scenario3 = 0.0;
                    _ULevelInfo.Scenario4 = 0.0;
                }

                else if (!CollectionHelper.IsFullVAR)
                {
                    foreach (var _ID in dict_ClientLevel.Keys)
                    {
                        UpdateUCParent = true;


                        _ULevelInfo.VaR = 0;

                        var _ClientLevelInfo = dict_ClientLevel[_ID];

                        long ITMQty = _ClientLevelInfo.CallBuyQty + Math.Abs(_ClientLevelInfo.PutBuyQty);
                        long DeliveryQty = _ClientLevelInfo.CallBuyQty + _ClientLevelInfo.CallSellQty + _ClientLevelInfo.PutBuyQty + _ClientLevelInfo.PutSellQty + _ClientLevelInfo.FutQty;
                        long DeliveryMargin = Math.Min(ITMQty, Math.Abs(DeliveryQty));

                        var _DeliveryMargin = DivideByBaseAndRound(Math.Abs(DeliveryMargin) * _ClientLevelInfo.ValMargin, nameof(_CPParent.DeliveryMargin));


                        _ULevelInfo.NetposDelMargin += Math.Abs(DeliveryMargin);
                        _ULevelInfo.Obligation += Math.Abs(DeliveryQty);

                        //Added by Akshay on 29-06-2021
                        var _CPUnderlying = new CPUnderlying();

                        var PosExpoOPT = DivideByBaseAndRound((_ClientLevelInfo.NetCallQnty - _ClientLevelInfo.NetPutQnty) * _ClientLevelInfo.ClosingPrice, nameof(_CPUnderlying.PosExpoOPT));
                        var PosExpoFUT = DivideByBaseAndRound(_ClientLevelInfo.NetFutQnty * _ClientLevelInfo.ClosingPrice, nameof(_CPUnderlying.PosExpoFUT));
                        var NetQnty = _ClientLevelInfo.NetQnty;

                        ULevelPosExpoOPT += Math.Abs(PosExpoOPT);
                        ULevelPosExpoFUT += Math.Abs(PosExpoFUT);

                        string Key = _ID + "_" + _Underlying;

                        //changed to TryGetValue on 27MAY2021 by Amey
                        //changed on 16FEB2021 by Amey
                        if (CollectionHelper.dict_ClientUnderlyingWiseSpanInfo.TryGetValue(Key, out UnderlyingSpanInfo _UnderlyingSpanInfo))
                        {
                            _ClientLevelInfo.Span = _UnderlyingSpanInfo.Span;
                            _ClientLevelInfo.Exposure = _UnderlyingSpanInfo.Exposure;
                            _ClientLevelInfo.MarginUtil = _UnderlyingSpanInfo.MarginUtil;

                            _ULevelInfo.Span += _UnderlyingSpanInfo.Span;
                            _ULevelInfo.Exposure += _UnderlyingSpanInfo.Exposure;
                        }
                        else if (CollectionHelper.dict_CDSClientUnderlyingWiseSpanInfo.TryGetValue(Key, out UnderlyingSpanInfo _CDSUnderlyingSpanInfo))
                        {
                            _ClientLevelInfo.Span = _CDSUnderlyingSpanInfo.Span;
                            _ClientLevelInfo.Exposure = _CDSUnderlyingSpanInfo.Exposure;
                            _ClientLevelInfo.MarginUtil = _CDSUnderlyingSpanInfo.MarginUtil;

                            _ULevelInfo.Span += _CDSUnderlyingSpanInfo.Span;
                            _ULevelInfo.Exposure += _CDSUnderlyingSpanInfo.Exposure;
                        }


                        if (CollectionHelper.dict_ClientUnderlyingWiseConsolidatedSpanInfo.TryGetValue(Key, out UnderlyingSpanInfo _UnderlyingConSpanInfo))
                        {
                            _ClientLevelInfo.SnapSpan = _UnderlyingConSpanInfo.Span;
                            _ClientLevelInfo.SnapExposure = _UnderlyingConSpanInfo.Exposure;
                            _ClientLevelInfo.SnapMarginUtil = _UnderlyingConSpanInfo.MarginUtil;

                            _ULevelInfo.SnapSpan += _UnderlyingConSpanInfo.Span;
                            _ULevelInfo.SnapExposure += _UnderlyingConSpanInfo.Exposure;
                        }

                        if (isUCParentExpanded)
                        {
                            UCClient _UCClient = null;

                            //added on 18FEB2021 by Amey
                            lock (CollectionHelper._CPLock)
                            {
                                _UCClient = _UCParent.bList_Clients.Where(v => v.ClientID == _ID).FirstOrDefault();
                            }

                            CollectionHelper.gc_UC.Invoke((MethodInvoker)(() =>
                            {
                                if (_UCClient is null)
                                {
                                    _UCClient = new UCClient();
                                    _UCClient.ClientID = _ID;

                                    if (CollectionHelper.dict_ClientInfo.TryGetValue(_ID, out ClientInfo _ClientInfo))
                                    {
                                        _UCClient.Name = _ClientInfo.Name;
                                        _UCClient.Ledger = _ClientInfo.ELM;
                                        _UCClient.Adhoc = _ClientInfo.AdHoc;
                                        _UCClient.Zone = _ClientInfo.Zone;
                                        _UCClient.Branch = _ClientInfo.Branch;
                                        _UCClient.Family = _ClientInfo.Family;
                                        _UCClient.Product = _ClientInfo.Product;
                                    }

                                    _UCParent.bList_Clients.Add(_UCClient);

                                    //added on 08MAR2021 by Amey
                                    isInitialLoadUCClientsSuccess = true;
                                }

                                if (CollectionHelper.dict_ClientInfo.TryGetValue(_ID, out ClientInfo _CInfo))
                                {
                                    _UCClient.Name = _CInfo.Name;
                                    _UCClient.Ledger = _CInfo.ELM;
                                    _UCClient.Adhoc = _CInfo.AdHoc;
                                    _UCClient.Zone = _CInfo.Zone;
                                    _UCClient.Branch = _CInfo.Branch;
                                    _UCClient.Family = _CInfo.Family;
                                    _UCClient.Product = _CInfo.Product;
                                }

                                //if (CollectionHelper.dict_ClientInfo.TryGetValue(_ID, out ClientInfo _CInfo))
                                //{
                                //    _UCClient.Ledger = _CInfo.ELM;
                                //}
                                //added on 26MAY2021 by Amey
                                //_UCClient.FuturesMTM = _ClientLevelInfo.FuturesMTM;
                                //_UCClient.OptionsMTM = _ClientLevelInfo.OptionsMTM;
                                //_UCClient.EquityMTM = _ClientLevelInfo.EquityMTM;

                                //_UCClient.MTM = _ClientLevelInfo.FuturesMTM + _ClientLevelInfo.OptionsMTM + _ClientLevelInfo.EquityMTM;

                                //Added by Akshay on 23-07-2021 for NPL
                                var UNPLKey = $"{_ID}^{_Underlying}";
                                var UNPL = _UCClient.MTM;
                                if (CollectionHelper.dict_NPLValues.TryGetValue(UNPLKey, out double _NPL))
                                    UNPL = _UCClient.MTM + DivideByBaseAndRound(_NPL, nameof(_CPParent.MTM));

                                _UCClient.NPL = UNPL;

                                ////added on 28MAY2021 by Amey
                                //_UCClient.IntradayFuturesMTM = _ClientLevelInfo.IntradayFuturesMTM;
                                //_UCClient.IntradayOptionsMTM = _ClientLevelInfo.IntradayOptionsMTM;
                                //_UCClient.IntradayEquityMTM = _ClientLevelInfo.IntradayEquityMTM;

                                ////added on 15APR2021 by Amey
                                //_UCClient.IntradayMTM = _ClientLevelInfo.IntradayFuturesMTM + _ClientLevelInfo.IntradayOptionsMTM + _ClientLevelInfo.IntradayEquityMTM;


                                _UCClient.FuturesMTM = _ClientLevelInfo.FuturesMTM + _ClientLevelInfo.CDSFuturesMTM;
                                _UCClient.OptionsMTM = _ClientLevelInfo.OptionsMTM + _ClientLevelInfo.CDSOptionsMTM;
                                _UCClient.EquityMTM = _ClientLevelInfo.EquityMTM;

                                _UCClient.MTM = _ClientLevelInfo.FuturesMTM + _ClientLevelInfo.OptionsMTM + _ClientLevelInfo.EquityMTM + _ClientLevelInfo.CDSFuturesMTM + _ClientLevelInfo.CDSOptionsMTM;
                                _UCClient.IntradayFuturesMTM = _ClientLevelInfo.IntradayFuturesMTM + _ClientLevelInfo.CDSIntradayFuturesMTM;
                                _UCClient.IntradayOptionsMTM = _ClientLevelInfo.IntradayOptionsMTM + _ClientLevelInfo.CDSIntradayOptionsMTM;
                                _UCClient.IntradayEquityMTM = _ClientLevelInfo.IntradayEquityMTM;

                                //added on 15APR2021 by Amey
                                _UCClient.IntradayMTM = _ClientLevelInfo.IntradayFuturesMTM + _ClientLevelInfo.IntradayOptionsMTM + _ClientLevelInfo.IntradayEquityMTM + _ClientLevelInfo.CDSIntradayFuturesMTM + _ClientLevelInfo.CDSIntradayOptionsMTM;




                                _UCClient.TheoreticalMTM = _ClientLevelInfo.TheoreticalMTM;

                                // Added by Snehadri on 21OCT2021 for UpsideDownVaR in Underlying Tab
                                _UCClient.VAR = 0;
                                _UCClient.Scenario1 = _ClientLevelInfo.Scenario1;
                                _UCClient.Scenario2 = _ClientLevelInfo.Scenario2;
                                _UCClient.Scenario3 = _ClientLevelInfo.Scenario3;
                                _UCClient.Scenario4 = _ClientLevelInfo.Scenario4;

                                _UCClient.Span = _ClientLevelInfo.Span;
                                _UCClient.Exposure = _ClientLevelInfo.Exposure;
                                _UCClient.MarginUtil = _ClientLevelInfo.MarginUtil;

                                _UCClient.SnapSpan = _ClientLevelInfo.SnapSpan;
                                _UCClient.SnapExposure = _ClientLevelInfo.SnapExposure;
                                _UCClient.SnapMarginUtil = _ClientLevelInfo.SnapMarginUtil;

                                _UCClient.Delta = _ClientLevelInfo.Delta;
                                _UCClient.Theta = _ClientLevelInfo.Theta;
                                _UCClient.Gamma = _ClientLevelInfo.Gamma;
                                _UCClient.Vega = _ClientLevelInfo.Vega;

                                //added on 24MAY2021 by Amey
                                _UCClient.DeltaAmount = _ClientLevelInfo.DeltaAmount;

                                _UCClient.EquityAmount = _ClientLevelInfo.EquityAmount;
                                _UCClient.PayInPayOut = _ClientLevelInfo.PayInPayOut;
                                _UCClient.DayNetPremium = _ClientLevelInfo.DayNetPremium;
                                _UCClient.DayNetPremiumCDS = _ClientLevelInfo.DayNetPremiumCDS;
                                _UCClient.VARMargin = _ClientLevelInfo.VARMargin;

                                //added by nikhil
                                _UCClient.T1Quantity = _ClientLevelInfo.T1Quantity;
                                _UCClient.T2Quantity = _ClientLevelInfo.T2Quantity;
                                _UCClient.EarlyPayIn = _ClientLevelInfo.EarlyPayIn;

                                _UCClient.DeliveryMargin = _DeliveryMargin;

                                //Added by Akshay on 29-06-2021
                                _UCClient.PosExpoOPT = PosExpoOPT;
                                _UCClient.PosExpoFUT = PosExpoFUT;
                                _UCClient.NetQnty = NetQnty;

                            }));
                        }
                    }
                }

                if (UpdateUCParent)
                {
                    _UCParent.FuturesMTM = _ULevelInfo.FuturesMTM + _ULevelInfo.CDSFuturesMTM ;
                    _UCParent.OptionsMTM = _ULevelInfo.OptionsMTM + _ULevelInfo.CDSOptionsMTM ;
                    _UCParent.EquityMTM = _ULevelInfo.EquityMTM;
                    _UCParent.MTM = _ULevelInfo.FuturesMTM + _ULevelInfo.OptionsMTM + _ULevelInfo.EquityMTM + _ULevelInfo.CDSFuturesMTM + _ULevelInfo.CDSOptionsMTM;

                    _UCParent.IntradayFuturesMTM = _ULevelInfo.IntradayFuturesMTM + _ULevelInfo.CDSIntradayFuturesMTM ;
                    _UCParent.IntradayOptionsMTM = _ULevelInfo.IntradayOptionsMTM + _ULevelInfo.CDSIntradayOptionsMTM ;
                    _UCParent.IntradayEquityMTM = _ULevelInfo.IntradayEquityMTM;
                    _UCParent.IntradayMTM = _ULevelInfo.IntradayFuturesMTM + _ULevelInfo.IntradayOptionsMTM + _ULevelInfo.IntradayEquityMTM + _ULevelInfo.CDSIntradayFuturesMTM + _ULevelInfo.CDSIntradayOptionsMTM ;

                    _UCParent.TheoreticalMTM = _ULevelInfo.TheoreticalMTM;

                    _UCParent.Theta = _ULevelInfo.Theta;
                    _UCParent.Delta = _ULevelInfo.Delta;
                    _UCParent.Gamma = _ULevelInfo.Gamma;
                    _UCParent.Vega = _ULevelInfo.Vega;
                    _UCParent.DeltaAmount = _ULevelInfo.DeltaAmount;

                    _UCParent.Span = _ULevelInfo.Span;
                    _UCParent.Exposure = _ULevelInfo.Exposure;
                    _UCParent.MarginUtil = (_ULevelInfo.Span < 0 ? 0 : _ULevelInfo.Span) + _ULevelInfo.Exposure;

                    _UCParent.SnapSpan = _ULevelInfo.SnapSpan;
                    _UCParent.SnapExposure = _ULevelInfo.SnapExposure;
                    _UCParent.SnapMarginUtil = (_ULevelInfo.SnapSpan < 0 ? 0 : _ULevelInfo.SnapSpan) + _ULevelInfo.SnapExposure;

                    _UCParent.DeliveryMargin = _ULevelInfo.DeliveryMargin;
                    _UCParent.NetposDelMargin = _ULevelInfo.NetposDelMargin;
                    _UCParent.Obligation = _ULevelInfo.NetposDelMargin;

                    _SpotPrice = _SpotPrice < 0 ? _CurrMonthLTP : _SpotPrice;
                    if (_UCParent.SpotPrice != 0)
                        _UCParent.PercentChange = (_SpotPrice - _UCParent.SpotPrice) / _UCParent.SpotPrice;
                    else
                        _UCParent.PercentChange = 0;

                    _UCParent.SpotPrice = _SpotPrice;

                    //Added by Akshay on 30-06-2021 for Pos Expo
                    _UCParent.PosExpoOPT = ULevelPosExpoOPT;
                    _UCParent.PosExpoFUT = ULevelPosExpoFUT;

                    //Added by Akshay on 23-07-2021 for NPL Values
                    var UNPL = _UCParent.MTM;
                    if (dict_UnderlyingLevelNPL.TryGetValue(_Underlying, out double _NPL))
                        UNPL = _UCParent.MTM + DivideByBaseAndRound(_NPL, nameof(_CPParent.MTM));

                    _UCParent.NPL = UNPL;
                    // Added by Snehadri on 29SEP2021
                    _UCParent.Turnover = DivideByBaseAndRound(_ULevelInfo.Turnover, nameof(_CPParent.Turnover));

                    // Added by Snehadri on 21OCT2021 for UpsideDownVaR in Underlying Tab
                    _UCParent.VAR = _ULevelInfo.VaR;
                    _UCParent.Scenario1 = _ULevelInfo.Scenario1;
                    _UCParent.Scenario2 = _ULevelInfo.Scenario2;
                    _UCParent.Scenario3 = _ULevelInfo.Scenario3;
                    _UCParent.Scenario4 = _ULevelInfo.Scenario4;


                }
            }
            catch (Exception ee) { _logger.Error(ee); }

            isComputing = false;
        }

        #endregion

        #region ClientWinow

        internal nCompute()
        {
            _logger = CollectionHelper._logger;
        }

        internal void AddToClientWindowQueue(List<ConsolidatedPositionInfo> list_ReceivedPositions)
        {
            isComputing = true;
            //Task.Run(() => ComputeAndUpdateClientWndow(list_ReceivedPositions));//, _ClientID, _Underlying));
            ComputeAndUpdateClientWndow(list_ReceivedPositions);
        }

        private void ComputeAndUpdateClientWndow(List<ConsolidatedPositionInfo> list_ReceivedPositions)
        {
            try
            {

                //var dict_ClientWindowPositions = new Dictionary<string, CWPositions>();

                double tDeltaCE = 0; //Added by Akshay 
                double tThetaCE = 0; //Added by Akshay 
                double tVegaCE = 0; //Added by Akshay 
                double tGammaCE = 0; //Added by Akshay
                long tLongCE = 0; //Added by Akshay 
                long tShortCE = 0; //Added by Akshay 
                long tTotalCE = 0; //Added by Akshay 

                double tDeltaPE = 0; //Added by Akshay 
                double tThetaPE = 0; //Added by Akshay 
                double tVegaPE = 0; //Added by Akshay 
                double tGammaPE = 0; //Added by Akshay
                long tLongPE = 0; //Added by Akshay 
                long tShortPE = 0; //Added by Akshay 
                long tTotalPE = 0; //Added by Akshay 

                double tDeltaTotal = 0; //Added by Akshay 
                double tThetaTotal = 0; //Added by Akshay 
                double tVegaTotal = 0; //Added by Akshay 
                double tGammaTotal = 0; //Added by Akshay
                long tLongTotal = 0; //Added by Akshay 
                long tShortTotal = 0; //Added by Akshay 
                long tTotal = 0; //Added by Akshay 

                long tQntyCFXX = 0; //Added by Akshay
                long tQntyXX = 0; //Added by Akshay 
                long tNetQntyXX = 0; //Added by Akshay 
                double tAvgPriceXX = 0; //Added by Akshay 

                long tQntyCFEQ = 0; //Added by Akshay
                long tQntyEQ = 0; //Added by Akshay 
                long tNetQntyEQ = 0; //Added by Akshay 
                double tAvgPriceEQ = 0; //Added by Akshay 

                double _DeltaAmt = 0;
                double _Delta = 0;
                double _Gamma = 0;
                double _Vega = 0;
                double _Theta = 0;
                double _MTM = 0;
                double _Span = 0;
                double _Exposure = 0;
                double _Margin = 0;
                double _VARMargin = 0;

                //added by nikhil
                long _T1Quantity = 0;
                long _T2Quantity = 0;
                long _EarlyPayIn = 0;

                foreach (var _PositionInfo in list_ReceivedPositions)//list_Positions)
                {

                    DateTime tExpiry = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry);
                    double tBEP = _PositionInfo.BEP;
                    double tPriceCF = _PositionInfo.PriceCF;

                    double tFMTM = 0;
                    double tOMTM = 0;
                    double tEMTM = 0;

                    double tIntradayFMTM = 0;
                    double tIntradayOMTM = 0;
                    double tIntradayEMTM = 0;

                    double tIntradayCost = 0; //Added by Akshay on 22-07-2021 for Cost Computing

                    //added on 24MAY2021 by Amey
                    switch (_PositionInfo.ScripType)
                    {
                        case en_ScripType.EQ:
                            tEMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                            tIntradayEMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                            break;
                        case en_ScripType.XX:
                            tFMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                            tIntradayFMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                            break;
                        case en_ScripType.CE:
                        case en_ScripType.PE:
                            tOMTM = DivideByBaseAndRound(_PositionInfo.MTM, nameof(_CPParent.MTM));
                            tIntradayOMTM = DivideByBaseAndRound(_PositionInfo.IntradayMTM, nameof(_CPParent.IntradayMTM));
                            break;
                    }


                    double tIntradayBEP = _PositionInfo.IntradayBEP;

                    //Added by Akshay on 21-12-2020 
                    double tSingleDelta = DivideByBaseAndRound(_PositionInfo.SingleDelta, nameof(_CPParent.AbsDelta));

                    //Added by Akshay on 21-12-2020 
                    double tSingleGamma = DivideByBaseAndRound(_PositionInfo.SingleGamma, nameof(_CPParent.AbsGamma));

                    double tDelta = DivideByBaseAndRound(_PositionInfo.Delta * (CollectionHelper._ValueSigns.Delta), nameof(_CPParent.Delta));

                    double tGamma = DivideByBaseAndRound(_PositionInfo.Gamma * (CollectionHelper._ValueSigns.Gamma), nameof(_CPParent.Gamma));

                    double tTheta = DivideByBaseAndRound(_PositionInfo.Theta * (CollectionHelper._ValueSigns.Theta), nameof(_CPParent.Theta));

                    //added on 24MAY2021 by Amey
                    double tDeltaAmt = DivideByBaseAndRound((_PositionInfo.Delta * _PositionInfo.UnderlyingLTP) * (CollectionHelper._ValueSigns.DeltaAmt), nameof(_CPParent.DeltaAmount));

                    double TimeValue = 0;
                    double _ExpTheta = tTheta;

                    double tVega = DivideByBaseAndRound(_PositionInfo.Vega * (CollectionHelper._ValueSigns.Vega), nameof(_CPParent.Vega));

                    //added on 15APR2021 by Amey
                    double tTMTM = DivideByBaseAndRound(_PositionInfo.TheoreticalMTM, nameof(_CPParent.MTM));

                    double _ROV = 0;

                    long tNetPsition = _PositionInfo.NetPosition;

                    //added on 29APR2021 by Amey
                    if (_PositionInfo.ScripType == en_ScripType.CE || _PositionInfo.ScripType == en_ScripType.PE)
                    {
                        TimeValue = DivideByBaseAndRound(GetTimeValue(_PositionInfo.ScripType, _PositionInfo.UnderlyingLTP, _PositionInfo.StrikePrice,
                            _PositionInfo.LTP, _PositionInfo.NetPosition), nameof(_CPParent.Theta));

                        //added on 3JUN2021 by Amey
                        if (_PositionInfo.ExpiryTimeSpan.TotalDays < 1)
                            _ExpTheta = Math.Min(Math.Abs(TimeValue), Math.Abs(tTheta)) * (tTheta < 0 ? -1 : 1);

                        _ROV = DivideByBaseAndRound(_PositionInfo.NetPosition * _PositionInfo.LTP, nameof(_CPParent.ROV));

                        if (_PositionInfo.ScripType == en_ScripType.CE)
                        {
                            tDeltaCE += tDelta;
                            tThetaCE += tTheta;
                            tVegaCE += tVega;
                            tGammaCE += tGamma;
                            if (tNetPsition > 0)
                                tLongCE += tNetPsition;
                            else
                                tShortCE += tNetPsition;
                            tTotalCE += tNetPsition;
                        }
                        else if (_PositionInfo.ScripType == en_ScripType.PE)
                        {
                            tDeltaPE += tDelta;
                            tThetaPE += tTheta;
                            tVegaPE += tVega;
                            tGammaPE += tGamma;
                            if (tNetPsition > 0)
                                tLongPE += tNetPsition;
                            else
                                tShortPE += tNetPsition;
                            tTotalPE += tNetPsition;
                        }

                        tDeltaTotal += tDelta;
                        tThetaTotal += tTheta;
                        tVegaTotal += tVega;
                        tGammaTotal += tGamma;
                        if (tNetPsition > 0)
                            tLongTotal += tNetPsition;
                        else
                            tShortTotal += tNetPsition;
                        tTotal += tNetPsition;
                    }
                    else if (_PositionInfo.ScripType == en_ScripType.XX)
                    {
                        tAvgPriceXX = _PositionInfo.BEP;
                        tQntyCFXX = _PositionInfo.NetPositionCF;
                        tQntyXX = _PositionInfo.IntradayNetPosition;
                        tNetQntyXX = tNetPsition;
                    }
                    else if (_PositionInfo.ScripType == en_ScripType.EQ)
                    {
                        tAvgPriceEQ = _PositionInfo.BEP;
                        tQntyCFEQ = _PositionInfo.NetPositionCF;
                        tQntyEQ = _PositionInfo.IntradayNetPosition;
                        tNetQntyEQ = tNetPsition;
                    }

                    _Delta += _PositionInfo.Delta;
                    _Vega += _PositionInfo.Vega;
                    _Theta += _PositionInfo.Theta;
                    _Gamma += _PositionInfo.Gamma;
                    _MTM += _PositionInfo.MTM;
                    _VARMargin += _PositionInfo.VARMargin;

                    //added by nikhil 
                    _T1Quantity += _PositionInfo.T1Quantity;
                    _T2Quantity += _PositionInfo.T2Quantity;
                    _EarlyPayIn += _PositionInfo.EarlyPayIn;

                    _DeltaAmt += tDeltaAmt;

                    if (CollectionHelper.dict_ClientUnderlyingWiseSpanInfo.TryGetValue(_PositionInfo.Username + "_" + _PositionInfo.Underlying, out UnderlyingSpanInfo _UnderlyingSpanInfo))
                    {
                        _Span = DivideByBaseAndRound(_UnderlyingSpanInfo.Span, nameof(_CPParent.Span));
                        _Exposure = DivideByBaseAndRound(_UnderlyingSpanInfo.Span, nameof(_CPParent.Exposure));
                        _Margin = DivideByBaseAndRound(_UnderlyingSpanInfo.Span, nameof(_CPParent.MarginUtil));
                    }

                    double tDaysToExpiry = _PositionInfo.ScripType == en_ScripType.EQ ? 0 : Math.Floor((_PositionInfo.ExpiryTimeSpan.TotalDays * 0.1) * 100) / 10;

                    #region Positions
                    var _CWPositions = new CWPositions();
                    var ScripToken = _PositionInfo.Token;

                    lock (CollectionHelper._CWLock)
                    {
                        _CWPositions = CollectionHelper.bList_ClientWindow.Where(v => v.ScripToken == ScripToken).FirstOrDefault();
                    }

                    if (_CWPositions is null)
                    {
                        _CWPositions = new CWPositions { ScripToken = ScripToken };
                        CollectionHelper.bList_ClientWindow.Add(_CWPositions);
                    }

                    var UNPLKey = $"{_ClientID}^{_PositionInfo.Underlying}";
                    var TotalMTM = tFMTM + tOMTM + tEMTM;
                    var UNPL = TotalMTM;
                    if (CollectionHelper.dict_NPLValues.TryGetValue(UNPLKey, out double _NPL))
                        UNPL = TotalMTM + DivideByBaseAndRound(_NPL, nameof(_CPParent.MTM));



                    _CWPositions.VARMargin = DivideByBaseAndRound(_PositionInfo.VARMargin, nameof(_CPParent.VARMargin));
                    _CWPositions.NPL = UNPL;

                    //added by nikhil
                    _CWPositions.T1Quantity = _PositionInfo.T1Quantity;
                    _CWPositions.T2Quantity = _PositionInfo.T2Quantity;
                    _CWPositions.EarlyPayIn = _PositionInfo.EarlyPayIn;

                    _CWPositions.ClientID = _PositionInfo.Username;
                    _CWPositions.Underlying = _PositionInfo.Underlying;
                    _CWPositions.ScripName = _PositionInfo.ScripName;
                    _CWPositions.Segment = _PositionInfo.Segment;
                    _CWPositions.Series = _PositionInfo.Series;
                    _CWPositions.InstrumentName = _PositionInfo.InstrumentName;
                    _CWPositions.ScripToken = _PositionInfo.Token;
                    _CWPositions.ExpiryDate = tExpiry;
                    _CWPositions.ScripType = _PositionInfo.ScripType;
                    _CWPositions.StrikePrice = _PositionInfo.StrikePrice;
                    _CWPositions.NetPosition = _PositionInfo.NetPosition;
                    _CWPositions.BEP = tBEP;
                    _CWPositions.NetPositionCF = _PositionInfo.NetPositionCF;
                    _CWPositions.PriceCF = tPriceCF;
                    _CWPositions.LTP = _PositionInfo.LTP;
                    _CWPositions.UnderlyingLTP = _PositionInfo.UnderlyingLTP;
                    _CWPositions.TheoreticalPrice = _PositionInfo.TheoreticalPrice;
                    _CWPositions.TheoreticalMTM = tTMTM;
                    _CWPositions.ROV = _ROV;
                    _CWPositions.SpotPrice = _PositionInfo.SpotPrice;
                    _CWPositions.AtmIV = _PositionInfo.ATM_IV;
                    _CWPositions.FuturesMTM = tFMTM;
                    _CWPositions.OptionsMTM = tOMTM;
                    _CWPositions.EquityMTM = tEMTM;
                    _CWPositions.MTM = tFMTM + tOMTM + tEMTM;
                    _CWPositions.IntradayFuturesMTM = tIntradayFMTM;
                    _CWPositions.IntradayOptionsMTM = tIntradayOMTM;
                    _CWPositions.IntradayEquityMTM = tIntradayEMTM;
                    _CWPositions.IntradayMTM = tIntradayFMTM + tIntradayOMTM + tIntradayEMTM;
                    _CWPositions.IntradayCost = tIntradayCost; //Added by Akshay on 22-07-2021 for Cost Computing
                    _CWPositions.IntradayBEP = tIntradayBEP;
                    _CWPositions.IntradayNetPosition = _PositionInfo.IntradayNetPosition;
                    _CWPositions.AbsDelta = tSingleDelta;    //Added by Akshay on 21-12-2020
                    _CWPositions.AbsGamma = tSingleGamma;    //Added by Akshay on 21-12-2020
                    _CWPositions.Delta = tDelta;
                    _CWPositions.Theta = tTheta;
                    _CWPositions.Gamma = tGamma;
                    _CWPositions.Vega = tVega;
                    _CWPositions.TV = TimeValue;
                    _CWPositions.ExpTheta = _ExpTheta;
                    _CWPositions.DeltaAmount = tDeltaAmt;
                    _CWPositions.DaysToExpiry = tDaysToExpiry;
                    _CWPositions.IsLTPCalculated = _PositionInfo.IsLTPCalculated;
                    _CWPositions.LiveIV = _PositionInfo.IVMiddle;
                    _CWPositions.IntradayBuyQuantity = _PositionInfo.IntradayBuyQuantity;
                    _CWPositions.IntradayBuyAvg = _PositionInfo.IntradayBuyAvg;
                    _CWPositions.IntradaySellQuantity = _PositionInfo.IntradaySellQuantity;
                    _CWPositions.IntradaySellAvg = _PositionInfo.IntradaySellAvg;
                    _CWPositions.Turnover = DivideByBaseAndRound((_PositionInfo.IntradayBuyQuantity * _PositionInfo.IntradayBuyAvg) + (_PositionInfo.IntradaySellQuantity * _PositionInfo.IntradaySellAvg), nameof(_CPParent.Turnover));
                    #endregion
                }

                #region Greeks
                var _CWGreeks = new CWGreeks();

                lock (CollectionHelper._CWLock)
                {
                    _CWGreeks = CollectionHelper.bList_ClientWindowGreeks.Where(v => v.Greeks == "Values").FirstOrDefault();
                }

                if (_CWGreeks is null)
                {
                    _CWGreeks = new CWGreeks { Greeks = "Values" };
                    CollectionHelper.bList_ClientWindowGreeks.Add(_CWGreeks);
                }

                _CWGreeks.Delta = DivideByBaseAndRound(_Delta * (CollectionHelper._ValueSigns.Delta), nameof(_CPParent.Delta));
                _CWGreeks.DeltaAmt = DivideByBaseAndRound(_DeltaAmt * (CollectionHelper._ValueSigns.DeltaAmt), nameof(_CPParent.DeltaAmount));
                _CWGreeks.Theta = DivideByBaseAndRound(_Theta * (CollectionHelper._ValueSigns.Theta), nameof(_CPParent.Theta));
                _CWGreeks.Gamma = DivideByBaseAndRound(_Gamma * (CollectionHelper._ValueSigns.Gamma), nameof(_CPParent.Gamma));
                _CWGreeks.Vega = DivideByBaseAndRound(_Vega * (CollectionHelper._ValueSigns.Vega), nameof(_CPParent.Vega));
                _CWGreeks.Span = DivideByBaseAndRound(_Span, nameof(_CPParent.Span));
                _CWGreeks.Exposure = DivideByBaseAndRound(_Exposure, nameof(_CPParent.Exposure));
                _CWGreeks.Margin = DivideByBaseAndRound(_Margin, nameof(_CPParent.MarginUtil));
                _CWGreeks.MTM = DivideByBaseAndRound(_MTM, nameof(_CPParent.MTM));
                _CWGreeks.VARMargin = DivideByBaseAndRound(_VARMargin, nameof(_CPParent.VARMargin));

                _CWGreeks.T1Quantity = _T1Quantity;
                _CWGreeks.T2Quantity = _T2Quantity;
                _CWGreeks.EarlyPayIn = _EarlyPayIn;
                #endregion

                #region Options
                var _CWOptions = new CWOptions();

                lock (CollectionHelper._CWLock)
                {
                    _CWOptions = CollectionHelper.bList_ClientWindowOptions.Where(v => v.Options == "CE").FirstOrDefault();
                }

                if (_CWOptions is null)
                {
                    _CWOptions = new CWOptions { Options = "CE" };
                    CollectionHelper.bList_ClientWindowOptions.Add(_CWOptions);
                }

                _CWOptions.Delta = tDeltaCE;
                _CWOptions.Theta = tThetaCE;
                _CWOptions.Vega = tVegaCE;
                _CWOptions.Gamma = tGammaCE;
                _CWOptions.Long = tLongCE;
                _CWOptions.Short = tShortCE;
                _CWOptions.Total = tTotalCE;

                lock (CollectionHelper._CWLock)
                {
                    _CWOptions = CollectionHelper.bList_ClientWindowOptions.Where(v => v.Options == "PE").FirstOrDefault();
                }

                if (_CWOptions is null)
                {
                    _CWOptions = new CWOptions { Options = "PE" };
                    CollectionHelper.bList_ClientWindowOptions.Add(_CWOptions);
                }

                _CWOptions.Delta = tDeltaPE;
                _CWOptions.Theta = tThetaPE;
                _CWOptions.Vega = tVegaPE;
                _CWOptions.Gamma = tGammaPE;
                _CWOptions.Long = tLongPE;
                _CWOptions.Short = tShortPE;
                _CWOptions.Total = tTotalPE;

                lock (CollectionHelper._CWLock)
                {
                    _CWOptions = CollectionHelper.bList_ClientWindowOptions.Where(v => v.Options == "Total").FirstOrDefault();
                }

                if (_CWOptions is null)
                {
                    _CWOptions = new CWOptions { Options = "Total" };
                    CollectionHelper.bList_ClientWindowOptions.Add(_CWOptions);
                }

                _CWOptions.Delta = tDeltaTotal;
                _CWOptions.Theta = tThetaTotal;
                _CWOptions.Vega = tVegaTotal;
                _CWOptions.Gamma = tGammaTotal;
                _CWOptions.Long = tLongTotal;
                _CWOptions.Short = tShortTotal;
                _CWOptions.Total = tTotal;
                #endregion

                #region Futures
                var _CWFutures = new CWFutures();

                lock (CollectionHelper._CWLock)
                {
                    _CWFutures = CollectionHelper.bList_ClientWindowFutures.Where(v => v.Futures == "Futures").FirstOrDefault();
                }

                if (_CWFutures is null)
                {
                    _CWFutures = new CWFutures { Futures = "Futures" };
                    CollectionHelper.bList_ClientWindowFutures.Add(_CWFutures);
                }

                _CWFutures.Qnty = tQntyXX;
                _CWFutures.QntyCF = tQntyCFXX;
                _CWFutures.NetQnty = tNetQntyXX;
                _CWFutures.AvgPrice = tAvgPriceXX;


                lock (CollectionHelper._CWLock)
                {
                    _CWFutures = CollectionHelper.bList_ClientWindowFutures.Where(v => v.Futures == "Stocks").FirstOrDefault();
                }

                if (_CWFutures is null)
                {
                    _CWFutures = new CWFutures { Futures = "Stocks" };
                    CollectionHelper.bList_ClientWindowFutures.Add(_CWFutures);
                }

                _CWFutures.Qnty = tQntyEQ;
                _CWFutures.QntyCF = tQntyCFEQ;
                _CWFutures.NetQnty = tNetQntyEQ;
                _CWFutures.AvgPrice = tAvgPriceEQ;
                #endregion

            }
            catch (Exception ee) { _logger.Error(ee); }

            isComputing = false;
        }

        #endregion


        //Added by Akshay on 13-06-2022 for Delivery Tab
        #region Delivery Report

        internal nCompute(string ClientID, bool DeliveryReport)
        {
            _logger = CollectionHelper._logger;

            this._ClientID = ClientID;

            //Added by Akshay on 25-03-2021
            ExpiryThreshHold = CollectionHelper.dict_DaysPercentage.Keys.Max();
        }

        internal void AddToDRClientQueue(List<ConsolidatedPositionInfo> list_ReceivedPositions)
        {
            //changed to TryGetValue on 27MAY2021 by Amey
            //added on 15MAR2021 by Amey
            //if (CollectionHelper.dict_Priority.TryGetValue(_ClientID, out double _Priority))
            //    _CPParent.Priority = _Priority;

            isComputing = true;

            //this.isDRParentExpanded = isDRParentExpanded || !isInitialLoadDRUnderlyingSuccess;
            //this.isCPUnderlyingExpanded = list_ExpandedUnderlying.Any() || !isInitialLoadCPUnderlyingSuccess;

            Task.Run(() => ComputeAndUpdateDeliveryReport(list_ReceivedPositions));
        }

        private void ComputeAndUpdateDeliveryReportOLD(List<ConsolidatedPositionInfo> list_ReceivedPositions)
        {
            try
            {
                var dict_RecentPositions = list_ReceivedPositions.GroupBy(row => row.Underlying).ToDictionary(kvp => kvp.Key, kvp => kvp.ToList());

                dict_UnderlyingLevel.Clear();

                foreach (var Underlying in dict_RecentPositions.Keys)
                {

                    long ULNetPosition = 0, ULIntradayNetPosition = 0, ULNetPositionCF = 0;
                    double ULBEP = 0.0, ULPriceCF = 0.0, ULIntradayBEP = 0.0, ULVARMargin = 0.0;



                    if (!dict_UnderlyingLevel.ContainsKey(Underlying))
                        dict_UnderlyingLevel.Add(Underlying, new UnderlyingLevelInfo());

                    DRUnderlying _DRUnderlying = null;

                    lock (CollectionHelper._DRLock)
                    {
                        _DRUnderlying = CollectionHelper.bList_DeliveryReport.Where(v => v.Underlying == Underlying && v.ClientID == _ClientID).FirstOrDefault();
                    }

                    CollectionHelper.gc_DR.Invoke((MethodInvoker)(() =>
                    {
                        if (_DRUnderlying is null)
                        {
                            _DRUnderlying = new DRUnderlying();
                            _DRUnderlying.ClientID = _ClientID;
                            _DRUnderlying.Underlying = Underlying;

                            _DRUnderlying.bList_Positions = new BindingList<DRPositions>();

                            lock (CollectionHelper._DRLock)
                                CollectionHelper.bList_DeliveryReport.Add(_DRUnderlying);
                        }
                    }));

                    foreach (var _PositionInfo in dict_RecentPositions[Underlying])
                    {
                        var ScripKey = $"{_PositionInfo.Segment}|{_PositionInfo.Token}";

                        if (_PositionInfo.LTP <= 0 || _PositionInfo.UnderlyingLTP <= 0)
                        {
                            if (CollectionHelper.IsDebug)
                                _logger.Debug($"NOLTP|{_ClientID}|{_PositionInfo.ScripName}|{_PositionInfo.Token}|{_PositionInfo.LTP}|{_PositionInfo.UnderlyingLTP}");

                            continue;
                        }

                        if (_PositionInfo.ScripType != en_ScripType.EQ && _PositionInfo.ExpiryTimeSpan.TotalDays <= 0)
                        {
                            if (!dict_TokensRemove.ContainsKey(ScripKey))
                                dict_TokensRemove.Add(ScripKey, Underlying);

                            if (CollectionHelper.IsDebug)
                                _logger.Debug($"Expired|{_ClientID}|{_PositionInfo.ScripName}|{_PositionInfo.ExpiryTimeSpan.TotalDays}");

                            continue;
                        }

                        DateTime Expiry = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry);

                        //Added by Akshay on 25-03-2021 For Delivery Margin
                        try
                        {
                            if (_PositionInfo.InstrumentName == en_InstrumentName.OPTSTK || _PositionInfo.InstrumentName == en_InstrumentName.FUTSTK)
                            {
                                if (!dict_ExpiryDays.ContainsKey(_PositionInfo.Expiry))
                                    dict_ExpiryDays.Add(_PositionInfo.Expiry, CountDaysToExpiry(CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry), DateTime.Now));

                                var DaysToExpiry = dict_ExpiryDays[_PositionInfo.Expiry];
                                if (DaysToExpiry <= ExpiryThreshHold)
                                {
                                    if (_PositionInfo.SpotPrice > 0 && _PositionInfo.InstrumentName == en_InstrumentName.OPTSTK)
                                    {

                                        if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.StrikePrice < _PositionInfo.SpotPrice && _PositionInfo.NetPosition > 0)
                                            dict_UnderlyingLevel[Underlying].CallBuyQty += _PositionInfo.NetPosition;
                                        else if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.StrikePrice < _PositionInfo.SpotPrice && _PositionInfo.NetPosition < 0)
                                            dict_UnderlyingLevel[Underlying].CallSellQty += _PositionInfo.NetPosition;
                                        else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.StrikePrice > _PositionInfo.SpotPrice && _PositionInfo.NetPosition > 0)
                                            dict_UnderlyingLevel[Underlying].PutBuyQty += _PositionInfo.NetPosition * -1;
                                        else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.StrikePrice > _PositionInfo.SpotPrice && _PositionInfo.NetPosition < 0)
                                            dict_UnderlyingLevel[Underlying].PutSellQty += _PositionInfo.NetPosition * -1;
                                        else
                                            continue;
                                    }
                                    else
                                        dict_UnderlyingLevel[Underlying].FutQty += _PositionInfo.NetPosition;

                                    dict_UnderlyingLevel[Underlying].Obligation += _PositionInfo.NetPosition;

                                }
                                else
                                    continue;

                                if (_PositionInfo.SpotPrice > 0 && CollectionHelper.dict_DaysPercentage.TryGetValue(DaysToExpiry, out double _DaysPercentage))
                                    dict_UnderlyingLevel[Underlying].ValMargin = _PositionInfo.SpotPrice * _PositionInfo.UnderlyingVARMargin * _DaysPercentage / 100;
                            }
                            else if (_PositionInfo.InstrumentName == en_InstrumentName.EQ)
                            {
                                ULNetPosition += _PositionInfo.NetPosition;
                                ULIntradayNetPosition += _PositionInfo.IntradayNetPosition;
                                ULNetPositionCF += _PositionInfo.NetPositionCF;
                                ULBEP += _PositionInfo.BEP;
                                ULPriceCF += _PositionInfo.PriceCF;
                                ULIntradayBEP += _PositionInfo.IntradayBEP;
                                ULVARMargin += _PositionInfo.VARMargin;

                            }
                            else
                                continue;
                        }
                        catch (Exception ee) { _logger.Error(ee, _ClientID + "|DeliveryMargin Loop : " + Underlying); }

                        var Token = _PositionInfo.Token;
                        var TokenSegment = _PositionInfo.Segment;
                        var ExpiryDate = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry);

                        DRPositions _DRPositions = null;

                        lock (CollectionHelper._CPLock)
                        {
                            _DRPositions = _DRUnderlying.bList_Positions.Where(v => v.ScripToken == Token).FirstOrDefault();
                        }

                        CollectionHelper.gc_DR.Invoke((MethodInvoker)(() =>
                        {
                            if (_DRPositions is null)
                            {
                                _DRPositions = new DRPositions
                                {
                                    ClientID = _ClientID,
                                    Underlying = _PositionInfo.Underlying,
                                    ScripName = _PositionInfo.ScripName,
                                    Segment = _PositionInfo.Segment,
                                    Series = _PositionInfo.Series,
                                    InstrumentName = _PositionInfo.InstrumentName,
                                    ScripToken = _PositionInfo.Token,
                                    ExpiryDate = ExpiryDate,
                                    ScripType = _PositionInfo.ScripType,
                                    StrikePrice = _PositionInfo.StrikePrice
                                };
                                _DRUnderlying.bList_Positions.Add(_DRPositions);
                            }

                            _DRPositions.NetPosition = _PositionInfo.NetPosition;
                            _DRPositions.BEP = _PositionInfo.BEP;
                            _DRPositions.NetPositionCF = _PositionInfo.NetPositionCF;
                            _DRPositions.PriceCF = _PositionInfo.PriceCF;
                            _DRPositions.LTP = _PositionInfo.LTP;
                            _DRPositions.UnderlyingLTP = _PositionInfo.UnderlyingLTP;
                            _DRPositions.SpotPrice = _PositionInfo.SpotPrice;
                        }));
                    }

                    var _UnderlyingInfo = dict_UnderlyingLevel[Underlying];

                    long ITMQty = _UnderlyingInfo.CallBuyQty + Math.Abs(_UnderlyingInfo.PutBuyQty);
                    long DeliveryQty = _UnderlyingInfo.CallBuyQty + _UnderlyingInfo.CallSellQty + _UnderlyingInfo.PutBuyQty + _UnderlyingInfo.PutSellQty + _UnderlyingInfo.FutQty;
                    //long DeliveryQty = Math.Abs(_UnderlyingInfo.CallBuyQty + _UnderlyingInfo.CallSellQty + _UnderlyingInfo.PutBuyQty + _UnderlyingInfo.PutSellQty + _UnderlyingInfo.FutQty);
                    long DeliveryMargin = Math.Min(ITMQty, Math.Abs(DeliveryQty));

                    CollectionHelper.gc_DR.Invoke((MethodInvoker)(() =>
                    {
                        _DRUnderlying.DeliveryMargin = DivideByBaseAndRound(Math.Abs(DeliveryMargin) * _UnderlyingInfo.ValMargin, nameof(_CPParent.DeliveryMargin));
                        _DRUnderlying.Obligation = DeliveryQty;
                        _DRUnderlying.DeliveryQty = DeliveryMargin;

                        _DRUnderlying.EQNetPosition = ULNetPosition;
                        _DRUnderlying.EQIntradayNetPosition = ULIntradayNetPosition;
                        _DRUnderlying.EQNetPositionCF = ULNetPositionCF;
                        _DRUnderlying.EQBEP = ULBEP;
                        _DRUnderlying.EQPriceCF = ULPriceCF;
                        _DRUnderlying.EQIntradayBEP = ULIntradayBEP;
                        _DRUnderlying.VARMargin = ULVARMargin;

                    }));
                }

                //foreach (var _ScripKey in dict_TokensRemove.Keys.ToList())
                //{
                //    if (dict_TokensRemove[_ScripKey] != "")
                //    {
                //        dict_TokensRemove[_ScripKey] = "";

                //        CPUnderlying _UnderlyingRow = null;
                //        CPPositions _ExpiredPosition = null;

                //        lock (CollectionHelper._DRLock)
                //        {
                //            _UnderlyingRow = _CPParent.bList_Underlying.Where(v => v.Underlying == dict_TokensRemove[_ScripKey]).FirstOrDefault();
                //            if (_UnderlyingRow is null)
                //                continue;

                //            var TokenToRemove = Convert.ToInt32(_ScripKey.Split('|')[1]);
                //            var TokenSegment = _ScripKey.Split('|')[0];

                //            _ExpiredPosition = _UnderlyingRow.bList_Positions.Where(v => v.ScripToken == TokenToRemove && v.Segment.ToString() == TokenSegment).FirstOrDefault();
                //            if (_ExpiredPosition is null)
                //                continue;
                //        }

                //        CollectionHelper.gc_CP.Invoke((MethodInvoker)(() =>
                //        {
                //            _UnderlyingRow.bList_Positions.Remove(_ExpiredPosition);
                //        }));
                //    }
                //}
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
        private void ComputeAndUpdateDeliveryReport(List<ConsolidatedPositionInfo> list_ReceivedPositions)
        {
            try
            {
                var dict_RecentPositions = list_ReceivedPositions.GroupBy(row => row.Underlying).ToDictionary(kvp => kvp.Key, kvp => kvp.ToList());

                dict_UnderlyingLevel.Clear();

                foreach (var Underlying in dict_RecentPositions.Keys)
                {

                    long ULNetPosition = 0, ULIntradayNetPosition = 0, ULNetPositionCF = 0;
                    double ULBEP = 0.0, ULPriceCF = 0.0, ULIntradayBEP = 0.0, ULVARMargin = 0.0;

                    BindingList<DRPositions> list_Positions = new BindingList<DRPositions>();

                    bool isITMPositionFound = false;


                    if (!dict_UnderlyingLevel.ContainsKey(Underlying))
                        dict_UnderlyingLevel.Add(Underlying, new UnderlyingLevelInfo());

                    //DRUnderlying _DRUnderlying = null;

                    //lock (CollectionHelper._DRLock)
                    //{
                    //    _DRUnderlying = CollectionHelper.bList_DeliveryReport.Where(v => v.Underlying == Underlying && v.ClientID == _ClientID).FirstOrDefault();
                    //}

                    //CollectionHelper.gc_DR.Invoke((MethodInvoker)(() =>
                    //{
                    //    if (_DRUnderlying is null)
                    //    {
                    //        _DRUnderlying = new DRUnderlying();
                    //        _DRUnderlying.ClientID = _ClientID;
                    //        _DRUnderlying.Underlying = Underlying;

                    //        _DRUnderlying.bList_Positions = new BindingList<DRPositions>();

                    //        lock (CollectionHelper._DRLock)
                    //            CollectionHelper.bList_DeliveryReport.Add(_DRUnderlying);
                    //    }
                    //}));

                    foreach (var _PositionInfo in dict_RecentPositions[Underlying])
                    {
                        var ScripKey = $"{_PositionInfo.Segment}|{_PositionInfo.Token}";

                        if (_PositionInfo.LTP <= 0 || _PositionInfo.UnderlyingLTP <= 0)
                        {
                            if (CollectionHelper.IsDebug)
                                _logger.Debug($"NOLTP|{_ClientID}|{_PositionInfo.ScripName}|{_PositionInfo.Token}|{_PositionInfo.LTP}|{_PositionInfo.UnderlyingLTP}");

                            continue;
                        }

                        if (_PositionInfo.ScripType != en_ScripType.EQ && _PositionInfo.ExpiryTimeSpan.TotalDays <= 0)
                        {
                            if (!dict_TokensRemove.ContainsKey(ScripKey))
                                dict_TokensRemove.Add(ScripKey, Underlying);

                            if (CollectionHelper.IsDebug)
                                _logger.Debug($"Expired|{_ClientID}|{_PositionInfo.ScripName}|{_PositionInfo.ExpiryTimeSpan.TotalDays}");

                            continue;
                        }

                        DateTime Expiry = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry);

                        //Added by Akshay on 25-03-2021 For Delivery Margin
                        try
                        {
                            if (_PositionInfo.InstrumentName == en_InstrumentName.OPTSTK || _PositionInfo.InstrumentName == en_InstrumentName.FUTSTK)
                            {
                                if (!dict_ExpiryDays.ContainsKey(_PositionInfo.Expiry))
                                    dict_ExpiryDays.Add(_PositionInfo.Expiry, CountDaysToExpiry(CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry), DateTime.Now));

                                var DaysToExpiry = dict_ExpiryDays[_PositionInfo.Expiry];
                                if (DaysToExpiry <= ExpiryThreshHold)
                                {
                                    if (_PositionInfo.SpotPrice > 0 && _PositionInfo.InstrumentName == en_InstrumentName.OPTSTK)
                                    {

                                        if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.StrikePrice < _PositionInfo.SpotPrice && _PositionInfo.NetPosition > 0)
                                        { dict_UnderlyingLevel[Underlying].CallBuyQty += _PositionInfo.NetPosition; isITMPositionFound = true; }
                                        else if (_PositionInfo.ScripType == en_ScripType.CE && _PositionInfo.StrikePrice < _PositionInfo.SpotPrice && _PositionInfo.NetPosition < 0)
                                        { dict_UnderlyingLevel[Underlying].CallSellQty += _PositionInfo.NetPosition; isITMPositionFound = true; }
                                        else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.StrikePrice > _PositionInfo.SpotPrice && _PositionInfo.NetPosition > 0)
                                        { dict_UnderlyingLevel[Underlying].PutBuyQty += _PositionInfo.NetPosition * -1; isITMPositionFound = true; }
                                        else if (_PositionInfo.ScripType == en_ScripType.PE && _PositionInfo.StrikePrice > _PositionInfo.SpotPrice && _PositionInfo.NetPosition < 0)
                                        { dict_UnderlyingLevel[Underlying].PutSellQty += _PositionInfo.NetPosition * -1; isITMPositionFound = true; }
                                        else
                                            continue;
                                    }
                                    else
                                        dict_UnderlyingLevel[Underlying].FutQty += _PositionInfo.NetPosition;

                                    dict_UnderlyingLevel[Underlying].Obligation += _PositionInfo.NetPosition;

                                }
                                else
                                    continue;

                                if (_PositionInfo.SpotPrice > 0 && CollectionHelper.dict_DaysPercentage.TryGetValue(DaysToExpiry, out double _DaysPercentage))
                                    dict_UnderlyingLevel[Underlying].ValMargin = _PositionInfo.SpotPrice * _PositionInfo.UnderlyingVARMargin * _DaysPercentage / 100;
                            }
                            else if (_PositionInfo.InstrumentName == en_InstrumentName.EQ)
                            {
                                ULNetPosition += _PositionInfo.NetPosition;
                                ULIntradayNetPosition += _PositionInfo.IntradayNetPosition;
                                ULNetPositionCF += _PositionInfo.NetPositionCF;
                                ULBEP += _PositionInfo.BEP;
                                ULPriceCF += _PositionInfo.PriceCF;
                                ULIntradayBEP += _PositionInfo.IntradayBEP;
                                ULVARMargin += _PositionInfo.VARMargin;

                            }
                            else
                                continue;
                        }
                        catch (Exception ee) { _logger.Error(ee, _ClientID + "|DeliveryMargin Loop : " + Underlying); }

                        var Token = _PositionInfo.Token;
                        var TokenSegment = _PositionInfo.Segment;
                        var ExpiryDate = CommonFunctions.ConvertFromUnixTimestamp(_PositionInfo.Expiry);

                        DRPositions _DRPositions = null;

                        lock (CollectionHelper._CPLock)
                        {
                            _DRPositions = list_Positions.Where(v => v.ScripToken == Token).FirstOrDefault();
                        }

                        CollectionHelper.gc_DR.Invoke((MethodInvoker)(() =>
                        {
                            if (_DRPositions is null)
                            {
                                _DRPositions = new DRPositions
                                {
                                    ClientID = _ClientID,
                                    Underlying = _PositionInfo.Underlying,
                                    ScripName = _PositionInfo.ScripName,
                                    Segment = _PositionInfo.Segment,
                                    Series = _PositionInfo.Series,
                                    InstrumentName = _PositionInfo.InstrumentName,
                                    ScripToken = _PositionInfo.Token,
                                    ExpiryDate = ExpiryDate,
                                    ScripType = _PositionInfo.ScripType,
                                    StrikePrice = _PositionInfo.StrikePrice
                                };
                                list_Positions.Add(_DRPositions);
                            }

                            _DRPositions.NetPosition = _PositionInfo.NetPosition;
                            _DRPositions.BEP = _PositionInfo.BEP;
                            _DRPositions.NetPositionCF = _PositionInfo.NetPositionCF;
                            _DRPositions.PriceCF = _PositionInfo.PriceCF;
                            _DRPositions.LTP = _PositionInfo.LTP;
                            _DRPositions.UnderlyingLTP = _PositionInfo.UnderlyingLTP;
                            _DRPositions.SpotPrice = _PositionInfo.SpotPrice;
                        }));
                    }

                    var _UnderlyingInfo = dict_UnderlyingLevel[Underlying];

                    long ITMQty = _UnderlyingInfo.CallBuyQty + Math.Abs(_UnderlyingInfo.PutBuyQty);
                    long DeliveryQty = _UnderlyingInfo.CallBuyQty + _UnderlyingInfo.CallSellQty + _UnderlyingInfo.PutBuyQty + _UnderlyingInfo.PutSellQty + _UnderlyingInfo.FutQty;
                    //long DeliveryQty = Math.Abs(_UnderlyingInfo.CallBuyQty + _UnderlyingInfo.CallSellQty + _UnderlyingInfo.PutBuyQty + _UnderlyingInfo.PutSellQty + _UnderlyingInfo.FutQty);
                    long DeliveryMargin = Math.Min(ITMQty, Math.Abs(DeliveryQty));

                    if (isITMPositionFound)
                    {

                        DRUnderlying _DRUnderlying = null;

                        lock (CollectionHelper._DRLock)
                        {
                            _DRUnderlying = CollectionHelper.bList_DeliveryReport.Where(v => v.Underlying == Underlying && v.ClientID == _ClientID).FirstOrDefault();
                        }

                        CollectionHelper.gc_DR.Invoke((MethodInvoker)(() =>
                        {
                            if (_DRUnderlying is null)
                            {
                                _DRUnderlying = new DRUnderlying();
                                _DRUnderlying.ClientID = _ClientID;
                                _DRUnderlying.Underlying = Underlying;

                                _DRUnderlying.bList_Positions = new BindingList<DRPositions>();

                                lock (CollectionHelper._DRLock)
                                    CollectionHelper.bList_DeliveryReport.Add(_DRUnderlying);

                                _DRUnderlying.bList_Positions = list_Positions;
                            }

                            _DRUnderlying.bList_Positions = list_Positions;

                            _DRUnderlying.DeliveryMargin = DivideByBaseAndRound(Math.Abs(DeliveryMargin) * _UnderlyingInfo.ValMargin, nameof(_CPParent.DeliveryMargin));
                           // _DRUnderlying.DeliveryMarginT1 = DivideByBaseAndRound(Math.Abs(DeliveryMargin) * _UnderlyingInfo.ValMarginT1, nameof(_CPParent.DeliveryMargin));

                            _DRUnderlying.Obligation = DeliveryQty;
                            _DRUnderlying.DeliveryQty = DeliveryMargin;

                            _DRUnderlying.EQNetPosition = ULNetPosition;
                            _DRUnderlying.EQIntradayNetPosition = ULIntradayNetPosition;
                            _DRUnderlying.EQNetPositionCF = ULNetPositionCF;
                            _DRUnderlying.EQBEP = ULBEP;
                            _DRUnderlying.EQPriceCF = ULPriceCF;
                            _DRUnderlying.EQIntradayBEP = ULIntradayBEP;
                            _DRUnderlying.VARMargin = ULVARMargin;

                        }));
                    }
                }

                //foreach (var _ScripKey in dict_TokensRemove.Keys.ToList())
                //{
                //    if (dict_TokensRemove[_ScripKey] != "")
                //    {
                //        dict_TokensRemove[_ScripKey] = "";

                //        CPUnderlying _UnderlyingRow = null;
                //        CPPositions _ExpiredPosition = null;

                //        lock (CollectionHelper._DRLock)
                //        {
                //            _UnderlyingRow = _CPParent.bList_Underlying.Where(v => v.Underlying == dict_TokensRemove[_ScripKey]).FirstOrDefault();
                //            if (_UnderlyingRow is null)
                //                continue;

                //            var TokenToRemove = Convert.ToInt32(_ScripKey.Split('|')[1]);
                //            var TokenSegment = _ScripKey.Split('|')[0];

                //            _ExpiredPosition = _UnderlyingRow.bList_Positions.Where(v => v.ScripToken == TokenToRemove && v.Segment.ToString() == TokenSegment).FirstOrDefault();
                //            if (_ExpiredPosition is null)
                //                continue;
                //        }

                //        CollectionHelper.gc_CP.Invoke((MethodInvoker)(() =>
                //        {
                //            _UnderlyingRow.bList_Positions.Remove(_ExpiredPosition);
                //        }));
                //    }
                //}
            }
            catch (Exception ee) { _logger.Error(ee); }
        }
        #endregion



        #region Supplimentary Methods

        private double GetTimeValue(en_ScripType ScripType, double UnderlyingLTP, double StrikePrice, double LTP, long NetPosition)
        {
            double IntrinsicValue;
            if (ScripType == en_ScripType.CE)
                IntrinsicValue = UnderlyingLTP - StrikePrice;
            else
                IntrinsicValue = StrikePrice - UnderlyingLTP;

            IntrinsicValue = IntrinsicValue < 0 ? 0 : IntrinsicValue;

            //fixed on 04MAY2021 by Amey
            double TimeValue = (LTP - IntrinsicValue);
            TimeValue = TimeValue < 0 ? 0 : TimeValue;

            //removed Abs on 12MAY2021 by Amey
            return TimeValue * NetPosition;
        }

        //Added by Akshay on 24-03-2021 For Calculating Days to Expiry excluding Weekends.
        private int CountDaysToExpiry(DateTime endDate, DateTime fromDate)
        {
            try
            {
                var DaysCounter = 0;
                for (DateTime i = fromDate.Date; i < endDate.Date; i = i.AddDays(1))
                {
                    if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday)
                        DaysCounter += 1;
                }
                return DaysCounter;
            }

            catch (Exception ee) { _logger.Error(ee); }

            return 10;
        }

        private double DivideByBaseAndRound(double Value, string ColName)
        {
            return Math.Round(Value / CollectionHelper.dict_BaseValue[ColName], CollectionHelper.dict_CustomDigits[ColName]);
        }

        private bool CheckTodayExpiry(double _PositionExpiry)
        {
            var _ExpiryDay = CommonFunctions.ConvertFromUnixTimestamp(_PositionExpiry).ToString("ddMMyyyy");
            var _TodayDay = DateTime.Now.ToString("ddMMyyyy");

            if (_ExpiryDay == _TodayDay)
                return true;
            else
                return false;
        }

        #endregion
    }
}
