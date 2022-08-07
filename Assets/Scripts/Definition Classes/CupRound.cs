using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupRound
{
    protected bool _twoLeg;
    protected int _numberOfBaskets;
    protected List<Club> _clubs;
    protected List<Club> _winners;
    protected List<Club> _loosers;
    protected List<Match> _matches;
    protected int _finishedMatches = 0;
    protected string _competitionName;
    protected float _winRankingPoints;
    protected float _drawRankingPoints;
    public CupRound(string competitionName, List<Club> clubs, List<Club> prevRoundClubs, bool twoLeg, int numberOfBaskets, float participationRankingPoints = 0, float winRankingPoints = 0, float drawRankingPoints = 0)
    {
        _winRankingPoints = winRankingPoints;
        _drawRankingPoints = drawRankingPoints;
        _competitionName = competitionName;
        if (clubs == null && prevRoundClubs == null)
        {
            Debug.LogError("Clubs and previousRoundClubs are empty. No clubs to play.");
            return;
        }

        _clubs = clubs ?? new List<Club>();

        // add every club from previous round to the clubs
        if (prevRoundClubs != null)
            _clubs.AddRange(prevRoundClubs);

        _twoLeg = twoLeg;
        _numberOfBaskets = numberOfBaskets;
        _winners = new List<Club>();
        _loosers = new List<Club>();
        _matches = new List<Match>();

        // points for participation
        if(participationRankingPoints > 0)
            _clubs.ForEach(club => club.AddRankingPoints(participationRankingPoints));
    }
    public Match[] GetMatches() => _matches.ToArray();

    // gets resut of the match and enters it into the list, return true if all matches finished 
    public virtual void SendMatchResult(Match m)
    {
        _finishedMatches++;
        for (int i = 0; i < _matches.Count; i++)
        {
            if (_matches[i].FirstTeamId == m.FirstTeamId && _matches[i].SecondTeamId == m.SecondTeamId)
            {
                _matches[i].Result = m.Result;
                if (m.Result.HostGoals == m.Result.GuestGoals)
                {
                    if (_drawRankingPoints > 0)
                    {
                        Database.clubDB[m.FirstTeamId].AddRankingPoints(_drawRankingPoints);
                        Database.clubDB[m.SecondTeamId].AddRankingPoints(_drawRankingPoints);
                    }
                }
                else
                {
                    if (m.Result.HostGoals > m.Result.GuestGoals)
                        Database.clubDB[m.FirstTeamId].AddRankingPoints(_winRankingPoints);
                    else
                        Database.clubDB[m.SecondTeamId].AddRankingPoints(_winRankingPoints);
                }
                break;
            }
        }
    }
    public virtual List<Club> GetWinners() { return _winners; }
    
}
