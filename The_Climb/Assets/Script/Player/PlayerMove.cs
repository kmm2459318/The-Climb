using System.Linq;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody RigidBody;
    PlayerState state;
    PlayerJump jump;
    PlayerSpecialAction special;

    public bool highJumpOn = false;      //ハイジャンプ可能か
    public bool quickJumpOn = false;     //クイックジャンプ可能か
    public bool meteorDropOn = false;    //メテオドロップ叶か

    private float groundMoveForce = 0.35f;     //プレイヤーの地上移動速度
    public float groundMaxSpeed = 6.459797f;   //プレイヤーの地上最高速度記憶
    private float moveInput = 0f;        //プレイヤーの移動方向
    private float airMoveForce = 60f;    //空中での移動速度
    public float maxAirSpeed = 10f;     //空中での速度制限

    public bool slipping = false;        //着地後勢い止めず滑ってる判定
    private float slippingTime = 0.05f;     //スリップ方向切り替え用
    private float slippingCounter = 0f;  //スリップ方向切り替えようタイム
    public Vector3 slipVelocity;                //滑り時のVelocity

    public float MoveInput => moveInput; // ←読み取り専用プロパティ
    public PlayerAnimation PlayerAnimation;
    public PlayerState State => state;


    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
        jump = gameObject.GetComponent<PlayerJump>();
        special = gameObject.GetComponent<PlayerSpecialAction>();

        PlayerAnimation = GameObject.Find("pico_chan_chr_pico_00").GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        //移動キー操作
        if (!special.meteorDrop)
        {
            MoveOperation();
        }
    }

    void FixedUpdate()
    {
        //移動
        if (special.highJumpChargeCounter == 0f)
        {
            if (state.isGrounded || (!state.isGrounded && state.isJumpMoveOK && !state.isLeftWall && !state.isRightWall))
            {
                jump.coyoteCounter = 0f;

                //プレイヤー地上の移動
                GroundPlayerMove();
            }
            else
            {
                jump.coyoteCounter += Time.fixedDeltaTime;

                //プレイヤー空中の移動
                AirPlayerMove();
            }
        }
    }

    private void MoveOperation()
    {
        if (Input.GetKey(state.keyBind.playerLMove) && Input.GetKey(state.keyBind.playerRMove) ||
            !Input.GetKey(state.keyBind.playerLMove) && !Input.GetKey(state.keyBind.playerRMove))  //止まる
        {
            moveInput = 0f;
        }
        else if (Input.GetKey(state.keyBind.playerLMove) && !state.isLeftWall)  //左移動
        {
            moveInput = -1f;
            state.playerDirectionRight = false;
        }
        else if (Input.GetKey(state.keyBind.playerRMove) && !state.isRightWall)  //右移動
        {
            moveInput = 1f;
            state.playerDirectionRight = true;
        }
        else
        {
            moveInput = 0f;
        }
    }

    private void GroundPlayerMove()
    {
        if (slipping)
        {
            //減少時間
            float slipFrictionX = 20f;

            if (moveInput == 1f)
            {
                //横速度だけ徐々に減衰させRU
                slipVelocity.x = Mathf.MoveTowards(Mathf.Abs(slipVelocity.x), 0f, slipFrictionX * Time.fixedDeltaTime);
                slippingCounter = 0;
            }
            else if (moveInput == -1f)
            {
                //横速度だけ徐々に減衰させRU
                slipVelocity.x = Mathf.MoveTowards(Mathf.Abs(slipVelocity.x) * -1.0f, 0f, slipFrictionX * Time.fixedDeltaTime);
                slippingCounter = 0;
            }
            else if (slippingCounter > slippingTime)
            {
                slipping = false;
            }
            else if (moveInput == 0f)
            {
                slipVelocity.x = Mathf.MoveTowards(slipVelocity.x, 0f, slipFrictionX * Time.fixedDeltaTime);
                slippingCounter += Time.fixedDeltaTime;
            }

            RigidBody.linearVelocity = new Vector3(slipVelocity.x, 0, 0);

            //一定以下になったらスリップ終了（普通の地上移動に戻す）
            if (Mathf.Abs(slipVelocity.x) <= groundMaxSpeed)
            {
                slipping = false;
            }
            return; //通常の地上移動処理はスキップ
        }

        if (moveInput != 0f)
        {
            // 地上：慣性なし、即応する左右移動
            Vector3 force = new Vector3(moveInput, 0f, 0f) * groundMoveForce;
            RigidBody.AddForce(force);
            RigidBody.linearVelocity = new Vector3(force.x * Time.deltaTime * 1000.0f, RigidBody.linearVelocity.y, 0f);
        }
        else
        {
            RigidBody.linearVelocity = new Vector3(0f, RigidBody.linearVelocity.y, 0f);
        }
    }

    private void AirPlayerMove()
    {
        // 空中：左右に力を加える
        Vector3 force = new Vector3(moveInput, 0f, 0f) * airMoveForce;
        switch (jump.landingJumpNumber)
        {
            case 0:
                {
                    break;
                }
            case 1:
                {
                    force *= 1.2f;
                    break;
                }
            default:
                {
                    force *= 1.4f;
                    break;
                }
        }
        RigidBody.AddForce(force, ForceMode.Acceleration);

        // 最大空中速度を制限
        if (!special.quickJumpUsed)
        {
            maxAirSpeed = 10f;
        }
        Vector3 horizontalVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, 0f);
        if (horizontalVelocity.magnitude > maxAirSpeed)
        {
            RigidBody.linearVelocity = new Vector3(Mathf.Sign(RigidBody.linearVelocity.x) * maxAirSpeed, RigidBody.linearVelocity.y, RigidBody.linearVelocity.z);
        }

        //徐々に遅くするよ
        if (maxAirSpeed > 10f) 
        {
            Debug.Log(maxAirSpeed);
            maxAirSpeed -= 0.14f;
        }
    }
}
