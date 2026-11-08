using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechStatsControl : MonoBehaviour
{
    public MechTotalStats CurrentStats
    {
        get;
        private set;
    }

    private MechEquipment equipment;

    void Awake()
    {
        equipment = GetComponent<MechEquipment>();

        RefreshStats();
    }

    public void RefreshStats()
    {
        CurrentStats = equipment.CalculateTotalStats();
    }
}
