using TheClimb.UniversalGravity;

namespace TheClimb.Astral
{
    public interface IAttractable    //  引き寄せ可能なオブジェクトにつけるInterface
    {
        GravitationTargetStatusBlock statProperty { get; }    //  ターゲットステータスブロック
        GravitationTargetStateID currentStateIDProperty { get; }    //  ターゲットステータスブロック

        void OnAttract();    //  引き寄せがスタートした瞬間の処理
    }
}