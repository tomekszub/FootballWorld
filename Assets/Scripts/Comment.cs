using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class Comment : MonoBehaviour
{
    public static Comment Instance;

    [SerializeField] MyClub _MyClub;
    [SerializeField] TextMeshProUGUI _StartStopButton;

    float _time = 0.1f;
    bool _isPlaying = false;
    int _minute = 0;
    int hostId, guestId;
    List<Footballer>[] teams = new List<Footballer>[2];
    float[] teamDef = new float[2], teamMid = new float[2], teamAtk = new float[2];
    int hostChances, _goalChances;
    MatchStats[] matchStats = new MatchStats[2];
    string[] teamName = new string[2];
    int playerWithBall;  // piłkarz przy piłce
    int guestBall;  // posiadanie piłki
    int[] defLastPlayerNumber = new int[2];// granice pozycji piłkarzy: obrona , pomoc , atak
    int[] midLastPlayerNumber = new int[2];
    List<int>[] teamsMidPos = new List<int>[2];// Lista piłkarzy(pomocnicy of., napastnicy) bedacych w środku formacji
    List<int>[] teamsDefPos = new List<int>[2];// Lista piłkarzy(obrońcy ) bedacych w środku formacji
    int[,] wingPos = new int[2, 2]; //static int HostLeftPos, GuestLeftPos, HostRightPos, GuestRightPos;
    int[,] defWingPos = new int[2, 2]; //int HostDefLeftPos, GuestDefLeftPos, HostDefRightPos, GuestDefRightPos;
    int not = 0;
    float curiosity = 1;
    int weakness = 0;
    int changedCuriosity = 0;
    bool end = false;
    string competitionName = "";
    #region Getters
    public string GetGuestName() => teamName[1];
    public string GetHostName() => teamName[0];
    public int GetGuestID() => guestId;
    public int GetHostID() => hostId;
    public int GetMinute() => _minute;
    public List<Footballer>[] GetTeams() => teams;
    public List<int>[] GetMidPos() => teamsMidPos;
    public List<int>[] GetDefPos() => teamsDefPos;
    public MatchStats[] GetMatchStats() => matchStats;
    public int GetHostChances() => hostChances;
    public int GetGoalChances() => _goalChances;
    public int[] GetDefLastPlayerNumber() => defLastPlayerNumber;
    public int[] GetMidLastPlayerNumber() => defLastPlayerNumber;
    public int[,] GetWingPos() => wingPos;
    public int[,] GetDefWingPos() => defWingPos;
    public int GetGuestBall() => guestBall;
    public int SetGuestBall(int val) => guestBall = val;
    public int GetPlayerWithBall() => playerWithBall;
    public int SetPlayerWithBall(int val) => playerWithBall = val;
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
        competitionName = compName;
        hostId = hostID;
        guestId = guestID;
        PrepareNextMatch();
    }
    public void OnSimSpeedChanged(float val)
    {
        _time = val;
    }

    void PrepareNextMatch()
    {
        end = false;
        matchStats[0] = new MatchStats(new List<Scorer>());
        matchStats[1] = new MatchStats(new List<Scorer>());
        teamName[0] = Database.clubDB[hostId].Name;
        teamName[1] = Database.clubDB[guestId].Name;
       
        teams[0] = new List<Footballer>(11);
        teams[1] = new List<Footballer>(11);
        for (int i = 0; i < 11; i++)
        {
            teams[0].Add(Database.footballersDB[Database.clubDB[hostId].FootballersIDs[i]]);
            teams[0][i].AddStatistic(competitionName, Footballer.PlayerStatistics.StatName.MatchesPlayed);
            teams[1].Add(Database.footballersDB[Database.clubDB[guestId].FootballersIDs[i]]);
            teams[1][i].AddStatistic(competitionName, Footballer.PlayerStatistics.StatName.MatchesPlayed);
        }
        teamsMidPos[0] = new List<int>();
        teamsMidPos[1] = new List<int>();
        teamsDefPos[0] = new List<int>();
        teamsDefPos[1] = new List<int>();
        Clear();
        RecognizeFormation(Database.clubDB[hostId].Formation, true);
        RecognizeFormation(Database.clubDB[guestId].Formation, false);
        CalculateWeakness();
        _isPlaying = false;
        CommentLine.StartingSettings();
        _StartStopButton.text = "Rozpocznij mecz";
    }
    public void StartStopButtonClick()
    {
        if (_isPlaying == false)
        {
            StartCoroutine(CommentStart());
        }
        if (end)
        {
            _MyClub.ProcesssMatchStats(matchStats, competitionName);
            WindowsManager.Instance.ShowWindow("Club Menu");
        }
    }
    // główna pętla
    IEnumerator CommentStart()
    {
        UpdateChances();
        CommentLine.UpdateResult(matchStats);
        if (_minute >= 90)
        {
            yield return new WaitForSeconds(_time);
            CommentLine.EndOfTheMatch();
            end = true;
            _StartStopButton.text = "Zakończ mecz";
            Debug.Log("KONIEC MECZU!!!---Wynik: " + matchStats[0].GetGoals() + "-" + matchStats[1].GetGoals() + " " + Time.time);
            StopAllCoroutines();
        }
        _isPlaying = true;
        if (_minute == 0)
        {
            CommentLine.StartingComment();
            _minute++;
            yield return new WaitForSeconds(_time);
            CommentLine.StartOfTheMatch();
            _minute++;
        }
        if (_minute < 90 && _minute > 1)
        {
            int ch = Random.Range(1, 100);
            if (ch >= 90)
            {
                not = 0;
                yield return new WaitForSeconds(_time);
                CommentLine.InfoComment();
                _minute++;
                StartCoroutine(CommentStart());
            }
            if (ch > 5 + _goalChances && ch < 90)
            {
                not++;
                if (not > 8)
                {
                    yield return new WaitForSeconds(_time);
                    CommentLine.BoringPartOfTheMatch();
                    not = 0;
                }
                _minute++;
                StartCoroutine(CommentStart());
            }
            if (ch <= 5 + _goalChances)
            {
                not = 0;
                yield return new WaitForSeconds(_time);
                AttackFirstPhase();
                yield return new WaitForSeconds(_time);
                AttackSecondPhase();
            }
        }
    }
    void AttackFirstPhase()
    {
        int who = Random.Range(1, 100);

        if (who <= hostChances)
            guestBall = 0;
        else
            guestBall = 1;

        int pos = Random.Range(1, defLastPlayerNumber[guestBall] + 1);
        playerWithBall = pos;
        CommentLine.AttackFirstPhase();
    }
    void AttackSecondPhase()
    {

        int counterPos = Random.Range(midLastPlayerNumber[GetReverseIsGuestBall()] + 1, 11);
        float counterChance = teams[GetReverseIsGuestBall()][counterPos].Tackle - teams[guestBall][playerWithBall].Pass;
        counterChance = (counterChance + 10) / 2;
        int dec = Random.Range(1, 37 + (int)counterChance);
        if (dec <= 10)
        {
            // środek podanie
            playerWithBall = Random.Range(defLastPlayerNumber[guestBall] + 1, midLastPlayerNumber[guestBall] + 1);
            CommentLine.PassMiddle();
            playerWithBall = teamsMidPos[guestBall][Random.Range(0, teamsMidPos[guestBall].Count)];
            StartCoroutine(AttackThirdPhase("middle"));
        }
        else if (dec <= 34 && dec > 10)
        {
            int isRightWing = dec > 22 ? 0 : 1;

            int pos = Random.Range(defLastPlayerNumber[guestBall] + 1, midLastPlayerNumber[guestBall] + 1);
            playerWithBall = pos;

            CommentLine.PassToTheWing(isRightWing);

            if (wingPos[guestBall, isRightWing] != pos)
                playerWithBall = wingPos[guestBall, isRightWing];
            else
            {
                // jesli skrzydlowym jest pomocnik podajacy pilke
                // to na skrzydel pilke musi przejac obronca
                // jesli akcja na prawym skrzydel to ostatni obronca z listy (kolejnosc pilkarzy od lewej), jesli nie to piewszy 
                if (isRightWing == 1)
                    playerWithBall = defLastPlayerNumber[guestBall];
                else
                     playerWithBall = 1;
            }

            StartCoroutine(AttackThirdPhase(isRightWing == 0 ? "left" : "right"));
        }
        else if (dec > 34 && dec <= 37)
        {
            playerWithBall = Random.Range(defLastPlayerNumber[guestBall] + 1, midLastPlayerNumber[guestBall] + 1);
            // Strzał
            CommentLine.DecidesToShoot();
            StartCoroutine(Shot(10));
        }
        else if (dec > 37)
        {
            // Kontratak
            playerWithBall = counterPos;
            guestBall = GetReverseIsGuestBall();
            CommentLine.InterceptionAndCounter();
            StartCoroutine(CounterAttack());
        }
    }
    void RecognizeFormation(string formation, bool host)
    {
        int isGuest = host ? 0 : 1;
        if (formation == "4-3-3")
        {
            teamDef[isGuest] = (teams[isGuest][1].Rating + teams[isGuest][2].Rating + teams[isGuest][3].Rating + teams[isGuest][4].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 4) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 4;
            teamMid[isGuest] = (teams[isGuest][5].Rating + teams[isGuest][6].Rating + teams[isGuest][7].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 3) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 7;
            teamAtk[isGuest] = (teams[isGuest][8].Rating + teams[isGuest][9].Rating + teams[isGuest][10].Rating);
            teamAtk[isGuest] = (teamAtk[isGuest] / 3) + (teamAtk[isGuest] / 3);
            teamsMidPos[isGuest].Add(10);
            wingPos[isGuest, 0] = 8;
            wingPos[isGuest, 1] = 9;
        }
        else if (formation == "4-2-3-1" || formation == "4-4-1-1")
        {
            teamDef[isGuest] = (teams[isGuest][1].Rating + teams[isGuest][2].Rating + teams[isGuest][3].Rating + teams[isGuest][4].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 4) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 4;
            teamMid[isGuest] = (teams[isGuest][5].Rating + teams[isGuest][6].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 2) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 6;
            teamAtk[isGuest] = (teams[isGuest][7].Rating + teams[isGuest][8].Rating + teams[isGuest][9].Rating + teams[isGuest][10].Rating);
            teamAtk[isGuest] = (teamAtk[isGuest] / 4) + (teamAtk[isGuest] / 3);
            teamsMidPos[isGuest].Add(7);
            teamsMidPos[isGuest].Add(10);
            wingPos[isGuest, 0] = 8;
            wingPos[isGuest, 1] = 9;
        }
        else if (formation == "4-1-4-1")
        {
            teamDef[isGuest] = (teams[isGuest][1].Rating + teams[isGuest][2].Rating + teams[isGuest][3].Rating + teams[isGuest][4].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 4) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 4;
            teamMid[isGuest] = (teams[isGuest][5].Rating + teams[isGuest][6].Rating + teams[isGuest][7].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 3) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 6;
            teamAtk[isGuest] = (teams[isGuest][8].Rating + teams[isGuest][9].Rating + teams[isGuest][10].Rating);
            teamAtk[isGuest] = (teamAtk[isGuest] / 3) + (teamAtk[isGuest] / 3);
            teamsMidPos[isGuest].Add(6);
            teamsMidPos[isGuest].Add(7);
            teamsMidPos[isGuest].Add(10);
            wingPos[isGuest, 0] = 8;
            wingPos[isGuest, 1] = 9;
        }
        else if (formation == "4-3-1-2")
        {
            teamDef[isGuest] = (teams[isGuest][1].Rating + teams[isGuest][2].Rating + teams[isGuest][3].Rating + teams[isGuest][4].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 4) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 4;
            teamMid[isGuest] = (teams[isGuest][5].Rating + teams[isGuest][6].Rating + teams[isGuest][7].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 3) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 7;
            teamAtk[isGuest] = (teams[isGuest][8].Rating + teams[isGuest][9].Rating + teams[isGuest][10].Rating);
            teamAtk[isGuest] = (teamAtk[isGuest] / 3) + (teamAtk[isGuest] / 3);
            teamsMidPos[isGuest].Add(8);
            teamsMidPos[isGuest].Add(9);
            teamsMidPos[isGuest].Add(10);
            wingPos[isGuest, 0] = 6;
            wingPos[isGuest, 1] = 7;
        }
        else if (formation == "4-4-2")
        {
            teamDef[isGuest] = (teams[isGuest][1].Rating + teams[isGuest][2].Rating + teams[isGuest][3].Rating + teams[isGuest][4].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 4) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 4;
            teamMid[isGuest] = (teams[isGuest][5].Rating + teams[isGuest][6].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 2) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 6;
            teamAtk[isGuest] = (teams[isGuest][7].Rating + teams[isGuest][8].Rating + teams[isGuest][9].Rating + teams[isGuest][10].Rating);
            teamAtk[isGuest] = (teamAtk[isGuest] / 4) + (teamAtk[isGuest] / 3);
            teamsMidPos[isGuest].Add(9);
            teamsMidPos[isGuest].Add(10);
            wingPos[isGuest, 0] = 7;
            wingPos[isGuest, 1] = 8;
        }
        else if (formation == "5-3-2")
        {
            teamDef[isGuest] = (teams[isGuest][1].Rating + teams[isGuest][2].Rating + teams[isGuest][3].Rating + teams[isGuest][4].Rating + teams[isGuest][5].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 5) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 5;
            teamMid[isGuest] = (teams[isGuest][6].Rating + teams[isGuest][7].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 2) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 7;
            teamAtk[isGuest] = (teams[isGuest][8].Rating + teams[isGuest][9].Rating + teams[isGuest][10].Rating);
            teamAtk[isGuest] = (teamAtk[isGuest] / 3) + (teamAtk[isGuest] / 3);
            teamsMidPos[isGuest].Add(8);
            teamsMidPos[isGuest].Add(9);
            teamsMidPos[isGuest].Add(10);
            wingPos[isGuest, 0] = 6;
            wingPos[isGuest, 1] = 7;
        }
        else if (formation == "3-4-1-2" || formation == "3-4-2-1")
        {
            teamDef[isGuest] = (teams[isGuest][1].Rating + teams[isGuest][2].Rating + teams[isGuest][3].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 3) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 3;
            teamMid[isGuest] = (teams[isGuest][4].Rating + teams[isGuest][5].Rating + teams[isGuest][6].Rating + teams[isGuest][7].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 4) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 7;
            teamAtk[isGuest] = (teams[isGuest][8].Rating + teams[isGuest][9].Rating + teams[isGuest][10].Rating);
            teamAtk[isGuest] = (teamAtk[isGuest] / 3) + (teamAtk[isGuest] / 3);
            teamsMidPos[isGuest].Add(8);
            teamsMidPos[isGuest].Add(9);
            teamsMidPos[isGuest].Add(10);
            wingPos[isGuest, 0] = 6;
            wingPos[isGuest, 1] = 7;
        }
        if (defLastPlayerNumber[isGuest] == 5)
        {
            teamsDefPos[isGuest].Add(2);
            teamsDefPos[isGuest].Add(3);
            teamsDefPos[isGuest].Add(4);
            defWingPos[isGuest, 0] = 1;
            defWingPos[isGuest, 1] = 5;
        }
        if (defLastPlayerNumber[isGuest] == 4)
        {
            teamsDefPos[isGuest].Add(2);
            teamsDefPos[isGuest].Add(3);
            defWingPos[isGuest, 0] = 1;
            defWingPos[isGuest, 1] = 4;
        }
        if (defLastPlayerNumber[isGuest] == 3)
        {
            teamsDefPos[isGuest].Add(1);
            teamsDefPos[isGuest].Add(2);
            teamsDefPos[isGuest].Add(3);
            defWingPos[isGuest, 0] = 1;
            defWingPos[isGuest, 1] = 3;
        }

    }
    void UpdateChances()
    {
        hostChances = 50;
        // difference between host's attack and guest's defense
        float advantage = (teamAtk[0] - teamDef[1]) * 2;
        advantage += (teamDef[0] - teamAtk[1]) * 2;
        advantage += (teamMid[0] - teamMid[1]) * 2;
        hostChances += 4 + Mathf.RoundToInt(advantage);
      
        hostChances -= weakness;

        if (hostChances >= 160)
            hostChances = 95;
        else if (hostChances >= 90)
            hostChances = 90;
        else if (hostChances <= -160)
            hostChances = 5;
        else if (hostChances < 10)
            hostChances = 10;

        float ratingDiff = Mathf.Abs(Database.clubDB[hostId].Rate - Database.clubDB[guestId].Rate);
        float goalChances = (5 + (ratingDiff * 3)) / curiosity + changedCuriosity;
        _goalChances = Mathf.RoundToInt(goalChances);
        _goalChances = Mathf.Clamp(_goalChances, 4, 35);
    }
    void CalculateWeakness()
    {
        weakness = 0;
        int rnd = Random.Range(1, 101);
        if (rnd <= 10)
        {
            int ran = Random.Range(0, (Database.clubDB[hostId].Rate * 10 + Database.clubDB[guestId].Rate * 10));
            if (Database.clubDB[hostId].Rate * 10 > ran)
            {
                weakness = 10;
                if (Database.clubDB[hostId].Rate > Database.clubDB[guestId].Rate)
                {
                    curiosity = 2;
                }
                if (Database.clubDB[hostId].Rate < Database.clubDB[guestId].Rate)
                {
                    curiosity = 0.5f;
                }
                if (Database.clubDB[hostId].Rate == Database.clubDB[guestId].Rate)
                {
                    curiosity = 3;
                }
            }
            else
            {
                weakness = -10;
                if (Database.clubDB[hostId].Rate > Database.clubDB[guestId].Rate)
                {
                    curiosity = 0.5f;
                }
                if (Database.clubDB[hostId].Rate < Database.clubDB[guestId].Rate)
                {
                    curiosity = 2;
                }
                if (Database.clubDB[hostId].Rate == Database.clubDB[guestId].Rate)
                {
                    curiosity = 3;
                }
            }
        }
    }
    void Clear()
    {
        teamsMidPos[0].Clear();
        teamsMidPos[1].Clear();
        teamsDefPos[0].Clear();
        teamsDefPos[1].Clear();
        matchStats[0].Reset();
        matchStats[1].Reset();
        _minute = 0;
    }
    void ChangeCuriosity()
    {
        // conversion from 0 , 1 to -1,1
        int multiplier = guestBall * 2 - 1;

        if (Database.clubDB[hostId].Rate > Database.clubDB[guestId].Rate)
        {
            changedCuriosity += 5 * multiplier;
        }
        else if (Database.clubDB[hostId].Rate < Database.clubDB[guestId].Rate)
        {
            changedCuriosity += -5 * multiplier;
        }
        else
        {
            changedCuriosity -= 2;
        }
    }
    IEnumerator FreeKick(bool isPenalty)
    {
        yield return new WaitForSeconds(_time);
        if (isPenalty)
        {
            _minute++;
            int[] penaltyPlayers = new int[10];
            for (int i = 1; i < 11; i++)
            {
                penaltyPlayers[i] = i;
            }
            penaltyPlayers = penaltyPlayers.OrderByDescending(x => teams[guestBall][x].Penalty).ToArray();
            playerWithBall = penaltyPlayers[0];
            CommentLine.PreparingToPenalty();
            _minute++;
            yield return new WaitForSeconds(_time);
            float plus = teams[guestBall][playerWithBall].Penalty - (1.5f * teams[GetReverseIsGuestBall()][0].Rating);
            int rnd = Random.Range(1, 101);
            if (rnd < 80 + plus)
            {
                // gol
                GoalScored();
                CommentLine.PenaltyGoal();
            }
            else
            {
                CommentLine.PenaltyMissed();
            }
            matchStats[guestBall].ShotTaken();
            _minute++;
            StartCoroutine(CommentStart());
        }
        else
        {

        }
    }
    IEnumerator Shot(int difficulty)
    {
        yield return new WaitForSeconds(_time);
        if (difficulty == 10)
        {
            // strzał z bardzo daleka
            matchStats[guestBall].ShotTaken();
            _minute++;
            int x = Random.Range(1, 101);
            float plus = (teams[guestBall][playerWithBall].Shoot * 2) - teams[GetReverseIsGuestBall()][0].Rating;
            if (x <= 20)
            {
                // rożny
                CommentLine.LongShotCorner();
                StartCoroutine(Corner());
            }
            else
            {
                if (x <= 30 + plus)
                {
                    GoalScored();
                    CommentLine.LongShotGoal();
                }
                else
                    CommentLine.LongShotMiss();

                _minute++;
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 8)
        {
            // strzal z okolo 20 metrow przy kontrataku
            matchStats[guestBall].ShotTaken();
            _minute++;
            float keeperPlus = 20 + teams[GetReverseIsGuestBall()][0].Rating * 3;
            float shooterPlus = 20 + teams[0][playerWithBall].Shoot * 2;
            int x = Random.Range(1, (int)keeperPlus + (int)shooterPlus + 10);
            if (x <= keeperPlus)
            {
                if (Random.Range(1, 31) <= teams[GetReverseIsGuestBall()][0].Rating)  // maks 10/30 szansy ze zlapie
                {
                    //lapie
                    CommentLine.CounterAttackSave();
                    _minute++;
                    StartCoroutine(CommentStart());
                }
                else
                {
                    //korner
                    CommentLine.CounterAttackCorner();
                    StartCoroutine(Corner());
                }
            }
            else
            {
                if (x <= keeperPlus + shooterPlus)
                {
                    GoalScored();
                    CommentLine.CounterAttackGoal();
                }
                else
                {
                    CommentLine.CounterAttackMiss();
                }
                _minute++;
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 5)  // okolo 16 metrow normalny atak
        {
            matchStats[guestBall].ShotTaken();
            _minute++;
            float keeperPlus = 5 + teams[GetReverseIsGuestBall()][0].Rating * 2;
            float shooterPlus = 15 + teams[guestBall][playerWithBall].Shoot * 4;
            int x = Random.Range(1, (int)keeperPlus + (int)shooterPlus + 15);
            if (x <= keeperPlus)
            {
                // obrona
                int saveType = Random.Range(1, 51);
                if (saveType <= teams[GetReverseIsGuestBall()][0].Rating)  // maks 10/50 szansy ze zlapie
                {
                    //lapie
                    CommentLine.NormalAttackSave();
                    _minute++;
                    StartCoroutine(CommentStart());
                }
                else
                {
                    //korner
                    CommentLine.NormalAttackCorner();
                    StartCoroutine(Corner());
                }
            }
            else if (x > keeperPlus && x <= keeperPlus + shooterPlus)
            {
                // goool
                GoalScored();
                CommentLine.NormalAttackGoal();
                _minute++;
                StartCoroutine(CommentStart());
            }
            else if (x > keeperPlus + shooterPlus)
            {
                //pudło
                CommentLine.NormalAttackMiss();
                _minute++;
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 2)    //   Sam na sam
        {
            matchStats[guestBall].ShotTaken();
            _minute++;
            float keeperPlus = 10 + teams[GetReverseIsGuestBall()][0].Rating * 2;
            float shooterPlus = 25 + teams[guestBall][playerWithBall].Shoot * 5;
            int x = Random.Range(1, (int)keeperPlus + (int)shooterPlus + 5);
            if (x <= keeperPlus)
            {
                int saveType = Random.Range(1, 51);
                if (saveType <= teams[GetReverseIsGuestBall()][0].Rating)  // maks 10/50 szansy ze zlapie albo nie bedzie roznego
                {
                    //lapie
                    CommentLine.OneOnOneAttackSave();
                    _minute++;
                    StartCoroutine(CommentStart());
                }
                else
                {
                    //korner
                    CommentLine.OneOnOneAttackCorner();
                    StartCoroutine(Corner());
                }
            }
            else if (x > keeperPlus && x <= keeperPlus + shooterPlus)
            {
                // goool
                GoalScored();
                CommentLine.OneOnOneAttackGoal();
                _minute++;
                StartCoroutine(CommentStart());
            }
            else if (x > keeperPlus + shooterPlus)
            {
                CommentLine.OneOnOneAttackMiss();
                _minute++;
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 3)    //   główka
        {
            matchStats[guestBall].ShotTaken();
            _minute++;
            float keeperPlus = 5 + teams[GetReverseIsGuestBall()][0].Rating * 2;
            float shooterPlus = 10 + teams[guestBall][playerWithBall].Heading * 5;
            int x = Random.Range(1, (int)keeperPlus + (int)shooterPlus + 10);
            //float plus = (teams[0][playerWithBall].Shoot * 3) - (int)teams[1][0].Rate;
            if (x <= keeperPlus)
            {
                int saveType = Random.Range(1, 31);
                if (saveType <= teams[GetReverseIsGuestBall()][0].Rating)  // maks 10/30 szansy ze zlapie albo nie bedzie roznego
                {
                    //lapie
                    CommentLine.HeaderSave();
                    _minute++;
                    StartCoroutine(CommentStart());
                }
                else
                {
                    //korner
                    CommentLine.HeaderCorner();
                    StartCoroutine(Corner());
                }

            }
            else if (x > keeperPlus && x <= keeperPlus + shooterPlus)
            {
                // goool
                GoalScored();
                CommentLine.HeaderGoal();
                _minute++;
                StartCoroutine(CommentStart());
            }
            else if (x > keeperPlus + shooterPlus)
            {
                CommentLine.HeaderMiss();
                _minute++;
                StartCoroutine(CommentStart());
            }
        }
    }

    private void GoalScored()
    {
        matchStats[guestBall].GoalScored();
        matchStats[guestBall].AddScorer(teams[guestBall][playerWithBall], teamName[guestBall], 1);
        teams[guestBall][playerWithBall].AddStatistic(competitionName, Footballer.PlayerStatistics.StatName.Goals);
        ChangeCuriosity();
    }

    IEnumerator CounterAttack()
    {
        yield return new WaitForSeconds(_time);
        int rnd = Random.Range(1, 100);
        float x = 20 - teams[guestBall][playerWithBall].Pass;
        if (rnd <= 30)
        {
            CommentLine.CounterAttackShotTry();
            yield return new WaitForSeconds(_time);
            StartCoroutine(Shot(8));
        }
        else if (rnd > 30 && rnd <= 30 + x)
        {
            CommentLine.CounterAttackFailedPass();
            _minute++;
            StartCoroutine(CommentStart());
        }
        else if (rnd > 30 + x)
        {
            transform.Find("Text").GetComponent<Text>().text += "\n" + teams[guestBall][playerWithBall].Surname + " podaje do ";
            int newPos;
            do {
                newPos = Random.Range(midLastPlayerNumber[guestBall] + 1, 11);
            } while (playerWithBall == newPos);
            playerWithBall = newPos;
            transform.Find("Text").GetComponent<Text>().text += teams[guestBall][newPos].AlteredSurname + ", ten ma przed soba tylko bramkarza.";
            yield return new WaitForSeconds(_time);
            int chan = Random.Range(1, 101);
            if (chan < 70)
            {
                CommentLine.CounterAttackPenaltyFoul();
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
        yield return new WaitForSeconds(_time);
        List<Footballer> cornerPlayers = new List<Footballer>();
        for (int i = 1; i < 11; i++)
        {
            cornerPlayers.Add(teams[guestBall][i]);
        }
        cornerPlayers = cornerPlayers.OrderByDescending(x => x.Corner).ToList();
        CommentLine.CornerExecution(cornerPlayers[0]);
        float border = 65 + cornerPlayers[0].FreeKicks + cornerPlayers[0].Pass;
        int acc = Random.Range(1, 100);
        if (acc <= border)
        {
            List<Footballer> attackerHeaders = new List<Footballer>();
            List<Footballer> defenderHeaders = new List<Footballer>();
            attackerHeaders = cornerPlayers.OrderByDescending(x => x.Heading).ToList();
            for (int i = 1; i < 11; i++)
            {
                defenderHeaders.Add(teams[1][i]);
            }
            int y = Random.Range(0, 30); // losowanie numeru piłkarzy, którzy będą walczyć o piłkę
            y = y / 10;
            for (int i = 1; i < 11; i++)
            {
                if (attackerHeaders[y].Surname == teams[guestBall][i].Surname && attackerHeaders[y].Name == teams[guestBall][i].Name)
                {
                    playerWithBall = i;
                    break;
                }
            }
            int rnd = Random.Range(1, 101);
            // główka
            if (rnd <= (30 + (attackerHeaders[y].Rating - defenderHeaders[y].Rating) * 10))
            {
                // zgubienie obrońcy i strzał głową
                yield return new WaitForSeconds(_time);
                CommentLine.FreeHeader();
                yield return new WaitForSeconds(_time);
                StartCoroutine(Shot(3));
            }
            else
            {
                yield return new WaitForSeconds(_time);
                int ran = Random.Range(1, 101);
                if (ran <= (50 + (attackerHeaders[y].Heading - defenderHeaders[y].Heading) * 10))
                {
                    CommentLine.ContestedHeader();
                    yield return new WaitForSeconds(_time);
                    StartCoroutine(Shot(3));
                }
                else
                {
                    CommentLine.DefenderWinsHeader(defenderHeaders[y]);
                    _minute++;
                    StartCoroutine(CommentStart());
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(_time);
            CommentLine.FailedCross();
            _minute++;
            StartCoroutine(CommentStart());
        }
    }
    IEnumerator AttackThirdPhase(string direction)
    {
        _minute++;
        yield return new WaitForSeconds(_time);
        if (direction == "left" || direction == "right")
        {
            int dir = 0;
            if (direction == "right") dir = 1;
            CommentLine.TryingToDodge();
            yield return new WaitForSeconds(_time);
            int rnd = Random.Range(1, 101);
            float plus = ((teams[guestBall][playerWithBall].Dribling + teams[guestBall][playerWithBall].Speed) - (teams[GetReverseIsGuestBall()][defWingPos[GetReverseIsGuestBall(), dir]].Tackle + teams[GetReverseIsGuestBall()][defWingPos[GetReverseIsGuestBall(), dir]].Speed)) * 3;
            if (rnd < 55 + plus)
            {
                int decision = Random.Range(1, 101);
                //dośrodkowanie bądź strzał
                if (decision <= 70)
                {
                    CommentLine.DecidesToCross();
                    float border = 40 + teams[0][playerWithBall].Pass * 4;           //----------------- ewentualnie zmniejszyc mnożnik gdyby za dużo goli z główki
                    int acc = Random.Range(1, 100);
                    if (acc <= border)
                    {
                        int attackerHeader;
                        do
                        {
                            attackerHeader = Random.Range(midLastPlayerNumber[guestBall] + 1, 11);
                        } while (attackerHeader == playerWithBall);
                        int defenderHeader;
                        do
                        {
                            defenderHeader = Random.Range(1, defLastPlayerNumber[guestBall] + 1);
                        } while (defenderHeader == defWingPos[GetReverseIsGuestBall(), dir]);

                        int abc = Random.Range(1, 101);
                        // główka
                        if (abc <= (30 + (teams[guestBall][attackerHeader].Rating - teams[GetReverseIsGuestBall()][defenderHeader].Rating) * 10))
                        {
                            // zgubienie obrońcy i stzrał głową
                            yield return new WaitForSeconds(_time);
                            playerWithBall = attackerHeader;
                            CommentLine.FreeHeader();
                            yield return new WaitForSeconds(_time);
                            StartCoroutine(Shot(3));
                        }
                        else
                        {
                            yield return new WaitForSeconds(_time);
                            int ran = Random.Range(1, 101);
                            if (ran <= (50 + (teams[guestBall][attackerHeader].Heading - teams[GetReverseIsGuestBall()][defenderHeader].Heading) * 10))
                            {
                                playerWithBall = attackerHeader;
                                CommentLine.ContestedHeader();
                                yield return new WaitForSeconds(_time);
                                StartCoroutine(Shot(3));
                            }
                            else
                            {
                                CommentLine.DefenderWinsHeader(teams[GetReverseIsGuestBall()][defenderHeader]);
                                _minute++;
                                StartCoroutine(CommentStart());
                            }
                        }
                    }
                    else
                    {
                        yield return new WaitForSeconds(_time);
                        CommentLine.FailedCross();
                        _minute++;
                        StartCoroutine(CommentStart());
                    }
                }
                else
                {
                    CommentLine.DecidesToShootInsteadOfCrossing();
                    StartCoroutine(Shot(5));
                }
            }
            else
            {
                CommentLine.FailedWingDribble(dir);
                _minute++;
                StartCoroutine(CommentStart());
            }
        }
        else if (direction == "middle")
        {

            CommentLine.TryingToDodge();
            yield return new WaitForSeconds(_time);
            //Debug.Log(teamsDefPos[GetReverseIsGuestBall()].Count);
            int firstDef = teamsDefPos[GetReverseIsGuestBall()][Random.Range(0, teamsDefPos[GetReverseIsGuestBall()].Count)];
            int rnd = Random.Range(1, 101);
            float plus = ((teams[guestBall][playerWithBall].Dribling + teams[guestBall][playerWithBall].Speed) - (teams[GetReverseIsGuestBall()][firstDef].Tackle + teams[GetReverseIsGuestBall()][firstDef].Speed)) * 3;
            if (rnd < 45 + plus)
            {
                int decision = Random.Range(1, 101);
                //podanie bądź strzał
                if (decision <= 65)
                {
                    CommentLine.DecidesToPass();
                    float border = 30 + teams[guestBall][playerWithBall].Pass * 2;           //-------------------- ewentualnei zmniejszyc mnożnik gdyby za dużo goli z główki
                    int acc = Random.Range(1, 100);
                    if (acc <= border)
                    {
                        int atacker;
                        do
                        {
                            atacker = Random.Range(midLastPlayerNumber[guestBall] + 1, 11);
                        } while (atacker == playerWithBall);
                        int defender;
                        do
                        {
                            defender = teamsDefPos[GetReverseIsGuestBall()][Random.Range(0, teamsDefPos[GetReverseIsGuestBall()].Count)];
                        } while (defender == firstDef);

                        /*int abc = Random.Range(1, 101);
                        
                        if (abc <= (20 + (teams[guestBall][hostAtacker].Rate - teams[GetReverseIsGuestBall()][teamDef[1]fender].Rate) * 10))
                        {
                            // zgubienie obrońcy i strzał 
                            yield return new WaitForSeconds(time);
                            this.transform.Find("Text").GetComponent<Text>().text += "\n" + teams[0][hostAtacker].Surname + " gubi obrońcę, przyjmuje piłkę i strzela...";
                            yield return new WaitForSeconds(time);
                            playerWithBall = hostAtacker;
                            StartCoroutine(Shot(2));
                        }
                        else
                        {*/
                        playerWithBall = atacker;
                        yield return new WaitForSeconds(_time);
                        CommentLine.ChanceForOneOnOne();
                        yield return new WaitForSeconds(_time);
                        int ran = Random.Range(1, 101);
                        if (ran <= (50 + (teams[guestBall][atacker].Dribling - teams[GetReverseIsGuestBall()][defender].Tackle) * 10))
                        {
                            CommentLine.OneToOneSituation();
                            yield return new WaitForSeconds(_time);
                            StartCoroutine(Shot(2));
                        }
                        else
                        {
                            playerWithBall = defender;
                            CommentLine.FailedChanceOneToOne();
                            _minute++;
                            StartCoroutine(CommentStart());
                        }
                        //}
                    }
                    else
                    {
                        yield return new WaitForSeconds(_time);
                        CommentLine.FailedPass();
                        _minute++;
                        StartCoroutine(CommentStart());
                    }
                }
                else
                {
                    CommentLine.DecidesToShootInsteadOfPassing();
                    StartCoroutine(Shot(5));
                }
            }
            else
            {
                CommentLine.FailedMidDribble(teams[GetReverseIsGuestBall()][firstDef]);
                _minute++;
                StartCoroutine(CommentStart());
            }
        }
	}
    public int GetReverseIsGuestBall() => (guestBall == 0) ? 1 : 0;
}
