using UnityEngine;

namespace TheClimb.UniversalGravity
{
    [DisallowMultipleComponent]
    public class GravitationTargetStatusHolder : MonoBehaviour, IGravitationStatus    //  ステータスのスクリプタブルオブジェクトを保持
    {
        [SerializeField] GravitationTargetStatusBlock gravitationTargetStatusBlock;    //  万有引力操作対象のステータス

        public GravitationTargetStatusBlock statusBlockGetter => gravitationTargetStatusBlock;    //  ステータスゲッター
    }
}