using UnityEngine;
using System.Collections;

public class Team
{
	public string Name;
	public int Id, Points, ScoredGoals, LostGoals, DifferenceGoals, MatchesPlayed, Wins, Draws, Loses;

	public Team(int id, string name, int points, int scoredGoals, int lostGoals, int wins, int draws, int loses)
	{
		Id = id;
		Name = name;
		Points = points;
		ScoredGoals = scoredGoals;
		LostGoals = lostGoals;
		Wins = wins;
		Draws = draws;
		Loses = loses;
		DifferenceGoals = scoredGoals - lostGoals;
		MatchesPlayed = wins + draws + loses;
	}
    public Team(int id, string name)
    {
        Id = id;
        Name = name;
        Points = 0;
        ScoredGoals = 0;
        LostGoals = 0;
        Wins = 0;
        Draws = 0;
        Loses = 0;
        DifferenceGoals = 0;
        MatchesPlayed = 0;
    }
}
