#nullable enable
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class PageNode
{
    public string Name;
    public VisualElement? VisualElement;
    public PageNode? Parent;
    public List<PageNode> Children = new List<PageNode>();

    public IEnumerable<PageNode> Siblings =>
        Parent == null ? Enumerable.Empty<PageNode>() : Parent.Children.Where(c => c != this);

    public PageNode(string name, VisualElement? ve)
    {
        Name = name;
        VisualElement = ve;
    }

    public void AddChild(PageNode child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    public static Dictionary<string, PageNode> GeneratePageLookup(
        UIDocument document,
        PageTree root
    )
    {
        var lookup = new Dictionary<string, PageNode>();

        void Build(PageTree tree, PageNode? parent)
        {
            var element = document.rootVisualElement.Q(tree.Name);
            var node = new PageNode(tree.Name, element);
            lookup[tree.Name] = node;

            if (parent != null)
                parent.AddChild(node);

            foreach (var child in tree.Children)
                Build(child, node);
        }

        Build(root, null);
        return lookup;
    }
}


[System.Serializable]
public class PageTree
{
    public string Name;
    public List<PageTree> Children = new List<PageTree>();
}
