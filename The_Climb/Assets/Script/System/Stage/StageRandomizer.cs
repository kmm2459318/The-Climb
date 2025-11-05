using UnityEngine;
using UnityEngine.SceneManagement;

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
        for (int i = 0; i < StageName.Length; i++)
        {
            int randomIndex = Random.Range(i, StageName.Length);
            (StageName[i], StageName[randomIndex]) = (StageName[randomIndex], StageName[i]);
        }

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
        SceneManager.LoadScene(StageName[ButtonNo]);
        PlayerPrefs.SetInt(StageName[ButtonNo], 1);
    }
}
