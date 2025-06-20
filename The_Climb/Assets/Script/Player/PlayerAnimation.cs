using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    private int spacePressCount = 0;
    private bool isJumping = false;

    private PlayerMove PlayerMove;
    private PlayerState PlayerState;
    private PlayerSpecialAction PlayerSpecial;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private bool highJumpStarted = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        PlayerMove = GameObject.Find("TentativePlayer").GetComponent<PlayerMove>();
        PlayerState = GameObject.Find("TentativePlayer").GetComponent<PlayerState>();
        PlayerSpecial = GameObject.Find("TentativePlayer").GetComponent<PlayerSpecialAction>();

        if (animator == null)
        {
            Debug.LogError("Animator が見つかりません。");
        }
    }

    void Update()
    {
        // 地面チェック
        if (IsGrounded())
        {
            if (isJumping)
            {
                Debug.Log("着地しました。");
            }
            isJumping = false;
        }

        // ランアニメ
        bool isRunning = (PlayerMove.State.isGrounded || PlayerMove.slipping) && Mathf.Abs(PlayerMove.MoveInput) > 0f;
        animator.SetBool("IsRunning", isRunning);

        // 向き反転
        if (PlayerMove.MoveInput > 0f)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (PlayerMove.MoveInput < 0f)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }

        // ★チャージジャンプアニメ（ため）
        if (PlayerState.highJumpOn && PlayerState.isGrounded && Input.GetKeyDown(PlayerState.keyBind.playerJump))
        {
            animator.SetTrigger("HighJumpStart"); // しゃがむ開始（1回再生）
            highJumpStarted = true;
        }

        if (PlayerState.highJumpOn && PlayerState.isGrounded && Input.GetKey(PlayerState.keyBind.playerJump) && highJumpStarted)
        {
            animator.SetBool("isHighJumpCharging", true); // チャージ中（しゃがみ状態を維持）
        }
        else
        {
            animator.SetBool("isHighJumpCharging", false);
            highJumpStarted = false;
        }

        // ★チャージジャンプ発動時（ジャンプ離した or チャージ完了）
        if (PlayerSpecial.highJumpChargeCounter >= PlayerSpecial.highJumpChargeTime && Input.GetKeyUp(PlayerState.keyBind.playerJump))
        {
            animator.SetTrigger("HighJump");
        }

        // 通常ジャンプ：スペースキー入力で 3段階切替
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping && !PlayerState.highJumpOn)
        {
            spacePressCount++;
            int animStep = (spacePressCount - 1) % 3 + 1;
            Debug.Log("通常ジャンプ Step " + animStep);
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
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
