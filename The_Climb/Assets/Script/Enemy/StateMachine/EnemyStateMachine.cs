//  �G�L�����̃X�e�[�g�Ǘ�
public class EnemyStateMachine
{
    IEnemyState CurrentEnemyState;    //  �G�L�����̌��݂̃X�e�[�g

    //  ��ԕύX�֐�
    public void ChangeState(IEnemyState newState)
    {
        CurrentEnemyState?.Exit();
        CurrentEnemyState = newState;
        CurrentEnemyState?.Enter();
    }
    //  �X�e�[�g���Ƃ̏�Ԏ��s
    public void FixedUpdate()
    {
        CurrentEnemyState?.FixedUpdate();
    }
}
