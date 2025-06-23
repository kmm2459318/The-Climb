using UnityEngine;

[CreateAssetMenu(fileName = "HevvyStats", menuName = "Enemy/HevvyStats", order = 1)]
public class HevvyStats : ScriptableObject
{
    [Header("ジャンプ設定")]
    public float JumpForce = 10f;               // 縦方向ジャンプ力
    public float HorizontalJumpForce = 2f;      // 横方向ジャンプ力
    public float JumpInterval = 3f;             // ジャンプ間隔
    [Header("チャージジャンプ設定")]
    public int JumpsBeforeCharge = 3;
    public float ChargeDuration = 1.5f;
    public float ChargeJumpForce = 20f;
    public float SlowFallGravityScale = 0.2f;
    [Header("ジャンプ方向制限設定")]
    public float LeftBoundary;
    public float RightBoundary;
}