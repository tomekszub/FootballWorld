using System.Collections.Generic;
using UnityEngine;

public class SquadListPanel : MonoBehaviour
{
    [SerializeField] FootballerTableData _FootballersTableData;
    [SerializeField] TMPro.TextMeshProUGUI _CoachText;
    [SerializeField] TMPro.TMP_Dropdown _TournamentSelectionDropdown;

    int _currLeagueTeamIndex;

    private void Awake()
    {
        _currLeagueTeamIndex = MyClub.Instance.MyInLeagueIndex;
    }

    void OnEnable()
    {
        _TournamentSelectionDropdown.ClearOptions();
        var tournaments = new List<string>(MyClub.Instance.MyTournaments);
        tournaments.Insert(0, "");
        _TournamentSelectionDropdown.AddOptions(tournaments);
        SetupSquad(fieldsChanged: true);
    }

    public void SetupSquad(int id = -1, bool fieldsChanged = false)
    {
        if (id == -1)
            id = MyClub.Instance.MyClubID;

        if(id == MyClub.Instance.MyClubID)
        {
            _TournamentSelectionDropdown.gameObject.SetActive(true);
            _TournamentSelectionDropdown.value = 0;
        }
        else
            _TournamentSelectionDropdown.gameObject.SetActive(false);

        _CoachText.text = Database.clubDB[id].Name;

        _FootballersTableData.ShowData(Database.Instance.GetFootballersFromClub(id), fieldsChanged);
    }

    public void ShowNextClub()
    {
        _currLeagueTeamIndex++;
        if(_currLeagueTeamIndex >= Database.leagueDB[MyClub.Instance.MyLeagueID].Teams.Count)
            _currLeagueTeamIndex = 0;
        SetupSquad(Database.leagueDB[MyClub.Instance.MyLeagueID].Teams[_currLeagueTeamIndex].Id);
    }

    public void ShowPreviousClub()
    {
        _currLeagueTeamIndex--;
        if (_currLeagueTeamIndex <= -1)
            _currLeagueTeamIndex = Database.leagueDB[MyClub.Instance.MyLeagueID].Teams.Count - 1;
        SetupSquad(Database.leagueDB[MyClub.Instance.MyLeagueID].Teams[_currLeagueTeamIndex].Id);
    }

    public void OnTournamentChanged(int option)
    {
        string tournament = option == 0 ? "" : _TournamentSelectionDropdown.options[option].text;
        _FootballersTableData.UpdateStats(tournament);
    }
}
