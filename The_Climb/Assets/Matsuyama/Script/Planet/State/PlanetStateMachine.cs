using System;

namespace TheClimb.Astral
{
    //  敵キャラのステート管理
    public class PlanetStateMachine
    {
        public event Action<IPlanetState> OnStateChanged;    //  状態が変化したとき

        IPlanetState CurrentEnemyState;    //  敵キャラの現在のステート

        //  現在の状態を返すプロパティ
        public IPlanetState CurrentStateProperty => CurrentEnemyState;

        //  状態変更関数
        public void ChangeState(IPlanetState newState)
        {
            CurrentEnemyState?.Exit();
            CurrentEnemyState = newState;
            CurrentEnemyState?.Enter();

            OnStateChanged?.Invoke(CurrentEnemyState);
        }

        //  ステートごとの状態実行
        public void Update()
        {
            CurrentEnemyState?.Update();
        }
    }
}