using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MainUtility
{
    public interface IPlugin
    {
        UserControl FindFilesByParams(SearchArguments args);
        List<String> searchResult { get; set; }
        bool IsSearchInProgress { get; set; }
    }

    public interface IPluginData
    {
        String Extension { get; }
    }
}
