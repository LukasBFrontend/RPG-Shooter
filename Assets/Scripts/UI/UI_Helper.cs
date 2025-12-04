#nullable enable
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UIElements;

public class PageNode
{
    public string Name;
    public VisualElement? VisualElement;
    public PageNode? Parent;
    public List<PageNode> Children = new();

    public IEnumerable<PageNode> Siblings =>
        Parent == null ? Enumerable.Empty<PageNode>() : Parent.Children.Where(c => c != this)
    ;

    public PageNode(string name, VisualElement? visualElement)
    {
        Name = name;
        VisualElement = visualElement;
    }

    public void AddChild(PageNode child)
    {
        child.Parent = this;
        Children.Add(child);
        if (child == this)
        {
            throw new Exception("A node cannot reference itself.");
        }
    }

    public static Dictionary<string, PageNode> GeneratePageLookup(
        UIDocument document,
        PageTree root
    )
    {
        Dictionary<string, PageNode> _lookup = new();

        void Build(PageTree tree, PageNode? parent)
        {
            var element = document.rootVisualElement.Q(tree.Name);
            var node = new PageNode(tree.Name, element);
            _lookup[tree.Name] = node;

            parent?.AddChild(node);

            foreach (var child in tree.Children)
            {
                Build(child, node);
            }
        }

        Build(root, null);
        return _lookup;
    }
}


[System.Serializable]
public class PageTree
{
    public string Name = "";
    public List<PageTree> Children = new();
}
