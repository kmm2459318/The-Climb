using TheClimb.UniversalGravity;
using UnityEngine;

namespace TheClimb.Astral
{
    [System.Serializable]
    public class GravitationStatusBlock    //  天体ステータスブロック
    {
        [Header("万有引力")]
        public GravitationLevel gravitationLevel;    //  重力レベル(未使用)
        public float RotationSpeed;    //  天体自転速度
        public float AttractRange;     //  引き寄せ半径
        public float Mass;             //  引き寄せ半径
    }
}