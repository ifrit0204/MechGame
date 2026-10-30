using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechJumpAction : MonoBehaviour
{
    private PlayerInput mechInputAction;
    [SerializeField] private MechControl mech;
    public bool getJump, isJump, hasJumped, boostJumping, boostJump, hasBoosted, hasBoostJumped, boostRecharge, isFall, isGrounded;
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!getJump && !mech.hasJumped && !mech.isFall && mech.isGrounded)
            {
                getJump = isJump = true;
                mech.isJump = isJump;
                // mech.anim.SetTrigger("Jump");
                // mech.isGrounded = false;
            }
            else if (mech.getJump && (mech.hasJumped || mech.isFall) && !mech.boostRecharge)
            {
                // isGrounded or isGrounded!
                if (mech.jumpTime <= 0) isJump = false;
                mech.isJump = isJump;
                if ((!mech.isJump && mech.jumpTime <= 0) || (mech.isFall && !mech.hasBoostJumped && !mech.boostRecharge)) boostJump = boostRecharge = hasBoostJumped = true;
                mech.boostJump = boostJump;
                mech.hasBoostJumped = hasBoostJumped;
                mech.boostRecharge = boostRecharge;
            }
            boostJumping = true;
            mech.boostJumping = boostJumping;
        }
        else if (context.canceled)
        {
            if (mech.jumpTime <= 0)
            {
                getJump = isJump = false;
                mech.isJump = isJump;
            }
            if (mech.boostJumpTime <= 0)
            {
                getJump = boostJump = hasBoostJumped = false;
                mech.boostJump = boostJump;
                mech.hasBoostJumped = hasBoostJumped;
            }
            if (!mech.isGrounded)
            {
                isFall = hasJumped = true;
            }
            boostJumping = false;
            mech.boostJumping = boostJumping;
            // if (!mech.isGrounded) isFall = true;
            // mech.isFall = isFall;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        getJump = boostJump = false;

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
