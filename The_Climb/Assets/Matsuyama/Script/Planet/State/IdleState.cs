using TheClimb.Logging;

namespace TheClimb.Astral
{
    public class IdleState : IPlanetState    //  Idle状態
    {
        PlanetController _planetController;        //  プラネットコントローラーインスタンス
        PlanetStateMachine _planetStateMachine;    //  プラネットステートマシーンインスタンス
        PlanetCommandProvider _CommandProvider;    //  コマンドプロバイダ

        public IdleState(PlanetController planetController, PlanetStateMachine planetStateMachine, PlanetCommandProvider commandProvider)    //  コンストラクタ
        {
            _planetController = planetController;
            _planetStateMachine = planetStateMachine;
            _CommandProvider = commandProvider;
        }
        
        public void Enter()    //  Idle状態突入時の関数
        {
            LogUtility.Log(LogPrefix.idleState, "IdleState突入", LogLevel.VerBose);
            _CommandProvider.followOrbital.Execute();
        }
        public void Update()    //  Idle状態中の関数
        {
            _CommandProvider.rotationPlanet.Execute();
        }
        public void Exit()    //  Idle状態を抜ける時の関数
        {

        }
    }
}