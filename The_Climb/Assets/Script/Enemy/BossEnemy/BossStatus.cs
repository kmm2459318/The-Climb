using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/BossStatus")]
public class BossStatus : ScriptableObject
{
    public float MoveSpeed = 2.0f;

    public float JumpForce = 4.0f;         // �ʏ�W�����v
    public float JumpInterval = 2.0f;

    public float AttackInterval = 5.0f;    // �U���܂ł̑ҋ@����
    public float AttackStopDuration = 1.5f; // ��~�t�F�[�Y����
    public float SpecialJumpForce = 12.0f;  // ����W�����v��

    public float ApexDetectThreshold = 0.2f;  // Y���x�����̒l�ȉ��Ȃ�ō��_
    public float SlowFallSpeed = 2.0f;        // ������藎���鎞�̉��~���x
}