using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyViewer.Model
{
    public interface IMasterDataService
    {
        IEnumerable<string> GetAllDatabases();
        IEnumerable<string> GetAllTablesForGivenDatabase(string databaseName);
    }
}
