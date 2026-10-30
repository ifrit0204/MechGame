public enum DamageType
{
    Physical, Energy, Explosion
}
public enum WeaponType
{
    None, Shooting, Melee, Shield, Special
}
public enum WeaponSlot
{
    RightArm, LeftArm, RightShoulder, LeftShoulder
}
[System.Serializable]
public struct DamageData
{
    public int Damage;
    public DamageType Type;

    public DamageData(int damage, DamageType type)
    {
        Damage = damage;
        Type = type;
    }
}
public interface IDamageable
{
    void TakeDamage(DamageData damageData);
}