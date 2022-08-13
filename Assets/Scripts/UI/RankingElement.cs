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
        _TotalPoints.text = totalPoints.ToString();
        _Season1Points.text = season1Points.ToString();
        _Season2Points.text = season2Points.ToString();
        _Season3Points.text = season3Points.ToString();
        _Season4Points.text = season4Points.ToString();
        _Season5Points.text = season5Points.ToString();
    }
}
