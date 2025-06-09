using System.Collections;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
//  �ړ��X�N���v�g�Ɉړ��l��n��
public class KickerMoveCommander : MonoBehaviour, IWallHitTable
{
    Coroutine JumpLoop;
    [Header("Instance")]
    [SerializeField] KickerStatus kickerStatus;    //  �X�e�[�^�X�C���X�^���X
    EnemyMover enemyMover;    //  �G�l�~�[���[�o�[�C���X�^���X

    Vector3 Velocity;    //  �L�����N�^�[�ړ��l

    float CurrentMoveSpd;    //  ���݂̈ړ����x
    float CurrentJumpForce;    //  ���݂̃W�����v��
    float CurrentJumpFrequency;    //  ���݂̃W�����v�p�x
    [Header("Move Value")]
    [SerializeField] int MoveDir;    //  ���݂̓�������(X��)


    void Awake()
    {
        enemyMover = GetComponent<EnemyMover>();

        //  ���l������
        Initialize();
    }
    void Start()
    {
        Velocity = new Vector3(CurrentMoveSpd * MoveDir * Time.fixedDeltaTime, 0f, 0f);
        JumpLoop = StartCoroutine(PeriodicallyJump());
    }
    public void FixedUpdate()
    {
        Velocity = new Vector3(CurrentMoveSpd * MoveDir * Time.fixedDeltaTime, Velocity.y, 0f);
        //  ��{�ړ�
        enemyMover.BaseMove(Velocity);
    }
    //  ������
    void Initialize()
    {
        CurrentMoveSpd = kickerStatus.MoveSpdProperty;
        CurrentJumpForce = kickerStatus.JumpForceProperty;
        CurrentJumpFrequency = kickerStatus.JumpFrequencyProperty;
        if(MoveDir == 0)
        {
            MoveDir = 1;
        }
        else 
        {
            MoveDir = (int)Mathf.Sign(MoveDir);
        }
    }
    //  �ǂɓ����������̏���
    public void OnHitWall()
    {
        MoveDir *= -1;
    }
    //  ����I�ȃW�����v�̃��[�v
    IEnumerator PeriodicallyJump()
    {
        while (true)
        {
            yield return new WaitForSeconds(CurrentJumpFrequency);
            //  �W�����v
            enemyMover.Jump(CurrentJumpForce);
        }

    }
}