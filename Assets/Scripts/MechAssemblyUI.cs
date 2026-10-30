using UnityEngine;
using UnityEngine.UI;

public class MechAssemblyUI : MonoBehaviour
{
    [Header("Garage")]
    [SerializeField] private MechGarageSpawner garageSpawner;
    [SerializeField] private MechGarageMenuUI garageMenu;


    [Header("References")]
    [SerializeField] private MechPartListUI partListUI;

    [SerializeField] private MechAssemblyCurrentUI currentUI;

    [SerializeField] private MechStatsUI statsUI;


    [Header("Buttons")]
    [SerializeField] public Button headButton;
    [SerializeField] public Button bodyButton;
    [SerializeField] public Button armsButton;
    [SerializeField] public Button legsButton;

    [SerializeField] public Button rightWeaponButton;
    [SerializeField] public Button leftWeaponButton;

    [SerializeField] public Button rightShoulderButton;
    [SerializeField] public Button leftShoulderButton;

    [SerializeField] public Button boosterButton;
    [SerializeField] public Button fcsButton;
    [SerializeField] public Button generatorButton;


    private MechEquipment equipment;


    private void Start()
    {
        headButton.onClick.AddListener(() => OpenPartList(PartType.Head));

        bodyButton.onClick.AddListener(() => OpenPartList(PartType.Body));

        armsButton.onClick.AddListener(() => OpenPartList(PartType.Arms));

        legsButton.onClick.AddListener(() => OpenPartList(PartType.Legs));

        rightWeaponButton.onClick.AddListener(() => OpenPartList(PartType.RightWeapon));

        leftWeaponButton.onClick.AddListener(() => OpenPartList(PartType.LeftWeapon));

        rightShoulderButton.onClick.AddListener(() => OpenPartList(PartType.RightShoulder));

        leftShoulderButton.onClick.AddListener(() => OpenPartList(PartType.LeftShoulder));

        boosterButton.onClick.AddListener(() => OpenPartList(PartType.Booster));

        fcsButton.onClick.AddListener(() => OpenPartList(PartType.FCS));

        generatorButton.onClick.AddListener(() => OpenPartList(PartType.Generator));
    }
    public void InitializeAssembly()
    {
        if (garageSpawner == null)
            return;

        if (garageSpawner.SpawnedEquipment == null)
            return;

        equipment = garageSpawner.SpawnedEquipment;

        partListUI.SetEquipment(equipment);

        currentUI.SetEquipment(equipment);

        statsUI.SetEquipment(equipment);

        currentUI.Refresh();

        statsUI.Refresh();
    }


    // private void Update()
    // {
    //     // SpawnされたMechを取得
    //     if (equipment == null &&
    //         garageSpawner != null &&
    //         garageSpawner.SpawnedEquipment != null)
    //     {
    //         equipment = garageSpawner.SpawnedEquipment;

    //         partListUI.SetEquipment(equipment);

    //         currentUI.SetEquipment(equipment);

    //         statsUI.SetEquipment(equipment);

    //         currentUI.Refresh();

    //         statsUI.Refresh();
    //     }
    // }


    // private void OpenPartList(PartType partType)
    // {
    //     Debug.Log("OpenPartList : " + partType);
    //     if (equipment == null)
    //     {
    //         Debug.LogWarning("MechEquipmentがまだ生成されていません。");

    //         return;
    //     }
    //     partListUI.HideParts();
    //     partListUI.ShowParts(partType);
    // }
    // private void OpenPartList(PartType partType)
    // {
    //     if (equipment == null)
    //         return;

    //     partListUI.ShowParts(partType);

    //     garageMenu.ChangeState(GarageState.PartList);
    // }
    // private void OpenPartList(PartType partType)
    // {
    //     Debug.Log("equipment = " + equipment);
    //     Debug.Log("partListUI = " + partListUI);
    //     Debug.Log("garageMenu = " + garageMenu);
    //     if (equipment == null)
    //         return;

    //     partListUI.ShowParts(partType);

    //     garageMenu.ChangeState(GarageState.PartList);
    // }
    private void OpenPartList(PartType partType)
    {
        if (equipment == null)
            return;

        garageMenu.ChangeState(GarageState.PartList);

        partListUI.ShowParts(partType);
    }
    public Button GetButton(PartType type)
    {
        switch(type)
        {
            case PartType.Head:
                return headButton;

            case PartType.Body:
                return bodyButton;

            case PartType.Arms:
                return armsButton;

            case PartType.Legs:
                return legsButton;

            case PartType.RightWeapon:
                return rightWeaponButton;

            case PartType.LeftWeapon:
                return leftWeaponButton;

            case PartType.RightShoulder:
                return rightShoulderButton;

            case PartType.LeftShoulder:
                return leftShoulderButton;

            case PartType.Booster:
                return boosterButton;

            case PartType.FCS:
                return fcsButton;

            case PartType.Generator:
                return generatorButton;
        }

        return headButton;
    }
}
