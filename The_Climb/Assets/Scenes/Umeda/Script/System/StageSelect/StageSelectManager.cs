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

    [Header("デバッグ")]
    public int startStageId = 0;
    public bool[] clearedStages;

    [Header("分岐ルール（Path排他）")]
    public BranchRule[] branchRules;

    [Header("ステージ解放条件（OR依存）")]
    public StageRequirement[] stageRequirements;

    private bool[] prevClearedStages;

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

        prevClearedStages = new bool[clearedStages.Length];
    }

    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return null;
        yield return null;

        for (int i = 0; i < clearedStages.Length; i++)
            clearedStages[i] = false;

        // ★ Stage0 は常にON
        clearedStages[startStageId] = true;

        ApplyAllRules();
        Refresh();
        CopyArray(clearedStages, prevClearedStages);
    }

    private void Update()
    {
        if (!HasClearedChanged())
            return;

        // ★ Stage0は絶対にtrue
        clearedStages[startStageId] = true;

        ApplyAllRules();
        Refresh();
        CopyArray(clearedStages, prevClearedStages);
    }

    // ===============================
    // ルール総適用（核心）
    // ===============================
    private void ApplyAllRules()
    {
        bool changed;
        do
        {
            changed = false;
            changed |= ApplyStageRequirements();
            changed |= ApplyBranchExclusionToClearedStages();

            // ★ 毎ループでStage0を保証
            if (!clearedStages[startStageId])
            {
                clearedStages[startStageId] = true;
                changed = true;
            }
        }
        while (changed);
    }

    // ===============================
    // OR依存（親が1つでも必要）
    // ===============================
    private bool ApplyStageRequirements()
    {
        bool changed = false;

        foreach (var req in stageRequirements)
        {
            // ★ Stage0は無条件
            if (req.stage == startStageId)
                continue;

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
    // 分岐排他（ClearedStages）
    // ===============================
    private bool ApplyBranchExclusionToClearedStages()
    {
        bool changed = false;

        foreach (var rule in branchRules)
        {
            if (!IsValid(rule.triggerStage)) continue;
            if (rule.triggerStage == startStageId) continue;
            if (!clearedStages[rule.triggerStage]) continue;

            for (int i = 0; i < rule.blockFromStages.Length; i++)
            {
                int from = rule.blockFromStages[i];
                int to = rule.blockToStages[i];

                if (to == startStageId) continue;

                if (IsValid(from) && IsValid(to))
                {
                    if (clearedStages[from] && clearedStages[to])
                    {
                        clearedStages[to] = false;
                        changed = true;
                    }
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

        // ★ Stage0は常に有効・クリア扱い
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
