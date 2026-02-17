using UnityEngine;
using TheClimb.UniversalGravity;
using TheClimb.Item;

namespace TheClimb.Core
{
    [DefaultExecutionOrder(-50)]
    public class GameInitializationContext : MonoBehaviour    //  全体的な初期化管理者
    {
        [SerializeField] GravitationTargetStatusBlock _gravitationTargetStatusBlock;    //  万有引力操作ターゲットステータスブロック
        [SerializeField] ImpactBallStatus _imapctBallStatsus;    //  衝撃球ステータスブロック
        [SerializeField] CoroutineRunner coroutineRunner;    //  コルーチンランナー
    }
}