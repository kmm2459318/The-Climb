using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpecialAction : MonoBehaviour
{
    Rigidbody RigidBody;
    PlayerState state;
    PlayerMove move;
    PlayerJump jump;

    public GameObject headingAttack;
    public GameObject meteorDropAttack;

    public float highJumpChargeTime = 0.8f;  //ハイジャンプのチャージ時間
    public float highJumpChargeCounter = 0f;  //ハイジャンプのチャージカウンター
    private bool highJump = false;       //ハイジャンプする判定
    private float highJumpPower = 30f;   //ハイジャンプのパワー
    public bool highJumpUsed = false;   //ハイジャンプを使用したか判定

    private bool quickJump = false;      //クイックジャンプする判定
    private float quickJumpPowerX = 10f;  //クイックジャンプの横のパワー
    private float quickJumpPowerY = 5f;  //クイックジャンプの縦のパワー
    public bool quickJumpUsed = false;   //クイックジャンプを使用したか判定

    public bool meteorDrop = false;      //メテオドロップする判定
    public bool meteorDropUsed = false;   //メテオドロップを使用したか判定
    public bool meteorHighJumpOK = false;  //メテオドロップからのハイジャンプへの移行ができるか
    private float meteorDropPower = 30f;  //メテオドロップのパワー
    private float meteorDropAngle = 135f;  //メテオドロップの角度
    private float meteorDropXMove;        //メテオドロップのX軸移動
    private float meteorDropYMove;        //メテオドロップのY軸移動
    public bool meteorHighJump = false;  //メテオドロップ後のハイジャンプ
    public float meteorDropTime = 0.37f;  //メテオドロップからのハイジャンプに移行できるまでの時間
    public float meteorDropCounter = 0f;  //メテオドロップのカウンター

    public bool playHighJumpAnim = false; // このフレームでアニメーション再生
    public bool isHighJumpCharging = false;
    internal bool isChargeInsufficient;

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
        move = gameObject.GetComponent<PlayerMove>();
        jump = gameObject.GetComponent<PlayerJump>();

        headingAttack = transform.Find("HeadingAttack").gameObject;
        meteorDropAttack = transform.Find("MeteorDropAttack").gameObject;

        float meteorDropDirection = meteorDropAngle * Mathf.Deg2Rad;

        meteorDropXMove = Mathf.Sin(meteorDropDirection);
        meteorDropYMove = Mathf.Cos(meteorDropDirection);
    }

    void Update()
    {
        if (state.highJumpOn)
        {
            //チャージジャンプのチャージキー操作
            HighJumpChargeOperation();
        }

        if (state.meteorDropOn)
        {
            //メテオドロップキー操作
            MeteorDropOperation();
        }

        if (state.quickJumpOn)
        {
            //クイックジャンプキー操作
            QuickJumpOperation();
        }
    }

    private void FixedUpdate()
    {
        //ハイジャンプ実行
        if (highJump)
        {
            HighJumpUse();
        }

        if (meteorDrop)
        {
            MeteorDropUse();
        }

        if (meteorHighJump)
        {
            MeteorHighJumpUse();
        }

        if (quickJump)
        {
            QuickJumpUse();
        }

    }

    private void HighJumpChargeOperation()
    {
        if (jump.jumpCoolActive || state.isAir || (Input.GetKeyUp(state.keyBind.playerJump) && highJumpChargeCounter <= 0.2f))
        {
            highJumpChargeCounter = 0f;
        }
        else if (Input.GetKeyUp(state.keyBind.playerJump))
        {
            if (highJumpChargeCounter >= highJumpChargeTime)
            {
                highJump = true;
                //Debug.Log(RigidBody.linearVelocity.y);

                playHighJumpAnim = true;

                highJumpUsed = true;
                headingAttack.SetActive(true);
            }
            else
            {
                jump.jumping = true;
            }

            jump.jumpCoolActive = true;
            highJumpChargeCounter = 0f;
        }
        else if (state.isGrounded && Input.GetKey(state.keyBind.playerJump))
        {
            highJumpChargeCounter += Time.deltaTime;
            move.slipping = false;
        }
        
    }

    private void MeteorDropOperation()
    {
        if (state.isAir && Input.GetKey(state.keyBind.meteorDrop) && Input.GetKeyDown(state.keyBind.playerJump) && !meteorDropUsed)
        {
            meteorDrop = true;
            meteorDropUsed = true;
            meteorHighJumpOK = true;
            jump.landingJumpNumber = 0;
            meteorDropAttack.SetActive(true);

            if (state.playerDirectionRight)
            {
                meteorDropXMove = Mathf.Abs(meteorDropXMove);
            }
            else
            {
                meteorDropXMove = Mathf.Abs(meteorDropXMove) * -1f;
            }
        }
        
    }

    private void QuickJumpOperation()
    {
        if (state.isAir && Input.GetKeyDown(state.keyBind.playerJump) && !quickJumpUsed && !meteorDropUsed)
        {
            quickJump = true;
            quickJumpUsed = true;
        }
        
        //横移動入力中ならジャンプ力低下
        if (move.MoveInput == 1f || move.MoveInput == -1f)
        {
            quickJumpPowerY = 9f;
        }
        else
        {
            quickJumpPowerY = 15f;
        }
    }

    public void HighJumpUse()
    {
        RigidBody.AddForce(new Vector3(RigidBody.linearVelocity.x, highJumpPower, 0), ForceMode.Impulse);
        jump.jumpCoolActive = true;

        highJump = false;
    }

    private void MeteorDropUse()
    {
        RigidBody.useGravity = false;
        RigidBody.linearVelocity = new Vector3(0, 0, 0);

        RigidBody.AddForce(meteorDropPower * new Vector3(meteorDropXMove, meteorDropYMove, 0), ForceMode.Impulse);

        meteorDropCounter += Time.fixedDeltaTime;

        if ((state.isLeftWall || state.isRightWall) && !state.isGrounded)
        {
            meteorHighJumpOK = false;
        }

        //メテオドロップ終わり
        if (state.isLeftWall || state.isRightWall || state.isGrounded)
        {
            RigidBody.useGravity = true;
            meteorDrop = false;
        }
    }

    private void QuickJumpUse()
    {
        RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0, 0);
        RigidBody.AddForce(new Vector3(quickJumpPowerX * move.MoveInput, quickJumpPowerY, 0), ForceMode.Impulse) ;

        quickJump = false;
    }

    private void MeteorHighJumpUse()
    {
        RigidBody.linearVelocity = new Vector3(0, 0, 0);

        RigidBody.AddForce(new Vector3(meteorDropPower * meteorDropXMove, highJumpPower, 0), ForceMode.Impulse);

        meteorHighJump = false;
    }
}
