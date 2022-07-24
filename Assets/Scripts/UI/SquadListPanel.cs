using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SquadListPanel : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI _CoachText;
    [SerializeField, FormerlySerializedAs("squadListElements")] List<SquadListElement> _SquadListElements;
    [SerializeField] TMPro.TMP_Dropdown _TournamentSelectionDropdown;

    int _currLeagueTeamIndex;
    int _currTeamPlayersCount;

    private void Awake()
    {
        _currLeagueTeamIndex = MyClub.myInLeagueIndex;
    }

    public void Init()
    {
        _TournamentSelectionDropdown.ClearOptions();
        var tournaments = new List<string>(MyClub.myTournaments);
        tournaments.Insert(0, "");
        _TournamentSelectionDropdown.AddOptions(tournaments);
        SetupSquad();
    }

    public void SetupSquad(int id = -1)
    {
        if (id == -1)
            id = MyClub.myClubID;

        if(id == MyClub.myClubID)
        {
            _TournamentSelectionDropdown.gameObject.SetActive(true);
            _TournamentSelectionDropdown.value = 0;
        }
        else
            _TournamentSelectionDropdown.gameObject.SetActive(false);

        _CoachText.text = Database.clubDB[id].Name;

        _currTeamPlayersCount = Database.clubDB[id].FootballersIDs.Count;

        for (int i = 0; i < _SquadListElements.Count; i++)
        {
            if (i < _currTeamPlayersCount)
            {
                _SquadListElements[i].gameObject.SetActive(true);
                _SquadListElements[i].SetData(Database.footballersDB[Database.clubDB[id].FootballersIDs[i]], "");
            }
            else
                _SquadListElements[i].gameObject.SetActive(false);
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

    public void OnTournamentChanged(int option)
    {
        string tournament = option == 0 ? "" : _TournamentSelectionDropdown.options[option].text;
        for (int i = 0; i < _currTeamPlayersCount; i++)
        {
            _SquadListElements[i].UpdateStatistics(tournament);
        }
    }
}
