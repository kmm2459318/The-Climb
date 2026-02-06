using UnityEngine;
using System.Collections.Generic;

public class StageRandomizer : MonoBehaviour
{
    public string[] StageName; // ステージ名の配列
    public string[] BossStageName; // ボスステージ名の配列

    private string[] _stagePool; // 通常ステージのプールを保持する変数
    private string[] _bossPool; // ボスステージのプールを保持する変数

    // 初期化時に実行される関数
    private void Start()
    {
        // インスペクターで設定された初期値をプールとして保存
        _stagePool = (string[])StageName.Clone(); // StageNameの初期状態をコピー
        _bossPool = (string[])BossStageName.Clone(); // BossStageNameの初期状態をコピー

        // ゲーム開始時（タイトルからの遷移など）のみシャッフルを実行
        if (PlayerPrefs.GetInt("GameStart") == 1)
        {
            Shuffle(); // ランダムに並び替える関数の呼び出し
            Save(); // 結果を保存する関数の呼び出し

            for (int i = 1; i < 8; i++)
            {
                PlayerPrefs.SetInt("JustClearedStageId", 0);
            }


            PlayerPrefs.SetInt("GameStart", 0); // フラグをリセット
        }
        else
        {
            Load(); // 保存されたデータを読み込む関数の呼び出し
        }
    }

    // ステージを特定の規則に従って並び替える関数
    private void Shuffle()
    {
        // 全8ステージ（0～7）の結果を格納する配列を用意
        string[] result = new string[8]; // 8要素の配列を作成

        // 通常ステージ（0,1,2,4,5,6用）をシャッフルして重複を避ける準備
        List<string> tempNormal = new List<string>(_stagePool); // プールをリストに変換
        for (int i = 0; i < tempNormal.Count; i++)
        {
            int r = Random.Range(i, tempNormal.Count);
            (tempNormal[i], tempNormal[r]) = (tempNormal[r], tempNormal[i]); // 要素をランダムに入れ替え
        }

        // 規則1: インデックス 0, 1, 2, 4, 5, 6 に重複なしで割り当て
        int[] normalIndices = { 0, 1, 2, 4, 5, 6 }; // 割り当て対象のインデックス
        for (int i = 0; i < normalIndices.Length; i++)
        {
            if (i < tempNormal.Count)
            {
                result[normalIndices[i]] = tempNormal[i]; // シャッフル順に従って代入
            }
        }

        // 規則2: インデックス 3 は通常ステージプールからランダムに1つ選ぶ（重複を許容）
        if (_stagePool.Length > 0)
        {
            result[3] = _stagePool[Random.Range(0, _stagePool.Length)]; // プールから1つ選択して代入
        }

        // 規則3: インデックス 7 はボスステージプールからランダムに1つ選ぶ
        if (_bossPool != null && _bossPool.Length > 0)
        {
            result[7] = _bossPool[Random.Range(0, _bossPool.Length)]; // ボスプールから1つ選択して代入
        }

        // 最終的な結果をメインの配列に反映
        StageName = result; // 配列を上書き

        // デバッグ用にログを出力
        for (int i = 0; i < StageName.Length; i++)
        {
            Debug.Log($"ステージ{i}番目は {StageName[i]} に決定しました"); // 決定したステージ名を表示
        }
    }

    // 現在のステージ順を保存する関数
    private void Save()
    {
        string joined = string.Join(",", StageName); // 配列をカンマ区切りの文字列に変換
        PlayerPrefs.SetString("StageOrder", joined); // 文字列を保存
        PlayerPrefs.Save(); // 変更を確定
    }

    // 保存されたステージ順を読み込む関数
    private void Load()
    {
        if (PlayerPrefs.HasKey("StageOrder"))
        {
            string saved = PlayerPrefs.GetString("StageOrder"); // 文字列を読み込み
            string[] loadedOrder = saved.Split(','); // カンマで分割して配列に戻す

            // 配列のサイズが期待通りであれば適用
            if (loadedOrder.Length == 8)
            {
                StageName = loadedOrder; // 読み込んだ配列を適用
            }
        }
    }

    // 指定されたインデックス（ボタン番号）に対応するステージを開始する関数
    public void StartStage(int ButtonNo)
    {
        // 引数のButtonNoが1始まりであると想定
        int stageIndex = ButtonNo - 1; // 0始まりのインデックスに変換

        // 指定されたインデックスが範囲内かチェック
        if (stageIndex >= 0 && stageIndex < StageName.Length)
        {
            // 現在のステージIDを1始まりで保存（進行管理用）
            PlayerPrefs.SetInt("CurrentStageId", stageIndex + 1); // 1から始まるIDを設定

            // ★要望：ステージに入った時、対応する番号+1の要素を解放するためにJustClearedStageIdをセット
            // これにより StageSelectManager 側で次のステージが解放される仕組み
            PlayerPrefs.SetInt("JustClearedStageId", stageIndex + 1); // 次のステージ番号をセット

            PlayerPrefs.Save(); // 変更を確定

            // 指定されたシーンをロード
            System.Loading.SceneLoader.Instance.LoadScene(StageName[stageIndex]); // シーン遷移を実行
        }
        else
        {
            Debug.LogError($"StageRandomizer: stageIndex={stageIndex} は範囲外です"); // エラーログを出力
        }
    }
}
