using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : MonoBehaviour
{
    public static WindowsManager Instance;

    [SerializeField] GameObject TableWindow;
    [SerializeField] GameObject ClubMenuWindow;
    [SerializeField] GameObject MainMenuWindow;
    [SerializeField] GameObject SimulationWindow;
    [SerializeField] SquadListPanel SquadListWindow;
    [SerializeField] SquadTacticPanel SquadTacticWindow;
    [SerializeField] GameObject ShopWindow;
    [HideInInspector]
    [SerializeField] List<GameObject> allWindows = new List<GameObject>();
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
        allWindows.Add(TableWindow);
        allWindows.Add(ClubMenuWindow);
        allWindows.Add(MainMenuWindow);
        // commentPanel needs to be active at all times, it is last in hierarchy so invisible unless all tabs inactive 
        //allWindows.Add(SimulationWindow); 
        allWindows.Add(ShopWindow); 
        allWindows.Add(SquadListWindow.gameObject);
        allWindows.Add(SquadTacticWindow.gameObject);
        ShowWindow("Main Menu");
    }
    void HideAllWindows()
    {
        foreach (var win in allWindows)
        {
            win.SetActive(false);
        }
    }
    public void ShowWindow(string name)
    {
        HideAllWindows();
        switch (name)
        {
            case "Club Menu": ClubMenuWindow.SetActive(true); break;
            case "Main Menu": MainMenuWindow.SetActive(true); break;
            case "Table": TableWindow.SetActive(true); break;
            case "Simulation": SimulationWindow.SetActive(true); break;
            case "SquadList": 
                SquadListWindow.gameObject.SetActive(true);
                SquadListWindow.Init();
                break;
            case "SquadTactic":
                SquadTacticWindow.gameObject.SetActive(true);
                SquadTacticWindow.SetUpSquadScreen();
                break;
            case "Shop": ShopWindow.SetActive(true);break;
            default:
                throw new KeyNotFoundException($"Window with name {name} does not exist!");
        }
    }
}
