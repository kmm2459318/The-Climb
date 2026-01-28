using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class StageNameDisplay : MonoBehaviour
{
    [System.Serializable]
    public struct StageMapping
    {
        public string sceneName;   // シーン名（StageRandomizerのStageNameと一致させる）
        public string displayName; // UIに表示する名前
    }

    [Header("参照")]
    [SerializeField] private StageRandomizer stageRandomizer;
    [SerializeField] private List<TextMeshProUGUI> stageNameTexts; // ボタンのテキストコンポーネント（8個）

    [Header("設定")]
    [SerializeField] private List<StageMapping> stageMappings; // シーン名と表示名のマッピングリスト

    void Start()
    {
        if (stageRandomizer == null)
        {
            Debug.LogError("StageRandomizerが設定されていません！");
            return;
        }

        // StageRandomizerのStartが完了してStageNameが確定していることを期待
        // もしタイミングが合わない場合は、StageRandomizer側でイベントを発行するか、ここを遅延させる必要がある
        UpdateStageNames();
    }

    public void UpdateStageNames()
    {
        string[] currentStages = stageRandomizer.StageName;

        if (currentStages == null || currentStages.Length == 0)
        {
            Debug.LogWarning("StageRandomizerのステージリストが空です。");
            return;
        }

        for (int i = 0; i < stageNameTexts.Count; i++)
        {
            if (i >= currentStages.Length) break;

            if (stageNameTexts[i] != null)
            {
                string sceneName = currentStages[i];
                string displayName = GetDisplayName(sceneName);
                stageNameTexts[i].text = displayName;
            }
        }
    }

    private string GetDisplayName(string sceneName)
    {
        foreach (var mapping in stageMappings)
        {
            if (mapping.sceneName == sceneName)
            {
                return mapping.displayName;
            }
        }
        
        // マッピングが見つからない場合はシーン名をそのまま表示（または警告）
        return sceneName;
    }
}
