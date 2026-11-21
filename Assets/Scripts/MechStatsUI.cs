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
    [SerializeField] private Text boostForceText;        // 重量補正後の BOOST SPEED を表示
    [SerializeField] private Text boostEfficiencyText;
    [SerializeField] private Text weaponDamageText;
    [SerializeField] private Text ammoText;

    [Header("Texts (追加・任意)")]
    [SerializeField] private Text armorText;             // 総AP
    [SerializeField] private Text stabilityText;         // 総安定性
    [SerializeField] private Text meleeAptitudeText;     // 総近接適性
    [SerializeField] private Text hyperBoostSpeedText;   // 重量補正後のハイパーブースト速度
    [SerializeField] private Text boostJumpText;         // 重量補正後のブーストジャンプ
    [SerializeField] private Text boostPenaltyText;      // 重量によるブースト速度の低下率
    [SerializeField] private Text boostEnergyMaxText;    // EN容量
    [SerializeField] private Text energyRechargeText;    // EN回復(接地中)

    public void SetEquipment(MechEquipment equipment)
    {
        this.equipment = equipment;
    }

    // Garage / Shop のパネルが開いたときに、現在の値へ戻す
    private void OnEnable()
    {
        Refresh();
    }

    // 現在の装備構成を表示
    public void Refresh()
    {
        if (equipment == null)
            return;

        Show(equipment.CalculateTotalStats(), null);
    }

    // パーツ選択中に、付け替えた場合の値と現在との差を表示
    public void ShowPreviewStats(PartType partType, MechPartData previewPart)
    {
        if (equipment == null || previewPart == null)
            return;

        MechTotalStats current = equipment.CalculateTotalStats();
        MechTotalStats preview = equipment.CalculatePreviewStats(partType, previewPart);

        Show(preview, current);
    }

    // s を表示。c があれば c との差を色付きで出す(null なら差は出さない)
    private void Show(MechTotalStats s, MechTotalStats c)
    {
        float? C(System.Func<MechTotalStats, float> f)
        {
            return c != null ? f(c) : (float?)null;
        }

        // 重量による倍率
        float mul = equipment.GetBoostWeightMultiplier(s.weight);
        float cmul = c != null ? equipment.GetBoostWeightMultiplier(c.weight) : 1f;

        // ---- 重量・EN(小さいほうが良い) ----
        Set(weightText, "WEIGHT", s.weight, C(x => x.weight), false, "0");
        Set(energyText, "EN COST", s.energy, C(x => x.energy), false, "0");

        // ---- ブースト(重量補正後) ----
        Set(boostForceText, "BOOST SPEED", s.boostForce * mul, c != null ? c.boostForce * cmul : (float?)null, true, "0.0");
        Set(hyperBoostSpeedText, "HYPER BOOST", s.hyperBoostForce * mul, c != null ? c.hyperBoostForce * cmul : (float?)null, true, "0.0");
        Set(boostJumpText, "BOOST JUMP", s.boostJumpForce * mul, c != null ? c.boostJumpForce * cmul : (float?)null, true, "0.0");
        Set(boostPenaltyText, "BOOST WEIGHT PENALTY", (1f - mul) * 100f, c != null ? (1f - cmul) * 100f : (float?)null, false, "0.#'%'");
        Set(boostEfficiencyText, "BOOST EFFICIENCY", s.boostEfficiency, C(x => x.boostEfficiency), true, "0.0");

        // ---- EN容量・回復 ----
        Set(boostEnergyMaxText, "EN CAPACITY", s.boostEnergyMax, C(x => x.boostEnergyMax), true, "0");
        Set(energyRechargeText, "EN RECHARGE", s.energyRechargeRate, C(x => x.energyRechargeRate), true, "0.0");

        // ---- 防御・安定 ----
        Set(armorText, "ARMOR POINT", s.armor, C(x => x.armor), true, "0");
        Set(stabilityText, "STABILITY", s.stability, C(x => x.stability), true, "0.0");
        Set(meleeAptitudeText, "MELEE APTITUDE", s.meleeAptitude, C(x => x.meleeAptitude), true, "0.0");

        // ---- 機動 ----
        Set(moveSpeedText, "MOVE SPEED", s.moveSpeed, C(x => x.moveSpeed), true, "0.0");
        Set(jumpForceText, "JUMP FORCE", s.jumpForce, C(x => x.jumpForce), true, "0.0");
        Set(jumpPerformanceText, "JUMP PERFORMANCE", s.jumpPerformance, C(x => x.jumpPerformance), true, "0.0");

        // ---- 索敵・ロック ----
        Set(scanRangeText, "SCAN RANGE", s.scanRange, C(x => x.scanRange), true, "0.0");
        Set(lockDistanceText, "LOCK DISTANCE", s.lockDistance, C(x => x.lockDistance), true, "0.0");

        // ---- 武装 ----
        Set(weaponDamageText, "WEAPON DAMAGE", s.totalWeaponDamage, C(x => x.totalWeaponDamage), true, "0");
        Set(ammoText, "AMMO", s.totalAmmo, C(x => x.totalAmmo), true, "0");
    }

    private void Set(Text text, string label, float value, float? compare, bool higherIsBetter, string format)
    {
        if (text == null)
            return;

        text.supportRichText = true;
        text.text = MechStatText.Line(label, value, compare, higherIsBetter, format);
    }
}