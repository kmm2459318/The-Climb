using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    private int spacePressCount = 0;
    private bool isJumping = false;
    private Rigidbody rb;
    PlayerMove PlayerMove;
    PlayerState PlayerState;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    void Start()
    {
        animator = GetComponent<Animator>();
        PlayerMove = GameObject.Find("TentativePlayer").GetComponent<PlayerMove>();
        PlayerState = GameObject.Find("TentativePlayer").GetComponent<PlayerState>();

        if (animator == null)
            Debug.LogError("Animator が見つかりません。コンポーネントをアタッチしてください。");
    }

    void Update()
    {
        // ★ 地面チェック
        if (IsGrounded())
        {
            if (isJumping)
            {
                Debug.Log("着地しました。次のジャンプが可能になります。");
            }
            isJumping = false;
        }

        // ★ ランアニメ（地上かつ移動中なら再生）
        bool isRunning = PlayerMove.State.isGrounded && Mathf.Abs(PlayerMove.MoveInput) > 0f;
        animator.SetBool("IsRunning", isRunning);


        
        // 体の向き反転処理
        if (PlayerMove.MoveInput > 0f)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0); // 右向き
        }
        else if (PlayerMove.MoveInput < 0f)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0); // 左向き
        }

        // ★ スペース入力（ジャンプアニメ：3パターン切替）
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            spacePressCount++;
            int animStep = (spacePressCount - 1) % 3 + 1;
            Debug.Log("ジャンプ Step " + animStep);
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

    // ★ 3D用の地面判定
    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
