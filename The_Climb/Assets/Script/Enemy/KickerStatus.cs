using UnityEngine;

[CreateAssetMenu(fileName = "KickerStatus", menuName = "GameDate/Enemy/KickerStatus")]
////  キッカーのステータス
public class KickerStatus : ScriptableObject
{
    [Header("Kicker Status")]
    [SerializeField] float MoveSpd;    //  移動速度
    [SerializeField] float JumpForce;    //  ジャンプ力
    [SerializeField] int FirstMoveDir;    //  ジャンプ力

    public float MoveSpdProperty => MoveSpd;
    public float JumpForceProperty => JumpForce;
    public int FirstMoveDirProperty => FirstMoveDir;
}
