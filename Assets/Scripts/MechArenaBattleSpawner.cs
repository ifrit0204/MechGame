using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechArenaBattleSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyMechPrefab;

    private void Start()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        MechArenaEnemyData enemyData = MechGameManager.Instance.CurrentArenaEnemy;

        if(enemyData == null)
            return;

        GameObject enemy = Instantiate(enemyMechPrefab, transform.position, transform.rotation);

        MechEquipment equipment = enemy.GetComponent<MechEquipment>();

        ApplyEnemyLoadout(equipment, enemyData.loadout);
    }

    private void ApplyEnemyLoadout(MechEquipment equipment, MechEnemyLoadout loadout)
    {
        MechLoadout temp = new MechLoadout();

        temp.Head = loadout.Head;
        temp.Body = loadout.Body;
        temp.Arms = loadout.Arms;
        temp.Legs = loadout.Legs;

        temp.RightWeapon = loadout.RightWeapon;
        temp.LeftWeapon = loadout.LeftWeapon;

        temp.RightShoulder = loadout.RightShoulder;

        temp.LeftShoulder = loadout.LeftShoulder;

        temp.Booster = loadout.Booster;
        temp.FCS = loadout.FCS;
        temp.Generator = loadout.Generator;

        equipment.ApplyLoadout(temp);
    }
}