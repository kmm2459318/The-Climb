using UnityEngine;
using System.Collections;

public class RaycastReactiveObject : MonoBehaviour
{
    public bool startVisible = true;

    [Header("Fade")]
    public float fadeSpeed = 1f;

    [Header("Return")]
    public float stayTime = 2f;
    public float blinkDuration = 1f;
    public float blinkInterval = 0.15f;

    Renderer rend;
    Collider col;
    Material mat;

    float currentAlpha;
    bool rayHitThisFrame;
    bool isLocked;

    void Start()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();

        // マテリアルはインスタンス化しておく（重要）
        mat = rend.material;

        currentAlpha = startVisible ? 1f : 0f;
        ApplyState(currentAlpha);
    }

    void Update()
    {
        if (isLocked) return;

        float targetAlpha;

        if (rayHitThisFrame)
        {
            targetAlpha = startVisible ? 0f : 1f;
        }
        else
        {
            targetAlpha = startVisible ? 1f : 0f;
        }

        currentAlpha = Mathf.MoveTowards(
            currentAlpha,
            targetAlpha,
            fadeSpeed * Time.deltaTime
        );

        ApplyState(currentAlpha);

        if (Mathf.Approximately(currentAlpha, targetAlpha))
        {
            StartCoroutine(StateCompleteRoutine());
        }

        rayHitThisFrame = false;
    }

    public void OnRaycastHit()
    {
        rayHitThisFrame = true;
    }

    void ApplyState(float alpha)
    {
        Color c = mat.color;
        c.a = alpha;
        mat.color = c;

        col.enabled = alpha >= 0.95f;
    }

    IEnumerator StateCompleteRoutine()
    {
        isLocked = true;

        yield return new WaitForSeconds(stayTime);
        yield return StartCoroutine(BlinkRoutine());

        startVisible = !startVisible;
        isLocked = false;
    }

    IEnumerator BlinkRoutine()
    {
        float timer = 0f;

        while (timer < blinkDuration)
        {
            rend.enabled = !rend.enabled;
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        rend.enabled = true;
    }
}
