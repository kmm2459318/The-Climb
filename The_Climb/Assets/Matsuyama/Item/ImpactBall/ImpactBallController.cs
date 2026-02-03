using TheClimb.Core;
using TheClimb.Astral;
using UnityEngine;

namespace TheClimb.Item
{
    public class ImpactBallController : AttractableListenerBase    //  インパクトボールコントロールクラス
    {
        ItemStateFactory itemStateFactory;                             //  ItemのStateを生成する
        ItemStateMachine itemStateMachine;                             //  ItemのStateMachine
        ImpactBallContext _ctx;
        ItemCommandProvider itemCommandProvider;                       //  アイテムコマンドプロバイダープロバイダー
        ImpactBallRuntimeData _runtimeData;

        IItemStateContext itemStateContext;    //  Stateコンテキスト

        public IItemState currentState => itemStateMachine.CurrentState;    //  現在のステートを返す

        public override float RemainCount => _runtimeData._RemainingFuseTime;

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

        private void Update()
        {
            //Debug.Log();
        }
        public void Initialize(ImpactBallContext ctx, ICorutineRunner coroutineRunner)    //  初期化
        {
            IImpactable targetPlayer = ImpactableRegistry.GetPlayer();
            Debug.Log(targetPlayer);

            itemStateFactory = new ItemStateFactory();
            itemCommandProvider = new ItemCommandProvider(ctx, itemStateFactory, coroutineRunner, targetPlayer);
            itemStateMachine = new ItemStateMachine(itemStateFactory, itemCommandProvider, this.transform);

            itemStateContext = new ItemStateContext(itemStateMachine, this.transform, itemStateFactory);

            _runtimeData = ctx.RuntimeData;

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