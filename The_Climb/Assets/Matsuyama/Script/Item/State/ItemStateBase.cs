using TheClimb.Core;

namespace TheClimb.Item
{
    public abstract class ItemStateBase : IState    //  アイテムStatePatternのBaseClass
    {
        public ItemStateBase()    //  コンストラクタ
        { }

        public virtual void OnEnter() { }          //  State突入時の処理
        public virtual void OnUpdate() { }         //  State中の処理(Update)
        public virtual void OnFixedUpdate() { }    //  State中の処理(FixedUpdate)
        public virtual void OnExit() { }           //  State退場時の処理
    }
}