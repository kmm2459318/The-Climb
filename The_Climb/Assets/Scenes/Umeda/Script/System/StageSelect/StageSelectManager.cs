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
        StartCoroutine(InitializeAfterAllLoaded());
    }

    private IEnumerator InitializeAfterAllLoaded()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // デバッグ用初期化
        for (int i = 0; i < clearedStages.Length; i++)
            clearedStages[i] = false;

        clearedStages[startStageId] = true;

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

    // ===============================
    // ここがマップ管理の核
    // ===============================
    [ContextMenu("Debug Refresh")]
    public void RefreshDebug()
    {
        // ① 全ステージを表示 & ロック（黒）
        foreach (var stage in stages)
        {
            stage.gameObject.SetActive(true);   // ★非表示にしない
            stage.SetLocked();
        }

        // ② 全パスをロック（黒）
        foreach (var path in paths)
            path.SetState(StagePath.PathState.Locked);

        // ③ スタート地点（銀）
        if (startStageId >= 0 && startStageId < stages.Length)
            stages[startStageId].SetAvailable();

        // ④ クリア済みステージ（金）＋通過済みの道（金）
        for (int i = 0; i < clearedStages.Length; i++)
        {
            if (!clearedStages[i]) continue;

            stages[i].SetCleared();

            foreach (var path in paths)
            {
                if (path.fromStage == i && clearedStages[path.toStage])
                    path.SetState(StagePath.PathState.Passed);
            }
        }

        // ⑤ 次に行けるステージ（銀）＋道（銀）
        for (int i = 0; i < clearedStages.Length; i++)
        {
            if (!clearedStages[i]) continue;

            foreach (var path in paths)
            {
                if (path.fromStage == i && !clearedStages[path.toStage])
                {
                    path.SetState(StagePath.PathState.Available);
                    stages[path.toStage].SetAvailable();
                }
            }
        }
    }

    // ===============================
    // ステージクリア時に呼ぶ
    // ===============================
    public void UnlockStage(int stageId)
    {
        if (stageId < 0 || stageId >= clearedStages.Length)
            return;

        clearedStages[stageId] = true;
        RefreshDebug();
        CopyArray(clearedStages, prevClearedStages);
    }
}
