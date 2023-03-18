using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FootballerTableData : SerializedMonoBehaviour
{
    public enum FootballerFieldType
    {
        Nationality = 0,
        Name = 1,
        Surname = 2,
        Club = 3,
        Rating = 4,
        Shoot = 5,
        Pass = 6,
        Dribling = 7,
        Tackle = 8,
        Heading = 9,
        Speed = 10,
        Endurance = 11,
        Condition = 12,
        Perks = 13,
        MatchesPlayed = 14,
        Goals = 15,
        Assists = 16,
        Position = 17,
        RatingStars = 18,
        NameAndSurname = 19
    }

    [SerializeField] List<FootballerFieldType> _Fields;
    [SerializeField] FootballerTableDataRow _DataRowPrefab;

    [Title("References")]
    [SerializeField] FootballerTableDataRow _Header;
    [SerializeField] HorizontalLayoutGroup _HeaderLayoutGroup;
    [SerializeField] RectTransform _DataRowsParent;

    List<FootballerTableDataRow> _TableRows = new();

    public void ShowData(List<Footballer> footballers, bool fieldsChanged = false)
    {
        if (fieldsChanged)
        {
            _Header.ShowFields(_Fields);
            _HeaderLayoutGroup.enabled = false;
            // the only way it works eh
            Invoke(nameof(RebuildLayout), 0.01f);
        }

        int index = 0;

        for (;index < footballers.Count; index++)
        {
            FootballerTableDataRow dr;
            if (index >= _TableRows.Count)
            {
                dr = Instantiate(_DataRowPrefab, _DataRowsParent);
                dr.ShowFields(_Fields);
                _TableRows.Add(dr);
            }
            else
            {
                dr = _TableRows[index];
                if (fieldsChanged)
                    dr.ShowFields(_Fields);
                _TableRows[index].gameObject.SetActive(true);
            }

            dr.SetData(footballers[index]);
        }

        for (; index < _TableRows.Count; index++)
            _TableRows[index].gameObject.SetActive(false);
    }

    public void UpdateStats(string tournamentName)
    {
        for (int i = 0; i < _TableRows.Count; i++)
            _TableRows[i].UpdateStatistics(tournamentName);
    }

    void RebuildLayout() => _HeaderLayoutGroup.enabled = true;
}
