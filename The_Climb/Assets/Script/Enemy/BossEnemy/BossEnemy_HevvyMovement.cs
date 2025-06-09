using System.Collections;
using Unity.Mathematics;
using UnityEngine;
[RequireComponent(typeof(EnemyMover))]
public class BossEnemy_HevvyMovement : MonoBehaviour
{
    Coroutine jumpLoop;
    Coroutine attackLoop;

    [Header("ステータス")]
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
            enemyMover.Jump(bossStatus.JumpForce); // 通常ジャンプ
        }
    }

    bool isAttacking = false;

    IEnumerator SpecialAttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentAttackInterval);

            // 攻撃中フラグを立てる
            isAttacking = true;

            // 停止フェーズ
            yield return new WaitForSeconds(bossStatus.AttackStopDuration);

            // 特大ジャンプ
            enemyMover.Jump(currentJumpForce);

            // 最高到達点まで待機
            yield return new WaitUntil(() => Mathf.Abs(rb.linearVelocity.y) < apexThreshold);

            // ゆっくり下降
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -slowFallSpeed, rb.linearVelocity.z);

            // 地面につくまで待つ（ここでは地面接地は仮判定）
            yield return new WaitUntil(() => enemyMover.IsGrounded);

            // 攻撃終了
            isAttacking = false;
        }
    }
}