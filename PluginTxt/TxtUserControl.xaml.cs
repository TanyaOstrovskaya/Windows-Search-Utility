using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PluginTxt
{
    public partial class TxtUserControl : UserControl
    {
        private SearcherTxt _searcher;
        private BackgroundWorker backgrWorker;
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

        public TxtUserControl()
        {
            InitializeComponent();
        }
        public TxtUserControl(SearcherTxt searcher)
        {
            InitializeComponent();
            InitBackgroundWorker();
            this._searcher = searcher;          
        }

        private void InitBackgroundWorker()
        {
            backgrWorker = new BackgroundWorker();
            this.backgrWorker.WorkerSupportsCancellation = true;
            backgrWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgrWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);           
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("Поиск остановлен");
            }
            else
            {
                MessageBox.Show("Поиск завершен");
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            StopButton.Dispatcher.Invoke( () => StopButton.IsEnabled = true );
            this._searcher.FindFilesAsync (worker, e);
        }      

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {           
            backgrWorker.RunWorkerAsync();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            this.backgrWorker.CancelAsync();
        }
    }
}
