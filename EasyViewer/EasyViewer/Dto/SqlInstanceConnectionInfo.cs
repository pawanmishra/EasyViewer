using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyViewer.Dto
{
    public class SqlInstanceConnectionInfo
    {
        public bool IsLocalInstace { get; set; }
        public bool IsRemoteInstance { get; set; }
        public string ServerInstance { get; set; }
        public string UserName { get; set; }

        /// <summary>
        /// For this app, password will be represented as plain text in memory. Its a bad design but including advanced
        /// password handling is going to be an overkill for this small application.
        /// </summary>
        public string Password { get; set; }
    }
}
