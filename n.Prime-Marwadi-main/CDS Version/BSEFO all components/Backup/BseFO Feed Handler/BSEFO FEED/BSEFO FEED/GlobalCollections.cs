using NerveLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Sockets;
using System.Reflection;

public class GlobalCollections
{
    #region Objects

    public static NerveLogger _logger;

    #endregion

    #region Dictionaries

    /// <summary>
    /// Key : Token | Value : Packet
    /// </summary>
    public static ConcurrentDictionary<int, Packet> dict_LastPacket = new ConcurrentDictionary<int, Packet>();

    /// <summary>
    /// Key : Token | Value : Packet
    /// </summary>
    public static ConcurrentDictionary<string, int> dict_tokenScripname = new ConcurrentDictionary<string, int>();
    /// <summary>
    /// Key : Token | Value : Clients Subscribed Sockets
    /// </summary>
    public static ConcurrentDictionary<int, List<Socket>> dict_SubscribedClients = new ConcurrentDictionary<int, List<Socket>>();

    /// <summary>
    /// Key : ClientID | Value : ConnectionInfo
    /// </summary>
    public static ConcurrentDictionary<string, ConnectionInfo> dict_ConnectionInfo = new ConcurrentDictionary<string, ConnectionInfo>();

    #endregion

    #region DataSets

    public static DataSet ds_Config = new DataSet();

    #endregion

    #region Variables

    #region DateTime

    /// <summary>
    /// Will be updated on every tick.
    /// </summary>
    public static DateTime dte_LTT = DateTime.Now;

    #endregion

    #region Int

    internal static int MAXALLOWEDCONNECTIONS = 1;

    #endregion

    #region String

    public static string _AppPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

    public static string _Seperator = "^";

    public static string _EOF = "<EOF>";

    #endregion

    #endregion

    #region Initialisation

    private GlobalCollections() { }

    //public static void Initialise()
    //{
    //    if (Instance is null)
    //        lock (_ObjLock)
    //            if (Instance is null)
    //            {
    //                Instance = new GlobalCollections();

    //                CommonMethods.Initialise();
    //            }
    //}

    #endregion

    #region Public Requests

    //public string GetAppPath() => _AppPath;

    //public (string, string) GetSendStringItems() => (_Seperator, _EOF);

    //public DataSet GetConfigDataSet() => ds_Config;

    //public NerveLogger GetLogger() => _logger;

    //public DateTime GetLTT() => dte_LTT;

    //public int GetMaxConnectionsAllowed() => MAXALLOWEDCONNECTIONS;

    //public ConcurrentDictionary<int, Packet> GetLastPacketDictionary() => dict_LastPacket;

    //public ConcurrentDictionary<int, List<Socket>> GetSubscribedClientsDictionary() => dict_SubscribedClients;
    //public ConcurrentDictionary<string, ConnectionInfo> GetConnectionInfoDictionary() => dict_ConnectionInfo;

    #endregion
}