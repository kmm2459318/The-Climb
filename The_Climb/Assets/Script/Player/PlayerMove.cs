using System.Linq;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody RigidBody;
    KeyBind keyBind;

    public bool highJumpOn = false;      //ハイジャンプ可能か
    public bool quickJumpOn = false;     //クイックジャンプ可能か
    public bool meteorDropOn = false;    //メテオドロップ叶か

    private float groundMoveForce = 0.25f;     //プレイヤーの地上移動速度
    private float moveInput = 0f;        //プレイヤーの移動方向
    private float airMoveForce = 90f;    //空中での移動速度
    private float maxAirSpeed = 10f;     //空中での速度制限
    private bool jumping = false;        //ジャンプ入力中判定
    private float coyoteTime = 0.05f;    //コヨーテタイム
    private float coyoteCounter = 0f;    //コヨーテタイムカウント
    private float jumpCoolTime = 0.06f;  //ジャンプのクールタイム
    private float jumpCoolCounter = 0f;  //ジャンのクールタイムカウント
    private bool jumpCoolTiming = false;  //ジャンクールタイムを始める用判定
    private float jumpTime;              //ジャンプ入力時間
    private float jumpTimeMax = 0.1f;    //最大ジャンプ入力時間
    private float groundJumpPower = 10f;  //ジャンプでプレイヤーにかかる上方向の力
    private float maxJumpSpeed = 12f;   //空中での速度制限
    [SerializeField] AnimationCurve jumpCurve = new();  //ジャンプ時の速度カーブ

    private bool wasGrounded = false;    //前フレームの地面状態
    public bool landing = false;         //着地したか判定
    public bool slipping = false;        //着地後勢い止めず滑ってる判定
    private float slippingTime = 0.05f;     //スリップ方向切り替え用
    private float slippingCounter = 0f;  //スリップ方向切り替えようタイム
    Vector3 slipVelocity;                //滑り時のVelocity

    public Transform groundCheck;        //プレイヤー足元の地面判定用オブジェクト
    public bool isGrounded;              //地面判定
    public Transform jumpMoveOKCheck;    //プレイヤー足元のジャンプ判定用オブジェクト
    public bool isJumpMoveOK;            //ジャンプOK判定
    public Transform leftWallCheck;      //プレイヤー足元の左壁判定用オブジェクト
    public bool isLeftWall;              //左壁判定
    public Transform rightWallCheck;     //プレイヤー足元の右壁判定用オブジェクト
    public bool isRightWall;             //右壁判定
    public LayerMask groundLayer;        //地面レイヤー
    private float groundCheckRadius = 0.1f;  //地面判定の半径
    private bool isAir = false;          //空中判定

    private float landingJumpTime = 0.1f;  //着地ジャンプの猶予タイム
    private float landingJumpCounter = 0f;  //着地ジャンプの猶予カウンター
    private bool landingJumpOn = false;  //着地ジャンプのカウントを始める用
    private int landingJumpNumber = 0;   //着地ジャンプの連続回数
    private float landingLowJumpPower = 12;  //着地ジャンプのパワー
    private float landingHighJumpPower = 15f;  //着地ジャンプのパワー

    private float highJumpChargeTime = 0.8f;  //ハイジャンプのチャージ時間
    private float highJumpChargeCounter = 0f;  //ハイジャンプのチャージカウンター
    private bool highJump = false;       //ハイジャンプする判定
    private float highJumpPower = 25f;   //ハイジャンプのパワー

    private bool quickJump = false;      //クイックジャンプする判定
    public bool quickJumpUsed = false;  //クイックジャンプを使用したか判定

    void Start()
    {
        keyBind = GameObject.Find("GameManager").GetComponent<KeyBind>();
        RigidBody = GetComponent<Rigidbody>();

        // インスペクターまたはスクリプトで設定
        RigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Physics.gravity = new Vector3(0, -45.6F, 0); // Gを倍にする
    }

    private void Update()
    {
        //移動
        if (Input.GetKey(keyBind.playerMoveLeft) && Input.GetKey(keyBind.playerMoveRight) ||
            !Input.GetKey(keyBind.playerMoveLeft) && !Input.GetKey(keyBind.playerMoveRight))  //止まる
        {
            moveInput = 0f;
        }
        else if (Input.GetKey(keyBind.playerMoveLeft) && !isLeftWall)  //左移動
        {
            moveInput = -1f;
        }
        else if (Input.GetKey(keyBind.playerMoveRight) && !isRightWall)  //右移動
        {
            moveInput = 1f;
        }
        else 
        {
            moveInput = 0f;
        }

        //ジャンプ
        if ((coyoteCounter <= coyoteTime || isJumpMoveOK) && !jumpCoolTiming)
        {
            if (Input.GetKeyDown(keyBind.playerJump))
            {
                jumping = true;
                jumpCoolTiming = true;

                //着地ジャンプ
                if (landingJumpOn)
                {
                    landingJumpNumber++;
                    landingJumpOn = false;
                }
            }
        }

        if (jumping)
        {
            if (Input.GetKeyUp(keyBind.playerJump) || jumpTime >= jumpTimeMax)
            {
                jumping = false;
                jumpTime = 0;
            }
            else if (Input.GetKey(keyBind.playerJump))
            {
                jumpTime += Time.deltaTime;
            }
        }

        //チャージジャンプのチャージ
        if (highJumpOn)
        {
            if (jumpCoolTiming || isAir)
            {
                highJumpChargeCounter = 0f;
            }
            else if (Input.GetKey(keyBind.highJump) && isGrounded)
            {
                highJumpChargeCounter += Time.deltaTime;
            }
            else if (Input.GetKeyUp(keyBind.highJump))
            {
                if (highJumpChargeCounter >= highJumpChargeTime)
                {
                    jumpCoolTiming = true;
                    highJump = true;
                }
            }
        }

        //ジャンプのクールタイム
        if (jumpCoolTiming)
        {
            jumpCoolCounter += Time.deltaTime;
            isGrounded = false;

            if (jumpCoolCounter > jumpCoolTime)
            {
                jumpCoolTiming = false;
            }
        }

        //クイックジャンプ
        if (quickJumpOn)
        {
            if (isAir && Input.GetKeyDown(keyBind.playerJump) && !quickJumpUsed)
            {
                quickJump = true;
                quickJumpUsed = true;
            }
        }
    }

    void FixedUpdate()
    {
        // 左壁判定（カプセル形）
        isLeftWall = Physics.CheckCapsule(leftWallCheck.position + Vector3.up * 0.49f, leftWallCheck.position + Vector3.down * 0.49f, 0.001f, groundLayer);
        // 右壁判定（カプセル形）
        isRightWall = Physics.CheckCapsule(rightWallCheck.position + Vector3.up * 0.49f, rightWallCheck.position + Vector3.down * 0.49f, 0.001f, groundLayer);

        if (!jumpCoolTiming)
        {
            // 地面判定（カプセル形）
            isGrounded = Physics.CheckCapsule(groundCheck.position + Vector3.left * 0.3f, groundCheck.position + Vector3.right * 0.3f, groundCheckRadius, groundLayer);
        }

        //空中時、isJumpOKを反応させない
        if (isAir)
        {
            isJumpMoveOK = false;
        }
        else
        {
            // ジャンプOK判定（カプセル形）
            isJumpMoveOK = Physics.CheckCapsule(jumpMoveOKCheck.position + Vector3.left * 0.2f, jumpMoveOKCheck.position + Vector3.left * 0.2f, 0.3f, groundLayer);
        }

        //着地判定
        landing = false;
        if (!wasGrounded && isGrounded)
        {
            landing = true;

            landingJumpCounter = 0f;
            landingJumpOn = true;
            quickJumpUsed = false;

            // 横方向の速度が一定以上ならスリップ開始
            if (Mathf.Abs(RigidBody.linearVelocity.x) > 3.910599f)
            {
                slipping = true;
                slipVelocity = RigidBody.linearVelocity;
            }
        }

        //着地ジャンプ猶予カウント
        if (landingJumpOn)
        {
            landingJumpCounter += Time.fixedDeltaTime;

            if (landingJumpCounter > landingJumpTime)
            {
                landingJumpOn = false;
                landingJumpNumber = 0;
            }
        }

        //移動
        if (isGrounded || (!isGrounded && isJumpMoveOK && !isLeftWall && !isRightWall))
        {
            isAir = false;
            coyoteCounter = 0f;

            //プレイヤー地上の移動
            GroundPlayerMove();
        }
        else
        {
            isAir = true;
            coyoteCounter += Time.fixedDeltaTime;

            //プレイヤー空中の移動
            AirPlayerMove();
        }

        //ジャンプ
        if (jumping)
        {
            if (landingJumpNumber >= 2)
            {
                Jump(landingHighJumpPower);
            }
            else if (landingJumpNumber == 1)
            {
                Jump(landingLowJumpPower);
            }
            else
            {
                Jump(groundJumpPower);
            }
        }

        if (highJump)
        {
            HighJump();
        }

        if (quickJump)
        {
            Jump(landingHighJumpPower);

            quickJump = false;
        }

        //前フレームの接地判定
        wasGrounded = isGrounded;
    }

    private void GroundPlayerMove()
    {
        if (slipping)
        {
            //減少時間
            float slipFriction = 11f;

            if (moveInput == 1f)
            {
                //横速度だけ徐々に減衰させRU
                slipVelocity.x = Mathf.MoveTowards(Mathf.Abs(slipVelocity.x), 0f, slipFriction * Time.fixedDeltaTime);
                slippingCounter = 0;
            }
            else if(moveInput == -1f)
            {
                //横速度だけ徐々に減衰させRU
                slipVelocity.x = Mathf.MoveTowards(Mathf.Abs(slipVelocity.x) * -1.0f, 0f, slipFriction * Time.fixedDeltaTime);
                slippingCounter = 0;
            }
            else if (slippingCounter > slippingTime)
            {
                slipping = false;
            }
            else if (moveInput == 0f)
            {
                slipVelocity.x = Mathf.MoveTowards(slipVelocity.x, 0f, slipFriction * Time.fixedDeltaTime);
                slippingCounter += Time.fixedDeltaTime;
            }

            RigidBody.linearVelocity = new Vector3(slipVelocity.x, 0, 0);

            //一定以下になったらスリップ終了（普通の地上移動に戻す）
            if (Mathf.Abs(slipVelocity.x) <= 3.910599f)
            {
                slipping = false;
            }
            return; //通常の地上移動処理はスキップ
        }

        // 地上：慣性なし、即応する左右移動
        Vector3 force = new Vector3(moveInput, 0f, 0f) * groundMoveForce;
        RigidBody.AddForce(force);
        RigidBody.linearVelocity = force * Time.deltaTime * 1000.0f;
    }

    private void AirPlayerMove()
    {
        // 空中：左右に力を加える
        Vector3 force = new Vector3(moveInput, 0f, 0f) * airMoveForce;
        RigidBody.AddForce(force, ForceMode.Acceleration);

        // 最大空中速度を制限
        Vector3 horizontalVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, 0f);
        if (horizontalVelocity.magnitude > maxAirSpeed)
        {
            RigidBody.linearVelocity = new Vector3(Mathf.Sign(RigidBody.linearVelocity.x) * maxAirSpeed, RigidBody.linearVelocity.y, RigidBody.linearVelocity.z);
        }
    }

    private void Jump(float jumpPower)
    {
        RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0, RigidBody.linearVelocity.z);

        // ジャンプの速度をアニメーションカーブから取得
        float time = jumpTime / jumpTimeMax;
        float power = jumpPower * jumpCurve.Evaluate(time);

        if (time >= 1)
        {
            jumping = false;
            jumpTime = 0;
        }

        RigidBody.AddForce(power * Vector3.up, ForceMode.Impulse);

        // 最大ジャンプ速度を制限
        Vector3 horizontalVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, 0f);
        if (horizontalVelocity.magnitude > maxJumpSpeed)
        {
            RigidBody.linearVelocity = new Vector3(Mathf.Sign(RigidBody.linearVelocity.x) * maxJumpSpeed, RigidBody.linearVelocity.y, RigidBody.linearVelocity.z);
        }
    }

    private void HighJump()
    {
        RigidBody.AddForce(highJumpPower * Vector3.up, ForceMode.Impulse);

        highJump = false;
    }
}
