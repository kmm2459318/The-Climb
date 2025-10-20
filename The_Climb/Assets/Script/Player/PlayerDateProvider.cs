using UnityEngine;

namespace TheClimb.Player
{
    public class PlayerDateProvider : MonoBehaviour, IPlayerDataProvider    //  ƒvƒŒƒCƒ„[‚Ìî•ñ‚ð’ñ‹Ÿ‚·‚é
    {
        public Vector2 PostionProperty => this.transform.position;
    }
}