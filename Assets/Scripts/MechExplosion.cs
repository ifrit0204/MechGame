using System.Collections.Generic;
using UnityEngine;

public static class MechExplosion
{
    private static readonly HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    // 半径内の IDamageable にダメージを与える(同じ相手には1回だけ)
    public static void Explode(Vector3 position, float radius, DamageData damageData, TeamType shooterTeam)
    {
        hitTargets.Clear();

        Collider[] cols = Physics.OverlapSphere(position, radius);

        foreach (Collider col in cols)
        {
            TeamMember member = col.GetComponentInParent<TeamMember>();

            if (member != null && member.team == shooterTeam)
                continue;

            IDamageable damageable = col.GetComponentInParent<IDamageable>();

            if (damageable == null)
                continue;

            GameObject key = ((Component)damageable).gameObject;

            if (!hitTargets.Add(key))
                continue;

            damageable.TakeDamage(damageData);
        }
    }
}