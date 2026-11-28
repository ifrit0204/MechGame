using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechJumpAction : MonoBehaviour
{
    // private PlayerInput mechInputAction;
    // [SerializeField] private MechControl mech;
    // public bool getJump, isJump, hasJumped, isVerticalBoost, boostJumping, boostJump, hasBoosted, hasBoostJumped, boostRecharge, isFall, isGrounded;
    // public void OnJump(InputAction.CallbackContext context)
    // {
    //     if (context.performed)
    //     {
    //         if (!getJump && !mech.hasJumped && !mech.isFall && mech.isGrounded)
    //         {
    //             getJump = isJump = true;
    //             mech.isJump = isJump;
    //             // mech.anim.SetTrigger("Jump");
    //             // mech.isGrounded = false;
    //         }
    //         else if (mech.getJump && (mech.hasJumped || mech.isFall) && !mech.boostRecharge)
    //         {
    //             // isGrounded or isGrounded!
    //             if (mech.jumpTime <= 0) isJump = false;
    //             mech.isJump = isJump;
    //             if ((!mech.isJump && mech.jumpTime <= 0) || (mech.isFall && !mech.hasBoostJumped && !mech.boostRecharge)) boostJump = boostRecharge = hasBoostJumped = true;
    //             mech.boostJump = boostJump;
    //             mech.hasBoostJumped = hasBoostJumped;
    //             mech.boostRecharge = boostRecharge;
    //         }
    //         boostJumping = true;
    //         mech.boostJumping = boostJumping;
    //     }
    //     else if (context.canceled)
    //     {
    //         if (mech.jumpTime <= 0)
    //         {
    //             getJump = isJump = false;
    //             mech.isJump = isJump;
    //         }
    //         if (mech.boostJumpTime <= 0)
    //         {
    //             getJump = boostJump = hasBoostJumped = false;
    //             mech.boostJump = boostJump;
    //             mech.hasBoostJumped = hasBoostJumped;
    //         }
    //         if (!mech.isGrounded)
    //         {
    //             isFall = hasJumped = true;
    //         }
    //         boostJumping = false;
    //         mech.boostJumping = boostJumping;
    //         // if (!mech.isGrounded) isFall = true;
    //         // mech.isFall = isFall;
    //     }
    // }
    private PlayerInput mechInputAction;
    [SerializeField] private MechControl mech;
    [SerializeField] private float doubleTapTime = 0.25f; // 二度押しとみなす間隔(秒)
    private float lastPressTime = -999f;

    public bool getJump, isJump, hasJumped, isVerticalBoost, boostJump, hasBoosted, hasBoostJumped, boostRecharge, isFall, isGrounded;

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && MechPauseMenuUI.IsPaused) return;
        if (context.performed)
        {
            bool isDoublePress = Time.time - lastPressTime <= doubleTapTime;
            lastPressTime = Time.time;

            // 地上 → 通常ジャンプ(従来どおり)
            if (!getJump && !mech.hasJumped && !mech.isFall && mech.isGrounded)
            {
                getJump = isJump = true;
                mech.isJump = isJump;
            }
            // 空中
            else if (mech.getJump && (mech.hasJumped || mech.isFall) && !mech.isGrounded)
            {
                if (isDoublePress && !mech.boostRecharge && !mech.hasBoostJumped)
                {
                    if (!mech.CanUseBoostEnergy) return;
                    // ★ 二度押し → boostJump のみ
                    lastPressTime = -999f; // 三度押しで再発動しないようにリセット

                    isJump = false;
                    mech.isJump = false;

                    isVerticalBoost = false;
                    mech.isVerticalBoost = false;

                    boostJump = boostRecharge = hasBoostJumped = true;
                    mech.boostJump = boostJump;
                    mech.boostRecharge = boostRecharge;
                    mech.hasBoostJumped = hasBoostJumped;
                }
                else if (!mech.boostJump)
                {
                    // ★ 一度押し → isVerticalBoost
                    isVerticalBoost = true;
                    mech.isVerticalBoost = isVerticalBoost;
                }
            }
        }
        else if (context.canceled)
        {
            // ボタンを離したら垂直ブーストは止める
            isVerticalBoost = false;
            mech.isVerticalBoost = isVerticalBoost;

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
