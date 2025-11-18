using TheClimb.Astral;
using UnityEngine;

namespace TheClimb.Core
{
    public abstract class PlanetCommandBaseCtx    //  天体コマンドパターンコンテキスト
    {
        public Transform _planetTransform { get; protected set; }    //  天体トランスフォーム
        public OrbitalStatusBlock _orbitalStatusBlock { get; protected set; }    //  軌道のステータスブロック

        protected PlanetCommandBaseCtx(Transform planetTF, OrbitalStatusBlock orbitalStatus)    //  コンストラクタ
        {
            _planetTransform = planetTF;
            _orbitalStatusBlock = orbitalStatus;
        }
    }
}