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
        }

        private void Switch_To_File_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("切换到文件工作页面","弹窗标题",MessageBoxButton.OK,MessageBoxImage.Information);
        }

        private void Switch_to_Dir_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("切换到目录工作页面", "弹窗标题", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    FileInfo[] fileList = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
                    // 显示到结果窗口上
                    ShowResultWindow srw = new ShowResultWindow();
                    srw.addItemToUnSelectList(fileList);
                    srw.Show();
                    //var fileQuery = from file in fileList
                    //                where file.Extension == ".txt"
                    //                orderby file.CreationTime
                    //                select file;

                    //foreach(FileInfo fi in fileQuery)
                    //{
                    //    MessageBox.Show(fi.Extension,"File Result Show Window");
                    //}
                }
            }
        }

        private void Clear_Path_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}