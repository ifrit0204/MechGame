using UnityEngine;

[CreateAssetMenu(menuName = "Mech/Arms")]
public class MechArmsPartData : MechPartData
{
    public float armorPoint; // Armor Point
    public float recoilReduction; // recoil
    public float armMobility; // arm speed
    public float armStabilityPerformance; // stability for aiming
    public float aimingPerformance; // aiming accuracy
    public float armMeleeWeaponAptitude; // melee weapon performance
}
