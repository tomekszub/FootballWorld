using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;

public class Transfers_PlayerCard : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Click");
        }
    }
}
