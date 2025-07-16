using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;

    private PlayerMove playerMove;
    private PlayerState playerState;
    private PlayerSpecialAction playerSpecial;
    private PlayerJump playerJump;

    private bool wasGrounded = true;
    private bool isJumping = false;
    private bool crouchStarted = false;
    private bool prevQuickJumpUsed = false;
    private bool isMeteorDropping = false;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    // 壁判定用
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckDistance = 0.3f; // 壁との距離判定
    [SerializeField] private float wallCheckHeight = 0.5f;   // 判定の高さ調整（キャラの腰あたり）

    private float jumpKeyHoldTime = 0f;
    private float crouchThreshold = 0.2f;

    private float jumpAnimTimer = 0f;
    private float jumpAnimDuration = 0.5f;

    private float groundedVelocityLockTime = 0.2f;
    private float groundedVelocityLockTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMove = GameObject.Find("PlayerModel").GetComponent<PlayerMove>();
        playerState = GameObject.Find("PlayerModel").GetComponent<PlayerState>();
        playerSpecial = GameObject.Find("PlayerModel").GetComponent<PlayerSpecialAction>();
        playerJump = GameObject.Find("PlayerModel").GetComponent<PlayerJump>();

        if (animator == null)
            Debug.LogError("Animator が見つかりません。コンポーネントをアタッチしてください。");
    }

    void Update()
    {
        bool isGrounded = IsGrounded();
        bool hasMoveInput = Mathf.Abs(playerMove.MoveInput) > 0f;
        bool isJumpKeyPressed = Input.GetKey(playerState.keyBind.playerJump);

        Rigidbody rb = playerMove.GetComponent<Rigidbody>();
        float verticalVelocity = rb.linearVelocity.y;

        if (isJumping)
        {
            jumpAnimTimer -= Time.deltaTime;
            if (jumpAnimTimer <= 0f && !isMeteorDropping)
                isJumping = false;
        }

        // クイックジャンプ処理
        bool quickJumpUsedThisFrame = playerSpecial.quickJumpUsed && !prevQuickJumpUsed;
        if (quickJumpUsedThisFrame && !isMeteorDropping)
        {
            Debug.Log("クイックジャンプアニメ");

            ResetJumpTriggers();
            animator.Play("JumpAnimStep1");

            isJumping = true;
            jumpAnimTimer = jumpAnimDuration;
            crouchStarted = false;
            jumpKeyHoldTime = 0f;

            playerSpecial.quickJumpUsed = false;
            prevQuickJumpUsed = true;
            return;
        }

        // メテオドロップ
        if (playerSpecial.meteorDrop && !isMeteorDropping && !isGrounded)
        {
            Debug.Log("メテオドロップ開始");

            ResetJumpTriggers();
            animator.SetBool("IsMeteorDropping", true);

            isMeteorDropping = true;
            isJumping = true;

            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;

            rb.AddForce(Vector3.down * 30f, ForceMode.VelocityChange);

            playerSpecial.meteorDrop = false;
        }

        // ハイジャンプ
        if (playerSpecial.playHighJumpAnim && !isMeteorDropping)
        {
            Debug.Log("ハイジャンプ演出開始");

            ResetJumpTriggers();
            animator.SetTrigger("HighJump");
            animator.SetBool("IsCrouching", false);

            isJumping = true;
            jumpAnimTimer = jumpAnimDuration;
            crouchStarted = false;
            jumpKeyHoldTime = 0f;

            playerSpecial.playHighJumpAnim = false;
        }

        // 通常ジャンプ・着地ジャンプ
        else if (Input.GetKeyDown(playerState.keyBind.playerJump) && isGrounded && !isMeteorDropping)
        {
            int jumpType = 1;

            if (playerJump != null)
            {
                if (playerJump.landingJumpNumber >= 2)
                    jumpType = 3;
                else if (playerJump.landingJumpNumber == 1)
                    jumpType = 2;
            }

            ResetJumpTriggers();
            animator.SetTrigger($"JumpAnimStep{jumpType}");
            animator.SetBool("IsFalling", false);

            isJumping = true;
            jumpAnimTimer = jumpAnimDuration;

            Debug.Log($"ジャンプアニメ Step {jumpType}");
        }

        // 落下判定
        bool isFalling = !isGrounded && !isJumping && !isMeteorDropping && verticalVelocity < -0.1f;
        animator.SetBool("IsFalling", isFalling);

        // 着地処理
        if (isGrounded && !wasGrounded)
        {
            Debug.Log("着地しました");

            groundedVelocityLockTimer = groundedVelocityLockTime;

            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;
            rb.angularVelocity = Vector3.zero;

            isJumping = false;
            crouchStarted = false;

            ResetJumpTriggers();
            animator.ResetTrigger("StartCrouch");
            animator.ResetTrigger("EndCrouch");
            animator.SetBool("IsCrouching", false);
            animator.SetBool("IsMeteorDropping", false);
            animator.SetBool("IsFalling", false);
            isMeteorDropping = false;

            // animator.Play("Idle");  // ここは必要なら残してもOKですが、
            // 状態遷移はパラメータ管理に任せる方が安定します
        }

        // Y軸速度ロック
        if (groundedVelocityLockTimer > 0f)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;

            groundedVelocityLockTimer -= Time.deltaTime;
        }

        wasGrounded = isGrounded;

        // 向きの制御
        if (!crouchStarted && !isJumping && !isMeteorDropping)
        {
            if (playerMove.MoveInput > 0f)
                transform.rotation = Quaternion.Euler(0, 90, 0);
            else if (playerMove.MoveInput < 0f)
                transform.rotation = Quaternion.Euler(0, -90, 0);
        }

        // しゃがみ開始
        if (isJumpKeyPressed && isGrounded && !isJumping)
        {
            jumpKeyHoldTime += Time.deltaTime;

            if (jumpKeyHoldTime >= crouchThreshold && !crouchStarted)
            {
                animator.SetTrigger("StartCrouch");
                crouchStarted = true;
            }
        }
        else if (!crouchStarted)
        {
            jumpKeyHoldTime = 0f;
        }

        // しゃがみ解除
        bool shouldCancelCrouch = crouchStarted && (!isJumpKeyPressed || isJumping);
        if (shouldCancelCrouch)
        {
            animator.SetTrigger("EndCrouch");
            animator.SetBool("IsCrouching", false);
            crouchStarted = false;
            jumpKeyHoldTime = 0f;
        }

        // 壁判定
        bool touchingWall = IsTouchingWall();

        // Animator パラメータ更新（壁接触時はアイドルにしない）
        animator.SetBool("IsIdle", isGrounded && !hasMoveInput && !isJumpKeyPressed && !isJumping && !isMeteorDropping && !touchingWall);
        animator.SetBool("IsRunning", isGrounded && hasMoveInput && !isJumpKeyPressed);
        animator.SetBool("IsCrouching", crouchStarted);
        animator.SetBool("IsGrounded", isGrounded);

        prevQuickJumpUsed = playerSpecial.quickJumpUsed;
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private bool IsTouchingWall()
    {
        Vector3 origin = transform.position + Vector3.up * wallCheckHeight;

        bool rightHit = Physics.Raycast(origin, transform.right, wallCheckDistance, wallLayer);
        bool leftHit = Physics.Raycast(origin, -transform.right, wallCheckDistance, wallLayer);

        // デバッグ用にレイを可視化（Sceneビューで見れます）
        Debug.DrawRay(origin, transform.right * wallCheckDistance, Color.red);
        Debug.DrawRay(origin, -transform.right * wallCheckDistance, Color.red);

        return rightHit || leftHit;
    }

    private void ResetJumpTriggers()
    {
        animator.ResetTrigger("HighJump");
        animator.ResetTrigger("JumpAnimStep1");
        animator.ResetTrigger("JumpAnimStep2");
        animator.ResetTrigger("JumpAnimStep3");
        animator.ResetTrigger("MeteorDrop");
        animator.ResetTrigger("EndCrouch");
        animator.SetBool("IsFalling", false); // 落下を止める
    }
}
