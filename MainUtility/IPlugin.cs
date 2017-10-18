using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MainUtility
{
    public interface IPlugin
    {  
        void InitPlugin(SearchArguments args);
        bool FindFilesAsync(BackgroundWorker worker, DoWorkEventArgs e);

        ObservableCollection<String> pluginSearchResultList { get; set; }
        UserControl pluginUserControl { get; set; }    // user control provides UI to insert specific search arguments

        event EventHandler NewItemFound;        
    }

    public interface IPluginData
    {
        String Extension { get; }                       // file extention that plugin can work with
    }
}
