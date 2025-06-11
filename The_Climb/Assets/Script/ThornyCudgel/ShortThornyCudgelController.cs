using UnityEngine;

public class ShortThornyCudgelController : MonoBehaviour
{
    [Header("初回クールタイム（秒）")]
    public float initialCooldown = 2f;

    [Header("各フェーズ時間（秒）")]
    public float cooldown1 = 1f;
    public float shortStretchDuration = 0.2f;
    public float longStretchDuration = 2f;
    public float cooldown3 = 1f;
    public float shrinkDuration = 4f;

    [Header("移動距離")]
    public float shortStretchDistance = 2f;
    public float longStretchDistance = 75f;

    [Header("回転速度（度/秒）")]
    public float stretchRotationSpeed = 360f;
    public float shrinkRotationSpeed = 180f;

    private enum State
    {
        InitialCooldown,
        Cooldown1,
        ShortStretch,
        LongStretch,
        Cooldown3,
        Shrinking
    }

    private State currentState = State.InitialCooldown;

    private Vector3 baseLocalPosition;
    private Vector3 shortStretchStart;
    private Vector3 shortStretchEnd;
    private Vector3 longStretchEnd;

    private float timer = 0f;
    private float moveSpeed = 0f;
    private Vector3 targetLocalPosition;

    void Start()
    {
        baseLocalPosition = transform.localPosition;
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
                float t = Mathf.Clamp01(timer / shortStretchDuration);
                transform.localPosition = Vector3.Lerp(shortStretchStart, shortStretchEnd, t);
                Rotate(-stretchRotationSpeed);
                timer += Time.deltaTime;
                if (t >= 1f)
                {
                    StartLongStretch();
                }
                break;

            case State.LongStretch:
                MoveTowardsTarget(() =>
                {
                    longStretchEnd = transform.localPosition;
                    timer = 0f;
                    currentState = State.Cooldown3;
                });
                Rotate(-stretchRotationSpeed);
                break;

            case State.Cooldown3:
                if (TimerReached(cooldown3)) StartShrinking();
                break;

            case State.Shrinking:
                MoveTowardsTarget(() =>
                {
                    timer = 0f;
                    currentState = State.Cooldown1;  // 再ループ
                });
                Rotate(shrinkRotationSpeed);
                break;
        }
    }

    void StartShortStretch()
    {
        timer = 0f;
        shortStretchStart = baseLocalPosition;
        shortStretchEnd = baseLocalPosition + Vector3.up * shortStretchDistance;
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

    void Rotate(float speed)
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);
    }

    bool TimerReached(float duration)
    {
        timer += Time.deltaTime;
        return timer >= duration;
    }
}
