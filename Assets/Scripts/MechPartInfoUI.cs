using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechPartInfoUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text weightText;
    [SerializeField] private Text energyText;

    [SerializeField] private Text statText;

    [SerializeField] private MechEquipment equipment;

    public void ShowPart(MechPartData part)
    {
        if (part == null)
        {
            Clear();
            return;
        }

        Debug.Log("ShowPart : " + part.partName);

        if (iconImage != null)
        {
            iconImage.sprite = part.icon;
            iconImage.enabled = (part.icon != null);
        }

        if (nameText != null)
        {
            nameText.text = part.partName;
        }

        if (weightText != null)
        {
            weightText.text =
                "WEIGHT : " + part.weight.ToString("0");
        }

        if (energyText != null)
        {
            energyText.text =
                "EN : " + part.energyConsumption.ToString("0");
        }

        statText.text = "";

        // if(part is MechHeadPartData head)
        // {
        //     statText.text = "SCAN RANGE : " + head.scanRange + "\nLOCK DISTANCE : " + head.lockDistance;
        // }
        if(part is MechHeadPartData head)
        {
            MechHeadPartData current =
                equipment != null ?
                equipment.head :
                null;

            float scanDiff =
                head.scanRange -
                (current != null ?
                current.scanRange : 0);

            float lockDiff =
                head.lockDistance -
                (current != null ?
                current.lockDistance : 0);

            statText.supportRichText = true;

            statText.text =
                $"SCAN RANGE : {head.scanRange} {DiffText(scanDiff)}\n" +
                $"LOCK DISTANCE : {head.lockDistance} {DiffText(lockDiff)}";
        }

        else if(part is MechArmsPartData arms)
        {
            statText.text = "SCAN RANGE : " + arms.scanRange + "\nLOCK DISTANCE : " + arms.lockDistance;
        }
        // else if(part is MechLegsPartData legs)
        // {
        //     statText.text = "MOVE SPEED : " + legs.moveSpeed + "\nJUMP FORCE : " + legs.jumpForce + "\nJUMP PERFORMANCE : " + legs.jumpPerformance;
        // }
        // else if (part is MechLegsPartData legs)
        // {
        //     MechLegsPartData current =
        //         equipment.legs;

        //     statText.text =
        //         $"MOVE : {legs.moveSpeed}\n" +
        //         $"JUMP : {legs.jumpForce}\n" +
        //         $"PERFORMANCE : {legs.jumpPerformance}";
        // }
        else if(part is MechLegsPartData legs)
        {
            MechLegsPartData current =
                equipment.legs;

            statText.text =
                $"MOVE : {legs.moveSpeed}"
                + DiffText(
                    legs.moveSpeed -
                    current.moveSpeed) + "\n"

                + $"JUMP : {legs.jumpForce}"
                + DiffText(
                    legs.jumpForce -
                    current.jumpForce) + "\n"

                + $"PERFORMANCE : {legs.jumpPerformance}"
                + DiffText(
                    legs.jumpPerformance -
                    current.jumpPerformance);
        }
        else if(part is MechBoosterPartData booster)
        {
            MechBoosterPartData current = equipment.booster;

            float boostDiff =
                booster.boostForce -
                (current != null ?
                current.boostForce : 0);

            float costDiff =
                booster.boostCost -
                (current != null ?
                current.boostCost : 0);

            statText.text =
                $"BOOST FORCE : {booster.boostForce} {DiffText(boostDiff)}\n" +
                $"BOOST COST : {booster.boostCost} {DiffText(-costDiff)}";
            // statText.text = "BOOST FORCE : " + booster.boostForce + "\nBOOST COST : " + booster.boostCost + "\nEFFICIENCY : " + booster.boostEfficiency;
        }
        else if(part is MechGeneratorPartData generator)
        {
            statText.text = "EN MAX : " + generator.boostEnergyMax + "\nRECHARGE : " + generator.energyRechargeRate;
        }
        else if(part is MechWeaponPartData weapon)
        {
            statText.text = "DAMAGE : " + weapon.damageData.Damage + "\nAMMO : " + weapon.ammo + "\nRELOAD : " + weapon.reloadTime + "\nRANGE : " + weapon.lockDistance;
        }
        // Debug.Log("InfoUI : " + part.partName);

        // if(nameText != null)
        // {
        //     Debug.Log("NameText Found");
        // }

        // nameText.text = part.partName;
        // if (part == null)
        // {
        //     Clear();
        //     return;
        // }

        // Debug.Log("ShowPart : " + part.partName);

        // if(iconImage != null)
        // {
        //     iconImage.sprite = part.icon;
        //     iconImage.enabled = part.icon != null;
        // }

        // if(nameText != null)
        //     nameText.text = part.partName;

        // if(weightText != null)
        //     weightText.text = "WEIGHT : " + part.weight;

        // if(energyText != null)
        //     energyText.text = "EN : " + part.energyConsumption;

        // statText.text = "";
        // Debug.Log("InfoUI : " + part.partName);
        // if (part == null)
        // {
        //     return;
        // }

        // if (iconImage != null)
        // {
        //     iconImage.sprite = part.icon;
        //     iconImage.enabled = part.icon != null;
        // }

        // nameText.text = part.partName;

        // weightText.text = "WEIGHT : " + part.weight.ToString("0");

        // energyText.text = "EN : " + part.energyConsumption.ToString("0");

        // statText.text = "";
        // if (part is MechHeadPartData head)
        // {
        //     statText.text = "Scan Range : " + head.scanRange + "\nLock Distance : " + head.lockDistance;
        // }
        // else if (part is MechArmsPartData arms)
        // {
        //     statText.text = "Scan Range : " + arms.scanRange + "\nLock Distance : " + arms.lockDistance;
        // }
        // else if (part is MechLegsPartData legs)
        // {
        //     statText.text = "Move Speed : " + legs.moveSpeed + "\nJump Force : " + legs.jumpForce + "\nJump Performance : " + legs.jumpPerformance;
        // }
        // else if (part is MechBoosterPartData booster)
        // {
        //     statText.text = "Boost Force : " + booster.boostForce + "\nBoost Cost : " + booster.boostCost + "\nEfficiency : " + booster.boostEfficiency;
        // }
        // else if (part is MechGeneratorPartData generator)
        // {
        //     statText.text = "EN Max : " + generator.boostEnergyMax + "\nRecharge : " + generator.energyRechargeRate;
        // }
        // else if (part is MechWeaponPartData weapon)
        // {
        //     statText.text = "Damage : " + weapon.damageData.Damage + "\nAmmo : " + weapon.ammo + "\nReload : " + weapon.reloadTime + "\nRange : " + weapon.lockDistance;
        // }
    }
    private string DiffColor(float diff)
    {
        if(diff > 0)
            return "lime";

        if(diff < 0)
            return "red";

        return "white";
    }
    // private string DiffText(float diff)
    // {
    //     return $"<color={DiffColor(diff)}>" +
    //         $"({diff:+#;-#;0})" +
    //         $"</color>";
    // }
    private string DiffText(float diff)
    {
        if(diff == 0)
            return "";

        string color =
            diff > 0 ? "lime" : "red";

        return
            $"<color={color}> {diff:+0;-0}</color>";
    }
    private string WeightDiffText(float diff)
    {
        if(diff == 0)
            return "";

        string color = diff < 0 ? "lime" : "red";

        return
            $"<color={color}> {diff:+0;-0}</color>";
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Clear()
    {
        if(iconImage != null)
        {
            iconImage.sprite = null;
        }

        if(nameText != null)
        {
            nameText.text = "-";
        }

        if(weightText != null)
        {
            weightText.text = "-";
        }

        if(energyText != null)
        {
            energyText.text = "-";
        }

        if(statText != null)
        {
            statText.text = "";
        }
    }
}
