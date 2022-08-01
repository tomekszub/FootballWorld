using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class NationalTeamsPanel : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI _NameText;
    [SerializeField] List<NationalTeamsElement> _NationalTeamsElements;
    [SerializeField] TMPro.TMP_Dropdown _ContinentDropdown;

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
        var countries = Database.Instance.GetCountryMaster().GetCountryListFromContinent(continent);
        _countries = countries.ToList();
        _currCountryIndex = 0;
        SetupSquad();
    }

    public void SetupSquad()
    {
        var country = _countries[_currCountryIndex];
        _NameText.text = country;
        List<Footballer> footballers = new List<Footballer>();
        for (int i = 0; i < Database.footballersDB.Count; i++)
        {
            if(Database.footballersDB[i].Country == country)
                footballers.Add(Database.footballersDB[i]);
        }

        footballers.Sort();
        int maxPlayers = Mathf.Min(33, footballers.Count);

        for (int i = 0; i < _NationalTeamsElements.Count; i++)
        {
            if (i < maxPlayers)
            {
                _NationalTeamsElements[i].gameObject.SetActive(true);
                _NationalTeamsElements[i].SetData(footballers.ElementAt(i));
            }
            else
                _NationalTeamsElements[i].gameObject.SetActive(false);
        }

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
