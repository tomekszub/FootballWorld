using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class MyClub : MonoBehaviour 
{
    public static int myLeagueID;
    public static int myInLeagueIndex;
    public static int myClubID;
    public static HashSet<string> myTournaments = new HashSet<string>();
    static string leagueName;
    public int testHost, testGuest;
    DateTime startOfTheSeason = new DateTime(2018, 8, 17);
    DateTime currDate = new DateTime(2018, 6, 24);
    DateTime[] cupDates = { new DateTime(2018, 9, 4), new DateTime(2018, 10, 16), new DateTime(2018, 10, 30), new DateTime(2019, 1, 8), new DateTime(2019, 1, 22), new DateTime(2019, 2, 5), new DateTime(2019, 5, 21)};
    //Comment c;
    static int currentMatch = 0, firstMatchOfTheWeek = 0;
    //public Database Database;
    Comment c;
    public GameObject squadCell, emptyCell;
    public GameObject squadPanelsParent, substitutesParent;
    public GameObject OutputMoney;
    public GameObject calendarEntry;
    public GameObject calendarParent;
	public GameObject test;
	public GameObject OutputClubName;
	//string inputName, inputStadiumName, inputClubName, inputFormation;
	public GameObject playersText;
	public GameObject parentObj;
    public GameObject commentPanel, clubMenuPanel;
    public Dropdown leagueDropDown, clubDropDown, formationDropDown;
    List<CupKnockoutStage> nationalCup = new List<CupKnockoutStage>();
    List<CupRound> qualCL = new List<CupRound>();
    List<int> numbers = new List<int>();
    List<Club> clubs;
    string[] positions = new string[11];
    int roundnumber;
	public GameObject table;
	[SerializeField]
	Table tableScript;
	//public Comment comment;
	int teamsNumber;
	public static List<Match> matches = new List<Match>();
    //int matchesCount = 0;
    int currCupRound = 0, currCLRound = 0;
    // glupota, lenistwo ale zadziala na razie
    public MatchStats leagueScorers = new MatchStats(new List<Scorer>());
    //bool squadWindowNeedsUpdate = true;
	// Use this for initialization
	void Start () 
	{
        //tableScript = GameObject.GetComponent<Table> ();
        //currDate = startOfTheSeason;
        startOfTheSeason = startOfTheSeason.AddDays(1);
        c = FindObjectOfType<Comment>();
        //Database.clubDB.Add (new Club (0, inputClubName, 1, inputStadiumName, 10000, inputName, inputFormation, 10000, 10011));
    }
    public void PrepareLeagueDropDown()
    {
        //List<string> l = new List<string>();
        List<Dropdown.OptionData> l = new List<Dropdown.OptionData>();
        foreach (League_Old lg in Database.leagueDB)
        {
            //l.Add(lg.Name);
            l.Add(new Dropdown.OptionData(lg.Name, Resources.Load<Sprite>("Flags/" + lg.Country)));
        }
        leagueDropDown.AddOptions(l);
        UpdateClubsList(0);
    }
    public void UpdateClubsList(int chosenLeague)
    {
        List<string> cs = new List<string>();
        foreach (Club c in Database.leagueDB[chosenLeague].Teams)
        {
            cs.Add(c.Name);
        }
        clubDropDown.ClearOptions();
        clubDropDown.AddOptions(cs);
    }
	/*public void StartRandomPlayers()
	{
		inputFormation = InputFormation.transform.Find("Text").GetComponent<Text> ().text;
		numbers.Clear ();
		playersText.transform.GetComponent<Text> ().text = "";
		Reckognize (inputFormation);
		for (int i = 0; i < 11; i++) 
		{
			//print(i);
			int rnd;
			do 
			{
				do 
				{
					rnd = UnityEngine.Random.Range(0, Database.footballersDB.Count);
				} 
				while (Database.footballersDB[rnd].Pos.ToString() != positions[i] || Database.footballersDB[rnd].Id > 10000);
			} 
			while (SearchRepetitions(rnd));
			numbers.Add(rnd);
			Database.footballersDB.Add (new Footballer(10001 + i, Database.footballersDB[rnd].Name, Database.footballersDB[rnd].Surname,Database.footballersDB[rnd].AlteredSurname, Database.footballersDB[rnd].Country, Database.footballersDB[rnd].Rate, Database.footballersDB[rnd].FreeKicks, Database.footballersDB[rnd].Pos,   Database.footballersDB[rnd].Dribling, Database.footballersDB[rnd].Tackle, Database.footballersDB[rnd].Heading, Database.footballersDB[rnd].Shoot, Database.footballersDB[rnd].Speed, Database.footballersDB[rnd].Pass));
			playersText.transform.GetComponent<Text>().text += (i + 1) + " " + Database.footballersDB[rnd].Name + " " + Database.footballersDB[rnd].Surname + "\n";
		}
	}*/
	public void CreateTeam()
	{
        //myClubID = 10;

        //table.SetActive (true);
        //inputClubName = InputClubName.transform.Find("Text").GetComponent<Text> ().text;
        //Database.clubDB.Insert (0,new Club (0, inputClubName,"Hiszpania", 1, inputStadiumName, 10000, inputName, inputFormation, 10001, 10011,0));
        myLeagueID = leagueDropDown.value;
        //Debug.Log(myLeagueID);
        teamsNumber = Database.leagueDB [myLeagueID].Teams.Count;
        leagueName = Database.leagueDB[myLeagueID].Name;
        for (int i = 0; i < teamsNumber; i++) 
		{
			tableScript.tableTeams.Add(new Team(Database.leagueDB[myLeagueID].Teams[i].Id ,Database.leagueDB[myLeagueID].Teams[i].Name, 0, 0, 0, 0, 0, 0));
			//print (tableScript.tableTeams.Count + " " + tableScript.tableRowPrefabs.Count);
		}
        myInLeagueIndex = clubDropDown.value;
        myClubID = Database.leagueDB[myLeagueID].Teams[myInLeagueIndex].Id;
        //tableScript.tableTeams.Add(new Team(0 ,inputClubName, 0, 0, 0, 0, 0, 0));
        tableScript.ShowTable ();
        OutputClubName.transform.GetComponent<TextMeshProUGUI>().text = Database.clubDB[myClubID].Name;
        roundnumber = teamsNumber * 2 - 2;
        clubs = new List<Club>();
        for (int i = 0; i < teamsNumber; i++)
        {
            clubs.Add(Database.leagueDB[myLeagueID].Teams[i]);
        }
        // -1 oznacza ze numer rundy ma byc automatyczny, to znaczy kazda runda inny id rundy (dla ligi jest ok, np dla fazy gr lm trzeba to ustawic)
        MatchCalendar.CreateGroupCalendar(Database.leagueDB[myLeagueID].Name, -1, clubs, startOfTheSeason);
        myTournaments.Add(Database.leagueDB[myLeagueID].Name);
        CreateCupCalendar();
        myTournaments.Add("National Cup");
        CreateChampionsLeagueCalendar();
        myTournaments.Add("Champions Cup");
        UpdateCalendar();
        //c = new Comment();
        //c = FindObjectOfType<Comment>();
        parentObj.SetActive (false);
        OutputMoney.transform.GetComponent<TextMeshProUGUI>().text = currDate.Day + "/" + currDate.Month;
    }
	bool SearchRepetitions(int number)
	{
		// jesli number był juz wylosowany zawraca true
		if (numbers.Count == 0) 
		{
			return false;
		}
		for (int i = 0; i < numbers.Count; i++) 
		{
			if (number == numbers[i])
			{
				print ("powtórka");
				return true;
			}
		}
		return false;

	}
	/*void Reckognize(string formation)
	{
		if (formation == "4-2-3-1") 
		{
			positions[0] = "BR";
			positions[1] = "LO";
			positions[2] = "ŚO";
			positions[3] = "ŚO";
			positions[4] = "PO";
			positions[5] = "ŚP";
			positions[6] = "ŚP";
			positions[7] = "ŚPO";
			positions[8] = "LP";
			positions[9] = "PP";
			positions[10] = "N";
		}
		else if (formation == "4-3-3") 
		{
			print("4-3-3");
			positions[0] = "BR";
			positions[1] = "LO";
			positions[2] = "ŚO";
			positions[3] = "ŚO";
			positions[4] = "PO";
			positions[5] = "ŚP";
			positions[6] = "ŚP";
			positions[7] = "ŚPO";
			positions[8] = "LP";
			positions[9] = "PP";
			positions[10] = "N";
		}
		else if (formation == "4-3-1-2") 
		{
			print("4-3-1-2");
			positions[0] = "BR";
			positions[1] = "LO";
			positions[2] = "ŚO";
			positions[3] = "ŚO";
			positions[4] = "PO";
			positions[5] = "ŚP";
			positions[6] = "ŚP";
			positions[7] = "ŚP";
			positions[8] = "ŚPO";
			positions[9] = "N";
			positions[10] = "N";
		}
		else //(formation == "4-4-2") 
		{
			print("4-4-2");
            inputFormation = "4-4-2";
            positions[0] = "BR";
			positions[1] = "LO";
			positions[2] = "ŚO";
			positions[3] = "ŚO";
			positions[4] = "PO";
			positions[5] = "ŚP";
			positions[6] = "ŚP";
			positions[7] = "LP";
			positions[8] = "PP";
			positions[9] = "N";
			positions[10] = "N";
		}
	}*/
	// Update is called once per frame
	public void NextMatch()
	{
        calendarParent.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1;
        if (currentMatch >= matches.Count)
        {
            Debug.Log("Koniec meczów!");
            return;
        }
        if (matches[currentMatch].Date != currDate)
        {
            currDate = currDate.AddDays(1);
            OutputMoney.transform.GetComponent<Text>().text = currDate.Day + "/" + currDate.Month;
            UpdateCalendar();
            return;
        }
        int hostId, guestId;
        bool endOfRound = false;
        do
        {
            if (currentMatch >= matches.Count)
            {
                Debug.Log("Koniec meczów!");
                return;
            }
            if((currentMatch+1 < matches.Count && matches[currentMatch].Date < matches[currentMatch+1].Date) || currentMatch == matches.Count-1)
            {
                endOfRound = true;
            }

            hostId = matches[currentMatch].FirstTeamId;
            guestId = matches[currentMatch].SecondTeamId;
            if (hostId != myClubID && guestId != myClubID)
            {
                MatchStats[] ms = Simulation.SimulationStart(hostId, guestId, matches[currentMatch].CompetitionName);
                ProcesssMatchStats(ms, matches[currentMatch].CompetitionName);
                if (endOfRound)
                {
                    // add addDay(1) if we want automatically advance to the next day, otherwise we will ahve to click next match button after finished day
                    //UpdateCalendar();
                    return;
                }
            }
            
        }
        while (hostId != myClubID && guestId != myClubID);

        commentPanel.SetActive(true);
        clubMenuPanel.SetActive(false);
        Debug.Log("Competition Name: " + matches[currentMatch].CompetitionName);
        Debug.Log("Matches Size: " + matches.Count);
        Debug.Log("Current Match: " + currentMatch);
        Debug.Log("Host: " + hostId);
        Debug.Log("Guest: " + guestId);
        c.Init(hostId, guestId, matches[currentMatch].CompetitionName);
    }

    public void ProcesssMatchStats(MatchStats[] matchStats, string competitionName)
    {
        int hostId = matches[currentMatch].FirstTeamId;
        int guestId = matches[currentMatch].SecondTeamId;
        matches[currentMatch].Result.HostGoals = matchStats[0].GetGoals();
        matches[currentMatch].Result.GuestGoals = matchStats[1].GetGoals();
        matches[currentMatch].Finished = true;
        if (competitionName == leagueName)
        {
            #region Table
            if (matchStats[0].GetGoals() > matchStats[1].GetGoals())
            {
                for (int i = 0; i < tableScript.tableTeams.Count; i++)
                {
                    if (tableScript.tableTeams[i].Id == hostId)
                    {
                        tableScript.tableTeams[i].LostGoals += matchStats[1].GetGoals();
                        tableScript.tableTeams[i].Points += 3;
                        tableScript.tableTeams[i].ScoredGoals += matchStats[0].GetGoals();
                        tableScript.tableTeams[i].Wins++;
                        tableScript.tableTeams[i].MatchesPlayed++;
                        tableScript.tableTeams[i].DifferenceGoals += matchStats[0].GetGoals() - matchStats[1].GetGoals();
                    }
                    if (tableScript.tableTeams[i].Id == guestId)
                    {
                        tableScript.tableTeams[i].LostGoals += matchStats[0].GetGoals();
                        tableScript.tableTeams[i].ScoredGoals += matchStats[1].GetGoals();
                        tableScript.tableTeams[i].Loses++;
                        tableScript.tableTeams[i].MatchesPlayed++;
                        tableScript.tableTeams[i].DifferenceGoals -= matchStats[0].GetGoals() - matchStats[1].GetGoals();
                    }
                }
            }
            else if (matchStats[0].GetGoals() < matchStats[1].GetGoals())
            {
                for (int i = 0; i < tableScript.tableTeams.Count; i++)
                {
                    if (tableScript.tableTeams[i].Id == hostId)
                    {
                        tableScript.tableTeams[i].LostGoals += matchStats[1].GetGoals();
                        tableScript.tableTeams[i].ScoredGoals += matchStats[0].GetGoals();
                        tableScript.tableTeams[i].Loses++;
                        tableScript.tableTeams[i].MatchesPlayed++;
                        tableScript.tableTeams[i].DifferenceGoals += matchStats[0].GetGoals() - matchStats[1].GetGoals();
                    }
                    if (tableScript.tableTeams[i].Id == guestId)
                    {
                        tableScript.tableTeams[i].LostGoals += matchStats[0].GetGoals();
                        tableScript.tableTeams[i].Points += 3;
                        tableScript.tableTeams[i].ScoredGoals += matchStats[1].GetGoals();
                        tableScript.tableTeams[i].Wins++;
                        tableScript.tableTeams[i].MatchesPlayed++;
                        tableScript.tableTeams[i].DifferenceGoals -= matchStats[0].GetGoals() - matchStats[1].GetGoals();
                    }
                }
            }
            else
            {
                for (int i = 0; i < tableScript.tableTeams.Count; i++)
                {
                    if (tableScript.tableTeams[i].Id == hostId)
                    {
                        tableScript.tableTeams[i].LostGoals += matchStats[1].GetGoals();
                        tableScript.tableTeams[i].Points += 1;
                        tableScript.tableTeams[i].ScoredGoals += matchStats[0].GetGoals();
                        tableScript.tableTeams[i].Draws++;
                        tableScript.tableTeams[i].MatchesPlayed++;
                    }
                    if (tableScript.tableTeams[i].Id == guestId)
                    {
                        tableScript.tableTeams[i].LostGoals += matchStats[0].GetGoals();
                        tableScript.tableTeams[i].Points += 1;
                        tableScript.tableTeams[i].ScoredGoals += matchStats[1].GetGoals();
                        tableScript.tableTeams[i].Draws++;
                        tableScript.tableTeams[i].MatchesPlayed++;
                    }
                }
            }
            #endregion
            foreach (var s in matchStats[0].scorers)
            {
                leagueScorers.AddScorer(s);
            }
            foreach (var s in matchStats[1].scorers)
            {
                leagueScorers.AddScorer(s);
            }
            leagueScorers.scorers = leagueScorers.scorers.OrderByDescending(n => n.goals).ToList();
            test.GetComponent<Text>().text = "";
            for (int i = 0; i < 8; i++)
            {
                if (leagueScorers.scorers.Count >= i + 1) test.GetComponent<Text>().text += leagueScorers.scorers[i].goals + "\t" + leagueScorers.scorers[i].name + " " + leagueScorers.scorers[i].surname + "\n";
            }
        }
        else if(competitionName == "National Cup")
        {
            //Debug.Log("Mecz pucharowy");
            nationalCup[matches[currentMatch].RoundIndex].SendMatchResult(matches[currentMatch]);
            if (nationalCup[currCupRound].GetWinners() != null)
            {
                //time for new round
                //we are checking if the previous round was a final (number of winners == 1), if not..
                if(nationalCup[currCupRound].GetWinners().Count != 1)
                {
                    nationalCup.Add(new CupKnockoutStage("National Cup",null, nationalCup[currCupRound].GetWinners()));
                    currCupRound++;
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
                    AddEuropaCupMatchesToCalendar(nationalCup[currCupRound].GetMatches(), currCupRound, cupDates[currCupRound], cupDates[currCupRound].AddDays(7));
                    
                } 
            }
        }
        else if(competitionName == "Champions Cup")
        {
            qualCL[matches[currentMatch].RoundIndex].SendMatchResult(matches[currentMatch]);
            if (qualCL[currCLRound].GetWinners() != null)
            {
                if (currCLRound == 0)
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
                    qualCL.Add(new CupKnockoutStage("Champions Cup", cs, qualCL[0].GetWinners(), true, 2));
                    currCLRound++;
                    Match[] ms = qualCL[1].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms,1, new DateTime(2018,7,10), new DateTime(2018, 7, 17));
                }
                else if (currCLRound == 1)
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
                    qualCL.Add(new CupKnockoutStage("Champions Cup", cs, qualCL[1].GetWinners(), true, 2));
                    currCLRound++;
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
                    qualCL.Add(new CupKnockoutStage("Champions Cup", cs, null, true, 2));
                    currCLRound++;
                    //

                    Match[] ms = qualCL[2].GetMatches();
                    Match[] ms2 = qualCL[3].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms,2, new DateTime(2018, 7, 24), new DateTime(2018, 7, 31));
                    AddEuropaCupMatchesToCalendar(ms2,3, new DateTime(2018, 7, 24), new DateTime(2018, 7, 31));
                }
                else if (currCLRound == 3)
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
                    qualCL.Add(new CupKnockoutStage("Champions Cup", cs, qualCL[2].GetWinners(), true, 2));
                    currCLRound++;
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
                    qualCL.Add(new CupKnockoutStage("Champions Cup", cs, qualCL[3].GetWinners(), true, 2));
                    currCLRound++;
                    Match[] ms = qualCL[4].GetMatches();
                    Match[] ms2 = qualCL[5].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms,4, new DateTime(2018, 8, 7), new DateTime(2018, 8, 14));
                    AddEuropaCupMatchesToCalendar(ms2,5, new DateTime(2018, 8, 7), new DateTime(2018, 8, 14));
                }
                else if (currCLRound == 5)
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
                    qualCL.Add(new CupKnockoutStage("Champions Cup", cs, qualCL[4].GetWinners(), true, 2));
                    currCLRound++;
                    qualCL.Add(new CupKnockoutStage("Champions Cup", null, qualCL[5].GetWinners(), true, 2));
                    currCLRound++;
                    Match[] ms = qualCL[6].GetMatches();
                    Match[] ms2 = qualCL[7].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms,6, new DateTime(2018, 8, 21), new DateTime(2018, 8, 28));
                    AddEuropaCupMatchesToCalendar(ms2,7, new DateTime(2018, 8, 21), new DateTime(2018, 8, 28));
                }
                else if(currCLRound == 7)
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
                    pw.AddRange(qualCL[6].GetWinners());
                    pw.AddRange(qualCL[7].GetWinners());
                    qualCL.Add(new CupGroupStage("Champions Cup",8, new DateTime(2018, 9, 17),14,cs, pw));
                    currCLRound++;
                    /*Match[] ms = qualCL[8].GetMatches();
                    foreach (var item in ms)
                    {
                        Debug.LogWarning(item.Date + " " + item.FirstTeamId + " " + item.SecondTeamId);
                    }*/
                }
                else if(currCLRound == 8)                       // 1/8 finalu
                {
                    qualCL.Add(new CupKnockoutStage("Champions Cup", null, qualCL[8].GetWinners(), true, 2, true));
                    currCLRound++;
                    Match[] ms = qualCL[9].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms, 9, new DateTime(2019, 2, 19), new DateTime(2019, 3, 12));
                }
                else if (currCLRound == 9)                      // 1/4 finalu
                {
                    qualCL.Add(new CupKnockoutStage("Champions Cup", null, qualCL[9].GetWinners(), true, 1));
                    currCLRound++;
                    Match[] ms = qualCL[10].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms, 10, new DateTime(2019, 4, 9), new DateTime(2019, 4, 16));
                }
                else if (currCLRound == 10)                      // 1/2 finalu
                {
                    qualCL.Add(new CupKnockoutStage("Champions Cup", null, qualCL[10].GetWinners(), true, 1));
                    currCLRound++;
                    Match[] ms = qualCL[11].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms, 11, new DateTime(2019, 4, 30), new DateTime(2019, 5, 7));
                }
                else if (currCLRound == 11)                      // grand finale
                {
                    qualCL.Add(new CupKnockoutStage("Champions Cup", null, qualCL[11].GetWinners(), false, 1));
                    currCLRound++;
                    Match[] ms = qualCL[12].GetMatches();
                    //ms[0].Date = new DateTime(2019, 6, 28);
                    AddEuropaCupMatchesToCalendar(ms, 12, new DateTime(2019, 5, 28), new DateTime(), false);
                }
            }
        }
        currentMatch++;
        UpdateCalendar();
    }
    void UpdateCalendar()
    {
        // if current match is not in the same day that the first match of the day was we just set firstmatchoftheday to current match index
        // what it means is that we keep track of first match of the current day to list all of them in the list (ui vertical layout)
        if (currentMatch != 0 && matches[currentMatch-1].Date != currDate && matches[firstMatchOfTheWeek].Date != matches[currentMatch].Date) firstMatchOfTheWeek = currentMatch;
        int todayMatches = 0, currMatchOfTheDay = 0;
        DateTime nextMatchDay = matches[firstMatchOfTheWeek].Date;
        for (int i = firstMatchOfTheWeek; i < matches.Count; i++)
        {
            if (matches[i].Date == nextMatchDay) 
                todayMatches++;
            else 
                break;
        }

        string resultHelperString = "";

        foreach (Transform child in calendarParent.transform)
        {
            if (currMatchOfTheDay == todayMatches) 
                Destroy(child.gameObject);
            else
            {
                if (matches[firstMatchOfTheWeek + currMatchOfTheDay].Finished) 
                    resultHelperString = " " + matches[firstMatchOfTheWeek + currMatchOfTheDay].Result.HostGoals + " - " + matches[firstMatchOfTheWeek + currMatchOfTheDay].Result.GuestGoals + " ";
                else 
                    resultHelperString = " - ";
                child.GetComponentsInChildren<Text>()[1].text = GetClubNameByID(matches[firstMatchOfTheWeek + currMatchOfTheDay].FirstTeamId) + resultHelperString + GetClubNameByID(matches[firstMatchOfTheWeek + currMatchOfTheDay].SecondTeamId);
                currMatchOfTheDay++;
            }
        }
        for (int i = currMatchOfTheDay; i < todayMatches; i++)
        {
            GameObject go = Instantiate(calendarEntry, calendarParent.transform);
            if (matches[firstMatchOfTheWeek + currMatchOfTheDay].Finished) 
                resultHelperString = " " + matches[firstMatchOfTheWeek + currMatchOfTheDay].Result.HostGoals + " - " + matches[firstMatchOfTheWeek + currMatchOfTheDay].Result.GuestGoals + " ";
            else 
                resultHelperString = " - ";
            go.GetComponentsInChildren<Text>()[1].text = GetClubNameByID(matches[firstMatchOfTheWeek + i].FirstTeamId) + resultHelperString + GetClubNameByID(matches[firstMatchOfTheWeek + i].SecondTeamId);
        }
        
    }
    string GetClubNameByID(int id)
    {
        if (clubs != null)
        { 
            foreach (var c in clubs)
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
        qualCL.Add(new CupKnockoutStage("Champions Cup",cs, null, false));
        Match[] ms = qualCL[0].GetMatches();
        foreach (var m in ms)
        {
            //m.CompetitionName = "Champions Cup";
            m.Date = new DateTime(2018, 6, 25);
            m.RoundIndex = 0;
        }
        matches.InsertRange(0, ms);
    }
    public void CreateCupCalendar()
    {
        List<Club> cs = new List<Club>();
        int teams = 1;
        // look for a closest power of 2
        for (int i = 1; i < teamsNumber; i++)
        {
            if (teams*2 <= teamsNumber) teams *= 2;
            else break;
        }
        for (int i = 0; i < teams; i++)
        {
            cs.Add(Database.leagueDB[myLeagueID].Teams[i]);
        }
        nationalCup.Add(new CupKnockoutStage("National Cup",cs));
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
        AddEuropaCupMatchesToCalendar(nationalCup[0].GetMatches(), 0, cupDates[0], cupDates[0].AddDays(7));
    }
    void AddCupMatchesToCalendar(int indexOfRoundBeforeCup)
    {
        DateTime newDate;
        newDate = matches[indexOfRoundBeforeCup].Date.AddDays(3);
        Match[] m = nationalCup[currCupRound].GetMatches();
        int temp = 0;
        for (int i = 0; i < m.Length; i++)
        {
            //Debug.Log(Database.GetClubByID(nationalCup[0].GetMatches()[i].FirstTeamId).Name + " - " + Database.GetClubByID(nationalCup[0].GetMatches()[i].SecondTeamId).Name);
            if (i < m.Length / 2)
            {
                m[i].Date = newDate;
                temp = indexOfRoundBeforeCup + 1 + i;
                matches.Insert(temp, new Match(m[i]));
            }
            else
            {
                m[i].Date = newDate.AddDays(7);
                temp = indexOfRoundBeforeCup + 1 + teamsNumber / 2 + i;
                matches.Insert(temp, new Match(m[i]));
            }
        }
    }
    void AddEuropaCupMatchesToCalendar(Match[] ms,int roundInd, DateTime firstLeg, DateTime secondLeg,bool twoLeg = true)
    {
        if(twoLeg)
        {
            int firstLegIndex = -1, secondLegIndex = -1;
            for (int i = 0; i < matches.Count; i++)
            {
                if (firstLegIndex == -1)
                {
                    if (matches[i].Date > firstLeg)
                    {
                        firstLegIndex = i;
                        // zabezpieczenie w przypadku gdy nastepny mecz ma date wieksza od firsleg jak i od secondleg
                        if (matches[i].Date > secondLeg)
                        {
                            secondLegIndex = i + ms.Length / 2;
                            break;
                        }
                    }
                }
                else if (matches[i].Date > secondLeg)
                {
                    secondLegIndex = i + ms.Length / 2;
                    break;
                }
            }
            // zabezpieczenie przed bledem zwiazanym z pustym kalendarzem
            if(matches.Count == 0)
            {
                firstLegIndex = 0;
                secondLegIndex = ms.Length / 2;
            }
            // zabezpiecza przed przypadkiem w ktorym mecze odbywac sie beda na koncu kalendarza
            if (firstLegIndex == -1 && secondLegIndex == -1)
            {
                firstLegIndex = matches.Count;
                secondLegIndex = firstLegIndex + ms.Length / 2;
            }
            if (secondLegIndex == -1)
            {
                secondLegIndex = matches.Count + ms.Length / 2;
            }
            for (int i = 0; i < ms.Length / 2; i++)
            {
                ms[i].Date = firstLeg;
                ms[i].RoundIndex = roundInd;
                matches.Insert(firstLegIndex, ms[i]);
            }
            for (int i = ms.Length / 2; i < ms.Length; i++)
            {
                ms[i].Date = secondLeg;
                ms[i].RoundIndex = roundInd;
                matches.Insert(secondLegIndex, ms[i]);
            }
        }
        else
        {
            foreach (var item in ms)
            {
                item.Date = firstLeg;
                item.RoundIndex = roundInd;
            }
            if(matches.Count == 0)
            {
                matches.InsertRange(0, ms);
                return;
            }
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].Date > firstLeg)
                {
                    matches.InsertRange(i, ms);
                    return;
                }
            }
            // w razie gdyby data byla koncowka rozgrywek czyli np liga mistrzow final to
            matches.InsertRange(matches.Count, ms);
        }
    }
    public static int GetCurrentMatchIndex()
    {
        return currentMatch;
    }
    void ClearSquadCells()
    {
        for (int x = 0; x < squadPanelsParent.transform.childCount; x++)
        {
            int limit = squadPanelsParent.transform.GetChild(x).childCount;
            for (int i = limit - 1; i >= 0; i--)
            {
                Destroy(squadPanelsParent.transform.GetChild(x).GetChild(i).gameObject);
            }
        }
        int end = substitutesParent.transform.childCount;
        for (int i = end - 1; i >= 0; i--)
        {
            Destroy(substitutesParent.transform.GetChild(i).gameObject);
        }
    }
    public void FormationDropDownValue(int val)
    {
        //ClearSquadCells();
        UpdateSquadScreen(formationDropDown.options[val].text);
    }
    public void SetUpSquadScreen()
    {
        // set dropdown value to current formation
        for (int i = 0; i < formationDropDown.options.Count; i++)
        {
            if(formationDropDown.options[i].text == Database.clubDB[myClubID].Formation)
            {
                formationDropDown.value = i;
                break;
            }

        }
        UpdateSquadScreen();
    }
    public void UpdateSquadScreen(string formation = "")
    {
        ClearSquadCells();
        for (int i = 11; i < Database.clubDB[myClubID].FootballersIDs.Count; i++)
        {
            GameObject g = Instantiate(squadCell, substitutesParent.transform);
            g.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[i]]);
        }
        if(formation == "")formation = Database.clubDB[myClubID].Formation;
        GameObject go;
        if (formation == "4-3-3")
        {
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[6]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[5]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[7]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[8]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[10]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[9]]);
        }
        else if (formation == "4-2-3-1")
        {
            //go = Instantiate(emptyCell, squadPanelsParent.transform.GetChild(2));
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[5]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[6]]);
            //go = Instantiate(emptyCell, squadPanelsParent.transform.GetChild(2));
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[8]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[7]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[9]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[10]]);

        }
        else if (formation == "4-4-1-1")
        {
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[7]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[5]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[6]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[8]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[9]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[10]]);
        }
        else if (formation == "4-1-4-1")
        {
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[5]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[8]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[6]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[7]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[9]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[10]]);
        }
        else if (formation == "4-3-1-2")
        {
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[6]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[5]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[7]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[8]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[9]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[10]]);
        }
        else if (formation == "4-4-2")
        {
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[7]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[5]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[6]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[8]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[9]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[10]]);

        }
        else if (formation == "5-3-2")
        {
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[7]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[6]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[8]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[9]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[10]]);
        }
        else if (formation == "3-4-1-2")
        {
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[6]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[4]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[5]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[7]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[8]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[9]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[10]]);
        }
        else if(formation == "3-4-2-1")
        {
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[6]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[4]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[5]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[7]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[8]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[9]]);
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[10]]);
        }
        for (int i = 1; i <= int.Parse(formation[0].ToString()); i++)
        {
            go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(4));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[i]]);
        }
        go = Instantiate(squadCell, squadPanelsParent.transform.GetChild(5));
        go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[myClubID].FootballersIDs[0]]);
        //squadWindowNeedsUpdate = false;
    }

    public void SetMySquad()
    {
        string formation = formationDropDown.options[formationDropDown.value].text;
        for (int i = 11; i < Database.clubDB[myClubID].FootballersIDs.Count; i++)
        {
            Database.clubDB[myClubID].FootballersIDs[i] = substitutesParent.transform.GetChild(i-11).GetComponent<SquadCell>().GetFootballerID();
        }
        // save
        Database.clubDB[myClubID].FootballersIDs[0] = squadPanelsParent.transform.GetChild(5).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
        int defendersCount = int.Parse(formation[0].ToString());
        for (int i = 0; i < defendersCount; i++)
        {
            Database.clubDB[myClubID].FootballersIDs[i+1] = squadPanelsParent.transform.GetChild(4).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
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
                    Database.clubDB[myClubID].FootballersIDs[defendersCount+i] = squadPanelsParent.transform.GetChild(2).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[myClubID].FootballersIDs[defendersCount + inTheMiddle + 1] = squadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[myClubID].FootballersIDs[defendersCount + inTheMiddle + 2] = squadPanelsParent.transform.GetChild(2).GetChild(midFieldNumber-1).GetComponent<SquadCell>().GetFootballerID();
            }
            int attackersNumber = int.Parse(formation[4].ToString());
            int defAndMidCount = defendersCount + midFieldNumber;
            if (attackersNumber == 1)
            {
                Database.clubDB[myClubID].FootballersIDs[10] = squadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = attackersNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[myClubID].FootballersIDs[defAndMidCount + inTheMiddle + i + 1] = squadPanelsParent.transform.GetChild(0).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[myClubID].FootballersIDs[defAndMidCount + 1] = squadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[myClubID].FootballersIDs[defAndMidCount + 2] = squadPanelsParent.transform.GetChild(0).GetChild(attackersNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
        }
        else if(formation.Length == 7)
        {
            int defmidFieldNumber = int.Parse(formation[2].ToString());
            if (defmidFieldNumber == 1)
            {
                // 4-1-3-2
                Database.clubDB[myClubID].FootballersIDs[defendersCount + 1] = squadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = defmidFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[myClubID].FootballersIDs[defendersCount + i] = squadPanelsParent.transform.GetChild(2).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[myClubID].FootballersIDs[defendersCount + inTheMiddle + 1] = squadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[myClubID].FootballersIDs[defendersCount + inTheMiddle + 2] = squadPanelsParent.transform.GetChild(2).GetChild(defmidFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int midFieldNumber = int.Parse(formation[4].ToString());
            if (midFieldNumber == 1)
            {
                // 4-3-1-2
                Database.clubDB[myClubID].FootballersIDs[defendersCount + defmidFieldNumber + 1] = squadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = midFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[myClubID].FootballersIDs[defendersCount + defmidFieldNumber + i] = squadPanelsParent.transform.GetChild(1).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[myClubID].FootballersIDs[defendersCount + defmidFieldNumber + inTheMiddle + 1] = squadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[myClubID].FootballersIDs[defendersCount + defmidFieldNumber + inTheMiddle + 2] = squadPanelsParent.transform.GetChild(1).GetChild(midFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int attackersNumber = int.Parse(formation[6].ToString());
            int defAndMidCount = defendersCount + defmidFieldNumber + midFieldNumber;
            if (attackersNumber == 1)
            {
                Database.clubDB[myClubID].FootballersIDs[10] = squadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = attackersNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[myClubID].FootballersIDs[defAndMidCount + inTheMiddle + i] = squadPanelsParent.transform.GetChild(0).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[myClubID].FootballersIDs[defAndMidCount + 1] = squadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[myClubID].FootballersIDs[defAndMidCount + 2] = squadPanelsParent.transform.GetChild(0).GetChild(attackersNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
        }
        else if (formation.Length == 9)
        {
            int defmidFieldNumber = int.Parse(formation[2].ToString());
            if (defmidFieldNumber == 1)
            {
                // 4-1-2-1-2
                Database.clubDB[myClubID].FootballersIDs[defendersCount + 1] = squadPanelsParent.transform.GetChild(3).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = defmidFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[myClubID].FootballersIDs[defendersCount + i] = squadPanelsParent.transform.GetChild(3).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[myClubID].FootballersIDs[defendersCount + inTheMiddle + 1] = squadPanelsParent.transform.GetChild(3).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[myClubID].FootballersIDs[defendersCount + inTheMiddle + 2] = squadPanelsParent.transform.GetChild(3).GetChild(defmidFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int midFieldNumber = int.Parse(formation[4].ToString());
            int defmidAndDefCount = defendersCount + defmidFieldNumber;
            if (midFieldNumber == 1)
            {
                // 4-2-1-2-1
                Database.clubDB[myClubID].FootballersIDs[defmidAndDefCount + 1] = squadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = midFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[myClubID].FootballersIDs[defendersCount + i] = squadPanelsParent.transform.GetChild(2).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[myClubID].FootballersIDs[defmidAndDefCount + inTheMiddle + 1] = squadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[myClubID].FootballersIDs[defmidAndDefCount + inTheMiddle + 2] = squadPanelsParent.transform.GetChild(2).GetChild(midFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int atkMidFieldNumber = int.Parse(formation[6].ToString());
            int midAndDefCount = defmidAndDefCount + midFieldNumber;
            if (atkMidFieldNumber == 1)
            {
                // 4-1-3-1-2
                Database.clubDB[myClubID].FootballersIDs[midAndDefCount + 1] = squadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = atkMidFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[myClubID].FootballersIDs[midAndDefCount + i] = squadPanelsParent.transform.GetChild(1).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[myClubID].FootballersIDs[midAndDefCount + inTheMiddle + 1] = squadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[myClubID].FootballersIDs[midAndDefCount + inTheMiddle + 2] = squadPanelsParent.transform.GetChild(1).GetChild(atkMidFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int attackersNumber = int.Parse(formation[8].ToString());
            int defAndMidCount = midAndDefCount + atkMidFieldNumber;
            if (attackersNumber == 1)
            {
                Database.clubDB[myClubID].FootballersIDs[10] = squadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = attackersNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[myClubID].FootballersIDs[defAndMidCount + inTheMiddle + i] = squadPanelsParent.transform.GetChild(0).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[myClubID].FootballersIDs[defAndMidCount + 1] = squadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[myClubID].FootballersIDs[defAndMidCount + 2] = squadPanelsParent.transform.GetChild(0).GetChild(attackersNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
        }
        Database.clubDB[myClubID].Formation = formation;
        WindowsManager.Instance.ShowWindow("Club Menu");
    }
}
