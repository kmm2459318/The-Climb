using UnityEngine;

//  キッカーのステータスブロック
[System.Serializable]
public class KickerStatBlock
{
    public Vector3 EdgeRayOffset;    //  端判定のRayのオフセット
    public float MoveSpd;    //  移動速度
    public float JumpForce;    //  ジャンプ力
    public float JumpFrequency;    //  ジャンプ頻度
}
