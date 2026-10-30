using UnityEngine;
using UnityEngine.UI;

public class MechLockOnUI : MonoBehaviour
{
    [SerializeField] private MechLockOn lockOn;
    [SerializeField] private RectTransform reticle;
    [SerializeField] private Image reticleImage;
    // [SerializeField] private Image lockImageUI;
    [SerializeField] private RectTransform lockImage;
    [SerializeField] private RectTransform lockArea;
    [SerializeField] private Image lockAreaImage;

    private void Start()
    {
        lockArea.anchorMin = new Vector2(0.5f, 0.5f);
        lockArea.anchorMax = new Vector2(0.5f, 0.5f);

        lockArea.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if(lockOn == null)
            return;

        if(lockOn.currentTarget == null)
        {
            lockImage.gameObject.SetActive(false);
            // return;
        }

        lockImage.gameObject.SetActive(true);

        lockImage.position = lockOn.TargetScreenPosition;

        Vector3 pos = lockOn.TargetScreenPosition;

        Debug.Log("TargetPos = " + pos);
        Debug.Log(lockOn.gameObject.name);
        Debug.Log(lockOn.currentTarget);

        lockImage.position = pos;
        reticle.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        
        if(lockOn.IsLocked)
        {
            reticleImage.color = Color.red;
        }
        else
        {
            reticleImage.color = Color.white;
        }
        // if(lockOn.IsLocked)
        // {
        //     lockAreaImage.color = Color.red;
        // }
        // else
        // {
        //     lockAreaImage.color = Color.green;
        // }
        lockArea.sizeDelta = new Vector2(lockOn.LockRadius * 2f, lockOn.LockRadius * 2f);
    }
    public void SetLockOn(MechLockOn target)
    {
        lockOn = target;
    }
    
}