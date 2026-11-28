using UnityEngine;

public enum AIState
{
    Idle, Patrol, Alert, Chase, Combat, Search
}

public enum CombatAction
{
    None, Approach, Orbit, Retreat, SideBoost, GainAltitude, BehindMove, MeleeAttack,
    // 主導権を握ったときの突撃(ブースト接近 → 近接攻撃 → 離脱)
    Assault
}

public enum AIPersonality
{
    Aggressive, Balanced, Defensive, Sniper, Duelist
}

public class MechEnemyAI : MonoBehaviour
{
    [SerializeField] float combatDistance;
    [SerializeField] private MechEnemyStatsData statsData;

    AIState state;

    MechEnemySensor sensor;
    MechEnemyMovement movement;
    MechEnemyWeapon weapon;

    public Transform Target
    {
        get => blackboard.Target;

        set => blackboard.Target = value;
    }

    private float statsRegenPerSec = -1f;

    private float actionLockTimer;
    [SerializeField] private float minActionDuration = 1.5f;

    [SerializeField] private AIPersonality personality;

    [SerializeField] float reactionSpeed = 1f;

    [SerializeField] float courage = 1f;

    [SerializeField] float accuracy = 1f;

    private MechStatsControl statsControl;
    private MechEnemyBlackboard blackboard;
    private MechEnemyUnitControl unitControl;

    [SerializeField] private float alertTimer;
    [SerializeField] private float alertTime;
    private float actionTimer;
    CombatAction currentAction;
    public float thinkTimer;
    public float thinkInterval = 2f;

    public float retreatScore;
    public float orbitScore;
    public float approachScore;
    public float altitudeScore;
    public float behindScore;
    public float meleeScore;
    public float sideBoostScore;

    // ------------------------------------------------------------
    // 思考の設定(AI Profile)
    //
    // 優先順位: Loadout の aiProfile > StatsData の aiProfile > このインスペクターの値
    // Profile が指定されていない敵は、これまで通りインスペクターの値で動く。
    // ------------------------------------------------------------

    private bool profileApplied;

    [Header("Initiative / Assault (主導権と突撃)")]
    [SerializeField] private bool allowAssault = true;
    // 隙があったときに仕掛ける確率。-1なら性格の既定値を使う
    [SerializeField] private float assaultWillingness = -1f;
    // combatDistance × この値 以内なら突撃を始められる
    [SerializeField] private float assaultStartRate = 1.0f;
    // 突撃の最大継続時間(秒)
    [SerializeField] private float assaultMaxDuration = 3f;
    // 突撃が終わってから、次の突撃までの時間(秒)
    [SerializeField] private float assaultCooldown = 4f;
    // この時間プレイヤーに捕捉され続け、まだ遠いなら突撃を中止する(秒)
    [SerializeField] private float abortCaptureTime = 1f;
    // HP割合がこれ未満(courageで補正)なら突撃しない
    [SerializeField] private float assaultMinHpRate = 0.35f;
    // 突撃のブースト1回あたりのエネルギー消費
    [SerializeField] private float assaultBoostCost = 15f;
    // 通常の接近(Approach)のブースト1回あたりのエネルギー消費
    [SerializeField] private float approachBoostCost = 10f;
    // この速度以上で接近しているとき、近接射程に入ったらその勢いで攻撃する
    [SerializeField] private float dashAttackSpeed = 15f;
    // 主導権がないとき、「攻めの接近」スコアを何割だけ反映するか
    [SerializeField] private float disadvantageApproachRate = 0.15f;

    [Header("Action Weights (行動の好み。1が標準)")]
    [SerializeField] private float retreatWeight = 1f;
    [SerializeField] private float orbitWeight = 1f;
    [SerializeField] private float approachWeight = 1f;
    [SerializeField] private float altitudeWeight = 1f;
    [SerializeField] private float behindWeight = 1f;
    [SerializeField] private float sideBoostWeight = 1f;

    [Header("Resource (回復・減衰)")]
    [SerializeField] private float boostRegenPerSec = 20f;
    [SerializeField] private float pressureDecayPerSec = 30f;
    [SerializeField] private float aggressionDecayPerSec = 3f;

    private float assaultEndTime;
    private float nextAssaultTime;
    private float nextOpportunityCheck;
    private float capturedTimer;
    private bool assaultSwung;

    // 近接武器の情報(毎フレーム1回だけ更新)
    private bool hasMelee;
    private float meleeRange;

    private void Awake()
    {
        blackboard = GetComponent<MechEnemyBlackboard>();
        sensor = GetComponent<MechEnemySensor>();
        movement = GetComponent<MechEnemyMovement>();
        weapon = GetComponent<MechEnemyWeapon>();
        unitControl = GetComponent<MechEnemyUnitControl>();

        statsControl = GetComponent<MechStatsControl>();

        state = AIState.Idle;
    }

    // 思考の設定を適用する。
    // 生成直後(Startより前)に Loadout の Profile を渡すと、StatsData の Profile より優先される。
    public void ApplyProfile(MechEnemyAIProfile profile)
    {
        if (profile == null)
            return;

        personality = profile.personality;
        reactionSpeed = profile.reactionSpeed;
        courage = profile.courage;
        accuracy = profile.accuracy;
        minActionDuration = profile.minActionDuration;

        allowAssault = profile.allowAssault;
        assaultWillingness = profile.assaultWillingness;
        assaultStartRate = profile.assaultStartRate;
        assaultMaxDuration = profile.assaultMaxDuration;
        assaultCooldown = profile.assaultCooldown;
        abortCaptureTime = profile.abortCaptureTime;
        assaultMinHpRate = profile.assaultMinHpRate;
        assaultBoostCost = profile.assaultBoostCost;
        approachBoostCost = profile.approachBoostCost;
        dashAttackSpeed = profile.dashAttackSpeed;
        disadvantageApproachRate = profile.disadvantageApproachRate;

        retreatWeight = profile.retreatWeight;
        orbitWeight = profile.orbitWeight;
        approachWeight = profile.approachWeight;
        altitudeWeight = profile.altitudeWeight;
        behindWeight = profile.behindWeight;
        sideBoostWeight = profile.sideBoostWeight;

        boostRegenPerSec = profile.boostRegenPerSec;
        pressureDecayPerSec = profile.pressureDecayPerSec;
        aggressionDecayPerSec = profile.aggressionDecayPerSec;

        profileApplied = true;

        if (weapon != null)
            weapon.accuracy = accuracy;
        
        if (movement != null)
            movement.ApplyApproachSettings(profile.approach);
    }

    public void ApplyStats(MechTotalStats stats, float regenPerSec)
    {
        if (stats == null || blackboard == null)
            return;

        if (stats.boostEnergyMax > 0f)
        {
            blackboard.MaxBoostEnergy = stats.boostEnergyMax;
            blackboard.BoostEnergy = stats.boostEnergyMax;
        }

        if (regenPerSec > 0f)
            statsRegenPerSec = regenPerSec;
    }

    private void Start()
    {
        // Loadout 側から適用されていなければ、StatsData の Profile を使う
        if (!profileApplied && statsData != null && statsData.aiProfile != null)
        {
            ApplyProfile(statsData.aiProfile);
        }

        MechEnemyControl enemyControl = GetComponent<MechEnemyControl>();

        if (enemyControl != null)
            ApplyStats(enemyControl.Stats, enemyControl.EnergyRechargeRate);

        if (statsData != null)
        {
            combatDistance = statsData.combatDistance;
        }
        else
        {
            if (statsControl == null)
            {
                Debug.LogError("StatsControl Null");
                return;
            }

            if (statsControl.CurrentStats == null)
            {
                Debug.LogError("CurrentStats Null");
                return;
            }

            combatDistance = statsControl.CurrentStats.lockRange;
        }

        alertTimer = alertTime;

        weapon.accuracy = accuracy;

    }

    private void Update()
    {
        switch (state)
        {
            case AIState.Idle:
                UpdateIdle();
                break;

            case AIState.Patrol:
                UpdatePatrol();
                break;

            case AIState.Alert:
                UpdateAlert();
                break;

            case AIState.Chase:
                UpdateChase();
                break;

            case AIState.Combat:
                UpdateCombat();
                break;

            case AIState.Search:
                UpdateSearch();
                break;
        }
    }

    void UpdateIdle()
    {
        if (blackboard.CanSeeTarget)
        {
            Debug.Log("Target Found");
            state = AIState.Chase;
        }
    }

    void UpdatePatrol()
    {
        if (blackboard.CanSeeTarget)
        {
            state = AIState.Alert;
        }
    }

    void UpdateAlert()
    {
        if (alertTimer <= 0f)
        {
            state = AIState.Chase;
            alertTimer = alertTime;
        }
        else
        {
            alertTimer -= Time.deltaTime;
        }
    }

    void UpdateChase()
    {
        movement.LookAtTarget(blackboard.Target);

        movement.MoveTowards(blackboard.Target);

        if (blackboard.TargetDistance <= combatDistance)
        {
            state = AIState.Combat;
        }
    }

    // ------------------------------------------------------------
    // Combat
    // ------------------------------------------------------------

    void UpdateCombat()
    {
        weapon.AimWeapon(blackboard.Target);

        // 毎フレームの共通更新
        hasMelee = weapon.HasMeleeWeapon;
        meleeRange = hasMelee ? weapon.MeleeAttackRange : 0f;

        RegenerateResources();
        TrackCapture();

        // 隙(主導権)ができたら、思考間隔を待たずに突撃を始める
        CheckAssaultOpportunity();

        if (currentAction == CombatAction.Assault)
        {
            UpdateAssault();
        }

        // 突撃中は通常の行動選択を止める(途中で別の行動に切り替わらない = 芯)
        if (currentAction != CombatAction.Assault)
        {
            actionLockTimer -= Time.deltaTime;

            thinkTimer -= Time.deltaTime;

            if (thinkTimer <= 0f && actionLockTimer <= 0f)
            {
                DecideAction();

                Debug.Log("Action = " + currentAction);

                thinkTimer = Mathf.Max(1.0f, 2f / reactionSpeed);

                actionLockTimer = minActionDuration;
            }
        }

        if (currentAction == CombatAction.GainAltitude)
        {
            ExecuteAction();
            return;
        }

        ExecuteAction();

        if (currentAction != CombatAction.MeleeAttack)
            weapon.Fire();
    }

    // ブースト・圧力・攻撃性の回復/減衰
    // (これがないとブーストが一度空になると戻らず、攻撃性は上がる一方になる)
    void RegenerateResources()
    {
        float regen = statsRegenPerSec > 0f ? statsRegenPerSec : boostRegenPerSec;

        blackboard.BoostEnergy = Mathf.Clamp(
            blackboard.BoostEnergy + regen * Time.deltaTime,
            0f,
            blackboard.MaxBoostEnergy);
        // blackboard.BoostEnergy = Mathf.Clamp(
        //     blackboard.BoostEnergy + boostRegenPerSec * Time.deltaTime,
        //     0f,
        //     blackboard.MaxBoostEnergy);

        blackboard.Pressure = Mathf.Max(0f, blackboard.Pressure - pressureDecayPerSec * Time.deltaTime);

        blackboard.Aggression = Mathf.Max(0f, blackboard.Aggression - aggressionDecayPerSec * Time.deltaTime);
    }

    // プレイヤーに捕捉されている時間を測る
    void TrackCapture()
    {
        if (blackboard.IsCapturedByPlayer)
            capturedTimer += Time.deltaTime;
        else
            capturedTimer = 0f;
    }

    // 主導権: プレイヤーに捕捉されていない(向きか射程から外れている)
    bool HasInitiative()
    {
        return !blackboard.IsCapturedByPlayer;
    }

    // 隙があれば仕掛ける度合い
    float AssaultWillingness()
    {
        // Profile で指定されていれば、それを優先
        if (assaultWillingness >= 0f)
            return assaultWillingness;

        // 指定がなければ性格の既定値
        switch (personality)
        {
            case AIPersonality.Aggressive: return 1.0f;
            case AIPersonality.Duelist:    return 0.8f;
            case AIPersonality.Balanced:   return 0.6f;
            case AIPersonality.Defensive:  return 0.3f;
            case AIPersonality.Sniper:     return 0.0f;
        }

        return 0.5f;
    }

    bool CanStartAssault()
    {
        if (!allowAssault)
            return false;

        // 突撃の目的は近接攻撃。近接武器がなければ仕掛けない
        if (!hasMelee)
            return false;

        if (Time.time < nextAssaultTime)
            return false;

        if (!HasInitiative())
            return false;

        // 被弾した直後は回避を優先
        if (Time.time - blackboard.LastDamageTime < 0.6f)
            return false;

        if (blackboard.BoostEnergy < assaultBoostCost)
            return false;

        if (blackboard.TargetDistance > combatDistance * assaultStartRate)
            return false;

        float hpRate = blackboard.CurrentHP / Mathf.Max(1f, unitControl.maxHP);

        if (hpRate < assaultMinHpRate / courage)
            return false;

        return true;
    }

    void CheckAssaultOpportunity()
    {
        if (Time.time < nextOpportunityCheck)
            return;

        nextOpportunityCheck = Time.time + 0.25f;

        if (currentAction == CombatAction.Assault || currentAction == CombatAction.MeleeAttack)
            return;

        if (!CanStartAssault())
            return;

        // 性格によっては、隙があっても今回は見送る
        if (Random.value > AssaultWillingness())
        {
            nextOpportunityCheck = Time.time + 1.5f;
            return;
        }

        StartAssault();
    }

    void StartAssault()
    {
        currentAction = CombatAction.Assault;

        assaultEndTime = Time.time + assaultMaxDuration;
        assaultSwung = false;

        Debug.Log("Assault Start");
    }

    // 突撃中に毎フレーム行う「続けるか/やめるか」の判断
    void UpdateAssault()
    {
        if (!hasMelee)
        {
            EndAssault(CombatAction.Orbit);
            return;
        }

        // 近接攻撃を振り終えたら離脱(ヒットアンドアウェイ)
        if (assaultSwung && !weapon.IsMeleeAttackInProgress)
        {
            EndAssault(CombatAction.SideBoost);
            return;
        }

        // 時間切れ
        if (Time.time >= assaultEndTime)
        {
            EndAssault(CombatAction.Orbit);
            return;
        }

        // プレイヤーに捕捉され続けていて、まだ距離があるなら中止
        // (近づけているなら、そのまま振り切る)
        if (capturedTimer >= abortCaptureTime && blackboard.TargetDistance > meleeRange * 1.5f)
        {
            EndAssault(CombatAction.SideBoost);
            return;
        }
    }

    void EndAssault(CombatAction next)
    {
        currentAction = next;

        nextAssaultTime = Time.time + assaultCooldown;
        assaultSwung = false;

        // 離脱動作をしてから、次の判断をする
        actionLockTimer = 0.5f;
        thinkTimer = 1.0f;

        Debug.Log("Assault End -> " + next);
    }

    void ExecuteAssault()
    {
        movement.LookAtTarget(blackboard.Target);

        float distance = blackboard.TargetDistance;

        // 近接射程に入ったら攻撃(ブーストの勢いのまま振る)
        if (distance <= meleeRange)
        {
            movement.Stop();

            if (weapon.IsMeleeReady)
            {
                weapon.StartMeleeAttack();

                if (weapon.IsMeleeAttackInProgress)
                    assaultSwung = true;
            }

            return;
        }

        // 接近: 通常移動 + ブースト(間隔とエネルギーの制限つき)
        movement.MoveTowards(blackboard.Target);

        if (blackboard.BoostEnergy >= assaultBoostCost && movement.BoostTowards(blackboard.Target))
        {
            blackboard.BoostEnergy -= assaultBoostCost;
        }
    }

    // 自分がターゲットに向かって進んでいる速さ
    float ClosingSpeed()
    {
        if (movement.rb == null || blackboard.Target == null)
            return 0f;

        Vector3 toTarget = (blackboard.Target.position - transform.position).normalized;

        return Vector3.Dot(movement.rb.velocity, toTarget);
    }

    // ------------------------------------------------------------
    // 行動の選択
    // ------------------------------------------------------------

    void DecideAction()
    {
        retreatScore = 0;
        orbitScore = 0;
        approachScore = 0;
        altitudeScore = 0;
        behindScore = 0;
        meleeScore = 0;
        sideBoostScore = 0;

        // 「攻めるために詰める」スコア。
        // 主導権があるときだけ approachScore に反映する(なければごくわずか)。
        float attackApproach = 0f;

        bool hasInitiative = HasInitiative();

        switch (personality)
        {
            case AIPersonality.Aggressive:
                attackApproach += 80;
                behindScore += 80;
                break;

            case AIPersonality.Defensive:
                retreatScore += 80;
                orbitScore += 80;
                break;

            case AIPersonality.Sniper:
                retreatScore += 120;
                altitudeScore += 80;
                break;

            case AIPersonality.Duelist:
                behindScore += 120;
                orbitScore += 100;
                break;
        }

        float distance = blackboard.TargetDistance;

        float heightDifference = blackboard.HeightDifference;

        Vector3 playerForward = blackboard.Target.forward;

        Vector3 toEnemy = transform.position - blackboard.Target.position;

        float dot = Vector3.Dot(playerForward.normalized, toEnemy.normalized);

        // 近すぎる
        if (distance < combatDistance * 0.5f)
        {
            retreatScore += 100f;
        }

        // 遠すぎる(距離の調整は、主導権に関係なく行う)
        if (distance > combatDistance * 1.5f)
        {
            approachScore += 100f;
        }

        // 適正距離
        if (distance >= combatDistance * 0.7f && distance <= combatDistance * 1.2f)
        {
            orbitScore += 120f;
        }

        float hpRate = blackboard.CurrentHP / unitControl.maxHP;

        if (hpRate < (0.3f / courage))
        {
            retreatScore += 150f;
        }

        // 上を取られている
        if (heightDifference > 15f)
        {
            altitudeScore += 150f;
        }

        if (distance > 20f && distance < 60f)
        {
            behindScore += 70;
        }

        if (distance < 15f)
        {
            retreatScore += 300;
            altitudeScore += 150;
        }

        if (hasMelee && distance <= meleeRange)
        {
            meleeScore += 300f;
        }
        else if (hasMelee)
        {
            if (hasInitiative)
            {
                // 主導権があるときだけ、近接武器で詰める
                attackApproach += 400f;
            }
            else
            {
                // 主導権がないときは、回り込んで隙(主導権)を作りにいく
                behindScore += 150f;
                orbitScore += 80f;
            }
        }

        if (dot > 0.9f)
        {
            sideBoostScore += 200;
        }
        else if (dot < -0.8f)
        {
            behindScore += 150;
        }

        if (blackboard.TargetVelocity.magnitude > 25f)
        {
            behindScore += 100;
        }

        if (blackboard.IsPlayerBoosting)
        {
            if (distance < 40f)
            {
                sideBoostScore += 250f;
                retreatScore += 200f;
            }
            else
            {
                attackApproach += 200f;
            }
        }

        if (blackboard.IsPlayerBoosting && distance < 30f)
        {
            blackboard.Pressure += 20f * Time.deltaTime;
        }

        if (blackboard.IsPlayerMeleeAttacking)
        {
            currentAction = CombatAction.SideBoost;

            return;
        }

        if (blackboard.Pressure > 180f)
        {
            retreatScore += 300f;
            sideBoostScore += 200f;
        }

        if (distance < 30f)
        {
            behindScore += 150f;
            attackApproach += 100f;
        }

        if (distance < 50f)
        {
            blackboard.Aggression += Time.deltaTime * 10f;
        }

        if (Random.value < 0.15f)
        {
            altitudeScore += 80;
        }

        attackApproach += blackboard.Aggression;

        // 主導権がなければ、攻めの接近はごくわずかしか反映しない
        approachScore += hasInitiative
            ? attackApproach
            : attackApproach * disadvantageApproachRate;

        // 被弾した直後
        if (Time.time - blackboard.LastDamageTime < 0.6f)
        {
            int r = Random.Range(0, 100);

            if (r < 40)
                currentAction = CombatAction.SideBoost;

            else if (r < 70)
                currentAction = CombatAction.BehindMove;

            else
                currentAction = CombatAction.GainAltitude;

            return;
        }

        if (hasMelee && distance <= meleeRange)
        {
            currentAction = CombatAction.MeleeAttack;
            return;
        }

        // Profile の重みで、行動の好みを調整する(0なら、その行動は選ばれない)
        retreatScore *= retreatWeight;
        orbitScore *= orbitWeight;
        approachScore *= approachWeight;
        altitudeScore *= altitudeWeight;
        behindScore *= behindWeight;
        sideBoostScore *= sideBoostWeight;

        float bestScore = Mathf.Max(retreatScore, orbitScore, approachScore, altitudeScore, behindScore, meleeScore, sideBoostScore);

        if (bestScore == retreatScore)
        {
            currentAction = CombatAction.Retreat;
        }
        else if (bestScore == sideBoostScore)
        {
            currentAction = CombatAction.SideBoost;
        }
        else if (bestScore == orbitScore)
        {
            int r = Random.Range(0, 100);

            if (r < 50)
            {
                currentAction = CombatAction.Orbit;
            }
            else if (r < 70)
            {
                currentAction = CombatAction.SideBoost;
            }
            else if (r < 85)
            {
                currentAction = CombatAction.BehindMove;
            }
            else
            {
                currentAction = CombatAction.GainAltitude;
            }
        }
        else if (bestScore == approachScore)
        {
            currentAction = CombatAction.Approach;
        }
        else if (bestScore == altitudeScore)
        {
            currentAction = CombatAction.GainAltitude;
        }
        else if (bestScore == behindScore)
        {
            currentAction = CombatAction.BehindMove;
        }
        else if (bestScore == meleeScore)
        {
            currentAction = CombatAction.MeleeAttack;
        }
    }

    void ExecuteAction()
    {
        switch (currentAction)
        {
            case CombatAction.Approach:
                movement.LookAtTarget(blackboard.Target);
                movement.MoveTowards(blackboard.Target);

                // 遠いときはブーストで詰める(間隔・エネルギーの制限つき)
                if (blackboard.TargetDistance > 80f &&
                    blackboard.BoostEnergy >= approachBoostCost &&
                    movement.BoostTowards(blackboard.Target))
                {
                    blackboard.BoostEnergy -= approachBoostCost;
                }

                // 高速で接近中に近接武器の射程へ入ったら、その勢いのまま斬る
                if (hasMelee &&
                    blackboard.TargetDistance <= meleeRange &&
                    ClosingSpeed() >= dashAttackSpeed)
                {
                    weapon.StartMeleeAttack();
                }
                break;

            case CombatAction.Orbit:
                movement.OrbitTarget(blackboard.Target);
                break;

            case CombatAction.Retreat:
                movement.LookAtTarget(blackboard.Target);
                movement.MoveAway(blackboard.Target);
                break;

            case CombatAction.SideBoost:
                movement.LookAtTarget(blackboard.Target);
                if (blackboard.BoostEnergy >= 20f)
                {
                    movement.SideBoost(blackboard.Target);
                    blackboard.BoostEnergy -= 20f;
                    currentAction = CombatAction.Orbit;
                }
                break;

            case CombatAction.GainAltitude:
                movement.LookAtTarget(blackboard.Target);
                movement.GainAltitude(blackboard.Target);
                break;

            case CombatAction.BehindMove:
                movement.LookAtTarget(blackboard.Target);
                movement.MoveBehindTarget(blackboard.Target);
                break;

            case CombatAction.MeleeAttack:
                movement.LookAtTarget(blackboard.Target);
                if (blackboard.TargetDistance > meleeRange)
                {
                    movement.MoveTowards(blackboard.Target);   // 範囲外なら詰める
                }
                else
                {
                    movement.Stop();                           // 攻撃中は止まる
                    weapon.StartMeleeAttack();
                }
                break;

            case CombatAction.Assault:
                ExecuteAssault();
                break;
        }
    }

    void UpdateSearch()
    {
        movement.MovePosition(blackboard.LastKnownPosition);

        float dist = Vector3.Distance(transform.position, blackboard.LastKnownPosition);

        if (dist < 5f)
        {
            state = AIState.Patrol;
        }

        if (blackboard.CanSeeTarget)
        {
            state = AIState.Chase;
        }
    }
}
