using UnityEngine;

public class RevealOnLightAndPlayer : MonoBehaviour
{
    [Header("ライト色設定")]
    [SerializeField] private Color purpleColor = new Color(0.5f, 0f, 1f);
    [SerializeField] private float colorThreshold = 0.2f;

    [Header("時間設定")]
    [SerializeField] private float activationTime = 3f;
    [SerializeField] private float stayVisibleTime = 2f;
    [SerializeField] private float fadeDuration = 1f;

    [Header("コライダー設定")]
    [SerializeField] private Collider solidCollider;

    private Renderer rend;
    private bool isPlayerInside = false;
    private bool isLitByPurple = false;
    private float exposureTimer = 0f;

    private bool isActivated = false;
    private Coroutine fadeRoutine;


    private void OnEnable()
    {
        LightController.OnLightEnter += HandleLightEnter;
        LightController.OnLightExit += HandleLightExit;
        LightController.OnLightColorChanged += HandleColorChange;
    }

    private void OnDisable()
    {
        LightController.OnLightEnter -= HandleLightEnter;
        LightController.OnLightExit -= HandleLightExit;
        LightController.OnLightColorChanged -= HandleColorChange;
    }

    void Start()
    {
        rend = GetComponent<Renderer>();

        // 最初は透明
        Color c = rend.material.color;
        c.a = 0f;
        rend.material.color = c;

        if (solidCollider) solidCollider.enabled = false;
    }

    // ---------------------------------------------------------------
    // LightController からイベントを受け取る
    // ---------------------------------------------------------------
    private void HandleLightEnter(GameObject hitObj, Color currentColor)
    {
        if (hitObj != this.gameObject) return;

        isLitByPurple = IsPurple(currentColor);
    }

    private void HandleLightExit(GameObject hitObj, Color currentColor)
    {
        if (hitObj != this.gameObject) return;

        isLitByPurple = false;
        exposureTimer = 0f;

        if (!isActivated)
        {
            // フェードアウト
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeAlpha(0f));
        }
    }

    private void HandleColorChange(Color newColor)
    {
        // 色変更イベント時、もし照らされている状態なら即反応
        if (LightIsPointingHere())
        {
            isLitByPurple = IsPurple(newColor);
        }
    }

    // ---------------------------------------------------------------
    // プレイヤーの出入り
    // ---------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInside = false;
    }

    // ---------------------------------------------------------------
    // メインロジック
    // ---------------------------------------------------------------
    void Update()
    {
        if (isActivated) return;

        if (isLitByPurple && isPlayerInside)
        {
            exposureTimer += Time.deltaTime;

            if (exposureTimer >= activationTime)
                StartActivation();
        }
        else
        {
            exposureTimer = 0f;
        }
    }

    // ---------------------------------------------------------------
    // 実体化
    // ---------------------------------------------------------------
    private void StartActivation()
    {
        isActivated = true;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(ActivationRoutine());
    }

    private System.Collections.IEnumerator ActivationRoutine()
    {
        // コライダーON
        if (solidCollider) solidCollider.enabled = true;

        // 水色にフェード
        yield return FadeColorAlpha(Color.cyan, 1f);

        yield return new WaitForSeconds(stayVisibleTime);

        // 元に戻すフェード
        yield return FadeColorAlpha(Color.white, 0f);

        // コライダーOFF
        if (solidCollider) solidCollider.enabled = false;

        isActivated = false;
        exposureTimer = 0f;
    }

    // ---------------------------------------------------------------
    // フェード系
    // ---------------------------------------------------------------
    private IEnumerator FadeAlpha(float targetAlpha)
    {
        Color start = rend.material.color;
        Color end = start;
        end.a = targetAlpha;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            rend.material.color = Color.Lerp(start, end, t / fadeDuration);
            yield return null;
        }
    }

    private IEnumerator FadeColorAlpha(Color targetColor, float targetAlpha)
    {
        Color start = rend.material.color;
        Color end = targetColor;
        end.a = targetAlpha;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            rend.material.color = Color.Lerp(start, end, t / fadeDuration);
            yield return null;
        }
    }

    // ---------------------------------------------------------------
    // ユーティリティ
    // ---------------------------------------------------------------
    private bool IsPurple(Color c)
    {
        return Vector3.Distance(new Vector3(c.r, c.g, c.b),
                                new Vector3(purpleColor.r, purpleColor.g, purpleColor.b))
               < colorThreshold;
    }

    private bool LightIsPointingHere()
    {
        // LightControllerが照射中のオブジェクトを管理しているため
        // OnLightEnter/Exitで状態更新されるのでこのままでOK
        return isLitByPurple;
    }
}
