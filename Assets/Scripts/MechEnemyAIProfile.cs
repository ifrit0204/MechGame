using UnityEngine;

// 敵AIの「思考の設定」をまとめたアセット。
// MechEnemyStatsData または MechEnemyLoadout の Ai Profile に設定すると、
// MechEnemyAI がその内容を使って行動を選ぶ。
//
// 作成: Project右クリック > Create > Mech > Enemy AI Profile
[CreateAssetMenu(fileName = "EnemyAIProfile", menuName = "Mech/Enemy AI Profile")]
public class MechEnemyAIProfile : ScriptableObject
{
    [Header("Personality (性格)")]
    public AIPersonality personality = AIPersonality.Balanced;

    [Tooltip("大きいほど判断が速い")]
    public float reactionSpeed = 1f;

    [Tooltip("大きいほど、HPが減っても退きにくく、突撃にも踏み切りやすい")]
    public float courage = 1f;

    [Tooltip("偏差射撃の強さ。大きいほど、プレイヤーの移動先を予測して撃つ")]
    public float accuracy = 1f;

    [Tooltip("1つの行動を最低限続ける時間(秒)")]
    public float minActionDuration = 1.5f;

    [Header("Initiative / Assault (主導権と突撃)")]
    [Tooltip("オフにすると、近接武器を持っていても突撃しない")]
    public bool allowAssault = true;

    [Tooltip("隙があったときに仕掛ける確率(0〜1)。-1なら性格の既定値を使う")]
    [Range(-1f, 1f)]
    public float assaultWillingness = -1f;

    [Tooltip("combatDistance × この値 以内なら突撃を始められる")]
    public float assaultStartRate = 1f;

    [Tooltip("突撃の最大継続時間(秒)")]
    public float assaultMaxDuration = 3f;

    [Tooltip("突撃が終わってから、次の突撃までの時間(秒)")]
    public float assaultCooldown = 4f;

    [Tooltip("この時間プレイヤーに捕捉され続け、まだ遠いなら突撃を中止する(秒)")]
    public float abortCaptureTime = 1f;

    [Tooltip("HP割合がこれ未満(courageで補正)なら突撃しない")]
    public float assaultMinHpRate = 0.35f;

    [Tooltip("突撃のブースト1回あたりのエネルギー消費")]
    public float assaultBoostCost = 15f;

    [Tooltip("通常の接近(Approach)のブースト1回あたりのエネルギー消費")]
    public float approachBoostCost = 10f;

    [Tooltip("この速度以上で接近しているとき、近接射程に入ったらその勢いで攻撃する")]
    public float dashAttackSpeed = 15f;

    [Tooltip("主導権がないとき、「攻めの接近」スコアを何割だけ反映するか")]
    [Range(0f, 1f)]
    public float disadvantageApproachRate = 0.15f;

    [Header("Evasive Approach (回避しながら接近)")]
    [Tooltip("接近するとき、プレイヤーの向きから外れる側へ膨らんで近づく設定")]
    public MechEnemyApproachSettings approach = new MechEnemyApproachSettings();

    [Header("Action Weights (行動の好み。1が標準、0でその行動を選ばない)")]
    public float retreatWeight = 1f;
    public float orbitWeight = 1f;
    public float approachWeight = 1f;
    public float altitudeWeight = 1f;
    public float behindWeight = 1f;
    public float sideBoostWeight = 1f;

    [Header("Resource (回復・減衰)")]
    public float boostRegenPerSec = 20f;
    public float pressureDecayPerSec = 30f;
    public float aggressionDecayPerSec = 3f;
}
