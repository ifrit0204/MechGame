using UnityEngine;

public class MechLockOn : MonoBehaviour
{
    public Transform currentTarget;

    [SerializeField] private float lockRange = 500f;

    [SerializeField] private float lockRadius = 99999f;

    private Camera mainCam;
    private MechEquipment equipment;

    private void Start()
    {
        mainCam = Camera.main;
        equipment = GetComponent<MechEquipment>();

        Debug.Log(mainCam);
        if(equipment != null)
        {
            if(equipment.head != null)
            {
                lockRange = equipment.head.lockDistance;
            }

            if(equipment.FCS != null)
            {
                lockRadius = equipment.FCS.lockRadius;
            }
        }
        // if (equipment != null && equipment.head != null)
        // {
        //     lockRange = equipment.head.lockDistance;
        // }
        // if(equipment.fcs != null)
        // {
        //     lockRadius = equipment.fcs.lockRadius;
        // }
        // lockRange = equipment.head.lockDistance;
        // lockImage.position = lockOn.TargetScreenPosition;
    }

    private void Update()
    {
        FindTargetInScreenCenter();
        Debug.Log(currentTarget);
        Debug.Log(gameObject.name);
    }

    private void FindTargetInScreenCenter()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform bestTarget = null;

        float bestScore = float.MaxValue;

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        foreach(GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if(distance > lockRange)
                continue;

            Vector3 screenPos = mainCam.WorldToScreenPoint(enemy.transform.position);

            if(screenPos.z < 0)
                continue;

            float centerDistance = Vector2.Distance(screenCenter, new Vector2(screenPos.x, screenPos.y));

            if(centerDistance > lockRadius)
                continue;

            if(centerDistance < bestScore)
            {
                bestScore = centerDistance;
                bestTarget = enemy.transform;
            }
        }
        if(bestTarget == null)
        {
            Debug.Log(TargetScreenPosition);
            Debug.Log("Target Lost");
        }
        // Debug.Log(lockOn.TargetScreenPosition);

        currentTarget = bestTarget;
    }
    // public Vector3 TargetScreenPosition
    // {
    //     get
    //     {
    //         if(currentTarget == null)
    //             return Vector3.zero;

    //         return mainCam.WorldToScreenPoint(currentTarget.position);
    //     }
    // }
    // public Vector3 TargetScreenPosition
    // {
    //     get
    //     {
    //         if(currentTarget == null)
    //             return Vector3.zero;

    //         return mainCam.WorldToScreenPoint(currentTarget.position + Vector3.up * 3f);
    //     }
    // }
    public Vector3 TargetScreenPosition
    {
        get
        {
            Debug.Log("CurrentTarget = " + currentTarget);
            Debug.Log("MainCam = " + mainCam);

            if(currentTarget == null)
                return Vector3.zero;

            Vector3 pos = mainCam.WorldToScreenPoint(currentTarget.position + Vector3.up * 3f);

            Debug.Log("ScreenPos = " + pos);

            return pos;
        }
    }
    public bool IsLocked
    {
        get
        {
            return currentTarget != null;
        }
    }
    public float LockRadius
    {
        get
        {
            return lockRadius;
        }
    }
}