using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MechCameraControl : MonoBehaviour
{
    public Transform target; // EmptyObject
    public Vector3 offset; // Camera Position
    // smoothSpeed: 前後(奥行き)の追従速度。ブースト中は boostSmooth を使う
    public float smoothSpeed, boostSmooth, lookSensitivity;
    public Camera cam;
    public float normalFOV, boostFOV, fovSpeed;

    [Header("Follow Lag (遅れて追従)")]
    // 左右の追従速度。小さいほど遅れて、動いた方向にプレイヤーが寄って見える
    // (定常時のずれ ≒ プレイヤーの速さ ÷ この値)
    public float lateralSmoothSpeed = 4f;
    // 上下の追従速度
    public float verticalSmoothSpeed = 6f;
    // 追従点とプレイヤーの最大のずれ(カメラ基準: x=左右, y=上下, z=前後)。
    // 画面外にプレイヤーが出ないようにする上限
    public Vector3 maxLag = new Vector3(8f, 4f, 12f);

    private float xRotation, yRotation;
    private Vector2 lookInput;
    public bool isBoost, isJump, boostJump;
    private MechControl mech;

    // プレイヤーに遅れて追従する点。カメラはこの点を基準に配置され、この点を見る。
    // プレイヤーはこの点より先を進むので、画面の中心からずれて映る。
    private Vector3 pivot;
    private bool pivotInitialized;

    private bool boostCamera;
    private Vector3 shakeOffset;

    void Start()
    {
        GameObject targetObj = GameObject.Find("EmptyObject");

        if (targetObj != null)
        {
            target = targetObj.transform;
        }
        mech = FindFirstObjectByType<MechControl>();
        pivotInitialized = false;
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
        if (targetObj != null)
        {
            target = targetObj.transform;
        }

        // 新しいシーンでは、プレイヤーの位置から追従をやり直す
        pivotInitialized = false;
    }

    void LateUpdate()
    {
        if (mech == null)
        {
            mech = FindFirstObjectByType<MechControl>();
            return;
        }

        if (target == null)
            return;

        float dt = Time.deltaTime;

        // --- 右スティック入力でカメラ角度を更新 ---
        yRotation += mech.lookInput.x * lookSensitivity * dt;
        xRotation -= mech.lookInput.y * lookSensitivity * dt;
        xRotation = Mathf.Clamp(xRotation, -30f, 60f);

        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // --- プレイヤーに遅れて追従する点を更新 ---
        UpdatePivot(rotation, dt);

        // --- カメラは追従点を基準に置き、追従点を見る ---
        // (見る先を target ではなく pivot にすることで、
        //  プレイヤーが動いた方向に、画面の中心からずれて映る)
        transform.position = pivot + rotation * offset + rotation * shakeOffset;
        transform.LookAt(pivot);
    }

    private void UpdatePivot(Quaternion rotation, float dt)
    {
        if (!pivotInitialized)
        {
            pivot = target.position;
            pivotInitialized = true;
            return;
        }

        Quaternion inverse = Quaternion.Inverse(rotation);

        // プレイヤーが追従点からどれだけ離れているか(カメラ基準)
        Vector3 gap = inverse * (target.position - pivot);

        float depthSpeed = boostCamera ? boostSmooth : smoothSpeed;

        // 軸ごとに追従速度を変えて、ずれの一部だけ縮める
        // (1 - exp(-速度 × 時間) は、フレームレートに依存しない追従の割合)
        Vector3 step = new Vector3(
            gap.x * Follow(lateralSmoothSpeed, dt),
            gap.y * Follow(verticalSmoothSpeed, dt),
            gap.z * Follow(depthSpeed, dt));

        pivot += rotation * step;

        // ずれが大きくなりすぎたら、上限で止める(高速移動でも画面外に出さない)
        Vector3 remain = inverse * (target.position - pivot);

        remain.x = Mathf.Clamp(remain.x, -maxLag.x, maxLag.x);
        remain.y = Mathf.Clamp(remain.y, -maxLag.y, maxLag.y);
        remain.z = Mathf.Clamp(remain.z, -maxLag.z, maxLag.z);

        pivot = target.position - rotation * remain;
    }

    private static float Follow(float speed, float dt)
    {
        return 1f - Mathf.Exp(-Mathf.Max(0f, speed) * dt);
    }

    public IEnumerator CameraShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            shakeOffset = new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }

    public void SetBoostFOV(bool boosting)
    {
        float targetFOV = boosting ? boostFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);
    }

    // ブースト中は追従を速くする(smoothSpeed 自体は書き換えない)
    public void SetBoostCameraSpeed(bool boosting)
    {
        boostCamera = boosting;
    }

    public void ResetCameraEffects()
    {
        // FOVを通常に戻す
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normalFOV, Time.deltaTime * fovSpeed);

        // 追従速度を通常に戻す
        boostCamera = false;
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
