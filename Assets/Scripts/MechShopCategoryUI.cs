using System;
using UnityEngine;
using UnityEngine.UI;

public class MechShopCategoryUI : MonoBehaviour
{
    [Serializable]
    public class CategoryButton
    {
        public PartType partType;
        public Button button;
        public MechShopCategoryButtonUI categoryUI;
        // public Image background;
        public Text label;
    }

    [SerializeField] private MechShopUI shopUI;

    [Header("Part List")]
    [SerializeField] private MechShopPartListUI partListUI;

    [Header("Category Name")]
    [SerializeField] private Text categoryText;

    [Header("Categories")]
    [SerializeField] private CategoryButton[] categories;

    [SerializeField] private MechGarageMenuUI garageMenu;

    private PartType currentPartType;

    private readonly Color normalColor = new Color(1f, 1f, 1f, 0f);

    private readonly Color selectedColor = new Color(1f, 1f, 1f, 0.4f);

    // private void Start()
    // {
    //     foreach (CategoryButton category in categories)
    //     {
    //         PartType type = category.partType;

    //         category.button.onClick.AddListener(
    //             () => SelectCategory(type)
    //         );
    //     }

    //     SelectCategory(PartType.Head);
    // }
    // private void Start()
    // {
    //     foreach (CategoryButton category in categories)
    //     {
    //         category.label.text = GetPartName(category.partType);

    //         PartType type = category.partType;

    //         category.button.onClick.AddListener(() => SelectCategory(type));
    //     }

    //     // SelectCategory(PartType.Head);
    // }
    private void Start()
    {
        foreach(CategoryButton category in categories)
        {
            category.label.text = GetPartName(category.partType);

            PartType type = category.partType;

            category.button.onClick.AddListener(() => SelectCategory(type));
        }

        categoryText.text = "";
    }

    // private void SelectCategory(PartType type)
    // {
    //     currentPartType = type;

    //     // partListUI.ShowParts(type);
    //     // partListUI.ShowParts(type, ShopMode.Buy);
    //     partListUI.ShowParts(type, shopUI.CurrentMode);

    //     categoryText.text = GetPartName(type);

    //     RefreshHighlight();

    //     garageMenu.ChangeState(GarageState.ShopPartList);

    //     // shopUI.HideCategoryPanel();
    // }
    private void SelectCategory(PartType type)
    {
        currentPartType = type;

        garageMenu.ChangeState(GarageState.ShopPartList);

        partListUI.ShowParts(type, shopUI.CurrentMode);

        categoryText.text = GetPartName(type);

        RefreshHighlight();
    }

    private void RefreshHighlight()
    {
        foreach(CategoryButton category in categories)
        {
            category.categoryUI.SetSelected(category.partType == currentPartType);
        }
    }

    private string GetPartName(PartType type)
    {
        switch (type)
        {
            case PartType.Head: return "HEAD";
            case PartType.Body: return "BODY";
            case PartType.Arms: return "ARMS";
            case PartType.Legs: return "LEGS";

            case PartType.RightWeapon: return "R WEAPON";
            case PartType.LeftWeapon: return "L WEAPON";

            case PartType.RightShoulder: return "R SHOULDER";
            case PartType.LeftShoulder: return "L SHOULDER";

            case PartType.Booster: return "BOOSTER";
            case PartType.FCS: return "FCS";
            case PartType.Generator: return "GENERATOR";
        }

        return "";
    }
    public Button GetHeadButton()
    {
        foreach(CategoryButton category in categories)
        {
            if(category.partType == PartType.Head)
            {
                return category.button;
            }
        }

        return null;
    }
}
