using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EasyViewer.Dto
{
    public class DataGridAutoGenerateCommandArgs
    {
        public DataGrid Grid { get; set; }
        public DataGridAutoGeneratingColumnEventArgs ColumnEventArgs { get; set; }

        public DataGridAutoGenerateCommandArgs(DataGrid grid, DataGridAutoGeneratingColumnEventArgs eventArgs)
        {
            this.Grid = grid;
            this.ColumnEventArgs = eventArgs;
        }
    }
}
