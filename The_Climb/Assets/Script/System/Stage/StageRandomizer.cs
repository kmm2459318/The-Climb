using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StageRandomizer : MonoBehaviour
{
    public string[] StageName;

    void Start()
    {
        // タイトルからゲームスタートを押したときのみシャッフル
        if (PlayerPrefs.GetInt("GameStart") == 1)
        {
            ShuffleStage();
            SaveStageOrder(); // 順番を保存
            PlayerPrefs.SetInt("GameStart", 0);
        }
        else
        {
            // 保存されている順番があればそれを読み込む
            LoadStageOrder();
        }
    }

    // ステージをランダムに並び替える
    private void ShuffleStage()
    {
        // 元のリストをコピーしてシャッフル用リスト作成
        List<string> sourceList = new List<string>(StageName);
        
        // ソースリストをシャッフル
        for (int i = 0; i < sourceList.Count; i++)
        {
            int randomIndex = Random.Range(i, sourceList.Count);
            (sourceList[i], sourceList[randomIndex]) = (sourceList[randomIndex], sourceList[i]);
        }

        // 新しい配列（サイズ7）を作成
        string[] newStages = new string[7];
        
        // 埋めるべきインデックス（4番目=インデックス3を除く）
        int[] targetIndices = { 0, 1, 2, 4, 5, 6 };
        
        // ソースから順に埋める（ソースが足りない場合はループ）
        for (int i = 0; i < targetIndices.Length; i++)
        {
            if (sourceList.Count > 0)
            {
                // ソースリストの要素数より多く要求された場合は剰余で対応（あるいはランダム）
                // ここでは単純にシャッフルされたソースリストから順番に取る
                int sourceIndex = i % sourceList.Count;
                newStages[targetIndices[i]] = sourceList[sourceIndex];
            }
            else
            {
                Debug.LogError("ステージリストが空です！");
                return;
            }
        }

        // 4番目（インデックス3）に、選ばれた6つの中からランダムに1つ選んでコピー
        int randomPickIndex = Random.Range(0, targetIndices.Length);
        string duplicateStage = newStages[targetIndices[randomPickIndex]];
        newStages[3] = duplicateStage;
        
        Debug.Log($"4番目（インデックス3）を {duplicateStage} (インデックス{targetIndices[randomPickIndex]}のコピー) に設定しました");

        // 結果を反映
        StageName = newStages;

        for (int i = 0; i < StageName.Length; i++)
        {
            Debug.Log($"ステージ{i}番目は{StageName[i]}に決まりました");
        }
    }

    // ステージ順を保存（PlayerPrefsにカンマ区切りで保存）
    private void SaveStageOrder()
    {
        string joined = string.Join(",", StageName);
        PlayerPrefs.SetString("StageOrder", joined);
        PlayerPrefs.Save();
        Debug.Log($"ステージ順を保存しました: {joined}");
    }

    // ステージ順を読み込み
    private void LoadStageOrder()
    {
        if (PlayerPrefs.HasKey("StageOrder"))
        {
            string saved = PlayerPrefs.GetString("StageOrder");
            string[] loadedOrder = saved.Split(',');

            // 保存データと現在のStageNameの数が一致しているかチェック
            if (loadedOrder.Length == StageName.Length)
            {
                StageName = loadedOrder;
                Debug.Log($"ステージ順を読み込みました: {saved}");
            }
            else
            {
                Debug.LogWarning("ステージ数が変更されているため、保存データを無視します。");
            }
        }
    }

    // 指定されたボタン番号のステージをロード
    public void StartStage(int ButtonNo)
    {
        System.Loading.SceneLoader.Instance.LoadScene(StageName[ButtonNo]);

    }
}
