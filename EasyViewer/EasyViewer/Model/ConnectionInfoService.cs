using System.Configuration;
using EasyViewer.Dto;

namespace EasyViewer.Model
{
    public class ConnectionInfoService : IConnectionInfoService
    {
        private static bool _isRemoteConnection;
        private readonly string _localConnectionFormat = ConfigurationManager.AppSettings["LocalConnectionFormat"];
        private readonly string _remoteConnectionFormat = ConfigurationManager.AppSettings["RemoteConnectionFormat"];
        private string _serverInstance, _userName, _password;

        public string GetConnectionString(string dataBase)
        {
            if (_isRemoteConnection)
            {
                return string.Format(_remoteConnectionFormat, ServerInstance, UserName, Password, dataBase);
            }

            return string.Format(_localConnectionFormat, dataBase);
        }

        public void InitializeConnectionString(SqlInstanceConnectionInfo connectionInfo)
        {
            _isRemoteConnection = connectionInfo.IsRemoteInstance;
            if (connectionInfo.IsRemoteInstance)
            {   
                _serverInstance = connectionInfo.ServerInstance;
                _userName = connectionInfo.UserName;
                _password = connectionInfo.Password;
            }
        }

        public string ServerInstance
        {
            get { return _serverInstance; }
            set { _serverInstance = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
    }
}
