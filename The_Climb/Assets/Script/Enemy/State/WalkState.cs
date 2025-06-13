using System;
using UnityEngine;

//    �G�L�������s���
public class WalkState : IEnemyState
{
    KickerMoveCommander _kickerMoveCommander;    //  KickerMoveCommander�̃C���X�^���X
    EnemyStateMachine _enemyStateMachine;    //  EnemyStateMachine�̃C���X�^���X
    System.Action JumpTimeListener;    //  �W�����v���Ԃ̔��΂�҂ϐ�

    //  �R���X�g���N�^
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
        //  �ړ�
        _kickerMoveCommander.CommanderMethodMap[KickerMoveCommander.KickerCommanderMethod.MOVE].Execute();
        //  �[������
        if ((bool)_kickerMoveCommander.CommanderMethodMap[KickerMoveCommander.KickerCommanderMethod.IS_EDGE_POS].Execute())
        {
            //  �ړ������ω�
            _kickerMoveCommander.CommanderMethodMap[KickerMoveCommander.KickerCommanderMethod.FLIP_MOVE_DIR].Execute();
        }
    }
    public void Exit()
    {
        _kickerMoveCommander.OnJumpTime -= JumpTimeListener;
    }
}
