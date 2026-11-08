using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechBullet : MonoBehaviour, IProjectile
{
    public TeamType team;
    public DamageData damageData;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Initialize(DamageData damage, float speed, Transform target, TeamType shooterTeam)
    {
        damageData = damage;
        team = shooterTeam;
        Debug.Log("Bullet Team : " + team);
        rb.velocity = transform.forward * speed * 2f;
    }
    // public void Initialize(DamageData data, float speed)
    // {
    //     damageData = data;
    //     rb.velocity = transform.forward * speed;
    // }

    // private void OnTriggerEnter(Collider col)
    // {
    //     Debug.Log("Hit : " + col.name);
    //     IDamageable damageable = col.GetComponent<IDamageable>();

    //     if (damageable != null)
    //     {
    //         damageable.TakeDamage(damageData);
    //     }

    //     Destroy(gameObject);
    // }
    // private void OnTriggerEnter(Collider col)
    // {
    //     Debug.Log("Hit = " + col.name);

    //     TeamMember member = col.GetComponent<TeamMember>();

    //     if(member != null)
    //     {
    //         if(member.team == team)
    //             return;
    //     }

    //     IDamageable damageable = col.GetComponent<IDamageable>();

    //     Debug.Log("Damageable = " + damageable);

    //     if(damageable != null)
    //     {
    //         damageable.TakeDamage(damageData);
    //     }

    //     Destroy(gameObject);
    // }
    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("Hit : " + col.name);

        TeamMember member = col.GetComponentInParent<TeamMember>();

        Debug.Log("Member : " + member);

        if(member != null)
        {
            if(member.team == team)
                return;
        }

        IDamageable damageable = col.GetComponentInParent<IDamageable>();

        Debug.Log(col.GetComponentInParent<MechControl>());

        if(damageable != null)
        {
            Debug.Log("Damage!");
            damageable.TakeDamage(damageData);
        }

        Destroy(gameObject);
    }
}
