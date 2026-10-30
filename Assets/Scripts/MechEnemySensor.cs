using UnityEngine;

public class MechEnemySensor : MonoBehaviour
{
    // public Transform target;

    [SerializeField] private float detectDistance = 50f;

    [SerializeField] private float viewAngle = 120f;

    private MechEnemyBlackboard blackboard;

    private void Awake()
    {
        blackboard = GetComponent<MechEnemyBlackboard>();
    }

    private void Update()
    {
        if(blackboard.Target == null)
            return;

        bool visible = CanSeeTarget();

        blackboard.CanSeeTarget = visible;

        blackboard.TargetDistance =
            Vector3.Distance(
                transform.position,
                blackboard.Target.position);

        if(visible)
        {
            blackboard.LastKnownPosition =
                blackboard.Target.position;
        }
    }

    public bool CanSeeTarget()
    {
        if (blackboard.Target == null)
            return false;

        Vector3 dir = blackboard.Target.position - transform.position;

        float distance = dir.magnitude;

        if (distance > detectDistance)
            return false;

        float angle = Vector3.Angle(transform.forward, dir);

        if (angle > viewAngle * 0.5f)
            return false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir.normalized, out hit, detectDistance))
        {
            return hit.transform == blackboard.Target;
        }

        return false;
    }
}