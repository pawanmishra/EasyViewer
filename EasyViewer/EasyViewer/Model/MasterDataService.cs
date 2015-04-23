using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyViewer.Model
{
    public class MasterDataService : IMasterDataService
    {
        private const string MasterDb = "master";
        private const string SysDatabase = "sys.databases";
        private const string FetchDatabaseQuery = "select name from sys.databases order by name";

        private readonly IDataService _dataService;

        public MasterDataService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<List<string>> GetAllDatabases()
        {
            return await _dataService.ExecuteQuery(MasterDb, FetchDatabaseQuery);
        }

        public async Task<List<string>> GetAllTablesForGivenDatabase(string databaseName)
        {
            return await _dataService.ExecuteQuery(databaseName,
                "SELECT Table_schema + '.'+ TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' order by TABLE_SCHEMA,TABLE_NAME");
        }

        public async Task<List<Dto.ForeignKeyMetaData>> GetKeyMetaData(string dataBase)
        {
            return await _dataService.GetKeyMetaData(dataBase);
        }
    }
}
