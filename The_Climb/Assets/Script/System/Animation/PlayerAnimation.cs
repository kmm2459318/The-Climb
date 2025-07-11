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

        if (isJumping)
        {
            jumpAnimTimer -= Time.deltaTime;
            if (jumpAnimTimer <= 0f && !isMeteorDropping)
                isJumping = false;
        }

        if (isGrounded && !wasGrounded)
        {
            Debug.Log("着地しました");

            groundedVelocityLockTimer = groundedVelocityLockTime;

            Rigidbody rb = playerMove.GetComponent<Rigidbody>();
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;
            rb.angularVelocity = Vector3.zero;

            isJumping = false;
            crouchStarted = false;

            if (isMeteorDropping)
            {
                animator.SetBool("IsMeteorDropping", false);
                isMeteorDropping = false;
            }

            animator.SetBool("IsIdle", true);
            animator.SetBool("IsRunning", false);
            //animator.SetBool("IsCrouching", false);
        }

        if (groundedVelocityLockTimer > 0f)
        {
            Rigidbody rb = playerMove.GetComponent<Rigidbody>();
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;

            groundedVelocityLockTimer -= Time.deltaTime;
        }

        wasGrounded = isGrounded;

        if (!crouchStarted)
        {
            if (playerMove.MoveInput > 0f)
                transform.rotation = Quaternion.Euler(0, 90, 0);
            else if (playerMove.MoveInput < 0f)
                transform.rotation = Quaternion.Euler(0, -90, 0);
        }

        bool quickJumpUsedThisFrame = playerSpecial.quickJumpUsed && !prevQuickJumpUsed;

        if (playerSpecial.meteorDrop && !isMeteorDropping && !isGrounded)
        {
            Debug.Log("メテオドロップ開始");

            ResetJumpTriggers();
            animator.SetBool("IsMeteorDropping", true);

            isMeteorDropping = true;
            isJumping = true;

            Rigidbody rb = playerMove.GetComponent<Rigidbody>();
            Vector3 vel = rb.linearVelocity;
            vel.y = -20f;
            rb.linearVelocity = vel;

            playerSpecial.meteorDrop = false;
        }

        if (playerSpecial.playHighJumpAnim && !isJumping && !isMeteorDropping)
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
        else if (quickJumpUsedThisFrame && !isJumping && !isMeteorDropping)
        {
            Debug.Log("クイックジャンプアニメ");

            ResetJumpTriggers();
            animator.SetTrigger("JumpAnimStep1");

            isJumping = true;
            jumpAnimTimer = jumpAnimDuration;
        }
        else if (Input.GetKeyDown(playerState.keyBind.playerJump) && isGrounded && !isJumping && !isMeteorDropping)
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

        bool shouldCancelCrouch = crouchStarted && (!isJumpKeyPressed || isJumping);
        if (shouldCancelCrouch)
        {
            animator.SetTrigger("EndCrouch");
            animator.SetBool("IsCrouching", false);
            crouchStarted = false;
            jumpKeyHoldTime = 0f;
        }

        //animator.SetBool("IsCrouching", crouchStarted);
        animator.SetBool("IsIdle", !hasMoveInput && !isJumpKeyPressed && isGrounded && !isJumping && !isMeteorDropping);
        animator.SetBool("IsRunning", isGrounded && hasMoveInput && !isJumpKeyPressed);
        //animator.SetBool("IsGrounded", isGrounded);

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
