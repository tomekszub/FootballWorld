using System.Collections.Generic;
using UnityEngine;

public class CupRound
{
    public class EuropaTournamentData
    {
        public bool TwoLeg;
        public int NumberOfBaskets;
        public float ParticipationRankingPoints;
        public float WinRankingPoints;
        public float DrawRankingPoints;

        public EuropaTournamentData(bool twoLeg = true, int numberOfBaskets = 1, float participationRankingPoints = 0, float winRankingPoints = 0, float drawRankingPoints = 0)
        {
            TwoLeg = twoLeg;
            NumberOfBaskets = numberOfBaskets;
            ParticipationRankingPoints = participationRankingPoints;
            WinRankingPoints = winRankingPoints;
            DrawRankingPoints = drawRankingPoints;
        }
    }

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
    public CupRound(string competitionName, List<Club> clubs, EuropaTournamentData tournamentData)
    {
        _winRankingPoints = tournamentData.WinRankingPoints;
        _drawRankingPoints = tournamentData.DrawRankingPoints;
        _competitionName = competitionName;
        if (clubs == null)
        {
            Debug.LogError("Clubs are empty. No clubs to play.");
            return;
        }
        _clubs = clubs;

        _twoLeg = tournamentData.TwoLeg;
        _numberOfBaskets = tournamentData.NumberOfBaskets;
        _winners = new List<Club>();
        _loosers = new List<Club>();
        _matches = new List<Match>();

        // points for participation
        if(tournamentData.ParticipationRankingPoints > 0)
            _clubs.ForEach(club => club.AddRankingPoints(tournamentData.ParticipationRankingPoints));
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
    public virtual List<Club> GetLoosers() { return _loosers; }
    
}
