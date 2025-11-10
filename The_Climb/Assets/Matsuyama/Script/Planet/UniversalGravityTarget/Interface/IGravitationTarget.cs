namespace TheClimb.UniversalGravity
{
    public interface IGravitationStatus    //  万有引力影響対象につけるインターフェース
    {
        GravitationTargetStatusBlock statusBlockGetter { get; }    //  ステータスゲッター
    }
}