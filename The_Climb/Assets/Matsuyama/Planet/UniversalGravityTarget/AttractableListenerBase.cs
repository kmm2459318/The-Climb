using UnityEngine;

namespace TheClimb.Astral
{
    public abstract class AttractableListenerBase :MonoBehaviour, IAttractableListener    //  引き寄せ脳オブジェクトリスナー基底クラス
    {
        public virtual void OnAttract() { }
        
        public virtual float RemainCount { get; }
    }
}