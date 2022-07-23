using System.Collections.Generic;
using UnityEngine;

public class SquadListPanel : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI _CoachText;
    [SerializeField] List<SquadListElement> squadListElements;

    int _currLeagueTeamIndex;

    private void Awake()
    {
        _currLeagueTeamIndex = MyClub.myInLeagueIndex;
    }
    public void SetupSquad(int id)
    {
        _CoachText.text = Database.clubDB[id].Name;

        //squadListElements.ForEach(element => element.gameObject.SetActive(false));
        int squadCount = Database.clubDB[id].FootballersIDs.Count;

        for (int i = 0; i < squadListElements.Count; i++)
        {
            if (i < squadCount)
            {
                squadListElements[i].gameObject.SetActive(true);
                squadListElements[i].SetData(Database.footballersDB[Database.clubDB[id].FootballersIDs[i]]);
            }
            else
                squadListElements[i].gameObject.SetActive(false);
        }

    }

    public void ShowNextClub()
    {
        _currLeagueTeamIndex++;
        if(_currLeagueTeamIndex >= Database.leagueDB[MyClub.myLeagueID].Teams.Count)
            _currLeagueTeamIndex = 0;
        SetupSquad(Database.leagueDB[MyClub.myLeagueID].Teams[_currLeagueTeamIndex].Id);
    }

    public void ShowPreviousClub()
    {
        _currLeagueTeamIndex--;
        if (_currLeagueTeamIndex <= -1)
            _currLeagueTeamIndex = Database.leagueDB[MyClub.myLeagueID].Teams.Count - 1;
        SetupSquad(Database.leagueDB[MyClub.myLeagueID].Teams[_currLeagueTeamIndex].Id);
    }
}
