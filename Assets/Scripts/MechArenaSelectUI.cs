using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MechArenaSelectUI : MonoBehaviour
{
    [SerializeField] private MechArenaEnemyData[] enemies;

    private MechArenaEnemyData selectedEnemy;

    // [SerializeField] private MechArenaEnemyData[] enemies;

    // private MechArenaEnemyData selectedEnemy;

    [SerializeField] private Transform content;

    [SerializeField] private GameObject buttonPrefab;

    [SerializeField] private Text enemyNameText;

    [SerializeField] private Text descriptionText;

    [SerializeField] private Text rewardText;

    [SerializeField] public Button startButton;

    private List<Button> enemyButtons = new List<Button>();

    private void Awake()
    {
        startButton.onClick.AddListener(StartArena);

        startButton.interactable = false;

        Refresh();
    }

    private void Refresh()
    {
        foreach (var enemy in enemies)
        {
            CreateButton(enemy);
        }
        if (enemies.Length > 0)
        {
            SelectEnemy(enemies[0]);
        }
    }
    private void CreateButton(MechArenaEnemyData enemy)
    {
        GameObject obj = Instantiate(buttonPrefab, content);

        Text text = obj.GetComponentInChildren<Text>();

        text.text = enemy.enemyName;

        Button button = obj.GetComponent<Button>();

        enemyButtons.Add(button);

        button.onClick.AddListener(() => SelectEnemy(enemy));
    }

    public void SelectFirstEnemy()
    {
        if(enemies.Length == 0)
            return;

        SelectEnemy(enemies[0]);

        if(enemyButtons.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(enemyButtons[0].gameObject);
        }
    }

    private void SelectEnemy(MechArenaEnemyData enemy)
    {
        selectedEnemy = enemy;

        enemyNameText.text = enemy.enemyName;

        descriptionText.text = enemy.description;

        rewardText.text = enemy.reward.ToString();

        startButton.interactable = true;
    }
    public void StartArena()
    {
        if(selectedEnemy == null)
            return;

        MechGameManager.Instance.StartMissionResult();

        MechGameManager.Instance.CurrentArenaEnemy = selectedEnemy;

        SceneManager.LoadScene("ArenaScene");
    }
}