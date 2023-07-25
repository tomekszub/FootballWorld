using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using static CupRound;
using static League_Old;

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
	[SerializeField] Table _TableScript;
    [SerializeField] TextMeshProUGUI _NextMatchInfoText;

    string _leagueName;
    int _myLeagueRankingPos;
    DateTime _startOfTheSeason = new DateTime(2018, 8, 17);
    DateTime _currDate = new DateTime(2018, 6, 24);
    readonly DateTime[] _cupDates = { new DateTime(2018, 9, 4), new DateTime(2018, 11, 4), new DateTime(2019, 1, 1), new DateTime(2019, 3, 19), new DateTime(2019, 5, 15)};
    int _currentMatch = 0, _firstMatchOfTheWeek = 0;
    List<CupKnockoutStage> _nationalCup = new List<CupKnockoutStage>();
    List<CupRound> _championsCupRounds = new List<CupRound>();
    List<CupRound> _europaCupRounds = new List<CupRound>();
    List<CupRound> _europaTrophyRounds = new List<CupRound>();
    List<Team> _leagueTeams = new List<Team>();
    List<Club> _clubs;
    int _currCupRound = 0, _currChampionsCupRound = 0, _currEuropaCupRound = 0, _currEuropaTrophyRound = 0;
	int _teamsNumber;
    MatchStats _leagueScorers = new MatchStats(new List<Scorer>());
    bool _restAvailable = true;
    readonly Dictionary<EuropaTournamentType, List<(EuropaTournamentData data, DateTime firstLeg, DateTime secondLeg)>> _tournamentData = new()
    {
        {
            EuropaTournamentType.ChampionsCup,
            new List<(EuropaTournamentData, DateTime, DateTime)>()
            {
                (null, new DateTime(), new DateTime()),
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 7, 10), new DateTime(2018, 7, 17)),
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 7, 24), new DateTime(2018, 7, 31)),
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 8, 7), new DateTime(2018, 8, 14)),
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 8, 21), new DateTime(2018, 8, 28)),
                (new EuropaTournamentData( true, 0, 4, 2, 1f), new DateTime(2018, 9, 18), new DateTime()),  // group stage
			    (new EuropaTournamentData( true, 2, 4, 2, 1f), new DateTime(2019, 2, 19), new DateTime(2019, 3, 12)),	// 1/8
			    (new EuropaTournamentData( true, 1, 1, 2, 1f), new DateTime(2019, 4, 9), new DateTime(2019, 4, 16)),
                (new EuropaTournamentData( true, 1, 1, 2, 1f), new DateTime(2019, 4, 30), new DateTime(2019, 5, 7)),
                (new EuropaTournamentData( false, 1, 1, 2, 1f), new DateTime(2019, 5, 28), new DateTime())
            }
        },
        {
            EuropaTournamentType.EuropaCup,
            new List<(EuropaTournamentData, DateTime, DateTime)>()
            {
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 6, 27), new DateTime(2018, 7, 4)),
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 7, 11), new DateTime(2018, 7, 18)),
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 7, 25), new DateTime(2018, 8, 1)),
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 8, 8), new DateTime(2018, 8, 15)),
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 8, 22), new DateTime(2018, 8, 29)),
                (new EuropaTournamentData( true, 0, 1, 1, .5f),new DateTime(2018, 9, 19), new DateTime()),  // group stage
                (new EuropaTournamentData( true, 2, 1, 2, 1f), new DateTime(2019, 1, 30), new DateTime(2019, 2, 6)),	// play offs
			    (new EuropaTournamentData( true, 1, 3, 2, 1f), new DateTime(2019, 2, 20), new DateTime(2019, 3, 13)),	// 1/8
			    (new EuropaTournamentData( true, 1, 1, 2, 1f), new DateTime(2019, 4, 10), new DateTime(2019, 4, 17)),
                (new EuropaTournamentData( true, 1, 1, 2, 1f), new DateTime(2019, 5, 1), new DateTime(2019, 5, 8)),
                (new EuropaTournamentData( false, 1, 1, 2, 1f), new DateTime(2019, 5, 22), new DateTime())
            }
        },
        {
            EuropaTournamentType.EuropaTrophy,
            new List<(EuropaTournamentData, DateTime, DateTime)>()
            {
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 6, 28), new DateTime(2018, 7, 5)),
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 7, 12), new DateTime(2018, 7, 19)),
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 7, 26), new DateTime(2018, 8, 2)),
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 8, 9), new DateTime(2018, 8, 16)),
                (new EuropaTournamentData( true, 2, 0, 1, .5f),new DateTime(2018, 8, 23), new DateTime(2018, 8, 30)),
                (new EuropaTournamentData( true, 0, 1, 1, .5f),new DateTime(2018, 9, 20), new DateTime()),  // group stage
                (new EuropaTournamentData( true, 2, 0, 2, 1f), new DateTime(2019, 1, 31), new DateTime(2019, 2, 7)),	// play offs
			    (new EuropaTournamentData( true, 1, 1.5f, 2, 1f), new DateTime(2019, 2, 21), new DateTime(2019, 3, 14)),	// 1/8
			    (new EuropaTournamentData( true, 1, 0, 2, 1f), new DateTime(2019, 4, 11), new DateTime(2019, 4, 18)),
                (new EuropaTournamentData( true, 1, 1, 2, 1f), new DateTime(2019, 5, 2), new DateTime(2019, 5, 9)),
                (new EuropaTournamentData( false, 1, 1, 2, 1f), new DateTime(2019, 5, 23), new DateTime())
            }
        }
    };

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
        CreateEuropaTournamentsCalendar();
        MyTournaments.Add("Champions Cup");
        MyTournaments.Add("Europa Cup");
        MyTournaments.Add("Europa Trophy");

        UpdateCalendar();
        UpdateCurrentDateUI();
        UpdateNextMatchInfo();
    }

    public void ShowLeagueTable()
    {
        _TableScript.ShowTable(_leagueTeams, GetPositionRanges(_myLeagueRankingPos));
    }

    public void NextMatch()
    {
        while(_currDate != Matches[_currentMatch].Date)
        {
            if (_currentMatch >= Matches.Count)
                break;

            NextDay();
        }
    }

    public void NextDay()
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
            UpdateNextMatchInfo();
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
                UpdateNextMatchInfo("Koniec sezonu");
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
                MatchStats[] ms = Simulation.StartSimulation(hostId, guestId, Matches[_currentMatch].CompetitionName);
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
        Matches[_currentMatch].Result.HostGoals = matchStats[0].Goals;
        Matches[_currentMatch].Result.GuestGoals = matchStats[1].Goals;
        Matches[_currentMatch].Finished = true;

        if (competitionName == _leagueName)
        {
            #region Table
            if (matchStats[0].Goals > matchStats[1].Goals)
            {
                for (int i = 0; i < _leagueTeams.Count; i++)
                {
                    if (_leagueTeams[i].Id == hostId)
                    {
                        _leagueTeams[i].LostGoals += matchStats[1].Goals;
                        _leagueTeams[i].Points += 3;
                        _leagueTeams[i].ScoredGoals += matchStats[0].Goals;
                        _leagueTeams[i].Wins++;
                        _leagueTeams[i].MatchesPlayed++;
                        _leagueTeams[i].DifferenceGoals += matchStats[0].Goals - matchStats[1].Goals;
                    }
                    if (_leagueTeams[i].Id == guestId)
                    {
                        _leagueTeams[i].LostGoals += matchStats[0].Goals;
                        _leagueTeams[i].ScoredGoals += matchStats[1].Goals;
                        _leagueTeams[i].Loses++;
                        _leagueTeams[i].MatchesPlayed++;
                        _leagueTeams[i].DifferenceGoals -= matchStats[0].Goals - matchStats[1].Goals;
                    }
                }
            }
            else if (matchStats[0].Goals < matchStats[1].Goals)
            {
                for (int i = 0; i < _leagueTeams.Count; i++)
                {
                    if (_leagueTeams[i].Id == hostId)
                    {
                        _leagueTeams[i].LostGoals += matchStats[1].Goals;
                        _leagueTeams[i].ScoredGoals += matchStats[0].Goals;
                        _leagueTeams[i].Loses++;
                        _leagueTeams[i].MatchesPlayed++;
                        _leagueTeams[i].DifferenceGoals += matchStats[0].Goals - matchStats[1].Goals;
                    }
                    if (_leagueTeams[i].Id == guestId)
                    {
                        _leagueTeams[i].LostGoals += matchStats[0].Goals;
                        _leagueTeams[i].Points += 3;
                        _leagueTeams[i].ScoredGoals += matchStats[1].Goals;
                        _leagueTeams[i].Wins++;
                        _leagueTeams[i].MatchesPlayed++;
                        _leagueTeams[i].DifferenceGoals -= matchStats[0].Goals - matchStats[1].Goals;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _leagueTeams.Count; i++)
                {
                    if (_leagueTeams[i].Id == hostId)
                    {
                        _leagueTeams[i].LostGoals += matchStats[1].Goals;
                        _leagueTeams[i].Points += 1;
                        _leagueTeams[i].ScoredGoals += matchStats[0].Goals;
                        _leagueTeams[i].Draws++;
                        _leagueTeams[i].MatchesPlayed++;
                    }
                    if (_leagueTeams[i].Id == guestId)
                    {
                        _leagueTeams[i].LostGoals += matchStats[0].Goals;
                        _leagueTeams[i].Points += 1;
                        _leagueTeams[i].ScoredGoals += matchStats[1].Goals;
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
        else if (competitionName == "National Cup")
        {
            _nationalCup[Matches[_currentMatch].RoundIndex].SendMatchResult(Matches[_currentMatch]);
            var winners = _nationalCup[_currCupRound].GetWinners();
            if (winners != null)
            {
                if (winners.Count != 1)
                {
                    _nationalCup.Add(new CupKnockoutStage("National Cup", winners, new EuropaTournamentData(twoLeg: winners.Count != 2)));
                    _currCupRound++;
                    AddCupMatchesToCalendar(_nationalCup[_currCupRound].GetMatches(), _currCupRound, _cupDates[_currCupRound], _cupDates[_currCupRound].AddDays(14), winners.Count != 2);
                }
            }
        }
        else if (competitionName == "Champions Cup")
        {
            _championsCupRounds[Matches[_currentMatch].RoundIndex].SendMatchResult(Matches[_currentMatch]);
            var winners = _championsCupRounds[_currChampionsCupRound].GetWinners();
            if (winners != null)
            {
                if (_currChampionsCupRound < 5)  // qual and group stage
                {
                    _currChampionsCupRound++;
                    List<Club> cs = new List<Club>();
                    for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
                    {
                        List<Club> c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetClubsForEuropaTournament(i, EuropaTournamentType.ChampionsCup, _currChampionsCupRound);
                        if (c != null)
                            cs.AddRange(c);
                    }
                    cs.AddRange(winners);
                    if (_currChampionsCupRound != 5)
                    {
                        _championsCupRounds.Add(new CupKnockoutStage("Champions Cup", cs, _tournamentData[EuropaTournamentType.ChampionsCup][_currChampionsCupRound].data));
                        Match[] ms = _championsCupRounds[_currChampionsCupRound].GetMatches();
                        AddCupMatchesToCalendar(
                                ms,
                                _currChampionsCupRound,
                                _tournamentData[EuropaTournamentType.ChampionsCup][_currChampionsCupRound].firstLeg,
                                _tournamentData[EuropaTournamentType.ChampionsCup][_currChampionsCupRound].secondLeg
                            );
                    }
                    else
                        _championsCupRounds.Add(new CupGroupStage("Champions Cup", 5, _tournamentData[EuropaTournamentType.ChampionsCup][_currChampionsCupRound].firstLeg, 14, cs, _tournamentData[EuropaTournamentType.ChampionsCup][_currChampionsCupRound].data));

                }
                else  // knockout stage
                {
                    if (winners.Count == 1)
                    {
                        Debug.LogError("Champion has been found, it is !" + winners[0].Name + " from " + winners[0].CountryName);
                    }
                    else
                    {
                        _currChampionsCupRound++;
                        _championsCupRounds.Add(new CupKnockoutStage("Champions Cup", winners, _tournamentData[EuropaTournamentType.ChampionsCup][_currChampionsCupRound].data, null, _currChampionsCupRound == 6));
                        Match[] ms = _championsCupRounds[_currChampionsCupRound].GetMatches();
                        AddCupMatchesToCalendar(
                                ms,
                                _currChampionsCupRound,
                                _tournamentData[EuropaTournamentType.ChampionsCup][_currChampionsCupRound].firstLeg,
                                _tournamentData[EuropaTournamentType.ChampionsCup][_currChampionsCupRound].secondLeg,
                                winners.Count != 2
                            );
                    }
                }
            }
        }
        else if (competitionName == "Europa Cup")
        {
            _europaCupRounds[Matches[_currentMatch].RoundIndex].SendMatchResult(Matches[_currentMatch]);
            var winners = _europaCupRounds[_currEuropaCupRound].GetWinners();
            if (winners != null)
            {
                if (_currEuropaCupRound < 5)  // qual and group stage
                {
                    _currEuropaCupRound++;
                    List<Club> cs = new List<Club>();
                    for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
                    {
                        List<Club> c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetClubsForEuropaTournament(i, EuropaTournamentType.EuropaCup, _currEuropaCupRound);
                        if (c != null)
                            cs.AddRange(c);
                    }
                    cs.AddRange(winners);
                    if (_currEuropaCupRound != 5)
                    {
                        _europaCupRounds.Add(new CupKnockoutStage("Europa Cup", cs,
                            _tournamentData[EuropaTournamentType.EuropaCup][_currEuropaCupRound].data,
                            _currEuropaCupRound >= 2 ? _championsCupRounds[_currEuropaCupRound - 1].GetLoosers() : null));
                        Match[] ms = _europaCupRounds[_currEuropaCupRound].GetMatches();
                        AddCupMatchesToCalendar(
                                ms,
                                _currEuropaCupRound,
                                _tournamentData[EuropaTournamentType.EuropaCup][_currEuropaCupRound].firstLeg,
                                _tournamentData[EuropaTournamentType.EuropaCup][_currEuropaCupRound].secondLeg
                            );
                    }
                    else
                    {
                        cs.AddRange(_championsCupRounds[4].GetLoosers());
                        _europaCupRounds.Add(new CupGroupStage("Europa Cup", 5, _tournamentData[EuropaTournamentType.EuropaCup][_currEuropaCupRound].firstLeg, 14, cs, _tournamentData[EuropaTournamentType.EuropaCup][_currEuropaCupRound].data));
                    }

                }
                else  // knockout stage
                {
                    
                    if (winners.Count == 1)
                    {
                        Debug.LogError("Europa Cup champion has been found, it is !" + winners[0].Name + " from " + winners[0].CountryName);
                    }
                    else
                    {
                        _currEuropaCupRound++;
                        List<Club> clubsInCurrentRound = winners;
                        List<Club> clubsFromHigherCup = null;
                        if (_currEuropaCupRound == 6)
                        {
                            // remove first 8, because they are winners, so they are automatically in 1/8
                            clubsInCurrentRound.RemoveRange(0, 8);
                            clubsFromHigherCup = _championsCupRounds[5].GetLoosers();
                        }
                        else if(_currEuropaCupRound == 7)
                            clubsInCurrentRound.AddRange(_europaCupRounds[5].GetWinners().Take(8));

                        _europaCupRounds.Add(new CupKnockoutStage("Europa Cup", clubsInCurrentRound, _tournamentData[EuropaTournamentType.EuropaCup][_currEuropaCupRound].data, clubsFromHigherCup, _currEuropaCupRound == 6));
                        Match[] ms = _europaCupRounds[_currEuropaCupRound].GetMatches();
                        AddCupMatchesToCalendar(
                                ms,
                                _currEuropaCupRound,
                                _tournamentData[EuropaTournamentType.EuropaCup][_currEuropaCupRound].firstLeg,
                                _tournamentData[EuropaTournamentType.EuropaCup][_currEuropaCupRound].secondLeg,
                                clubsInCurrentRound.Count != 2
                            );
                    }
                }
            }
        }
        else if (competitionName == "Europa Trophy")
        {
            _europaTrophyRounds[Matches[_currentMatch].RoundIndex].SendMatchResult(Matches[_currentMatch]);
            var winners = _europaTrophyRounds[_currEuropaTrophyRound].GetWinners();
            if (winners != null)
            {
                if (_currEuropaTrophyRound < 5)  // qual and group stage
                {
                    _currEuropaTrophyRound++;
                    List<Club> cs = new List<Club>();
                    for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
                    {
                        List<Club> c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetClubsForEuropaTournament(i, EuropaTournamentType.EuropaTrophy, _currEuropaTrophyRound);
                        if (c != null)
                            cs.AddRange(c);
                    }
                    cs.AddRange(winners);
                    if (_currEuropaTrophyRound != 5)
                    {
                        _europaTrophyRounds.Add(new CupKnockoutStage("Europa Trophy", cs,
                            _tournamentData[EuropaTournamentType.EuropaTrophy][_currEuropaTrophyRound].data,
                            _currEuropaTrophyRound >= 1 ? _europaCupRounds[_currEuropaTrophyRound - 1].GetLoosers() : null));
                        Match[] ms = _europaTrophyRounds[_currEuropaTrophyRound].GetMatches();
                        AddCupMatchesToCalendar(
                                ms,
                                _currEuropaTrophyRound,
                                _tournamentData[EuropaTournamentType.EuropaTrophy][_currEuropaTrophyRound].firstLeg,
                                _tournamentData[EuropaTournamentType.EuropaTrophy][_currEuropaTrophyRound].secondLeg
                            );
                    }
                    else
                    {
                        cs.AddRange(_europaCupRounds[4].GetLoosers());
                        _europaTrophyRounds.Add(new CupGroupStage("Europa Trophy", 5, _tournamentData[EuropaTournamentType.EuropaTrophy][_currEuropaTrophyRound].firstLeg, 14, cs, _tournamentData[EuropaTournamentType.EuropaTrophy][_currEuropaTrophyRound].data));
                    }

                }
                else  // knockout stage
                {

                    if (winners.Count == 1)
                    {
                        Debug.LogError("Europa Trophy champion has been found, it is !" + winners[0].Name + " from " + winners[0].CountryName);
                    }
                    else
                    {
                        _currEuropaTrophyRound++;
                        List<Club> clubsInCurrentRound = winners;
                        List<Club> clubsFromHigherCup = _currEuropaTrophyRound == 6 ? _europaCupRounds[5].GetLoosers() : null;
                        _europaTrophyRounds.Add(new CupKnockoutStage("Europa Trophy", clubsInCurrentRound, _tournamentData[EuropaTournamentType.EuropaTrophy][_currEuropaTrophyRound].data, clubsFromHigherCup, _currEuropaTrophyRound == 6));
                        Match[] ms = _europaTrophyRounds[_currEuropaTrophyRound].GetMatches();
                        AddCupMatchesToCalendar(
                                ms,
                                _currEuropaTrophyRound,
                                _tournamentData[EuropaTournamentType.EuropaTrophy][_currEuropaTrophyRound].firstLeg,
                                _tournamentData[EuropaTournamentType.EuropaTrophy][_currEuropaTrophyRound].secondLeg,
                                clubsInCurrentRound.Count != 2
                            );
                    }
                }
            }
        }
        _currentMatch++;
        UpdateCalendar();
    }

    void CreateEuropaTournamentsCalendar()
    {
        // champions cup does not have a pre-eliminary round so we start from 1
        _currChampionsCupRound = 1;
        List<Club> cs = new List<Club>();
        for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
        {
            List<Club> c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetClubsForEuropaTournament(i, EuropaTournamentType.ChampionsCup, _currChampionsCupRound);
            if(c != null)
                cs.AddRange(c);
        }
        _championsCupRounds.Add(null);
        _championsCupRounds.Add(new CupKnockoutStage("Champions Cup",cs, _tournamentData[EuropaTournamentType.ChampionsCup][_currChampionsCupRound].data));
        Match[] ms = _championsCupRounds[_currChampionsCupRound].GetMatches();
        AddCupMatchesToCalendar(
                            ms,
                            1,
                            _tournamentData[EuropaTournamentType.ChampionsCup][_currChampionsCupRound].firstLeg,
                            _tournamentData[EuropaTournamentType.ChampionsCup][_currChampionsCupRound].secondLeg
                        );

        // EUROPA CUP
        _currEuropaCupRound = 0;
        cs = new List<Club>();
        for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
        {
            List<Club> c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetClubsForEuropaTournament(i, EuropaTournamentType.EuropaCup, _currEuropaCupRound);
            if (c != null)
                cs.AddRange(c);
        }
        _europaCupRounds.Add(new CupKnockoutStage("Europa Cup", cs, _tournamentData[EuropaTournamentType.EuropaCup][_currEuropaCupRound].data));
        ms = _europaCupRounds[_currEuropaCupRound].GetMatches();
        AddCupMatchesToCalendar(
                            ms,
                            0,
                            _tournamentData[EuropaTournamentType.EuropaCup][_currEuropaCupRound].firstLeg,
                            _tournamentData[EuropaTournamentType.EuropaCup][_currEuropaCupRound].secondLeg
                        );
        // EUROP TROPHY
        _currEuropaTrophyRound = 0;
        cs = new List<Club>();
        for (int i = 0; i < CurrFederationRankingLeagueIndex.Length; i++)
        {
            List<Club> c = Database.leagueDB[CurrFederationRankingLeagueIndex[i]].GetClubsForEuropaTournament(i, EuropaTournamentType.EuropaTrophy, _currEuropaTrophyRound);
            if (c != null)
                cs.AddRange(c);
        }
        _europaTrophyRounds.Add(new CupKnockoutStage("Europa Trophy", cs, _tournamentData[EuropaTournamentType.EuropaTrophy][_currEuropaTrophyRound].data));
        ms = _europaTrophyRounds[_currEuropaTrophyRound].GetMatches();
        AddCupMatchesToCalendar(
                            ms,
                            0,
                            _tournamentData[EuropaTournamentType.EuropaTrophy][_currEuropaTrophyRound].firstLeg,
                            _tournamentData[EuropaTournamentType.EuropaTrophy][_currEuropaTrophyRound].secondLeg
                        );
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
        _nationalCup.Add(new CupKnockoutStage("National Cup", cs, new EuropaTournamentData()));
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

    void UpdateNextMatchInfo(string alternativeText = "")
    {
        _NextMatchInfoText.text = alternativeText != "" ? alternativeText : Matches[_currentMatch].Date.Day + "/" + Matches[_currentMatch].Date.Month + " " + Matches[_currentMatch].CompetitionName;
    }

    void AddCupMatchesToCalendar(Match[] ms, int roundInd, DateTime firstLeg, DateTime secondLeg, bool twoLeg = true)
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
