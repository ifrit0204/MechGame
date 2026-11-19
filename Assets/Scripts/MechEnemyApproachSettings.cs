using UnityEngine;

// 敵がプレイヤーへ接近するときの機動の設定。
// MechEnemyMovement のインスペクターと、MechEnemyAIProfile の両方で使う。
[System.Serializable]
public class MechEnemyApproachSettings
{
    [Tooltip("オフにすると、従来どおりプレイヤーへ直線で接近する")]
    public bool evasive = true;

    [Tooltip("横へ膨らむ強さ。0=直線、1=約45度、大きいほど円を描くように近づく(大きすぎると近づくのが遅くなる)")]
    [Range(0f, 2f)]
    public float lateralRate = 0.8f;

    [Tooltip("この距離より近いと、横へ膨らまず直線で詰める(近接攻撃を当てるため)")]
    public float straightDistance = 30f;

    [Tooltip("この距離より遠いと、最大の強さで横へ膨らむ。間は少しずつ強くなる")]
    public float weaveDistance = 120f;

    [Tooltip("左右どちらへ膨らむかを決め直す間隔(秒)。短いとジグザグになる")]
    public float sideHoldTime = 1.5f;

    public MechEnemyApproachSettings Clone()
    {
        return (MechEnemyApproachSettings)MemberwiseClone();
    }
}
