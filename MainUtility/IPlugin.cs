using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;

namespace MainUtility
{
    public interface IPlugin
    {
        event EventHandler NewItemFound;

        ObservableCollection<String> searchResult { get; set; }
        UserControl userControl { get; set; }
            
        void InitPlugin(Window reletiveWindow, SearchArguments args);
        bool FindFilesByParams(SearchArguments args);
    }

    public interface IPluginData
    {
        String Extension { get; }
    }
}
