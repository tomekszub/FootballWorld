using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SquadListElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _PositionText;
    [SerializeField] TextMeshProUGUI _NameText;
    [SerializeField] TextMeshProUGUI _RatingText;
    [SerializeField] Image _FlagImage;
    [SerializeField] Image _RatingImage;
    [SerializeField] List<TextMeshProUGUI> _AttributesText;
    [SerializeField] List<TextMeshProUGUI> _StatisticksText;
    [SerializeField] Image _FatigueImage;

    Footballer _footballer;

    public void SetData(Footballer f, string tournamentFilter)
    {
        _footballer = f;
        _PositionText.text = f.Pos.ToString();
        _NameText.text = f.GetFullName();
        _RatingText.text = f.Rating.ToString();
        _RatingImage.sprite = Resources.Load<Sprite>("Stars/" + (int)f.Rating);
        _FlagImage.sprite = Database.Instance.GetCountryMaster().GetFlagByName(f.Country);
        _AttributesText[0].text = f.FreeKicks.ToString();
        _AttributesText[1].text = f.Dribling.ToString();
        _AttributesText[2].text = f.Tackle.ToString();
        _AttributesText[3].text = f.Heading.ToString();
        _AttributesText[4].text = f.Shoot.ToString();
        _AttributesText[5].text = f.Speed.ToString();
        _AttributesText[6].text = f.Pass.ToString();
        _FatigueImage.fillAmount = f.Condition / 100.0f;
        UpdateStatistics(tournamentFilter);
    }

    public void UpdateStatistics(string tournamentFilter)
    {
        var playerStatistics = new Footballer.PlayerStatistics(_footballer.GetPlayerStatistics(tournamentFilter) ?? new Footballer.PlayerStatistics());
        _StatisticksText[0].text = playerStatistics.MatchesPlayed.ToString();
        _StatisticksText[1].text = playerStatistics.Goals.ToString();
        _StatisticksText[2].text = playerStatistics.Assists.ToString();
    }
}
