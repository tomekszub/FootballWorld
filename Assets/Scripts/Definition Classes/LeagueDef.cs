using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class LeagueDef
{
    [SerializeField] string _Name;
    [SerializeField, Range(1,100)] int _Rating;

    [SerializeField, ListDrawerSettings(ListElementLabelName = "RegionName", AddCopiesLastElement = true)] 
    List<NationalityDistribution> _NationalityDistribution;

    public int Rating
    {
        get { return _Rating; }
    }
    public string Name
    {
        get { return _Name; }
    }

    public List<int> GetProbabilityTreshholds()
    {
        var ret = new List<int>();
        ret.Add(_NationalityDistribution[0].Probability);
        for (int i = 1; i < _NationalityDistribution.Count; i++)
        {
            ret.Add(ret[i-1] + _NationalityDistribution[i].Probability);
        }
        return ret;
    }

    public List<string> GetSingleCountriesFromDistribution()
    {
        List<string> ret = new List<string>();
        _NationalityDistribution.ForEach((x) => { if (x.IsCountry) ret.Add(x.RegionName); });
        return ret;
    }
    public (string,bool) GetRegionInfo(int index) => (_NationalityDistribution[index].RegionName, _NationalityDistribution[index].IsCountry);
}

[System.Serializable]
public class NationalityDistribution
{
    public bool IsCountry;
    public string RegionName;
    public int Probability;
}
