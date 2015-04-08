using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyViewer.Dto;

namespace EasyViewer.Model
{
    public interface IConnectionInfoService
    {
        string GetConnectionString(string dataBase);
        void InitializeConnectionString(SqlInstanceConnectionInfo connectionInfo);
        string ServerInstance { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
    }
}
