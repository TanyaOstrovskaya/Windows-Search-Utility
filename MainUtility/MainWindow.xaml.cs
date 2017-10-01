using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MainUtility
{
    public partial class MainWindow : Window
    {
        [ImportMany]
        IEnumerable<Lazy<IPlugin, IPluginData>> plugins;


        private CompositionContainer _container;

        public MainWindow()
        {
            InitializeComponent();

            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(SearchArguments).Assembly));
            String path = Directory.GetCurrentDirectory() + "\\Extensions";
            catalog.Catalogs.Add(new DirectoryCatalog(path));


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

            foreach (Lazy<IPlugin, IPluginData> i in plugins)
            {
                if (i.Metadata.Extension.Equals("txt"))
                {
                    i.Value.FindFilesByParams(new SearchArguments("C:\\Users\\user\\Desktop\\txt_folder\\papka", false, FileAttributes.Archive));
                    FilesList.ItemsSource = i.Value.searchResult;
                }
            }
            
        }
       

    }
}
