using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	public Database database;
	public GameObject moneyText, managerNameText, clubNameText;
	public List<Footballer> squadPlayers;
	public bool turnedOn;
    
	// Use this for initialization
	void Start () 
	{
        if (Database.clubDB[0].Id == 0) 
			clubNameText.transform.GetComponent<Text>().text = Database.clubDB[0].Name;
        else 
			Debug.LogError("Cos trzeba z tym zrobic. Najlepiej przetrzymywac gdzies id kierowanego zespolu");
    }
}
