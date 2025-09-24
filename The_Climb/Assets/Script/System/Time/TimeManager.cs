using System.Collections;
using TheClimb.Utility;
using UnityEngine;
using Zenject;

//  譌･莉倥・譎る俣邂｡逅・せ繧ｯ繝ｪ繝励ヨ
public class TimeManager : MonoBehaviour, ITimeProvider
{
    public SafeEventUtility OnChangedNight = new SafeEventUtility();    //  夜に変わった時の処理
    public Coroutine defaultTimeCount;    //  譎る俣蜉騾滄未謨ｰ
    public Coroutine timeAcceleration;    //  譎る俣蜉騾滄未謨ｰ

    [Inject] ITimeConfig TimeConfig;    //  譎る俣險ｭ螳・

    public float CurrentTime;    //  迴ｾ蝨ｨ縺ｮ譎る俣
    float TimeProgressValue;    //  ・醍ｧ貞ｽ薙◆繧翫・譎る俣騾ｲ陦悟､
    int CurrentDay;    //  迴ｾ蝨ｨ縺ｮ譌･莉・
    [SerializeField] bool IsNight;    //  螟懊°縺ｩ縺・°縺ｮ繝輔Λ繧ｰ
    bool IsStopCount;    //  繧ｿ繧､繝槭・繧呈ｭ｢繧√ｋ縺区ｭ｢繧√↑縺・°
    bool IsPlayerAttacked;    //  繝励Ξ繧､繝､繝ｼ縺梧判謦・＆繧後◆縺具ｼ井ｻｮ繝輔Λ繧ｰ・・

    //    繝励Ξ繧､繝､繝ｼ縺梧判謦・＆繧後◆縺九・繝ｭ繝代ユ繧｣
    public bool IsPlayerAttackedProperty
    {
        get => IsPlayerAttacked;
        set => IsPlayerAttacked = value;
    }

    //  迴ｾ蝨ｨ譎る俣蜿門ｾ励・繝ｭ繝代ユ繧｣
    public float CurrentTimeProperty
    {
        get => CurrentTime;
        set => CurrentTime = value;
    }
    //  迴ｾ蝨ｨ譌･謨ｰ蜿門ｾ励・繝ｭ繝代ユ繧｣
    public int CurrentDayProperty => CurrentDay;
    //  螟懷叙蠕励・繝ｭ繝代ユ繧｣
    public bool IsNightProperty
    {
        get => IsNight;
        set
        {
            if (IsNight != value)
            {
                IsNight = value;
                Debug.Log(value);
                OnChangedNight.Invoke();
            }
        }
    }
    void Awake()
    {
        Debug.Log($"[TimeManager] Awake - Instance ID: {this.GetInstanceID()}");
        //    値系の初期化
        InitializeValue();
    }
    void Start()
    {
        CoroutineUtility.SafeStartCoroutine(this, ref defaultTimeCount, DefaultTimeCount());
    }
    //  蛻晄悄蛹夜未謨ｰ
    void InitializeValue()
    {
        CurrentTime = TimeConfig.InitializeTimeProperty;
        TimeProgressValue = TimeConfig.ProgressTimeProperty;
        CurrentDay = TimeConfig.InitializeDateProperty;
        IsStopCount = false;
        IsPlayerAttacked = false;
    }
    //  譎る俣蜉騾溘Λ繝・ヱ繝ｼ髢｢謨ｰ
    public void StartTimeAcceleration(float TargetValue, float Duration)
    {
        CoroutineUtility.SafeStopCoroutine(this, ref defaultTimeCount);
        CoroutineUtility.SafeStartCoroutine(this, ref timeAcceleration, TimeAcceleration(TargetValue, Duration));
    }
    //  繝・ヵ繧ｩ繝ｫ繝域凾髢薙き繧ｦ繝ｳ繝磯未謨ｰ
    IEnumerator DefaultTimeCount()
    {
        while (!IsStopCount)
        {
            CurrentTime += TimeProgressValue * Time.deltaTime;
            yield return null;
        }
    }
    //  譎る俣蜉騾滄未謨ｰ
    public IEnumerator TimeAcceleration(float TargetTime, float Duration)
    {
        //float TimeDiference = Mathf.Abs(TargetTime - CurrentTime);    //  迴ｾ蝨ｨ譎る俣縺ｨ逶ｮ讓呎凾髢薙・蟾ｮ
        float WrappedTargrtTime = TargetTime > CurrentTime ? TargetTime : TargetTime + TimeConfig.OneDayTimeProperty;
        float TimeUntilTarget = WrappedTargrtTime - CurrentTime;
        float SecProgress = TimeUntilTarget / Duration;    //  1遘偵≠縺溘ｊ縺ｮ騾ｲ陦悟､
        float Epsilon = 2f;

        //int Direction = (TargetTime >= CurrentTime) ? 1 : -1;    //  蜉邂励°貂帷ｮ励・譁ｹ蜷・

        //while ((Direction == 1 && CurrentTime < TargetTime) || (Direction == -1 && CurrentTime > TargetTime))
        //{
        //    CurrentTime += Direction * SecProgress * Time.deltaTime;

        //    if((Direction == 1 && CurrentTime > TargetTime) || (Direction == -1 && CurrentTime > TargetTime))
        //    {
        //        CurrentTime = TargetTime;
        //    }

        //    yield return null;
        //}

        while (Mathf.Abs(WrappedTargrtTime - CurrentTime) > Epsilon)
        {
            CurrentTime += SecProgress * Time.deltaTime;
            yield return null;
        }
        Debug.Log("NightTrout");
        CurrentTime = TargetTime;
        timeAcceleration = null;
        defaultTimeCount = StartCoroutine(DefaultTimeCount());
        IsPlayerAttacked = false;
    }
}