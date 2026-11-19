using UnityEngine;

[CreateAssetMenu(menuName = "Mech/Body")]
public class MechBodyPartData : MechPartData
{
    public float armorPoint; // Armor Point
    public float bodyStabilityPerformance; // stability
    public float bodyMeleeWeaponAptitude; // melee Weapon Performance
    public float bodyEnergyRecharge; // generator recharge
    public float bodyBoostEfficiency; // booster efficiency
}