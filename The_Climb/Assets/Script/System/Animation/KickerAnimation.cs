using UnityEngine;

[RequireComponent(typeof(KickerMoveCommander))]
public class KickerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private KickerMoveCommander commander;
    private bool isJumping = false;

    void Awake()
    {
        commander = GetComponent<KickerMoveCommander>();
        commander.OnJumpTime += PlayJump;
    }

    void OnDestroy()
    {
        commander.OnJumpTime -= PlayJump;
    }

    private void PlayJump()
    {
        if (!isJumping && commander.IsGround())
        {
            animator.SetTrigger("Jump");
            isJumping = true;
        }
    }

    void Update()
    {
        // ジャンプアニメ終了後、地面に着いてたらIdleに戻す
        if (isJumping && commander.IsGround())
        {
            isJumping = false;
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Idle"); // Idle へ強制
        }
    }
}
