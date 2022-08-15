using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(ScrollRect))]
public class ScrollRectSnapper : MonoBehaviour, IPointerDownHandler
{
    ScrollRect rekt;
    bool _ScrollLocked = true;

    public void OnPointerDown(PointerEventData eventData)
    {
        _ScrollLocked = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        rekt = gameObject.GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_ScrollLocked)
            rekt.verticalNormalizedPosition = 0;
        else if (rekt.verticalNormalizedPosition < -0.1)
            _ScrollLocked = true;
    }

    public void ResetScrollLocker() => _ScrollLocked = true;
}
