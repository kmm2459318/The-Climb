using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(CharacterGroundChecker))]
[RequireComponent(typeof(LandGroundNotifier))]
//  �ړ��X�N���v�g�Ɉړ��l��n��
public class KickerMoveCommander : MonoBehaviour, IWallHitTable, ILandingHandler
{
    public enum KickerCommanderMethod    //  ���̃N���X���̊֐��ꗗ
    {
        MOVE,
        IS_EDGE_POS,
        FLIP_MOVE_DIR,
        JUMP,
    }
    

    [Header("Instance")]
    [SerializeField] KickerStatus kickerStatus;    //  �X�e�[�^�X�C���X�^���X
    KickerStatBlock kickerStatBlock;    //  �X�e�[�^�X�C���X�^���X
    EnemyMover enemyMover;    //  �G�l�~�[���[�o�[�C���X�^���X
    CharacterGroundChecker characterGroundChecker;    //  �O���E���h�`�F�b�J�[�C���X�^���X
    EnemyStateMachine enemyStateMachine;
    public Dictionary<KickerCommanderMethod, ICommand> CommanderMethodMap;    //  ���̃X�N���v�g�̊֐��̎���
    public event Action OnJumpTime;    //  �W�����v�^�C���̃T�u�X�N
    public event Action OnLandGround;    //  �n�ʒ��n�̃T�u�X�N
    Coroutine JumpLoop;    //  �W�����v���[�v�R���[�`���̕ϐ�

    ICommandProvider commandProvider;
    IEnemyStateFactory enemyStateFactory;

    Vector3 Velocity;    //  �L�����N�^�[�ړ��l
    Vector3 EdgeRayOffset;    //  �[�����m����Ray�̃I�t�Z�b�g

    [Header("Move Value")]
    [SerializeField] MoveDir CurrentMoveDir;    //  ���݂̓�������(X��)
    float CurrentMoveSpd;    //  ���݂̈ړ����x
    float CurrentJumpForce;    //  ���݂̃W�����v��
    float CurrentJumpFrequency;    //  ���݂̃W�����v�p�x

    void Awake()
    {
        enemyMover = GetComponent<EnemyMover>();
        kickerStatBlock = kickerStatus.GetStats(EnemyMode.NORMAL);
        characterGroundChecker = GetComponent<CharacterGroundChecker>();
        enemyStateMachine = new EnemyStateMachine();
        commandProvider = new DefaultCommandProvider(this);
        enemyStateFactory = new EnemyStateFactory(this, enemyStateMachine);

        //  ������Ԃ�Walk�ɕύX
        CommanderMethodMap =commandProvider.GetCommandMap();
        enemyStateMachine.ChangeState(enemyStateFactory.CreateWalkState());
        
        //  ���l������
        Initialize();
    }
    void Start()
    {
        //  ����I�ȃW�����v�̃��[�v���J�n
        JumpLoop = StartCoroutine(PeriodicallyJump());
    }
    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, Vector3.down * characterGroundChecker.GroundCheckDisProperty, Color.red);
        Debug.DrawRay(EdgeRayOffset + transform.position, Vector3.down * characterGroundChecker.GroundCheckDisProperty, Color.gray);
        enemyStateMachine.FixedUpdate();
    }
    //  ������
    void Initialize()
    {
        //CommanderMethodMap = new Dictionary<KickerCommanderMethod, ICommand>
        //{
        //    {KickerCommanderMethod.MOVE, new ActionCommand(Move)},
        //    {KickerCommanderMethod.IS_EDGE_POS, new FuncCommand<bool>(IsEdgePos) },
        //    {KickerCommanderMethod.FLIP_MOVE_DIR, new ActionCommand(FlipMoveDir)},
        //    {KickerCommanderMethod.JUMP, new ActionCommand(Jump)},
        //};

        EdgeRayOffset = kickerStatBlock.EdgeRayOffset;
        if(CurrentMoveDir != MoveDir.NONE && Mathf.Sign(EdgeRayOffset.x) != Mathf.Sign((int)CurrentMoveDir))
        {
            Debug.LogWarning($"CurrentMoveDir : {CurrentMoveDir} and EdgeRayOffset direction of x :{EdgeRayOffset.x} is not same direction.{Environment.NewLine}  Fix to CurrentMoveDirection");
            EdgeRayOffset *= (int)MoveDir.LEFT;
        }

        CurrentMoveSpd = kickerStatBlock.MoveSpd;
        CurrentJumpForce = kickerStatBlock.JumpForce;
        CurrentJumpFrequency = kickerStatBlock.JumpFrequency;
    }
    //  ����I�ȃW�����v���΃��[�v
    IEnumerator PeriodicallyJump()
    {
        while (true)
        {
            yield return new WaitForSeconds(CurrentJumpFrequency);
            if (characterGroundChecker.CheckIsGround())
            {
                OnJumpTime?.Invoke();
            }
        }
    }
    //  �ړ�
    public void Move()
    {
        Velocity = new Vector3(CurrentMoveSpd * (int)CurrentMoveDir * Time.fixedDeltaTime, Velocity.y, 0f);
        //  ��{�ړ�
        enemyMover.BaseMove(Velocity);
    }
    //  �[�����肷��
    public bool IsEdgePos()
    {
        return !characterGroundChecker.CheckIsGround(EdgeRayOffset + this.transform.position) &&
                characterGroundChecker.CheckIsGround();
    }
    //  �ړ��������]
    public void FlipMoveDir()
    {
        CurrentMoveDir = CurrentMoveDir switch
        {
            MoveDir.RIGHT => MoveDir.LEFT,
            MoveDir.LEFT => MoveDir.RIGHT,
            _ => MoveDir.NONE,
        };
        EdgeRayOffset.x *= -1f;
    }
    //  �W�����v
    public void Jump()
    {
        enemyMover.Jump(CurrentJumpForce);
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
        EdgeRayOffset.x *= -1f;
    }
    //    �X�e�[�W�ɒ��n�������̏���
    public void OnLandStage()
    {
        OnLandGround?.Invoke();
    }
}