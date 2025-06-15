using UnityEngine;

[CreateAssetMenu(fileName = "HevvyStats", menuName = "Enemy/HevvyStats", order = 1)]
public class HevvyStats : ScriptableObject
{
    [Header("ジャンプ設定")]
    public float jumpForce = 10f;               // 縦方向ジャンプ力
    public float horizontalJumpForce = 2f;      // 横方向ジャンプ力
    public float jumpInterval = 3f;             // ジャンプ間隔
    [Header("チャージジャンプ設定")]
    public int jumpsBeforeCharge = 3;
    public float chargeDuration = 1.5f;
    public float chargeJumpForce = 20f;
    public float slowFallGravityScale = 0.2f;
}