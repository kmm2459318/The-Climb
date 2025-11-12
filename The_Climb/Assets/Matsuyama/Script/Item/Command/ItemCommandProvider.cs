using TheClimb.Core;

namespace TheClimb.Item
{
    public class ItemCommandProvider    //  コマンドプロバイダー
    {
        public CountTillActivate countTillActivate {get ;}    //  アクティブになるまでカウントする

        public ItemCommandProvider(ImpactBallStatusBlock impactBallStat, ICorutineRunner runner)    //  コンストラクタ
        {
            countTillActivate = new CountTillActivate(impactBallStat, runner);
        }
    }
}