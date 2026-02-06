using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ステージの分岐ルールを定義するクラス
[System.Serializable]
public class BranchRule
{
    public int triggerStage; // トリガーとなるステージ番号
    public int[] blockFromStages; // ブロックの起点ステージ群
    public int[] blockToStages; // ブロックの対象ステージ群
}

// ステージの解放条件を定義するクラス
[System.Serializable]
public class StageRequirement
{
    public int stage; // 対象のステージ番号
    public int[] requiredAny; // 解放に必要なステージ番号のいずれか
}

// ステージ選択画面を管理するメインクラス
public class StageSelectManager : MonoBehaviour
{
    [Header("参照")]
    public StageNode[] stages; // 全ステージノードの配列
    public StagePath[] paths; // 全ステージパスの配列

    [Header("開始ステージ")]
    public int startStageId = 0; // 最初に解放されるステージID

    [Header("分岐ルール（排他）")]
    public BranchRule[] branchRules; // 用意された分岐ルールのリスト

    [Header("OR解放条件")]
    public StageRequirement[] stageRequirements; // いずれかのクリアで解放される条件リスト

    [Header("デバッグ：クリア状態")]
    [SerializeField] private bool[] clearedStages; // ステージごとのクリア状況（要素番号がそのままステージID）
    private bool[] selectedByBranch; // 分岐によって通過したかどうかの判定用

    [Header("デバッグ：分岐シミュレーション")]
    [SerializeField] private DebugBranch[] debugBranches; // エディタでのデバッグ用

    [System.Serializable]
    public class DebugBranch
    {
        public int branchIndex;          // 分岐の識別番号
        public int[] parentStages;       // この分岐が有効になる親ステージ
        public int[] selectableStages;   // この分岐で選択可能なステージ

        [HideInInspector]
        public int selectedStage = -1;   // 現在の選択状態（-1は未選択）
    }

    private const string CLEARED_KEY = "ClearedStages"; // PlayerPrefsで使用するキー

    // ===============================
    // Unity ライフサイクル関数
    // ===============================

    // インスタンス作成時に呼ばれる初期設定
    private void Awake()
    {
        // 参照が未設定ならシーン内から自動取得
        if (stages == null || stages.Length == 0)
            stages = FindObjectsOfType<StageNode>(true);

        if (paths == null || paths.Length == 0)
            paths = FindObjectsOfType<StagePath>(true);

        clearedStages = new bool[stages.Length]; // 配列を初期化
        selectedByBranch = new bool[stages.Length]; // 配列を初期化
    }

    // オブジェクトが有効になった時に初期化処理を開始
    private void OnEnable()
    {
        StartCoroutine(Init()); // コルーチンによる初期化の呼び出し
    }

    // 初期化プロセスのメインロジック
    private IEnumerator Init()
    {
        yield return null; // 1フレーム待機（他スクリプトのStart完了を待つ）

        // タイトルから「GameStart」経由で来た場合は全リセット
        if (PlayerPrefs.GetInt("GameStart") == 1)
        {
            ResetAllStages(); // 全リセット関数の呼び出し
            // GameStartフラグはStageRandomizer側で0にされる想定だが、ここでもリセットを考慮
        }
        else
        {
            LoadClearedStages(); // 前回保存された状況を読み出す
        }

        // 開始ステージは常に「クリア（解放）」扱いとする
        if (IsValid(startStageId))
            clearedStages[startStageId] = true;

        // ★ ステージ進入時の自動解放処理
        // StageRandomizerから渡された JustClearedStageId（ステージ番号+1）を処理
        if (PlayerPrefs.HasKey("JustClearedStageId"))
        {
            int nextToClear = PlayerPrefs.GetInt("JustClearedStageId");

            if (IsValid(nextToClear))
            {
                clearedStages[nextToClear] = true; // 次のステージを解放
                Debug.Log($"StageSelectManager: インデックス {nextToClear} を解放しました（进入ボーナス）");
            }

            PlayerPrefs.DeleteKey("JustClearedStageId"); // 重複処理を防ぐためにキーを削除
        }

        ApplyAllRules(); // 解放ルールや分岐の適用
        Refresh(); // 表示状態（色やボタンの有効化）の更新
        SaveClearedStages(); // 現在の状況を保存
    }

    // すべてのステージ状況を初期化する関数
    private void ResetAllStages()
    {
        for (int i = 0; i < clearedStages.Length; i++)
        {
            clearedStages[i] = false; // すべて未クリアにリセット
        }
        PlayerPrefs.DeleteKey(CLEARED_KEY); // 保存データも削除
        Debug.Log("StageSelectManager: すべてのステージ解放状況をリセットしました");
    }

    // ===============================
    // 外部からのボタン操作呼び出し用
    // ===============================
    public void OnStageSelected(int stageId)
    {
        if (!IsValid(stageId)) return;

        PlayerPrefs.SetInt("CurrentStageId", stageId); // 現在のステージIDを保存
        PlayerPrefs.Save();

        // StageRandomizerの機能を呼び出してステージを開始
        FindObjectOfType<StageRandomizer>().StartStage(stageId);
    }

    // ===============================
    // 解放ルールの適用
    // ===============================
    private void ApplyAllRules()
    {
        ApplyStageRequirements(); // OR条件の適用
    }

    // 指定された複数の前身ステージのいずれかがクリアされていれば解放（OR条件）
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
                    ok = true; // 1つでもクリアされていれば解放可能
                    break;
                }
            }

            if (!ok)
                clearedStages[req.stage] = false; // 条件を満たさない場合は未解放に設定
        }
    }

    // ===============================
    // 表示状態の更新（パスとノード）
    // ===============================
    private void Refresh()
    {
        // ① 全体を「ロック状態」で初期化
        foreach (var s in stages)
        {
            s.isInteractable = false;
            s.SetLocked(); // ロック状態の演出
        }

        foreach (var p in paths)
            p.SetState(StagePath.PathState.Locked); // パスをロック状態に

        // ② クリア済みステージの演出を適用
        for (int i = 0; i < stages.Length; i++)
        {
            if (!clearedStages[i]) continue;

            stages[i].SetCleared(); // クリア状態の演出
        }

        // ③ 条件を満たすパスとターゲットステージを「有効（進行可能）」にする
        foreach (var path in paths)
        {
            if (!clearedStages[path.fromStage]) continue; // 出発点がクリアされていないならスキップ
            if (IsBlockedPath(path)) continue; // 分岐ルールによって遮断されているならスキップ

            if (clearedStages[path.toStage])
            {
                path.SetState(StagePath.PathState.Passed); // 到着点もクリア済みなら通過済み表示
            }
            else
            {
                path.SetState(StagePath.PathState.Available); // 到着点が未クリアなら進行可能表示
                stages[path.toStage].isInteractable = true; // ボタン操作を有効化
                stages[path.toStage].SetAvailable(); // 次に選べる状態の演出
            }
        }
    }

    // ===============================
    // 分岐ルールによるブロック判定
    // ===============================
    private bool IsBlockedPath(StagePath path)
    {
        foreach (var rule in branchRules)
        {
            if (!IsValid(rule.triggerStage)) continue;
            if (!selectedByBranch[rule.triggerStage]) continue; // トリガーとなる分岐が未選択なら無視

            // 出発点がブロックルールの対象かチェック
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

            // 到着点がブロック対象かチェック
            foreach (int t in rule.blockToStages)
            {
                if (path.toStage == t)
                    return true; // 遮断すべきパスであれば真を返す
            }
        }
        return false;
    }

    // ===============================
    // セーブとロード（PlayerPrefs）
    // ===============================
    private void SaveClearedStages()
    {
        string data = string.Join(",", clearedStages); // 配列を文字列に変換
        PlayerPrefs.SetString(CLEARED_KEY, data); // 文字列として保存
        PlayerPrefs.Save();
    }

    private void LoadClearedStages()
    {
        if (!PlayerPrefs.HasKey(CLEARED_KEY)) return;

        string[] parts = PlayerPrefs.GetString(CLEARED_KEY).Split(','); // カンマで分割
        for (int i = 0; i < parts.Length && i < clearedStages.Length; i++)
            bool.TryParse(parts[i], out clearedStages[i]); // ブール値に変換して代入
    }

    // ===============================
    // 便利機能（ユーティリティ）
    // ===============================
    
    // IDが有効な範囲内か判定する関数
    private bool IsValid(int id) => id >= 0 && id < stages.Length;

    // エディタデバッグ用の分岐ロジック適用
    private void ApplyDebugBranches()
    {
        // 全リセット
        for (int i = 0; i < clearedStages.Length; i++)
        {
            clearedStages[i] = false;
            selectedByBranch[i] = false;
        }

        // 開始地点の解放
        if (IsValid(startStageId))
            clearedStages[startStageId] = true;

        // 連鎖的な依存関係の解決
        bool changed;
        do
        {
            changed = false;

            foreach (var b in debugBranches)
            {
                // 親の条件をチェック
                bool parentOK = true;
                foreach (int p in b.parentStages)
                {
                    if (!IsValid(p) || !clearedStages[p])
                    {
                        parentOK = false;
                        break;
                    }
                }

                // 親が非成立なら選択をリセット
                if (!parentOK)
                {
                    if (b.selectedStage != -1)
                    {
                        b.selectedStage = -1;
                        changed = true;
                    }
                    continue;
                }

                // 選択されているステージを解放状態にする
                if (b.selectedStage != -1 && IsValid(b.selectedStage))
                {
                    if (!clearedStages[b.selectedStage])
                    {
                        clearedStages[b.selectedStage] = true;
                        selectedByBranch[b.selectedStage] = true; // 分岐通過フラグを立てる
                        changed = true;
                    }
                }
            }
        }
        while (changed); // 変化がなくなるまでループ
    }


#if UNITY_EDITOR
    // エディタ上で値が変更されたときに即座に表示を反映する関数
    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        if (stages == null || clearedStages == null) return;

        ApplyDebugBranches(); // デバッグ設定を反映
        ApplyAllRules(); // ルールを適用
        Refresh(); // 表示を更新
        SaveClearedStages(); // 状況を保存
    }
#endif
}
