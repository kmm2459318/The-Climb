using UnityEngine;

namespace TheClimb.Core
{
    [DefaultExecutionOrder(-100)]
    public class CoroutineRunnerContext : MonoBehaviour    //  天体コンテキスト
    {
        public static CoroutineRunnerContext Instance { get; private set; }    //  インスタンス
        public ICorutineRunner _corutineRunner { get; private set; }    //  コルーチンランナー

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void RegistCoroutineRunner(ICorutineRunner corutineRunner)    //  登録メソッド
        {
            _corutineRunner = corutineRunner;
        }
    }
}