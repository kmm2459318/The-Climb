using UnityEngine;

namespace TheClimb.Astral
{
    [System.Serializable]
    public class PlanetDataProvider : IPlanetDataProvider    //  天体のデータを提供する
    {
        private readonly Transform _planetTransform;

        public PlanetDataProvider(Transform planetTransform)    //  コンストラクタ
        {
            _planetTransform = planetTransform;
        }

        public Vector3 PositionProperty => _planetTransform.position;
    }
}