using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using static Footballer.PlayerStatistics;

public class PlayerInfoPanel : BasePanel
{
    public class PlayerInfoPanelData : PanelData
    {
        public enum InfoContext
        {
            None = 0,
            Club = 1,
            Country = 2
        }

        public Footballer Footballer;
        public InfoContext Context;

        public PlayerInfoPanelData(Footballer footballer, InfoContext context)
        {
            Footballer = footballer;
            Context = context;
        }
    }

    [Header("Header")]
    [SerializeField] TextMeshProUGUI _FullName;
    [SerializeField] Image _NationalityFlag;
    [SerializeField] TextMeshProUGUI _ClubName;
    [SerializeField] Image _ClubCountryFlag;
    [SerializeField] Button _NextPlayer;
    [SerializeField] Button _PreviousPlayer;
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
    [SerializeField] TextMeshProUGUI _FreeKick;
    [SerializeField] TextMeshProUGUI _Endurance;
    [Header("Match Stats")]
    [SerializeField] List<MatchStatEntry> _MatchStatsEntries;

    List<Footballer> _contextPlayers;
    int _currIndexInContextList;

    protected override void OnShow(PanelData panelData)
    {
        Footballer player;
        PlayerInfoPanelData infoData = panelData as PlayerInfoPanelData;

        bool hasInfoData = infoData != null;
        bool shouldShowArrows = hasInfoData && infoData.Context != PlayerInfoPanelData.InfoContext.None;

        _NextPlayer.onClick.RemoveAllListeners();
        _PreviousPlayer.onClick.RemoveAllListeners();
        _NextPlayer.gameObject.SetActive(shouldShowArrows);
        _PreviousPlayer.gameObject.SetActive(shouldShowArrows);

        if(hasInfoData)
        {
            player = infoData.Footballer;

            if (infoData.Context == PlayerInfoPanelData.InfoContext.Club)
            {
                _contextPlayers = Database.Instance.GetFootballersFromClub(player.ClubID);
                _currIndexInContextList = 0;
                for (int i = 0; i < _contextPlayers.Count; i++)
                {
                    if (_contextPlayers[i].Id == player.Id)
                    {
                        _currIndexInContextList = i;
                        break;
                    }
                }
            }
            _NextPlayer.onClick.AddListener(NextPlayer);
            _PreviousPlayer.onClick.AddListener(PreviousPlayer);
        }
        else
        {
            player = Database.Instance.GetFootballersFromClub(MyClub.Instance.MyClubID)[0];
        }

        ShowPlayerInfo(player);
    }

    void ShowPlayerInfo(Footballer player)
    {
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
        _FreeKick.text = player.FreeKicks.ToString();
        _Endurance.text = player.Endurance.ToString();

        var stats = player.Statistics;

        _MatchStatsEntries.ForEach(entry => entry.gameObject.SetActive(false));

        int index = 0;

        foreach (var stat in stats)
        {
            _MatchStatsEntries[index].SetData(
                stat.Key,
                stat.Value.GetStat(StatName.MatchesPlayed),
                stat.Value.GetStat(StatName.Goals),
                stat.Value.GetStat(StatName.Assists),
                stat.Value.GetStat(StatName.MatchRating));

            _MatchStatsEntries[index].gameObject.SetActive(true);
            index++;

            if (index == _MatchStatsEntries.Count)
                break;
        }
    }

    void NextPlayer()
    {
        _currIndexInContextList++;

        _currIndexInContextList %= _contextPlayers.Count;

        ShowPlayerInfo(_contextPlayers[_currIndexInContextList]);
    }

    void PreviousPlayer()
    {
        _currIndexInContextList--;

        if (_currIndexInContextList < 0)
            _currIndexInContextList = _contextPlayers.Count - 1;

        ShowPlayerInfo(_contextPlayers[_currIndexInContextList]);
    }
}
