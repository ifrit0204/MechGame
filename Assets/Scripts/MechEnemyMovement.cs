using UnityEngine;

public class MechEnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [SerializeField] private MechEnemyStatsData statsData;

    private MechStatsControl statsControl;

    int strafeDirection = 1;

    // private MechEquipment equipment;
    // private MechTotalStats stats;

    private void Awake()
    {
        statsControl = GetComponent<MechStatsControl>();
        // equipment = GetComponent<MechEquipment>();
    }

    private void Start()
    {
        if(statsData != null)
        {
            moveSpeed = statsData.moveSpeed;
        }
        else
        {
            if(statsControl == null)
            {
                Debug.LogError("StatsControl Null");
                return;
            }

            if(statsControl.CurrentStats == null)
            {
                Debug.LogError("CurrentStats Null");
                return;
            }

            moveSpeed = statsControl.CurrentStats.moveSpeed;
        }

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

    public void MovePosition(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;

        transform.position += dir * moveSpeed * Time.deltaTime;

        dir.y = 0;

        if(dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);
        }
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

        transform.position += moveDir * moveSpeed * Time.deltaTime;
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