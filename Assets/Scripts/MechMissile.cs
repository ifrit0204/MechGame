using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MechMissile : MonoBehaviour, IProjectile
{
    [Header("Damage")]
    public DamageData damageData;

    [Header("Flight")]
    public float moveSpeed = 50f;
    public float turnSpeed = 180f;
    public float boostTime = 0.5f;
    public float selfDestroyTime = 10f;

    [Header("Homing")]
    public float aimHeight = 2f;
    private Rigidbody rb;
    private Transform target;
    private float timer;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
    }

    private void Start()
    {
        Destroy(gameObject, selfDestroyTime);
    }
    public void Initialize(DamageData damage, float speed, Transform target)
    {
        damageData = damage;

        moveSpeed = speed;

        this.target = target;
    }

    // public void Initialize(DamageData data, float speed, Transform targetTransform)
    // {
    //     damageData = data;
    //     moveSpeed = speed;
    //     target = targetTransform;
    // }
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (timer < boostTime)
        {
            rb.velocity = transform.forward * moveSpeed;
            return;
        }

        if (target == null)
        {
            rb.velocity = transform.forward * moveSpeed;
            return;
        }

        Vector3 targetPos = target.position;

        Vector3 direction = (targetPos - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        Quaternion nextRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(nextRotation);

        rb.velocity = nextRotation * Vector3.forward * moveSpeed;
    }

    // private void FixedUpdate()
    // {
    //     timer += Time.fixedDeltaTime;

    //     // 発射直後は直進
    //     if (timer < boostTime)
    //     {
    //         rb.velocity = transform.forward * moveSpeed;
    //         return;
    //     }

    //     // ターゲットが消えた
    //     if (target == null)
    //     {
    //         rb.velocity = transform.forward * moveSpeed;
    //         return;
    //     }

    //     // 少し上を狙う
    //     Vector3 targetPos = target.position + Vector3.up * aimHeight;

    //     Vector3 direction = (targetPos - transform.position).normalized;

    //     Quaternion targetRotation = Quaternion.LookRotation(direction);

    //     Quaternion nextRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);

    //     rb.MoveRotation(nextRotation);

    //     rb.velocity = nextRotation * Vector3.forward * moveSpeed;
    // }

    private void OnTriggerEnter(Collider col)
    {
        IDamageable damageable = col.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damageData);
        }
        Destroy(gameObject);
    }
}