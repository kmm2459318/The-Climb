using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StagePoolData
{
    [Header("Stage")]
    public string sceneName;
    public string stageName;

    [Header("Images")]
    public Sprite loadingImage;
    public Sprite stagePreviewSprite;
}

public class StageRandomizer : MonoBehaviour
{
    [Header("ステージ設定")]
    public List<StagePoolData> NormalStagePool = new List<StagePoolData>();
    public List<StagePoolData> BossStagePool = new List<StagePoolData>();

    [Header("結果（他スクリプト参照用）")]
    public string[] SceneName = new string[8];
    public string[] StageName = new string[8];
    public Sprite[] LoadingImage = new Sprite[8];

    private int[] _currentSlotIndices = new int[8];

    private void Start()
    {
        if (PlayerPrefs.GetInt("GameStart") == 1 || !PlayerPrefs.HasKey("StageIndexOrder"))
        {
            Shuffle();

            for (int i = 0; i <= 20; i++)
                PlayerPrefs.SetInt($"StageCleared_{i}", 0);

            PlayerPrefs.DeleteKey("JustClearedStageId");
            PlayerPrefs.SetInt("GameStart", 0);
            PlayerPrefs.Save();
        }
        else
        {
            Load();
        }
    }

    private void Shuffle()
    {
        // ランダム化用のインデックスリストを作成
        List<int> pool = new List<int>();
        for (int i = 0; i < NormalStagePool.Count; i++)
            pool.Add(i); // 通常ステージのインデックスを追加

        // フィッシャー・イェーツのシャッフル
        for (int i = 0; i < pool.Count; i++)
        {
            int r = Random.Range(i, pool.Count);
            (pool[i], pool[r]) = (pool[r], pool[i]); // 要素を入れ替え
        }

        int[] result = new int[8];

        // スロット0〜6 (List 1〜7) に重複なしで割り当て
        // 以前はスロット3を別途ランダムにしていたが、一括で順番に割り当てるように変更
        int[] normalSlots = { 0, 1, 2, 3, 4, 5, 6 };
        for (int i = 0; i < normalSlots.Length && i < pool.Count; i++)
            result[normalSlots[i]] = pool[i]; // シャッフルされたインデックスをスロットに設定

        // ボスステージの設定 (スロット7)
        if (BossStagePool.Count > 0)
            result[7] = Random.Range(0, BossStagePool.Count) + 1000; // ボスは1000以上の値で管理

        _currentSlotIndices = result; // 結果を保存
        Apply(); // 各配列にデータを適用
        PlayerPrefs.SetString("StageIndexOrder", string.Join(",", _currentSlotIndices)); // 保存
    }

    private void Load()
    {
        string[] s = PlayerPrefs.GetString("StageIndexOrder").Split(',');
        for (int i = 0; i < 8; i++)
            _currentSlotIndices[i] = int.Parse(s[i]);

        Apply();
    }

    private void Apply()
    {
        for (int i = 0; i < 8; i++)
        {
            int idx = _currentSlotIndices[i];

            if (idx >= 1000)
            {
                var d = BossStagePool[idx - 1000];
                SceneName[i] = d.sceneName;
                StageName[i] = d.stageName;
                LoadingImage[i] = d.loadingImage;
            }
            else
            {
                var d = NormalStagePool[idx];
                SceneName[i] = d.sceneName;
                StageName[i] = d.stageName;
                LoadingImage[i] = d.loadingImage;
            }
        }
    }

    public Sprite GetStagePreviewSprite(int stageId)
    {
        int slot = stageId - 1;
        if (slot < 0 || slot >= _currentSlotIndices.Length)
            return null;

        int idx = _currentSlotIndices[slot];

        if (idx >= 1000)
        {
            int b = idx - 1000;
            return (b >= 0 && b < BossStagePool.Count)
                ? BossStagePool[b].stagePreviewSprite
                : null;
        }
        else
        {
            return (idx >= 0 && idx < NormalStagePool.Count)
                ? NormalStagePool[idx].stagePreviewSprite
                : null;
        }
    }

    public void StartStage(int id)
    {
        PlayerPrefs.SetInt("CurrentStageId", id);
        PlayerPrefs.Save();

        System.Loading.SceneLoader.Instance.LoadScene(
            SceneName[id - 1],
            LoadingImage[id - 1]
        );
    }
}