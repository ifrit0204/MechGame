using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechBoostBar : MonoBehaviour
{
    public Slider boostSlider;
    public MechControl mech;

    void Update()
    {
        boostSlider.value = mech.boostEnergy / mech.boostEnergyMax;
    }
}
