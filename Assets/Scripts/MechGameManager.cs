using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MechGameManager : MonoBehaviour
{
    public static MechGameManager Instance;
    public int enemyCount;
    public MechLoadout CurrentLoadout = new MechLoadout();
    public MechMissionData CurrentMission;
    [SerializeField] private MechMissionData firstMission;
    public MechArenaEnemyData CurrentArenaEnemy;
    private HashSet<string> clearedArenaEnemies = new HashSet<string>();


    public int Credits = 50000;
    private int bonusCredits;

    public int LastReward;
    public string LastResultTitle;
    public bool HasResult;
    private float missionStartTime;

    public MechResultData CurrentResult = new MechResultData();
    public void StartMissionResult()
    {
        CurrentResult = new MechResultData();

        CurrentResult.clearTime = 0;
    }
    public void BeginBattle()
    {
        missionStartTime = Time.time;
    }

    public void AddBonusCredits(int amount)
    {
        bonusCredits += amount;
    }
    // public void NewGame()
    // {
    //     Credits = 50000;
    //     bonusCredits = 0;

    //     CurrentMission = firstMission;

    //     Debug.Log("初期ミッション設定 : " + CurrentMission.missionName);
    // }

    [SerializeField] private MechPartData[] starterParts;

    public List<MechPartData> OwnedParts = new List<MechPartData>();
    // [SerializeField] private MechPartData[] starterParts;
    public bool IsOwned(MechPartData part)
    {
        return OwnedParts.Contains(part);
    }
    public bool BuyPart(MechPartData part)
    {
        if (part == null)
            return false;

        if (OwnedParts.Contains(part))
            return false;

        if (Credits < part.price)
            return false;

        Credits -= part.price;

        OwnedParts.Add(part);

        return true;
    }
    public bool SellPart(MechPartData part)
    {
        if (part == null)
            return false;

        if (!OwnedParts.Contains(part))
            return false;

        if (IsEquipped(part))
            return false;

        Credits += part.price;

        OwnedParts.Remove(part);

        return true;
    }
    public bool IsEquipped(MechPartData part)
    {
        return
            CurrentLoadout.Head == part ||
            CurrentLoadout.Body == part ||
            CurrentLoadout.Arms == part ||
            CurrentLoadout.Legs == part ||

            CurrentLoadout.RightWeapon == part ||
            CurrentLoadout.LeftWeapon == part ||

            CurrentLoadout.RightShoulder == part ||
            CurrentLoadout.LeftShoulder == part ||

            CurrentLoadout.Booster == part ||
            CurrentLoadout.FCS == part ||
            CurrentLoadout.Generator == part;
    }
    private void InitializeStarterLoadout()
    {
        foreach(MechPartData part in starterParts)
        {
            if(part == null)
                continue;

            if(part is MechHeadPartData head)
                CurrentLoadout.Head = head;

            else if(part is MechBodyPartData body)
                CurrentLoadout.Body = body;

            else if(part is MechArmsPartData arms)
                CurrentLoadout.Arms = arms;

            else if(part is MechLegsPartData legs)
                CurrentLoadout.Legs = legs;

            else if(part is MechBoosterPartData booster)
                CurrentLoadout.Booster = booster;

            else if(part is MechFCSPartData fcs)
                CurrentLoadout.FCS = fcs;

            else if(part is MechGeneratorPartData generator)
                CurrentLoadout.Generator = generator;
        }
        foreach(MechPartData part in starterParts)
        {
            MechWeaponPartData weapon = part as MechWeaponPartData;

            if(weapon == null)
                continue;

            if(CurrentLoadout.RightWeapon == null)
            {
                CurrentLoadout.RightWeapon = weapon;
                continue;
            }

            if(CurrentLoadout.LeftWeapon == null)
            {
                CurrentLoadout.LeftWeapon = weapon;
                continue;
            }

            if(CurrentLoadout.RightShoulder == null)
            {
                CurrentLoadout.RightShoulder = weapon;
                continue;
            }

            if(CurrentLoadout.LeftShoulder == null)
            {
                CurrentLoadout.LeftShoulder = weapon;
                continue;
            }
        }
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            if(CurrentMission == null)
            {
                CurrentMission = firstMission;
            }

            foreach(MechPartData part in starterParts)
            {
                if(part == null)
                    continue;

                if(!OwnedParts.Contains(part))
                {
                    OwnedParts.Add(part);
                }
            }

            InitializeStarterLoadout();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Debug.Log("OwnedParts Count : " + OwnedParts.Count);
        Debug.Log(CurrentLoadout.Head);
        Debug.Log(CurrentLoadout.Body);
        Debug.Log(CurrentLoadout.Arms);
        Debug.Log(CurrentLoadout.Legs);
        enemyCount = FindObjectsOfType<MechEnemyUnitControl>().Length;
    }

    public void EnemyDestroyed()
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            MissionClear();
        }
    }
    // void MissionClear()
    // {
    //     SceneManager.LoadScene("ResultScene");
    // }
    void MissionClear()
    {
        if(CurrentMission == null)
            return;

        int totalReward = CurrentMission.reward + bonusCredits;

        CurrentResult.title = CurrentMission.missionName;
        CurrentResult.missionReward = CurrentMission.reward;

        CurrentResult.bonusReward = bonusCredits;

        CurrentResult.totalReward = totalReward;
        CurrentResult.clearTime = Time.time - missionStartTime;
        // CurrentResult.arenaRank = CurrentArenaEnemy.arenaRank;
        CurrentResult.rank = CalculateRank();

        MechControl player = FindFirstObjectByType<MechControl>();

        if(player != null)
        {
            CurrentResult.damageTaken = player.totalDamageTaken;
        }

        // CurrentResult.damageTaken = player.totalDamageTaken;

        Credits += totalReward;

        LastReward = totalReward;
        LastResultTitle = CurrentMission.missionName;
        HasResult = true;

        bonusCredits = 0;

        SceneManager.LoadScene("ResultScene");
    }
    public void ArenaClear()
    {
        if(CurrentArenaEnemy == null)
            return;

        int reward = CurrentArenaEnemy.reward;

        string id = CurrentArenaEnemy.enemyName;

        CurrentResult.title = CurrentArenaEnemy.enemyName;

        CurrentResult.arenaRank = CurrentArenaEnemy.arenaRank;

        if(!clearedArenaEnemies.Contains(id))
        {
            Credits += reward;
            clearedArenaEnemies.Add(id);
        }

        LastReward = reward;
        LastResultTitle = CurrentArenaEnemy.enemyName;
        HasResult = true;

        SceneManager.LoadScene("ResultScene");
    }
    private string CalculateRank()
    {
        float score = 0;

        score += CurrentResult.enemyKillCount * 100;

        score += CurrentResult.veinDestroyedCount * 200;

        score -= CurrentResult.damageTaken;

        score -= CurrentResult.clearTime;

        if(score >= 1000)
            return "S";

        if(score >= 700)
            return "A";

        if(score >= 500)
            return "B";

        if(score >= 300)
            return "C";

        return "D";
    }
}
