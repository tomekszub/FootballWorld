using System;
using System.Collections.Generic;
using System.Linq;
public class CupKnockoutStage : CupRound
{
    bool AfterGroupStage;
    /// <summary>
    /// Constructor for knockout stage for any competition.
    /// </summary>
    /// <param name="competitionName"></param>
    /// <param name="clubs"></param>
    /// <param name="higherCupClubs">Loosers from higher tier cups</param>
    /// <param name="twoLeg"></param>
    /// <param name="numberOfBaskets"></param>
    /// <param name="afterGroupStage">If true clubs will be treated as teams returned by groupStage, meaning that 0,2,4,etc will be group winners, 1,3,5,etc 2nd place</param>
    public CupKnockoutStage(string competitionName, List<Club> clubs, EuropaTournamentData tournamentData, List<Club> higherCupClubs = null, bool afterGroupStage = false): base(competitionName, clubs, tournamentData)
    {
        AfterGroupStage = afterGroupStage;
        CreateMatches(higherCupClubs);
    }
    void CreateMatches(List<Club> higherCupClubs = null)
    {
        if(higherCupClubs == null)
            higherCupClubs= new List<Club>();

        if(_numberOfBaskets < 2)
        {
            _clubs.AddRange(higherCupClubs);
            int r;
            int clubsCount = _clubs.Count;
            for (int i = 0; i < clubsCount; i += 2)
            {
                r = UnityEngine.Random.Range(0, clubsCount-i);
                Match m = new Match(_competitionName, DateTime.Now, 0, _clubs[r].Id, 0);
                // its just moving the r-th element to the end
                _clubs.Add(_clubs[r]);
                _clubs.RemoveAt(r);
                // now second team, PS. it ain't pretty but it's an honest work
                r = UnityEngine.Random.Range(0, clubsCount - i - 1);
                m.SecondTeamId = _clubs[r].Id;
                _clubs.Add(_clubs[r]);
                _clubs.RemoveAt(r);
                _matches.Add(m);
            }
        }
        else
        {
            if (AfterGroupStage == false)
            {
                _clubs.AddRange(higherCupClubs);
                _clubs = _clubs.OrderByDescending(n => n.RankingPoints).ToList();
            }
            else
            {
                // playoffs after grup stage
                if (higherCupClubs.Count > 0)
                {
                    _clubs = _clubs.OrderByDescending(n => n.RankingPoints).ToList();
                    higherCupClubs = higherCupClubs.OrderByDescending(n => n.RankingPoints).ToList();
                    _clubs.AddRange(higherCupClubs);
                }
                else // normal after group knockout
                {
                    List<Club> winners = new List<Club>();
                    List<Club> secondPlace = new List<Club>();
                    for (int i = 0; i < _clubs.Count; i++)
                    {
                        if (i % 2 == 0)
                            winners.Add(_clubs[i]);
                        else
                            secondPlace.Add(_clubs[i]);
                    }
                    _clubs = winners;
                    _clubs.AddRange(secondPlace);
                }
            }
            
            int r, half;
            int clubsCount = _clubs.Count;
            for (int i = 0; i < clubsCount/2; i = i + 1)
            {
                half = clubsCount / 2 - i;
                r = UnityEngine.Random.Range(0, half);
                Match m = new Match("", DateTime.Now, 0, _clubs[r].Id, 0);
                // its just moving the r-th element to the end
                _clubs.Add(_clubs[r]);
                _clubs.RemoveAt(r);
                // now second team, PS. it ain't pretty but it's an honest work
                r = UnityEngine.Random.Range(half-1, clubsCount - i*2 - 1);
                m.SecondTeamId = _clubs[r].Id;
                _clubs.Add(_clubs[r]);
                _clubs.RemoveAt(r);
                m.CompetitionName = _competitionName;
                _matches.Add(m);
            }
        }
        
        if(_twoLeg)
        {
            int matchesCount = _matches.Count;
            // jesli final to nie bedize rewanzu
            if (matchesCount == 1) 
                return;
            for (int i = 0; i < matchesCount; i++)
            {
                _matches.Add(_matches[i].InvertTeams());
            }
        }
    }
    bool DetermineWhoWon()
    {
        //check if all are finished
        if (_finishedMatches != _matches.Count) 
            return false;
        _winners.Clear();
        if (_twoLeg)
        {
            int firstLegCount = _matches.Count / 2;
            bool oneFound = false;
            for (int i = 0; i < firstLegCount; i++)
            {
                AdvanceDecider.WhoIsWinning who = AdvanceDecider.NormalTwoLeg(_matches[i].Result, _matches[firstLegCount + i].Result);
                //TODO considering penalties normaltwoleg is invalid, for now penalties are offline so as below we have host winning when a draw happens
                if (who == AdvanceDecider.WhoIsWinning.Draw || who == AdvanceDecider.WhoIsWinning.Host)
                {
                    foreach (var c in _clubs)
                    {
                        if (c.Id == _matches[i].SecondTeamId)
                        {
                            _winners.Add(c);
                            if (oneFound) 
                                break;
                            oneFound = true;
                        }
                        else if (c.Id == _matches[i].FirstTeamId)
                        {
                            _loosers.Add(c);
                            if (oneFound) 
                                break;
                            oneFound = true;
                        }
                    }
                }
                else
                {
                    foreach (var c in _clubs)
                    {
                        if (c.Id == _matches[i].FirstTeamId)
                        {
                            _winners.Add(c);
                            if (oneFound) 
                                break;
                            oneFound = true;
                        }
                        else if (c.Id == _matches[i].SecondTeamId)
                        {
                            _loosers.Add(c);
                            if (oneFound) 
                                break;
                            oneFound = true;
                        }
                    }
                }
                oneFound = false;
                // przenoszenie rozstawienia (wspolczynnika rankingowego, ranking points)
                //winners.Last().SetRankingPoints(loosers.Last().GetRankingPoints());
            }
        }
        else
        {
            int matchesCount = _matches.Count;
            bool oneFound = false;
            for (int i = 0; i < matchesCount; i++)
            {
                // TODO: same situation, right now we dont have penalties so draw is a host win
                if (_matches[i].Result.HostGoals > _matches[i].Result.GuestGoals || _matches[i].Result.HostGoals == _matches[i].Result.GuestGoals)
                {
                    foreach (var c in _clubs)
                    {
                        if (c.Id == _matches[i].FirstTeamId)
                        {
                            _winners.Add(c);
                            if (oneFound) break;
                            oneFound = true;
                        }
                        else if (c.Id == _matches[i].SecondTeamId)
                        {
                            _loosers.Add(c);
                            if (oneFound) break;
                            oneFound = true;
                        }
                    }
                }
                else
                {
                    foreach (var c in _clubs)
                    {
                        if (c.Id == _matches[i].SecondTeamId)
                        {
                            _winners.Add(c);
                            if (oneFound) break;
                            oneFound = true;
                        }
                        else if (c.Id == _matches[i].FirstTeamId)
                        {
                            _loosers.Add(c);
                            if (oneFound) break;
                            oneFound = true;
                        }
                    }
                }
                oneFound = false;
                //winners.Last().SetRankingPoints(loosers.Last().GetRankingPoints());
            }
        }
        return true;
    }
    public override List<Club> GetWinners() =>  DetermineWhoWon() ? base.GetWinners() : null;
}
