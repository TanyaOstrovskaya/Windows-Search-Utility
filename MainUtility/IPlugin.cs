using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainUtility
{
    public interface IPlugin
    {
        bool FindFilesByParams(SearchArguments args);
        List<String> searchResult { get; set; }
    }

    public interface IPluginData
    {
        String Extension { get; }
    }
}
