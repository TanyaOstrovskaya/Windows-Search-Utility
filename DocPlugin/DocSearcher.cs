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


namespace DocPlugin
{
    [Export(typeof(MainUtility.IPlugin))]
    [ExportMetadata("Extension", "doc")]
    public class DocSearcher : MainUtility.IPlugin
    {
        public ObservableCollection<string> searchResult { get; set; }
        public UserControl userControl { get; set; }

        private bool _isSearchStoppedByUser { get; set; }
        private SearchArguments _args;

        public DocSearcher()
        {

        }

        public void InitPlugin(System.Windows.Window relativeWindow, SearchArguments args)
        {
            this._args = args;
            userControl = new DocUserControl();
            (userControl as DocUserControl).SearchStart += new EventHandler(OnSearchButtonClick);
        }

        public event EventHandler NewItemFound;

        protected virtual void OnNewItemFound()
        {
            if (NewItemFound != null) NewItemFound(this, EventArgs.Empty);
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

        }

        public bool FindFilesByParams(SearchArguments args)
        {

            CheckDocFileProperties(@"C:\Users\user\Documents\Учеба\cv.doc");

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
            searchResult = new ObservableCollection<string>();

            try
            {
                foreach (string file in Directory.GetFiles(dirPath))
                {
                    FileInfo fInfo = new FileInfo(file);
                    if (this.CheckAllSearchParameters(file, _args.Attributes) && (DateTime.Compare(fInfo.CreationTime, _args.LastTime) < 0) && (fInfo.Length < _args.FileSize))
                    {
                        searchResult.Add(file);
                        OnNewItemFound();
                    }
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
            searchResult = new ObservableCollection<string>();
            try
            {
                foreach (string dir in Directory.GetDirectories(dirPath))
                {
                    foreach (string file in Directory.GetFiles(dir))
                    {
                        FileInfo fInfo = new FileInfo(file);
                        if (this.CheckAllSearchParameters(file, _args.Attributes) && (DateTime.Compare(fInfo.CreationTime, _args.LastTime) < 0) && (fInfo.Length < _args.FileSize))
                        {
                            searchResult.Add(file);
                            OnNewItemFound();
                        }
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
            string userTitle = (userControl as DocUserControl).docTitle.Text;
            string userSubject = (userControl as DocUserControl).docSubject.Text;
            string userAuthor = (userControl as DocUserControl).docAuthor.Text;
            string userManager = (userControl as DocUserControl).docManager.Text;
            string userCompany = (userControl as DocUserControl).docOrganization.Text;

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


