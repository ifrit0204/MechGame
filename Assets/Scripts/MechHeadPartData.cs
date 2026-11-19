using UnityEngine;

// public PartType partType;
[CreateAssetMenu(menuName = "Mech/Head")]
public class MechHeadPartData : MechPartData
{
    public float armorPoint; // Armor Point
    public float headStabilityPerformance; // stability
    public float headMeleeWeaponAptitude; // melee Weapon Performance
    public float scanRange;
    public float lockDistance;
}