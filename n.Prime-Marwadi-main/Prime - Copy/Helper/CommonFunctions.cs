using DevExpress.XtraGrid.Views.Grid;
using Prime.Helper;
using System;
using System.Collections.Concurrent;

namespace Prime
{
    static class CommonFunctions
    {
        #region Datetime to Tick and Tick to Datetime Conversions

        static ConcurrentDictionary<double, DateTime> dict_ExpiryDate = new ConcurrentDictionary<double, DateTime>();
        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            if (dict_ExpiryDate.TryGetValue(timestamp, out DateTime dt_Expiry))
                return dt_Expiry;
            else
            {
                var dt_ExpiryDate = new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
                dict_ExpiryDate.TryAdd(timestamp, dt_ExpiryDate);
                return dt_ExpiryDate;
            }
        }

        static ConcurrentDictionary<DateTime, double> dict_ExpiryDateUnix = new ConcurrentDictionary<DateTime, double>();
        public static double ConvertToUnixTimestamp(DateTime date)
        {
            if (dict_ExpiryDateUnix.TryGetValue(date, out double dt_Expiry))
                return dt_Expiry;
            else
            {
                var ExpiryDateUnix = (date - new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                dict_ExpiryDateUnix.TryAdd(date, ExpiryDateUnix);
                return ExpiryDateUnix;
            }
        }

        #endregion

        #region Common

        public static void gridView_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            //added on 24MAY2021 by Amey
            if (CollectionHelper.dict_CustomDigits.TryGetValue(e.Info.Column.FieldName, out int RoundDigit))
                e.Info.DisplayText = e.Info.SummaryItem.GetFormatDisplayText("{0:N" + RoundDigit + "}", e.Info.SummaryItem.SummaryValue);
            else
                e.Info.DisplayText = e.Info.SummaryItem.GetFormatDisplayText("{0:N2}", e.Info.SummaryItem.SummaryValue);
        }

        #endregion

        #region Standard functions

        public static double dOne(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {
            return (Math.Log(UnderlyingPrice / ExercisePrice) + (Interest - Dividend + 0.5 * Math.Pow(Volatility, 2)) * Time) / (Volatility * (Math.Sqrt(Time)));
        }
        
        public static double NdOne(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {
            return Math.Exp(-(Math.Pow(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend), 2)) / 2) / (Math.Sqrt(2 * 3.14159265358979));
        }

        public static double dTwo(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {
            return dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend) - Volatility * Math.Sqrt(Time);
        }

        public static double NdTwo(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {
            return NORMSDIST(dTwo(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend));
        }

        public static double CallOption(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {
            return Math.Exp(-Dividend * Time) * UnderlyingPrice * NORMSDIST(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend)) - ExercisePrice * Math.Exp(-Interest * Time) * NORMSDIST(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend) - Volatility * Math.Sqrt(Time));
        }

        public static double PutOption(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {
            double dTwoo = -dTwo(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend);
            double dOnee = -dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend);

            return ExercisePrice * Math.Exp(-Interest * Time) * NORMSDIST(dTwoo) - Math.Exp(-Dividend * Time) * UnderlyingPrice * NORMSDIST(dOnee);
        }

        public static double CallDelta(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {

            double clldt = NORMSDIST(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend));
            return clldt;
        }

        public static double PutDelta(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {
            double putdt = NORMSDIST(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend)) - 1;
            return putdt;
        }

        public static double CallTheta(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {

            double CT = 0;
            CT = -(UnderlyingPrice * Volatility * NdOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend)) / (2 * Math.Sqrt(Time)) - Interest * ExercisePrice * Math.Exp(-Interest * (Time)) * NdTwo(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend);
            return CT / 365;
        }

        public static double Gamma(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {
            double ga = NdOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend) / (UnderlyingPrice * (Volatility * Math.Sqrt(Time)));
            return ga;
        }

        public static double Vega(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {

            //return 0.01 * UnderlyingPrice * Math.Sqrt(Time) * NdOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend);
            double vg = 0.01 * UnderlyingPrice * Math.Sqrt(Time) * NdOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend);
            return vg;
        }

        public static double PutTheta(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {

            double pt = 0;
            pt = -(UnderlyingPrice * Volatility * NdOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend)) / (2 * Math.Sqrt(Time)) + Interest * ExercisePrice * Math.Exp(-Interest * (Time)) * (1 - NdTwo(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend));
            return pt / 365;
        }

        public static double CallRho(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {

            return 0.01 * ExercisePrice * Time * Math.Exp(-Interest * Time) * NORMSDIST(dTwo(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend));
        }

        public static double PutRho(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
        {

            return -0.01 * ExercisePrice * Time * Math.Exp(-Interest * Time) * (1 - NORMSDIST(dTwo(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend)));
        }

        public static double ImpliedCallVolatility(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Target, double Volatility, int Dividend)
        {

            double high = 0;
            double low = 0;
            high = 5;
            low = 0;
            while ((high - low) > 0.0001)
            {
                if (CallOption(UnderlyingPrice, ExercisePrice, Time, Interest, (high + low) / 2, Dividend) > Target)
                {
                    high = (high + low) / 2;
                }
                else
                {
                    low = (high + low) / 2;
                }
            }
            return (high + low) / 2;
        }

        public static double ImpliedPutVolatility(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Target, double Volatility, int Dividend)
        {
            double high = 0;
            double low = 0;

            high = 5;
            low = 0;
            while ((high - low) > 0.0001)
            {
                if (PutOption(UnderlyingPrice, ExercisePrice, Time, Interest, (high + low) / 2, Dividend) > Target)
                {
                    high = (high + low) / 2;
                }
                else
                {
                    low = (high + low) / 2;
                }
            }
            return (high + low) / 2;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #endregion

        #region NormsDistribution Function
        private static double erf(double x)
        {
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;
            x = Math.Abs(x);
            double t = 1 / (1 + p * x);
            return 1 - ((((((a5 * t + a4) * t) + a3) * t + a2) * t) + a1) * t * Math.Exp(-1 * x * x);
        }

        public static double NORMSDIST(double z)
        {
            double sign = 1;
            if (z < 0) sign = -1;
            return 0.5 * (1.0 + sign * erf(Math.Abs(z) / Math.Sqrt(2)));
        }
        #endregion
    }
}
