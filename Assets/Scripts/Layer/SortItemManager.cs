using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SortItemManager : Singleton<SortItemManager>
{
    List<SortItem> _sortQueA = new();
    List<SortItem> _sortQueB = new();
    List<SortItem> _sortQueC = new();
    Dictionary<string, List<SortItem>> _layerQueLookup = new();
    void Start()
    {
        _layerQueLookup.Add("Character A", _sortQueA);
        _layerQueLookup.Add("Character B", _sortQueB);
        _layerQueLookup.Add("Character C", _sortQueC);

        List<SortItem> _activeSortItems = FindObjectsByType<SortItem>(FindObjectsSortMode.None).ToList();

        SortVertical(_activeSortItems);
        foreach (SortItem item in _activeSortItems)
        {
            AddToSortOrder(item);
        }
    }

    public void RemoveFromSortOrder(SortItem item)
    {
        _sortQueA.Remove(item);
        _sortQueB.Remove(item);
        _sortQueC.Remove(item);
    }

    public void AddToSortOrder(SortItem item)
    {
        RemoveFromSortOrder(item);

        if (!_layerQueLookup.TryGetValue(item.SpriteRenderer.sortingLayerName, out var sortQue))
        {
            Debug.LogError($"Item {item.name} could was not sorted. Sorting layer ID does not correspond to character layer. ({item.SpriteRenderer.sortingLayerName})");
            return;
        }
        sortQue.Add(item);
    }

    public void MarkDirty(SortItem item)
    {
        List<SortItem> _sortQue = _layerQueLookup[item.SpriteRenderer.sortingLayerName];
        AddToSortOrder(item);

        if (_sortQue == null)
        {
            return;
        }

        SortVertical(_sortQue);
    }

    void RefreshSortQueItems(List<SortItem> _sortQue)
    {
        foreach (SortItem item in _sortQue)
        {
            List<SortItem> _targetSortQue = _layerQueLookup[item.SpriteRenderer.sortingLayerName];

            if (_targetSortQue != null && _targetSortQue != _sortQue)
            {
                AddToSortOrder(item);
            }
        }
    }

    void SortVertical(List<SortItem> sortQue)
    {
        sortQue.Sort((a, b) => b.SpriteRenderer.bounds.min.y.CompareTo(a.SpriteRenderer.bounds.min.y));

        for (int i = 0; i < sortQue.Count; i++)
        {
            SpriteRenderer _sprite = sortQue[i].SpriteRenderer;
            int _order = i * 2 + 2;

            _sprite.sortingOrder = _order;
        }
    }

}
