using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using MainUtility;
using System.IO;

namespace DocPlugin
{
    [Export(typeof(MainUtility.IPlugin))]
    [ExportMetadata("Extension", "doc")]
    public partial class UserControl1 : UserControl, MainUtility.IPlugin
    {
        private SearchArguments _args;
        private bool _isSearchStoppedByUser { get; set; }
        public List<string> searchResult { get; set; }
        public bool IsSearchInProgress { get; set; }


        public UserControl1()
        {
            InitializeComponent();
        }

        public UserControl FindFilesByParams(SearchArguments args)
        {
            return this;
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            IsSearchInProgress = true;

            var dirPath = _args.DirPath;
            if (_args.IsSearchRecursive)
                SearchDirRecursively(dirPath);
            else
                SearchDir(dirPath);

            IsSearchInProgress = false;
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

