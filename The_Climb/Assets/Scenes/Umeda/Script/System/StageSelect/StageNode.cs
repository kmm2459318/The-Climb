using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor; // SceneAssetを使うため
#endif
using TMPro;
using System.Collections.Generic;

public class StageNode : MonoBehaviour
{
    [Header("ステージ設定")]
    public int stageId;
#if UNITY_EDITOR
    public SceneAsset sceneAsset; // インスペクターでシーンを参照
#endif
    private string sceneName;      // 実行時にSceneManager.LoadScene用に変換
    public List<int> nextStageIds;
    public List<GameObject> connectedPaths;

    [Header("UI設定")]
    public GameObject promptUI;
    public Vector3 uiOffset = new Vector3(0, 2, 0);

    [HideInInspector] public bool isUnlocked = false;
    private bool playerNearby = false;

    private void Awake()
    {
        foreach (var path in connectedPaths)
            if (path != null) path.SetActive(false);

        if (promptUI != null)
            promptUI.SetActive(false);

#if UNITY_EDITOR
        if (sceneAsset != null)
            sceneName = sceneAsset.name; // SceneManager用に変換
#endif
    }

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.Space) && !string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }

        if (promptUI != null)
            promptUI.transform.position = transform.position + uiOffset;
    }

    public void Unlock()
    {
        if (isUnlocked) return;
        isUnlocked = true;

        foreach (var path in connectedPaths)
            if (path != null) path.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (promptUI != null)
                promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }

    // Runtime用（SceneName を直接セット）
    public void SetSceneName_Runtime(string name)
    {
        this.GetType(); // 空の安全策（消してもOK）
        typeof(StageNode).ToString(); // これも消してOK
                                      // 実行時に sceneName をセット
        var sceneNameField = typeof(StageNode).GetField("sceneName",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);
        if (sceneNameField != null)
            sceneNameField.SetValue(this, name);
    }
}
