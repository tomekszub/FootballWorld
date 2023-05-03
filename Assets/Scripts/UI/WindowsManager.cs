using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : SerializedMonoBehaviour
{
    public static WindowsManager Instance;

    [SerializeField] Dictionary<string, GameObject> _UIPanels;

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
            panel.Value.SetActive(false);
        }
    }
    public void ShowWindow(string name)
    {
        HideAllWindows();
        _UIPanels[name].SetActive(true);
    }
}
