using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechHyperBoostAction : MonoBehaviour
{
    private PlayerInput mechInputAction;
    [SerializeField] private MechControl mech;
    public bool getHyperBoost, hasHyperBoosted, isHyperBoost, isHyperBoostReady, boostRecharge;
    public void OnHyperBoost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!getHyperBoost && mech.boostEnergy > 0)
            {
                getHyperBoost = isHyperBoost = true;
                mech.getHyperBoost = getHyperBoost;
                mech.isHyperBoost = isHyperBoost;
            }
            else if (mech.getHyperBoost && !mech.boostRecharge)
            {
                isHyperBoost = boostRecharge = true;
                mech.isHyperBoost = isHyperBoost;
                mech.boostRecharge = boostRecharge;
            }
        }
        else if (context.canceled)
        {
            if ((mech.getHyperBoost || mech.isHyperBoost) && mech.boostRecharge)
            {
                getHyperBoost = isHyperBoost = boostRecharge = false;
                mech.getHyperBoost = getHyperBoost;
                mech.isHyperBoost = isHyperBoost;
                mech.boostRecharge = boostRecharge;
                hasHyperBoosted = true;
                mech.hasHyperBoosted = hasHyperBoosted;
            }
        }
    }
    void Start()
    {
        getHyperBoost = false;

        mechInputAction = GetComponent<PlayerInput>();

        if (mechInputAction != null)
        {
            // 両方のアクションマップを有効化
            var keyboardMap = mechInputAction.actions.FindActionMap("Keyboard");
            var xboxMap = mechInputAction.actions.FindActionMap("XboxControl");

            if (keyboardMap != null) keyboardMap.Enable();
            if (xboxMap != null) xboxMap.Enable();
        }
    }
}
