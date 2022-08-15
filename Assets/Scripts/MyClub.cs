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
	public int[] CurrFederationRankingLeagueIndex { get; private set; } = new int[54];
    public Dictionary<string, int> CurrFederationRankingCountryName { get; private set; } = new Dictionary<string, int>();

    [SerializeField] TextMeshProUGUI _DateText;
    [SerializeField] CalendarEntry _CalendarMatchEntryPrefab;
    [SerializeField] GameObject _CalendarEntriesParent;
    [SerializeField] TextMeshProUGUI _ScorersText;
    [SerializeField] TextMeshProUGUI _OutputClubName;
    [SerializeField] GameObject _MainMenuPanel;
    [SerializeField] GameObject _ClubMenuPanel;
	[SerializeField] Table _TableScript;

    string _leagueName;
    int _myLeagueRankingPos;
    DateTime _startOfTheSeason = new DateTime(2018, 8, 17);
    DateTime _currDate = new DateTime(2018, 6, 24);
    readonly DateTime[] _cupDates = { new DateTime(2018, 9, 4), new DateTime(2018, 10, 16), new DateTime(2018, 10, 30), new DateTime(2019, 1, 8), new DateTime(2019, 1, 22), new DateTime(2019, 2, 5), new DateTime(2019, 5, 21)};
    int _currentMatch = 0, _firstMatchOfTheWeek = 0;
    List<CupKnockoutStage> _nationalCup = new List<CupKnockoutStage>();
    List<CupRound> _qualCL = new List<CupRound>();
    List<Team> _leagueTeams = new List<Team>();
    List<Club> _clubs;
	int _teamsNumber;
    int _currCupRound = 0, _currCLRound = 0;
    MatchStats _leagueScorers = new MatchStats(new List<Scorer>());
    bool _restAvailable = true;

    void Awake()
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

	public void GenerateGameData(int leagueID, int inLeagueIndex)
    {
        MyLeagueID = leagueID;
        _teamsNumber = Database.leagueDB[MyLeagueID].Teams.Count;
        _leagueName = Database.leagueDB[MyLeagueID].Name;
        MyInLeagueIndex = inLeagueIndex;
        MyClubID = Database.leagueDB[MyLeagueID].Teams[MyInLeagueIndex].Id;
        _OutputClubName.text = Database.clubDB[MyClubID].Name;

        for (int i = 0; i < _teamsNumber; i++)
        {
            _leagueTeams.Add(new Team(Database.leagueDB[MyLeagueID].Teams[i].Id, Database.leagueDB[MyLeagueID].Teams[i].Name, 0, 0, 0, 0, 0, 0));
        }
        ShowLeagueTable();

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

        // at the start of the career, ranking is inferred from order in leagueDB which starts form 0)
        CurrFederationRankingLeagueIndex = Enumerable.Range(0, 54).ToArray();
        for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
        {
            CurrFederationRankingCountryName.Add(Database.leagueDB[CurrFederationRankingLeagueIndex[i]].Country, i);
        }
        _myLeagueRankingPos = MyLeagueID;
        CreateChampionsLeagueCalendar();
        MyTournaments.Add("Champions Cup");

        UpdateCalendar();
        UpdateCurrentDateUI();
    }

    public void ShowLeagueTable()
    {
        _TableScript.ShowTable(_leagueTeams, League_Old.GetPositionRanges(_myLeagueRankingPos));
    }

    public void NextMatch()
	{
        _CalendarEntriesParent.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1;
        if (_currentMatch >= Matches.Count)
        {
            Debug.Log("Koniec meczów!");
            return;
        }

        if (_restAvailable)
        {
            // resting - fatigue loss - probably should be calculating at the next match, not at every day
            Database.footballersDB.ForEach(f => f.LoseFatigue());
            _restAvailable = false;
        }

        if (Matches[_currentMatch].Date != _currDate)
        {
            _restAvailable = true;
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

        WindowsManager.Instance.ShowWindow("Simulation");
        Comment.Instance.Init(hostId, guestId, Matches[_currentMatch].CompetitionName);
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
                for (int i = 0; i < _leagueTeams.Count; i++)
                {
                    if (_leagueTeams[i].Id == hostId)
                    {
                        _leagueTeams[i].LostGoals += matchStats[1].GetGoals();
                        _leagueTeams[i].Points += 3;
                        _leagueTeams[i].ScoredGoals += matchStats[0].GetGoals();
                        _leagueTeams[i].Wins++;
                        _leagueTeams[i].MatchesPlayed++;
                        _leagueTeams[i].DifferenceGoals += matchStats[0].GetGoals() - matchStats[1].GetGoals();
                    }
                    if (_leagueTeams[i].Id == guestId)
                    {
                        _leagueTeams[i].LostGoals += matchStats[0].GetGoals();
                        _leagueTeams[i].ScoredGoals += matchStats[1].GetGoals();
                        _leagueTeams[i].Loses++;
                        _leagueTeams[i].MatchesPlayed++;
                        _leagueTeams[i].DifferenceGoals -= matchStats[0].GetGoals() - matchStats[1].GetGoals();
                    }
                }
            }
            else if (matchStats[0].GetGoals() < matchStats[1].GetGoals())
            {
                for (int i = 0; i < _leagueTeams.Count; i++)
                {
                    if (_leagueTeams[i].Id == hostId)
                    {
                        _leagueTeams[i].LostGoals += matchStats[1].GetGoals();
                        _leagueTeams[i].ScoredGoals += matchStats[0].GetGoals();
                        _leagueTeams[i].Loses++;
                        _leagueTeams[i].MatchesPlayed++;
                        _leagueTeams[i].DifferenceGoals += matchStats[0].GetGoals() - matchStats[1].GetGoals();
                    }
                    if (_leagueTeams[i].Id == guestId)
                    {
                        _leagueTeams[i].LostGoals += matchStats[0].GetGoals();
                        _leagueTeams[i].Points += 3;
                        _leagueTeams[i].ScoredGoals += matchStats[1].GetGoals();
                        _leagueTeams[i].Wins++;
                        _leagueTeams[i].MatchesPlayed++;
                        _leagueTeams[i].DifferenceGoals -= matchStats[0].GetGoals() - matchStats[1].GetGoals();
                    }
                }
            }
            else
            {
                for (int i = 0; i < _leagueTeams.Count; i++)
                {
                    if (_leagueTeams[i].Id == hostId)
                    {
                        _leagueTeams[i].LostGoals += matchStats[1].GetGoals();
                        _leagueTeams[i].Points += 1;
                        _leagueTeams[i].ScoredGoals += matchStats[0].GetGoals();
                        _leagueTeams[i].Draws++;
                        _leagueTeams[i].MatchesPlayed++;
                    }
                    if (_leagueTeams[i].Id == guestId)
                    {
                        _leagueTeams[i].LostGoals += matchStats[0].GetGoals();
                        _leagueTeams[i].Points += 1;
                        _leagueTeams[i].ScoredGoals += matchStats[1].GetGoals();
                        _leagueTeams[i].Draws++;
                        _leagueTeams[i].MatchesPlayed++;
                    }
                }
            }
            #endregion

            for (int i = 0; i < matchStats[0].scorers.Count; i++)
            {
                _leagueScorers.AddScorer(matchStats[0].scorers[i]);
            }
            for (int i = 0; i < matchStats[1].scorers.Count; i++)
            {
                _leagueScorers.AddScorer(matchStats[1].scorers[i]);
            }

            _leagueScorers.scorers = _leagueScorers.scorers.OrderByDescending(n => n.goals).ToList();
            _ScorersText.text = "";
            int maxScorers = (int)MathF.Min(8, _leagueScorers.scorers.Count);
            for (int i = 0; i < maxScorers; i++)
            {
                _ScorersText.text += _leagueScorers.scorers[i].goals + "\t" + _leagueScorers.scorers[i].name + " " + _leagueScorers.scorers[i].surname + "\n";
            }
        }
        else if(competitionName == "National Cup")
        {
            _nationalCup[Matches[_currentMatch].RoundIndex].SendMatchResult(Matches[_currentMatch]);
            var winners = _nationalCup[_currCupRound].GetWinners();
            if (winners != null)
            {
                if(winners.Count != 1)
                {
                    _nationalCup.Add(new CupKnockoutStage("National Cup", null, winners));
                    _currCupRound++;
                    AddCupMatchesToCalendar(_nationalCup[_currCupRound].GetMatches(), _currCupRound, _cupDates[_currCupRound], _cupDates[_currCupRound].AddDays(7));
                } 
            }
        }
        else if(competitionName == "Champions Cup")
        {
            _qualCL[Matches[_currentMatch].RoundIndex].SendMatchResult(Matches[_currentMatch]);
            var winners = _qualCL[_currCLRound].GetWinners();
            if (winners != null)
            {
                if (_currCLRound == 0)
                {
                    List<Club> cs = new List<Club>();
                    for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
                    {
                        Club c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetFirstQ_CL_Clubs(i);
                        if (c != null) cs.Add(c);
                    }
                    if (cs.Count != 33)
                    {
                        Debug.LogError("It should be 33 added teams in this round!");
                        return;
                    }
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", cs, winners, true, 2, false, 0, 1, .5f));
                    _currCLRound++;
                    Match[] ms = _qualCL[1].GetMatches();
                    AddCupMatchesToCalendar(ms,1, new DateTime(2018,7,10), new DateTime(2018, 7, 17));
                }
                else if (_currCLRound == 1)
                {
                    List<Club> cs = new List<Club>();
                    for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
                    {
                        Club c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetSecondChampionsPathQ_CL_Clubs(i);
                        if (c != null) cs.Add(c);
                    }
                    if (cs.Count != 3)
                    {
                        Debug.LogError("It should be 3 added teams in this round!");
                        return;
                    }
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", cs, winners, true, 2, false, 0, 1, .5f));
                    _currCLRound++;
                    // league path
                    cs = new List<Club>();
                    for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
                    {
                        Club c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetSecondLeaguePathQ_CL_Clubs(i);
                        if (c != null) cs.Add(c);
                    }
                    if (cs.Count != 6)
                    {
                        Debug.LogError("It should be 6 added teams in this round!");
                        return;
                    }
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", cs, null, true, 2, false, 0, 1, .5f));
                    _currCLRound++;
                    Match[] ms = _qualCL[2].GetMatches();
                    Match[] ms2 = _qualCL[3].GetMatches();
                    AddCupMatchesToCalendar(ms,2, new DateTime(2018, 7, 24), new DateTime(2018, 7, 31));
                    AddCupMatchesToCalendar(ms2,3, new DateTime(2018, 7, 24), new DateTime(2018, 7, 31));
                }
                else if (_currCLRound == 3)
                {
                    List<Club> cs = new List<Club>();
                    for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
                    {
                        Club c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetThirdChampionsPathQ_CL_Clubs(i);
                        if (c != null) cs.Add(c);
                    }
                    if (cs.Count != 2)
                    {
                        Debug.LogError("It should be 2 added teams in this round!");
                        return;
                    }
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", cs, _qualCL[2].GetWinners(), true, 2, false, 0, 1, .5f));
                    _currCLRound++;
                    cs = new List<Club>();
                    for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
                    {
                        Club c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetThirdLeaguePathQ_CL_Clubs(i);
                        if (c != null) cs.Add(c);
                    }
                    if (cs.Count != 5)
                    {
                        Debug.LogError("It should be 5 added teams in this round!");
                        return;
                    }
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", cs, winners, true, 2, false, 0, 1, .5f));
                    _currCLRound++;
                    Match[] ms = _qualCL[4].GetMatches();
                    Match[] ms2 = _qualCL[5].GetMatches();
                    AddCupMatchesToCalendar(ms,4, new DateTime(2018, 8, 7), new DateTime(2018, 8, 14));
                    AddCupMatchesToCalendar(ms2,5, new DateTime(2018, 8, 7), new DateTime(2018, 8, 14));
                }
                else if (_currCLRound == 5)
                {
                    List<Club> cs = new List<Club>();
                    for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
                    {
                        Club c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetPlayOffsChampionsPathQ_CL_Clubs(i);
                        if (c != null) cs.Add(c);
                    }
                    if (cs.Count != 2)
                    {
                        Debug.LogError("It should be 2 added teams in this round!");
                        return;
                    }
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", cs, _qualCL[4].GetWinners(), true, 2, false, 1, 1, .5f));
                    _currCLRound++;
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", null, winners, true, 2, false, 1, 1, .5f));
                    _currCLRound++;
                    Match[] ms = _qualCL[6].GetMatches();
                    Match[] ms2 = _qualCL[7].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddCupMatchesToCalendar(ms,6, new DateTime(2018, 8, 21), new DateTime(2018, 8, 28));
                    AddCupMatchesToCalendar(ms2,7, new DateTime(2018, 8, 21), new DateTime(2018, 8, 28));
                }
                else if(_currCLRound == 7)
                {
                    List<Club> cs = new List<Club>();
                    for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
                    {
                        List<Club> c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetGroupStage_CL_Clubs(i);
                        if (c != null) cs.AddRange(c);
                    }
                    if (cs.Count != 26)
                    {
                        Debug.LogError("It should be 26 added teams in this round!");
                        return;
                    }
                    List<Club> pw = new List<Club>();
                    pw.AddRange(_qualCL[6].GetWinners());
                    pw.AddRange(winners);
                    _qualCL.Add(new CupGroupStage("Champions Cup",8, new DateTime(2018, 9, 17),14,cs, pw, 4, 4, 2, 1));
                    _currCLRound++;
                }
                else if(_currCLRound == 8)                       // 1/8 finalu
                {
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", null, winners, true, 2, true, 4, 2, 1));
                    _currCLRound++;
                    Match[] ms = _qualCL[9].GetMatches();
                    AddCupMatchesToCalendar(ms, 9, new DateTime(2019, 2, 19), new DateTime(2019, 3, 12));
                }
                else if (_currCLRound == 9)                      // 1/4 finalu
                {
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", null, winners, true, 1, false, 1, 2, 1));
                    _currCLRound++;
                    Match[] ms = _qualCL[10].GetMatches();
                    AddCupMatchesToCalendar(ms, 10, new DateTime(2019, 4, 9), new DateTime(2019, 4, 16));
                }
                else if (_currCLRound == 10)                      // 1/2 finalu
                {
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", null, winners, true, 1, false, 1, 2, 1));
                    _currCLRound++;
                    Match[] ms = _qualCL[11].GetMatches();
                    AddCupMatchesToCalendar(ms, 11, new DateTime(2019, 4, 30), new DateTime(2019, 5, 7));
                }
                else if (_currCLRound == 11)                      // grand finale
                {
                    _qualCL.Add(new CupKnockoutStage("Champions Cup", null, winners, false, 1, false, 1, 2, 1));
                    _currCLRound++;
                    Match[] ms = _qualCL[12].GetMatches();
                    AddCupMatchesToCalendar(ms, 12, new DateTime(2019, 5, 28), new DateTime(), false);
                }
            }
        }
        _currentMatch++;
        UpdateCalendar();
    }

    void CreateChampionsLeagueCalendar()
    {
        List<Club> cs = new List<Club>();
        for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
        {
            Club c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetPreeliminary_CL_Clubs(i);
            if(c != null)
                cs.Add(c);
        }
        if (cs.Count != 2)
        {
            Debug.LogError("It should be 2 teams in this round!");
            return;
        }
        _qualCL.Add(new CupKnockoutStage("Champions Cup",cs, null, false, 0, false, 0, 1, 0.5f));
        Match[] ms = _qualCL[0].GetMatches();
        foreach (var m in ms)
        {
            m.Date = new DateTime(2018, 6, 25);
            m.RoundIndex = 0;
        }
        Matches.InsertRange(0, ms);
    }

    void CreateCupCalendar()
    {
        List<Club> cs = new List<Club>();
        int teams = 1;
        CalculateBiggestTeamsNumber();
        for (int i = 0; i < teams; i++)
        {
            cs.Add(Database.leagueDB[MyLeagueID].Teams[i]);
        }
        _nationalCup.Add(new CupKnockoutStage("National Cup", cs));
        AddCupMatchesToCalendar(_nationalCup[0].GetMatches(), 0, _cupDates[0], _cupDates[0].AddDays(7));

        void CalculateBiggestTeamsNumber()
        {
            for (int i = 1; i < _teamsNumber; i++)
            {
                if (teams * 2 <= _teamsNumber)
                    teams *= 2;
                else
                    break;
            }
        }
    }

    void UpdateCurrentDateUI() => _DateText.text = _currDate.Day + "/" + _currDate.Month;

    void UpdateCalendar()
    {
        // if current match is not in the same day that the first match of the day was we just set firstmatchoftheday to current match index
        // what it means is that we keep track of first match of the current day to list all of them in the list (ui vertical layout)
        if (_currentMatch != 0 && Matches[_currentMatch-1].Date != _currDate && Matches[_firstMatchOfTheWeek].Date != Matches[_currentMatch].Date) 
            _firstMatchOfTheWeek = _currentMatch;

        int todayMatches = 0, currMatchOfTheDay = 0;
        DateTime nextMatchDay = Matches[_firstMatchOfTheWeek].Date;
        for (int i = _firstMatchOfTheWeek; i < Matches.Count; i++)
        {
            if (Matches[i].Date == nextMatchDay) 
                todayMatches++;
            else 
                break;
        }

        string resultHelperString;

        foreach (Transform child in _CalendarEntriesParent.transform)
        {
            if (currMatchOfTheDay == todayMatches) 
                Destroy(child.gameObject);
            else
            {
                if (Matches[_firstMatchOfTheWeek + currMatchOfTheDay].Finished) 
                    resultHelperString = Matches[_firstMatchOfTheWeek + currMatchOfTheDay].Result.HostGoals + " - " + Matches[_firstMatchOfTheWeek + currMatchOfTheDay].Result.GuestGoals;
                else 
                    resultHelperString = "-";
                child.GetComponent<CalendarEntry>().SetData(Matches[_firstMatchOfTheWeek + currMatchOfTheDay].FirstTeamId, Matches[_firstMatchOfTheWeek + currMatchOfTheDay].SecondTeamId, resultHelperString);
                currMatchOfTheDay++;
            }
        }
        for (int i = currMatchOfTheDay; i < todayMatches; i++)
        {
            CalendarEntry entry = Instantiate(_CalendarMatchEntryPrefab, _CalendarEntriesParent.transform);
            if (Matches[_firstMatchOfTheWeek + currMatchOfTheDay].Finished) 
                resultHelperString = Matches[_firstMatchOfTheWeek + currMatchOfTheDay].Result.HostGoals + " - " + Matches[_firstMatchOfTheWeek + currMatchOfTheDay].Result.GuestGoals;
            else 
                resultHelperString = "-";
            entry.SetData(Matches[_firstMatchOfTheWeek + i].FirstTeamId, Matches[_firstMatchOfTheWeek + i].SecondTeamId, resultHelperString);
        }
    }

    void AddCupMatchesToCalendar(Match[] ms,int roundInd, DateTime firstLeg, DateTime secondLeg,bool twoLeg = true)
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
            if (firstLegIndex == -1)
                firstLegIndex = Matches.Count;
            if (secondLegIndex == -1)
                secondLegIndex = Matches.Count + ms.Length / 2;
            
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
}
