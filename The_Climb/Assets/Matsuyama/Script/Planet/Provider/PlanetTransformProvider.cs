using UnityEngine;

namespace TheClimb.Astral
{
    public class PlanetTransformProvider : MonoBehaviour, ITransformProvider
    {
        public Transform transformGetter => this.transform;
    }
}