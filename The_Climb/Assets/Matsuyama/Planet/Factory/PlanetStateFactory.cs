namespace TheClimb.Astral
{
    public class PlanetStateFactory    //  天体のStateを生成するファクトリー
    {
        readonly PlanetController _planetStateController;    //  天体コントローラーインスタンス
        readonly PlanetStateMachine _planetStateMachine;     //  天体ステートマシーンインスタンス
        readonly PlanetCommandProvider _commandProvider;     // 天体のコマンドプロバイダ

        public PlanetStateFactory(PlanetController planetController, PlanetStateMachine planetStateMachine, PlanetCommandProvider commandProvider)    //  コンストラクタ
        {
            _planetStateController = planetController;
            _planetStateMachine = planetStateMachine;
            _commandProvider = commandProvider;
        }
        public IPlanetState CreateIdleState()    //  IdleState生成
        {
            return new IdleState(_planetStateController, _planetStateMachine, _commandProvider);
        }
        //  ジャンプ状態生成
        //public IEnemyState CreateJumpState()
        //{
        //    return new JumpState(_kickerMoveCommander, _enemyStateMachine, this);
        //}
    }
}