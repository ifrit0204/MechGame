using UnityEngine;

public class MechBulletTrail : MonoBehaviour
{
    [SerializeField] private TrailRenderer trail;

    void OnDestroy()
    {
        // シーンを閉じるときは何もしない
        if (trail == null || !gameObject.scene.isLoaded)
            return;

        trail.transform.SetParent(null);
        trail.emitting = false;
        Destroy(trail.gameObject, trail.time);
    }
}