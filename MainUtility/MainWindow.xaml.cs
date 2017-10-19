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
using System.Collections.ObjectModel;
using System.Windows.Threading;

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
        private SearchArguments searchArgs;
        private ObservableCollection<string> resList = new ObservableCollection<string>();
        Window pluginWindow;

        private IPlugin currPlugin;

        public SearchArguments SearchArgs {
            get { return searchArgs; }
            set
            {
                this.searchArgs = value;
                this.currentDirName.Content = SearchArgs.DirPath;
            }
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public MainWindow()
        {
            InitializeComponent();
            string appPath = AppDomain.CurrentDomain.BaseDirectory;

            _catalog = new AggregateCatalog();     

            _watcher = new FileSystemWatcher();
            _watcher.Path = appPath;
            _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite  | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            _watcher.Filter = "*.dll";
            _watcher.Changed += new FileSystemEventHandler(OnExtensionCatalogChanged);
            _watcher.Created += new FileSystemEventHandler(OnExtensionCatalogChanged);
            _watcher.Deleted += new FileSystemEventHandler(OnExtensionCatalogChanged);
            _watcher.Renamed += new RenamedEventHandler(OnRenamed);
            _watcher.EnableRaisingEvents = true;

            _dirCatalog = new DirectoryCatalog(appPath);

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

        private void OnExtensionCatalogChanged(object source, FileSystemEventArgs e)
        {
            _dirCatalog.Refresh();
            ComposeExtensions();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (pluginNamesListBox.SelectedIndex == -1)
                return;

            ChangeSearchArgsButton.IsEnabled = false;
            FilesList.Text = String.Empty;
            String pluginName = pluginNamesListBox.SelectedItem.ToString();

            foreach (Lazy<IPlugin, IPluginData> i in plugins)
            {
                if (i.Metadata.Extension.Equals(pluginName))
                {
                    i.Value.InitPlugin(SearchArgs);
                    currPlugin = i.Value;
                    pluginWindow = new Window();
                    Panel pluginPanel = new StackPanel();
                    pluginPanel.Children.Add(i.Value.pluginUserControl);                    
                    pluginWindow.Content = pluginPanel;
                    pluginWindow.Width = i.Value.pluginUserControl.Width + 20;
                    pluginWindow.Height = i.Value.pluginUserControl.Height + 40;
                    i.Value.NewItemFound += new EventHandler(HandleNewItemFound);                   

                    pluginWindow.Show();
                    pluginWindow.Closed += new EventHandler(HandlePluginWindowClosed);
                }
            }
        }

        private void HandlePluginWindowClosed(object sender, EventArgs e)
        {
            ChangeSearchArgsButton.IsEnabled = true;
        }

        private void HandleNewItemFound(object sender, EventArgs e)
        {
            FilesList.Dispatcher.Invoke(DispatcherPriority.Render, (Action)delegate () 
            {
                int index = (sender as IPlugin).pluginSearchResultList.Count - 1;
                FilesList.Text += (sender as MainUtility.IPlugin).pluginSearchResultList[index] + "\n";
            });
        }

        private void ChangeSearchArgs_Click(object sender, RoutedEventArgs e)
        {
            FirstWindow window = new FirstWindow();
            window.Show();
            this.Close();
        }
    }
}
