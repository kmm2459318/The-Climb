using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
//  �G�L�����N�^�[�����ۂɓ������X�N���v�g
public class EnemyMover : MonoBehaviour
{
    Rigidbody RB;    //  ���W�b�h�{�f�B�C���X�^���X
    void Awake()
    {
        RB = GetComponent<Rigidbody>();
    }
    //  ��{�ړ�
    public void BaseMove(Vector3 Velocity)
    {
        RB.MovePosition(RB.position + Velocity);
    }
    //  �W�����v
    public void Jump(float JumpForce)
    {
        Vector3 Velocity = RB.linearVelocity;
        Velocity.y = 0;
        RB.linearVelocity = Velocity;
        RB.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }
}
