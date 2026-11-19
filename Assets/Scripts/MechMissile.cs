// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MechMissile : MonoBehaviour, IProjectile
{
    private MechEnemyBlackboard blackboard;

    [Header("Damage")]
    public TeamType team;
    public DamageData damageData;
    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffectPrefab;        // 通常の着弾(壁・地面など)
    [SerializeField] private GameObject damageEffectPrefab;     // ダメージを与えたとき(任意)
    
    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 3f;
    private bool hasHit;

    [Header("Flight")]
    public float moveSpeed = 50f;
    public float turnSpeed = 180f;
    public float boostTime = 0.5f;
    public float selfDestroyTime = 10f;

    [Header("Homing")]
    public float aimHeight = 2f;
    private Rigidbody rb;
    private Transform target;
    private float timer;
    
    [Header("Acceleration (加速)")]
    // 発射直後の速度(最高速度に対する割合)
    [Range(0f, 1f)] public float startSpeedRate = 0.15f;
    // 加速を始めるまでの時間(秒)
    public float accelDelay = 0.4f;
    // 加速しきるまでの時間(秒)
    public float accelTime = 1.2f;
    // 大きいほど、後半に急激に加速する(1で一定の加速)
    public float accelPower = 3f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;

        blackboard = GetComponent<MechEnemyBlackboard>();
    }

    private void Start()
    {
        Destroy(gameObject, selfDestroyTime);
    }
    public void Initialize(DamageData damage, float speed, Transform target, TeamType shooterTeam)
    {
        damageData = damage;

        moveSpeed = speed;

        this.target = target;

        team = shooterTeam;
    }
    private float GetCurrentSpeed()
    {
        float t = Mathf.Clamp01((timer - accelDelay) / Mathf.Max(0.01f, accelTime));

        float rate = Mathf.Lerp(startSpeedRate, 1f, Mathf.Pow(t, accelPower));

        return moveSpeed * rate;
    }
    
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        float speed = GetCurrentSpeed();

        if (timer < boostTime)
        {
            rb.velocity = transform.forward * speed;
            return;
        }

        if (target == null)
        {
            rb.velocity = transform.forward * speed;
            return;
        }

        Vector3 targetPos = target.position;

        Vector3 direction = (targetPos - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        Quaternion nextRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(nextRotation);

        rb.velocity = nextRotation * Vector3.forward * speed;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (hasHit)
            return;

        TeamMember member = col.GetComponentInParent<TeamMember>();

        if (member != null && member.team == team)
            return;

        hasHit = true;

        Vector3 hitPoint = col.ClosestPoint(transform.position);
        Vector3 hitNormal = -transform.forward;

        IDamageable damageable = col.GetComponentInParent<IDamageable>();

        if (explosionRadius > 0f)
        {
            MechExplosion.Explode(hitPoint, explosionRadius, damageData, team);
        }
        else if (damageable != null)
        {
            damageable.TakeDamage(damageData);
        }

        GameObject prefab = (damageable != null && damageEffectPrefab != null)
            ? damageEffectPrefab
            : hitEffectPrefab;

        MechHitEffect.Spawn(prefab, hitPoint, hitNormal);

        Destroy(gameObject);
    }
}