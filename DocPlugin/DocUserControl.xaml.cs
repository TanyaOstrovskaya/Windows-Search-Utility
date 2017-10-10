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
using System.ComponentModel.Composition;
using MainUtility;
using System.IO;

namespace DocPlugin
{
    public partial class DocUserControl : UserControl
    {
        public DocUserControl()
        {
            InitializeComponent();
        }

        public event EventHandler SearchStart;

        protected virtual void OnSearchStart()
        {
            if (SearchStart != null) SearchStart(this, EventArgs.Empty);
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            OnSearchStart();
        }


        
        
    }
}

