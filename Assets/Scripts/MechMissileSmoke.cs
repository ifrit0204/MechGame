using UnityEngine;

public class MechMissileSmoke : MonoBehaviour
{
    [SerializeField] private ParticleSystem smoke;

    void OnDestroy()
    {
        // シーンを閉じるときは何もしない
        if (smoke == null || !gameObject.scene.isLoaded)
            return;

        smoke.transform.SetParent(null);
        smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        // 出ていた煙が消えるまで待ってから削除
        Destroy(smoke.gameObject, smoke.main.startLifetime.constantMax + 0.5f);
    }
}