using System.Collections;
using Unity.Mathematics;
using UnityEngine;
[RequireComponent(typeof(EnemyMover))]
public class BossEnemy_HevvyMovement : MonoBehaviour
{
    Coroutine jumpLoop;
    Coroutine attackLoop;

    [Header("�X�e�[�^�X")]
    [SerializeField] BossStatus bossStatus;

    EnemyMover enemyMover;
    Rigidbody rb;

    Vector3 velocity;
    float currentMoveSpd;
    float currentJumpForce;
    float currentJumpInterval;
    float currentAttackInterval;
    float slowFallSpeed;
    float apexThreshold;

    [SerializeField] int moveDir = 1;

    void Awake()
    {
        enemyMover = GetComponent<EnemyMover>();
        rb = GetComponent<Rigidbody>();
        Initialize();
    }

    void Start()
    {
        jumpLoop = StartCoroutine(PeriodicallyJump());
        attackLoop = StartCoroutine(SpecialAttackLoop());
    }

    void FixedUpdate()
    {
        if (!isAttacking)
        {
            velocity = new Vector3(currentMoveSpd * moveDir * Time.fixedDeltaTime, rb.linearVelocity.y, 0f);
            enemyMover.BaseMove(velocity);
        }
    }

    void Initialize()
    {
        currentMoveSpd = bossStatus.MoveSpeed;
        currentJumpForce = bossStatus.SpecialJumpForce;
        currentJumpInterval = bossStatus.JumpInterval;
        currentAttackInterval = bossStatus.AttackInterval;
        slowFallSpeed = bossStatus.SlowFallSpeed;
        apexThreshold = bossStatus.ApexDetectThreshold;

        moveDir = (moveDir == 0) ? 1 : (int)Mathf.Sign(moveDir);
    }

    public void OnHitWall()
    {
        moveDir *= -1;
    }

    IEnumerator PeriodicallyJump()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentJumpInterval);
            enemyMover.Jump(bossStatus.JumpForce); // �ʏ�W�����v
        }
    }

    bool isAttacking = false;

    IEnumerator SpecialAttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentAttackInterval);

            // �U�����t���O�𗧂Ă�
            isAttacking = true;

            // ��~�t�F�[�Y
            yield return new WaitForSeconds(bossStatus.AttackStopDuration);

            // ����W�����v
            enemyMover.Jump(currentJumpForce);

            // �ō����B�_�܂őҋ@
            yield return new WaitUntil(() => Mathf.Abs(rb.linearVelocity.y) < apexThreshold);

            // ������艺�~
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -slowFallSpeed, rb.linearVelocity.z);

            // �n�ʂɂ��܂ő҂i�����ł͒n�ʐڒn�͉�����j
            yield return new WaitUntil(() => enemyMover.IsGrounded);

            // �U���I��
            isAttacking = false;
        }
    }
}