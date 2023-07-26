using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NationalTeamsPanel : BasePanel
{
    [SerializeField] TextMeshProUGUI _NameText;
    [SerializeField] Image _CountryImage;
    [SerializeField] TMP_Dropdown _ContinentDropdown;
    [SerializeField] FootballerTableData _FootballerTableData;

    int _currCountryIndex = 0;
    List<string> _countries;

    public void OnEnable()
    {
        _ContinentDropdown.value = 0;
        UpdateCountries(_ContinentDropdown.options[0].text);
    }

    public void OnContinentChanged(int option) => UpdateCountries(_ContinentDropdown.options[option].text);

    void UpdateCountries(string continent)
    {
        var countries = Database.Instance.CountryMaster.GetCountryListFromContinent(continent);
        _countries = countries.ToList();
        _countries.Sort();
        _currCountryIndex = 0;
        SetupSquad(true);
    }

    public void SetupSquad(bool fieldsChanged = false)
    {
        var country = _countries[_currCountryIndex];
        _NameText.text = country;
        _CountryImage.sprite = Database.Instance.CountryMaster.GetFlagByName(country);
        List<Footballer> footballers = Database.Instance.GetFootballersFromCountry(country);

        footballers.Sort();

        _FootballerTableData.ShowData(footballers.Take(30).ToList(), fieldsChanged);
    }

    public void ShowNextClub()
    {
        _currCountryIndex++;
        if (_currCountryIndex >= _countries.Count)
            _currCountryIndex = 0;
        SetupSquad();
    }

    public void ShowPreviousClub()
    {
        _currCountryIndex--;
        if (_currCountryIndex <= -1)
            _currCountryIndex = _countries.Count - 1;
        SetupSquad();
    }
}
