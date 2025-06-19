using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BossEnemy_DropMovement : MonoBehaviour
{
    public DropStats stats;
    private Rigidbody rb;
    [SerializeField] private Transform player;

    private enum State
    {
        RushMove,
        FirstHover,
        Aiming,
        MeteorDrop,
        Rising,
        Waiting
    }

    private State currentState;

    private int rushCounter = 0;
    private int meteorCounter = 0;
    private bool firstHoverDone = false;
    private int aimMoveCounter = 0;
    private bool aimingToRight = true;
    private bool movingToB = true;

    private bool hasDropped = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentState = State.RushMove;
        transform.position = new Vector3(stats.pointA_X, stats.groundY, 0f);
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.RushMove:
                PerformRushMove();
                break;

            case State.FirstHover:
                StartCoroutine(FirstHoverCoroutine());
                break;

            case State.MeteorDrop:
                PerformMeteorDrop();
                break;

            case State.Rising:
                PerformRising();
                break;

            case State.Waiting:
                // コルーチン中の待機
                break;

            case State.Aiming:
                PerformAiming();
                break;
        }
    }

    void PerformRushMove()
    {
        float targetX = movingToB ? stats.pointB_X : stats.pointA_X;
        float direction = Mathf.Sign(targetX - transform.position.x);
        rb.linearVelocity = new Vector3(direction * stats.rushSpeed, 0f, 0f);

        if (Mathf.Abs(transform.position.x - targetX) < 0.2f)
        {
            rushCounter++;
            rb.linearVelocity = Vector3.zero;

            if (rushCounter >= stats.diagonalRushCount)
            {
                if (!firstHoverDone)
                {
                    currentState = State.FirstHover;
                }
                else
                {
                    meteorCounter = 0;
                    currentState = State.Rising;
                }
            }
            else
            {
                movingToB = !movingToB;
            }
        }
    }

    IEnumerator FirstHoverCoroutine()
    {
        currentState = State.Waiting;
        firstHoverDone = true;

        transform.position = new Vector3(transform.position.x, stats.hoverHeight, 0f);
        yield return new WaitForSeconds(stats.waitBeforeMeteor);

        aimMoveCounter = 0;
        aimingToRight = true;
        currentState = State.Aiming;
    }

    void PerformAiming()
    {
        float targetX = aimingToRight ? stats.aimMoveRightX : stats.aimMoveLeftX;
        float direction = Mathf.Sign(targetX - transform.position.x);
        rb.linearVelocity = new Vector3(direction * stats.aimMoveSpeed, 0f, 0f);

        if (Mathf.Abs(transform.position.x - targetX) < 0.2f)
        {
            aimMoveCounter++;
            rb.linearVelocity = Vector3.zero;

            if (aimMoveCounter >= stats.aimMoveCount)
            {
                StartCoroutine(WaitAndDrop());
            }
            else
            {
                aimingToRight = !aimingToRight;
            }
        }
    }

    IEnumerator WaitAndDrop()
    {
        currentState = State.Waiting;
        yield return new WaitForSeconds(stats.waitBeforeMeteor);
        currentState = State.MeteorDrop;
        hasDropped = false; // Drop前にリセット
    }

    void PerformMeteorDrop()
    {
        if (hasDropped) return;

        hasDropped = true;
        Vector3 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * stats.meteorDropSpeed;
    }

    void PerformRising()
    {
        rb.linearVelocity = new Vector3(0f, stats.riseSpeed, 0f);
            
        if (transform.position.y >= stats.hoverHeight)
        {
            rb.linearVelocity = Vector3.zero;
            currentState = State.MeteorDrop;
            hasDropped = false; // Rising経由のMeteorDropでも一度だけ落下
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == State.MeteorDrop && collision.gameObject.CompareTag("Ground"))
        {
            rb.linearVelocity = Vector3.zero;
            hasDropped = false;
            meteorCounter++;

            if (meteorCounter >= stats.meteorDropCount)
            {
                rushCounter = 0;
                currentState = State.RushMove;
            }
            else
            {
                StartCoroutine(WaitAndRise());
            }
        }
    }

    IEnumerator WaitAndRise()
    {
        currentState = State.Waiting;
        yield return new WaitForSeconds(stats.waitBeforeMeteor);
        currentState = State.Rising;
    }
}
