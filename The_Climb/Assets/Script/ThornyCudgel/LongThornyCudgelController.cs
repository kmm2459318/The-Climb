using UnityEngine;

public class LongThornyCudgelController : MonoBehaviour
{
    [Header("初回クールタイム（秒）")]
    public float initialCooldown = 1f;

    [Header("各フェーズ時間（秒）")]
    public float cooldown1 = 1f;
    public float shortStretchDuration = 0.2f;
    public float cooldown2 = 1f;
    public float longStretchDuration = 4f;
    public float cooldown3 = 1f;
    public float shrinkDuration = 6f;

    [Header("移動距離")]
    public float shortStretchDistance = 2f;
    public float longStretchDistance = 150f;

    [Header("回転速度（度/秒）")]
    public float stretchRotationSpeed = 360f;  // 左回転
    public float shrinkRotationSpeed = 180f;   // 右回転

    private enum State
    {
        InitialCooldown,
        Cooldown1,
        ShortStretch,
        Cooldown2,
        LongStretch,
        Cooldown3,
        Shrinking
    }

    private State currentState = State.InitialCooldown;

    private Vector3 baseLocalPosition;
    private Vector3 shortStretchEnd;
    private Vector3 longStretchEnd;

    private float timer = 0f;
    private float moveSpeed = 0f;
    private Vector3 targetLocalPosition;

    private float currentYRotation = 0f;

    void Start()
    {
        baseLocalPosition = transform.localPosition;
        currentYRotation = transform.localEulerAngles.y;
        timer = 0f;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.InitialCooldown:
                if (TimerReached(initialCooldown))
                {
                    timer = 0f;
                    currentState = State.Cooldown1;
                }
                break;

            case State.Cooldown1:
                if (TimerReached(cooldown1)) StartShortStretch();
                break;

            case State.ShortStretch:
                MoveTowardsTarget(() =>
                {
                    shortStretchEnd = transform.localPosition;
                    timer = 0f;
                    currentState = State.Cooldown2;
                });
                RotateContinuous(stretchRotationSpeed);
                break;

            case State.Cooldown2:
                if (TimerReached(cooldown2)) StartLongStretch();
                break;

            case State.LongStretch:
                MoveTowardsTarget(() =>
                {
                    longStretchEnd = transform.localPosition;
                    timer = 0f;
                    currentState = State.Cooldown3;
                });
                RotateContinuous(stretchRotationSpeed);
                break;

            case State.Cooldown3:
                if (TimerReached(cooldown3)) StartShrinking();
                break;

            case State.Shrinking:
                timer += Time.deltaTime;
                MoveTowardsTarget(() =>
                {
                    timer = 0f;
                    currentState = State.Cooldown1;
                    // 縮み終わりで回転リセット
                    currentYRotation = 0f;
                    ApplyRotation();
                });
                RotateContinuous(-shrinkRotationSpeed);
                break;
        }
    }

    void StartShortStretch()
    {
        timer = 0f;
        targetLocalPosition = baseLocalPosition + Vector3.up * shortStretchDistance;
        moveSpeed = shortStretchDistance / shortStretchDuration;
        currentState = State.ShortStretch;
    }

    void StartLongStretch()
    {
        timer = 0f;
        targetLocalPosition = shortStretchEnd + Vector3.up * longStretchDistance;
        moveSpeed = longStretchDistance / longStretchDuration;
        currentState = State.LongStretch;
    }

    void StartShrinking()
    {
        timer = 0f;
        targetLocalPosition = baseLocalPosition;
        float totalDistance = Vector3.Distance(transform.localPosition, baseLocalPosition);
        moveSpeed = totalDistance / shrinkDuration;
        currentState = State.Shrinking;
    }

    void MoveTowardsTarget(System.Action onComplete)
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.localPosition, targetLocalPosition) < 0.01f)
        {
            onComplete?.Invoke();
        }
    }

    void RotateContinuous(float speed)
    {
        currentYRotation += speed * Time.deltaTime;
        currentYRotation %= 360f;
        ApplyRotation();
    }

    void ApplyRotation()
    {
        Vector3 euler = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(euler.x, currentYRotation, euler.z);
    }

    bool TimerReached(float duration)
    {
        timer += Time.deltaTime;
        return timer >= duration;
    }
}
