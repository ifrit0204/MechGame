using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechStatsControl : MonoBehaviour
{
    public MechTotalStats CurrentStats { get; private set; }

    private MechEquipment equipment;

    void Awake()
    {
        equipment = GetComponent<MechEquipment>();
        RefreshStats();
    }

    public void RefreshStats()
    {
        if (equipment == null)
            equipment = GetComponent<MechEquipment>();

        if (equipment == null)
        {
            Debug.LogError("MechEquipment が見つかりません: " + name, this);
            return;
        }

        CurrentStats = equipment.CalculateTotalStats();
    }
}
