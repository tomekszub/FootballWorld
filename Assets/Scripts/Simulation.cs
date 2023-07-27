using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Simulation : MonoBehaviour
{
    static int _minute = 1;
    public static int hostId, guestId;
    static List<Footballer>[] _teams = new List<Footballer>[2];
    static float[] teamDef = new float[2], teamMid = new float[2], teamAtk = new float[2];
    static int hostChances, goalChances;  // szanse gospodarzy na akcje i akcje bramkowe w meczu
    static MatchStats[] matchStats = new MatchStats[2];//static int hostGoals, guestGoals, hostShots, guestShots
    static string[] teamName = new string[2];
    public static int playerWithBall;  // piłkarz przy piłce
    public static int prevPlayerWithBall;
    public static int guestBall;  // posiadanie piłki
    static int[] defLastPlayerNumber = new int[2];// granice pozycji piłkarzy: obrona , pomoc , atak
    static int[] midLastPlayerNumber = new int[2];
    static List<int>[] teamsMidPos = new List<int>[2];// Lista piłkarzy(pomocnicy of., napastnicy) bedacych w środku formacji
    static List<int>[] teamsDefPos = new List<int>[2];// Lista piłkarzy(obrońcy ) bedacych w środku formacji
    static int[,] wingPos = new int[2, 2]; //static int HostLeftPos, GuestLeftPos, HostRightPos, GuestRightPos;
    static int[,] defWingPos = new int[2, 2]; //int HostDefLeftPos, GuestDefLeftPos, HostDefRightPos, GuestDefRightPos;
    static float curiosity = 1;
    static int weakness = 0;
    static int changedCuriosity = 0;
    static string _competitionName = "";

    static int PlayerWithBall
    {
        get => playerWithBall;
        set
        {
            prevPlayerWithBall = playerWithBall;
            playerWithBall = value;
        }
    }

    static void PrepareNextMatch()
    {
        matchStats[0] = new MatchStats(new List<Scorer>());
        matchStats[1] = new MatchStats(new List<Scorer>());
        teamName[0] = Database.clubDB[hostId].Name;
        teamName[1] = Database.clubDB[guestId].Name;
        _teams[0] = new List<Footballer>();
        _teams[1] = new List<Footballer>();
        for (int i = 0; i < 11; i++)
        {
            _teams[0].Add(Database.footballersDB[Database.clubDB[hostId].FootballersIDs[i]]);
            _teams[0][i].AddStatistic(_competitionName, Footballer.PlayerStatistics.StatName.MatchesPlayed);
            _teams[1].Add(Database.footballersDB[Database.clubDB[guestId].FootballersIDs[i]]);
            _teams[1][i].AddStatistic(_competitionName, Footballer.PlayerStatistics.StatName.MatchesPlayed);
        }
        teamsMidPos[0] = new List<int>();
        teamsMidPos[1] = new List<int>();
        teamsDefPos[0] = new List<int>();
        teamsDefPos[1] = new List<int>();
        ClearLists();
        RecognizeFormation(Database.clubDB[hostId].Formation, true);
        RecognizeFormation(Database.clubDB[guestId].Formation, false);
        CalculateChances();
    }
    public static MatchStats[] StartSimulation(int hostID, int guestID, string competitionName)
    {
        _competitionName = competitionName;
        hostId = hostID;
        guestId = guestID;
        PrepareNextMatch();

        // zmienne pomocnicze
        int who;
        int pos;
        // zabezpieczenie do max 130 min , chyba lepiej niz while(true)
        while (_minute < 130)
        {
            UpdateChances();
            if (_minute >= 90)
            {
                Debug.Log("KONIEC !!!---Wynik: " + teamName[0] + matchStats[0].Goals + "-" + matchStats[1].Goals + teamName[1]);
                break;
            }
            else
            {
                int ch = Random.Range(1, 100);
                if (ch > 5 + goalChances)
                {
                    MinutePassed();
                    continue;
                }
                else
                {
                    who = Random.Range(1, 100);
                    guestBall = 1;
                    if (who <= hostChances) guestBall = 0;
                    pos = Random.Range(1, defLastPlayerNumber[guestBall] + 1);
                    PlayerWithBall = pos;
                    //Debug.Log("First Phase");
                    AttackSecondPhase();
                }
            }
        }
        return matchStats;
    }
    static void AttackSecondPhase()
    {
        //Debug.Log("Second phase");
        int counterPos = Random.Range(midLastPlayerNumber[GetReverseIsGuestBall()] + 1, 11);
        float counterChance = _teams[GetReverseIsGuestBall()][counterPos].Tackle - _teams[guestBall][PlayerWithBall].Pass;
        counterChance += 100;
        counterChance /= 20;
        int dec = Random.Range(1, 37 + (int)counterChance);
        if (dec <= 10)
        {
            // środek podanie
            AttackThirdPhase("middle");
        }
        else if (dec <= 22 && dec > 10)
        {
            // Prawo podanie
            AttackThirdPhase("right");
        }
        else if (dec > 22 && dec <= 34)
        {
            // Lewo podanie
            AttackThirdPhase("left");
        }
        else if (dec > 34 && dec <= 37)
        {
            // Strzał
            Shot(10);
        }
        else
        {
            // Kontratak
            PlayerWithBall = counterPos;
            guestBall = GetReverseIsGuestBall();
            CounterAttack();
        }
    }
    static void RecognizeFormation(string formation, bool host)
    {
        int isGuest = host ? 0 : 1;
        if (formation == "4-3-3")
        {
            teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating + _teams[isGuest][4].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 4) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 4;
            teamMid[isGuest] = (_teams[isGuest][5].Rating + _teams[isGuest][6].Rating + _teams[isGuest][7].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 3) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 7;
            teamAtk[isGuest] = (_teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
            teamAtk[isGuest] = (teamAtk[isGuest] / 3) + (teamAtk[isGuest] / 3);
            teamsMidPos[isGuest].Add(10);
            wingPos[isGuest, 0] = 8;
            wingPos[isGuest, 1] = 9;
        }
        else if (formation == "4-2-3-1" || formation == "4-4-1-1")
        {
            teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating + _teams[isGuest][4].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 4) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 4;
            teamMid[isGuest] = (_teams[isGuest][5].Rating + _teams[isGuest][6].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 2) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 6;
            teamAtk[isGuest] = (_teams[isGuest][7].Rating + _teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
            teamAtk[isGuest] = (teamAtk[isGuest] / 4) + (teamAtk[isGuest] / 3);
            teamsMidPos[isGuest].Add(7);
            teamsMidPos[isGuest].Add(10);
            wingPos[isGuest, 0] = 8;
            wingPos[isGuest, 1] = 9;
        }
        else if (formation == "4-1-4-1")
        {
            teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating + _teams[isGuest][4].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 4) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 4;
            teamMid[isGuest] = (_teams[isGuest][5].Rating + _teams[isGuest][6].Rating + _teams[isGuest][7].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 3) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 6;
            teamAtk[isGuest] = (_teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
            teamAtk[isGuest] = (teamAtk[isGuest] / 3) + (teamAtk[isGuest] / 3);
            teamsMidPos[isGuest].Add(6);
            teamsMidPos[isGuest].Add(7);
            teamsMidPos[isGuest].Add(10);
            wingPos[isGuest, 0] = 8;
            wingPos[isGuest, 1] = 9;
        }
        else if (formation == "4-3-1-2")
        {
            teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating + _teams[isGuest][4].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 4) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 4;
            teamMid[isGuest] = (_teams[isGuest][5].Rating + _teams[isGuest][6].Rating + _teams[isGuest][7].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 3) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 7;
            teamAtk[isGuest] = (_teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
            teamAtk[isGuest] = (teamAtk[isGuest] / 3) + (teamAtk[isGuest] / 3);
            teamsMidPos[isGuest].Add(8);
            teamsMidPos[isGuest].Add(9);
            teamsMidPos[isGuest].Add(10);
            wingPos[isGuest, 0] = 5;
            wingPos[isGuest, 1] = 6;
        }
        else if (formation == "4-4-2")
        {
            teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating + _teams[isGuest][4].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 4) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 4;
            teamMid[isGuest] = (_teams[isGuest][5].Rating + _teams[isGuest][6].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 2) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 6;
            teamAtk[isGuest] = (_teams[isGuest][7].Rating + _teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
            teamAtk[isGuest] = (teamAtk[isGuest] / 4) + (teamAtk[isGuest] / 3);
            teamsMidPos[isGuest].Add(9);
            teamsMidPos[isGuest].Add(10);
            wingPos[isGuest, 0] = 7;
            wingPos[isGuest, 1] = 8;
        }
        else if (formation == "5-3-2")
        {
            // dzielimy przez 4 ,a nie przez 5, zeby uwydatnic zalete posiadania 5 obroncow
            teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating + _teams[isGuest][4].Rating + _teams[isGuest][5].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 5) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 5;
            teamMid[isGuest] = (_teams[isGuest][6].Rating + _teams[isGuest][7].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 2) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 7;
            teamAtk[isGuest] = (_teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
            teamAtk[isGuest] = (teamAtk[isGuest] / 3) + (teamAtk[isGuest] / 3);
            teamsMidPos[isGuest].Add(8);
            teamsMidPos[isGuest].Add(9);
            teamsMidPos[isGuest].Add(10);
            wingPos[isGuest, 0] = 6;
            wingPos[isGuest, 1] = 7;
        }
        else if (formation == "3-4-1-2" || formation == "3-4-2-1")
        {
            teamDef[isGuest] = (_teams[isGuest][1].Rating + _teams[isGuest][2].Rating + _teams[isGuest][3].Rating);
            teamDef[isGuest] = (teamDef[isGuest] / 3) + (teamDef[isGuest] / 4);
            defLastPlayerNumber[isGuest] = 3;
            teamMid[isGuest] = (_teams[isGuest][4].Rating + _teams[isGuest][5].Rating + _teams[isGuest][6].Rating + _teams[isGuest][7].Rating);
            teamMid[isGuest] = (teamMid[isGuest] / 4) + (teamMid[isGuest] / 3);
            midLastPlayerNumber[isGuest] = 7;
            teamAtk[isGuest] = (_teams[isGuest][8].Rating + _teams[isGuest][9].Rating + _teams[isGuest][10].Rating);
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
        else if (defLastPlayerNumber[isGuest] == 4)
        {
            teamsDefPos[isGuest].Add(2);
            teamsDefPos[isGuest].Add(3);
            defWingPos[isGuest, 0] = 1;
            defWingPos[isGuest, 1] = 4;
        }
        else if (defLastPlayerNumber[isGuest] == 3)
        {
            teamsDefPos[isGuest].Add(1);
            teamsDefPos[isGuest].Add(2);
            teamsDefPos[isGuest].Add(3);
        }
    }
    static void UpdateChances()
    {
        hostChances = 50;
        // difference between host's attack and guest's defense
        float advantage = ((teamAtk[0] - teamDef[1]) / 10) / 3;
        advantage += ((teamDef[0] - teamAtk[1]) / 10) / 3;
        advantage += ((teamMid[0] - teamMid[1]) / 10) / 3;
        hostChances += 4 + Mathf.RoundToInt(advantage);

        hostChances -= weakness;
        if (hostChances > 90 && hostChances < 160)
            hostChances = 90;
        if (hostChances >= 160)
            hostChances = 95;
        if (hostChances < 10 && hostChances > -60)
            hostChances = 10;
        if (hostChances <= -160)
            hostChances = 5;

        float ratingDiff = Mathf.Abs(Database.clubDB[hostId].Rate - Database.clubDB[guestId].Rate) / 10;
        float GoalChances = (5 + (ratingDiff / 3)) / curiosity + changedCuriosity;
        goalChances = Mathf.Clamp(Mathf.RoundToInt(GoalChances), 4, 35);
    }
    static void CalculateChances()
    {
        weakness = 0;
        int rnd = Random.Range(1, 101);

        if (rnd > 10)
            return;

        int ran = Random.Range(0, Database.clubDB[hostId].Rate + Database.clubDB[guestId].Rate);

        bool hostIsWeakToday = Database.clubDB[hostId].Rate > ran;

        weakness = hostIsWeakToday ? 10 : -10;

        if (Database.clubDB[hostId].Rate > Database.clubDB[guestId].Rate)
            curiosity = hostIsWeakToday ? 2 : 0.5f;
        else if (Database.clubDB[hostId].Rate < Database.clubDB[guestId].Rate)
            curiosity = hostIsWeakToday ? 0.5f : 2;
        else
            curiosity = 3;
    }
    static void ClearLists()
    {
        //if(transform.Find("Text") != null) transform.Find("Text").GetComponent<Text>().text = "";
        teamsMidPos[0].Clear();
        teamsMidPos[1].Clear();
        teamsDefPos[0].Clear();
        teamsDefPos[1].Clear();
        matchStats[0].Reset();
        matchStats[1].Reset();
        _minute = 0;
    }
    static void UpdateCuriosity()
    {
        int multiplier = guestBall / 3 - 1;

        if (Database.clubDB[hostId].Rate > Database.clubDB[guestId].Rate)
            changedCuriosity += 5 * multiplier;
        else if (Database.clubDB[hostId].Rate < Database.clubDB[guestId].Rate)
            changedCuriosity += -5 * multiplier;
        else
            changedCuriosity -= 2;
    }
    static void FreeKick(bool isPenalty)
    {
        PlayerWithBall = -1;
        if (isPenalty)
        {
            matchStats[guestBall].ShotTaken();
            MinutePassed();
            int[] penaltyPlayers = new int[10];
            for (int i = 1; i < 11; i++)
            {
                penaltyPlayers[i - 1] = i;
            }
            penaltyPlayers = penaltyPlayers.OrderByDescending(x => _teams[guestBall][x].Penalty).ToArray();
            PlayerWithBall = penaltyPlayers[0];
            MinutePassed();
            float plus = (_teams[guestBall][PlayerWithBall].Penalty / 5) - (_teams[GetReverseIsGuestBall()][0].Rating / 7);
            int rnd = Random.Range(1, 101);
            if (rnd < 80 + plus)
            {
                // gol
                GoalScored();
                MinutePassed();
                return;
            }
            else
            {
                MinutePassed();
                return;
            }
        }
        else
        {
        }
    }

    static void GoalScored()
    {
        matchStats[guestBall].GoalScored();
        matchStats[guestBall].AddScorer(_teams[guestBall][PlayerWithBall], teamName[guestBall], 1);
        _teams[guestBall][PlayerWithBall].AddStatistic(_competitionName, Footballer.PlayerStatistics.StatName.Goals);
        if (prevPlayerWithBall != -1)
        {
            _teams[guestBall][prevPlayerWithBall].AddStatistic(_competitionName, Footballer.PlayerStatistics.StatName.Assists);
            prevPlayerWithBall = -1;
        }
        UpdateCuriosity();
    }

    static void Shot(int difficulty)
    {
        
        if (difficulty == 10)
        {
            // strzał z bardzo daleka
            matchStats[guestBall].ShotTaken();
            MinutePassed();
            int x = Random.Range(1, 101);
            float plus = (_teams[guestBall][PlayerWithBall].Shoot / 5) - (_teams[GetReverseIsGuestBall()][0].Rating / 10);
            if (x <= 20)
            {
                // rożny
                Corner();
            }
            else if (x > 20 && x <= 30 + plus)
            {
                // goool
                GoalScored();
                MinutePassed();
                return;
            }
            else if (x > 30 + plus)
            {
                //pudło
                MinutePassed();
                return;
            }
        }
        else if (difficulty == 8)
        {
            // strzal z okolo 20 metrow przy kontrataku
            matchStats[guestBall].ShotTaken();
            MinutePassed();
            int keeperPlus = (int)(20 + _teams[GetReverseIsGuestBall()][0].Rating / 3);
            int shooterPlus = (int)(20 + _teams[0][PlayerWithBall].Shoot / 5);
            int x = Random.Range(1, keeperPlus + shooterPlus + 10);
            if (x <= keeperPlus)
            {
                // obrona
                int saveType = Random.Range(1, 31);
                if (saveType <= _teams[GetReverseIsGuestBall()][0].Rating / 10)  // maks 10/30 szansy ze zlapie
                {
                    //lapie
                    MinutePassed();
                    return;
                }
                else
                {
                    //korner
                    Corner();
                }
            }
            else if (x > keeperPlus && x <= keeperPlus + shooterPlus)
            {
                // goool
                GoalScored();
                MinutePassed();
                return;
            }
            else if (x > keeperPlus + shooterPlus)
            {
                //pudło
                MinutePassed();
                return;
            }
        }
        else if (difficulty == 5)  // okolo 16 metrow normalny atak
        {
            matchStats[guestBall].ShotTaken();
            MinutePassed();
            int keeperPlus = (int)(5 + _teams[GetReverseIsGuestBall()][0].Rating / 5);
            int shooterPlus = (int)(15 + _teams[guestBall][PlayerWithBall].Shoot / 3);
            int x = Random.Range(1, keeperPlus + shooterPlus + 15);
            if (x <= keeperPlus)
            {
                // obrona
                int saveType = Random.Range(1, 51);
                if (saveType <= _teams[GetReverseIsGuestBall()][0].Rating / 10)  // maks 10/50 szansy ze zlapie
                {
                    //lapie
                    MinutePassed();
                    return;
                }
                else
                {
                    //korner
                    Corner();
                }
            }
            else if (x > keeperPlus && x <= keeperPlus + shooterPlus)
            {
                // goool
                GoalScored();
                MinutePassed();
                return;
            }
            else if (x > keeperPlus + shooterPlus)
            {
                //pudło
                MinutePassed();
                return;
            }
        }
        else if (difficulty == 2)    //   Sam na sam
        {
            matchStats[guestBall].ShotTaken();
            MinutePassed();
            int keeperPlus = (int)(10 + _teams[GetReverseIsGuestBall()][0].Rating / 5);
            int shooterPlus = (int)(25 + _teams[guestBall][PlayerWithBall].Shoot / 2);
            int x = Random.Range(1, keeperPlus + shooterPlus + 5);
            if (x <= keeperPlus)
            {
                int saveType = Random.Range(1, 51);
                if (saveType <= _teams[GetReverseIsGuestBall()][0].Rating / 10)  // maks 10/50 szansy ze zlapie albo nie bedzie roznego
                {
                    //lapie
                    MinutePassed();
                    return;
                }
                else
                {
                    //korner
                    Corner();
                }
            }
            else if (x > keeperPlus && x <= keeperPlus + shooterPlus)
            {
                // goool
                GoalScored();
                MinutePassed();
                return;
            }
            else if (x > keeperPlus + shooterPlus)
            {
                MinutePassed();
                return;
            }
        }
        else if (difficulty == 3)    //   główka
        {
            matchStats[guestBall].ShotTaken();
            MinutePassed();
            int keeperPlus = (int)(5 + _teams[GetReverseIsGuestBall()][0].Rating / 5);
            int shooterPlus = (int)(10 + _teams[guestBall][PlayerWithBall].Heading / 2);
            int x = Random.Range(1, keeperPlus + shooterPlus + 10);
            if (x <= keeperPlus)
            {
                int saveType = Random.Range(1, 31);
                if (saveType <= _teams[GetReverseIsGuestBall()][0].Rating / 10)  // maks 10/30 szansy ze zlapie albo nie bedzie roznego
                {
                    MinutePassed();
                    return;
                }
                else
                    Corner();
            }
            else if (x > keeperPlus && x <= keeperPlus + shooterPlus)
            {
                // goool
                GoalScored();
                MinutePassed();
                return;
            }
            else if (x > keeperPlus + shooterPlus)
            {
                MinutePassed();
                return;
            }
        }
    }
    static void CounterAttack()
    {
        prevPlayerWithBall = -1;

        int rnd = Random.Range(1, 100);
        float x = 20 - _teams[guestBall][PlayerWithBall].Pass / 10;
        if (rnd <= 30)
        {
            Shot(8);
        }
        else if (rnd > 30 && rnd <= 30 + x)
        {
            MinutePassed();
            return;
        }
        else if (rnd > 30 + x)
        {
            List<int> indices = Enumerable.Range(midLastPlayerNumber[guestBall] + 1, 10 - midLastPlayerNumber[guestBall]).ToList();
            indices.Remove(PlayerWithBall);

            PlayerWithBall = indices[Random.Range(0, indices.Count)];

            int chan = Random.Range(1, 101);
            if (chan < 70)
            {
                FreeKick(true);
            }
            else
            {
                Shot(2);
            }
        }
    }
    static void Corner()
    {
        
        List<Footballer> cornerPlayers = new List<Footballer>();
        for (int i = 1; i < 11; i++)
        {
            cornerPlayers.Add(_teams[guestBall][i]);
        }
        cornerPlayers = cornerPlayers.OrderByDescending(x => x.Corner).ToList();
        float border = 35 + ((cornerPlayers[0].FreeKicks + cornerPlayers[0].Pass) * 0.32f);
        cornerPlayers.RemoveAt(0);
        int acc = Random.Range(1, 100);
        if (acc <= border)
        {
            List<Footballer> attackerHeaders = new List<Footballer>();
            List<Footballer> defenderHeaders = new List<Footballer>();
            attackerHeaders = cornerPlayers.OrderByDescending(x => x.Heading).ToList();
            for (int i = 1; i < 11; i++)
            {
                defenderHeaders.Add(_teams[1][i]);
            }
            int y = Random.Range(0, 30); // losowanie numeru piłkarzy, którzy będą walczyć o piłkę
            y /= 10;
            for (int i = 1; i < 11; i++)
            {
                if (attackerHeaders[y].Surname == _teams[guestBall][i].Surname && attackerHeaders[y].Name == _teams[guestBall][i].Name)
                {
                    PlayerWithBall = i;
                    break;
                }
            }
            int rnd = Random.Range(1, 101);
            // główka
            if (rnd <= 25 + ((attackerHeaders[y].Rating - defenderHeaders[y].Rating) / 5))
            {
                // zgubienie obrońcy i strzał głową
                Shot(3);
            }
            else
            {
                int ran = Random.Range(1, 101);
                if (ran <= 50 + ((attackerHeaders[y].Heading - defenderHeaders[y].Heading) / 2))
                {
                    Shot(3);
                }
                else
                {
                    MinutePassed();
                    return;
                }
            }
        }
        else
        {
            MinutePassed();
            return;
        }
    }

    static void AttackThirdPhase(string direction)
    {
        //Debug.Log("Third phase");
        MinutePassed();
        if (direction == "left" || direction == "right")
        {
            int dir = direction == "right" ? 1 : 0;
            float plus = ((_teams[guestBall][PlayerWithBall].Dribling + _teams[guestBall][PlayerWithBall].Speed) - (_teams[GetReverseIsGuestBall()][defWingPos[GetReverseIsGuestBall(), dir]].Tackle + _teams[GetReverseIsGuestBall()][defWingPos[GetReverseIsGuestBall(), dir]].Speed)) / 3;
            if (Random.Range(1, 101) < 55 + plus)
            {
                //dośrodkowanie bądź strzał
                if (Random.Range(1, 101) <= 70)
                {
                    float border = 40 + (_teams[0][PlayerWithBall].Pass / 3);           //----------------- ewentualnei zmniejszyc mnożnik gdyby za dużo goli z główki
                    int acc = Random.Range(1, 100);
                    if (acc <= border)
                    {
                        List<int> playersToChoose = new List<int>();
                        for (int i = midLastPlayerNumber[guestBall] + 1; i < 11; i++)
                            playersToChoose.Add(i);

                        playersToChoose.Remove(PlayerWithBall);

                        int attackerHeader = playersToChoose[Random.Range(0, playersToChoose.Count)];

                        playersToChoose.Clear();
                        for (int i = 1; i < defLastPlayerNumber[guestBall] + 1; i++)
                            playersToChoose.Add(i);

                        playersToChoose.Remove(defWingPos[GetReverseIsGuestBall(), dir]);

                        int defenderHeader = playersToChoose[Random.Range(0, playersToChoose.Count)];

                        // główka
                        if (Random.Range(1, 101) <= 30 + ((_teams[guestBall][attackerHeader].Rating - _teams[GetReverseIsGuestBall()][defenderHeader].Rating) / 3))
                        {
                            // zgubienie obrońcy i stzrał głową
                            
                            PlayerWithBall = attackerHeader;                          
                            Shot(3);
                        }
                        else
                        {
                            if (Random.Range(1, 101) <= 50 + ((_teams[guestBall][attackerHeader].Heading - _teams[GetReverseIsGuestBall()][defenderHeader].Heading) / 3))
                            {
                                PlayerWithBall = attackerHeader;
                                Shot(3);
                            }
                            else
                            {
                                MinutePassed();
                                return;
                            }
                        }
                    }
                    else
                    {
                        MinutePassed();
                        return;
                    }
                }
                else
                {
                    Shot(5);
                }
            }
            else
            {
                MinutePassed();
                return;
            }
        }
        else if (direction == "middle")
        {          
            int firstDef = teamsDefPos[GetReverseIsGuestBall()][Random.Range(0, teamsDefPos[GetReverseIsGuestBall()].Count)];
            float plus = ((_teams[guestBall][PlayerWithBall].Dribling + _teams[guestBall][PlayerWithBall].Speed) - (_teams[GetReverseIsGuestBall()][firstDef].Tackle + _teams[GetReverseIsGuestBall()][firstDef].Speed)) / 3;
            if (Random.Range(1, 101) < 45 + plus)
            {
                int decision = Random.Range(1, 101);
                //podanie bądź strzał
                if (decision <= 65)
                {
                    float border = 30 + _teams[guestBall][PlayerWithBall].Pass / 3;
                    if (Random.Range(1, 101) <= border)
                    {
                        List<int> playersToChoose = new List<int>();
                        for (int i = midLastPlayerNumber[guestBall] + 1; i < 11; i++)
                            playersToChoose.Add(i);

                        playersToChoose.Remove(PlayerWithBall);

                        PlayerWithBall = playersToChoose[Random.Range(0, playersToChoose.Count)];

                        playersToChoose = new(teamsDefPos[GetReverseIsGuestBall()]);

                        playersToChoose.Remove(firstDef);

                        int defenderIndex = playersToChoose[Random.Range(0, playersToChoose.Count)];

                        if (Random.Range(1, 101) <= (50 + (_teams[guestBall][PlayerWithBall].Dribling - _teams[GetReverseIsGuestBall()][defenderIndex].Tackle) / 3))
                        {                     
                            Shot(2);
                        }
                        else
                        {
                            PlayerWithBall = defenderIndex;
                            MinutePassed();
                            return;
                        }
                    }
                    else
                    {
                        MinutePassed();
                        return;
                    }
                }
                else
                    Shot(5);
            }
            else
                MinutePassed();
        }
    }
    static int GetReverseIsGuestBall()
    {
        // returns opposite to guestBall eg. guestBall 1, returns 0 and vice versa
        return (guestBall == 0) ? 1 : 0;
    }

    static void MinutePassed()
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
}
