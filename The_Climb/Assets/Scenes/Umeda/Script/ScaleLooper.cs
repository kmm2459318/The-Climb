using UnityEngine;
using System.Collections;

public class ScaleLooper : MonoBehaviour
{
    [Header("スケール設定")]
    public float shrinkTime = 1f;
    public float waitTime = 4f;
    public float expandTime = 1f;

    [Header("初回のみ待機時間")]
    public float initialWait = 0f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private bool isRunning = false;

    IEnumerator Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.position;

        // 🔵 シーンが準備完了するまで待つ（ロード時間によるズレ防止）
        while (!SceneReadyManager.SceneReady)
            yield return null;

        // ここで初めてギミック開始
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
                Vector3 pos = transform.position;
                pos.z -= 2f;
                transform.position = pos;
            }));

            // 待機①
            if (waitTime > 0f)
                yield return new WaitForSeconds(waitTime);

            // === 拡大 ===
            Vector3 restorePos = transform.position;
            restorePos.z = originalPosition.z;
            transform.position = restorePos;

            yield return StartCoroutine(ScaleTo(1f, expandTime));

            // 待機②
            if (waitTime > 0f)
                yield return new WaitForSeconds(waitTime);
        }
    }

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
