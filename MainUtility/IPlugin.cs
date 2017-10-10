using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace MainUtility
{
    public interface IPlugin
    {
        event EventHandler SearchEnd;

        List<String> searchResult { get; set; }
        UserControl userControl { get; set; }
            
        void InitPlugin(Window reletiveWindow, SearchArguments args);
        bool FindFilesByParams(SearchArguments args);
    }

    public interface IPluginData
    {
        String Extension { get; }
    }
}
