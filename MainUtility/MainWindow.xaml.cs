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
using System.ComponentModel;

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
        private String path = AppDomain.CurrentDomain.BaseDirectory;
        private UserControl currControl;

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
                    pluginNamesListBox.Items.Add( i.Metadata.Extension);
                }
            }));
        }

        public void ShowDir()
        {
            this.currentDirName.Content = SearchArgs.DirPath;
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
            foreach (Lazy<IPlugin, IPluginData> i in plugins)
            {
                if (i.Metadata.Extension.Equals(pliginName))
                {
                    i.Value.InitPlugin(this, SearchArgs);

                    PluginWindow pluginWindow = new PluginWindow();
                    Panel pluginPanel = new StackPanel();
                    pluginPanel.Children.Add(i.Value.userControl);                    
                    pluginWindow.Content = pluginPanel;

                    i.Value.SearchEnd += new EventHandler(HandleSearchEnd);

                    pluginWindow.Show();
                }
            }

        }

        private void HandleSearchEnd(object sender, EventArgs e)
        {
            FilesList.Items.Clear();
            FilesList.ItemsSource = (sender as MainUtility.IPlugin).searchResult;
            FilesList.UpdateLayout();
        }      


    }
}
