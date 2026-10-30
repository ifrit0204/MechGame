using UnityEngine;
using UnityEngine.UI;

public class MechStatsUI : MonoBehaviour
{
    [SerializeField]
    private MechEquipment equipment;


    [Header("Texts")]
    [SerializeField] private Text weightText;

    [SerializeField] private Text energyText;

    [SerializeField] private Text moveSpeedText;

    [SerializeField] private Text jumpForceText;

    [SerializeField] private Text jumpPerformanceText;

    [SerializeField] private Text scanRangeText;

    [SerializeField] private Text lockDistanceText;

    [SerializeField] private Text boostForceText;

    [SerializeField] private Text boostEfficiencyText;

    [SerializeField] private Text weaponDamageText;

    [SerializeField] private Text ammoText;


    public void SetEquipment(MechEquipment equipment)
    {
        this.equipment = equipment;
    }


    public void Refresh()
    {
        if (equipment == null)
        {
            return;
        }


        MechTotalStats stats = equipment.CalculateTotalStats();


        weightText.text = "WEIGHT : " + stats.weight.ToString("0");


        energyText.text = "EN : " + stats.energy.ToString("0");


        moveSpeedText.text = "MOVE SPEED : " + stats.moveSpeed.ToString("0.0");


        jumpForceText.text = "JUMP FORCE : " + stats.jumpForce.ToString("0.0");


        jumpPerformanceText.text = "JUMP PERFORMANCE : " + stats.jumpPerformance.ToString("0.0");


        scanRangeText.text = "SCAN RANGE : " + stats.scanRange.ToString("0.0");


        lockDistanceText.text = "LOCK DISTANCE : " + stats.lockDistance.ToString("0.0");


        boostForceText.text = "BOOST FORCE : " + stats.boostForce.ToString("0.0");


        boostEfficiencyText.text = "BOOST EFFICIENCY : " + stats.boostEfficiency.ToString("0.0");


        weaponDamageText.text = "WEAPON DAMAGE : " + stats.totalWeaponDamage.ToString("0");


        ammoText.text = "AMMO : " + stats.totalAmmo.ToString("0");
    }
    public void ShowPreviewStats(PartType partType, MechPartData previewPart)
    {
        MechTotalStats current = equipment.CalculateTotalStats();

        // MechTotalStats preview = equipment.CalculatePreviewStats(partType, previewPart);
        MechLoadout temp = MechGameManager.Instance.CurrentLoadout.Clone();
    }
    // public void ShowPreviewStats(PartType partType, MechPartData previewPart)
    // {
    //     if(equipment == null)
    //         return;

    //     MechTotalStats current =
    //         equipment.CalculateTotalStats();

    //     MechLoadout temp =
    //         MechGameManager.Instance
    //         .CurrentLoadout
    //         .Clone();

    //     switch(partType)
    //     {
    //         case PartType.Head:
    //             temp.Head =
    //                 (MechHeadPartData)previewPart;
    //             break;

    //         case PartType.Body:
    //             temp.Body =
    //                 (MechBodyPartData)previewPart;
    //             break;

    //         case PartType.Arms:
    //             temp.Arms =
    //                 (MechArmsPartData)previewPart;
    //             break;

    //         case PartType.Legs:
    //             temp.Legs =
    //                 (MechLegsPartData)previewPart;
    //             break;

    //         case PartType.Booster:
    //             temp.Booster =
    //                 (MechBoosterPartData)previewPart;
    //             break;

    //         case PartType.Generator:
    //             temp.Generator =
    //                 (MechGeneratorPartData)previewPart;
    //             break;

    //         case PartType.FCS:
    //             temp.FCS =
    //                 (MechFCSPartData)previewPart;
    //             break;
    //     }

    //     MechTotalStats preview =
    //         equipment.CalculateStatsFromLoadout(temp);

    //     ShowDiff(current, preview);
    // }
    private void ShowDiff(MechTotalStats current, MechTotalStats preview)
    {
        weightText.text =
            $"WEIGHT : {preview.weight:0}" +
            Diff(preview.weight - current.weight);

        energyText.text =
            $"EN : {preview.energy:0}" +
            Diff(preview.energy - current.energy);

        moveSpeedText.text =
            $"MOVE : {preview.moveSpeed:0}" +
            Diff(preview.moveSpeed - current.moveSpeed);

        jumpForceText.text =
            $"JUMP : {preview.jumpForce:0}" +
            Diff(preview.jumpForce - current.jumpForce);

        scanRangeText.text =
            $"SCAN : {preview.scanRange:0}" +
            Diff(preview.scanRange - current.scanRange);

        lockDistanceText.text =
            $"LOCK : {preview.lockDistance:0}" +
            Diff(preview.lockDistance - current.lockDistance);
    }
    private string Diff(float diff)
    {
        if(diff == 0)
            return "";

        string color =
            diff > 0 ?
            "lime" :
            "red";

        return
            $" <color={color}>({diff:+0;-0})</color>";
    }
}
