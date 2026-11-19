using UnityEngine;

public class MechEnemySensor : MonoBehaviour
{
    [SerializeField] private float detectDistance;

    [SerializeField] private float viewAngle;

    [SerializeField] private Transform sensorPoint;

    [SerializeField] private float offsetY;

    [SerializeField] private float offsetZ;
    [SerializeField] private MechEnemyStatsData statsData;

    [Header("Player View (敵がプレイヤーに捕捉されているかの推定)")]
    // プレイヤーのFCS情報が取れないときに使う値
    [SerializeField] private float fallbackPlayerLockRange = 150f;
    [SerializeField] private float fallbackPlayerLockAngle = 35f;

    private MechEnemyBlackboard blackboard;
    private MechStatsControl statsControl;
    private float lockTimer;

    // ターゲット(プレイヤー)側の情報。ターゲットが変わったときだけ取得し直す
    private Transform cachedTarget;
    private Rigidbody targetRb;
    private MechStatsControl targetStats;

    private void Awake()
    {
        blackboard = GetComponent<MechEnemyBlackboard>();
        statsControl = GetComponent<MechStatsControl>();
    }

    private void Update()
    {
        if (blackboard.Target == null)
            return;

        if (blackboard.Target != cachedTarget)
        {
            cachedTarget = blackboard.Target;
            targetRb = cachedTarget.GetComponent<Rigidbody>();
            targetStats = cachedTarget.GetComponent<MechStatsControl>();
        }

        bool visible = CanSeeTarget();

        blackboard.CanSeeTarget = visible;

        blackboard.TargetDistance = Vector3.Distance(transform.position, blackboard.Target.position);

        if (visible)
        {
            blackboard.LastKnownPosition = blackboard.Target.position;
        }

        blackboard.HeightDifference = blackboard.Target.position.y - transform.position.y;

        if (targetRb != null)
        {
            blackboard.TargetVelocity = targetRb.velocity;

            // プレイヤーが敵に向かって近づいている速さ
            Vector3 toEnemy = (transform.position - blackboard.Target.position).normalized;

            float closingSpeed = Vector3.Dot(targetRb.velocity, toEnemy);

            blackboard.IsPlayerBoosting = closingSpeed > 20f;
        }

        UpdatePlayerView();
    }

    // プレイヤーから見て、自分(敵)が「向き」「射程」に入っているか = 捕捉されているか
    private void UpdatePlayerView()
    {
        float lockRange = fallbackPlayerLockRange;
        float lockAngle = fallbackPlayerLockAngle;

        if (targetStats != null && targetStats.CurrentStats != null)
        {
            if (targetStats.CurrentStats.lockRange > 0f)
                lockRange = targetStats.CurrentStats.lockRange;

            if (targetStats.CurrentStats.lockRadius > 0f)
                lockAngle = targetStats.CurrentStats.lockRadius;
        }

        Vector3 toEnemy = transform.position - blackboard.Target.position;

        blackboard.PlayerLookAngle = Vector3.Angle(blackboard.Target.forward, toEnemy);

        blackboard.IsInPlayerSight = blackboard.PlayerLookAngle <= lockAngle;

        blackboard.IsInPlayerRange = toEnemy.magnitude <= lockRange;

        blackboard.IsCapturedByPlayer = blackboard.IsInPlayerSight && blackboard.IsInPlayerRange;
    }

    public bool CanSeeTarget()
    {
        if (blackboard.Target == null)
            return false;

        float lockRange = detectDistance;

        if (statsControl != null && statsControl.CurrentStats != null)
        {
            lockRange = statsControl.CurrentStats.lockRange;
        }

        float lockRadius = viewAngle;

        if (statsControl != null && statsControl.CurrentStats != null)
        {
            lockRadius = statsControl.CurrentStats.lockRadius;
        }

        float lockSpeed = 120f;

        if (statsData != null)
        {
            lockSpeed = statsData.targetLockSpeed;
        }

        if (statsControl != null && statsControl.CurrentStats != null)
        {
            lockSpeed = statsControl.CurrentStats.lockSpeed;
        }

        float distance = Vector3.Distance(transform.position, blackboard.Target.position);

        Vector3 dir = blackboard.Target.position - transform.position;

        float angle = Vector3.Angle(transform.forward, dir);

        if (angle > lockRadius)
        {
            return false;
        }

        if (distance <= lockRange)
        {
            lockTimer += Time.deltaTime * lockSpeed;

            if (lockTimer >= lockSpeed)
            {
                return true;
            }

            return false;
        }

        lockTimer = 0;
        return false;
    }
}