using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class ClubAvailability
{
    public Club c;
    public int val;
    public ClubAvailability(Club cl, int v)
    {
        c = cl;
        val = v;
    }
}

public class CupGroupStage : CupRound
{
    int ClubsInGroup, RoundIndex, BreakBetweenRounds;
    List<Club[]> group;
    List<List<Team>> groupTable;
    
    public CupGroupStage(string competitionName,int roundIndex, DateTime start,int breakBetweenRounds, List<Club> clubs, List<Club> prevRoundClubs = null, int clubsInGroup = 4) : base(competitionName, clubs, prevRoundClubs, true, 0)
    {
        ClubsInGroup = clubsInGroup;
        RoundIndex = roundIndex;
        BreakBetweenRounds = breakBetweenRounds;
        CreateGroups();
        CreateMatches(start);
    }
    void CreateGroups()
    {
        group = new List<Club[]>();
        int groupsCount = clubs.Count / ClubsInGroup;
        for (int i = 0; i < groupsCount; i++)
        {
            group.Add(new Club[ClubsInGroup]);
        }
        clubs = clubs.OrderByDescending(n => n.GetRankingPoints()).ToList();
        List<List<Club>> pot = new List<List<Club>>();
        for (int i = 0; i < ClubsInGroup; i++)
        {
            pot.Add(new List<Club>());
        }
        for (int i = 0; i < clubs.Count; i++)
        {
            pot[i / groupsCount].Add(clubs[i]);
        }
        int fuse = 3;
        bool ok;
        while (fuse > 0)
        {
            ok = true;

            foreach (Club c in pot[0])
            {
                group[GetRandomAvailableGroup(0, c.CountryName, true)][0] = c;
            }
            for (int p = 1; p < pot.Count; p++)
            {
                DrawFromAPot(p, pot[p]);
            }
            
            for (int i = 0; i < group.Count; i++)
            {
                for (int x = 0; x < group[i].Length; x++)
                {
                    if (group[i][x] == null) 
                        ok = false;
                }
            }
            if (ok) 
                break;
            fuse--;
        }
    }
    void DebugGroups()
    {
        for (int i = 0; i < group.Count; i++)
        {
            Debug.LogError("GRUPA " + (i + 1));
            for (int x = 0; x < groupTable[i].Count; x++)
            {
                Debug.LogWarning(groupTable[i][x].Name + " " + groupTable[i][x].Points);
            }
        }
    }
    void CreateMatches(DateTime start)
    {
        groupTable = new List<List<Team>>();
        for (int i = 0; i < group.Count; i++)
        {
            List<Team> t = new List<Team>();
            for (int x = 0; x < group[i].Length; x++)
            {
                t.Add(new Team(group[i][x].Id, group[i][x].Name));
            }
            matches.AddRange(MatchCalendar.CreateGroupCalendar(competitionName,RoundIndex, group[i].ToList(), start, BreakBetweenRounds, group.Count / 4));
            groupTable.Add(t);
        }
        matches = matches.ToArray().OrderBy(m => m.Date).ToList();
    }
    int GetRandomAvailableGroup(int pot, string country, bool returnGroupIndex)
    {
        List<int> availableGroups = new List<int>();
        bool ok;
        for (int g = 0; g < group.Count; g++)
        {
            ok = true;
            // sprawdzamy czy jest wolne mejsce w grupie, jesli nie to kolejna grupa
            if (group[g][pot] != null) 
                continue;
            // od 0 do pot, jesli pot 0 to nie mamy co sprawdzac innych zepsoplow z tej grupy 
            //bo aktualny zespol bedzie pierwszym dodanym
            for (int i = 0; i < pot; i++)
            {
                if (group[g][i].CountryName == country)
                {
                    ok = false;
                    break;
                }
            }
            if (ok) 
                availableGroups.Add(g);
        }

        if (!returnGroupIndex)
            return availableGroups.Count;

        int randomIndex = UnityEngine.Random.Range(0, availableGroups.Count - 1);
        return availableGroups.Count>0?availableGroups[randomIndex]:-1;
    }
    void DrawFromAPot(int pot, List<Club> lastPotContent)
    {
        List<ClubAvailability> cas = new List<ClubAvailability>();
        for (int i = 0; i < lastPotContent.Count; i++)
        {
            cas.Add(new ClubAvailability(lastPotContent[i], GetRandomAvailableGroup(pot, lastPotContent[i].CountryName, false)));
        }
        cas = cas.ToArray().OrderBy(n => n.val).ToList();
        while (cas.Count != 0)
        {
            int g = GetRandomAvailableGroup(pot, cas[0].c.CountryName, true);
            if (g == -1)
            {
                Debug.LogError("Nie mozna znalezc pasujacego miejsca...");
                return;
            }
            group[g][pot] = cas[0].c;
            cas.RemoveAt(0);
            if (cas.Count > 0 && cas[0].val != 1)
            {
                for (int i = 0; i < cas.Count; i++)
                {
                    cas[i].val = GetRandomAvailableGroup(pot, cas[i].c.CountryName, false);
                }
                cas = cas.ToArray().OrderBy(n => n.val).ToList();
            }
        }
    }
    public override void SendMatchResult(Match m)
    {
        finishedMatches++;
        int matchID = -1;
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].FirstTeamId == m.FirstTeamId && matches[i].SecondTeamId == m.SecondTeamId)
            {
                matches[i].Result = m.Result;
                matchID = i;
                break;
            }
        }
        AdvanceDecider.WhoIsWinning who = AdvanceDecider.NormalMatch(matches[matchID].Result);
        int groupID = -1;
        for (int i = 0; i < groupTable.Count; i++)
        {
            for (int c = 0; c < groupTable[i].Count; c++)
            {
                if(matches[matchID].FirstTeamId == groupTable[i][c].Id)
                {
                    if (who == AdvanceDecider.WhoIsWinning.Host)
                    {
                        groupTable[i][c].Points += 3;
                        groupTable[i][c].Wins++;
                    }
                    else if (who == AdvanceDecider.WhoIsWinning.Draw)
                    {
                        groupTable[i][c].Points += 1;
                        groupTable[i][c].Draws++;
                    }
                    else
                    {
                        groupTable[i][c].Loses++;
                    }
                    groupTable[i][c].MatchesPlayed++;
                    groupTable[i][c].ScoredGoals += matches[matchID].Result.HostGoals;
                    groupTable[i][c].LostGoals += matches[matchID].Result.GuestGoals;
                    groupTable[i][c].DifferenceGoals += matches[matchID].Result.HostGoals - matches[matchID].Result.GuestGoals;
                    if (groupID == -1) 
                        groupID = i;
                    else 
                        break;
                }
                else if(matches[matchID].SecondTeamId == groupTable[i][c].Id)
                {
                    if (who == AdvanceDecider.WhoIsWinning.Guest)
                    {
                        groupTable[i][c].Points += 3;
                        groupTable[i][c].Wins++;
                    }
                    else if (who == AdvanceDecider.WhoIsWinning.Draw)
                    {
                        groupTable[i][c].Points += 1;
                        groupTable[i][c].Draws++;
                    }
                    else
                    {
                        groupTable[i][c].Loses++;
                    }
                    groupTable[i][c].MatchesPlayed++;
                    groupTable[i][c].ScoredGoals += matches[matchID].Result.GuestGoals;
                    groupTable[i][c].LostGoals += matches[matchID].Result.HostGoals;
                    groupTable[i][c].DifferenceGoals += matches[matchID].Result.GuestGoals - matches[matchID].Result.HostGoals;
                    if (groupID == -1) 
                        groupID = i;
                    else 
                        break;
                }
            }
            if (groupID != -1) 
                break;
        }
        groupTable[groupID] = Table.SortTeamsTableByGoalDifference(groupTable[groupID]);
        //DebugGroups();
    }
    bool DetermineWhoWon()
    {
        //check if all are finished
        if (finishedMatches != matches.Count) 
            return false;
        winners.Clear();
        for (int i = 0; i < groupTable.Count; i++)
        {
            for (int c = 0; c < group[i].Length; c++)
            {
                if (group[i][c].Id == groupTable[i][0].Id || group[i][c].Id == groupTable[i][1].Id) 
                    winners.Add(group[i][c]);
                else if (group[i][c].Id == groupTable[i][2].Id) 
                    loosers.Add(group[i][c]);
            }
        }
        return true;
    }
    public override List<Club> GetWinners()
    {
        if (!DetermineWhoWon()) 
            return null;
        return base.GetWinners();
    }
}

