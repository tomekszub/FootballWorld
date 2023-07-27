using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CommentPanelFootballerRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _PosText;
    [SerializeField] Image _CountryImage;
    [SerializeField] TextMeshProUGUI _NameText;
    [SerializeField] Image _ConditionFill;
    [SerializeField] Image _RatingImage;
    [SerializeField] TextMeshProUGUI _MatchRating;

    Footballer _footballer;
    PlayersMatchData _playersMatchData;

    public void Init(Footballer footballer, PlayersMatchData playersMatchData)
    {
        _footballer = footballer;
        _playersMatchData = playersMatchData;
        _PosText.text = footballer.Pos.ToString();
        _CountryImage.sprite = Database.Instance.CountryMaster.GetFlagByName(footballer.Country);
        _NameText.text = footballer.Surname;
        _RatingImage.sprite = Resources.Load<Sprite>($"Stars/{Mathf.Max(1, Mathf.RoundToInt(footballer.Rating / 10))}");
        _ConditionFill.fillAmount = footballer.Condition / 100;
        _MatchRating.text = playersMatchData.MatchRating.ToString();
    }

    public void UpdateState()
    {
        _ConditionFill.fillAmount = _footballer.Condition / 100;
        _MatchRating.text = Math.Round(_playersMatchData.MatchRating, 2).ToString();
    }
}
