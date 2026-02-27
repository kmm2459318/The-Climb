using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    private enum State
    {
        Patrol,
        Chase,
        Attack,
        Tackle
    }

    [Header("巡回ポイント")]
    [SerializeField] private Transform[] patrolPoints;

    [Header("設定")]
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float loseSightTime = 3f;
    [Header("攻撃距離")]
    [SerializeField] private float attackDistance = 2f;

    [SerializeField] private float attackCooldown = 1.5f;
    [Header("攻撃判定")]
    [SerializeField] private Collider attackCollider;
    [Header("タックル設定")]
    [SerializeField] private float tackleSpeed = 12f;
    [SerializeField] private float tackleDuration = 1.2f;
    private bool isTackling = false;
    private float tackleTimer;
    private Vector3 tackleDirection;
    private Animator animator;
    private float attackTimer;

    private NavMeshAgent agent;
    private EnemyDetection detection;
    private Transform player;

    private int currentIndex = 0;
    private float waitTimer;
    private float loseTimer;
    private bool isWaiting = false;
    private State currentState = State.Patrol;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        detection = GetComponent<EnemyDetection>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (attackCollider != null)
            attackCollider.enabled = false;

        MoveToNextPoint();
    }

    void Update()
    {
       // Debug.Log(agent.isOnNavMesh);
        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol();
                break;

            case State.Chase:
                UpdateChase();
                break;
            case State.Attack:
                UpdateAttack();
                break;
            case State.Tackle:
                UpdateTackle();
                break;
        }
        if (currentState == State.Tackle)
        {
            animator.SetFloat("Speed", tackleSpeed);
        }
        else
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }
    public void EnableAttack()
    {
        Debug.Log("EnableAttack called");

        if (attackCollider != null)
            attackCollider.enabled = true;
    }

    public void DisableAttack()
    {
        Debug.Log("DisableAttack called");

        if (attackCollider != null)
            attackCollider.enabled = false;
    }
    void UpdatePatrol()
    {
        if (detection.CanSeePlayer)
        {
            currentState = State.Chase;
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
            {
                MoveToNextPoint();
            }
        }
    }

    void UpdateChase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackDistance)
        {
            StartTackle();
            return;
        }

        if (!detection.CanSeePlayer)
        {
            loseTimer -= Time.deltaTime;

            if (loseTimer <= 0f)
            {
                currentState = State.Patrol;
                MoveToNextPoint();
            }
        }
        else
        {
            loseTimer = loseSightTime;
        }
    }
    void StartTackle()
    {
        currentState = State.Tackle;

        agent.velocity = Vector3.zero;
        tackleTimer = tackleDuration;

        agent.isStopped = true;
        agent.updateRotation = false;

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f;
        transform.forward = dir;

        animator.SetTrigger("Tackle"); // ←追加
    }
    void UpdateTackle()
    {
        agent.Move(transform.forward * tackleSpeed * Time.deltaTime);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName("Tackle"))
        {
            EndTackle();
        }
    }
    void EndTackle()
    {
        isTackling = false;

        agent.isStopped = false;
        agent.updateRotation = true;

        agent.ResetPath();

        currentState = State.Chase; // ← 追加（重要）
    }
    void UpdateAttack()
    {
        agent.isStopped = true;
        transform.LookAt(player);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            currentState = State.Chase;
            return;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 攻撃アニメ中なら何もしない（最後まで再生させる）
        if (stateInfo.IsName("Attack"))
            return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            animator.SetTrigger("Attack");
            attackTimer = attackCooldown;
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
}