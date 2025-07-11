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

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private float jumpKeyHoldTime = 0f;
    private float crouchThreshold = 0.2f;

    private float jumpAnimTimer = 0f;
    private float jumpAnimDuration = 0.5f;

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

        // ジャンプアニメ維持タイマー
        if (isJumping)
        {
            jumpAnimTimer -= Time.deltaTime;
            if (jumpAnimTimer <= 0f)
                isJumping = false;
        }

        // 着地検出
        if (isGrounded && !wasGrounded)
        {
            Debug.Log("着地しました");
            isJumping = false;
        }
        wasGrounded = isGrounded;

        // 向き制御（しゃがみ中は向き固定）
        if (!crouchStarted)
        {
            if (playerMove.MoveInput > 0f)
                transform.rotation = Quaternion.Euler(0, 90, 0);
            else if (playerMove.MoveInput < 0f)
                transform.rotation = Quaternion.Euler(0, -90, 0);
        }

        // quickJumpUsed の立ち上がり（フレーム初めて true）検出
        bool quickJumpUsedThisFrame = playerSpecial.quickJumpUsed && !prevQuickJumpUsed;

        // ===== ジャンプアニメ再生 =====

        if (playerSpecial.playHighJumpAnim && !isJumping)
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
        else if (playerSpecial.meteorDrop && !isJumping)
        {
            animator.SetTrigger("MeteorDrop");
            isJumping = true;
            jumpAnimTimer = jumpAnimDuration;
        }
        else if (quickJumpUsedThisFrame && !isJumping) 
        {
            Debug.Log("クイックジャンプアニメ");

            ResetJumpTriggers();
            animator.SetTrigger("JumpAnimStep1");

            isJumping = true;
            jumpAnimTimer = jumpAnimDuration;
        }
        else if (Input.GetKeyDown(playerState.keyBind.playerJump) && isGrounded && !isJumping)
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

            isJumping = true;
            jumpAnimTimer = jumpAnimDuration;

            Debug.Log($"ジャンプアニメ Step {jumpType}");
        }

        // ===== しゃがみチャージ制御 =====

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

        // しゃがみ解除条件
        bool shouldCancelCrouch = crouchStarted && (!isJumpKeyPressed || isJumping);
        if (shouldCancelCrouch)
        {
            animator.SetTrigger("EndCrouch");
            animator.SetBool("IsCrouching", false);
            crouchStarted = false;
            jumpKeyHoldTime = 0f;
        }

        // ===== Animator Bool 更新 =====

        animator.SetBool("IsCrouching", crouchStarted);
        animator.SetBool("IsIdle", !hasMoveInput && !isJumpKeyPressed && isGrounded && !isJumping);
        animator.SetBool("IsRunning", isGrounded && hasMoveInput && !isJumpKeyPressed);
        animator.SetBool("IsGrounded", isGrounded);

        // 最後に quickJumpUsed の状態を保存
        prevQuickJumpUsed = playerSpecial.quickJumpUsed;
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void ResetJumpTriggers()
    {
        animator.ResetTrigger("HighJump");
        animator.ResetTrigger("JumpAnimStep1");
        animator.ResetTrigger("JumpAnimStep2");
        animator.ResetTrigger("JumpAnimStep3");
        animator.ResetTrigger("MeteorDrop");
    }
}
