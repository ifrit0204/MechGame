using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechMeleeWeapon : MonoBehaviour
{
    [SerializeField] private MechHitbox hitbox;
    [SerializeField] private TrailRenderer trail;
    private MechWeaponPartData currentWeapon;
    // public void SetWeapon(MechWeaponPartData weapon)
    // {
    //     currentWeapon = weapon;
    //     hitbox.damageData = weapon.damageData;
    // }
    public void SetWeapon(MechWeaponPartData weapon, float damageMultiplier = 1f)
    {
        currentWeapon = weapon;

        DamageData data = weapon.damageData;   // structなのでコピーされる
        data.Damage = Mathf.RoundToInt(data.Damage * damageMultiplier);

        hitbox.damageData = data;
    }
    public void BeginAttack()
    {
        Debug.Log("Begin Attack");
        hitbox.Activate();
        if (trail != null)
        {
            trail.Clear();          // 前回の尾を消してから
            trail.emitting = true;
        }
    }
    public void EndAttack()
    {
        hitbox.Deactivate();
        if (trail != null)
            trail.emitting = false;
    }
}
