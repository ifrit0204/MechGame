using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechArenaBattleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyMechPrefab;

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

        enemy.GetComponent<MechEnemyWeapon>().SetWeapon(enemyData.loadout.RightWeapon);

        ApplyEnemyLoadout(equipment, enemyData.loadout);

        Debug.Log(equipment.currentRightWeapon);

        GameObject weaponObj = equipment.currentRightWeapon;

        if (weaponObj == null)
        {
            Debug.LogError("currentRightWeapon is null");

            return;
        }

        Debug.Log(enemyData.loadout.RightWeapon);

        // Transform muzzle = weaponObj.transform.Find(weaponData.muzzleName);

        // Transform muzzle = weaponObj.transform.Find("Muzzle");

        Transform muzzle = weaponObj.transform;

        enemy.GetComponent<MechEnemyWeapon>().SetMuzzle(weaponObj.transform);

        Debug.Log("Muzzle = " + muzzle);

        Debug.Log(muzzle);

        enemy.GetComponent<MechEnemyWeapon>().SetMuzzle(muzzle);

        Debug.Log(weaponObj);

        // Debug.Log(weaponObj.GetComponent<MechWeaponModel>());

        // MechWeaponModel model = weaponObj.GetComponent<MechWeaponModel>();

        // enemy.GetComponent<MechEnemyWeapon>().SetMuzzle(model.muzzle);
        
        enemy.GetComponent<MechStatsControl>().RefreshStats();
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