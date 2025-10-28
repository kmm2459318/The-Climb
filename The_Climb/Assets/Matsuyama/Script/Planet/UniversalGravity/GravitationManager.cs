using TheClimb.Astral;
using UnityEngine;

namespace TheClimb.UniversalGravity
{
    public class PlanetManager : MonoBehaviour    //  
    {
        public static PlanetManager Instance { get; private set; }    //  シングルトンインスタンス提供
        [SerializeField] PlanetTransformProvider planetTransformProvider;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        void Start()
        {

        }
        void Update()
        {

        }
        void InitializePlanet()
        {
            
        }
    }
}