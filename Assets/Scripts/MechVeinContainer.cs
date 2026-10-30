using System.Collections;
using UnityEngine;

public class MechVeinContainer : MonoBehaviour, IDamageable
{
    public float maxHP;
    public int rewardBonus;

    public GameObject explosionPrefab;
    public ParticleSystem veinLeakParticle;

    private float currentHP;
    private bool isDestroyed;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(DamageData damageData)
    {
        if (isDestroyed)
            return;

        currentHP -= damageData.Damage;

        if (currentHP <= 0)
        {
            DestroyContainer();
        }
    }

    private void DestroyContainer()
    {
        if (isDestroyed)
            return;

        isDestroyed = true;

        gameObject.tag = "Untagged";
        GetComponent<Collider>().enabled = false;

        StartCoroutine(DestroyRoutine());
    }

    IEnumerator DestroyRoutine()
    {
        MechGameManager.Instance.CurrentResult.veinDestroyedCount++;
        
        if (veinLeakParticle != null)
        {
            veinLeakParticle.Stop();
        }

        GetComponent<Collider>().enabled = false;

        foreach (var r in GetComponentsInChildren<MeshRenderer>())
        {
            r.enabled = false;
        }

        GameObject effect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(effect, 2f);

        MechGameManager.Instance.AddBonusCredits(rewardBonus);

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }
}