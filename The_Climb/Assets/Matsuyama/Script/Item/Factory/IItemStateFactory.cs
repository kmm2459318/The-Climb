using TheClimb.Core;

namespace TheClimb.Item
{
    public interface IItemStateFactory    //  アイテムインターフェース
    {
        IState CreateState(ItemStateID stateID);    //  State生成
    }
}