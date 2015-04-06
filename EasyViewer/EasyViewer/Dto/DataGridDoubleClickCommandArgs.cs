using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace EasyViewer.Dto
{
    public class DataGridDoubleClickCommandArgs
    {
        public DataGrid Grid { get; set; }
        public MouseButtonEventArgs RowEventArgs { get; set; }

        public DataGridDoubleClickCommandArgs(DataGrid grid, MouseButtonEventArgs eventArgs)
        {
            this.Grid = grid;
            this.RowEventArgs = eventArgs;
        }
    }
}
