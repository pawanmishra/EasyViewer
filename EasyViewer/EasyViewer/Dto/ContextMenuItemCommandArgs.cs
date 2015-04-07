using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyViewer.Dto
{
    public class ContextMenuItemCommandArgs
    {
        public MenuItem MenuItem { get; set; }
        public RoutedEventArgs RoutedEventArgs { get; set; }

        public ContextMenuItemCommandArgs(MenuItem grid, RoutedEventArgs eventArgs)
        {
            this.MenuItem = grid;
            this.RoutedEventArgs = eventArgs;
        }
    }
}
