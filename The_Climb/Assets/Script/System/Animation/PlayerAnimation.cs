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

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private float jumpKeyHoldTime = 0f;
    private float crouchThreshold = 0.2f; // この時間以上でしゃがみ開始

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

        // 地面着地検出
        if (isGrounded && !wasGrounded)
        {
            Debug.Log("着地しました");
            isJumping = false;
        }
        wasGrounded = isGrounded;

        // 向き制御（チャージ中は向きを固定）
        bool isChargingHighJump = jumpKeyHoldTime > 0f && !isJumping && isGrounded;
        if (!isChargingHighJump)
        {
            if (playerMove.MoveInput > 0f)
                transform.rotation = Quaternion.Euler(0, 90, 0);
            else if (playerMove.MoveInput < 0f)
                transform.rotation = Quaternion.Euler(0, -90, 0);
        }

        // 走り状態
        bool isRunning = isGrounded && hasMoveInput && !isJumpKeyPressed;
        animator.SetBool("IsRunning", isRunning);

        // ジャンプキー長押し時間カウント（しゃがみ始め）
        if (isJumpKeyPressed && isGrounded && !isJumping)
        {
            jumpKeyHoldTime += Time.deltaTime;

            if (jumpKeyHoldTime >= crouchThreshold && !crouchStarted)
            {
                animator.SetTrigger("StartCrouch");
                crouchStarted = true;
            }
        }
        else
        {
            // 長押しが中断されたらカウントリセット
            jumpKeyHoldTime = 0f;
        }

        // しゃがみ状態のキャンセル判定
        bool shouldCancelCrouch = hasMoveInput || isJumping || (!isJumpKeyPressed && crouchStarted);
        if (crouchStarted && shouldCancelCrouch)
        {
            animator.SetTrigger("EndCrouch");
            animator.SetBool("IsCrouching", false);
            crouchStarted = false;
        }

        // アニメーターにしゃがみ状態を反映
        animator.SetBool("IsCrouching", crouchStarted);

        // アイドル状態
        bool isIdle = !hasMoveInput && !isJumpKeyPressed && isGrounded && !isJumping;
        animator.SetBool("IsIdle", isIdle);

        // ハイジャンプ演出（優先）
        if (playerSpecial.playHighJumpAnim)
        {
            animator.ResetTrigger("EndCrouch"); // 衝突防止
            animator.SetTrigger("HighJump");
            animator.SetBool("IsCrouching", false);
            crouchStarted = false;
            jumpKeyHoldTime = 0f;
            isJumping = true;
            playerSpecial.playHighJumpAnim = false;
        }

        // メテオドロップ演出
        if (playerSpecial.meteorDrop)
        {
            animator.SetTrigger("MeteorDrop");
            isJumping = true;
            return; // 他の処理はスキップ
        }

        // 通常ジャンプアニメ（ハイジャンプ演出中は無効）
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping && !playerSpecial.playHighJumpAnim)
        {
            isJumping = true;

            int jumpType = 1;
            if (playerJump != null)
            {
                if (playerJump.landingJumpNumber >= 2)
                    jumpType = 3;
                else if (playerJump.landingJumpNumber == 1)
                    jumpType = 2;
            }

            switch (jumpType)
            {
                case 1:
                    animator.SetTrigger("JumpAnimStep1");
                    break;
                case 2:
                    animator.SetTrigger("JumpAnimStep2");
                    break;
                case 3:
                    animator.SetTrigger("JumpAnimStep3");
                    break;
            }

            Debug.Log($"ジャンプアニメ Step {jumpType}");
        }

        // 地面判定のAnimator反映
        animator.SetBool("IsGrounded", isGrounded);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
