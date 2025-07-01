using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BossEnemy_HevvyMovement : MonoBehaviour
{
    private enum BossState
    {
        Idle, Move, ChargeVertical, JumpVertical,
        ChargeArc, JumpArc, Defeated
    }

    [Header("参照")]
    public Transform player;
    public Animator animator;
    public HevvyStats stats;

    [Header("起動条件")]
    public float activationDistance = 10f;

    private BossState currentState;
    private int hitCount = 0;
    private bool isVulnerable = false;
    private Rigidbody rb;
    private bool hasActivated = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("[Boss] 初期化完了");
        ChangeState(BossState.Idle);
    }

    void Update()
    {
        if (!hasActivated && currentState == BossState.Idle)
        {
            float dist = Vector3.Distance(player.position, transform.position);
            Debug.Log($"[Boss] Idle中：プレイヤーとの距離 = {dist:F2}");

            if (dist <= activationDistance)
            {
                Debug.Log("[Boss] プレイヤーが接近 → 起動");
                ChangeState(BossState.Move);
                hasActivated = true;
            }
        }
        else if (currentState == BossState.Move)
        {
            MoveTowardsPlayer();
            CheckForTransition();
        }
    }

    void ChangeState(BossState newState)
    {
        Debug.Log($"[Boss] 状態遷移: {currentState} → {newState}");
        currentState = newState;

        switch (newState)
        {
            case BossState.Idle:
                PlayAnimation("Idle");
                break;
            case BossState.Move:
                PlayAnimation("HopMove");
                break;
            case BossState.ChargeVertical:
                PlayAnimation("Charge");
                StartCoroutine(ChargeThenJumpVertical());
                break;
            case BossState.ChargeArc:
                PlayAnimation("Charge");
                StartCoroutine(ChargeThenJumpArc());
                break;
            case BossState.Defeated:
                PlayAnimation("Defeat");
                Debug.Log("[Boss] 撃破されました");
                Destroy(gameObject, 2f);
                break;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * stats.hopSpeed * Time.deltaTime;
        Debug.Log("[Boss] プレイヤーに向かって移動中");
    }

    void CheckForTransition()
    {
        float dist = Vector3.Distance(player.position, transform.position);
        Debug.Log($"[Boss] プレイヤーとの距離: {dist:F2}");

        if (dist < stats.nearTriggerDistance)
        {
            Debug.Log("[Boss] プレイヤーが近い → 垂直チャージへ");
            ChangeState(BossState.ChargeVertical);
        }
        else if (dist > stats.farTriggerDistance)
        {
            Debug.Log("[Boss] プレイヤーが遠い → 山なりチャージへ");
            ChangeState(BossState.ChargeArc);
        }
    }

    IEnumerator ChargeThenJumpVertical()
    {
        Debug.Log("[Boss] 垂直ジャンプ チャージ開始");
        yield return new WaitForSeconds(stats.chargeTime);

        PlayAnimation("JumpVertical");
        rb.linearVelocity = new Vector3(0, stats.verticalJumpForce, 0);
        Debug.Log("[Boss] 垂直ジャンプ 実行");

        // Wait until falling then grounded
        yield return new WaitUntil(() => rb.linearVelocity.y <= 0);
        yield return new WaitUntil(() => IsGrounded());

        Debug.Log("[Boss] 垂直ジャンプ終了 → 移動へ");
        ChangeState(BossState.Move);
    }

    IEnumerator ChargeThenJumpArc()
    {
        Debug.Log("[Boss] 山なりジャンプ チャージ開始");
        yield return new WaitForSeconds(stats.chargeTime);

        PlayAnimation("JumpArc");

        Vector3 targetPos = player.position;
        Vector3 direction = (targetPos - transform.position).normalized;
        rb.linearVelocity = direction * stats.arcJumpForce + Vector3.up * stats.arcJumpHeight;

        Debug.Log("[Boss] 山なりジャンプ 実行");

        yield return new WaitUntil(() => rb.linearVelocity.y <= 0);
        yield return new WaitUntil(() => IsGrounded());

        Debug.Log("[Boss] 山なりジャンプ終了 → 移動へ");
        ChangeState(BossState.Move);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void PlayAnimation(string animName)
    {
        if (animator != null && !string.IsNullOrEmpty(animName))
        {
            animator.Play(animName);
            Debug.Log($"[Boss] アニメーション再生: {animName}");
        }
        else
        {
            Debug.Log($"[Boss] アニメーションスキップ（未設定）: {animName}");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == BossState.JumpArc && collision.contacts[0].normal.y > 0.5f)
        {
            Debug.Log("[Boss] 地面に着地（保険的遷移）");
            ChangeState(BossState.Move);
        }
    }

    // Gizmosで可視化
    private void OnDrawGizmosSelected()
    {
        if (stats == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activationDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stats.nearTriggerDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.farTriggerDistance);
    }
}