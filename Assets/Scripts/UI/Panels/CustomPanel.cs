using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomPanel : BasePanel
{
    public class CustomPanelData : PanelData
    {
        public string Title;
        public string Description;
        public Action OnConfirm;
        public Action OnCancel;

        public CustomPanelData() {}

        public CustomPanelData(string title, string description, Action onConfirm, Action onCancel = null)
        {
            Title = title;
            Description = description;
            OnConfirm = onConfirm;
            OnCancel = onCancel;
        }
    }

    [SerializeField] TextMeshProUGUI _Title;
    [SerializeField] TextMeshProUGUI _Desc;
    [SerializeField] Button _ConfirmButton;
    [SerializeField] Button _CancelButton;

    protected override void OnShow(PanelData panelData)
    {
        CustomPanelData customData = panelData as CustomPanelData;

        if (customData == null)
            return;

        _Title.text = customData.Title;
        _Desc.text = customData.Description;
        _ConfirmButton.onClick.RemoveAllListeners();
        _CancelButton.onClick.RemoveAllListeners();
        _ConfirmButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            customData.OnConfirm?.Invoke();
        });
        _CancelButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            customData.OnCancel?.Invoke();
        });
    }
}
