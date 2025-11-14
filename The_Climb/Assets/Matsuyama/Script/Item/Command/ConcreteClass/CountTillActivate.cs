using System.Collections;
using TheClimb.Core;
using TheClimb.Logging;
using UnityEngine;

namespace TheClimb.Item
{
    public class CountTillActivate : ItemCommandBase   //  爆発するまでカウントする
    {
        ImpactBallStatusBlock _impactBallStat;    //  衝撃球のステータス
        ICorutineRunner _coroutineRunner;    //  コルーチンランナー

        public CountTillActivate(ImpactBallStatusBlock stat, ICorutineRunner runner)    //  コンストラクタ
        {
            _impactBallStat = stat;
            _coroutineRunner = runner;
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
            Debug.Log(123123123);
        }
    }
}