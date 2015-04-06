using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyViewer.Dto;

namespace EasyViewer.Model
{
    public interface IDataService
    {
        QueryData FetchQueryData();
        List<ForeignKeyMetaData> GetForeignKeyMetaData();
    }
}
