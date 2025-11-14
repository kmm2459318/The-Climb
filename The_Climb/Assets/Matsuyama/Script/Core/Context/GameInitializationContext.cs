using UnityEngine;
using TheClimb.UniversalGravity;
using TheClimb.Item;


namespace TheClimb.Core
{
    public class GameInitializationContext : MonoBehaviour    //  全体的な初期化管理者
    {
        [SerializeField] InpactBallController inpactBallController;    //  インパクトボールコントローラー
        [SerializeField] GravitationTargetStatusBlock _gravitationTargetStatusBlock;    //  万有引力操作ターゲットステータスブロック
        [SerializeField] ImpactBallStatus _imapctBallStatsus;    //  衝撃球ステータスブロック
        [SerializeField] CoroutineRunner coroutineRunner;    //  コルーチンランナー
        private void Awake()
        {
            inpactBallController.Initialize(_gravitationTargetStatusBlock, _imapctBallStatsus, coroutineRunner);    //  衝撃球コントローラー初期化
        }
    }
}