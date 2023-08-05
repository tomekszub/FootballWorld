using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SquadTacticPanel : BasePanel
{
    [SerializeField] Dropdown _FormationDropDown;
    [SerializeField] GameObject _SquadCell;
    [SerializeField] GameObject _EmptyCell;
    [SerializeField] GameObject _SquadPanelsParent;
    [SerializeField] GameObject _SubstitutesParent;
    [Header("Squad Presets")]
    [SerializeField] Color _ActivePresetBackgroundColor;
    [SerializeField] List<Image> _SquadPresetsButtons;

    int _currSquadPreset = 0;
    List<(string, List<int>)> _temporaryPresets;

    void OnEnable()
    {
        _temporaryPresets = new(MyClub.Instance.SquadPresets);
        // creating new lists to avoid editing existing ones
        _temporaryPresets.ForEach(x => x.Item2 = new(x.Item2));

        ChangeSquadPreset(MyClub.Instance.ActiveSquadPresetIndex);
    }

    public void FormationDropDownValue(int val)
    {
        UpdateSquadScreen(_FormationDropDown.options[val].text);
    }

    public void UpdateSquadScreen(string formation = "")
    {
        ClearSquadCells();

        var ids = _temporaryPresets[_currSquadPreset].Item2;

        for (int i = 11; i < ids.Count; i++)
            SpawnFootballerCell(ids[i], _SubstitutesParent.transform);

        if (formation == "") 
            formation = MyClub.Instance.Club.Formation;

        if (formation == "4-3-3")
        {
            SpawnFootballerCell(ids[6], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[5], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[7], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[8], _SquadPanelsParent.transform.GetChild(0));
            SpawnFootballerCell(ids[10], _SquadPanelsParent.transform.GetChild(0));
            SpawnFootballerCell(ids[9], _SquadPanelsParent.transform.GetChild(0));
        }
        else if (formation == "4-2-3-1")
        {
            SpawnFootballerCell(ids[5], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[6], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[8], _SquadPanelsParent.transform.GetChild(1));
            SpawnFootballerCell(ids[7], _SquadPanelsParent.transform.GetChild(1));
            SpawnFootballerCell(ids[9], _SquadPanelsParent.transform.GetChild(1));
            SpawnFootballerCell(ids[10], _SquadPanelsParent.transform.GetChild(0));

        }
        else if (formation == "4-4-1-1")
        {
            SpawnFootballerCell(ids[7], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[5], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[6], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[8], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[9], _SquadPanelsParent.transform.GetChild(1));
            SpawnFootballerCell(ids[10], _SquadPanelsParent.transform.GetChild(0));
        }
        else if (formation == "4-1-4-1")
        {
            SpawnFootballerCell(ids[5], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[8], _SquadPanelsParent.transform.GetChild(1));
            SpawnFootballerCell(ids[6], _SquadPanelsParent.transform.GetChild(1));
            SpawnFootballerCell(ids[7], _SquadPanelsParent.transform.GetChild(1));
            SpawnFootballerCell(ids[9], _SquadPanelsParent.transform.GetChild(1));
            SpawnFootballerCell(ids[10], _SquadPanelsParent.transform.GetChild(0));
        }
        else if (formation == "4-3-1-2")
        {
            SpawnFootballerCell(ids[6], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[5], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[7], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[8], _SquadPanelsParent.transform.GetChild(1));
            SpawnFootballerCell(ids[9], _SquadPanelsParent.transform.GetChild(0));
            SpawnFootballerCell(ids[10], _SquadPanelsParent.transform.GetChild(0));
        }
        else if (formation == "4-4-2")
        {
            SpawnFootballerCell(ids[7], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[5], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[6], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[8], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[9], _SquadPanelsParent.transform.GetChild(0));
            SpawnFootballerCell(ids[10], _SquadPanelsParent.transform.GetChild(0));
        }
        else if (formation == "5-3-2")
        {
            SpawnFootballerCell(ids[7], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[6], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[8], _SquadPanelsParent.transform.GetChild(1));
            SpawnFootballerCell(ids[9], _SquadPanelsParent.transform.GetChild(0));
            SpawnFootballerCell(ids[10], _SquadPanelsParent.transform.GetChild(0));
        }
        else if (formation == "3-4-1-2")
        {
            SpawnFootballerCell(ids[6], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[4], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[5], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[7], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[8], _SquadPanelsParent.transform.GetChild(1));
            SpawnFootballerCell(ids[9], _SquadPanelsParent.transform.GetChild(0));
            SpawnFootballerCell(ids[10], _SquadPanelsParent.transform.GetChild(0));
        }
        else if (formation == "3-4-2-1")
        {
            SpawnFootballerCell(ids[6], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[4], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[5], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[7], _SquadPanelsParent.transform.GetChild(2));
            SpawnFootballerCell(ids[8], _SquadPanelsParent.transform.GetChild(1));
            SpawnFootballerCell(ids[9], _SquadPanelsParent.transform.GetChild(1));
            SpawnFootballerCell(ids[10], _SquadPanelsParent.transform.GetChild(0));
        }

        for (int i = 1; i <= int.Parse(formation[0].ToString()); i++)
            SpawnFootballerCell(ids[i], _SquadPanelsParent.transform.GetChild(4));

        SpawnFootballerCell(ids[0], _SquadPanelsParent.transform.GetChild(5));
    }

    public void SaveCurrentPreset()
    {
        List<int> ids = Enumerable.Repeat(-1, MyClub.Instance.Club.FootballersIDs.Count).ToList();

        string formation = _FormationDropDown.options[_FormationDropDown.value].text;

        for (int i = 11; i < ids.Count; i++)
        {
            ids[i] = _SubstitutesParent.transform.GetChild(i - 11).GetComponent<SquadCell>().GetFootballerID();
        }
        // save
        ids[0] = _SquadPanelsParent.transform.GetChild(5).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
        int defendersCount = int.Parse(formation[0].ToString());
        for (int i = 0; i < defendersCount; i++)
        {
            ids[i + 1] = _SquadPanelsParent.transform.GetChild(4).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
        }

        if (formation.Length == 5)  // formacje 4-3-3 4-4-2 3-5-2 itd.
        {
            int midFieldNumber = int.Parse(formation[2].ToString());
            if (midFieldNumber == 1)
            {
                // nie ma poki co i chyba nie bedzie formacji 4-1-5
            }
            else
            {
                int inTheMiddle = midFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    ids[defendersCount + i] = _SquadPanelsParent.transform.GetChild(2).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                ids[defendersCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                ids[defendersCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(2).GetChild(midFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int attackersNumber = int.Parse(formation[4].ToString());
            int defAndMidCount = defendersCount + midFieldNumber;
            if (attackersNumber == 1)
            {
                ids[10] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = attackersNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    ids[defAndMidCount + inTheMiddle + i + 1] = _SquadPanelsParent.transform.GetChild(0).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                ids[defAndMidCount + 1] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                ids[defAndMidCount + 2] = _SquadPanelsParent.transform.GetChild(0).GetChild(attackersNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
        }
        else if (formation.Length == 7)
        {
            int defmidFieldNumber = int.Parse(formation[2].ToString());
            if (defmidFieldNumber == 1)
            {
                // 4-1-3-2
                ids[defendersCount + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = defmidFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    ids[defendersCount + i] = _SquadPanelsParent.transform.GetChild(2).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                ids[defendersCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                ids[defendersCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(2).GetChild(defmidFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int midFieldNumber = int.Parse(formation[4].ToString());
            if (midFieldNumber == 1)
            {
                // 4-3-1-2
                ids[defendersCount + defmidFieldNumber + 1] = _SquadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = midFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    ids[defendersCount + defmidFieldNumber + i] = _SquadPanelsParent.transform.GetChild(1).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                ids[defendersCount + defmidFieldNumber + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                ids[defendersCount + defmidFieldNumber + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(1).GetChild(midFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int attackersNumber = int.Parse(formation[6].ToString());
            int defAndMidCount = defendersCount + defmidFieldNumber + midFieldNumber;
            if (attackersNumber == 1)
            {
                ids[10] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = attackersNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    ids[defAndMidCount + inTheMiddle + i] = _SquadPanelsParent.transform.GetChild(0).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                ids[defAndMidCount + 1] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                ids[defAndMidCount + 2] = _SquadPanelsParent.transform.GetChild(0).GetChild(attackersNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
        }
        else if (formation.Length == 9)
        {
            int defmidFieldNumber = int.Parse(formation[2].ToString());
            if (defmidFieldNumber == 1)
            {
                // 4-1-2-1-2
                ids[defendersCount + 1] = _SquadPanelsParent.transform.GetChild(3).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = defmidFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    ids[defendersCount + i] = _SquadPanelsParent.transform.GetChild(3).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                ids[defendersCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(3).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                ids[defendersCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(3).GetChild(defmidFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int midFieldNumber = int.Parse(formation[4].ToString());
            int defmidAndDefCount = defendersCount + defmidFieldNumber;
            if (midFieldNumber == 1)
            {
                // 4-2-1-2-1
                ids[defmidAndDefCount + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = midFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    ids[defendersCount + i] = _SquadPanelsParent.transform.GetChild(2).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                ids[defmidAndDefCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                ids[defmidAndDefCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(2).GetChild(midFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int atkMidFieldNumber = int.Parse(formation[6].ToString());
            int midAndDefCount = defmidAndDefCount + midFieldNumber;
            if (atkMidFieldNumber == 1)
            {
                // 4-1-3-1-2
                ids[midAndDefCount + 1] = _SquadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = atkMidFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    ids[midAndDefCount + i] = _SquadPanelsParent.transform.GetChild(1).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                ids[midAndDefCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                ids[midAndDefCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(1).GetChild(atkMidFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int attackersNumber = int.Parse(formation[8].ToString());
            int defAndMidCount = midAndDefCount + atkMidFieldNumber;
            if (attackersNumber == 1)
            {
                ids[10] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = attackersNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    ids[defAndMidCount + inTheMiddle + i] = _SquadPanelsParent.transform.GetChild(0).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                ids[defAndMidCount + 1] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                ids[defAndMidCount + 2] = _SquadPanelsParent.transform.GetChild(0).GetChild(attackersNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
        }

        _temporaryPresets[_currSquadPreset] = (formation, ids);
    }

    public void SaveData()
    {
        SaveCurrentPreset();

        MyClub.Instance.Club.FootballersIDs = _temporaryPresets[_currSquadPreset].Item2;

        MyClub.Instance.Club.Formation = _temporaryPresets[_currSquadPreset].Item1;

        MyClub.Instance.SaveSquadPresets(_temporaryPresets, _currSquadPreset);

        WindowsManager.Instance.ShowWindow("Club");
    }

    public void SquadPresetChangedViaButton(int number)
    {
        SaveCurrentPreset();
        ChangeSquadPreset(number);
    }

    void ChangeSquadPreset(int number)
    {
        _currSquadPreset = number;

        SetDropdownToFormation(_temporaryPresets[_currSquadPreset].Item1);

        UpdateSquadScreen(_temporaryPresets[_currSquadPreset].Item1);

        SetActivePresetButton(number);
    }

    void SetDropdownToFormation(string formation)
    {
        // set dropdown value to current formation
        for (int i = 0; i < _FormationDropDown.options.Count; i++)
        {
            if (_FormationDropDown.options[i].text == formation)
            {
                _FormationDropDown.value = i;
                break;
            }
        }
    }

    void ClearSquadCells()
    {
        for (int x = 0; x < _SquadPanelsParent.transform.childCount; x++)
        {
            int limit = _SquadPanelsParent.transform.GetChild(x).childCount;
            for (int i = limit - 1; i >= 0; i--)
            {
                Destroy(_SquadPanelsParent.transform.GetChild(x).GetChild(i).gameObject);
            }
        }
        int end = _SubstitutesParent.transform.childCount;
        for (int i = end - 1; i >= 0; i--)
        {
            Destroy(_SubstitutesParent.transform.GetChild(i).gameObject);
        }
    }

    void SetActivePresetButton(int index)
    {
        for (int i = 0; i < _SquadPresetsButtons.Count; i++)
            _SquadPresetsButtons[i].color = i == index ? _ActivePresetBackgroundColor : Color.white;
    }

    void SpawnFootballerCell(int footballerId, Transform parent)
    {
        GameObject g = Instantiate(_SquadCell, parent);
        g.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[footballerId]);
    }
}
