using UnityEngine;

public class LongThornyCudgelController : MonoBehaviour
{
    [Header("����N�[���^�C���i�b�j")]
    public float initialCooldown = 2f;  // �Q�[���J�n���Ɉ�x����

    [Header("�e�t�F�[�Y���ԁi�b�j")]
    public float cooldown1 = 1f;             // shortStretch�O
    public float shortStretchDuration = 0.2f;
    public float cooldown2 = 1f;             // longStretch�O
    public float longStretchDuration = 2f;
    public float cooldown3 = 1f;             // shrink�O
    public float shrinkDuration = 4f;

    [Header("�ړ�����")]
    public float shortStretchDistance = 2f;
    public float longStretchDistance = 75f;

    [Header("��]���x�i�x/�b�j")]
    public float stretchRotationSpeed = 360f;   // ����]
    public float shrinkRotationSpeed = 180f;    // �E��]

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
                MoveTowardsTarget(() =>
                {
                    shortStretchEnd = transform.localPosition;
                    timer = 0f;
                    currentState = State.Cooldown2;
                });
                Rotate(-stretchRotationSpeed);
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
                Rotate(-stretchRotationSpeed);
                break;

            case State.Cooldown3:
                if (TimerReached(cooldown3)) StartShrinking();
                break;

            case State.Shrinking:
                MoveTowardsTarget(() =>
                {
                    timer = 0f;
                    currentState = State.Cooldown1;  // �ă��[�v
                });
                Rotate(shrinkRotationSpeed);
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

    void Rotate(float speed)
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);  // ���[�J�����ŉ�]
    }

    bool TimerReached(float duration)
    {
        timer += Time.deltaTime;
        return timer >= duration;
    }
}
