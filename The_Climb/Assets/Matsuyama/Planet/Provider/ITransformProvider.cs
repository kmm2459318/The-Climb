using UnityEngine;

namespace TheClimb.Astral
{
    public interface ITransformProvider    //  トランスフォーム提供Interface
    {
        Transform transformGetter { get; }
    }
}
