using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MechWeaponPartData : MechPartData
{
    public WeaponType weaponType;
    
    public float lockDistance;
    public float bulletSpeed;
    public float reloadTime;
    public float spread;
    public int ammo;
    public string muzzleName;

    public string attackTrigger;
    public float attackRange;
    public GameObject projectilePrefab;

    public DamageData damageData;
    // チャージ
    public bool canCharge;
    public float chargeTime;

    public DamageData chargeDamageData;
    // public class RifleData : MechWeaponPartData
    // {
    //     public float bulletSpeed;
    // }

    // public class BladeData : MechWeaponPartData
    // {
    //     public float attackRange;
    // }

    // public class MissileData : MechWeaponPartData
    // {
    //     public float lockDistance;
    // }
}
