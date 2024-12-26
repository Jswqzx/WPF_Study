using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace WPF_Study
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            OperationType_File.IsChecked = true;
        }
        private void Select_Path_Click(object sender, RoutedEventArgs e)
        {
            // Configure open folder dialog box
            OpenFolderDialog dialog = new();

            dialog.Multiselect = false;
            dialog.Title = "Select a folder";

            // Show open folder dialog box
            bool? result = dialog.ShowDialog();

            // Process open folder dialog box results
            if (result == true)
            {
                // Get the selected folder
                string fullPathToFolder = dialog.FolderName;
                string folderNameOnly = dialog.SafeFolderName;

                MessageBoxResult windowResult = MessageBox.Show(fullPathToFolder,"FileName",MessageBoxButton.YesNo);

                // 确定选择目录
                if (windowResult == MessageBoxResult.Yes)
                {
                    // 回显到目录输入框中
                    SelectedPath.Text = fullPathToFolder;
                    // 读取目录下的所有文件
                    DirectoryInfo dirInfo = new DirectoryInfo(fullPathToFolder);
                    FileSystemInfo[] fileList;
                    if(OperationType_Dir.IsChecked == true)
                    {
                        fileList = dirInfo.GetDirectories("*", SearchOption.AllDirectories);
                    }
                    else
                    {
                        fileList = dirInfo.GetFiles("*", SearchOption.AllDirectories);
                    }

                    // 显示到结果窗口上
                    ShowResultWindow srw = new ShowResultWindow();
                    srw.addItemToUnSelectList(fileList);
                    srw.Show();
                }
            }
        }

        private void Clear_Path_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}