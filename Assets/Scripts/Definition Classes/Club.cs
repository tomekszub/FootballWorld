using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Club
{
	public int Id;
	public string Name;
	public int Rate;
	public string Stadium;
	public int StadiumCapacity;
	public string Coach;
	public string Formation;
	public int StartingFootballerId, EndingFootballerId;
    public List<int> FootballersIDs;
    //public float RankingPoints;
    List<float> RankingPointsList;
    float RankingPoints;
    public string CountryName;

	public Club(int id, string name, string countryName, int rate, string stadium, int stadiumCapacity, string coach, string formation, int startingFootballerId, int endingFootballerId, List<float> rankingPointsList)
	{
		Id = id;
		Name = name;
        CountryName = countryName;
		Rate = rate;
		Stadium = stadium;
		StadiumCapacity = stadiumCapacity;
		Coach = coach;
		Formation = formation;
		StartingFootballerId = startingFootballerId;
		EndingFootballerId = endingFootballerId;
        FootballersIDs = new List<int>();
        for (int i = StartingFootballerId; i <= EndingFootballerId; i++)
        {
            FootballersIDs.Add(i);
        }
        RankingPointsList = rankingPointsList;
        UpdateRankingPoints();
	}
    public float GetRankingPoints()
    {
        return RankingPoints;
    }
    public void SetRankingPoints(float newValue)
    {
        if (newValue > RankingPoints) RankingPoints = newValue;
    }
    public void PrepareRankingPointsForNewSeason()
    {
        RankingPointsList.RemoveAt(0);
        RankingPointsList.Add(0);
    }
    public void InsertNewSeasonPoints(float amount)
    {
        RankingPointsList[4] = amount;
    }
    public void AddNewSeasonPoints(float amount)
    {
        RankingPointsList[4] += amount;
    }
    public void UpdateRankingPoints()
    {
        RankingPoints = 0;
        foreach (float item in RankingPointsList)
        {
            RankingPoints += item;
        }
        RankingPoints /= 5;
    }
	
}
