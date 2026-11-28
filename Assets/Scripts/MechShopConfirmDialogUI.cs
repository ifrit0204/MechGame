using System;
using System.Collections;
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

        Close(false); // 起動時は閉じるだけ(通知・選択解除はしない)
    }

    public void Show(string message, Action confirmAction)
    {
        gameObject.SetActive(true);

        messageText.text = message;

        onConfirm = confirmAction;

        StopAllCoroutines();
        StartCoroutine(SelectYesNextFrame());
    }

    private IEnumerator SelectYesNextFrame()
    {
        yield return null; // Time.timeScale が 0 でも進む

        EventSystem es = EventSystem.current;

        if (es == null)
            es = FindFirstObjectByType<EventSystem>();

        if (es == null)
        {
            Debug.LogWarning("MechShopConfirmDialogUI: EventSystem が見つかりません", this);
            yield break;
        }

        // 前回の選択が残っていると再選択が無視されるので、一度外す
        es.SetSelectedGameObject(null);
        es.SetSelectedGameObject(yesButton.gameObject);
    }

    private void Close(bool notify)
    {
        gameObject.SetActive(false);

        onConfirm = null;

        if (notify)
        {
            // 閉じるダイアログのボタンを選択したままにしない
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);

            OnClosed?.Invoke();
        }
    }

    public void Hide()
    {
        Close(true);
    }

    private void OnYes()
    {
        onConfirm?.Invoke();
    }
    // private void Close(bool notify)
    // {
    //     gameObject.SetActive(false);

    //     onConfirm = null;

    //     if (notify)
    //     {
    //         if (EventSystem.current != null)
    //             EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
    //         // if (EventSystem.current != null)
    //             // EventSystem.current.SetSelectedGameObject(null);

    //         OnClosed?.Invoke();
    //     }
    // }


    // private void OnYes()
    // {
    //     onConfirm?.Invoke();

    //     Hide();
    // }

    // public void Hide()
    // {
    //     gameObject.SetActive(false);

    //     onConfirm = null;

    //     EventSystem.current.SetSelectedGameObject(null);

    //     OnClosed?.Invoke();
    // }
}