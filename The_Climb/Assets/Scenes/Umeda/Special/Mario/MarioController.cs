using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MarioController : MonoBehaviour
{
    public static class MarioInput
    {
        public static bool Left;
        public static bool Right;
        public static bool Jump;
        public static bool Dash;
    }

    [Header("移動")]
    public float moveSpeed = 5f;
    public float dashMultiplier = 1.5f;
    public float jumpForce = 7f;

    [Header("空中操作")]
    [Range(0f, 1f)]
    public float airTurnRate = 0.15f;

    [Header("接地判定")]
    public Transform groundCheck;
    public float groundRadius = 0.25f;
    public LayerMask groundLayer;

    [Header("重力調整")]
    public float fallGravityMultiplier = 2.5f;
    public float lowJumpMultiplier = 2.0f;

    [Header("ダッシュターン")]
    public float turnDuration = 0.05f;
    public float turnSlideForce = 1.2f;
    public float dashTurnMultiplier = 1.5f;

    [Header("モデル")]
    public GameObject idleModel;
    public GameObject walkModelA;
    public GameObject walkModelB;
    public GameObject jumpModel;
    public GameObject turnModel;
    public GameObject gameOverModel;

    [Header("歩行アニメ")]
    public float walkInterval = 0.15f;
    public float dashWalkSpeedMultiplier = 0.6f;

    [Header("死亡演出")]
    public float deathJumpForce = 8f;
    public float deathGravityMultiplier = 1.5f;

    [Header("死亡後処理")]
    public float deathWaitTime = 2.5f;   // 落下演出時間
    public string ignoreGroundLayerName = "IgnoreGround";
    public string normalLayerName = "Player";

    Rigidbody rb;
    Vector3 baseScale;

    float moveInput;
    int lastDirection = 1;
    bool isGrounded;
    bool isFacingRight = true;
    bool isTurning;
    bool isDead;
    Collider[] allColliders;

    float walkTimer;
    int walkStep;

    PlayerHealth health;

    void Awake()
    {
        allColliders = GetComponentsInChildren<Collider>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        baseScale = transform.localScale;
        ShowIdle();

        health = GetComponent<PlayerHealth>();
        if (health != null)
            health.OnDead += GameOver;
    }

    void Update()
    {
        if (isDead) return;

        HandleInput();
        CheckGround();
        HandleJump();
        HandleFacingAndTurn();
        UpdateModel();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        HandleMovement();
        ApplyBetterJump();
    }

    // ===== 入力 =====
    void HandleInput()
    {
        bool left = Input.GetKey(KeyCode.A) || MarioInput.Left;
        bool right = Input.GetKey(KeyCode.D) || MarioInput.Right;

        if (left && !right) lastDirection = -1;
        else if (right && !left) lastDirection = 1;

        moveInput = (left || right) ? lastDirection : 0;
    }

    // ===== 向き & ターン =====
    void HandleFacingAndTurn()
    {
        if (moveInput == 0 || isTurning) return;

        bool wantRight = moveInput > 0;
        if (wantRight == isFacingRight) return;

        bool isDashing =
            Input.GetKey(KeyCode.LeftShift) || MarioInput.Dash;

        bool canTurn =
            isDashing &&
            isGrounded &&
            Mathf.Abs(rb.linearVelocity.x) > moveSpeed * 0.8f;

        if (canTurn)
        {
            StartCoroutine(TurnCoroutine(wantRight));
        }
        else
        {
            isFacingRight = wantRight;
            ApplyFacing();
        }
    }

    IEnumerator TurnCoroutine(bool faceRight)
    {
        isTurning = true;

        DisableAllModels();
        turnModel.SetActive(true);

        float force = turnSlideForce * dashTurnMultiplier;
        rb.AddForce(
            new Vector3(isFacingRight ? force : -force, 0f, 0f),
            ForceMode.Impulse
        );

        yield return new WaitForSeconds(turnDuration);

        isFacingRight = faceRight;
        ApplyFacing();

        isTurning = false;
    }

    // ===== 移動 =====
    void HandleMovement()
    {
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || MarioInput.Dash)
            speed *= dashMultiplier;

        float targetX = moveInput * speed;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(targetX, rb.linearVelocity.y, 0f);
        }
        else
        {
            float newX = Mathf.Lerp(rb.linearVelocity.x, targetX, airTurnRate);
            rb.linearVelocity = new Vector3(newX, rb.linearVelocity.y, 0f);
        }
    }

    // ===== ジャンプ =====
    void HandleJump()
    {
        if (
            (Input.GetKeyDown(KeyCode.W) ||
             Input.GetKeyDown(KeyCode.Space) ||
             MarioInput.Jump)
            && isGrounded
        )
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            ShowJump();
        }
    }

    // ===== 接地 =====
    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundLayer
        );
    }

    // ===== モデル =====
    void UpdateModel()
    {
        if (isTurning) return;

        if (!isGrounded)
        {
            ShowJump();
            return;
        }

        if (Mathf.Abs(moveInput) < 0.1f)
        {
            walkTimer = 0;
            walkStep = 0;
            ShowIdle();
            return;
        }

        float interval = walkInterval;
        if (Input.GetKey(KeyCode.LeftShift) || MarioInput.Dash)
            interval *= dashWalkSpeedMultiplier;

        walkTimer += Time.deltaTime;
        if (walkTimer >= interval)
        {
            walkTimer = 0;
            walkStep = (walkStep + 1) % 4;
        }

        DisableAllModels();
        if (walkStep == 0 || walkStep == 2) idleModel.SetActive(true);
        else if (walkStep == 1) walkModelA.SetActive(true);
        else walkModelB.SetActive(true);
    }

    // ===== ジャンプ補正 =====
    void ApplyBetterJump()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y
                * (fallGravityMultiplier - 1f)
                * Time.fixedDeltaTime;
        }
        else if (
            rb.linearVelocity.y > 0 &&
            !Input.GetKey(KeyCode.Space) &&
            !Input.GetKey(KeyCode.W) &&
            !MarioInput.Jump
        )
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y
                * (lowJumpMultiplier - 1f)
                * Time.fixedDeltaTime;
        }
    }

    // ===== 向き適用 =====
    void ApplyFacing()
    {
        transform.localScale = new Vector3(
            isFacingRight ? Mathf.Abs(baseScale.x) : -Mathf.Abs(baseScale.x),
            baseScale.y,
            baseScale.z
        );
    }

    // ===== 表示 =====
    void DisableAllModels()
    {
        idleModel.SetActive(false);
        walkModelA.SetActive(false);
        walkModelB.SetActive(false);
        jumpModel.SetActive(false);
        turnModel.SetActive(false);
        gameOverModel.SetActive(false);
    }

    void ShowIdle()
    {
        DisableAllModels();
        idleModel.SetActive(true);
    }

    void ShowJump()
    {
        DisableAllModels();
        jumpModel.SetActive(true);
    }

    public float respawnDelay = 2f;  // Inspectorで調整可能

    public bool IsDead()
    {
        return isDead;
    }

    public void GameOver()
    {
        if (isDead) return;
        isDead = true;

        health.isInvincible = true;

        foreach (var col in allColliders)
            col.enabled = false;

        DisableAllModels();
        gameOverModel.SetActive(true);

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = true;
        rb.isKinematic = false;

        rb.AddForce(Vector3.up * deathJumpForce, ForceMode.Impulse);

        StartCoroutine(DeathRoutine());
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        PlayerRespawnUmeda respawn = GetComponent<PlayerRespawnUmeda>();
        if (respawn != null)
            respawn.Respawn();
    }

    IEnumerator DeathRoutine()
    {
        float timer = 0f;

        while (timer < deathWaitTime)
        {
            timer += Time.deltaTime;

            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y
                    * (deathGravityMultiplier - 1f)
                    * Time.deltaTime;
            }

            yield return null;
        }

        PlayerRespawnUmeda respawn = GetComponent<PlayerRespawnUmeda>();
        if (respawn != null)
            respawn.Respawn();
    }

    void OnDestroy()
    {
        if (health != null)
            health.OnDead -= GameOver;
    }

    public void OnRespawnComplete()
    {
        isDead = false;

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = true;
        rb.isKinematic = false;

        DisableAllModels();
        ShowIdle();
    }

    public void OnRespawn()
    {
        foreach (var col in allColliders)
            col.enabled = true;

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = true;
        rb.isKinematic = false;

        DisableAllModels();
        ShowIdle();

        isDead = false;

        if (health != null)
            health.isInvincible = false;
    }
}
