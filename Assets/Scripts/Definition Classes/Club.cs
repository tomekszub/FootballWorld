using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Club
{
    const int MIN_PLAYERS_COUNT = 22;
    const int MAX_PLAYERS_COUNT = 40;

    public float RankingPoints => _rankingPoints.Sum();
    public decimal Budget => _budget;

    public int Id;
	public string Name;
	public int Rate;
	public string Stadium;
	public int StadiumCapacity;
	public string Coach;
	public string Formation;
	public int StartingFootballerId, EndingFootballerId;
    public List<int> FootballersIDs;
    public string CountryName;

    float[] _rankingPoints;
    decimal _budget;

	public Club(int id, string name, string countryName, int rate, string stadium, int stadiumCapacity, string coach, string formation, int startingFootballerId, int endingFootballerId)
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
        _rankingPoints = new float[] { 0,0,0,0,0};
        _budget = Rate;
	}

    public void AddRankingPoints(float amount) => _rankingPoints[4] += amount / League_Old.GetNumberOfClubsInEuropaCups(MyClub.Instance.CurrFederationRankingCountryName[CountryName]);

    public void PrepareRankingPointsForNewSeason()
    {
        for (int i = 0; i < 4; i--)
        {
            _rankingPoints[i] = _rankingPoints[i + 1];
        }
        _rankingPoints[4] = 0;
    }

    public float GetRankingPoints(int seasonIndex) => _rankingPoints[seasonIndex];

    public void AddPlayer(int id, decimal fee)
    {
        _budget -= fee;
        FootballersIDs.Add(id);
    }

    public void RemovePlayer(int id)
    {
        int index = FootballersIDs.FindIndex(i => i == id);
        FootballersIDs.RemoveAt(index);
    }

    public bool HasSpaceForPlayer() => FootballersIDs.Count < MAX_PLAYERS_COUNT;

    public bool HasEnoughPlayers() => FootballersIDs.Count >= MIN_PLAYERS_COUNT;

    public bool HasEnoughMoney(decimal amount) => _budget >= amount;

    public void RefreshSquad()
    {
        List<Footballer> footballers = new();

        foreach(var index in FootballersIDs)
            footballers.Add(Database.footballersDB[index]);

        var positions = FormationDefinitions.Formations[Formation];

        for (int i = 0; i < FootballersIDs.Count; i++)
            FootballersIDs[i] = -1;

        footballers.Sort();

        LookForExactPositions(0, 11);

        LookForSimilarPositions(0, 11);

        LookForExactPositions(11, 22);

        LookForSimilarPositions(11, 22);

        for (int i = 0; i < FootballersIDs.Count; i++)
        {
            if (FootballersIDs[i] == -1)
            {
                FootballersIDs[i] = footballers[0].Id;
                footballers.RemoveAt(0);
            }
        }

        void LookForExactPositions(int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                int index = footballers.FindIndex(f => f.Pos == positions[i % 11]);

                if (index != -1)
                {
                    FootballersIDs[i] = footballers[index].Id;
                    footballers.RemoveAt(index);
                }
            }
        }

        void LookForSimilarPositions(int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                if (FootballersIDs[i] == -1)
                {
                    int index = -1;

                    switch (positions[i % 11])
                    {
                        case Footballer.Position.PO:
                        case Footballer.Position.ŚO:
                        case Footballer.Position.LO:
                        case Footballer.Position.ŚPD:
                            index = footballers.FindIndex(f => f.Pos == Footballer.Position.PO || f.Pos == Footballer.Position.ŚO || f.Pos == Footballer.Position.LO || f.Pos == Footballer.Position.ŚPD);
                            break;
                        case Footballer.Position.LP:
                        case Footballer.Position.ŚP:
                        case Footballer.Position.PP:
                            index = footballers.FindIndex(f => f.Pos == Footballer.Position.LP || f.Pos == Footballer.Position.ŚP || f.Pos == Footballer.Position.PP);
                            break;
                        case Footballer.Position.ŚPO:
                        case Footballer.Position.N:
                            index = footballers.FindIndex(f => f.Pos == Footballer.Position.ŚPO || f.Pos == Footballer.Position.N);
                            break;
                    }

                    if (index != -1)
                    {
                        FootballersIDs[i] = footballers[index].Id;
                        footballers.RemoveAt(index);
                    }
                }
            }
        }
    }
}
