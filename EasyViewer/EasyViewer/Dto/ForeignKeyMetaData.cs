using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyViewer.Dto
{
    public class ForeignKeyMetaData
    {
        private const string VarcharQueryFormat = "select * from {0} where {1} = '{2}'";
        private const string IntQueryFormat = "select * from {0} where {1} = {2}";
        private string SqlNumericDataTypes = ConfigurationManager.AppSettings["SqlServerNumericDataType"];

        public string CurrentColumn { get; set; }
        public string CurrentTable { get ; set; }
        public string CurrentTableSchema { get; set; }
        public string ReferencedTable { get; set; }
        public string ReferencedColumn { get; set; }
        public string ReferencedTableSchema { get; set; }
        public string DataType { get; set; }
        public int Value { get; set; }

        public ForeignKeyMetaData(string currentColumn, string currentTable, string currentTableSchema,
            string referencedTable, string referencedColumn, string referencedTableSchema, string dataType)
        {
            this.CurrentColumn = currentColumn;
            this.CurrentTableSchema = currentTableSchema;
            this.CurrentTable = currentTableSchema + "." + currentTable;
            this.ReferencedColumn = referencedColumn;
            this.ReferencedTableSchema = referencedTableSchema;
            this.ReferencedTable = referencedTableSchema + "." + referencedTable;
            this.DataType = dataType;
        }

        public string GetTargetSqlQuery(object columnValue)
        {
            if (SqlNumericDataTypes.Contains(this.DataType))
            {
                return string.Format(IntQueryFormat, this.ReferencedTable, this.ReferencedColumn, columnValue);
            }

            return string.Format(VarcharQueryFormat, this.ReferencedTable, this.ReferencedColumn, columnValue);
        }
    }
}
