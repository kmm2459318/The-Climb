using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    Rigidbody RigidBody;
    PlayerState state;
    PlayerMove move;
    PlayerSpecialAction special;
    private PlayerAnimation playerAnimation;


    public bool jumping = false;        //ジャンプ入力中判定
    private float coyoteTime = 0.05f;    //コヨーテタイム
    public float coyoteCounter = 0f;    //コヨーテタイムカウント
    private float jumpCoolTime = 0.06f;  //ジャンプのクールタイム
    private float jumpCoolCounter = 0f;  //ジャンのクールタイムカウント
    public bool jumpCoolActive = false;  //ジャンクールタイムを始める用判定
    private bool isJumpQueued = false;   //ジャンプキーが押された判定
    private float jumpQueueTime = 0.2f;  //ジャンプ先行入力猶予時間
    private float jumpQueueCounter = 0f;  //ジャンプ先行入力カウンター
    private float jumpTime;              //ジャンプ入力時間
    private float jumpTimeMax = 0.2f;    //最大ジャンプ入力時間
    private float jumpTimeMaxSaving = 0.2f;  //最大ジャンプ入力時間を保持
    private float groundJumpPower = 11f;  //ジャンプでプレイヤーにかかる上方向の力
    private float maxJumpSpeed = 12f;    //空中での速度制限
    [SerializeField] AnimationCurve jumpCurve = new();  //ジャンプ時の速度カーブ

    public int landingJumpNumber = 0;   //着地ジャンプの連続回数
    private float landingLowJumpPower = 13f;  //一回目着地ジャンプのパワー
    private float landingHighJumpPower = 15f;  //二回目着地ジャンプのパワー

    public  bool  isOnTrampoline      = false; //トランポリンに乗っているかの判定
    public  float TrampolinePower     = 1.5f;  //トランポリンのジャンプ倍率
    private float TrampolineGraceTime = 0.15f; //トランポリンの効果を維持する時間
    private float TrampolineTimer     = 0f;    //トランポリンの効果を管理するタイマー
    private bool  TrampolineJumping   = false; //トランポリンのジャンプ中判定

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
        move = gameObject.GetComponent<PlayerMove>();
        special = gameObject.GetComponent<PlayerSpecialAction>();
        playerAnimation = GetComponent<PlayerAnimation>();

    }

    void Update()
    {
        //ジャンプキー操作
        JumpOperation();

        //ジャンプのクールタイム
        if (jumpCoolActive)
        {
            jumpCoolCounter += Time.deltaTime;
            state.isGrounded = false;
            state.isJumpMoveOK = false;
            //Debug.Log(jumpCoolCounter);

            if (jumpCoolCounter > jumpCoolTime)
            {
                jumpCoolActive = false;
                jumpCoolCounter = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        //ジャンプ
        if (jumping)
        {
            jumpTime += Time.fixedDeltaTime;
            float JumpPower;

            special.headingAttack.SetActive(true);

            if (landingJumpNumber >= 2)
            {
                JumpPower = landingHighJumpPower;
            }
            else if (landingJumpNumber == 1)
            {
                JumpPower = landingLowJumpPower;
            }
            else
            {
                JumpPower = groundJumpPower; 
            }

            //トランポリンに乗っていたらトランポリンの効果を反映
            if (isOnTrampoline)
            {
                TrampolineJumping = true;
                TrampolineTimer = TrampolineGraceTime;
            }

            Jump(JumpPower);
        }

        //トランポリン効果のタイマー
        if(TrampolineJumping)
        {
            TrampolineTimer -= Time.fixedDeltaTime;
            if (TrampolineTimer <= 0)
            {
                TrampolineJumping = false;
            }
        }
    }

    private void JumpOperation()
    {
        //ジャンプキー押された
        if (Input.GetKeyDown(state.keyBind.playerJump) && !special.meteorHighJumpOK && !isJumpQueued)
        {
            isJumpQueued = true;
            jumpQueueCounter = 0f;
        }

        //ジャンプの判定を開始させる
        if ((coyoteCounter <= coyoteTime || state.isJumpMoveOK) && !jumpCoolActive && special.highJumpChargeCounter < special.highJumpChargeTime)
        {
            //通常ジャンプと着地ジャンプ
            if (isJumpQueued)
            {
                jumping = true;
                jumpCoolActive = true;
                jumpTime = 0f;
                jumpTimeMax = jumpTimeMaxSaving;
                //Debug.Log(RigidBody.linearVelocity.y);
                //Debug.Log("true後"+special.headingAttack);

                //着地ジャンプ
                if (state.landingJumpOn)
                {
                    landingJumpNumber++;
                    state.LandingJumpReset();
                }
            }
            else if (Input.GetKey(state.keyBind.playerJump) && special.meteorHighJumpOK && state.landingJumpOn)  //メテオドロップからのハイジャンプ
            {
                if (special.meteorDropCounter >= special.meteorDropTime)
                {
                    jumpCoolActive = true;
                    special.meteorHighJump = true;
                    landingJumpNumber++;
                    special.headingAttack.SetActive(true);
                }
                special.meteorHighJumpOK = false;
                state.LandingJumpReset();
            }
        }

        //ジャンプボタンが押され続けてる
        if (jumping)
        {
            if (Input.GetKeyUp(state.keyBind.playerJump) && jumpTime <= jumpTimeMaxSaving * 1 / 2)
            {
                jumpTimeMax = jumpTimeMaxSaving * 1 / 2;
            }
        }

        //ジャンプ先行入力のカウント
        if (isJumpQueued)
        {
            jumpQueueCounter += Time.deltaTime;
            
            if (jumpQueueCounter > jumpQueueTime)
            {
                isJumpQueued = false;
            }
        }
    }

    public void Jump(float jumpPower)
    {
        Debug.Log($"[Jump] Power: {jumpPower}, Time: {jumpTime}, Max: {jumpTimeMax}");
        RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0, RigidBody.linearVelocity.z);



        //トランポリンに乗っている場合ジャンプ力を上げる
        if(TrampolineJumping)
        {
            jumpPower *= TrampolinePower;
        }

        // ジャンプの速度をアニメーションカーブから取得
        float time = jumpTime / jumpTimeMaxSaving;
        float power = jumpPower * jumpCurve.Evaluate(time);

        if (jumpTime >= jumpTimeMax)
        {
            jumping = false;
        }

        RigidBody.AddForce(power * Vector3.up, ForceMode.Impulse);

        // 最大ジャンプ速度を制限
        Vector3 horizontalVelocity = new Vector3(RigidBody.linearVelocity.x, 0f, 0f);
        if (horizontalVelocity.magnitude > maxJumpSpeed)
        {
            RigidBody.linearVelocity = new Vector3(Mathf.Sign(RigidBody.linearVelocity.x) * maxJumpSpeed, RigidBody.linearVelocity.y, RigidBody.linearVelocity.z);
        }

    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         