using UnityEngine;
using System;

public class PlayerJump : MonoBehaviour
{
    // ==========================================================
    // 参照
    // ==========================================================
    private Rigidbody RigidBody;
    private PlayerState state;
    private PlayerMove move;
    private PlayerSpecialAction special;
    private PlayerAnimation playerAnimation;

    // ==========================================================
    // ジャンプ設定（Inspector表示）
    // ==========================================================
    [Header("ジャンプパワー")]
    public float groundJumpPower = 11f;
    public float landingLowJumpPower = 13f;
    public float landingHighJumpPower = 15f;
    public float maxJumpSpeed = 12f;
    [SerializeField] private AnimationCurve jumpCurve = new AnimationCurve();

    // ==========================================================
    // ジャンプ状態（Inspector表示）
    // ==========================================================
    [Header("ジャンプ状態")]
    public bool jumping = false;
    public int landingJumpNumber = 0;
    public bool jumpCoolActive = false;

    // ==========================================================
    // ジャンプ内部用（Inspector非表示）
    // ==========================================================
    private float jumpTime;
    private float coyoteTime = 0.13f;
    public float coyoteCounter = 0f;
    private float jumpCoolTime = 0.2f;
    private float jumpCoolCounter = 0f;
    private bool isJumpQueued = false;
    private float jumpQueueTime = 0.2f;
    private float jumpQueueCounter = 0f;
    private float jumpTimeMax = 0.2f;
    private float jumpTimeMaxSaving = 0.2f;

    // ==========================================================
    // トランポリン関連
    // ==========================================================
    [Header("トランポリン")]
    public bool isOnTrampoline = false;
    public float TrampolinePower = 1.5f;
    private bool TrampolineJumping = false;
    private float TrampolineGraceTime = 0.15f;
    private float TrampolineTimer = 0f;

    // ==========================================================
    // イベント
    // ==========================================================
    public event Action OnJumped;

    // ==========================================================
    // 初期化
    // ==========================================================
    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
        move = GetComponent<PlayerMove>();
        special = GetComponent<PlayerSpecialAction>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    // ==========================================================
    // 入力・ジャンプ処理
    // ==========================================================
    void Update()
    {
        JumpOperation();

        // ジャンプクールタイム管理
        if (jumpCoolActive)
        {
            jumpCoolCounter += Time.deltaTime;
            state.isGrounded = false;
            state.isJumpMoveOK = false;

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
            float JumpPower = landingJumpNumber >= 2 ? landingHighJumpPower :
                              landingJumpNumber == 1 ? landingLowJumpPower :
                              groundJumpPower;

            if (isOnTrampoline)
            {
                TrampolineJumping = true;
                TrampolineTimer = TrampolineGraceTime;
            }

            Jump(JumpPower);
        }

        // トランポリン効果の管理
        if (TrampolineJumping)
        {
            TrampolineTimer -= Time.fixedDeltaTime;
            if (TrampolineTimer <= 0)
            {
                TrampolineJumping = false;
            }
        }
    }

    // ==========================================================
    // ジャンプ操作判定
    // ==========================================================
    private void JumpOperation()
    {
        if (Input.GetKeyDown(state.keyBind.playerJump) && !special.meteorHighJumpOK && !isJumpQueued)
        {
            isJumpQueued = true;
            jumpQueueCounter = 0f;
        }

        if ((coyoteCounter <= coyoteTime || state.isJumpMoveOK) && !jumpCoolActive && special.highJumpChargeCounter < special.highJumpChargeTime)
        {
            if (isJumpQueued)
            {
                jumping = true;
                jumpCoolActive = true;
                jumpTime = 0f;
                jumpTimeMax = jumpTimeMaxSaving;
                isJumpQueued = false;
                OnJumped?.Invoke();

                if (state.landingJumpOn)
                {
                    state.LandingJumpReset();
                }
            }
            else if (Input.GetKey(state.keyBind.playerJump) && special.meteorHighJumpOK && state.landingJumpOn)
            {
                if (special.meteorDropCounter >= special.meteorDropTime)
                {
                    jumpCoolActive = true;
                    special.meteorHighJump = true;
                    landingJumpNumber++;
                    OnJumped?.Invoke();
                }
                special.meteorHighJumpOK = false;
                state.LandingJumpReset();
            }
        }

        if (jumping)
        {
            if (Input.GetKeyUp(state.keyBind.playerJump) && jumpTime <= jumpTimeMaxSaving * 0.5f)
            {
                jumpTimeMax = jumpTimeMaxSaving * 0.5f;
            }
        }

        if (isJumpQueued)
        {
            jumpQueueCounter += Time.deltaTime;
            if (jumpQueueCounter > jumpQueueTime)
            {
                isJumpQueued = false;
            }
        }
    }

    // ==========================================================
    // ジャンプ実行
    // ==========================================================
    public void Jump(float jumpPower)
    {
        RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0, RigidBody.linearVelocity.z);

        if (TrampolineJumping)
            jumpPower *= TrampolinePower;

        float time = jumpTime / jumpTimeMaxSaving;
        float power = jumpPower * jumpCurve.Evaluate(time);

        if (jumpTime >= jumpTimeMax)
            jumping = false;

        RigidBody.AddForce(power * Vector3.up, ForceMode.Impulse);

        Vector3 horizontalVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, 0f);
        if (horizontalVelocity.magnitude > maxJumpSpeed)
        {
            RigidBody.linearVelocity = new Vector3(Mathf.Sign(RigidBody.linearVelocity.x) * maxJumpSpeed,
                                                   RigidBody.linearVelocity.y,
                                                   RigidBody.linearVelocity.z);
        }
    }
}
