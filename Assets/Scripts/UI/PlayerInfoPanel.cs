using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInfoPanel : BasePanel
{
    [Header("Header")]
    [SerializeField] TextMeshProUGUI _FullName;
    [SerializeField] Image _NationalityFlag;
    [SerializeField] TextMeshProUGUI _ClubName;
    [SerializeField] Image _ClubCountryFlag;
    [Header("Info")]
    [SerializeField] TextMeshProUGUI _Position;
    [SerializeField] TextMeshProUGUI _Age;
    [SerializeField] TextMeshProUGUI _Height;
    [SerializeField] TextMeshProUGUI _Weight;
    [Header("Stats")]
    [SerializeField] TextMeshProUGUI _Rating;
    [SerializeField] Image _RatingStars;
    [SerializeField] TextMeshProUGUI _Shooting;
    [SerializeField] TextMeshProUGUI _Dribble;
    [SerializeField] TextMeshProUGUI _Speed;
    [SerializeField] TextMeshProUGUI _Pass;
    [SerializeField] TextMeshProUGUI _Heading;
    [SerializeField] TextMeshProUGUI _Tackle;

    public class PlayerInfoPanelData : PanelData
    {
        public Footballer Footballer;
    }

    protected override void OnShow(PanelData panelData)
    {
        Footballer player = panelData is PlayerInfoPanelData p ? p.Footballer : Database.Instance.GetFootballersFromClub(MyClub.Instance.MyClubID)[0];

        _FullName.text = player.FullName;
        _NationalityFlag.sprite = Database.Instance.CountryMaster.GetFlagByName(player.Country);
        _ClubName.text = player.ClubID == -1 ? "Free Agent" : Database.clubDB[player.ClubID].Name;
        _ClubCountryFlag.sprite = player.ClubID == -1 ? null : Database.Instance.CountryMaster.GetFlagByName(Database.clubDB[player.ClubID].CountryName);
        _Position.text = player.Pos.ToString();
        _Age.text = (MyClub.Instance.CurrentDate.Year - player.BirthYear).ToString();
        _Height.text = player.Height.ToString();
        _Weight.text = player.Weight.ToString();
        _Rating.text = player.Rating.ToString();
        _RatingStars.sprite = Resources.Load<Sprite>($"Stars/{Mathf.Max(1, Mathf.RoundToInt(player.Rating / 10))}");
        _Shooting.text = player.Shoot.ToString();
        _Dribble.text = player.Dribling.ToString();
        _Speed.text = player.Speed.ToString();
        _Pass.text = player.Pass.ToString();
        _Heading.text = player.Heading.ToString();
        _Tackle.text = player.Tackle.ToString();
    }
}
