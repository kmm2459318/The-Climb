
using UnityEngine;

public class RevealOnCollision : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private Light referenceLight;
    [SerializeField] private Color activeColor = new Color(0.5f, 0f, 1f); // 紫色
    private Renderer objRenderer;
    private Color originalColor;
    private bool isRevealed = false;
    private Coroutine currentFadeCoroutine;
    private bool wasActiveColor = false;
    private bool isPlayerInside = false;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();

        if (objRenderer != null)
        {
            originalColor = objRenderer.material.color;
            Color transparentColor = originalColor;
            transparentColor.a = 0f;
            objRenderer.material.color = transparentColor;
        }

        wasActiveColor = IsActiveLightColor();
    }

    void Update()
    {
        bool isNowActive = IsActiveLightColor();

        // ライトが紫→白になったらフェードアウト
        if (wasActiveColor && !isNowActive && isRevealed)
        {
            if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = StartCoroutine(FadeToAlpha(0f));
            isRevealed = false;
        }

        // ライトが白→紫になったら、プレイヤーが中にいればフェードイン
        if (!wasActiveColor && isNowActive && isPlayerInside && !isRevealed)
        {
            if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = StartCoroutine(FadeToAlpha(originalColor.a));
            isRevealed = true;
        }

        wasActiveColor = isNowActive;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            isPlayerInside = true;

            if (IsActiveLightColor() && !isRevealed)
            {
                if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);
                currentFadeCoroutine = StartCoroutine(FadeToAlpha(originalColor.a));
                isRevealed = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            isPlayerInside = false;

            if (IsActiveLightColor() && isRevealed)
            {
                if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);
                currentFadeCoroutine = StartCoroutine(FadeToAlpha(0f));
                isRevealed = false;
            }
        }
    }

    private bool IsActiveLightColor()
    {
        return Vector3.Distance(
            new Vector3(referenceLight.color.r, referenceLight.color.g, referenceLight.color.b),
            new Vector3(activeColor.r, activeColor.g, activeColor.b)
        ) < 0.1f;
    }

    private System.Collections.IEnumerator FadeToAlpha(float targetAlpha)
    {
        float startAlpha = objRenderer.material.color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            Color newColor = objRenderer.material.color;
            newColor.a = newAlpha;
            objRenderer.material.color = newColor;

            yield return null;
        }

        Color finalColor = objRenderer.material.color;
        finalColor.a = targetAlpha;
        objRenderer.material.color = finalColor;
    }
}
