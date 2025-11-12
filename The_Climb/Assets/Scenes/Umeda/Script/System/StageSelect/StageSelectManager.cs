using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageSelectManager : MonoBehaviour
{
    [Header("参照設定")]
    public StageNode[] stages;
    public StagePath[] paths;

    [Header("デバッグ設定")]
    public int startStageId = 0;
    public bool[] clearedStages;

    private bool[] prevClearedStages;

    private void Awake()
    {
        // 参照確保
        if (stages == null || stages.Length == 0)
            stages = FindObjectsOfType<StageNode>(true);

        if (paths == null || paths.Length == 0)
            paths = FindObjectsOfType<StagePath>(true);

        if (clearedStages == null || clearedStages.Length < stages.Length)
            clearedStages = new bool[stages.Length];

        prevClearedStages = new bool[clearedStages.Length];
    }

    private void OnEnable()
    {
        // OnEnableで遅延初期化
        StartCoroutine(InitializeAfterAllLoaded());
    }

    private IEnumerator InitializeAfterAllLoaded()
    {
        // すべてのオブジェクト（特にPath）がAwake, Startを終えるのを待つ
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // 全チェック → 全解除
        for (int i = 0; i < clearedStages.Length; i++)
            clearedStages[i] = true;
        for (int i = 0; i < clearedStages.Length; i++)
            clearedStages[i] = false;

        clearedStages[startStageId] = true;

        // Refresh実行
        RefreshDebug();
        CopyArray(clearedStages, prevClearedStages);
    }

    private void Update()
    {
        if (Application.isPlaying && HasClearedStagesChanged())
        {
            RefreshDebug();
            CopyArray(clearedStages, prevClearedStages);
        }
    }

    private bool HasClearedStagesChanged()
    {
        if (clearedStages == null || prevClearedStages == null) return false;
        if (clearedStages.Length != prevClearedStages.Length) return true;

        for (int i = 0; i < clearedStages.Length; i++)
        {
            if (clearedStages[i] != prevClearedStages[i])
                return true;
        }
        return false;
    }

    private void CopyArray(bool[] source, bool[] target)
    {
        for (int i = 0; i < source.Length; i++)
            target[i] = source[i];
    }

    public void UnlockStage(int stageId)
    {
        if (stageId < 0 || stageId >= stages.Length) return;
        var stage = stages[stageId];
        if (stage == null) return;

        stage.Unlock();
        clearedStages[stageId] = true;

        foreach (var path in paths)
        {
            if (path.fromStage == stageId)
                path.ShowPath();
        }

        foreach (int nextId in stage.nextStageIds)
        {
            if (nextId < 0 || nextId >= stages.Length) continue;
            var nextStage = stages[nextId];

            if (!nextStage.isUnlocked)
            {
                nextStage.gameObject.SetActive(true);
                nextStage.isUnlocked = true;
            }
        }

        LockConflictingBranches(stageId);
    }

    private void LockConflictingBranches(int clearedId)
    {
        foreach (var path in paths)
        {
            if (path.fromStage != clearedId && !clearedStages[path.fromStage])
            {
                if (clearedStages[clearedId] && !clearedStages[path.toStage])
                    path.HidePath();
            }
        }
    }

    [ContextMenu("Debug Refresh")]
    public void RefreshDebug()
    {
        if (stages == null || paths == null) return;

        foreach (var path in paths)
            path.HidePath();

        foreach (var stage in stages)
        {
            stage.gameObject.SetActive(false);
            stage.isUnlocked = false;
        }

        for (int i = 0; i < clearedStages.Length; i++)
        {
            if (clearedStages[i])
                UnlockStage(i);
        }

        if (startStageId >= 0 && startStageId < stages.Length)
            stages[startStageId].gameObject.SetActive(true);
    }
}
