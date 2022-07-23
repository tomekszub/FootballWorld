using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


public class Table : MonoBehaviour
{
	static List<Team> tempTeams = new List<Team> ();


	//public GameObject tableRowPrefab;
	public List<GameObject> tableRowPrefabs;
	public List<Team> tableTeams = new List<Team>();



	public static List<Team> SortTeamsTableByGoalDifference(List<Team> teams)
	{
        teams = teams.OrderByDescending(n => n.Points).ToList();
		// Table must be sorted by descending number of points before calling this method
		List<Team> returnTeams = new List<Team> ();
		for (int x = 0; x < teams.Count; x++) 
		{
			tempTeams.Add(teams[x]);
			for (int i = x; i < teams.Count; i++) 
			{
				if (teams[x].Points == teams[i].Points && x != i)
				{
					tempTeams.Add(teams[i]);
                    //teams.RemoveAt(i);  // dodalem dla testu
				}
			}
			if (tempTeams.Count > 1) 
			{
				tempTeams = tempTeams.OrderByDescending(n => n.DifferenceGoals).ToList();
				x += tempTeams.Count - 1;
			}
			for (int i = 0; i < tempTeams.Count; i++) 
			{
				returnTeams.Add(tempTeams[i]);
			}
			tempTeams.Clear();
		}
		return returnTeams;
	}
	void Start()
	{
		//Debug.Log("GameObject name: " + gameObject.name);
		//print ("st");
	}
	public void ShowTable()
	{
        WindowsManager.Instance.ShowWindow("Table");
		tableTeams = SortTeamsTableByGoalDifference (tableTeams);
		for (int i = 0; i < tableRowPrefabs.Count; i++) 
		{
            //print (tableRowPrefabs[0].name);
            //Debug.Log();
            //GameObject go = (GameObject)Instantiate(tableRowPrefab);
            //go.transform.SetParent(this.transform);
            if (i >= tableTeams.Count)
            {
                tableRowPrefabs[i].SetActive(false);
                continue;
            }
			tableRowPrefabs[i].transform.Find("Name").GetComponent<Text>().text =  (i + 1) + " " + tableTeams[i].Name;
			tableRowPrefabs[i].transform.Find("Points").GetComponent<Text>().text = tableTeams[i].Points.ToString();
			tableRowPrefabs[i].transform.Find("MatchesPlayed").GetComponent<Text>().text = tableTeams[i].MatchesPlayed.ToString();
			tableRowPrefabs[i].transform.Find("Wins").GetComponent<Text>().text = tableTeams[i].Wins.ToString();
			tableRowPrefabs[i].transform.Find("Draws").GetComponent<Text>().text = tableTeams[i].Draws.ToString();
			tableRowPrefabs[i].transform.Find("Loses").GetComponent<Text>().text = tableTeams[i].Loses.ToString();
			tableRowPrefabs[i].transform.Find("GoalsScored").GetComponent<Text>().text = tableTeams[i].ScoredGoals.ToString();
			tableRowPrefabs[i].transform.Find("GoalsLost").GetComponent<Text>().text = tableTeams[i].LostGoals.ToString();
			tableRowPrefabs[i].transform.Find("GoalsDifference").GetComponent<Text>().text = tableTeams[i].DifferenceGoals.ToString();
			//inputData(i, go);
		}
	}
	void Update()
	{
		//print (tableRowPrefabs.Count + " i liczba druzyn to " + tableTeams.Count);
	}
	/*void inputData(int i, GameObject go)
	{
		go.transform.Find("Name").GetComponent<Text>().text =  (i + 1) + " " + tableTeams[i].Name;
		/*go.transform.Find("Surname").GetComponent<Text>().text =  footballersDB[i].Surname;
		go.transform.Find("FreeKicks").GetComponent<Text>().text =  "Stałe fragmenty gry: " + footballersDB[i].FreeKicks.ToString();
		go.transform.Find("Flag").GetComponent<Image>().sprite =  footballersDB[i].Flag;
		go.transform.Find("Country").GetComponent<Text>().text =  footballersDB[i].Country;
		go.transform.Find("Position").GetComponent<Text>().text =  footballersDB[i].Pos.ToString();
		go.transform.Find("Stats1").GetComponent<Text>().text =  "Drybling: " + footballersDB[i].Dribling.ToString() + "\n";
		go.transform.Find("Stats1").GetComponent<Text>().text +=  "Odbiór: " + footballersDB[i].Tackle.ToString() + "\n";
		go.transform.Find("Stats1").GetComponent<Text>().text +=  "Strzały: " + footballersDB[i].Shoot.ToString();
		go.transform.Find("Stats2").GetComponent<Text>().text =  "Główki: " + footballersDB[i].Heading.ToString() + "\n";
		go.transform.Find("Stats2").GetComponent<Text>().text +=  "Podania: " + footballersDB[i].Pass.ToString() + "\n";
		go.transform.Find("Stats2").GetComponent<Text>().text +=  "Szybkosć: " + footballersDB[i].Speed.ToString();
	}*/
}
