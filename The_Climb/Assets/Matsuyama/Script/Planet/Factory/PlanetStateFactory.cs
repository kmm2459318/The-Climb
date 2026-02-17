namespace TheClimb.Astral
{
    public class PlanetStateFactory    //  天体のStateを生成するファクトリー
    {
        readonly PlanetController controller;
        readonly PlanetStateMachine stateMachine;
        readonly PlanetCommandProvider commandProvider;

        public PlanetStateFactory(PlanetController controller, PlanetStateMachine sm, PlanetCommandProvider cmdProvider)    //  Controllerから呼ばれる
        {
            this.controller = controller;
            stateMachine = sm;
            commandProvider = cmdProvider;
        }

        public IPlanetState CreateIdleState()    //  IdleState生成
        {
            return new IdleState(commandProvider);
        }

        //  ジャンプ状態生成    現在は未使用
        //public IEnemyState CreateJumpState()
        //{
        //    return new JumpState(_kickerMoveCommander, _enemyStateMachine, this);
        //}
    }
}