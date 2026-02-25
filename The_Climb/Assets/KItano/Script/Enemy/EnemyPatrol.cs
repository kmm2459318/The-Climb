using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [Header("巡回ポイント")]
    [SerializeField] private Transform[] patrolPoints;

    [Header("設定")]
    [SerializeField] private float waitTime = 2f;

    private NavMeshAgent agent;
    private int currentIndex = 0;
    private float waitTimer;
    private bool isWaiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentIndex].position);
        }
    }

    void Update()
    {
        if (patrolPoints.Length == 0) return;

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

    void MoveToNextPoint()
    {
        currentIndex = (currentIndex + 1) % patrolPoints.Length;

        agent.isStopped = false;
        agent.SetDestination(patrolPoints[currentIndex].position);

        isWaiting = false;
    }
}