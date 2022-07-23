using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LeagueMaster")]
public class LeagueMaster : SerializedScriptableObject
{
    [SerializeField, DictionaryDrawerSettings(KeyLabel = "Country", ValueLabel = "League")] 
    Dictionary<string, LeagueDef> _Leagues;

    public LeagueDef GetLeagueDefinition(string country) => _Leagues.ContainsKey(country) ? _Leagues[country] : null;
}
