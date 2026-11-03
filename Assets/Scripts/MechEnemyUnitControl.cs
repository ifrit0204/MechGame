using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechEnemyUnitControl : MonoBehaviour, IDamageable
{
    public float maxHP = 100;
    public float currentHP;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject veinPickupPrefab;
    [SerializeField] private float veinValue;
    private bool isDead;
    private MechEnemyBlackboard blackboard;
    void Start()
    {
        isDead = false;
        // MechGameManager.Instance.enemyCount++;
        currentHP = maxHP;
        blackboard = GetComponent<MechEnemyBlackboard>();
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
