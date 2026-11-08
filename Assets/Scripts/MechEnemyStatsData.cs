using UnityEngine;

[CreateAssetMenu(menuName = "Mech/Enemy Stats")]
public class MechEnemyStatsData : ScriptableObject
{
    public float maxHP;

    public float moveSpeed;

    public float detectDistance;

    public float combatDistance;

    public float targetLockSpeed;

    public float fireInterval;

    public float damage;
}