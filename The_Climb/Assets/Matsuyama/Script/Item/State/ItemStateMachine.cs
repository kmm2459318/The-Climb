using TheClimb.Core;

namespace TheClimb.Item
{
    public class ItemStateMachine : ItemSMBase    //  アイテムStateMachine
    {
        ItemStateFactory _itemStateFactory;

        public ItemStateMachine(ItemStateFactory stateFactory)    //  コンストラクタ
        {
            _itemStateFactory = stateFactory;
        }
        public override void Initialize()    //  初期化
        {
            ChangeState(_itemStateFactory.CreateState(ItemStateID.Idle));    //  状態変更
        }

        public override void Update()        //  CurrentStateの常時処理を回す
        {
            base.Update();
        }
        public override void ChangeState(IState nextState)
        {
            base.ChangeState(nextState);
        }
    }
}