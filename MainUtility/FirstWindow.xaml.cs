using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MainUtility
{
    public partial class FirstWindow : Window
    {
        private SearchArguments selectedArgs { get; set; }

        public FirstWindow()
        {
            InitializeComponent();
        }

        private void DialogButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.selectedArgs.DirPath = dialog.SelectedPath;
                this.InputDirBlock.Text = dialog.SelectedPath;
            }
        }

        private void InputDirBlock_DataContextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.selectedArgs.DirPath = InputDirBlock.Text;
        }

        private void fileSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Double> e)
        {
            this.selectedArgs.Length = (int)fileSizeSlider.Value * 1024;
        }

        private bool GetCheckedRecursiveSearch ()
        {
            return recursiveSearchContainer.Children.OfType<RadioButton>().FirstOrDefault(r => (bool)r.IsChecked).Content.Equals("да");
        }
        
        private FileAttributes GetSelectedFileAttributes()
        {
            FileAttributes result = new FileAttributes();
            foreach (CheckBox chBox in attribitesStackPanel.Children)
            {
                if ((bool)chBox.IsChecked)                
                    result |= FileAttributes.Archive;              
            }

            return result;
        }

        private FileAttributes GetFileAttributeNumberByName(string attr)
        {
            switch (attr)
            {
                case "Архивный":
                    return FileAttributes.Archive;                 
                case "Скрытый":
                    return FileAttributes.Hidden;                   
                case "Системный":
                    return FileAttributes.System; 
                case "Только чтение":
                    return FileAttributes.ReadOnly;
                default:
                    return 0;
            }       
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateSelectedArgs())
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.SearchArgs = selectedArgs;
                mainWindow.Show();
                this.Close();
            }
        }

        private bool ValidateSelectedArgs()
        {
            return true;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectedArgs.LastDate = (DateTime)this.datePicker.SelectedDate;
        }
    }
}
