//  敵キャラのステート管理
public class EnemyStateMachine
{
    IEnemyState CurrentEnemyState;    //  敵キャラの現在のステート

    //  状態変更関数
    public void ChangeState(IEnemyState newState)
    {
        CurrentEnemyState?.Exit();
        CurrentEnemyState = newState;
        CurrentEnemyState?.Enter();
    }
    //  ステートごとの状態実行
    public void FixedUpdate()
    {
        CurrentEnemyState?.FixedUpdate();
    }
}
