using UnityEngine;

public class MechAmmoControl : MonoBehaviour
{
    private MechEquipment equipment;

    // WeaponSlot の順(RightArm, LeftArm, RightShoulder, LeftShoulder)
    private readonly int[] current = new int[4];
    private readonly int[] max = new int[4];

    void Awake()
    {
        equipment = GetComponent<MechEquipment>();
    }

    // 装備を反映した後に呼ぶ(弾数を満タンにする)
    public void Initialize()
    {
        if (equipment == null)
            equipment = GetComponent<MechEquipment>();

        foreach (WeaponSlot slot in System.Enum.GetValues(typeof(WeaponSlot)))
        {
            MechWeaponPartData weapon = GetWeapon(slot);
            int i = (int)slot;

            max[i] = weapon != null ? weapon.ammo : 0;
            current[i] = max[i];
        }
    }

    public MechWeaponPartData GetWeapon(WeaponSlot slot)
    {
        if (equipment == null) return null;

        switch (slot)
        {
            case WeaponSlot.RightArm:      return equipment.rightWeapon;
            case WeaponSlot.LeftArm:       return equipment.leftWeapon;
            case WeaponSlot.RightShoulder: return equipment.rightShoulder;
            case WeaponSlot.LeftShoulder:  return equipment.leftShoulder;
        }
        return null;
    }

    // 弾を使う武器か(近接・シールドなどは弾数なし)
    public bool UsesAmmo(WeaponSlot slot)
    {
        MechWeaponPartData weapon = GetWeapon(slot);

        return weapon != null
            && weapon.weaponType != WeaponType.Melee
            && weapon.weaponType != WeaponType.Shield
            && weapon.weaponType != WeaponType.None;
    }

    public int GetAmmo(WeaponSlot slot)    { return current[(int)slot]; }
    public int GetMaxAmmo(WeaponSlot slot) { return max[(int)slot]; }

    // 撃てるか確認して、弾を減らす。撃てれば true
    public bool TryConsume(WeaponSlot slot, int amount = 1)
    {
        if (!UsesAmmo(slot))
            return true;   // 弾を使わない武器は常に使える

        int i = (int)slot;

        if (current[i] < amount)
            return false;

        current[i] -= amount;
        return true;
    }
}