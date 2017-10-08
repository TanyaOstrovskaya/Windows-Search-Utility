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
        private String path = Directory.GetCurrentDirectory() + "\\Extensions";

        public SearchArguments SearchArgs { get; set; }


        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public MainWindow()
        {
            InitializeComponent();

            _catalog = new AggregateCatalog();
           

            _watcher = new FileSystemWatcher();
            _watcher.Path = path;
            _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            _watcher.Filter = "*.dll";
            _watcher.Changed += new FileSystemEventHandler(OnExtensionCatalogChanged);
            _watcher.Created += new FileSystemEventHandler(OnExtensionCatalogChanged);
            _watcher.Deleted += new FileSystemEventHandler(OnExtensionCatalogChanged);
            _watcher.Renamed += new RenamedEventHandler(OnRenamed);
            _watcher.EnableRaisingEvents = true;

            _dirCatalog = new DirectoryCatalog(path);

            ComposeExtensions();
            SearchArgs = new SearchArguments();

        }

       
        private static void OnRenamed(object source, RenamedEventArgs e)
        {            
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

                    if (i.Metadata.Extension.Equals("doc"))
                    {
                        i.Value.FindFilesByParams(SearchArgs);
                        FilesList.ItemsSource = i.Value.searchResult;
                    }
                }
            }));
        }

        public void ShowDir()
        {
            this.currentDirName.Content = SearchArgs.DirPath;
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            String pliginName = pluginNamesListBox.SelectedItem.ToString();
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {                
                foreach (Lazy<IPlugin, IPluginData> i in plugins)
                {
                    if (i.Metadata.Extension.Equals(pliginName))
                    {
                        Window window = new Window();
                        StackPanel panel = new StackPanel();                     
                        UserControl control = i.Value.FindFilesByParams(SearchArgs);
                        panel.Children.Add(control);
                        window.Content = panel;
                        window.Show();
                    }
                }
            }));

        }
    }
}
