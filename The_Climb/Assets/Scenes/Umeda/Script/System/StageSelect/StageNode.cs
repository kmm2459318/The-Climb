using UnityEngine;
using TMPro;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageNode : MonoBehaviour
{
    [Header("ステージ設定")]
    public int stageId;   // 0始まり（StageRandomizer.StageNameと対応）

#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif

    [Header("見た目（Sphere）")]
    public Renderer nodeRenderer;
    public Material lockedMat;       // 黒
    public Material availableMat;    // 銀
    public Material clearedMat;      // 金

    [Header("Canvas内 共通ステージ名テキスト")]
    public TextMeshProUGUI sharedStageNameText;
    public string unknownText = "???";

    [Header("UI設定")]
    public GameObject promptUI;
    public Vector3 uiOffset = new Vector3(0, 2, 0);

    [HideInInspector] public bool isUnlocked = false;
    private bool playerNearby = false;

    [Header("外部参照")]
    public StageRandomizer stageRandomizer;
    public StageRoute stageRoute;
    public StageSelectManager stageSelectManager;

    // ===============================
    // Unity Lifecycle
    // ===============================
    private void Awake()
    {
        if (nodeRenderer == null)
            nodeRenderer = GetComponentInChildren<Renderer>();

        if (promptUI != null)
            promptUI.SetActive(false);

        SetLocked();
    }

    private void Update()
    {
        if (playerNearby && isUnlocked && Input.GetKeyDown(KeyCode.Space))
        {
            // ★ ステージに入った瞬間にクリア扱い
            PlayerPrefs.SetInt("LastClearedStage", stageId);
            PlayerPrefs.Save();

            stageSelectManager.OnStageSelected(stageId);
        }

        if (promptUI != null)
            promptUI.transform.position = transform.position + uiOffset;
    }

    // ===============================
    // 見た目制御
    // ===============================
    public void SetLocked()
    {
        isUnlocked = false;

        if (nodeRenderer != null && lockedMat != null)
            nodeRenderer.material = lockedMat;
    }

    public void SetAvailable()
    {
        isUnlocked = true;

        if (nodeRenderer != null && availableMat != null)
            nodeRenderer.material = availableMat;
    }

    public void SetCleared()
    {
        isUnlocked = true;

        if (nodeRenderer != null && clearedMat != null)
            nodeRenderer.material = clearedMat;
    }

    // ===============================
    // ステージ名表示制御
    // ===============================
    private void UpdateStageNameText()
    {
        if (sharedStageNameText == null)
            return;

        if (!isUnlocked)
        {
            sharedStageNameText.text = unknownText;
            return;
        }

        if (stageRandomizer == null)
        {
            sharedStageNameText.text = "No Randomizer";
            return;
        }

        if (stageId >= 0 && stageId < stageRandomizer.StageName.Length)
        {
            string internalName = stageRandomizer.StageName[stageId];
            sharedStageNameText.text = GetDisplayStageName(internalName);
        }
        else
        {
            sharedStageNameText.text = "Invalid Stage";
        }
    }

    // ===============================
    // 内部名 → 表示名変換
    // ===============================
    private string GetDisplayStageName(string internalName)
    {
        switch (internalName)
        {
            case "Umeda":
                return "光が示す道";
            case "Kitano":
                return "ライトシフター";
            case "Matsuyama":
                return "アストラル";
            case "Yuoka":
                return "爆発ウーマン";
            case "Nakamura":
                return "陰陽相棒ステージ";
            case "Nisiyama":
                return "ターンオーバー";
            default:
                return internalName;
        }
    }

    // ===============================
    // Trigger
    // ===============================
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = true;

        if (promptUI != null)
            promptUI.SetActive(true);

        UpdateStageNameText();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = false;

        if (promptUI != null)
            promptUI.SetActive(false);

        if (sharedStageNameText != null)
            sharedStageNameText.text = "";
    }

#if UNITY_EDITOR
    public void RefreshSceneName()
    {
        if (sceneAsset != null)
            sceneAsset.name.ToString();
    }
#endif
}
