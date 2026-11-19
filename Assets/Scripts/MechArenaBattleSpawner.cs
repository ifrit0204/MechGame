using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechArenaBattleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyMechPrefab;

    private IEnumerator Start()
    {
        if (MechGameManager.Instance.CurrentArenaEnemy == null)
            yield break;

        MechControl player = FindFirstObjectByType<MechControl>();

        while (player == null)
        {
            yield return null;
            player = FindFirstObjectByType<MechControl>();
        }

        SpawnEnemy(player.transform);
    }

    private void SpawnEnemy(Transform playerTarget)
    {
        MechArenaEnemyData enemyData = MechGameManager.Instance.CurrentArenaEnemy;

        if(enemyData == null)
            return;

        GameObject enemy = Instantiate(enemyMechPrefab, transform.position, transform.rotation);

        MechEnemyAI enemyAI = enemy.GetComponent<MechEnemyAI>();

        if (enemyAI == null)
        {
            Debug.LogError("MechEnemyAI is missing from the enemy prefab.", enemy);
            Destroy(enemy);
            return;
        }

        enemyAI.Target = playerTarget;

        MechEquipment equipment = enemy.GetComponent<MechEquipment>();

        MechEnemyWeapon enemyWeapon = enemy.GetComponent<MechEnemyWeapon>();
        enemyWeapon.SetWeapon(enemyData.loadout.RightWeapon);

        ApplyEnemyLoadout(equipment, enemyData.loadout);

        GameObject weaponObj = equipment.currentRightWeapon;

        if (weaponObj == null)
        {
            if (enemyData.loadout.RightWeapon != null)
                Debug.LogError("currentRightWeapon is null", enemy);
        }
        else
        {
            enemyWeapon.SetMuzzle(weaponObj.transform);
        }

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