using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCreationPanel : MonoBehaviour
{
    [SerializeField] Dropdown leagueDropDown, clubDropDown;
    public void StartGame()
    {
        MyClub.Instance.GenerateGameData(leagueDropDown.value, clubDropDown.value);
        WindowsManager.Instance.ShowWindow("Club");
    }

    public void UpdateClubsList(int chosenLeague)
    {
        List<string> cs = new List<string>();
        foreach (Club c in Database.leagueDB[chosenLeague].Teams)
        {
            cs.Add(c.Name);
        }
        clubDropDown.ClearOptions();
        clubDropDown.AddOptions(cs);
    }

    public void PrepareLeagueDropDown()
    {
        List<Dropdown.OptionData> leagueOptions = new List<Dropdown.OptionData>();
        foreach (League_Old lg in Database.leagueDB)
        {
            leagueOptions.Add(new Dropdown.OptionData(lg.Name, Resources.Load<Sprite>("Flags/" + lg.Country)));
        }
        leagueDropDown.AddOptions(leagueOptions);
        UpdateClubsList(0);
    }
}
