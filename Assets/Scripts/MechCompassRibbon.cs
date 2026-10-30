using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MechCompassRibbon : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public MechCameraControl cameraControl;

    [Header("UI")]
    public RectTransform ribbonArea;
    public Text headingText;

    [Header("Enemy")]
    public GameObject enemyMarkerPrefab;
    public RectTransform enemyContainer;

    [Header("Setting")]
    public float visibleAngle = 90f;
    public float ribbonWidth = 800f;

    [Header("Compass")]
    public RectTransform tickContainer;
    public GameObject tickPrefab;
    public int tickStep = 30;
    public float pixelsPerDegree = 5f;

    private Dictionary<Transform, RectTransform> markers = new Dictionary<Transform, RectTransform>();

    private List<RectTransform> tickRects = new List<RectTransform>();

    void Start()
    {
        CreateTicks();
    }
    void UpdateTicks()
    {
        int index = 0;

        for (int angle = -360; angle <= 360; angle += tickStep)
        {
            RectTransform tick = tickRects[index];

            float degree = angle + cameraControl.Heading;

            degree = (degree % 360 + 360) % 360;

            float relativeAngle = Mathf.DeltaAngle(cameraControl.Heading, degree);

            float x = relativeAngle * pixelsPerDegree;

            tick.anchoredPosition = new Vector2(x, 0);

            Text text = tick.GetComponentInChildren<Text>();

            int d = Mathf.RoundToInt(degree);

            if(d == 0)
                text.text = "N";

            else if(d == 90)
                text.text = "E";

            else if(d == 180)
                text.text = "S";

            else if(d == 270)
                text.text = "W";

            else
                text.text = d.ToString("000");

            index++;
        }
    }
    void CreateTicks()
    {
        for (int angle = -360; angle <= 360; angle += tickStep)
        {
            GameObject obj = Instantiate(tickPrefab, tickContainer);

            tickRects.Add(obj.GetComponent<RectTransform>());
        }
    }

    void Update()
    {
        UpdateHeading();
        UpdateTicks();
        UpdateEnemies();
    }

    void UpdateHeading()
    {
        if(cameraControl == null)
        {
            Debug.LogError("cameraControl が NULL");
            return;
        }

        if(headingText == null)
        {
            Debug.LogError("headingText が NULL");
            return;
        }

        float heading = cameraControl.Heading;

        headingText.text = Mathf.RoundToInt(heading).ToString("000");
    }
    // void UpdateHeading()
    // {
    //     float heading = cameraControl.Heading;

    //     headingText.text = Mathf.RoundToInt(heading).ToString("000");
    // }

    void UpdateEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            if(!markers.ContainsKey(enemy.transform))
            {
                GameObject marker = Instantiate(enemyMarkerPrefab, enemyContainer);
                markers.Add(enemy.transform, marker.GetComponent<RectTransform>());
            }
        }
        List<Transform> removeList = new List<Transform>();
        foreach(var pair in markers)
        {
            Transform enemy = pair.Key;
            if(enemy == null)
            {
                Destroy(pair.Value.gameObject);
                removeList.Add(pair.Key);
                continue;
            }
            UpdateMarker(enemy, pair.Value);
        }
        foreach(var t in removeList)
        {
            markers.Remove(t);
        }
    }
    void UpdateMarker(Transform enemy, RectTransform marker)
    {
        Vector3 dir = enemy.position - player.position;

        dir.y = 0;

        float enemyBearing = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        if (enemyBearing < 0) enemyBearing += 360f;

        float heading = cameraControl.Heading;

        float relativeAngle = Mathf.DeltaAngle(heading, enemyBearing);

        if (Mathf.Abs(relativeAngle) > 180f)
        {
            marker.gameObject.SetActive(false);
            return;
        }

        marker.gameObject.SetActive(true);

        float x = relativeAngle * pixelsPerDegree;

        marker.anchoredPosition = new Vector2(x, 0);
    }
}