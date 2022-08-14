using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingElement : MonoBehaviour
{
    [SerializeField] Image _Flag;
    [SerializeField] TextMeshProUGUI _Name;
    [SerializeField] TextMeshProUGUI _TotalPoints;
    [SerializeField] TextMeshProUGUI _Season1Points;
    [SerializeField] TextMeshProUGUI _Season2Points;
    [SerializeField] TextMeshProUGUI _Season3Points;
    [SerializeField] TextMeshProUGUI _Season4Points;
    [SerializeField] TextMeshProUGUI _Season5Points;

    public void SetData(string country, float totalPoints, float season1Points, float season2Points, float season3Points, float season4Points, float season5Points)
    {
        _Name.text = country;
        _Flag.sprite = Database.Instance.GetCountryMaster().GetFlagByName(country);
        _TotalPoints.text = totalPoints.ToString("F3");
        _Season1Points.text = season1Points.ToString("F3");
        _Season2Points.text = season2Points.ToString("F3");
        _Season3Points.text = season3Points.ToString("F3");
        _Season4Points.text = season4Points.ToString("F3");
        _Season5Points.text = season5Points.ToString("F3");
    }
}
