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

    private Vector3 currentMoveDirection;
    private Vector3 horizontalVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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