using UnityEngine;
using TheClimb.Astral;

namespace TheClimb.UniversalGravity
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class GravitationTargetMarker : AttractableBase     //  万有引力影響対象につけるコンポーネント
    {
        [SerializeField] GravitationTargetStatusBlock gravitationTargetStatusBlock;    //  万有引力操作対象のステータス
        public override GravitationTargetStatusBlock statProperty => gravitationTargetStatusBlock;    //  ステータスプロパティ
        public Rigidbody rigidBody { get; private set ;}   //  リジッドボディプロパティ

        public override GravitationTargetStateID currentStateIDProperty => curretStateID;
        IAttractableListener[] attractableListener;

        void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();

            attractableListener = GetComponents<IAttractableListener>();
        }
        void Initialize()    //  初期化
        {
            
        }
        void OnEnable()
        {
            GravitationObjectResistry.RegisterTarget(this, rigidBody);    //  レジストリーに登録
        }
        
        private void OnDisable()
        {
            GravitationObjectResistry.UnregisterTarget(this, rigidBody);    //  レジストリーから登録解除
        }

        public override void OnAttract()    //  引き寄せがスタートした瞬間の処理
        {
            base.OnAttract();

            foreach (var listener in attractableListener)
                listener?.OnAttract();
        }
    }
}