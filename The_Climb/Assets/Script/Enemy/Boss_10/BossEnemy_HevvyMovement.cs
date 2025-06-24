using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BossEnemy_HevvyMovement : MonoBehaviour
{
    public HevvyStats stats;
    private CharacterGroundChecker GroundChecker;
    private float LeftBoundary;/* = -15f; *///Bossが壁によりすぎないようにするための向き変更のライン（左）
    private float RightBoundary;/* = 15f; /*//*/Bossが壁によりすぎないようにするための向き変更のライン（右）*/
    private Rigidbody rb;
    private float Timer;
    private int JumpCount = 0;
    private bool IsCharging = false;
    private float ChargeTimer = 0f;
    [SerializeField] private int HorizontalDirection = 1;
    private bool IsGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GroundChecker = GetComponent<CharacterGroundChecker>();
        LeftBoundary = stats.LeftBoundary;
        RightBoundary = stats.RightBoundary;
    }

    private void Update()
    {
       
        {
            if (IsCharging)
                {
                Debug.Log("チャージジャンプのFlagがたちました");
                ChargeTimer += Time.deltaTime;

                if (ChargeTimer >= stats.JumpInterval && GroundChecker.CheckIsGround())

                {
                    Debug.Log("チャージジャンプが呼び出しされました");
                    ChargeJump();
                        IsCharging = false;
                        ChargeTimer = 0f;
                        JumpCount = 0;
                    }
                    

                    return;
                }

                Timer += Time.deltaTime;

            if (Timer >= stats.JumpInterval && GroundChecker.CheckIsGround())

            {
                Timer = 0f;
                    JumpCount++;

                    if (JumpCount >= stats.JumpsBeforeCharge)
                    {
                        BeginCharge();
                    }
                    else
                    {
                    
                        NormalJump();
                    }
                }
            // 一定のラインに到達したら向きを変える
            if (transform.position.x <= LeftBoundary)
            {
                Debug.Log(transform.position);
                HorizontalDirection = 1;
            }
            else if (transform.position.x >= RightBoundary)
            {
                Debug.Log(transform.position);
                HorizontalDirection = -1;
            }

        }
    }

    void NormalJump()
    {
        //if (GroundChecker.CheckIsGround())
        //{ 
            rb.linearVelocity = Vector3.zero;
            Vector3 jumpVector = new Vector3(stats.HorizontalJumpForce * HorizontalDirection, stats.JumpForce, 0f);
            rb.AddForce(jumpVector, ForceMode.Impulse);
        //}
    }

    void BeginCharge()
    {
        IsCharging = true;
        rb.linearVelocity = Vector3.zero;

        // たまに左右を切り替える（オプション）
        //HorizontalDirection *= Random.value > 0.5f ? -1 : 1;
    }

    void ChargeJump()
    {
        //if (GroundChecker.CheckIsGround())
        //{
            rb.linearVelocity = Vector3.zero;
            Vector3 jumpVector = new Vector3(0f, stats.ChargeJumpForce, 0f);
            rb.AddForce(jumpVector, ForceMode.Impulse);

            // 降下時に重力を弱くする（月面風）
            StartCoroutine(SlowFallCoroutine());
        //}
    }

    System.Collections.IEnumerator SlowFallCoroutine()
    {
        float originalDrag = rb.linearDamping;

        rb.useGravity = false;
        rb.linearDamping = 0f;

        // 上昇している間は待機
        while (rb.linearVelocity.y > 0f)
        {
            yield return null;
        }

        // 降下開始
        rb.useGravity = true;
        rb.linearDamping = originalDrag;

        rb.AddForce(Vector3.down * Physics.gravity.y * stats.SlowFallGravityScale, ForceMode.Acceleration);
    }
}