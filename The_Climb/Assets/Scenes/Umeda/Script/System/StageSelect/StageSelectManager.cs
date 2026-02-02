using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    private StageRandomizer stageRandomizer;

    [Header("デバッグ")]
    public int startStageId = 0;
    public bool[] clearedStages;

    [Header("分岐ルール（Path排他）")]
    public BranchRule[] branchRules;

    [Header("ステージ解放条件（OR依存）")]
    public StageRequirement[] stageRequirements;

    private bool[] prevClearedStages;

    private const string CLEARED_KEY = "ClearedStages";
    private const string LAST_CLEARED_KEY = "LastClearedStage";

    // ===============================
    // Lifecycle
    // ===============================
    private void Awake()
    {
        if (stages == null || stages.Length == 0)
            stages = FindObjectsOfType<StageNode>(true);

        if (paths == null || paths.Length == 0)
            paths = FindObjectsOfType<StagePath>(true);

        if (clearedStages == null || clearedStages.Length != stages.Length)
            clearedStages = new bool[stages.Length];

        prevClearedStages = new bool[stages.Length];
    }

    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return null;
        yield return null;

        // ① 永続データロード
        LoadClearedStages();

        // ② Stage0は常にON
        clearedStages[startStageId] = true;

        // ③ 今回クリア分を反映（1回限り）
        ApplyLastClearedStage();

        // ④ ルール適用
        ApplyAllRules();

        // ⑤ 表示更新
        Refresh();

        CopyArray(clearedStages, prevClearedStages);
        SaveClearedStages();
    }

    private void Update()
    {
        if (!HasClearedChanged())
            return;

        // Stage0保証
        if (!clearedStages[startStageId])
            clearedStages[startStageId] = true;

        ApplyAllRules();
        Refresh();

        CopyArray(clearedStages, prevClearedStages);
        SaveClearedStages();
    }

    public void OnStageSelected(int stageId)
    {
        if (!IsValid(stageId))
        {
            stageRandomizer.StartStage(stageId);
            return;
        }
        // ロックされてたら無視
        if (!clearedStages[stageId])
        {
            Debug.Log("ロックされてたら無視");
            return;
        }
            

        Debug.Log($"Stage Selected: {stageId}");

        // 例：次のシーンへ
        PlayerPrefs.SetInt("SelectedStage", stageId);

        //⭐⭐★★
        stageRandomizer.StartStage(stageId);
    }

    // ===============================
    // ルール総適用
    // ===============================
    private void ApplyAllRules()
    {
        bool changed;
        do
        {
            changed = false;
            changed |= ApplyStageRequirements();
            changed |= ApplyBranchExclusionToClearedStages();

            if (!clearedStages[startStageId])
            {
                clearedStages[startStageId] = true;
                changed = true;
            }
        }
        while (changed);
    }

    // ===============================
    // OR依存
    // ===============================
    private bool ApplyStageRequirements()
    {
        bool changed = false;

        foreach (var req in stageRequirements)
        {
            if (req.stage == startStageId) continue;
            if (!IsValid(req.stage)) continue;

            bool satisfied = false;
            foreach (int parent in req.requiredAny)
            {
                if (IsValid(parent) && clearedStages[parent])
                {
                    satisfied = true;
                    break;
                }
            }

            if (!satisfied && clearedStages[req.stage])
            {
                clearedStages[req.stage] = false;
                changed = true;
            }
        }

        return changed;
    }

    // ===============================
    // 分岐排他
    // ===============================
    private bool ApplyBranchExclusionToClearedStages()
    {
        bool changed = false;

        foreach (var rule in branchRules)
        {
            if (!IsValid(rule.triggerStage)) continue;
            if (!clearedStages[rule.triggerStage]) continue;

            for (int i = 0; i < rule.blockFromStages.Length; i++)
            {
                int from = rule.blockFromStages[i];
                int to = rule.blockToStages[i];

                if (to == startStageId) continue;
                if (!IsValid(from) || !IsValid(to)) continue;

                if (clearedStages[from] && clearedStages[to])
                {
                    clearedStages[to] = false;
                    changed = true;
                }
            }
        }

        return changed;
    }

    // ===============================
    // 表示更新
    // ===============================
    private void Refresh()
    {
        HashSet<StagePath> blockedPaths = CalculateBlockedPaths();

        foreach (var s in stages)
        {
            s.gameObject.SetActive(true);
            s.SetLocked();
        }

        foreach (var p in paths)
            p.SetState(StagePath.PathState.Locked);

        stages[startStageId].SetAvailable();
        stages[startStageId].SetCleared();

        for (int i = 0; i < clearedStages.Length; i++)
            if (clearedStages[i])
                stages[i].SetCleared();

        foreach (var path in paths)
        {
            if (blockedPaths.Contains(path)) continue;
            if (!clearedStages[path.fromStage]) continue;

            if (clearedStages[path.toStage])
            {
                path.SetState(StagePath.PathState.Passed);
            }
            else
            {
                path.SetState(StagePath.PathState.Available);
                stages[path.toStage].SetAvailable();
            }
        }
    }

    // ===============================
    // Path排他
    // ===============================
    private HashSet<StagePath> CalculateBlockedPaths()
    {
        HashSet<StagePath> blocked = new HashSet<StagePath>();

        foreach (var rule in branchRules)
        {
            if (!IsValid(rule.triggerStage)) continue;
            if (!clearedStages[rule.triggerStage]) continue;

            for (int i = 0; i < rule.blockFromStages.Length; i++)
            {
                int from = rule.blockFromStages[i];
                int to = rule.blockToStages[i];

                foreach (var path in paths)
                    if (path.fromStage == from && path.toStage == to)
                        blocked.Add(path);
            }
        }
        return blocked;
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
        if (!PlayerPrefs.HasKey(CLEARED_KEY))
            return;

        string[] parts = PlayerPrefs.GetString(CLEARED_KEY).Split(',');

        for (int i = 0; i < parts.Length && i < clearedStages.Length; i++)
            bool.TryParse(parts[i], out clearedStages[i]);
    }

    private void ApplyLastClearedStage()
    {
        if (!PlayerPrefs.HasKey(LAST_CLEARED_KEY)) return;

        int stage = PlayerPrefs.GetInt(LAST_CLEARED_KEY);
        if (IsValid(stage))
            clearedStages[stage] = true;

        PlayerPrefs.DeleteKey(LAST_CLEARED_KEY);
    }

    // ===============================
    // Utility
    // ===============================
    private bool IsValid(int id) => id >= 0 && id < stages.Length;

    private bool HasClearedChanged()
    {
        for (int i = 0; i < clearedStages.Length; i++)
            if (clearedStages[i] != prevClearedStages[i])
                return true;
        return false;
    }

    private void CopyArray(bool[] src, bool[] dst)
    {
        for (int i = 0; i < src.Length; i++)
            dst[i] = src[i];
    }
}
