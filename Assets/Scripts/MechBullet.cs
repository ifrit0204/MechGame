using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechBullet : MonoBehaviour, IProjectile
{
    public DamageData damageData;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Initialize(DamageData damage, float speed, Transform target)
    {
        damageData = damage;
        rb.velocity = transform.forward * speed;
    }
    // public void Initialize(DamageData data, float speed)
    // {
    //     damageData = data;
    //     rb.velocity = transform.forward * speed;
    // }

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("Hit : " + col.name);
        IDamageable damageable = col.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damageData);
        }

        Destroy(gameObject);
    }
}
