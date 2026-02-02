using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StageRandomizer : MonoBehaviour
{
    public string[] StageName;
    public string[] BossStageName;

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

        // 新しい配列（サイズ8）を作成
        string[] newStages = new string[8];
        
        // 埋めるべきインデックス（3番目と7番目を除く）
        int[] targetIndices = { 0, 1, 2, 4, 5, 6 };
        
        // ソースから順に埋める
        for (int i = 0; i < targetIndices.Length; i++)
        {
            if (sourceList.Count > 0)
            {
                int sourceIndex = i % sourceList.Count;
                newStages[targetIndices[i]] = sourceList[sourceIndex];
            }
            else
            {
                Debug.LogError("ステージリストが空です！");
                return;
            }
        }

        // 3番目（インデックス3）に、StageNameの中からランダムに1つ選んで設定（重複あり）
        if (sourceList.Count > 0)
        {
            string randomStage = sourceList[Random.Range(0, sourceList.Count)];
            newStages[3] = randomStage;
            Debug.Log($"3番目（インデックス3）を {randomStage} に設定しました");
        }

        // 7番目（インデックス7）に、BossStageNameの中からランダムに1つ選んで設定
        if (BossStageName != null && BossStageName.Length > 0)
        {
            string bossStage = BossStageName[Random.Range(0, BossStageName.Length)];
            newStages[7] = bossStage;
            Debug.Log($"7番目（インデックス7）をBossステージ {bossStage} に設定しました");
        }
        else
        {
            Debug.LogWarning("BossStageNameが設定されていません！");
        }

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
            if (loadedOrder.Length == 8)
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
        int stageIndex = ButtonNo - 1;

        // ★ 進行用StageIdは「1始まり」
        PlayerPrefs.SetInt("CurrentStageId", stageIndex + 1);
        PlayerPrefs.Save();

        if (stageIndex >= 0 && stageIndex < StageName.Length)
        {
            System.Loading.SceneLoader.Instance.LoadScene(StageName[stageIndex]);
        }
    }
}
