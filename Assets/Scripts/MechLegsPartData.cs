using UnityEngine;

[CreateAssetMenu(menuName = "Mech/Legs")]
public class MechLegsPartData : MechPartData
{
    public float armorPoint; // Armor Point
    public float legStabilityPerformance; // stability
    public float legMeleeWeaponAptitude; // melee Weapon Performance
    public float moveSpeed, jumpForce, jumpPerformance, groundMultiplier, jumpTime, jumpTimer;
}
