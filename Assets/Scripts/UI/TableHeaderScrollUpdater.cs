using UnityEngine;

public class TableHeaderScrollUpdater : MonoBehaviour
{
    [SerializeField] RectTransform _HeaderContentRect;
    [SerializeField] RectTransform _TableRowsContent;

    public void UpdateRect()
    {
        _HeaderContentRect.anchoredPosition = new Vector2(_TableRowsContent.anchoredPosition.x, _HeaderContentRect.anchoredPosition.y);
    }
}
