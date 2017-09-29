using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;

namespace MainUtility
{
    public partial class MainWindow : Window
    {
        private CompositionContainer _container;

        [Import(typeof(IPlugin))]
        public IPlugin txtPlugin;


        public MainWindow()
        {
            InitializeComponent();




            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(MainWindow).Assembly));
            String path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Extensions";
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





            this.FilesList.ItemsSource = txtPlugin.searchResult;
            txtPlugin.FindFilesByParams(new SearchArguments("C:\\Users\\user\\Desktop\\txt_folder", false, FileAttributes.ReadOnly));
        }
       

    }
}
