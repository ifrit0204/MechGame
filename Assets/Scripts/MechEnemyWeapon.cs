using UnityEngine;

public class MechEnemyWeapon : MonoBehaviour
{
    public Transform muzzle;
    public GameObject bulletPrefab;
    // public Transform target;

    private MechEnemyBlackboard blackboard;

    private void Awake()
    {
        blackboard = GetComponent<MechEnemyBlackboard>();
    }

    public void Fire()
    {
        if(blackboard.Target == null)
            return;
        // if (target == null)
            // return;

        Rigidbody playerRb = blackboard.Target.GetComponent<Rigidbody>();

        Vector3 predictedPos = blackboard.Target.position;

        if (playerRb != null)
        {
            predictedPos += playerRb.velocity * 0.5f;
        }

        Vector3 fireDir = (predictedPos - muzzle.position).normalized;

        Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(fireDir));
    }
}