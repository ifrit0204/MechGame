using UnityEngine;
using UnityEngine.UI;

public class MechArmorUI : MonoBehaviour
{
    [SerializeField] private Text armorText;

    private MechControl mech;

    void Update()
    {
        if (armorText == null) return;

        // まだプレイヤーが生成されていなければ探す
        if (mech == null)
        {
            mech = FindFirstObjectByType<MechControl>();
            if (mech == null) return;
        }

        armorText.text = $"AP {Mathf.CeilToInt(mech.currentHP)} / {Mathf.CeilToInt(mech.maxHP)}";
    }
}