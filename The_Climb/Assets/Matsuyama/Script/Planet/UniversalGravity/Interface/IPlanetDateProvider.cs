using UnityEngine;

namespace TheClimb.Astral
{
    public interface IPlanetDataProvider    //  天体データ提供
    {
        public Vector2 PostionProperty { get; }
    }
}