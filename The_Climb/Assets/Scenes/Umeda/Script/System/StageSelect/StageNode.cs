using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class StageNode : MonoBehaviour
{
    // ==============================
    // ステージ設定
    // ==============================
    [Header("ステージ設定")]
    public int stageId;

#if UNITY_EDITOR
    public SceneAsset sceneAsset;   // Editor用
#endif

    [SerializeField, HideInInspector]
    private string sceneName;       // Runtime用

    public List<int> nextStageIds = new List<int>();

    // ==============================
    // 見た目（Sphere）
    // ==============================
    public enum NodeState
    {
        Locked,
        Available,
        Cleared
    }

    [Header("見た目（Sphere）")]
    public Renderer nodeRenderer;   // 子SphereのRenderer
    public Material lockedMat;      // 黒
    public Material availableMat;   // 銀
    public Material clearedMat;     // 金

    [SerializeField]
    private NodeState currentState = NodeState.Locked;

    // ==============================
    // UI
    // ==============================
    [Header("UI設定")]
    public GameObject promptUI;
    public Vector3 uiOffset = new Vector3(0, 2, 0);

    // ==============================
    // 状態管理
    // ==============================
    [HideInInspector] public bool isUnlocked = false;
    private bool playerNearby = false;

    // ==============================
    // 外部参照
    // ==============================
    public StageRandomizer stageRandomizer;
    public StageRoute stageRoute;

    // ==============================
    // Unity Lifecycle
    // ==============================
    private void Awake()
    {
        // Renderer自動取得
        if (nodeRenderer == null)
            nodeRenderer = GetComponentInChildren<Renderer>();

        if (nodeRenderer == null)
            Debug.LogError($"[StageNode] Rendererが見つかりません: {name}");

        if (promptUI != null)
            promptUI.SetActive(false);

        ApplyState(NodeState.Locked);
    }

    private void Start()
    {
        RefreshSceneName();
    }

    private void Update()
    {
        if (playerNearby && isUnlocked && Input.GetKeyDown(KeyCode.Space))
        {
            stageRandomizer?.StartStage(stageId);
            stageRoute?.OnStageButtonPressed(stageId);
        }

        if (promptUI != null)
            promptUI.transform.position = transform.position + uiOffset;
    }

    // ==============================
    // 見た目・状態制御
    // ==============================
    public void ApplyState(NodeState state)
    {
        currentState = state;

        switch (state)
        {
            case NodeState.Locked:
                isUnlocked = false;
                SetMaterial(lockedMat);
                break;

            case NodeState.Available:
                isUnlocked = true;
                SetMaterial(availableMat);
                break;

            case NodeState.Cleared:
                isUnlocked = true;
                SetMaterial(clearedMat);
                break;
        }
    }

    private void SetMaterial(Material mat)
    {
        if (nodeRenderer == null || mat == null) return;

        // material を使うことでインスタンス化される
        nodeRenderer.material = mat;
    }

    // 外部から呼びやすいAPI
    public void SetLocked() => ApplyState(NodeState.Locked);
    public void SetAvailable() => ApplyState(NodeState.Available);
    public void SetCleared() => ApplyState(NodeState.Cleared);

    // ==============================
    // Trigger
    // ==============================
    private void OnTriggerEnter(Collider other)
    {
        if (!isUnlocked) return;

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

    // ==============================
    // Scene 名管理
    // ==============================
#if UNITY_EDITOR
    public void RefreshSceneName()
    {
        if (sceneAsset != null)
            sceneName = sceneAsset.name;
    }
#endif

    // Runtime用（ビルド後）
    public string GetSceneName()
    {
        return sceneName;
    }
}
