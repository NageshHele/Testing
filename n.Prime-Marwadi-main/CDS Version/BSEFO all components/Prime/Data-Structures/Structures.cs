using n.Structs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Prime
{
    public class OGInfo
    {
        public int OGFrom { get; set; } = -10;
        public int OGTo { get; set; } = 10;
    }

    public class CPParent : INotifyPropertyChanged
    {
        string _ClientID;
        public string ClientID
        {
            get { return _ClientID; }
            set
            {
                if (_ClientID != value)
                {
                    _ClientID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Zone;
        public string Zone
        {
            get { return _Zone; }
            set
            {
                if (_Zone != value)
                {
                    _Zone = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Branch;
        public string Branch
        {
            get { return _Branch; }
            set
            {
                if (_Branch != value)
                {
                    _Branch = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Family;
        public string Family
        {
            get { return _Family; }
            set
            {
                if (_Family != value)
                {
                    _Family = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Product;
        public string Product
        {
            get { return _Product; }
            set
            {
                if (_Product != value)
                {
                    _Product = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Ledger;
        public double Ledger
        {
            get { return _Ledger; }
            set
            {
                if (_Ledger != value)
                {
                    _Ledger = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Adhoc;
        public double Adhoc
        {
            get { return _Adhoc; }
            set
            {
                if (_Adhoc != value)
                {
                    _Adhoc = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _FuturesMTM;
        public double FuturesMTM
        {
            get { return _FuturesMTM; }
            set
            {
                if (_FuturesMTM != value)
                {
                    _FuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _OptionsMTM;
        public double OptionsMTM
        {
            get { return _OptionsMTM; }
            set
            {
                if (_OptionsMTM != value)
                {
                    _OptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _EquityMTM;
        public double EquityMTM
        {
            get { return _EquityMTM; }
            set
            {
                if (_EquityMTM != value)
                {
                    _EquityMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MTM;
        public double MTM
        {
            get { return _MTM; }
            set
            {
                if (_MTM != value)
                {
                    _MTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _NPL = 0;
        public double NPL
        {
            get { return _NPL; }
            set
            {
                if (_NPL != value)
                {
                    _NPL = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MonthlyMTM = 0;
        public double MonthlyMTM
        {
            get { return _MonthlyMTM; }
            set
            {
                if (_MonthlyMTM != value)
                {
                    _MonthlyMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayFuturesMTM;
        public double IntradayFuturesMTM
        {
            get { return _IntradayFuturesMTM; }
            set
            {
                if (_IntradayFuturesMTM != value)
                {
                    _IntradayFuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayOptionsMTM;
        public double IntradayOptionsMTM
        {
            get { return _IntradayOptionsMTM; }
            set
            {
                if (_IntradayOptionsMTM != value)
                {
                    _IntradayOptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }
        double _IntradayEquityMTM;
        public double IntradayEquityMTM
        {
            get { return _IntradayEquityMTM; }
            set
            {
                if (_IntradayEquityMTM != value)
                {
                    _IntradayEquityMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 28OCT2020 by Amey
        double _IntradayMTM;
        public double IntradayMTM
        {
            get { return _IntradayMTM; }
            set
            {
                if (_IntradayMTM != value)
                {
                    _IntradayMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _TheoreticalMTM;
        public double TheoreticalMTM
        {
            get { return _TheoreticalMTM; }
            set
            {
                if (_TheoreticalMTM != value)
                {
                    _TheoreticalMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntrinsicMTM;
        public double IntrinsicMTM
        {
            get { return _IntrinsicMTM; }
            set
            {
                if(_IntrinsicMTM != value)
                {
                    _IntrinsicMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _ROV;
        public double ROV
        {
            get { return _ROV; }
            set
            {
                if (_ROV != value)
                {
                    _ROV = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _VAR;
        public double VAR
        {
            get { return _VAR; }
            set
            {
                if (_VAR != value)
                {
                    _VAR = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario1;          //changed on 19-06-18 by shrii
        public double Scenario1
        {
            get { return _Scenario1; }
            set
            {
                if (_Scenario1 != value)
                {
                    _Scenario1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario2;          //changed on 19-06-18 by shrii
        public double Scenario2
        {
            get { return _Scenario2; }
            set
            {
                if (_Scenario2 != value)
                {
                    _Scenario2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario3;          //changed on 19-06-18 by shrii
        public double Scenario3
        {
            get { return _Scenario3; }
            set
            {
                if (_Scenario3 != value)
                {
                    _Scenario3 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario4;          //changed on 19-06-18 by shrii
        public double Scenario4
        {
            get { return _Scenario4; }
            set
            {
                if (_Scenario4 != value)
                {
                    _Scenario4 = value;
                    NotifyPropertyChanged();
                }
            }
        }


        double _VaRUti;
        public double VaRUti
        {
            get { return _VaRUti; }
            set
            {
                if (_VaRUti != value)
                {
                    _VaRUti = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Span;
        public double Span
        {
            get { return _Span; }
            set
            {
                if (_Span != value)
                {
                    _Span = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Exposure;
        public double Exposure
        {
            get { return _Exposure; }
            set
            {
                if (_Exposure != value)
                {
                    _Exposure = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MarginUtil;
        public double MarginUtil
        {
            get { return _MarginUtil; }
            set
            {
                if (_MarginUtil != value)
                {
                    _MarginUtil = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SnapSpan;
        public double SnapSpan
        {
            get { return _SnapSpan; }
            set
            {
                if (_SnapSpan != value)
                {
                    _SnapSpan = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SnapExposure;
        public double SnapExposure
        {
            get { return _SnapExposure; }
            set
            {
                if (_SnapExposure != value)
                {
                    _SnapExposure = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SnapExpiryMargin;       //added on 22AUG2022 by ninad
        public double SnapExpiryMargin
        {
            get { return _SnapExpiryMargin; }
            set
            {
                if (_SnapExpiryMargin != value)
                {
                    _SnapExpiryMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SnapMarginUtil;
        public double SnapMarginUtil
        {
            get { return _SnapMarginUtil; }
            set
            {
                if (_SnapMarginUtil != value)
                {
                    _SnapMarginUtil = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MarginAvailable;
        public double MarginAvailable
        {
            get { return _MarginAvailable; }
            set
            {
                if (_MarginAvailable != value)
                {
                    _MarginAvailable = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _EODMargin;//24-12-2019
        public double EODMargin
        {
            get { return _EODMargin; }
            set
            {
                if (_EODMargin != value)
                {
                    _EODMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 10-12-2020 for ExpirySpan
        double _ExpMargin;//24-12-2019
        public double ExpiryMargin
        {
            get { return _ExpMargin; }
            set
            {
                if (_ExpMargin != value)
                {
                    _ExpMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 27NOV2020 by Amey
        double _MarginDifference;
        public double MarginDifference
        {
            get { return _MarginDifference; }
            set
            {
                if (_MarginDifference != value)
                {
                    _MarginDifference = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 14OCT2020 by Amey
        double _PeakMargin = 0;
        public double PeakMargin
        {
            get { return _PeakMargin; }
            set
            {
                if (_PeakMargin != value)
                {
                    _PeakMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // added by Snehadri on 10NOV2021
        string _PeakMarginTime = "00:00:00";
        public string PeakMarginTime
        {
            get { return _PeakMarginTime; }
            set
            {
                if (_PeakMarginTime != value)
                {
                    _PeakMarginTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _TotalPeakMargin = 0;
        public double TotalPeakMargin
        {
            get { return _TotalPeakMargin; }
            set
            {
                if (_TotalPeakMargin != value)
                {
                    _TotalPeakMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // added by Snehadri on 10NOV2021
        string _TotalPeakMarginTime = "00:00:00";
        public string TotalPeakMarginTime
        {
            get { return _TotalPeakMarginTime; }
            set
            {
                if (_TotalPeakMarginTime != value)
                {
                    _TotalPeakMarginTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 24JUN2021 by Akshay
        double _VARPeakMargin = 0;
        public double VARPeakMargin
        {
            get { return _VARPeakMargin; }
            set
            {
                if (_VARPeakMargin != value)
                {
                    _VARPeakMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // added by Snehadri on 10NOV2021
        string _VarPeakMarginTime = "00:00:00";
        public string VarPeakMarginTime
        {
            get { return _VarPeakMarginTime; }
            set
            {
                if (_VarPeakMarginTime != value)
                {
                    _VarPeakMarginTime = value;
                    NotifyPropertyChanged();
                }
            }
        }



        double _TimeValue;
        public double TV
        {
            get { return _TimeValue; }
            set
            {
                if (_TimeValue != value)
                {
                    _TimeValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _ExpTheta;
        public double ExpTheta
        {
            get { return _ExpTheta; }
            set
            {
                if (_ExpTheta != value)
                {
                    _ExpTheta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Theta;
        public double Theta
        {
            get { return _Theta; }
            set
            {
                if (_Theta != value)
                {
                    _Theta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Delta;
        public double Delta
        {
            get { return _Delta; }
            set
            {
                if (_Delta != value)
                {
                    _Delta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _DeltaAmount;
        public double DeltaAmount
        {
            get { return _DeltaAmount; }
            set
            {
                if (_DeltaAmount != value)
                {
                    _DeltaAmount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 21-12-2020
        double _AbsDelta;
        public double AbsDelta
        {
            get { return _AbsDelta; }
            set
            {
                if (_AbsDelta != value)
                {
                    _AbsDelta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 21-12-2020
        double _AbsGamma;
        public double AbsGamma
        {
            get { return _AbsGamma; }
            set
            {
                if (_AbsGamma != value)
                {
                    _AbsGamma = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Vega;
        public double Vega
        {
            get { return _Vega; }
            set
            {
                if (_Vega != value)
                {
                    _Vega = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Gamma;
        public double Gamma
        {
            get { return _Gamma; }
            set
            {
                if (_Gamma != value)
                {
                    _Gamma = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _VarDistribution;//added by Navin on 02-07-2019 
        public string VarDistribution
        {
            get { return _VarDistribution; }
            set
            {
                if (_VarDistribution != value)
                {
                    _VarDistribution = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 09-12-2020 for NewColumn EquityAmt
        double _EquityAmt;
        public double EquityAmount
        {
            get { return _EquityAmt; }
            set
            {
                if (_EquityAmt != value)
                {
                    _EquityAmt = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 14-12-2020 for new payin-payout column
        double _PayInPayOut;
        public double PayInPayOut
        {
            get { return _PayInPayOut; }
            set
            {
                if (_PayInPayOut != value)
                {
                    _PayInPayOut = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 15-01-2021 DayNet Premium
        double _DayNetPrem;
        public double DayNetPremium
        {
            get { return _DayNetPrem; }
            set
            {
                if (_DayNetPrem != value)
                {
                    _DayNetPrem = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 15-01-2021 DayNet Premium
        double _DayPrem;
        public double DayPremium
        {
            get { return _DayPrem; }
            set
            {
                if (_DayPrem != value)
                {
                    _DayPrem = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 15-01-2021 DayNet Premium
        double _DayNetPremCDS;
        public double DayNetPremiumCDS
        {
            get { return _DayNetPremCDS; }
            set
            {
                if (_DayNetPremCDS != value)
                {
                    _DayNetPremCDS = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 15-01-2021 DayNet Premium
        double _DayPremCDS;
        public double DayPremiumCDS
        {
            get { return _DayPremCDS; }
            set
            {
                if (_DayPremCDS != value)
                {
                    _DayPremCDS = value;
                    NotifyPropertyChanged();
                }
            }
        }



        //Added by Akshay on 22-12-2020 For Priority Column
        double _Priority = 999;
        public double Priority
        {
            get { return _Priority; }
            set
            {
                if (_Priority != value)
                {
                    _Priority = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 31-12-2020 For VAREQ
        double _VARMargin;
        public double VARMargin
        {
            get { return _VARMargin; }
            set
            {
                if (_VARMargin != value)
                {
                    _VARMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Nikhil on 11JAN2022 for T-1 quantity
        long _T1Quantity;
        public long T1Quantity
        {
            get { return _T1Quantity; }
            set
            {
                if (_T1Quantity != value)
                {
                    _T1Quantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Nikhil on 11JAN2022 for T-2 quantity
        long _T2Quantity;
        public long T2Quantity
        {
            get { return _T2Quantity; }
            set
            {
                if (_T2Quantity != value)
                {
                    _T2Quantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Nikhil on 11JAN2022 for EarlyPayIn quantity
        long _EarlyPayIn;
        public long EarlyPayIn
        {
            get { return _EarlyPayIn; }
            set
            {
                if (_EarlyPayIn != value)
                {
                    _EarlyPayIn = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 24-03-2021 for DeliveryMargin
        double _DeliveryMargin;
        public double DeliveryMargin
        {
            get { return _DeliveryMargin; }
            set
            {
                if (_DeliveryMargin != value)
                {
                    _DeliveryMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // Added by Snehadri on 29Sep2021
        double _Turnover;
        public double Turnover
        {
            get { return _Turnover; }
            set
            {
                if (_Turnover != value)
                {
                    _Turnover = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _BankniftyExpoOPT;
        public double BankniftyExpoOPT
        {
            get { return _BankniftyExpoOPT; }
            set
            {
                if (_BankniftyExpoOPT != value)
                {
                    _BankniftyExpoOPT = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _NiftyExpoOPT;
        public double NiftyExpoOPT
        {
            get { return _NiftyExpoOPT; }
            set
            {
                if (_NiftyExpoOPT != value)
                {
                    _NiftyExpoOPT = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MarginLimitUtil;
        public double MarginLimitUtil
        {
            get { return _MarginLimitUtil; }
            set
            {
                if (_MarginLimitUtil != value)
                {
                    _MarginLimitUtil = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MTMLimitUtil;
        public double MTMLimitUtil
        {
            get { return _MTMLimitUtil; }
            set
            {
                if (_MTMLimitUtil != value)
                {
                    _MTMLimitUtil = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _VARLimitUtil;
        public double VARLimitUtil
        {
            get { return _VARLimitUtil; }
            set
            {
                if (_VARLimitUtil != value)
                {
                    _VARLimitUtil = value;
                    NotifyPropertyChanged();
                }
            }
        }


        double _MarginLimit;
        public double MarginLimit
        {
            get { return _MarginLimit; }
            set
            {
                if (_MarginLimit != value)
                {
                    _MarginLimit = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MTMLimit;
        public double MTMLimit
        {
            get { return _MTMLimit; }
            set
            {
                if (_MTMLimit != value)
                {
                    _MTMLimit = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _VARLimit;
        public double VARLimit
        {
            get { return _VARLimit; }
            set
            {
                if (_VARLimit != value)
                {
                    _VARLimit = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _BankniftyExpoLimit;
        public double BankniftyExpoLimit
        {
            get { return _BankniftyExpoLimit; }
            set
            {
                if(_BankniftyExpoLimit != value)
                {
                    _BankniftyExpoLimit = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _NiftyExpoLimit;
        public double NiftyExpoLimit
        {
            get { return _NiftyExpoLimit; }
            set
            {
                if (_NiftyExpoLimit != value)
                {
                    _NiftyExpoLimit = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _CollateralQty;
        public long CollateralQty
        {
            get { return _CollateralQty; }
            set
            {
                if (_CollateralQty != value)
                {
                    _CollateralQty = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _CollateralValue;
        public double CollateralValue
        {
            get { return _CollateralValue; }
            set
            {
                if (_CollateralValue != value)
                {
                    _CollateralValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _CollateralHaircut;
        public double CollateralHaircut
        {
            get { return _CollateralHaircut; }
            set
            {
                if (_CollateralHaircut != value)
                {
                    _CollateralHaircut = value;
                    NotifyPropertyChanged();
                }
            }
        }


        double _CDSIntradayMTM;
        public double CDSIntradayMTM
        {
            get { return _CDSIntradayMTM; }
            set
            {
                if (_CDSIntradayMTM != value)
                {
                    _CDSIntradayMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _CDSIntradayOptionsMTM;
        public double CDSIntradayOptionsMTM
        {
            get { return _CDSIntradayOptionsMTM; }
            set
            {
                if (_CDSIntradayOptionsMTM != value)
                {
                    _CDSIntradayOptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _CDSIntradayFuturesMTM;
        public double CDSIntradayFuturesMTM
        {
            get { return _CDSIntradayFuturesMTM; }
            set
            {
                if (_CDSIntradayFuturesMTM != value)
                {
                    _CDSIntradayFuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _CDSFuturesMTM;
        public double CDSFuturesMTM
        {
            get { return _CDSFuturesMTM; }
            set
            {
                if (_CDSFuturesMTM != value)
                {
                    _CDSFuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _CDSOptionsMTM;
        public double CDSOptionsMTM
        {
            get { return _CDSOptionsMTM; }
            set
            {
                if (_CDSOptionsMTM != value)
                {
                    _CDSOptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _CDSMTM;
        public double CDSMTM
        {
            get { return _CDSMTM; }
            set
            {
                if (_CDSMTM != value)
                {
                    _CDSMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //Added by Akshay on 31-12-2021 for CDS
        double _CDSSpan;
        public double CDSSpan
        {
            get { return _CDSSpan; }
            set
            {
                if (_CDSSpan != value)
                {
                    _CDSSpan = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _CDSExposure;
        public double CDSExposure
        {
            get { return _CDSExposure; }
            set
            {
                if (_CDSExposure != value)
                {
                    _CDSExposure = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _CDSMarginUtil;
        public double CDSMarginUtil
        {
            get { return _CDSMarginUtil; }
            set
            {
                if (_CDSMarginUtil != value)
                {
                    _CDSMarginUtil = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _CDSPeakMargin = 0;
        public double CDSPeakMargin
        {
            get { return _CDSPeakMargin; }
            set
            {
                if (_CDSPeakMargin != value)
                {
                    _CDSPeakMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }



        double _CDSEODMargin;//24-12-2019
        public double CDSEODMargin
        {
            get { return _CDSEODMargin; }
            set
            {
                if (_CDSEODMargin != value)
                {
                    _CDSEODMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 10-12-2020 for ExpirySpan
        double _CDSExpMargin;//24-12-2019
        public double CDSExpiryMargin
        {
            get { return _CDSExpMargin; }
            set
            {
                if (_CDSExpMargin != value)
                {
                    _CDSExpMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 27NOV2020 by Amey
        double _CDSMarginDifference;
        public double CDSMarginDifference
        {
            get { return _CDSMarginDifference; }
            set
            {
                if (_CDSMarginDifference != value)
                {
                    _CDSMarginDifference = value;
                    NotifyPropertyChanged();
                }
            }
        }



        BindingList<CPUnderlying> _bList_Underlying;
        public BindingList<CPUnderlying> bList_Underlying
        {
            get { return _bList_Underlying; }
            set
            {
                if (_bList_Underlying != value)
                {
                    _bList_Underlying = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    public class CPUnderlying : INotifyPropertyChanged
    {
        string _ClientID;
        public string ClientID
        {
            get { return _ClientID; }
            set
            {
                if (_ClientID != value)
                {
                    _ClientID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Underlying;
        public string Underlying
        {
            get { return _Underlying; }
            set
            {
                if (_Underlying != value)
                {
                    _Underlying = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _FuturesMTM;
        public double FuturesMTM
        {
            get { return _FuturesMTM; }
            set
            {
                if (_FuturesMTM != value)
                {
                    _FuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _OptionsMTM;
        public double OptionsMTM
        {
            get { return _OptionsMTM; }
            set
            {
                if (_OptionsMTM != value)
                {
                    _OptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _EquityMTM;
        public double EquityMTM
        {
            get { return _EquityMTM; }
            set
            {
                if (_EquityMTM != value)
                {
                    _EquityMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MTM;
        public double MTM
        {
            get { return _MTM; }
            set
            {
                if (_MTM != value)
                {
                    _MTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayFuturesMTM;
        public double IntradayFuturesMTM
        {
            get { return _IntradayFuturesMTM; }
            set
            {
                if (_IntradayFuturesMTM != value)
                {
                    _IntradayFuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayOptionsMTM;
        public double IntradayOptionsMTM
        {
            get { return _IntradayOptionsMTM; }
            set
            {
                if (_IntradayOptionsMTM != value)
                {
                    _IntradayOptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayEquityMTM;
        public double IntradayEquityMTM
        {
            get { return _IntradayEquityMTM; }
            set
            {
                if (_IntradayEquityMTM != value)
                {
                    _IntradayEquityMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 28OCT2020 by Amey
        double _IntradayMTM;
        public double IntradayMTM
        {
            get { return _IntradayMTM; }
            set
            {
                if (_IntradayMTM != value)
                {
                    _IntradayMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntrinsicMTM;
        public double IntrinsicMTM
        {
            get { return _IntrinsicMTM; }
            set
            {
                if (_IntrinsicMTM != value)
                {
                    _IntrinsicMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _NPL = 0;
        public double NPL
        {
            get { return _NPL; }
            set
            {
                if (_NPL != value)
                {
                    _NPL = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _TheoreticalMTM;
        public double TheoreticalMTM
        {
            get { return _TheoreticalMTM; }
            set
            {
                if (_TheoreticalMTM != value)
                {
                    _TheoreticalMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _ROV;
        public double ROV
        {
            get { return _ROV; }
            set
            {
                if (_ROV != value)
                {
                    _ROV = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _VAR;          //changed on 19-06-18 by shrii
        public double VAR
        {
            get { return _VAR; }
            set
            {
                if (_VAR != value)
                {
                    _VAR = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario1;          //changed on 19-06-18 by shrii
        public double Scenario1
        {
            get { return _Scenario1; }
            set
            {
                if (_Scenario1 != value)
                {
                    _Scenario1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario2;          //changed on 19-06-18 by shrii
        public double Scenario2
        {
            get { return _Scenario2; }
            set
            {
                if (_Scenario2 != value)
                {
                    _Scenario2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario3;          //changed on 19-06-18 by shrii
        public double Scenario3
        {
            get { return _Scenario3; }
            set
            {
                if (_Scenario3 != value)
                {
                    _Scenario3 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario4;          //changed on 19-06-18 by shrii
        public double Scenario4
        {
            get { return _Scenario4; }
            set
            {
                if (_Scenario4 != value)
                {
                    _Scenario4 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _TimeValue;
        public double TV
        {
            get { return _TimeValue; }
            set
            {
                if (_TimeValue != value)
                {
                    _TimeValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _ExpTheta;
        public double ExpTheta
        {
            get { return _ExpTheta; }
            set
            {
                if (_ExpTheta != value)
                {
                    _ExpTheta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Theta;
        public double Theta
        {
            get { return _Theta; }
            set
            {
                if (_Theta != value)
                {
                    _Theta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Delta;
        public double Delta
        {
            get { return _Delta; }
            set
            {
                if (_Delta != value)
                {
                    _Delta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _DeltaAmount;
        public double DeltaAmount
        {
            get { return _DeltaAmount; }
            set
            {
                if (_DeltaAmount != value)
                {
                    _DeltaAmount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 21-12-2020
        double _AbsDelta;
        public double AbsDelta
        {
            get { return _AbsDelta; }
            set
            {
                if (_AbsDelta != value)
                {
                    _AbsDelta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 21-12-2020
        double _AbsGamma;
        public double AbsGamma
        {
            get { return _AbsGamma; }
            set
            {
                if (_AbsGamma != value)
                {
                    _AbsGamma = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Vega;
        public double Vega
        {
            get { return _Vega; }
            set
            {
                if (_Vega != value)
                {
                    _Vega = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Gamma;
        public double Gamma
        {
            get { return _Gamma; }
            set
            {
                if (_Gamma != value)
                {
                    _Gamma = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Span;
        public double Span
        {
            get { return _Span; }
            set
            {
                if (_Span != value)
                {
                    _Span = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Exposure;
        public double Exposure
        {
            get { return _Exposure; }
            set
            {
                if (_Exposure != value)
                {
                    _Exposure = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MarginUtil;
        public double MarginUtil
        {
            get { return _MarginUtil; }
            set
            {
                if (_MarginUtil != value)
                {
                    _MarginUtil = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SnapSpan;
        public double SnapSpan
        {
            get { return _SnapSpan; }
            set
            {
                if (_SnapSpan != value)
                {
                    _SnapSpan = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SnapExposure;
        public double SnapExposure
        {
            get { return _SnapExposure; }
            set
            {
                if (_SnapExposure != value)
                {
                    _SnapExposure = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SnapMarginUtil;
        public double SnapMarginUtil
        {
            get { return _SnapMarginUtil; }
            set
            {
                if (_SnapMarginUtil != value)
                {
                    _SnapMarginUtil = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 23-03-2021
        double _DeliveryMargin;
        public double DeliveryMargin
        {
            get { return _DeliveryMargin; }
            set
            {
                if (_DeliveryMargin != value)
                {
                    _DeliveryMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 23-03-2021
        Int64 _NetposDelMargin;
        public Int64 NetposDelMargin
        {
            get { return _NetposDelMargin; }
            set
            {
                if (_NetposDelMargin != value)
                {
                    _NetposDelMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 23-03-2021
        Int64 _Obligation;
        public Int64 Obligation
        {
            get { return _Obligation; }
            set
            {
                if (_Obligation != value)
                {
                    _Obligation = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 29-06-2021 for POS EXPO
        double _PosExpoOPT;
        public double PosExpoOPT
        {
            get { return _PosExpoOPT; }
            set
            {
                if (_PosExpoOPT != value)
                {
                    _PosExpoOPT = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //Added by Akshay on 29-06-2021 for POS EXPO
        double _PosExpoFUT;
        public double PosExpoFUT
        {
            get { return _PosExpoFUT; }
            set
            {
                if (_PosExpoFUT != value)
                {
                    _PosExpoFUT = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 29-06-2021 for Net Qnty
        Int64 _NetQnty;
        public Int64 NetQnty
        {
            get { return _NetQnty; }
            set
            {
                if (_NetQnty != value)
                {
                    _NetQnty = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // Added by Snehadri on 29Sep2021
        double _Turnover;
        public double Turnover
        {
            get { return _Turnover; }
            set
            {
                if (_Turnover != value)
                {
                    _Turnover = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 27-07-2021 for Window
        string _Analysis;
        public string Analysis
        {
            get { return _Analysis; }
            set
            {
                if (_Analysis != value)
                {
                    _Analysis = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _CollateralQty;
        public long CollateralQty
        {
            get { return _CollateralQty; }
            set
            {
                if (_CollateralQty != value)
                {
                    _CollateralQty = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _CollateralValue;
        public double CollateralValue
        {
            get { return _CollateralValue; }
            set
            {
                if (_CollateralValue != value)
                {
                    _CollateralValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _CollateralHaircut;
        public double CollateralHaircut
        {
            get { return _CollateralHaircut; }
            set
            {
                if (_CollateralHaircut != value)
                {
                    _CollateralHaircut = value;
                    NotifyPropertyChanged();
                }
            }
        }


        BindingList<CPPositions> _bList_Positions;
        public BindingList<CPPositions> bList_Positions
        {
            get { return _bList_Positions; }
            set
            {
                if (_bList_Positions != value)
                {
                    _bList_Positions = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    public class CPPositions : INotifyPropertyChanged
    {
        string _ClientID;
        public string ClientID
        {
            get { return _ClientID; }
            set
            {
                if (_ClientID != value)
                {
                    _ClientID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Underlying;
        public string Underlying
        {
            get { return _Underlying; }
            set
            {
                if (_Underlying != value)
                {
                    _Underlying = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Series;
        public string Series
        {
            get { return _Series; }
            set
            {
                if (_Series != value)
                {
                    _Series = value;
                    NotifyPropertyChanged();
                }
            }
        }

        en_Segment _Segment;
        public en_Segment Segment
        {
            get { return _Segment; }
            set
            {
                if (_Segment != value)
                {
                    _Segment = value;
                    NotifyPropertyChanged();
                }
            }
        }

        en_InstrumentName _InstrumentName;
        public en_InstrumentName InstrumentName
        {
            get { return _InstrumentName; }
            set
            {
                if (_InstrumentName != value)
                {
                    _InstrumentName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _ScripName;
        public string ScripName
        {
            get { return _ScripName; }
            set
            {
                if (_ScripName != value)
                {
                    _ScripName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 23NOV2020 by Amey
        int _ScripToken;
        public int ScripToken
        {
            get { return _ScripToken; }
            set
            {
                if (_ScripToken != value)
                {
                    _ScripToken = value;
                    NotifyPropertyChanged();
                }
            }
        }

        DateTime _ExpiryDate;
        public DateTime ExpiryDate
        {
            get { return _ExpiryDate; }
            set
            {
                if (_ExpiryDate != value)
                {
                    _ExpiryDate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        en_ScripType _ScripType;
        public en_ScripType ScripType
        {
            get { return _ScripType; }
            set
            {
                if (_ScripType != value)
                {
                    _ScripType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _StrikePrice;
        public double StrikePrice
        {
            get { return _StrikePrice; }
            set
            {
                if (_StrikePrice != value)
                {
                    _StrikePrice = value;
                    NotifyPropertyChanged();
                }
            }
        }

        Int64 _NetPosition;
        public Int64 NetPosition
        {
            get { return _NetPosition; }
            set
            {
                if (_NetPosition != value)
                {
                    _NetPosition = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _BEP;
        public double BEP
        {
            get { return _BEP; }
            set
            {
                if (_BEP != value)
                {
                    _BEP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        Int64 _NetPositionCF;
        public Int64 NetPositionCF
        {
            get { return _NetPositionCF; }
            set
            {
                if (_NetPositionCF != value)
                {
                    _NetPositionCF = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _PriceCF;
        public double PriceCF
        {
            get { return _PriceCF; }
            set
            {
                if (_PriceCF != value)
                {
                    _PriceCF = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _LTP;
        public double LTP
        {
            get { return _LTP; }
            set
            {
                if (_LTP != value)
                {
                    _LTP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _TheoreticalPrice;
        public double TheoreticalPrice
        {
            get { return _TheoreticalPrice; }
            set
            {
                if (_TheoreticalPrice != value)
                {
                    _TheoreticalPrice = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _UnderlyingLTP;
        public double UnderlyingLTP
        {
            get { return _UnderlyingLTP; }
            set
            {
                if (_UnderlyingLTP != value)
                {
                    _UnderlyingLTP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SpotPrice;
        public double SpotPrice
        {
            get { return _SpotPrice; }
            set
            {
                if (_SpotPrice != value)
                {
                    _SpotPrice = value;
                    NotifyPropertyChanged();
                }
            }
        }

        int _OGRange;
        public int OGRange
        {
            get { return _OGRange; }
            set
            {
                if (_OGRange != value)
                {
                    _OGRange = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _PriceAtOG;
        public double PriceAtOG
        {
            get { return _PriceAtOG; }
            set
            {
                if (_PriceAtOG != value)
                {
                    _PriceAtOG = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IV;
        public double IV
        {
            get { return _IV; }
            set
            {
                if (_IV != value)
                {
                    _IV = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _ATMIV;
        public double AtmIV
        {
            get { return _ATMIV; }
            set
            {
                if (_ATMIV != value)
                {
                    _ATMIV = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _FuturesMTM;
        public double FuturesMTM
        {
            get { return _FuturesMTM; }
            set
            {
                if (_FuturesMTM != value)
                {
                    _FuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _OptionsMTM;
        public double OptionsMTM
        {
            get { return _OptionsMTM; }
            set
            {
                if (_OptionsMTM != value)
                {
                    _OptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _EquityMTM;
        public double EquityMTM
        {
            get { return _EquityMTM; }
            set
            {
                if (_EquityMTM != value)
                {
                    _EquityMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MTM;
        public double MTM
        {
            get { return _MTM; }
            set
            {
                if (_MTM != value)
                {
                    _MTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayFuturesMTM;
        public double IntradayFuturesMTM
        {
            get { return _IntradayFuturesMTM; }
            set
            {
                if (_IntradayFuturesMTM != value)
                {
                    _IntradayFuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayOptionsMTM;
        public double IntradayOptionsMTM
        {
            get { return _IntradayOptionsMTM; }
            set
            {
                if (_IntradayOptionsMTM != value)
                {
                    _IntradayOptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayEquityMTM;
        public double IntradayEquityMTM
        {
            get { return _IntradayEquityMTM; }
            set
            {
                if (_IntradayEquityMTM != value)
                {
                    _IntradayEquityMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 28OCT2020 by Amey
        double _IntradayMTM;
        public double IntradayMTM
        {
            get { return _IntradayMTM; }
            set
            {
                if (_IntradayMTM != value)
                {
                    _IntradayMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _TheoreticalMTM;
        public double TheoreticalMTM
        {
            get { return _TheoreticalMTM; }
            set
            {
                if (_TheoreticalMTM != value)
                {
                    _TheoreticalMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntrinsicMTM;
        public double IntrinsicMTM
        {
            get { return _IntrinsicMTM; }
            set
            {
                if (_IntrinsicMTM != value)
                {
                    _IntrinsicMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _ROV;
        public double ROV
        {
            get { return _ROV; }
            set
            {
                if (_ROV != value)
                {
                    _ROV = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 30OCT2020 by Amey
        double _IntradayBEP;
        public double IntradayBEP
        {
            get { return _IntradayBEP; }
            set
            {
                if (_IntradayBEP != value)
                {
                    _IntradayBEP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 30OCT2020 by Amey
        Int64 _IntradayNetPosition;
        public Int64 IntradayNetPosition
        {
            get { return _IntradayNetPosition; }
            set
            {
                if (_IntradayNetPosition != value)
                {
                    _IntradayNetPosition = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _VAR;          //changed on 19-06-18 by shrii
        public double VAR
        {
            get { return _VAR; }
            set
            {
                if (_VAR != value)
                {
                    _VAR = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _TimeValue;
        public double TV
        {
            get { return _TimeValue; }
            set
            {
                if (_TimeValue != value)
                {
                    _TimeValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _ExpTheta;
        public double ExpTheta
        {
            get { return _ExpTheta; }
            set
            {
                if (_ExpTheta != value)
                {
                    _ExpTheta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Theta;
        public double Theta
        {
            get { return _Theta; }
            set
            {
                if (_Theta != value)
                {
                    _Theta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Delta;
        public double Delta
        {
            get { return _Delta; }
            set
            {
                if (_Delta != value)
                {
                    _Delta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _DeltaAmount;
        public double DeltaAmount
        {
            get { return _DeltaAmount; }
            set
            {
                if (_DeltaAmount != value)
                {
                    _DeltaAmount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 21-12-2020
        double _AbsDelta;
        public double AbsDelta
        {
            get { return _AbsDelta; }
            set
            {
                if (_AbsDelta != value)
                {
                    _AbsDelta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 21-12-2020
        double _AbsGamma;
        public double AbsGamma
        {
            get { return _AbsGamma; }
            set
            {
                if (_AbsGamma != value)
                {
                    _AbsGamma = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Vega;
        public double Vega
        {
            get { return _Vega; }
            set
            {
                if (_Vega != value)
                {
                    _Vega = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Gamma;
        public double Gamma
        {
            get { return _Gamma; }
            set
            {
                if (_Gamma != value)
                {
                    _Gamma = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _DaysToExpiry;
        public double DaysToExpiry
        {
            get { return _DaysToExpiry; }
            set
            {
                if (_DaysToExpiry != value)
                {
                    _DaysToExpiry = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 31-12-2020 For VAREQ
        double _VARMargin;
        public double VARMargin
        {
            get { return _VARMargin; }
            set
            {
                if (_VARMargin != value)
                {
                    _VARMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _T1Quantity;
        public long T1Quantity
        {
            get { return _T1Quantity; }
            set
            {
                if (_T1Quantity != value)
                {
                    _T1Quantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Nikhil on 11JAN2022 for T-2 quantity
        long _T2Quantity;
        public long T2Quantity
        {
            get { return _T2Quantity; }
            set
            {
                if (_T2Quantity != value)
                {
                    _T2Quantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Nikhil on 11JAN2022 for EarlyPayIn quantity
        long _EarlyPayIn;
        public long EarlyPayIn
        {
            get { return _EarlyPayIn; }
            set
            {
                if (_EarlyPayIn != value)
                {
                    _EarlyPayIn = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario1;          //changed on 19-06-18 by shrii
        public double Scenario1
        {
            get { return _Scenario1; }
            set
            {
                if (_Scenario1 != value)
                {
                    _Scenario1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario2;          //changed on 19-06-18 by shrii
        public double Scenario2
        {
            get { return _Scenario2; }
            set
            {
                if (_Scenario2 != value)
                {
                    _Scenario2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario3;          //changed on 19-06-18 by shrii
        public double Scenario3
        {
            get { return _Scenario3; }
            set
            {
                if (_Scenario3 != value)
                {
                    _Scenario3 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario4;          //changed on 19-06-18 by shrii
        public double Scenario4
        {
            get { return _Scenario4; }
            set
            {
                if (_Scenario4 != value)
                {
                    _Scenario4 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        bool _IsLTPCalculated;
        public bool IsLTPCalculated
        {
            get { return _IsLTPCalculated; }
            set
            {
                if (_IsLTPCalculated != value)
                {
                    _IsLTPCalculated = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _IntradayBuyQuantity;
        public long IntradayBuyQuantity
        {
            get { return _IntradayBuyQuantity; }
            set
            {
                if (_IntradayBuyQuantity != value)
                {
                    _IntradayBuyQuantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayBuyAvg;
        public double IntradayBuyAvg
        {
            get { return _IntradayBuyAvg; }
            set
            {
                if (_IntradayBuyAvg != value)
                {
                    _IntradayBuyAvg = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _IntradaySellQuantity;
        public long IntradaySellQuantity
        {
            get { return _IntradaySellQuantity; }
            set
            {
                if (_IntradaySellQuantity != value)
                {
                    _IntradaySellQuantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradaySellAvg;
        public double IntradaySellAvg
        {
            get { return _IntradaySellAvg; }
            set
            {
                if (_IntradaySellAvg != value)
                {
                    _IntradaySellAvg = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // Added by Snehadri on 29Sep2021
        double _Turnover;
        public double Turnover
        {
            get { return _Turnover; }
            set
            {
                if (_Turnover != value)
                {
                    _Turnover = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    //Added by Akshay on 13-06-2022 for Delivery Tab
    public class DRUnderlying : INotifyPropertyChanged
    {
        string _ClientID;
        public string ClientID
        {
            get { return _ClientID; }
            set
            {
                if (_ClientID != value)
                {
                    _ClientID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Underlying;
        public string Underlying
        {
            get { return _Underlying; }
            set
            {
                if (_Underlying != value)
                {
                    _Underlying = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 23-03-2021
        double _DeliveryMargin;
        public double DeliveryMargin
        {
            get { return _DeliveryMargin; }
            set
            {
                if (_DeliveryMargin != value)
                {
                    _DeliveryMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 23-03-2021
        Int64 _Obligation;
        public Int64 Obligation
        {
            get { return _Obligation; }
            set
            {
                if (_Obligation != value)
                {
                    _Obligation = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //Added by Akshay on 19-08-2022
        Int64 _DeliveryQty;
        public Int64 DeliveryQty
        {
            get { return _DeliveryQty; }
            set
            {
                if (_DeliveryQty != value)
                {
                    _DeliveryQty = value;
                    NotifyPropertyChanged();
                }
            }
        }



        //Added by Akshay on 19-08-2022
        Int64 _EQNetPosition;
        public Int64 EQNetPosition
        {
            get { return _EQNetPosition; }
            set
            {
                if (_EQNetPosition != value)
                {
                    _EQNetPosition = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //Added by Akshay on 19-08-2022
        Int64 _EQIntradayNetPosition;
        public Int64 EQIntradayNetPosition
        {
            get { return _EQIntradayNetPosition; }
            set
            {
                if (_EQIntradayNetPosition != value)
                {
                    _EQIntradayNetPosition = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //Added by Akshay on 19-08-2022
        Int64 _EQNetPositionCF;
        public Int64 EQNetPositionCF
        {
            get { return _EQNetPositionCF; }
            set
            {
                if (_EQNetPositionCF != value)
                {
                    _EQNetPositionCF = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //Added by Akshay on 23-03-2021
        double _EQBEP;
        public double EQBEP
        {
            get { return _EQBEP; }
            set
            {
                if (_EQBEP != value)
                {
                    _EQBEP = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //Added by Akshay on 23-03-2021
        double _EQPriceCF;
        public double EQPriceCF
        {
            get { return _EQPriceCF; }
            set
            {
                if (_EQPriceCF != value)
                {
                    _EQPriceCF = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //Added by Akshay on 23-03-2021
        double _EQIntradayBEP;
        public double EQIntradayBEP
        {
            get { return _EQIntradayBEP; }
            set
            {
                if (_EQIntradayBEP != value)
                {
                    _EQIntradayBEP = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //Added by Akshay on 23-03-2021
        double _VARMargin;
        public double VARMargin
        {
            get { return _VARMargin; }
            set
            {
                if (_VARMargin != value)
                {
                    _VARMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }


        BindingList<DRPositions> _bList_Positions;
        public BindingList<DRPositions> bList_Positions
        {
            get { return _bList_Positions; }
            set
            {
                if (_bList_Positions != value)
                {
                    _bList_Positions = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    //Added by Akshay on 13-06-2022 for Delivery Tab
    public class DRPositions : INotifyPropertyChanged
    {
        string _ClientID;
        public string ClientID
        {
            get { return _ClientID; }
            set
            {
                if (_ClientID != value)
                {
                    _ClientID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Underlying;
        public string Underlying
        {
            get { return _Underlying; }
            set
            {
                if (_Underlying != value)
                {
                    _Underlying = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Series;
        public string Series
        {
            get { return _Series; }
            set
            {
                if (_Series != value)
                {
                    _Series = value;
                    NotifyPropertyChanged();
                }
            }
        }

        en_Segment _Segment;
        public en_Segment Segment
        {
            get { return _Segment; }
            set
            {
                if (_Segment != value)
                {
                    _Segment = value;
                    NotifyPropertyChanged();
                }
            }
        }

        en_InstrumentName _InstrumentName;
        public en_InstrumentName InstrumentName
        {
            get { return _InstrumentName; }
            set
            {
                if (_InstrumentName != value)
                {
                    _InstrumentName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _ScripName;
        public string ScripName
        {
            get { return _ScripName; }
            set
            {
                if (_ScripName != value)
                {
                    _ScripName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 23NOV2020 by Amey
        int _ScripToken;
        public int ScripToken
        {
            get { return _ScripToken; }
            set
            {
                if (_ScripToken != value)
                {
                    _ScripToken = value;
                    NotifyPropertyChanged();
                }
            }
        }

        DateTime _ExpiryDate;
        public DateTime ExpiryDate
        {
            get { return _ExpiryDate; }
            set
            {
                if (_ExpiryDate != value)
                {
                    _ExpiryDate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        en_ScripType _ScripType;
        public en_ScripType ScripType
        {
            get { return _ScripType; }
            set
            {
                if (_ScripType != value)
                {
                    _ScripType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _StrikePrice;
        public double StrikePrice
        {
            get { return _StrikePrice; }
            set
            {
                if (_StrikePrice != value)
                {
                    _StrikePrice = value;
                    NotifyPropertyChanged();
                }
            }
        }

        Int64 _NetPosition;
        public Int64 NetPosition
        {
            get { return _NetPosition; }
            set
            {
                if (_NetPosition != value)
                {
                    _NetPosition = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _BEP;
        public double BEP
        {
            get { return _BEP; }
            set
            {
                if (_BEP != value)
                {
                    _BEP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        Int64 _NetPositionCF;
        public Int64 NetPositionCF
        {
            get { return _NetPositionCF; }
            set
            {
                if (_NetPositionCF != value)
                {
                    _NetPositionCF = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _PriceCF;
        public double PriceCF
        {
            get { return _PriceCF; }
            set
            {
                if (_PriceCF != value)
                {
                    _PriceCF = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _LTP;
        public double LTP
        {
            get { return _LTP; }
            set
            {
                if (_LTP != value)
                {
                    _LTP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _UnderlyingLTP;
        public double UnderlyingLTP
        {
            get { return _UnderlyingLTP; }
            set
            {
                if (_UnderlyingLTP != value)
                {
                    _UnderlyingLTP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SpotPrice;
        public double SpotPrice
        {
            get { return _SpotPrice; }
            set
            {
                if (_SpotPrice != value)
                {
                    _SpotPrice = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }


    public class UCParent : INotifyPropertyChanged
    {
        string _Underlying;
        public string Underlying
        {
            get { return _Underlying; }
            set
            {
                if (_Underlying != value)
                {
                    _Underlying = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _FuturesMTM;
        public double FuturesMTM
        {
            get { return _FuturesMTM; }
            set
            {
                if (_FuturesMTM != value)
                {
                    _FuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _OptionsMTM;
        public double OptionsMTM
        {
            get { return _OptionsMTM; }
            set
            {
                if (_OptionsMTM != value)
                {
                    _OptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _EquityMTM;
        public double EquityMTM
        {
            get { return _EquityMTM; }
            set
            {
                if (_EquityMTM != value)
                {
                    _EquityMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MTM;
        public double MTM
        {
            get { return _MTM; }
            set
            {
                if (_MTM != value)
                {
                    _MTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayFuturesMTM;
        public double IntradayFuturesMTM
        {
            get { return _IntradayFuturesMTM; }
            set
            {
                if (_IntradayFuturesMTM != value)
                {
                    _IntradayFuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayOptionsMTM;
        public double IntradayOptionsMTM
        {
            get { return _IntradayOptionsMTM; }
            set
            {
                if (_IntradayOptionsMTM != value)
                {
                    _IntradayOptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayEquityMTM;
        public double IntradayEquityMTM
        {
            get { return _IntradayEquityMTM; }
            set
            {
                if (_IntradayEquityMTM != value)
                {
                    _IntradayEquityMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 28OCT2020 by Amey
        double _IntradayMTM;
        public double IntradayMTM
        {
            get { return _IntradayMTM; }
            set
            {
                if (_IntradayMTM != value)
                {
                    _IntradayMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _TheoreticalMTM;
        public double TheoreticalMTM
        {
            get { return _TheoreticalMTM; }
            set
            {
                if (_TheoreticalMTM != value)
                {
                    _TheoreticalMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _VAR;          //changed on 19-06-18 by shrii
        public double VAR
        {
            get { return _VAR; }
            set
            {
                if (_VAR != value)
                {
                    _VAR = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Theta;
        public double Theta
        {
            get { return _Theta; }
            set
            {
                if (_Theta != value)
                {
                    _Theta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _DeltaAmount;
        public double DeltaAmount
        {
            get { return _DeltaAmount; }
            set
            {
                if (_DeltaAmount != value)
                {
                    _DeltaAmount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Delta;
        public double Delta
        {
            get { return _Delta; }
            set
            {
                if (_Delta != value)
                {
                    _Delta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Vega;
        public double Vega
        {
            get { return _Vega; }
            set
            {
                if (_Vega != value)
                {
                    _Vega = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Gamma;
        public double Gamma
        {
            get { return _Gamma; }
            set
            {
                if (_Gamma != value)
                {
                    _Gamma = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Span;
        public double Span
        {
            get { return _Span; }
            set
            {
                if (_Span != value)
                {
                    _Span = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Exposure;
        public double Exposure
        {
            get { return _Exposure; }
            set
            {
                if (_Exposure != value)
                {
                    _Exposure = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MarginUtil;
        public double MarginUtil
        {
            get { return _MarginUtil; }
            set
            {
                if (_MarginUtil != value)
                {
                    _MarginUtil = value;
                    NotifyPropertyChanged();
                }
            }
        }
        double _SnapSpan;
        public double SnapSpan
        {
            get { return _SnapSpan; }
            set
            {
                if (_SnapSpan != value)
                {
                    _SnapSpan = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SnapExposure;
        public double SnapExposure
        {
            get { return _SnapExposure; }
            set
            {
                if (_SnapExposure != value)
                {
                    _SnapExposure = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SnapMarginUtil;
        public double SnapMarginUtil
        {
            get { return _SnapMarginUtil; }
            set
            {
                if (_SnapMarginUtil != value)
                {
                    _SnapMarginUtil = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 23-03-2021
        double _DeliveryMargin;
        public double DeliveryMargin
        {
            get { return _DeliveryMargin; }
            set
            {
                if (_DeliveryMargin != value)
                {
                    _DeliveryMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 23-03-2021
        Int64 _NetposDelMargin;
        public Int64 NetposDelMargin
        {
            get { return _NetposDelMargin; }
            set
            {
                if (_NetposDelMargin != value)
                {
                    _NetposDelMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 23-03-2021
        Int64 _Obligation;
        public Int64 Obligation
        {
            get { return _Obligation; }
            set
            {
                if (_Obligation != value)
                {
                    _Obligation = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 27APR2021 by Amey
        double _PercentChange;
        public double PercentChange
        {
            get { return _PercentChange; }
            set
            {
                if (_PercentChange != value)
                {
                    _PercentChange = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 27APR2021 by Amey
        double _SpotPrice;
        public double SpotPrice
        {
            get { return _SpotPrice; }
            set
            {
                if (_SpotPrice != value)
                {
                    _SpotPrice = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //Added by Akshay on 29-06-2021 for POS EXPO
        double _PosExpoOPT;
        public double PosExpoOPT
        {
            get { return _PosExpoOPT; }
            set
            {
                if (_PosExpoOPT != value)
                {
                    _PosExpoOPT = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 29-06-2021 for POS EXPO
        double _PosExpoFUT;
        public double PosExpoFUT
        {
            get { return _PosExpoFUT; }
            set
            {
                if (_PosExpoFUT != value)
                {
                    _PosExpoFUT = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 23-07-2021 for NPL
        double _NPL;
        public double NPL
        {
            get { return _NPL; }
            set
            {
                if (_NPL != value)
                {
                    _NPL = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Turnover;       // Added by Snehadri
        public double Turnover
        {
            get { return _Turnover; }
            set
            {
                if (_Turnover != value)
                {
                    _Turnover = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario1;          //changed on 19-06-18 by shrii
        public double Scenario1
        {
            get { return _Scenario1; }
            set
            {
                if (_Scenario1 != value)
                {
                    _Scenario1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario2;          //changed on 19-06-18 by shrii
        public double Scenario2
        {
            get { return _Scenario2; }
            set
            {
                if (_Scenario2 != value)
                {
                    _Scenario2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario3;          //changed on 19-06-18 by shrii
        public double Scenario3
        {
            get { return _Scenario3; }
            set
            {
                if (_Scenario3 != value)
                {
                    _Scenario3 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario4;          //changed on 19-06-18 by shrii
        public double Scenario4
        {
            get { return _Scenario4; }
            set
            {
                if (_Scenario4 != value)
                {
                    _Scenario4 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        BindingList<UCClient> _bList_Clients;
        public BindingList<UCClient> bList_Clients
        {
            get { return _bList_Clients; }
            set
            {
                if (_bList_Clients != value)
                {
                    _bList_Clients = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    public class UCClient : INotifyPropertyChanged
    {
        string _ClientID;
        public string ClientID
        {
            get { return _ClientID; }
            set
            {
                if (_ClientID != value)
                {
                    _ClientID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Zone;
        public string Zone
        {
            get { return _Zone; }
            set
            {
                if (_Zone != value)
                {
                    _Zone = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Branch;
        public string Branch
        {
            get { return _Branch; }
            set
            {
                if (_Branch != value)
                {
                    _Branch = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Family;
        public string Family
        {
            get { return _Family; }
            set
            {
                if (_Family != value)
                {
                    _Family = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Product;
        public string Product
        {
            get { return _Product; }
            set
            {
                if (_Product != value)
                {
                    _Product = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Ledger;
        public double Ledger
        {
            get { return _Ledger; }
            set
            {
                if (_Ledger != value)
                {
                    _Ledger = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Adhoc;
        public double Adhoc
        {
            get { return _Adhoc; }
            set
            {
                if (_Adhoc != value)
                {
                    _Adhoc = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _FuturesMTM;
        public double FuturesMTM
        {
            get { return _FuturesMTM; }
            set
            {
                if (_FuturesMTM != value)
                {
                    _FuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _OptionsMTM;
        public double OptionsMTM
        {
            get { return _OptionsMTM; }
            set
            {
                if (_OptionsMTM != value)
                {
                    _OptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _EquityMTM;
        public double EquityMTM
        {
            get { return _EquityMTM; }
            set
            {
                if (_EquityMTM != value)
                {
                    _EquityMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MTM;
        public double MTM
        {
            get { return _MTM; }
            set
            {
                if (_MTM != value)
                {
                    _MTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayFuturesMTM;
        public double IntradayFuturesMTM
        {
            get { return _IntradayFuturesMTM; }
            set
            {
                if (_IntradayFuturesMTM != value)
                {
                    _IntradayFuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayOptionsMTM;
        public double IntradayOptionsMTM
        {
            get { return _IntradayOptionsMTM; }
            set
            {
                if (_IntradayOptionsMTM != value)
                {
                    _IntradayOptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayEquityMTM;
        public double IntradayEquityMTM
        {
            get { return _IntradayEquityMTM; }
            set
            {
                if (_IntradayEquityMTM != value)
                {
                    _IntradayEquityMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 28OCT2020 by Amey
        double _IntradayMTM;
        public double IntradayMTM
        {
            get { return _IntradayMTM; }
            set
            {
                if (_IntradayMTM != value)
                {
                    _IntradayMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _TheoreticalMTM;
        public double TheoreticalMTM
        {
            get { return _TheoreticalMTM; }
            set
            {
                if (_TheoreticalMTM != value)
                {
                    _TheoreticalMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _VAR;
        public double VAR
        {
            get { return _VAR; }
            set
            {
                if (_VAR != value)
                {
                    _VAR = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Span;
        public double Span
        {
            get { return _Span; }
            set
            {
                if (_Span != value)
                {
                    _Span = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Exposure;
        public double Exposure
        {
            get { return _Exposure; }
            set
            {
                if (_Exposure != value)
                {
                    _Exposure = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MarginUtil;
        public double MarginUtil
        {
            get { return _MarginUtil; }
            set
            {
                if (_MarginUtil != value)
                {
                    _MarginUtil = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SnapSpan;
        public double SnapSpan
        {
            get { return _SnapSpan; }
            set
            {
                if (_SnapSpan != value)
                {
                    _SnapSpan = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SnapExposure;
        public double SnapExposure
        {
            get { return _SnapExposure; }
            set
            {
                if (_SnapExposure != value)
                {
                    _SnapExposure = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SnapMarginUtil;
        public double SnapMarginUtil
        {
            get { return _SnapMarginUtil; }
            set
            {
                if (_SnapMarginUtil != value)
                {
                    _SnapMarginUtil = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Theta;
        public double Theta
        {
            get { return _Theta; }
            set
            {
                if (_Theta != value)
                {
                    _Theta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _DeltaAmount;
        public double DeltaAmount
        {
            get { return _DeltaAmount; }
            set
            {
                if (_DeltaAmount != value)
                {
                    _DeltaAmount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Delta;
        public double Delta
        {
            get { return _Delta; }
            set
            {
                if (_Delta != value)
                {
                    _Delta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Vega;
        public double Vega
        {
            get { return _Vega; }
            set
            {
                if (_Vega != value)
                {
                    _Vega = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Gamma;
        public double Gamma
        {
            get { return _Gamma; }
            set
            {
                if (_Gamma != value)
                {
                    _Gamma = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 09-12-2020 for NewColumn EquityAmt
        double _EquityAmt;
        public double EquityAmount
        {
            get { return _EquityAmt; }
            set
            {
                if (_EquityAmt != value)
                {
                    _EquityAmt = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 14-12-2020 for new payin-payout column
        double _PayInPayOut;
        public double PayInPayOut
        {
            get { return _PayInPayOut; }
            set
            {
                if (_PayInPayOut != value)
                {
                    _PayInPayOut = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 15-01-2021 DayNet Premium
        double _DayNetPrem;
        public double DayNetPremium
        {
            get { return _DayNetPrem; }
            set
            {
                if (_DayNetPrem != value)
                {
                    _DayNetPrem = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _DayNetPremCDS;
        public double DayNetPremiumCDS
        {
            get { return _DayNetPremCDS; }
            set
            {
                if (_DayNetPremCDS != value)
                {
                    _DayNetPremCDS = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 31-12-2020 For VAREQ
        double _VARMargin;
        public double VARMargin
        {
            get { return _VARMargin; }
            set
            {
                if (_VARMargin != value)
                {
                    _VARMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _T1Quantity;
        public long T1Quantity
        {
            get { return _T1Quantity; }
            set
            {
                if (_T1Quantity != value)
                {
                    _T1Quantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Nikhil on 11JAN2022 for T-2 quantity
        long _T2Quantity;
        public long T2Quantity
        {
            get { return _T2Quantity; }
            set
            {
                if (_T2Quantity != value)
                {
                    _T2Quantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Nikhil on 11JAN2022 for EarlyPayIn quantity
        long _EarlyPayIn;
        public long EarlyPayIn
        {
            get { return _EarlyPayIn; }
            set
            {
                if (_EarlyPayIn != value)
                {
                    _EarlyPayIn = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 24-03-2021 for DeliveryMargin
        double _DeliveryMargin;
        public double DeliveryMargin
        {
            get { return _DeliveryMargin; }
            set
            {
                if (_DeliveryMargin != value)
                {
                    _DeliveryMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //Added by Akshay on 29-06-2021 for POS EXPO
        double _PosExpoOPT;
        public double PosExpoOPT
        {
            get { return _PosExpoOPT; }
            set
            {
                if (_PosExpoOPT != value)
                {
                    _PosExpoOPT = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 29-06-2021 for POS EXPO
        double _PosExpoFUT;
        public double PosExpoFUT
        {
            get { return _PosExpoFUT; }
            set
            {
                if (_PosExpoFUT != value)
                {
                    _PosExpoFUT = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 29-06-2021 for Net Qnty
        Int64 _NetQnty;
        public Int64 NetQnty
        {
            get { return _NetQnty; }
            set
            {
                if (_NetQnty != value)
                {
                    _NetQnty = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 23-07-2021 for NPL
        double _NPL;
        public double NPL
        {
            get { return _NPL; }
            set
            {
                if (_NPL != value)
                {
                    _NPL = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario1;          //changed on 19-06-18 by shrii
        public double Scenario1
        {
            get { return _Scenario1; }
            set
            {
                if (_Scenario1 != value)
                {
                    _Scenario1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario2;          //changed on 19-06-18 by shrii
        public double Scenario2
        {
            get { return _Scenario2; }
            set
            {
                if (_Scenario2 != value)
                {
                    _Scenario2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario3;          //changed on 19-06-18 by shrii
        public double Scenario3
        {
            get { return _Scenario3; }
            set
            {
                if (_Scenario3 != value)
                {
                    _Scenario3 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Scenario4;          //changed on 19-06-18 by shrii
        public double Scenario4
        {
            get { return _Scenario4; }
            set
            {
                if (_Scenario4 != value)
                {
                    _Scenario4 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    public class CWPositions : INotifyPropertyChanged
    {
        string _ClientID;
        public string ClientID
        {
            get { return _ClientID; }
            set
            {
                if (_ClientID != value)
                {
                    _ClientID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Underlying;
        public string Underlying
        {
            get { return _Underlying; }
            set
            {
                if (_Underlying != value)
                {
                    _Underlying = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _Series;
        public string Series
        {
            get { return _Series; }
            set
            {
                if (_Series != value)
                {
                    _Series = value;
                    NotifyPropertyChanged();
                }
            }
        }

        en_Segment _Segment;
        public en_Segment Segment
        {
            get { return _Segment; }
            set
            {
                if (_Segment != value)
                {
                    _Segment = value;
                    NotifyPropertyChanged();
                }
            }
        }

        en_InstrumentName _InstrumentName;
        public en_InstrumentName InstrumentName
        {
            get { return _InstrumentName; }
            set
            {
                if (_InstrumentName != value)
                {
                    _InstrumentName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _ScripName;
        public string ScripName
        {
            get { return _ScripName; }
            set
            {
                if (_ScripName != value)
                {
                    _ScripName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 23NOV2020 by Amey
        int _ScripToken;
        public int ScripToken
        {
            get { return _ScripToken; }
            set
            {
                if (_ScripToken != value)
                {
                    _ScripToken = value;
                    NotifyPropertyChanged();
                }
            }
        }

        DateTime _ExpiryDate;
        public DateTime ExpiryDate
        {
            get { return _ExpiryDate; }
            set
            {
                if (_ExpiryDate != value)
                {
                    _ExpiryDate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        en_ScripType _ScripType;
        public en_ScripType ScripType
        {
            get { return _ScripType; }
            set
            {
                if (_ScripType != value)
                {
                    _ScripType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _StrikePrice;
        public double StrikePrice
        {
            get { return _StrikePrice; }
            set
            {
                if (_StrikePrice != value)
                {
                    _StrikePrice = value;
                    NotifyPropertyChanged();
                }
            }
        }

        Int64 _NetPosition;
        public Int64 NetPosition
        {
            get { return _NetPosition; }
            set
            {
                if (_NetPosition != value)
                {
                    _NetPosition = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _BEP;
        public double BEP
        {
            get { return _BEP; }
            set
            {
                if (_BEP != value)
                {
                    _BEP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        Int64 _NetPositionCF;
        public Int64 NetPositionCF
        {
            get { return _NetPositionCF; }
            set
            {
                if (_NetPositionCF != value)
                {
                    _NetPositionCF = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _PriceCF;
        public double PriceCF
        {
            get { return _PriceCF; }
            set
            {
                if (_PriceCF != value)
                {
                    _PriceCF = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _LTP;
        public double LTP
        {
            get { return _LTP; }
            set
            {
                if (_LTP != value)
                {
                    _LTP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _TheoreticalPrice;
        public double TheoreticalPrice
        {
            get { return _TheoreticalPrice; }
            set
            {
                if (_TheoreticalPrice != value)
                {
                    _TheoreticalPrice = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _NPL;
        public double NPL
        {
            get { return _NPL; }
            set
            {
                if (_NPL != value)
                {
                    _NPL = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _UnderlyingLTP;
        public double UnderlyingLTP
        {
            get { return _UnderlyingLTP; }
            set
            {
                if (_UnderlyingLTP != value)
                {
                    _UnderlyingLTP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _SpotPrice;
        public double SpotPrice
        {
            get { return _SpotPrice; }
            set
            {
                if (_SpotPrice != value)
                {
                    _SpotPrice = value;
                    NotifyPropertyChanged();
                }
            }
        }

        int _OGRange;
        public int OGRange
        {
            get { return _OGRange; }
            set
            {
                if (_OGRange != value)
                {
                    _OGRange = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _PriceAtOG;
        public double PriceAtOG
        {
            get { return _PriceAtOG; }
            set
            {
                if (_PriceAtOG != value)
                {
                    _PriceAtOG = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IV;
        public double IV
        {
            get { return _IV; }
            set
            {
                if (_IV != value)
                {
                    _IV = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _LiveIV;
        public double LiveIV
        {
            get { return _LiveIV; }
            set
            {
                if (_LiveIV != value)
                {
                    _LiveIV = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _ATMIV;
        public double AtmIV
        {
            get { return _ATMIV; }
            set
            {
                if (_ATMIV != value)
                {
                    _ATMIV = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _FuturesMTM;
        public double FuturesMTM
        {
            get { return _FuturesMTM; }
            set
            {
                if (_FuturesMTM != value)
                {
                    _FuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _OptionsMTM;
        public double OptionsMTM
        {
            get { return _OptionsMTM; }
            set
            {
                if (_OptionsMTM != value)
                {
                    _OptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _EquityMTM;
        public double EquityMTM
        {
            get { return _EquityMTM; }
            set
            {
                if (_EquityMTM != value)
                {
                    _EquityMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MTM;
        public double MTM
        {
            get { return _MTM; }
            set
            {
                if (_MTM != value)
                {
                    _MTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayFuturesMTM;
        public double IntradayFuturesMTM
        {
            get { return _IntradayFuturesMTM; }
            set
            {
                if (_IntradayFuturesMTM != value)
                {
                    _IntradayFuturesMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayOptionsMTM;
        public double IntradayOptionsMTM
        {
            get { return _IntradayOptionsMTM; }
            set
            {
                if (_IntradayOptionsMTM != value)
                {
                    _IntradayOptionsMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayEquityMTM;
        public double IntradayEquityMTM
        {
            get { return _IntradayEquityMTM; }
            set
            {
                if (_IntradayEquityMTM != value)
                {
                    _IntradayEquityMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 28OCT2020 by Amey
        double _IntradayMTM;
        public double IntradayMTM
        {
            get { return _IntradayMTM; }
            set
            {
                if (_IntradayMTM != value)
                {
                    _IntradayMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _TheoreticalMTM;
        public double TheoreticalMTM
        {
            get { return _TheoreticalMTM; }
            set
            {
                if (_TheoreticalMTM != value)
                {
                    _TheoreticalMTM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _ROV;
        public double ROV
        {
            get { return _ROV; }
            set
            {
                if (_ROV != value)
                {
                    _ROV = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 30OCT2020 by Amey
        double _IntradayBEP;
        public double IntradayBEP
        {
            get { return _IntradayBEP; }
            set
            {
                if (_IntradayBEP != value)
                {
                    _IntradayBEP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //added on 30OCT2020 by Amey
        Int64 _IntradayNetPosition;
        public Int64 IntradayNetPosition
        {
            get { return _IntradayNetPosition; }
            set
            {
                if (_IntradayNetPosition != value)
                {
                    _IntradayNetPosition = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _VAR;          //changed on 19-06-18 by shrii
        public double VAR
        {
            get { return _VAR; }
            set
            {
                if (_VAR != value)
                {
                    _VAR = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _TimeValue;
        public double TV
        {
            get { return _TimeValue; }
            set
            {
                if (_TimeValue != value)
                {
                    _TimeValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _ExpTheta;
        public double ExpTheta
        {
            get { return _ExpTheta; }
            set
            {
                if (_ExpTheta != value)
                {
                    _ExpTheta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Theta;
        public double Theta
        {
            get { return _Theta; }
            set
            {
                if (_Theta != value)
                {
                    _Theta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Delta;
        public double Delta
        {
            get { return _Delta; }
            set
            {
                if (_Delta != value)
                {
                    _Delta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _DeltaAmount;
        public double DeltaAmount
        {
            get { return _DeltaAmount; }
            set
            {
                if (_DeltaAmount != value)
                {
                    _DeltaAmount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 21-12-2020
        double _AbsDelta;
        public double AbsDelta
        {
            get { return _AbsDelta; }
            set
            {
                if (_AbsDelta != value)
                {
                    _AbsDelta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 21-12-2020
        double _AbsGamma;
        public double AbsGamma
        {
            get { return _AbsGamma; }
            set
            {
                if (_AbsGamma != value)
                {
                    _AbsGamma = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Vega;
        public double Vega
        {
            get { return _Vega; }
            set
            {
                if (_Vega != value)
                {
                    _Vega = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Gamma;
        public double Gamma
        {
            get { return _Gamma; }
            set
            {
                if (_Gamma != value)
                {
                    _Gamma = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _DaysToExpiry;
        public double DaysToExpiry
        {
            get { return _DaysToExpiry; }
            set
            {
                if (_DaysToExpiry != value)
                {
                    _DaysToExpiry = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 31-12-2020 For VAREQ
        double _VARMargin;
        public double VARMargin
        {
            get { return _VARMargin; }
            set
            {
                if (_VARMargin != value)
                {
                    _VARMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _T1Quantity;
        public long T1Quantity
        {
            get { return _T1Quantity; }
            set
            {
                if (_T1Quantity != value)
                {
                    _T1Quantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Nikhil on 11JAN2022 for T-2 quantity
        long _T2Quantity;
        public long T2Quantity
        {
            get { return _T2Quantity; }
            set
            {
                if (_T2Quantity != value)
                {
                    _T2Quantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Nikhil on 11JAN2022 for EarlyPayIn quantity
        long _EarlyPayIn;
        public long EarlyPayIn
        {
            get { return _EarlyPayIn; }
            set
            {
                if (_EarlyPayIn != value)
                {
                    _EarlyPayIn = value;
                    NotifyPropertyChanged();
                }
            }
        }



        bool _IsLTPCalculated;
        public bool IsLTPCalculated
        {
            get { return _IsLTPCalculated; }
            set
            {
                if (_IsLTPCalculated != value)
                {
                    _IsLTPCalculated = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _IntradayBuyQuantity;
        public long IntradayBuyQuantity
        {
            get { return _IntradayBuyQuantity; }
            set
            {
                if (_IntradayBuyQuantity != value)
                {
                    _IntradayBuyQuantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradayBuyAvg;
        public double IntradayBuyAvg
        {
            get { return _IntradayBuyAvg; }
            set
            {
                if (_IntradayBuyAvg != value)
                {
                    _IntradayBuyAvg = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _IntradaySellQuantity;
        public long IntradaySellQuantity
        {
            get { return _IntradaySellQuantity; }
            set
            {
                if (_IntradaySellQuantity != value)
                {
                    _IntradaySellQuantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _IntradaySellAvg;
        public double IntradaySellAvg
        {
            get { return _IntradaySellAvg; }
            set
            {
                if (_IntradaySellAvg != value)
                {
                    _IntradaySellAvg = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // Added by Snehadri on 29Sep2021
        double _Turnover;
        public double Turnover
        {
            get { return _Turnover; }
            set
            {
                if (_Turnover != value)
                {
                    _Turnover = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Akshay on 22-07-2021 for Cost Computing
        double _IntradayCost;
        public double IntradayCost
        {
            get { return _IntradayCost; }
            set
            {
                if (_IntradayCost != value)
                {
                    _IntradayCost = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    public class CWOptions : INotifyPropertyChanged
    {
        string _Options;
        public string Options
        {
            get { return _Options; }
            set
            {
                if (_Options != value)
                {
                    _Options = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Delta;
        public double Delta
        {
            get { return _Delta; }
            set
            {
                if (_Delta != value)
                {
                    _Delta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Theta;
        public double Theta
        {
            get { return _Theta; }
            set
            {
                if (_Theta != value)
                {
                    _Theta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Vega;
        public double Vega
        {
            get { return _Vega; }
            set
            {
                if (_Vega != value)
                {
                    _Vega = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Gamma;
        public double Gamma
        {
            get { return _Gamma; }
            set
            {
                if (_Gamma != value)
                {
                    _Gamma = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _Long;
        public long Long
        {
            get { return _Long; }
            set
            {
                if (_Long != value)
                {
                    _Long = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _Short;
        public long Short
        {
            get { return _Short; }
            set
            {
                if (_Short != value)
                {
                    _Short = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _Total;
        public long Total
        {
            get { return _Total; }
            set
            {
                if (_Total != value)
                {
                    _Total = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    public class CWFutures : INotifyPropertyChanged
    {
        string _Futures;
        public string Futures
        {
            get { return _Futures; }
            set
            {
                if (_Futures != value)
                {
                    _Futures = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _QntyCF;
        public long QntyCF
        {
            get { return _QntyCF; }
            set
            {
                if (_QntyCF != value)
                {
                    _QntyCF = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _Qnty;
        public long Qnty
        {
            get { return _Qnty; }
            set
            {
                if (_Qnty != value)
                {
                    _Qnty = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _NetQnty;
        public long NetQnty
        {
            get { return _NetQnty; }
            set
            {
                if (_NetQnty != value)
                {
                    _NetQnty = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _AvgPrice;
        public double AvgPrice
        {
            get { return _AvgPrice; }
            set
            {
                if (_AvgPrice != value)
                {
                    _AvgPrice = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    public class CWGreeks : INotifyPropertyChanged
    {
        string _Greeks;
        public string Greeks
        {
            get { return _Greeks; }
            set
            {
                if (_Greeks != value)
                {
                    _Greeks = value;
                    NotifyPropertyChanged();
                }
            }
        }


        double _DeltaAmt;
        public double DeltaAmt
        {
            get { return _DeltaAmt; }
            set
            {
                if (_DeltaAmt != value)
                {
                    _DeltaAmt = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Delta;
        public double Delta
        {
            get { return _Delta; }
            set
            {
                if (_Delta != value)
                {
                    _Delta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Theta;
        public double Theta
        {
            get { return _Theta; }
            set
            {
                if (_Theta != value)
                {
                    _Theta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Gamma;
        public double Gamma
        {
            get { return _Gamma; }
            set
            {
                if (_Gamma != value)
                {
                    _Gamma = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Vega;
        public double Vega
        {
            get { return _Vega; }
            set
            {
                if (_Vega != value)
                {
                    _Vega = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MTM;
        public double MTM
        {
            get { return _MTM; }
            set
            {
                if (_MTM != value)
                {
                    _MTM = value;
                    NotifyPropertyChanged();
                }
            }
        }


        double _Span;
        public double Span
        {
            get { return _Span; }
            set
            {
                if (_Span != value)
                {
                    _Span = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Exposure;
        public double Exposure
        {
            get { return _Exposure; }
            set
            {
                if (_Exposure != value)
                {
                    _Exposure = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Margin;
        public double Margin
        {
            get { return _Margin; }
            set
            {
                if (_Margin != value)
                {
                    _Margin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _VARMargin;
        public double VARMargin
        {
            get { return _VARMargin; }
            set
            {
                if (_VARMargin != value)
                {
                    _VARMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        long _T1Quantity;
        public long T1Quantity
        {
            get { return _T1Quantity; }
            set
            {
                if (_T1Quantity != value)
                {
                    _T1Quantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Nikhil on 11JAN2022 for T-2 quantity
        long _T2Quantity;
        public long T2Quantity
        {
            get { return _T2Quantity; }
            set
            {
                if (_T2Quantity != value)
                {
                    _T2Quantity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //Added by Nikhil on 11JAN2022 for EarlyPayIn quantity
        long _EarlyPayIn;
        public long EarlyPayIn
        {
            get { return _EarlyPayIn; }
            set
            {
                if (_EarlyPayIn != value)
                {
                    _EarlyPayIn = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private void NotifyPropertyChanged([CallerMemberName] string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    public class SpanMargin : INotifyPropertyChanged
    {
        string _Client;
        public string Client
        {
            get { return _Client; }
            set
            {
                if (_Client != value)
                {
                    _Client = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Span;
        public double Span
        {
            get { return _Span; }
            set
            {
                if (_Span != value)
                {
                    _Span = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _Exposure;
        public double Exposure
        {
            get { return _Exposure; }
            set
            {
                if (_Exposure != value)
                {
                    _Exposure = value;
                    NotifyPropertyChanged();
                }
            }
        }

        double _MarginUtil;
        public double MarginUtil
        {
            get { return _MarginUtil; }
            set
            {
                if (_MarginUtil != value)
                {
                    _MarginUtil = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    //added on 07DEC2020 by Amey
    public class MaxVaRStruct
    {
        public double MaxVaR { get; set; }
        public List<VaRInfo> list_VaRInfo { get; set; }
    }

    //added on 07DEC2020 by Amey
    public class VaRInfo
    {
        public en_Segment Segment { get; set; }
        public int Token { get; set; }
        public double VaR { get; set; }
        public int OGIdx { get; set; }
        public double IV { get; set; }
        public double PriceAtOG { get; set; }
    }

    public class Scenario
    {
        public string Name { get; set; }
        public string Update { get; set; }
        public string Delete { get; set; }

        public List<ScenarioParams> list_ScenarioParams { get; set; }
    }

    public class ScenarioParams
    {
        public string Stock { get; set; }
        public double StockJump { get; set; }
        public double IVJump { get; set; }
    }

    public class ClientLevelInfo
    {
        public double FuturesMTM { get; set; } = 0;
        public double OptionsMTM { get; set; } = 0;
        public double EquityMTM { get; set; } = 0;
        //public double MTM { get; set; } = 0;
        public double IntradayFuturesMTM { get; set; } = 0;
        public double IntradayOptionsMTM { get; set; } = 0;
        public double IntradayEquityMTM { get; set; } = 0;
        //public double IntradayMTM { get; set; } = 0;
        public double TheoreticalMTM { get; set; } = 0;
        public double IntrinsicMTM { get; set; } = 0;
        public double ROV { get; set; } = 0;
        public double Delta { get; set; } = 0;
        public double Gamma { get; set; } = 0;
        public double DeltaAmount { get; set; } = 0;
        public double TimeValue { get; set; } = 0;
        public double ExpTheta { get; set; } = 0;

        public double Theta { get; set; } = 0;
        public double Vega { get; set; } = 0;
        public double SingleDelta { get; set; } = 0;
        public double SingleGamma { get; set; } = 0;
        public double VARMargin { get; set; } = 0;
        public long T1Quantity { get; set; } = 0;     //Added On 11JAN2022 by nikhil
        public long T2Quantity { get; set; } = 0;     //Added On 11JAN2022 by nikhil
        public long EarlyPayIn { get; set; } = 0;     //Added On 11JAN2022 by nikhil

        public double EquityAmount { get; set; } = 0;
        public double VaR { get; set; } = 0;
        public Int64 CallLongQty { get; set; } = 0;        //Added by Akshay on 23-03-2021
        public Int64 PutLongQty { get; set; } = 0;     //Added by Akshay on 23-03-2021       
        public Int64 CallBuyQty { get; set; } = 0;     //Added by Akshay on 23-03-2021  
        public Int64 CallSellQty { get; set; } = 0;     //Added by Akshay on 23-03-2021  
        public Int64 PutBuyQty { get; set; } = 0;     //Added by Akshay on 23-03-2021  
        public Int64 PutSellQty { get; set; } = 0;     //Added by Akshay on 23-03-2021  
        public Int64 FutQty { get; set; } = 0;     //Added by Akshay on 23-03-2021  
        public double ValMargin { get; set; } = 0;      //Added by Akshay on 23-03-2021
        public Int64 Obligation { get; set; } = 0;      //Added by Akshay on 23-03-2021
        public double Span { get; set; } = 0;
        public double Exposure { get; set; } = 0;
        public double MarginUtil { get; set; } = 0;
        public double SnapSpan { get; set; } = 0;
        public double SnapExposure { get; set; } = 0;
        public double SnapMarginUtil { get; set; } = 0;
        public double PayInPayOut { get; set; } = 0;
        public double DayNetPremium { get; set; } = 0;
        public double DayNetPremiumCDS { get; set; } = 0;
        public Int64 NetCallQnty { get; set; } = 0;      //Added by Akshay on 29-06-2021
        public Int64 NetPutQnty { get; set; } = 0;      //Added by Akshay on 29-06-2021
        public Int64 NetFutQnty { get; set; } = 0;      //Added by Akshay on 29-06-2021
        public Int64 NetQnty { get; set; } = 0;      //Added by Akshay on 29-06-2021
        public double ClosingPrice { get; set; } = 0;      //Added by Akshay on 30-06-2021
        public double Turnover { get; set; } = 0;       //Added by Snehadri on 29SEP2021
        public double Scenario1 { get; set; } = 0;
        public double Scenario2 { get; set; } = 0;
        public double Scenario3 { get; set; } = 0;

        public double Scenario4 { get; set; } = 0;

        public long CollateralQty { get; set; } = 0;
        public double CollateralValue { get; set; } = 0;
        public double CollateralHaircut { get; set; } = 0;

        public double CDSFuturesMTM { get; set; } = 0;
        public double CDSOptionsMTM { get; set; } = 0;
        //public double MTM { get; set; } = 0;
        public double CDSIntradayFuturesMTM { get; set; } = 0;
        public double CDSIntradayOptionsMTM { get; set; } = 0;
     
    }

    public class UnderlyingLevelInfo
    {
        public double FuturesMTM { get; set; } = 0;
        public double OptionsMTM { get; set; } = 0;
        public double EquityMTM { get; set; } = 0;
        //public double MTM { get; set; } = 0;
        public double IntradayFuturesMTM { get; set; } = 0;
        public double IntradayOptionsMTM { get; set; } = 0;
        public double IntradayEquityMTM { get; set; } = 0;
        //public double IntradayMTM { get; set; } = 0;
        public double TheoreticalMTM { get; set; } = 0;
        public double IntrinsicMTM { get; set; } = 0;
        public double ROV { get; set; } = 0;
        public double Delta { get; set; } = 0;
        public double DeltaAmount { get; set; } = 0;
        public double Gamma { get; set; } = 0;
        public double TimeValue { get; set; } = 0;
        public double ExpTheta { get; set; } = 0;
        public double Theta { get; set; } = 0;
        public double Vega { get; set; } = 0;
        public double SingleDelta { get; set; } = 0;
        public double SingleGamma { get; set; } = 0;
        public double VARMargin { get; set; } = 0;
        public long T1Quantity { get; set; } = 0;     //Added On 11JAN2022 by nikhil
        public long T2Quantity { get; set; } = 0;     //Added On 11JAN2022 by nikhil
        public long EarlyPayIn { get; set; } = 0;     //Added On 11JAN2022 by nikhil

        public double EquityAmount { get; set; } = 0;
        public double Span { get; set; } = 0;
        public double Exposure { get; set; } = 0;
        public double MarginUtil { get; set; } = 0;
        public double SnapSpan { get; set; } = 0;
        public double SnapExposure { get; set; } = 0;
        public double SnapMarginUtil { get; set; } = 0;
        public Int64 CallLongQty { get; set; } = 0;        //Added by Akshay on 23-03-2021
        public Int64 PutLongQty { get; set; } = 0;     //Added by Akshay on 23-03-2021  
        public Int64 CallBuyQty { get; set; } = 0;     //Added by Akshay on 23-03-2021  
        public Int64 CallSellQty { get; set; } = 0;     //Added by Akshay on 23-03-2021  
        public Int64 PutBuyQty { get; set; } = 0;     //Added by Akshay on 23-03-2021  
        public Int64 PutSellQty { get; set; } = 0;     //Added by Akshay on 23-03-2021  
        public Int64 FutQty { get; set; } = 0;     //Added by Akshay on 23-03-2021  
        public double ValMargin { get; set; } = 0;      //Added by Akshay on 23-03-2021
        public Int64 Obligation { get; set; } = 0;      //Added by Akshay on 23-03-2021
        public double VaR { get; set; } = 0;
        public double DeliveryMargin { get; set; } = 0;
        public long NetposDelMargin { get; set; } = 0;
        public Int64 NetCallQnty { get; set; } = 0;      //Added by Akshay on 29-06-2021
        public Int64 NetPutQnty { get; set; } = 0;      //Added by Akshay on 29-06-2021
        public Int64 NetFutQnty { get; set; } = 0;      //Added by Akshay on 29-06-2021
        public Int64 NetQnty { get; set; } = 0;      //Added by Akshay on 29-06-2021
        public double ClosingPrice { get; set; } = 0;      //Added by Akshay on 30-06-2021
        public double Turnover { get; set; } = 0;       //Added by Snehadri on 29SEP2021
        public double Scenario1 { get; set; } = 0;
        public double Scenario2 { get; set; } = 0;
        public double Scenario3 { get; set; } = 0;
        public double Scenario4 { get; set; } = 0;

        public long CollateralQty { get; set; } = 0;
        public double CollateralValue { get; set; } = 0;
        public double CollateralHaircut { get; set; } = 0;

        public double CDSFuturesMTM { get; set; } = 0;
        public double CDSOptionsMTM { get; set; } = 0;
        //public double MTM { get; set; } = 0;
        public double CDSIntradayFuturesMTM { get; set; } = 0;
        public double CDSIntradayOptionsMTM { get; set; } = 0;
       
    }

    public class ClientSpanInfo
    {
        public string ClientID { get; set; }
        public double Span { get; set; }
        public double Exposure { get; set; }
        public double MarginUtil { get; set; }
    }

    public class UnderlyingSpanInfo
    {
        public string Underlying { get; set; }
        public double Span { get; set; }
        public double Exposure { get; set; }
        public double MarginUtil { get; set; }
    }

    public class NetPositionInfo
    {
        static string defStr = "-";
        static int defInt = 0;
        static double defDbl = 0;

        public string Username { get; set; } = defStr;
        public string Name { get; set; } = defStr;
        public string Zone { get; set; } = defStr;
        public string Branch { get; set; } = defStr;
        public string Family { get; set; } = defStr;
        public string Product { get; set; } = defStr;
        public en_Segment Segment { get; set; } = en_Segment.NSECM;
        public string Series { get; set; } = "-";
        public string ScripName { get; set; } = defStr;
        public int Token { get; set; } = defInt;
        public double LTP { get; set; } = -1;
        public DateTime ExpiryDate { get; set; } = new DateTime(1980, 1, 1);
        public string Underlying { get; set; } = defStr;
        public double UnderlyingLTP { get; set; } = -1;
        public double StrikePrice { get; set; } = defDbl;
        public en_ScripType ScripType { get; set; }
        public en_InstrumentName InstrumentName { get; set; }
        public double BEP { get; set; } = defDbl;
        public double IVLower { get; set; } = defDbl;
        public double IVMiddle { get; set; } = defDbl;
        public double IVHigher { get; set; } = defDbl;
        public long IntradayNetPosition { get; set; } = defInt;
        public double ATM_IV { get; set; } = defDbl;
        public long NetPositionCF { get; set; } = defInt;
        public double PriceCF { get; set; } = defDbl;
        public bool IsLTPCalculated { get; set; } = false;
        public double Delta { get; set; } = defDbl;
        public double Gamma { get; set; } = defDbl;
        public double Theta { get; set; } = defDbl;
        public double Vega { get; set; } = defDbl;
        public double DeltaAmount { get; set; } = defDbl;
        public bool IsLDO { get; set; } = false;
        public double MTM { get; set; } = defDbl;
        public long NetPosition { get; set; } = defInt;
        public double IntradayMTM { get; set; } = defDbl;
        public double IntradayBEP { get; set; } = defDbl;
        public double ROV { get; set; } = defDbl;
        public double VARMargin { get; set; } = defDbl;
        public long T1Quantity { get; set; } = defInt;     //Added On 11JAN2022 by nikhil
        public long T2Quantity { get; set; } = defInt;     //Added On 11JAN2022 by nikhil
        public long EarlyPayIn { get; set; } = defInt;     //Added On 11JAN2022 by nikhil
        public double IntrinsicMTM { get; set; } = defDbl; // Added By Snehadri on 09062022
        public double SingleDelta { get; set; } = defDbl;
        public double SingleGamma { get; set; } = defDbl;
        public double EquityAmount { get; set; } = defDbl;
        public double DayNetPremium { get; set; } = defDbl;
        public double DayPremium { get; set; } = defDbl;

        public double DayNetPremiumCDS { get; set; } = defDbl;
        public double DayPremiumCDS { get; set; } = defDbl;

        //Added by Akshay on 22-03-2021
        public double UnderlyingVARMargin { get; set; } = defDbl;
        public double SpotPrice { get; set; } = -1;
        public long IntradayBuyQuantity { get; set; } = defInt;
        public double IntradayBuyAvg { get; set; } = defDbl;
        public long IntradaySellQuantity { get; set; } = defInt;
        public double IntradaySellAvg { get; set; } = defDbl;
        public double TheoreticalPrice { get; set; } = -1;
        public double TheoreticalMTM { get; set; } = defDbl;
        // Added by Snehadri on 29Sep2021
        public double Turnover { get; set; } = defDbl;
        public string Moneyness { get; set; } = defStr;
        public double ITM_OTM_Percentage { get; set; } = defDbl;

        public double CDSMTM { get; set; } = defDbl;
        public double CDSIntradayMTM { get; set; } = defDbl;
    }

    public class ValueSigns
    {
        public int VaR { get; set; } = -1;
        public int Delta { get; set; } = 1;
        public int Gamma { get; set; } = 1;
        public int Theta { get; set; } = 1;
        public int Vega { get; set; } = 1;
        public int DeltaAmt { get; set; } = 1;
    }

    public class UpdateChangelog
    {
        public DateTime ChangelogDate { get; set; } = DateTime.Now;
        public string Changelog { get; set; } = "";
    }

    public class RuleAlert
    {
        public int detail_level { get; set; }
        public string ColumnName { get; set; }
        public List<Group> list_Groups { get; set; }
        public List<string> list_Clients { get; set; }
        public string[] arr_Scrips { get; set; }
    }

    public class Group
    {
        public string Op1 { get; set; }
        public double Value1 { get; set; }
        public string LogicalOp { get; set; }
        public string Op2 { get; set; }
        public double Value2 { get; set; }
        public string AlertMessage { get; set; }
    }

    // Added by Snehadri on 11AUG2021 for Rule Builder
    public class ComputeforRule
    {
        public static bool StartCompute = false;
    }

    public class UpDownVarRange
    {
        public string Underlying { get; set; }
        public SortedSet<int> SS_UpDownVarRange { get; set; }
    }

    public class ConcentrationRiskMargin
    {
        public string Underlying { get; set; }
        public double MarginUtil { get; set; }
        public double Percentage { get; set; }
    }


    public class LimitInfo
    {
        public double MTMLimit { get; set; } = 0;
        public double VARLimit { get; set; } = 0;
        public double MarginLimit { get; set; } = 0;
        public double BankniftyExpoLimit { get; set; } = 0;
        public double NiftyExpoLimit { get; set; } = 0;

    }
}
