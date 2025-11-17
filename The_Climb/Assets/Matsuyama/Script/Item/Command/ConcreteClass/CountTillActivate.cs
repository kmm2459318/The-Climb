using System.Collections;
using TheClimb.Core;
using TheClimb.Logging;
using UnityEngine;

namespace TheClimb.Item
{
    public class CountTillActivate : ItemCommandBase   //  爆発するまでカウントする
    {
        ImpactBallStatusBlock _impactBallStat;    //  衝撃球のステータス

        IItemStateFactory _itemStateFactory;    //  ItemのStateFacotry
        ICommandContext _commandContext;    //  コマンドコンテキスト
        ICorutineRunner _coroutineRunner;    //  コルーチンランナー

        public CountTillActivate(ImpactBallStatusBlock stat, IItemStateFactory stateFactory, ICorutineRunner runner)    //  コンストラクタ
        {
            _impactBallStat = stat;

            _itemStateFactory = stateFactory;
            _coroutineRunner = runner;
        }

        public void InjectContext(ICommandContext commandContext)    //  コンテキスト注入
        {
            _commandContext = commandContext;
        }

        public override void Execute()    //  カウント開始
        {
            LogUtility.Log(LogPrefix.countTillActivate, "爆発タイマーカウント開始", LogLevel.Debug);
            _coroutineRunner.StartCoroutine(CountTillExplosion());
        }

        IEnumerator CountTillExplosion()    //  爆発するまでカウントするコルーチン
        {
            float ElapsedTime = 0;    //  経過時間

            while (ElapsedTime < _impactBallStat.ExplosionCount)
            {
                ElapsedTime += Time.deltaTime;
                yield return null;
            }

            _commandContext.ChangeState(_itemStateFactory.CreateState(ItemStateID.Expolosing));    //  爆発stateに変更
        }
    }
}