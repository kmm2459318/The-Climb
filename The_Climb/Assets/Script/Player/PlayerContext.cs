using TheClimb.Astral;
using UnityEngine;

namespace TheClimb.Player
{
    [DefaultExecutionOrder(-100)]
    public class PlayerContext : MonoBehaviour    //  プレイヤーコンテキスト
    {
        [SerializeField] Transform PlayerTransform;
        public static PlayerContext Instance { get; private set; }              //  プロパティ
        public PlayerController PlayerController { get; private set; }          //  プロパティ
        
        public IPlayerDataProvider _PlayerDataProvider { get; private set; }    //  プロパティ

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _PlayerDataProvider = new PlayerDataProvider(PlayerTransform);
        }

        public void RegistController(PlayerController playerController)    //  コントローラー登録メソッド
        {
            PlayerController = playerController;
        }
    }
}