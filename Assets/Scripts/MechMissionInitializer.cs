using UnityEngine;

public class MechMissionInitializer : MonoBehaviour
{
    void Start()
    {
        MechGameManager.Instance.InitializeEnemyCount();

        MechGameManager.Instance.BeginBattle();
    }
}