using UnityEngine;
using TheClimb.UniversalGravity;
using TheClimb.Item;
using TheClimb.Player;

namespace TheClimb.Core
{
    [DefaultExecutionOrder(-50)]
    public class GameInitializationContext : MonoBehaviour    //  全体的な初期化管理者
    {
        [SerializeField] InpactBallController[] inpactBallController;    //  インパクトボールコントローラー
        [SerializeField] GravitationTargetStatusBlock _gravitationTargetStatusBlock;    //  万有引力操作ターゲットステータスブロック
        [SerializeField] ImpactBallStatus _imapctBallStatsus;    //  衝撃球ステータスブロック
        [SerializeField] CoroutineRunner coroutineRunner;    //  コルーチンランナー
        private void Awake()
        {
            for (int idx = 0; idx < inpactBallController.Length; idx++)
            {
                inpactBallController[idx].Initialize(_gravitationTargetStatusBlock, _imapctBallStatsus, coroutineRunner, PlayerContext.Instance._PlayerDataProvider);
            }
        }
    }
}