using TheClimb.Astral;
using UnityEngine;

namespace TheClimb.Core
{
    public class OrbitalContext : PlanetCommandBaseCtx    //  軌道コンテキスト
    {
        public Transform _playerTransform;    //  プレイヤートランスフォーム

        public ICorutineRunner _corutineRunner;    //  コルーチンランナー
        public OrbitalContext(Transform planetTF, OrbitalStatusBlock status, Transform playerTF, ICorutineRunner runner)
            : base (planetTF, status)    //  コンテキスト
        {
            _playerTransform = playerTF;
            _corutineRunner = runner;
        }
    }
}