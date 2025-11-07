using UnityEngine;

namespace TheClimb.Player
{
    public interface IPlayerDataProvider    //  プレイヤーのデータを提供するプロパティ
    {
        Transform TransformProperty { get; }
        Vector3 PositionProperty { get; }
    }
}