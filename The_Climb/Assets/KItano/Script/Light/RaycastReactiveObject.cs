using UnityEngine;
using System.Collections;

public class RaycastReactiveObject : MonoBehaviour
{
    [Header("初期表示設定")]
    public bool startVisible = true;  // 初期表示状態

    [Header("フェード設定")]
    public float fadeSpeed = 1f;       // フェード速度

    [Header("待機＆点滅設定")]
    public float stayTime = 2f;        // 完全表示／透明後の待機時間
    public float blinkDuration = 1f;   // 点滅時間（完全表示時のみ）
    public float blinkInterval = 0.15f;// 点滅間隔

    [Header("コライダー設定")]
    public Collider raycastCollider;   // Raycast判定用（isTrigger = true）
    public Collider solidCollider;     // 実体Collider（isTrigger = false）

    // 内部変数
    private Renderer rend;
    private Material mat;

    private float currentAlpha;
    private bool rayHitThisFrame;
    private bool isLocked;
    private bool hasCompletedThisCycle;
    private bool awaitRaycastNextLoop; // 次ループをRaycast待ちか

    void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;

        currentAlpha = startVisible ? 1f : 0f;
        ApplyState(currentAlpha);

        raycastCollider.enabled = true;

        Debug.Log($"[{name}] 初期化完了 | 初期表示状態: {(startVisible ? "表示" : "非表示")}");
    }

    void Update()
    {
        if (isLocked) return;

        // 完全表示／完全透明後のループ待ち
        if (awaitRaycastNextLoop)
        {
            if (rayHitThisFrame)
            {
                Debug.Log($"[{name}] 次ループ開始のRaycastを検知");
                awaitRaycastNextLoop = false;
                hasCompletedThisCycle = false; // 次ループ開始
            }
        }

        // フェード処理
        if (!awaitRaycastNextLoop)
        {
            float targetAlpha = rayHitThisFrame ? (startVisible ? 0f : 1f) : (startVisible ? 1f : 0f);

            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
            ApplyState(currentAlpha);

            // 完全到達判定
            if (!hasCompletedThisCycle && Mathf.Approximately(currentAlpha, targetAlpha))
            {
                hasCompletedThisCycle = true;

                if (targetAlpha == 1f)
                    Debug.Log($"[{name}] オブジェクトが完全に表示されました");
                else
                    Debug.Log($"[{name}] オブジェクトが完全に透明になりました");

                StartCoroutine(StateCompleteRoutine());
            }
        }

        rayHitThisFrame = false;
    }

    // Raycastから呼ばれる
    public void OnRaycastHit()
    {
        if (!rayHitThisFrame)
            Debug.Log($"[{name}] Raycastがオブジェクトに当たりました");

        rayHitThisFrame = true;
    }

    // アルファとColliderを反映
    void ApplyState(float alpha)
    {
        Color c = mat.color;
        c.a = alpha;
        mat.color = c;

        // 実体Colliderのみ制御
        solidCollider.enabled = alpha >= 0.95f;
    }

    // 完全到達後の待機・点滅・Raycast待ち設定
    IEnumerator StateCompleteRoutine()
    {
        isLocked = true;

        Debug.Log($"[{name}] 完全到達 → 一定時間待機開始 ({stayTime}秒)");
        yield return new WaitForSeconds(stayTime);

        // 完全表示時のみ点滅
        float targetAlpha = startVisible ? 1f : 0f;
        if (targetAlpha == 1f)
        {
            Debug.Log($"[{name}] 点滅開始（完全表示時のみ）");
            yield return StartCoroutine(BlinkRoutine());
        }

        // 点滅後または完全透明時はRaycast待ちにする
        awaitRaycastNextLoop = true;
        Debug.Log($"[{name}] 次のループ開始をRaycast待ちに設定");

        isLocked = false;
    }

    // 点滅処理
    IEnumerator BlinkRoutine()
    {
        int totalBlinks = Mathf.CeilToInt(blinkDuration / blinkInterval);

        for (int i = 0; i < totalBlinks; i++)
        {
            rend.enabled = !rend.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        rend.enabled = true; // 最終的に必ず表示状態
    }
}
