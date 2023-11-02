using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SquadListPanel : BasePanel
{
    [SerializeField] FootballerTableData _FootballersTableData;
    [SerializeField] TextMeshProUGUI _CoachText;
    [SerializeField] TMP_Dropdown _TournamentSelectionDropdown;
    [SerializeField] TMP_Dropdown _TableDataModeDropdown;
    [SerializeField] Button _AutoChoiceButton;

    int _currLeagueTeamIndex;

    void OnEnable()
    {
        _currLeagueTeamIndex = MyClub.Instance.MyInLeagueIndex;
        _TournamentSelectionDropdown.ClearOptions();
        _TableDataModeDropdown.ClearOptions();
        var tournaments = new List<string>(MyClub.Instance.MyTournaments);
        tournaments.Insert(0, "");
        _TournamentSelectionDropdown.AddOptions(tournaments);
        _TableDataModeDropdown.AddOptions(_FootballersTableData.GetAvailableTableDataModes());
        SetupSquad();
    }

    void SetupSquad() => SetupSquad(MyClub.Instance.MyClubID, true);

    void SetupSquad(int id, bool fieldsChanged = false)
    {
        bool isMyClub = id == MyClub.Instance.MyClubID;

        _TournamentSelectionDropdown.gameObject.SetActive(isMyClub);
        _TableDataModeDropdown.gameObject.SetActive(isMyClub);
        _AutoChoiceButton.gameObject.SetActive(isMyClub);

        if (isMyClub)
        {
            _TournamentSelectionDropdown.value = 0;
        }

        _TableDataModeDropdown.value = 0;

        _CoachText.text = Database.clubDB[id].Name;

        _FootballersTableData.ShowData(Database.GetFootballersFromClub(id), fieldsChanged, () => _FootballersTableData.ShowData(Database.GetFootballersFromClub(id)));
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

    public void OnTableDataModeChanged(int option)
    {
        _FootballersTableData.ChangeTableMode(_TableDataModeDropdown.options[option].text);
    }

    public void SquadAutoChoose()
    {
        CustomPanel.CustomPanelData customPanelData = new();
        customPanelData.Title = "Auto Squad Selection";
        customPanelData.Description = $"Do you want to automatically choose squad based on overall Rating?";
        customPanelData.OnConfirm = OnComfirm;
        WindowsManager.Instance.ShowWindow("Custom", customPanelData, false);

        void OnComfirm()
        {
            MyClub.Instance.Club.RefreshSquad();
            MyClub.Instance.UpdateCurrentPresetSquad();
            _FootballersTableData.ShowData(Database.GetFootballersFromClub(MyClub.Instance.MyClubID), false, () => Database.GetFootballersFromClub(MyClub.Instance.MyClubID));
        }
    }
}
