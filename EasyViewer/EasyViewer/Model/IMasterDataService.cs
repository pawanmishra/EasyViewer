using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyViewer.Dto;

namespace EasyViewer.Model
{
    public interface IMasterDataService
    {
        Task<List<string>> GetAllDatabases();
        Task<List<string>> GetAllTablesForGivenDatabase(string databaseName);
        Task<List<ForeignKeyMetaData>> GetKeyMetaData(string dataBase);
    }
}
