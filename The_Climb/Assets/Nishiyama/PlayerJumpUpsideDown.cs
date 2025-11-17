using UnityEngine;

public class PlayerJumpUpsideDown : MonoBehaviour
{
    Rigidbody rb;
    PlayerState state;
    PlayerMove move;

    // 基本ジャンプパラメータ
    public bool jumping = false;
    private float coyoteTime = 0.13f;
    private float coyoteCounter = 0f;

    private float jumpCoolTime = 0.2f;
    private float jumpCoolCounter = 0f;
    private bool jumpCoolActive = false;

    private bool isJumpQueued = false;
    private float jumpQueueTime = 0.2f;
    private float jumpQueueCounter = 0f;

    private float jumpTime;
    private float jumpTimeMax = 0.2f;
    private float jumpTimeMaxSaving = 0.2f;

    private float groundJumpPower = 11f;
    private float maxJumpSpeed = 12f;

    [SerializeField] AnimationCurve jumpCurve = new();

    // 着地ジャンプ（3段ジャンプ）
    public int landingJumpNumber = 0;
    private float landingLowJumpPower = 13f;
    private float landingHighJumpPower = 15f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
        move = GetComponent<PlayerMove>();
    }

    void Update()
    {
        JumpInput();
        CoyoteUpdate();

        // クールタイム
        if (jumpCoolActive)
        {
            jumpCoolCounter += Time.deltaTime;
            if (jumpCoolCounter > jumpCoolTime)
            {
                jumpCoolActive = false;
                jumpCoolCounter = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (jumping)
        {
            jumpTime += Time.fixedDeltaTime;

            float jumpPower = GetJumpPower();
            ExecuteJump(jumpPower);
        }
    }

    // -------------------------------------------------------
    // 入力処理
    // -------------------------------------------------------
    void JumpInput()
    {
        // 先行入力
        if (state.inputManager.jumpDown && !isJumpQueued)
        {
            isJumpQueued = true;
            jumpQueueCounter = 0f;
        }

        // ジャンプ可能判定
        if ((coyoteCounter <= coyoteTime || state.isJumpMoveOK) && !jumpCoolActive)
        {
            if (isJumpQueued)
            {
                StartJump();
            }
        }

        // ジャンプボタンが離されたら最大ジャンプ時間を短縮
        if (jumping)
        {
            if (state.inputManager.jumpUp && jumpTime <= jumpTimeMaxSaving * 0.5f)
            {
                jumpTimeMax = jumpTimeMaxSaving * 0.5f;
            }
        }

        // 先行入力カウント
        if (isJumpQueued)
        {
            jumpQueueCounter += Time.deltaTime;
            if (jumpQueueCounter > jumpQueueTime)
                isJumpQueued = false;
        }
    }

    void StartJump()
    {
        jumping = true;
        jumpCoolActive = true;
        jumpTime = 0f;
        jumpTimeMax = jumpTimeMaxSaving;
        isJumpQueued = false;

        // 着地ジャンプ（最大3段）
        if (state.landingJumpOn)
        {
            landingJumpNumber++;
            state.LandingJumpReset();
        }
        else
        {
            landingJumpNumber = 0;
        }
    }

    // -------------------------------------------------------
    // ジャンプ力の判定（通常／着地ジャンプ）
    // -------------------------------------------------------
    float GetJumpPower()
    {
        if (landingJumpNumber >= 2)
            return landingHighJumpPower;

        if (landingJumpNumber == 1)
            return landingLowJumpPower;

        return groundJumpPower;
    }

    // -------------------------------------------------------
    // 実際のジャンプ実行（上下反転対応）
    // -------------------------------------------------------
    void ExecuteJump(float jumpPower)
    {
        // Y速度リセット
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        float time = jumpTime / jumpTimeMaxSaving;
        float power = jumpPower * jumpCurve.Evaluate(time);

        if (jumpTime >= jumpTimeMax)
            jumping = false;

        // 反転ジャンプの方向（普通はup、反転時はdown）
        Vector3 dir = move.IsUpsideDown ? Vector3.down : Vector3.up;

        rb.AddForce(dir * power, ForceMode.Impulse);

        // 横方向制限
        Vector3 h = new(rb.linearVelocity.x, 0f, 0f);
        if (h.magnitude > maxJumpSpeed)
            rb.linearVelocity = new Vector3(Mathf.Sign(rb.linearVelocity.x) * maxJumpSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
    }

    // -------------------------------------------------------
    // コヨーテタイム更新
    // -------------------------------------------------------
    void CoyoteUpdate()
    {
        if (state.isGrounded)
            coyoteCounter = 0f;
        else
            coyoteCounter += Time.deltaTime;
    }
}
