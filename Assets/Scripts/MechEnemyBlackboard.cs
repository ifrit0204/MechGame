using UnityEngine;

public class MechEnemyBlackboard : MonoBehaviour
{
    // 現在のターゲット
    public Transform Target;

    // 最後に確認した位置
    public Vector3 LastKnownPosition;

    // ターゲットが見えているか
    public bool CanSeeTarget;

    // ターゲットまでの距離
    public float TargetDistance;

    // 攻撃可能か
    public bool InAttackRange;

    // 現在HP
    public float CurrentHP;

    // 生存フラグ
    public bool IsDead;

    // 被弾したか
    public bool WasDamaged;

    // 被弾方向
    public Vector3 DamageDirection;

    // 最後に撃たれた時間
    public float LastDamageTime;

    // 現在の行動目標地点
    public Vector3 MoveDestination;

    // 現在の遮蔽物
    public Transform CoverPoint;
}