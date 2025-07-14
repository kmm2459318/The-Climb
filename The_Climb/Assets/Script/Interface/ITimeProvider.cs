//  時間提供インターフェース
public interface ITimeProvider
{
    //  現在時間取得プロパティ
    float CurrentTimeProperty { get; set; }
    //  現在日数取得プロパティ
    int CurrentDayProperty { get; }
}
