using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyViewer.Dto
{
    public class ForeignKeyMetaData
    {
        public string CurrentColumn { get; set; }
        public string CurrentTable { get; set; }
        public string ReferencedTable { get; set; }
        public string ReferencedColumn { get; set; }
        public int Value { get; set; }

        public ForeignKeyMetaData(string currentColumn, string currentTable, string referencedTable,
            string referencedColumn)
        {
            this.CurrentColumn = currentColumn;
            this.CurrentTable = currentTable;
            this.ReferencedColumn = referencedColumn;
            this.ReferencedTable = referencedTable;
        }
    }
}
