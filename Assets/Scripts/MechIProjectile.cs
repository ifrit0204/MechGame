using UnityEngine;

public interface IProjectile
{
    // void Initialize(DamageData damage, float speed, Transform target);
    void Initialize(DamageData damage, float speed, Transform target, TeamType shooterTeam);
}