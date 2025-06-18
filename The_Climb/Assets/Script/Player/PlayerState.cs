using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerState : MonoBehaviour
{
    Rigidbody RigidBody;
    public KeyBind keyBind;
    PlayerMove move;
    PlayerJump jump;
    PlayerSpecialAction special;

    public bool highJumpOn = false;      //ハイジャンプ可能か
    public bool quickJumpOn = false;     //クイックジャンプ可能か
    public bool meteorDropOn = false;    //メテオドロップ叶か

    public bool playerDirectionRight = true;  //プレイヤーの見ている方向が右ならtrue、左ならfalse
    private bool wasGrounded = false;    //前フレームの地面状態
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

    void Start()
    {
        keyBind = GameObject.Find("KeyManager").GetComponent<KeyBind>();
        RigidBody = GetComponent<Rigidbody>();
        move = GetComponent<PlayerMove>();
        jump = GetComponent<PlayerJump>();
        special = GetComponent<PlayerSpecialAction>();

        groundLayer = GameLayer.ToMask(GameLayers.GROUND);

        // インスペクターまたはスクリプトで設定
        RigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Physics.gravity = new Vector3(0, -45.6F, 0); // Gを倍にする
    }

    private void FixedUpdate()
    {
        // 左壁判定（カプセル形）
        isLeftWall = Physics.CheckCapsule(leftWallCheck.position + Vector3.up * 0.68f, leftWallCheck.position + Vector3.down * 0.68f, 0.001f, groundLayer);
        // 右壁判定（カプセル形）
        isRightWall = Physics.CheckCapsule(rightWallCheck.position + Vector3.up * 0.68f, rightWallCheck.position + Vector3.down * 0.68f, 0.001f, groundLayer);

        if (!jump.jumpCoolActive)
        {
            // 地面判定（カプセル形）
            isGrounded = Physics.CheckCapsule(groundCheck.position + Vector3.left * 0.1f, groundCheck.position + Vector3.right * 0.1f, groundCheckRadius, groundLayer);
        }

        //空中時、isJumpOKを反応させない
        if (isAir)
        {
            isJumpMoveOK = false;
        }
        else
        {
            // ジャンプOK判定（カプセル形）
            isJumpMoveOK = Physics.CheckCapsule(jumpMoveOKCheck.position + Vector3.left * 0.1f, jumpMoveOKCheck.position + Vector3.right * 0.1f, 0.3f, groundLayer);
        }

        //着地チェック
        LandingChack();

        //空中判定
        if (!isGrounded && !isJumpMoveOK)
        {
            isAir = true;
        }
        else
        {
            isAir = false;
        }

        //前フレームの接地判定
        wasGrounded = isGrounded;
    }

    private void LandingChack()
    {
        //着地判定
        if (!wasGrounded && isGrounded)
        {
            landingJumpCounter = 0f;
            landingJumpOn = true;

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
            landingJumpCounter += Time.fixedDeltaTime;

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
}
