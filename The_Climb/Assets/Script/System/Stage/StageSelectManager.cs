using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BranchRule
{
    public int triggerStage; // トリガーとなるステージ番号
    public int[] blockFromStages; // ブロックの起点ステージ群
    public int[] blockToStages; // ブロックの対象ステージ群
}

[System.Serializable]
public class StageRequirement
{
    public int stage; // 対象のステージ番号
    public int[] requiredAny; // 解放に必要なステージ番号のいずれか
}

public class StageSelectManager : MonoBehaviour
{
    [Header("参照")]
    public StageNode[] stages; // ステージノードの配列
    public StagePath[] paths; // ステージパスの配列

    [Header("開始ステージ")]
    public int startStageId = 0; // 最初に解放されるステージID

    [Header("分岐ルール（排他）")]
    public BranchRule[] branchRules; // 分岐ルールの配列

    [Header("OR解放条件")]
    public StageRequirement[] stageRequirements; // 解放条件の配列

    [Header("デバッグ：クリア状態")]
    [SerializeField] private bool[] clearedStages; // ステージごとのクリア状況
    bool[] selectedByBranch; // 分岐によって選択されたかどうかの管理

    [Header("デバッグ：分岐シミュレーション")]
    [SerializeField] private DebugBranch[] debugBranches; // デバッグ用の分岐設定

    [System.Serializable]
    public class DebugBranch
    {
        public int branchIndex; // 分岐番号
        public int[] parentStages; // 有効化条件となるステージ群
        public int[] selectableStages; // 選択可能なステージ群

        [HideInInspector]
        public int selectedStage = -1; // 現在選択されているステージ
    }

    private const string CLEARED_KEY = "ClearedStages"; // 保存用のキー

    // インスタンス作成時に実行される関数
    private void Awake()
    {
        // 参照が設定されていない場合は自動取得
        if (stages == null || stages.Length == 0)
            stages = FindObjectsOfType<StageNode>(true);

        if (paths == null || paths.Length == 0)
            paths = FindObjectsOfType<StagePath>(true);

        clearedStages = new bool[stages.Length]; // 配列を初期化
        selectedByBranch = new bool[stages.Length]; // 配列を初期化
    }

    // オブジェクトが有効になった時に実行される関数
    private void OnEnable()
    {
        StartCoroutine(Init()); // 初期化プロセスを開始する関数の呼び出し
    }

    // 初期化プロセスのコルーチン
    private IEnumerator Init()
    {
        yield return null; // 1フレーム待機

        // タイトルから来た場合は全リセット
        if (PlayerPrefs.GetInt("GameStart") == 1)
        {
            ResetAllStages(); // 全リセットを行う関数の呼び出し
        }
        else
        {
            LoadClearedStages(); // 保存された状況を読み込む関数の呼び出し
        }

        // 開始ステージは常にクリア（選択可能）扱いとする
        if (IsValid(startStageId))
            clearedStages[startStageId] = true;

        // ステージ進入時の自動解放（番号+1）の処理
        if (PlayerPrefs.HasKey("JustClearedStageId"))
        {
            int nextToClear = PlayerPrefs.GetInt("JustClearedStageId"); // 解放すべきIDを取得

            if (IsValid(nextToClear))
            {
                clearedStages[nextToClear] = true; // 指定されたステージをクリア済みに設定
                Debug.Log($"StageSelectManager: インデックス {nextToClear} を解放しました（进入ボーナス）");
            }

            PlayerPrefs.DeleteKey("JustClearedStageId"); // キーを削除して重複処理を防止
        }

        ApplyAllRules(); // ルールを適用する関数の呼び出し
        Refresh(); // 表示を最新の状態に更新する関数の呼び出し
        SaveClearedStages(); // 現在の状況を保存する関数の呼び出し
    }

    // 全ステージのクリア状況をリセットする関数
    private void ResetAllStages()
    {
        for (int i = 0; i < clearedStages.Length; i++)
        {
            clearedStages[i] = false; // クリア状況を偽（未クリア）に設定
        }
        PlayerPrefs.DeleteKey(CLEARED_KEY); // 保存データを削除
        Debug.Log("StageSelectManager: クリア状況をすべてリセットしました");
    }

    // ステージが選択された時に呼ばれる関数（外部UI等から）
    public void OnStageSelected(int stageId)
    {
        if (!IsValid(stageId)) return;

        PlayerPrefs.SetInt("CurrentStageId", stageId); // 現在のステージIDを保存
        PlayerPrefs.Save();

        // StageRandomizerを通じてステージを開始
        FindObjectOfType<StageRandomizer>().StartStage(stageId);
    }

    // すべてのルールを適用する関数
    private void ApplyAllRules()
    {
        ApplyStageRequirements(); // 解放条件を適用する関数の呼び出し
    }

    // 解放条件（OR条件）を適用する関数
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
                    ok = true; // 条件のいずれかがクリアされていればOK
                    break;
                }
            }

            if (!ok)
                clearedStages[req.stage] = false; // 条件を満たしていなければ未クリアに変更
        }
    }

    // 画面表示（ステージボタンやパス）を最新の状態に更新する関数
    private void Refresh()
    {
        // 全要素を一旦ロック状態に設定
        foreach (var s in stages)
        {
            s.isInteractable = false;
            s.SetLocked(); // ロック演出を呼び出す
        }

        foreach (var p in paths)
            p.SetState(StagePath.PathState.Locked); // 経路をロック状態に設定

        // クリア済みステージの演出を適用
        for (int i = 0; i < stages.Length; i++)
        {
            if (!clearedStages[i]) continue;

            stages[i].SetCleared(); // クリア演出を呼び出す
        }

        // 進行可能な経路とステージを有効化
        foreach (var path in paths)
        {
            if (!clearedStages[path.fromStage]) continue;
            if (IsBlockedPath(path)) continue;

            if (clearedStages[path.toStage])
            {
                path.SetState(StagePath.PathState.Passed); // 通過済み演出を呼び出す
            }
            else
            {
                path.SetState(StagePath.PathState.Available); // 進行可能演出を呼び出す
                stages[path.toStage].isInteractable = true; // ボタン操作を可能にする
                stages[path.toStage].SetAvailable(); // 進行可能演出を呼び出す
            }
        }
    }

    // 指定されたパスがブロックされているか判定する関数
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
                    return true; // ブロック対象であれば真を返す
            }
        }
        return false;
    }

    // クリア状況を保存する関数
    private void SaveClearedStages()
    {
        string data = string.Join(",", clearedStages); // 配列を文字列に変換
        PlayerPrefs.SetString(CLEARED_KEY, data); // 文字列を保存
        PlayerPrefs.Save();
    }

    // 保存されたクリア状況を読み込む関数
    private void LoadClearedStages()
    {
        if (!PlayerPrefs.HasKey(CLEARED_KEY)) return;

        string[] parts = PlayerPrefs.GetString(CLEARED_KEY).Split(','); // 文字列を分割
        for (int i = 0; i < parts.Length && i < clearedStages.Length; i++)
            bool.TryParse(parts[i], out clearedStages[i]); // ブール値に変換して代入
    }

    // IDが配列の範囲内かチェックする関数
    private bool IsValid(int id) => id >= 0 && id < stages.Length;

#if UNITY_EDITOR
    // エディタ上での設定変更時に呼ばれるデバッグ関数
    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        if (stages == null || clearedStages == null) return;

        ApplyDebugBranches(); // デバッグ用分岐を適用する関数の呼び出し
        ApplyAllRules(); // ルールを適用する関数の呼び出し
        Refresh(); // 表示を更新する関数の呼び出し
        SaveClearedStages(); // 状況を保存する関数の呼び出し
    }

    // デバッグ用の分岐シミュレーションを適用する関数
    private void ApplyDebugBranches()
    {
        for (int i = 0; i < clearedStages.Length; i++)
        {
            clearedStages[i] = false;
            selectedByBranch[i] = false;
        }

        if (IsValid(startStageId))
            clearedStages[startStageId] = true;

        bool changed;
        do
        {
            changed = false;
            foreach (var b in debugBranches)
            {
                bool parentOK = true;
                foreach (int p in b.parentStages)
                {
                    if (!IsValid(p) || !clearedStages[p])
                    {
                        parentOK = false;
                        break;
                    }
                }

                if (!parentOK)
                {
                    if (b.selectedStage != -1)
                    {
                        b.selectedStage = -1;
                        changed = true;
                    }
                    continue;
                }

                if (b.selectedStage != -1 && IsValid(b.selectedStage))
                {
                    if (!clearedStages[b.selectedStage])
                    {
                        clearedStages[b.selectedStage] = true;
                        selectedByBranch[b.selectedStage] = true;
                        changed = true;
                    }
                }
            }
        }
        while (changed);
    }
#endif
}
