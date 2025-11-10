using UnityEngine;

namespace TheClimb.Player
{
    public class PlayerDataProvider : IPlayerDataProvider    //  プレイヤーの情報を提供する
    {
        private readonly Transform _playerTransform;    //  プレイヤーのトランスフォーム

        public PlayerDataProvider(Transform PlayerTransform)    //  コンストラクタ
        {
            _playerTransform = PlayerTransform;
        }

        public Transform TransformProperty => _playerTransform;
        public Vector3 PositionProperty => _playerTransform.position;
    }
}