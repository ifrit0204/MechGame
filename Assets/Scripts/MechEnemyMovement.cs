using UnityEngine;

public class MechEnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    int strafeDirection = 1;

    private void Start()
    {
        StartCoroutine(ChangeStrafe());
    }

    public void MoveTowards(Transform target)
    {
        if (target == null)
            return;

        Vector3 dir = (target.position - transform.position).normalized;

        transform.position += dir * moveSpeed * Time.deltaTime;

        dir.y = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);
    }

    public void Strafe(Transform target)
    {
        if (target == null)
            return;

        Vector3 lookDir = target.position - transform.position;

        lookDir.y = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 5f * Time.deltaTime);

        Vector3 moveDir = transform.right * strafeDirection;

        if (Physics.Raycast(transform.position, moveDir, 3f))
        {
            strafeDirection *= -1;
            moveDir = transform.right * strafeDirection;
        }

        transform.position +=
            moveDir * moveSpeed * Time.deltaTime;
    }

    System.Collections.IEnumerator ChangeStrafe()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(2f, 4f));

            strafeDirection *= -1;
        }
    }
}