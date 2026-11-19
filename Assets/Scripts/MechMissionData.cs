using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Mech/Mission")]
public class MechMissionData : ScriptableObject
{
    public string missionName;
    [TextArea(2, 5)]
    public string description;

    public string sceneName;

    public int reward;

    [Header("Briefing (ブリーフィング)")]
    // ミッションを選んだときに流すブリーフィングの音声
    public AudioClip briefingVoice;

    // ブリーフィングの字幕(例: 「オペレーター: 今回の任務は〜」)
    [TextArea(1, 3)]
    public string briefingSubtitle;

    [Header("Voice (ボイス・字幕)")]
    // ミッション完了時に、リザルトへ移る前に流すボイス。
    // 例: 「目標の撃破を確認。帰投してください」
    public AudioClip completeVoice;

    // ボイスと同時に表示する字幕。話者の名前を付けたいときは、文の頭に入れる(例: 「オペレーター: 〜」)
    // ボイスか字幕のどちらか一方でも設定されていれば、このミッションの設定を使う。
    // 両方空なら MechGameManager の既定の設定を使う(それも空なら、すぐリザルトへ移る)
    [TextArea(1, 3)]
    public string completeSubtitle;

    [Header("Result (リザルト画面のボイス・字幕)")]
    // リザルト画面が表示されたときに流すボイス
    public AudioClip resultVoice;

    [TextArea(1, 3)]
    public string resultSubtitle;
}
