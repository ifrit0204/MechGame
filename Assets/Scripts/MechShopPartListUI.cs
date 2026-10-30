using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MechShopPartListUI : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private MechPartDatabase database;

    // [Header("Player")]
    // [SerializeField] private MechPlayerData playerData;

    [Header("UI")]
    [SerializeField] private Transform content;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private MechPartInfoUI infoUI;
    [SerializeField] private MechStatsUI statsUI;

    private PartType currentType;
    private ShopMode currentMode;

    public MechPartData SelectedPart { get; private set; }
    private bool hasSelectedCategory;
    public PartType CurrentType
    {
        get { return currentType; }
    }
    private void Start()
    {
        Debug.Log("infoUI = " + infoUI);
    }

    public void ShowParts(PartType type, ShopMode mode)
    {
        Debug.Log("PartList Active = " + gameObject.activeInHierarchy);

        hasSelectedCategory = true;

        currentType = type;
        currentMode = mode;

        ClearList();

        MechPartData[] parts = database.GetParts(type);

        if(parts == null)
            return;

        foreach(MechPartData part in parts)
        {
            if(mode == ShopMode.Buy)
            {
                // 購入モード
                // 所持済みは表示しない
                if(MechGameManager.Instance.IsOwned(part))
                    continue;
            }
            else
            {
                // 売却モード
                // 所持していないものは表示しない
                if(!MechGameManager.Instance.IsOwned(part))
                    continue;

                if(MechGameManager.Instance.IsEquipped(part))
                    continue;
            }

            CreateButton(part);
        }
        StartCoroutine(SelectFirstButtonNextFrame());
        // SelectFirstButton();
    }
    public Action<MechPartData> OnPartSelected;
    private IEnumerator SelectFirstButtonNextFrame()
    {
        yield return null;

        if(content.childCount == 0)
            yield break;

        Button firstButton = content.GetChild(0).GetComponent<Button>();

        if(firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }
    }

    private void CreateButton(MechPartData part)
    {
        GameObject obj = Instantiate(buttonPrefab, content);

        MechPartButtonUI ui = obj.GetComponent<MechPartButtonUI>();

        if(ui != null)
        {
            ui.SetPart(part);
            // ui.Initialize(part, infoUI);
            ui.Initialize(part, currentType, infoUI, statsUI);
        }

        Button button = obj.GetComponent<Button>();

        if(button != null)
        {
            button.onClick.AddListener(() => SelectPart(part));
        }
        Debug.Log("Button Found = " + (button != null));
        Debug.Log("InitializeInfoUI = " + infoUI);
    }

    // private void SelectPart(MechPartData part)
    // {
    //     SelectedPart = part;

    //     OnPartSelected?.Invoke(part);
    // }
    private void SelectPart(MechPartData part)
    {
        SelectedPart = part;

        if(infoUI != null)
        {
            infoUI.ShowPart(part);
        }

        OnPartSelected?.Invoke(part);
    }
    // private void SelectPart(MechPartData part)
    // {
    //     Debug.Log("SelectPart : " + part.partName);
    //     SelectedPart = part;

    //     if(infoUI != null)
    //     {
    //         infoUI.ShowPart(part);
    //     }

    //     Debug.Log("選択 : " + part.partName);
    // }

    // public void Refresh()
    // {
    //     ShowParts(currentType);
    // }
    // public void Refresh()
    // {
    //     ShowParts(currentType, currentMode);
    // }
    public void Refresh()
    {
        ClearSelection();

        if(!hasSelectedCategory)
            return;

        ShowParts(currentType, currentMode);
    }

    private void ClearList()
    {
        for(int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
    public void SetMode(ShopMode mode)
    {
        currentMode = mode;
    }
    public void ClearSelection()
    {
        SelectedPart = null;

        if(infoUI != null)
        {
            infoUI.Clear();
        }
    }
    private void SelectFirstButton()
    {
        if(content.childCount == 0)
            return;

        Button firstButton = content.GetChild(0).GetComponent<Button>();

        if(firstButton == null)
            return;

        EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
    }
}