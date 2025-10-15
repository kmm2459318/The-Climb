using UnityEngine;
<<<<<<< HEAD

public class PlayerState : MonoBehaviour
{
    [Header("特殊アクションの可否")]
    public bool highJumpOn = false;
    public bool quickJumpOn = false;
    public bool meteorDropOn = false;
=======
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerState : MonoBehaviour
{
    public bool highJumpOn = false;      //ハイジャンプ可能か
    public bool quickJumpOn = false;     //クイックジャンプ可能か
    public bool meteorDropOn = false;    //メテオドロップ叶か
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995

    [Header("参照")]
    public Rigidbody RigidBody;
<<<<<<< HEAD
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
=======
    public InputManager inputManager;
    PlayerMove move;
    PlayerJump jump;
    PlayerSpecialAction special; 
    public PlayerAnimation PlayerAnimation;

    public bool playerDirectionRight = true;  //プレイヤーの見ている方向が右ならtrue、左ならfalse
    private bool wasGrounded = false;    //前フレームの地面状態
    public bool landing = false;         //着地判定
    private float landingJumpTime = 0.1f;  //着地ジャンプの猶予タイム
    private float landingJumpCounter = 0f;  //着地ジャンプの猶予カウンター
    public bool landingJumpOn = false;  //着地ジャンプのカウントを始める用

    public Transform groundCheck;        //プレイヤー足元の地面判定用オブジェクト
    public bool isGrounded;              //地面判定
    public Transform jumpMoveOKCheck;    //プレイヤー足元のジャンプ判定用オブジェクト
    public bool isJumpMoveOK;            //ジャンプOK判定
    public Transform leftWallCheck;      //プレイヤー足元の左壁判定用オブジェクト
    public bool isLeftWall;              //左壁判定
    public Transform rightWallCheck;     //プレイヤー足元の右壁判定用オブジェクト
    public bool isRightWall;             //右壁判定
    public LayerMask groundLayer;  //地面レイヤー
    private float groundCheckRadius = 0.1f;  //地面判定の半径
    public bool isAir = false;          //空中判定

    private float playerFallSpeed = -19f;  //プレイヤーの落下速度

    void Start()
    {
        inputManager = GameObject.Find("KeyManager").GetComponent<InputManager>();
        RigidBody = GetComponent<Rigidbody>();
        move = GetComponent<PlayerMove>();
        jump = GetComponent<PlayerJump>();
        special = GetComponent<PlayerSpecialAction>();

        PlayerAnimation = transform.Find("pico_chan_chr_pico_00").GetComponent<PlayerAnimation>();

        groundLayer = GameLayer.ToMask(GameLayers.GROUND);

        // インスペクターまたはスクリプトで設定
        //RigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        RigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        RigidBody.interpolation = RigidbodyInterpolation.Interpolate;

        Physics.gravity = new Vector3(0, -45F, 0); // Gを倍にする
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
    }

    void Update()
    {
<<<<<<< HEAD
        // 壁判定
        isLeftWall = CheckHitCapsule(leftWallCheck.position + Vector3.up * 0.6f, leftWallCheck.position + Vector3.down * 0.6f, 0.001f, groundLayer);
        isRightWall = CheckHitCapsule(rightWallCheck.position + Vector3.up * 0.6f, rightWallCheck.position + Vector3.down * 0.6f, 0.001f, groundLayer);
=======
        // 左壁判定（カプセル形）
        isLeftWall = Physics.CheckCapsule(leftWallCheck.position + Vector3.up * 0.60f, leftWallCheck.position + Vector3.down * 0.60f, 0.001f, groundLayer);
        // 右壁判定（カプセル形）
        isRightWall = Physics.CheckCapsule(rightWallCheck.position + Vector3.up * 0.60f, rightWallCheck.position + Vector3.down * 0.60f, 0.001f, groundLayer);
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995

        // 地面判定
        if (jump.jumpCoolActive || jump.jumping)
            isGrounded = false;
        else
<<<<<<< HEAD
            isGrounded = CheckHitCapsule(groundCheck.position + Vector3.up * 0f, groundCheck.position + Vector3.down * 0.5f, groundCheckRadius, groundLayer);

        // ジャンプ可能判定
=======
        {
            // 地面判定（カプセル形）
            isGrounded = Physics.CheckCapsule(groundCheck.position + Vector3.up * 0.0f, groundCheck.position + Vector3.down * 0.1f, groundCheckRadius, groundLayer);
        }

        //空中時、isJumpOKを反応させない
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
        if (isAir)
            isJumpMoveOK = false;
        else
<<<<<<< HEAD
            isJumpMoveOK = CheckHitSphere(jumpMoveOKCheck.position, 0.19f, groundLayer);

        // 着地チェック
=======
        {
            // ジャンプOK判定（カプセル形）
            isJumpMoveOK = Physics.CheckSphere(jumpMoveOKCheck.position, 0.19f, groundLayer);
        }

        //着地チェック
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
        if (!jump.jumpCoolActive)
            LandingCheck();

<<<<<<< HEAD
        // 空中判定
        isAir = !isGrounded && !isJumpMoveOK;

        // 落下速度制御
=======
        //空中判定
        if (!isGrounded && !isJumpMoveOK)
        {
            isAir = true;
        }
        else
        {
            isAir = false;
        }
        
        //落下速度調整
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
        if (RigidBody.linearVelocity.y < playerFallSpeed && !isGrounded && !jump.jumping && !landing)
        {
            RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, playerFallSpeed, 0);
        }

<<<<<<< HEAD
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
=======
        //壁に当たるのならば強制停止
    //    if ((isLeftWall && RigidBody.linearVelocity.x < 0) ||
    //(isRightWall && RigidBody.linearVelocity.x > 0))
    //    {
    //        RigidBody.linearVelocity = new Vector3(0, RigidBody.linearVelocity.y, 0);
    //    }

        //前フレームの接地判定
        wasGrounded = isGrounded;
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
    }

    private void LandingCheck()
    {
        landing = false;
<<<<<<< HEAD

        if (!wasGrounded && isGrounded)
        {
            landing = true;
            jump.landingJumpNumber = Mathf.Min(jump.landingJumpNumber + 1, 2);
=======
        //着地判定
        if (!wasGrounded && isGrounded)
        {
            landing = true;

>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
            landingJumpCounter = 0f;
            landingJumpOn = true;
            isLeftWall = false;
            isRightWall = false;
            //RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0, 0);

            // 横方向の速度が一定以上ならスリップ開始
            if (Mathf.Abs(RigidBody.linearVelocity.x) > move.groundMaxSpeed && !special.meteorDrop)
            {
                move.slipping = true;
                move.slipVelocity = RigidBody.linearVelocity;
            }
        }

<<<<<<< HEAD
=======
        //着地ジャンプ猶予カウント
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
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

<<<<<<< HEAD
    // ----------------------------------------------------------
    // PlayerJump から呼べるように追加
    // ----------------------------------------------------------
=======
>>>>>>> 54d0e82e499ed78363d7d179fe2ab1d876978995
    public void LandingJumpReset()
    {
        landingJumpOn = false;
        special.quickJumpUsed = false;
        special.meteorDropUsed = false;
        special.meteorDropCounter = 0f;
    }
}
