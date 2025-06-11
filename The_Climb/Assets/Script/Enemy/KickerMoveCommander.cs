using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(CharacterGroundChecker))]
//  �ړ��X�N���v�g�Ɉړ��l��n��
public class KickerMoveCommander : MonoBehaviour, IWallHitTable
{
    Coroutine JumpLoop;    //  �W�����v���[�v�R���[�`���̕ϐ�
    [Header("Instance")]
    [SerializeField] KickerStatus kickerStatus;    //  �X�e�[�^�X�C���X�^���X
    KickerStatBlock kickerStatBlock;    //  �X�e�[�^�X�C���X�^���X
    EnemyMover enemyMover;    //  �G�l�~�[���[�o�[�C���X�^���X

    Vector3 Velocity;    //  �L�����N�^�[�ړ��l

    float CurrentMoveSpd;    //  ���݂̈ړ����x
    float CurrentJumpForce;    //  ���݂̃W�����v��
    float CurrentJumpFrequency;    //  ���݂̃W�����v�p�x
    [Header("Move Value")]
    [SerializeField] MoveDir CurrentMoveDir;    //  ���݂̓�������(X��)


    void Awake()
    {
        enemyMover = GetComponent<EnemyMover>();
        kickerStatBlock = kickerStatus.GetStats(EnemyStates.NORMAL);
        //  ���l������
        Initialize();
    }
    void Start()
    {
        Velocity = new Vector3(CurrentMoveSpd * (int)CurrentMoveDir * Time.fixedDeltaTime, 0f, 0f);
        //  ����I�ȃW�����v�̃��[�v���J�n
        JumpLoop = StartCoroutine(PeriodicallyJump());
    }
    public void FixedUpdate()
    {
        Velocity = new Vector3(CurrentMoveSpd * (int)CurrentMoveDir * Time.fixedDeltaTime, Velocity.y, 0f);
        //  ��{�ړ�
        enemyMover.BaseMove(Velocity);
    }
    //  ������
    void Initialize()
    {
        CurrentMoveSpd = kickerStatBlock.MoveSpd;
        CurrentJumpForce = kickerStatBlock.JumpForce;
        CurrentJumpFrequency = kickerStatBlock.JumpFrequency;
    }
    //  �ǂɓ����������̏���
    public void OnHitWall()
    {
        if(CurrentMoveDir != MoveDir.NONE && CurrentMoveDir == MoveDir.RIGHT)
        {
            CurrentMoveDir = MoveDir.LEFT;
        }
        else if(CurrentMoveDir == MoveDir.LEFT)
        {
            CurrentMoveDir = MoveDir.RIGHT;
        }
    }
    //  ����I�ȃW�����v�̃��[�v
    IEnumerator PeriodicallyJump()
    {
        CharacterGroundChecker characterGroundChecker = GetComponent<CharacterGroundChecker>();
        
        while (true)
        {
            yield return new WaitForSeconds(CurrentJumpFrequency);

            if(characterGroundChecker.CheckIsGround())
            //  �W�����v
            enemyMover.Jump(CurrentJumpForce);
        }

    }
}
//  �ȉ��R�[�h�ۑ���
//if ((int)CurrentMoveDir == 0)
//{
//    (int)CurrentMoveDir = 1;
//}
//else
//{
//    MoveDir = (int)Mathf.Sign(MoveDir);
//}