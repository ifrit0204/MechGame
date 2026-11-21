using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class MechPartInfoUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text armorPointText;
    [SerializeField] private Text stabilityPerformanceText;
    [SerializeField] private Text meleeWeaponAptitudeText;
    [SerializeField] private Text weightText;
    [SerializeField] private Text energyText;
    [SerializeField] private Text statText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private MechEquipment equipment;

    public void ShowPart(MechPartData part, PartType partType)
    {
        if (part == null)
        {
            Clear();
            return;
        }

        // 同じ枠で今装備しているパーツ
        MechPartData current = equipment != null ? equipment.GetCurrentPart(partType) : null;

        if (iconImage != null)
        {
            iconImage.sprite = part.icon;
            iconImage.enabled = (part.icon != null);
        }

        if (nameText != null)
            nameText.text = part.partName;

        if (descriptionText != null)
        {
            descriptionText.text = part.description;
        }

        // 重量・消費ENは小さいほうが良い
        if (weightText != null)
        {
            weightText.supportRichText = true;
            weightText.text = Line("WEIGHT", part.weight, current?.weight, false, "0");
        }

        if (energyText != null)
        {
            energyText.supportRichText = true;
            energyText.text = Line("EN", part.energyConsumption, current?.energyConsumption, false, "0");
        }

        if (statText == null)
            return;

        statText.supportRichText = true;

        StringBuilder sb = new StringBuilder();

        AppendPartStats(sb, part, current);

        // sb.AppendLine();

        // AppendMechPreview(sb, part, partType);

        statText.text = sb.ToString();
    }

    // "ラベル : 値 (+差)" を作る。差は良くなれば緑、悪くなれば赤
    // higherIsBetter = false のときは、値が小さくなるほうを緑にする
    private string Line(string label, float value, float? current, bool higherIsBetter = true, string format = "0.##")
    {
        string text = label + " : " + value.ToString(format);

        if (!current.HasValue)
            return text;

        float diff = value - current.Value;

        if (Mathf.Abs(diff) < 0.0001f)
            return text;

        bool good = higherIsBetter ? diff > 0f : diff < 0f;

        string color = good ? "lime" : "red";
        string sign = diff > 0f ? "+" : "-";

        return $"{text} <color={color}>({sign}{Mathf.Abs(diff).ToString(format)})</color>";
    }

    // パーツごとの性能(同じ種類の現在パーツと比較)
    private void AppendPartStats(StringBuilder sb, MechPartData part, MechPartData current)
    {
        if (part is MechHeadPartData head)
        {
            var c = current as MechHeadPartData;

            sb.AppendLine(Line("ARMOR POINT", head.armorPoint, c?.armorPoint));
            sb.AppendLine(Line("STABILITY", head.headStabilityPerformance, c?.headStabilityPerformance));
            sb.AppendLine(Line("MELEE APTITUDE", head.headMeleeWeaponAptitude, c?.headMeleeWeaponAptitude));
            sb.AppendLine(Line("SCAN RANGE", head.scanRange, c?.scanRange));
            sb.AppendLine(Line("LOCK DISTANCE", head.lockDistance, c?.lockDistance));
        }
        else if (part is MechBodyPartData body)
        {
            var c = current as MechBodyPartData;

            sb.AppendLine(Line("ARMOR POINT", body.armorPoint, c?.armorPoint));
            sb.AppendLine(Line("STABILITY", body.bodyStabilityPerformance, c?.bodyStabilityPerformance));
            sb.AppendLine(Line("MELEE APTITUDE", body.bodyMeleeWeaponAptitude, c?.bodyMeleeWeaponAptitude));
            sb.AppendLine(Line("ENERGY RECHARGE", body.bodyEnergyRecharge, c?.bodyEnergyRecharge));
            sb.AppendLine(Line("BOOST EFFICIENCY", body.bodyBoostEfficiency, c?.bodyBoostEfficiency));
        }
        else if (part is MechArmsPartData arms)
        {
            var c = current as MechArmsPartData;

            sb.AppendLine(Line("ARMOR POINT", arms.armorPoint, c?.armorPoint));
            sb.AppendLine(Line("RECOIL REDUCTION", arms.recoilReduction, c?.recoilReduction));
            sb.AppendLine(Line("ARM MOBILITY", arms.armMobility, c?.armMobility));
            sb.AppendLine(Line("ARM STABILITY", arms.armStabilityPerformance, c?.armStabilityPerformance));
            sb.AppendLine(Line("AIMING", arms.aimingPerformance, c?.aimingPerformance));
            sb.AppendLine(Line("MELEE APTITUDE", arms.armMeleeWeaponAptitude, c?.armMeleeWeaponAptitude));
        }
        else if (part is MechLegsPartData legs)
        {
            var c = current as MechLegsPartData;

            sb.AppendLine(Line("ARMOR POINT", legs.armorPoint, c?.armorPoint));
            sb.AppendLine(Line("STABILITY", legs.legStabilityPerformance, c?.legStabilityPerformance));
            sb.AppendLine(Line("MELEE APTITUDE", legs.legMeleeWeaponAptitude, c?.legMeleeWeaponAptitude));
            sb.AppendLine(Line("MOVE SPEED", legs.moveSpeed, c?.moveSpeed));
            sb.AppendLine(Line("JUMP FORCE", legs.jumpForce, c?.jumpForce));
            sb.AppendLine(Line("JUMP PERFORMANCE", legs.jumpPerformance, c?.jumpPerformance));
            sb.AppendLine(Line("GROUND MULTIPLIER", legs.groundMultiplier, c?.groundMultiplier));
        }
        else if (part is MechBoosterPartData booster)
        {
            var c = current as MechBoosterPartData;

            sb.AppendLine(Line("BOOST FORCE", booster.boostForce, c?.boostForce));
            sb.AppendLine(Line("BOOST FIRST FORCE", booster.boostFirstForce, c?.boostFirstForce));
            sb.AppendLine(Line("BOOST COST", booster.boostCost, c?.boostCost, false));
            sb.AppendLine(Line("BOOST EFFICIENCY", booster.boostEfficiency, c?.boostEfficiency));
            sb.AppendLine(Line("BOOST JUMP FORCE", booster.boostJumpForce, c?.boostJumpForce));
            sb.AppendLine(Line("BOOST JUMP COST", booster.boostJumpCost, c?.boostJumpCost, false));
            sb.AppendLine(Line("HYPER BOOST FORCE", booster.hyperBoostForce, c?.hyperBoostForce));
            sb.AppendLine(Line("HYPER BOOST COST", booster.hyperBoostCost, c?.hyperBoostCost, false));
        }
        else if (part is MechFCSPartData fcs)
        {
            var c = current as MechFCSPartData;

            sb.AppendLine(Line("LOCK RANGE", fcs.lockRange, c?.lockRange));
            sb.AppendLine(Line("LOCK SPEED", fcs.lockSpeed, c?.lockSpeed));
            sb.AppendLine(Line("LOCK RADIUS", fcs.lockRadius, c?.lockRadius));
        }
        else if (part is MechGeneratorPartData gen)
        {
            var c = current as MechGeneratorPartData;

            sb.AppendLine(Line("EN MAX", gen.boostEnergyMax, c?.boostEnergyMax));
            sb.AppendLine(Line("RECHARGE (GROUND)", gen.energyRechargeRate, c?.energyRechargeRate));
            sb.AppendLine(Line("RECHARGE (AIR)", gen.energyRechargeRateBasic, c?.energyRechargeRateBasic));
        }
        else if (part is MechWeaponPartData weapon)
        {
            var c = current as MechWeaponPartData;

            sb.AppendLine(Line("DAMAGE", weapon.damageData.Damage, c != null ? c.damageData.Damage : (float?)null));
            sb.AppendLine(Line("AMMO", weapon.ammo, c?.ammo, true, "0"));
            sb.AppendLine(Line("RELOAD", weapon.reloadTime, c?.reloadTime, false));
            sb.AppendLine(Line("SPREAD", weapon.spread, c?.spread, false));
            sb.AppendLine(Line("RANGE", weapon.lockDistance, c?.lockDistance));
            sb.AppendLine(Line("BULLET SPEED", weapon.bulletSpeed, c?.bulletSpeed));
        }
    }

    // このパーツに付け替えたあとの、機体全体の値(重量補正後のブースト速度など)
    private void AppendMechPreview(StringBuilder sb, MechPartData part, PartType partType)
    {
        if (equipment == null)
            return;

        float currentWeight = equipment.CalculateTotalStats().weight;
        float newWeight = equipment.GetPreviewTotalWeight(partType, part);

        float currentMul = equipment.GetBoostWeightMultiplier(currentWeight);
        float newMul = equipment.GetBoostWeightMultiplier(newWeight);

        MechBoosterPartData currentBooster = equipment.booster;

        // ブースターを選んでいるときはそのブースター、それ以外は今のブースターで計算
        MechBoosterPartData newBooster = partType == PartType.Booster ? part as MechBoosterPartData : currentBooster;

        sb.AppendLine("<b>- AFTER EQUIP -</b>");
        sb.AppendLine(Line("TOTAL WEIGHT", newWeight, currentWeight, false, "0"));

        if (newBooster == null)
            return;

        float? curBoost = currentBooster != null ? currentBooster.boostForce * currentMul : (float?)null;
        float? curHyper = currentBooster != null ? currentBooster.hyperBoostForce * currentMul : (float?)null;
        float? curJump  = currentBooster != null ? currentBooster.boostJumpForce * currentMul : (float?)null;

        sb.AppendLine(Line("BOOST SPEED", newBooster.boostForce * newMul, curBoost));
        sb.AppendLine(Line("HYPER BOOST SPEED", newBooster.hyperBoostForce * newMul, curHyper));
        sb.AppendLine(Line("BOOST JUMP", newBooster.boostJumpForce * newMul, curJump));
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
    // private string DiffText(float diff)
    // {
    //     if(diff == 0)
    //         return "";

    //     string color = diff > 0 ? "lime" : "red";

    //     return
    //         $"<color={color}> {diff:+0;-0}</color>";
    // }
    // private string WeightDiffText(float diff)
    // {
    //     if(diff == 0)
    //         return "";

    //     string color = diff < 0 ? "lime" : "red";

    //     return
    //         $"<color={color}> {diff:+0;-0}</color>";
    // }
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

        if (descriptionText != null)
        {
            descriptionText.text = "";
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
