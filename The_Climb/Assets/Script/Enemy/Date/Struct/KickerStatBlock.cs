using UnityEngine;

//  キッカーのステータスブロック
[System.Serializable]
public class KickerStatBlock
{
    [Header("端を判定する光線の位置")]
    public Vector3 EdgeRayOffset;    //  端判定のRayのオフセット
    [Header("移動値")]
    public float MoveSpd;    //  移動速度
    public float JumpForce;    //  ジャンプ力
    public float JumpFrequency;    //  ジャンプ頻度
    public float BlowForce;    //  吹っ飛ばし力
}
