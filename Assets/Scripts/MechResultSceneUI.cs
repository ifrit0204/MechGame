using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MechResultSceneUI : MonoBehaviour
{
    [SerializeField] private Text titleText;

    [SerializeField] private Text killText;

    [SerializeField] private Text damageText;

    [SerializeField] private Text veinText;

    [SerializeField] private Text timeText;

    [SerializeField] private Text rewardText;

    [SerializeField] private Text bonusText;

    [SerializeField] private Text totalText;

    [SerializeField] private Text rankText;

    [SerializeField] private Text arenaText;

    [SerializeField] private float voiceStartDelay = 0.5f;

    void Start()
    {
        var result = MechGameManager.Instance.CurrentResult;

        titleText.text = result.title;

        killText.text = result.enemyKillCount.ToString();

        damageText.text = result.damageTaken.ToString("0");

        veinText.text = result.veinDestroyedCount.ToString();

        timeText.text = FormatTime(result.clearTime);

        rewardText.text = result.missionReward + " C";

        bonusText.text = result.bonusReward + " C";

        totalText.text = result.totalReward + " C";

        rankText.text = result.rank;

        arenaText.text = "#" + result.arenaRank;

        PlayResultVoice(result);
    }

    private void PlayResultVoice(MechResultData result)
    {
        // アリーナのときはミッションのボイスを流さない(後述)
        if (result.isArena)
            return;

        MechMissionData mission = MechGameManager.Instance.CurrentMission;

        if (mission == null)
            return;

        MechGameManager.Instance.PlayVoiceLine(
            mission.resultVoice,
            mission.resultSubtitle,
            voiceStartDelay);
    }

    private string FormatTime(float time)
    {
        int min = Mathf.FloorToInt(time / 60);

        int sec = Mathf.FloorToInt(time % 60);

        return $"{min:00}:{sec:00}";
    }
    public void ReturnGarage()
    {
        MechGameManager.Instance.StopBriefing();

        SceneManager.LoadScene("GarageScene");
    }
}
