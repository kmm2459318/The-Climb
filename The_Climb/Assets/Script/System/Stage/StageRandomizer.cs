using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[System.Serializable]
public class StagePoolData
{
    public string sceneName;
    public string stageName;
    public Sprite loadingImage;
}

public class StageRandomizer : MonoBehaviour
{
    // --- インスペクター設定用 (リスト形式でグループ化) ---
    [Header("ステージ設定 (インスペクターで設定)")]
    public List<StagePoolData> NormalStagePool = new List<StagePoolData>();
    public Sprite[] StageSprites;
    public Texture[] StageTextures;

    [Header("ボス設定")]
    public List<StagePoolData> BossStagePool = new List<StagePoolData>();


    // --- 実行時の結果 (他のスクリプトが参照する) ---
    [Header("シャッフル結果 (他のスクリプトが参照)")]
    public string[] SceneName = new string[8];
    public string[] StageName = new string[8];
    public Sprite[] LoadingImage = new Sprite[8];

    // 各スロットがプールの何番目か (型判定用に1000+はボス)
    private int[] _currentSlotIndices = new int[8];


    private void Start()
    {
        // 配列の初期化 (8スロット固定)
        SceneName = new string[8];
        StageName = new string[8];
        LoadingImage = new Sprite[8];

        // ゲーム開始フラグがあるか、保存された並び順がない場合にシャッフルを実行
        if (PlayerPrefs.GetInt("GameStart") == 1 || !PlayerPrefs.HasKey("StageIndexOrder"))
        {
            Shuffle();
            
            // 全フラグリセット (0〜20)
            for (int i = 0; i <= 20; i++) PlayerPrefs.SetInt($"StageCleared_{i}", 0);
            
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
        List<int> poolIndices = new List<int>();
        for (int i = 0; i < NormalStagePool.Count; i++) poolIndices.Add(i);

        // シャッフル
        for (int i = 0; i < poolIndices.Count; i++)
        {
            int r = Random.Range(i, poolIndices.Count);
            (poolIndices[i], poolIndices[r]) = (poolIndices[r], poolIndices[i]);
        }

        int[] result = new int[8];

        // 規則1: 0, 1, 2, 4, 5, 6 (通常)
        int[] normalSlots = { 0, 1, 2, 4, 5, 6 };
        for (int i = 0; i < normalSlots.Length; i++)
        {
            if (i < poolIndices.Count) result[normalSlots[i]] = poolIndices[i];
        }

        // 規則2: 3 (通常プールから重複許可)
        if (NormalStagePool.Count > 0) result[3] = Random.Range(0, NormalStagePool.Count);

        // 規則3: 7 (ボスプール)
        if (BossStagePool.Count > 0)
        {
            result[7] = Random.Range(0, BossStagePool.Count) + 1000;
        }

        _currentSlotIndices = result;
        ApplyIndicesToResult();
        
        PlayerPrefs.SetString("StageIndexOrder", string.Join(",", _currentSlotIndices));
        PlayerPrefs.Save();
    }

    private void Load()
    {
        if (PlayerPrefs.HasKey("StageIndexOrder"))
        {
            string[] strings = PlayerPrefs.GetString("StageIndexOrder").Split(',');
            if (strings.Length == 8)
            {
                for (int i = 0; i < 8; i++) _currentSlotIndices[i] = int.Parse(strings[i]);
                ApplyIndicesToResult();
            }
        }
    }

    private void ApplyIndicesToResult()
    {
        for (int i = 0; i < 8; i++)
        {
            int idx = _currentSlotIndices[i];
            
            if (idx >= 1000) // ボス
            {
                int bIdx = idx - 1000;
                if (bIdx < BossStagePool.Count)
                {
                    SceneName[i] = BossStagePool[bIdx].sceneName;
                    StageName[i] = BossStagePool[bIdx].stageName;
                    LoadingImage[i] = BossStagePool[bIdx].loadingImage;
                }
            }
            else // 通常
            {
                if (idx < NormalStagePool.Count)
                {
                    SceneName[i] = NormalStagePool[idx].sceneName;
                    StageName[i] = NormalStagePool[idx].stageName;
                    LoadingImage[i] = NormalStagePool[idx].loadingImage;
                }
            }
        }
    }

    // 他のスクリプトからは 1始まりのステージID (1〜8) が渡される
    public void StartStage(int id)
    {
        int arrayIdx = id - 1; // 内部スロットに変換

        if (arrayIdx >= 0 && arrayIdx < 8)
        {
            // StageSelectManager用 (1始まり)
            PlayerPrefs.SetInt("JustClearedStageId", id);
            // 他システム用 (1始まり)
            PlayerPrefs.SetInt("CurrentStageId", id);
            PlayerPrefs.Save();

            string scene = SceneName[arrayIdx];
            Sprite img = LoadingImage[arrayIdx];

            if (string.IsNullOrEmpty(scene))
            {
                Debug.LogError($"[StageRandomizer] ステージ {id} のシーン名が空です。インスペクターで NormalStagePool / BossStagePool を設定してください。");
                return;
            }
            
            Debug.Log($"Loading Stage {id}: Scene={scene}, Image={(img ? img.name : "null")}");
            System.Loading.SceneLoader.Instance.LoadScene(scene, img);
        }
    }
}
