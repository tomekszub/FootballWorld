using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : MonoBehaviour
{
    public static WindowsManager Instance;

    public GameObject TableWindow;
    public GameObject ClubMenuWindow;
    public GameObject MainMenuWindow;
    public GameObject SimulationWindow;
    public SquadListPanel SquadWindow;
    public GameObject ShopWindow;
    [HideInInspector]
    public List<GameObject> allWindows = new List<GameObject>();
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
        allWindows.Add(SquadWindow.gameObject);
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
            case "Squad": 
                SquadWindow.gameObject.SetActive(true); 
                SquadWindow.SetupSquad(MyClub.myClubID);
                break;
            case "Shop": ShopWindow.SetActive(true);break;
            default:
                break;
        }
    }
}
