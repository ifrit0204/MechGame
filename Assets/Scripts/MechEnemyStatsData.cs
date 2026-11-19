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

    [Header("AI")]
    // 敵AIの思考設定。未設定なら MechEnemyAI のインスペクターの値を使う。
    // (Loadout 側にも指定がある場合は、Loadout が優先される)
    public MechEnemyAIProfile aiProfile;
}
