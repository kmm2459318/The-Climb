using UnityEngine;

[CreateAssetMenu(fileName = "HevvyStats", menuName = "BossStats/HevvyStats")]
public class HevvyStats : ScriptableObject
{
    [Header("移動・ジャンプ関連")]
    public float hopSpeed = 2f;
    public float verticalJumpForce = 12f;
    public float arcJumpForce = 8f;
    public float arcJumpHeight = 5f;

    [Header("チャージ・スタン時間")]
    public float chargeTime = 1.5f;
    public float stunDuration = 3f;

    [Header("距離トリガー")]
    public float nearTriggerDistance = 3f;
    public float farTriggerDistance = 6f;

    [Header("必要ヒット数")]
    public int requiredHitsToDefeat = 3;
}