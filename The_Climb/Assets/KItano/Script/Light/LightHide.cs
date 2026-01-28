using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class LightHide : MonoBehaviour
{
    [Header("① 消失設定")]
    [SerializeField] private float hideTime_Hide = 2f;

    [Header("② 再出現設定")]
    [SerializeField] private float stayHiddenTime_Hide = 3f;
    [SerializeField] private float blinkDuration_Hide = 2f;
    [SerializeField] private float blinkInterval_Hide = 0.2f;

    [Header("③ Collider設定")]
    [SerializeField] private Collider lightHitCollider_Hide; // 常にON
    [SerializeField] private Collider solidCollider_Hide;    // 表示中のみON

    [Header("④ Controller設定")]
    [SerializeField] private LightHideController controller_Hide;

    private Renderer rend_Hide;
    private Coroutine routine_Hide;
    private bool isRunning_Hide = false;

    // --------------------------------------------------
    // ④ 初期化
    // --------------------------------------------------
    private void Awake()
    {
        Debug.Log($"LightHide：④ 初期化 ({gameObject.name})");

        rend_Hide = GetComponent<Renderer>();
        SetAlpha_Hide(1f);

        if (solidCollider_Hide) solidCollider_Hide.enabled = true;
        if (lightHitCollider_Hide) lightHitCollider_Hide.enabled = true;
    }

    // --------------------------------------------------
    // ⑤ イベント登録
    // --------------------------------------------------
    private void OnEnable()
    {
        if (controller_Hide == null)
        {
            Debug.LogError("LightHide：Controller未設定");
            return;
        }

        controller_Hide.OnLightEnter += HandleLightEnter_Hide;
        Debug.Log("LightHide：⑤ Controller登録完了");
    }

    private void OnDisable()
    {
        if (controller_Hide != null)
            controller_Hide.OnLightEnter -= HandleLightEnter_Hide;
        Debug.Log("LightHide：⑤ Controller登録解除");
    }

    // --------------------------------------------------
    // ⑥ 光検知
    // --------------------------------------------------
    private void HandleLightEnter_Hide(GameObject hitObj, Color color)
    {
        if (isRunning_Hide) return;
        if (hitObj != lightHitCollider_Hide.gameObject) return;

        Debug.Log("LightHide：⑥ 光検知 → 消失開始");
        routine_Hide = StartCoroutine(MainRoutine_Hide());
    }

    // --------------------------------------------------
    // ⑦ メイン処理
    // --------------------------------------------------
    private IEnumerator MainRoutine_Hide()
    {
        isRunning_Hide = true;

        yield return Hide_Hide();
        yield return new WaitForSeconds(stayHiddenTime_Hide);
        yield return Blink_Hide();
        Reveal_Hide();

        isRunning_Hide = false;
        routine_Hide = null;

        Debug.Log("LightHide：⑦ 待機状態へ戻る（再消去可能）");
    }

    // --------------------------------------------------
    // ⑧ フェードアウト
    // --------------------------------------------------
    private IEnumerator Hide_Hide()
    {
        Debug.Log("LightHide：⑧ フェードアウト開始");

        float t = 0f;
        Color start = rend_Hide.material.color;
        Color end = start;
        end.a = 0f;

        while (t < hideTime_Hide)
        {
            t += Time.deltaTime;
            rend_Hide.material.color = Color.Lerp(start, end, t / hideTime_Hide);
            yield return null;
        }

        rend_Hide.material.color = end;

        if (solidCollider_Hide)
        {
            solidCollider_Hide.enabled = false;
            Debug.Log("LightHide：⑧ 当たり判定OFF");
        }
    }

    // --------------------------------------------------
    // ⑨ 点滅（復活前）
    // --------------------------------------------------
    private IEnumerator Blink_Hide()
    {
        Debug.Log("LightHide：⑨ 点滅開始（復活予兆）");

        float elapsed = 0f;
        bool visible = false;

        while (elapsed < blinkDuration_Hide)
        {
            elapsed += blinkInterval_Hide;
            visible = !visible;
            SetAlpha_Hide(visible ? 1f : 0f);
            yield return new WaitForSeconds(blinkInterval_Hide);
        }
    }

    // --------------------------------------------------
    // ⑩ 再出現
    // --------------------------------------------------
    private void Reveal_Hide()
    {
        Debug.Log("LightHide：⑩ 再出現");

        SetAlpha_Hide(1f);

        if (solidCollider_Hide)
            solidCollider_Hide.enabled = true;
    }

    // --------------------------------------------------
    // ⑪ 共通処理
    // --------------------------------------------------
    private void SetAlpha_Hide(float a)
    {
        Color c = rend_Hide.material.color;
        c.a = a;
        rend_Hide.material.color = c;
    }
}
