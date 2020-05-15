namespace Explorer
{
    public class TreeItem
    {
        public string Name { get; set; }
        public TreeItem Parent { get; set; }

        public TreeItem(TreeItem parent, string name)
        {
            Parent = parent;
            Name = name;
        }

        public string GetFullPath()
        {
            if (Parent == null) return Name;
            else return Parent.GetFullPath() + "\\" + Name;
        }
    }
}