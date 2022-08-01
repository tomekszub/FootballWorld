using Sirenix.OdinInspector;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
//[CreateAssetMenu(fileName = "CountryMaster")]
public class CountryMaster : SerializedScriptableObject
{
    [SerializeField] Dictionary<string, Country> _Countries;

    [SerializeField] Dictionary<string, HashSet<string>> _Regions;

    [SerializeField] Dictionary<string, HashSet<string>> _Continents;

    public List<string> GetRandomClubNames(string countryName, int amount)
    {
        Country country = GetCountryByName(countryName);
        if (country == null)
            return null;
        var firstParts = country.GetClubNameFirstPart();
        var secondParts = country.GetClubNameSecondPart();

        if(firstParts == null || secondParts == null)
        {
            throw new MissingReferenceException("There is not enough text assets for " + countryName);
        }

        var randomItems = GetRandomItems(firstParts, secondParts, amount);
        List<string> ret = new List<string>();
        randomItems.ForEach(r => ret.Add($"{r.Item1} {r.Item2}"));
        return ret;
    }
    public List<(string, string)> GetRandomPeople(string countryName, int amount)
    {
        Country country = GetCountryByName(countryName);
        if (country == null)
            return null;

        return GetRandomItems(country.GetNames(), country.GetSurnames(), amount);
    }

    public string GetRandomCountryFromRegion(string regionName, List<string> excludeCountries = null)
    {
        var countries = new HashSet<string>(_Regions[regionName]);
        if(excludeCountries != null)
        {
            for (int i = 0; i < excludeCountries.Count; i++)
            {
                if(countries.Contains(excludeCountries[i]))
                    countries.Remove(excludeCountries[i]);
            }
        }
        var countriesList = countries.ToList();
        return countriesList.Count > 0 ? countriesList[Random.Range(0, countriesList.Count)] : null;
    }
    public Sprite GetFlagByName(string countryName) => _Countries.ContainsKey(countryName) ? _Countries[countryName].GetFlag() : null;
    public HashSet<string> GetCountryListFromContinent(string continentName)
    {
        HashSet<string> result = new HashSet<string>();
        var regions = _Continents[continentName];
        foreach (var region in regions)
        {
            foreach (var country in _Regions[region])
            {
                result.Add(country);
            }
        }
        return result;
    }

    List<(string, string)> GetRandomItems(TextAsset firstSet, TextAsset secondSet, int amount)
    {
        List<string> firstParts = TextAssetParser.Parse(firstSet);
        List<string> secondParts = TextAssetParser.Parse(secondSet);

        var ret = new List<(string, string)>();
        int firstIndex, secondIndex, originalSecondIndex;
        bool teamB;

        for (int i = 0; i < amount; i++)
        {
            teamB = false;
            firstIndex = Random.Range(0, firstParts.Count);
            secondIndex = Random.Range(0, secondParts.Count);
            originalSecondIndex = secondIndex;
            while(ret.Contains((firstParts[firstIndex], secondParts[secondIndex])))
            {
                secondIndex++;
                if(secondIndex == secondParts.Count)
                    secondIndex = 0;
                if(secondIndex == originalSecondIndex)
                {
                    // iterated through whole list and did not find any free name so we make it team B
                    teamB = true;
                    continue;
                }
            }
            ret.Add((firstParts[firstIndex], teamB ? secondParts[secondIndex] + " B" : secondParts[secondIndex]));
        }

        return ret;
    }
    Country GetCountryByName(string countryName) => _Countries.ContainsKey(countryName) ? _Countries[countryName] : null;
}
