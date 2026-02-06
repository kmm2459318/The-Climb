using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static StageSelectManager;

[System.Serializable]
public class BranchRule
{
    public int triggerStage;
    public int[] blockFromStages;
    public int[] blockToStages;
}

[System.Serializable]
public class StageRequirement
{
    public int stage;
    public int[] requiredAny;
}

public class StageSelectManager : MonoBehaviour
{
    [Header("参照")]
    public StageNode[] stages;
    public StagePath[] paths;

    [Header("開始ステージ")]
    public int startStageId = 0;

    [Header("分岐ルール（排他）")]
    public BranchRule[] branchRules;

    [Header("OR解放条件")]
    public StageRequirement[] stageRequirements;

    [Header("デバッグ：クリア状態")]
    [SerializeField] private bool[] clearedStages;
    bool[] selectedByBranch;

    [Header("デバッグ：分岐シミュレーション")]
    [SerializeField] private DebugBranch[] debugBranches;

    [System.Serializable]
    public class DebugBranch
    {
        public int branchIndex;          // 分岐番号（0,1,2...）
        public int[] parentStages;       // この分岐が有効になる条件
        public int[] selectableStages;   // この分岐で選べるステージ

        [HideInInspector]
        public int selectedStage = -1;   // 選択中（-1 = 未選択）
    }

    private const string CLEARED_KEY = "ClearedStages";

    // ===============================
    // Unity Lifecycle
    // ===============================
    private void Awake()
    {
        if (stages == null || stages.Length == 0)
            stages = FindObjectsOfType<StageNode>(true);

        if (paths == null || paths.Length == 0)
            paths = FindObjectsOfType<StagePath>(true);

        clearedStages = new bool[stages.Length];

        selectedByBranch = new bool[stages.Length];
    }

    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return null;

        LoadClearedStages();

        // Start は常にクリア
        if (IsValid(startStageId))
            clearedStages[startStageId] = true;

        //// ステージ突入＝即クリア
        //if (PlayerPrefs.HasKey("CurrentStageId"))
        //{
        //    int entered = PlayerPrefs.GetInt("CurrentStageId");
        //    if (IsValid(entered))
        //        clearedStages[entered] = true;

        //    PlayerPrefs.DeleteKey("CurrentStageId");
        //}

        // ★ ゴールしたステージのみをクリア扱いにする
        if (PlayerPrefs.HasKey("JustClearedStageId"))
        {
            int cleared = PlayerPrefs.GetInt("JustClearedStageId");

            if (IsValid(cleared))
                clearedStages[cleared] = true;

            PlayerPrefs.DeleteKey("JustClearedStageId");
        }

        ApplyAllRules();
        Refresh();
        SaveClearedStages();
    }

    // ===============================
    // 外部呼び出し
    // ===============================
    public void OnStageSelected(int stageId)
    {
        if (!IsValid(stageId)) return;

        PlayerPrefs.SetInt("CurrentStageId", stageId);
        PlayerPrefs.Save();

        FindObjectOfType<StageRandomizer>().StartStage(stageId);
    }

    // ===============================
    // ルール適用
    // ===============================
    private void ApplyAllRules()
    {
        ApplyStageRequirements();
    }

    // OR 条件
    private void ApplyStageRequirements()
    {
        foreach (var req in stageRequirements)
        {
            if (!IsValid(req.stage)) continue;

            bool ok = false;
            foreach (int p in req.requiredAny)
            {
                if (IsValid(p) && clearedStages[p])
                {
                    ok = true;
                    break;
                }
            }

            if (!ok)
                clearedStages[req.stage] = false;
        }
    }

    // ===============================
    // 表示更新（Path 主体）
    // ===============================
    private void Refresh()
    {
        // ① 初期化：全部 Locked
        foreach (var s in stages)
        {
            s.isInteractable = false;
            s.SetLocked();
        }

        foreach (var p in paths)
            p.SetState(StagePath.PathState.Locked);

        // ② クリア済み
        for (int i = 0; i < stages.Length; i++)
        {
            if (!clearedStages[i]) continue;

            stages[i].SetCleared();
        }

        // ③ Path ベースで進行可能判定
        foreach (var path in paths)
        {
            if (!clearedStages[path.fromStage]) continue;
            if (IsBlockedPath(path)) continue;

            if (clearedStages[path.toStage])
            {
                path.SetState(StagePath.PathState.Passed);
            }
            else
            {
                path.SetState(StagePath.PathState.Available);
                stages[path.toStage].isInteractable = true;
                stages[path.toStage].SetAvailable();
            }
        }
    }

    // ===============================
    // 分岐判定（★ここが核心）
    // ===============================
    private bool IsBlockedPath(StagePath path)
    {
        foreach (var rule in branchRules)
        {
            if (!IsValid(rule.triggerStage)) continue;
            if (!selectedByBranch[rule.triggerStage]) continue;

            bool fromMatch = false;
            foreach (int f in rule.blockFromStages)
            {
                if (path.fromStage == f)
                {
                    fromMatch = true;
                    break;
                }
            }
            if (!fromMatch) continue;

            foreach (int t in rule.blockToStages)
            {
                if (path.toStage == t)
                    return true;
            }
        }
        return false;
    }

    // ===============================
    // Save / Load
    // ===============================
    private void SaveClearedStages()
    {
        string data = string.Join(",", clearedStages);
        PlayerPrefs.SetString(CLEARED_KEY, data);
        PlayerPrefs.Save();
    }

    private void LoadClearedStages()
    {
        if (!PlayerPrefs.HasKey(CLEARED_KEY)) return;

        string[] parts = PlayerPrefs.GetString(CLEARED_KEY).Split(',');
        for (int i = 0; i < parts.Length && i < clearedStages.Length; i++)
            bool.TryParse(parts[i], out clearedStages[i]);
    }

    // ===============================
    // Utility
    // ===============================
    private bool IsValid(int id) => id >= 0 && id < stages.Length;

    private void ApplyDebugBranches()
    {
        // =========================
        // 全リセット
        // =========================
        for (int i = 0; i < clearedStages.Length; i++)
        {
            clearedStages[i] = false;
            selectedByBranch[i] = false;
        }

        // Start は常にクリア扱い
        if (IsValid(startStageId))
            clearedStages[startStageId] = true;

        // =========================
        // 分岐の連鎖解決
        // =========================
        bool changed;
        do
        {
            changed = false;

            foreach (var b in debugBranches)
            {
                // ---------
                // 親条件チェック
                // ---------
                bool parentOK = true;
                foreach (int p in b.parentStages)
                {
                    if (!IsValid(p) || !clearedStages[p])
                    {
                        parentOK = false;
                        break;
                    }
                }

                // 親が成立していない → この分岐は無効
                if (!parentOK)
                {
                    if (b.selectedStage != -1)
                    {
                        b.selectedStage = -1;
                        changed = true;
                    }
                    continue;
                }

                // ---------
                // 選択ステージ反映
                // ---------
                if (b.selectedStage != -1 && IsValid(b.selectedStage))
                {
                    if (!clearedStages[b.selectedStage])
                    {
                        clearedStages[b.selectedStage] = true;
                        selectedByBranch[b.selectedStage] = true; // ★重要
                        changed = true;
                    }
                }
            }
        }
        while (changed); // 分岐2 → 分岐3 などの連鎖対応
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        if (stages == null || clearedStages == null) return;

        ApplyDebugBranches();
        ApplyAllRules();
        Refresh();
        SaveClearedStages();
    }
#endif
}
