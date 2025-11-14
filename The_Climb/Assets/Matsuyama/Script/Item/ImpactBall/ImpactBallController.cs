using TheClimb.Core;
using TheClimb.Astral;
using TheClimb.UniversalGravity;

namespace TheClimb.Item
{
    public class InpactBallController : AttractableListenerBase    //  インパクトボールコントロールクラス
    {
        ItemStateFactory itemStateFactory;                             //  ItemのStateを生成する
        ItemStateMachine itemStateMachine;                             //  ItemのStateMachine
        GravitationTargetStatusBlock _gravitationTargetStatusBlock;    //  ターゲットのステータスブロック
        ImpactBallStatus _impactBallStatus;                  //  インパクトボールのステータスブロック
        ItemCommandProvider itemCommandProvider;

        ICorutineRunner _corutineRunner;    //  コルーチンランナー
        public IState currentState => itemStateMachine.CurrentState;

        public GravitationTargetStatusBlock statProperty => _gravitationTargetStatusBlock;    //  ステータスプロパティ

        private void OnEnable()
        {
            ItemEventBus.onAttractiong += itemCommandProvider.countTillActivate.Execute; 
        }

        private void OnDisable()
        {
            ItemEventBus.onAttractiong -= itemCommandProvider.countTillActivate.Execute;
        }

        public void Initialize(GravitationTargetStatusBlock gravitationStat, ImpactBallStatus ballStatus, ICorutineRunner corutineRunner)    //  初期化
        {
            _gravitationTargetStatusBlock = gravitationStat;
            _impactBallStatus = ballStatus;
            _corutineRunner = corutineRunner;
            itemCommandProvider = new ItemCommandProvider(_impactBallStatus.GetStatus(ItemMode.Normal), corutineRunner);
            itemStateFactory = new ItemStateFactory();
            itemStateMachine = new ItemStateMachine(itemStateFactory, itemCommandProvider);

            itemStateMachine.Initialize();    //  ステートマシーン初期化
        }

        public override void OnAttract()    //  引き寄せられた時の処理
        {
            itemStateMachine.ChangeState(itemStateFactory.CreateState(ItemStateID.Attracting));    //  状態変更
        }
    }
}