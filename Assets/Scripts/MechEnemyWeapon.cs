using System.Collections;
using UnityEngine;

public class MechEnemyWeapon : MonoBehaviour
{
    private Transform muzzle;
    public GameObject bulletPrefab;
    [SerializeField] private MechWeaponPartData weaponData;
    [SerializeField] private Transform armPivot;

    [Header("Melee")]
    // アニメーションを再生できない場合だけ使う、タイマー式のヒット判定
    [SerializeField] private float meleeWindup = 0.25f;
    [SerializeField] private float meleeActiveTime = 0.25f;
    [SerializeField] private float meleeCooldown = 1f;
    // アニメーションイベント(Disable)が来なくても、この秒数で必ず攻撃を終了させる安全装置
    [SerializeField] private float meleeMaxDuration = 1.5f;

    public float accuracy = 1f;
    private float nextFireTime;
    private float nextMeleeAttackTime;
    private bool meleeAttackInProgress;
    private MechMeleeWeapon activeMeleeWeapon;
    private Coroutine meleeRoutine;

    private MechEnemyBlackboard blackboard;
    private MechEquipment equipment;
    private Animator animator;

    private void Awake()
    {
        blackboard = GetComponent<MechEnemyBlackboard>();
        equipment = GetComponent<MechEquipment>();
        animator = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        // 無効化・破棄されたときにヒット判定が残らないようにする
        if (meleeAttackInProgress)
            FinishMeleeAttack();
    }

    public bool IsMeleeAttackInProgress => meleeAttackInProgress;
    public bool IsMeleeReady => !meleeAttackInProgress && Time.time >= nextMeleeAttackTime;

    public bool HasMeleeWeapon
    {
        get
        {
            MechMeleeWeapon meleeWeapon;
            MechWeaponPartData meleeWeaponData;
            return TryGetMeleeWeapon(out meleeWeapon, out meleeWeaponData);
        }
    }

    public float MeleeAttackRange
    {
        get
        {
            MechMeleeWeapon meleeWeapon;
            MechWeaponPartData meleeWeaponData;
            return TryGetMeleeWeapon(out meleeWeapon, out meleeWeaponData)
                ? Mathf.Max(0f, meleeWeaponData.attackRange)
                : 0f;
        }
    }

    public void AimWeapon(Transform target)
    {
        if (target == null)
            return;

        if (armPivot == null)
            return;

        Vector3 dir = target.position - armPivot.position;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        armPivot.rotation = Quaternion.Slerp(armPivot.rotation, targetRot, Time.deltaTime * 10f);
    }

    public void Fire()
    {
        if (weaponData == null || weaponData.projectilePrefab == null || muzzle == null)
            return;

        if (Time.time < nextFireTime)
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

        if (playerRb != null)
        {
            predictedPos += playerRb.velocity * (accuracy * 0.5f);
        }

        Vector3 fireDir = (predictedPos - muzzle.position).normalized;

        GameObject bullet = Instantiate(weaponData.projectilePrefab, muzzle.position, Quaternion.LookRotation(fireDir));

        Collider bulletCol = bullet.GetComponent<Collider>();

        if (bulletCol != null)
        {
            Collider[] ownerCols = transform.root.GetComponentsInChildren<Collider>();

            foreach (var col in ownerCols)
            {
                Physics.IgnoreCollision(bulletCol, col);
            }
        }

        IProjectile projectile = bullet.GetComponent<IProjectile>();

        if (projectile != null)
        {
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

    // ------------------------------------------------------------
    // 近接攻撃
    // ------------------------------------------------------------

    public void StartMeleeAttack()
    {
        if (blackboard.Target == null || meleeAttackInProgress || Time.time < nextMeleeAttackTime)
            return;

        MechMeleeWeapon meleeWeapon;
        MechWeaponPartData meleeWeaponData;

        if (!TryGetMeleeWeapon(out meleeWeapon, out meleeWeaponData))
            return;

        if (meleeWeaponData.attackRange <= 0f ||
            Vector3.Distance(transform.position, blackboard.Target.position) > meleeWeaponData.attackRange)
            return;

        nextMeleeAttackTime = Time.time + Mathf.Max(meleeCooldown, meleeWeaponData.reloadTime);
        meleeWeapon.SetWeapon(meleeWeaponData);

        activeMeleeWeapon = meleeWeapon;
        meleeAttackInProgress = true;

        // アニメーションが再生できたら、ヒット判定はアニメーションイベントに任せる。
        // 再生できなければ、従来のタイマー方式で判定する。
        bool animationPlayed = TriggerMeleeAnimation(meleeWeaponData.attackTrigger);

        meleeRoutine = StartCoroutine(MeleeAttackRoutine(animationPlayed));
    }

    // アニメーションイベントの受け口。
    // LaserBladeSlash クリップに登録されたイベント名と同じ名前にしておく。
    public void EnableMeleeWeaponHitbox()
    {
        if (!meleeAttackInProgress || activeMeleeWeapon == null)
            return;

        activeMeleeWeapon.BeginAttack();
    }

    public void DisableMeleeWeaponHitbox()
    {
        FinishMeleeAttack();
    }

    private IEnumerator MeleeAttackRoutine(bool animationDriven)
    {
        if (animationDriven)
        {
            // 開始・終了はイベントが担当する。
            // イベントが来なくても詰まらないよう、一定時間で必ず終了させる。
            yield return new WaitForSeconds(meleeMaxDuration);
        }
        else
        {
            if (meleeWindup > 0f)
                yield return new WaitForSeconds(meleeWindup);

            if (activeMeleeWeapon != null)
                activeMeleeWeapon.BeginAttack();

            if (meleeActiveTime > 0f)
                yield return new WaitForSeconds(meleeActiveTime);
        }

        meleeRoutine = null;
        FinishMeleeAttack();
    }

    private void FinishMeleeAttack()
    {
        if (meleeRoutine != null)
        {
            StopCoroutine(meleeRoutine);
            meleeRoutine = null;
        }

        if (activeMeleeWeapon != null)
        {
            activeMeleeWeapon.EndAttack();
            activeMeleeWeapon = null;
        }

        meleeAttackInProgress = false;
    }

    private bool TryGetMeleeWeapon(out MechMeleeWeapon meleeWeapon, out MechWeaponPartData meleeWeaponData)
    {
        meleeWeapon = null;
        meleeWeaponData = null;

        if (equipment == null)
            equipment = GetComponent<MechEquipment>();

        if (equipment == null)
            return false;

        if (TryGetMeleeWeapon(equipment.currentRightWeapon, equipment.rightWeapon, out meleeWeapon))
        {
            meleeWeaponData = equipment.rightWeapon;
            return true;
        }

        if (TryGetMeleeWeapon(equipment.currentLeftWeapon, equipment.leftWeapon, out meleeWeapon))
        {
            meleeWeaponData = equipment.leftWeapon;
            return true;
        }

        return false;
    }

    private bool TryGetMeleeWeapon(GameObject weaponObject, MechWeaponPartData partData, out MechMeleeWeapon meleeWeapon)
    {
        meleeWeapon = null;

        if (weaponObject == null || partData == null)
            return false;

        meleeWeapon = weaponObject.GetComponentInChildren<MechMeleeWeapon>();
        return meleeWeapon != null;
    }

    // トリガーを実際にセットできたら true を返す
    private bool TriggerMeleeAnimation(string triggerName)
    {
        if (animator == null || string.IsNullOrEmpty(triggerName))
            return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == triggerName)
            {
                animator.SetTrigger(triggerName);
                return true;
            }
        }

        return false;
    }
}