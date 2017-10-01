using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using MainUtility;

namespace PluginTxt
{
    [Export(typeof(MainUtility.IPlugin))]
    [ExportMetadata("Extension", "txt")]
    public class Searcher : MainUtility.IPlugin
    {
        public List<string> searchResult { get; set; }
        private bool _isSearchStoppedByUser { get; set; }
        private SearchArguments _args;

        public Searcher() { }

        public bool FindFilesByParams(SearchArguments args)
        {
            _isSearchStoppedByUser = false;
            _args = args;

            var dirPath = args.DirPath;

            if (args.IsSearchRecursive)
                SearchDirRecursively(dirPath);
            else
                SearchDir(dirPath);

            return true;
        }

        private void SearchDir(string dirPath)
        {
            searchResult = new List<string>();
            try
            {
                foreach (string file in Directory.GetFiles(dirPath))
                {
                    if (this.CheckAllSearchParameters(file, _args.Attributes))
                        searchResult.Add(file);
                    if (_isSearchStoppedByUser)
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SearchDirRecursively(string dirPath)
        {
            try
            {
                foreach (string dir in Directory.GetDirectories(dirPath))
                {
                    foreach (string file in Directory.GetFiles(dir))
                    {
                        if (CheckAllSearchParameters(file, _args.Attributes))
                            searchResult.Add(file);
                        if (_isSearchStoppedByUser)
                            return;
                    }
                    SearchDirRecursively(dir);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private bool CheckAllSearchParameters(string file, FileAttributes searchAttributes)
        {
            FileAttributes fileAttr = File.GetAttributes(file);
            if ((fileAttr.CompareTo (searchAttributes) == 0) && ( file != null ))
                return true;
       
            return false;
        }
    }
}
