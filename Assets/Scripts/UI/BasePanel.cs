using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public class PanelData { }

    public void Show(PanelData panelData)
    {
        OnShow(panelData);
        gameObject.SetActive(true);
    }

    protected virtual void OnShow(PanelData panelData) { }
}
