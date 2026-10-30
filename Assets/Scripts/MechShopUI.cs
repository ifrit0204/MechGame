using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MechShopUI : MonoBehaviour
{
    [SerializeField] private MechShopPartListUI partListUI;
    [SerializeField] private MechShopCategoryUI categoryUI;

    [SerializeField] private GameObject shopCategoryPanel;
    [SerializeField] private GameObject shopPanel;

    [SerializeField] public Button buyButton;
    [SerializeField] public Button sellButton;

    // [SerializeField] private Text creditsText;
    [SerializeField] private Text buyButtonText;

    private ShopMode currentMode;

    [SerializeField] private Button buyModeButton;
    [SerializeField] private Button sellModeButton;

    [SerializeField] private ShopConfirmDialogUI confirmDialog;
    [SerializeField] private MechGarageMenuUI garageMenu;

    private void Start()
    {
        buyModeButton.onClick.AddListener(OpenBuyMode);
        sellModeButton.onClick.AddListener(OpenSellMode);

        buyButton.onClick.AddListener(ExecuteBuy);
        sellButton.onClick.AddListener(ExecuteSell);

        // OpenBuyMode();

        // RefreshCredits();

        partListUI.OnPartSelected += OnPartSelected;
    }
    private void OnPartSelected(MechPartData part)
    {
        if(currentMode == ShopMode.Buy)
        {
            ShowBuyConfirm(part);
        }
        else
        {
            ShowSellConfirm(part);
        }
    }
    private void ShowBuyConfirm(MechPartData part)
    {
        confirmDialog.Show(
            $"【購入確認】\n{part.partName}\n{part.price} C\n購入しますか？",
            () =>
            {
                bool success =
                    MechGameManager.Instance.BuyPart(part);

                if(success)
                {
                    // RefreshCredits();
                    partListUI.Refresh();
                }
            });
    }
    private void ShowSellConfirm(MechPartData part)
    {
        confirmDialog.Show(
            $"【売却確認】\n{part.partName}\n{part.price} C\n売却しますか？",
            () =>
            {
                bool success =
                    MechGameManager.Instance.SellPart(part);

                if(success)
                {
                    // RefreshCredits();
                    partListUI.Refresh();
                }
            });
    }
    
    public void HideCategoryPanel()
    {
        shopCategoryPanel.SetActive(false);
    }

    public void ShowCategoryPanel()
    {
        shopCategoryPanel.SetActive(true);
    }

    private void ExecuteBuy()
    {
        MechPartData part =
            partListUI.SelectedPart;

        if(part == null)
            return;

        confirmDialog.Show(
            $"【購入確認】\n{part.partName}\n{part.price} C\n購入しますか？",
            () =>
            {
                bool success =
                    MechGameManager.Instance
                    .BuyPart(part);

                if(success)
                {
                    // RefreshCredits();

                    partListUI.Refresh();
                }
            });
    }
    private void ExecuteSell()
    {
        MechPartData part =
            partListUI.SelectedPart;

        if(part == null)
            return;

        confirmDialog.Show(
            $"【売却確認】\n{part.partName}\n{part.price} C\n売却しますか？",
            () =>
            {
                bool success =
                    MechGameManager.Instance
                    .SellPart(part);

                if(success)
                {
                    // RefreshCredits();

                    partListUI.Refresh();
                }
            });
    }

    private void ExecuteShop()
    {
        Debug.Log("ExecuteShop");
        Debug.Log(
        "SelectedPart = " +
        (partListUI.SelectedPart != null ? partListUI.SelectedPart.partName : "NULL"));
        MechPartData part = partListUI.SelectedPart;

        if(part == null)
            return;

        bool success = false;

        if(currentMode == ShopMode.Buy)
        {
            success = MechGameManager.Instance.BuyPart(part);
        }
        else
        {
            success = MechGameManager.Instance.SellPart(part);
        }

        if(success)
        {
            // RefreshCredits();

            partListUI.Refresh();
        }
    }

    // private void RefreshCredits()
    // {
    //     creditsText.text = "CREDITS : " + MechGameManager.Instance.Credits;
    //         // playerData.credits;
    // }
    public void OpenBuyMode()
    {
        Debug.Log("Buy Click");
        
        currentMode = ShopMode.Buy;

        partListUI.SetMode(currentMode);

        garageMenu.ChangeState(GarageState.ShopCategory);

        Button headButton = categoryUI.GetHeadButton();

        if (headButton != null)
        {
            EventSystem.current.SetSelectedGameObject(headButton.gameObject);
        }

        // buyButton.gameObject.SetActive(true);
        // sellButton.gameObject.SetActive(false);
    }
    public void OpenSellMode()
    {
        Debug.Log("Sell Click");

        currentMode = ShopMode.Sell;

        partListUI.SetMode(currentMode);

        garageMenu.ChangeState(GarageState.ShopCategory);

        Button headButton = categoryUI.GetHeadButton();

        if(headButton != null)
        {
            EventSystem.current.SetSelectedGameObject(headButton.gameObject);
        }

        // buyButton.gameObject.SetActive(false);
        // sellButton.gameObject.SetActive(true);
    }
    public ShopMode CurrentMode
    {
        get { return currentMode; }
    }
    // public bool BuyPart(MechPartData part)
    // {
    //     if(Credits < part.price)
    //         return false;

    //     Credits -= part.price;

    //     OwnedParts.Add(part);

    //     if(mode == ShopMode.Buy)
    //     {
    //         if(MechGameManager.Instance.IsOwned(part))
    //             continue;
    //     }

    //     return true;
    // }
    // public bool SellPart(MechPartData part)
    // {
    //     if(!OwnedParts.Contains(part))
    //         return false;

    //     Credits += part.price;

    //     OwnedParts.Remove(part);

    //     if (mode == ShopMode.Sell)
    //     {
    //         if (!MechGameManager.Instance.IsOwned(part))
    //         continue;
    //     }

    //     return true;
    // }
    // public bool IsEquipped(MechPartData part)
    // {
    //     var loadout = CurrentLoadout;

    //     return
    //         loadout.Head == part ||
    //         loadout.Body == part ||
    //         loadout.Arms == part ||
    //         loadout.Legs == part ||

    //         loadout.RightWeapon == part ||
    //         loadout.LeftWeapon == part ||

    //         loadout.RightShoulder == part ||
    //         loadout.LeftShoulder == part ||

    //         loadout.Booster == part ||
    //         loadout.FCS == part ||
    //         loadout.Generator == part;
    // }
}
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class MechShopUI : MonoBehaviour
// {
//     [SerializeField] private MechPartDatabase database;

//     [SerializeField] private Transform content;

//     [SerializeField] private GameObject buttonPrefab;

//     [SerializeField] private MechPlayerData playerData;
//     [SerializeField] private Button buyButton;
//     private MechPartData selectedPart;
//     private MechPartInfoUI infoUI;
//     [SerializeField] private Button headButton;
//     [SerializeField] private Button bodyButton;
//     [SerializeField] private Button armsButton;
//     [SerializeField] private Button legsButton;

//     [SerializeField] private Button rightWeaponButton;
//     [SerializeField] private Button leftWeaponButton;

//     [SerializeField] private Button rightShoulderButton;
//     [SerializeField] private Button leftShoulderButton;

//     [SerializeField] private Button boosterButton;
//     [SerializeField] private Button fcsButton;
//     [SerializeField] private Button generatorButton;

//     private PartType currentType;

//     private void SelectPart(MechPartData part)
//     {
//         selectedPart = part;

//         infoUI.ShowPart(part);
//     }
//     private void ClearList()
//     {
//         for (int i = content.childCount - 1; i >= 0; i--)
//         {
//             Destroy(content.GetChild(i).gameObject);
//         }
//     }
//     private void Start()
//     {
//         buyButton.onClick.AddListener(BuySelectedPart);

//         headButton.onClick.AddListener(() => ShowShopParts(PartType.Head));

//         bodyButton.onClick.AddListener(() => ShowShopParts(PartType.Body));
//     }
//     public void ShowShopParts(PartType type)
//     {
//         ClearList();

//         MechPartData[] parts = database.GetParts(type);

//         foreach(var part in parts)
//         {
//             if(playerData.IsOwned(part))
//                 continue;

//             CreateShopButton(part);
//         }
//     }
//     private void CreateShopButton(MechPartData part)
//     {
//         GameObject obj = Instantiate(buttonPrefab, content);

//         MechPartButtonUI ui = obj.GetComponent<MechPartButtonUI>();

//         ui.SetPart(part);

//         Button button = obj.GetComponent<Button>();

//         button.onClick.AddListener(() => SelectPart(part));
//     }
//     private void BuySelectedPart()
//     {
//         if(selectedPart == null)
//             return;

//         bool success = playerData.BuyPart(selectedPart, selectedPart.price);

//         if(success)
//         {
//             Debug.Log("購入成功");

//             ShowShopParts(currentType);
//         }
//         else
//         {
//             Debug.Log("お金が足りません");
//         }
//     }
// }
