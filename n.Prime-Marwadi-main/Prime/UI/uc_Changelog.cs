using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prime.UI
{
    public partial class uc_Changelog : XtraUserControl
    {
        List<UpdateChangelog> list_UpdateChangelog = new List<UpdateChangelog>();

        private uc_Changelog()
        {
            InitializeComponent();

            AddToChangelog();
            gc_Changelog.DataSource = list_UpdateChangelog;
        }

        public static uc_Changelog Instance { get; private set; }

        public static void Initialise()
        {
            if (Instance is null)
                Instance = new uc_Changelog();
        }

        private void AddToChangelog()
        {
            Dictionary<DateTime, List<string>> dict_Changelog = new Dictionary<DateTime, List<string>>();
            dict_Changelog.Add(new DateTime(2021, 06, 03), new List<string>()
            {
                "- Fixed This.",
                "- Added That."
            });

            dict_Changelog.Add(new DateTime(2021, 05, 25), new List<string>()
            {
                "- Added This.",
                "- Added That."
            });

            foreach (var _Date in dict_Changelog.Keys)
                foreach (var _Changelog in dict_Changelog[_Date])
                    list_UpdateChangelog.Add(new UpdateChangelog() { ChangelogDate = _Date, Changelog = _Changelog });
        }

        private void gc_Changelog_DataSourceChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < gv_Changelog.Columns.Count; i++)
            {
                string ColumnFieldName = gv_Changelog.Columns[i].FieldName;
                if (ColumnFieldName == nameof(UpdateChangelog.ChangelogDate))
                    gv_Changelog.Columns[i].GroupIndex = 0;
            }
        }
    }
}
