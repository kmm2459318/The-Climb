using UnityEngine;

public class ShortThornyCudgelController : MonoBehaviour
{
    [Header("初回クールタイム（秒）")]
    public float initialCooldown = 1f;

    [Header("各フェーズ時間（秒）")]
    public float cooldown1 = 4f;
    public float shortStretchDuration = 0.25f;
    public float longStretchDuration = 0.25f;
    public float cooldown3 = 0f;
    public float shrinkDuration = 1.5f;

    [Header("移動距離")]
    public float shortStretchDistance = 2f;
    public float longStretchDistance = 23f;

    [Header("回転速度（度/秒）")]
    public float stretchRotationSpeed = 360f;  // 左回転
    public float shrinkRotationSpeed = 180f;   // 右回転

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
    private Vector3 targetLocalPosition;

    private float timer = 0f;
    private float moveSpeed = 0f;

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
                float t1 = Mathf.Clamp01(timer / shortStretchDuration);
                float easedT1 = Mathf.SmoothStep(0f, 1f, t1);
                transform.localPosition = Vector3.Lerp(shortStretchStart, shortStretchEnd, easedT1);
                Rotate(stretchRotationSpeed); // 左回転
                timer += Time.deltaTime;
                if (t1 >= 1f) StartLongStretch();
                break;

            case State.LongStretch:
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPosition, moveSpeed * Time.deltaTime);
                Rotate(stretchRotationSpeed); // 左回転
                if (Vector3.Distance(transform.localPosition, targetLocalPosition) < 0.01f)
                {
                    longStretchEnd = transform.localPosition;
                    timer = 0f;
                    currentState = State.Cooldown3;
                }
                break;

            case State.Cooldown3:
                if (TimerReached(cooldown3)) StartShrinking();
                break;

            case State.Shrinking:
                float t3 = Mathf.Clamp01(timer / shrinkDuration);
                transform.localPosition = Vector3.Lerp(longStretchEnd, baseLocalPosition, t3);
                Rotate(-shrinkRotationSpeed); // 右回転
                timer += Time.deltaTime;
                if (t3 >= 1f)
                {
                    timer = 0f;
                    transform.localRotation = Quaternion.identity;
                    currentState = State.Cooldown1;
                }
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
        currentState = State.Shrinking;
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
