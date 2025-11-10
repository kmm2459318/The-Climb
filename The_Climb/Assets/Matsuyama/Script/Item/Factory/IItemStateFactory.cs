using TheClimb.Core;

namespace TheClimb.Item
{
    public interface IItemStateFacroy    //  アイテムインターフェース
    {
        IState CreateState(ItemStateID stateID);    //  State生成
    }
}