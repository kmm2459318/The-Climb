using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    private enum State
    {
        Patrol,
        Charge,
        Tackle,
        Recovery
    }

    [Header("巡回ポイント")]
    [SerializeField] private Transform[] patrolPoints;

    [Header("設定")]
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float loseSightTime = 3f;

    [Header("攻撃距離")]
    [SerializeField] private float attackDistance = 3f;

    [Header("攻撃判定")]
    [SerializeField] private Collider attackCollider;

    [Header("タックル設定")]
    [SerializeField] private float tackleSpeed = 10f;
    
    [SerializeField] private float tackleLoseSightDelay = 0.3f;

    private float loseSightTimer = 0f;
  
    private State currentState = State.Patrol;

    private NavMeshAgent agent;
    private Animator animator;
    private EnemyDetection detection;
    private Transform player;

    private int currentIndex = 0;
    private float waitTimer;
    private bool isWaiting = false;

    private Vector3 lastSeenDirection;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        detection = GetComponent<EnemyDetection>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (attackCollider != null)
            attackCollider.enabled = false;

        MoveToNextPoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol();
                break;

            case State.Charge:
                UpdateCharge();
                break;

            case State.Tackle:
                UpdateTackle();
                break;

            case State.Recovery:
                UpdateRecovery();
                break;
        }

        // Speed制御
        if (currentState == State.Tackle)
        {
            animator.SetFloat("Speed", tackleSpeed);
        }
        else if (currentState == State.Charge || currentState == State.Recovery)
        {
            animator.SetFloat("Speed", 0f);
        }
        else
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    // =========================
    // Patrol
    // =========================
    void UpdatePatrol()
    {
        if (detection.CanSeePlayer)
        {
            StartCharge();
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitTime;
                agent.isStopped = true;
            }
        }

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
                MoveToNextPoint();
        }
    }

    void MoveToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.isStopped = false;
        agent.SetDestination(patrolPoints[currentIndex].position);

        currentIndex = (currentIndex + 1) % patrolPoints.Length;
        isWaiting = false;
    }

    // =========================
    // Charge（溜め）
    // =========================
    void StartCharge()
    {
        currentState = State.Charge;

        agent.isStopped = true;
        agent.ResetPath();              // ★追加
        agent.velocity = Vector3.zero;  // ★追加

        lastSeenDirection = (player.position - transform.position).normalized;
        lastSeenDirection.y = 0f;

        transform.forward = lastSeenDirection;

        animator.SetFloat("Speed", 0f); // ★追加（念押し）
        animator.SetTrigger("Charge");
    }

    void UpdateCharge()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Chargeアニメが終わったらTackleへ
        if (!stateInfo.IsName("Charge"))
        {
            StartTackle();
        }
    }

    // =========================
    // Tackle
    // =========================
    void StartTackle()
    {
        currentState = State.Tackle;

        agent.velocity = Vector3.zero;
        agent.updateRotation = false;

        transform.forward = lastSeenDirection;

        animator.SetTrigger("Tackle");

        loseSightTimer = 0f;   // ★追加

        EnableAttack();
    }

    void UpdateTackle()
    {
        // 前進
        agent.Move(transform.forward * tackleSpeed * Time.deltaTime);

        // 視界チェック
        if (!detection.CanSeePlayer)
        {
            loseSightTimer += Time.deltaTime;
        }
        else
        {
            loseSightTimer = 0f;
        }

        // 見失ったら終了
        if (loseSightTimer >= tackleLoseSightDelay)
        {
            DisableAttack();
            StartRecovery();
        }
    }

    // =========================
    // Recovery（硬直）
    // =========================
    void StartRecovery()
    {
        currentState = State.Recovery;

        agent.updateRotation = true;
        agent.velocity = Vector3.zero;

        animator.SetTrigger("Recovery");
    }

    void UpdateRecovery()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName("Recovery"))
        {
            if (detection.CanSeePlayer)
            {
                StartCharge();
            }
            else
            {
                agent.isStopped = false;
                currentState = State.Patrol;
                MoveToNextPoint();
            }
        }
    }

    // =========================
    // 攻撃判定
    // =========================
    void EnableAttack()
    {
        if (attackCollider != null)
            attackCollider.enabled = true;
    }

    void DisableAttack()
    {
        if (attackCollider != null)
            attackCollider.enabled = false;
    }
}