using UnityEngine;

// パーツの組み合わせから、機体全体の能力値(MechTotalStats)を計算する。
// プレイヤー(MechEquipment)も敵(MechEnemyLoadout)も、この1か所を通すことで、
// 同じパーツなら必ず同じ数値になる。
public static class MechStatsCalculator
{
    // ------------------------------------------------------------
    // 入口(プレイヤーのLoadout / 敵のLoadout)
    // ------------------------------------------------------------

    public static MechTotalStats Calculate(MechLoadout loadout)
    {
        if (loadout == null)
            return new MechTotalStats();

        return Calculate(
            loadout.Head, loadout.Body, loadout.Arms, loadout.Legs,
            loadout.RightWeapon, loadout.LeftWeapon,
            loadout.RightShoulder, loadout.LeftShoulder,
            loadout.Booster, loadout.FCS, loadout.Generator);
    }

    public static MechTotalStats Calculate(MechEnemyLoadout loadout)
    {
        if (loadout == null)
            return new MechTotalStats();

        return Calculate(
            loadout.Head, loadout.Body, loadout.Arms, loadout.Legs,
            loadout.RightWeapon, loadout.LeftWeapon,
            loadout.RightShoulder, loadout.LeftShoulder,
            loadout.Booster, loadout.FCS, loadout.Generator);
    }

    // ------------------------------------------------------------
    // 本体(MechEquipment.CalculateTotalStats の中身をそのまま移したもの)
    // ------------------------------------------------------------

    public static MechTotalStats Calculate(
        MechHeadPartData head,
        MechBodyPartData body,
        MechArmsPartData arms,
        MechLegsPartData legs,
        MechWeaponPartData rightWeapon,
        MechWeaponPartData leftWeapon,
        MechWeaponPartData rightShoulder,
        MechWeaponPartData leftShoulder,
        MechBoosterPartData booster,
        MechFCSPartData fcs,
        MechGeneratorPartData generator)
    {
        MechTotalStats stats = new MechTotalStats();

        if (head != null)
        {
            stats.weight += head.weight;
            stats.energy += head.energyConsumption;
            stats.armor += head.armorPoint;
            stats.stability += head.headStabilityPerformance;
            stats.meleeAptitude += head.headMeleeWeaponAptitude;

            stats.scanRange = Mathf.Max(stats.scanRange, head.scanRange);
            stats.lockDistance = Mathf.Max(stats.lockDistance, head.lockDistance);
        }

        if (body != null)
        {
            stats.weight += body.weight;
            stats.energy += body.energyConsumption;
            stats.armor += body.armorPoint;
            stats.stability += body.bodyStabilityPerformance;
            stats.meleeAptitude += body.bodyMeleeWeaponAptitude;
            stats.energyRecharge += body.bodyEnergyRecharge;
        }

        if (arms != null)
        {
            stats.weight += arms.weight;
            stats.energy += arms.energyConsumption;
            stats.armor += arms.armorPoint;
            stats.stability += arms.armStabilityPerformance;
            stats.meleeAptitude += arms.armMeleeWeaponAptitude;

            stats.recoilReduction = arms.recoilReduction;
            stats.armMobility = arms.armMobility;
            stats.aimingPerformance = arms.aimingPerformance;
        }

        if (legs != null)
        {
            stats.weight += legs.weight;
            stats.energy += legs.energyConsumption;
            stats.armor += legs.armorPoint;
            stats.stability += legs.legStabilityPerformance;
            stats.meleeAptitude += legs.legMeleeWeaponAptitude;

            stats.moveSpeed = legs.moveSpeed;
            stats.jumpForce = legs.jumpForce;
            stats.jumpPerformance = legs.jumpPerformance;
            stats.groundMultiplier = legs.groundMultiplier;
        }

        if (booster != null)
        {
            stats.weight += booster.weight;
            stats.energy += booster.energyConsumption;

            stats.boostFirstForce = booster.boostFirstForce;
            stats.boostForce = booster.boostForce;
            stats.boostEfficiency = booster.boostEfficiency;
            stats.boostCost = booster.boostCost;
            stats.boostVerticalForce = booster.boostVerticalForce;
            stats.boostVerticalCost = booster.boostVerticalCost;
            stats.boostJumpForce = booster.boostJumpForce;
            stats.boostJumpCost = booster.boostJumpCost;
            stats.hyperBoostForce = booster.hyperBoostForce;
            stats.hyperBoostFirstForce = booster.hyperBoostFirstForce;
            stats.hyperBoostCost = booster.hyperBoostCost;
        }

        if (generator != null)
        {
            stats.weight += generator.weight;
            stats.energy += generator.energyConsumption;

            stats.boostEnergy = generator.boostEnergy;
            stats.boostEnergyMax = generator.boostEnergyMax;
            stats.energyRechargeRate = generator.energyRechargeRate;
            stats.energyRechargeRateBasic = generator.energyRechargeRateBasic;
        }

        AddWeaponStats(rightWeapon, stats);
        AddWeaponStats(leftWeapon, stats);
        AddWeaponStats(rightShoulder, stats);
        AddWeaponStats(leftShoulder, stats);

        if (fcs != null)
        {
            stats.weight += fcs.weight;
            stats.energy += fcs.energyConsumption;

            stats.lockRange = fcs.lockRange;
            stats.lockSpeed = fcs.lockSpeed;
            stats.lockRadius = fcs.lockRadius;
        }

        return stats;
    }

    private static void AddWeaponStats(MechWeaponPartData weapon, MechTotalStats stats)
    {
        if (weapon == null)
            return;

        stats.weight += weapon.weight;
        stats.energy += weapon.energyConsumption;

        stats.totalWeaponDamage += weapon.damageData.Damage;
        stats.totalAmmo += weapon.ammo;
    }
}
