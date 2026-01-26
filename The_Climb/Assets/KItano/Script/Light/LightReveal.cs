// ----------------------------
// LightReveal.cs
// ----------------------------
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class LightReveal : MonoBehaviour
{
    [Header("① 出現設定")]
    [SerializeField] private float revealTime = 2f;

    [Header("② 消失設定")]
    [SerializeField] private float stayTime = 3f;
    [SerializeField] private float blinkDuration = 2f;
    [SerializeField] private float blinkInterval = 0.2f;

    [Header("③ Collider設定")]
    [SerializeField] private Collider lightHitCollider; // 常にON
    [SerializeField] private Collider solidCollider;    // 出現時のみON

    [Header("④ Controller設定")]
    [SerializeField] private LightRevealController controller;

    private Renderer rend;
    private Coroutine routine;
    private bool isRunning = false;

    // ----------------------------
    // 初期化
    // ----------------------------
    private void Awake()
    {
        rend = GetComponent<Renderer>();
        SetAlpha(0f);

        if (solidCollider) solidCollider.enabled = false;
        if (lightHitCollider) lightHitCollider.enabled = true;

        Debug.Log($"[②] Awake ({gameObject.name}) 初期化完了");
    }

    // ----------------------------
    // イベント登録
    // ----------------------------
    private void OnEnable()
    {
        if (controller != null)
        {
            controller.OnLightEnter += HandleLightEnter;
            Debug.Log("[②] OnEnable → LightEnterイベント登録");
        }
    }

    private void OnDisable()
    {
        if (controller != null)
        {
            controller.OnLightEnter -= HandleLightEnter;
            Debug.Log("[②] OnDisable → LightEnterイベント解除");
        }
    }

    // ----------------------------
    // 光検知
    // ----------------------------
    private void HandleLightEnter(GameObject hitObj, Color color)
    {
        if (isRunning) return;
        if (hitObj != lightHitCollider.gameObject) return;

        Debug.Log("[②] 光検知 → メインルーチン開始");
        routine = StartCoroutine(MainRoutine());
    }

    // ----------------------------
    // メイン処理
    // ----------------------------
    private IEnumerator MainRoutine()
    {
        isRunning = true;

        yield return Reveal();
        yield return new WaitForSeconds(stayTime);
        yield return Blink();
        Hide();

        isRunning = false;
        routine = null;

        Debug.Log("[②] 待機状態に戻る（再照射可能）");

        // 再照射可能にする
        if (controller != null)
            controller.ResetLastHit();
    }

    // ----------------------------
    // フェードイン
    // ----------------------------
    private IEnumerator Reveal()
    {
        Debug.Log("[②] フェードイン開始");

        float t = 0f;
        Color start = rend.material.color;
        Color end = start;
        end.a = 1f;

        while (t < revealTime)
        {
            t += Time.deltaTime;
            rend.material.color = Color.Lerp(start, end, t / revealTime);
            yield return null;
        }

        rend.material.color = end;

        if (solidCollider)
        {
            solidCollider.enabled = true;
            Debug.Log("[②] フェードイン完了 → 当たり判定ON");
        }
    }

    // ----------------------------
    // 点滅
    // ----------------------------
    private IEnumerator Blink()
    {
        Debug.Log("[②] 点滅開始");

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < blinkDuration)
        {
            elapsed += blinkInterval;
            visible = !visible;
            SetAlpha(visible ? 1f : 0f);
            yield return new WaitForSeconds(blinkInterval);
        }

        Debug.Log("[②] 点滅終了");
    }

    // ----------------------------
    // 消失
    // ----------------------------
    private void Hide()
    {
        SetAlpha(0f);

        if (solidCollider)
            solidCollider.enabled = false;

        Debug.Log("[②] 消失完了 → 当たり判定OFF");
    }

    // ----------------------------
    // 共通処理
    // ----------------------------
    private void SetAlpha(float a)
    {
        Color c = rend.material.color;
        c.a = a;
        rend.material.color = c;
    }
}
