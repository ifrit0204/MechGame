using UnityEngine;

public static class MechHitEffect
{
    // 着弾位置にエフェクトを出し、一定時間後に自動で消す
    public static void Spawn(GameObject prefab, Vector3 position, Vector3 normal, float lifeTime = 2f)
    {
        if (prefab == null)
            return;

        // 法線方向(当たった面の外側)へエフェクトの正面を向ける
        Quaternion rot = normal.sqrMagnitude > 0.001f
            ? Quaternion.LookRotation(normal)
            : Quaternion.identity;

        GameObject fx = Object.Instantiate(prefab, position, rot);

        Object.Destroy(fx, lifeTime);
    }
}