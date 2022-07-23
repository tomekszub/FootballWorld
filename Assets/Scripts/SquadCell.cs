using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SquadCell : MonoBehaviour, IPointerDownHandler
{
    Footballer playerOnPosition = null;
    static GameObject selectedFootballer = null;
    // Start is called before the first frame update
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + " Was Clicked.");
        if (selectedFootballer == null)
        {
            selectedFootballer = gameObject;
            GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }
        else
        {
            if (selectedFootballer != gameObject)
            {
                Footballer temp = selectedFootballer.GetComponent<SquadCell>().playerOnPosition;
                selectedFootballer.GetComponent<SquadCell>().SetFootballer(playerOnPosition);
                SetFootballer(temp);
            }
            GetComponent<TextMeshProUGUI>().color = Color.white;
            selectedFootballer = null;
        }
        
    }
    public int GetFootballerID()
    {
        return playerOnPosition.Id;
    }
    public void SetFootballer(Footballer f)
    {
        playerOnPosition = f;
        GetComponent<TextMeshProUGUI>().SetText(playerOnPosition.Surname);
        GetComponent<TextMeshProUGUI>().color = Color.white;
    }
}
