using System.Linq;
using System.Collections.Generic;

public class Club
{
    public float RankingPoints => _rankingPoints.Sum();

	public int Id;
	public string Name;
	public int Rate;
	public string Stadium;
	public int StadiumCapacity;
	public string Coach;
	public string Formation;
	public int StartingFootballerId, EndingFootballerId;
    public List<int> FootballersIDs;
    float[] _rankingPoints;
    public string CountryName;

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
	}

    public void AddRankingPoints(float amount) => _rankingPoints[4] += amount;
    public void PrepareRankingPointsForNewSeason()
    {
        for (int i = 0; i < 4; i--)
        {
            _rankingPoints[i] = _rankingPoints[i + 1];
        }
        _rankingPoints[4] = 0;
    }
	
}
