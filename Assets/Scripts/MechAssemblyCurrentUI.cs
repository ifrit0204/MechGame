using UnityEngine;

public class MechAssemblyCurrentUI : MonoBehaviour
{
    [Header("Equipment")]
    [SerializeField]
    private MechEquipment equipment;


    [Header("Current Parts")]
    [SerializeField] private MechPartButtonUI headUI;
    [SerializeField] private MechPartButtonUI bodyUI;
    [SerializeField] private MechPartButtonUI armsUI;
    [SerializeField] private MechPartButtonUI legsUI;

    [SerializeField] private MechPartButtonUI rightWeaponUI;
    [SerializeField] private MechPartButtonUI leftWeaponUI;

    [SerializeField] private MechPartButtonUI rightShoulderUI;
    [SerializeField] private MechPartButtonUI leftShoulderUI;

    [SerializeField] private MechPartButtonUI boosterUI;
    [SerializeField] private MechPartButtonUI fcsUI;
    [SerializeField] private MechPartButtonUI generatorUI;


    public void SetEquipment(MechEquipment equipment)
    {
        this.equipment = equipment;
    }


    public void Refresh()
    {
        Debug.Log("CurrentUI Refresh");
        if (equipment == null)
        {
            return;
        }


        SetPart(
            headUI,
            equipment.head
        );

        SetPart(
            bodyUI,
            equipment.body
        );

        SetPart(
            armsUI,
            equipment.arms
        );

        SetPart(
            legsUI,
            equipment.legs
        );


        SetPart(
            rightWeaponUI,
            equipment.rightWeapon
        );

        SetPart(
            leftWeaponUI,
            equipment.leftWeapon
        );


        SetPart(
            rightShoulderUI,
            equipment.rightShoulder
        );

        SetPart(
            leftShoulderUI,
            equipment.leftShoulder
        );


        SetPart(
            boosterUI,
            equipment.booster
        );

        SetPart(
            fcsUI,
            equipment.FCS
        );

        SetPart(
            generatorUI,
            equipment.generator
        );
        Debug.Log(equipment.head);
        Debug.Log(equipment.body);
        Debug.Log(equipment.arms);
        Debug.Log(equipment.legs);
    }


    private void SetPart(MechPartButtonUI ui, MechPartData part)
    {
        if (ui == null)
        {
            return;
        }

        if (part == null)
        {
            ui.Clear();
            return;
        }

        ui.SetPart(part);
        ui.SetEquipped(false);
    }
}
