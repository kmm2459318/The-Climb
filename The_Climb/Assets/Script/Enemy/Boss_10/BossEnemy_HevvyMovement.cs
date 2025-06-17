using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BossEnemy_HevvyMovement : MonoBehaviour
{
    public HevvyStats stats;
    private CharacterGroundChecker groundChecker;
    private float LeftBoundary;/* = -15f; *///Bossが壁によりすぎないようにするための向き変更のライン（左）
    private float RightBoundary;/* = 15f; /*//*/Bossが壁によりすぎないようにするための向き変更のライン（右）*/
    private Rigidbody rb;
    private float timer;
    private int jumpCount = 0;
    private bool isCharging = false;
    private float chargeTimer = 0f;
    [SerializeField] private int horizontalDirection = 1;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        groundChecker = GetComponent<CharacterGroundChecker>();
        LeftBoundary = stats.LeftBoundary;
        RightBoundary = stats.RightBoundary;
    }

    private void Update()
    {
       
        {
            if (isCharging)
                {
                Debug.Log("チャージジャンプのFlagがたちました");
                chargeTimer += Time.deltaTime;

                if (chargeTimer >= stats.jumpInterval && groundChecker.CheckIsGround())

                {
                    Debug.Log("チャージジャンプが呼び出しされました");
                    ChargeJump();
                        isCharging = false;
                        chargeTimer = 0f;
                        jumpCount = 0;
                    }
                    

                    return;
                }

                timer += Time.deltaTime;

            if (timer >= stats.jumpInterval && groundChecker.CheckIsGround())

            {
                timer = 0f;
                    jumpCount++;

                    if (jumpCount >= stats.jumpsBeforeCharge)
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
                horizontalDirection = 1;
            }
            else if (transform.position.x >= RightBoundary)
            {
                Debug.Log(transform.position);
                horizontalDirection = -1;
            }

        }
    }

    void NormalJump()
    {
        //if (groundChecker.CheckIsGround())
        //{ 
            rb.linearVelocity = Vector3.zero;
            Vector3 jumpVector = new Vector3(stats.horizontalJumpForce * horizontalDirection, stats.jumpForce, 0f);
            rb.AddForce(jumpVector, ForceMode.Impulse);
        //}
    }

    void BeginCharge()
    {
        isCharging = true;
        rb.linearVelocity = Vector3.zero;

        // たまに左右を切り替える（オプション）
        //horizontalDirection *= Random.value > 0.5f ? -1 : 1;
    }

    void ChargeJump()
    {
        //if (groundChecker.CheckIsGround())
        //{
            rb.linearVelocity = Vector3.zero;
            Vector3 jumpVector = new Vector3(0f, stats.chargeJumpForce, 0f);
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

        rb.AddForce(Vector3.down * Physics.gravity.y * stats.slowFallGravityScale, ForceMode.Acceleration);
    }
}