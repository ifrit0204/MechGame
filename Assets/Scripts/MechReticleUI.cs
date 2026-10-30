using UnityEngine;

public class MechReticleUI : MonoBehaviour
{
    [SerializeField] private RectTransform reticle;

    void Start()
    {
        reticle.anchorMin = new Vector2(0.5f, 0.5f);
        reticle.anchorMax = new Vector2(0.5f, 0.5f);

        reticle.anchoredPosition = Vector2.zero;
    }
}