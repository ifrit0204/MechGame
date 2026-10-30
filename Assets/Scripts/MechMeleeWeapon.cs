using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechMeleeWeapon : MonoBehaviour
{
    [SerializeField] private MechHitbox hitbox;
    private MechWeaponPartData currentWeapon;
    public void SetWeapon(MechWeaponPartData weapon)
    {
        currentWeapon = weapon;
        hitbox.damageData = weapon.damageData;
    }
    public void BeginAttack()
    {
        Debug.Log("Begin Attack");
        hitbox.Activate();
    }
    public void EndAttack()
    {
        hitbox.Deactivate();
    }
}
