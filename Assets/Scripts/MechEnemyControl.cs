using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MechEnemyControl : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Ground")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 1.2f;

    public bool isGrounded;

    [Header("Move")]
    public float moveSpeed;
    public float groundMultiplier = 1f;

    [Header("Jump")]
    public float jumpForce = 10f;

    [Header("Boost")]
    public float boostForce = 20f;

    [Header("Parts (パーツから能力値を決める)")]
    [SerializeField] private MechEquipment equipment;

    // プレイヤーのブースト(毎フレーム加わる力)と、敵のブースト(1回の瞬間的な加速)は
    // 力の与え方が違うので、パーツの boostForce をこの倍率で敵用の値に直す。
    // 目安: 基準にしたい敵の「今のBoost Force ÷ そのブースターの boostForce」
    [SerializeField] private float boostImpulseRate = 1f;

    private MechTotalStats stats;
    private bool partsApplied;

    private Vector3 currentMoveDirection;
    private Vector3 horizontalVelocity;

    // パーツから計算した能力値(装備がなければ null)。
    // まだ計算していなければ、ここで一度だけ計算する。
    public MechTotalStats Stats
    {
        get
        {
            if (!partsApplied)
                ApplyParts();

            return stats;
        }
    }

    // ブースト用エネルギーの回復量(毎秒)。ジェネレーターとボディから決まる。0なら未計算。
    public float EnergyRechargeRate { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (!partsApplied)
            ApplyParts();
    }

    // プレイヤーの MechControl.ApplyParts() の敵版。
    // 敵にパーツを装備し終えたあとで呼ぶ(装備を変えたときも、もう一度呼ぶ)。
    // パーツに該当する値がない項目(脚がない、など)は、インスペクターの値のまま残す。
    public void ApplyParts()
    {
        partsApplied = true;

        if (equipment == null)
            equipment = GetComponent<MechEquipment>();

        if (equipment == null)
        {
            stats = null;
            return;
        }

        stats = equipment.CalculateTotalStats();

        // 重いほどブースト速度が下がる(プレイヤーと同じ式)
        float weightMultiplier = equipment.GetBoostWeightMultiplier(stats.weight);

        if (stats.moveSpeed > 0f)
        {
            moveSpeed = stats.moveSpeed;

            if (stats.groundMultiplier > 0f)
                groundMultiplier = stats.groundMultiplier;
        }

        if (stats.boostForce > 0f)
            boostForce = stats.boostForce * weightMultiplier * boostImpulseRate;

        EnergyRechargeRate = equipment.CalcRechargeRate(stats.energyRechargeRateBasic, stats);
    }

    private void FixedUpdate()
    {
        GroundCheck();

        MovePhysics();
    }

    void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundLayer);
    }

    void MovePhysics()
    {
        Vector3 targetMove = currentMoveDirection * moveSpeed * groundMultiplier;

        horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetMove, Time.fixedDeltaTime * 8f);

        rb.AddForce(horizontalVelocity, ForceMode.Acceleration);
    }

    public void Move(Vector3 direction)
    {
        direction.y = 0;

        currentMoveDirection = direction.normalized;
    }

    public void Jump()
    {
        if(!isGrounded)
            return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    public void Boost(Vector3 dir)
    {
        rb.AddForce(dir.normalized * boostForce, ForceMode.VelocityChange);
    }

    public void JumpBoost(Vector3 dir)
    {
        rb.AddForce(dir.normalized * boostForce + Vector3.up * jumpForce, ForceMode.VelocityChange);
    }
}
