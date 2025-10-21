using UnityEngine;

namespace TheClimb.Player
{
    public class PlayerDataProvider : IPlayerDataProvider    //  プレイヤーの情報を提供する
    {
        private readonly Transform _planetTransform;

        public PlayerDataProvider(Transform planetTransform)    //  コンストラクタ
        {
            _planetTransform = planetTransform;
        }

        public Vector3 PositionProperty => _planetTransform.position;
    }
}