using System.Collections.Generic;
using UnityEngine;

public class MechHitbox : MonoBehaviour
{
    public DamageData damageData;

    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffectPrefab;      // 壁・地面など
    [SerializeField] private GameObject damageEffectPrefab;   // ダメージを与えたとき(未設定なら hitEffectPrefab を使う)

    private bool activeHitbox;
    private TeamMember ownerTeam;

    // 今回の攻撃で既にヒットした相手(1回の攻撃で多段ヒットしないようにする)
    private readonly HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    private void Awake()
    {
        ownerTeam = GetComponentInParent<TeamMember>();
    }

    public void Activate()
    {
        activeHitbox = true;
        hitTargets.Clear();
    }

    public void Deactivate()
    {
        activeHitbox = false;
        hitTargets.Clear();
    }

    private void OnTriggerEnter(Collider col)
    {
        TryHit(col);
    }

    // 攻撃開始時に既に重なっていた相手にも当たるようにする
    private void OnTriggerStay(Collider col)
    {
        TryHit(col);
    }

    private void TryHit(Collider col)
    {
        if (!activeHitbox)
            return;

        // 自分と同じチームには当てない
        TeamMember member = col.GetComponentInParent<TeamMember>();

        if (member != null && ownerTeam != null && member.team == ownerTeam.team)
            return;

        IDamageable damageable = col.GetComponentInParent<IDamageable>();

        // 同じ相手(=同じ IDamageable を持つ object)には1回の攻撃で1度だけ
        GameObject key = damageable != null
            ? ((Component)damageable).gameObject
            : col.gameObject;

        if (!hitTargets.Add(key))
            return;

        if (damageable != null)
            damageable.TakeDamage(damageData);

        SpawnEffect(col, damageable != null);
    }

    private void SpawnEffect(Collider col, bool damaged)
    {
        // 当たった相手の表面で、このヒットボックスに一番近い点
        Vector3 hitPoint = col.ClosestPoint(transform.position);

        // 相手からヒットボックス側へ向かう方向を法線の代わりにする
        Vector3 normal = (transform.position - hitPoint);

        GameObject prefab = (damaged && damageEffectPrefab != null)
            ? damageEffectPrefab
            : hitEffectPrefab;

        MechHitEffect.Spawn(prefab, hitPoint, normal);
    }
}