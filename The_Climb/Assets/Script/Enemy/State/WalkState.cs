using System;
using UnityEngine;

//    敵キャラ歩行状態
public class WalkState : IEnemyState
{
    KickerMoveCommander _kickerMoveCommander;    //  KickerMoveCommanderのインスタンス
    EnemyStateMachine _enemyStateMachine;    //  EnemyStateMachineのインスタンス
    System.Action JumpTimeListener;    //  ジャンプ時間の発火を待つ変数

    //  コンストラクタ
    public WalkState(KickerMoveCommander kickerMoveCommander, EnemyStateMachine enemyStateMachine, IEnemyStateFactory enemyStateFactory)
    {
        _kickerMoveCommander = kickerMoveCommander;
        _enemyStateMachine = enemyStateMachine;
        JumpTimeListener += () =>
        {
            _enemyStateMachine.ChangeState(enemyStateFactory.CreateJumpState());
        };
    }
    public void Enter()
    {
        _kickerMoveCommander.OnJumpTime += JumpTimeListener;
    }
    public void FixedUpdate()
    {
        //  移動
        _kickerMoveCommander.CommanderMethodMap[KickerMoveCommander.KickerCommanderMethod.MOVE].Execute();
        //  端か判定
        if ((bool)_kickerMoveCommander.CommanderMethodMap[KickerMoveCommander.KickerCommanderMethod.IS_EDGE_POS].Execute())
        {
            //  移動方向変化
            _kickerMoveCommander.CommanderMethodMap[KickerMoveCommander.KickerCommanderMethod.FLIP_MOVE_DIR].Execute();
        }
    }
    public void Exit()
    {
        _kickerMoveCommander.OnJumpTime -= JumpTimeListener;
    }
}
