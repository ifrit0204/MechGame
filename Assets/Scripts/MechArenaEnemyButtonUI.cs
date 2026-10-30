using UnityEngine;
using UnityEngine.UI;

public class MechArenaEnemyButtonUI : MonoBehaviour
{
    [SerializeField] private Text enemyNameText;

    [SerializeField] private Text rewardText;

    public void SetEnemy(MechArenaEnemyData enemy)
    {
        enemyNameText.text = enemy.enemyName;

        rewardText.text = enemy.reward + " C";
    }
}