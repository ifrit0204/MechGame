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

    [Header("表示切り替え(ラベルと値の両方を入れる。空でもよい)")]
    // ミッションのときだけ表示する項目(例: ヴェイン破壊数、特別加算額、依頼評価値)
    [SerializeField] private GameObject[] missionOnlyObjects;
    // アリーナのときだけ表示する項目(例: アリーナ順位)
    [SerializeField] private GameObject[] arenaOnlyObjects;

    void Start()
    {
        if (MechGameManager.Instance == null)
        {
            Debug.LogError("MechGameManager がありません");
            return;
        }

        var result = MechGameManager.Instance.CurrentResult;

        SetText(titleText, result.title);

        SetText(killText, result.enemyKillCount.ToString());

        SetText(damageText, result.damageTaken.ToString("0"));

        SetText(veinText, result.veinDestroyedCount.ToString());

        SetText(timeText, FormatTime(result.clearTime));

        SetText(rewardText, result.missionReward + " C");

        SetText(bonusText, result.bonusReward + " C");

        SetText(totalText, result.totalReward + " C");

        // アリーナでは評価ランクを使わないので、空欄ではなく「-」にする
        SetText(rankText, string.IsNullOrEmpty(result.rank) ? "-" : result.rank);

        SetText(arenaText, "#" + result.arenaRank);

        SetActiveAll(missionOnlyObjects, !result.isArena);
        SetActiveAll(arenaOnlyObjects, result.isArena);

        PlayResultVoice(result);
    }

    private static void SetText(Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private static void SetActiveAll(GameObject[] objects, bool active)
    {
        if (objects == null)
            return;

        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }

    private void PlayResultVoice(MechResultData result)
    {
        // アリーナのときはミッションのボイスを流さない
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