using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechAttackAction : MonoBehaviour
{
    private PlayerInput mechRightArmInputAction;
    private PlayerInput mechLeftArmInputAction;
    private PlayerInput mechRightShoulderInputAction;
    private PlayerInput mechLeftShoulderInputAction;
    [SerializeField] private MechControl mech;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzleRightArm;
    [SerializeField] private Transform muzzleLeftArm;
    [SerializeField] private Transform muzzleRightShoulder;
    [SerializeField] private Transform muzzleLeftShoulder;
    [SerializeField] private MechHitbox rightHandHitbox;
    [SerializeField] private MechHitbox leftHandHitbox;
    private MechMeleeWeapon currentMeleeWeapon;
    private MechEquipment equipment;
    private MechLockOn lockOn;
    private float rightArmChargeTimer;
    private bool rightArmCharging;
    private float leftArmChargeTimer;
    private bool leftArmCharging;
    private float rightShoulderChargeTimer;
    private bool rightShoulderCharging;
    private float leftShoulderChargeTimer;
    private bool leftShoulderCharging;
    public float attackRange;
    [SerializeField] private float uppercutRange;
    [SerializeField] private float uppercutForce;
    [SerializeField] private int uppercutDamage;
    [SerializeField] private int rightPunchDamage;
    [SerializeField] private int leftPunchDamage;
    
    [SerializeField] private float meleeDashSpeed;
    [SerializeField] private float meleeAttackDistance;
    // public bool isAttack;
    private Animator anim;
    private Rigidbody rb;
    private bool leftArmFiring;
    private bool rightArmFiring;
    private bool leftShoulderFiring;
    private bool rightShoulderFiring;
    private float leftArmReload;
    private float rightArmReload;
    private float leftShoulderReload;
    private float rightShoulderReload;

    void Start()
    {
        equipment = GetComponent<MechEquipment>();
        lockOn = GetComponent<MechLockOn>();
        // currentMeleeWeapon = GetComponent<MechMeleeWeapon>();

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        mechRightArmInputAction = GetComponent<PlayerInput>();
        mechLeftArmInputAction = GetComponent<PlayerInput>();
        mechRightShoulderInputAction = GetComponent<PlayerInput>();
        mechLeftShoulderInputAction = GetComponent<PlayerInput>();

        if (mechRightArmInputAction != null)
        {
            var keyboardMap = mechRightArmInputAction.actions.FindActionMap("Keyboard");
            var xboxMap = mechRightArmInputAction.actions.FindActionMap("XboxControl");
            if (keyboardMap != null) keyboardMap.Enable();
            if (xboxMap != null) xboxMap.Enable();
        }
        if (mechLeftArmInputAction != null)
        {
            var keyboardMap = mechLeftArmInputAction.actions.FindActionMap("Keyboard");
            var xboxMap = mechLeftArmInputAction.actions.FindActionMap("XboxControl");
            if (keyboardMap != null) keyboardMap.Enable();
            if (xboxMap != null) xboxMap.Enable();
        }
        if (mechRightShoulderInputAction != null)
        {
            var keyboardMap = mechRightShoulderInputAction.actions.FindActionMap("Keyboard");
            var xboxMap = mechRightShoulderInputAction.actions.FindActionMap("XboxControl");
            if (keyboardMap != null) keyboardMap.Enable();
            if (xboxMap != null) xboxMap.Enable();
        }
        if (mechLeftShoulderInputAction != null)
        {
            var keyboardMap = mechLeftShoulderInputAction.actions.FindActionMap("Keyboard");
            var xboxMap = mechLeftShoulderInputAction.actions.FindActionMap("XboxControl");
            if (keyboardMap != null) keyboardMap.Enable();
            if (xboxMap != null) xboxMap.Enable();
        }
    }
    // public void OnRightArmAttack(InputAction.CallbackContext context)
    // {
    //     if (context.started)
    //     {
    //         rightArmCharging = true;
    //         rightArmChargeTimer = 0;
    //     }

    //     if (context.canceled)
    //     {
    //         ReleaseRightArmAttack();
    //     }
    //     // if (context.performed)
    //     // FireWeapon(WeaponSlot.RightArm);
    // }
    public void OnRightArmAttack(InputAction.CallbackContext context)
    {
        MechWeaponPartData weapon = equipment.rightWeapon;

        if (weapon != null && !weapon.canCharge)
        {
            if (context.started)
            {
                rightArmFiring = true;
            }

            if (context.canceled)
            {
                rightArmFiring = false;
            }

            return;
        }

        // チャージ武器
        if (context.started)
        {
            rightArmCharging = true;
            rightArmChargeTimer = 0;
        }

        if (context.canceled)
        {
            ReleaseRightArmAttack();
        }
    }

    public void OnLeftArmAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            FireWeapon(WeaponSlot.LeftArm);
        }
        if(context.started)
        {
            leftArmFiring = true;
        }

        if(context.canceled)
        {
            leftArmFiring = false;
        }
    }

    public void OnRightShoulderAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            FireWeapon(WeaponSlot.RightShoulder);
        }
        if(context.started)
        {
            rightShoulderFiring = true;
        }

        if(context.canceled)
        {
            rightShoulderFiring = false;
        }
    }

    public void OnLeftShoulderAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            FireWeapon(WeaponSlot.LeftShoulder);
        }
        if(context.started)
        {
            leftShoulderFiring = true;
        }

        if(context.canceled)
        {
            leftShoulderFiring = false;
        }
    }

    private MechWeaponPartData GetWeapon(WeaponSlot slot)
    {
        switch(slot)
        {
            case WeaponSlot.RightArm:
            return equipment.rightWeapon;

            case WeaponSlot.LeftArm:
            return equipment.leftWeapon;

            case WeaponSlot.RightShoulder:
            return equipment.rightShoulder;

            case WeaponSlot.LeftShoulder:
            return equipment.leftShoulder;
        }

    return null;
    }

    private Transform GetMuzzle(WeaponSlot slot)
    {
        switch(slot)
        {
            case WeaponSlot.RightArm:
            return muzzleRightArm;

            case WeaponSlot.LeftArm:
            return muzzleLeftArm;

            case WeaponSlot.RightShoulder:
            return muzzleRightShoulder;

            case WeaponSlot.LeftShoulder:
            return muzzleLeftShoulder;
        }

        return null;
    }

    private void FireWeapon(WeaponSlot slot)
    {
        MechWeaponPartData weapon = GetWeapon(slot);

        if(weapon == null)
        {
            Punch(slot);
            return;
        }

        switch(weapon.weaponType)
        {
            case WeaponType.Shooting:
            ShootWeapon(weapon, slot);
            break;

            case WeaponType.Melee:
            MeleeWeaponAttack(weapon);
            break;

            case WeaponType.Shield:
            ShieldAttack(weapon);
            break;

            // case WeaponType.Special:
            // SpecialAttack(weapon);
            // break;
        }
    }
    private void ShootWeapon(MechWeaponPartData weapon, WeaponSlot slot)
    {
        Transform muzzle = GetMuzzle(slot);

        Quaternion shootRotation;

        if(lockOn != null && lockOn.currentTarget != null)
        {
            Vector3 dir = (lockOn.currentTarget.position - muzzle.position).normalized;

            shootRotation = Quaternion.LookRotation(dir);
        }
        else
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            Vector3 aimPoint;

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f))
            {
                aimPoint = hit.point;
            }
            else
            {
                aimPoint = ray.origin + ray.direction * 1000f;
            }

            Vector3 dir = (aimPoint - muzzle.position).normalized;

            shootRotation = Quaternion.LookRotation(dir);
        }

        // GameObject bulletObj = Instantiate(bulletPrefab, muzzle.position, shootRotation);
        // GameObject bulletObj = Instantiate(weapon.projectilePrefab, muzzle.position, shootRotation);
        GameObject bulletObj = Instantiate(weapon.projectilePrefab, muzzle.position, shootRotation);

        IProjectile projectile = bulletObj.GetComponent<IProjectile>();

        if(projectile != null)
        {
            projectile.Initialize(weapon.damageData, weapon.bulletSpeed, lockOn.currentTarget, TeamType.Player);
        }

        // MechBullet bullet = bulletObj.GetComponent<MechBullet>();

        // bullet.Initialize(weapon.damageData, weapon.bulletSpeed);
    }
    // private void ShootWeapon(MechWeaponPartData weapon, WeaponSlot slot)
    // {
    //     Transform muzzle = GetMuzzle(slot);

    //     // GameObject bulletObj = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
    //     Quaternion shootRotation;

    //     if(lockOn != null && lockOn.currentTarget != null)
    //     {
    //         Vector3 dir = (lockOn.currentTarget.position - muzzle.position).normalized;

    //         shootRotation = Quaternion.LookRotation(dir);
    //     }
    //     else
    //     {
    //         Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

    //         Vector3 aimPoint;

    //         RaycastHit hit;
    //         // shootRotation = Quaternion.LookRotation(Camera.main.transform.forward);
    //     }
    //     GameObject bulletObj = Instantiate(bulletPrefab, muzzle.position, shootRotation);

    //     MechBullet bullet = bulletObj.GetComponent<MechBullet>();

    //     bullet.Initialize(weapon.damageData, weapon.bulletSpeed);
    // }
    private void MeleeAttack(MechWeaponPartData weapon)
    {
        // Collider[] hits = Physics.OverlapSphere(transform.position, weapon.attackRange);

        // foreach(Collider hit in hits)
        // {
        //     IDamageable damageable = hit.GetComponent<IDamageable>();

        //     if(damageable != null)
        //     {
        //         damageable.TakeDamage(weapon.damageData);
        //     }
        // }
    }
    private void ShieldAttack(MechWeaponPartData weapon)
    {
        // Collider[] hits = Physics.OverlapSphere(transform.position, weapon.attackRange);

        // foreach(Collider hit in hits)
        // {
        //     IDamageable damageable = hit.GetComponent<IDamageable>();

        //     if(damageable != null)
        //     {
        //         damageable.TakeDamage(weapon.damageData);
        //     }
        // }
    }
    private void Punch(WeaponSlot slot)
    {
        switch(slot)
        {
            case WeaponSlot.RightArm:
            anim.SetTrigger("PunchRight");
            break;

            case WeaponSlot.LeftArm:
            anim.SetTrigger("PunchLeft");
            break;
        }
    }
    private void Update()
    {
        ChargeWeapon();

        HandleAutoFire();
    }
    private void ChargeWeapon()
    {
        if(rightArmCharging)
        {
            rightArmChargeTimer += Time.deltaTime;
        }

        if(leftArmCharging)
        {
            leftArmChargeTimer += Time.deltaTime;
        }

        if(rightShoulderCharging)
        {
            rightShoulderChargeTimer += Time.deltaTime;
        }

        if(leftShoulderCharging)
        {
            leftShoulderChargeTimer += Time.deltaTime;
        }
    }
    private void HandleAutoFire()
    {
        if (rightArmReload > 0)
            rightArmReload -= Time.deltaTime;

        if(leftArmReload > 0)
            leftArmReload -= Time.deltaTime;

        if(rightShoulderReload > 0)
            rightShoulderReload -= Time.deltaTime;

        if(leftShoulderReload > 0)
            leftShoulderReload -= Time.deltaTime;

        // if(rightArmFiring)
        // {
        //     MechWeaponPartData weapon = equipment.rightWeapon;

        //     if(weapon != null && rightArmReload <= 0)
        //     {
        //         FireWeapon(WeaponSlot.RightArm);

        //         rightArmReload = weapon.reloadTime;
        //     }
        // }
        if(rightArmFiring)
        {
            MechWeaponPartData weapon = equipment.rightWeapon;

            if(weapon != null && weapon.weaponType == WeaponType.Shooting && rightArmReload <= 0)
            {
                FireWeapon(WeaponSlot.RightArm);

                rightArmReload = weapon.reloadTime;
            }
        }

        if(leftArmFiring)
        {
            MechWeaponPartData weapon = equipment.leftWeapon;

            if(weapon != null && weapon.weaponType == WeaponType.Shooting && leftArmReload <= 0)
            {
                FireWeapon(WeaponSlot.LeftArm);

                leftArmReload = weapon.reloadTime;
            }
        }

        if(rightShoulderFiring)
        {
            MechWeaponPartData weapon = equipment.rightShoulder;

            if(weapon != null && weapon.weaponType == WeaponType.Shooting && rightShoulderReload <= 0)
            {
                FireWeapon(WeaponSlot.RightShoulder);

                rightShoulderReload = weapon.reloadTime;
            }
        }

        if(leftShoulderFiring)
        {
            MechWeaponPartData weapon = equipment.leftShoulder;

            if(weapon != null && weapon.weaponType == WeaponType.Shooting && leftShoulderReload <= 0)
            {
                FireWeapon(WeaponSlot.LeftShoulder);

                leftShoulderReload = weapon.reloadTime;
            }
        }
    }
    private void ReleaseRightArmAttack()
    {
        rightArmCharging = false;

        MechWeaponPartData weapon = equipment.rightWeapon;

        if(weapon == null)
        {
            if(rightArmChargeTimer >= 1f)
            {
                UppercutAttack();
            }
            else
            {
                Punch(WeaponSlot.RightArm);
            }

            rightArmChargeTimer = 0;
            return;
        }

        if(weapon.canCharge && rightArmChargeTimer >= weapon.chargeTime)
        {
            ChargeAttack(weapon, WeaponSlot.RightArm);
        }
        else
        {
            FireWeapon(WeaponSlot.RightArm);
        }

        rightArmChargeTimer = 0;
    }
    private void ChargeAttack(MechWeaponPartData weapon, WeaponSlot slot)
    {
        switch(weapon.weaponType)
        {
            case WeaponType.Shooting:

            ShootChargeWeapon(weapon, slot);

            break;

            case WeaponType.Melee:

            ChargeBladeAttack(weapon);

            break;
        }
    }
    private void ShootChargeWeapon(MechWeaponPartData weapon, WeaponSlot slot)
    {
        Transform muzzle = GetMuzzle(slot);

        Quaternion shootRotation = muzzle.rotation;
        // GameObject bulletObj = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        GameObject bulletObj = Instantiate(weapon.projectilePrefab, muzzle.position, shootRotation);

        IProjectile projectile = bulletObj.GetComponent<IProjectile>();


        // if (projectile != null)
        // {
        //     projectile.Initialize(weapon.damageData, weapon.bulletSpeed, lockOn.currentTarget);
        // }

        if (projectile != null)
        {
            projectile.Initialize(weapon.chargeDamageData, weapon.bulletSpeed * 1.5f, lockOn.currentTarget, TeamType.Player);
        }
    }
    private void ChargeBladeAttack(MechWeaponPartData weapon)
    {
        // Collider[] hits = Physics.OverlapSphere(transform.position, weapon.attackRange * 2f);

        // foreach(Collider hit in hits)
        // {
        //     IDamageable damageable = hit.GetComponent<IDamageable>();

        //     if(damageable != null)
        //     {
        //         damageable.TakeDamage(weapon.chargeDamageData);
        //     }
        // }
    }
    // private void UppercutAttack()
    // {
    //     anim.SetTrigger("Uppercut");

    //     Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 2f, uppercutRange);

    //     foreach(Collider hit in hits)
    //     {
    //         IDamageable damageable =
    //         hit.GetComponent<IDamageable>();

    //         if(damageable != null)
    //         {
    //             damageable.TakeDamage(new DamageData(uppercutDamage, DamageType.Physical));
    //         }

    //         Rigidbody enemyRb = hit.GetComponent<Rigidbody>();

    //         if(enemyRb != null)
    //         {
    //             Vector3 launchDirection = transform.forward * 5f + Vector3.up * uppercutForce;

    //             enemyRb.AddForce(launchDirection, ForceMode.VelocityChange);
    //         }
    //     }
    // }
    // public void MeleeAttackEnd()
    // {
    //     mech.isAttack = false;
    // }
    private void MeleeWeaponAttack(MechWeaponPartData weapon)
    {
        GameObject weaponObject = null;

        if(weapon == equipment.leftWeapon)
        weaponObject = equipment.currentLeftWeapon;

        if(weapon == equipment.rightWeapon)
        weaponObject = equipment.currentRightWeapon;

        if(weaponObject == null)
        return;

        MechMeleeWeapon melee = weaponObject.GetComponent<MechMeleeWeapon>();

        if(melee == null)
        return;

        // melee.SetWeapon(weapon);

        // anim.SetTrigger(weapon.attackTrigger);
        StartCoroutine(MeleeApproachAndAttack(weapon, melee));
        // currentMeleeWeapon = equipment.CurrentMeleeWeapon;

        // if(currentMeleeWeapon == null)
        // {
        //     Debug.LogError("MechMeleeWeaponが見つかりません");
        //     return;
        // }
        // Debug.Log(currentMeleeWeapon.name);
        // currentMeleeWeapon.SetWeapon(weapon);
        // anim.SetTrigger(weapon.attackTrigger);
    }
    private IEnumerator MeleeApproachAndAttack(MechWeaponPartData weapon, MechMeleeWeapon melee)
    {
        if(lockOn == null || lockOn.currentTarget == null)
        {
            melee.SetWeapon(weapon);
            anim.SetTrigger(weapon.attackTrigger);
            yield break;
        }

        Transform target = lockOn.currentTarget;

        while(target != null)
        {
            Vector3 targetPos = target.position;

            Vector3 dir = (targetPos - transform.position);

            // dir.y = 0;

            float distance = dir.magnitude;

            if(distance <= meleeAttackDistance)
                break;

            transform.position += dir.normalized * mech.boostForce * Time.deltaTime;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 15f);

            yield return null;
        }

        melee.SetWeapon(weapon);

        anim.SetTrigger(weapon.attackTrigger);
    }
    private void UppercutAttack()
    {
        anim.SetTrigger("Uppercut");
    }
    public void ApplyUppercutHit()
    {
        // 自機を少し持ち上げる
        mech.upwardVelocity = 15f;
        
        // Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 2f, uppercutRange);

        // foreach(Collider hit in hits)
        // {
        //     IDamageable damageable = hit.GetComponent<IDamageable>();

        //     if(damageable != null)
        //     {
        //         damageable.TakeDamage(new DamageData(uppercutDamage, DamageType.Physical));
        //     }

        //     Rigidbody enemyRb = hit.GetComponent<Rigidbody>();

        //     if(enemyRb != null)
        //     {
        //         enemyRb.AddForce(transform.forward * 5f + Vector3.up * uppercutForce, ForceMode.VelocityChange);
        //     }
        // }
    }
    public void EnableMeleeWeaponHitbox()
    {
        MechMeleeWeapon melee = equipment.GetCurrentMeleeWeapon();

        if(melee == null)
        {
            Debug.LogError("MeleeWeaponなし");
            return;
        }

        melee.BeginAttack();
    }

    public void DisableMeleeWeaponHitbox()
    {
        MechMeleeWeapon melee = equipment.GetCurrentMeleeWeapon();

        if(melee == null)
        {
            Debug.LogError("MeleeWeaponなし");
            return;
        }

        melee.EndAttack();
    }
    public void EnableRightPunchHitbox()
    {
        rightHandHitbox.damageData = new DamageData(rightPunchDamage, DamageType.Physical);

        rightHandHitbox.Activate();
    }

    public void DisableRightPunchHitbox()
    {
        rightHandHitbox.Deactivate();
    }
    public void EnableLeftPunchHitbox()
    {
        leftHandHitbox.damageData = new DamageData(leftPunchDamage, DamageType.Physical);

        leftHandHitbox.Activate();
    }

    public void DisableLeftPunchHitbox()
    {
        leftHandHitbox.Deactivate();
    }
    public void EnableRightUppercutHitbox()
    {
        mech.upwardVelocity = 15f;

        rightHandHitbox.damageData = new DamageData(uppercutDamage, DamageType.Physical);

        rightHandHitbox.Activate();
    }
    public void EnableLeftUppercutHitbox()
    {
        mech.upwardVelocity = 15f;

        leftHandHitbox.damageData = new DamageData(uppercutDamage, DamageType.Physical);

        leftHandHitbox.Activate();
    }
    private Quaternion GetShootRotation(Transform muzzle)
    {
        if (lockOn != null && lockOn.currentTarget != null)
        {
            Vector3 dir = (lockOn.currentTarget.position - muzzle.position).normalized;

            return Quaternion.LookRotation(dir);
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            return Quaternion.LookRotation((hit.point - muzzle.position).normalized);
        }

        return Quaternion.LookRotation(ray.direction);
    }
}
    // public void EnableBladeHitbox()
    // {
    //     Debug.Log("Blade ON");
    // }

    // public void DisableBladeHitbox()
    // {
    //     Debug.Log("Blade OFF");
    // }
//     public void OnRightArmAttack(InputAction.CallbackContext context)
//     {
//         if (context.performed)
//         {
//             FireRightArmWeapon();
//         }
//         else if (context.canceled)
//         {
//             //
//         }
//     }
//     public void OnLeftArmAttack(InputAction.CallbackContext context)
//     {
//         if (context.performed)
//         {
//             FireLeftArmWeapon();
//         }
//     }
//     public void OnRightShoulderAttack(InputAction.CallbackContext context)
//     {
//         if (context.performed)
//         {
//             FireRightShoulderWeapon();
//         }
//     }
//     private void FireRightArmWeapon()
//     {
//         // 右手武器が無ければ撃たない
//         if (equipment.rightWeapon == null)
//         return;

//         ShootRightArmWeapon(equipment.rightWeapon);
//     }
//     private void FireLeftArmWeapon()
//     {
//         // 武器無し
//         if (equipment.leftWeapon == null)
//         {
//             PunchLeft();
//             return;
//         }

//         switch(equipment.leftWeapon.weaponType)
//         {
//             case WeaponType.Shooting:
//             ShootLeftArmWeapon(equipment.leftWeapon);
//             break;

//             case WeaponType.Melee:
//             MeleeLeftArmWeapon(equipment.leftWeapon);
//             break;

//             case WeaponType.Shield:
//             ShieldAttack(equipment.leftWeapon);
//             break;
//         }
//     }
//     private void FireRightShoulderWeapon()
//     {
//         // 右手武器が無ければ撃たない
//         if (equipment.rightShoulder == null)
//         return;

//         ShootRightShoulderWeapon(equipment.rightShoulder);
//     }
//     private void FireLeftShoulderWeapon()
//     {
//         // 右手武器が無ければ撃たない
//         if (equipment.leftShoulder == null)
//         return;

//         ShootLeftShoulderWeapon(equipment.leftShoulder);
//     }
//     private void ShootRightArmWeapon(MechRightArmWeaponPartData weaponData)
//     {
//         GameObject bulletObj = Instantiate(bulletPrefab, muzzleRightArm.position, muzzleRightArm.rotation);

//         MechBullet bullet = bulletObj.GetComponent<MechBullet>();

//         bullet.Initialize(weaponData.damageData, weaponData.bulletSpeed);
//     }
//     private void ShootLeftArmWeapon(MechLeftArmWeaponPartData weaponData)
//     {
//         GameObject bulletObj = Instantiate(bulletPrefab, muzzleLeftArm.position, muzzleLeftArm.rotation);

//         MechBullet bullet = bulletObj.GetComponent<MechBullet>();

//         bullet.Initialize(weaponData.damageData, weaponData.bulletSpeed);
//     }
//     private void MeleeLeftArmWeapon(MechLeftArmWeaponPartData weaponData)
//     {
//         Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

//         foreach(Collider hit in hits)
//         {
//             IDamageable damageable = hit.GetComponent<IDamageable>();

//             if(damageable != null)
//             {
//                 damageable.TakeDamage(weaponData.damageData);
//             }
//         }
//     }
//     private void PunchLeft()
//     {
//         Collider[] hits = Physics.OverlapSphere(transform.position, 2f);

//         foreach(Collider hit in hits)
//         {
//             IDamageable damageable = hit.GetComponent<IDamageable>();

//             if(damageable != null)
//             {
//                 damageable.TakeDamage(new DamageData(50, DamageType.Physical));
//             }
//         }
//     }
//     private void ShieldAttack(MechLeftArmWeaponPartData weaponData)
//     {
//         Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

//         foreach(Collider hit in hits)
//         {
//             IDamageable damageable = hit.GetComponent<IDamageable>();

//             if(damageable != null)
//             {
//                 damageable.TakeDamage(weaponData.damageData);
//             }
//         }
//     }
//     private void ShootRightShoulderWeapon(MechRightShoulderWeaponPartData weaponData)
//     {
//         GameObject bulletObj = Instantiate(bulletPrefab, muzzleRightShoulder.position, muzzleRightShoulder.rotation);

//         MechBullet bullet = bulletObj.GetComponent<MechBullet>();

//         bullet.Initialize(weaponData.damageData, weaponData.bulletSpeed);
//     }
//     private void ShootLeftShoulderWeapon(MechLeftShoulderWeaponPartData weaponData)
//     {
//         GameObject bulletObj = Instantiate(bulletPrefab, muzzleLeftShoulder.position, muzzleLeftShoulder.rotation);

//         MechBullet bullet = bulletObj.GetComponent<MechBullet>();

//         bullet.Initialize(weaponData.damageData, weaponData.bulletSpeed);
//     }
// }
