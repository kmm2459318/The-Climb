using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerState : MonoBehaviour
{
    public bool highJumpOn = false;      //ハイジャンプ可能か
    public bool quickJumpOn = false;     //クイックジャンプ可能か
    public bool meteorDropOn = false;    //メテオドロップ叶か

    [HideInInspector] public Rigidbody RigidBody;
    [HideInInspector] public InputManager inputManager;
    PlayerMove move;
    PlayerJump jump;
    PlayerSpecialAction special;
    [HideInInspector] public PlayerAnimation PlayerAnimation;

    public bool playerDirectionRight = true;  //プレイヤーの見ている方向が右ならtrue、左ならfalse
    private bool wasGrounded = false;    //前フレームの地面状態
    public bool landing = false;         //着地判定
    private float landingJumpTime = 0.1f;  //着地ジャンプの猶予タイム
    private float landingJumpCounter = 0f;  //着地ジャンプの猶予カウンター
    public bool landingJumpOn = false;   //着地ジャンプのカウントを始める用

    [HideInInspector] public Transform groundCheck;        //プレイヤー足元の地面判定用オブジェクト
    public bool isGrounded;              //地面判定
    [HideInInspector] public Transform jumpMoveOKCheck;    //プレイヤー足元のジャンプ判定用オブジェクト
    public bool isJumpMoveOK;            //ジャンプOK判定
    [HideInInspector] public Transform leftWallCheck;      //プレイヤー足元の左壁判定用オブジェクト
    public bool isLeftWall;              //左壁判定
    [HideInInspector] public Transform rightWallCheck;     //プレイヤー足元の右壁判定用オブジェクト
    public bool isRightWall;             //右壁判定
    [HideInInspector] public LayerMask groundLayer;        //地面レイヤー
    private float groundCheckRadius = 0.1f;  //地面判定の半径
    public bool isAir = false;           //空中判定

    private float playerFallSpeed = -19f;  //プレイヤーの落下速度

    public float erosionLevel = 0;       //プレイヤーの侵蝕度
    public int sanityLevel = 100;        //プレイヤーの正気度
    public bool carryingBuddy = true;    //Buddyをおんぶしてる状態か判定

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
    }

    private void Update()
    {
        // 左壁判定（カプセル形）
        isLeftWall = Physics.CheckCapsule(leftWallCheck.position + Vector3.up * 0.60f, leftWallCheck.position + Vector3.down * 0.60f, 0.001f, groundLayer);
        // 右壁判定（カプセル形）
        isRightWall = Physics.CheckCapsule(rightWallCheck.position + Vector3.up * 0.60f, rightWallCheck.position + Vector3.down * 0.60f, 0.001f, groundLayer);

        if (jump.jumpCoolActive || jump.jumping)
        {
            isGrounded = false;
        }
        else
        {
            // 地面判定（カプセル形）
            isGrounded = Physics.CheckCapsule(groundCheck.position + Vector3.up * 0.0f, groundCheck.position + Vector3.down * 0.1f, groundCheckRadius, groundLayer);
        }

        //空中時、isJumpOKを反応させない
        if (isAir)
        {
            isJumpMoveOK = false;
        }
        else
        {
            // ジャンプOK判定（カプセル形）
            isJumpMoveOK = Physics.CheckSphere(jumpMoveOKCheck.position, 0.19f, groundLayer);
        }

        //着地チェック
        if (!jump.jumpCoolActive)
        {
            LandingCheck();
        }

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
        if (RigidBody.linearVelocity.y < playerFallSpeed && !isGrounded && !jump.jumping && !landing)
        {
            RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, playerFallSpeed, 0);
        }

        //正気度０もしくは侵蝕度１００で死
        if (sanityLevel <= 0 || erosionLevel >= 100)
        {
            PlayerDead();
        }

        //壁に当たるのならば強制停止
        //    if ((isLeftWall && RigidBody.linearVelocity.x < 0) ||
        //(isRightWall && RigidBody.linearVelocity.x > 0))
        //    {
        //        RigidBody.linearVelocity = new Vector3(0, RigidBody.linearVelocity.y, 0);
        //    }

        //前フレームの接地判定
        wasGrounded = isGrounded;
    }

    private void LandingCheck()
    {
        landing = false;
        //着地判定
        if (!wasGrounded && isGrounded)
        {
            landing = true;

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

        //着地ジャンプ猶予カウント
        if (landingJumpOn)
        {
            landingJumpCounter += Time.deltaTime;

            if (landingJumpCounter > landingJumpTime)
            {
                jump.landingJumpNumber = 0;
                special.meteorHighJumpOK = false;
                LandingJumpReset();
            }
        }
    }

    public void LandingJumpReset()
    {
        landingJumpOn = false;
        special.quickJumpUsed = false;
        special.meteorDropUsed = false;
        special.meteorDropCounter = 0;
    }

    private void PlayerDead()
    {
        Debug.Log("栗松、帰国の準備をしろ。");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SearchItem"))
        {
            Destroy(other.gameObject);
        }
    }
}
