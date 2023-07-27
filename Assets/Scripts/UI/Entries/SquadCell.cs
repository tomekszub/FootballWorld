using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SquadCell : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] TextMeshProUGUI _Text;

    Footballer playerOnPosition = null;
    static GameObject selectedFootballer = null;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (selectedFootballer == null)
        {
            selectedFootballer = gameObject;
            _Text.color = Color.yellow;
        }
        else
        {
            if (selectedFootballer != gameObject)
            {
                Footballer temp = selectedFootballer.GetComponent<SquadCell>().playerOnPosition;
                selectedFootballer.GetComponent<SquadCell>().SetFootballer(playerOnPosition);
                SetFootballer(temp);
            }
            _Text.color = Color.white;
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
        _Text.SetText($"{playerOnPosition.Surname} <size=40%>({playerOnPosition.Pos})");
        _Text.color = Color.white;
    }
}
