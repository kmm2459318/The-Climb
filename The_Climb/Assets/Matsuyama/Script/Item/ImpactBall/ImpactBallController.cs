using TheClimb.Core;
using TheClimb.Astral;
using TheClimb.UniversalGravity;
using TheClimb.Player;

namespace TheClimb.Item
{
    public class InpactBallController : AttractableListenerBase    //  インパクトボールコントロールクラス
    {
        ItemStateFactory itemStateFactory;                             //  ItemのStateを生成する
        ItemStateMachine itemStateMachine;                             //  ItemのStateMachine
        GravitationTargetStatusBlock _gravitationTargetStatusBlock;    //  ターゲットのステータスブロック
        ImpactBallStatus _impactBallStatus;                  //  インパクトボールのステータスブロック
        ItemCommandProvider itemCommandProvider;    //  アイテムコマンドプロバイダープロバイダー

        ICorutineRunner _corutineRunner;    //  コルーチンランナー
        IPlayerDataProvider playerDataProvider;    //  プレイヤー情報提供者
        public IState currentState => itemStateMachine.CurrentState;    //  現在のステートを返す

        public GravitationTargetStatusBlock statProperty => _gravitationTargetStatusBlock;    //  ステータスプロパティ

        private void OnEnable()
        {
            ItemEventBus.onAttractiong += itemCommandProvider.countTillActivate.Execute;
            ItemEventBus.onExplosion += itemCommandProvider.explosionInpact.Execute;
        }

        private void OnDisable()
        {
            ItemEventBus.onAttractiong -= itemCommandProvider.countTillActivate.Execute;
            ItemEventBus.onExplosion += itemCommandProvider.explosionInpact.Execute;
        }

        public void Initialize(GravitationTargetStatusBlock gravitationStat, ImpactBallStatus ballStatus, ICorutineRunner corutineRunner, IPlayerDataProvider playerDataProvider)    //  初期化
        {
            _gravitationTargetStatusBlock = gravitationStat;
            _impactBallStatus = ballStatus;
            _corutineRunner = corutineRunner;
            itemStateFactory = new ItemStateFactory();
            itemCommandProvider = new ItemCommandProvider(_impactBallStatus.GetStatus(ItemMode.Normal),this.transform, itemStateFactory, corutineRunner, playerDataProvider);
            itemStateMachine = new ItemStateMachine(itemStateFactory, itemCommandProvider);

            itemCommandProvider.InjectContext(itemStateMachine);
            itemStateMachine.Initialize();    //  ステートマシーン初期化
        }

        public override void OnAttract()    //  引き寄せられた時の処理
        {
            itemStateMachine.ChangeState(itemStateFactory.CreateState(ItemStateID.Attracting));    //  状態変更
        }
    }
}