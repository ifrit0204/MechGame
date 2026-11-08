using UnityEngine;

public class MechEnemyWeapon : MonoBehaviour
{
    private Transform muzzle;
    public GameObject bulletPrefab;
    [SerializeField] private MechWeaponPartData weaponData;
    private float nextFireTime;
    // public Transform target;

    private MechEnemyBlackboard blackboard;

    private void Awake()
    {
        blackboard = GetComponent<MechEnemyBlackboard>();
    }

    public void Fire()
    {
        if(weaponData == null)
            return;

        if(Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + weaponData.reloadTime;

        SpawnBullet();
    }

    private void SpawnBullet()
    {
        if (blackboard.Target == null)
            return;

        Rigidbody playerRb = blackboard.Target.GetComponent<Rigidbody>();

        Vector3 predictedPos = blackboard.Target.position;

        if(playerRb != null)
        {
            predictedPos += playerRb.velocity * 0.5f;
        }

        Vector3 fireDir = (predictedPos - muzzle.position).normalized;

        // GameObject bullet = Instantiate(weaponData.projectilePrefab, muzzle.position, Quaternion.LookRotation(fireDir));

        // IProjectile projectile = bullet.GetComponent<IProjectile>();

        // if(projectile != null)
        // {
        //     projectile.Initialize(weaponData.damageData, weaponData.bulletSpeed, blackboard.Target);
        // }

        GameObject bullet = Instantiate(weaponData.projectilePrefab, muzzle.position, Quaternion.LookRotation(fireDir));

        Collider bulletCol = bullet.GetComponent<Collider>();

        if(bulletCol != null)
        {
            Collider[] ownerCols = transform.root.GetComponentsInChildren<Collider>();

            foreach(var col in ownerCols)
            {
                Physics.IgnoreCollision(bulletCol, col);
            }
        }

        IProjectile projectile = bullet.GetComponent<IProjectile>();

        if(projectile != null)
        {
            // projectile.Initialize(weaponData.damageData, weaponData.bulletSpeed, blackboard.Target);
            projectile.Initialize(weaponData.damageData, weaponData.bulletSpeed, blackboard.Target, TeamType.Enemy);
        }
    }

    public void SetWeapon(MechWeaponPartData data)
    {
        weaponData = data;
    }
    public void SetMuzzle(Transform muzzleTransform)
    {
        muzzle = muzzleTransform;
    }

    // public void Fire()
    // {
    //     Debug.Log("Fire");

    //     if(blackboard.Target == null)
    //         return;
    //     // if (target == null)
    //         // return;

    //     Rigidbody playerRb = blackboard.Target.GetComponent<Rigidbody>();

    //     Vector3 predictedPos = blackboard.Target.position;

    //     if (playerRb != null)
    //     {
    //         predictedPos += playerRb.velocity * 0.5f;
    //     }

    //     Vector3 fireDir = (predictedPos - muzzle.position).normalized;

    //     // Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(fireDir));

    //     GameObject bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(fireDir));

    //     MechBullet projectile = bullet.GetComponent<MechBullet>();

    //     projectile.Initialize(damageData, bulletSpeed, blackboard.Target);

    //     Debug.Log("Bullet Spawn");
    // }
}