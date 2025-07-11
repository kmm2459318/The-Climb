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
    public GameObject quickJumpAttack;

    public float highJumpChargeTime = 0.8f;  //ハイジャンプのチャージ時間
    public float highJumpChargeCounter = 0f;  //ハイジャンプのチャージカウンター
    private bool highJump = false;       //ハイジャンプする判定
    private float highJumpPower = 30f;   //ハイジャンプのパワー
    public bool highJumpUsed = false;   //ハイジャンプを使用したか判定

    private bool quickJump = false;      //クイックジャンプする判定
    public bool isGroundNear = false;   //地面が近いとクイックジャンプ発動させない用
    private float quickJumpPowerX = 10f;  //クイックジャンプの横のパワー
    private float quickJumpPowerY = 10f;  //クイックジャンプの縦のパワー
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

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerState>();
        move = gameObject.GetComponent<PlayerMove>();
        jump = gameObject.GetComponent<PlayerJump>();

        headingAttack = transform.Find("HeadingAttack").gameObject;
        meteorDropAttack = transform.Find("MeteorDropAttack").gameObject;
        quickJumpAttack = transform.Find("QuickJumpAttack").gameObject;

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

        //地面が近いか（クイックジャンプ用判定）
        if (!isGroundNear && RigidBody.linearVelocity.y < 0)
        {
            isGroundNear = Physics.CheckSphere(state.jumpMoveOKCheck.position + Vector3.down * 0.4f, 0.19f, state.groundLayer);
        }
        else if (state.isAir && isGroundNear)
        {
            isGroundNear = false;
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
        if (jump.jumpCoolActive || state.isAir || (Input.GetKeyUp(state.keyBind.playerJump) && highJumpChargeCounter <= 0.2f)) //ハイジャンプ不可
        {
            highJumpChargeCounter = 0f;
        }
        else if (Input.GetKeyUp(state.keyBind.playerJump))　//ハイジャンプ放す
        {
            if (highJumpChargeCounter >= highJumpChargeTime)
            {
                highJump = true;
                highJumpUsed = true;
                headingAttack.SetActive(true);
            }
            //else
            //{
            //    jump.jumping = true;
            //}

            jump.jumpCoolActive = true;
            highJumpChargeCounter = 0f;
        }
        else if (state.isGrounded && Input.GetKey(state.keyBind.playerJump) && !state.landingJumpOn && !Input.GetKeyDown(state.keyBind.playerJump)) //ハイジャンプおしっぱの状態
        {
            RigidBody.linearVelocity = new Vector3(0, RigidBody.linearVelocity.y, 0);
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
        if (state.isAir && Input.GetKeyDown(state.keyBind.playerJump) && !quickJumpUsed && !meteorDropUsed && !isGroundNear)
        {
            quickJump = true;
            quickJumpUsed = true;
            quickJumpAttack.SetActive(true);
        }
        
        //横移動入力中ならジャンプ力低下
        if (move.MoveInput == 1f || move.MoveInput == -1f)
        {
            quickJumpPowerY = 7f;
        }
        else
        {
            quickJumpPowerY = 12f;
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
        meteorDropCounter += Time.fixedDeltaTime;

        //壁にぶつかったらメテオハイジャンプ不可
        if (((state.isLeftWall && !state.playerDirectionRight) || (state.isRightWall && state.playerDirectionRight)) && !state.isGrounded)
        {
            meteorHighJumpOK = false;
            RigidBody.linearVelocity = Vector3.zero;
        }
        else
        {
            //斜め下に移動させる
            RigidBody.AddForce(meteorDropPower * new Vector3(meteorDropXMove, meteorDropYMove, 0), ForceMode.Impulse);
        }

        //メテオドロップ終わり
        if ((state.isLeftWall && !state.playerDirectionRight) || (state.isRightWall && state.playerDirectionRight) || state.isGrounded)
        {
            RigidBody.useGravity = true;
            meteorDrop = false;
            RigidBody.linearVelocity = Vector3.zero;
        }
    }

    private void QuickJumpUse()
    {
        RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0, 0);
        RigidBody.AddForce(new Vector3(quickJumpPowerX * move.MoveInput, quickJumpPowerY, 0), ForceMode.Impulse);
        
        //クイックジャンプの横移動速度制限
        if (move.MoveInput != 0f)
        {
            move.maxAirSpeed = 15f;
        }
        quickJump = false;
    }

    private void MeteorHighJumpUse()
    {
        RigidBody.linearVelocity = new Vector3(0, 0, 0);

        RigidBody.AddForce(new Vector3(meteorDropPower * meteorDropXMove, highJumpPower, 0), ForceMode.Impulse);

        meteorHighJump = false;
    }
}
