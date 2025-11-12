using System;
using System.Collections.Generic;
using TheClimb.Core;

namespace TheClimb.Item
{
    public class ItemStateFactoryBase : IItemStateFactory    //  ItemState生成クラス
    {
        protected readonly Dictionary<ItemStateID, Func<IState>> itemStateRegistry = new();    //  State登録用辞書

        public virtual IState CreateState(ItemStateID state)    //  Stateを生成
        {
            if (itemStateRegistry.TryGetValue(state, out Func<IState> creator))
            { return creator(); }

            throw new Exception("_state is not registed");
        }

        protected void Register(ItemStateID stateID, Func<IState> ceator)    //  生成関数登録
        {
            itemStateRegistry[stateID] = ceator;
        }
    }
}