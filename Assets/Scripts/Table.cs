using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Table : MonoBehaviour
{
    [SerializeField] List<TableRow> _TableRows;

	readonly Color32 topPositionsColor = new Color32(8,249,0,255);
	readonly Color32 topPositionsColor2 = new Color32(255,255,0,255);
	readonly Color32 topPositionsColor3 = new Color32(255,153,51,255);
	readonly Color32 bottomPositionsColor = new Color32(255,102,102,255);

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
	public void ShowTable(List<Team> tableTeams, int[] positionRanges, bool showFlags = false)
	{
        WindowsManager.Instance.ShowWindow("Table");
		tableTeams = SortTeamsTableByGoalDifference (tableTeams);
	    Color32 rowColor;
		int posRange2 = positionRanges[0] + positionRanges[1];
		int posRange3 = posRange2 + positionRanges[2];
		int posRange4 = tableTeams.Count - positionRanges[3];
		for (int i = 0; i < _TableRows.Count; i++) 
		{
            if (i >= tableTeams.Count)
            {
				_TableRows[i].gameObject.SetActive(false);
                continue;
            }

			if (i < positionRanges[0])
				rowColor = topPositionsColor;
			else if (i < posRange2)
				rowColor = topPositionsColor2;
			else if (i < posRange3)
				rowColor = topPositionsColor3;
			else if (i < posRange4)
				rowColor = Color.white;
			else
				rowColor = bottomPositionsColor;

			_TableRows[i].SetTeam(tableTeams[i], rowColor, showFlags);
		}
	}
}
