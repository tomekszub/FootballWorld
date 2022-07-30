using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


public class Table : MonoBehaviour
{
	public List<GameObject> tableRowPrefabs;
	public List<Team> tableTeams = new List<Team>();
	public static List<Team> SortTeamsTableByGoalDifference(List<Team> teams)
	{
		// Table must be sorted by descending number of points before calling this method
        var teamsOrdered = teams.OrderByDescending(n => n.Points).ToList();

		List<Team> returnTeams = new List<Team> ();
		List<Team> tempTeams = new List<Team>();

		for (int x = 0; x < teamsOrdered.Count; x++) 
		{
			tempTeams.Add(teamsOrdered[x]);
			for (int i = x; i < teamsOrdered.Count; i++) 
			{
				if (teamsOrdered[x].Points == teamsOrdered[i].Points && x != i)
				{
					tempTeams.Add(teamsOrdered[i]);
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
	public void ShowTable()
	{
        WindowsManager.Instance.ShowWindow("Table");
		tableTeams = SortTeamsTableByGoalDifference (tableTeams);
		for (int i = 0; i < tableRowPrefabs.Count; i++) 
		{
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
		}
	}
}
