using UnityEngine;

//  �G���̏�Ԃ𐶐�����t�@�N�g���[
public class EnemyStateFactory : IEnemyStateFactory
{
    readonly KickerMoveCommander _kickerMoveCommander;
    readonly EnemyStateMachine _enemyStateMachine;

    public EnemyStateFactory(KickerMoveCommander kickerMoveCommmander, EnemyStateMachine enemyStateMachine)
    {
        _kickerMoveCommander = kickerMoveCommmander;
        _enemyStateMachine = enemyStateMachine;
    }
    //  �ړ���Ԑ���
    public IEnemyState CreateWalkState()
    {
        return new WalkState(_kickerMoveCommander, _enemyStateMachine, this);
    }
    //  �W�����v��Ԑ���
    public IEnemyState CreateJumpState()
    {
        return new JumpState(_kickerMoveCommander, _enemyStateMachine, this);
    }
}
