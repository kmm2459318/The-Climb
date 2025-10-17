using TheClimb.UniversalGravity;

namespace TheClimb.Astral
{
    [System.Serializable]
    public class PlanetStatusBlock    //  天体ステータスブロック
    {
        public GravitationLevel gravitationLevel;

        public float RotationSpeed;    //  天体自転速度
        public float AttractRange;     //  引き寄せ半径
        public float Mass;     //  引き寄せ半径
    }
}