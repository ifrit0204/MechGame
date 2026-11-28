using UnityEngine;

// 敵の MechEnemyLoadout から能力値を計算して、敵側に渡す。
// プレイヤーの MechControl.ApplyParts() の敵版にあたる。
//
// 注意: MechEnemyUnitControl / MechEnemyMovement / MechEnemyWeapon などの中身は
// まだ見ていないので、「どの値をどこに入れるか」は下の Apply() の最後にコメントで示した。
// 実際の変数名に合わせて、数行つなぎ直してほしい。
public class MechEnemyStatsApplier : MonoBehaviour
{
    [SerializeField] private MechEnemyLoadout loadout;

    [Header("Weight (MechEquipment と同じ値にそろえる)")]
    [SerializeField] private float weightReference = 100000f;
    [SerializeField, Range(0f, 0.9f)] private float maxBoostWeightPenalty = 0.3f;

    public MechTotalStats Stats { get; private set; }

    // 計算済みの値(敵の各スクリプトはここから読む)
    public float MaxHP { get; private set; }
    public float MoveSpeed { get; private set; }
    public float GroundMultiplier { get; private set; }
    public float BoostForce { get; private set; }
    public float BoostFirstForce { get; private set; }
    public float BoostCost { get; private set; }
    public float BoostEnergyMax { get; private set; }
    public float LockRange { get; private set; }

    public MechEnemyLoadout Loadout { get { return loadout; } }

    // アリーナの敵なら MechArenaEnemyData.loadout を渡す。
    // インスペクターで loadout を設定済みなら、引数なしで呼べる。
    public void Apply(MechEnemyLoadout newLoadout = null)
    {
        if (newLoadout != null)
            loadout = newLoadout;

        if (loadout == null)
        {
            Debug.LogWarning("MechEnemyStatsApplier: loadout が未設定です", this);
            return;
        }

        Stats = MechStatsCalculator.Calculate(loadout);

        // 重いほどブースト速度が下がる(プレイヤーと同じ式)
        float ratio = Mathf.Clamp01(Stats.weight / weightReference);
        float boostWeightMultiplier = 1f - ratio * maxBoostWeightPenalty;

        MaxHP = Stats.armor;
        MoveSpeed = Stats.moveSpeed;
        GroundMultiplier = Stats.groundMultiplier;
        BoostForce = Stats.boostForce * boostWeightMultiplier;
        BoostFirstForce = Stats.boostFirstForce * boostWeightMultiplier;
        BoostCost = Stats.boostCost;
        BoostEnergyMax = Stats.boostEnergyMax;
        LockRange = Stats.lockDistance;

        // ここから先は、敵の既存スクリプトに合わせてつなぐ(例):
        //
        //   unit.maxHP = MaxHP;  unit.currentHP = MaxHP;
        //   movement.moveSpeed = MoveSpeed * GroundMultiplier;
        //   movement.boostForce = BoostForce;
        //   sensor.range = LockRange;
        //   weapon.SetWeapons(loadout.RightWeapon, loadout.LeftWeapon,
        //                     loadout.RightShoulder, loadout.LeftShoulder);
    }
}
