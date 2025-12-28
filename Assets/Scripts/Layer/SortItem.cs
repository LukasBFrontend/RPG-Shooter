using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SortItem : MonoBehaviour
{
    public bool IncludeInPerLayerSorting = true;
    public SpriteRenderer SpriteRenderer;
    float _lastY;

    void OnDisable()
    {
        if (!IncludeInPerLayerSorting)
        {
            return;
        }

        SortItemManager.Instance.RemoveFromSortOrder(this);
    }

    void LateUpdate()
    {
        if (!IncludeInPerLayerSorting)
        {
            return;
        }

        if (Utils.VisibleToCamera(transform, Camera.main) && Mathf.Abs(transform.position.y - _lastY) > 0.01f)
        {
            SortItemManager.Instance.MarkDirty(this);
        }

        _lastY = transform.position.y;
    }
}
