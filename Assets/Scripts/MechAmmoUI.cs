using UnityEngine;
using UnityEngine.UI;

public class MechAmmoUI : MonoBehaviour
{
    [SerializeField] private Text rightArmText;
    [SerializeField] private Text leftArmText;
    [SerializeField] private Text rightShoulderText;
    [SerializeField] private Text leftShoulderText;

    private MechAmmoControl ammo;

    void Update()
    {
        // プレイヤーは後から生成されるので、見つかるまで探す
        if (ammo == null)
        {
            ammo = FindFirstObjectByType<MechAmmoControl>();
            if (ammo == null) return;
        }

        Show(rightArmText, "R-A", WeaponSlot.RightArm);
        Show(leftArmText, "L-A", WeaponSlot.LeftArm);
        Show(rightShoulderText, "R-S", WeaponSlot.RightShoulder);
        Show(leftShoulderText, "L-S", WeaponSlot.LeftShoulder);
    }

    private void Show(Text text, string label, WeaponSlot slot)
    {
        if (text == null) return;

        if (!ammo.UsesAmmo(slot))
        {
            text.text = label + "  --";
            return;
        }

        text.text = label + "  " + ammo.GetAmmo(slot) + " / " + ammo.GetMaxAmmo(slot);
    }
}