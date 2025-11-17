using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomSceneAssigner : MonoBehaviour
{
    [Header("シーンリスト（ScriptableObject）")]
    public SceneLibrary sceneLibrary;

    [Header("割り当て対象の StageNode")]
    public List<StageNode> stageNodes = new List<StageNode>();

    [Header("ランダム種（固定すると毎回同じ順）")]
    public int randomSeed = 0;
    public bool useFixedSeed = false;

#if UNITY_EDITOR
    [ContextMenu("Assign Random Scenes")]
    public void AssignRandomScenes()
    {
        if (sceneLibrary == null || sceneLibrary.sceneAssets.Count == 0)
        {
            Debug.LogError("SceneLibrary が空です！");
            return;
        }

        if (stageNodes.Count == 0)
            stageNodes.AddRange(FindObjectsOfType<StageNode>(true));

        List<SceneAsset> pool = new List<SceneAsset>(sceneLibrary.sceneAssets);

        if (useFixedSeed)
            Random.InitState(randomSeed);

        // シャッフル
        for (int i = 0; i < pool.Count; i++)
        {
            int rand = Random.Range(i, pool.Count);
            (pool[i], pool[rand]) = (pool[rand], pool[i]);
        }

        // 割り当て
        for (int i = 0; i < stageNodes.Count; i++)
        {
            if (i < pool.Count)
            {
                StageNode node = stageNodes[i];
                node.sceneAsset = pool[i];
                node.RefreshSceneName();
                EditorUtility.SetDirty(node);
            }
        }

        Debug.Log("ランダムでシーンを割り当てました！");
    }
#endif
}
