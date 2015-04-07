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
        public string CurrentTable { get ; set; }
        public string CurrentTableSchema { get; set; }
        public string ReferencedTable { get; set; }
        public string ReferencedColumn { get; set; }
        public string ReferencedTableSchema { get; set; }
        public int Value { get; set; }

        public ForeignKeyMetaData(string currentColumn, string currentTable, string currentTableSchema,
            string referencedTable, string referencedColumn, string referencedTableSchema)
        {
            this.CurrentColumn = currentColumn;
            this.CurrentTableSchema = currentTableSchema;
            this.CurrentTable = currentTableSchema + "." + currentTable;
            this.ReferencedColumn = referencedColumn;
            this.ReferencedTableSchema = referencedTableSchema;
            this.ReferencedTable = referencedTableSchema + "." + referencedTable;
        }
    }
}
