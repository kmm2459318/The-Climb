using UnityEngine;
using System.Collections;

public class BgmYSwitchFade : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float thresholdY = 172f;
    [SerializeField] AudioClip StageClip;
    [SerializeField] AudioClip BossClip;
    [SerializeField] float fadeDuration = 1.5f;
    [SerializeField] float maxVolume = 1f;

    AudioSource srcA;
    AudioSource srcB;
    AudioSource active;
    bool isHigh = false;
    Coroutine fadeRoutine;

    void Awake()
    {
        srcA = gameObject.AddComponent<AudioSource>();
        srcB = gameObject.AddComponent<AudioSource>();
        srcA.loop = srcB.loop = true;
        srcA.playOnAwake = srcB.playOnAwake = false;
        active = srcA;
    }

    void Start()
    {
        if (target == null) Debug.LogWarning("BgmYSwitchFade: target が設定されていません。");
        // 初期クリップをセット（y によって決定）
        isHigh = (target != null && target.position.y >= thresholdY);
        AudioSource inactive = (active == srcA) ? srcB : srcA;
        active.clip = isHigh ? BossClip : StageClip;
        inactive.clip = isHigh ? StageClip : BossClip;
        active.volume = maxVolume;
        inactive.volume = 0f;
        if (active.clip != null) active.Play();
    }

    void Update()
    {
        if (target == null) return;
        bool nowHigh = target.position.y >= thresholdY;
        if (nowHigh != isHigh)
        {
            isHigh = nowHigh;
            StartFade(isHigh ? BossClip : StageClip);
        }
    }

    void StartFade(AudioClip nextClip)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(CrossFadeTo(nextClip));
    }

    IEnumerator CrossFadeTo(AudioClip nextClip)
    {
        AudioSource next = (active == srcA) ? srcB : srcA;
        // 次のソースにクリップをセットして再生（音量 0 から）
        next.clip = nextClip;
        next.volume = 0f;
        if (next.clip != null && !next.isPlaying) next.Play();

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / fadeDuration);
            active.volume = Mathf.Lerp(maxVolume, 0f, p);
            next.volume = Mathf.Lerp(0f, maxVolume, p);
            yield return null;
        }

        // 切替完了
        active.volume = 0f;
        next.volume = maxVolume;
        if (active.isPlaying) active.Stop();
        active = next;
        fadeRoutine = null;
    }
}
