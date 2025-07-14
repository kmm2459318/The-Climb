//  時間提供インターフェース
using System;

public interface ITimeProvider
{
    //  現在時間取得プロパティ
    float CurrentTimeProperty { get; set; }
    //  現在日数取得プロパティ
    int CurrentDayProperty { get; }
    //  夜かどうかのプロパティ
    bool IsNightProperty { get; set; }

    event Action<bool> OnChangedNight;
}
