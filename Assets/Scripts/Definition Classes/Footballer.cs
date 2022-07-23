using UnityEngine;

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
	public int MatchesPlayed, Goals, Assists, CleanSheet;
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
		Rating = Mathf.Round((freeKicks + dribling + tackle + heading + shoot + speed + pass) / 7);
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
	}
	public Footballer(int id, string name, string surname, string country, int matchesPlayed, int goals, int assists,int cleanSheet)
	{
		Id = id;
		Name = name;
		Surname = surname;
		Flag = Resources.Load<Sprite> ("Flags/" + country);
		MatchesPlayed = matchesPlayed;
		Goals = goals;
		Assists = assists;
		CleanSheet = cleanSheet;
	}
}
