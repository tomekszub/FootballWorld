using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static FootballerTableData;
using static Footballer.PlayerStatistics;
using System.Collections;
using UnityEngine.UI;
using System;

public class FootballerTableDataRow : SerializedMonoBehaviour
{
    [SerializeField] Dictionary<FootballerFieldType, TableDataField> _Fields;
    [SerializeField] LayoutGroup _LayoutGroup;

    Footballer _footballer;

    public void SetData(Footballer footballer, string tournamentFilter = "")
    {
        _footballer = footballer;
        _Fields[FootballerFieldType.Position].SetTextData(footballer.Pos.ToString());
        _Fields[FootballerFieldType.Nationality].SetImageData(Database.Instance.CountryMaster.GetFlagByName(footballer.Country));
        _Fields[FootballerFieldType.Name].SetTextData(footballer.Name);
        _Fields[FootballerFieldType.Surname].SetTextData(footballer.Surname);
        _Fields[FootballerFieldType.NameAndSurname].SetTextData(footballer.FullName);
        _Fields[FootballerFieldType.Rating].SetTextData(footballer.Rating.ToString());
        _Fields[FootballerFieldType.RatingStars].SetTextData(footballer.Rating.ToString());
        _Fields[FootballerFieldType.RatingStars].SetImageData(Resources.Load<Sprite>($"Stars/{Mathf.Max(1, Mathf.RoundToInt(footballer.Rating/10))}"));

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

        _Fields[FootballerFieldType.Shoot].SetTextData(footballer.Shoot.ToString());
        _Fields[FootballerFieldType.Pass].SetTextData(footballer.Pass.ToString());
        _Fields[FootballerFieldType.Dribling].SetTextData(footballer.Dribling.ToString());
        _Fields[FootballerFieldType.Tackle].SetTextData(footballer.Tackle.ToString());
        _Fields[FootballerFieldType.Heading].SetTextData(footballer.Heading.ToString());
        _Fields[FootballerFieldType.Speed].SetTextData(footballer.Speed.ToString());
        _Fields[FootballerFieldType.Endurance].SetTextData(footballer.Endurance.ToString());
        _Fields[FootballerFieldType.Condition].SetImageFillAmount(footballer.Condition / 100);
        if(_Fields[FootballerFieldType.Perks] is PerksTableDataField perksDataField)
            perksDataField.SetPerks(footballer.Perks);
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

    void SetFieldVisibility(FootballerFieldType fieldName, bool setTo)
    {
        if (_Fields.ContainsKey(fieldName))
            _Fields[fieldName].gameObject.SetActive(setTo);
    }

    IEnumerator UpdateLayoutGroup(List<FootballerFieldType> fields)
    {
        TurnOffAllFields();
        yield return new WaitForEndOfFrame();
        foreach (var field in fields)
            SetFieldVisibility(field, true);
    }

    public void RefreshThemAll(List<FootballerFieldType> fields)
    {
        foreach (var field in fields)
            SetFieldVisibility(field, true);
    }
}
