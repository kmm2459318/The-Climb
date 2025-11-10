using TheClimb.UniversalGravity;

namespace TheClimb.Astral
{
    public interface IAtractable    //  引き寄せ可能なオブジェクトにつけるInterface
    {
        GravitationTargetStatusBlock statProperty { get; }    //  ターゲットステータスブロック

        void OnAttracting();    //  引き寄せられてる時の処理
    }
}