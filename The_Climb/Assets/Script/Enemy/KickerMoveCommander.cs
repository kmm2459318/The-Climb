using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
//  �ړ��X�N���v�g�Ɉړ��l��n��
public class KickerMoveCommander : MonoBehaviour, IWallHitTable
{
    [SerializeField] KickerStatus kickerStatus;    //  �X�e�[�^�X�C���X�^���X
    EnemyMover enemyMover;    //  �G�l�~�[���[�o�[�C���X�^���X

    Vector2 Velocity;    //  �L�����N�^�[�ړ��l

    float CurrentMoveSpd;    //  ���݂̈ړ����x
    int CurrentMoveDir;    //  ���݂̓�������(X��)

    void Awake()
    {
        enemyMover = GetComponent<EnemyMover>();

        //  ���l������
        Initialize();
    }
    void Start()
    {
        Velocity = new Vector3(CurrentMoveSpd * CurrentMoveDir * Time.fixedDeltaTime, 0f, 0f);
    }
    public void FixedUpdate()
    {
        Velocity = new Vector3(CurrentMoveSpd * CurrentMoveDir * Time.fixedDeltaTime, 0f, 0f);
        enemyMover.Move(Velocity);
    }
    //  ������
    void Initialize()
    {
        CurrentMoveSpd = kickerStatus.MoveSpdProperty;
        CurrentMoveDir = kickerStatus.FirstMoveDirProperty;
    }
    //  �ǂɓ����������̏���
    public void OnHitWall()
    {
        CurrentMoveDir *= -1;
    }
}