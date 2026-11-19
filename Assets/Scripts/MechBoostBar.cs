using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechBoostBar : MonoBehaviour
{
    public Slider boostSlider;
    public MechControl mech;
    [SerializeField] private float smoothSpeed = 5f;

    public void SetMech(MechControl target)
    {
        mech = target;

        // 切り替え直後にバーが飛ばないよう、現在値に合わせておく
        if (mech != null && mech.boostEnergyMax > 0f)
            boostSlider.value = mech.boostEnergy / mech.boostEnergyMax;
    }

    void Update()
    {
        if (mech == null || mech.boostEnergyMax <= 0f)
            return;

        float target = mech.boostEnergy / mech.boostEnergyMax;

        boostSlider.value = Mathf.MoveTowards(boostSlider.value, target, smoothSpeed * Time.deltaTime);
    }
}
