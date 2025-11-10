using TheClimb.Core;

namespace TheClimb.Item
{
    public class ItemSMBase : IStateMachine    //  アイテムStateMachineBaseクラス
    {
        protected IState currentState;    //  現在の状態

        public virtual void Initialize() { }                 //  初期化
        public virtual void Update() { }                     //  State中の処理(Update)
        public virtual void FixedUpdate() { }                //  State中の処理(FixedUpdate)
        public virtual void ChangeState(IState nextState)    //  状態変更
        {
            currentState?.OnExit();
            currentState = nextState;
            currentState?.OnEnter();
        }
    }
}