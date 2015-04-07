using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyViewer.Model
{
    public class MasterDataService : IMasterDataService
    {
        private const string MASTER_DB = "master";
        private const string SYS_DATABASE = "sys.databases";
        private const string FETCH_DATABASE_QUERY = "select name from sys.databases order by name";

        private readonly IDataService _dataService;

        public MasterDataService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IEnumerable<string> GetAllDatabases()
        {
            List<string> dataBases = new List<string>();
            var data = _dataService.FetchQueryData(MASTER_DB, SYS_DATABASE, FETCH_DATABASE_QUERY);
            var dataTable = data.QueryDataTable;
            var datatableRow = dataTable.Rows;

            foreach (DataRow dr in datatableRow)
            {
                dataBases.Add(dr.ItemArray[0].ToString());
            }

            return dataBases;
        }

        public IEnumerable<string> GetAllTablesForGivenDatabase(string databaseName)
        {
            List<string> tables = new List<string>();
            var data = _dataService.FetchQueryData(databaseName, "INFORMATION_SCHEMA.TABLES", 
                "SELECT Table_schema + '.'+ TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'");
            var dataTable = data.QueryDataTable;
            var datatableRow = dataTable.Rows;
            foreach (DataRow dr in datatableRow)
            {

                tables.Add(dr.ItemArray[0].ToString());
            }

            return tables;
        }
    }
}
