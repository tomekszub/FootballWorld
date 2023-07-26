using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum Perk
{
    Wonderkid = 0,
    Marathoner = 1
}

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
	public float Rating;
	public float FreeKicks;// wykonywanie stałych fragmentów gry
	public float Corner, Penalty;
	public int ClubID;
	public Position Pos;
	public float Dribling, Tackle, Heading, Shoot, Speed, Pass;
    public int BirthYear;
	public int Endurance;
    public byte Height, Weight;

	public float Condition
    {
		get { return _fatigue; }
		private set
		{ 
			_fatigue = Math.Clamp(value, 0, 100);
		}
    }
	public string FullName => Name != "" ? $"{Name} {Surname}" : Surname;
    public HashSet<Perk> Perks => _perks;

    HashSet<Perk> _perks;
	float _fatigue;
	Dictionary<string, PlayerStatistics> _statistics;

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
        Height = (byte)(155 + (heading / 2));
        Weight = (byte)UnityEngine.Random.Range(50, 110);
        _perks = new HashSet<Perk> ();
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

	public void AddStatistic(string tournamentName, PlayerStatistics.StatName statName, double val = 1)
    {
		if (!_statistics.ContainsKey(tournamentName))
			_statistics.Add(tournamentName, new PlayerStatistics());

        _statistics[tournamentName].ChangeStat(statName, val);
    }

    public int CompareTo(Footballer f) => f.Rating.CompareTo(Rating);

	public void LoseFatigue()
	{
		Condition += BASE_FATIGUE_LOSS + (MAX_FATIGUE_LOSS_ENDURANCE - (Endurance * BASE_FATIGUE_LOSS_PER_ENDURANCE)) - (GetTotalMatchesPlayed() * BASE_REST_VALUE_LOSS_PER_MATCH);
	}

	public void GainFatigue(bool goalkeeper)
	{
		Condition -= (BASE_FATIGUE_GAIN + (MAX_FATIGUE_GAIN_ENDURANCE - (Endurance * BASE_FATIGUE_GAIN_PER_ENDURANCE))) * (goalkeeper ? 0.6f : 1f);
	}

    public void AddPerk(Perk perk) => _perks.Add(perk);

	int GetTotalMatchesPlayed()
    {
		var stats = GetPlayerStatistics("");
		return (int)stats.GetStat(PlayerStatistics.StatName.MatchesPlayed);
    }

    public class PlayerStatistics
    {
        public enum StatName
        {
            MatchesPlayed,
            Goals,
            Assists,
            CleanSheet,
            MatchRating
        }

        Dictionary<StatName, double> _stats;

        public PlayerStatistics(Dictionary<StatName, double> stats)
        {
            _stats = new Dictionary<StatName, double>(stats);
        }

		public PlayerStatistics()
        {
            _stats = new();

            foreach (var statName in Enum.GetValues(typeof(StatName)))
                _stats.Add((StatName)statName, 0);
        }

		public PlayerStatistics(PlayerStatistics playerStatistics)
        {
            _stats = new(playerStatistics._stats);
		}

        public double GetStat(StatName statName) => _stats[statName];

        public void ChangeStat(StatName statName, double change) => _stats[statName] += change;

        public static PlayerStatistics operator +(PlayerStatistics p1, PlayerStatistics p2)
        {
            Dictionary<StatName, double> stats = new(p1._stats);

            foreach(var statKVP in p2._stats)
            {
                if(stats.ContainsKey(statKVP.Key))
                    stats[statKVP.Key] += statKVP.Value;
                else
                    stats.Add(statKVP.Key, statKVP.Value);
            }

			return new PlayerStatistics(stats);
        }
    }
}
