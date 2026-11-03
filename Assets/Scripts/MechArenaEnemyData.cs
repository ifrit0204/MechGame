using UnityEngine;

[CreateAssetMenu(fileName = "ArenaEnemy", menuName = "Mech/ArenaEnemy")]
public class MechArenaEnemyData : ScriptableObject
{
    public string enemyName;
    [TextArea(2, 5)]
    public string description;

    public int reward;

    // public GameObject enemyPrefab;

    public MechEnemyLoadout loadout;

    public int arenaRank;
}