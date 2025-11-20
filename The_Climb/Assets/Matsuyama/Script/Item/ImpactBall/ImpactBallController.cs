using TheClimb.Core;
using TheClimb.Astral;
using TheClimb.UniversalGravity;
using TheClimb.Player;
using TheClimb.Logging;

namespace TheClimb.Item
{
    public class InpactBallController : AttractableListenerBase    //  インパクトボールコントロールクラス
    {
        ItemStateFactory itemStateFactory;                             //  ItemのStateを生成する
        ItemStateMachine itemStateMachine;                             //  ItemのStateMachine
        GravitationTargetStatusBlock _gravitationTargetStatusBlock;    //  ターゲットのステータスブロック
        ImpactBallStatus _impactBallStatus;                            //  インパクトボールのステータスブロック
        ItemCommandProvider itemCommandProvider;                       //  アイテムコマンドプロバイダープロバイダー

        IItemStateContext itemStateContext;    //  Stateコンテキスト

        public GravitationTargetStatusBlock statProperty => _gravitationTargetStatusBlock;    //  ステータスプロパティ
        public IItemState currentState => itemStateMachine.CurrentState;    //  現在のステートを返す

        private void OnEnable()
        {
            ItemEventBus.onAttractiong += HandleCountTillActivate;
            ItemEventBus.onExplosion += HandleExplosionInpact;
        }

        private void OnDisable()
        {
            ItemEventBus.onAttractiong -= HandleCountTillActivate;
            ItemEventBus.onExplosion -= HandleExplosionInpact;
        }

        public void Initialize(GravitationTargetStatusBlock gravitationStat, ImpactBallStatus ballStatus, ICorutineRunner corutineRunner, IPlayerDataProvider playerDataProvider)    //  初期化
        {
            _gravitationTargetStatusBlock = gravitationStat;
            _impactBallStatus = ballStatus;
            itemStateFactory = new ItemStateFactory();
            itemCommandProvider = new ItemCommandProvider(_impactBallStatus.GetStatus(ItemMode.Normal),this.transform, itemStateFactory, corutineRunner, playerDataProvider);
            itemStateMachine = new ItemStateMachine(itemStateFactory, itemCommandProvider, this.transform);

            itemStateContext = new ItemStateContext(itemStateMachine, this.transform, itemStateFactory);

            itemCommandProvider.InjectContext(itemStateMachine, itemStateContext);
            itemStateMachine.Initialize();    //  ステートマシーン初期化
        }

        public override void OnAttract()    //  引き寄せられた時の処理
        {
            itemStateMachine.ChangeState(itemStateFactory.CreateState(ItemStateID.Attracting), itemStateContext);    //  状態変更
        }

        void HandleCountTillActivate(AttractEventArg attractEventArg)    //  CountTillActivateの関数をHandleする
        {
            if (attractEventArg.targeTransform != this.transform) return;

            itemCommandProvider.countTillActivate.Execute();
        }

        void HandleExplosionInpact(AttractEventArg attractEventArg)    //  爆発制御メソッド
        {
            if (attractEventArg.targeTransform != this.transform) return;

            itemCommandProvider.explosionInpact.Execute();
        }
    }
}