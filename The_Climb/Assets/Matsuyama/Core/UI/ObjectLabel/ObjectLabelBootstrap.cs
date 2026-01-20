using UnityEngine;

namespace TheClimb.Core
{
    public class ObjectLabelBootstrap : MonoBehaviour    //  オブジェクトラベルBootstrap
    {
        [SerializeField, Tooltip("衝撃球のトランスフォームプロパティ")]
        Transform _impactBallTF;
        
        ObjectLabelContext _ctx;
        
        void Awake()
        {
            _ctx = new ObjectLabelContext(Camera.main.transform, _impactBallTF);
        }

        void Start()
        {
            GetComponent<ObjectLabel>().Initialize(_ctx);
        }
    }
}