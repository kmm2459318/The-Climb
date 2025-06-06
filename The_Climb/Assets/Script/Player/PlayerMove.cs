using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody RigidBody;    
    private float moveSpeed = 3.0f;     //プレイヤーの地上移動速度
    private float moveInput = 0f;       //プレイヤーの移動方向
    private Vector3 move;               //プレイヤーの地上移動
    private float airMoveForce = 23f;   //空中での移動速度
    private float maxAirSpeed = 6f;     //空中での速度制限
    private bool jumping = false;       //ジャンプ入力中判定
    private bool jumpStarted = false;   //ジャンプスタート判定
    private float jumpTime;             //ジャンプ入力時間
    private float jumpTimeMax = 0.5f;   //最大ジャンプ入力時間
    private float jumpPower = 5f;       //ジャンプでプレイヤーにかかる上方向の力
    [SerializeField] AnimationCurve jumpCurve = new();  //ジャンプ時の速度カーブ

    
    public LayerMask groundLayer;            //地面レイヤー
    public Transform groundCheck;            //プレイヤー足元の地面判定用オブジェクト
    public bool isGrounded;                  //地面判定
    private float groundCheckRadius = 0.2f;  //地面判定の半径

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //移動
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) ||
            !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))  //止まる
        {
            moveInput = 0f;
        }
        else if (Input.GetKey(KeyCode.A))  //左移動
        {
            moveInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D))  //右移動
        {
            moveInput = 1f;
        }

        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumping = true;
            jumpStarted = true;
        }

        if (jumping)
        {
            if (Input.GetKeyUp(KeyCode.Space) || jumpTime >= jumpTimeMax)
            {
                jumping = false;
                jumpTime = 0;
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                jumpTime += Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        // 地面判定（円形）
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        //移動
        if (isGrounded)
        {
            // 地上：慣性なし、即応する左右移動
            move = new Vector3(moveInput, 0f, 0f) * moveSpeed * Time.fixedDeltaTime;
            RigidBody.MovePosition(RigidBody.position + move);
        }
        else
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

        //ジャンプ
        if (jumping)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (jumpStarted)
        {
            jumpStarted = false;
            RigidBody.linearVelocity = new Vector3(move.x, 0, move.z);
        }
        else
        {
            RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0, RigidBody.linearVelocity.z);
        }

        // ジャンプの速度をアニメーションカーブから取得
        float time = jumpTime / jumpTimeMax;
        float power = jumpPower * jumpCurve.Evaluate(time);

        if (time >= 1)
        {
            jumping = false;
            jumpTime = 0;
        }

        RigidBody.AddForce(power * Vector3.up, ForceMode.Impulse);
    }
}
