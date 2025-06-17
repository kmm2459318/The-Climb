using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BossEnemy_DropMovement : MonoBehaviour
{
    public DropStats stats;
    private Rigidbody rb;

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

    private bool movingToB = true; // true = A→B, false = B→A


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
                // 待機中（コルーチン）
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
                movingToB = !movingToB; // 方向反転して再開
            }
        }
    }

    IEnumerator FirstHoverCoroutine()
    {
        currentState = State.Waiting;
        firstHoverDone = true;

        // Y方向に上昇させてから停止
        transform.position = new Vector3(transform.position.x, stats.hoverHeight, 0f);
        yield return new WaitForSeconds(stats.waitBeforeMeteor);

        aimMoveCounter = 0;
        aimingToRight = true;
        currentState = State.Aiming;
    }

    void PerformMeteorDrop()
    {
        rb.linearVelocity = new Vector3(0f, -stats.meteorDropSpeed, 0f);

        if (transform.position.y <= stats.groundY)
        {
            meteorCounter++;
            rb.linearVelocity = Vector3.zero;

            if (meteorCounter >= stats.meteorDropCount)
            {
                rushCounter = 0;
                movingToB = true; // Aに戻ってまた往復
                transform.position = new Vector3(stats.pointA_X, stats.groundY, 0f);
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

    void PerformRising()
    {
        rb.linearVelocity = new Vector3(0f, stats.riseSpeed, 0f);

        if (transform.position.y >= stats.hoverHeight)
        {
            rb.linearVelocity = Vector3.zero;
            currentState = State.MeteorDrop;
        }
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
    }


}
