using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class Comment : MonoBehaviour
{
    public static Comment Instance;

    [SerializeField] TextMeshProUGUI _StartStopButton;
    [SerializeField] List<CommentPanelFootballerRow> _HostSquadStatsUI;
    [SerializeField] List<CommentPanelFootballerRow> _GuestSquadStatsUI;

    float _time = 0.1f;
    bool _isPlaying = false;
    int _minute = 0;
    int _hostId, _guestId;
    List<Footballer>[] _teams = new List<Footballer>[2];
    List<PlayersMatchData>[] _teamsMatchData = new List<PlayersMatchData>[2];
    float[] _teamDef = new float[2], _teamMid = new float[2], _teamAtk = new float[2];
    int _hostChances, _goalChances;
    MatchStats[] _matchStats = new MatchStats[2];
    string[] _teamName = new string[2];
    int _playerWithBall;  // piłkarz przy piłce
    int _prevPlayerWithBall;
    int[] _defLastPlayerNumber = new int[2];// granice pozycji piłkarzy: obrona , pomoc , atak
    int[] _midLastPlayerNumber = new int[2];
    List<int>[] _teamsMidPos = new List<int>[2];// Lista piłkarzy(pomocnicy of., napastnicy) bedacych w środku formacji
    List<int>[] _teamsDefPos = new List<int>[2];// Lista piłkarzy(obrońcy ) bedacych w środku formacji
    int[,] _wingPos = new int[2, 2]; //static int HostLeftPos, GuestLeftPos, HostRightPos, GuestRightPos;
    int[,] _defWingPos = new int[2, 2]; //int HostDefLeftPos, GuestDefLeftPos, HostDefRightPos, GuestDefRightPos;
    int _not = 0;
    float _curiosity = 1;
    int _weakness = 0;
    int _changedCuriosity = 0;
    bool _end = false;
    string _competitionName = "";

    #region Getters
    public string GetGuestName() => _teamName[1];
    public string GetHostName() => _teamName[0];
    public int GetGuestID() => _guestId;
    public int GetHostID() => _hostId;
    public int GetMinute() => _minute;
    public List<Footballer>[] GetTeams() => _teams;
    public List<int>[] GetMidPos() => _teamsMidPos;
    public List<int>[] GetDefPos() => _teamsDefPos;
    public MatchStats[] GetMatchStats() => _matchStats;
    public int GetHostChances() => _hostChances;
    public int GetGoalChances() => _goalChances;
    public int[] GetDefLastPlayerNumber() => _defLastPlayerNumber;
    public int[] GetMidLastPlayerNumber() => _defLastPlayerNumber;
    public int[,] GetWingPos() => _wingPos;
    public int[,] GetDefWingPos() => _defWingPos;

    public int GuestBall 
    {
        private set;
        get;
    }

    public int PlayerWithBall
    {
        get => _playerWithBall;
        private set
        {
            _prevPlayerWithBall = _playerWithBall;
            _playerWithBall = value;
        }
    }
    #endregion

    public int ReverseGuestBall => (GuestBall == 0) ? 1 : 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    // Use this for initialization, 
    public void Init(int hostID, int guestID, string compName)
    {
        _competitionName = compName;
        _hostId = hostID;
        _guestId = guestID;
        _time = 0.1f;
        PrepareNextMatch();
    }

    public void OnSimSpeedChanged(float val) => _time = val;

    public void SimulateMatch()
    {
        if(!_end)
        {
            _time = 0;
            if(!_isPlaying)
                StartCoroutine(CommentStart());
        }
    }

    void PrepareNextMatch()
    {
        _end = false;
        _matchStats[0] = new MatchStats(new List<Scorer>());
        _matchStats[1] = new MatchStats(new List<Scorer>());
        _teamName[0] = Database.clubDB[_hostId].Name;
        _teamName[1] = Database.clubDB[_guestId].Name;
       
        _teams[0] = new List<Footballer>(11);
        _teams[1] = new List<Footballer>(11);
        _teamsMatchData[0] = new(11);
        _teamsMatchData[1] = new(11);
        for (int i = 0; i < 11; i++)
        {
            _teams[0].Add(Database.footballersDB[Database.clubDB[_hostId].FootballersIDs[i]]);
            _teams[0][i].AddStatistic(_competitionName, Footballer.PlayerStatistics.StatName.MatchesPlayed);
            _teamsMatchData[0].Add(new PlayersMatchData());
            _HostSquadStatsUI[i].Init(_teams[0][i], _teamsMatchData[0][i]);
            _teams[1].Add(Database.footballersDB[Database.clubDB[_guestId].FootballersIDs[i]]);
            _teams[1][i].AddStatistic(_competitionName, Footballer.PlayerStatistics.StatName.MatchesPlayed);
            _teamsMatchData[1].Add(new PlayersMatchData());
            _GuestSquadStatsUI[i].Init(_teams[1][i], _teamsMatchData[1][i]);
        }
        _teamsMidPos[0] = new List<int>();
        _teamsMidPos[1] = new List<int>();
        _teamsDefPos[0] = new List<int>();
        _teamsDefPos[1] = new List<int>();
        Clear();
        RecognizeFormation(Database.clubDB[_hostId].Formation, true);
        RecognizeFormation(Database.clubDB[_guestId].Formation, false);
        CalculateWeaknessOfTheDay();
        _isPlaying = false;
        CommentLine.Instance.StartingSettings();
        _StartStopButton.text = "Rozpocznij mecz";
    }

    public void StartStopButtonClick()
    {
        if (_end)
        {
            MyClub.Instance.ProcesssMatchStats(_matchStats, _competitionName);
            WindowsManager.Instance.ShowWindow("Club");
            return;
        }

        if (_isPlaying == false)
            StartCoroutine(CommentStart());
    }

    IEnumerator CommentStart()
    {
        UpdateChances();
        CommentLine.Instance.UpdateResult(_matchStats);
        if (_minute >= 90)
        {
            if(_time > 0)
                yield return new WaitForSeconds(_time);

            CommentLine.Instance.EndOfTheMatch();
            _end = true;
            _StartStopButton.text = "Zakończ mecz";
            Debug.Log("KONIEC MECZU!!!---Wynik: " + _matchStats[0].GetGoals() + " - " + _matchStats[1].GetGoals());
            StopAllCoroutines();
            yield break;
        }

        _isPlaying = true;

        if (_minute == 0)
        {
            CommentLine.Instance.StartingComment();
            MinutePassed();
            if(_time > 0) yield return new WaitForSeconds(_time);
            CommentLine.Instance.StartOfTheMatch();
            MinutePassed();
        }
        if (_minute < 90 && _minute > 1)
        {
            int ch = Random.Range(1, 100);
            if (ch >= 90)
            {
                _not = 0;
                if(_time > 0) yield return new WaitForSeconds(_time);
                CommentLine.Instance.InfoComment();
                MinutePassed();
                StartCoroutine(CommentStart());
            }
            if (ch > 5 + _goalChances && ch < 90)
            {
                _not++;
                if (_not > 8)
                {
                    if(_time > 0) yield return new WaitForSeconds(_time);
                    CommentLine.Instance.BoringPartOfTheMatch();
                    _not = 0;
                }
                MinutePassed();
                StartCoroutine(CommentStart());
            }
            if (ch <= 5 + _goalChances)
            {
                _not = 0;
                if(_time > 0) yield return new WaitForSeconds(_time);
                AttackFirstPhase();
                if(_time > 0) yield return new WaitForSeconds(_time);
                AttackSecondPhase();
            }
        }
    }

    void AttackFirstPhase()
    {
        GuestBall = Random.Range(1, 100) <= _hostChances ? 0 : 1;

        PlayerWithBall = Random.Range(1, _defLastPlayerNumber[GuestBall] + 1);

        CommentLine.Instance.AttackFirstPhase();
    }

    void AttackSecondPhase()
    {
        int counterPos = Random.Range(_midLastPlayerNumber[ReverseGuestBall] + 1, 11);
        float counterChance = _teams[ReverseGuestBall][counterPos].Tackle - _teams[GuestBall][PlayerWithBall].Pass;
        counterChance = (counterChance + 100) / 20;
        int dec = Random.Range(1, 37 + (int)counterChance);
        if (dec <= 10)
        {
            // środek podanie
            PlayerWithBall = Random.Range(_defLastPlayerNumber[GuestBall] + 1, _midLastPlayerNumber[GuestBall] + 1);
            CommentLine.Instance.PassMiddle();
            PlayerWithBall = _teamsMidPos[GuestBall][Random.Range(0, _teamsMidPos[GuestBall].Count)];
            StartCoroutine(AttackThirdPhase("middle"));
        }
        else if (dec <= 34)
        {
            int isRightWing = dec > 22 ? 0 : 1;

            int pos = Random.Range(_defLastPlayerNumber[GuestBall] + 1, _midLastPlayerNumber[GuestBall] + 1);
            PlayerWithBall = pos;

            CommentLine.Instance.PassToTheWing(isRightWing);

            if (_wingPos[GuestBall, isRightWing] != pos)
                PlayerWithBall = _wingPos[GuestBall, isRightWing];
            else
            {
                // jesli skrzydlowym jest pomocnik podajacy pilke
                // to na skrzydel pilke musi przejac obronca
                // jesli akcja na prawym skrzydel to ostatni obronca z listy (kolejnosc pilkarzy od lewej), jesli nie to piewszy 
                if (isRightWing == 1)
                    PlayerWithBall = _defLastPlayerNumber[GuestBall];
                else
                     PlayerWithBall = 1;
            }

            StartCoroutine(AttackThirdPhase(isRightWing == 0 ? "left" : "right"));
        }
        else if (dec <= 37)
        {
            PlayerWithBall = Random.Range(_defLastPlayerNumber[GuestBall] + 1, _midLastPlayerNumber[GuestBall] + 1);
            // Strzał
            CommentLine.Instance.DecidesToShoot();
            StartCoroutine(Shot(10));
        }
        else
        {
            // Kontratak
            PlayerWithBall = counterPos;
            GuestBall = ReverseGuestBall;
            CommentLine.Instance.InterceptionAndCounter();
            StartCoroutine(CounterAttack());
        }
    }

    void RecognizeFormation(string formation, bool host)
    {
        int isGuest = host ? 0 : 1;
        if (formation == "4-3-3")
        {
            _teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating + _teams[isGuest][4].Rating);
            _teamDef[isGuest] = (_teamDef[isGuest] / 4) + (_teamDef[isGuest] / 4);
            _defLastPlayerNumber[isGuest] = 4;
            _teamMid[isGuest] = (_teams[isGuest][5].Rating + _teams[isGuest][6].Rating + _teams[isGuest][7].Rating);
            _teamMid[isGuest] = (_teamMid[isGuest] / 3) + (_teamMid[isGuest] / 3);
            _midLastPlayerNumber[isGuest] = 7;
            _teamAtk[isGuest] = (_teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
            _teamAtk[isGuest] = (_teamAtk[isGuest] / 3) + (_teamAtk[isGuest] / 3);
            _teamsMidPos[isGuest].Add(10);
            _wingPos[isGuest, 0] = 8;
            _wingPos[isGuest, 1] = 9;
        }
        else if (formation == "4-2-3-1" || formation == "4-4-1-1")
        {
            _teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating + _teams[isGuest][4].Rating);
            _teamDef[isGuest] = (_teamDef[isGuest] / 4) + (_teamDef[isGuest] / 4);
            _defLastPlayerNumber[isGuest] = 4;
            _teamMid[isGuest] = (_teams[isGuest][5].Rating + _teams[isGuest][6].Rating);
            _teamMid[isGuest] = (_teamMid[isGuest] / 2) + (_teamMid[isGuest] / 3);
            _midLastPlayerNumber[isGuest] = 6;
            _teamAtk[isGuest] = (_teams[isGuest][7].Rating + _teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
            _teamAtk[isGuest] = (_teamAtk[isGuest] / 4) + (_teamAtk[isGuest] / 3);
            _teamsMidPos[isGuest].Add(7);
            _teamsMidPos[isGuest].Add(10);
            _wingPos[isGuest, 0] = 8;
            _wingPos[isGuest, 1] = 9;
        }
        else if (formation == "4-1-4-1")
        {
            _teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating + _teams[isGuest][4].Rating);
            _teamDef[isGuest] = (_teamDef[isGuest] / 4) + (_teamDef[isGuest] / 4);
            _defLastPlayerNumber[isGuest] = 4;
            _teamMid[isGuest] = (_teams[isGuest][5].Rating + _teams[isGuest][6].Rating + _teams[isGuest][7].Rating);
            _teamMid[isGuest] = (_teamMid[isGuest] / 3) + (_teamMid[isGuest] / 3);
            _midLastPlayerNumber[isGuest] = 6;
            _teamAtk[isGuest] = (_teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
            _teamAtk[isGuest] = (_teamAtk[isGuest] / 3) + (_teamAtk[isGuest] / 3);
            _teamsMidPos[isGuest].Add(6);
            _teamsMidPos[isGuest].Add(7);
            _teamsMidPos[isGuest].Add(10);
            _wingPos[isGuest, 0] = 8;
            _wingPos[isGuest, 1] = 9;
        }
        else if (formation == "4-3-1-2")
        {
            _teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating + _teams[isGuest][4].Rating);
            _teamDef[isGuest] = (_teamDef[isGuest] / 4) + (_teamDef[isGuest] / 4);
            _defLastPlayerNumber[isGuest] = 4;
            _teamMid[isGuest] = (_teams[isGuest][5].Rating + _teams[isGuest][6].Rating + _teams[isGuest][7].Rating);
            _teamMid[isGuest] = (_teamMid[isGuest] / 3) + (_teamMid[isGuest] / 3);
            _midLastPlayerNumber[isGuest] = 7;
            _teamAtk[isGuest] = (_teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
            _teamAtk[isGuest] = (_teamAtk[isGuest] / 3) + (_teamAtk[isGuest] / 3);
            _teamsMidPos[isGuest].Add(8);
            _teamsMidPos[isGuest].Add(9);
            _teamsMidPos[isGuest].Add(10);
            _wingPos[isGuest, 0] = 6;
            _wingPos[isGuest, 1] = 7;
        }
        else if (formation == "4-4-2")
        {
            _teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating + _teams[isGuest][4].Rating);
            _teamDef[isGuest] = (_teamDef[isGuest] / 4) + (_teamDef[isGuest] / 4);
            _defLastPlayerNumber[isGuest] = 4;
            _teamMid[isGuest] = (_teams[isGuest][5].Rating + _teams[isGuest][6].Rating);
            _teamMid[isGuest] = (_teamMid[isGuest] / 2) + (_teamMid[isGuest] / 3);
            _midLastPlayerNumber[isGuest] = 6;
            _teamAtk[isGuest] = (_teams[isGuest][7].Rating + _teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
            _teamAtk[isGuest] = (_teamAtk[isGuest] / 4) + (_teamAtk[isGuest] / 3);
            _teamsMidPos[isGuest].Add(9);
            _teamsMidPos[isGuest].Add(10);
            _wingPos[isGuest, 0] = 7;
            _wingPos[isGuest, 1] = 8;
        }
        else if (formation == "5-3-2")
        {
            _teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating + _teams[isGuest][4].Rating + _teams[isGuest][5].Rating);
            _teamDef[isGuest] = (_teamDef[isGuest] / 5) + (_teamDef[isGuest] / 4);
            _defLastPlayerNumber[isGuest] = 5;
            _teamMid[isGuest] = (_teams[isGuest][6].Rating + _teams[isGuest][7].Rating);
            _teamMid[isGuest] = (_teamMid[isGuest] / 2) + (_teamMid[isGuest] / 3);
            _midLastPlayerNumber[isGuest] = 7;
            _teamAtk[isGuest] = (_teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
            _teamAtk[isGuest] = (_teamAtk[isGuest] / 3) + (_teamAtk[isGuest] / 3);
            _teamsMidPos[isGuest].Add(8);
            _teamsMidPos[isGuest].Add(9);
            _teamsMidPos[isGuest].Add(10);
            _wingPos[isGuest, 0] = 6;
            _wingPos[isGuest, 1] = 7;
        }
        else if (formation == "3-4-1-2" || formation == "3-4-2-1")
        {
            _teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating);
            _teamDef[isGuest] = (_teamDef[isGuest] / 3) + (_teamDef[isGuest] / 4);
            _defLastPlayerNumber[isGuest] = 3;
            _teamMid[isGuest] = (_teams[isGuest][4].Rating + _teams[isGuest][5].Rating + _teams[isGuest][6].Rating + _teams[isGuest][7].Rating);
            _teamMid[isGuest] = (_teamMid[isGuest] / 4) + (_teamMid[isGuest] / 3);
            _midLastPlayerNumber[isGuest] = 7;
            _teamAtk[isGuest] = (_teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
            _teamAtk[isGuest] = (_teamAtk[isGuest] / 3) + (_teamAtk[isGuest] / 3);
            _teamsMidPos[isGuest].Add(8);
            _teamsMidPos[isGuest].Add(9);
            _teamsMidPos[isGuest].Add(10);
            _wingPos[isGuest, 0] = 6;
            _wingPos[isGuest, 1] = 7;
        }
        if (_defLastPlayerNumber[isGuest] == 5)
        {
            _teamsDefPos[isGuest].Add(2);
            _teamsDefPos[isGuest].Add(3);
            _teamsDefPos[isGuest].Add(4);
            _defWingPos[isGuest, 0] = 1;
            _defWingPos[isGuest, 1] = 5;
        }
        else if (_defLastPlayerNumber[isGuest] == 4)
        {
            _teamsDefPos[isGuest].Add(2);
            _teamsDefPos[isGuest].Add(3);
            _defWingPos[isGuest, 0] = 1;
            _defWingPos[isGuest, 1] = 4;
        }
        else if (_defLastPlayerNumber[isGuest] == 3)
        {
            _teamsDefPos[isGuest].Add(1);
            _teamsDefPos[isGuest].Add(2);
            _teamsDefPos[isGuest].Add(3);
            _defWingPos[isGuest, 0] = 1;
            _defWingPos[isGuest, 1] = 3;
        }

    }

    void UpdateChances()
    {
        _hostChances = 50;
        // difference between host's attack and guest's defense
        float advantage = ((_teamAtk[0] - _teamDef[1]) / 10) / 3;
        advantage += ((_teamDef[0] - _teamAtk[1]) / 10) / 3;
        advantage += ((_teamMid[0] - _teamMid[1]) / 10) / 3;
        _hostChances += 4 + Mathf.RoundToInt(advantage);
      
        _hostChances -= _weakness;

        if (_hostChances >= 160)
            _hostChances = 95;
        else if (_hostChances >= 90)
            _hostChances = 90;
        else if (_hostChances <= -160)
            _hostChances = 5;
        else if (_hostChances < 10)
            _hostChances = 10;

        float ratingDiff = Mathf.Abs(Database.clubDB[_hostId].Rate - Database.clubDB[_guestId].Rate) / 10;
        float goalChances = (5 + (ratingDiff / 3)) / _curiosity + _changedCuriosity;
        _goalChances = Mathf.Clamp(Mathf.RoundToInt(goalChances), 4, 35);
    }

    void CalculateWeaknessOfTheDay()
    {
        _weakness = 0;
        int rnd = Random.Range(1, 101);

        if (rnd > 10)
            return;

        int ran = Random.Range(0, Database.clubDB[_hostId].Rate + Database.clubDB[_guestId].Rate);

        bool hostIsWeakToday = Database.clubDB[_hostId].Rate > ran;

        _weakness = hostIsWeakToday ? 10 : -10;

        if (Database.clubDB[_hostId].Rate > Database.clubDB[_guestId].Rate)
            _curiosity = hostIsWeakToday ? 2 : 0.5f;
        else if (Database.clubDB[_hostId].Rate < Database.clubDB[_guestId].Rate)
            _curiosity = hostIsWeakToday ? 0.5f : 2;
        else
            _curiosity = 3;
    }

    void Clear()
    {
        _teamsMidPos[0].Clear();
        _teamsMidPos[1].Clear();
        _teamsDefPos[0].Clear();
        _teamsDefPos[1].Clear();
        _matchStats[0].Reset();
        _matchStats[1].Reset();
        _minute = 0;
    }

    void UpdateCuriosity()
    {
        // conversion from 0 , 1 to -1,1
        int multiplier = GuestBall / 3 - 1;

        if (Database.clubDB[_hostId].Rate > Database.clubDB[_guestId].Rate)
            _changedCuriosity += 5 * multiplier;
        else if (Database.clubDB[_hostId].Rate < Database.clubDB[_guestId].Rate)
            _changedCuriosity += -5 * multiplier;
        else
            _changedCuriosity -= 2;
    }

    void GoalScored()
    {
        _matchStats[GuestBall].GoalScored();
        _matchStats[GuestBall].AddScorer(_teams[GuestBall][PlayerWithBall], _teamName[GuestBall], 1);
        _teamsMatchData[GuestBall][PlayerWithBall].MatchRating += .5f;
        _teams[GuestBall][PlayerWithBall].AddStatistic(_competitionName, Footballer.PlayerStatistics.StatName.Goals);
        _teamsMatchData[GuestBall].ForEach(data => data.MatchRating += .5f);
        _teamsMatchData[ReverseGuestBall].ForEach(data => data.MatchRating -= .5f);
        if (_prevPlayerWithBall != -1)
        {
            _teams[GuestBall][_prevPlayerWithBall].AddStatistic(_competitionName, Footballer.PlayerStatistics.StatName.Assists);
            _teamsMatchData[GuestBall][_prevPlayerWithBall].MatchRating += .25f;
            _prevPlayerWithBall = -1;
        }
        UpdateCuriosity();
    }

    void MinutePassed()
    {
        _minute++;
        for (int j = 0; j < 2; j++)
            for (int i = 0; i < _teams[j].Count; i++)
                _teams[j][i].GainFatigue(i == 0);
        _HostSquadStatsUI.ForEach(f => f.UpdateState());
        _GuestSquadStatsUI.ForEach(f => f.UpdateState());
    }

    IEnumerator FreeKick(bool isPenalty)
    {
        // no assist when free kick
        PlayerWithBall = -1;

        if(_time > 0) 
            yield return new WaitForSeconds(_time);

        if (isPenalty)
        {
            MinutePassed();
            int[] penaltyPlayers = new int[10];
            for (int i = 1; i < 11; i++)
            {
                penaltyPlayers[i-1] = i;
            }
            penaltyPlayers = penaltyPlayers.OrderByDescending(x => _teams[GuestBall][x].Penalty).ToArray();
            PlayerWithBall = penaltyPlayers[0];
            CommentLine.Instance.PreparingToPenalty();
            MinutePassed();

            if(_time > 0) 
                yield return new WaitForSeconds(_time);

            float plus = (_teams[GuestBall][PlayerWithBall].Penalty / 5) - (_teams[ReverseGuestBall][0].Rating / 7);
            int rnd = Random.Range(1, 101);
            if (rnd < 80 + plus)
            {
                // gol
                GoalScored();
                CommentLine.Instance.PenaltyGoal();
            }
            else
                CommentLine.Instance.PenaltyMissed();

            _matchStats[GuestBall].ShotTaken();
            MinutePassed();
            StartCoroutine(CommentStart());
        }
        else
        {
            //TODO: free kick
        }
    }

    IEnumerator Shot(int difficulty)
    {
        if(_time > 0) 
            yield return new WaitForSeconds(_time);

        if (difficulty == 10)
        {
            // strzał z bardzo daleka
            _matchStats[GuestBall].ShotTaken();
            MinutePassed();
            int x = Random.Range(1, 101);
            float plus = (_teams[GuestBall][PlayerWithBall].Shoot / 5) - (_teams[ReverseGuestBall][0].Rating / 10);
            if (x <= 20)
            {
                // rożny
                CommentLine.Instance.LongShotCorner();
                StartCoroutine(Corner());
            }
            else
            {
                if (x <= 30 + plus)
                {
                    GoalScored();
                    CommentLine.Instance.LongShotGoal();
                }
                else
                    CommentLine.Instance.LongShotMiss();

                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 8)
        {
            // strzal z okolo 20 metrow przy kontrataku
            _matchStats[GuestBall].ShotTaken();
            MinutePassed();
            int keeperPlus = (int)(20 + _teams[ReverseGuestBall][0].Rating / 3);
            int shooterPlus = (int)(20 + _teams[0][PlayerWithBall].Shoot / 5);
            int x = Random.Range(1, keeperPlus + shooterPlus + 10);
            if (x <= keeperPlus)
            {
                if (Random.Range(1, 31) <= _teams[ReverseGuestBall][0].Rating / 10)  // maks 10/30 szansy ze zlapie
                {
                    //lapie
                    CommentLine.Instance.CounterAttackSave();
                    MinutePassed();
                    StartCoroutine(CommentStart());
                }
                else
                {
                    //korner
                    CommentLine.Instance.CounterAttackCorner();
                    StartCoroutine(Corner());
                }
            }
            else
            {
                if (x <= keeperPlus + shooterPlus)
                {
                    GoalScored();
                    CommentLine.Instance.CounterAttackGoal();
                }
                else
                {
                    CommentLine.Instance.CounterAttackMiss();
                }
                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 5)  // okolo 16 metrow normalny atak
        {
            _matchStats[GuestBall].ShotTaken();
            MinutePassed();
            int keeperPlus = (int)(5 + _teams[ReverseGuestBall][0].Rating / 5);
            int shooterPlus = (int)(15 + _teams[GuestBall][PlayerWithBall].Shoot / 3);
            int x = Random.Range(1, keeperPlus + shooterPlus + 15);
            if (x <= keeperPlus)
            {
                if (Random.Range(1, 51) <= _teams[ReverseGuestBall][0].Rating / 10)  // maks 10/50 szansy ze zlapie
                {
                    //lapie
                    CommentLine.Instance.NormalAttackSave();
                    MinutePassed();
                    StartCoroutine(CommentStart());
                }
                else
                {
                    //korner
                    CommentLine.Instance.NormalAttackCorner();
                    StartCoroutine(Corner());
                }
            }
            else
            {
                if (x <= keeperPlus + shooterPlus)
                {
                    GoalScored();
                    CommentLine.Instance.NormalAttackGoal();
                }
                else
                    CommentLine.Instance.NormalAttackMiss();

                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 2)    //   Sam na sam
        {
            _matchStats[GuestBall].ShotTaken();
            MinutePassed();
            int keeperPlus = (int)(10 + _teams[ReverseGuestBall][0].Rating / 5);
            int shooterPlus = (int)(25 + _teams[GuestBall][PlayerWithBall].Shoot / 2);
            int x = Random.Range(1, keeperPlus + shooterPlus + 5);
            if (x <= keeperPlus)
            {
                if (Random.Range(1, 51) <= _teams[ReverseGuestBall][0].Rating / 10)  // maks 10/50 szansy ze zlapie albo nie bedzie roznego
                {
                    //lapie
                    CommentLine.Instance.OneOnOneAttackSave();
                    MinutePassed();
                    StartCoroutine(CommentStart());
                }
                else
                {
                    //korner
                    CommentLine.Instance.OneOnOneAttackCorner();
                    StartCoroutine(Corner());
                }
            }
            else
            {
                if (x <= keeperPlus + shooterPlus)
                {
                    GoalScored();
                    CommentLine.Instance.OneOnOneAttackGoal();
                }
                else
                    CommentLine.Instance.OneOnOneAttackMiss();

                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 3)    //   główka
        {
            _matchStats[GuestBall].ShotTaken();
            MinutePassed();
            int keeperPlus = (int)(5 + _teams[ReverseGuestBall][0].Rating / 5);
            int shooterPlus = (int)(10 + _teams[GuestBall][PlayerWithBall].Heading / 2);
            int x = Random.Range(1, keeperPlus + shooterPlus + 10);
            if (x <= keeperPlus)
            {
                if (Random.Range(1, 31) <= _teams[ReverseGuestBall][0].Rating / 10)  // maks 10/30 szansy ze zlapie albo nie bedzie roznego
                {
                    //lapie
                    CommentLine.Instance.HeaderSave();
                    MinutePassed();
                    StartCoroutine(CommentStart());
                }
                else
                {
                    //korner
                    CommentLine.Instance.HeaderCorner();
                    StartCoroutine(Corner());
                }

            }
            else
            {
                if (x <= keeperPlus + shooterPlus)
                {
                    GoalScored();
                    CommentLine.Instance.HeaderGoal();
                }
                else
                {
                    CommentLine.Instance.HeaderMiss();
                }
                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
    }

    IEnumerator CounterAttack()
    {
        // getting ball from opponent
        _prevPlayerWithBall = -1;

        if(_time > 0) 
            yield return new WaitForSeconds(_time);

        int rnd = Random.Range(1, 100);
        float x = 20 - _teams[GuestBall][PlayerWithBall].Pass / 10;
        if (rnd <= 30)
        {
            CommentLine.Instance.CounterAttackShotTry();

            if(_time > 0) 
                yield return new WaitForSeconds(_time);

            StartCoroutine(Shot(8));
        }
        else if (rnd <= 30 + x)
        {
            CommentLine.Instance.CounterAttackFailedPass();
            MinutePassed();
            StartCoroutine(CommentStart());
        }
        else
        {
            CommentLine.Instance.CounterAttackSuccessPass();

            List<int> indices = Enumerable.Range(_midLastPlayerNumber[GuestBall] + 1, 10 - _midLastPlayerNumber[GuestBall]).ToList();
            indices.Remove(PlayerWithBall);

            PlayerWithBall = indices[Random.Range(0, indices.Count)];

            CommentLine.Instance.CounterAttackPreShot();

            if(_time > 0) 
                yield return new WaitForSeconds(_time);

            int chan = Random.Range(1, 101);
            if (chan < 70)
            {
                CommentLine.Instance.CounterAttackPenaltyFoul();
                StartCoroutine(FreeKick(true));
            }
            else
            {
                StartCoroutine(Shot(2));
            }
        }
    }

    IEnumerator Corner()
    {
        if(_time > 0) 
            yield return new WaitForSeconds(_time);

        List<Footballer> cornerPlayers = new List<Footballer>();
        for (int i = 1; i < 11; i++)
            cornerPlayers.Add(_teams[GuestBall][i]);

        cornerPlayers = cornerPlayers.OrderByDescending(x => x.Corner).ToList();
        CommentLine.Instance.CornerExecution(cornerPlayers[0]);
        float border = 65 + ((cornerPlayers[0].FreeKicks + cornerPlayers[0].Pass) / 10);
        // executor of corner cannot jump to the header...
        cornerPlayers.RemoveAt(0);
        if (Random.Range(1, 101) <= border)
        {
            List<Footballer> attackerHeaders = new List<Footballer>();
            List<Footballer> defenderHeaders = new List<Footballer>();
            attackerHeaders = cornerPlayers.OrderByDescending(x => x.Heading).ToList();
            for (int i = 1; i < 11; i++)
            {
                defenderHeaders.Add(_teams[ReverseGuestBall][i]);
            }
            int y = Random.Range(0, 30); // losowanie indeksu piłkarzy, którzy będą walczyć o piłkę
            y /= 10;
            for (int i = 1; i < 11; i++)
            {
                if (attackerHeaders[y].Id == _teams[GuestBall][i].Id)
                {
                    PlayerWithBall = i;
                    break;
                }
            }
            // TODO: instead of comparing rating, hould compare positioning or smth
            // główka
            if (Random.Range(1, 101) <= 25 + ((attackerHeaders[y].Rating - defenderHeaders[y].Rating) / 5))
            {
                // zgubienie obrońcy i strzał głową
                if(_time > 0) 
                    yield return new WaitForSeconds(_time);

                CommentLine.Instance.FreeHeader();

                if(_time > 0) 
                    yield return new WaitForSeconds(_time);

                StartCoroutine(Shot(3));
            }
            else
            {
                if(_time > 0) 
                    yield return new WaitForSeconds(_time);

                if (Random.Range(1, 101) <= 50 + ((attackerHeaders[y].Heading - defenderHeaders[y].Heading) / 2))
                {
                    CommentLine.Instance.ContestedHeader();

                    if(_time > 0) 
                        yield return new WaitForSeconds(_time);

                    StartCoroutine(Shot(3));
                }
                else
                {
                    CommentLine.Instance.DefenderWinsHeader(defenderHeaders[y]);
                    MinutePassed();
                    StartCoroutine(CommentStart());
                }
            }
        }
        else
        {
            if(_time > 0) 
                yield return new WaitForSeconds(_time);

            CommentLine.Instance.FailedCross();
            MinutePassed();
            StartCoroutine(CommentStart());
        }
    }

    IEnumerator AttackThirdPhase(string direction)
    {
        MinutePassed();

        if(_time > 0) 
            yield return new WaitForSeconds(_time);

        if (direction == "left" || direction == "right")
        {
            int dir = direction == "right" ? 1 : 0;
            CommentLine.Instance.TryingToDodge();

            if(_time > 0) 
                yield return new WaitForSeconds(_time);

            float plus = ((_teams[GuestBall][PlayerWithBall].Dribling + _teams[GuestBall][PlayerWithBall].Speed) - (_teams[ReverseGuestBall][_defWingPos[ReverseGuestBall, dir]].Tackle + _teams[ReverseGuestBall][_defWingPos[ReverseGuestBall, dir]].Speed)) / 3;
            if (Random.Range(1, 101) < 55 + plus)
            {
                if (Random.Range(1, 101) <= 70)
                {
                    CommentLine.Instance.DecidesToCross();
                    float border = 40 + (_teams[0][PlayerWithBall].Pass / 3);           //----------------- ewentualnie zmniejszyc mnożnik gdyby za dużo goli z główki
                    int acc = Random.Range(1, 100);
                    if (acc <= border)
                    {
                        List<int> playersToChoose = new List<int>();
                        for (int i = _midLastPlayerNumber[GuestBall] + 1; i < 11; i++)
                            playersToChoose.Add(i);

                        playersToChoose.Remove(PlayerWithBall);

                        int attackerHeader = playersToChoose[Random.Range(0, playersToChoose.Count)];
                        
                        playersToChoose.Clear();
                        for (int i = 1; i < _defLastPlayerNumber[GuestBall] + 1; i++)
                            playersToChoose.Add(i);

                        playersToChoose.Remove(_defWingPos[ReverseGuestBall, dir]);

                        int defenderHeader = playersToChoose[Random.Range(0, playersToChoose.Count)];

                        // główka
                        if (Random.Range(1, 101) <= (30 + (_teams[GuestBall][attackerHeader].Rating - _teams[ReverseGuestBall][defenderHeader].Rating) / 3))
                        {
                            // zgubienie obrońcy i strzał głową
                            if(_time > 0) 
                                yield return new WaitForSeconds(_time);

                            PlayerWithBall = attackerHeader;
                            CommentLine.Instance.FreeHeader();

                            if(_time > 0) 
                                yield return new WaitForSeconds(_time);

                            StartCoroutine(Shot(3));
                        }
                        else
                        {
                            if(_time > 0) 
                                yield return new WaitForSeconds(_time);

                            if (Random.Range(1, 101) <= 50 + ((_teams[GuestBall][attackerHeader].Heading - _teams[ReverseGuestBall][defenderHeader].Heading) / 3))
                            {
                                PlayerWithBall = attackerHeader;
                                CommentLine.Instance.ContestedHeader();

                                if(_time > 0) 
                                    yield return new WaitForSeconds(_time);

                                StartCoroutine(Shot(3));
                            }
                            else
                            {
                                CommentLine.Instance.DefenderWinsHeader(_teams[ReverseGuestBall][defenderHeader]);
                                MinutePassed();
                                StartCoroutine(CommentStart());
                            }
                        }
                    }
                    else
                    {
                        if(_time > 0) 
                            yield return new WaitForSeconds(_time);

                        CommentLine.Instance.FailedCross();
                        MinutePassed();
                        StartCoroutine(CommentStart());
                    }
                }
                else
                {
                    CommentLine.Instance.DecidesToShootInsteadOfCrossing();
                    StartCoroutine(Shot(5));
                }
            }
            else
            {
                CommentLine.Instance.FailedWingDribble(dir);
                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
        else if (direction == "middle")
        {

            CommentLine.Instance.TryingToDodge();

            if(_time > 0) 
                yield return new WaitForSeconds(_time);

            int firstDef = _teamsDefPos[ReverseGuestBall][Random.Range(0, _teamsDefPos[ReverseGuestBall].Count)];
            float plus = ((_teams[GuestBall][PlayerWithBall].Dribling + _teams[GuestBall][PlayerWithBall].Speed) - (_teams[ReverseGuestBall][firstDef].Tackle + _teams[ReverseGuestBall][firstDef].Speed)) / 3;
            if (Random.Range(1, 101) < 45 + plus)
            {
                //podanie bądź strzał
                if (Random.Range(1, 101) <= 65)
                {
                    CommentLine.Instance.DecidesToPass();
                    float border = 30 + _teams[GuestBall][PlayerWithBall].Pass / 3;
                    if (Random.Range(1, 101) <= border)
                    {
                        List<int> playersToChoose = new List<int>();
                        for (int i = _midLastPlayerNumber[GuestBall] + 1; i < 11; i++)
                            playersToChoose.Add(i);

                        playersToChoose.Remove(PlayerWithBall);

                        PlayerWithBall = playersToChoose[Random.Range(0, playersToChoose.Count)];

                        playersToChoose = new(_teamsDefPos[ReverseGuestBall]);

                        playersToChoose.Remove(firstDef);

                        int defenderIndex = playersToChoose[Random.Range(0, playersToChoose.Count)];

                        if(_time > 0) 
                            yield return new WaitForSeconds(_time);

                        CommentLine.Instance.ChanceForOneOnOne();

                        if(_time > 0) 
                            yield return new WaitForSeconds(_time);

                        if (Random.Range(1, 101) <= (50 + (_teams[GuestBall][PlayerWithBall].Dribling - _teams[ReverseGuestBall][defenderIndex].Tackle) / 3))
                        {
                            CommentLine.Instance.OneToOneSituation();

                            if(_time > 0) 
                                yield return new WaitForSeconds(_time);

                            StartCoroutine(Shot(2));
                        }
                        else
                        {
                            PlayerWithBall = defenderIndex;
                            CommentLine.Instance.FailedChanceOneToOne();
                            MinutePassed();
                            StartCoroutine(CommentStart());
                        }
                    }
                    else
                    {
                        if(_time > 0) 
                            yield return new WaitForSeconds(_time);

                        CommentLine.Instance.FailedPass();
                        MinutePassed();
                        StartCoroutine(CommentStart());
                    }
                }
                else
                {
                    CommentLine.Instance.DecidesToShootInsteadOfPassing();
                    StartCoroutine(Shot(5));
                }
            }
            else
            {
                CommentLine.Instance.FailedMidDribble(_teams[ReverseGuestBall][firstDef]);
                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
	}
}
