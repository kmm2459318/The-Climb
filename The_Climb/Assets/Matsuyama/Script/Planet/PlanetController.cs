using UnityEngine;
using TheClimb.Logging;
using TheClimb.Player;

namespace TheClimb.Astral
{
    public class PlanetController : MonoBehaviour    //  天体を包括的にコントロールする
    {
        PlanetMover planetMover;                              //  天体の移動処理持ち
        [SerializeField] PlanetStatus planetStatus;           //  天体のステータス
        PlanetStateMachine planetStaeMachine;                 //  天体のステートマシーン
        PlanetStateFactory planetStateFactory;                //  天体ステートファクトリー
        PlanetCommandProvider planetCommandProvider;          //  天体関数提供クラス
        VectorToPlanetCalculator planetToPlanetCalculator;    //  天体へのベクトル計算クラス

        void Awake()
        {
            PlanetContext.Instance.RegistPlanetController(this);
        }

        public void Initialize(IPlanetDataProvider planetDataProvider, IPlayerDataProvider playerDataProvider)    //  初期化
        {
            
            planetMover = new PlanetMover();
            planetStaeMachine = new PlanetStateMachine();
            planetCommandProvider = new PlanetCommandProvider(planetMover, this.transform, planetStatus.GetStats(PlanetIDs.Earth));
            planetStateFactory = new PlanetStateFactory(this, planetStaeMachine, planetCommandProvider);
        }

        private void Start()
        {
            planetStaeMachine.ChangeState(planetStateFactory.CreateIdleState());
            LogUtility.Log(LogPrefix.uiFactory,"fuckyou", LogLevel.Warning);
        }

        void Update()
        {
            planetStaeMachine.Update();
        }
    }
}