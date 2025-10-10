using UnityEngine;

namespace TheClimb.Astral
{
    public class PlanetDataProvider : MonoBehaviour, IPlanetDataProvider    //  天体のデータを提供する
    {
        public Vector2 PostionProperty => this.transform.position;
    }
}