using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyViewer.Dto
{
    public class QueryData
    {   
        public DataTable QueryDataTable { get; set; }
        public string TableName { get; set; }
        public int Counter { get; set; }

        public QueryData(DataTable table, string tableName)
        {
            this.QueryDataTable = table;
            this.TableName = tableName;
        }
    }
}
