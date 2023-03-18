using UnityEngine;
using UnityEngine.UI;

public class CalendarEntry : MonoBehaviour
{
    [SerializeField] Image _HostFlag;
    [SerializeField] Image _GuestFlag;
    [SerializeField] TMPro.TextMeshProUGUI _ResultText;

    public void SetData(int hostID, int guestID, string resultString)
    {
        _HostFlag.sprite = Database.Instance.CountryMaster.GetFlagByName(Database.clubDB[hostID].CountryName);
        _GuestFlag.sprite = Database.Instance.CountryMaster.GetFlagByName(Database.clubDB[guestID].CountryName);
        _ResultText.text = $"{Database.clubDB[hostID].Name} {resultString} {Database.clubDB[guestID].Name}";
    }
}
