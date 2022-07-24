using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Footballer 
{
	public int Id;
	public string Name;
	public string Surname;
	public string AlteredSurname;
	public string Country;
	public Sprite Flag;
	public float Rating; // 1-10 1 amator, 2 pół-amator, 3 bardzo słaby, 4 słaby, 5 sredni, 6 niezły, 7 dobry, 8 bardzo dobry, 9 rewelacyjny, 10 klasa swiatowa
	public float FreeKicks;// wykonywanie stałych fragmentów gry
	public float Corner, Penalty;
	Dictionary<string, PlayerStatistics> _statistics;
	public enum Position
	{
		BR,PO,ŚO,LO,ŚPD,ŚP,PP,ŚPO,LP,N
	}
	public Position Pos;
	public float Dribling, Tackle, Heading, Shoot, Speed, Pass;
    public int BirthYear;

	public string GetFullName()
    {
		if (Name != "")
			return $"{Name} {Surname}";
		else
			return Surname;
    }

	public Footballer(int id, string name, string surname, string alteredSurname, string country, float rating, float freeKicks, Position pos, float dribling, float tackle, float heading, float shoot, float speed, float pass, int birthYear = 1995)
	{
		Id = id;
		Name = name;
		Surname = surname;
		AlteredSurname = alteredSurname;
		Country = country;
		Flag = Resources.Load<Sprite> ("Flags/" + country);
		Rating = Mathf.Round((dribling + tackle + heading + shoot + speed + pass) / 6);
		FreeKicks = freeKicks;
		Pos = pos;
		Dribling = dribling;
		Tackle = tackle;
		Heading = heading;
		Shoot = shoot;
		Speed = speed;
		Pass = pass;
		Corner = freeKicks - heading + pass;
		Penalty = freeKicks + shoot;
        BirthYear = birthYear;
		_statistics = new Dictionary<string, PlayerStatistics> ();
	}
	public Footballer(int id, string name, string surname, string country, Dictionary<string,PlayerStatistics> matchStatistics)
	{
		Id = id;
		Name = name;
		Surname = surname;
		Flag = Resources.Load<Sprite>("Flags/" + country);
		_statistics = new Dictionary<string, PlayerStatistics>(matchStatistics);
	}

	public PlayerStatistics GetPlayerStatistics(string tournament)
	{
		if(tournament == "")
        {
			if (_statistics.Count == 0)
				return null;

			PlayerStatistics playerStatistics = new PlayerStatistics();

			foreach (var item  in _statistics.Values)
            {
				playerStatistics += item;
            }

			return playerStatistics;
        }
		else
			return _statistics.ContainsKey(tournament) ? _statistics[tournament] : null;
	}

	public List<string> GetPlayedTournaments()
    {
		return _statistics.Keys.ToList();
    }

	public void AddStatistic(string tournamentName, PlayerStatistics.StatName statName)
    {
		if (!_statistics.ContainsKey(tournamentName))
			_statistics.Add(tournamentName, new PlayerStatistics());

		switch (statName)
		{
			case PlayerStatistics.StatName.MatchesPlayed:
				_statistics[tournamentName].MatchesPlayed++;
				break;
			case PlayerStatistics.StatName.Goals:
				_statistics[tournamentName].Goals++;
				break;
			case PlayerStatistics.StatName.Assists:
				_statistics[tournamentName].Assists++;
				break;
			case PlayerStatistics.StatName.CleanSheet:
				_statistics[tournamentName].CleanSheet++;
				break;
			default:
				break;
        }
    }

    public class PlayerStatistics
    {
        public enum StatName
        {
            MatchesPlayed,
            Goals,
            Assists,
            CleanSheet
        }
        public int MatchesPlayed, Goals, Assists, CleanSheet;

        public PlayerStatistics(int matchesPlayed, int goals, int assists, int cleanSheet)
        {
            MatchesPlayed = matchesPlayed;
            Goals = goals;
            Assists = assists;
            CleanSheet = cleanSheet;
        }

		public PlayerStatistics()
        {
			MatchesPlayed = 0;
			Goals = 0;
			Assists = 0;
			CleanSheet = 0;
		}

		public PlayerStatistics(PlayerStatistics playerStatistics)
        {
			MatchesPlayed = playerStatistics.MatchesPlayed;
			Goals = playerStatistics.Goals;
			Assists = playerStatistics.Assists;
			CleanSheet = playerStatistics.CleanSheet;
		}

        public static PlayerStatistics operator +(PlayerStatistics p1, PlayerStatistics p2)
        {
			return new PlayerStatistics(p1.MatchesPlayed + p2.MatchesPlayed, p1.Goals + p2.Goals, p1.Assists + p2.Assists, p1.CleanSheet + p2.CleanSheet);
        }
    }
}
