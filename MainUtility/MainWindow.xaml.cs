using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Security.Permissions;
using System.Threading;

namespace MainUtility
{
    public partial class MainWindow : Window
    {
        [ImportMany(AllowRecomposition = true)]
        IEnumerable<Lazy<IPlugin, IPluginData>> plugins;

        private AggregateCatalog catalog;
        private DirectoryCatalog dirCatalog;
        private CompositionContainer _container;
        private FileSystemWatcher watcher;
        private String path = Directory.GetCurrentDirectory() + "\\Extensions";

        public SearchArguments SearchArgs { get; set; }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public MainWindow()
        {
            InitializeComponent();

            catalog = new AggregateCatalog();
           

            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Filter = "*.dll";
            watcher.Changed += new FileSystemEventHandler(OnExtensionCatalogChanged);
            watcher.Created += new FileSystemEventHandler(OnExtensionCatalogChanged);
            watcher.Deleted += new FileSystemEventHandler(OnExtensionCatalogChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            watcher.EnableRaisingEvents = true;

            dirCatalog = new DirectoryCatalog(path);

            ComposeExtensions();

                      
        }

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }

        private void ComposeExtensions ()
        {
            catalog = new AggregateCatalog();

            catalog.Catalogs.Add(new AssemblyCatalog(typeof(SearchArguments).Assembly));
            
            catalog.Catalogs.Add(dirCatalog);

            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }


            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                pluginNamesListBox.Items.Clear();

                foreach (Lazy<IPlugin, IPluginData> i in plugins)
                {
                    AddPluginCheckBox(i.Metadata.Extension);

                    //if (i.Metadata.Extension.Equals("txt"))
                    //{
                    //    i.Value.FindFilesByParams(new SearchArguments("C:\\Users\\user\\Desktop\\txt_folder\\papka", false, FileAttributes.Archive));
                    //    FilesList.ItemsSource = i.Value.searchResult;
                    //}
                }
            }));
        }

        private void AddPluginCheckBox(string extension)
        {
            pluginNamesListBox.Items.Add(extension);
        }

        private void FillPluginNamesStackPanel ()
        {
            
        }

        private void OnExtensionCatalogChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("Directory modified");
            dirCatalog.Refresh();

            ComposeExtensions();


        }

    }
}
