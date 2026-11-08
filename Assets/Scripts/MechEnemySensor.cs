using UnityEngine;

public class MechEnemySensor : MonoBehaviour
{
    // public Transform target;

    [SerializeField] private float detectDistance;

    [SerializeField] private float viewAngle;

    [SerializeField] private Transform sensorPoint;

    [SerializeField] private float offsetY;

    [SerializeField] private float offsetZ;
    [SerializeField] private MechEnemyStatsData statsData;

    private MechEnemyBlackboard blackboard;
    private MechStatsControl statsControl;
    private float lockTimer;

    private void Awake()
    {
        blackboard = GetComponent<MechEnemyBlackboard>();
        statsControl = GetComponent<MechStatsControl>();
    }

    private void Update()
    {
        if (blackboard.Target == null)
        {
            Debug.Log("Target Null");
            return;
        }

        bool visible = CanSeeTarget();

        blackboard.CanSeeTarget = visible;

        Debug.Log("CanSeeTarget = " + visible);

        blackboard.TargetDistance = Vector3.Distance(transform.position, blackboard.Target.position);

        if(visible)
        {
            blackboard.LastKnownPosition = blackboard.Target.position;
        }
    }

    public bool CanSeeTarget()
    {
        if (blackboard.Target == null)
            return false;

        // if (statsControl == null)
        //     return false;

        // if (statsControl.CurrentStats == null)
        //     return false;

        // float lockRange = statsControl.CurrentStats.lockRange;

        // if(statsControl != null && statsControl.CurrentStats != null)
        // {
        //     float lockRange = statsControl.CurrentStats.lockRange;
        // }
        // else
        // {
        //     float lockRange = detectDistance;
        // }

        float lockRange = detectDistance;

        if(statsControl != null && statsControl.CurrentStats != null)
        {
            lockRange = statsControl.CurrentStats.lockRange;
        }

        float lockRadius = viewAngle;

        if(statsControl != null && statsControl.CurrentStats != null)
        {
            lockRadius = statsControl.CurrentStats.lockRadius;
        }

        float lockSpeed = 120f;

        if(statsData != null)
        {
            lockSpeed = statsData.targetLockSpeed;
        }

        if(statsControl != null && statsControl.CurrentStats != null)
        {
            lockSpeed = statsControl.CurrentStats.lockSpeed;
        }

        float distance = Vector3.Distance(transform.position, blackboard.Target.position);

        Vector3 dir = blackboard.Target.position - transform.position;

        float angle = Vector3.Angle(transform.forward, dir);

        // float lockRadius = statsControl.CurrentStats.lockRadius;

        if (angle > lockRadius)
        {
            return false;
        }
        if (distance <= lockRange)
        {
            lockTimer += Time.deltaTime * lockSpeed;

            if(lockTimer >= lockSpeed)
            {
                return true;
            }

            // if (statsControl != null && statsControl.CurrentStats != null && lockTimer >= statsControl.CurrentStats.lockSpeed)
            // {
            //     return true;
            // }
            // else if (statsControl == null && statsControl.CurrentStats == null && lockTimer >= statsData.targetLockSpeed)
            // {
            //     return true;
            // }

            return false;
        }

        lockTimer = 0;
        return false;
        // return distance <= lockRange;
    }
}