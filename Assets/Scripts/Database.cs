using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;


public class Database : MonoBehaviour
{
    public static Database Instance;

	public static List<Footballer> footballersDB = new List<Footballer>();
	public static List<Club> clubDB = new List<Club>();
	public static List<League_Old> leagueDB = new List<League_Old>();
	public static List<Club> tempLeague = new List<Club>();
	public static string[,] arbiterDB = new string[2,5];
	public GameObject nameInput, surnameInput, countryInput;
	public GameObject playerPanelPrefab;
    public GameObject searchResultContent;
    public GameObject warningText;
    [SerializeField] CountryMaster _CountryMaster;
    [SerializeField] LeagueMaster _LeagueMaster;
    [SerializeField] LeagueGenerator _LeagueGenerator;

    void Awake () 
	{
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        else
            Instance = this;

        arbiterDB [0, 0] = "44";
		arbiterDB [1, 0] = "Howard Webb";
		arbiterDB [0, 1] = "46";
		arbiterDB [1, 1] = "Stephane Lannoy";
		arbiterDB [0, 2] = "46";
		arbiterDB [1, 2] = "Wolfgang Stark";
		arbiterDB [0, 3] = "40";
		arbiterDB [1, 3] = "Victor Kassai";
		arbiterDB [0, 4] = "44";
		arbiterDB [1, 4] = "Nicola Rizzoli";

        _LeagueGenerator.Generate("Spain", 16);
        _LeagueGenerator.Generate("England", 16);
        _LeagueGenerator.Generate("Italy", 16);
        _LeagueGenerator.Generate("Germany", 14);
        _LeagueGenerator.Generate("France", 14);
        _LeagueGenerator.Generate("Russia", 12);
        _LeagueGenerator.Generate("Portugal", 12);
        _LeagueGenerator.Generate("Belgium", 12);
        _LeagueGenerator.Generate("Ukraine", 12);
        _LeagueGenerator.Generate("Turkey", 12);
        _LeagueGenerator.Generate("Netherlands", 12);
        _LeagueGenerator.Generate("Austria", 12);
        _LeagueGenerator.Generate("Czechia", 12);
        _LeagueGenerator.Generate("Greece", 12);
        _LeagueGenerator.Generate("Croatia", 10);
        _LeagueGenerator.Generate("Denmark", 12);
        _LeagueGenerator.Generate("Switzerland", 10);
        _LeagueGenerator.Generate("Cyprus", 10);
        _LeagueGenerator.Generate("Serbia", 12);
        _LeagueGenerator.Generate("Scotland", 12);
        _LeagueGenerator.Generate("Belarus", 12);
        _LeagueGenerator.Generate("Sweden", 12);
        _LeagueGenerator.Generate("Norway", 12);
        _LeagueGenerator.Generate("Kazakhstan", 10);
        _LeagueGenerator.Generate("Poland", 12);
        _LeagueGenerator.Generate("Azerbaijan", 10);
        _LeagueGenerator.Generate("Israel", 12);
        _LeagueGenerator.Generate("Bulgaria", 10);
        _LeagueGenerator.Generate("Romania", 12);
        _LeagueGenerator.Generate("Slovakia", 12);
        _LeagueGenerator.Generate("Slovenia", 10);
        _LeagueGenerator.Generate("Hungary", 10);
        _LeagueGenerator.Generate("Macedonia", 10);
        _LeagueGenerator.Generate("Moldova", 8);
        _LeagueGenerator.Generate("Albania", 10);
        _LeagueGenerator.Generate("Ireland", 8);
        _LeagueGenerator.Generate("Finland", 8);
        _LeagueGenerator.Generate("Iceland", 8);
        _LeagueGenerator.Generate("Bosnia", 10);
        _LeagueGenerator.Generate("Lithuania", 8);
        _LeagueGenerator.Generate("Latvia", 8);
        _LeagueGenerator.Generate("Luxembourg", 8);
        _LeagueGenerator.Generate("Armenia", 8);
        _LeagueGenerator.Generate("Malta", 8);
        _LeagueGenerator.Generate("Estonia", 8);
        _LeagueGenerator.Generate("Georgia", 8);
        _LeagueGenerator.Generate("Wales", 8);
        _LeagueGenerator.Generate("Montenegro", 8);
        _LeagueGenerator.Generate("Faroe Islands", 8);
        _LeagueGenerator.Generate("Gibraltar", 8);
        _LeagueGenerator.Generate("Northern Ireland", 8);
        _LeagueGenerator.Generate("Kosovo", 8);
        _LeagueGenerator.Generate("Andorra", 8);
        _LeagueGenerator.Generate("San Marino", 8);
    }

    public CountryMaster GetCountryMaster() => _CountryMaster;

	public void Search()
	{
		ClearPanels ();
		warningText.SetActive (false);
		//print (footballersDB.Count);
		for (int i = 0; i < footballersDB.Count; i++) 
		{
			//print(footballersDB[i].Surname);
			if(footballersDB[i].Surname == surnameInput.transform.Find("Text").GetComponent<Text>().text || surnameInput.transform.Find("Text").GetComponent<Text>().text == "")
			{
				//print ("raz");
				if(footballersDB[i].Name == nameInput.transform.Find("Text").GetComponent<Text>().text || nameInput.transform.Find("Text").GetComponent<Text>().text == "")
				{
					//print ("dwa");
					if(footballersDB[i].Country == countryInput.transform.Find("Text").GetComponent<Text>().text || countryInput.transform.Find("Text").GetComponent<Text>().text == "")
					{
						//print ("trzy");
						if(surnameInput.transform.Find("Text").GetComponent<Text>().text != "" || nameInput.transform.Find("Text").GetComponent<Text>().text != "" || countryInput.transform.Find("Text").GetComponent<Text>().text != "")
						{
							GameObject go = Instantiate(playerPanelPrefab);
							go.transform.SetParent(searchResultContent.transform);
							inputData(i, go);
						}
					}
				}
			}
		}
	}
    public static Club GetClubByID(int id)
    {
        for (int i = 0; i < clubDB.Count; i++)
        {
            if (clubDB[i].Id == id) return clubDB[i];
        }
        return null;
    }
	void ClearPanels()
	{
        for(int i = searchResultContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(searchResultContent.transform.GetChild(i).gameObject);
        }
	}
	void inputData(int i, GameObject go)
	{
		go.transform.Find("Name").GetComponent<Text>().text =  footballersDB[i].Name;
		go.transform.Find("Rate").GetComponent<Image>().sprite =  Resources.Load<Sprite>("Stars/" + (int)footballersDB[i].Rating);
		go.transform.Find("Surname").GetComponent<Text>().text =  footballersDB[i].Surname;
		go.transform.Find("FreeKicks").GetComponent<Text>().text =  "Stałe fragmenty gry: " + footballersDB[i].FreeKicks.ToString();
		go.transform.Find("Flag").GetComponent<Image>().sprite =  _CountryMaster.GetFlagByName(footballersDB[i].Country);
		go.transform.Find("Country").GetComponent<Text>().text =  footballersDB[i].Country;
		go.transform.Find("Position").GetComponent<Text>().text =  footballersDB[i].Pos.ToString();
		go.transform.Find("Stats1").GetComponent<Text>().text =  "Drybling: " + footballersDB[i].Dribling.ToString() + "\n";
		go.transform.Find("Stats1").GetComponent<Text>().text +=  "Odbiór: " + footballersDB[i].Tackle.ToString() + "\n";
		go.transform.Find("Stats1").GetComponent<Text>().text +=  "Strzały: " + footballersDB[i].Shoot.ToString();
		go.transform.Find("Stats2").GetComponent<Text>().text =  "Główki: " + footballersDB[i].Heading.ToString() + "\n";
		go.transform.Find("Stats2").GetComponent<Text>().text +=  "Podania: " + footballersDB[i].Pass.ToString() + "\n";
		go.transform.Find("Stats2").GetComponent<Text>().text +=  "Szybkosć: " + footballersDB[i].Speed.ToString();
	}
}