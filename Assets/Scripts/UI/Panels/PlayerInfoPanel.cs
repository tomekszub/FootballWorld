using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using static Footballer.PlayerStatistics;
using System;
using Random = UnityEngine.Random;

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
        public Action OnSuccessfullPurchase;

        public PlayerInfoPanelData(Footballer footballer, InfoContext context, Action onSuccessfullPurchase = null)
        {
            Footballer = footballer;
            Context = context;
            OnSuccessfullPurchase = onSuccessfullPurchase;
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
    [SerializeField] PerksTableDataField _PerksTable;
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
    Footballer _currFootballer;
    PlayerInfoPanelData _infoData;

    public void ShowTransferNegotiationWindow()
    {
        if (_currFootballer.ClubID == MyClub.Instance.MyClubID)
            return;

        decimal transferFee = (decimal)(_currFootballer.Rating * (Random.value * 2 + 0.2f));

        CustomPanel.CustomPanelData customPanelData = new();
        customPanelData.Title = "Transfer";
        customPanelData.Description = $"Please confirm transfer of {_currFootballer.FullName} to your club.\nExpected fee: {transferFee}M $.";
        customPanelData.OnConfirm = () =>
        {
            if(TransferHub.TryTransferToMyClub(_currFootballer, transferFee) == TransferHub.TransferResult.Success)
            {
                UpdatePlayerInfo();
                _infoData.OnSuccessfullPurchase?.Invoke();
            }
        };
        WindowsManager.Instance.ShowWindow("Custom", customPanelData, false);
    }

    protected override void OnShow(PanelData panelData)
    {
        _infoData = panelData as PlayerInfoPanelData;

        bool hasInfoData = _infoData != null;
        bool shouldShowArrows = hasInfoData && _infoData.Context != PlayerInfoPanelData.InfoContext.None;

        _NextPlayer.onClick.RemoveAllListeners();
        _PreviousPlayer.onClick.RemoveAllListeners();
        _NextPlayer.gameObject.SetActive(shouldShowArrows);
        _PreviousPlayer.gameObject.SetActive(shouldShowArrows);

        if (hasInfoData)
        {
            _currFootballer = _infoData.Footballer;

            if (_infoData.Context == PlayerInfoPanelData.InfoContext.Club)
            {
                _contextPlayers = Database.GetFootballersFromClub(_currFootballer.ClubID);
                _currIndexInContextList = 0;
                for (int i = 0; i < _contextPlayers.Count; i++)
                {
                    if (_contextPlayers[i].Id == _currFootballer.Id)
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
            _currFootballer = Database.GetFootballersFromClub(MyClub.Instance.MyClubID)[0];
        }

        UpdatePlayerInfo();
    }

    void UpdatePlayerInfo()
    {
        int knowledgeLevel = MyClub.Instance.GetKnowledgeOfPlayer(_currFootballer.Id);

        _FullName.text = _currFootballer.FullName;
        _NationalityFlag.sprite = Database.Instance.CountryMaster.GetFlagByName(_currFootballer.Country);
        _ClubName.text = _currFootballer.ClubID == -1 ? "Free Agent" : Database.clubDB[_currFootballer.ClubID].Name;
        _ClubCountryFlag.sprite = _currFootballer.ClubID == -1 ? null : Database.Instance.CountryMaster.GetFlagByName(Database.clubDB[_currFootballer.ClubID].CountryName);
        _Position.text = knowledgeLevel >= Footballer.BASE_INFO_KNOWLEDGE_LEVEL ? _currFootballer.Pos.ToString() : "?";
        _Age.text = knowledgeLevel >= Footballer.BASE_INFO_KNOWLEDGE_LEVEL ?
            (MyClub.Instance.CurrentDate.Year - _currFootballer.BirthYear).ToString() :
            "?";
        _Height.text = knowledgeLevel >= Footballer.BASE_INFO_KNOWLEDGE_LEVEL ? _currFootballer.Height.ToString() : "?";
        _Weight.text = knowledgeLevel >= Footballer.BASE_INFO_KNOWLEDGE_LEVEL ? _currFootballer.Weight.ToString() : "?";

        if (knowledgeLevel >= Footballer.PERK_KNOWLEDGE_LEVEL)
            _PerksTable.SetPerks(_currFootballer.Perks);
        else
            _PerksTable.ShowMissingData();

        _Rating.text = knowledgeLevel >= Footballer.RATING_KNOWLEDGE_LEVEL ? _currFootballer.Rating.ToString() : "?";
        _RatingStars.sprite = Resources.Load<Sprite>(knowledgeLevel >= Footballer.RATING_KNOWLEDGE_LEVEL ?
            $"Stars/{Mathf.Max(1, Mathf.RoundToInt(_currFootballer.Rating / 10))}" :
            "Stars/0");
        _Shooting.text = knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? _currFootballer.Shoot.ToString() : "?";
        _Dribble.text = knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? _currFootballer.Dribling.ToString() : "?";
        _Speed.text = knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? _currFootballer.Speed.ToString() : "?";
        _Pass.text = knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? _currFootballer.Pass.ToString() : "?";
        _Heading.text = knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? _currFootballer.Heading.ToString() : "?";
        _Tackle.text = knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? _currFootballer.Tackle.ToString() : "?";
        _FreeKick.text = knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? _currFootballer.FreeKicks.ToString() : "?";
        _Endurance.text = knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? _currFootballer.Endurance.ToString() : "?";

        var stats = _currFootballer.Statistics;

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

        _currFootballer = _contextPlayers[_currIndexInContextList];

        UpdatePlayerInfo();
    }

    void PreviousPlayer()
    {
        _currIndexInContextList--;

        if (_currIndexInContextList < 0)
            _currIndexInContextList = _contextPlayers.Count - 1;

        _currFootballer = _contextPlayers[_currIndexInContextList];

        UpdatePlayerInfo();
    }
}
