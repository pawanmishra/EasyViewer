using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyViewer.Dto;

namespace EasyViewer.Model
{
    public interface IDataService
    {
        Task<List<string>> ExecuteQuery(string database, string query);
        QueryData FetchQueryData(string dataBase, string tableName, string query);
        List<ForeignKeyMetaData> GetForeignKeyMetaData(string dataBase);
    }
}
