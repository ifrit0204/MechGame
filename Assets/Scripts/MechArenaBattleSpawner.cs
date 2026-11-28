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

        if (enemyData.loadout == null)
        {
            Debug.LogError("MechArenaEnemyData に loadout が設定されていません: " + enemyData.enemyName, enemyData);
            return;
        }

        GameObject enemy = Instantiate(enemyMechPrefab, transform.position, transform.rotation);

        MechEnemyAI enemyAI = enemy.GetComponent<MechEnemyAI>();

        if (enemyAI == null)
        {
            Debug.LogError("MechEnemyAI is missing from the enemy prefab.", enemy);
            Destroy(enemy);
            return;
        }

        enemyAI.Target = playerTarget;

        // Loadout に指定された思考設定を適用する(未設定なら何もしない)
        enemyAI.ApplyProfile(enemyData.loadout.aiProfile);

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

        // パーツを装備し終えたあとで、能力値を計算して敵の動きに渡す
        enemy.GetComponent<MechEnemyControl>().ApplyParts();

        enemy.GetComponent<MechStatsControl>().RefreshStats();
    }

    // 敵のパーツを装備する。
    // equipment.ApplyLoadout() は MechGameManager.CurrentLoadout(プレイヤーの装備)を
    // 書き換えてしまうので、敵には使わず、EquipPart で1つずつ装備する。
    private void ApplyEnemyLoadout(MechEquipment equipment, MechEnemyLoadout loadout)
    {
        equipment.EquipPart(PartType.Head, loadout.Head);
        equipment.EquipPart(PartType.Body, loadout.Body);
        equipment.EquipPart(PartType.Arms, loadout.Arms);
        equipment.EquipPart(PartType.Legs, loadout.Legs);

        equipment.EquipPart(PartType.RightWeapon, loadout.RightWeapon);
        equipment.EquipPart(PartType.LeftWeapon, loadout.LeftWeapon);

        equipment.EquipPart(PartType.RightShoulder, loadout.RightShoulder);
        equipment.EquipPart(PartType.LeftShoulder, loadout.LeftShoulder);

        equipment.EquipPart(PartType.Booster, loadout.Booster);
        equipment.EquipPart(PartType.FCS, loadout.FCS);
        equipment.EquipPart(PartType.Generator, loadout.Generator);
    }
}
