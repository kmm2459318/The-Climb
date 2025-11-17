using TheClimb.Core;

namespace TheClimb.Item
{
    public interface ICommandContext    //  コマンドコンテキスト
    {
        void ChangeState(IState stateID);    //  状態変更
    }
}