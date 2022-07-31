using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TableRow : MonoBehaviour
{
    [SerializeField] Image _RowBackground;
    [SerializeField] TextMeshProUGUI _Name;
    [SerializeField] Image _Flag;
    [SerializeField] TextMeshProUGUI _MatchesPlayed;
    [SerializeField] TextMeshProUGUI _Wins;
    [SerializeField] TextMeshProUGUI _Draws;
    [SerializeField] TextMeshProUGUI _Loses;
    [SerializeField] TextMeshProUGUI _GoalScored;
    [SerializeField] TextMeshProUGUI _GoalLost;
    [SerializeField] TextMeshProUGUI _GoalDiff;
    [SerializeField] TextMeshProUGUI _Points;
    
    Team _team;

    public void SetTeam(Team t, Color32 rowColor, bool showFlag = false)
    {
        _team = t;
        _RowBackground.color = rowColor;
        UpdateContent(showFlag);
    }

    void UpdateContent(bool showFlag = false)
    {
        _Name.text = _team.Name;

        if (showFlag)
        {
            _Flag.gameObject.SetActive(true);
            _Flag.sprite = Database.Instance.GetCountryMaster().GetFlagByName(_team.Name);
        }
        else
            _Flag.gameObject.SetActive(false);

        _MatchesPlayed.text = _team.MatchesPlayed.ToString();
        _Wins.text = _team.Wins.ToString();
        _Draws.text = _team.Draws.ToString();
        _Loses.text = _team.Loses.ToString();
        _GoalScored.text = _team.ScoredGoals.ToString();
        _GoalLost.text = _team.LostGoals.ToString();
        _GoalDiff.text = _team.DifferenceGoals.ToString();
        _Points.text = _team.Points.ToString();
    }
}
