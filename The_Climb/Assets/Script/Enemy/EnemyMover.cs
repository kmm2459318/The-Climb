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
    //  �ړ�
    public void Move(Vector3 Velocity)
    {
        RB.MovePosition(RB.position + Velocity);
    }
}
