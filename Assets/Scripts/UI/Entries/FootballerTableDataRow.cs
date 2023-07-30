using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static FootballerTableData;
using static Footballer.PlayerStatistics;
using static PlayerInfoPanel.PlayerInfoPanelData;
using UnityEngine.UI;
using System;
using System.Linq;

public class FootballerTableDataRow : SerializedMonoBehaviour
{
    [SerializeField] Dictionary<FootballerFieldType, TableDataField> _Fields;
    [SerializeField] LayoutGroup _LayoutGroup;

    Footballer _footballer;
    InfoContext _playerInfoContext;

    public void SetData(Footballer footballer, string tournamentFilter = "", InfoContext playerInfoContext = InfoContext.None)
    {
        _playerInfoContext = playerInfoContext;
        _footballer = footballer;
        int knowledgeLevel = MyClub.Instance.GetKnowledgeOfPlayer(footballer.Id);

        _Fields[FootballerFieldType.Position].SetTextData(footballer.Pos.ToString());
        _Fields[FootballerFieldType.Nationality].SetImageData(Database.Instance.CountryMaster.GetFlagByName(footballer.Country));
        _Fields[FootballerFieldType.Name].SetTextData(footballer.Name);
        _Fields[FootballerFieldType.Surname].SetTextData(footballer.Surname);
        _Fields[FootballerFieldType.NameAndSurname].SetTextData(footballer.FullName);
        _Fields[FootballerFieldType.Rating].SetTextData(
            knowledgeLevel >= Footballer.RATING_KNOWLEDGE_LEVEL ? footballer.Rating.ToString() : "?");
        _Fields[FootballerFieldType.RatingStars].SetTextData(
            knowledgeLevel >= Footballer.RATING_KNOWLEDGE_LEVEL ? footballer.Rating.ToString() : "?");
        _Fields[FootballerFieldType.RatingStars].SetImageData(Resources.Load<Sprite>(
            knowledgeLevel >= Footballer.RATING_KNOWLEDGE_LEVEL ? $"Stars/{Mathf.Max(1, Mathf.RoundToInt(footballer.Rating/10))}" : "Stars/0"));

        if (footballer.ClubID != -1)
        {
            _Fields[FootballerFieldType.Club].SetImageData(Database.Instance.CountryMaster.GetFlagByName(Database.clubDB[footballer.ClubID].CountryName));
            _Fields[FootballerFieldType.Club].SetTextData(Database.clubDB[footballer.ClubID].Name);
        }
        else
        {
            _Fields[FootballerFieldType.Club].SetImageData(null);
            _Fields[FootballerFieldType.Club].SetTextData("Free Agent");
        }

        _Fields[FootballerFieldType.Shoot].SetTextData(
            knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? footballer.Shoot.ToString() : "?");
        _Fields[FootballerFieldType.Pass].SetTextData(
            knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? footballer.Pass.ToString() : "?");
        _Fields[FootballerFieldType.Dribling].SetTextData(
            knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? footballer.Dribling.ToString() : "?");
        _Fields[FootballerFieldType.Tackle].SetTextData(
            knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? footballer.Tackle.ToString() : "?");
        _Fields[FootballerFieldType.Heading].SetTextData(
            knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? footballer.Heading.ToString() : "?");
        _Fields[FootballerFieldType.Speed].SetTextData(
            knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? footballer.Speed.ToString() : "?");
        _Fields[FootballerFieldType.Endurance].SetTextData(
            knowledgeLevel >= Footballer.STATS_KNOWLEDGE_LEVEL ? footballer.Endurance.ToString() : "?");
        _Fields[FootballerFieldType.Condition].SetImageFillAmount(footballer.Condition / 100);

        if(_Fields[FootballerFieldType.Perks] is PerksTableDataField perksDataField)
            perksDataField.SetPerks(
                knowledgeLevel >= Footballer.PERK_KNOWLEDGE_LEVEL ? footballer.Perks : Enumerable.Empty<Perk>());
        UpdateStatistics(tournamentFilter);
    }

    public void UpdateStatistics(string tournamentFilter)
    {
        var playerStatistics = _footballer.GetPlayerStatistics(tournamentFilter);
        var matchesPlayed = playerStatistics.GetStat(StatName.MatchesPlayed);
        _Fields[FootballerFieldType.MatchesPlayed].SetTextData(matchesPlayed.ToString());
        _Fields[FootballerFieldType.Goals].SetTextData(playerStatistics.GetStat(StatName.Goals).ToString());
        _Fields[FootballerFieldType.Assists].SetTextData(playerStatistics.GetStat(StatName.Assists).ToString());
        var avgRating = matchesPlayed > 0 ? Math.Round(playerStatistics.GetStat(StatName.MatchRating) / matchesPlayed, 2).ToString() : "-";
        _Fields[FootballerFieldType.AvgMatchRating].SetTextData(avgRating);
    }

    public void ShowFields(List<FootballerFieldType> fields)
    {
        TurnOffAllFields();
        RefreshThemAll(fields);
    }

    public void ShowNewField(FootballerFieldType fieldName) => SetFieldVisibility(fieldName, true);

    public void HideField(FootballerFieldType fieldName) => SetFieldVisibility(fieldName, false);

    public void TurnOffAllFields()
    {
        foreach (var field in _Fields)
            field.Value.gameObject.SetActive(false);
    }

    public void RefreshThemAll(List<FootballerFieldType> fields)
    {
        foreach (var field in fields)
            SetFieldVisibility(field, true);
    }

    public void OpenPlayerInfo()
    {
        WindowsManager.Instance.ShowWindow("PlayerInfo", new PlayerInfoPanel.PlayerInfoPanelData(_footballer, _playerInfoContext), false);
    }

    void SetFieldVisibility(FootballerFieldType fieldName, bool setTo)
    {
        if (_Fields.ContainsKey(fieldName))
            _Fields[fieldName].gameObject.SetActive(setTo);
    }
}
