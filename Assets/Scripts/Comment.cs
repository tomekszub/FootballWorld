using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Comment : MonoBehaviour
{
    public static Comment instance;
    // test z dodawaniem pkt do tablicy
    //[SerializeField]
    Table tableScript;
    public GameObject tableObj;
    public GameObject commentPanel;
    //
    public MyClub mc;
    public GameObject start_stop_button;
    public GameObject menuGame;
    public static bool coomment_or_teams_Test = false;
    public static float time = 0.1f;
    //public GameObject resultText;
    public static bool isPlaying = false;
    static int minute = 0;
    public static int hostId, guestId;
    //static int hostId, guestId, hostStartFootballer, guestStartFootballer; // id zespołów i id piłkarzy uczestniczącyh
    static List<Footballer>[] teams = new List<Footballer>[2];
    static float[] teamDef = new float[2], teamMid = new float[2], teamAtk = new float[2];
    static int hostChances, goalChances;  // szanse gospodarzy na akcje i akcje bramkowe w meczu
    static MatchStats[] matchStats = new MatchStats[2];//static int hostGoals, guestGoals, hostShots, guestShots
    static string[] teamName = new string[2];
    public static int playerWithBall;  // piłkarz przy piłce
    public static int guestBall;  // posiadanie piłki
    static int[] defLastPlayerNumber = new int[2];// granice pozycji piłkarzy: obrona , pomoc , atak
    static int[] midLastPlayerNumber = new int[2];
    static List<int>[] teamsMidPos = new List<int>[2];// Lista piłkarzy(pomocnicy of., napastnicy) bedacych w środku formacji
    static List<int>[] teamsDefPos = new List<int>[2];// Lista piłkarzy(obrońcy ) bedacych w środku formacji
    static int[,] wingPos = new int[2, 2]; //static int HostLeftPos, GuestLeftPos, HostRightPos, GuestRightPos;
    static int[,] defWingPos = new int[2, 2]; //int HostDefLeftPos, GuestDefLeftPos, HostDefRightPos, GuestDefRightPos;
    static int not = 0;
    static float curiosity = 1;
    static int weakness = 0;
    static int changedCuriosity = 0;
    static bool end = false;
    static string competitionName = "";
    #region Geters
    public static string GetGuestName()
    {
        return teamName[1];
    }
    public static string GetHostName()
    {
        return teamName[0];
    }
    public static int GetMinute()
    {
        return minute;
    }
    public static List<Footballer>[] GetTeams()
    {
        return teams;
    }
    public static List<int>[] GetMidPos()
    {
        return teamsMidPos;
    }
    public static List<int>[] GetDefPos()
    {
        return teamsDefPos;
    }
    public static MatchStats[] GetMatchStats()
    {
        return matchStats;
    }
    public static int GetHostChances()
    {
        return hostChances;
    }
    public static int GetGoalChances()
    {
        return goalChances;
    }
    public static int[] GetDefLastPlayerNumber()
    {
        return defLastPlayerNumber;
    }
    public static int[] GetMidLastPlayerNumber()
    {
        return defLastPlayerNumber;
    }
    public static int[,] GetWingPos()
    {
        return wingPos;
    }
    public static int[,] GetDefWingPos()
    {
        return defWingPos;
    }
    #endregion
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        tableScript = tableObj.GetComponent<Table>();
    }
    // Use this for initialization, 
    public void Init(int hostID, int guestID, string compName, bool test=false)
    {
        coomment_or_teams_Test = test;
        competitionName = compName;
        hostId = hostID;
        guestId = guestID;
        PrepareNextMatch();
    }
    public static void SetIsPlaying()
    {
        isPlaying = false;
    }

    public void OnSimSpeedChanged(float val)
    {
        time = val;
    }

    public void PrepareNextMatch()
    {
        
        end = false;
        matchStats[0] = new MatchStats(new List<Scorer>());
        matchStats[1] = new MatchStats(new List<Scorer>());
        //bool hostFlag = false;
        //bool guestFlag = false;
        teamName[0] = Database.clubDB[hostId].Name;
        teamName[1] = Database.clubDB[guestId].Name;
        /*
        for (int i = 0; i < Database.clubDB.Count; i++)
        {
            if (!hostFlag && Database.clubDB[i].Id == hostId)
            {
                hostFlag = true;
                hostId = i;
                teamName[0] = Database.clubDB[i].Name;
                for (int x = 0; x < Database.footballersDB.Count; x++)
                {
                    if (Database.footballersDB[x].Id == Database.clubDB[i].StartingFootballerId)
                    {
                        hostStartFootballer = x;
                    }
                }
            }
            if (!guestFlag && Database.clubDB[i].Id == guestId)
            {
                guestFlag = true;
                guestId = i;
                teamName[1] = Database.clubDB[i].Name;
                for (int x = 0; x < Database.footballersDB.Count; x++)
                {
                    if (Database.footballersDB[x].Id == Database.clubDB[i].StartingFootballerId)
                    {
                        guestStartFootballer = x;
                    }
                }
            }
            if (hostFlag && guestFlag)
                break;
        }*/
        teams[0] = new List<Footballer>();
        teams[1] = new List<Footballer>();
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
        ClearLists();
        RecognizeFormation(Database.clubDB[hostId].Formation, true);
        RecognizeFormation(Database.clubDB[guestId].Formation, false);
        CalculateChances();
        //print (hostName + "------");
        //resultText.transform.GetComponent<Text> ().text = hostName + "  " + matchStats[0].GetGoals() + " - " + minute + " - " + matchStats[1ś.GetGoals() + "  " + guestName;
        SetIsPlaying();
        CommentLine.StartingSettings();
        start_stop_button.GetComponentInChildren<Text>().text = "Rozpocznij mecz";
        //Debug.Log("START " + Time.time);
    }
    public void StartMatch()
    {
        //Debug.Log(isPlaying);
        if (isPlaying == false)
        {
            //ResetStats ();
            StartCoroutine(CommentStart());
        }
        if (end)
        {
            mc.ProcesssMatchStats(matchStats, competitionName);
            WindowsManager.Instance.ShowWindow("Club Menu");
        }
    }
    // główna pętla
    IEnumerator CommentStart()
    {
        UpdateChances();
        CommentLine.UpdateResult(matchStats);
        //resultText.transform.GetComponent<Text>().text = hostName + "  " + matchStats[0].GetGoals() + " - " + matchStats[1].GetGoals() + "  " + guestName;
        if (minute >= 90)
        {
            yield return new WaitForSeconds(time);
            CommentLine.EndOfTheMatch();
            // KONIEC
            if (!coomment_or_teams_Test)
            {
                end = true;
                start_stop_button.GetComponentInChildren<Text>().text = "Zakończ mecz";
                Debug.Log("KONIEC MECZU!!!---Wynik: " + matchStats[0].GetGoals() + "-" + matchStats[1].GetGoals() + " " + Time.time);
                StopAllCoroutines();
                //commentPanel.SetActive(false);
            }
        }
        isPlaying = true;
        if (minute == 0)
        {
            CommentLine.StartingComment();
            minute++;
            yield return new WaitForSeconds(time);
            CommentLine.StartOfTheMatch();
            minute++;
        }
        if (minute < 90 && minute > 1)
        {
            int ch = Random.Range(1, 100);
            if (ch >= 90)
            {
                not = 0;
                yield return new WaitForSeconds(time);
                CommentLine.InfoComment();
                minute++;
                StartCoroutine(CommentStart());
            }
            if (ch > 5 + goalChances && ch < 90)
            {
                not++;
                if (not > 8)
                {
                    yield return new WaitForSeconds(time);
                    CommentLine.BoringPartOfTheMatch();
                    not = 0;
                }
                minute++;
                StartCoroutine(CommentStart());
            }
            if (ch <= 5 + goalChances)
            {
                not = 0;
                yield return new WaitForSeconds(time);
                CommentLine.AttackFirstPhase();
                yield return new WaitForSeconds(time);
                AttackSecondPhase();
            }
        }
    }
    void AttackSecondPhase()
    {

        int counterPos = Random.Range(midLastPlayerNumber[GetReverseIsGuestBall()] + 1, 11);
        float counterChance = teams[GetReverseIsGuestBall()][counterPos].Tackle - teams[guestBall][playerWithBall].Pass;
        counterChance += 10;
        counterChance /= 2;
        int dec = Random.Range(1, 37 + (int)counterChance);
        if (dec <= 10)
        {
            // środek podanie
            CommentLine.PassMiddle();
            StartCoroutine(AttackThirdPhase("middle"));
        }
        else if (dec <= 22 && dec > 10)
        {
            // Prawo podanie
            CommentLine.PassToTheWing(true);
            StartCoroutine(AttackThirdPhase("right"));
        }
        else if (dec > 22 && dec <= 34)
        {
            // Lewo podanie
            CommentLine.PassToTheWing(false);
            StartCoroutine(AttackThirdPhase("left"));
        }
        else if (dec > 34 && dec <= 37)
        {
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
        float advantage;
        hostChances = 50;
        // difference between host's attack and guest's defense
        advantage = (teamAtk[0] - teamDef[1]) * 2;
        hostChances += 4 + Mathf.RoundToInt(advantage);
        advantage = (teamDef[0] - teamAtk[1]) * 2;
        hostChances += Mathf.RoundToInt(advantage);
        advantage = (teamMid[0] - teamMid[1]) * 2;
        hostChances += Mathf.RoundToInt(advantage);
      
        hostChances = hostChances - weakness;
        if (hostChances > 90 && hostChances < 160)
            hostChances = 90;
        if (hostChances >= 160)
            hostChances = 95;
        if (hostChances < 10 && hostChances > -60)
            hostChances = 10;
        if (hostChances <= -160)
            hostChances = 5;
        float GoalChances = (5 + (Mathf.Abs(Database.clubDB[hostId].Rate - Database.clubDB[guestId].Rate)) * 3) / curiosity + changedCuriosity;
        goalChances = Mathf.RoundToInt(GoalChances);
        if (goalChances > 35)
        {
            goalChances = 35;
        }
        if (goalChances < 4)
        {
            goalChances = 4;
        }
        //print("CHANCE UPDATE: Minute: " + minute + ". Goal: " + goalChances + " Host: " + hostChances);
    }
    void CalculateChances()
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
        float advantage;
        hostChances = 50;
        //guestChances = 0;
        if (teamAtk[0] > teamDef[1])
        {
            advantage = (teamAtk[0] - teamDef[1]) * 2;
            //print (Mathf.RoundToInt(advantage));
            hostChances += 4 + Mathf.RoundToInt(advantage);
        }
        else
        {
            advantage = (teamDef[1] - teamAtk[0]) * 2;
            hostChances += 4 - Mathf.RoundToInt(advantage);
        }
        print("1. " + hostChances);
        if (teamAtk[1] > teamDef[0])
        {
            advantage = (teamAtk[1] - teamDef[0]) * 2;
            hostChances -= Mathf.RoundToInt(advantage);
        }
        else
        {
            advantage = (teamDef[0] - teamAtk[1]) * 2;
            hostChances += Mathf.RoundToInt(advantage);
        }
        print("2. " + hostChances);
        if (teamMid[0] > teamMid[1])
        {
            advantage = (teamMid[0] - teamMid[1]) * 2;
            hostChances += Mathf.RoundToInt(advantage);
        }
        else
        {
            advantage = (teamMid[1] - teamMid[0]) * 2;
            hostChances -= Mathf.RoundToInt(advantage);
        }
        print("3. Przed weaknessem : " + hostChances + "teamMid[0] - teamMid[1]" + teamMid[0] + " " + teamMid[1]);
        hostChances = hostChances - weakness;
        if (hostChances > 90 && hostChances < 160)
            hostChances = 90;
        if (hostChances >= 160)
            hostChances = 95;
        if (hostChances < 10 && hostChances > -60)
            hostChances = 10;
        if (hostChances <= -160)
            hostChances = 5;
        float GoalChances = (5 + (Mathf.Abs(Database.clubDB[hostId].Rate - Database.clubDB[guestId].Rate)) * 3) / curiosity;
        goalChances = Mathf.RoundToInt(GoalChances);
        //print (GoalChances + ", " + goalChances + " hostCh - " + hostChances);
    }
    void ClearLists()
    {
        //if(transform.Find("Text") != null) transform.Find("Text").GetComponent<Text>().text = "";
        teamsMidPos[0].Clear();
        teamsMidPos[1].Clear();
        teamsDefPos[0].Clear();
        teamsDefPos[1].Clear();
        matchStats[0].Reset();
        matchStats[1].Reset();
        minute = 0;
    }
    void ChangeCuriosity()
    {
        
        if (!System.Convert.ToBoolean(guestBall))
        {
            if (Database.clubDB[hostId].Rate > Database.clubDB[guestId].Rate)
            {
                changedCuriosity += 5;
            }
            if (Database.clubDB[hostId].Rate < Database.clubDB[guestId].Rate)
            {
                changedCuriosity += -5;
            }
            if (Database.clubDB[hostId].Rate == Database.clubDB[guestId].Rate)
            {
                changedCuriosity += -2;
            }
        }
        else
        {
            if (Database.clubDB[hostId].Rate > Database.clubDB[guestId].Rate)
            {
                changedCuriosity += -5;
            }
            if (Database.clubDB[hostId].Rate < Database.clubDB[guestId].Rate)
            {
                changedCuriosity += 5;
            }
            if (Database.clubDB[hostId].Rate == Database.clubDB[guestId].Rate)
            {
                changedCuriosity += -2;
            }
        }
        print("Zmiana curiosity changedCurisoty = " + changedCuriosity);
    }
    IEnumerator FreeKick(bool isPenalty)
    {
        yield return new WaitForSeconds(time);
        if (isPenalty)
        {
            matchStats[guestBall].ShotTaken();
            minute++;
            List<Footballer> penaltyPlayers = new List<Footballer>();
            for (int i = 1; i < 11; i++)
            {
                penaltyPlayers.Add(teams[guestBall][i]);
            }
            penaltyPlayers = penaltyPlayers.OrderByDescending(x => x.Penalty).ToList();
            CommentLine.PreparingToPenalty(penaltyPlayers[0]);
            minute++;
            yield return new WaitForSeconds(time);
            float plus = penaltyPlayers[0].Penalty - (1.5f * teams[GetReverseIsGuestBall()][0].Rating);
            int rnd = Random.Range(1, 101);
            if (rnd < 80 + plus)
            {
                // gol
                matchStats[guestBall].GoalScored();
                matchStats[guestBall].AddScorer(penaltyPlayers[0], teamName[guestBall],1);
                penaltyPlayers[0].AddStatistic(competitionName, Footballer.PlayerStatistics.StatName.Goals);;
                ChangeCuriosity();
                CommentLine.PenaltyGoal();
                minute++;
                StartCoroutine(CommentStart());
            }
            else
            {
                CommentLine.PenaltyMissed();
                minute++;
                StartCoroutine(CommentStart());
            }

        }
        else
        {

        }
    }
    IEnumerator Shot(int difficulty)
    {
        yield return new WaitForSeconds(time);
        if (difficulty == 10)
        {
            // strzał z bardzo daleka
            matchStats[guestBall].ShotTaken();
            minute++;
            int x = Random.Range(1, 101);
            float plus = (teams[guestBall][playerWithBall].Shoot * 3) - teams[GetReverseIsGuestBall()][0].Rating;
            if (x <= 20)
            {
                // rożny
                CommentLine.LongShotCorner();
                StartCoroutine(Corner());
            }
            else if (x > 20 && x <= 30 + plus)
            {
                // goool
                matchStats[guestBall].GoalScored();
                matchStats[guestBall].AddScorer(teams[guestBall][playerWithBall], teamName[guestBall],1);
                teams[guestBall][playerWithBall].AddStatistic(competitionName, Footballer.PlayerStatistics.StatName.Goals);
                ChangeCuriosity();
                CommentLine.LongShotGoal();
                minute++;
                StartCoroutine(CommentStart());
            }
            else if (x > 30 + plus)
            {
                //pudło
                CommentLine.LongShotMiss();
                minute++;
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 8)
        {
            // strzal z okolo 20 metrow przy kontrataku
            matchStats[guestBall].ShotTaken();
            minute++;
            float keeperPlus = 20 + teams[GetReverseIsGuestBall()][0].Rating * 3;
            float shooterPlus = 20 + teams[0][playerWithBall].Shoot * 2;
            int x = Random.Range(1, (int)keeperPlus + (int)shooterPlus + 10);
            if (x <= keeperPlus)
            {
                // obrona
                int saveType = Random.Range(1, 31);
                if (saveType <= teams[GetReverseIsGuestBall()][0].Rating)  // maks 10/30 szansy ze zlapie
                {
                    //lapie
                    CommentLine.CounterAttackSave();
                    minute++;
                    StartCoroutine(CommentStart());
                }
                else
                {
                    //korner
                    CommentLine.CounterAttackCorner();
                    StartCoroutine(Corner());
                }
            }
            else if (x > keeperPlus && x <= keeperPlus + shooterPlus)
            {
                // goool
                matchStats[guestBall].GoalScored();
                matchStats[guestBall].AddScorer(teams[guestBall][playerWithBall], teamName[guestBall],1);
                teams[guestBall][playerWithBall].AddStatistic(competitionName, Footballer.PlayerStatistics.StatName.Goals);
                ChangeCuriosity();
                CommentLine.CounterAttackGoal();
                minute++;
                StartCoroutine(CommentStart());
            }
            else if (x > keeperPlus + shooterPlus)
            {
                //pudło
                CommentLine.CounterAttackMiss();
                minute++;
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 5)  // okolo 16 metrow normalny atak
        {
            matchStats[guestBall].ShotTaken();
            minute++;
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
                    minute++;
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
                matchStats[guestBall].GoalScored();
                matchStats[guestBall].AddScorer(teams[guestBall][playerWithBall], teamName[guestBall],1);
                teams[guestBall][playerWithBall].AddStatistic(competitionName, Footballer.PlayerStatistics.StatName.Goals);
                ChangeCuriosity();
                CommentLine.NormalAttackGoal();
                minute++;
                StartCoroutine(CommentStart());
            }
            else if (x > keeperPlus + shooterPlus)
            {
                //pudło
                CommentLine.NormalAttackMiss();
                minute++;
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 2)    //   Sam na sam
        {
            matchStats[guestBall].ShotTaken();
            minute++;
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
                    minute++;
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
                matchStats[guestBall].GoalScored();
                matchStats[guestBall].AddScorer(teams[guestBall][playerWithBall], teamName[guestBall],1);
                teams[guestBall][playerWithBall].AddStatistic(competitionName, Footballer.PlayerStatistics.StatName.Goals);
                ChangeCuriosity();
                CommentLine.OneOnOneAttackGoal();
                minute++;
                StartCoroutine(CommentStart());
            }
            else if (x > keeperPlus + shooterPlus)
            {
                CommentLine.OneOnOneAttackMiss();
                minute++;
                StartCoroutine(CommentStart());
            }
        }
        else if (difficulty == 3)    //   główka
        {
            matchStats[guestBall].ShotTaken();
            minute++;
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
                    minute++;
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
                matchStats[guestBall].GoalScored();
                matchStats[guestBall].AddScorer(teams[guestBall][playerWithBall], teamName[guestBall],1);
                teams[guestBall][playerWithBall].AddStatistic(competitionName, Footballer.PlayerStatistics.StatName.Goals);;
                ChangeCuriosity();
                CommentLine.HeaderGoal();
                minute++;
                StartCoroutine(CommentStart());
            }
            else if (x > keeperPlus + shooterPlus)
            {
                CommentLine.HeaderMiss();
                minute++;
                StartCoroutine(CommentStart());
            }
        }
    }
    IEnumerator CounterAttack()
    {
        yield return new WaitForSeconds(time);
        int rnd = Random.Range(1, 100);
        float x = 20 - teams[guestBall][playerWithBall].Pass;
        if (rnd <= 30)
        {
            CommentLine.CounterAttackShotTry();
            yield return new WaitForSeconds(time);
            StartCoroutine(Shot(8));
        }
        else if (rnd > 30 && rnd <= 30 + x)
        {
            CommentLine.CounterAttackFailedPass();
            minute++;
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
            yield return new WaitForSeconds(time);
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
        yield return new WaitForSeconds(time);
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
                yield return new WaitForSeconds(time);
                CommentLine.FreeHeader();
                yield return new WaitForSeconds(time);
                StartCoroutine(Shot(3));
            }
            else
            {
                yield return new WaitForSeconds(time);
                int ran = Random.Range(1, 101);
                if (ran <= (50 + (attackerHeaders[y].Heading - defenderHeaders[y].Heading) * 10))
                {
                    CommentLine.ContestedHeader();
                    yield return new WaitForSeconds(time);
                    StartCoroutine(Shot(3));
                }
                else
                {
                    CommentLine.DefenderWinsHeader(defenderHeaders[y]);
                    minute++;
                    StartCoroutine(CommentStart());
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(time);
            CommentLine.FailedCross();
            minute++;
            StartCoroutine(CommentStart());
        }
    }
    IEnumerator AttackThirdPhase(string direction)
    {
        minute++;
        yield return new WaitForSeconds(time);
        if (direction == "left" || direction == "right")
        {
            int dir = 0;
            if (direction == "right") dir = 1;
            CommentLine.TryingToDodge();
            yield return new WaitForSeconds(time);
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
                            yield return new WaitForSeconds(time);
                            playerWithBall = attackerHeader;
                            CommentLine.FreeHeader();
                            yield return new WaitForSeconds(time);
                            StartCoroutine(Shot(3));
                        }
                        else
                        {
                            yield return new WaitForSeconds(time);
                            int ran = Random.Range(1, 101);
                            if (ran <= (50 + (teams[guestBall][attackerHeader].Heading - teams[GetReverseIsGuestBall()][defenderHeader].Heading) * 10))
                            {
                                playerWithBall = attackerHeader;
                                CommentLine.ContestedHeader();
                                yield return new WaitForSeconds(time);
                                StartCoroutine(Shot(3));
                            }
                            else
                            {
                                CommentLine.DefenderWinsHeader(teams[GetReverseIsGuestBall()][defenderHeader]);
                                minute++;
                                StartCoroutine(CommentStart());
                            }
                        }
                    }
                    else
                    {
                        yield return new WaitForSeconds(time);
                        CommentLine.FailedCross();
                        minute++;
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
                minute++;
                StartCoroutine(CommentStart());
            }
        }
        else if (direction == "middle")
        {

            CommentLine.TryingToDodge();
            yield return new WaitForSeconds(time);
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
                        yield return new WaitForSeconds(time);
                        CommentLine.ChanceForOneOnOne();
                        yield return new WaitForSeconds(time);
                        int ran = Random.Range(1, 101);
                        if (ran <= (50 + (teams[guestBall][atacker].Dribling - teams[GetReverseIsGuestBall()][defender].Tackle) * 10))
                        {
                            CommentLine.OneToOneSituation();
                            yield return new WaitForSeconds(time);
                            StartCoroutine(Shot(2));
                        }
                        else
                        {
                            playerWithBall = defender;
                            CommentLine.FailedChanceOneToOne();
                            minute++;
                            StartCoroutine(CommentStart());
                        }
                        //}
                    }
                    else
                    {
                        yield return new WaitForSeconds(time);
                        CommentLine.FailedPass();
                        minute++;
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
                minute++;
                StartCoroutine(CommentStart());
            }
        }
	}
    public static int GetReverseIsGuestBall()
    {
        return (guestBall == 0)?1:0;
    }
}
