using UnityEngine;

public class RevealOnLightAndPlayer : MonoBehaviour
{
    [Header("対象設定")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private Light referenceLight;

    [Header("色設定")]
    [SerializeField] private Color activeLightColor = new Color(0.5f, 0f, 1f); // 紫ライト
    [SerializeField] private float colorThreshold = 0.2f;
    [SerializeField] private Color activatedColor = Color.cyan; // 水色
    [SerializeField] private Color originalColor = Color.white; // 元の色

    [Header("時間設定")]
    [SerializeField] private float fadeDuration = 1f;          // フェード時間
    [SerializeField] private float activationTime = 3f;        // 紫ライトを当て続ける時間
    [SerializeField] private float activatedDuration = 2f;     // 水色で表示し続ける時間

    [Header("コライダー設定")]
    [SerializeField] private Collider physicsCollider; // 物理用コライダー（別オブジェクト）

    private Collider triggerCollider;
    private Renderer objRenderer;
    private Coroutine fadeRoutine;

    private bool isPlayerInside = false;
    private bool isRevealed = false;
    private bool isActivated = false;
    private float exposureTimer = 0f;

    void Start()
    {
        Debug.Log("[Start] スクリプト開始");

        objRenderer = GetComponent<Renderer>();
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null) triggerCollider.isTrigger = true;

        if (physicsCollider != null)
        {
            physicsCollider.enabled = false;
            Debug.Log("[Start] 物理コライダーを無効化しました");
        }

        // 初期は透明
        Color startColor = originalColor;
        startColor.a = 0f;
        objRenderer.material.color = startColor;
        Debug.Log("[Start] オブジェクトを透明化しました");
    }

    void Update()
    {
        if (referenceLight == null) return;

        // ✅ 出現中（isActivated）ならライトの影響を無視
        if (isActivated) return;

        bool isActiveLight = IsActiveLightColor();

        if (isActiveLight && isPlayerInside)
        {
            exposureTimer += Time.deltaTime;
            Debug.Log($"[Update] 紫ライト中 + プレイヤー接触中, 経過時間 = {exposureTimer:F2}");

            if (exposureTimer >= activationTime)
            {
                Debug.Log("[Update] activationTime に達しました → ActivateSequence 実行");
                if (fadeRoutine != null) StopCoroutine(fadeRoutine);
                fadeRoutine = StartCoroutine(ActivateSequence());
            }
        }
        else
        {
            // 紫ライトが外れた・プレイヤーが離れたらカウントリセット
            if (exposureTimer > 0)
                Debug.Log("[Update] 照射条件が外れたので exposureTimer リセット");
            exposureTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            isPlayerInside = true;
            Debug.Log("[OnTriggerEnter] プレイヤーがトリガーに入りました");

            if (IsActiveLightColor() && !isRevealed && !isActivated)
            {
                Debug.Log("[OnTriggerEnter] 紫ライト下でプレイヤー検知 → フェードイン開始");
                if (fadeRoutine != null) StopCoroutine(fadeRoutine);
                fadeRoutine = StartCoroutine(FadeAlpha(1f)); // フェードイン
                isRevealed = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            isPlayerInside = false;
            exposureTimer = 0f;
            Debug.Log("[OnTriggerExit] プレイヤーがトリガーから出ました → フェードアウト");
            if (!isActivated) // ✅ 実体化中はフェードアウト禁止
            {
                if (fadeRoutine != null) StopCoroutine(fadeRoutine);
                fadeRoutine = StartCoroutine(FadeAlpha(0f));
                isRevealed = false;
            }
        }
    }

    private System.Collections.IEnumerator ActivateSequence()
    {
        isActivated = true;
        Debug.Log("[ActivateSequence] 開始");

        if (physicsCollider != null)
        {
            physicsCollider.enabled = true;
            Debug.Log("[ActivateSequence] 物理コライダー有効化");
        }

        // 水色にフェード
        Color start = objRenderer.material.color;
        Color end = activatedColor;
        float elapsed = 0f;
        Debug.Log("[ActivateSequence] 水色にフェード中...");
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            objRenderer.material.color = Color.Lerp(start, end, elapsed / fadeDuration);
            yield return null;
        }

        Debug.Log("[ActivateSequence] 水色フェード完了 → 水色状態で待機");
        yield return new WaitForSeconds(activatedDuration);

        // 元の色に戻す
        Debug.Log("[ActivateSequence] 元の色に戻します");
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            objRenderer.material.color = Color.Lerp(activatedColor, originalColor, elapsed / fadeDuration);
            yield return null;
        }

        // 透明に戻る
        Debug.Log("[ActivateSequence] 透明化開始");
        elapsed = 0f;
        Color transparent = originalColor; transparent.a = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            objRenderer.material.color = Color.Lerp(originalColor, transparent, elapsed / fadeDuration);
            yield return null;
        }

        if (physicsCollider != null)
        {
            physicsCollider.enabled = false;
            Debug.Log("[ActivateSequence] 物理コライダー無効化");
        }

        Debug.Log("[ActivateSequence] 完了：透明状態へ戻りました");
        isActivated = false;
        isRevealed = false;
        exposureTimer = 0f;
    }

    private System.Collections.IEnumerator FadeAlpha(float targetAlpha)
    {
        Debug.Log($"[FadeAlpha] フェード開始 → targetAlpha = {targetAlpha}");
        Color start = objRenderer.material.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            Color c = start;
            c.a = Mathf.Lerp(start.a, targetAlpha, t);
            objRenderer.material.color = c;
            yield return null;
        }

        Debug.Log("[FadeAlpha] フェード完了");
    }

    private bool IsActiveLightColor()
    {
        Vector3 lightVec = new Vector3(referenceLight.color.r, referenceLight.color.g, referenceLight.color.b);
        Vector3 targetVec = new Vector3(activeLightColor.r, activeLightColor.g, activeLightColor.b);
        bool result = Vector3.Distance(lightVec, targetVec) < colorThreshold;

        return result;
    }
}
