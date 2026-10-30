using UnityEngine;

[CreateAssetMenu(fileName = "EnemyLoadout", menuName = "Mech/EnemyLoadout")]
public class MechEnemyLoadout : ScriptableObject
{
    public MechHeadPartData Head;
    public MechBodyPartData Body;
    public MechArmsPartData Arms;
    public MechLegsPartData Legs;

    public MechWeaponPartData RightWeapon;
    public MechWeaponPartData LeftWeapon;

    public MechWeaponPartData RightShoulder;
    public MechWeaponPartData LeftShoulder;

    public MechBoosterPartData Booster;
    public MechFCSPartData FCS;
    public MechGeneratorPartData Generator;
}