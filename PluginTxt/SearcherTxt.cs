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
        private SearchArguments _args;

        public event EventHandler NewItemFound;
        protected virtual void OnNewItemFound()
        {
            if (NewItemFound != null)
                NewItemFound(this, EventArgs.Empty);
        }

        public SearcherTxt() { }

        public void InitPlugin (SearchArguments args) 
        {
            this._args = args;
            pluginUserControl = new TxtUserControl(this);       
        }       

        public bool FindFilesAsync (BackgroundWorker worker, DoWorkEventArgs e)
        {
            if (worker.CancellationPending)            
                e.Cancel = true;            

            var dirPath = _args.DirPath;
            pluginSearchResultList = new ObservableCollection<string>();
            string substr = (pluginUserControl as TxtUserControl).SearchSubstrText.Trim();

            if (_args.IsSearchRecursive)
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

                    if ((fInfo.Extension.Equals(".txt"))
                        && this.CheckAllSearchParameters(file, _args.Attributes)
                        && (DateTime.Compare(fInfo.CreationTime, _args.LastTime) < 0)
                        && (fInfo.Length < _args.FileSize) && (CheckFileContainsSubstring(file, substr)))
                       
                    {
                        pluginSearchResultList.Add(file);
                        OnNewItemFound();
                    }
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    Thread.Sleep(50);      // sleep makes UI thread be able to response during the search

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
