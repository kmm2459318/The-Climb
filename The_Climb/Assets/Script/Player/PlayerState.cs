using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [Header("特殊アクションの可否")]
    public bool highJumpOn = false;
    public bool quickJumpOn = false;
    public bool meteorDropOn = false;

    [Header("参照")]
    public Rigidbody RigidBody;
    public KeyBind keyBind;
    public PlayerMove move;
    public PlayerJump jump;
    public PlayerSpecialAction special;
    public PlayerAnimation PlayerAnimation;

    [Header("方向・接地状態")]
    public bool playerDirectionRight = true;
    public bool isGrounded;
    public bool isJumpMoveOK;
    public bool isLeftWall;
    public bool isRightWall;
    public bool isAir;
    private bool wasGrounded = false;
    public bool landing = false;
    public bool landingJumpOn = false;

    [Header("判定位置")]
    public Transform groundCheck;
    public Transform jumpMoveOKCheck;
    public Transform leftWallCheck;
    public Transform rightWallCheck;

    [Header("レイヤー設定")]
    public LayerMask groundLayer;

    [Header("判定設定")]
    private float groundCheckRadius = 0.1f;
    private Collider[] _hitBuffer = new Collider[8];

    [Header("ジャンプ猶予関連")]
    private float landingJumpTime = 0.1f;
    private float landingJumpCounter = 0f;

    [Header("落下制御")]
    private float playerFallSpeed = -19f;

    void Start()
    {
        if (!RigidBody) RigidBody = GetComponent<Rigidbody>();
        if (!move) move = GetComponent<PlayerMove>();
        if (!jump) jump = GetComponent<PlayerJump>();
        if (!special) special = GetComponent<PlayerSpecialAction>();
        if (!PlayerAnimation) PlayerAnimation = GetComponent<PlayerAnimation>();
        if (!keyBind) keyBind = GameObject.Find("KeyManager").GetComponent<KeyBind>();

        groundLayer = GameLayer.ToMask(GameLayers.GROUND);

        RigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        RigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        Physics.gravity = new Vector3(0, -45f, 0);
    }

    void Update()
    {
        // 壁判定
        isLeftWall = CheckHitCapsule(leftWallCheck.position + Vector3.up * 0.6f, leftWallCheck.position + Vector3.down * 0.6f, 0.001f, groundLayer);
        isRightWall = CheckHitCapsule(rightWallCheck.position + Vector3.up * 0.6f, rightWallCheck.position + Vector3.down * 0.6f, 0.001f, groundLayer);

        // 地面判定
        if (jump.jumpCoolActive || jump.jumping)
            isGrounded = false;
        else
            isGrounded = CheckHitCapsule(groundCheck.position + Vector3.up * 0f, groundCheck.position + Vector3.down * 0.5f, groundCheckRadius, groundLayer);

        // ジャンプ可能判定
        if (isAir)
            isJumpMoveOK = false;
        else
            isJumpMoveOK = CheckHitSphere(jumpMoveOKCheck.position, 0.19f, groundLayer);

        // 着地チェック
        if (!jump.jumpCoolActive)
            LandingCheck();

        // 空中判定
        isAir = !isGrounded && !isJumpMoveOK;

        // 落下速度制御
        if (RigidBody.linearVelocity.y < playerFallSpeed && !isGrounded && !jump.jumping && !landing)
        {
            RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, playerFallSpeed, 0);
        }

        wasGrounded = isGrounded;
    }

    // 子オブジェクトも含めてレイヤー判定
    private bool CheckHitCapsule(Vector3 start, Vector3 end, float radius, LayerMask layer)
    {
        int count = Physics.OverlapCapsuleNonAlloc(start, end, radius, _hitBuffer);
        for (int i = 0; i < count; i++)
        {
            if (IsObjectOrChildOnLayer(_hitBuffer[i].gameObject, layer))
                return true;
        }
        return false;
    }

    private bool CheckHitSphere(Vector3 center, float radius, LayerMask layer)
    {
        int count = Physics.OverlapSphereNonAlloc(center, radius, _hitBuffer);
        for (int i = 0; i < count; i++)
        {
            if (IsObjectOrChildOnLayer(_hitBuffer[i].gameObject, layer))
                return true;
        }
        return false;
    }

    private bool IsObjectOrChildOnLayer(GameObject obj, LayerMask mask)
    {
        Transform t = obj.transform;
        while (t != null)
        {
            if (((1 << t.gameObject.layer) & mask) != 0)
                return true;
            t = t.parent;
        }
        return false;
    }

    private void LandingCheck()
    {
        landing = false;

        if (!wasGrounded && isGrounded)
        {
            landing = true;
            jump.landingJumpNumber = Mathf.Min(jump.landingJumpNumber + 1, 2);
            landingJumpCounter = 0f;
            landingJumpOn = true;
            isLeftWall = false;
            isRightWall = false;

            if (Mathf.Abs(RigidBody.linearVelocity.x) > move.groundMaxSpeed && !special.meteorDrop)
            {
                move.slipping = true;
                move.slipVelocity = RigidBody.linearVelocity;
            }
        }

        if (landingJumpOn)
        {
            landingJumpCounter += Time.deltaTime;
            if (landingJumpCounter > landingJumpTime)
            {
                jump.landingJumpNumber = 0;
                special.meteorHighJumpOK = false;
                landingJumpOn = false;
            }
        }
    }

    // ----------------------------------------------------------
    // PlayerJump から呼べるように追加
    // ----------------------------------------------------------
    public void LandingJumpReset()
    {
        landingJumpOn = false;
        special.quickJumpUsed = false;
        special.meteorDropUsed = false;
        special.meteorDropCounter = 0f;
    }
}
