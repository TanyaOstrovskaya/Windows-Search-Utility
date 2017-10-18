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
using System.Collections.ObjectModel;
using System.Threading;
using System.ComponentModel;

namespace PluginTxt
{
    [Export(typeof(MainUtility.IPlugin))]
    [ExportMetadata("Extension", "txt")]
    public class SearcherTxt : MainUtility.IPlugin
    {
        public ObservableCollection<string> pluginSearchResultList { get; set; }
        public UserControl pluginUserControl { get; set; }
        public bool isSearchStopped { get; set; }

        public SearchArguments _args;

        public SearcherTxt() { }

        public void InitPlugin(Window relativeWindow, SearchArguments args) 
        {
            this._args = args;
            pluginUserControl = new TxtUserControl(this);       
        }

        public event EventHandler NewItemFound;

        protected virtual void OnNewItemFound()
        {
            if (NewItemFound != null) 
                NewItemFound(this, EventArgs.Empty);
        }

        public bool FindFilesByParams(SearchArguments args, BackgroundWorker worker, DoWorkEventArgs e)
        {
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }

            isSearchStopped = false;
            _args = args;
            var dirPath = args.DirPath;
            pluginSearchResultList = new ObservableCollection<string>();
            string substr = (pluginUserControl as TxtUserControl).SearchSubstrText.Trim();

            if (args.IsSearchRecursive)
                SearchDirRecursively(dirPath, substr, worker, e);
            else
                SearchDir(dirPath, substr, worker, e);

            return true;
        }

        private void SearchDir(string dirPath, string substr, BackgroundWorker worker, DoWorkEventArgs e)
        {
            try
            {
                foreach (string file in Directory.GetFiles(dirPath))
                {
                    FileInfo fInfo = new FileInfo(file);
                    if ( this.CheckAllSearchParameters(file, _args.Attributes)
                        && (DateTime.Compare(fInfo.CreationTime, _args.LastTime) < 0)
                        && (fInfo.Length < _args.FileSize) && (CheckFileContainsSubstring(file, substr))
                        && (fInfo.Extension.Equals(".txt")))
                    {
                        pluginSearchResultList.Add(file);
                        OnNewItemFound();
                    }
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    Thread.Sleep(100);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SearchDirRecursively (string dirPath, string substr, BackgroundWorker worker, DoWorkEventArgs e)
        {           
            try
            {
                foreach (string dir in Directory.GetDirectories(dirPath))
                {
                    SearchDir(dir, substr, worker, e); 
                    SearchDirRecursively(dir, substr, worker, e);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private bool CheckFileContainsSubstring (string file, string substr)
        {
            foreach (string line in File.ReadLines(file))
            {
                if (line.Contains(substr))
                    return true;                
            }
            return false;
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
