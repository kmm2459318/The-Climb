using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    // ★ 画像表示機能（追加）
    // ===============================
    [Header("Stage Image")]
    public Image stageImage;          // Sprite用
    public RawImage stageRawImage;    // Texture用

    public Sprite unknownSprite;      // ロック中用（任意）
    public Texture unknownTexture;    // ロック中用（任意）

    // ===============================
    // Unity Lifecycle
    // ===============================
    private void Awake()
    {
        if (nodeRenderer == null)
            nodeRenderer = GetComponentInChildren<Renderer>(true);

        // 初期状態はロック
        SetLocked();

        if (promptUI)
            promptUI.SetActive(false);

        HideStageImage();
    }

    private void Update()
    {
        // UI追従
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
    // 見た目制御
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
            else if (stageRandomizer && stageId - 1 < stageRandomizer.StageName.Length)
            {
                stageNameText.text = stageRandomizer.StageName[stageId - 1];
            }
        }

        ShowStageImage();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;

        if (promptUI)
            promptUI.SetActive(false);

        if (stageNameText)
            stageNameText.text = "";

        HideStageImage();
    }

    // ===============================
    // ★ 画像制御（追加）
    // ===============================
    private void ShowStageImage()
    {
        if (!isInteractable)
        {
            // ロック中
            if (stageImage && unknownSprite)
            {
                stageImage.sprite = unknownSprite;
                stageImage.gameObject.SetActive(true);
            }

            if (stageRawImage && unknownTexture)
            {
                stageRawImage.texture = unknownTexture;
                stageRawImage.gameObject.SetActive(true);
            }
            return;
        }

        // 解放済み
        if (stageRandomizer == null) return;

        if (stageImage && stageId - 1 < stageRandomizer.StageSprites.Length)
        {
            stageImage.sprite = stageRandomizer.StageSprites[stageId - 1];
            stageImage.gameObject.SetActive(true);
        }

        if (stageRawImage && stageId - 1 < stageRandomizer.StageTextures.Length)
        {
            stageRawImage.texture = stageRandomizer.StageTextures[stageId - 1];
            stageRawImage.gameObject.SetActive(true);
        }
    }

    private void HideStageImage()
    {
        if (stageImage)
            stageImage.gameObject.SetActive(false);

        if (stageRawImage)
            stageRawImage.gameObject.SetActive(false);
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
