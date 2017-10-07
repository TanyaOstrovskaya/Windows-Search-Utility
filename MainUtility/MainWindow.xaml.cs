using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MainUtility
{
    public partial class MainWindow : Window
    {
        [ImportMany(AllowRecomposition = true)]
        IEnumerable<Lazy<IPlugin, IPluginData>> plugins;

        private AggregateCatalog catalog;
        private DirectoryCatalog dirCatalog;
        private CompositionContainer _container;

        public SearchArguments SearchArgs { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            //An aggregate catalog that combines multiple catalogs
            catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(SearchArguments).Assembly));
            String path = Directory.GetCurrentDirectory() + "\\Extensions";
            dirCatalog = new DirectoryCatalog(path);
            catalog.Catalogs.Add(dirCatalog);

            catalog.Changed += OnExtensionCatalogChanged;

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

            FillPluginNamesStackPanel();

                      
        }

        private void AddPluginCheckBox(string extension)
        {
            pluginNamesListBox.Items.Add(extension);
        }

        private void FillPluginNamesStackPanel ()
        {
            foreach (Lazy<IPlugin, IPluginData> i in plugins)
            {
                AddPluginCheckBox(i.Metadata.Extension);

                //if (i.Metadata.Extension.Equals("txt"))
                //{
                //    i.Value.FindFilesByParams(new SearchArguments("C:\\Users\\user\\Desktop\\txt_folder\\papka", false, FileAttributes.Archive));
                //    FilesList.ItemsSource = i.Value.searchResult;
                //}
            }
        }

        void OnExtensionCatalogChanged(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            Console.WriteLine("Directory modified");
            dirCatalog.Refresh();            
        }

    }
}
