using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Awake()
    {
        // 起動直後は黒で完全に塗りつぶしておく（アルファ1）
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1f);
        }
    }

    private void Start()
    {
        // 最初にフェードイン
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeOut()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 念のため完全に黒
        fadeImage.color = new Color(0, 0, 0, 1f);
    }

    public IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 完全に透明にしておく
        fadeImage.color = new Color(0, 0, 0, 0f);
    }
}
