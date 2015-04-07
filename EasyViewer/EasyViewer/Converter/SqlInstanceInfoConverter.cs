using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using EasyViewer.Dto;

namespace EasyViewer.Converter
{
    public class SqlInstanceInfoConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var connectionInfo = new SqlInstanceConnectionInfo();
            connectionInfo.IsLocalInstace = bool.Parse(values[0].ToString());
            connectionInfo.IsRemoteInstance = bool.Parse(values[1].ToString());
            connectionInfo.ServerInstance = values[2].ToString();
            connectionInfo.UserName = values[3].ToString();
            return connectionInfo;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
