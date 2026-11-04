using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MechShopConfirmDialogUI : MonoBehaviour
{
    // [SerializeField] private GameObject gameObject;

    [SerializeField] private Text messageText;

    [SerializeField] private Button yesButton;

    [SerializeField] private Button noButton;

    private Action onConfirm;
    public System.Action OnClosed;

    private void Start()
    {
        yesButton.onClick.AddListener(OnYes);

        noButton.onClick.AddListener(Hide);

        Hide();
    }

    public void Show(string message, Action confirmAction)
    {
        gameObject.SetActive(true);

        messageText.text = message;

        onConfirm = confirmAction;

        EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
    }

    private void OnYes()
    {
        onConfirm?.Invoke();
    }

    // private void OnYes()
    // {
    //     onConfirm?.Invoke();

    //     Hide();
    // }

    public void Hide()
    {
        gameObject.SetActive(false);

        onConfirm = null;

        EventSystem.current.SetSelectedGameObject(null);

        OnClosed?.Invoke();
    }
}