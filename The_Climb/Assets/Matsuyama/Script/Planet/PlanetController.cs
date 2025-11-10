using UnityEngine;
using TheClimb.Logging;
using TheClimb.Player;
using TheClimb.Core;

namespace TheClimb.Astral
{
    public class PlanetController : MonoBehaviour    //  天体を包括的にコントロールする
    {
        [SerializeField] PlanetStatus planetStatus;           //  天体のステータス
        GravitationStatusBlock currentGravitationStat;        //  天体の万有引力ステータスブロック
        OrbitalStatusBlock currentOrbitalStat;                //  天体の円軌道追従ステータスブロック
        PlanetStateMachine planetStateMachine;                //  天体のステートマシーン
        PlanetStateFactory planetStateFactory;                //  天体ステートファクトリー
        PlanetCommandProvider planetCommandProvider;          //  天体関数提供クラス
        OrbitalContext orbitalContext;                        //  天体円軌道コンテキスト
        VectorToPlanetCalculator planetToPlanetCalculator;    //  天体へのベクトル計算クラス

        void Awake()
        {
            PlanetContext.Instance.RegistPlanetController(this);
            currentGravitationStat = planetStatus.GetGraviatationStatus(PlanetIDs.Earth);
            currentOrbitalStat = planetStatus.GetOrbitalStatus(PlanetIDs.Earth);
        }

        private void Start()
        {
            planetStateMachine.ChangeState(planetStateFactory.CreateIdleState());
            LogUtility.Log(LogPrefix.uiFactory,"fuckyou", LogLevel.Warning);
        }
        public void Initialize(IPlanetDataProvider planetDataProvider, IPlayerDataProvider playerDataProvider, ICorutineRunner runner)    //  初期化
        {
            planetStateMachine = new PlanetStateMachine();
            orbitalContext = new OrbitalContext(this.transform, currentOrbitalStat, playerDataProvider.TransformProperty, runner);
            planetCommandProvider = new PlanetCommandProvider(this.transform, playerDataProvider.TransformProperty, currentGravitationStat, currentOrbitalStat, orbitalContext);
            planetCommandProvider.orbitalFollower.Initialize();
            planetStateFactory = new PlanetStateFactory(this, planetStateMachine, planetCommandProvider);
            PlanetEventBus.ActivePlanet(playerDataProvider.TransformProperty, currentOrbitalStat.OrbitRadius, currentOrbitalStat.OrbitalSamples);    //  半円を表示
        }

        void Update()
        {
            planetStateMachine.Update();
        }
    }
}