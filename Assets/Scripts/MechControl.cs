using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MechControl : MonoBehaviour, IDamageable
{
    public float moveSpeed, jumpForce, jumpPerformance, boostForce, boostFirstForce, boostJumpForce, hyperBoost, hyperFirstBoost, fallSpeed, jumpCancelSpeed;
    public float boostEnergy, boostEnergyMax, boostCost, boostJumpCost, hyperBoostCost, boostEfficiency, energyRechargeRate, energyRechargeRateBasic;
    public float jumpTime, jumpTimer, boostTime, boostTimer, boostJumpTime, boostJumpTimer, boostRechargeTime, boostRechargeTimer, energyRechargeTime, energyRechargeTimer;
    public float groundCheckDistance, upwardLimit;
    public float upwardVelocity, gravityVelocity;

    public float maxHP;
    public float currentHP;
    public float totalDamageTaken;

    [SerializeField] private GameObject explosionPrefab;

    // totalDamageTaken += damageData.Damage;
    // CurrentResult.damageTaken = player.totalDamageTaken;

    // public float rollAngle = 0f; // 現在の傾き
    // public float maxRollAngle = 22f; // 最大傾き角度
    // public float rollSpeed = 8f; // 傾きの速度

    public Vector2 moveInput;
    public Vector2 lookInput;
    private Vector3 boostDirection;
    private Vector3 horizontalVelocity; // 水平慣性
    public LayerMask groundLayer;
    public Rigidbody rb;
    public Animator anim;
    public bool isGrounded, isEnergyRecharge, getBoost, boosting, isBoost, hasBoosted, getJump, isJump, boostJumping, boostJump, hasBoostJumped, getHyperBoost, isHyperBoost, isHyperBoostReady, hasHyperBoosted, boostRecharge, hasJumped, isFall, isMeleeDash;
    private bool isDead;
    private MechJumpAction mechJump;
    private MechBoostAction mechBoost;
    private MechHyperBoostAction mechHyper;
    public MechEquipment equipment;
    public MechCameraControl mechCamera;
    private MechLockOn lockOn;
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mechCamera = FindObjectOfType<MechCameraControl>();
        if (Camera.main == null)
        {
            Debug.LogError("Main Cameraが見つかりません");
            return;
        }

        Transform cam = Camera.main.transform;
    }
    void Start()
    {
        hasBoostJumped = isDead = false;
        
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        
        mechJump = GetComponent<MechJumpAction>();
        mechBoost = GetComponent<MechBoostAction>();
        mechHyper = GetComponent<MechHyperBoostAction>();
        mechCamera = FindFirstObjectByType<MechCameraControl>();
        lockOn = GetComponent<MechLockOn>();

        jumpTimer = jumpTime;
        boostTimer = boostTime;
        boostJumpTimer = boostJumpTime;
        boostRechargeTimer = boostRechargeTime;
        energyRechargeTimer = energyRechargeTime;

        // Transform cam = Camera.main.transform;
        if (Camera.main == null)
        {
            Debug.LogError("Main Cameraが見つかりません");
            return;
        }

        Transform cam = Camera.main.transform;
        ApplyParts();
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    public void ApplyParts()
    {
        if(equipment == null)
            return;
        if(equipment.legs != null)
            moveSpeed = equipment.legs.moveSpeed;
        if(equipment.booster != null)
            boostForce = equipment.booster.boostForce;
        if(equipment.generator != null)
            boostEnergyMax = equipment.generator.boostEnergyMax;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            anim.SetTrigger("Dash");
        }
    }
    public void TakeDamage(DamageData damageData)
    {
        totalDamageTaken += damageData.Damage;

        switch (damageData.Type)
        {
            case DamageType.Physical:
            currentHP -= damageData.Damage;
            break;

            case DamageType.Energy:
            currentHP -= damageData.Damage * 0.8f;
            break;

            case DamageType.Explosion:
            currentHP -= damageData.Damage * 1.2f;
            break;
        }
        // Debug.Log("After " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        // MechGameManager.Instance.CurrentResult.enemyKillCount++;

        if (isDead)
        return;

        isDead = true;

        StartCoroutine(DeathRoutine());
    }
    IEnumerator DeathRoutine()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<MechControl>().enabled = false;

        foreach(var r in GetComponentsInChildren<MeshRenderer>())
        {
            r.enabled = false;
        }

        GameObject effect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(effect, 2f);

        yield return new WaitForSeconds(2f);

        // MechGameManager.Instance.EnemyDestroyed();

        Destroy(gameObject);
    }
    void Update()
    {
        // 移動量の合成（縦横両方を考慮）
        float moveAmount = Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y);

        // Animatorに渡すパラメータ
        anim.SetFloat("Move", moveAmount);

        if (!isGrounded && rb.velocity.y > 0) anim.SetTrigger("Jump");
        if (!isGrounded && rb.velocity.y < 0) anim.SetTrigger("Fall");
    }
    void FixedUpdate()
    {
        Transform cam = Camera.main.transform;

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 movement = camForward * moveInput.y + camRight * moveInput.x;

        Vector3 targetMove = movement * moveSpeed * 100f; // groundMultiplier

        // ★ 慣性を使って滑らかに移動
        horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetMove, Time.fixedDeltaTime * 10f);

        if (isGrounded) rb.AddForce(horizontalVelocity, ForceMode.Acceleration);
        else if (!isGrounded && (isJump || isFall || boostJump))
        {
            // ★ 空中専用の移動速度（慣性ではなく入力方向へ）
            Vector3 airMove = movement * (moveSpeed * 0.54f);

            airMove = Vector3.Lerp(airMove, Vector3.zero, Time.fixedDeltaTime);

            horizontalVelocity = airMove;
            // ★ 加速し続けない自然な空中移動
            rb.AddForce(airMove, ForceMode.VelocityChange);
            // if (isJump || boostJump)
            // {
            //     // ★ 空中専用の移動速度（慣性ではなく入力方向へ）
            //     Vector3 airMove = movement * (moveSpeed * 0.054f);

            //     airMove = Vector3.Lerp(airMove, Vector3.zero, Time.fixedDeltaTime);

            //     // ★ 空中では Acceleration が自然（質量に依存しない）
            //     rb.AddForce(airMove, ForceMode.Acceleration);
            // }
        }


        // rb.AddForce(movement * moveSpeed, ForceMode.Force);

        Vector3 boostDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        if (moveInput.sqrMagnitude < 0.01f)
        {
            boostDirection = transform.forward;
        }

        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.12f, Vector3.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(transform.position + Vector3.up * 0.12f, Vector3.down * groundCheckDistance, Color.red);
        if (isGrounded)
        {
            mechJump.isGrounded = isGrounded;
            getJump = boostJump = hasBoosted = hasBoostJumped = hasJumped = isFall = false;
            mechJump.hasJumped = hasJumped;
            mechBoost.hasBoosted = hasBoosted;
            mechHyper.hasHyperBoosted = hasHyperBoosted;
            mechJump.isFall = isFall;
            mechJump.getJump = getJump;
            jumpTime = jumpTimer;
            // boostTime = boostTimer;
            boostJumpTime = boostJumpTimer;
            // boostRechargeTime = boostRechargeTimer;
            energyRechargeRate += boostEnergy;
            energyRechargeRate = Mathf.Min(energyRechargeRate, boostEnergyMax);
            boostEnergy = Mathf.Clamp(boostEnergy, 0, boostEnergyMax);
            upwardVelocity = gravityVelocity = 0.00f;
            horizontalVelocity = Vector3.zero;
        }
        if ((hasBoosted || hasBoostJumped || boostRecharge) && !isGrounded)
        {
            boostRechargeTime -= Time.fixedDeltaTime;
            if (boostRechargeTime <= 0)
            {
                getBoost = isBoost = boostJump = boostRecharge = hasBoostJumped = false;
                mechBoost.getBoost = getBoost;
                mechBoost.isBoost = isBoost;
                mechJump.boostJump = boostJump;
                mechJump.boostRecharge = boostRecharge;
                // mechJump.hasBoosted = hasBoosted;
                // mechHyper.hasHyperBoosted = hasHyperBoosted;
                mechJump.hasBoostJumped = hasBoostJumped;
                // boostTime = boostTimer;
                boostJumpTime = boostJumpTimer;
                boostRechargeTime = boostRechargeTimer;
                mechCamera.SetBoostFOV(false);
                mechCamera.SetBoostCameraSpeed(false);
                mechCamera.ResetCameraEffects();
            }
            if (boostJumpTime >= 0)
            {
                if (boostRechargeTime <= 0)
                {
                    hasBoostJumped = false;
                    mechJump.hasBoostJumped = hasBoostJumped;
                    // boostJumpTime = boostJumpTimer;
                    boostRechargeTime = boostRechargeTimer;
                    mechCamera.ResetCameraEffects();
                }
            }
        }
        else if (isBoost)
        {
            boostRechargeTime -= Time.fixedDeltaTime;
            if (boostRechargeTime <= 0)
            {
                getBoost = isBoost = boostRecharge = false;
                mechBoost.getBoost = getBoost;
                mechBoost.isBoost = isBoost;
                mechBoost.boostRecharge = boostRecharge;
                hasBoosted = true;
                mechBoost.hasBoosted = hasBoosted;
                boostTime = boostTimer;
                boostRechargeTime = boostRechargeTimer;
                mechCamera.SetBoostFOV(false);
                mechCamera.SetBoostCameraSpeed(false);
                mechCamera.ResetCameraEffects();
            }
            if (boostTime <= 0)
            {
                getBoost = isBoost = boostRecharge = false;
                mechBoost.getBoost = getBoost;
                mechBoost.isBoost = isBoost;
                mechBoost.boostRecharge = boostRecharge;
                hasBoosted = true;
                mechBoost.hasBoosted = hasBoosted;
                boostTime = boostTimer;
                boostRechargeTime = boostRechargeTimer;
                mechCamera.SetBoostFOV(false);
                mechCamera.SetBoostCameraSpeed(false);
                mechCamera.ResetCameraEffects();
                // boostRecharge = true;
                // mechBoost.boostRecharge = boostRecharge;
            }
        }
        if (boosting && !(isBoost || isHyperBoost))
        {
            boostEnergy -= boostCost * Time.fixedDeltaTime * boostEfficiency;
            boostDirection.Normalize();
            horizontalVelocity = boostDirection * boostForce;
            Quaternion targetRotation = Quaternion.LookRotation(boostDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.36f);
            rb.AddForce(horizontalVelocity, ForceMode.Acceleration);
        }
        if (boostJumping)
        {
            Debug.Log("Jumping!");
            float upwardDrag = Mathf.Lerp(1f, 0.54f, rb.velocity.y / jumpForce);
            boostEnergy -= boostCost * Time.fixedDeltaTime;
            upwardVelocity = jumpForce * upwardDrag;
            gravityVelocity = 0.00f;
            // boostDirection.Normalize();
            // horizontalVelocity = boostDirection * boostForce;
            // Quaternion targetRotation = Quaternion.LookRotation(boostDirection);
            // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.36f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
        }
        if (!isHyperBoost)
        {
            if (!isGrounded && rb.velocity.y < 0)
            {
                getJump = isFall = true;
                mechJump.isFall = isFall;
                mechJump.getJump = getJump;
                isJump = false;
                mechJump.isJump = isJump;
            }
            else if (rb.velocity.y >= 0 && !isJump && !isGrounded)
            {
                getJump = hasJumped = isFall = true;
                mechJump.hasJumped = hasJumped;
                mechJump.getJump = getJump;
                mechJump.isFall = isFall;
                isJump = false;
                mechJump.isJump = isJump;
            }
            if (!isGrounded && upwardVelocity > 0)
            {
                // ★ 慣性減衰
                upwardVelocity = Mathf.Lerp(upwardVelocity, 0, Time.fixedDeltaTime * 2f);

                // ★ 慣性による上昇
                rb.AddForce(Vector3.up * upwardVelocity, ForceMode.Acceleration);
            }
            if (!isGrounded && upwardVelocity <= upwardLimit)
            {
                gravityVelocity += Physics.gravity.y * jumpForce * Time.fixedDeltaTime;

                gravityVelocity = Mathf.Clamp(gravityVelocity, Physics.gravity.y * jumpForce * jumpPerformance, 0);

                rb.AddForce(Vector3.up * gravityVelocity, ForceMode.Acceleration);
            }
        }



        if (isJump && jumpTime >= 0)
        {
            float upwardDrag = Mathf.Lerp(1f, 0.54f, rb.velocity.y / jumpForce);
            // Vector3 airMove = movement * (moveSpeed * 0.54f);
            jumpTime -= Time.fixedDeltaTime;
            energyRechargeRate = energyRechargeRateBasic;

            // ★ 上方向慣性を記録
            upwardVelocity = jumpForce * upwardDrag;

            rb.AddForce(Vector3.up * jumpForce * upwardVelocity, ForceMode.Force);
            // rb.AddForce(airMove, ForceMode.Acceleration);
        }

        else if (boostJump && boostJumpTime >= 0 && boostEnergy > 0)
        {
            float upwardDrag = Mathf.Lerp(1f, 0.54f, rb.velocity.y / jumpForce * boostFirstForce);
            boostJumpTime -= Time.fixedDeltaTime;
            boostEnergy -= boostJumpCost * Time.fixedDeltaTime;

            // ★ 上方向慣性を記録
            upwardVelocity = boostJumpForce * upwardDrag;
            gravityVelocity = 0.00f;

            rb.AddForce(Vector3.up * jumpForce * upwardVelocity, ForceMode.Force);
            boostJump = hasBoostJumped = boostRecharge = true;
            mechJump.boostJump = boostJump;
            mechJump.hasBoostJumped = hasBoostJumped;
            mechJump.boostRecharge = boostRecharge;
        }
        if (isBoost && boostTime >= 0 && boostEnergy > 0)
        {
            boostTime -= Time.fixedDeltaTime;
            boostEnergy -= boostCost * Time.fixedDeltaTime;
            mechCamera.SetBoostFOV(true);
            mechCamera.SetBoostCameraSpeed(true);
            boostDirection.Normalize();

            // ★ ブースト開始時に水平慣性方向を記録
            horizontalVelocity = boostDirection * (boostForce + boostFirstForce);

            Quaternion targetRotation = Quaternion.LookRotation(boostDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.36f);
            // ★ 慣性方向を少しずつ減衰（空気抵抗）
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.fixedDeltaTime * 0.8f);

            // ★ 慣性による水平移動
            rb.AddForce(horizontalVelocity, ForceMode.Acceleration);
            // rb.AddForce(boostDirection * boostForce * boostFirstForce, ForceMode.Force);
            if (moveInput.x != 0) mechCamera.SetSideBoostCamera(true);
            else if (moveInput.x == 0) mechCamera.SetSideBoostCamera(false);
            else mechCamera.ResetSideBoostCamera();
            isBoost = hasBoosted = boostRecharge = true;
            mechBoost.isBoost = isBoost;
            mechBoost.hasBoosted = hasBoosted;
            mechBoost.boostRecharge = boostRecharge;
            // mechCamera.smoothSpeed = 100f;
        }
        if (isHyperBoost && boostEnergy > 0)
        {
            boostEnergy -= hyperBoostCost * Time.fixedDeltaTime;
            mechCamera.SetBoostFOV(true);
            mechCamera.SetBoostCameraSpeed(true);
            boostDirection.Normalize();
            horizontalVelocity = boostDirection * (hyperBoost + hyperFirstBoost);
            Quaternion targetRotation = Quaternion.LookRotation(boostDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.36f);
            // ★ 慣性方向を少しずつ減衰（空気抵抗）
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.fixedDeltaTime * 0.8f);
            rb.AddForce(horizontalVelocity, ForceMode.Acceleration);
        }
        
        if (boostEnergy <= 0)
        {
            isEnergyRecharge = true;
            isBoost = isHyperBoost = false;
            mechBoost.isBoost = isBoost;
            mechHyper.isHyperBoost = isHyperBoost;
            energyRechargeTime -= Time.fixedDeltaTime;
            if (energyRechargeTime <= 0)
            {
                boostEnergy = boostEnergyMax / 2;
                isEnergyRecharge = false;
                energyRechargeTime = energyRechargeTimer;
            }
        }


        if (!isJump && !isFall && !isBoost && !boostJump && !isHyperBoost)
        {
            // ★ ブースト終了後の慣性滑り
            rb.AddForce(horizontalVelocity, ForceMode.Acceleration);

            // ★ 徐々に慣性を消す
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.fixedDeltaTime);
        }
        if (hasBoosted || hasHyperBoosted)
        {
            rb.AddForce(horizontalVelocity, ForceMode.Acceleration);
        }
        if (!boosting && !isBoost && !boostJump && !isHyperBoost)
        {
            if (isEnergyRecharge) return;

            // boostEnergy += energyRechargeRate * Time.fixedDeltaTime;
            boostEnergy = Mathf.Min(boostEnergy + energyRechargeRate * Time.fixedDeltaTime, boostEnergyMax);
            // boostEnergy = Mathf.Clamp(boostEnergy, 0, boostEnergyMax);
        }
        if(isMeleeDash && lockOn.currentTarget != null)
        {
            Vector3 dir = lockOn.currentTarget.position - transform.position;

            // dir.y = 0;

            rb.AddForce(dir.normalized * boostForce * boostFirstForce, ForceMode.Acceleration);
        }
        // if(isMeleeDash && lockOn.currentTarget != null)
        // {
        //     Vector3 dir = lockOn.currentTarget.position - transform.position;

        //     rb.AddForce(dir.normalized * boostForce * boostFirstForce, ForceMode.Acceleration);
        // }

        if (movement != Vector3.zero)
        {
            if (lockOn.currentTarget != null)
            {
                Vector3 dir = lockOn.currentTarget.position - transform.position;

                dir.y = 0;

                Quaternion targetRot = Quaternion.LookRotation(dir);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
                return;
            }
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.12f);
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isGrounded = true;
            hasJumped = isFall = false;
            mechJump.hasJumped = hasJumped;
            mechJump.isFall = isFall;
        }
    }
    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isGrounded = false;
            mechJump.isGrounded = isGrounded;
            energyRechargeRate = energyRechargeRateBasic;
        }
    }
}
