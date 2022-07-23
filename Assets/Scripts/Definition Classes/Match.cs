using System;
public struct MatchResult
{
    public int HostGoals, GuestGoals;
    public MatchResult(int hostGoals = 0, int guestGoals = 0)
    {
        HostGoals = hostGoals;
        GuestGoals = guestGoals;
    }
}
[System.Serializable]
public class Match 
{
	public int RoundIndex, FirstTeamId, SecondTeamId;
    public MatchResult Result;
    public bool Finished;
    public DateTime Date;
    public string CompetitionName;

	public Match(string competitionName, DateTime date, int roundIndex, int firstTeamId, int secondTeamId, MatchResult matchResult = new MatchResult(), bool finished = false)
	{
        CompetitionName = competitionName;
        Date = date;
		RoundIndex = roundIndex;
		FirstTeamId = firstTeamId;
		SecondTeamId = secondTeamId;
        Result = matchResult;
        Finished = finished;
	}
    public Match(Match m)
    {
        CompetitionName = m.CompetitionName;
        Date = m.Date;
        RoundIndex = m.RoundIndex;
        FirstTeamId = m.FirstTeamId;
        SecondTeamId = m.SecondTeamId;
        Result = m.Result;
        Finished = m.Finished;
    }
    public Match InvertTeams()
    {
        Match m = new Match(this);
        m.FirstTeamId = SecondTeamId;
        m.SecondTeamId = FirstTeamId;
        return m;
    }
    public override string ToString()
    {
        return "Competition: " + CompetitionName + " Date: " + Date + " RoundIndex: " + RoundIndex + " Finished: " + Finished;
    }
}
