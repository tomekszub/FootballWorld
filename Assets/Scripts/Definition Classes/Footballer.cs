using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Footballer : IComparable<Footballer>
{
	const float BASE_FATIGUE_LOSS = 10;
	const float BASE_REST_VALUE_LOSS_PER_MATCH = 0.12f;
	const float MAX_FATIGUE_LOSS_ENDURANCE = 0.4f;
	const float BASE_FATIGUE_LOSS_PER_ENDURANCE = 0.04f;

	const float BASE_FATIGUE_GAIN = 0.41f;
	const float MAX_FATIGUE_GAIN_ENDURANCE = 0.3f;
	const float BASE_FATIGUE_GAIN_PER_ENDURANCE = 0.003f;

	public enum Position
	{
		BR,PO,ŚO,LO,ŚPD,ŚP,PP,ŚPO,LP,N
	}

	public int Id;
	public string Name;
	public string Surname;
	public string AlteredSurname;
	public string Country;
	public float Rating; // 1-10 1 amator, 2 pół-amator, 3 bardzo słaby, 4 słaby, 5 sredni, 6 niezły, 7 dobry, 8 bardzo dobry, 9 rewelacyjny, 10 klasa swiatowa
	public float FreeKicks;// wykonywanie stałych fragmentów gry
	public float Corner, Penalty;
	public int ClubID;
	public Position Pos;
	public float Dribling, Tackle, Heading, Shoot, Speed, Pass;
    public int BirthYear;
	public int Endurance;

	public float Condition
    {
		get { return _fatigue; }
		private set
		{ 
			_fatigue = Math.Clamp(value, 0, 100);
		}
    }

	float _fatigue;
	Dictionary<string, PlayerStatistics> _statistics;

	public string GetFullName() => Name != "" ? $"{Name} {Surname}" : Surname;

	public Footballer(int id, string name, string surname, string alteredSurname, string country, float freeKicks, Position pos, float dribling, float tackle, float heading, float shoot, float speed, float pass, int birthYear, int endurance, int clubID = -1)
	{
		Id = id;
		Name = name;
		Surname = surname;
		AlteredSurname = alteredSurname;
		Country = country;
        switch (pos)
        {
            case Position.BR:
				Rating = Mathf.Round((dribling + tackle + speed + pass) / 4);
				break;
            case Position.PO:
            case Position.LO:
				Rating = Mathf.Round((dribling + tackle + speed + pass) / 4);
				break;
            case Position.ŚO:
            case Position.ŚPD:
				Rating = Mathf.Round((heading + tackle + pass) / 3);
				break;
            case Position.ŚP:
				Rating = Mathf.Round((dribling + tackle + pass) / 3);
				break;
            case Position.PP:
            case Position.LP:
				Rating = Mathf.Round((dribling + shoot + pass + speed) / 4);
				break;
            case Position.ŚPO:
				Rating = Mathf.Round((dribling + shoot + pass) / 3);
				break;
            case Position.N:
				Rating = Mathf.Round((dribling + shoot + heading + speed) / 4);
				break;
        }
        FreeKicks = freeKicks;
		Pos = pos;
		Dribling = dribling;
		Tackle = tackle;
		Heading = heading;
		Shoot = shoot;
		Speed = speed;
		Pass = pass;
        // TODO: calculating corner skill is a nonsense right now
        // + when executing corner only pass and free kicks count so Corner is only for selecting players wtf
		Corner = freeKicks - heading + pass;
		Penalty = (freeKicks + shoot) / 2;
        BirthYear = birthYear;
		_statistics = new Dictionary<string, PlayerStatistics> ();
		ClubID = clubID;
		Condition = 100;
		Endurance = endurance;
    }
    public Footballer(int id, string name, string surname, string country, Dictionary<string,PlayerStatistics> matchStatistics)
	{
		Id = id;
		Name = name;
		Surname = surname;
		_statistics = new Dictionary<string, PlayerStatistics>(matchStatistics);
	}

	public PlayerStatistics GetPlayerStatistics(string tournament)
	{
		if(tournament == "")
        {
			PlayerStatistics playerStatistics = new PlayerStatistics();

			foreach (var item  in _statistics.Values)
            {
				playerStatistics += item;
            }

			return playerStatistics;
        }

		return _statistics.ContainsKey(tournament) ? _statistics[tournament] : new PlayerStatistics();
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

    public int CompareTo(Footballer f) => f.Rating.CompareTo(Rating);

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

	public void LoseFatigue()
	{
		Condition += BASE_FATIGUE_LOSS + (MAX_FATIGUE_LOSS_ENDURANCE - (Endurance * BASE_FATIGUE_LOSS_PER_ENDURANCE)) - (GetTotalMatchesPlayed() * BASE_REST_VALUE_LOSS_PER_MATCH);
	}

	public void GainFatigue(bool goalkeeper)
	{
		Condition -= (BASE_FATIGUE_GAIN + (MAX_FATIGUE_GAIN_ENDURANCE - (Endurance * BASE_FATIGUE_GAIN_PER_ENDURANCE))) * (goalkeeper ? 0.6f : 1f);
	}

	int GetTotalMatchesPlayed()
    {
		var stats = GetPlayerStatistics("");
		return stats.MatchesPlayed;
    }
}
