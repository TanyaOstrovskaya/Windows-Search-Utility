using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MainUtility
{
    public partial class FirstWindow : Window
    {
        private string CurrentDir { get; set; } = null;
        private long MaxFileSize { get; set; }
        private DateTime selectedDate { get; set; } 

        public FirstWindow()
        {
            InitializeComponent();
            this.fileSizeSlider.Value = 0;
            this.MaxFileSize = 0;
        }

        private void DialogButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
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
            this.MaxFileSize = (long)fileSizeSlider.Value*1024;
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
                    result |= GetFileAttributeNumberByName(chBox.Content.ToString());
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

        private void StartSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateUserInput())
            {
                MainWindow mainWindow = new MainWindow();
                SearchArguments args = new SearchArguments(CurrentDir, GetCheckedRecursiveSearch(), GetSelectedFileAttributes(), MaxFileSize, selectedDate);
                mainWindow.SearchArgs = args;
                mainWindow.Show();
                this.Close();
            }
        }

        private bool ValidateUserInput()
        {
            if (!(System.IO.Directory.Exists(CurrentDir)))
            {
                MessageBox.Show ("Неверно введен путь к директории");
                return false;
            }           
            if (recursiveSearchContainer.Children.OfType<RadioButton>().FirstOrDefault(r => (bool)r.IsChecked) == null)
            {
                MessageBox.Show("Не указано, производить ли поиск в каталогах");
                return false;
            }
            if (!((bool)(archiveChBox.IsChecked) || (bool)(hiddenChBox.IsChecked) || (bool)(systemChBox.IsChecked) || (bool)(readOnlyChBox.IsChecked)))
            {
                MessageBox.Show("Не указаны атрибуты файла");
                return false;
            }
            if (MaxFileSize == 0)
            {
                MessageBox.Show("Не указан максимальный размер файла");
                return false;
            }
            if (selectedDate == DateTime.MinValue)
            {
                MessageBox.Show("Дата не указана");
                return false;
            }

            return true;
        }

        private void userDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectedDate = (DateTime)userDate.SelectedDate;
        }
    }
}
