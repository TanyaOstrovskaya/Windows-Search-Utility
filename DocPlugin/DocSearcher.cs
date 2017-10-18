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
using Microsoft.Office.Core;
using Microsoft.Office.Interop;
using System.ComponentModel;
using System.Threading;

namespace DocPlugin
{
    [Export(typeof(MainUtility.IPlugin))]
    [ExportMetadata("Extension", "doc")]
    public class DocSearcher : MainUtility.IPlugin
    {
        public ObservableCollection<string> pluginSearchResultList { get; set; }
        public UserControl pluginUserControl { get; set; }      

        private SearchArguments _args;
        public string userTitle;
        public string userSubject;
        public string userAuthor;
        public string userManager;
        public string userCompany;

        public event EventHandler NewItemFound;
        protected virtual void OnNewItemFound()
        {
            if (NewItemFound != null) NewItemFound(this, EventArgs.Empty);
        }

        public DocSearcher() { }

        public void InitPlugin(SearchArguments args)
        {
            this._args = args;
            pluginUserControl = new DocUserControl(this);
        }

        public bool FindFilesAsync(BackgroundWorker worker, DoWorkEventArgs e)
        {
            if (worker.CancellationPending)
                e.Cancel = true;

            var dirPath = _args.DirPath;
            pluginSearchResultList = new ObservableCollection<string>();

            if (_args.IsSearchRecursive)
                SearchDirRecursively(dirPath, worker, e);
            else
                SearchDir(dirPath, worker, e);

            return true;
        }

        private void SearchDir(string dirPath, BackgroundWorker worker, DoWorkEventArgs e)
        {
            try
            {
                foreach (string file in Directory.GetFiles(dirPath))
                {
                    FileInfo fInfo = new FileInfo(file);
                    if ((fInfo.Extension).Equals(".doc")
                        && this.CheckAllSearchParameters(file, _args.Attributes)
                        && (DateTime.Compare(fInfo.CreationTime, _args.LastTime) < 0)
                        && (fInfo.Length < _args.FileSize)
                        && (CheckDocFileProperties(@file)))
                    {
                        pluginSearchResultList.Add(file);
                        OnNewItemFound();
                    }
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    Thread.Sleep(100);      // sleep makes UI thread be able to response during the search
                }   
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SearchDirRecursively(string dirPath, BackgroundWorker worker, DoWorkEventArgs e)
        {
            try
            {
                foreach (string dir in Directory.GetDirectories(dirPath))
                {
                    SearchDir(dir, worker, e);
                    SearchDirRecursively(dir, worker, e);
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

        private bool CheckDocFileProperties(string file)
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            object isReadOnly = false;
            object isVisible = true;
            object saveChanges = false;
            object missingValue = System.Reflection.Missing.Value;

            Dictionary<string, string> properties = new Dictionary<string, string>()
            {
                {"Title", null },
                {"Subject", null },
                {"Author", null },
                {"Manager", null },
                {"Company", null }
            };

            object fileName = file;
            wordApp.Visible = false;
            Microsoft.Office.Interop.Word.Document wordDoc =
                        wordApp.Documents.Open(ref fileName,
                        ref missingValue, ref isReadOnly,
                        ref missingValue, ref missingValue,
                        ref missingValue, ref missingValue,
                        ref missingValue, ref missingValue,
                        ref missingValue, ref missingValue,
                        ref isVisible);

            object docProps = wordDoc.BuiltInDocumentProperties;
            Type propertyType = docProps.GetType();
            List<string> propKeys = properties.Keys.ToList();

            foreach (var propKey in propKeys)
            {
                object property = propertyType.InvokeMember("Item",
                    System.Reflection.BindingFlags.Default |
                    System.Reflection.BindingFlags.GetProperty,
                    null,
                    docProps,
                    new object[] { propKey });

                Type validatedType = property.GetType();

                string propValue = validatedType.InvokeMember("Value",
                    System.Reflection.BindingFlags.Default |
                    System.Reflection.BindingFlags.GetProperty,
                    null,
                    property,
                    new object[] { }).ToString();

                properties[propKey] = propValue;           
            }

            wordDoc.Close(ref saveChanges, ref missingValue, ref missingValue);           
            wordApp.Quit(ref saveChanges, ref missingValue, ref missingValue);

            return ComparePropsWithUserInput(properties);
        }

        private bool ComparePropsWithUserInput (Dictionary<string, string> props)
        {

            if ((userTitle.Equals(props["Title"]) || userTitle.Length == 0) &&
                (userSubject.Equals(props["Subject"]) || userSubject.Length == 0) &&
                (userAuthor.Equals(props["Author"]) || userAuthor.Length == 0) &&
                (userManager.Equals(props["Manager"]) || userManager.Length == 0) &&
                (userCompany.Equals(props["Company"]) || userCompany.Length == 0) )
            {
                return true;
            } 
            else
            {
                return false;
            }
        }
    }
}


