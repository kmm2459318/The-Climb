using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;

    private PlayerMove playerMove;
    private PlayerState playerState;
    private PlayerSpecialAction playerSpecial;

    private bool wasGrounded = true;
    private bool isJumping = false;
    private bool crouchStarted = false;
    private int spacePressCount = 0;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMove = GameObject.Find("PlayerModel").GetComponent<PlayerMove>();
        playerState = GameObject.Find("PlayerModel").GetComponent<PlayerState>();
        playerSpecial = GameObject.Find("PlayerModel").GetComponent<PlayerSpecialAction>();

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
            crouchStarted = true; // 一度だけtrueにする
        }

        // チャージが終わったら解除
        if (playerSpecial.highJumpChargeCounter == 0f)
        {
            crouchStarted = false; // ←ここでだけ false に戻す
        }



        // ハイジャンプ実行：ジャンプアニメ再生
        if (playerSpecial.playHighJumpAnim)
        {
            animator.SetTrigger("HighJump");
            playerSpecial.playHighJumpAnim = false;
            crouchStarted = false;
        }

        // チャージ失敗やキャンセル時、再チャージ可能にする
        if (Input.GetKeyUp(playerState.keyBind.playerJump))
        {
            crouchStarted = false;
        }


        // メテオドロップ検知
        if (playerSpecial.meteorDrop)
        {
            animator.SetTrigger("MeteorDrop");
            isJumping = true;
            return;
        }

        // 通常ジャンプ：地上から Space で3段階ループ
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            spacePressCount++;
            int animStep = (spacePressCount - 1) % 3 + 1;
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

            Debug.Log($"通常ジャンプ Step {animStep}");
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
