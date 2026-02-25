using UnityEngine;

namespace TheClimb.Astral
{
    [CreateAssetMenu(fileName = "PlanetAbilityStats" ,menuName = "Astral/AbilityStats")]
    public class PlanetAbilityStatus : ScriptableObject    //  天体の能力の能力値
    {
        [Header("天体のアビリティの能力値")]
        [Tooltip("反発力(吹き飛ばし力)")]
        public float repulsiveFouce;
    }
}