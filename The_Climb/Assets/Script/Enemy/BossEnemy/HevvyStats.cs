using UnityEngine;

[CreateAssetMenu(fileName = "HevvyStats", menuName = "Enemy/HevvyStats", order = 1)]
public class HevvyStats : ScriptableObject
{
    [Header("�W�����v�ݒ�")]
    public float jumpForce = 10f;               // �c�����W�����v��
    public float horizontalJumpForce = 2f;      // �������W�����v��
    public float jumpInterval = 3f;             // �W�����v�Ԋu
    [Header("�`���[�W�W�����v�ݒ�")]
    public int jumpsBeforeCharge = 3;
    public float chargeDuration = 1.5f;
    public float chargeJumpForce = 20f;
    public float slowFallGravityScale = 0.2f;
}