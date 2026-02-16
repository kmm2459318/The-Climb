using TheClimb.UniversalGravity;
using UnityEngine;

namespace TheClimb.Astral
{
    public abstract class AttractableBase : MonoBehaviour, IAttractable    //  引き寄せ用マーカーコンポーネントの基底クラス
    {
        protected GravitationTargetStateID curretStateID; 

        public abstract GravitationTargetStatusBlock statProperty { get; }    //  ステータス取得
        public abstract GravitationTargetStateID currentStateIDProperty { get; }   //  現在状態State取得
        public virtual void OnAttract()    //  引き寄せがスタートした瞬間の処理
        {
            curretStateID = GravitationTargetStateID.Attracting;
        }
    }
}