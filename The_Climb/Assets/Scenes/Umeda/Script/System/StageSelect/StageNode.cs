using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageNode : MonoBehaviour
{
    [Header("ステージ設定")]
    public int stageId;

#if UNITY_EDITOR
    [Header("Editor専用")]
    public SceneAsset sceneAsset;
#endif

    [Header("見た目")]
    public Renderer nodeRenderer;
    public Material lockedMat;
    public Material availableMat;
    public Material clearedMat;

    [Header("UI")]
    public TextMeshProUGUI stageNameText;
    public string unknownText = "???";
    public GameObject promptUI;
    public Vector3 uiOffset = new Vector3(0, 2, 0);

    // ★ StageSelectManager から直接触られる
    [Header("State")]
    public bool isInteractable = false;

    private bool playerNearby;

    [Header("外部参照")]
    public StageSelectManager stageSelectManager;
    public StageRandomizer stageRandomizer;

    // ===============================
    // Unity Lifecycle
    // ===============================
    private void Awake()
    {
        if (nodeRenderer == null)
            nodeRenderer = GetComponentInChildren<Renderer>(true);

        // 初期状態はロック（非表示にはしない）
        SetLocked();

        if (promptUI)
            promptUI.SetActive(false);
    }

    private void Update()
    {
        // UI追従だけは常に行う
        if (promptUI)
            promptUI.transform.position = transform.position + uiOffset;

        if (!isInteractable) return;
        if (!playerNearby) return;
        if (stageSelectManager == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stageSelectManager.OnStageSelected(stageId);
        }
    }

    // ===============================
    // 見た目制御（★重要）
    // ===============================
    public void SetLocked()
    {
        isInteractable = false;

        if (nodeRenderer && lockedMat)
            nodeRenderer.material = lockedMat;
    }

    public void SetAvailable()
    {
        Debug.Log($"{name} SetAvailable");

        isInteractable = true;

        if (nodeRenderer && availableMat)
            nodeRenderer.material = availableMat;
    }

    public void SetCleared()
    {
        // クリア済みは「押せない」
        isInteractable = false;

        if (nodeRenderer && clearedMat)
            nodeRenderer.material = clearedMat;
    }

    // ===============================
    // Trigger
    // ===============================
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = true;

        if (promptUI)
            promptUI.SetActive(true);

        if (stageNameText)
        {
            if (!isInteractable)
            {
                stageNameText.text = unknownText;
            }
            else if (stageRandomizer && stageId < stageRandomizer.StageName.Length)
            {
                stageNameText.text = stageRandomizer.StageName[stageId];
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;

        if (promptUI)
            promptUI.SetActive(false);

        if (stageNameText)
            stageNameText.text = "";
    }

#if UNITY_EDITOR
    // ===============================
    // Editor補助
    // ===============================
    public void RefreshSceneName()
    {
        if (sceneAsset != null)
        {
            string _ = sceneAsset.name;
        }
    }
#endif
}
