using UnityEngine;

namespace TheClimb.Core
{
    public interface IEffectSystem    //  エフェクトシステムのインターフェース
    {
        void Play(EffectKey key, Vector3 pos);
        void Stop(EffectKey key);
    }
}