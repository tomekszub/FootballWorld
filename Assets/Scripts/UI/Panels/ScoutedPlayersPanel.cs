using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;

public class ScoutedPlayersPanel : BasePanel
{
    [SerializeField] int _EntriesPerPage;
    [SerializeField] FootballerTableData _FootballerTableData;
    [SerializeField] TextMeshProUGUI _PageCounter;
    [SerializeField] TMP_Dropdown _PositionDropdown;

    List<(Footballer, int)> _players = new();
    List<(Footballer, int)> _filteredPlayers;
    int _currPage;
    int _pageMax;

    void Awake()
    {
        _PositionDropdown.options.Clear();
        _PositionDropdown.options.Add(new TMP_Dropdown.OptionData("No Filter"));

        foreach (var pos in Enum.GetNames(typeof(Footballer.Position)))
            _PositionDropdown.options.Add(new TMP_Dropdown.OptionData(pos));
    }

    void OnEnable()
    {
        _players.Clear();

        _PositionDropdown.value = 0;

        var playerDatabase = Database.footballersDB;
        foreach (var scoutedPlayerKVP in MyClub.Instance.ScoutedPlayers)
        {
            _players.Add((playerDatabase[scoutedPlayerKVP.Key], scoutedPlayerKVP.Value));
        }

        _filteredPlayers = new(_players);

        _currPage = 0;

        ShowPlayers();
    }

    public void NextPage()
    {
        if (_currPage >= _pageMax - 1)
            return;

        _currPage++;
        UpdatePageCounter();
        ShowPlayers();
    }

    public void PreviousPage()
    {
        if (_currPage <= 0)
            return;

        _currPage--;
        UpdatePageCounter();
        ShowPlayers();
    }

    public void OnPositionChanged(int option)
    {
        if (option == 0)
        {
            _filteredPlayers = new(_players);
        }
        else
        {
            var position = (Footballer.Position)(option - 1);

            // lets try work on ienumerables
            _filteredPlayers = _players.Where(p => p.Item1.Pos == position).ToList();
        }

        _currPage = 0;

        ShowPlayers();
    }

    void ShowPlayers()
    {

        _filteredPlayers = _filteredPlayers.OrderBy(p => p.Item1.Surname).ToList();

        _pageMax = Mathf.CeilToInt(_filteredPlayers.Count / (float)_EntriesPerPage);

        UpdatePageCounter();

        _FootballerTableData.ShowData(_filteredPlayers.Skip(_currPage * _EntriesPerPage).Take(_EntriesPerPage).Select(x => x.Item1).ToList(), true);
    }

    void UpdatePageCounter() => _PageCounter.text = $"{_currPage + 1}/{_pageMax}";
}
