using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechEnemyMovement : MonoBehaviour
{
    public float moveSpeed, jumpForce;
    [SerializeField] private MechEnemyStatsData statsData;
    private MechEnemyControl control;
    private MechStatsControl statsControl;

    public Rigidbody rb;
    public bool isGrounded;

    public int strafeDirection = 1;
    public int orbitDirection = 1;

    public float boostCooldown = 1.5f;
    private float nextBoostTime;
    private float nextJumpTime;

    private float nextApproachBoostTime;
    [SerializeField] private float approachBoostInterval = 0.8f;

    [Header("Evasive Approach (回避しながら接近)")]
    // Profile が指定されている敵は、生成時にその内容で上書きされる
    [SerializeField] private MechEnemyApproachSettings approach = new MechEnemyApproachSettings();

    // 敵から見て、右へ膨らむ(+1)か、左へ膨らむ(-1)か
    private int approachSide = 1;
    private float nextSideChangeTime;

    private void Awake()
    {
        statsControl = GetComponent<MechStatsControl>();
        rb = GetComponent<Rigidbody>();
        control = GetComponent<MechEnemyControl>();

        // 最初の側はランダム(複数の敵が同じ側へ揃わないように)
        approachSide = Random.value > 0.5f ? 1 : -1;
    }

    private void Start()
    {
        bool hasPartsSpeed = control != null && control.Stats != null && control.Stats.moveSpeed > 0f;

        if (hasPartsSpeed)
        {
            moveSpeed = control.moveSpeed;
        }
        else if (statsData != null)
        {
            moveSpeed = statsData.moveSpeed;
        }
        else if (statsControl != null && statsControl.CurrentStats != null)
        {
            moveSpeed = statsControl.CurrentStats.moveSpeed;
        }
        else
        {
            Debug.LogError("敵の移動速度が決まりません(StatsData も CurrentStats もありません)", this);
            return;
        }

        if (!hasPartsSpeed && control != null && moveSpeed > 0f)
            control.moveSpeed = moveSpeed;

        StartCoroutine(ChangeStrafe());
        StartCoroutine(ChangeOrbit());
    }

    // 接近の機動の設定を差し替える(MechEnemyAI.ApplyProfile から呼ばれる)
    public void ApplyApproachSettings(MechEnemyApproachSettings settings)
    {
        if (settings == null)
            return;

        approach = settings.Clone();
    }

    // プレイヤーへまっすぐ向かう(従来どおり)
    public void MoveTowards(Transform target)
    {
        if(target == null)
        {
            Debug.LogError("Target Null");
            return;
        }

        if(control == null)
        {
            Debug.LogError("Control Null");
            return;
        }

        Vector3 dir = target.position - transform.position;

        control.Move(dir);
    }

    // プレイヤーへ接近する。
    // 直線ではなく、プレイヤーの向きから外れる側へ膨らみながら近づく。
    public void ApproachTarget(Transform target)
    {
        if(target == null)
        {
            Debug.LogError("Target Null");
            return;
        }

        if(control == null)
        {
            Debug.LogError("Control Null");
            return;
        }

        control.Move(GetApproachDirection(target));
    }

    // 接近するときの進行方向。
    // 遠いほど大きく横へ膨らみ、近づくと直線になる(近接攻撃を当てるため)。
    public Vector3 GetApproachDirection(Transform target)
    {
        Vector3 toTarget = target.position - transform.position;

        float distance = toTarget.magnitude;

        if (distance < 0.01f)
            return Vector3.zero;

        Vector3 straight = toTarget / distance;

        if (approach == null || !approach.evasive)
            return straight;

        // 近いほど直線、遠いほど最大の強さ
        float t = Mathf.InverseLerp(approach.straightDistance, approach.weaveDistance, distance);

        float lateralRate = approach.lateralRate * t;

        if (lateralRate <= 0.001f)
            return straight;

        // 敵から見て右方向(プレイヤーのほうを向いているときの右)
        Vector3 flat = new Vector3(straight.x, 0f, straight.z);

        Vector3 right = flat.sqrMagnitude > 0.0001f
            ? Vector3.Cross(Vector3.up, flat.normalized)
            : transform.right;

        UpdateApproachSide(target, right);

        return (straight + right * (approachSide * lateralRate)).normalized;
    }

    // 左右どちらへ膨らむかを決める。
    // 「プレイヤーの正面からの横ずれ」が大きくなる側 = プレイヤーの向きから外れる側 を選ぶ。
    // 決めたら sideHoldTime の間は同じ側を続ける(ジグザグを防ぐ)。
    private void UpdateApproachSide(Transform target, Vector3 right)
    {
        if (Time.time < nextSideChangeTime)
            return;

        nextSideChangeTime = Time.time + Mathf.Max(0.1f, approach.sideHoldTime);

        // プレイヤーから見て、敵が右にいるか左にいるか(+: 右 / -: 左)
        Vector3 playerToEnemy = transform.position - target.position;
        playerToEnemy.y = 0f;

        float offset = Vector3.Dot(target.right, playerToEnemy);

        // ほぼ正面にいて左右の差がないときは、今の側を続ける
        if (Mathf.Abs(offset) < 1f)
            return;

        // 敵から見て右へ動いたとき、プレイヤーの正面からの横ずれがどう変わるか
        float change = Vector3.Dot(target.right, right);

        // 横ずれが大きくなる側を選ぶ
        approachSide = (change * Mathf.Sign(offset) >= 0f) ? 1 : -1;
    }

    public void MoveAway(Transform target)
    {
        if(target == null)
            return;

        Vector3 dir = transform.position - target.position;

        control.Move(dir);
    }

    public void MovePosition(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        control.Move(dir);
    }

    public void OrbitTarget(Transform target)
    {
        Vector3 toTarget =
            target.position - transform.position;

        toTarget.y = 0;

        Vector3 orbitDir =
            Vector3.Cross(Vector3.up, toTarget).normalized
            * orbitDirection;

        control.Move(orbitDir);

        LookAtTarget(target);
    }

    public void SideBoost(Transform target)
    {
        if(Time.time < nextBoostTime)
            return;

        nextBoostTime = Time.time + boostCooldown;

        Vector3 toTarget = target.position - transform.position;

        Vector3 right = Vector3.Cross(Vector3.up, toTarget).normalized;

        int dir = Random.value > 0.5f ? 1 : -1;

        control.Boost(right * dir);
    }

    public void Jump()
    {
        if (!isGrounded)
            return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    public void JumpOverTarget(Transform target)
    {
        if (target == null)
            return;

        Vector3 dir = target.position - transform.position;

        control.JumpBoost(dir);
    }

    public void GainAltitude(Transform target)
    {
        if(Time.time < nextJumpTime)
            return;

        nextJumpTime = Time.time + 3f;

        Vector3 toTarget =
            (target.position - transform.position).normalized;

        Vector3 boostDir =
            Vector3.up * 2f + toTarget;

        control.Boost(boostDir);
    }

    public void Strafe(Transform target)
    {
        if(target == null)
            return;

        Vector3 lookDir =
            target.position - transform.position;

        lookDir.y = 0;

        LookAtTarget(target);

        Vector3 moveDir =
            transform.right * strafeDirection;

        if(Physics.Raycast(transform.position, moveDir, 3f))
        {
            strafeDirection *= -1;

            moveDir =
                transform.right * strafeDirection;
        }

        control.Move(moveDir);
    }

    public void MoveBehindTarget(Transform target)
    {
        Vector3 behindPos =
            target.position
            - target.forward * 20f;

        Vector3 dir =
            (behindPos - transform.position);

        control.Move(dir);

        LookAtTarget(target);
    }

    public void LookAtTarget(Transform target)
    {
        if(target == null)
            return;

        Vector3 dir = target.position - transform.position;

        dir.y = 0;

        if(dir.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        float turnSpeed =
            control.isGrounded ? 5f : 2f;

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * turnSpeed);
    }

    // ブーストで接近する。方向は ApproachTarget と同じ(斜めに膨らむ)。
    // 実際にブーストしたら true。間隔が空くまでは何もしない。
    public bool BoostTowards(Transform target)
    {
        if (target == null || control == null || Time.time < nextApproachBoostTime)
            return false;

        nextApproachBoostTime = Time.time + approachBoostInterval;

        control.Boost(GetApproachDirection(target));

        return true;
    }

    public void Stop()
    {
        if (control != null) control.Move(Vector3.zero);
    }

    System.Collections.IEnumerator ChangeStrafe()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(2f, 4f));

            strafeDirection *= -1;
        }
    }
    IEnumerator ChangeOrbit()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(2f,5f));

            orbitDirection = Random.value > 0.5f ? 1 : -1;
        }
    }
}
