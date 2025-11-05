using UnityEngine;
using System.Collections;

public class ScaleLooper : MonoBehaviour
{
    [Header("スケール設定")]
    public float shrinkTime = 1f;      // 0まで縮小する時間
    public float waitTime = 4f;        // 縮小・拡大の待機時間
    public float expandTime = 1f;      // 等倍まで拡大する時間

    [Header("初回のみ待機時間")]
    public float initialWait = 0f;     // 初回だけ待つ時間

    private Vector3 originalScale;     // 元のスケール
    private Vector3 originalPosition;  // 元の位置
    private bool isRunning = false;

    void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.position;
        StartCoroutine(ScaleLoop());
    }

    private IEnumerator ScaleLoop()
    {
        if (isRunning) yield break;
        isRunning = true;

        // 初回待機
        if (initialWait > 0f)
            yield return new WaitForSeconds(initialWait);

        while (true)
        {
            // === 縮小 ===
            yield return StartCoroutine(ScaleTo(0f, shrinkTime, beforeZeroAction: () =>
            {
                // 0になる直前にZ座標を -2
                Vector3 pos = transform.position;
                pos.z -= 2f;
                transform.position = pos;
            }));

            // 待機①
            if (waitTime > 0f)
                yield return new WaitForSeconds(waitTime);

            // === 拡大 ===
            // 0から大きくなる直前にZ座標を +2（元に戻す）
            Vector3 restorePos = transform.position;
            restorePos.z = originalPosition.z;
            transform.position = restorePos;

            yield return StartCoroutine(ScaleTo(1f, expandTime));

            // 待機②（①と同じ）
            if (waitTime > 0f)
                yield return new WaitForSeconds(waitTime);
        }
    }

    /// <summary>
    /// 初期スケールを基準に、scaleRatio（0〜1）に応じて補間。
    /// </summary>
    private IEnumerator ScaleTo(float scaleRatio, float duration, System.Action beforeZeroAction = null)
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = originalScale * scaleRatio;
        float elapsed = 0f;
        bool actionInvoked = false;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsed += Time.deltaTime;

            // 95% 進んだあたりで1回だけ実行
            if (!actionInvoked && t >= 0.95f && beforeZeroAction != null)
            {
                beforeZeroAction.Invoke();
                actionInvoked = true;
            }

            yield return null;
        }

        transform.localScale = targetScale;
    }
}
