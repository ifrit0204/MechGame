using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechAttackAction : MonoBehaviour
{
    [SerializeField] private MechControl mech;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzleRightArm;
    [SerializeField] private Transform muzzleLeftArm;
    [SerializeField] private Transform muzzleRightShoulder;
    [SerializeField] private Transform muzzleLeftShoulder;
    [SerializeField] private MechHitbox rightHandHitbox;
    [SerializeField] private MechHitbox leftHandHitbox;

    public float attackRange;
    [SerializeField] private float uppercutRange;
    [SerializeField] private float uppercutForce;
    [SerializeField] private int uppercutDamage;
    [SerializeField] private int rightPunchDamage;
    [SerializeField] private int leftPunchDamage;

    [SerializeField] private float meleeDashSpeed;
    [SerializeField] private float meleeAttackDistance;

    [Header("Charge / Unarmed (チャージ・素手)")]
    // 素手のとき、この秒数以上押し続けてから離すとアッパーカット(それ未満ならパンチ)
    [SerializeField] private float uppercutHoldTime = 1f;
    // チャージ射撃の弾速倍率
    [SerializeField] private float chargeBulletSpeedRate = 1.5f;
    // 近接攻撃の前に、ロック対象へ詰める最大時間(秒)
    [SerializeField] private float meleeApproachMaxTime = 1.5f;
    // Animator のトリガー名(大文字・小文字も Animator と同じにすること)
    [SerializeField] private string punchRightTrigger = "PunchRight";
    [SerializeField] private string punchLeftTrigger = "PunchLeft";
    [SerializeField] private string uppercutTrigger = "UpperCut";

    [Header("Melee Aptitude (近接適性)")]
    // Aptitude 1ポイントあたりの攻撃力アップ率(0.01 = 1ポイントで+1%)
    [SerializeField] private float aptitudeDamageRate = 0.01f;

    private MechEquipment equipment;
    private MechLockOn lockOn;
    private Animator anim;
    private MechAmmoControl ammo;

    // 攻撃ボタン1つ分の状態
    private class SlotInput
    {
        public bool held;          // 押している最中か
        public float holdTime;     // 押し続けている時間
        public bool chargeFired;   // 近接のチャージ攻撃を既に出したか
        public float reload;       // 連射・再発射までの残り時間
    }

    // WeaponSlot の順(RightArm, LeftArm, RightShoulder, LeftShoulder)
    private readonly SlotInput[] inputs =
    {
        new SlotInput(), new SlotInput(), new SlotInput(), new SlotInput()
    };

    // ボタンを押したときの動き方
    private enum AttackStyle
    {
        None,         // 何もしない(武器なしの肩など)
        Unarmed,      // 素手: タップ=パンチ / 長押しして離す=アッパーカット
        AutoFire,     // 射撃(チャージなし): 押している間、連射
        ChargeShot,   // 射撃(チャージあり): 押してチャージ → 離すとチャージ射撃
        Instant,      // 近接(チャージなし)など: 押した瞬間に攻撃
        MeleeCharge   // 近接(チャージあり): 一定時間押し続けるとチャージ攻撃に移行
    }

    private bool meleeBusy;
    private MechMeleeWeapon activeMelee;
    private readonly Dictionary<MechWeaponPartData, MechWeaponPartData> chargeDataCache =
        new Dictionary<MechWeaponPartData, MechWeaponPartData>();

    private Transform CurrentLockTarget
    {
        get { return lockOn != null ? lockOn.currentTarget : null; }
    }

    void Start()
    {
        equipment = GetComponent<MechEquipment>();
        lockOn = GetComponent<MechLockOn>();
        anim = GetComponent<Animator>();
        ammo = GetComponent<MechAmmoControl>();

        PlayerInput playerInput = GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            var keyboardMap = playerInput.actions.FindActionMap("Keyboard");
            var xboxMap = playerInput.actions.FindActionMap("XboxControl");
            if (keyboardMap != null) keyboardMap.Enable();
            if (xboxMap != null) xboxMap.Enable();
        }
    }

    private void OnDestroy()
    {
        foreach (MechWeaponPartData data in chargeDataCache.Values)
        {
            if (data != null)
                Destroy(data);
        }
    }

    // ------------------------------------------------------------
    // 入力(PlayerInput のイベントから呼ばれる。メソッド名は変えない)
    // ------------------------------------------------------------

    public void OnRightArmAttack(InputAction.CallbackContext context)
    {
        HandleAttackInput(WeaponSlot.RightArm, context);
    }

    public void OnLeftArmAttack(InputAction.CallbackContext context)
    {
        HandleAttackInput(WeaponSlot.LeftArm, context);
    }

    public void OnRightShoulderAttack(InputAction.CallbackContext context)
    {
        HandleAttackInput(WeaponSlot.RightShoulder, context);
    }

    public void OnLeftShoulderAttack(InputAction.CallbackContext context)
    {
        HandleAttackInput(WeaponSlot.LeftShoulder, context);
    }

    private float GetMeleeDamageMultiplier()
    {
        MechTotalStats stats = equipment.CalculateTotalStats();

        return 1f + stats.meleeAptitude * aptitudeDamageRate;
    }

    private void HandleAttackInput(WeaponSlot slot, InputAction.CallbackContext context)
    {
        if (context.started)
            OnAttackPressed(slot);
        else if (context.canceled)
            OnAttackReleased(slot);
    }

    private void OnAttackPressed(WeaponSlot slot)
    {
        SlotInput input = inputs[(int)slot];

        if (input.held)
            return;

        input.held = true;
        input.holdTime = 0f;
        input.chargeFired = false;

        // 押した瞬間に攻撃するのは「Instant」だけ。
        // それ以外は、押し続けた時間や離したタイミングで決まる。
        if (GetAttackStyle(slot, GetWeapon(slot)) == AttackStyle.Instant)
        {
            FireWeapon(slot);
        }
    }

    private void OnAttackReleased(WeaponSlot slot)
    {
        SlotInput input = inputs[(int)slot];

        if (!input.held)
            return;

        float holdTime = input.holdTime;

        input.held = false;
        input.holdTime = 0f;

        MechWeaponPartData weapon = GetWeapon(slot);

        switch (GetAttackStyle(slot, weapon))
        {
            case AttackStyle.Unarmed:
                // 長押しして離す → アッパーカット / 押してすぐ離す → パンチ
                if (holdTime >= uppercutHoldTime)
                    UppercutAttack();
                else
                    Punch(slot);
                break;

            case AttackStyle.ChargeShot:
                if (input.reload > 0f)
                    break;

                // チャージ完了していればチャージ射撃、足りなければ通常射撃
                if (holdTime >= weapon.chargeTime)
                    ChargeAttack(weapon, slot);
                else
                    FireWeapon(slot);

                input.reload = weapon.reloadTime;
                break;

            case AttackStyle.MeleeCharge:
                // チャージ攻撃に移行していなければ、離したときに通常攻撃
                if (!input.chargeFired)
                    FireWeapon(slot);
                break;
        }

        input.chargeFired = false;
    }

    private void Update()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            UpdateSlot((WeaponSlot)i);
        }
    }

    private void UpdateSlot(WeaponSlot slot)
    {
        SlotInput input = inputs[(int)slot];

        if (input.reload > 0f)
            input.reload -= Time.deltaTime;

        if (!input.held)
            return;

        input.holdTime += Time.deltaTime;

        MechWeaponPartData weapon = GetWeapon(slot);

        switch (GetAttackStyle(slot, weapon))
        {
            case AttackStyle.AutoFire:
                if (input.reload <= 0f)
                {
                    FireWeapon(slot);
                    input.reload = weapon.reloadTime;
                }
                break;

            case AttackStyle.MeleeCharge:
                // 一定時間押し続けたら、離さなくてもチャージ攻撃に移行する
                if (!input.chargeFired && input.holdTime >= weapon.chargeTime)
                {
                    input.chargeFired = true;
                    ChargeAttack(weapon, slot);
                }
                break;
        }
    }

    // UIのチャージゲージ用: 0〜1
    public float GetChargeRate(WeaponSlot slot)
    {
        SlotInput input = inputs[(int)slot];

        if (!input.held)
            return 0f;

        MechWeaponPartData weapon = GetWeapon(slot);

        if (weapon != null && !weapon.canCharge)
            return 0f;

        float full = weapon != null ? weapon.chargeTime : uppercutHoldTime;

        if (full <= 0f)
            return 0f;

        return Mathf.Clamp01(input.holdTime / full);
    }

    // ------------------------------------------------------------
    // 武器の種類 → 動き方
    // ------------------------------------------------------------

    private AttackStyle GetAttackStyle(WeaponSlot slot, MechWeaponPartData weapon)
    {
        if (weapon == null)
        {
            // 素手の動きがあるのは両腕だけ
            bool isArm = slot == WeaponSlot.RightArm || slot == WeaponSlot.LeftArm;

            return isArm ? AttackStyle.Unarmed : AttackStyle.None;
        }

        switch (weapon.weaponType)
        {
            case WeaponType.Melee:
                return weapon.canCharge ? AttackStyle.MeleeCharge : AttackStyle.Instant;

            case WeaponType.Shooting:
                return weapon.canCharge ? AttackStyle.ChargeShot : AttackStyle.AutoFire;

            default:
                return AttackStyle.Instant;
        }
    }

    private MechWeaponPartData GetWeapon(WeaponSlot slot)
    {
        switch (slot)
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

    private GameObject GetWeaponObject(WeaponSlot slot)
    {
        switch (slot)
        {
            case WeaponSlot.RightArm:
                return equipment.currentRightWeapon;

            case WeaponSlot.LeftArm:
                return equipment.currentLeftWeapon;

            case WeaponSlot.RightShoulder:
                return equipment.currentRightShoulder;

            case WeaponSlot.LeftShoulder:
                return equipment.currentLeftShoulder;
        }

        return null;
    }

    private Transform GetMuzzle(WeaponSlot slot)
    {
        switch (slot)
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

    // ------------------------------------------------------------
    // 通常攻撃
    // ------------------------------------------------------------

    private void FireWeapon(WeaponSlot slot)
    {
        MechWeaponPartData weapon = GetWeapon(slot);

        if (weapon == null)
        {
            Punch(slot);
            return;
        }

        switch (weapon.weaponType)
        {
            case WeaponType.Shooting:
                ShootWeapon(weapon, slot);
                break;

            case WeaponType.Melee:
                MeleeWeaponAttack(weapon, slot, false);
                break;

            case WeaponType.Shield:
                ShieldAttack(weapon);
                break;
        }
    }

    private void ShootWeapon(MechWeaponPartData weapon, WeaponSlot slot)
    {
        Transform muzzle = GetMuzzle(slot);

        if (muzzle == null || weapon.projectilePrefab == null)
            return;

        if (ammo != null && !ammo.TryConsume(slot))
            return;

        GameObject bulletObj = Instantiate(weapon.projectilePrefab, muzzle.position, GetShootRotation(muzzle));

        IProjectile projectile = bulletObj.GetComponent<IProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(weapon.damageData, weapon.bulletSpeed, CurrentLockTarget, TeamType.Player);
        }
    }

    private void ShieldAttack(MechWeaponPartData weapon)
    {
        // 未実装
    }

    // ------------------------------------------------------------
    // チャージ攻撃
    // ------------------------------------------------------------

    private void ChargeAttack(MechWeaponPartData weapon, WeaponSlot slot)
    {
        switch (weapon.weaponType)
        {
            case WeaponType.Shooting:
                ShootChargeWeapon(weapon, slot);
                break;

            case WeaponType.Melee:
                MeleeWeaponAttack(weapon, slot, true);
                break;
        }
    }

    private void ShootChargeWeapon(MechWeaponPartData weapon, WeaponSlot slot)
    {
        Transform muzzle = GetMuzzle(slot);

        if (muzzle == null || weapon.projectilePrefab == null)
            return;

        if (ammo != null && !ammo.TryConsume(slot))
            return;

        // 通常射撃と同じ照準(ロック対象 or 画面中央)で撃つ
        GameObject bulletObj = Instantiate(weapon.projectilePrefab, muzzle.position, GetShootRotation(muzzle));

        IProjectile projectile = bulletObj.GetComponent<IProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(weapon.chargeDamageData, weapon.bulletSpeed * chargeBulletSpeedRate, CurrentLockTarget, TeamType.Player);
        }
    }

    // ------------------------------------------------------------
    // 近接武器(通常 / チャージ)
    // ------------------------------------------------------------

    private void MeleeWeaponAttack(MechWeaponPartData weapon, WeaponSlot slot, bool charged)
    {
        // 前の攻撃の「詰め」が終わるまでは、新しい攻撃を受け付けない
        if (meleeBusy)
            return;

        GameObject weaponObject = GetWeaponObject(slot);

        if (weaponObject == null)
            return;

        MechMeleeWeapon melee = weaponObject.GetComponentInChildren<MechMeleeWeapon>();

        if (melee == null)
            return;

        // アニメーションイベント(EnableMeleeWeaponHitbox など)が呼ぶ対象を覚えておく
        activeMelee = melee;

        // チャージ攻撃は、チャージ用のダメージを持つデータで攻撃する
        MechWeaponPartData attackData = charged ? GetChargeData(weapon) : weapon;

        string trigger = weapon.attackTrigger;

        if (charged && !string.IsNullOrEmpty(weapon.chargeAttackTrigger))
            trigger = weapon.chargeAttackTrigger;

        StartCoroutine(MeleeApproachAndAttack(attackData, melee, trigger));
    }

    // チャージ攻撃用に、damageData を chargeDamageData に差し替えた複製を使う
    // (元のアセットは書き換えない)
    private MechWeaponPartData GetChargeData(MechWeaponPartData weapon)
    {
        MechWeaponPartData data;

        if (!chargeDataCache.TryGetValue(weapon, out data) || data == null)
        {
            data = Instantiate(weapon);
            data.damageData = weapon.chargeDamageData;

            chargeDataCache[weapon] = data;
        }

        return data;
    }

    private IEnumerator MeleeApproachAndAttack(MechWeaponPartData attackData, MechMeleeWeapon melee, string trigger)
    {
        meleeBusy = true;

        try
        {
            Transform target = CurrentLockTarget;

            if (target != null)
            {
                float elapsed = 0f;

                while (target != null && elapsed < meleeApproachMaxTime)
                {
                    Vector3 dir = target.position - transform.position;

                    if (dir.magnitude <= meleeAttackDistance)
                        break;

                    transform.position += dir.normalized * mech.boostForce * Time.deltaTime;

                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 15f);

                    elapsed += Time.deltaTime;

                    yield return null;
                }
            }

            // melee.SetWeapon(attackData);
            melee.SetWeapon(attackData, GetMeleeDamageMultiplier());

            anim.SetTrigger(trigger);
        }
        finally
        {
            meleeBusy = false;
        }
    }

    // ------------------------------------------------------------
    // 素手(パンチ / アッパーカット)
    // ------------------------------------------------------------

    private void Punch(WeaponSlot slot)
    {
        switch (slot)
        {
            case WeaponSlot.RightArm:
                anim.SetTrigger(punchRightTrigger);
                break;

            case WeaponSlot.LeftArm:
                anim.SetTrigger(punchLeftTrigger);
                break;
        }
    }

    private void UppercutAttack()
    {
        anim.SetTrigger(uppercutTrigger);
    }

    // ------------------------------------------------------------
    // アニメーションイベントの受け口(名前は変えない)
    // ------------------------------------------------------------

    public void ApplyUppercutHit()
    {
        // 自機を少し持ち上げる
        mech.upwardVelocity = 15f;
    }

    private MechMeleeWeapon GetActiveMelee()
    {
        if (activeMelee != null)
            return activeMelee;

        return equipment.GetCurrentMeleeWeapon();
    }

    public void EnableMeleeWeaponHitbox()
    {
        MechMeleeWeapon melee = GetActiveMelee();

        if (melee == null)
        {
            Debug.LogError("MeleeWeaponなし");
            return;
        }

        melee.BeginAttack();
    }

    public void DisableMeleeWeaponHitbox()
    {
        MechMeleeWeapon melee = GetActiveMelee();

        if (melee == null)
        {
            Debug.LogError("MeleeWeaponなし");
            return;
        }

        melee.EndAttack();
    }

    public void EnableRightPunchHitbox()
    {
        int damage = Mathf.RoundToInt(rightPunchDamage * GetMeleeDamageMultiplier());
        rightHandHitbox.damageData = new DamageData(damage, DamageType.Physical);

        rightHandHitbox.Activate();
    }

    public void DisableRightPunchHitbox()
    {
        rightHandHitbox.Deactivate();
    }

    public void EnableLeftPunchHitbox()
    {
        int damage = Mathf.RoundToInt(leftPunchDamage * GetMeleeDamageMultiplier());
        leftHandHitbox.damageData = new DamageData(damage, DamageType.Physical);

        leftHandHitbox.Activate();
    }

    public void DisableLeftPunchHitbox()
    {
        leftHandHitbox.Deactivate();
    }

    public void EnableRightUppercutHitbox()
    {
        mech.upwardVelocity = 15f;

        int damage = Mathf.RoundToInt(rightPunchDamage * GetMeleeDamageMultiplier());
        rightHandHitbox.damageData = new DamageData(damage, DamageType.Physical);

        rightHandHitbox.Activate();
    }

    public void EnableLeftUppercutHitbox()
    {
        mech.upwardVelocity = 15f;

        int damage = Mathf.RoundToInt(leftPunchDamage * GetMeleeDamageMultiplier());
        leftHandHitbox.damageData = new DamageData(damage, DamageType.Physical);

        leftHandHitbox.Activate();
    }

    // ------------------------------------------------------------
    // 照準
    // ------------------------------------------------------------

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
