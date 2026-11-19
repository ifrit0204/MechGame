using UnityEngine;

[CreateAssetMenu(fileName = "ArenaEnemy", menuName = "Mech/ArenaEnemy")]
public class MechArenaEnemyData : ScriptableObject
{
    public string enemyName;
    [TextArea(2, 5)]
    public string description;

    public int reward;

    // public GameObject enemyPrefab;

    public MechEnemyLoadout loadout;

    public int arenaRank;

    [Header("Voice (ボイス・字幕)")]
    // バトル開始時に、この敵について流すボイスと字幕。どちらも空なら、何も流さない。
    public AudioClip startVoice;

    [TextArea(1, 3)]
    public string startSubtitle;

    // この敵を倒したときに流すボイスと字幕。
    // どちらも空なら、MechGameManager の共通のアリーナ用の設定を使う。
    public AudioClip clearVoice;

    [TextArea(1, 3)]
    public string clearSubtitle;

    [Header("Select (対戦相手を選んだときのボイス・字幕)")]
    // アリーナで対戦相手を選んだときに流す、この敵の紹介ボイス
    public AudioClip selectVoice;

    // ボイスと同時に表示する字幕(例: 「オペレーター: 次の相手は〜」)
    [TextArea(1, 3)]
    public string selectSubtitle;
}
