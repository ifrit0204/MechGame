using System;

[Serializable]
public class MechLoadout
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
    public MechGeneratorPartData Generator;
    public MechFCSPartData FCS;
    public MechLoadout Clone()
    {
        return new MechLoadout
        {
            Head = Head,
            Body = Body,
            Arms = Arms,
            Legs = Legs,

            RightWeapon = RightWeapon,
            LeftWeapon = LeftWeapon,

            RightShoulder = RightShoulder,
            LeftShoulder = LeftShoulder,

            Booster = Booster,
            Generator = Generator,
            FCS = FCS
        };
    }
}