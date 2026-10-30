using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MechCameraControl : MonoBehaviour
{
    public Transform target; // EmptyObject
    public Vector3 offset; // Camera Position
    public float smoothSpeed, boostSmooth, lookSensitivity;
    public Camera cam;
    public float normalFOV, boostFOV, fovSpeed;
    private float xRotation, yRotation;
    private Vector2 lookInput;
    public bool isBoost, isJump, boostJump;
    private MechControl mech;
    void Start()
    {
        GameObject targetObj = GameObject.Find("EmptyObject");

        if(targetObj != null)
        {
            target = targetObj.transform;
        }
        mech = FindFirstObjectByType<MechControl>();
    }
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
        GameObject targetObj = GameObject.Find("EmptyObject");
        if(targetObj != null)
        {
            target = targetObj.transform;
        }
    }

    // public void OnLook(InputAction.CallbackContext context)
    // {
    //     lookInput = context.ReadValue<Vector2>();
    // }
    void LateUpdate()
    {
        if(mech == null)
        {
            mech = FindFirstObjectByType<MechControl>();
            return;
        }
        // --- 右スティック入力でカメラ角度を更新 ---
        if(mech != null)
        {
            yRotation += mech.lookInput.x * lookSensitivity * Time.deltaTime;
            xRotation -= mech.lookInput.y * lookSensitivity * Time.deltaTime;
        }
        xRotation = Mathf.Clamp(xRotation, -30f, 60f);

        // --- カメラの回転を適用 ---
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // --- EmptyObjectを基点にカメラ位置を更新 ---
        Vector3 desiredPosition = target.position + rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // --- カメラが常にEmptyObjectを向く ---
        transform.LookAt(target);
    }
    public IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
    public void SetBoostFOV(bool boosting)
    {
        float targetFOV = boosting ? boostFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);
    }
    public void SetBoostCameraSpeed(bool boosting)
    {
        smoothSpeed = boosting ? boostSmooth : smoothSpeed;
    }
    public void ResetCameraEffects()
    {
        // FOVを通常に戻す
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normalFOV, Time.deltaTime * fovSpeed);

        // 追従速度を通常に戻す
        smoothSpeed = 10f; // 通常値（必要に応じて調整）
    }
    public void SetSideBoostCamera(bool right)
    {
        Vector3 sideOffset = right ? new Vector3(12f, 3.6f, -36f) : new Vector3(-12f, 3.6f, -36f);
        offset = Vector3.Lerp(offset, sideOffset + new Vector3(0f, 3.6f, -36f), Time.deltaTime * 5f);
    }
    public void ResetSideBoostCamera()
    {
        offset = Vector3.Lerp(offset, new Vector3(0f, 3.6f, -36f), Time.deltaTime * 5f);
    }
    public float Heading
    {
        get
        {
            float h = yRotation % 360f;

            if (h < 0) h += 360f;

            return h;
        }
    }
}
