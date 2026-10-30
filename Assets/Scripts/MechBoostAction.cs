using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechBoostAction : MonoBehaviour
{
    private PlayerInput mechInputAction;
    [SerializeField] private MechControl mech;
    public bool boosting, getBoost, isBoost, hasBoosted, boostRecharge;
    public void OnBoost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!getBoost)
            {
                getBoost = isBoost = true;
                mech.getBoost = getBoost;
                mech.isBoost = isBoost;
            }
            else if (mech.getBoost && !mech.boostRecharge)
            {
                if (mech.boostTime > 0) isBoost = true;
                // mech.boostRecharge = boostRecharge;
                mech.isBoost = isBoost;
                // if (!mech.boostRecharge) isBoost = true;
                // mech.isBoost = isBoost;
            }
            boosting = true;
            mech.boosting = boosting;
        }
        else if (context.canceled)
        {
            if ((mech.getBoost || mech.isBoost) && mech.hasBoosted)
            {
                getBoost = isBoost = false;
                mech.getBoost = getBoost;
                mech.isBoost = isBoost;
                if (!mech.isGrounded) hasBoosted = true;
                mech.hasBoosted = hasBoosted;
                mech.boostTime = mech.boostTimer;
                mech.boostRechargeTime = mech.boostRechargeTimer;
            }
            else if (!mech.isBoost)
            {
                mech.boostTime = mech.boostTimer;
                mech.boostRechargeTime = mech.boostRechargeTimer;
            }
            boosting = false;
            mech.boosting = boosting;
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        getBoost = false;

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
