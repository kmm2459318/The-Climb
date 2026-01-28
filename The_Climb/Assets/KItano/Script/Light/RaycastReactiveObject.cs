using UnityEngine;
using System.Collections;

public class RaycastReactiveObject : MonoBehaviour
{
    [Header("初期状態")]
    public bool startVisible = true;

    [Header("フェード設定")]
    public float fadeSpeed = 1f;

    [Header("待機・点滅設定")]
    public float stayTime = 2f;
    public float blinkDuration = 1f;
    public float blinkInterval = 0.15f;

    [Header("Collider")]
    public Collider raycastCollider;   // isTrigger = true
    public Collider solidCollider;     // isTrigger = false

    // 内部状態
    private Renderer rend;
    private Material mat;

    private float currentAlpha;
    private bool isCurrentlyVisible;
    private bool rayHitThisFrame;

    private bool isLocked;
    private bool awaitRaycastNextLoop;
    private bool hasCompletedThisCycle;
    private bool forceFadeOutAfterBlink;
    private bool hasStateChangeStarted;
    private bool autoReturnToVisible; // 出現スタート専用
    void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;

        isCurrentlyVisible = startVisible;
        currentAlpha = isCurrentlyVisible ? 1f : 0f;

        ApplyState(currentAlpha);

        Debug.Log($"[{name}] 初期化完了 | 初期状態: {(isCurrentlyVisible ? "表示" : "非表示")}");
    }

    void Update()
    {
        if (isLocked) return;

        // 次ループ開始待ち（Raycast必須）
        if (awaitRaycastNextLoop)
        {
            if (rayHitThisFrame)
            {
                Debug.Log($"[{name}] 次のループ開始用Raycastを検知");
                awaitRaycastNextLoop = false;
                hasCompletedThisCycle = false;
            }

            rayHitThisFrame = false;
            return;
        }
        float targetAlpha;

        // 点滅後は強制透明化
        if (forceFadeOutAfterBlink)
        {
            targetAlpha = 0f;
        }
        else if (autoReturnToVisible)
        {
            // ★ 出現スタート専用：Raycast不要で出現
            targetAlpha = 1f;
        }
        else
        {
            targetAlpha = rayHitThisFrame
                ? (isCurrentlyVisible ? 0f : 1f)
                : (isCurrentlyVisible ? 1f : 0f);
        }


        // フェード
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
        ApplyState(currentAlpha);

        // 完全到達
        if (!hasCompletedThisCycle
    && hasStateChangeStarted
    && Mathf.Approximately(currentAlpha, targetAlpha))
        {
            hasCompletedThisCycle = true;
            hasStateChangeStarted = false;

            isCurrentlyVisible = targetAlpha > 0.5f;

            if (!isCurrentlyVisible)
            {
                Debug.Log($"[{name}] オブジェクトが完全に透明になりました");

                // ★ 出現スタート専用ルート
                if (startVisible)
                {
                    Debug.Log($"[{name}] 出現スタートのため、自動再出現ルートに入ります");
                    autoReturnToVisible = true;
                }
            }
            else
            {
                Debug.Log($"[{name}] オブジェクトが完全に表示されました");
            }

            StartCoroutine(StateCompleteRoutine());
        }



        rayHitThisFrame = false;
    }

    // Raycastから呼ばれる
    public void OnRaycastHit()
    {
        if (!rayHitThisFrame)
        {
            Debug.Log($"[{name}] Raycastが命中しました");
            hasStateChangeStarted = true; 
        }

        rayHitThisFrame = true;
    }


    void ApplyState(float alpha)
    {
        Color c = mat.color;
        c.a = alpha;
        mat.color = c;

        // 実体Colliderは完全表示時のみ有効
        solidCollider.enabled = alpha >= 0.95f;
    }

    IEnumerator StateCompleteRoutine()
    {
        isLocked = true;

        Debug.Log($"[{name}] 完全到達後の待機開始（{stayTime}秒）");
        yield return new WaitForSeconds(stayTime);

        // 完全表示時のみ点滅
        if (isCurrentlyVisible)
        {
            Debug.Log($"[{name}] 点滅処理開始");
            yield return StartCoroutine(BlinkRoutine());
            forceFadeOutAfterBlink = true;
        }

        // ★ 出現スタート専用：自動復帰
        if (autoReturnToVisible)
        {
            Debug.Log($"[{name}] Raycastなしで自動的に出現状態へ戻ります");
            autoReturnToVisible = false;
            hasCompletedThisCycle = false;
            isLocked = false;
            yield break;
        }

        // 通常ルート
        Debug.Log($"[{name}] 次ループはRaycast待ち");
        awaitRaycastNextLoop = true;

        isLocked = false;
    }



    IEnumerator BlinkRoutine()
    {
        int blinkCount = Mathf.CeilToInt(blinkDuration / blinkInterval);

        for (int i = 0; i < blinkCount; i++)
        {
            rend.enabled = !rend.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        rend.enabled = true;
        Debug.Log($"[{name}] 点滅終了、表示状態を確定");
    }
}
