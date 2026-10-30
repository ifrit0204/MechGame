using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechBattleSpawner : MonoBehaviour
{
    public GameObject mechPrefab;

    void Start()
    {
        GameObject mech = Instantiate(mechPrefab, transform.position, transform.rotation);

        MechCameraControl cameraControl = FindFirstObjectByType<MechCameraControl>();

        cameraControl.target = mech.transform;

        MechEquipment equipment = mech.GetComponent<MechEquipment>();

        equipment.ApplyLoadout(MechGameManager.Instance.CurrentLoadout);

        MechLockOn lockOn = mech.GetComponent<MechLockOn>();

        MechLockOnUI ui = FindFirstObjectByType<MechLockOnUI>();

        ui.SetLockOn(lockOn);
        MechEnemyAI[] enemies = FindObjectsByType<MechEnemyAI>(FindObjectsSortMode.None);

        MechGameManager.Instance.BeginBattle();

        foreach (MechEnemyAI enemy in enemies)
        {
            enemy.Target = mech.transform;
        }
    }
    // public GameObject mechPrefab;

    // void Start()
    // {
        
    //     GameObject mech = Instantiate(mechPrefab, transform.position, transform.rotation);

    //     MechEquipment equipment = mech.GetComponent<MechEquipment>();

    //     equipment.ApplyLoadout(MechGameManager.Instance.CurrentLoadout);
    // }
// void Start()
// {
//     GameObject mech = Instantiate(mechPrefab, transform.position, transform.rotation);

//     // カメラ設定
//     MechCameraControl cameraControl = FindFirstObjectByType<MechCameraControl>();
//     cameraControl.target = mech.transform;

//     // 装備適用
//     MechEquipment equipment = mech.GetComponent<MechEquipment>();
//     equipment.ApplyLoadout(MechGameManager.Instance.CurrentLoadout);

//     // ロックオン設定
//     MechLockOn lockOn = mech.GetComponent<MechLockOn>();
//     MechLockOnUI ui = FindFirstObjectByType<MechLockOnUI>();
//     ui.SetLockOn(lockOn);

//     // 敵全員の追尾対象をプレイヤーへ変更
//     MechEnemyAI[] enemies = FindObjectsByType<MechEnemyAI>(
//         FindObjectsSortMode.None);

//     foreach (MechEnemyAI enemy in enemies)
//     {
//         enemy.target = mech.transform;
//     }
// }
}
