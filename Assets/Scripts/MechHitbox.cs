using UnityEngine;

public class MechHitbox : MonoBehaviour
{
    public DamageData damageData;

    private bool activeHitbox;

    public void Activate()
    {
        activeHitbox = true;
        Debug.Log("Hitbox ON");
    }

    public void Deactivate()
    {
        activeHitbox = false;
        Debug.Log("Hitbox OFF");
    }
    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("TRIGGER");
        Debug.Log("Hit : " + col.name);
        if (!activeHitbox)
        return;
        Debug.Log("DAMAGE");

        IDamageable damageable = col.GetComponent<IDamageable>();
        // IDamageable damageable = col.GetComponentInParent<IDamageable>();
        

        Debug.Log("damageable = " + damageable);

        if(damageable != null)
        {
            damageable.TakeDamage(damageData);
        }
    }
}