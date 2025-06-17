using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    private int spacePressCount = 0;
    private bool isJumping = false;
    PlayerMove PlayerMove;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    void Start()
    {
        animator = GetComponent<Animator>();
        PlayerMove = GameObject.Find("TentativePlayer").GetComponent<PlayerMove>();

        if (animator == null)
        {
            Debug.LogError("Animator が見つかりません。コンポーネントをアタッチしてください。");
        }
    }

    void Update()
    {
        // 着地チェック
        if (PlayerMove.landing)
        {
            isJumping = false;
            Debug.Log("着地しました。次のジャンプが可能になります。");
        }

        // スペース入力
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

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }


}
