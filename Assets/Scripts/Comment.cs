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

    float _time = 0.1f;
    bool _isPlaying = false;
    int _minute = 0;
    int _hostId, _guestId;
    List<Footballer>[] _teams = new List<Footballer>[2];
    float[] _teamDef = new float[2], _teamMid = new float[2], _teamAtk = new float[2];
    int _hostChances, _goalChances;
    MatchStats[] _matchStats = new MatchStats[2];
    string[] _teamName = new string[2];
    int _playerWithBall;  // piłkarz przy piłce
    int _guestBall;  // posiadanie piłki
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
    public int GetGuestBall() => _guestBall;
    public int SetGuestBall(int val) => _guestBall = val;
    public int GetPlayerWithBall() => _playerWithBall;
    public int SetPlayerWithBall(int val) => _playerWithBall = val;
    #endregion
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
    public void OnSimSpeedChanged(float val)
    {
        _time = val;
    }

    public void SimulateMatch()
    {
        _time = 0;
        StartCoroutine(CommentStart());
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
        for (int i = 0; i < 11; i++)
        {
            _teams[0].Add(Database.footballersDB[Database.clubDB[_hostId].FootballersIDs[i]]);
            _teams[0][i].AddStatistic(_competitionName, Footballer.PlayerStatistics.StatName.MatchesPlayed);
            _teams[1].Add(Database.footballersDB[Database.clubDB[_guestId].FootballersIDs[i]]);
            _teams[1][i].AddStatistic(_competitionName, Footballer.PlayerStatistics.StatName.MatchesPlayed);
        }
        _teamsMidPos[0] = new List<int>();
        _teamsMidPos[1] = new List<int>();
        _teamsDefPos[0] = new List<int>();
        _teamsDefPos[1] = new List<int>();
        Clear();
        RecognizeFormation(Database.clubDB[_hostId].Formation, true);
        RecognizeFormation(Database.clubDB[_guestId].Formation, false);
        CalculateWeakness();
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
        }
        if (_isPlaying == false)
        {
            StartCoroutine(CommentStart());
        }
    }
    // główna pętla
    IEnumerator CommentStart()
    {
        UpdateChances();
        CommentLine.Instance.UpdateResult(_matchStats);
        if (_minute >= 90)
        {
            if(_time > 0) yield return new WaitForSeconds(_time);
            CommentLine.Instance.EndOfTheMatch();
            _end = true;
            _StartStopButton.text = "Zakończ mecz";
            Debug.Log("KONIEC MECZU!!!---Wynik: " + _matchStats[0].GetGoals() + "-" + _matchStats[1].GetGoals() + " " + Time.time);
            StopAllCoroutines();
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
        int who = Random.Range(1, 100);

        if (who <= _hostChances)
            _guestBall = 0;
        else
            _guestBall = 1;

        int pos = Random.Range(1, _defLastPlayerNumber[_guestBall] + 1);
        _playerWithBall = pos;
        CommentLine.Instance.AttackFirstPhase();
    }
    void AttackSecondPhase()
    {

        int counterPos = Random.Range(_midLastPlayerNumber[GetReverseIsGuestBall()] + 1, 11);
        float counterChance = _teams[GetReverseIsGuestBall()][counterPos].Tackle - _teams[_guestBall][_playerWithBall].Pass;
        counterChance = (counterChance + 10) / 2;
        int dec = Random.Range(1, 37 + (int)counterChance);
        if (dec <= 10)
        {
            // środek podanie
            _playerWithBall = Random.Range(_defLastPlayerNumber[_guestBall] + 1, _midLastPlayerNumber[_guestBall] + 1);
            CommentLine.Instance.PassMiddle();
            _playerWithBall = _teamsMidPos[_guestBall][Random.Range(0, _teamsMidPos[_guestBall].Count)];
            StartCoroutine(AttackThirdPhase("middle"));
        }
        else if (dec <= 34 && dec > 10)
        {
            int isRightWing = dec > 22 ? 0 : 1;

            int pos = Random.Range(_defLastPlayerNumber[_guestBall] + 1, _midLastPlayerNumber[_guestBall] + 1);
            _playerWithBall = pos;

            CommentLine.Instance.PassToTheWing(isRightWing);

            if (_wingPos[_guestBall, isRightWing] != pos)
                _playerWithBall = _wingPos[_guestBall, isRightWing];
            else
            {
                // jesli skrzydlowym jest pomocnik podajacy pilke
                // to na skrzydel pilke musi przejac obronca
                // jesli akcja na prawym skrzydel to ostatni obronca z listy (kolejnosc pilkarzy od lewej), jesli nie to piewszy 
                if (isRightWing == 1)
                    _playerWithBall = _defLastPlayerNumber[_guestBall];
                else
                     _playerWithBall = 1;
            }

            StartCoroutine(AttackThirdPhase(isRightWing == 0 ? "left" : "right"));
        }
        else if (dec > 34 && dec <= 37)
        {
            _playerWithBall = Random.Range(_defLastPlayerNumber[_guestBall] + 1, _midLastPlayerNumber[_guestBall] + 1);
            // Strzał
            CommentLine.Instance.DecidesToShoot();
            StartCoroutine(Shot(10));
        }
        else if (dec > 37)
        {
            // Kontratak
            _playerWithBall = counterPos;
            _guestBall = GetReverseIsGuestBall();
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
        float advantage = (_teamAtk[0] - _teamDef[1]) * 2;
        advantage += (_teamDef[0] - _teamAtk[1]) * 2;
        advantage += (_teamMid[0] - _teamMid[1]) * 2;
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

        float ratingDiff = Mathf.Abs(Database.clubDB[_hostId].Rate - Database.clubDB[_guestId].Rate);
        float goalChances = (5 + (ratingDiff * 3)) / _curiosity + _changedCuriosity;
        _goalChances = Mathf.RoundToInt(goalChances);
        _goalChances = Mathf.Clamp(_goalChances, 4, 35);
    }
    void CalculateWeakness()
    {
        _weakness = 0;
        int rnd = Random.Range(1, 101);
        if (rnd <= 10)
        {
            int ran = Random.Range(0, (Database.clubDB[_hostId].Rate * 10 + Database.clubDB[_guestId].Rate * 10));
            if (Database.clubDB[_hostId].Rate * 10 > ran)
            {
                _weakness = 10;
                if (Database.clubDB[_hostId].Rate > Database.clubDB[_guestId].Rate)
                {
                    _curiosity = 2;
                }
                if (Database.clubDB[_hostId].Rate < Database.clubDB[_guestId].Rate)
                {
                    _curiosity = 0.5f;
                }
                if (Database.clubDB[_hostId].Rate == Database.clubDB[_guestId].Rate)
                {
                    _curiosity = 3;
                }
            }
            else
            {
                _weakness = -10;
                if (Database.clubDB[_hostId].Rate > Database.clubDB[_guestId].Rate)
                {
                    _curiosity = 0.5f;
                }
                if (Database.clubDB[_hostId].Rate < Database.clubDB[_guestId].Rate)
                {
                    _curiosity = 2;
                }
                if (Database.clubDB[_hostId].Rate == Database.clubDB[_guestId].Rate)
                {
                    _curiosity = 3;
                }
            }
        }
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
    void ChangeCuriosity()
    {
        // conversion from 0 , 1 to -1,1
        int multiplier = _guestBall * 2 - 1;

        if (Database.clubDB[_hostId].Rate > Database.clubDB[_guestId].Rate)
        {
            _changedCuriosity += 5 * multiplier;
        }
        else if (Database.clubDB[_hostId].Rate < Database.clubDB[_guestId].Rate)
        {
            _changedCuriosity += -5 * multiplier;
        }
        else
        {
            _changedCuriosity -= 2;
        }
    }
    void GoalScored()
    {
        _matchStats[_guestBall].GoalScored();
        _matchStats[_guestBall].AddScorer(_teams[_guestBall][_playerWithBall], _teamName[_guestBall], 1);
        _teams[_guestBall][_playerWithBall].AddStatistic(_competitionName, Footballer.PlayerStatistics.StatName.Goals);
        ChangeCuriosity();
    }
    void MinutePassed()
    {
        _minute++;
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < _teams[j].Count; i++)
            {
                _teams[j][i].GainFatigue(i == 0);
            }
        }
    }
    IEnumerator FreeKick(bool isPenalty)
    {
        if(_time > 0) yield return new WaitForSeconds(_time);
        if (isPenalty)
        {
            MinutePassed();
            int[] penaltyPlayers = new int[10];
            for (int i = 1; i < 11; i++)
            {
                penaltyPlayers[i-1] = i;
            }
            penaltyPlayers = penaltyPlayers.OrderByDescending(x => _teams[_guestBall][x].Penalty).ToArray();
            _playerWithBall = penaltyPlayers[0];
            CommentLine.Instance.PreparingToPenalty();
            MinutePassed();
            if(_time > 0) yield return new WaitForSeconds(_time);
            float plus = _teams[_guestBall][_playerWithBall].Penalty - (1.5f * _teams[GetReverseIsGuestBall()][0].Rating);
            int rnd = Random.Range(1, 101);
            if (rnd < 80 + plus)
            {
                // gol
                GoalScored();
                CommentLine.Instance.PenaltyGoal();
            }
            else
            {
                CommentLine.Instance.PenaltyMissed();
            }
            _matchStats[_guestBall].ShotTaken();
            MinutePassed();
            StartCoroutine(CommentStart());
        }
        else
        {

        }
    }
    IEnumerator Shot(int difficulty)
    {
        if(_time > 0) yield return new WaitForSeconds(_time);
        if (difficulty == 10)
        {
            // strzał z bardzo daleka
            _matchStats[_guestBall].ShotTaken();
            MinutePassed();
            int x = Random.Range(1, 101);
            float plus = (_teams[_guestBall][_playerWithBall].Shoot * 2) - _teams[GetReverseIsGuestBall()][0].Rating;
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
            _matchStats[_guestBall].ShotTaken();
            MinutePassed();
            float keeperPlus = 20 + _teams[GetReverseIsGuestBall()][0].Rating * 3;
            float shooterPlus = 20 + _teams[0][_playerWithBall].Shoot * 2;
            int x = Random.Range(1, (int)keeperPlus + (int)shooterPlus + 10);
            if (x <= keeperPlus)
            {
                if (Random.Range(1, 31) <= _teams[GetReverseIsGuestBall()][0].Rating)  // maks 10/30 szansy ze zlapie
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
            _matchStats[_guestBall].ShotTaken();
            MinutePassed();
            float keeperPlus = 5 + _teams[GetReverseIsGuestBall()][0].Rating * 2;
            float shooterPlus = 15 + _teams[_guestBall][_playerWithBall].Shoot * 4;
            int x = Random.Range(1, (int)keeperPlus + (int)shooterPlus + 15);
            if (x <= keeperPlus)
            {
                if (Random.Range(1, 51) <= _teams[GetReverseIsGuestBall()][0].Rating)  // maks 10/50 szansy ze zlapie
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
            _matchStats[_guestBall].ShotTaken();
            MinutePassed();
            float keeperPlus = 10 + _teams[GetReverseIsGuestBall()][0].Rating * 2;
            float shooterPlus = 25 + _teams[_guestBall][_playerWithBall].Shoot * 5;
            int x = Random.Range(1, (int)keeperPlus + (int)shooterPlus + 5);
            if (x <= keeperPlus)
            {
                if (Random.Range(1, 51) <= _teams[GetReverseIsGuestBall()][0].Rating)  // maks 10/50 szansy ze zlapie albo nie bedzie roznego
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
            _matchStats[_guestBall].ShotTaken();
            MinutePassed();
            float keeperPlus = 5 + _teams[GetReverseIsGuestBall()][0].Rating * 2;
            float shooterPlus = 10 + _teams[_guestBall][_playerWithBall].Heading * 5;
            int x = Random.Range(1, (int)keeperPlus + (int)shooterPlus + 10);
            if (x <= keeperPlus)
            {
                if (Random.Range(1, 31) <= _teams[GetReverseIsGuestBall()][0].Rating)  // maks 10/30 szansy ze zlapie albo nie bedzie roznego
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
        if(_time > 0) yield return new WaitForSeconds(_time);
        int rnd = Random.Range(1, 100);
        float x = 20 - _teams[_guestBall][_playerWithBall].Pass;
        if (rnd <= 30)
        {
            CommentLine.Instance.CounterAttackShotTry();
            if(_time > 0) yield return new WaitForSeconds(_time);
            StartCoroutine(Shot(8));
        }
        else if (rnd > 30 && rnd <= 30 + x)
        {
            CommentLine.Instance.CounterAttackFailedPass();
            MinutePassed();
            StartCoroutine(CommentStart());
        }
        else
        {
            CommentLine.Instance.CounterAttackSuccessPass();
            int newPos;
            do 
            {
                newPos = Random.Range(_midLastPlayerNumber[_guestBall] + 1, 11);
            } 
            while (_playerWithBall == newPos);

            _playerWithBall = newPos;
            CommentLine.Instance.CounterAttackPreShot();
            if(_time > 0) yield return new WaitForSeconds(_time);
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
        if(_time > 0) yield return new WaitForSeconds(_time);
        List<Footballer> cornerPlayers = new List<Footballer>();
        for (int i = 1; i < 11; i++)
        {
            cornerPlayers.Add(_teams[_guestBall][i]);
        }
        cornerPlayers = cornerPlayers.OrderByDescending(x => x.Corner).ToList();
        CommentLine.Instance.CornerExecution(cornerPlayers[0]);
        float border = 65 + cornerPlayers[0].FreeKicks + cornerPlayers[0].Pass;
        if (Random.Range(1, 101) <= border)
        {
            List<Footballer> attackerHeaders = new List<Footballer>();
            List<Footballer> defenderHeaders = new List<Footballer>();
            attackerHeaders = cornerPlayers.OrderByDescending(x => x.Heading).ToList();
            for (int i = 1; i < 11; i++)
            {
                defenderHeaders.Add(_teams[GetReverseIsGuestBall()][i]);
            }
            int y = Random.Range(0, 30); // losowanie indeksu piłkarzy, którzy będą walczyć o piłkę
            y /= 10;
            for (int i = 1; i < 11; i++)
            {
                if (attackerHeaders[y].Id == _teams[_guestBall][i].Id)
                {
                    _playerWithBall = i;
                    break;
                }
            }
            // główka
            if (Random.Range(1, 101) <= (30 + (attackerHeaders[y].Rating - defenderHeaders[y].Rating) * 10))
            {
                // zgubienie obrońcy i strzał głową
                if(_time > 0) yield return new WaitForSeconds(_time);
                CommentLine.Instance.FreeHeader();
                if(_time > 0) yield return new WaitForSeconds(_time);
                StartCoroutine(Shot(3));
            }
            else
            {
                if(_time > 0) yield return new WaitForSeconds(_time);
                if (Random.Range(1, 101) <= (50 + (attackerHeaders[y].Heading - defenderHeaders[y].Heading) * 10))
                {
                    CommentLine.Instance.ContestedHeader();
                    if(_time > 0) yield return new WaitForSeconds(_time);
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
            if(_time > 0) yield return new WaitForSeconds(_time);
            CommentLine.Instance.FailedCross();
            MinutePassed();
            StartCoroutine(CommentStart());
        }
    }
    IEnumerator AttackThirdPhase(string direction)
    {
        MinutePassed();
        if(_time > 0) yield return new WaitForSeconds(_time);
        if (direction == "left" || direction == "right")
        {
            int dir = direction == "right" ? 1 : 0;
            CommentLine.Instance.TryingToDodge();
            if(_time > 0) yield return new WaitForSeconds(_time);
            float plus = ((_teams[_guestBall][_playerWithBall].Dribling + _teams[_guestBall][_playerWithBall].Speed) - (_teams[GetReverseIsGuestBall()][_defWingPos[GetReverseIsGuestBall(), dir]].Tackle + _teams[GetReverseIsGuestBall()][_defWingPos[GetReverseIsGuestBall(), dir]].Speed)) * 3;
            if (Random.Range(1, 101) < 55 + plus)
            {
                int decision = Random.Range(1, 101);
                //dośrodkowanie bądź strzał
                if (decision <= 70)
                {
                    CommentLine.Instance.DecidesToCross();
                    float border = 40 + _teams[0][_playerWithBall].Pass * 4;           //----------------- ewentualnie zmniejszyc mnożnik gdyby za dużo goli z główki
                    int acc = Random.Range(1, 100);
                    if (acc <= border)
                    {
                        int attackerHeader;
                        do
                        {
                            attackerHeader = Random.Range(_midLastPlayerNumber[_guestBall] + 1, 11);
                        } while (attackerHeader == _playerWithBall);
                        int defenderHeader;
                        do
                        {
                            defenderHeader = Random.Range(1, _defLastPlayerNumber[_guestBall] + 1);
                        } while (defenderHeader == _defWingPos[GetReverseIsGuestBall(), dir]);

                        // główka
                        if (Random.Range(1, 101) <= (30 + (_teams[_guestBall][attackerHeader].Rating - _teams[GetReverseIsGuestBall()][defenderHeader].Rating) * 10))
                        {
                            // zgubienie obrońcy i stzrał głową
                            if(_time > 0) yield return new WaitForSeconds(_time);
                            _playerWithBall = attackerHeader;
                            CommentLine.Instance.FreeHeader();
                            if(_time > 0) yield return new WaitForSeconds(_time);
                            StartCoroutine(Shot(3));
                        }
                        else
                        {
                            if(_time > 0) yield return new WaitForSeconds(_time);
                            if (Random.Range(1, 101) <= (50 + (_teams[_guestBall][attackerHeader].Heading - _teams[GetReverseIsGuestBall()][defenderHeader].Heading) * 10))
                            {
                                _playerWithBall = attackerHeader;
                                CommentLine.Instance.ContestedHeader();
                                if(_time > 0) yield return new WaitForSeconds(_time);
                                StartCoroutine(Shot(3));
                            }
                            else
                            {
                                CommentLine.Instance.DefenderWinsHeader(_teams[GetReverseIsGuestBall()][defenderHeader]);
                                MinutePassed();
                                StartCoroutine(CommentStart());
                            }
                        }
                    }
                    else
                    {
                        if(_time > 0) yield return new WaitForSeconds(_time);
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
            if(_time > 0) yield return new WaitForSeconds(_time);
            int firstDef = _teamsDefPos[GetReverseIsGuestBall()][Random.Range(0, _teamsDefPos[GetReverseIsGuestBall()].Count)];
            float plus = ((_teams[_guestBall][_playerWithBall].Dribling + _teams[_guestBall][_playerWithBall].Speed) - (_teams[GetReverseIsGuestBall()][firstDef].Tackle + _teams[GetReverseIsGuestBall()][firstDef].Speed)) * 3;
            if (Random.Range(1, 101) < 45 + plus)
            {
                //podanie bądź strzał
                if (Random.Range(1, 101) <= 65)
                {
                    CommentLine.Instance.DecidesToPass();
                    float border = 30 + _teams[_guestBall][_playerWithBall].Pass * 2;
                    if (Random.Range(1, 101) <= border)
                    {
                        int atackerIndex;
                        do
                        {
                            atackerIndex = Random.Range(_midLastPlayerNumber[_guestBall] + 1, 11);
                        } 
                        while (atackerIndex == _playerWithBall);

                        int defenderIndex;
                        do
                        {
                            defenderIndex = _teamsDefPos[GetReverseIsGuestBall()][Random.Range(0, _teamsDefPos[GetReverseIsGuestBall()].Count)];
                        } 
                        while (defenderIndex == firstDef);

                        _playerWithBall = atackerIndex;
                        if(_time > 0) yield return new WaitForSeconds(_time);
                        CommentLine.Instance.ChanceForOneOnOne();
                        if(_time > 0) yield return new WaitForSeconds(_time);
                        if (Random.Range(1, 101) <= (50 + (_teams[_guestBall][atackerIndex].Dribling - _teams[GetReverseIsGuestBall()][defenderIndex].Tackle) * 10))
                        {
                            CommentLine.Instance.OneToOneSituation();
                            if(_time > 0) yield return new WaitForSeconds(_time);
                            StartCoroutine(Shot(2));
                        }
                        else
                        {
                            _playerWithBall = defenderIndex;
                            CommentLine.Instance.FailedChanceOneToOne();
                            MinutePassed();
                            StartCoroutine(CommentStart());
                        }
                        //}
                    }
                    else
                    {
                        if(_time > 0) yield return new WaitForSeconds(_time);
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
                CommentLine.Instance.FailedMidDribble(_teams[GetReverseIsGuestBall()][firstDef]);
                MinutePassed();
                StartCoroutine(CommentStart());
            }
        }
	}
    public int GetReverseIsGuestBall() => (_guestBall == 0) ? 1 : 0;
}
