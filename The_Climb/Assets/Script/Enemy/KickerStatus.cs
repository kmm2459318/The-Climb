using UnityEngine;

[CreateAssetMenu(fileName = "KickerStatus", menuName = "GameDate/Enemy/KickerStatus")]
////  �L�b�J�[�̃X�e�[�^�X
public class KickerStatus : ScriptableObject
{
    [Header("Kicker Status")]
    [SerializeField] float MoveSpd;    //  �ړ����x
    [SerializeField] float JumpForce;    //  �W�����v��
    [SerializeField] int FirstMoveDir;    //  �W�����v��

    public float MoveSpdProperty => MoveSpd;
    public float JumpForceProperty => JumpForce;
    public int FirstMoveDirProperty => FirstMoveDir;
}
