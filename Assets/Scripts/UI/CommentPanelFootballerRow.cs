using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommentPanelFootballerRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _PosText;
    [SerializeField] Image _CountryImage;
    [SerializeField] TextMeshProUGUI _NameText;
    [SerializeField] Image _ConditionFill;
    [SerializeField] Image _RatingImage;
    [SerializeField] TextMeshProUGUI _MatchRating;

    Footballer _footballer;

    public void Init(Footballer footballer)
    {
        _footballer = footballer;
        _PosText.text = footballer.Pos.ToString();
        _CountryImage.sprite = Database.Instance.CountryMaster.GetFlagByName(footballer.Country);
        _NameText.text = footballer.Surname;
        _RatingImage.sprite = Resources.Load<Sprite>($"Stars/{Mathf.Max(1, Mathf.RoundToInt(footballer.Rating / 10))}");
        _ConditionFill.fillAmount = footballer.Condition / 100;
        _MatchRating.text = "5.0";
    }

    public void UpdateState(float rating)
    {
        _ConditionFill.fillAmount = _footballer.Condition / 100;
        _MatchRating.text = rating.ToString();
    }
}
