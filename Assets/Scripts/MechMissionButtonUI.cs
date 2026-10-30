using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechMissionButtonUI : MonoBehaviour
{
    [SerializeField] private Text missionNameText;

    [SerializeField] private Text rewardText;

    private MechMissionData mission;

    public void SetMission(MechMissionData data)
    {
        mission = data;

        missionNameText.text = data.missionName;

        rewardText.text = data.reward + " C";
    }
}
