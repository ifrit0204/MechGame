using UnityEngine;

public class MechPartDatabase : MonoBehaviour
{
    [Header("Frame")]
    public MechHeadPartData[] heads;
    public MechBodyPartData[] bodies;
    public MechArmsPartData[] arms;
    public MechLegsPartData[] legs;

    [Header("Weapons")]
    public MechWeaponPartData[] rightWeapons;
    public MechWeaponPartData[] leftWeapons;
    public MechWeaponPartData[] rightShoulderWeapons;
    public MechWeaponPartData[] leftShoulderWeapons;

    [Header("System")]
    public MechBoosterPartData[] boosters;
    public MechFCSPartData[] FCSs;
    public MechGeneratorPartData[] generators;


    public MechPartData[] GetParts(PartType partType)
    {
        switch (partType)
        {
            case PartType.Head:
                return heads;

            case PartType.Body:
                return bodies;

            case PartType.Arms:
                return arms;

            case PartType.Legs:
                return legs;

            case PartType.RightWeapon:
                return rightWeapons;

            case PartType.LeftWeapon:
                return leftWeapons;

            case PartType.RightShoulder:
                return rightShoulderWeapons;

            case PartType.LeftShoulder:
                return leftShoulderWeapons;

            case PartType.Booster:
                return boosters;

            case PartType.FCS:
                return FCSs;

            case PartType.Generator:
                return generators;
        }

        return null;
    }
}
