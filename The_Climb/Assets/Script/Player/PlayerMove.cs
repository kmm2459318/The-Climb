using System.Linq;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody RigidBody;    
    private float groundMoveForce = 0.25f;     //プレイヤーの地上移動速度
    private float moveInput = 0f;       //プレイヤーの移動方向
    private float airMoveForce = 90f;   //空中での移動速度
    private float maxAirSpeed = 10f;     //空中での速度制限
    private bool jumping = false;       //ジャンプ入力中判定
    private float jumpTime;             //ジャンプ入力時間
    private float jumpTimeMax = 0.1f;   //最大ジャンプ入力時間
    private float jumpPower = 15f;       //ジャンプでプレイヤーにかかる上方向の力
    private float maxJumpSpeed = 100f;     //空中での速度制限
    [SerializeField] AnimationCurve jumpCurve = new();  //ジャンプ時の速度カーブ

    public Transform groundCheck;       //プレイヤー足元の地面判定用オブジェクト
    public bool isGrounded;             //地面判定
    public Transform jumpOKCheck;       //プレイヤー足元のジャンプ判定用オブジェクト
    public bool isJumpOK;               //ジャンプOK判定
    public Transform leftWallCheck;     //プレイヤー足元の左壁判定用オブジェクト
    public bool isLeftWall;             //左壁判定
    public Transform rightWallCheck;    //プレイヤー足元の右壁判定用オブジェクト
    public bool isRightWall;            //右壁判定
    public LayerMask groundLayer;       //地面レイヤー
    private float groundCheckRadius = 0.001f;  //地面判定の半径

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -50.6F, 0); // Gを倍にする
    }

    private void Update()
    {
        //移動
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) ||
            !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))  //止まる
        {
            moveInput = 0f;
        }
        else if (Input.GetKey(KeyCode.A) && !isLeftWall)  //左移動
        {
            moveInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D) && !isRightWall)  //右移動
        {
            moveInput = 1f;
        }
        else 
        {
            moveInput = 0f;
        }

        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && isJumpOK)
        {
            jumping = true;
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
        Vector3[] floorOffsets = new Vector3[] {
            Vector3.zero,
            Vector3.left * 0.499f,
            Vector3.right * 0.499f
        };
        Vector3[] wallOffsets = new Vector3[] {
            Vector3.zero,
            Vector3.up * 0.49f,
            Vector3.down * 0.49f
        };

        // 地面判定（円形）
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = floorOffsets.Any(offset =>
            Physics.CheckCapsule(groundCheck.position + Vector3.left * 0.499f, groundCheck.position + Vector3.right * 0.499f, groundCheckRadius, groundLayer)
        );
        // ジャンプOK判定（円形）
        isJumpOK = floorOffsets.Any(offset =>
            Physics.CheckCapsule(jumpOKCheck.position + Vector3.left * 0.299f, jumpOKCheck.position + Vector3.right * 0.299f, 0.2f, groundLayer)
        );
        // 左壁判定（円形）
        isLeftWall = wallOffsets.Any(offset =>
            Physics.CheckCapsule(leftWallCheck.position + Vector3.up * 0.49f, leftWallCheck.position + Vector3.down * 0.49f, groundCheckRadius, groundLayer)
        );
        // 右壁判定（円形）
        isRightWall = wallOffsets.Any(offset =>
            Physics.CheckCapsule(rightWallCheck.position + Vector3.up * 0.49f, rightWallCheck.position + Vector3.down * 0.49f, groundCheckRadius, groundLayer)
        );

        //移動
        if (isGrounded)
        {
            //プレイヤー地上の移動
            GroundPlayerMove();
        }
        else
        {
            //プレイヤー空中の移動
            AirPlayerMove();
        }

        //ジャンプ
        if (jumping)
        {
            Jump();
        }

        //壁に当たったらジャンプが止まる
        //if (jumping && (isLeftWall || isRightWall))
        //{
        //    jumping = false;
        //    RigidBody.linearVelocity = new Vector3(RigidBody.linearVelocity.x, 0, RigidBody.linearVelocity.z);
        //}
    }

    private void GroundPlayerMove() 
    {
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

    private void Jump()
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
}
