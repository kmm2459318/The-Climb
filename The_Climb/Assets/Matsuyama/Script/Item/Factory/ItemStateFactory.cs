using TheClimb.Core;

namespace TheClimb.Item
{
    public class ItemStateFactory : ItemStateFactoryBase    //  アイテムstate
    {
        public ItemStateFactory()    //  コンストラクタ
        {
            Register(ItemStateID.Idle, () => new InpactBallIdleState());
        }

        public override IState CreateState(ItemStateID stateID)    //  state生成
        {
            return base.CreateState(stateID);
        }
    }
}