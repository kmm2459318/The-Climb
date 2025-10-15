using UnityEngine;

namespace TheClimb.UniversalGravity
{
    [CreateAssetMenu(menuName = "UniversalGravity/GravitationTargetStatus")]
    public class GravitationTargetStatusBlock : ScriptableObject    //  万有引力を受けるオブジェクトのデータ
    {
        [Header("影響される重力レベル")]
        public GravitationLevel affectedGravitationLevel;    //  引力の影響を受けるかどうか

        [Header("質量")]
        public float Mass;    //  質量
    }
}