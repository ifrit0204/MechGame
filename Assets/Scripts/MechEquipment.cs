using UnityEngine;

public class MechEquipment : MonoBehaviour
{
    [Header("Database")]
    [SerializeField]
    private MechPartDatabase database;

    [Header("Mech")]
    [SerializeField]
    private MechControl mech;


    //==================================================
    // 現在装備しているパーツ
    //==================================================

    [Header("Current Parts")]

    public MechHeadPartData head;
    public MechBodyPartData body;
    public MechArmsPartData arms;
    public MechLegsPartData legs;

    public MechWeaponPartData rightWeapon;
    public MechWeaponPartData leftWeapon;

    public MechWeaponPartData rightShoulder;
    public MechWeaponPartData leftShoulder;

    public MechFCSPartData FCS;
    public MechBoosterPartData booster;
    public MechGeneratorPartData generator;


    //==================================================
    // 現在の3Dモデル
    //==================================================

    [Header("Current Models")]

    public GameObject currentHead;
    public GameObject currentBody;
    public GameObject currentArms;
    public GameObject currentLegs;

    public GameObject currentRightWeapon;
    public GameObject currentLeftWeapon;

    public GameObject currentRightShoulder;
    public GameObject currentLeftShoulder;

    public GameObject currentFCS;
    public GameObject currentBooster;
    public GameObject currentGenerator;


    //==================================================
    // Socket
    //==================================================

    [Header("Sockets")]

    public Transform headSocket;
    public Transform bodySocket;
    public Transform armsSocket;
    public Transform legsSocket;

    public Transform rightWeaponSocket;
    public Transform leftWeaponSocket;

    public Transform rightShoulderSocket;
    public Transform leftShoulderSocket;

    public Transform FCSSocket;
    public Transform boosterSocket;
    public Transform generatorSocket;


    //==================================================
    // Current Index
    //==================================================

    private int currentHeadIndex = -1;
    private int currentBodyIndex = -1;
    private int currentArmsIndex = -1;
    private int currentLegsIndex = -1;

    private int currentRightWeaponIndex = -1;
    private int currentLeftWeaponIndex = -1;

    private int currentRightShoulderIndex = -1;
    private int currentLeftShoulderIndex = -1;

    private int currentFCSIndex = -1;
    private int currentBoosterIndex = -1;
    private int currentGeneratorIndex = -1;


    //==================================================
    // Weapon
    //==================================================

    public MechMeleeWeapon CurrentMeleeWeapon
    {
        get;
        private set;
    }


    //==================================================
    // Database
    //==================================================

    public MechPartDatabase Database
    {
        get
        {
            return database;
        }
    }


    //==================================================
    // 共通のパーツ変更処理
    //==================================================

    public void ChangePart(PartType partType, int index)
    {
        if (database == null)
        {
            Debug.LogError("MechPartDatabaseが設定されていません。");
            return;
        }

        MechPartData[] parts =
            database.GetParts(partType);

        if (parts == null)
        {
            Debug.LogError("パーツ一覧が存在しません : " + partType);

            return;
        }

        if (index < 0 || index >= parts.Length)
        {
            Debug.LogError("パーツIndexが範囲外です : " + partType + " / " + index);

            return;
        }

        MechPartData part = parts[index];

        if (part == null)
        {
            Debug.LogError("パーツデータがnullです : " + partType + " / " + index);

            return;
        }


        // 実際に装備
        EquipPart(partType, part);


        // 現在Indexを更新
        SetCurrentIndex(partType, index);


        // Loadoutを更新
        UpdateLoadout(partType, part);


        // 機体性能を更新
        if (mech != null)
        {
            mech.ApplyParts();
        }
    }
    public void ChangePart(PartType partType, MechPartData part)
    {
        if(part == null)
            return;

        EquipPart(partType, part);

        UpdateLoadout(partType, part);

        if(mech != null)
        {
            mech.ApplyParts();
        }
    }


    //==================================================
    // 実際の装備処理
    //==================================================

    public void EquipPart(PartType partType, MechPartData part)
    {
        switch (partType)
        {
            case PartType.Head:
                EquipHead((MechHeadPartData)part);
                break;

            case PartType.Body:
                EquipBody((MechBodyPartData)part);
                break;

            case PartType.Arms:
                EquipArms((MechArmsPartData)part);
                break;

            case PartType.Legs:
                EquipLegs((MechLegsPartData)part);
                break;

            case PartType.RightWeapon:
                EquipRightWeapon((MechWeaponPartData)part);
                break;

            case PartType.LeftWeapon:
                EquipLeftWeapon((MechWeaponPartData)part);
                break;

            case PartType.RightShoulder:
                EquipRightShoulderWeapon((MechWeaponPartData)part);
                break;

            case PartType.LeftShoulder:
                EquipLeftShoulderWeapon((MechWeaponPartData)part);
                break;

            case PartType.Booster:
                EquipBooster((MechBoosterPartData)part);
                break;

            case PartType.FCS:
                EquipFCS((MechFCSPartData)part);
                break;

            case PartType.Generator:
                EquipGenerator((MechGeneratorPartData)part);
                break;
        }
    }


    //==================================================
    // Loadout更新
    //==================================================

    private void UpdateLoadout(PartType partType, MechPartData part)
    {
        if (MechGameManager.Instance == null)
        {
            return;
        }

        MechLoadout loadout = MechGameManager.Instance.CurrentLoadout;

        if (loadout == null)
        {
            loadout = new MechLoadout();

            MechGameManager.Instance.CurrentLoadout = loadout;
        }


        switch (partType)
        {
            case PartType.Head:
                loadout.Head = (MechHeadPartData)part;
                break;

            case PartType.Body:
                loadout.Body = (MechBodyPartData)part;
                break;

            case PartType.Arms:
                loadout.Arms = (MechArmsPartData)part;
                break;

            case PartType.Legs:
                loadout.Legs = (MechLegsPartData)part;
                break;

            case PartType.RightWeapon:
                loadout.RightWeapon = (MechWeaponPartData)part;
                break;

            case PartType.LeftWeapon:
                loadout.LeftWeapon = (MechWeaponPartData)part;
                break;

            case PartType.RightShoulder:
                loadout.RightShoulder = (MechWeaponPartData)part;
                break;

            case PartType.LeftShoulder:
                loadout.LeftShoulder = (MechWeaponPartData)part;
                break;

            case PartType.Booster:
                loadout.Booster = (MechBoosterPartData)part;
                break;

            case PartType.FCS:
                loadout.FCS = (MechFCSPartData)part;
                break;

            case PartType.Generator:
                loadout.Generator = (MechGeneratorPartData)part;
                break;
        }
    }


    //==================================================
    // Index更新
    //==================================================

    private void SetCurrentIndex(PartType partType, int index)
    {
        switch (partType)
        {
            case PartType.Head:
                currentHeadIndex = index;
                break;

            case PartType.Body:
                currentBodyIndex = index;
                break;

            case PartType.Arms:
                currentArmsIndex = index;
                break;

            case PartType.Legs:
                currentLegsIndex = index;
                break;

            case PartType.RightWeapon:
                currentRightWeaponIndex = index;
                break;

            case PartType.LeftWeapon:
                currentLeftWeaponIndex = index;
                break;

            case PartType.RightShoulder:
                currentRightShoulderIndex = index;
                break;

            case PartType.LeftShoulder:
                currentLeftShoulderIndex = index;
                break;

            case PartType.Booster:
                currentBoosterIndex = index;
                break;

            case PartType.FCS:
                currentFCSIndex = index;
                break;

            case PartType.Generator:
                currentGeneratorIndex = index;
                break;
        }
    }


    //==================================================
    // Head
    //==================================================

    public void EquipHead(MechHeadPartData part)
    {
        DestroyCurrent(ref currentHead);

        if (part == null || part.modelPrefab == null)
        {
            head = part;
            return;
        }

        currentHead = Instantiate(part.modelPrefab, headSocket);

        head = part;
    }


    //==================================================
    // Body
    //==================================================

    public void EquipBody(MechBodyPartData part)
    {
        DestroyCurrent(ref currentBody);

        if (part == null || part.modelPrefab == null)
        {
            body = part;
            return;
        }

        currentBody = Instantiate(part.modelPrefab, bodySocket);

        body = part;
    }


    //==================================================
    // Arms
    //==================================================

    public void EquipArms(MechArmsPartData part)
    {
        DestroyCurrent(ref currentArms);

        if (part == null || part.modelPrefab == null)
        {
            arms = part;
            return;
        }

        currentArms = Instantiate(part.modelPrefab, armsSocket);

        arms = part;
    }


    //==================================================
    // Legs
    //==================================================

    public void EquipLegs(MechLegsPartData part)
    {
        DestroyCurrent( ref currentLegs);

        if (part == null || part.modelPrefab == null)
        {
            legs = part;
            return;
        }

        currentLegs = Instantiate(part.modelPrefab, legsSocket);

        legs = part;
    }


    //==================================================
    // Right Weapon
    //==================================================

    public void EquipRightWeapon(MechWeaponPartData part)
    {
        DestroyCurrent(ref currentRightWeapon);

        if (part == null || part.modelPrefab == null)
        {
            rightWeapon = part;
            return;
        }

        currentRightWeapon = Instantiate(part.modelPrefab, rightWeaponSocket);

        rightWeapon = part;
    }


    //==================================================
    // Left Weapon
    //==================================================

    public void EquipLeftWeapon(MechWeaponPartData part)
    {
        DestroyCurrent(ref currentLeftWeapon);

        CurrentMeleeWeapon = null;

        if (part == null || part.modelPrefab == null)
        {
            leftWeapon = part;
            return;
        }

        currentLeftWeapon = Instantiate(part.modelPrefab, leftWeaponSocket);

        leftWeapon = part;


        CurrentMeleeWeapon = currentLeftWeapon.GetComponent<MechMeleeWeapon>();

        if (CurrentMeleeWeapon != null)
        {
            CurrentMeleeWeapon.SetWeapon(part);
        }
    }


    //==================================================
    // Right Shoulder
    //==================================================

    public void EquipRightShoulderWeapon(MechWeaponPartData part)
    {
        DestroyCurrent(ref currentRightShoulder);

        if (part == null || part.modelPrefab == null)
        {
            rightShoulder = part;
            return;
        }

        currentRightShoulder = Instantiate(part.modelPrefab, rightShoulderSocket);

        rightShoulder = part;
    }


    //==================================================
    // Left Shoulder
    //==================================================

    public void EquipLeftShoulderWeapon(MechWeaponPartData part)
    {
        DestroyCurrent(ref currentLeftShoulder);

        if (part == null || part.modelPrefab == null)
        {
            leftShoulder = part;
            return;
        }

        currentLeftShoulder = Instantiate(part.modelPrefab, leftShoulderSocket);

        leftShoulder = part;
    }


    //==================================================
    // Booster
    //==================================================

    public void EquipBooster(MechBoosterPartData part)
    {
        DestroyCurrent(ref currentBooster);

        if (part == null || part.modelPrefab == null)
        {
            booster = part;
            return;
        }

        currentBooster = Instantiate(part.modelPrefab, boosterSocket);

        booster = part;
    }


    //==================================================
    // FCS
    //==================================================

    public void EquipFCS(MechFCSPartData part)
    {
        DestroyCurrent(ref currentFCS);

        if (part == null || part.modelPrefab == null)
        {
            FCS = part;
            return;
        }

        currentFCS = Instantiate(part.modelPrefab, FCSSocket);

        FCS = part;
    }


    //==================================================
    // Generator
    //==================================================

    public void EquipGenerator(MechGeneratorPartData part)
    {
        DestroyCurrent(ref currentGenerator);

        if (part == null || part.modelPrefab == null)
        {
            generator = part;
            return;
        }

        currentGenerator = Instantiate(part.modelPrefab, generatorSocket);

        generator = part;
    }


    //==================================================
    // GameObject削除
    //==================================================

    private void DestroyCurrent(ref GameObject current)
    {
        if (current != null)
        {
            Destroy(current);
            current = null;
        }
    }


    //==================================================
    // 現在装備パーツ取得
    //==================================================

    public MechPartData GetCurrentPart(PartType partType)
    {
        switch (partType)
        {
            case PartType.Head:
                return head;

            case PartType.Body:
                return body;

            case PartType.Arms:
                return arms;

            case PartType.Legs:
                return legs;

            case PartType.RightWeapon:
                return rightWeapon;

            case PartType.LeftWeapon:
                return leftWeapon;

            case PartType.RightShoulder:
                return rightShoulder;

            case PartType.LeftShoulder:
                return leftShoulder;

            case PartType.Booster:
                return booster;

            case PartType.FCS:
                return FCS;

            case PartType.Generator:
                return generator;
        }

        return null;
    }


    //==================================================
    // 現在Index取得
    //==================================================

    public int GetCurrentIndex(PartType partType)
    {
        switch (partType)
        {
            case PartType.Head:
                return currentHeadIndex;

            case PartType.Body:
                return currentBodyIndex;

            case PartType.Arms:
                return currentArmsIndex;

            case PartType.Legs:
                return currentLegsIndex;

            case PartType.RightWeapon:
                return currentRightWeaponIndex;

            case PartType.LeftWeapon:
                return currentLeftWeaponIndex;

            case PartType.RightShoulder:
                return currentRightShoulderIndex;

            case PartType.LeftShoulder:
                return currentLeftShoulderIndex;

            case PartType.Booster:
                return currentBoosterIndex;

            case PartType.FCS:
                return currentFCSIndex;

            case PartType.Generator:
                return currentGeneratorIndex;
        }

        return -1;
    }


    //==================================================
    // Loadoutから装備を反映
    //==================================================

    public void ApplyLoadout(MechLoadout loadout)
    {
        if (loadout == null)
        {
            Debug.LogWarning("MechLoadoutがnullです。");

            return;
        }
        if (database == null)
        {
            Debug.LogError("MechEquipment : MechPartDatabaseが設定されていません。");

            return;
        }


        // Loadoutを現在のものとして保持
        if (MechGameManager.Instance != null)
        {
            MechGameManager.Instance.CurrentLoadout = loadout;
        }


        ApplyLoadoutPart(PartType.Head, loadout.Head);

        ApplyLoadoutPart(PartType.Body, loadout.Body);

        ApplyLoadoutPart(PartType.Arms, loadout.Arms);

        ApplyLoadoutPart(PartType.Legs, loadout.Legs);

        ApplyLoadoutPart(PartType.RightWeapon, loadout.RightWeapon);

        ApplyLoadoutPart(PartType.LeftWeapon, loadout.LeftWeapon);

        ApplyLoadoutPart(PartType.RightShoulder, loadout.RightShoulder);

        ApplyLoadoutPart(PartType.LeftShoulder, loadout.LeftShoulder);

        ApplyLoadoutPart(PartType.Booster, loadout.Booster);

        ApplyLoadoutPart(PartType.FCS, loadout.FCS);

        ApplyLoadoutPart(PartType.Generator, loadout.Generator);


        // 全パーツを読み込んだ後に一度だけ更新
        if (mech != null)
        {
            mech.ApplyParts();
        }
    }


    private void ApplyLoadoutPart(PartType partType, MechPartData part)
    {
        if (part == null)
        {
            return;
        }

        EquipPart(partType, part);


        int index = FindPartIndex(partType, part);

        SetCurrentIndex(partType, index);
    }


    //==================================================
    // Database内からIndexを検索
    //==================================================

    private int FindPartIndex(PartType partType, MechPartData part)
    {
        MechPartData[] parts = database.GetParts(partType);

        if (parts == null)
        {
            return -1;
        }

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == part)
            {
                return i;
            }
        }

        return -1;
    }


    //==================================================
    // PlayerPrefs保存
    //==================================================

    public void SaveParts()
    {
        PlayerPrefs.SetInt("Head", currentHeadIndex);

        PlayerPrefs.SetInt("Body", currentBodyIndex);

        PlayerPrefs.SetInt("Arms", currentArmsIndex);

        PlayerPrefs.SetInt("Legs", currentLegsIndex);

        PlayerPrefs.SetInt("RightWeapon", currentRightWeaponIndex);

        PlayerPrefs.SetInt("LeftWeapon", currentLeftWeaponIndex);

        PlayerPrefs.SetInt("RightShoulder", currentRightShoulderIndex);

        PlayerPrefs.SetInt("LeftShoulder", currentLeftShoulderIndex);

        PlayerPrefs.SetInt("Booster", currentBoosterIndex);

        PlayerPrefs.SetInt("FCS", currentFCSIndex);

        PlayerPrefs.SetInt("Generator", currentGeneratorIndex);

        PlayerPrefs.Save();
    }


    //==================================================
    // PlayerPrefsロード
    //==================================================

    public void LoadParts()
    {
        ChangePart(PartType.Head, PlayerPrefs.GetInt("Head", 0));

        ChangePart(PartType.Body, PlayerPrefs.GetInt("Body", 0));

        ChangePart(PartType.Arms, PlayerPrefs.GetInt("Arms", 0));

        ChangePart(PartType.Legs, PlayerPrefs.GetInt("Legs", 0));

        ChangePart(PartType.RightWeapon, PlayerPrefs.GetInt("RightWeapon", 0));

        ChangePart(PartType.LeftWeapon, PlayerPrefs.GetInt("LeftWeapon", 0));

        ChangePart(PartType.RightShoulder, PlayerPrefs.GetInt("RightShoulder", 0));

        ChangePart(PartType.LeftShoulder, PlayerPrefs.GetInt("LeftShoulder", 0));

        ChangePart(PartType.Booster, PlayerPrefs.GetInt("Booster", 0));

        ChangePart(PartType.FCS, PlayerPrefs.GetInt("FCS", 0));

        ChangePart(PartType.Generator, PlayerPrefs.GetInt("Generator", 0));
    }


    //==================================================
    // Melee Weapon
    //==================================================

    public MechMeleeWeapon GetCurrentMeleeWeapon()
    {
        if (currentLeftWeapon == null)
        {
            return null;
        }

        return currentLeftWeapon
            .GetComponent<MechMeleeWeapon>();
    }
    public MechTotalStats CalculateTotalStats()
    {
        MechTotalStats stats = new MechTotalStats();


        //==============================
        // Head
        //==============================

        if (head != null)
        {
            stats.weight += head.weight;
            stats.energy += head.energyConsumption;
            stats.armor += head.armorPoint;
            stats.stability += head.headStabilityPerformance;
            stats.meleeAptitude += head.headMeleeWeaponAptitude;

            // stats.armorPoint = Mathf.Max(stats.armorPoint, head.armorPoint);
            // stats.headStabilityPerformance = Mathf.Max(stats.headStabilityPerformance, head.headStabilityPerformance);
            // stats.headMeleeWeaponAptitude = Mathf.Max(stats.headMeleeWeaponAptitude, head.headMeleeWeaponAptitude);
            stats.scanRange = Mathf.Max(stats.scanRange, head.scanRange);
            stats.lockDistance = Mathf.Max(stats.lockDistance, head.lockDistance);

        }


        //==============================
        // Body
        //==============================

        if (body != null)
        {
            stats.weight += body.weight;
            stats.energy += body.energyConsumption;
            stats.armor += body.armorPoint;
            stats.stability += body.bodyStabilityPerformance;
            stats.meleeAptitude += body.bodyMeleeWeaponAptitude;

            // stats.armorPoint = Mathf.Max(stats.armorPoint, body.armorPoint);
            // stats.bodyStabilityPerformance = Mathf.Max(stats.bodyStabilityPerformance, body.bodyStabilityPerformance);
            // stats.bodyMeleeWeaponAptitude = Mathf.Max(stats.bodyMeleeWeaponAptitude, body.bodyMeleeWeaponAptitude);

        }


        //==============================
        // Arms
        //==============================

        if (arms != null)
        {
            stats.weight += arms.weight;
            stats.energy += arms.energyConsumption;
            stats.armor += arms.armorPoint;
            stats.stability += arms.armStabilityPerformance;
            stats.meleeAptitude += arms.armMeleeWeaponAptitude;

            stats.recoilReduction = arms.recoilReduction;
            stats.armMobility = arms.armMobility;
            stats.aimingPerformance = arms.aimingPerformance;
            // stats.armorPoint = Mathf.Max(stats.armorPoint, arms.armorPoint);
            // stats.recoilReduction = Mathf.Max(stats.recoilReduction, arms.recoilReduction);
            // stats.armMobility = Mathf.Max(stats.armMobility, arms.armMobility);
            // stats.armStabilityPerformance = Mathf.Max(stats.armStabilityPerformance, arms.armStabilityPerformance);
            // stats.aimingPerformance = Mathf.Max(stats.aimingPerformance, arms.aimingPerformance);
            // stats.armMeleeWeaponAptitude = Mathf.Max(stats.armMeleeWeaponAptitude, arms.armMeleeWeaponAptitude);


        }


        //==============================
        // Legs
        //==============================

        if (legs != null)
        {
            stats.weight += legs.weight;
            stats.energy += legs.energyConsumption;
            stats.armor += legs.armorPoint;
            stats.stability += legs.legStabilityPerformance;
            stats.meleeAptitude += legs.legMeleeWeaponAptitude;


            stats.moveSpeed = legs.moveSpeed;
            stats.jumpForce = legs.jumpForce;
            stats.jumpPerformance = legs.jumpPerformance;
            stats.groundMultiplier = legs.groundMultiplier;

        }


        //==============================
        // Booster
        //==============================

        if (booster != null)
        {
            stats.weight += booster.weight;
            stats.energy += booster.energyConsumption;

            stats.boostForce = booster.boostForce;
            stats.boostEfficiency = booster.boostEfficiency;
            stats.boostEnergyCost = booster.boostCost;
        }


        //==============================
        // Weapons
        //==============================

        AddWeaponStats(rightWeapon, stats);

        AddWeaponStats(leftWeapon, stats);

        AddWeaponStats(rightShoulder, stats);

        AddWeaponStats(leftShoulder, stats);


        //==============================
        // FCS
        //==============================

        if (FCS != null)
        {
            stats.weight += FCS.weight;
            stats.energy += FCS.energyConsumption;
            stats.lockRange = FCS.lockRange;
            stats.lockSpeed = FCS.lockSpeed;
            stats.lockRadius = FCS.lockRadius;
        }


        //==============================
        // Generator
        //==============================

        if (generator != null)
        {
            stats.weight += generator.weight;
            stats.energy += generator.energyConsumption;
        }


        return stats;
    }
    public MechTotalStats CalculateStatsFromLoadout(MechLoadout loadout)
    {
        MechTotalStats stats = new MechTotalStats();

        if(loadout.Head != null)
        {
            stats.weight += loadout.Head.weight;
            stats.energy += loadout.Head.energyConsumption;
            stats.armor += loadout.Head.armorPoint;
            stats.stability += loadout.Head.headStabilityPerformance;
            stats.meleeAptitude += loadout.Head.headMeleeWeaponAptitude;

            stats.scanRange = Mathf.Max(stats.scanRange, loadout.Head.scanRange);
            stats.lockDistance = Mathf.Max(stats.lockDistance, loadout.Head.lockDistance);
        }

        if (loadout.Body != null)
        {
            stats.weight += loadout.Body.weight;
            stats.energy += loadout.Body.energyConsumption;
            stats.armor += loadout.Body.armorPoint;
            stats.stability += loadout.Body.bodyStabilityPerformance;
            stats.meleeAptitude += loadout.Body.bodyMeleeWeaponAptitude;
        }

        if (loadout.Arms != null)
        {
            stats.weight += loadout.Arms.weight;
            stats.energy += loadout.Arms.energyConsumption;
            stats.armor += loadout.Arms.armorPoint;
            stats.stability += loadout.Arms.armStabilityPerformance;
            stats.meleeAptitude += loadout.Arms.armMeleeWeaponAptitude;

            stats.armorPoint = Mathf.Max(stats.armorPoint, arms.armorPoint);
            stats.recoilReduction = Mathf.Max(stats.recoilReduction, arms.recoilReduction);
            stats.armMobility = Mathf.Max(stats.armMobility, arms.armMobility);
            stats.armStabilityPerformance = Mathf.Max(stats.armStabilityPerformance, arms.armStabilityPerformance);
            stats.aimingPerformance = Mathf.Max(stats.aimingPerformance, arms.aimingPerformance);
            stats.armMeleeWeaponAptitude = Mathf.Max(stats.armMeleeWeaponAptitude, arms.armMeleeWeaponAptitude);
        }

        if (loadout.Legs != null)
        {
            stats.weight += loadout.Legs.weight;
            stats.energy += loadout.Legs.energyConsumption;
            stats.armor += loadout.Legs.armorPoint;
            stats.stability += loadout.Legs.legStabilityPerformance;
            stats.meleeAptitude += loadout.Legs.legMeleeWeaponAptitude;


            stats.moveSpeed = loadout.Legs.moveSpeed;
            stats.jumpForce = loadout.Legs.jumpForce;
            stats.jumpPerformance = loadout.Legs.jumpPerformance;
            stats.groundMultiplier = loadout.Legs.groundMultiplier;

        }

        return stats;
    }
    //==================================================
    // Weapon Stats
    //==================================================

    private void AddWeaponStats(MechWeaponPartData weapon, MechTotalStats stats)
    {
        if (weapon == null)
        {
            return;
        }

        stats.weight += weapon.weight;
        stats.energy += weapon.energyConsumption;

        stats.totalWeaponDamage += weapon.damageData.Damage;
        stats.totalAmmo += weapon.ammo;
    }

}
