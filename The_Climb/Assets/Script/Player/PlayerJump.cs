using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    Rigidbody RigidBody;
    PlayerState state;
    PlayerMove move;
    PlayerSpecialAction special;

    public bool jumping = false;        //ジャンプ入力中判定
    private float coyoteTime = 0.05f;    //コヨーテタイム
    public float coyoteCounter = 0f;    //コヨーテタイムカウント
    private float jumpCoolTime = 0.06f;  //ジャンプのクールタイム
    private float jumpCoolCounter = 0f;  //ジャンのクールタイムカウント
    public bool jumpCoolActive = false;  //ジャンクールタイムを始める用判定
    private float jumpTime;              //ジャンプ入力時間
    private float jumpTimeMax = 0.15f;    //最大ジャンプ入力時間
    private float jumpTimeMaxSaving = 0.15f;  //最大ジャンプ入力時間を保持
    private float groundJumpPower = 14f;  //ジャンプでプレイヤーにかかる上方向の力
    private float maxJumpSpeed = 12f;    //空中での速度制限
    [SerializeField] AnimationCurve jumpCurve = new();  //ジャンプ時の速度カーブ

    public int landingJumpNumber = 0;   //着地ジャンプの連続回数
    private float landingLowJumpPower = 16f;  //一回目着地ジャンプのパワー
    public float landingHighJumpPower = 18f;  //二回目着地ジャンプのパワー

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
        move = gameObject.GetComponent<PlayerMove>();
        special = gameObject.GetComponent<PlayerSpecialAction>();
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

            if (jumpCoolCounter > jumpCoolTime)
            {
                jumpCoolActive = false;
            }
        }
    }

    private void FixedUpdate()
    {
        //ジャンプ
        if (jumping)
        {
            jumpTime += Time.fixedDeltaTime;

            if (landingJumpNumber >= 2)
            {
                Jump(landingHighJumpPower);
                state.PlayerAnimation.animator.SetTrigger("JumpAnimStep3");
            }
            else if (landingJumpNumber == 1)
            {
                Jump(landingLowJumpPower);
                state.PlayerAnimation.animator.SetTrigger("JumpAnimStep2");
            }
            else
            {
                Jump(groundJumpPower);
                state.PlayerAnimation.animator.SetTrigger("JumpAnimStep1");
            }
        }
    }

    private void JumpOperation()
    {
        if ((coyoteCounter <= coyoteTime || state.isJumpMoveOK) && !jumpCoolActive && special.highJumpChargeCounter < special.highJumpChargeTime)
        {
            if (Input.GetKeyDown(state.keyBind.playerJump) && !special.meteorHighJumpOK)
            {
                jumping = true;
                jumpCoolActive = true;
                jumpTimeMax = jumpTimeMaxSaving;

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
                    special.meteorHighJump = true;
                    landingJumpNumber++;
                }
                special.meteorHighJumpOK = false;
                state.LandingJumpReset();
            }
        }

        if (jumping)
        {
            if (Input.GetKeyUp(state.keyBind.playerJump) && jumpTime <= jumpTimeMaxSaving * 2 / 3)
            {
                jumpTimeMax = jumpTimeMaxSaving * 2 / 3;
            }
        }
    }

    public void Jump(float jumpPower)
    {
        Debug.Log("jannpu-");
        RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0, RigidBody.linearVelocity.z);

        // ジャンプの速度をアニメーションカーブから取得
        float time = jumpTime / jumpTimeMaxSaving;
        float power = jumpPower * jumpCurve.Evaluate(time);

        if (jumpTime >= jumpTimeMax)
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
}
