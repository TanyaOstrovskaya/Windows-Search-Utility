using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public TxtUserControl(SearcherTxt searcher)
        {
            InitializeComponent();
            this._searcher = searcher;


            backgroundWorker1 = new BackgroundWorker();
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork +=
                new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged +=
                new ProgressChangedEventHandler(
            backgroundWorker1_ProgressChanged);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("Canceled");
            }
            else
            {
                MessageBox.Show("Done");
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            StopButton.Dispatcher.Invoke(() => StopButton.IsEnabled = true);
            this._searcher.FindFilesByParams(_searcher._args, worker, e);
        }

        private SearcherTxt _searcher;
        private BackgroundWorker backgroundWorker1;
        private string searchSubstrText; 
        public string SearchSubstrText
        {
            get
            {
                searchSubstrText = new TextRange(SubstringTextBox.Document.ContentStart, SubstringTextBox.Document.ContentEnd).Text;
                return searchSubstrText;
            }
            set
            {
                searchSubstrText = value;
            }
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
           
            backgroundWorker1.RunWorkerAsync(searchSubstrText);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            this.backgroundWorker1.CancelAsync();
        }
    }
}
