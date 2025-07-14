using UnityEngine;
using Zenject;

//  日付・時間管理スクリプト
public class TimeManager : MonoBehaviour, ITimeProvider
{
    [Inject] ITimeConfig TimeConfig;    //  時間設定

    public float CurrentTime;    //  現在の時間
    float TimeProgressValue;    //  １秒当たりの時間進行値
    int CurrentDay;    //  現在の日付
    [SerializeField] bool IsNight;

    //  現在時間取得プロパティ
    public float CurrentTimeProperty
    {
        get => CurrentTime;
        set => CurrentTime = value;
    }
    //  現在日数取得プロパティ
    public int CurrentDayProperty => CurrentDay;
    //  夜取得プロパティ
    public bool IsNightProperty
    {
        get => IsNight;
        set => IsNight = value;
    }
    void Awake()
    {
        CurrentTime = TimeConfig.InitializeTimeProperty;
        TimeProgressValue = TimeConfig.ProgressTimeProperty;
        CurrentDay = TimeConfig.InitializeDateProperty;
    }
    void Update()
    {
        CurrentTime += TimeProgressValue * Time.deltaTime;
    }
    //  初期化関数
    void Initialize()
    {

    }
}
