using Feed_Receiver_BSE.Data_Structures;
using NerveLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Feed_Receiver_BSE.Helper
{
    internal class CollectionHelper
    {
        private static DataSet ds_Config = new DataSet();

        internal static NerveLogger _logger;

        #region Flags

        internal static bool IsDebug = false;

        #endregion

        #region Lists

        /// <summary>
        /// DataSource for gc_ConnectionInfo.
        /// </summary>
        internal static BindingList<ConnectionInfo> bList_ConnectionInfo = new BindingList<ConnectionInfo>();

        #endregion

        #region Dictionaries

        /// <summary>
        /// Key : Token | Value : Last packet of any token will be stored here.
        /// </summary>
        internal static ConcurrentDictionary<int, Packet> dict_LastPacket = new ConcurrentDictionary<int, Packet>();

        ///<summary>
        ///
        ///</summary>
        internal static ConcurrentDictionary<string, int> dict_IndexToken = new ConcurrentDictionary<string,int>();

        /// <summary>
        /// Key : Token | Value : ID of Subscribed clients for the Tokens.
        /// </summary>
        internal static ConcurrentDictionary<int, HashSet<string>> dict_SubscribedClients = new ConcurrentDictionary<int, HashSet<string>>();

        /// <summary>
        /// Key : ID of Subscribed clients for the Tokens | Value : UDPClient Object
        /// </summary>
        internal static ConcurrentDictionary<string, SocketInfo> dict_UDPClientSocket = new ConcurrentDictionary<string, SocketInfo>();

        #endregion

        /// <summary>
        /// Lock for any operation on bList_ConnectionInfo.
        /// </summary>
        internal static readonly object _GridLock = new object();

        private CollectionHelper() { }

        internal static void Initialise()
        {
            var xmlString = File.ReadAllText(Application.StartupPath + @"\config.xml");
            var stringReader = new StringReader(xmlString);
            ds_Config.ReadXml(stringReader);

            IsDebug = Convert.ToBoolean(GetFromConfig("FLAGS", "DEBUG-MODE"));
            _logger = new NerveLogger(true, IsDebug, ApplicationName: "FR-BSE");
            _logger.Initialize(Application.StartupPath);
        }

        internal static object GetFromConfig(string Table, string ColumnName, int RowNum = 0)
        {
            return ds_Config.Tables[Table].Rows[RowNum][ColumnName];
        }
    }
}
