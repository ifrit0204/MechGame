using UnityEngine;

public class MechBullet : MonoBehaviour, IProjectile
{
    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffectPrefab;        // 通常の着弾(壁・地面など)
    [SerializeField] private GameObject damageEffectPrefab;     // ダメージを与えたとき(任意)

    [Header("Explosion (爆発系の武器だけ設定)")]
    [SerializeField] private float explosionRadius = 0f;        // 0 なら直撃した1体だけにダメージ

    public TeamType team;
    public DamageData damageData;

    private Rigidbody rb;
    private bool hasHit;   // 同じフレームに複数のColliderへ当たっても、処理は1回だけにする

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(DamageData damage, float speed, Transform target, TeamType shooterTeam)
    {
        damageData = damage;
        team = shooterTeam;
        rb.velocity = transform.forward * speed * 2f;
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
        Vector3 hitNormal = -transform.forward;   // 飛んできた方向の逆

        IDamageable damageable = col.GetComponentInParent<IDamageable>();

        // --- ダメージ ---
        if (explosionRadius > 0f)
        {
            // 爆発: 半径内を全員対象にする(直撃した相手も含まれる)
            MechExplosion.Explode(hitPoint, explosionRadius, damageData, team);
        }
        else if (damageable != null)
        {
            damageable.TakeDamage(damageData);
        }

        // --- 着弾エフェクト ---
        GameObject prefab = (damageable != null && damageEffectPrefab != null)
            ? damageEffectPrefab
            : hitEffectPrefab;

        MechHitEffect.Spawn(prefab, hitPoint, hitNormal);

        Destroy(gameObject);
    }
}