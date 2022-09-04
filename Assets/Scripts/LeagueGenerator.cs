using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static Footballer;

public class LeagueGenerator : MonoBehaviour
{
    const int FOOTBALLERS_PER_TEAM = 25;

    [SerializeField] bool _ReducedTeamAmount_TEST = false;

    [SerializeField] CountryMaster _CountryMaster;
    [SerializeField] LeagueMaster _LeagueMaster;


    LeagueDef _leagueDef;
    string _country;
    int _clubsAmount;
    List<Credentials> _people = new List<Credentials>();
    public void Generate(string country, int clubAmount)
    {
        if (_ReducedTeamAmount_TEST)
            clubAmount = 2;
        // TODO: cash every name data in a dictionary for the time of creating db

        _leagueDef = _LeagueMaster.GetLeagueDefinition(country);

        if (_leagueDef == null)
            return;

        _clubsAmount = clubAmount;
        _country = country;
        _people = CreateRandomPeople();

        // 1-100
        var clubRatings = GenerateClubRatings();
        var clubNames = _CountryMaster.GetRandomClubNames(country, clubAmount);
        var clubs = new List<Club>();
        int clubID = Database.clubDB.Count;

        for (int i = 0; i < clubAmount; i++)
        {
            clubs.Add(GenerateClub(clubID + i, clubRatings[i], clubNames[i], "Coach Full Name"));
        }

        Database.leagueDB.Add(new League_Old(_leagueDef.Name, country, clubs, 90f));
    }

    Club GenerateClub(int id, int rating, string name, string coachName)
    {
        int startingID = Database.footballersDB.Count;
        var formationInfo = GetRandomFormation();

        Club c = new Club(id, name, _country, rating, "Stadion", 410, coachName, formationInfo.Item1, startingID, startingID + FOOTBALLERS_PER_TEAM - 1);
        GenerateFootballers(startingID, formationInfo.Item2, rating, id);
        Database.clubDB.Add(c);
        return c;
    }

    (string, List<Position>) GetRandomFormation()
    {
        int random = Random.Range(0, FormationDefinitions.Formations.Count);
        string formation = FormationDefinitions.Formations.ElementAt(random).Key;
        return (formation, new List<Position>(FormationDefinitions.Formations.ElementAt(random).Value));
    }

    List<int> GenerateClubRatings()
    {
        List<int> result = new List<int>(_clubsAmount);

        int ratingOffset = 15;
        int minAverageRating = Mathf.Max(1, _leagueDef.Rating - ratingOffset);
        int maxRating = Mathf.Min(101, _leagueDef.Rating + ratingOffset);

        int lowestRating = Mathf.Max(1, minAverageRating - ratingOffset);

        int topTeams = Random.Range(0, 3);
        for (int i = 0; i < _clubsAmount; i++)
        {
            // top teams with higher possible rating
            if (i < topTeams)
            {
                result.Add(Random.Range(_leagueDef.Rating, maxRating));
            }
            // bottom 3 teams with possible lower rating
            else if (i >= _clubsAmount - 3)
                result.Add(Random.Range(lowestRating, minAverageRating));
            else
                result.Add(Random.Range(minAverageRating, _leagueDef.Rating));
        }

        return result;
    }

    List<Credentials> CreateRandomPeople()
    {
        int amount = _clubsAmount * FOOTBALLERS_PER_TEAM;
        List<int> probabilityTreshholds = _leagueDef.GetProbabilityTreshholds();

        Dictionary<string, int> nationalities = new Dictionary<string, int>();
        // list of country names specified, instead of regions 
        List<string> singleCountries = _leagueDef.GetSingleCountriesFromDistribution();

        for (int i = 0; i < amount; i++)
        {
            int randomNumber = Random.Range(0, probabilityTreshholds[probabilityTreshholds.Count - 1] + 1);
            for (int j = 0; j < probabilityTreshholds.Count; j++)
            {
                if (randomNumber <= probabilityTreshholds[j])
                {
                    var regionInfo = _leagueDef.GetRegionInfo(j);
                    string country;
                    if (regionInfo.Item2)
                        country = regionInfo.Item1;
                    else
                    {
                        country = _CountryMaster.GetRandomCountryFromRegion(regionInfo.Item1, singleCountries);
                        if (country == null)
                            continue;
                    }

                    if (nationalities.ContainsKey(country))
                        nationalities[country]++;
                    else
                        nationalities.Add(country, 1);

                    break;
                }
            }
        }
        List<Credentials> ret = new List<Credentials>();

        foreach (var nat in nationalities)
        {
            var footballers = _CountryMaster.GetRandomPeople(nat.Key, nat.Value);
            if (footballers == null)
            {
                Debug.LogError("There is no country key: " + nat.Key);
                continue;
            }
            foreach (var footballer in footballers)
            {
                ret.Add(new Credentials(footballer.Item1, footballer.Item2, nat.Key));
            }
        }
        return ret;
    }

    void GenerateFootballers(int startingDBIndex, List<Position> pos, int rating, int clubID)
    {
        int baseRating = rating;
        int randomPersonIndex;
        int minSkill;
        int enduranceMin;
        float freeKickSkill;
        Credentials person;
        for (int i = 0; i < FOOTBALLERS_PER_TEAM; i++)
        {
            randomPersonIndex = Random.Range(0, _people.Count);
            person = _people[randomPersonIndex];
            _people.RemoveAt(randomPersonIndex);
            minSkill = Random.Range(0, 5) * 20;
            freeKickSkill = Random.Range(minSkill, minSkill + 20) / 10;
            enduranceMin = (int)(10 + baseRating * 0.5f);

            Database.footballersDB.Add(new Footballer(
                startingDBIndex + i, 
                person.Name, 
                person.Surname, 
                person.Surname, 
                person.Country,
                0, // for now rating is left in the constructor for backward compatibility, but is evaluated in the constructor from other stats
                freeKickSkill,
                pos[i % pos.Count], 
                Mathf.Clamp(Random.Range(baseRating - 20, baseRating + 10) / 10, 1, 100),
                Mathf.Clamp(Random.Range(baseRating - 20, baseRating + 10) / 10, 1, 100),
                Mathf.Clamp(Random.Range(baseRating - 20, baseRating + 10) / 10, 1, 100),
                Mathf.Clamp(Random.Range(baseRating - 20, baseRating + 10) / 10, 1, 100),
                Mathf.Clamp(Random.Range(baseRating - 20, baseRating + 10) / 10, 1, 100),
                Mathf.Clamp(Random.Range(baseRating - 20, baseRating + 10) / 10, 1, 100), 
                1996,
                Mathf.Clamp(Random.Range(enduranceMin, enduranceMin + 40), 1, 100),
                clubID));
        }
    }

    public class Credentials
    {
        public string Name;
        public string Surname;
        public string Country;

        public Credentials(string name, string surname, string country)
        {
            Name = name;
            Surname = surname;
            Country = country;
        }
    }
}
