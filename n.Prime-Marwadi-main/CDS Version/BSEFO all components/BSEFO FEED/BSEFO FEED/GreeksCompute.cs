using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GreeksCompute
{
    #region Greeks Functions
    public static double CallTheta(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
    {
        double CT = 0;
        CT = -(UnderlyingPrice * Volatility * NdOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend)) / (2 * Math.Sqrt(Time)) - Interest * ExercisePrice * Math.Exp(-Interest * (Time)) * NdTwo(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend);
        return CT / 365;
    }

    public static double PutTheta(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
    {
        double PT = 0;
        PT = -(UnderlyingPrice * Volatility * NdOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend)) / (2 * Math.Sqrt(Time)) + Interest * ExercisePrice * Math.Exp(-Interest * (Time)) * (1 - NdTwo(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend));
        return PT / 365;
    }

    public static double PutDelta(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
    {
        double putdt = NORMSDIST(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend)) - 1;
        return putdt;
    }

    public static double CallDelta(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
    {
        double clldt = NORMSDIST(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend));
        return clldt;
    }

    public static double Gamma(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
    {
        double ga = NdOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend) / (UnderlyingPrice * (Volatility * Math.Sqrt(Time)));
        return ga;
    }

    public static double Vega(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
    {
        double vg = 0.01 * UnderlyingPrice * Math.Sqrt(Time) * NdOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend);
        return vg;
    }

    private static double NdOne(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
    {
        return Math.Exp(-(Math.Pow(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend), 2)) / 2) / (Math.Sqrt(2 * 3.14159265358979));
    }

    private static double NdTwo(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
    {
        return NORMSDIST(dTwo(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend));
    }

    public static double ImpliedCallVolatility(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Target, double Volatility, int Dividend)
    {
        try
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
            return Math.Round(((high + low) / 2) * 100, 2);
        }
        catch (Exception ee)
        {
            //FO.logger.WriteLog("GreeksCompute - Exception occurred while calculating IV for CE " + DateTime.Now.ToString() + Environment.NewLine + ee.ToString());
            return 0;
        }
    }

    public static double ImpliedPutVolatility(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Target, double Volatility, int Dividend)
    {
        try
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
            return Math.Round(((high + low) / 2) * 100, 2);
        }
        catch (Exception ee)
        {
            //FO.logger.WriteLog("GreeksCompute - Exception occurred while calculating IV for PE " + DateTime.Now.ToString() + Environment.NewLine + ee.ToString());
            return 0;
        }
    }

    public static double CallOption(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
    {
        try
        {
            return Math.Exp(-Dividend * Time) * UnderlyingPrice * NORMSDIST(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend)) - ExercisePrice * Math.Exp(-Interest * Time) * NORMSDIST(dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend) - Volatility * Math.Sqrt(Time));
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public static double PutOption(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
    {
        try
        {
            double dTwoo = -dTwo(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend);
            double dOnee = -dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend);

            return ExercisePrice * Math.Exp(-Interest * Time) * NORMSDIST(dTwoo) - Math.Exp(-Dividend * Time) * UnderlyingPrice * NORMSDIST(dOnee);
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public static double NORMSDIST(double z)
    {

        double sign = 1;
        if (z < 0) sign = -1;
        return 0.5 * (1.0 + sign * erf(Math.Abs(z) / Math.Sqrt(2)));
    }

    public static double dOne(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
    {
        try
        {
            return (Math.Log(UnderlyingPrice / ExercisePrice) + (Interest - Dividend + 0.5 * Math.Pow(Volatility, 2)) * Time) / (Volatility * (Math.Sqrt(Time)));
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public static double dTwo(double UnderlyingPrice, double ExercisePrice, double Time, int Interest, double Volatility, int Dividend)
    {
        try
        {
            return dOne(UnderlyingPrice, ExercisePrice, Time, Interest, Volatility, Dividend) - Volatility * Math.Sqrt(Time);
        }
        catch (Exception)
        {
            return 0;
        }
    }

    private static double erf(double x)
    {
        //A&S formula 7.1.26
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
    #endregion

}
