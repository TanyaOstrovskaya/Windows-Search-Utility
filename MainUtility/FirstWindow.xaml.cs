using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MainUtility
{
    public partial class FirstWindow : Window
    {
        private string CurrentDir { get; set; }
        private int MaxFileSize { get; set; }
        public string Size { get; set; }


        public FirstWindow()
        {
            InitializeComponent();

        }

        private void DialogButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.CurrentDir = dialog.SelectedPath;
                this.InputDirBlock.Text = dialog.SelectedPath;
            }

        }

        private void InputDirBlock_DataContextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.CurrentDir = InputDirBlock.Text;
        }

        private void fileSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Double> e)
        {
            this.MaxFileSize = (int)fileSizeSlider.Value;
        }

    }
}
