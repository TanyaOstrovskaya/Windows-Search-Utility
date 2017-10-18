using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MainUtility
{
    public interface IPlugin
    {
        ObservableCollection<String> pluginSearchResultList { get; set; }
        UserControl pluginUserControl { get; set; }
        bool isSearchStopped { get; set; }

        void InitPlugin(Window reletiveWindow, SearchArguments args);
        bool FindFilesByParams(SearchArguments args, BackgroundWorker worker, DoWorkEventArgs e);

        event EventHandler NewItemFound;    
    }

    public interface IPluginData
    {
        String Extension { get; }
    }
}
