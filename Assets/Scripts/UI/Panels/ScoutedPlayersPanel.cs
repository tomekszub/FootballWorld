using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ScoutedPlayersPanel : BasePanel
{
    [SerializeField] int _EntriesPerPage;
    [SerializeField] FootballerTableData _FootballerTableData;
    [SerializeField] TextMeshProUGUI _PageCounter;

    List<(Footballer, int)> _players;
    int _currPage;
    int _pageMax;

    void OnEnable()
    {
        _currPage = 0;
        _players = new List<(Footballer, int)> ();
        var playerDatabase = Database.footballersDB;
        foreach (var scoutedPlayerKVP in MyClub.Instance.ScoutedPlayers)
        {
            _players.Add((playerDatabase[scoutedPlayerKVP.Key], scoutedPlayerKVP.Value));
        }

        _players = _players.OrderBy(p => p.Item1.Surname).ToList();

        _pageMax = Mathf.CeilToInt(_players.Count / (float)_EntriesPerPage);

        UpdatePageCounter();
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

    void ShowPlayers()
    {
        _FootballerTableData.ShowData(_players.Skip(_currPage * _EntriesPerPage).Take(_EntriesPerPage).Select(x => x.Item1).ToList(), true);
    }

    void UpdatePageCounter() => _PageCounter.text = $"{_currPage + 1}/{_pageMax}";
}
