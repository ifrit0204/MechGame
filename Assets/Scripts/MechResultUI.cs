using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MechResultUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [SerializeField] private Text titleText;

    [SerializeField] private Text rewardText;

    [SerializeField] private Button closeButton;

    private void Start()
    {
        panel.SetActive(false);

        closeButton.onClick.AddListener(Close);

        EventSystem.current.SetSelectedGameObject(closeButton.gameObject);

        ShowResult();
    }

    private void ShowResult()
    {
        if(!MechGameManager.Instance.HasResult)
            return;

        panel.SetActive(true);

        titleText.text = "MISSION COMPLETE\n" + MechGameManager.Instance.LastResultTitle;

        rewardText.text = "REWARD : " + MechGameManager.Instance.LastReward + " C";
    }

    private void Close()
    {
        panel.SetActive(false);

        MechGameManager.Instance.HasResult = false;
    }
}