using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : SerializedMonoBehaviour
{
    public static WindowsManager Instance;

    [SerializeField] Dictionary<string, BasePanel> _UIPanels;

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        ShowWindow("MainMenu");
    }

    void HideAllWindows()
    {
        foreach (var panel in _UIPanels)
        {
            panel.Value.gameObject.SetActive(false);
        }
    }

    public void ShowWindow(string name)
    {
        ShowWindow(name, new BasePanel.PanelData(), true);
    }

    public void ShowWindow(string name, BasePanel.PanelData panelData, bool hideOtherWindows)
    {
        if(hideOtherWindows)
            HideAllWindows();
        _UIPanels[name].Show(panelData);
    }
}
