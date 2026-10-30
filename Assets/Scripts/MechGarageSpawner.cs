using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MechGarageSpawner : MonoBehaviour
{
    // public MechEquipment SpawnedEquipment
    // {
    //     get;
    //     private set;
    // }

    // public Action<MechEquipment> OnSpawned;
    [Header("UI")]
    [SerializeField] private MechGarageMenuUI garageMenu;
    [SerializeField] private MechPartListUI partListUI;
    // [SerializeField] private Text guideText;
    public GameObject mechPrefab;
    public MechEquipment SpawnedEquipment { get; private set; }
    void Start()
    {
        GameObject mech = Instantiate(mechPrefab, transform.position, transform.rotation);

        MechEquipment equipment = mech.GetComponent<MechEquipment>();

        SpawnedEquipment = equipment;

        equipment.ApplyLoadout(MechGameManager.Instance.CurrentLoadout);

        DisableMechControl(mech);
    }
    void DisableMechControl(GameObject mech)
    {
        MechControl control = mech.GetComponent<MechControl>();
        if(control != null) control.enabled = false;

        MechJumpAction jump = mech.GetComponent<MechJumpAction>();
        if(jump != null) jump.enabled = false;

        MechBoostAction boost = mech.GetComponent<MechBoostAction>();
        if(boost != null) boost.enabled = false;

        MechHyperBoostAction hyper = mech.GetComponent<MechHyperBoostAction>();
        if(hyper != null) hyper.enabled = false;

        MechAttackAction attack = mech.GetComponent<MechAttackAction>();
        if(attack != null) attack.enabled = false;
    }
}
