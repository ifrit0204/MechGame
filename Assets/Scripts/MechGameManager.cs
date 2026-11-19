using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MechGameManager : MonoBehaviour
{
    private const string LogTag = "[MissionVoice] ";

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

    [Header("Mission Complete Voice (ミッション完了ボイス・字幕)")]
    // ミッション側(MechMissionData)に指定がないときに使うボイスと字幕
    [SerializeField] private AudioClip defaultCompleteVoice;
    [SerializeField, TextArea(1, 3)] private string defaultCompleteSubtitle;
    // アリーナの敵側(MechArenaEnemyData.clearVoice)に指定がないときに使うボイスと字幕
    // (どちらも空なら、すぐリザルトへ移る)
    [SerializeField] private AudioClip arenaCompleteVoice;
    [SerializeField, TextArea(1, 3)] private string arenaCompleteSubtitle;
    // 最後の敵を倒してから、ボイスが始まるまでの間(秒)
    [SerializeField] private float voiceStartDelay = 0.8f;
    // ボイスが終わってから、リザルトへ移るまでの間(秒)
    [SerializeField] private float resultDelayAfterVoice = 0.8f;
    [SerializeField, Range(0f, 1f)] private float voiceVolume = 1f;

    [Header("Subtitle (字幕)")]
    // ボイスがなく字幕だけのとき、最低限表示する時間(秒)
    [SerializeField] private float subtitleMinDuration = 2f;
    // ボイスがなく字幕だけのとき、1文字あたりの表示時間(秒)
    [SerializeField] private float subtitleSecondsPerChar = 0.12f;

    private AudioSource voiceSource;

    // 登場ボイスなど、待ち時間つきで再生を予約しているもの
    private Coroutine voiceLineRoutine;

    // ミッション完了の演出中(ボイス再生中)か
    private bool isClearing;
    public bool IsMissionClearing { get { return isClearing; } }

    public MechResultData CurrentResult = new MechResultData();
    public void StartMissionResult()
    {
        CurrentResult = new MechResultData();

        CurrentResult.clearTime = 0;
    }
    public void BeginBattle()
    {
        missionStartTime = Time.time;

        // 前のミッションの演出フラグが残らないようにする(このオブジェクトはシーンをまたいで残る)
        isClearing = false;
    }

    public void AddBonusCredits(int amount)
    {
        bonusCredits += amount;
    }

    [SerializeField] private MechPartData[] starterParts;

    public List<MechPartData> OwnedParts = new List<MechPartData>();
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

            // ボイス再生用の AudioSource(2D)。なければ自動で追加する
            voiceSource = GetComponent<AudioSource>();

            if (voiceSource == null)
                voiceSource = gameObject.AddComponent<AudioSource>();

            voiceSource.playOnAwake = false;
            voiceSource.spatialBlend = 0f;

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
            // 既に別の GameManager が残っているので、こちらは破棄される。
            // このシーンのインスペクターで設定した値(ボイスなど)は使われず、
            // 残っている側の GameManager の値が使われる。
            Debug.LogWarning(LogTag + "シーン「" + gameObject.scene.name + "」の GameManager は、"
                + "既に GameManager が存在するため破棄されます。ボイスの設定は、残る側の GameManager のものが使われます");

            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // シーンが切り替わったら、予約中のボイスと再生中のボイスを止める
    // (登場ボイスの途中でやられて、リザルトにまで声が残らないようにする)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopVoiceLine();

        if (voiceSource != null)
            voiceSource.Stop();
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

    public void InitializeEnemyCount()
    {
        enemyCount = FindObjectsOfType<MechEnemyUnitControl>().Length;

        Debug.Log("敵数 : " + enemyCount);
    }

    public void EnemyDestroyed()
    {
        enemyCount--;
        Debug.Log("EnemyDestroyed");
        Debug.Log("残敵数 : " + enemyCount);
        if (enemyCount <= 0)
        {
            MissionClear();
        }
    }
    void MissionClear()
    {
        if(CurrentMission == null)
        {
            Debug.LogWarning(LogTag + "MissionClear: CurrentMission が空のため、何もせず戻ります(リザルトへも移りません)");
            return;
        }

        // 二重に呼ばれても、演出とリザルト遷移は1回だけ
        if (isClearing)
            return;

        isClearing = true;

        int totalReward = CurrentMission.reward + bonusCredits;

        CurrentResult.title = CurrentMission.missionName;
        CurrentResult.missionReward = CurrentMission.reward;

        CurrentResult.bonusReward = bonusCredits;

        CurrentResult.totalReward = totalReward;

        // クリアタイムは、ボイスを流す前の時点で確定させる(ボイスの時間を含めない)
        CurrentResult.clearTime = Time.time - missionStartTime;
        CurrentResult.rank = CalculateRank();

        CurrentResult.isArena = false;

        MechControl player = FindFirstObjectByType<MechControl>();

        if(player != null)
        {
            CurrentResult.damageTaken = player.totalDamageTaken;
        }

        Credits += totalReward;

        LastReward = totalReward;
        LastResultTitle = CurrentMission.missionName;
        HasResult = true;

        bonusCredits = 0;

        // ミッション固有の設定 → なければ既定の設定 → それもなければ、すぐリザルトへ
        // (ボイスと字幕は、必ず同じ側のものを組み合わせて使う)
        AudioClip voice;
        string subtitle;

        if (HasLine(CurrentMission.completeVoice, CurrentMission.completeSubtitle))
        {
            voice = CurrentMission.completeVoice;
            subtitle = CurrentMission.completeSubtitle;
        }
        else
        {
            voice = defaultCompleteVoice;
            subtitle = defaultCompleteSubtitle;
        }

        StartCoroutine(ResultSequence(voice, subtitle));
    }
    public void ArenaClear()
    {
        if(CurrentArenaEnemy == null)
            return;

        if (isClearing)
            return;

        isClearing = true;

        int reward = CurrentArenaEnemy.reward;

        string id = CurrentArenaEnemy.enemyName;

        CurrentResult.title = CurrentArenaEnemy.enemyName;

        CurrentResult.arenaRank = CurrentArenaEnemy.arenaRank;

        CurrentResult.isArena = true;

        if(!clearedArenaEnemies.Contains(id))
        {
            Credits += reward;
            clearedArenaEnemies.Add(id);
        }

        LastReward = reward;
        LastResultTitle = CurrentArenaEnemy.enemyName;
        HasResult = true;

        // この敵の撃破ボイス・字幕 → なければ共通のアリーナ用 → それもなければ、すぐリザルトへ
        AudioClip voice;
        string subtitle;

        if (HasLine(CurrentArenaEnemy.clearVoice, CurrentArenaEnemy.clearSubtitle))
        {
            voice = CurrentArenaEnemy.clearVoice;
            subtitle = CurrentArenaEnemy.clearSubtitle;
        }
        else
        {
            voice = arenaCompleteVoice;
            subtitle = arenaCompleteSubtitle;
        }

        StartCoroutine(ResultSequence(voice, subtitle));
    }

    // ボイスと字幕を同時に出し、終わってからリザルト画面へ移る。
    // どちらもなければ、待たずにすぐ移る(従来どおり)。
    private IEnumerator ResultSequence(AudioClip voice, string subtitle)
    {
        bool hasVoice = voice != null && voiceSource != null;
        bool hasSubtitle = !string.IsNullOrEmpty(subtitle);

        if (hasVoice || hasSubtitle)
        {
            // 予約中の別のボイス(登場ボイスなど)があれば止める
            StopVoiceLine();

            // 最後の敵の爆発などが落ち着くまで、少し待つ
            yield return new WaitForSecondsRealtime(voiceStartDelay);

            float duration = GetLineDuration(hasVoice ? voice : null, subtitle);

            if (hasVoice)
                PlayVoice(voice);

            if (hasSubtitle)
                MechSubtitleUI.Show(subtitle, duration);

            // ボイスの長さ + 余韻 だけ待つ
            yield return new WaitForSecondsRealtime(duration + resultDelayAfterVoice);

            MechSubtitleUI.Hide();
        }

        isClearing = false;

        SceneManager.LoadScene("ResultScene");
    }

    // ------------------------------------------------------------
    // ボイス・字幕(どこからでも呼べる)
    // ------------------------------------------------------------

    // ボイスを再生し、同時に字幕を表示する。
    // ボイスだけ、字幕だけでもよい。delay 秒待ってから始める。
    // 例: アリーナの敵の登場ボイス
    public void PlayVoiceLine(AudioClip voice, string subtitle, float delay = 0f)
    {
        if (!HasLine(voice, subtitle))
            return;

        StopVoiceLine();

        voiceLineRoutine = StartCoroutine(VoiceLineRoutine(voice, subtitle, delay));
    }

    private IEnumerator VoiceLineRoutine(AudioClip voice, string subtitle, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay);

        float duration = GetLineDuration(voice, subtitle);

        PlayVoice(voice);

        if (!string.IsNullOrEmpty(subtitle))
            MechSubtitleUI.Show(subtitle, duration);

        voiceLineRoutine = null;
    }

    private void StopVoiceLine()
    {
        if (voiceLineRoutine != null)
        {
            StopCoroutine(voiceLineRoutine);
            voiceLineRoutine = null;
        }
    }

    public void StopBriefing()
    {
        StopVoiceLine();

        if (voiceSource != null)
            voiceSource.Stop();

        MechSubtitleUI.Hide();
    }

    private void PlayVoice(AudioClip voice)
    {
        if (voice == null || voiceSource == null)
            return;

        voiceSource.Stop();
        voiceSource.clip = voice;
        voiceSource.volume = voiceVolume;
        voiceSource.Play();
    }

    private static bool HasLine(AudioClip voice, string subtitle)
    {
        return voice != null || !string.IsNullOrEmpty(subtitle);
    }

    // 字幕を出し続ける時間。ボイスがあればその長さ、なければ文字数から決める。
    private float GetLineDuration(AudioClip voice, string subtitle)
    {
        if (voice != null)
            return voice.length;

        int length = string.IsNullOrEmpty(subtitle) ? 0 : subtitle.Length;

        return Mathf.Max(subtitleMinDuration, length * subtitleSecondsPerChar);
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
