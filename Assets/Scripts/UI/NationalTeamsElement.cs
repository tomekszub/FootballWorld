using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NationalTeamsElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _PositionText;
    [SerializeField] TextMeshProUGUI _NameText;
    [SerializeField] TextMeshProUGUI _RatingText;
    [SerializeField] Image _RatingImage;
    [SerializeField] List<TextMeshProUGUI> _AttributesText;
    [SerializeField] Image _FlagImage;
    [SerializeField] TextMeshProUGUI _ClubNameText;

    public void SetData(Footballer f)
    {
        _PositionText.text = f.Pos.ToString();
        _NameText.text = f.GetFullName();
        _RatingText.text = f.Rating.ToString();
        _RatingImage.sprite = Resources.Load<Sprite>("Stars/" + (int)f.Rating);
        if (f.ClubID != -1)
        {
            _FlagImage.enabled = true;
            _FlagImage.sprite = Database.Instance.GetCountryMaster().GetFlagByName(Database.clubDB[f.ClubID].CountryName);
            _ClubNameText.text = Database.clubDB[f.ClubID].Name;
        }
        else
        {
            _FlagImage.enabled = false;
            _ClubNameText.text = "Bez klubu";
        }
        _AttributesText[0].text = f.FreeKicks.ToString();
        _AttributesText[1].text = f.Dribling.ToString();
        _AttributesText[2].text = f.Tackle.ToString();
        _AttributesText[3].text = f.Heading.ToString();
        _AttributesText[4].text = f.Shoot.ToString();
        _AttributesText[5].text = f.Speed.ToString();
        _AttributesText[6].text = f.Pass.ToString();
    }
}
