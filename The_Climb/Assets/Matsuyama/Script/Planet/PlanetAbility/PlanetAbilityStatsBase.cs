using UnityEngine;

namespace TheClimb.Core
{
    public abstract class PlanetAbilityStatsBase : ScriptableObject
    {
        public abstract float ChargeCompleteTime { get; }
        public abstract float PrimaryEffectSpawnTime { get; }
        public abstract float SecondaryEffectSpawnTime { get; }
        public virtual float RepulsiveFouce { get; }

    }
}