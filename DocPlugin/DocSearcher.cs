using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using MainUtility;
using System.Windows.Controls;
using System.Windows;

namespace DocPlugin
{
    [Export(typeof(MainUtility.IPlugin))]
    [ExportMetadata("Extension", "doc")]
    public class DocSearcher : MainUtility.IPlugin
    {
        public List<string> searchResult { get; set; }
        public UserControl userControl { get; set; }

        private bool _isSearchStoppedByUser { get; set; }
        private SearchArguments _args;

        public DocSearcher()
        {         
        }

        public void InitPlugin(Window relativeWindow, SearchArguments args)
        {
            this._args = args;            
            userControl = new DocUserControl();
            (userControl as DocUserControl).docTitle.Text = "helllllolo";
            (userControl as DocUserControl).SearchStart  += new EventHandler(OnSearchButtonClick);
        }

        public event EventHandler SearchEnd;

        protected virtual void OnSearchEnded()
        {
            if (SearchEnd != null) SearchEnd(this, EventArgs.Empty);
        }

        private void OnSearchButtonClick(object sender, EventArgs e)
        {
            /*

            1. get params from text fields
            2. validate them
            3. do search
            4. event "Search stopped"
            5. relative window will handle it
                       
            */

            FindFilesByParams(_args);

            OnSearchEnded();

        }

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
                    FileInfo fInfo = new FileInfo(file);
                    if (this.CheckAllSearchParameters(file, _args.Attributes) && (DateTime.Compare(fInfo.CreationTime, _args.LastTime) < 0) && (fInfo.Length < _args.FileSize))
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
            searchResult = new List<string>();
            try
            {
                foreach (string dir in Directory.GetDirectories(dirPath))
                {
                    foreach (string file in Directory.GetFiles(dir))
                    {
                        FileInfo fInfo = new FileInfo(file);
                        if (this.CheckAllSearchParameters(file, _args.Attributes) && (DateTime.Compare(fInfo.CreationTime, _args.LastTime) < 0) && (fInfo.Length < _args.FileSize))
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
            if ((fileAttr.CompareTo(searchAttributes) == 0) && (file != null))
                return true;

            return false;
        }
    }
}
