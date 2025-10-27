using UnityEngine;

namespace TheClimb.Player
{
    public interface IPlayerDataProvider    //  プレイヤーのデータを提供するプロパティ
    {
        Vector3 PositionProperty { get; }
    }
}