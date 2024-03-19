using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using Engine.Data_Structures;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace Engine.UI
{
    public partial class EODPositions : XtraForm
    {
        clsWriteLog _logger;

        DXMenuItem menu_ExportToCSV = new DXMenuItem();

        public EODPositions(clsWriteLog _logger, string _MySQLCon)
        {
            InitializeComponent();

            //added on 09APR2021 by Amey
            menu_ExportToCSV.Caption = "Export As CSV";
            menu_ExportToCSV.Click += Menu_ExportToCSV_Click;

            this._logger = _logger;

            ReadEODTable(_MySQLCon);
        }

        private void Menu_ExportToCSV_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog _Save = new SaveFileDialog();
                _Save.Filter = "CSV file (*.csv)|*.csv";
                DialogResult result = _Save.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string folderName = _Save.FileName;
                    gv_EODPositions.ExportToCsv(folderName);

                    XtraMessageBox.Show("Exported successfully", "Success");
                }
            }
            catch (Exception ee) { _logger.WriteLog("Menu_ExportToCSV_Click EOD : " + ee); XtraMessageBox.Show("Something went wrong. Please try again.", "Error"); }
        }

        private void ReadEODTable(string _MySQLCon)
        {
            try
            {
                //List<n.Structs.PositionInfo> list_EODPositions = new List<n.Structs.PositionInfo>();

                var dt_EOD = new ds_Engine.dt_EODPositionsDataTable();
                using (MySqlConnection myConnEod = new MySqlConnection(_MySQLCon))
                {
                    MySqlCommand myCmdEod = new MySqlCommand("sp_GetEOD", myConnEod);
                    myCmdEod.CommandType = CommandType.StoredProcedure;
                    myConnEod.Open();
                    MySqlDataAdapter dadapter = new MySqlDataAdapter(myCmdEod);

                    dadapter.Fill(dt_EOD);

                    dadapter.Dispose();
                    myConnEod.Close();
                }

                //added on 10FEB2021 by Amey
                //list_EODPositions = dt_EOD.AsEnumerable().Select(v => new PositionInfo
                //{
                //    Username = v.Username,
                //    Token = v.Token,
                //    Underlying = v.Underlying,
                //    ScripName = v.ScripName,
                //    InstrumentName = v.InstrumentName == "FUTIDX" ? en_InstrumentName.FUTIDX : (v.InstrumentName == "FUTSTK" ? en_InstrumentName.FUTSTK :
                //                        (v.InstrumentName == "OPTIDX" ? en_InstrumentName.OPTIDX : (v.InstrumentName == "OPTSTK" ? en_InstrumentName.OPTSTK :
                //                        en_InstrumentName.EQ))),
                //    Expiry = v.Expiry,
                //    ScripType = (v.ScripType == "EQ" ? en_ScripType.EQ : (v.ScripType == "XX" ? en_ScripType.XX : (v.ScripType == "CE" ? en_ScripType.CE :
                //                    en_ScripType.PE))),
                //    StrikePrice = v.StrikePrice,
                //    TradePrice = v.TradePrice,
                //    TradeQuantity = v.TradeQuantity,
                //    UnderlyingScripName = v.UnderlyingScripName,
                //    UnderlyingToken = v.UnderlyingToken
                //}).ToList();

                gc_EODPositions.DataSource = dt_EOD;
                gv_EODPositions.BestFitColumns();
            }
            catch (Exception ee)
            {
                _logger.WriteLog("ReadEODTable EODPositions : " + ee);
            }
        }

        private void gv_EODPositions_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            e.Menu.Items.Add(menu_ExportToCSV);
        }
    }
}