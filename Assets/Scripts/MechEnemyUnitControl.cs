using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechEnemyUnitControl : MonoBehaviour, IDamageable
{
    public float maxHP;
    public float currentHP;
    public float armorPoint;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject veinPickupPrefab;
    [SerializeField] private float veinValue;
    [SerializeField] private MechEnemyStatsData statsData;
    private bool isDead;
    private MechEnemyBlackboard blackboard;
    public TeamType team = TeamType.Enemy;

    private MechStatsControl statsControl;

    // private MechEquipment equipment;
    // private MechTotalStats stats;

    void Awake()
    {
        blackboard = GetComponent<MechEnemyBlackboard>();
        // blackboard = GetComponent<MechEnemyBlackboard>();

        statsControl = GetComponent<MechStatsControl>();
        // equipment = GetComponent<MechEquipment>();
    }

    void Start()
    {
        isDead = false;

        if (statsData != null)
        {
            maxHP = statsData.maxHP;
        }
        else
        {
            if(statsControl == null)
            {
                Debug.LogError("StatsControl Null");
                return;
            }

            if(statsControl.CurrentStats == null)
            {
                Debug.LogError("CurrentStats Null");
                return;
            }

            maxHP = statsControl.CurrentStats.armor;
        }

        currentHP = maxHP;

        blackboard.CurrentHP = currentHP;
    }
    public void TakeDamage(DamageData damageData)
    {
        Debug.Log("TakeDamage");
        Debug.Log("Before " + currentHP);
        blackboard.CurrentHP = currentHP;
        switch (damageData.Type)
        {
            case DamageType.Physical:
            currentHP -= damageData.Damage;
            break;

            case DamageType.Energy:
            currentHP -= damageData.Damage * 0.8f;
            break;

            case DamageType.Explosion:
            currentHP -= damageData.Damage * 1.2f;
            break;
        }
        blackboard.CurrentHP = currentHP;
        Debug.Log("After " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        MechGameManager.Instance.CurrentResult.enemyKillCount++;
        // MechGameManager.Instance.EnemyDestroyed();


        if (isDead)
        return;

        gameObject.tag = "Untagged";
        GetComponent<Collider>().enabled = false;

        isDead = true;
        blackboard.IsDead = true;

        // Instantiate(veinPickupPrefab, transform.position, Quaternion.identity);
        GameObject veinObj = Instantiate(veinPickupPrefab, transform.position, Quaternion.identity);

        MechVeinPickup pickup = veinObj.GetComponent<MechVeinPickup>();

        pickup.healRate = 0.05f * veinValue;

        pickup.energyRate = 0.1f * veinValue;

        StartCoroutine(DeathRoutine());
    }
    IEnumerator DeathRoutine()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<MechEnemyAI>().enabled = false;

        foreach(var r in GetComponentsInChildren<MeshRenderer>())
        {
            r.enabled = false;
        }

        GameObject effect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(effect, 2f);

        yield return new WaitForSeconds(2f);

        Debug.Log("DeathRoutine End");

        // MechGameManager.Instance.EnemyDestroyed();
        MechGameManager.Instance.EnemyDestroyed();

        Destroy(gameObject);
    }
}
