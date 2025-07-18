using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerState : MonoBehaviour
{
    public bool highJumpOn = false;      //ハイジャンプ可能か
    public bool quickJumpOn = false;     //クイックジャンプ可能か
    public bool meteorDropOn = false;    //メテオドロップ叶か

    public Rigidbody RigidBody;
    public KeyBind keyBind;
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
    public bool justLanded = false; // 今フレーム着地したか


    void Start()
    {
        keyBind = GameObject.Find("KeyManager").GetComponent<KeyBind>();
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
        if (!wasGrounded && isGrounded)
        {
            landing = true;

            //  着地ジャンプ回数を増やす（最大2まで）
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

        // 猶予時間カウント（オーバーしたらリセット）
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SearchItem"))
        {
            Destroy(other.gameObject);
        }
    }
}
