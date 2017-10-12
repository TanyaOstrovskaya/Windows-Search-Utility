using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PluginTxt
{
    public partial class TxtUserControl : UserControl
    {
        public TxtUserControl()
        {
            InitializeComponent();
        }

        public event EventHandler SearchStart;
         
        public string searchTxtSubstring { get; set; }

        protected virtual void OnSearchStart()
        {
            if (SearchStart != null) SearchStart(this, EventArgs.Empty);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            searchTxtSubstring = new TextRange(SearchSubstringText.Document.ContentStart, SearchSubstringText.Document.ContentEnd).Text;
            OnSearchStart();
        }
    }
}
