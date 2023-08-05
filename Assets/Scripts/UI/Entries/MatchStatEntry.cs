using System;
using TMPro;
using UnityEngine;

public class MatchStatEntry : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _TournamentName;
    [SerializeField] TextMeshProUGUI _MatchesPlayed;
    [SerializeField] TextMeshProUGUI _Goals;
    [SerializeField] TextMeshProUGUI _Assists;
    [SerializeField] TextMeshProUGUI _MatchRating;

    public void SetData(string name, double matchesPlayed, double goals, double assists, double matchRating)
    {
        _TournamentName.text = name;
        _MatchesPlayed.text = matchesPlayed.ToString();
        _Goals.text = goals.ToString();
        _Assists.text = assists.ToString();
        _MatchRating.text = matchesPlayed > 0 ? Math.Round(matchRating / matchesPlayed, 2).ToString() : "-";
    }
}
