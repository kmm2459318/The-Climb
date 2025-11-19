using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    Rigidbody RigidBody;
    PlayerState state;
    PlayerMove move;
    PlayerSpecialAction special;
    PlayerKnockBack knock;

    [Header("ジャンプ設定")]
    public bool jumping = false; 　//ジャンプ入力中判定
    private float coyoteTime = 0.13f;　//コヨーテタイム
    public float coyoteCounter = 0f;　//コヨーテタイムカウント
    private float jumpCoolTime = 0.2f;　//ジャンプのクールタイム
    private float jumpCoolCounter = 0f;　//ジャンプのクールタイムカウント
    public bool jumpCoolActive = false;　//ジャンクールタイムを始める用判定
    private bool isJumpQueued = false;　//ジャンプキーが押された判定
    private float jumpQueueTime = 0.2f;　//ジャンプ選考入力猶予時間
    private float jumpQueueCounter = 0f;　//ジャンプ選考入力カウンター
    private float jumpTime;　　　　　　　//ジャンプ入力時間　
    private float jumpTimeMax = 0.2f;　//最大ジャンプ入力時間
    private float jumpTimeMaxSaving = 0.2f;　//最大図アンプ入力時間を保持
    private float groundJumpPower = 13f;　//ジャンプでプレイヤーにかかる上方向の力
    private float maxJumpSpeed = 12f;　//空中での速度制限
    [SerializeField] AnimationCurve jumpCurve = new();　//ジャンプの速度カーブ

    [Header("着地ジャンプ")]
    public int landingJumpNumber = 0;　//着地ジャンプの連続回数
    private float landingLowJumpPower = 15f;　//一回目着地ジャンプのパワー
    private float landingHighJumpPower = 17f;　//二回目の着地ジャンプパワー

    [Header("トランポリン")]
    public bool isOnTrampoline = false;　//トランポリンに乗ってるかの判定
    public float TrampolinePower = 1.5f;　//トランポリンの倍率
    private float TrampolineGraceTime = 0.15f;　//トランポリンの効果を維持する時間
    private float TrampolineTimer = 0f;　//トランポリンの効果を管理するタイマー
    private bool TrampolineJumping = false;　//トランポリンのジャンプ中判定

    [Header("重力設定")]
    public float gravityPower = 20f; // 自前重力
    private Vector3 gravityDirection = Vector3.down;

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
        move = GetComponent<PlayerMove>();
        special = GetComponent<PlayerSpecialAction>();
        knock = GetComponent<PlayerKnockBack>();
    }

    void Update()
    {
        if (!knock.knockBacking)
            JumpOperation();

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

    private void FixedUpdate()
    {
        　//反転してる時だけ自前重力
        if (move.IsUpsideDown)
        {
            RigidBody.useGravity = false;

            // 反転時のカスタム重力
            Vector3 gravityDirection = Vector3.up;
            RigidBody.AddForce(gravityDirection * gravityPower, ForceMode.Acceleration);
        }
        else
        {
            // 通常時はUnityの重力に完全任せる
            RigidBody.useGravity = true;
        }

        // ジャンプ
        if (jumping)
        {
            jumpTime += Time.fixedDeltaTime;
            float JumpPower = groundJumpPower;

            special.headingAttack.SetActive(true);

            if (!state.carryingBuddy)
            {
                if (landingJumpNumber >= 2)
                    JumpPower = landingHighJumpPower;
                else if (landingJumpNumber == 1)
                    JumpPower = landingLowJumpPower;
            }

            if (isOnTrampoline)
            {
                TrampolineJumping = true;
                TrampolineTimer = TrampolineGraceTime;
            }

            Jump(JumpPower);
        }

        if (TrampolineJumping)
        {
            TrampolineTimer -= Time.fixedDeltaTime;
            if (TrampolineTimer <= 0)
                TrampolineJumping = false;
        }
    }

    private void JumpOperation()
    {
        if (state.inputManager.jumpDown && !special.meteorHighJumpOK && !isJumpQueued)
        {
            isJumpQueued = true;
            jumpQueueCounter = 0f;
        }

        if (((coyoteCounter <= coyoteTime || state.isJumpMoveOK) || (move.IsUpsideDown && state.isGrounded))
             && !jumpCoolActive
             && special.highJumpChargeCounter < special.highJumpChargeTime)
        {
            if (isJumpQueued)
            {
                jumping = true;
                jumpCoolActive = true;
                jumpTime = 0f;
                jumpTimeMax = jumpTimeMaxSaving;
                isJumpQueued = false;

                if (state.landingJumpOn)
                {
                    landingJumpNumber++;
                    state.LandingJumpReset();
                }
            }
            else if (state.inputManager.jumpHeld && special.meteorHighJumpOK && state.landingJumpOn)
            {
                if (special.meteorDropCounter >= special.meteorDropTime)
                {
                    jumpCoolActive = true;
                    special.meteorHighJump = true;
                    landingJumpNumber++;
                    special.headingAttack.SetActive(true);
                }
                special.meteorHighJumpOK = false;
                state.LandingJumpReset();
            }
        }

        if (jumping && state.inputManager.jumpUp && jumpTime <= jumpTimeMaxSaving * 0.5f)
        {
            jumpTimeMax = jumpTimeMaxSaving * 0.5f;
        }

        if (isJumpQueued)
        {
            jumpQueueCounter += Time.deltaTime;
            if (jumpQueueCounter > jumpQueueTime)
                isJumpQueued = false;
        }
    }

    public void Jump(float jumpPower)
    {
        // Y方向速度をリセット
        Vector3 vel = RigidBody.linearVelocity;
        vel.y = 0f;
        RigidBody.linearVelocity = vel;

        if (TrampolineJumping)
            jumpPower *= TrampolinePower;

        float time = jumpTime / jumpTimeMaxSaving;
        float power = jumpPower * jumpCurve.Evaluate(time);

        if (jumpTime >= jumpTimeMax)
            jumping = false;

        // 上下反転ジャンプ
        Vector3 jumpDirection = move.IsUpsideDown ? Vector3.down : Vector3.up;

        RigidBody.AddForce(power * jumpDirection, ForceMode.Impulse);

        // 横速度制限
        Vector3 horizontalVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, RigidBody.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxJumpSpeed)
            RigidBody.linearVelocity = new Vector3(Mathf.Sign(RigidBody.linearVelocity.x) * maxJumpSpeed, RigidBody.linearVelocity.y, RigidBody.linearVelocity.z);
    }
}
