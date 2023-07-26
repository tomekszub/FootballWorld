using UnityEngine;
using UnityEngine.UI;

public class SquadTacticPanel : BasePanel
{
    [SerializeField] Dropdown _FormationDropDown;
    [SerializeField] GameObject _SquadCell, _EmptyCell;
    [SerializeField] GameObject _SquadPanelsParent, _SubstitutesParent;

    public void FormationDropDownValue(int val)
    {
        UpdateSquadScreen(_FormationDropDown.options[val].text);
    }

    void OnEnable()
    {
        // set dropdown value to current formation
        for (int i = 0; i < _FormationDropDown.options.Count; i++)
        {
            if (_FormationDropDown.options[i].text == Database.clubDB[MyClub.Instance.MyClubID].Formation)
            {
                _FormationDropDown.value = i;
                break;
            }
        }
        UpdateSquadScreen();
    }

    public void UpdateSquadScreen(string formation = "")
    {
        ClearSquadCells();
        for (int i = 11; i < Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs.Count; i++)
        {
            GameObject g = Instantiate(_SquadCell, _SubstitutesParent.transform);
            g.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[i]]);
        }
        if (formation == "") formation = Database.clubDB[MyClub.Instance.MyClubID].Formation;
        GameObject go;
        if (formation == "4-3-3")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[10]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[9]]);
        }
        else if (formation == "4-2-3-1")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[10]]);

        }
        else if (formation == "4-4-1-1")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[10]]);
        }
        else if (formation == "4-1-4-1")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[10]]);
        }
        else if (formation == "4-3-1-2")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[10]]);
        }
        else if (formation == "4-4-2")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[10]]);

        }
        else if (formation == "5-3-2")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[10]]);
        }
        else if (formation == "3-4-1-2")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[4]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[10]]);
        }
        else if (formation == "3-4-2-1")
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[6]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[4]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[5]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(2));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[7]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[8]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(1));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[9]]);
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(0));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[10]]);
        }
        for (int i = 1; i <= int.Parse(formation[0].ToString()); i++)
        {
            go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(4));
            go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[i]]);
        }
        go = Instantiate(_SquadCell, _SquadPanelsParent.transform.GetChild(5));
        go.GetComponent<SquadCell>().SetFootballer(Database.footballersDB[Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[0]]);
    }

    public void SetMySquad()
    {
        string formation = _FormationDropDown.options[_FormationDropDown.value].text;
        for (int i = 11; i < Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs.Count; i++)
        {
            Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[i] = _SubstitutesParent.transform.GetChild(i - 11).GetComponent<SquadCell>().GetFootballerID();
        }
        // save
        Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[0] = _SquadPanelsParent.transform.GetChild(5).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
        int defendersCount = int.Parse(formation[0].ToString());
        for (int i = 0; i < defendersCount; i++)
        {
            Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[i + 1] = _SquadPanelsParent.transform.GetChild(4).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
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
                    Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + i] = _SquadPanelsParent.transform.GetChild(2).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(2).GetChild(midFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int attackersNumber = int.Parse(formation[4].ToString());
            int defAndMidCount = defendersCount + midFieldNumber;
            if (attackersNumber == 1)
            {
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[10] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = attackersNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defAndMidCount + inTheMiddle + i + 1] = _SquadPanelsParent.transform.GetChild(0).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defAndMidCount + 1] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defAndMidCount + 2] = _SquadPanelsParent.transform.GetChild(0).GetChild(attackersNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
        }
        else if (formation.Length == 7)
        {
            int defmidFieldNumber = int.Parse(formation[2].ToString());
            if (defmidFieldNumber == 1)
            {
                // 4-1-3-2
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = defmidFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + i] = _SquadPanelsParent.transform.GetChild(2).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(2).GetChild(defmidFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int midFieldNumber = int.Parse(formation[4].ToString());
            if (midFieldNumber == 1)
            {
                // 4-3-1-2
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + defmidFieldNumber + 1] = _SquadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = midFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + defmidFieldNumber + i] = _SquadPanelsParent.transform.GetChild(1).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + defmidFieldNumber + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + defmidFieldNumber + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(1).GetChild(midFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int attackersNumber = int.Parse(formation[6].ToString());
            int defAndMidCount = defendersCount + defmidFieldNumber + midFieldNumber;
            if (attackersNumber == 1)
            {
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[10] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = attackersNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defAndMidCount + inTheMiddle + i] = _SquadPanelsParent.transform.GetChild(0).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defAndMidCount + 1] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defAndMidCount + 2] = _SquadPanelsParent.transform.GetChild(0).GetChild(attackersNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
        }
        else if (formation.Length == 9)
        {
            int defmidFieldNumber = int.Parse(formation[2].ToString());
            if (defmidFieldNumber == 1)
            {
                // 4-1-2-1-2
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + 1] = _SquadPanelsParent.transform.GetChild(3).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = defmidFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + i] = _SquadPanelsParent.transform.GetChild(3).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(3).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(3).GetChild(defmidFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int midFieldNumber = int.Parse(formation[4].ToString());
            int defmidAndDefCount = defendersCount + defmidFieldNumber;
            if (midFieldNumber == 1)
            {
                // 4-2-1-2-1
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defmidAndDefCount + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = midFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defendersCount + i] = _SquadPanelsParent.transform.GetChild(2).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defmidAndDefCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(2).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defmidAndDefCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(2).GetChild(midFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int atkMidFieldNumber = int.Parse(formation[6].ToString());
            int midAndDefCount = defmidAndDefCount + midFieldNumber;
            if (atkMidFieldNumber == 1)
            {
                // 4-1-3-1-2
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[midAndDefCount + 1] = _SquadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = atkMidFieldNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[midAndDefCount + i] = _SquadPanelsParent.transform.GetChild(1).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[midAndDefCount + inTheMiddle + 1] = _SquadPanelsParent.transform.GetChild(1).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[midAndDefCount + inTheMiddle + 2] = _SquadPanelsParent.transform.GetChild(1).GetChild(atkMidFieldNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
            int attackersNumber = int.Parse(formation[8].ToString());
            int defAndMidCount = midAndDefCount + atkMidFieldNumber;
            if (attackersNumber == 1)
            {
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[10] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
            }
            else
            {
                int inTheMiddle = attackersNumber - 2;
                for (int i = 1; i <= inTheMiddle; i++)
                {
                    Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defAndMidCount + inTheMiddle + i] = _SquadPanelsParent.transform.GetChild(0).GetChild(i).GetComponent<SquadCell>().GetFootballerID();
                }
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defAndMidCount + 1] = _SquadPanelsParent.transform.GetChild(0).GetChild(0).GetComponent<SquadCell>().GetFootballerID();
                Database.clubDB[MyClub.Instance.MyClubID].FootballersIDs[defAndMidCount + 2] = _SquadPanelsParent.transform.GetChild(0).GetChild(attackersNumber - 1).GetComponent<SquadCell>().GetFootballerID();
            }
        }
        Database.clubDB[MyClub.Instance.MyClubID].Formation = formation;
        WindowsManager.Instance.ShowWindow("Club");
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
}
