using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor; // SceneAsset
#endif
using System.Collections.Generic;

public class StageNode : MonoBehaviour
{
    [Header("ステージ設定")]
    public int stageId;

#if UNITY_EDITOR
    public SceneAsset sceneAsset; // ここにシーンをD&D
#endif

    private string sceneName; // 実行用のシーン名

    public List<int> nextStageIds = new List<int>();
    public List<GameObject> connectedPaths = new List<GameObject>();

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
    }

    private void Start()
    {
        RefreshSceneName();
    }

    public void RefreshSceneName()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
            sceneName = sceneAsset.name;
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

#if UNITY_EDITOR
        // 編集中にアセットが変更されたら名前も更新しておく
        if (sceneAsset != null && sceneName != sceneAsset.name)
            sceneName = sceneAsset.name;
#endif
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
