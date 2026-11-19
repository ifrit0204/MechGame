using UnityEngine;

public class MechEnemyMeleeAnimationEventReceiver : MonoBehaviour
{
    private MechEnemyWeapon enemyWeapon;

    private void Awake()
    {
        enemyWeapon = GetComponentInParent<MechEnemyWeapon>();

        if (enemyWeapon == null)
            Debug.LogError("MechEnemyWeapon was not found on a parent object.", this);
    }

    public void EnableMeleeWeaponHitbox()
    {
        if (enemyWeapon != null)
            enemyWeapon.EnableMeleeWeaponHitbox();
    }

    public void DisableMeleeWeaponHitbox()
    {
        if (enemyWeapon != null)
            enemyWeapon.DisableMeleeWeaponHitbox();
    }
}
