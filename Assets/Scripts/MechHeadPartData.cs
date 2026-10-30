using UnityEngine;

// public PartType partType;
[CreateAssetMenu(menuName = "Mech/Head")]
public class MechHeadPartData : MechPartData
{
    // public string partName;
    // public GameObject modelPrefab;
    public float scanRange;
    public float lockDistance;
    // public float moveSpeed, jumpForce, jumpPerformance, boostForce, boostFirstForce, boostJumpForce, hyperBoost, hyperFirstBoost, fallSpeed, jumpCancelSpeed;
    public float jumpTime, jumpTimer, boostTime, boostTimer, boostJumpTime, boostJumpTimer, boostRechargeTime, boostRechargeTimer, energyRechargeTime, energyRechargeTimer;
    public float boostEnergy, boostEnergyMax, boostCost, boostJumpCost, hyperBoostCost, boostEfficiency, energyRechargeRate, energyRechargeRateBasic;
}