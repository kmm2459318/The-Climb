using UnityEngine;
using Zenject;

[RequireComponent(typeof(SunMover))]
//  太陽に移動指示を出す
public class SunMoveCommander : MonoBehaviour
{
    SunStat sunStat;    //  太陽のステータス
    SunMover sunMover;    //  太陽移動インスタンス
    ITimeProvider TimeProvider;    //  時間値プロバイダ
    ITimeConfig TimeConfig;    //  日付のインスタンス

    [Inject]
    public void Construct(
    SunStat sunStat,    //  太陽のステータス
    SunMover sunMover,    //  太陽移動インスタンス
    ITimeProvider TimeProvider,    //  時間値プロバイダ
    ITimeConfig TimeConfig    //  日付のインスタンス
    )
    {
        this.sunStat = sunStat;
        this.sunMover = sunMover;
        this.TimeProvider = TimeProvider;
        this.TimeConfig = TimeConfig;
    }

    float SunAngleX;    //  太陽のアングル(X)
    public bool IsNight;    //  夜かどうか

    public bool IsNightProperty => IsNight;

    void Update()
    {
        if (TimeProvider.CurrentTimeProperty <= TimeConfig.DayTimeProperty)
        {
            IsNight = false;
            float DayTimeProgress = TimeProvider.CurrentTimeProperty / TimeConfig.DayTimeProperty; 
            SunAngleX = Mathf.Lerp(sunStat.DayTimeStartAngle, sunStat.DayTimeLastAngle, DayTimeProgress);
        }
        else if (TimeProvider.CurrentTimeProperty <= TimeConfig.OneDayTimeProperty)
        {
            IsNight = true;
            float NightTime = TimeProvider.CurrentTimeProperty - TimeConfig.DayTimeProperty;
            float t = NightTime / TimeConfig.NightTimeProperty;
            SunAngleX = Mathf.Lerp(sunStat.NightTimeStartAngle, sunStat.NightTimeLastAngle, t);
        }
        else
        {
            IsNight = false;
            TimeProvider.CurrentTimeProperty = 0f;
        }
        sunMover.ChangeAngle(SunAngleX);
    }
}