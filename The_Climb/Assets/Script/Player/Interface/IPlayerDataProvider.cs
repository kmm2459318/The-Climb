using UnityEngine;

namespace TheClimb.Player
{
    public interface IPlayerDataProvider    //  プレイヤーのデータを提供するプロパティ
    {
        Vector2 PositionProperty { get; }
    }
}