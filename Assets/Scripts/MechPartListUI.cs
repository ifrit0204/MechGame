using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MechPartListUI : MonoBehaviour
{
    [Header("Equipment")]
    [SerializeField] private MechEquipment equipment;

    [Header("List")]
    [SerializeField] private Transform content;

    [SerializeField] private GameObject buttonPrefab;

    [Header("Current Equipment")]
    [SerializeField] private MechPartButtonUI currentPartUI;
    [SerializeField] private MechAssemblyUI assemblyUI;


    private PartType currentPartType;
    private List<MechPartData> displayedParts = new List<MechPartData>();

    [SerializeField] private MechAssemblyCurrentUI currentEquipmentUI;

    [SerializeField] private MechStatsUI statsUI;

    [SerializeField] private MechPartInfoUI infoUI;

    [SerializeField] private MechGarageMenuUI garageMenu;

    [SerializeField] private GameObject partListPanel;

    [SerializeField] private GameObject partInfoPanel;

    
    private void Start()
    {
        partListPanel.SetActive(false);
        partInfoPanel.SetActive(false);
    }
    public bool IsPartListOpen()
    {
        return partListPanel.activeSelf;
    }
    public void ClosePartList()
    {
        HideParts();
    }



    //==================================================
    // パーツ一覧表示
    //==================================================

    public void ShowParts(PartType partType)
    {
        if(content.childCount > 0)
        {
            ClearList();
        }
        HideParts();
        partListPanel.SetActive(true);
        partInfoPanel.SetActive(true);
        infoUI.Show();
        Debug.Log("ShowParts Start");
        if (equipment == null)
        {
            Debug.LogError("MechPartListUI : Equipmentが設定されていません。");

            return;
        }

        currentPartType = partType;

        ClearList();

        // List<MechPartData> displayedParts = new List<MechPartData>();
        displayedParts.Clear();

        foreach(MechPartData part in MechGameManager.Instance.OwnedParts)
        {
            if(GetPartType(part) == partType)
            {
                // displayedParts.Add(part);
                displayedParts.Add(part);
            }
        }

        if(displayedParts.Count == 0)
        {
            Debug.Log("所持パーツなし : " + partType);
            return;
        }

        // MechPartData[] displayedParts = equipment.Database.GetParts(partType);

        // Debug.Log(equipment.Database);

        // Debug.Log(displayedParts.Length);


        // if (displayedParts == null)
        // {
        //     Debug.LogWarning("パーツ一覧がありません : " + partType);

        //     return;
        // }


        // int currentIndex = equipment.GetCurrentIndex(partType);


        // for (int i = 0; i < displayedParts.Length; i++)
        for(int i = 0; i < displayedParts.Count; i++)
        {
            int index = i;

            MechPartData part = displayedParts[i];


            GameObject obj = Instantiate(buttonPrefab, content);

            Debug.Log(obj.name);


            MechPartButtonUI buttonUI = obj.GetComponent<MechPartButtonUI>();


            if (buttonUI != null)
            {
                buttonUI.SetPart(part);

                // buttonUI.Initialize(part, infoUI);
                buttonUI.Initialize(part, currentPartType, infoUI, statsUI);

                // bool isCurrent = index == currentIndex;
                bool isCurrent = equipment.GetCurrentPart(partType) == part;

                buttonUI.SetEquipped(isCurrent);
                buttonUI.SetSelected(isCurrent);
            }


            Button button = obj.GetComponent<Button>();


            if (button != null)
            {
                button.onClick.AddListener(
                    () =>
                    {
                        EquipPart(partType, part);
                    }
                );
            }
        }


        // 現在装備中のパーツ
        ShowCurrentPart(partType);
        if(displayedParts.Count > 0)
        {
            infoUI.ShowPart(displayedParts[0]);
        }
        StartCoroutine(SelectFirstButtonNextFrame());
    }
    public void HideParts()
    {
        ClearList();

        partListPanel.SetActive(false);
        partInfoPanel.SetActive(false);
        infoUI.Hide();

        if(infoUI != null)
        {
            infoUI.Hide();
        }
    }


    //==================================================
    // 装備
    //==================================================

    // private void EquipPart(PartType partType, int index)
    private void EquipPart(PartType partType, MechPartData part)
    {
        equipment.ChangePart(partType, part);


        ShowCurrentPart(partType);

        MechPartData currentPart = equipment.GetCurrentPart(partType);

        if(currentPart != null)
        {
            infoUI.ShowPart(currentPart);
        }


        RefreshEquippedMarks(partType);


        // Assembly画面の現在装備表示更新
        if (currentEquipmentUI != null)
        {
            currentEquipmentUI.Refresh();
        }


        // 能力値更新
        if (statsUI != null)
        {
            statsUI.Refresh();
        }
        HideParts();
        // garageMenu.RemovePartListHistory();
        // garageMenu.ClearHistory();

        // garageMenu.ChangeState(GarageState.Assembly, false);
        // garageMenu.ChangeState(GarageState.Assembly, false);
        garageMenu.RemoveLastHistory();

        garageMenu.ChangeState(GarageState.Assembly, false);

        // EventSystem.current.SetSelectedGameObject(assemblyUI.headButton.gameObject);

        Button targetButton = assemblyUI.GetButton(partType);

        EventSystem.current.SetSelectedGameObject(targetButton.gameObject);
    }



    //==================================================
    // 現在装備表示
    //==================================================

    private void ShowCurrentPart(PartType partType)
    {
        if (currentPartUI == null)
        {
            return;
        }


        MechPartData currentPart = equipment.GetCurrentPart(partType);


        if (currentPart == null)
        {
            currentPartUI.Clear();
            return;
        }


        currentPartUI.SetPart(
            currentPart
        );


        currentPartUI.SetEquipped(
            false
        );
    }


    //==================================================
    // EQUIPPED表示更新
    //==================================================

    // private void RefreshEquippedMarks(PartType partType)
    // {
    //     int currentIndex = equipment.GetCurrentIndex(partType);


    //     for (int i = 0; i < content.childCount; i++)
    //     {
    //         Transform child = content.GetChild(i);


    //         MechPartButtonUI buttonUI = child.GetComponent<MechPartButtonUI>();


    //         if (buttonUI != null)
    //         {
    //             buttonUI.SetEquipped(i == currentIndex);
    //         }
    //     }
    // }
    private void RefreshEquippedMarks(PartType partType)
    {
        MechPartData currentPart = equipment.GetCurrentPart(partType);

        for(int i = 0; i < content.childCount; i++)
        {
            MechPartButtonUI buttonUI = content.GetChild(i).GetComponent<MechPartButtonUI>();

            if(buttonUI == null)
                continue;

            bool equipped = i < displayedParts.Count && displayedParts[i] == currentPart;

            buttonUI.SetEquipped(equipped);
            buttonUI.SetSelected(equipped);
        }
    }


    //==================================================
    // 一覧削除
    //==================================================

    private void ClearList()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
    public void SetEquipment(MechEquipment equipment)
    {
        this.equipment = equipment;
    }
    private PartType GetPartType(MechPartData part)
    {
        if(part is MechHeadPartData)
            return PartType.Head;

        if(part is MechBodyPartData)
            return PartType.Body;

        if(part is MechArmsPartData)
            return PartType.Arms;

        if(part is MechLegsPartData)
            return PartType.Legs;

        if(part is MechBoosterPartData)
            return PartType.Booster;

        if(part is MechFCSPartData)
            return PartType.FCS;

        if(part is MechGeneratorPartData)
            return PartType.Generator;

        if(part is MechWeaponPartData weapon)
        {
            if(equipment.Database.rightWeapons != null &&
            System.Array.Exists(
                equipment.Database.rightWeapons,
                x => x == weapon))
                return PartType.RightWeapon;

            if(equipment.Database.leftWeapons != null &&
            System.Array.Exists(
                equipment.Database.leftWeapons,
                x => x == weapon))
                return PartType.LeftWeapon;

            if(equipment.Database.rightShoulderWeapons != null &&
            System.Array.Exists(
                equipment.Database.rightShoulderWeapons,
                x => x == weapon))
                return PartType.RightShoulder;

            if(equipment.Database.leftShoulderWeapons != null &&
            System.Array.Exists(
                equipment.Database.leftShoulderWeapons,
                x => x == weapon))
                return PartType.LeftShoulder;
        }

        return PartType.Head;
    }
    private IEnumerator SelectFirstButtonNextFrame()
    {
        yield return null;

        if (content.childCount == 0)
            yield break;

        Button firstButton =
            content.GetChild(0)
            .GetComponent<Button>();

        if (firstButton != null)
        {
            EventSystem.current
                .SetSelectedGameObject(
                    firstButton.gameObject
                );
        }
    }
}
