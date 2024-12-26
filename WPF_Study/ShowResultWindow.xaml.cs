using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.VisualBasic.FileIO;

namespace WPF_Study
{
    /// <summary>
    /// ShowResultWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShowResultWindow : Window
    {

        fileVM vm;

        public int fileCount { get; set; } = 100;

        private List<FileItem> storeList = new List<FileItem>();

        public ShowResultWindow()
        {
            InitializeComponent();

            vm = new fileVM();

            this.DataContext = vm;

            nameSelectBox.SelectedIndex = 0;
        }

        // 添加项目列表
        public void addItemToUnSelectList(FileSystemInfo[] fileList)
        {
            foreach (var file in fileList)
            {
                FileItem fileItem = new FileItem();
                fileItem.Name = file.Name;
                fileItem.FullName = file.FullName;
                vm.unSelectFileNames.Add(fileItem);
                storeList.Add(fileItem);
            }
            vm.unSelectFileCount = vm.unSelectFileNames.Count;
        }

        // 全选
        private void checkUnSelectAll(object sender, RoutedEventArgs e)
        {
            unSelectedList.SelectAll();
        }

        // 取消全选
        private void unCheckUnSelectAll(object sender, RoutedEventArgs e)
        {
            unSelectedList.UnselectAll();
        }

        // 选择文件
        private void UnSelecteToSelected(object sender, RoutedEventArgs e)
        {
            updateFileList(unSelectedList, vm.selectedFileNames, vm.unSelectFileNames);
            if(unSelectAll.IsChecked == true)
            {
                unSelectAll.IsChecked = false;
            }
        }

        // 全选选择文件
        private void checkSelectedAll(object sender, RoutedEventArgs e)
        {
            selectedList.SelectAll();
        }

        // 取消全选选择文件
        private void unCheckSelectedAll(object sender, RoutedEventArgs e)
        {
            selectedList.UnselectAll();
        }

        // 取消文件选择
        private void SelectedToUnSelect(object sender, RoutedEventArgs e)
        {
            updateFileList(selectedList, vm.unSelectFileNames, vm.selectedFileNames);
            if (selectedAll.IsChecked == true)
            {
                selectedAll.IsChecked = false;
            }
        }

        // 更新文件列表
        private void updateFileList(ListBox listBox, ObservableCollection<FileItem> sou, ObservableCollection<FileItem> des)
        {
            var selectedItems = listBox.SelectedItems;

            List<FileItem> tempArr = new List<FileItem>();
            foreach (FileItem item in selectedItems)
            {
                tempArr.Add(item);
            }

            foreach (FileItem item in tempArr)
            {
                sou.Add(item);
                des.Remove(item);
            }

            vm.updateCount(vm);
        }

        // 匹配字符串
        private bool MatchString(string filterWord,int filterStartIndex,string desStr,int desStrStartIndex)
        {
            while (filterStartIndex < filterWord.Length && desStrStartIndex < desStr.Length)
            {
                if (filterWord[filterStartIndex] == desStr[desStrStartIndex]) // 字符相等
                {
                    filterStartIndex++;
                    desStrStartIndex++;
                }
                else if ('*'.Equals(filterWord[filterStartIndex])) // 匹配到通配符
                {
                    // 判断通配符是否在过滤词的最后一个
                    if (filterStartIndex == filterWord.Length - 1)
                    {
                        return true;
                    }
                    else // 通配符后还有字符
                    {
                        // 获取通配符下一个字符
                        char matchStr = filterWord[filterStartIndex + 1];
                        // 从当前下标开始匹配
                        desStrStartIndex = desStr.IndexOf(matchStr, desStrStartIndex);
                        // 没匹配到
                        if (desStrStartIndex == -1)
                        {
                            return false;
                        }
                        filterStartIndex++;
                    }
                }
                else // 没匹配到，并且也不是通配符
                {
                    return false;
                }
            }

            // 走出循环，判断是否匹配完毕
            if (filterStartIndex == filterWord.Length && desStrStartIndex == desStr.Length)
            {
                return true;
            }

            return false;
        }

        // 判断文件名是否符合过滤词
        private bool IsMatches(string filterWord, FileItem item)
        {
            // 判断是否含有通配符*
            if (filterWord.Contains("*"))
            {
                // 获取通配符位置，从而进入不同的逻辑
                int wildcardIndex = 0, startMatchIndex = 0;
                // 通配符在头，用通配符后面的字符去进行匹配
                if ( '*'.Equals(filterWord[0]) )
                {
                    // 获取通配符后的第一个非通配符字符
                    char matchStr = ' ';
                    for(int i = 1; i < filterWord.Length; i++)
                    {
                        if (!"*".Equals(filterWord[i]))
                        {
                            matchStr = filterWord[i];
                            break;
                        }
                    }
                    // 判断目标字符串中是否有该字符
                    startMatchIndex = item.Name.IndexOf(matchStr);
                    if (startMatchIndex == -1)
                        return false;
                    else
                        wildcardIndex = 1;
                }
                // 循环匹配，遇到通配符就跳过
                if (MatchString(filterWord, wildcardIndex, item.Name, startMatchIndex))
                    return true;

                return false;
            }
            else if(filterWord.Equals(item.Name))// 没有通配符，进行全字符串比较
            {
                return true;
            }

            return false;
        }

        // 获取过滤过滤词过滤后的列表
        private ObservableCollection<FileItem> getFilterList(string filterWord, ObservableCollection<FileItem> filterList)
        {
            ObservableCollection<FileItem> tempList = new ObservableCollection<FileItem>();
            // 用过滤词遍历过滤列表
            foreach (FileItem item in filterList)
            {
                // 判断该文件名是否符合过滤词
                if(IsMatches(filterWord,item))
                    tempList.Add(item);
            }

            return tempList;
        }

        // 未选择文件过滤点击事件
        private void FilterUnSelectList(object sender, RoutedEventArgs e)
        {
            vm.unSelectFileNames = getListAfterFilter(UnSelectFilterWord.Text, vm.unSelectFileNames);

        }

        // 重置
        private void resetAll(object sender, RoutedEventArgs e)
        {
            // 重置数组与计数
            vm.unSelectFileNames.Clear();
            vm.selectedFileNames.Clear();

            foreach(var item in storeList)
            {
                vm.unSelectFileNames.Add(item);
            }
            vm.selectedFileCount = 0;
            vm.unSelectFileCount = vm.unSelectFileCount;

            // 重置全选框
            unSelectAll.IsChecked = false;
            selectedAll.IsChecked = false;

            // 重置过滤词
            UnSelectFilterWord.Text = string.Empty;
        }

        // 筛选已选文件列表
        private void FilterSelectedList(object sender, RoutedEventArgs e)
        {
            // 获取过滤词
            vm.selectedFileNames = getListAfterFilter(SelectedFilterWord.Text, vm.selectedFileNames);
        }

        // 根据过滤词过滤数组，然后返回
        private ObservableCollection<FileItem> getListAfterFilter(string filterWord, ObservableCollection<FileItem> desList)
        {
            ObservableCollection<FileItem> resList = new ObservableCollection<FileItem>();
            if (!String.IsNullOrEmpty(filterWord))
            {
                // 复制数组
                foreach (FileItem item in desList)
                {
                    resList.Add(item);
                }

                var WordList = filterWord.Split('/');

                foreach (string word in WordList)
                {
                    // 对列表进行过滤
                    resList = getFilterList(word, resList);
                }
            }
            return resList;
        }

        // 执行文件名称操作
        private void nameOperationConfirm(object sender, RoutedEventArgs e)
        {
            int selectionIndex = nameSelectBox.SelectedIndex;
            // 下标为0表示去除名称中的指定字符串
            if (selectionIndex == 0)
            {
                var opStrList = nameOperationValue.Text.Split("/");
                foreach (FileItem fileItem in vm.selectedFileNames)
                {
                    foreach (string opStr in opStrList)
                    {
                        fileItem.Name.Replace(opStr, "");
                    }
                }
            }
            else // 下标为1表示进行替换操作
            {
                string replaceSource = nameOperationValue.Text;
                string replaceTarget = replaceValue.Text;

                if (!"".Equals(replaceSource))
                {
                    var fileList =  vm.selectedFileNames;
                    foreach (FileItem fileItem in fileList)
                    {
                        fileItem.Name = fileItem.Name.Replace(replaceSource,replaceTarget);
                    }
                }
            }
            // 更新名称
            //foreach (FileItem fileItem in vm.selectedFileNames)
            //{
            //    if()
            //    {

            //    }
            //    else
            //    {

            //    }
            //}

        }

        // 
        private void modChange(object sender, SelectionChangedEventArgs e)
        {
            int value = nameSelectBox.SelectedIndex;
            if(value == 1)
            {
                rePlaceBlock.Visibility = Visibility.Visible;
            }
            else
            {
                rePlaceBlock.Visibility = Visibility.Collapsed;
            }
            replaceValue.Text = string.Empty;
        }

        // 提取文件
        private void extractFiles(object sender, RoutedEventArgs e)
        {
            // 1.获取目录列表
            var originalDirList = vm.selectedFileNames;
            // 2. 遍历列表获取目录对象
            int step = 0;
            if(originalDirList.Count != 0)
            {
                step = 100 / originalDirList.Count;
            }
            foreach (FileItem fileItem in originalDirList)
            {
                // 3.判断该路径是否为目录
                string path = fileItem.FullName;
                extractDir(path,path);

                fileProcessBar.Value += step;
            }
            MessageBox.Show("提取完成","结果提示",MessageBoxButton.OK);
        }

        private void extractDir(string sourcePath,string targetPath)
        {
            if (Directory.Exists(sourcePath) && Directory.Exists(targetPath))
            {
                // 4. 判断该目录下是否存在子目录
                string[] subDirPaths = Directory.GetDirectories(sourcePath);
                // 5. 遍历子目录，进行递归
                foreach (string subPath in subDirPaths)
                {
                    // 6. 判断该路径下是否还有目录，有就进行递归
                    if (Directory.Exists(subPath))
                    {
                        extractDir(subPath, sourcePath);
                    }
                }
                // 7. 获取指定目录下的文件
                if(!sourcePath.Equals(targetPath))
                {
                    string[] sourceFiles = Directory.GetFiles(sourcePath);
                    // 8. 遍历文件，移动到指定目录下
                    foreach (string sourceFile in sourceFiles)
                    {
                        // 文件名称
                        string[] splitList = sourceFile.Split("\\");
                        string fileName = splitList[splitList.Length - 2] + "_" + splitList[splitList.Length - 1];

                        // 移动文件
                        File.Copy(sourceFile, targetPath + "\\" + fileName, true);
                        File.Delete(sourceFile);
                    }

                    Directory.Delete(sourcePath);
                }
            }
        }
    }


    public class fileVM : INotifyPropertyChanged
    {
        private ObservableCollection<FileItem> _unSelectFileNames;

        private ObservableCollection<FileItem> _selectedFileNames;

        private int _unSelectFileCount;

        private int _selectedFileCount;

        public int unSelectFileCount
        {
            get 
            {
                if (_unSelectFileNames != null)
                {
                    return _unSelectFileNames.Count;
                }
                return 0;
            }
            set
            {
                _unSelectFileCount = value;
                OnPropertyChanged("unSelectFileCount");
            }
        }

        public int selectedFileCount
        {
            get
            {
                if (_selectedFileNames != null)
                {
                    return _selectedFileNames.Count;
                }
                return 0;
            }
            set
            {
                _selectedFileCount = value;
                OnPropertyChanged("selectedFileCount");
            }
        }

        public void updateCount(fileVM vm)
        {
            vm.unSelectFileCount = vm.unSelectFileNames.Count;
            vm.selectedFileCount = vm.selectedFileNames.Count;
        }

        public ObservableCollection<FileItem> unSelectFileNames
        {
            get { 
                if (_unSelectFileNames == null)
                {
                    _unSelectFileNames = new ObservableCollection<FileItem> { };
                }
                return _unSelectFileNames; 
            } 
            set 
            { 
                _unSelectFileNames = value;
                OnPropertyChanged("unSelectFileNames");
            }
        }

        public ObservableCollection<FileItem> selectedFileNames
        {
            get
            {
                if (_selectedFileNames == null)
                {
                    _selectedFileNames = new ObservableCollection<FileItem> { };
                }
                return _selectedFileNames;
            }
            set 
            { 
                _selectedFileNames = value;
                OnPropertyChanged("selectedFileNames");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FileItem : INotifyPropertyChanged
    {
        private string _Name;

        private string _FullName;

        public string FullName {
            get { return _FullName; }
            set 
            {
                _FullName = value;
                OnPropertyChanged("FullName");
            } 
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
