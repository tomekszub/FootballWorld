using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class MyClub : MonoBehaviour 
{
    public static MyClub Instance;

    public int MyLeagueID { get; private set; }
    public int MyInLeagueIndex { get; private set; }
    public int MyClubID { get; private set; }
    public HashSet<string> MyTournaments { get; private set; } = new HashSet<string>();
	public List<Match> Matches { get; set; } = new List<Match>();

    [SerializeField] GameObject _SquadCell, _EmptyCell;
    [SerializeField] GameObject _SquadPanelsParent, _SubstitutesParent;
    [SerializeField] TextMeshProUGUI _DateText;
    [SerializeField] GameObject _CalendarMatchEntry;
    [SerializeField] GameObject _CalendarEntriesParent;
    [SerializeField] TextMeshProUGUI _ScorersText;
    [SerializeField] TextMeshProUGUI _OutputClubName;
    [SerializeField] GameObject _MainMenuPanel;
    [SerializeField] GameObject _ClubMenuPanel;
    [SerializeField] Dropdown _FormationDropDown;
	[SerializeField] Table _TableScript;

    string _leagueName;
    DateTime _startOfTheSeason = new DateTime(2018, 8, 17);
    DateTime _currDate = new DateTime(2018, 6, 24);
    readonly DateTime[] _cupDates = { new DateTime(2018, 9, 4), new DateTime(2018, 10, 16), new DateTime(2018, 10, 30), new DateTime(2019, 1, 8), new DateTime(2019, 1, 22), new DateTime(2019, 2, 5), new DateTime(2019, 5, 21)};
    int _currentMatch = 0, _firstMatchOfTheWeek = 0;
    List<CupKnockoutStage> _nationalCup = new List<CupKnockoutStage>();
    List<CupRound> _qualCL = new List<CupRound>();
    List<Club> _clubs;
	int _teamsNumber;
    int _currCupRound = 0, _currCLRound = 0;
    MatchStats _leagueScorers = new MatchStats(new List<Scorer>());

    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        else
            Instance = this;
    }

    void Start () 
	{
        _startOfTheSeason = _startOfTheSeason.AddDays(1);
    }

	public void CreateTeam(int leagueID, int inLeagueIndex)
	{
        MyLeagueID = leagueID;
        _teamsNumber = Database.leagueDB [MyLeagueID].Teams.Count;
        _leagueName = Database.leagueDB[MyLeagueID].Name;
        MyInLeagueIndex = inLeagueIndex;
        MyClubID = Database.leagueDB[MyLeagueID].Teams[MyInLeagueIndex].Id;
        _OutputClubName.text = Database.clubDB[MyClubID].Name;

        for (int i = 0; i < _teamsNumber; i++) 
		{
			_TableScript.tableTeams.Add(new Team(Database.leagueDB[MyLeagueID].Teams[i].Id ,Database.leagueDB[MyLeagueID].Teams[i].Name, 0, 0, 0, 0, 0, 0));
		}
        _TableScript.ShowTable ();

        _clubs = new List<Club>();
        for (int i = 0; i < _teamsNumber; i++)
        {
            _clubs.Add(Database.leagueDB[MyLeagueID].Teams[i]);
        }

        // -1 oznacza ze numer rundy ma byc automatyczny, to znaczy kazda runda inny id rundy (dla ligi jest ok, np dla fazy gr lm trzeba to ustawic)
        MatchCalendar.CreateGroupCalendar(Database.leagueDB[MyLeagueID].Name, -1, _clubs, _startOfTheSeason);
        MyTournaments.Add(Database.leagueDB[MyLeagueID].Name);
        CreateCupCalendar();
        MyTournaments.Add("National Cup");
        CreateChampionsLeagueCalendar();
        MyTournaments.Add("Champions Cup");
        UpdateCalendar();

        _MainMenuPanel.SetActive (false);
        UpdateCurrentDateUI();
    }

	public void NextMatch()
	{
        _CalendarEntriesParent.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1;
        if (_currentMatch >= Matches.Count)
        {
            Debug.Log("Koniec meczów!");
            return;
        }
        if (Matches[_currentMatch].Date != _currDate)
        {
            _currDate = _currDate.AddDays(1);
            UpdateCurrentDateUI();
            UpdateCalendar();
            return;
        }
        int hostId, guestId;
        bool endOfRound = false;
        do
        {
            if (_currentMatch >= Matches.Count)
            {
                Debug.Log("Koniec meczów!");
                return;
            }
            if((_currentMatch+1 < Matches.Count && Matches[_currentMatch].Date < Matches[_currentMatch+1].Date) || _currentMatch == Matches.Count-1)
            {
                endOfRound = true;
            }

            hostId = Matches[_currentMatch].FirstTeamId;
            guestId = Matches[_currentMatch].SecondTeamId;
            if (hostId != MyClubID && guestId != MyClubID)
            {
                MatchStats[] ms = Simulation.SimulationStart(hostId, guestId, Matches[_currentMatch].CompetitionName);
                ProcesssMatchStats(ms, Matches[_currentMatch].CompetitionName);
                if (endOfRound)
                {
                    // add addDay(1) if we want automatically advance to the next day, otherwise we will ahve to click next match button after finished day
                    //UpdateCalendar();
                    return;
                }
            }
            
        }
        while (hostId != MyClubID && guestId != MyClubID);

        _ClubMenuPanel.SetActive(false);
        Comment.Instance.Init(hostId, guestId, Matches[_currentMatch].CompetitionName);
    }

    private void UpdateCurrentDateUI()
    {
        _DateText.text = _currDate.Day + "/" + _currDate.Month;
    }

    public void ProcesssMatchStats(MatchStats[] matchStats, string competitionName)
    {
        int hostId = Matches[_currentMatch].FirstTeamId;
        int guestId = Matches[_currentMatch].SecondTeamId;
        Matches[_currentMatch].Result.HostGoals = matchStats[0].GetGoals();
        Matches[_currentMatch].Result.GuestGoals = matchStats[1].GetGoals();
        Matches[_currentMatch].Finished = true;
        if (competitionName == _leagueName)
        {
            #region Table
            if (matchStats[0].GetGoals() > matchStats[1].GetGoals())
            {
                for (int i = 0; i < _TableScript.tableTeams.Count; i++)
                {
                    if (_TableScript.tableTeams[i].Id == hostId)
                    {
                        _TableScript.tableTeams[i].LostGoals += matchStats[1].GetGoals();
                        _TableScript.tableTeams[i].Points += 3;
                        _TableScript.tableTeams[i].ScoredGoals += matchStats[0].GetGoals();
                        _TableScript.tableTeams[i].Wins++;
                        _TableScript.tableTeams[i].MatchesPlayed++;
                        _TableScript.tableTeams[i].DifferenceGoals += matchStats[0].GetGoals() - matchStats[1].GetGoals();
                    }
                    if (_TableScript.tableTeams[i].Id == guestId)
                    {
                        _TableScript.tableTeams[i].LostGoals += matchStats[0].GetGoals();
                        _TableScript.tableTeams[i].ScoredGoals += matchStats[1].GetGoals();
                        _TableScript.tableTeams[i].Loses++;
                        _TableScript.tableTeams[i].MatchesPlayed++;
                        _TableScript.tableTeams[i].DifferenceGoals -= matchStats[0].GetGoals() - matchStats[1].GetGoals();
                    }
                }
            }
            else if (matchStats[0].GetGoals() < matchStats[1].GetGoals())
            {
                for (int i = 0; i < _TableScript.tableTeams.Count; i++)
                {
                    if (_TableScript.tableTeams[i].Id == hostId)
                    {
                        _TableScript.tableTeams[i].LostGoals += matchStats[1].GetGoals();
                        _TableScript.tableTeams[i].ScoredGoals += matchStats[0].GetGoals();
                        _TableScript.tableTeams[i].Loses++;
                        _TableScript.tableTeams[i].MatchesPlayed++;
                        _TableScript.tableTeams[i].DifferenceGoals += matchStats[0].GetGoals() - matchStats[1].GetGoals();
                    }
                    if (_TableScript.tableTeams[i].Id == guestId)
                    {
                        _TableScript.tableTeams[i].LostGoals += matchStats[0].GetGoals();
                        _TableScript.tableTeams[i].Points += 3;
                        _TableScript.tableTeams[i].ScoredGoals += matchStats[1].GetGoals();
                        _TableScript.tableTeams[i].Wins++;
                        _TableScript.tableTeams[i].MatchesPlayed++;
                        _TableScript.tableTeams[i].DifferenceGoals -= matchStats[0].GetGoals() - matchStats[1].GetGoals();
                    }
                }
            }
            else
            {
                for (int i = 0; i < _TableScript.tableTeams.Count; i++)
                {
                    if (_TableScript.tableTeams[i].Id == hostId)
                    {
                        _TableScript.tableTeams[i].LostGoals += matchStats[1].GetGoals();
                        _TableScript.tableTeams[i].Points += 1;
                        _TableScript.tableTeams[i].ScoredGoals += matchStats[0].GetGoals();
                        _TableScript.tableTeams[i].Draws++;
                        _TableScript.tableTeams[i].MatchesPlayed++;
                    }
                    if (_TableScript.tableTeams[i].Id == guestId)
                    {
                        _TableScript.tableTeams[i].LostGoals += matchStats[0].GetGoals();
                        _TableScript.tableTeams[i].Points += 1;
                        _TableScript.tableTeams[i].ScoredGoals += matchStats[1].GetGoals();
                        _TableScript.tableTeams[i].Draws++;
                        _TableScript.tableTeams[i].MatchesPlayed++;
                    }
                }
            }
            #endregion
            foreach (var s in matchStats[0].scorers)
            {
                _leagueScorers.AddScorer(s);
            }
            foreach (var s in matchStats[1].scorers)
            {
                _leagueScorers.AddScorer(s);
            }
            _leagueScorers.scorers = _leagueScorers.scorers.OrderByDescending(n => n.goals).ToList();
            _ScorersText.text = "";
            for (int i = 0; i < 8; i++)
            {
                if (_leagueScorers.scorers.Count >= i + 1) _ScorersText.text += _leagueScorers.scorers[i].goals + "\t" + _leagueScorers.scorers[i].name + " " + _leagueScorers.scorers[i].surname + "\n";
            }
        }
        else if(competitionName == "National Cup")
        {
            //Debug.Log("Mecz pucharowy");
            _nationalCup[Matches[_currentMatch].RoundIndex].SendMatchResult(Matches[_currentMatch]);
            if (_nationalCup[_currCupRound].GetWinners() != null)
            {
                //time for new round
                //we are checking if the previous round was a final (number of winners == 1), if not..
                if(_nationalCup[_currCupRound].GetWinners().Count != 1)
                {
                    _nationalCup.Add(new CupKnockoutStage("National Cup",null, _nationalCup[_currCupRound].GetWinners()));
                    _currCupRound++;
                    /*int ind = 0;
                    int roundInQuestion = 4 + currCupRound * (roundnumber / 5);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        //TODO: improve this loop eg. upon contact with roundindex that not the correct index and its the leage match increment i "teamNumber/2" times
                        if (matches[i].CompetitionName == leagueName && matches[i].RoundIndex == roundInQuestion)
                        {
                            ind = i + teamsNumber/2 - 1;
                            break;
                        }
                    }
                    AddCupMatchesToCalendar(ind);*/
                    AddEuropaCupMatchesToCalendar(_nationalCup[_currCupRound].GetMatches(), _currCupRound, _cupDates[_currCupRound], _cupDates[_currCupRound].AddDays(7));
                    
                } 
            }
        }
        else if(competitionName == "Champions Cup")
        {
            _qualCL[Matches[_currentMatch].RoundIndex].SendMatchResult(Matches[_currentMatch]);
            if (_qualCL[_currCLRound].GetWinners() != null)
            {
                if (_currCLRound == 0)
                {
                    List<Club> cs = new List<Club>();
                    for (int i = 0; i < Database.leagueDB.Count; i++)
                    {
                        Club c = Database.leagueDB[i].GetFirstQ_CL_Clubs(i + 1);
                        if (c != null) cs.Add(c);
                    }
                    if (cs.Count != 33)
                    {
                        Debug.LogError("It should be 33 added teams in this round!");
                        return;
                    }
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", cs, _qualCL[0].GetWinners(), true, 2));
                    _currCLRound++;
                    Match[] ms = _qualCL[1].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms,1, new DateTime(2018,7,10), new DateTime(2018, 7, 17));
                }
                else if (_currCLRound == 1)
                {
                    List<Club> cs = new List<Club>();
                    for (int i = 0; i < Database.leagueDB.Count; i++)
                    {
                        Club c = Database.leagueDB[i].GetSecondChampionsPathQ_CL_Clubs(i + 1);
                        if (c != null) cs.Add(c);
                    }
                    if (cs.Count != 3)
                    {
                        Debug.LogError("It should be 3 added teams in this round!");
                        return;
                    }
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", cs, _qualCL[1].GetWinners(), true, 2));
                    _currCLRound++;
                    // league path
                    cs = new List<Club>();
                    for (int i = 0; i < Database.leagueDB.Count; i++)
                    {
                        Club c = Database.leagueDB[i].GetSecondLeaguePathQ_CL_Clubs(i + 1);
                        if (c != null) cs.Add(c);
                    }
                    if (cs.Count != 6)
                    {
                        Debug.LogError("It should be 6 added teams in this round!");
                        return;
                    }
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", cs, null, true, 2));
                    _currCLRound++;
                    //

                    Match[] ms = _qualCL[2].GetMatches();
                    Match[] ms2 = _qualCL[3].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms,2, new DateTime(2018, 7, 24), new DateTime(2018, 7, 31));
                    AddEuropaCupMatchesToCalendar(ms2,3, new DateTime(2018, 7, 24), new DateTime(2018, 7, 31));
                }
                else if (_currCLRound == 3)
                {
                    List<Club> cs = new List<Club>();
                    for (int i = 0; i < Database.leagueDB.Count; i++)
                    {
                        Club c = Database.leagueDB[i].GetThirdChampionsPathQ_CL_Clubs(i + 1);
                        if (c != null) cs.Add(c);
                    }
                    if (cs.Count != 2)
                    {
                        Debug.LogError("It should be 2 added teams in this round!");
                        return;
                    }
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", cs, _qualCL[2].GetWinners(), true, 2));
                    _currCLRound++;
                    cs = new List<Club>();
                    for (int i = 0; i < Database.leagueDB.Count; i++)
                    {
                        Club c = Database.leagueDB[i].GetThirdLeaguePathQ_CL_Clubs(i + 1);
                        if (c != null) cs.Add(c);
                    }
                    if (cs.Count != 5)
                    {
                        Debug.LogError("It should be 5 added teams in this round!");
                        return;
                    }
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", cs, _qualCL[3].GetWinners(), true, 2));
                    _currCLRound++;
                    Match[] ms = _qualCL[4].GetMatches();
                    Match[] ms2 = _qualCL[5].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms,4, new DateTime(2018, 8, 7), new DateTime(2018, 8, 14));
                    AddEuropaCupMatchesToCalendar(ms2,5, new DateTime(2018, 8, 7), new DateTime(2018, 8, 14));
                }
                else if (_currCLRound == 5)
                {
                    List<Club> cs = new List<Club>();
                    for (int i = 0; i < Database.leagueDB.Count; i++)
                    {
                        Club c = Database.leagueDB[i].GetPlayOffsChampionsPathQ_CL_Clubs(i + 1);
                        if (c != null) cs.Add(c);
                    }
                    if (cs.Count != 2)
                    {
                        Debug.LogError("It should be 2 added teams in this round!");
                        return;
                    }
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", cs, _qualCL[4].GetWinners(), true, 2));
                    _currCLRound++;
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", null, _qualCL[5].GetWinners(), true, 2));
                    _currCLRound++;
                    Match[] ms = _qualCL[6].GetMatches();
                    Match[] ms2 = _qualCL[7].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms,6, new DateTime(2018, 8, 21), new DateTime(2018, 8, 28));
                    AddEuropaCupMatchesToCalendar(ms2,7, new DateTime(2018, 8, 21), new DateTime(2018, 8, 28));
                }
                else if(_currCLRound == 7)
                {
                    List<Club> cs = new List<Club>();
                    for (int i = 0; i < Database.leagueDB.Count; i++)
                    {
                        List<Club> c = Database.leagueDB[i].GetGroupStage_CL_Clubs(i + 1);
                        if (c != null) cs.AddRange(c);
                    }
                    if (cs.Count != 26)
                    {
                        Debug.LogError("It should be 26 added teams in this round!");
                        return;
                    }
                    List<Club> pw = new List<Club>();
                    pw.AddRange(_qualCL[6].GetWinners());
                    pw.AddRange(_qualCL[7].GetWinners());
                    _qualCL.Add(new CupGroupStage("Champions Cup",8, new DateTime(2018, 9, 17),14,cs, pw));
                    _currCLRound++;
                    /*Match[] ms = qualCL[8].GetMatches();
                    foreach (var item in ms)
                    {
                        Debug.LogWarning(item.Date + " " + item.FirstTeamId + " " + item.SecondTeamId);
                    }*/
                }
                else if(_currCLRound == 8)                       // 1/8 finalu
                {
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", null, _qualCL[8].GetWinners(), true, 2, true));
                    _currCLRound++;
                    Match[] ms = _qualCL[9].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms, 9, new DateTime(2019, 2, 19), new DateTime(2019, 3, 12));
                }
                else if (_currCLRound == 9)                      // 1/4 finalu
                {
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", null, _qualCL[9].GetWinners(), true, 1));
                    _currCLRound++;
                    Match[] ms = _qualCL[10].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms, 10, new DateTime(2019, 4, 9), new DateTime(2019, 4, 16));
                }
                else if (_currCLRound == 10)                      // 1/2 finalu
                {
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", null, _qualCL[10].GetWinners(), true, 1));
                    _currCLRound++;
                    Match[] ms = _qualCL[11].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms, 11, new DateTime(2019, 4, 30), new DateTime(2019, 5, 7));
                }
                else if (_currCLRound == 11)                      // grand finale
                {
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", null, _qualCL[11].GetWinners(), false, 1));
                    _currCLRound++;
                    Match[] ms = _qualCL[12].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms, 12, new DateTime(2019, 5, 28), new DateTime(), false);
                }
            }
        }
        _currentMatch++;
        UpdateCalendar();
    }
    void UpdateCalendar()
    {
        // if current match is not in the same day that the first match of the day was we just set firstmatchoftheday to current match index
        // what it means is that we keep track of first match of the current day to list all of them in the list (ui vertical layout)
        if (_currentMatch != 0 && Matches[_currentMatch-1].Date != _currDate && Matches[_firstMatchOfTheWeek].Date != Matches[_currentMatch].Date) _firstMatchOfTheWeek = _currentMatch;
        int todayMatches = 0, currMatchOfTheDay = 0;
        DateTime nextMatchDay = Matches[_firstMatchOfTheWeek].Date;
        for (int i = _firstMatchOfTheWeek; i < Matches.Count; i++)
        {
            if (Matches[i].Date == nextMatchDay) 
                todayMatches++;
            else 
                break;
        }

        string resultHelperString = "";

        foreach (Transform child in _CalendarEntriesParent.transform)
        {
            if (currMatchOfTheDay == todayMatches) 
                Destroy(child.gameObject);
            else
            {
                if (Matches[_firstMatchOfTheWeek + currMatchOfTheDay].Finished) 
                    resultHelperString = " " + Matches[_firstMatchOfTheWeek + currMatchOfTheDay].Result.HostGoals + " - " + Matches[_firstMatchOfTheWeek + currMatchOfTheDay].Result.GuestGoals + " ";
                else 
                    resultHelperString = " - ";
                child.GetComponentsInChildren<Text>()[1].text = GetClubNameByID(Matches[_firstMatchOfTheWeek + currMatchOfTheDay].FirstTeamId) + resultHelperString + GetClubNameByID(Matches[_firstMatchOfTheWeek + currMatchOfTheDay].SecondTeamId);
                currMatchOfTheDay++;
            }
        }
        for (int i = currMatchOfTheDay; i < todayMatches; i++)
        {
            GameObject go = Instantiate(_CalendarMatchEntry, _CalendarEntriesParent.transform);
            if (Matches[_firstMatchOfTheWeek + currMatchOfTheDay].Finished) 
                resultHelperString = " " + Matches[_firstMatchOfTheWeek + currMatchOfTheDay].Result.HostGoals + " - " + Matches[_firstMatchOfTheWeek + currMatchOfTheDay].Result.GuestGoals + " ";
            else 
                resultHelperString = " - ";
            go.GetComponentsInChildren<Text>()[1].text = GetClubNameByID(Matches[_firstMatchOfTheWeek + i].FirstTeamId) + resultHelperString + GetClubNameByID(Matches[_firstMatchOfTheWeek + i].SecondTeamId);
        }
        
    }
    string GetClubNameByID(int id)
    {
        if (_clubs != null)
        { 
            foreach (var c in _clubs)
            {
                if (c.Id == id) return c.Name;
            }
        }
        //not found in the domestic clubs
        Club cl = Database.GetClubByID(id);
        if (cl != null) return cl.Name;
        else return "";
    }

    public void CreateChampionsLeagueCalendar()
    {
        List<Club> cs = new List<Club>();
        for (int i = 0; i < Database.leagueDB.Count; i++)
        {
            Club c = Database.leagueDB[i].GetPreeliminary_CL_Clubs(i + 1);
            if(c != null)cs.Add(c);
        }
        if (cs.Count != 2)
        {
            Debug.LogError("It should be 2 teams in this round!");
            return;
        }
        _qualCL.Add(new CupKnockoutStage("Champions Cup",cs, null, false));
        Match[] ms = _qualCL[0].GetMatches();
        foreach (var m in ms)
        {
            //m.CompetitionName = "Champions Cup";
            m.Date = new DateTime(2018, 6, 25);
            m.RoundIndex = 0;
        }
        Matches.InsertRange(0, ms);
    }
    public void CreateCupCalendar()
    {
        List<Club> cs = new List<Club>();
        int teams = 1;
        // look for a closest power of 2
        for (int i = 1; i < _teamsNumber; i++)
        {
            if (teams*2 <= _teamsNumber) teams *= 2;
            else break;
        }
        for (int i = 0; i < teams; i++)
        {
            cs.Add(Database.leagueDB[MyLeagueID].Teams[i]);
        }
        _nationalCup.Add(new CupKnockoutStage("National Cup",cs));
        //int ind = 0;
        /*for (int i = 0; i < matches.Count; i++)
        {
            if ()
            {
                ind = i + teamsNumber / 2 - 1;
                break;
            }
        }
        AddCupMatchesToCalendar(ind);*/
        AddEuropaCupMatchesToCalendar(_nationalCup[0].GetMatches(), 0, _cupDates[0], _cupDates[0].AddDays(7));
    }
    void AddCupMatchesToCalendar(int indexOfRoundBeforeCup)
    {
        DateTime newDate;
        newDate = Matches[indexOfRoundBeforeCup].Date.AddDays(3);
        Match[] m = _nationalCup[_currCupRound].GetMatches();
        int temp = 0;
        for (int i = 0; i < m.Length; i++)
        {
            //Debug.Log(Database.GetClubByID(nationalCup[0].GetMatches()[i].FirstTeamId).Name + " - " + Database.GetClubByID(nationalCup[0].GetMatches()[i].SecondTeamId).Name);
            if (i < m.Length / 2)
            {
                m[i].Date = newDate;
                temp = indexOfRoundBeforeCup + 1 + i;
                Matches.Insert(temp, new Match(m[i]));
            }
            else
            {
                m[i].Date = newDate.AddDays(7);
                temp = indexOfRoundBeforeCup + 1 + _teamsNumber / 2 + i;
                Matches.Insert(temp, new Match(m[i]));
            }
        }
    }
    void AddEuropaCupMatchesToCalendar(Match[] ms,int roundInd, DateTime firstLeg, DateTime secondLeg,bool twoLeg = true)
    {
        if(twoLeg)
        {
            int firstLegIndex = -1, secondLegIndex = -1;
            for (int i = 0; i < Matches.Count; i++)
            {
                if (firstLegIndex == -1)
                {
                    if (Matches[i].Date > firstLeg)
                    {
                        firstLegIndex = i;
                        // zabezpieczenie w przypadku gdy nastepny mecz ma date wieksza od firsleg jak i od secondleg
                        if (Matches[i].Date > secondLeg)
                        {
                            secondLegIndex = i + ms.Length / 2;
                            break;
                        }
                    }
                }
                else if (Matches[i].Date > secondLeg)
                {
                    secondLegIndex = i + ms.Length / 2;
                    break;
                }
            }
            // zabezpieczenie przed bledem zwiazanym z pustym kalendarzem
            if(Matches.Count == 0)
            {
                firstLegIndex = 0;
                secondLegIndex = ms.Length / 2;
            }
            // zabezpiecza przed przypadkiem w ktorym mecze odbywac sie beda na koncu kalendarza
            if (firstLegIndex == -1 && secondLegIndex == -1)
            {
                firstLegIndex = Matches.Count;
                secondLegIndex = firstLegIndex + ms.Length / 2;
            }
            if (secondLegIndex == -1)
            {
                secondLegIndex = Matches.Count + ms.Length / 2;
            }
            for (int i = 0; i < ms.Length / 2; i++)
            {
                ms[i].Date = firstLeg;
                ms[i].RoundIndex = roundInd;
                Matches.Insert(firstLegIndex, ms[i]);
            }
            for (int i = ms.Length / 2; i < ms.Length; i++)
            {
                ms[i].Date = secondLeg;
                ms[i].RoundIndex = roundInd;
                Matches.Insert(secondLegIndex, ms[i]);
            }
        }
        else
        {
            foreach (var item in ms)
            {
                item.Date = firstLeg;
                item.RoundIndex = roundInd;
            }
            if(Matches.Count == 0)
            {
                Matches.InsertRange(0, ms);
                return;
            }
            for (int i = 0; i < Matches.Count; i++)
            {
                if (Matches[i].Date > firstLeg)
                {
                    Matches.InsertRange(i, ms);
                    return;
                }
            }
            // w razie gdyby data byla koncowka rozgrywek czyli np liga mistrzow final to
            Matches.InsertRange(Matches.Count, ms);
        }
    }
    void ClearSquadCells()
    {
        for (int x = 0; x < _SquadPanelsParent.transform.childCount; x++)
        {
            int limit = _SquadPanelsParent.transform.GetChild(x).childCount;
            for (int i = limit - 1; i >= 0; i--)
            {
                Destroy(_SquadPanelsParent.transform.GetChild(x).GetChild(i).gameObject);
            }
        }
        int end = _SubstitutesParent.transform.childCount;
        for (int i = end - 1; i >= 0; i--)
        {
            Destroy(_SubstitutesParent.transform.GetChild(i).gameObject);
        }
    }
    public void FormationDropDownValue(int val)
    {
        UpdateSquadScreen(_FormationDropDown.options[val].text);
    }
    public void SetUpSquadScreen()
    {
        // set dropdown value to current formation
        for (int i = 0; i < _FormationDropDown.options.Count; i++)
        {
            if(_FormationDropDown.options[i].text == Database.clubDB[MyClubID].Formation)
            {
                _FormationDropDown.value = i;
                break;
            }
        }
        UpdateSquadScreen();
    }
    public void UpdateSquadScreen(string formation = "")
    {
        ClearSquadCells();
        for (int i = 11; i < Database.clubDB[MyClubID].FootballersIDs.Count; i++)
        {
            GameObject g = Instantiate(_SquadCell, _SubstitutesParent.transform);
            g.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[i]]);
        }
        if(formation == "")formation = Database.clubDB[MyClubID].Formation;
        GameObject go;
        if (formation == "4-3-3")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[10]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[9]]);
        }
        else if (formation == "4-2-3-1")
        {
            //go = Instantiate(emptyCell, squadPanelsParent.transform.GetChild(2));
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[6]]);
            //go = Instantiate(emptyCell, squadPanelsParent.transform.GetChild(2));
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[10]]);

        }
        else if (formation == "4-4-1-1")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[10]]);
        }
        else if (formation == "4-1-4-1")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[10]]);
        }
        else if (formation == "4-3-1-2")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[10]]);
        }
        else if (formation == "4-4-2")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[10]]);

        }
        else if (formation == "5-3-2")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[10]]);
        }
        else if (formation == "3-4-1-2")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[4]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[10]]);
        }
        else if(formation == "3-4-2-1")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[4]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[10]]);
        }
        for (int i = 1; i <= int.Parse(formation[0].ToString()); i++)
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(4));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[i]]);
        }
        go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(5));
        go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClubID].FootballersIDs[0]]);
        //squadWindowNeedsUpdate = false;
    }

    public void SetMySquad()
    {
        string formation = _FormationDropDown.options[_FormationDropDown.value].text;
        for (int i = 11; i < Database.clubDB[MyClubID].FootballersIDs.Count; i++)
        {
            Database.clubDB[MyClubID].FootballersIDs[i] = _SubstitutesParent.transform.GetChild(i-11).GetComponent<SquadCell>().GetFootballerID();
        }
        // save
        Database.clubDB[MyClubID].FootballersIDs[0] = _SquadPanelsParent.transform.GetChild(5).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
        int defendersCount = int.Parse(formation[0].ToString());
        for (int i = 0; i < defendersCount; i++)
        {
            Database.clubDB[MyClubID].FootballersIDs[i+1] = _SquadPanelsParent.transform.GetChild(4).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
        }
        if(formation.Length == 5)  // formacje 4-3-3 4-4-2 3-5-2 itd.
        {
            int midFieldNumber = int.Parse(formation[2].ToString());
            if (midFieldNumber == 1)
            {
                // nie ma poki co i chyba nie bedzie formacji 4-1-5
            }
            else
            {
                int inTheMiddle = midFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClubID].FootballersIDs[defendersCount+i] = _SquadPanelsParent.transform.GetChild(2).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClubID].FootballersIDs[defendersCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClubID].FootballersIDs[defendersCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(2).GetChild(midFieldNumber-1).GetComponent<SquadCell>().GetFootballerID();
            }
            int attackersNumber = int.Parse(formation[4].ToString());
            int defAndMidCount = defendersCount + midFieldNumber;
            if (attackersNumber == 1)
            {
                Database.clubDB[MyClubID].FootballersIDs[10] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = attackersNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClubID].FootballersIDs[defAndMidCount + inTheMiddle + i + 1] = _SquadPanelsParent.transform.GetChild(0).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClubID].FootballersIDs[defAndMidCount + 1] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClubID].FootballersIDs[defAndMidCount + 2] = _SquadPanelsParent.transform.GetChild(0).GetChild(attackersNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
        }
        else if(formation.Length == 7)
        {
            int defmidFieldNumber = int.Parse(formation[2].ToString());
            if (defmidFieldNumber == 1)
            {
                // 4-1-3-2
                Database.clubDB[MyClubID].FootballersIDs[defendersCount + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = defmidFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClubID].FootballersIDs[defendersCount + i] = _SquadPanelsParent.transform.GetChild(2).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClubID].FootballersIDs[defendersCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClubID].FootballersIDs[defendersCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(2).GetChild(defmidFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int midFieldNumber = int.Parse(formation[4].ToString());
            if (midFieldNumber == 1)
            {
                // 4-3-1-2
                Database.clubDB[MyClubID].FootballersIDs[defendersCount + defmidFieldNumber + 1] = _SquadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = midFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClubID].FootballersIDs[defendersCount + defmidFieldNumber + i] = _SquadPanelsParent.transform.GetChild(1).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClubID].FootballersIDs[defendersCount + defmidFieldNumber + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClubID].FootballersIDs[defendersCount + defmidFieldNumber + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(1).GetChild(midFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int attackersNumber = int.Parse(formation[6].ToString());
            int defAndMidCount = defendersCount + defmidFieldNumber + midFieldNumber;
            if (attackersNumber == 1)
            {
                Database.clubDB[MyClubID].FootballersIDs[10] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = attackersNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClubID].FootballersIDs[defAndMidCount + inTheMiddle + i] = _SquadPanelsParent.transform.GetChild(0).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClubID].FootballersIDs[defAndMidCount + 1] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClubID].FootballersIDs[defAndMidCount + 2] = _SquadPanelsParent.transform.GetChild(0).GetChild(attackersNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
        }
        else if (formation.Length == 9)
        {
            int defmidFieldNumber = int.Parse(formation[2].ToString());
            if (defmidFieldNumber == 1)
            {
                // 4-1-2-1-2
                Database.clubDB[MyClubID].FootballersIDs[defendersCount + 1] = _SquadPanelsParent.transform.GetChild(3).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = defmidFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClubID].FootballersIDs[defendersCount + i] = _SquadPanelsParent.transform.GetChild(3).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClubID].FootballersIDs[defendersCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(3).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClubID].FootballersIDs[defendersCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(3).GetChild(defmidFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int midFieldNumber = int.Parse(formation[4].ToString());
            int defmidAndDefCount = defendersCount + defmidFieldNumber;
            if (midFieldNumber == 1)
            {
                // 4-2-1-2-1
                Database.clubDB[MyClubID].FootballersIDs[defmidAndDefCount + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = midFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClubID].FootballersIDs[defendersCount + i] = _SquadPanelsParent.transform.GetChild(2).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClubID].FootballersIDs[defmidAndDefCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClubID].FootballersIDs[defmidAndDefCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(2).GetChild(midFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int atkMidFieldNumber = int.Parse(formation[6].ToString());
            int midAndDefCount = defmidAndDefCount + midFieldNumber;
            if (atkMidFieldNumber == 1)
            {
                // 4-1-3-1-2
                Database.clubDB[MyClubID].FootballersIDs[midAndDefCount + 1] = _SquadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = atkMidFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClubID].FootballersIDs[midAndDefCount + i] = _SquadPanelsParent.transform.GetChild(1).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClubID].FootballersIDs[midAndDefCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClubID].FootballersIDs[midAndDefCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(1).GetChild(atkMidFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int attackersNumber = int.Parse(formation[8].ToString());
            int defAndMidCount = midAndDefCount + atkMidFieldNumber;
            if (attackersNumber == 1)
            {
                Database.clubDB[MyClubID].FootballersIDs[10] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = attackersNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClubID].FootballersIDs[defAndMidCount + inTheMiddle + i] = _SquadPanelsParent.transform.GetChild(0).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClubID].FootballersIDs[defAndMidCount + 1] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClubID].FootballersIDs[defAndMidCount + 2] = _SquadPanelsParent.transform.GetChild(0).GetChild(attackersNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
        }
        Database.clubDB[MyClubID].Formation = formation;
        WindowsManager.Instance.ShowWindow("Club Menu");
    }
}
