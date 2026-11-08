using UnityEngine;

public enum AIState
{
    Idle, Patrol, Alert, Chase, Combat, Search
}
// public enum AIState
// {
//     Patrol, Alert, Chase, Combat, Search
// }

public class MechEnemyAI : MonoBehaviour
{
    [SerializeField] float combatDistance;
    [SerializeField] private MechEnemyStatsData statsData;

    AIState state;

    MechEnemySensor sensor;
    MechEnemyMovement movement;
    MechEnemyWeapon weapon;

    public Transform Target
    {
        get => blackboard.Target;

        set => blackboard.Target = value;
    }
    // private MechEquipment equipment;
    // private MechTotalStats stats;
    private MechStatsControl statsControl;
    private MechEnemyBlackboard blackboard;

    [SerializeField] private float alertTimer;
    [SerializeField] private float alertTime;

    private void Awake()
    {
        blackboard = GetComponent<MechEnemyBlackboard>();
        sensor = GetComponent<MechEnemySensor>();
        movement = GetComponent<MechEnemyMovement>();
        weapon = GetComponent<MechEnemyWeapon>();

        // equipment = GetComponent<MechEquipment>();
        statsControl = GetComponent<MechStatsControl>();

        state = AIState.Idle;
    }

    private void Start()
    {
        if (statsData != null)
        {
            combatDistance = statsData.combatDistance;
        }
        else
        {
            if (statsControl == null)
            {
                Debug.LogError("StatsControl Null");
                return;
            }

            if (statsControl.CurrentStats == null)
            {
                Debug.LogError("CurrentStats Null");
                return;
            }

            combatDistance = statsControl.CurrentStats.lockRange;
        }
        

        alertTimer = alertTime;
    }

    private void Update()
    {
        Debug.Log(state);
        
        switch (state)
        {
            case AIState.Idle:
                UpdateIdle();
                break;

            case AIState.Patrol:
                UpdatePatrol();
                break;

            case AIState.Alert:
                UpdateAlert();
                break;

            case AIState.Chase:
                UpdateChase();
                break;

            case AIState.Combat:
                UpdateCombat();
                break;

            case AIState.Search:
                UpdateSearch();
                break;
        }
    }

    void UpdateIdle()
    {
        Debug.Log("Idle");

        if (blackboard.CanSeeTarget)
        {
            Debug.Log("Target Found");
            state = AIState.Chase;
        }
        // if (sensor.CanSeeTarget())
        // {
        //     state = AIState.Chase;
        // }
    }
    void UpdatePatrol()
    {
        if (blackboard.CanSeeTarget)
        {
            state = AIState.Alert;
        }
    }

    void UpdateAlert()
    {
        if (alertTimer <= 0f)
        {
            state = AIState.Chase;
            alertTimer = alertTime;
        }
        else
        {
            alertTimer -= Time.deltaTime;
        }
    }

    // void UpdateChase()
    // {
    //     movement.MoveTowards(sensor.target);

    //     float dist =
    //         Vector3.Distance(transform.position, sensor.target.position);

    //     if (dist <= combatDistance)
    //     {
    //         state = AIState.Combat;
    //     }
    // }
    void UpdateChase()
    {
        Debug.Log("Chasing");

        movement.MoveTowards(blackboard.Target);

        if(blackboard.TargetDistance <= combatDistance)
        {
            state = AIState.Combat;
        }
    }

    void UpdateCombat()
    {
        Debug.Log("Combat");
        
        movement.Strafe(blackboard.Target);

        weapon.Fire();

        if (!blackboard.CanSeeTarget)
        {
            state = AIState.Search;
        }
    }
    
    void UpdateSearch()
    {
        movement.MovePosition(blackboard.LastKnownPosition);

        float dist = Vector3.Distance(transform.position, blackboard.LastKnownPosition);

        if(dist < 120f)
        {
            state = AIState.Patrol;
        }

        if(blackboard.CanSeeTarget)
        {
            state = AIState.Chase;
        }
    }
}
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public enum AIState
// {
//     Idle, Chase, Combat
// }

// public class MechEnemyAI : MonoBehaviour
// {
//     public Transform target;
//     public float moveSpeed;
//     public float detectDistance; // 50
//     public float viewAngle; // 120
//     public LayerMask obstacleMask;
//     int strafeDirection = 1;
//     AIState state = AIState.Idle;
//     // void Update()
//     // {
//     //     if(target == null)
//     //     return;

//     //     Vector3 dir = (target.position - transform.position).normalized;

//     //     transform.position += dir * moveSpeed * Time.deltaTime;
        
//     //     transform.rotation = Quaternion.LookRotation(dir);
//     // }
//     Vector3 GetStrafeDirection()
//     {
//         return transform.right * strafeDirection;
//     }
//     void Update()
//     {
//         switch(state)
//         {
//             case AIState.Idle:
//                 UpdateIdle();
//                 break;

//             case AIState.Chase:
//                 UpdateChase();
//                 break;

//             case AIState.Combat:
//                 UpdateCombat();
//                 break;
//         }
//     }
//     void UpdateIdle()
//     {
//         if(CanSeeTarget())
//         {
//             state = AIState.Chase;
//         }
//     }
//     void UpdateChase()
//     {
//         MoveTowardsTarget();

//         float dist = Vector3.Distance(transform.position, target.position);

//         if(dist < combatDistance)
//         {
//             state = AIState.Combat;
//         }
//     }
//     void UpdateCombat()
//     {
//         Vector3 lookDir = target.position - transform.position;

//         lookDir.y = 0;

//         transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 5f * Time.deltaTime);

//         StrafeMove();

//         TryAttack();
//     }
//     Rigidbody playerRb =
//     target.GetComponent<Rigidbody>();

//     Vector3 predictedPos =
//     target.position +
//     playerRb.velocity * 0.5f;

//     Vector3 fireDir =
//     (predictedPos - muzzle.position)
//     .normalized;

//     Instantiate(
//     bulletPrefab,
//     muzzle.position,
//     Quaternion.LookRotation(fireDir));
//     IEnumerator ChangeStrafe()
//     {
//         while(true)
//         {
//             yield return new WaitForSeconds(
//                 Random.Range(2f,4f));

//             strafeDirection *= -1;
//         }
//     }
//     bool CanSeeTarget()
//     {
//         if(target == null)
//             return false;

//         Vector3 dir = (target.position - transform.position);

//         float distance = dir.magnitude;

//         if(distance > detectDistance)
//             return false;

//         float angle = Vector3.Angle(transform.forward, dir);

//         if(angle > viewAngle * 0.5f)
//             return false;

//         RaycastHit hit;

//         if(Physics.Raycast(transform.position, dir.normalized, out hit, detectDistance))
//         {
//             if(hit.transform == target)
//                 return true;
//         }

//         return false;
//     }
//     bool IsObstacle(Vector3 dir)
//     {
//         return Physics.Raycast(transform.position, dir, 5f);
//     }
//     Vector3 moveDir = GetStrafeDirection();

// if(IsObstacle(moveDir))
// {
//     strafeDirection *= -1;
//     moveDir = GetStrafeDirection();
// }

// transform.position +=
//     moveDir *
//     moveSpeed *
//     Time.deltaTime;
// }
