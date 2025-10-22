using UnityEngine;

namespace TheClimb.Astral
{
    [DefaultExecutionOrder(-100)]
    public class PlanetContext : MonoBehaviour    //  天体コンテキスト
    {
        [SerializeField] Transform PlanetTransform;

        public static PlanetContext Instance { get; private set; }
        public PlanetController _PlanetController { get; private set; }
        
        public IPlanetDataProvider _PlanetDataProvider { get; private set; }

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _PlanetDataProvider = new PlanetDataProvider(PlanetTransform);
        }

        public void RegistPlanetController(PlanetController planetController)    //  登録メソッド
        {
            _PlanetController = planetController;
        }
    }
}