using UnityEngine;

namespace MirzaBeig.CinematicExplosionsFree
{
    public class CustomImpulse : MonoBehaviour
    {
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private float magnitude = 0.5f;

        // これより遠い爆発では揺らさない。近いほど強く揺れる
        [SerializeField] private float maxDistance = 100f;

        void OnEnable()
        {
            MechCameraControl cam = FindFirstObjectByType<MechCameraControl>();

            if (cam == null)
                return;

            float distance = Vector3.Distance(transform.position, cam.transform.position);
            float rate = 1f - Mathf.Clamp01(distance / maxDistance);

            if (rate <= 0f)
                return;

            // 爆発Prefabが先に消えても揺れが止まらないよう、カメラ側でコルーチンを動かす
            cam.StartCoroutine(cam.CameraShake(duration, magnitude * rate));
        }
    }
}