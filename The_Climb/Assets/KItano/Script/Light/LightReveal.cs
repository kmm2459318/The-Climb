using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class LightReveal : MonoBehaviour
{
    [Header("① 出現設定")]
    [SerializeField] private float revealTime_Reveal = 2f;

    [Header("② 消失設定")]
    [SerializeField] private float stayTime_Reveal = 3f;
    [SerializeField] private float blinkDuration_Reveal = 2f;
    [SerializeField] private float blinkInterval_Reveal = 0.2f;

    [Header("③ Collider設定")]
    [SerializeField] private Collider lightHitCollider_Reveal; // 常にON
    [SerializeField] private Collider solidCollider_Reveal;    // 出現時のみON

    [Header("④ Controller設定")]
    [SerializeField] private LightRevealController controller_Reveal;

    private Renderer rend_Reveal;
    private Coroutine routine_Reveal;
    private bool isRunning_Reveal = false;

    // --------------------------------------------------
    // ④ 初期化
    // --------------------------------------------------
    private void Awake()
    {
        Debug.Log($"LightReveal：④ 初期化 ({gameObject.name})");

        rend_Reveal = GetComponent<Renderer>();
        SetAlpha_Reveal(0f);

        if (solidCollider_Reveal) solidCollider_Reveal.enabled = false;
        if (lightHitCollider_Reveal) lightHitCollider_Reveal.enabled = true;
    }

    // --------------------------------------------------
    // ⑤ イベント登録
    // --------------------------------------------------
    private void OnEnable()
    {
        if (controller_Reveal == null)
        {
            Debug.LogError("LightReveal：Controller未設定");
            return;
        }

        controller_Reveal.OnLightEnter += HandleLightEnter_Reveal;
        Debug.Log("LightReveal：⑤ Controller登録完了");
    }

    private void OnDisable()
    {
        if (controller_Reveal != null)
            controller_Reveal.OnLightEnter -= HandleLightEnter_Reveal;
        Debug.Log("LightReveal：⑤ Controller登録解除");
    }

    // --------------------------------------------------
    // ⑥ 光検知
    // --------------------------------------------------
    private void HandleLightEnter_Reveal(GameObject hitObj, Color color)
    {
        if (isRunning_Reveal) return;
        if (hitObj != lightHitCollider_Reveal.gameObject) return;

        Debug.Log("LightReveal：⑥ 光検知 → 出現開始");
        routine_Reveal = StartCoroutine(MainRoutine_Reveal());
    }

    // --------------------------------------------------
    // ⑦ メイン処理
    // --------------------------------------------------
    private IEnumerator MainRoutine_Reveal()
    {
        isRunning_Reveal = true;

        yield return Reveal_Reveal();
        yield return new WaitForSeconds(stayTime_Reveal);
        yield return Blink_Reveal();
        Hide_Reveal();

        isRunning_Reveal = false;
        routine_Reveal = null;

        Debug.Log("LightReveal：⑦ 待機状態へ戻る（再照射可能）");
    }

    // --------------------------------------------------
    // ⑧ フェードイン
    // --------------------------------------------------
    private IEnumerator Reveal_Reveal()
    {
        Debug.Log("LightReveal：⑧ フェードイン開始");

        float t = 0f;
        Color start = rend_Reveal.material.color;
        Color end = start;
        end.a = 1f;

        while (t < revealTime_Reveal)
        {
            t += Time.deltaTime;
            rend_Reveal.material.color = Color.Lerp(start, end, t / revealTime_Reveal);
            yield return null;
        }

        rend_Reveal.material.color = end;

        if (solidCollider_Reveal)
        {
            solidCollider_Reveal.enabled = true;
            Debug.Log("LightReveal：⑧ 当たり判定ON");
        }
    }

    // --------------------------------------------------
    // ⑨ 点滅
    // --------------------------------------------------
    private IEnumerator Blink_Reveal()
    {
        Debug.Log("LightReveal：⑨ 点滅開始");

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < blinkDuration_Reveal)
        {
            elapsed += blinkInterval_Reveal;
            visible = !visible;
            SetAlpha_Reveal(visible ? 1f : 0f);
            yield return new WaitForSeconds(blinkInterval_Reveal);
        }
    }

    // --------------------------------------------------
    // ⑩ 消失
    // --------------------------------------------------
    private void Hide_Reveal()
    {
        Debug.Log("LightReveal：⑩ 消失");

        SetAlpha_Reveal(0f);

        if (solidCollider_Reveal)
            solidCollider_Reveal.enabled = false;
    }

    // --------------------------------------------------
    // ⑪ 共通処理
    // --------------------------------------------------
    private void SetAlpha_Reveal(float a)
    {
        Color c = rend_Reveal.material.color;
        c.a = a;
        rend_Reveal.material.color = c;
    }
}
