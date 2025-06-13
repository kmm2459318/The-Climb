using UnityEngine;

//  �G�L�����ړ����
public class JumpState : IEnemyState
{
    KickerMoveCommander _kickerMoveCommander;    //  KickerMoveCommander�̃C���X�^���X
    EnemyStateMachine _enemyStateMachine;    //  EnemyStateMachine�̃C���X�^���X
    System.Action LamdGroundListener;    //  �n�ʒ��n�������̔��΂�҂ϐ�

    //  �R���X�g���N�^
    public JumpState(KickerMoveCommander kickerMoveCommander, EnemyStateMachine enemyStateMachine, IEnemyStateFactory enemyStateFactory)
    {
        _kickerMoveCommander = kickerMoveCommander;
        _enemyStateMachine = enemyStateMachine;
        LamdGroundListener += () =>
        {
            _enemyStateMachine.ChangeState(enemyStateFactory.CreateWalkState());
        };
    }
    public void Enter()
    {
        _kickerMoveCommander.OnLandGround += LamdGroundListener;
        _kickerMoveCommander.CommanderMethodMap[KickerMoveCommander.KickerCommanderMethod.JUMP].Execute();
    }
    public void FixedUpdate()
    {
        //  �ړ�
        _kickerMoveCommander.CommanderMethodMap[KickerMoveCommander.KickerCommanderMethod.MOVE].Execute();
    }
    public void Exit()
    {
        _kickerMoveCommander.OnLandGround -= LamdGroundListener;
    }
}
