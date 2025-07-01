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

        // 着地検出
        if (isGrounded && !wasGrounded)
        {
            Debug.Log("着地しました");
            isJumping = false;
        }
        wasGrounded = isGrounded;

        // 移動入力（走り判定）
        bool hasMoveInput = Mathf.Abs(playerMove.MoveInput) > 0f;

        // ジャンプキー押下判定
        bool isJumpKeyPressed = Input.GetKey(playerState.keyBind.playerJump);

        // しゃがみ開始トリガー（ジャンプキー押した瞬間）
        if (isJumpKeyPressed && !crouchStarted)
        {
            animator.SetTrigger("StartCrouch");
            crouchStarted = true;
        }
        // ジャンプキー離したらしゃがみ解除フラグOFF
        if (!isJumpKeyPressed)
        {
            crouchStarted = false;
        }
        animator.SetBool("IsCrouching", isJumpKeyPressed);

        // 走り状態（ジャンプキー押してない・かつ移動キー押している・かつ地面にいる）
        bool isRunning = playerState.isGrounded && hasMoveInput && !isJumpKeyPressed;
        animator.SetBool("IsRunning", isRunning);

        // 向き制御
        if (playerMove.MoveInput > 0f)
            transform.rotation = Quaternion.Euler(0, 90, 0);
        else if (playerMove.MoveInput < 0f)
            transform.rotation = Quaternion.Euler(0, -90, 0);

        // ハイジャンプ実行
        if (playerSpecial.playHighJumpAnim)
        {
            animator.SetTrigger("HighJump");
            playerSpecial.playHighJumpAnim = false;
            crouchStarted = false;
        }

        // メテオドロップ
        if (playerSpecial.meteorDrop)
        {
            animator.SetTrigger("MeteorDrop");
            isJumping = true;
            return;
        }

        // 通常ジャンプ判定
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            isJumping = true;

            int jumpType = 1; // デフォルト通常ジャンプ

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
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
