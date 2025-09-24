using UnityEngine;
using System.Collections;

//  闇の進行度を調整する
public class FadeController : MonoBehaviour, IDownFading
{
    [SerializeField] FadeSetting fadeSetting;    //  フェード設定
    [SerializeField] GameObject fadeQuad;    //  フェード板
    Material overLayMaterial;    //  四隅からフェードするのマテリアル
    Coroutine downFadeCoroutine;    //  ダウンフェードコルーチン
    Coroutine fadeCoroutine;    //  ダウンフェードコルーチン

    [Header("現在のフェード進行度")]
    [SerializeField, Range(0, 1)] float CurrentProgress;    //  フェード進行度
    float CurrentProgressRate_Sec;    //  秒間のフェード進行速度(割合値)

    void Awake()
    {
        overLayMaterial = fadeSetting.OverLayMaterial;

        //  初期化
        InitializeValue();
    }
    //  初期化
    void InitializeValue()
    {
        CurrentProgress = fadeSetting.Progress;
        CurrentProgressRate_Sec = fadeSetting.FadeProgressRate_Sec;
    }
    //  ダウン時フェードラッパー
    public void StartDownFading()
    {
        //CoroutineUtility.SafeStartCoroutine(this, ref downFadeCoroutine, DownFading());
        CoroutineUtility.SafeStartCoroutine(this, ref fadeCoroutine, DownFading());
    }
    //  フェードアウト処理
    public IEnumerator DownFading()
    {
        while(CurrentProgress > 0)
        {
            FadeSetter.AdjustFadeQuadPosition(fadeQuad);
            CurrentProgress -= CurrentProgressRate_Sec * Time.deltaTime;
            overLayMaterial.SetFloat("_Progress", CurrentProgress);
            yield return null;
        }
        while (CurrentProgress <= 1)
        {
            FadeSetter.AdjustFadeQuadPosition(fadeQuad);
            CurrentProgress += CurrentProgressRate_Sec * Time.deltaTime;
            overLayMaterial.SetFloat("_Progress", CurrentProgress);
            yield return null;
        }
        downFadeCoroutine = null;
    }
    //  フェードルーチン
    private IEnumerator FadeRoutine(float from, float to)
    {
        CurrentProgress = from;
        overLayMaterial.SetFloat("_Progress", CurrentProgress);
        FadeSetter.AdjustFadeQuadPosition(fadeQuad);

        float direction = Mathf.Sign(to - from);

        while (direction > 0 ? CurrentProgress < to : CurrentProgress > to)
        {
            CurrentProgress += CurrentProgressRate_Sec * direction * Time.deltaTime;
            CurrentProgress = Mathf.Clamp01(CurrentProgress);

            overLayMaterial.SetFloat("_Progress", CurrentProgress);
            yield return null;
        }

        fadeCoroutine = null;
    }
}