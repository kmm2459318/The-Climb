using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BossEnemy_HevvyMovement : MonoBehaviour
{
    public HevvyStats stats;
    [SerializeField] private CharacterGroundChecker groundChecker;
    [SerializeField] private float leftBoundary = -5f; //Bossが壁によりすぎないようにするための向き変更のライン（左）
    [SerializeField] private float rightBoundary = 5f; //Bossが壁によりすぎないようにするための向き変更のライン（右）
    private Rigidbody rb;
    private float timer;
    private int jumpCount = 0;
    private bool isCharging = false;
    private float chargeTimer = 0f;
    private int horizontalDirection = 1;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        {
                if (isCharging)
                {
                    chargeTimer += Time.deltaTime;

                    if (chargeTimer >= stats.chargeDuration)
                    {
                    
                        ChargeJump();
                        isCharging = false;
                        chargeTimer = 0f;
                        jumpCount = 0;
                    }
                    // たまに左右を切り替える（オプション）
                    horizontalDirection *= Random.value > 0.5f ? -1 : 1;

                    return;
                }

                timer += Time.deltaTime;

                if (timer >= stats.jumpInterval)
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
            if (transform.position.x <= leftBoundary)
            {
                horizontalDirection = 1;
            }
            else if (transform.position.x >= rightBoundary)
            {
                horizontalDirection = -1;
            }

        }
    }

    void NormalJump()
    {
        //CharacterGroundChecker characterGroundChecker = GetComponent<CharacterGroundChecker>();
        //if (characterGroundChecker.CheckIsGround())
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
    }

    void ChargeJump()
    {
        //CharacterGroundChecker characterGroundChecker = GetComponent<CharacterGroundChecker>();
        //if (characterGroundChecker.CheckIsGround())
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
        float originalGravity = rb.mass;
        rb.linearDamping = 0f;
        rb.useGravity = false;

        while (rb.linearVelocity.y > 0f)
        {
            yield return null;
        }

        rb.useGravity = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, 0f);
        rb.AddForce(Vector3.down * Physics.gravity.y * stats.slowFallGravityScale, ForceMode.Acceleration);
    }
}