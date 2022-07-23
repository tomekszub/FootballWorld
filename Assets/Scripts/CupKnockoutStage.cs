using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CupKnockoutStage : CupRound
{
    bool AfterGroupStage;
    /// <summary>
    /// Constructor for knockout stage for any competition.
    /// </summary>
    /// <param name="competitionName"></param>
    /// <param name="clubs"></param>
    /// <param name="prevRoundClubs"></param>
    /// <param name="twoLeg"></param>
    /// <param name="numberOfBaskets"></param>
    /// <param name="afterGroupStage">If true clubs will be treated as teams returned by groupStage, meaning that 0,2,4,etc will be group winners, 1,3,5,etc 2nd place</param>
    public CupKnockoutStage(string competitionName, List<Club> clubs, List<Club> prevRoundClubs = null, bool twoLeg = true, int numberOfBaskets = 0, bool afterGroupStage = false) : base(competitionName, clubs, prevRoundClubs, twoLeg, numberOfBaskets)
    {
        AfterGroupStage = afterGroupStage;
        CreateMatches();
    }
    void CreateMatches()
    {
        
        if(numberOfBaskets < 2)
        {
            int r;
            int clubsCount = clubs.Count;
            for (int i = 0; i < clubsCount; i = i + 2)
            {
                r = UnityEngine.Random.Range(0, clubsCount-i);
                Match m = new Match("",DateTime.Now, 0, clubs[r].Id, 0);
                // its just moving the r-th element to the end
                clubs.Add(clubs[r]);
                clubs.RemoveAt(r);
                // now second team, PS. it ain't pretty but it's an honest work
                r = UnityEngine.Random.Range(0, clubsCount - i - 1);
                m.SecondTeamId = clubs[r].Id;
                clubs.Add(clubs[r]);
                clubs.RemoveAt(r);
                m.CompetitionName = competitionName;
                matches.Add(m);
            }
        }
        else
        {
            if(AfterGroupStage == false)clubs = clubs.ToArray().OrderByDescending(n => n.GetRankingPoints()).ToList();
            else
            {

                List<Club> winners = new List<Club>();
                List<Club> secondPlace = new List<Club>();
                for (int i = 0; i < clubs.Count; i++)
                {
                    if (i % 2 == 0) winners.Add(clubs[i]);
                    else secondPlace.Add(clubs[i]);
                }
                clubs.Clear();
                clubs.AddRange(winners);
                clubs.AddRange(secondPlace);
            }
            
            foreach (Club c in clubs)
            {
                Debug.Log(c.Name + " :  " + c.GetRankingPoints());
            }
            
            int r, half;
            int clubsCount = clubs.Count;
            for (int i = 0; i < clubsCount/2; i = i + 1)
            {
                half = clubsCount / 2 - i;
                r = UnityEngine.Random.Range(0, half);
                //Debug.LogWarning("Mecz: " + r + " z range 0 - " + half);
                Match m = new Match("", DateTime.Now, 0, clubs[r].Id, 0);
                // its just moving the r-th element to the end
                clubs.Add(clubs[r]);
                clubs.RemoveAt(r);
                // now second team, PS. it ain't pretty but it's an honest work
                r = UnityEngine.Random.Range(half-1, clubsCount - i*2 - 1);
                //Debug.LogWarning(r + " z range " + (half-1) + " - " + (clubsCount - i - 1));
                m.SecondTeamId = clubs[r].Id;
                clubs.Add(clubs[r]);
                clubs.RemoveAt(r);
                m.CompetitionName = competitionName;
                matches.Add(m);
            }
        }
        
        if(twoLeg)
        {
            int matchesCount = matches.Count;
            // jesli final to nie bedize rewanzu
            if (matchesCount == 1) return;
            for (int i = 0; i < matchesCount; i++)
            {
                matches.Add(matches[i].InvertTeams());
            }
        }
    }
    bool DetermineWhoWon()
    {
        //check if all are finished
        if (finishedMatches != matches.Count) 
            return false;
        winners.Clear();
        if (twoLeg)
        {
            int firstLegCount = matches.Count / 2;
            bool oneFound = false;
            for (int i = 0; i < firstLegCount; i++)
            {
                AdvanceDecider.WhoIsWinning who = AdvanceDecider.NormalTwoLeg(matches[i].Result, matches[firstLegCount + i].Result);
                //TODO considering penalties normaltwoleg is invalid, for now penalties are offline so as below we have host winning when a draw happens
                if (who == AdvanceDecider.WhoIsWinning.Draw || who == AdvanceDecider.WhoIsWinning.Host)
                {
                    foreach (var c in clubs)
                    {
                        if (c.Id == matches[i].SecondTeamId)
                        {
                            winners.Add(c);
                            if (oneFound) break;
                            oneFound = true;
                        }
                        else if (c.Id == matches[i].FirstTeamId)
                        {
                            loosers.Add(c);
                            if (oneFound) break;
                            oneFound = true;
                        }
                    }
                }
                else
                {
                    foreach (var c in clubs)
                    {
                        if (c.Id == matches[i].FirstTeamId)
                        {
                            winners.Add(c);
                            if (oneFound) break;
                            oneFound = true;
                        }
                        else if (c.Id == matches[i].SecondTeamId)
                        {
                            loosers.Add(c);
                            if (oneFound) break;
                            oneFound = true;
                        }
                    }
                }
                oneFound = false;
                // przenoszenie rozstawienia (wspolczynnika rankingowego, ranking points)
                winners.Last().SetRankingPoints(loosers.Last().GetRankingPoints());
            }
        }
        else
        {
            int matchesCount = matches.Count;
            bool oneFound = false;
            for (int i = 0; i < matchesCount; i++)
            {
                // TODO: same situation, right now we dont have penalties so draw is a host win
                if (matches[i].Result.HostGoals > matches[i].Result.GuestGoals || matches[i].Result.HostGoals == matches[i].Result.GuestGoals)
                {
                    foreach (var c in clubs)
                    {
                        if (c.Id == matches[i].FirstTeamId)
                        {
                            winners.Add(c);
                            if (oneFound) break;
                            oneFound = true;
                        }
                        else if (c.Id == matches[i].SecondTeamId)
                        {
                            loosers.Add(c);
                            if (oneFound) break;
                            oneFound = true;
                        }
                    }
                }
                else
                {
                    foreach (var c in clubs)
                    {
                        if (c.Id == matches[i].SecondTeamId)
                        {
                            winners.Add(c);
                            if (oneFound) break;
                            oneFound = true;
                        }
                        else if (c.Id == matches[i].FirstTeamId)
                        {
                            loosers.Add(c);
                            if (oneFound) break;
                            oneFound = true;
                        }
                    }
                }
                oneFound = false;
                winners.Last().SetRankingPoints(loosers.Last().GetRankingPoints());
            }
        }
        return true;
    }
    public override List<Club> GetWinners()
    {
        if (!DetermineWhoWon()) return null; 
        return base.GetWinners();
    }
    public override void SendMatchResult(Match m)
    {
        base.SendMatchResult(m);
    }
}
