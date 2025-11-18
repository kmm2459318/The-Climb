using TheClimb.Core;
using TheClimb.Player;
using UnityEngine;

namespace TheClimb.Item
{
    public class ItemCommandProvider    //  コマンドプロバイダー
    {
        ItemStateFactory _stateFactory;    //  ItemのStateFacotry

        ICommandContext _commandContext;    //  コマンドコンテキスト
        IPlayerDataProvider _playerDataProvider;    //  コマンドコンテキスト
        ICorutineRunner _coroutineRunner;    //  コマンドコンテキスト

        public CountTillActivate countTillActivate {get ;}    //  アクティブになるまでカウントする
        public ExplosionInpact explosionInpact{get ;}    //  アクティブになるまでカウントする

        public ItemCommandProvider(ImpactBallStatusBlock impactBallStat,Transform ImpactBallTF, IItemStateFactory stateFactory, ICorutineRunner coroutineRunner, IPlayerDataProvider playerDataProvider)    //  コンストラクタ
        {
            _playerDataProvider = playerDataProvider;

            Debug.Log(impactBallStat);
            countTillActivate = new CountTillActivate(impactBallStat, stateFactory, coroutineRunner);
            explosionInpact = new ExplosionInpact(impactBallStat, ImpactBallTF, stateFactory, coroutineRunner, playerDataProvider);
        }

        public void InjectContext(ICommandContext context)    //  コンテキスト依存注入
        {
            _commandContext = context;
            countTillActivate.InjectContext(context);
            explosionInpact.InjectContext(context);    
        }
    }
}