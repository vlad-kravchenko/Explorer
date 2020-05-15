using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Explorer
{
    public partial class MainWindow : Window
    {
        enum WorkMode { NONE, COPY, MOVE }

        string sourcePath = string.Empty;
        WorkMode mode = WorkMode.NONE;

        public MainWindow()
        {
            InitializeComponent();

            foreach(var drive in DriveInfo.GetDrives())
            {
                AddTreeViewItem(null, drive.Name, new FolderItem(null, drive.Name));
            }
        }

        private void AddTreeViewItem(TreeViewItem parent, string name, TreeItem tag)
        {
            var item = new TreeViewItem
            {
                Header = name,
                Tag = tag
            };
            if (tag is FolderItem)
            {
                item.Expanded += TreeViewItem_Expanded;
                item.Items.Add(new TreeViewItem());
            }
            item.Selected += TreeViewItem_Selected;

            if (parent != null)
                parent.Items.Add(item);
            else
                MainTree.Items.Add(item);
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            var item = e.Source as TreeViewItem;
            if (item.Tag == null) return;
            if (item.Tag is FileItem)
            {
                string fileName = (item.Tag as FileItem).GetFullPath();
                FileInfo fileInfo = new FileInfo(fileName);
                Info.Text = "";
                Info.Text += fileName;
                Info.Text += Environment.NewLine;
                Info.Text += GetFileSize(fileInfo.Length);
            }
            else
            {
                string dirPath = (item.Tag as FolderItem).GetFullPath();
                Info.Text = "";
                Info.Text += dirPath;
            }
        }

        private string GetFileSize(long length)
        {
            if (length < 1024)
            {
                return length + " b";
            }
            else if (length > 1024 * 1024 * 1024)
            {
                return (length / (1024 * 1024 * 1024)).ToString() + " Gb";
            }
            else if (length > 1024 * 1024)
            {
                return (length / (1024 * 1024)).ToString() + " Mb";
            }
            else
            {
                return (length / (1024)).ToString() + " kb";
            }
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            var item = e.Source as TreeViewItem;
            if (item.Tag == null || item.Tag is FileItem) return;

            var tag = item.Tag as FolderItem;
            item.Items.Clear();
            string path = tag.GetFullPath();
            try
            {
                foreach (var folder in Directory.GetDirectories(path))
                {
                    string dirName = Path.GetFileName(folder);
                    var subTag = new FolderItem(tag, dirName);
                    tag.Folders.Add(subTag);
                    AddTreeViewItem(item, dirName, subTag);
                }
                foreach (var file in Directory.GetFiles(path))
                {
                    string fileName = Path.GetFileName(file);
                    var subTag = new FileItem(tag, fileName);
                    tag.Files.Add(fileName);
                    AddTreeViewItem(item, fileName, subTag);
                }
            }
            catch { }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0)
            {
                var item = e.Source as TreeViewItem;
                if (item.Tag == null) return;

                sourcePath = (item.Tag as TreeItem).GetFullPath();
                mode = WorkMode.COPY;
            }
            else if (e.Key == Key.X && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0)
            {
                var item = e.Source as TreeViewItem;
                if (item.Tag == null) return;

                sourcePath = (item.Tag as TreeItem).GetFullPath();
                mode = WorkMode.MOVE;
            }
            else if (e.Key == Key.V && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0)
            {
                var item = e.Source as TreeViewItem;
                if (item.Tag == null) return;

                string destPath = (item.Tag as TreeItem).GetFullPath();
                if (mode == WorkMode.COPY)
                {
                    if (MessageBox.Show($"You sure you want to copy file '{sourcePath}' to folder '{destPath}'?", "Copy file", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        File.Copy(sourcePath, destPath + "\\" + Path.GetFileName(sourcePath), true);
                        UpdateTree(item);
                    }
                }
                else if (mode == WorkMode.MOVE)
                {
                    if (MessageBox.Show($"You sure you want to move file '{sourcePath}' to folder '{destPath}'?", "Move file", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        File.Move(sourcePath, destPath + "\\" + Path.GetFileName(sourcePath));
                        UpdateTree(item);
                    }
                }
                mode = WorkMode.NONE;
            }
            else if (e.Key == Key.Delete)
            {
                var item = e.Source as TreeViewItem;
                if (item.Tag == null) return;
                string path = (item.Tag as TreeItem).GetFullPath();
                if (MessageBox.Show($"You sure you want to delete file '{path}", "Delete file", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.Delete(path);
                    UpdateTree(item);
                }
            }
        }

        private void UpdateTree(TreeViewItem item)
        {
            item.IsExpanded = false;
            item.IsExpanded = true;
        }
    }
}