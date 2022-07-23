using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupRound
{
    protected bool twoLeg;
    protected int numberOfBaskets;
    protected List<Club> clubs;
    protected List<Club> winners;
    protected List<Club> loosers;
    protected List<Match> matches;
    protected int finishedMatches = 0;
    protected string competitionName;
    public CupRound(string competitionName, List<Club> clubs, List<Club> prevRoundClubs, bool twoLeg, int numberOfBaskets)
    {
        this.competitionName = competitionName;
        if (clubs == null && prevRoundClubs == null)
        {
            Debug.LogError("Clubs and previousRoundClubs are empty. No clubs to play.");
            return;
        }
        if (clubs != null) this.clubs = clubs;
        else this.clubs = new List<Club>();
        // add every club from previous round to the clubs
        if (prevRoundClubs != null)
        {
            foreach (var c in prevRoundClubs)
            {
                this.clubs.Add(c);
            }
        }
        this.twoLeg = twoLeg;
        this.numberOfBaskets = numberOfBaskets;
        winners = new List<Club>();
        loosers = new List<Club>();
        matches = new List<Match>();
    }
    public Match[] GetMatches()
    {
        return matches.ToArray();
    }
    // gets resut of the match and enters it into the list, return true if all matches finished 
    public virtual void SendMatchResult(Match m)
    {
        //Debug.LogWarning("SMR");
        finishedMatches++;
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].FirstTeamId == m.FirstTeamId && matches[i].SecondTeamId == m.SecondTeamId)
            {
                matches[i].Result = m.Result;
                break;
            }
        }
    }
    public virtual List<Club> GetWinners() { return winners; }
    
}
