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

        _logger = logger; //new NerveLogger(true, Convert.ToBoolean(CommonMethods.GetFromConfig("OTHER", "DEBUG-MODE")), ApplicationName: "FeedReceiver-BSEFO");
        //_logger.Initialize();
    }

    internal static object GetFromConfig(string Table, string ColumnName, int RowNum = 0) => GlobalCollections.ds_Config.Tables[Table].Rows[RowNum][ColumnName];

    internal static void ConsoleWrite(string _Message) => Console.WriteLine($"\r {DateTime.Now} : {_Message}");

    #endregion
}