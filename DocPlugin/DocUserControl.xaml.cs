using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;


namespace DocPlugin
{
    public partial class DocUserControl : UserControl
    {
        private DocSearcher _searcher;
        private BackgroundWorker backgrWorker;

        public DocUserControl()
        {
            InitializeComponent();
        }
        public DocUserControl(DocSearcher searcher)
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
            StopButton.Dispatcher.Invoke(() => StopButton.IsEnabled = true);
            this._searcher.FindFilesAsync(worker, e);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _searcher.userTitle = docTitle.Text;
            _searcher.userSubject = docSubject.Text;
            _searcher.userAuthor = docAuthor.Text;
            _searcher.userManager = docManager.Text;
            _searcher.userCompany = docOrganization.Text;

            backgrWorker.RunWorkerAsync();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            this.backgrWorker.CancelAsync();
        }
    }
}

