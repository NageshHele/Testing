using NerveLog;
using System;
using System.IO;

internal class CommonMethods
{
    #region Supplimentary

    public static NerveLogger _logger;

    internal static void Initialise(NerveLogger logger)
    {
        var xmlString = File.ReadAllText(GlobalCollections._AppPath + @"\config.xml");
        var stringReader = new StringReader(xmlString);
        GlobalCollections.ds_Config.ReadXml(stringReader);

        InitIVValues();

        _logger = logger; //new NerveLogger(true, Convert.ToBoolean(CommonMethods.GetFromConfig("OTHER", "DEBUG-MODE")), ApplicationName: "FeedReceiver-BSEFO");
        //_logger.Initialize();
    }

    internal static object GetFromConfig(string Table, string ColumnName, int RowNum = 0) => GlobalCollections.ds_Config.Tables[Table].Rows[RowNum][ColumnName];

    internal static void ConsoleWrite(string _Message) => Console.WriteLine($"\r {DateTime.Now} : {_Message}");


    private static void InitIVValues()
    {
        try
        {
            nCalculate.flag_ComputeIV = Convert.ToBoolean(GlobalCollections.ds_Config.Tables["IV"].Rows[0]["ENABLE"].ToString());                                         //added on 22-3-20 by Amey
            nCalculate.IVINTEREST = Convert.ToInt32(GlobalCollections.ds_Config.Tables["IV"].Rows[0]["INTEREST"].ToString());                                         //added on 22-3-20 by Amey
            nCalculate.IVNOFUTINTEREST = Convert.ToDouble(GlobalCollections.ds_Config.Tables["IV"].Rows[0]["NOFUTINTEREST"].ToString());                                   //added on 30-3-20 by Amey
            nCalculate.IVDIVIDEND = Convert.ToInt32(GlobalCollections.ds_Config.Tables["IV"].Rows[0]["DIVIDEND"].ToString());                                   //added on 30-3-20 by Amey
            nCalculate.IVMAXVALUE = Convert.ToDouble(GlobalCollections.ds_Config.Tables["IV"].Rows[0]["MAX_VALUE"].ToString());                                  //added on 30-3-20 by Amey
            nCalculate.IVDEFAULTVALUE = Convert.ToDouble(GlobalCollections.ds_Config.Tables["IV"].Rows[0]["DEFAULT_VALUE"].ToString());                                  //added on 30-3-20 by Amey

            nCalculate.flag_MARKETVOL = Convert.ToBoolean(GlobalCollections.ds_Config.Tables["MARKETVOL"].Rows[0]["ENABLE"].ToString());                                         //added on 23-3-20 by Amey

            nCalculate.flag_GREEKS = Convert.ToBoolean(GlobalCollections.ds_Config.Tables["GREEKS"].Rows[0]["ENABLE"].ToString());                                         //added on 30-3-20 by Amey
            nCalculate.GREEKSINTEREST = Convert.ToInt32(GlobalCollections.ds_Config.Tables["GREEKS"].Rows[0]["INTEREST"].ToString());                                   //added on 30-3-20 by Amey
            nCalculate.GREEKSDIVIDEND = Convert.ToInt32(GlobalCollections.ds_Config.Tables["GREEKS"].Rows[0]["DIVIDEND"].ToString());                                   //added on 30-3-20 by Amey

        }
        catch (Exception ee) { _logger.Error(ee); }

    }



    #endregion
}