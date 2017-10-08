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

        private AggregateCatalog _catalog;
        private DirectoryCatalog _dirCatalog;
        private CompositionContainer _container;
        private FileSystemWatcher _watcher;
        private String _dirCatalogPath; 

        public SearchArguments SearchArgs { get; set; }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public MainWindow()
        {
            InitializeComponent();          

            _dirCatalogPath = Directory.GetCurrentDirectory() + "\\Extensions";
            _dirCatalog = new DirectoryCatalog(_dirCatalogPath);

            InitDirWatcher();
            ComposeExtensions();                      
        }

        private void InitDirWatcher ()
        {
            _watcher = new FileSystemWatcher();
            _watcher.Path = _dirCatalogPath;
            _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            _watcher.Filter = "*.dll";
            _watcher.Changed += new FileSystemEventHandler(OnExtensionCatalogChanged);
            _watcher.Created += new FileSystemEventHandler(OnExtensionCatalogChanged);
            _watcher.Deleted += new FileSystemEventHandler(OnExtensionCatalogChanged);
            _watcher.Renamed += new RenamedEventHandler(OnRenamed); 
            _watcher.EnableRaisingEvents = true;
        }
    
        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }

        private void ComposeExtensions ()
        {
            _catalog = new AggregateCatalog();
            _catalog.Catalogs.Add(new AssemblyCatalog(typeof(SearchArguments).Assembly));            
            _catalog.Catalogs.Add(_dirCatalog);
            _container = new CompositionContainer(_catalog);

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
                }

            }));
        }

        private void AddPluginCheckBox(string extension)
        {
            pluginNamesListBox.Items.Add(extension);
        }

        private void OnExtensionCatalogChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("Directory modified");
            _dirCatalog.Refresh();
            ComposeExtensions();
        }

    }
}
