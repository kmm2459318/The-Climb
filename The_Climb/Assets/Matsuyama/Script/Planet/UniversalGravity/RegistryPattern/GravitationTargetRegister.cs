using UnityEngine;

namespace TheClimb.UniversalGravity
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody), typeof(GravitationTargetStatusHolder))]
    public class GravitationTargetRegister : MonoBehaviour    //  万有引力影響対象につけるコンポーネント
    {
        public Rigidbody rigidBody { get; private set ;}
        void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
        }
        void OnEnable()
        {
            GravitationObjectResistry.RegisterTarget(this, rigidBody);    //  レジストリーに登録
        }
        
        private void OnDisable()
        {
            GravitationObjectResistry.UnregisterTarget(this, rigidBody);    //  レジストリーから登録解除
        }
    }
}