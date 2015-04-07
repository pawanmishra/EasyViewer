using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyViewer.Dto;

namespace EasyViewer.Model
{
    public interface IViewerService
    {
        QueryData ProcessGridDoubleClick(DataGridDoubleClickCommandArgs args, List<ForeignKeyMetaData> metaData, string database);
    }
}
