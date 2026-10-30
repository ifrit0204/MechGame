using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MechGarageMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject garageMenuPanel;
    [SerializeField] private GameObject assemblyPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject missionPanel;
    [SerializeField] private GameObject categoryPanel;
    [SerializeField] private GameObject shopCategoryPanel;
    [SerializeField] private GameObject shopPartListPanel;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject arenaPanel;

    [Header("Buttons")]
    [SerializeField] private Button assemblyButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button missionButton;
    [SerializeField] private Button arenaButton;

    [Header("UI")]
    [SerializeField] private MechAssemblyUI assemblyUI;
    [SerializeField] private MechPartListUI partListUI;
    [SerializeField] private MechShopUI shopUI;
    [SerializeField] private MechMissionSelectUI missionUI;
    [SerializeField] private MechArenaSelectUI arenaUI;

    private Stack<GarageState> stateHistory = new Stack<GarageState>();

    public GarageState CurrentState
    {
        get;
        private set;
    }

    private void Start()
    {
        assemblyButton.onClick.AddListener(OpenAssembly);
        shopButton.onClick.AddListener(OpenShop);
        missionButton.onClick.AddListener(OpenMission);
        arenaButton.onClick.AddListener(OpenArena);

        ChangeState(GarageState.MainMenu, false);

        EventSystem.current.SetSelectedGameObject(assemblyButton.gameObject);
    }
    // public void ClearHistory()
    // {
    //     stateHistory.Clear();
    // }
    // public void RemovePartListHistory()
    // {
    //     if(stateHistory.Count == 0)
    //         return;

    //     if(stateHistory.Peek() == GarageState.PartList)
    //     {
    //         stateHistory.Pop();
    //     }
    // }
    public void RemoveLastHistory()
    {
        if(stateHistory.Count > 0)
        {
            stateHistory.Pop();
        }
    }

    //==================================
    // State変更
    //==================================

    public void ChangeState(GarageState newState, bool saveHistory = true)
    {
        if(saveHistory)
        {
            stateHistory.Push(CurrentState);
        }

        CurrentState = newState;

        RefreshPanels();
    }

    //==================================
    // 戻る
    //==================================

    // public void GoBack()
    // {
    //     if(stateHistory.Count == 0)
    //         return;

    //     if(CurrentState == GarageState.PartList)
    //     {
    //         FindFirstObjectByType<MechPartListUI>().ClosePartList();
    //     }

    //     CurrentState = stateHistory.Pop();

    //     RefreshPanels();
    // }
    public void GoBack()
    {
        Debug.Log($"CurrentState = {CurrentState}");

        if(stateHistory.Count == 0)
            return;

        // if(CurrentState == GarageState.PartList)
        // {
        //     var partList = FindFirstObjectByType<MechPartListUI>();

        //     Debug.Log($"partList = {partList}");

        //     partList.ClosePartList();
        // }
        if(CurrentState == GarageState.PartList)
        {
            partListUI.ClosePartList();
        }
        if(CurrentState == GarageState.ShopPartList)
        {
            shopPartListPanel.SetActive(false);
        }

        CurrentState = stateHistory.Pop();

        RefreshPanels();
        switch (CurrentState)
        {
            case GarageState.MainMenu:
                EventSystem.current.SetSelectedGameObject(assemblyButton.gameObject);
                break;

            // case GarageState.Assembly:
            //     EventSystem.current.SetSelectedGameObject(categoryButton.gameObject);
            //     break;
        }
    }

    //==================================
    // パネル更新
    //==================================

    private void RefreshPanels()
    {
        garageMenuPanel.SetActive(false);
        assemblyPanel.SetActive(false);
        shopPanel.SetActive(false);
        missionPanel.SetActive(false);
        arenaPanel.SetActive(false);
        categoryPanel.SetActive(false);
        shopCategoryPanel.SetActive(false);
        shopPartListPanel.SetActive(false);
        statsPanel.SetActive(false);

        switch(CurrentState)
        {
            case GarageState.MainMenu:

                garageMenuPanel.SetActive(true);

                break;

            case GarageState.Assembly:

                assemblyPanel.SetActive(true);

                categoryPanel.SetActive(true);

                statsPanel.SetActive(true);

                break;

            case GarageState.Shop:

                shopPanel.SetActive(true);
                // shopCategoryPanel.SetActive(true);

                break;

            case GarageState.ShopCategory:

                // shopPanel.SetActive(true);

                shopCategoryPanel.SetActive(true);

                statsPanel.SetActive(true);

                break;

            case GarageState.ShopPartList:

                // shopPanel.SetActive(true);
                shopPartListPanel.SetActive(true);

                statsPanel.SetActive(true);

                break;

            case GarageState.Mission:

                missionPanel.SetActive(true);

                break;

            case GarageState.Arena:

                arenaPanel.SetActive(true);

                break;

            case GarageState.PartList:

                assemblyPanel.SetActive(true);

                categoryPanel.SetActive(true);

                statsPanel.SetActive(true);

                break;
        }
    }
    // public void GoBack()
    // {
    //     if(stateHistory.Count == 0)
    //         return;

    //     if(CurrentState == GarageState.PartList)
    //     {
    //         FindFirstObjectByType<MechPartListUI>().ClosePartList();
    //     }

    //     CurrentState = stateHistory.Pop();

    //     RefreshPanels();
    // }

    //==================================
    // ボタン
    //==================================

    public void OpenAssembly()
    {
        assemblyUI.InitializeAssembly();
        ChangeState(GarageState.Assembly);
        EventSystem.current.SetSelectedGameObject(assemblyUI.headButton.gameObject);
    }

    public void OpenShop()
    {
        ChangeState(GarageState.Shop);
        EventSystem.current.SetSelectedGameObject(shopUI.buyButton.gameObject);
    }

    public void OpenMission()
    {
        ChangeState(GarageState.Mission);
        // EventSystem.current.SetSelectedGameObject(missionUI.startButton.gameObject);
    }
    public void OpenArena()
    {
        ChangeState(GarageState.Arena);
        arenaUI.SelectFirstEnemy();
        // EventSystem.current.SetSelectedGameObject(arenaUI.startButton.gameObject);
    }
}