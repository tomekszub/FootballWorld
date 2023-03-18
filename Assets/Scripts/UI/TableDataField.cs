using UnityEngine;
using UnityEngine.UI;

public class TableDataField : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI _Text;
    [SerializeField] Image _Image;

    public void SetTextData(string text) => _Text.text = text;

    public void SetImageData(Sprite sprite) => _Image.sprite = sprite;

    public void SetImageFillAmount(float fillAmount)
    {
        _Image.fillAmount = fillAmount;
    }
}
