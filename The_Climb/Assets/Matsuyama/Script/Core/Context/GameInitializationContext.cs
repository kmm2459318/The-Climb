using TheClimb.Item;
using TheClimb.UniversalGravity;
using UnityEngine;

namespace TheClimb.Core
{
    public class GameInitializationContext : MonoBehaviour
    {
        [SerializeField] InpactBallController inpactBallController;
        [SerializeField] GravitationTargetStatusBlock _gravitationTargetStatusBlock;
        private void Awake()
        {
            inpactBallController.Initialize(_gravitationTargetStatusBlock);
        }
    }
}