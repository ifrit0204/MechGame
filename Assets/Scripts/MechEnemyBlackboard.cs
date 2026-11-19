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

    public float HeightDifference;

    public Vector3 TargetVelocity;

    public float Aggression;

    public bool IsPlayerBoosting;
    
    public bool IsPlayerMeleeAttacking;

    public float BoostEnergy = 100f;

    public float MaxBoostEnergy = 100f;

    public float Pressure;

    public float PlayerLookAngle;     // プレイヤーの正面と敵方向の角度

    public bool IsInPlayerSight;      // プレイヤーの向き(ロック角度)内か

    public bool IsInPlayerRange;      // プレイヤーのロック射程内か

    public bool IsCapturedByPlayer;   // 向き・射程の両方に入っている = 捕捉されている
}