using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MechMissionSelectUI : MonoBehaviour
{
    [SerializeField] private MechMissionData[] missions;

    [SerializeField] private Transform content;

    [SerializeField] private GameObject buttonPrefab;

    [SerializeField] private Text missionNameText;

    [SerializeField] private Text descriptionText;

    [SerializeField] private Text rewardText;

    [SerializeField] public Button startButton;

    private MechMissionData selectedMission;

    // public MechMissionData CurrentMission;

    private List<Button> missionButtons = new List<Button>();

    private void Start()
    {
        startButton.onClick.AddListener(LaunchMission);
    }

    // private void LaunchMission()
    // {
    //     if(selectedMission == null)
    //         return;

    //     SceneManager.LoadScene(selectedMission.sceneName);
    //     MechGameManager.Instance.CurrentMission = selectedMission;
    // }
    private void LaunchMission()
    {
        if(selectedMission == null)
            return;

        MechGameManager.Instance.StartMissionResult();

        MechGameManager.Instance.CurrentMission = selectedMission;

        SceneManager.LoadScene(selectedMission.sceneName);
    }

    private void SelectMission(MechMissionData mission)
    {
        selectedMission = mission;

        missionNameText.text = mission.missionName;

        descriptionText.text = mission.description;

        rewardText.text = mission.reward.ToString();
    }
    private void OnEnable()
    {
        Refresh();

        SelectFirstMission();
    }
    

    private void Refresh()
    {
        Clear();

        missionButtons.Clear();
        
        foreach(var mission in missions)
        {
            CreateButton(mission);
        }
    }

    // private void CreateButton(MechMissionData mission)
    // {
    //     GameObject obj = Instantiate(buttonPrefab, content);

    //     Button button = obj.GetComponent<Button>();

    //     Text text = obj.GetComponentInChildren<Text>();

    //     text.text = mission.missionName;

    //     button.onClick.AddListener(() => StartMission(mission));
    // }
    private void CreateButton(MechMissionData mission)
    {
        GameObject obj = Instantiate(buttonPrefab, content);

        MechMissionButtonUI ui = obj.GetComponent<MechMissionButtonUI>();

        ui.SetMission(mission);

        Button button = obj.GetComponent<Button>();

        missionButtons.Add(button);

        button.onClick.AddListener(() => SelectMission(mission));
    }

    public void SelectFirstMission()
    {
        if(missions.Length == 0)
            return;

        SelectMission(missions[0]);

        if(missionButtons.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(missionButtons[0].gameObject);
        }
    }

    private void StartMission(MechMissionData mission)
    {
        SceneManager.LoadScene(mission.sceneName);
    }

    private void Clear()
    {
        for(int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
}
