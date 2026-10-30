using UnityEngine;

public abstract class MechPartData : ScriptableObject
{
    public string partName;
    public Sprite icon;
    public GameObject modelPrefab;
    public float weight, energyConsumption;
    public int price;
    // public WeaponType weaponType;
}