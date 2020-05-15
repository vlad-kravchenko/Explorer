using System.Collections.Generic;

namespace Explorer
{
    public class FolderItem : TreeItem
    {
        public FolderItem(TreeItem parent, string name) : base(parent, name)
        {
            Folders = new List<TreeItem>();
            Files = new List<string>();
        }

        public List<TreeItem> Folders { get; set; }
        public List<string> Files { get; set; }
    }
}