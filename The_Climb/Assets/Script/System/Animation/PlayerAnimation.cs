using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;

    private PlayerMove playerMove;
    private PlayerState playerState;
    private PlayerSpecialAction playerSpecial;
    private PlayerJump playerJump;  // ← 追加

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
        playerJump = GameObject.Find("PlayerModel").GetComponent<PlayerJump>(); // ← 追加

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

        // チャージ中は走りアニメを止めるように判定を修正
        bool isChargingHighJump = playerSpecial.highJumpChargeCounter > 0f;

        bool isRunning = playerState.isGrounded
                         && Mathf.Abs(playerMove.MoveInput) > 0f
                         && !isChargingHighJump;

        animator.SetBool("IsRunning", isRunning);

        // しゃがみアニメもこの後に設定
        animator.SetBool("IsChargingHighJump", isChargingHighJump);

        // 向き制御
        if (playerMove.MoveInput > 0f)
            transform.rotation = Quaternion.Euler(0, 90, 0);
        else if (playerMove.MoveInput < 0f)
            transform.rotation = Quaternion.Euler(0, -90, 0);

        // --- 各ジャンプアニメーション処理 ---

        // ハイジャンプチャージ中に一度だけしゃがみ開始アニメ
        if (playerSpecial.highJumpChargeCounter > 0.1f && !crouchStarted)
        {
            animator.SetTrigger("StartCrouch");
            crouchStarted = true;
        }

        // チャージが終わったら解除
        if (playerSpecial.highJumpChargeCounter == 0f)
        {
            crouchStarted = false;
        }

        // ハイジャンプ実行時
        if (playerSpecial.playHighJumpAnim)
        {
            animator.SetTrigger("HighJump");
            playerSpecial.playHighJumpAnim = false;
            crouchStarted = false;
        }

        // チャージキャンセル時も解除
        if (Input.GetKeyUp(playerState.keyBind.playerJump))
        {
            crouchStarted = false;
        }

        // メテオドロップ
        if (playerSpecial.meteorDrop)
        {
            animator.SetTrigger("MeteorDrop");
            isJumping = true;
            return;
        }

        // 通常ジャンプ：ジャンプ入力時に landingJumpNumber に応じてジャンプアニメ再生
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            int animStep = 1;

            if (playerJump.landingJumpNumber >= 2)
                animStep = 3;
            else if (playerJump.landingJumpNumber == 1)
                animStep = 2;

            isJumping = true;

            switch (animStep)
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

            Debug.Log($"ジャンプアニメ Step {animStep}");
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
