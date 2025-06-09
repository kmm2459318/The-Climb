using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/BossStatus")]
public class BossStatus : ScriptableObject
{
    public float MoveSpeed = 2.0f;

    public float JumpForce = 4.0f;         // 通常ジャンプ
    public float JumpInterval = 2.0f;

    public float AttackInterval = 5.0f;    // 攻撃までの待機時間
    public float AttackStopDuration = 1.5f; // 停止フェーズ時間
    public float SpecialJumpForce = 12.0f;  // 特大ジャンプ力

    public float ApexDetectThreshold = 0.2f;  // Y速度がこの値以下なら最高点
    public float SlowFallSpeed = 2.0f;        // ゆっくり落ちる時の下降速度
}