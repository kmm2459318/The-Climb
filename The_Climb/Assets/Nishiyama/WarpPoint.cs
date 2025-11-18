using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WarpPoint : MonoBehaviour
{
    [Header("ワープ時のフェードUI")]
    public Image fadeImage;

    [Header("吸い込まれる速度")]
    public float suckSpeed = 3f;

    [Header("ワープ先Floor Index")]
    public int targetFloorIndex = 0;

    [Header("フェード時間")]
    public float fadeDuration = 1f;

    private bool isWarping = false;
    private FloorSystemManager floorManager;

    private void Start()
    {
        // シーン内からFloorSystemManagerを自動取得
        floorManager = FindObjectOfType<FloorSystemManager>();
        if (floorManager == null)
        {
            Debug.LogError("[WarpPoint] FloorSystemManagerがシーン内に見つかりません。");
        }

        if (fadeImage == null && floorManager != null)
        {
            fadeImage = floorManager.fadeImage; // FloorSystemManagerのUIを使い回す
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isWarping) return;
        if (other.CompareTag("Player"))
        {
            StartCoroutine(WarpSequence(other.gameObject));
        }
    }

    private IEnumerator WarpSequence(GameObject player)
    {
        if (isWarping) yield break;
        isWarping = true;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        Vector3 startPos = player.transform.position;
        Vector3 targetPos = transform.position;

        // 吸い込まれる演出
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * suckSpeed;
            player.transform.position = Vector3.Lerp(startPos, targetPos, t);
            player.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            yield return null;
        }

        yield return StartCoroutine(FadeOut(fadeDuration / 2f));

        // FloorSystemManagerに切り替えを依頼
        if (floorManager != null)
        {
            floorManager.SendMessage("SwitchFloorRoutineExternal", targetFloorIndex);
        }

        yield return StartCoroutine(FadeIn(fadeDuration / 2f));

        isWarping = false;
    }

    private IEnumerator FadeOut(float duration)
    {
        if (fadeImage == null) yield break;

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    private IEnumerator FadeIn(float duration)
    {
        if (fadeImage == null) yield break;

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - t / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
