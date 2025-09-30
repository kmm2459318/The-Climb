using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageSwitcher : MonoBehaviour
{
    [Header("ステージPrefab")]
    public GameObject lightStagePrefab;
    public GameObject darkStagePrefab;

    [Header("フェード用")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1f;

    private GameObject currentStage;
    private bool isLightWorld = true;

    void Start()
    {
        // 最初は光のステージをロード
        currentStage = Instantiate(lightStagePrefab);
        fadeCanvas.alpha = 0;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchStage();
        }
    }
    public void SwitchStage()
    {
        StartCoroutine(SwitchRoutine());
    }

    IEnumerator SwitchRoutine()
    {
        // フェードアウト
        yield return Fade(1f);

        // 既存ステージ削除
        if (currentStage != null) Destroy(currentStage);

        // ステージ切り替え
        if (isLightWorld)
        {
            currentStage = Instantiate(darkStagePrefab);
        }
        else
        {
            currentStage = Instantiate(lightStagePrefab);
        }
        isLightWorld = !isLightWorld;

        // フェードイン
        yield return Fade(0f);
    }

    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvas.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }
        fadeCanvas.alpha = targetAlpha;
    }
}